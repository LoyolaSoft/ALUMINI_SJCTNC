using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.Payroll
{
    public static class PayrollSQL
    {
        public static string GetPayrollSQL(PayrollSQLCommads sEnumCommand)
        {
            string query = string.Empty;
            switch (sEnumCommand)
            {
                case PayrollSQLCommads.FetchSalaryTemplates:
                    {
                        query = @"SELECT 
                                SD.TEMPLATE_ID,
                                SD.TEMPLATE_NAME,
                                SD.DESCRIPTION,
                                SD.STAFF_CATEGORY,
                                SC.STF_CATEGORY
                            FROM
                                PR_SALARY_TEMPLATE_DETAILS AS SD
                                    INNER JOIN
                                SUP_STAFF_CATEGORY AS SC ON SC.STF_CATEGORY_ID = SD.STAFF_CATEGORY
                            WHERE
                                SD.IS_DELETED != 1
                                    -- AND SD.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case PayrollSQLCommads.FetchSalaryTemplateById:
                    {
                        query = @"SELECT 
                                SD.TEMPLATE_ID,
                                SD.TEMPLATE_NAME,
                                SD.DESCRIPTION,
                                SD.STAFF_CATEGORY,
                                SC.STF_CATEGORY
                            FROM
                                PR_SALARY_TEMPLATE_DETAILS AS SD
                                    INNER JOIN
                                SUP_STAFF_CATEGORY AS SC ON SC.STF_CATEGORY_ID = SD.STAFF_CATEGORY
                            WHERE
                                SD.IS_DELETED != 1
                                    AND SD.TEMPLATE_ID =?TEMPLATE_ID;";
                        break;
                    }
                case PayrollSQLCommads.SaveSalaryTemplate:
                    {
                        query = @"INSERT INTO 
                                    PR_SALARY_TEMPLATE_DETAILS(TEMPLATE_NAME
                                    ,DESCRIPTION,STAFF_CATEGORY) 
                                    VALUES(?TEMPLATE_NAME,?DESCRIPTION,?STAFF_CATEGORY);";
                        break;
                    }
                case PayrollSQLCommads.UpdateSalaryTemplate:
                    {
                        query = @"UPDATE PR_SALARY_TEMPLATE_DETAILS 
                                SET 
                                    TEMPLATE_NAME = ?TEMPLATE_NAME,
                                    DESCRIPTION = ?DESCRIPTION,
                                    STAFF_CATEGORY =?STAFF_CATEGORY
                                WHERE
                                    TEMPLATE_ID = ?TEMPLATE_ID;";
                        break;
                    }
                case PayrollSQLCommads.DeleteSalaryTemplates:
                    {
                        query = @"UPDATE PR_SALARY_TEMPLATE_DETAILS 
                                SET 
                                    IS_DELETED = 1
  
                                WHERE
                                    TEMPLATE_ID = ?TEMPLATE_ID;";
                        break;
                    }
                case PayrollSQLCommads.FetchSalaryComponents:
                    {
                        query = @"SELECT 
                                SCD.COMPONENT_ID,
                                SCD.COMPONENT_NAME,
                                SCD.NAME_IN_PAYSLIP,
                                SCD.SALARY_COMPONENT_TYPE,
                                SCT.SALARY_COMPONENT_NAME
                            FROM
                                PR_SALARY_COMPONENT_DETAILS AS SCD
                                    INNER JOIN
                                SUP_SALARY_COMPONENT_TYPE AS SCT ON SCT.SALARY_COMPONENT_TYPE_ID = SCD.SALARY_COMPONENT_TYPE
                            WHERE
                                SCD.IS_DELETED != 1
                                    -- AND SCD.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case PayrollSQLCommads.FetchSalaryComponentById:
                    {
                        query = @"SELECT 
                                SCD.COMPONENT_ID,
                                SCD.COMPONENT_NAME,
                                SCD.NAME_IN_PAYSLIP,
                                SCD.SALARY_COMPONENT_TYPE,
                                SCT.SALARY_COMPONENT_NAME
                            FROM
                                PR_SALARY_COMPONENT_DETAILS AS SCD
                                    INNER JOIN
                                SUP_SALARY_COMPONENT_TYPE AS SCT ON SCT.SALARY_COMPONENT_TYPE_ID = SCD.SALARY_COMPONENT_TYPE
                            WHERE
                                SCD.IS_DELETED != 1
                                    AND SCD.COMPONENT_ID = ?COMPONENT_ID;";
                        break;
                    }
                case PayrollSQLCommads.SaveSalaryComponent:
                    {
                        query = @"INSERT INTO PR_SALARY_COMPONENT_DETAILS
                                    (COMPONENT_NAME,
                                    NAME_IN_PAYSLIP,                                   
                                    SALARY_COMPONENT_TYPE) 
                                    VALUES(?COMPONENT_NAME,
                                    ?NAME_IN_PAYSLIP,                                 
                                    ?SALARY_COMPONENT_TYPE);";
                        break;
                    }
                case PayrollSQLCommads.UpdateSalaryComponent:
                    {
                        query = @"UPDATE PR_SALARY_COMPONENT_DETAILS 
                                SET 
                                    COMPONENT_NAME = ?COMPONENT_NAME,
                                    NAME_IN_PAYSLIP =?NAME_IN_PAYSLIP,                                  
                                    SALARY_COMPONENT_TYPE =?SALARY_COMPONENT_TYPE
                                                                      
                                WHERE
                                    COMPONENT_ID =?COMPONENT_ID;";
                        break;
                    }
                case PayrollSQLCommads.DeleteSalaryComponents:
                    {
                        query = @"UPDATE PR_SALARY_COMPONENT_DETAILS 
                                SET 
                                    IS_DELETED = 1
                                WHERE
                                    COMPONENT_ID = ?COMPONENT_ID;";
                        break;
                    }
                case PayrollSQLCommads.FetchSalaryTemplateComponentsByTemplateId:
                    {
                        query = @"SELECT 
                                TCD.CTEMPLATE_ID,
                                TCD.TEMPLATE_ID,
                                TCD.COMPONENTT_ID,
                                SCD.COMPONENT_NAME,
                                TD.TEMPLATE_NAME,
                                TCD.IS_ACTIVE,
                                TCD.ACADEMIC_YEAR,
                                TCD.PERCENTAGE_COMPONENT_ID,
                                TCD.PERCENTAGE,
                                TCD.FLAT_AMOUNT,
                                SCD1.COMPONENT_NAME AS PERCENTAGE_COMPONENT_NAME,
                                SCT.SALARY_COMPONENT_NAME
                            FROM
                                PR_SALARY_TEMPLATE_COMPONENT_DETAILS AS TCD
                                    INNER JOIN
                                PR_SALARY_COMPONENT_DETAILS AS SCD ON TCD.COMPONENTT_ID = SCD.COMPONENT_ID
                                    INNER JOIN
                                PR_SALARY_TEMPLATE_DETAILS AS TD ON TD.TEMPLATE_ID = TCD.TEMPLATE_ID
                                    LEFT JOIN
                                PR_SALARY_COMPONENT_DETAILS AS SCD1 ON SCD1.COMPONENT_ID = TCD.PERCENTAGE_COMPONENT_ID
                                    INNER JOIN
                                SUP_SALARY_COMPONENT_TYPE AS SCT ON SCT.SALARY_COMPONENT_TYPE_ID = SCD.SALARY_COMPONENT_TYPE
                            WHERE
                                    TCD.IS_DELETED != 1
                                    -- AND TCD.ACADEMIC_YEAR = ?AC_YEAR
                                    AND TCD.TEMPLATE_ID = ?TEMPLATE_ID;";
                        break;
                    }
                case PayrollSQLCommads.FetchSalaryTemplateComponenstById:
                    {
                        query = @"SELECT 
                                TCD.CTEMPLATE_ID,
                                TCD.TEMPLATE_ID,
                                TCD.COMPONENTT_ID,
                                SCD.COMPONENT_NAME,
                                TD.TEMPLATE_NAME,
                                TCD.IS_ACTIVE,
                                TCD.ACADEMIC_YEAR,
                                TCD.PERCENTAGE_COMPONENT_ID,
                                TCD.PERCENTAGE,
                                TCD.FLAT_AMOUNT,
                                TCD.IS_MANUAL,
                                TCD.IS_GROSS_PAY,
                                SCD1.COMPONENT_NAME AS PERCENTAGE_COMPONENT_NAME
                            FROM
                                PR_SALARY_TEMPLATE_COMPONENT_DETAILS AS TCD
                                    INNER JOIN
                                PR_SALARY_COMPONENT_DETAILS AS SCD ON TCD.COMPONENTT_ID = SCD.COMPONENT_ID
                                    INNER JOIN
                                PR_SALARY_TEMPLATE_DETAILS AS TD ON TD.TEMPLATE_ID = TCD.TEMPLATE_ID
                                    LEFT JOIN
                                PR_SALARY_COMPONENT_DETAILS AS SCD1 ON SCD1.COMPONENT_ID = TCD.PERCENTAGE_COMPONENT_ID
                            WHERE
                                TCD.IS_DELETED != 1
                                    
                                    AND TCD.CTEMPLATE_ID = ?CTEMPLATE_ID;";
                        break;
                    }
                case PayrollSQLCommads.SaveSalaryTemplateComponents:
                    {
                        query = @"INSERT INTO PR_SALARY_TEMPLATE_COMPONENT_DETAILS
                                    (TEMPLATE_ID,
									COMPONENTT_ID,
                                    PERCENTAGE,
                                    FLAT_AMOUNT,
                                    PERCENTAGE_COMPONENT_ID,
                                    IS_MANUAL,IS_GROSS_PAY) 
                                    VALUES(?TEMPLATE_ID,?COMPONENTT_ID,
                                    ?PERCENTAGE,
                                    ?FLAT_AMOUNT,
                                    ?PERCENTAGE_COMPONENT_ID,
                                    ?IS_MANUAL,?IS_GROSS_PAY);";
                        break;
                    }
                case PayrollSQLCommads.UpdateSalaryTemplateComponents:
                    {
                        query = @"UPDATE PR_SALARY_TEMPLATE_COMPONENT_DETAILS 
                                SET 
                                    COMPONENTT_ID=?COMPONENTT_ID,
                                    PERCENTAGE =?PERCENTAGE,
                                    FLAT_AMOUNT =?FLAT_AMOUNT,					
                                    PERCENTAGE_COMPONENT_ID =?PERCENTAGE_COMPONENT_ID,
                                    IS_MANUAL = ?IS_MANUAL,
                                    IS_GROSS_PAY = ?IS_GROSS_PAY
                                WHERE
                                    CTEMPLATE_ID =?CTEMPLATE_ID;";
                        break;
                    }
                case PayrollSQLCommads.DeleteSalaryTemplateComponents:
                    {
                        query = @"UPDATE PR_SALARY_TEMPLATE_COMPONENT_DETAILS 
                                SET 
                                   IS_DELETED=1
                                WHERE
                                    CTEMPLATE_ID =?CTEMPLATE_ID;";
                        break;
                    }
                case PayrollSQLCommads.SetActiveandDeactiveTemplateComponents:
                    {
                        query = @"UPDATE PR_SALARY_TEMPLATE_COMPONENT_DETAILS 
                                    SET 
                                       IS_ACTIVE=?IS_ACTIVE
                                    WHERE
                                        CTEMPLATE_ID =?CTEMPLATE_ID;";
                        break;
                    }
                case PayrollSQLCommads.CheckIsComponentExists:
                    {
                        query = @"SELECT 
                                TEMPLATE_ID, COMPONENTT_ID
                            FROM
                                PR_SALARY_TEMPLATE_COMPONENT_DETAILS
                            WHERE
                                TEMPLATE_ID =?TEMPLATE_ID  AND COMPONENTT_ID =?COMPONENTT_ID;";
                        break;
                    }

                case PayrollSQLCommads.FetchEmployeeSalaryTemplates:
                    {
                        query = @"SELECT 
                                    SP.STAFF_ID AS EMPLOYEE_ID,
                                    SP.FIRST_NAME,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH, '%Y-%m-%d') AS DOB,
                                    SP.STAFF_CODE,
                                    DATE_FORMAT(SP.DATE_OF_JOIN, '%Y-%m-%d') AS DATE_OF_JOIN,
                                    SC.STF_CATEGORY,
                                    ST.EMP_SALARY_TEMP_ID,
                                    ST.TEMPLATE_ID,
                                    SQ.QUALIFICATION,
                                    SQ.QUALIFICATION_ID
                                FROM
                                    STF_PERSONAL_INFO AS SP
                                        LEFT JOIN
                                    PR_EMPOLYEE_SALARY_TEMPLATE AS ST ON SP.STAFF_ID = ST.EMPLOYEE_ID  AND ST.IS_DELETED!=1
                                        LEFT JOIN
                                    SUP_STAFF_CATEGORY AS SC ON SC.STF_CATEGORY_ID = SP.CATEGORY
                                        LEFT JOIN
                                    SUP_QUALIFICATION AS SQ ON SQ.QUALIFICATION_ID = SP.QUALIFICATION
                                WHERE
                                    SP.IS_DELETED != 1 AND SP.IS_LEFT != 1;";
                        break;
                    }
                case PayrollSQLCommads.SaveEmpolyeeSalarytemplate:
                    {
                        query = @"INSERT INTO 
                                PR_EMPOLYEE_SALARY_TEMPLATE(TEMPLATE_ID,EMPLOYEE_ID) 
                                VALUES(?TEMPLATE_ID,?EMPLOYEE_ID);";
                        break;
                    }
                case PayrollSQLCommads.UpdateEmployeeSalarytemplate:
                    {
                        query = @"UPDATE PR_EMPOLYEE_SALARY_TEMPLATE 
                                SET 
                                    TEMPLATE_ID =?TEMPLATE_ID,
                                    EMPLOYEE_ID =?EMPLOYEE_ID,
                                    IS_DELETED=0
                                WHERE
                                    EMP_SALARY_TEMP_ID =?EMP_SALARY_TEMP_ID;";
                        break;
                    }
                case PayrollSQLCommads.UpdateDeleteInEmployeetemplate:
                    {
                        query = @"UPDATE PR_EMPOLYEE_SALARY_TEMPLATE 
                                    SET 
                                       IS_DELETED=1
                                    WHERE
                                        EMP_SALARY_TEMP_ID =?EMP_SALARY_TEMP_ID;";
                        break;
                    }
                case PayrollSQLCommads.UpdateIsDeletedInEmployeeTemplates:
                    {
                        query = @"UPDATE PR_EMPOLYEE_SALARY_TEMPLATE 
                                SET 
                                    IS_DELETED = 1
                                WHERE
                                    ACADEMIC_YEAR = ?AC_YEAR";
                        break;
                    }
                case PayrollSQLCommads.FetchLeaveSetting:
                    {
                        query = @"SELECT 
                                LS.STF_LEAVE_SETTING_ID,
                                SC.STF_CATEGORY,
                                SLT.LEAVE_TYPE,
                                LS.M_MAX_DAYS,
                                LS.Y_MAX_DAYS,
                                LS.EXP_YEAR_FROM,
                                LS.EXP_YEAR_TO
                            FROM
                                PR_LEAVE_SETTING AS LS
                                    INNER JOIN
                                SUP_STAFF_CATEGORY AS SC ON LS.STAFF_CATEGORY = SC.STF_CATEGORY_ID
                                    INNER JOIN
                                SUP_LEAVE_TYPE AS SLT ON SLT.LEAVE_TYPE_ID = LS.LEAVE_TYPE
                            WHERE
                                LS.IS_DELETED != 1
                                    AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case PayrollSQLCommads.FetchLeaveSettingById:
                    {
                        query = @"SELECT 
                                LS.STF_LEAVE_SETTING_ID,
                                LS.STAFF_CATEGORY,
                                SC.STF_CATEGORY,
                                LS.LEAVE_TYPE,
                                LS.M_MAX_DAYS,
                                LS.Y_MAX_DAYS,
                                LS.EXP_YEAR_FROM,
                                LS.EXP_YEAR_TO
                            FROM
                                PR_LEAVE_SETTING AS LS
                                    INNER JOIN
                                SUP_STAFF_CATEGORY AS SC ON LS.STAFF_CATEGORY = SC.STF_CATEGORY_ID
                                    INNER JOIN
                                SUP_LEAVE_TYPE AS SLT ON SLT.LEAVE_TYPE_ID = LS.LEAVE_TYPE
                            WHERE
                                LS.IS_DELETED != 1
                                    AND STF_LEAVE_SETTING_ID = ?STF_LEAVE_SETTING_ID;";
                        break;
                    }
                case PayrollSQLCommads.SaveLeaveSetting:
                    {
                        query = @"INSERT INTO 
                                PR_LEAVE_SETTING(STAFF_CATEGORY, 
                                LEAVE_TYPE, M_MAX_DAYS, 
                                Y_MAX_DAYS, ACADEMIC_YEAR,EXP_YEAR_FROM,EXP_YEAR_TO,IS_CARRIED_NEXT_YEAR) 
                                VALUES(?STAFF_CATEGORY,
                                 ?LEAVE_TYPE,
                                 ?M_MAX_DAYS,
                                 ?Y_MAX_DAYS, 
                                 ?AC_YEAR,?EXP_YEAR_FROM,?EXP_YEAR_TO,?IS_CARRIED_NEXT_YEAR);";
                        break;
                    }
                case PayrollSQLCommads.UpdateLeaveSetting:
                    {
                        query = @"UPDATE PR_LEAVE_SETTING 
                                    SET 
                                        STAFF_CATEGORY =?STAFF_CATEGORY,
                                        LEAVE_TYPE = ?LEAVE_TYPE,
                                        M_MAX_DAYS = ?M_MAX_DAYS,
                                        Y_MAX_DAYS = ?Y_MAX_DAYS,
                                        EXP_YEAR_FROM=?EXP_YEAR_FROM,
                                        EXP_YEAR_TO=?EXP_YEAR_TO,
                                        IS_CARRIED_NEXT_YEAR=?IS_CARRIED_NEXT_YEAR
                                    WHERE
                                        STF_LEAVE_SETTING_ID = ?STF_LEAVE_SETTING_ID;";
                        break;
                    }
                case PayrollSQLCommads.DeleteLeaveSetting:
                    {
                        query = @"UPDATE PR_LEAVE_SETTING 
                                    SET 
                                       IS_DELETED=1
                                    WHERE
                                        STF_LEAVE_SETTING_ID = ?STF_LEAVE_SETTING_ID;";
                        break;
                    }
                case PayrollSQLCommads.FetchPaidleaveapplied:
                    {
                        query = @"SELECT 
                                PLE.PAID_LEAVE_ID,
                                PLE.EMPLOYEE_ID,
                                SP.FIRST_NAME,
                                SS.LEAVE_TYPE,
                                DATE_FORMAT(PLE.DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                DATE_FORMAT(PLE.DATE_TO, '%d/%m/%Y') AS DATE_TO
                            FROM
                                PR_PAID_LEAVE_ENTRY AS PLE
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON PLE.EMPLOYEE_ID = SP.STAFF_ID
                                    INNER JOIN
                                SUP_LEAVE_TYPE AS SS ON SS.LEAVE_TYPE_ID = PLE.LEAVE_TYPE_ID
                            WHERE
                                PLE.IS_DELETED != 1
                                    AND ACADEMIC_YEAR =?AC_YEAR;";
                        break;
                    }
                case PayrollSQLCommads.FetchPaidleaveappliedById:
                    {
                        query = @"SELECT 
                                    PLE.PAID_LEAVE_ID,
                                    PLE.EMPLOYEE_ID,
                                    SP.FIRST_NAME,
                                    PLE.LEAVE_TYPE_ID,
                                    SS.LEAVE_TYPE,
                                    DATE_FORMAT(PLE.DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                    DATE_FORMAT(PLE.DATE_TO, '%d/%m/%Y') AS DATE_TO
                                FROM
                                    PR_PAID_LEAVE_ENTRY AS PLE
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON PLE.EMPLOYEE_ID = SP.STAFF_ID
                                        INNER JOIN
                                    SUP_LEAVE_TYPE AS SS ON SS.LEAVE_TYPE_ID = PLE.LEAVE_TYPE_ID
                                WHERE
                                    PLE.IS_DELETED != 1
                                        AND PAID_LEAVE_ID = ?PAID_LEAVE_ID;";
                        break;
                    }
                case PayrollSQLCommads.SavePaidleaveentry:
                    {
                        query = @"INSERT INTO PR_PAID_LEAVE_ENTRY
                                    (EMPLOYEE_ID, LEAVE_TYPE_ID, 
                                    DATE_FROM, 
                                    DATE_TO, 
                                     ACADEMIC_YEAR) 
                                    VALUES(?EMPLOYEE_ID, ?LEAVE_TYPE_ID, ?DATE_FROM, ?DATE_TO,?AC_YEAR);";
                        break;
                    }
                case PayrollSQLCommads.UpdatePaidleaveentry:
                    {
                        query = @"UPDATE PR_PAID_LEAVE_ENTRY 
                                SET 
                                    EMPLOYEE_ID =?EMPLOYEE_ID,
                                    LEAVE_TYPE_ID = ?LEAVE_TYPE_ID,
                                    DATE_FROM = ?DATE_FROM,
                                    DATE_TO =?DATE_TO
                                WHERE
                                    PAID_LEAVE_ID = ?PAID_LEAVE_ID;";
                        break;
                    }
                case PayrollSQLCommads.DeletePaidleaveentry:
                    {
                        query = @"UPDATE PR_PAID_LEAVE_ENTRY 
                                SET 
                                   IS_DELETED=1
                                WHERE
                                    PAID_LEAVE_ID = ?PAID_LEAVE_ID;";
                        break;
                    }
                case PayrollSQLCommads.FetchIncrementType:
                    {
                        query = @"SELECT 
                                SALARY_INCREMENT_TYPE_ID,
                                INCREMENT_TYPE_NAME,
                                AMOUNT,
                                ACADEMIC_YEAR
                            FROM
                                PR_SUP_SALARY_INCREMENT_TYPE
                            WHERE
                                IS_DELETED != 1 AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case PayrollSQLCommads.FetchIncrementTypeById:
                    {
                        query = @"SELECT 
                                    SALARY_INCREMENT_TYPE_ID,
                                    INCREMENT_TYPE_NAME,
                                    AMOUNT,
                                    ACADEMIC_YEAR
                                FROM
                                    PR_SUP_SALARY_INCREMENT_TYPE
                                WHERE
                                    IS_DELETED != 1 AND SALARY_INCREMENT_TYPE_ID=?SALARY_INCREMENT_TYPE_ID;";
                        break;
                    }
                case PayrollSQLCommads.SaveIncrementType:
                    {
                        query = @"INSERT INTO PR_SUP_SALARY_INCREMENT_TYPE
                                (INCREMENT_TYPE_NAME
                                ,AMOUNT,
                                 ACADEMIC_YEAR) 
                                VALUES(?INCREMENT_TYPE_NAME, 
                                ?AMOUNT, ?AC_YEAR);";
                        break;
                    }
                case PayrollSQLCommads.UpdateIncrementType:
                    {
                        query = @"UPDATE PR_SUP_SALARY_INCREMENT_TYPE 
                                SET 
                                    INCREMENT_TYPE_NAME =?INCREMENT_TYPE_NAME,
                                    AMOUNT = ?AMOUNT
                                WHERE
                                    SALARY_INCREMENT_TYPE_ID =?SALARY_INCREMENT_TYPE_ID;";
                        break;
                    }
                case PayrollSQLCommads.DeleteSalaryIncrementType:
                    {
                        query = @"UPDATE PR_SUP_SALARY_INCREMENT_TYPE 
                                    SET 
                                       IS_DELETED=1
                                    WHERE
                                        SALARY_INCREMENT_TYPE_ID =?SALARY_INCREMENT_TYPE_ID;";
                        break;
                    }
                case PayrollSQLCommads.FetchIncrementDetails:
                    {
                        query = @"SELECT 
                                SID.INCREMENT_ID,
                                SID.SALARY_INCREMENT_TYPE_ID,
                                SIT.INCREMENT_TYPE_NAME,
                                SID.FROM_YEAR,
                                SID.TO_YEAR,
                                SID.TOTAL_YEAR,
                                SID.AMOUNT
                            FROM
                                PR_SALARY_INCREMENT_DETAILS AS SID
                                    INNER JOIN
                                PR_SUP_SALARY_INCREMENT_TYPE AS SIT ON SID.SALARY_INCREMENT_TYPE_ID = SIT.SALARY_INCREMENT_TYPE_ID
                            WHERE
                                SID.IS_DELETED != 1 AND SID.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case PayrollSQLCommads.FetchIncrementDetailsById:
                    {
                        query = @"SELECT 
                                    SID.INCREMENT_ID,
                                    SID.SALARY_INCREMENT_TYPE_ID,
                                    SIT.INCREMENT_TYPE_NAME,
                                    SID.FROM_YEAR,
                                    SID.TO_YEAR,
                                    SID.TOTAL_YEAR,
                                    SID.AMOUNT
                                FROM
                                    PR_SALARY_INCREMENT_DETAILS AS SID
                                        INNER JOIN
                                    PR_SUP_SALARY_INCREMENT_TYPE AS SIT ON SID.SALARY_INCREMENT_TYPE_ID = SIT.SALARY_INCREMENT_TYPE_ID
                                WHERE
                                    SID.IS_DELETED != 1
                                        AND SID.INCREMENT_ID = ?INCREMENT_ID;";
                        break;
                    }
                case PayrollSQLCommads.SaveIncrementDetails:
                    {
                        query = @"INSERT INTO PR_SALARY_INCREMENT_DETAILS
                                (SALARY_INCREMENT_TYPE_ID, 
                                FROM_YEAR, 
                                TO_YEAR,
                                 TOTAL_YEAR, 
                                 AMOUNT,
                                 ACADEMIC_YEAR) 
                                VALUES(?SALARY_INCREMENT_TYPE_ID, 
                                ?FROM_YEAR, 
                                ?TO_YEAR, ?TOTAL_YEAR,
                                 ?AMOUNT
                                , ?AC_YEAR);";
                        break;
                    }
                case PayrollSQLCommads.UpdateIncrementDetails:
                    {
                        query = @"UPDATE PR_SALARY_INCREMENT_DETAILS 
                                    SET 
                                        SALARY_INCREMENT_TYPE_ID =?SALARY_INCREMENT_TYPE_ID,
                                        FROM_YEAR = ?FROM_YEAR,
                                        TO_YEAR =?TO_YEAR,
                                        TOTAL_YEAR =?TOTAL_YEAR,
                                        AMOUNT = ?AMOUNT
                                    WHERE
                                        INCREMENT_ID = ?INCREMENT_ID;";
                        break;
                    }
                case PayrollSQLCommads.DeleteSalaryIncrementDetails:
                    {
                        query = @"UPDATE PR_SALARY_INCREMENT_DETAILS 
                                    SET 
                                       IS_DELETED=1
                                    WHERE
                                        INCREMENT_ID = ?INCREMENT_ID;";
                        break;
                    }
                case PayrollSQLCommads.FetchPFComponents:
                    {
                        query = @"SELECT 
                                PFC.PF_COMPONENT_ID,
                                PFC.PF_COMPONENT_NAME,
                                PFC.NAME_IN_PAYSLIP,
                                PFC.SALARY_COMPONENT_TYPE,
                                SCT.SALARY_COMPONENT_NAME
                            FROM
                                PR_PF_COMPONENTS AS PFC
                                    INNER JOIN
                                SUP_SALARY_COMPONENT_TYPE AS SCT ON SCT.SALARY_COMPONENT_TYPE_ID = PFC.SALARY_COMPONENT_TYPE
                            WHERE
                                PFC.IS_DELETED != 1;";
                        break;
                    }
                case PayrollSQLCommads.FetchPFComponentsById:
                    {
                        query = @"SELECT 
                                PFC.PF_COMPONENT_ID,
                                PFC.PF_COMPONENT_NAME,             
                                PFC.SALARY_COMPONENT_TYPE,
                                PFC.NAME_IN_PAYSLIP
                                
                            FROM
                                PR_PF_COMPONENTS AS PFC
       
                            WHERE
                                PFC.PF_COMPONENT_ID = ?PF_COMPONENT_ID;";
                        break;
                    }
                case PayrollSQLCommads.SavePFComponents:
                    {
                        query = @"INSERT INTO PR_PF_COMPONENTS
                                (PF_COMPONENT_NAME,ACADEMIC_YEAR,SALARY_COMPONENT_TYPE,NAME_IN_PAYSLIP) 
                                 VALUES(?PF_COMPONENT_NAME,?AC_YEAR,?SALARY_COMPONENT_TYPE,?NAME_IN_PAYSLIP);";
                        break;
                    }
                case PayrollSQLCommads.UpdatePFComponents:
                    {
                        query = @"UPDATE PR_PF_COMPONENTS 
                                SET 
                                    PF_COMPONENT_NAME =?PF_COMPONENT_NAME,                                 
                                    SALARY_COMPONENT_TYPE = ?SALARY_COMPONENT_TYPE,
                                    NAME_IN_PAYSLIP = ?NAME_IN_PAYSLIP
                                WHERE
                                    PF_COMPONENT_ID = ?PF_COMPONENT_ID;";
                        break;
                    }
                case PayrollSQLCommads.DeletePFComponents:
                    {
                        query = @"UPDATE PR_PF_COMPONENTS 
                                SET 
                                   IS_DELETED=1
                                WHERE
                                    PF_COMPONENT_ID = ?PF_COMPONENT_ID;";
                        break;
                    }
                case PayrollSQLCommads.FetchTemplatePFComponentByTemplateId:
                    {
                        query = @"SELECT 
                                    PFC.TEMPLATE_PF_COMPONENT_ID,
                                    PFC.TEMPLATE_ID,
                                    PSTD.TEMPLATE_NAME,
                                    PFC.PF_COMPONENT_ID,
                                    PPC.PF_COMPONENT_NAME,
                                    IFNULL(SC.COMPONENT_NAME,
                                            IFNULL(PC.PF_COMPONENT_NAME, '')) AS FIRST_COMPONENT,
                                    IFNULL(SC1.COMPONENT_NAME,
                                            IFNULL(PC1.PF_COMPONENT_NAME, '')) AS SECOND_COMPONENT,
                                    PFC.IS_FIRST_PF_COMPONENT,
                                    PFC.IS_SECOND_PF_COMPONENT,
                                    PFC.OPERATORS,
                                    PFC.PERCENTAGE
                                FROM
                                    PR_TEMPLATE_PFCOMPONENT_DETAILS AS PFC
                                        INNER JOIN
                                    PR_PF_COMPONENTS AS PPC ON PPC.PF_COMPONENT_ID = PFC.PF_COMPONENT_ID
                                        INNER JOIN
                                    PR_SALARY_TEMPLATE_DETAILS AS PSTD ON PSTD.TEMPLATE_ID = PFC.TEMPLATE_ID
                                        LEFT JOIN
                                    PR_SALARY_COMPONENT_DETAILS AS SC ON SC.COMPONENT_ID = PFC.FIRST_COMPONENT
                                        AND PFC.IS_FIRST_PF_COMPONENT != 1
                                        LEFT JOIN
                                    PR_SALARY_COMPONENT_DETAILS AS SC1 ON SC1.COMPONENT_ID = PFC.SECOND_COMPONENT
                                        AND PFC.IS_SECOND_PF_COMPONENT != 1
                                        LEFT JOIN
                                    PR_PF_COMPONENTS AS PC ON PC.PF_COMPONENT_ID = PFC.FIRST_COMPONENT
                                        AND PFC.IS_FIRST_PF_COMPONENT != 0
                                        LEFT JOIN
                                    PR_PF_COMPONENTS AS PC1 ON PC1.PF_COMPONENT_ID = PFC.SECOND_COMPONENT
                                        AND PFC.IS_SECOND_PF_COMPONENT != 0
                                WHERE
                                    PFC.ACADEMIC_YEAR = ?AC_YEAR AND PFC.TEMPLATE_ID=?TEMPLATE_ID
                                        AND PFC.IS_DELETED != 1;";
                        break;
                    }
                case PayrollSQLCommads.FetchTemplatePFComponentsById:
                    {
                        query = @"SELECT 
                                    PFC.TEMPLATE_PF_COMPONENT_ID,
                                    PFC.TEMPLATE_ID,
                                    PSTD.TEMPLATE_NAME,
                                    PFC.PF_COMPONENT_ID,
                                    PPC.PF_COMPONENT_NAME,
                                    PFC.FIRST_COMPONENT,
                                    PFC.SECOND_COMPONENT,
                                    PFC.IS_FIRST_PF_COMPONENT,
                                    PFC.IS_SECOND_PF_COMPONENT,
                                    PFC.OPERATORS,
                                    PFC.PERCENTAGE,
                                    PFC.FIXED_AMOUNT,
                                    PFC.PERCENTAGE_AMOUNT
                                FROM
                                    PR_TEMPLATE_PFCOMPONENT_DETAILS AS PFC
                                        INNER JOIN
                                    PR_PF_COMPONENTS AS PPC ON PPC.PF_COMPONENT_ID = PFC.PF_COMPONENT_ID
                                        INNER JOIN
                                    PR_SALARY_TEMPLATE_DETAILS AS PSTD ON PSTD.TEMPLATE_ID = PFC.TEMPLATE_ID
                                        LEFT JOIN
                                    PR_SALARY_COMPONENT_DETAILS AS SC ON SC.COMPONENT_ID = PFC.FIRST_COMPONENT
                                        AND PFC.IS_FIRST_PF_COMPONENT != 1
                                        LEFT JOIN
                                    PR_SALARY_COMPONENT_DETAILS AS SC1 ON SC1.COMPONENT_ID = PFC.SECOND_COMPONENT
                                        AND PFC.IS_SECOND_PF_COMPONENT != 1
                                        LEFT JOIN
                                    PR_PF_COMPONENTS AS PC ON PC.PF_COMPONENT_ID = PFC.FIRST_COMPONENT
                                        AND PFC.IS_FIRST_PF_COMPONENT != 0
                                        LEFT JOIN
                                    PR_PF_COMPONENTS AS PC1 ON PC1.PF_COMPONENT_ID = PFC.SECOND_COMPONENT
                                        AND PFC.IS_SECOND_PF_COMPONENT != 0
                                WHERE
                                    PFC.TEMPLATE_PF_COMPONENT_ID = ?TEMPLATE_PF_COMPONENT_ID
                                        AND PFC.IS_DELETED != 1;";
                        break;
                    }
                case PayrollSQLCommads.SaveTemplatePFComponents:
                    {
                        query = @"INSERT INTO PR_TEMPLATE_PFCOMPONENT_DETAILS
                                (TEMPLATE_ID,PF_COMPONENT_ID, FIRST_COMPONENT, 
								 IS_FIRST_PF_COMPONENT,
                                 OPERATORS, SECOND_COMPONENT, IS_SECOND_PF_COMPONENT, 
                                 PERCENTAGE, ACADEMIC_YEAR,FIXED_AMOUNT,PERCENTAGE_AMOUNT) 
                                 VALUES(?TEMPLATE_ID,?PF_COMPONENT_ID, ?FIRST_COMPONENT, 
                                 ?IS_FIRST_PF_COMPONENT, ?OPERATORS, 
                                 ?SECOND_COMPONENT, ?IS_SECOND_PF_COMPONENT,
                                 ?PERCENTAGE, ?AC_YEAR,?FIXED_AMOUNT,?PERCENTAGE_AMOUNT);";
                        break;
                    }
                case PayrollSQLCommads.UpdateTemplatePFComponents:
                    {
                        query = @"UPDATE PR_TEMPLATE_PFCOMPONENT_DETAILS 
                                SET 
									PF_COMPONENT_ID=?PF_COMPONENT_ID,
                                    FIRST_COMPONENT = ?FIRST_COMPONENT,
                                    IS_FIRST_PF_COMPONENT = ?IS_FIRST_PF_COMPONENT,
                                    OPERATORS =?OPERATORS,
                                    SECOND_COMPONENT =?SECOND_COMPONENT,
                                    IS_SECOND_PF_COMPONENT = ?IS_SECOND_PF_COMPONENT,
                                    PERCENTAGE = ?PERCENTAGE,
                                    FIXED_AMOUNT=?FIXED_AMOUNT,
                                    PERCENTAGE_AMOUNT=?PERCENTAGE_AMOUNT
                                    
                                WHERE
                                    TEMPLATE_PF_COMPONENT_ID = ?TEMPLATE_PF_COMPONENT_ID;";
                        break;
                    }
                case PayrollSQLCommads.DeleteTemplatePFComponents:
                    {
                        query = @"UPDATE PR_TEMPLATE_PFCOMPONENT_DETAILS 
                                    SET 
                                       IS_DELETED=1
                                    WHERE
                                        TEMPLATE_PF_COMPONENT_ID = ?TEMPLATE_PF_COMPONENT_ID;";
                        break;
                    }
                case PayrollSQLCommads.SetActiveandDeactiveTemplatePFComponents:
                    {
                        query = @"UPDATE PR_TEMPLATE_PFCOMPONENT_DETAILS 
                                    SET 
                                       IS_ACTIVE=?IS_ACTIVE
                                    WHERE
                                        TEMPLATE_PF_COMPONENT_ID = ?TEMPLATE_PF_COMPONENT_ID;";
                        break;
                    }
                case PayrollSQLCommads.FetchTemplateIncrements:
                    {
                        query = @"SELECT 
                                STI.TEMPLATE_INCREMENT_ID,
                                STI.TEMPLATE_ID,
                                STI.SALARY_INCREMENT_TYPE_ID,
                                STI.COMPONENT_ID,
                                ST.TEMPLATE_NAME,
                                SIT.INCREMENT_TYPE_NAME,
                                SCD.COMPONENT_NAME
                            FROM
                                PR_SALARY_TEMPLATE_INCREMENT AS STI
                                    INNER JOIN
                                PR_SALARY_TEMPLATE_DETAILS AS ST ON ST.TEMPLATE_ID = STI.TEMPLATE_ID
                                    INNER JOIN
                                PR_SUP_SALARY_INCREMENT_TYPE AS SIT ON SIT.SALARY_INCREMENT_TYPE_ID = STI.SALARY_INCREMENT_TYPE_ID
                                    INNER JOIN
                                PR_SALARY_COMPONENT_DETAILS AS SCD ON SCD.COMPONENT_ID = STI.COMPONENT_ID
                            WHERE
                                STI.IS_DELETED != 1
                                    AND STI.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;

                    }
                case PayrollSQLCommads.FetchTemplateIncrementsById:
                    {
                        query = @"SELECT 
                                    STI.TEMPLATE_INCREMENT_ID,
                                    STI.TEMPLATE_ID,
                                    STI.SALARY_INCREMENT_TYPE_ID,
                                    STI.COMPONENT_ID,
                                    ST.TEMPLATE_NAME,
                                    SIT.INCREMENT_TYPE_NAME,
                                    SCD.COMPONENT_NAME
                                FROM
                                    PR_SALARY_TEMPLATE_INCREMENT AS STI
                                        INNER JOIN
                                    PR_SALARY_TEMPLATE_DETAILS AS ST ON ST.TEMPLATE_ID = STI.TEMPLATE_ID
                                        INNER JOIN
                                    PR_SUP_SALARY_INCREMENT_TYPE AS SIT ON SIT.SALARY_INCREMENT_TYPE_ID = STI.SALARY_INCREMENT_TYPE_ID
                                        INNER JOIN
                                    PR_SALARY_COMPONENT_DETAILS AS SCD ON SCD.COMPONENT_ID = STI.COMPONENT_ID
                                WHERE
                                    STI.IS_DELETED != 1
                                        AND STI.TEMPLATE_INCREMENT_ID = ?TEMPLATE_INCREMENT_ID;";
                        break;

                    }
                case PayrollSQLCommads.SaveTemplateIncrements:
                    {
                        query = @"INSERT INTO PR_SALARY_TEMPLATE_INCREMENT
                                (TEMPLATE_ID, SALARY_INCREMENT_TYPE_ID, 
                                COMPONENT_ID, ACADEMIC_YEAR) 
                                VALUES(?TEMPLATE_ID, ?SALARY_INCREMENT_TYPE_ID,
                                 ?COMPONENT_ID, ?AC_YEAR);";
                        break;

                    }
                case PayrollSQLCommads.UpdateTemplateIncrements:
                    {
                        query = @"UPDATE PR_SALARY_TEMPLATE_INCREMENT 
                                SET 
                                    TEMPLATE_ID =?TEMPLATE_ID,
                                    SALARY_INCREMENT_TYPE_ID =?SALARY_INCREMENT_TYPE_ID,
                                    COMPONENT_ID =?COMPONENT_ID
                                WHERE
                                    TEMPLATE_INCREMENT_ID =?TEMPLATE_INCREMENT_ID;";
                        break;

                    }
                case PayrollSQLCommads.DeleteTemplateIncrements:
                    {
                        query = @"UPDATE PR_SALARY_TEMPLATE_INCREMENT 
                                SET 
                                    IS_DELETED=1
                                WHERE
                                    TEMPLATE_INCREMENT_ID =?TEMPLATE_INCREMENT_ID;";
                        break;

                    }
                case PayrollSQLCommads.FetchEmployees:
                    {
                        query = @"SELECT 
                                SP.STAFF_ID,
                                SP.STAFF_CODE,
                                SP.FIRST_NAME,
                                DATE_FORMAT(SP.DATE_OF_JOIN, '%Y/%m/%d') AS DATE_OF_JOIN
                            FROM
                                STF_PERSONAL_INFO AS SP
                                    INNER JOIN
                                SUP_STAFF_CATEGORY AS SC ON SC.STF_CATEGORY_ID = SP.CATEGORY
                                    INNER JOIN
                                PR_EMPOLYEE_SALARY_TEMPLATE AS EST ON EST.EMPLOYEE_ID = SP.STAFF_ID
                                    AND EST.ACADEMIC_YEAR = ?AC_YEAR
                            WHERE
                                SP.CATEGORY = ?CATEGORY AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1;";
                        break;
                    }
                case PayrollSQLCommads.FetchAllemployeesalarytemplates:
                    {
                        query = @"SELECT 
                                    EMP_SALARY_TEMP_ID, TEMPLATE_ID, EMPLOYEE_ID, ACADEMIC_YEAR
                                FROM
                                    PR_EMPOLYEE_SALARY_TEMPLATE
                                WHERE
                                    IS_DELETED != 1 AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case PayrollSQLCommads.FetchEmpolyeeAttendanceById:
                    {
                        //query = @"SELECT 
                        //        STF_ATT_ID,
                        //        STAFF_ID,
                        //        DATE_FORMAT(DATE, '%Y/%m/%d') AS DATE,
                        //        TIME,
                        //        SESSION
                        //    FROM
                        //        STAFF_ATTENDANCE_?AC_YEAR
                        //    WHERE
                        //        IS_DELETED != 1 AND STAFF_ID = ?STAFF_ID;";
                        query = @"SELECT 
                                SA.STF_ATT_ID,
                                SA.STAFF_ID,
                                DATE_FORMAT(SA.DATE, '%Y/%m/%d') AS DATE
                            FROM
                                STAFF_ATTENDANCE AS SA
                                    INNER JOIN
                                CALENDER AS CAL ON SA.DATE = DAY_ORDER_DATE
                            WHERE
                                SA.IS_DELETED != 1 AND SA.STAFF_ID = ?STAFF_ID
                                    AND SA.ACADEMIC_YEAR = ?AC_YEAR
                                    AND MONTH(DATE) = ?MONTH
                                    AND CAL.IS_HOLIDAY != 1;";

                        break;
                    }
                case PayrollSQLCommads.FetchLeaveSettingByCategoryId:
                    {
                        query = @"SELECT 
                                STF_LEAVE_SETTING_ID,
                                STAFF_CATEGORY,
                                LEAVE_TYPE,
                                M_MAX_DAYS,
                                Y_MAX_DAYS,
                                ACADEMIC_YEAR
                            FROM
                                PR_LEAVE_SETTING
                            WHERE
                                ACADEMIC_YEAR = ?AC_YEAR AND IS_DELETED != 1
                                    AND STAFF_CATEGORY = ?STAFF_CATEGORY;";
                        break;
                    }
                case PayrollSQLCommads.Fetchtemplatesalarycomponents:
                    {
                        query = @"SELECT 
	                                SCD.COMPONENT_ID,
                                    TCD.CTEMPLATE_ID,
                                    TCD.TEMPLATE_ID,
                                    TCD.COMPONENTT_ID,
                                    SCD.COMPONENT_NAME,
                                    TCD.FLAT_AMOUNT,
                                    TCD.PERCENTAGE,
                                    TCD.PERCENTAGE_COMPONENT_ID,
                                    SCD.SALARY_COMPONENT_TYPE,
                                    TCD.IS_MANUAL,
                                    SCD.NAME_IN_PAYSLIP,
                                    TCD.IS_GROSS_PAY
                                FROM
                                    PR_SALARY_TEMPLATE_COMPONENT_DETAILS AS TCD
                                        INNER JOIN
                                    PR_SALARY_COMPONENT_DETAILS AS SCD ON SCD.COMPONENT_ID = TCD.COMPONENTT_ID
                                WHERE
                                    TCD.TEMPLATE_ID = ?TEMPLATE_ID;";
                        break;
                    }
                case PayrollSQLCommads.Fetchtemplatepfcomponents:
                    {
                        query = @"SELECT 
                                TPD.TEMPLATE_PF_COMPONENT_ID,
                                TPD.TEMPLATE_ID,
                                TPD.PF_COMPONENT_ID,
                                PC.PF_COMPONENT_NAME,
                                TPD.FIRST_COMPONENT,
                                TPD.IS_FIRST_PF_COMPONENT,
                                TPD.OPERATORS,
                                TPD.SECOND_COMPONENT,
                                TPD.IS_SECOND_PF_COMPONENT,
                                TPD.PERCENTAGE,
                                PC.SALARY_COMPONENT_TYPE,
                                PC.NAME_IN_PAYSLIP,
                                TPD.FIXED_AMOUNT,
                                TPD.PERCENTAGE_AMOUNT
                            FROM
                                PR_TEMPLATE_PFCOMPONENT_DETAILS AS TPD
                                    INNER JOIN
                                PR_PF_COMPONENTS AS PC ON PC.PF_COMPONENT_ID = TPD.PF_COMPONENT_ID
                            WHERE
                                TPD.IS_DELETED != 1
                                    AND TPD.TEMPLATE_ID = ?TEMPLATE_ID;";
                        break;
                    }
                case PayrollSQLCommads.Fetchsalaryincrementtemplate:
                    {
                        query = @"SELECT 
                                    TEMPLATE_INCREMENT_ID,
                                    TEMPLATE_ID,
                                    SALARY_INCREMENT_TYPE_ID,
                                    COMPONENT_ID
                                FROM
                                    PR_SALARY_TEMPLATE_INCREMENT
                                WHERE
                                    IS_DELETED != 1 AND TEMPLATE_ID = ?TEMPLATE_ID;";
                        break;
                    }
                case PayrollSQLCommads.FetchEmpolyeeIncrementDetails:
                    {
                        query = @"SELECT 
                                SID.INCREMENT_ID,
                                SID.SALARY_INCREMENT_TYPE_ID,
                                SID.FROM_YEAR,
                                SID.TO_YEAR,
                                SID.TOTAL_YEAR,
                                SID.AMOUNT
                            FROM
                                PR_SALARY_INCREMENT_DETAILS AS SID
                            WHERE
                                SID.IS_DELETED != 1
                                    AND SID.SALARY_INCREMENT_TYPE_ID = ?SALARY_INCREMENT_TYPE_ID
                                    AND (SELECT 
                                        TIMESTAMPDIFF( YEAR, DATE_OF_JOIN, now()) AS COUNT
                                    FROM
                                        STF_PERSONAL_INFO
                                    WHERE
                                        STAFF_ID =?EMPLOYEE_ID) BETWEEN FROM_YEAR AND TO_YEAR;";
                        break;
                    }
                case PayrollSQLCommads.FetchPaidLeaveById:
                    {
                        query = @"SELECT 
                                PAID_LEAVE_ID,
                                EMPLOYEE_ID,
                                LEAVE_TYPE_ID,
                                DATE_FROM,
                                DATE_TO,
                                ENTRY_DATE,
                                ACADEMIC_YEAR
                            FROM
                                PR_PAID_LEAVE_ENTRY
                            WHERE
                                EMPLOYEE_ID = ?EMPLOYEE_ID
                                    AND ?MONTH BETWEEN MONTH(DATE_FROM) AND MONTH(DATE_TO);";
                        break;
                    }
                case PayrollSQLCommads.FetchEmployeeAttendanceByDate:
                    {
                        //query = @"SELECT 
                        //            STF_ATT_ID,
                        //            STAFF_ID,
                        //            DATE_FORMAT(DATE, '%Y/%m/%d') AS DATE,
                        //            TIME,
                        //            SESSION,
                        //            ATT_TYPE
                        //        FROM
                        //            STAFF_ATTENDANCE_?AC_YEAR
                        //        WHERE
                        //            DATE BETWEEN ?DATE_FROM AND ?DATE_TO
                        //                AND IS_DELETED != 1;";
                        query = @"";
                       
                        break;
                    }
                case PayrollSQLCommads.FetchEmpolyeeAttendanceByIdandMonth:
                    {
                        query = @"SELECT 
                                STF_ATT_ID,
                                STAFF_ID,
                                DATE_FORMAT(DATE, '%Y/%m/%d') AS DATE,
                                TIME,
                                SESSION
                            FROM
                                STAFF_ATTENDANCE_?AC_YEAR
                            WHERE
                                IS_DELETED != 1 AND STAFF_ID = ?STAFF_ID
                                    AND MONTH(DATE) = ?MONTH;";
                        break;
                    }

                case PayrollSQLCommads.FetchTemplatesSalaryComponent:
                    {
                        //query = @"SELECT 
                        //        TCD.CTEMPLATE_ID,
                        //        TCD.TEMPLATE_ID,
                        //        TD.TEMPLATE_NAME,
                        //        TCD.COMPONENTT_ID,
                        //        SCD.COMPONENT_NAME,
                        //        SCD.NAME_IN_PAYSLIP,
                        //        SCD.PERCENTAGE,
                        //        SCD.FLAT_AMOUNT,
                        //        SCCD.COMPONENT_NAME AS PERCENTAGE_COMPONENT_ID,
                        //        SCT.SALARY_COMPONENT_NAME
                        //    FROM
                        //        PR_SALARY_TEMPLATE_COMPONENT_DETAILS AS TCD
                        //            INNER JOIN
                        //        PR_SALARY_COMPONENT_DETAILS AS SCD ON SCD.COMPONENT_ID = TCD.COMPONENTT_ID
                        //            INNER JOIN
                        //        SUP_SALARY_COMPONENT_TYPE AS SCT ON SCT.SALARY_COMPONENT_TYPE_ID = SCD.SALARY_COMPONENT_TYPE
                        //            INNER JOIN
                        //        PR_SALARY_TEMPLATE_DETAILS AS TD ON TD.TEMPLATE_ID = TCD.TEMPLATE_ID
                        //            LEFT JOIN
                        //        PR_SALARY_COMPONENT_DETAILS AS SCCD ON SCCD.COMPONENT_ID = SCD.PERCENTAGE_COMPONENT_ID
                        //    WHERE
                        //        TCD.TEMPLATE_ID =?TEMPLATE_ID;";
                        query = @"SELECT 
                                TCD.CTEMPLATE_ID,
                                TCD.TEMPLATE_ID,
                                TCD.COMPONENTT_ID,
                                SCD.COMPONENT_NAME,
                                TD.TEMPLATE_NAME,
                                TCD.IS_ACTIVE,
                                TCD.ACADEMIC_YEAR,
                                TCD.PERCENTAGE_COMPONENT_ID,
                                TCD.PERCENTAGE,
                                TCD.FLAT_AMOUNT,
                                SCD1.COMPONENT_NAME AS PERCENTAGE_COMPONENT_NAME,
                                SCT.SALARY_COMPONENT_NAME
                            FROM
                                PR_SALARY_TEMPLATE_COMPONENT_DETAILS AS TCD
                                    INNER JOIN
                                PR_SALARY_COMPONENT_DETAILS AS SCD ON TCD.COMPONENTT_ID = SCD.COMPONENT_ID
                                    INNER JOIN
                                PR_SALARY_TEMPLATE_DETAILS AS TD ON TD.TEMPLATE_ID = TCD.TEMPLATE_ID
                                    LEFT JOIN
                                PR_SALARY_COMPONENT_DETAILS AS SCD1 ON SCD1.COMPONENT_ID = TCD.PERCENTAGE_COMPONENT_ID
                                    INNER JOIN
                                SUP_SALARY_COMPONENT_TYPE AS SCT ON SCT.SALARY_COMPONENT_TYPE_ID = SCD.SALARY_COMPONENT_TYPE
                            WHERE
                                TCD.IS_DELETED != 1
                                    AND TCD.TEMPLATE_ID = ?TEMPLATE_ID;";
                        break;
                    }
                case PayrollSQLCommads.FetchTemplatesPFComponent:
                    {
                        //query = @"SELECT 
                        //            TPFD.TEMPLATE_ID,
                        //            TD.TEMPLATE_NAME,
                        //            TPFD.PF_COMPONENT_ID,
                        //            PFC.PF_COMPONENT_NAME,
                        //            IFNULL(SC.COMPONENT_NAME,
                        //                    IFNULL(PC.PF_COMPONENT_NAME, '')) AS FIRST_COMPONENT,
                        //            IFNULL(SC1.COMPONENT_NAME,
                        //                    IFNULL(PC1.PF_COMPONENT_NAME, '')) AS SECOND_COMPONENT,
                        //            PFC.IS_FIRST_PF_COMPONENT,
                        //            PFC.IS_SECOND_PF_COMPONENT,
                        //            PFC.OPERATORS,
                        //            PFC.PERCENTAGE,
                        //            SCT.SALARY_COMPONENT_NAME,
                        //            PFC.NAME_IN_PAYSLIP
                        //        FROM
                        //            PR_TEMPLATE_PFCOMPONENT_DETAILS AS TPFD
                        //                INNER JOIN
                        //            PR_PF_COMPONENTS AS PFC ON PFC.PF_COMPONENT_ID = TPFD.PF_COMPONENT_ID
                        //                INNER JOIN
                        //            PR_SALARY_TEMPLATE_DETAILS AS TD ON TD.TEMPLATE_ID = TPFD.TEMPLATE_ID
                        //                INNER JOIN
                        //            SUP_SALARY_COMPONENT_TYPE AS SCT ON SCT.SALARY_COMPONENT_TYPE_ID = PFC.SALARY_COMPONENT_TYPE
                        //                LEFT JOIN
                        //            PR_SALARY_COMPONENT_DETAILS AS SC ON SC.COMPONENT_ID = PFC.FIRST_COMPONENT
                        //                AND PFC.IS_FIRST_PF_COMPONENT != 1
                        //                LEFT JOIN
                        //            PR_SALARY_COMPONENT_DETAILS AS SC1 ON SC1.COMPONENT_ID = PFC.SECOND_COMPONENT
                        //                AND PFC.IS_SECOND_PF_COMPONENT != 1
                        //                LEFT JOIN
                        //            PR_PF_COMPONENTS AS PC ON PC.PF_COMPONENT_ID = PFC.FIRST_COMPONENT
                        //                AND PFC.IS_FIRST_PF_COMPONENT != 0
                        //                LEFT JOIN
                        //            PR_PF_COMPONENTS AS PC1 ON PC1.PF_COMPONENT_ID = PFC.SECOND_COMPONENT
                        //                AND PFC.IS_SECOND_PF_COMPONENT != 0
                        //        WHERE
                        //            TPFD.TEMPLATE_ID = ?TEMPLATE_ID;";
                        query = @"SELECT 
                                    PFC.TEMPLATE_PF_COMPONENT_ID,
                                    PFC.TEMPLATE_ID,
                                    PSTD.TEMPLATE_NAME,
                                    PFC.PF_COMPONENT_ID,
                                    PPC.PF_COMPONENT_NAME,
                                    IFNULL(SC.COMPONENT_NAME,
                                            IFNULL(PC.PF_COMPONENT_NAME, '')) AS FIRST_COMPONENT,
                                    IFNULL(SC1.COMPONENT_NAME,
                                            IFNULL(PC1.PF_COMPONENT_NAME, '')) AS SECOND_COMPONENT,
                                    PFC.IS_FIRST_PF_COMPONENT,
                                    PFC.IS_SECOND_PF_COMPONENT,
                                    PFC.OPERATORS,
                                    PFC.PERCENTAGE
                                FROM
                                    PR_TEMPLATE_PFCOMPONENT_DETAILS AS PFC
                                        INNER JOIN
                                    PR_PF_COMPONENTS AS PPC ON PPC.PF_COMPONENT_ID = PFC.PF_COMPONENT_ID
                                        INNER JOIN
                                    PR_SALARY_TEMPLATE_DETAILS AS PSTD ON PSTD.TEMPLATE_ID = PFC.TEMPLATE_ID
                                        LEFT JOIN
                                    PR_SALARY_COMPONENT_DETAILS AS SC ON SC.COMPONENT_ID = PFC.FIRST_COMPONENT
                                        AND PFC.IS_FIRST_PF_COMPONENT != 1
                                        LEFT JOIN
                                    PR_SALARY_COMPONENT_DETAILS AS SC1 ON SC1.COMPONENT_ID = PFC.SECOND_COMPONENT
                                        AND PFC.IS_SECOND_PF_COMPONENT != 1
                                        LEFT JOIN
                                    PR_PF_COMPONENTS AS PC ON PC.PF_COMPONENT_ID = PFC.FIRST_COMPONENT
                                        AND PFC.IS_FIRST_PF_COMPONENT != 0
                                        LEFT JOIN
                                    PR_PF_COMPONENTS AS PC1 ON PC1.PF_COMPONENT_ID = PFC.SECOND_COMPONENT
                                        AND PFC.IS_SECOND_PF_COMPONENT != 0
                                WHERE
                                     PFC.TEMPLATE_ID=?TEMPLATE_ID
                                        AND PFC.IS_DELETED != 1;";
                        break;
                    }
                case PayrollSQLCommads.FetchEmployeeByCategoryId:
                    {
                        query = @"SELECT 
                                STAFF_ID, CONCAT(FIRST_NAME, '-', STAFF_CODE) AS FIRST_NAME
                            FROM
                                STF_PERSONAL_INFO AS SP
                                    INNER JOIN
                                PR_EMPOLYEE_SALARY_TEMPLATE AS EST ON SP.STAFF_ID = EST.EMPLOYEE_ID
                                    AND EST.ACADEMIC_YEAR = ?AC_YEAR
                            WHERE
                                SP.IS_DELETED != 1 AND SP.CATEGORY = ?CATEGORY;";
                        break;
                    }
                case PayrollSQLCommads.FetchLeaveTranscationBySettingId:
                    {
                        query = @"SELECT 
                                LEAVE_SETTING_ID,TRANSCATIOIN_ID
                            FROM
                                LM_TRANSACTION_DETAILS
                            WHERE
                                LEAVE_SETTING_ID = ?LEAVE_SETTING_ID AND CREDIT_DAYS IS NOT NULL;";
                        break;
                    }
                case PayrollSQLCommads.UpdateLeaveTranscations:
                    {
                        query = @"UPDATE LM_TRANSACTION_DETAILS 
                                    SET 
                                        CREDIT_DAYS = ?CREDIT_DAYS,
                                        IS_CARRIED_NEXT_YEAR=?IS_CARRIED_NEXT_YEAR
                                    WHERE
                                        TRANSCATIOIN_ID =?TRANSCATIOIN_ID;";
                            break;
                    }
                case PayrollSQLCommads.FetchEmployeeByCategoryIdandExperience:
                    {
                        query = @"SELECT 
                                STAFF_ID, CONCAT(FIRST_NAME, '-', STAFF_CODE) AS FIRST_NAME
                            FROM
                                STF_PERSONAL_INFO AS SP
                            WHERE
                                SP.IS_DELETED != 1 AND SP.CATEGORY = ?CATEGORY
                                    AND TIMESTAMPDIFF(YEAR, DATE_OF_JOIN, NOW()) BETWEEN ?EXP_YEAR_FROM  AND ?EXP_YEAR_TO;";
                        break;
                    }
                case PayrollSQLCommads.FetchStaffAttendanceByDate:
                    {
                        query = @"SELECT 
                                SA.STF_ATT_ID,
                                SA.STAFF_ID,
                                SP.FIRST_NAME,
                                SP.STAFF_CODE,
                                DATE_FORMAT(DATE, '%Y/%m/%d') AS DATE,
                                LAS.STATUS_ID,
                                LAS.STATUS_NAME,
                                DATE_FORMAT(TIME_IN, '%d/%m/%Y') AS TIME_IN,
                                DATE_FORMAT(TIME_OUT, '%d/%m/%Y') AS TIME_OUT,
                                TIME_FORMAT(TIME_IN, '%H:%i:%S') AS IN_TIME,
                                TIME_FORMAT(TIME_OUT, '%H:%i:%S') AS OUT_TIME
                            FROM
                                STAFF_ATTENDANCE AS SA
                                    INNER JOIN
                                LM_SUP_ATTENDANCE_STATUS AS LAS ON SA.STATUS = LAS.STATUS_ID
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = SA.STAFF_ID
                            WHERE
                                SA.IS_DELETED != 1
                                    AND DATE_FORMAT(DATE, '%Y-%m-%d') BETWEEN ?DATE_FROM AND ?DATE_TO;";
                        break;
                    }
                case PayrollSQLCommads.FetchlmLeaveStatus:
                    {
                        query = @"SELECT 
                                    STATUS_ID, STATUS_NAME
                                FROM
                                    LM_SUP_ATTENDANCE_STATUS
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case PayrollSQLCommads.FetchStaffAttendanceByStfAttId:
                    {
                        query = @"SELECT 
                                SA.STF_ATT_ID,
                                SA.STAFF_ID,
                                SP.FIRST_NAME,
                                SP.STAFF_CODE,
                                DATE_FORMAT(DATE, '%Y/%M/%D') AS DATE,
                                LAS.STATUS_ID,
                                LAS.STATUS_NAME,
                                DATE_FORMAT(TIME_IN, '%d/%m/%Y') AS TIME_IN,
                                DATE_FORMAT(TIME_OUT, '%d/%m/%Y') AS TIME_OUT,
                                TIME_FORMAT(TIME_IN, '%H:%i:%S') AS IN_TIME,
                                TIME_FORMAT(TIME_OUT, '%H:%i:%S') AS OUT_TIME
                            FROM
                                STAFF_ATTENDANCE AS SA
                                    INNER JOIN
                                LM_SUP_ATTENDANCE_STATUS AS LAS ON SA.STATUS = LAS.STATUS_ID
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = SA.STAFF_ID
                            WHERE
                                SA.IS_DELETED != 1
                                    AND SA.STF_ATT_ID = ?STF_ATT_ID;";
                        break;
                    }
                case PayrollSQLCommads.UpdateStaffAttendance:
                    {
                        query = @"UPDATE STAFF_ATTENDANCE 
                                SET 
                                    TIME_IN =?TIME_IN,
                                    TIME_OUT =?TIME_OUT,
                                    STATUS = ?STATUS_ID
                                WHERE
                                    STF_ATT_ID = ?STF_ATT_ID;";
                        break;
                    }
                case PayrollSQLCommads.Converttotimeformat:
                    {
                        query = @"SELECT time_format(?TIME,'%H:%i:%S') AS TIME;";
                        break;
                    }
                case PayrollSQLCommads.FetchEmpolyeeLeaveAppliedById:
                    {
                        query = @"SELECT 
                                SLD.LEAVE_APPLY_ID,
                                SLD.STAFF_ID,
                                SLD.FROM_DATE,
                                SLD.FROM_SESSION,
                                SLD.TO_DATE,
                                SLD.TO_SESSION,
                                SLD.NO_OF_DAYS
                            FROM
                                LM_STAFF_LEAVE_DETAILS AS SLD
                            WHERE
                                STAFF_ID = ?STAFF_ID AND APPROVAL_STATUS1 = ?APPROVAL_STATUS1
                                    AND APPROVAL_STATUS2 = ?APPROVAL_STATUS2
                                    AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case PayrollSQLCommads.FetchEmployeeincrementByMonthandyear:
                    {
                        query = @"SELECT 
                                    STAFF_ID AS EMPLOYEE_ID,
                                    FIRST_NAME,
                                    DATE_FORMAT(DATE_OF_JOIN,'%Y-%m-%d') AS DATE_OF_JOIN,
                                    CURRENT_COUNT,
                                    LAST_COUNT,
                                    EID.INCREMENT_DETAILS_ID,
                                    EID.STATUS,
                                    TEMPLATE_ID,
                                    SALARY_INCREMENT_TYPE_ID,
                                    TEMPLATE_NAME
                                FROM
                                    (SELECT 
                                        STAFF_ID,
                                            FIRST_NAME,
                                            DATE_OF_JOIN,
                                            CURRENT_COUNT,
                                            LAST_COUNT,
                                            TEMPLATE_ID,
                                            SALARY_INCREMENT_TYPE_ID,
                                            TEMPLATE_NAME
                                    FROM
                                        (SELECT 
                                        SP.STAFF_ID,
                                            SP.FIRST_NAME,
                                            SP.DATE_OF_JOIN,
                                            TIMESTAMPDIFF(YEAR, DATE_OF_JOIN, LAST_DAY(DATE(CONCAT_WS('-', ?YEAR, ?MONTH, 1)))) AS CURRENT_COUNT,
                                            TIMESTAMPDIFF(YEAR, DATE_OF_JOIN, LAST_DAY(DATE(CONCAT_WS('-', ?YEAR, ?MONTH, 1) - INTERVAL 1 MONTH))) AS LAST_COUNT,
                                            STI.TEMPLATE_ID,
                                            STI.SALARY_INCREMENT_TYPE_ID,
                                            ST.TEMPLATE_NAME
                                    FROM
                                        STF_PERSONAL_INFO AS SP
                                    INNER JOIN PR_EMPOLYEE_SALARY_TEMPLATE AS EST ON SP.STAFF_ID = EST.EMPLOYEE_ID
                                    INNER JOIN PR_SALARY_TEMPLATE_INCREMENT AS STI ON STI.TEMPLATE_ID = EST.TEMPLATE_ID
	                                INNER JOIN PR_SALARY_TEMPLATE_DETAILS AS ST ON ST.TEMPLATE_ID=STI.TEMPLATE_ID
                                    WHERE
                                        EST.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SP.CATEGORY = ?STAFF_CATEGORY) AS T
                                    WHERE
                                        T.CURRENT_COUNT != T.LAST_COUNT) AS T1
                                        LEFT JOIN
                                    PR_EMPLOYEE_INCREMENT_DETAILS AS EID ON EID.EMPLOYEE_ID = T1.STAFF_ID AND EID.ACADEMIC_YEAR = ?AC_YEAR;;";
                        break;
                    }
                case PayrollSQLCommads.SaveEmployeeIncrementDetails:
                    {
                        query = @"INSERT INTO PR_EMPLOYEE_INCREMENT_DETAILS(EMPLOYEE_ID,STATUS,EXPERIENCE_YEAR,ACADEMIC_YEAR) VALUES(?EMPLOYEE_ID,?STATUS,?EXPERIENCE_YEAR,?AC_YEAR);";
                        break;

                    }
                case PayrollSQLCommads.UpdateEmployeeIncrementDetails:
                    {
                        query = @"UPDATE PR_EMPLOYEE_INCREMENT_DETAILS 
                                    SET 
                                        STATUS = ?STATUS,EXPERIENCE_YEAR=?EXPERIENCE_YEAR
                                    WHERE
                                        INCREMENT_DETAILS_ID =?INCREMENT_DETAILS_ID;";
                        break;
                    }
                case PayrollSQLCommads.FetchIncrementDetailsByIncrementtypeIdandExperience:
                    {
                        query = @"SELECT 
                                    SID.INCREMENT_ID,
                                    SID.SALARY_INCREMENT_TYPE_ID,
                                    SID.FROM_YEAR,
                                    SID.TO_YEAR,
                                    SID.TOTAL_YEAR,
                                    SID.AMOUNT
                                FROM
                                    PR_SALARY_INCREMENT_DETAILS AS SID
                                WHERE
                                    SID.IS_DELETED != 1
                                        AND SID.SALARY_INCREMENT_TYPE_ID = ?SALARY_INCREMENT_TYPE_ID
                                        AND ?CURRENT_COUNT BETWEEN FROM_YEAR AND TO_YEAR;";
                        break;
                    }
                case PayrollSQLCommads.FetchEmpolyeeIncrementStatus:
                    {
                        query = @"SELECT 
                                INCREMENT_DETAILS_ID,
                                EMPLOYEE_ID,
                                DATE_FORMAT(APPROVAL_INCREMENT_DATE, '%Y-%m-%d') AS APPROVAL_INCREMENT_DATE,
                                STATUS,
                                ACADEMIC_YEAR,
                                EXPERIENCE_YEAR
                            FROM
                                PR_EMPLOYEE_INCREMENT_DETAILS
                            WHERE
                                EMPLOYEE_ID = ?EMPLOYEE_ID AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case PayrollSQLCommads.FetchEmpolyeeIncrementDetailsByExprience:
                    {
                        query = @"SELECT 
                                SID.INCREMENT_ID,
                                SID.SALARY_INCREMENT_TYPE_ID,
                                SID.FROM_YEAR,
                                SID.TO_YEAR,
                                SID.TOTAL_YEAR,
                                SID.AMOUNT
                            FROM
                                PR_SALARY_INCREMENT_DETAILS AS SID
                            WHERE
                                SID.IS_DELETED != 1
                                    AND SID.SALARY_INCREMENT_TYPE_ID = ?SALARY_INCREMENT_TYPE_ID
                                    AND ?EXPRIENCE BETWEEN FROM_YEAR AND TO_YEAR;";
                        break;
                    }
                case PayrollSQLCommads.FetchPayrollyears:
                    {
                        query = @"SELECT 
                                    YEAR_ID,
                                    YEAR,
                                    DATE_FORMAT(DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                    DATE_FORMAT(DATE_TO, '%d/%m/%Y') AS DATE_TO,
                                    ACTIVE_YEAR,
                                    NAME
                                FROM
                                    PR_PAYROLL_YEAR
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case PayrollSQLCommads.FetchPayrollyearById:
                    {
                        query = @"SELECT 
                                YEAR_ID,
                                YEAR,
                                DATE_FORMAT(DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                DATE_FORMAT(DATE_TO, '%d/%m/%Y') AS DATE_TO,
                                ACTIVE_YEAR,
                                NAME
                            FROM
                                PR_PAYROLL_YEAR
                            WHERE
                                IS_DELETED != 1 AND YEAR_ID = ?YEAR_ID;";
                        break;
                    }
                case PayrollSQLCommads.FetchSalaryTemplateComponenstByTemplateId:
                    {
                        query = @"SELECT 
                                SCD.COMPONENT_ID, SCD.COMPONENT_NAME
                            FROM
                                PR_SALARY_TEMPLATE_COMPONENT_DETAILS AS TCD
                                    INNER JOIN
                                PR_SALARY_COMPONENT_DETAILS AS SCD ON SCD.COMPONENT_ID = TCD.COMPONENTT_ID
                            WHERE
                                TCD.TEMPLATE_ID = ?TEMPLATE_ID
                                    AND TCD.IS_DELETED != 1;";
                        break;
                    }
                case PayrollSQLCommads.CheckIsIncrementExist:
                    {
                        query = @"SELECT 
                                INCREMENT_ID
                            FROM
                                PR_SALARY_INCREMENT_DETAILS
                            WHERE
                                SALARY_INCREMENT_TYPE_ID = ?SALARY_INCREMENT_TYPE_ID
                                    AND FROM_YEAR BETWEEN ?FROM_YEAR AND ?TO_YEAR
                                    OR TO_YEAR BETWEEN ?FROM_YEAR AND ?TO_YEAR
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case PayrollSQLCommads.FetchStaffWorkingtimedetails:
                    {
                        query = @"SELECT 
                                TD.STAFF_WORKING_TIME_ID,
                                TD.STAFF_CATEGORY,
                                SC.STF_CATEGORY,
                                TD.SHIFT,
                                SS.SHIFT_NAME,
                                TD.TIME_IN,
                                TD.TIME_OUT
                            FROM
                                LM_STAFF_WORKING_TIME_DETAILS AS TD
                                    INNER JOIN
                                SUP_STAFF_CATEGORY AS SC ON SC.STF_CATEGORY_ID = TD.STAFF_CATEGORY
                                    INNER JOIN
                                SUP_SHIFT AS SS ON SS.SHIFT_ID = TD.SHIFT
                            WHERE
                                TD.IS_DELETED != 1;";
                        break;
                    }
                case PayrollSQLCommads.FetchStaffWorkingdetailsById:
                    {
                        query = @"SELECT 
                                TD.STAFF_WORKING_TIME_ID,
                                TD.STAFF_CATEGORY,
                                SC.STF_CATEGORY,
                                TD.SHIFT,
                                SS.SHIFT_NAME,
                                TD.TIME_IN,
                                TD.TIME_OUT
                            FROM
                                LM_STAFF_WORKING_TIME_DETAILS AS TD
                                    INNER JOIN
                                SUP_STAFF_CATEGORY AS SC ON SC.STF_CATEGORY_ID = TD.STAFF_CATEGORY
                                    INNER JOIN
                                SUP_SHIFT AS SS ON SS.SHIFT_ID = TD.SHIFT
                            WHERE
                                TD.IS_DELETED != 1
                                    AND TD.STAFF_WORKING_TIME_ID = ?STAFF_WORKING_TIME_ID;";
                        break;
                    }
                case PayrollSQLCommads.SaveStaffWorkingtimes:
                    {
                        query = @"INSERT INTO 
                                LM_STAFF_WORKING_TIME_DETAILS
                                (STAFF_CATEGORY, SHIFT, TIME_IN, TIME_OUT, ACADEMIC_YEAR)
                                VALUES(?STAFF_CATEGORY, ?SHIFT, ?TIME_IN, ?TIME_OUT, ?AC_YEAR);";
                        break;
                    }
                case PayrollSQLCommads.UpdateStaffWorkingtimes:
                    {
                        query = @"UPDATE LM_STAFF_WORKING_TIME_DETAILS 
                                SET 
                                    STAFF_CATEGORY =?STAFF_CATEGORY,
                                    SHIFT =?SHIFT,
                                    TIME_IN = ?TIME_IN,
                                    TIME_OUT =?TIME_OUT
                                WHERE
                                    STAFF_WORKING_TIME_ID =?STAFF_WORKING_TIME_ID;";
                        break;
                    }
                case PayrollSQLCommads.DeleteStaffWorkingtimes:
                    {
                        query = @"UPDATE LM_STAFF_WORKING_TIME_DETAILS 
                                    SET 
  
                                        IS_DELETED =1
                                    WHERE
                                        STAFF_WORKING_TIME_ID =?STAFF_WORKING_TIME_ID;";
                        break;
                    }
                case PayrollSQLCommads.FetchLateComersByDateandShift:
                    {
                        query = @"SELECT 
                                    SA.STF_ATT_ID,
                                    SP.FIRST_NAME,
                                    SP.STAFF_CODE,
                                    DATE_FORMAT(SA.DATE, '%Y-%m-%d') AS DATE,
                                    DATE_FORMAT(SA.TIME_IN, '%Y-%m-%d') AS TIME_IN,
                                    DATE_FORMAT(SA.TIME_OUT, '%Y-%m-%d') AS TIME_OUT,
                                    TIME_FORMAT(TIME_FORMAT(TIME_IN, '%r'), '%h:%i:%s') AS IN_TIME,
                                    TIME_FORMAT(TIME_FORMAT(TIME_OUT, '%r'), '%h:%i:%s') AS OUT_TIME,
                                    SAS.STATUS_ID,
                                    SAS.STATUS_NAME
                                FROM
                                    STAFF_ATTENDANCE AS SA
                                        INNER JOIN
                                    LM_SUP_ATTENDANCE_STATUS AS SAS ON SAS.STATUS_ID = SA.STATUS
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SA.STAFF_ID = SP.STAFF_ID
                                WHERE
                                    SP.SHIFT = ?SHIFT
                                        AND DATE_FORMAT(SA.TIME_IN, '%Y-%m-%d') BETWEEN ?DATE_FROM AND ?DATE_TO
                                        AND CAST(TIME_IN AS TIME) > (SELECT 
                                            CAST(TD.TIME_IN AS TIME)
                                        FROM
                                            LM_STAFF_WORKING_TIME_DETAILS AS TD
                                        WHERE
                                            TD.SHIFT = SP.SHIFT
                                                AND TD.STAFF_CATEGORY = SP.CATEGORY
                                                AND TD.ACADEMIC_YEAR = ?AC_YEAR)
                                        OR CAST(TIME_OUT AS TIME) < (SELECT 
                                            CAST(TD.TIME_OUT AS TIME)
                                        FROM
                                            LM_STAFF_WORKING_TIME_DETAILS AS TD
                                        WHERE
                                            TD.SHIFT = SP.SHIFT
                                                AND TD.STAFF_CATEGORY = SP.CATEGORY
                                                AND TD.ACADEMIC_YEAR = ?AC_YEAR);";
                        break;
                    }
                case PayrollSQLCommads.FetchFaildtologoutlist:
                    {
                        query = @"SELECT 
                                    SA.STF_ATT_ID,
                                    SP.FIRST_NAME,
                                    SP.STAFF_CODE,
                                    DATE_FORMAT(SA.DATE, '%Y-%m-%d') AS DATE,
                                    DATE_FORMAT(SA.TIME_IN, '%Y-%m-%d') AS TIME_IN,
                                    DATE_FORMAT(SA.TIME_OUT, '%Y-%m-%d') AS TIME_OUT,
                                    TIME_FORMAT(TIME_FORMAT(TIME_IN, '%r'), '%h:%i:%s') AS IN_TIME,
                                    TIME_FORMAT(TIME_FORMAT(TIME_OUT, '%r'), '%h:%i:%s') AS OUT_TIME,
                                    SAS.STATUS_ID,
                                    SAS.STATUS_NAME
                                FROM
                                    STAFF_ATTENDANCE AS SA
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = SA.STAFF_ID
                                        INNER JOIN
                                    LM_SUP_ATTENDANCE_STATUS AS SAS ON SAS.STATUS_ID = SA.STATUS
                                WHERE
                                    DATE_FORMAT(SA.DATE, '%Y-%m-%d') BETWEEN ?DATE_FROM AND ?DATE_TO
                                        AND SA.IS_DELETED != 1
                                        AND SA.TIME_OUT IS  NULL;";
                        break;
                    }
                case PayrollSQLCommads.FetchAlltemplatesalarycomponents:
                    {
                        query = @"SELECT 
                                SCD.COMPONENT_ID,
                                TCD.CTEMPLATE_ID,
                                TCD.TEMPLATE_ID,
                                TCD.COMPONENTT_ID,
                                SCD.COMPONENT_NAME,
                                TCD.FLAT_AMOUNT,
                                TCD.PERCENTAGE,
                                TCD.PERCENTAGE_COMPONENT_ID,
                                SCD.SALARY_COMPONENT_TYPE,
                                TCD.IS_MANUAL,
                                SCD.NAME_IN_PAYSLIP,
                                TCD.IS_GROSS_PAY
                            FROM
                                PR_SALARY_TEMPLATE_COMPONENT_DETAILS AS TCD
                                    INNER JOIN
                                PR_SALARY_COMPONENT_DETAILS AS SCD ON SCD.COMPONENT_ID = TCD.COMPONENTT_ID
                            WHERE
                                TCD.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case PayrollSQLCommads.FetchAlltemplatepfcomponents:
                    {
                        query = @"SELECT 
                                TPD.TEMPLATE_PF_COMPONENT_ID,
                                TPD.TEMPLATE_ID,
                                TPD.PF_COMPONENT_ID,
                                PC.PF_COMPONENT_NAME,
                                TPD.FIRST_COMPONENT,
                                TPD.IS_FIRST_PF_COMPONENT,
                                TPD.OPERATORS,
                                TPD.SECOND_COMPONENT,
                                TPD.IS_SECOND_PF_COMPONENT,
                                TPD.PERCENTAGE,
                                PC.SALARY_COMPONENT_TYPE,
                                PC.NAME_IN_PAYSLIP
                            FROM
                                PR_TEMPLATE_PFCOMPONENT_DETAILS AS TPD
                                    INNER JOIN
                                PR_PF_COMPONENTS AS PC ON PC.PF_COMPONENT_ID = TPD.PF_COMPONENT_ID
                            WHERE
                                TPD.IS_DELETED != 1
                                    AND TPD.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case PayrollSQLCommads.FetchAllsalaryincrementtemplate:
                    {
                        query = @"SELECT 
                                    TEMPLATE_INCREMENT_ID,
                                    TEMPLATE_ID,
                                    SALARY_INCREMENT_TYPE_ID,
                                    COMPONENT_ID
                                FROM
                                    PR_SALARY_TEMPLATE_INCREMENT
                                WHERE
                                    IS_DELETED != 1 AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case PayrollSQLCommads.CheckIssalarycomponentExist:
                    {
                        query = @"SELECT 
                                SALARY_GENERATION_ID,
                                EMPLOYEE_ID,
                                COMPONENT_ID,
                                AMOUNT,
                                IS_PF_COMPONENT,
                                MONTH,
                                ACADEMIC_YEAR
                            FROM
                                PR_EMPLOYEE_SALARY_GENERATION
                            WHERE
                                EMPLOYEE_ID = ?EMPLOYEE_ID AND COMPONENT_ID = ?COMPONENT_ID
                                    AND IS_PF_COMPONENT = ?IS_PF_COMPONENT
                                    AND MONTH = ?MONTH
                                    AND ACADEMIC_YEAR = ?AC_YEAR
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case PayrollSQLCommads.SaveSalaryGeneration:
                    {
                        query = @"INSERT INTO 
                                PR_EMPLOYEE_SALARY_GENERATION
                                (EMPLOYEE_ID, COMPONENT_ID, AMOUNT, IS_PF_COMPONENT,
                                 MONTH, ACADEMIC_YEAR,LOSS_OF_PAY, NET_PAY,CTEMPLATE_ID) VALUES";
                        break;
                    }
                case PayrollSQLCommads.UpdateSalaryGeneration:
                    {
                        query = @"UPDATE PR_EMPLOYEE_SALARY_GENERATION 
                                SET 
                                    AMOUNT = ?AMOUNT,
                                    IS_PF_COMPONENT =?IS_PF_COMPONENT,
                                    MONTH =?MONTH,
                                    LOSS_OF_PAY =?LOSS_OF_PAY,
                                    NET_PAY =?NET_PAY
                                WHERE
                                    SALARY_GENERATION_ID = ?SALARY_GENERATION_ID;";
                        break;
                    }
                case PayrollSQLCommads.FetchEmployeeSalarygenerationBystaffIdandMonthyear:
                    {
                        query = @"SELECT 
                                T.EMPLOYEE_ID,
                                SCD.COMPONENT_ID,
                                AMOUNT,
                                LOSS_OF_PAY,
                                NET_PAY,
                                SCD.COMPONENT_NAME,
                                MONTH,
                                T.ACADEMIC_YEAR,
                                FIRST_NAME,
                                STAFF_CODE,
                                SCD.SALARY_COMPONENT_TYPE,
                                SCD.NAME_IN_PAYSLIP,
                                STCD.IS_GROSS_PAY,PHOTO
                            FROM
                                (SELECT 
                                    ESG.COMPONENT_ID,
                                        ESG.AMOUNT,
                                        ESG.LOSS_OF_PAY,
                                        ESG.NET_PAY,
                                        ESG.EMPLOYEE_ID,
                                        ESG.MONTH,
                                        ESG.ACADEMIC_YEAR,
                                        SP.FIRST_NAME,
                                        SP.STAFF_CODE,SP.PHOTO
                                FROM
                                    PR_EMPLOYEE_SALARY_GENERATION AS ESG
                                INNER JOIN STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = ESG.EMPLOYEE_ID
                                WHERE
                                    ESG.ACADEMIC_YEAR = ?AC_YEAR
                                        AND ESG.EMPLOYEE_ID IN (?EMPLOYEE_ID)
                                        AND ESG.MONTH = ?MONTH
                                        AND ESG.IS_PF_COMPONENT = 0) AS T
                                    INNER JOIN
                                PR_SALARY_COMPONENT_DETAILS AS SCD ON T.COMPONENT_ID = SCD.COMPONENT_ID
                                    INNER JOIN
                                PR_EMPOLYEE_SALARY_TEMPLATE AS EST ON EST.EMPLOYEE_ID = T.EMPLOYEE_ID
                                    INNER JOIN
                                PR_SALARY_TEMPLATE_COMPONENT_DETAILS AS STCD ON STCD.TEMPLATE_ID = EST.TEMPLATE_ID
                                    AND SCD.COMPONENT_ID = STCD.COMPONENTT_ID 
                            UNION SELECT 
                                T.EMPLOYEE_ID,
                                PFC.PF_COMPONENT_ID AS COMPONENT_ID,
                                AMOUNT,
                                LOSS_OF_PAY,
                                NET_PAY,
                                PFC.PF_COMPONENT_NAME AS COMPONENT_NAME,
                                MONTH,
                                T.ACADEMIC_YEAR,
                                FIRST_NAME,
                                STAFF_CODE,
                                PFC.SALARY_COMPONENT_TYPE,
                                PFC.NAME_IN_PAYSLIP,
                                '0' AS IS_GROSS_PAY,
                                PHOTO
                            FROM
                                (SELECT 
                                    ESG.COMPONENT_ID,
                                        ESG.AMOUNT,
                                        ESG.LOSS_OF_PAY,
                                        ESG.NET_PAY,
                                        ESG.EMPLOYEE_ID,
                                        ESG.MONTH,
                                        ESG.ACADEMIC_YEAR,
                                        SP.FIRST_NAME,
                                        SP.STAFF_CODE,SP.PHOTO
                                FROM
                                    PR_EMPLOYEE_SALARY_GENERATION AS ESG
                                INNER JOIN STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = ESG.EMPLOYEE_ID
                                WHERE
                                    ESG.ACADEMIC_YEAR = ?AC_YEAR
                                        AND ESG.EMPLOYEE_ID IN (?EMPLOYEE_ID)
                                        AND ESG.MONTH = ?MONTH
                                        AND ESG.IS_PF_COMPONENT = 1) AS T
                                    INNER JOIN
                                PR_PF_COMPONENTS AS PFC ON PFC.PF_COMPONENT_ID = T.COMPONENT_ID;";
                        break;
                    }
                case PayrollSQLCommads.CheckIsTemplateMapped:
                    {
                        query = @"SELECT 
                                STC.TEMPLATE_ID
                            FROM
                                PR_SALARY_TEMPLATE_COMPONENT_DETAILS AS STC
                            WHERE
                                STC.TEMPLATE_ID = ?TEMPLATE_ID
                                    AND STC.IS_DELETED != 1 
                            UNION ALL SELECT 
                                TPC.TEMPLATE_ID
                            FROM
                                PR_TEMPLATE_PFCOMPONENT_DETAILS AS TPC
                            WHERE
                                TPC.TEMPLATE_ID = ?TEMPLATE_ID
                                    AND TPC.IS_DELETED != 2;";
                        break;
                    }
                case PayrollSQLCommads.CheckIsComponentMapped:
                    {
                        query = @"SELECT 
                                STC.TEMPLATE_ID
                            FROM
                                PR_SALARY_TEMPLATE_COMPONENT_DETAILS AS STC
                            WHERE
                                STC.COMPONENTT_ID = ?COMPONENT_ID
                                    AND STC.IS_DELETED != 1;";
                        break;
                    }
                case PayrollSQLCommads.CheckIsSalarygeneratedForSC:
                    {
                        query = @"SELECT 
                                SG.SALARY_GENERATION_ID
                            FROM
                                PR_EMPLOYEE_SALARY_GENERATION AS SG
                                    INNER JOIN
                                PR_SALARY_TEMPLATE_COMPONENT_DETAILS AS STC ON STC.CTEMPLATE_ID = SG.CTEMPLATE_ID
                            WHERE
                                SG.IS_DELETED != 1
                                    AND STC.CTEMPLATE_ID = ?CTEMPLATE_ID
                                    AND SG.IS_PF_COMPONENT != 1;";
                        break;
                    }
                case PayrollSQLCommads.CheckIsDynamicComponentMapped:
                    {
                        query = @"SELECT 
                                PFC.PF_COMPONENT_ID
                            FROM
                                PR_PF_COMPONENTS AS PFC
                            WHERE
                                PFC.PF_COMPONENT_ID = ?PF_COMPONENT_ID
                                    AND STC.IS_DELETED != 1;";
                        break;
                    }
                case PayrollSQLCommads.CheckIsSalarygeneratedForPFC:
                    {
                        query = @"SELECT 
                                SG.SALARY_GENERATION_ID
                            FROM
                                PR_EMPLOYEE_SALARY_GENERATION AS SG
                                    INNER JOIN
                                PR_TEMPLATE_PFCOMPONENT_DETAILS AS TPFC ON TPFC.TEMPLATE_PF_COMPONENT_ID = SG.CTEMPLATE_ID
                            WHERE
                                SG.IS_DELETED != 1
                                    AND TPFC.TEMPLATE_PF_COMPONENT_ID = ?CTEMPLATE_ID
                                    AND SG.IS_PF_COMPONENT = 1;";
                        break;
                    }
                case PayrollSQLCommads.FetchSalaryMonths:
                    {
                        query = @"SELECT 
                                MONTH_ID,
                                MONTH_NAME,
                                FROM_DATE,
                                TO_DATE
                            FROM
                                PR_SUP_SALARY_MONTH
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case PayrollSQLCommads.FetchSalaryMonthById:
                    {
                        query = @"SELECT 
                                MONTH_ID,
                                MONTH_NAME,
                                FROM_DATE,
                                TO_DATE
                            FROM
                                PR_SUP_SALARY_MONTH
                            WHERE 
	                            MONTH_ID=?MONTH_ID AND
                                IS_DELETED != 1;";
                        break;
                    }
                case PayrollSQLCommads.SaveSalaryMonth:
                    {
                        query = @"INSERT INTO
                                PR_SUP_SALARY_MONTH(MONTH_NAME, FROM_DATE, TO_DATE) 
                                VALUES(?MONTH_NAME, ?FROM_DATE, ?TO_DATE);";
                        break;
                    }
                case PayrollSQLCommads.UpdateSalaryMonth:
                    {
                        query = @"UPDATE PR_SUP_SALARY_MONTH 
                                SET 
                                    MONTH_NAME =?MONTH_NAME,
                                    FROM_DATE =?FROM_DATE,
                                    TO_DATE =?TO_DATE
                                WHERE
                                    MONTH_ID =?MONTH_ID;";
                        break;
                    }
                case PayrollSQLCommads.DeleteSalaryMonth:
                    {
                        query = @"UPDATE PR_SUP_SALARY_MONTH 
                                SET 
                                    IS_DELETED=1
                                WHERE
                                    MONTH_ID =?MONTH_ID;";
                        break;
                    }


            }
            return query;
        }
    }
}
