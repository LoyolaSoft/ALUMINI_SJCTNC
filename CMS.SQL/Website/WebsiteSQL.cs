using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.Website
{
    public static class WebsiteSQL
    {
        public static string GetWebsiteSQLSQL(WebsiteSQLCommands sEnumCommand)
        {
            string query = "";

            switch (sEnumCommand)
            {
                case WebsiteSQLCommands.FetchWebHomeEvents:
                    {
                        query = @"SELECT 
                                    EVENT_ID,
                                    EVENT_NAME,
                                    DEPARTMENT,
                                    EVENT_TITLE,
                                    DATE,
                                    VENUE,
                                    TIME,
                                    FILE_PATH,
                                    IMG_PATH,
                                    DESCRIPTION,
                                    APPROVER1,
                                    APPROVER2,
                                    APPROVER3,
                                    ENTRY_ID,
                                    ACADEMIC_YEAR,
                                    SEMESTER_TYPE,
                                    IS_ACTIVE
                                FROM
                                    web_home_events
                                WHERE
                                    IS_DELETED != 1 and ENTRY_ID=?ENTRY_ID;";
                        break;
                    }
                case WebsiteSQLCommands.FetchsupAchievementList:
                    {
                        query = @"SELECT 
                                        ACHIVEMENT_ID as ACHIEVEMENT_ID, ACHIVEMENT_NAME as ACHIEVEMENT_NAME
                                    FROM
                                        sjctnc_website_live.sup_achievement
                                    WHERE
                                        IS_DELETED != 1 AND IS_ACTIVE = 1;";
                        break;
                    }
                case WebsiteSQLCommands.FetchsupDepartmentList:
                    {
                        query = @"SELECT 
                                        DEPARTMENT_ID, DEPARTMENT_NAME as DEPARTMENT
                                    FROM
                                        sjctnc_website_live.sup_department
                                    WHERE
                                        IS_DELETED != 1";
                        break;
                    }
                case WebsiteSQLCommands.SaveWebAchievement:
                    {
                        query = @"insert into web_home_achievement(
                                                            ACHIEVEMNT_CATEGORY,
                                                            DEPARTMENT,
                                                            DATE,
                                                            FILE_PATH,
                                                            IMG_PATH,
                                                            DESCRIPTION,
                                                            ENTRY_ID,
                                                            SDG_ID,   
                                                            LINKTEXT,   
                                                            LINK,   
                                                            STAFF_ID,   
                                                            ACADEMIC_YEAR,   
                                                            SEMESTER_TYPE,   
                                                            TITLE)
                                                        VALUES(
                                                            ?ACHIEVEMNT_CATEGORY,
                                                            ?DEPARTMENT,
                                                            ?DATE,
                                                            ?FILE_PATH,
                                                            ?IMG_PATH,
                                                            ?DESCRIPTION,
                                                            ?ENTRY_ID,
                                                            ?SDG_ID,
                                                            ?LINKTEXT,
                                                            ?LINK,
                                                            ?STAFF_ID,
                                                            ?ACADEMIC_YEAR,
                                                            ?SEMESTER_TYPE,
                                                            ?TITLE);";
                        break;
                    }
                case WebsiteSQLCommands.FetchWebHomeAchievement:
                    {

                        query = @"SELECT 
                                    ACHIEVEMENT_ID,
                                    ACHIEVEMNT_CATEGORY,
                                    DATE_FORMAT(DATE, '%d/%m/%Y') AS DATE,
                                    IMG_PATH,
                                    FILE_PATH,
                                    DESCRIPTION,
                                    APPROVE_LEVEL1,
                                    APPROVER_LEVEL2,
                                    APPROVER_LEVEL3,
                                    ENTRY_ID,
                                    LINK,
                                    LINKTEXT,
                                    TITLE,
                                    SDG_ID,
                                    ENTRY_DATE,
                                    sa.ACHIVEMENT_NAME AS ACHIEVEMENT_NAME,
                                     sd.DEPARTMENT_NAME AS DEPARTMENT
                                FROM
                                    cms_sjctnc.web_home_achievement AS wha
                                        INNER JOIN
                                    sjctnc_website_live.sup_achievement AS sa ON sa.ACHIVEMENT_ID = wha.ACHIEVEMNT_CATEGORY
                                        INNER JOIN
                                    sjctnc_website_live.sup_department AS sd ON sd.DEPARTMENT_ID = wha.DEPARTMENT
                                WHERE
                                    WHA.IS_DELETED != 1 and ENTRY_ID=?ENTRY_ID order by wha.ACHIEVEMENT_ID desc;";
                        break;
                    }
                case WebsiteSQLCommands.DeleteWebAchievement:
                    {
                        query = @" UPDATE cms_sjctnc.web_home_achievement 
                                        SET 
                                            IS_DELETED = ?IS_DELETED
                                        WHERE
                                            ACHIEVEMENT_ID = ?ACHIEVEMENT_ID";
                        break;
                    }
                case WebsiteSQLCommands.FetchHomeAchievementbyId:
                    {
                        query = @"SELECT 
                                        ACHIEVEMENT_ID,
                                        ACHIEVEMNT_CATEGORY,
                                        wha.DEPARTMENT as DEPARTMENT_ID,
                                        DATE_FORMAT(DATE, '%d/%m/%Y') AS DATE,
                                        IMG_PATH,
                                        FILE_PATH,
                                        DESCRIPTION,
                                        APPROVE_LEVEL1,
                                        APPROVER_LEVEL2,
                                        APPROVER_LEVEL3,
                                        ENTRY_ID,
                                        TITLE,
                                        SDG_ID,
                                        LINK,
                                        STAFF_ID,
                                        LINKTEXT,
                                        ENTRY_DATE,
                                        sa.ACHIVEMENT_NAME AS ACHIEVEMENT_NAME,
                                         sd.DEPARTMENT_NAME AS DEPARTMENT
                                    FROM
                                        cms_sjctnc.web_home_achievement AS wha
                                            INNER JOIN
                                        sjctnc_website_live.sup_achievement AS sa ON sa.ACHIVEMENT_ID = wha.ACHIEVEMNT_CATEGORY
                                            INNER JOIN
                                        sjctnc_website_live.sup_department AS sd ON sd.DEPARTMENT_ID = wha.DEPARTMENT
                                    WHERE
                                        WHA.IS_DELETED != 1 and ACHIEVEMENT_ID=?ACHIEVEMENT_ID ; ";
                        break;
                    }
                case WebsiteSQLCommands.UpdateWebHomeAchievement:
                    {
                        query = @"UPDATE cms_sjctnc.web_home_achievement 
                                            SET 
                                                ACHIEVEMNT_CATEGORY = ?ACHIEVEMNT_CATEGORY,
                                                DEPARTMENT = ?DEPARTMENT,
                                                DATE = ?DATE,
                                                IMG_PATH = ?IMG_PATH,
                                                DESCRIPTION = ?DESCRIPTION,
                                                SDG_ID = ?SDG_ID,
                                                LINKTEXT = ?LINKTEXT,
                                                LINK = ?LINK,
                                                TITLE = ?TITLE
                                            WHERE
                                                ACHIEVEMENT_ID = ?ACHIEVEMENT_ID;";
                        break;
                    }
                case WebsiteSQLCommands.UpdateWebHomeAchievementWithoutImage:
                    {
                        query = @"UPDATE cms_sjctnc.web_home_achievement 
                                            SET 
                                                ACHIEVEMNT_CATEGORY = ?ACHIEVEMNT_CATEGORY,
                                                DEPARTMENT = ?DEPARTMENT,
                                                DATE = ?DATE,
                                                DESCRIPTION = ?DESCRIPTION,
                                                SDG_ID = ?SDG_ID,
                                                LINKTEXT = ?LINKTEXT,
                                                LINK = ?LINK,
                                                STAFF_ID = ?STAFF_ID,
                                                TITLE = ?TITLE
                                            WHERE
                                                ACHIEVEMENT_ID = ?ACHIEVEMENT_ID;";
                        break;
                    }
                case WebsiteSQLCommands.FetchWebAchievementforUpdateApprovalStatus:
                    {
                        query = @"SELECT 
                                    ACHIEVEMENT_ID,
                                    ACHIEVEMNT_CATEGORY,
                                    DATE_FORMAT(DATE, '%d/%m/%Y') AS DATE,
                                    IMG_PATH,
                                    FILE_PATH,
                                    DESCRIPTION,
                                    TITLE,
                                    APPROVE_LEVEL1,
                                    APPROVER_LEVEL2,
                                    APPROVER_LEVEL3,
                                    ENTRY_ID,
                                    ENTRY_DATE,
                                    sa.ACHIVEMENT_NAME AS ACHIEVEMENT_NAME,
                                    sd.DEPARTMENT_NAME AS DEPARTMENT,
                                    CONCAT(spi.FIRST_NAME,
                                            ' (',
                                            spi.STAFF_CODE,
                                            ')') AS FIRST_NAME
                                FROM
                                    cms_sjctnc.web_home_achievement AS wha
                                        INNER JOIN
                                    sjctnc_website_live.sup_achievement AS sa ON sa.ACHIVEMENT_ID = wha.ACHIEVEMNT_CATEGORY
                                        INNER JOIN
                                    sjctnc_website_live.sup_department AS sd ON sd.DEPARTMENT_ID = wha.DEPARTMENT
                                        INNER JOIN
                                    stf_personal_info AS spi ON spi.STAFF_ID = wha.ENTRY_ID
                                WHERE
                                    WHA.IS_DELETED != 1 ORDER BY ACHIEVEMENT_ID desc;";
                        break;
                    }
                    case WebsiteSQLCommands.UpdateWebAchievementApproveStatus2:
                    {
                        query = @"update web_home_achievement set APPROVER_LEVEL2=?APPROVER_LEVEL2,APPROVERID2=?APPROVERID2 where ACHIEVEMENT_ID=?ACHIEVEMENT_ID";
                        break;
                    } 
                case WebsiteSQLCommands.UpdateWebAchievementApproveStatus1:
                    {
                        query = @"update web_home_achievement set APPROVE_LEVEL1=?APPROVE_LEVEL1,APPROVERID1=?APPROVERID1 where ACHIEVEMENT_ID=?ACHIEVEMENT_ID";
                        break;
                    } 
                case WebsiteSQLCommands.FetchApprovedachievementforSlevelApproval:
                    {
                        query = @"SELECT 
                                    ACHIEVEMENT_ID,
                                    ACHIEVEMNT_CATEGORY,
                                    DATE_FORMAT(DATE, '%d/%m/%Y') AS DATE,
                                    IMG_PATH,
                                    DESCRIPTION,
                                    TITLE,
                                    APPROVE_LEVEL1,
                                    APPROVER_LEVEL2,
                                    APPROVER_LEVEL3,
                                    ENTRY_ID,
                                    ENTRY_DATE,
                                    sa.ACHIVEMENT_NAME AS ACHIEVEMENT_NAME,
                                    sd.DEPARTMENT_NAME AS DEPARTMENT,
                                    CONCAT(spi.FIRST_NAME,
                                            ' (',
                                            spi.STAFF_CODE,
                                            ')') AS FIRST_NAME
                                FROM
                                    CMS_SJCTNC.WEB_HOME_ACHIEVEMENT AS WHA
                                        INNER JOIN
                                    SJCTNC_WEBSITE_LIVE.SUP_ACHIEVEMENT AS SA ON SA.ACHIVEMENT_ID = WHA.ACHIEVEMNT_CATEGORY
                                        INNER JOIN
                                    SJCTNC_WEBSITE_LIVE.SUP_DEPARTMENT AS SD ON SD.DEPARTMENT_ID = WHA.DEPARTMENT
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = WHA.ENTRY_ID
                                WHERE
                                    WHA.IS_DELETED != 1 AND APPROVE_LEVEL1=1 ORDER BY ACHIEVEMENT_ID desc;";
                        break;
                    }
                case WebsiteSQLCommands.SavehomeAchievementinWebsiteDB:
                    {
                        query = @"INSERT INTO sjctnc_website_live.home_achivement (
                                                                                    ACHIEVEMENT_DATE ,
                                                                                    TITLE,
                                                                                    IMAGE_PATH,
                                                                                    ABOUT,
                                                                                    ROLE_ID,
                                                                                    DEPT_ID,
                                                                                    IS_ACTIVE,
                                                                                    LINK,
                                                                                    LINKTEXT,
                                                                                    ENTRY_ID,
                                                                                    STAFF_ID,
                                                                                    FILE_PATH,
                                                                                    SDG_ID)
                                                                                 VALUES  (
                                                                                    ?DATE,
                                                                                    ?TITLE,
                                                                                    ?IMG_PATH,
                                                                                    ?DESCRIPTION,
                                                                                    ?ACHIEVEMNT_CATEGORY,
                                                                                    ?DEPARTMENT,
                                                                                    1,
                                                                                    ?LINK,
                                                                                    ?LINKTEXT,
                                                                                    ?ENTRY_ID,
                                                                                    ?STAFF_ID,
                                                                                    ?FILE_PATH,
                                                                                    ?SDG_ID)";
                        break;
                    }
                case WebsiteSQLCommands.FetchHomeAchievementbyIDForInsertWebsite:
                    {
                        query = @"SELECT 
                                        ACHIEVEMENT_ID,
                                        ACHIEVEMNT_CATEGORY,
                                        DEPARTMENT,
                                        DATE_FORMAT(DATE,'%Y-%m-%d') AS DATE,
                                        IMG_PATH,
                                        FILE_PATH,
                                        DESCRIPTION,
                                        IS_ACTIVE,
                                        TITLE,
                                        LINK,
                                        ENTRY_ID,
                                        STAFF_ID,
                                        LINKTEXT,
                                        SDG_ID
                                    FROM
                                        web_home_achievement
                                    WHERE
                                        IS_DELETED != 1 AND ACHIEVEMENT_ID = ?ACHIEVEMENT_ID and APPROVE_LEVEL1=1 and APPROVER_LEVEL2=1;";
                        break;
                    } 
                case WebsiteSQLCommands.FetchSDGList:
                    {
                        query = @"SELECT 
                                    SDG_ID, SDG_NAME_LIST
                                FROM
                                    sjctnc_website_live.sup_sdg_list
                                WHERE
                                    IS_DELETED != 1";
                        break;
                    }
                case WebsiteSQLCommands.FetchStaffList:
                    {
                        query = @"SELECT 
                                        STAFF_ID,
                                        CONCAT(first_name, ' ( ', staff_code, ' )') AS FIRST_NAME
                                    FROM
                                        stf_personal_info AS spi
                                    WHERE
                                        spi.IS_DELETED != 1;";
                        break;
                    }
                case WebsiteSQLCommands.UpdateAchievementFilePathByID:
                    {
                        query = @"update web_home_achievement set FILE_PATH=?FILE_PATH WHERE ACHIEVEMENT_ID=?ACHIEVEMENT_ID";
                        break;
                    }
                case WebsiteSQLCommands.UpdateAchievementImgPathbyID:
                    {
                        query = @"update web_home_achievement set IMG_PATH=?IMG_PATH WHERE ACHIEVEMENT_ID=?ACHIEVEMENT_ID";
                        break;
                    }
                case WebsiteSQLCommands.FetchMenuClicksReport:
                    {
                        query = @"SELECT 
                                    mc.CLICK_DATE,
                                    mm.MAIN_MENU_NAME AS MAIN_MENU,
                                    sm.SUB_MENU_NAME AS SUB_MENU,
                                    im.INSIDE_MENU_NAME AS INSIDE_MENU,
                                    SUM(mc.CLICK_COUNT) AS CLICK_COUNT
                                FROM 
                                    sjctnc_website_live.mn_menu_clicks as mc
                                LEFT JOIN sjctnc_website_live.mn_main_menus as mm ON mc.MAIN_MENU_ID = mm.MAIN_MENU_ID
                                LEFT JOIN sjctnc_website_live.mn_sub_menus as sm ON mc.SUB_MENU_ID = sm.SUB_MENU_ID
                                LEFT JOIN sjctnc_website_live.mn_inside_menus as im ON mc.INSIDE_MENU_ID = im.INSIDE_MENU_ID
                                WHERE mc.CLICK_DATE BETWEEN ?FROM_DATE AND ?TO_DATE
                                GROUP BY mc.CLICK_DATE, mm.MAIN_MENU_NAME, sm.SUB_MENU_NAME, im.INSIDE_MENU_NAME
                                ORDER BY mc.CLICK_DATE DESC;";
                        break;
                    }
                case WebsiteSQLCommands.FetchMenuWiseTotalClicksReport:
                    {
                        query = @"SELECT 
                                    '' AS CLICK_DATE,
                                    mm.MAIN_MENU_NAME AS MAIN_MENU,
                                    sm.SUB_MENU_NAME AS SUB_MENU,
                                    im.INSIDE_MENU_NAME AS INSIDE_MENU,
                                    SUM(mc.CLICK_COUNT) AS CLICK_COUNT
                                FROM 
                                    sjctnc_website_live.mn_menu_clicks as mc
                                LEFT JOIN sjctnc_website_live.mn_main_menus as mm ON mc.MAIN_MENU_ID = mm.MAIN_MENU_ID
                                LEFT JOIN sjctnc_website_live.mn_sub_menus as sm ON mc.SUB_MENU_ID = sm.SUB_MENU_ID
                                LEFT JOIN sjctnc_website_live.mn_inside_menus as im ON mc.INSIDE_MENU_ID = im.INSIDE_MENU_ID
                                GROUP BY mm.MAIN_MENU_NAME, sm.SUB_MENU_NAME, im.INSIDE_MENU_NAME
                                ORDER BY SUM(mc.CLICK_COUNT) DESC;";
                        break;
                    }

                #region Staff Role Responsibilities

                case WebsiteSQLCommands.FetchResponsibilities:
                    {
                        query = @"SELECT 
                                      STF_ROLE_RES_ID,
                                      ROLES__RESPONSIBILITIES,
                                      IS_ACTIVE,
                                      IS_DELETED,
                                      ORDER_BY
                                  FROM
                                      sup_staff_roles_responsibile
                                  WHERE
                                     IS_DELETED !=1;";
                        break;
                    }

                case WebsiteSQLCommands.UpdateStaffRoleResponsibilities:
                    {
                        query = @"UPDATE 
                                    sup_staff_roles_responsibile
                                    SET 
                                    ROLES__RESPONSIBILITIES=?ROLES__RESPONSIBILITIES,
                                    IS_ACTIVE=?IS_ACTIVE,
                                    ORDER_BY=?ORDER_BY
                                   

                                    WHERE STF_ROLE_RES_ID=?STF_ROLE_RES_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }

                case WebsiteSQLCommands.AddStaffRoleResponsibilities:
                    {
                        query = @"INSERT INTO sup_staff_roles_responsibile
                                    (
                                     ROLES__RESPONSIBILITIES,
                                     IS_ACTIVE,
                                     ORDER_BY)
                                    VALUES
                                    (
                                    ?ROLES__RESPONSIBILITIES,
                                    ?IS_ACTIVE,
                                    ?ORDER_BY );";

                        break;
                    }

                case WebsiteSQLCommands.FetchRoleResponsibilityById:
                    {
                        query = @"SELECT
                                STF_ROLE_RES_ID,
								ROLES__RESPONSIBILITIES,
                                IS_ACTIVE,
                                IS_DELETED,
                                ORDER_BY

                                FROM 
                                sup_staff_roles_responsibile

                                WHERE STF_ROLE_RES_ID=?STF_ROLE_RES_ID
                                AND IS_DELETED !=1;";
                        break;

                    }


                case WebsiteSQLCommands.DeleteRoleResponsibility:
                    {
                        query = @"UPDATE sup_staff_roles_responsibile 
                                SET 
                                IS_DELETED=?IS_DELETED
                                WHERE 
                               STF_ROLE_RES_ID=?STF_ROLE_RES_ID";
                        break;
                    }

                case WebsiteSQLCommands.FetchRoleResponsibilityByid:
                    {
                        query = @" SELECT 
                                    CERTIFICATE_ID,
                                    CERTIFICATE_NAME
                                FROM
                                    SUP_CERTIFICATE_LIST
                                WHERE
                                    FIND_IN_SET(?CATEGORY_ID, CATEGORY_ID)
                                    AND IS_ACTIVE=1
                                    AND IS_DELETED=0
                                ORDER BY ORDER_BY;";
                        break;
                    }
                    #endregion
            }
            return query;
        }
    }
}
