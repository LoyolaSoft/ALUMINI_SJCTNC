
using CMS.DAO;
using CMS.Models;
using CMS.Services;
using CMS.SQL.Examination;
using CMS.SQL.Fee;
using CMS.SQL.SupportData;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.IO;
using System.Net;
//using CMS.EaseBuzz;
using System.Text;
using System.Security.Cryptography;
using System.Web.Helpers;
using System.Security.Policy;
using Quartz.Util;
using System.Reflection.Emit;
using System.Web.UI.WebControls;
using System.Collections.ObjectModel;
using System.Net.Http;
using CMS.SQL.StaffSQL;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using CMS.SQL.PaymentGateway;
using System.Net.Http.Headers;
using CMS.SQL.Academics;
using RestSharp;

namespace CMS.Controllers
{
    public class PaymentGatewayController : Controller
    {
        // GET: PaymentGateway

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PaymentMode(string sFrequencyTypeId, string sFrequencyId, string sFeeRootId, string sAcademicyear)
        {
            FeeTransactionViewModel liFeeTransactions = new FeeTransactionViewModel();
            try
            {
                OrderIdResponse objOrderInfo = new OrderIdResponse();
                Dictionary<string, object> input = new Dictionary<string, object>();
                string cf_clientId = string.Empty;
                string cf_secret_id = string.Empty;
                string sStudentId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                Notes notes = new Notes();
                List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null)
                {
                    liFeeTransactions.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = sFrequencyTypeId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicyear).DataSource.Data;

                    if (sFrequencyTypeId == Common.FrequencyType.SemesterFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FREQUENCY_TYPE_ID = Common.FrequencyType.SemesterFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicyear).DataSource.Data;
                    if (sFrequencyTypeId == Common.FrequencyType.ConvocationFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FREQUENCY_TYPE_ID = Common.FrequencyType.ConvocationFee, FEE_ROOT_ID = Common.FeeRoot.ConvocationFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicyear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.MessFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FEE_ROOT_ID = Common.FeeRoot.MessFee, FREQUENCY_TYPE_ID = sFrequencyTypeId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FecthMessFeesByStudentIdAndFeeRootId), sAcademicyear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.HostelFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_TYPE_ID = sFrequencyTypeId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelFeeStudentAccountInfo), sAcademicyear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.ExamFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_TYPE_ID = sFrequencyTypeId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicyear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.AdmissionFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FREQUENCY_TYPE_ID = Common.FrequencyType.AdmissionFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicyear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.LibraryFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FREQUENCY_TYPE_ID = Common.FrequencyType.LibraryFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicyear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.RevaluationFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FREQUENCY_TYPE_ID = Common.FrequencyType.RevaluationFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicyear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.PhotoCopyFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FREQUENCY_TYPE_ID = Common.FrequencyType.PhotoCopyFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicyear).DataSource.Data;

                    //else if (sFrequencyTypeId == Common.FrequencyType.HostelApplicationFee)
                    //    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = Common.FrequencyType.HostelApplicationFee, FEE_ROOT_ID = Common.FeeRoot.HostelApplicationfee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountHostelFeeInfo), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;

                    //if (!string.IsNullOrEmpty(sPaymentTypeId) && sPaymentTypeId != "3")
                    //{
                    //    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { FREQUENCY_ID = sPaymentTypeId, STUDENT_ID = sStudentId, FREQUENCY_ID = Common.FrequencyType.AdmissionFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountInstalmentFeeInfo), sAcademicyear).DataSource.Data;
                    //}
                    if (studentAcountList != null && studentAcountList.Count > 0)
                    {
                        studentAcountList.FirstOrDefault().FREQUENCY_TYPE_ID = sFrequencyTypeId;
                        studentAcountList.FirstOrDefault().FEE_ROOT_ID = sFeeRootId;
                        studentAcountList.FirstOrDefault().FREQUENCY_ID = sFrequencyId;
                        studentAcountList.FirstOrDefault().ACADEMIC_YEAR = sAcademicyear;
                        studentAcountList.FirstOrDefault().FREQUENCY_TYPE_ID = sFrequencyTypeId;
                        liFeeTransactions.liFeeStudentAccount = studentAcountList;
                    }
                    //List<int> lastHeads = new List<int> { 5, 17, 18, 19, 20, 21 };

                    //// 1. Normal heads (not in lastHeads), ordered by HEAD_ID
                    //var normalHeads = studentAcountList
                    //    .Where(x => !lastHeads.Contains(Convert.ToInt32(x.HEAD_ID)))
                    //    .OrderBy(x => Convert.ToInt32(x.HEAD_ID));

                    //// 2. Last heads (in lastHeads), ordered according to the lastHeads list
                    //var tailHeads = studentAcountList
                    //    .Where(x => lastHeads.Contains(Convert.ToInt32(x.HEAD_ID)))
                    //    .OrderBy(x => lastHeads.IndexOf(Convert.ToInt32(x.HEAD_ID)));

                    //var orderedList = normalHeads.Concat(tailHeads).ToList();
                    //// 2. Set total balance
                    //decimal sbalance = Convert.ToDecimal(sAmount); // example balance
                    //decimal totalAssigned = 0;

                    //foreach (var item in orderedList.ToList()) // loop through a copy to modify original list
                    //{
                    //    if (sbalance <= 0)
                    //    {
                    //        // Remove any remaining items if balance is 0
                    //        studentAcountList.Remove(item);
                    //        continue;
                    //    }
                    //    if (item.BALANCE == "0")
                    //    {
                    //        studentAcountList.Remove(item);
                    //        continue;
                    //    }

                    //    decimal requiredAmount = Convert.ToDecimal(item.BALANCE);

                    //    if (requiredAmount <= sbalance)
                    //    {
                    //        // Full amount can be paid
                    //        item.BALANCE = requiredAmount.ToString();
                    //        sbalance -= requiredAmount;
                    //        totalAssigned += requiredAmount;
                    //    }
                    //    else
                    //    {
                    //        // Only partial amount can be paid
                    //        item.BALANCE = sbalance.ToString();
                    //        totalAssigned += sbalance;
                    //        sbalance = 0;
                    //    }
                    //}

                    liFeeTransactions.liPAYMENT_MODE_CHARGES = CalculateTransactionFee(studentAcountList, (List<FEE_PAYMENT_MODE_CHARGES>)CMSHandler.FetchData<FEE_PAYMENT_MODE_CHARGES>(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchPaymentModes)).DataSource.Data);

                    if (studentAcountList.Sum(o => decimal.Parse(o.BALANCE)) <= 0)
                    {
                        return RedirectToAction("dashboard", "dashboard");
                    }
                }
            }
            catch (Exception ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("PaymentGatewayController", "PaymentMode", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("PaymentGatewayController", "PaymentMode", ex.Message);
                }
            }
            return View(liFeeTransactions);
        }

        private List<FEE_PAYMENT_MODE_CHARGES> CalculateTransactionFee(List<FEE_STUDENT_ACCOUNT> feeStudentAccounts, List<FEE_PAYMENT_MODE_CHARGES> feePaymentModeCharges)
        {
            try
            {
                decimal totalAmount = feeStudentAccounts.Sum(o => decimal.Parse(o.BALANCE));
                feePaymentModeCharges = feePaymentModeCharges.Where(o => decimal.Parse(o.FROM_CHARGES_AMOUNT) <= totalAmount && decimal.Parse(o.TO_CHARGES_AMOUNT) >= totalAmount).ToList();
                if (feePaymentModeCharges != null && feePaymentModeCharges.Count > 0)
                {
                    foreach (var item in feePaymentModeCharges)
                    {
                        if (item.IS_PERCENTAGE_OR_FLAT == "1")
                        {
                            item.TRANSACTION_FEE = (totalAmount * decimal.Parse(item.PERCENTAGE_OR_FLAT) / 100).ToString();
                            item.GST_AMOUNT_ON_TRANSACTION_FEE = (decimal.Parse(item.TRANSACTION_FEE) * decimal.Parse(item.GST_ON_TRANSACTION_FEE) / 100).ToString();
                            item.TOTAL_AMOUNT = (totalAmount + decimal.Parse(item.GST_AMOUNT_ON_TRANSACTION_FEE) + decimal.Parse(item.TRANSACTION_FEE)).ToString("#.##");
                            item.FEE_AMOUNT = totalAmount.ToString("#.##");
                        }
                        else
                        {
                            item.TRANSACTION_FEE = item.PERCENTAGE_OR_FLAT;
                            item.GST_AMOUNT_ON_TRANSACTION_FEE = (decimal.Parse(item.TRANSACTION_FEE) * decimal.Parse(item.GST_ON_TRANSACTION_FEE) / 100).ToString();
                            item.TOTAL_AMOUNT = (totalAmount + decimal.Parse(item.GST_AMOUNT_ON_TRANSACTION_FEE) + decimal.Parse(item.TRANSACTION_FEE)).ToString();
                            item.FEE_AMOUNT = totalAmount.ToString();
                        }
                    }
                }
                else
                {
                    // no payment methods not activated 
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("PaymentGatewayController", "CalculateTransactionFee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("PaymentGatewayController", "CalculateTransactionFee", ex.Message);
                }
            }
            return feePaymentModeCharges;
        }
        public ActionResult PaymentModeForAdmin(string sFrequencyTypeId, string sFrequencyId, string sFeeRootId, string sStudentId, string sAmount, string sAcademicyear, string sHead = null, string sSelectionID = null)
        {
            FeeTransactionViewModel liFeeTransactions = new FeeTransactionViewModel();
            try
            {
                if (string.IsNullOrEmpty(sHead))
                {
                    sHead = sSelectionID;
                }
                OrderIdResponse objOrderInfo = new OrderIdResponse();
                Dictionary<string, object> input = new Dictionary<string, object>();
                string cf_clientId = string.Empty;
                string cf_secret_id = string.Empty;
                Notes notes = new Notes();
                List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
                if(string.IsNullOrEmpty(sStudentId))
                {
                    sStudentId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                }
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null)
                {
                    liFeeTransactions.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = sFrequencyTypeId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicyear).DataSource.Data;

                    if (sFrequencyTypeId == Common.FrequencyType.SemesterFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_TYPE_ID = sFrequencyTypeId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicyear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.MessFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FEE_ROOT_ID = Common.FeeRoot.MessFee, FREQUENCY_TYPE_ID = sFrequencyTypeId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FecthMessFeesByStudentIdAndFeeRootId), sAcademicyear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.HostelFee)
                    {
                        if (!string.IsNullOrEmpty(sHead) && sHead.Contains(","))
                        {
                            studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FEE_ROOT_ID = Common.FeeRoot.HostelFee, FREQUENCY_TYPE_ID = sFrequencyTypeId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelFeeStudentAccountInfoWithoutHead), sAcademicyear).DataSource.Data;
                            var selectedHeads = sHead.Split(',').ToList();
                            studentAcountList = studentAcountList.Where(s => selectedHeads.Contains(s.HEAD_ID) || selectedHeads.Contains(s.HEAD)).ToList();
                        }
                        else
                        {
                            studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { HEAD_ID = sHead, STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FEE_ROOT_ID = Common.FeeRoot.HostelFee, FREQUENCY_TYPE_ID = sFrequencyTypeId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelFeeStudentAccountInfo), sAcademicyear).DataSource.Data;
                        }
                    }
                    else if (sFrequencyTypeId == Common.FrequencyType.ExamFee)
                    {
                        var dueSummary = FeeDueHelper.GetStudentFeeDueSummary(sStudentId, sAcademicyear);
                        if (dueSummary.HasDues && !FeeDueHelper.HasActiveExamFeePermission(sStudentId, null, sAcademicyear))
                        {
                            TempData["ErrorMessage"] = "Exam fee payment is blocked due to outstanding fee dues (College/Hostel/Bus/Old Fee). Please clear your dues or contact the administration.";
                            return RedirectToAction("PayExamFee", "Examination");
                        }
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_TYPE_ID = sFrequencyTypeId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchExamStudentAccountFeeInfo), sAcademicyear).DataSource.Data;
                        if (studentAcountList == null || studentAcountList.Count == 0)
                        {
                            var res = CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAllFeeDuesByStudentId), sAcademicyear);
                            if (res != null && res.DataSource != null && res.DataSource.Data != null)
                            {
                                var allList = (List<FEE_STUDENT_ACCOUNT>)res.DataSource.Data;
                                studentAcountList = allList.Where(x => x.FREQUENCY_TYPE_ID == sFrequencyTypeId && (string.IsNullOrEmpty(sFrequencyId) || x.FREQUENCY_ID == sFrequencyId)).ToList();
                            }
                        }
                    }
                    else if (sFrequencyTypeId == Common.FrequencyType.BusFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FEE_ROOT_ID = Common.FeeRoot.BusFee, FREQUENCY_TYPE_ID = sFrequencyTypeId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusStudentAccountFeeInfo), sAcademicyear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.AdmissionFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_TYPE_ID = sFrequencyTypeId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicyear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.LibraryFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_TYPE_ID = sFrequencyTypeId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicyear).DataSource.Data;
                 
                    else if (sFrequencyTypeId == "ALL")
                    {
                        var res = CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAllFeeDuesByStudentId), sAcademicyear);
                        studentAcountList = (res != null && res.DataSource != null && res.DataSource.Data != null)
                                            ? (List<FEE_STUDENT_ACCOUNT>)res.DataSource.Data
                                            : new List<FEE_STUDENT_ACCOUNT>();
                    }

                    if (studentAcountList != null && studentAcountList.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(sHead))
                        {
                            if (sHead.Contains(","))
                            {
                                var selectedHeads = sHead.Split(',').ToList();
                                studentAcountList = studentAcountList.Where(s => selectedHeads.Contains(s.HEAD_ID) || selectedHeads.Contains(s.HEAD)).ToList();
                            }
                            else
                            {
                                studentAcountList = studentAcountList.Where(s => s.HEAD_ID == sHead).ToList();
                            }
                        }
                    }

                    if (studentAcountList != null && studentAcountList.Count > 0)
                    {
                        studentAcountList.FirstOrDefault().FREQUENCY_TYPE_ID = sFrequencyTypeId;
                    studentAcountList.FirstOrDefault().FEE_ROOT_ID = sFeeRootId;
                    studentAcountList.FirstOrDefault().FREQUENCY_ID = sFrequencyId;
                    studentAcountList.FirstOrDefault().STUDENT_ID = studentAcountList.FirstOrDefault().STUDENT_ID;
                    studentAcountList.FirstOrDefault().ACADEMIC_YEAR = sAcademicyear;
                    studentAcountList.FirstOrDefault().FREQUENCY_TYPE_ID = sFrequencyTypeId;
                    studentAcountList.FirstOrDefault().PAYMENTTYPEID = "3";
                    liFeeTransactions.liFeeStudentAccount = studentAcountList;

                    liFeeTransactions.liPAYMENT_MODE_CHARGES = CalculateTransactionFee(studentAcountList, (List<FEE_PAYMENT_MODE_CHARGES>)CMSHandler.FetchData<FEE_PAYMENT_MODE_CHARGES>(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchPaymentModes)).DataSource.Data);

                    if (studentAcountList.Sum(o => decimal.Parse(o.BALANCE)) <= 0)
                    {
                        return RedirectToAction("dashboard", "dashboard");
                    }
                    }
                }
            }
            catch (Exception ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("PaymentGatewayController", "PaymentMode", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("PaymentGatewayController", "PaymentMode", ex.Message);
                }
            }
            return View(liFeeTransactions);
        }

        public async Task<ActionResult> CFCreateOrder(string sFrequencyTypeId, string sFeeRootId, string sFrequencyId, string sAcademicYear, string sPaymentMode, string sAmount, string sSelectionID = null)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            FeeTransactionViewModel liFeeTransactions = new FeeTransactionViewModel();
            OrderIdResponse objOrderInfo = new OrderIdResponse();
            Dictionary<string, object> input = new Dictionary<string, object>();
            Notes notes = new Notes();
            string sResponse = string.Empty;
            try
            {
                string sIsPGLive = System.Configuration.ConfigurationManager.AppSettings["ISPG_LIVE"].ToString();
                string key = string.Empty;
                string secret = string.Empty;
                List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
                var USER = Session[Common.SESSION_VARIABLES.USER_ID];
                var sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    liFeeTransactions.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = sFrequencyTypeId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;

                    if (sFrequencyTypeId == Common.FrequencyType.SemesterFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_TYPE_ID = Common.FrequencyType.SemesterFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.MessFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_TYPE_ID = Common.FrequencyType.MessFee, FEE_ROOT_ID = Common.FeeRoot.MessFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FecthMessFeesByStudentIdAndFeeRootId), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.HostelFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_TYPE_ID = Common.FrequencyType.HostelFee, FEE_ROOT_ID = Common.FeeRoot.HostelFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelFeeStudentAccountInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.ExamFee)
                    {
                        var dueSummary = FeeDueHelper.GetStudentFeeDueSummary(sUserId, sAcademicYear);
                        if (dueSummary.HasDues && !FeeDueHelper.HasActiveExamFeePermission(sUserId, null, sAcademicYear))
                        {
                            return Json(new { status = false, message = "Exam fee payment is blocked due to outstanding fee dues (College/Hostel/Bus/Old Fee)." }, JsonRequestBehavior.AllowGet);
                        }
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_TYPE_ID = Common.FrequencyType.ExamFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchExamStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                        if (studentAcountList == null || studentAcountList.Count == 0)
                        {
                            studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_TYPE_ID = Common.FrequencyType.ExamFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                        }
                        if (studentAcountList == null || studentAcountList.Count == 0)
                        {
                            var res = CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAllFeeDuesByStudentId), sAcademicYear);
                            if (res != null && res.DataSource != null && res.DataSource.Data != null)
                            {
                                var allList = (List<FEE_STUDENT_ACCOUNT>)res.DataSource.Data;
                                studentAcountList = allList.Where(x => x.FREQUENCY_TYPE_ID == sFrequencyTypeId && (string.IsNullOrEmpty(sFrequencyId) || x.FREQUENCY_ID == sFrequencyId)).ToList();
                            }
                        }
                    }
                    else if (sFrequencyTypeId == Common.FrequencyType.AdmissionFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_TYPE_ID = Common.FrequencyType.AdmissionFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.LibraryFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_TYPE_ID = Common.FrequencyType.LibraryFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.ConvocationFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_TYPE_ID = Common.FrequencyType.ConvocationFee, FEE_ROOT_ID = Common.FeeRoot.ConvocationFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.Supplementary)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_TYPE_ID = Common.FrequencyType.Supplementary, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.RevaluationFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_TYPE_ID = Common.FrequencyType.RevaluationFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.Certificate)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_TYPE_ID = Common.FrequencyType.Certificate, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.Term_II)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_TYPE_ID = Common.FrequencyType.Term_II, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.PhotoCopyFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_TYPE_ID = Common.FrequencyType.PhotoCopyFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == "ALL")
                    {
                        var res = CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAllFeeDuesByStudentId), sAcademicYear);
                        studentAcountList = (res != null && res.DataSource != null && res.DataSource.Data != null)
                                            ? (List<FEE_STUDENT_ACCOUNT>)res.DataSource.Data
                                            : new List<FEE_STUDENT_ACCOUNT>();
                    }

                    //else if (sFrequencyTypeId == Common.FrequencyType.valueaddedfee)
                    //    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_TYPE_ID = Common.FrequencyType.valueaddedfee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    //if (!string.IsNullOrEmpty(sPaymentTypeId) && sPaymentTypeId != "3")
                    //{
                    //    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { INSTALMENT_ID = sPaymentTypeId, STUDENT_ID = sUserId.ToString(), FREQUENCY_ID = Common.FrequencyType.AdmissionFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountInstalmentFeeInfo), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                    //}
                    if (!string.IsNullOrEmpty(sSelectionID) && studentAcountList != null && studentAcountList.Count > 0)
                    {
                        var selectedHeads = sSelectionID.Split(',').ToList();
                        studentAcountList = studentAcountList.Where(s => selectedHeads.Contains(s.HEAD_ID) || selectedHeads.Contains(s.HEAD)).ToList();
                    }
                    liFeeTransactions.liFeeStudentAccount = studentAcountList;
                    liFeeTransactions.liFeeStudentAccount.FirstOrDefault().ACADEMIC_YEAR = sAcademicYear;
                    if (studentAcountList.Sum(o => decimal.Parse(o.BALANCE)) <= 0)
                    {
                        return RedirectToAction("dashboard", "dashboard");
                    }



                    // Charges Adding Method
                    var liPAYMENT_MODE_CHARGES = CalculateTransactionFee(studentAcountList, (List<FEE_PAYMENT_MODE_CHARGES>)CMSHandler.FetchData<FEE_PAYMENT_MODE_CHARGES>(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchPaymentModes)).DataSource.Data);

                    string fullAmount = string.Empty;
                    var Matched = liPAYMENT_MODE_CHARGES.Where(x => x.PAYMENT_MODE == sPaymentMode).ToList();
                    fullAmount = Matched.FirstOrDefault().TOTAL_AMOUNT.ToString();

                    var customer = (List<CustomerDetails>)CMSHandler.FetchData<CustomerDetails>(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchStudentInfo).Replace("?STUDENT_ID", "'" + sUserId + "'")).DataSource.Data;

                    var cfOrderExisting = (List<CF_ORDER_INFO>)CMSHandler.FetchData<CF_ORDER_INFO>(new Order_tag() { FREQUENCY_ID = sFrequencyId, USER_ID = sUserId.ToString() },
                        PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.IsOrderExistingByPaymentMode).Replace("?PAYMENT_METHODS", "'" + sPaymentMode + "'").Replace("?ACADEMIC_YEAR", sAcademicYear)).DataSource.Data;
                    if (cfOrderExisting != null)
                    {
                        if (cfOrderExisting.FirstOrDefault().order_status.ToUpper() == "PAID")
                        {
                            return Json(new { status = false, Message = "Payment is done for this transaction", PaymentList = cfOrderExisting.FirstOrDefault().payment_link }, JsonRequestBehavior.AllowGet);
                        }
                        if (cfOrderExisting != null && cfOrderExisting.Count > 0)
                        {
                            var result = await CFResponseCheck(cfOrderExisting.FirstOrDefault().order_id) as JsonResult;
                            string order_id = cfOrderExisting.FirstOrDefault().order_id;
                            // Convert to Dictionary to parse the status
                            var serializerSettings = new JsonSerializerSettings
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            };
                            string jsonStr = JsonConvert.SerializeObject(result.Data, serializerSettings);
                            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStr);
                            var CF_API_Info = (List<FEE_CF_SETTING>)CMSHandler.FetchData<FEE_CF_SETTING>(new FEE_CF_SETTING() { TRANSACTION_TYPE = Common.CF_TRANSACTION_TYPE.PAYMENT_GATEWAY, IS_LIVE = sIsPGLive }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchAPIKeyInfoByTransactionType), string.Empty).DataSource.Data;
                            string sGatewayUrl = string.Empty;
                            if (sIsPGLive == "1")
                            {
                                sGatewayUrl = "https://api.cashfree.com";
                            }
                            else
                            {
                                sGatewayUrl = "https://test.cashfree.com";
                            }
                            using (var httpClient = new HttpClient())
                            {
                                using (var request = new HttpRequestMessage(new HttpMethod("GET"), CF_API_Info.FirstOrDefault().BASE_URL + "/orders/" + order_id))
                                {
                                    request.Headers.TryAddWithoutValidation("accept", "application/json");
                                    request.Headers.TryAddWithoutValidation("x-api-version", CF_API_Info.FirstOrDefault().API_VERSION);
                                    request.Headers.TryAddWithoutValidation("x-client-id", CF_API_Info.FirstOrDefault().CLIENT_ID);
                                    request.Headers.TryAddWithoutValidation("x-client-secret", CF_API_Info.FirstOrDefault().SECRET_ID);

                                    var response = await httpClient.SendAsync(request);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        sResponse = await response.Content.ReadAsStringAsync();
                                        var updateOrderInfo = CMSHandler.SaveOrUpdate(new CF_ORDER_INFO() { FEE_CF_ORDER_ID = cfOrderExisting.FirstOrDefault().FEE_CF_ORDER_ID, CF_ORDER_RESPONSE = sResponse },
                                            PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.UpdateCFOrderResponseInfoAsJson).Replace("?fee_cf_order_id", cfOrderExisting.FirstOrDefault().FEE_CF_ORDER_ID), sAcademicYear);
                                        var orderResponse = JsonConvert.DeserializeObject<CF_ORDER_INFO>(sResponse);
                                        if (orderResponse.order_status.ToUpper() == "PAID")
                                        {
                                            orderResponse = FeeCashFreeCollection(orderResponse, cfOrderExisting.FirstOrDefault().order_id, orderResponse.order_tags.ACADEMIC_YEAR_ID, cfOrderExisting.FirstOrDefault().actual_amount);
                                            return RedirectToAction("DashBoard", "DashBoard");
                                        }
                                        else
                                        {
                                            string orderExpiryTime = orderResponse.order_expiry_time;

                                            // Parse the expiry time
                                            DateTimeOffset expiryDateTime = DateTimeOffset.Parse(orderExpiryTime);

                                            // Get the current time with offset
                                            DateTimeOffset currentDateTime = DateTimeOffset.Now;

                                            // Compare
                                            if (currentDateTime > expiryDateTime)
                                            {
                                                var Response = CMSHandler.SaveOrUpdate(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.DeleteCFOderInfo).Replace("?fee_cf_order_id", cfOrderExisting.FirstOrDefault().fee_cf_order_id), sAcademicYear);
                                                // create new order id 

                                                // Payment Spliting Method Calling
                                                liFeeTransactions.liPAYMENT_MODE_CHARGES = CalculateTransactionFee(null, (List<FEE_PAYMENT_MODE_CHARGES>)CMSHandler.FetchData<FEE_PAYMENT_MODE_CHARGES>(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchPaymentModes)).DataSource.Data);
                                                liFeeTransactions.liPAYMENT_MODE_CHARGES = liFeeTransactions.liPAYMENT_MODE_CHARGES.Where(o => o.PAYMENT_MODE == sPaymentMode).ToList();
                                                CF_OrderRequest orderRequest = new CF_OrderRequest();
                                                orderRequest.order_tags = new Order_tag();

                                                if (sIsPGLive == "1")
                                                {
                                                    orderRequest.order_splits = PaymentGatewayMethods.BindVendorSplits(studentAcountList);
                                                    if (sPaymentMode == "upi")
                                                    {
                                                        orderRequest.order_splits.Add(new OrderSplitep
                                                        {
                                                            amount = Convert.ToDecimal(5.09),
                                                            vendor_id = "TEST001"
                                                        });
                                                    }
                                                }
                                                orderRequest.customer_details = customer.FirstOrDefault();
                                                orderRequest.order_currency = "INR";
                                                orderRequest.order_id = CommonMethods.GetUniqueCodeForPaymentGateway(Common.CF_PAYMENT_GATEWAY_KEY.ORDER);
                                                orderRequest.order_amount = fullAmount.ToString();
                                                orderRequest.order_tags.FREQUENCY_ID = studentAcountList.FirstOrDefault().FREQUENCY_ID;
                                                orderRequest.order_tags.FREQUENCY_TYPE = sFrequencyTypeId;
                                                orderRequest.order_tags.INSTITUTE_CODE = liFeeTransactions.liCollegeDetails.FirstOrDefault().COLLEGE_NAME;
                                                orderRequest.order_tags.PROGRAME_OR_CLASS_ID = studentAcountList.FirstOrDefault().PROGRAMME_GROUP_ID;
                                                orderRequest.order_tags.ACADEMIC_YEAR_ID = sAcademicYear.ToString();
                                                orderRequest.order_tags.USER_ID = sUserId.ToString();
                                                orderRequest.order_meta = new OrderMeta();
                                                orderRequest.order_meta.payment_methods = sPaymentMode;
                                                orderRequest.order_meta.return_url = CF_API_Info.FirstOrDefault().RETURN_URL;
                                                orderRequest.order_meta.notify_url = CF_API_Info.FirstOrDefault().NOTIFY_URL;

                                                SaveCFOrderInfo SaveCFOrderInfo = new SaveCFOrderInfo();
                                                SaveCFOrderInfo.CF_ORDER_REQUEST = JsonConvert.SerializeObject(orderRequest);
                                                SaveCFOrderInfo.order_id = orderRequest.order_id;
                                                SaveCFOrderInfo.UDF1 = sUserId.ToString();
                                                SaveCFOrderInfo.UDF2 = sFrequencyTypeId;
                                                SaveCFOrderInfo.UDF3 = sFrequencyId;
                                                SaveCFOrderInfo.UDF4 = studentAcountList.FirstOrDefault().ROLL_NO;
                                                SaveCFOrderInfo.UDF5 = sFeeRootId;
                                                SaveCFOrderInfo.UDF6 = sAmount;
                                                SaveCFOrderInfo.UDF7 = sSelectionID;
                                                SaveCFOrderInfo.order_amount = fullAmount.ToString();
                                                SaveCFOrderInfo.actual_amount = sAmount;

                                                var saveResult = CMSHandler.SaveOrUpdate(SaveCFOrderInfo, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.SaveCFOrderInfoRequest).Replace("?AC_YEAR", sAcademicYear).Replace("?order_amount", fullAmount.ToString()).Replace("?actual_amount", sAmount)).DataSource.Data;
                                                var cfOrderInfo = (List<CF_ORDER_INFO>)CMSHandler.FetchData<CF_ORDER_INFO>(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchOrderInfoByOrderId).Replace("?AC_YEAR", sAcademicYear).Replace("?ORDER_ID", "'" + orderRequest.order_id + "'")).DataSource.Data;
                                                using (var client = new HttpClient())
                                                {
                                                    using (var request1 = new HttpRequestMessage(new System.Net.Http.HttpMethod("POST"), CF_API_Info.FirstOrDefault().BASE_URL + "/orders"))
                                                    {
                                                        request1.Headers.TryAddWithoutValidation("accept", "application/json");
                                                        request1.Headers.TryAddWithoutValidation("x-api-version", CF_API_Info.FirstOrDefault().API_VERSION);
                                                        request1.Headers.TryAddWithoutValidation("x-client-id", CF_API_Info.FirstOrDefault().CLIENT_ID);
                                                        request1.Headers.TryAddWithoutValidation("x-client-secret", CF_API_Info.FirstOrDefault().SECRET_ID);
                                                        request1.Content = new StringContent(JsonConvert.SerializeObject(orderRequest));
                                                        string s = JsonConvert.SerializeObject(orderRequest);
                                                        request1.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                                                        var response1 = await client.SendAsync(request1);
                                                        if (response.IsSuccessStatusCode)
                                                        {
                                                            sResponse = await response1.Content.ReadAsStringAsync();
                                                        }
                                                        else
                                                        {
                                                            sResponse = await response1.Content.ReadAsStringAsync();
                                                            using (ErrorLog objHandler = new ErrorLog())
                                                            {
                                                                objHandler.WriteError("PaymentGatewayController", "CFCreateOrder", "Err MSg: " + sResponse, "Query is empty!");
                                                                objHandler.WriteError("PaymentGatewayController", "CFCreateOrder", sResponse);
                                                            }
                                                            return Json(new { status = false, Message = "Sorry for inconvenience , unable to initiate this transaction", PaymentList = "" }, JsonRequestBehavior.AllowGet);
                                                        }
                                                    }
                                                }
                                                var orderResponse1 = JsonConvert.DeserializeObject<CF_ORDER_INFO>(sResponse);
                                                var updateOrderInfo1 = CMSHandler.SaveOrUpdate(new CF_ORDER_INFO() { CF_ORDER_RESPONSE = sResponse }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.UpdateCFOrderResponseInfoAsJson).Replace("?AC_YEAR", sAcademicYear).Replace("?fee_cf_order_id", "'" + cfOrderInfo.FirstOrDefault().FEE_CF_ORDER_ID + "'").Replace("?CF_ORDER_RESPONSE", "'" + sResponse + "'"));
                                                return Json(new { status = true, Message = "", PaymentList = orderResponse.payment_session_id, payment_url = orderResponse.payment_link }, JsonRequestBehavior.AllowGet);
                                            }
                                            else
                                            {
                                                return Json(new { status = true, Message = "", PaymentList = orderResponse.payment_session_id, payment_url = orderResponse.payment_link }, JsonRequestBehavior.AllowGet);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // error log 
                                    }
                                }
                            }

                            // Check status and return accordingly
                            if (json.ContainsKey("status") && json["status"].ToString().ToLower() == "success")
                            {
                                return Json(new
                                {
                                    status = false,
                                    Message = "Payment is done for this transaction",
                                    PaymentList = cfOrderExisting.FirstOrDefault().payment_session_id,
                                }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                // Not paid yet, proceed with deleting and creating new order

                                // Delete Old Order
                                var Response = CMSHandler.SaveOrUpdate(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.DeleteCFOderInfo).Replace("?fee_cf_order_id", cfOrderExisting.FirstOrDefault().fee_cf_order_id), sAcademicYear);
                                // create new order id 

                                // Payment Spliting Method Calling
                                liFeeTransactions.liPAYMENT_MODE_CHARGES = CalculateTransactionFee(null, (List<FEE_PAYMENT_MODE_CHARGES>)CMSHandler.FetchData<FEE_PAYMENT_MODE_CHARGES>(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchPaymentModes)).DataSource.Data);
                                liFeeTransactions.liPAYMENT_MODE_CHARGES = liFeeTransactions.liPAYMENT_MODE_CHARGES.Where(o => o.PAYMENT_MODE == sPaymentMode).ToList();
                                CF_OrderRequest orderRequest = new CF_OrderRequest();
                                orderRequest.order_tags = new Order_tag();

                                if (sIsPGLive == "1")
                                {
                                    orderRequest.order_splits = PaymentGatewayMethods.BindVendorSplits(studentAcountList);
                                    if (sPaymentMode == "upi")
                                    {
                                        orderRequest.order_splits.Add(new OrderSplitep
                                        {
                                            amount = Convert.ToDecimal(5.09),
                                            vendor_id = "TEST001"
                                        });
                                    }
                                }
                                orderRequest.customer_details = customer.FirstOrDefault();
                                orderRequest.order_currency = "INR";
                                orderRequest.order_id = CommonMethods.GetUniqueCodeForPaymentGateway(Common.CF_PAYMENT_GATEWAY_KEY.ORDER);
                                orderRequest.order_amount = fullAmount.ToString();
                                orderRequest.order_tags.FREQUENCY_ID = studentAcountList.FirstOrDefault().FREQUENCY_ID;
                                orderRequest.order_tags.FREQUENCY_TYPE = sFrequencyTypeId;
                                orderRequest.order_tags.INSTITUTE_CODE = liFeeTransactions.liCollegeDetails.FirstOrDefault().COLLEGE_NAME;
                                orderRequest.order_tags.PROGRAME_OR_CLASS_ID = studentAcountList.FirstOrDefault().PROGRAMME_GROUP_ID;
                                orderRequest.order_tags.ACADEMIC_YEAR_ID = sAcademicYear.ToString();
                                orderRequest.order_tags.USER_ID = sUserId.ToString();
                                orderRequest.order_meta = new OrderMeta();
                                orderRequest.order_meta.payment_methods = sPaymentMode;
                                orderRequest.order_meta.return_url = CF_API_Info.FirstOrDefault().RETURN_URL;
                                orderRequest.order_meta.notify_url = CF_API_Info.FirstOrDefault().NOTIFY_URL;

                                SaveCFOrderInfo SaveCFOrderInfo = new SaveCFOrderInfo();
                                SaveCFOrderInfo.CF_ORDER_REQUEST = JsonConvert.SerializeObject(orderRequest);
                                SaveCFOrderInfo.order_id = orderRequest.order_id;
                                SaveCFOrderInfo.UDF1 = sUserId.ToString();
                                SaveCFOrderInfo.UDF2 = sFrequencyTypeId;
                                SaveCFOrderInfo.UDF3 = sFrequencyId;
                                SaveCFOrderInfo.UDF4 = studentAcountList.FirstOrDefault().ROLL_NO;
                                SaveCFOrderInfo.UDF5 = sFeeRootId;
                                SaveCFOrderInfo.UDF6 = sAmount;
                                SaveCFOrderInfo.UDF7 = sSelectionID;
                                SaveCFOrderInfo.order_amount = fullAmount.ToString();
                                SaveCFOrderInfo.actual_amount = sAmount;

                                var saveResult = CMSHandler.SaveOrUpdate(SaveCFOrderInfo, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.SaveCFOrderInfoRequest).Replace("?AC_YEAR", sAcademicYear).Replace("?order_amount", fullAmount.ToString()).Replace("?actual_amount", sAmount)).DataSource.Data;
                                var cfOrderInfo = (List<CF_ORDER_INFO>)CMSHandler.FetchData<CF_ORDER_INFO>(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchOrderInfoByOrderId).Replace("?AC_YEAR", sAcademicYear).Replace("?ORDER_ID", "'" + orderRequest.order_id + "'")).DataSource.Data;
                                using (var client = new HttpClient())
                                {
                                    using (var request = new HttpRequestMessage(new System.Net.Http.HttpMethod("POST"), CF_API_Info.FirstOrDefault().BASE_URL + "/orders"))
                                    {
                                        request.Headers.TryAddWithoutValidation("accept", "application/json");
                                        request.Headers.TryAddWithoutValidation("x-api-version", CF_API_Info.FirstOrDefault().API_VERSION);
                                        request.Headers.TryAddWithoutValidation("x-client-id", CF_API_Info.FirstOrDefault().CLIENT_ID);
                                        request.Headers.TryAddWithoutValidation("x-client-secret", CF_API_Info.FirstOrDefault().SECRET_ID);
                                        request.Content = new StringContent(JsonConvert.SerializeObject(orderRequest));
                                        string s = JsonConvert.SerializeObject(orderRequest);
                                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                                        var response = await client.SendAsync(request);
                                        if (response.IsSuccessStatusCode)
                                        {
                                            sResponse = await response.Content.ReadAsStringAsync();
                                        }
                                        else
                                        {
                                            sResponse = await response.Content.ReadAsStringAsync();
                                            using (ErrorLog objHandler = new ErrorLog())
                                            {
                                                objHandler.WriteError("PaymentGatewayController", "CFCreateOrder", "Err MSg: " + sResponse, "Query is empty!");
                                                objHandler.WriteError("PaymentGatewayController", "CFCreateOrder", sResponse);
                                            }
                                            return Json(new { status = false, Message = "Sorry for inconvenience , unable to initiate this transaction", PaymentList = "" }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                }
                                var orderResponse = JsonConvert.DeserializeObject<CF_ORDER_INFO>(sResponse);
                                var updateOrderInfo = CMSHandler.SaveOrUpdate(new CF_ORDER_INFO() { CF_ORDER_RESPONSE = sResponse }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.UpdateCFOrderResponseInfoAsJson).Replace("?AC_YEAR", sAcademicYear).Replace("?fee_cf_order_id", "'" + cfOrderInfo.FirstOrDefault().FEE_CF_ORDER_ID + "'").Replace("?CF_ORDER_RESPONSE", "'" + sResponse + "'"));
                                return Json(new { status = true, Message = "", PaymentList = orderResponse.payment_session_id, payment_url = orderResponse.payment_link }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            // Delete Old Order
                            var Response = CMSHandler.SaveOrUpdate(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.DeleteCFOderInfo).Replace("?fee_cf_order_id", cfOrderExisting.FirstOrDefault().fee_cf_order_id), sAcademicYear);
                            //create new order id 
                            var CF_API_Info = (List<FEE_CF_SETTING>)CMSHandler.FetchData<FEE_CF_SETTING>(new FEE_CF_SETTING() { TRANSACTION_TYPE = Common.CF_TRANSACTION_TYPE.PAYMENT_GATEWAY, IS_LIVE = sIsPGLive },
                           PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchAPIKeyInfoByTransactionType)).DataSource.Data;
                            // Payment Spliting Method Calling
                            liFeeTransactions.liPAYMENT_MODE_CHARGES = CalculateTransactionFee(null, (List<FEE_PAYMENT_MODE_CHARGES>)CMSHandler.FetchData<FEE_PAYMENT_MODE_CHARGES>(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchPaymentModes)).DataSource.Data);
                            liFeeTransactions.liPAYMENT_MODE_CHARGES = liFeeTransactions.liPAYMENT_MODE_CHARGES.Where(o => o.PAYMENT_MODE == sPaymentMode).ToList();
                            CF_OrderRequest orderRequest = new CF_OrderRequest();
                            orderRequest.order_tags = new Order_tag();

                            if (sIsPGLive == "1")
                            {
                                orderRequest.order_splits = PaymentGatewayMethods.BindVendorSplits(studentAcountList);
                                if (sPaymentMode == "upi")
                                {
                                    orderRequest.order_splits.Add(new OrderSplitep
                                    {
                                        amount = Convert.ToDecimal(5.09),
                                        vendor_id = "TEST001"
                                    });
                                }
                            }
                            orderRequest.customer_details = customer.FirstOrDefault();
                            orderRequest.order_currency = "INR";
                            orderRequest.order_id = CommonMethods.GetUniqueCodeForPaymentGateway(Common.CF_PAYMENT_GATEWAY_KEY.ORDER);
                            orderRequest.order_amount = fullAmount.ToString();
                            orderRequest.order_tags.FREQUENCY_ID = studentAcountList.FirstOrDefault().FREQUENCY_ID;
                            orderRequest.order_tags.FREQUENCY_TYPE = sFrequencyTypeId;
                            orderRequest.order_tags.INSTITUTE_CODE = liFeeTransactions.liCollegeDetails.FirstOrDefault().COLLEGE_NAME;
                            orderRequest.order_tags.PROGRAME_OR_CLASS_ID = studentAcountList.FirstOrDefault().PROGRAMME_GROUP_ID;
                            orderRequest.order_tags.ACADEMIC_YEAR_ID = sAcademicYear.ToString();
                            orderRequest.order_tags.USER_ID = sUserId.ToString();
                            orderRequest.order_meta = new OrderMeta();
                            orderRequest.order_meta.payment_methods = sPaymentMode;
                            orderRequest.order_meta.return_url = CF_API_Info.FirstOrDefault().RETURN_URL;
                            orderRequest.order_meta.notify_url = CF_API_Info.FirstOrDefault().NOTIFY_URL;

                            SaveCFOrderInfo SaveCFOrderInfo = new SaveCFOrderInfo();
                            SaveCFOrderInfo.CF_ORDER_REQUEST = JsonConvert.SerializeObject(orderRequest);
                            SaveCFOrderInfo.order_id = orderRequest.order_id;
                            SaveCFOrderInfo.UDF1 = JsonConvert.SerializeObject(orderRequest);
                            SaveCFOrderInfo.UDF1 = sUserId.ToString();
                            SaveCFOrderInfo.UDF2 = sFrequencyTypeId;
                            SaveCFOrderInfo.UDF3 = sFrequencyId;
                            SaveCFOrderInfo.UDF4 = studentAcountList.FirstOrDefault().ROLL_NO;
                            SaveCFOrderInfo.UDF5 = sFeeRootId;
                            SaveCFOrderInfo.UDF6 = sAmount;
                            SaveCFOrderInfo.UDF7 = sSelectionID;
                            SaveCFOrderInfo.order_amount = fullAmount.ToString();
                            SaveCFOrderInfo.actual_amount = sAmount;

                            var saveResult = CMSHandler.SaveOrUpdate(SaveCFOrderInfo, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.SaveCFOrderInfoRequest).Replace("?AC_YEAR", sAcademicYear).Replace("?order_amount", fullAmount.ToString()).Replace("?actual_amount", sAmount)).DataSource.Data;
                            var cfOrderInfo = (List<CF_ORDER_INFO>)CMSHandler.FetchData<CF_ORDER_INFO>(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchOrderInfoByOrderId).Replace("?AC_YEAR", sAcademicYear).Replace("?ORDER_ID", "'" + orderRequest.order_id + "'")).DataSource.Data;
                            using (var client = new HttpClient())
                            {
                                using (var request = new HttpRequestMessage(new System.Net.Http.HttpMethod("POST"), CF_API_Info.FirstOrDefault().BASE_URL + "/orders"))
                                {
                                    request.Headers.TryAddWithoutValidation("accept", "application/json");
                                    request.Headers.TryAddWithoutValidation("x-api-version", CF_API_Info.FirstOrDefault().API_VERSION);
                                    request.Headers.TryAddWithoutValidation("x-client-id", CF_API_Info.FirstOrDefault().CLIENT_ID);
                                    request.Headers.TryAddWithoutValidation("x-client-secret", CF_API_Info.FirstOrDefault().SECRET_ID);
                                    request.Content = new StringContent(JsonConvert.SerializeObject(orderRequest));
                                    string s = JsonConvert.SerializeObject(orderRequest);
                                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                                    var response = await client.SendAsync(request);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        sResponse = await response.Content.ReadAsStringAsync();
                                    }
                                    else
                                    {
                                        sResponse = await response.Content.ReadAsStringAsync();
                                        using (ErrorLog objHandler = new ErrorLog())
                                        {
                                            objHandler.WriteError("PaymentGatewayController", "CFCreateOrder", "Err MSg: " + sResponse, "Query is empty!");
                                            objHandler.WriteError("PaymentGatewayController", "CFCreateOrder", sResponse);
                                        }
                                        return Json(new { status = false, Message = "Sorry for inconvenience , unable to initiate this transaction", PaymentList = "" }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                            var orderResponse = JsonConvert.DeserializeObject<CF_ORDER_INFO>(sResponse);
                            var updateOrderInfo = CMSHandler.SaveOrUpdate(new CF_ORDER_INFO() { CF_ORDER_RESPONSE = sResponse }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.UpdateCFOrderResponseInfoAsJson).Replace("?AC_YEAR", sAcademicYear).Replace("?fee_cf_order_id", "'" + cfOrderInfo.FirstOrDefault().FEE_CF_ORDER_ID + "'").Replace("?CF_ORDER_RESPONSE", "'" + sResponse + "'"));
                            return Json(new { status = true, Message = "", PaymentList = orderResponse.payment_session_id, payment_url = orderResponse.payment_link }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var CF_API_Info = (List<FEE_CF_SETTING>)CMSHandler.FetchData<FEE_CF_SETTING>(new FEE_CF_SETTING() { TRANSACTION_TYPE = Common.CF_TRANSACTION_TYPE.PAYMENT_GATEWAY, IS_LIVE = sIsPGLive },
                            PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchAPIKeyInfoByTransactionType)).DataSource.Data;
                        // Payment Spliting Method Calling
                        liFeeTransactions.liPAYMENT_MODE_CHARGES = CalculateTransactionFee(null, (List<FEE_PAYMENT_MODE_CHARGES>)CMSHandler.FetchData<FEE_PAYMENT_MODE_CHARGES>(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchPaymentModes)).DataSource.Data);
                        liFeeTransactions.liPAYMENT_MODE_CHARGES = liFeeTransactions.liPAYMENT_MODE_CHARGES.Where(o => o.PAYMENT_MODE == sPaymentMode).ToList();
                        CF_OrderRequest orderRequest = new CF_OrderRequest();
                        orderRequest.order_tags = new Order_tag();

                        if (sIsPGLive == "1")
                        {
                            orderRequest.order_splits = PaymentGatewayMethods.BindVendorSplits(studentAcountList);
                            if (sPaymentMode == "upi")
                            {
                                orderRequest.order_splits.Add(new OrderSplitep
                                {
                                    amount = Convert.ToDecimal(5.09),
                                    vendor_id = "TEST001"
                                });
                            }
                        }
                        orderRequest.customer_details = customer.FirstOrDefault();
                        orderRequest.order_currency = "INR";
                        orderRequest.order_id = CommonMethods.GetUniqueCodeForPaymentGateway(Common.CF_PAYMENT_GATEWAY_KEY.ORDER);
                        orderRequest.order_amount = fullAmount.ToString();
                        orderRequest.order_tags.FREQUENCY_ID = studentAcountList.FirstOrDefault().FREQUENCY_ID;
                        orderRequest.order_tags.FREQUENCY_TYPE = sFrequencyTypeId;
                        orderRequest.order_tags.INSTITUTE_CODE = liFeeTransactions.liCollegeDetails.FirstOrDefault().COLLEGE_NAME;
                        orderRequest.order_tags.PROGRAME_OR_CLASS_ID = studentAcountList.FirstOrDefault().PROGRAMME_GROUP_ID;
                        orderRequest.order_tags.ACADEMIC_YEAR_ID = sAcademicYear.ToString();
                        orderRequest.order_tags.USER_ID = sUserId.ToString();
                        orderRequest.order_meta = new OrderMeta();
                        orderRequest.order_meta.payment_methods = sPaymentMode;
                        orderRequest.order_meta.return_url = CF_API_Info.FirstOrDefault().RETURN_URL;

                        orderRequest.order_meta.notify_url = CF_API_Info.FirstOrDefault().NOTIFY_URL;

                        SaveCFOrderInfo SaveCFOrderInfo = new SaveCFOrderInfo();
                        SaveCFOrderInfo.CF_ORDER_REQUEST = JsonConvert.SerializeObject(orderRequest);
                        SaveCFOrderInfo.order_id = orderRequest.order_id;
                        SaveCFOrderInfo.UDF1 = sUserId.ToString();
                        SaveCFOrderInfo.UDF2 = sFrequencyTypeId;
                        SaveCFOrderInfo.UDF3 = sFrequencyId;
                        SaveCFOrderInfo.UDF4 = studentAcountList.FirstOrDefault().ROLL_NO;
                        SaveCFOrderInfo.UDF5 = sFeeRootId;
                        SaveCFOrderInfo.UDF6 = sAmount;
                        SaveCFOrderInfo.UDF7 = sSelectionID;
                        SaveCFOrderInfo.order_amount = fullAmount.ToString();
                        SaveCFOrderInfo.actual_amount = sAmount;

                        var saveResult = CMSHandler.SaveOrUpdate(SaveCFOrderInfo, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.SaveCFOrderInfoRequest).Replace("?AC_YEAR", sAcademicYear).Replace("?order_amount", fullAmount.ToString()).Replace("?actual_amount", sAmount)).DataSource.Data;
                        var cfOrderInfo = (List<CF_ORDER_INFO>)CMSHandler.FetchData<CF_ORDER_INFO>(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchOrderInfoByOrderId).Replace("?AC_YEAR", sAcademicYear).Replace("?ORDER_ID", "'" + orderRequest.order_id + "'")).DataSource.Data;
                        using (var client = new HttpClient())
                        {
                            using (var request = new HttpRequestMessage(new HttpMethod("POST"), CF_API_Info.FirstOrDefault().BASE_URL + "/orders"))
                            {
                                request.Headers.TryAddWithoutValidation("accept", "application/json");
                                request.Headers.TryAddWithoutValidation("x-api-version", CF_API_Info.FirstOrDefault().API_VERSION);
                                request.Headers.TryAddWithoutValidation("x-client-id", CF_API_Info.FirstOrDefault().CLIENT_ID);
                                request.Headers.TryAddWithoutValidation("x-client-secret", CF_API_Info.FirstOrDefault().SECRET_ID);
                                request.Content = new StringContent(JsonConvert.SerializeObject(orderRequest));
                                string s = JsonConvert.SerializeObject(orderRequest);
                                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                                var response = await client.SendAsync(request);
                                if (response.IsSuccessStatusCode)
                                {
                                    sResponse = await response.Content.ReadAsStringAsync();
                                }
                                else
                                {
                                    sResponse = await response.Content.ReadAsStringAsync();
                                    using (ErrorLog objHandler = new ErrorLog())
                                    {
                                        objHandler.WriteError("PaymentGatewayController", "CFCreateOrder", "Err MSg: " + sResponse, "Query is empty!");
                                        objHandler.WriteError("PaymentGatewayController", "CFCreateOrder", sResponse);
                                    }
                                    return Json(new { status = false, Message = "Sorry for inconvenience , unable to initiate this transaction", PaymentList = "" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                        var orderResponse = JsonConvert.DeserializeObject<CF_ORDER_INFO>(sResponse);
                        var updateOrderInfo = CMSHandler.SaveOrUpdate(new CF_ORDER_INFO() { CF_ORDER_RESPONSE = sResponse }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.UpdateCFOrderResponseInfoAsJson).Replace("?AC_YEAR", sAcademicYear).Replace("?fee_cf_order_id", "'" + cfOrderInfo.FirstOrDefault().FEE_CF_ORDER_ID + "'").Replace("?CF_ORDER_RESPONSE", "'" + sResponse + "'"));
                        return Json(new { status = true, Message = "", PaymentList = orderResponse.payment_session_id, payment_url = orderResponse.payment_link }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("PaymentGatewayController", "CFCreateOrder", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("PaymentGatewayController", "CFCreateOrder", ex.Message);
                }
                return Json(new { status = false, Message = "Sorry for inconvenience , unable to initiate this transaction", PaymentList = "" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, Message = "Server is busy ,Please Try later", PaymentList = "" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CFReturnURL(string order_id)
        
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            string sAcademicYear = string.Empty;
            string sResponse = string.Empty;
            string sSplitResponse = string.Empty;
            string sGatewayUrl = string.Empty;
            List<CF_ORDER_INFO> cfOrderInfo = new List<CF_ORDER_INFO>();

            string sIsPGLive = System.Configuration.ConfigurationManager.AppSettings["ISPG_LIVE"].ToString();
            if (sIsPGLive == "2")
            {
                sGatewayUrl = "https://api.cashfree.com";
            }
            else
            {
                sGatewayUrl = "https://test.cashfree.com";
            }
            if (order_id != null)
            {
                var CF_API_Info = (List<FEE_CF_SETTING>)CMSHandler.FetchData<FEE_CF_SETTING>(new FEE_CF_SETTING() { TRANSACTION_TYPE = Common.CF_TRANSACTION_TYPE.PAYMENT_GATEWAY, IS_LIVE = sIsPGLive }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchAPIKeyInfoByTransactionType), string.Empty).DataSource.Data;

                var liAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchAcademicYearInfo), string.Empty).DataSource.Data;

                if (liAcademicYear != null && liAcademicYear.Count > 0)
                {
                    foreach (var acyear in liAcademicYear)
                    {
                        var OrderInfo = (List<CF_ORDER_INFO>)CMSHandler.FetchData<CF_ORDER_INFO>(new CF_ORDER_INFO() { order_id = order_id }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchOrderInfoByOrderId), acyear.AC_YEAR).DataSource.Data;
                        if (OrderInfo != null && OrderInfo.Count > 0)
                        {
                            cfOrderInfo.AddRange(OrderInfo);
                            sAcademicYear = acyear.AC_YEAR;
                        }
                    }
                }

                if (cfOrderInfo != null && cfOrderInfo.Count > 0)
                {
                    if (!string.IsNullOrEmpty(cfOrderInfo.FirstOrDefault().CF_ORDER_RESPONSE))
                    {
                        var orderResponseExiting = JsonConvert.DeserializeObject<CF_ORDER_INFO>(cfOrderInfo.FirstOrDefault().CF_ORDER_RESPONSE);
                        if (orderResponseExiting != null && orderResponseExiting.order_status != null && orderResponseExiting.order_status.ToUpper() == "PAID")
                        {
                            return RedirectToAction("PaymentSuccess", new { order_id = order_id });
                        }
                    }
                }

                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), CF_API_Info.FirstOrDefault().BASE_URL + "/orders/" + order_id))
                    {
                        request.Headers.TryAddWithoutValidation("accept", "application/json");
                        request.Headers.TryAddWithoutValidation("x-api-version", CF_API_Info.FirstOrDefault().API_VERSION);
                        request.Headers.TryAddWithoutValidation("x-client-id", CF_API_Info.FirstOrDefault().CLIENT_ID);
                        request.Headers.TryAddWithoutValidation("x-client-secret", CF_API_Info.FirstOrDefault().SECRET_ID);

                        var response = await httpClient.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            sResponse = await response.Content.ReadAsStringAsync();
                            var updateOrderInfo = CMSHandler.SaveOrUpdate(new CF_ORDER_INFO() { FEE_CF_ORDER_ID = cfOrderInfo.FirstOrDefault().FEE_CF_ORDER_ID, CF_ORDER_RESPONSE = sResponse },
                                PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.UpdateCFOrderResponseInfoAsJson).Replace("?fee_cf_order_id", cfOrderInfo.FirstOrDefault().FEE_CF_ORDER_ID), sAcademicYear);

                        }
                        else
                        {
                            // error log missing
                        }
                    }
                }

                var orderResponse = JsonConvert.DeserializeObject<CF_ORDER_INFO>(sResponse);
                if (orderResponse != null)
                {
                    using (var httpClient = new HttpClient())
                    {
                        using (var request = new HttpRequestMessage(new HttpMethod("GET"), sGatewayUrl + "/api/v2/easy-split/orders/" + order_id))
                        {
                            request.Headers.TryAddWithoutValidation("accept", "application/json");
                            //  request.Headers.TryAddWithoutValidation("x-api-version", CF_API_Info.FirstOrDefault().API_VERSION);
                            request.Headers.TryAddWithoutValidation("x-client-id", CF_API_Info.FirstOrDefault().CLIENT_ID);
                            request.Headers.TryAddWithoutValidation("x-client-secret", CF_API_Info.FirstOrDefault().SECRET_ID);

                            var response = await httpClient.SendAsync(request);
                            if (response.IsSuccessStatusCode)
                            {
                                sSplitResponse = await response.Content.ReadAsStringAsync();
                                var saveSplitInfo = CMSHandler.SaveOrUpdate(new FEE_CF_SPLIT_INFO_BY_ORDER() { CF_ORDER_ID = order_id, CF_SPLIT_INFO = sSplitResponse }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.SaveSplitInfo), string.Empty, false);
                            }
                            else
                            {
                                //error log
                            }
                        }
                    }
                    //var orderSplitResponse = JsonConvert.DeserializeObject<CF_SPLIT_RESPONSE_BY_ORDER>(sSplitResponse);

                    if (orderResponse.order_status != null && orderResponse.order_status.ToUpper() == "PAID")
                    {
                        orderResponse = FeeCashFreeCollection(orderResponse, cfOrderInfo.FirstOrDefault().order_id, orderResponse.order_tags.ACADEMIC_YEAR_ID, cfOrderInfo.FirstOrDefault().actual_amount);
                    }
                }
                if (!string.IsNullOrEmpty(order_id))
                {
                    return RedirectToAction("PaymentSuccess", new { order_id = order_id });
                }
                else
                {
                    return RedirectToAction("DashBoard", "DashBoard");
                }

            }
            return View(new CF_ORDER_INFO());
        }

        public async Task<ActionResult> CFOrderResponse(string order_id)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            string sResponse = string.Empty;
            string sSplitResponse = string.Empty;
            string sAcademicYear = string.Empty;
            string sGatewayUrl = System.Configuration.ConfigurationManager.AppSettings["ISPG_LIVE"] == "2"
                                 ? "https://api.cashfree.com"
                                 : "https://test.cashfree.com";

            if (string.IsNullOrEmpty(order_id))
            {
                TempData["Message"] = "Invalid Order ID";
                return RedirectToAction("FeePaidByAdmin", "Fee");
            }

            var CF_API_Info = (List<FEE_CF_SETTING>)CMSHandler.FetchData<FEE_CF_SETTING>(
                new FEE_CF_SETTING() { TRANSACTION_TYPE = Common.CF_TRANSACTION_TYPE.PAYMENT_GATEWAY, IS_LIVE = System.Configuration.ConfigurationManager.AppSettings["ISPG_LIVE"] },
                PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchAPIKeyInfoByTransactionType), string.Empty).DataSource.Data;

            var liAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(
                null, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchAcademicYearInfo), string.Empty).DataSource.Data;

            List<CF_ORDER_INFO> cfOrderInfo = new List<CF_ORDER_INFO>();

            foreach (var acYear in liAcademicYear)
            {
                var orderList = (List<CF_ORDER_INFO>)CMSHandler.FetchData<CF_ORDER_INFO>(
                    new CF_ORDER_INFO() { order_id = order_id },
                    PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchOrderInfoByOrderId),
                    acYear.AC_YEAR).DataSource.Data;

                if (orderList != null && orderList.Count > 0)
                {
                    cfOrderInfo.AddRange(orderList);
                }
            }

            if (cfOrderInfo.Count == 0)
            {
                TempData["Message"] = "Order not found.";
                return RedirectToAction("FeePaidByAdmin", "Fee");
            }

            var existingResponse = cfOrderInfo.FirstOrDefault().CF_ORDER_RESPONSE;
            if (!string.IsNullOrEmpty(existingResponse))
            {
                var parsed = JsonConvert.DeserializeObject<CF_ORDER_INFO>(existingResponse);
                if (parsed?.order_status?.ToUpper() == "PAID")
                {
                    TempData["Message"] = "Payment already completed.";
                    return RedirectToAction("FeePaidByAdmin", "Fee");
                }

                dynamic k = JsonConvert.DeserializeObject(existingResponse);
                sAcademicYear = k.order_tags?.ACADEMIC_YEAR_ID;
            }

            // Order Status from CF
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, CF_API_Info.FirstOrDefault().BASE_URL + "/orders/" + order_id);
                request.Headers.TryAddWithoutValidation("accept", "application/json");
                request.Headers.TryAddWithoutValidation("x-api-version", CF_API_Info.FirstOrDefault().API_VERSION);
                request.Headers.TryAddWithoutValidation("x-client-id", CF_API_Info.FirstOrDefault().CLIENT_ID);
                request.Headers.TryAddWithoutValidation("x-client-secret", CF_API_Info.FirstOrDefault().SECRET_ID);

                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    sResponse = await response.Content.ReadAsStringAsync();
                    CMSHandler.SaveOrUpdate(new CF_ORDER_INFO()
                    {
                        FEE_CF_ORDER_ID = cfOrderInfo.FirstOrDefault().FEE_CF_ORDER_ID,
                        CF_ORDER_RESPONSE = sResponse
                    }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.UpdateCFOrderResponseInfoAsJson)
                       .Replace("?fee_cf_order_id", cfOrderInfo.FirstOrDefault().FEE_CF_ORDER_ID), sAcademicYear);
                }
            }

            var orderResponse = JsonConvert.DeserializeObject<CF_ORDER_INFO>(sResponse);

            // Save Easy Split
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, sGatewayUrl + "/api/v2/easy-split/orders/" + order_id);
                request.Headers.TryAddWithoutValidation("accept", "application/json");
                request.Headers.TryAddWithoutValidation("x-client-id", CF_API_Info.FirstOrDefault().CLIENT_ID);
                request.Headers.TryAddWithoutValidation("x-client-secret", CF_API_Info.FirstOrDefault().SECRET_ID);

                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    sSplitResponse = await response.Content.ReadAsStringAsync();
                    CMSHandler.SaveOrUpdate(new FEE_CF_SPLIT_INFO_BY_ORDER()
                    {
                        CF_ORDER_ID = order_id,
                        CF_SPLIT_INFO = sSplitResponse
                    }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.SaveSplitInfo), string.Empty, false);
                }
            }

            if (orderResponse.order_status.ToUpper() == "PAID")
            {
                orderResponse = FeeCashFreeCollection(orderResponse, cfOrderInfo.FirstOrDefault().order_id, orderResponse.order_tags.ACADEMIC_YEAR_ID, cfOrderInfo.FirstOrDefault().actual_amount);
                TempData["Message"] = "Payment Success!";
                return RedirectToAction("FeePaidByAdmin", "Fee");
            }

            TempData["Message"] = "Payment is not completed yet.";
            return RedirectToAction("FeePaidByAdmin", "Fee");
        }

        public async Task<JsonResult> CFResponseCheck(string order_id)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            string sResponse = string.Empty;
            string sSplitResponse = string.Empty;
            string sAcademicYear = string.Empty;
            string sGatewayUrl = System.Configuration.ConfigurationManager.AppSettings["ISPG_LIVE"] == "2"
                                 ? "https://api.cashfree.com"
                                 : "https://test.cashfree.com";

            if (string.IsNullOrEmpty(order_id))
            {
                TempData["Message"] = "Invalid Order ID";
                //return RedirectToAction("DashboardIndex", "Dashboard");
            }

            var CF_API_Info = (List<FEE_CF_SETTING>)CMSHandler.FetchData<FEE_CF_SETTING>(
                new FEE_CF_SETTING() { TRANSACTION_TYPE = Common.CF_TRANSACTION_TYPE.PAYMENT_GATEWAY, IS_LIVE = System.Configuration.ConfigurationManager.AppSettings["ISPG_LIVE"] },
                PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchAPIKeyInfoByTransactionType), string.Empty).DataSource.Data;

            var liAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(
                null, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchAcademicYearInfo), string.Empty).DataSource.Data;

            List<CF_ORDER_INFO> cfOrderInfo = new List<CF_ORDER_INFO>();

            foreach (var acYear in liAcademicYear)
            {
                var orderList = (List<CF_ORDER_INFO>)CMSHandler.FetchData<CF_ORDER_INFO>(
                    new CF_ORDER_INFO() { order_id = order_id },
                    PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchOrderInfoByOrderId),
                    acYear.AC_YEAR).DataSource.Data;

                if (orderList != null && orderList.Count > 0)
                {
                    cfOrderInfo.AddRange(orderList);
                }
            }

            if (cfOrderInfo.Count == 0)
            {
                TempData["Message"] = "Order not found.";
                //return RedirectToAction("DashboardIndex", "Dashboard");
            }

            var existingResponse = cfOrderInfo.FirstOrDefault().CF_ORDER_RESPONSE;
            if (!string.IsNullOrEmpty(existingResponse))
            {
                var parsed = JsonConvert.DeserializeObject<CF_ORDER_INFO>(existingResponse);
                if (parsed?.order_status?.ToUpper() == "PAID")
                {
                    TempData["Message"] = "Payment already completed.";
                    //return RedirectToAction("DashboardIndex", "Dashboard");
                }

                dynamic k = JsonConvert.DeserializeObject(existingResponse);
                sAcademicYear = k.order_tags?.ACADEMIC_YEAR_ID;
            }

            // Order Status from CF
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, CF_API_Info.FirstOrDefault().BASE_URL + "/orders/" + order_id);
                request.Headers.TryAddWithoutValidation("accept", "application/json");
                request.Headers.TryAddWithoutValidation("x-api-version", CF_API_Info.FirstOrDefault().API_VERSION);
                request.Headers.TryAddWithoutValidation("x-client-id", CF_API_Info.FirstOrDefault().CLIENT_ID);
                request.Headers.TryAddWithoutValidation("x-client-secret", CF_API_Info.FirstOrDefault().SECRET_ID);

                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    sResponse = await response.Content.ReadAsStringAsync();
                    CMSHandler.SaveOrUpdate(new CF_ORDER_INFO()
                    {
                        FEE_CF_ORDER_ID = cfOrderInfo.FirstOrDefault().FEE_CF_ORDER_ID,
                        CF_ORDER_RESPONSE = sResponse
                    }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.UpdateCFOrderResponseInfoAsJson)
                       .Replace("?fee_cf_order_id", cfOrderInfo.FirstOrDefault().FEE_CF_ORDER_ID), sAcademicYear);
                }
            }

            var orderResponse = JsonConvert.DeserializeObject<CF_ORDER_INFO>(sResponse);

            // Save Easy Split
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, sGatewayUrl + "/api/v2/easy-split/orders/" + order_id);
                request.Headers.TryAddWithoutValidation("accept", "application/json");
                request.Headers.TryAddWithoutValidation("x-client-id", CF_API_Info.FirstOrDefault().CLIENT_ID);
                request.Headers.TryAddWithoutValidation("x-client-secret", CF_API_Info.FirstOrDefault().SECRET_ID);

                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    sSplitResponse = await response.Content.ReadAsStringAsync();
                    CMSHandler.SaveOrUpdate(new FEE_CF_SPLIT_INFO_BY_ORDER()
                    {
                        CF_ORDER_ID = order_id,
                        CF_SPLIT_INFO = sSplitResponse
                    }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.SaveSplitInfo), string.Empty, false);
                }
            }

            if (orderResponse.order_status.ToUpper() == "PAID")
            {
                orderResponse = FeeCashFreeCollection(orderResponse, cfOrderInfo.FirstOrDefault().order_id, orderResponse.order_tags.ACADEMIC_YEAR_ID, cfOrderInfo.FirstOrDefault().actual_amount);

                return Json(new
                {
                    status = true,
                    message = "Payment Success!",
                    redirectUrl = Url.Action("DashboardIndex", "Dashboard")
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    status = false,
                    message = "Payment is not completed yet.",
                    redirectUrl = Url.Action("DashboardIndex", "Dashboard")
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public CF_ORDER_INFO FeeCashFreeCollection(CF_ORDER_INFO objPaymentInfo, string sCFOrderId, string sAcademicYear, string sAmount)
        {
            try
            {
                string sStudentId = objPaymentInfo.order_tags.USER_ID;
                string sFrequencyId = objPaymentInfo.order_tags.FREQUENCY_ID;
                string sFrequencyTypeId = objPaymentInfo.order_tags.FREQUENCY_TYPE;
                List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
                //string sAmount = ((!string.IsNullOrEmpty(objPaymentInfo.order_amount)) ? decimal.Parse(objPaymentInfo.order_amount) : 0).ToString();
                if (sFrequencyTypeId == Common.FrequencyType.SemesterFee)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.SemesterFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountHeadWiseFeeInfo), sAcademicYear).DataSource.Data;
                else if (sFrequencyTypeId == Common.FrequencyType.MessFee)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.MessFee, FEE_ROOT_ID = Common.FeeRoot.MessFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FecthMessFeesByStudentIdAndFeeRootId), sAcademicYear).DataSource.Data;
                else if (sFrequencyTypeId == Common.FrequencyType.HostelFee)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.HostelFee, FEE_ROOT_ID = Common.FeeRoot.HostelFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FecthMessFeesByStudentIdAndFeeRootId), sAcademicYear).DataSource.Data;
                else if (sFrequencyTypeId == Common.FrequencyType.ExamFee)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.ExamFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountHeadWiseFeeInfo), sAcademicYear).DataSource.Data;
                else if (sFrequencyTypeId == Common.FrequencyType.AdmissionFee)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.AdmissionFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountHeadWiseFeeInfo), sAcademicYear).DataSource.Data;
                else if (sFrequencyTypeId == Common.FrequencyType.LibraryFee)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.LibraryFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountHeadWiseFeeInfo), sAcademicYear).DataSource.Data;
                else if (sFrequencyTypeId == Common.FrequencyType.ConvocationFee)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.ConvocationFee, FEE_ROOT_ID = Common.FeeRoot.ConvocationFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountHeadWiseFeeInfo), sAcademicYear).DataSource.Data;
                else if (sFrequencyTypeId == Common.FrequencyType.Supplementary)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.Supplementary, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountHeadWiseFeeInfo), sAcademicYear).DataSource.Data;
                else if (sFrequencyTypeId == Common.FrequencyType.RevaluationFee)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.RevaluationFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountHeadWiseFeeInfo), sAcademicYear).DataSource.Data;
                else if (sFrequencyTypeId == Common.FrequencyType.Certificate)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.Certificate, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountHeadWiseFeeInfo), sAcademicYear).DataSource.Data;
                else if (sFrequencyTypeId == Common.FrequencyType.Term_II)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.Term_II, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountHeadWiseFeeInfo), sAcademicYear).DataSource.Data;
                else if (sFrequencyTypeId == Common.FrequencyType.PhotoCopyFee)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.PhotoCopyFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountHeadWiseFeeInfo), sAcademicYear).DataSource.Data;
                else if (sFrequencyTypeId == "12")
                {
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(
                        new FEE_STUDENT_ACCOUNT() { 
                            STUDENT_ID = sStudentId, 
                            FREQUENCY_TYPE_ID = sFrequencyTypeId, 
                            FEE_ROOT_ID = Common.FeeRoot.CollegeFee, 
                            FREQUENCY_ID = sFrequencyId 
                        }, 
                        SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountHeadWiseFeeInfo), 
                        sAcademicYear).DataSource.Data;
                }
                else if (sFrequencyTypeId == "ALL")
                {
                    var res = CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAllFeeDuesByStudentId), sAcademicYear);
                    studentAcountList = (res != null && res.DataSource != null && res.DataSource.Data != null)
                                        ? (List<FEE_STUDENT_ACCOUNT>)res.DataSource.Data
                                        : new List<FEE_STUDENT_ACCOUNT>();
                }

                string sUDF7 = objPaymentInfo.UDF7;
                if (string.IsNullOrEmpty(sUDF7))
                {
                    var cfOrderDbList = (List<CF_ORDER_INFO>)CMSHandler.FetchData<CF_ORDER_INFO>(
                        null, 
                        PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchOrderInfoByOrderId)
                            .Replace("?AC_YEAR", sAcademicYear)
                            .Replace("?ORDER_ID", "'" + sCFOrderId + "'"), 
                        sAcademicYear).DataSource.Data;
                    if (cfOrderDbList != null && cfOrderDbList.Count > 0)
                    {
                        sUDF7 = cfOrderDbList.FirstOrDefault().UDF7;
                    }
                }

                if (studentAcountList != null && studentAcountList.Count > 0 && !string.IsNullOrEmpty(sUDF7))
                {
                    var selectedHeads = sUDF7.Split(',').ToList();
                    studentAcountList = studentAcountList.Where(s => selectedHeads.Contains(s.HEAD_ID) || selectedHeads.Contains(s.HEAD)).ToList();
                }

                List<FEE_STUDENT_ACCOUNT> FetchFeeStudentAccount = new List<FEE_STUDENT_ACCOUNT>();
                if (sFrequencyTypeId != "12")
                {
                    string sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFrequencyIdNew)
                        .Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, sFrequencyId);

                    FetchFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(
                        new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, ACADEMIC_YEAR = sAcademicYear },
                        sSQL,
                        sAcademicYear).DataSource.Data;
                }

                //List<FEE_STUDENT_ACCOUNT> filteredHeads = new List<FEE_STUDENT_ACCOUNT>();

                //foreach (var item in studentAcountList.OrderBy(x => Convert.ToInt32(x.INSTALLMENT_ID)).OrderBy(x => Convert.ToInt32(x.HEAD_ID)))
                //{
                //    if (!string.IsNullOrEmpty(item.BALANCE))
                //    {
                //        decimal balance = 0;
                //        if (decimal.TryParse(item.BALANCE, out balance) && balance > 0)
                //        {
                //            filteredHeads.Add(item);
                //        }
                //    }
                //}

                //decimal dAmount = Convert.ToDecimal(sAmount);
                //List<FEE_STUDENT_ACCOUNT> updatedHeads = new List<FEE_STUDENT_ACCOUNT>();
                //foreach (var head in filteredHeads)
                //{
                //    decimal balance = decimal.Parse(head.BALANCE);
                //    if (dAmount <= 0)
                //        break;

                //    if (balance <= dAmount)
                //    {
                //        head.BALANCE = balance.ToString();
                //        dAmount -= balance;
                //    }
                //    else
                //    {
                //        head.BALANCE = dAmount.ToString();
                //        dAmount = 0;
                //    }
                //    updatedHeads.Add(head);
                //}

                //studentAcountList = updatedHeads;
                //studentAcountList.RemoveAll(r => r.BALANCE == "0");

                if (objPaymentInfo.order_status.ToLower() == "paid")
                {
                    int ReceiptNo = 0;
                    string Prefix = "00";
                    string TransactionId = string.Empty;

                    if (!string.IsNullOrEmpty(sAcademicYear) && studentAcountList != null && studentAcountList.Count > 0)
                    {
                        decimal totalBalance = studentAcountList.Sum(x => decimal.Parse(x.BALANCE));
                        if (Convert.ToDecimal(sAmount) >= totalBalance)
                        {
                            var GroupFeeStudentAccount = studentAcountList.GroupBy(x => x.FREQUENCY_ID);

                            foreach (var FrequencyItem in GroupFeeStudentAccount)
                            {
                                 sFrequencyId = FrequencyItem.FirstOrDefault().FREQUENCY_ID;
                                 var feeTransactions = FrequencyItem.ToList();

                                 string getAcYearSql = "SELECT ACADEMIC_YEAR FROM fee_frequency_fee_mode WHERE FEE_FREQUENCY_FEE_MODE_ID = @FEE_FREQUENCY_FEE_MODE_ID";
                                 var acYearResult = DapperHandler.FetchAll<dynamic>(getAcYearSql, new { FEE_FREQUENCY_FEE_MODE_ID = sFrequencyId }, null).FirstOrDefault();
                                 string itemAcademicYear = sAcademicYear;
                                 if (acYearResult != null)
                                 {
                                     itemAcademicYear = Convert.ToString(acYearResult.ACADEMIC_YEAR);
                                 }

                                 decimal balance = feeTransactions.Sum(x => decimal.Parse(x.BALANCE));
                                 if (balance > 0)
                                 {
                                     var objFeeTransactionModel = new FEE_TRANSACTION
                                     {
                                         STUDENT_ID = sStudentId,
                                         CLASS = null,
                                         FREQUENCY = sFrequencyId,
                                         FREQUENCY_TO = sFrequencyId,
                                         ACADEMIC_YEAR = itemAcademicYear,
                                         PAYMENT_DATE = DateTime.Today.ToString("yyyy-MM-dd"),
                                         USERNAME = sStudentId,
                                         COLLECTED = balance.ToString(),
                                         CF_ORDER_ID = objPaymentInfo.order_id,
                                         PayUResponse_Id = null,
                                         PAYMENT_MODE = Common.PaymentMode.Online
                                     };

                                     var FetchReceiptNo = (List<AUTO_GENERATE_NUMBERS>)CMSHandler.FetchData<AUTO_GENERATE_NUMBERS>(new StuPersonalInfo() { STUDENT_ID = sStudentId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAutoGenerateNumber), string.Empty).DataSource.Data;
                                     if (FetchReceiptNo != null && FetchReceiptNo.Count > 0)
                                     {
                                         ReceiptNo = Convert.ToInt32(FetchReceiptNo.FirstOrDefault().exam_receipt_no);
                                         AUTO_GENERATE_NUMBERS objAutoGenerateNumber = new AUTO_GENERATE_NUMBERS
                                         {
                                             exam_receipt_no = (ReceiptNo + 1).ToString()
                                         };
                                         objFeeTransactionModel.RECEIPT_NO = Prefix + objAutoGenerateNumber.exam_receipt_no;
                                         objPaymentInfo.RECEIPT_NO = objFeeTransactionModel.RECEIPT_NO;

                                         var UpdateReceiptNo = CMSHandler.SaveOrUpdate(objAutoGenerateNumber, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateAutoGenerateNumber), string.Empty, false);
                                         if (UpdateReceiptNo.Success)
                                         {
                                             var SaveFeeTransaction = CMSHandler.SaveOrUpdate(objFeeTransactionModel, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveCFFeeTransaction), itemAcademicYear, true);
                                             if (SaveFeeTransaction.Success)
                                             {
                                                 var FetchTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(new FEE_TRANSACTION() { CF_ORDER_ID = sCFOrderId, FREQUENCY = sFrequencyId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTransactionByCashFreeId), itemAcademicYear).DataSource.Data;
                                                 TransactionId = (FetchTransaction != null && FetchTransaction.Count > 0) ? FetchTransaction.FirstOrDefault().TRANSACTION_ID : SaveFeeTransaction.RowUniqueId.ToString();

                                                 foreach (var item in feeTransactions)
                                                 {
                                                     item.DEBIT = item.BALANCE;
                                                     item.PAID_AMOUNT = item.BALANCE;
                                                     item.HEAD = item.HEAD_ID;
                                                     item.FEE_MAIN_HEAD_ID = item.FEE_MAIN_HEAD_ID;
                                                     item.TRANSACTION_ID = TransactionId;
                                                     item.RECEIPT_NO = objFeeTransactionModel.RECEIPT_NO;
                                                     item.FREQUENCY = sFrequencyId;
                                                     item.INSTALLMENT_ID = item.INSTALLMENT_ID;

                                                     var SaveFeeCollection = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeCollection), string.Empty, false);
                                                     if (SaveFeeCollection.Success)
                                                     {
                                                         if (sFrequencyTypeId == "12")
                                                         {
                                                             // 1. Fetch Frequency ID (FEE_MODE = 12)
                                                             string getFrequencySql = "SELECT FEE_FREQUENCY_FEE_MODE_ID FROM fee_frequency_fee_mode WHERE FEE_MODE = 12 AND ACADEMIC_YEAR = @ACADEMIC_YEAR";
                                                             var frequencyResult = DapperHandler.FetchAll<dynamic>(getFrequencySql, new { ACADEMIC_YEAR = itemAcademicYear }, null).FirstOrDefault();
                                                             string frequencyId = frequencyResult != null ? Convert.ToString(frequencyResult.FEE_FREQUENCY_FEE_MODE_ID) : "";

                                                             // 2. Fetch Application Type ID (CLASS_LEVEL)
                                                             string appTypeSql = @"
                                                                 SELECT C.CLASS_LEVEL AS APPLICATION_TYPE_ID 
                                                                 FROM STU_CLASS S
                                                                 INNER JOIN cp_Classes C ON C.CLASS_ID = S.CLASS_ID
                                                                 WHERE S.STUDENT_ID = @STUDENT_ID 
                                                                   AND S.ACADEMIC_YEAR = @ACADEMIC_YEAR 
                                                                   AND S.IS_DELETED != 1";
                                                             var appTypeResult = DapperHandler.FetchAll<dynamic>(appTypeSql, new { STUDENT_ID = sStudentId, ACADEMIC_YEAR = itemAcademicYear }, null).FirstOrDefault();
                                                             string applicationTypeId = appTypeResult != null ? Convert.ToString(appTypeResult.APPLICATION_TYPE_ID) : "";

                                                             // 3. Fetch FEE_MAIN_HEAD_ID and HEAD_ID (HEAD) from fee_main_heads
                                                             string mainHeadSql = @"
                                                                 SELECT FEE_MAIN_HEAD_ID, HEAD_ID 
                                                                 FROM fee_main_heads 
                                                                 WHERE FREQUENCY_ID = @FREQUENCY_ID 
                                                                   AND APPLICATION_TYPE_ID = @APPLICATION_TYPE_ID 
                                                                   AND ACADEMIC_YEAR = @ACADEMIC_YEAR 
                                                                   AND IS_DELETED != 1";
                                                             var mainHeadResult = DapperHandler.FetchAll<dynamic>(mainHeadSql, new {
                                                                 FREQUENCY_ID = frequencyId,
                                                                 APPLICATION_TYPE_ID = applicationTypeId,
                                                                 ACADEMIC_YEAR = itemAcademicYear
                                                             }, null).FirstOrDefault();

                                                             string feeMainHeadId = mainHeadResult != null ? Convert.ToString(mainHeadResult.FEE_MAIN_HEAD_ID) : "";
                                                             string headId = mainHeadResult != null ? Convert.ToString(mainHeadResult.HEAD_ID) : "";

                                                             if (!string.IsNullOrEmpty(frequencyId) && !string.IsNullOrEmpty(headId) && !string.IsNullOrEmpty(feeMainHeadId))
                                                             {
                                                                 string updateSql = @"UPDATE FEE_STUDENT_ACCOUNT 
                                                                                      SET DEBIT = DEBIT + ?DEBIT 
                                                                                      WHERE STUDENT_ID = ?STUDENT_ID 
                                                                                        AND ACADEMIC_YEAR = ?ACADEMIC_YEAR 
                                                                                        AND FREQUENCY_ID = ?FREQUENCY_ID 
                                                                                        AND HEAD = ?HEAD 
                                                                                        AND FEE_MAIN_HEAD_ID = ?FEE_MAIN_HEAD_ID";
                                                                 CMSHandler.SaveOrUpdate(new { 
                                                                     DEBIT = item.PAID_AMOUNT, 
                                                                     STUDENT_ID = sStudentId, 
                                                                     ACADEMIC_YEAR = itemAcademicYear,
                                                                     FREQUENCY_ID = frequencyId,
                                                                     HEAD = headId,
                                                                     FEE_MAIN_HEAD_ID = feeMainHeadId
                                                                 }, updateSql);
                                                             }
                                                         }
                                                         else
                                                         {
                                                             CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeStudentAccount), itemAcademicYear);
                                                         }
                                                     }
                                                 }
                                             }
                                            else
                                            {
                                                objPaymentInfo.ERROR_CODE = "Pending";
                                                objPaymentInfo.ERROR_DESCRIPTION = "Please note Transaction Id and enquire with admin !!";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    objPaymentInfo.ERROR_CODE = "Pending";
                    objPaymentInfo.ERROR_DESCRIPTION = "Payment status not PAID.";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "FeeCashFreeCollection", ex.Message);
                }
            }

            return objPaymentInfo;
        }

        #region settlement update

        public async Task<ActionResult> SettlementUpdate()
        {
            DataTable dtActiveYear = new DataTable();
            AccountViewModel objAccount = new AccountViewModel();
            dtActiveYear = objAccount.FetchActiveYear().DataSource.Table;
            string strForm = string.Empty;
            string acyear = dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.AC_YEAR].ToString();
            var cfOrderInfo = (List<CF_ORDER_INFO>)CMSHandler.FetchData<CF_ORDER_INFO>(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchCFPaidOrdersForSettlement), acyear).DataSource.Data;
            if (cfOrderInfo != null && cfOrderInfo.Count > 0)
            {
                foreach (var item in cfOrderInfo)
                {
                    SettlementUpdateById(item.CUSTOM_ORDER_ID);
                }

            }
            return View();
        }
        public JsonResult SettlementUpdateById(string order_id)
        {
            try
            {
                DataTable dtActiveYear = new DataTable();
                AccountViewModel objAccount = new AccountViewModel();
                dtActiveYear = objAccount.FetchActiveYear().DataSource.Table;
                string strForm = string.Empty;
                string acyear = dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.AC_YEAR].ToString();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                string sPaymentLink = string.Empty;
                string sResponse = string.Empty;
                string sIsPGLive = System.Configuration.ConfigurationManager.AppSettings["ISPG_LIVE"].ToString();
                var CF_API_Info = (List<FEE_CF_SETTING>)CMSHandler.FetchData<FEE_CF_SETTING>(
                    new FEE_CF_SETTING() { TRANSACTION_TYPE = Common.CF_TRANSACTION_TYPE.PAYMENT_GATEWAY, IS_LIVE = sIsPGLive },
                    PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchAPIKeyInfoByTransactionType)
                ).DataSource.Data;

                string setlement_url = CF_API_Info.FirstOrDefault().BASE_URL + "/" + Common.CF_API_URL.ORDER + "/" + order_id + "/" + Common.CF_API_URL.SETTLEMENT_BY_ORDER;
                var client = new RestClient(setlement_url);

                // Create a GET request
                var request = new RestRequest
                {
                    Method = Method.Get
                };

                // Add headers to the request
                request.AddHeader("accept", "application/json");
                request.AddHeader("x-api-version", CF_API_Info.FirstOrDefault().API_VERSION);
                request.AddHeader("x-client-id", CF_API_Info.FirstOrDefault().CLIENT_ID);
                request.AddHeader("x-client-secret", CF_API_Info.FirstOrDefault().SECRET_ID);

                // Execute the request synchronously
                var response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    sResponse = response.Content;
                }
                else
                {
                    sResponse = response.Content;
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("PaymentGatewayController", "SettlementUpdateById", "Error Msg: " + sResponse, "Query is empty!");
                    }
                    return Json(new { status = false, Message = "Sorry for inconvenience, unable to initiate this transaction", PaymentList = "" }, JsonRequestBehavior.AllowGet);
                }

                var orderResponse = JsonConvert.DeserializeObject<CF_SETTLEMENT_RESPONSE_BY_ORDER>(sResponse);
                if (orderResponse != null)
                {
                    DateTime parsedDateTime = DateTime.Parse(orderResponse.transfer_time);
                    orderResponse.payment_time = parsedDateTime.ToString("yyyy-MM-dd");

                    var updateOrderInfo = CMSHandler.SaveOrUpdate(
                        new CF_SETTLEMENT_RESPONSE_BY_ORDER { CF_ORDER_ID = orderResponse.order_id, SETTLEMENT_DATE = orderResponse.payment_time },
                        PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.UpdateSettlementInfoByOrderid), acyear);

                    return Json(new { status = true, Message = "", order_id = orderResponse.order_id }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("PaymentGatewayController", "SettlementUpdateById", "Error Msg: " + ex.Message, "Query is empty!");
                }
                return Json(new { status = false, Message = "Sorry for inconvenience, unable to initiate this transaction", PaymentList = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = false, Message = "Server is busy, please try later", PaymentList = "" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SettlementUpdateByIdv2(string order_id)
        {
            try
            {
                AccountViewModel objAccount = new AccountViewModel();
                DataTable dtActiveYear = objAccount.FetchActiveYear().DataSource.Table;
                string acyear = dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.AC_YEAR].ToString();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                string sIsPGLive = ConfigurationManager.AppSettings["ISPG_LIVE"];
                var CF_API_Info = (List<FEE_CF_SETTING>)CMSHandler.FetchData<FEE_CF_SETTING>(new FEE_CF_SETTING { TRANSACTION_TYPE = Common.CF_TRANSACTION_TYPE.PAYMENT_GATEWAY, IS_LIVE = sIsPGLive }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchAPIKeyInfoByTransactionType)).DataSource.Data;

                var cf = CF_API_Info.First();
                string orderSettlementUrl = $"{cf.BASE_URL}/orders/{order_id}/settlements";

                var client1 = new RestClient(orderSettlementUrl);
                var request1 = new RestRequest { Method = Method.Get };

                request1.AddHeader("accept", "application/json");
                request1.AddHeader("x-api-version", cf.API_VERSION);
                request1.AddHeader("x-client-id", cf.CLIENT_ID);
                request1.AddHeader("x-client-secret", cf.SECRET_ID);

                var response1 = client1.Execute(request1);

                if (!response1.IsSuccessful || string.IsNullOrEmpty(response1.Content))
                {
                    return Json(new { status = false, Message = "Unable to fetch settlement info" }, JsonRequestBehavior.AllowGet);
                }

                var orderSettlement = JsonConvert.DeserializeObject<CF_ORDER_SETTLEMENT>(response1.Content);

                if (orderSettlement == null || string.IsNullOrEmpty(orderSettlement.payment_time))
                {
                    return Json(new { status = true, Message = "Settlement not initiated yet" }, JsonRequestBehavior.AllowGet);
                }

                DateTimeOffset paymentTime = DateTimeOffset.Parse(orderSettlement.payment_time);
                string startDate = paymentTime.AddMinutes(-10).ToString("yyyy-MM-ddTHH:mm:sszzz");
                string endDate = paymentTime.AddMinutes(10).ToString("yyyy-MM-ddTHH:mm:sszzz");
                string reconUrl = $"{cf.BASE_URL}/recon/vendor";

                var client2 = new RestClient(reconUrl);
                var request2 = new RestRequest { Method = Method.Post };

                request2.AddHeader("Content-Type", "application/json");
                request2.AddHeader("x-api-version", cf.API_VERSION);
                request2.AddHeader("x-client-id", cf.CLIENT_ID);
                request2.AddHeader("x-client-secret", cf.SECRET_ID);

                var requestBody = new
                {
                    pagination = new { limit = 100 },
                    filters = new
                    {
                        start_date = startDate,
                        end_date = endDate
                    }
                };

                string jsonBody = JsonConvert.SerializeObject(requestBody);
                request2.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

                var response2 = client2.Execute(request2);

                if (!response2.IsSuccessful || string.IsNullOrEmpty(response2.Content))
                {
                    return Json(new { status = false, Message = $"Recon API failed: {response2.StatusCode} - {response2.Content}" }, JsonRequestBehavior.AllowGet);
                }

                var recon = JsonConvert.DeserializeObject<CF_RECON_VENDOR_RESPONSE>(response2.Content);

                var settlement = recon?.data?.FirstOrDefault(x => x.merchant_order_id == order_id && x.settled == true);

                if (settlement == null)
                {
                    return Json(new { status = true, Message = "Settlement pending or not found" }, JsonRequestBehavior.AllowGet);
                }
                DateTime settlementDate = DateTime.Parse(settlement.settlement_time);

                CMSHandler.SaveOrUpdate(new CF_SETTLEMENT_RESPONSE_BY_ORDER { CF_ORDER_ID = order_id, SETTLEMENT_DATE = settlementDate.ToString("yyyy-MM-dd") }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.UpdateSettlementInfoByOrderid), acyear);

                return Json(new { status = true, Message = "Settlement updated successfully", order_id = order_id, settlement_date = settlementDate.ToString("yyyy-MM-dd"), utr = settlement.settlement_utr, settlement_id = settlement.settlement_id }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("PaymentGatewayController", "SettlementUpdateByIdv2", ex.Message, order_id);
                }

                return Json(new
                {
                    status = false,
                    Message = "Server error while updating settlement"
                }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion

        #region auto fee entry
        public async Task<JsonResult> CFFeentry(string x)
        {
            DataTable dtActiveYear = new DataTable();
            AccountViewModel objAccount = new AccountViewModel();
            dtActiveYear = objAccount.FetchActiveYear().DataSource.Table;
            string strForm = string.Empty;
            string sAcademicYear = dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.AC_YEAR].ToString();
            string no_of_back_days = string.Empty;
            if (!string.IsNullOrEmpty(x))
                no_of_back_days = x;
            else
                no_of_back_days = "2";
            string sql = PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchCFOrderByForFeeEntry).Replace("?NO_OF_BACK_DAYS", no_of_back_days);
            var cfOrderInfo = (List<CF_ORDER_INFO>)CMSHandler.FetchData<CF_ORDER_INFO>(null, sql, sAcademicYear).DataSource.Data;
            if (cfOrderInfo != null && cfOrderInfo.Count > 0)
            {
                foreach (var item in cfOrderInfo)
                {
                    CFReturnURL(item.CUSTOM_ORDER_ID);
                    SettlementUpdateByIdv2(item.CUSTOM_ORDER_ID);
                }

            }
            return Json(JsonRequestBehavior.AllowGet, "All done..");
        }


        public ActionResult CFFeeManualEntry(string passkey, string orderids)
        {
            if (string.IsNullOrEmpty(passkey))
            {
                ModelState.AddModelError("", "Passkey is required.");
                return View();
            }
            passkey = passkey.ToUpper();
            if (passkey == "AUX@2025@SET")
            {
                if (!string.IsNullOrEmpty(orderids))
                {
                    var lines = orderids.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    var orderList = lines.Where(l => l.Trim().StartsWith("ORDER_")).Select(l => l.Trim()).ToList();

                    foreach (var orderId in orderList)
                    {
                        CFReturnURL(orderId);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Passkey.");
                return View();
            }



            return View();
        }

        public ActionResult CFFeeManualResponseEntry(string passkey, string orderids)
        {
            if (string.IsNullOrEmpty(passkey))
            {
                ModelState.AddModelError("", "Passkey is required.");
                return View();
            }
            passkey = passkey.ToUpper();
            if (passkey == "AUX@2025@SET")
            {
                if (!string.IsNullOrEmpty(orderids))
                {
                    var lines = orderids.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    var orderList = lines.Where(l => l.Trim().StartsWith("ORDER_")).Select(l => l.Trim()).ToList();

                    foreach (var orderId in orderList)
                    {
                        CFResponseCheck(orderId);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Passkey.");
                return View();
            }



            return View();
        }
        public ActionResult CFFeeManualSettelmentEntry(string passkey, string orderids)
        {
            if (string.IsNullOrEmpty(passkey))
            {
                ModelState.AddModelError("", "Passkey is required.");
                return View();
            }
            passkey = passkey.ToUpper();
            if (passkey == "AUX@2025@SET")
            {
                if (!string.IsNullOrEmpty(orderids))
                {
                    var lines = orderids.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    var orderList = lines.Where(l => l.Trim().StartsWith("ORDER_")).Select(l => l.Trim()).ToList();

                    foreach (var orderId in orderList)
                    {
                        SettlementUpdateByIdv2(orderId);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Passkey.");
                return View();
            }



            return View();
        }


        public JsonResult FetchSettlementByDate(string startDate, string endDate, string cursor = null)
        {
            try
            {
                startDate = "2025-10-23";
                endDate = "2025-10-23";
                AccountViewModel objAccount = new AccountViewModel();
                var dtActiveYear = objAccount.FetchActiveYear().DataSource.Table;
                string acyear = dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.AC_YEAR].ToString();

                string sIsPGLive = System.Configuration.ConfigurationManager.AppSettings["ISPG_LIVE"].ToString();
                var CF_API_Info = (List<FEE_CF_SETTING>)CMSHandler.FetchData<FEE_CF_SETTING>(
                    new FEE_CF_SETTING()
                    {
                        TRANSACTION_TYPE = Common.CF_TRANSACTION_TYPE.PAYMENT_GATEWAY,
                        IS_LIVE = sIsPGLive
                    },
                    PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchAPIKeyInfoByTransactionType)
                ).DataSource.Data;
                var apiConfig = CF_API_Info.FirstOrDefault();

                string url = $"{apiConfig.BASE_URL}/recon/vendor" +
             $"?limit=1000&cursor={cursor}" +
             $"&start_date={startDate:yyyy-MM-dd}T00:00:00+05:30" +
             $"&end_date={endDate:yyyy-MM-dd}T23:59:59+05:30";

                var client = new RestClient(url);
                var request = new RestRequest(url, Method.Get);

                request.AddHeader("accept", "application/json");
                request.AddHeader("x-api-version", apiConfig.API_VERSION);
                request.AddHeader("x-client-id", apiConfig.CLIENT_ID);
                request.AddHeader("x-client-secret", apiConfig.SECRET_ID);
                request.AddHeader("content-type", "application/json");

                var response = client.Execute(request);

                if (!response.IsSuccessful)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("PaymentGatewayController", "FetchSettlementByDate", response.Content, "Payment API Failed!");
                    }
                    return Json(new { status = false, Message = "API request failed", data = "" }, JsonRequestBehavior.AllowGet);
                }

                var settlementResult = JsonConvert.DeserializeObject<CF_SETTLEMENT_LIST_RESPONSE>(response.Content);

                if (settlementResult?.data != null)
                {
                    foreach (var settlement in settlementResult.data)
                    {
                        DateTime parsedDateTime = DateTime.Parse(settlement.settlement_time);
                        settlement.settlement_time = parsedDateTime.ToString("yyyy-MM-dd");
                        CMSHandler.SaveOrUpdate(new CF_SETTLEMENT_RESPONSE_BY_ORDER { CF_ORDER_ID = settlement.merchant_order_id, SETTLEMENT_DATE = settlement.settlement_time }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.UpdateSettlementInfoByOrderid), acyear);
                    }

                    string nextCursor = settlementResult.pagination?.next_cursor;

                    return Json(new { status = true, Message = "Settlement List Updated", next_cursor = nextCursor, count = settlementResult.data.Count }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("PaymentGatewayController", "FetchSettlementByDate", ex.Message, "Error Occurred!");
                }
                return Json(new { status = false, Message = "Server Error" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = false, Message = "No data found" }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        public async Task<ActionResult> PaymentSuccess(string order_id)
        {
            FeeTransactionViewModel liFeeTransactions = new FeeTransactionViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.COLLEGE_DETAILS] != null)
                {
                    liFeeTransactions.liCollegeDetails = (List<CollegeDetails>)Session[Common.SESSION_VARIABLES.COLLEGE_DETAILS];
                }
                else
                {
                    liFeeTransactions.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                }

                var liAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchAcademicYearInfo), string.Empty).DataSource.Data;

                foreach (var acYear in liAcademicYear)
                {
                    var orderList = (List<CF_ORDER_INFO>)CMSHandler.FetchData<CF_ORDER_INFO>(new CF_ORDER_INFO() { order_id = order_id }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchOrderInfoByOrderId), acYear.AC_YEAR).DataSource.Data;
                    if (orderList != null && orderList.Count > 0)
                    {
                        liFeeTransactions.FeeTransaction.ORDER_ID = order_id;
                        liFeeTransactions.FeeTransaction.PAYMENT_DATE = orderList.FirstOrDefault().created_at;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(liFeeTransactions.FeeTransaction.ORDER_ID))
                {
                    var edvironOrder = DapperHandler.FetchAll<EDVIRON_ORDER_INFO>(
                        EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchOrderByCustomOrderId), 
                        new { CUSTOM_ORDER_ID = order_id }).ToList();
                    if (edvironOrder != null && edvironOrder.Count > 0)
                    {
                        liFeeTransactions.FeeTransaction.ORDER_ID = order_id;
                        liFeeTransactions.FeeTransaction.PAYMENT_DATE = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("PaymentGatewayController", "PaymentSuccess", ex.Message, "Error Occurred!");
                }
            }
            return View(liFeeTransactions);
        }

        public ActionResult AdmissionPaymentResponse(string sOrderId)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                if (string.IsNullOrEmpty(sAcademicYear))
                {
                    var academicYears = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchActiveYear)).DataSource.Data;
                    if (academicYears != null && academicYears.Count > 0)
                    {
                        sAcademicYear = academicYears.FirstOrDefault().AC_YEAR;
                    }
                }

                // Check if this is a convocation order (frequency type id is "11")
                bool isConvocation = false;
                var edvironOrderCheck = DapperHandler.FetchAll<EDVIRON_ORDER_INFO>(EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchOrderByCustomOrderId), new { CUSTOM_ORDER_ID = sOrderId }).ToList();
                if (edvironOrderCheck != null && edvironOrderCheck.Count > 0)
                {
                    if (edvironOrderCheck.FirstOrDefault().FREQUENCY_TYPE_ID == "11")
                    {
                        isConvocation = true;
                    }
                }
                if (!isConvocation)
                {
                    var cfOrderCheck = (List<CF_ORDER_INFO>)CMSHandler.FetchData<CF_ORDER_INFO>(new CF_ORDER_INFO() { order_id = sOrderId }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchOrderInfoByOrderId), sAcademicYear).DataSource.Data;
                    if (cfOrderCheck != null && cfOrderCheck.Count > 0)
                    {
                        var orderCheck = cfOrderCheck.FirstOrDefault();
                        var orderResponseCheck = Newtonsoft.Json.JsonConvert.DeserializeObject<CF_ORDER_INFO>(orderCheck.CF_ORDER_RESPONSE);
                        if (orderResponseCheck != null && orderResponseCheck.order_tags != null && orderResponseCheck.order_tags.FREQUENCY_TYPE == "11")
                        {
                            isConvocation = true;
                        }
                    }
                }

                if (isConvocation)
                {
                    return RedirectToAction("ConvocationPaymentResponse", new { sOrderId = sOrderId });
                }

                // 0. Fetch College Details Upfront
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;

                // 1. Check Cashfree Order Info
                var cfOrderInfo = (List<CF_ORDER_INFO>)CMSHandler.FetchData<CF_ORDER_INFO>(new CF_ORDER_INFO() { order_id = sOrderId }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchOrderInfoByOrderId), sAcademicYear).DataSource.Data;

                if (cfOrderInfo != null && cfOrderInfo.Count > 0)
                {
                    var order = cfOrderInfo.FirstOrDefault();
                    var orderResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CF_ORDER_INFO>(order.CF_ORDER_RESPONSE);

                    objViewModel.Status = orderResponse.order_status;
                    objViewModel.Message = (orderResponse.order_status.ToUpper() == "PAID" || orderResponse.order_status.ToUpper() == "SUCCESS") ? "Payment Successful" : "Payment " + orderResponse.order_status;
                    objViewModel.Amount = orderResponse.order_amount;
                    objViewModel.TransactionId = orderResponse.order_id;
                    objViewModel.ReceiptNo = orderResponse.RECEIPT_NO;
                    objViewModel.PaymentDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                    if (orderResponse.order_status.ToUpper() == "PAID" || orderResponse.order_status.ToUpper() == "SUCCESS")
                    {
                        string sStudentId = orderResponse.order_tags.USER_ID;
                        string sFrequencyId = orderResponse.order_tags.FREQUENCY_ID;

                        var fetchTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(
                            new FEE_TRANSACTION() { CF_ORDER_ID = sOrderId },
                            SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTransactionByCashFreeId),
                            sAcademicYear).DataSource.Data;

                        if (fetchTransaction != null && fetchTransaction.Count > 0)
                        {
                            var latestReceipt = fetchTransaction.OrderByDescending(x => x.TRANSACTION_ID).FirstOrDefault();
                            var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT()
                            {
                                TRANSACTION_ID = latestReceipt.TRANSACTION_ID,
                                FREQUENCY_ID = sFrequencyId,
                                ACADEMIC_YEAR = sAcademicYear
                            };
                            objViewModel.liFeeTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeReceiptByTransactionId), sAcademicYear).DataSource.Data;
                            
                            if (objViewModel.liFeeTransaction != null)
                            {
                                var studentDetails = DapperHandler.FetchAll<dynamic>("SELECT FIRST_NAME, LAST_NAME, REGISTER_NO, ROLL_NO FROM STU_PERSONAL_INFO WHERE STUDENT_ID = @STUDENT_ID", new { STUDENT_ID = sStudentId }).FirstOrDefault();
                                if (studentDetails != null)
                                {
                                    foreach (var item in objViewModel.liFeeTransaction)
                                    {
                                        item.FIRST_NAME = Convert.ToString(studentDetails.FIRST_NAME) + " " + Convert.ToString(studentDetails.LAST_NAME);
                                        item.REGISTER_NO = Convert.ToString(studentDetails.REGISTER_NO);
                                        item.ROLL_NO = Convert.ToString(studentDetails.ROLL_NO);
                                    }
                                }
                            }
                            
                            objViewModel.ReceiptNo = latestReceipt.RECEIPT_NO;
                            objViewModel.PaymentDate = latestReceipt.PAYMENT_DATE;

                            var arrearTransaction = objViewModel.liFeeTransaction != null ? objViewModel.liFeeTransaction.FirstOrDefault(x => x.FREQUENCY_TYPE_ID == "12") : null;
                            if (arrearTransaction != null)
                            {
                                var fetchFeeCollection = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(
                                    @"SELECT HEAD FROM FEE_COLLECTION WHERE TRANSACTION_ID=@TRANSACTION_ID", 
                                    new { TRANSACTION_ID = latestReceipt.TRANSACTION_ID }
                                 ).ToList();

                                if (fetchFeeCollection != null && fetchFeeCollection.Count > 0)
                                {
                                    foreach(var item in fetchFeeCollection)
                                    {
                                        DapperHandler.SaveOrUpdate(
                                            SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.updatepaidstatusforaarearcourse),
                                            new { STUDENT_ID = sStudentId, HEAD = item.HEAD }
                                        );
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // 2. Check Edviron Order Info
                    var edvironOrder = DapperHandler.FetchAll<EDVIRON_ORDER_INFO>(EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchOrderByCustomOrderId), new { CUSTOM_ORDER_ID = sOrderId }).ToList();
                    if (edvironOrder != null && edvironOrder.Count > 0)
                    {
                        var order = edvironOrder.FirstOrDefault();
                        objViewModel.Status = order.STATUS;
                        objViewModel.Message = (order.STATUS.ToUpper() == "PAID" || order.STATUS.ToUpper() == "SUCCESS") ? "Payment Successful" : "Payment " + order.STATUS;
                        objViewModel.Amount = order.ACTUAL_AMOUNT ?? order.AMOUNT;
                        objViewModel.TransactionId = order.CUSTOM_ORDER_ID;
                        objViewModel.PaymentDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                        if (order.STATUS.ToUpper() == "PAID" || order.STATUS.ToUpper() == "SUCCESS")
                        {
                            string sStudentId = order.STUDENT_ID;
                            string sFrequencyId = order.FREQUENCY_ID;

                            var fetchTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(
                                new FEE_TRANSACTION() { CF_ORDER_ID = sOrderId },
                                SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTransactionByCashFreeId)
                                    .Replace("?CF_ORDER_ID", "'" + sOrderId + "'")
                                    .Replace("?FREQUENCY", "'" + sFrequencyId + "'"),
                                sAcademicYear
                            ).DataSource.Data;

                            if (fetchTransaction != null && fetchTransaction.Count > 0)
                            {
                                var latestReceipt = fetchTransaction.OrderByDescending(x => x.TRANSACTION_ID).FirstOrDefault();
                                var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT()
                                {
                                    TRANSACTION_ID = latestReceipt.TRANSACTION_ID,
                                    FREQUENCY_ID = sFrequencyId,
                                    ACADEMIC_YEAR = sAcademicYear
                                };
                                objViewModel.liFeeTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeReceiptByTransactionId), sAcademicYear).DataSource.Data;
                                
                                if (objViewModel.liFeeTransaction != null)
                                {
                                    var studentDetails = DapperHandler.FetchAll<dynamic>("SELECT FIRST_NAME, LAST_NAME, REGISTER_NO, ROLL_NO FROM STU_PERSONAL_INFO WHERE STUDENT_ID = @STUDENT_ID", new { STUDENT_ID = sStudentId }).FirstOrDefault();
                                    if (studentDetails != null)
                                    {
                                        foreach (var item in objViewModel.liFeeTransaction)
                                        {
                                            item.FIRST_NAME = Convert.ToString(studentDetails.FIRST_NAME) + " " + Convert.ToString(studentDetails.LAST_NAME);
                                            item.REGISTER_NO = Convert.ToString(studentDetails.REGISTER_NO);
                                            item.ROLL_NO = Convert.ToString(studentDetails.ROLL_NO);
                                        }
                                    }
                                }
                                
                                objViewModel.ReceiptNo = latestReceipt.RECEIPT_NO;
                                objViewModel.PaymentDate = latestReceipt.PAYMENT_DATE;
                                
                                var arrearTransaction = objViewModel.liFeeTransaction != null ? objViewModel.liFeeTransaction.FirstOrDefault(x => x.FREQUENCY_TYPE_ID == "12") : null;
                                if (arrearTransaction != null)
                                {
                                    var fetchFeeCollection = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(
                                        @"SELECT HEAD FROM FEE_COLLECTION WHERE TRANSACTION_ID=@TRANSACTION_ID", 
                                        new { TRANSACTION_ID = latestReceipt.TRANSACTION_ID }
                                    ).ToList();

                                    if (fetchFeeCollection != null && fetchFeeCollection.Count > 0)
                                    {
                                        foreach(var item in fetchFeeCollection)
                                        {
                                            DapperHandler.SaveOrUpdate(
                                                SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.updatepaidstatusforaarearcourse),
                                                new { STUDENT_ID = sStudentId, HEAD = item.HEAD }
                                            );
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var staffEdvironOrders = DapperHandler.FetchAll<STAFF_FEE_EDVIRON_ORDER_INFO>(
                            EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchOrderByCustomOrderId)
                                     .Replace("FEE_EDVIRON_ORDER_INFO", "staff_fee_edviron_order_info"),
                            new { CUSTOM_ORDER_ID = sOrderId }).ToList();

                        if (staffEdvironOrders != null && staffEdvironOrders.Count > 0)
                        {
                            var order = staffEdvironOrders.FirstOrDefault();
                            objViewModel.Status = order.STATUS;
                            objViewModel.Message = (order.STATUS.ToUpper() == "PAID" || order.STATUS.ToUpper() == "SUCCESS") ? "Payment Successful" : "Payment " + order.STATUS;
                            objViewModel.Amount = order.ACTUAL_AMOUNT ?? order.AMOUNT;
                            objViewModel.TransactionId = order.CUSTOM_ORDER_ID;
                            objViewModel.PaymentDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                            if (order.STATUS.ToUpper() == "PAID" || order.STATUS.ToUpper() == "SUCCESS")
                            {
                                string sStaffId = order.STUDENT_ID;
                                string sFrequencyId = order.FREQUENCY_ID;

                                var fetchTransaction = DapperHandler.FetchAll<FEE_STAFF_TRANSACTION>(
                                    "SELECT TRANSACTION_ID,RECEIPT_NO,PAYMENT_DATE,COLLECTED FROM staff_fee_transaction WHERE CF_ORDER_ID = ?CF_ORDER_ID AND FREQUENCY = ?FREQUENCY",
                                    new { CF_ORDER_ID = sOrderId, FREQUENCY = sFrequencyId }).ToList();

                                if (fetchTransaction != null && fetchTransaction.Count > 0)
                                {
                                    var latestReceipt = fetchTransaction.OrderByDescending(x => x.TRANSACTION_ID).FirstOrDefault();
                                    
                                    var receiptQuery = @"SELECT 
                                                            FT.TRANSACTION_ID,
                                                            FT.FREQUENCY,
                                                            FT.PAYMENT_DATE,
                                                            FT.RECEIPT_NO,
                                                            FT.COLLECTED,
                                                            FT.PAYMENT_MODE,
                                                            'Staff' AS PROGRAMME_NAME,
                                                            FFF.FREQUENCY_ID AS MAIN_HEAD_ID,
                                                            FF.FREQUENCY_NAME,
                                                            CONCAT(MH.MAIN_HEAD,' (',H.HEAD_CODE,')') AS MAIN_HEAD,
                                                            FC.PAID_AMOUNT AS DEBIT
                                                         FROM 
                                                            staff_fee_transaction AS FT
                                                         INNER JOIN 
                                                            staff_fee_collection AS FC ON FT.TRANSACTION_ID = FC.TRANSACTION_ID
                                                         INNER JOIN 
                                                            FEE_MAIN_HEADS AS FMH ON FC.FREQUENCY = FMH.FREQUENCY_ID AND FC.HEAD = FMH.HEAD_ID AND FMH.ACADEMIC_YEAR = FT.ACADEMIC_YEAR
                                                         INNER JOIN 
                                                            SUP_FEE_MAIN_HEAD AS MH ON MH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                                         INNER JOIN 
                                                            SUP_HEAD AS H ON FC.HEAD = H.HEAD_ID
                                                         INNER JOIN 
                                                            FEE_FREQUENCY_FEE_MODE AS FFF ON FT.FREQUENCY = FFF.FEE_FREQUENCY_FEE_MODE_ID
                                                         INNER JOIN 
                                                            SUP_FEE_FREQUENCY AS FF ON FFF.FREQUENCY_ID = FF.FREQUENCY_ID
                                                         WHERE 
                                                            FT.TRANSACTION_ID = ?TRANSACTION_ID";

                                    var receiptItems = DapperHandler.FetchAll<FEE_TRANSACTION>(
                                        receiptQuery, new { TRANSACTION_ID = latestReceipt.TRANSACTION_ID }).ToList();

                                    if (receiptItems != null && receiptItems.Count > 0)
                                    {
                                        var staffDetails = DapperHandler.FetchAll<dynamic>(
                                            "SELECT FIRST_NAME, STAFF_CODE FROM STF_PERSONAL_INFO WHERE STAFF_ID = @STAFF_ID", 
                                            new { STAFF_ID = sStaffId }).FirstOrDefault();

                                        if (staffDetails != null)
                                        {
                                            foreach (var item in receiptItems)
                                            {
                                                item.FIRST_NAME = Convert.ToString(staffDetails.FIRST_NAME);
                                                item.REGISTER_NO = Convert.ToString(staffDetails.STAFF_CODE);
                                                item.ROLL_NO = Convert.ToString(staffDetails.STAFF_CODE);
                                            }
                                        }
                                        objViewModel.liFeeTransaction = receiptItems;
                                    }

                                    objViewModel.ReceiptNo = latestReceipt.RECEIPT_NO;
                                    objViewModel.PaymentDate = latestReceipt.PAYMENT_DATE;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("PaymentGatewayController", "AdmissionPaymentResponse", ex.Message);
                }
            }
            return View(objViewModel);
        }

        public ActionResult ConvocationPaymentResponse(string sOrderId)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                if (string.IsNullOrEmpty(sAcademicYear))
                {
                    var academicYears = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchActiveYear)).DataSource.Data;
                    if (academicYears != null && academicYears.Count > 0)
                    {
                        sAcademicYear = academicYears.FirstOrDefault().AC_YEAR;
                    }
                }

                // 0. Fetch College Details Upfront
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;

                // 1. Check Cashfree Order Info
                var cfOrderInfo = (List<CF_ORDER_INFO>)CMSHandler.FetchData<CF_ORDER_INFO>(new CF_ORDER_INFO() { order_id = sOrderId }, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchOrderInfoByOrderId), sAcademicYear).DataSource.Data;

                if (cfOrderInfo != null && cfOrderInfo.Count > 0)
                {
                    var order = cfOrderInfo.FirstOrDefault();
                    var orderResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CF_ORDER_INFO>(order.CF_ORDER_RESPONSE);

                    objViewModel.Status = orderResponse.order_status;
                    objViewModel.Message = (orderResponse.order_status.ToUpper() == "PAID" || orderResponse.order_status.ToUpper() == "SUCCESS") ? "Payment Successful" : "Payment " + orderResponse.order_status;
                    objViewModel.Amount = orderResponse.order_amount;
                    objViewModel.TransactionId = orderResponse.order_id;
                    objViewModel.ReceiptNo = orderResponse.RECEIPT_NO;
                    objViewModel.PaymentDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                    if (orderResponse.order_status.ToUpper() == "PAID" || orderResponse.order_status.ToUpper() == "SUCCESS")
                    {
                        string sStudentId = orderResponse.order_tags.USER_ID;
                        string sFrequencyId = orderResponse.order_tags.FREQUENCY_ID;

                        var fetchTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(
                            new FEE_TRANSACTION() { CF_ORDER_ID = sOrderId },
                            SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTransactionByCashFreeId),
                            sAcademicYear).DataSource.Data;

                        if (fetchTransaction != null && fetchTransaction.Count > 0)
                        {
                            var latestReceipt = fetchTransaction.OrderByDescending(x => x.TRANSACTION_ID).FirstOrDefault();
                            var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT()
                            {
                                TRANSACTION_ID = latestReceipt.TRANSACTION_ID,
                                FREQUENCY_ID = sFrequencyId,
                                ACADEMIC_YEAR = sAcademicYear
                            };
                            objViewModel.liFeeTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeReceiptByTransactionId), sAcademicYear).DataSource.Data;
                            
                            if (objViewModel.liFeeTransaction != null && objViewModel.liFeeTransaction.Count > 0)
                            {
                                var studentDetails = DapperHandler.FetchAll<dynamic>(SQL.Student.StudentSQL.GetStudentSQL(StudentSQLCommads.Getstudentacademicdetailsforalumnae), new { STUDENT_ID = sStudentId }).FirstOrDefault();
                                if (studentDetails == null)
                                {
                                    studentDetails = DapperHandler.FetchAll<dynamic>(SQL.Student.StudentSQL.GetStudentSQL(StudentSQLCommads.GetMBAstudentacademicdetailsforalumnae), new { STUDENT_ID = sStudentId }).FirstOrDefault();
                                }

                                if (studentDetails != null)
                                {
                                    ViewBag.Department = Convert.ToString(studentDetails.DEPARTMENT);
                                    ViewBag.ProgrammeName = Convert.ToString(studentDetails.PROGRAMME_NAME);
                                    ViewBag.YearOfPassing = Convert.ToString(studentDetails.EXPIRED_YEAR);

                                    foreach (var item in objViewModel.liFeeTransaction)
                                    {
                                        item.FIRST_NAME = Convert.ToString(studentDetails.FIRST_NAME);
                                        item.REGISTER_NO = Convert.ToString(studentDetails.REGISTER_NO);
                                        item.ROLL_NO = Convert.ToString(studentDetails.ROLL_NO);
                                    }
                                }
                            }
                            
                            objViewModel.ReceiptNo = latestReceipt.RECEIPT_NO;
                            objViewModel.PaymentDate = latestReceipt.PAYMENT_DATE;
                        }
                    }
                }
                else
                {
                    // 2. Check Edviron Order Info
                    var edvironOrder = DapperHandler.FetchAll<EDVIRON_ORDER_INFO>(EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchOrderByCustomOrderId), new { CUSTOM_ORDER_ID = sOrderId }).ToList();
                    if (edvironOrder != null && edvironOrder.Count > 0)
                    {
                        var order = edvironOrder.FirstOrDefault();
                        objViewModel.Status = order.STATUS;
                        objViewModel.Message = (order.STATUS.ToUpper() == "PAID" || order.STATUS.ToUpper() == "SUCCESS") ? "Payment Successful" : "Payment " + order.STATUS;
                        objViewModel.Amount = order.ACTUAL_AMOUNT ?? order.AMOUNT;
                        objViewModel.TransactionId = order.CUSTOM_ORDER_ID;
                        objViewModel.PaymentDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                        if (order.STATUS.ToUpper() == "PAID" || order.STATUS.ToUpper() == "SUCCESS")
                        {
                            string sStudentId = order.STUDENT_ID;
                            string sFrequencyId = order.FREQUENCY_ID;

                            var fetchTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(
                                new FEE_TRANSACTION() { CF_ORDER_ID = sOrderId },
                                SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTransactionByCashFreeId)
                                    .Replace("?CF_ORDER_ID", "'" + sOrderId + "'")
                                    .Replace("?FREQUENCY", "'" + sFrequencyId + "'"),
                                sAcademicYear
                            ).DataSource.Data;

                            if (fetchTransaction != null && fetchTransaction.Count > 0)
                            {
                                var latestReceipt = fetchTransaction.OrderByDescending(x => x.TRANSACTION_ID).FirstOrDefault();
                                var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT()
                                {
                                    TRANSACTION_ID = latestReceipt.TRANSACTION_ID,
                                    FREQUENCY_ID = sFrequencyId,
                                    ACADEMIC_YEAR = sAcademicYear
                                };
                                objViewModel.liFeeTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeReceiptByTransactionId), sAcademicYear).DataSource.Data;
                                
                                if (objViewModel.liFeeTransaction != null && objViewModel.liFeeTransaction.Count > 0)
                                {
                                    var studentDetails = DapperHandler.FetchAll<dynamic>(SQL.Student.StudentSQL.GetStudentSQL(StudentSQLCommads.Getstudentacademicdetailsforalumnae), new { STUDENT_ID = sStudentId }).FirstOrDefault();
                                    if (studentDetails == null)
                                    {
                                        studentDetails = DapperHandler.FetchAll<dynamic>(SQL.Student.StudentSQL.GetStudentSQL(StudentSQLCommads.GetMBAstudentacademicdetailsforalumnae), new { STUDENT_ID = sStudentId }).FirstOrDefault();
                                    }

                                    if (studentDetails != null)
                                    {
                                        ViewBag.Department = Convert.ToString(studentDetails.DEPARTMENT);
                                        ViewBag.ProgrammeName = Convert.ToString(studentDetails.PROGRAMME_NAME);
                                        ViewBag.YearOfPassing = Convert.ToString(studentDetails.EXPIRED_YEAR);

                                        foreach (var item in objViewModel.liFeeTransaction)
                                        {
                                            item.FIRST_NAME = Convert.ToString(studentDetails.FIRST_NAME);
                                            item.REGISTER_NO = Convert.ToString(studentDetails.REGISTER_NO);
                                            item.ROLL_NO = Convert.ToString(studentDetails.ROLL_NO);
                                        }
                                    }
                                }
                                
                                objViewModel.ReceiptNo = latestReceipt.RECEIPT_NO;
                                objViewModel.PaymentDate = latestReceipt.PAYMENT_DATE;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("PaymentGatewayController", "ConvocationPaymentResponse", ex.Message);
                }
            }
            return View(objViewModel);
        }

        public ActionResult PaymentModeforarrearStudent(string sFrequencyTypeId, string sFrequencyId, string sFeeRootId, string sStudentId, string sAmount, string sProgrammeGroupId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            FeeTransactionViewModel liFeeTransactions = new FeeTransactionViewModel();
            try
            {
                OrderIdResponse objOrderInfo = new OrderIdResponse();
                Dictionary<string, object> input = new Dictionary<string, object>();
                string cf_clientId = string.Empty;
                string cf_secret_id = string.Empty;
                Notes notes = new Notes();
                List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();

                if (string.IsNullOrEmpty(sStudentId))
                {
                    sStudentId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                }

                // Date window check for registered unpaid courses
                if (!string.IsNullOrEmpty(sStudentId) && !string.IsNullOrEmpty(sAcademicYear))
                {
                    string registeredCoursesSql = @"
                        SELECT COURSE_ID 
                        FROM EXAM_REGISTRATION_ARREAR 
                        WHERE STUDENT_ID = @STUDENT_ID 
                          AND ACADEMIC_YEAR = @ACADEMIC_YEAR 
                          AND IS_REGISTERED = '1' 
                          AND IS_PAID = '0' 
                          AND IS_ACTIVE = '1'";
                    var registeredCourses = DapperHandler.FetchAll<dynamic>(registeredCoursesSql, new { STUDENT_ID = sStudentId, ACADEMIC_YEAR = sAcademicYear }, null);

                    if (registeredCourses != null && registeredCourses.Count > 0)
                    {
                        // Fetch student's APPTYPE_ID
                        string appTypeSql = @"
                            SELECT APPTYPE_ID 
                            FROM stu_personal_info
                            WHERE STUDENT_ID = @STUDENT_ID AND IS_DELETED != 1;";
                        var appTypeResult = DapperHandler.FetchAll<dynamic>(appTypeSql, new { STUDENT_ID = sStudentId }, null).FirstOrDefault();
                        string applicationTypeId = appTypeResult != null ? Convert.ToString(appTypeResult.APPTYPE_ID) : "";

                        // Fetch fee settings
                        string settingsSql = @"
                            SELECT COURSE_ID, START_DATE, END_DATE 
                            FROM arrear_course_fee_setting 
                            WHERE APPLICATION_TYPE = @APPLICATION_TYPE 
                              AND IS_ACTIVE = '1' 
                              AND IS_DELETED = '0'";
                        var feeSettingsList = DapperHandler.FetchAll<dynamic>(settingsSql, new { APPLICATION_TYPE = applicationTypeId }, null);

                        DateTime today = DateTime.Today;
                        bool hasLockedCourse = false;

                        foreach (var c in registeredCourses)
                        {
                            string courseId = Convert.ToString(c.COURSE_ID);
                            var setting = feeSettingsList != null ? (feeSettingsList.FirstOrDefault(s => Convert.ToString(s.COURSE_ID) == courseId) 
                                          ?? feeSettingsList.FirstOrDefault(s => Convert.ToString(s.COURSE_ID) == "0")) : null;

                            bool isLocked = true;
                            if (setting != null)
                            {
                                string startDateStr = Convert.ToString(setting.START_DATE);
                                string endDateStr = Convert.ToString(setting.END_DATE);
                                
                                DateTime startDate = DateTime.MinValue;
                                DateTime endDate = DateTime.MaxValue;
                                
                                bool hasStart = DateTime.TryParse(startDateStr, out startDate);
                                bool hasEnd = DateTime.TryParse(endDateStr, out endDate);
                                
                                if (hasStart && hasEnd)
                                {
                                    if (today >= startDate.Date && today <= endDate.Date) isLocked = false;
                                }
                                else if (hasStart)
                                {
                                    if (today >= startDate.Date) isLocked = false;
                                }
                                else if (hasEnd)
                                {
                                    if (today <= endDate.Date) isLocked = false;
                                }
                                else
                                {
                                    isLocked = false;
                                }
                            }

                            if (isLocked)
                            {
                                hasLockedCourse = true;
                                break;
                            }
                        }

                        if (hasLockedCourse)
                        {
                            TempData["ErrorMessage"] = "Payment is locked because the registration window for one or more registered courses has closed.";
                            return RedirectToAction("StudentArrearRegistration", "Examination");
                        }
                    }
                }


                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    liFeeTransactions.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;

                    if (string.IsNullOrEmpty(sFrequencyTypeId)) sFrequencyTypeId = "12";
                    if (string.IsNullOrEmpty(sFeeRootId)) sFeeRootId = Common.FeeRoot.CollegeFee;
                    if (string.IsNullOrEmpty(sFrequencyId)) sFrequencyId = "Arrear";

                    string sSql = SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchArrearStudentAccountFeeInfo)
                        .Replace("?FREQUENCY_TYPE_ID", "'" + sFrequencyTypeId + "'")
                        .Replace("?FREQUENCY_ID", "'" + sFrequencyId + "'")
                        .Replace("?FEE_ROOT_ID", "'" + sFeeRootId + "'")
                        .Replace("?STUDENT_ID", "'" + sStudentId + "'");

                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(
                        null, 
                        sSql, 
                        sAcademicYear).DataSource.Data;

                    liFeeTransactions.liFeeStudentAccount = studentAcountList;

                    liFeeTransactions.liPAYMENT_MODE_CHARGES = CalculateTransactionFee(studentAcountList, (List<FEE_PAYMENT_MODE_CHARGES>)CMSHandler.FetchData<FEE_PAYMENT_MODE_CHARGES>(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchPaymentModes)).DataSource.Data);

                    if (studentAcountList.Sum(o => decimal.Parse(o.BALANCE)) <= 0)
                    {
                        return RedirectToAction("DashBoard", "dashboard");
                    }
                }
            }
            catch (Exception ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("PaymentGatewayController", "PaymentMode", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("PaymentGatewayController", "PaymentMode", ex.Message);
                }
            }
            return View(liFeeTransactions);



        }
    }
}
