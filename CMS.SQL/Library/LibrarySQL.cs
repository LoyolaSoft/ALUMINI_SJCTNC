using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAO;

namespace CMS.SQL.Library
{
    public class LibrarySQL
    {
        public static string GetLibrarySQL(LibrarySQLCommands sEnumCommand)
        {
            string sQuery = string.Empty;
            switch (sEnumCommand)
            {
                #region Book Details
                case LibrarySQLCommands.FetchBookInfoByAuthorForDocTitleCountSubject:
                    {
                        sQuery = @"SELECT 
                                        BOOK_ID,
                                        RACK_ID,
                                        ACCESS_NO,
                                        DOC_TITLE,
										'DOC_TITLE' AS MEMBER_TYPE,
                                        count(*) as TOTAL_COUNT
                                    FROM
                                        LI_BOOK_INFO AS BI
                                            LEFT JOIN
                                        SUP_LI_SUBJECT_TYPE AS S ON S.SUBJECT_ID = BI.SUBJECT_ID
                                            LEFT JOIN
                                        SUP_LI_CLASS_NO AS CN ON CN.CLASS_NO_ID = BI.CLASS_NO_ID
                                    WHERE
                                        BI.IS_DELETED != 1
                                            
                                            AND BI.SUBJECT_ID IN(?SUBJECT_ID) 
											AND BI.ACADEMIC_YEAR IN (?ACADEMIC_YEAR)
                                             group by DOC_TITLE
                                            ORDER BY DOC_TITLE;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookInfoBySubjectid:
                    {
                        sQuery = @"SELECT 
									BI.BOOK_ID,
									RACK_ID,
									ACCESS_NO,
									CN.CLASS_NO,
									BOOK_NO,
									LANGUAGE,
									CONCAT(PERSONAL_AUTHOR1,',',IFNULL(PERSONAL_AUTHOR2,'')) AS PERSONAL_AUTHOR1,
									JOINED_AUTHOR1,
									JOINED_AUTHOR2,
									CORPORATE_AUTHOR1,
									CORPORATE_AUTHOR2,
									DOC_TITLE,
									PARALLEL_TITLE,
									NOTES,
									PHYDESC,
									ILLUSTRATE,
									STATUS,
									PRICE,
									DATE_FORMAT(DOP, '%d/%m/%Y') AS DOP,
									ISBN_NO,
									DISIPLINE,
									TAPS,
									DETAILS,
									VC,
									NAME_OF_PUBLISHER,
									PLACE_OF_PUBLISHER,
									SERIES,
									LIB_CATEGORY,
									DOC_CATEGORY,
									BOOK_PUBLISH_YEAR,
									SUB_ACCOUNT,
									KEYWORD,
									INVOICE,
									ACADEMIC_YEAR,
									LI_BOOK_STATUS,
									ENTRY_DATE,
									RFID,
									BARCODE,
									BILL_NO,
									BILL_AMOUNT,
									CALL_NO,
									BI.SUBJECT_ID,
									SECTION_ID,
									DATE_FORMAT(BILL_DATE, '%d/%m/%Y') AS BILL_DATE,
									SUPPLIER_NAME,
									SUPPLIER_ADDRESS,
									BI.CLASS_NO_ID,
									S.SUBJECT,
                                    if(bi.IS_WEEDED_BOOK =0 ,'No' ,'Yes')AS IS_WEEDED_BOOK,
                                    BI.RACK_NO,
									ss.STATUS_NAME,
                                    LBS.STATUS_NAME AS LI_STATUS_NAME,
                                    LBI.IS_RETURNED,
					                UA.USERNAME,
                                    from_unixtime(LBI.ISSUE_DATE, '%Y-%m-%d') AS ISSUE_DATE,
                                    from_unixtime(LBI.ACTUAL_RETURNED_DATE, '%Y-%m-%d') AS RETURNED_DATE

                                FROM
									LI_BOOK_INFO AS BI
										LEFT JOIN
									SUP_LI_SUBJECT_TYPE AS S ON S.SUBJECT_ID = BI.SUBJECT_ID
										LEFT JOIN
									SUP_LI_CLASS_NO AS CN ON CN.CLASS_NO_ID = BI.CLASS_NO_ID
										INNER JOIN
									sup_li_status AS ss ON ss.STATUS_ID = bi.STATUS
                                         LEFT JOIN
					                 SUP_LI_BOOK_STATUS AS LBS ON LBS.STATUS_ID = BI.LI_BOOK_STATUS
					               	     LEFT JOIN
					                 LI_BOOK_ISSUE_?AC_YEAR AS LBI ON LBI.BOOK_ID = BI.BOOK_ID
					               			and lbi.IS_RETURNED !=1
					               		LEFT JOIN
					                 USER_ACCOUNT AS UA ON UA.USER_ID = LBI.USER_ID
					               				AND UA.ROLE = LBI.ROLE
								WHERE
									BI.IS_DELETED != 1
										AND BI.SUBJECT_ID IN(?SUBJECT_ID) AND BI.ACADEMIC_YEAR IN(?ACADEMIC_YEAR_ID)  group by ACCESS_NO ORDER BY CAST(ACCESS_NO AS SIGNED);";
                        break;
                    }
                case LibrarySQLCommands.ListBookDetails:
                    {
                        sQuery = @"SELECT 
                                        LD.BOOK_ID,
                                        CONCAT(IFNULL(SR.RACK_INFO, ''),
                                                ' ( ',
                                                R.RACK_NAME,
                                                ' - ',
                                                C.CELL_NAME,
                                                ' ) ') AS RACK_ID,
                                        LD.ACCESS_NO,
                                        SLC.CLASS_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                        PERSONAL_AUTHOR1,
                                        PERSONAL_AUTHOR2,
                                        JOINED_AUTHOR1,
                                        JOINED_AUTHOR2,
                                        CORPORATE_AUTHOR1,
                                        CORPORATE_AUTHOR2,
                                        LD.DOC_TITLE,
                                        LD.PARALLEL_TITLE,
                                        LD.NOTES,
                                        LD.PHYDESC,
                                        LD.ILLUSTRATE,
                                        SS.STATUS_NAME AS STATUS,
                                        LD.PRICE,
                                        DATE_FORMAT(LD.DOP, '%Y/%m/%d') AS DOP,
                                        LD.ISBN_NO,
                                        LD.DISIPLINE,
                                        LD.NAME_OF_PUBLISHER,
                                        LD.PLACE_OF_PUBLISHER,
                                        LD.RFID,
                                        LD.BARCODE,
                                        LD.BOOK_PUBLISH_YEAR
                                    FROM
                                        LI_BOOK_INFO AS LD
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = LD.LANGUAGE
                                            LEFT JOIN
                                        SUP_LI_STATUS AS SS ON SS.STATUS_ID = LD.STATUS
                                            AND SS.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_LI_CLASS_NO AS SLC ON SLC.CLASS_NO_ID = LD.CLASS_NO
                                            AND SLC.IS_DELETED != 1
                                            INNER JOIN
                                        LI_RACK_INFO AS SR ON SR.RACK_INFO_ID = LD.RACK_ID
                                            AND SR.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_LI_RACKS AS R ON R.RACK_ID = SR.RACK_ID
                                            INNER JOIN
                                        SUP_LI_CELLS AS C ON C.CELL_ID = SR.CELL_ID
                                    WHERE
                                        LD.IS_DELETED != 1
                                    GROUP BY LD.BOOK_ID
                                    ORDER BY LD.BOOK_ID;";
                    }
                    break;
                case LibrarySQLCommands.SaveBookDetails:
                    {
                        sQuery = @"INSERT INTO LI_BOOK_INFO
                                    (
	                                    RACK_ID,
	                                    ACCESS_NO,
	                                    CLASS_NO,
	                                    BOOK_NO,
	                                    LANGUAGE,
	                                    PERSONAL_AUTHOR1,
	                                    PERSONAL_AUTHOR2,
	                                    JOINED_AUTHOR1,
	                                    JOINED_AUTHOR2,
	                                    CORPORATE_AUTHOR1,
	                                    CORPORATE_AUTHOR2,
	                                    DOC_TITLE,
	                                    PARALLEL_TITLE,
	                                    NOTES,
	                                    PHYDESC,
	                                    ILLUSTRATE,
	                                    STATUS,
	                                    PRICE,
	                                    DOP,
	                                    ISBN_NO,
	                                    DISIPLINE,
	                                    TAPS,
	                                    DETAILS,
	                                    VC,
	                                    NAME_OF_PUBLISHER,
	                                    PLACE_OF_PUBLISHER,
	                                    SERIES,
	                                    LIB_CATEGORY,
	                                    DOC_CATEGORY,
	                                    BOOK_PUBLISH_YEAR,
	                                    SUB_ACCOUNT,
	                                    KEYWORD,
	                                    INVOICE,
	                                    ACADEMIC_YEAR,
	                                    RFID,
	                                    BARCODE
                                    )
                                    VALUES
                                    (
                                        ?RACK_ID,
	                                    ?ACCESS_NO,
	                                    ?CLASS_NO,
	                                    ?BOOK_NO,
	                                    ?LANGUAGE,
	                                    ?PERSONAL_AUTHOR1,
	                                    ?PERSONAL_AUTHOR2,
	                                    ?JOINED_AUTHOR1,
	                                    ?JOINED_AUTHOR2,
	                                    ?CORPORATE_AUTHOR1,
	                                    ?CORPORATE_AUTHOR2,
	                                    ?DOC_TITLE,
	                                    ?PARALLEL_TITLE,
	                                    ?NOTES,
	                                    ?PHYDESC,
	                                    ?ILLUSTRATE,
	                                    ?STATUS,
	                                    ?PRICE,
	                                    ?DOP,
	                                    ?ISBN_NO,
	                                    ?DISIPLINE,
	                                    ?TAPS,
	                                    ?DETAILS,
	                                    ?VC,
	                                    ?NAME_OF_PUBLISHER,
	                                    ?PLACE_OF_PUBLISHER,
	                                    ?SERIES,
	                                    ?LIB_CATEGORY,
                                        ?DOC_CATEGORY,
	                                    ?BOOK_PUBLISH_YEAR,
	                                    ?SUB_ACCOUNT,
	                                    ?KEYWORD,
	                                    ?INVOICE,
	                                    ?AC_YEAR,
	                                    ?RFID,
	                                    ?BARCODE
                                    );";
                    }
                    break;
                case LibrarySQLCommands.UpdateBookDetails:
                    {
                        sQuery = @"UPDATE LI_BOOK_INFO
                                   SET
	                                    RACK_ID = ?RACK_ID,
	                                    ACCESS_NO = ?ACCESS_NO,
	                                    CLASS_NO = ?CLASS_NO,
	                                    BOOK_NO = ?BOOK_NO,
	                                    LANGUAGE = ?LANGUAGE,
	                                    PERSONAL_AUTHOR1 = ?PERSONAL_AUTHOR1,
	                                    PERSONAL_AUTHOR2 = ?PERSONAL_AUTHOR2,
	                                    JOINED_AUTHOR1 = ?JOINED_AUTHOR1,
	                                    JOINED_AUTHOR2 = ?JOINED_AUTHOR2,
	                                    CORPORATE_AUTHOR1 = ?CORPORATE_AUTHOR1,
	                                    CORPORATE_AUTHOR2 = ?CORPORATE_AUTHOR2,
	                                    DOC_TITLE = ?DOC_TITLE,
	                                    PARALLEL_TITLE = ?PARALLEL_TITLE,
	                                    NOTES = ?NOTES,
	                                    PHYDESC = ?PHYDESC,
	                                    ILLUSTRATE = ?ILLUSTRATE,
	                                    STATUS = ?STATUS,
	                                    PRICE = ?PRICE,
	                                    DOP = ?DOP,
	                                    ISBN_NO = ?ISBN_NO,
	                                    DISIPLINE = ?DISIPLINE,
	                                    TAPS = ?TAPS,
	                                    VC = ?VC,
	                                    NAME_OF_PUBLISHER = ?NAME_OF_PUBLISHER,
	                                    PLACE_OF_PUBLISHER = ?PLACE_OF_PUBLISHER,
	                                    SERIES = ?SERIES,
	                                    LIB_CATEGORY = ?LIB_CATEGORY,
	                                    DOC_CATEGORY = ?DOC_CATEGORY,
	                                    BOOK_PUBLISH_YEAR = ?BOOK_PUBLISH_YEAR,
	                                    SUB_ACCOUNT = ?SUB_ACCOUNT,
	                                    KEYWORD = ?KEYWORD,
	                                    INVOICE = ?INVOICE,
	                                    RFID = ?RFID,
	                                    BARCODE = ?BARCODE
                                    WHERE BOOK_ID = ?BOOK_ID;";
                    }
                    break;
                case LibrarySQLCommands.DeleteBookDetails:
                    {
                        sQuery = @"UPDATE LI_BOOK_DETAILS SET IS_DELETED=1 WHERE BOOK_ID=?BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchLastAccessNo:
                    {
                        sQuery = @"SELECT IF(ACCESS_NO IS NOT NULL,ACCESS_NO+1,0) AS ACCESS_NO FROM AUTO_GENERATE_NUMBERS;";
                        break;
                    }
                case LibrarySQLCommands.UpdateAutoGenerateNumber:
                    {
                        sQuery = @"UPDATE auto_generate_numbers SET Access_No=?ACCESS_NO WHERE auto_generate_numbers_id=1;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsById:
                    {
                        sQuery = @"SELECT 
                                        BOOK_ID,
                                        RACK_ID,
                                        ACCESS_NO,
                                        CLASS_NO,
                                        BOOK_NO,
                                        LANGUAGE,
                                        PERSONAL_AUTHOR1,
                                        PERSONAL_AUTHOR2,
                                        JOINED_AUTHOR1,
                                        JOINED_AUTHOR2,
                                        CORPORATE_AUTHOR1,
                                        CORPORATE_AUTHOR2,
                                        DOC_TITLE,
                                        PARALLEL_TITLE,
                                        NOTES,
                                        PHYDESC,
                                        ILLUSTRATE,
                                        STATUS,
                                        PRICE,
                                        DATE_FORMAT(DOP,'%d/%m/%Y') AS DOP,
                                        ISBN_NO,
                                        DISIPLINE,
                                        TAPS,
                                        DETAILS,
                                        VC,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        SERIES,
                                        LIB_CATEGORY,
                                        DOC_CATEGORY,
                                        BOOK_PUBLISH_YEAR,
                                        SUB_ACCOUNT,
                                        KEYWORD,
                                        INVOICE,
                                        ACADEMIC_YEAR,
                                        LI_BOOK_STATUS,
                                        ENTRY_DATE,
                                        RFID,
                                        BARCODE,
                                        SS.IS_CIRCULATION,
                                        SS.STATUS_NAME
                                    FROM
                                        LI_BOOK_INFO AS BI
                                        
                                        INNER JOIN
                                        
                                        SUP_LI_STATUS AS SS ON SS.STATUS_ID=BI.STATUS
                                        AND SS.IS_DELETED!=1
                                        
                                        WHERE BOOK_ID=?BOOK_ID AND BI.IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookPublisherDetailsByIsbn:
                    {
                        sQuery = @"SELECT 
                                    NAME_OF_PUBLISHER, PLACE_OF_PUBLISHER, BOOK_PUBLISH_YEAR
                                FROM
                                    LI_BOOK_DETAILS
                                WHERE
                                    ISBN_NO LIKE '?ISBN_NO%'
                                ORDER BY BOOK_ID;";
                        break;
                    }

                #endregion
                #region Book Relocate
                case LibrarySQLCommands.SaveBookRelocate:
                    {
                        sQuery = @"UPDATE 
                                    LI_BOOK_DETAILS
                                        SET
                                        STATUS=?STATUS, 
                                        DISCODE=?DISCODE
                                        WHERE 
                                BOOK_ID=?BOOK_ID;";
                        break;
                    }
                #endregion

                #region Rules
                case LibrarySQLCommands.ListLibRules:
                    {
                        sQuery = @"  
                                 SELECT 
                                    LR.RULE_ID,
	                                UT.USER_TYPE_NAME AS USER_TYPE,
                                    IFNULL(SR.ROLE_NAME,'') AS ROLE,
	                                IFNULL(SP.APPLICATION_TYPE,'') AS PROGRAMME_LEVEL,
                                    LR.NO_OF_BOOKS,
                                    LR.NO_OF_DAYS,
                                    LR.NO_OF_RENEW_DAYS,
                                    LR.PER_DAY_DUE_COST,
                                    LR.REVISED_DUE_DATES,
	                                LR.REVISED_DUE_COST

                                FROM
                                    LI_RULES AS LR
                                        INNER JOIN
                                    SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID = LR.USER_TYPE
                                        LEFT JOIN
                                    SUP_ROLE AS SR ON SR.ROLE_ID = LR.ROLE
                                        LEFT JOIN
                                    SUP_APPLICATION_TYPE AS SP ON SP.APPLICATION_TYPE_ID = LR.PROGRAMME_LEVEL
                                        AND SP.IS_DELETED != 1
                                WHERE
                                    LR.IS_DELETED != 1
                                    AND LR.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }

                case LibrarySQLCommands.AddLiRules:
                    {
                        sQuery = @"INSERT INTO li_rules
                                    (
                                    USER_TYPE,
                                    ROLE,
                                    PROGRAMME_LEVEL,
                                    NO_OF_BOOKS,
                                    NO_OF_DAYS,
                                    NO_OF_RENEW_DAYS,
                                    PER_DAY_DUE_COST,
                                    REVISED_DUE_DATES,
                                    REVISED_DUE_COST,
                                    ACADEMIC_YEAR)
                                    VALUES
                                    (
                                    ?USER_TYPE,
                                    ?ROLE,
                                    ?PROGRAMME_LEVEL,
                                    ?NO_OF_BOOKS,
                                    ?NO_OF_DAYS,
                                    ?NO_OF_RENEW_DAYS,
                                    ?PER_DAY_DUE_COST,
                                    ?REVISED_DUE_DATES,
                                    ?REVISED_DUE_COST,
                                    ?AC_YEAR);";
                        break;
                    }

                case LibrarySQLCommands.FetchRules:
                    {
                        sQuery = @"SELECT
                                    RULE_ID,
                                    MEM_CATEGORY,
                                    MAX_BOOKS,
                                    LOAN_PERIOD,
                                    FINE_AMOUNT,
                                    DEPOSIT,
                                    AUTORENEVAL,
                                    EXTRA_BOOKS,
                                    ACADEMIC_YEAR

                                    FROM LI_RULES 
                                    WHERE
                                     ACADEMIC_YEAR=?AC_YEAR
                                    AND IS_DELETED!=1;";
                        break;
                    }

                case LibrarySQLCommands.FetchLiRulebyId:
                    {
                        sQuery = @"SELECT
                                    RULE_ID,
									USER_TYPE,
                                    ROLE,
                                    PROGRAMME_LEVEL,
                                    NO_OF_BOOKS,
                                    NO_OF_DAYS,
									NO_OF_RENEW_DAYS,
                                    PER_DAY_DUE_COST,
                                    REVISED_DUE_DATES,
                                    REVISED_DUE_COST
                                  
                                    FROM LI_RULES 
                                    WHERE RULE_ID=?RULE_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }

                case LibrarySQLCommands.UpdateLiRules:
                    {
                        sQuery = @"UPDATE 
                                    LI_RULES
                                    SET 
                                    USER_TYPE=?USER_TYPE,
                                    ROLE=?ROLE,
                                    PROGRAMME_LEVEL=?PROGRAMME_LEVEL,
                                    NO_OF_BOOKS=?NO_OF_BOOKS,
                                    NO_OF_DAYS=?NO_OF_DAYS,
									NO_OF_RENEW_DAYS=?NO_OF_RENEW_DAYS,
                                    PER_DAY_DUE_COST=?PER_DAY_DUE_COST,
                                    REVISED_DUE_DATES=?REVISED_DUE_DATES,
                                    REVISED_DUE_COST=?REVISED_DUE_COST

                                    WHERE RULE_ID=?RULE_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.DeleteLiRuleById:
                    {
                        sQuery = @"UPDATE LI_RULES SET IS_DELETED=?IS_DELETED WHERE RULE_ID=?RULE_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.FetchMemberByMemberType:
                    {
                        sQuery = @"SELECT 
                                    LR.RULE_ID, LR.MEM_CATEGORY
                                FROM
                                    LI_RULES AS LR
                                        INNER JOIN
                                    LI_USER_DETAILS AS LD ON LD.MEMBER_TYPE = LR.MEM_CATEGORY
                                        AND LD.ACADEMIC_YEAR = LR.ACADEMIC_YEAR
                                        AND LD.IS_DELETED != 1
                                WHERE
                                    LR.RULE_ID =?RULE_ID
                                        AND LR.ACADEMIC_YEAR =?AC_YEAR
                                        AND LR.IS_DELETED != 1
                                GROUP BY LR.RULE_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchRulesByUserType:
                    {
                        sQuery = @"SELECT 
                                        RULE_ID,
                                        USER_TYPE,
                                        ROLE,
                                        PROGRAMME_LEVEL,
                                        NO_OF_BOOKS,
                                        NO_OF_DAYS,
                                        NO_OF_RENEW_DAYS,
                                        PER_DAY_DUE_COST,
                                        REVISED_DUE_DATES,
                                        REVISED_DUE_COST,
                                        ACADEMIC_YEAR
                                    FROM
                                        LI_RULES
                                    WHERE
                                        USER_TYPE = ?USER_TYPE
                                        AND  ROLE=?ROLE
                                        AND  PROGRAMME_LEVEL=?PROGRAMME_LEVEL
                                        AND ACADEMIC_YEAR=?AC_YEAR
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchRuleByUserTypeAndRole:
                    {
                        sQuery = @"SELECT NO_OF_BOOKS,
                                        NO_OF_DAYS,
                                        NO_OF_RENEW_DAYS,
                                        PER_DAY_DUE_COST,
                                        REVISED_DUE_DATES,
                                        REVISED_DUE_COST
                                        FROM LI_RULES WHERE USER_TYPE=?USER_TYPE
                                        AND ROLE=?ROLE 
                                        AND PROGRAMME_LEVEL=?PROGRAMME_LEVEL 
                                        AND IS_DELETED!=1 
                                        AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case LibrarySQLCommands.FetchRuleByMembeForStudent:
                    {
                        sQuery = @"SELECT 
                                    RULE_ID
                                FROM
                                    LI_RULES AS LR 
                                    INNER JOIN
                                    SUP_LI_MEMBER_CATEGORY AS SM ON SM.CATEGORY_ID=LR.MEM_CATEGORY
                                    AND SM.IS_DELETED!=1
                                    
                                    INNER JOIN
                                    STU_PERSONAL_INFO AS STU ON STU.STUDENT_ID=?STUDENT_ID
                                    AND STU.IS_DELETED!=1
                                    AND STU.IS_LEFT!=1
                                    
                                    INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID=STU.STUDENT_ID
                                    AND SC.ACADEMIC_YEAR=?AC_YEAR
                                    AND SC.IS_DELETED!=1
                                    
                                    INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID=SC.PROGRAMME_GROUP_ID
                                    AND CPP.IS_DELETED!=1
                                    
                                WHERE
                                    LR.MEM_CATEGORY=?MEM_CATEGORY
                                    AND LR.APPTYPE_ID=CPP.APPTYPE_ID
                                        AND LR.IS_DELETED != 1; ";
                        break;
                    }
                case LibrarySQLCommands.IsRulesExistForUser:
                    {
                        sQuery = @"SELECT 
                                        RULE_ID,
                                        USER_TYPE,
                                        ROLE,
                                        PROGRAMME_LEVEL,
                                        NO_OF_BOOKS,
                                        NO_OF_DAYS,
                                        NO_OF_RENEW_DAYS,
                                        PER_DAY_DUE_COST,
                                        REVISED_DUE_DATES,
                                        REVISED_DUE_COST,
                                        ACADEMIC_YEAR
                                    FROM
                                        LI_RULES
                                    WHERE
                                        USER_TYPE = ?USER_TYPE
                                        AND ACADEMIC_YEAR=?AC_YEAR
                                        AND IS_DELETED != 1;";
                        break;
                    }
                

                


                #endregion
                #region Periods
                case LibrarySQLCommands.ListLibPeriods:
                    {
                        sQuery = @"  
                                 SELECT 
                                    LR.PERIOD_ID,
	                                UT.USER_TYPE_NAME AS USER_TYPE,
                                    IFNULL(SR.ROLE_NAME,'') AS ROLE,
	                                IFNULL(SP.APPLICATION_TYPE,'') AS PROGRAMME_LEVEL,
                                    DATE_FORMAT(SEM_BEGIN_DATE,'%Y/%m/%d') AS SEM_BEGIN_DATE,
									DATE_FORMAT(SEM_END_DATE,'%Y/%m/%d') AS SEM_END_DATE,
									ST.SEMESTER_TYPE
                                   

                                FROM
                                    LI_PERIODS AS LR
                                        INNER JOIN
                                    SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID = LR.USER_TYPE
                                        LEFT JOIN
                                    SUP_ROLE AS SR ON SR.ROLE_ID = LR.ROLE
                                        LEFT JOIN
                                    SUP_APPLICATION_TYPE AS SP ON SP.APPLICATION_TYPE_ID = LR.PROGRAMME_LEVEL
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                        SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=LR.SEMESTER_TYPE
                                        AND ST.IS_DELETED!=1
                                        
                                WHERE
                                    LR.IS_DELETED != 1
                                    AND LR.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case LibrarySQLCommands.AddLiPeriods:
                    {
                        sQuery = @"INSERT INTO li_periods
                                    (
                                    USER_TYPE,
                                    ROLE,
                                    PROGRAMME_LEVEL,
                                    SEM_BEGIN_DATE,
                                    SEM_END_DATE,
                                    SEMESTER_TYPE,
                                    ACADEMIC_YEAR)
                                    VALUES
                                    (
                                    ?USER_TYPE,
                                    ?ROLE,
                                    ?PROGRAMME_LEVEL,
                                    ?SEM_BEGIN_DATE,
                                    ?SEM_END_DATE,
                                    ?SEMESTER_TYPE,
                                    ?AC_YEAR);";
                        break;
                    }
                case LibrarySQLCommands.UpdateLiPeriods:
                    {
                        sQuery = @"UPDATE li_periods 
                                    SET 
		                                    USER_TYPE=?USER_TYPE,
		                                    ROLE=?ROLE,
		                                    PROGRAMME_LEVEL=?PROGRAMME_LEVEL,
                                            SEM_BEGIN_DATE=?SEM_BEGIN_DATE,
                                            SEM_END_DATE=?SEM_END_DATE,
                                            SEMESTER_TYPE=?SEMESTER_TYPE,
                                            ACADEMIC_YEAR=?AC_YEAR 
                                    WHERE PERIOD_ID=?PERIOD_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchLiPeriodbyId:
                    {
                        sQuery = @"
                                    SELECT
                                    PERIOD_ID,
									USER_TYPE,
                                    ROLE,
                                    PROGRAMME_LEVEL,
                                    DATE_FORMAT(SEM_BEGIN_DATE,'%d/%m/%Y') AS SEM_BEGIN_DATE,
                                    DATE_FORMAT(SEM_END_DATE,'%d/%m/%Y') AS SEM_END_DATE,
									SEMESTER_TYPE
                                    
                                  
                                    FROM LI_PERIODS 
                                    WHERE PERIOD_ID=?PERIOD_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.DeleteLiPeriodById:
                    {
                        sQuery = @"UPDATE LI_PERIODS SET IS_DELETED=?IS_DELETED WHERE PERIOD_ID=?PERIOD_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.FetchPeriodByUserType:
                    {
                        sQuery = @"SELECT 
                                        PERIOD_ID,
                                        USER_TYPE,
                                        ROLE,
                                        PROGRAMME_LEVEL,
                                        DATE_FORMAT(SEM_BEGIN_DATE, '%Y-%m-%d') AS SEM_BEGIN_DATE,
                                         DATE_FORMAT(SEM_END_DATE, '%Y-%m-%d') AS SEM_END_DATE,
                                        SEMESTER_TYPE,
                                        ACADEMIC_YEAR
                                    FROM
                                        LI_PERIODS
                                    WHERE
                                        USER_TYPE=?USER_TYPE
                                        AND  ROLE=?ROLE
                                        AND PROGRAMME_LEVEL=?PROGRAMME_LEVEL
                                        AND ACADEMIC_YEAR=?AC_YEAR
                                        AND SEMESTER_TYPE=(SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_DELETED!=1 AND IS_ACTIVE=1 AND SEMESTER_TYPE_ID!=3)
                                        AND IS_DELETED != 1;";
                        break;
                    }

                case LibrarySQLCommands.ListLibRackInfo:
                    {
                        sQuery = @"SELECT 
                                    RACK_INFO_ID,
                                    RACK_INFO,
                                    SR.RACK_NAME AS RACK,
                                    SC.CELL_NAME AS CELL
                                FROM
                                    LI_RACK_INFO AS LRI
                                        INNER JOIN
                                    SUP_LI_RACKS AS SR ON SR.RACK_ID = LRI.RACK_ID
                                        AND SR.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_LI_CELLS AS SC ON SC.CELL_ID = LRI.CELL_ID
                                        AND SC.IS_DELETED != 1
                                WHERE
                                    LRI.IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.AddLiRackInfo:
                    {
                        sQuery = @"INSERT INTO li_rack_info
                                (RACK_INFO,
                                RACK_ID,
                                CELL_ID)
                                VALUES
                                (?RACK_INFO,
                                ?RACK_ID,
                                ?CELL_ID);";
                        break;
                    }
                case LibrarySQLCommands.UpdateLiRackInfo:
                    {
                        sQuery = @"UPDATE LI_RACK_INFO 
	                                SET 
                                    RACK_INFO=?RACK_INFO,
                                    RACK_ID=?RACK_ID,
                                    CELL_ID=?CELL_ID 
                                WHERE 
                                RACK_INFO_ID=?RACK_INFO_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchLiRackInfobyId:
                    {
                        sQuery = @"SELECT 
                                    RACK_INFO_ID,
                                    RACK_INFO,
                                    RACK_ID,
                                    CELL_ID
                                    FROM LI_RACK_INFO 
                                    WHERE 
                                    RACK_INFO_ID=?RACK_INFO_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchRackInfoByRackAndCell:
                    {
                        sQuery = @"
                                    SELECT 
                                    RACK_INFO_ID,
                                    RACK_ID,
                                    CELL_ID
                                    FROM LI_RACK_INFO 
                                    WHERE 
                                    RACK_ID=?RACK_ID
                                    AND CELL_ID=?CELL_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.DeleteLiRackInfoById:
                    {
                        sQuery = @"UPDATE LI_RACK_INFO SET IS_DELETED=?IS_DELETED WHERE RACK_INFO_ID=?RACK_INFO_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }

                #endregion

                #region Member

                case LibrarySQLCommands.FetchLiMemberDetails:
                    {
                        sQuery = @"SELECT 
                                         *
                                    FROM
                                        (SELECT 
                                            (@rm:=@rm + 1) AS RUN_NUMBER,
                                                LD.USER_ID,
                                                LD.USERNAME,
                                                LD.PASSWORD,
                                                'EXTERNAL' AS USER_TYPE,
                                                LD.BOID,
                                                CONCAT(IFNULL(LD.FIRST_NAME, ''), ' ', IFNULL(LD.LAST_NAME, '')) AS FIRST_NAME,
                                                LD.EMAIL,
                                                LD.MOBILE,
                                                DATE_FORMAT(LD.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                                LD.DEGREE,
                                                LD.ADDRESS1,
                                                LD.BLOOD_GROUP_ID,
                                                LD.REGISTERED_DATE,
                                                LD.ACADEMIC_YEAR,
                                                LD.PHOTO,
                                                LD.RFID
                                        FROM
                                            LI_EXTERNAL_USER_INFO AS LD, (SELECT @rm:=0) AS T
                                        WHERE
                                            LD.IS_DELETED != 1) AS outT
                                    WHERE
                                        outT.RUN_NUMBER > {0}
                                        {1}
                                    LIMIT {2};";
                        break;
                    }
                case LibrarySQLCommands.FetchLiMemberDetailsTotalCount:
                    {
                        sQuery = @"SELECT 
                                         COUNT(USER_ID) AS COUNT_FIELD
                                FROM
                                    LI_EXTERNAL_USER_INFO
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchLiMemberDetailsById:
                    {
                        sQuery = @"SELECT 
                                        LD.USER_ID,
                                        LD.USERNAME,
                                        LD.PASSWORD,
                                        'EXTERNAL' AS USER_TYPE,
                                        LD.BOID,
                                        LD.FIRST_NAME,
                                        LD.LAST_NAME,
                                        LD.EMAIL,
                                        LD.MOBILE,
                                        DATE_FORMAT(LD.DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                        LD.DEGREE,
                                        LD.ADDRESS1,
                                        LD.ADDRESS2,
                                        LD.BLOOD_GROUP_ID,
										DATE_FORMAT(LD.REGISTERED_DATE,'%d/%m/%Y') AS REGISTERED_DATE,
                                        LD.ACADEMIC_YEAR,
                                        LD.PHOTO,
                                        LD.RFID,
                                        LD.STATE,
                                        LD.PINCODE,
                                        LD.CITY,
                                        LD.BAR_CODE
                                    FROM
                                        LI_EXTERNAL_USER_INFO AS LD
                                    WHERE
                                        LD.IS_DELETED != 1 AND USER_ID=?USER_ID;";
                        break;
                    }



                case LibrarySQLCommands.AddLiMember:
                    {
                        sQuery = @"INSERT INTO  li_user_details
                                   (
                                        USERNAME,
                                        PASSWORD,
                                        ROLE,
                                        BOID,
                                        FIRST_NAME,
                                        LAST_NAME,
                                        EMAIL,
                                        MOBILE,
                                        LAST_LOGIN,
                                        DATE_OF_BIRTH,
                                        MEMBER_TYPE,
                                        ADDRESS1,
                                         ADDRESS2, 
                                         CITY,
                                         STATE,
                                         PINCODE,
                                        BLOOD_GROUP_ID,
                                        REGISTERED_DATE,
                                        EXPIRED_DATE,
                                        ACADEMIC_YEAR,
                                        PHOTO,
                                        RFID
                                    )
                                    VALUES
                                    (
                                        ?USERNAME,
                                        ?PASSWORD,
                                        ?ROLE,
                                        ?BOID,
                                        ?FIRST_NAME,
                                        ?LAST_NAME,
                                        ?EMAIL,
                                        ?MOBILE,
                                        ?LAST_LOGIN,
                                        ?DATE_OF_BIRTH,
                                        ?MEMBER_TYPE,
                                        ?ADDRESS1,
                                        ?ADDRESS2,
                                        ?CITY,
                                        ?STATE,
                                        ?PINCODE,
                                        ?BLOOD_GROUP_ID,
                                        NOW(),
                                        ?EXPIRED_DATE,
                                        ?AC_YEAR,
                                        ?PHOTO,
                                        ?RFID
                                    );";
                        break;
                    }
                case LibrarySQLCommands.UpdateLiMember:
                    {
                        sQuery = @"UPDATE LI_USER_DETAILS
                                    SET 
                                        USERNAME=?USERNAME,
                                        PASSWORD=?PASSWORD,
                                        ROLE=?ROLE,
                                        BOID=?BOID,
                                        FIRST_NAME=?FIRST_NAME,
                                        LAST_NAME=?LAST_NAME,
                                        EMAIL=?EMAIL,
                                        MOBILE=?MOBILE,
                                        LAST_LOGIN=?LAST_LOGIN,
                                        DATE_OF_BIRTH=?DATE_OF_BIRTH,
                                        MEMBER_TYPE=?MEMBER_TYPE,
                                        ADDRESS1=?ADDRESS1,
                                         ADDRESS2=?ADDRESS2,
                                         CITY=?CITY,
                                        STATE=?STATE,
                                        PINCODE=?PINCODE,
                                        BLOOD_GROUP_ID=?BLOOD_GROUP_ID,
                                        EXPIRED_DATE=?EXPIRED_DATE,
                                        PHOTO=?PHOTO,
                                        RFID=?RFID
                                  WHERE USER_ID=?USER_ID
                                  AND IS_DELETED!=1;";
                        break;
                    }


                case LibrarySQLCommands.DeleteMemberById:
                    {
                        sQuery = @"UPDATE LI_USER_DETAILS SET IS_DELETED=?IS_DELETED
                                        WHERE USER_ID=?USER_ID
                                        AND IS_DELETED!=1;";
                        break;
                    }

                case LibrarySQLCommands.FetchUserDetailsByRollno:
                    {
                        sQuery = @"SELECT 
                                        SPI.STUDENT_ID AS USER_ID,
                                        DATE_FORMAT(SPI.DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                        SPI.PHOTO,
                                        SPI.FIRST_NAME,
                                        SPI.LAST_NAME,
                                        SPI.STU_MOBILENO AS MOBILENO,
                                        SPI.BLOOD_GROUP,
                                        SPI.STU_EMAILID AS EMAILID,
                                        SPI.BOID_NUMBER AS BOID,
                                        SPI.RFID,
                                        CONCAT(IFNULL(SA.P_DOOR_DETAIL,SA.C_DOOR_DETAIL),' ',
                                        IFNULL(SA.P_STREET,SA.C_STREET)) AS ADDRESS1,
                                        CONCAT(IFNULL(SA.P_VILLAGE_AREA,SA.C_VILLAGE_AREA),' ',
                                        IFNULL(SA.P_POST_PLACE_TOWN,SA.C_POST_PLACE_TOWN)) AS ADDRESS2,
                                        IFNULL(SA.P_CITY,SA.C_CITY) AS CITY,
                                        IFNULL(SA.P_PINCODE,SA.C_PINCODE) AS PINCODE
                                       
                                    FROM
                                        STU_PERSONAL_INFO AS SPI
                                        
                                        LEFT JOIN
                                        STU_ADDRESS AS SA ON SA.STUDENT_ID=SPI.STUDENT_ID
                                        AND SA.IS_DELETED!=1
                                        
                                    WHERE
                                        SPI.ROLL_NO =?ROLL_NO
                                            AND SPI.IS_LEFT != 1
                                            AND SPI.IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchStudentDetailsByBoid:
                    {
                        sQuery = @"SELECT 
                                        SPI.STUDENT_ID AS USER_ID,
                                        DATE_FORMAT(SPI.DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                        SPI.PHOTO,
                                        SPI.FIRST_NAME,
                                        SPI.LAST_NAME,
                                        SPI.STU_MOBILENO AS MOBILENO,
                                        SPI.BLOOD_GROUP,
                                        SPI.STU_EMAILID AS EMAILID,
                                        SPI.BOID_NUMBER,
                                        SPI.RFID,
                                         IF(IS_LEFT!=1,0,1) AS STATUS,
									IF(IS_DELETED!=1,0,1) AS STATUS1
                                    FROM
                                        STU_PERSONAL_INFO AS SPI
                                    WHERE
                                        SPI.BOID_NUMBER =?BOID_NUMBER;";
                        break;
                    }
                case LibrarySQLCommands.FetchUserDetailsByStaffCode:
                    {
                        sQuery = @"
                               
 SELECT 
                                    STF.STAFF_ID AS USER_ID,
                                    STF.FIRST_NAME,
                                    STF.LAST_NAME,
                                   DATE_FORMAT(STF.DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                    STF.BLOOD_GROUP,
                                    STF.PHOTO,
                                    STF.BOID_NUMBER AS BOID,
                                    STF.RFID,
                                    CONCAT(IFNULL(SA.PDOOR_DETAILS,SA.CDOOR_DETAILS),' ',
                                    IFNULL(SA.PSTREET,SA.CSTREET),' ',
                                    IFNULL(SA.PPLACE,SA.CPLACE)) AS ADDRESS1,
                                    CONCAT(IFNULL(SA.PVILLAGE,SA.CVILLAGE),' ',
                                    IFNULL(SA.PDISTRICT,SA.CDISTRICT)) AS ADDRESS2,
                                    IFNULL(SA.PCITY,SA.CCITY) AS CITY,
                                    IFNULL(SA.PPIN_CODE,SA.CPIN_CODE) AS PINCODE,
                                    IFNULL(SA.PPHONE_NO,SA.CPHONE_NO) AS MOBILE,
                                    IFNULL(SA.PEMAIL,SA.CEMAIL) AS EMAIL
                                    
                                FROM
                                    STF_PERSONAL_INFO AS STF
                                    
                                    LEFT JOIN
                                    
                                    STF_ADDRESS AS SA ON SA.STAFFNO=STF.STAFF_ID
                                    AND SA.ISDELETED!=1
                                    
                                WHERE
                                    STF.STAFF_CODE =?STAFF_CODE
                                        AND STF.IS_LEFT != 1
                                        AND STF.IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchStaffDetailsByBoid:
                    {
                        sQuery = @"
                                    SELECT 
                                    STAFF_ID AS USER_ID,
                                    FIRST_NAME,
                                    LAST_NAME,
                                   DATE_FORMAT(DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                    BLOOD_GROUP,
                                    PHOTO,
                                    BOID_NUMBER,
                                    RFID,
                                    IF(IS_LEFT!=1,0,1) AS STATUS,
									IF(IS_DELETED!=1,0,1) AS STATUS1
                                FROM
                                    STF_PERSONAL_INFO
                                WHERE
                                    BOID_NUMBER =?BOID_NUMBER;";
                        break;
                    }
                case LibrarySQLCommands.FetchLiMemberDetailsByBoid:
                    {
                        sQuery = @"SELECT 
                                        USER_ID,
                                        USERNAME,
                                        PASSWORD,
                                        USER_TYPE,
                                        BOID,
                                        FIRST_NAME,
                                        LAST_NAME,
                                        EMAIL,
                                        MOBILE,
                                        DATE_FORMAT(DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                        DEGREE,
                                        ADDRESS1,
                                        ADDRESS2,
                                        BLOOD_GROUP_ID,
                                        DATE_FORMAT(REGISTERED_DATE,'%d/%m/%Y') AS REGISTERED_DATE,
                                        ACADEMIC_YEAR,
                                        PHOTO,
                                        RFID,
                                        STATE,
                                        PINCODE,
                                        CITY,
                                        BAR_CODE
                                    FROM
                                        LI_EXTERNAL_USER_INFO
                                    WHERE
                                        IS_DELETED != 1 AND BOID=?BOID;";
                        break;
                    }
                case LibrarySQLCommands.FetchLiMemberDetailsByBarCode:
                    {
                        sQuery = @"SELECT 
                                        USER_ID,
                                        USERNAME,
                                        PASSWORD,
                                        USER_TYPE,
                                        BOID,
                                        FIRST_NAME,
                                        LAST_NAME,
                                        EMAIL,
                                        MOBILE,
                                        DATE_FORMAT(DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                        DEGREE,
                                        ADDRESS1,
                                        ADDRESS2,
                                        BLOOD_GROUP_ID,
                                        DATE_FORMAT(REGISTERED_DATE,'%d/%m/%Y') AS REGISTERED_DATE,
                                        ACADEMIC_YEAR,
                                        PHOTO,
                                        RFID,
                                        STATE,
                                        PINCODE,
                                        CITY,
                                        BAR_CODE
                                    FROM
                                        LI_EXTERNAL_USER_INFO
                                    WHERE
                                        IS_DELETED != 1 AND BAR_CODE=?BAR_CODE;";
                        break;
                    }
                case LibrarySQLCommands.FetchLiMemberDetailsByRfid:
                    {
                        sQuery = @"SELECT 
                                        USER_ID,
                                        USERNAME,
                                        PASSWORD,
                                        USER_TYPE,
                                        BOID,
                                        FIRST_NAME,
                                        LAST_NAME,
                                        EMAIL,
                                        MOBILE,
                                        DATE_FORMAT(DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                        DEGREE,
                                        ADDRESS1,
                                        ADDRESS2,
                                        BLOOD_GROUP_ID,
                                        DATE_FORMAT(REGISTERED_DATE,'%d/%m/%Y') AS REGISTERED_DATE,
                                        ACADEMIC_YEAR,
                                        PHOTO,
                                        RFID,
                                        STATE,
                                        PINCODE,
                                        CITY,
                                        BAR_CODE
                                    FROM
                                        LI_EXTERNAL_USER_INFO
                                    WHERE
                                        IS_DELETED != 1 AND RFID=?RFID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateBoidandRfidByStudentId:
                    {
                        sQuery = @"UPDATE STU_PERSONAL_INFO 
                                    SET 
                                        DATE_OF_BIRTH=?DATE_OF_BIRTH,
                                        BLOOD_GROUP=?BLOOD_GROUP,
                                        PHOTO=?PHOTO,
                                        FIRST_NAME=?FIRST_NAME,
                                        LAST_NAME=?LAST_NAME,
                                        STU_MOBILENO=?STU_MOBILENO,
                                        STU_EMAILID=?STU_EMAILID,
                                         BOID_NUMBER =?BOID_NUMBER,
                                        RFID =?RFID
                                    WHERE
                                        STUDENT_ID =?STUDENT_ID
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.UpdateBoidandRfidByStffId:
                    {
                        sQuery = @"UPDATE STF_PERSONAL_INFO  
                                    SET 
                                        FIRST_NAME=?FIRST_NAME,
                                        LAST_NAME=?LAST_NAME,
                                        DATE_OF_BIRTH=?DATE_OF_BIRTH,
                                        BLOOD_GROUP=?BLOOD_GROUP,
                                        PHOTO=?PHOTO,
                                        BOID_NUMBER =?BOID_NUMBER,
                                        RFID =?RFID
                                    WHERE
                                        STAFF_ID =?STAFF_ID
                                            AND IS_DELETED != 1;";
                        break;
                    }

                case LibrarySQLCommands.FetchUserCredentialByBARCODE:
                    {
                        sQuery = @"SELECT 
                                        UA.USER_ID, 
                                        UA.ROLE, 
                                        UA.USER_TYPE, 
										UT.USER_TYPE_NAME,
                                        UA.NAME AS FIRST_NAME,
                                           date_format(SP.DATE_OF_BIRTH,'%d-%m-%Y') AS DATE_OF_BIRTH,
                                        BG.BLOOD_GROUP_NAME AS BLOOD_GROUP,
                                        SP.PHOTO,
                                        PG.APPTYPE_ID AS PROGRAMME_LEVEL,
                                        (SELECT AC_YEAR FROM ACADEMIC_YEAR WHERE ACTIVE_YEAR=1) AS AC_YEAR,
                                        '0' AS SEMESTER,
                                        (SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_ACTIVE=1  AND SEMESTER_TYPE_ID!=3) AS SEMESTER_TYPE
                                    FROM
                                        USER_ACCOUNT AS UA
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = UA.USER_ID
                                            AND SP.ROLL_NO = UA.USERNAME
                                            AND SP.BAR_CODE = ?SCAN_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
	                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=SP.PROGRAM_ID
                                            
                                            INNER JOIN 
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=UA.USER_TYPE
                                           
                                        
                                            LEFT JOIN 
                                            SUP_BLOOD_GROUP AS BG ON BG.BLOOD_GROUP_ID=SP.BLOOD_GROUP
                                            AND BG.IS_DELETED!=1
                                    WHERE
                                        UA.IS_DELETED != 1 
                                    UNION 
                                    SELECT 
                                        UA.USER_ID, 
                                        UA.ROLE, 
                                        UA.USER_TYPE, 
										UT.USER_TYPE_NAME,
                                        date_format(STF.DATE_OF_BIRTH,'%d-%m-%Y') AS DATE_OF_BIRTH,
										BG.BLOOD_GROUP_NAME AS BLOOD_GROUP,
                                        STF.PHOTO,
                                        UA.NAME AS FIRST_NAME,
                                        '0' AS PROGRAMME_LEVEL ,
                                        (SELECT AC_YEAR FROM ACADEMIC_YEAR WHERE ACTIVE_YEAR=1) AS AC_YEAR,
                                        '0' AS SEMESTER,
                                        (SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_ACTIVE=1  AND SEMESTER_TYPE_ID!=3) AS SEMESTER_TYPE
                                    FROM
                                        USER_ACCOUNT AS UA
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = UA.USER_ID
                                            AND STF.STAFF_CODE = UA.USERNAME
                                            AND STF.BAR_CODE = ?SCAN_ID
                                            AND STF.IS_DELETED != 1
                                            
                                             INNER JOIN 
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=UA.USER_TYPE
                                           
                                             LEFT JOIN 
                                            SUP_BLOOD_GROUP AS BG ON BG.BLOOD_GROUP_ID=STF.BLOOD_GROUP
                                            AND BG.IS_DELETED!=1
                                    WHERE
                                        UA.IS_DELETED != 1 
                                    UNION 
                                    SELECT 
                                        USER_ID,
                                        '14' AS ROLE,
                                        USER_TYPE,
										UT.USER_TYPE_NAME,
                                        BG.BLOOD_GROUP_NAME AS BLOOD_GROUP,
                                         date_format(UI.DATE_OF_BIRTH,'%d-%m-%Y') AS DATE_OF_BIRTH,
                                         UI.PHOTO,
                                        CONCAT(IFNULL(UI.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(UI.LAST_NAME, '')) AS FIRST_NAME,
                                                '0' AS PROGRAMME_LEVEL,
	                                    (SELECT AC_YEAR FROM ACADEMIC_YEAR WHERE ACTIVE_YEAR=1) AS AC_YEAR,
                                        '0' AS SEMESTER,
	                                    (SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_ACTIVE=1  AND SEMESTER_TYPE_ID!=3) AS SEMESTER_TYPE
                                    FROM
                                        LI_EXTERNAL_USER_INFO AS UI
                                         INNER JOIN 
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=UI.USER_TYPE
                                           
                                        LEFT JOIN 
                                            SUP_BLOOD_GROUP AS BG ON BG.BLOOD_GROUP_ID=UI.BLOOD_GROUP_ID
                                            AND BG.IS_DELETED!=1
                                    WHERE
                                        UI.IS_DELETED != 1 AND UI.BAR_CODE=?SCAN_ID;";
                        break;

                    }
                case LibrarySQLCommands.FetchUserCredentialByRFID:
                    {
                        sQuery = @"SELECT 
                                        UA.USER_ID, 
                                        UA.ROLE, 
                                        UA.USER_TYPE, 
                                        UT.USER_TYPE_NAME,
                                        date_format(SP.DATE_OF_BIRTH,'%d-%m-%Y') AS DATE_OF_BIRTH,
                                        BG.BLOOD_GROUP_NAME AS BLOOD_GROUP,
                                        SP.PHOTO,
                                        UA.NAME AS FIRST_NAME,
                                        (SELECT AC_YEAR FROM ACADEMIC_YEAR WHERE ACTIVE_YEAR=1) AS AC_YEAR,
                                        PG.APPTYPE_ID AS PROGRAMME_LEVEL,
                                        '0' AS SEMESTER,
                                        (SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_ACTIVE=1  AND SEMESTER_TYPE_ID!=3) AS SEMESTER_TYPE
                                    FROM
                                        USER_ACCOUNT AS UA
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = UA.USER_ID
                                            AND SP.ROLL_NO = UA.USERNAME
                                            AND SP.RFID = ?SCAN_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
	                                     CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=SP.PROGRAM_ID
                                          INNER JOIN 
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=UA.USER_TYPE
                                            
                                              LEFT JOIN 
                                            SUP_BLOOD_GROUP AS BG ON BG.BLOOD_GROUP_ID=SP.BLOOD_GROUP
                                            AND BG.IS_DELETED!=1
                                    WHERE
                                        UA.IS_DELETED != 1 
                                    UNION 
                                    SELECT 
                                        UA.USER_ID, 
                                        UA.ROLE, 
                                        UA.USER_TYPE, 
										UT.USER_TYPE_NAME,
                                        date_format(STF.DATE_OF_BIRTH,'%d-%m-%Y') AS DATE_OF_BIRTH,
                                        BG.BLOOD_GROUP_NAME AS BLOOD_GROUP,
                                        STF.PHOTO,
                                        UA.NAME AS FIRST_NAME,
                                        (SELECT AC_YEAR FROM ACADEMIC_YEAR WHERE ACTIVE_YEAR=1) AS AC_YEAR,
                                        '0' AS PROGRAMME_LEVEL,
                                        '0' AS SEMESTER,
                                        (SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_ACTIVE=1  AND SEMESTER_TYPE_ID!=3) AS SEMESTER_TYPE
                                    FROM
                                        USER_ACCOUNT AS UA
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = UA.USER_ID
                                            AND STF.STAFF_CODE = UA.USERNAME
                                            AND STF.RFID =  ?SCAN_ID
                                            AND STF.IS_DELETED != 1
                                              INNER JOIN 
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=UA.USER_TYPE
                                            
                                             LEFT JOIN 
                                            SUP_BLOOD_GROUP AS BG ON BG.BLOOD_GROUP_ID=STF.BLOOD_GROUP
                                            AND BG.IS_DELETED!=1
                                    WHERE
                                        UA.IS_DELETED != 1 
                                    UNION 
                                    SELECT 
                                        USER_ID,
                                        '14' AS ROLE,
                                        USER_TYPE,
										UT.USER_TYPE_NAME,
                                        date_format(UI.DATE_OF_BIRTH,'%d-%m-%Y') AS DATE_OF_BIRTH,
                                        BG.BLOOD_GROUP_NAME AS BLOOD_GROUP,
                                        UI.PHOTO,
                                        CONCAT(IFNULL(UI.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(UI.LAST_NAME, '')) AS FIRST_NAME,
	                                    (SELECT AC_YEAR FROM ACADEMIC_YEAR WHERE ACTIVE_YEAR=1) AS AC_YEAR,
                                        '0' AS PROGRAMME_LEVEL,
                                        '0' AS SEMESTER,
                                        (SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_ACTIVE=1  AND SEMESTER_TYPE_ID!=3) AS SEMESTER_TYPE
                                    FROM
                                        LI_EXTERNAL_USER_INFO AS UI
                                         INNER JOIN 
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=UI.USER_TYPE
                                            
                                         LEFT JOIN 
                                            SUP_BLOOD_GROUP AS BG ON BG.BLOOD_GROUP_ID=UI.BLOOD_GROUP_ID
                                            AND BG.IS_DELETED!=1
                                    WHERE
                                        UI.IS_DELETED != 1 AND UI.RFID= ?SCAN_ID;";
                        break;
                    }
                #endregion

                #region Login
                case LibrarySQLCommands.FetchUserDetailsByBoidId:
                    {
                        sQuery = @"
                            SELECT
	                                    U.USER_ID,
                                        BOID,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(LAST_NAME, '')) AS FIRST_NAME,
                                        DATE_FORMAT(DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        G.BLOOD_GROUP_NAME,
                                        U.PHOTO,
                                        R.MAX_BOOKS,
                                        SD.DEGREE
                                    FROM
                                        LI_USER_DETAILS AS U
                                            INNER JOIN
                                        SUP_BLOOD_GROUP AS G ON G.BLOOD_GROUP_ID = U.BLOOD_GROUP_ID
                                            INNER JOIN
	                                    LI_RULES AS R ON R.MEM_CATEGORY=U.MEMBER_TYPE
                                          INNER JOIN
                                            SUP_DEGREE AS SD ON SD.DEGREE_ID=U.DEGREE
                                            AND U.IS_DELETED!=1
                                    WHERE
                                        U.IS_DELETED != 1 AND U.BOID =?BOID;";
                        break;
                    }
                case LibrarySQLCommands.FetchUserDetailsByRfid:
                    {

                        sQuery = @"SELECT
	                                    U.USER_ID,
                                        RFID,
                                        BOID,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(LAST_NAME, '')) AS FIRST_NAME,
                                        DATE_FORMAT(DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        R.MAX_BOOKS,
                                        G.BLOOD_GROUP_NAME,
                                        U.PHOTO
                                        
                                    FROM
                                        LI_USER_DETAILS AS U
                                            INNER JOIN
                                        SUP_BLOOD_GROUP AS G ON G.BLOOD_GROUP_ID = U.BLOOD_GROUP_ID
                                            INNER JOIN
	                                    LI_RULES AS R ON R.MEM_CATEGORY=U.MEMBER_TYPE
                                            
                                             
                                          
                                    WHERE
                                        U.IS_DELETED != 1 AND U.RFID=?RFID;";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalVisitorsByDate:
                    {
                        sQuery = @"SELECT 
                                    SUM(IF(USER_ID != 0, 1, 0)) AS VISITORS
                                FROM
                                    LI_LOGIN_DETAILS AS LD
                                WHERE
                                    DATE_FORMAT(LD.LOGIN_DATE, '%Y-%m-%d') =DATE_FORMAT(now(), '%Y-%m-%d')
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchUserDetailsById:
                    {
                        sQuery = @"SELECT
	                                    U.USER_ID,
                                        BOID,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(LAST_NAME, '')) AS FIRST_NAME,
                                        DATE_FORMAT(DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        R.MAX_BOOKS,
                                        U.PHOTO
                                    FROM
                                        LI_USER_DETAILS AS U
                                             INNER JOIN
	                                    LI_RULES AS R ON R.MEM_CATEGORY=U.MEMBER_TYPE
                                    WHERE
                                        U.IS_DELETED != 1 AND U.RFID=?RFID;";
                        break;
                    }
                case LibrarySQLCommands.AddLoginDetails:
                    {
                        sQuery = @"INSERT INTO LI_LOGIN_DETAILS
                                    (USER_ID,
                                    LOGIN_DATE,
                                    EXIT_DATE,
                                    SPENT_TIME,
                                    NO_OF_ATTEMPT,
                                    ACADEMIC_YEAR,
                                    IS_ACTIVE)
                                    VALUES
                                    (?USER_ID,
                                    NOW(),
                                    ?EXIT_DATE,
                                    ?SPENT_TIME,
                                    ?NO_OF_ATTEMPT,
                                    ?AC_YEAR,
                                    ?IS_ACTIVE);";
                        break;
                    }
                case LibrarySQLCommands.CheckUserLoggedAlready:
                    {
                        sQuery = @"SELECT 
                                    USER_ID,
                                    LOGIN_DATE,
                                    EXIT_DATE,
                                    SPENT_TIME,
                                    NO_OF_ATTEMPT,
                                    ACADEMIC_YEAR,
                                    IS_ACTIVE
                                FROM
                                    LI_LOGIN_DETAILS AS LD
                                WHERE
                                    LD.USER_ID =?USER_ID
                                        AND DATE_FORMAT(LD.LOGIN_DATE, '%Y-%m-%d') = DATE_FORMAT(NOW(), '%Y-%m-%d')
                                        AND LD.IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.UpdateLoginDetails:
                    {
                        sQuery = @"UPDATE LI_LOGIN_DETAILS 
                                    SET 
                                        LOGIN_DATE = NOW(),
                                        NO_OF_ATTEMPT = NO_OF_ATTEMPT + 1,
                                        ACADEMIC_YEAR =?AC_YEAR,
                                         IS_ACTIVE = ?IS_ACTIVE
                                    WHERE
                                        LOGIN_ID = ?LOGIN_ID
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchUserLoginDetails:
                    {
                        sQuery = @"SELECT 
	                                    NAME as FIRST_NAME,
                                        DATE_FORMAT(NOW(),'%d/%m/%Y') as 'CURRENT_DATE',
                                        DATE_FORMAT(NOW(),'%h:%i:%s') AS 'CURRENT_TIME',
                                        SEC_TO_TIME(SUM(TIME_TO_SEC(TIMEDIFF(DATE_FORMAT(CHECKOUT_DATE, '%h:%i:%s'),
                                                                DATE_FORMAT(CHECKIN_DATE, '%h:%i:%s'))))) AS LIB_HRS
                                    FROM
                                        LI_CHECKIN_INFO_?AC_YEAR AS CH
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CH.SEMESTER_TYPE
                                            AND ST.IS_ACTIVE = 1
                                            INNER JOIN
	                                    USER_ACCOUNT AS U ON U.USER_ID=CH.USER_ID
		                                     AND U.USERNAME=?SCAN_ID
                                    WHERE CH.IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.LogoutUserLoginDetails:
                    {
                        sQuery = @"UPDATE 
                                    LI_LOGIN_DETAILS
                                    SET 
                                    EXIT_DATE=NOW(),
                                    IS_ACTIVE=?IS_ACTIVE
                                    WHERE 
                                    LOGIN_ID=?LOGIN_ID
                                     AND IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.UpdateTotalSpentTimeByLoginId:
                    {
                        sQuery = @"UPDATE LI_LOGIN_DETAILS
                                    SET 
                                    SPENT_TIME=  SEC_TO_TIME( TIME_TO_SEC(timediff(EXIT_DATE,LOGIN_DATE))+IF(SPENT_TIME IS NULL,0,TIME_TO_SEC(SPENT_TIME)))
                                    WHERE 
                                    LOGIN_ID=?LOGIN_ID
                                     AND IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.FetchLoginIdByUserIdandDate:
                    {
                        sQuery = @"SELECT 
                                        LOGIN_ID
                                    FROM
                                        LI_LOGIN_DETAILS
                                    WHERE
                                        USER_ID = ?USER_ID
                                            AND DATE_FORMAT(LOGIN_DATE, '%Y-%m-%d') = DATE_FORMAT(NOW(), '%Y-%m-%d')
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.UpdateLastLoginDetailsByUserId:
                    {
                        sQuery = @"UPDATE LI_USER_DETAILS 
                                    SET 
                                        LAST_LOGIN =NOW()
                                    WHERE
                                        USER_ID =?USER_ID AND IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.CheckUserExpiredorNot:
                    {
                        sQuery = @"   SELECT 
                                    USER_ID
                                FROM
                                    LI_USER_DETAILS AS LD
                                WHERE
                                    LD.USER_ID =?USER_ID
									AND   DATE_FORMAT(LD.EXPIRED_DATE,'%Y-%m-%d')<= DATE_FORMAT(NOW(),'%Y-%m-%d')
								    AND LD.IS_DELETED != 1;";
                        break;
                    }
                #endregion

                #region Library Dues
                case LibrarySQLCommands.FetchDuesList:
                    {
                        sQuery = @"SELECT 
                                        DUES_ID,
                                      
                                        ISSUE_ID,
                                       
                                        REASON,
                                      CREDIT,
                                      DEBIT,
                                        ACADEMIC_YEAR,
                                        IS_PAID
                                    FROM
                                        LI_ISSUE_DUES
                                    WHERE
                                        IS_PAID != 1 AND IS_DELETED!=1
                                        AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case LibrarySQLCommands.SaveDues:
                    {
                        sQuery = @"INSERT INTO LI_ISSUE_DUES
                                    (
                                        ISSUE_ID,
                                        ACADEMIC_YEAR,
                                        ENTRY_DATE
                                    )
                                    VALUES
                                    (
                                        ?ISSUE_ID,
                                        ?AC_YEAR,
                                        unix_timestamp()
                                    );";
                        break;
                    }
                case LibrarySQLCommands.UpdateDues:
                    {
                        sQuery = @"UPDATE LI_ISSUE_DUES
                                    SET
                                        REASON=?REASON,
                                        DUE_FINE_AMOUNT=IFNULL(DUE_FINE_AMOUNT,0)+?DUE_FINE_AMOUNT,
                                        REVISED_DUE_FINE_AMOUNT=IFNULL(REVISED_DUE_FINE_AMOUNT,0)+?REVISED_DUE_FINE_AMOUNT,
                                        CREDIT=IFNULL(CREDIT,0)+?DUES
                                    WHERE DUES_ID=?DUES_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchtBendingDues:
                    {
                        sQuery = @"SELECT 
   
                                        LD.DUES_ID,
                                        LD.REASON,
                                        LD.FINE_AMOUNT,
                                        LD.IS_PAID,
                                        LB.BOOK_NO,
                                        LB.CLASS_NO,
                                       CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                       UPPER(CONCAT(IFNULL(LB.DOC_TITLE, ''), '  ', IFNULL(LB.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
                                        LB.BOOK_STATUS,
                                        CONCAT(IFNULL(LU.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(LU.LAST_NAME, '')) AS FIRST_NAME,
                                        CONCAT(IFNULL(SM.CATEGORY_CODE, ''),
                                                '-',
                                                IFNULL(SM.CATEGORY_NAME, '')) AS MEMBER_TYPE,
                                        LU.BOID,
                                        LU.PHOTO,
                                        LU.RFID,
                                        LI.ISSUE_DATE,
                                        LI.RETURN_DATE
                                        FROM
                                        LI_DUES AS LD
                                            INNER JOIN
                                        LI_BOOK_ISSUE AS LI ON LI.ISSUE_ID = LD.ISSUE_ID
                                            AND LI.IS_DELETED != 1
                                            INNER JOIN
                                        LI_BOOK_DETAILS AS LB ON LB.BOOK_ID = LD.BOOK_ID
                                            AND LB.IS_DELETED != 1
                                            
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=LB.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=LB.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                            
                                            INNER JOIN
                                        LI_USER_DETAILS AS LU ON LU.USER_ID = LD.USER_ID
                                            AND LU.IS_DELETED != 1
                                            INNER JOIN 
                                            LI_RULES AS LR ON LR.MEM_CATEGORY=LU.MEMBER_TYPE
                                            AND LR.ACADEMIC_YEAR=?AC_YEAR
                                            AND LR.IS_DELETED!=1
                                            LEFT JOIN
                                        SUP_LI_BOOK_STATUS AS SS ON SS.STATUS_ID = LI.STATUS
                                            AND SS.IS_DELETED != 1
                                            LEFT JOIN
                                            SUP_LI_MEMBER_CATEGORY AS SM ON SM.CATEGORY_ID=LU.MEMBER_TYPE
                                            AND SM.IS_DELETED!=1
                                    WHERE
                                        LD.ACADEMIC_YEAR =?AC_YEAR
                                        AND LD.IS_PAID!=1
                                            AND LD.IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.PayFineAmountByDuesId:
                    {
                        sQuery = @"UPDATE 
                                    LI_ISSUE_DUES
                                    SET IS_PAID=?IS_PAID,DEBIT=?DEBIT,
                                    RECEIPT_NO=?RECEIPT_NO,
                                    PAYMENT_DATE=unix_timestamp()
                                    WHERE DUES_ID=?DUES_ID;";
                        break;
                    }
                case LibrarySQLCommands.CheckUserReturnedBookorNot:
                    {
                        sQuery = @"SELECT 
                                        DUES_ID
                                    FROM
                                        LI_ISSUE_DUES AS LD
                                            INNER JOIN
                                        li_book_issue_?AC_YEAR LI ON LI.ISSUE_ID = LD.ISSUE_ID
                                            AND LI.IS_RETURNED= 1 OR LI.IS_MISSED=1
                                            AND LI.IS_DELETED != 1
                                    WHERE
                                        LD.DUES_ID =?DUES_ID
                                        AND LD.ACADEMIC_YEAR=?AC_YEAR
                                            AND LD.IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.UpdateMissingorDamaingAmountByDuesId:
                    {
                        sQuery = @"UPDATE LI_DUES 
                                    SET 
                                        FINE_AMOUNT = FINE_AMOUNT + ?FINE_AMOUNT
                                    WHERE
                                        DUES_ID = ?DUES_ID AND IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchDuesIdByIssueId:
                    {
                        sQuery = @"SELECT 
                                    DUES_ID
                                FROM
                                    LI_DUES
                                WHERE
                                    ISSUE_ID =?ISSUE_ID AND IS_DELETED != 1;";
                        break;
                    }

                case LibrarySQLCommands.UpdateLiDuesForDamageFine:
                    {
                        sQuery = @" UPDATE LI_ISSUE_DUES SET DAMAGE_FINE_AMOUNT=?DAMAGE_FINE_AMOUNT,CREDIT= if(CREDIT is NULL,0,CREDIT)+?DAMAGE_FINE_AMOUNT WHERE ISSUE_ID=?ISSUE_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateLiDuesForMissingFine:
                    {
                        sQuery = @"UPDATE LI_ISSUE_DUES SET MISSING_FINE_AMOUNT=?MISSING_FINE_AMOUNT,CREDIT=if(CREDIT is NULL,0,CREDIT)+?MISSING_FINE_AMOUNT WHERE ISSUE_ID=?ISSUE_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchDueDetailsForPaymentByDuesId:
                    {
                        sQuery = @"SELECT 
                                    LI.ISSUE_ID,
                                      IF(LD.CREDIT!=0,LD.CREDIT,0) AS CREDIT,
									IF(LD.DEBIT!=0,LD.DEBIT,0) AS DEBIT,
                                    IF(LD.CREDIT!=0,LD.CREDIT,0)-IF(LD.DEBIT!=0,LD.DEBIT,0) AS BALANCE,
                                    LD.DUES_ID,
                                    IF(LD.DUE_FINE_AMOUNT != 0,
                                        LD.DUE_FINE_AMOUNT,

                                        0) AS DUE_FINE_AMOUNT,
                                    IF(LD.REVISED_DUE_FINE_AMOUNT != 0,
                                        LD.REVISED_DUE_FINE_AMOUNT,
                                        0) AS REVISED_DUE_FINE_AMOUNT,
                                    IF(LD.DAMAGE_FINE_AMOUNT != 0,
                                        LD.DAMAGE_FINE_AMOUNT,
                                        0) AS DAMAGE_FINE_AMOUNT,
                                    IF(LD.MISSING_FINE_AMOUNT != 0,
                                        LD.MISSING_FINE_AMOUNT,
                                        0) AS MISSING_FINE_AMOUNT
                                FROM
                                    LI_ISSUE_DUES AS LD
                                        INNER JOIN
                                    LI_BOOK_ISSUE_?AC_YEAR AS LI ON LI.ISSUE_ID = LD.ISSUE_ID
                                        AND LI.IS_DELETED != 1
                                WHERE
                                    LD.IS_DELETED != 1 AND LD.IS_PAID != 1
                                     AND LD.ACADEMIC_YEAR=?AC_YEAR
                                    AND LD.DUES_ID=?DUES_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchDueDetailsForRecieptByDuesId:
                    {
                        sQuery = @"SELECT 
                                    LI.ISSUE_ID,
                                    IF(LD.CREDIT != 0,
                                        LD.CREDIT,
                                        0) AS CREDIT,
                                    IF(LD.DEBIT != 0,
                                        LD.DEBIT,
                                        0) AS DEBIT,
                                    LD.DUES_ID,
                                    IF(LD.DUE_FINE_AMOUNT != 0,
                                        LD.DUE_FINE_AMOUNT,
                                        0) AS DUE_FINE_AMOUNT,
                                    IF(LD.REVISED_DUE_FINE_AMOUNT != 0,
                                        LD.REVISED_DUE_FINE_AMOUNT,
                                        0) AS REVISED_DUE_FINE_AMOUNT,
                                    IF(LD.DAMAGE_FINE_AMOUNT != 0,
                                        LD.DAMAGE_FINE_AMOUNT,
                                        0) AS DAMAGE_FINE_AMOUNT,
                                    IF(LD.MISSING_FINE_AMOUNT != 0,
                                        LD.MISSING_FINE_AMOUNT,
                                        0) AS MISSING_FINE_AMOUNT,
                                      DATE_FORMAT(FROM_UNIXTIME(LD.PAYMENT_DATE), '%Y-%m-%d') AS PAYMENT_DATE
                                      
                                FROM
                                    LI_ISSUE_DUES AS LD
                                        INNER JOIN
                                    LI_BOOK_ISSUE_?AC_YEAR AS LI ON LI.ISSUE_ID = LD.ISSUE_ID
                                        AND LI.IS_DELETED != 1
                                WHERE
                                    LD.IS_DELETED != 1 AND LD.IS_PAID = 1
                                    AND LD.ACADEMIC_YEAR=?AC_YEAR
                                    AND LD.DUES_ID=?DUES_ID;";
                        break;
                    }
                #endregion

                #region Report
                case LibrarySQLCommands.FetchTotalIssuedBooksByDate:
                    {
                        sQuery = @"SELECT 
                                        ISSUE_ID,
                                        BI.USER_ID,
                                        BI.BOOK_ID,
                                        B.BOOK_NO,
                                        UPPER(NAME) AS FIRST_NAME,
                                        USERNAME,
                                        UT.USER_TYPE_NAME,
                                        B.ACCESS_NO,
                                        B.DOC_TITLE AS BOOK_TITLE,
                                        CONCAT(IFNULL(B.PERSONAL_AUTHOR1, ''),
                                            ' ',
                                            IFNULL(B.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                           
                                        concat(IFNULL(SP.FIRST_NAME,''),' - ',IFNULL(SP.STAFF_CODE,'')) AS ISSUE_BY,
  	                                    concat(IFNULL(STF.FIRST_NAME,''),' - ',IFNULL(STF.STAFF_CODE,'')) AS RECEIVED_BY,
                                        DATE_FORMAT(FROM_UNIXTIME(BI.ISSUE_DATE), '%Y-%m-%d') AS ISSUE_DATE,
                                        DATE_FORMAT(FROM_UNIXTIME(BI.RETURN_DATE), '%Y-%m-%d') AS RETURN_DATE,
                                        DATE_FORMAT(FROM_UNIXTIME(BI.ACTUAL_RETURNED_DATE), '%Y-%m-%d') AS ACTUAL_RETURNED_DATE
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS BI
                                            INNER JOIN
                                        LI_BOOK_INFO AS B ON B.BOOK_ID = BI.BOOK_ID
                                            AND B.IS_DELETED != 1
                                            INNER JOIN
	                                    USER_ACCOUNT AS UA ON UA.USER_ID=BI.USER_ID
		                                    AND UA.USER_TYPE=BI.USER_TYPE
                                            AND UA.IS_DELETED!=1
                                            LEFT JOIN
	                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID=BI.ISSUE_BY
		                                    AND SP.IS_DELETED!=1
                                            left JOIN
	                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID=BI.RECEIVED_BY
		                                    AND STF.IS_DELETED!=1
                                            LEFT JOIN
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=BI.USER_TYPE
                                    WHERE
                                        BI.IS_DELETED != 1
                                            AND (DATE_FORMAT(FROM_UNIXTIME(BI.ISSUE_DATE), '%Y-%m-%d') BETWEEN ?FROM_DATE AND ?TO_DATE)
                                    ORDER BY DATE_FORMAT(FROM_UNIXTIME(BI.ISSUE_DATE), '%Y-%m-%d') ASC, BI.ISSUE_ID ASC;";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalReturnedBooksByDate:
                    {
                        sQuery = @"SELECT 
                                            ISSUE_ID,
                                            BI.USER_ID,
                                            BI.BOOK_ID,
                                            B.BOOK_NO,
                                            UPPER(NAME) AS FIRST_NAME,
                                            USERNAME,
                                             UT.USER_TYPE_NAME,
                                            B.ACCESS_NO,
                                            B.DOC_TITLE AS BOOK_TITLE,
                                            CONCAT(IFNULL(B.PERSONAL_AUTHOR1, ''),
                                            ' ',
                                            IFNULL(B.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                            concat(IFNULL(SP.FIRST_NAME,''),' - ',IFNULL(SP.STAFF_CODE,'')) AS ISSUE_BY,
  	                                        concat(IFNULL(STF.FIRST_NAME,''),' - ',IFNULL(STF.STAFF_CODE,'')) AS RECEIVED_BY,
                                            DATE_FORMAT(FROM_UNIXTIME(BI.ISSUE_DATE), '%Y-%m-%d') AS ISSUE_DATE,
                                            DATE_FORMAT(FROM_UNIXTIME(BI.RETURN_DATE), '%Y-%m-%d') AS RETURN_DATE,
                                            DATE_FORMAT(FROM_UNIXTIME(BI.ACTUAL_RETURNED_DATE), '%Y-%m-%d') AS ACTUAL_RETURNED_DATE
                                        FROM
                                            LI_BOOK_ISSUE_?AC_YEAR AS BI
                                                INNER JOIN
                                            LI_BOOK_INFO AS B ON B.BOOK_ID = BI.BOOK_ID
                                                AND B.IS_DELETED != 1
                                                INNER JOIN
	                                        USER_ACCOUNT AS UA ON UA.USER_ID=BI.USER_ID
		                                        AND UA.USER_TYPE=BI.USER_TYPE
                                                AND UA.IS_DELETED!=1
                                                LEFT JOIN
	                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID=BI.ISSUE_BY
		                                        AND SP.IS_DELETED!=1
                                                left JOIN
	                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID=BI.RECEIVED_BY
		                                        AND STF.IS_DELETED!=1
                                                
                                                 LEFT JOIN
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=BI.USER_TYPE
                                        WHERE
                                            BI.IS_DELETED != 1 
                                                AND (DATE_FORMAT(FROM_UNIXTIME(BI.ACTUAL_RETURNED_DATE), '%Y-%m-%d') BETWEEN ?FROM_DATE AND ?TO_DATE)
                                        ORDER BY DATE_FORMAT(FROM_UNIXTIME(BI.ACTUAL_RETURNED_DATE), '%Y-%m-%d') ASC, BI.ISSUE_ID ASC;";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalReservedBooksByDate:
                    {
                        sQuery = @"SELECT 
                                        RESERVE_ID,
                                        R.USER_ID,
                                        u.PHOTO,
                                        R.BOOK_ID,
                                        UPPER(NAME) AS FIRST_NAME,
                                        USERNAME,
                                        B.ACCESS_NO,
                                        B.DOC_TITLE AS BOOK_TITLE,
                                        IS_CANCELLED,
                                        IS_USER_CANCELLED,
                                        IS_SYSTEM_CANCELLED,
                                        IS_CSMS_SENT AS 'CANCELLED_SMS',
                                        IS_ASMS_SENT AS 'APPROVED_SMS',
                                        IS_SCSMS_SENT AS 'SYSTEM_CANCELLED_SMS',
                                        DATE_FORMAT(FROM_UNIXTIME(R.ENTRY_DATE), '%Y-%m-%d') AS ENTRY_DATE,
                                        DATE_FORMAT(FROM_UNIXTIME(R.APPROVED_DATE),
                                                '%Y-%m-%d') AS APPROVED_DATE,
                                        DATE_FORMAT(FROM_UNIXTIME(R.CANCELLED_DATE),
                                                '%Y-%m-%d') AS CANCELLED_DATE
                                    FROM
                                        LI_BOOK_RESERVE AS R
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = R.SEMESTER_TYPE
                                            AND ST.IS_ACTIVE = 1
                                            INNER JOIN
                                        USER_ACCOUNT AS U ON U.USER_ID = R.USER_ID
                                            AND U.USER_TYPE = R.USER_TYPE
                                            INNER JOIN
                                        LI_BOOK_INFO AS B ON B.BOOK_ID = R.BOOK_ID
                                            AND B.IS_DELETED != 1
                                    WHERE
                                        R.IS_DELETED != 1 
                                            AND (DATE_FORMAT(FROM_UNIXTIME(R.ENTRY_DATE), '%Y-%m-%d') BETWEEN ?FROM_DATE AND ?TO_DATE)
                                            AND R.ACADEMIC_YEAR = ?AC_YEAR 
                                            ORDER BY DATE_FORMAT(FROM_UNIXTIME(R.ENTRY_DATE), '%Y-%m-%d') ASC, U.USER_ID ASC;";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalRecallBooksByDate:
                    {
                        sQuery = @" SELECT 
                                    LM.USER_ID,
                                    CONCAT(IFNULL(LD.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LD.LAST_NAME, '')) AS FIRST_NAME,
	                                CONCAT(IFNULL(SM.CATEGORY_CODE,''),'-',IFNULL(SM.CATEGORY_NAME,'')) AS MEMBER_TYPE,
                                    LD.BOID,
                                    LD.PHOTO,
                                    LD.RFID,
                                    LB.BOOK_ID,
                                    LB.BOOK_NO,
                                    LB.BOOK_NO,
                                    CONCAT(IFNULL(SLC.CLASS_CODE, ''),
									' - ',
									IFNULL(SLC.CLASS_NAME, '')) AS CLASS_NAME,
									CONCAT(IFNULL(SLS.SUBJECT_CODE, ''),
									' - ',
									IFNULL(SLS.SUBJECT_NAME, '')) AS SUBJECT_NAME,
                                    CONCAT(IFNULL(LBB.BLOCK_NAME, ''),
                                            ' ',
                                            'BOW:',
                                            IFNULL(LR.BOW_NO, ''),
                                            ' ',
                                            'RACK:',
                                            IFNULL(LR.RACK_NO, ''),
                                            ' ',
                                            'RACK:',
                                            IFNULL(LR.SECTION_NO, ''),
                                            ' ',
                                            'SECTION:',
                                            IFNULL(LR.POSITION, '')) AS RACK_NO,
                                    GROUP_CONCAT(IFNULL(LA.FIRST_NAME, ''),' ',IFNULL(LA.LAST_NAME, '')) AS AUTHOR,
                                    LB.BOOK_TITLE,
                                    LB.STATUS AS BOOK_STATUS,
                                LM.REASON,
                                LM.MESSAGE_CONTENT,
                                LM.SENT_DATE,
                                LM.STATUS
                                FROM LI_MEMBER_RECALL AS LM
                                INNER JOIN 
                                  LI_USER_DETAILS AS LD ON LD.USER_ID = LM.USER_ID
                                        AND LD.IS_DELETED != 1
                                        INNER JOIN
                                    LI_BOOK_DETAILS AS LB ON LB.BOOK_ID = LM.BOOK_ID
                                        AND LB.IS_DELETED != 1
							
                                     LEFT JOIN 
                                     SUP_LI_MEMBER_CATEGORY AS SM ON SM.CATEGORY_ID=LD.MEMBER_TYPE
                                     AND SM.IS_DELETED!=1
                                     
									 INNER JOIN
                                    LI_AUTHOR_INFO AS LA ON FIND_IN_SET( LA.AUTHOR_ID,LB.AUTHOR_ID)
                                        AND LA.IS_DELETED != 1
                                       INNER JOIN
                                    LI_RACK_INFO AS LR ON LR.RACK_ID = LB.RACK_ID
                                        AND LR.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_LI_BLOCK AS LBB ON LBB.BLOCK_ID = LR.BLOCK_ID
                                        AND LBB.IS_DELETED != 1
                                        INNER JOIN
										SUP_LI_CLASS AS SLC ON SLC.CLASS_ID = LB.CLASS_ID
										AND SLC.IS_DELETED != 1
										INNER JOIN
										SUP_LI_SUBJECTS AS SLS ON SLS.LI_SUBJECT_ID = LB.SUBJECT_ID
										AND LB.IS_DELETED != 1
                                     
                                WHERE 
                                 DATE_FORMAT(LM.SENT_DATE,'%Y-%m-%d')=DATE_FORMAT(?SENT_DATE,'%Y-%m-%d')
                                 AND LM.IS_DELETED!=1
                                 GROUP BY LM.RECALL_ID
                                 ORDER BY LM.RECALL_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalFineListByDate:
                    {
                        sQuery = @"   
                                SELECT 
                                LD.USER_ID,
                                    CONCAT(IFNULL(LUD.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LUD.LAST_NAME, '')) AS FIRST_NAME,
	                                CONCAT(IFNULL(SM.CATEGORY_CODE,''),'-',IFNULL(SM.CATEGORY_NAME,'')) AS MEMBER_TYPE,
                                    LUD.BOID,
                                    LUD.PHOTO,
                                    LUD.RFID,
                                    LB.BOOK_ID,
                                    LB.BOOK_NO,
                                    LB.CLASS_NO,
                                    LB.RACK_ID,
                                    CONCAT(IFNULL(LB.PERSONAL_AUTHOR1, ''),
                                            ' ',
                                            IFNULL(LB.PERSONAL_AUTHOR2, '')) AS AUTHOR,
									LB.BOOK_TITLE,
									LB.STATUS AS BOOK_STATUS,
									LI.ISSUE_DATE,
									LD.REASON,
									LD.FINE_AMOUNT,
									LD.IS_PAID
                                
                                FROM 
                                LI_DUES AS LD 
                                INNER JOIN LI_USER_DETAILS AS LUD ON LUD.USER_ID=LD.USER_ID
                                AND LUD.IS_DELETED!=1
                                
                                INNER JOIN
                                LI_BOOK_DETAILS AS LB ON LB.BOOK_ID=LD.BOOK_ID
                                AND LB.IS_DELETED!=1
                                
                                INNER JOIN 
								LI_BOOK_ISSUE AS LI ON LI.USER_ID=LD.USER_ID
                                AND LI.BOOK_ID=LD.BOOK_ID
                                AND LI.IS_DELETED!=1
                                
                                LEFT JOIN
								LI_BOOK_RETURN AS LR ON LR.ISSUE_ID=LI.ISSUE_ID
                                AND LR.IS_DELETED!=1
                                
                                INNER JOIN 
                                SUP_LI_MEMBER_CATEGORY AS SM ON SM.CATEGORY_ID=LUD.MEMBER_TYPE
                                AND SM.IS_DELETED!=1
                                
                                WHERE 
                                LD.USER_ID=?USER_ID
                                AND LD.ACADEMIC_YEAR=?AC_YEAR
                                AND LD.IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.BindTotalVisitorsByDate:
                    {
                        sQuery = @"SELECT 
                                        U.USERNAME,
                                        U.NAME AS FIRST_NAME,
                                        DATE_FORMAT(LI.CHECKIN_DATE, '%Y-%m-%d') as CHECKIN_DATE,
                                        TIME_FORMAT(DATE_FORMAT(LI.CHECKIN_DATE, '%H:%i:%s'),'%r') as CHECKIN_TIME,
                                        DATE_FORMAT(LI.CHECKOUT_DATE, '%Y-%m-%d') as CHECKOUT_DATE,
                                        TIME_FORMAT(DATE_FORMAT(LI.CHECKOUT_DATE, '%H:%i:%s'),'%r') as CHECKOUT_TIME,
                                        CASE 
                                            WHEN U.USER_TYPE != 5 THEN 'STAFF'
                                            ELSE 'STUDENT'
                                        END AS USER_TYPE_NAME,
                                        COALESCE(CC.CLASS_NAME, CC2.CLASS_NAME, '') AS CLASS_NAME,
                                        COUNT(U.USERNAME) AS 'COUNT'
                                    FROM
                                        LI_CHECKIN_INFO_?AC_YEAR AS LI
		                                    INNER JOIN
	                                    USER_ACCOUNT AS U ON U.USER_ID = LI.USER_ID
		                                    AND U.USER_TYPE = LI.USER_TYPE
                                            INNER JOIN
	                                    SUP_USER_TYPE AS T ON T.USER_TYPE_ID = U.USER_TYPE
                                            LEFT JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = LI.USER_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            LEFT JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            AND CC.IS_DELETED != 1
                                            LEFT JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = LI.USER_ID
                                            LEFT JOIN
                                        CP_CLASSES AS CC2 ON CC2.CLASS_ID = SP.CLASS_ID
                                            AND CC2.IS_DELETED != 1
                                    WHERE
                                        (DATE_FORMAT(LI.CHECKIN_DATE, '%Y-%m-%d') BETWEEN ?FROM_DATE AND ?TO_DATE)
                                        AND LI.IS_DELETED != 1
                                    GROUP BY LI.USER_ID, LI.USER_TYPE, DATE_FORMAT(LI.CHECKIN_DATE, '%Y-%m-%d')
                                    ORDER BY 
                                        CASE 
                                            WHEN U.USER_TYPE != 5 THEN 1
                                            WHEN COALESCE(CC.CLASS_LEVEL, CC2.CLASS_LEVEL) = 1 AND COALESCE(CC.CLASS_YEAR, CC2.CLASS_YEAR) = 3 THEN 2
                                            WHEN COALESCE(CC.CLASS_YEAR, CC2.CLASS_YEAR) = 2 THEN 3
                                            WHEN COALESCE(CC.CLASS_YEAR, CC2.CLASS_YEAR) = 1 THEN 4
                                            ELSE 5
                                        END ASC,
                                        COALESCE(CC.CLASS_LEVEL, CC2.CLASS_LEVEL) ASC,
                                        DATE_FORMAT(LI.CHECKIN_DATE, '%Y-%m-%d') ASC,
                                        TIME_FORMAT(DATE_FORMAT(LI.CHECKIN_DATE, '%H:%i:%s'), '%r') ASC,
                                        U.USERNAME ASC;";
                        break;
                    }


                case LibrarySQLCommands.BindLibraryCheckoutEntryByDate:
                    {
                        sQuery = @"SELECT 
                                            U.USERNAME,
                                            U.NAME AS FIRST_NAME,
                                            DATE_FORMAT(LI.CHECKIN_DATE, '%Y-%m-%d') AS CHECKIN_DATE,
                                            TIME_FORMAT(LI.CHECKIN_DATE, '%r') AS CHECKIN_TIME,
                                            DATE_FORMAT(LI.CHECKOUT_DATE, '%Y-%m-%d') AS CHECKOUT_DATE,
                                            TIME_FORMAT(LI.CHECKOUT_DATE, '%r') AS CHECKOUT_TIME,
                                            T.USER_TYPE_NAME,
                                            LI.CHECKIN_ID AS CHECKIN
                                        FROM
                                            LI_CHECKIN_INFO_?AC_YEAR AS LI
                                                INNER JOIN
                                            USER_ACCOUNT AS U ON U.USER_ID = LI.USER_ID
                                                AND U.USER_TYPE = LI.USER_TYPE
                                                INNER JOIN
                                            SUP_USER_TYPE AS T ON T.USER_TYPE_ID = U.USER_TYPE
                                        WHERE
                                            (DATE(LI.CHECKIN_DATE) BETWEEN ?FROM_DATE AND ?TO_DATE)
                                                AND LI.IS_DELETED != 1
                                                AND LI.IS_CHECKOUT != 1
                                        ORDER BY DATE(LI.CHECKIN_DATE) ASC, LI.CHECKIN_ID ASC;";
                        break;
                    }

                case LibrarySQLCommands.FetchTotalLibraryMembers:
                    {
                        sQuery = @"SELECT 
                                    LD.USER_ID,
                                    CONCAT(IFNULL(LD.FIRST_NAME,''),' ',IFNULL(LD.LAST_NAME,'')) AS FIRST_NAME,
                                    LD.BOID,
                                    LD.LAST_LOGIN,
                                   DATE_FORMAT(LD.DATE_OF_BIRTH,'%d-%m-%Y') AS DATE_OF_BIRTH,
                                    CONCAT(IFNULL(SM.CATEGORY_CODE, ''),
                                                                            ' - ',
                                                                            IFNULL(SM.CATEGORY_NAME, '')) AS MEMBER_TYPE,
                                    SD.DEGREE,
                                    DATE_FORMAT(LD.REGISTERED_DATE,'%d-%m-%Y') AS REGISTERED_DATE,
                                    DATE_FORMAT(LD.EXPIRED_DATE,'%d-%m-%Y') AS EXPIRED_DATE,
                                    LD.PHOTO,
                                    LD.RFID
                                FROM
                                    LI_USER_DETAILS AS LD
                                        INNER JOIN
                                    SUP_LI_MEMBER_CATEGORY AS SM ON SM.CATEGORY_ID = LD.MEMBER_TYPE
                                        AND SM.IS_DELETED != 1
        
                                        LEFT JOIN 
                                        SUP_DEGREE AS SD ON SD.DEGREE_ID=LD.DEGREE
                                        AND SD.ISDELETED!=1
                                   
                                WHERE
                                    DATE_FORMAT(LD.EXPIRED_DATE,'%Y-%m-%d')>= DATE_FORMAT(NOW(),'%Y-%m-%d')
                                        AND LD.IS_DELETED != 1;   ";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalExpiredMembersByToday:
                    {
                        sQuery = @"	SELECT 
		                            LD.USER_ID,
		                            CONCAT(IFNULL(LD.FIRST_NAME,''),' ',IFNULL(LD.LAST_NAME,'')) AS FIRST_NAME,
		                            LD.BOID,
		                            LD.LAST_LOGIN,
		                            DATE_FORMAT(LD.DATE_OF_BIRTH,'%d-%m-%Y') AS DATE_OF_BIRTH,
		                            CONCAT(IFNULL(SM.CATEGORY_CODE, ''),
												                            ' - ',
												                            IFNULL(SM.CATEGORY_NAME, '')) AS MEMBER_TYPE,
		                            SD.DEGREE,
		                            DATE_FORMAT(LD.REGISTERED_DATE,'%Y-%m-%d') AS REGISTERED_DATE,
		                            DATE_FORMAT(LD.EXPIRED_DATE,'%Y-%m-%d') AS EXPIRED_DATE,
		                            LD.PHOTO,
		                            LD.RFID
	                            FROM
		                            LI_USER_DETAILS AS LD
			                            INNER JOIN
		                            SUP_LI_MEMBER_CATEGORY AS SM ON SM.CATEGORY_ID = LD.MEMBER_TYPE
			                            AND SM.IS_DELETED != 1
			
			                            LEFT JOIN 
			                            SUP_DEGREE AS SD ON SD.DEGREE_ID=LD.DEGREE
			                            AND SD.ISDELETED!=1
		                              --  AND SD.ISACTIVE=1
	                            WHERE
		                            
                                     DATE_FORMAT(LD.EXPIRED_DATE,'%Y-%m-%d')<= DATE_FORMAT(NOW(),'%Y-%m-%d')
			                            AND LD.IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalHomeReaders:
                    {
                        sQuery = @"   SELECT 
                                        LD.USER_ID,
                                        CONCAT(IFNULL(LD.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(LD.LAST_NAME, '')) AS FIRST_NAME,
                                        CONCAT(IFNULL(SM.CATEGORY_CODE, ''),
                                                ' - ',
                                                IFNULL(SM.CATEGORY_NAME, '')) AS MEMBER_TYPE,
                                        LD.BOID,
                                        LD.PHOTO,
                                        LD.RFID,
                                        LB.BOOK_NO,
                                         CONCAT(IFNULL(SLC.CLASS_CODE, ''),
									' - ',
									IFNULL(SLC.CLASS_NAME, '')) AS CLASS_NAME,
									CONCAT(IFNULL(SLS.SUBJECT_CODE, ''),
									' - ',
									IFNULL(SLS.SUBJECT_NAME, '')) AS SUBJECT_NAME,
                                    CONCAT(IFNULL(LBB.BLOCK_NAME, ''),
                                            ' ',
                                            'BOW:',
                                            IFNULL(LR.BOW_NO, ''),
                                            ' ',
                                            'RACK:',
                                            IFNULL(LR.RACK_NO, ''),
                                            ' ',
                                            'RACK:',
                                            IFNULL(LR.SECTION_NO, ''),
                                            ' ',
                                            'SECTION:',
                                            IFNULL(LR.POSITION, '')) AS RACK_NO,
                                    GROUP_CONCAT(IFNULL(LA.FIRST_NAME, ''),' ',IFNULL(LA.LAST_NAME, '')) AS AUTHOR,
                                        LB.BOOK_TITLE,
                                        LB.STATUS AS BOOK_STATUS,
                                        LI.ISSUE_DATE
                                    FROM
										LI_BOOK_ISSUE AS LI
                                        INNER JOIN
                                    LI_USER_DETAILS AS LD ON LD.USER_ID = LI.USER_ID
                                        AND LD.IS_DELETED != 1
                                        INNER JOIN
                                    LI_BOOK_DETAILS AS LB ON LB.BOOK_ID = LI.BOOK_ID
                                        AND LB.IS_DELETED != 1
							
                                     LEFT JOIN 
                                     SUP_LI_MEMBER_CATEGORY AS SM ON SM.CATEGORY_ID=LD.MEMBER_TYPE
                                     AND SM.IS_DELETED!=1
                                     
									 INNER JOIN
                                    LI_AUTHOR_INFO AS LA ON FIND_IN_SET( LA.AUTHOR_ID,LB.AUTHOR_ID)
                                        AND LA.IS_DELETED != 1
                                       INNER JOIN
                                    LI_RACK_INFO AS LR ON LR.RACK_ID = LB.RACK_ID
                                        AND LR.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_LI_BLOCK AS LBB ON LBB.BLOCK_ID = LR.BLOCK_ID
                                        AND LBB.IS_DELETED != 1
                                        INNER JOIN
										SUP_LI_CLASS AS SLC ON SLC.CLASS_ID = LB.CLASS_ID
										AND SLC.IS_DELETED != 1
										INNER JOIN
										SUP_LI_SUBJECTS AS SLS ON SLS.LI_SUBJECT_ID = LB.SUBJECT_ID
										AND LB.IS_DELETED != 1
                                    WHERE
                                            LI.ACADEMIC_YEAR=?AC_YEAR
                                            AND LI.STATUS != 2
                                            AND LI.STATUS != 3
                                            AND LI.IS_DELETED != 1
                                            GROUP BY LI.ISSUE_ID
                                            ORDER BY LI.ISSUE_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalIssueByStartandEndDate:
                    {
                        sQuery = @"SELECT 
                                 LI.ISSUE_ID,
                                        LD.USER_ID,
                                        CONCAT(IFNULL(LD.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(LD.LAST_NAME, '')) AS FIRST_NAME,
                                        CONCAT(IFNULL(SM.CATEGORY_CODE, ''),
                                                '-',
                                                IFNULL(SM.CATEGORY_NAME, '')) AS MEMBER_TYPE,
                                        LD.BOID,
                                        LD.PHOTO,
                                        LD.RFID,
                                        SD.DEGREE,
                                        LB.BOOK_ID,
                                        LB.BOOK_NO,
                                        LB.CLASS_NO,
                                        LB.RACK_ID,
                                        CONCAT(IFNULL(LB.PERSONAL_AUTHOR1, ''),
                                                ' ',
                                                IFNULL(LB.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                        LB.BOOK_TITLE,
                                        LB.STATUS AS BOOK_STATUS,
                                        LI.ISSUE_DATE,
                                        CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SPI.LAST_NAME, '')) AS ISSUE_BY,
                                        SS.STATUS_NAME AS STATUS
                                    FROM
                                        LI_BOOK_ISSUE AS LI
                                            INNER JOIN
                                        LI_USER_DETAILS AS LD ON LD.USER_ID = LI.USER_ID
                                            AND LD.IS_DELETED != 1
                                            INNER JOIN
                                        LI_BOOK_DETAILS AS LB ON LB.BOOK_ID = LI.BOOK_ID
                                            AND LB.IS_DELETED != 1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = LI.ISSUE_BY
                                            AND SPI.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_LI_MEMBER_CATEGORY AS SM ON SM.CATEGORY_ID = LD.MEMBER_TYPE
                                            AND SM.IS_DELETED != 1
        
                                            LEFT JOIN  SUP_DEGREE AS SD ON SD.DEGREE_ID=LD.DEGREE
                                            AND SD.ISDELETED!=1
        
                                            LEFT JOIN 
                                            SUP_LI_STATUS AS SS ON SS.STATUS_ID=LI.STATUS
                                            AND SS.IS_DELETED!=1
                                    WHERE
                                              LI.ISSUE_DATE BETWEEN DATE_FORMAT(?START_DATE, '%Y-%m-%d') AND DATE_FORMAT(?END_DATE, '%Y-%m-%d')
                                            AND LI.IS_DELETED != 1;";
                        break;
                    }

                case LibrarySQLCommands.FetchTotalDuesByDate:
                    {
                        sQuery = @"SELECT 
                                    LD.DUES_ID,
                                    LD.USER_ID,
                                    LD.BOOK_ID,
                                    LD.ISSUE_ID,
                                    LD.REASON,
                                    LD.FINE_AMOUNT,
                                    LD.ACADEMIC_YEAR,
                                    LD.IS_PAID,
                                    LB.BOOK_NO,
                                    LB.CLASS_NO,
                                    LB.RACK_ID,
                                    CONCAT(IFNULL(LB.PERSONAL_AUTHOR1, ''),
                                            ' ',
                                            IFNULL(LB.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                    LB.BOOK_TITLE,
                                    LB.STATUS AS BOOK_STATUS,
                                    CONCAT(IFNULL(LU.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LU.LAST_NAME, '')) AS FIRST_NAME,
                                    CONCAT(IFNULL(SM.CATEGORY_CODE, ''),
                                            '-',
                                            IFNULL(SM.CATEGORY_NAME, '')) AS MEMBER_TYPE,
                                    LU.BOID,
                                    LU.PHOTO,
                                    LU.RFID,
                                    LI.ISSUE_DATE,
                                    LI.RETURN_DATE,
                                    LI.STATUS
                                FROM
                                    LI_DUES AS LD
                                        INNER JOIN
                                    LI_BOOK_ISSUE AS LI ON LI.ISSUE_ID = LD.ISSUE_ID
                                        AND LI.IS_DELETED != 1
                                        INNER JOIN
                                    LI_BOOK_DETAILS AS LB ON LB.BOOK_ID = LD.BOOK_ID
                                        AND LB.IS_DELETED != 1
                                        INNER JOIN
                                    LI_USER_DETAILS AS LU ON LU.USER_ID = LD.USER_ID
                                        AND LU.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_LI_STATUS AS SS ON SS.STATUS_ID = LI.STATUS
                                        AND SS.IS_DELETED != 1
                                        LEFT JOIN
                                        SUP_LI_MEMBER_CATEGORY AS SM ON SM.CATEGORY_ID=LU.MEMBER_TYPE
                                        AND SM.IS_DELETED!=1
                                WHERE
                                    LD.ACADEMIC_YEAR =?AC_YEAR
                                    AND LI.ISSUE_DATE=?ISSUE_DATE
                                        AND LD.IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalFinePaidList:
                    {
                        sQuery = @"       
                                    SELECT 
                                    LD.DUES_ID,
                                    LD.USER_ID,
                                    LD.BOOK_ID,
                                    LD.ISSUE_ID,
                                    LD.REASON,
                                     LD.PAYMENT_DATE,
                                    LD.FINE_AMOUNT,
                                    LD.ACADEMIC_YEAR,
                                    LD.IS_PAID,
                                    LB.BOOK_NO,
                                    CONCAT(IFNULL(SLC.CLASS_CODE, ''),
									' - ',
									IFNULL(SLC.CLASS_NAME, '')) AS CLASS_NAME,
									CONCAT(IFNULL(SLS.SUBJECT_CODE, ''),
									' - ',
									IFNULL(SLS.SUBJECT_NAME, '')) AS SUBJECT_NAME,
                                    CONCAT(IFNULL(LBB.BLOCK_NAME, ''),
                                            ' ',
                                            'BOW:',
                                            IFNULL(LR.BOW_NO, ''),
                                            ' ',
                                            'RACK:',
                                            IFNULL(LR.RACK_NO, ''),
                                            ' ',
                                            'RACK:',
                                            IFNULL(LR.SECTION_NO, ''),
                                            ' ',
                                            'SECTION:',
                                            IFNULL(LR.POSITION, '')) AS RACK_NO,
                                    GROUP_CONCAT(IFNULL(LA.FIRST_NAME, ''),' ',IFNULL(LA.LAST_NAME, '')) AS AUTHOR,
                                    LB.BOOK_TITLE,
                                    LB.STATUS AS BOOK_STATUS,
                                    CONCAT(IFNULL(LU.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LU.LAST_NAME, '')) AS FIRST_NAME,
                                    CONCAT(IFNULL(SM.CATEGORY_CODE, ''),
                                            '-',
                                            IFNULL(SM.CATEGORY_NAME, '')) AS MEMBER_TYPE,
                                    LU.BOID,
                                    LU.PHOTO,
                                    LU.RFID,
                                    LI.ISSUE_DATE,
                                    LI.RETURN_DATE,
                                    SS.STATUS_NAME AS STATUS
                                FROM
                                    LI_DUES AS LD
                                        INNER JOIN
                                    LI_BOOK_ISSUE AS LI ON LI.ISSUE_ID = LD.ISSUE_ID
                                        AND LI.IS_DELETED != 1
                                        INNER JOIN
                                    LI_BOOK_DETAILS AS LB ON LB.BOOK_ID = LD.BOOK_ID
                                        AND LB.IS_DELETED != 1
                                        INNER JOIN
                                    LI_USER_DETAILS AS LU ON LU.USER_ID = LD.USER_ID
                                        AND LU.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_LI_STATUS AS SS ON SS.STATUS_ID = LI.STATUS
                                        AND SS.IS_DELETED != 1
                                        INNER JOIN
                                          SUP_LI_MEMBER_CATEGORY AS SM ON SM.CATEGORY_ID=LU.MEMBER_TYPE
                                     AND SM.IS_DELETED!=1
									 INNER JOIN
                                    LI_AUTHOR_INFO AS LA ON FIND_IN_SET( LA.AUTHOR_ID,LB.AUTHOR_ID)
                                        AND LA.IS_DELETED != 1
                                       INNER JOIN
                                    LI_RACK_INFO AS LR ON LR.RACK_ID = LB.RACK_ID
                                        AND LR.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_LI_BLOCK AS LBB ON LBB.BLOCK_ID = LR.BLOCK_ID
                                        AND LBB.IS_DELETED != 1
                                        INNER JOIN
										SUP_LI_CLASS AS SLC ON SLC.CLASS_ID = LB.CLASS_ID
										AND SLC.IS_DELETED != 1
										INNER JOIN
										SUP_LI_SUBJECTS AS SLS ON SLS.LI_SUBJECT_ID = LB.SUBJECT_ID
										AND LB.IS_DELETED != 1
                                WHERE
                                    LD.ACADEMIC_YEAR =?AC_YEAR
								AND   LD.IS_PAID=1
								AND LD.IS_DELETED != 1
                                GROUP BY LD.DUES_ID
                                ORDER BY LD.DUES_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalFinePaidListByDate:
                    {
                        sQuery = @"SELECT 
                                LD.DUES_ID,
                                LD.REASON,
                                DATE_FORMAT(FROM_UNIXTIME(LD.PAYMENT_DATE),'%Y-%m-%d') AS PAYMENT_DATE, 
                                LD.DEBIT AS FINE_AMOUNT,
                                LD.ACADEMIC_YEAR,
                                LD.IS_PAID,
                                LB.ACCESS_NO AS BOOK_NO,
                                LB.DOC_TITLE AS BOOK_TITLE,
                                LB.STATUS AS BOOK_STATUS,
                                UA.NAME AS FIRST_NAME,
                                UA.USERNAME AS BOID,
                                SM.USER_TYPE_NAME AS MEMBER_TYPE,
                                DATE_FORMAT(FROM_UNIXTIME(LI.ISSUE_DATE),'%Y-%m-%d') AS ISSUE_DATE, 
                                DATE_FORMAT(FROM_UNIXTIME( LI.RETURN_DATE),'%Y-%m-%d') AS RETURN_DATE,
                                SS.STATUS_NAME AS STATUS
                            FROM
                                LI_ISSUE_DUES AS LD
                                    INNER JOIN
                                LI_BOOK_ISSUE_?AC_YEAR AS LI ON LI.ISSUE_ID = LD.ISSUE_ID
                                    AND LI.IS_DELETED != 1
                                    INNER JOIN
                                LI_BOOK_INFO AS LB ON LB.BOOK_ID = LI.BOOK_ID
                                    AND LB.IS_DELETED != 1
                                    INNER JOIN
                                USER_ACCOUNT AS UA ON UA.USER_ID = LI.USER_ID
                                    AND UA.USER_TYPE = LI.USER_TYPE
                                    AND UA.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_LI_STATUS AS SS ON SS.STATUS_ID = LB.LI_BOOK_STATUS
                                    AND SS.IS_DELETED != 1
                                    INNER JOIN
                                sup_user_type AS SM ON SM.USER_TYPE_ID = UA.USER_TYPE
                                    LEFT JOIN
                                LI_RACK_INFO AS LR ON LR.RACK_ID = LB.RACK_ID
                                    AND LR.IS_DELETED != 1
        
                            WHERE
		                            LD.ACADEMIC_YEAR = ?AC_YEAR
                                    AND LD.IS_PAID = 1
                                    AND LD.IS_DELETED != 1 
                                    AND (DATE_FORMAT(FROM_UNIXTIME(PAYMENT_DATE), '%Y-%m-%d') BETWEEN ?FROM_DATE AND ?TO_DATE)
                            GROUP BY LD.DUES_ID
                            ORDER BY DATE_FORMAT(FROM_UNIXTIME(PAYMENT_DATE), '%Y-%m-%d') ASC, LD.DUES_ID ASC;";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalFineNotPaidListByDate:
                    {
                        sQuery = @"SELECT 
                                LD.DUES_ID,
                                LD.REASON,
                                DATE_FORMAT(FROM_UNIXTIME(LD.PAYMENT_DATE),'%Y-%m-%d') AS PAYMENT_DATE, 
                                LD.DEBIT AS FINE_AMOUNT,
                                LD.ACADEMIC_YEAR,
                                LD.IS_PAID,
                                LB.ACCESS_NO AS BOOK_NO,
                                LB.DOC_TITLE AS BOOK_TITLE,
                                LB.STATUS AS BOOK_STATUS,
                                UA.NAME AS FIRST_NAME,
                                UA.USERNAME AS BOID,
                                SM.USER_TYPE_NAME AS MEMBER_TYPE,
                                DATE_FORMAT(FROM_UNIXTIME(LI.ISSUE_DATE),'%Y-%m-%d') AS ISSUE_DATE, 
                                DATE_FORMAT(FROM_UNIXTIME( LI.RETURN_DATE),'%Y-%m-%d') AS RETURN_DATE,
                                SS.STATUS_NAME AS STATUS
                            FROM
                                LI_ISSUE_DUES AS LD
                                    INNER JOIN
                                LI_BOOK_ISSUE_?AC_YEAR AS LI ON LI.ISSUE_ID = LD.ISSUE_ID
                                    AND LI.IS_DELETED != 1
                                    INNER JOIN
                                LI_BOOK_INFO AS LB ON LB.BOOK_ID = LI.BOOK_ID
                                    AND LB.IS_DELETED != 1
                                    INNER JOIN
                                USER_ACCOUNT AS UA ON UA.USER_ID = LI.USER_ID
                                    AND UA.USER_TYPE = LI.USER_TYPE
                                    AND UA.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_LI_STATUS AS SS ON SS.STATUS_ID = LB.LI_BOOK_STATUS
                                    AND SS.IS_DELETED != 1
                                    INNER JOIN
                                sup_user_type AS SM ON SM.USER_TYPE_ID = UA.USER_TYPE
                                    LEFT JOIN
                                LI_RACK_INFO AS LR ON LR.RACK_ID = LB.RACK_ID
                                    AND LR.IS_DELETED != 1
        
                            WHERE
		                            LD.ACADEMIC_YEAR = ?AC_YEAR
                                    AND LD.IS_PAID != 1
                                    AND LD.IS_DELETED != 1 
                                    AND DATE_FORMAT(FROM_UNIXTIME(PAYMENT_DATE),'%Y-%m-%d')=?PAYMENT_DATE
                            GROUP BY LD.DUES_ID
                            ORDER BY LD.DUES_ID;";
                        break;
                    }
                case LibrarySQLCommands.BindTotalBookDetails:
                    {
                        sQuery = @" SELECT 
                                        BOOK_NO,
                                        BOOK_ID,
                                        CONCAT(IFNULL(LB.BLOCK_NAME, ''),
                                                ' ',
                                                'BOW:',
                                                IFNULL(LR.BOW_NO, ''),
                                                ' ',
                                                'RACK:',
                                                IFNULL(LR.RACK_NO, ''),
                                                ' ',
                                                'RACK:',
                                                IFNULL(LR.SECTION_NO, ''),
                                                ' ',
                                                'SECTION:',
                                                IFNULL(LR.POSITION, '')) AS RACK_NO,
                                        LD.BOOK_ID,
                                        ACCESS_NO,
                                        CONCAT(IFNULL(SLC.CLASS_CODE, ''),
                                                ' - ',
                                                IFNULL(SLC.CLASS_NAME, '')) AS CLASS_NAME,
                                        CONCAT(IFNULL(SLS.SUBJECT_CODE, ''),
                                                ' - ',
                                                IFNULL(SLS.SUBJECT_NAME, '')) AS SUBJECT_NAME,
                                        L.LANGUAGE_NAME,
                                        GROUP_CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.PLACE, ''),
                                            ' ',
                                            IFNULL(LA.PUBLISHER, '')) AS AUTHOR,
                                        BOOK_TITLE,
                                        PARALLEL_TITLE,
                                        NOTES,
                                        CONCAT(IFNULL(LI.ILLUSTRATE_CODE, ''),
                                                IFNULL(LI.ILLUSTRATE, '')) AS ILLUSTRATE,
                                        SS.STATUS_NAME AS STATUS,
                                        PRICE,
                                        ISBN_NO,
                                        DETAILS,
                                        SERIES_CODE,
                                        RFID,
                                        IFNULL(IS_ARTICLE, 0) AS IS_ARTICLE,
                                        BOOK_PUPLISH_YEAR,
                                        IS_CIRCULATION
                                    FROM
                                        LI_BOOK_DETAILS AS LD
                                            LEFT JOIN
                                        SUP_LI_ILLUSTRATION AS LI ON LI.ILLUSTRATE_ID = LD.ILLUSTRATE
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = LD.LANGUAGE
                                            LEFT JOIN
                                        SUP_LI_STATUS AS SS ON SS.STATUS_ID = LD.STATUS
                                            AND SS.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON FIND_IN_SET(LA.AUTHOR_ID,LD.AUTHOR_ID)
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_RACK_INFO AS LR ON LR.RACK_ID = LD.RACK_ID
                                            AND LR.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_LI_CLASS AS SLC ON SLC.CLASS_ID = LD.CLASS_ID
                                            AND SLC.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_LI_SUBJECTS AS SLS ON SLS.LI_SUBJECT_ID = LD.SUBJECT_ID
                                            AND SLS.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_LI_BLOCK AS LB ON LB.BLOCK_ID = LR.BLOCK_ID
                                            AND LB.IS_DELETED != 1
                                    WHERE

                                        LD.IS_DELETED != 1
                                        AND LD.IS_ARTICLE!=1
                                        GROUP BY LD.BOOK_ID ORDER BY LD.BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.BindTotalBookDetailsBySubjectWise:
                    {
                        sQuery = @"   
                                 SELECT 
                                       
                                        
                                        CONCAT(IFNULL(LB.BLOCK_NAME, ''),
                                                ' ',
                                                'BOW:',
                                                IFNULL(LR.BOW_NO, ''),
                                                ' ',
                                                'RACK:',
                                                IFNULL(LR.RACK_NO, ''),
                                                ' ',
                                                'RACK:',
                                                IFNULL(LR.SECTION_NO, ''),
                                                ' ',
                                                'SECTION:',
                                                IFNULL(LR.POSITION, '')) AS RACK_NO,
                                        LD.BOOK_ID,
                                        LD.BOOK_NO,
                                        ACCESS_NO,
                                        CONCAT(IFNULL(SLC.CLASS_CODE, ''),
                                                ' - ',
                                                IFNULL(SLC.CLASS_NAME, '')) AS CLASS_NAME,
                                        CONCAT(IFNULL(SLS.SUBJECT_CODE, ''),
                                                ' - ',
                                                IFNULL(SLS.SUBJECT_NAME, '')) AS SUBJECT_NAME,
                                        L.LANGUAGE_NAME,
                                        GROUP_CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.PLACE, ''),
                                            ' ',
                                            IFNULL(LA.PUBLISHER, '')) AS AUTHOR,
                                        BOOK_TITLE,
                                        PARALLEL_TITLE,
                                        NOTES,
                                        CONCAT(IFNULL(LI.ILLUSTRATE_CODE, ''),
                                                IFNULL(LI.ILLUSTRATE, '')) AS ILLUSTRATE,
                                        SS.STATUS_NAME AS STATUS,
                                        PRICE,
                                        ISBN_NO,
                                        DETAILS,
                                        SERIES_CODE,
                                        RFID,
                                        IFNULL(IS_ARTICLE, 0) AS IS_ARTICLE,
                                        BOOK_PUPLISH_YEAR,
                                        IS_CIRCULATION
                                    FROM
                                        LI_BOOK_DETAILS AS LD
                                            LEFT JOIN
                                        SUP_LI_ILLUSTRATION AS LI ON LI.ILLUSTRATE_ID = LD.ILLUSTRATE
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = LD.LANGUAGE
                                            LEFT JOIN
                                        SUP_LI_STATUS AS SS ON SS.STATUS_ID = LD.STATUS
                                            AND SS.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON FIND_IN_SET(LA.AUTHOR_ID,LD.AUTHOR_ID)
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_RACK_INFO AS LR ON LR.RACK_ID = LD.RACK_ID
                                            AND LR.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_LI_CLASS AS SLC ON SLC.CLASS_ID = LD.CLASS_ID
                                            AND SLC.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_LI_SUBJECTS AS SLS ON SLS.LI_SUBJECT_ID = LD.SUBJECT_ID
                                            AND SLS.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_LI_BLOCK AS LB ON LB.BLOCK_ID = LR.BLOCK_ID
                                            AND LB.IS_DELETED != 1
                                    WHERE

                                        LD.IS_DELETED != 1
                                        AND LD.IS_ARTICLE!=1
                                        AND  LD.BOOK_NO LIKE '%?BOOK_NO%' 
                                        GROUP BY LD.BOOK_ID ORDER BY LD.BOOK_ID; ";
                        break;
                    }
                case LibrarySQLCommands.FetchFrequentVisitorsByStartandEndDate:
                    {
                        sQuery = @"SELECT 
                                        U.USERNAME,
                                        U.NAME AS FIRST_NAME,
                                        DATE_FORMAT(li.CHECKIN_DATE, '%Y-%m-%d') as CHECKIN_DATE,
                                        TIME_FORMAT(DATE_FORMAT(li.CHECKIN_DATE, '%H:%i:%s'),'%r') as CHECKIN_TIME,
                                        DATE_FORMAT(li.CHECKOUT_DATE, '%Y-%m-%d') as CHECKOUT_DATE,
                                        TIME_FORMAT(DATE_FORMAT(li.CHECKOUT_DATE, '%H:%i:%s'),'%r')as CHECKOUT_TIME,
                                        T.USER_TYPE_NAME,
                                        U.PHOTO,
                                        COUNT(U.USERNAME) AS 'COUNT'
                                    FROM
                                        LI_CHECKIN_INFO_?AC_YEAR AS LI
		                                    INNER JOIN
	                                    USER_ACCOUNT AS U ON U.USER_ID=LI.USER_ID
		                                    AND U.USER_TYPE=LI.USER_TYPE
                                            INNER JOIN
	                                    SUP_USER_TYPE AS T ON T.USER_TYPE_ID=U.USER_TYPE
                                    WHERE
                                        DATE_FORMAT(li.CHECKIN_DATE, '%Y-%m-%d') BETWEEN ?START_DATE AND ?END_DATE
                                    GROUP BY LI.USER_ID,LI.USER_TYPE HAVING COUNT(U.USERNAME)>5
                                    ORDER BY COUNT(U.USERNAME) DESC,USERNAME;";
                        break;
                    }
                case LibrarySQLCommands.FetchFrequentIssueByStartDateandEndDate:
                    {
                        sQuery = @"  
                              SELECT 
                                    LI.ISSUE_ID,
                                    LI.USER_ID,
                                    CONCAT(IFNULL(LD.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LD.LAST_NAME, '')) AS FIRST_NAME,
                                    CONCAT(IFNULL(SM.CATEGORY_CODE, ''),
                                            '-',
                                            IFNULL(SM.CATEGORY_NAME, '')) AS MEMBER_TYPE,
                                    LD.BOID,
                                    LD.PHOTO,
                                    LD.RFID,
                                    LB.BOOK_NO,
                                    CONCAT(IFNULL(SLC.CLASS_CODE, ''),
									' - ',
									IFNULL(SLC.CLASS_NAME, '')) AS CLASS_NAME,
									CONCAT(IFNULL(SLS.SUBJECT_CODE, ''),
									' - ',
									IFNULL(SLS.SUBJECT_NAME, '')) AS SUBJECT_NAME,
                                    CONCAT(IFNULL(LBB.BLOCK_NAME, ''),
                                            ' ',
                                            'BOW:',
                                            IFNULL(LR.BOW_NO, ''),
                                            ' ',
                                            'RACK:',
                                            IFNULL(LR.RACK_NO, ''),
                                            ' ',
                                            'RACK:',
                                            IFNULL(LR.SECTION_NO, ''),
                                            ' ',
                                            'SECTION:',
                                            IFNULL(LR.POSITION, '')) AS RACK_NO,
                                    GROUP_CONCAT(IFNULL(LA.FIRST_NAME, ''),' ',IFNULL(LA.LAST_NAME, '')) AS AUTHOR,
                                    LB.BOOK_TITLE,
                                    LB.STATUS AS BOOK_STATUS,
                                    LI.ISSUE_DATE,
                                    LI.RETURN_DATE
                                FROM
                                    LI_BOOK_ISSUE AS LI
                                        INNER JOIN
                                    LI_USER_DETAILS AS LD ON LD.USER_ID = LI.USER_ID
                                        AND LD.IS_DELETED != 1
                                        INNER JOIN
                                    LI_BOOK_DETAILS AS LB ON LB.BOOK_ID = LI.BOOK_ID
                                        AND LB.IS_DELETED != 1
							
                                     LEFT JOIN 
                                     SUP_LI_MEMBER_CATEGORY AS SM ON SM.CATEGORY_ID=LD.MEMBER_TYPE
                                     AND SM.IS_DELETED!=1
                                     
									 INNER JOIN
                                    LI_AUTHOR_INFO AS LA ON FIND_IN_SET( LA.AUTHOR_ID,LB.AUTHOR_ID)
                                        AND LA.IS_DELETED != 1
                                       INNER JOIN
                                    LI_RACK_INFO AS LR ON LR.RACK_ID = LB.RACK_ID
                                        AND LR.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_LI_BLOCK AS LBB ON LBB.BLOCK_ID = LR.BLOCK_ID
                                        AND LBB.IS_DELETED != 1
                                        INNER JOIN
										SUP_LI_CLASS AS SLC ON SLC.CLASS_ID = LB.CLASS_ID
										AND SLC.IS_DELETED != 1
										INNER JOIN
										SUP_LI_SUBJECTS AS SLS ON SLS.LI_SUBJECT_ID = LB.SUBJECT_ID
										AND LB.IS_DELETED != 1
                                WHERE
                                    LI.ISSUE_DATE BETWEEN DATE_FORMAT(?START_DATE,'%Y-%m-%d') AND DATE_FORMAT(?END_DATE,'%Y-%m-%d')
                                        AND LI.IS_DELETED != 1
                                        AND LI.STATUS=1
                                        GROUP BY LI.ISSUE_ID
                                        ORDER BY LI.ISSUE_ID;";
                        break;
                    }
                #endregion

                #region Year wise Report

                case LibrarySQLCommands.FetchTotalIssuedBooksByYear:
                    {
                        sQuery = @"SELECT 
                                        ISSUE_ID,
                                        BI.USER_ID,
                                        BI.BOOK_ID,
                                        B.BOOK_NO,
                                        UPPER(NAME) AS FIRST_NAME,
                                        USERNAME,
                                        UT.USER_TYPE_NAME,
                                        B.ACCESS_NO,
                                        B.DOC_TITLE AS BOOK_TITLE,
                                        CONCAT(IFNULL(B.PERSONAL_AUTHOR1, ''),
                                            ' ',
                                            IFNULL(B.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                           
                                        concat(IFNULL(SP.FIRST_NAME,''),' - ',IFNULL(SP.STAFF_CODE,'')) AS ISSUE_BY,
  	                                    concat(IFNULL(STF.FIRST_NAME,''),' - ',IFNULL(STF.STAFF_CODE,'')) AS RECEIVED_BY,
                                        DATE_FORMAT(FROM_UNIXTIME(BI.ISSUE_DATE), '%Y-%m-%d') AS ISSUE_DATE,
                                        DATE_FORMAT(FROM_UNIXTIME(BI.RETURN_DATE), '%Y-%m-%d') AS RETURN_DATE,
                                        DATE_FORMAT(FROM_UNIXTIME(BI.ACTUAL_RETURNED_DATE), '%Y-%m-%d') AS ACTUAL_RETURNED_DATE
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS BI
                                            INNER JOIN
                                        LI_BOOK_INFO AS B ON B.BOOK_ID = BI.BOOK_ID
                                            AND B.IS_DELETED != 1
                                            INNER JOIN
	                                    USER_ACCOUNT AS UA ON UA.USER_ID=BI.USER_ID
		                                    AND UA.USER_TYPE=BI.USER_TYPE
                                            AND UA.IS_DELETED!=1
                                            LEFT JOIN
	                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID=BI.ISSUE_BY
		                                    AND SP.IS_DELETED!=1
                                            left JOIN
	                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID=BI.RECEIVED_BY
		                                    AND STF.IS_DELETED!=1
                                            LEFT JOIN
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=BI.USER_TYPE
                                    WHERE
                                        BI.IS_DELETED != 1 AND DATE_FORMAT(FROM_UNIXTIME(BI.ISSUE_DATE), '%Y-%m-%d') = ?ISSUE_DATE;";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalReturnedBooksByYear:
                    {
                        sQuery = @"SELECT 
                                            ISSUE_ID,
                                            BI.USER_ID,
                                            BI.BOOK_ID,
                                            B.BOOK_NO,
                                            UPPER(NAME) AS FIRST_NAME,
                                            USERNAME,
                                             UT.USER_TYPE_NAME,
                                            B.ACCESS_NO,
                                            B.DOC_TITLE AS BOOK_TITLE,
                                            CONCAT(IFNULL(B.PERSONAL_AUTHOR1, ''),
                                            ' ',
                                            IFNULL(B.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                            concat(IFNULL(SP.FIRST_NAME,''),' - ',IFNULL(SP.STAFF_CODE,'')) AS ISSUE_BY,
  	                                        concat(IFNULL(STF.FIRST_NAME,''),' - ',IFNULL(STF.STAFF_CODE,'')) AS RECEIVED_BY,
                                            DATE_FORMAT(FROM_UNIXTIME(BI.ISSUE_DATE), '%Y-%m-%d') AS ISSUE_DATE,
                                            DATE_FORMAT(FROM_UNIXTIME(BI.RETURN_DATE), '%Y-%m-%d') AS RETURN_DATE,
                                            DATE_FORMAT(FROM_UNIXTIME(BI.ACTUAL_RETURNED_DATE), '%Y-%m-%d') AS ACTUAL_RETURNED_DATE
                                        FROM
                                            LI_BOOK_ISSUE_?AC_YEAR AS BI
                                                INNER JOIN
                                            LI_BOOK_INFO AS B ON B.BOOK_ID = BI.BOOK_ID
                                                AND B.IS_DELETED != 1
                                                INNER JOIN
	                                        USER_ACCOUNT AS UA ON UA.USER_ID=BI.USER_ID
		                                        AND UA.USER_TYPE=BI.USER_TYPE
                                                AND UA.IS_DELETED!=1
                                                LEFT JOIN
	                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID=BI.ISSUE_BY
		                                        AND SP.IS_DELETED!=1
                                                left JOIN
	                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID=BI.RECEIVED_BY
		                                        AND STF.IS_DELETED!=1
                                                
                                                 LEFT JOIN
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=BI.USER_TYPE
                                        WHERE
                                            BI.IS_DELETED != 1 
                                            AND DATE_FORMAT(FROM_UNIXTIME(BI.ACTUAL_RETURNED_DATE), '%Y-%m-%d') =?ACTUAL_RETURNED_DATE;";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalReservedBooksByYear:
                    {
                        sQuery = @"SELECT 
                                        RESERVE_ID,
                                        R.USER_ID,
                                        u.PHOTO,
                                        R.BOOK_ID,
                                        UPPER(NAME) AS FIRST_NAME,
                                        USERNAME,
                                        B.ACCESS_NO,
                                        B.DOC_TITLE AS BOOK_TITLE,
                                        IS_CANCELLED,
                                        IS_USER_CANCELLED,
                                        IS_SYSTEM_CANCELLED,
                                        IS_CSMS_SENT AS 'CANCELLED_SMS',
                                        IS_ASMS_SENT AS 'APPROVED_SMS',
                                        IS_SCSMS_SENT AS 'SYSTEM_CANCELLED_SMS',
                                        DATE_FORMAT(FROM_UNIXTIME(R.ENTRY_DATE), '%Y-%m-%d') AS ENTRY_DATE,
                                        DATE_FORMAT(FROM_UNIXTIME(R.APPROVED_DATE),
                                                '%Y-%m-%d') AS APPROVED_DATE,
                                        DATE_FORMAT(FROM_UNIXTIME(R.CANCELLED_DATE),
                                                '%Y-%m-%d') AS CANCELLED_DATE
                                    FROM
                                        LI_BOOK_RESERVE AS R
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = R.SEMESTER_TYPE
                                            AND ST.IS_ACTIVE = 1
                                            INNER JOIN
                                        USER_ACCOUNT AS U ON U.USER_ID = R.USER_ID
                                            AND U.USER_TYPE = R.USER_TYPE
                                            INNER JOIN
                                        LI_BOOK_INFO AS B ON B.BOOK_ID = R.BOOK_ID
                                            AND B.IS_DELETED != 1
                                    WHERE
                                        R.IS_DELETED != 1    
                                             AND DATE_FORMAT(FROM_UNIXTIME(R.ENTRY_DATE), '%Y-%m-%d')=?ENTRY_DATE                                        
                                            AND R.ACADEMIC_YEAR = ?AC_YEAR 
                                            ORDER BY U.USER_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalFinePaidListByYear:
                    {
                        sQuery = @"SELECT 
                                LD.DUES_ID,
                                LD.REASON,
                                DATE_FORMAT(FROM_UNIXTIME(LD.PAYMENT_DATE),'%Y-%m-%d') AS PAYMENT_DATE, 
                                LD.DEBIT AS FINE_AMOUNT,
                                LD.ACADEMIC_YEAR,
                                LD.IS_PAID,
                                LB.ACCESS_NO AS BOOK_NO,
                                LB.DOC_TITLE AS BOOK_TITLE,
                                LB.STATUS AS BOOK_STATUS,
                                UA.NAME AS FIRST_NAME,
                                UA.USERNAME AS BOID,
                                SM.USER_TYPE_NAME AS MEMBER_TYPE,
                                DATE_FORMAT(FROM_UNIXTIME(LI.ISSUE_DATE),'%Y-%m-%d') AS ISSUE_DATE, 
                                DATE_FORMAT(FROM_UNIXTIME( LI.RETURN_DATE),'%Y-%m-%d') AS RETURN_DATE,
                                SS.STATUS_NAME AS STATUS
                            FROM
                                LI_ISSUE_DUES AS LD
                                    INNER JOIN
                                LI_BOOK_ISSUE_?AC_YEAR AS LI ON LI.ISSUE_ID = LD.ISSUE_ID
                                    AND LI.IS_DELETED != 1
                                    INNER JOIN
                                LI_BOOK_INFO AS LB ON LB.BOOK_ID = LI.BOOK_ID
                                    AND LB.IS_DELETED != 1
                                    INNER JOIN
                                USER_ACCOUNT AS UA ON UA.USER_ID = LI.USER_ID
                                    AND UA.USER_TYPE = LI.USER_TYPE
                                    AND UA.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_LI_STATUS AS SS ON SS.STATUS_ID = LB.LI_BOOK_STATUS
                                    AND SS.IS_DELETED != 1
                                    INNER JOIN
                                sup_user_type AS SM ON SM.USER_TYPE_ID = UA.USER_TYPE
                                    LEFT JOIN
                                LI_RACK_INFO AS LR ON LR.RACK_ID = LB.RACK_ID
                                    AND LR.IS_DELETED != 1
        
                            WHERE
		                            LD.ACADEMIC_YEAR = ?AC_YEAR
                                    AND LD.IS_PAID = 1
                                    AND LD.IS_DELETED != 1                                    
                                    AND DATE_FORMAT(FROM_UNIXTIME(PAYMENT_DATE),'%Y-%m-%d')=?PAYMENT_DATE
                                    GROUP BY LD.DUES_ID
                                    ORDER BY LD.DUES_ID;";
                        break;
                    }
                case LibrarySQLCommands.BindTotalVisitorsByYear:
                    {
                        sQuery = @"SELECT 
                                        U.USERNAME,
                                        U.NAME AS FIRST_NAME,
                                        DATE_FORMAT(li.CHECKIN_DATE, '%Y-%m-%d') as CHECKIN_DATE,
                                        TIME_FORMAT(DATE_FORMAT(li.CHECKIN_DATE, '%H:%i:%s'),'%r') as CHECKIN_TIME,
                                        DATE_FORMAT(li.CHECKOUT_DATE, '%Y-%m-%d') as CHECKOUT_DATE,
                                        TIME_FORMAT(DATE_FORMAT(li.CHECKOUT_DATE, '%H:%i:%s'),'%r')as CHECKOUT_TIME,
                                        T.USER_TYPE_NAME,
                                        COUNT(U.USERNAME) AS 'COUNT'
                                    FROM
                                        LI_CHECKIN_INFO_?AC_YEAR AS LI
		                                    INNER JOIN
	                                    USER_ACCOUNT AS U ON U.USER_ID=LI.USER_ID
		                                    AND U.USER_TYPE=LI.USER_TYPE
                                            INNER JOIN
	                                    SUP_USER_TYPE AS T ON T.USER_TYPE_ID=U.USER_TYPE
                                    WHERE
                                        DATE_FORMAT(li.CHECKIN_DATE, '%Y-%m-%d') = ?CHECKIN_DATE and 
                                        LI.IS_DELETED!=1
                                        group by LI.USER_ID,LI.USER_TYPE;";
                        break;
                    }
                #endregion

                #region Book Issue
                case LibrarySQLCommands.ListBookIssue:
                    {
                        //sQuery = @"SELECT 
                        //             U.USER_ID,
                        //                DATE_FORMAT(BI.ISSUE_DATE,'%d/%m/%Y %r') AS ISSUE_DATE,
                        //                DATE_FORMAT(U.LAST_LOGIN,'%d/%m/%Y %r') AS LAST_LOGIN,
                        //                CONCAT(IFNULL(U.FIRST_NAME,''),'  ',IFNULL(U.LAST_NAME,'')) AS FIRST_NAME,
                        //                U.EMAIL,
                        //                U.MOBILE,
                        //                U.PHOTO,
                        //                U.BOID,
                        //                D.DEGREE,
                        //                M.CATEGORY_NAME,
                        //                B.ACCESS_NO,
                        //                B.CLASS_NO,
                        //                B.BOOK_ID,
                        //                B.BOOK_NO,
                        //                B.ISBN_NO,
                        //                L.LANGUAGE_NAME,
                        //                UPPER(CONCAT(IFNULL(B.BOOK_TITLE,''),'  ',IFNULL(B.PARALLEL_TITLE,''))) AS BOOK_TITLE,
                        //                R.FINE_AMOUNT,
                        //                R.LOAN_PERIOD,
                        //                R.RULE_ID,
                        //                DATEDIFF(CURDATE(),BI.ISSUE_DATE) AS DATE_COUNT,
                        //                DATE_FORMAT(DATE_ADD(BI.ISSUE_DATE,INTERVAL R.LOAN_PERIOD DAY),'%d/%m/%Y') as RETURN_DATE,
                        //                SUM(IF(DATEDIFF(CURDATE(),BI.ISSUE_DATE)>R.LOAN_PERIOD,DATEDIFF(CURDATE(),BI.ISSUE_DATE)*R.FINE_AMOUNT,'0')) AS DUES
                        //            FROM
                        //                LI_BOOK_ISSUE AS BI
                        //              INNER JOIN
                        //             LI_USER_DETAILS AS U ON U.USER_ID=BI.USER_ID
                        //              INNER JOIN
                        //             LI_BOOK_DETAILS AS B ON B.BOOK_ID=BI.BOOK_ID
                        //              INNER JOIN
                        //             LI_RULES AS R ON R.MEM_CATEGORY=U.MEMBER_TYPE
                        //              AND R.IS_DELETED!=1
                        //              INNER JOIN
                        //             SUP_DEGREE AS D ON D.DEGREE_ID=U.DEGREE
                        //              INNER JOIN
                        //             SUP_LI_MEMBER_CATEGORY AS M ON M.CATEGORY_ID=U.MEMBER_TYPE
                        //              INNER JOIN
                        //             SUP_LANGUAGE AS L ON L.LANGUAGE_ID=B.LANGUAGE
                        //            WHERE BI.STATUS=1 GROUP BY BI.ISSUE_ID;";

                        sQuery = @"SELECT 
                                    U.USER_ID,
                                    BI.ISSUE_ID,
                                    DATE_FORMAT(BI.ISSUE_DATE, '%d/%m/%Y %r') AS ISSUE_DATE,
                                    DATE_FORMAT(BI.ISSUE_DATE, '%Y-%m-%d') AS DAY_ORDER_DATE,
                                    DATE_FORMAT(U.LAST_LOGIN, '%d/%m/%Y %r') AS LAST_LOGIN,
                                    CONCAT(IFNULL(U.FIRST_NAME, ''), '  ', IFNULL(U.LAST_NAME, '')) AS FIRST_NAME,
                                    U.EMAIL,
                                    U.MOBILE,
                                    U.PHOTO,
                                    U.BOID,
                                    D.DEGREE,
                                    M.CATEGORY_NAME,
                                    B.ACCESS_NO,
                                    B.CLASS_NO,
                                    B.BOOK_ID,
                                    B.BOOK_NO,
                                    B.ISBN_NO,
                                    L.LANGUAGE_NAME AS LANGUAGE,
                                    UPPER(CONCAT(IFNULL(B.DOC_TITLE, ''), '  ', IFNULL(B.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
                                    R.FINE_AMOUNT,
                                    R.LOAN_PERIOD,
                                    R.RULE_ID,
                                    CAL.DAY_ID,
                                    DATEDIFF(CURDATE(), BI.ISSUE_DATE) AS DATE_COUNT,
                                    CURDATE() AS 'CURRENT_DATE'
                                FROM
                                    LI_BOOK_ISSUE AS BI
                                INNER JOIN LI_USER_DETAILS AS U ON U.USER_ID = BI.USER_ID
                                INNER JOIN LI_BOOK_DETAILS AS B ON B.BOOK_ID = BI.BOOK_ID
                                INNER JOIN LI_RULES AS R ON R.MEM_CATEGORY = U.MEMBER_TYPE AND R.IS_DELETED != 1
                                LEFT JOIN SUP_DEGREE AS D ON D.DEGREE_ID = U.DEGREE
                                INNER JOIN SUP_LI_MEMBER_CATEGORY AS M ON M.CATEGORY_ID = U.MEMBER_TYPE
                                LEFT JOIN SUP_LANGUAGE AS L ON L.LANGUAGE_ID = B.LANGUAGE
                                INNER JOIN CALENDER AS CAL ON CAL.DAY_ORDER_DATE=DATE_FORMAT(BI.ISSUE_DATE,'%Y-%m-%d')
                                WHERE
                                    BI.STATUS = 1;";
                        break;
                    }
                case LibrarySQLCommands.ListBookIssueForAddDue:
                    {
                        sQuery = @"SELECT 
                                        LI.ISSUE_ID,
                                        CAL.DAY_ID,
                                        LR.NO_OF_DAYS,
                                        IF(BR.RENEW_ID != 0, '1', '0') AS IS_RENEWED,
                                        LR.NO_OF_RENEW_DAYS,
                                        LR.PER_DAY_DUE_COST,
                                        LR.REVISED_DUE_DATES,
                                        LR.REVISED_DUE_COST,
                                        IF(LD.CREDIT != 0,
                                            LD.CREDIT,
                                            0) - IF(LD.DEBIT != 0,
                                            LD.DEBIT,
                                            0) AS FINE_AMOUNT
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS LI
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER_DATE = DATE_FORMAT(FROM_UNIXTIME(LI.ISSUE_DATE), '%Y-%m-%d')
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = LI.USER_ID
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SPI.PROGRAM_ID
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USER_ID = SPI.STUDENT_ID
                                            AND UA.IS_DELETED != 1
                                            AND UA.USER_TYPE = LI.USER_TYPE
                                            INNER JOIN
                                        LI_RULES AS LR ON LR.USER_TYPE = UA.USER_TYPE
                                            AND LR.ROLE =LI.ROLE
                                            AND LR.PROGRAMME_LEVEL = PG.APPTYPE_ID
                                            AND LR.ACADEMIC_YEAR=?AC_YEAR
                                            LEFT JOIN
                                        LI_BOOK_RENEW AS BR ON BR.ISSUE_ID = LI.ISSUE_ID
                                            AND BR.IS_DELETED != 1
                                            AND BR.ACADEMIC_YEAR =?AC_YEAR
                                            AND BR.IS_CONFIRMED = '1'
                                            LEFT JOIN
                                        LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                            AND LD.IS_DELETED != 1
                                            AND LD.ACADEMIC_YEAR=?AC_YEAR
                                    WHERE
                                        LI.IS_DELETED != 1
                                            AND LI.IS_RETURNED != 1
											 AND LI.IS_MISSED!=1
                                            AND LI.IS_SMS_SENT = 1 
                                    UNION SELECT 
                                        LI.ISSUE_ID,
                                        CAL.DAY_ID,
                                        LR.NO_OF_DAYS,
                                        IF(BR.RENEW_ID != 0, '1', '0') AS IS_RENEWED,
                                        LR.NO_OF_RENEW_DAYS,
                                        LR.PER_DAY_DUE_COST,
                                        LR.REVISED_DUE_DATES,
                                        LR.REVISED_DUE_COST,
                                        IF(LD.CREDIT != 0,
                                            LD.CREDIT,
                                            0) - IF(LD.DEBIT != 0,
                                            LD.DEBIT,
                                            0) AS FINE_AMOUNT
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS LI
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER_DATE = DATE_FORMAT(FROM_UNIXTIME(LI.ISSUE_DATE), '%Y-%m-%d')
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = LI.USER_ID
                                            AND STF.IS_DELETED != 1
                                            AND STF.IS_LEFT != 1
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                            AND UA.IS_DELETED != 1
                                            AND UA.USER_TYPE = LI.USER_TYPE
                                            INNER JOIN
                                        LI_RULES AS LR ON LR.USER_TYPE = UA.USER_TYPE
                                            AND LR.ROLE = LI.ROLE
                                            AND LR.ACADEMIC_YEAR=?AC_YEAR
                                            LEFT JOIN
                                        LI_BOOK_RENEW AS BR ON BR.ISSUE_ID = LI.ISSUE_ID
                                            AND BR.IS_DELETED != 1
                                            AND BR.ACADEMIC_YEAR =?AC_YEAR
                                            AND BR.IS_CONFIRMED = '1'
                                            LEFT JOIN
                                        LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                            AND LD.IS_DELETED != 1
                                            AND LD.ACADEMIC_YEAR=?AC_YEAR
                                    WHERE
                                        LI.IS_DELETED != 1
                                            AND LI.IS_RETURNED != 1
											 AND LI.IS_MISSED!=1
                                            AND LI.IS_SMS_SENT = 1 
                                    UNION SELECT 
                                        LI.ISSUE_ID,
                                        CAL.DAY_ID,
                                        LR.NO_OF_DAYS,
                                        IF(BR.RENEW_ID != 0, '1', '0') AS IS_RENEWED,
                                        LR.NO_OF_RENEW_DAYS,
                                        LR.PER_DAY_DUE_COST,
                                        LR.REVISED_DUE_DATES,
                                        LR.REVISED_DUE_COST,
                                        IF(LD.CREDIT != 0,
                                            LD.CREDIT,
                                            0) - IF(LD.DEBIT != 0,
                                            LD.DEBIT,
                                            0) AS FINE_AMOUNT
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS LI
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER_DATE = DATE_FORMAT(FROM_UNIXTIME(LI.ISSUE_DATE), '%Y-%m-%d')
                                            INNER JOIN
                                        LI_EXTERNAL_USER_INFO AS UI ON UI.USER_ID = LI.USER_ID
                                            AND UI.USER_TYPE = LI.USER_TYPE
                                            INNER JOIN
                                        LI_RULES AS LR ON LR.USER_TYPE = UI.USER_TYPE
                                        AND LR.ACADEMIC_YEAR=?AC_YEAR
                                        AND LR.IS_DELETED!=1
                                            LEFT JOIN
                                        LI_BOOK_RENEW AS BR ON BR.ISSUE_ID = LI.ISSUE_ID
                                            AND BR.IS_DELETED != 1
                                            AND BR.ACADEMIC_YEAR =?AC_YEAR
                                            AND BR.IS_CONFIRMED = '1'
                                            LEFT JOIN
                                        LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                            AND LD.IS_DELETED != 1
                                            AND LD.ACADEMIC_YEAR=?AC_YEAR
                                    WHERE
                                        LI.IS_DELETED != 1
                                            AND LI.IS_RETURNED != 1
                                            AND LI.IS_MISSED!=1
                                            AND LI.IS_SMS_SENT = 1;";
                        break;
                    }
                case LibrarySQLCommands.ListBookIssueForSendSms:
                    {
                        sQuery = @"SELECT 
                                        LI.ISSUE_ID,
                                        CAL.DAY_ID,
                                         IF(LR.NO_OF_DAYS != 0, LR.NO_OF_DAYS, '0') AS NO_OF_DAYS,
                                        IF(BR.RENEW_ID != 0, '1', '0') AS IS_RENEWED,
                                         IF(LR.NO_OF_RENEW_DAYS != 0, LR.NO_OF_RENEW_DAYS, '0') AS NO_OF_RENEW_DAYS
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS LI
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER_DATE = DATE_FORMAT(FROM_UNIXTIME(LI.ISSUE_DATE), '%Y-%m-%d')
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = LI.USER_ID
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SPI.PROGRAM_ID
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USER_ID = SPI.STUDENT_ID
                                            AND UA.IS_DELETED != 1
                                            AND UA.USER_TYPE = LI.USER_TYPE
                                            INNER JOIN
                                        LI_RULES AS LR ON LR.USER_TYPE = UA.USER_TYPE
                                            AND LR.ROLE =LI.ROLE
                                            AND LR.PROGRAMME_LEVEL = PG.APPTYPE_ID
                                            AND LR.ACADEMIC_YEAR=?AC_YEAR
                                            LEFT JOIN
                                        LI_BOOK_RENEW AS BR ON BR.ISSUE_ID = LI.ISSUE_ID
                                            AND BR.IS_DELETED != 1
                                            AND BR.ACADEMIC_YEAR =?AC_YEAR
                                            AND BR.IS_CONFIRMED = '1'
                                    WHERE
                                        LI.IS_DELETED != 1
                                            AND LI.IS_RETURNED != 1
                                            AND LI.IS_DAMAGED!=1
                                            AND LI.IS_SMS_SENT != 1 
                                    UNION SELECT 
                                        LI.ISSUE_ID,
                                        CAL.DAY_ID,
                                         IF(LR.NO_OF_DAYS != 0, LR.NO_OF_DAYS, '0') AS NO_OF_DAYS,
                                        IF(BR.RENEW_ID != 0, '1', '0') AS IS_RENEWED,
                                         IF(LR.NO_OF_RENEW_DAYS != 0, LR.NO_OF_RENEW_DAYS, '0') AS NO_OF_RENEW_DAYS
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS LI
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER_DATE = DATE_FORMAT(FROM_UNIXTIME(LI.ISSUE_DATE), '%Y-%m-%d')
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = LI.USER_ID
                                            AND STF.IS_DELETED != 1
                                            AND STF.IS_LEFT != 1
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                            AND UA.IS_DELETED != 1
                                            AND UA.USER_TYPE = LI.USER_TYPE
                                            INNER JOIN
                                        LI_RULES AS LR ON LR.USER_TYPE = UA.USER_TYPE
                                            AND LR.ROLE = LI.ROLE
                                            AND LR.ACADEMIC_YEAR=?AC_YEAR
                                            LEFT JOIN
                                        LI_BOOK_RENEW AS BR ON BR.ISSUE_ID = LI.ISSUE_ID
                                            AND BR.IS_DELETED != 1
                                            AND BR.ACADEMIC_YEAR =?AC_YEAR
                                            AND BR.IS_CONFIRMED = '1'
                                    WHERE
                                        LI.IS_DELETED != 1
                                            AND LI.IS_RETURNED != 1
                                             AND LI.IS_DAMAGED!=1
                                            AND LI.IS_SMS_SENT != 1 
                                    UNION SELECT 
                                        LI.ISSUE_ID,
                                        CAL.DAY_ID,
                                         IF(LR.NO_OF_DAYS != 0, LR.NO_OF_DAYS, '0') AS NO_OF_DAYS,
                                        IF(BR.RENEW_ID != 0, '1', '0') AS IS_RENEWED,
                                         IF(LR.NO_OF_RENEW_DAYS != 0, LR.NO_OF_RENEW_DAYS, '0') AS NO_OF_RENEW_DAYS
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS LI
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER_DATE = DATE_FORMAT(FROM_UNIXTIME(LI.ISSUE_DATE), '%Y-%m-%d')
                                            INNER JOIN
                                        LI_EXTERNAL_USER_INFO AS UI ON UI.USER_ID = LI.USER_ID
                                            AND UI.USER_TYPE = LI.USER_TYPE
                                            INNER JOIN
                                        LI_RULES AS LR ON LR.USER_TYPE = UI.USER_TYPE
                                            AND LR.ACADEMIC_YEAR=?AC_YEAR
                                            LEFT JOIN
                                        LI_BOOK_RENEW AS BR ON BR.ISSUE_ID = LI.ISSUE_ID
                                            AND BR.IS_DELETED != 1
                                            AND BR.ACADEMIC_YEAR =?AC_YEAR
                                            AND BR.IS_CONFIRMED = '1'
                                    WHERE
                                        LI.IS_DELETED != 1
                                            AND LI.IS_RETURNED != 1
                                             AND LI.IS_DAMAGED!=1
                                            AND LI.IS_SMS_SENT != 1;";
                        break;
                    }
                case LibrarySQLCommands.UpdateLiBookIssueForSmsSend:
                    {
                        sQuery = @"UPDATE
                                    LI_BOOK_ISSUE_?AC_YEAR
                                    SET
                                    IS_SMS_SENT=1
                                    WHERE 
                                    ISSUE_ID IN(?ISSUE_ID);";
                        break;
                    }
                case LibrarySQLCommands.FetchBookIssueDetailsById:
                    {
                        sQuery = @"SELECT 
                                        BI.ISSUE_ID,
                                        U.USER_ID,
                                        DATE_FORMAT(BI.ISSUE_DATE, '%d/%m/%Y %r') AS ISSUE_DATE,
                                        DATE_FORMAT(U.LAST_LOGIN, '%d/%m/%Y %r') AS LAST_LOGIN,
                                        CONCAT(IFNULL(U.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(U.LAST_NAME, '')) AS FIRST_NAME,
                                        U.EMAIL,
                                        U.MOBILE,
                                        U.PHOTO,
                                        U.BOID,
                                       
                                        M.CATEGORY_NAME,
                                        B.ACCESS_NO,
                                        SLS.CLASS_NO,
                                        B.BOOK_NO,
                                        B.BOOK_ID,
                                        B.ISBN_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE, ''),
                                                        '  ',
                                                        IFNULL(B.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
                                        
                                        R.LOAN_PERIOD,
                                        R.RULE_ID,
                                        IFNULL(LD.FINE_AMOUNT,R.FINE_AMOUNT) AS FINE_AMOUNT
                                    FROM
                                        LI_BOOK_ISSUE AS BI
                                            INNER JOIN
                                        LI_USER_DETAILS AS U ON U.USER_ID = BI.USER_ID
                                            INNER JOIN
                                        LI_BOOK_DETAILS AS B ON B.BOOK_ID = BI.BOOK_ID
                                            INNER JOIN
                                        LI_RULES AS R ON R.MEM_CATEGORY = U.MEMBER_TYPE
                                            AND R.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_LI_MEMBER_CATEGORY AS M ON M.CATEGORY_ID = U.MEMBER_TYPE
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = B.LANGUAGE
                                            LEFT JOIN
                                        LI_DUES AS LD ON LD.USER_ID = BI.USER_ID
                                            AND LD.BOOK_ID = BI.BOOK_ID
                                            AND LD.IS_PAID != 1
                                            
                                            INNER JOIN
                                            SUP_LI_CLASS_NO AS SLS ON SLS.CLASS_NO_ID=B.CLASS_NO
                                            AND SLS.IS_DELETED!=1
                                    WHERE
                                          BI.USER_ID =?USER_ID
                                          AND BI.IS_DELETED!=1
                                          AND BI.STATUS!=2;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookIssueDetailsByUserRfid:
                    {
                        sQuery = @"SELECT 
                                        BI.ISSUE_ID,
                                        U.USER_ID,
                                        DATE_FORMAT(BI.ISSUE_DATE, '%d/%m/%Y %r') AS ISSUE_DATE,
                                        DATE_FORMAT(U.LAST_LOGIN, '%d/%m/%Y %r') AS LAST_LOGIN,
                                        CONCAT(IFNULL(U.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(U.LAST_NAME, '')) AS FIRST_NAME,
                                        U.EMAIL,
                                        U.MOBILE,
                                        U.PHOTO,
                                        U.BOID,
                                        D.DEGREE,
                                        M.CATEGORY_NAME,
                                        B.ACCESS_NO,
                                        SLS.CLASS_NO,
                                        B.BOOK_NO,
                                        B.BOOK_ID,
                                        B.ISBN_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE, ''),
                                                        '  ',
                                                        IFNULL(B.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
                                        
                                        R.LOAN_PERIOD,
                                        R.RULE_ID,
                                        IFNULL(LD.FINE_AMOUNT,R.FINE_AMOUNT) AS FINE_AMOUNT
                                    FROM
                                        LI_BOOK_ISSUE AS BI
                                            INNER JOIN
                                        LI_USER_DETAILS AS U ON U.USER_ID = BI.USER_ID
                                        AND U.IS_DELETED!=1
                                            INNER JOIN
                                        LI_BOOK_DETAILS AS B ON B.BOOK_ID = BI.BOOK_ID
                                         AND B.IS_DELETED!=1
                                            INNER JOIN
                                        LI_RULES AS R ON R.MEM_CATEGORY = U.MEMBER_TYPE
                                            AND R.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_DEGREE AS D ON D.DEGREE_ID = U.DEGREE
                                            LEFT JOIN
                                        SUP_LI_MEMBER_CATEGORY AS M ON M.CATEGORY_ID = U.MEMBER_TYPE
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = B.LANGUAGE
                                            LEFT JOIN
                                        LI_DUES AS LD ON LD.USER_ID = BI.USER_ID
                                            AND LD.BOOK_ID = BI.BOOK_ID
                                            AND LD.IS_PAID != 1
                                            
                                            INNER JOIN
                                            SUP_LI_CLASS_NO AS SLS ON SLS.CLASS_NO_ID=B.CLASS_NO
                                            AND SLS.IS_DELETED!=1
                                            
                                          
                                            
                                    WHERE
                                          U.RFID =?RFID
                                          AND BI.IS_DELETED!=1
                                          AND BI.STATUS!=2 GROUP BY BI.ISSUE_ID   ORDER BY BI.ISSUE_ID;";
                        break;
                    }
                case LibrarySQLCommands.ListBooksForIssue:
                    {
                        sQuery = @"";
                        break;
                    }
                case LibrarySQLCommands.ListBooksForIssueByRfid:
                    {
                        sQuery = @"SELECT 
                                    BOOK_ID,
                                    ACCESS_NO,
                                    DOC_TITLE,
                                    CONCAT(IFNULL(PERSONAL_AUTHOR1, ''),
                                            '',
                                            IFNULL(PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                    ISBN_NO,
                                    LI_BOOK_STATUS
                                FROM
                                    LI_BOOK_INFO AS LI
                                       LEFT JOIN
                                    SUP_LI_BOOK_STATUS AS LS ON LS.STATUS_ID = LI.LI_BOOK_STATUS
                                        AND LS.IS_DELETED != 1
                                WHERE
                                    RFID =?RFID;";
                        break;
                    }
                case LibrarySQLCommands.ListBooksForIssueByBarcode:
                    {
                        sQuery = @"SELECT 
                                    BOOK_ID,
                                    ACCESS_NO,
                                    DOC_TITLE,
                                    CONCAT(IFNULL(PERSONAL_AUTHOR1, ''),
                                            '',
                                            IFNULL(PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                    ISBN_NO,
                                    LI_BOOK_STATUS
                                FROM
                                    LI_BOOK_INFO AS LI
                                        LEFT JOIN
                                    SUP_LI_BOOK_STATUS AS LS ON LS.STATUS_ID = LI.LI_BOOK_STATUS
                                        AND LS.IS_DELETED != 1
                                WHERE
                                    BARCODE=?BARCODE;";
                        break;
                    }
                case LibrarySQLCommands.SaveBookIssue:
                    {
                        sQuery = @"INSERT INTO `li_book_issue_?AC_YEAR`
                                (`USER_ID`,
                                `USER_TYPE`,
                                `ROLE`,
                                `BOOK_ID`,
                                `ISSUE_DATE`,
                                `ISSUE_BY`,
                                `RETURN_DATE`,
                                `SEMESTER_ID`,
                                `SEMESTER_TYPE`)
                                VALUES
                                (
                                ?USER_ID,
                                ?USER_TYPE,
                                ?ROLE,
                                ?BOOK_ID,
                                unix_timestamp(),
                                ?ISSUE_BY,
                                ?RETURN_DATE,
                                ?SEMESTER_ID,
                                ?SEMESTER_TYPE);";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalBookIssueByUserId:
                    {
                        sQuery = @"SELECT 
                                        SUM(IF(LI.ISSUE_ID!=0,1,0)) AS TOTAL_ISSUE_COUNT
                                    FROM
                                        LI_BOOK_ISSUE AS LI
                                            INNER JOIN
                                            LI_USER_DETAILS AS LD ON LD.USER_ID=LI.USER_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS STU ON STU.BOID_NUMBER = LD.BOID
                                            AND STU.IS_DELETED != 1
                                            AND STU.IS_LEFT != 1
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = STU.STUDENT_ID
                                            AND SC.IS_DELETED != 1
                                            AND SC.ACADEMIC_YEAR =?AC_YEAR
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS S ON S.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            AND S.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND S.BATCH = SC.BATCH
                                            AND S.IS_ACTIVE = 1
                                    WHERE
                                             LI.IS_DELETED != 1
                                            AND LI.USER_ID=?USER_ID
                                        AND LI.SEMESTER_ID=S.SEMESTER
                                            AND LI.ACADEMIC_YEAR =?AC_YEAR;";
                        break;
                    }
                case LibrarySQLCommands.UpdateBookInfoLiBookStatus:
                    {
                        sQuery = @"UPDATE LI_BOOK_INFO SET LI_BOOK_STATUS=?LI_BOOK_STATUS WHERE BOOK_ID=?BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateBookIssueStatus:
                    {
                        sQuery = @"UPDATE LI_BOOK_ISSUE SET STATUS=?STATUS,RETURN_DATE=NOW(),RECEIVED_BY=?RECEIVED_BY WHERE ISSUE_ID=?ISSUE_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchLiBookForCirculation:
                    {
                        sQuery = @"SELECT 
                                        B.BOOK_ID,
                                        B.ACCESS_NO,
                                        SLSS.CLASS_NO,
                                        B.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                          CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE,B.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        NOTES,
                                        ILLUSTRATE,
                                        PRICE,
                                        ISBN_NO,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        BOOK_PUBLISH_YEAR,
                                        RFID
                                    FROM
                                        LI_BOOK_DETAILS AS B
		                                    INNER JOIN
	                                    SUP_LANGUAGE AS L ON L.LANGUAGE_ID=B.LANGUAGE
                                        
                                        INNER JOIN
                                        SUP_LI_STATUS AS SLS ON SLS.STATUS_ID=B.STATUS
                                        AND SLS.IS_DELETED!=1
		                               LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=B.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=B.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=B.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=B.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=B.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=B.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1

                                              INNER JOIN
                                            SUP_LI_CLASS_NO AS SLSS ON SLSS.CLASS_NO_ID=B.CLASS_NO
                                            AND SLSS.IS_DELETED!=1
										
                                    WHERE
                                        B.BOOK_STATUS != 1 AND B.IS_DELETED!=1
                                        AND B.STATUS!=3;";
                        break;
                    }
                case LibrarySQLCommands.FetchLibookForReturn:
                    {
                        sQuery = @"SELECT 
                                        BI.ISSUE_ID,
                                        U.USER_ID,
                                        DATE_FORMAT(BI.ISSUE_DATE, '%d/%m/%Y %r') AS ISSUE_DATE,
                                        DATE_FORMAT(U.LAST_LOGIN, '%d/%m/%Y %r') AS LAST_LOGIN,
                                        CONCAT(IFNULL(U.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(U.LAST_NAME, '')) AS FIRST_NAME,
                                        U.EMAIL,
                                        U.MOBILE,
                                        U.PHOTO,
                                        U.BOID,
                                        D.DEGREE,
                                        M.CATEGORY_NAME,
                                        B.ACCESS_NO,
                                        SLS.CLASS_NO,
                                        B.BOOK_NO,
                                        B.BOOK_ID,
                                        B.ISBN_NO,
                                        B. RFID,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE, ''),
                                                        '  ',
                                                        IFNULL(B.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
                                                        CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                        
                                        R.LOAN_PERIOD,
                                        R.RULE_ID,
                                        IFNULL(LD.FINE_AMOUNT,R.FINE_AMOUNT) AS FINE_AMOUNT
                                    FROM
                                        LI_BOOK_ISSUE AS BI
                                            INNER JOIN
                                        LI_USER_DETAILS AS U ON U.USER_ID = BI.USER_ID
                                            INNER JOIN
                                        LI_BOOK_DETAILS AS B ON B.BOOK_ID = BI.BOOK_ID
                                         LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=B.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=B.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=B.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=B.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=B.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=B.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
                                            INNER JOIN
                                        LI_RULES AS R ON R.MEM_CATEGORY = U.MEMBER_TYPE
                                            AND R.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_DEGREE AS D ON D.DEGREE_ID = U.DEGREE
                                            INNER JOIN
                                        SUP_LI_MEMBER_CATEGORY AS M ON M.CATEGORY_ID = U.MEMBER_TYPE
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = B.LANGUAGE
                                            LEFT JOIN
                                        LI_DUES AS LD ON LD.USER_ID = BI.USER_ID
                                            AND LD.BOOK_ID = BI.BOOK_ID
                                            AND LD.IS_PAID != 1
                                            
                                            INNER JOIN
                                            SUP_LI_CLASS_NO AS SLS ON SLS.CLASS_NO_ID=B.CLASS_NO
                                            AND SLS.IS_DELETED!=1
                                    WHERE
                                          BI.USER_ID =?USER_ID
                                          AND BI.STATUS!=2;";
                        break;

                    }
                case LibrarySQLCommands.FetchIssueIdByBookIdandUserId:
                    {
                        sQuery = @"
                                    SELECT 
                                    LI.ISSUE_ID,
                                    IF(LD.CREDIT!=0,LD.CREDIT,0)-IF(LD.DEBIT!=0,LD.DEBIT,0)  AS FINE
                                FROM
                                    li_book_issue_?AC_YEAR AS LI
                                        LEFT JOIN
                                    LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                        AND LD.IS_DELETED != 1
                                        AND LD.ACADEMIC_YEAR =?AC_YEAR
                                WHERE
                                    LI.BOOK_ID =?BOOK_ID AND LI.USER_ID =?USER_ID
                                        AND LI.USER_TYPE =?USER_TYPE
                                        AND LI.IS_RETURNED!=1
                                        AND LI.IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.SaveBookReturn:
                    {
                        sQuery = @"UPDATE li_book_issue_?AC_YEAR SET ACTUAL_RETURNED_DATE=unix_timestamp(),IS_RETURNED=?IS_RETURNED WHERE ISSUE_ID=?ISSUE_ID;";
                        break;
                    }
                
                case LibrarySQLCommands.SaveLibraryCheckin:
                    {
                        sQuery = @"INSERT INTO LI_CHECKIN_INFO_?AC_YEAR
                                        ( 
                                        USER_ID, 
                                        USER_TYPE, 
                                        ROLE, 
                                        PROGRAMME_LEVEL,
                                        CHECKIN_DATE,
                                        SEMESTER_TYPE
                                        )
                                        values
										(
                                        ?USER_ID, 
                                        ?USER_TYPE, 
                                        ?ROLE, 
                                        ?PROGRAMME_LEVEL,
                                        ?CHECKIN_DATE, 
                                        ?SEMESTER_TYPE
                                        );";
                        break;
                    }
                //case LibrarySQLCommands.FetchStudentDetailsForCheckin:
                //    {
                //        sQuery = @"SELECT 
                //                    S.STUDENT_ID AS USER_ID,
                //                    'STUDENT' AS USER_TYPE,
                //                    'STUDENT' AS ROLE,
                //                    P.PROGRAMME_LEVEL,
                //                    SC.SEMESTER_ID,
                //                    SEM.SEMESTER_TYPE
                //                FROM 
                //                    STU_PERSONAL_INFO AS S
                //                    INNER JOIN STU_CLASS AS SC ON SC.STUDENT_ID = S.STUDENT_ID AND SC.ACADEMIC_YEAR = ?AC_YEAR AND SC.IS_DELETED != 1
                //                    INNER JOIN CP_PROGRAMME_2017 AS P ON P.PROGRAMME_ID = SC.PROGRAMME_ID
                //                    INNER JOIN ACADEMIC_SEMESTER AS SEM ON SEM.SEMESTER = SC.SEMESTER_ID AND SEM.ACADEMIC_YEAR = ?AC_YEAR
                //                WHERE 
                //                    S.STUDENT_ID = ?USER_ID;";
                //        break;
                //    }
                case LibrarySQLCommands.FetchStudentDetailsForCheckin:
                    {
                        sQuery = @"          
                                            SELECT 
                         S.STUDENT_ID AS USER_ID,
                        'STUDENT' AS USER_TYPE,
                        'STUDENT' AS ROLE,
                        scl.CLASSLEVEL
                    FROM
                        stu_personal_info AS s
                            INNER JOIN
                        stu_class AS sc ON sc.STUDENT_ID = s.STUDENT_ID
                            INNER JOIN
                        cp_classes AS cc ON cc.CLASS_ID = sc.CLASS_ID
                            INNER JOIN
                        sup_class_level AS scl ON scl.CLASSLEVELID = cc.CLASS_LEVEL
                    WHERE
                        S.STUDENT_ID = ?USER_ID;
                              ";
                        break;
                    }
                case LibrarySQLCommands.FetchUserCredentialByStudentLibrary:
                    {
                        sQuery = @"          SELECT 
                                            UA.USER_ID, 
                                            UA.ROLE, 
                                            UA.USER_TYPE, 
                                            UA.NAME AS FIRST_NAME,
                                            (SELECT AC_YEAR FROM ACADEMIC_YEAR WHERE ACTIVE_YEAR=1) AS AC_YEAR,
                                            PG.APPTYPE_ID AS PROGRAMME_LEVEL,
                                            '0' AS SEMESTER,
                                            (SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_ACTIVE=1  AND SEMESTER_TYPE_ID!=3) AS SEMESTER_TYPE
                                        FROM
                                            USER_ACCOUNT AS UA
                                                INNER JOIN
                                            STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = UA.USER_ID
                                                AND SP.ROLL_NO = UA.USERNAME
                                                AND SP.STUDENT_ID in (?STUDENT_ID)
                                                AND SP.IS_DELETED != 1
                                                 INNER JOIN
                                             STU_CLASS AS sc ON sc.STUDENT_ID = sp.STUDENT_ID
                                                 INNER JOIN
                                             CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=Sc.PROGRAMME_GROUP_ID
                                        WHERE
                                            UA.IS_DELETED != 1 and SC.ACADEMIC_YEAR=?AC_YEAR;
                              ";
                        break;
                    }
                case LibrarySQLCommands.FetchBookInfoByIssueId:
                    {
                        sQuery = @"SELECT 
                                        BI.BOOK_ID
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS LI
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                    WHERE
                                        LI.IS_DELETED != 1
                                        AND LI.ISSUE_ID=?ISSUE_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateIssueReturnConfirmedByIssueId:
                    {
                        sQuery = @"UPDATE LI_BOOK_ISSUE_?AC_YEAR SET IS_RETURN_CONFIRMED=?IS_RETURN_CONFIRMED WHERE ISSUE_ID=?ISSUE_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateIssueReturnDamagedByIssueId:
                    {
                        sQuery = @"UPDATE LI_BOOK_ISSUE_?AC_YEAR SET IS_DAMAGED=?IS_DAMAGED WHERE ISSUE_ID=?ISSUE_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateLiBookIssueForReturnedConfirmedSmsSent:
                    {
                        sQuery = @"UPDATE LI_BOOK_ISSUE_?AC_YEAR SET IS_RCSMS_SENT=?IS_RCSMS_SENT WHERE ISSUE_ID IN(?ISSUE_ID);";
                        break;
                    }
                case LibrarySQLCommands.UpdateLiBookIssueForReturnedDamagedSmsSent:
                    {
                        sQuery = @"UPDATE LI_BOOK_ISSUE_?AC_YEAR SET IS_DSMS_SENT=?IS_DSMS_SENT,DSMS_SENT_DATE=unix_timestamp() WHERE ISSUE_ID IN(?ISSUE_ID);";
                        break;
                    }
                case LibrarySQLCommands.UpdateIssueReturnMisseddByIssueId:
                    {
                        sQuery = @"UPDATE LI_BOOK_ISSUE_?AC_YEAR SET IS_MISSED=?IS_MISSED WHERE ISSUE_ID=?ISSUE_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateLiBookIssueForMissingSmsSent:
                    {
                        sQuery = @"UPDATE LI_BOOK_ISSUE_?AC_YEAR SET IS_MSMS_SENT=?IS_MSMS_SENT,MSMS_SENT_DATE=unix_timestamp() WHERE ISSUE_ID IN(?ISSUE_ID);";
                        break;
                    }
                #endregion

                #region Book Retrun
                case LibrarySQLCommands.SaveBookRetrun:
                    {
                        sQuery = @"INSERT INTO LI_BOOK_RETURN
                                    (
                                        ISSUE_ID,
                                        RETURN_DATE,
                                        RECEIVED_BY,
                                        STATUS,
										FINE_AMOUNT,
                                        ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?ISSUE_ID,
                                        now(),
                                        ?RECEIVED_BY,
                                        ?STATUS,
										?FINE_AMOUNT,
                                        ?AC_YEAR
                                    );";
                        break;
                    }
                #endregion

                #region Book Renew

                case LibrarySQLCommands.UpdateBookIssueForRenewal:
                    {
                        sQuery = @"UPDATE LI_BOOK_ISSUE SET IS_RENEWED=?IS_RENEWED,
                                        RENEW_ID=?RENEW_ID WHERE ISSUE_ID=?ISSUE_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookIssueBySemesterAndBookId:
                    {
                        sQuery = @"SELECT 
                                        LI.ISSUE_ID, LI.SEMESTER_ID
                                    FROM
                                        LI_BOOK_ISSUE AS LI
                                            INNER JOIN
                                            LI_USER_DETAILS AS LD ON LD.USER_ID=LI.USER_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS STU ON STU.BOID_NUMBER = LD.BOID
                                            AND STU.IS_DELETED != 1
                                            AND STU.IS_LEFT != 1
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = STU.STUDENT_ID
                                            AND SC.IS_DELETED != 1
                                            AND SC.ACADEMIC_YEAR =?AC_YEAR
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS S ON S.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            AND S.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND S.BATCH = SC.BATCH
                                            AND S.IS_ACTIVE = 1
                                    WHERE
                                        LI.BOOK_ID =?BOOK_ID AND LI.IS_DELETED != 1
                                        AND LI.USER_ID=?USER_ID
                                        AND LI.SEMESTER_ID=S.SEMESTER
                                        AND LI.IS_RENEWED=1
                                        AND LI.IS_RENEW_CANCELLED!=1
                                            AND LI.ACADEMIC_YEAR =?AC_YEAR;";
                        break;
                    }
                case LibrarySQLCommands.FetchRenewDayIdByUserId:
                    {
                        sQuery = @"SELECT 
                                    SR.RENEW_DAY_ID,
                                    SR.DAYS
                                     FROM LI_USER_DETAILS AS LU
                                    INNER JOIN
                                    sup_li_member_category AS SM ON SM.CATEGORY_ID=LU.MEMBER_TYPE
                                    AND SM.IS_DELETED!=1

                                    INNER JOIN
                                    SUP_LI_RENEW_DAYS AS SR ON SR.MEMBER_TYPE=SM.CATEGORY_ID
                                    AND SR.IS_DELETED!=1

                                    WHERE 
                                    LU.USER_ID=?USER_ID
                                    AND LU.IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsForRenewByUserId:
                    {
                        sQuery = @"SELECT 
                                        BI.ISSUE_ID,
                                        U.USER_ID,
                                        DATE_FORMAT(BI.ISSUE_DATE, '%d/%m/%Y %r') AS ISSUE_DATE,
                                        DATE_FORMAT(U.LAST_LOGIN, '%d/%m/%Y %r') AS LAST_LOGIN,
                                        CONCAT(IFNULL(U.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(U.LAST_NAME, '')) AS FIRST_NAME,
                                        U.EMAIL,
                                        U.MOBILE,
                                        U.PHOTO,
                                        U.BOID,
                                        D.DEGREE,
                                        M.CATEGORY_NAME,
                                        B.ACCESS_NO,
                                        SLS.CLASS_NO,
                                        B.BOOK_NO,
                                        B.BOOK_ID,
                                        B.ISBN_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE, ''),
                                                        '  ',
                                                        IFNULL(B.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
                                        
                                        R.LOAN_PERIOD,
                                        R.RULE_ID,
                                        IFNULL(LD.FINE_AMOUNT,'0') AS FINE_AMOUNT
                                    FROM
                                        LI_BOOK_ISSUE AS BI
                                            INNER JOIN
                                        LI_USER_DETAILS AS U ON U.USER_ID = BI.USER_ID
                                            INNER JOIN
                                        LI_BOOK_DETAILS AS B ON B.BOOK_ID = BI.BOOK_ID
                                            INNER JOIN
                                        LI_RULES AS R ON R.MEM_CATEGORY = U.MEMBER_TYPE
                                            AND R.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_DEGREE AS D ON D.DEGREE_ID = U.DEGREE
                                            INNER JOIN
                                        SUP_LI_MEMBER_CATEGORY AS M ON M.CATEGORY_ID = U.MEMBER_TYPE
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = B.LANGUAGE
                                            LEFT JOIN
                                        LI_DUES AS LD ON LD.USER_ID = BI.USER_ID
                                            AND LD.BOOK_ID = BI.BOOK_ID
                                            AND LD.IS_PAID != 1
                                            
                                            INNER JOIN
                                            SUP_LI_CLASS_NO AS SLS ON SLS.CLASS_NO_ID=B.CLASS_NO
                                            AND SLS.IS_DELETED!=1
                                    WHERE
                                          BI.USER_ID =?USER_ID
                                          AND BI.IS_RENEWED!=1
                                          AND BI.STATUS!=2;";
                        break;
                    }
                #endregion

                #region Book Reading
                case LibrarySQLCommands.ListBooksForReadingByRfid:
                    {
                        sQuery = @"SELECT 
                                        B.BOOK_ID,
                                        B.ACCESS_NO,
                                        B.CLASS_NO,
                                        B.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                          CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE,B.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        NOTES,
                                        ILLUSTRATE,
                                        PRICE,
                                        ISBN_NO,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        BOOK_PUBLISH_YEAR,
                                        RFID
                                    FROM
                                        LI_BOOK_DETAILS AS B
		                                    INNER JOIN
	                                    SUP_LANGUAGE AS L ON L.LANGUAGE_ID=B.LANGUAGE
                                        
                                        INNER JOIN
                                        SUP_LI_STATUS AS SLS ON SLS.STATUS_ID=B.STATUS
                                        AND SLS.IS_DELETED!=1
		                               LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=B.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=B.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=B.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=B.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=B.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=B.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
										
                                    WHERE
                                        B.BOOK_STATUS != 1 AND BOOK_STATUS!=3 AND BOOK_STATUS!=4 AND BOOK_STATUS!=5
                                        AND B.BOOK_STATUS != 6 AND B.IS_DELETED!=1
                                        AND B.RFID=?RFID
                                        AND B.STATUS!=3;";
                        break;
                    }
                case LibrarySQLCommands.SaveBookReading:
                    {
                        sQuery = @"INSERT INTO LI_BOOK_READING
                                    (
                                        USER_ID,
                                        BOOK_ID,
                                        READING_DATE,
                                        ISSUE_BY,
                                        STATUS,
                                        ACADEMIC_YEAR,
                                        SEMESTER_ID,
                                        SEMESTER_TYPE
                                    )
                                    VALUES
                                    (
                                        ?USER_ID,
                                        ?BOOK_ID,
                                        now(),
                                        ?ISSUE_BY,
                                        ?STATUS,
                                        ?AC_YEAR,
                                        ?SEMESTER_ID,
                                        ?SEMESTER_TYPE
                                    );";
                        break;
                    }
                case LibrarySQLCommands.FetchBookReadingDetailsById:
                    {
                        sQuery = @"SELECT 
                                        BR.READING_ID,
                                        U.USER_ID,
                                        DATE_FORMAT(BR.READING_DATE, '%d/%m/%Y %r') AS READING_DATE,
                                        DATE_FORMAT(U.LAST_LOGIN, '%d/%m/%Y %r') AS LAST_LOGIN,
                                        CONCAT(IFNULL(U.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(U.LAST_NAME, '')) AS FIRST_NAME,
                                        U.EMAIL,
                                        U.MOBILE,
                                        U.PHOTO,
                                        U.BOID,
                                        D.DEGREE,
                                        M.CATEGORY_NAME,
                                        B.ACCESS_NO,
                                        SLS.CLASS_NO,
                                        B.BOOK_NO,
                                        B.BOOK_ID,
                                        B.ISBN_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE, ''),
                                                        '  ',
                                                        IFNULL(B.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
                                        
                                        R.LOAN_PERIOD,
                                        R.RULE_ID,
                                        IFNULL(LD.FINE_AMOUNT,R.FINE_AMOUNT) AS FINE_AMOUNT
                                    FROM
                                        LI_BOOK_READING AS BR
                                            INNER JOIN
                                        LI_USER_DETAILS AS U ON U.USER_ID = BR.USER_ID
                                            INNER JOIN
                                        LI_BOOK_DETAILS AS B ON B.BOOK_ID = BR.BOOK_ID
                                            INNER JOIN
                                        LI_RULES AS R ON R.MEM_CATEGORY = U.MEMBER_TYPE
                                            AND R.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_DEGREE AS D ON D.DEGREE_ID = U.DEGREE
                                            INNER JOIN
                                        SUP_LI_MEMBER_CATEGORY AS M ON M.CATEGORY_ID = U.MEMBER_TYPE
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = B.LANGUAGE
                                            INNER JOIN
                                            SUP_LI_CLASS_NO AS SLS ON SLS.CLASS_NO_ID=B.CLASS_NO
                                            AND SLS.IS_DELETED!=1
                                    WHERE
                                          BR.USER_ID =?USER_ID
                                          AND BR.IS_DELETED!=1
                                          AND BR.STATUS!=2;";
                        break;
                    }
                case LibrarySQLCommands.SaveBookReadingRetrun:
                    {
                        sQuery = @"UPDATE LI_BOOK_READING
                                SET STATUS=?STATUS,
                                RETURN_DATE=?NOW(),
                                RECEIVED_BY=?RECEIVED_BY
                                WHERE 
                                READING_ID=?READING_ID;";
                        break;
                    }
                #endregion

                #region Articles
                case LibrarySQLCommands.FetchTotalArticles:
                    {
                        sQuery = @" SELECT 
                                    BOOK_ID,
                                    CONCAT(IFNULL(LB.BLOCK_NAME, ''),
                                            ' ',
                                            'BOW:',
                                            IFNULL(LR.BOW_NO, ''),
                                            ' ',
                                            'RACK:',
                                            IFNULL(LR.RACK_NO, ''),
                                            ' ',
                                            'RACK:',
                                            IFNULL(LR.SECTION_NO, ''),
                                            ' ',
                                            'SECTION:',
                                            IFNULL(LR.POSITION, '')) AS RACK_NO,
                                    GROUP_CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.PLACE, ''),
                                            ' ',
                                            IFNULL(LA.PUBLISHER, '')) AS AUTHOR,
                                    BOOK_TITLE,
                                    SERIES_CODE,
                                    SERIES,
                                    BOOK_PUPLISH_YEAR,
                                    SA.ARTICLE_TYPE_NAME AS ARTICLE_TYPE,
                                    PAGES,
                                    ABSTRACT,
                                    RFID
                                FROM
                                    LI_BOOK_DETAILS AS LD
                                        INNER JOIN
                                    SUP_LI_ARTICLE_TYPE AS SA ON SA.ARTICLE_TYPE_ID = LD.ARTICLE_TYPE
                                        AND SA.IS_DELETED != 1
                                        LEFT JOIN
                                    LI_AUTHOR_INFO AS LA ON FIND_IN_SET(LA.AUTHOR_ID,LD.AUTHOR_ID)
                                        AND LA.IS_DELETED != 1
                                        LEFT JOIN
                                    LI_RACK_INFO AS LR ON LR.RACK_ID = LD.RACK_ID
                                        AND LR.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_LI_BLOCK AS LB ON LB.BLOCK_ID = LR.BLOCK_ID
                                        AND LB.IS_DELETED != 1
                                WHERE
                                    LD.IS_DELETED != 1 AND LD.IS_ARTICLE = 1
                                    GROUP BY LD.BOOK_ID ORDER BY LD.BOOK_ID";
                        break;
                    }
                case LibrarySQLCommands.FetchArticlesById:
                    {
                        sQuery = @"SELECT 
                                    BOOK_ID,
                                    LD.RACK_ID,
                                    LD.AUTHOR_ID,
                                    BOOK_NO,
                                    BOOK_TITLE,
                                    SERIES_CODE,
                                    SERIES,
                                    BOOK_PUPLISH_YEAR,
                                    ARTICLE_TYPE,
                                    PAGES,
                                    ABSTRACT,
                                    RFID
                                FROM
                                    LI_BOOK_DETAILS AS LD
                                        INNER JOIN
                                    SUP_LI_ARTICLE_TYPE AS SA ON SA.ARTICLE_TYPE_ID = LD.ARTICLE_TYPE
                                        AND SA.IS_DELETED != 1
                                        LEFT JOIN
                                    LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID IN(LD.AUTHOR_ID)
                                        AND LA.IS_DELETED != 1
                                        LEFT JOIN
                                    LI_RACK_INFO AS LR ON LR.RACK_ID = LD.RACK_ID
                                        AND LR.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_LI_BLOCK AS LB ON LB.BLOCK_ID = LR.BLOCK_ID
                                        AND LB.IS_DELETED != 1
                                WHERE
                                    LD.IS_DELETED != 1 AND LD.IS_ARTICLE = 1
                                    AND LD.BOOK_ID=?BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.SaveArticleDetails:
                    {
                        sQuery = @"INSERT INTO LI_BOOK_DETAILS
                                    (
                                      RACK_ID,
                                      BOOK_NO,
                                      AUTHOR_ID,
                                      BOOK_TITLE,
                                      SERIES_CODE,
                                      SERIES,
                                      ACADEMIC_YEAR,
                                      RFID,
                                      IS_ARTICLE,
                                      BOOK_PUPLISH_YEAR,
									  ARTICLE_TYPE,
                                      PAGES,
								      ABSTRACT
                                    )
                                    VALUES
                                    (
                                      ?RACK_ID,
                                      ?BOOK_NO,
                                      ?AUTHOR_ID,
                                      ?BOOK_TITLE,
                                      ?SERIES_CODE,
                                      ?SERIES,
                                      ?AC_YEAR,
                                      ?RFID,
                                      '1',
                                      ?BOOK_PUPLISH_YEAR,
									  ?ARTICLE_TYPE,
                                      ?PAGES,
									  ?ABSTRACT
                                    );";
                        break;
                    }
                case LibrarySQLCommands.UpdateArticleDetailsById:
                    {
                        sQuery = @"UPDATE LI_BOOK_DETAILS SET 
	                                    BOOK_NO=?BOOK_NO,
	                                    RACK_ID=?RACK_ID,
	                                    AUTHOR_ID=?AUTHOR_ID,
	                                    BOOK_TITLE=?BOOK_TITLE,
	                                    SERIES_CODE=?SERIES_CODE,
                                        SERIES=?SERIES,
	                                    RFID=?RFID,
                                        BOOK_PUPLISH_YEAR=?BOOK_PUPLISH_YEAR,
                                        ARTICLE_TYPE=?ARTICLE_TYPE,
                                        PAGES=?PAGES,
                                        ABSTRACT=?ABSTRACT
                                    WHERE  BOOK_ID=?BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.DeleteArticleById:
                    {
                        sQuery = @"UPDATE
                                    LI_BOOK_DETAILS 
                                        SET IS_DELETED=?IS_DELETED
                                        WHERE BOOK_ID=?BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsByRfid:
                    {
                        sQuery = @"SELECT 
                                        BOOK_ID,
                                        RACK_ID,
                                        ACCESS_NO,
                                        CLASS_NO,
                                        BOOK_NO,
                                        LANGUAGE,
                                        PERSONAL_AUTHOR1,
                                        PERSONAL_AUTHOR2,
                                        JOINED_AUTHOR1,
                                        JOINED_AUTHOR2,
                                        CORPORATE_AUTHOR1,
                                        CORPORATE_AUTHOR2,
                                        DOC_TITLE,
                                        PARALLEL_TITLE,
                                        NOTES,
                                        PHYDESC,
                                        ILLUSTRATE,
                                        STATUS,
                                        PRICE,
                                        DATE_FORMAT(DOP,'%d/%m/%Y') AS DOP,
                                        ISBN_NO,
                                        DISIPLINE,
                                        TAPS,
                                        DETAILS,
                                        VC,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        SERIES,
                                        LIB_CATEGORY,
                                        DOC_CATEGORY,
                                        BOOK_PUBLISH_YEAR,
                                        SUB_ACCOUNT,
                                        KEYWORD,
                                        INVOICE,
                                        ACADEMIC_YEAR,
                                        LI_BOOK_STATUS,
                                        ENTRY_DATE,
                                        RFID,
                                        BARCODE
                                    FROM
                                        LI_BOOK_INFO
                                    WHERE
                                        IS_DELETED != 1 AND RFID=?RFID;";
                        break;
                    }

                case LibrarySQLCommands.FetchBookDetailsByAccessNo:
                    {
                        sQuery = @"SELECT 
                                        BOOK_ID,
                                        RACK_ID,
                                        ACCESS_NO,
                                        CLASS_NO,
                                        BOOK_NO,
                                        LANGUAGE,
                                        PERSONAL_AUTHOR1,
                                        PERSONAL_AUTHOR2,
                                        JOINED_AUTHOR1,
                                        JOINED_AUTHOR2,
                                        CORPORATE_AUTHOR1,
                                        CORPORATE_AUTHOR2,
                                        DOC_TITLE,
                                        PARALLEL_TITLE,
                                        NOTES,
                                        PHYDESC,
                                        ILLUSTRATE,
                                        STATUS,
                                        PRICE,
                                        DATE_FORMAT(DOP,'%d/%m/%Y') AS DOP,
                                        ISBN_NO,
                                        DISIPLINE,
                                        TAPS,
                                        DETAILS,
                                        VC,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        SERIES,
                                        LIB_CATEGORY,
                                        DOC_CATEGORY,
                                        BOOK_PUBLISH_YEAR,
                                        SUB_ACCOUNT,
                                        KEYWORD,
                                        INVOICE,
                                        ACADEMIC_YEAR,
                                        LI_BOOK_STATUS,
                                        ENTRY_DATE,
                                        RFID,
                                        BARCODE
                                    FROM
                                        LI_BOOK_INFO
                                    WHERE
                                        IS_DELETED != 1 AND ACCESS_NO=?ACCESS_NO;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsByBookNo:
                    {
                        sQuery = @"SELECT 
                                        BOOK_ID,
                                        RACK_ID,
                                        ACCESS_NO,
                                        CLASS_NO,
                                        BOOK_NO,
                                        LANGUAGE,
                                        PERSONAL_AUTHOR1,
                                        PERSONAL_AUTHOR2,
                                        JOINED_AUTHOR1,
                                        JOINED_AUTHOR2,
                                        CORPORATE_AUTHOR1,
                                        CORPORATE_AUTHOR2,
                                        DOC_TITLE,
                                        PARALLEL_TITLE,
                                        NOTES,
                                        PHYDESC,
                                        ILLUSTRATE,
                                        STATUS,
                                        PRICE,
                                        DATE_FORMAT(DOP,'%d/%m/%Y') AS DOP,
                                        ISBN_NO,
                                        DISIPLINE,
                                        TAPS,
                                        DETAILS,
                                        VC,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        SERIES,
                                        LIB_CATEGORY,
                                        DOC_CATEGORY,
                                        BOOK_PUBLISH_YEAR,
                                        SUB_ACCOUNT,
                                        KEYWORD,
                                        INVOICE,
                                        ACADEMIC_YEAR,
                                        LI_BOOK_STATUS,
                                        ENTRY_DATE,
                                        RFID,
                                        BARCODE
                                    FROM
                                        LI_BOOK_INFO
                                    WHERE
                                        IS_DELETED != 1 AND BOOK_NO=?BOOK_NO;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsByBarCode:
                    {
                        sQuery = @"SELECT 
                                        BOOK_ID,
                                        RACK_ID,
                                        ACCESS_NO,
                                        CLASS_NO,
                                        BOOK_NO,
                                        LANGUAGE,
                                        PERSONAL_AUTHOR1,
                                        PERSONAL_AUTHOR2,
                                        JOINED_AUTHOR1,
                                        JOINED_AUTHOR2,
                                        CORPORATE_AUTHOR1,
                                        CORPORATE_AUTHOR2,
                                        DOC_TITLE,
                                        PARALLEL_TITLE,
                                        NOTES,
                                        PHYDESC,
                                        ILLUSTRATE,
                                        STATUS,
                                        PRICE,
                                        DATE_FORMAT(DOP,'%d/%m/%Y') AS DOP,
                                        ISBN_NO,
                                        DISIPLINE,
                                        TAPS,
                                        DETAILS,
                                        VC,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        SERIES,
                                        LIB_CATEGORY,
                                        DOC_CATEGORY,
                                        BOOK_PUBLISH_YEAR,
                                        SUB_ACCOUNT,
                                        KEYWORD,
                                        INVOICE,
                                        ACADEMIC_YEAR,
                                        LI_BOOK_STATUS,
                                        ENTRY_DATE,
                                        RFID,
                                        BARCODE
                                    FROM
                                        LI_BOOK_INFO
                                    WHERE
                                        IS_DELETED != 1 AND BARCODE=?BARCODE;";
                        break;
                    }
                #endregion

                #region Author Info
                case LibrarySQLCommands.ListAuthorInfo:
                    {
                        sQuery = @"SELECT 
                                    AUTHOR_ID,
                                    FIRST_NAME,
                                    LAST_NAME,
                                    PUBLISHER,
                                    PLACE
                                FROM
                                    LI_AUTHOR_INFO WHERE IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.FetchAuthorInfoById:
                    {
                        sQuery = @"SELECT 
                                        AUTHOR_ID, 
                                        FIRST_NAME, 
                                        LAST_NAME, 
                                        PUBLISHER, 
                                        PLACE
                                    FROM
                                        LI_AUTHOR_INFO
                                    WHERE
                                        AUTHOR_ID = ?AUTHOR_ID;";
                        break;
                    }
                case LibrarySQLCommands.DeleteAuthorInfo:
                    {
                        sQuery = @"UPDATE LI_AUTHOR_INFO SET IS_DELETED=1 WHERE AUTHOR_ID = ?AUTHOR_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateAuthorInfo:
                    {
                        sQuery = @"UPDATE LI_AUTHOR_INFO 
                                    SET 
                                        FIRST_NAME=?FIRST_NAME,
                                        LAST_NAME=?LAST_NAME,
                                        PUBLISHER=?PUBLISHER,
                                        PLACE=?PLACE
                                      --  ENTRY_ID=?ENTRY_ID
                                    WHERE
                                        AUTHOR_ID = ?AUTHOR_ID;";
                        break;
                    }
                case LibrarySQLCommands.SaveAuthorInfo:
                    {
                        sQuery = @"INSERT INTO LI_AUTHOR_INFO 
                                   (
                                       FIRST_NAME,
                                       LAST_NAME,
                                       PUBLISHER,
                                       PLACE,
                                     --  ENTRY_ID,
                                       ACADEMIC_YEAR
                                   )
                                   VALUES
                                   (
                                       ?FIRST_NAME,
                                       ?LAST_NAME,
                                       ?PUBLISHER,
                                       ?PLACE,
                                     --  ?ENTRY_ID,
                                       ?AC_YEAR
                                   );";
                        break;
                    }
                #endregion

                #region Book Reserve
                case LibrarySQLCommands.FetchAvailableBooks:
                    {
                        sQuery = @"SELECT 

                                        LD.BOOK_ID,
                                        LD.ACCESS_NO,
                                        SLC.CLASS_NO,
                                        LD.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                        CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                       UPPER(CONCAT(IFNULL(LD.DOC_TITLE,LD.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        LD.NOTES,
                                       LD.PHYDESC,
                                       LD.ILLUSTRATE,
                                        CONCAT(IFNULL(SS.STATUS,''),' - ',IFNULL(SS.STATUS_CODE,'')) STATUS,
                                        LD.PRICE,
                                        DATE_FORMAT(LD.DOP,'%Y/%m/%d') AS DOP,
                                        LD.ISBN_NO,
                                        LD.DISCODE,
                                        LD.NAME_OF_PUBLISHER,
                                        LD.PLACE_OF_PUBLISHER,
                                        CONCAT(IFNULL(SLS.SERIES_NAME,''),' - ',IFNULL(SLS.SERIES_CODE,'')) AS SERIES,
                                        LD.RFID,
                                        CONCAT(IFNULL(SLL.LIB_DESCRIPTION,''),' - ',IFNULL(SLL.LIB_CODE,'')) AS LIB_CODE,
                                        LD.BOOK_PUBLISH_YEAR
                                        
                                    FROM
                                        LI_BOOK_DETAILS AS LD
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = LD.LANGUAGE
                                            LEFT JOIN
                                        SUP_LI_STATUS AS SS ON SS.STATUS_ID = LD.STATUS
                                            AND SS.IS_DELETED != 1
                                            
                                            LEFT JOIN
                                        SUP_LI_CLASS_NO AS SLC ON SLC.CLASS_NO_ID = LD.CLASS_NO
                                            AND SLC.IS_DELETED != 1
                                            LEFT JOIN
                                            SUP_LI_SERIES AS SLS ON SLS.SERIES_ID=LD.SERIES
                                            AND SLS.IS_DELETED!=1
                                            
                                            LEFT JOIN
                                            SUP_LI_LIB_CODE AS SLL ON SLL.LIB_CODE_ID=LD.LIB_CODE
                                            AND SLL.IS_DELETED!=1
                                    WHERE

                                      LD.BOOK_STATUS != 1 AND LD.BOOK_STATUS != 5 AND LD.BOOK_STATUS!=6 AND LD.BOOK_STATUS!=3
                                        AND LD.IS_DELETED != 1 GROUP BY LD.BOOK_ID
                                ORDER BY LD.BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.AddBookReserve:
                    {
                        sQuery = @"INSERT INTO LI_BOOK_RESERVE
                                    (
                                        USER_ID,
                                        USER_TYPE,
                                        ROLE,
                                        PROGRAMME_LEVEL,
                                        BOOK_ID,
                                        ENTRY_DATE,
                                        ACADEMIC_YEAR,
                                        SEMESTER_TYPE
                                    )VALUES
                                    (
                                        ?USER_ID,
                                        ?USER_TYPE,
                                        ?ROLE,
                                        ?PROGRAMME_LEVEL,
                                        ?BOOK_ID,
                                        unix_timestamp(),
                                        ?AC_YEAR,
                                        (SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_ACTIVE=1 AND SEMESTER_TYPE_ID!=3)
                                    );";
                        break;
                    }

                case LibrarySQLCommands.DeleteBookReserveById:
                    {
                        sQuery = @"UPDATE LI_BOOK_RESERVE SET IS_DELETED=?IS_DELETED WHERE RESERVE_ID=?RESERVE_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchMilliSecondsByDates:
                    {
                        sQuery = @"SELECT 
                                  datediff(DATE_FORMAT(?START_DATE,'%Y-%m-%d'),DATE_FORMAT(?END_DATE,'%Y-%m-%d'))*24*3600000 
                                 AS ELASPED_TIME;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsForReserveByAccessNo:
                    {
                        sQuery = @"SELECT 

                                        LD.BOOK_ID,
                                        LD.ACCESS_NO,
                                        SLC.CLASS_NO,
                                        LD.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                        CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                       UPPER(CONCAT(IFNULL(LD.DOC_TITLE,LD.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        LD.NOTES,
                                       LD.PHYDESC,
                                       LD.ILLUSTRATE,
                                        CONCAT(IFNULL(SS.STATUS,''),' - ',IFNULL(SS.STATUS_CODE,'')) STATUS,
                                        LD.PRICE,
                                        DATE_FORMAT(LD.DOP,'%Y/%m/%d') AS DOP,
                                        LD.ISBN_NO,
                                        LD.DISCODE,
                                        LD.NAME_OF_PUBLISHER,
                                        LD.PLACE_OF_PUBLISHER,
                                        CONCAT(IFNULL(SLS.SERIES_NAME,''),' - ',IFNULL(SLS.SERIES_CODE,'')) AS SERIES,
                                        LD.RFID,
                                        CONCAT(IFNULL(SLL.LIB_DESCRIPTION,''),' - ',IFNULL(SLL.LIB_CODE,'')) AS LIB_CODE,
                                        LD.BOOK_PUBLISH_YEAR
                                        
                                    FROM
                                        LI_BOOK_DETAILS AS LD
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = LD.LANGUAGE
                                            LEFT JOIN
                                        SUP_LI_STATUS AS SS ON SS.STATUS_ID = LD.STATUS
                                            AND SS.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=LD.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=LD.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=LD.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=LD.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=LD.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=LD.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_LI_CLASS_NO AS SLC ON SLC.CLASS_NO_ID = LD.CLASS_NO
                                            AND SLC.IS_DELETED != 1
                                            LEFT JOIN
                                            SUP_LI_SERIES AS SLS ON SLS.SERIES_ID=LD.SERIES
                                            AND SLS.IS_DELETED!=1
                                            
                                            LEFT JOIN
                                            SUP_LI_LIB_CODE AS SLL ON SLL.LIB_CODE_ID=LD.LIB_CODE
                                            AND SLL.IS_DELETED!=1
                                    WHERE

                                      LD.BOOK_STATUS != 1 AND LD.BOOK_STATUS != 5 AND LD.BOOK_STATUS!=6 AND LD.BOOK_STATUS!=3
                                        AND LD.IS_DELETED != 1 
                                        AND LD.ACCESS_NO=?ACCESS_NO
                                        GROUP BY LD.BOOK_ID
                                ORDER BY LD.BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookReserveBySemesterAndBookIdForStudent:
                    {
                        sQuery = @"SELECT 
                                        LR.RESERVE_ID, LR.SEMESTER_ID
                                    FROM
                                        LI_BOOK_RESERVE AS LR
                                            INNER JOIN
                                            LI_USER_DETAILS AS LD ON LD.USER_ID=LR.USER_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS STU ON STU.BOID_NUMBER = LD.BOID
                                            AND STU.IS_DELETED != 1
                                            AND STU.IS_LEFT != 1
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = STU.STUDENT_ID
                                            AND SC.IS_DELETED != 1
                                            AND SC.ACADEMIC_YEAR =?AC_YEAR
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS S ON S.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            AND S.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND S.BATCH = SC.BATCH
                                            AND S.IS_ACTIVE = 1
                                    WHERE
                                        LR.BOOK_ID =?BOOK_ID AND LR.IS_DELETED != 1
                                        AND LR.USER_ID=?USER_ID
                                        AND LR.SEMESTER_ID=S.SEMESTER
                                            AND LR.ACADEMIC_YEAR =?AC_YEAR;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookReserveBySemesterAndBookIdForStaff:
                    {
                        sQuery = @"           SELECT 
                                        LR.RESERVE_ID, LR.SEMESTER_TYPE
                                    FROM
                                        LI_BOOK_RESERVE AS LR
                                            INNER JOIN
                                            LI_USER_DETAILS AS LD ON LD.USER_ID=LR.USER_ID
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.BOID_NUMBER = LD.BOID
                                            AND STF.IS_DELETED != 1
                                            AND STF.IS_LEFT != 1
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS S ON S.SEMESTER_TYPE_ID=LR.SEMESTER_TYPE
											AND S.IS_ACTIVE = 1
                                            AND S.IS_DELETED!=1
                                    WHERE
                                        LR.BOOK_ID =?BOOK_ID AND LR.IS_DELETED != 1
                                        AND LR.USER_ID=?USER_ID
                                            AND LR.ACADEMIC_YEAR =?AC_YEAR;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsForReserveByRfid:
                    {
                        sQuery = @"SELECT 

                                        LD.BOOK_ID,
                                        LD.ACCESS_NO,
                                        SLC.CLASS_NO,
                                        LD.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                        CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                       UPPER(CONCAT(IFNULL(LD.DOC_TITLE,LD.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        LD.NOTES,
                                       LD.PHYDESC,
                                       LD.ILLUSTRATE,
                                        CONCAT(IFNULL(SS.STATUS,''),' - ',IFNULL(SS.STATUS_CODE,'')) STATUS,
                                        LD.PRICE,
                                        DATE_FORMAT(LD.DOP,'%Y/%m/%d') AS DOP,
                                        LD.ISBN_NO,
                                        LD.DISCODE,
                                        LD.NAME_OF_PUBLISHER,
                                        LD.PLACE_OF_PUBLISHER,
                                        CONCAT(IFNULL(SLS.SERIES_NAME,''),' - ',IFNULL(SLS.SERIES_CODE,'')) AS SERIES,
                                        LD.RFID,
                                        CONCAT(IFNULL(SLL.LIB_DESCRIPTION,''),' - ',IFNULL(SLL.LIB_CODE,'')) AS LIB_CODE,
                                        LD.BOOK_PUBLISH_YEAR
                                        
                                    FROM
                                        LI_BOOK_DETAILS AS LD
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = LD.LANGUAGE
                                            LEFT JOIN
                                        SUP_LI_STATUS AS SS ON SS.STATUS_ID = LD.STATUS
                                            AND SS.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=LD.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=LD.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=LD.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=LD.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=LD.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=LD.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_LI_CLASS_NO AS SLC ON SLC.CLASS_NO_ID = LD.CLASS_NO
                                            AND SLC.IS_DELETED != 1
                                            LEFT JOIN
                                            SUP_LI_SERIES AS SLS ON SLS.SERIES_ID=LD.SERIES
                                            AND SLS.IS_DELETED!=1
                                            
                                            LEFT JOIN
                                            SUP_LI_LIB_CODE AS SLL ON SLL.LIB_CODE_ID=LD.LIB_CODE
                                            AND SLL.IS_DELETED!=1
                                    WHERE

                                      LD.BOOK_STATUS != 1 AND LD.BOOK_STATUS != 5 AND LD.BOOK_STATUS!=6 AND LD.BOOK_STATUS!=3
                                        AND LD.IS_DELETED != 1 
                                        AND LD.RFID=?RFID
                                        GROUP BY LD.BOOK_ID
                                ORDER BY LD.BOOK_ID;";
                        break;
                    }
                #endregion

                #region Book Reservation Circulation for admin
                case LibrarySQLCommands.FetchAllReservedBooks:
                    {
                        sQuery = @"SELECT 

                                        LD.BOOK_ID,
                                        LD.ACCESS_NO,
                                        SLC.CLASS_NO,
                                        LD.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                        CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                       UPPER(CONCAT(IFNULL(LD.DOC_TITLE,LD.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        LD.NOTES,
                                       LD.PHYDESC,
                                       LD.ILLUSTRATE,
                                        CONCAT(IFNULL(SS.STATUS,''),' - ',IFNULL(SS.STATUS_CODE,'')) STATUS,
                                        LD.PRICE,
                                        DATE_FORMAT(LD.DOP,'%Y/%m/%d') AS DOP,
                                        LD.ISBN_NO,
                                        LD.DISCODE,
                                        LD.NAME_OF_PUBLISHER,
                                        LD.PLACE_OF_PUBLISHER,
                                        CONCAT(IFNULL(SLS.SERIES_NAME,''),' - ',IFNULL(SLS.SERIES_CODE,'')) AS SERIES,
                                        LD.RFID,
                                        CONCAT(IFNULL(SLL.LIB_DESCRIPTION,''),' - ',IFNULL(SLL.LIB_CODE,'')) AS LIB_CODE,
                                        LD.BOOK_PUBLISH_YEAR,
                                            LBR.RESERVE_ID,
                                        LBR.USER_ID,
                                        CONCAT(IFNULL(LU.FIRST_NAME,''),' ',
                                        IFNULL(LU.LAST_NAME,'')) AS FIRST_NAME,
                                        LU.BOID,
                                         M.CATEGORY_NAME,
                                         D.DEGREE,
                                        DATE_FORMAT(LBR.RESERVE_DATE,'%Y-%m-%d') AS RESERVE_DATE
                                    FROM
                                        LI_BOOK_DETAILS AS LD
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = LD.LANGUAGE
                                            LEFT JOIN
                                        SUP_LI_STATUS AS SS ON SS.STATUS_ID = LD.STATUS
                                            AND SS.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=LD.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=LD.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=LD.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=LD.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=LD.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=LD.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_LI_CLASS_NO AS SLC ON SLC.CLASS_NO_ID = LD.CLASS_NO
                                            AND SLC.IS_DELETED != 1
                                            LEFT JOIN
                                            SUP_LI_SERIES AS SLS ON SLS.SERIES_ID=LD.SERIES
                                            AND SLS.IS_DELETED!=1
                                            
                                            LEFT JOIN
                                            SUP_LI_LIB_CODE AS SLL ON SLL.LIB_CODE_ID=LD.LIB_CODE
                                            AND SLL.IS_DELETED!=1
                                            
                                            INNER JOIN
                                             LI_BOOK_RESERVE AS LBR ON LBR.BOOK_ID = LD.BOOK_ID
                                             AND LBR.IS_APPROVED!=1
                                             AND LBR.IS_COLLECTED!=1
                                            AND LBR.IS_DELETED != 1
                                            AND LBR.ACADEMIC_YEAR=?AC_YEAR
                                            
                                            INNER JOIN
                                            LI_USER_DETAILS AS LU ON LU.USER_ID=LBR.USER_ID
                                            AND LU.IS_DELETED!=1
                                             INNER JOIN
                                        SUP_DEGREE AS D ON D.DEGREE_ID = LU.DEGREE
                                            INNER JOIN
                                        SUP_LI_MEMBER_CATEGORY AS M ON M.CATEGORY_ID = LU.MEMBER_TYPE
                                           
                                            
                                    WHERE

                                      LD.BOOK_STATUS =5
                                        AND LD.IS_DELETED != 1
                                        AND LD.STATUS!=3 GROUP BY LBR.RESERVE_ID
                                ORDER BY LD.BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchAllReservedBookByUserId:
                    {
                        sQuery = @"SELECT 

                                        LD.BOOK_ID,
                                        LD.ACCESS_NO,
                                        SLC.CLASS_NO,
                                        LD.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                        CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                       UPPER(CONCAT(IFNULL(LD.DOC_TITLE,LD.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        LD.NOTES,
                                       LD.PHYDESC,
                                       LD.ILLUSTRATE,
                                        CONCAT(IFNULL(SS.STATUS,''),' - ',IFNULL(SS.STATUS_CODE,'')) STATUS,
                                        LD.PRICE,
                                        DATE_FORMAT(LD.DOP,'%Y/%m/%d') AS DOP,
                                        LD.ISBN_NO,
                                        LD.DISCODE,
                                        LD.NAME_OF_PUBLISHER,
                                        LD.PLACE_OF_PUBLISHER,
                                        CONCAT(IFNULL(SLS.SERIES_NAME,''),' - ',IFNULL(SLS.SERIES_CODE,'')) AS SERIES,
                                        LD.RFID,
                                        CONCAT(IFNULL(SLL.LIB_DESCRIPTION,''),' - ',IFNULL(SLL.LIB_CODE,'')) AS LIB_CODE,
                                        LD.BOOK_PUBLISH_YEAR,
                                            LBR.RESERVE_ID,
                                        LBR.USER_ID,
                                        DATE_FORMAT(LBR.RESERVE_DATE,'%Y-%m-%d') AS RESERVE_DATE
                                    FROM
                                        LI_BOOK_DETAILS AS LD
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = LD.LANGUAGE
                                            LEFT JOIN
                                        SUP_LI_STATUS AS SS ON SS.STATUS_ID = LD.STATUS
                                            AND SS.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=LD.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=LD.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=LD.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=LD.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=LD.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=LD.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_LI_CLASS_NO AS SLC ON SLC.CLASS_NO_ID = LD.CLASS_NO
                                            AND SLC.IS_DELETED != 1
                                            LEFT JOIN
                                            SUP_LI_SERIES AS SLS ON SLS.SERIES_ID=LD.SERIES
                                            AND SLS.IS_DELETED!=1
                                            
                                            LEFT JOIN
                                            SUP_LI_LIB_CODE AS SLL ON SLL.LIB_CODE_ID=LD.LIB_CODE
                                            AND SLL.IS_DELETED!=1
                                            
                                            INNER JOIN
                                             LI_BOOK_RESERVE AS LBR ON LBR.BOOK_ID = LD.BOOK_ID
                                             AND LBR.IS_APPROVED!=1
                                             AND LBR.IS_COLLECTED!=1
                                            AND LBR.IS_DELETED != 1
                                            AND LBR.ACADEMIC_YEAR=?AC_YEAR
                                            
                                            
                                            
                                    WHERE

                                      LD.BOOK_STATUS =5
                                        AND LD.IS_DELETED != 1
                                        AND LBR.USER_ID=?USER_ID
                                        AND LD.STATUS!=3 GROUP BY LBR.RESERVE_ID
                                ORDER BY LBR.RESERVE_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchAllRenewedBooks:
                    {
                        sQuery = @"SELECT 

                                        LD.BOOK_ID,
                                        LD.ACCESS_NO,
                                        SLC.CLASS_NO,
                                        LD.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                        CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                       UPPER(CONCAT(IFNULL(LD.DOC_TITLE,LD.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        LD.NOTES,
                                       LD.PHYDESC,
                                       LD.ILLUSTRATE,
                                        CONCAT(IFNULL(SS.STATUS,''),' - ',IFNULL(SS.STATUS_CODE,'')) STATUS,
                                        LD.PRICE,
                                        DATE_FORMAT(LD.DOP,'%Y/%m/%d') AS DOP,
                                        LD.ISBN_NO,
                                        LD.DISCODE,
                                        LD.NAME_OF_PUBLISHER,
                                        LD.PLACE_OF_PUBLISHER,
                                        CONCAT(IFNULL(SLS.SERIES_NAME,''),' - ',IFNULL(SLS.SERIES_CODE,'')) AS SERIES,
                                        LD.RFID,
                                        CONCAT(IFNULL(SLL.LIB_DESCRIPTION,''),' - ',IFNULL(SLL.LIB_CODE,'')) AS LIB_CODE,
                                        LD.BOOK_PUBLISH_YEAR,
                                            LI.ISSUE_ID,
                                        LI.USER_ID,
                                        CONCAT(IFNULL(LU.FIRST_NAME,''),' ',
                                        IFNULL(LU.LAST_NAME,'')) AS FIRST_NAME,
                                        LU.BOID,
                                         M.CATEGORY_NAME,
                                         D.DEGREE,
                                        DATE_FORMAT(LI.ISSUE_DATE,'%Y-%m-%d') AS ISSUE_DATE
                                    FROM
                                        LI_BOOK_DETAILS AS LD
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = LD.LANGUAGE
                                            LEFT JOIN
                                        SUP_LI_STATUS AS SS ON SS.STATUS_ID = LD.STATUS
                                            AND SS.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=LD.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=LD.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=LD.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=LD.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=LD.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=LD.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_LI_CLASS_NO AS SLC ON SLC.CLASS_NO_ID = LD.CLASS_NO
                                            AND SLC.IS_DELETED != 1
                                            LEFT JOIN
                                            SUP_LI_SERIES AS SLS ON SLS.SERIES_ID=LD.SERIES
                                            AND SLS.IS_DELETED!=1
                                            
                                            LEFT JOIN
                                            SUP_LI_LIB_CODE AS SLL ON SLL.LIB_CODE_ID=LD.LIB_CODE
                                            AND SLL.IS_DELETED!=1
                                            
                                            INNER JOIN
                                             LI_BOOK_ISSUE AS LI ON LI.BOOK_ID = LD.BOOK_ID
                                            AND LI.IS_DELETED != 1
                                            AND LI.ACADEMIC_YEAR=?AC_YEAR
                                            AND LI.STATUS!=2
                                            AND LI.IS_RENEWED=1
                                            AND LI.IS_RENEW_CANCELLED!=1
                                            
                                            INNER JOIN
                                            LI_USER_DETAILS AS LU ON LU.USER_ID=LI.USER_ID
                                            AND LU.IS_DELETED!=1
                                             INNER JOIN
                                        SUP_DEGREE AS D ON D.DEGREE_ID = LU.DEGREE
                                            INNER JOIN
                                        SUP_LI_MEMBER_CATEGORY AS M ON M.CATEGORY_ID = LU.MEMBER_TYPE
                                           
                                            
                                    WHERE

                                      
                                         LD.IS_DELETED != 1
                                        AND LD.STATUS!=3 GROUP BY LI.ISSUE_ID
                                ORDER BY LI.ISSUE_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateBookReserveStatusById:
                    {
                        sQuery = @"UPDATE LI_BOOK_RESERVE 
                                     SET IS_COLLECTED=?IS_COLLECTED ,
                                         COLLECTED_DATE=NOW()
                                     WHERE 
                                     RESERVE_ID=?RESERVE_ID 
                                     AND IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.UpdateBookApprovedById:
                    {
                        sQuery = @"UPDATE LI_BOOK_RESERVE 
                                     SET APPROVED_DATE=NOW() ,
                                         IS_APPROVED=?IS_APPROVED
                                     WHERE 
                                     RESERVE_ID=?RESERVE_ID 
                                     AND IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.FetchAllApprovedBooksByDate:
                    {
                        sQuery = @"SELECT 
                                    RESERVE_ID,
                                    BOOK_ID,
                                    DATE_FORMAT(RESERVE_DATE, '%Y-%m-%d') AS RESERVE_DATE,
                                    USER_ID,
                                    REASON,
                                    DATE_FROM,
                                    DATE_TO,
                                    ACADEMIC_YEAR,
                                    IS_APPROVED,
                                    IS_COLLECTED,
                                    IS_ACTIVE,
                                    IS_DELETED
                                FROM
                                    LI_BOOK_RESERVE
                                WHERE
                                    IS_APPROVED = 1 AND IS_COLLECTED != 1
                                        AND DATE_FORMAT(APPROVED_DATE, '%Y-%m-%d') = DATE_FORMAT(?APPROVED_DATE, '%Y-%m-%d')
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchFailedtoTakeBookUsersWithingOneday:
                    {
                        sQuery = @"SELECT 
                                        BR.RESERVE_ID,
                                        BR.BOOK_ID,
                                        DATE_FORMAT(FROM_UNIXTIME(BR.APPROVED_DATE),
                                                '%Y-%m-%d') AS APPROVED_DATE,
                                        C.DAY_ID,
                                        DATE_FORMAT(FROM_UNIXTIME(BR.ENTRY_DATE), '%Y-%m-%d') AS ENTRY_DATE
                                    FROM
                                        LI_BOOK_RESERVE AS BR
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = BR.BOOK_ID
                                            INNER JOIN
                                        CALENDER AS C ON C.DAY_ORDER_DATE = DATE_FORMAT(FROM_UNIXTIME(BR.APPROVED_DATE),
                                                '%Y-%m-%d')
                                            AND C.IS_DELETED != 1
                                    WHERE
                                           BR.IS_DELETED != 1
                                           AND IS_SYSTEM_CANCELLED != 1
                                           AND BR.IS_COLLECTED != 1
                                           AND BR.IS_CANCELLED!=1
                                            AND BR.IS_APPROVED=1
                                            AND DATE_FORMAT(NOW(), '%Y-%m-%d') > DATE_FORMAT(FROM_UNIXTIME(BR.APPROVED_DATE),
                                                '%Y-%m-%d')
                                            AND BR.IS_USER_CANCELLED != 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchAllReserveRequestedTransaction:
                    {
                        sQuery = @"SELECT 
                                        BR.RESERVE_ID,
                                        UT.USER_TYPE_NAME,
										SR.ROLE_NAME AS ROLE,
                                        CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                        UPPER(CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                        '  ',
                                                        IFNULL(BI.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
										BI.BOOK_NO,
                                        BI.ACCESS_NO,
                                         CONCAT(IFNULL(BI.PERSONAL_AUTHOR1, ''),
                                                '  ',
                                                IFNULL(BI.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                       date_format(from_unixtime(BR.ENTRY_DATE),'%d-%m-%Y') AS ENTRY_DATE
                                       
									
                                    FROM
                                        LI_BOOK_RESERVE AS BR
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = BR.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = BR.USER_ID
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SPI.PROGRAM_ID
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USER_ID = SPI.STUDENT_ID
                                            AND UA.IS_DELETED != 1
                                            AND UA.USER_TYPE = BR.USER_TYPE
                                            INNER JOIN 
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=UA.USER_TYPE
                                              INNER JOIN
                                            SUP_ROLE AS SR ON SR.ROLE_ID=BR.ROLE
                                            
                                    WHERE
                                        BR.IS_DELETED != 1
                                            AND BR.IS_CANCELLED != 1
                                            AND BR.IS_USER_CANCELLED!= 1 
                                            AND BR.IS_APPROVED!=1
                                            AND BR.ACADEMIC_YEAR=?AC_YEAR
                                    UNION SELECT 
                                        BR.RESERVE_ID,
                                        UT.USER_TYPE_NAME,
										SR.ROLE_NAME AS ROLE,
                                        CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(STF.LAST_NAME, '')) AS FIRST_NAME,
                                        UPPER(CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                        '  ',
                                                        IFNULL(BI.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
										BI.BOOK_NO,
                                        BI.ACCESS_NO,
                                         CONCAT(IFNULL(BI.PERSONAL_AUTHOR1, ''),
                                                '  ',
                                                IFNULL(BI.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                       date_format(from_unixtime(BR.ENTRY_DATE),'%d-%m-%Y') AS ENTRY_DATE
                                    FROM
                                        LI_BOOK_RESERVE AS BR
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = BR.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                           
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = BR.USER_ID
                                            AND STF.IS_DELETED != 1
                                            AND STF.IS_LEFT != 1
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                            AND UA.IS_DELETED != 1
                                            AND UA.USER_TYPE = BR.USER_TYPE
                                            INNER JOIN 
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=UA.USER_TYPE
                                              INNER JOIN
                                            SUP_ROLE AS SR ON SR.ROLE_ID=BR.ROLE
                                            
                                    WHERE
                                        BR.IS_DELETED != 1
                                            AND BR.IS_CANCELLED != 1
                                            AND BR.IS_USER_CANCELLED!= 1 
                                            AND BR.IS_APPROVED!=1
											AND BR.ACADEMIC_YEAR=?AC_YEAR
                                    UNION SELECT 
                                        BR.RESERVE_ID,
                                        UT.USER_TYPE_NAME,
										'' AS ROLE,
                                        CONCAT(IFNULL(UI.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(UI.LAST_NAME, '')) AS FIRST_NAME,
                                        UPPER(CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                        '  ',
                                                        IFNULL(BI.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
										BI.BOOK_NO,
                                        BI.ACCESS_NO,
                                         CONCAT(IFNULL(BI.PERSONAL_AUTHOR1, ''),
                                                '  ',
                                                IFNULL(BI.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                       date_format(from_unixtime(BR.ENTRY_DATE),'%d-%m-%Y') AS ENTRY_DATE
                                    FROM
                                        LI_BOOK_RESERVE AS BR
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = BR.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                            INNER JOIN
                                        LI_EXTERNAL_USER_INFO AS UI ON UI.USER_ID = BR.USER_ID
                                            AND UI.USER_TYPE = BR.USER_TYPE
                                            INNER JOIN 
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=UI.USER_TYPE
				
                                    WHERE
                                        BR.IS_DELETED != 1
                                            AND BR.IS_CANCELLED != 1
                                            AND BR.IS_USER_CANCELLED!= 1 
                                            AND BR.IS_APPROVED!=1
											AND BR.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookInfoByReserveId:
                    {
                        sQuery = @"SELECT 
                                        BI.BOOK_ID
                                    FROM
                                        LI_BOOK_RESERVE AS BR
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = BR.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                    WHERE
                                        BR.IS_DELETED != 1
                                        AND BR.RESERVE_ID=?RESERVE_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateLiBookReserveApprovedSmsSent:
                    {
                        sQuery = @"UPDATE LI_BOOK_RESERVE SET IS_ASMS_SENT=?IS_ASMS_SENT WHERE RESERVE_ID=?RESERVE_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateLiBookReserveCancelSmsSent:
                    {
                        sQuery = @"UPDATE LI_BOOK_RESERVE SET IS_CSMS_SENT=?IS_CSMS_SENT WHERE RESERVE_ID=?RESERVE_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateReserveApprovedByReserveId:
                    {
                        sQuery = @"UPDATE LI_BOOK_RESERVE SET IS_APPROVED=?IS_APPROVED,APPROVED_BY=?APPROVED_BY,APPROVED_DATE=unix_timestamp() WHERE RESERVE_ID=?RESERVE_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateReserveCancelledByReserveId:
                    {
                        sQuery = @"UPDATE LI_BOOK_RESERVE SET IS_CANCELLED=?IS_CANCELLED,CANCEL_REASON=?CANCEL_REASON,CANCELLED_BY=?CANCELLED_BY,CANCELLED_DATE=unix_timestamp() WHERE RESERVE_ID=?RESERVE_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateReserveCollectedByReserveId:
                    {
                        sQuery = @"UPDATE LI_BOOK_RESERVE SET IS_COLLECTED=?IS_COLLECTED,COLLECTED_BY=?COLLECTED_BY,COLLECTED_DATE=unix_timestamp() WHERE RESERVE_ID=?RESERVE_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateSystemReserveCancelByReserveId:
                    {
                        sQuery = @"UPDATE LI_BOOK_RESERVE SET IS_SYSTEM_CANCELLED=?IS_SYSTEM_CANCELLED WHERE RESERVE_ID=?RESERVE_ID;";
                        break;
                    }
                #endregion

                #region Rack Info Maintain
                case LibrarySQLCommands.ListRackInfo:
                    {
                        sQuery = @"SELECT 
                                        R.RACK_ID,
                                        B.BLOCK_NAME,
                                        R.BOW_NO,
                                        R.SECTION_NO,
                                        R.RACK_NO,
                                        R.POSITION
                                    FROM
                                        LI_RACK_INFO AS R
                                            INNER JOIN
                                        SUP_LI_BLOCK AS B ON B.BLOCK_ID = R.BLOCK_ID
                                    WHERE
                                        R.IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchRackInfoById:
                    {
                        sQuery = @"SELECT 
                                        R.RACK_ID,
                                        R.BLOCK_ID,
                                        R.BOW_NO,
                                        R.SECTION_NO,
                                        R.RACK_NO,
                                        R.POSITION
                                    FROM
                                        LI_RACK_INFO AS R
                                    WHERE
                                        R.RACK_ID = ?RACK_ID;";
                        break;
                    }
                case LibrarySQLCommands.SaveRackInfo:
                    {

                        sQuery = @"INSERT INTO LI_RACK_INFO
                                    (
                                        RACK_ID,
                                        BLOCK_ID,
                                        BOW_NO,
                                        SECTION_NO,
                                        RACK_NO,
                                        POSITION,
                                        ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?RACK_ID,
                                        ?BLOCK_ID,
                                        ?BOW_NO,
                                        ?SECTION_NO,
                                        ?RACK_NO,
                                        ?POSITION,
                                        ?AC_YEAR
                                    );";
                        break;
                    }
                case LibrarySQLCommands.UpdateRackInfo:
                    {
                        sQuery = @"UPDATE LI_RACK_INFO
                                    SET
                                    BLOCK_ID = ?BLOCK_ID,
                                    BOW_NO = ?BOW_NO,
                                    SECTION_NO = ?SECTION_NO,
                                    RACK_NO = ?RACK_NO,
                                    POSITION = ?POSITION
                                    WHERE RACK_ID = ?RACK_ID;";
                        break;
                    }
                case LibrarySQLCommands.DeleteRackInfo:
                    {
                        sQuery = @"UPDATE LI_RACK_INFO SET IS_DELETED=1 WHERE RACK_ID = ?RACK_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchRackInfoByBlockandRow:
                    {
                        sQuery = @"SELECT 
                                    RACK_ID,
                                    BLOCK_ID
                                FROM
                                    LI_RACK_INFO
                                WHERE
                                    BLOCK_ID =?BLOCK_ID 
                                        AND RACK_NO=?RACK_NO
                                        AND SECTION_NO=?SECTION_NO
                                        AND BOW_NO=?BOW_NO
                                        AND POSITION=?POSITION
                                        AND IS_DELETED!=1;";
                        break;
                    }
                #endregion

                #region OPAC
                case LibrarySQLCommands.FetchBookDetailsByAuthor:
                    {
                        sQuery = @"SELECT 
                                        B.BOOK_ID,
                                        B.ACCESS_NO,
                                        B.CLASS_NO,
                                        B.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                          CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE,B.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        NOTES,
                                        ILLUSTRATE,
                                        PRICE,
                                        ISBN_NO,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        BOOK_PUBLISH_YEAR,
                                        RFID
                                    FROM
                                        LI_BOOK_DETAILS AS B
		                                    INNER JOIN
	                                    SUP_LANGUAGE AS L ON L.LANGUAGE_ID=B.LANGUAGE
                                        
                                        INNER JOIN
                                        SUP_LI_STATUS AS SLS ON SLS.STATUS_ID=B.STATUS
                                        AND SLS.IS_DELETED!=1
		                               LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=B.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=B.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=B.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=B.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=B.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=B.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
										
                                    WHERE
                                         B.IS_DELETED!=1
                                        AND B.PERSONAL_AUTHOR1=?PERSONAL_AUTHOR1
                                        AND B.STATUS!=3 GROUP BY B.BOOK_ID ORDER BY B.BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsByDocTitle:
                    {
                        sQuery = @"SELECT 
                                        B.BOOK_ID,
                                        B.ACCESS_NO,
                                        B.CLASS_NO,
                                        B.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                          CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE,B.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        NOTES,
                                        ILLUSTRATE,
                                        PRICE,
                                        ISBN_NO,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        BOOK_PUBLISH_YEAR,
                                        RFID
                                    FROM
                                        LI_BOOK_DETAILS AS B
		                                    INNER JOIN
	                                    SUP_LANGUAGE AS L ON L.LANGUAGE_ID=B.LANGUAGE
                                        
                                        INNER JOIN
                                        SUP_LI_STATUS AS SLS ON SLS.STATUS_ID=B.STATUS
                                        AND SLS.IS_DELETED!=1
		                               LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=B.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=B.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=B.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=B.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=B.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=B.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
										
                                    WHERE
                                         B.IS_DELETED!=1
                                        AND B.DOC_TITLE like '%?DOC_TITLE%'
                                        AND B.STATUS!=3 GROUP BY B.BOOK_ID ORDER BY B.BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsBySeries:
                    {
                        sQuery = @"SELECT 
                                        B.BOOK_ID,
                                        B.ACCESS_NO,
                                        B.CLASS_NO,
                                        B.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                          CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE,B.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        NOTES,
                                        ILLUSTRATE,
                                        PRICE,
                                        ISBN_NO,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        BOOK_PUBLISH_YEAR,
                                        RFID
                                    FROM
                                        LI_BOOK_DETAILS AS B
		                                    INNER JOIN
	                                    SUP_LANGUAGE AS L ON L.LANGUAGE_ID=B.LANGUAGE
                                        
                                        INNER JOIN
                                        SUP_LI_STATUS AS SLS ON SLS.STATUS_ID=B.STATUS
                                        AND SLS.IS_DELETED!=1
		                               LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=B.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=B.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=B.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=B.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=B.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=B.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
										
                                    WHERE
                                         B.IS_DELETED!=1
                                        AND B.SERIES=?SERIES
                                        AND B.STATUS!=3 GROUP BY B.BOOK_ID ORDER BY B.BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsByAnywords:
                    {
                        sQuery = @"SELECT 
                                        B.BOOK_ID,
                                        B.ACCESS_NO,
                                        B.CLASS_NO,
                                        B.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                          CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE,B.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        NOTES,
                                        ILLUSTRATE,
                                        PRICE,
                                        ISBN_NO,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        BOOK_PUBLISH_YEAR,
                                        RFID
                                    FROM
                                        LI_BOOK_DETAILS AS B
		                                    INNER JOIN
	                                    SUP_LANGUAGE AS L ON L.LANGUAGE_ID=B.LANGUAGE
                                        
                                        INNER JOIN
                                        SUP_LI_STATUS AS SLS ON SLS.STATUS_ID=B.STATUS
                                        AND SLS.IS_DELETED!=1
		                               LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=B.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=B.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=B.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=B.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=B.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=B.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
										
                                    WHERE
                                         B.IS_DELETED!=1
                                        AND B.DOC_TITLE like '%?DOC_TITLE%'
                                        AND B.STATUS!=3 GROUP BY B.BOOK_ID ORDER BY B.BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsByKeywords:
                    {
                        sQuery = @"SELECT 
                                        B.BOOK_ID,
                                        B.ACCESS_NO,
                                        B.CLASS_NO,
                                        B.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                          CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE,B.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        NOTES,
                                        ILLUSTRATE,
                                        PRICE,
                                        ISBN_NO,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        BOOK_PUBLISH_YEAR,
                                        RFID
                                    FROM
                                        LI_BOOK_DETAILS AS B
		                                    INNER JOIN
	                                    SUP_LANGUAGE AS L ON L.LANGUAGE_ID=B.LANGUAGE
                                        
                                        INNER JOIN
                                        SUP_LI_STATUS AS SLS ON SLS.STATUS_ID=B.STATUS
                                        AND SLS.IS_DELETED!=1
		                               LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=B.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=B.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=B.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=B.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=B.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=B.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
										
                                    WHERE
                                         B.IS_DELETED!=1
                                        AND B.KEYWORD=?KEYWORD
                                        AND B.STATUS!=3 GROUP BY B.BOOK_ID ORDER BY B.BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsByIsbnForopac:
                    {
                        sQuery = @"SELECT 
                                        B.BOOK_ID,
                                        B.ACCESS_NO,
                                        B.CLASS_NO,
                                        B.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                          CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE,B.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        NOTES,
                                        ILLUSTRATE,
                                        PRICE,
                                        ISBN_NO,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        BOOK_PUBLISH_YEAR,
                                        RFID
                                    FROM
                                        LI_BOOK_DETAILS AS B
		                                    INNER JOIN
	                                    SUP_LANGUAGE AS L ON L.LANGUAGE_ID=B.LANGUAGE
                                        
                                        INNER JOIN
                                        SUP_LI_STATUS AS SLS ON SLS.STATUS_ID=B.STATUS
                                        AND SLS.IS_DELETED!=1
		                               LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=B.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=B.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=B.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=B.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=B.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=B.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
										
                                    WHERE
                                         B.IS_DELETED!=1
                                        AND B.ISBN_NO LIKE '?ISBN_NO%'
                                        AND B.STATUS!=3 GROUP BY B.BOOK_ID ORDER BY B.BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsByDocType:
                    {
                        sQuery = @"SELECT 
                                        B.BOOK_ID,
                                        B.ACCESS_NO,
                                        B.CLASS_NO,
                                        B.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                          CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE,B.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        NOTES,
                                        ILLUSTRATE,
                                        PRICE,
                                        ISBN_NO,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        BOOK_PUBLISH_YEAR,
                                        RFID
                                    FROM
                                        LI_BOOK_DETAILS AS B
		                                    INNER JOIN
	                                    SUP_LANGUAGE AS L ON L.LANGUAGE_ID=B.LANGUAGE
                                        
                                        INNER JOIN
                                        SUP_LI_STATUS AS SLS ON SLS.STATUS_ID=B.STATUS
                                        AND SLS.IS_DELETED!=1
		                               LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=B.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=B.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=B.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=B.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=B.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=B.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
										
                                    WHERE
                                         B.IS_DELETED!=1
                                        AND B.DOC_TYPE=?DOC_TYPE
                                        AND B.STATUS!=3 GROUP BY B.BOOK_ID ORDER BY B.BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsBylanguage:
                    {
                        sQuery = @"SELECT 
                                        B.BOOK_ID,
                                        B.ACCESS_NO,
                                        B.CLASS_NO,
                                        B.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                          CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE,B.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        NOTES,
                                        ILLUSTRATE,
                                        PRICE,
                                        ISBN_NO,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        BOOK_PUBLISH_YEAR,
                                        RFID
                                    FROM
                                        LI_BOOK_DETAILS AS B
		                                    INNER JOIN
	                                    SUP_LANGUAGE AS L ON L.LANGUAGE_ID=B.LANGUAGE
                                        
                                        INNER JOIN
                                        SUP_LI_STATUS AS SLS ON SLS.STATUS_ID=B.STATUS
                                        AND SLS.IS_DELETED!=1
		                               LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=B.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=B.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=B.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=B.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=B.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=B.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
										
                                    WHERE
                                         B.IS_DELETED!=1
                                        AND B.LANGUAGE=?LANGUAGE
                                        AND B.STATUS!=3 GROUP BY B.BOOK_ID ORDER BY B.BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsByPublisher:
                    {
                        sQuery = @"SELECT 
                                        B.BOOK_ID,
                                        B.ACCESS_NO,
                                        B.CLASS_NO,
                                        B.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                          CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE,B.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        NOTES,
                                        ILLUSTRATE,
                                        PRICE,
                                        ISBN_NO,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        BOOK_PUBLISH_YEAR,
                                        RFID
                                    FROM
                                        LI_BOOK_DETAILS AS B
		                                    INNER JOIN
	                                    SUP_LANGUAGE AS L ON L.LANGUAGE_ID=B.LANGUAGE
                                        
                                        INNER JOIN
                                        SUP_LI_STATUS AS SLS ON SLS.STATUS_ID=B.STATUS
                                        AND SLS.IS_DELETED!=1
		                               LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=B.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=B.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=B.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=B.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=B.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=B.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
										
                                    WHERE
                                         B.IS_DELETED!=1
                                        AND B.NAME_OF_PUBLISHER LIKE '%?NAME_OF_PUBLISHER%'
                                        AND B.STATUS!=3  GROUP BY B.BOOK_ID ORDER BY B.BOOK_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchBookDetailsByAccessNoforopac:
                    {
                        sQuery = @"SELECT 
                                        B.BOOK_ID,
                                        B.ACCESS_NO,
                                        B.CLASS_NO,
                                        B.BOOK_NO,
                                        L.LANGUAGE_NAME AS LANGUAGE,
                                          CONCAT(IFNULL(LA.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA.LAST_NAME, '')) AS PERSONAL_AUTHOR1,
                                             CONCAT(IFNULL(LA1.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA1.LAST_NAME, '')) AS PERSONAL_AUTHOR2,
                                             CONCAT(IFNULL(LA2.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA2.LAST_NAME, '')) AS JOINED_AUTHOR1,
                                               CONCAT(IFNULL(LA3.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA3.LAST_NAME, '')) AS JOINED_AUTHOR2,
                                               CONCAT(IFNULL(LA4.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA4.LAST_NAME, '')) AS CORPORATE_AUTHOR1,
                                             CONCAT(IFNULL(LA5.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LA5.LAST_NAME, '')) AS CORPORATE_AUTHOR2,
                                        UPPER(CONCAT(IFNULL(B.DOC_TITLE,B.PARALLEL_TITLE))) AS BOOK_TITLE,
                                        NOTES,
                                        ILLUSTRATE,
                                        PRICE,
                                        ISBN_NO,
                                        NAME_OF_PUBLISHER,
                                        PLACE_OF_PUBLISHER,
                                        BOOK_PUBLISH_YEAR,
                                        RFID
                                    FROM
                                        LI_BOOK_DETAILS AS B
		                                    INNER JOIN
	                                    SUP_LANGUAGE AS L ON L.LANGUAGE_ID=B.LANGUAGE
                                        
                                        INNER JOIN
                                        SUP_LI_STATUS AS SLS ON SLS.STATUS_ID=B.STATUS
                                        AND SLS.IS_DELETED!=1
		                               LEFT JOIN
                                        LI_AUTHOR_INFO AS LA ON LA.AUTHOR_ID=B.PERSONAL_AUTHOR1
                                            AND LA.IS_DELETED != 1
                                            LEFT JOIN
                                        LI_AUTHOR_INFO AS LA1 ON LA1.AUTHOR_ID=B.PERSONAL_AUTHOR2
                                            AND LA1.IS_DELETED != 1
                                             LEFT JOIN
                                        LI_AUTHOR_INFO AS LA2 ON LA2.AUTHOR_ID=B.JOINED_AUTHOR1
                                            AND LA2.IS_DELETED != 1
                                              LEFT JOIN
                                        LI_AUTHOR_INFO AS LA3 ON LA3.AUTHOR_ID=B.JOINED_AUTHOR2
                                            AND LA3.IS_DELETED != 1
                                            LEFT JOIN
                                               LI_AUTHOR_INFO AS LA4 ON LA4.AUTHOR_ID=B.CORPORATE_AUTHOR1
                                            AND LA4.IS_DELETED != 1
                                             LEFT JOIN
                                               LI_AUTHOR_INFO AS LA5 ON LA5.AUTHOR_ID=B.CORPORATE_AUTHOR2
                                            AND LA5.IS_DELETED != 1
										
                                    WHERE
                                         B.IS_DELETED!=1
                                        AND B.ACCESS_NO=?ACCESS_NO
                                        AND B.STATUS!=3 GROUP BY B.BOOK_ID ORDER BY B.BOOK_ID;";
                        break;
                    }
                #endregion

                #region Book Circulation Cancel
                case LibrarySQLCommands.UpdateBookReserveDetailsForCancel:
                    {
                        sQuery = @"UPDATE
                                    LI_BOOK_RESERVE
                                    SET
                                    IS_CANCELLED=?IS_CANCELLED
                                    WHERE RESERVE_ID=?RESERVE_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateBookRenewDetailsForCancel:
                    {
                        sQuery = @"UPDATE
                                    LI_BOOK_ISSUE 
                                        SET
                                    IS_RENEW_CANCELLED=?IS_RENEW_CANCELLED
                                    WHERE
                                    ISSUE_ID=?ISSUE_ID;";
                        break;
                    }
                case LibrarySQLCommands.UpdateLiBookIssueForRenewedCancelSmsSend:
                    {
                        sQuery = @"UPDATE
                                    LI_BOOK_ISSUE
                                        SET
                                        IS_RENEW_CANCEL_SMS_SENT=?IS_RENEW_CANCEL_SMS_SENT 
                                        WHERE
                                        ISSUE_ID IN(?ISSUE_ID)
                                        AND IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.UpdateLiBookReserveForReserveCancelSmsSend:
                    {
                        sQuery = @"UPDATE
                                    LI_BOOK_RESERVE
                                    SET 
                                    IS_CANCEL_SMS_SENT=?IS_CANCEL_SMS_SENT 
                                    WHERE RESERVE_ID 
                                    IN(?RESERVE_ID)
                                    AND IS_DELETED!=1;";
                        break;
                    }
                #endregion

                #region Sms
                case LibrarySQLCommands.FetchUserForDueSendigSms:
                    {
                        sQuery = @"SELECT 
                                                CONCAT('Dear Mr/Miss ',
                                                        IFNULL(SPI.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(SPI.LAST_NAME, ''),
                                                        'Kindly Submit Your ',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                        ' Book By Tomorrow . Or Else You Have To Pay Fine Amount Per Day ',
                                                        LR.PER_DAY_DUE_COST,
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                IF(SPI.STU_MOBILENO IS NULL ,FR_MOBILE,SPI.STU_MOBILENO) AS MOBILE 
                                            FROM
                                                LI_BOOK_ISSUE_?AC_YEAR AS LI
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = LI.USER_ID
                                                    AND SPI.IS_DELETED != 1
                                                    AND SPI.IS_LEFT != 1
                                                    INNER JOIN
                                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SPI.PROGRAM_ID
                                                    INNER JOIN
                                                USER_ACCOUNT AS UA ON UA.USER_ID = SPI.STUDENT_ID
                                                    AND UA.IS_DELETED != 1
                                                    AND UA.USER_TYPE = LI.USER_TYPE
                                                    INNER JOIN
                                                LI_RULES AS LR ON LR.USER_TYPE = UA.USER_TYPE
                                                    AND LR.ROLE =LI.ROLE
                                                    AND LR.PROGRAMME_LEVEL = PG.APPTYPE_ID
                                                    AND LR.ACADEMIC_YEAR=?AC_YEAR
                                            WHERE
                                                LI.IS_DELETED != 1
                                                    AND LI.ISSUE_ID IN (?ISSUE_ID) 
                                            UNION SELECT 
                                                CONCAT('Dear Mr/Miss ',
                                                        IFNULL(STF.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(STF.LAST_NAME, ''),
                                                        'Kindly Submit Your ',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                        ' Book By Tomorrow . Or Else You Have To Pay Fine Amount Per Day ',
                                                        LR.PER_DAY_DUE_COST,
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                 UA.MOBILE 
                                            FROM
                                                LI_BOOK_ISSUE_?AC_YEAR AS LI
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = LI.USER_ID
                                                    AND STF.IS_DELETED != 1
                                                    AND STF.IS_LEFT != 1
                                                    INNER JOIN
                                                USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                                    AND UA.IS_DELETED != 1
                                                    AND UA.USER_TYPE = LI.USER_TYPE
                                                    INNER JOIN
                                                LI_RULES AS LR ON LR.USER_TYPE = UA.USER_TYPE
                                                    AND LR.ROLE = LI.ROLE
                                                    AND LR.ACADEMIC_YEAR=?AC_YEAR
        
                                            WHERE
                                                LI.IS_DELETED != 1
                                                    AND LI.ISSUE_ID IN (?ISSUE_ID) 
                                            UNION SELECT 
                                               CONCAT('Dear Mr/Miss ',
                                                        IFNULL(UI.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(UI.LAST_NAME, ''),
                                                        'Kindly Submit Your ',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                        ' Book By Tomorrow . Or Else You Have To Pay Fine Amount Per Day',
                                                        LR.PER_DAY_DUE_COST,
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                UI.MOBILE AS MOBILE 
                                            FROM
                                                LI_BOOK_ISSUE_?AC_YEAR AS LI
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                LI_EXTERNAL_USER_INFO AS UI ON UI.USER_ID = LI.USER_ID
                                                    AND UI.USER_TYPE = LI.USER_TYPE
                                                    INNER JOIN
                                                LI_RULES AS LR ON LR.USER_TYPE = UI.USER_TYPE
                                                    AND LR.IS_DELETED != 1
                                                      AND LR.ACADEMIC_YEAR=?AC_YEAR
                                            WHERE
                                                LI.IS_DELETED != 1
                                                    AND LI.ISSUE_ID IN (?ISSUE_ID);";
                        break;
                    }
                case LibrarySQLCommands.FetchUserForReserveCancelSendingSms:
                    {
                        sQuery = @"SELECT 
                                    LD.USER_ID,
                                    CONCAT(IFNULL(LD.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LD.LAST_NAME, '')) AS FIRST_NAME,
                                    CONCAT(IFNULL(LB.DOC_TITLE, ''),
                                            ' ',
                                            IFNULL(LB.PARALLEL_TITLE, '')) AS BOOK_TITLE,
                                    CONCAT('Dear Mr/Miss ',
                                            IFNULL(LD.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LD.LAST_NAME, ''),
                                            'Your Reserved ',
                                            CONCAT(IFNULL(LB.DOC_TITLE, ''),
                                                    ' ',
                                                    IFNULL(LB.PARALLEL_TITLE, '')),
                                            ' was cancelled due to some problem ',
                                            r.FINE_AMOUNT,
                                            ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                    LD.MOBILE
                                FROM
                                    LI_BOOK_RESERVE AS LI
                                        INNER JOIN
                                    LI_USER_DETAILS AS LD ON LD.USER_ID = LI.USER_ID
                                        AND LD.IS_DELETED != 1
                                        INNER JOIN
                                    LI_BOOK_DETAILS AS LB ON LB.BOOK_ID = LI.BOOK_ID
                                        AND LB.IS_DELETED != 1
                                        INNER JOIN
                                    li_rules AS r ON r.MEM_CATEGORY = ld.member_type
                                        AND r.is_deleted != 1
                                WHERE
                                    LI.RESERVE_ID IN (?RESERVE_ID)
                                    AND LI.IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.FetchUserForRenewedCancelSendingSms:
                    {
                        sQuery = @"SELECT 
                                    LD.USER_ID,
                                    CONCAT(IFNULL(LD.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LD.LAST_NAME, '')) AS FIRST_NAME,
                                    CONCAT(IFNULL(LB.DOC_TITLE, ''),
                                            ' ',
                                            IFNULL(LB.PARALLEL_TITLE, '')) AS BOOK_TITLE,
                                    CONCAT('Dear Mr/Miss ',
                                            IFNULL(LD.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LD.LAST_NAME, ''),
                                            'Your Renewed ',
                                            CONCAT(IFNULL(LB.DOC_TITLE, ''),
                                                    ' ',
                                                    IFNULL(LB.PARALLEL_TITLE, '')),
                                            ' was cancelled so please submit your book immediately ',
                                            r.FINE_AMOUNT,
                                            ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                    LD.MOBILE
                                FROM
                                    LI_BOOK_ISSUE AS LI
                                        INNER JOIN
                                    LI_USER_DETAILS AS LD ON LD.USER_ID = LI.USER_ID
                                        AND LD.IS_DELETED != 1
                                        INNER JOIN
                                    LI_BOOK_DETAILS AS LB ON LB.BOOK_ID = LI.BOOK_ID
                                        AND LB.IS_DELETED != 1
                                        INNER JOIN
                                    li_rules AS r ON r.MEM_CATEGORY = ld.member_type
                                        AND r.is_deleted != 1
                                WHERE
                                    LI.ISSUE_ID IN (?ISSUE_ID)
                                    AND LI.IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.FetchUserForIssueReturnConfirmedSendingSms:
                    {
                        sQuery = @"SELECT 
                                                CONCAT('Dear Mr/Miss ',
                                                        IFNULL(SPI.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(SPI.LAST_NAME, ''),
                                                        'Your Returned Book ',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                        'is Confirmed by admin Successfully.',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                IF(SPI.STU_MOBILENO IS NULL ,FR_MOBILE,SPI.STU_MOBILENO) AS MOBILE 
                                            FROM
                                                LI_BOOK_ISSUE_?AC_YEAR AS LI
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = LI.USER_ID
                                                    AND SPI.IS_DELETED != 1
                                                    AND SPI.IS_LEFT != 1
											         INNER JOIN
                                                USER_ACCOUNT AS UA ON UA.USER_ID = SPI.STUDENT_ID
                                                    AND UA.IS_DELETED != 1
                                                    AND UA.USER_TYPE = LI.USER_TYPE
                                            WHERE
                                                LI.IS_DELETED != 1
                                                    AND LI.ISSUE_ID IN (?ISSUE_ID) 
                                            UNION SELECT 
                                                CONCAT('Dear Mr/Miss ',
                                                        IFNULL(STF.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(STF.LAST_NAME, ''),
                                                       'Your Returned Book ',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                        'is Confirmed by admin Successfully.',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                UA.MOBILE 
                                            FROM
                                                LI_BOOK_ISSUE_?AC_YEAR AS LI
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = LI.USER_ID
                                                    AND STF.IS_DELETED != 1
                                                    AND STF.IS_LEFT != 1
                                                    INNER JOIN
                                                USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                                    AND UA.IS_DELETED != 1
                                                    AND UA.USER_TYPE = LI.USER_TYPE
        
                                            WHERE
                                                LI.IS_DELETED != 1
                                                    AND LI.ISSUE_ID IN (?ISSUE_ID) 
                                            UNION SELECT 
                                               CONCAT('Dear Mr/Miss ',
                                                        IFNULL(UI.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(UI.LAST_NAME, ''),
                                                      'Your Returned Book ',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                          'is Confirmed by admin Successfully.',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                UI.MOBILE AS MOBILE 
                                            FROM
                                                LI_BOOK_ISSUE_?AC_YEAR AS LI
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                LI_EXTERNAL_USER_INFO AS UI ON UI.USER_ID = LI.USER_ID
                                                    AND UI.USER_TYPE = LI.USER_TYPE
                                            WHERE
                                                LI.IS_DELETED != 1
                                                    AND LI.ISSUE_ID IN (?ISSUE_ID);";
                        break;
                    }
                case LibrarySQLCommands.FetchUserForIssueReturnDamageSendingSms:
                    {
                        sQuery = @"                           SELECT 
                                                CONCAT('Dear Mr/Miss ',
                                                        IFNULL(SPI.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(SPI.LAST_NAME, ''),
                                                        'Your Returned Book ',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                          'is Damaged So kindly pay fine ','?FINE',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                IF(SPI.STU_MOBILENO IS NULL ,FR_MOBILE,SPI.STU_MOBILENO) AS MOBILE 
                                            FROM
                                                LI_BOOK_ISSUE_?AC_YEAR AS LI
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = LI.USER_ID
                                                    AND SPI.IS_DELETED != 1
                                                    AND SPI.IS_LEFT != 1
											         INNER JOIN
                                                USER_ACCOUNT AS UA ON UA.USER_ID = SPI.STUDENT_ID
                                                    AND UA.IS_DELETED != 1
                                                    AND UA.USER_TYPE = LI.USER_TYPE
                                            WHERE
                                                LI.IS_DELETED != 1
                                                    AND LI.ISSUE_ID=?ISSUE_ID 
                                            UNION SELECT 
                                                CONCAT('Dear Mr/Miss ',
                                                        IFNULL(STF.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(STF.LAST_NAME, ''),
                                                       'Your Returned Book ',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                           'is Damaged So kindly pay fine ','?FINE',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                UA.MOBILE 
                                            FROM
                                                LI_BOOK_ISSUE_?AC_YEAR AS LI
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = LI.USER_ID
                                                    AND STF.IS_DELETED != 1
                                                    AND STF.IS_LEFT != 1
                                                    INNER JOIN
                                                USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                                    AND UA.IS_DELETED != 1
                                                    AND UA.USER_TYPE = LI.USER_TYPE
        
                                            WHERE
                                                LI.IS_DELETED != 1
                                                    AND LI.ISSUE_ID=?ISSUE_ID
                                            UNION SELECT 
                                               CONCAT('Dear Mr/Miss ',
                                                        IFNULL(UI.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(UI.LAST_NAME, ''),
                                                      'Your Returned Book ',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                           'is Damaged So kindly pay fine ','?FINE',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                UI.MOBILE AS MOBILE 
                                            FROM
                                                LI_BOOK_ISSUE_?AC_YEAR AS LI
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                LI_EXTERNAL_USER_INFO AS UI ON UI.USER_ID = LI.USER_ID
                                                    AND UI.USER_TYPE = LI.USER_TYPE
                                            WHERE
                                                LI.IS_DELETED != 1
                                                    AND LI.ISSUE_ID=?ISSUE_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchUserForIssueReturnMissingSendingSms:
                    {
                        sQuery = @"                   SELECT 
                                                CONCAT('Dear Mr/Miss ',
                                                        IFNULL(SPI.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(SPI.LAST_NAME, ''),
                                                        'Your Issued Book ',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                          'is Missed So kindly pay fine ','?FINE',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                IF(SPI.STU_MOBILENO IS NULL ,FR_MOBILE,SPI.STU_MOBILENO) AS MOBILE 
                                            FROM
                                                LI_BOOK_ISSUE_?AC_YEAR AS LI
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = LI.USER_ID
                                                    AND SPI.IS_DELETED != 1
                                                    AND SPI.IS_LEFT != 1
											         INNER JOIN
                                                USER_ACCOUNT AS UA ON UA.USER_ID = SPI.STUDENT_ID
                                                    AND UA.IS_DELETED != 1
                                                    AND UA.USER_TYPE = LI.USER_TYPE
                                            WHERE
                                                LI.IS_DELETED != 1
                                                    AND LI.ISSUE_ID=?ISSUE_ID 
                                            UNION SELECT 
                                                CONCAT('Dear Mr/Miss ',
                                                        IFNULL(STF.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(STF.LAST_NAME, ''),
                                                         'Your Issued Book ',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                             'is Missed So kindly pay fine ','?FINE',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                UA.MOBILE 
                                            FROM
                                                LI_BOOK_ISSUE_?AC_YEAR AS LI
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = LI.USER_ID
                                                    AND STF.IS_DELETED != 1
                                                    AND STF.IS_LEFT != 1
                                                    INNER JOIN
                                                USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                                    AND UA.IS_DELETED != 1
                                                    AND UA.USER_TYPE = LI.USER_TYPE
        
                                            WHERE
                                                LI.IS_DELETED != 1
                                                    AND LI.ISSUE_ID=?ISSUE_ID
                                            UNION SELECT 
                                               CONCAT('Dear Mr/Miss ',
                                                        IFNULL(UI.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(UI.LAST_NAME, ''),
                                                          'Your Issued Book ',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                             'is Missed So kindly pay fine ','?FINE',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                UI.MOBILE AS MOBILE 
                                            FROM
                                                LI_BOOK_ISSUE_?AC_YEAR AS LI
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                LI_EXTERNAL_USER_INFO AS UI ON UI.USER_ID = LI.USER_ID
                                                    AND UI.USER_TYPE = LI.USER_TYPE
                                            WHERE
                                                LI.IS_DELETED != 1
                                                    AND LI.ISSUE_ID=?ISSUE_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchUserForReserveApprovedSendingSms:
                    {
                        sQuery = @"SELECT 
                                                CONCAT('Dear Mr/Miss ',
                                                        IFNULL(SPI.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(SPI.LAST_NAME, ''),
                                                        'Your Reserved Book',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                        'is Approved by admin Successfully.',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                  IF(SPI.STU_MOBILENO IS NULL,FR_MOBILE,SPI.STU_MOBILENO) AS MOBILE 
                                            FROM
                                                LI_BOOK_RESERVE AS BR
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = BR.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = BR.USER_ID
                                                    AND SPI.IS_DELETED != 1
                                                    AND SPI.IS_LEFT != 1
											         INNER JOIN
                                                USER_ACCOUNT AS UA ON UA.USER_ID = SPI.STUDENT_ID
                                                    AND UA.IS_DELETED != 1
                                                    AND UA.USER_TYPE = BR.USER_TYPE
                                                    
                                            WHERE
                                                BR.IS_DELETED != 1
                                                    AND BR.RESERVE_ID IN (?RESERVE_ID) 
                                            UNION SELECT 
                                                 CONCAT('Dear Mr/Miss ',
                                                        IFNULL(STF.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(STF.LAST_NAME, ''),
                                                        'Your Reserved Book',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                        'is Approved by admin Successfully.',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                UA.MOBILE 
                                            FROM
                                                LI_BOOK_RESERVE AS BR
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = BR.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = BR.USER_ID
                                                    AND STF.IS_DELETED != 1
                                                    AND STF.IS_LEFT != 1
                                                    INNER JOIN
                                                USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                                    AND UA.IS_DELETED != 1
                                                    AND UA.USER_TYPE = BR.USER_TYPE
                                                    
                                            WHERE
                                               BR.IS_DELETED != 1
                                                    AND BR.RESERVE_ID IN (?RESERVE_ID) 
                                            UNION SELECT 
                                                 CONCAT('Dear Mr/Miss ',
                                                        IFNULL(UI.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(UI.LAST_NAME, ''),
                                                        'Your Reserved Book',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                        'is Approved by admin Successfully.',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                UI.MOBILE AS MOBILE 
                                            FROM
                                                LI_BOOK_RESERVE AS BR
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = BR.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                LI_EXTERNAL_USER_INFO AS UI ON UI.USER_ID = BR.USER_ID
                                                    AND UI.USER_TYPE = BR.USER_TYPE
                                            WHERE
                                                BR.IS_DELETED != 1
                                                    AND BR.RESERVE_ID IN (?RESERVE_ID);";
                        break;
                    }
                case LibrarySQLCommands.FetchUserForReserveCancellingSendingSms:
                    {
                        sQuery = @"SELECT 
                                                CONCAT('Dear Mr/Miss ',
                                                        IFNULL(SPI.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(SPI.LAST_NAME, ''),
                                                        'Your Reserved Book',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                        'is Cancelled by admin For This Reason ','?CANCEL_REASON',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                IF(SPI.STU_MOBILENO IS NULL ,FR_MOBILE,SPI.STU_MOBILENO) AS MOBILE 
                                            FROM
                                                LI_BOOK_RESERVE AS BR
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = BR.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = BR.USER_ID
                                                    AND SPI.IS_DELETED != 1
                                                    AND SPI.IS_LEFT != 1
											         INNER JOIN
                                                USER_ACCOUNT AS UA ON UA.USER_ID = SPI.STUDENT_ID
                                                    AND UA.IS_DELETED != 1
                                                    AND UA.USER_TYPE = BR.USER_TYPE
												
                                            WHERE
                                                BR.IS_DELETED != 1
                                                    AND BR.RESERVE_ID=?RESERVE_ID 
                                            UNION SELECT 
                                                 CONCAT('Dear Mr/Miss ',
                                                        IFNULL(STF.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(STF.LAST_NAME, ''),
                                                        'Your Reserved Book',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                           'is Cancelled by admin For This Reason ','?CANCEL_REASON',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                UA.MOBILE 
                                            FROM
                                                LI_BOOK_RESERVE AS BR
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = BR.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = BR.USER_ID
                                                    AND STF.IS_DELETED != 1
                                                    AND STF.IS_LEFT != 1
                                                    INNER JOIN
                                                USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                                    AND UA.IS_DELETED != 1
                                                    AND UA.USER_TYPE = BR.USER_TYPE
												
                                            WHERE
                                               BR.IS_DELETED != 1
                                                    AND BR.RESERVE_ID=?RESERVE_ID
                                            UNION SELECT 
                                                 CONCAT('Dear Mr/Miss ',
                                                        IFNULL(UI.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(UI.LAST_NAME, ''),
                                                        'Your Reserved Book',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                           'is Cancelled by admin For This Reason ?CANCEL_REASON.',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                UI.MOBILE AS MOBILE 
                                            FROM
                                                LI_BOOK_RESERVE AS BR
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = BR.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                LI_EXTERNAL_USER_INFO AS UI ON UI.USER_ID = BR.USER_ID
                                                    AND UI.USER_TYPE = BR.USER_TYPE
                                            WHERE
                                                BR.IS_DELETED != 1
                                                    AND BR.RESERVE_ID=?RESERVE_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchUserForSystemReserveCancellingSendingSms:
                    {
                        sQuery = @"SELECT 
                                                CONCAT('Dear Mr/Miss ',
                                                        IFNULL(SPI.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(SPI.LAST_NAME, ''),
                                                        'Your Reserved Book',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                        'is Cancelled by Automatically By System  Due to Not collecting book within a day!.',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                               IF(SPI.STU_MOBILENO IS NULL ,FR_MOBILE,SPI.STU_MOBILENO) AS MOBILE 
                                            FROM
                                                LI_BOOK_RESERVE AS BR
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = BR.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = BR.USER_ID
                                                    AND SPI.IS_DELETED != 1
                                                    AND SPI.IS_LEFT != 1
											         INNER JOIN
                                                USER_ACCOUNT AS UA ON UA.USER_ID = SPI.STUDENT_ID
                                                    AND UA.IS_DELETED != 1
                                                    AND UA.USER_TYPE = BR.USER_TYPE
                                                    
                                            WHERE
                                                BR.IS_DELETED != 1
                                                    AND BR.RESERVE_ID IN(?RESERVE_ID)
                                            UNION SELECT 
                                                 CONCAT('Dear Mr/Miss ',
                                                        IFNULL(STF.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(STF.LAST_NAME, ''),
                                                        'Your Reserved Book',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                              'is Cancelled by Automatically By System  Due to Not collecting book within a day!.',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                UA.MOBILE 
                                            FROM
                                                LI_BOOK_RESERVE AS BR
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = BR.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = BR.USER_ID
                                                    AND STF.IS_DELETED != 1
                                                    AND STF.IS_LEFT != 1
                                                    INNER JOIN
                                                USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                                    AND UA.IS_DELETED != 1
                                                    AND UA.USER_TYPE = BR.USER_TYPE
                                                    
                                            WHERE
                                               BR.IS_DELETED != 1
                                                    AND BR.RESERVE_ID IN(?RESERVE_ID)
                                            UNION SELECT 
                                                 CONCAT('Dear Mr/Miss ',
                                                        IFNULL(UI.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(UI.LAST_NAME, ''),
                                                        'Your Reserved Book',
                                                        CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                                ' ',
                                                                IFNULL(BI.PARALLEL_TITLE, '')),
                                                            'is Cancelled by Automatically By System  Due to Not collecting book within a day!.',
                                                        ' . By ',(SELECT COLLEGE_CODE FROM COLLEGE_DETAILS),' Library Thank You !') AS MESSAGE_CONTENT,
                                                UI.MOBILE AS MOBILE 
                                            FROM
                                                LI_BOOK_RESERVE AS BR
                                                    INNER JOIN
                                                LI_BOOK_INFO AS BI ON BI.BOOK_ID = BR.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                    INNER JOIN
                                                LI_EXTERNAL_USER_INFO AS UI ON UI.USER_ID = BR.USER_ID
                                                    AND UI.USER_TYPE = BR.USER_TYPE
                                            WHERE
                                                BR.IS_DELETED != 1
                                                    AND BR.RESERVE_ID IN(?RESERVE_ID);";
                        break;
                    }
                #endregion

                #region Common
                case LibrarySQLCommands.FetchAcademicSemesterByLiUserId:
                    {
                        sQuery = @"SELECT 
                                    S.SEMESTER
                                FROM
                                    LI_USER_DETAILS AS LD
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS STU ON STU.BOID_NUMBER = LD.BOID
                                        AND STU.IS_LEFT != 1
                                        AND STU.IS_DELETED != 1
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = STU.STUDENT_ID
                                        AND SC.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR =?AC_YEAR
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND CPP.IS_DELETED != 1
                                        INNER JOIN
                                    ACADEMIC_SEMESTER S ON S.PROGRAMME_GROUP_ID = CPP.PROGRAMME_GROUP_ID
                                        AND S.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND S.BATCH = SC.BATCH
                                        AND S.IS_DELETED != 1
                                        AND S.IS_ACTIVE = 1
                                WHERE
                                    LD.USER_ID =?USER_ID AND LD.IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchLiRuleByLiUserId:
                    {
                        sQuery = @"      SELECT 
                                        LD.USER_ID,
                                        LR.MAX_BOOKS
                                        FROM LI_USER_DETAILS AS LD 
                                        INNER JOIN
                                        STU_PERSONAL_INFO AS STU ON STU.BOID_NUMBER=LD.BOID
                                        AND STU.IS_DELETED!=1
                                        AND STU.IS_LEFT!=1
        
                                        INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID=STU.STUDENT_ID
                                        AND SC.IS_DELETED!=1
                                        AND SC.ACADEMIC_YEAR=?AC_YEAR
        
                                        INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID=SC.PROGRAMME_GROUP_ID
                                        AND CPP.IS_DELETED!=1
        
        
                                        INNER JOIN
                                        SUP_LI_MEMBER_CATEGORY AS SM ON SM.CATEGORY_ID=LD.MEMBER_TYPE
                                        AND SM.IS_DELETED!=1
        
                                        INNER JOIN
                                        LI_RULES AS LR ON LR.MEM_CATEGORY=SM.CATEGORY_ID
                                        AND LR.APPTYPE_ID=CPP.APPTYPE_ID
                                        AND LR.IS_DELETED!=1
        
                                        WHERE
                                        LD.USER_ID=?USER_ID
                                        AND LD.IS_DELETED!=1;";
                        break;
                    }
                case LibrarySQLCommands.FetchLiMemberTypeByLiUserId:
                    {
                        sQuery = @"
                                    SELECT 
                                    LU.MEMBER_TYPE
                                    FROM LI_USER_DETAILS AS LU
                                    INNER JOIN
                                    sup_li_member_category AS SM ON SM.CATEGORY_ID=LU.MEMBER_TYPE
                                    AND SM.IS_DELETED!=1

                                    WHERE
                                    LU.USER_ID=?USER_ID
                                    AND LU.IS_DELETED!=1;";
                        break;
                    }
                #endregion

                #region Miscellaneous
                case LibrarySQLCommands.FetchCalenderDetails:
                    {
                        sQuery = @"SELECT 
                                        DAY_ID,
                                        DATE_FORMAT(DAY_ORDER_DATE,'%Y-%m-%d') AS DAY_ORDER_DATE,
                                        DAY_TYPE,
                                        DAY_ORDER,
                                        DAY_ORDER_MONTH,
                                        IS_HOLIDAY,
                                        IF(CURDATE()=DATE_FORMAT(DAY_ORDER_DATE,'%Y-%m-%d'),'1','0') AS 'IS_CURDATE'
                                    FROM
                                        CALENDER
                                    WHERE
                                        ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                #endregion

                #region User Provisions 
                case LibrarySQLCommands.FetchLiUserInfoByRfidForUserProvisios:
                    {
                        sQuery = @"SELECT 
                                    CONCAT(IFNULL(FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LAST_NAME, '')) AS FIRST_NAME,
                                    BOID,
                                    MOBILE,
                                    SM.CATEGORY_NAME AS MEMBER_TYPE,
                                    RFID,
                                    DATE_FORMAT(DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                    LP.EXTRA_BOOKS,
                                    LI.USER_ID,
                                    LP.PROVISION_ID
                                FROM
                                    LI_USER_DETAILS AS LI
                                        INNER JOIN
                                    SUP_LI_MEMBER_CATEGORY AS SM ON SM.CATEGORY_ID = LI.MEMBER_TYPE
                                        AND SM.IS_DELETED != 1
	    
                                        LEFT JOIN 
                                        LI_USER_PROVISIONS AS LP ON LP.USER_ID=LI.USER_ID
                                        AND LP.IS_DELETED!=1
        
                                WHERE
                                    LI.RFID = ?RFID
                                        AND LI.IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchLiUserInfoByBoidForUserProvisios:
                    {
                        sQuery = @"SELECT 
                                    CONCAT(IFNULL(FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LAST_NAME, '')) AS FIRST_NAME,
                                    BOID,
                                    MOBILE,
                                    SM.CATEGORY_NAME AS MEMBER_TYPE,
                                    RFID,
                                    DATE_FORMAT(DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                    LP.EXTRA_BOOKS,
                                    LP.PROVISION_ID
                                FROM
                                    LI_USER_DETAILS AS LI
                                        INNER JOIN
                                    SUP_LI_MEMBER_CATEGORY AS SM ON SM.CATEGORY_ID = LI.MEMBER_TYPE
                                        AND SM.IS_DELETED != 1
	    
                                        LEFT JOIN 
                                        LI_USER_PROVISIONS AS LP ON LP.USER_ID=LI.USER_ID
                                        AND LP.IS_DELETED!=1
        
                                WHERE
                                    LI.BOID = ?BOID
                                        AND LI.IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.SaveLiUserProvisions:
                    {
                        sQuery = @"INSERT INTO LI_USER_PROVISIONS(USER_ID,EXTRA_BOOKS)
                                   VALUES(?USER_ID,?EXTRA_BOOKS);";
                        break;
                    }
                case LibrarySQLCommands.UpdateLiUserProvisions:
                    {
                        sQuery = @"UPDATE LI_USER_PROVISIONS SET USER_ID=?USER_ID,EXTRA_BOOKS=?EXTRA_BOOKS WHERE PROVISION_ID=?PROVISION_ID;";
                        break;
                    }
                case LibrarySQLCommands.DeleteLiUserProvisions:
                    {
                        sQuery = @"UPDATE LI_USER_PROVISIONS SET IS_DELETED=?IS_DELETED WHERE PROVISION_ID=?PROVISION_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchLiUserProvisionByUserId:
                    {
                        sQuery = @"SELECT PROVISION_ID FROM LI_USER_PROVISION WHERE USER_ID=?USER_ID AND IS_DELETED!=1;";
                        break;
                    }
                #endregion

                #region External User Info
                case LibrarySQLCommands.SaveExternalUser:
                    {
                        sQuery = @"INSERT INTO LI_EXTERNAL_USER_INFO
                                    (
	                                    USERNAME,
	                                    PASSWORD,
	                                    USER_TYPE,
	                                    BOID,
	                                    FIRST_NAME,
	                                    LAST_NAME,
	                                    EMAIL,
	                                    MOBILE,
	                                    DATE_OF_BIRTH,
	                                    DEGREE,
	                                    ADDRESS1,
	                                    ADDRESS2,
	                                    BLOOD_GROUP_ID,
	                                    REGISTERED_DATE,
	                                    ACADEMIC_YEAR,
	                                    PHOTO,
	                                    RFID,
	                                    STATE,
	                                    PINCODE,
	                                    CITY,
	                                    BAR_CODE
                                    )
                                    VALUES
                                    (
                                        ?USERNAME,
	                                    ?PASSWORD,
	                                    ?USER_TYPE,
	                                    ?BOID,
	                                    ?FIRST_NAME,
	                                    ?LAST_NAME,
	                                    ?EMAIL,
	                                    ?MOBILE,
	                                    ?DATE_OF_BIRTH,
	                                    ?DEGREE,
	                                    ?ADDRESS1,
	                                    ?ADDRESS2,
	                                    ?BLOOD_GROUP_ID,
	                                    ?REGISTERED_DATE,
	                                    ?AC_YEAR,
	                                    ?PHOTO,
	                                    ?RFID,
	                                    ?STATE,
	                                    ?PINCODE,
	                                    ?CITY,
	                                    ?BAR_CODE
                                    );";
                        break;
                    }
                case LibrarySQLCommands.UpdateExternalUser:
                    {
                        sQuery = @"UPDATE LI_EXTERNAL_USER_INFO
                                    SET
	                                    USERNAME = ?USERNAME,
	                                    PASSWORD = ?PASSWORD,
	                                    USER_TYPE = ?USER_TYPE,
	                                    BOID = ?BOID,
	                                    FIRST_NAME = ?FIRST_NAME,
	                                    LAST_NAME = ?LAST_NAME,
	                                    EMAIL = ?EMAIL,
	                                    MOBILE = ?MOBILE,
	                                    DATE_OF_BIRTH = ?DATE_OF_BIRTH,
	                                    DEGREE = ?DEGREE,
	                                    ADDRESS1 = ?ADDRESS1,
	                                    ADDRESS2 = ?ADDRESS2,
	                                    BLOOD_GROUP_ID = ?BLOOD_GROUP_ID,
	                                    RFID = ?RFID,
	                                    STATE = ?STATE,
	                                    PINCODE = ?PINCODE,
	                                    CITY = ?CITY,
	                                    BAR_CODE = ?BAR_CODE
                                    WHERE USER_ID = ?USER_ID;";
                        break;
                    }
                case LibrarySQLCommands.DeleteExternalUser:
                    {
                        sQuery = @"UPDATE LI_EXTERNAL_USER_INFO SET IS_DELETED=1 WHERE USER_ID=?USER_ID;";
                        break;
                    }
                #endregion

                #region CHEKIN
                case LibrarySQLCommands.FetchValidateCheckIn:
                    {
                        sQuery = @"SELECT CHECKIN_ID FROM LI_CHECKIN_INFO_?AC_YEAR WHERE USER_ID=?USER_ID AND USER_TYPE=?USER_TYPE AND DATE_FORMAT(CHECKIN_DATE,'%Y-%m-%d')=CURDATE() AND IS_CHECKOUT!=1;";
                        break;
                    }
                case LibrarySQLCommands.FetchValidateCheckInbulk:
                    {
                        sQuery = @"SELECT CHECKIN_ID FROM LI_CHECKIN_INFO_?AC_YEAR WHERE USER_ID=?USER_ID AND USER_TYPE=?USER_TYPE AND DATE_FORMAT(CHECKIN_DATE,'%Y-%m-%d')=?CHECKIN_DATE AND IS_CHECKOUT!=1;";
                        break;
                    }
                case LibrarySQLCommands.FetchNotCheckedoutUsersForSystemCheckout:
                    {
                        sQuery = @"SELECT 
                                    CHECKIN_ID
                                FROM
                                    LI_CHECKIN_INFO_?AC_YEAR
                                WHERE
                                    IS_CHECKOUT != 1 AND IS_DELETED != 1
                                        AND IS_SYSTEM_CHECKOUT != 1
                                        AND DATE_FORMAT(NOW(), '%Y-%m-%d') = DATE_FORMAT(CHECKIN_DATE, '%Y-%m-%d');";
                        break;
                    }
                case LibrarySQLCommands.UpdateIsSystemCheckoutByCheckInId:
                    {
                        sQuery = @"UPDATE LI_CHECKIN_INFO_?AC_YEAR SET IS_SYSTEM_CHECKOUT=?IS_SYSTEM_CHECKOUT WHERE CHECKIN_ID=?CHECKIN_ID;";
                        break;
                    }
                #endregion

                #region CIRCULATION
                case LibrarySQLCommands.FetchTotalBookIssuedByUserId:
                    {
                        sQuery = @"SELECT 
                                    LI.ISSUE_ID,
                                    USER_ID,
                                    USER_TYPE,
                                    ROLE,
                                    BI.ACCESS_NO,
	                                BI.DOC_TITLE AS BOOK_TITLE,
                                    CONCAT(IFNULL(BI.PERSONAL_AUTHOR1, ''),
                                                '  ',
                                                IFNULL(BI.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                   date_format(from_unixtime(ISSUE_DATE),'%d-%m-%Y') as ISSUE_DATE,
                                    ISSUE_BY,
                                    if(IS_RETURNED='0',if(IS_MISSED='0','issued','missed'),if(IS_DAMAGED='0','returned','damaged')) AS STATUS,
                                       IF(LD.CREDIT!=0,LD.CREDIT,0) AS CREDIT,
									IF(LD.DEBIT!=0,LD.DEBIT,0) AS DEBIT,
                                    IF(LD.CREDIT!=0,LD.CREDIT,0)-IF(LD.DEBIT!=0,LD.DEBIT,0) AS BALANCE,
                                    date_format(from_unixtime(LD.PAYMENT_DATE),'%d-%m-%Y') as PAYMENT_DATE,
                                    IS_SMS_SENT,
                                    IS_RENEWED,
                                   date_format(from_unixtime(ACTUAL_RETURNED_DATE),'%d-%m-%Y') as ACTUAL_RETURNED_DATE,
                                    IS_RETURNED,
                                    IS_RETURN_CONFIRMED,
                                    RECEIVED_BY,
                                    date_format(RETURN_DATE,'%d-%m-%Y') as RETURN_DATE,
                                    SEMESTER_ID,
                                    SEMESTER_TYPE,
                                    LD.DUES_ID
                                FROM
                                    LI_BOOK_ISSUE_?AC_YEAR AS LI
                                        INNER JOIN
                                    LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                        AND BI.IS_DELETED != 1
                                        LEFT JOIN
                                    LI_ISSUE_DUES AS LD ON LD.ISSUE_ID=LI.ISSUE_ID
                                        AND LD.IS_DELETED!=1
                                WHERE
                                    LI.USER_ID =?USER_ID AND LI.USER_TYPE=?USER_TYPE
                                        AND LI.IS_RETURN_CONFIRMED!=1
                                        AND LI.IS_DELETED!= 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalBookIssuedForExternal:
                    {
                        sQuery = @"SELECT 
                                    LI.ISSUE_ID,
                                    USER_ID,
                                    USER_TYPE,
                                    ROLE,
	                                BI.DOC_TITLE AS BOOK_TITLE,
                                    BI.ISBN_NO,
                                    CONCAT(IFNULL(BI.PERSONAL_AUTHOR1, ''),
                                                '  ',
                                                IFNULL(BI.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                   date_format(from_unixtime(ISSUE_DATE),'%d-%m-%Y') as ISSUE_DATE,
                                    ISSUE_BY,
                                  if(IS_RETURNED='0',if(IS_MISSED='0','issued','missed'),if(IS_DAMAGED='0','returned','damaged')) AS STATUS,
                                      IF(LD.CREDIT!=0,LD.CREDIT,0) AS CREDIT,
									IF(LD.DEBIT!=0,LD.DEBIT,0) AS DEBIT,
                                    IF(LD.CREDIT!=0,LD.CREDIT,0)-IF(LD.DEBIT!=0,LD.DEBIT,0) AS BALANCE,
                                   date_format(from_unixtime(LD.PAYMENT_DATE),'%d-%m-%Y') as PAYMENT_DATE,
                                    IS_SMS_SENT,
                                    IS_RENEWED,
                                   date_format(from_unixtime(ACTUAL_RETURNED_DATE),'%d-%m-%Y') as ACTUAL_RETURNED_DATE,
                                    IS_RETURNED,
                                    IS_RETURN_CONFIRMED,
                                    RECEIVED_BY,
                                    date_format(RETURN_DATE,'%d-%m-%Y') as RETURN_DATE,
                                    SEMESTER_ID,
                                    SEMESTER_TYPE,
                                    LD.DUES_ID
                                FROM
                                    LI_BOOK_ISSUE_?AC_YEAR AS LI
                                        INNER JOIN
                                    LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                        AND BI.IS_DELETED != 1
         
                                         LEFT JOIN
                                        LI_ISSUE_DUES AS LD ON LD.ISSUE_ID=LI.ISSUE_ID
                                        AND LD.IS_DELETED!=1
                                WHERE
                                    LI.USER_ID =?USER_ID AND LI.USER_TYPE=?USER_TYPE
                                        AND LI.IS_DELETED!= 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchTotalBookReservedByUserId:
                    {
                        sQuery = @"SELECT 
                                    BR.RESERVE_ID,
                                    USER_ID,
                                    USER_TYPE,
                                    ROLE,
                                    BI.BOOK_ID,
	                                BI.DOC_TITLE AS BOOK_TITLE,
                                     BI.ISBN_NO,
                                    BI.ACCESS_NO,
                                    BI.BOOK_NO,
								     CONCAT(IFNULL(BI.PERSONAL_AUTHOR1, ''),
                                                '  ',
                                                IFNULL(BI.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                    date_format(from_unixtime(BR.ENTRY_DATE),'%Y-%m-%d') as ENTRY_DATE,
                                    BS.STATUS_NAME AS STATUS,
                                    IFNULL(BR.REASON,'') AS REASON,
                                    IS_APPROVED,
                                    IS_CANCELLED,
                                    IS_COLLECTED,
							        date_format(from_unixtime(IFNULL(BR.APPROVED_DATE,'')),'%Y-%m-%d') as APPROVED_DATE,
                                    date_format(from_unixtime(IFNULL(BR.COLLECTED_DATE,'')),'%Y-%m-%d') as COLLECTED_DATE,
									date_format(from_unixtime(IFNULL(BR.CANCELLED_DATE,'')),'%Y-%m-%d') as CANCELLED_DATE,
									IFNULL(CANCEL_REASON,'') AS CANCEL_REASON,
                                    SEMESTER_ID,
                                    SEMESTER_TYPE
                                    
                                FROM
                                    LI_BOOK_RESERVE AS BR
                                        INNER JOIN
                                    LI_BOOK_INFO AS BI ON BI.BOOK_ID = BR.BOOK_ID
                                        AND BI.IS_DELETED != 1
                                        INNER JOIN
                                        SUP_LI_BOOK_STATUS AS BS ON BS.STATUS_ID=BI.LI_BOOK_STATUS
                                        AND BS.IS_DELETED!=1
                                        
                                WHERE
                                    BR.USER_ID =?USER_ID AND BR.USER_TYPE=?USER_TYPE
                                        AND BR.IS_DELETED!= 1
                                        AND BR.IS_USER_CANCELLED!=1
                                        AND BR.IS_SYSTEM_CANCELLED!=1;";
                        break;
                    }
                case LibrarySQLCommands.FetchActiveReservedBooksByBookId:
                    {
                        sQuery = @"SELECT 
                                    RESERVE_ID
                                FROM
                                    LI_BOOK_RESERVE
                                WHERE
                                    STATUS != 5 AND STATUS != 10
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case LibrarySQLCommands.FetchAllReturnedTransactionWhichNotConfirmed:
                    {
                        sQuery = @"SELECT * FROM (

                                    SELECT 

                                     (@rm:=@rm + 1) AS RUN_NUMBER,
                                     ISSUE_ID,
                                     USER_TYPE_NAME,
                                     ROLE,
                                     FIRST_NAME,
                                     BOOK_TITLE,
                                     AUTHOR,
                                     ISSUE_DATE,
                                     ACTUAL_RETURNED_DATE,
                                     RETURN_DATE,
                                     CREDIT,
                                     DEBIT,
                                     BALANCE,
                                     STATUS

                                    FROM 
                                    (


                                    SELECT 
                                        LI.ISSUE_ID,
                                        UT.USER_TYPE_NAME,
                                         SR.ROLE_NAME AS ROLE,
                                        CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                        UPPER(CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                        '  ',
                                                        IFNULL(BI.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
                                         CONCAT(IFNULL(BI.PERSONAL_AUTHOR1, ''),
                                            ' ',
                                            IFNULL(BI.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                        DATE_FORMAT(FROM_UNIXTIME(LI.ISSUE_DATE), '%d-%m-%Y') AS ISSUE_DATE,
                                        DATE_FORMAT(FROM_UNIXTIME(LI.ACTUAL_RETURNED_DATE),
                                                '%d-%m-%Y') AS ACTUAL_RETURNED_DATE,
                                        DATE_FORMAT(LI.RETURN_DATE, '%d-%m-%Y') AS RETURN_DATE,
                                         IF(LD.CREDIT!=0,LD.CREDIT,0) AS CREDIT,
									IF(LD.DEBIT!=0,LD.DEBIT,0) AS DEBIT,
                                    IF(LD.CREDIT!=0,LD.CREDIT,0)-IF(LD.DEBIT!=0,LD.DEBIT,0) AS BALANCE,
                                         if(LI.IS_RETURNED='0',IF(LI.IS_DAMAGED='0',if(LI.IS_MISSED='0','issued','missed'),'damaged'),'returned') AS STATUS
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS LI
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = LI.USER_ID
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USER_ID = SPI.STUDENT_ID
                                            AND UA.IS_DELETED != 1
                                            AND UA.USER_TYPE = LI.USER_TYPE
                                              INNER JOIN 
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=UA.USER_TYPE
                                              INNER JOIN
                                            SUP_ROLE AS SR ON SR.ROLE_ID=LI.ROLE
                                            LEFT JOIN
                                        LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                            AND LD.IS_DELETED != 1
                                            AND LD.ACADEMIC_YEAR=?AC_YEAR
                                    WHERE
                                        LI.IS_DELETED != 1
                                            AND LI.IS_RETURNED = 1 
                                            AND LI.IS_RETURN_CONFIRMED!=1
                                            AND LI.IS_DAMAGED!=1
                                    UNION SELECT 
                                        LI.ISSUE_ID,
                                         UT.USER_TYPE_NAME,
                                           SR.ROLE_NAME AS ROLE,
                                        CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(STF.LAST_NAME, '')) AS FIRST_NAME,
                                        UPPER(CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                        '  ',
                                                        IFNULL(BI.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
                                        CONCAT(IFNULL(BI.PERSONAL_AUTHOR1, ''),
                                            ' ',
                                            IFNULL(BI.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                        DATE_FORMAT(FROM_UNIXTIME(LI.ISSUE_DATE), '%d-%m-%Y') AS ISSUE_DATE,
                                         DATE_FORMAT(FROM_UNIXTIME(LI.ACTUAL_RETURNED_DATE),
                                                '%d-%m-%Y') AS ACTUAL_RETURNED_DATE,
                                        DATE_FORMAT(LI.RETURN_DATE, '%d-%m-%Y') AS RETURN_DATE,
                                          IF(LD.CREDIT!=0,LD.CREDIT,0) AS CREDIT,
									IF(LD.DEBIT!=0,LD.DEBIT,0) AS DEBIT,
                                    IF(LD.CREDIT!=0,LD.CREDIT,0)-IF(LD.DEBIT!=0,LD.DEBIT,0) AS BALANCE,
                                             if(LI.IS_RETURNED='0',IF(LI.IS_DAMAGED='0',if(LI.IS_MISSED='0','issued','missed'),'damaged'),'returned') AS STATUS
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS LI
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = LI.USER_ID
                                            AND STF.IS_DELETED != 1
                                            AND STF.IS_LEFT != 1
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                            AND UA.IS_DELETED != 1
                                            AND UA.USER_TYPE = LI.USER_TYPE
                                            INNER JOIN 
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=UA.USER_TYPE
                                             INNER JOIN
                                            SUP_ROLE AS SR ON SR.ROLE_ID=LI.ROLE
                                            LEFT JOIN
                                        LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                            AND LD.IS_DELETED != 1
                                             AND LD.ACADEMIC_YEAR=?AC_YEAR
                                    WHERE
                                        LI.IS_DELETED != 1
                                            AND LI.IS_RETURNED = 1 
                                             AND LI.IS_RETURN_CONFIRMED!=1
                                             AND LI.IS_DAMAGED!=1
                                    UNION SELECT 
                                         LI.ISSUE_ID,
                                          UT.USER_TYPE_NAME,
                                          '' AS ROLE,
                                        CONCAT(IFNULL(UI.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(UI.LAST_NAME, '')) AS FIRST_NAME,
                                        UPPER(CONCAT(IFNULL(BI.DOC_TITLE, ''),
                                                        '  ',
                                                        IFNULL(BI.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
                                        CONCAT(IFNULL(BI.PERSONAL_AUTHOR1, ''),
                                            ' ',
                                            IFNULL(BI.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                        DATE_FORMAT(FROM_UNIXTIME(LI.ISSUE_DATE), '%d-%m-%Y') AS ISSUE_DATE,
                                        DATE_FORMAT(FROM_UNIXTIME(LI.ACTUAL_RETURNED_DATE),
                                                '%d-%m-%Y') AS ACTUAL_RETURNED_DATE,
                                        DATE_FORMAT(LI.RETURN_DATE, '%d-%m-%Y') AS RETURN_DATE,
                                         IF(LD.CREDIT!=0,LD.CREDIT,0) AS CREDIT,
									IF(LD.DEBIT!=0,LD.DEBIT,0) AS DEBIT,
                                    IF(LD.CREDIT!=0,LD.CREDIT,0)-IF(LD.DEBIT!=0,LD.DEBIT,0) AS BALANCE,
                                         if(LI.IS_RETURNED='0',IF(LI.IS_DAMAGED='0',if(LI.IS_MISSED='0','issued','missed'),'damaged'),'returned') AS STATUS
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS LI
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                            INNER JOIN
                                        LI_EXTERNAL_USER_INFO AS UI ON UI.USER_ID = LI.USER_ID
                                            AND UI.USER_TYPE = LI.USER_TYPE
                                            INNER JOIN 
                                            SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=UI.USER_TYPE
                                            LEFT JOIN
                                        LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                            AND LD.IS_DELETED != 1
											AND LD.ACADEMIC_YEAR=?AC_YEAR
                                    WHERE
                                        LI.IS_DELETED != 1
                                            AND LI.IS_RETURNED = 1
		                                     AND LI.IS_RETURN_CONFIRMED!=1
                                                 AND LI.IS_DAMAGED!=1) AS T,(SELECT @rm:=0) AS TT
                                                 ORDER BY RUN_NUMBER) AS OT WHERE
			                                            OT.RUN_NUMBER > {0}
                                                        {1}
			                                            LIMIT {2};";
                        break;
                    }
                case LibrarySQLCommands.FetchAllReturnedTransactionCountWhichNotConfirmed:
                    {
                        sQuery = @"
                                    SELECT 
                                        COUNT(ISSUE_ID) AS TOTAL_COUNT
                                    FROM
                                        (SELECT 
                                            LI.ISSUE_ID
                                        FROM
                                            LI_BOOK_ISSUE_?AC_YEAR AS LI
                                        INNER JOIN LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                        INNER JOIN STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = LI.USER_ID
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                        INNER JOIN USER_ACCOUNT AS UA ON UA.USER_ID = SPI.STUDENT_ID
                                            AND UA.IS_DELETED != 1
                                            AND UA.USER_TYPE = LI.USER_TYPE
                                        INNER JOIN SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID = UA.USER_TYPE
                                        INNER JOIN SUP_ROLE AS SR ON SR.ROLE_ID = LI.ROLE
                                        LEFT JOIN LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                            AND LD.IS_DELETED != 1
                                            AND LD.ACADEMIC_YEAR =?AC_YEAR
                                        WHERE
                                            LI.IS_DELETED != 1
                                                AND LI.IS_RETURNED = 1
                                                AND LI.IS_RETURN_CONFIRMED != 1
                                                AND LI.IS_DAMAGED != 1 UNION SELECT 
                                            LI.ISSUE_ID
                                        FROM
                                            LI_BOOK_ISSUE_?AC_YEAR AS LI
                                        INNER JOIN LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                        INNER JOIN STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = LI.USER_ID
                                            AND STF.IS_DELETED != 1
                                            AND STF.IS_LEFT != 1
                                        INNER JOIN USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                            AND UA.IS_DELETED != 1
                                            AND UA.USER_TYPE = LI.USER_TYPE
                                        INNER JOIN SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID = UA.USER_TYPE
                                        INNER JOIN SUP_ROLE AS SR ON SR.ROLE_ID = LI.ROLE
                                        LEFT JOIN LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                            AND LD.IS_DELETED != 1
                                            AND LD.ACADEMIC_YEAR =?AC_YEAR
                                        WHERE
                                            LI.IS_DELETED != 1
                                                AND LI.IS_RETURNED = 1
                                                AND LI.IS_RETURN_CONFIRMED != 1
                                                AND LI.IS_DAMAGED != 1 UNION SELECT 
                                            LI.ISSUE_ID
                                        FROM
                                            LI_BOOK_ISSUE_?AC_YEAR AS LI
                                        INNER JOIN LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                            AND BI.IS_DELETED != 1
                                        INNER JOIN LI_EXTERNAL_USER_INFO AS UI ON UI.USER_ID = LI.USER_ID
                                            AND UI.USER_TYPE = LI.USER_TYPE
                                        INNER JOIN SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID = UI.USER_TYPE
                                        LEFT JOIN LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                            AND LD.IS_DELETED != 1
                                            AND LD.ACADEMIC_YEAR =?AC_YEAR
                                        WHERE
                                            LI.IS_DELETED != 1
                                                AND LI.IS_RETURNED = 1
                                                AND LI.IS_RETURN_CONFIRMED != 1
                                                AND LI.IS_DAMAGED != 1) AS T;";
                        break;

                    }
                case LibrarySQLCommands.FetchAllReturnedTransactionWhichIsNotReturned:
                    {
                        sQuery = @"SELECT RUN_NUMBER,
                                                    ISSUE_ID,
                                                    USER_TYPE_NAME,
                                                    ROLE,
                                                    FIRST_NAME,
                                                    BOOK_TITLE,
                                                    AUTHOR,
                                                    ISSUE_DATE,
                                                    ACTUAL_RETURNED_DATE,
                                                    RETURN_DATE,
                                                    CREDIT,
                                                    DEBIT,
                                                    BALANCE,
                                                    STATUS FROM (
                                            SELECT 
                                                (@rm:=@rm + 1) AS RUN_NUMBER,
                                                ISSUE_ID,
                                                USER_TYPE_NAME,
                                                ROLE,
                                                FIRST_NAME,
                                                BOOK_TITLE,
                                                AUTHOR,
                                                ISSUE_DATE,
                                                ACTUAL_RETURNED_DATE,
                                                RETURN_DATE,
                                                CREDIT,
                                                DEBIT,
                                                BALANCE,
                                                STATUS
                                            FROM
                                                (SELECT 
                                                    LI.ISSUE_ID,
                                                        UT.USER_TYPE_NAME,
                                                        SR.ROLE_NAME AS ROLE,
                                                        CONCAT(IFNULL(SPI.FIRST_NAME, ''), '  ', IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                                        UPPER(CONCAT(IFNULL(BI.DOC_TITLE, ''), '  ', IFNULL(BI.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
                                                        CONCAT(IFNULL(BI.PERSONAL_AUTHOR1, ''), ' ', IFNULL(BI.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                                        DATE_FORMAT(FROM_UNIXTIME(LI.ISSUE_DATE), '%d-%m-%Y') AS ISSUE_DATE,
                                                        DATE_FORMAT(FROM_UNIXTIME(LI.ACTUAL_RETURNED_DATE), '%d-%m-%Y') AS ACTUAL_RETURNED_DATE,
                                                        DATE_FORMAT(LI.RETURN_DATE, '%d-%m-%Y') AS RETURN_DATE,
                                                        IF(LD.CREDIT != 0, LD.CREDIT, 0) AS CREDIT,
                                                        IF(LD.DEBIT != 0, LD.DEBIT, 0) AS DEBIT,
                                                        IF(LD.CREDIT != 0, LD.CREDIT, 0) - IF(LD.DEBIT != 0, LD.DEBIT, 0) AS BALANCE,
                                                        if(LI.IS_RETURNED='0',IF(LI.IS_DAMAGED='0',if(LI.IS_MISSED='0','issued','missed'),'damaged'),'returned') AS STATUS
                                                FROM
                                                    LI_BOOK_ISSUE_?AC_YEAR AS LI
                                                INNER JOIN LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                LEFT JOIN LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                                    AND LD.IS_DELETED != 1
                                                    AND LD.ACADEMIC_YEAR =?AC_YEAR
                                                INNER JOIN STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = LI.USER_ID
                                                    AND SPI.IS_DELETED != 1
                                                    AND SPI.IS_LEFT != 1
                                                INNER JOIN SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID = LI.USER_TYPE
                                                INNER JOIN SUP_ROLE AS SR ON SR.ROLE_ID = LI.ROLE
                                                WHERE
                                                    LI.IS_DELETED != 1
                                                        AND LI.IS_RETURNED != 1
                                                        AND LI.IS_MISSED != 1 UNION SELECT 
                                                    LI.ISSUE_ID,
                                                        UT.USER_TYPE_NAME,
                                                        SR.ROLE_NAME AS ROLE,
                                                        CONCAT(IFNULL(STF.FIRST_NAME, ''), '  ', IFNULL(STF.LAST_NAME, '')) AS FIRST_NAME,
                                                        UPPER(CONCAT(IFNULL(BI.DOC_TITLE, ''), '  ', IFNULL(BI.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
                                                        CONCAT(IFNULL(BI.PERSONAL_AUTHOR1, ''), ' ', IFNULL(BI.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                                        DATE_FORMAT(FROM_UNIXTIME(LI.ISSUE_DATE), '%d-%m-%Y') AS ISSUE_DATE,
                                                        DATE_FORMAT(FROM_UNIXTIME(LI.ACTUAL_RETURNED_DATE), '%d-%m-%Y') AS ACTUAL_RETURNED_DATE,
                                                        DATE_FORMAT(LI.RETURN_DATE, '%d-%m-%Y') AS RETURN_DATE,
                                                        IF(LD.CREDIT != 0, LD.CREDIT, 0) AS CREDIT,
                                                        IF(LD.DEBIT != 0, LD.DEBIT, 0) AS DEBIT,
                                                        IF(LD.CREDIT != 0, LD.CREDIT, 0) - IF(LD.DEBIT != 0, LD.DEBIT, 0) AS BALANCE,
                                                        if(LI.IS_RETURNED='0',IF(LI.IS_DAMAGED='0',if(LI.IS_MISSED='0','issued','missed'),'damaged'),'returned') AS STATUS
                                                FROM
                                                    LI_BOOK_ISSUE_?AC_YEAR AS LI
                                                INNER JOIN LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                LEFT JOIN LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                                    AND LD.IS_DELETED != 1
                                                    AND LD.ACADEMIC_YEAR =?AC_YEAR
                                                INNER JOIN STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = LI.USER_ID
                                                    AND STF.IS_DELETED != 1
                                                    AND STF.IS_LEFT != 1
                                                INNER JOIN SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID = LI.USER_TYPE
                                                INNER JOIN SUP_ROLE AS SR ON SR.ROLE_ID = LI.ROLE
                                                WHERE
                                                    LI.IS_DELETED != 1
                                                        AND LI.IS_RETURNED != 1
                                                        AND LI.IS_MISSED != 1 UNION SELECT 
                                                    LI.ISSUE_ID,
                                                        UT.USER_TYPE_NAME,
                                                        '' AS ROLE,
                                                        CONCAT(IFNULL(UI.FIRST_NAME, ''), '  ', IFNULL(UI.LAST_NAME, '')) AS FIRST_NAME,
                                                        UPPER(CONCAT(IFNULL(BI.DOC_TITLE, ''), '  ', IFNULL(BI.PARALLEL_TITLE, ''))) AS BOOK_TITLE,
                                                        CONCAT(IFNULL(BI.PERSONAL_AUTHOR1, ''), ' ', IFNULL(BI.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                                        DATE_FORMAT(FROM_UNIXTIME(LI.ISSUE_DATE), '%d-%m-%Y') AS ISSUE_DATE,
                                                        DATE_FORMAT(FROM_UNIXTIME(LI.ACTUAL_RETURNED_DATE), '%d-%m-%Y') AS ACTUAL_RETURNED_DATE,
                                                        DATE_FORMAT(LI.RETURN_DATE, '%d-%m-%Y') AS RETURN_DATE,
                                                        IF(LD.CREDIT != 0, LD.CREDIT, 0) AS CREDIT,
                                                        IF(LD.DEBIT != 0, LD.DEBIT, 0) AS DEBIT,
                                                        IF(LD.CREDIT != 0, LD.CREDIT, 0) - IF(LD.DEBIT != 0, LD.DEBIT, 0) AS BALANCE,
                                                        if(LI.IS_RETURNED='0',IF(LI.IS_DAMAGED='0',if(LI.IS_MISSED='0','issued','missed'),'damaged'),'returned') AS STATUS
                                                FROM
                                                    LI_BOOK_ISSUE_?AC_YEAR AS LI
                                                INNER JOIN LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                                    AND BI.IS_DELETED != 1
                                                INNER JOIN LI_EXTERNAL_USER_INFO AS UI ON UI.USER_ID = LI.USER_ID
                                                    AND UI.USER_TYPE = LI.USER_TYPE
                                                LEFT JOIN LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                                    AND LD.IS_DELETED != 1
                                                    AND LD.ACADEMIC_YEAR =?AC_YEAR
                                                INNER JOIN SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID = UI.USER_TYPE
                                                WHERE
                                                    LI.IS_DELETED != 1
                                                        AND LI.IS_RETURNED != 1
                                                        AND LI.IS_MISSED != 1) AS T,(SELECT @rm:=0) AS TT
                                                        ORDER BY RUN_NUMBER) AS OT WHERE
			                                            OT.RUN_NUMBER > {0}
                                                        {1}
			                                            LIMIT {2};";
                        break;
                    }
                case LibrarySQLCommands.FetchAllReturnedTransactionCountWhichIsNotReturned:
                    {
                        sQuery = @"SELECT 
    
                                    ISSUE_ID,
                                    COUNT(ISSUE_ID) AS TOTAL_COUNT
                                FROM
                                    (SELECT 
                                        LI.ISSUE_ID
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS LI
                                    INNER JOIN LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                        AND BI.IS_DELETED != 1
                                    LEFT JOIN LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                        AND LD.IS_DELETED != 1
                                        AND LD.ACADEMIC_YEAR =?AC_YEAR
                                    INNER JOIN STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = LI.USER_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                    INNER JOIN SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID = LI.USER_TYPE
                                    INNER JOIN SUP_ROLE AS SR ON SR.ROLE_ID = LI.ROLE
                                    WHERE
                                        LI.IS_DELETED != 1
                                            AND LI.IS_RETURNED != 1
                                            AND LI.IS_MISSED != 1 UNION SELECT 
                                        LI.ISSUE_ID
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS LI
                                    INNER JOIN LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                        AND BI.IS_DELETED != 1
                                    LEFT JOIN LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                        AND LD.IS_DELETED != 1
                                        AND LD.ACADEMIC_YEAR =?AC_YEAR
                                    INNER JOIN STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = LI.USER_ID
                                        AND STF.IS_DELETED != 1
                                        AND STF.IS_LEFT != 1
                                    INNER JOIN SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID = LI.USER_TYPE
                                    INNER JOIN SUP_ROLE AS SR ON SR.ROLE_ID = LI.ROLE
                                    WHERE
                                        LI.IS_DELETED != 1
                                            AND LI.IS_RETURNED != 1
                                            AND LI.IS_MISSED != 1 UNION SELECT 
                                        LI.ISSUE_ID
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS LI
                                    INNER JOIN LI_BOOK_INFO AS BI ON BI.BOOK_ID = LI.BOOK_ID
                                        AND BI.IS_DELETED != 1
                                    INNER JOIN LI_EXTERNAL_USER_INFO AS UI ON UI.USER_ID = LI.USER_ID
                                        AND UI.USER_TYPE = LI.USER_TYPE
                                    LEFT JOIN LI_ISSUE_DUES AS LD ON LD.ISSUE_ID = LI.ISSUE_ID
                                        AND LD.IS_DELETED != 1
                                        AND LD.ACADEMIC_YEAR =?AC_YEAR
                                    INNER JOIN SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID = UI.USER_TYPE
                                    WHERE
                                        LI.IS_DELETED != 1
                                            AND LI.IS_RETURNED != 1
                                            AND LI.IS_MISSED != 1) AS T;";
                        break;
                    }
                #endregion

                #region OPAC Search
                case LibrarySQLCommands.FetchBookInfoBySearchText:
                    {
                        sQuery = @"SELECT 
                                        BI.BOOK_ID,BR.RESERVE_ID,
                                        UPPER(BI.DOC_TITLE) AS DOC_TITLE,
                                        UPPER(BI.PARALLEL_TITLE) AS PARALLEL_TITLE,
                                        BI.ISBN_NO,
                                        BI.LI_BOOK_STATUS,
                                        BS.STATUS_NAME,
                                        BI.CLASS_NO,
                                        BI.BOOK_NO,
                                        SS.STATUS_NAME AS STATUS,
                                        CONCAT(RI.RACK_INFO,' ( ',R.RACK_NAME,' - ',C.CELL_NAME,' ) ') AS RACK_ID,
                                        CONCAT(IFNULL(BI.PERSONAL_AUTHOR1,''),' ',IFNULL(BI.PERSONAL_AUTHOR2,'')) AS PERSONAL_AUTHOR1
                                    FROM
                                        LI_BOOK_INFO AS BI
                                            LEFT JOIN
                                        LI_RACK_INFO AS RI ON RI.RACK_INFO_ID = BI.RACK_ID
                                            LEFT JOIN
                                        SUP_LI_RACKS AS R ON R.RACK_ID = RI.RACK_ID
                                            LEFT JOIN
                                        SUP_LI_CELLS AS C ON C.CELL_ID = RI.CELL_ID
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = BI.LANGUAGE
                                            LEFT JOIN
                                        SUP_LI_ILLUSTRATION AS LI ON LI.ILLUSTRATE_ID = BI.ILLUSTRATE
                                            LEFT JOIN
                                        SUP_LI_DISCIPLINE AS D ON D.DISCIPLINE_ID = BI.DISIPLINE
                                            LEFT JOIN
                                        SUP_LI_SERIES AS LS ON LS.SERIES_ID = BI.SERIES
                                            LEFT JOIN
                                        SUP_LI_BOOK_TYPE AS BT ON BT.TYPE_ID = BI.DOC_CATEGORY
                                            LEFT JOIN
										SUP_LI_BOOK_STATUS AS BS ON BS.STATUS_ID=BI.LI_BOOK_STATUS                                            
                                            LEFT JOIN
                                            SUP_LI_STATUS AS SS ON SS.STATUS_ID=BI.STATUS
                                            AND SS.IS_DELETED!=1
                                            LEFT JOIN
										LI_BOOK_RESERVE AS BR ON BR.BOOK_ID=BI.BOOK_ID
											AND BR.USER_ID=?USER_ID
                                            AND BR.USER_TYPE=?USER_TYPE
                                            AND BR.SEMESTER_TYPE=(SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_ACTIVE=1 AND IS_DELETED!=1 AND SEMESTER_TYPE_ID!=3)
                                            AND BR.ACADEMIC_YEAR=?AC_YEAR
                                            AND BR.IS_DELETED!=1
                                            AND BR.IS_USER_CANCELLED!=1
                                            AND BR.IS_CANCELLED!=1
                                            AND BR.IS_SYSTEM_CANCELLED!=1
                                    WHERE
                                        BI.IS_DELETED != 1 
                                        AND (DOC_TITLE LIKE '%?SEARCH_TEXT%' 
                                        OR BOOK_NO LIKE '%?SEARCH_TEXT%' 
                                        OR PERSONAL_AUTHOR1 LIKE '%?SEARCH_TEXT%'
                                        OR BARCODE  LIKE '%?SEARCH_TEXT%'
                                        OR ACCESS_NO LIKE '%?SEARCH_TEXT%');";
                        break;
                    }
                case LibrarySQLCommands.FetchRulesByUserId:
                    {
                        sQuery = @"SELECT 
                                            SC.STUDENT_ID AS USER_ID,
                                            SP.ROLL_NO AS USERNAME,
                                            UA.USER_TYPE AS USER_TYPE,
                                            UA.ROLE,
                                            PG.APPTYPE_ID AS PROGRAMME_LEVEL,
                                            R.NO_OF_BOOKS,
                                            R.NO_OF_DAYS,
                                            R.NO_OF_RENEW_DAYS,
                                            R.PER_DAY_DUE_COST,
                                            R.REVISED_DUE_DATES,
                                            R.REVISED_DUE_COST
                                        FROM
                                            STU_CLASS AS SC
                                                INNER JOIN
                                            STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                                AND SP.IS_DELETED != 1
                                                INNER JOIN
                                            CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                                INNER JOIN
                                            CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                                INNER JOIN
                                            USER_ACCOUNT AS UA ON UA.USER_ID = SC.STUDENT_ID
												AND UA.USER_TYPE=?USER_TYPE
                                                AND UA.USERNAME = SP.ROLL_NO
                                                AND UA.IS_DELETED != 1
                                                INNER JOIN
                                            LI_RULES AS R ON R.USER_TYPE = UA.USER_TYPE 
                                               AND R.PROGRAMME_LEVEL=PG.APPTYPE_ID
                                               AND R.ACADEMIC_YEAR=?AC_YEAR
                                        WHERE
                                            SC.ACADEMIC_YEAR =?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                                AND SC.STUDENT_ID =?USER_ID
                                        UNION SELECT 
                                                STF.STAFF_ID AS USER_ID,
                                                STF.STAFF_CODE AS USERNAME,
                                                UA.USER_TYPE AS USER_TYPE,
                                                UA.ROLE,
                                                '0' AS PROGRAMME_LEVEL,
                                                R.NO_OF_BOOKS,
                                                R.NO_OF_DAYS,
                                                R.NO_OF_RENEW_DAYS,
                                                R.PER_DAY_DUE_COST,
                                                R.REVISED_DUE_DATES,
                                                R.REVISED_DUE_COST
                                            FROM
                                                STF_PERSONAL_INFO AS STF
                                                    INNER JOIN
                                                USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                                    AND UA.USER_TYPE=?USER_TYPE
                                                    AND UA.USERNAME = STF.STAFF_CODE
                                                    AND UA.IS_DELETED != 1
                                                    INNER JOIN
                                                LI_RULES AS R ON R.USER_TYPE = UA.USER_TYPE
                                                AND R.ACADEMIC_YEAR=?AC_YEAR
                                            WHERE
                                                STF.IS_DELETED != 1
                                                AND STF.STAFF_ID =?USER_ID;";
                        break;
                    }
                case LibrarySQLCommands.CancelBookReservation:
                    {
                        sQuery = @"UPDATE LI_BOOK_RESERVE SET IS_USER_CANCELLED=1  WHERE RESERVE_ID=?RESERVE_ID;";
                        break;
                    }
                case LibrarySQLCommands.BookReservationListForUserById:
                    {
                        sQuery = @"SELECT 
                                        RESERVE_ID,
                                        S.BOOK_ID,
                                        UPPER(CONCAT(IFNULL(S.DOC_TITLE,''),' ',IFNULL(S.PARALLEL_TITLE,''))) AS BOOK_TITLE,
										UPPER(CONCAT(IFNULL(S.PERSONAL_AUTHOR1,''),'  ',IFNULL(S.PERSONAL_AUTHOR2,''))) AS AUTHOR,
                                        S.ISBN_NO,
                                        BR.IS_APPROVED,
                                        DATE_FORMAT(IFNULL(FROM_UNIXTIME(BR.APPROVED_DATE), ''),
                                                '%d/%m/%Y') AS APPROVED_DATE,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SP.LAST_NAME, '')) AS APPROVED_BY,
                                        BR.IS_CANCELLED,
                                        DATE_FORMAT(IFNULL(FROM_UNIXTIME(BR.CANCELLED_DATE), ''),
                                                '%d/%m/%Y') AS CANCELLED_DATE,
                                        CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(STF.LAST_NAME, '')) AS CANCELLED_BY,
                                        BR.CANCEL_REASON,
                                        BR.IS_USER_CANCELLED,
                                        BR.IS_SYSTEM_CANCELLED,
                                        BR.IS_COLLECTED,
                                         DATE_FORMAT(IFNULL(FROM_UNIXTIME(BR.COLLECTED_DATE), ''),
                                                '%d/%m/%Y') AS COLLECTED_DATE,
                                        DATE_FORMAT(FROM_UNIXTIME(BR.ENTRY_DATE), '%d/%m/%Y') AS ENTRY_DATE
                                    FROM
                                        LI_BOOK_RESERVE AS BR
                                            INNER JOIN
                                        LI_BOOK_INFO AS S ON S.BOOK_ID = BR.BOOK_ID
                                            AND S.IS_DELETED != 1
                                            LEFT JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = BR.APPROVED_BY
                                            AND SP.IS_DELETED != 1
                                            LEFT JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = BR.CANCELLED_BY
                                            AND SP.IS_DELETED != 1
                                            
                                    WHERE
                                        BR.SEMESTER_TYPE = (SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_DELETED!=1 AND IS_ACTIVE=1 AND SEMESTER_TYPE_ID!=3) 
                                        AND BR.USER_ID=?USER_ID
                                        AND BR.USER_TYPE=?USER_TYPE
                                        AND BR.IS_DELETED != 1;";
                        break;
                    }
                #endregion

                #region Library Account Query
                case LibrarySQLCommands.FetchUserCredentialByRFIDLibrary:
                    {
                        sQuery = @"SELECT 
                                        UA.USER_ID, 
                                        UA.ROLE, 
                                        UA.USER_TYPE, 
                                        UA.NAME AS FIRST_NAME,
                                        (SELECT AC_YEAR FROM ACADEMIC_YEAR WHERE ACTIVE_YEAR=1) AS AC_YEAR,
                                        PG.APPTYPE_ID AS PROGRAMME_LEVEL,
                                        '0' AS SEMESTER,
                                        (SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_ACTIVE=1  AND SEMESTER_TYPE_ID!=3) AS SEMESTER_TYPE
                                    FROM
                                        USER_ACCOUNT AS UA
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = UA.USER_ID
                                            AND SP.ROLL_NO = UA.USERNAME
                                            AND SP.RFID = ?SCAN_ID
                                            AND SP.IS_DELETED != 1
                                             INNER JOIN
                                         STU_CLASS AS sc ON sc.STUDENT_ID = sp.STUDENT_ID
                                             INNER JOIN
	                                     CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=Sc.PROGRAMME_GROUP_ID
                                    WHERE
                                        UA.IS_DELETED != 1 
                                    UNION 
                                    SELECT 
                                        UA.USER_ID, 
                                        UA.ROLE, 
                                        UA.USER_TYPE, 
                                        UA.NAME AS FIRST_NAME,
                                        (SELECT AC_YEAR FROM ACADEMIC_YEAR WHERE ACTIVE_YEAR=1) AS AC_YEAR,
                                        '0' AS PROGRAMME_LEVEL,
                                        '0' AS SEMESTER,
                                        (SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_ACTIVE=1  AND SEMESTER_TYPE_ID!=3) AS SEMESTER_TYPE
                                    FROM
                                        USER_ACCOUNT AS UA
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = UA.USER_ID
                                            AND STF.STAFF_CODE = UA.USERNAME
                                            AND STF.RFID =  ?SCAN_ID
                                            AND STF.IS_DELETED != 1
                                    WHERE
                                        UA.IS_DELETED != 1 
                                    UNION 
                                    SELECT 
                                        USER_ID,
                                        '14' AS ROLE,
                                        USER_TYPE,
                                        CONCAT(IFNULL(UI.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(UI.LAST_NAME, '')) AS FIRST_NAME,
	                                    (SELECT AC_YEAR FROM ACADEMIC_YEAR WHERE ACTIVE_YEAR=1) AS AC_YEAR,
                                        '0' AS PROGRAMME_LEVEL,
                                        '0' AS SEMESTER,
                                        (SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_ACTIVE=1  AND SEMESTER_TYPE_ID!=3) AS SEMESTER_TYPE
                                    FROM
                                        LI_EXTERNAL_USER_INFO AS UI
                                    WHERE
                                        UI.IS_DELETED != 1 AND UI.RFID= ?SCAN_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchUserCredentialByBARCODELibrary:
                    {
                        sQuery = @"SELECT 
                                        UA.USER_ID, 
                                        UA.ROLE, 
                                        UA.USER_TYPE, 
                                        UA.NAME AS FIRST_NAME,
                                        PG.APPTYPE_ID AS PROGRAMME_LEVEL,
                                        (SELECT AC_YEAR FROM ACADEMIC_YEAR WHERE ACTIVE_YEAR=1) AS AC_YEAR,
                                        '0' AS SEMESTER,
                                        (SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_ACTIVE=1  AND SEMESTER_TYPE_ID!=3) AS SEMESTER_TYPE
                                    FROM
                                        USER_ACCOUNT AS UA
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = UA.USER_ID
                                            AND SP.ROLL_NO = UA.USERNAME
                                            AND SP.BAR_CODE = ?SCAN_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
	                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=SP.PROGRAM_ID
                                    WHERE
                                        UA.IS_DELETED != 1 
                                    UNION 
                                    SELECT 
                                        UA.USER_ID, 
                                        UA.ROLE, 
                                        UA.USER_TYPE, 
                                        UA.NAME AS FIRST_NAME,
                                        '0' AS PROGRAMME_LEVEL ,
                                        (SELECT AC_YEAR FROM ACADEMIC_YEAR WHERE ACTIVE_YEAR=1) AS AC_YEAR,
                                        '0' AS SEMESTER,
                                        (SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_ACTIVE=1  AND SEMESTER_TYPE_ID!=3) AS SEMESTER_TYPE
                                    FROM
                                        USER_ACCOUNT AS UA
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = UA.USER_ID
                                            AND STF.STAFF_CODE = UA.USERNAME
                                            AND STF.BAR_CODE = ?SCAN_ID
                                            AND STF.IS_DELETED != 1
                                    WHERE
                                        UA.IS_DELETED != 1 
                                    UNION 
                                    SELECT 
                                        USER_ID,
                                        '14' AS ROLE,
                                        USER_TYPE,
                                        CONCAT(IFNULL(UI.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(UI.LAST_NAME, '')) AS FIRST_NAME,
                                                '0' AS PROGRAMME_LEVEL,
	                                    (SELECT AC_YEAR FROM ACADEMIC_YEAR WHERE ACTIVE_YEAR=1) AS AC_YEAR,
                                        '0' AS SEMESTER,
	                                    (SELECT SEMESTER_TYPE_ID FROM SUP_SEMESTER_TYPE WHERE IS_ACTIVE=1  AND SEMESTER_TYPE_ID!=3) AS SEMESTER_TYPE
                                    FROM
                                        LI_EXTERNAL_USER_INFO AS UI
                                    WHERE
                                        UI.IS_DELETED != 1 AND UI.BAR_CODE=?SCAN_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchActiveSemesterByUserId:
                    {
                        sQuery = @"SELECT 
                                        SC.STUDENT_ID,
                                        SEM.SEMESTER,
                                        C.CLASS_YEAR,
                                        AB.BATCH_ID AS BATCH
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        ACADEMIC_YEAR AS AY ON AY.AC_YEAR = SC.ACADEMIC_YEAR
                                            AND AY.ACTIVE_YEAR = 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        ACADEMIC_BATCHES AS AB ON AB.ACADEMIC_YEAR_ID = AY.ACADEMIC_YEAR_ID
                                            AND AB.BATCH_YEAR = C.CLASS_YEAR
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                            AND SEM.BATCH = AB.BATCH_ID
                                            AND SEM.ACADEMIC_YEAR = AY.AC_YEAR
                                            AND SEM.IS_ACTIVE = 1
                                            AND SEM.IS_DELETED != 1
                                    WHERE SC.IS_DELETED!=1 AND SC.STUDENT_ID=?USER_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchValidateCheckInLibrary:
                    {
                        sQuery = @"SELECT CHECKIN_ID FROM LI_CHECKIN_INFO_?AC_YEAR WHERE USER_ID=?USER_ID AND USER_TYPE=?USER_TYPE AND DATE_FORMAT(CHECKIN_DATE,'%Y-%m-%d')=CURDATE() AND IS_CHECKOUT!=1;";
                        break;
                    }
                case LibrarySQLCommands.SaveCheckInEntry:
                    {
                        sQuery = @"INSERT INTO LI_CHECKIN_INFO_?AC_YEAR
                                  (
	                                  USER_ID,
	                                  USER_TYPE,
	                                  ROLE,
	                                  PROGRAMME_LEVEL,
	                                  CHECKIN_DATE,
	                                  SEMESTER_ID,
	                                  SEMESTER_TYPE
                                  )
                                  VALUES
                                  (
                                      ?USER_ID,
	                                  ?USER_TYPE,
	                                  ?ROLE,
	                                  ?PROGRAMME_LEVEL,
	                                  NOW(),
	                                  ?SEMESTER,
	                                  ?SEMESTER_TYPE
                                  );";
                        break;
                    }
                case LibrarySQLCommands.SaveBulkCheckInEntry:
                    {
                        sQuery = @"INSERT INTO LI_CHECKIN_INFO_?AC_YEAR
                                  (
	                                  USER_ID,
	                                  USER_TYPE,
	                                  ROLE,
	                                  PROGRAMME_LEVEL,
	                                  CHECKIN_DATE,
	                                  SEMESTER_ID,
	                                  SEMESTER_TYPE
                                  )
                                  VALUES
                                  (
                                      ?USER_ID,
	                                  ?USER_TYPE,
	                                  ?ROLE,
	                                  ?PROGRAMME_LEVEL,
	                                  ?CHECKIN_DATE,
	                                  ?SEMESTER,
	                                  ?SEMESTER_TYPE
                                  );";
                        break;
                    }
                case LibrarySQLCommands.SaveCheckOutEntry:
                    {
                        sQuery = @"UPDATE LI_CHECKIN_INFO_?AC_YEAR SET IS_CHECKOUT=1,CHECKOUT_DATE=NOW() WHERE CHECKIN_ID=?CHECKIN_ID;";
                        break;
                    }
                #endregion

                case LibrarySQLCommands.FetchIssuedBookListForReview:
                    {
                        sQuery = @"SELECT 
                                    BI.ISSUE_ID,
                                    LR.REVIEW,
                                    LR.REVIEW_ID,
                                    BI.USER_ID,
                                    BI.BOOK_ID,
                                    B.BOOK_NO,
                                    UPPER(STF.FIRST_NAME) AS FIRST_NAME,
                                    UA.USERNAME,
                                    UT.USER_TYPE_NAME,
                                    B.ACCESS_NO,
                                    B.DOC_TITLE AS BOOK_TITLE,
                                    CONCAT(IFNULL(B.PERSONAL_AUTHOR1, ''),
                                            ' ',
                                            IFNULL(B.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' - ',
                                            IFNULL(SP.STAFF_CODE, '')) AS ISSUE_BY,
                                    DATE_FORMAT(FROM_UNIXTIME(BI.ISSUE_DATE), '%Y-%m-%d') AS ISSUE_DATE,
                                    DATE_FORMAT(FROM_UNIXTIME(BI.ACTUAL_RETURNED_DATE),
                                            '%Y-%m-%d') AS ACTUAL_RETURNED_DATE,
                                    BI.SEMESTER_TYPE
                                FROM
                                    LI_BOOK_ISSUE_?AC_YEAR AS BI
                                        INNER JOIN
                                    LI_BOOK_INFO AS B ON B.BOOK_ID = BI.BOOK_ID
                                        AND B.IS_DELETED != 1
                                        INNER JOIN
                                    USER_ACCOUNT AS UA ON UA.USER_ID = BI.USER_ID
                                        AND UA.USER_TYPE = BI.USER_TYPE
                                        AND UA.IS_DELETED != 1
                                        LEFT JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = BI.ISSUE_BY
                                        AND SP.IS_DELETED != 1
                                        LEFT JOIN
                                    STU_PERSONAL_INFO AS STF ON STF.STUDENT_ID = BI.USER_ID
                                        AND UA.USERNAME = STF.ROLL_NO
                                        AND STF.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID = BI.USER_TYPE
                                        LEFT JOIN
                                    LI_USER_REVIEW_INFO AS LR ON LR.USER_ID = BI.USER_ID
                                        AND LR.IS_DELETED != 1
                                        AND LR.ISSUE_ID = BI.ISSUE_ID
                                        AND LR.ACADEMIC_YEAR = ?AC_YEAR
                                WHERE
                                    BI.IS_DELETED != 1
                                        AND UA.USER_ID = ?USER_ID
                                UNION SELECT 
                                    BI.ISSUE_ID,
                                    LR.REVIEW,
                                    LR.REVIEW_ID,
                                    BI.USER_ID,
                                    BI.BOOK_ID,
                                    B.BOOK_NO,
                                    UPPER(NAME) AS FIRST_NAME,
                                    UA.USERNAME,
                                    UT.USER_TYPE_NAME,
                                    B.ACCESS_NO,
                                    B.DOC_TITLE AS BOOK_TITLE,
                                    CONCAT(IFNULL(B.PERSONAL_AUTHOR1, ''),
                                            ' ',
                                            IFNULL(B.PERSONAL_AUTHOR2, '')) AS AUTHOR,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' - ',
                                            IFNULL(SP.STAFF_CODE, '')) AS ISSUE_BY,
                                    DATE_FORMAT(FROM_UNIXTIME(BI.ISSUE_DATE), '%Y-%m-%d') AS ISSUE_DATE,
                                    DATE_FORMAT(FROM_UNIXTIME(BI.ACTUAL_RETURNED_DATE),
                                            '%Y-%m-%d') AS ACTUAL_RETURNED_DATE,
                                    BI.SEMESTER_TYPE
                                FROM
                                    LI_BOOK_ISSUE_?AC_YEAR AS BI
                                        INNER JOIN
                                    LI_BOOK_INFO AS B ON B.BOOK_ID = BI.BOOK_ID
                                        AND B.IS_DELETED != 1
                                        INNER JOIN
                                    USER_ACCOUNT AS UA ON UA.USER_ID = BI.USER_ID
                                        AND UA.USER_TYPE = BI.USER_TYPE
                                        AND UA.IS_DELETED != 1
                                        LEFT JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = BI.ISSUE_BY
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = BI.USER_ID
                                        AND STF.STAFF_CODE = UA.USERNAME
                                        AND STF.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID = BI.USER_TYPE
                                        LEFT JOIN
                                    LI_USER_REVIEW_INFO AS LR ON LR.USER_ID = BI.USER_ID
                                        AND LR.IS_DELETED != 1
                                        AND LR.ISSUE_ID = BI.ISSUE_ID
                                        AND LR.ACADEMIC_YEAR = ?AC_YEAR
                                WHERE
                                    BI.IS_DELETED != 1
                                        AND UA.USER_ID = ?USER_ID;";
                        break;
                    }
                case LibrarySQLCommands.SaveLiUserReviewInfo:
                    {
                        sQuery = @"INSERT INTO li_user_review_info
                                (
                                USER_ID,
                                ISSUE_ID,
                                ACADEMIC_YEAR,
                                REVIEW,
                                SEMESTER_TYPE)
                                VALUES
                                (
                                ?USER_ID,
                                ?ISSUE_ID,
                                ?ACADEMIC_YEAR,
                                ?REVIEW,
                                ?SEMESTER_TYPE
                                );";
                        break;
                    }
                case LibrarySQLCommands.UpdateLiUserReviewInfo:
                    {
                        sQuery = @"UPDATE LI_USER_REVIEW_INFO 
                                                            SET 
                                                                REVIEW = ?REVIEW
                                                            WHERE
                                                                REVIEW_ID = ?REVIEW_ID;";
                        break;
                    }
                case LibrarySQLCommands.FetchERAStudentListForMarkEntry:
                    {
                        sQuery = @" SELECT 
                                        C.CLASS_ID,
                                        C.CLASS_NAME,
                                        CCS.COURSE_ID,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        CR.COURSE_TITLE,
                                        CCS.STAFF_ID,
                                        SP.STUDENT_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS 'STUDENT_NAME',
                                        CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(STF.LAST_NAME, '')) AS 'STAFF_NAME',
                                        SCC.COMPONENT,
                                        CG.MAX_MARK,
                                        ES.PER_BOOK_MARK,
                                        CCTG.GROUP_ID,
                                        SCC.COMPONENT_ID
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            AND C.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID = C.CLASS_ID
                                            AND CCS.STAFF_ID = ?STAFF_ID
                                            AND sc.ACADEMIC_YEAR = ccs.ACADEMIC_YEAR
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCS.COURSE_ID
                                            AND CR.IS_DELETED != 1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = CCS.STAFF_ID
                                            AND STF.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_TYPE AS CCT ON CCT.COURSE_TYPE_ID = CR.COURSE_TYPE
                                            INNER JOIN
                                        CP_COURSE_TYPE_GROUP AS CCTG ON CCTG.COURSE_TYPE_ID = CCT.COURSE_TYPE_ID
                                            INNER JOIN
                                        CIA_GROUP_COMPONENT AS CG ON CG.CIA_GROUP_ID = CCTG.GROUP_ID
                                            AND CG.IS_DELETED != 1
                                            INNER JOIN
                                        ERA_SETTING AS ES ON ES.COURSE_ID = CCS.COURSE_ID
                                            AND ES.ACADEMIC_YEAR = ?AC_YEAR  and es.COMPONENT_ID=?COMPONENT_ID
                                            AND ES.IS_ACTIVE = 1
                                            AND ES.IS_DELETED != 1
                                            AND ES.SEMESTER_TYPE = CCS.SEMESTER_TYPE_ID
                                            INNER JOIN
                                        SUP_CIA_COMPONENTS AS SCC ON SCC.COMPONENT_ID = ES.COMPONENT_ID
                                            AND CG.SUP_CIA_COMPONENT_ID = scc.COMPONENT_ID
                                            AND SCC.IS_DELETED != 1
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ccs.COURSE_ID = ?COURSE_ID
                                            AND ccs.CLASS_ID = ?CLASS_ID
                                            AND SC.IS_DELETED != 1;";

                        break;
                    }
                case LibrarySQLCommands.FetchSupCIAComponentbyCourseIdAndERASetting:
                    {
                        sQuery = @"SELECT 
                                        SCIA.COMPONENT_ID, SCIA.COMPONENT
                                    FROM
                                        CP_COURSE_ROOT AS CR
                                            INNER JOIN
                                        CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CR.COURSE_TYPE
                                            INNER JOIN
                                        CP_COURSE_TYPE_GROUP CG ON CG.COURSE_TYPE_ID = CR.COURSE_TYPE
                                            INNER JOIN
                                        CIA_GROUP_COMPONENT AS CIA ON CIA_GROUP_ID = CG.GROUP_ID
                                            INNER JOIN
                                        SUP_CIA_COMPONENTS AS SCIA ON SCIA.COMPONENT_ID = CIA.SUP_CIA_COMPONENT_ID
                                            AND COURSE_ROOT_ID = ?COURSE_ID
                                            INNER JOIN
                                        ERA_SETTING AS ES ON ES.COURSE_ID = CR.COURSE_ROOT_ID
                                            AND ES.IS_DELETED != 1 AND ES.IS_WRITING =1
                                            AND ES.ACADEMIC_YEAR = ?AC_YEAR 
                                           -- AND ES.COMPONENT_ID=?COMPONENT_ID
                                            AND ES.COMPONENT_ID = CIA.SUP_CIA_COMPONENT_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchSupREADINGComponentbyCourseIdAndERASetting:
                    {
                        sQuery = @"SELECT 
                                        SCIA.COMPONENT_ID, SCIA.COMPONENT
                                    FROM
                                        CP_COURSE_ROOT AS CR
                                            INNER JOIN
                                        CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CR.COURSE_TYPE
                                            INNER JOIN
                                        CP_COURSE_TYPE_GROUP CG ON CG.COURSE_TYPE_ID = CR.COURSE_TYPE
                                            INNER JOIN
                                        CIA_GROUP_COMPONENT AS CIA ON CIA_GROUP_ID = CG.GROUP_ID
                                            INNER JOIN
                                        SUP_CIA_COMPONENTS AS SCIA ON SCIA.COMPONENT_ID = CIA.SUP_CIA_COMPONENT_ID
                                            AND COURSE_ROOT_ID = ?COURSE_ID
                                            INNER JOIN
                                        ERA_SETTING AS ES ON ES.COURSE_ID = CR.COURSE_ROOT_ID
                                            AND ES.IS_DELETED != 1 AND ES.IS_READING =1
                                            AND ES.ACADEMIC_YEAR = ?AC_YEAR 
                                           -- AND ES.COMPONENT_ID=?COMPONENT_ID
                                            AND ES.COMPONENT_ID = CIA.SUP_CIA_COMPONENT_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchReviewedStudentList:
                    {
                        sQuery = @" SELECT 
                                        LAS.REVIEW_ID,
                                        LAS.REVIEW,
                                        SPI.STUDENT_ID,
                                        SPI.ROLL_NO,
                                        COUNT(distinct LAS.REVIEW_ID) AS COUNT,
                                      SUM(ECM.COURSE_GROUP_MARK) AS COURSE_GROUP_MARK
                                    FROM
                                        LI_USER_REVIEW_INFO AS LAS
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON LAS.USER_ID = SPI.STUDENT_ID
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND LAS.ACADEMIC_YEAR = sc.ACADEMIC_YEAR
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            AND C.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = ?COURSE_ID
                                            INNER JOIN
                                        CP_COURSE_TYPE AS CCT ON CCT.COURSE_TYPE_ID = ccr.COURSE_TYPE
                                            INNER JOIN
                                        CP_COURSE_TYPE_GROUP AS CCTG ON CCTG.COURSE_TYPE_ID = CCT.COURSE_TYPE_ID 
                                            INNER JOIN
                                        CIA_GROUP_COMPONENT AS CG ON CG.CIA_GROUP_ID = CCTG.GROUP_ID AND CG.SUP_CIA_COMPONENT_ID=?COMPONENT_ID
                                            AND CG.IS_DELETED != 1
                                            INNER JOIN
                                        ERA_SETTING AS ES ON ES.COURSE_ID = CCR.COURSE_ROOT_ID
                                            AND ES.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ES.IS_ACTIVE = 1 AND ES.COMPONENT_ID=CG.SUP_CIA_COMPONENT_ID
                                            AND ES.SEMESTER_TYPE = (SELECT 
                                                SEMESTER_TYPE_ID
                                            FROM
                                                SUP_SEMESTER_TYPE
                                            WHERE
                                                IS_ACTIVE = 1 AND IS_DELETED != 1)
                                            INNER JOIN
                                        SUP_CIA_COMPONENTS AS SCC ON SCC.COMPONENT_ID = ES.COMPONENT_ID
                                            AND CG.SUP_CIA_COMPONENT_ID = scc.COMPONENT_ID
                                            AND SCC.IS_DELETED != 1
                                            LEFT JOIN
                                        ERA_COMPONENT_MARK AS ECM ON ECM.COURSE_ID = CCR.COURSE_ROOT_ID
                                            AND ECM.IS_DELETED != 1
                                            AND ECM.ERA_SETTING_ID = ES.ERA_SETTING_ID
                                            AND ECM.STUDENT_ID = SC.STUDENT_ID
                                            AND ECM.COURSE_GROUP_ID = CG.CIA_GROUP_COMPONENT_ID
                                            AND ECM.REVIEW_ID = LAS.REVIEW_ID
                                    WHERE
                                        SC.CLASS_ID = ?CLASS_ID AND LAS.IS_DELETED != 1
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY STUDENT_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchReviewedListById:
                    {
                        sQuery = @"SELECT 
                                        LAS.REVIEW_ID,
                                        LAS.REVIEW,
                                        SPI.STUDENT_ID,
                                        SPI.ROLL_NO,
                                        BI.BOOK_NO,
                                        BI.ACCESS_NO,
                                        BI.DOC_TITLE,
                                        BI.PERSONAL_AUTHOR1,
                                        scc.COMPONENT,
                                        LAS.ISSUE_ID,
                                        BI.BOOK_ID,
                                        ES.ERA_SETTING_ID,
                                        PER_BOOK_MARK,
                                        CIA.MAX_MARK,
                                        CIA.CIA_GROUP_COMPONENT_ID,
                                        ECM.COURSE_GROUP_MARK,
                                        ECM.ERA_COMPONENT_ID,
                                        SCC.COMPONENT_ID, 
                                        CCS.COURSE_ID 
                                    FROM
                                        LI_USER_REVIEW_INFO AS LAS
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USER_ID = LAS.USER_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON UA.USERNAME = SPI.ROLL_NO
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR =  ?AC_YEAR
                                            INNER JOIN
                                        LI_BOOK_ISSUE_?AC_YEAR AS LBI ON LBI.ISSUE_ID = LAS.ISSUE_ID
                                            AND SC.STUDENT_ID = LAS.USER_ID
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = LBI.BOOK_ID
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID = SC.CLASS_ID
                                            AND CCS.COURSE_ID = ?COURSE_ID
                                            AND SC.ACADEMIC_YEAR = CCS.ACADEMIC_YEAR
                                            INNER JOIN
                                        ERA_SETTING AS ES ON ES.COURSE_ID = CCS.COURSE_ID
                                            AND es.COMPONENT_ID = ?COMPONENT_ID
                                            AND ES.ACADEMIC_YEAR = CCS.ACADEMIC_YEAR
                                            AND CCS.SEMESTER_TYPE_ID = ES.SEMESTER_TYPE
                                            AND ES.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCS.COURSE_ID
                                            INNER JOIN
                                        CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CR.COURSE_TYPE
                                            INNER JOIN
                                        CP_COURSE_TYPE_GROUP CG ON CG.COURSE_TYPE_ID = CR.COURSE_TYPE
                                            INNER JOIN
                                        CIA_GROUP_COMPONENT AS CIA ON CIA_GROUP_ID = CG.GROUP_ID
                                            INNER JOIN
                                        SUP_CIA_COMPONENTS AS scc ON scc.COMPONENT_ID = CIA.SUP_CIA_COMPONENT_ID
                                            AND scc.COMPONENT_ID = ?COMPONENT_ID
                                            LEFT JOIN
                                        ERA_COMPONENT_MARK AS ECM ON ECM.COURSE_ID = CCS.COURSE_ID
                                            AND ecm.STUDENT_ID = SC.STUDENT_ID
                                            AND ecm.IS_DELETED != 1
                                            AND ES.ERA_SETTING_ID = ecm.ERA_SETTING_ID
                                            AND ecm.REVIEW_ID = LAS.REVIEW_ID
                                    WHERE
                                        SC.STUDENT_ID = ?STUDENT_ID
                                            AND LAS.IS_DELETED != 1
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY REVIEW_ID;";

                        break;
                    }
                case LibrarySQLCommands.SaveERAMarkCourseWise:
                    {
                        sQuery = @"INSERT INTO ERA_COMPONENT_MARK(
                                                        COURSE_ID, 
                                                        STUDENT_ID, 
                                                        STAFF_ID,
                                                        REVIEW_ID, 
                                                        ERA_SETTING_ID,
                                                        COURSE_GROUP_ID, 
                                                        COURSE_GROUP_MARK,
                                                        ACADEMIC_YEAR
                                                   )VALUES(
                                                        ?COURSE_ID, 
                                                        ?STUDENT_ID, 
                                                        ?STAFF_ID,
                                                        ?REVIEW_ID, 
                                                        ?ERA_SETTING_ID,
                                                        ?COURSE_GROUP_ID, 
                                                        ?COURSE_GROUP_MARK,
                                                        ?ACADEMIC_YEAR);";

                        break;
                    }
                case LibrarySQLCommands.UpdateERAMarkCourseWise:
                    {
                        sQuery = @"UPDATE ERA_COMPONENT_MARK SET 
                                                        COURSE_GROUP_MARK=?COURSE_GROUP_MARK, 
                                                        COURSE_GROUP_ID =?COURSE_GROUP_ID,
                                                        ERA_SETTING_ID=?ERA_SETTING_ID
                                                    WHERE
                                                        ERA_COMPONENT_ID =?ERA_COMPONENT_ID;";

                        break;
                    }
                case LibrarySQLCommands.CheckComponentAlreadyExistorNot:
                    {
                        sQuery = @"SELECT 
                                        ERA_COMPONENT_ID,
                                        STUDENT_ID
                                    FROM
                                        ERA_COMPONENT_MARK
                                    WHERE
                                        IS_DELETED != 1
                                        AND REVIEW_ID = ?REVIEW_ID
                                        AND COURSE_GROUP_ID = ?COURSE_GROUP_ID
                                        AND STUDENT_ID = ?STUDENT_ID
                                        AND COURSE_ID = ?COURSE_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchERAComponentMark:
                    {
                        sQuery = @"SELECT 
                                        ECM.STUDENT_ID,
                                        ECM.COURSE_ID,
                                        ECM.COURSE_GROUP_ID,
                                        SUM(ecm.COURSE_GROUP_MARK) AS COURSE_GROUP_MARK,
                                        ccr.SEMESTER_ID,
                                        CIA.MAX_MARK
                                    FROM
                                        ERA_COMPONENT_MARK AS ECM
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = ECM.COURSE_ID
                                            INNER JOIN
                                        CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CCR.COURSE_TYPE
                                            INNER JOIN
                                        CP_COURSE_TYPE_GROUP CG ON CG.COURSE_TYPE_ID = CCR.COURSE_TYPE
                                            INNER JOIN
                                        CIA_GROUP_COMPONENT AS CIA ON CIA_GROUP_ID = CG.GROUP_ID
                                            INNER JOIN
                                        SUP_CIA_COMPONENTS AS scc ON scc.COMPONENT_ID = CIA.SUP_CIA_COMPONENT_ID
                                            AND scc.COMPONENT_ID = ?COMPONENT_ID
                                    WHERE
                                        ECM.STUDENT_ID = ?STUDENT_ID
                                            AND ECM.IS_DELETED != 1
                                            AND ECM.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ecm.COURSE_ID = ?COURSE_ID
                                            AND COURSE_GROUP_ID = ?COURSE_GROUP_ID
                                    GROUP BY ECM.STUDENT_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchCIAGroupCMarks:
                    {
                        sQuery = @"SELECT 
                                        CIA_GROUP_MARK_ID,
                                        COURSE_GROUP_MARK
                                    FROM
                                        CIA_GROUP_MARKS_?AC_YEAR
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID
                                        AND COURSE_ID = ?COURSE_ID
                                        AND COURSE_GROUP_ID = ?COURSE_GROUP_ID;";

                        break;
                    }
                case LibrarySQLCommands.SaveCIAGroupCMarks:
                    {
                        sQuery = @" INSERT INTO CIA_GROUP_MARKS_?AC_YEAR (
                                                                STUDENT_ID, 
                                                                COURSE_ID, 
                                                                COURSE_GROUP_ID,
                                                                COURSE_GROUP_MARK,
                                                                SEMESTER_ID, 
                                                                ENTRY_ID
                                                          )VALUES (
                                                                ?STUDENT_ID, 
                                                                ?COURSE_ID, 
                                                                ?COURSE_GROUP_ID, 
                                                                ?COURSE_GROUP_MARK,
                                                                ?SEMESTER_ID, 
                                                                ?ENTRY_ID);";

                        break;
                    }
                case LibrarySQLCommands.UpdateCIAGroupCMarks:
                    {
                        sQuery = @"UPDATE CIA_GROUP_MARKS_?AC_YEAR SET 
                                                                COURSE_GROUP_MARK=?COURSE_GROUP_MARK,
                                                                ENTRY_ID=?ENTRY_ID
                                                              WHERE 
                                                                CIA_GROUP_MARK_ID =?CIA_GROUP_MARK_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchClassListERA:
                    {
                        sQuery = @"SELECT 
                                    CS.CLASS_ID,
                                    CLS.CLASS_NAME,
                                    CLS.PROGRAMME_GROUP_ID,
                                    CLS.CLASS_LEVEL,
                                    CLS.SHIFT,
                                    CLS.PROGRAMME_MODE,
                                    CLS.DEPARTMENT
                                FROM
                                    CP_CLASS_COURSE_STAFF AS CS
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = CS.CLASS_ID
                                        INNER JOIN
                                    CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID = CS.COURSE_ID
                                        INNER JOIN
                                    SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CS.SEMESTER_TYPE_ID
                                        AND ST.IS_ACTIVE = 1
                                        INNER JOIN
                                    ERA_SETTING AS ES ON ES.COURSE_ID = CS.COURSE_ID
                                        AND CS.ACADEMIC_YEAR = ES.ACADEMIC_YEAR
                                        AND ES.IS_DELETED != 1 
                                        AND ES.IS_ACTIVE=1
                                        AND ST.SEMESTER_TYPE_ID = ES.SEMESTER_TYPE
                                WHERE
                                    CS.STAFF_ID = ?STAFF_ID AND CS.IS_DELETED != 1
                                        AND CS.ACADEMIC_YEAR = ?AC_YEAR
                                GROUP BY CS.CLASS_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchCourseListERA:
                    {
                        sQuery = @"SELECT 
                                        CS.COURSE_ID, 
                                        UPPER(CR.COURSE_TITLE) AS 'COURSE_TITLE'
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CS
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CS.COURSE_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = CS.CLASS_ID
                                            INNER JOIN
                                        SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CS.SEMESTER_TYPE_ID
                                            AND ST.IS_ACTIVE = 1
                                            INNER JOIN
                                        ERA_SETTING AS ES ON ES.COURSE_ID = CS.COURSE_ID
                                            AND CS.ACADEMIC_YEAR = ES.ACADEMIC_YEAR
                                            AND ES.IS_DELETED != 1
                                            AND ES.IS_ACTIVE = 1
                                            AND ST.SEMESTER_TYPE_ID = ES.SEMESTER_TYPE
                                    WHERE
                                        CS.CLASS_ID = ?CLASS_ID AND CS.STAFF_ID = ?STAFF_ID
                                            AND CS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CS.IS_DELETED != 1
                                    GROUP BY COURSE_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchliIssuedBookListbyStudent:
                    {
                        sQuery = @"SELECT 
                                    SPI.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    COUNT(DISTINCT LAS.ISSUE_ID) AS COUNT,
                                    SUM(DISTINCT ecm.COURSE_GROUP_MARK) AS COURSE_GROUP_MARK,
                                    PER_BOOK_MARK
                                FROM
                                    LI_BOOK_ISSUE_?AC_YEAR AS LAS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON LAS.USER_ID = SPI.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                        AND C.IS_DELETED != 1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = ?COURSE_ID
                                        INNER JOIN
                                    CP_COURSE_TYPE AS CCT ON CCT.COURSE_TYPE_ID = ccr.COURSE_TYPE
                                        INNER JOIN
                                    CP_COURSE_TYPE_GROUP AS CCTG ON CCTG.COURSE_TYPE_ID = CCT.COURSE_TYPE_ID
                                        INNER JOIN
                                    CIA_GROUP_COMPONENT AS CG ON CG.CIA_GROUP_ID = CCTG.GROUP_ID
                                        AND CG.SUP_CIA_COMPONENT_ID = ?COMPONENT_ID
                                        AND CG.IS_DELETED != 1
                                        INNER JOIN
                                    ERA_SETTING AS ES ON ES.COURSE_ID = CCR.COURSE_ROOT_ID
                                        AND ES.ACADEMIC_YEAR = ?AC_YEAR
                                        AND ES.IS_ACTIVE = 1
                                        AND ES.COMPONENT_ID = CG.SUP_CIA_COMPONENT_ID
                                        AND ES.SEMESTER_TYPE = (SELECT 
                                            SEMESTER_TYPE_ID
                                        FROM
                                            SUP_SEMESTER_TYPE
                                        WHERE
                                            IS_ACTIVE = 1 AND IS_DELETED != 1)
                                        INNER JOIN
                                    SUP_CIA_COMPONENTS AS SCC ON SCC.COMPONENT_ID = ES.COMPONENT_ID
                                        AND CG.SUP_CIA_COMPONENT_ID = scc.COMPONENT_ID
                                        AND SCC.IS_DELETED != 1
                                        LEFT JOIN
                                    CIA_GROUP_MARKS_?AC_YEAR AS ECM ON ECM.COURSE_ID = CCR.COURSE_ROOT_ID
                                        AND ECM.STUDENT_ID = SC.STUDENT_ID
                                        AND ECM.COURSE_GROUP_ID = CG.CIA_GROUP_COMPONENT_ID
                                        AND ECM.SEMESTER_ID IN (SELECT 
                                            SS.SUP_SEMESTER_ID
                                        FROM
                                            SUP_SEMESTER AS SS
                                        WHERE
                                            SS.IS_DELETED != 1 AND SS.IS_ACTIVE = 1)
                                WHERE
                                    SC.CLASS_ID = ?CLASS_ID AND LAS.IS_DELETED != 1
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                GROUP BY STUDENT_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchliIssuedBookDetailsbyStudentID:
                    {
                        sQuery = @"SELECT 
                                        SPI.STUDENT_ID,
                                        SPI.ROLL_NO,
                                        BI.BOOK_NO,
                                        BI.ACCESS_NO,
                                        BI.DOC_TITLE,
                                        BI.PERSONAL_AUTHOR1,
                                        scc.COMPONENT,
                                        LBI.ISSUE_ID,
                                        BI.BOOK_ID,
                                        ES.ERA_SETTING_ID,
                                        PER_BOOK_MARK,
                                        CIA.MAX_MARK,
                                        CIA.CIA_GROUP_COMPONENT_ID,
                                        ECM.COURSE_GROUP_MARK,
                                        ECM.CIA_GROUP_MARK_ID,
                                        SCC.COMPONENT_ID,
                                        CCS.COURSE_ID
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS LBI
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USER_ID = LBI.USER_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON UA.USERNAME = SPI.ROLL_NO
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        LI_BOOK_INFO AS BI ON BI.BOOK_ID = LBI.BOOK_ID
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID = SC.CLASS_ID
                                            AND CCS.COURSE_ID = ?COURSE_ID
                                            AND CCS.STAFF_ID = ?STAFF_ID
                                            AND SC.ACADEMIC_YEAR = CCS.ACADEMIC_YEAR
                                            INNER JOIN
                                        ERA_SETTING AS ES ON ES.COURSE_ID = CCS.COURSE_ID
                                            AND es.COMPONENT_ID = ?COMPONENT_ID
                                            AND ES.ACADEMIC_YEAR = CCS.ACADEMIC_YEAR
                                            AND CCS.SEMESTER_TYPE_ID = ES.SEMESTER_TYPE
                                            AND ES.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCS.COURSE_ID
                                            INNER JOIN
                                        CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CR.COURSE_TYPE
                                            INNER JOIN
                                        CP_COURSE_TYPE_GROUP CG ON CG.COURSE_TYPE_ID = CR.COURSE_TYPE
                                            INNER JOIN
                                        CIA_GROUP_COMPONENT AS CIA ON CIA_GROUP_ID = CG.GROUP_ID
                                            INNER JOIN
                                        SUP_CIA_COMPONENTS AS scc ON SCC.COMPONENT_ID = CIA.SUP_CIA_COMPONENT_ID
                                            AND SCC.COMPONENT_ID = ?COMPONENT_ID
                                            LEFT JOIN
                                        CIA_GROUP_MARKS_?AC_YEAR AS ECM ON ECM.COURSE_ID = CR.COURSE_ROOT_ID
                                            AND ECM.STUDENT_ID = SC.STUDENT_ID
                                            AND ECM.COURSE_GROUP_ID = CIA.CIA_GROUP_COMPONENT_ID
                                            AND ECM.SEMESTER_ID IN (SELECT 
                                                SS.SUP_SEMESTER_ID
                                            FROM
                                                SUP_SEMESTER AS SS
                                            WHERE
                                                SS.IS_DELETED != 1 AND SS.IS_ACTIVE = 1)
                                    WHERE
                                        SC.STUDENT_ID = ?STUDENT_ID
                                            AND LBI.IS_DELETED != 1
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY ISSUE_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchCIAGroupComponentMarksStudentID:
                    {
                        sQuery = @"SELECT 
                                        ECM.STUDENT_ID,
                                        ECM.COURSE_ID,
                                        ECM.COURSE_GROUP_ID,
                                        ecm.COURSE_GROUP_MARK AS COURSE_GROUP_MARK,
                                        ccr.SEMESTER_ID,
                                        CIA.MAX_MARK,
                                        CIA_GROUP_MARK_ID
                                    FROM
                                        CIA_GROUP_MARKS_?AC_YEAR AS ECM
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = ECM.COURSE_ID
                                            INNER JOIN
                                        CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CCR.COURSE_TYPE
                                            INNER JOIN
                                        CP_COURSE_TYPE_GROUP CG ON CG.COURSE_TYPE_ID = CCR.COURSE_TYPE
                                            INNER JOIN
                                        CIA_GROUP_COMPONENT AS CIA ON CIA_GROUP_ID = CG.GROUP_ID
                                            AND CIA.CIA_GROUP_COMPONENT_ID = ECM.COURSE_GROUP_ID
       
                                            INNER JOIN
                                        SUP_CIA_COMPONENTS AS SCC ON SCC.COMPONENT_ID = CIA.SUP_CIA_COMPONENT_ID
                                            AND SCC.COMPONENT_ID = ?COMPONENT_ID
                                    WHERE
                                        ECM.STUDENT_ID = ?STUDENT_ID
                                            AND ecm.COURSE_ID = ?COURSE_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchCIAGroupComponentIDbyCourseId:
                    {
                        sQuery = @"SELECT 
                                        CCR.COURSE_ROOT_ID AS COURSE_ID,
                                        CIA.CIA_GROUP_COMPONENT_ID as COURSE_GROUP_ID,
                                        CCR.SEMESTER_ID,
                                        CIA.MAX_MARK
                                    FROM
                                        CP_COURSE_ROOT AS CCR
                                            INNER JOIN
                                        CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CCR.COURSE_TYPE AND CT.IS_DELETED !=1
                                            INNER JOIN
                                        CP_COURSE_TYPE_GROUP CG ON CG.COURSE_TYPE_ID = CCR.COURSE_TYPE AND CG.IS_DELETED !=1
                                            INNER JOIN
                                        CIA_GROUP_COMPONENT AS CIA ON CIA_GROUP_ID = CG.GROUP_ID AND CIA.IS_DELETED !=1
                                            INNER JOIN
                                        SUP_CIA_COMPONENTS AS SCC ON SCC.COMPONENT_ID = CIA.SUP_CIA_COMPONENT_ID
                                            AND SCC.COMPONENT_ID = ?COMPONENT_ID
                                    WHERE
                                        CCR.COURSE_ROOT_ID = ?COURSE_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchCIAGroupComponentMarkbyCoursID:
                    {
                        sQuery = @"SELECT 
                                        CIA_GROUP_MARK_ID,
                                        COURSE_GROUP_MARK
                                    FROM
                                        CIA_GROUP_MARKS_?AC_YEAR
                                    WHERE
                                        STUDENT_ID =?STUDENT_ID
                                        AND COURSE_ID = ?COURSE_ID
                                        AND COURSE_GROUP_ID = ?COURSE_GROUP_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchStudentListByClassID:
                    {
                        sQuery = @"SELECT 
                                        C.CLASS_ID,
                                        SP.STUDENT_ID,
                                        C.CLASS_NAME,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        SP.STUDENT_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS 'STUDENT_NAME'
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            AND C.IS_DELETED != 1
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND c.CLASS_ID = ?CLASS_ID
                                            AND SC.IS_DELETED != 1;";

                        break;
                    }
                case LibrarySQLCommands.FetchStudentIssuedBookListByClassID:
                    {
                        sQuery = @" SELECT 
                                        SPI.STUDENT_ID,
                                        SPI.ROLL_NO,
                                        LBI.ACCESS_NO,
                                        lbi.DOC_TITLE,
                                        LBI.PERSONAL_AUTHOR1,
                                        SLS.STATUS_NAME,
                                        LBI.BOOK_NO,
                                        FROM_UNIXTIME(LAS.ISSUE_DATE, '%Y-%m-%d') AS ISSUE_DATE
                                    FROM
                                        LI_BOOK_ISSUE_?AC_YEAR AS LAS
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON LAS.USER_ID = SPI.STUDENT_ID
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            AND C.IS_DELETED != 1
                                            AND SC.IS_DELETED != 1
                                            INNER JOIN
                                        LI_BOOK_INFO AS LBI ON LBI.BOOK_ID = LAS.BOOK_ID
                                            LEFT JOIN
                                        SUP_LI_STATUS AS SLS ON SLS.STATUS_ID = LBI.STATUS
                                    WHERE
                                        SC.CLASS_ID = ?CLASS_ID AND LAS.IS_DELETED != 1
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR;";

                        break;
                    }
                case LibrarySQLCommands.FetchClassListbyClassCourse:
                    {
                        sQuery = @"SELECT 
                                        CS.CLASS_ID,
                                        CLS.CLASS_NAME,
                                        CLS.PROGRAMME_GROUP_ID,
                                        CLS.CLASS_LEVEL,
                                        CLS.SHIFT,
                                        CLS.PROGRAMME_MODE,
                                        CLS.DEPARTMENT
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CS
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = CS.CLASS_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID = CS.COURSE_ID
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CS.SEMESTER_TYPE_ID
                                            AND ST.IS_ACTIVE = 1
                                    WHERE
                                        CS.STAFF_ID = ?STAFF_ID AND CS.IS_DELETED != 1
                                            AND CS.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY CS.CLASS_ID;";

                        break;
                    }
                case LibrarySQLCommands.UpdateBookCollectionbyBookId:
                    {
                        sQuery = @" UPDATE LI_BOOK_INFO 
                                                    SET 
                                                        LI_BOOK_COLLECTION_ID = ?LI_BOOK_COLLECTION_ID
                                                    WHERE
                                                        BOOK_ID = ?BOOK_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchBookInfoBySearchTextforCollection:
                    {
                        sQuery = @"SELECT 
                                        BI.BOOK_ID,
                                        UPPER(BI.DOC_TITLE) AS DOC_TITLE,
                                        UPPER(BI.PARALLEL_TITLE) AS PARALLEL_TITLE,
                                        BI.ISBN_NO,
                                        BI.LI_BOOK_STATUS,
                                        BI.ACCESS_NO,
                                        BS.STATUS_NAME,
                                        BI.CLASS_NO,
                                        BI.BOOK_NO,
                                        SS.STATUS_NAME AS STATUS,
                                        CONCAT(RI.RACK_INFO,' ( ',R.RACK_NAME,' - ',C.CELL_NAME,' ) ') AS RACK_ID,
                                        CONCAT(IFNULL(BI.PERSONAL_AUTHOR1,''),' ',IFNULL(BI.PERSONAL_AUTHOR2,'')) AS PERSONAL_AUTHOR1,
                                        SLC.COLLECTION_NAME
                                    FROM
                                        LI_BOOK_INFO AS BI
                                            LEFT JOIN
                                        LI_RACK_INFO AS RI ON RI.RACK_INFO_ID = BI.RACK_ID
                                            LEFT JOIN
                                        SUP_LI_RACKS AS R ON R.RACK_ID = RI.RACK_ID
                                            LEFT JOIN
                                        SUP_LI_CELLS AS C ON C.CELL_ID = RI.CELL_ID
                                            LEFT JOIN
                                        SUP_LANGUAGE AS L ON L.LANGUAGE_ID = BI.LANGUAGE
                                            LEFT JOIN
                                        SUP_LI_ILLUSTRATION AS LI ON LI.ILLUSTRATE_ID = BI.ILLUSTRATE
                                            LEFT JOIN
                                        SUP_LI_DISCIPLINE AS D ON D.DISCIPLINE_ID = BI.DISIPLINE
                                            LEFT JOIN
                                        SUP_LI_SERIES AS LS ON LS.SERIES_ID = BI.SERIES
                                            LEFT JOIN
                                        SUP_LI_BOOK_TYPE AS BT ON BT.TYPE_ID = BI.DOC_CATEGORY
                                            LEFT JOIN
										SUP_LI_BOOK_STATUS AS BS ON BS.STATUS_ID=BI.LI_BOOK_STATUS                                            
                                            LEFT JOIN
                                        SUP_LI_STATUS AS SS ON SS.STATUS_ID=BI.STATUS
                                            AND SS.IS_DELETED!=1
                                              LEFT JOIN
                                        SUP_LI_BOOK_COLLECTION AS SLC ON SLC.COLLECTION_ID = BI.LI_BOOK_COLLECTION_ID
                                             AND SLC.IS_DELETED != 1
                                    WHERE
                                        BI.IS_DELETED != 1 
                                        AND (DOC_TITLE LIKE '%?SEARCH_TEXT%' 
                                        OR BOOK_NO LIKE '%?SEARCH_TEXT%' 
                                        OR PERSONAL_AUTHOR1 LIKE '%?SEARCH_TEXT%'
                                        OR BARCODE  LIKE '%?SEARCH_TEXT%'
                                        OR ACCESS_NO LIKE '%?SEARCH_TEXT%');";
                        break;
                    }
                case LibrarySQLCommands.FetchERASettingYearWise:
                    {
                        sQuery = @"SELECT 
                                        ES.PER_BOOK_MARK,
                                        ES.IS_READING,
                                        ES.IS_WRITING,
                                        ES.ERA_SETTING_ID,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        SC.COMPONENT,
                                        ES.IS_ACTIVE
                                    FROM
                                        ERA_SETTING AS ES
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = ES.COURSE_ID
                                            INNER JOIN
                                        SUP_CIA_COMPONENTS AS SC ON SC.COMPONENT_ID = ES.COMPONENT_ID
                                    WHERE
                                        ES.IS_DELETED != 1
                                            AND ES.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ES.SEMESTER_TYPE = ?SEMESTER_TYPE;";
                        break;
                    }
                case LibrarySQLCommands.DeleteERASettingById:
                    {
                        sQuery = @"UPDATE ERA_SETTING 
                                                        SET 
                                                            IS_DELETED = ?IS_DELETED
                                                        WHERE
                                                            ERA_SETTING_ID = ?ERA_SETTING_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchERASettingById:
                    {
                        sQuery = @" SELECT 
                                        ES.PER_BOOK_MARK,
                                        ES.IS_READING,
                                        ES.IS_WRITING,
                                        ES.ERA_SETTING_ID,
                                        CCR.COURSE_CODE,
                                        ES.COURSE_ID,
                                        ES.COMPONENT_ID,
                                        CCR.COURSE_TITLE,
                                        SC.COMPONENT,
                                        ES.IS_ACTIVE,
                                        ES.ACADEMIC_YEAR,
                                        ES.SEMESTER_TYPE
                                    FROM
                                        ERA_SETTING AS ES
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = ES.COURSE_ID
                                            INNER JOIN
                                        SUP_CIA_COMPONENTS AS SC ON SC.COMPONENT_ID = ES.COMPONENT_ID
                                    WHERE
                                        ES.IS_DELETED != 1 and ERA_SETTING_ID=?ERA_SETTING_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchComponentByCourseCode:
                    {
                        sQuery = @"SELECT 
                                        SCC.COMPONENT,
                                        SCC.COMPONENT_ID
                                    FROM
                                        CP_COURSE_ROOT AS CCR
                                            INNER JOIN
                                        CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CCR.COURSE_TYPE
                                            AND CT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_TYPE_GROUP CG ON CG.COURSE_TYPE_ID = CCR.COURSE_TYPE
                                            AND CG.IS_DELETED != 1
                                            INNER JOIN
                                        CIA_GROUP_COMPONENT AS CIA ON CIA_GROUP_ID = CG.GROUP_ID
                                            AND CIA.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_CIA_COMPONENTS AS SCC ON SCC.COMPONENT_ID = CIA.SUP_CIA_COMPONENT_ID
                                    WHERE
                                        CCR.COURSE_CODE = ?COURSE_CODE
                                            AND SCC.IS_DELETED != 1;";

                        break;
                    }
                case LibrarySQLCommands.CheckCourseCodeExistOrNots:
                    {
                        sQuery = @"SELECT 
                                        COURSE_ROOT_ID,
                                        COURSE_CODE
                                    FROM
                                        CP_COURSE_ROOT
                                    WHERE
                                        COURSE_CODE = ?COURSE_CODE
                                            AND IS_DELETED != 1;";

                        break;
                    }
                case LibrarySQLCommands.SaveERASetting:
                    {
                        sQuery = @" INSERT INTO ERA_SETTING
                                                     (COURSE_ID,
                                                     ACADEMIC_YEAR,
                                                     COMPONENT_ID,
                                                     SEMESTER_TYPE,
                                                     IS_ACTIVE,
                                                     PER_BOOK_MARK,
                                                     IS_READING,
                                                     IS_WRITING)
                                                VALUES( 
                                                     ?COURSE_ID,
                                                     ?ACADEMIC_YEAR,
                                                     ?COMPONENT_ID,
                                                     ?SEMESTER_TYPE,
                                                     ?IS_ACTIVE,
                                                     ?PER_BOOK_MARK,
                                                     ?IS_READING,
                                                     ?IS_WRITING);";

                        break;
                    }
                case LibrarySQLCommands.UpdateERASetting:
                    {
                        sQuery = @"UPDATE ERA_SETTING 
                                                    SET 
                                                     COURSE_ID =?COURSE_ID,
                                                     ACADEMIC_YEAR =?ACADEMIC_YEAR,
                                                     COMPONENT_ID =?COMPONENT_ID,
                                                     SEMESTER_TYPE =?SEMESTER_TYPE,
                                                     IS_ACTIVE =?IS_ACTIVE,
                                                     PER_BOOK_MARK =?PER_BOOK_MARK,
                                                     IS_READING =?IS_READING,
                                                     IS_WRITING =?IS_WRITING
                                                WHERE 
                                                    ERA_SETTING_ID=?ERA_SETTING_ID;";

                        break;
                    } 
                case LibrarySQLCommands.CheckSameERASettingExist:
                    {
                        sQuery = @"SELECT 
                                        ERA_SETTING_ID,
                                        COURSE_ID
                                    FROM
                                        ERA_SETTING
                                    WHERE
                                        COURSE_ID = ?COURSE_ID 
                                        AND COMPONENT_ID = ?COMPONENT_ID
                                        AND ACADEMIC_YEAR = ?AC_YEAR
                                        AND SEMESTER_TYPE = ?SEMESTER_TYPE
                                        AND IS_DELETED != 1;";

                        break;
                    }
                case LibrarySQLCommands.ActiveOrDeactiveERASetting:
                    {
                        sQuery = @"UPDATE ERA_SETTING 
                                                        SET 
                                                            IS_ACTIVE = ?IS_ACTIVE
                                                        WHERE
                                                            ERA_SETTING_ID = ?ERA_SETTING_ID;";

                        break;
                    }
                case LibrarySQLCommands.fetchStudentClassAndShift:
                    {
                            sQuery = @"SELECT 
                                            SPI.STUDENT_ID,
                                            CC.CLASS_YEAR,
                                            CC.CLASS_LEVEL,
                                            CC.SHIFT
                                        FROM
                                            STU_PERSONAL_INFO AS SPI
                                                INNER JOIN
                                            STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                                INNER JOIN
                                            CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        WHERE
                                            SPI.STUDENT_ID = ?STUDENT_ID
                                                AND sc.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SPI.IS_LEFT != 1
                                                AND SPI.IS_DELETED != 1;";

                        break;
                    }
                case LibrarySQLCommands.FetchStudentLiBookIssueAndreviewByClassID:
                    {
                            sQuery = @"SELECT 
                                            SPI.STUDENT_ID,
                                            SPI.ROLL_NO,
                                            LBI.ACCESS_NO,
                                            lbi.DOC_TITLE,
                                            LBI.PERSONAL_AUTHOR1,
                                            SLS.STATUS_NAME,
                                            LBI.BOOK_NO,
                                            FROM_UNIXTIME(LAS.ISSUE_DATE, '%Y-%m-%d') AS ISSUE_DATE,
                                            REVIEW,
                                            ecm.COURSE_GROUP_MARK,
                                            LBI.LI_BOOK_COLLECTION_ID
                                        FROM
                                            LI_BOOK_ISSUE_?AC_YEAR AS LAS
                                                INNER JOIN
                                            STU_PERSONAL_INFO AS SPI ON LAS.USER_ID = SPI.STUDENT_ID
                                                INNER JOIN
                                            STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                                INNER JOIN
                                            CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                                AND C.IS_DELETED != 1
                                                AND SC.IS_DELETED != 1
                                                INNER JOIN
                                            LI_BOOK_INFO AS LBI ON LBI.BOOK_ID = LAS.BOOK_ID
                                                LEFT JOIN
                                            SUP_LI_STATUS AS SLS ON SLS.STATUS_ID = LBI.STATUS
                                                LEFT JOIN
                                            LI_USER_REVIEW_INFO AS LRI ON LRI.ISSUE_ID = LAS.ISSUE_ID
                                                AND LAS.USER_ID = LRI.USER_ID
                                                AND LRI.ACADEMIC_YEAR = ?AC_YEAR
                                                LEFT JOIN
                                            ERA_COMPONENT_MARK AS ECM ON ECM.REVIEW_ID = LRI.REVIEW_ID
                                        WHERE
                                            SC.CLASS_ID = ?CLASS_ID AND LAS.IS_DELETED != 1
                                                AND SPI.IS_DELETED != 1
                                                AND SPI.IS_LEFT != 1
                                                AND SC.ACADEMIC_YEAR = ?AC_YEAR;";

                        break;
                    }
                case LibrarySQLCommands.FetchListeningAudioFiles:
                    {
                            sQuery = @"SELECT 
                                            ELA.LISTENING_ID,
                                            ELA.DESCRIPTION,
                                            ELA.AUDIO_URL,
                                            DATE_FORMAT(ela.AVAILABLE_FROM_DATE, '%d/%m/%Y') AS AVAILABLE_FROM_DATE,
                                            DATE_FORMAT(ela.AVAILABLE_TO_DATE, '%d/%m/%Y') AS AVAILABLE_TO_DATE,
                                            ELA.DURATION_SECONDS,
                                            ELA.CLASS_YEAR,
                                            ELA.SEMESTER_TYPE,
                                            ELA.ACADEMIC_YEAR,
                                            ELQA.ANSWER_ID,
                                            ELQ.QUESTION_ID,
                                              CASE 
                                                WHEN COUNT(elqa.ANSWER_ID) > 0 THEN '1'
                                                ELSE '0'
                                            END AS HAS_ATTEMPTED
                                        FROM
                                            ERA_LISTENING_AUDIO_FILES AS ELA
                                                LEFT JOIN
                                            ERA_LISTENING_QUIZ_QESTION AS ELQ ON ELA.LISTENING_ID = ELQ.LISTENING_ID
                                                LEFT JOIN
                                            ERA_LISTENING_QUIZ_ANSWERS AS ELQA ON ELQA.LISTENING_ID = ELA.LISTENING_ID
                                                AND ELQA.QUESTION_ID = ELQ.QUESTION_ID
                                                AND ELQA.IS_DELETED != 1
                                                AND ELQA.STUDENT_ID = ?STUDENT_ID
                                        WHERE
                                            ELA.IS_ACTIVE = 1
                                                AND ELA.IS_DELETED != 1
                                                AND ELA.ACADEMIC_YEAR = ?AC_YEAR
                                                AND ELA.SEMESTER_TYPE = ?SEMESTER_TYPE
                                                AND ELA.CLASS_YEAR = ?CLASS_YEAR
                                        GROUP BY ELA.LISTENING_ID
                                        ORDER BY AVAILABLE_FROM_DATE , ela.LISTENING_ID;";

                        break;
                    }
                case LibrarySQLCommands.FetchQuizQuestionByListeningId:
                    {
                            sQuery = @"SELECT 
                                            QUESTION_ID,
                                            LISTENING_ID,
                                            ACADEMIC_YEAR,
                                            QUESTION_TEXT,
                                            OPTION_A,
                                            OPTION_B,
                                            OPTION_C,
                                            OPTION_D,
                                            CORRECT_ANSWER
                                        FROM
                                            ERA_LISTENING_QUIZ_QESTION
                                        WHERE
                                            LISTENING_ID = ?LISTENING_ID AND IS_DELETED != 1;";

                        break;
                    }
                case LibrarySQLCommands.SaveListeningQuizAnswer:
                    {
                            sQuery = @"INSERT INTO ERA_LISTENING_QUIZ_ANSWERS
                                                                        (LISTENING_ID,
                                                                        QUESTION_ID,
                                                                        STUDENT_ID,
                                                                        SELECTED_ANSWER,
                                                                        ACADEMIC_YEAR
                                                                        )
                                                                    VALUES
                                                                        (?LISTENING_ID,
                                                                        ?QUESTION_ID,
                                                                        ?STUDENT_ID,
                                                                        ?SELECTED_ANSWER,
                                                                        ?ACADEMIC_YEAR);";

                        break;
                    }
                case LibrarySQLCommands.FetchQuizQuestionAnswerByListeningId:
                    {
                            sQuery = @"SELECT 
                                            q.LISTENING_ID,
                                            lf.DESCRIPTION,
                                            q.QUESTION_ID,
                                            q.QUESTION_TEXT,
                                            q.OPTION_A,
                                            q.OPTION_B,
                                            q.OPTION_C,
                                            q.OPTION_D,
                                            q.CORRECT_ANSWER,
                                            a.SELECTED_ANSWER,
                                            CASE 
                                                WHEN q.CORRECT_ANSWER = a.SELECTED_ANSWER THEN 'P'
                                                ELSE 'F'
                                            END AS RESULT,
                                            a.STUDENT_ID,
                                            a.ACADEMIC_YEAR,
                                            a.SUBMITTED_AT 
                                        FROM 
                                            ERA_LISTENING_QUIZ_QESTION q
                                        INNER JOIN 
                                            ERA_LISTENING_AUDIO_FILES lf ON q.LISTENING_ID = lf.LISTENING_ID
                                        Inner JOIN 
                                            ERA_LISTENING_QUIZ_ANSWERS a 
                                            ON q.QUESTION_ID = a.QUESTION_ID 
                                            AND q.LISTENING_ID = a.LISTENING_ID
                                            AND a.IS_DELETED != 1
    
                                        WHERE 
                                            a.STUDENT_ID = ?STUDENT_ID 
                                            and q.LISTENING_ID =?LISTENING_ID
                                            AND a.ACADEMIC_YEAR = ?AC_YEAR
                                            AND q.IS_DELETED != 1
                                            AND lf.IS_DELETED != 1
                                        ORDER BY 
                                            q.LISTENING_ID, q.QUESTION_ID;";

                        break;
                    }
                case LibrarySQLCommands.UpdateLibraryCheckout:
                    {
                        sQuery = @"UPDATE LI_CHECKIN_INFO_?AC_YEAR
                                   SET 
                                       CHECKOUT_DATE = ?CHECKOUT_DATE,IS_CHECKOUT=1
                                   WHERE 
                            
                                        CHECKIN_ID=?CHECKIN_ID";
                                      
                        break;
                    }

                default:
                    break;
            }
            return sQuery;
        }
    }
}
