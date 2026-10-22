using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAO;
namespace CMS.SQL.Hostel
{
    public static class HostelSQL
    {
        public static string GetHostelSQL(HostelSQLCommands sEnumCommand)
        {
            string query = string.Empty;
            switch (sEnumCommand)
            {
                case HostelSQLCommands.UploadStudentImageByStudentId:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET PHOTO=?PHOTO WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case HostelSQLCommands.UpdateApplicationSubmittionForHostel:
                    {
                        query = @"UPDATE HOSTEL_RENEWAL_REGISTRARION SET IS_SUBMITTED =?IS_SUBMITTED , STATUS=?STATUS WHERE RENEWAL_REGISTRATION_ID =?RENEWAL_REGISTRATION_ID;";
                        break;
                    }
                case HostelSQLCommands.UpdateFamilyPhoto:
                    {
                        query = @"UPDATE HOSTEL_RENEWAL_REGISTRARION SET FAMILY_PHOTO =?FAMILY_PHOTO WHERE STUDENT_ID =?STUDENT_ID;";
                        break;
                    }
                case HostelSQLCommands.DeleteStudentRelation:
                    {
                        query = @"UPDATE ADM_STU_RELATIONS SET IS_DELETED=1 WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case HostelSQLCommands.UpdateHostelRegistration:
                    {
                        query = @"UPDATE HOSTEL_RENEWAL_REGISTRARION
                                            SET
                                            CLASS_ID= ?CLASS_ID,
                                            PARISH_NAME= ?PARISH_NAME,
                                            COLLEGE_RELATION= ?COLLEGE_RELATION,
                                            STUDENT_INFO= ?STUDENT_INFO,
                                            ACADEMIC_YEAR= ?AC_YEAR
                                            WHERE RENEWAL_REGISTRATION_ID= ?RENEWAL_REGISTRATION_ID;";
                        break;
                    }
                case HostelSQLCommands.SaveHostelRegistration:
                    {
                        query = @"INSERT INTO HOSTEL_RENEWAL_REGISTRARION
                                            (
                                            STUDENT_ID,
                                            CLASS_ID,
                                            ROOM_NO,
                                            PARISH_NAME,
                                            COLLEGE_RELATION,
                                            STUDENT_INFO,
                                            ACADEMIC_YEAR)
                                            VALUES
                                            (
                                            ?STUDENT_ID,
                                            ?CLASS_ID,
                                            ?ROOM_NO,
                                            ?PARISH_NAME,
                                            ?COLLEGE_RELATION,
                                            ?STUDENT_INFO,
                                            ?AC_YEAR);";
                        break;
                    }
                case HostelSQLCommands.UpdateStuPersonalByStudentId:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET 
	                                FIRST_NAME=?FIRST_NAME,
	                                LAST_NAME=?LAST_NAME,
	                                DATE_OF_BIRTH=?DATE_OF_BIRTH,
	                                PROGRAM_ID=?PROGRAM_ID,
	                                CLASS_ID=?CLASS_ID,
	                                RELIGION=?RELIGION,
	                                COMMUNITY=?COMMUNITY,
	                                BLOOD_GROUP=?BLOOD_GROUP,
                                    TALENT=?TALENT,
                                    STU_EMAILID=?STU_EMAILID,
                                    IS_RELATION=?IS_RELATION WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case HostelSQLCommands.FetchSiblingsList:
                    {
                        query= @"SELECT 
                                    SIBLING_ID, 
                                    STUDENT_ID,
                                    NAME,
                                    QUALIFICATION,
                                    EMPLOYED, 
                                    DATE_FORMAT(DATE_OF_BIRTH,'%d/%m/%Y') AS 'DATE_OF_BIRTH',
                                    GENDER,
                                    OCCUPATION,
                                    MONTHLY_INCOME,
                                    IS_SAME_COLLEGE
                                FROM
                                    STU_SIBLING
                                WHERE
                                    STUDENT_ID = ?STUDENT_ID AND IS_DELETED!=2;";
                        break;
                    }
                case HostelSQLCommands.FetchStudentList:
                    {
                        query = @"SELECT 
	                                SP.STUDENT_ID,
                                    SP.ADMISSION_NO,
                                    SP.FIRST_NAME,
                                    SP.LAST_NAME,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y')AS 'DATE_OF_BIRTH',
                                    SP.RELIGION,
                                    SP.BLOOD_GROUP,
                                    SP.CASTE,
                                    SP.COMMUNITY,
                                    SP.PHOTO,
                                    SP.PROGRAM_ID,
                                    SP.STU_MOBILENO,
                                    SP.FR_MOBILE,
                                    SP.TALENT,
                                    SP.IS_RELATION,
                                    SC.CLASS_ID,
                                    D.ADDRESS_ID, 
                                    D.C_DOOR_DETAIL, 
                                    C_STREET, 
                                    C_VILLAGE_AREA, 
                                    C_POST_PLACE_TOWN, 
                                    C_TALUK, C_CITY, 
                                    C_DISTRICT, C_PINCODE, 
                                    C_COUNTRY, P_DOOR_DETAIL, 
                                    P_STREET, P_VILLAGE_AREA, 
                                    P_POST_PLACE_TOWN, P_TALUK, 
                                    P_CITY, P_DISTRICT, 
                                    P_PINCODE, P_COUNTRY,
                                    P_PHONENO,C_PHONENO,
                                    RENEWAL_REGISTRATION_ID, 
                                    HOSTEL_ID, ROOM_NO, SP.STU_EMAILID,
                                    PARISH_NAME,HR.IS_SUBMITTED,HR.FAMILY_PHOTO
                                FROM
                                    STU_CLASS SC 
		                                INNER JOIN
	                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
		                                LEFT JOIN
	                                STU_ADDRESS AS D ON D.STUDENT_ID=SC.STUDENT_ID
		                                INNER JOIN
	                                HOSTEL_RENEWAL_REGISTRARION AS HR ON HR.STUDENT_ID=SC.STUDENT_ID 
                                WHERE SC.IS_DELETED!=1 AND SC.STUDENT_ID=?STUDENT_ID AND SC.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case HostelSQLCommands.FetchStudentRelationsByStudentId:
                    {
                        query = @"SELECT 
                                        SR.RELATION_ID,
                                        STUDENT_ID,
                                        RELATION_NAME,
                                        RELATION_SHIP,
                                        OCCUPATION,
                                        INCOME,
                                        ACADEMIC_YEAR,
                                        SPR.RELATION
                                    FROM
                                        STU_RELATIONS AS SR
                                        INNER JOIN
									SUP_RELATION AS SPR ON SPR.RELATION_ID=SR.RELATION_SHIP
                                    WHERE
                                        STUDENT_ID =?STUDENT_ID AND SR.IS_DELETED!=1;";
                        break;
                    }
                case HostelSQLCommands.InsertStudentRelation:
                    {
                        query = @"INSERT INTO STU_RELATIONS(
	                                STUDENT_ID,
	                                RELATION_NAME,
	                                RELATION_SHIP,
	                                OCCUPATION,
	                                INCOME)
                                 VALUES(
                                    ?STUDENT_ID,
	                                ?RELATION_NAME,
	                                ?RELATION_SHIP,
	                                ?OCCUPATION,
	                                ?INCOME);";
                        break;
                    }
                case HostelSQLCommands.UpdateStudentRelation:
                    {
                        query = @"UPDATE STU_RELATIONS SET
	                                RELATION_NAME = ?RELATION_NAME,
	                                RELATION_SHIP = ?RELATION_SHIP,
	                                OCCUPATION = ?OCCUPATION,
	                                INCOME = ?INCOME
                                WHERE RELATION_ID = ?RELATION_ID;";
                        break;
                    }
                case HostelSQLCommands.FetchStudentListForPrint:
                    {
                        query = @"SELECT 
	                                SP.STUDENT_ID,
                                    SP.ADMISSION_NO,
                                    SP.FIRST_NAME,
                                    SP.LAST_NAME,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y')AS 'DATE_OF_BIRTH',
                                    R.RELIGION ,
                                    SP.RELIGION AS 'RELIGION_ID',
                                    S.COMMUNITY AS 'CASTE',
                                    B.BLOOD_GROUP_NAME,
                                    SP.PHOTO,
                                    SP.FR_MOBILE,
                                    CP.PROGRAMME_NAME,
                                    C.CLASS_NAME,
                                    SP.TALENT,
                                    SP.IS_RELATION,
                                    C_VILLAGE_AREA, 
                                    C_DOOR_DETAIL, 
                                    C_TALUK, C_DISTRICT,
                                    C_PHONENO,SP.STU_EMAILID,
                                    PARISH_NAME,HR.IS_SUBMITTED,HR.FAMILY_PHOTO
                                FROM
                                    STU_CLASS SC 
		                                INNER JOIN
	                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1 AND SC.ACADEMIC_YEAR=?AC_YEAR
		                                LEFT JOIN
	                                STU_ADDRESS AS D ON D.STUDENT_ID=SC.STUDENT_ID
		                                LEFT JOIN
	                                HOSTEL_RENEWAL_REGISTRARION AS HR ON HR.STUDENT_ID=SC.STUDENT_ID 
										INNER JOIN
									CP_CLASSES AS C ON C.CLASS_ID =SC.CLASS_ID
										LEFT JOIN
									CP_PROGRAMME_GROUP AS P ON P.PROGRAMME_GROUP_ID=SP.PROGRAM_ID
										LEFT JOIN
									CP_PROGRAMME AS CP ON CP.PROGRAMME_ID=P.PROGRAMME_ID
										INNER JOIN
									SUP_RELIGION AS R ON R.RELIGIONID=SP.RELIGION
										LEFT JOIN
									SUP_BLOOD_GROUP AS B ON B.BLOOD_GROUP_ID=SP.BLOOD_GROUP
										LEFT JOIN
									SUP_COMMUNITY AS S ON S.COMMUNITYID = SP.COMMUNITY
                                WHERE SC.IS_DELETED!=1 AND SC.STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case HostelSQLCommands.FetchHostelInfoByStudentId:
                    {
                        query = @"SELECT 
                                        RENEWAL_REGISTRATION_ID
                                    FROM
                                        HOSTEL_RENEWAL_REGISTRARION
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case HostelSQLCommands.UpdateFeePaidForHostel:
                    {
                        query = @"UPDATE HOSTEL_RENEWAL_REGISTRARION SET IS_FEE_PAID =1 WHERE RENEWAL_REGISTRATION_ID =?RENEWAL_REGISTRATION_ID;";
                        break;
                    }
                case HostelSQLCommands.FetchHostelStudentByClassYear:
                    {
                        query = @"SELECT 
                                HS.STUDENT_ID,
                                CONCAT(IFNULL(UPPER(SP.FIRST_NAME), ''),
                                        ' ',
                                        IFNULL(UPPER(SP.LAST_NAME), '')) AS NAME,
                                SP.ROLL_NO,
                                SH.HOSTEL_NAME,
                                DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                HS.HOSTEL_ID,
                                CC.CLASS_NAME
                            FROM
                                HOSTEL_STUDENT_INFO AS HS
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = HS.STUDENT_ID AND SC.ACADEMIC_YEAR=HS.ACADEMIC_YEAR
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1
                                    INNER JOIN
                                SUP_HOSTEL AS SH ON SH.HOSTEL_ID = HS.HOSTEL_ID
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                            WHERE
		                            HS.HOSTEL_ID = ?HOSTEL_ID
                                    AND CC.CLASS_YEAR IN (?CLASS_YEAR)
                                    AND HS.ACADEMIC_YEAR =?ACADEMIC_YEAR;";
                        break;
                    }
                case HostelSQLCommands.IsStuClassPromoted:
                    {
                        query = @"SELECT 
                                STUDENT_ID
                            FROM
                                STU_CLASS
                            WHERE
                                STUDENT_ID =?STUDENT_ID
                                    AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case HostelSQLCommands.IsStudentExist:
                    {
                        query = @"SELECT 
                                STU_HOSTEL_ID,STUDENT_ID
                            FROM
                                HOSTEL_STUDENT_INFO
                            WHERE
                                STUDENT_ID =?STUDENT_ID  AND ACADEMIC_YEAR =?AC_YEAR;";
                        break;
                    }
                case HostelSQLCommands.SaveStudentPromotion:
                    {
                        query = @"INSERT INTO
                                     HOSTEL_STUDENT_INFO
                                     (STUDENT_ID,
                                     HOSTEL_ID,
                                     ADMITTED_YEAR,
                                     ACADEMIC_YEAR)  
                                      VALUES
                                      (?STUDENT_ID,
                                      ?HOSTEL_ID,
                                      ?ADMITTED_YEAR,
                                      ?ACADEMIC_YEAR);";
                        break;
                    }
                case HostelSQLCommands.UpdateStudentPromotion:
                    {
                        query = @"UPDATE HOSTEL_STUDENT_INFO 
                        SET 
                            HOSTEL_ID =?HOSTEL_ID
                        WHERE
                            STUDENT_ID = ?STUDENT_ID AND ACADEMIC_YEAR =?ACADEMIC_YEAR;";
                        break;
                    }
                #region Hostel Student Info
                case HostelSQLCommands.ListHostelStudents:
                    {
                        query = @"SELECT 
                            HS.STU_HOSTEL_ID,
                            SP.STUDENT_ID,
                            SP.ROLL_NO,
                            SP.REGISTER_NO,
                            CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                    ' ',
                                    IFNULL(SP.LAST_NAME, '')) AS NAME,
                            C.CLASS_ID,
                            C.CLASS_NAME,
                            H.HOSTEL_NAME,
                            HS.ROOM_NO,
                            HS.COT_NO,
                            HS.BLOCK_ID,
                            B.BLOCK_NAME,
                            HS.ADMITTED_YEAR,
                            HS.IS_LEFT
                        FROM
                            STU_CLASS AS SC
                                INNER JOIN
                            STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                AND SP.IS_DELETED != 1
                                AND SP.IS_LEFT != 1
                                INNER JOIN
                            HOSTEL_STUDENT_INFO AS HS ON HS.STUDENT_ID = SC.STUDENT_ID
                                AND HS.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                AND HS.IS_DELETED != 1
                                LEFT JOIN
                            SUP_HOSTEL AS H ON H.HOSTEL_ID = HS.HOSTEL_ID
                                INNER JOIN
                            CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                LEFT JOIN
                            SUP_BLOCK AS B ON B.BLOCK_ID = HS.BLOCK_ID
                        WHERE
                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                AND SC.IS_DELETED != 1;";
                        break;
                    }
                case HostelSQLCommands.FetchHostelStudentInfoById:
                    {
                        query = @"SELECT 
                                        STU_HOSTEL_ID,
                                        STUDENT_ID,
                                        HOSTEL_ID,
                                        BLOCK_ID,
                                        ROOM_NO,
                                        COT_NO,
                                        BLOCK_INCHARGE,
                                        ADMITTED_YEAR,
                                        DATE_FORMAT(ADMITTED_DATE,'%d/%m/%Y') AS ADMITTED_DATE,
                                        ACADEMIC_YEAR,
                                        IS_LEFT
                                    FROM
                                        HOSTEL_STUDENT_INFO
                                    WHERE
                                        STU_HOSTEL_ID = ?STU_HOSTEL_ID AND IS_DELETED != 1;";
                        break;
                    }
                case HostelSQLCommands.IsExistHostelStudent:
                    {
                        query = @"SELECT STU_HOSTEL_ID,STUDENT_ID FROM HOSTEL_STUDENT_INFO WHERE STUDENT_ID=?STUDENT_ID AND ACADEMIC_YEAR=?AC_YEAR AND IS_DELETED!=1;";
                        break;
                    }
                case HostelSQLCommands.SaveHostelStudent:
                    {
                        query = @"INSERT INTO HOSTEL_STUDENT_INFO
                                  (
                                      STUDENT_ID,
                                      HOSTEL_ID,
                                      BLOCK_ID,
                                      ROOM_NO,
                                      COT_NO,
                                      BLOCK_INCHARGE,
                                      ADMITTED_YEAR,
                                      ADMITTED_DATE,
                                      ACADEMIC_YEAR,
                                      IS_LEFT
                                  )
                                  VALUES
                                  (
                                      ?STUDENT_ID,
                                      ?HOSTEL_ID,
                                      ?BLOCK_ID,
                                      ?ROOM_NO,
                                      ?COT_NO,
                                      ?BLOCK_INCHARGE,
                                      ?ADMITTED_YEAR,
                                      ?ADMITTED_DATE,
                                      ?AC_YEAR,
                                      ?IS_LEFT
                                  );";
                        break;
                    }
                case HostelSQLCommands.UpdateHostelStudent:
                    {
                        query = @"UPDATE HOSTEL_STUDENT_INFO
                                  SET
                                  STUDENT_ID = ?STUDENT_ID,
                                  HOSTEL_ID = ?HOSTEL_ID,
                                  BLOCK_ID = ?BLOCK_ID,
                                  ROOM_NO = ?ROOM_NO,
                                  COT_NO = ?COT_NO,
                                  BLOCK_INCHARGE = ?BLOCK_INCHARGE,
                                  ADMITTED_YEAR = ?ADMITTED_YEAR,
                                  ADMITTED_DATE = ?ADMITTED_DATE,
                                  ACADEMIC_YEAR = ?AC_YEAR,
                                  IS_LEFT = ?IS_LEFT
                                  WHERE STU_HOSTEL_ID = ?STU_HOSTEL_ID;";
                        break;
                    }
                case HostelSQLCommands.DeleteHostelStudent:
                    {
                        query = @"UPDATE HOSTEL_STUDENT_INFO SET IS_DELETED=1 WHERE STU_HOSTEL_ID=?STU_HOSTEL_ID;";
                        break;
                    }
                case HostelSQLCommands.GetBlockByHostelId:
                    {
                        query = @"SELECT 
                                        BLOCK_ID,
                                        BLOCK_NAME
                                    FROM
                                        SUP_BLOCK
                                    WHERE
                                        HOSTEL_ID = ?HOSTEL_ID AND IS_DELETED!=1;";
                        break;
                    }
                case HostelSQLCommands.FetchStudentInfoForSMS:
                    {
                        query = @"SELECT 
                                SP.STUDENT_ID,
                                UPPER(CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, ''),
                                                ' - ',
                                                SP.ROLL_NO)) AS FIRST_NAME,
                                (IFNULL(SP.STU_MOBILENO,
                                        IFNULL(SP.FR_MOBILE, IFNULL(MO_MOBILE, '')))) AS STU_MOBILENO,
                                SH.HOSTEL_NAME
                            FROM
                                STU_PERSONAL_INFO AS SP
                                    INNER JOIN
                                HOSTEL_STUDENT_INFO AS HS ON HS.STUDENT_ID = SP.STUDENT_ID
                                    AND HS.IS_DELETED != 1
                                    AND HS.IS_LEFT != 1
                                    AND HS.ACADEMIC_YEAR = ?AC_YEAR
                                    INNER JOIN
                                SUP_HOSTEL AS SH ON SH.HOSTEL_ID = HS.HOSTEL_ID
                            WHERE
                                SP.IS_DELETED != 1 AND SP.IS_LEFT != 1
                                    AND SP.STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                #endregion
                default:
                    break;
            }
            return query;
        }
    }
}
