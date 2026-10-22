using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAO;
using CMS.Utility;

namespace CMS.SQL.Account
{
    public class AccountSQL
    {
        public static string GetAccountSQL(AccountSQLCommands sEnumCommand)
        {
            string query = string.Empty;
            switch (sEnumCommand)
            {
                case AccountSQLCommands.UpdatePassword:
                    {
                        query = @"UPDATE USER_ACCOUNT SET PASSWORD=?CONFIRM_PASSWORD WHERE USERNAME=?USERNAME AND USER_ACCOUNT_ID!=0";
                        break;
                    }
                case AccountSQLCommands.FetchPassword:
                    {
                        query = @"SELECT 
                                        PASSWORD
                                    FROM
                                        USER_ACCOUNT
                                    WHERE
                                        USERNAME = ?USERNAME AND IS_DELETED !=1;";
                        break;
                    }
                case AccountSQLCommands.FetchEmailAddress:
                    {
                        query = @"";
                        break;
                    }
                case AccountSQLCommands.FetchUserAccountByRole:
                    {
                        query = @"SELECT 
                                        USER_ACCOUNT_ID,
                                        USERNAME,
                                        ROLE_NAME AS ROLE,
                                        NAME,
                                        EMAIL,
                                        MOBILE,
                                        USER_ID,
                                        USER_TYPE_NAME AS USER_TYPE,
                                        PHOTO
                                    FROM
                                        USER_ACCOUNT AS UA
                                            INNER JOIN
                                        SUP_ROLE AS SP ON SP.ROLE_ID = UA.ROLE
                                            LEFT JOIN
                                        SUP_USER_TYPE AS SU ON SU.USER_TYPE_ID = UA.USER_TYPE
                                    WHERE
                                        UA.ROLE =?ROLE AND UA.IS_DELETED!=1;";
                        break;
                    }
                case AccountSQLCommands.FetchUserAccountByRoleNew:
                    {
                        query = @"SELECT 
                                        USER_ACCOUNT_ID,
                                        USERNAME,
                                        ROLE_NAME AS ROLE,
                                        NAME,
                                        EMAIL,
                                        MOBILE,
                                        USER_ID,
                                        USER_TYPE_NAME AS USER_TYPE,
                                        PHOTO
                                    FROM
                                        USER_ACCOUNT AS UA
                                            INNER JOIN
                                        SUP_ROLE AS SP ON SP.ROLE_ID = UA.ROLE
                                            LEFT JOIN
                                        SUP_USER_TYPE AS SU ON SU.USER_TYPE_ID = UA.USER_TYPE
                                    WHERE
                                        UA.ROLE =?ROLE AND UA.IS_DELETED!=1 limit 100;";
                        break;
                    }
                case AccountSQLCommands.FetchUserAccountByUserNameForUpdate:
                    {
                        query = @"SELECT 
                                        USER_ACCOUNT_ID,
                                        USERNAME,
                                        ROLE_NAME AS ROLE,
                                        NAME,
                                        EMAIL,
                                        MOBILE,
                                        USER_ID,
                                        USER_TYPE_NAME AS USER_TYPE,
                                        PHOTO
                                    FROM
                                        USER_ACCOUNT AS UA
                                            INNER JOIN
                                        SUP_ROLE AS SP ON SP.ROLE_ID = UA.ROLE
                                            LEFT JOIN
                                        SUP_USER_TYPE AS SU ON SU.USER_TYPE_ID = UA.USER_TYPE
                                    WHERE
                                          UA.IS_DELETED!=1 
                                          and UA.ROLE =?ROLE 
                                          and  USERNAME like '?USERNAME%' LIMIT 100;";
                        break;
                    }
                case AccountSQLCommands.UpdateUserAccount:
                    {
                        query = @"UPDATE USER_ACCOUNT
                                  SET
	                                  USERNAME = ?USERNAME,
	                                  PASSWORD = ?PASSWORD,
	                                  ROLE = ?ROLE,
	                                  NAME = ?NAME,
	                                  EMAIL = ?EMAIL,
	                                  MOBILE = ?MOBILE,
	                                  USER_ID = ?USER_ID,
	                                  USER_TYPE = ?USER_TYPE
                                  WHERE USER_ACCOUNT_ID = ?USER_ACCOUNT_ID;";
                        break;
                    }
                case AccountSQLCommands.SaveUserAccount:
                    {
                        query = @"INSERT INTO USER_ACCOUNT
                                        (
                                        USERNAME,
                                        PASSWORD,
                                        ROLE,
                                        NAME,
                                        EMAIL,
                                        MOBILE,
                                        USER_ID,
                                        USER_TYPE)
                                        VALUES
                                        (
                                        ?USERNAME,
                                        ?PASSWORD,
                                        ?ROLE,
                                        ?NAME,
                                        ?EMAIL,
                                        ?MOBILE,
                                        ?USER_ID,
                                        ?USER_TYPE);";
                        break;
                    }
                case AccountSQLCommands.FetchExistingUserAccount:
                    { 
                        query = @"SELECT USER_ACCOUNT_ID FROM USER_ACCOUNT WHERE USER_ID=?USER_ID AND ROLE=?ROLE AND IS_DELETED!=1;";
                        break;
                    }
                case AccountSQLCommands.FetchStaffNameByUserIdAndUserType:
                    {
                        query = @"SELECT 
                                        STAFF_ID AS USER_ID,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                '',
                                                ' ',
                                                IFNULL(LAST_NAME, '')) AS FIRST_NAME
                                    FROM
                                        STF_PERSONAL_INFO
                                    WHERE
                                        USER_TYPE = ?USER_TYPE AND STAFF_ID = ?USER_ID AND IS_DELETED!=1;";
                        break;
                    }
                case AccountSQLCommands.FetchStudentNameByUserIdAndUserType:
                    {
                        query = @"SELECT 
                                        STUDENT_ID AS USER_ID,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                '',
                                                ' ',
                                                IFNULL(LAST_NAME, '')) AS FIRST_NAME
                                    FROM
                                        STU_PERSONAL_INFO
                                    WHERE
                                        USER_TYPE = ?USER_TYPE AND STUDENT_ID = ?USER_ID AND IS_DELETED!=1";
                        break;
                    }
                case AccountSQLCommands.ResetPassword:
                    {
                        query = @"UPDATE USER_ACCOUNT SET PASSWORD=?PASSWORD WHERE USER_ACCOUNT_ID=?USER_ACCOUNT_ID;";
                        break;
                    }
                case AccountSQLCommands.UpdateUserAccountDetails:
                    {
                        query = @"UPDATE USER_ACCOUNT SET 
                                                        PASSWORD=?PASSWORD,
                                                        NAME=?NAME,
                                                        ROLE =?ROLE,
                                                        USER_TYPE=?USER_TYPE
                                                    WHERE 
                                                        USER_ACCOUNT_ID=?USER_ACCOUNT_ID;";
                        break;
                    }
                case AccountSQLCommands.FetchLoginCoverImage:
                    {
                        query = @"SELECT
                                        PHOTO
                                    FROM
                                        CALENDER WHERE DAY_ORDER_DATE=CURDATE() AND IS_DELETED!=1;";
                        break;
                    }
                case AccountSQLCommands.FetchUserCredentialsForgetPassword:
                    {
                        query = @"SELECT 
	                                    U.USER_ACCOUNT_ID,
                                        U.USERNAME,
                                        U.EMAIL,
                                        U.MOBILE,
                                        U.USER_ID
                                    FROM
                                        USER_ACCOUNT AS U
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.ROLL_NO = U.USERNAME
                                            AND SP.STUDENT_ID = U.USER_ID
                                            AND SP.ROLL_NO = ?USERNAME
                                            AND SP.DATE_OF_BIRTH = ?DATE_OF_BIRTH
                                            AND SP.IS_DELETED != 1
                                    WHERE
                                        U.IS_DELETED!=1 
                                    UNION
                                    SELECT 
	                                    U.USER_ACCOUNT_ID,
                                        U.USERNAME,
                                        U.EMAIL,
                                        U.MOBILE,
                                        U.USER_ID
                                    FROM
                                        USER_ACCOUNT AS U
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_CODE = U.USERNAME
                                            AND SP.STAFF_ID = U.USER_ID
                                            AND SP.STAFF_CODE = ?USERNAME
                                            AND SP.DATE_OF_BIRTH = ?DATE_OF_BIRTH
                                            AND SP.IS_DELETED != 1
                                    WHERE
                                        U.IS_DELETED!=1;";
                        break;
                    }
                case AccountSQLCommands.SaveUserRolesByAcYear:
                    {
                        query = @"INSERT INTO USER_ROLES_BY_ACADEMIC_YEAR
                                  (
                                      USER_ID, 
                                      ROLE_ID, 
                                      USER_TYPE, 
                                      ACADEMIC_YEAR, 
                                      IS_ACTIVE
                                  ) VALUES
                                  (
                                      ?USER_ID, 
                                      ?ROLE_ID, 
                                      ?USER_TYPE, 
                                      ?AC_YEAR, 
                                      '1'
                                  );";
                        break;
                    }
                case AccountSQLCommands.FetchUserAccountById:
                    {
                        query = @"SELECT USER_ACCOUNT_ID,USERNAME,ROLE,NAME,USER_ID,USER_TYPE,NAME,EMAIL FROM USER_ACCOUNT WHERE USER_ACCOUNT_ID=?USER_ACCOUNT_ID;";
                        break;
                    }
                case AccountSQLCommands.DeleteUserAccount:
                    {
                        query = @"UPDATE USER_ACCOUNT SET IS_DELETED=1 WHERE USER_ACCOUNT_ID=?USER_ACCOUNT_ID;";
                        break;
                    }
                case AccountSQLCommands.FetchUserAccountByUserid:
                    {
                        query = @"SELECT 
                                      USER_ACCOUNT_ID,
                                      USERNAME,
                                      ROLE_NAME AS ROLE,
                                      NAME,
                                      EMAIL,
                                      MOBILE,
                                      ur.ROLE_ID,
                                      ua.USER_ID,USER_ROLE_ID,
                                      USER_TYPE_NAME AS USER_TYPE,
                                      PHOTO
                                  FROM
                                      USER_ACCOUNT AS UA                                          
											INNER JOIN
                                        USER_ROLES_BY_ACADEMIC_YEAR AS UR ON UR.USER_ID=UA.USER_ID
                                         AND UA.USER_TYPE=UR.USER_TYPE
                                         AND UR.IS_DELETED!=1
                                             INNER JOIN
                                       SUP_ROLE AS SP ON SP.ROLE_ID = UR.ROLE_ID
                                           LEFT JOIN
                                        SUP_USER_TYPE AS SU ON SU.USER_TYPE_ID = UA.USER_TYPE
                                  WHERE
                                    UA.USER_ID =?USER_ID AND UA.ROLE=?ROLE
                                    AND UA.IS_DELETED!=1
                                    AND UR.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case AccountSQLCommands.FetchUserAccountByUserName:
                    {
                        query = @"SELECT 
                                      USER_ACCOUNT_ID,
                                      USERNAME,
                                      ROLE_NAME AS ROLE,
                                      NAME,
                                      EMAIL,
                                      MOBILE,
                                      ur.ROLE_ID,
                                      ua.USER_ID,USER_ROLE_ID,
                                      USER_TYPE_NAME AS USER_TYPE,
                                      PHOTO
                                  FROM
                                      USER_ACCOUNT AS UA                                          
											INNER JOIN
                                        USER_ROLES_BY_ACADEMIC_YEAR AS UR ON UR.USER_ID=UA.USER_ID
                                         AND UA.USER_TYPE=UR.USER_TYPE
                                         AND UR.IS_DELETED!=1
                                             INNER JOIN
                                       SUP_ROLE AS SP ON SP.ROLE_ID = UR.ROLE_ID
                                           LEFT JOIN
                                        SUP_USER_TYPE AS SU ON SU.USER_TYPE_ID = UA.USER_TYPE
                                  WHERE
                                      UA.USERNAME =?USERNAME AND UA.ROLE=?ROLE
                                  AND UA.IS_DELETED!=1
                                  AND UR.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case AccountSQLCommands.FetchExistingUserRoles:
                    {
                        query = @"SELECT 
                                      USER_ROLE_ID,
                                      USER_ID,
                                      ROLE_ID,
                                      USER_TYPE,
                                      ACADEMIC_YEAR,
                                      IS_ACTIVE,
                                      IS_DELETED
                                  FROM
                                      user_roles_by_academic_year
                                  WHERE
                                      USER_ID =?USER_ID AND ROLE_ID =?ROLE_ID
                                  AND USER_TYPE = ?USER_TYPE
                                  AND ACADEMIC_YEAR = ?AC_YEAR
                                  AND IS_DELETED != 1;";
                        break;
                    }
                case AccountSQLCommands.FetchExistingUserRolesbyUsername:
                    {
                        query = @"SELECT 
                                    SR.USER_ROLE_ID,
                                    SR.USER_ID,
                                    SR.ROLE_ID,
                                    SR.USER_TYPE,
                                    SR.ACADEMIC_YEAR,
                                    SR.IS_ACTIVE,
                                    SR.IS_DELETED
                                FROM
                                    user_roles_by_academic_year AS sr
                                        INNER JOIN
                                    user_account AS ua ON ua.USER_ID = sr.user_id
                                WHERE
                                    sr.ROLE_ID = ?ROLE_ID
                                        AND UA.USERNAME = ?USERNAME
                                        AND SR.USER_TYPE = ?USER_TYPE
                                        AND SR.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SR.IS_DELETED != 1;";
                        break;
                    }
                case AccountSQLCommands.FetchUserIdbyUserName:
                    {
                        query = @"SELECT 
                                        USER_ID
                                    FROM
                                        user_account
                                    WHERE
                                        USERNAME = ?USERNAME AND IS_DELETED !=1";
                        break;
                    }
                case AccountSQLCommands.DeleteUserRoles:
                    {
                        query = @"UPDATE USER_ROLES_BY_ACADEMIC_YEAR SET IS_DELETED=?IS_DELETED WHERE USER_ROLE_ID=?USER_ROLE_ID;";
                        break;
                    }
                case AccountSQLCommands.FetchUserRolesById:
                    {
                        query = @"SELECT 
                                      USER_ROLE_ID,
                                      UR.USER_ID,UA.USERNAME,
                                      UR.ROLE_ID,
                                      UR.USER_TYPE,
                                      AY.ACADEMIC_YEAR_ID  AS ACADEMIC_YEAR,
                                      UR.IS_ACTIVE,
                                      UR.IS_DELETED
                                  FROM
                                      USER_ROLES_BY_ACADEMIC_YEAR AS UR
                                      INNER JOIN
                                      USER_ACCOUNT AS UA ON UA.USER_ID=UR.USER_ID 
                                      AND UA.USER_TYPE=UR.USER_TYPE
                                      LEFT JOIN
                                      ACADEMIC_YEAR AS AY ON AY.AC_YEAR=UR.ACADEMIC_YEAR
                                  WHERE
                                      USER_ROLE_ID =?USER_ROLE_ID;";
                        break;
                    }
                case AccountSQLCommands.UpdateUserRoles:
                    {
                        query = @"UPDATE 
                                   USER_ROLES_BY_ACADEMIC_YEAR 
                                  SET  
                                    ROLE_ID=?ROLE_ID, 
                                    USER_TYPE=?USER_TYPE, 
                                    ACADEMIC_YEAR=?AC_YEAR
                                  WHERE 
                                    USER_ROLE_ID=?USER_ROLE_ID;";
                        break;
                    }
                case AccountSQLCommands.FetchUsersfromuseraccount:
                    {
                        query = @"SELECT 
                                     UA.USER_ID AS STUDENT_ID,
                                     CONCAT(IFNULL(NAME, ''),
                                             ' ',
                                             '(',
                                             USERNAME,
                                             ')') AS FIRST_NAME
                                 FROM
                                     USER_ACCOUNT AS UA
                                 WHERE
                                      UA.IS_DELETED != 1; ";
                        break;
                    }
                case AccountSQLCommands.FetchUsersByRole:
                    {
                        query = @"SELECT 
                                     UA.USER_ID AS STUDENT_ID,
                                     CONCAT(IFNULL(NAME, ''),
                                             ' ',
                                             '(',
                                             USERNAME,
                                             ')') AS FIRST_NAME
                                 FROM
                                     USER_ACCOUNT AS UA
                                 WHERE
                                     UA.ROLE =?ROLE AND UA.IS_DELETED != 1";
                        break;
                    }
                case AccountSQLCommands.CheckUsernameExistOrNot:
                    {
                        query = @"SELECT 
                                        USERNAME,
                                        PASSWORD,
                                        ROLE,
                                        USER_ACCOUNT_ID
                                    FROM
                                        USER_ACCOUNT
                                    WHERE
                                        USERNAME = ?USERNAME
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case AccountSQLCommands.CheckValidUsername:
                    {
                        query = @"SELECT 
                                        ROLL_NO USERNAME, 
                                        STUDENT_ID AS USER_ID
                                    FROM
                                        STU_PERSONAL_INFO
                                    WHERE
                                        ROLL_NO = ?USERNAME AND IS_DELETED != 1
                                            AND IS_LEFT != 1 
                                    UNION SELECT 
                                        STAFF_CODE AS USERNAME,
                                        STAFF_ID AS USER_ID
                                    FROM
                                        STF_PERSONAL_INFO
                                    WHERE
                                        STAFF_CODE = ?USERNAME
                                            AND IS_DELETED != 1";
                        break;
                    }

                case AccountSQLCommands.SaveUserLoginDetection:
                    {
                        query = @"CREATE TABLE IF NOT EXISTS  `user_login_log_?AC_YEAR` (
                                      `ID` INT(11) NOT NULL AUTO_INCREMENT,
                                      `IP_ADDRESS` varchar(50) NOT NULL,
                                      `USERNAME` varchar(100) NOT NULL,
                                      `STATUS` varchar(20) NOT NULL,
                                      `USER_AGENT` text DEFAULT NULL,
                                      `LATITUDE` varchar(50) DEFAULT NULL,
                                      `LONGITUDE` varchar(50) DEFAULT NULL,
                                      `PLACE` varchar(555) DEFAULT NULL,
                                      `LOGTIME` datetime NOT NULL,
                                      `IS_DELETED` INT(11) DEFAULT '0',
                                      PRIMARY KEY (`ID`)
                                    );

                        INSERT INTO USER_LOGIN_LOG_?AC_YEAR (USERNAME,
                                                                STATUS,
                                                                IP_ADDRESS,
                                                                LOGTIME,
                                                                LONGITUDE,
                                                                LATITUDE,
                                                                PLACE,
                                                                USER_AGENT)
                                                            values(
                                                                ?USERNAME,
                                                                ?STATUS,
                                                                ?IP_ADDRESS,
                                                                ?LOGTIME,
                                                                ?LONGITUDE,
                                                                ?LATITUDE,
                                                                ?PLACE,
                                                                ?USER_AGENT);";
                        break;
                    }

                case AccountSQLCommands.SaveUserChangePasswordLog:
                    {
                        query = @"INSERT INTO USER_CHANGE_PASSWORD (
                                        USERNAME,
                                        PASSWORD,
                                        ROLE,
                                        LAST_LOGIN,
                                        IS_DELETED,
                                        IS_ACTIVE,
                                        USER_ID,
                                        USER_TYPE,
                                        DATE,
                                        TIME)
                                    VALUES (
                                        ?USERNAME,
                                        ?PASSWORD,
                                        ?ROLE,
                                        ?LAST_LOGIN,
                                        '0',
                                        '1',
                                        ?USER_ID,
                                        ?USER_TYPE,
                                        ?DATE,
                                        ?TIME);";
                        break;
                    }
                case AccountSQLCommands.FetchUserRolesByAcademicYear:
                    {
                        query = @"SELECT 
                                      USER_ID, 
                                      ROLE_ID, 
                                      USER_TYPE, 
                                      ACADEMIC_YEAR, 
                                      IS_ACTIVE,
                                      IS_DELETED
                                  FROM 
                                      USER_ROLES_BY_ACADEMIC_YEAR 
                                  WHERE 
                                      ACADEMIC_YEAR=?ACADEMIC_YEAR AND IS_DELETED!=1;";
                        break;
                    }

            }
               

            return query;
        }

    }
}
