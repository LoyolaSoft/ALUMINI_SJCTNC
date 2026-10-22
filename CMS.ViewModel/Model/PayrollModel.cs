using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{
    public class PayrollModel
    {

    }
    public class PR_PAYROLL_YEAR
    {
        public string YEAR_ID { get; set; }
        public string YEAR { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string ACTIVE_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string NAME { get; set; }
    }
  
    public class PR_SALARY_TEMPLATE_DETAILS
    {
        public string TEMPLATE_ID { get; set; }
        public string TEMPLATE_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string ANNUAL_CTC { get; set; }
        public string STAFF_CATEGORY { get; set; }
        public string STF_CATEGORY { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }

    }
  
    public class PR_SALARY_COMPONENT_DETAILS
    {
        public string COMPONENT_ID { get; set; }
        public string COMPONENT_TYPE { get; set; }
        public string COMPONENT_NAME { get; set; }
        public string NAME_IN_PAYSLIP { get; set; }
        public string CALCULATION_TYPE { get; set; }
        public string PERCENTAGE { get; set; }
        public string FLAT_AMOUNT { get; set; }
        public string STAFF_CATEGORY { get; set; }
        public string SALARY_COMPONENT_TYPE { get; set; }
        public string SALARY_COMPONENT_NAME { get; set; }
        public string PERCENTAGE_COMPONENT_ID { get; set; }
        public string IS_MANUAL { get; set; }
        public string AMOUNT { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string IS_GROSS_PAY { get; set; }

        public string IS_FLAT { get; set; }
        public string COMPONENTT_ID { get; set; }
        public string CTEMPLATE_ID { get; set; }
        public string TEMPLATE_PF_COMPONENT_ID { get; set; }





    }
    public class PR_SALARY_TEMPLATE_COMPONENT_DETAILS
    {
        public string CTEMPLATE_ID { get; set; }
        public string TEMPLATE_ID { get; set; }
        public  string TEMPLATE_NAME { get; set; }
        public string COMPONENTT_ID { get; set; }
        public string COMPONENT_NAME { get; set; }
        public string PERCENTAGE_OF_CTC { get; set; }
        public string FLAT_AMOUNT { get; set; }
 

        public string PERCENTAGE { get; set; }
        public string AMOUNT_MONTHLY { get; set; }
        public string AMOUNT_ANNUALY { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string PERCENTAGE_COMPONENT_ID { get; set; }
        public string PERCENTAGE_COMPONENT_NAME { get; set; }
        public string SALARY_COMPONENT_TYPE { get; set; }
        public string IS_MANUAL { get; set; }
        public string NAME_IN_PAYSLIP { get; set; }
        public string SALARY_COMPONENT_NAME { get; set; }
        public string IS_GROSS_PAY { get; set; }
        public string IS_FLAT { get; set; }
        public string COMPONENT_ID { get; set; }
     

    }

    public class PR_PF_DETAILS
    {
        public string PF_ID { get; set; }
        public string PF_NUMBER { get; set; }
        public string DEDUCTION_CYCLE { get; set; }
        public string EMPLOYER_CONTRIBUTION_RATE_IN_PERCENT { get; set; }
        public string EMPLOYER_CONTRIBUTION_RATE_IN_FLAT { get; set; }
        public string EMPLOYEE_CONTRIBUTION_RATE_IN_FLAT { get; set; }
        public string IS_PRO_RATE_RESTRICTED_PF_WAGE { get; set; }
        public string IS_OVERRIDE_PF_CONTRIBUTION_RATE_AT_EMPLOYEE_LEVEL { get; set; }
        public string STAFF_CATEGORY { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR_ID { get; set; }
    }
    public class SUP_SALARY_COMPONENT_TYPE
    {
        public string SALARY_COMPONENT_TYPE_ID { get; set; }
        public string SALARY_COMPONENT_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        
    }
    public class PR_EMPOLYEE_SALARY_TEMPLATE
    {
        public string EMP_SALARY_TEMP_ID { get; set; }
        public string TEMPLATE_ID { get; set; }
        public string TEMPLATE_NAME { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string DOB { get; set; }
        public  string DATE_OF_JOIN { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string STF_CATEGORY { get; set; }
        public string QUALIFICATION { get; set; }
        public string QUALIFICATION_ID { get; set; }

        public string IS_DELETED { get; set; }   

    }
    public class JSON_PAYROLL
    {
        public List<PR_EMPOLYEE_SALARY_TEMPLATE> Liemployetemplate { get; set; }
        public List<PR_EMPLOYEE_INCREMENT_DETAILS> Liemployeincrement { get; set; }
        public List<PR_EMPLOYEE_SALARY_GENERATION> Lisalarygeneration { get; set; }




    }
    public class PR_LEAVE_SETTING
    {
      
        public string STF_LEAVE_SETTING_ID { get; set; }
        public string STAFF_CATEGORY { get; set; }
        public string STF_CATEGORY { get; set; }
        public string EXP_YEAR_FROM { get; set; }
        public string EXP_YEAR_TO { get; set; }

        public string LEAVE_TYPE { get; set; }
        public string M_MAX_DAYS { get; set; }
        public string Y_MAX_DAYS { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_CARRIED_NEXT_YEAR { get; set; }
    }

    public class PR_PAID_LEAVE_ENTRY
    {
        public string PAID_LEAVE_ID { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LEAVE_TYPE_ID { get; set; }
        public string LEAVE_TYPE { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string ENTRY_DATE { get; set; }
        public string MONTH { get; set; }
        public string ACADEMIC_YEAR { get; set; }

        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }


    }
    public class PR_SUP_SALARY_INCREMENT_TYPE
    {
        public string SALARY_INCREMENT_TYPE_ID { get; set; }
        public string INCREMENT_TYPE_NAME { get; set; }
        public string AMOUNT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class PR_SALARY_INCREMENT_DETAILS
    {
        public string INCREMENT_ID { get; set; }
        public string SALARY_INCREMENT_TYPE_ID { get; set; }
        public string INCREMENT_TYPE_NAME { get; set; }

        public string FROM_YEAR { get; set; }
        public string TO_YEAR { get; set; }
        public string TOTAL_YEAR { get; set; }
        public string AMOUNT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string CURRENT_COUNT { get; set; }
        public string LAST_COUNT { get; set; }
        public string EXPRIENCE { get; set; }

    }
    public class PR_PF_COMPONENTS
    {
        public string PF_COMPONENT_ID { get; set; }
        public string PF_COMPONENT_NAME { get; set; }
        public string FIRST_COMPONENT { get; set; }
        public string IS_FIRST_PF_COMPONENT { get; set; }
        public string OPERATORS { get; set; }
        public string SECOND_COMPONENT { get; set; }
        public string IS_SECOND_PF_COMPONENT { get; set; }
        public string PERCENTAGE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FLAT_AMOUNT { get; set; }
        public string SALARY_COMPONENT_TYPE { get; set; }
        public string NAME_IN_PAYSLIP { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string SALARY_COMPONENT_NAME { get; set; }
        public string TEMPLATE_PF_COMPONENT_ID { get; set; }
    }


    public class PR_TEMPLATE_PFCOMPONENT_DETAILS
    {
        public string TEMPLATE_PF_COMPONENT_ID { get; set; }
        public string TEMPLATE_ID { get; set; }
        public string TEMPLATE_NAME { get; set; }
        public string PF_COMPONENT_ID { get; set; }
        public string PF_COMPONENT_NAME { get; set; }

        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

        public string FIRST_COMPONENT { get; set; }
        public string IS_FIRST_PF_COMPONENT { get; set; }
        public string OPERATORS { get; set; }
        public string SECOND_COMPONENT { get; set; }
        public string IS_SECOND_PF_COMPONENT { get; set; }
        public string PERCENTAGE { get; set; }
        public string SALARY_COMPONENT_TYPE { get; set; }
        public string NAME_IN_PAYSLIP { get; set; }
        public string SALARY_COMPONENT_NAME { get; set; }
        public string FIXED_AMOUNT { get; set; }
        public string PERCENTAGE_AMOUNT { get; set; }




    }
    public class PR_SALARY_TEMPLATE_INCREMENT
    {
        public string TEMPLATE_INCREMENT_ID { get; set; }
        public string TEMPLATE_ID { get; set; }
        public string TEMPLATE_NAME { get; set; }

        public string SALARY_INCREMENT_TYPE_ID { get; set; }
      
        public string INCREMENT_TYPE_NAME { get; set; }
        public string COMPONENT_ID { get; set; }
        public string COMPONENT_NAME { get; set; }

        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

    }
    public class STAFF_ATTENDANCE
    {
        public string STF_ATT_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string STAFF_CODE { get; set; }
        public string DATE { get; set; }

        public string TIME { get; set; }
        public string SESSION { get; set; }
        public string MONTH { get; set; }
        public string IS_DELETED { get; set; }
        public string ATT_TYPE { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string LEAVE_TYPE { get; set; }
        public string COUNT { get; set; }
        public string FIRST_NAME { get; set; }
        public string TIME_IN { get; set; }
        public string TIME_OUT { get; set; }
        public string IN_TIME { get; set; }
        public string OUT_TIME { get; set; }
        public string STATUS_NAME { get; set; }
        public string STATUS_ID { get; set; }
        public string SHIFT { get; set; }

    }

    public class PR_EMPLOYEE_INCREMENT_DETAILS
    {
        public string INCREMENT_DETAILS_ID { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string ACTUAL_INCREMENT_DATE { get; set; }
        public string APPROVAL_INCREMENT_DATE { get; set; }
        public string STATUS { get; set; }
        public string STATUS_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string MONTH { get; set; }
        public string STAFF_CATEGORY { get; set; }
        public string CURRENT_COUNT { get; set; }
        public string LAST_COUNT { get; set; }
        public string TEMPLATE_ID { get; set; }
        public string TEMPLATE_NAME { get; set; }
        public string SALARY_INCREMENT_TYPE_ID { get; set; }
        public string YEAR { get; set; }
        public string DATE_OF_JOIN { get; set; }
        public string FROM_YEAR { get; set; }
        public string TO_YEAR { get; set; }
        public string TOTAL_YEAR { get; set; }
        public string AMOUNT { get; set; }
        public string EXPERIENCE_YEAR { get; set; }


    }
    public class PR_SUP_INCREMENT_STATUS
    {
        public string STATUS_ID { get; set; }
        public string STATUS_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class PR_EMPLOYEE_SALARY_GENERATION
    {
        public string SALARY_GENERATION_ID { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string COMPONENT_ID { get; set; }
        public string COMPONENT_NAME { get; set; }
        public string AMOUNT { get; set; }
        public string IS_PF_COMPONENT { get; set; }
        public string MONTH { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string LOSS_OF_PAY { get; set; }
        public string NET_PAY { get; set; }
        public string SALARY_COMPONENT_TYPE { get; set; }
        public string NAME_IN_PAYSLIP { get; set; }
        public string IS_GROSS_PAY { get; set; }
        public string PHOTO { get; set; }
        public string CTEMPLATE_ID { get; set; }


    }
    public class PR_SUP_SALARY_MONTH
    {
        public string MONTH_ID { get; set; }
        public string MONTH_NAME { get; set; }
        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }


        
    }





}
