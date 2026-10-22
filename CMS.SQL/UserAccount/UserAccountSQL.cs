using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.UserAccount
{
    public static class UserAccountSQL
    {
        public static string GetSupportDataSQL(UserAccountSQLCommands sEnumCommand)
        {
            string query = "";

            switch (sEnumCommand)
            {
                case UserAccountSQLCommands.FetchUserAccountByName:
                    {
                        query = @"SELECT 
                                    USER_ACCOUNT_ID,
                                    PASSWORD,
                                    USERNAME,
                                    ROLE,
                                    USER_ID,
                                    USER_TYPE,
                                    EMAIL
                                FROM
                                    USER_ACCOUNT
                                WHERE
                                    USERNAME = ?USERNAME AND IS_DELETED!=1;";
                        break;
                    }
                case UserAccountSQLCommands.FetchSupRole:
                    {
                        query = @"SELECT 
                                    ROLE_ID,
                                    ROLE_NAME
                                FROM
                                    SUP_ROLE;";
                        break;
                    }
                case UserAccountSQLCommands.FetchUserType:
                    {
                        query = @"SELECT                                     
                                    USER_TYPE_ID,
                                    USER_TYPE_NAME
                                FROM
                                    SUP_USER_TYPE;";
                        break;
                    }
                case UserAccountSQLCommands.DeleteUserAccount:
                    {
                        query = @"UPDATE USER_ACCOUNT SET IS_DELETED=1 WHERE USER_ACCOUNT_ID=?USER_ACCOUNT_ID;";
                        break;
                    }
                case UserAccountSQLCommands.FetchUserAccountByRoleAndId:
                    {
                        query = @"SELECT 
                                    USER_ACCOUNT_ID,
                                    USERNAME,
                                    PASSWORD,
                                    NAME,
                                    EMAIL,
                                    MOBILE,
                                    USER_ID,ROLE,USER_TYPE
                                    FROM
                                       USER_ACCOUNT WHERE USERNAME=?USERNAME AND USER_TYPE=?USER_TYPE;";
                        break;
                    }
               
                case UserAccountSQLCommands.SaveUserAccount:
                    {
                        query = @"INSERT INTO USER_ACCOUNT(
                                    USERNAME,
                                    PASSWORD,
                                    NAME,
                                    EMAIL,
                                    MOBILE,
                                    USER_ID,ROLE,USER_TYPE)
                                VALUES 
	                                (?USERNAME,?PASSWORD,?NAME,?EMAIL,?MOBILE,?USER_ID,?ROLE,?USER_TYPE);";
                        break;
                    }
                case UserAccountSQLCommands.UpdateUserAccount:
                    {
                        query = @"UPDATE USER_ACCOUNT SET  
                                USERNAME=?USERNAME,
                                PASSWORD=?PASSWORD,
                                NAME=?NAME,
                                EMAIL=?EMAIL,
                                MOBILE=?MOBILE,
                                USER_ID=?USER_ID,
                                PHOTO=?PHOTO,
                                ROLE=?ROLE,
                                USER_TYPE=?USER_TYPE
	                            WHERE USER_ACCOUNT_ID=?USER_ACCOUNT_ID";
                        break;
                    }
                case UserAccountSQLCommands.UpdateUserAccountWithoutPassword:
                    {
                        query = @"UPDATE USER_ACCOUNT SET  
                                USERNAME=?USERNAME,
                                NAME=?NAME,
                                EMAIL=?EMAIL,
                                MOBILE=?MOBILE,
                                USER_ID=?USER_ID,
                                PHOTO=?PHOTO,
                                ROLE=?ROLE,
                                USER_TYPE=?USER_TYPE
	                            WHERE USER_ACCOUNT_ID=?USER_ACCOUNT_ID";
                        break;
                    }
                case UserAccountSQLCommands.ValidateUserRole:
                    {
                        query = @"SELECT 
                                        COUNT(*) > 0 AS ROLE_STATUS
                                    FROM
                                        MENU_ITEMS AS MI
                                            INNER JOIN
                                        USER_ROLES_RIGHTS AS UR ON MI.MENU_ID = UR.MENU_ITEM_ID
                                            INNER JOIN
                                        SUP_ROLE AS SP ON SP.ROLE_ID = UR.ROLE_ID
		                                    INNER JOIN
	                                    USER_ROLES_BY_ACADEMIC_YEAR AS UA ON UA.ROLE_ID=UR.ROLE_ID
		                                    AND UA.USER_ID=?USER_ID
                                            AND UA.ACADEMIC_YEAR=?AC_YEAR
                                            AND UA.IS_DELETED!=1
                                    WHERE
                                         MI.CONTROLLER = ?CONTROLLER
	                                     AND MI.ACTION = ?ACTION
	                                     AND UR.IS_ACTIVE = 1;";
                        break;
                    }
                case UserAccountSQLCommands.FetchUserCredentials:
                    {
                        query = @"SELECT 
                                        UA.USER_ACCOUNT_ID,
                                        UA.USERNAME,
                                        UA.MOBILE,
                                        SR.ROLE_NAME,
                                        UA.ROLE,
                                        SU.USER_TYPE_NAME,
                                        SU.USER_TYPE_ID,
                                        UA.USER_ID,
                                        UA.NAME,
                                        UA.PHOTO,CONTROLLER_NAME,ACTION_NAME,IS_OTP
                                    FROM
                                        USER_ACCOUNT AS UA
                                            LEFT JOIN
                                        SUP_ROLE AS SR ON SR.ROLE_ID = UA.ROLE
                                            LEFT JOIN
                                        SUP_USER_TYPE AS SU ON SU.USER_TYPE_ID = UA.USER_TYPE
                                    WHERE
                                        UA.USERNAME = ?USERNAME
                                            AND UA.PASSWORD = ?PASSWORD AND UA.IS_DELETED!=1;";
                        break;

                    }
                case UserAccountSQLCommands.FetchMenuItemsByRole:
                    {
                        query = @"SELECT 
                                    M.MODULE_NAME,
                                    MI.ACTION,
                                    MI.CONTROLLER,
                                    MI.MENU_NAME,
                                    M.STYLE,
                                    M.HAS_SUB
                                FROM
                                    USER_ROLES_RIGHTS AS RR
                                        INNER JOIN
                                    MENU_ITEMS AS MI ON MI.MENU_ID = RR.MENU_ITEM_ID
                                        AND MI.IS_DELETED!=1
                                        INNER JOIN
                                    MODULES AS M ON M.MODULES_ID = MI.PARENT_ID
										INNER JOIN
									USER_ROLES_BY_ACADEMIC_YEAR AS UA ON UA.ROLE_ID=RR.ROLE_ID
										AND UA.USER_ID=?USER_ID
                                        AND UA.USER_TYPE=?USER_TYPE
										AND UA.ACADEMIC_YEAR=?ACADEMIC_YEAR
                                        AND UA.IS_DELETED!=1
                                WHERE
                                     RR.IS_ACTIVE = 1   group by RR.MENU_ITEM_ID
                                ORDER BY M.MODULES_ORDER , MI.MENU_NAME;";
                        break;
                    }
                case UserAccountSQLCommands.FetchCollegeInfo:
                    {
                        query = @"SELECT 
                                    COLLEGE_ID,
                                    MANAGEMENT_NAME,
                                    COLLEGE_NAME,
                                    PLACE,
                                    EMAIL,
                                    COLLEGE_CODE,
                                    COLLEGE_LOGO
                                FROM
                                    COLLEGE_DETAILS;";

                        break;
                    }
                case UserAccountSQLCommands.FetchActiveYear:
                    {
                        query = @"SELECT 
                                        AC_YEAR,
                                        ACADEMIC_NAME
                                    FROM
                                        ACADEMIC_YEAR
                                    WHERE
                                        ACTIVE_YEAR=1 AND IS_DELETED !=1
                                    ORDER BY ACADEMIC_YEAR_ID;";
                        break;
                    }
                case UserAccountSQLCommands.UpdateUserAccountImage:
                    {
                        query = @"UPDATE USER_ACCOUNT SET PHOTO=?PHOTO WHERE USER_ID=?USER_ID AND ROLE=?ROLE;";
                        break;
                    }
                case UserAccountSQLCommands.SaveUserRoleByAcademicYear:
                    {
                        query = @"INSERT INTO USER_ROLES_BY_ACADEMIC_YEAR(
								 USER_ID,
								 ROLE_ID,
								 USER_TYPE, 
								 ACADEMIC_YEAR,
                                 IS_ACTIVE)
							VALUES(
                                 ?USER_ID,
								 ?ROLE_ID,
								 ?USER_TYPE, 
								 ?ACADEMIC_YEAR,
                                 ?IS_ACTIVE
                                 );";
                        break;
                    }
                case UserAccountSQLCommands.FetchUserAccountuserUIdAndRole:
                    {
                        query = @"SELECT 
                                    USER_ACCOUNT_ID,
                                    PASSWORD,
                                    USERNAME,
                                    ROLE,
                                    USER_ID,
                                    USER_TYPE,
                                    EMAIL
                                FROM
                                    USER_ACCOUNT
                                WHERE
                                    USER_ID = ?USER_ID AND ROLE=5 AND IS_DELETED!=1;";
                        break;
                    }
                case UserAccountSQLCommands.FetchUserAccountByUserIdandName:
                    {
                        query = @"SELECT 
                                    USER_ACCOUNT_ID,
                                    PASSWORD,
                                    USERNAME,
                                    ROLE,
                                    USER_ID,
                                    USER_TYPE,
                                    EMAIL
                                FROM
                                    USER_ACCOUNT
                                WHERE
                                    USER_ID = ?USER_ID AND USERNAME=?USERNAME AND IS_DELETED!=1;";
                        break;
                    }
                case UserAccountSQLCommands.FetchsupCategory:
                    {
                        query = @"SELECT 
                                    STF_CATEGORY_ID,
                                    STF_CATEGORY
                                FROM
                                    SUP_STAFF_CATEGORY
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case UserAccountSQLCommands.FetchUserAccountByRoleAndUserId:
                    {
                        query = @"SELECT 
                                    USER_ACCOUNT_ID,
                                    USERNAME,
                                    PASSWORD,
                                    NAME,
                                    EMAIL,
                                    MOBILE,
                                    USER_ID,ROLE,USER_TYPE
                                    FROM
                                       USER_ACCOUNT WHERE USER_ID=?USER_ID AND USER_TYPE=?USER_TYPE;";
                        break;
                    }
                case UserAccountSQLCommands.FetchUserRolebyAcYearRoleAndUserIdAndAcYear:
                    {
                        query = @"SELECT 
                                        USER_ID
                                    FROM
                                        USER_ROLES_BY_ACADEMIC_YEAR
                                    WHERE
                                        USER_ID = ?USER_ID AND IS_DELETED !=1 and IS_ACTIVE=1
                                        AND ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND ROLE_ID = ?ROLE_ID;";
                        break;
                    }

            }
            return query;
        }
    }
}
