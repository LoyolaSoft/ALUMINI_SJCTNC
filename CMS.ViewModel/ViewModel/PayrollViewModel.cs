using CMS.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModel.ViewModel
{
    public class PayrollViewModel
    {

        #region SelectList
        public SelectList COMPONENTS { get; set; }
        public SelectList ComponentType { get; set; }
        public SelectList Templates { get; set; }

        public SelectList STAFF_CATEGORY { get; set; }
        public SelectList LEAVE_TYPE { get; set; }
        public SelectList EMPLOYEE { get; set; }
        public SelectList INCREMENT_TYPE { get; set; }
        public SelectList PF_COMPONENTS { get; set; }
        public SelectList AcademicYearList { get; set; }
        public SelectList LEAVE_STATUS { get; set; }
        public SelectList INCREMENT_STATUS { get; set; }

        public SelectList SHIFT { get; set; }

        #endregion
        #region List
        public List<PR_PAYROLL_YEAR> lipayrollyear = new List<PR_PAYROLL_YEAR>();
        public List<PR_SALARY_COMPONENT_DETAILS> lisalarycomponentdetails = new List<PR_SALARY_COMPONENT_DETAILS>();
        public List<PR_SALARY_TEMPLATE_DETAILS> lisalarytemplate = new List<PR_SALARY_TEMPLATE_DETAILS>();
        public List<PR_SALARY_TEMPLATE_COMPONENT_DETAILS> lisalarytemplatecomponent = new List<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>();
        public List<PR_EMPOLYEE_SALARY_TEMPLATE> liemployeesalarytemplate = new List<PR_EMPOLYEE_SALARY_TEMPLATE>();
        public List<PR_LEAVE_SETTING> lileavesetting = new List<PR_LEAVE_SETTING>();
        public List<PR_PAID_LEAVE_ENTRY> lipaidleaveentry = new List<PR_PAID_LEAVE_ENTRY>();
        public List<PR_SUP_SALARY_INCREMENT_TYPE> liincrementtype = new List<PR_SUP_SALARY_INCREMENT_TYPE>();
        public List<PR_SALARY_INCREMENT_DETAILS> liincrementdetails = new List<PR_SALARY_INCREMENT_DETAILS>();
        public List<PR_PF_COMPONENTS> lipfcomponents = new List<PR_PF_COMPONENTS>();
        public List<PR_TEMPLATE_PFCOMPONENT_DETAILS> litemplatepfcomponent = new List<PR_TEMPLATE_PFCOMPONENT_DETAILS>();
        public List<PR_SALARY_TEMPLATE_INCREMENT> lisalarytemplateincrements = new List<PR_SALARY_TEMPLATE_INCREMENT>();

        public List<STF_PERSONAL_INFO> liemployee = new List<STF_PERSONAL_INFO>();
        public List<STAFF_ATTENDANCE> listaffattendance = new List<STAFF_ATTENDANCE>();
        public List<PR_EMPLOYEE_SALARY_GENERATION> liemployeesalarygeneration = new List<PR_EMPLOYEE_SALARY_GENERATION>();

        public List<PR_SUP_SALARY_MONTH> lisalarymonths = new List<PR_SUP_SALARY_MONTH>();

        public List<CollegeDetails> liCollegeDetails { get; set; }
        public List<PR_EMPLOYEE_INCREMENT_DETAILS> liEmployeeincrementdetails{ get; set; }
        public List<LM_STAFF_WORKING_TIME_DETAILS> listaffworkingtimedetails { get; set; }


        #endregion
        #region Model

        #endregion
    }

}
