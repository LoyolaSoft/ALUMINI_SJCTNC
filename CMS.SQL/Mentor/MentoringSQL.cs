using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAO;

namespace CMS.SQL.Mentor
{
    public class MentoringSQL
    {
        public static string GetMentoring(MentoringSQLCommads sEnumCommand)
        {

            string query = "";
            switch (sEnumCommand)
            {
                case MentoringSQLCommads.FetchStudentlistForMentoringAllotment:
                    {
                        query = @"SELECT 
                                        CS.STUDENT_ID,
                                        CS.CLASS_ID,
                                        CS.ACADEMIC_YEAR,
                                        STU.ADMISSION_NO,
                                        STU.ROLL_NO,
                                        STU.FIRST_NAME AS STUDENT_NAME,
                                        SGEN.GENDER_NAME AS STUDENT_GENDER,
                                        CPC.CLASS_NAME,
                                        CP.PROGRAMME_NAME
                                    FROM
                                        STU_CLASS AS CS
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS STU ON STU.STUDENT_ID = CS.STUDENT_ID
                                            AND STU.IS_DELETED <> 1
                                            AND STU.IS_LEFT <> 1
                                            INNER JOIN
                                        sup_gender AS SGEN ON SGEN.GENDER_ID = STU.GENDER
                                            AND SGEN.IS_DELETED <> 1
                                            INNER JOIN
                                        CP_CLASSES AS CPC ON CPC.CLASS_ID = CS.CLASS_ID
                                            AND CS.PROGRAMME_GROUP_ID = CPC.PROGRAMME_GROUP_ID
                                            -- AND CPC.CLASS_LEVEL IN (?CLASS_LEVEL)
                                            AND CPC.IS_DELETED <> 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID = CPC.PROGRAMME_GROUP_ID
                                            AND CPP.PROGRAMME_GROUP_ID = CS.PROGRAMME_GROUP_ID
                                            AND CPP.IS_DELETED <> 1
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPP.PROGRAMME_ID
                                            AND CP.IS_DELETED <> 1
                                    WHERE
                                        CPC.SHIFT IN (?SHIFT)
                                            AND CPC.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                                            AND CS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND CPC.CLASS_YEAR IN (?CLASS_YEAR)
                                            AND CPP.APPTYPE_ID IN (?APPTYPE_ID)
                                            AND CS.IS_DELETED <> 1;";
                        break;

                    }
                case MentoringSQLCommads.FetchMentorStaffList:
                    {
                        query = @"SELECT 
                                        stf.STAFF_ID, STAFF_CODE, FIRST_NAME, GENDER
                                    FROM
                                        STF_PERSONAL_INFO AS STF
                                            INNER JOIN
                                        MENTOR_ELIGIBILITY_STAFF_DETAILS AS MES ON MES.STAFF_ID = STF.STAFF_ID
                                    WHERE
                                        MES.IS_DELETED <> 1
                                            AND STF.IS_DELETED <> 1
        AND STF.IS_LEFT <> 1;";
                        break;
                    }
                case MentoringSQLCommads.SaveMentoringAllocation:
                    {
                        query = @"INSERT INTO MENTORING_STUDENT_LIST (STUDENT_ID, CLASS_ID, STAFF_ID, BATCH, CREATED_BY,CREATED_DATE) VALUES (STUDENT_ID,CLASS_ID,STAFF_ID,BATCH,CREATED_BY,CREATED_DATE);";
                        break;
                    }
                case MentoringSQLCommads.FetchMentoringSetting:
                    {
                        query = @"SELECT 
                                    MEN.MENTORING_STAFF_SETTING_ID,
                                    MEN.STAFF_ID,
                                    MEN.MAX_STUDENT_COUNT,
                                    MEN.BATCH,
                                    STF.GENDER
                                FROM
                                    MENTORING_STAFF_SETTINGS AS MEN
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = MEN.STAFF_ID
                                WHERE
                                    IS_DELTED <> 1 AND BATCH = ?BATCH;";
                        break;
                    }
                case MentoringSQLCommads.FetchAllotedMetorstudentandStaffList:
                    {
                        query = @"SELECT 
                                        ME.STUDENT_ID,
                                        CS.ACADEMIC_YEAR,
                                        STU.ADMISSION_NO,
                                        STU.PHOTO,
                                        STU.ROLL_NO,
                                        STU.FIRST_NAME AS STUDENT_NAME,
                                        SGEN.GENDER_NAME AS STUDENT_GENDER,
                                        CPC.CLASS_NAME,
                                        CP.PROGRAMME_NAME,
                                        STF.FIRST_NAME AS STAFF_NAME,
                                        GEN.GENDER_NAME AS STAFF_GENDER
                                    FROM
                                        MENTORING_STUDENT_LIST AS ME
                                            INNER JOIN
                                        STU_CLASS AS CS ON ME.STUDENT_ID = CS.STUDENT_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS STU ON STU.STUDENT_ID = CS.STUDENT_ID
                                            AND STU.IS_DELETED <> 1
                                            AND STU.IS_LEFT <> 1
                                            INNER JOIN
                                        CP_CLASSES AS CPC ON CPC.CLASS_ID = CS.CLASS_ID
                                            AND CS.PROGRAMME_GROUP_ID = CPC.PROGRAMME_GROUP_ID
                                            AND CPC.IS_DELETED <> 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID = CPC.PROGRAMME_GROUP_ID
                                            AND CPP.PROGRAMME_GROUP_ID = CS.PROGRAMME_GROUP_ID
                                            AND CPP.IS_DELETED <> 1
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPP.PROGRAMME_ID
                                            AND CP.IS_DELETED <> 1
                                            INNER JOIN
                                        stf_personal_info AS STF ON STF.STAFF_ID = ME.STAFF_ID
                                            AND STF.IS_DELETED <> 1
                                            AND STF.IS_LEFT <> 1
                                            INNER JOIN
                                        sup_gender AS GEN ON GEN.GENDER_ID = STF.GENDER
                                            AND GEN.IS_DELETED <> 1
                                            INNER JOIN
                                        sup_gender AS SGEN ON SGEN.GENDER_ID = STU.GENDER
                                            AND SGEN.IS_DELETED <> 1
                                    WHERE
                                        CPC.SHIFT IN (?SHIFT)
                                            AND CPC.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                                            AND CS.ACADEMIC_YEAR = ?ACADEMIC_YEAR AND ME.BATCH = ?ACADEMIC_YEAR
                                            AND CPC.CLASS_YEAR IN (?CLASS_YEAR)
                                            AND CPP.APPTYPE_ID IN (?APPTYPE_ID)
                                            AND CS.IS_DELETED <> 1
                                    GROUP BY CS.STUDENT_ID;";
                        break;
                    }
                case MentoringSQLCommads.FetchGlobalAllotedMentoringDetails:
                    {
                        query = @"SELECT 
                                        ME.STUDENT_ID,
                                        ME.STAFF_ID,
                                        CS.ACADEMIC_YEAR,
                                        STU.ADMISSION_NO,
                                        STU.ROLL_NO,
                                        STU.FIRST_NAME AS STUDENT_NAME,
                                        SGEN.GENDER_NAME AS STUDENT_GENDER,
                                        CPC.CLASS_NAME,
                                        CP.PROGRAMME_NAME,
                                        STF.FIRST_NAME AS STAFF_NAME,
                                        GEN.GENDER_NAME AS STAFF_GENDER
                                    FROM
                                        MENTORING_STUDENT_LIST AS ME
                                            INNER JOIN
                                        STU_CLASS AS CS ON ME.STUDENT_ID = CS.STUDENT_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS STU ON STU.STUDENT_ID = CS.STUDENT_ID
                                            AND STU.IS_DELETED <> 1
                                            AND STU.IS_LEFT <> 1
                                            INNER JOIN
                                        CP_CLASSES AS CPC ON CPC.CLASS_ID = CS.CLASS_ID
                                            AND CS.PROGRAMME_GROUP_ID = CPC.PROGRAMME_GROUP_ID
                                            AND CPC.IS_DELETED <> 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID = CPC.PROGRAMME_GROUP_ID
                                            AND CPP.PROGRAMME_GROUP_ID = CS.PROGRAMME_GROUP_ID
                                            AND CPP.IS_DELETED <> 1
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPP.PROGRAMME_ID
                                            AND CP.IS_DELETED <> 1
                                            INNER JOIN
                                        stf_personal_info AS STF ON STF.STAFF_ID = ME.STAFF_ID
                                            AND STF.IS_DELETED <> 1
                                            AND STF.IS_LEFT <> 1
                                            INNER JOIN
                                        sup_gender AS GEN ON GEN.GENDER_ID = STF.GENDER
                                            AND GEN.IS_DELETED <> 1
                                            INNER JOIN
                                        sup_gender AS SGEN ON SGEN.GENDER_ID = STU.GENDER
                                            AND SGEN.IS_DELETED <> 1
                                    WHERE
                                        CS.ACADEMIC_YEAR = ?ACADEMIC_YEAR   
                                            AND ME.BATCH = ?ACADEMIC_YEAR
                                            AND CS.IS_DELETED <> 1
                                    GROUP BY CS.STUDENT_ID;";
                        break;
                    }
                case MentoringSQLCommads.FetchStaffWithCountForAllocation:
                    {
                        query = @"SELECT 
                                    MSL.STAFF_ID,
                                    SPI.STAFF_CODE,
                                    SPI.FIRST_NAME AS STAFF_NAME,
                                    COUNT(MSL.STUDENT_ID) AS COUNT,
                                    SPI.GENDER
                                FROM
                                    MENTORING_STUDENT_LIST AS MSL
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = MSL.STAFF_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = MSL.STUDENT_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS STU ON STU.STUDENT_ID = SC.STUDENT_ID
                                WHERE
                                    MSL.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND STU.IS_DELETED != 1
                                        AND STU.IS_LEFT != 1 AND SPI.GENDER=1
                                        AND SPI.STAFF_ID not IN (SELECT 
                                            STAFF_ID
                                        FROM
                                            MENTORING_STAFF_SETTINGS AS MSS
                                        WHERE
                                            MSS.IS_DELTED != 1)
                                GROUP BY STAFF_ID;";
                        break;
                    }
            }
            return query;
        }
    }
}