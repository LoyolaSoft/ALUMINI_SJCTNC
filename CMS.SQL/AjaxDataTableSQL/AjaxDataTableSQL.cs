using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.AjaxDataTableSQL
{
   public static class AjaxDataTableSQL
    {
        public static string GetAjaxDataTableSQL(AjaxDataTableCommands sEnumCommand)
        {
            string query = "";
            switch (sEnumCommand)
            {
                case AjaxDataTableCommands.StudentList:
                    {
                        query = @"SELECT 
                                        RUN_NUMBER,
                                        STUDENT_ID,
                                        ROLL_NO,
                                        REGISTER_NO,
                                        FIRST_NAME,
                                        DEPARTMENT,
                                        CLASS_NAME,
                                        SHIFT_NAME,
                                        IS_LEFT,
                                        PHOTO
                                    FROM
                                        (SELECT 
                                            (@rm:=@rm + 1) AS RUN_NUMBER,
                                                SPI.STUDENT_ID,
                                                ROLL_NO,
                                                REGISTER_NO,
                                                FIRST_NAME,
                                                D.DEPARTMENT,
                                                CC.CLASS_NAME,
                                                S.SHIFT_NAME,
                                                IF(SPI.IS_LEFT = 0, 'NO', 'YES') AS IS_LEFT,
                                                SPI.PHOTO
                                        FROM
                                            STU_PERSONAL_INFO SPI
                                        LEFT JOIN CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = SPI.DEPT_ID
                                            AND D.IS_DELETED != 1
                                        LEFT JOIN SUP_SHIFT AS S ON S.SHIFT_ID = SPI.SHIFT_ID
                                        LEFT JOIN STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                        INNER JOIN CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            AND CC.IS_DELETED != 1, (SELECT @rm:=0) AS T
                                        WHERE                                            
                                            CC.IS_DELETED != 1
                                        ORDER BY RUN_NUMBER , SPI.ROLL_NO) AS outT
                                    WHERE
                                        outT.RUN_NUMBER > {0}
                                            {1}
                                    LIMIT {2};";
                        break;
                    }
                case AjaxDataTableCommands.StudentTotalCount:
                    {
                        query = @"SELECT 
	                                    COUNT(SPI.STUDENT_ID) AS COUNT_FIELD
                                    FROM
                                        STU_PERSONAL_INFO SPI
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR                                          
                                    WHERE
                                         SC.IS_DELETED != 1;";
                        break;
                    }
            }
            return query;
        }
    }
}
