using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.Placement
{
    public static class PlacementSQL
    {
        public static string GetPlacementSQL(PlacementSQLCommands sEnumCommand)
        {
            string query = "";

            switch (sEnumCommand)
            {
                #region PlaceMent News Portal 
                case PlacementSQLCommands.SaveStuplacementNews:
                    {
                        query = @"INSERT INTO stu_placement_news(
                                                           
                                                            FROM_DATE,
                                                            TO_DATE,
                                                            FILE_PATH,
                                                            IMG_PATH,
                                                            DESCRIPTION,
                                                            ENTRY_ID,
                                                            LINKTEXT,   
                                                            ACADEMIC_YEAR,   
                                                            SEMESTER_TYPE,   
                                                            LINK,    
                                                            IS_UG,    
                                                            IS_PG,    
                                                            IS_INTERVIEW,    
                                                            OTHERS,    
                                                            TITLE)
                                                        VALUES(
                                                            ?FROM_DATE,
                                                            ?TO_DATE,
                                                            ?FILE_PATH,
                                                            ?IMG_PATH,
                                                            ?DESCRIPTION,
                                                            ?ENTRY_ID,
                                                            ?LINKTEXT,   
                                                            ?ACADEMIC_YEAR,   
                                                            ?SEMESTER_TYPE,   
                                                            ?LINK, 
                                                            ?IS_UG,    
                                                            ?IS_PG,    
                                                            ?IS_INTERVIEW,    
                                                            ?OTHERS,
                                                            ?TITLE);";
                        break;
                    }
                case PlacementSQLCommands.UpdatePlacementFilePathByID:
                    {
                        query = @"  UPDATE stu_placement_news SET FILE_PATH=?FILE_PATH WHERE PLACEMENT_ID=?PLACEMENT_ID";
                        break;
                    }
                case PlacementSQLCommands.UpdatePlacementImgPathbyID:
                    {
                        query = @"UPDATE stu_placement_news SET IMG_PATH=?IMG_PATH WHERE PLACEMENT_ID=?PLACEMENT_ID";
                        break;
                    }
                case PlacementSQLCommands.UpdateStuplacementNewsWithoutImage:
                    {
                        query = @"UPDATE stu_placement_news 
                                            SET 
                                                FROM_DATE = ?FROM_DATE,
                                                TO_DATE = ?TO_DATE,
                                                DESCRIPTION = ?DESCRIPTION,
                                                LINKTEXT = ?LINKTEXT,
                                                LINK = ?LINK,
                                                IS_UG = ?IS_UG,
                                                IS_PG = ?IS_PG,
                                                IS_INTERVIEW = ?IS_INTERVIEW,
                                                OTHERS = ?OTHERS,
                                                TITLE = ?TITLE
                                            WHERE
                                                PLACEMENT_ID = ?PLACEMENT_ID;";
                        break;
                    }
                case PlacementSQLCommands.FetchstuPlacementNews:
                    {
                        query = @"SELECT 
                                    PLACEMENT_ID,
                                    TITLE,
                                    date_format(FROM_DATE,'%d/%m/%Y') as FROM_DATE,
                                    date_format(TO_DATE,'%d/%m/%Y') as TO_DATE,
                                    FILE_PATH,
                                    IMG_PATH,
                                    DESCRIPTION,
                                    ENTRY_ID,
                                    ACADEMIC_YEAR,
                                    SEMESTER_TYPE,
                                    LINK,
                                    IS_UG,
                                    IS_PG,
                                    IS_INTERVIEW,
                                    OTHERS,
                                    LINKTEXT
                                FROM
                                    stu_placement_news 
                                where IS_DELETED !=1 and ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case PlacementSQLCommands.FetchstuPlacementNewsByID:
                    {
                        query = @"SELECT 
                                    PLACEMENT_ID,
                                    TITLE,
                                    date_format(FROM_DATE,'%d/%m/%Y') as FROM_DATE,
                                    date_format(TO_DATE,'%d/%m/%Y') as TO_DATE,
                                    FILE_PATH,
                                    IMG_PATH,
                                    DESCRIPTION,
                                    ENTRY_ID,
                                    ACADEMIC_YEAR,
                                    SEMESTER_TYPE,
                                    LINK,
                                    IS_UG,
                                    IS_PG,
                                    IS_INTERVIEW,
                                    OTHERS,
                                    LINKTEXT
                                FROM
                                    stu_placement_news 
                                where IS_DELETED !=1 and PLACEMENT_ID=?PLACEMENT_ID;";
                        break;
                    }
                case PlacementSQLCommands.DeleteStudentPlacementNews:
                    {
                        query = @" UPDATE stu_placement_news 
                                        SET 
                                            IS_DELETED = ?IS_DELETED
                                        WHERE
                                            PLACEMENT_ID = ?PLACEMENT_ID";
                        break;
                    }
                case PlacementSQLCommands.FetchActiveInterviewList:
                    {
                        query = @"SELECT 
                                    PLACEMENT_ID,
                                    TITLE,
                                    DATE_FORMAT(FROM_DATE, '%d/%m/%Y') AS FROM_DATE,
                                    DATE_FORMAT(TO_DATE, '%d/%m/%Y') AS TO_DATE,
                                    FILE_PATH,
                                    IMG_PATH,
                                    DESCRIPTION,
                                    ENTRY_ID,
                                    ACADEMIC_YEAR,
                                    SEMESTER_TYPE,
                                    LINK,
                                    IS_UG,
                                    IS_PG,
                                    IS_INTERVIEW,
                                    OTHERS,
                                    LINKTEXT
                                FROM
                                    stu_placement_news
                                WHERE
                                    IS_DELETED != 1 AND TO_DATE >= CURDATE() and IS_INTERVIEW =1  
									-- UG and  IS_UG=1
                                    -- PG and  IS_PG=1
                                    and ACADEMIC_YEAR=?AC_YEAR
                                    order by PLACEMENT_ID desc;";
                        break;
                    }
                case PlacementSQLCommands.FetchCompletedInterviewList:
                    {
                        query = @"SELECT 
                                        PLACEMENT_ID,
                                        TITLE,
                                        DATE_FORMAT(FROM_DATE, '%d/%m/%Y') AS FROM_DATE,
                                        DATE_FORMAT(TO_DATE, '%d/%m/%Y') AS TO_DATE,
                                        FILE_PATH,
                                        IMG_PATH,
                                        DESCRIPTION,
                                        ENTRY_ID,
                                        ACADEMIC_YEAR,
                                        SEMESTER_TYPE,
                                        LINK,
                                        IS_UG,
                                        IS_PG,
                                        IS_INTERVIEW,
                                        OTHERS,
                                        LINKTEXT
                                    FROM
                                        stu_placement_news
                                    WHERE
                                        IS_DELETED != 1 
                                        -- UG and  IS_UG=1
                                        -- PG and  IS_PG=1
                                        and ACADEMIC_YEAR=?AC_YEAR
                                        AND TO_DATE < CURDATE() 
                                        and IS_INTERVIEW =1 order by PLACEMENT_ID desc;";
                        break;
                    }
                case PlacementSQLCommands.FetchOtherAdList:
                    {
                        query = @"SELECT 
                                    PLACEMENT_ID,
                                    TITLE,
                                    DATE_FORMAT(FROM_DATE, '%d/%m/%Y') AS FROM_DATE,
                                    DATE_FORMAT(TO_DATE, '%d/%m/%Y') AS TO_DATE,
                                    FILE_PATH,
                                    IMG_PATH,
                                    DESCRIPTION,
                                    ENTRY_ID,
                                    ACADEMIC_YEAR,
                                    SEMESTER_TYPE,
                                    LINK,
                                    IS_UG,
                                    IS_PG,
                                    IS_INTERVIEW,
                                    OTHERS,
                                    LINKTEXT
                                FROM
                                    stu_placement_news
                                WHERE
                                    IS_DELETED != 1 
                                     -- UG and  IS_UG=1
                                     -- PG and  IS_PG=1
                                    and IS_INTERVIEW !=1 
                                    and ACADEMIC_YEAR=?AC_YEAR
                                    and OTHERS =1 order by PLACEMENT_ID desc;";
                        break;
                    }
                case PlacementSQLCommands.FetchStuInfoForValidate:
                    {
                        query = @"SELECT 
                                        spi.STUDENT_ID,
                                        ROLL_NO,
                                        CLASS_YEAR,
                                        CLASS_LEVEL
                                    FROM
                                        stu_class AS sc
                                            INNER JOIN
                                        cp_classes AS cc ON cc.CLASS_ID = sc.CLASS_ID
                                            INNER JOIN
                                        stu_personal_info AS spi ON spi.STUDENT_ID = sc.STUDENT_ID
                                    WHERE
                                        sc.STUDENT_ID = ?STUDENT_ID
                                            AND spi.IS_DELETED != 1
                                            AND spi.IS_LEFT != 1
                                            AND sc.IS_DELETED != 1
                                            AND sc.ACADEMIC_YEAR = ?AC_YEAR;";
        
                        break;
                    }
                #endregion

                #region Campus Interview Selected Student Details 

                case PlacementSQLCommands.FetchInterviewPlacedStudentList:
                    {
                        query = @"SELECT 
                                        ROLL_NO,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(LAST_NAME, '')) AS FIRST_NAME,
                                        YEAR_OF_PLACEMENT,
                                        COMPANY_NAME,
                                        POSITION_OFFERED,
                                        SALARY_PACKAGE,
                                        SIP.IS_WEB_PLACEMENT,
                                        SPI.PHOTO,
                                        PLACED_ID,
                                        SIP.REMARKS,
                                        FILE_PATH,
                                        IMG_PATH,
                                        CP.PROGRAMME_NAME
                                    FROM
                                        STU_INTERVIEW_PLACED_INFO AS SIP
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SIP.STUDENT_ID
                                            LEFT JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SPI.PROGRAM_ID
                                            LEFT JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                    WHERE
                                        SIP.IS_DELETED != 1
                                            AND YEAR_OF_PLACEMENT = ?YEAR_OF_PLACEMENT;";
                        break;
                    }
                     case PlacementSQLCommands.FetchInterviewPlacedStudentListById:
                    {
                        query = @"SELECT 
                                        ROLL_NO,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(LAST_NAME, '')) AS FIRST_NAME,
                                        YEAR_OF_PLACEMENT,
                                        COMPANY_NAME,
                                        POSITION_OFFERED,
                                        IS_WEB_PLACEMENT,
                                        SALARY_PACKAGE,
                                        PLACED_ID,
                                        SIP.REMARKS,
                                        FILE_PATH,
                                        IMG_PATH,
                                        CP.PROGRAMME_NAME
                                    FROM
                                        STU_INTERVIEW_PLACED_INFO AS SIP
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SIP.STUDENT_ID
                                            LEFT JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SPI.PROGRAM_ID
                                            LEFT JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                    WHERE
                                        SIP.IS_DELETED != 1
                                            AND PLACED_ID = ?PLACED_ID;";
                        break;
                    }
                case PlacementSQLCommands.CheckRollNoExistOrNot:
                    {
                        query = @" SELECT 
                                        STUDENT_ID, ROLL_NO, FIRST_NAME
                                    FROM
                                        stu_personal_info
                                    WHERE
                                        ROLL_NO = ?ROLL_NO AND IS_DELETED != 1;";
                        break;
                    } 
                case PlacementSQLCommands.SavePlacedStudentDetails:
                    {
                        query = @" INSERT INTO STU_INTERVIEW_PLACED_INFO 
	                                                                 (ENTRY_ID,
	                                                                 STUDENT_ID,
	                                                                 YEAR_OF_PLACEMENT,
	                                                                 COMPANY_NAME,
	                                                                 POSITION_OFFERED,
	                                                                 SALARY_PACKAGE,
	                                                                 REMARKS,
	                                                                 FILE_PATH,
	                                                                 IS_WEB_PLACEMENT,
	                                                                 IMG_PATH)
                                                                 VALUES(
	                                                                 ?ENTRY_ID,
	                                                                 ?STUDENT_ID,
	                                                                 ?YEAR_OF_PLACEMENT,
	                                                                 ?COMPANY_NAME,
	                                                                 ?POSITION_OFFERED,
	                                                                 ?SALARY_PACKAGE,
	                                                                 ?REMARKS,
	                                                                 ?FILE_PATH,
	                                                                 ?IS_WEB_PLACEMENT,
	                                                                 ?IMG_PATH
                                                                 );";
                        break;
                    }
                case PlacementSQLCommands.UpdatePlacedStudentDetailsExceptFileImage:
                    {
                        query = @"UPDATE STU_INTERVIEW_PLACED_INFO 
                                            SET 
                                                STUDENT_ID = ?STUDENT_ID,
                                                YEAR_OF_PLACEMENT = ?YEAR_OF_PLACEMENT,
                                                COMPANY_NAME = ?COMPANY_NAME,
                                                POSITION_OFFERED = ?POSITION_OFFERED,
                                                REMARKS = ?REMARKS,
                                                IS_WEB_PLACEMENT = ?IS_WEB_PLACEMENT,
                                                SALARY_PACKAGE = ?SALARY_PACKAGE
                                                
                                            WHERE
                                                PLACED_ID = ?PLACED_ID;";
                        break;
                    }
                case PlacementSQLCommands.UpdatePlacedStudentDetailsFilePathByID:
                    {
                        query = @"  UPDATE STU_INTERVIEW_PLACED_INFO SET FILE_PATH=?FILE_PATH WHERE PLACED_ID=?PLACED_ID";
                        break;
                    }
                case PlacementSQLCommands.UpdatePlacedStudentDetailsImgPathbyID:
                    {
                        query = @"UPDATE STU_INTERVIEW_PLACED_INFO SET IMG_PATH=?IMG_PATH WHERE PLACED_ID=?PLACED_ID";
                        break;
                    }
                case PlacementSQLCommands.DeleteInterViewPlacedStudentNews:
                    {
                        query = @" UPDATE STU_INTERVIEW_PLACED_INFO 
                                        SET 
                                            IS_DELETED = ?IS_DELETED
                                        WHERE
                                            PLACED_ID = ?PLACED_ID";
                        break;
                    }
                    #endregion
            }
            return query;
        }
    }
}
