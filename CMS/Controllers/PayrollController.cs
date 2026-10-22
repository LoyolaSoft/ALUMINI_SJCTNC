using CMS.DAO;
using CMS.SQL.Examination;
using CMS.SQL.Payroll;
using CMS.SQL.SupportData;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.Controllers
{
    public class PayrollController : Controller
    {
        PayrollViewModel objViewModel = new PayrollViewModel();
        JsonResultData ObjJsonData = new JsonResultData();
        DataTable dt = new DataTable();
        ResultArgs resultargs = new ResultArgs();
        // GET: Payroll
        public ActionResult Index()
        {
            return View();
        }
        #region Payroll Year
        public ActionResult ListPayrollYears()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objViewModel.lipayrollyear = (List<PR_PAYROLL_YEAR>)CMSHandler.FetchData<PR_PAYROLL_YEAR>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchPayrollyears), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListPayrollYears", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListPayrollYears", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchPayrollyears(string sYearId)
        {
            var objModel = new PR_PAYROLL_YEAR();
            if (!string.IsNullOrEmpty(sYearId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var lipayrollyear = (List<PR_PAYROLL_YEAR>)CMSHandler.FetchData<PR_PAYROLL_YEAR>(new PR_PAYROLL_YEAR() { YEAR_ID = sYearId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchPayrollyearById), sAcademicYear).DataSource.Data;
                    if (lipayrollyear != null && lipayrollyear.Count > 0)
                    {
                        objModel.YEAR_ID = lipayrollyear.FirstOrDefault().YEAR_ID;
                        objModel.YEAR = lipayrollyear.FirstOrDefault().YEAR;
                        objModel.DATE_FROM = lipayrollyear.FirstOrDefault().DATE_FROM;
                        objModel.DATE_TO = lipayrollyear.FirstOrDefault().DATE_TO;
                        objModel.NAME = lipayrollyear.FirstOrDefault().NAME;


                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "FetchPayrollyears", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdatePayrollyear(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<PR_PAYROLL_YEAR>(sJsonData);
                    var SavesalaryTemplate = CMSHandler.SaveOrUpdate(Result, PayrollSQL.GetPayrollSQL((!string.IsNullOrEmpty(Result.YEAR_ID) ? PayrollSQLCommads.UpdatePayrollyear : PayrollSQLCommads.SavePayrollyear)), sAcademicYear);
                    if (SavesalaryTemplate.Success)
                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        objResult.Message = Common.Messages.FailedToSaveRecords;
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Payroll", "SaveorUpdatePayrollyear", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Payroll", "SaveorUpdatePayrollyear", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult DeletePayrollyears(string sYearId)
        {
            if (!string.IsNullOrEmpty(sYearId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new PR_PAYROLL_YEAR() { YEAR_ID = sYearId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.DeletePayrollyear));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "DeletePayrollyears", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SetActiveandDeactivePayrollyear(string sYearId, string sActive)
        {
            if (!string.IsNullOrEmpty(sYearId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new PR_PAYROLL_YEAR() { YEAR_ID = sYearId, ACTIVE_YEAR = sActive }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.SetActiveandDeactivePayrollyear));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "SetActiveandDeactivePayrollyear", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Salary Template
        public ActionResult ListSalaryTemplates()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objViewModel.lisalarytemplate = (List<PR_SALARY_TEMPLATE_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_DETAILS>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchSalaryTemplates), sAcademicYear).DataSource.Data;
                    var staffcatetgory = (List<SUP_STAFF_CATEGORY>)CMSHandler.FetchData<SUP_STAFF_CATEGORY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffCategory), sAcademicYear).DataSource.Data;
                    if (staffcatetgory != null && staffcatetgory.Count > 0)
                    {
                        objViewModel.STAFF_CATEGORY = new SelectList(staffcatetgory, Common.sup_staff_subcategory.STF_CATEGORY_ID, Common.sup_staff_subcategory.STF_CATEGORY);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListSalaryTemplates", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListSalaryTemplates", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchSalaryTemplate(string sTemplateId)
        {
            var objModel = new PR_SALARY_TEMPLATE_DETAILS();
            if (!string.IsNullOrEmpty(sTemplateId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var lisalarytempate = (List<PR_SALARY_TEMPLATE_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_DETAILS>(new PR_SALARY_TEMPLATE_DETAILS() { TEMPLATE_ID = sTemplateId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchSalaryTemplateById), sAcademicYear).DataSource.Data;
                    if (lisalarytempate != null && lisalarytempate.Count > 0)
                    {
                        objModel.TEMPLATE_ID = lisalarytempate.FirstOrDefault().TEMPLATE_ID;
                        objModel.TEMPLATE_NAME = lisalarytempate.FirstOrDefault().TEMPLATE_NAME;
                        objModel.DESCRIPTION = lisalarytempate.FirstOrDefault().DESCRIPTION;
                        objModel.STAFF_CATEGORY = lisalarytempate.FirstOrDefault().STAFF_CATEGORY;
                        objModel.ANNUAL_CTC = lisalarytempate.FirstOrDefault().ANNUAL_CTC;
                        objModel.ACADEMIC_YEAR = lisalarytempate.FirstOrDefault().ACADEMIC_YEAR;

                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "FetchSalaryTemplate", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateSalaryTemplate(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<PR_SALARY_TEMPLATE_DETAILS>(sJsonData);
                    var CheckIsMapped = (List<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>(new PR_SALARY_TEMPLATE_COMPONENT_DETAILS() { TEMPLATE_ID = Result.TEMPLATE_ID }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.CheckIsTemplateMapped)).DataSource.Data;
                    if (CheckIsMapped != null && CheckIsMapped.Count > 0)
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = "This template already mapped...!";
                    }
                    else
                    {
                        var SavesalaryTemplate = CMSHandler.SaveOrUpdate(Result, PayrollSQL.GetPayrollSQL((!string.IsNullOrEmpty(Result.TEMPLATE_ID) ? PayrollSQLCommads.UpdateSalaryTemplate : PayrollSQLCommads.SaveSalaryTemplate)), sAcademicYear);
                        if (SavesalaryTemplate.Success)
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                    }

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Payroll", "SaveorUpdateSalaryTemplate", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Payroll", "SaveorUpdateSalaryTemplate", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult DeleteSalaryTemplate(string sTemplateId)
        {
            if (!string.IsNullOrEmpty(sTemplateId))
            {
                try
                {
                    var CheckIsMapped=(List<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>(new PR_SALARY_TEMPLATE_COMPONENT_DETAILS() { TEMPLATE_ID = sTemplateId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.CheckIsTemplateMapped)).DataSource.Data;
                    if (CheckIsMapped != null && CheckIsMapped.Count > 0)
                    {
                        ObjJsonData.ErrorCode =Common.ErrorCode.BadRequest;
                        ObjJsonData.Message = "This template already mapped...!";
                    }
                    else
                    {
                        var ResultArg = CMSHandler.SaveOrUpdate(new PR_SALARY_TEMPLATE_DETAILS() { TEMPLATE_ID = sTemplateId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.DeleteSalaryTemplates));
                        if (ResultArg.Success)
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "DeleteSalaryTemplate", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Salary Component
        public ActionResult ListSalaryComponents()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objViewModel.lisalarycomponentdetails = (List<PR_SALARY_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_COMPONENT_DETAILS>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchSalaryComponents), sAcademicYear).DataSource.Data;
                    var componenttype = (List<SUP_SALARY_COMPONENT_TYPE>)CMSHandler.FetchData<SUP_SALARY_COMPONENT_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupSalaryCompoenttype), sAcademicYear).DataSource.Data;
                    if (componenttype != null && componenttype.Count > 0)
                    {
                        objViewModel.ComponentType = new SelectList(componenttype, Common.SUP_SALARY_COMPONENT_TYPE.SALARY_COMPONENT_TYPE_ID, Common.SUP_SALARY_COMPONENT_TYPE.SALARY_COMPONENT_NAME);
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListSalaryComponents", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListSalaryComponents", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchSalaryComponent(string sComponentId)
        {
            var objModel = new PR_SALARY_COMPONENT_DETAILS();
            if (!string.IsNullOrEmpty(sComponentId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var lisalarycomponent = (List<PR_SALARY_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_COMPONENT_DETAILS>(new PR_SALARY_COMPONENT_DETAILS() { COMPONENT_ID = sComponentId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchSalaryComponentById), sAcademicYear).DataSource.Data;
                    if (lisalarycomponent != null && lisalarycomponent.Count > 0)
                    {
                        objModel.COMPONENT_ID = lisalarycomponent.FirstOrDefault().COMPONENT_ID;
                        objModel.COMPONENT_NAME = lisalarycomponent.FirstOrDefault().COMPONENT_NAME;
                        objModel.SALARY_COMPONENT_TYPE = lisalarycomponent.FirstOrDefault().SALARY_COMPONENT_TYPE;

                        objModel.NAME_IN_PAYSLIP = lisalarycomponent.FirstOrDefault().NAME_IN_PAYSLIP;


                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "FetchSalaryTemplate", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateSalaryComponents(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<PR_SALARY_COMPONENT_DETAILS>(sJsonData);
                    var CheckIsMapped = (List<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>(new PR_SALARY_TEMPLATE_COMPONENT_DETAILS() { COMPONENT_ID = Result.COMPONENT_ID }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.CheckIsComponentMapped)).DataSource.Data;
                    if (CheckIsMapped != null && CheckIsMapped.Count > 0)
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = "This Component already mapped...!";
                    }
                    else
                    {
                        var SavesalaryTemplate = CMSHandler.SaveOrUpdate(Result, PayrollSQL.GetPayrollSQL((!string.IsNullOrEmpty(Result.COMPONENT_ID) ? PayrollSQLCommads.UpdateSalaryComponent : PayrollSQLCommads.SaveSalaryComponent)), sAcademicYear);
                        if (SavesalaryTemplate.Success)
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                    }

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Payroll", "SaveorUpdateSalaryComponents", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Payroll", "SaveorUpdateSalaryComponents", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult DeleteSalaryComponent(string sComponentId)
        {
            if (!string.IsNullOrEmpty(sComponentId))
            {
                try
                {

                    var CheckIsMapped = (List<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>(new PR_SALARY_TEMPLATE_COMPONENT_DETAILS() { COMPONENT_ID = sComponentId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.CheckIsComponentMapped)).DataSource.Data;
                    if (CheckIsMapped != null && CheckIsMapped.Count > 0)
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                        ObjJsonData.Message = "This Component already mapped...!";
                    }
                    else
                    {
                        var ResultArg = CMSHandler.SaveOrUpdate(new PR_SALARY_COMPONENT_DETAILS() { COMPONENT_ID = sComponentId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.DeleteSalaryComponents));
                        if (ResultArg.Success)
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }
                   

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "DeleteSalaryComponent", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Salary Template component details
        public ActionResult ListSalaryTemplateComponents()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var Templates = (List<PR_SALARY_TEMPLATE_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_DETAILS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTemplates), sAcademicYear).DataSource.Data;
                    if (Templates != null && Templates.Count > 0)
                    {
                        objViewModel.Templates = new SelectList(Templates, Common.PR_SALARY_TEMPLATE_DETAILS.TEMPLATE_ID, Common.PR_SALARY_TEMPLATE_DETAILS.TEMPLATE_NAME);
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListSalaryTemplateComponents", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListSalaryTemplateComponents", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindSalaryTemplateComponents(string sTemplateId)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objViewModel.lisalarytemplatecomponent = (List<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>(new PR_SALARY_TEMPLATE_DETAILS() { TEMPLATE_ID = sTemplateId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchSalaryTemplateComponentsByTemplateId), sAcademicYear).DataSource.Data;
                    var salarycomponent = (List<PR_SALARY_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_COMPONENT_DETAILS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchpayrollcomponents), sAcademicYear).DataSource.Data;
                    if (salarycomponent != null && salarycomponent.Count > 0)
                    {
                        objViewModel.COMPONENTS = new SelectList(salarycomponent, Common.PR_SALARY_COMPONENT_DETAILS.COMPONENT_ID, Common.PR_SALARY_COMPONENT_DETAILS.COMPONENT_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindSalaryTemplateComponents", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindSalaryTemplateComponents", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchSalaryTemplateComponents(string sCTemplateId)
        {
            var objModel = new PR_SALARY_TEMPLATE_COMPONENT_DETAILS();
            if (!string.IsNullOrEmpty(sCTemplateId))
            {
                try
                {


                    var litemplatecomponents = (List<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>(new PR_SALARY_TEMPLATE_COMPONENT_DETAILS() { CTEMPLATE_ID = sCTemplateId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchSalaryTemplateComponenstById)).DataSource.Data;
                    if (litemplatecomponents != null && litemplatecomponents.Count > 0)
                    {
                        objModel.CTEMPLATE_ID = litemplatecomponents.FirstOrDefault().CTEMPLATE_ID;
                        objModel.COMPONENTT_ID = litemplatecomponents.FirstOrDefault().COMPONENTT_ID;
                        objModel.COMPONENT_NAME = litemplatecomponents.FirstOrDefault().COMPONENT_NAME;
                        objModel.SALARY_COMPONENT_TYPE = litemplatecomponents.FirstOrDefault().SALARY_COMPONENT_TYPE;
                        objModel.PERCENTAGE = litemplatecomponents.FirstOrDefault().PERCENTAGE;
                        objModel.FLAT_AMOUNT = litemplatecomponents.FirstOrDefault().FLAT_AMOUNT;
                        objModel.PERCENTAGE_COMPONENT_ID = litemplatecomponents.FirstOrDefault().PERCENTAGE_COMPONENT_ID;
                        objModel.NAME_IN_PAYSLIP = litemplatecomponents.FirstOrDefault().NAME_IN_PAYSLIP;
                        objModel.IS_MANUAL = litemplatecomponents.FirstOrDefault().IS_MANUAL;
                        objModel.IS_GROSS_PAY = litemplatecomponents.FirstOrDefault().IS_GROSS_PAY;

                        objModel.PERCENTAGE = litemplatecomponents.FirstOrDefault().PERCENTAGE;


                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "FetchSalaryTemplateComponents", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateSalaryTemplateComponents(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>(sJsonData);
                    var CheckIsMapped = (List<PR_EMPLOYEE_SALARY_GENERATION>)CMSHandler.FetchData<PR_EMPLOYEE_SALARY_GENERATION>(new PR_SALARY_TEMPLATE_COMPONENT_DETAILS() { CTEMPLATE_ID = Result.CTEMPLATE_ID }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.CheckIsSalarygeneratedForSC)).DataSource.Data;
                    if (CheckIsMapped != null && CheckIsMapped.Count > 0)
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = "Already salary generated using this Mapped.You can't edit ...!";
                    }
                    else
                    {
                        if (Result.IS_FLAT == "1")
                        {
                            Result.PERCENTAGE = null;
                            Result.PERCENTAGE_COMPONENT_ID = null;
                        }
                        else
                        {
                            Result.FLAT_AMOUNT = null;

                        }
                        var SavesalaryTemplate = CMSHandler.SaveOrUpdate(Result, PayrollSQL.GetPayrollSQL((!string.IsNullOrEmpty(Result.CTEMPLATE_ID) ? PayrollSQLCommads.UpdateSalaryTemplateComponents : PayrollSQLCommads.SaveSalaryTemplateComponents)), sAcademicYear);
                        if (SavesalaryTemplate.Success)
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Payroll", "SaveorUpdateSalaryTemplateComponents", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Payroll", "SaveorUpdateSalaryTemplateComponents", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult DeleteSalaryTemplateComponents(string sCTemplateId)
        {
            if (!string.IsNullOrEmpty(sCTemplateId))
            {
                try
                {
                    var CheckIsMapped = (List<PR_EMPLOYEE_SALARY_GENERATION>)CMSHandler.FetchData<PR_EMPLOYEE_SALARY_GENERATION>(new PR_SALARY_TEMPLATE_COMPONENT_DETAILS() { CTEMPLATE_ID = sCTemplateId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.CheckIsSalarygeneratedForSC)).DataSource.Data;
                    if (CheckIsMapped != null && CheckIsMapped.Count > 0)
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                        ObjJsonData.Message = "Already salary generated using this Mapped";
                    }
                    else
                    {
                        var ResultArg = CMSHandler.SaveOrUpdate(new PR_SALARY_TEMPLATE_COMPONENT_DETAILS() { CTEMPLATE_ID = sCTemplateId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.DeleteSalaryTemplateComponents));
                        if (ResultArg.Success)
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "DeleteSalaryTemplateComponents", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchTemplateComponentByTemplate(string sTemplateId)
        {

            string sOption = string.Empty;
            if (!string.IsNullOrEmpty(sTemplateId))
            {
                try
                {
                    var litemplatecomponents = (List<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>(new PR_SALARY_TEMPLATE_COMPONENT_DETAILS() { TEMPLATE_ID = sTemplateId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchSalaryTemplateComponenstByTemplateId)).DataSource.Data;
                    if (litemplatecomponents != null && litemplatecomponents.Count > 0)
                    {
                        foreach (var item in litemplatecomponents)
                        {
                            sOption += "<option value=" + item.COMPONENT_ID + ">" + item.COMPONENT_NAME + "</option>";
                        }
                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "FetchTemplateComponentByTemplate", ex.Message);
                    }
                }
            }
            return Json(sOption, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SetActiveandDeactiveTemplateComponents(string sCTemplateId, string sActive)
        {
            if (!string.IsNullOrEmpty(sCTemplateId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new PR_SALARY_TEMPLATE_COMPONENT_DETAILS() { CTEMPLATE_ID = sCTemplateId, IS_ACTIVE = sActive }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.SetActiveandDeactiveTemplateComponents));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "SetActiveandDeactiveTemplateComponents", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Empolyee Salary Template
        public ActionResult ListEmpolyeesalarytemplates()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objViewModel.liemployeesalarytemplate = (List<PR_EMPOLYEE_SALARY_TEMPLATE>)CMSHandler.FetchData<PR_EMPOLYEE_SALARY_TEMPLATE>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmployeeSalaryTemplates), sAcademicYear).DataSource.Data;

                    var Templates = (List<PR_SALARY_TEMPLATE_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_DETAILS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTemplates), sAcademicYear).DataSource.Data;
                    if (Templates != null && Templates.Count > 0)
                    {
                        objViewModel.Templates = new SelectList(Templates, Common.PR_SALARY_TEMPLATE_DETAILS.TEMPLATE_ID, Common.PR_SALARY_TEMPLATE_DETAILS.TEMPLATE_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListEmpolyeesalarytemplates", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListEmpolyeesalarytemplates", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult SaveorUpdateEmployeeSalaryTemplates(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            ResultArgs Resultargs = new ResultArgs();
            int success = 0, failed = 0;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<JSON_PAYROLL>(sJsonData);
                    if (Result != null)
                    {
                        CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode), sAcademicYear);
                        var UpadteResult = CMSHandler.SaveOrUpdate(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.UpdateIsDeletedInEmployeeTemplates), sAcademicYear);
                        if (UpadteResult.Success)
                        {
                            foreach (var item in Result.Liemployetemplate)
                            {
                                if (!string.IsNullOrEmpty(item.EMP_SALARY_TEMP_ID) && !string.IsNullOrEmpty(item.TEMPLATE_ID))
                                {
                                    Resultargs = CMSHandler.SaveOrUpdate(item, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.UpdateEmployeeSalarytemplate), sAcademicYear);
                                }
                                else
                                {
                                    Resultargs = CMSHandler.SaveOrUpdate(item, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.SaveEmpolyeeSalarytemplate), sAcademicYear);
                                }
                                if (Resultargs.Success)
                                    success++;
                                else
                                    failed++;
                            }
                        }
                    }
                    objResult.Message = "" + success + " Record Saved Successfully\n" + failed + "Record Faild to Save";



                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Payroll", "SaveorUpdateEmployeeSalaryTemplates", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Payroll", "SaveorUpdateEmployeeSalaryTemplates", ex.Message);
                }

            }
            return Json(objResult);
        }
        #endregion
        #region Leave Setting 
        public ActionResult ListLeaveSetting()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objViewModel.lileavesetting = (List<PR_LEAVE_SETTING>)CMSHandler.FetchData<PR_LEAVE_SETTING>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchLeaveSetting), sAcademicYear).DataSource.Data;
                    var staffcatetgory = (List<SUP_STAFF_CATEGORY>)CMSHandler.FetchData<SUP_STAFF_CATEGORY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffCategory), sAcademicYear).DataSource.Data;
                    if (staffcatetgory != null && staffcatetgory.Count > 0)
                    {
                        objViewModel.STAFF_CATEGORY = new SelectList(staffcatetgory, Common.sup_staff_subcategory.STF_CATEGORY_ID, Common.sup_staff_subcategory.STF_CATEGORY);
                    }
                    var leavetype = (List<SUP_LEAVE_TYPE>)CMSHandler.FetchData<SUP_LEAVE_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLeaveType), sAcademicYear).DataSource.Data;
                    if (leavetype != null && leavetype.Count > 0)
                    {
                        objViewModel.LEAVE_TYPE = new SelectList(leavetype, Common.SUP_LEAVE_TYPE.LEAVE_TYPE_ID, Common.SUP_LEAVE_TYPE.LEAVE_TYPE);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListLeaveSetting", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListLeaveSetting", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchLeaveSetting(string sLeavesettingId)
        {
            var objModel = new PR_LEAVE_SETTING();
            if (!string.IsNullOrEmpty(sLeavesettingId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var lileavesetting = (List<PR_LEAVE_SETTING>)CMSHandler.FetchData<PR_LEAVE_SETTING>(new PR_LEAVE_SETTING() { STF_LEAVE_SETTING_ID = sLeavesettingId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchLeaveSettingById), sAcademicYear).DataSource.Data;
                    if (lileavesetting != null && lileavesetting.Count > 0)
                    {
                        objModel.STF_LEAVE_SETTING_ID = lileavesetting.FirstOrDefault().STF_LEAVE_SETTING_ID;
                        objModel.STAFF_CATEGORY = lileavesetting.FirstOrDefault().STAFF_CATEGORY;
                        objModel.LEAVE_TYPE = lileavesetting.FirstOrDefault().LEAVE_TYPE;
                        objModel.M_MAX_DAYS = lileavesetting.FirstOrDefault().M_MAX_DAYS;
                        objModel.Y_MAX_DAYS = lileavesetting.FirstOrDefault().Y_MAX_DAYS;
                        objModel.EXP_YEAR_FROM = lileavesetting.FirstOrDefault().EXP_YEAR_FROM;
                        objModel.EXP_YEAR_TO = lileavesetting.FirstOrDefault().EXP_YEAR_TO;

                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "FetchLeaveSetting", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateLeaveSetting(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            string sSQL = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<PR_LEAVE_SETTING>(sJsonData);
                    var Resultargs = CMSHandler.SaveOrUpdate(Result, PayrollSQL.GetPayrollSQL((!string.IsNullOrEmpty(Result.STF_LEAVE_SETTING_ID) ? PayrollSQLCommads.UpdateLeaveSetting : PayrollSQLCommads.SaveLeaveSetting)), sAcademicYear, true);
                    if (Resultargs.Success)
                    {

                        //Check is Exist
                        if (!string.IsNullOrEmpty(Result.STF_LEAVE_SETTING_ID))
                        {
                            var IsTranscationExist = (List<LM_TRANSACTION_DETAILS>)CMSHandler.FetchData<LM_TRANSACTION_DETAILS>(new LM_TRANSACTION_DETAILS() { LEAVE_SETTING_ID = Result.STF_LEAVE_SETTING_ID }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchLeaveTranscationBySettingId)).DataSource.Data;
                            if (IsTranscationExist != null && IsTranscationExist.Count > 0)
                            {
                                foreach (var item in IsTranscationExist)
                                {
                                    item.CREDIT_DAYS = Result.Y_MAX_DAYS;
                                    var resultarg = CMSHandler.SaveOrUpdate(item, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.UpdateLeaveTranscations));
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(Resultargs.RowUniqueId.ToString()))
                        {
                            var listaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO() { CATEGORY = Result.STAFF_CATEGORY, EXP_YEAR_FROM = Result.EXP_YEAR_FROM, EXP_YEAR_TO = Result.EXP_YEAR_TO }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmployeeByCategoryIdandExperience)).DataSource.Data;
                            if (listaff != null && listaff.Count > 0)
                            {
                                sSQL = @"INSERT INTO 
                                LM_TRANSACTION_DETAILS
                                (STAFF_ID,CREDIT_DAYS,
                                LEAVE_SETTING_ID,ACADEMIC_YEAR,IS_CARRIED_NEXT_YEAR) 
                                VALUES";
                                foreach (var item in listaff)
                                {
                                    sSQL += "('" + item.STAFF_ID + "','" + Result.Y_MAX_DAYS + "','" + Resultargs.RowUniqueId.ToString() + "','" + sAcademicYear + "','" + Result.IS_CARRIED_NEXT_YEAR + "'),";
                                }
                                sSQL = sSQL.TrimEnd(',');
                                sSQL = sSQL + ";";
                                var resultargs = CMSHandler.SaveOrUpdate(null, sSQL, sAcademicYear);
                            }
                        }

                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;

                    }
                    else
                        objResult.Message = Common.Messages.FailedToSaveRecords;

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Payroll", "SaveorUpdateLeaveSetting", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Payroll", "SaveorUpdateLeaveSetting", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult DeleteLeaveSetting(string sLeavesettingId)
        {
            if (!string.IsNullOrEmpty(sLeavesettingId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new PR_LEAVE_SETTING() { STF_LEAVE_SETTING_ID = sLeavesettingId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.DeleteLeaveSetting));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "DeleteLeaveSetting", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Paid Leave Entry
        public ActionResult ListPaidleaveEntry()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objViewModel.lipaidleaveentry = (List<PR_PAID_LEAVE_ENTRY>)CMSHandler.FetchData<PR_PAID_LEAVE_ENTRY>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchPaidleaveapplied), sAcademicYear).DataSource.Data;

                    var liempolyee = (List<PR_EMPOLYEE_SALARY_TEMPLATE>)CMSHandler.FetchData<PR_EMPOLYEE_SALARY_TEMPLATE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllEmployee), sAcademicYear).DataSource.Data;
                    if (liempolyee != null && liempolyee.Count > 0)
                    {
                        objViewModel.EMPLOYEE = new SelectList(liempolyee, Common.PR_SALARY_COMPONENT_DETAILS.EMPLOYEE_ID, Common.PR_SALARY_COMPONENT_DETAILS.FIRST_NAME);
                    }
                    var leavetype = (List<SUP_LEAVE_TYPE>)CMSHandler.FetchData<SUP_LEAVE_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLeaveType), sAcademicYear).DataSource.Data;
                    if (leavetype != null && leavetype.Count > 0)
                    {
                        objViewModel.LEAVE_TYPE = new SelectList(leavetype, Common.SUP_LEAVE_TYPE.LEAVE_TYPE_ID, Common.SUP_LEAVE_TYPE.LEAVE_TYPE);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListPaidleaveEntry", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListPaidleaveEntry", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchPaidleaveEntry(string sPaidleaveid)
        {
            var objModel = new PR_PAID_LEAVE_ENTRY();
            if (!string.IsNullOrEmpty(sPaidleaveid))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var lipaidleaveentry = (List<PR_PAID_LEAVE_ENTRY>)CMSHandler.FetchData<PR_PAID_LEAVE_ENTRY>(new PR_PAID_LEAVE_ENTRY() { PAID_LEAVE_ID = sPaidleaveid }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchPaidleaveappliedById), sAcademicYear).DataSource.Data;
                    if (lipaidleaveentry != null && lipaidleaveentry.Count > 0)
                    {
                        objModel.PAID_LEAVE_ID = lipaidleaveentry.FirstOrDefault().PAID_LEAVE_ID;
                        objModel.LEAVE_TYPE_ID = lipaidleaveentry.FirstOrDefault().LEAVE_TYPE_ID;
                        objModel.DATE_FROM = lipaidleaveentry.FirstOrDefault().DATE_FROM;
                        objModel.DATE_TO = lipaidleaveentry.FirstOrDefault().DATE_TO;
                        objModel.EMPLOYEE_ID = lipaidleaveentry.FirstOrDefault().EMPLOYEE_ID;


                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "FetchPaidleaveEntry", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdatePaidleaveEntry(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<PR_PAID_LEAVE_ENTRY>(sJsonData);
                    Result.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(Result.DATE_FROM);
                    Result.DATE_TO = CommonMethods.ConvertdatetoyearFromat(Result.DATE_TO);
                    var Resultargs = CMSHandler.SaveOrUpdate(Result, PayrollSQL.GetPayrollSQL((!string.IsNullOrEmpty(Result.PAID_LEAVE_ID) ? PayrollSQLCommads.UpdatePaidleaveentry : PayrollSQLCommads.SavePaidleaveentry)), sAcademicYear);
                    if (Resultargs.Success)
                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        objResult.Message = Common.Messages.FailedToSaveRecords;

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Payroll", "SaveorUpdatePaidleaveEntry", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Payroll", "SaveorUpdatePaidleaveEntry", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult DeletePaidleaveentry(string sPaidleaveid)
        {
            if (!string.IsNullOrEmpty(sPaidleaveid))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new PR_PAID_LEAVE_ENTRY() { PAID_LEAVE_ID = sPaidleaveid }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.DeletePaidleaveentry));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "DeletePaidleaveentry", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Salary Increment Type
        public ActionResult ListSalaryIncrementType()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objViewModel.liincrementtype = (List<PR_SUP_SALARY_INCREMENT_TYPE>)CMSHandler.FetchData<PR_SUP_SALARY_INCREMENT_TYPE>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchIncrementType), sAcademicYear).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListSalaryIncrementType", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListSalaryIncrementType", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchSalaryIncrementType(string sSalaryincrementtypeId)
        {
            var objModel = new PR_SUP_SALARY_INCREMENT_TYPE();
            if (!string.IsNullOrEmpty(sSalaryincrementtypeId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var liincrementtype = (List<PR_SUP_SALARY_INCREMENT_TYPE>)CMSHandler.FetchData<PR_SUP_SALARY_INCREMENT_TYPE>(new PR_SUP_SALARY_INCREMENT_TYPE() { SALARY_INCREMENT_TYPE_ID = sSalaryincrementtypeId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchIncrementTypeById), sAcademicYear).DataSource.Data;
                    if (liincrementtype != null && liincrementtype.Count > 0)
                    {
                        objModel.SALARY_INCREMENT_TYPE_ID = liincrementtype.FirstOrDefault().SALARY_INCREMENT_TYPE_ID;
                        objModel.INCREMENT_TYPE_NAME = liincrementtype.FirstOrDefault().INCREMENT_TYPE_NAME;
                        objModel.AMOUNT = liincrementtype.FirstOrDefault().AMOUNT;
                        objModel.ACADEMIC_YEAR = liincrementtype.FirstOrDefault().ACADEMIC_YEAR;

                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "FetchSalaryIncrementType", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateSalaryIncrementType(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<PR_SUP_SALARY_INCREMENT_TYPE>(sJsonData);

                    var Resultargs = CMSHandler.SaveOrUpdate(Result, PayrollSQL.GetPayrollSQL((!string.IsNullOrEmpty(Result.SALARY_INCREMENT_TYPE_ID) ? PayrollSQLCommads.UpdateIncrementType : PayrollSQLCommads.SaveIncrementType)), sAcademicYear);
                    if (Resultargs.Success)
                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        objResult.Message = Common.Messages.FailedToSaveRecords;

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Payroll", "SaveorUpdateSalaryIncrementType", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Payroll", "SaveorUpdateSalaryIncrementType", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult DeleteSalaryIncrementType(string sSalaryincrementtypeId)
        {
            if (!string.IsNullOrEmpty(sSalaryincrementtypeId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new PR_SUP_SALARY_INCREMENT_TYPE() { SALARY_INCREMENT_TYPE_ID = sSalaryincrementtypeId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.DeleteSalaryIncrementType));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "DeleteSalaryIncrementType", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Salary Increment Details
        public ActionResult ListIncrementDetails()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objViewModel.liincrementdetails = (List<PR_SALARY_INCREMENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_INCREMENT_DETAILS>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchIncrementDetails), sAcademicYear).DataSource.Data;
                    var liincrementtype = (List<PR_SUP_SALARY_INCREMENT_TYPE>)CMSHandler.FetchData<PR_SUP_SALARY_INCREMENT_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchIncrementTypes), sAcademicYear).DataSource.Data;
                    if (liincrementtype != null && liincrementtype.Count > 0)
                    {
                        objViewModel.INCREMENT_TYPE = new SelectList(liincrementtype, Common.PR_SUP_SALARY_INCREMENT_TYPE.SALARY_INCREMENT_TYPE_ID, Common.PR_SUP_SALARY_INCREMENT_TYPE.INCREMENT_TYPE_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListIncrementDetails", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListIncrementDetails", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchIncrementDetails(string sIncrementId)
        {
            var objModel = new PR_SALARY_INCREMENT_DETAILS();
            if (!string.IsNullOrEmpty(sIncrementId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var liincrementdetails = (List<PR_SALARY_INCREMENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_INCREMENT_DETAILS>(new PR_SALARY_INCREMENT_DETAILS() { INCREMENT_ID = sIncrementId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchIncrementDetailsById), sAcademicYear).DataSource.Data;
                    if (liincrementdetails != null && liincrementdetails.Count > 0)
                    {
                        objModel.SALARY_INCREMENT_TYPE_ID = liincrementdetails.FirstOrDefault().SALARY_INCREMENT_TYPE_ID;
                        objModel.FROM_YEAR = liincrementdetails.FirstOrDefault().FROM_YEAR;
                        objModel.TO_YEAR = liincrementdetails.FirstOrDefault().TO_YEAR;
                        objModel.TOTAL_YEAR = liincrementdetails.FirstOrDefault().TOTAL_YEAR;
                        objModel.AMOUNT = liincrementdetails.FirstOrDefault().AMOUNT;
                        objModel.ACADEMIC_YEAR = liincrementdetails.FirstOrDefault().ACADEMIC_YEAR;
                        objModel.INCREMENT_ID = liincrementdetails.FirstOrDefault().INCREMENT_ID;

                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "FetchIncrementDetails", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateIncrementDetails(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<PR_SALARY_INCREMENT_DETAILS>(sJsonData);

                    var CheckIsExist = (List<PR_SALARY_INCREMENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_INCREMENT_DETAILS>(Result, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.CheckIsIncrementExist), sAcademicYear).DataSource.Data;
                    if (CheckIsExist != null && CheckIsExist.Count > 0)
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                        ObjJsonData.Message = Common.Messages.RecordsExistAlready;
                    }
                    else
                    {
                        var Resultargs = CMSHandler.SaveOrUpdate(Result, PayrollSQL.GetPayrollSQL((!string.IsNullOrEmpty(Result.INCREMENT_ID) ? PayrollSQLCommads.UpdateIncrementDetails : PayrollSQLCommads.SaveIncrementDetails)), sAcademicYear);
                        if (Resultargs.Success)
                        {
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        }
                        else
                        {
                            ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }

                }
                else
                {
                    ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
                    ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Payroll", "SaveorUpdateIncrementDetails", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Payroll", "SaveorUpdateIncrementDetails", ex.Message);
                }

            }
            return Json(ObjJsonData);
        }
        public JsonResult DeleteIncrementDetails(string sIncrementId)
        {
            if (!string.IsNullOrEmpty(sIncrementId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new PR_SALARY_INCREMENT_DETAILS() { INCREMENT_ID = sIncrementId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.DeleteSalaryIncrementDetails));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "DeleteIncrementDetails", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Salary PF Components
        public ActionResult ListPFComponents()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objViewModel.lipfcomponents = (List<PR_PF_COMPONENTS>)CMSHandler.FetchData<PR_PF_COMPONENTS>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchPFComponents), sAcademicYear).DataSource.Data;

                    var componenttype = (List<SUP_SALARY_COMPONENT_TYPE>)CMSHandler.FetchData<SUP_SALARY_COMPONENT_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupSalaryCompoenttype), sAcademicYear).DataSource.Data;
                    if (componenttype != null && componenttype.Count > 0)
                    {
                        objViewModel.ComponentType = new SelectList(componenttype, Common.SUP_SALARY_COMPONENT_TYPE.SALARY_COMPONENT_TYPE_ID, Common.SUP_SALARY_COMPONENT_TYPE.SALARY_COMPONENT_NAME);
                    }


                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListPFComponents", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListPFComponents", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchPFComponents(string sPfcomponentId)
        {
            var objModel = new PR_PF_COMPONENTS();
            if (!string.IsNullOrEmpty(sPfcomponentId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var lipfcomponents = (List<PR_PF_COMPONENTS>)CMSHandler.FetchData<PR_PF_COMPONENTS>(new PR_PF_COMPONENTS() { PF_COMPONENT_ID = sPfcomponentId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchPFComponentsById), sAcademicYear).DataSource.Data;
                    if (lipfcomponents != null && lipfcomponents.Count > 0)
                    {
                        objModel.PF_COMPONENT_ID = lipfcomponents.FirstOrDefault().PF_COMPONENT_ID;
                        objModel.PF_COMPONENT_NAME = lipfcomponents.FirstOrDefault().PF_COMPONENT_NAME;

                        objModel.SALARY_COMPONENT_TYPE = lipfcomponents.FirstOrDefault().SALARY_COMPONENT_TYPE;
                        objModel.NAME_IN_PAYSLIP = lipfcomponents.FirstOrDefault().NAME_IN_PAYSLIP;
                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "FetchPFComponents", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdatePFComponents(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<PR_PF_COMPONENTS>(sJsonData);

                    var CheckIsMapped = (List<PR_PF_COMPONENTS>)CMSHandler.FetchData<PR_PF_COMPONENTS>(new PR_PF_COMPONENTS() { PF_COMPONENT_ID = Result.PF_COMPONENT_ID }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.CheckIsDynamicComponentMapped)).DataSource.Data;
                    if (CheckIsMapped != null && CheckIsMapped.Count > 0)
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = "This Component already mapped...!";
                    }
                    else
                    {
                        var Resultargs = CMSHandler.SaveOrUpdate(Result, PayrollSQL.GetPayrollSQL((!string.IsNullOrEmpty(Result.PF_COMPONENT_ID) ? PayrollSQLCommads.UpdatePFComponents : PayrollSQLCommads.SavePFComponents)), sAcademicYear);
                        if (Resultargs.Success)
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                    }

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Payroll", "SaveorUpdatePFComponents", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Payroll", "SaveorUpdatePFComponents", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult DeletePFComponents(string sPfcomponentId)
        {
            if (!string.IsNullOrEmpty(sPfcomponentId))
            {
                try
                {
                    var CheckIsMapped = (List<PR_PF_COMPONENTS>)CMSHandler.FetchData<PR_PF_COMPONENTS>(new PR_PF_COMPONENTS() { PF_COMPONENT_ID = sPfcomponentId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.CheckIsDynamicComponentMapped)).DataSource.Data;
                    if (CheckIsMapped != null && CheckIsMapped.Count > 0)
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                        ObjJsonData.Message = "This Component already mapped...!";
                    }
                    else
                    {
                        var ResultArg = CMSHandler.SaveOrUpdate(new PR_PF_COMPONENTS() { PF_COMPONENT_ID = sPfcomponentId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.DeletePFComponents));
                        if (ResultArg.Success)
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "DeletePFComponents", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Template PF Component Details
        public ActionResult ListTemplatePFComponents()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();


                    var Templates = (List<PR_SALARY_TEMPLATE_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_DETAILS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTemplates), sAcademicYear).DataSource.Data;
                    if (Templates != null && Templates.Count > 0)
                    {
                        objViewModel.Templates = new SelectList(Templates, Common.PR_SALARY_TEMPLATE_DETAILS.TEMPLATE_ID, Common.PR_SALARY_TEMPLATE_DETAILS.TEMPLATE_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListTemplatePFComponents", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListTemplatePFComponents", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindPFTemplateComponents(string sPFTemplateId)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objViewModel.litemplatepfcomponent = (List<PR_TEMPLATE_PFCOMPONENT_DETAILS>)CMSHandler.FetchData<PR_TEMPLATE_PFCOMPONENT_DETAILS>(new PR_TEMPLATE_PFCOMPONENT_DETAILS() { TEMPLATE_ID = sPFTemplateId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchTemplatePFComponentByTemplateId), sAcademicYear).DataSource.Data;

                    var lipfcomponents = (List<PR_PF_COMPONENTS>)CMSHandler.FetchData<PR_PF_COMPONENTS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllPFComponents), sAcademicYear).DataSource.Data;
                    if (lipfcomponents != null && lipfcomponents.Count > 0)
                    {
                        objViewModel.PF_COMPONENTS = new SelectList(lipfcomponents, Common.PR_SALARY_COMPONENT_DETAILS.PF_COMPONENT_ID, Common.PR_SALARY_COMPONENT_DETAILS.PF_COMPONENT_NAME);
                    }
                    var salarycomponent = (List<PR_SALARY_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_COMPONENT_DETAILS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchpayrollcomponents), sAcademicYear).DataSource.Data;
                    if (salarycomponent != null && salarycomponent.Count > 0)
                    {
                        objViewModel.COMPONENTS = new SelectList(salarycomponent, Common.PR_SALARY_COMPONENT_DETAILS.COMPONENT_ID, Common.PR_SALARY_COMPONENT_DETAILS.COMPONENT_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindPFTemplateComponents", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindPFTemplateComponents", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchTemplatePFComponents(string sPfcomponentId)
        {
            var objModel = new PR_TEMPLATE_PFCOMPONENT_DETAILS();
            if (!string.IsNullOrEmpty(sPfcomponentId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var litemplatepfcomponents = (List<PR_TEMPLATE_PFCOMPONENT_DETAILS>)CMSHandler.FetchData<PR_TEMPLATE_PFCOMPONENT_DETAILS>(new PR_TEMPLATE_PFCOMPONENT_DETAILS() { TEMPLATE_PF_COMPONENT_ID = sPfcomponentId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchTemplatePFComponentsById), sAcademicYear).DataSource.Data;
                    if (litemplatepfcomponents != null && litemplatepfcomponents.Count > 0)
                    {
                        objModel.TEMPLATE_PF_COMPONENT_ID = litemplatepfcomponents.FirstOrDefault().TEMPLATE_PF_COMPONENT_ID;
                        objModel.TEMPLATE_ID = litemplatepfcomponents.FirstOrDefault().TEMPLATE_ID;
                        objModel.PF_COMPONENT_ID = litemplatepfcomponents.FirstOrDefault().PF_COMPONENT_ID;
                        objModel.PF_COMPONENT_NAME = litemplatepfcomponents.FirstOrDefault().PF_COMPONENT_NAME;
                        objModel.FIRST_COMPONENT = litemplatepfcomponents.FirstOrDefault().FIRST_COMPONENT;
                        objModel.IS_FIRST_PF_COMPONENT = litemplatepfcomponents.FirstOrDefault().IS_FIRST_PF_COMPONENT;
                        objModel.SECOND_COMPONENT = litemplatepfcomponents.FirstOrDefault().SECOND_COMPONENT;
                        objModel.IS_SECOND_PF_COMPONENT = litemplatepfcomponents.FirstOrDefault().IS_SECOND_PF_COMPONENT;
                        objModel.OPERATORS = litemplatepfcomponents.FirstOrDefault().OPERATORS;
                        objModel.PERCENTAGE = litemplatepfcomponents.FirstOrDefault().PERCENTAGE;
                        objModel.FIXED_AMOUNT = litemplatepfcomponents.FirstOrDefault().FIXED_AMOUNT;
                        objModel.PERCENTAGE_AMOUNT = litemplatepfcomponents.FirstOrDefault().PERCENTAGE_AMOUNT;


                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "FetchTemplatePFComponents", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateTemplatePFComponents(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<PR_TEMPLATE_PFCOMPONENT_DETAILS>(sJsonData);

                    var CheckIsMapped = (List<PR_EMPLOYEE_SALARY_GENERATION>)CMSHandler.FetchData<PR_EMPLOYEE_SALARY_GENERATION>(new PR_SALARY_TEMPLATE_COMPONENT_DETAILS() { CTEMPLATE_ID = Result.TEMPLATE_PF_COMPONENT_ID }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.CheckIsSalarygeneratedForPFC)).DataSource.Data;
                    if (CheckIsMapped != null && CheckIsMapped.Count > 0)
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = "Already salary generated using this Mapped.You can't edit ...!";
                    }
                    else
                    {
                        var Resultargs = CMSHandler.SaveOrUpdate(Result, PayrollSQL.GetPayrollSQL((!string.IsNullOrEmpty(Result.TEMPLATE_PF_COMPONENT_ID) ? PayrollSQLCommads.UpdateTemplatePFComponents : PayrollSQLCommads.SaveTemplatePFComponents)), sAcademicYear);
                        if (Resultargs.Success)
                        {
                            objResult.ErrorCode = Common.ErrorCode.Ok;
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        }
                        else
                        {
                            objResult.ErrorCode = Common.ErrorCode.NoContent;
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Payroll", "SaveorUpdateTemplatePFComponents", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Payroll", "SaveorUpdateTemplatePFComponents", ex.Message);
                }
                objResult.Message = Common.ErrorMessage.BadRequest;
            }
            return Json(objResult);
        }
        public JsonResult DeleteTemplatePFComponents(string sTemplatecomponentId)
        {
            if (!string.IsNullOrEmpty(sTemplatecomponentId))
            {
                try
                {
                    var CheckIsMapped = (List<PR_EMPLOYEE_SALARY_GENERATION>)CMSHandler.FetchData<PR_EMPLOYEE_SALARY_GENERATION>(new PR_SALARY_TEMPLATE_COMPONENT_DETAILS() { CTEMPLATE_ID = sTemplatecomponentId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.CheckIsSalarygeneratedForPFC)).DataSource.Data;
                    if (CheckIsMapped != null && CheckIsMapped.Count > 0)
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                        ObjJsonData.Message = "Already salary generated using this Mapped.You can't edit ...!";
                    }
                    else
                    {
                        var ResultArg = CMSHandler.SaveOrUpdate(new PR_TEMPLATE_PFCOMPONENT_DETAILS() { TEMPLATE_PF_COMPONENT_ID = sTemplatecomponentId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.DeleteTemplatePFComponents));
                        if (ResultArg.Success)
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "DeleteTemplatePFComponents", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SetActiveandDeactiveTemplatePfComponents(string sCTemplateId, string sActive)
        {
            if (!string.IsNullOrEmpty(sCTemplateId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new PR_TEMPLATE_PFCOMPONENT_DETAILS() { TEMPLATE_PF_COMPONENT_ID = sCTemplateId, IS_ACTIVE = sActive }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.SetActiveandDeactiveTemplatePFComponents));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "SetActiveandDeactiveTemplatePfComponents", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region  Salary Template Increment Details
        public ActionResult ListSalaryTemplateIncrements()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objViewModel.lisalarytemplateincrements = (List<PR_SALARY_TEMPLATE_INCREMENT>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_INCREMENT>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchTemplateIncrements), sAcademicYear).DataSource.Data;

                    var Templates = (List<PR_SALARY_TEMPLATE_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_DETAILS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTemplates), sAcademicYear).DataSource.Data;
                    if (Templates != null && Templates.Count > 0)
                    {
                        objViewModel.Templates = new SelectList(Templates, Common.PR_SALARY_TEMPLATE_DETAILS.TEMPLATE_ID, Common.PR_SALARY_TEMPLATE_DETAILS.TEMPLATE_NAME);
                    }
                    var salarycomponent = (List<PR_SALARY_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_COMPONENT_DETAILS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchpayrollcomponents), sAcademicYear).DataSource.Data;
                    if (salarycomponent != null && salarycomponent.Count > 0)
                    {
                        objViewModel.COMPONENTS = new SelectList(salarycomponent, Common.PR_SALARY_COMPONENT_DETAILS.COMPONENT_ID, Common.PR_SALARY_COMPONENT_DETAILS.COMPONENT_NAME);
                    }
                    var liincrementtype = (List<PR_SUP_SALARY_INCREMENT_TYPE>)CMSHandler.FetchData<PR_SUP_SALARY_INCREMENT_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchIncrementTypes), sAcademicYear).DataSource.Data;
                    if (liincrementtype != null && liincrementtype.Count > 0)
                    {
                        objViewModel.INCREMENT_TYPE = new SelectList(liincrementtype, Common.PR_SUP_SALARY_INCREMENT_TYPE.SALARY_INCREMENT_TYPE_ID, Common.PR_SUP_SALARY_INCREMENT_TYPE.INCREMENT_TYPE_NAME);
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListSalaryTemplateIncrements", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListSalaryTemplateIncrements", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchSalaryTemplateIncrements(string sTemplateincrementId)
        {
            var objModel = new PR_SALARY_TEMPLATE_INCREMENT();
            if (!string.IsNullOrEmpty(sTemplateincrementId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var liincrementtemplates = (List<PR_SALARY_TEMPLATE_INCREMENT>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_INCREMENT>(new PR_SALARY_TEMPLATE_INCREMENT() { TEMPLATE_INCREMENT_ID = sTemplateincrementId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchTemplateIncrementsById), sAcademicYear).DataSource.Data;
                    if (liincrementtemplates != null && liincrementtemplates.Count > 0)
                    {
                        objModel.TEMPLATE_INCREMENT_ID = liincrementtemplates.FirstOrDefault().TEMPLATE_INCREMENT_ID;
                        objModel.TEMPLATE_ID = liincrementtemplates.FirstOrDefault().TEMPLATE_ID;
                        objModel.SALARY_INCREMENT_TYPE_ID = liincrementtemplates.FirstOrDefault().SALARY_INCREMENT_TYPE_ID;
                        objModel.COMPONENT_ID = liincrementtemplates.FirstOrDefault().COMPONENT_ID;

                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "FetchSalaryTemplateIncrements", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateSalaryTemplateIncrements(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<PR_SALARY_TEMPLATE_INCREMENT>(sJsonData);

                    var Resultargs = CMSHandler.SaveOrUpdate(Result, PayrollSQL.GetPayrollSQL((!string.IsNullOrEmpty(Result.TEMPLATE_INCREMENT_ID) ? PayrollSQLCommads.UpdateTemplateIncrements : PayrollSQLCommads.SaveTemplateIncrements)), sAcademicYear);
                    if (Resultargs.Success)
                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        objResult.Message = Common.Messages.FailedToSaveRecords;

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Payroll", "SaveorUpdateSalaryTemplateIncrements", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Payroll", "SaveorUpdateSalaryTemplateIncrements", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult DeleteSalaryTemplateIncrements(string sPfcomponentId)
        {
            if (!string.IsNullOrEmpty(sPfcomponentId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new PR_PF_COMPONENTS() { PF_COMPONENT_ID = sPfcomponentId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.DeleteTemplateIncrements));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "DeleteSalaryTemplateIncrements", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Template Component Details
        public ActionResult ListTemplateComponents()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Templates = (List<PR_SALARY_TEMPLATE_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_DETAILS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTemplates), sAcademicYear).DataSource.Data;
                    if (Templates != null && Templates.Count > 0)
                    {
                        objViewModel.Templates = new SelectList(Templates, Common.PR_SALARY_TEMPLATE_DETAILS.TEMPLATE_ID, Common.PR_SALARY_TEMPLATE_DETAILS.TEMPLATE_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListTemplateComponents", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListTemplateComponents", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindSalaryComponents(string sTemplateId)
        {
            try
            {
                if (!string.IsNullOrEmpty(sTemplateId))
                {
                    objViewModel.lisalarytemplatecomponent = (List<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>(new PR_SALARY_TEMPLATE_COMPONENT_DETAILS() { TEMPLATE_ID = sTemplateId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchTemplatesSalaryComponent)).DataSource.Data;
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindSalaryComponents", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindSalaryComponents", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindPFComponents(string sTemplateId)
        {
            try
            {
                if (!string.IsNullOrEmpty(sTemplateId))
                {
                    objViewModel.litemplatepfcomponent = (List<PR_TEMPLATE_PFCOMPONENT_DETAILS>)CMSHandler.FetchData<PR_TEMPLATE_PFCOMPONENT_DETAILS>(new PR_TEMPLATE_PFCOMPONENT_DETAILS() { TEMPLATE_ID = sTemplateId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchTemplatesPFComponent)).DataSource.Data;
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindPFComponents", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindPFComponents", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        #endregion
        #region Salary PaySlip Details
        public ActionResult SalaryPaySlipGeneration()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Acyearlist = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (Acyearlist != null && Acyearlist.Count > 0)
                    {
                        objViewModel.AcademicYearList = new SelectList(Acyearlist, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var staffcatetgory = (List<SUP_STAFF_CATEGORY>)CMSHandler.FetchData<SUP_STAFF_CATEGORY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffCategory), sAcademicYear).DataSource.Data;
                    if (staffcatetgory != null && staffcatetgory.Count > 0)
                    {
                        objViewModel.STAFF_CATEGORY = new SelectList(staffcatetgory, Common.sup_staff_subcategory.STF_CATEGORY_ID, Common.sup_staff_subcategory.STF_CATEGORY);
                    }

                    //Fetch list which is need to generate payslip
                    objViewModel.lisalarytemplate = (List<PR_SALARY_TEMPLATE_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_DETAILS>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchSalaryTemplates), sAcademicYear).DataSource.Data;
                    objViewModel.lisalarytemplatecomponent = (List<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchAlltemplatesalarycomponents), sAcademicYear).DataSource.Data;
                    objViewModel.litemplatepfcomponent = (List<PR_TEMPLATE_PFCOMPONENT_DETAILS>)CMSHandler.FetchData<PR_TEMPLATE_PFCOMPONENT_DETAILS>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchAlltemplatepfcomponents), sAcademicYear).DataSource.Data;
                    objViewModel.lisalarytemplateincrements = (List<PR_SALARY_TEMPLATE_INCREMENT>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_INCREMENT>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchAllsalaryincrementtemplate), sAcademicYear).DataSource.Data;
                    objViewModel.liemployeesalarytemplate = (List<PR_EMPOLYEE_SALARY_TEMPLATE>)CMSHandler.FetchData<PR_EMPOLYEE_SALARY_TEMPLATE>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchAllemployeesalarytemplates), sAcademicYear).DataSource.Data;
                    objViewModel.lileavesetting = (List<PR_LEAVE_SETTING>)CMSHandler.FetchData<PR_LEAVE_SETTING>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchLeaveSetting), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SalaryPaySlipGeneration", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SalaryPaySlipGeneration", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindPaySlipGeneration(string sAcyear, string sMonth, string sEmpolyeecateogry)
        {
            try
            {
                string sAcademicYear = sAcyear;
                var litemplatesalarycompoents = new List<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>();
                var litemplatepfcompoents = new List<PR_TEMPLATE_PFCOMPONENT_DETAILS>();
                var liempolyeeincrement = new List<PR_SALARY_INCREMENT_DETAILS>();
                var litemplateincrement = new List<PR_SALARY_TEMPLATE_INCREMENT>();

                var lisalarycomponents = new List<PR_SALARY_COMPONENT_DETAILS>();
                var lipfcomponents = new List<PR_PF_COMPONENTS>();
                var lilop = new List<STAFF_ATTENDANCE>();

                double percentage = 0;
                //Fetch Employee List
                var liemployee = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO() { CATEGORY = sEmpolyeecateogry }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmployees), sAcademicYear).DataSource.Data;
                //Fetch Employee Salary templates
                var liemployeesalarytemplate = (List<PR_EMPOLYEE_SALARY_TEMPLATE>)CMSHandler.FetchData<PR_EMPOLYEE_SALARY_TEMPLATE>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchAllemployeesalarytemplates), sAcademicYear).DataSource.Data;

                if (liemployee != null && liemployee.Count > 0 && liemployeesalarytemplate != null && liemployeesalarytemplate.Count > 0)
                {
                    foreach (var employee in liemployee)
                    {
                        var empsalarytemplate = liemployeesalarytemplate.Where(o => o.EMPLOYEE_ID == employee.STAFF_ID).ToList();
                        //Fetch Employee Attendance and Leave Applied
                        var liemployeeattendance = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { STAFF_ID = employee.STAFF_ID, MONTH = sMonth }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmpolyeeAttendanceById), sAcademicYear).DataSource.Data;

                        var liemployeeleaveapplied = (List<LM_STAFF_LEAVE_DETAILS>)CMSHandler.FetchData<LM_STAFF_LEAVE_DETAILS>(new LM_STAFF_LEAVE_DETAILS() { STAFF_ID = employee.STAFF_ID, APPROVAL_STATUS1 = Common.LEAVE_STATUS.Approved, APPROVAL_STATUS2 = Common.LEAVE_STATUS.Approved }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmpolyeeLeaveAppliedById), sAcademicYear).DataSource.Data;

                        var lileavesetting = (List<PR_LEAVE_SETTING>)CMSHandler.FetchData<PR_LEAVE_SETTING>(new PR_LEAVE_SETTING() { STAFF_CATEGORY = sEmpolyeecateogry }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchLeaveSettingByCategoryId), sAcademicYear).DataSource.Data;
                        //Fetch Salary Compoents and PF Compoents and Increment
                        if (empsalarytemplate != null && empsalarytemplate.Count > 0)
                        {
                            litemplatesalarycompoents = (List<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>(new PR_SALARY_TEMPLATE_COMPONENT_DETAILS() { TEMPLATE_ID = empsalarytemplate.FirstOrDefault().TEMPLATE_ID }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.Fetchtemplatesalarycomponents), sAcademicYear).DataSource.Data;
                            litemplatepfcompoents = (List<PR_TEMPLATE_PFCOMPONENT_DETAILS>)CMSHandler.FetchData<PR_TEMPLATE_PFCOMPONENT_DETAILS>(new PR_SALARY_TEMPLATE_DETAILS() { TEMPLATE_ID = empsalarytemplate.FirstOrDefault().TEMPLATE_ID }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.Fetchtemplatepfcomponents), sAcademicYear).DataSource.Data;

                            litemplateincrement = (List<PR_SALARY_TEMPLATE_INCREMENT>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_INCREMENT>(new PR_SALARY_TEMPLATE_DETAILS() { TEMPLATE_ID = empsalarytemplate.FirstOrDefault().TEMPLATE_ID }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.Fetchsalaryincrementtemplate), sAcademicYear).DataSource.Data;
                            if (litemplateincrement != null && litemplateincrement.Count > 0)
                            {
                                var liemployeeincrementstatus = (List<PR_EMPLOYEE_INCREMENT_DETAILS>)CMSHandler.FetchData<PR_EMPLOYEE_INCREMENT_DETAILS>(new PR_EMPLOYEE_INCREMENT_DETAILS() { EMPLOYEE_ID = employee.STAFF_ID }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmpolyeeIncrementStatus), sAcademicYear).DataSource.Data;
                                if (liemployeeincrementstatus != null && liemployeeincrementstatus.Count > 0)
                                {
                                    if (liemployeeincrementstatus.FirstOrDefault().STATUS == Common.INCREMENT_STATUS.Approved)
                                    {

                                        liempolyeeincrement = (List<PR_SALARY_INCREMENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_INCREMENT_DETAILS>(new PR_SALARY_INCREMENT_DETAILS() { SALARY_INCREMENT_TYPE_ID = litemplateincrement.FirstOrDefault().SALARY_INCREMENT_TYPE_ID, EMPLOYEE_ID = employee.STAFF_ID, EXPRIENCE = liemployeeincrementstatus.FirstOrDefault().EXPERIENCE_YEAR }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmpolyeeIncrementDetailsByExprience), sAcademicYear).DataSource.Data;
                                    }
                                    else
                                    {
                                        liemployeeincrementstatus.FirstOrDefault().EXPERIENCE_YEAR = Convert.ToString((Convert.ToInt32(liemployeeincrementstatus.FirstOrDefault().EXPERIENCE_YEAR) - 1));
                                        liempolyeeincrement = (List<PR_SALARY_INCREMENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_INCREMENT_DETAILS>(new PR_SALARY_INCREMENT_DETAILS() { SALARY_INCREMENT_TYPE_ID = litemplateincrement.FirstOrDefault().SALARY_INCREMENT_TYPE_ID, EMPLOYEE_ID = employee.STAFF_ID, EXPRIENCE = liemployeeincrementstatus.FirstOrDefault().EXPERIENCE_YEAR }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmpolyeeIncrementDetailsByExprience), sAcademicYear).DataSource.Data;
                                    }
                                }
                                else
                                {
                                    liempolyeeincrement = (List<PR_SALARY_INCREMENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_INCREMENT_DETAILS>(new PR_SALARY_INCREMENT_DETAILS() { SALARY_INCREMENT_TYPE_ID = litemplateincrement.FirstOrDefault().SALARY_INCREMENT_TYPE_ID, EMPLOYEE_ID = employee.STAFF_ID }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmpolyeeIncrementDetails), sAcademicYear).DataSource.Data;
                                }
                            }
                        }
                        //Fetch Paid Levae
                        var lipaidleave = (List<PR_PAID_LEAVE_ENTRY>)CMSHandler.FetchData<PR_PAID_LEAVE_ENTRY>(new PR_PAID_LEAVE_ENTRY() { EMPLOYEE_ID = employee.STAFF_ID, MONTH = sMonth }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchPaidLeaveById), sAcademicYear).DataSource.Data;

                        //Component Clac
                        if (litemplatesalarycompoents != null && litemplatesalarycompoents.Count > 0)
                        {
                            foreach (var salarycomponent in litemplatesalarycompoents)
                            {
                                if (salarycomponent.IS_MANUAL == Common.IsActiveFalg.IsActive)
                                {
                                    var incrementtemplate = litemplateincrement.Where(o => o.COMPONENT_ID == salarycomponent.COMPONENTT_ID);
                                    if (incrementtemplate != null)
                                    {

                                        var increment = liempolyeeincrement.Where(o => o.SALARY_INCREMENT_TYPE_ID == incrementtemplate.FirstOrDefault().SALARY_INCREMENT_TYPE_ID).Select(p => p.AMOUNT).ToArray();
                                        lisalarycomponents.Add(new PR_SALARY_COMPONENT_DETAILS() { COMPONENTT_ID = salarycomponent.COMPONENTT_ID, COMPONENT_ID = salarycomponent.COMPONENT_ID, COMPONENT_NAME = salarycomponent.COMPONENT_NAME, NAME_IN_PAYSLIP = salarycomponent.NAME_IN_PAYSLIP, FLAT_AMOUNT = increment[0], SALARY_COMPONENT_TYPE = salarycomponent.SALARY_COMPONENT_TYPE, EMPLOYEE_ID = employee.STAFF_ID, IS_GROSS_PAY = salarycomponent.IS_GROSS_PAY ,CTEMPLATE_ID=salarycomponent.CTEMPLATE_ID});
                                    }

                                }
                                else
                                {
                                    if (salarycomponent.FLAT_AMOUNT != null && salarycomponent.FLAT_AMOUNT != "")
                                    {
                                        lisalarycomponents.Add(new PR_SALARY_COMPONENT_DETAILS() { COMPONENTT_ID = salarycomponent.COMPONENTT_ID, COMPONENT_ID = salarycomponent.COMPONENT_ID, COMPONENT_NAME = salarycomponent.COMPONENT_NAME, NAME_IN_PAYSLIP = salarycomponent.NAME_IN_PAYSLIP, FLAT_AMOUNT = salarycomponent.FLAT_AMOUNT, SALARY_COMPONENT_TYPE = salarycomponent.SALARY_COMPONENT_TYPE, EMPLOYEE_ID = employee.STAFF_ID, IS_GROSS_PAY = salarycomponent.IS_GROSS_PAY, CTEMPLATE_ID = salarycomponent.CTEMPLATE_ID });
                                    }
                                    else if (salarycomponent.PERCENTAGE != null && salarycomponent.PERCENTAGE != "")
                                    {
                                        var percompoent = lisalarycomponents.Where(o => o.COMPONENTT_ID == salarycomponent.PERCENTAGE_COMPONENT_ID && o.EMPLOYEE_ID == employee.STAFF_ID).ToList();
                                        if (percompoent != null && percompoent.Count > 0)
                                        {
                                            percentage = (double)(Convert.ToInt32(percompoent.FirstOrDefault().FLAT_AMOUNT) * Convert.ToInt32(salarycomponent.PERCENTAGE)) / 100;
                                            lisalarycomponents.Add(new PR_SALARY_COMPONENT_DETAILS() { COMPONENTT_ID = salarycomponent.COMPONENTT_ID, COMPONENT_ID = salarycomponent.COMPONENT_ID, COMPONENT_NAME = salarycomponent.COMPONENT_NAME, NAME_IN_PAYSLIP = salarycomponent.NAME_IN_PAYSLIP, FLAT_AMOUNT = Math.Round(percentage).ToString(), SALARY_COMPONENT_TYPE = salarycomponent.SALARY_COMPONENT_TYPE, EMPLOYEE_ID = employee.STAFF_ID, IS_GROSS_PAY = salarycomponent.IS_GROSS_PAY, CTEMPLATE_ID = salarycomponent.CTEMPLATE_ID });
                                        }
                                        else
                                        {
                                            var percentcomponent = litemplatesalarycompoents.Where(o => o.COMPONENTT_ID == salarycomponent.PERCENTAGE_COMPONENT_ID).ToList();
                                            if (percentcomponent != null && percentcomponent.Count > 0)
                                            {
                                                percentage = (double)(Convert.ToInt32(percompoent.FirstOrDefault().FLAT_AMOUNT) * Convert.ToInt32(salarycomponent.PERCENTAGE)) / 100;
                                                lisalarycomponents.Add(new PR_SALARY_COMPONENT_DETAILS() { COMPONENT_ID = salarycomponent.COMPONENTT_ID, COMPONENT_NAME = salarycomponent.COMPONENT_NAME, NAME_IN_PAYSLIP = salarycomponent.NAME_IN_PAYSLIP, FLAT_AMOUNT = Math.Round(percentage).ToString(), SALARY_COMPONENT_TYPE = salarycomponent.SALARY_COMPONENT_TYPE, EMPLOYEE_ID = employee.STAFF_ID, IS_GROSS_PAY = salarycomponent.IS_GROSS_PAY, CTEMPLATE_ID = salarycomponent.CTEMPLATE_ID });
                                            }
                                        }
                                    }
                                }
                                percentage = 0;
                            }
                        }
                        //PF Component calc
                        if (litemplatepfcompoents != null && litemplatepfcompoents.Count > 0)
                        {
                            double firstcomponent = 0;
                            double secondcomponent = 0;
                            double calculation = 0;
                            double per_age = 0;
                            foreach (var pfcomponent in litemplatepfcompoents)
                            {
                                if (pfcomponent.IS_FIRST_PF_COMPONENT != Common.IsActiveFalg.IsActive)
                                {
                                    var firstarray = lisalarycomponents.Where(o => o.COMPONENTT_ID == pfcomponent.FIRST_COMPONENT && o.EMPLOYEE_ID == employee.STAFF_ID).Select(p => p.FLAT_AMOUNT).ToArray();
                                    firstcomponent = Convert.ToInt32(firstarray[0]);
                                }
                                if (pfcomponent.IS_SECOND_PF_COMPONENT != Common.IsActiveFalg.IsActive)
                                {
                                    var secondtarray = lisalarycomponents.Where(o => o.COMPONENTT_ID == pfcomponent.SECOND_COMPONENT && o.EMPLOYEE_ID == employee.STAFF_ID).Select(p => p.FLAT_AMOUNT).ToArray();
                                    secondcomponent = Convert.ToInt32(secondtarray[0]);
                                }

                                calculation = (int)dt.Compute(firstcomponent + pfcomponent.OPERATORS + secondcomponent, "");
                                if (pfcomponent.PERCENTAGE != null && pfcomponent.PERCENTAGE != "")
                                {
                                    if (pfcomponent.FIXED_AMOUNT != null && pfcomponent.PERCENTAGE_AMOUNT != null)
                                    {
                                        if (calculation > Convert.ToInt32(pfcomponent.FIXED_AMOUNT))
                                        {
                                            per_age = (double)(Convert.ToInt32(pfcomponent.PERCENTAGE_AMOUNT) * Convert.ToInt32(pfcomponent.PERCENTAGE)) / 100;
                                        }

                                        lipfcomponents.Add(new PR_PF_COMPONENTS() { PF_COMPONENT_ID = pfcomponent.PF_COMPONENT_ID, PF_COMPONENT_NAME = pfcomponent.PF_COMPONENT_NAME, SALARY_COMPONENT_TYPE = pfcomponent.SALARY_COMPONENT_TYPE, FLAT_AMOUNT = per_age.ToString(), EMPLOYEE_ID = employee.STAFF_ID,TEMPLATE_PF_COMPONENT_ID=pfcomponent.TEMPLATE_PF_COMPONENT_ID });
                                    }
                                }
                                else
                                {

                                    lipfcomponents.Add(new PR_PF_COMPONENTS() { PF_COMPONENT_ID = pfcomponent.PF_COMPONENT_ID, PF_COMPONENT_NAME = pfcomponent.PF_COMPONENT_NAME, SALARY_COMPONENT_TYPE = pfcomponent.SALARY_COMPONENT_TYPE, FLAT_AMOUNT = calculation.ToString(), EMPLOYEE_ID = employee.STAFF_ID, TEMPLATE_PF_COMPONENT_ID = pfcomponent.TEMPLATE_PF_COMPONENT_ID });
                                }

                            }
                        }

                        //Calc of LOP 
                        if (liemployeeleaveapplied != null && liemployeeleaveapplied.Count > 0)
                        {
                            foreach (var item in liemployeeleaveapplied)
                            {
                                var listaffattendance = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { DATE_TO = item.TO_DATE, DATE_FROM = item.FROM_DATE, STAFF_ID = employee.STAFF_ID }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmployeeAttendanceByDate), sAcademicYear).DataSource.Data;
                                if (listaffattendance != null && listaffattendance.Count > 0)
                                {
                                    foreach (var staffattendance in listaffattendance)
                                    {
                                        liemployeeattendance.RemoveAll(o => o.STF_ATT_ID == staffattendance.STF_ATT_ID);
                                    }
                                }
                            }
                        }
                        if (liemployeeattendance != null && liemployeeattendance.Count > 0)
                        {
                            lilop.Add(new STAFF_ATTENDANCE() { STAFF_ID = employee.STAFF_ID, COUNT = liemployeeattendance.Count().ToString() });
                        }

                        //if (lipaidleave != null && lipaidleave.Count > 0)
                        //{
                        //    foreach (var item in lipaidleave)
                        //    {
                        //        var listaffattendance = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { DATE_TO = item.DATE_TO, DATE_FROM = item.DATE_FROM }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmployeeAttendanceByDate), sAcademicYear).DataSource.Data;
                        //        if (listaffattendance != null && listaffattendance.Count > 0)
                        //        {
                        //            foreach (var staffattendance in listaffattendance)
                        //            {
                        //                liemployeeattendance.RemoveAll(o => o.STF_ATT_ID == staffattendance.STF_ATT_ID);
                        //            }
                        //        }
                        //    }
                        //}
                        //if (lileavesetting != null && lileavesetting.Count > 0 && liemployeeattendance != null && liemployeeattendance.Count > 0)
                        //{
                        //    int leavecount = 0;
                        //    foreach (var item in lileavesetting)
                        //    {
                        //        var lsc = liemployeeattendance.Where(o => o.LEAVE_TYPE == item.LEAVE_TYPE).ToList();
                        //        if (lsc != null && lsc.Count > 0)
                        //        {
                        //            leavecount += Convert.ToInt32(item.Y_MAX_DAYS);
                        //        }

                        //    }
                        //    int total = Convert.ToInt32(liemployeeattendance.Count()) - leavecount;
                        //    if (Convert.ToUInt32(liemployeeattendance.Count()) > total)
                        //    {
                        //        var licurrentmonth = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { STAFF_ID = employee.STAFF_ID, MONTH = sMonth }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmpolyeeAttendanceByIdandMonth), sAcademicYear).DataSource.Data;
                        //        if (licurrentmonth != null && licurrentmonth.Count > 0)
                        //        {
                        //            lilop.Add(new STAFF_ATTENDANCE() { STAFF_ID = employee.STAFF_ID, COUNT = licurrentmonth.Count().ToString() });
                        //        }
                        //    }
                        //}
                    }
                    objViewModel.liemployee = liemployee;
                    objViewModel.lipfcomponents = lipfcomponents;
                    objViewModel.lisalarycomponentdetails = lisalarycomponents;
                    objViewModel.listaffattendance = lilop;
                    ViewBag.Year = sAcyear;
                    ViewBag.Month = sMonth;


                }



            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindPaySlipGeneration", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindPaySlipGeneration", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult Savesalarygeneration(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            ResultArgs Resultargs = new ResultArgs();
            int update = 0;int save = 0;
            string sInsertQuery = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                   string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    

                    var Result = JsonConvert.DeserializeObject<JSON_PAYROLL>(sJsonData);
                    if (Result != null)
                    {
                        sInsertQuery += PayrollSQL.GetPayrollSQL(PayrollSQLCommads.SaveSalaryGeneration);
                        foreach (var item in Result.Lisalarygeneration)
                        {

                            var Checkisexist =(List<PR_EMPLOYEE_SALARY_GENERATION>)CMSHandler.FetchData<PR_EMPLOYEE_SALARY_GENERATION>(item, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.CheckIssalarycomponentExist), sAcademicYear).DataSource.Data;

                            if (Checkisexist != null && Checkisexist.Count > 0)
                            {
                                Resultargs = CMSHandler.SaveOrUpdate(item, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.UpdateSalaryGeneration), sAcademicYear);
                                update++;
                            }
                            else
                            {
                                sInsertQuery += "('" + item.EMPLOYEE_ID + "','" + item.COMPONENT_ID + "','" + item.AMOUNT + "','" + item.IS_PF_COMPONENT + "','" + item.MONTH + "','" + sAcademicYear + "','" + item.LOSS_OF_PAY + "','" + item.NET_PAY + "','" + item.CTEMPLATE_ID + "'),";
                                save++;
                            }
                           
                           
                        }
                        if (save > 0)
                        {
                            sInsertQuery = sInsertQuery.TrimEnd(',');
                            sInsertQuery = sInsertQuery + ";";
                            Resultargs = CMSHandler.SaveOrUpdate(null, sInsertQuery, sAcademicYear);
                        }
                        objResult.Message = "" + save + " Record Saved Successfully\n" + update + "Record Updated Successfully ...!";
                    }
                  
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Payroll", "Savesalarygeneration", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Payroll", "Savesalarygeneration", ex.Message);
                }

            }
            return Json(objResult);
        }
        public ActionResult EmpolyeePayslip()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Acyearlist = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (Acyearlist != null && Acyearlist.Count > 0)
                    {
                        objViewModel.AcademicYearList = new SelectList(Acyearlist, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var staffcatetgory = (List<SUP_STAFF_CATEGORY>)CMSHandler.FetchData<SUP_STAFF_CATEGORY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffCategory), sAcademicYear).DataSource.Data;
                    if (staffcatetgory != null && staffcatetgory.Count > 0)
                    {
                        objViewModel.STAFF_CATEGORY = new SelectList(staffcatetgory, Common.sup_staff_subcategory.STF_CATEGORY_ID, Common.sup_staff_subcategory.STF_CATEGORY);
                    }
                   
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EmpolyeePayslip", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EmpolyeePayslip", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchEmpolyeeByCategory(string sCategoryId)
        {
            string sOption = string.Empty;
            if (!string.IsNullOrEmpty(sCategoryId))
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    try
                    {


                        var liemployee = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO() { CATEGORY = sCategoryId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmployeeByCategoryId), sAcademicYear).DataSource.Data;
                        if (liemployee != null && liemployee.Count > 0)
                        {
                            foreach (var item in liemployee)
                            {
                                sOption += "<option value=" + item.STAFF_ID + ">" + item.FIRST_NAME + "</option>";
                            }


                        }

                    }
                    catch (Exception ex)
                    {

                        using (ErrorLog ObjLog = new ErrorLog())
                        {
                            ObjLog.WriteError("Payroll", "FetchSalaryTemplateIncrements", ex.Message);
                        }
                    }
                }
            }
            return Json(sOption, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindEmployeePayslip(string sAcyear, string sMonth, string sEmpolyeecateogry, string sEmployeeId)
        {
            try
            {
                string sAcademicYear = sAcyear;
                var litemplatesalarycompoents = new List<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>();
                var litemplatepfcompoents = new List<PR_TEMPLATE_PFCOMPONENT_DETAILS>();
                var liempolyeeincrement = new List<PR_SALARY_INCREMENT_DETAILS>();
                var litemplateincrement = new List<PR_SALARY_TEMPLATE_INCREMENT>();

                var lisalarycomponents = new List<PR_SALARY_COMPONENT_DETAILS>();
                var lipfcomponents = new List<PR_PF_COMPONENTS>();
                var lilop = new List<STAFF_ATTENDANCE>();

                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                double percentage = 0;
                //Fetch Employee List
                var liemployee = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO() { CATEGORY = sEmpolyeecateogry }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmployees), sAcademicYear).DataSource.Data;
                //Fetch Employee Salary templates
                var liemployeesalarytemplate = (List<PR_EMPOLYEE_SALARY_TEMPLATE>)CMSHandler.FetchData<PR_EMPOLYEE_SALARY_TEMPLATE>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchAllemployeesalarytemplates), sAcademicYear).DataSource.Data;
                var liemployeeleaveapplied = (List<LM_STAFF_LEAVE_DETAILS>)CMSHandler.FetchData<LM_STAFF_LEAVE_DETAILS>(new LM_STAFF_LEAVE_DETAILS() { STAFF_ID = sEmployeeId, APPROVAL_STATUS1 = Common.LEAVE_STATUS.Approved, APPROVAL_STATUS2 = Common.LEAVE_STATUS.Approved }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmpolyeeLeaveAppliedById), sAcademicYear).DataSource.Data;

                if (liemployeesalarytemplate != null && liemployeesalarytemplate.Count > 0)
                {

                    var empsalarytemplate = liemployeesalarytemplate.Where(o => o.EMPLOYEE_ID == sEmployeeId).ToList();
                    //Fetch Employee Attendance and leave setting
                    var liemployeeattendance = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { STAFF_ID = sEmployeeId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmpolyeeAttendanceById), sAcademicYear).DataSource.Data;
                    var lileavesetting = (List<PR_LEAVE_SETTING>)CMSHandler.FetchData<PR_LEAVE_SETTING>(new PR_LEAVE_SETTING() { STAFF_CATEGORY = sEmpolyeecateogry }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchLeaveSettingByCategoryId), sAcademicYear).DataSource.Data;
                    //Fetch Salary Compoents and PF Compoents and Increment
                    if (empsalarytemplate != null && empsalarytemplate.Count > 0)
                    {
                        litemplatesalarycompoents = (List<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_COMPONENT_DETAILS>(new PR_SALARY_TEMPLATE_COMPONENT_DETAILS() { TEMPLATE_ID = empsalarytemplate.FirstOrDefault().TEMPLATE_ID }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.Fetchtemplatesalarycomponents), sAcademicYear).DataSource.Data;
                        litemplatepfcompoents = (List<PR_TEMPLATE_PFCOMPONENT_DETAILS>)CMSHandler.FetchData<PR_TEMPLATE_PFCOMPONENT_DETAILS>(new PR_SALARY_TEMPLATE_DETAILS() { TEMPLATE_ID = empsalarytemplate.FirstOrDefault().TEMPLATE_ID }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.Fetchtemplatepfcomponents), sAcademicYear).DataSource.Data;
                        litemplateincrement = (List<PR_SALARY_TEMPLATE_INCREMENT>)CMSHandler.FetchData<PR_SALARY_TEMPLATE_INCREMENT>(new PR_SALARY_TEMPLATE_DETAILS() { TEMPLATE_ID = empsalarytemplate.FirstOrDefault().TEMPLATE_ID }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.Fetchsalaryincrementtemplate), sAcademicYear).DataSource.Data;
                        if (litemplateincrement != null && litemplateincrement.Count > 0)
                        {

                            var liemployeeincrementstatus = (List<PR_EMPLOYEE_INCREMENT_DETAILS>)CMSHandler.FetchData<PR_EMPLOYEE_INCREMENT_DETAILS>(new PR_EMPLOYEE_INCREMENT_DETAILS() { EMPLOYEE_ID = sEmployeeId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmpolyeeIncrementStatus), sAcademicYear).DataSource.Data;
                            if (liemployeeincrementstatus != null && liemployeeincrementstatus.Count > 0)
                            {
                                if (liemployeeincrementstatus.FirstOrDefault().STATUS == Common.INCREMENT_STATUS.Approved)
                                {

                                    liempolyeeincrement = (List<PR_SALARY_INCREMENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_INCREMENT_DETAILS>(new PR_SALARY_INCREMENT_DETAILS() { SALARY_INCREMENT_TYPE_ID = litemplateincrement.FirstOrDefault().SALARY_INCREMENT_TYPE_ID, EMPLOYEE_ID = sEmployeeId, EXPRIENCE = liemployeeincrementstatus.FirstOrDefault().EXPERIENCE_YEAR }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmpolyeeIncrementDetailsByExprience), sAcademicYear).DataSource.Data;
                                }
                                else
                                {
                                    liemployeeincrementstatus.FirstOrDefault().EXPERIENCE_YEAR = Convert.ToString((Convert.ToInt32(liemployeeincrementstatus.FirstOrDefault().EXPERIENCE_YEAR) - 1));
                                    liempolyeeincrement = (List<PR_SALARY_INCREMENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_INCREMENT_DETAILS>(new PR_SALARY_INCREMENT_DETAILS() { SALARY_INCREMENT_TYPE_ID = litemplateincrement.FirstOrDefault().SALARY_INCREMENT_TYPE_ID, EMPLOYEE_ID = sEmployeeId, EXPRIENCE = liemployeeincrementstatus.FirstOrDefault().EXPERIENCE_YEAR }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmpolyeeIncrementDetailsByExprience), sAcademicYear).DataSource.Data;
                                }
                            }
                            else
                            {
                                liempolyeeincrement = (List<PR_SALARY_INCREMENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_INCREMENT_DETAILS>(new PR_SALARY_INCREMENT_DETAILS() { SALARY_INCREMENT_TYPE_ID = litemplateincrement.FirstOrDefault().SALARY_INCREMENT_TYPE_ID, EMPLOYEE_ID = sEmployeeId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmpolyeeIncrementDetails), sAcademicYear).DataSource.Data;
                            }
                        }
                    }
                    //Fetch Paid Levae
                    var lipaidleave = (List<PR_PAID_LEAVE_ENTRY>)CMSHandler.FetchData<PR_PAID_LEAVE_ENTRY>(new PR_PAID_LEAVE_ENTRY() { EMPLOYEE_ID = sEmployeeId, MONTH = sMonth }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchPaidLeaveById), sAcademicYear).DataSource.Data;

                    //Component Clac
                    if (litemplatesalarycompoents != null && litemplatesalarycompoents.Count > 0)
                    {
                        // var salarycomponent=litemplatesalarycompoents.Where(o=>o.s)
                        foreach (var salarycomponent in litemplatesalarycompoents)
                        {
                            if (salarycomponent.IS_MANUAL == Common.IsActiveFalg.IsActive)
                            {
                                var incrementtemplate = litemplateincrement.Where(o => o.COMPONENT_ID == salarycomponent.COMPONENTT_ID);
                                if (incrementtemplate != null)
                                {
                                    var increment = liempolyeeincrement.Where(o => o.SALARY_INCREMENT_TYPE_ID == incrementtemplate.FirstOrDefault().SALARY_INCREMENT_TYPE_ID).Select(p => p.AMOUNT).ToArray();
                                    lisalarycomponents.Add(new PR_SALARY_COMPONENT_DETAILS() { COMPONENTT_ID = salarycomponent.COMPONENTT_ID, COMPONENT_ID = salarycomponent.COMPONENT_ID, COMPONENT_NAME = salarycomponent.COMPONENT_NAME, NAME_IN_PAYSLIP = salarycomponent.NAME_IN_PAYSLIP, FLAT_AMOUNT = increment[0], SALARY_COMPONENT_TYPE = salarycomponent.SALARY_COMPONENT_TYPE, EMPLOYEE_ID = sEmployeeId, IS_GROSS_PAY = salarycomponent.IS_GROSS_PAY });
                                }

                            }
                            else
                            {
                                if (salarycomponent.FLAT_AMOUNT != null && salarycomponent.FLAT_AMOUNT != "")
                                {
                                    lisalarycomponents.Add(new PR_SALARY_COMPONENT_DETAILS() { COMPONENTT_ID = salarycomponent.COMPONENTT_ID, COMPONENT_ID = salarycomponent.COMPONENT_ID, COMPONENT_NAME = salarycomponent.COMPONENT_NAME, NAME_IN_PAYSLIP = salarycomponent.NAME_IN_PAYSLIP, FLAT_AMOUNT = salarycomponent.FLAT_AMOUNT, SALARY_COMPONENT_TYPE = salarycomponent.SALARY_COMPONENT_TYPE, EMPLOYEE_ID = sEmployeeId, IS_GROSS_PAY = salarycomponent.IS_GROSS_PAY });
                                }
                                else if (salarycomponent.PERCENTAGE != null && salarycomponent.PERCENTAGE != "")
                                {
                                    var percompoent = lisalarycomponents.Where(o => o.COMPONENT_ID == salarycomponent.PERCENTAGE_COMPONENT_ID).ToList();
                                    if (percompoent != null && percompoent.Count > 0)
                                    {
                                        percentage = (double)(Convert.ToInt32(percompoent.FirstOrDefault().FLAT_AMOUNT) * Convert.ToInt32(salarycomponent.PERCENTAGE)) / 100;
                                        lisalarycomponents.Add(new PR_SALARY_COMPONENT_DETAILS() { COMPONENTT_ID = salarycomponent.COMPONENTT_ID, COMPONENT_ID = salarycomponent.COMPONENT_ID, COMPONENT_NAME = salarycomponent.COMPONENT_NAME, NAME_IN_PAYSLIP = salarycomponent.NAME_IN_PAYSLIP, FLAT_AMOUNT = Math.Round(percentage).ToString(), SALARY_COMPONENT_TYPE = salarycomponent.SALARY_COMPONENT_TYPE, EMPLOYEE_ID = sEmployeeId, IS_GROSS_PAY = salarycomponent.IS_GROSS_PAY });
                                    }
                                    else
                                    {
                                        var percentcomponent = litemplatesalarycompoents.Where(o => o.COMPONENTT_ID == salarycomponent.PERCENTAGE_COMPONENT_ID).ToList();
                                        if (percentcomponent != null && percentcomponent.Count > 0)
                                        {
                                            percentage = (double)(Convert.ToInt32(percompoent.FirstOrDefault().FLAT_AMOUNT) * Convert.ToInt32(salarycomponent.PERCENTAGE)) / 100;
                                            lisalarycomponents.Add(new PR_SALARY_COMPONENT_DETAILS() { COMPONENTT_ID = salarycomponent.COMPONENTT_ID, COMPONENT_ID = salarycomponent.COMPONENT_ID, COMPONENT_NAME = salarycomponent.COMPONENT_NAME, NAME_IN_PAYSLIP = salarycomponent.NAME_IN_PAYSLIP, FLAT_AMOUNT = Math.Round(percentage).ToString(), SALARY_COMPONENT_TYPE = salarycomponent.SALARY_COMPONENT_TYPE, EMPLOYEE_ID = sEmployeeId, IS_GROSS_PAY = salarycomponent.IS_GROSS_PAY });
                                        }
                                    }
                                }
                            }
                            percentage = 0;
                        }
                    }
                    //PF Component calc
                    if (litemplatepfcompoents != null && litemplatepfcompoents.Count > 0)
                    {
                        double firstcomponent = 0;
                        double secondcomponent = 0;
                        double calculation = 0;
                        double per_age = 0;
                        foreach (var pfcomponent in litemplatepfcompoents)
                        {
                            if (pfcomponent.IS_FIRST_PF_COMPONENT != Common.IsActiveFalg.IsActive)
                            {
                                var firstarray = lisalarycomponents.Where(o => o.COMPONENTT_ID == pfcomponent.FIRST_COMPONENT).Select(p => p.FLAT_AMOUNT).ToArray();
                                firstcomponent = Convert.ToInt32(firstarray[0]);
                            }
                            else if (pfcomponent.IS_SECOND_PF_COMPONENT != Common.IsActiveFalg.IsActive)
                            {
                                var secondtarray = lisalarycomponents.Where(o => o.COMPONENTT_ID == pfcomponent.SECOND_COMPONENT).Select(p => p.FLAT_AMOUNT).ToArray();
                                secondcomponent = Convert.ToInt32(secondtarray[0]);
                            }

                            calculation = (int)dt.Compute(firstcomponent + pfcomponent.OPERATORS + secondcomponent, "");
                            if (pfcomponent.PERCENTAGE != null && pfcomponent.PERCENTAGE != "")
                            {
                                if (calculation > 20000)
                                {
                                    per_age = (double)(Convert.ToInt32(15000) * Convert.ToInt32(pfcomponent.PERCENTAGE)) / 100;
                                }
                                lipfcomponents.Add(new PR_PF_COMPONENTS() { PF_COMPONENT_ID = pfcomponent.PF_COMPONENT_ID, PF_COMPONENT_NAME = pfcomponent.PF_COMPONENT_NAME, SALARY_COMPONENT_TYPE = pfcomponent.SALARY_COMPONENT_TYPE, FLAT_AMOUNT = per_age.ToString(), EMPLOYEE_ID = sEmployeeId, NAME_IN_PAYSLIP = pfcomponent.NAME_IN_PAYSLIP });
                            }
                            else
                            {

                                lipfcomponents.Add(new PR_PF_COMPONENTS() { PF_COMPONENT_ID = pfcomponent.PF_COMPONENT_ID, PF_COMPONENT_NAME = pfcomponent.PF_COMPONENT_NAME, SALARY_COMPONENT_TYPE = pfcomponent.SALARY_COMPONENT_TYPE, FLAT_AMOUNT = calculation.ToString(), EMPLOYEE_ID = sEmployeeId, NAME_IN_PAYSLIP = pfcomponent.NAME_IN_PAYSLIP });
                            }

                        }
                    }
                    //Calc of LOP 
                    if (liemployeeleaveapplied != null && liemployeeleaveapplied.Count > 0)
                    {
                        foreach (var item in liemployeeleaveapplied)
                        {
                            var listaffattendance = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { DATE_TO = item.TO_DATE, DATE_FROM = item.FROM_DATE, STAFF_ID = sEmployeeId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmployeeAttendanceByDate), sAcademicYear).DataSource.Data;
                            if (listaffattendance != null && listaffattendance.Count > 0)
                            {
                                foreach (var staffattendance in listaffattendance)
                                {
                                    liemployeeattendance.RemoveAll(o => o.STF_ATT_ID == staffattendance.STF_ATT_ID);
                                }
                            }
                        }
                    }
                    if (liemployeeattendance != null && liemployeeattendance.Count > 0)
                    {
                        lilop.Add(new STAFF_ATTENDANCE() { STAFF_ID = sEmployeeId, COUNT = liemployeeattendance.Count().ToString() });
                    }

                    //if (lipaidleave != null && lipaidleave.Count > 0)
                    //{
                    //    foreach (var item in lipaidleave)
                    //    {
                    //        var listaffattendance = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { DATE_TO = item.DATE_TO, DATE_FROM = item.DATE_FROM }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmployeeAttendanceByDate), sAcademicYear).DataSource.Data;
                    //        if (listaffattendance != null && listaffattendance.Count > 0)
                    //        {
                    //            foreach (var staffattendance in listaffattendance)
                    //            {
                    //                liemployeeattendance.RemoveAll(o => o.STF_ATT_ID == staffattendance.STF_ATT_ID);
                    //            }
                    //        }
                    //    }
                    //}
                    //if (lileavesetting != null && lileavesetting.Count > 0 && liemployeeattendance != null && liemployeeattendance.Count > 0)
                    //{
                    //    int leavecount = 0;
                    //    foreach (var item in lileavesetting)
                    //    {
                    //        var lsc = liemployeeattendance.Where(o => o.LEAVE_TYPE == item.LEAVE_TYPE).ToList();
                    //        if (lsc != null && lsc.Count > 0)
                    //        {
                    //            leavecount += Convert.ToInt32(item.Y_MAX_DAYS);
                    //        }

                    //    }
                    //    int total = Convert.ToInt32(liemployeeattendance.Count()) - leavecount;
                    //    if (Convert.ToUInt32(liemployeeattendance.Count()) > total)
                    //    {
                    //        var licurrentmonth = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { STAFF_ID = sEmployeeId, MONTH = sMonth }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmpolyeeAttendanceByIdandMonth), sAcademicYear).DataSource.Data;
                    //        if (licurrentmonth != null && licurrentmonth.Count > 0)
                    //        {
                    //            lilop.Add(new STAFF_ATTENDANCE() { STAFF_ID = sEmployeeId, COUNT = licurrentmonth.Count().ToString() });
                    //        }
                    //    }
                    //}

                }
                objViewModel.liemployee = liemployee.Where(o => o.STAFF_ID == sEmployeeId).ToList();
                objViewModel.lipfcomponents = lipfcomponents;
                objViewModel.lisalarycomponentdetails = lisalarycomponents;
                objViewModel.listaffattendance = lilop;
                ViewBag.Year = sAcyear;
                ViewBag.Monthinnumber = sMonth;
                ViewBag.Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(sMonth)); ;

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindEmployeePayslip", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindEmployeePayslip", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindEmployeePayslipreceipt(string sAcyear, string sMonth, string sEmployeeId)
        {
            string sSQL = string.Empty;
            var objModel = new PR_EMPLOYEE_SALARY_GENERATION();

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                   
                    string sAcademicYear = sAcyear;
                    objModel.MONTH = sMonth;
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;

                    sSQL = PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmployeeSalarygenerationBystaffIdandMonthyear).Replace(Common.Delimiter.QUS+Common.STF_PERSONAL_INFO.EMPLOYEE_ID,sEmployeeId);
                    objViewModel.liemployeesalarygeneration = (List<PR_EMPLOYEE_SALARY_GENERATION>)CMSHandler.FetchData<PR_EMPLOYEE_SALARY_GENERATION>(objModel, sSQL, sAcademicYear).DataSource.Data;

                    ViewBag.Year = sAcyear;
                    ViewBag.Monthinnumber = sMonth;
                    ViewBag.Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(sMonth)); ;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindEmployeePayslipreceipt", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindEmployeePayslipreceipt", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        #endregion

        #region Increment Maintain details
        public ActionResult ListEmployeeIncrementdetails()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Acyearlist = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (Acyearlist != null && Acyearlist.Count > 0)
                    {
                        objViewModel.AcademicYearList = new SelectList(Acyearlist, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var staffcatetgory = (List<SUP_STAFF_CATEGORY>)CMSHandler.FetchData<SUP_STAFF_CATEGORY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffCategory), sAcademicYear).DataSource.Data;
                    if (staffcatetgory != null && staffcatetgory.Count > 0)
                    {
                        objViewModel.STAFF_CATEGORY = new SelectList(staffcatetgory, Common.sup_staff_subcategory.STF_CATEGORY_ID, Common.sup_staff_subcategory.STF_CATEGORY);
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListEmployeeIncrementdetails", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListEmployeeIncrementdetails", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindIncrementEmployeeList(string sAcyear, string sMonth, string sEmpolyeecateogry)
        {
            var liemployeeincrement = new List<PR_EMPLOYEE_INCREMENT_DETAILS>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {

                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var liincrmentstatus = (List<PR_SUP_INCREMENT_STATUS>)CMSHandler.FetchData<PR_SUP_INCREMENT_STATUS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchIncrementStatus)).DataSource.Data;
                    if (liincrmentstatus != null && liincrmentstatus.Count > 0)
                    {
                        objViewModel.INCREMENT_STATUS = new SelectList(liincrmentstatus, Common.PR_SUP_INCREMENT_STATUS.STATUS_ID, Common.PR_SUP_INCREMENT_STATUS.STATUS_NAME);
                    }
                    var liemployeedetails = (List<PR_EMPLOYEE_INCREMENT_DETAILS>)CMSHandler.FetchData<PR_EMPLOYEE_INCREMENT_DETAILS>(new PR_EMPLOYEE_INCREMENT_DETAILS { MONTH = sMonth, YEAR = sAcyear, STAFF_CATEGORY = sEmpolyeecateogry, ACADEMIC_YEAR = sAcyear }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchEmployeeincrementByMonthandyear), sAcademicYear).DataSource.Data;
                    if (liemployeedetails != null && liemployeedetails.Count > 0)
                    {
                        foreach (var item in liemployeedetails)
                        {

                            var licurrentincrment = (List<PR_SALARY_INCREMENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_INCREMENT_DETAILS>(new PR_SALARY_INCREMENT_DETAILS() { SALARY_INCREMENT_TYPE_ID = item.SALARY_INCREMENT_TYPE_ID, CURRENT_COUNT = item.CURRENT_COUNT }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchIncrementDetailsByIncrementtypeIdandExperience), sAcademicYear).DataSource.Data;
                            var lilastincrement = (List<PR_SALARY_INCREMENT_DETAILS>)CMSHandler.FetchData<PR_SALARY_INCREMENT_DETAILS>(new PR_SALARY_INCREMENT_DETAILS() { SALARY_INCREMENT_TYPE_ID = item.SALARY_INCREMENT_TYPE_ID, CURRENT_COUNT = item.LAST_COUNT }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchIncrementDetailsByIncrementtypeIdandExperience), sAcademicYear).DataSource.Data;
                            if (licurrentincrment != null && lilastincrement != null)
                            {
                                if (licurrentincrment != null && licurrentincrment.Count > 0)
                                {
                                    foreach (var currentincre in licurrentincrment)
                                    {
                                        lilastincrement.RemoveAll(o => o.INCREMENT_ID == currentincre.INCREMENT_ID);
                                    }
                                    if (lilastincrement != null && lilastincrement.Count > 0)
                                    {
                                        liemployeeincrement.Add(new PR_EMPLOYEE_INCREMENT_DETAILS() { EMPLOYEE_ID = item.EMPLOYEE_ID, DATE_OF_JOIN = item.DATE_OF_JOIN, FIRST_NAME = item.FIRST_NAME, FROM_YEAR = lilastincrement.FirstOrDefault().FROM_YEAR, TO_YEAR = lilastincrement.FirstOrDefault().TO_YEAR, TOTAL_YEAR = lilastincrement.FirstOrDefault().TOTAL_YEAR, AMOUNT = lilastincrement.FirstOrDefault().AMOUNT, STATUS = item.STATUS, STATUS_NAME = item.STATUS_NAME, CURRENT_COUNT = item.CURRENT_COUNT, TEMPLATE_NAME = item.TEMPLATE_NAME, INCREMENT_DETAILS_ID = item.INCREMENT_DETAILS_ID });
                                    }
                                }
                            }
                        }
                    }
                    objViewModel.liEmployeeincrementdetails = liemployeeincrement;

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindIncrementEmployeeList", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindIncrementEmployeeList", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult SaveorUpdateEmployeeIncrementDetails(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            int success = 0;
            int failed = 0;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (!string.IsNullOrEmpty(sJsonData))
                    {
                        var Result = JsonConvert.DeserializeObject<JSON_PAYROLL>(sJsonData);
                        if (Result != null)
                        {
                            foreach (var item in Result.Liemployeincrement)
                            {
                                var Resultargs = CMSHandler.SaveOrUpdate(item, PayrollSQL.GetPayrollSQL((!string.IsNullOrEmpty(item.INCREMENT_DETAILS_ID) ? PayrollSQLCommads.UpdateEmployeeIncrementDetails : PayrollSQLCommads.SaveEmployeeIncrementDetails)), sAcademicYear);
                                if (Resultargs.Success)
                                    success++;
                                else
                                    failed++;
                            }
                            objResult.Message = "" + success + " Record Saved Successfully\n" + failed + "Record Faild to Save";
                        }
                    }




                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Payroll", "SaveorUpdateEmployeeIncrementDetails", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Payroll", "SaveorUpdateEmployeeIncrementDetails", ex.Message);
                }

            }
            return Json(objResult);
        }
        #endregion
        #region Staff Attendance
        public ActionResult StaffAttendance()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var lileavestatus = (List<LM_LEAVE_STATUS>)CMSHandler.FetchData<LM_LEAVE_STATUS>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchlmLeaveStatus)).DataSource.Data;

                    if (lileavestatus != null && lileavestatus.Count > 0)
                    {
                        objViewModel.LEAVE_STATUS = new SelectList(lileavestatus, Common.SUP_LI_STATUS.STATUS_ID, Common.SUP_LI_STATUS.STATUS_NAME);

                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("StaffAttendance", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("StaffAttendance", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindStaffAttendanceByDate(string sFromDate, string sToDate)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    sFromDate = CommonMethods.ConvertdatetoyearFromat(sFromDate);
                    sToDate = CommonMethods.ConvertdatetoyearFromat(sToDate);

                    objViewModel.listaffattendance = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { DATE_FROM = sFromDate, DATE_TO = sToDate }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchStaffAttendanceByDate)).DataSource.Data;
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindStaffAttendanceByDate", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindStaffAttendanceByDate", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchStaffAttendanceDetails(string sStaffAttendanceId)
        {
            var objModel = new STAFF_ATTENDANCE();
            if (!string.IsNullOrEmpty(sStaffAttendanceId))
            {
                try
                {
                    var listaffattendance = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { STF_ATT_ID = sStaffAttendanceId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchStaffAttendanceByStfAttId)).DataSource.Data;
                    if (listaffattendance != null && listaffattendance.Count > 0)
                    {
                        objModel.STF_ATT_ID = listaffattendance.FirstOrDefault().STF_ATT_ID;
                        objModel.STATUS_ID = listaffattendance.FirstOrDefault().STATUS_ID;
                        objModel.STATUS_NAME = listaffattendance.FirstOrDefault().STATUS_NAME;
                        objModel.STATUS_NAME = listaffattendance.FirstOrDefault().STATUS_NAME;

                        objModel.TIME_IN = listaffattendance.FirstOrDefault().TIME_IN;
                        objModel.TIME_OUT = listaffattendance.FirstOrDefault().TIME_OUT;
                        objModel.IN_TIME = listaffattendance.FirstOrDefault().IN_TIME;
                        objModel.OUT_TIME = listaffattendance.FirstOrDefault().OUT_TIME;


                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchStaffAttendanceDetails", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchStaffAttendanceDetails", "Payroll", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateStaffAttendance(string sJsonData)
        {

            if (!string.IsNullOrEmpty(sJsonData))
            {
                try
                {
                    var staffattendancedetails = JsonConvert.DeserializeObject<STAFF_ATTENDANCE>(sJsonData);
                    if (staffattendancedetails != null)
                    {

                        var timeinformat = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { TIME = staffattendancedetails.IN_TIME }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.Converttotimeformat)).DataSource.Data;
                        var timeoutformat = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { TIME = staffattendancedetails.OUT_TIME }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.Converttotimeformat)).DataSource.Data;

                        staffattendancedetails.TIME_IN = string.Concat(CommonMethods.ConvertdatetoyearFromat(staffattendancedetails.TIME_IN), " ", timeinformat.FirstOrDefault().TIME);
                        staffattendancedetails.TIME_OUT = string.Concat(CommonMethods.ConvertdatetoyearFromat(staffattendancedetails.TIME_OUT), " ", timeoutformat.FirstOrDefault().TIME);
                        resultargs = CMSHandler.SaveOrUpdate(staffattendancedetails, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.UpdateStaffAttendance));
                        if (resultargs.Success)
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("UpdateStaffAttendance", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("UpdateStaffAttendance", "Payroll", ex.Message);
                    }
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        //Staff Working time details Shift wise
        public ActionResult ListStaffworkingdetails()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objViewModel.listaffworkingtimedetails = (List<LM_STAFF_WORKING_TIME_DETAILS>)CMSHandler.FetchData<LM_STAFF_WORKING_TIME_DETAILS>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchStaffWorkingtimedetails), sAcademicYear).DataSource.Data;

                    //Fetch Staff Category
                    var staffcatetgory = (List<SUP_STAFF_CATEGORY>)CMSHandler.FetchData<SUP_STAFF_CATEGORY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffCategory)).DataSource.Data;
                    if (staffcatetgory != null && staffcatetgory.Count > 0)
                    {
                        objViewModel.STAFF_CATEGORY = new SelectList(staffcatetgory, Common.sup_staff_subcategory.STF_CATEGORY_ID, Common.sup_staff_subcategory.STF_CATEGORY);
                    }
                    //Fetch Shift
                    var lishift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (lishift != null && lishift.Count > 0)
                    {
                        objViewModel.SHIFT = new SelectList(lishift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListStaffworkingdetails", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListStaffworkingdetails", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchStaffWorkingdetails(string sStaffworkingtimeId)
        {
            var objModel = new LM_STAFF_WORKING_TIME_DETAILS();
            if (!string.IsNullOrEmpty(sStaffworkingtimeId))
            {
                try
                {
                    var listaffworkingtimes = (List<LM_STAFF_WORKING_TIME_DETAILS>)CMSHandler.FetchData<LM_STAFF_WORKING_TIME_DETAILS>(new LM_STAFF_WORKING_TIME_DETAILS() { STAFF_WORKING_TIME_ID = sStaffworkingtimeId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchStaffWorkingdetailsById)).DataSource.Data;
                    if (listaffworkingtimes != null && listaffworkingtimes.Count > 0)
                    {
                        objModel.STAFF_WORKING_TIME_ID = listaffworkingtimes.FirstOrDefault().STAFF_WORKING_TIME_ID;
                        objModel.STAFF_CATEGORY = listaffworkingtimes.FirstOrDefault().STAFF_CATEGORY;
                        objModel.SHIFT = listaffworkingtimes.FirstOrDefault().SHIFT;
                        objModel.TIME_IN = listaffworkingtimes.FirstOrDefault().TIME_IN;
                        objModel.TIME_OUT = listaffworkingtimes.FirstOrDefault().TIME_OUT;

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchStaffWorkingdetails", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchStaffWorkingdetails", "Payroll", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateStaffWorkingdetails(string sJsonData)
        {

            if (!string.IsNullOrEmpty(sJsonData))
            {
                try
                {

                    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                    {
                        string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                        var staffworkingdetails = JsonConvert.DeserializeObject<LM_STAFF_WORKING_TIME_DETAILS>(sJsonData);
                        if (staffworkingdetails != null)
                        {

                            var timeinformat = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { TIME = staffworkingdetails.TIME_IN }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.Converttotimeformat)).DataSource.Data;
                            var timeoutformat = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { TIME = staffworkingdetails.TIME_OUT }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.Converttotimeformat)).DataSource.Data;
                            staffworkingdetails.TIME_IN = timeinformat.FirstOrDefault().TIME;
                            staffworkingdetails.TIME_OUT = timeoutformat.FirstOrDefault().TIME;

                            resultargs = CMSHandler.SaveOrUpdate(staffworkingdetails, PayrollSQL.GetPayrollSQL((!string.IsNullOrEmpty(staffworkingdetails.STAFF_WORKING_TIME_ID)) ? PayrollSQLCommads.UpdateStaffWorkingtimes : PayrollSQLCommads.SaveStaffWorkingtimes), sAcademicYear);
                            if (resultargs.Success)
                                ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            else
                                ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SaveorUpdateStaffWorkingdetails", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("SaveorUpdateStaffWorkingdetails", "Payroll", ex.Message);
                    }
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteStaffWorkingtimes(string sStaffworkingtimeId)
        {
            if (!string.IsNullOrEmpty(sStaffworkingtimeId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new LM_STAFF_WORKING_TIME_DETAILS() { STAFF_WORKING_TIME_ID = sStaffworkingtimeId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.DeleteStaffWorkingtimes));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "DeleteStaffWorkingtimes", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        //Report of Latecomers and Faild to logout
        public ActionResult ListLateComers()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    //Fetch Leave Status
                    var lileavestatus = (List<LM_LEAVE_STATUS>)CMSHandler.FetchData<LM_LEAVE_STATUS>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchlmLeaveStatus)).DataSource.Data;
                    if (lileavestatus != null && lileavestatus.Count > 0)
                    {
                        objViewModel.LEAVE_STATUS = new SelectList(lileavestatus, Common.SUP_LI_STATUS.STATUS_ID, Common.SUP_LI_STATUS.STATUS_NAME);

                    }
                    //Fetch Shift
                    var lishift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (lishift != null && lishift.Count > 0)
                    {
                        objViewModel.SHIFT = new SelectList(lishift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListLateComers", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListLateComers", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindLateComers(STAFF_ATTENDANCE sa)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    sa.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(sa.DATE_FROM);
                    sa.DATE_TO = CommonMethods.ConvertdatetoyearFromat(sa.DATE_TO);

                    objViewModel.listaffattendance = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { DATE_FROM = sa.DATE_FROM, DATE_TO = sa.DATE_TO, SHIFT = sa.SHIFT }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchLateComersByDateandShift), sAcademicYear).DataSource.Data;
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindLateComers", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindLateComers", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindLogoutFaild(STAFF_ATTENDANCE sa)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    sa.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(sa.DATE_FROM);
                    sa.DATE_TO = CommonMethods.ConvertdatetoyearFromat(sa.DATE_TO);

                    objViewModel.listaffattendance = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { DATE_FROM = sa.DATE_FROM, DATE_TO = sa.DATE_TO }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchFaildtologoutlist), sAcademicYear).DataSource.Data;
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindLogoutFaild", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindLogoutFaild", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchLateComersDetails(string sStaffAttendanceId)
        {
            var objModel = new STAFF_ATTENDANCE();
            if (!string.IsNullOrEmpty(sStaffAttendanceId))
            {
                try
                {
                    var listaffattendance = (List<STAFF_ATTENDANCE>)CMSHandler.FetchData<STAFF_ATTENDANCE>(new STAFF_ATTENDANCE() { STF_ATT_ID = sStaffAttendanceId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchStaffAttendanceByStfAttId)).DataSource.Data;
                    if (listaffattendance != null && listaffattendance.Count > 0)
                    {
                        objModel.STF_ATT_ID = listaffattendance.FirstOrDefault().STF_ATT_ID;
                        objModel.STATUS_ID = listaffattendance.FirstOrDefault().STATUS_ID;
                        objModel.STATUS_NAME = listaffattendance.FirstOrDefault().STATUS_NAME;
                        objModel.STATUS_NAME = listaffattendance.FirstOrDefault().STATUS_NAME;

                        objModel.TIME_IN = listaffattendance.FirstOrDefault().TIME_IN;
                        objModel.TIME_OUT = listaffattendance.FirstOrDefault().TIME_OUT;
                        objModel.IN_TIME = listaffattendance.FirstOrDefault().IN_TIME;
                        objModel.OUT_TIME = listaffattendance.FirstOrDefault().OUT_TIME;


                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchLateComersDetails", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchLateComersDetails", "Payroll", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Payroll support data
        public ActionResult ListSalaryMonths()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objViewModel.lisalarymonths = (List<PR_SUP_SALARY_MONTH>)CMSHandler.FetchData<PR_SUP_SALARY_MONTH>(null, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchSalaryMonths), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListSalaryMonths", "Payroll", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListSalaryMonths", "Payroll", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchSalaryMonths(string sMonthId)
        {
            var objModel = new PR_SUP_SALARY_MONTH();
            if (!string.IsNullOrEmpty(sMonthId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var lisalarymenth = (List<PR_SUP_SALARY_MONTH>)CMSHandler.FetchData<PR_SUP_SALARY_MONTH>(new PR_SUP_SALARY_MONTH() { MONTH_ID = sMonthId}, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.FetchSalaryMonthById), sAcademicYear).DataSource.Data;
                    if (lisalarymenth != null && lisalarymenth.Count > 0)
                    {
                        objModel.MONTH_ID = lisalarymenth.FirstOrDefault().MONTH_ID;
                        objModel.MONTH_NAME= lisalarymenth.FirstOrDefault().MONTH_NAME;
                        objModel.FROM_DATE = lisalarymenth.FirstOrDefault().FROM_DATE;
                        objModel.TO_DATE = lisalarymenth.FirstOrDefault().TO_DATE;
                        
                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "FetchSalaryMonths", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateSalaryMonths(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<PR_SUP_SALARY_MONTH>(sJsonData);
                    Result.FROM_DATE = CommonMethods.DateConvertionForPayU(Result.FROM_DATE);
                    Result.TO_DATE = CommonMethods.DateConvertionForPayU(Result.TO_DATE);
                    var SaveSalarymonth = CMSHandler.SaveOrUpdate(Result, PayrollSQL.GetPayrollSQL((!string.IsNullOrEmpty(Result.MONTH_ID) ? PayrollSQLCommads.UpdateSalaryMonth : PayrollSQLCommads.SaveSalaryMonth)), sAcademicYear);
                    if (SaveSalarymonth.Success)
                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        objResult.Message = Common.Messages.FailedToSaveRecords;
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Payroll", "SaveorUpdateSalaryMonths", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Payroll", "SaveorUpdateSalaryMonths", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult DeleteSalaryMonths(string sMonthId)
        {
            if (!string.IsNullOrEmpty(sMonthId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new PR_SUP_SALARY_MONTH() { MONTH_ID = sMonthId }, PayrollSQL.GetPayrollSQL(PayrollSQLCommads.DeleteSalaryMonth));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Payroll", "DeleteSalaryMonths", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}