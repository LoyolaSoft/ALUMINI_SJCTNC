using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.Communication
{
    public static class CommunicationSQL
    {
        public static string GetCommunicationSQL(CommunicationSQLCommands sEnumCommand)
        {
            string query = "";

            switch (sEnumCommand)
            {
                case CommunicationSQLCommands.FetchSolutionInfiApi:
                    {
                        query = @"SELECT SMS_SETTING_ID,
                                    BASE_URL,
                                    API_KEY,
                                    METHOD,
                                    FORMAT,
                                    UNICODE,
                                    FLASH,
                                    DLRURL,SENDER,REQUEST_METHOD
                                FROM SMS_SETTING_FOR_SOLUTIONINFI
                                 WHERE 
                                 IS_DELETED!=1
                                AND API_MODE=?API_MODE 
                                AND IS_ACTIVE=1;";
                        break;
                    }
                case CommunicationSQLCommands.UpdateSMSBalance:
                    {
                        query = @"
                                UPDATE SMS_BALANCE
                                        SET
                                        USED_CREDITS = ?USED_CREDITS,
                                        BALANCE_CREDITS = ?BALANCE_CREDITS                                        
                                        WHERE IS_ACTIVE=?IS_ACTIVE ;";
                        break;
                    }
                case CommunicationSQLCommands.SaveSentMessages:
                    {
                        query = @"INSERT INTO SENT_SMS(
                                            CLASS_IDS,
                                            START_TIME,
                                            END_TIME,
                                            CONTENT,
                                            SMS_COUNT,
                                            DATE,
                                            STATUS,
                                            TRANSACTION_ID,
                                            CREDIT_COUNT,
                                            SMS_MODE,
                                            ACADEMIC_YEAR,
                                            RECEIPIENT_NOS)
                                            VALUES
                                            (
                                            ?CLASS_IDS,
                                            ?START_TIME,
                                            ?END_TIME,
                                            ?CONTENT,
                                            ?SMS_COUNT,
                                            current_date(),
                                            ?STATUS,
                                            ?TRANSACTION_ID,
                                            ?CREDIT_COUNT,
                                            ?SMS_MODE,
                                            ?ACADEMIC_YEAR_ID,
                                            ?RECEIPIENT_NOS);";
                        break;
                    }
                case CommunicationSQLCommands.FetchSMSBalance:
                    {
                        query = @"SELECT SMS_BALANCE_ID,
                                    SMS_DATE,
                                    RECEIVED_CREDITS,
                                    USED_CREDITS,
                                    BALANCE_CREDITS,
                                    IS_DELETED,
                                    ACADEMIC_YEAR,
                                    SMS_LIMIT
                                FROM SMS_BALANCE WHERE IS_ACTIVE=1 AND  IS_DELETED!=1;";
                        break;
                    }
                case CommunicationSQLCommands.FetchStudentInfoForIndividualSMS:
                    {
                        query = @"SELECT 
                                        SC.STUDENT_ID AS USER_ID,
                                        SP.ROLL_NO,
                                        CONCAT(IF(SP.FIRST_NAME IS NULL,
                                                    '',
                                                    FIRST_NAME),
                                                ' ',
                                                IF(SP.LAST_NAME IS NULL, '', LAST_NAME)) AS FIRST_NAME,
                                                CONCAT(IFNULL(SP.FR_MOBILE,
                                                        IFNULL(SP.MO_MOBILE,
                                                                IFNULL(SP.STU_MOBILENO, '')))) AS MOBILE,
                                        CLS.CLASS_NAME AS CLASS,
                                        SC.CLASS_ID
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        SP.IS_LEFT != 1 AND SP.IS_DELETED != 1
                                            AND SC.IS_DELETED != 1
                                            AND CLS.IS_DELETED != 1
                                            AND SC.CLASS_ID IN (?CLASS_ID)
                                        ORDER BY SC.CLASS_ID,SP.ROLL_NO;";
                        break;
                    }
                case CommunicationSQLCommands.FetchClasswiseRecordsCount:
                    {
                        query = @"SELECT 
                                COUNT(SPI.STUDENT_ID) AS RECORDSCOUNT
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                    AND SPI.IS_DELETED != 1
                                    AND SPI.IS_LEFT!=1
                            WHERE
                                SC.CLASS_ID IN (?CLASS_ID)
                                    AND SC.IS_DELETED != 1
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case CommunicationSQLCommands.FetchStaffInfoForIndividualSMS:
                    {
                        query = @"SELECT 
                            ST.STAFF_ID AS USER_ID,
                            ST.STAFF_CODE,
                            CONCAT(IF(ST.FIRST_NAME IS NULL,
                                        '',
                                        FIRST_NAME),
                                    ' ',
                                    IF(ST.LAST_NAME IS NULL, '', LAST_NAME)) AS FIRST_NAME,
                            SA.CCELL_NO AS MOBILE,
                            '' AS CLASS,
                            '' AS CLASS_ID
                        FROM
                            stf_personal_info AS ST
                                LEFT JOIN
                            stf_address AS SA ON SA.STAFFNO = ST.STAFF_ID
                        WHERE
                            STAFF_TYPE IN (?STAFF_TYPE);";

                        break;
                    }
                case CommunicationSQLCommands.FetchClasswiseMobileNumbers:
                    {
                        query = @"SELECT 
                            MOBILE_NO AS MOBILE,
                            STUDENT_ID AS RECEIPIENT_ID,
                            CLASS_ID,
                            '2' AS IS_STAFF
                        FROM
                            (SELECT 
                                UPPER(CONCAT(IFNULL(ST.FIRST_NAME, ''), '', IFNULL(ST.LAST_NAME, ''))) AS NAME,
                                    ST.ROLL_NO,
                                    SC.STUDENT_ID,
                                    ST.REGISTER_NO,
                                    sc.CLASS_ID,
                                    CP.PROGRAMME_NAME,
                                    CC.CLASS_NAME,
                                    CC.CLASS_YEAR,
                                    ST.SHIFT_ID,
                                    PM.PROGRAMME_MODE,
                                    DATE_FORMAT(ST.DATE_OF_BIRTH, '%d/%m/%Y') AS DOB,
                                    IFNULL(ST.FR_MOBILE, IFNULL(ST.MO_MOBILE, IFNULL(ST.STU_MOBILENO, NULL))) AS MOBILE_NO,
                                    ST.STU_EMAILID
                            FROM
                                stu_class AS SC
                            INNER JOIN stu_personal_info AS ST ON ST.STUDENT_ID = SC.STUDENT_ID
                            INNER JOIN cp_classes AS CC ON CC.CLASS_ID = SC.CLASS_ID 
                            INNER JOIN cp_programme_group AS PG ON PG.PROGRAMME_GROUP_ID = CC.PROGRAMME_GROUP_ID
                            LEFT JOIN sup_programme_mode AS PM ON PM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                            INNER JOIN cp_programme AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                            WHERE
			                        SC.IS_DELETED != 1
                                    AND SC.ACADEMIC_YEAR =?AC_YEAR
			                        AND SC.CLASS_ID IN(?CLASS_ID)
                                    AND ST.IS_DELETED != 1
                            ORDER BY PG.PROGRAMME_GROUP_ID , PM.PROGRAMME_MODE_ID , ST.ROLL_NO) AS T
                        WHERE
                            MOBILE_NO != '';";
                        break;
                    }
                case CommunicationSQLCommands.FetchStaffRecordsCount:
                    {
                        query = @"SELECT 
                                COUNT(SP.STAFF_ID) AS RECORDSCOUNT
                            FROM
                                STF_PERSONAL_INFO AS SP
                            WHERE
                                SP.IS_LEFT != 1 AND SP.CATEGORY =?CATEGORY
                                    AND SP.IS_DELETED != 1;";
                        break;
                    }
                case CommunicationSQLCommands.FetchStaffAndStudentRecordsCount:
                    {
                        query = @"SELECT 
                                COUNT(C.MOBILE) AS RECORDSCOUNT
                            FROM
                                (SELECT 
                                    STAFF.STAFF_ID AS MOBILE
                                FROM
                                    STF_PERSONAL_INFO AS STAFF
                                WHERE
                                        STAFF.IS_DELETED != 1 UNION ALL SELECT 
                                    SP.STUDENT_ID AS MOBILE
                                FROM
                                    STU_PERSONAL_INFO AS SP
                                INNER JOIN STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                    AND SP.IS_DELETED != 1 AND SP.IS_LEFT!=1
                                WHERE
                                        SC.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR) AS C;";
                        break;
                    }
                case CommunicationSQLCommands.FetchSentMessages:
                    {
                        query = @"SELECT 
                                SENT_SMS_ID,
                                CLASS_IDS,
                                DATE_FORMAT(DATE, '%d-%m-%Y') AS DATE,
                                SS.END_TIME,
                                CONTENT,
                                SMS_COUNT,
                                CREDIT_COUNT,
                                MT.MESSAGE_TYPE_NAME AS SMS_MODE,
                                IF(STATUS = 0, 'SUCCESS','FAILED') AS STATUS
                            FROM
                                SENT_SMS AS SS
                                    LEFT JOIN
                                SUP_MESSAGE_TYPE AS MT ON MT.MSG_TYPE_ID = SS.SMS_MODE
                            ORDER BY SENT_SMS_ID DESC;";
                        break;
                    }
                case CommunicationSQLCommands.FetchSentMessagesByAcYear:
                    {
                        query = @"SELECT 
                                    SENT_SMS_ID,
                                    CLASS_IDS,
                                    DATE_FORMAT(DATE, '%d-%m-%Y') AS DATE,
                                    SS.END_TIME,
                                    CONCAT(CC.CLASS_NAME,' - ', CONTENT) AS CONTENT,
                                    SMS_COUNT,
                                    CREDIT_COUNT,
                                    MT.MESSAGE_TYPE_NAME AS SMS_MODE,
                                    IF(STATUS = 0, 'SUCCESS', 'FAILED') AS STATUS
    
                                FROM
                                    SENT_SMS AS SS
                                        LEFT JOIN
                                    CP_cLASSES AS CC ON CC.CLASS_ID = SS.CLASS_IDS
                                        LEFT JOIN
                                    SUP_MESSAGE_TYPE AS MT ON MT.MSG_TYPE_ID = SS.SMS_MODE WHERE SS.ACADEMIC_YEAR=?AC_YEAR
                                ORDER BY SENT_SMS_ID DESC;";
                        break;
                    }

                case CommunicationSQLCommands.FetchTransactionIdBySentId:
                    {
                        query = @"SELECT 
	                                SENT_SMS_LIST_ID,
                                    SENT_ITEM_ID,
                                    GROUP_CONCAT(TRANSACTION_ID)AS 'TRANSACTION_ID' ,
                                    MOBILE_NO,
                                    CONTENT
                                FROM
                                    SENT_SMS_LIST_?AC_YEAR WHERE SENT_ITEM_ID = ?SENT_ITEM_ID;";
                        break;
                    }
                case CommunicationSQLCommands.FetchSentItemForViewMode:
                    {
                        query = @"SELECT 
                                    SP.SENT_SMS_LIST_ID,
                                   'N/A' AS REGISTER_NUMBER,
                                    '' AS NAME,
                                    SP.`DATE`,
                                    SP.COMPOSED_TIME,
                                    DATE_FORMAT(SP.SENT_TIME,'%h:%i:%s: %p') as SENT_TIME,
                                    SP.CONTENT,
                                    SP.RECEIPIENT_ID,
                                    'N/A' AS CLASS,
                                    SP.STATUS AS 'STATUS',
                                    SP.CREDIT_COUNT,
                                    SP.MOBILE_NO,
                                    DATE_FORMAT(SP.DELIVERED_DATE,'%d/%m/%Y') AS DELIVERED_DATE ,
	                                DATE_FORMAT(SP.SENT_DATE,'%d/%m/%Y') AS SENT_DATE ,
                                    SP.SENT_ITEM_ID
                               FROM
                                   SENT_SMS_LIST AS SP
                               WHERE
	                               SENT_ITEM_ID = ?SENT_ITEM_ID;";
                        break;
                    }
                case CommunicationSQLCommands.FetchSenderInfoForResend:
                    {

                        query = @"SELECT 
                                    RECEIPIENT_ID, IS_STAFF, MOBILE_NO, CLASS_ID AS CLASS,CONTENT
                                FROM
                                    SENT_SMS_LIST
                                WHERE
                                    SENT_SMS_LIST_ID = ?SENT_SMS_LIST_ID;";
                        break;
                    }
                case CommunicationSQLCommands.FetchStaffMobileNumbers:
                    {
                        query = @"SELECT 
                                    SA.CCELL_NO AS MOBILE,
                                    SP.STAFF_ID AS RECEIPIENT_ID,
                                    '' CLASS_ID,
                                    '2' AS IS_STAFF
                                FROM
                                    STF_PERSONAL_INFO AS SP
                                        INNER JOIN
                                    STF_ADDRESS AS SA ON SA.STAFFNO = SP.STAFF_ID
                                WHERE
                                    SP.IS_DELETED != 1 AND SP.IS_LEFT != 1
                                        AND SP.CATEGORY IN (?CATEGORY)
                                        AND SA.CCELL_NO IS NOT NULL;";
                        break;
                    }
                case CommunicationSQLCommands.FetchStaffAndStudentMobileNumbers:
                    {
                        query = @"SELECT 
                                        CONCAT(IFNULL(SPI.FR_MOBILE,
                                                        IFNULL(SPI.MO_MOBILE,
                                                                IFNULL(SPI.STU_MOBILENO, '')))) AS MOBILE,
                                        SC.STUDENT_ID AS RECEIPIENT_ID,
                                        SC.CLASS_ID,
                                        '' AS IS_STAFF
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                            AND SPI.IS_DELETED != 1
                                    WHERE
                                        SPI.FR_MOBILE IS NOT NULL
                                            AND SPI.FR_MOBILE != ''
                                            AND SC.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR 
                                    UNION SELECT 
                                        SA.CCELL_NO AS MOBILE,
                                        SP.STAFF_ID AS RECEIPIENT_ID,
                                        '' CLASS_ID,
                                        '2' AS IS_STAFF
                                    FROM
                                        STF_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        STF_ADDRESS AS SA ON SA.STAFFNO = SP.STAFF_ID
                                    WHERE
                                        SP.IS_DELETED != 1
                                            AND SA.CCELL_NO IS NOT NULL
                                            AND SA.CCELL_NO != ''
                                            AND SP.IS_LEFT != 1;";
                        break;
                    }
                case CommunicationSQLCommands.FetchHostelStudentRecordsCount:
                    {
                        query = @"SELECT 
                                COUNT(STUDENT_ID) AS RECORDSCOUNT
                            FROM
                                HOSTEL_STUDENT_INFO
                            WHERE
                                ACADEMIC_YEAR = ?AC_YEAR AND IS_DELETED != 1
                                    AND IS_LEFT != 1;";
                        break;
                    }
                case CommunicationSQLCommands.FetchHostelStudentMobileNumbers:
                    {
                        query = @"SELECT 
                        MOBILE_NO AS MOBILE,
                        STUDENT_ID AS RECEIPIENT_ID,
                        CLASS_ID,
                        '2' AS IS_STAFF
                    FROM
                        (SELECT 
                            UPPER(CONCAT(IFNULL(ST.FIRST_NAME, ''), '', IFNULL(ST.LAST_NAME, ''))) AS NAME,
                                ST.ROLL_NO,
                                SC.STUDENT_ID,
                                ST.REGISTER_NO,
                                sc.CLASS_ID,
                                CP.PROGRAMME_NAME,
                                CC.CLASS_NAME,
                                CC.CLASS_YEAR,
                                ST.SHIFT_ID,
                                PM.PROGRAMME_MODE,
                                DATE_FORMAT(ST.DATE_OF_BIRTH, '%d/%m/%Y') AS DOB,
                                IFNULL(ST.FR_MOBILE, IFNULL(ST.MO_MOBILE, IFNULL(ST.STU_MOBILENO, NULL))) AS MOBILE_NO,
                                ST.STU_EMAILID
                        FROM
                            hostel_student_info AS HST
                        INNER JOIN stu_class AS SC ON SC.STUDENT_ID = HST.STUDENT_ID
                        INNER JOIN stu_personal_info AS ST ON ST.STUDENT_ID = SC.STUDENT_ID
                        INNER JOIN cp_classes AS CC ON CC.CLASS_ID = SC.CLASS_ID
                        INNER JOIN cp_programme_group AS PG ON PG.PROGRAMME_GROUP_ID = CC.PROGRAMME_GROUP_ID
                        LEFT JOIN sup_programme_mode AS PM ON PM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                        INNER JOIN cp_programme AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                        WHERE
                            SC.IS_DELETED != 1
                                AND SC.ACADEMIC_YEAR =?AC_YEAR
                                AND HST.ACADEMIC_YEAR =?AC_YEAR
                                AND HST.IS_DELETED != 1
                                AND HST.IS_LEFT != 1
                                AND ST.IS_DELETED != 1
                        ORDER BY PG.PROGRAMME_GROUP_ID , PM.PROGRAMME_MODE_ID , ST.ROLL_NO) AS T
                    WHERE
                        MOBILE_NO != '';";
                        break;
                    }
                case CommunicationSQLCommands.FetchEmailSetting:
                    {
                        query = @"SELECT 
                        EMAIL_SETTING_ID,
                        SMTP_HOST,
                        SMTP_PORT,
                        SMTP_ENABLE,
                        FROM_EMAIL_ID,
                        FROM_EMAIL_PASSWORD
                    FROM
                        EMAIL_SETTING
                    WHERE
                        IS_DELETED != 1 ";
                        break;
                    }
                case CommunicationSQLCommands.FetchSMSTemplateActiveByTemplateId:
                    {
                        query = @"SELECT SMS_TEMPLATE_ID, SMS_TEMPLATE, SMS_MODULE FROM SMS_TEMPLATE_MODULE WHERE SMS_TEMPLATE_ID=?SMS_TEMPLATE_ID AND IS_ACTIVE=1 AND IS_DELETED!=1;";
                        break;
                    }
                case CommunicationSQLCommands.FetchStudentDetailsForSMSByStudentId:
                    {
                        query = @"SELECT 
                                SC.STUDENT_ID,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                SC.CLASS_ID,
                                CONCAT(IFNULL(SP.FR_MOBILE,
                                                        IFNULL(SP.MO_MOBILE,
                                                                IFNULL(SP.STU_MOBILENO, '')))) AS STU_MOBILENO,
                                CL.CLASS_NAME,
                                CP.PROGRAMME_NAME
                            FROM
                                STU_CLASS AS SC
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
                            WHERE
                                SC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SC.STUDENT_ID = ?STUDENT_ID
                                    AND SC.IS_DELETED != 1;";
                        break;
                    }
                case CommunicationSQLCommands.FetchActiveSMSvendors:
                    {
                        query = @"SELECT 
                                SMS_VENDOR_ID,
                                VENDOR_NAME,
                                VENDOR_PHONE,
                                URL,
                                ACCOUNT_MANAGER_NO,
                                SUPPORT_EMAIL_ID,
                                SALES_EMAIL_ID,
                                INTEGREATION_EMAIL_ID,
                                IS_ACTIVE,
                                IS_DELETED
                            FROM
                                SMS_VENDOR_DETAILS
                            WHERE
                                IS_ACTIVE = 1 AND IS_DELETED != 1;";
                        break;
                    }
                case CommunicationSQLCommands.FetchDovesoftSettings:
                    {
                        query = @"SELECT 
                                SMS_SETTING_ID,
                                SENDER,
                                BASE_URL,
                                API_KEY,
                                METHOD,
                                FORMAT,
                                ACCUSAGE,
                                USER,
                                ENTITY_ID,
                                IS_ACTIVE,
                                IS_DELETED
                            FROM
                                SMS_SETTING_FOR_DOVE_SOFT
                            WHERE
                                IS_DELETED != 1
                                AND API_METHOD=?API_METHOD;";
                        break;
                    }
                case CommunicationSQLCommands.FetchSMSReportByModeAndDateWise:
                    {
                        query = @" SELECT 
                                        SPI.FIRST_NAME,
                                        SPI.ROLL_NO,
                                        CC.CLASS_NAME,
                                        date_format(SS.DATE,'%d/%m/%Y') as DATE,
                                        SMT.MESSAGE_TYPE_NAME,
                                        SL.CONTENT,
                                        ifnull(FR_MOBILE,STU_MOBILENO) AS MOBILE_NO

                                    FROM
                                        sent_sms AS ss
                                            INNER JOIN
                                        sent_sms_list AS sl ON sl.sent_item_id = ss.sent_sms_id
                                            LEFT JOIN
                                        cp_classes AS cc ON cc.class_id = ss.class_ids
                                            INNER JOIN
                                        stu_class AS sc ON sc.student_id = sl.receipient_id
                                            AND sc.ACADEMIC_YEAR = ss.ACADEMIC_YEAR
                                            INNER JOIN
                                        stu_personal_info AS spi ON spi.student_id = sc.STUDENT_ID
                                            INNER JOIN
                                        sup_message_type AS smt ON smt.MSG_TYPE_ID = ss.sms_mode
                                    WHERE
                                        SMS_MODE IN (?SMS_MODE)
                                            AND ss.date BETWEEN '?FROM_DATE' AND '?TO_DATE' order by ss.DATE,ROLL_NO;";
                        break;
                    }
                case CommunicationSQLCommands.FetchSMSCreditAndDebit:
                    {
                        query = @"SELECT SMS_BALANCE_ID,
                                     DATE_FORMAT(SMS_DATE,'%d/%m/%Y') as SMS_DATE,
                                    RECEIVED_CREDITS,
                                    USED_CREDITS,
                                    BALANCE_CREDITS,
                                    IS_DELETED,
                                    IS_ACTIVE,
                                    ACADEMIC_YEAR,
                                    SMS_LIMIT
                                FROM SMS_BALANCE WHERE IS_ACTIVE=1 AND  IS_DELETED!=1;";
                        break;
                    }
            }
            return query;
        }
    }
}
