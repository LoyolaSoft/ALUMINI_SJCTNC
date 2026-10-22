using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAO;
using CMS.Utility;

namespace CMS.SQL.StaffSQL
{
    public static class StaffSQL
    {
        public static string GetStaffSQL(StaffSQLCommands sEnumCommand)
        {
            string query = string.Empty;
            switch (sEnumCommand)
            {
                case StaffSQLCommands.FetchUserAccountByStaffCode:
                    {
                        query = @"SELECT 
                                    USER_ACCOUNT_ID,
                                    USERNAME,
                                    ROLE,
                                    USER_TYPE
                                FROM
                                    USER_ACCOUNT WHERE USERNAME=?USERNAME AND IS_DELETED!=1;";
                        break;
                    }
                case StaffSQLCommands.PersonalIfExits:
                    {
                        query = @"SELECT STAFF_ID,STAFF_CODE FROM STF_PERSONAL_INFO WHERE STAFF_CODE=?STAFF_CODE AND IS_DELETED!=1;";
                        break;
                    }
                case StaffSQLCommands.FetchLastInsertedId:
                    {
                        query = @" SELECT 
                                                    STAFF_ID
                                                FROM
                                                    STF_PERSONAL_INFO
                                                ORDER BY STF_PERSONAL_INFO.STAFF_ID DESC
                                                LIMIT 1;";
                        break;
                    }
                case StaffSQLCommands.FetchStaffList:
                    {
                        query = @"SELECT 
                                    STAFF_ID,
                                    STAFF_CODE,
                                    FIRST_NAME,
                                    D.DEPARTMENT,
                                    G.GENDER_NAME,
                                    GROUP_CONCAT(S.SHIFT_NAME) AS SHIFT_NAME,
                                    SPI.IS_LEFT,
                                    DATE_FORMAT(DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                    SPI.PHOTO
                                FROM
                                    STF_PERSONAL_INFO SPI
                                        LEFT JOIN
                                    CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = SPI.DEPARTMENT
                                        LEFT JOIN
                                    SUP_GENDER AS G ON G.GENDER_ID = SPI.GENDER
                                        LEFT JOIN
                                    SUP_SHIFT AS S ON FIND_IN_SET(S.SHIFT_ID, SPI.SHIFT)
                                WHERE
                                    SPI.IS_DELETED != 1
                                GROUP BY STAFF_ID
                                ORDER BY STAFF_ID";
                        break;
                    }
                case StaffSQLCommands.FetchPersonalRecordByID:
                    {
                        query = @" SELECT 
	                                            STAFF_ID,
                                                
                                                STAFF_CODE,
                                                FIRST_NAME,
                                                LAST_NAME,
                                                KNOWN_AS,
                                                PLACE_OF_BIRTH,
                                                DATE_FORMAT(DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                                DATE_FORMAT(DATE_OF_JOIN,'%d/%m/%Y') AS DATE_OF_JOIN,
                                                BLOOD_GROUP,
                                                MARITAL_STATUS,
                                                DEPARTMENT,
                                                SHIFT,
                                                MOTHER_TONGUE,
                                                GENDER,
                                                COMMUNITY,
                                                NATIONALITY,
                                                RELIGION,
                                                PHOTO,IS_LEFT
                                            FROM
                                                STF_PERSONAL_INFO WHERE STAFF_ID=?STAFF_ID AND IS_DELETED!=1;";
                        break;
                    }
                case StaffSQLCommands.FetchPaperById:
                    {
                        query = @"SELECT       PAPER_ID,
                                               STAFF_NO,
                                               PAPER_NAME,
                                               L.LEVEL,
                                               P.PUBLISH_CATEGORY,
                                               JOURNAL_NAME,
                                               NO_OF_PAGES,
                                               PAGES_FROM, 
                                               PAGES_TO,
                                               V.VOLUME,
											   DATE_FORMAT(YEAR_PUBLISHED,'%d/%m/%Y') AS YEAR_PUBLISHED
                                            FROM STF_PAPER SP 
                                            INNER JOIN SUP_LEVEL AS L ON L.LEVEL_ID=SP.LEVEL
                                            INNER JOIN SUP_PUBLISH_CATEGORY AS P ON P.PUBLISH_CATEGORY_ID=SP.PUBLISHING_CATEGORY
                                            INNER JOIN SUP_VOLUME AS V ON V.VOLUME_ID=SP.VOLUME
                                            WHERE  SP.IS_DELETED!=1 AND SP.STAFF_NO=?STAFF_NO;";
                        break;
                    }
                case StaffSQLCommands.BindPaperInfo:
                    {
                        query = @"SELECT 
                                    PAPER_ID,
                                    STAFF_NO, 
                                    PAPER_NAME, 
                                    LEVEL, 
                                    PUBLISHING_CATEGORY, 
                                    JOURNAL_NAME,
                                    NO_OF_PAGES,
                                    PAGES_FROM, 
                                    PAGES_TO, 
                                    VOLUME, 
                                    DATE_FORMAT(YEAR_PUBLISHED,'%d/%m/%Y') AS YEAR_PUBLISHED, 
                                    IS_DELETED
                                FROM
                                    STF_PAPER WHERE IS_DELETED!=1 AND PAPER_ID=?PAPER_ID;";
                        break;
                    }
                case StaffSQLCommands.DeletePaper:
                    {
                        query = @"UPDATE STF_PAPER 
                                SET 
                                    IS_DELETED = ?IS_DELETED
                                WHERE
                                    PAPER_ID = ?PAPER_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchTrainingById:
                    {
                        query = @"SELECT TRAINING_NO,
                                                STAFF_NO,
                                                 DATE_FORMAT(DATE_FROM,'%d/%m/%Y') AS DATE_FROM,
                                                 DATE_FORMAT(DATE_TO,'%d/%m/%Y') AS DATE_TO,
                                                PROGRAMME,
                                                PLACE,
                                                TLEVEL,
                                                COMMENTS
                                            FROM STF_TRAINING WHERE ISDELETED!=1 AND STAFF_NO=?STAFF_NO;";
                        break;
                    }
                case StaffSQLCommands.BindTraining:
                    {
                        query = @"SELECT TRAINING_NO,
                                                STAFF_NO,
                                                DATE_FORMAT(DATE_FROM,'%d/%m/%Y') AS DATE_FROM,
                                                DATE_FORMAT(DATE_TO,'%d/%m/%Y') AS DATE_TO,
                                                PROGRAMME,
                                                PLACE,
                                                TLEVEL,
                                                COMMENTS,
                                                ISDELETED
                                            FROM STF_TRAINING WHERE ISDELETED!=1 AND TRAINING_NO=?TRAINING_NO;";
                        break;
                    }
                case StaffSQLCommands.DeleteTraining:
                    {
                        query = @"UPDATE STF_TRAINING  SET ISDELETED=?ISDELETED WHERE TRAINING_NO=?TRAINING_NO;";
                        break;
                    }
                case StaffSQLCommands.FetchQualificationById:
                    {
                        query = @"SELECT 
											QUALIFICATION_NO,
											STAFF_NO,
											D.DEGREE,
											DEGREE_TYPE,
											MAIN_SUBJECT,
											ALLIED_SUBJECT,
											DATE_FORMAT(COMPLETION_MONTH,'%d/%m/%Y') AS COMPLETION_MONTH,
											INSTITUTE_OF_STUDY,
											UNIVERSITY,
											PERCENTAGE,
											SQ.ISDELETED,
											DATE_FORMAT(COMPLETION_YEAR,'%d/%m/%Y') AS COMPLETION_YEAR
                                            FROM STF_QUALIFICATION SQ
                                            INNER JOIN SUP_DEGREE AS D ON D.DEGREE_ID=SQ.DEGREE
                                            WHERE SQ.ISDELETED!=1 AND STAFF_NO=?STAFF_NO;";
                        break;
                    }
                case StaffSQLCommands.BindQualification:
                    {
                        query = @"SELECT 
                                              QUALIFICATION_NO,
                                               STAFF_NO,
                                               DEGREE,
                                               DEGREE_TYPE,
                                               MAIN_SUBJECT,
                                               ALLIED_SUBJECT,
                                               DATE_FORMAT(COMPLETION_MONTH,'%d/%m/%Y') AS COMPLETION_MONTH,
                                               INSTITUTE_OF_STUDY,
                                               UNIVERSITY,
                                               PERCENTAGE,
                                               ISDELETED,
                                               DATE_FORMAT(COMPLETION_YEAR,'%d/%m/%Y') AS COMPLETION_YEAR
                                            FROM STF_QUALIFICATION WHERE ISDELETED!=1 AND QUALIFICATION_NO=?QUALIFICATION_NO;";
                        break;
                    }
                case StaffSQLCommands.DeleteQualification:
                    {
                        query = @"UPDATE STF_QUALIFICATION  SET ISDELETED=?ISDELETED WHERE QUALIFICATION_NO=?QUALIFICATION_NO;";
                        break;
                    }
                case StaffSQLCommands.FetchPublishById:
                    {
                        query = @"SELECT 
	                                            PUBLISH_ID,
                                                STAFF_NO,
                                                BOOK_NAME,
                                                L.LEVEL,
                                                SPC.PUBLISH_CATEGORY,
                                                JOURNAL_NAME,
                                                V.VOLUME,
                                                PUBLISHER,
                                                DATE_FORMAT(PYEAR,'%d/%m/%Y') AS PYEAR,
                                                EDITION
                                            FROM STF_PUBLISH SP 
                                            LEFT JOIN SUP_LEVEL AS L ON L.LEVEL_ID=SP.LEVEL
                                            LEFT JOIN SUP_PUBLISH_CATEGORY AS SPC ON SPC.PUBLISH_CATEGORY_ID=SP.PUBLISHING_CATEGORY
                                            LEFT JOIN SUP_VOLUME AS V ON V.VOLUME_ID=SP.VOLUME
                                            WHERE SP.ISDELETED!=1 AND STAFF_NO=?STAFF_NO;";
                        break;
                    }
                case StaffSQLCommands.BindPublish:
                    {
                        query = @"SELECT 
	                                            PUBLISH_ID,
                                                STAFF_NO,
                                                BOOK_NAME,
                                                LEVEL,
                                                PUBLISHING_CATEGORY,
                                                JOURNAL_NAME,
                                                VOLUME,
                                                PUBLISHER,
                                                DATE_FORMAT(PYEAR,'%d/%m/%Y') AS PYEAR,
                                                EDITION
                                            FROM STF_PUBLISH WHERE ISDELETED!=1 AND PUBLISH_ID=?PUBLISH_ID;";
                        break;
                    }
                case StaffSQLCommands.DeletePublish:
                    {
                        query = @"UPDATE STF_PUBLISH  SET ISDELETED=?ISDELETED WHERE PUBLISH_ID=?PUBLISH_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchCounselingById:
                    {
                        query = @"SELECT STF_COUN_ID,
													   DATE_FORMAT(DATE_OF_COUN,'%d/%m/%Y') AS DATE_OF_COUN,
                                                       DURATION,
                                                       STAFF,
                                                       REASON,
                                                       GIVEN,
                                                       ACTION_TAKEN,
                                                       REMARK
                                                    FROM STF_COUNSELING WHERE IS_DELETED!=1 AND STAFF=?STAFF;";
                        break;
                    }
                case StaffSQLCommands.BindCounselling:
                    {
                        query = @"SELECT 
                                    STF_COUN_ID,
                                    DATE_FORMAT(DATE_OF_COUN,'%d/%m/%Y') AS DATE_OF_COUN, 
                                    DURATION, 
                                    STAFF, 
                                    REASON, 
                                    GIVEN, 
                                    ACTION_TAKEN, 
                                    REMARK
                                FROM
                                    STF_COUNSELING WHERE IS_DELETED!=1 AND STF_COUN_ID=?STF_COUN_ID;";
                        break;
                    }
                case StaffSQLCommands.DeleteCounselling:
                    {
                        query = @"UPDATE STF_COUNSELING 
                                SET 
                                    IS_DELETED = ?IS_DELETED
                                WHERE
                                    STF_COUN_ID = ?STF_COUN_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchAddressById:
                    {
                        query = @"SELECT 
                                               ADDRESS_NO,
                                               STAFFNO,
                                               CDOOR_DETAILS,
                                               CSTREET,
                                               CPLACE,
                                               CCITY,
                                               CPIN_CODE,
                                               CDISTRICT,
                                               CVILLAGE,
                                               CCOUNTRY,
                                               CPHONE_NO,
                                               CCELL_NO,
                                               CEMAIL,
                                               PSTREET,
                                               PVILLAGE,
                                               PPLACE,
                                               PCITY,
                                               PPIN_CODE,
                                               PDISTRICT,
                                               PCOUNTRY,
                                               PPHONE_NO,
                                               PCELL_NO,
                                               PDOOR_DETAILS,
                                               PEMAIL
                                            FROM STF_ADDRESS WHERE STAFFNO=?STAFFNO ;";
                        break;
                    }
                case StaffSQLCommands.FetchServiceById:
                    {
                        query = @"SELECT 
                                                SERVICE_NO,
                                                STAFF_NO,
                                                DATE_FORMAT(DATE_OF_APPOINT,'%d/%m/%Y') AS DATE_OF_APPOINT,
                                                APPOINTMENT_NAME,
                                                APPOINTMENT_NATURE,
                                                DATE_FORMAT(DATE_OF_TERMINATION,'%d/%m/%Y') AS DATE_OF_TERMINATION,
                                                INSTITUTE,
                                                REMARKS,
                                                PLACE
                                            FROM STF_SERVICES WHERE ISDELETED!=1 AND STAFF_NO=?STAFF_NO;";
                        break;
                    }
                case StaffSQLCommands.BindServices:
                    {
                        query = @" SELECT SERVICE_NO,
                                                STAFF_NO,
                                                DATE_FORMAT(DATE_OF_APPOINT,'%d/%m/%Y') AS DATE_OF_APPOINT,
                                                APPOINTMENT_NAME,
                                                APPOINTMENT_NATURE,
                                                DATE_FORMAT(DATE_OF_TERMINATION,'%d/%m/%Y') AS DATE_OF_TERMINATION,
                                                INSTITUTE,
                                                REMARKS,
                                                PLACE,
                                                ISDELETED
                                            FROM STF_SERVICES WHERE  ISDELETED!=1 AND SERVICE_NO=?SERVICE_NO;";
                        break;
                    }
                case StaffSQLCommands.DeleteServices:
                    {
                        query = @"UPDATE STF_SERVICES  SET ISDELETED=?ISDELETED WHERE SERVICE_NO=?SERVICE_NO;";
                        break;
                    }
                case StaffSQLCommands.FetchSubjectDetailsById:
                    {
                        query = @"Select 
                                                STAFF_ID,
                                                BOARD_MEMBER,
                                                STAFF_CODE,
                                                DESIGNATION,
                                                KNOWLEDGE_IN_COMPUTER,
                                                MAIN_SUBJECT,
                                                NON_TEACHING_CATEGORY,
                                                QUALIFICATION,
                                                SESSIONS,
                                                SHIFT,
                                                SUB_CATEGORY FROM STF_PERSONAL_INFO 
                                            WHERE STAFF_ID=?STAFF_ID ;";
                        break;
                    }
                case StaffSQLCommands.FetchLeavingById:
                    {
                        query = @"select 
                                                DATE_FORMAT(LEAVING_DATE,'%d/%m/%Y') AS LEAVING_DATE,
                                                LEAVING_REMARKS,
                                                DEPARTMENT,
                                                DATE_FORMAT(RETIREMENT_DATE,'%d/%m/%Y') AS RETIREMENT_DATE,
                                                LEAVING_REASON,
                                                IS_LEFT  FROM STF_PERSONAL_INFO
                                                WHERE STAFF_ID =?STAFF_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchFamily:
                    {
                        query = @"SELECT 
                                                FAMILY_NO, 
                                                STAFF_NO,
                                                NAME, 
                                                R.RELATION, 
                                                DATE_FORMAT(DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH
                                            FROM
                                                STF_FAMILY SF
                                                INNER JOIN SUP_RELATION AS R ON R.RELATION_ID=SF.RELATION WHERE  ISDELETED!=1 AND STAFF_NO=?STAFF_NO;";
                        break;
                    }
                case StaffSQLCommands.BindFamily:
                    {
                        query = @"SELECT 
                                                FAMILY_NO, 
                                                STAFF_NO,
                                                NAME, 
                                                RELATION, 
                                                DATE_FORMAT(DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH
                                            FROM
                                                STF_FAMILY WHERE ISDELETED!=1 AND FAMILY_NO=?FAMILY_NO;";
                        break;
                    }
                case StaffSQLCommands.DeleteFamily:
                    {
                        query = @"UPDATE STF_FAMILY  SET ISDELETED=?ISDELETED WHERE FAMILY_NO=?FAMILY_NO;";
                        break;
                    }
                case StaffSQLCommands.InsertPersonal:
                    {
                        query = @"INSERT INTO STF_PERSONAL_INFO(
                                    FIRST_NAME,LAST_NAME,GENDER,
                                    KNOWN_AS,PLACE_OF_BIRTH,COMMUNITY,
                                    NATIONALITY,MARITAL_STATUS,
                                    DATE_OF_JOIN,DATE_OF_BIRTH,
                                    BLOOD_GROUP,RELIGION,
                                    MOTHER_TONGUE,DEPARTMENT,STAFF_CODE,SHIFT,PHOTO)
                                    VALUES
                                    (
                                      ?FIRST_NAME,?LAST_NAME,?GENDER,
                                      ?KNOWN_AS,?PLACE_OF_BIRTH,?COMMUNITY,
                                      ?NATIONALITY,?MARITAL_STATUS,
                                      ?DATE_OF_JOIN,?DATE_OF_BIRTH,
                                      ?BLOOD_GROUP,?RELIGION,
                                      ?MOTHER_TONGUE,?DEPARTMENT,?STAFF_CODE,?SHIFT,?PHOTO
                                    );";
                        break;
                    }

                case StaffSQLCommands.InsertStaffAddress:
                    {
                        query = @"INSERT INTO STF_ADDRESS(
                                                  STAFFNO,CDOOR_DETAILS, CSTREET,CPLACE,CCITY,
                                                  CPIN_CODE, CDISTRICT,CCOUNTRY,CVILLAGE,CPHONE_NO,
                                                  CCELL_NO,CEMAIL,PSTREET,PVILLAGE,PPLACE,
                                                  PCITY,PPIN_CODE,PDISTRICT,PCOUNTRY,PPHONE_NO,
                                                  PCELL_NO,PDOOR_DETAILS,PEMAIL)
                                                  VALUES
                                                  (
                                                    ?STAFFNO,?CDOOR_DETAILS,?CSTREET,?CPLACE,?CCITY,
                                                    ?CPIN_CODE,?CDISTRICT,?CCOUNTRY,?CVILLAGE,?CPHONE_NO,
                                                    ?CCELL_NO,?CEMAIL,?PSTREET,?PVILLAGE,?PPLACE,
                                                    ?PCITY,?PPIN_CODE,?PDISTRICT,?PCOUNTRY,?PPHONE_NO,
                                                    ?PCELL_NO,?PDOOR_DETAILS,?PEMAIL
                                                   );";
                        break;
                    }
                case StaffSQLCommands.InsertStaffServices:
                    {
                        query = @"INSERT INTO STF_SERVICES(
                                                STAFF_NO,
                                                DATE_OF_APPOINT,
                                                APPOINTMENT_NAME,
                                                DATE_OF_TERMINATION,
                                                INSTITUTE,
                                                REMARKS,
                                                PLACE)
                                                VALUES
                                                (
                                                  ?STAFF_NO,?DATE_OF_APPOINT,?APPOINTMENT_NAME,?DATE_OF_TERMINATION,?INSTITUTE,?REMARKS,?PLACE
                                                );";
                        break;
                    }
                case StaffSQLCommands.InsertStaffCounseling:
                    {
                        query = @"INSERT INTO STF_COUNSELING(
                                             STAFF,
                                             DATE_OF_COUN,
                                             DURATION,
                                             REASON,
                                             GIVEN,
                                             ACTION_TAKEN,
                                             REMARK)
                                             VALUES
                                            (
                                              ?STAFF,?DATE_OF_COUN,?DURATION,?REASON,?GIVEN,?ACTION_TAKEN,?REMARK
                                            );";
                        break;
                    }
                case StaffSQLCommands.InsertStaffPaper:
                    {
                        query = @"INSERT INTO STF_PAPER(
                                                    STAFF_NO,
                                                    PAPER_NAME,
                                                    LEVEL,
                                                    PUBLISHING_CATEGORY,
                                                    JOURNAL_NAME,
                                                    NO_OF_PAGES,
                                                    PAGES_FROM,
                                                    PAGES_TO,
                                                    VOLUME,
                                                    YEAR_PUBLISHED)
                                                    VALUES
                                                    (
                                                      ?STAFF_NO,?PAPER_NAME,?LEVEL,?PUBLISHING_CATEGORY,?JOURNAL_NAME,?NO_OF_PAGES,?PAGES_FROM,?PAGES_TO,?VOLUME,?YEAR_PUBLISHED
                                                    );";
                        break;
                    }
                case StaffSQLCommands.InsertStaffPublish:
                    {
                        query = @"INSERT INTO STF_PUBLISH(STAFF_NO,BOOK_NAME,JOURNAL_NAME,PUBLISHER,PYEAR,EDITION,LEVEL,VOLUME,PUBLISHING_CATEGORY)VALUES
                                                (
                                                    ?STAFF_NO,?BOOK_NAME,?JOURNAL_NAME,?PUBLISHER,?PYEAR,?EDITION,?LEVEL,?VOLUME,?PUBLISHING_CATEGORY
                                                );";
                        break;
                    }
                case StaffSQLCommands.InsertStaffQualification:
                    {
                        query = @"INSERT INTO STF_QUALIFICATION(
                                                STAFF_NO,
                                                MAIN_SUBJECT,
                                                ALLIED_SUBJECT,
                                                COMPLETION_YEAR,
                                                COMPLETION_MONTH,
                                                INSTITUTE_OF_STUDY,
                                                DEGREE,UNIVERSITY,
                                                PERCENTAGE)
                                                VALUES
                                                (
                                                   ?STAFF_NO,?MAIN_SUBJECT,?ALLIED_SUBJECT,?COMPLETION_YEAR,?COMPLETION_MONTH,?INSTITUTE_OF_STUDY,?DEGREE,?UNIVERSITY,?PERCENTAGE
                                                 );";
                        break;
                    }
                case StaffSQLCommands.InsertStaffTraining:
                    {
                        query = @"INSERT INTO STF_TRAINING(STAFF_NO,
                                                  DATE_FROM,
                                                  DATE_TO,
                                                  PROGRAMME,
                                                  PLACE,
                                                  TLEVEL,
                                                  COMMENTS)
                                                   VALUES
                                                   (
                                                    ?STAFF_NO,?DATE_FROM,?DATE_TO,?PROGRAMME,?PLACE,?TLEVEL,?COMMENTS
                                                    );";
                        break;
                    }
                case StaffSQLCommands.InsertStaffFamily:
                    {
                        query = @"INSERT INTO STF_FAMILY(NAME,RELATION,DATE_OF_BIRTH,STAFF_NO)
                                                VALUES
                                                (
                                                   ?NAME,?RELATION,?DATE_OF_BIRTH,?STAFF_NO
                                                );";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffPersonal:
                    {
                        query = @"UPDATE STF_PERSONAL_INFO SET
                                                  FIRST_NAME=?FIRST_NAME,
                                                  LAST_NAME=?LAST_NAME,
                                                  STAFF_CODE=?STAFF_CODE,
                                                  GENDER=?GENDER,
                                                  KNOWN_AS=?KNOWN_AS,
                                                  PLACE_OF_BIRTH=?PLACE_OF_BIRTH,
                                                  COMMUNITY=?COMMUNITY,
                                                  NATIONALITY=?NATIONALITY,
                                                  MARITAL_STATUS=?MARITAL_STATUS,
                                                  DATE_OF_JOIN=?DATE_OF_JOIN,
                                                  DATE_OF_BIRTH=?DATE_OF_BIRTH,
                                                  BLOOD_GROUP=?BLOOD_GROUP,
                                                  RELIGION=?RELIGION,
                                                  MOTHER_TONGUE=?MOTHER_TONGUE,
                                                  SHIFT=?SHIFT,
                                                  DEPARTMENT=?DEPARTMENT,IS_LEFT=?IS_LEFT WHERE STAFF_ID=?STAFF_ID;";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffAddress:
                    {
                        query = @"UPDATE STF_ADDRESS
                                                 SET
                                                 ADDRESS_NO = ?ADDRESS_NO,
                                                 CDOOR_DETAILS =?CDOOR_DETAILS,
                                                 CSTREET = ?CSTREET,
                                                 CPLACE = ?CPLACE,
                                                 CCITY = ?CCITY,
                                                 CPIN_CODE =?CPIN_CODE,
                                                 CDISTRICT = ?CDISTRICT,
                                                 CVILLAGE = ?CVILLAGE,
                                                 CCOUNTRY = ?CCOUNTRY,
                                                 CPHONE_NO = ?CPHONE_NO,
                                                 CCELL_NO = ?CCELL_NO,
                                                 CEMAIL = ?CEMAIL,
                                                 PSTREET = ?PSTREET,
                                                 PVILLAGE = ?PVILLAGE,
                                                 PPLACE =?PPLACE,
                                                 PCITY =?PCITY,
                                                 PPIN_CODE =?PPIN_CODE,
                                                 PDISTRICT =?PDISTRICT,
                                                 PCOUNTRY =?PCOUNTRY,
                                                 PPHONE_NO =?PPHONE_NO,
                                                 PCELL_NO =?PCELL_NO,
                                                 PDOOR_DETAILS =?PDOOR_DETAILS,
                                                 PEMAIL = ?PEMAIL,
                                                 STAFFNO=?STAFFNO
                                                 WHERE ADDRESS_NO = ?ADDRESS_NO;";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffCounseling:
                    {
                        query = @"UPDATE STF_COUNSELING SET
                                             DATE_OF_COUN=?DATE_OF_COUN,
                                             DURATION=?DURATION,
                                             REASON=?REASON,
                                             GIVEN=?GIVEN,
                                             ACTION_TAKEN=?ACTION_TAKEN,
                                             STAFF=?STAFF,
                                             REMARK=?REMARK WHERE STF_COUN_ID=?STF_COUN_ID;";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffPaper:
                    {
                        query = @"UPDATE STF_PAPER SET
                                                    PAPER_NAME=?PAPER_NAME,
                                                    LEVEL=?LEVEL,
                                                    PUBLISHING_CATEGORY=?PUBLISHING_CATEGORY,
                                                    JOURNAL_NAME=?JOURNAL_NAME,
                                                    NO_OF_PAGES=?NO_OF_PAGES,
                                                    PAGES_FROM=?PAGES_FROM,
                                                    PAGES_TO=?PAGES_TO,
                                                    VOLUME=?VOLUME,
                                                    STAFF_NO=?STAFF_NO,
                                                    YEAR_PUBLISHED=?YEAR_PUBLISHED WHERE PAPER_ID=?PAPER_ID;";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffPublish:
                    {
                        query = @"UPDATE STF_PUBLISH SET 
	                                            BOOK_NAME=?BOOK_NAME,
                                                JOURNAL_NAME=?JOURNAL_NAME,
                                                PUBLISHER=?PUBLISHER,
                                                PYEAR=?PYEAR,
                                                STAFF_NO=?STAFF_NO,
                                                VOLUME=?VOLUME,
                                                PUBLISHING_CATEGORY=?PUBLISHING_CATEGORY,
                                                LEVEL=?LEVEL,
                                                EDITION=?EDITION WHERE PUBLISH_ID=?PUBLISH_ID;";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffQualification:
                    {
                        query = @"UPDATE STF_QUALIFICATION SET 
	                                            MAIN_SUBJECT=?MAIN_SUBJECT,
                                                ALLIED_SUBJECT=?ALLIED_SUBJECT,
                                                COMPLETION_YEAR=?COMPLETION_YEAR,
                                                COMPLETION_MONTH=?COMPLETION_MONTH,
                                                INSTITUTE_OF_STUDY=?INSTITUTE_OF_STUDY,
                                                DEGREE=?DEGREE,
                                                UNIVERSITY=?UNIVERSITY,
                                                STAFF_NO=?STAFF_NO,
                                                PERCENTAGE=?PERCENTAGE WHERE QUALIFICATION_NO=?QUALIFICATION_NO;";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffService:
                    {
                        query = @"UPDATE STF_SERVICES SET
                                                   DATE_OF_APPOINT=?DATE_OF_APPOINT,
                                                   APPOINTMENT_NAME=?APPOINTMENT_NAME,
                                                   DATE_OF_TERMINATION=?DATE_OF_TERMINATION,
                                                   INSTITUTE=?INSTITUTE,
                                                   REMARKS=?REMARKS,
                                                   STAFF_NO=?STAFF_NO,
                                                   PLACE=?PLACE WHERE SERVICE_NO=?SERVICE_NO;";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffSubjectDetails:
                    {
                        query = @"UPDATE STF_PERSONAL_INFO SET                                                 
                                                 BOARD_MEMBER =?BOARD_MEMBER,
                                                 DESIGNATION =?DESIGNATION,
                                                 KNOWLEDGE_IN_COMPUTER =?KNOWLEDGE_IN_COMPUTER,
                                                 MAIN_SUBJECT =?MAIN_SUBJECT,
                                                 NON_TEACHING_CATEGORY =?NON_TEACHING_CATEGORY,
                                                 QUALIFICATION =?QUALIFICATION,
                                                 SESSIONS =?SESSIONS,
                                                 SHIFT =?SHIFT,
                                                 SUB_CATEGORY =?SUB_CATEGORY
                                             WHERE STAFF_ID =?STAFF_ID; ";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffTraining:
                    {
                        query = @"UPDATE STF_TRAINING SET 
                                                  DATE_FROM=?DATE_FROM,
                                                  DATE_TO=?DATE_TO,
                                                  PROGRAMME=?PROGRAMME,
                                                  PLACE=?PLACE,
                                                  TLEVEL=?TLEVEL,
                                                  STAFF_NO=?STAFF_NO,
                                                  COMMENTS=?COMMENTS WHERE TRAINING_NO=?TRAINING_NO;";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffLeaving:
                    {
                        query = @"UPDATE STF_PERSONAL_INFO
                                                         SET
                                                         LEAVING_DATE =?LEAVING_DATE,
                                                         LEAVING_REMARKS =?LEAVING_REMARKS,
                                                         DEPARTMENT =?DEPARTMENT,
                                                         RETIREMENT_DATE =?RETIREMENT_DATE,
                                                         LEAVING_REASON =?LEAVING_REASON,
                                                         IS_LEFT = ?IS_LEFT
                                                         WHERE STAFF_ID =?STAFF_ID;";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffFamily:
                    {
                        query = @"UPDATE STF_FAMILY 
                                                SET 
                                                    NAME = ?NAME,
                                                    RELATION = ?RELATION,
                                                    DATE_OF_BIRTH = ?DATE_OF_BIRTH,
                                                    STAFF_NO=?STAFF_NO
                                                WHERE
                                                    FAMILY_NO = ?FAMILY_NO;";
                        break;
                    }
                case StaffSQLCommands.DeleteQuery:
                    {
                        query = @"UPDATE STF_PERSONAL_INFO 
                                                SET 
                                                    IS_DELETED = ?IS_DELETED
                                                WHERE
                                                    STAFF_ID = ?STAFF_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchStaffProfileView:
                    {
                        query = @"SELECT 
                                        SF.STAFF_ID,
                                        SF.STAFF_CODE,
                                        SF.FIRST_NAME,
                                        SG.GENDER_NAME,
                                        SN.NATIONALITY_NAME,
                                        SC.STF_CATEGORY,
                                        DATE_FORMAT(DATE_OF_BIRTH, '%d/%m/%y') AS DATE_OF_BIRTH,
                                        DATE_FORMAT(DATE_OF_JOIN, '%d/%m/%y') AS DATE_OF_JOIN,
                                        DT.DEPARTMENT,
                                        SH.SHIFT_NAME,
                                        SF.QUALIFICATION,
                                        SF.PHOTO
                                    FROM
                                        STF_PERSONAL_INFO AS SF
                                            LEFT JOIN
                                        SUP_GENDER AS SG ON SG.GENDER_ID = SF.GENDER
                                            LEFT JOIN
                                        SUP_NATIONALITY AS SN ON SN.NATIONALITY_ID = SF.NATIONALITY
                                            LEFT JOIN
                                        SUP_STAFF_CATEGORY AS SC ON SC.STF_CATEGORY_ID = SF.CATEGORY
		                                    LEFT JOIN
	                                    CP_DEPARTMENT AS DT ON DT.DEPARTMENT_ID=SF.DEPARTMENT
		                                    INNER JOIN
	                                    SUP_SHIFT AS SH ON SH.SHIFT_ID=SF.SHIFT
                                    WHERE
                                        SF.STAFF_ID=?STAFF_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchClassWiseStudentForStaff:
                    {
                        query = @"SELECT 
                                    SC.STUDENT_ID,
                                    UPPER(CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, ''))) AS 'FIRST_NAME',
                                    SP.ADMISSION_NO,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS 'DATE_OF_BIRTH',
                                    IFNULL(SP.STU_MOBILENO,
                                            IFNULL(SP.FR_MOBILE, MO_MOBILE)) AS 'MOBILE',
                                    UPPER(C.CLASS_NAME) AS 'CLASS_NAME',
                                    UPPER(CONCAT(IFNULL(R.COURSE_TITLE, ''),
                                            ' ',
                                            IFNULL(R.COURSE_CODE, ''))) AS 'COURSE'
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID = SC.CLASS_ID
                                        AND CCS.ACADEMIC_YEAR = ?AC_YEAR 
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_LEFT != 1
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                        AND C.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = CCS.STAFF_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS R ON R.COURSE_ROOT_ID = CCS.COURSE_ID
                                        AND R.IS_DELETED != 1
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CCS.SEMESTER_ID IN (?SEMESTER_ID)
                                        AND SC.CLASS_ID = ?CLASS_ID
                                        AND SC.IS_DELETED != 1 
		                                AND CCS.STAFF_ID = ?STAFF_ID
                                UNION SELECT 
                                    SC.STUDENT_ID,
                                    UPPER(CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, ''))) AS 'FIRST_NAME',
                                    SP.ADMISSION_NO,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS 'DATE_OF_BIRTH',
                                    IFNULL(SP.STU_MOBILENO,
                                            IFNULL(SP.FR_MOBILE, MO_MOBILE)) AS 'MOBILE',
                                    UPPER(C.CLASS_NAME) AS 'CLASS_NAME',
                                    UPPER(CONCAT(IFNULL(R.COURSE_TITLE, ''),
                                            ' ',
                                            IFNULL(R.COURSE_CODE, ''))) AS 'COURSE'
                                FROM
                                    STU_COURSE_INFO AS SC
                                        INNER JOIN
                                    CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID = SC.CLASS_ID
                                        AND CCS.ACADEMIC_YEAR = ?AC_YEAR 
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_LEFT != 1
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                        AND C.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = CCS.STAFF_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS R ON R.COURSE_ROOT_ID = CCS.COURSE_ID
                                        AND R.IS_DELETED != 1
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CCS.SEMESTER_ID IN (?SEMESTER_ID)
                                        AND SC.S_CLASS_ID = ?CLASS_ID
                                        AND SC.IS_DELETED != 1
                                        AND CCS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CCS.STAFF_ID = ?STAFF_ID;";
                        break;
                    }
                case StaffSQLCommands.ListStudentCourseRemarkList:
                    {
                        query = @"SELECT 
                                REMARK_ID,
                                REMARK,
                                C.CLASS_ID,
                                C.CLASS_NAME,
                                RM.COURSE_ID,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                CR.COURSE_TITLE,
                                RM.STAFF_ID,
                                STF.FIRST_NAME AS 'STAFF_NAME',
                                SP.STUDENT_ID,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(SP.LAST_NAME, '')) AS 'STUDENT_NAME',
                                CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(STF.LAST_NAME, '')) AS 'STAFF_NAME',
							   SCC.COMPONENT
                            FROM
                                STU_COURSE_REMARK AS RM
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = RM.STUDENT_ID
                                    AND SC.ACADEMIC_YEAR =?AC_YEAR
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    AND SC.IS_DELETED != 1
                                    INNER JOIN
                                CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                    AND C.IS_DELETED != 1
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = RM.COURSE_ID
                                    AND CR.IS_DELETED != 1
                                    INNER JOIN
                                STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = RM.STAFF_ID
                                    AND STF.IS_DELETED != 1
                                    
                                    INNER JOIN
								    CIA_GROUP_COMPONENT AS CG ON CG.CIA_GROUP_COMPONENT_ID=RM.COURSE_GROUP_ID
                                    AND CG.IS_DELETED!=1
                                    
                                    INNER JOIN
                                    SUP_CIA_COMPONENTS AS SCC ON SCC.COMPONENT_ID=CG.SUP_CIA_COMPONENT_ID
                                    AND SCC.IS_DELETED!=1
                                    
                            WHERE
                                RM.ACADEMIC_YEAR =?AC_YEAR                                   
                                    AND RM.COURSE_ID =?COURSE_ID
                                    AND RM.CLASS_ID = ?CLASS_ID
                                    AND SC.IS_DELETED != 1;";
                        break;
                    }
                case StaffSQLCommands.FetchStudentByCourseIdClassId:
                    {
                        query = @"SELECT 
                            SC.STUDENT_ID,
                            CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                    ' ',
                                    IFNULL(SP.LAST_NAME, '')) AS STUDENT_NAME,
                            SP.ROLL_NO,
                            SP.REGISTER_NO,
                            CC.COURSE_ID,
                            SC.CLASS_ID,
                            CLS.CLASS_NAME,
                            RM.REMARK,
                            RM.REMARK_ID,
                            IF(COURSE_GROUP_MARK = '-1',
                                'A',
                                COURSE_GROUP_MARK) AS COURSE_GROUP_MARK,
                           GC.CIA_GROUP_COMPONENT_ID AS COURSE_GROUP_ID
                        FROM
                            STU_CLASS AS SC
                                INNER JOIN
                            STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                AND SP.IS_DELETED != 1
                                AND SP.IS_LEFT != 1
                                INNER JOIN
                            CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                AND CLS.IS_DELETED != 1
                                INNER JOIN
                            CP_CLASS_COURSE AS CC ON CC.CLASS_ID = CLS.CLASS_ID
                                AND CC.IS_DELETED != 1
                                AND CC.COURSE_ID = ?COURSE_ID
                                AND CC.CLASS_ID = ?CLASS_ID
                                INNER JOIN
                            CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                AND CR.IS_DELETED != 1
                                INNER JOIN
                            CP_COURSE_TYPE_GROUP AS CG ON CG.COURSE_TYPE_ID = CR.COURSE_TYPE
                                AND CG.IS_DELETED != 1
                                INNER JOIN
                            CIA_GROUP_COMPONENT AS GC ON GC.CIA_GROUP_ID = CG.GROUP_ID
                                INNER JOIN
                            SUP_CIA_COMPONENTS AS SCC ON SCC.COMPONENT_ID = GC.SUP_CIA_COMPONENT_ID
                                AND SCC.COMPONENT_ID IN(?COMPONENT_ID)
                                LEFT JOIN
                            CIA_GROUP_MARKS_?AC_YEAR AS CGM ON SC.STUDENT_ID = CGM.STUDENT_ID
                                AND CGM.COURSE_GROUP_ID = GC.CIA_GROUP_COMPONENT_ID
                                AND CGM.COURSE_ID = CR.COURSE_ROOT_ID
                                LEFT JOIN
                            STU_COURSE_REMARK AS RM ON RM.STUDENT_ID = SC.STUDENT_ID
                                AND RM.CLASS_ID =?CLASS_ID
                                AND RM.COURSE_ID = ?COURSE_ID
                                 AND RM.COURSE_GROUP_ID=GC.CIA_GROUP_COMPONENT_ID
                                LEFT JOIN
                            SUP_PART AS P ON P.PART_ID = CR.PART
                        WHERE
                            SC.CLASS_ID = ?CLASS_ID
                                AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                AND SC.IS_DELETED != 1 
                        UNION SELECT 
                            CL.STUDENT_ID,
                            CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                    ' ',
                                    IFNULL(SP.LAST_NAME, '')) AS STUDENT_NAME,
                            SP.ROLL_NO,
                            SP.REGISTER_NO,
                            CI.COURSE_ID,
                            CI.S_CLASS_ID AS CLASS_ID,
                            CLS.CLASS_NAME,
                            RM.REMARK,
                            RM.REMARK_ID,
                            IF(COURSE_GROUP_MARK = '-1',
                                'A',
                                COURSE_GROUP_MARK) AS COURSE_GROUP_MARK,
                            GC.CIA_GROUP_COMPONENT_ID AS COURSE_GROUP_ID
                          
                        FROM
                            STU_COURSE_INFO AS CI
                                INNER JOIN
                            STU_CLASS AS CL ON CI.STUDENT_ID = CL.STUDENT_ID
                                AND CL.ACADEMIC_YEAR = ?AC_YEAR
                                AND CL.IS_DELETED != 1
                                INNER JOIN
                            CP_CLASSES AS CLS ON CLS.CLASS_ID = CL.CLASS_ID
                                AND CLS.IS_DELETED != 1
                                INNER JOIN
                            STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CL.STUDENT_ID
                                AND SP.IS_DELETED != 1
                                INNER JOIN
                            CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CI.COURSE_ID
                                AND CR.IS_DELETED != 1
                                INNER JOIN
                            CP_COURSE_TYPE_GROUP AS CG ON CG.COURSE_TYPE_ID = CR.COURSE_TYPE
                                AND CG.IS_DELETED != 1
                                INNER JOIN
                            CIA_GROUP_COMPONENT AS GC ON GC.CIA_GROUP_ID = CG.GROUP_ID
                                INNER JOIN
                            SUP_CIA_COMPONENTS AS SCC ON SCC.COMPONENT_ID = GC.SUP_CIA_COMPONENT_ID
                                AND SCC.COMPONENT_ID IN(?COMPONENT_ID)
                                LEFT JOIN
                            CIA_GROUP_MARKS_?AC_YEAR AS CGM ON CL.STUDENT_ID = CGM.STUDENT_ID
                                AND CGM.COURSE_GROUP_ID = GC.CIA_GROUP_COMPONENT_ID
                                AND CGM.COURSE_ID = CR.COURSE_ROOT_ID
                                LEFT JOIN
                            STU_COURSE_REMARK AS RM ON RM.STUDENT_ID = CL.STUDENT_ID
                                AND RM.CLASS_ID = ?CLASS_ID
                                AND RM.COURSE_ID = ?COURSE_ID
                                 AND RM.COURSE_GROUP_ID=GC.CIA_GROUP_COMPONENT_ID
                                LEFT JOIN
                            SUP_PART AS P ON P.PART_ID = CR.PART
                        WHERE
                            CI.S_CLASS_ID = ?CLASS_ID
                                AND CI.COURSE_ID = ?COURSE_ID
                             --   AND CI.STAFF_ID = ?STAFF_ID
                                AND CI.ACADEMIC_YEAR = ?AC_YEAR
                                AND CI.IS_DELETED != 1
                        ORDER BY ROLL_NO , REGISTER_NO;";
                        break;
                    }
                case StaffSQLCommands.InsertStuCourseRemark:
                    {
                        query = @"INSERT INTO STU_COURSE_REMARK
                                (
                                CLASS_ID,
                                COURSE_ID,
                                STUDENT_ID,
                                STAFF_ID,
                                REMARK,
                                ACADEMIC_YEAR,
                                COURSE_GROUP_ID)
                                VALUES
                                (
                                ?CLASS_ID,
                                ?COURSE_ID,
                                ?STUDENT_ID,
                                ?STAFF_ID,
                                ?REMARK,
                                ?AC_YEAR,
                                ?COURSE_GROUP_ID
                                );";
                        break;
                    }
                case StaffSQLCommands.UpdateStuCourseRemark:
                    {
                        query = @"UPDATE STU_COURSE_REMARK
                                SET
                                CLASS_ID =?CLASS_ID,
                                COURSE_ID = ?COURSE_ID,
                                STUDENT_ID = ?STUDENT_ID,                               
                                REMARK =?REMARK,                                
                                ACADEMIC_YEAR = ?AC_YEAR,
                                COURSE_GROUP_ID=?COURSE_GROUP_ID
                                WHERE REMARK_ID = ?REMARK_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchStaffDesignationByAcademicYear:
                    {
                        query = @"SELECT 
                                DESIGNATION_ID,
                                ST.STAFF_ID,
                                ST.STAFF_CODE,
                                ST.FIRST_NAME,
                                SPD.DESIGNATION AS DESIGNATION_NAME,
                                SD.DESIGNATION,
                                SD.INCHARGE,
                                CAT.CATEGORY_NAME AS CATEGORY,
                                DP.DEPARTMENT,
                                STF.FIRST_NAME AS INCHARGE_NAME
                            FROM
                                STF_PERSONAL_INFO AS ST
                                    LEFT JOIN
                                STF_DESIGNATION AS SD ON ST.STAFF_ID = SD.STAFF_ID
                                    AND SD.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    AND SD.IS_DELETED!=1
                                    LEFT JOIN
                                SUP_DESIGNATION AS SPD ON SPD.DESIGNATIONID = SD.DESIGNATION
                                    LEFT JOIN
                                STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = SD.INCHARGE
                                    LEFT JOIN
                                sup_category CAT ON CAT.CATEGORY_ID = ST.CATEGORY
                                    LEFT JOIN
                                cp_department AS DP ON DP.DEPARTMENT_ID = ST.DEPARTMENT
                            WHERE
                                ST.SHIFT = ?SHIFT  ORDER BY ST.STAFF_ID;";
                        break;
                    }
                case StaffSQLCommands.InsertStaffDesignation:
                    {
                        query = @"INSERT INTO STF_DESIGNATION 
                                (
                                `STAFF_ID`,
                                `DESIGNATION`,
                                `INCHARGE`,
                                `ACADEMIC_YEAR`)
                                VALUES
                                (?STAFF_ID,
                                ?DESIGNATION,
                                ?INCHARGE,
                                ?ACADEMIC_YEAR);";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffDesignation:
                    {
                        query = @"UPDATE `STF_DESIGNATION`
                                SET
                                `STAFF_ID` = ?STAFF_ID,
                                `DESIGNATION` = ?DESIGNATION,
                                `INCHARGE` = ?INCHARGE,
                                `ACADEMIC_YEAR` = ?ACADEMIC_YEAR,
                                `IS_DELETED`=0
                                WHERE `DESIGNATION_ID` = ?DESIGNATION_ID;";
                        break;
                    }
                case StaffSQLCommands.UpdateIsDeleteForStaffInStaffIncharge:
                    {
                        query = @"UPDATE STF_DESIGNATION SET IS_DELETED=1 WHERE STAFF_ID=?STAFF_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchStaffInchargeByDesignation:
                    {
                        query = @"SELECT DESIGNATION_ID FROM STF_DESIGNATION WHERE STAFF_ID=?STAFF_ID AND DESIGNATION=?DESIGNATION AND ACADEMIC_YEAR=?ACADEMIC_YEAR";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffImage:
                    {
                        query = @"UPDATE STF_PERSONAL_INFO SET PHOTO=?PHOTO WHERE STAFF_ID=?STAFF_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchStaffAppliedList:
                    {
                        query = @"SELECT 
                                SP.STAFF_CODE,
                                SLA.LEAVE_APPLY_ID,
                                SLA.STAFF_ID,
                                LT.LEAVE_TYPE,
                                DATE_FORMAT(SLA.FROM_DATE, '%d/%m/%Y') AS FROM_DATE,
                                DATE_FORMAT(SLA.TO_DATE, '%d/%m/%Y') AS TO_DATE,
                                SLA.DESCRIPTION,
                                SLA.APPROVAL_STATUS1,
                                SLA.APPROVAL_STATUS2,
                                SLS.LEAVE_STATUS AS LEAVE_STATUS1,
                                SLS1.LEAVE_STATUS AS LEAVE_STATUS2,
                                DATE_FORMAT(SLA.APPLY_DATE, '%d/%m/%Y') AS APPLY_DATE,
                                SLA.NO_OF_DAYS
                            FROM
                                LM_STAFF_LEAVE_DETAILS AS SLA
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = SLA.STAFF_ID
                                    INNER JOIN
                                pr_leave_setting AS LS ON LS.STF_LEAVE_SETTING_ID = SLA.LEAVE_SETTING_ID
                                    INNER JOIN
                                sup_leave_type AS LT ON LT.LEAVE_TYPE_ID = LS.LEAVE_TYPE
                                    LEFT JOIN
                                sup_leave_status AS SLS ON SLS.LEAVE_STATUS_ID = SLA.APPROVAL_STATUS1
                                    LEFT JOIN
                                sup_leave_status AS SLS1 ON SLS1.LEAVE_STATUS_ID = SLA.APPROVAL_STATUS2
                            WHERE
                                SLA.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SLA.STAFF_ID = ?STAFF_ID
                                    AND SLA.IS_DELETED != 1;";
                        break;
                    }
                case StaffSQLCommands.FetchLeaveAppliedById:
                    {
                        query = @"SELECT 
                                SP.STAFF_CODE,
                                SLA.LEAVE_APPLY_ID,
                                SLA.STAFF_ID,
                                SLA.LEAVE_SETTING_ID,
                                DATE_FORMAT(SLA.FROM_DATE, '%d/%m/%Y') AS FROM_DATE,
                                DATE_FORMAT(SLA.TO_DATE, '%d/%m/%Y') AS TO_DATE,
                                SLA.DESCRIPTION,
                                SLA.FROM_SESSION,
                                SLA.TO_SESSION
                            FROM
                                LM_STAFF_LEAVE_DETAILS AS SLA
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = SLA.STAFF_ID
                                    
                            WHERE
                                SLA.LEAVE_APPLY_ID = ?LEAVE_APPLY_ID
                                    AND SLA.IS_DELETED != 1;";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffleaveapply:
                    {
                        query = @"UPDATE LM_STAFF_LEAVE_DETAILS
                                 SET 
                                 FROM_DATE=?FROM_DATE,
                                 FROM_SESSION=?FROM_SESSION,
                                 TO_DATE=?TO_DATE,
                                 TO_SESSION=?TO_SESSION,
                                 DESCRIPTION=?DESCRIPTION,
                                 LEAVE_SETTING_ID=?LEAVE_SETTING_ID,
                                 NO_OF_DAYS=?NO_OF_DAYS
                                 WHERE LEAVE_APPLY_ID=?LEAVE_APPLY_ID;";
                        break;
                    }
                case StaffSQLCommands.SaveStaffleaveapply:
                    {
                        query = @"INSERT INTO 
                                LM_STAFF_LEAVE_DETAILS(STAFF_ID, 
                                LEAVE_TYPE_ID,
                                 FROM_DATE, TO_DATE, DESCRIPTION, FROM_SESSION,TO_SESSION,LEAVE_SETTING_ID,NO_OF_DAYS,
                                 ACADEMIC_YEAR)
                                 VALUES(?STAFF_ID, 
                                ?LEAVE_TYPE_ID,
                                 ?FROM_DATE, ?TO_DATE, ?DESCRIPTION,?FROM_SESSION,?TO_SESSION,?LEAVE_SETTING_ID,?NO_OF_DAYS,
                                 ?ACADEMIC_YEAR);";
                        break;
                    }
                case StaffSQLCommands.DeleteAppliedLeave:
                    {
                        query = @"UPDATE STAFF_LEAVE_APPLY 
                                SET 
                                    IS_DELETED = 1
                                WHERE
                                    LEAVE_APPLY_ID = ?LEAVE_APPLY_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchLeaveAppliedStaff:
                    {
                        query = @"SELECT 
                                SP.STAFF_CODE,
                                SP.FIRST_NAME,
                                SP.PHOTO,
                                SLA.LEAVE_APPLY_ID,
                                SLA.STAFF_ID,
                                SLA.LEAVE_TYPE_ID,
                                LT.LEAVE_TYPE,
                                DATE_FORMAT(SLA.FROM_DATE,  '%Y/%m/%d') AS FROM_DATE,
                                DATE_FORMAT(SLA.TO_DATE,  '%Y/%m/%d') AS TO_DATE,
                                DATE_FORMAT(SLA.APPLY_DATE,  '%Y/%m/%d') AS APPLY_DATE,
                                SLA.DESCRIPTION,
                                SLA.APPROVAL_STATUS1,
                                SLA.APPROVAL_STATUS2,
                                SLS1.LEAVE_STATUS AS LEAVE_STATUS1,
                                SLS2.LEAVE_STATUS AS LEAVE_STATUS2

                            FROM
                                LM_STAFF_LEAVE_DETAILS AS SLA
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = SLA.STAFF_ID
                                    INNER JOIN
                                USER_ACCOUNT AS UA ON UA.USER_ID = SP.STAFF_ID
                                    AND UA.IS_DELETED != 1 AND UA.ROLE IN(?ROLE_ID)
                                    INNER JOIN
                                PR_LEAVE_SETTING AS LS ON LS.STF_LEAVE_SETTING_ID = SLA.LEAVE_SETTING_ID
                                    INNER JOIN
                                SUP_LEAVE_TYPE AS LT ON LS.LEAVE_TYPE = LT.LEAVE_TYPE_ID
                                    LEFT JOIN
                                SUP_LEAVE_STATUS AS SLS1 ON SLS1.LEAVE_STATUS_ID = SLA.APPROVAL_STATUS1
                                    LEFT JOIN
                                SUP_LEAVE_STATUS AS SLS2 ON SLS2.LEAVE_STATUS_ID = SLA.APPROVAL_STATUS2
                            WHERE
                                SLA.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SLA.IS_DELETED != 1;";
                        break;
                    }
                case StaffSQLCommands.FetchLeaveAppliedDetails:
                    {
                        query = @"SELECT 
                                SP.STAFF_CODE,
                                SP.FIRST_NAME,
                                SP.PHOTO,
                                SLA.LEAVE_APPLY_ID,
                                SLA.STAFF_ID,
                                SLA.LEAVE_TYPE_ID,
                                LT.LEAVE_TYPE,
                                DATE_FORMAT(SLA.FROM_DATE, '%Y/%m/%d') AS FROM_DATE,
                                DATE_FORMAT(SLA.TO_DATE,  '%Y/%m/%d') AS TO_DATE,
                                DATE_FORMAT(SLA.APPLY_DATE,  '%Y/%m/%d') AS APPLY_DATE,
                                SLA.DESCRIPTION,
                                SLA.APPROVAL_STATUS1,
                                SLA.APPROVAL_STATUS2,
                                SLS1.LEAVE_STATUS AS LEAVE_STATUS1,
                                SLS2.LEAVE_STATUS AS LEAVE_STATUS2,
                                SLA.NO_OF_DAYS,
                                SLA.LEAVE_SETTING_ID,
                                LS.LEAVE_TYPE
                            FROM
                                LM_STAFF_LEAVE_DETAILS AS SLA
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = SLA.STAFF_ID
                                    INNER JOIN
                                PR_LEAVE_SETTING AS LS ON LS.STF_LEAVE_SETTING_ID = SLA.LEAVE_SETTING_ID
                                    INNER JOIN
                                SUP_LEAVE_TYPE AS LT ON LS.LEAVE_TYPE = LT.LEAVE_TYPE_ID
                                    LEFT JOIN
                                SUP_LEAVE_STATUS AS SLS1 ON SLS1.LEAVE_STATUS_ID = SLA.APPROVAL_STATUS1
                                    LEFT JOIN
                                SUP_LEAVE_STATUS AS SLS2 ON SLS2.LEAVE_STATUS_ID = SLA.APPROVAL_STATUS2
                            WHERE
                                SLA.LEAVE_APPLY_ID = ?LEAVE_APPLY_ID
                                    AND SLA.IS_DELETED != 1;";
                        break;
                    }
                case StaffSQLCommands.UpdateLeaveStatus:
                    {
                        query = @"UPDATE LM_STAFF_LEAVE_DETAILS 
                                    SET 
                                        APPROVAL_STATUS1 =?APPROVAL_STATUS1,
                                        APPROVAL_STATUS2 =?APPROVAL_STATUS2
                                    WHERE
                                        LEAVE_APPLY_ID =?LEAVE_APPLY_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchclasslistByStaffId:
                    {
                        query = @"SELECT 
                                    CI.CLASS_ID, CC.CLASS_NAME
                                FROM
                                    CLASS_INCHARGE AS CI
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CI.CLASS_ID = CC.CLASS_ID
                                WHERE
                                    CI.STAFF_ID =?STAFF_ID
                                        AND CI.ACADEMIC_YEAR =?AC_YEAR
                                        AND CI.IS_DELETED != 1;";
                        break;
                    }
                case StaffSQLCommands.FetchClassWiseStudent:
                    {
                        query = @"SELECT 
                                SPI.STUDENT_ID,SPI.FIRST_NAME, SPI.ROLL_NO, SPI.REGISTER_NO
                            FROM
                                stu_class AS SC
                                    INNER JOIN
                                stu_personal_info AS SPI ON SC.STUDENT_ID = SPI.STUDENT_ID
                                    INNER JOIN
                                cp_classes AS CC ON CC.CLASS_ID = SC.CLASS_ID
                            WHERE
                                SC.CLASS_ID = ?CLASS_ID
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SPI.IS_DELETED != 1
                                    AND SPI.IS_LEFT != 1
                                    AND SC.IS_DELETED != 1
                            ORDER BY SPI.REGISTER_NO;";
                        break;
                    }
                case StaffSQLCommands.FetchStudentListByDepartmentId:
                    {
                        query = @"SELECT 
                                    SC.STUDENT_ID,
                                    SPI.FIRST_NAME,
                                    DATE_FORMAT(SPI.DATE_OF_BIRTH, '%Y/%m/%d') AS DATE_OF_BIRTH,
                                    SG.GENDER_NAME AS GENDER,
                                    SPI.ROLL_NO,
                                    SPI.REGISTER_NO,
                                    CC.CLASS_NAME,
                                    IFNULL(SPI.STU_MOBILENO,
                                            IFNULL(SPI.FR_MOBILE, '')) AS STU_MOBILENO
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        AND SPI.IS_LEFT != 1
                                        AND SPI.IS_DELETED != 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                        AND CP.DEPARTMENT IN(?DEPARTMENT_ID)
                                        INNER JOIN
                                    SUP_GENDER AS SG ON SG.GENDER_ID = SPI.GENDER
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                ORDER BY CC.CLASS_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchStaffListByDepartmentId:
                    {
                        query = @"SELECT 
                                SP.STAFF_ID,
                                SP.STAFF_CODE ,
                                SP.FIRST_NAME,
                                SG.GENDER_NAME,
                                SR.RELIGION,
                                DATE_FORMAT(SP.DATE_OF_BIRTH, '%Y/%m/%d') AS DATE_OF_BIRTH,
                                DATE_FORMAT(SP.DATE_OF_JOIN, '%Y/%m/%d') AS DATE_OF_JOIN,
                                SS.SHIFT_NAME
                            FROM
                                STF_PERSONAL_INFO AS SP
                                    LEFT JOIN
                                SUP_GENDER AS SG ON SG.GENDER_ID = SP.GENDER
                                    LEFT JOIN
                                SUP_RELIGION AS SR ON SR.RELIGIONID = SP.RELIGION
                                    LEFT JOIN
                                SUP_SHIFT AS SS ON SS.SHIFT_ID = SP.SHIFT
                            WHERE
                                SP.DEPARTMENT =?DEPARTMENT_ID AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1;";
                        break;
                    }
                case StaffSQLCommands.FetchStaffProfileById:
                    {
                        query = @"SELECT 
                                SP.STAFF_ID,
                                SP.STAFF_CODE,
                                SP.FIRST_NAME,
                                SG.GENDER_NAME,
                                SR.RELIGION,
                                DATE_FORMAT(SP.DATE_OF_BIRTH, '%Y/%m/%d') AS DATE_OF_BIRTH,
                                DATE_FORMAT(SP.DATE_OF_JOIN, '%Y/%m/%d') AS DATE_OF_JOIN,
                                SS.SHIFT_NAME,
                                SP.PHOTO,
                                SSC.STF_CATEGORY,
                                CONCAT(SA.CDOOR_DETAILS,
                                        ',',
                                        SA.CSTREET,
                                        ',',
                                        SA.CPLACE,
                                        ',',
                                        SA.CCITY,
                                        ',',
                                        SA.CPIN_CODE) AS ADDRESS,SA.CCELL_NO AS MOBILE_NO
                            FROM
                                STF_PERSONAL_INFO AS SP
                                    LEFT JOIN
                                SUP_GENDER AS SG ON SG.GENDER_ID = SP.GENDER
                                    LEFT JOIN
                                SUP_RELIGION AS SR ON SR.RELIGIONID = SP.RELIGION
                                    LEFT JOIN
                                SUP_SHIFT AS SS ON SS.SHIFT_ID = SP.SHIFT
                                    LEFT JOIN
                                SUP_STAFF_CATEGORY AS SSC ON SSC.STF_CATEGORY_ID = SP.CATEGORY
                                    LEFT JOIN
                                STF_ADDRESS AS SA ON SA.STAFFNO = SP.STAFF_ID
                            WHERE
                                SP.STAFF_ID =?STAFF_ID AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1;";
                        break;
                    }
                case StaffSQLCommands.FetchStudentListByClassId:
                    {
                        query = @"SELECT 
                        SC.STUDENT_ID,
                        SPI.FIRST_NAME,
                        DATE_FORMAT(SPI.DATE_OF_BIRTH, '%Y/%m/%d') AS DATE_OF_BIRTH,
                        SG.GENDER_NAME AS GENDER,
                        SPI.ROLL_NO,
                        SPI.REGISTER_NO,
                        CC.CLASS_NAME,
                        IFNULL(SPI.STU_MOBILENO,
                                IFNULL(SPI.FR_MOBILE, '')) AS STU_MOBILENO
                    FROM
                        STU_CLASS AS SC
                            INNER JOIN
                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                            AND SPI.IS_LEFT != 1
                            AND SPI.IS_DELETED != 1
                            INNER JOIN
                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                            INNER JOIN
                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                            INNER JOIN
                        SUP_GENDER AS SG ON SG.GENDER_ID = SPI.GENDER
                            INNER JOIN
                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                    WHERE
                        SC.ACADEMIC_YEAR = ?AC_YEAR and SC.CLASS_ID IN(?CLASS_ID)
                    ORDER BY CC.CLASS_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchStudentList:
                    {
                        query = @"SELECT 
                                    SC.STUDENT_ID,
                                    SPI.FIRST_NAME,
                                    DATE_FORMAT(SPI.DATE_OF_BIRTH, '%Y/%m/%d') AS DATE_OF_BIRTH,
                                    SG.GENDER_NAME AS GENDER,
                                    SPI.ROLL_NO,
                                    SPI.REGISTER_NO,
                                    CC.CLASS_NAME,
                                    IFNULL(SPI.STU_MOBILENO,
                                            IFNULL(SPI.FR_MOBILE, '')) AS STU_MOBILENO
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        AND SPI.IS_LEFT != 1
                                        AND SPI.IS_DELETED != 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                        INNER JOIN
                                    SUP_GENDER AS SG ON SG.GENDER_ID = SPI.GENDER
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.IS_DELETED != 1
                                ORDER BY CC.CLASS_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchLeavesettingByCategoryId:
                    {
                        query = @"SELECT 
                                    LS.STF_LEAVE_SETTING_ID,
                                    CONCAT(L.LEAVE_TYPE, '-', LS.ACADEMIC_YEAR) AS LEAVE_TYPE
                                FROM
                                    PR_LEAVE_SETTING AS LS
                                        INNER JOIN
                                    SUP_LEAVE_TYPE AS L ON L.LEAVE_TYPE_ID = LS.LEAVE_TYPE
                                WHERE
                                    LS.IS_DELETED != 1
                                        AND LS.STF_LEAVE_SETTING_ID IN (?STF_LEAVE_SETTING_ID);";
                        break;
                    }
                case StaffSQLCommands.FetchLmTransactionDetailsByStaffId:
                    {
                        query = @"SELECT 
                            LT.TRANSCATIOIN_ID,
                            L.LEAVE_TYPE,
                            SUM(LT.CREDIT_DAYS) AS CREDIT_DAYS,
                            SUM(LT.DEBIT_DAYS) AS DEBIT_DAYS
                        FROM
                            LM_TRANSACTION_DETAILS AS LT
                                INNER JOIN
                            PR_LEAVE_SETTING AS LS ON LS.STF_LEAVE_SETTING_ID = LT.LEAVE_SETTING_ID
                                AND LS.IS_DELETED != 1
                                INNER JOIN
                            SUP_LEAVE_TYPE AS L ON L.LEAVE_TYPE_ID = LS.LEAVE_TYPE
                                INNER JOIN
                            STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = LT.STAFF_ID
                                AND STF.IS_DELETED != 1
                        WHERE
                            LT.IS_DELETED != 1
                                AND LT.ACADEMIC_YEAR = ?AC_YEAR
                                AND LT.STAFF_ID = ?STAFF_ID
                        GROUP BY LEAVE_SETTING_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchCalenderDetails:
                    {
                        query = @"SELECT 
                                DAY_ID,
                                DATE_FORMAT(DAY_ORDER_DATE, '%Y-%m-%d') AS DAY_ORDER_DATE,
                                DAY_TYPE,
                                DAY_ORDER,
                                DAY_ORDER_MONTH,
                                IS_HOLIDAY,
                                IF(CURDATE() = DATE_FORMAT(DAY_ORDER_DATE, '%Y-%m-%d'),
                                    '1',
                                    '0') AS 'IS_CURDATE'
                            FROM
                                CALENDER
                            WHERE
                                IS_DELETED != 1 AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case StaffSQLCommands.FetchLmMaintainDetailsByDateFrom:
                    {
                        query = @"SELECT 
                                LEAVE_APPLY_ID, LEAVE_SETTING_ID
                            FROM
                                LM_STAFF_LEAVE_DETAILS
                            WHERE
                                DATE_FORMAT(FROM_DATE, '%Y-%m-%d') = DATE_FORMAT(?FROM_DATE, '%Y-%m-%d')
                                    OR DATE_FORMAT(TO_DATE, '%Y-%m-%d') = DATE_FORMAT(?FROM_DATE, '%Y-%m-%d')
                                    AND IS_DELETED != 1
                                    AND STAFF_ID = ?STAFF_ID
                                    AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case StaffSQLCommands.FetchApprovalroleByUserId:
                    {
                        query = @"SELECT 
                                    APPROVAL_ID, ROLE_ID, APPROVER_ROLE_ID, APPROVAL_LEVEL
                                FROM
                                    LM_ROLE_BASED_APPROVAL
                                WHERE
                                    APPROVER_ROLE_ID IN (?APPROVER_ROLE_ID);";
                        break;
                    }
                case StaffSQLCommands.FetchLeaveTranscation:
                    {
                        query = @"SELECT 
                                    TRANSCATIOIN_ID,
                                    STAFF_ID,
                                    LEAVE_TYPE,
                                    CREDIT_DAYS,
                                    DEBIT_DAYS,
                                    LEAVE_SETTING_ID,
                                    LEAVE_APPLY_ID
                                FROM
                                    LM_TRANSACTION_DETAILS
                                WHERE
                                    LEAVE_APPLY_ID = ?LEAVE_APPLY_ID AND IS_DELETED!=1;";
                        break;
                    }
                case StaffSQLCommands.SaveLeaveTranscations:
                    {
                        query = @"INSERT INTO 
                            LM_TRANSACTION_DETAILS(STAFF_ID, DEBIT_DAYS
                            ,ACADEMIC_YEAR,
                             LEAVE_SETTING_ID, LEAVE_APPLY_ID,LEAVE_TYPE)
                            VALUES(?STAFF_ID, ?DEBIT_DAYS, ?AC_YEAR, ?LEAVE_SETTING_ID, ?LEAVE_APPLY_ID,?LEAVE_TYPE);";
                        break;
                    }
                case StaffSQLCommands.UpdateIsDeletedByTranscationId:
                    {
                        query = @"UPDATE LM_TRANSACTION_DETAILS 
                                    SET 
                                        IS_DELETED = 1
                                    WHERE
                                        TRANSCATIOIN_ID = ?TRANSCATIOIN_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchRoleBasedApproval:
                    {
                        query = @"SELECT 
                                RBA.APPROVAL_ID,
                                RBA.ROLE_ID,
                                SR.ROLE_NAME,
                                RBA.APPROVER_ROLE_ID,
                                SSR.ROLE_NAME AS APPROVER_ROLE,
                                RBA.APPROVAL_LEVEL,
                                AL.LEVEL_NAME
                            FROM
                                LM_ROLE_BASED_APPROVAL AS RBA
                                    INNER JOIN
                                SUP_ROLE AS SR ON RBA.ROLE_ID = SR.ROLE_ID
                                    INNER JOIN
                                SUP_ROLE AS SSR ON SSR.ROLE_ID = RBA.APPROVER_ROLE_ID
                                    INNER JOIN
                                LM_SUP_APPROVAL_LEVEL AS AL ON AL.LEVEL_ID = RBA.APPROVAL_LEVEL
                            WHERE
                                RBA.IS_DELETED != 1;";
                        break;
                    }
                case StaffSQLCommands.FetchRoleBasedApprovalById:
                    {
                        query = @"SELECT 
                                RBA.APPROVAL_ID,
                                RBA.ROLE_ID,
                                SR.ROLE_NAME,
                                RBA.APPROVER_ROLE_ID,
                                SSR.ROLE_NAME AS APPROVER_ROLE,
                                RBA.APPROVAL_LEVEL,
                                AL.LEVEL_NAME
                            FROM
                                LM_ROLE_BASED_APPROVAL AS RBA
                                    INNER JOIN
                                SUP_ROLE AS SR ON RBA.ROLE_ID = SR.ROLE_ID
                                    INNER JOIN
                                SUP_ROLE AS SSR ON SSR.ROLE_ID = RBA.APPROVER_ROLE_ID
                                    INNER JOIN
                                LM_SUP_APPROVAL_LEVEL AS AL ON AL.LEVEL_ID = RBA.APPROVAL_LEVEL
                            WHERE
                                RBA.IS_DELETED != 1 AND RBA.APPROVAL_ID=?APPROVAL_ID;";
                        break;
                    }
                case StaffSQLCommands.UpdateRoleBasedApproval:
                    {
                        query = @"UPDATE LM_ROLE_BASED_APPROVAL 
                                SET 
                                    ROLE_ID = ?ROLE_ID,
                                    APPROVER_ROLE_ID = ?APPROVER_ROLE_ID,
                                    APPROVAL_LEVEL =?APPROVAL_LEVEL
                                WHERE
                                    APPROVAL_ID = ?APPROVAL_ID;";
                        break;
                    }
                case StaffSQLCommands.SaveRoleBasedApproval:
                    {
                        query = @"INSERT INTO 
                                LM_ROLE_BASED_APPROVAL(ROLE_ID, APPROVER_ROLE_ID, APPROVAL_LEVEL) 
                                VALUES(?ROLE_ID, ?APPROVER_ROLE_ID, ?APPROVAL_LEVEL);";
                        break;
                    }
                case StaffSQLCommands.DeleteRoleBasedApproval:
                    {
                        query = @"UPDATE LM_ROLE_BASED_APPROVAL 
                                    SET 
                                        IS_DELETED = 1
                                    WHERE
                                        APPROVAL_ID = ?APPROVAL_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchLeaveTranscationByStaffId:
                    {
                        query = @"SELECT 
                            SUM(CREDIT_DAYS) AS CREDIT_DAYS,
                            LEAVE_SETTING_ID,
                            SUM(IFNULL(DEBIT_DAYS, 0)) AS DEBIT_DAYS,
                            IS_CARRIED_NEXT_YEAR,
                            ACADEMIC_YEAR
                        FROM
                            LM_TRANSACTION_DETAILS AS TD
                        WHERE
                            TD.IS_DELETED != 1 AND TD.STAFF_ID = ?STAFF_ID
                        GROUP BY TD.LEAVE_SETTING_ID;";
                        break;
                    }
                #region Staff Resume 
                case StaffSQLCommands.FetchListStaffResume:
                    {
                        query = @"SELECT 
                                        SR.RESUME_ID,
                                        SR.STAFF_NAME,
                                        SR.ADDRESS,
                                        SR.QUALIFICATION_ID,
                                        CONTACT_NO,
                                        EMAIL_ID,
                                        R.RELIGION AS RELIGION_ID,
                                        SUBJECT,
                                        DATE_FORMAT(ENTRY_DATE,'%d/%m/%Y') AS ENTRY_DATE
                                    FROM
                                        STF_RESUME AS SR
		                                    INNER JOIN
	                                    SUP_RELIGION AS R ON R.RELIGIONID=SR.RELIGION_ID
                                    WHERE
                                       SR.IS_DELETED!=1;";
                        break;
                    }
                case StaffSQLCommands.FetchStaffResumeByResumeId:
                    {
                        query = @"SELECT 
                                      RESUME_ID,
                                      STAFF_NAME,
                                      ADDRESS,
                                      QUALIFICATION_ID,
                                      CONTACT_NO,
                                      EMAIL_ID,
                                      RELIGION_ID,
                                      SUBJECT
                                  FROM
                                      STF_RESUME
                                  WHERE
                                      RESUME_ID = ?RESUME_ID;";
                        break;
                    }
                case StaffSQLCommands.SaveStaffResume:
                    {
                        query = @"INSERT INTO STF_RESUME
                                  (
	                                  STAFF_NAME,
	                                  ADDRESS,
	                                  QUALIFICATION_ID,
	                                  CONTACT_NO,
	                                  EMAIL_ID,
	                                  RELIGION_ID,
	                                  SUBJECT,
	                                  ENTRY_DATE,
                                      ACADEMIC_YEAR
                                  )
                                  VALUES
                                  (
	                                  ?STAFF_NAME,
	                                  ?ADDRESS,
	                                  ?QUALIFICATION_ID,
	                                  ?CONTACT_NO,
	                                  ?EMAIL_ID,
	                                  ?RELIGION_ID,
	                                  ?SUBJECT,
	                                  NOW(),
                                      ?AC_YEAR
                                  );";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffResume:
                    {
                        query = @"UPDATE STF_RESUME 
                                  SET 
                                      STAFF_NAME = ?STAFF_NAME,
                                      ADDRESS=?ADDRESS,
                                      QUALIFICATION_ID=?QUALIFICATION_ID,
                                      CONTACT_NO=?CONTACT_NO,
                                      EMAIL_ID=?EMAIL_ID,
                                      RELIGION_ID=?RELIGION_ID,
                                      SUBJECT=?SUBJECT
                                  WHERE
                                      RESUME_ID = ?RESUME_ID;";
                        break;
                    }
                case StaffSQLCommands.DeleteStaffResume:
                    {
                        query = @"UPDATE STF_RESUME SET IS_DELETED=1 WHERE RESUME_ID=?RESUME_ID;";
                        break;
                    }
                #endregion


                case StaffSQLCommands.InsertStaffPersonal:
                    {
                        query = @"INSERT INTO STF_PERSONAL_INFO(
                                    FIRST_NAME,
                                    LAST_NAME,
                                    GENDER,
                                    KNOWN_AS,
                                    PLACE_OF_BIRTH,
                                    COMMUNITY,
                                    NATIONALITY,
                                    MARITAL_STATUS,
                                    DATE_OF_JOIN,
                                    DATE_OF_BIRTH,
                                    BLOOD_GROUP,
                                    RELIGION,
                                    MOTHER_TONGUE,
                                    DEPARTMENT,
                                    STAFF_CODE,
                                    SHIFT,
                                    PHOTO,
                                    ROLES,
                                    MOBILE_NO)
                                    VALUES
                                    (
                                      ?FIRST_NAME,
                                      ?LAST_NAME,
                                      ?GENDER,
                                      ?KNOWN_AS,
                                      ?PLACE_OF_BIRTH,
                                      ?COMMUNITY,
                                      ?NATIONALITY,
                                      ?MARITAL_STATUS,
                                      ?DATE_OF_JOIN,
                                      ?DATE_OF_BIRTH,
                                      ?BLOOD_GROUP,
                                      ?RELIGION,
                                      ?MOTHER_TONGUE,
                                      ?DEPARTMENT,
                                      ?STAFF_CODE,
                                      ?SHIFT,
                                      ?PHOTO,
                                      ?ROLES,
                                      ?MOBILE_NO
                                    );";
                        break;
                    }
                case StaffSQLCommands.FetchstfPersonalInfoByID:
                    {
                        query = @" SELECT 
                                        STAFF_ID,
                                        STAFF_CODE,
                                        FIRST_NAME,
                                        LAST_NAME,
                                        KNOWN_AS,
                                        PLACE_OF_BIRTH,
                                        DATE_FORMAT(DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        DATE_FORMAT(DATE_OF_JOIN, '%d/%m/%Y') AS DATE_OF_JOIN,
                                        BLOOD_GROUP,
                                        MARITAL_STATUS,
                                        DEPARTMENT,
                                        SHIFT,
                                        STAFF_STATUS,
                                        MOTHER_TONGUE,
                                        GENDER,
                                        COMMUNITY,
                                        NATIONALITY,
                                        RELIGION,
                                        PHOTO,
                                        IS_LEFT,
                                        QUALIFICATION,
                                        CASTE,
                                        AADTHAR_NO,
                                        DATE_FORMAT(DATE_OF_ENTRY_TEACHING, '%d/%m/%Y') AS DATE_OF_ENTRY_TEACHING,
                                        PAN_NO,
                                        IS_LEFT,
                                        DATE_FORMAT(RETIREMENT_DATE, '%d/%m/%Y') AS RETIREMENT_DATE,
                                        DATE_FORMAT(LEAVING_DATE, '%d/%m/%Y') AS LEAVING_DATE,
                                        STAFF_APPROVAL_NO,
                                        MPHIL_APPROVAL_NO,
                                        PHD_APPROVAL_NO,
                                        FATHER_NAME,
                                        MOTHER_NAME,
                                        SPOUSE_NAME,
                                        ADDRESS_PRESENT,
                                        ADDRESS_PERMANENT,
                                        EPF_NO,
                                        UAN_NO,
                                        PHD_COMPLETION,
                                        NET_COMPLETION,
                                        SET_COMPLETION,
                                        MPHIL_COMPLETION,
                                        PG_COMPLETION,
                                        UG_COMPLETION,
                                        HSC_COMPLETION,
                                        SSLC_COMPLETION,
                                        UG_EXPERIENCE,
                                        PG_EXPERIENCE,
                                        MPHIL_EXPERIENCE,
                                        PHD_EXPERIENCE,
                                        PHYSICAL_STATUS,
                                        BANK_AC_NO,
                                        BANK_NAME,
                                        BRANCH_NAME,
                                        IFSC_CODE,
                                        SALARY_BASIC,
                                        SALARY_DA,
                                        PACKAGE,
                                        SPECIAL_ALLOWANCE,
                                        OTHERS_SALARY,
                                        PP_SALARY,
                                        GROSS_SALARY,
                                        GSUITE_MAIL_ID,
                                        EMAIL,
                                        DESIGNATION,
                                        DATE_FORMAT(DATE_OF_EPF, '%d/%m/%Y') AS DATE_OF_EPF,
                                        MOBILE_NO,
                                        CATEGORY,
                                        ACADEMIC_YEAR
                                    FROM
                                        STF_PERSONAL_INFO
                                    WHERE
                                        STAFF_ID = ?STAFF_ID AND IS_DELETED != 1;";
                        break;
                    }

                case StaffSQLCommands.UpdateStaffPersonalInfo:
                    {
                        query = @"UPDATE STF_PERSONAL_INFO SET
                                                    STAFF_CODE=?STAFF_CODE,
                                                    FIRST_NAME=?FIRST_NAME,
                                                    LAST_NAME=?LAST_NAME,
                                                    KNOWN_AS=?KNOWN_AS,
                                                    PLACE_OF_BIRTH=?PLACE_OF_BIRTH,
                                                    DATE_OF_BIRTH=?DATE_OF_BIRTH,
                                                    DATE_OF_JOIN=?DATE_OF_JOIN,
                                                    BLOOD_GROUP=?BLOOD_GROUP,
                                                    EMAIL=?EMAIL,
                                                    MOBILE_NO=?MOBILE_NO,
                                                    GSUITE_MAIL_ID=?GSUITE_MAIL_ID,
                                                    MARITAL_STATUS=?MARITAL_STATUS,
                                                    DEPARTMENT=?DEPARTMENT,
                                                    DESIGNATION=?DESIGNATION,
                                                    SHIFT=?SHIFT,
                                                    MOTHER_TONGUE=?MOTHER_TONGUE,
                                                    GENDER=?GENDER,
                                                    COMMUNITY=?COMMUNITY,
                                                    NATIONALITY=?NATIONALITY,
                                                    RELIGION=?RELIGION,
                                                    IS_LEFT=?IS_LEFT,
                                                    QUALIFICATION=?QUALIFICATION,
                                                    CASTE=?CASTE,
                                                    AADTHAR_NO=?AADTHAR_NO,
                                                    DATE_OF_ENTRY_TEACHING=?DATE_OF_ENTRY_TEACHING,
                                                    PAN_NO=?PAN_NO,
                                                    RETIREMENT_DATE=?RETIREMENT_DATE,
                                                    LEAVING_DATE=?LEAVING_DATE,
                                                    STAFF_APPROVAL_NO=?STAFF_APPROVAL_NO,
                                                    MPHIL_APPROVAL_NO=?MPHIL_APPROVAL_NO,
                                                    PHD_APPROVAL_NO=?PHD_APPROVAL_NO,
                                                    FATHER_NAME=?FATHER_NAME,
                                                    MOTHER_NAME=?MOTHER_NAME,
                                                    SPOUSE_NAME=?SPOUSE_NAME,
                                                    ADDRESS_PRESENT=?ADDRESS_PRESENT,
                                                    ADDRESS_PERMANENT=?ADDRESS_PERMANENT,
                                                    EPF_NO=?EPF_NO,
                                                    UAN_NO=?UAN_NO,
                                                    PHD_COMPLETION=?PHD_COMPLETION,
                                                    NET_COMPLETION=?NET_COMPLETION,
                                                    SET_COMPLETION=?SET_COMPLETION,
                                                    MPHIL_COMPLETION=?MPHIL_COMPLETION,
                                                    PG_COMPLETION=?PG_COMPLETION,
                                                    UG_COMPLETION=?UG_COMPLETION,
                                                    HSC_COMPLETION=?HSC_COMPLETION,
                                                    SSLC_COMPLETION=?SSLC_COMPLETION,
                                                    UG_EXPERIENCE=?UG_EXPERIENCE,
                                                    PG_EXPERIENCE=?PG_EXPERIENCE,
                                                    MPHIL_EXPERIENCE=?MPHIL_EXPERIENCE,
                                                    PHD_EXPERIENCE=?PHD_EXPERIENCE,
                                                    PHYSICAL_STATUS=?PHYSICAL_STATUS,
                                                    BANK_AC_NO=?BANK_AC_NO,
                                                    BANK_NAME=?BANK_NAME,
                                                    BRANCH_NAME=?BRANCH_NAME,
                                                    IFSC_CODE=?IFSC_CODE,
                                                    SALARY_BASIC=?SALARY_BASIC,
                                                    SALARY_DA=?SALARY_DA,
                                                    PACKAGE=?PACKAGE,
                                                    SPECIAL_ALLOWANCE=?SPECIAL_ALLOWANCE,
                                                    OTHERS_SALARY=?OTHERS_SALARY,
                                                    PP_SALARY=?PP_SALARY,
                                                    GROSS_SALARY=?GROSS_SALARY,
                                                    ROLE=?ROLE,
                                                    USER_TYPE=?USER_TYPE,
                                                    DATE_OF_EPF=?DATE_OF_EPF,
                                                    RFID=?RFID,
                                                    BAR_CODE=?BAR_CODE,
                                                    STAFF_STATUS=?STAFF_STATUS,
                                                    ACADEMIC_YEAR=?ACADEMIC_YEAR
                                                 WHERE 
                                                    STAFF_ID=?STAFF_ID;";
                        break;
                    }
                case StaffSQLCommands.SaveStaffPersonalInfo:
                    {
                        query = @"INSERT INTO STF_PERSONAL_INFO(
                                        STAFF_CODE,
                                        FIRST_NAME,
                                        LAST_NAME,
                                        KNOWN_AS,
                                        PLACE_OF_BIRTH,
                                        DATE_OF_BIRTH,
                                        DATE_OF_JOIN,
                                        BLOOD_GROUP,
                                        MARITAL_STATUS,
                                        DEPARTMENT,
                                        SHIFT,
                                        MOTHER_TONGUE,
                                        GENDER,
                                        COMMUNITY,
                                        NATIONALITY,
                                        RELIGION,
                                        PHOTO,
                                        QUALIFICATION,
                                        CASTE,
                                        AADTHAR_NO,
                                        DATE_OF_ENTRY_TEACHING,
                                        PAN_NO,
                                        IS_LEFT,
                                        RETIREMENT_DATE,
                                        LEAVING_DATE,
                                        STAFF_APPROVAL_NO,
                                        MPHIL_APPROVAL_NO,
                                        PHD_APPROVAL_NO,
                                        FATHER_NAME,
                                        MOTHER_NAME,
                                        SPOUSE_NAME,
                                        ADDRESS_PRESENT,
                                        ADDRESS_PERMANENT,
                                        EPF_NO,
                                        UAN_NO,
                                        PHD_COMPLETION,
                                        NET_COMPLETION,
                                        SET_COMPLETION,
                                        MPHIL_COMPLETION,
                                        PG_COMPLETION,
                                        UG_COMPLETION,
                                        HSC_COMPLETION,
                                        SSLC_COMPLETION,
                                        UG_EXPERIENCE,
                                        PG_EXPERIENCE,
                                        MPHIL_EXPERIENCE,
                                        PHD_EXPERIENCE,
                                        PHYSICAL_STATUS,
                                        BANK_AC_NO,
                                        BANK_NAME,
                                        BRANCH_NAME,
                                        IFSC_CODE,
                                        SALARY_BASIC,
                                        SALARY_DA,
                                        PACKAGE,
                                        SPECIAL_ALLOWANCE,
                                        OTHERS_SALARY,
                                        PP_SALARY,
                                        GROSS_SALARY,
                                        GSUITE_MAIL_ID,
                                        EMAIL,
                                        DESIGNATION,
                                        DATE_OF_EPF,
                                        MOBILE_NO,
                                        CATEGORY,
                                        ROLE,
                                        USER_TYPE,
                                        STAFF_STATUS,
                                        ACADEMIC_YEAR
                                )values(
                                        ?STAFF_CODE,
                                        ?FIRST_NAME,
                                        ?LAST_NAME,
                                        ?KNOWN_AS,
                                        ?PLACE_OF_BIRTH,
                                        ?DATE_OF_BIRTH,
                                        ?DATE_OF_JOIN,
                                        ?BLOOD_GROUP,
                                        ?MARITAL_STATUS,
                                        ?DEPARTMENT,
                                        ?SHIFT,
                                        ?MOTHER_TONGUE,
                                        ?GENDER,
                                        ?COMMUNITY,
                                        ?NATIONALITY,
                                        ?RELIGION,
                                        ?PHOTO,
                                        ?QUALIFICATION,
                                        ?CASTE,
                                        ?AADTHAR_NO,
                                        ?DATE_OF_ENTRY_TEACHING,
                                        ?PAN_NO,
                                        ?IS_LEFT,
                                        ?RETIREMENT_DATE,
                                        ?LEAVING_DATE,
                                        ?STAFF_APPROVAL_NO,
                                        ?MPHIL_APPROVAL_NO,
                                        ?PHD_APPROVAL_NO,
                                        ?FATHER_NAME,
                                        ?MOTHER_NAME,
                                        ?SPOUSE_NAME,
                                        ?ADDRESS_PRESENT,
                                        ?ADDRESS_PERMANENT,
                                        ?EPF_NO,
                                        ?UAN_NO,
                                        ?PHD_COMPLETION,
                                        ?NET_COMPLETION,
                                        ?SET_COMPLETION,
                                        ?MPHIL_COMPLETION,
                                        ?PG_COMPLETION,
                                        ?UG_COMPLETION,
                                        ?HSC_COMPLETION,
                                        ?SSLC_COMPLETION,
                                        ?UG_EXPERIENCE,
                                        ?PG_EXPERIENCE,
                                        ?MPHIL_EXPERIENCE,
                                        ?PHD_EXPERIENCE,
                                        ?PHYSICAL_STATUS,
                                        ?BANK_AC_NO,
                                        ?BANK_NAME,
                                        ?BRANCH_NAME,
                                        ?IFSC_CODE,
                                        ?SALARY_BASIC,
                                        ?SALARY_DA,
                                        ?PACKAGE,
                                        ?SPECIAL_ALLOWANCE,
                                        ?OTHERS_SALARY,
                                        ?PP_SALARY,
                                        ?GROSS_SALARY,
                                        ?GSUITE_MAIL_ID,
                                        ?EMAIL,
                                        ?DESIGNATION,
                                        ?DATE_OF_EPF,
                                        ?MOBILE_NO,
                                        ?CATEGORY,
                                        ?ROLE,
                                        ?USER_TYPE,
                                        ?STAFF_STATUS,
                                        ?ACADEMIC_YEAR
                                        );";
                        break;
                    }
                case StaffSQLCommands.FetchStaffListForExport:
                    {
                        query = @" SELECT 
                                        STAFF_CODE,
                                        FIRST_NAME,
                                        LAST_NAME,
                                        DATE_FORMAT(DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        DATE_FORMAT(DATE_OF_JOIN, '%d/%m/%Y') AS DATE_OF_JOIN,
                                        BLOOD_GROUP_NAME AS BLOOD_GROUP,
                                        MARITAL_STATUS_NAME AS MARITAL_STATUS,
                                        cd.DEPARTMENT,
                                        spi.SHIFT,
                                        sss.STATUS AS STAFF_STATUS,
                                        sg.GENDER_NAME AS GENDER,
                                        sc.COMMUNITY AS COMMUNITY,
                                        sn.NATIONALITY_NAME AS NATIONALITY,
                                        sr.RELIGION,
                                        QUALIFICATION,
                                        CASTE,
                                        AADTHAR_NO,
                                        DATE_FORMAT(DATE_OF_ENTRY_TEACHING, '%d/%m/%Y') AS DATE_OF_ENTRY_TEACHING,
                                        PAN_NO,
                                        DATE_FORMAT(RETIREMENT_DATE, '%d/%m/%Y') AS RETIREMENT_DATE,
                                        DATE_FORMAT(LEAVING_DATE, '%d/%m/%Y') AS LEAVING_DATE,
                                        STAFF_APPROVAL_NO,
                                        MPHIL_APPROVAL_NO,
                                        PHD_APPROVAL_NO,
                                        FATHER_NAME,
                                        MOTHER_NAME,
                                        SPOUSE_NAME,
                                        ADDRESS_PRESENT,
                                        ADDRESS_PERMANENT,
                                        EPF_NO,
                                        UAN_NO,
                                        PHD_COMPLETION,
                                        NET_COMPLETION,
                                        SET_COMPLETION,
                                        MPHIL_COMPLETION,
                                        PG_COMPLETION,
                                        UG_COMPLETION,
                                        HSC_COMPLETION,
                                        SSLC_COMPLETION,
                                        UG_EXPERIENCE,
                                        PG_EXPERIENCE,
                                        MPHIL_EXPERIENCE,
                                        PHD_EXPERIENCE,
                                        PHYSICAL_STATUS,
                                        BANK_AC_NO,
                                        BANK_NAME,
                                        BRANCH_NAME,
                                        IFSC_CODE,
                                        SALARY_BASIC,
                                        SALARY_DA,
                                        PACKAGE,
                                        SPECIAL_ALLOWANCE,
                                        OTHERS_SALARY,
                                        PP_SALARY,
                                        GROSS_SALARY,
                                        GSUITE_MAIL_ID,
                                        EMAIL,
                                        sd.DESIGNATION,
                                        DATE_FORMAT(DATE_OF_EPF, '%d/%m/%Y') AS DATE_OF_EPF,
                                        MOBILE_NO,
                                        sca.CATEGORY_NAME AS CATEGORY,
                                        ay.AC_YEAR AS ACADEMIC_YEAR
                                    FROM
                                        STF_PERSONAL_INFO AS spi
                                            LEFT JOIN
                                        sup_blood_group AS sbg ON sbg.BLOOD_GROUP_ID = spi.BLOOD_GROUP
                                            LEFT JOIN
                                        sup_marrital_status AS sms ON sms.MARITAL_STAUS_ID = spi.MARITAL_STATUS
                                            LEFT JOIN
                                        cp_department AS cd ON cd.DEPARTMENT_ID = spi.DEPARTMENT
                                            LEFT JOIN
                                        sup_shift AS ss ON FIND_IN_SET(ss.SHIFT_ID, SPI.SHIFT)
                                            LEFT JOIN
                                        sup_gender AS sg ON sg.GENDER_ID = spi.GENDER
                                            LEFT JOIN
                                        sup_community AS sc ON sc.COMMUNITYID = spi.COMMUNITY
                                            LEFT JOIN
                                        sup_nationality AS sn ON sn.NATIONALITY_ID = spi.NATIONALITY
                                            LEFT JOIN
                                        sup_religion AS sr ON sr.RELIGIONID = spi.RELIGION
                                            LEFT JOIN
                                        sup_category AS sca ON sca.CATEGORY_ID = spi.CATEGORY
                                            LEFT JOIN
                                        sup_designation AS sd ON sd.DESIGNATIONID = spi.DESIGNATION
                                            LEFT JOIN
                                        sup_staff_status AS sss ON sss.STATUS_ID = spi.STAFF_STATUS
                                            LEFT JOIN
                                        ACADEMIC_YEAR AS ay ON ay.ACADEMIC_YEAR_ID = spi.ACADEMIC_YEAR
                                    WHERE
                                        spi.IS_DELETED != 1 AND spi.is_left != 1
                                    GROUP BY STAFF_ID
                                    ORDER BY STAFF_CODE; ";
                        break;
                    }
                case StaffSQLCommands.FetchStaffPayslipMonthWise:
                    {
                        query = @"SELECT 
                                    STAFF_ID,
                                    STAFF_CODE,
                                    FIRST_NAME,
                                    MOBILE_NO,
                                    LAST_NAME,
                                    DATE_FORMAT(DATE_OF_JOIN,'%d/%m/%Y') as DATE_OF_JOIN,
                                    DATE_FORMAT(DATE_OF_BIRTH,'%d/%m/%Y') as DATE_OF_BIRTH,
                                    BLOOD_GROUP,
                                    RELIGION,
                                    sd.DESIGNATION,
                                    sdm.DEPARTMENT,
                                    sg.GENDER_NAME AS GENDER,
                                    sc.COMMUNITY,
                                    PHOTO,
                                    QUALIFICATION,
                                    GROSS_SALARY,
                                    PP_SALARY,
                                    OTHERS_SALARY,
                                    SPECIAL_ALLOWANCE,
                                    PACKAGE,
                                    SALARY_DA,
                                    SALARY_BASIC,
                                    IFSC_CODE,
                                    BRANCH_NAME,
                                    BANK_NAME,
                                    BANK_AC_NO,
                                    EMAIL,
                                    UAN_NO,
                                    EPF_NO,
                                    MOTHER_NAME,
                                    VOTER_ID,
                                    FATHER_NAME,
                                    DATE_FORMAT(DATE_OF_ENTRY_TEACHING,'%d/%m/%Y') as DATE_OF_ENTRY_TEACHING,
                                    PAN_NO,
                                    AADTHAR_NO
                                FROM
                                    stf_personal_info AS spi
                                        LEFT JOIN
                                    sup_designation AS sd ON sd.DESIGNATIONID = spi.DESIGNATION
                                        LEFT JOIN
                                    sup_department AS sdm ON sdm.DEPARTMENTID = spi.DEPARTMENT
                                        LEFT JOIN
                                    sup_gender AS sg ON sg.GENDER_ID = spi.GENDER
                                        LEFT JOIN
                                    sup_community AS sc ON sc.COMMUNITYID = spi.COMMUNITY
                                WHERE
                                    spi.STAFF_ID = ?STAFF_ID AND spi.IS_DELETED != 1;";
                        break;
                    }
                case StaffSQLCommands.FetchStaffListForCommunity:
                    {
                        query = @"SELECT 
                                    SPI.STAFF_CODE,
                                    SPI.FIRST_NAME,
                                    SPI.CATEGORY,
                                    SSC.STF_CATEGORY,
                                    SPI.CASTE,
                                    SC.COMMUNITYID,
                                    SC.COMMUNITY,
                                    SD.DEPARTMENT,
                                    GENDER_NAME as GENDER
                                FROM
                                    STF_PERSONAL_INFO AS SPI
                                        LEFT JOIN
                                    SUP_STAFF_CATEGORY AS SSC ON SSC.STF_CATEGORY_ID = SPI.CATEGORY
                                        LEFT JOIN
                                    SUP_COMMUNITY AS SC ON SC.COMMUNITYID = SPI.COMMUNITY
                                        LEFT JOIN
                                    CP_DEPARTMENT AS SD ON SD.DEPARTMENT_ID IN (SPI.DEPARTMENT)
                                        LEFT JOIN
                                    SUP_GENDER AS SG ON SG.GENDER_ID = SPI.GENDER
                                WHERE
                                    SPI.IS_LEFT != 1 AND SPI.IS_DELETED != 1
                                        AND SPI.CATEGORY IN (?CATEGORY)
                                ORDER BY STF_CATEGORY DESC, COMMUNITYORDER , DEPARTMENT , GENDER_NAME;";
                        break;
                    }
                case StaffSQLCommands.FetchStaffListbyStaffId:
                    {
                        query = @" SELECT 
                                    STAFF_ID,
                                    STAFF_CODE,
                                    FIRST_NAME,
                                    MOBILE_NO
                                FROM
                                    STF_PERSONAL_INFO 
                                WHERE
                                    STAFF_ID =?STAFF_ID;";
                        break;
                    }
                    case StaffSQLCommands.FetchStaffProfilebyStaffId:
                    {
                        query = @"SELECT 
                                    SPI.STAFF_ID,
                                    SPI.STAFF_CODE,
                                    PROFILE_ID,
                                    IFNULL(sp.STAFF_NAME, spi.first_name) AS STAFF_NAME,
                                    IFNULL(sp.ADDRESS, ADDRESS_PERMANENT) AS ADDRESS,
                                    IFNULL(sp.QUALIFICATION, spi.QUALIFICATION) AS QUALIFICATION,
                                    IFNULL(sp.CONTACT_NO, spi.MOBILE_NO) AS CONTACT_NO,
                                    IFNULL(sp.EMAIL_ID, spi.EMAIL) AS EMAIL_ID,
                                    IFNULL(SP.COLLEGE_EMAIL_ID, SPI.GSUITE_MAIL_ID) AS COLLEGE_EMAIL_ID,
                                    IFNULL(sp.PHOTO, spi.PHOTO) AS PHOTO,
                                    sp.SPECIALIZATION,
                                    sp.DESIGNATION,
                                    sp.PHD_SUPERVISOR,
                                    sp.NEW_ACCEPTING_PHD,
                                    sp.LINKEDIN_LINK,
                                    sp.GOOGLE_SCHOLAR,
                                    sp.SCOPUS_ID,
                                    sp.STF_ROLE_RES_ID,
                                    sp.IRINS_ID,
                                    sp.ORCID_ID,
                                    sp.RESEARCHER_ID,
                                    sp.FILE_PATH,
                                    sp.PHD_AWARDEES,
                                    sp.IS_EMAIL_SHOW,
                                    sp.IS_CONTACT_NO_SHOW,
                                    DATE_FORMAT(SPI.DATE_OF_JOIN, '%d/%m/%Y') AS DATE_OF_JOIN,
                                    IFNULL(sp.DEPARTMENT, spi.DEPARTMENT) AS DEPARTMENT
                                FROM
                                    STF_PERSONAL_INFO AS SPI
                                        LEFT JOIN
                                    STF_PROFILE AS SP ON SPI.STAFF_ID = SP.STAFF_ID
                                        AND SP.IS_DELETED != 1
                                WHERE
                                    SPI.IS_DELETED != 1 
                                    AND SPI.IS_LEFT != 1
	                                AND spi.STAFF_ID = ?STAFF_ID;";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffProfileByID:
                    {
                        query = @"UPDATE  STF_PROFILE  
                                          SET 
                                            STAFF_NAME =?STAFF_NAME,
                                            ADDRESS =?ADDRESS,
                                            QUALIFICATION =?QUALIFICATION,
                                            CONTACT_NO =?CONTACT_NO,
                                            EMAIL_ID =?EMAIL_ID,
                                            DESIGNATION =?DESIGNATION,
                                            SPECIALIZATION =?SPECIALIZATION,
                                            PHD_SUPERVISOR =?PHD_SUPERVISOR,
                                            NEW_ACCEPTING_PHD =?NEW_ACCEPTING_PHD,
                                            LINKEDIN_LINK =?LINKEDIN_LINK,
                                            GOOGLE_SCHOLAR =?GOOGLE_SCHOLAR,
                                            SCOPUS_ID =?SCOPUS_ID,
                                            IRINS_ID =?IRINS_ID,
                                            ORCID_ID =?ORCID_ID,
                                            STAFF_ID =?STAFF_ID,
                                            IS_EMAIL_SHOW =?IS_EMAIL_SHOW,
                                            IS_CONTACT_NO_SHOW =?IS_CONTACT_NO_SHOW,
                                            COLLEGE_EMAIL_ID =?COLLEGE_EMAIL_ID,
                                            RESEARCHER_ID =?RESEARCHER_ID,
                                            LAST_UPDATE_DATE =?LAST_UPDATE_DATE,
                                            PHD_AWARDEES =?PHD_AWARDEES,
                                            STF_ROLE_RES_ID =?STF_ROLE_RES_ID,
                                            DEPARTMENT =?DEPARTMENT            
  
                                        WHERE 
                                            PROFILE_ID=?PROFILE_ID;";
                        break;
                    }
                case StaffSQLCommands.SaveStaffProfileByID:
                    {
                        query = @"INSERT INTO STF_PROFILE( 
                                                    STAFF_NAME,
                                                    ADDRESS,
                                                    QUALIFICATION,
                                                    CONTACT_NO,
                                                    EMAIL_ID,
                                                    DESIGNATION,
                                                    SPECIALIZATION,
                                                    PHD_SUPERVISOR,
                                                    NEW_ACCEPTING_PHD,
                                                    LINKEDIN_LINK,
                                                    GOOGLE_SCHOLAR,
                                                    SCOPUS_ID,
                                                    IRINS_ID,
                                                    ORCID_ID,
                                                    STAFF_ID,
                                                    IS_EMAIL_SHOW,
                                                    IS_CONTACT_NO_SHOW,
                                                    COLLEGE_EMAIL_ID,
                                                    RESEARCHER_ID,
                                                    LAST_UPDATE_DATE,
                                                    PHD_AWARDEES,
                                                    STF_ROLE_RES_ID,
                                                    DEPARTMENT
                                                )VALUES(
                                                    ?STAFF_NAME,
                                                    ?ADDRESS,
                                                    ?QUALIFICATION,
                                                    ?CONTACT_NO,
                                                    ?EMAIL_ID,
                                                    ?DESIGNATION,
                                                    ?SPECIALIZATION,
                                                    ?PHD_SUPERVISOR,
                                                    ?NEW_ACCEPTING_PHD,
                                                    ?LINKEDIN_LINK,
                                                    ?GOOGLE_SCHOLAR,
                                                    ?SCOPUS_ID,
                                                    ?IRINS_ID,
                                                    ?ORCID_ID,
                                                    ?STAFF_ID,
                                                    ?IS_EMAIL_SHOW,
                                                    ?IS_CONTACT_NO_SHOW,
                                                    ?COLLEGE_EMAIL_ID,
                                                    ?RESEARCHER_ID,
                                                    ?LAST_UPDATE_DATE,
                                                    ?PHD_AWARDEES,
                                                    ?STF_ROLE_RES_ID,
                                                    ?DEPARTMENT);";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffProfiletimage:
                    {
                        query = @"UPDATE STF_PROFILE SET PHOTO=?PHOTO WHERE PROFILE_ID=?PROFILE_ID;";
                        break;
                    }

                    case StaffSQLCommands.FetchAllStfProfileForApproval:
                    {
                        query = @"SELECT 
                                    SPI.STAFF_ID,
                                    SPI.STAFF_CODE,
                                    SP.PROFILE_ID,
                                    IFNULL(sp.STAFF_NAME, spi.first_name) AS STAFF_NAME,
                                    IFNULL(sp.ADDRESS, ADDRESS_PERMANENT) AS ADDRESS,
                                    IFNULL(sp.QUALIFICATION, spi.QUALIFICATION) AS QUALIFICATION,
                                    IFNULL(sp.CONTACT_NO, spi.MOBILE_NO) AS CONTACT_NO,
                                    IFNULL(sp.EMAIL_ID, spi.EMAIL) AS EMAIL_ID,
                                    IFNULL(SP.COLLEGE_EMAIL_ID, SPI.GSUITE_MAIL_ID) AS COLLEGE_EMAIL_ID,
                                    IFNULL(sp.PHOTO, spi.PHOTO) AS PHOTO,
                                    sp.SPECIALIZATION,
                                    sp.DESIGNATION,
                                    sp.PHD_SUPERVISOR,
                                    sp.NEW_ACCEPTING_PHD,
                                    sp.LINKEDIN_LINK,
                                    sp.GOOGLE_SCHOLAR,
                                    sp.SCOPUS_ID,
                                    sp.IRINS_ID,
                                    sp.ORCID_ID,
                                    sp.RESEARCHER_ID,
                                    sp.IS_EMAIL_SHOW,
                                    sp.IS_CONTACT_NO_SHOW,
                                    sp.IS_SHOW_WEBSITE,
                                    DATE_FORMAT(sp.LAST_UPDATE_DATE, '%Y-%m-%d %H:%i:%s') AS LAST_UPDATE_DATE,
                                    cd.DEPARTMENT AS DEPARTMENT
                                FROM
                                    STF_PERSONAL_INFO AS SPI
                                        LEFT JOIN
                                    STF_PROFILE AS SP ON SPI.STAFF_ID = SP.STAFF_ID
                                        AND SP.IS_DELETED != 1
                                         LEFT JOIN
                                    CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = IFNULL(SP.DEPARTMENT, SPI.DEPARTMENT)
                                WHERE
                                    SPI.IS_DELETED != 1 
                                    AND SPI.IS_LEFT != 1 ORDER BY DEPARTMENT,STAFF_NAME;";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffProfileApprovalStaff:
                    {
                        query = @"UPDATE STF_PROFILE 
                                            SET 
                                                APPROVER_ID = ?APPROVER_ID,
                                                APPROVED_DATE = ?APPROVED_DATE,
                                                IS_SHOW_WEBSITE = ?IS_SHOW_WEBSITE
                                            WHERE
                                                PROFILE_ID = ?PROFILE_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchStfProfileForApprovalbyDeptID:
                    {
                        query = @"SELECT 
                                    SPI.STAFF_ID,
                                    SPI.STAFF_CODE,
                                    SP.PROFILE_ID,
                                    IFNULL(sp.STAFF_NAME, spi.FIRST_NAME) AS STAFF_NAME,
                                    IFNULL(sp.ADDRESS, ADDRESS_PERMANENT) AS ADDRESS,
                                    IFNULL(sp.QUALIFICATION, spi.QUALIFICATION) AS QUALIFICATION,
                                    IFNULL(sp.CONTACT_NO, spi.MOBILE_NO) AS CONTACT_NO,
                                    IFNULL(sp.EMAIL_ID, spi.EMAIL) AS EMAIL_ID,
                                    IFNULL(SP.COLLEGE_EMAIL_ID, SPI.GSUITE_MAIL_ID) AS COLLEGE_EMAIL_ID,
                                    IFNULL(sp.PHOTO, spi.PHOTO) AS PHOTO,
                                    sp.SPECIALIZATION,
                                    sp.DESIGNATION,
                                    sp.PHD_SUPERVISOR,
                                    sp.NEW_ACCEPTING_PHD,
                                    sp.LINKEDIN_LINK,
                                    sp.GOOGLE_SCHOLAR,
                                    sp.SCOPUS_ID,
                                    sp.IRINS_ID,
                                    sp.ORCID_ID,
                                    sp.RESEARCHER_ID,
                                    sp.IS_EMAIL_SHOW,
                                    sp.IS_CONTACT_NO_SHOW,
                                    sp.IS_SHOW_WEBSITE,
                                    DATE_FORMAT(sp.LAST_UPDATE_DATE, '%Y-%m-%d %H:%i:%s') AS LAST_UPDATE_DATE,
                                    cd.DEPARTMENT AS DEPARTMENT
                                FROM
                                    STF_PERSONAL_INFO AS SPI
                                        LEFT JOIN
                                    STF_PROFILE AS SP ON SPI.STAFF_ID = SP.STAFF_ID
                                        AND SP.IS_DELETED != 1
                                         LEFT JOIN
                                    CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = IFNULL(SP.DEPARTMENT, SPI.DEPARTMENT)
                                WHERE
                                    SPI.IS_DELETED != 1 AND IFNULL(SP.DEPARTMENT, SPI.DEPARTMENT) in (?DEPARTMENT)
                                    AND SPI.IS_LEFT != 1 ORDER BY DEPARTMENT,STAFF_NAME;";
                        break;
                    }
                case StaffSQLCommands.UpdateStaffProfileFiles:
                    {
                        query = @"UPDATE stf_profile SET FILE_PATH=?FILE_PATH WHERE PROFILE_ID = ?PROFILE_ID;";
                        break;
                    }
                case StaffSQLCommands.FetchStaffDetailsByDepartmentId:
                    {
                        query = @"SELECT 
                                        SPI.STAFF_CODE,
                                        SPI.FIRST_NAME,
                                        SPI.CATEGORY,
                                        SSC.STF_CATEGORY,
                                        SPI.CASTE,
                                        SC.COMMUNITYID,
                                        SC.COMMUNITY,
                                        SD.DEPARTMENT,
                                        sdn.DESIGNATION,
                                        GENDER_NAME AS GENDER,
                                        SPI.GENDER AS GENDER_ID
                                    FROM
                                        STF_PERSONAL_INFO AS SPI
                                            LEFT JOIN
                                        SUP_STAFF_CATEGORY AS SSC ON SSC.STF_CATEGORY_ID = SPI.CATEGORY
                                            LEFT JOIN
                                        SUP_COMMUNITY AS SC ON SC.COMMUNITYID = SPI.COMMUNITY
                                            LEFT JOIN
                                        CP_DEPARTMENT AS SD ON SD.DEPARTMENT_ID IN (SPI.DEPARTMENT)
                                            LEFT JOIN
                                        SUP_GENDER AS SG ON SG.GENDER_ID = SPI.GENDER
                                            LEFT JOIN 
                                        SUP_DESIGNATION AS SDN ON SDN.DESIGNATIONID = SPI.DESIGNATION
                                    WHERE
                                        SPI.IS_LEFT != 1 AND SPI.IS_DELETED != 1
                                            AND SPI.DEPARTMENT IN (?DEPARTMENT)
                                    ORDER BY SPI.DEPARTMENT,SPI.STAFF_ORDER,STF_CATEGORY DESC , COMMUNITYORDER , DEPARTMENT , GENDER_NAME;";
                        break;
                    }
                case StaffSQLCommands.FetchProgrammelistByStaffId:
                    {
                        query = @"SELECT 
                                      CPG.PROGRAMME_ID, CP.PROGRAMME_DESCRIPTION
                                  FROM
                                      cp_programme_group AS CPG
                                          INNER JOIN
                                      cp_programme AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                  WHERE
                                      CPG.PROGRAMME_INCHARGE_ID = ?STAFF_ID
                                          AND CPG.IS_DELETED != 1;";
                        break;
                    }
                case StaffSQLCommands.FetchStaffListByUserId:
                    {
                        query = @"SELECT 
                                    SPI.STAFF_ID,
                                    SPI.STAFF_CODE,
                                    SPI.FIRST_NAME,
                                    D.DEPARTMENT,
                                    G.GENDER_NAME,
                                    GROUP_CONCAT(S.SHIFT_NAME) AS SHIFT_NAME,
                                    SPI.IS_LEFT,
                                    DATE_FORMAT(SPI.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                    SPI.PHOTO
                                FROM
                                    STF_PERSONAL_INFO SPI
                                        LEFT JOIN
                                    CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = SPI.DEPARTMENT
                                        LEFT JOIN
                                    SUP_GENDER AS G ON G.GENDER_ID = SPI.GENDER
                                        LEFT JOIN
                                    SUP_SHIFT AS S ON FIND_IN_SET(S.SHIFT_ID, SPI.SHIFT)
                                WHERE
                                    SPI.IS_DELETED != 1
                                    AND (SPI.STAFF_ID = ?STAFF_ID OR SPI.STAFF_CODE = ?STAFF_ID)
                                GROUP BY SPI.STAFF_ID
                                ORDER BY SPI.STAFF_ID;";
                        break;
                    }
                default:
                    break;
            }

            return query;
        }
    }
}
