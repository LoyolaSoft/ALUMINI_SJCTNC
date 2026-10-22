using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.Fee
{

    public static class FeeSQL
    {
        public static string GetFeeSQL(FeeSQLCommands sEnumCommand)
        {
            string query = "";
            switch (sEnumCommand)
            {
                case FeeSQLCommands.FetchRazorPayOrderInfoByPaidStatus:
                    {
                        query = @"SELECT 
                                    O.ID, O.AMOUNT
                                FROM
                                    FEE_RAZORPAY_ORDER_INFO_?AC_YEAR AS O
                                        LEFT JOIN
                                    FEE_RAZORPAY_PAYMENT_INFO_?AC_YEAR AS P ON P.ORDER_ID = O.ID
                                WHERE
                                    O.IS_DELETED != 1 AND O.STATUS = 'PAID'
                                        AND P.ID IS NULL;";
                        break;
                    }
                case FeeSQLCommands.FetchRazorPaySettlementInfoById:
                    {
                        query = @"SELECT `SETTLEMENT_ID`,
                                    `ID`,
                                    `ENTITY`,
                                    `AMOUNT`,
                                    `STATUS`,
                                    `FEES`,
                                    `TAX`,
                                    `UTR`,
                                    `CREATED_AT`,
                                    `RECIPIENT`
                                FROM `FEE_RAZORPAY_SETTLEMENT_INFO_?AC_YEAR` WHERE ID=?ID;";
                        break;
                    }
                case FeeSQLCommands.FetchRazorPayOrderInfoForUpdate:
                    {
                        query = @"SELECT 
                                    RAZORPAY_ORDER_ID, ID
                                FROM
                                    FEE_RAZORPAY_ORDER_INFO_?AC_YEAR AS O
                                WHERE
                                    STATUS = 'CREATED';";

                        break;
                    }
                case FeeSQLCommands.FetchRazorPayTransfersForSettlemetUpdate:
                    {
                        query = @"SELECT 
                                    RAZORPAY_TRANSFER_ID, ID
                                FROM
                                    FEE_RAZORPAY_TRANSFER_INFO_?AC_YEAR
                                WHERE
                                    RECIPIENT_SETTLEMENT_ID = '' ;";

                        break;
                    }
                case FeeSQLCommands.FetchRazorPaySettlementIds:
                    {
                        query = @"SELECT 
                                    RAZORPAY_TRANSFER_ID, ID,RECIPIENT_SETTLEMENT_ID
                                FROM
                                    FEE_RAZORPAY_TRANSFER_INFO_?AC_YEAR
                                WHERE
                                    RECIPIENT_SETTLEMENT_ID != '' GROUP BY RECIPIENT_SETTLEMENT_ID;";

                        break;
                    }
                case FeeSQLCommands.FetchRazorpayTransferListForCollegeTransactoins:
                    {
                        query = @"SELECT 
                                    FMH.FREQUENCY_ID,
                                    SP.STUDENT_ID,
                                    SUM((IFNULL(FC.PAID_AMOUNT, 0))) AS BALANCE,
                                    FF.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    SFM.MAIN_HEAD AS HEAD_NAME,
                                    SP.STU_EMAILID,
                                    SP.STU_MOBILENO,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    PR.STATUS,
                                    DATE_FORMAT(F.PAYMENT_DATE, '%d/%m/%Y') AS TRANSACTION_DATE,F.RECEIPT_NO,RA.ACCOUNT_ID,PR.ID,FMH.MAIN_HEAD_ID,FMH.BANK_ACCOUNT_ID,F.RAZORPAY_ID
                                    
                                FROM
                                    FEE_RAZORPAY_PAYMENT_INFO_?AC_YEAR AS PR
                                        INNER JOIN
                                    FEE_TRANSACTION AS F ON F.RAZORPAY_ID = PR.RAZORPAY_PAMENT_ID
                                        INNER JOIN
                                    FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = F.TRANSACTION_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = PR.UDF1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                        AND PG.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FMH ON FMH.APPLICATION_TYPE_ID = PG.APPTYPE_ID
                                        AND FMH.PROGRAMME_MODE = PG.PROGRAMME_MODE
                                        AND FMH.SHIFT = PG.SHIFT
                                        AND FC.HEAD = FMH.HEAD_ID
                                        AND FMH.ACADEMIC_YEAR =  ?AC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR =  ?AC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = F.FREQUENCY
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        AND FT.FEE_ROOT_ID = PR.UDF5
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                    LEFT JOIN fee_razorpay_accounts AS RA ON RA.BANK_ACCOUNT_ID=FMH.BANK_ACCOUNT_iD
                                WHERE
                                    PR.RAZORPAY_PAMENT_ID = ?RAZORPAY_PAMENT_ID
                                GROUP BY SP.STUDENT_ID , F.FREQUENCY , FMH.MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchPaymentids:
                    {
                        query = @"SELECT 
                                      FP.RAZORPAY_PAMENT_ID, FP.ID, FT.ID AS INVOICE_ID
                                  FROM
                                      fee_razorpay_payment_info_2020 AS FP
                                          LEFT JOIN
                                      fee_razorpay_transfer_info_2020 AS FT ON FT.SOURCE = FP.ID
                                  HAVING FT.ID IS NULL;";
                        break;
                    }
                case FeeSQLCommands.FetchRazorpayTransferListForNonCollegeTransactoins:
                    {
                        query = @"SELECT 
                                    FMH.FREQUENCY_ID,
                                    SP.STUDENT_ID,
                                    SUM((IFNULL(FC.PAID_AMOUNT, 0))) AS BALANCE,
                                    FF.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    SFM.MAIN_HEAD AS HEAD_NAME,
                                    SP.STU_EMAILID,
                                    SP.STU_MOBILENO,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    PR.STATUS,
                                    DATE_FORMAT(F.PAYMENT_DATE, '%d/%m/%Y') AS TRANSACTION_DATE,F.RECEIPT_NO,RA.ACCOUNT_ID,PR.ID,FMH.MAIN_HEAD_ID,FMH.BANK_ACCOUNT_ID,F.RAZORPAY_ID
                                FROM
                                    FEE_RAZORPAY_PAYMENT_INFO_?AC_YEAR AS PR
                                        INNER JOIN
                                    FEE_TRANSACTION AS F ON F.RAZORPAY_ID = PR.RAZORPAY_PAMENT_ID
                                        INNER JOIN
                                    FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = F.TRANSACTION_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = PR.UDF1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                        AND PG.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FMH ON F.FREQUENCY=FMH.FREQUENCY_ID
                                        AND FC.HEAD = FMH.HEAD_ID
                                        AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = F.FREQUENCY
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        AND FT.FEE_ROOT_ID = PR.UDF5
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                      LEFT JOIN fee_razorpay_accounts AS RA ON RA.BANK_ACCOUNT_ID=FMH.BANK_ACCOUNT_iD
                                WHERE
                                    PR.RAZORPAY_PAMENT_ID = ?RAZORPAY_PAMENT_ID
                                GROUP BY SP.STUDENT_ID , F.FREQUENCY , FMH.MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchRazorpayPaymentStatusForNonCollegeTransactions:
                    {
                        query = @"SELECT 
                                    FMH.FREQUENCY_ID,
                                    SP.STUDENT_ID,
                                    SUM((IFNULL(FC.PAID_AMOUNT, 0))) AS BALANCE,
                                    FF.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    SFM.MAIN_HEAD AS HEAD_NAME,
                                    SP.STU_EMAILID,
                                    SP.STU_MOBILENO,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    PR.STATUS,
                                    DATE_FORMAT(F.PAYMENT_DATE, '%d/%m/%Y') AS TRANSACTION_DATE,F.RECEIPT_NO
                                FROM
                                    FEE_RAZORPAY_PAYMENT_INFO_?AC_YEAR AS PR
                                        INNER JOIN
                                    FEE_TRANSACTION AS F ON F.RAZORPAY_ID = PR.RAZORPAY_PAMENT_ID
                                        INNER JOIN
                                    FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = F.TRANSACTION_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = PR.UDF1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                        AND PG.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FMH ON F.FREQUENCY=FMH.FREQUENCY_ID
                                        AND FC.HEAD = FMH.HEAD_ID
                                        AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = F.FREQUENCY
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        AND FT.FEE_ROOT_ID = PR.UDF5
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                WHERE
                                    PR.RAZORPAY_PAMENT_ID = ?RAZORPAY_PAMENT_ID
                                GROUP BY SP.STUDENT_ID , F.FREQUENCY , FMH.MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchRazorpayPaymentStatusForCollegeTranctions:
                    {
                        query = @"SELECT 
                                    FMH.FREQUENCY_ID,
                                    SP.STUDENT_ID,
                                    SUM((IFNULL(FC.PAID_AMOUNT, 0))) AS BALANCE,
                                    FF.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    SFM.MAIN_HEAD AS HEAD_NAME,
                                    SP.STU_EMAILID,
                                    SP.STU_MOBILENO,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    PR.STATUS,
                                    DATE_FORMAT(F.PAYMENT_DATE, '%d/%m/%Y') AS TRANSACTION_DATE,F.RECEIPT_NO
                                FROM
                                    FEE_RAZORPAY_PAYMENT_INFO_?AC_YEAR AS PR
                                        INNER JOIN
                                    FEE_TRANSACTION AS F ON F.RAZORPAY_ID = PR.RAZORPAY_PAMENT_ID
                                        INNER JOIN
                                    FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = F.TRANSACTION_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = PR.UDF1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                        AND PG.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FMH ON FMH.APPLICATION_TYPE_ID = PG.APPTYPE_ID
                                        AND FMH.PROGRAMME_MODE = PG.PROGRAMME_MODE
                                        AND FMH.SHIFT = PG.SHIFT
                                        AND FC.HEAD = FMH.HEAD_ID
                                        AND FMH.ACADEMIC_YEAR =  ?AC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR =  ?AC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = F.FREQUENCY
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        AND FT.FEE_ROOT_ID = PR.UDF5
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                WHERE
                                    PR.RAZORPAY_PAMENT_ID = ?RAZORPAY_PAMENT_ID
                                GROUP BY SP.STUDENT_ID , F.FREQUENCY , FMH.MAIN_HEAD_ID;";

                        break;
                    }
                case FeeSQLCommands.FetchTransactionByRazorpayId:
                    {
                        query = @"SELECT 
                                `TRANSACTION_ID`,
                                    `STUDENT_ID`,
                                    `FREQUENCY`,
                                    `CLASS`,
                                    `PAYMENT_DATE`,
                                    `RECEIPT_NO`,
                                    `PAYMENT_MODE`,
                                    `DD_CHEQUE_NO`,
                                    `COLLECTED`,
                                    `DISCOUNT`,
                                    `DEDUCT_STUDENT_ACCOUNT`,
                                    `RECEIPT_NARRATION`,
                                    `USERNAME`,
                                    `ACADEMIC_YEAR`,
                                    `IS_DELETED`,
                                    `IS_AMOUNT_COLLECTED`,
                                    `IS_ACCOUNTANT_COLLECTED`,
                                    `IS_ADVANCE`,
                                    `UPLOAD_FLAG`,
                                    `IS_DOWNLOADED`,
                                    `IS_UPDATED`,
                                    `DOWNLOAD_TIME`,
                                    `FEE_TRANSACTION_COUNTER`,
                                    `FEE_TRANSACTION_BANK`,
                                    `TEMP_ID`,
                                    `EXCESS_AMT`,
                                    `F_TRANSACTION_ID`,
                                    `FREQUENCY_TO`,
                                    `CHALLAN_NO`,
                                    `PayUResponse_Id`,
                                    `RAZORPAY_ID`
                                FROM `FEE_TRANSACTION` WHERE  RAZORPAY_ID=?RAZORPAY_ID AND FREQUENCY=?FREQUENCY AND ACADEMIC_YEAR=?AC_YEAR ;";
                        break;
                    }
                case FeeSQLCommands.FetchRazorpayPaymentInfoById:
                    {
                        query = @"SELECT 
                                    `RAZORPAY_PAMENT_ID`,
                                    `ID`,
                                    `ENTITY`,
                                    `AMOUNT`,
                                    `CURRENCY`,
                                    `STATUS`,
                                    `ORDER_ID`,
                                    `INVOICE_ID`,
                                    `INTERNATIONAL`,
                                    `METHOD`,
                                    `AMOUNT_REFUNDED`,
                                    `REFUND_STATUS`,
                                    `CAPTURED`,
                                    `DESCRIPTION`,
                                    `CARD_ID`,
                                    `BANK`,
                                    `WALLET`,
                                    `VPA`,
                                    `EMAIL`,
                                    `CONTACT`,
                                    `FEE`,
                                    `TAX`,
                                    `ERROR_CODE`,
                                    `ERROR_DESCRIPTION`,
                                    `CREATED_AT`,
                                    `UDF1`,
                                    `UDF2`,
                                    `UDF3`,
                                    `UDF4`,
                                    `UDF5`,
                                    `UDF6`,
                                    `IS_DELETED`
                                FROM
                                    `FEE_RAZORPAY_PAYMENT_INFO_?AC_YEAR`
                                WHERE
                                    ID = ?ID AND IS_DELETED!=1";
                        break;
                    }
                case FeeSQLCommands.SaveRazorpayPaymentInfo:
                    {
                        query = @"INSERT INTO `FEE_RAZORPAY_PAYMENT_INFO_?AC_YEAR`
                                        (
                                        `ID`,
                                        `ENTITY`,
                                        `AMOUNT`,
                                        `CURRENCY`,
                                        `STATUS`,
                                        `ORDER_ID`,
                                        `INVOICE_ID`,
                                        `INTERNATIONAL`,
                                        `METHOD`,
                                        `AMOUNT_REFUNDED`,
                                        `REFUND_STATUS`,
                                        `CAPTURED`,
                                        `DESCRIPTION`,
                                        `CARD_ID`,
                                        `BANK`,
                                        `WALLET`,
                                        `VPA`,
                                        `EMAIL`,
                                        `CONTACT`,
                                        `FEE`,
                                        `TAX`,
                                        `ERROR_CODE`,
                                        `ERROR_DESCRIPTION`,
                                        `CREATED_AT`,
                                        `UDF1`,
                                        `UDF2`,
                                        `UDF3`,
                                        `UDF4`,
                                        `UDF5`,
                                        `UDF6`
                                        )
                                        VALUES
                                        (
                                        ?ID ,
                                        ?ENTITY ,
                                        ?AMOUNT ,
                                        ?CURRENCY ,
                                        ?STATUS ,
                                        ?ORDER_ID ,
                                        ?INVOICE_ID ,
                                        ?INTERNATIONAL ,
                                        ?METHOD ,
                                        ?AMOUNT_REFUNDED ,
                                        ?REFUND_STATUS ,
                                        ?CAPTURED ,
                                        ?DESCRIPTION ,
                                        ?CARD_ID ,
                                        ?BANK ,
                                        ?WALLET ,
                                        ?VPA ,
                                        ?EMAIL ,
                                        ?CONTACT ,
                                        ?FEE ,
                                        ?TAX ,
                                        ?ERROR_CODE ,
                                        ?ERROR_DESCRIPTION ,
                                        ?CREATED_AT ,
                                        ?UDF1 ,
                                        ?UDF2 ,
                                        ?UDF3 ,
                                        ?UDF4 ,
                                        ?UDF5 ,
                                        ?UDF6  );";
                        break;
                    }
                case FeeSQLCommands.SaveRazorpayOrderInfo:
                    {
                        query = @"INSERT INTO `FEE_RAZORPAY_ORDER_INFO_?AC_YEAR`
                                (`ID`,
                                `ENTITY`,
                                `AMOUNT`,
                                `AMOUNT_PAID`,
                                `AMOUNT_DUE`,
                                `CURRENCY`,
                                `RECEIPT`,
                                `OFFER_ID`,
                                `STATUS`,
                                `ATTEMPTS`,
                                `CREATED_AT`,
                                `UDF1`,
                                `UDF2`,
                                `UDF3`,
                                `UDF4`,
                                `UDF5`)
                                VALUES
                                (
                                ?ID,
                                ?ENTITY,
                                ?AMOUNT,
                                ?AMOUNT_PAID,
                                ?AMOUNT_DUE,
                                ?CURRENCY,
                                ?RECEIPT,
                                ?OFFER_ID,
                                ?STATUS,
                                ?ATTEMPTS,
                                ?CREATED_AT,
                                ?UDF1,
                                ?UDF2,
                                ?UDF3,
                                ?UDF4,
                                ?UDF5
                                );
";
                        break;
                    }
                case FeeSQLCommands.FetchRazorpayOrderInfoByUDF:
                    {

                        query = @"SELECT 
                                    RAZORPAY_ORDER_ID,
                                    ID,
                                    ENTITY,
                                    AMOUNT,
                                    AMOUNT_PAID,
                                    AMOUNT_DUE,
                                    CURRENCY,
                                    RECEIPT,
                                    OFFER_ID,
                                    STATUS,
                                    ATTEMPTS,
                                    CREATED_AT,
                                    UDF1,
                                    UDF2,
                                    UDF3,
                                    UDF4,
                                    UDF5,
                                    UDF6,
                                    IS_ACTIVE,
                                    IS_DELETED
                                FROM
                                    FEE_RAZORPAY_ORDER_INFO_?AC_YEAR AS OI
                                WHERE
                                    UDF1 = ?UDF1 AND UDF2 = ?UDF2 AND UDF3 = ?UDF3
                                        AND UDF5 = ?UDF5 AND AMOUNT=?AMOUNT
                                ;";
                        break;
                    }
                case FeeSQLCommands.FetchRazorpayAccountInfo:
                    {
                        query = @"SELECT 
                                    RAZORPAY_ACCOUNT_ID,
                                    ACCOUNT_ID,
                                    ACCOUNT_NAME,
                                    ACCOUNT_EMAIL_ID,
                                    ACTIVATED_AT,
                                    CREATED_AT,
                                    BANK_ACCOUNT_ID,
                                    IS_ACTIVE,
                                    IS_DELETED,
                                    RAZORPAY_ACCOUNT_TYPE_ID,
                                    `KEY`,
                                    SECRET_KEY
                                FROM
                                    FEE_RAZORPAY_ACCOUNTS AS RA
                                WHERE
                                    RAZORPAY_ACCOUNT_TYPE_ID = ?RAZORPAY_ACCOUNT_TYPE_ID ";
                        break;
                    }

                case FeeSQLCommands.FetchStatusByPayUResponseId:
                    {
                        query = @"SELECT 
                                    FMH.FREQUENCY_ID,
                                    SP.STUDENT_ID,
                                    SUM((IFNULL(FC.PAID_AMOUNT, 0))) AS BALANCE,
                                    FF.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    SFM.MAIN_HEAD AS HEAD_NAME,
                                    SP.STU_EMAILID,
                                    SP.STU_MOBILENO,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    PR.STATUS,
                                    DATE_FORMAT(F.PAYMENT_DATE, '%d/%m/%Y') AS TRANSACTION_DATE,F.RECEIPT_NO
                                FROM
                                    FEE_PAYU_RESPONSE AS PR
                                        INNER JOIN
                                    FEE_TRANSACTION AS F ON F.PAYURESPONSE_ID = PR.PAYU_RESPONSE_ID
                                        INNER JOIN
                                    FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = F.TRANSACTION_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = PR.UDF1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                        AND PG.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FMH ON FMH.APPLICATION_TYPE_ID = PG.APPTYPE_ID
                                        AND FMH.PROGRAMME_MODE = PG.PROGRAMME_MODE
                                        AND FMH.SHIFT = PG.SHIFT
                                        AND FC.HEAD = FMH.HEAD_ID
                                        AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = F.FREQUENCY
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        AND FT.FEE_ROOT_ID = PR.UDF5
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                WHERE
                                    PR.PAYU_RESPONSE_ID = ?PAYU_RESPONSE_ID
                                GROUP BY SP.STUDENT_ID , F.FREQUENCY , FMH.MAIN_HEAD_ID; ";
                        break;
                    }
                case FeeSQLCommands.FetchStatusForHostelAndMessByPayUResponseId:
                    {
                        query = @"SELECT 
                                    FMH.FREQUENCY_ID,
                                    SP.STUDENT_ID,
                                    SUM((IFNULL(FC.PAID_AMOUNT, 0))) AS BALANCE,
                                    FF.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    SFM.MAIN_HEAD AS HEAD_NAME,
                                    SP.STU_EMAILID,
                                    SP.STU_MOBILENO,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    PR.STATUS,
                                    DATE_FORMAT(F.PAYMENT_DATE, '%d/%m/%Y') AS TRANSACTION_DATE,F.RECEIPT_NO
                                FROM
                                    FEE_PAYU_RESPONSE AS PR
                                        INNER JOIN
                                    FEE_TRANSACTION AS F ON F.PAYURESPONSE_ID = PR.PAYU_RESPONSE_ID
                                        INNER JOIN
                                    FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = F.TRANSACTION_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = PR.UDF1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                        AND PG.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FMH ON F.FREQUENCY=FMH.FREQUENCY_ID
                                        AND FC.HEAD = FMH.HEAD_ID
                                        AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = F.FREQUENCY
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        AND FT.FEE_ROOT_ID = PR.UDF5
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                WHERE
                                    PR.PAYU_RESPONSE_ID = ?PAYU_RESPONSE_ID
                                GROUP BY SP.STUDENT_ID , F.FREQUENCY , FMH.MAIN_HEAD_ID; ";
                        break;
                    }
                case FeeSQLCommands.FetchPayUSettleAPI:
                    {
                        query = @"SELECT 
                                    MERCHANT_ACCOUNT_INFO,
                                    ACCOUNT_ID,
                                    USERNAME,
                                    `PASSWORD`,
                                    `KEY`,
                                    SALT,
                                    BASE_URL,
                                    AUTHORIZATION,
                                    API_TYPE,
                                    HASH_SEQUENCE,
                                    BANK_ACCOUNT_ID,
                                    surl,furl,curl
                                FROM
                                    FEE_MERCHANT_ACCOUNT_INFO AS MI
                                        INNER JOIN
                                    SUP_BANK_ACCOUNT AS BA ON BA.BANK_ACCOUNT_ID = MI.ACCOUNT_ID
                                      
                                 
                                WHERE
                                    BA.ACCOUNT_TYPE = ?ACCOUNT_TYPE
                                        AND MI.IS_DELETED != 1
                                        AND BA.IS_DELETED != 1
                                        AND MI.API_TYPE = 3; ";
                        break;
                    }

                case FeeSQLCommands.FetchRefundList:
                    {
                        query = @"SELECT 
                                        PAYU_RESPONSE_ID,
                                        txnid,
                                        udf3,
                                        udf4,
                                        amount,
                                        productinfo,
                                        firstname,
                                        email,
                                        phone,
                                        bank_ref_num,
                                        bankcode,
                                        error,
                                        error_Message,
                                        name_on_card,
                                        mihpayid,
                                        de.status,
                                        unmappedstatus,
                                        addedon,fr.FREQUENCY_NAME as udf2
                                    FROM
                                        fee_payu_response_double_entry as de inner join sup_fee_frequency as fr on fr.FREQUENCY_ID = de.udf2; ";
                        break;
                    }
                case FeeSQLCommands.UpdatePayUResponse:
                    {
                        query = @"UPDATE fee_payu_response
                                                        SET
                                                        `key` = ?KEY,
                                                        txnid = ?TXNID,
                                                        amount = ?AMOUNT,
                                                        productinfo = ?PRODUCTINFO,
                                                        firstname = ?FIRSTNAME,
                                                        email = ?EMAIL,
                                                        phone = ?PHONE,
                                                        lastname = ?LASTNAME,
                                                        address1 = ?ADDRESS1,
                                                        address2 = ?ADDRESS2,
                                                        city = ?CITY,
                                                        state = ?STATE,
                                                        country = ?COUNTRY,
                                                        zipcode = ?ZIPCODE,
                                                        udf1 = ?UDF1,
                                                        udf2 = ?UDF2,
                                                        udf3 = ?UDF3,
                                                        udf4 = ?UDF4,
                                                        udf5 = ?UDF5,
                                                        udf6 = ?UDF6,
                                                        udf7 = ?UDF7,
                                                        udf8 = ?UDF8,
                                                        udf9 = ?UDF9,
                                                        udf10 = ?UDF10,
                                                        hash = ?HASH,
                                                        payment_source = ?PAYMENT_SOURCE,
                                                        PG_TYPE = ?PG_TYPE,
                                                        bank_ref_num = ?BANK_REF_NUM,
                                                        bankcode = ?BANKCODE,
                                                        error = ?ERROR,
                                                        error_Message = ?ERROR_MESSAGE,
                                                        name_on_card = ?NAME_ON_CARD,
                                                        cardnum = ?CARDNUM,
                                                        mode = ?MODE,
                                                        mihpayid = ?MIHPAYID,
                                                        status = ?STATUS,
                                                        unmappedstatus = ?UNMAPPEDSTATUS,
                                                        addedon = ?ADDEDON,
                                                        additionalCharges = ?ADDITIONALCHARGES
                                                        WHERE PAYU_RESPONSE_ID =?PAYU_RESPONSE_ID";
                        break;
                    }
                case FeeSQLCommands.SavePayURequest:
                    {
                        query = @"INSERT INTO FEE_PAYU_REQUEST
                                    (
                                    `KEY`,
                                    TXNID,
                                    AMOUNT,
                                    PRODUCTINFO,
                                    FIRSTNAME,
                                    EMAIL,
                                    PHONE,
                                    LASTNAME,
                                    ADDRESS1,
                                    ADDRESS2,
                                    CITY,
                                    STATE,
                                    COUNTRY,
                                    ZIPCODE,
                                    UDF1,
                                    UDF2,
                                    UDF3,
                                    UDF4,
                                    UDF5,
                                    PG,HASH)
                                    VALUES
                                    (
                                    ?KEY,
                                    ?TXNID,
                                    ?AMOUNT,
                                    ?PRODUCTINFO,
                                    ?FIRSTNAME,
                                    ?EMAIL,
                                    ?PHONE,
                                    ?LASTNAME,
                                    ?ADDRESS1,
                                    ?ADDRESS2,
                                    ?CITY,
                                    ?STATE,
                                    ?COUNTRY,
                                    ?ZIPCODE,
                                    ?UDF1,
                                    ?UDF2,
                                    ?UDF3,
                                    ?UDF4,
                                    ?UDF5,
                                    ?PG,?HASH);";
                        break;
                    }
                case FeeSQLCommands.IsPayURequestExist:
                    {
                        query = @"SELECT 
                                   txnid
                                FROM
                                    FEE_PAYU_REQUEST AS FP
                                WHERE
                                     TXNID = ?TXNID;";
                        break;
                    }
                case FeeSQLCommands.IsPayUResponseExist:
                    {
                        query = @"SELECT 
                                   PAYU_RESPONSE_ID, mihpayid,txnid
                                FROM
                                    fee_payu_response AS FP
                                WHERE					
                                         TXNID = ?TXNID;";
                        break;
                    }
                case FeeSQLCommands.SavePayUResponse:
                    {
                        query = @"INSERT INTO FEE_PAYU_RESPONSE
                                    (
                                    `KEY`,
                                    TXNID,
                                    AMOUNT,
                                    PRODUCTINFO,
                                    FIRSTNAME,
                                    EMAIL,
                                    PHONE,
                                    LASTNAME,
                                    ADDRESS1,
                                    ADDRESS2,
                                    CITY,
                                    STATE,
                                    COUNTRY,
                                    ZIPCODE,
                                    UDF1,
                                    UDF2,
                                    UDF3,
                                    UDF4,
                                    UDF5,
                                    UDF6,
                                    UDF7,
                                    UDF8,
                                    UDF9,
                                    UDF10,
                                    `HASH`,
                                    PAYMENT_SOURCE,
                                    PG_TYPE,
                                    BANK_REF_NUM,
                                    BANKCODE,
                                    `ERROR`,
                                    ERROR_MESSAGE,
                                    NAME_ON_CARD,
                                    CARDNUM,
                                    `MODE`,
                                    MIHPAYID,
                                    `STATUS`,
                                    UNMAPPEDSTATUS,ADDEDON,ADDITIONALCHARGES)
                                    VALUES
                                    (
                                    ?KEY,
                                    ?TXNID,
                                    ?AMOUNT,
                                    ?PRODUCTINFO,
                                    ?FIRSTNAME,
                                    ?EMAIL,
                                    ?PHONE,
                                    ?LASTNAME,
                                    ?ADDRESS1,
                                    ?ADDRESS2,
                                    ?CITY,
                                    ?STATE,
                                    ?COUNTRY,
                                    ?ZIPCODE,
                                    ?UDF1,
                                    ?UDF2,
                                    ?UDF3,
                                    ?UDF4,
                                    ?UDF5,
                                    ?UDF6,
                                    ?UDF7,
                                    ?UDF8,
                                    ?UDF9,
                                    ?UDF10,
                                    ?HASH,
                                    ?PAYMENT_SOURCE,
                                    ?PG_TYPE,
                                    ?BANK_REF_NUM,
                                    ?BANKCODE,
                                    ?ERROR,
                                    ?ERROR_MESSAGE,
                                    ?NAME_ON_CARD,
                                    ?CARDNUM,
                                    ?MODE,
                                    ?MIHPAYID,
                                    ?STATUS,
                                    ?UNMAPPEDSTATUS,?ADDEDON,?ADDITIONALCHARGES);
";
                        break;
                    }
                case FeeSQLCommands.FetchPayUPaymentAccountInfo:
                    {
                        query = @"SELECT 
                                    MERCHANT_ACCOUNT_INFO,
                                    ACCOUNT_ID,
                                    USERNAME,
                                    `PASSWORD`,
                                    `KEY`,
                                    SALT,
                                    BASE_URL,
                                    AUTHORIZATION,
                                    API_TYPE,
                                    HASH_SEQUENCE,
                                    BANK_ACCOUNT_ID,
                                    SP.REGISTER_NO,
                                    SP.ROLL_NO,
                                    SP.STUDENT_ID,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    SP.STU_EMAILID,
                                    SP.STU_MOBILENO,surl,furl,curl
                                FROM
                                    FEE_MERCHANT_ACCOUNT_INFO AS MI
                                        INNER JOIN
                                    SUP_BANK_ACCOUNT AS BA ON BA.BANK_ACCOUNT_ID = MI.ACCOUNT_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = ?STUDENT_ID
                                        AND SP.PROGRAMME_MODE = BA.PROGRAMME_MODE and sp.IS_DELETED!=1
                                WHERE
                                    BA.ACCOUNT_TYPE = ?ACCOUNT_TYPE
                                        AND MI.IS_DELETED != 1
                                        AND BA.IS_DELETED != 1
                                        AND MI.IS_ACTIVE=1
                                        AND MI.API_TYPE = ?API_TYPE;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeSummaryByStudentId:
                    {
                        query = @"SELECT 
                                    (SUM(IFNULL(CREDIT, 0)) - SUM(IFNULL(DEBIT, 0))) AS BALANCE,
                                    SUM(IFNULL(CREDIT, 0)) AS CREDIT,
                                    SUM(IFNULL(DEBIT, 0)) AS DEBIT,
                                    FREQUENCY_ID,
                                    TRANSACTION_DATE,
                                    FT.TRANSACTION_ID,
                                    H.HEAD,FT.FEE_TRANSACTION_BANK,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    CLS.CLASS_CODE
                                FROM
                                    FEE_STUDENT_ACCOUNT AS SA
                                        LEFT JOIN
                                    FEE_TRANSACTION AS FT ON FT.TRANSACTION_ID = SA.TRANSACTION_ID
                                        INNER JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = SA.HEAD
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SA.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SCLS ON SCLS.STUDENT_ID = SP.STUDENT_ID
                                        AND SCLS.ACADEMIC_YEAR = ?AC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SCLS.CLASS_ID
                                WHERE
                                    FREQUENCY_ID = ?FREQUENCY_ID
                                        AND SA.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SA.IS_DELETED != 1
                                        AND SA.STUDENT_ID = ?STUDENT_ID
                                GROUP BY HEAD;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeMainHeads:
                    {
                        query = @"SELECT 
                                FEE_MAIN_HEAD_ID,
                                FH.MAIN_HEAD_ID,
                                AP.APPLICATION_TYPE,
                                FR.FEE_ROOT,
                                FH.HEAD_ID,
                                FH.ACADEMIC_YEAR,
                                SFH.MAIN_HEAD,
                                SH.HEAD,
                                CONCAT(IFNULL(SFF.FREQUENCY_NAME,''),'- ',IFNULL(SFT.FREQUENCY_TYPE,'')) AS FREQUENCY_NAME,
                                SF.SHIFT_NAME,
                                SPM.PROGRAMME_MODE,
                                CONCAT(IFNULL(B.PASSBOOK_NO,''),'-',IFNULL(BA.BANK_NAME,''))AS PASSBOOK_NO,
                                FCAT.FEE_CATEGORY,FH.FREQUENCY_ID
                            FROM
                                FEE_MAIN_HEADS AS FH
                                    INNER JOIN
                                SUP_FEE_MAIN_HEAD AS SFH ON SFH.MAIN_HEAD_ID = FH.MAIN_HEAD_ID
                                    AND SFH.IS_DELETED != 1
                                    INNER JOIN
                                SUP_HEAD AS SH ON SH.HEAD_ID = FH.HEAD_ID
                                    AND SH.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_SHIFT AS SF ON SF.SHIFT_ID = FH.SHIFT
                                    AND SF.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = FH.PROGRAMME_MODE
                                    AND SPM.IS_DELETED != 1
                                    LEFT JOIN
                                FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FH.FREQUENCY_ID
                                    LEFT JOIN
                                SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                    AND SFF.IS_DELETED != 1
                                    LEFT JOIN
                                FEE_ROOT AS FR ON FR.FEE_ROOT_ID = FH.FEE_ROOT_ID
                                    AND FR.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_APPLICATION_TYPE AS AP ON AP.APPLICATION_TYPE_ID = FH.APPLICATION_TYPE_ID
                                    AND AP.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_BANK_ACCOUNT AS B ON B.BANK_ACCOUNT_ID = FH.BANK_ACCOUNT_ID
                                    AND B.IS_DELETED != 1
                                    LEFT JOIN SUP_BANK AS BA ON BA.BANK_ID=B.BANK AND BA.IS_DELETED!=1
                                    LEFT JOIN
                                SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                    AND SFT.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_FEE_CATEGORY AS FCAT ON FCAT.FEE_CATEGORY_ID = FH.FEE_CATEGORY_ID
                                    AND FCAT.IS_DELETED != 1
                            WHERE
                                FH.IS_DELETED != 1
                                    AND FH.FEE_ROOT_ID = ?FEE_ROOT_ID
                                    AND FH.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case FeeSQLCommands.SaveFeeMainHead:
                    {
                        query = @"INSERT INTO FEE_MAIN_HEADS
                                (
                                FREQUENCY_ID,
                                APPLICATION_TYPE_ID,                              
                                MAIN_HEAD_ID,
                                HEAD_ID,
                                PROGRAMME_MODE,
                                SHIFT,
                                FEE_CATEGORY_ID,
                                ACADEMIC_YEAR,
                                FEE_ROOT_ID,               
                                BANK_ACCOUNT_ID                                
                                )
                                VALUES
                                (
                                ?FREQUENCY_ID,
                                ?APPLICATION_TYPE_ID,                              
                                ?MAIN_HEAD_ID,
                                ?HEAD_ID,
                                ?PROGRAMME_MODE,
                                ?SHIFT,
                                ?FEE_CATEGORY_ID,
                                ?ACADEMIC_YEAR,
                                ?FEE_ROOT_ID,               
                                ?BANK_ACCOUNT_ID
                                );";
                        break;
                    }
                case FeeSQLCommands.SaveFeeMainHeadForHostel:
                    {
                        query = @"INSERT INTO FEE_MAIN_HEADS
                                (
                                FREQUENCY_ID,                                                              
                                MAIN_HEAD_ID,
                                HEAD_ID,                              
                                FEE_CATEGORY_ID,
                                ACADEMIC_YEAR,
                                FEE_ROOT_ID,               
                                BANK_ACCOUNT_ID                                
                                )
                                VALUES
                                (
                                ?FREQUENCY_ID,                                                              
                                ?MAIN_HEAD_ID,
                                ?HEAD_ID,                              
                                ?FEE_CATEGORY_ID,
                                ?ACADEMIC_YEAR,
                                ?FEE_ROOT_ID,               
                                ?BANK_ACCOUNT_ID
                                );";
                        break;
                    }
                case FeeSQLCommands.UpdateFeeMainHead:
                    {
                        query = @"UPDATE FEE_MAIN_HEADS
                                    SET
                                    FEE_CATEGORY_ID = ?FEE_CATEGORY_ID,
                                    BANK_ACCOUNT_ID = ?BANK_ACCOUNT_ID
                                    WHERE FEE_MAIN_HEAD_ID = ?FEE_MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.DeleteFeeMainHead:
                    {
                        query = @"UPDATE FEE_MAIN_HEADS
                                SET
                                IS_DELETED=1
                                WHERE FEE_MAIN_HEAD_ID = ?FEE_MAIN_HEAD_ID;";
                        break;

                    }
                case FeeSQLCommands.FetchSubHeadsByMainHeadId:
                    {
                        query = @"SELECT 
                                        FEE_MAIN_HEAD_ID,
                                        FH.MAIN_HEAD_ID,
                                        SH.HEAD_ID,
                                        SH.HEAD,
                                        IF(FEE_MAIN_HEAD_ID IS NOT NULL,
                                            'SELECTED',
                                            '') AS STATUS
                                    FROM
                                        SUP_HEAD AS SH
                                            LEFT JOIN
                                        FEE_MAIN_HEADS AS FH ON SH.HEAD_ID = FH.HEAD_ID
                                            AND FH.IS_DELETED != 1
                                            AND FH.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FH.MAIN_HEAD_ID = ?MAIN_HEAD_ID
                                            AND FH.SHIFT = ?SHIFT
                                            AND FH.PROGRAMME_MODE = ?PROGRAMME_MODE
                                            AND FH.FREQUENCY_ID = ?FREQUENCY_ID
                                            AND FH.FEE_ROOT_ID = ?FEE_ROOT_ID
                                            AND FH.APPLICATION_TYPE_ID = ?APPLICATION_TYPE_ID
                                    WHERE
                                        SH.IS_DELETED != 1;
                                ";
                        break;

                    }
                case FeeSQLCommands.FetchSubHeadsByMainHeadIdForHostel:
                    {
                        query = @"SELECT 
                                        FEE_MAIN_HEAD_ID,
                                        FH.MAIN_HEAD_ID,
                                        SH.HEAD_ID,
                                        SH.HEAD,
                                        IF(FEE_MAIN_HEAD_ID IS NOT NULL,
                                            'SELECTED',
                                            '') AS STATUS
                                    FROM
                                        SUP_HEAD AS SH
                                            LEFT JOIN
                                        FEE_MAIN_HEADS AS FH ON SH.HEAD_ID = FH.HEAD_ID
                                            AND FH.IS_DELETED != 1
                                            AND FH.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FH.MAIN_HEAD_ID = ?MAIN_HEAD_ID                                                                               
                                            AND FH.FREQUENCY_ID = ?FREQUENCY_ID
                                            AND FH.FEE_ROOT_ID = ?FEE_ROOT_ID                                            
                                    WHERE
                                        SH.IS_DELETED != 1;
                                ";
                        break;

                    }
                case FeeSQLCommands.FetchFeeMainHeadsByMainHeadId:
                    {
                        query = @"SELECT 
                                        MAIN_HEAD_ID,
                                        HEAD_ID,
                                        SHIFT,
                                        PROGRAMME_MODE,
                                        FREQUENCY_ID
                                    FROM
                                        FEE_MAIN_HEADS
                                    WHERE
                                        MAIN_HEAD_ID = ?MAIN_HEAD_ID AND SHIFT=?SHIFT AND PROGRAMME_MODE=?PROGRAMME_MODE AND FREQUENCY_ID=?FREQUENCY_ID  AND FEE_ROOT_ID = ?FEE_ROOT_ID
                                            AND APPLICATION_TYPE_ID = ?APPLICATION_TYPE_ID AND ACADEMIC_YEAR=?AC_YEAR AND IS_DELETED!=1; ";
                        break;
                    }
                case FeeSQLCommands.FetchFeeMainHeadsByMainHeadIdForHostel:
                    {
                        query = @"SELECT 
                                        MAIN_HEAD_ID,
                                        HEAD_ID,
                                        SHIFT,
                                        PROGRAMME_MODE,
                                        FREQUENCY_ID
                                    FROM
                                        FEE_MAIN_HEADS
                                    WHERE
                                        MAIN_HEAD_ID = ?MAIN_HEAD_ID AND FREQUENCY_ID=?FREQUENCY_ID  AND FEE_ROOT_ID = ?FEE_ROOT_ID
                                            AND ACADEMIC_YEAR=?AC_YEAR AND IS_DELETED!=1; ";
                        break;
                    }
                case FeeSQLCommands.DeleteFeeMainHeadByMainHeadIdAndHeadId:
                    {
                        query = @"SET SQL_SAFE_UPDATES=0;
                                UPDATE FEE_MAIN_HEADS
                                SET
                                IS_DELETED=1
                                WHERE SHIFT=?SHIFT AND PROGRAMME_MODE=?PROGRAMME_MODE AND FREQUENCY_ID=?FREQUENCY_ID AND ACADEMIC_YEAR = ?ACADEMIC_YEAR AND MAIN_HEAD_ID=?MAIN_HEAD_ID AND HEAD_ID=?HEAD_ID;
                                SET SQL_SAFE_UPDATES=1;";
                        break;

                    }
                case FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFrequencyId:
                    {
                        query = @"SELECT 
                                        FSA.STUDENT_ID,
                                        FSA.FREQUENCY_ID,
                                        SFM.HEAD_ID,
                                        SFM.HEAD,
                                        FSA.CREDIT,
                                        FSA.DEBIT,
                                        (IFNULL(SUM(FSA.CREDIT), 0) - IFNULL(SUM(FSA.DEBIT), 0)) AS BALANCE,FSA.FEE_MAIN_HEAD_ID
                                    FROM
                                        FEE_STUDENT_ACCOUNT AS FSA
                                            INNER JOIN
                                        SUP_HEAD AS SFM ON SFM.HEAD_ID = FSA.HEAD
                                    WHERE
                                        FSA.STUDENT_ID = ?STUDENT_ID
                                            AND FSA.IS_DELETED != 1
                                            AND SFM.IS_DELETED!=1
                                            AND FSA.FREQUENCY_ID IN ( ?FREQUENCY_ID)
                                            AND FSA.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY FSA.HEAD,FSA.FREQUENCY_ID
                                    ORDER BY FSA.F_STUDENT_AC_ID ASC;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFeeModeId:
                    {
                        query = @"SELECT 
                                        FSA.STUDENT_ID,
                                        FSA.FREQUENCY_ID,
                                        SFM.HEAD_ID,
                                        SFM.HEAD,
                                        FSA.CREDIT,
                                        FSA.DEBIT,
                                        (IFNULL(SUM(FSA.CREDIT), 0) - IFNULL(SUM(FSA.DEBIT), 0)) AS BALANCE,
                                        FSA.FEE_MAIN_HEAD_ID
                                    FROM
                                        FEE_STUDENT_ACCOUNT AS FSA
                                            INNER JOIN
                                        SUP_HEAD AS SFM ON SFM.HEAD_ID = FSA.HEAD
                                            INNER JOIN
                                        FEE_FREQUENCY_FEE_MODE AS FFFM ON FFFM.FEE_FREQUENCY_FEE_MODE_ID = FSA.FREQUENCY_ID
                                    WHERE
                                        FSA.STUDENT_ID = ?STUDENT_ID
                                            AND FSA.IS_DELETED != 1
                                            AND SFM.IS_DELETED != 1
                                            AND FFFM.FEE_MODE = ?FEE_MODE
                                            AND FSA.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY FSA.HEAD , FSA.FREQUENCY_ID
                                    ORDER BY FSA.F_STUDENT_AC_ID ASC;";
                        break;
                    }
                case FeeSQLCommands.CheckIsExistingAccountForHostelAndMess:
                    {
                        query = @"SELECT 
                                FSA.F_STUDENT_AC_ID,
                                FSA.STUDENT_ID,
                                FSA.FREQUENCY_ID,
                                SFM.HEAD_ID,
                                SFM.HEAD,
                                FSA.CREDIT,
                                FSA.DEBIT,
                                (IFNULL(SUM(FSA.CREDIT), 0) - IFNULL(SUM(FSA.DEBIT), 0)) AS BALANCE,
                                FSA.FEE_MAIN_HEAD_ID
                            FROM
                                FEE_STUDENT_ACCOUNT AS FSA
                                    INNER JOIN
                                FEE_MAIN_HEADS AS FMH ON FMH.FEE_MAIN_HEAD_ID = FSA.FEE_MAIN_HEAD_ID
                                    INNER JOIN
                                SUP_FEE_MAIN_HEAD AS FM ON FM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                    INNER JOIN
                                SUP_HEAD AS SFM ON SFM.HEAD_ID = FMH.HEAD_ID
                            WHERE
                                FSA.STUDENT_ID = ?STUDENT_ID
                                    AND FSA.IS_DELETED != 1
                                    AND SFM.IS_DELETED != 1
                                    AND FSA.FREQUENCY_ID IN (?FREQUENCY_ID)
                                    AND FSA.ACADEMIC_YEAR = ?AC_YEAR
                                    AND FMH.MAIN_HEAD_ID IN (?MAIN_HEAD_ID)
                            GROUP BY FSA.HEAD , FSA.FREQUENCY_ID
                            ORDER BY FSA.F_STUDENT_AC_ID ASC;";
                        break;
                    }
                case FeeSQLCommands.FetchCreditByStudentId:
                    {
                        query = @"SELECT 
                                        IFNULL(SUM(CREDIT), 0) CREDIT,
                                        IFNULL(SUM(DEBIT), 0) DEBIT,
                                        (IFNULL(SUM(CREDIT), 0) - IFNULL(SUM(DEBIT), 0)) AS BALANCE
                                    FROM
                                        FEE_STUDENT_ACCOUNT
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID AND IS_DELETED != 1
                                            AND FREQUENCY_ID = ?FREQUENCY_ID
                                            AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case FeeSQLCommands.SaveFeeTransaction:
                    {
                        query = @"INSERT INTO FEE_TRANSACTION
                                        (
                                        STUDENT_ID,
                                        FREQUENCY,
                                        PAYMENT_DATE,
                                        RECEIPT_NO,
                                        PAYMENT_MODE,
                                        COLLECTED,
                                        USERNAME,
                                        ACADEMIC_YEAR,
                                        FREQUENCY_TO,RAZORPAY_ID,CF_ORDER_ID)
                                        VALUES
                                        (
                                        ?STUDENT_ID,
                                        ?FREQUENCY,
                                        ?PAYMENT_DATE,
                                        ?RECEIPT_NO,
                                        ?PAYMENT_MODE,
                                        ?COLLECTED,
                                        ?USERNAME,
                                        ?AC_YEAR,
                                        ?FREQUENCY,'?CF_ORDER_ID','?CF_ORDER_ID');
                                        ";
                        break;
                    }
                case FeeSQLCommands.FetchAutoGenerateNumber:
                    {
                        query = @"SELECT 
                                        auto_generate_numbers_id, exam_receipt_no,sum(advance_receipt_no+1) as advance_receipt_no,advance_receipt_prefix,sum(Access_No + 1 ) as Access_No
                                    FROM
                                        AUTO_GENERATE_NUMBERS WHERE AUTO_GENERATE_NUMBERS_ID=1;";
                        break;
                    }
                case FeeSQLCommands.UpdateAutoGenerateNumber:
                    {
                        query = @"UPDATE AUTO_GENERATE_NUMBERS SET
                                        EXAM_RECEIPT_NO=?EXAM_RECEIPT_NO,
                                        ADVANCE_RECEIPT_NO=?ADVANCE_RECEIPT_NO
                                        WHERE AUTO_GENERATE_NUMBERS_ID=1;";
                        break;
                    }
                case FeeSQLCommands.SaveFeeCollection:
                    {
                        query = @"INSERT INTO FEE_COLLECTION
                                        (
                                        TRANSACTION_ID,
                                        HEAD,
                                        PAID_AMOUNT,
                                        FREQUENCY,
                                        RECEIPT_NO,FEE_MAIN_HEAD_ID)
                                        VALUES
                                        (
                                        ?TRANSACTION_ID,
                                        ?HEAD,
                                        ?PAID_AMOUNT,
                                        ?FREQUENCY,
                                        ?RECEIPT_NO,?FEE_MAIN_HEAD_ID);";
                        break;
                    }
                case FeeSQLCommands.SaveFeeStudentAccount:
                    {
                        query = @"INSERT INTO FEE_STUDENT_ACCOUNT
                                        (
                                        STUDENT_ID,
                                        ACADEMIC_YEAR,
                                        FREQUENCY_ID,
                                        HEAD,
                                        DEBIT,
                                        TRANSACTION_DATE,
                                        TRANSACTION_ID,FEE_MAIN_HEAD_ID)
                                        VALUES
                                        (
                                        ?STUDENT_ID,
                                        ?AC_YEAR,
                                        ?FREQUENCY_ID,
                                        ?HEAD,
                                        ?DEBIT,
                                        ?TRANSACTION_DATE,
                                        ?TRANSACTION_ID,?FEE_MAIN_HEAD_ID);";
                        break;
                    }
                case FeeSQLCommands.FetchFeeStudentAccountByTransactionId:
                    {
                        query = @"SELECT 
                                        FSA.STUDENT_ID,SFM.HEAD, FSA.TRANSACTION_ID, FSA.DEBIT
                                    FROM
                                        FEE_STUDENT_ACCOUNT AS FSA
                                            INNER JOIN
                                        SUP_HEAD AS SFM ON SFM.HEAD_ID = FSA.HEAD
                                    WHERE
                                        TRANSACTION_ID = ?TRANSACTION_ID
                                            AND FSA.IS_DELETED != 1
                                            AND SFM.IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.FetchTotalDebitByTransactionId:
                    {
                        query = @"SELECT 
                                        FT.RECEIPT_NO, FT.COLLECTED,DATE_FORMAT(PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,SPM.PAYMENT_MODE,FT.FEE_TRANSACTION_BANK,FT.STUDENT_ID
                                    FROM
                                        FEE_TRANSACTION AS FT INNER JOIN SUP_PAYMENT_MODE AS SPM ON SPM.PAYMENT_MODE_ID=FT.PAYMENT_MODE
                                    WHERE
                                        FT.TRANSACTION_ID = ?TRANSACTION_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchClassWiseFeeDetails:
                    {
                        query = @"SELECT 
                                SP.STUDENT_ID,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                UPPER(CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                IFNULL(SP.LAST_NAME, ''))) AS FIRST_NAME,
                                SUM(IFNULL(FSAC.CREDIT, 0)) AS TOTAL_AMOUNT,
                                IF(SUM((IFNULL(FSAC.CREDIT, 0) - IFNULL(FSAC.DEBIT, 0))) = 0,
                                    'PAID',
                                    'NOT PAID') AS STATUS,
                                (SUM(IFNULL(FSAC.CREDIT, 0)) - SUM(IFNULL(FSAC.DEBIT, 0))) AS BALANCE,
                                CC.CLASS_CODE AS CLASS,
                                CC.CLASS_YEAR,
                                CC.PROGRAMME_GROUP_ID /*,
                                FROM_UNIXTIME(RP.CREATED_AT, '%d/%m/%Y') AS PAYMENT_DATE,
                                RO.ID AS ORDER_ID,
                                RP.ID AS RAZOR_PAY_ID */
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID 
                                 AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID AND CC.IS_DELETED!=1
                                    LEFT JOIN
                                FEE_STUDENT_ACCOUNT AS FSAC ON SP.STUDENT_ID = FSAC.STUDENT_ID
                                    AND FSAC.FREQUENCY_ID = ?FREQUENCY
                                    AND FSAC.IS_DELETED!=1                                    
                                   /* LEFT JOIN
                                FEE_RAZORPAY_ORDER_INFO_?AC_YEAR AS RO ON RO.UDF1 = SP.STUDENT_ID AND RO.IS_DELETED!=1
                                    AND FIND_IN_SET(?FREQUENCY, RO.UDF3)
                                    LEFT JOIN
                                FEE_RAZORPAY_PAYMENT_INFO_?AC_YEAR AS RP ON RP.ORDER_ID = RO.ID AND RP.IS_DELETED!=1
                                    AND FIND_IN_SET(?FREQUENCY, RO.UDF3) */
                            WHERE
                                SC.CLASS_ID IN (?CLASS)
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
		                            AND SC.IS_DELETED!=1
                            GROUP BY SP.STUDENT_ID ORDER BY SC.CLASS_ID,SP.ROLL_NO;";
                        break;
                    }
                case FeeSQLCommands.FetchMonthlyStudentFeeDetails:
                    {
                        query = @"SELECT
	                                    SF.FREQUENCY_NAME,
                                        MONTHNAME(FT.PAYMENT_DATE) AS MONTH,F.FEE_TRANSACTION_BANK,
                                        DATE_FORMAT(PAYMENT_DATE, '%d/%m/%y') AS PAYMENT_DATE,
                                        SUM(FT.COLLECTED) AS COLLECTED
                                    FROM
                                        FEE_TRANSACTION AS FT
		                                    INNER JOIN
	                                    SUP_FEE_FREQUENCY AS SF ON SF.FREQUENCY_ID=FT.FREQUENCY
											INNER JOIN
										STU_CLASS AS SC ON SC.STUDENT_ID=FT.STUDENT_ID AND SC.ACADEMIC_YEAR=?AC_YEAR
											INNER JOIN
										STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID AND SP.PROGRAMME_MODE=?PROGRAMME_MODE
                                    WHERE
                                        PAYMENT_DATE BETWEEN ?DATE_FROM AND ?DATE_TO
                                    GROUP BY PAYMENT_DATE;";
                        break;
                    }
                case FeeSQLCommands.FetchStudentFeeDetailsOnlyMonth:
                    {
                        query = @"SELECT
                                        MONTHNAME(FT.PAYMENT_DATE) AS MONTH,
                                        SUM(FT.COLLECTED) AS COLLECTED
                                    FROM
                                        FEE_TRANSACTION AS FT
		                                    INNER JOIN
	                                    SUP_FEE_FREQUENCY AS SF ON SF.FREQUENCY_ID=FT.FREQUENCY
											INNER JOIN
										STU_CLASS AS SC ON SC.STUDENT_ID=FT.STUDENT_ID AND SC.ACADEMIC_YEAR=?AC_YEAR
											INNER JOIN
										STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID AND SP.PROGRAMME_MODE=?PROGRAMME_MODE
                                    WHERE
                                        PAYMENT_DATE BETWEEN ?DATE_FROM AND ?DATE_TO
                                    GROUP BY MONTH ORDER BY MONTH;";
                        break;
                    }
                case FeeSQLCommands.FetchStudentAccountOnlyCredit:
                    {
                        query = @"SELECT 
                                        HEAD, CREDIT
                                    FROM
                                        FEE_STUDENT_ACCOUNT
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID AND FREQUENCY_ID = ?FREQUENCY_ID
                                            AND TRANSACTION_ID IS NULL
                                            AND IS_DELETED != 1
                                            AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND DEBIT IS NULL;";
                        break;
                    }
                case FeeSQLCommands.DeleteFeeCollection:
                    {
                        query = @"UPDATE FEE_COLLECTION
                                SET
                                IS_DELETED=1
                                WHERE TRANSACTION_ID = ?TRANSACTION_ID;";
                        break;
                    }
                case FeeSQLCommands.DeleteFeeTransaction:
                    {
                        query = @"UPDATE FEE_TRANSACTION
                                SET
                                IS_DELETED=1
                                WHERE TRANSACTION_ID = ?TRANSACTION_ID;";
                        break;
                    }
                case FeeSQLCommands.DeleteFeeStudentAccount:
                    {
                        query = @"UPDATE FEE_STUDENT_ACCOUNT
                                SET
                                IS_DELETED=1
                                WHERE TRANSACTION_ID = ?TRANSACTION_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchTotalDebitByStudentIdAndFrequencyId:
                    {
                        query = @"SELECT 
                                        FT.RECEIPT_NO,
                                        FT.COLLECTED,
                                        DATE_FORMAT(PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,
                                        SPM.PAYMENT_MODE,
                                        FT.STUDENT_ID
                                    FROM
                                        FEE_TRANSACTION AS FT
                                            INNER JOIN
                                        SUP_PAYMENT_MODE AS SPM ON SPM.PAYMENT_MODE_ID = FT.PAYMENT_MODE
                                            
                                    WHERE
                                        FT.STUDENT_ID = ?STUDENT_ID AND FT.FREQUENCY = ?FREQUENCY AND FT.IS_DELETED!=1;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeStudentAccountForReceiptByStudentIdAndFrequencyId:
                    {
                        query = @"SELECT 
                                        FSA.STUDENT_ID,SFM.HEAD, FSA.TRANSACTION_ID, FSA.DEBIT,FT.RECEIPT_NO
                                    FROM
                                        FEE_STUDENT_ACCOUNT AS FSA
											INNER JOIN
										fee_transaction AS FT ON FT.TRANSACTION_ID=FSA.TRANSACTION_ID
                                            INNER JOIN
                                        SUP_HEAD AS SFM ON SFM.HEAD_ID = FSA.HEAD
                                    WHERE
                                        FSA.STUDENT_ID=?STUDENT_ID AND FSA.FREQUENCY_ID=?FREQUENCY_ID AND FSA.TRANSACTION_ID IS NOT NULL
                                            AND FSA.IS_DELETED != 1
                                            AND SFM.IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeDiscount:
                    {
                        query = @"SELECT 
                                        FD.DISCOUNT_ID,
                                        FD.DISCOUNT_NAME,
                                        FD.DISCOUNT_VALUE,
                                        SFT.FREQUENCY_TYPE,
                                        FD.IS_USED
                                    FROM
                                        FEE_DISCOUNT AS FD
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FD.FREQUENCY_TYPE
                                    WHERE
                                        FD.IS_DELETED != 1
                                            AND SFT.IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.SaveFeeDiscount:
                    {
                        query = @"INSERT INTO FEE_DISCOUNT
                                        (
                                        DISCOUNT_NAME,
                                        FREQUENCY_TYPE,
                                        DISCOUNT_VALUE)
                                        VALUES
                                        (
                                        ?DISCOUNT_NAME,
                                        ?FREQUENCY_TYPE,
                                        ?DISCOUNT_VALUE);";
                        break;
                    }
                case FeeSQLCommands.FetchFeeDiscountById:
                    {
                        query = @"SELECT 
                                        DISCOUNT_ID, DISCOUNT_NAME, FREQUENCY_TYPE, DISCOUNT_VALUE
                                    FROM
                                        FEE_DISCOUNT
                                    WHERE
                                        DISCOUNT_ID = ?DISCOUNT_ID;";
                        break;
                    }
                case FeeSQLCommands.UpdateFeeDiscount:
                    {
                        query = @"UPDATE FEE_DISCOUNT
                                        SET
                                        DISCOUNT_NAME=?DISCOUNT_NAME,
                                        FREQUENCY_TYPE=?FREQUENCY_TYPE,
                                        DISCOUNT_VALUE=?DISCOUNT_VALUE
                                        WHERE DISCOUNT_ID=?DISCOUNT_ID;";
                        break;
                    }
                case FeeSQLCommands.DeleteFeeDiscount:
                    {
                        query = @"UPDATE FEE_DISCOUNT
                                        SET
                                        IS_DELETED=1
                                        WHERE DISCOUNT_ID=?DISCOUNT_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeDiscountAllotmentByClassIdAndAcademicyear:
                    {
                        query = @"SELECT 
                                        FDA.DISCOUNT_ALLOTMENT_ID,
                                        FDA.ACCADEMIC_YEAR,
                                        FDA.CLASS_ID,
                                        FDA.DISCOUNT_ID,
                                        FD.DISCOUNT_NAME,
                                        FD.DISCOUNT_VALUE,
                                        SP.STUDENT_ID,
                                        SP.ROLL_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME
                                    FROM
                                        FEE_DISCOUNT_ALLOTMENT AS FDA
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = FDA.STUDENT_ID
                                            AND SC.CLASS_ID = FDA.CLASS_ID
                                            AND SC.ACADEMIC_YEAR = FDA.ACCADEMIC_YEAR
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            INNER JOIN
                                        FEE_DISCOUNT AS FD ON FD.DISCOUNT_ID = FDA.DISCOUNT_ID
                                    WHERE
                                        SP.IS_LEFT != 1 AND SP.IS_DELETED != 1
                                            AND SC.IS_DELETED != 1
                                            AND FDA.ACCADEMIC_YEAR = ?ACCADEMIC_YEAR
                                            AND FDA.CLASS_ID = ?CLASS_ID
                                            AND FDA.IS_DELETED != 1
                                            AND FD.IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.CheckIsFeeDiscountAllotmentExisting:
                    {
                        query = @"SELECT 
                                        DISCOUNT_ALLOTMENT_ID
                                    FROM
                                        FEE_DISCOUNT_ALLOTMENT
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID AND ACCADEMIC_YEAR = ?ACCADEMIC_YEAR
                                            AND CLASS_ID = ?CLASS_ID
                                            AND DISCOUNT_ID = ?DISCOUNT_ID
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.SaveFeeDiscountAllotment:
                    {
                        query = @"INSERT INTO FEE_DISCOUNT_ALLOTMENT
                                        (
                                        STUDENT_ID,
                                        ACCADEMIC_YEAR,
                                        CLASS_ID,
                                        DISCOUNT_ID)
                                        VALUES
                                        (
                                        ?STUDENT_ID,
                                        ?ACCADEMIC_YEAR,
                                        ?CLASS_ID,
                                        ?DISCOUNT_ID);";
                        break;
                    }
                case FeeSQLCommands.DeleteFeeDiscountAllotment:
                    {
                        query = @"UPDATE FEE_DISCOUNT_ALLOTMENT
                                        SET
                                        IS_DELETED=1
                                        WHERE DISCOUNT_ALLOTMENT_ID=?DISCOUNT_ALLOTMENT_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeStructureByClassAndFrequencyId:
                    {
                        query = @"SELECT 
                                        FS.FEE_STRUCTURE_ID,
                                        FS.ACADEMIC_YEAR,
                                        FS.FREQUENCY,
                                        SFF.FREQUENCY_NAME,
                                        FS.CLASS,
                                        CLS.CLASS_NAME,
                                        SH.HEAD_ID,
                                        SH.HEAD,
                                        FS.AMOUNT,
                                        SFC.FEE_CATEGORY_ID,
                                        SFC.FEE_CATEGORY
                                    FROM
                                        FEE_STRUCTURE AS FS
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FS.FREQUENCY
                                            INNER JOIN
                                        SUP_HEAD AS SH ON SH.HEAD_ID = FS.HEAD
                                            AND FS.FEE_CATEGORY = SH.FEE_CATEGORY
                                            INNER JOIN
                                        SUP_FEE_CATEGORY AS SFC ON SFC.FEE_CATEGORY_ID = FS.FEE_CATEGORY
                                            INNER JOIN
                                        CP_CLASSES_?AC_YEAR AS CLS ON CLS.CLASS_ID = FS.CLASS
                                    WHERE
                                        FS.FREQUENCY = ?FREQUENCY AND FS.IS_DELETED != 1
                                            AND SH.IS_DELETED != 1
                                            AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FS.CLASS = ?CLASS;";
                        break;
                    }
                case FeeSQLCommands.CheckIsFeeStructureExisting:
                    {
                        query = @"SELECT 
                                        FEE_STRUCTURE_ID
                                    FROM
                                        FEE_STRUCTURE
                                    WHERE
                                        FREQUENCY = ?FREQUENCY AND CLASS = ?CLASS
                                            AND HEAD = ?HEAD AND ACADEMIC_YEAR=?ACADEMIC_YEAR
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.SaveFeeStructure:
                    {
                        query = @"INSERT INTO FEE_STRUCTURE
                                    (
                                    ACADEMIC_YEAR,
                                    FREQUENCY,
                                    CLASS_YEAR_ID,
                                    HEAD,
                                    AMOUNT,
                                    FEE_CATEGORY,
                                    PROGRAMME,
                                    BANK_ACCOUNT_ID,FEE_MAIN_HEAD_ID)
                                    VALUES
                                    (
                                    ?ACADEMIC_YEAR,
                                    ?FREQUENCY,
                                    ?CLASS_YEAR_ID,
                                    ?HEAD,
                                    ?AMOUNT,
                                    ?FEE_CATEGORY,
                                    ?PROGRAMME,
                                    ?BANK_ACCOUNT_ID,?FEE_MAIN_HEAD_ID);
                                    ";
                        break;
                    }
                case FeeSQLCommands.SaveHostelAndMessFeeStructure:
                    {
                        query = @"INSERT INTO FEE_STRUCTURE_FOR_HOSTEL_AND_MESS
                                    (
                                    ACADEMIC_YEAR,
                                    FREQUENCY,
                                    
                                    HEAD,
                                    AMOUNT,
                                    
                                    
                                    BANK_ACCOUNT_ID,FEE_MAIN_HEAD_ID)
                                    VALUES
                                    (
                                    ?ACADEMIC_YEAR,
                                    ?FREQUENCY,
                                  
                                    ?HEAD,
                                    ?AMOUNT,
                                  
                                 
                                    ?BANK_ACCOUNT_ID,?FEE_MAIN_HEAD_ID);
                                    ";
                        break;
                    }
                case FeeSQLCommands.DeleteFeeStructure:
                    {
                        query = @"UPDATE FEE_STRUCTURE
                                SET
                                IS_DELETED=1
                                WHERE FEE_STRUCTURE_ID = ?FEE_STRUCTURE_ID;";
                        break;
                    }
                case FeeSQLCommands.DeleteFeeHostelAndMessStructure:
                    {
                        query = @"UPDATE FEE_STRUCTURE_FOR_HOSTEL_AND_MESS
                                SET
                                IS_DELETED=1
                                WHERE FEE_STRUCTURE_ID = ?FEE_STRUCTURE_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeStructureById:
                    {
                        query = @"SELECT 
                                        FEE_STRUCTURE_ID,
                                        ACADEMIC_YEAR,
                                        FREQUENCY,
                                        CLASS,
                                        HEAD,
                                        AMOUNT,
                                        FEE_CATEGORY,
                                        PROGRAMME
                                    FROM
                                        FEE_STRUCTURE
                                    WHERE
                                        FEE_STRUCTURE_ID = ?FEE_STRUCTURE_ID AND IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.UpdateFeeStructure:
                    {
                        query = @"UPDATE FEE_STRUCTURE
                                        SET
                                        ACADEMIC_YEAR= ?ACADEMIC_YEAR,
                                        FREQUENCY= ?FREQUENCY,
                                        CLASS_YEAR_ID= ?CLASS_YEAR_ID,
                                        HEAD= ?HEAD,
                                        AMOUNT= ?AMOUNT,
                                        FEE_CATEGORY= ?FEE_CATEGORY,
                                        PROGRAMME= ?PROGRAMME,
                                        BANK_ACCOUNT_ID= ?BANK_ACCOUNT_ID,
                                        FEE_MAIN_HEAD_ID=?FEE_MAIN_HEAD_ID
                                        WHERE FEE_STRUCTURE_ID= ?FEE_STRUCTURE_ID;";
                        break;
                    }
                case FeeSQLCommands.UpdateFeeStructureForHostelAndMess:
                    {
                        query = @"UPDATE FEE_STRUCTURE_FOR_HOSTEL_AND_MESS
                                        SET
                                        ACADEMIC_YEAR= ?ACADEMIC_YEAR,
                                        FREQUENCY= ?FREQUENCY,
                                        
                                        HEAD= ?HEAD,
                                        AMOUNT= ?AMOUNT,
                                        
                                       
                                        BANK_ACCOUNT_ID= ?BANK_ACCOUNT_ID,
                                        FEE_MAIN_HEAD_ID=?FEE_MAIN_HEAD_ID
                                        WHERE FEE_STRUCTURE_ID= ?FEE_STRUCTURE_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeDiscountByClassIdAndDiscountId:
                    {
                        query = @"SELECT 
                                        DISCOUNT_ALLOTMENT_ID, STUDENT_ID, CLASS_ID, DISCOUNT_ID
                                    FROM
                                        FEE_DISCOUNT_ALLOTMENT 
                                    WHERE
                                        DISCOUNT_ID = ?DISCOUNT_ID AND CLASS_ID = ?CLASS_ID
                                            AND IS_DELETED != 1
                                            AND ACCADEMIC_YEAR = ?ACCADEMIC_YEAR;";
                        break;
                    }
                case FeeSQLCommands.SaveFeeDiscountFrequency:
                    {
                        query = @"INSERT INTO FEE_DISCOUNT_FREQUENCY
                                        (
                                        DISCOUNT_ID,
                                        FREQUENCY_ID)
                                        VALUES
                                        (
                                        ?DISCOUNT_ID,
                                        ?FREQUENCY_ID);";
                        break;
                    }
                case FeeSQLCommands.FetchFeeDiscountFrequencyByDiscountId:
                    {
                        query = @"SELECT 
                                        DISCOUNT_FRE_ID, DISCOUNT_ID, FREQUENCY_ID
                                    FROM
                                        FEE_DISCOUNT_FREQUENCY
                                    WHERE
                                        DISCOUNT_ID = ?DISCOUNT_ID AND IS_DELETED!=1;";
                        break;
                    }
                case FeeSQLCommands.DeleteFeeDiscountFrequency:
                    {
                        query = @"UPDATE FEE_DISCOUNT_FREQUENCY
                                SET
                                IS_DELETED=1
                                WHERE DISCOUNT_ID = ?DISCOUNT_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeDiscountHead:
                    {
                        query = @"SELECT 
                                        FDH.DISCOUNT_HEAD_ID,
                                        FD.DISCOUNT_NAME,
                                        SFF.FREQUENCY_NAME,
                                        SH.HEAD
                                    FROM
                                        FEE_DISCOUNT_HEAD AS FDH
                                            INNER JOIN
                                        FEE_DISCOUNT_FREQUENCY AS FDF ON FDF.FREQUENCY_ID = FDH.FREQUENCY_ID
                                            AND FDF.DISCOUNT_ID = FDH.DISCOUNT_ID
                                            INNER JOIN
                                        FEE_DISCOUNT AS FD ON FD.DISCOUNT_ID = FDF.DISCOUNT_ID
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FDF.FREQUENCY_ID
                                            INNER JOIN
                                        SUP_HEAD AS SH ON SH.HEAD_ID = FDH.HEAD_ID
                                    WHERE
                                        FD.IS_DELETED != 1
                                            AND FDF.IS_DELETED != 1 AND FDH.IS_DELETED!=1;";
                        break;
                    }
                case FeeSQLCommands.FetchHeadByDiscountIdAndFrequencyId:
                    {
                        query = @"SELECT 
                                        SH.HEAD_ID,
                                        SH.HEAD,
                                        IF(FDH.DISCOUNT_HEAD_ID IS NOT NULL,
                                            'SELECTED',
                                            '') AS STATUS
                                    FROM
                                        SUP_HEAD AS SH
                                            LEFT JOIN
                                        FEE_DISCOUNT_HEAD AS FDH ON FDH.HEAD_ID = SH.HEAD_ID
                                            AND FDH.DISCOUNT_ID = ?DISCOUNT_ID
                                            AND FDH.FREQUENCY_ID = ?FREQUENCY_ID
                                            AND FDH.IS_DELETED != 1
                                    WHERE
                                        SH.IS_DELETED != 1";
                        break;
                    }
                case FeeSQLCommands.FetchFeeDiscountHeadByDiscountIdAndFrequencyId:
                    {
                        query = @"SELECT 
                                        DISCOUNT_HEAD_ID, DISCOUNT_ID, FREQUENCY_ID, HEAD_ID
                                    FROM
                                        FEE_DISCOUNT_HEAD
                                    WHERE
                                        DISCOUNT_ID = ?DISCOUNT_ID AND FREQUENCY_ID = ?FREQUENCY_ID
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.DeleteFeeDiscountHead:
                    {
                        query = @"UPDATE FEE_DISCOUNT_HEAD
                                        SET
                                        IS_DELETED=1
                                        WHERE DISCOUNT_HEAD_ID=?DISCOUNT_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.SaveFeeDiscountHead:
                    {
                        query = @"INSERT INTO FEE_DISCOUNT_HEAD
                                        (
                                        DISCOUNT_ID,
                                        FREQUENCY_ID,
                                        HEAD_ID)
                                        VALUES
                                        (
                                        ?DISCOUNT_ID,
                                        ?FREQUENCY_ID,
                                        ?HEAD_ID);";
                        break;
                    }
                case FeeSQLCommands.DeleteFeeDiscountHeadByDiscountId:
                    {
                        query = @"UPDATE FEE_DISCOUNT_HEAD
                                SET
                                IS_DELETED=1
                                WHERE DISCOUNT_ID = ?DISCOUNT_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeAdvancePaymentByClassIdAndAcademicYear:
                    {
                        query = @"SELECT 
                                        AD.ADVANCE_ID,
                                        AD.STUDENT_ID,
                                        AD.RECEIPT_NO,
                                        AD.COLLECTED,
                                        AD.DEBIT,
                                        AD.ACADEMIC_YEAR,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        SPM.PAYMENT_MODE,
                                        DATE_FORMAT(AD.PAYMENT_DATE, '%d/%m/%y') AS PAYMENT_DATE,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO
                                    FROM
                                        FEE_ADVANCE_PAYMENT AS AD
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = AD.STUDENT_ID
                                            INNER JOIN
                                        SUP_PAYMENT_MODE AS SPM ON SPM.PAYMENT_MODE_ID = AD.PAYMENT_MODE
                                    WHERE
                                        AD.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND AD.CLASS = ?CLASS AND AD.IS_DELETED!=1;";
                        break;
                    }
                case FeeSQLCommands.SaveFeeAdvancePayment:
                    {
                        query = @"INSERT INTO FEE_ADVANCE_PAYMENT
                                        (
                                        STUDENT_ID,
                                        CLASS,
                                        PAYMENT_DATE,
                                        RECEIPT_NO,
                                        PAYMENT_MODE,
                                        COLLECTED,
                                        USERNAME,
                                        ACADEMIC_YEAR,
                                        IS_ACCOUNTANT_COLLECTED)
                                        VALUES
                                        (
                                        ?STUDENT_ID,
                                        ?CLASS,
                                        ?PAYMENT_DATE,
                                        ?RECEIPT_NO,
                                        ?PAYMENT_MODE,
                                        ?COLLECTED,
                                        ?USERNAME,
                                        ?ACADEMIC_YEAR,
                                        1);";
                        break;
                    }
                case FeeSQLCommands.FetchHallTicketValidation:
                    {
                        query = @"SELECT 
                                        HTV.HALL_TICKET_VALIDATION_ID,
                                        HTV.STUDENT_ID,
                                        HTV.REASON,
                                        CLS.CLASS_ID,
                                        CLS.CLASS_NAME,
                                        SP.REGISTER_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,SP.ROLL_NO
                                    FROM
                                        HALL_TICKET_VALIDATION AS HTV
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = HTV.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = HTV.ACADEMIC_YEAR
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        SC.CLASS_ID IN (?CLASS_ID)
		                                    AND HTV.EXAM_SETTING_ID=(SELECT EXAM_SETTING_ID FROM EXAM_SETTING WHERE IS_ACTIVE=1)
                                            AND HTV.ACADEMIC_YEAR = ?AC_YEAR
                                            AND HTV.IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.FetchHallTicketValidationByACYear:
                    {
                        query = @"SELECT 
                                    HTV.HALL_TICKET_VALIDATION_ID,
                                    HTV.STUDENT_ID,
                                    HTV.REASON,
                                    CLS.CLASS_ID,
                                    CLS.CLASS_NAME,
                                    SP.REGISTER_NO,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    SP.ROLL_NO
                                FROM
                                    HALL_TICKET_VALIDATION AS HTV
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = HTV.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = HTV.ACADEMIC_YEAR
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                WHERE
                                    HTV.EXAM_SETTING_ID = (SELECT 
                                            EXAM_SETTING_ID
                                        FROM
                                            EXAM_SETTING
                                        WHERE
                                            IS_ACTIVE = 1)
                                        AND HTV.ACADEMIC_YEAR = ?AC_YEAR
                                        AND HTV.IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.DeleteHalTicketValidation:
                    {
                        query = @"UPDATE HALL_TICKET_VALIDATION
                                        SET
                                        IS_DELETED=1
                                        WHERE HALL_TICKET_VALIDATION_ID=?HALL_TICKET_VALIDATION_ID;";
                        break;
                    }
                case FeeSQLCommands.UpdateHallTicketValidationPaidStatus:
                    {
                        query = @"UPDATE HALL_TICKET_VALIDATION_?AC_YEAR
                                        SET
                                        IS_PAID=1
                                        WHERE HALL_TICKET_VALIDATION_ID=?HALL_TICKET_VALIDATION_ID;";
                        break;
                    }
                case FeeSQLCommands.CheckIsHallTicketValidationExisting:
                    {
                        query = @"SELECT 
                                    HALL_TICKET_VALIDATION_ID
                                FROM
                                    HALL_TICKET_VALIDATION
                                WHERE
                                    STUDENT_ID = ?STUDENT_ID
                                        AND ACADEMIC_YEAR = ?AC_YEAR
                                        AND EXAM_SETTING_ID = (SELECT 
                                            EXAM_SETTING_ID
                                        FROM
                                            EXAM_SETTING
                                        WHERE
                                            IS_ACTIVE = 1)
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.SaveHallTicketValidation:
                    {
                        query = @"INSERT INTO HALL_TICKET_VALIDATION
                                        (
                                        STUDENT_ID,
                                        REASON,
                                        ACADEMIC_YEAR,
                                        EXAM_SETTING_ID)
                                        VALUES
                                        (
                                        ?STUDENT_ID,
                                        ?REASON,
                                        ?AC_YEAR,
                                        (SELECT EXAM_SETTING_ID FROM EXAM_SETTING WHERE IS_ACTIVE=1));";
                        break;
                    }
                case FeeSQLCommands.UpdateHallTicketValidation:
                    {
                        query = @"UPDATE HALL_TICKET_VALIDATION
                                        SET
                                        REASON=?REASON
                                        WHERE HALL_TICKET_VALIDATION_ID= ?HALL_TICKET_VALIDATION_ID;";
                        break;
                    }
                case FeeSQLCommands.DeleteFeeAdvancePayment:
                    {
                        query = @"UPDATE FEE_ADVANCE_PAYMENT
                                        SET
                                        IS_DELETED=1
                                        WHERE ADVANCE_ID=?ADVANCE_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFrequencyIdByFeeMode:
                    {
                        query = @"SELECT 
                                        group_concat(FEE_FREQUENCY_FEE_MODE_ID )  AS FREQUENCY_ID
                                    FROM
                                        FEE_FREQUENCY_FEE_MODE AS FFF
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID=FFF.FREQUENCY_ID AND SFF.ACADEMIC_YEAR=FFF.ACADEMIC_YEAR
		                                        left JOIN
	                                    SUP_SEMESTER_TYPE AS SMT ON SMT.SEMESTER_TYPE_ID=SFF.SEMESTER_TYPE 
                                    WHERE
                                        FFF.FEE_MODE = ?FEE_MODE;";
                        break;
                    }
                case FeeSQLCommands.FetchFrequencyIdByFeeModeForMessFee:
                    {
                        query = @"SELECT 
                                        group_concat(FEE_FREQUENCY_FEE_MODE_ID )  AS FREQUENCY_ID
                                    FROM
                                        FEE_FREQUENCY_FEE_MODE AS FFF
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID=FFF.FREQUENCY_ID AND SFF.ACADEMIC_YEAR=FFF.ACADEMIC_YEAR
		                                        left JOIN
	                                    SUP_SEMESTER_TYPE AS SMT ON SMT.SEMESTER_TYPE_ID=SFF.SEMESTER_TYPE 
                                    WHERE
                                        FFF.FEE_MODE = ?FEE_MODE AND FFF.IS_DELETED !=1";
                        break;
                    }
                case FeeSQLCommands.FetchStudentAccountFeeInfo:
                    {
                        query = @"SELECT 
                                    FMH.FREQUENCY_ID,
                                    FS.STUDENT_ID,
                                    SUM((IFNULL(FS.CREDIT, 0) - IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    FF.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    SFM.MAIN_HEAD AS HEAD,
                                    FT.FREQUENCY_TYPE_ID,
                                    FMH.FEE_ROOT_ID,
                                    FMH.BANK_ACCOUNT_ID,
                                    FS.ACADEMIC_YEAR,
                                    SP.STU_EMAILID,
                                    SP.STU_MOBILENO,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,FT.FREQUENCY_TYPE as FREQUENCY_NAME,FS.FEE_MAIN_HEAD_ID,FBA.ACCOUNT_LABEL,FS.HEAD AS HEAD_ID
                                FROM
                                    FEE_STUDENT_ACCOUNT AS FS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = FS.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                        AND PG.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FMH ON FMH.APPLICATION_TYPE_ID = SP.APPTYPE_ID
                                        AND FS.HEAD = FMH.HEAD_ID
                                        AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        AND FT.FEE_ROOT_ID = ?FEE_ROOT_ID
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                        LEFT JOIN 
                                     fee_eb_accounts AS FBA ON FBA.BANK_ACCOUNT_ID = FMH.BANK_ACCOUNT_ID
                                WHERE
                                    FS.STUDENT_ID = ?STUDENT_ID
                                        AND FT.FREQUENCY_TYPE_ID = ?FREQUENCY_TYPE_ID AND FFF.FEE_FREQUENCY_FEE_MODE_ID= ?FREQUENCY_ID
                                        AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FS.IS_DELETED != 1
                                GROUP BY FS.STUDENT_ID , FS.FREQUENCY_ID, FMH.MAIN_HEAD_ID
                                ;";
                        break;
                    }
                case FeeSQLCommands.FetchBusStudentAccountFeeInfo:
                    {
                        query = @"SELECT 
                                    FMH.FREQUENCY_ID,
                                    FS.STUDENT_ID,
                                    SUM((IFNULL(FS.CREDIT, 0) - IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    FF.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    SFM.MAIN_HEAD AS HEAD,
                                    FT.FREQUENCY_TYPE_ID,
                                    FMH.FEE_ROOT_ID,
                                    FMH.BANK_ACCOUNT_ID,
                                    FS.ACADEMIC_YEAR,
                                    SP.STU_EMAILID,
                                    SP.STU_MOBILENO,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,FT.FREQUENCY_TYPE as FREQUENCY_NAME,FS.FEE_MAIN_HEAD_ID,FBA.ACCOUNT_LABEL,FS.HEAD AS HEAD_ID
                                FROM
                                    FEE_STUDENT_ACCOUNT AS FS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = FS.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                        AND PG.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FMH ON FS.FREQUENCY_ID = fmh.FREQUENCY_ID
                                        AND FS.HEAD = FMH.HEAD_ID
                                        AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        AND FT.FEE_ROOT_ID = ?FEE_ROOT_ID
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                        LEFT JOIN 
                                     fee_eb_accounts AS FBA ON FBA.BANK_ACCOUNT_ID = FMH.BANK_ACCOUNT_ID
                                WHERE
                                    FS.STUDENT_ID = ?STUDENT_ID
                                        AND FT.FREQUENCY_TYPE_ID = ?FREQUENCY_TYPE_ID AND FFF.FEE_FREQUENCY_FEE_MODE_ID= ?FREQUENCY_ID
                                        AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FS.IS_DELETED != 1
                                GROUP BY FS.STUDENT_ID , FS.FREQUENCY_ID, FMH.MAIN_HEAD_ID
                                ;";
                        break;
                    }
                case FeeSQLCommands.FetchExamStudentAccountFeeInfo:
                    {
                        query = @"SELECT 
                                    FMH.FREQUENCY_ID,
                                    FS.STUDENT_ID,
                                    SUM((IFNULL(FS.CREDIT, 0) - IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    FF.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    SFM.MAIN_HEAD AS HEAD,
                                    FT.FREQUENCY_TYPE_ID,
                                    FMH.FEE_ROOT_ID,
                                    FMH.BANK_ACCOUNT_ID,
                                    FS.ACADEMIC_YEAR,
                                    SP.STU_EMAILID,
                                    SP.STU_MOBILENO,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,FT.FREQUENCY_TYPE as FREQUENCY_NAME,FS.FEE_MAIN_HEAD_ID,FBA.ACCOUNT_LABEL,FS.HEAD AS HEAD_ID
                                FROM
                                    FEE_STUDENT_ACCOUNT AS FS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = FS.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                        AND PG.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FMH ON  FS.FREQUENCY_ID = fmh.FREQUENCY_ID
                                        AND FS.HEAD = FMH.HEAD_ID
                                        AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        AND FT.FEE_ROOT_ID = ?FEE_ROOT_ID
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                        LEFT JOIN 
                                     fee_eb_accounts AS FBA ON FBA.BANK_ACCOUNT_ID = FMH.BANK_ACCOUNT_ID
                                WHERE
                                    FS.STUDENT_ID = ?STUDENT_ID
                                        AND FT.FREQUENCY_TYPE_ID = ?FREQUENCY_TYPE_ID AND FFF.FEE_FREQUENCY_FEE_MODE_ID= ?FREQUENCY_ID
                                        AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FS.IS_DELETED != 1
                                GROUP BY FS.STUDENT_ID , FS.FREQUENCY_ID, FMH.MAIN_HEAD_ID
                                ;";
                        break;
                    }
                case FeeSQLCommands.FetchStudentAccountFeeDueInfo:
                    {
                        query = @"SELECT 
                                        FMH.FREQUENCY_ID,
                                        FS.STUDENT_ID,
                                        SUM((IFNULL(FS.CREDIT, 0) - IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                        FF.FREQUENCY_NAME,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        SFM.MAIN_HEAD AS HEAD,
                                        FT.FREQUENCY_TYPE_ID,
                                        FMH.FEE_ROOT_ID,
                                        -- FMH.BANK_ACCOUNT_ID,
                                        FS.ACADEMIC_YEAR,
                                        SP.STU_EMAILID,
                                        SP.STU_MOBILENO,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        FT.FREQUENCY_TYPE AS FREQUENCY_NAME,
                                        FS.FEE_MAIN_HEAD_ID,
                                        FBA.ACCOUNT_LABEL,
                                        FS.HEAD AS HEAD_ID
                                    FROM
                                        FEE_STUDENT_ACCOUNT AS FS
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = FS.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        FEE_MAIN_HEADS AS FMH ON FMH.FEE_MAIN_HEAD_ID = FS.FEE_MAIN_HEAD_ID
                                            AND FS.HEAD = FMH.HEAD_ID
                                            AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FMH.IS_DELETED != 1
                                            INNER JOIN
                                        FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                            AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                            AND FT.FEE_ROOT_ID =?FEE_ROOT_ID
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                            LEFT JOIN
                                        SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                            LEFT JOIN
                                        SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                            LEFT JOIN
                                        FEE_EB_ACCOUNTS AS FBA ON FBA.BANK_ACCOUNT_ID = FMH.BANK_ACCOUNT_ID
                                    WHERE
                                        FS.STUDENT_ID = ?STUDENT_ID
                                            AND FT.FREQUENCY_TYPE_ID = ?FREQUENCY_TYPE_ID
                                            AND FFF.FEE_FREQUENCY_FEE_MODE_ID = ?FREQUENCY_ID
                                            AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FS.IS_DELETED != 1
                                    GROUP BY FS.STUDENT_ID , FS.FREQUENCY_ID , FMH.MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchArrearStudentAccountFeeInfo:
                    {
                        query = @"SELECT 
                                    FMH.FREQUENCY_ID,
                                    FS.STUDENT_ID,
                                    SUM((IFNULL(FS.CREDIT, 0) - IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    FF.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    SFM.MAIN_HEAD AS HEAD,
                                    FT.FREQUENCY_TYPE_ID,
                                    FMH.FEE_ROOT_ID,
                                    FS.ACADEMIC_YEAR,
                                    FMH.BANK_ACCOUNT_ID,
                                    SP.STU_EMAILID,
                                    SP.STU_MOBILENO,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,FT.FREQUENCY_TYPE as FREQUENCY_NAME,FS.FEE_MAIN_HEAD_ID,FBA.ACCOUNT_LABEL,FS.HEAD AS HEAD_ID
                                FROM
                                    FEE_STUDENT_ACCOUNT AS FS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = FS.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                 
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FMH ON FMH.APPLICATION_TYPE_ID = SP.APPTYPE_ID
                                        AND FS.HEAD = FMH.HEAD_ID
                                        AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        AND FT.FEE_ROOT_ID = ?FEE_ROOT_ID
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                        LEFT JOIN 
                                     fee_eb_accounts AS FBA ON FBA.BANK_ACCOUNT_ID = FMH.BANK_ACCOUNT_ID
                                WHERE
                                    FS.STUDENT_ID = ?STUDENT_ID
                                        AND FT.FREQUENCY_TYPE_ID = ?FREQUENCY_TYPE_ID
                                        AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FS.IS_DELETED != 1
                                GROUP BY FS.STUDENT_ID , FS.FREQUENCY_ID, FMH.MAIN_HEAD_ID
                                ;";
                        break;
                    }
                case FeeSQLCommands.FetchAllFeeDuesByStudentId:
                    {
                        query = @"SELECT 
                                    FMH.FREQUENCY_ID,
                                    FS.STUDENT_ID,
                                    FMH.FEE_MAIN_HEAD_ID,
                                    SUM((IFNULL(FS.CREDIT, 0) - IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    SUM(IFNULL(FS.DEBIT, 0)) AS DEBIT,
                                    MAX(FS.TRANSACTION_ID) AS TRANSACTION_ID,
                                    FF.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    CONCAT(IFNULL(H.HEAD, ''),
                                            ' (',
                                            IFNULL(H.HEAD_CODE, ''),
                                            ') -',
                                            FS.ACADEMIC_YEAR) AS HEAD,
                                    SP.STU_EMAILID,
                                    FMH.HEAD_ID,
                                    SP.STU_MOBILENO,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    FS.ACADEMIC_YEAR,
                                    FT.FREQUENCY_TYPE,
                                    FT.FREQUENCY_TYPE_ID,
                                    FT.FEE_ROOT_ID
                                FROM
                                    FEE_STUDENT_ACCOUNT AS FS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = FS.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FMH ON FS.FREQUENCY_ID = FMH.FREQUENCY_ID
                                        AND FS.HEAD = FMH.HEAD_ID
                                        AND FMH.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR = FMH.ACADEMIC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        INNER JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        INNER JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                WHERE
                                    FS.STUDENT_ID = ?STUDENT_ID
                                        AND FS.IS_DELETED != 1
                                GROUP BY FS.STUDENT_ID , FS.FREQUENCY_ID , FMH.HEAD_ID , FMH.FEE_MAIN_HEAD_ID 
                                ORDER BY ACADEMIC_YEAR, BALANCE DESC;";
                        break;
                    }
                case FeeSQLCommands.FetchAllFeeDuesByStaffId:
                    {
                        query = @"SELECT 
                                    FMH.FREQUENCY_ID,
                                    FS.STAFF_ID AS STUDENT_ID,
                                    FMH.FEE_MAIN_HEAD_ID,
                                    SUM((IFNULL(FS.CREDIT, 0) - IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    SUM(IFNULL(FS.DEBIT, 0)) AS DEBIT,
                                    MAX(FS.TRANSACTION_ID) AS TRANSACTION_ID,
                                    FF.FREQUENCY_NAME,
                                    SP.FIRST_NAME AS FIRST_NAME,
                                    CONCAT(IFNULL(H.HEAD, ''),
                                            ' (',
                                            IFNULL(H.HEAD_CODE, ''),
                                            ') -',
                                            FS.ACADEMIC_YEAR) AS HEAD,
                                    SP.KNOWN_AS AS STU_EMAILID,
                                    FMH.HEAD_ID,
                                    FMH.BANK_ACCOUNT_ID,
                                    SP.USER_FIELD1 AS STU_MOBILENO,
                                    SP.STAFF_CODE AS ROLL_NO,
                                    SP.STAFF_CODE AS REGISTER_NO,
                                    FS.ACADEMIC_YEAR,
                                    FT.FREQUENCY_TYPE,
                                    FT.FREQUENCY_TYPE_ID,
                                    FT.FEE_ROOT_ID
                                FROM
                                    fee_staff_account AS FS
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = FS.STAFF_ID
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FMH ON FS.FREQUENCY_ID = FMH.FREQUENCY_ID
                                        AND FS.HEAD = FMH.HEAD_ID
                                        AND FMH.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR = FMH.ACADEMIC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        INNER JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        INNER JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                WHERE
                                    FS.STAFF_ID = ?STUDENT_ID
                                        AND FS.IS_DELETED != 1
                                GROUP BY FS.STAFF_ID , FS.FREQUENCY_ID , FMH.HEAD_ID , FMH.FEE_MAIN_HEAD_ID 
                                ORDER BY ACADEMIC_YEAR, BALANCE DESC;";
                        break;
                    }
                case FeeSQLCommands.FecthMessFeesByStudentIdAndFeeRootId:
                    {
                        query = @"SELECT 
                                    FMH.FREQUENCY_ID,
                                    FS.STUDENT_ID,
                                    SUM((IFNULL(FS.CREDIT, 0) - IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    SUM(IFNULL(FS.DEBIT, 0)) AS DEBIT,
                                    MAX(FS.TRANSACTION_ID) AS TRANSACTION_ID,
                                    FF.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    CONCAT(IFNULL(H.HEAD, ''),
                                            ' (',
                                            IFNULL(H.HEAD_CODE, ''),
                                            ') -',
                                            FS.ACADEMIC_YEAR) AS HEAD,
                                    SP.STU_EMAILID,
                                    FMH.HEAD_ID,
                                    SP.STU_MOBILENO,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    FS.ACADEMIC_YEAR,
                                    FT.FREQUENCY_TYPE
                                FROM
                                    FEE_STUDENT_ACCOUNT AS FS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = FS.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                        AND PG.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FMH ON fs.frequency_id = fmh.frequency_id
                                        AND FS.HEAD = FMH.HEAD_ID
                                        AND FMH.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR = FMH.ACADEMIC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        AND FT.FEE_ROOT_ID = ?FEE_ROOT_ID
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        INNER JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        INNER JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                WHERE
                                    FS.STUDENT_ID = ?STUDENT_ID
                                        AND FT.FREQUENCY_TYPE_ID = ?FREQUENCY_TYPE_ID
                                        AND FS.IS_DELETED != 1
                                GROUP BY FS.STUDENT_ID , FS.FREQUENCY_ID , FMH.HEAD_ID 
                                ORDER BY ACADEMIC_YEAR,BALANCE DESC;";
                        break;
                    }
                case FeeSQLCommands.CheckIsExistingAccount:
                    {
                        query = @"SELECT 
                                        F_STUDENT_AC_ID
                                    FROM
                                        FEE_STUDENT_ACCOUNT
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID AND FREQUENCY_ID = ?FREQUENCY_ID
                                            AND IS_DELETED != 1
                                            AND IS_CANCELLED_HEAD != 1;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeStructureForStudentAccount:
                    {
                        query = @"SELECT 
										FS.FEE_STRUCTURE_ID,
                                        SC.STUDENT_ID,
                                        FS.FREQUENCY,
                                        FS.CLASS,
                                        FS.HEAD,
                                        SH.HEAD AS 'HEAD_NAME',
                                        FS.AMOUNT,
                                        FS.FEE_MODE,
                                        FS.FEE_CATEGORY,
                                        FS.PROGRAMME,
                                        PG.PROGRAMME_MODE,
                                        PG.SHIFT
                                    FROM
                                        FEE_STRUCTURE AS FS
                                            INNER JOIN
										CP_CLASSES AS CLS ON CLS.CLASS_ID=FS.CLASS
											INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=CLS.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.PROGRAM_ID = PG.PROGRAMME_GROUP_ID
											INNER JOIN
										STU_CLASS AS SC ON SC.STUDENT_ID=SP.STUDENT_ID AND SC.ACADEMIC_YEAR=?AC_YEAR
											INNER JOIN
										FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID=FS.FREQUENCY
											INNER JOIN
										SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID=FFF.FREQUENCY_ID
											INNER JOIN
										SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID=SFF.SEMESTER_TYPE AND SST.IS_ACTIVE=1
                                             INNER JOIN
                                        SUP_HEAD AS SH ON SH.HEAD_ID = FS.HEAD
                                    WHERE
                                        SC.STUDENT_ID = ?STUDENT_ID AND FREQUENCY=?FREQUENCY
                                            AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FS.IS_DELETED != 1 AND SP.IS_DELETED!=1 AND PG.IS_DELETED!=1;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeStudentAccountByStudentId:
                    {
                        query = @"SELECT 
                                    FM.FREQUENCY_ID,
                                    FS.STUDENT_ID,
                                    SUM((IFNULL(FS.CREDIT, 0) - IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    FR.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
									SP.STU_EMAILID,
                                    SP.STU_MOBILENO,
                                    SP.REGISTER_NO,
                                    SP.ROLL_NO,
                                    FS.HEAD,
                                    H.HEAD as HEAD_NAME
                                FROM
									 STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID= SC.STUDENT_ID AND SC.ACADEMIC_YEAR=?AC_YEAR
                                        INNER JOIN
                                    FEE_STUDENT_ACCOUNT AS FS ON FS.STUDENT_ID = SC.STUDENT_ID
                                        INNER JOIN
                                 CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID= SC.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FM ON FM.FREQUENCY_ID = FS.FREQUENCY_ID
                                        AND FM.HEAD_ID = FS.HEAD
                                        AND PG.SHIFT = FM.SHIFT
                                        AND PG.PROGRAMME_MODE = FM.PROGRAMME_MODE
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFM ON FFM.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FR ON FR.FREQUENCY_ID = FFM.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = FR.SEMESTER_TYPE
                                        AND ST.IS_ACTIVE = 1
                                        INNER JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FM.MAIN_HEAD_ID
                                        INNER JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FM.HEAD_ID
                                WHERE
                                    SC.STUDENT_ID= ?STUDENT_ID
                                        AND FM.ACADEMIC_YEAR = ?AC_YEAR AND FS.FREQUENCY_ID=?FREQUENCY_ID AND SC.IS_DELETED!=1 AND SP.IS_DELETED!=1
                                GROUP BY FS.STUDENT_ID , FS.HEAD;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeStudentAccountByStudentIdForSemesterFee:
                    {
                        query = @"SELECT 
                                     FMH.FREQUENCY_ID,
                                    FS.STUDENT_ID,
                                    SUM((IFNULL(FS.CREDIT, 0) - IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    FF.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
									SP.STU_EMAILID,
                                    SP.STU_MOBILENO,
                                    SP.REGISTER_NO,
                                    SP.ROLL_NO,
                                    FS.HEAD,
                                    SFM.MAIN_HEAD as HEAD_NAME
                                FROM
									 FEE_STUDENT_ACCOUNT AS FS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = FS.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                        AND PG.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FMH ON FMH.APPLICATION_TYPE_ID = PG.APPTYPE_ID
                                        AND FMH.PROGRAMME_MODE = PG.PROGRAMME_MODE
                                        AND FMH.SHIFT = PG.SHIFT
                                        AND FS.HEAD = FMH.HEAD_ID
                                        AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        AND FT.FEE_ROOT_ID = ?FEE_ROOT_ID
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                WHERE
                                    SP.STUDENT_ID= ?STUDENT_ID
                                         AND FS.FREQUENCY_ID IN (?FREQUENCY_ID) 
                                GROUP BY FS.STUDENT_ID ,FS.FREQUENCY_ID, FMH.MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchMerchantInfo:
                    {
                        query = @"SELECT 
                                        MERCHANT_ACCOUNT_INFO,
                                        ACCOUNT_ID,
                                        USERNAME,
                                        `PASSWORD`,
                                        `KEY`,
                                        SALT,
                                        BASE_URL,
                                        AUTHORIZATION,
                                        API_TYPE,
                                        HASH_SEQUENCE,
                                        surl,
                                        furl,
                                        curl
                                    FROM
                                        fee_merchant_account_info
                                    WHERE
                                        IS_DELETED != 1 AND IS_ACTIVE = 1;";
                        break;
                    }
                case FeeSQLCommands.FetchPayUresponseByTxnid:
                    {
                        query = @"SELECT 
                                        PAYU_RESPONSE_ID,
                                        `key`,
                                        txnid,
                                        amount,
                                        productinfo,
                                        firstname,
                                        email,
                                        phone,
                                        lastname,
                                        address1,
                                        address2,
                                        city,
                                        state,
                                        country,
                                        zipcode,
                                        udf1,
                                        udf2,
                                        udf3,
                                        udf4,
                                        udf5,
                                        udf6,
                                        udf7,
                                        udf8,
                                        udf9,
                                        udf10,
                                        hash,
                                        payment_source,
                                        PG_TYPE,
                                        bank_ref_num,
                                        bankcode,
                                        error,
                                        error_Message,
                                        name_on_card,
                                        cardnum,
                                        mode,
                                        mihpayid,
                                        status,
                                        unmappedstatus,
                                        addedon,
                                        additionalCharges
                                    FROM
                                        fee_payu_response
                                    WHERE
                                        txnid =?TXNID;";
                        break;
                    }
                case FeeSQLCommands.FetchTotalAndDebitByUserIdAndFrequencyId:
                    {
                        query = @"SELECT 
                        FMH.FREQUENCY_ID,
                        SP.STUDENT_ID,
                        SUM(IFNULL(FC.PAID_AMOUNT, 0)) AS BALANCE,
                        FF.FREQUENCY_NAME,
                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                ' ',
                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                        SFM.MAIN_HEAD,
                        SP.STU_EMAILID,
                        SP.STU_MOBILENO,
                        SP.ROLL_NO,
                        SP.REGISTER_NO,
                        F.RECEIPT_NO,
                        F.TRANSACTION_ID,
                        F.FREQUENCY,
                        F.COLLECTED,F.FEE_TRANSACTION_BANK,
                        DATE_FORMAT(F.PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,
                        SPM.PAYMENT_MODE,
                        F.ACADEMIC_YEAR,P.PROGRAMME_NAME
                    FROM
                        FEE_TRANSACTION AS F
                            INNER JOIN
                        FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = F.TRANSACTION_ID
                            INNER JOIN
                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = F.STUDENT_ID
                            INNER JOIN
                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                            AND PG.IS_DELETED != 1
                            INNER JOIN
                        FEE_MAIN_HEADS AS FMH ON FMH.APPLICATION_TYPE_ID = PG.APPTYPE_ID
                            AND FMH.PROGRAMME_MODE = PG.PROGRAMME_MODE
                            AND FMH.SHIFT = PG.SHIFT
                            AND FC.HEAD = FMH.HEAD_ID
                            AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                            AND FMH.IS_DELETED != 1
                            INNER JOIN
                        FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                            AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                            AND FFF.FEE_FREQUENCY_FEE_MODE_ID = F.FREQUENCY
                            INNER JOIN
                        SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                            AND FT.FEE_ROOT_ID = ?FEE_ROOT_ID
                            INNER JOIN
                        SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                            LEFT JOIN
                        SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                            LEFT JOIN
                        SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                            LEFT JOIN
                        SUP_PAYMENT_MODE AS SPM ON SPM.PAYMENT_MODE_ID = F.PAYMENT_MODE
                            LEFT JOIN
                        cp_programme AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                    WHERE
                        SP.STUDENT_ID = ?STUDENT_ID
                            AND F.FREQUENCY IN (?FREQUENCY_ID)
                    GROUP BY F.STUDENT_ID , f.FREQUENCY , FMH.MAIN_HEAD_ID;";
                        break;
                    }

                case FeeSQLCommands.FetchMessCollectionList:
                    {
                        query = @"SELECT 
                        FMH.FREQUENCY_ID,
                        SP.STUDENT_ID,
                        sum(IFNULL(FC.PAID_AMOUNT, 0)) AS BALANCE,
                        FF.FREQUENCY_NAME,
                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                ' ',
                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                        SFM.MAIN_HEAD,
                        SP.STU_EMAILID,
                        SP.STU_MOBILENO,
                        SP.ROLL_NO,
                        SP.REGISTER_NO,
                        F.RECEIPT_NO,
                        F.TRANSACTION_ID,
                        F.FREQUENCY,
                        F.COLLECTED,F.FEE_TRANSACTION_BANK,
                        DATE_FORMAT(F.PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,
                        SPM.PAYMENT_MODE,
                        F.ACADEMIC_YEAR,P.PROGRAMME_NAME
                    FROM
                        FEE_TRANSACTION AS F
                            INNER JOIN
                        FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = F.TRANSACTION_ID
                            INNER JOIN
                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = F.STUDENT_ID
                            INNER JOIN
                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                            AND PG.IS_DELETED != 1
                            INNER JOIN
                        FEE_MAIN_HEADS AS FMH ON f.FREQUENCY =FMH.FREQUENCY_ID
                           AND FMH.FEE_ROOT_ID=?FEE_ROOT_ID
                            AND FC.HEAD = FMH.HEAD_ID
                            AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                            AND FMH.IS_DELETED != 1
                            INNER JOIN
                        FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                            AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                            AND FFF.FEE_FREQUENCY_FEE_MODE_ID = F.FREQUENCY                            
                            INNER JOIN
                        SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                            AND FT.FEE_ROOT_ID = ?FEE_ROOT_ID
                            INNER JOIN
                        SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                            LEFT JOIN
                        SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                            LEFT JOIN
                        SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                            LEFT JOIN
                        SUP_PAYMENT_MODE AS SPM ON SPM.PAYMENT_MODE_ID = F.PAYMENT_MODE
                            LEFT JOIN
                        cp_programme AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                    WHERE
                        SP.STUDENT_ID = ?STUDENT_ID
                            AND F.FREQUENCY IN (?FREQUENCY)
                    GROUP BY F.STUDENT_ID , f.FREQUENCY , FMH.MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeStudentAccountByTransactionIdAndFrequencyId:
                    {
                        query = @"SELECT
                                FSA.TRANSACTION_ID,
                                FSA.STUDENT_ID,
                                FSA.FREQUENCY_ID,
                                SMH.MAIN_HEAD,
                                FSA.CREDIT,
                                SUM(FSA.DEBIT) AS DEBIT
                            FROM
                                FEE_STUDENT_ACCOUNT AS FSA
                                    INNER JOIN
                                SUP_HEAD AS SFM ON SFM.HEAD_ID = FSA.HEAD
                                    AND FSA.FREQUENCY_ID = ?FREQUENCY_ID
                                    AND FSA.TRANSACTION_ID = ?TRANSACTION_ID
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = FSA.STUDENT_ID AND SC.ACADEMIC_YEAR=?AC_YEAR
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID= SC.STUDENT_ID
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS APP ON APP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                    INNER JOIN
                                FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FSA.FREQUENCY_ID
                                    AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                                    INNER JOIN
                                SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                    INNER JOIN
                                SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = SFF.SEMESTER_TYPE
                                    AND IS_ACTIVE = 1
                                    INNER JOIN
                                FEE_MAIN_HEADS AS FMH ON FMH.FREQUENCY_ID = FSA.FREQUENCY_ID
                                AND SFM.HEAD_ID=FMH.HEAD_ID
                                    AND APP.PROGRAMME_MODE = FMH.PROGRAMME_MODE
                                    AND FMH.SHIFT = APP.SHIFT
                                    INNER JOIN
                                SUP_FEE_MAIN_HEAD AS SMH ON SMH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                            WHERE
                                FSA.TRANSACTION_ID = ?TRANSACTION_ID
                                    AND FSA.IS_DELETED != 1
                                    AND FSA.IS_CANCELLED_HEAD != 1
                                    AND FSA.IS_REFUND != 1
                                    AND SFM.IS_DELETED != 1
                                    AND FSA.FREQUENCY_ID = ?FREQUENCY_ID
                                    AND FSA.ACADEMIC_YEAR = ?AC_YEAR
                            GROUP BY FSA.STUDENT_ID , SMH.MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeFrequencyFeeMode:
                    {
                        query = @"SELECT 
                                        FEE_FREQUENCY_FEE_MODE_ID,
                                        SFF.FREQUENCY_NAME,
                                        SFT.FREQUENCY_TYPE
                                    FROM
                                        FEE_FREQUENCY_FEE_MODE AS FFF
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                            INNER JOIN
                                        sup_semester_type AS SST ON SST.SEMESTER_TYPE_ID = SFF.SEMESTER_TYPE
                                    WHERE
                                        SST.IS_ACTIVE = 1
                                            AND SFF.IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeModeByFrequencyId:
                    {
                        query = @"SELECT 
                                        FEE_FREQUENCY_FEE_MODE_ID,
                                        FFF.FREQUENCY_ID,
                                        SFT.FREQUENCY_TYPE_ID,
                                        SFT.FREQUENCY_TYPE,
                                        IF(FEE_FREQUENCY_FEE_MODE_ID IS NOT NULL,
                                            'SELECTED',
                                            '') AS STATUS
                                    FROM
                                        SUP_FREQUENCY_TYPE AS SFT
                                            LEFT JOIN
                                        FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_MODE = SFT.FREQUENCY_TYPE_ID
                                            AND SFT.IS_DELETED != 1
                                            AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FFF.FREQUENCY_ID = ?FREQUENCY_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeFrequencyFeeModeByFrequencyId:
                    {
                        query = @"SELECT 
                                        FEE_FREQUENCY_FEE_MODE_ID,
                                        FREQUENCY_ID,
                                        FEE_MODE
                                    FROM
                                        FEE_FREQUENCY_FEE_MODE
                                    WHERE
                                        ACADEMIC_YEAR=?AC_YEAR AND  FREQUENCY_ID=?FREQUENCY_ID;";
                        break;
                    }
                case FeeSQLCommands.SaveFeeFrequencyFeeMode:
                    {
                        query = @"INSERT INTO FEE_FREQUENCY_FEE_MODE
                                (
                                FREQUENCY_ID,
                                FEE_MODE,
                                ACADEMIC_YEAR
                                )
                                VALUES
                                (
                                ?FREQUENCY_ID,
                                ?FEE_MODE,
                                ?ACADEMIC_YEAR
                                );";
                        break;
                    }
                case FeeSQLCommands.FetchFeeStructureByAcademicYearAndFrequencyId:
                    {
                        query = @"SELECT 
                                        FS.FEE_STRUCTURE_ID,
                                        FS.ACADEMIC_YEAR,
                                        FS.FREQUENCY,
                                        SFF.FREQUENCY_NAME,
                                        FS.CLASS,                                       
                                        SH.HEAD_ID,
                                        SH.HEAD,
                                        FS.AMOUNT,
                                        SFC.FEE_CATEGORY_ID,
                                        SFC.FEE_CATEGORY,
                                        SST.FREQUENCY_TYPE AS FEE_MODE,FS.FEE_MAIN_HEAD_ID
                                    FROM
                                        FEE_STRUCTURE AS FS
                                        INNER JOIN FEE_MAIN_HEADS AS MH ON MH.FEE_MAIN_HEAD_ID=FS.FEE_MAIN_HEAD_ID AND MH.IS_DELETED!=1                                          
                                            INNER JOIN
                                        FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY AND FFF.ACADEMIC_YEAR=?AC_YEAR
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS SFF ON FFF.FREQUENCY_ID = SFF.FREQUENCY_ID AND SFF.ACADEMIC_YEAR=?AC_YEAR
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS SST ON SST.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                            INNER JOIN
                                        SUP_HEAD AS SH ON SH.HEAD_ID = FS.HEAD
                                            INNER JOIN
                                        SUP_FEE_CATEGORY AS SFC ON SFC.FEE_CATEGORY_ID = FS.FEE_CATEGORY                                           
                                    WHERE
                                        FS.IS_DELETED != 1
                                            AND SH.IS_DELETED != 1
                                           AND FS.CLASS_YEAR_ID=?CLASS_YEAR_ID AND  FS.PROGRAMME=?PROGRAMME
                                            AND FS.FREQUENCY = ?FREQUENCY
                                            AND FS.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case FeeSQLCommands.BindFeeStructureForEditByClass:
                    {
                        query = @"SELECT 
                                        FS.FEE_STRUCTURE_ID,
                                        FMH.FEE_MAIN_HEAD_ID,
                                        FS.ACADEMIC_YEAR,
                                        FMH.APPLICATION_TYPE_ID,
                                        FMH.PROGRAMME_MODE,
                                        FMH.SHIFT,
                                        FMH.FREQUENCY_ID,
                                        SH.HEAD_ID,
                                        SH.HEAD,
                                        SFM.MAIN_HEAD,
                                        FS.AMOUNT,
                                        FMH.BANK_ACCOUNT_ID,
                                        FMH.FEE_CATEGORY_ID,
                                        fc.FEE_CATEGORY as FEE_CATEGORY_NAME,
                                       CONCAT(BA.PASSBOOK_NO,' - ',B.BANK_NAME) AS PASSBOOK_NO
                                    FROM
                                        FEE_MAIN_HEADS AS FMH
                                            INNER JOIN
                                        cp_programme_group AS PG ON FMH.APPLICATION_TYPE_ID = PG.APPTYPE_ID
                                            AND FMH.PROGRAMME_MODE = PG.PROGRAMME_MODE
                                            AND FMH.SHIFT = PG.SHIFT
                                            AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                                            LEFT JOIN
                                        FEE_STRUCTURE AS FS ON  FS.FEE_MAIN_HEAD_ID = FMH.FEE_MAIN_HEAD_ID
                                            AND FS.FREQUENCY = FMH.FREQUENCY_ID                                            
                                            AND FS.PROGRAMME = PG.PROGRAMME_GROUP_ID
                                            AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FS.IS_DELETED != 1
                                            AND FS.CLASS_YEAR_ID = ?CLASS_YEAR_ID
                                            INNER JOIN
                                        SUP_HEAD AS SH ON SH.HEAD_ID = FMH.HEAD_ID
                                            INNER JOIN
                                        SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT join sup_fee_category as fc on fc.FEE_CATEGORY_ID=FMH.FEE_CATEGORY_ID
                                        LEFT JOIN sup_bank_account AS BA ON BA.BANK_ACCOUNT_ID=FMH.BANK_ACCOUNT_ID
                                        LEFT JOIN sup_bank AS B ON B.BANK_ID=BA.BANK
                                    WHERE
                                        FMH.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FMH.FREQUENCY_ID = ?FREQUENCY
                                            AND PG.PROGRAMME_GROUP_ID = ?PROGRAMME;";
                        break;
                    }
                case FeeSQLCommands.BindFeeHostelStructure:
                    {
                        query = @"SELECT 
                                        FS.FEE_STRUCTURE_ID,
                                        FMH.FEE_MAIN_HEAD_ID,
                                        FS.ACADEMIC_YEAR,
                                        FMH.APPLICATION_TYPE_ID,
                                        FMH.PROGRAMME_MODE,
                                        FMH.SHIFT,
                                        FMH.FREQUENCY_ID,
                                        SH.HEAD_ID,
                                        SH.HEAD,
                                        SFM.MAIN_HEAD,
                                        FS.AMOUNT,
                                        FMH.BANK_ACCOUNT_ID,
                                        FMH.FEE_CATEGORY_ID,
                                        fc.FEE_CATEGORY as FEE_CATEGORY_NAME,
                                       CONCAT(BA.PASSBOOK_NO,' - ',B.BANK_NAME) AS PASSBOOK_NO
                                    FROM
                                        FEE_MAIN_HEADS AS FMH
                                            left join
                                        fee_structure_for_hostel_and_mess AS FS ON  FS.FEE_MAIN_HEAD_ID = FMH.FEE_MAIN_HEAD_ID
                                            AND FS.FREQUENCY = FMH.FREQUENCY_ID                                            
                                            
                                            AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FS.IS_DELETED != 1
                                           
                                            INNER JOIN
                                        SUP_HEAD AS SH ON SH.HEAD_ID = FMH.HEAD_ID
                                            INNER JOIN
                                        SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT join sup_fee_category as fc on fc.FEE_CATEGORY_ID=FMH.FEE_CATEGORY_ID
                                        LEFT JOIN sup_bank_account AS BA ON BA.BANK_ACCOUNT_ID=FMH.BANK_ACCOUNT_ID
                                        LEFT JOIN sup_bank AS B ON B.BANK_ID=BA.BANK
                                    WHERE
                                        FMH.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FMH.FREQUENCY_ID = ?FREQUENCY AND FMH.IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.BindHostelAndMessStructureForAccountCreation:
                    {
                        query = @"SELECT 
                                FS.FEE_STRUCTURE_ID,
                                FMH.FEE_MAIN_HEAD_ID,
                                FS.ACADEMIC_YEAR,
                                FMH.APPLICATION_TYPE_ID,
                                FMH.PROGRAMME_MODE,
                                FMH.SHIFT,
                                FMH.FREQUENCY_ID,
                                SH.HEAD_ID,
                                SH.HEAD,
                                SFM.MAIN_HEAD,
                                FS.AMOUNT,
                                FMH.BANK_ACCOUNT_ID,
                                FMH.FEE_CATEGORY_ID,
                                FC.FEE_CATEGORY AS FEE_CATEGORY_NAME,
                                CONCAT(BA.PASSBOOK_NO, ' - ', B.BANK_NAME) AS PASSBOOK_NO
                            FROM
                                FEE_MAIN_HEADS AS FMH
                                    LEFT JOIN
                                FEE_STRUCTURE_FOR_HOSTEL_AND_MESS AS FS ON FS.FEE_MAIN_HEAD_ID = FMH.FEE_MAIN_HEAD_ID
                                    AND FS.FREQUENCY = FMH.FREQUENCY_ID
                                    AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                    AND FS.IS_DELETED != 1
                                    INNER JOIN
                                SUP_HEAD AS SH ON SH.HEAD_ID = FMH.HEAD_ID 
                                    INNER JOIN
                                SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID 
                                    LEFT JOIN
                                SUP_FEE_CATEGORY AS FC ON FC.FEE_CATEGORY_ID = FMH.FEE_CATEGORY_ID
                                    LEFT JOIN
                                SUP_BANK_ACCOUNT AS BA ON BA.BANK_ACCOUNT_ID = FMH.BANK_ACCOUNT_ID
                                    LEFT JOIN
                                SUP_BANK AS B ON B.BANK_ID = BA.BANK
                            WHERE
                                FMH.ACADEMIC_YEAR =?AC_YEAR
                                    AND FMH.FREQUENCY_ID =?FREQUENCY_ID
                                    AND FMH.MAIN_HEAD_ID IN(?MAIN_HEAD_ID)
                                    AND FMH.IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.FetchStudentListByClassIdForFeeStudentAccount:
                    {
                        query = @"SELECT 
                                        SC.STUDENT_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        ROLL_NO,
                                        REGISTER_NO,
                                        SC.PROGRAMME_GROUP_ID,
                                        SC.CLASS_ID,
                                        CLS.CLASS_CODE,
                                        DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        SS.SHIFT_NAME,
                                        SPM.PROGRAMME_MODE
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
		                                    INNER JOIN
                                            SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID=CLS.PROGRAMME_MODE
                                            INNER JOIN SUP_SHIFT AS SS ON SS.SHIFT_ID=CLS.SHIFT
                                    WHERE
                                         CLS.PROGRAMME_GROUP_ID=?PROGRAMME AND CLS.CLASS_YEAR=?CLASS_YEAR_ID ;";
                        break;
                    }
                case FeeSQLCommands.FetchHostelStudentsByAcademicYear:
                    {
                        query = @"SELECT DISTINCT
                                    SC.STUDENT_ID,
                                    UPPER(CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                    ' ',
                                                    IFNULL(SP.LAST_NAME, ''))) AS FIRST_NAME,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    CLS.CLASS_NAME,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                    SS.SHIFT_NAME,
                                    SPM.PROGRAMME_MODE,
                                    B.BLOCK_NAME,
                                    H.HOSTEL_NAME,
                                    CAST(IFNULL((
                                        SELECT SUM(IFNULL(FSA.CREDIT, 0) - IFNULL(FSA.DEBIT, 0)) 
                                        FROM fee_student_account AS FSA 
                                        INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FSA.FREQUENCY_ID
                                        INNER JOIN SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FEE_ROOT_ID = '2'
                                        WHERE FSA.STUDENT_ID = STU.STUDENT_ID 
                                          AND FSA.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                          AND FSA.IS_DELETED != 1
                                    ), 0) AS DECIMAL(10,2)) AS BALANCE_AMOUNT
                                FROM
                                    (
                                        SELECT STUDENT_ID 
                                        FROM HOSTEL_STUDENT_INFO 
                                        WHERE ACADEMIC_YEAR = ?ACADEMIC_YEAR AND IS_DELETED != 1 AND IS_LEFT != 1
                                        
                                        UNION
                                        
                                        SELECT FSA.STUDENT_ID 
                                        FROM fee_student_account AS FSA
                                        INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FSA.FREQUENCY_ID
                                        INNER JOIN SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FEE_ROOT_ID = '2'
                                        WHERE FSA.ACADEMIC_YEAR = ?ACADEMIC_YEAR AND FSA.IS_DELETED != 1
                                    ) AS STU
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = STU.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND SC.IS_DELETED != 1
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = CLS.PROGRAMME_MODE
                                        INNER JOIN
                                    SUP_SHIFT AS SS ON SS.SHIFT_ID = CLS.SHIFT
                                        LEFT JOIN
                                    HOSTEL_STUDENT_INFO AS HS ON HS.STUDENT_ID = STU.STUDENT_ID
                                        AND HS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND HS.IS_DELETED != 1
                                        AND HS.IS_LEFT != 1
                                        LEFT JOIN
                                    SUP_HOSTEL AS H ON H.HOSTEL_ID = HS.HOSTEL_ID
                                        LEFT JOIN
                                    SUP_BLOCK AS B ON B.BLOCK_ID = HS.BLOCK_ID
                                ORDER BY CLS.CLASS_YEAR, SP.ROLL_NO;";
                        break;
                    }
                case FeeSQLCommands.FetchDaywiseFeeReport:
                    {
                        query = @"SELECT 
                                    T.TRANSACTION_ID,
                                    T.STUDENT_ID,
                                    SP.ADMISSION_NO,
                                    SP.FIRST_NAME,
                                    T.FREQUENCY,
                                    DATE_FORMAT(T.PAYMENT_DATE,'%d/%m/%Y') AS 'PAYMENT_DATE',
                                    T.PAYURESPONSE_ID,
                                    PM.PAYMENT_MODE,
                                    T.COLLECTED
                                FROM
                                    FEE_TRANSACTION AS T
		                                INNER JOIN
	                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=T.STUDENT_ID AND SP.IS_DELETED!=1
		                                INNER JOIN
	                                SUP_PAYMENT_MODE AS PM ON PM.PAYMENT_MODE_ID=T.PAYMENT_MODE
                                WHERE
                                    T.PAYMENT_DATE = ?PAYMENT_DATE AND T.FREQUENCY=?FREQUENCY;";
                        break;
                    }
                case FeeSQLCommands.FetchDaywiseFeeReportByTransactionId:
                    {
                        query = @"SELECT 
                                    T.TRANSACTION_ID,
                                    T.STUDENT_ID,
                                    T.FREQUENCY,
                                    T.COLLECTED,
                                    DATE_FORMAT(T.PAYMENT_DATE,'%d/%m/%Y') AS 'PAYMENT_DATE',
                                    H.HEAD_ID,
                                    H.HEAD,
                                    C.PAID_AMOUNT
                                FROM
                                    FEE_TRANSACTION AS T
                                        INNER JOIN
                                    FEE_COLLECTION AS C ON C.TRANSACTION_ID = T.TRANSACTION_ID
                                        AND C.IS_DELETED != 1
		                                INNER JOIN
	                                FEE_MAIN_HEADS AS MH ON MH.FEE_MAIN_HEAD_ID=C.HEAD AND MH.IS_DELETED!=1
		                                INNER JOIN
	                                SUP_HEAD AS H ON H.HEAD_ID=MH.HEAD_ID AND H.IS_DELETED!=1
                                 WHERE T.IS_DELETED!=1 AND T.TRANSACTION_ID=?TRANSACTION_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchDayWiseFeeCollection:
                    {
                        query = @"SELECT 
                                    SFF.FREQUENCY_NAME,
                                    SFT.FREQUENCY_TYPE,
                                    MONTHNAME(FT.PAYMENT_DATE) AS MONTH,
                                    DATE_FORMAT(PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,
                                    SUM(FT.COLLECTED) AS COLLECTED
                                FROM
                                    FEE_TRANSACTION AS FT
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FT.FREQUENCY
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_FREQUENCY_TYPE AS SFT ON FFF.FEE_MODE = SFT.FREQUENCY_TYPE_ID
                                        INNER JOIN
                                    SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = SFF.SEMESTER_TYPE
                                        AND SST.IS_ACTIVE = 1
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = FT.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                WHERE
                                    PAYMENT_DATE BETWEEN ?DATE_FROM AND ?DATE_TO
                                GROUP BY PAYMENT_DATE;";
                        break;
                    }
                case FeeSQLCommands.FetchFeePaymentDetailsByDate:
                    {
                        query = @"SELECT 
                                SFF.FREQUENCY_NAME,
                                SFT.FREQUENCY_TYPE,
                                MONTHNAME(FT.PAYMENT_DATE) AS MONTH,
                                DATE_FORMAT(PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,
                                COLLECTED,
                                SPM.PAYMENT_MODE,
                                SP.ROLL_NO,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME
                            FROM
                                FEE_TRANSACTION AS FT
                                    INNER JOIN
                                FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FT.FREQUENCY
                                    INNER JOIN
                                SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                    LEFT JOIN
                                SUP_FREQUENCY_TYPE AS SFT ON FFF.FEE_MODE = SFT.FREQUENCY_TYPE_ID
                                    INNER JOIN
                                SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = SFF.SEMESTER_TYPE       
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = FT.STUDENT_ID
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    INNER JOIN
                                SUP_PAYMENT_MODE AS SPM ON SPM.PAYMENT_MODE_ID = FT.PAYMENT_MODE
                            WHERE
                                PAYMENT_DATE = ?PAYMENT_DATE;";
                        break;
                    }
                case FeeSQLCommands.FetchMonthwiseFeeTransaction:
                    {
                        query = @"SELECT 
                                    C.DAY_ORDER_MONTH,
                                    DATE_FORMAT(C.DAY_ORDER_DATE,'%Y-%m-%d') AS 'DAY_ORDER_DATE',
                                    SUM(COLLECTED) AS 'COLLECTED'
                                FROM
                                    CALENDER AS C
                                INNER JOIN FEE_TRANSACTION AS T ON T.PAYMENT_DATE = C.DAY_ORDER_DATE
                                GROUP BY C.DAY_ORDER_MONTH
                                ORDER BY C.DAY_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchDaywiseFeeTransaction:
                    {
                        query = @"SELECT 
                                SPT.FREQUENCY_TYPE, SUM(COLLECTED) AS 'COLLECTED'
                            FROM
                                FEE_TRANSACTION AS T
                                    INNER JOIN
                                FEE_FREQUENCY_FEE_MODE AS FF ON FF.FEE_FREQUENCY_FEE_MODE_ID = T.FREQUENCY
                                    LEFT JOIN
                                SUP_FREQUENCY_TYPE AS SPT ON SPT.FREQUENCY_TYPE_ID = FF.FEE_MODE
                            WHERE
                                T.PAYMENT_DATE = DATE_FORMAT(CURDATE(), '%Y-%m-%d')
                            GROUP BY SPT.FREQUENCY_TYPE_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchTotalFeeTransaction:
                    {
                        query = @"SELECT 
                                    FREQUENCY_TYPE,
                                     SUM(COLLECTED) AS 'COLLECTED'
                                FROM
                                    SUP_FREQUENCY_TYPE AS F
		                                INNER JOIN
	                                FEE_TRANSACTION AS T ON T.FREQUENCY=F.FREQUENCY_TYPE_ID
                                GROUP BY F.FREQUENCY_TYPE_ID ORDER BY F.FREQUENCY_TYPE_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchDatewiseFeeReport:
                    {
                        query = @"SELECT 
                                    UPPER(CONCAT( IFNULL(S.FIRST_NAME,''),' ',IFNULL(S.LAST_NAME ,''))) AS NAME ,
                                    UPPER(S.ADMISSION_NO) AS ADMISSION_NO,
                                    UPPER(S.ROLL_NO) AS ROLL_NO,
                                    UPPER(T.RECEIPT_NO) AS RECEIPT_NO,
                                    T.COLLECTED,
                                    UPPER(CLS.CLASS_NAME) AS CLASS_NAME,
                                    DATE_FORMAT(T.PAYMENT_DATE,'%d/%m/%Y') AS TRANSACTION_DATE,
                                    UPPER(PM.PAYMENT_MODE) AS PAYMENT_MODE
                                    FROM
                                        FEE_TRANSACTION AS T
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS S ON S.STUDENT_ID = T.STUDENT_ID AND S.IS_DELETED!=1 AND S.IS_LEFT!=1
                                            INNER JOIN
                                        STU_CLASS AS C ON C.STUDENT_ID = S.STUDENT_ID
                                            AND C.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = C.CLASS_ID
		                                    INNER JOIN
	                                    SUP_PAYMENT_MODE AS PM ON PM.PAYMENT_MODE_ID=T.PAYMENT_MODE
                                    WHERE  T.PAYMENT_DATE BETWEEN  ?DATE_FROM AND ?DATE_TO AND FREQUENCY = ?FREQUENCY ORDER BY T.PAYMENT_DATE ;";
                        break;
                    }
                case FeeSQLCommands.FetchHeadwiseFeeReport:
                    {
                        query = @"SELECT 
                                        FT.TRANSACTION_ID,
                                        FT.STUDENT_ID,
                                        UPPER(CONCAT(IFNULL(SP.FIRST_NAME, ' '),
                                                        ' ',
                                                        IFNULL(SP.LAST_NAME, ' '))) AS `FIRST_NAME`,
                                        SP.ROLL_NO,
                                        DATE_FORMAT(FT.PAYMENT_DATE, '%Y/%m/%d') AS PAYMENT_DATE,
                                        FC.HEAD,
                                        CONCAT(CP.PROGRAMME_NAME,
                                                ' ',
                                                SH.DESCRIPTION,
                                                '-',
                                                SPM.PROGRAMME_MODE) AS PROGRAMME_NAME,
                                        FC.PAID_AMOUNT
                                    FROM
                                        FEE_TRANSACTION AS FT
                                            INNER JOIN
                                        FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = FT.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        FEE_STRUCTURE AS FS ON FS.HEAD = FC.HEAD
                                            AND FS.PROGRAMME = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                                            INNER JOIN
                                        SUP_SHIFT AS SH ON SH.SHIFT_ID = CPG.SHIFT
                                            INNER JOIN
                                        SUP_BANK_ACCOUNT AS SB ON SB.BANK_ACCOUNT_ID = FS.BANK_ACCOUNT_ID
                                    WHERE
                                        PAYMENT_DATE BETWEEN ?DATE_FROM AND ?DATE_TO
                                            AND FT.FREQUENCY = ?FREQUENCY
                                            AND CPG.PROGRAMME_MODE = ?PROGRAMME_MODE
                                            AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CLS.CLASS_LEVEL=?CLASS_LEVEL
                                            AND CLS.CLASS_YEAR=?CLASS_YEAR
                                            AND FC.HEAD IN (?HEAD)
                                    GROUP BY FT.STUDENT_ID , FC.HEAD
                                    ORDER BY FT.TRANSACTION_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchMainHeadwiseFeeReport:
                    {
                        query = @"SELECT 
                                        SC.PROGRAMME_GROUP_ID,
                                        FMH.MAIN_HEAD_ID,
                                        FMH.HEAD_ID,
                                        SUM(FC.PAID_AMOUNT) AS PAID_AMOUNT,
                                        DATE_FORMAT(FT.PAYMENT_DATE,'%Y/%m/%d') AS PAYMENT_DATE
                                    FROM
                                        FEE_TRANSACTION AS FT
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = FT.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        FEE_MAIN_HEADS AS FMH ON FMH.FREQUENCY_ID = FT.FREQUENCY
                                            AND FMH.PROGRAMME_MODE = CPG.PROGRAMME_MODE
                                            AND FMH.SHIFT = CPG.SHIFT
                                            AND FMH.HEAD_ID = FC.HEAD                                          
                                    WHERE
                                        PAYMENT_DATE BETWEEN ?DATE_FROM AND ?DATE_TO
                                            AND FT.FREQUENCY = ?FREQUENCY AND CLS.CLASS_LEVEL = ?CLASS_LEVEL
                                            AND CLS.CLASS_YEAR = ?CLASS_YEAR
                                            AND CLS.SHIFT = ?SHIFT
                                            AND CLS.PROGRAMME_MODE = ?PROGRAMME_MODE
                                            AND FMH.MAIN_HEAD_ID IN (?MAIN_HEAD_ID)
                                    GROUP BY FT.PAYMENT_DATE , SC.PROGRAMME_GROUP_ID , FMH.MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchMainHeadwiseFeeReportByDate:
                    {
                        query = @"SELECT 
                                        SC.PROGRAMME_GROUP_ID,
                                        FMH.MAIN_HEAD_ID,
                                        FMH.HEAD_ID,
                                        SUM(FC.PAID_AMOUNT) AS PAID_AMOUNT,
                                        DATE_FORMAT(FT.PAYMENT_DATE,'%Y/%m/%d') AS PAYMENT_DATE
                                    FROM
                                        FEE_TRANSACTION AS FT
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = FT.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        FEE_MAIN_HEADS AS FMH ON FMH.FREQUENCY_ID = FT.FREQUENCY
                                            AND FMH.PROGRAMME_MODE = CPG.PROGRAMME_MODE
                                            AND FMH.SHIFT = CPG.SHIFT
                                            AND FMH.HEAD_ID = FC.HEAD
                                    WHERE
                                        PAYMENT_DATE BETWEEN ?DATE_FROM AND ?DATE_TO
                                            AND FT.FREQUENCY = ?FREQUENCY AND CLS.CLASS_LEVEL = ?CLASS_LEVEL
                                            AND CLS.CLASS_YEAR = ?CLASS_YEAR
                                            AND CLS.SHIFT = ?SHIFT
                                            AND CLS.PROGRAMME_MODE = ?PROGRAMME_MODE
                                            AND FMH.MAIN_HEAD_ID IN (?MAIN_HEAD_ID)
                                    GROUP BY SC.PROGRAMME_GROUP_ID , FMH.MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchMessFeeReport:
                    {
                        query = @"SELECT 
                                    FA.STUDENT_ID,
                                    IF(SUM((IFNULL(FA.CREDIT, 0) - IFNULL(FA.DEBIT, 0))) = 0,
                                        'PAID',
                                        'NOT PAID') AS STATUS,
                                    SUM((IFNULL(FA.CREDIT, 0) - IFNULL(FA.DEBIT, 0))) AS BALANCE,
                                    SUM((IFNULL(FA.CREDIT, 0))) AS CREDIT,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    SP.STU_EMAILID,
                                    SP.STU_MOBILENO,
                                    MH.MAIN_HEAD_ID,
                                    FMH.FEE_MAIN_HEAD_ID,
                                    MH.MAIN_HEAD,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    CL.CLASS_CODE,
                                    FMH.HEAD_ID,
                                    H.HOSTEL_NAME,
                                    SP.IS_DELETED,
                                    SP.IS_LEFT
                                FROM
                                    FEE_STUDENT_ACCOUNT AS FA
                                        LEFT JOIN
                                    FEE_MAIN_HEADS AS FMH ON FMH.FEE_MAIN_HEAD_ID = FA.FEE_MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS MH ON MH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FMH.HEAD_ID
                                        LEFT JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = FA.STUDENT_ID
                                      --  AND SP.IS_DELETED != 1
                                      --  AND SP.IS_LEFT != 1
                                        LEFT JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                        AND SC.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        LEFT JOIN
                                    CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                        LEFT JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND PG.IS_DELETED != 1
                                        LEFT JOIN
                                    CP_PROGRAMME AS CP ON PG.PROGRAMME_ID = CP.PROGRAMME_ID
                                        AND CP.IS_DELETED != 1
                                        LEFT JOIN
                                    HOSTEL_STUDENT_INFO AS HS ON HS.STUDENT_ID = SP.STUDENT_ID
                                        AND HS.IS_DELETED != 1
                                        AND HS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        LEFT JOIN
                                    SUP_HOSTEL AS H ON H.HOSTEL_ID = HS.HOSTEL_ID
                                        AND H.IS_DELETED != 1
                                WHERE
                                    FA.FREQUENCY_ID = ?FREQUENCY_ID
                                        AND FA.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND MH.MAIN_HEAD_ID IN (?MAIN_HEAD_ID)
                                GROUP BY FA.STUDENT_ID , FA.HEAD;";
                        break;
                    }
                case FeeSQLCommands.FetchMainHeadwiseFeeReportByFeeRootId:
                    {
                        query = @"CALL TEST();";
                        //               query = @"SELECT 
                        //                           FMH.FREQUENCY_ID AS FREQUENCY,
                        //                           SP.STUDENT_ID,
                        //                           SUM((IFNULL(FC.PAID_AMOUNT, 0))) AS BALANCE,
                        //                           FF.FREQUENCY_NAME,
                        //                           CONCAT(IFNULL(SP.FIRST_NAME, ''),
                        //                                   ' ',
                        //                                   IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                        //                           H.HEAD AS HEAD,
                        //                           SP.ROLL_NO,
                        //                           SP.REGISTER_NO,
                        //                           SFM.MAIN_HEAD_ID,
                        //                           FMH.HEAD_ID,
                        //                           PG.PROGRAMME_GROUP_ID,
                        //                           DATE_FORMAT(F.PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,F.RECEIPT_NO
                        //                       FROM
                        //                           FEE_TRANSACTION AS F 
                        //                               INNER JOIN
                        //                           FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = F.TRANSACTION_ID
                        //                               INNER JOIN
                        //                           STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = F.STUDENT_ID
                        //                               INNER JOIN
                        //STU_CLASS AS SC ON SC.STUDENT_ID=SP.STUDENT_ID
                        //	AND SC.ACADEMIC_YEAR=?ACADEMIC_YEAR
                        //                               AND SC.IS_DELETED!=1
                        //                               INNER JOIN
                        //CP_CLASSES AS CLS ON CLS.CLASS_ID=SC.CLASS_ID
                        //                               INNER JOIN
                        //                           CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                        //                               AND PG.IS_DELETED != 1
                        //                               INNER JOIN
                        //                           FEE_MAIN_HEADS AS FMH ON FMH.APPLICATION_TYPE_ID = PG.APPTYPE_ID
                        //                               AND FMH.PROGRAMME_MODE = PG.PROGRAMME_MODE
                        //                               AND FMH.SHIFT = PG.SHIFT
                        //                               AND FC.HEAD = FMH.HEAD_ID
                        //                               AND FMH.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                        //                               AND FMH.IS_DELETED != 1
                        //                               INNER JOIN
                        //                           FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                        //                               AND FFF.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                        //                               AND FFF.FEE_FREQUENCY_FEE_MODE_ID = F.FREQUENCY
                        //                               INNER JOIN
                        //                           SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                        //                               AND FT.FEE_ROOT_ID = ?FEE_ROOT_ID
                        //                               INNER JOIN
                        //                           SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                        //                               LEFT JOIN
                        //                           SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                        //                               LEFT JOIN
                        //                           SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                        //                       WHERE
                        //                           PG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID) AND F.FREQUENCY=?FREQUENCY
                        //                           AND CLS.CLASS_YEAR=?CLASS_YEAR
                        //                           AND F.PAYMENT_DATE >= ?DATE_FROM
                        //                           AND F.PAYMENT_DATE <= ?DATE_TO
                        //                          -- AND F.PAYMENT_DATE BETWEEN ?DATE_FROM AND ?DATE_TO
                        //                       GROUP BY SP.STUDENT_ID , F.FREQUENCY , FMH.MAIN_HEAD_ID,FMH.HEAD_ID ORDER BY FC.HEAD, F.PAYMENT_DATE; ";
                        break;
                    }
                case FeeSQLCommands.FetchMainHeadwiseFeeReportByFeeRootIdForDateWiseReport:
                    {
                        query = @"SELECT 
                                    FT.STUDENT_ID,
                                    FC.PAID_AMOUNT AS BALANCE,
                                    FM.HEAD_ID,
                                    FM.MAIN_HEAD_ID,                                    
                                    PG.PROGRAMME_GROUP_ID,
                                    DATE_FORMAT(FT.PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,
                                    FROM_UNIXTIME(FC.SETTLEMENT_DATE, '%d/%m/%Y') AS SETTLEMENT_DATE
                                FROM
                                    FEE_TRANSACTION AS FT
                                        INNER JOIN
                                    FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FM ON FM.FEE_MAIN_HEAD_ID = FC.FEE_MAIN_HEAD_ID AND FM.FEE_CATEGORY_ID IN (?FEE_CATEGORY_ID)
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = FT.STUDENT_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                WHERE
                                    FT.FREQUENCY = ?FREQUENCY
                                        AND FC.SETTLEMENT_DATE BETWEEN UNIX_TIMESTAMP(?DATE_FROM) AND UNIX_TIMESTAMP(ADDDATE(?DATE_TO, 1))
                                        AND FT.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND PG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                                ORDER BY FC.SETTLEMENT_DATE;";
                        break;
                    }
                case FeeSQLCommands.FetchMainHeadwiseFeeReportByFeeRootIdForDateWiseReportByDate:
                    {
                        query = @"SELECT 
                                    FMH.FREQUENCY_ID AS FREQUENCY,
                                    SP.STUDENT_ID,
                                    SUM((IFNULL(FC.PAID_AMOUNT, 0))) AS BALANCE,
                                    FF.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    H.HEAD AS HEAD,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    SFM.MAIN_HEAD_ID,
                                    P.PROGRAMME_NAME,
                                    FMH.HEAD_ID,
                                    PG.PROGRAMME_GROUP_ID,
                                    DATE_FORMAT(F.PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,F.RECEIPT_NO,
 FROM_UNIXTIME(S.created_at, '%d/%m/%Y') AS SETTLEMENT_DATE
                                FROM
                                    FEE_TRANSACTION AS F 
                                        INNER JOIN
                                    FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = F.TRANSACTION_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = F.STUDENT_ID
                                        INNER JOIN
									STU_CLASS AS SC ON SC.STUDENT_ID=SP.STUDENT_ID
										AND SC.ACADEMIC_YEAR=?ACADEMIC_YEAR
                                        AND SC.IS_DELETED!=1
                                        INNER JOIN
									CP_CLASSES AS CLS ON CLS.CLASS_ID=SC.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                        AND PG.IS_DELETED != 1
                                    INNER JOIN
                                    CP_PROGRAMME AS P ON P.PROGRAMME_ID=PG.PROGRAMME_ID
                                        INNER JOIN
                                     FEE_MAIN_HEADS AS FMH ON FMH.FEE_MAIN_HEAD_ID=FC.FEE_MAIN_HEAD_ID
                                        AND FMH.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = F.FREQUENCY
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        AND FT.FEE_ROOT_ID = ?FEE_ROOT_ID
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                                  INNER JOIN
                                        FEE_RAZORPAY_TRANSFER_INFO_?AC_YEAR AS T ON T.UDF2 = F.RAZORPAY_ID
                                            AND T.UDF1 = F.STUDENT_ID
                                            AND T.UDF3 = FMH.MAIN_HEAD_ID
                                            INNER JOIN
                                        FEE_RAZORPAY_SETTLEMENT_INFO_?AC_YEAR AS S ON S.ID = T.RECIPIENT_SETTLEMENT_ID
                                WHERE
                                    PG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID) AND F.FREQUENCY=?FREQUENCY                                      
                                    AND F.PAYMENT_DATE =?DATE_FROM 
                                GROUP BY SP.STUDENT_ID , F.FREQUENCY , FMH.MAIN_HEAD_ID,FMH.HEAD_ID ORDER BY FC.HEAD, F.PAYMENT_DATE; ";
                        break;
                    }
                case FeeSQLCommands.FetchMainHeadwiseFeeReportByFeeRootIdAndFeeCategory:
                    {
                        query = @"SELECT 
                                FT.STUDENT_ID,
                                FC.PAID_AMOUNT AS BALANCE,
                                FM.HEAD_ID,
                                FM.MAIN_HEAD_ID,
                                PG.PROGRAMME_GROUP_ID,
                                DATE_FORMAT(FT.PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,
                                FROM_UNIXTIME(FC.SETTLEMENT_DATE, '%d/%m/%Y') AS SETTLEMENT_DATE
                            FROM
                                FEE_TRANSACTION AS FT
                                    INNER JOIN
                                FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID
                                    INNER JOIN
                                FEE_MAIN_HEADS AS FM ON FM.FEE_MAIN_HEAD_ID = FC.FEE_MAIN_HEAD_ID
                                    AND FEE_CATEGORY_ID IN (?FEE_CATEGORY_ID)
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = FT.STUDENT_ID
                                    AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    INNER JOIN
                                CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                    AND CLASS_YEAR = ?CLASS_YEAR
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                            WHERE
                                FT.FREQUENCY = ?FREQUENCY                                   
                                    AND FT.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    AND PG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                            ORDER BY FC.SETTLEMENT_DATE;";
                        break;
                    }
                case FeeSQLCommands.FetchMainHeadsByPRogrammeGroupIdAndFrequencyId:
                    {
                        query = @"SELECT 
                                        PROGRAMME_GROUP_ID, 
                                        MAIN_HEAD_ID, 
                                        MAIN_HEAD, 
                                        MAX(HEAD_COUNT) AS HEAD_COUNT
                                    FROM
                                        (SELECT 
                                            PG.PROGRAMME_GROUP_ID,
                                                MH.MAIN_HEAD_ID,
                                                MH.MAIN_HEAD,
                                                COUNT(FMH.HEAD_ID) AS HEAD_COUNT
                                        FROM
                                            FEE_MAIN_HEADS AS FMH
		                                    INNER JOIN 
                                        SUP_FEE_MAIN_HEAD AS MH ON MH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
		                                    INNER JOIN 
                                        SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
		                                    INNER JOIN 
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_MODE = FMH.PROGRAMME_MODE
                                            AND PG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                                            AND PG.SHIFT = FMH.SHIFT
                                            AND PG.APPTYPE_ID = FMH.APPLICATION_TYPE_ID
                                            AND PG.IS_DELETED != 1
                                        WHERE
                                            FEE_ROOT_ID = ?FEE_ROOT_ID AND FREQUENCY_ID = ?FREQUENCY
                                                AND FMH.IS_DELETED != 1
                                        GROUP BY PG.PROGRAMME_GROUP_ID , MH.MAIN_HEAD_ID) AS T group by T.MAIN_HEAD_ID;";
                        //query = @"SELECT 
                        //             MH.MAIN_HEAD_ID,
                        //                MH.MAIN_HEAD,
                        //                PG.PROGRAMME_GROUP_ID,
                        //                COUNT(FMH.HEAD_ID) AS HEAD_COUNT
                        //            FROM
                        //                FEE_MAIN_HEADS AS FMH
                        //                    INNER JOIN
                        //                SUP_FEE_MAIN_HEAD AS MH ON MH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                        //                    INNER JOIN
                        //                SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                        //              INNER JOIN
                        //             CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_MODE=FMH.PROGRAMME_MODE
                        //              AND PG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                        //                    AND PG.SHIFT=FMH.SHIFT
                        //                    AND PG.APPTYPE_ID=FMH.APPLICATION_TYPE_ID
                        //                    AND PG.IS_DELETED!=1
                        //            WHERE
                        //                FEE_ROOT_ID = ?FEE_ROOT_ID 
                        //                AND FREQUENCY_ID = ?FREQUENCY_TYPE 
                        //                AND FMH.IS_DELETED!=1
                        //                GROUP BY PG.PROGRAMME_GROUP_ID,MH.MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchMainHeadsByPRogrammeGroupIdAndFrequencyIdAndFeeCategory:
                    {
                        query = @"SELECT 
                                PROGRAMME_GROUP_ID,
                                MAIN_HEAD_ID,
                                MAIN_HEAD,
                                MAX(HEAD_COUNT) AS HEAD_COUNT,
                                FEE_CATEGORY_ID
                            FROM
                                (SELECT 
                                    PG.PROGRAMME_GROUP_ID,
                                        MH.MAIN_HEAD_ID,
                                        MH.MAIN_HEAD,
                                        COUNT(FMH.HEAD_ID) AS HEAD_COUNT,
                                        FEE_CATEGORY_ID
                                FROM
                                    FEE_MAIN_HEADS AS FMH
                                INNER JOIN SUP_FEE_MAIN_HEAD AS MH ON MH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                INNER JOIN SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                INNER JOIN CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_MODE = FMH.PROGRAMME_MODE
                                    AND PG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                                    AND PG.SHIFT = FMH.SHIFT
                                    AND PG.APPTYPE_ID = FMH.APPLICATION_TYPE_ID	
                                    AND PG.IS_DELETED != 1
                                WHERE
                                    FEE_ROOT_ID = 1 AND FREQUENCY_ID = ?FREQUENCY  AND FEE_CATEGORY_ID IN (?FEE_CATEGORY_ID)
                                        AND FMH.IS_DELETED != 1
                                GROUP BY PG.PROGRAMME_GROUP_ID , MH.MAIN_HEAD_ID) AS T
                            GROUP BY T.MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchMappedSupHead:
                    {
                        query = @"SELECT 
                                   MAIN_HEAD_ID, HEAD_ID, SHIFT, PROGRAMME_MODE, FREQUENCY_ID,APPLICATION_TYPE_ID,FEE_ROOT_ID
                                FROM
                                    FEE_MAIN_HEADS AS FH
                                WHERE
                                    IS_DELETED != 1 AND FH.SHIFT = ?SHIFT
                                        AND FH.PROGRAMME_MODE = ?PROGRAMME_MODE
                                        AND FH.FREQUENCY_ID = ?FREQUENCY_ID
                                        AND FH.APPLICATION_TYPE_ID = ?APPLICATION_TYPE_ID
                                        AND FH.FEE_ROOT_ID=?FEE_ROOT_ID AND FH.MAIN_HEAD_ID !=?MAIN_HEAD_ID AND FH.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case FeeSQLCommands.FetchMappedSupHeadForHostel:
                    {
                        query = @"SELECT 
                                   MAIN_HEAD_ID, HEAD_ID, SHIFT, PROGRAMME_MODE, FREQUENCY_ID,APPLICATION_TYPE_ID,FEE_ROOT_ID
                                FROM
                                    FEE_MAIN_HEADS AS FH
                                WHERE
                                    IS_DELETED != 1                                        
                                        AND FH.FREQUENCY_ID = ?FREQUENCY_ID                                        
                                        AND FH.FEE_ROOT_ID=?FEE_ROOT_ID AND FH.MAIN_HEAD_ID !=?MAIN_HEAD_ID AND FH.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case FeeSQLCommands.BindFeeMainHeadBySubHead:
                    {
                        query = @"SELECT 
                                FEE_MAIN_HEAD_ID,
                                FH.MAIN_HEAD_ID,
                                FR.FEE_ROOT,
                                AP.APPLICATION_TYPE,
                                SH.HEAD_ID,
                                SFH.MAIN_HEAD,
                                SH.HEAD,
                                SFF.FREQUENCY_NAME,
                                SFT.FREQUENCY_TYPE,
                                SF.SHIFT_NAME,
                                SPM.PROGRAMME_MODE,
                                FH.ACADEMIC_YEAR,
                                B.BANK_ACCOUNT_ID,
                                FCAT.FEE_CATEGORY_ID
                            FROM
                                SUP_HEAD AS SH
                                    LEFT JOIN
                                FEE_MAIN_HEADS AS FH ON FH.HEAD_ID = SH.HEAD_ID AND FH.ACADEMIC_YEAR = ?AC_YEAR
                                    AND FH.FREQUENCY_ID = ?FREQUENCY_ID
                                    AND FH.FEE_ROOT_ID = ?FEE_ROOT_ID
                                    AND FH.SHIFT = ?SHIFT
                                    AND FH.APPLICATION_TYPE_ID = ?APPLICATION_TYPE_ID
                                    AND FH.MAIN_HEAD_ID = ?MAIN_HEAD_ID
                                    AND FH.PROGRAMME_MODE = ?PROGRAMME_MODE
                                  AND  FH.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_FEE_MAIN_HEAD AS SFH ON SFH.MAIN_HEAD_ID = FH.MAIN_HEAD_ID
                                    AND SFH.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_SHIFT AS SF ON SF.SHIFT_ID = FH.SHIFT
                                    AND FH.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = FH.PROGRAMME_MODE
                                    AND SPM.IS_DELETED != 1
                                    LEFT JOIN
                                FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FH.FREQUENCY_ID
                                    LEFT JOIN
                                SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                    AND SFF.IS_DELETED != 1
                                    LEFT JOIN
                                FEE_ROOT AS FR ON FR.FEE_ROOT_ID = FH.FEE_ROOT_ID
                                    AND FR.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_APPLICATION_TYPE AS AP ON AP.APPLICATION_TYPE_ID = FH.APPLICATION_TYPE_ID
                                    AND AP.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_BANK_ACCOUNT AS B ON B.BANK_ACCOUNT_ID = FH.BANK_ACCOUNT_ID
                                    AND B.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                    AND SFT.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_FEE_CATEGORY AS FCAT ON FCAT.FEE_CATEGORY_ID = FH.FEE_CATEGORY_ID
                                    AND FCAT.IS_DELETED != 1
                            WHERE
                                SH.IS_DELETED != 1 
                                    AND SH.HEAD_ID IN (?HEAD_ID);";
                        break;
                    }
                case FeeSQLCommands.BindFeeMainHeadBySubHeadForHostel:
                    {
                        query = @"SELECT 
                                FEE_MAIN_HEAD_ID,
                                FH.MAIN_HEAD_ID,
                                FR.FEE_ROOT,
                                AP.APPLICATION_TYPE,
                                SH.HEAD_ID,
                                SFH.MAIN_HEAD,
                                SH.HEAD,
                                SFF.FREQUENCY_NAME,
                                SFT.FREQUENCY_TYPE,
                                SF.SHIFT_NAME,
                                SPM.PROGRAMME_MODE,
                                FH.ACADEMIC_YEAR,
                                B.BANK_ACCOUNT_ID,
                                FCAT.FEE_CATEGORY_ID
                            FROM
                                SUP_HEAD AS SH
                                    LEFT JOIN
                                FEE_MAIN_HEADS AS FH ON FH.HEAD_ID = SH.HEAD_ID AND FH.ACADEMIC_YEAR = ?AC_YEAR
                                    AND FH.FREQUENCY_ID = ?FREQUENCY_ID
                                    AND FH.FEE_ROOT_ID = ?FEE_ROOT_ID                                    
                                    AND FH.MAIN_HEAD_ID = ?MAIN_HEAD_ID                                    
                                  AND  FH.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_FEE_MAIN_HEAD AS SFH ON SFH.MAIN_HEAD_ID = FH.MAIN_HEAD_ID
                                    AND SFH.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_SHIFT AS SF ON SF.SHIFT_ID = FH.SHIFT
                                    AND FH.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = FH.PROGRAMME_MODE
                                    AND SPM.IS_DELETED != 1
                                    LEFT JOIN
                                FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FH.FREQUENCY_ID
                                    LEFT JOIN
                                SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                    AND SFF.IS_DELETED != 1
                                    LEFT JOIN
                                FEE_ROOT AS FR ON FR.FEE_ROOT_ID = FH.FEE_ROOT_ID
                                    AND FR.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_APPLICATION_TYPE AS AP ON AP.APPLICATION_TYPE_ID = FH.APPLICATION_TYPE_ID
                                    AND AP.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_BANK_ACCOUNT AS B ON B.BANK_ACCOUNT_ID = FH.BANK_ACCOUNT_ID
                                    AND B.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                    AND SFT.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_FEE_CATEGORY AS FCAT ON FCAT.FEE_CATEGORY_ID = FH.FEE_CATEGORY_ID
                                    AND FCAT.IS_DELETED != 1
                            WHERE
                                SH.IS_DELETED != 1
                                    AND SH.HEAD_ID IN (?HEAD_ID);";
                        break;
                    }
                case FeeSQLCommands.CheckIsSubHeadMapped:
                    {
                        query = @"SELECT 
                                COUNT(STUDENT_ID)>0 AS `STATUS`
                            FROM
                                FEE_STUDENT_ACCOUNT AS FA
                            WHERE
                                FA.FREQUENCY_ID = ?FREQUENCY_ID
                                    AND FA.ACADEMIC_YEAR = ?AC_YEAR
                                    AND HEAD = ?HEAD_ID AND FA.IS_DELETED!=1;";
                        break;
                    }
                case FeeSQLCommands.FetchFeemainHeadbyId:
                    {
                        query = @"SELECT 
                            FEE_MAIN_HEAD_ID,
                            FREQUENCY_ID,
                            APPLICATION_TYPE_ID,
                            MAIN_HEAD_ID,
                            HEAD_ID,
                            PROGRAMME_MODE,
                            SHIFT,
                            FEE_CATEGORY_ID,
                            FEE_ROOT_ID,
                            BANK_ACCOUNT_ID
                        FROM
                            FEE_MAIN_HEADS
                        WHERE
                            FEE_MAIN_HEAD_ID = ?FEE_MAIN_HEAD_ID AND IS_DELETED!=1 AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeStructureReportByProgrammeWiseReport:
                    {
                        query = @"SELECT 
                                FS.FEE_STRUCTURE_ID,
                                MH.APPLICATION_TYPE_ID,
                                AP.APPLICATION_TYPE,
                                PM.PROGRAMME_MODE_ID,
                                PM.PROGRAMME_MODE,
                                MH.SHIFT,
                                SS.SHIFT_NAME,
                                MH.FREQUENCY_ID,
                                H.HEAD_ID,
                                H.HEAD,
                                MH.MAIN_HEAD_ID,
                                SMH.MAIN_HEAD,
                                FS.AMOUNT,
                                FS.BANK_ACCOUNT_ID,
                                MH.FEE_CATEGORY_ID,
                                FCAT.FEE_CATEGORY,
                                CONCAT(IFNULL(CY.CLASS_YEAR, ''),
                                        '- ',
                                        IFNULL(cp.PROGRAMME_NAME, '')) AS PROGRAMME_NAME,
                                FS.PROGRAMME,
                                FS.ACADEMIC_YEAR,
                                FS.CLASS_YEAR_ID
                            FROM
                                FEE_MAIN_HEADS AS MH
                                    LEFT JOIN
                                FEE_STRUCTURE AS FS ON MH.FEE_MAIN_HEAD_ID = FS.FEE_MAIN_HEAD_ID
                                    AND PROGRAMME IN (?PROGRAMME_GROUP_ID)
                                    AND FS.CLASS_YEAR_ID IN (?CLASS_YEAR_ID)
                                    AND FS.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_FEE_MAIN_HEAD AS SMH ON SMH.MAIN_HEAD_ID = MH.MAIN_HEAD_ID
                                    AND SMH.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_HEAD AS H ON H.HEAD_ID = MH.HEAD_ID
                                    AND H.IS_DELETED != 1
                                    LEFT JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = FS.PROGRAMME
                                    AND PG.IS_DELETED != 1
                                    LEFT JOIN
                                CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                                    AND CP.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_SHIFT AS SS ON SS.SHIFT_ID = PG.SHIFT
                                    AND SS.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_FEE_CATEGORY AS FCAT ON FCAT.FEE_CATEGORY_ID = MH.FEE_CATEGORY_ID
                                    AND FCAT.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = MH.PROGRAMME_MODE
                                    AND PM.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_APPLICATION_TYPE AS AP ON AP.APPLICATION_TYPE_ID = MH.APPLICATION_TYPE_ID
                                    AND AP.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_BANK_ACCOUNT AS BA ON BA.BANK_ACCOUNT_ID = MH.BANK_ACCOUNT_ID
                                    LEFT JOIN
                                SUP_BANK AS B ON B.BANK_ID = BA.BANK
                                    LEFT JOIN
                                sup_class_year AS CY ON CY.CLASS_YEAR_ID = FS.CLASS_YEAR_ID
                            WHERE
                                FS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    AND MH.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    AND MH.FREQUENCY_ID = ?FREQUENCY_ID
                                    AND MH.PROGRAMME_MODE = ?PROGRAMME_MODE
                                    AND MH.SHIFT = ?SHIFT
                                    AND MH.IS_DELETED != 1
                            ORDER BY MH.APPLICATION_TYPE_ID , PG.PROGRAMME_GROUP_ID , FS.CLASS_YEAR_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeMainHeadForFeeStructureByGroup:
                    {
                        query = @"SELECT 
                                    MH.MAIN_HEAD_ID, MH.MAIN_HEAD, SH.HEAD_ID, SH.HEAD
                                FROM
                                    FEE_MAIN_HEADS AS FMH
                                        LEFT JOIN
                                    FEE_STRUCTURE AS FS ON FS.FEE_MAIN_HEAD_ID = FMH.FEE_MAIN_HEAD_ID
                                        AND FS.IS_DELETED != 1
                                        AND FS.PROGRAMME IN (?PROGRAMME_GROUP_ID)
                                        AND FS.CLASS_YEAR_ID IN (?CLASS_YEAR_ID)  
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS MH ON MH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        AND MH.IS_DELETED != 2
                                        LEFT JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FMH.HEAD_ID
                                        AND SH.IS_DELETED != 1
                                WHERE
                                    FMH.FREQUENCY_ID = ?FREQUENCY_ID
                                        AND FMH.PROGRAMME_MODE = ?PROGRAMME_MODE
                                        AND FMH.SHIFT = ?SHIFT
                                        AND FMH.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND FS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND FMH.IS_DELETED != 1
                                GROUP BY MH.MAIN_HEAD_ID , SH.HEAD_ID
                                ORDER BY MH.MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchHosteFeePaidReport:
                    {
                        query = @"SELECT 
                                FA.STUDENT_ID,
                                IF(SUM((IFNULL(FA.CREDIT, 0) - IFNULL(FA.DEBIT, 0))) = 0,
                                    'PAID',
                                    'NOT PAID') AS STATUS,
                                SUM((IFNULL(FA.CREDIT, 0) - IFNULL(FA.DEBIT, 0))) AS BALANCE,
                                SUM((IFNULL(FA.CREDIT, 0))) AS CREDIT,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                SP.STU_EMAILID,
                                SP.STU_MOBILENO,
                                MH.MAIN_HEAD_ID,
                                FMH.FEE_MAIN_HEAD_ID,
                                MH.MAIN_HEAD,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                CL.CLASS_CODE,
                                FMH.HEAD_ID,
                                H.HOSTEL_NAME
                            FROM
                                FEE_STUDENT_ACCOUNT AS FA
                                    LEFT JOIN
                                FEE_MAIN_HEADS AS FMH ON FMH.FEE_MAIN_HEAD_ID = FA.FEE_MAIN_HEAD_ID
                                    LEFT JOIN
                                SUP_FEE_MAIN_HEAD AS MH ON MH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                    LEFT JOIN
                                SUP_HEAD AS SH ON SH.HEAD_ID = FMH.HEAD_ID
                                    LEFT JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = FA.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1
                                    LEFT JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                    AND SC.IS_DELETED != 1
                                    AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    LEFT JOIN
                                CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                    LEFT JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                    AND PG.IS_DELETED != 1
                                    LEFT JOIN
                                CP_PROGRAMME AS CP ON PG.PROGRAMME_ID = CP.PROGRAMME_ID
                                    AND CP.IS_DELETED != 1
                                    LEFT JOIN
                                HOSTEL_STUDENT_INFO AS HS ON HS.STUDENT_ID = SP.STUDENT_ID
                                    AND HS.IS_DELETED != 1
                                    AND HS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    LEFT JOIN
                                SUP_HOSTEL AS H ON H.HOSTEL_ID = HS.HOSTEL_ID
                                    AND H.IS_DELETED != 1
                            WHERE
                                FA.FREQUENCY_ID = ?FREQUENCY_ID
                                    AND FA.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                            GROUP BY FA.STUDENT_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchMessFeePaidDate:
                    {
                        query = @"SELECT 
                                   FT.STUDENT_ID,DATE_FORMAT(FT.PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,FMH.HEAD_ID,FMH.MAIN_HEAD_ID
                                FROM
                                    FEE_TRANSACTION AS FT
                                        LEFT JOIN
                                    FEE_STUDENT_ACCOUNT AS FS ON FS.TRANSACTION_ID = FT.TRANSACTION_ID 
                                        LEFT JOIN
                                    FEE_MAIN_HEADS AS FMH ON FMH.FEE_MAIN_HEAD_ID = FS.FEE_MAIN_HEAD_ID   
                                        AND FMH.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS MH ON MH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID 
                                        AND MH.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FMH.HEAD_ID
                                        AND SH.IS_DELETED != 1
                                WHERE
                                    FT.FREQUENCY = ?FREQUENCY_ID AND FMH.MAIN_HEAD_ID IN  (?MAIN_HEAD_ID) AND
                                         FS.ACADEMIC_YEAR = ?ACADEMIC_YEAR GROUP BY FS.STUDENT_ID,FMH.MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchHostelFeePaidDate:
                    {
                        query = @"SELECT 
                                   FT.STUDENT_ID,DATE_FORMAT(FT.PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE
                                FROM
                                    FEE_TRANSACTION AS FT
                                        LEFT JOIN
                                    FEE_STUDENT_ACCOUNT AS FS ON FS.TRANSACTION_ID = FT.TRANSACTION_ID 
                                        LEFT JOIN
                                    FEE_MAIN_HEADS AS FMH ON FMH.FEE_MAIN_HEAD_ID = FS.FEE_MAIN_HEAD_ID   
                                        AND FMH.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS MH ON MH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID 
                                        AND MH.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FMH.HEAD_ID
                                        AND SH.IS_DELETED != 1
                                WHERE
                                    FT.FREQUENCY = ?FREQUENCY_ID AND
                                         FS.ACADEMIC_YEAR = ?ACADEMIC_YEAR GROUP BY FS.STUDENT_ID;";
                        break;
                    }
                case FeeSQLCommands.UpdateFeeStudenAccountForHostelAndMess:
                    {
                        query = @"UPDATE FEE_STUDENT_ACCOUNT SET CREDIT=?CREDIT WHERE F_STUDENT_AC_ID=?F_STUDENT_AC_ID;";
                        break;
                    }

                case FeeSQLCommands.updatepaidstatusforaarearcourse:
                    {
                        query = @"SET SQL_SAFE_UPDATES = 0;
                                  UPDATE EXAM_REGISTRATION_ARREAR SET IS_PAID=1 WHERE STUDENT_ID=?STUDENT_ID AND IS_REGISTERED=1;
                                   SET SQL_SAFE_UPDATES = 1;";

                        break;
                    }
                case FeeSQLCommands.FetchSettlementByDateWise:
                    {
                        query = @"SELECT 
                                                             
                                from_unixtime(RS.CREATED_AT, '%d/%m/%Y') AS PAYMENT_DATE,
                                round((RS.AMOUNT/100),2) AMOUNT,RS.ID,
                                
                                RAC.ACCOUNT_NAME
                            FROM
                                FEE_RAZORPAY_SETTLEMENT_INFO_?AC_YEAR  AS RS
                                
                                    INNER JOIN
                                FEE_RAZORPAY_ACCOUNTS AS RAC ON RAC.ACCOUNT_ID = RS.RECIPIENT
                            WHERE
                                RS.CREATED_AT BETWEEN unix_timestamp(?DATE_FROM) AND unix_timestamp(?DATE_TO) 
                                AND RAC.BANK_ACCOUNT_ID=?BANK_ACCOUNT_ID
                                
                                ;";
                        break;
                    }
                case FeeSQLCommands.FetchSettlementByDateWiseAndAccount:
                    {
                        query = @"SELECT 
                                SC.STUDENT_ID,
                                UPPER(CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, ''))) AS FIRST_NAME,
                                CL.CLASS_NAME,
                                from_unixtime(FT.CREATED_AT, '%d/%m/%Y') AS PAYMENT_DATE,
                                ROUND(FT.AMOUNT/100) AS AMOUNT,
                                
                                RAC.ACCOUNT_NAME,IF(SP.IS_LEFT=1,'LEFT','-') AS IS_LEFT
                            FROM
                                FEE_RAZORPAY_TRANSFER_INFO_?AC_YEAR  AS FT
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = FT.udf1
                                    AND SC.IS_DELETED != 1
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    LEFT JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    -- AND SP.IS_LEFT != 1
                                    INNER JOIN
                                CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                    AND CL.IS_DELETED != 1
                                
                                
                                    INNER JOIN
                                FEE_RAZORPAY_ACCOUNTS AS RAC ON RAC.ACCOUNT_ID = FT.RECIPIENT
                            WHERE
                                FT.RECIPIENT_SETTLEMENT_ID=?RECIPIENT_SETTLEMENT_ID ORDER BY FT.CREATED_AT;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeStudentAmountByPaymentId:
                    {
                        query = @"SELECT 
                                    MH.MAIN_HEAD_ID,
                                    SUM(FS.AMOUNT) AS TOTAL_AMOUNT,
                                    MH.MAIN_HEAD,
                                    RAC.ACCOUNT_NAME,
                                    CONCAT(IFNULL(SB.PASSBOOK_NO, ''),
                                    '- ',
                                    IFNULL(B.BANK_NAME, '')) AS PASSBOOK_NO
                                FROM
                                    FEE_STRUCTURE AS FS
                                        LEFT JOIN
                                    FEE_MAIN_HEADS AS FMH ON FMH.FEE_MAIN_HEAD_ID = FS.FEE_MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS MH ON MH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_BANK_ACCOUNT AS SB ON SB.BANK_ACCOUNT_ID = FMH.BANK_ACCOUNT_ID
                                        LEFT JOIN
                                    SUP_BANK AS B ON B.BANK_ID = SB.BANK
                                        LEFT JOIN
                                    FEE_RAZORPAY_TRANSFER_INFO_?AC_YEAR AS RP ON RP.UDF3 = FMH.MAIN_HEAD_ID
                                        AND `SOURCE` IN (?RAZOR_PAY_ID)
                                        LEFT JOIN
                                    FEE_RAZORPAY_ACCOUNTS AS RAC ON RAC.ACCOUNT_ID = RP.RECIPIENT
                                WHERE
                                    PROGRAMME = ?PROGRAMME_GROUP_ID AND CLASS_YEAR_ID = ?CLASS_YEAR
                                        AND FREQUENCY = ?FREQUENCY
                                GROUP BY FMH.MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchMainHeadByProgrammeAndFrequency:
                    {
                        query = @"SELECT 
                                        FM.MAIN_HEAD_ID, FM.HEAD_ID
                                    FROM
                                        CP_PROGRAMME_GROUP AS PG
                                            INNER JOIN
                                        FEE_MAIN_HEADS AS FM ON FM.APPLICATION_TYPE_ID = PG.APPTYPE_ID
                                            AND FM.SHIFT = PG.SHIFT
                                            AND FM.PROGRAMME_MODE = PG.PROGRAMME_MODE
                                            AND FM.FEE_ROOT_ID = 1
                                    WHERE
                                        FM.FREQUENCY_ID = ?FREQUENCY_ID
                                            AND PG.PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                            AND FM.HEAD_ID = ?HEAD_ID
                                            AND FM.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND FM.IS_DELETED != 1;";
                        break;
                    }

                #region Term Fee Abstract Report
                case FeeSQLCommands.FetchTermFeeAbstractByProgrammeGroupIds:
                    {
                        query = @"SELECT 
                                FT.STUDENT_ID,
                                FC.PAID_AMOUNT AS BALANCE,
                                FM.HEAD_ID,
                                FM.MAIN_HEAD_ID,
                                PG.PROGRAMME_GROUP_ID,
                                DATE_FORMAT(FT.PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,
                                FROM_UNIXTIME(FC.SETTLEMENT_DATE, '%d/%m/%Y') AS SETTLEMENT_DATE
                            FROM
                                FEE_TRANSACTION AS FT
                                    INNER JOIN
                                FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID
                                    INNER JOIN
                                FEE_MAIN_HEADS AS FM ON FM.FEE_MAIN_HEAD_ID = FC.FEE_MAIN_HEAD_ID AND FM.FEE_CATEGORY_ID IN (?FEE_CATEGORY_ID)                               
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = FT.STUDENT_ID
                                    AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    INNER JOIN
                                CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                    AND CLASS_YEAR = ?CLASS_YEAR
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                            WHERE
                                FT.FREQUENCY = ?FREQUENCY                              
                                    AND FT.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    AND PG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                            ORDER BY FC.SETTLEMENT_DATE;";
                        break;
                    }
                #endregion
                case FeeSQLCommands.FetchFrequencyIdByFeeModeAndActiveSemester:
                    {
                        query = @"SELECT 
                                FFF.FEE_FREQUENCY_FEE_MODE_ID AS FREQUENCY_ID,
                                CONCAT(IFNULL(SFF.FREQUENCY_NAME, ''),
                                        '( ',
                                        IFNULL(SFT.FREQUENCY_TYPE, ''),
                                        ')') AS FREQUENCY_NAME
                            FROM
                                FEE_FREQUENCY_FEE_MODE AS FFF
                                    INNER JOIN
                                SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                    INNER JOIN
                                SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                    INNER JOIN
                                SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = SFF.SEMESTER_TYPE
                                    AND SST.IS_ACTIVE = 1
                            WHERE
                                FFF.ACADEMIC_YEAR = ?AC_YEAR
                                    AND FFF.FEE_MODE = ?FEE_MODE
                                    AND SFT.IS_DELETED != 1
                                    AND SFF.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case FeeSQLCommands.SelectInsertForFeeStudentAccountByFrequencyAndProgrammeAndClassyear:
                    {
                        query = @"INSERT INTO FEE_STUDENT_ACCOUNT(STUDENT_ID,ACADEMIC_YEAR,FREQUENCY_ID,HEAD,CREDIT,FEE_MAIN_HEAD_ID)
                                SELECT 
                                '?STUDENT_ID' AS STUDENT_ID,
                                    FS.ACADEMIC_YEAR,
                                    FS.FREQUENCY AS FREQUENCY_ID,
                                    SH.HEAD_ID AS HEAD,
                                    FS.AMOUNT AS CREDIT,
                                    FS.FEE_MAIN_HEAD_ID
                                FROM
                                    FEE_STRUCTURE AS FS
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS MH ON MH.FEE_MAIN_HEAD_ID = FS.FEE_MAIN_HEAD_ID
                                        AND MH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY
                                        AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS SFF ON FFF.FREQUENCY_ID = SFF.FREQUENCY_ID
                                        AND SFF.ACADEMIC_YEAR = ?AC_YEAR
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS SST ON SST.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        INNER JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FS.HEAD
                                        INNER JOIN
                                    SUP_FEE_CATEGORY AS SFC ON SFC.FEE_CATEGORY_ID = FS.FEE_CATEGORY
                                WHERE
                                    FS.IS_DELETED != 1
                                        AND SH.IS_DELETED != 1
                                        AND FS.CLASS_YEAR_ID = ?CLASS_YEAR_ID
                                        AND FS.PROGRAMME = ?PROGRAMME
                                        AND FS.FREQUENCY = ?FREQUENCY
                                        AND FS.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case FeeSQLCommands.FetchStudentTransactionByFrequencytype:
                    {
                        query = @"SELECT 
                                    FT.STUDENT_ID,
                                    CONCAT(CP.PROGRAMME_NAME,
                                            '- ',
                                            PM.PROGRAMME_MODE,
                                            '- ',
                                            SH.DESCRIPTION) AS PROGRAMME_NAME,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    FT.TRANSACTION_ID,
                                    DATE_FORMAT(PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,
                                    FT.RECEIPT_NO,
                                    SPM.PAYMENT_MODE,
                                    COLLECTED,
                                    FFF.FEE_FREQUENCY_FEE_MODE_ID AS FREQUENCY_ID
                                FROM
                                    FEE_TRANSACTION AS FT
                                        LEFT JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = FT.STUDENT_ID
                                        AND SC.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
                                    CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CL.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                                        INNER JOIN
                                    SUP_PAYMENT_MODE AS SPM ON SPM.PAYMENT_MODE_ID = FT.PAYMENT_MODE
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FT.FREQUENCY
                                        AND FEE_MODE = ?FREQUENCY_TYPE
                                        AND SC.ACADEMIC_YEAR=FFF.ACADEMIC_YEAR
                                        INNER JOIN
                                    sup_fee_frequency AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        INNER JOIN
                                    SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                                        INNER JOIN
                                    SUP_SHIFT AS SH ON SH.SHIFT_ID = PG.SHIFT
                                WHERE
                                    FT.STUDENT_ID = ?STUDENT_ID AND
                                    FT.FREQUENCY= ?FREQUENCY_ID
                                        AND FT.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeReceiptByTransactionId:
                    {
                        query = @"SELECT 
                                        FT.TRANSACTION_ID,
                                        FT.FREQUENCY,
                                        FT.PAYMENT_DATE,
                                        FT.RECEIPT_NO,
                                        FMH.MAIN_HEAD_ID,
                                        SFT.FREQUENCY_TYPE AS FREQUENCY_NAME,
                                        cONCAT(MH.MAIN_HEAD,' (',SH.HEAD_CODE,')') AS MAIN_HEAD,
                                        FT.FEE_TRANSACTION_BANK,
                                        SUM(IFNULL(PAID_AMOUNT, 0)) AS DEBIT,
                                        SFT.FREQUENCY_TYPE_ID
                                    FROM
                                        FEE_TRANSACTION AS FT
                                            INNER JOIN
                                        FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID
                                            INNER JOIN
                                        FEE_MAIN_HEADS AS FMH ON FMH.FEE_MAIN_HEAD_ID = FC.FEE_MAIN_HEAD_ID
                                            INNER JOIN
                                        SUP_FEE_MAIN_HEAD AS MH ON MH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                            INNER JOIN
                                        SUP_HEAD AS SH ON SH.HEAD_ID = FMH.HEAD_ID
                                            INNER JOIN
                                        FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FT.FREQUENCY
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                    WHERE
                                            FT.TRANSACTION_ID = ?TRANSACTION_ID
                                            AND FC.IS_DELETED != 1
                                            AND FT.IS_DELETED !=1
                                            AND FC.FREQUENCY = ?FREQUENCY_ID
                                    GROUP BY FMH.MAIN_HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.SaveFeeTransactionForBankExcelUpload:
                    {
                        query = @"INSERT INTO FEE_TRANSACTION
                                  (
					                  STUDENT_ID,
					                  FREQUENCY,
					                  CLASS,
					                  PAYMENT_DATE,
					                  RECEIPT_NO,
					                  PAYMENT_MODE,
					                  COLLECTED,
					                  USERNAME,
					                  ACADEMIC_YEAR,
					                  FREQUENCY_TO,
                                      FEE_TRANSACTION_BANK
				                )
                                VALUES
                                (
					                ?STUDENT_ID,
					                ?FREQUENCY,
					                ?CLASS,
					                ?PAYMENT_DATE,
					                ?RECEIPT_NO,
					                '6',
					                ?COLLECTED,
					                ?USERNAME,
					                ?AC_YEAR,
					                ?FREQUENCY_TO,
					                ?FEE_TRANSACTION_BANK
				                );";
                        break;
                    }
                case FeeSQLCommands.SaveFeeStudentAccountForBankExcelUpload:
                    {
                        query = @"INSERT INTO FEE_STUDENT_ACCOUNT
                                  (
	                                  STUDENT_ID,
	                                  ACADEMIC_YEAR,
	                                  FREQUENCY_ID,
	                                  HEAD,
	                                  CREDIT,
	                                  FEE_MAIN_HEAD_ID
                                  ) VALUES
                                  (
                                      ?STUDENT_ID,
	                                  ?AC_YEAR,
	                                  ?FREQUENCY_ID,
	                                  ?HEAD,
	                                  ?CREDIT,
	                                  ?FEE_MAIN_HEAD_ID
                                  );";
                        break;
                    }
                case FeeSQLCommands.FetchFeeMainHeadLateFee:
                    {
                        query = @"SELECT 
                                      SP.STUDENT_ID, FM.FEE_MAIN_HEAD_ID
                                  FROM
                                      STU_PERSONAL_INFO AS SP
                                          INNER JOIN
                                      CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                          INNER JOIN
                                      FEE_MAIN_HEADS AS FM ON FM.APPLICATION_TYPE_ID = PG.APPTYPE_ID
                                          AND FM.SHIFT = PG.SHIFT
                                          AND FM.ACADEMIC_YEAR = ?AC_YEAR
                                          AND FM.FEE_ROOT_ID = 1
                                          AND FM.FREQUENCY_ID = ?FREQUENCY
                                          AND FM.IS_DELETED != 1
                                  WHERE
                                      FM.HEAD_ID = ?HEAD AND SP.STUDENT_ID = ?STUDENT_ID
                                          AND SP.IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.FetchFrequencyIdByFeeModes:
                    {
                        query = @"SELECT 
                                        group_concat(FEE_FREQUENCY_FEE_MODE_ID )  AS FREQUENCY_ID
                                    FROM
                                        FEE_FREQUENCY_FEE_MODE AS FFF
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID=FFF.FREQUENCY_ID AND SFF.ACADEMIC_YEAR=?AC_YEAR
		                                        left JOIN
	                                    SUP_SEMESTER_TYPE AS SMT ON SMT.SEMESTER_TYPE_ID=SFF.SEMESTER_TYPE 
                                    WHERE
                                        FFF.FEE_MODE IN(?FEE_MODE)
                                            AND FFF.ACADEMIC_YEAR = ?AC_YEAR";
                        break;
                    }
                #region Fee Bank Copy
                case FeeSQLCommands.FetchFeeMainHeadsByFrequencyId:
                    {
                        query = @"SELECT 
                                        H.HEAD_ID,
                                        H.HEAD
                                    FROM
                                        FEE_MAIN_HEADS AS FM
		                                    INNER JOIN
	                                    SUP_HEAD AS H ON H.HEAD_ID=FM.HEAD_ID
                                    WHERE
                                        FM.IS_DELETED != 1
                                            AND FM.FREQUENCY_ID = 2
                                    GROUP BY HEAD_ID ORDER BY HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchStudentsAccountByFrequencyId:
                    {
                        query = @"SELECT 
                                        SC.STUDENT_ID,
                                        UPPER(CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                        '  ',
                                                        IFNULL(SP.LAST_NAME, ''))) AS FIRST_NAME,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        C.CLASS_NAME,
                                        ACC.HEAD,
                                        ACC.CREDIT,
                                        PG.PROGRAMME_GROUP_CODE
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=C.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        SUP_PROGRAMME_TYPE AS PT ON PT.PROGRAMME_TYPE_ID = C.CLASS_LEVEL
                                            INNER JOIN
                                        FEE_STUDENT_ACCOUNT AS ACC ON ACC.STUDENT_ID = SC.STUDENT_ID
                                            AND ACC.FREQUENCY_ID = ?FREQUENCY_ID
                                    WHERE
                                        SC.IS_DELETED != 1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    ORDER BY C.CLASS_ID,SC.STUDENT_ID;";
                        break;
                    }
                #endregion

                #region Fee Exist
                case FeeSQLCommands.CheckIsStudentAccountExist:
                    {
                        query = @"SELECT 
                                        STUDENT_ID, 
                                        FREQUENCY_ID, 
                                        HEAD, 
                                        CREDIT, 
                                        FEE_MAIN_HEAD_ID
                                    FROM
                                        FEE_STUDENT_ACCOUNT
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID AND FREQUENCY_ID = ?FREQUENCY_ID
                                            AND HEAD = ?HEAD
                                            AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case FeeSQLCommands.CheckIsFeeTransactionExist:
                    {
                        query = @"";
                        break;
                    }
                case FeeSQLCommands.FetchStudentInfo:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.ROLL_NO,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(LAST_NAME, '')) AS FIRST_NAME,
                                        CC.CLASS_CODE,
                                        DD.TEXT2,
                                        DD.TEXT3
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.STUDENT_ID != 1
                                            AND SC.IS_DELETED != 1
                                            LEFT JOIN
                                        DEMO_TEST_TABLE AS DD ON DD.TEXT1 = SP.ROLL_NO
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY SP.STUDENT_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeStudentAccount:
                    {
                        query = @"SELECT 
                                       FS.STUDENT_ID,
                                       SP.FIRST_NAME,
                                       FS.FREQUENCY_ID,
                                       SF.FREQUENCY_TYPE,
                                       SUM(IFNULL(FS.CREDIT, 0) - IFNULL(FS.DEBIT, 0)) AS BALANCE
                                       
                                   FROM
                                       FEE_STUDENT_ACCOUNT AS FS
                                           INNER JOIN
                                       STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = FS.STUDENT_ID
                                           AND SP.IS_DELETED != 1
                                           INNER JOIN 
                                       FEE_FREQUENCY_FEE_MODE AS FF ON FF.FEE_FREQUENCY_FEE_MODE_ID=FS.FREQUENCY_ID
                                           AND FF.ACADEMIC_YEAR=FS.ACADEMIC_YEAR
                                           INNER JOIN
                                       SUP_FREQUENCY_TYPE AS SF ON SF.FREQUENCY_TYPE_ID=FF.FEE_MODE
                                    WHERE FS.ACADEMIC_YEAR=?AC_YEAR
                                   GROUP BY FS.STUDENT_ID , FS.FREQUENCY_ID
                                   ;";
                        break;
                    }
                #endregion

                #region Gate Fee Entry
                case FeeSQLCommands.FetchGateFeeByStudentId:
                    {
                        query = @"SELECT 
                                        LATE_FEE_ID,
                                        SL.STUDENT_ID,
                                        FREQUENCY_ID,
                                        FINE_AMOUNT,
                                        DATE_FORMAT(SL.ENTRY_DATE,'%d/%m/%Y %H:%i:%s') ENTRY_DATE,
                                        SL.ENTRY_ID,
                                        IS_FINE_ADDED,
                                        IS_LEAVE_ADDED,
                                        SP.ROLL_NO,
                                        IF(DATE_FORMAT(SL.ENTRY_DATE,'%Y-%m-%d')=CURDATE(),'1','0') AS IS_CURDATE,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        STF.STAFF_CODE,
                                        CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(STF.LAST_NAME, '')) AS 'STAFF_NAME'
                                    FROM
                                        STU_LATE_ENTRY AS SL
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SL.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = SL.ENTRY_ID
                                            AND STF.IS_DELETED != 1
                                    WHERE
                                        SL.IS_DELETED != 1
                                            AND SL.STUDENT_ID = ?STUDENT_ID
                                            AND SL.FREQUENCY_ID = ?FREQUENCY_ID
                                            AND SL.ACADEMIC_YEAR=?AC_YEAR ORDER BY LATE_FEE_ID DESC;";
                        break;
                    }
                case FeeSQLCommands.FetchCreditOnlyByStudentIdAndFrequencyId:
                    {
                        query = @"SELECT 
                                        F_STUDENT_AC_ID,
                                        STUDENT_ID,
                                        ACADEMIC_YEAR,
                                        FREQUENCY_ID,
                                        HEAD,
                                        CREDIT,
                                        FEE_MAIN_HEAD_ID
                                    FROM
                                        FEE_STUDENT_ACCOUNT
                                    WHERE
                                        FREQUENCY_ID = ?FREQUENCY_ID AND STUDENT_ID = ?STUDENT_ID
                                            AND CREDIT IS NOT NULL;";
                        break;
                    }
                case FeeSQLCommands.UpdateFeeStudentAccount:
                    {
                        query = @"UPDATE FEE_STUDENT_ACCOUNT SET CREDIT=CREDIT + ?CREDIT WHERE F_STUDENT_AC_ID=?F_STUDENT_AC_ID;";
                        break;
                    }
                case FeeSQLCommands.SaveFeeStudentAccountCreation:
                    {
                        query = @"INSERT INTO FEE_STUDENT_ACCOUNT
                                  (
	                                  STUDENT_ID,
	                                  ACADEMIC_YEAR,
	                                  FREQUENCY_ID,
	                                  HEAD,
	                                  CREDIT,
	                                  FEE_MAIN_HEAD_ID
                                  )
                                  VALUES
                                  (
	                                  ?STUDENT_ID,
	                                  ?AC_YEAR,
	                                  ?FREQUENCY_ID,
	                                  ?HEAD,
	                                  ?CREDIT,
	                                  ?FEE_MAIN_HEAD_ID
                                  );";
                        break;
                    }
                case FeeSQLCommands.SaveStudentLateEntry:
                    {
                        query = @"INSERT INTO STU_LATE_ENTRY
                                  (
	                                  STUDENT_ID,
	                                  FREQUENCY_ID,
	                                  FINE_AMOUNT,
	                                  ENTRY_ID,
	                                  ACADEMIC_YEAR,
	                                  IS_FINE_ADDED,
	                                  SEMESTER_TYPE,
	                                  IS_LEAVE_ADDED
                                  )
                                  VALUES
                                  (
                                     ?STUDENT_ID,
	                                 ?FREQUENCY_ID,
	                                 ?FINE_AMOUNT,
	                                 ?ENTRY_ID,
	                                 ?AC_YEAR,
	                                 ?IS_FINE_ADDED,
	                                 ?SEMESTER_TYPE,
	                                 ?IS_LEAVE_ADDED
                                  );";
                        break;
                    }
                case FeeSQLCommands.FetchLateFeeByDate:
                    {
                        query = @"SELECT 
                                        SP.ROLL_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        STF.STAFF_CODE,
                                        CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(STF.LAST_NAME, '')) AS 'STAFF_NAME',
                                        DATE_FORMAT(SL.ENTRY_DATE,'%d/%m/%Y %H:%i:%s') ENTRY_DATE,
                                        FINE_AMOUNT,
                                        SL.LATE_FEE_ID
                                    FROM
                                        STU_LATE_ENTRY AS SL
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SL.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = SL.ENTRY_ID
                                           
                                    WHERE
                                        SL.IS_DELETED != 1 AND SL.IS_SMS_SENT !=1
                                            AND DATE_FORMAT(SL.ENTRY_DATE,'%Y-%m-%d')=CURDATE()
                                            AND SL.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case FeeSQLCommands.FetchLateFeeInfoForSMSByDate:
                    {
                        query = @"SELECT 
                                        MOBILE AS 'to',
                                        CONCAT('Dear parent of ',
                                                UA.NAME,
                                                ', \n   Your Son / Daughter  ',
                                                TIME_FORMAT(TIMEDIFF(DATE_FORMAT(s.ENTRY_DATE, '%H:%i:%s'),
                                                                '08:25:00'),
                                                        '%i Mins : %s Sec '),
                                                ' late today (',
                                                DATE_FORMAT(CURDATE(), '%d/%m/%Y'),
                                                ')  to the College. Regards SJC (Cuddalore)') AS message,
                                        sp.student_id AS custom,
                                        '0' AS custom1,
                                        '0' AS custom2,
                                        '1' AS CreditCount
                                    FROM
                                        STU_LATE_ENTRY AS S
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = S.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USERNAME = SP.ROLL_NO
                                    WHERE
                                        DATE_FORMAT(S.ENTRY_DATE, '%Y-%m-%d') = CURDATE()
                                        AND S.IS_SMS_SENT!=1
                                    ORDER BY LATE_FEE_ID;";
                        break;
                    }
                case FeeSQLCommands.UpdateLateFeeSMSUpdate:
                    {
                        query = @"UPDATE STU_LATE_ENTRY SET IS_SMS_SENT=1 WHERE LATE_FEE_ID!=0;";
                        break;
                    }
                case FeeSQLCommands.FetchLateEntryStudentDateWise:
                    {
                        query = @"SELECT 
                                        SPI.ROLL_NO,
                                        CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                        STF.STAFF_CODE,
                                        CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(STF.LAST_NAME, '')) AS 'STAFF_NAME',
                                        DATE_FORMAT(SL.ENTRY_DATE, '%d/%m/%Y') ENTRY_DATE,
                                        DATE_FORMAT(SL.ENTRY_DATE,  '%H:%i:%s') ENTRY_TIME,
                                        FINE_AMOUNT,
                                        CC.CLASS_NAME,
                                        CC.CLASS_CODE,
                                        SS.SHIFT_NAME,
                                        SPI.FR_MOBILE,
                                        SPI.STU_MOBILENO
                                    FROM
                                        STU_LATE_ENTRY AS SL
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SL.STUDENT_ID
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            left JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = SL.ENTRY_ID
                                            AND STF.IS_DELETED != 1
                                            INNER JOIN
                                        stu_class AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        sup_shift AS SS ON SS.SHIFT_ID = CC.SHIFT
                                    WHERE
                                    
                                        SL.IS_DELETED != 1 AND SL.STUDENT_ID = ?STUDENT_ID
                                            AND DATE_FORMAT(SL.ENTRY_DATE, '%Y-%m-%d') BETWEEN '?FROM_DATE' AND '?TODATE'
                                            AND SL.ACADEMIC_YEAR = ?AC_YEAR
                                      
                                    ORDER BY CC.CLASS_ID , SPI.ROLL_NO ,STR_TO_DATE(SL.ENTRY_DATE, '%d/%m/%Y %H:%i:%s');";
                        break;
                    }
                case FeeSQLCommands.FetchStudentLateEntryAbsentReport:
                    {
                        query = @"SELECT 
                                        SPI.STUDENT_ID,
                                        SPI.ROLL_NO,
                                        CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                        CC.CLASS_NAME,
                                        COUNT(SL.LATE_FEE_ID) AS COUNT
                                    FROM
                                        STU_LATE_ENTRY AS SL
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SL.STUDENT_ID
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            INNER JOIN
                                        stu_class AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        SL.IS_DELETED != 1
                                            AND SL.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SL.SEMESTER_TYPE = ?SEMESTER_TYPE
                                            ?CLASS_ID_CONDITION
                                    GROUP BY SPI.STUDENT_ID, SPI.ROLL_NO, SPI.FIRST_NAME, SPI.LAST_NAME, CC.CLASS_NAME
                                    ORDER BY CC.CLASS_NAME, SPI.ROLL_NO;";
                        break;
                    }
                case FeeSQLCommands.FetchLateEntryReportDateWise:
                    {
                        query = @"SELECT 
                                        SPI.ROLL_NO,
                                        CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                        STF.STAFF_CODE,
                                        CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(STF.LAST_NAME, '')) AS 'STAFF_NAME',
                                        DATE_FORMAT(SL.ENTRY_DATE, '%d/%m/%Y %H:%i:%s') ENTRY_DATE,
                                        FINE_AMOUNT,
                                        CC.CLASS_NAME,
                                        CC.CLASS_CODE,
                                        SS.SHIFT_NAME,
                                        SPI.FR_MOBILE,
                                        SPI.STU_MOBILENO
                                    FROM
                                        STU_LATE_ENTRY AS SL
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SL.STUDENT_ID
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            left JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = SL.ENTRY_ID
                                            AND STF.IS_DELETED != 1
                                            INNER JOIN
                                        stu_class AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        sup_shift AS SS ON SS.SHIFT_ID = CC.SHIFT
                                    WHERE
                                    
                                        SL.IS_DELETED != 1
                                            AND DATE_FORMAT(SL.ENTRY_DATE, '%Y-%m-%d') BETWEEN '?FROM_DATE' AND '?TODATE'
                                            AND SL.ACADEMIC_YEAR = ?AC_YEAR
                                      
                                    ORDER BY CC.CLASS_ID , SPI.ROLL_NO ,STR_TO_DATE(SL.ENTRY_DATE, '%d/%m/%Y %H:%i:%s');";
                        break;
                    }
                case FeeSQLCommands.FetchLateEntryStudentClassWise:
                    {
                        query = @"SELECT 
                                        SPI.ROLL_NO,
                                        CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                        STF.STAFF_CODE,
                                        CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(STF.LAST_NAME, '')) AS 'STAFF_NAME',
                                        DATE_FORMAT(SL.ENTRY_DATE, '%d/%m/%Y %H:%i:%s') ENTRY_DATE,
                                        FINE_AMOUNT,
                                        CC.CLASS_NAME,
                                        CC.CLASS_CODE,
                                        SS.SHIFT_NAME,
                                        SPI.FR_MOBILE
                                        
                                    FROM
                                        STU_LATE_ENTRY AS SL
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SL.STUDENT_ID
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            left JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = SL.ENTRY_ID
                                            AND STF.IS_DELETED != 1
                                            INNER JOIN
                                        stu_class AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        sup_shift AS SS ON SS.SHIFT_ID = CC.SHIFT
                                    WHERE
                                        SL.IS_DELETED != 1
                                        and sc.CLASS_ID in (?CLASS_ID)
                                            AND SL.ACADEMIC_YEAR = ?AC_YEAR
                                    ORDER BY SL.ENTRY_DATE, CC.CLASS_ID , SPI.ROLL_NO ;";
                        break;
                    }
                case FeeSQLCommands.FetchGateFeeByStudentIdAndFrequency:
                    {
                        query = @"SELECT 
                                        LATE_FEE_ID,
                                        SL.STUDENT_ID,
                                        FREQUENCY_ID,
                                        FINE_AMOUNT,
                                        DATE_FORMAT(SL.ENTRY_DATE,'%d/%m/%Y %H:%i:%s') ENTRY_DATE,
                                        SL.ENTRY_ID,
                                        IS_FINE_ADDED,
                                        IS_LEAVE_ADDED,
                                        SP.ROLL_NO,
                                        IF(DATE_FORMAT(SL.ENTRY_DATE,'%Y-%m-%d')=CURDATE(),'1','0') AS IS_CURDATE,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        STF.STAFF_CODE,
                                        CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(STF.LAST_NAME, '')) AS 'STAFF_NAME'
                                    FROM
                                        STU_LATE_ENTRY AS SL
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SL.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            LEFT JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = SL.ENTRY_ID
                                            AND STF.IS_DELETED != 1
                                    WHERE
                                        SL.IS_DELETED != 1
                                            AND SL.STUDENT_ID = ?STUDENT_ID AND SL.SEMESTER_TYPE=?SEMESTER_TYPE
                                            AND SL.FREQUENCY_ID = ?FREQUENCY_ID
                                            AND SL.ACADEMIC_YEAR=?AC_YEAR ORDER BY LATE_FEE_ID DESC;";
                        break;
                    }
                #endregion

                #region Offline Fee Collection
                case FeeSQLCommands.FetchFeeTransactionByRollNo:
                    {
                        query = @"SELECT 
                                        SUM(IFNULL(FS.CREDIT, '0')) - SUM(IFNULL(FS.DEBIT, '0')) AS BALANCE,
                                        SUM(FS.DEBIT) AS DEBIT,
                                        FT.FREQUENCY_TYPE_ID,
                                        FS.STUDENT_ID,
                                        FS.HEAD,
                                        SC.CLASS_ID,
                                        FT.FREQUENCY_TYPE AS FREQUENCY_NAME,
                                        FT.FEE_ROOT_ID
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        FEE_STUDENT_ACCOUNT AS FS ON FS.STUDENT_ID = SC.STUDENT_ID
                                            AND FS.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            INNER JOIN
                                        FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID
                                            AND FFF.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                            AND FF.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                    WHERE
                                        SP.ROLL_NO = ?ROLL_NO
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY FFF.FEE_MODE;";
                        break;
                    }
                #endregion
                #region Certificates
                case FeeSQLCommands.BindFeeStructureForEditByProgrammeMode:
                    {
                        query = @"SELECT 
                                FS.FEE_STRUCTURE_ID,
                                FMH.FEE_MAIN_HEAD_ID,
                                FS.ACADEMIC_YEAR,
                                FMH.APPLICATION_TYPE_ID,
                                FMH.PROGRAMME_MODE,
                                FMH.SHIFT,
                                FMH.FREQUENCY_ID,
                                SH.HEAD_ID,
                                SH.HEAD,
                                SFM.MAIN_HEAD,
                                FS.AMOUNT,
                                FMH.BANK_ACCOUNT_ID,
                                FMH.FEE_CATEGORY_ID,
                                FC.FEE_CATEGORY AS FEE_CATEGORY_NAME,
                                CONCAT(BA.PASSBOOK_NO, ' - ', B.BANK_NAME) AS PASSBOOK_NO
                            FROM
                                FEE_MAIN_HEADS AS FMH
                                    LEFT JOIN
                                FEE_STRUCTURE_FOR_CERTIFICATES AS FS ON FS.FEE_MAIN_HEAD_ID = FMH.FEE_MAIN_HEAD_ID
                                    AND FS.FREQUENCY = FMH.FREQUENCY_ID
                                    AND FS.ACADEMIC_YEAR =?AC_YEAR
                                    AND FS.IS_DELETED != 1
                                    INNER JOIN
                                SUP_HEAD AS SH ON SH.HEAD_ID = FMH.HEAD_ID
                                    INNER JOIN
                                SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                    LEFT JOIN
                                SUP_FEE_CATEGORY AS FC ON FC.FEE_CATEGORY_ID = FMH.FEE_CATEGORY_ID
                                    LEFT JOIN
                                SUP_BANK_ACCOUNT AS BA ON BA.BANK_ACCOUNT_ID = FMH.BANK_ACCOUNT_ID
                                    LEFT JOIN
                                SUP_BANK AS B ON B.BANK_ID = BA.BANK
                            WHERE
                                FMH.ACADEMIC_YEAR =?AC_YEAR
                                    AND FMH.FREQUENCY_ID =?FREQUENCY
                                    AND FMH.PROGRAMME_MODE =?PROGRAMME_MODE
                                    AND FMH.SHIFT = ?SHIFT;";
                        break;
                    }
                case FeeSQLCommands.BindFeeStructureForEditByModeandShift:
                    {
                        query = @"SELECT 
                                        FS.FEE_STRUCTURE_ID,
                                        FMH.FEE_MAIN_HEAD_ID,
                                        FS.ACADEMIC_YEAR,
                                        FMH.APPLICATION_TYPE_ID,
                                        FMH.PROGRAMME_MODE,
                                        FMH.SHIFT,
                                        FMH.FREQUENCY_ID,
                                        SH.HEAD_ID,
                                        SH.HEAD,
                                        SFM.MAIN_HEAD,
                                        FS.AMOUNT,
                                        FMH.BANK_ACCOUNT_ID,
                                        FMH.FEE_CATEGORY_ID,
                                        FC.FEE_CATEGORY AS FEE_CATEGORY_NAME,
                                        CONCAT(BA.PASSBOOK_NO, ' - ', B.BANK_NAME) AS PASSBOOK_NO
                                    FROM
                                        FEE_MAIN_HEADS AS FMH
                                            LEFT JOIN
                                        FEE_STRUCTURE_FOR_CERTIFICATES AS FS ON FS.FEE_MAIN_HEAD_ID = FMH.FEE_MAIN_HEAD_ID
                                            AND FS.FREQUENCY = FMH.FREQUENCY_ID
                                            AND FS.ACADEMIC_YEAR =?AC_YEAR
                                            AND FS.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_HEAD AS SH ON SH.HEAD_ID = FMH.HEAD_ID
                                            INNER JOIN
                                        SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                            LEFT JOIN
                                        SUP_FEE_CATEGORY AS FC ON FC.FEE_CATEGORY_ID = FMH.FEE_CATEGORY_ID
                                            LEFT JOIN
                                        SUP_BANK_ACCOUNT AS BA ON BA.BANK_ACCOUNT_ID = FMH.BANK_ACCOUNT_ID
                                            LEFT JOIN
                                        SUP_BANK AS B ON B.BANK_ID = BA.BANK
                                    WHERE
                                        FMH.ACADEMIC_YEAR =?AC_YEAR
                                            AND FMH.FREQUENCY_ID =?FREQUENCY
                                            AND FMH.PROGRAMME_MODE =?PROGRAMME_MODE
                                            AND FMH.SHIFT = ?SHIFT;";
                        break;
                    }
                case FeeSQLCommands.UpdateCertificateFeeStructure:
                    {
                        query = @"UPDATE FEE_STRUCTURE_FOR_CERTIFICATES
                                        SET
                                        ACADEMIC_YEAR= ?ACADEMIC_YEAR,
                                        FREQUENCY= ?FREQUENCY,								
                                        HEAD= ?HEAD,
                                        AMOUNT= ?AMOUNT,
                                        FEE_CATEGORY= ?FEE_CATEGORY,                                      
                                        BANK_ACCOUNT_ID= ?BANK_ACCOUNT_ID,
                                        FEE_MAIN_HEAD_ID=?FEE_MAIN_HEAD_ID
                                        WHERE FEE_STRUCTURE_ID= ?FEE_STRUCTURE_ID;";
                        break;
                    }
                case FeeSQLCommands.SaveCertificateFeeStructure:
                    {
                        query = @"INSERT INTO FEE_STRUCTURE_FOR_CERTIFICATES
                                    (
                                    ACADEMIC_YEAR,
                                    FREQUENCY,
                                    HEAD,
                                    AMOUNT,
                                    FEE_CATEGORY,
                                    BANK_ACCOUNT_ID,FEE_MAIN_HEAD_ID)
                                    VALUES
                                    (
                                    ?ACADEMIC_YEAR,
                                    ?FREQUENCY,
                                    ?HEAD,
                                    ?AMOUNT,
                                    ?FEE_CATEGORY,
                                    ?BANK_ACCOUNT_ID,?FEE_MAIN_HEAD_ID);";
                        break;
                    }
                case FeeSQLCommands.DeleteCertificateFeeStructure:
                    {
                        query = @"UPDATE FEE_STRUCTURE_FOR_CERTIFICATES
                                SET
                                IS_DELETED=1
                                WHERE FEE_STRUCTURE_ID = ?FEE_STRUCTURE_ID;";
                        break;
                    }
                #endregion

                case FeeSQLCommands.FetchStudentbyProgrammeGroupAndClassYearForFee:
                    {
                        query = @"SELECT 
                                SC.STUDENT_ID,
                                UPPER(SP.FIRST_NAME) AS FIRST_NAME,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                PG.PROGRAMME_GROUP_ID,
                                UPPER(CL.CLASS_NAME) AS CLASS_NAME,
                                UPPER(CONCAT((CP.PROGRAMME_NAME),
                                                ' (SHIFT - ',
                                                (SS.SHIFT_NAME),
                                                ') ',
                                                (PM.PROGRAMME_MODE))) AS PROGRAMME_NAME
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    INNER JOIN
                                CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                    AND CL.CLASS_YEAR = ?CLASS_YEAR
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CL.PROGRAMME_GROUP_ID
                                    INNER JOIN
                                SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                                    INNER JOIN
                                SUP_SHIFT AS SS ON SS.SHIFT_ID = PG.SHIFT
                                    INNER JOIN
                                CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                            WHERE
                                SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    AND PG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                            ORDER BY SP.ROLL_NO;";
                        break;
                    }
                case FeeSQLCommands.FetchClassList:
                    {
                        query = @"SELECT 
                                        CLASS_ID, CLASS_NAME
                                    FROM
                                        cp_classes
                                    WHERE
                                        IS_DELETED != 1 AND CLASS_TYPE = 2;";
                        break;
                    }
                #region Fee due Details for Student

                case FeeSQLCommands.FetchStudentDetailsFroFeeDues:
                    {
                        query = @"SELECT 
                                        SPI.STUDENT_ID,
                                        SPI.ROLL_NO,
                                        CC.CLASS_ID
                                    FROM
                                        STU_PERSONAL_INFO AS SPI
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        sc.ACADEMIC_YEAR = ?AC_YEAR
                                            AND sc.IS_DELETED != 1
                                    GROUP BY STUDENT_ID order by ROLL_NO;";
                        break;
                    }
                case FeeSQLCommands.FetchStudentDetailsFroFeeDuesByRollNo:
                    {
                        query = @"SELECT 
                                        SPI.STUDENT_ID, SPI.ROLL_NO, CC.CLASS_ID
                                    FROM
                                        STU_PERSONAL_INFO AS SPI
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        SPI.ROLL_NO = ?ROLL_NO
                                            AND sc.IS_DELETED != 1
                                    GROUP BY STUDENT_ID
                                    ORDER BY ROLL_NO;";
                        break;
                    }
                case FeeSQLCommands.FetchPassengerDetailsByRollNo:
                    {
                        query = @"SELECT 
                                        SPI.STUDENT_ID, 
                                        SPI.ROLL_NO,
                                        5 AS ROLE
                                    FROM
                                        STU_PERSONAL_INFO AS SPI
       
                                    WHERE
                                        SPI.ROLL_NO = ?ROLL_NO
                                            AND spi.IS_DELETED != 1
                                    GROUP BY STUDENT_ID 
                             UNION SELECT 
                                        SPI.STAFF_ID,
                                        SPI.STAFF_CODE,
                                        4 AS ROLE
                                    FROM
                                        Stf_PERSONAL_INFO AS SPI
                                    WHERE
                                        SPI.STAFF_CODE = ?ROLL_NO
                                            AND spi.IS_DELETED != 1
                                    GROUP BY STAFF_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeDueInfoforUpdateDueList:
                    {
                        query = @"SELECT 
                                    sfd.STUDENT_ID,
                                    spi.ROLL_NO,
                                    sfd.STU_DUE_ID
                                FROM
                                    STU_FEE_DUE_INFO SFD
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SFD.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID  
                                        AND SFD.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SPI.IS_DELETED != 1
                                        -- AND SPI.IS_LEFT != 1
                                        AND SFD.IS_DELETED != 1; ";
                        break;
                    }
                case FeeSQLCommands.SaveStuFeeDueInfo:
                    {
                        query = @"INSERT INTO STU_FEE_DUE_INFO(
                                                          STUDENT_ID,
                                                          ACADEMIC_YEAR,
                                                          TERMFEE1,
                                                          TERMFEE1PAID,
                                                          TERMFEE2,
                                                          TERMFEE2PAID,
                                                          EXAMFEE1, 
                                                          EXAMFEE1PAID,
                                                          EXAMFEE2, 
                                                          EXAMFEE2PAID, 
                                                          BUSFEE, 
                                                          BUSFEEPAID, 
                                                          HOSTELFEE, 
                                                          HOSTELFEEPAID, 
                                                          FREQUENCY_ID, 
                                                          ENTRY_ID, 
                                                          ENTRY_DATE)
                                                        VALUES(
                                                          ?STUDENT_ID,
                                                          ?ACADEMIC_YEAR,
                                                          ?TERMFEE1,
                                                          ?TERMFEE1PAID,
                                                          ?TERMFEE2,
                                                          ?TERMFEE2PAID,
                                                          ?EXAMFEE1, 
                                                          ?EXAMFEE1PAID,
                                                          ?EXAMFEE2, 
                                                          ?EXAMFEE2PAID,
                                                          ?BUSFEE, 
                                                          ?BUSFEEDUE, 
                                                          ?HOSTELFEE, 
                                                          ?HOSTELFEEPAID, 
                                                          ?FREQUENCY_ID, 
                                                          ?ENTRY_ID, 
                                                          ?ENTRY_DATE
                                                          );";
                        break;
                    }
                case FeeSQLCommands.UpdateStuFeeDueInfo:
                    {
                        query = @"UPDATE  STU_FEE_DUE_INFO SET 
                                                          STUDENT_ID =?STUDENT_ID,
                                                          ACADEMIC_YEAR =?ACADEMIC_YEAR,
                                                          TERMFEE1 =?TERMFEE1,
                                                          TERMFEE1PAID =?TERMFEE1PAID,
                                                          TERMFEE2 =?TERMFEE2,
                                                          TERMFEE2PAID =?TERMFEE2PAID,
                                                          EXAMFEE1 =?EXAMFEE1, 
                                                          EXAMFEE1PAID =?EXAMFEE1PAID, 
                                                          EXAMFEE2 =?EXAMFEE2, 
                                                          EXAMFEE2PAID =?EXAMFEE2PAID, 
                                                          BUSFEE =?BUSFEE, 
                                                          BUSFEEPAID =?BUSFEEPAID, 
                                                          HOSTELFEE =?HOSTELFEE, 
                                                          HOSTELFEEPAID =?HOSTELFEEPAID, 
                                                          FREQUENCY_ID =?FREQUENCY_ID, 
                                                          ENTRY_ID =?ENTRY_ID, 
                                                          ENTRY_DATE=?ENTRY_DATE
                                                      where
                                                          STU_DUE_ID =?STU_DUE_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchStudentFeeDueDetails:
                    {
                        query = @"SELECT 
                                    sfd.STUDENT_ID,
                                    spi.FIRST_NAME,
                                    spi.ROLL_NO,
                                    STU_DUE_ID,
                                    cc.CLASS_NAME,
                                    cc.CLASS_ID,
                                    sfd.TERMFEE1,
                                    TERMFEE1PAID,
                                    IF(TERMFEE1 IS NULL,
                                        TERMFEE1,
                                        SUM(IFNULL(TERMFEE1, 0) - IFNULL(TERMFEE1PAID, 0))) AS TERMFEE1DUE,
                                    sfd.TERMFEE2,
                                    sfd.TERMFEE2PAID,
                                    IF(TERMFEE2 IS NULL,
                                        TERMFEE2,
                                        SUM(IFNULL(TERMFEE2, 0)) - SUM(IFNULL(TERMFEE2PAID, 0))) AS TERMFEE2DUE,
                                    sfd.EXAMFEE1,
                                    EXAMFEE1PAID,
                                    IF(EXAMFEE1 IS NULL,
                                        EXAMFEE1,
                                        SUM(IFNULL(EXAMFEE1, 0) - IFNULL(EXAMFEE1PAID, 0))) AS EXAMFEE1DUE,
                                    sfd.EXAMFEE2,
                                    EXAMFEE2PAID,
                                    IF(EXAMFEE2 IS NULL,
                                        EXAMFEE2,
                                        SUM(IFNULL(EXAMFEE2, 0) - IFNULL(EXAMFEE2PAID, 0))) AS EXAMFEE2DUE,
                                    B_CREDIT as BUSFEE,
                                   B_DEBIT as  BUSFEEPAID,
                                    IF(B_CREDIT IS NULL,
                                        B_CREDIT,
                                        SUM(IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0))) AS BUSFEEDUE,
                                    IF(HFSA.CREDIT IS NULL,
                                        HFSA.CREDIT,
                                        SUM(IFNULL(HFSA.CREDIT, '0'))) AS HOSTELFEE,
                                    IF(HFSA.DEBIT IS NULL,
                                        HFSA.DEBIT,
                                        SUM(IFNULL(HFSA.DEBIT, '0'))) HOSTELFEEPAID,
                                    IF(HFSA.CREDIT IS NULL,
                                        NULL,
                                        SUM(IFNULL(HFSA.CREDIT, '0')) - SUM(IFNULL(HFSA.DEBIT, '0'))) AS HOSTELFEEDUE,
                                    SUM(CASE
											WHEN sfd.TERMFEE1 IS NULL THEN 0
											ELSE IF(IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0) < 0,
												0,
												IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0))
										END + CASE
											WHEN sfd.TERMFEE2 IS NULL THEN 0
											ELSE IF(IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0)<0,
												0,
												IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0))
										END + CASE
											WHEN sfd.EXAMFEE1 IS NULL THEN 0
											ELSE IF(IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0)<0,
												0,
												IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0))
										END + CASE
											WHEN sfd.EXAMFEE2 IS NULL THEN 0
											ELSE IF(IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0)<0,
												0,
												IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0))
										END + CASE
											WHEN B_CREDIT IS NULL THEN 0
											ELSE IF(IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0)<0,
												0,
												IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0))
										END + CASE
											WHEN HFSA.CREDIT IS NULL THEN 0
											ELSE IF(IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0)<0,
												0,
												IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0))
										END) AS TOTAL_DUE_AMOUNT,
                                    SFD.ENTRY_ID,
                                    SFD.ENTRY_DATE
                                FROM
                                    STU_PERSONAL_INFO AS SPI
                                        INNER JOIN
                                    STU_FEE_DUE_INFO AS SFD ON SPI.STUDENT_ID = SFD.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = SFD.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        LEFT JOIN
                                    (SELECT 
                                        STUDENT_ID,
                                            SUM(IFNULL(CREDIT, '0')) AS CREDIT,
                                            SUM(IFNULL(DEBIT, '0')) AS DEBIT
                                    FROM
                                        HOSTEL_FEE_STUDENT_ACCOUNT AS hf
                                    WHERE
                                        hf.IS_DELETED != 1
                                            AND hf.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY STUDENT_ID) AS HFSA ON HFSA.STUDENT_ID = SPI.STUDENT_ID
                                    left join (SELECT 
                                        STUDENT_ID,
                                            SUM(IFNULL(CREDIT, '0')) AS B_CREDIT,
                                            SUM(IFNULL(DEBIT, '0')) AS B_DEBIT
                                    FROM
                                        BUS_FEE_STUDENT_ACCOUNT AS Bf
                                    WHERE
                                        Bf.IS_DELETED != 1
                                            AND Bf.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY STUDENT_ID) AS BFSA ON BFSA.STUDENT_ID = SPI.STUDENT_ID
                                WHERE
                                    SFD.ACADEMIC_YEAR = ?AC_YEAR AND SC.CLASS_ID IN (?CLASS_ID)
                                        AND SC.IS_DELETED != 1
                                        AND SFD.IS_DELETED != 1
                                        AND SPI.IS_DELETED != 1
                                GROUP BY SFD.STUDENT_ID , sfd.ACADEMIC_YEAR
                                ORDER BY ROLL_NO , SC.CLASS_ID;";
                        break;

                    }
                case FeeSQLCommands.FetchStudentFeeDueDetailsClasswise:
                    {
                        query = @"SELECT 
                                        SPI.STUDENT_ID,
                                        SPI.FIRST_NAME,
                                        SPI.ROLL_NO,
                                        CUR.STU_DUE_ID,
                                        CC.CLASS_ID,
                                        CC.CLASS_NAME,

                                        CUR.TERMFEE1,
                                        CUR.TERMFEE1PAID,
                                        GREATEST(
                                            IFNULL(CUR.TERMFEE1, 0) - IFNULL(CUR.TERMFEE1PAID, 0),
                                            0
                                        ) AS TERMFEE1DUE,

                                        CUR.TERMFEE2,
                                        CUR.TERMFEE2PAID,
                                        GREATEST(
                                            IFNULL(CUR.TERMFEE2, 0) - IFNULL(CUR.TERMFEE2PAID, 0),
                                            0
                                        ) AS TERMFEE2DUE,

                                        IFNULL(EF1.EXAMFEE1, 0) AS EXAMFEE1,
                                        IFNULL(EF1.EXAMFEE1PAID, 0) AS EXAMFEE1PAID,
                                        GREATEST(
                                            IFNULL(EF1.EXAMFEE1, 0) - IFNULL(EF1.EXAMFEE1PAID, 0),
                                            0
                                        ) AS EXAMFEE1DUE,

                                        IFNULL(EF2.EXAMFEE2, 0) AS EXAMFEE2,
                                        IFNULL(EF2.EXAMFEE2PAID, 0) AS EXAMFEE2PAID,
                                        GREATEST(
                                            IFNULL(EF2.EXAMFEE2, 0) - IFNULL(EF2.EXAMFEE2PAID, 0),
                                            0
                                        ) AS EXAMFEE2DUE,

                                        /* Previous year due */
                                        IFNULL(ARR.ARREAR_DUE, 0) AS PREVIOUS_YEAR_TERM_DUE,

                                        IFNULL(HF.HOSTEL_FEE, 0) AS HOSTELFEE,
                                        IFNULL(HF.HOSTEL_FEE_PAID, 0) AS HOSTELFEEPAID,
                                        GREATEST(
                                            IFNULL(HF.HOSTEL_FEE, 0) - IFNULL(HF.HOSTEL_FEE_PAID, 0),
                                            0
                                        ) AS HOSTELFEEDUE,

                                        IFNULL(BF.BUS_FEE, 0) AS BUSFEE,
                                        IFNULL(BF.BUS_FEE_PAID, 0) AS BUSFEEPAID,
                                        GREATEST(
                                            IFNULL(BF.BUS_FEE, 0) - IFNULL(BF.BUS_FEE_PAID, 0),
                                            0
                                        ) AS BUSFEEDUE,

                                        (
                                            IFNULL(ARR.ARREAR_DUE, 0)
                                            + GREATEST(IFNULL(CUR.TERMFEE1, 0) - IFNULL(CUR.TERMFEE1PAID, 0), 0)
                                            + GREATEST(IFNULL(CUR.TERMFEE2, 0) - IFNULL(CUR.TERMFEE2PAID, 0), 0)
                                            + GREATEST(IFNULL(EF1.EXAMFEE1, 0) - IFNULL(EF1.EXAMFEE1PAID, 0), 0)
                                            + GREATEST(IFNULL(EF2.EXAMFEE2, 0) - IFNULL(EF2.EXAMFEE2PAID, 0), 0)
                                            + GREATEST(IFNULL(HF.HOSTEL_FEE, 0) - IFNULL(HF.HOSTEL_FEE_PAID, 0), 0)
                                            + GREATEST(IFNULL(BF.BUS_FEE, 0) - IFNULL(BF.BUS_FEE_PAID, 0), 0)
                                        ) AS TOTAL_DUE_AMOUNT,

                                        CUR.ENTRY_ID,
                                        CUR.ENTRY_DATE

                                    FROM STU_PERSONAL_INFO SPI

                                    INNER JOIN STU_CLASS SC 
                                        ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.IS_DELETED <> 1
                                        AND SC.CLASS_ID IN (?CLASS_ID)

                                    INNER JOIN CP_CLASSES CC 
                                        ON CC.CLASS_ID = SC.CLASS_ID

                                    LEFT JOIN STU_FEE_DUE_INFO CUR 
                                        ON CUR.STUDENT_ID = SPI.STUDENT_ID
                                        AND CUR.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CUR.IS_DELETED <> 1

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            SFD.STUDENT_ID,
                                            SUM(
                                                 GREATEST(IFNULL(SFD.TERMFEE1, 0) - IFNULL(SFD.TERMFEE1PAID, 0), 0)
                                                + GREATEST(IFNULL(SFD.TERMFEE2, 0) - IFNULL(SFD.TERMFEE2PAID, 0), 0)
                                            ) AS ARREAR_DUE
                                        FROM STU_FEE_DUE_INFO SFD
                                        INNER JOIN STU_CLASS SC_ARR ON SC_ARR.STUDENT_ID = SFD.STUDENT_ID AND SC_ARR.CLASS_ID IN (?CLASS_ID) AND SC_ARR.IS_DELETED <> 1
                                        WHERE
                                            SFD.ACADEMIC_YEAR < ?AC_YEAR
                                            AND SFD.IS_DELETED <> 1
                                        GROUP BY SFD.STUDENT_ID
                                    ) ARR 
                                        ON ARR.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,
                                            SUM(IFNULL(FSA.CREDIT, 0)) AS BUS_FEE,
                                            SUM(IFNULL(FSA.DEBIT, 0)) AS BUS_FEE_PAID
                                        FROM FEE_STUDENT_ACCOUNT FSA
                                        INNER JOIN STU_CLASS SC_BF ON SC_BF.STUDENT_ID = FSA.STUDENT_ID AND SC_BF.CLASS_ID IN (?CLASS_ID) AND SC_BF.IS_DELETED <> 1
                                        INNER JOIN FEE_FREQUENCY_FEE_MODE FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                        WHERE
                                            FFF.FEE_MODE = '8'
                                            AND FSA.IS_DELETED <> 1
                                        GROUP BY FSA.STUDENT_ID
                                    ) BF 
                                        ON BF.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,
                                            SUM(IFNULL(FSA.CREDIT, 0)) AS HOSTEL_FEE,
                                            SUM(IFNULL(FSA.DEBIT, 0)) AS HOSTEL_FEE_PAID
                                        FROM FEE_STUDENT_ACCOUNT FSA
                                        INNER JOIN STU_CLASS SC_HF ON SC_HF.STUDENT_ID = FSA.STUDENT_ID AND SC_HF.CLASS_ID IN (?CLASS_ID) AND SC_HF.IS_DELETED <> 1
                                        INNER JOIN FEE_FREQUENCY_FEE_MODE FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                        WHERE
                                            FFF.FEE_MODE = '4'
                                            AND FSA.IS_DELETED <> 1
                                        GROUP BY FSA.STUDENT_ID
                                    ) HF 
                                        ON HF.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,
                                            SUM(IFNULL(FSA.CREDIT, 0)) AS EXAMFEE1,
                                            SUM(IFNULL(FSA.DEBIT, 0)) AS EXAMFEE1PAID
                                        FROM FEE_STUDENT_ACCOUNT FSA
                                        INNER JOIN STU_CLASS SC_EF1 ON SC_EF1.STUDENT_ID = FSA.STUDENT_ID AND SC_EF1.CLASS_ID IN (?CLASS_ID) AND SC_EF1.ACADEMIC_YEAR = ?AC_YEAR AND SC_EF1.IS_DELETED <> 1
                                        INNER JOIN FEE_FREQUENCY_FEE_MODE FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                        INNER JOIN SUP_FEE_FREQUENCY SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        WHERE
                                            FFF.FEE_MODE = '1'
                                            AND SFF.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SFF.SEMESTER_TYPE = 1
                                            AND FSA.IS_DELETED <> 1
                                            AND SFF.IS_DELETED <> 1
                                        GROUP BY FSA.STUDENT_ID
                                    ) EF1 
                                        ON EF1.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,
                                            SUM(IFNULL(FSA.CREDIT, 0)) AS EXAMFEE2,
                                            SUM(IFNULL(FSA.DEBIT, 0)) AS EXAMFEE2PAID
                                        FROM FEE_STUDENT_ACCOUNT FSA
                                        INNER JOIN STU_CLASS SC_EF2 ON SC_EF2.STUDENT_ID = FSA.STUDENT_ID AND SC_EF2.CLASS_ID IN (?CLASS_ID) AND SC_EF2.ACADEMIC_YEAR = ?AC_YEAR AND SC_EF2.IS_DELETED <> 1
                                        INNER JOIN FEE_FREQUENCY_FEE_MODE FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                        INNER JOIN SUP_FEE_FREQUENCY SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        WHERE
                                            FFF.FEE_MODE = '1'
                                            AND SFF.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SFF.SEMESTER_TYPE = 2
                                            AND FSA.IS_DELETED <> 1
                                            AND SFF.IS_DELETED <> 1
                                        GROUP BY FSA.STUDENT_ID
                                    ) EF2 
                                        ON EF2.STUDENT_ID = SPI.STUDENT_ID

                                    WHERE
                                        SPI.IS_DELETED <> 1

                                    ORDER BY 
                                        CC.CLASS_ID,
                                        SPI.ROLL_NO;";
                        break;
                    }
                case FeeSQLCommands.FetchAllFeeDueDetails:
                    {
                        query = @"SELECT 
                                        SPI.STUDENT_ID,
                                        SPI.FIRST_NAME,
                                        SPI.ROLL_NO,
                                        CUR.STU_DUE_ID,
                                        CC.CLASS_ID,
                                        CC.CLASS_NAME,

                                        CUR.TERMFEE1,
                                        CUR.TERMFEE1PAID,
                                        GREATEST(
                                            IFNULL(CUR.TERMFEE1, 0) - IFNULL(CUR.TERMFEE1PAID, 0),
                                            0
                                        ) AS TERMFEE1DUE,

                                        CUR.TERMFEE2,
                                        CUR.TERMFEE2PAID,
                                        GREATEST(
                                            IFNULL(CUR.TERMFEE2, 0) - IFNULL(CUR.TERMFEE2PAID, 0),
                                            0
                                        ) AS TERMFEE2DUE,

                                        CUR.EXAMFEE1,
                                        CUR.EXAMFEE1PAID,
                                        GREATEST(
                                            IFNULL(CUR.EXAMFEE1, 0) - IFNULL(CUR.EXAMFEE1PAID, 0),
                                            0
                                        ) AS EXAMFEE1DUE,

                                        CUR.EXAMFEE2,
                                        CUR.EXAMFEE2PAID,
                                        GREATEST(
                                            IFNULL(CUR.EXAMFEE2, 0) - IFNULL(CUR.EXAMFEE2PAID, 0),
                                            0
                                        ) AS EXAMFEE2DUE,

                                        /* Previous year due */
                                        IFNULL(ARR.ARREAR_DUE, 0) AS PREVIOUS_YEAR_TERM_DUE,

                                       
                                        IFNULL(HF.HOSTEL_FEE, 0) AS HOSTELFEE,

                                        IFNULL(HF.HOSTEL_FEE_PAID, 0) AS HOSTELFEEPAID,

                                        GREATEST(
                                            IFNULL(HF.HOSTEL_FEE, 0)
                                            - IFNULL(HF.HOSTEL_FEE_PAID, 0),
                                            0
                                        ) AS HOSTELFEEDUE,


                                        IFNULL(BF.BUS_FEE, 0) AS BUSFEE,

                                        IFNULL(BF.BUS_FEE_PAID, 0) AS BUSFEEPAID,

                                        GREATEST(
                                            IFNULL(BF.BUS_FEE, 0)
                                            - IFNULL(BF.BUS_FEE_PAID, 0),
                                            0
                                        ) AS BUSFEEDUE,

                  

                                        (
                                            IFNULL(ARR.ARREAR_DUE, 0)

                                            + GREATEST(
                                                IFNULL(CUR.TERMFEE1, 0)
                                                - IFNULL(CUR.TERMFEE1PAID, 0),
                                                0
                                            )

                                            + GREATEST(
                                                IFNULL(CUR.TERMFEE2, 0)
                                                - IFNULL(CUR.TERMFEE2PAID, 0),
                                                0
                                            )

                                            + GREATEST(
                                                IFNULL(CUR.EXAMFEE1, 0)
                                                - IFNULL(CUR.EXAMFEE1PAID, 0),
                                                0
                                            )

                                            + GREATEST(
                                                IFNULL(CUR.EXAMFEE2, 0)
                                                - IFNULL(CUR.EXAMFEE2PAID, 0),
                                                0
                                            )

                                            + GREATEST(
                                                IFNULL(HF.HOSTEL_FEE, 0)
                                                - IFNULL(HF.HOSTEL_FEE_PAID, 0),
                                                0
                                            )

                                            + GREATEST(
                                                IFNULL(BF.BUS_FEE, 0)
                                                - IFNULL(BF.BUS_FEE_PAID, 0),
                                                0
                                            )

                                        ) AS TOTAL_DUE_AMOUNT,

                                        CUR.ENTRY_ID,
                                        CUR.ENTRY_DATE

                                    FROM STU_PERSONAL_INFO SPI

                                    INNER JOIN STU_CLASS SC 
                                        ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.IS_DELETED <> 1

                                    INNER JOIN CP_CLASSES CC 
                                        ON CC.CLASS_ID = SC.CLASS_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            *
                                        FROM STU_FEE_DUE_INFO
                                        WHERE
                                            ACADEMIC_YEAR = ?AC_YEAR
                                            AND IS_DELETED <> 1
                                    ) CUR 
                                        ON CUR.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            STUDENT_ID,

                                            SUM(
                                                GREATEST(
                                                    IFNULL(TERMFEE1, 0)
                                                    - IFNULL(TERMFEE1PAID, 0),
                                                    0
                                                )
                                                +
                                                GREATEST(
                                                    IFNULL(TERMFEE2, 0)
                                                    - IFNULL(TERMFEE2PAID, 0),
                                                    0
                                                )
                                            ) AS ARREAR_DUE

                                        FROM STU_FEE_DUE_INFO

                                        WHERE
                                            ACADEMIC_YEAR < ?AC_YEAR
                                            AND IS_DELETED <> 1

                                        GROUP BY STUDENT_ID

                                    ) ARR 
                                        ON ARR.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,

                                            SUM(IFNULL(FSA.CREDIT, 0)) AS BUS_FEE,

                                            SUM(IFNULL(FSA.DEBIT, 0)) AS BUS_FEE_PAID

                                        FROM FEE_STUDENT_ACCOUNT FSA

                                        INNER JOIN FEE_FREQUENCY_FEE_MODE FFF
                                            ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID

                                        INNER JOIN SUP_FEE_FREQUENCY SFF
                                            ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID

                                        INNER JOIN SUP_FREQUENCY_TYPE SFT
                                            ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE

                                        WHERE
                                            SFT.FREQUENCY_TYPE_ID = 8
                                            AND FSA.IS_DELETED <> 1

                                        GROUP BY FSA.STUDENT_ID

                                    ) BF 
                                        ON BF.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,

                                            SUM(IFNULL(FSA.CREDIT, 0)) AS HOSTEL_FEE,

                                            SUM(IFNULL(FSA.DEBIT, 0)) AS HOSTEL_FEE_PAID

                                        FROM FEE_STUDENT_ACCOUNT FSA

                                        INNER JOIN FEE_FREQUENCY_FEE_MODE FFF
                                            ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID

                                        INNER JOIN SUP_FEE_FREQUENCY SFF
                                            ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID

                                        INNER JOIN SUP_FREQUENCY_TYPE SFT
                                            ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE

                                        WHERE
                                            SFT.FREQUENCY_TYPE_ID = 4
                                            AND FSA.IS_DELETED <> 1

                                        GROUP BY FSA.STUDENT_ID

                                    ) HF 
                                        ON HF.STUDENT_ID = SPI.STUDENT_ID

                                    WHERE
                                        SPI.IS_DELETED <> 1
                                        AND SC.ACADEMIC_YEAR =?AC_YEAR

                                    ORDER BY 
                                        CC.CLASS_ID,
                                        SPI.ROLL_NO;";
                        break;
                    }

                //query = @"SELECT 
                //                    sfd.STUDENT_ID,
                //                    spi.FIRST_NAME,
                //                    spi.ROLL_NO,
                //                    STU_DUE_ID,
                //                    cc.CLASS_NAME,
                //                    cc.CLASS_ID,
                //                    sfd.TERMFEE1,
                //                    TERMFEE1PAID,
                //                    IF(TERMFEE1 IS NULL,
                //                        TERMFEE1,
                //                        SUM(IFNULL(TERMFEE1, 0) - IFNULL(TERMFEE1PAID, 0))) AS TERMFEE1DUE,
                //                    sfd.TERMFEE2,
                //                    sfd.TERMFEE2PAID,
                //                    IF(TERMFEE2 IS NULL,
                //                        TERMFEE2,
                //                        SUM(IFNULL(TERMFEE2, 0)) - SUM(IFNULL(TERMFEE2PAID, 0))) AS TERMFEE2DUE,
                //                    sfd.EXAMFEE1,
                //                    EXAMFEE1PAID,
                //                    IF(EXAMFEE1 IS NULL,
                //                        EXAMFEE1,
                //                        SUM(IFNULL(EXAMFEE1, 0) - IFNULL(EXAMFEE1PAID, 0))) AS EXAMFEE1DUE,
                //                    sfd.EXAMFEE2,
                //                    EXAMFEE2PAID,
                //                    IF(EXAMFEE2 IS NULL,
                //                        EXAMFEE2,
                //                        SUM(IFNULL(EXAMFEE2, 0) - IFNULL(EXAMFEE2PAID, 0))) AS EXAMFEE2DUE,
                //                    B_CREDIT AS BUSFEE,
                //                    B_DEBIT AS BUSFEEPAID,
                //                    IF(B_CREDIT IS NULL,
                //                        B_CREDIT,
                //                        SUM(IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0))) AS BUSFEEDUE,
                //                    IF(HFSA.CREDIT IS NULL,
                //                        HFSA.CREDIT,
                //                        SUM(IFNULL(HFSA.CREDIT, '0'))) AS HOSTELFEE,
                //                    IF(HFSA.DEBIT IS NULL,
                //                        HFSA.DEBIT,
                //                        SUM(IFNULL(HFSA.DEBIT, '0'))) HOSTELFEEPAID,
                //                    IF(HFSA.CREDIT IS NULL,
                //                        NULL,
                //                        SUM(IFNULL(HFSA.CREDIT, '0')) - SUM(IFNULL(HFSA.DEBIT, '0'))) AS HOSTELFEEDUE,
                //                    SUM(CASE
                //                        WHEN sfd.TERMFEE1 IS NULL THEN 0
                //                        ELSE IF(IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0) < 0,
                //                            0,
                //                            IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0))
                //                    END + CASE
                //                        WHEN sfd.TERMFEE2 IS NULL THEN 0
                //                        ELSE IF(IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0)<0,
                //                            0,
                //                            IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0))
                //                    END + CASE
                //                        WHEN sfd.EXAMFEE1 IS NULL THEN 0
                //                        ELSE IF(IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0)<0,
                //                            0,
                //                            IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0))
                //                    END + CASE
                //                        WHEN sfd.EXAMFEE2 IS NULL THEN 0
                //                        ELSE IF(IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0)<0,
                //                            0,
                //                            IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0))
                //                    END + CASE
                //                        WHEN B_CREDIT IS NULL THEN 0
                //                        ELSE IF(IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0)<0,
                //                            0,
                //                            IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0))
                //                    END + CASE
                //                        WHEN HFSA.CREDIT IS NULL THEN 0
                //                        ELSE IF(IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0)<0,
                //                            0,
                //                            IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0))
                //                    END) AS TOTAL_DUE_AMOUNT,
                //                    sfd.ENTRY_ID,
                //                    sfd.ENTRY_DATE
                //                FROM
                //                    STU_PERSONAL_INFO AS SPI
                //                        INNER JOIN
                //                    STU_FEE_DUE_INFO AS SFD ON SPI.STUDENT_ID = SFD.STUDENT_ID
                //                        INNER JOIN
                //                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                //                        AND SC.ACADEMIC_YEAR = SFD.ACADEMIC_YEAR
                //                        INNER JOIN
                //                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                //                        INNER JOIN
                //                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                //                        LEFT JOIN
                //                    (SELECT 
                //                        STUDENT_ID,
                //                            SUM(IFNULL(CREDIT, '0')) AS CREDIT,
                //                            SUM(IFNULL(DEBIT, '0')) AS DEBIT
                //                    FROM
                //                        HOSTEL_FEE_STUDENT_ACCOUNT AS hf
                //                    WHERE
                //                        hf.IS_DELETED != 1
                //                            AND hf.ACADEMIC_YEAR = ?AC_YEAR
                //                    GROUP BY STUDENT_ID) AS HFSA ON HFSA.STUDENT_ID = SPI.STUDENT_ID
                //                        LEFT JOIN
                //                    (SELECT 
                //                        STUDENT_ID,
                //                            SUM(IFNULL(CREDIT, '0')) AS B_CREDIT,
                //                            SUM(IFNULL(DEBIT, '0')) AS B_DEBIT
                //                    FROM
                //                        BUS_FEE_STUDENT_ACCOUNT AS Bf
                //                    WHERE
                //                        Bf.IS_DELETED != 1
                //                            AND Bf.ACADEMIC_YEAR = ?AC_YEAR
                //                    GROUP BY STUDENT_ID) AS BFSA ON BFSA.STUDENT_ID = SPI.STUDENT_ID
                //                WHERE
                //                    SFD.ACADEMIC_YEAR = ?AC_YEAR
                //                        AND SC.IS_DELETED != 1
                //                        AND SFD.IS_DELETED != 1
                //                        AND SPI.IS_DELETED != 1
                //                GROUP BY SFD.STUDENT_ID , sfd.ACADEMIC_YEAR
                //                ORDER BY ROLL_NO , SC.CLASS_ID;";
                //    break;
                //}
                case FeeSQLCommands.FetchAllFeeDueDetailsWitharrearAmmount:
                    {
                        query = @"SELECT 
                                        SPI.STUDENT_ID,
                                        SPI.FIRST_NAME,
                                        SPI.ROLL_NO,
                                        CUR.STU_DUE_ID,
                                        CC.CLASS_ID,
                                        CC.CLASS_NAME,

                                        CUR.TERMFEE1,
                                        CUR.TERMFEE1PAID,
                                        GREATEST(
                                            IFNULL(CUR.TERMFEE1, 0) - IFNULL(CUR.TERMFEE1PAID, 0),
                                            0
                                        ) AS TERMFEE1DUE,

                                        CUR.TERMFEE2,
                                        CUR.TERMFEE2PAID,
                                        GREATEST(
                                            IFNULL(CUR.TERMFEE2, 0) - IFNULL(CUR.TERMFEE2PAID, 0),
                                            0
                                        ) AS TERMFEE2DUE,

                                        IFNULL(EF1.EXAMFEE1, 0) AS EXAMFEE1,
                                        IFNULL(EF1.EXAMFEE1PAID, 0) AS EXAMFEE1PAID,
                                        GREATEST(
                                            IFNULL(EF1.EXAMFEE1, 0) - IFNULL(EF1.EXAMFEE1PAID, 0),
                                            0
                                        ) AS EXAMFEE1DUE,

                                        IFNULL(EF2.EXAMFEE2, 0) AS EXAMFEE2,
                                        IFNULL(EF2.EXAMFEE2PAID, 0) AS EXAMFEE2PAID,
                                        GREATEST(
                                            IFNULL(EF2.EXAMFEE2, 0) - IFNULL(EF2.EXAMFEE2PAID, 0),
                                            0
                                        ) AS EXAMFEE2DUE,

                                        /* Previous year due */
                                        IFNULL(ARR.ARREAR_DUE, 0) AS PREVIOUS_YEAR_TERM_DUE,

                                        IFNULL(HF.HOSTEL_FEE, 0) AS HOSTELFEE,
                                        IFNULL(HF.HOSTEL_FEE_PAID, 0) AS HOSTELFEEPAID,
                                        GREATEST(
                                            IFNULL(HF.HOSTEL_FEE, 0) - IFNULL(HF.HOSTEL_FEE_PAID, 0),
                                            0
                                        ) AS HOSTELFEEDUE,

                                        IFNULL(BF.BUS_FEE, 0) AS BUSFEE,
                                        IFNULL(BF.BUS_FEE_PAID, 0) AS BUSFEEPAID,
                                        GREATEST(
                                            IFNULL(BF.BUS_FEE, 0) - IFNULL(BF.BUS_FEE_PAID, 0),
                                            0
                                        ) AS BUSFEEDUE,

                                        (
                                            IFNULL(ARR.ARREAR_DUE, 0)
                                            + GREATEST(IFNULL(CUR.TERMFEE1, 0) - IFNULL(CUR.TERMFEE1PAID, 0), 0)
                                            + GREATEST(IFNULL(CUR.TERMFEE2, 0) - IFNULL(CUR.TERMFEE2PAID, 0), 0)
                                            + GREATEST(IFNULL(EF1.EXAMFEE1, 0) - IFNULL(EF1.EXAMFEE1PAID, 0), 0)
                                            + GREATEST(IFNULL(EF2.EXAMFEE2, 0) - IFNULL(EF2.EXAMFEE2PAID, 0), 0)
                                            + GREATEST(IFNULL(HF.HOSTEL_FEE, 0) - IFNULL(HF.HOSTEL_FEE_PAID, 0), 0)
                                            + GREATEST(IFNULL(BF.BUS_FEE, 0) - IFNULL(BF.BUS_FEE_PAID, 0), 0)
                                        ) AS TOTAL_DUE_AMOUNT,

                                        CUR.ENTRY_ID,
                                        CUR.ENTRY_DATE

                                    FROM STU_PERSONAL_INFO SPI

                                    INNER JOIN STU_CLASS SC 
                                        ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.IS_DELETED <> 1

                                    INNER JOIN CP_CLASSES CC 
                                        ON CC.CLASS_ID = SC.CLASS_ID

                                    LEFT JOIN STU_FEE_DUE_INFO CUR 
                                        ON CUR.STUDENT_ID = SPI.STUDENT_ID
                                        AND CUR.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CUR.IS_DELETED <> 1

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            SFD.STUDENT_ID,
                                            SUM(
                                                GREATEST(IFNULL(SFD.TERMFEE1, 0) - IFNULL(SFD.TERMFEE1PAID, 0), 0)
                                                + GREATEST(IFNULL(SFD.TERMFEE2, 0) - IFNULL(SFD.TERMFEE2PAID, 0), 0)
                                            ) AS ARREAR_DUE
                                        FROM STU_FEE_DUE_INFO SFD
                                        INNER JOIN STU_CLASS SC_ARR ON SC_ARR.STUDENT_ID = SFD.STUDENT_ID AND SC_ARR.ACADEMIC_YEAR = ?AC_YEAR AND SC_ARR.IS_DELETED <> 1
                                        WHERE
                                            SFD.ACADEMIC_YEAR < ?AC_YEAR
                                            AND SFD.IS_DELETED <> 1
                                        GROUP BY SFD.STUDENT_ID
                                    ) ARR 
                                        ON ARR.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,
                                            SUM(IFNULL(FSA.CREDIT, 0)) AS BUS_FEE,
                                            SUM(IFNULL(FSA.DEBIT, 0)) AS BUS_FEE_PAID
                                        FROM FEE_STUDENT_ACCOUNT FSA
                                        INNER JOIN STU_CLASS SC_BF ON SC_BF.STUDENT_ID = FSA.STUDENT_ID AND SC_BF.ACADEMIC_YEAR = ?AC_YEAR AND SC_BF.IS_DELETED <> 1
                                        INNER JOIN FEE_FREQUENCY_FEE_MODE FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                        WHERE
                                            FFF.FEE_MODE = '8'
                                            AND FSA.IS_DELETED <> 1
                                        GROUP BY FSA.STUDENT_ID
                                    ) BF 
                                        ON BF.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,
                                            SUM(IFNULL(FSA.CREDIT, 0)) AS HOSTEL_FEE,
                                            SUM(IFNULL(FSA.DEBIT, 0)) AS HOSTEL_FEE_PAID
                                        FROM FEE_STUDENT_ACCOUNT FSA
                                        INNER JOIN STU_CLASS SC_HF ON SC_HF.STUDENT_ID = FSA.STUDENT_ID AND SC_HF.ACADEMIC_YEAR = ?AC_YEAR AND SC_HF.IS_DELETED <> 1
                                        INNER JOIN FEE_FREQUENCY_FEE_MODE FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                        WHERE
                                            FFF.FEE_MODE = '4'
                                            AND FSA.IS_DELETED <> 1
                                        GROUP BY FSA.STUDENT_ID
                                    ) HF 
                                        ON HF.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,
                                            SUM(IFNULL(FSA.CREDIT, 0)) AS EXAMFEE1,
                                            SUM(IFNULL(FSA.DEBIT, 0)) AS EXAMFEE1PAID
                                        FROM FEE_STUDENT_ACCOUNT FSA
                                        INNER JOIN STU_CLASS SC_EF1 ON SC_EF1.STUDENT_ID = FSA.STUDENT_ID AND SC_EF1.ACADEMIC_YEAR = ?AC_YEAR AND SC_EF1.IS_DELETED <> 1
                                        INNER JOIN FEE_FREQUENCY_FEE_MODE FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                        INNER JOIN SUP_FEE_FREQUENCY SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        WHERE
                                            FFF.FEE_MODE = '1'
                                            AND SFF.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SFF.SEMESTER_TYPE = 1
                                            AND FSA.IS_DELETED <> 1
                                            AND SFF.IS_DELETED <> 1
                                        GROUP BY FSA.STUDENT_ID
                                    ) EF1 
                                        ON EF1.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,
                                            SUM(IFNULL(FSA.CREDIT, 0)) AS EXAMFEE2,
                                            SUM(IFNULL(FSA.DEBIT, 0)) AS EXAMFEE2PAID
                                        FROM FEE_STUDENT_ACCOUNT FSA
                                        INNER JOIN STU_CLASS SC_EF2 ON SC_EF2.STUDENT_ID = FSA.STUDENT_ID AND SC_EF2.ACADEMIC_YEAR = ?AC_YEAR AND SC_EF2.IS_DELETED <> 1
                                        INNER JOIN FEE_FREQUENCY_FEE_MODE FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                        INNER JOIN SUP_FEE_FREQUENCY SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        WHERE
                                            FFF.FEE_MODE = '1'
                                            AND SFF.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SFF.SEMESTER_TYPE = 2
                                            AND FSA.IS_DELETED <> 1
                                            AND SFF.IS_DELETED <> 1
                                        GROUP BY FSA.STUDENT_ID
                                    ) EF2 
                                        ON EF2.STUDENT_ID = SPI.STUDENT_ID

                                    WHERE
                                        SPI.IS_DELETED <> 1

                                    ORDER BY 
                                        CC.CLASS_ID,
                                        SPI.ROLL_NO;";
                        break;
                    }
                case FeeSQLCommands.DeletedFeeStuDueDetails:
                    {
                        query = @"UPDATE STU_FEE_DUE_INFO  SET IS_DELETED =?IS_DELETED WHERE STU_DUE_ID=?STU_DUE_ID;";
                        break;
                    }
                case FeeSQLCommands.DeletedHostelFeeAccountByID:
                    {
                        query = @"UPDATE hostel_fee_student_account  SET IS_DELETED =?IS_DELETED WHERE F_STUDENT_AC_ID=?F_STUDENT_AC_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeDueListById:
                    {
                        query = @"SELECT 
                                    STU_DUE_ID,
                                    spi.STUDENT_ID,
                                    spi.ACADEMIC_YEAR,
                                    TERMFEE1,
                                    TERMFEE1PAID,
                                    TERMFEE2,
                                    TERMFEE2PAID,
                                    EXAMFEE1,
                                    EXAMFEE1PAID,
                                    EXAMFEE2,
                                    EXAMFEE2PAID,
                                    BUSFEE,
                                    BUSFEEPAID,
                                    HOSTELFEE,
                                    HOSTELFEEPAID,
                                    spi.FIRST_NAME,
                                    spi.ROLL_NO
                                FROM
                                    stu_fee_due_info AS sfd
                                        INNER JOIN
                                    stu_personal_info AS spi ON spi.STUDENT_ID = sfd.STUDENT_ID
                                WHERE
                                    STU_DUE_ID = ?STU_DUE_ID;";
                        break;
                    }
                case FeeSQLCommands.UpdateStuDueFeebyID:
                    {
                        query = @"UPDATE  STU_FEE_DUE_INFO SET 
                                                          TERMFEE1 =?TERMFEE1,
                                                          TERMFEE1PAID =?TERMFEE1PAID,
                                                          TERMFEE2 =?TERMFEE2,
                                                          TERMFEE2PAID =?TERMFEE2PAID,
                                                          EXAMFEE1 =?EXAMFEE1,
                                                          EXAMFEE1PAID =?EXAMFEE1PAID,
                                                          EXAMFEE2 =?EXAMFEE2,
                                                          EXAMFEE2PAID =?EXAMFEE2PAID,
                                                          BUSFEE =?BUSFEE, 
                                                          BUSFEEPAID =?BUSFEEPAID, 
                                                          HOSTELFEE =?HOSTELFEE, 
                                                          HOSTELFEEPAID =?HOSTELFEEPAID, 
                                                          ENTRY_DATE=?ENTRY_DATE,
                                                          ENTRY_ID=?ENTRY_ID
                                                      where
                                                          STU_DUE_ID =?STU_DUE_ID;";
                        break;
                    }
                case FeeSQLCommands.InsertStuDueFeebyID:
                    {
                        query = @" INSERT INTO stu_fee_due_info (
                                                STUDENT_ID,
                                                ACADEMIC_YEAR,
                                                TERMFEE1,
                                                TERMFEE2,
                                                EXAMFEE1,
                                                EXAMFEE2,
                                                SEMESTER_TYPE,
                                                ENTRY_ID
                                            )
                                            VALUES (
                                               ?STUDENT_ID,
                                               ?ACADEMIC_YEAR,
                                               ?TERMFEE1,
                                               ?TERMFEE2,
                                               ?EXAMFEE1,
                                               ?EXAMFEE2,
                                               ?SEMESTER_TYPE,
                                               ?ENTRY_ID
   
                                            );";
                        break;
                    }
                case FeeSQLCommands.FetchHostelDueByRollNO:
                    {
                        query = @"SELECT 
                                    hfsa.STUDENT_ID,
                                    spi.ROLL_NO,
                                    F_STUDENT_AC_ID
                                FROM
                                    hostel_fee_student_Account hfsa
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = hfsa.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID AND HFSA.ACADEMIC_YEAR= SC.ACADEMIC_YEAR
                                WHERE
                                    hfsa.ACADEMIC_YEAR = ?AC_YEAR and ROLL_NO = ?ROLL_NO and HEAD=?HEAD
                                      --  AND SPI.IS_DELETED != 1
                                       -- AND SPI.IS_LEFT != 1
                                        AND hfsa.IS_DELETED != 1 GROUP BY SPI.STUDENT_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchsupHeadOnlyHeadCode:
                    {
                        query = @"select HEAD_ID,HEAD_CODE from sup_head where IS_DELETED !=1 and HEAD_CODE !='';";
                        break;
                    }
                case FeeSQLCommands.FetchHostelFeeDetails:
                    {
                        query = @"SELECT 
                                        HFS.CREDIT,
                                        HFS.F_STUDENT_AC_ID,
                                        DEBIT,
                                        SPI.STUDENT_ID,
                                        SPI.FIRST_NAME,
                                        SPI.ROLL_NO,
                                        CC.CLASS_NAME,
                                        sh.HEAD,
                                        sh.HEAD_CODE,
                                        SUM(IFNULL(CREDIT, '0') - IFNULL(DEBIT, '0')) AS BALANCE
                                    FROM
                                        HOSTEL_FEE_STUDENT_ACCOUNT AS HFS
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = HFS.STUDENT_ID
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND HFS.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        SUP_HEAD AS SH ON SH.HEAD_ID = HFS.HEAD
                                    WHERE
                                        hfs.IS_DELETED != 1
                                            AND HFS.ACADEMIC_YEAR = ?AC_YEAR GROUP BY HFS.STUDENT_ID,HFS.HEAD
                                    ORDER BY ROLL_NO, sh.HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchHostelFeeById:
                    {
                        query = @"SELECT 
                                    F_STUDENT_AC_ID,
                                    spi.STUDENT_ID,
                                    spi.ACADEMIC_YEAR,
                                    CREDIT,
                                    DEBIT,
                                    spi.FIRST_NAME,
                                    spi.ROLL_NO
                                FROM
                                    hostel_fee_student_account AS hfs
                                        INNER JOIN
                                    stu_personal_info AS spi ON spi.STUDENT_ID = hfs.STUDENT_ID
                                WHERE
                                    F_STUDENT_AC_ID = ?F_STUDENT_AC_ID;";
                        break;
                    }
                case FeeSQLCommands.UpdateHostelFeeAccountByID:
                    {
                        query = @"UPDATE  hostel_fee_student_account SET 
                                                          CREDIT =?CREDIT,
                                                          DEBIT =?DEBIT,
                                                          ENTRY_DATE=?ENTRY_DATE,
                                                          ENTRY_ID=?ENTRY_ID
                                                      where
                                                          F_STUDENT_AC_ID =?F_STUDENT_AC_ID;";
                        break;
                    }

                case FeeSQLCommands.BusFeeStudentAccountByIDandTerm:
                    {
                        query = @"SELECT 
                                        BFS.STUDENT_ID,
                                        SPI.ROLL_NO,
                                        BFS.STUDENT_AC_ID
                                    FROM
                                        bus_fee_student_account bfs
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = bfs.STUDENT_ID
                                    WHERE
                                        bfs.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ROLL_NO = ?ROLL_NO
                                            AND TERM = ?TERM
                                            AND bfs.IS_DELETED != 1
                                    GROUP BY SPI.STUDENT_ID , TERM 
                                    UNION SELECT 
                                        BFS.STUDENT_ID,
                                        STAFF_CODE,
                                        BFS.STUDENT_AC_ID
                                    FROM
                                        BUS_FEE_STUDENT_ACCOUNT BFS
                                            INNER JOIN
                                        STf_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = BFS.STUDENT_ID
                                    WHERE
                                        BFS.ACADEMIC_YEAR = ?AC_YEAR  AND TERM = ?TERM
                                            AND BFS.IS_DELETED != 1
                                            AND STAFF_CODE = ?ROLL_NO;";
                        break;
                    }


                case FeeSQLCommands.AddTerm1Fees:
                    {
                        query = @"  INSERT INTO STU_FEE_DUE_INFO (
                           STUDENT_ID,
                           ACADEMIC_YEAR,
                           SEMESTER_TYPE,
                           TERMFEE1,
                           TERMFEE1PAID)
                           VALUES(
                           ?STUDENT_ID,
                           ?ACADEMIC_YEAR,
                           ?SEMESTER_TYPE,
                           ?TERMFEE1,
                           ?TERMFEE1PAID
                           );";
                        break;
                    }




                case FeeSQLCommands.AddTerm2Fees_list:
                    {
                        query = @"  INSERT INTO STU_FEE_DUE_INFO (
                           STUDENT_ID,
                           ACADEMIC_YEAR,
                           SEMESTER_TYPE,
                           TERMFEE2,
                           TERMFEE2PAID)
                           VALUES(
                           ?STUDENT_ID,
                           ?ACADEMIC_YEAR,
                           ?SEMESTER_TYPE,
                           ?TERMFEE2,
                           ?TERMFEE2PAID
                           );";
                        break;
                    }

                case FeeSQLCommands.AddExamFees:
                    {
                        query = @"   INSERT INTO STU_FEE_DUE_INFO (
                           STUDENT_ID,
                           ACADEMIC_YEAR,
                           SEMESTER_TYPE,
                           EXAMFEE1,
                           EXAMFEE1PAID,
                           EXAMFEE2,
                           EXAMFEE2PAID)
                           VALUES(
                           ?STUDENT_ID,
                           ?ACADEMIC_YEAR,
                           ?SEMESTER_TYPE,
                           ?EXAMFEE1,
                           ?EXAMFEE1PAID,
                           ?EXAMFEE2,
                           ?EXAMFEE2PAID
                           );";
                        break;
                    }
                case FeeSQLCommands.UpdateTerm1Fees:
                    {
                        query = @"    UPDATE  STU_FEE_DUE_INFO SET 
                                      TERMFEE1 =?TERMFEE1,
                                      TERMFEE1PAID =?TERMFEE1PAID, 
                                      SEMESTER_TYPE=?SEMESTER_TYPE
                                  where
                                      STU_DUE_ID =?STU_DUE_ID;";
                        break;
                    }
                case FeeSQLCommands.HostelUpdateFeeStudent:
                    {
                        query = @"     UPDATE  hostel_fee_student_account SET 
                                      CREDIT =?CREDIT,
                                      DEBIT =?DEBIT, 
                                      HEAD=?HEAD
                                  where
                                      F_STUDENT_AC_ID =?F_STUDENT_AC_ID;";
                        break;
                    }
                case FeeSQLCommands.HostelUpdateFeeStudentNew:
                    {
                        query = @"     UPDATE  fee_student_account SET 
                                      CREDIT =?CREDIT,
                                      DEBIT =?DEBIT, 
                                      HEAD=?HEAD,
                                      FREQUENCY_ID=?FREQUENCY_ID,
                                      FEE_MAIN_HEAD_ID=?FEE_MAIN_HEAD_ID
                                  where
                                      F_STUDENT_AC_ID =?F_STUDENT_AC_ID;";
                        break;
                    }
                case FeeSQLCommands.UpdateTerm2Fees_list:
                    {
                        query = @"    UPDATE  STU_FEE_DUE_INFO SET 
                                      TERMFEE2 =?TERMFEE2,
                                      TERMFEE2PAID =?TERMFEE2PAID, 
                                      SEMESTER_TYPE=?SEMESTER_TYPE
                                  where
                                      STU_DUE_ID =?STU_DUE_ID;";
                        break;
                    }
                case FeeSQLCommands.UpdateExamFees:
                    {
                        query = @"UPDATE  STU_FEE_DUE_INFO SET 
                                      EXAMFEE1 =?EXAMFEE1,
                                      EXAMFEE1PAID =?EXAMFEE2PAID,
                                      EXAMFEE2 =?EXAMFEE2,
                                      EXAMFEE2PAID =?EXAMFEE2PAID, 
                                      SEMESTER_TYPE=?SEMESTER_TYPE
                                  where
                                      STU_DUE_ID =?STU_DUE_ID;";
                        break;
                    }

                case FeeSQLCommands.UpdateBusFees:
                    {
                        query = @"UPDATE  bus_fee_student_account SET 
                                      CREDIT =?CREDIT,
                                      DEBIT =?DEBIT,
                                      TERM=?TERM,
                                      BUS_NO=?BUS_NO
                                  where
                                      STUDENT_AC_ID =?STUDENT_AC_ID;";
                        break;
                    }

                case FeeSQLCommands.FetchTermandExamFeeAdd:

                    {
                        query = @" SELECT 
                                        STU_DUE_ID,
                                        SFDI.STUDENT_ID,
                                        SFDI.ACADEMIC_YEAR,
                                        SST.SEMESTER_TYPE
                                    FROM
                                        STU_FEE_DUE_INFO AS SFDI
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SFDI.STUDENT_ID
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = SFDI.SEMESTER_TYPE
                                    WHERE
                                        SFDI.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND ROLL_NO = ?ROLL_NO
                                            AND SFDI.IS_DELETED != 1
                                    GROUP BY SPI.STUDENT_ID;";
                        break;
                    }




                case FeeSQLCommands.BusFeeStudentandstaffAccountByIDandTerm:
                    {
                        query = @"SELECT 
                                        STUDENT_ID,
                                        ROLL_NO,
                                        '5' AS ROLE
                                    FROM
                                   
                                        STU_PERSONAL_INFO
                                    WHERE
												IS_DELETED != 1 AND
                                             ROLL_NO = ?ROLL_NO
                                    UNION SELECT 
                                        STAFF_ID,
                                        STAFF_CODE,
                                        '4'  AS ROLE
                                    FROM
                                    
                                        STF_PERSONAL_INFO 
                                    WHERE
                                        STAFF_CODE= ?ROLL_NO AND 
										IS_DELETED != 1;";



                        break;
                    }

                case FeeSQLCommands.HostelFeeStudentAccountByIDandTerm:
                    {
                        query = @"   SELECT 
                                                SPI.STUDENT_ID, SPI.ROLL_NO, BFS.F_STUDENT_AC_ID
                                            FROM
                                                STU_PERSONAL_INFO AS SPI
                                                    LEFT JOIN
                                                hostel_fee_student_account bfs ON SPI.STUDENT_ID = bfs.STUDENT_ID
                                                    AND bfs.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                                    AND bfs.IS_DELETED != 1
                                            WHERE
                                                ROLL_NO = ?ROLL_NO
                                            GROUP BY SPI.STUDENT_ID;";

                        break;
                    }
                case FeeSQLCommands.HostelFeeStudentAccountByIDandTermNew:
                    {
                        query = @"   SELECT 
                                                SPI.STUDENT_ID, SPI.ROLL_NO, BFS.F_STUDENT_AC_ID
                                            FROM
                                                STU_PERSONAL_INFO AS SPI
                                                    LEFT JOIN
                                                fee_student_account bfs ON SPI.STUDENT_ID = bfs.STUDENT_ID
                                                    AND bfs.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                                    AND bfs.IS_DELETED != 1
                                            WHERE
                                                ROLL_NO = ?ROLL_NO
                                            GROUP BY SPI.STUDENT_ID;";

                        break;
                    }
                case FeeSQLCommands.HostelUpdateFeeStudentAccountByIDandTerm:
                    {
                        query = @"   SELECT 
                                        BFS.STUDENT_ID,
                                        SPI.ROLL_NO,
                                        BFS.F_STUDENT_AC_ID
                                    FROM
                                        hostel_fee_student_account bfs
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = bfs.STUDENT_ID
                                    WHERE
                                        bfs.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND ROLL_NO = ?ROLL_NO
                                            AND HEAD = ?HEAD
                                            AND bfs.IS_DELETED != 1
                                    GROUP BY SPI.STUDENT_ID; ";

                        break;
                    }
                case FeeSQLCommands.HostelUpdateFeeStudentAccountByIDandTermNew:
                    {
                        query = @"   SELECT 
                                        BFS.STUDENT_ID,
                                        SPI.ROLL_NO,
                                        BFS.F_STUDENT_AC_ID
                                    FROM
                                        fee_student_account bfs
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = bfs.STUDENT_ID
                                    WHERE
                                        bfs.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND ROLL_NO = ?ROLL_NO
                                            AND HEAD = ?HEAD
                                            AND bfs.IS_DELETED != 1
                                    GROUP BY SPI.STUDENT_ID; ";

                        break;
                    }
                case FeeSQLCommands.FetchStudentByRollId:
                    {
                        query = @"SELECT 
                                        STUDENT_ID,
                                        ROLL_NO
                                    FROM
                                   
                                        STU_PERSONAL_INFO
                                    WHERE
												IS_DELETED != 1 AND
                                             ROLL_NO = ?ROLL_NO;";



                        break;
                    }
                //case FeeSQLCommands.FetchStudentIDByRollId:
                //    {
                //        query = @" SELECT 
                //                        BFS.STUDENT_ID,
                //                        SPI.ROLL_NO,
                //                        BFS.STU_DUE_ID
                //                    FROM
                //                        STU_FEE_DUE_INFO bfs
                //                            INNER JOIN
                //                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = bfs.STUDENT_ID
                //                        INNER JOIN
                //                        bus_fee_student_account AS BFSA ON bfs.STUDENT_ID=BFSA.STUDENT_ID
                //                        INNER JOIN
                //                        hostel_fee_student_account AS HFSA ON bfs.STUDENT_ID=HFSA.STUDENT_ID
                //                    WHERE
                //                        bfs.ACADEMIC_YEAR = 2025
                //                            AND ROLL_NO = ""A25CMDB47""
                //                            AND SEMESTER_TYPE = 1
                //                            AND bfs.IS_DELETED != 1
                //                    GROUP BY SPI.STUDENT_ID";



                //        break;
                //    }
                case FeeSQLCommands.FetchStudentIDByRollId:
                    {
                        query = @" SELECT 
                                        BFS.STUDENT_ID,
                                        SPI.ROLL_NO,
                                        BFS.STU_DUE_ID
                                    FROM
                                        STU_FEE_DUE_INFO bfs
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = bfs.STUDENT_ID
                                    WHERE
                                        bfs.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ROLL_NO = ?ROLL_NO
                                            AND SEMESTER_TYPE = ?SEMESTER_TYPE
                                            AND bfs.IS_DELETED != 1
                                    GROUP BY SPI.STUDENT_ID";



                        break;
                    }
                case FeeSQLCommands.FetchBusFeeStudentAccountbyAcYearsjctnc:
                    {
                        query = @"SELECT
                FSA.F_STUDENT_AC_ID AS STUDENT_AC_ID,
                SPI.FIRST_NAME,
                SPI.ROLL_NO,
                SPI.STUDENT_ID,
                CC.CLASS_NAME,
                SH.HEAD,
                SH.HEAD_CODE,
                SUM(IFNULL(FSA.CREDIT,0)) AS CREDIT,
                SUM(IFNULL(FSA.DEBIT,0)) AS DEBIT,
                SUM(IFNULL(FSA.CREDIT,0) - IFNULL(FSA.DEBIT,0)) AS BALANCE,
                SBRD.BUS_ROUTE,
                SBRD.TERM
            FROM fee_student_account FSA
            LEFT JOIN student_bus_route_details SBRD
                ON SBRD.STUDENT_ID = FSA.STUDENT_ID
                AND SBRD.ACADEMIC_YEAR = FSA.ACADEMIC_YEAR
            INNER JOIN stu_personal_info SPI
                ON SPI.STUDENT_ID = FSA.STUDENT_ID
            INNER JOIN stu_class SC
                ON SC.STUDENT_ID = SPI.STUDENT_ID
                AND SC.ACADEMIC_YEAR = FSA.ACADEMIC_YEAR
            INNER JOIN cp_classes CC
                ON CC.CLASS_ID = SC.CLASS_ID
            INNER JOIN sup_head SH
                ON SH.HEAD_ID = FSA.HEAD
            WHERE FSA.IS_DELETED <> 1
                AND FSA.ACADEMIC_YEAR = ?AC_YEAR
                AND FSA.FREQUENCY_ID IN (27,28)
                AND FSA.HEAD = 137
            GROUP BY
                SPI.STUDENT_ID,
                SH.HEAD,
                SBRD.TERM,
                SBRD.BUS_ROUTE,
                SPI.FIRST_NAME,
                SPI.ROLL_NO,
                CC.CLASS_NAME,
                SH.HEAD_CODE";
                        break;
                    }
                case FeeSQLCommands.FetchBusFeeByIdsjctnc:
                    {
                        query = @"SELECT
                FSA.F_STUDENT_AC_ID AS STUDENT_AC_ID,
                SPI.FIRST_NAME,
                SPI.ROLL_NO,
                SPI.STUDENT_ID,
                CC.CLASS_NAME,
                SH.HEAD,
                SH.HEAD_CODE,
                FSA.CREDIT,
                FSA.DEBIT,
                SBRD.BUS_ROUTE,
                SBRD.BUS_NO,
                SBRD.TERM,
                FSA.FREQUENCY_ID
            FROM fee_student_account FSA
            LEFT JOIN student_bus_route_details SBRD
                ON SBRD.STUDENT_ID = FSA.STUDENT_ID
                AND SBRD.ACADEMIC_YEAR = FSA.ACADEMIC_YEAR
            INNER JOIN stu_personal_info SPI
                ON SPI.STUDENT_ID = FSA.STUDENT_ID
            INNER JOIN stu_class SC
                ON SC.STUDENT_ID = SPI.STUDENT_ID
                AND SC.ACADEMIC_YEAR = FSA.ACADEMIC_YEAR
            INNER JOIN cp_classes CC
                ON CC.CLASS_ID = SC.CLASS_ID
            INNER JOIN sup_head SH
                ON SH.HEAD_ID = FSA.HEAD
            WHERE FSA.IS_DELETED <> 1
                AND FSA.F_STUDENT_AC_ID = ?STUDENT_AC_ID";
                        break;
                    }


                case FeeSQLCommands.DeletedBusFeeAccountByIDSjctnc:
                    {
                        query = "UPDATE fee_student_account SET IS_DELETED = ?IS_DELETED WHERE F_STUDENT_AC_ID = ?STUDENT_AC_ID";
                        break;
                    }

                case FeeSQLCommands.FetchBusFeeStudentAccountbyAcYear:
                    {
                        query = @"SELECT 
                                    BFS.STUDENT_AC_ID,
                                    SPI.FIRST_NAME,
                                    SPI.ROLL_NO,
                                    SPI.STUDENT_ID,
                                    CC.CLASS_NAME,
                                    sh.HEAD,
                                    sh.HEAD_CODE,
                                    BFS.CREDIT,
                                    DEBIT,
                                    SUM(IFNULL(CREDIT, '0') - IFNULL(DEBIT, '0')) AS BALANCE,
                                    BUS_ROUTE,
                                    TERM
                                FROM
                                    bus_fee_student_account AS BFS
                                        INNER JOIN
                                    stu_personal_info AS spi ON spi.student_id = BFS.student_id
                                        INNER JOIN
                                    stu_class AS sc ON sc.student_id = spi.student_id
                                        AND Bfs.ACADEMIC_YEAR = sc.academic_year
                                        INNER JOIN
                                    cp_classes AS cc ON cc.class_id = sc.CLASS_ID
                                        INNER JOIN
                                    sup_head AS sh ON sh.HEAD_ID = BFS.HEAD
                                WHERE
                                    BFS.IS_DELETED != 1 AND BFS.ROLE = 5
                                        AND BFS.ACADEMIC_YEAR = ?AC_YEAR
                                GROUP BY BFS.STUDENT_ID , BFS.HEAD , TERM 
                                UNION SELECT 
                                    BFS.STUDENT_AC_ID,
                                    SPI.FIRST_NAME,
                                    SPI.STAFF_CODE,
                                    SPI.STAFF_ID,
                                    cd.DEPARTMENT,
                                    sh.HEAD,
                                    sh.HEAD_CODE,
                                    BFS.CREDIT,
                                    DEBIT,
                                    SUM(IFNULL(CREDIT, '0') - IFNULL(DEBIT, '0')) AS BALANCE,
                                    BUS_ROUTE,
                                    TERM
                                FROM
                                    BUS_FEE_STUDENT_ACCOUNT AS BFS
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = BFS.STUDENT_ID
                                        INNER JOIN
                                    CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SPI.DEPARTMENT
                                        INNER JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = BFS.HEAD
                                WHERE
                                    BFS.IS_DELETED != 1 AND BFS.ROLE = 4
                                        AND BFS.ACADEMIC_YEAR = ?AC_YEAR
                                GROUP BY BFS.STUDENT_ID , BFS.HEAD , TERM
                                ORDER BY ROLL_NO;";
                        break;
                    }
                case FeeSQLCommands.DeletedBusFeeAccountByID:
                    {
                        query = @"UPDATE bus_fee_student_account  SET IS_DELETED =?IS_DELETED WHERE STUDENT_AC_ID=?STUDENT_AC_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchBusFeeById:
                    {
                        query = @"SELECT 
                                    STUDENT_AC_ID,
                                    spi.STUDENT_ID,
                                    spi.ACADEMIC_YEAR,
                                    CREDIT,
                                    DEBIT,
                                    spi.FIRST_NAME,
                                    spi.ROLL_NO
                                FROM
                                    bus_fee_student_account AS hfs
                                        INNER JOIN
                                    stu_personal_info AS spi ON spi.STUDENT_ID = hfs.STUDENT_ID
                                WHERE
                                    STUDENT_AC_ID = ?STUDENT_AC_ID AND hfs.role = 5 
                                UNION SELECT 
                                    STUDENT_AC_ID,
                                    spi.staff_id,
                                    spi.ACADEMIC_YEAR,
                                    CREDIT,
                                    DEBIT,
                                    spi.FIRST_NAME,
                                    spi.staff_code
                                FROM
                                    bus_fee_student_account AS hfs
                                        INNER JOIN
                                    stf_personal_info AS spi ON spi.staff_id = hfs.STUDENT_ID
                                WHERE
                                    STUDENT_AC_ID = ?STUDENT_AC_ID AND hfs.role = 4;";
                        break;
                    }
                case FeeSQLCommands.UpdateBusFeeAccountByID:
                    {
                        query = @"UPDATE  bus_fee_student_account SET 
                                                          CREDIT =?CREDIT,
                                                          DEBIT =?DEBIT,
                                                          ENTRY_DATE=?ENTRY_DATE,
                                                          ENTRY_ID=?ENTRY_ID
                                                      where
                                                          STUDENT_AC_ID =?STUDENT_AC_ID;";
                        break;
                    }
                case FeeSQLCommands.AddBusFeeAccountByID:
                    {
                        query = @"   insert into bus_fee_student_account (
                               STUDENT_ID,
                               BUS_ROUTE,
                               ROLE,
                               ACADEMIC_YEAR,
                               TERM,
                               HEAD,
                               ENTRY_ID,
                               BUS_NO,
                               CREDIT,
                               DEBIT)
                               values(
                               ?STUDENT_ID,
                               ?BUS_ROUTE,
                               ?ROLE,
                               ?ACADEMIC_YEAR,
                               ?TERM,
                               ?HEAD,
                               ?ENTRY_ID,
                               ?BUS_NO,
                               ?CREDIT,
                               ?DEBIT);";
                        break;
                    }
                case FeeSQLCommands.AddHostelFeeAccountByID:
                    {
                        query = @"  insert into hostel_fee_student_account (
                               STUDENT_ID,
                               ACADEMIC_YEAR,
                               HEAD,
                               ENTRY_ID,
                               CREDIT,
                               DEBIT)
                               values(
                               ?STUDENT_ID,
                               ?ACADEMIC_YEAR,
                               ?HEAD,
                               ?ENTRY_ID,
                               ?CREDIT,
                               ?DEBIT);";
                        break;
                    }
                case FeeSQLCommands.AddHostelFeeAccountByIDNew:
                    {
                        query = @"  insert into fee_student_account (
                               STUDENT_ID,
                               ACADEMIC_YEAR,
                               HEAD,
                               CREDIT,
                               DEBIT,
                               FREQUENCY_ID,
                               FEE_MAIN_HEAD_ID)
                               values(
                               ?STUDENT_ID,
                               ?ACADEMIC_YEAR,
                               ?HEAD,
                               ?CREDIT,
                               ?DEBIT,
                               ?FREQUENCY_ID,
                               ?FEE_MAIN_HEAD_ID);";
                        break;
                    }
                case FeeSQLCommands.AddExamandtermFeeAccountByID:
                    {
                        query = @"   INSERT INTO stu_fee_due_info (
                                        STUDENT_ID,
                                        ACADEMIC_YEAR,
                                        TERMFEE1,
                                        TERMFEE2,
                                        EXAMFEE1,
                                        EXAMFEE2,
                                        SEMESTER_TYPE,
                                        ENTRY_ID
                                    )
                                    VALUES (
                                       ?STUDENT_ID,
                                       ?ACADEMIC_YEAR,
                                       ?TERMFEE1,
                                       ?TERMFEE2,
                                       ?EXAMFEE1,
                                       ?EXAMFEE2,
                                       ?SEMESTER_TYPE,
                                       ?ENTRY_ID
   
                                    );";
                        break;
                    }
                case FeeSQLCommands.FetchFeeDueDetailsByDepartmentId:
                    {
                        query = @"SELECT 
                                    sfd.STUDENT_ID,
                                    spi.FIRST_NAME,
                                    spi.ROLL_NO,
                                    STU_DUE_ID,
                                    cc.CLASS_NAME,
                                    cc.CLASS_ID,
                                    sfd.TERMFEE1,
                                    TERMFEE1PAID,
                                    IF(TERMFEE1 IS NULL,
                                        TERMFEE1,
                                        SUM(IFNULL(TERMFEE1, 0) - IFNULL(TERMFEE1PAID, 0))) AS TERMFEE1DUE,
                                    sfd.TERMFEE2,
                                    sfd.TERMFEE2PAID,
                                    IF(TERMFEE2 IS NULL,
                                        TERMFEE2,
                                        SUM(IFNULL(TERMFEE2, 0)) - SUM(IFNULL(TERMFEE2PAID, 0))) AS TERMFEE2DUE,
                                    sfd.EXAMFEE1,
                                    EXAMFEE1PAID,
                                    IF(EXAMFEE1 IS NULL,
                                        EXAMFEE1,
                                        SUM(IFNULL(EXAMFEE1, 0) - IFNULL(EXAMFEE1PAID, 0))) AS EXAMFEE1DUE,
                                    sfd.EXAMFEE2,
                                    EXAMFEE2PAID,
                                    IF(EXAMFEE2 IS NULL,
                                        EXAMFEE2,
                                        SUM(IFNULL(EXAMFEE2, 0) - IFNULL(EXAMFEE2PAID, 0))) AS EXAMFEE2DUE,
                                    B_CREDIT as BUSFEE,
                                   B_DEBIT as  BUSFEEPAID,
                                    IF(B_CREDIT IS NULL,
                                        B_CREDIT,
                                        SUM(IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0))) AS BUSFEEDUE,
                                    IF(HFSA.CREDIT IS NULL,
                                        HFSA.CREDIT,
                                        SUM(IFNULL(HFSA.CREDIT, '0'))) AS HOSTELFEE,
                                    IF(HFSA.DEBIT IS NULL,
                                        HFSA.DEBIT,
                                        SUM(IFNULL(HFSA.DEBIT, '0'))) HOSTELFEEPAID,
                                    IF(HFSA.CREDIT IS NULL,
                                        NULL,
                                        SUM(IFNULL(HFSA.CREDIT, '0')) - SUM(IFNULL(HFSA.DEBIT, '0'))) AS HOSTELFEEDUE,
                                    SUM(CASE
											WHEN sfd.TERMFEE1 IS NULL THEN 0
											ELSE IF(IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0) < 0,
												0,
												IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0))
										END + CASE
											WHEN sfd.TERMFEE2 IS NULL THEN 0
											ELSE IF(IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0)<0,
												0,
												IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0))
										END + CASE
											WHEN sfd.EXAMFEE1 IS NULL THEN 0
											ELSE IF(IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0)<0,
												0,
												IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0))
										END + CASE
											WHEN sfd.EXAMFEE2 IS NULL THEN 0
											ELSE IF(IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0)<0,
												0,
												IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0))
										END + CASE
											WHEN B_CREDIT IS NULL THEN 0
											ELSE IF(IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0)<0,
												0,
												IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0))
										END + CASE
											WHEN HFSA.CREDIT IS NULL THEN 0
											ELSE IF(IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0)<0,
												0,
												IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0))
										END) AS TOTAL_DUE_AMOUNT,
                                    sfd.ENTRY_ID,
                                    sfd.ENTRY_DATE
                                FROM
                                    STU_PERSONAL_INFO AS SPI
                                        INNER JOIN
                                    STU_FEE_DUE_INFO AS SFD ON SPI.STUDENT_ID = SFD.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = SFD.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        LEFT JOIN
                                    (SELECT 
                                        STUDENT_ID,
                                            SUM(IFNULL(CREDIT, '0')) AS CREDIT,
                                            SUM(IFNULL(DEBIT, '0')) AS DEBIT
                                    FROM
                                        HOSTEL_FEE_STUDENT_ACCOUNT AS hf
                                    WHERE
                                        hf.IS_DELETED != 1
                                            AND hf.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY STUDENT_ID) AS HFSA ON HFSA.STUDENT_ID = SPI.STUDENT_ID
                                    left join (SELECT 
                                        STUDENT_ID,
                                            SUM(IFNULL(CREDIT, '0')) AS B_CREDIT,
                                            SUM(IFNULL(DEBIT, '0')) AS B_DEBIT
                                    FROM
                                        BUS_FEE_STUDENT_ACCOUNT AS Bf
                                    WHERE
                                        Bf.IS_DELETED != 1
                                            AND Bf.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY STUDENT_ID) AS BFSA ON BFSA.STUDENT_ID = SPI.STUDENT_ID
                                WHERE
                                    SFD.ACADEMIC_YEAR = ?AC_YEAR and CC.DEPARTMENT =?DEPARTMENT
                                        AND SC.IS_DELETED != 1
                                        AND SFD.IS_DELETED != 1
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                GROUP BY SFD.STUDENT_ID , sfd.ACADEMIC_YEAR
                                ORDER BY ROLL_NO , SC.CLASS_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeDueDetailsByDepartmentIdArrear:
                    {

                        query = @"SELECT 
                                        SPI.STUDENT_ID,
                                        SPI.FIRST_NAME,
                                        SPI.ROLL_NO,
                                        CUR.STU_DUE_ID,
                                        CC.CLASS_ID,
                                        CC.CLASS_NAME,

                                        CUR.TERMFEE1,
                                        CUR.TERMFEE1PAID,
                                        GREATEST(
                                            IFNULL(CUR.TERMFEE1, 0) - IFNULL(CUR.TERMFEE1PAID, 0),
                                            0
                                        ) AS TERMFEE1DUE,

                                        CUR.TERMFEE2,
                                        CUR.TERMFEE2PAID,
                                        GREATEST(
                                            IFNULL(CUR.TERMFEE2, 0) - IFNULL(CUR.TERMFEE2PAID, 0),
                                            0
                                        ) AS TERMFEE2DUE,

                                        IFNULL(EF1.EXAMFEE1, 0) AS EXAMFEE1,
                                        IFNULL(EF1.EXAMFEE1PAID, 0) AS EXAMFEE1PAID,
                                        GREATEST(
                                            IFNULL(EF1.EXAMFEE1, 0) - IFNULL(EF1.EXAMFEE1PAID, 0),
                                            0
                                        ) AS EXAMFEE1DUE,

                                        IFNULL(EF2.EXAMFEE2, 0) AS EXAMFEE2,
                                        IFNULL(EF2.EXAMFEE2PAID, 0) AS EXAMFEE2PAID,
                                        GREATEST(
                                            IFNULL(EF2.EXAMFEE2, 0) - IFNULL(EF2.EXAMFEE2PAID, 0),
                                            0
                                        ) AS EXAMFEE2DUE,

                                        /* Previous year due */
                                        IFNULL(ARR.ARREAR_DUE, 0) AS PREVIOUS_YEAR_TERM_DUE,

                                       
                                        IFNULL(HF.HOSTEL_FEE, 0) AS HOSTELFEE,

                                        IFNULL(HF.HOSTEL_FEE_PAID, 0) AS HOSTELFEEPAID,

                                        GREATEST(
                                            IFNULL(HF.HOSTEL_FEE, 0)
                                            - IFNULL(HF.HOSTEL_FEE_PAID, 0),
                                            0
                                        ) AS HOSTELFEEDUE,


                                        IFNULL(BF.BUS_FEE, 0) AS BUSFEE,

                                        IFNULL(BF.BUS_FEE_PAID, 0) AS BUSFEEPAID,

                                        GREATEST(
                                            IFNULL(BF.BUS_FEE, 0)
                                            - IFNULL(BF.BUS_FEE_PAID, 0),
                                            0
                                        ) AS BUSFEEDUE,

                  

                                        (
                                            IFNULL(ARR.ARREAR_DUE, 0)

                                            + GREATEST(
                                                IFNULL(CUR.TERMFEE1, 0)
                                                - IFNULL(CUR.TERMFEE1PAID, 0),
                                                0
                                            )

                                            + GREATEST(
                                                IFNULL(CUR.TERMFEE2, 0)
                                                - IFNULL(CUR.TERMFEE2PAID, 0),
                                                0
                                            )

                                            + GREATEST(
                                                IFNULL(EF1.EXAMFEE1, 0)
                                                - IFNULL(EF1.EXAMFEE1PAID, 0),
                                                0
                                            )

                                            + GREATEST(
                                                IFNULL(EF2.EXAMFEE2, 0)
                                                - IFNULL(EF2.EXAMFEE2PAID, 0),
                                                0
                                            )

                                            + GREATEST(
                                                IFNULL(HF.HOSTEL_FEE, 0)
                                                - IFNULL(HF.HOSTEL_FEE_PAID, 0),
                                                0
                                            )

                                            + GREATEST(
                                                IFNULL(BF.BUS_FEE, 0)
                                                - IFNULL(BF.BUS_FEE_PAID, 0),
                                                0
                                            )

                                        ) AS TOTAL_DUE_AMOUNT,

                                        CUR.ENTRY_ID,
                                        CUR.ENTRY_DATE

                                    FROM STU_PERSONAL_INFO SPI

                                    INNER JOIN STU_CLASS SC 
                                        ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.IS_DELETED <> 1

                                    INNER JOIN CP_CLASSES CC 
                                        ON CC.CLASS_ID = SC.CLASS_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            *
                                        FROM STU_FEE_DUE_INFO
                                        WHERE
                                            ACADEMIC_YEAR = ?AC_YEAR
                                            AND IS_DELETED <> 1
                                    ) CUR 
                                        ON CUR.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            STUDENT_ID,

                                            SUM(
                                                GREATEST(
                                                    IFNULL(TERMFEE1, 0)
                                                    - IFNULL(TERMFEE1PAID, 0),
                                                    0
                                                )
                                                +
                                                GREATEST(
                                                    IFNULL(TERMFEE2, 0)
                                                    - IFNULL(TERMFEE2PAID, 0),
                                                    0
                                                )
                                            ) AS ARREAR_DUE

                                        FROM STU_FEE_DUE_INFO

                                        WHERE
                                            ACADEMIC_YEAR < ?AC_YEAR
                                            AND IS_DELETED <> 1

                                        GROUP BY STUDENT_ID

                                    ) ARR 
                                        ON ARR.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,

                                            SUM(IFNULL(FSA.CREDIT, 0)) AS BUS_FEE,

                                            SUM(IFNULL(FSA.DEBIT, 0)) AS BUS_FEE_PAID

                                        FROM FEE_STUDENT_ACCOUNT FSA

                                        INNER JOIN FEE_FREQUENCY_FEE_MODE FFF
                                            ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID

                                        INNER JOIN SUP_FEE_FREQUENCY SFF
                                            ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID

                                        INNER JOIN SUP_FREQUENCY_TYPE SFT
                                            ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE

                                        WHERE
                                            SFT.FREQUENCY_TYPE_ID = 8
                                            AND FSA.IS_DELETED <> 1

                                        GROUP BY FSA.STUDENT_ID

                                    ) BF 
                                        ON BF.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,

                                            SUM(IFNULL(FSA.CREDIT, 0)) AS HOSTEL_FEE,

                                            SUM(IFNULL(FSA.DEBIT, 0)) AS HOSTEL_FEE_PAID

                                        FROM FEE_STUDENT_ACCOUNT FSA

                                        INNER JOIN FEE_FREQUENCY_FEE_MODE FFF
                                            ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID

                                        INNER JOIN SUP_FEE_FREQUENCY SFF
                                            ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID

                                        INNER JOIN SUP_FREQUENCY_TYPE SFT
                                            ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE

                                        WHERE
                                            SFT.FREQUENCY_TYPE_ID = 4
                                            AND FSA.IS_DELETED <> 1

                                        GROUP BY FSA.STUDENT_ID

                                    ) HF 
                                        ON HF.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,
                                            SUM(IFNULL(FSA.CREDIT, 0)) AS EXAMFEE1,
                                            SUM(IFNULL(FSA.DEBIT, 0)) AS EXAMFEE1PAID
                                        FROM FEE_STUDENT_ACCOUNT FSA
                                        INNER JOIN STU_CLASS SC_EF1 ON SC_EF1.STUDENT_ID = FSA.STUDENT_ID AND SC_EF1.ACADEMIC_YEAR = ?AC_YEAR AND SC_EF1.IS_DELETED <> 1
                                        INNER JOIN CP_CLASSES CC_EF1 ON CC_EF1.CLASS_ID = SC_EF1.CLASS_ID AND CC_EF1.DEPARTMENT = ?DEPARTMENT
                                        INNER JOIN FEE_FREQUENCY_FEE_MODE FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                        INNER JOIN SUP_FEE_FREQUENCY SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        WHERE
                                            FFF.FEE_MODE = '1'
                                            AND SFF.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SFF.SEMESTER_TYPE = 1
                                            AND FSA.IS_DELETED <> 1
                                            AND SFF.IS_DELETED <> 1
                                        GROUP BY FSA.STUDENT_ID
                                    ) EF1 
                                        ON EF1.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,
                                            SUM(IFNULL(FSA.CREDIT, 0)) AS EXAMFEE2,
                                            SUM(IFNULL(FSA.DEBIT, 0)) AS EXAMFEE2PAID
                                        FROM FEE_STUDENT_ACCOUNT FSA
                                        INNER JOIN STU_CLASS SC_EF2 ON SC_EF2.STUDENT_ID = FSA.STUDENT_ID AND SC_EF2.ACADEMIC_YEAR = ?AC_YEAR AND SC_EF2.IS_DELETED <> 1
                                        INNER JOIN CP_CLASSES CC_EF2 ON CC_EF2.CLASS_ID = SC_EF2.CLASS_ID AND CC_EF2.DEPARTMENT = ?DEPARTMENT
                                        INNER JOIN FEE_FREQUENCY_FEE_MODE FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                        INNER JOIN SUP_FEE_FREQUENCY SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        WHERE
                                            FFF.FEE_MODE = '1'
                                            AND SFF.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SFF.SEMESTER_TYPE = 2
                                            AND FSA.IS_DELETED <> 1
                                            AND SFF.IS_DELETED <> 1
                                        GROUP BY FSA.STUDENT_ID
                                    ) EF2 
                                        ON EF2.STUDENT_ID = SPI.STUDENT_ID

                                    WHERE
                                        SPI.IS_DELETED <> 1
                                        AND  CC.DEPARTMENT =?DEPARTMENT

                                    ORDER BY 
                                        CC.CLASS_ID,
                                        SPI.ROLL_NO;";

                        //query = @"SELECT 
                        //            SPI.STUDENT_ID,
                        //            SPI.FIRST_NAME,
                        //            SPI.ROLL_NO,
                        //            CUR.STU_DUE_ID,
                        //            CC.CLASS_ID,
                        //            CC.CLASS_NAME,
                        //            CUR.TERMFEE1,
                        //            CUR.TERMFEE1PAID,
                        //            GREATEST(IFNULL(CUR.TERMFEE1, 0) - IFNULL(CUR.TERMFEE1PAID, 0),
                        //                    0) TERMFEE1DUE,
                        //            CUR.TERMFEE2,
                        //            CUR.TERMFEE2PAID,
                        //            GREATEST(IFNULL(CUR.TERMFEE2, 0) - IFNULL(CUR.TERMFEE2PAID, 0),
                        //                    0) TERMFEE2DUE,
                        //            CUR.EXAMFEE1,
                        //            CUR.EXAMFEE1PAID,
                        //            GREATEST(IFNULL(CUR.EXAMFEE1, 0) - IFNULL(CUR.EXAMFEE1PAID, 0),
                        //                    0) EXAMFEE1DUE,
                        //            CUR.EXAMFEE2,
                        //            CUR.EXAMFEE2PAID,
                        //            GREATEST(IFNULL(CUR.EXAMFEE2, 0) - IFNULL(CUR.EXAMFEE2PAID, 0),
                        //                    0) EXAMFEE2DUE,
                        //            IFNULL(ARR.ARREAR_DUE, 0) AS PREVIOUS_YEAR_TERM_DUE,
                        //            IFNULL(BF.BUS_FEE, 0) AS BUSFEE,
                        //            IFNULL(HF.HOSTEL_FEE, 0) AS HOSTELFEE,
                        //            (IFNULL(ARR.ARREAR_DUE, 0) + GREATEST(IFNULL(CUR.TERMFEE1, 0) - IFNULL(CUR.TERMFEE1PAID, 0),
                        //                    0) + GREATEST(IFNULL(CUR.TERMFEE2, 0) - IFNULL(CUR.TERMFEE2PAID, 0),
                        //                    0) + GREATEST(IFNULL(CUR.EXAMFEE1, 0) - IFNULL(CUR.EXAMFEE1PAID, 0),
                        //                    0) + GREATEST(IFNULL(CUR.EXAMFEE2, 0) - IFNULL(CUR.EXAMFEE2PAID, 0),
                        //                    0) + IFNULL(BF.BUS_FEE, 0) + IFNULL(HF.HOSTEL_FEE, 0)) AS TOTAL_DUE_AMOUNT,
                        //            CUR.ENTRY_ID,
                        //            CUR.ENTRY_DATE
                        //        FROM
                        //            STU_PERSONAL_INFO SPI
                        //                INNER JOIN
                        //            STU_CLASS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                        //                AND SC.ACADEMIC_YEAR = ?AC_YEAR
                        //                AND SC.IS_DELETED <>1
                        //                INNER JOIN
                        //            CP_CLASSES CC ON CC.CLASS_ID = SC.CLASS_ID
                        //                LEFT JOIN
                        //            (SELECT 
                        //                *
                        //            FROM
                        //                STU_FEE_DUE_INFO
                        //            WHERE
                        //                ACADEMIC_YEAR = ?AC_YEAR AND IS_DELETED <>1) CUR ON CUR.STUDENT_ID = SPI.STUDENT_ID
                        //                LEFT JOIN
                        //            (SELECT 
                        //                STUDENT_ID,
                        //                    SUM(GREATEST(IFNULL(TERMFEE1, 0) - IFNULL(TERMFEE1PAID, 0), 0) + GREATEST(IFNULL(TERMFEE2, 0) - IFNULL(TERMFEE2PAID, 0), 0)) AS ARREAR_DUE
                        //            FROM
                        //                STU_FEE_DUE_INFO
                        //            WHERE
                        //                ACADEMIC_YEAR < ?AC_YEAR AND IS_DELETED <>1
                        //            GROUP BY STUDENT_ID) ARR ON ARR.STUDENT_ID = SPI.STUDENT_ID
                        //                LEFT JOIN
                        //            (SELECT 
                        //                STUDENT_ID,
                        //                    GREATEST(SUM(IFNULL(CREDIT, 0)) - SUM(IFNULL(DEBIT, 0)), 0) AS BUS_FEE
                        //            FROM
                        //                BUS_FEE_STUDENT_ACCOUNT
                        //            WHERE
                        //                IS_DELETED <>1
                        //            GROUP BY STUDENT_ID) BF ON BF.STUDENT_ID = SPI.STUDENT_ID
                        //                LEFT JOIN
                        //            (SELECT 
                        //                STUDENT_ID,
                        //                    GREATEST(SUM(IFNULL(CREDIT, 0)) - SUM(IFNULL(DEBIT, 0)), 0) AS HOSTEL_FEE
                        //            FROM
                        //                HOSTEL_FEE_STUDENT_ACCOUNT
                        //            WHERE
                        //                IS_DELETED <>1
                        //            GROUP BY STUDENT_ID) HF ON HF.STUDENT_ID = SPI.STUDENT_ID
                        //        WHERE
                        //            SPI.IS_DELETED <>1  and CC.DEPARTMENT =?DEPARTMENT
                        //        ORDER BY CC.CLASS_ID , SPI.ROLL_NO;";
                        break;
                    }
                case FeeSQLCommands.FetchProgrammeGroupByDepartment:
                    {
                        query = @"SELECT 
                                        PG.PROGRAMME_GROUP_ID AS PROGRAMME_ID,
                                        CONCAT(IFNULL(CP.PROGRAMME_NAME, ''),
                                                ' (',
                                                IFNULL(CONCAT(SPM.PROGRAMME_MODE), ''),
                                                ')',
                                                '-',
                                                IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), '')) AS PROGRAMME_NAME
                                    FROM
                                        CP_PROGRAMME_GROUP AS PG
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                                            INNER JOIN
                                        SUP_SHIFT AS SH ON SH.SHIFT_ID = PG.SHIFT
                                    WHERE
                                         DEPARTMENT=?DEPARTMENT
                                    GROUP BY PG.PROGRAMME_GROUP_ID;;";
                        break;
                    }
                case FeeSQLCommands.FetchClassListByProgrammeGroupId:
                    {
                        query = @"SELECT 
                                        CLASS_ID, 
                                        CLASS_NAME, 
                                        CLASS_CODE
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        PROGRAMME_GROUP_ID in (?PROGRAMME_GROUP_ID)
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case FeeSQLCommands.FetchStudentMobileNoSMS:
                    {
                        query = @" SELECT 
                                        SP.STUDENT_ID,
                                        CONCAT(IFNULL(UPPER(SP.FIRST_NAME), ''),
                                                ' (',
                                                sp.ROLL_NO,
                                                ')') AS FIRST_NAME,
                                        UPPER(ROLL_NO) AS ROLL_NO,
                                        IFNULL(FR_MOBILE,STU_MOBILENO) AS MOBILE_NO,
                                        IF(CLS.SHIFT = 1,
                                            REPLACE(CLS.CLASS_NAME, ' DAY', ''),
                                            REPLACE(CLS.CLASS_NAME, ' EVE', '')) AS CLASS_NAME,
                                        SC.CLASS_ID
                                    FROM
                                        STU_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        SP.STUDENT_ID =?STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    ORDER BY ROLL_NO;";
                        break;
                    }
                case FeeSQLCommands.FetchStudentInfoforLateEntrySMS:
                    {
                        query = @"SELECT 
                                    CONCAT(IFNULL(UPPER(SP.FIRST_NAME), ''),
                                            ' (',
                                            sp.ROLL_NO,
                                            ')') AS FIRST_NAME,
                                    UPPER(ROLL_NO) AS ROLL_NO,
                                    IFNULL(FR_MOBILE, SP.STU_MOBILENO) AS MOBILE_NO,
                                    IF(CLS.SHIFT = 1,
                                        REPLACE(CLS.CLASS_NAME, ' DAY', ''),
                                        REPLACE(CLS.CLASS_NAME, ' EVE', '')) AS CLASS_NAME,
                                    SP.STUDENT_ID,
                                    SC.CLASS_ID
                                FROM
                                    STU_LATE_ENTRY AS SL
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SL.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                WHERE
                                    SL.IS_DELETED != 1 and SL.IS_SMS_SENT !=1
                                        AND DATE_FORMAT(SL.ENTRY_DATE, '%Y-%m-%d') = CURDATE()
                                        AND SL.ACADEMIC_YEAR = ?AC_YEAR
                                ORDER BY LATE_FEE_ID DESC;";
                        break;
                    }
                #endregion

                case FeeSQLCommands.FetchExamFeepaidStatus:
                    {
                        query = @"SELECT 
                                    sfd.STUDENT_ID,
                                    spi.FIRST_NAME,
                                    spi.ROLL_NO,
                                    STU_DUE_ID,
                                    cc.CLASS_NAME,
                                    cc.CLASS_ID,
                                    sfd.EXAMFEE1,
                                    EXAMFEE1PAID,
                                    IF(EXAMFEE1 IS NULL,
                                        EXAMFEE1,
                                        SUM(IFNULL(EXAMFEE1, 0) - IFNULL(EXAMFEE1PAID, 0))) AS EXAMFEE1DUE,
                                    sfd.EXAMFEE2,
                                    EXAMFEE2PAID,
                                    IF(EXAMFEE2 IS NULL,
                                        EXAMFEE2,
                                        SUM(IFNULL(EXAMFEE2, 0) - IFNULL(EXAMFEE2PAID, 0))) AS EXAMFEE2DUE
                                FROM
                                    STU_PERSONAL_INFO AS SPI
                                        INNER JOIN
                                    STU_FEE_DUE_INFO AS SFD ON SPI.STUDENT_ID = SFD.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = SFD.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                       
                                WHERE
                                    SFD.ACADEMIC_YEAR = ?AC_YEAR  AND SC.CLASS_ID IN (?CLASS_ID)
                                        AND SC.IS_DELETED != 1
                                        AND SFD.IS_DELETED != 1
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                GROUP BY SFD.STUDENT_ID , sfd.ACADEMIC_YEAR
                             -- unpaid   having EXAMFEE1DUE >0
                                -- paid   having EXAMFEE1DUE <=0
                             -- unpaid2   having EXAMFEE2DUE >0
                             -- paid2   having EXAMFEE2DUE <=0
                                ORDER BY ROLL_NO , SC.CLASS_ID;";
                        break;
                    }
                case FeeSQLCommands.DeleteExamDefaultersList:
                    {
                        query = @"UPDATE EXAM_DEFAULTERS 
                                                SET 
                                                    IS_DELETED = ?IS_DELETED
                                                WHERE
                                                    EXAM_DEFALTER_ID IN (SELECT 
                                                            supQuery.EXAM_DEFALTER_ID
                                                        FROM
                                                            (SELECT 
                                                                EXAM_DEFALTER_ID
                                                            FROM
                                                                CMS_SJCTNC.EXAM_DEFAULTERS AS ED
                                                            INNER JOIN STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = ED.STUDENT_ID
                                                            WHERE
                                                                DEFAULT_CATEGORY_ID = 1
                                                                    AND ED.IS_DELETED != 1
                                                                    AND spi.ROLL_NO = ?ROLL_NO) AS supQuery);";
                        break;
                    }
                case FeeSQLCommands.DeleteHostelDefaultersListfromSjcExamResultDB:
                    {
                        query = @" UPDATE hostel_fee_student_account 
                                                SET 
                                                    IS_DELETED = ?IS_DELETED
                                                WHERE
                                                    F_STUDENT_AC_ID IN (SELECT 
                                                            supQuery.F_STUDENT_AC_ID
                                                        FROM
                                                            (SELECT 
                                                                F_STUDENT_AC_ID,
                                                                STUDENT_ID,
                                                                ACADEMIC_YEAR
                                                            FROM
                                                                CMS_SJCTNC.hostel_fee_student_account AS ED
                                                            INNER JOIN STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = ED.STUDENT_ID
                                                            WHERE
                                                            
                                                                     ED.IS_DELETED != 1
                                                                    AND spi.ROLL_NO = ?ROLL_NO) AS supQuery);";
                        break;
                    }
                case FeeSQLCommands.DeleteBusDefaultersList:
                    {
                        query = @"   UPDATE bus_fee_student_account 
                                                SET 
                                                    IS_DELETED = ?IS_DELETED
                                                WHERE
                                                    STUDENT_AC_ID IN (SELECT 
                                                            supQuery.STUDENT_AC_ID
                                                        FROM
                                                            (SELECT 
                                                                STUDENT_AC_ID,
                                                                STUDENT_ID,
                                                                ACADEMIC_YEAR
                                                            FROM
                                                                CMS_SJCTNC.bus_fee_student_account AS ED
                                                            INNER JOIN STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = ED.STUDENT_ID
                                                            WHERE
                                                            
                                                                     ED.IS_DELETED != 1
                                                                    AND spi.ROLL_NO = ?ROLL_NO) AS supQuery);";
                        break;
                    }
                case FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB:
                    {
                        query = @"UPDATE ?RESULT_DB.EXAM_DEFAULTERS 
                                                                    SET 
                                                                        IS_DELETED = ?IS_DELETED
                                                                    WHERE
                                                                        ROLL_NO = ?ROLL_NO
                                                                            AND DEFAULT_CATEGORY_ID = 1
                                                                            AND IS_DELETED !=1;";
                        break;
                    }
                case FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters:
                    {
                        query = @"SELECT 
                                        sfd.STUDENT_ID,
                                        spi.FIRST_NAME,
                                        spi.ROLL_NO,
                                        STU_DUE_ID,
                                        SUM(CASE
                                            WHEN sfd.TERMFEE1 IS NULL THEN 0
                                            ELSE IF(IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0) < 0,
                                                0,
                                                IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0))
                                        END + CASE
                                            WHEN sfd.TERMFEE2 IS NULL THEN 0
                                            ELSE IF(IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0) < 0,
                                                0,
                                                IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0))
                                        END + CASE
                                            WHEN B_CREDIT IS NULL THEN 0
                                            ELSE IF(IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0) < 0,
                                                0,
                                                IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0))
                                        END + CASE
                                            WHEN HFSA.CREDIT IS NULL THEN 0
                                            ELSE IF(IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0) < 0,
                                                0,
                                                IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0))
                                        END) AS TOTAL_DUE_AMOUNT
                                    FROM
                                        STU_PERSONAL_INFO AS SPI
                                            INNER JOIN
                                        STU_FEE_DUE_INFO AS SFD ON SPI.STUDENT_ID = SFD.STUDENT_ID
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = SFD.ACADEMIC_YEAR
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            LEFT JOIN
                                        (SELECT 
                                            STUDENT_ID,
                                                SUM(IFNULL(CREDIT, '0')) AS CREDIT,
                                                SUM(IFNULL(DEBIT, '0')) AS DEBIT
                                        FROM
                                            HOSTEL_FEE_STUDENT_ACCOUNT AS hf
                                        WHERE
                                            hf.IS_DELETED != 1
                                                AND hf.ACADEMIC_YEAR = ?AC_YEAR
                                        GROUP BY STUDENT_ID) AS HFSA ON HFSA.STUDENT_ID = SPI.STUDENT_ID
                                            LEFT JOIN
                                        (SELECT 
                                            STUDENT_ID,
                                                SUM(IFNULL(CREDIT, '0')) AS B_CREDIT,
                                                SUM(IFNULL(DEBIT, '0')) AS B_DEBIT
                                        FROM
                                            BUS_FEE_STUDENT_ACCOUNT AS Bf
                                        WHERE
                                            Bf.IS_DELETED != 1
                                                AND Bf.ACADEMIC_YEAR = ?AC_YEAR
                                        GROUP BY STUDENT_ID) AS BFSA ON BFSA.STUDENT_ID = SPI.STUDENT_ID
                                    WHERE
                                        SFD.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SPI.ROLL_NO = ?ROLL_NO
                                            AND SC.IS_DELETED != 1
                                            AND SFD.IS_DELETED != 1
                                            AND SPI.IS_DELETED != 1
                                    GROUP BY SFD.STUDENT_ID , sfd.ACADEMIC_YEAR HAVING TOTAL_DUE_AMOUNT>0
                                    ORDER BY ROLL_NO , SC.CLASS_ID;";
                        break;
                    }
                case FeeSQLCommands.DeletedStuLateEntrybyId:
                    {
                        query = @"UPDATE STU_LATE_ENTRY 
                                                    SET 
                                                        IS_DELETED = ?IS_DELETED
                                                    WHERE
                                                        LATE_FEE_ID = ?LATE_FEE_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchStudentAccountHeadWiseFeeInfo:
                    {
                        query = @"SELECT 
                                    FMH.FREQUENCY_ID,
                                    FMH.BANK_ACCOUNT_ID,
                                    FS.STUDENT_ID,
                                    SUM((IFNULL(FS.CREDIT, 0) - IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    FF.FREQUENCY_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    SFM.MAIN_HEAD AS HEAD,
                                    SP.STU_EMAILID,
                                    SP.STU_MOBILENO,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,FT.FREQUENCY_TYPE as FREQUENCY_NAME,FS.FEE_MAIN_HEAD_ID,FBA.ACCOUNT_LABEL,FS.HEAD AS HEAD_ID
                                FROM
                                    FEE_STUDENT_ACCOUNT AS FS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = FS.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                        AND PG.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FMH ON FMH.APPLICATION_TYPE_ID = PG.APPTYPE_ID
                                        AND FMH.PROGRAMME_MODE = PG.PROGRAMME_MODE
                                        AND FMH.SHIFT = PG.SHIFT
                                        AND FS.HEAD = FMH.HEAD_ID
                                        AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FMH.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                        AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        AND FT.FEE_ROOT_ID = ?FEE_ROOT_ID
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                        LEFT JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                        LEFT JOIN 
                                     fee_eb_accounts AS FBA ON FBA.BANK_ACCOUNT_ID = FMH.BANK_ACCOUNT_ID
                                WHERE
                                    FS.STUDENT_ID = ?STUDENT_ID
                                        AND FT.FREQUENCY_TYPE_ID = ?FREQUENCY_TYPE_ID AND FFF.FEE_FREQUENCY_FEE_MODE_ID= ?FREQUENCY_ID
                                        AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FS.IS_DELETED != 1
                                GROUP BY FS.STUDENT_ID , FS.FREQUENCY_ID, FMH.HEAD_ID
                                ;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFrequencyIdNew:
                    {
                        query = @"SELECT 
    FSA.STUDENT_ID,
    FSA.FREQUENCY_ID,
    SFM.HEAD_ID,
    FSA.HEAD,
    FSA.CREDIT,
    FSA.DEBIT,
    (IFNULL(SUM(FSA.CREDIT), 0) - IFNULL(SUM(FSA.DEBIT), 0)) AS BALANCE,
    FSA.FEE_MAIN_HEAD_ID,
    FS.FEE_STRUCTURE_ID,
    FS.FEE_ROOT_ID,
    FS.AMOUNT,
    SP.STU_MOBILENO,
    SP.STU_EMAILID
FROM FEE_STUDENT_ACCOUNT AS FSA

INNER JOIN STU_CLASS AS SC 
    ON SC.STUDENT_ID = FSA.STUDENT_ID
    AND SC.IS_DELETED != 1

INNER JOIN STU_PERSONAL_INFO AS SP 
    ON SP.STUDENT_ID = FSA.STUDENT_ID

INNER JOIN FEE_STRUCTURE AS FS 
    ON FS.HEAD = FSA.HEAD
    AND FS.ACADEMIC_YEAR =?AC_YEAR
    AND FS.FREQUENCY = FSA.FREQUENCY_ID
    AND FS.FEE_MAIN_HEAD_ID = FSA.FEE_MAIN_HEAD_ID
    AND FS.IS_DELETED != 1

INNER JOIN SUP_HEAD AS SFM 
    ON SFM.HEAD_ID = FSA.HEAD

WHERE 
    FSA.STUDENT_ID = ?STUDENT_ID
    AND FSA.IS_DELETED != 1
    AND FSA.IS_CANCELLED_HEAD != 1
    AND SFM.IS_DELETED != 1
    AND FSA.FREQUENCY_ID = ?FREQUENCY_ID
    AND FSA.ACADEMIC_YEAR = ?AC_YEAR

GROUP BY
    FSA.STUDENT_ID,
    FSA.FREQUENCY_ID,
    SFM.HEAD_ID,
    SFM.HEAD,
    FSA.CREDIT,
    FSA.DEBIT,
    FSA.FEE_MAIN_HEAD_ID,
    FS.FEE_STRUCTURE_ID,
    FS.FEE_ROOT_ID,
    FS.AMOUNT,
    SP.STU_MOBILENO,
    SP.STU_EMAILID;";
                        break;
                    }
                case FeeSQLCommands.SaveCFFeeTransaction:
                    {
                        query = @"INSERT INTO FEE_TRANSACTION
                                        (
                                        STUDENT_ID,
                                        FREQUENCY,
                                        CLASS,
                                        PAYMENT_DATE,
                                        RECEIPT_NO,
                                        PAYMENT_MODE,
                                        COLLECTED,
                                        USERNAME,
                                        ACADEMIC_YEAR,
                                        FREQUENCY_TO,CF_ORDER_ID,PAYURESPONSE_ID,RAZORPAY_ID)
                                        VALUES
                                        (
                                        ?STUDENT_ID,
                                        ?FREQUENCY,
                                        ?CLASS,
                                        ?PAYMENT_DATE,
                                        ?RECEIPT_NO,
                                        ?PAYMENT_MODE,
                                        ?COLLECTED,
                                        ?USERNAME,
                                        ?AC_YEAR,
                                        ?FREQUENCY_TO,?CF_ORDER_ID,?PAYURESPONSE_ID,?RAZORPAY_ID);
                                        ";
                        break;
                    }
                case FeeSQLCommands.FetchTransactionByCashFreeId:
                    {
                        query = @"SELECT 
                                `TRANSACTION_ID`,
                                    `STUDENT_ID`,
                                    `FREQUENCY`,
                                    `CLASS`,
                                    `PAYMENT_DATE`,
                                    `RECEIPT_NO`,
                                    `PAYMENT_MODE`,
                                    `DD_CHEQUE_NO`,
                                    `COLLECTED`,
                                    `DISCOUNT`,
                                    `DEDUCT_STUDENT_ACCOUNT`,
                                    `RECEIPT_NARRATION`,
                                    `USERNAME`,
                                    `ACADEMIC_YEAR`,
                                    `IS_DELETED`,
                                    `IS_AMOUNT_COLLECTED`,
                                    `IS_ACCOUNTANT_COLLECTED`,
                                    `IS_ADVANCE`,
                                    `UPLOAD_FLAG`,
                                    `IS_DOWNLOADED`,
                                    `IS_UPDATED`,
                                    `DOWNLOAD_TIME`,
                                    `FEE_TRANSACTION_COUNTER`,
                                    `FEE_TRANSACTION_BANK`,
                                    `TEMP_ID`,
                                    `EXCESS_AMT`,
                                    `F_TRANSACTION_ID`,
                                    `FREQUENCY_TO`,
                                    `CHALLAN_NO`,
                                    `PayUResponse_Id`,
                                    `RAZORPAY_ID`,
                                    `CF_ORDER_ID`
                                FROM `FEE_TRANSACTION` WHERE  CF_ORDER_ID=?CF_ORDER_ID AND FREQUENCY=?FREQUENCY AND ACADEMIC_YEAR=?AC_YEAR ;";
                        break;
                    }

                case FeeSQLCommands.FetchLateEntryStudentDateWiseFormentor:
                    {
                        query = @"SELECT 
                                    SPI.ROLL_NO,
                                    CONCAT(
                                        IFNULL(SPI.FIRST_NAME, ''), ' ',
                                        IFNULL(SPI.LAST_NAME, '')
                                    ) AS FIRST_NAME,
                                    STF.STAFF_CODE,
                                    CONCAT(
                                        IFNULL(STF.FIRST_NAME, ''), ' ',
                                        IFNULL(STF.LAST_NAME, '')
                                    ) AS STAFF_NAME,
                                    DATE_FORMAT(SL.ENTRY_DATE, '%d/%m/%Y %H:%i:%s') AS ENTRY_DATE,
                                    SL.FINE_AMOUNT,
                                    CC.CLASS_NAME,
                                    CC.CLASS_CODE,
                                    SS.SHIFT_NAME,
                                    SPI.STU_MOBILENO
                                FROM STU_LATE_ENTRY SL
                                INNER JOIN STU_PERSONAL_INFO SPI 
                                    ON SPI.STUDENT_ID = SL.STUDENT_ID
                                    AND SPI.IS_DELETED != 1
                                    AND SPI.IS_LEFT != 1
                                LEFT JOIN STF_PERSONAL_INFO STF 
                                    ON STF.STAFF_ID = SL.ENTRY_ID
                                    AND STF.IS_DELETED != 1
                                INNER JOIN stu_class SC 
                                    ON SC.STUDENT_ID = SPI.STUDENT_ID
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SC.IS_DELETED != 1
                                INNER JOIN CP_CLASSES CC 
                                    ON CC.CLASS_ID = SC.CLASS_ID
                                INNER JOIN sup_shift SS 
                                    ON SS.SHIFT_ID = CC.SHIFT
                                WHERE 
                                    SL.IS_DELETED != 1
                                    AND SL.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SL.STUDENT_ID = ?STUDENT_ID
                                    AND DATE(SL.ENTRY_DATE) BETWEEN ?FROM_DATE AND ?TODATE
                                ORDER BY 
                                    CC.CLASS_ID,
                                    SPI.ROLL_NO,
                                    SL.ENTRY_DATE;
                                ";
                        break;
                    }
                case FeeSQLCommands.FetchHostelFeeDetailsSjctnc:
                    {
                        query = @"SELECT 
                                    HFS.CREDIT,
                                    HFS.F_STUDENT_AC_ID,
                                    DEBIT,
                                    SPI.STUDENT_ID,
                                    SPI.FIRST_NAME,
                                    SPI.ROLL_NO,
                                    sh.HEAD,
                                    sh.HEAD_CODE,
                                    SUM(IFNULL(CREDIT, '0') - IFNULL(DEBIT, '0')) AS BALANCE
                                FROM
                                    FEE_STUDENT_ACCOUNT AS HFS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = HFS.STUDENT_ID
                                        INNER JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = HFS.HEAD
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON HFS.FREQUENCY_ID = FFF.FEE_FREQUENCY_FEE_MODE_ID
                                WHERE
                                    hfs.IS_DELETED != 1
                                        AND HFS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND fff.FEE_MODE = ?FREQUENCY_TYPE
                                GROUP BY HFS.STUDENT_ID , HFS.HEAD
                                ORDER BY ROLL_NO , sh.HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchHostelFeeByIdSjctnc:
                    {
                        query = @"SELECT 
                                    F_STUDENT_AC_ID,
                                    spi.STUDENT_ID,
                                    spi.ACADEMIC_YEAR,
                                    CREDIT,
                                    DEBIT,
                                    spi.FIRST_NAME,
                                    spi.ROLL_NO
                                FROM
                                    FEE_STUDENT_ACCOUNT AS HFS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = HFS.STUDENT_ID
                                WHERE
                                    F_STUDENT_AC_ID = ?F_STUDENT_AC_ID;";
                        break;
                    }
                case FeeSQLCommands.UpdateHostelFeeAccountByIDSjctnc:
                    {
                        query = @"UPDATE fee_student_account SET 
                                                          CREDIT =?CREDIT,
                                                          DEBIT =?DEBIT,
                                                          ENTRY_DATE=?ENTRY_DATE,
                                                          ENTRY_ID=?ENTRY_ID
                                                      where
                                                          F_STUDENT_AC_ID =?F_STUDENT_AC_ID;";
                        break;
                    }
                case FeeSQLCommands.DeletedHostelFeeAccountByIDSjctnc:
                    {
                        query = @"UPDATE fee_student_account  SET IS_DELETED =?IS_DELETED WHERE F_STUDENT_AC_ID=?F_STUDENT_AC_ID AND (DEBIT IS NULL OR DEBIT = 0);";
                        break;
                    }
                case FeeSQLCommands.DeleteAllUnpaidHostelFeeForStudent:
                    {
                        query = @"UPDATE fee_student_account 
                                  SET IS_DELETED = 1 
                                  WHERE STUDENT_ID = ?STUDENT_ID 
                                    AND FREQUENCY_ID IN (?FREQUENCY_ID) 
                                    AND ACADEMIC_YEAR = ?AC_YEAR 
                                    AND (DEBIT IS NULL OR DEBIT = 0);";
                        break;
                    }
                case FeeSQLCommands.FetchHostelFeeStudentAccountInfo:
                    {
                        query = @"SELECT 
                                        FMH.FREQUENCY_ID,
                                        FS.STUDENT_ID,
                                        SUM((IFNULL(FS.CREDIT, 0) - IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                        FF.FREQUENCY_NAME,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CONCAT(IFNULL(H.HEAD, ''),
                                                ' (',
                                                IFNULL(H.HEAD_CODE, ''),
                                                ') -',
                                                FS.ACADEMIC_YEAR) AS HEAD,
                                        FT.FREQUENCY_TYPE_ID,
                                        FMH.FEE_ROOT_ID,
                                        FMH.BANK_ACCOUNT_ID,
                                        FS.ACADEMIC_YEAR,
                                        SP.STU_EMAILID,
                                        SP.STU_MOBILENO,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        FT.FREQUENCY_TYPE AS FREQUENCY_NAME,
                                        FS.FEE_MAIN_HEAD_ID,
                                        FBA.ACCOUNT_LABEL,
                                        FS.HEAD AS HEAD_ID
                                    FROM
                                        FEE_STUDENT_ACCOUNT AS FS
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = FS.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        FEE_MAIN_HEADS AS FMH ON FS.HEAD = FMH.HEAD_ID
                                            AND FMH.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                            AND FMH.IS_DELETED != 1
                                            INNER JOIN
                                        FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                            AND FFF.ACADEMIC_YEAR = FMH.ACADEMIC_YEAR
                                            AND FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                            AND FT.FEE_ROOT_ID = ?FEE_ROOT_ID
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                            LEFT JOIN
                                        SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                            LEFT JOIN
                                        SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                            LEFT JOIN
                                        fee_eb_accounts AS FBA ON FBA.BANK_ACCOUNT_ID = FMH.BANK_ACCOUNT_ID
                                    WHERE
                                        FS.STUDENT_ID = ?STUDENT_ID
                                            AND FT.FREQUENCY_TYPE_ID = ?FREQUENCY_TYPE_ID
                                            AND FFF.FEE_FREQUENCY_FEE_MODE_ID = ?FREQUENCY_ID
                                            AND FS.IS_DELETED != 1
                                            AND FMH.HEAD_ID = ?HEAD_ID
                                    GROUP BY FS.STUDENT_ID , FS.FREQUENCY_ID , FMH.HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchHostelFeeStudentAccountInfoWithoutHead:
                    {
                        query = @"SELECT 
                                        FMH.FREQUENCY_ID,
                                        FS.STUDENT_ID,
                                        SUM((IFNULL(FS.CREDIT, 0) - IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                        FF.FREQUENCY_NAME,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CONCAT(IFNULL(H.HEAD, ''),
                                                ' (',
                                                IFNULL(H.HEAD_CODE, ''),
                                                ') -',
                                                FS.ACADEMIC_YEAR) AS HEAD,
                                        FT.FREQUENCY_TYPE_ID,
                                        FMH.FEE_ROOT_ID,
                                        FS.ACADEMIC_YEAR,
                                        SP.STU_EMAILID,
                                        SP.STU_MOBILENO,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        FT.FREQUENCY_TYPE AS FREQUENCY_NAME,
                                        FS.FEE_MAIN_HEAD_ID,
                                        FMH.BANK_ACCOUNT_ID,
                                        FBA.ACCOUNT_LABEL,
                                        FS.HEAD AS HEAD_ID
                                    FROM
                                        FEE_STUDENT_ACCOUNT AS FS
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = FS.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        FEE_MAIN_HEADS AS FMH ON FS.HEAD = FMH.HEAD_ID
                                            AND FMH.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                            AND FMH.IS_DELETED != 1
                                            INNER JOIN
                                        FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                            AND FFF.ACADEMIC_YEAR = FMH.ACADEMIC_YEAR
                                            AND FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS FT ON FT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                            AND FT.FEE_ROOT_ID = ?FEE_ROOT_ID
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                            LEFT JOIN
                                        SUP_FEE_MAIN_HEAD AS SFM ON SFM.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                            LEFT JOIN
                                        SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
                                            LEFT JOIN
                                        fee_eb_accounts AS FBA ON FBA.BANK_ACCOUNT_ID = FMH.BANK_ACCOUNT_ID
                                    WHERE
                                        FS.STUDENT_ID = ?STUDENT_ID
                                            AND FT.FREQUENCY_TYPE_ID = ?FREQUENCY_TYPE_ID
                                            AND FFF.FEE_FREQUENCY_FEE_MODE_ID = ?FREQUENCY_ID
                                            AND FS.IS_DELETED != 1
                                           -- AND FMH.HEAD_ID = ?HEAD_ID
                                    GROUP BY FS.STUDENT_ID , FS.FREQUENCY_ID , FMH.HEAD_ID;";
                        break;
                    }
                case FeeSQLCommands.FetchFeeMainHeadbyFeeMode:
                    {
                        query = @"SELECT 
                                      FEE_MAIN_HEAD_ID
                                  FROM
                                      FEE_MAIN_HEADS AS FMH
                                          INNER JOIN
                                      FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                  WHERE
                                      FFF.FEE_MODE = ?FEE_MODE AND FFF.HEAD_ID = ?HEAD_ID and FFF.ACADEMIC_YEAR
                                  LIMIT 1;";
                        break;
                    }
                case FeeSQLCommands.FetchFrequencyByFeeModeAndYear:
                    {
                        query = @"SELECT 
                                    FREQUENCY_ID
                                FROM
                                    FEE_FREQUENCY_FEE_MODE
                                WHERE
                                    FEE_MODE = ?FEE_MODE AND ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                ORDER BY FREQUENCY_ID DESC;";
                        break;
                    }
                case FeeSQLCommands.FetchBusList:
                    {
                        query = "SELECT BUS_ID, BUS_NAME, DRIVER_NAME, ROUTE, IS_ACIVE, IS_DELETED, ALLOTED_SEATS, IF(IS_ACIVE=1, 'Active', 'Inactive') AS ACTIVE_STATUS FROM sup_bus WHERE IS_DELETED = 0";
                        break;
                    }
                case FeeSQLCommands.SaveBus:
                    {
                        query = "INSERT INTO sup_bus (BUS_NAME, DRIVER_NAME, ROUTE, ALLOTED_SEATS, IS_ACIVE, IS_DELETED) VALUES (?BUS_NAME, ?DRIVER_NAME, ?ROUTE, ?ALLOTED_SEATS, ?IS_ACIVE, 0)";
                        break;
                    }
                case FeeSQLCommands.UpdateBus:
                    {
                        query = "UPDATE sup_bus SET BUS_NAME=?BUS_NAME, DRIVER_NAME=?DRIVER_NAME, ROUTE=?ROUTE, ALLOTED_SEATS=?ALLOTED_SEATS, IS_ACIVE=?IS_ACIVE WHERE BUS_ID=?BUS_ID";
                        break;
                    }
                case FeeSQLCommands.DeleteBus:
                    {
                        query = "UPDATE sup_bus SET IS_DELETED=1 WHERE BUS_ID=?BUS_ID";
                        break;
                    }
                case FeeSQLCommands.FetchBoardingStages:
                    {
                        query = @"SELECT BS.BUS_STAGE_ID, BS.STAGE_NAME, BS.KM, BS.BUS_ID, BS.ALLOTED_SEATS, BS.AMOUNT, BS.AMOUNT_II, 
                                         BS.ACADEMIC_YEAR, BS.IS_ACTIVE, BS.IS_DELETED, B.BUS_NAME, IF(BS.IS_ACTIVE=1, 'Active', 'Inactive') AS ACTIVE_STATUS 
                                  FROM transport_bus_stage BS 
                                  INNER JOIN sup_bus B ON B.BUS_ID = BS.BUS_ID 
                                  WHERE BS.IS_DELETED !=1";
                        break;
                    }
                case FeeSQLCommands.SaveBoardingStage:
                    {
                        query = "INSERT INTO transport_bus_stage (STAGE_NAME, KM, BUS_ID, ALLOTED_SEATS, AMOUNT, AMOUNT_II, ACADEMIC_YEAR, IS_ACTIVE, IS_DELETED) VALUES (?STAGE_NAME, ?KM, ?BUS_ID, ?ALLOTED_SEATS, ?AMOUNT, ?AMOUNT_II, ?ACADEMIC_YEAR, ?IS_ACTIVE, 0)";
                        break;
                    }
                case FeeSQLCommands.UpdateBoardingStage:
                    {
                        query = "UPDATE transport_bus_stage SET STAGE_NAME=?STAGE_NAME, KM=?KM, BUS_ID=?BUS_ID, ALLOTED_SEATS=?ALLOTED_SEATS, AMOUNT=?AMOUNT, AMOUNT_II=?AMOUNT_II, ACADEMIC_YEAR=?ACADEMIC_YEAR, IS_ACTIVE=?IS_ACTIVE WHERE BUS_STAGE_ID=?BUS_STAGE_ID";
                        break;
                    }
                case FeeSQLCommands.DeleteBoardingStage:
                    {
                        query = "UPDATE transport_bus_stage SET IS_DELETED=1 WHERE BUS_STAGE_ID=?BUS_STAGE_ID";
                        break;
                    }
                case FeeSQLCommands.FetchBusFrequencies:
                    {
                        query = @"SELECT FFF.FEE_FREQUENCY_FEE_MODE_ID AS FEE_FREQUENCY_FEE_MODE_ID, SFF.FREQUENCY_NAME AS FREQUENCY_NAME, FFF.FREQUENCY_ID AS FREQUENCY_ID
                                  FROM FEE_FREQUENCY_FEE_MODE AS FFF
                                  INNER JOIN SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                  WHERE FFF.FEE_MODE = '8' AND FFF.ACADEMIC_YEAR = ?ACADEMIC_YEAR";
                        break;
                    }
                case FeeSQLCommands.FetchBusMainHeads:
                    {
                        query = @"SELECT MH.FEE_MAIN_HEAD_ID, MH.MAIN_HEAD_ID, SMH.MAIN_HEAD 
                                  FROM FEE_MAIN_HEADS AS MH 
                                  INNER JOIN SUP_FEE_MAIN_HEAD AS SMH ON SMH.MAIN_HEAD_ID = MH.MAIN_HEAD_ID 
                                  WHERE MH.ACADEMIC_YEAR = ?ACADEMIC_YEAR AND MH.FREQUENCY_ID = ?FREQUENCY_ID GROUP BY MH.MAIN_HEAD_ID";
                        break;
                    }
                case FeeSQLCommands.FetchRegisteredStudentsForBusStage:
                    {
                        query = @"SELECT TR.BUS_REGISTERED_ID, TR.STAGE_ID, TR.STUDENT_ID, TR.STATUS, TR.ACADEMIC_YEAR, TR.IS_ACTIVE, 
                                         SPI.ROLL_NO, SPI.FIRST_NAME, BS.STAGE_NAME, B.BUS_NAME, CC.CLASS_NAME, SPI.ADMISSION_NO, BS.AMOUNT
                                  FROM transport_bus_registration TR 
                                  INNER JOIN stu_personal_info SPI ON SPI.STUDENT_ID = TR.STUDENT_ID 
                                  INNER JOIN transport_bus_stage BS ON BS.BUS_STAGE_ID = TR.STAGE_ID 
                                  INNER JOIN sup_bus B ON B.BUS_ID = BS.BUS_ID 
                                  LEFT JOIN stu_class SC ON SC.STUDENT_ID = TR.STUDENT_ID AND SC.ACADEMIC_YEAR = TR.ACADEMIC_YEAR
                                  LEFT JOIN cp_classes CC ON CC.CLASS_ID = SC.CLASS_ID 
                                  WHERE TR.IS_DELETED !=1 AND TR.IS_ACTIVE = 1 AND BS.IS_DELETED !=1 AND B.IS_DELETED !=1
                                    AND TR.STAGE_ID = ?STAGE_ID AND TR.ACADEMIC_YEAR = ?ACADEMIC_YEAR";
                        break;
                    }
                case FeeSQLCommands.CheckExistingFeeStudentAccountForBus:
                    {
                        query = "SELECT F_STUDENT_AC_ID FROM fee_student_account WHERE STUDENT_ID = ?STUDENT_ID AND ACADEMIC_YEAR = ?ACADEMIC_YEAR AND FREQUENCY_ID = ?FREQUENCY_ID AND HEAD = '137' AND IS_DELETED !=1";
                        break;
                    }
                case FeeSQLCommands.SaveBusFeeStudentAccount:
                    {
                        query = "INSERT INTO fee_student_account (STUDENT_ID, ACADEMIC_YEAR, FREQUENCY_ID, HEAD, CREDIT, FEE_MAIN_HEAD_ID, IS_DELETED) VALUES (?STUDENT_ID, ?ACADEMIC_YEAR, ?FREQUENCY_ID, ?HEAD, ?CREDIT, ?FEE_MAIN_HEAD_ID, 0)";
                        break;
                    }
                case FeeSQLCommands.FetchAllActiveBusRegistrations:
                    {
                        query = "SELECT STUDENT_ID, STAGE_ID, ACADEMIC_YEAR FROM transport_bus_registration WHERE IS_DELETED !=1 AND IS_ACTIVE = 1";
                        break;
                    }
                case FeeSQLCommands.FetchAllStudentsPersonalInfo:
                    {
                        query = "SELECT STUDENT_ID, ROLL_NO, ADMISSION_NO, FIRST_NAME FROM stu_personal_info WHERE IS_DELETED <> 1";
                        break;
                    }
                case FeeSQLCommands.FetchAllBusFrequencies:
                    {
                        query = @"SELECT FFF.FEE_FREQUENCY_FEE_MODE_ID AS FEE_FREQUENCY_FEE_MODE_ID, SFF.FREQUENCY_NAME AS FREQUENCY_NAME, FFF.FREQUENCY_ID AS FREQUENCY_ID, FFF.ACADEMIC_YEAR
                                  FROM FEE_FREQUENCY_FEE_MODE AS FFF
                                  INNER JOIN SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                  WHERE FFF.FEE_MODE = '8'";
                        break;
                    }
                case FeeSQLCommands.FetchAllBusMainHeads:
                    {
                        query = "SELECT FEE_MAIN_HEAD_ID, FREQUENCY_ID, ACADEMIC_YEAR FROM FEE_MAIN_HEADS WHERE HEAD_ID = '137'";
                        break;
                    }
                case FeeSQLCommands.FetchAllBusFeeStudentAccounts:
                    {
                        query = "SELECT F_STUDENT_AC_ID, STUDENT_ID, ACADEMIC_YEAR, FREQUENCY_ID FROM fee_student_account WHERE HEAD = '137' AND IS_DELETED !=1";
                        break;
                    }
                case FeeSQLCommands.FetchHostelStudentPaidReportDateWise:
                    {
                        query = @"SELECT 
                                    FT.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    UPPER(CONCAT(IFNULL(SPI.FIRST_NAME, ''), ' ', IFNULL(SPI.LAST_NAME, ''))) AS FIRST_NAME,
                                    CLS.CLASS_NAME,
                                    H.HOSTEL_NAME,
                                    HS.ROOM_NO,
                                    SH.HEAD_CODE AS HEAD_CODE,
                                    SH.HEAD AS HEAD_NAME,
                                    FFF.FREQUENCY_ID,
                                    FF.FREQUENCY_NAME,
                                    FC.PAID_AMOUNT AS DEBIT,
                                    0 AS CREDIT,
                                    0 AS BALANCE,
                                    DATE_FORMAT(FT.PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,
                                    FT.RECEIPT_NO AS RECEIPT_NO,
                                    PM.PAYMENT_MODE AS PAYMENT_MODE,
                                    SC.ACADEMIC_YEAR
                                FROM
                                    FEE_TRANSACTION AS FT
                                        INNER JOIN
                                    FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = FT.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                        LEFT JOIN
                                    HOSTEL_STUDENT_INFO AS HS ON HS.STUDENT_ID = SPI.STUDENT_ID AND HS.ACADEMIC_YEAR = SC.ACADEMIC_YEAR AND HS.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_HOSTEL AS H ON H.HOSTEL_ID = HS.HOSTEL_ID
                                        INNER JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FC.HEAD
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FT.FREQUENCY AND FFF.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FEE_ROOT_ID = '2'
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_PAYMENT_MODE AS PM ON PM.PAYMENT_MODE_ID = FT.PAYMENT_MODE
                                WHERE
                                    FT.IS_DELETED != 1
                                    AND FC.PAID_AMOUNT > 0
                                    AND FT.PAYMENT_DATE BETWEEN ?DATE_FROM AND ?DATE_TO
                                    ?FREQUENCY_CONDITION
                                    ?HEAD_CONDITION
                                    ?HOSTEL_CONDITION
                                ORDER BY FT.PAYMENT_DATE, SPI.ROLL_NO, FF.FREQUENCY_NAME, SH.HEAD;";
                        break;
                    }
                case FeeSQLCommands.FetchHostelStudentBalanceReport:
                    {
                        query = @"SELECT 
                                    FS.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    UPPER(CONCAT(IFNULL(SPI.FIRST_NAME, ''), ' ', IFNULL(SPI.LAST_NAME, ''))) AS FIRST_NAME,
                                    CLS.CLASS_NAME,
                                    H.HOSTEL_NAME,
                                    HS.ROOM_NO,
                                    SH.HEAD_CODE AS HEAD_CODE,
                                    SH.HEAD AS HEAD_NAME,
                                    FFF.FREQUENCY_ID,
                                    FF.FREQUENCY_NAME,
                                    IFNULL((
                                        SELECT SUM(IFNULL(FC.PAID_AMOUNT, 0)) 
                                        FROM FEE_TRANSACTION AS FT 
                                        INNER JOIN FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID 
                                        WHERE FT.STUDENT_ID = FS.STUDENT_ID 
                                          AND FT.FREQUENCY = FS.FREQUENCY_ID 
                                          AND FC.HEAD = FS.HEAD 
                                          AND FT.IS_DELETED != 1
                                    ), 0) AS DEBIT,
                                    FS.CREDIT AS CREDIT,
                                    (IFNULL(FS.CREDIT, 0) - IFNULL((
                                        SELECT SUM(IFNULL(FC.PAID_AMOUNT, 0)) 
                                        FROM FEE_TRANSACTION AS FT 
                                        INNER JOIN FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID 
                                        WHERE FT.STUDENT_ID = FS.STUDENT_ID 
                                          AND FT.FREQUENCY = FS.FREQUENCY_ID 
                                          AND FC.HEAD = FS.HEAD 
                                          AND FT.IS_DELETED != 1
                                    ), 0)) AS BALANCE,
                                    FS.ACADEMIC_YEAR
                                FROM
                                    fee_student_account AS FS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = FS.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID AND SC.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                        LEFT JOIN
                                    HOSTEL_STUDENT_INFO AS HS ON HS.STUDENT_ID = SPI.STUDENT_ID AND HS.ACADEMIC_YEAR = FS.ACADEMIC_YEAR AND HS.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_HOSTEL AS H ON H.HOSTEL_ID = HS.HOSTEL_ID
                                        INNER JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FS.HEAD
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID AND FFF.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FEE_ROOT_ID = '2'
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                WHERE
                                    FS.IS_DELETED != 1
                                    AND (IFNULL(FS.CREDIT, 0) - IFNULL((
                                        SELECT SUM(IFNULL(FC.PAID_AMOUNT, 0)) 
                                        FROM FEE_TRANSACTION AS FT 
                                        INNER JOIN FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID 
                                        WHERE FT.STUDENT_ID = FS.STUDENT_ID 
                                          AND FT.FREQUENCY = FS.FREQUENCY_ID 
                                          AND FC.HEAD = FS.HEAD 
                                          AND FT.IS_DELETED != 1
                                    ), 0)) > 0
                                    AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                    ?FREQUENCY_CONDITION
                                    ?HEAD_CONDITION
                                    ?HOSTEL_CONDITION
                                ORDER BY SPI.ROLL_NO, FF.FREQUENCY_NAME, SH.HEAD;";
                        break;
                    }
                case FeeSQLCommands.FetchHostelStudentFrequencyWiseReport:
                    {
                        query = @"SELECT 
                                    FS.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    UPPER(CONCAT(IFNULL(SPI.FIRST_NAME, ''), ' ', IFNULL(SPI.LAST_NAME, ''))) AS FIRST_NAME,
                                    CLS.CLASS_NAME,
                                    H.HOSTEL_NAME,
                                    HS.ROOM_NO,
                                    SH.HEAD_CODE AS HEAD_CODE,
                                    FFF.FREQUENCY_ID,
                                    FF.FREQUENCY_NAME,
                                    MAX(IFNULL(FS.CREDIT, 0)) AS CREDIT,
                                    MAX(IFNULL((
                                        SELECT SUM(IFNULL(FC.PAID_AMOUNT, 0)) 
                                        FROM FEE_TRANSACTION AS FT 
                                        INNER JOIN FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID 
                                        WHERE FT.STUDENT_ID = FS.STUDENT_ID 
                                          AND FT.FREQUENCY = FS.FREQUENCY_ID 
                                          AND FC.HEAD = FS.HEAD 
                                          AND FT.IS_DELETED != 1
                                    ), 0)) AS DEBIT,
                                    (MAX(IFNULL(FS.CREDIT, 0)) - MAX(IFNULL((
                                        SELECT SUM(IFNULL(FC.PAID_AMOUNT, 0)) 
                                        FROM FEE_TRANSACTION AS FT 
                                        INNER JOIN FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID 
                                        WHERE FT.STUDENT_ID = FS.STUDENT_ID 
                                          AND FT.FREQUENCY = FS.FREQUENCY_ID 
                                          AND FC.HEAD = FS.HEAD 
                                          AND FT.IS_DELETED != 1
                                    ), 0))) AS BALANCE,
                                    FS.ACADEMIC_YEAR
                                FROM
                                    fee_student_account AS FS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = FS.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID AND SC.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                        LEFT JOIN
                                    HOSTEL_STUDENT_INFO AS HS ON HS.STUDENT_ID = SPI.STUDENT_ID AND HS.ACADEMIC_YEAR = FS.ACADEMIC_YEAR AND HS.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_HOSTEL AS H ON H.HOSTEL_ID = HS.HOSTEL_ID
                                        INNER JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FS.HEAD
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID AND FFF.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FEE_ROOT_ID = '2'
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                WHERE
                                    FS.IS_DELETED != 1
                                    AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                    ?FREQUENCY_CONDITION
                                    ?HOSTEL_CONDITION
                                GROUP BY FS.STUDENT_ID, FFF.FREQUENCY_ID, FS.HEAD
                                ORDER BY SPI.ROLL_NO, FF.FREQUENCY_NAME;";
                        break;
                    }
                case FeeSQLCommands.FetchHostelStudentFrequencyAndHeadWiseReport:
                    {
                        query = @"SELECT 
                                    FS.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    UPPER(CONCAT(IFNULL(SPI.FIRST_NAME, ''), ' ', IFNULL(SPI.LAST_NAME, ''))) AS FIRST_NAME,
                                    CLS.CLASS_NAME,
                                    H.HOSTEL_NAME,
                                    HS.ROOM_NO,
                                    SH.HEAD_CODE AS HEAD_CODE,
                                    SH.HEAD AS HEAD_NAME,
                                    FFF.FREQUENCY_ID,
                                    FF.FREQUENCY_NAME,
                                    IFNULL((
                                        SELECT SUM(IFNULL(FC.PAID_AMOUNT, 0)) 
                                        FROM FEE_TRANSACTION AS FT 
                                        INNER JOIN FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID 
                                        WHERE FT.STUDENT_ID = FS.STUDENT_ID 
                                          AND FT.FREQUENCY = FS.FREQUENCY_ID 
                                          AND FC.HEAD = FS.HEAD 
                                          AND FT.IS_DELETED != 1
                                    ), 0) AS DEBIT,
                                    FS.CREDIT AS CREDIT,
                                    (IFNULL(FS.CREDIT, 0) - IFNULL((
                                        SELECT SUM(IFNULL(FC.PAID_AMOUNT, 0)) 
                                        FROM FEE_TRANSACTION AS FT 
                                        INNER JOIN FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID 
                                        WHERE FT.STUDENT_ID = FS.STUDENT_ID 
                                          AND FT.FREQUENCY = FS.FREQUENCY_ID 
                                          AND FC.HEAD = FS.HEAD 
                                          AND FT.IS_DELETED != 1
                                    ), 0)) AS BALANCE,
                                    NULL AS PAYMENT_DATE,
                                    FS.ACADEMIC_YEAR
                                FROM
                                    fee_student_account AS FS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = FS.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID AND SC.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                        LEFT JOIN
                                    HOSTEL_STUDENT_INFO AS HS ON HS.STUDENT_ID = SPI.STUDENT_ID AND HS.ACADEMIC_YEAR = FS.ACADEMIC_YEAR AND HS.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_HOSTEL AS H ON H.HOSTEL_ID = HS.HOSTEL_ID
                                        INNER JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FS.HEAD
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID AND FFF.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FEE_ROOT_ID = '2'
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                WHERE
                                    FS.IS_DELETED != 1
                                    AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                    ?FREQUENCY_CONDITION
                                    ?HEAD_CONDITION
                                    ?HOSTEL_CONDITION
                                ORDER BY SPI.ROLL_NO, FF.FREQUENCY_NAME, SH.HEAD;";
                        break;
                    }
                case FeeSQLCommands.FetchBusFeeStudentPaidReportDateWise:
                    {
                        query = @"SELECT 
                                    FT.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    SPI.REGISTER_NO,
                                    UPPER(CONCAT(IFNULL(SPI.FIRST_NAME, ''), ' ', IFNULL(SPI.LAST_NAME, ''))) AS FIRST_NAME,
                                    CLS.CLASS_NAME,
                                    SPI.STU_MOBILENO AS MOBILE_NO,
                                    SH.HEAD_ID,
                                    SH.HEAD_CODE,
                                    SH.HEAD AS HEAD_NAME,
                                    FFF.FREQUENCY_ID,
                                    FF.FREQUENCY_NAME,
                                    FC.PAID_AMOUNT AS DEBIT,
                                    0 AS CREDIT,
                                    0 AS BALANCE,
                                    DATE_FORMAT(FT.PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,
                                    FT.RECEIPT_NO,
                                    PM.PAYMENT_MODE,
                                    FT.ACADEMIC_YEAR
                                FROM
                                    FEE_TRANSACTION AS FT
                                        INNER JOIN
                                    FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = FT.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FT.FREQUENCY AND FFF.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FREQUENCY_TYPE_ID = '8'
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FC.HEAD
                                        LEFT JOIN
                                    SUP_PAYMENT_MODE AS PM ON PM.PAYMENT_MODE_ID = FT.PAYMENT_MODE
                                WHERE
                                    FT.IS_DELETED != 1
                                    AND FC.PAID_AMOUNT > 0
                                    AND FT.PAYMENT_DATE BETWEEN ?DATE_FROM AND ?DATE_TO
                                    ?FREQUENCY_CONDITION
                                    ?HEAD_CONDITION
                                ORDER BY FT.PAYMENT_DATE, SPI.ROLL_NO, FF.FREQUENCY_NAME;";
                        break;
                    }
                case FeeSQLCommands.FetchBusFeeStudentBalanceReport:
                    {
                        query = @"SELECT 
                                    FS.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    SPI.REGISTER_NO,
                                    UPPER(CONCAT(IFNULL(SPI.FIRST_NAME, ''), ' ', IFNULL(SPI.LAST_NAME, ''))) AS FIRST_NAME,
                                    CLS.CLASS_NAME,
                                    SPI.STU_MOBILENO AS MOBILE_NO,
                                    SH.HEAD_ID,
                                    SH.HEAD_CODE,
                                    SH.HEAD AS HEAD_NAME,
                                    FFF.FREQUENCY_ID,
                                    FF.FREQUENCY_NAME,
                                    SUM(IFNULL(FS.CREDIT, 0)) AS CREDIT,
                                    SUM(IFNULL(FS.DEBIT, 0)) AS DEBIT,
                                    (SUM(IFNULL(FS.CREDIT, 0)) - SUM(IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    FS.ACADEMIC_YEAR
                                FROM
                                    FEE_STUDENT_ACCOUNT AS FS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = FS.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID AND SC.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID AND FFF.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FREQUENCY_TYPE_ID = '8'
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FS.HEAD
                                WHERE
                                    FS.IS_DELETED != 1
                                    AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                    ?FREQUENCY_CONDITION
                                    ?HEAD_CONDITION
                                GROUP BY FS.STUDENT_ID, FS.FREQUENCY_ID, FS.HEAD
                                HAVING BALANCE > 0
                                ORDER BY SPI.ROLL_NO, FF.FREQUENCY_NAME;";
                        break;
                    }
                case FeeSQLCommands.FetchBusFeeStudentFrequencyWiseReport:
                    {
                        query = @"SELECT 
                                    FS.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    SPI.REGISTER_NO,
                                    UPPER(CONCAT(IFNULL(SPI.FIRST_NAME, ''), ' ', IFNULL(SPI.LAST_NAME, ''))) AS FIRST_NAME,
                                    CLS.CLASS_NAME,
                                    SPI.STU_MOBILENO AS MOBILE_NO,
                                    SH.HEAD_ID,
                                    SH.HEAD_CODE,
                                    SH.HEAD AS HEAD_NAME,
                                    FFF.FREQUENCY_ID,
                                    FF.FREQUENCY_NAME,
                                    SUM(IFNULL(FS.CREDIT, 0)) AS CREDIT,
                                    SUM(IFNULL(FS.DEBIT, 0)) AS DEBIT,
                                    (SUM(IFNULL(FS.CREDIT, 0)) - SUM(IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    FS.ACADEMIC_YEAR
                                FROM
                                    FEE_STUDENT_ACCOUNT AS FS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = FS.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID AND SC.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID AND FFF.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FREQUENCY_TYPE_ID = '8'
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FS.HEAD
                                WHERE
                                    FS.IS_DELETED != 1
                                    AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                    ?FREQUENCY_CONDITION
                                    ?HEAD_CONDITION
                                GROUP BY FS.STUDENT_ID, FS.FREQUENCY_ID, FS.HEAD
                                ORDER BY SPI.ROLL_NO, FF.FREQUENCY_NAME;";
                        break;
                    }
                case FeeSQLCommands.FetchBusFeeStaffPaidReportDateWise:
                    {
                        query = @"SELECT 
                                    FT.STAFF_ID AS STUDENT_ID,
                                    SP.STAFF_CODE AS ROLL_NO,
                                    SP.STAFF_CODE AS REGISTER_NO,
                                    UPPER(CONCAT(IFNULL(SP.FIRST_NAME, ''), ' ', IFNULL(SP.LAST_NAME, ''))) AS FIRST_NAME,
                                    'Staff' AS CLASS_NAME,
                                    SP.USER_FIELD1 AS MOBILE_NO,
                                    SH.HEAD_ID,
                                    SH.HEAD_CODE,
                                    SH.HEAD AS HEAD_NAME,
                                    FFF.FREQUENCY_ID,
                                    FF.FREQUENCY_NAME,
                                    FC.PAID_AMOUNT AS DEBIT,
                                    0 AS CREDIT,
                                    0 AS BALANCE,
                                    DATE_FORMAT(FT.PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,
                                    FT.RECEIPT_NO,
                                    PM.PAYMENT_MODE,
                                    FT.ACADEMIC_YEAR
                                FROM
                                    staff_fee_transaction AS FT
                                        INNER JOIN
                                    staff_fee_collection AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = FT.STAFF_ID AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FT.FREQUENCY AND FFF.ACADEMIC_YEAR = ?AC_YEAR
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FREQUENCY_TYPE_ID = '8'
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FC.HEAD
                                        LEFT JOIN
                                    SUP_PAYMENT_MODE AS PM ON PM.PAYMENT_MODE_ID = FT.PAYMENT_MODE
                                WHERE
                                    FT.IS_DELETED != 1
                                    AND FC.PAID_AMOUNT > 0
                                    AND FT.PAYMENT_DATE BETWEEN ?DATE_FROM AND ?DATE_TO
                                    ?FREQUENCY_CONDITION
                                    ?HEAD_CONDITION
                                ORDER BY FT.PAYMENT_DATE, SP.STAFF_CODE, FF.FREQUENCY_NAME;";
                        break;
                    }
                case FeeSQLCommands.FetchBusFeeStaffBalanceReport:
                    {
                        query = @"SELECT 
                                    FS.STAFF_ID AS STUDENT_ID,
                                    SP.STAFF_CODE AS ROLL_NO,
                                    SP.STAFF_CODE AS REGISTER_NO,
                                    UPPER(CONCAT(IFNULL(SP.FIRST_NAME, ''), ' ', IFNULL(SP.LAST_NAME, ''))) AS FIRST_NAME,
                                    'Staff' AS CLASS_NAME,
                                    SP.USER_FIELD1 AS MOBILE_NO,
                                    SH.HEAD_ID,
                                    SH.HEAD_CODE,
                                    SH.HEAD AS HEAD_NAME,
                                    FFF.FREQUENCY_ID,
                                    FF.FREQUENCY_NAME,
                                    SUM(IFNULL(FS.CREDIT, 0)) AS CREDIT,
                                    SUM(IFNULL(FS.DEBIT, 0)) AS DEBIT,
                                    (SUM(IFNULL(FS.CREDIT, 0)) - SUM(IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    FS.ACADEMIC_YEAR
                                FROM
                                    fee_staff_account AS FS
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = FS.STAFF_ID AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID AND FFF.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FREQUENCY_TYPE_ID = '8'
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FS.HEAD
                                WHERE
                                    FS.IS_DELETED != 1
                                    AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                    ?FREQUENCY_CONDITION
                                    ?HEAD_CONDITION
                                GROUP BY FS.STAFF_ID, FS.FREQUENCY_ID, FS.HEAD
                                HAVING BALANCE > 0
                                ORDER BY SP.STAFF_CODE, FF.FREQUENCY_NAME;";
                        break;
                    }
                case FeeSQLCommands.FetchBusFeeStaffFrequencyWiseReport:
                    {
                        query = @"SELECT 
                                    FS.STAFF_ID AS STUDENT_ID,
                                    SP.STAFF_CODE AS ROLL_NO,
                                    SP.STAFF_CODE AS REGISTER_NO,
                                    UPPER(CONCAT(IFNULL(SP.FIRST_NAME, ''), ' ', IFNULL(SP.LAST_NAME, ''))) AS FIRST_NAME,
                                    'Staff' AS CLASS_NAME,
                                    SP.USER_FIELD1 AS MOBILE_NO,
                                    SH.HEAD_ID,
                                    SH.HEAD_CODE,
                                    SH.HEAD AS HEAD_NAME,
                                    FFF.FREQUENCY_ID,
                                    FF.FREQUENCY_NAME,
                                    SUM(IFNULL(FS.CREDIT, 0)) AS CREDIT,
                                    SUM(IFNULL(FS.DEBIT, 0)) AS DEBIT,
                                    (SUM(IFNULL(FS.CREDIT, 0)) - SUM(IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    FS.ACADEMIC_YEAR
                                FROM
                                    fee_staff_account AS FS
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = FS.STAFF_ID AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID AND FFF.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FREQUENCY_TYPE_ID = '8'
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FS.HEAD
                                WHERE
                                    FS.IS_DELETED != 1
                                    AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                    ?FREQUENCY_CONDITION
                                    ?HEAD_CONDITION
                                GROUP BY FS.STAFF_ID, FS.FREQUENCY_ID, FS.HEAD
                                ORDER BY SP.STAFF_CODE, FF.FREQUENCY_NAME;";
                        break;
                    }

            }
            return query;
        }
    }
}

