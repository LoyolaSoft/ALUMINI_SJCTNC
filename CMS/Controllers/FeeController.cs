using CMS.DAO;
using MySql.Data.MySqlClient;
using CMS.Models;
using CMS.SQL.Examination;
using CMS.SQL.Fee;
using CMS.SQL.SupportData;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using Razorpay.Api;
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
using CMS.SQL.TimeTable;
using OfficeOpenXml;
using System.Web;
using System.Web.Management;
using CMS.SQL.Website;
using CMS.SQL.Student;
using System.Data.Entity.Infrastructure;
using CMS.SQL.Communication;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http.Results;
using Dapper;



namespace CMS.Controllers
{
    public class FeeController : Controller
    {
        string sresult = string.Empty;
        int Number = 0;
        public string sCreditCount { get; set; }
        public string sSender { get; set; }
        string sBaseUrl = string.Empty;
        string sFormat = string.Empty;
        string sMethod = string.Empty;
        string sAPI = string.Empty;
        string sAPIKey = string.Empty;
        public string sUser { get; set; }
        public string sAccusage { get; set; }
        public string sEntityId { get; set; }
        string sResult = string.Empty;
        public string sSmsMode = string.Empty;
        public string sClassIds { get; set; }
        int nLogcount = 0;
        string sUniCode = string.Empty;
        public string sTextMessage { get; set; }
        Int32 nSMSTimeOut = 0;
        int iReceivedCredits = 0;
        int iSMSLimit = 0;
        int iSMSSent = 0;
        int SMSCount = 0;
        int iUsedCredit = 0;
        int iTotalSendCreditCount = 0;
        string sStartTime { get; set; }
        string sTemplateId = string.Empty;
        string sTransactionId = string.Empty;
        string sJsonString = string.Empty;
        public int sSmsCount = 0;
        public string sResultMessageSMS { get; set; }
        List<SMS_SETTING> liSMSSetting = new List<SMS_SETTING>();
        public string sReceipientNo { get; set; }
        public List<MessageResult.sms> liSMS = new List<MessageResult.sms>();
        public List<MessageResult.listsms> listbulkSMS = new List<MessageResult.listsms>();
        JsonResultData ObjJsonDataSMS = new JsonResultData();
        ResultArgs resultArgsSMS = new ResultArgs();
        public string jsonMonth;

        #region HostelAndMess
        public ActionResult ListHostelFeeStructure()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListHostelFeeStructure", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListHostelFeeStructure", "Fee", ex.Message);
                }
            }
            return View(objViewModel);

        }
        public ActionResult ListMessFeeStructure()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListHostelFeeStructure", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListHostelFeeStructure", "Fee", ex.Message);
                }
            }
            return View(objViewModel);

        }
        public ActionResult MessFeeStudentAccountCreation()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindHostelStudentsByAcademicYear(string Academicyear)
        {
            //string sFrequency,
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;
                    objModel.ACADEMIC_YEAR = Academicyear;
                    objViewModel.lstHostelStudentInfo = (List<HOSTEL_STUDENT_INFO>)CMSHandler.FetchData<HOSTEL_STUDENT_INFO>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelStudentsByAcademicYear), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindStudentsByAcademicYearAndClassId", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindStudentsByAcademicYearAndClassId", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult HostelFeeStudentAccountCreation()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        #endregion

        #region RazorPay

        public JsonResultData UpdateSettlements()
        {
            JsonResultData oResult = new JsonResultData();
            try
            {

                string sSQL = string.Empty;
                string sAcademicYear = string.Empty;
                string secret = string.Empty; string key = string.Empty;
                sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                List<FEE_RAZORPAY_TRANSFER_INFO> transfers = new List<FEE_RAZORPAY_TRANSFER_INFO>();
                transfers = (List<FEE_RAZORPAY_TRANSFER_INFO>)CMSHandler.FetchData<FEE_RAZORPAY_TRANSFER_INFO>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorPayTransfersForSettlemetUpdate), sAcademicYear).DataSource.Data;
                var FetchMarchantAccount = (List<FEE_RAZORPAY_ACCOUNTS>)CMSHandler.FetchData<FEE_RAZORPAY_ACCOUNTS>(new FEE_RAZORPAY_ACCOUNTS() { RAZORPAY_ACCOUNT_TYPE_ID = Common.RazorpayAccountType.MarchantAccount }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayAccountInfo), sAcademicYear).DataSource.Data;
                //Update OrderIds
                UpdateOrderId(sAcademicYear);
                UpdatePaymentsByOrderId(sAcademicYear);

                if (FetchMarchantAccount != null & FetchMarchantAccount.Count() > 0)
                {
                    key = FetchMarchantAccount.FirstOrDefault().KEY;
                    secret = FetchMarchantAccount.FirstOrDefault().SECRET_KEY;
                    if (transfers != null && transfers.Count > 0)
                    {
                        foreach (var item in transfers)
                        {
                            TransferItemResponse transferResponse = JsonConvert.DeserializeObject<TransferItemResponse>(CommonMethods.GetMethodForRazorpay("https://api.razorpay.com/v1/transfers/" + item.ID, key, secret));
                            if (transferResponse != null && !string.IsNullOrEmpty(transferResponse.recipient_settlement_id))
                            {
                                sSQL += "UPDATE FEE_RAZORPAY_TRANSFER_INFO_" + sAcademicYear + " SET RECIPIENT_SETTLEMENT_ID='" + transferResponse.recipient_settlement_id + "' WHERE RAZORPAY_TRANSFER_ID='" + item.RAZORPAY_TRANSFER_ID + "';";
                            }
                        }
                        using (MySQLHandler oHandler = new MySQLHandler())
                        {
                            oHandler.ExecuteAsScripts(sSQL);
                            oResult.ErrorCode = Common.ErrorCode.Ok;
                            oResult.Message = "Updated Successfully";
                        }
                    }
                }
                UpdateSettlementsInfo(sAcademicYear);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "UpdateSettlements", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "UpdateSettlements", ex.Message);
                }

            }

            return oResult;
        }
        public void UpdateSettlementsInfo(string sAcademicYear)
        {
            JsonResultData oResult = new JsonResultData();
            string sSQL = string.Empty;

            string secret = string.Empty; string key = string.Empty;
            //sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            List<FEE_RAZORPAY_TRANSFER_INFO> transfers = new List<FEE_RAZORPAY_TRANSFER_INFO>();
            transfers = (List<FEE_RAZORPAY_TRANSFER_INFO>)CMSHandler.FetchData<FEE_RAZORPAY_TRANSFER_INFO>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorPaySettlementIds), sAcademicYear).DataSource.Data;
            var FetchMarchantAccount = (List<FEE_RAZORPAY_ACCOUNTS>)CMSHandler.FetchData<FEE_RAZORPAY_ACCOUNTS>(new FEE_RAZORPAY_ACCOUNTS() { RAZORPAY_ACCOUNT_TYPE_ID = Common.RazorpayAccountType.MarchantAccount }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayAccountInfo), sAcademicYear).DataSource.Data;

            if (FetchMarchantAccount != null & FetchMarchantAccount.Count() > 0)
            {
                key = FetchMarchantAccount.FirstOrDefault().KEY;
                secret = FetchMarchantAccount.FirstOrDefault().SECRET_KEY;
                if (transfers != null && transfers.Count > 0)
                {
                    sSQL = @"INSERT INTO `FEE_RAZORPAY_SETTLEMENT_INFO_" + sAcademicYear + @"`
                            (
                            `ID`,
                            `ENTITY`,
                            `AMOUNT`,
                            `STATUS`,
                            `FEES`,
                            `TAX`,
                            `UTR`,
                            `CREATED_AT`,
                            `RECIPIENT`)
                            VALUES";
                    foreach (var item in transfers)
                    {
                        TransferResponse transferResponse = JsonConvert.DeserializeObject<TransferResponse>(CommonMethods.GetMethodForRazorpay("https://api.razorpay.com/v1/transfers?recipient_settlement_id=" + item.RECIPIENT_SETTLEMENT_ID + "&expand[]=recipient_settlement&count=1", key, secret));
                        if (transferResponse != null && transferResponse.count > 0)
                        {
                            var FetchSettlementInfo = (List<FEE_RAZORPAY_SETTLEMENT_INFO>)CMSHandler.FetchData<FEE_RAZORPAY_SETTLEMENT_INFO>(new FEE_RAZORPAY_SETTLEMENT_INFO() { ID = item.RECIPIENT_SETTLEMENT_ID }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorPaySettlementInfoById), sAcademicYear).DataSource.Data;
                            if (FetchSettlementInfo == null)
                            {
                                sSQL += "('" + transferResponse.items.FirstOrDefault().recipient_settlement.id + "','" + transferResponse.items.FirstOrDefault().recipient_settlement.entity + "','" + transferResponse.items.FirstOrDefault().recipient_settlement.amount + "','" + transferResponse.items.FirstOrDefault().recipient_settlement.status + "','" + transferResponse.items.FirstOrDefault().recipient_settlement.fees + "','" + transferResponse.items.FirstOrDefault().recipient_settlement.tax + "','" + transferResponse.items.FirstOrDefault().recipient_settlement.utr + "','" + transferResponse.items.FirstOrDefault().recipient_settlement.created_at + "','" + transferResponse.items.FirstOrDefault().recipient + "'),";
                            }

                        }
                    }
                    sSQL = sSQL.TrimEnd(',');
                    using (MySQLHandler oHandler = new MySQLHandler())
                    {
                        oHandler.ExecuteAsScripts(sSQL);
                        oResult.ErrorCode = Common.ErrorCode.Ok;
                        oResult.Message = "Updated Successfully";
                    }
                }
            }
        }


        public void UpdateOrderId(string sAcademicYear)
        {
            JsonResultData oResult = new JsonResultData();
            string sSQL = string.Empty;

            string secret = string.Empty; string key = string.Empty;
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            List<FEE_RAZORPAY_ORDER_INFO> Orders = new List<FEE_RAZORPAY_ORDER_INFO>();
            Orders = (List<FEE_RAZORPAY_ORDER_INFO>)CMSHandler.FetchData<FEE_RAZORPAY_ORDER_INFO>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorPayOrderInfoForUpdate), sAcademicYear).DataSource.Data;
            var FetchMarchantAccount = (List<FEE_RAZORPAY_ACCOUNTS>)CMSHandler.FetchData<FEE_RAZORPAY_ACCOUNTS>(new FEE_RAZORPAY_ACCOUNTS() { RAZORPAY_ACCOUNT_TYPE_ID = Common.RazorpayAccountType.MarchantAccount }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayAccountInfo), sAcademicYear).DataSource.Data;

            if (FetchMarchantAccount != null & FetchMarchantAccount.Count() > 0)
            {
                key = FetchMarchantAccount.FirstOrDefault().KEY;
                secret = FetchMarchantAccount.FirstOrDefault().SECRET_KEY;
                if (Orders != null && Orders.Count > 0)
                {
                    foreach (var item in Orders)
                    {
                        OrderIdResponse orderResponse = JsonConvert.DeserializeObject<OrderIdResponse>(CommonMethods.GetMethodForRazorpay("https://api.razorpay.com/v1/orders/" + item.ID, key, secret));
                        if (orderResponse != null)
                        {
                            if (orderResponse.status.ToLower() == "paid")
                                sSQL += "UPDATE FEE_RAZORPAY_ORDER_INFO_" + sAcademicYear + " SET STATUS='" + orderResponse.status + "' WHERE RAZORPAY_ORDER_ID='" + item.RAZORPAY_ORDER_ID + "';";
                        }
                    }
                    using (MySQLHandler oHandler = new MySQLHandler())
                    {
                        oHandler.ExecuteAsScripts(sSQL);
                        oResult.ErrorCode = Common.ErrorCode.Ok;
                        oResult.Message = "Updated Successfully";
                    }
                }
            }

        }

        public void UpdatePaymentsByOrderId(string sAcademicYear)
        {
            JsonResultData oResult = new JsonResultData();
            string sSQL = string.Empty;

            string secret = string.Empty; string key = string.Empty;
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            List<FEE_RAZORPAY_ORDER_INFO> Orders = new List<FEE_RAZORPAY_ORDER_INFO>();
            Orders = (List<FEE_RAZORPAY_ORDER_INFO>)CMSHandler.FetchData<FEE_RAZORPAY_ORDER_INFO>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorPayOrderInfoByPaidStatus), sAcademicYear).DataSource.Data;
            var FetchMarchantAccount = (List<FEE_RAZORPAY_ACCOUNTS>)CMSHandler.FetchData<FEE_RAZORPAY_ACCOUNTS>(new FEE_RAZORPAY_ACCOUNTS() { RAZORPAY_ACCOUNT_TYPE_ID = Common.RazorpayAccountType.MarchantAccount }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayAccountInfo), sAcademicYear).DataSource.Data;

            if (FetchMarchantAccount != null & FetchMarchantAccount.Count() > 0)
            {
                key = FetchMarchantAccount.FirstOrDefault().KEY;
                secret = FetchMarchantAccount.FirstOrDefault().SECRET_KEY;
                if (Orders != null && Orders.Count > 0)
                {
                    foreach (var item in Orders)
                    {
                        TransferResponse orderResponse = JsonConvert.DeserializeObject<TransferResponse>(CommonMethods.GetMethodForRazorpay("https://api.razorpay.com/v1/payments?order_id=" + item.ID, key, secret));
                        if (orderResponse != null)
                        {
                            foreach (var innerItem in orderResponse.items)
                            {
                                if (innerItem.captured == true)
                                {
                                    RazorPayPaymentResponseEntryBySystem(orderResponse.items.FirstOrDefault().id);
                                }
                            }
                        }
                    }
                }
            }

        }
        //this method is a entry point for razor pay 
        public ActionResult RazorPayPaymentGate(string sFrequencyTypeId, string sFeeRootId, string sFrequencyId, string sAcademicYear)
        {

            FeeTransactionViewModel liFeeTransactions = new FeeTransactionViewModel();
            OrderIdResponse objOrderInfo = new OrderIdResponse();
            Dictionary<string, object> input = new Dictionary<string, object>();
            Notes notes = new Notes();
            try
            {
                string key = string.Empty;
                string secret = string.Empty;
                List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    liFeeTransactions.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    //string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = sFrequencyTypeId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                    var FetchMarchantAccount = (List<FEE_RAZORPAY_ACCOUNTS>)CMSHandler.FetchData<FEE_RAZORPAY_ACCOUNTS>(new FEE_RAZORPAY_ACCOUNTS() { RAZORPAY_ACCOUNT_TYPE_ID = Common.RazorpayAccountType.MarchantAccount }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayAccountInfo), sAcademicYear).DataSource.Data;

                    if (FetchMarchantAccount != null & FetchMarchantAccount.Count() > 0)
                    {
                        key = FetchMarchantAccount.FirstOrDefault().KEY;
                        secret = FetchMarchantAccount.FirstOrDefault().SECRET_KEY;
                    }

                    if (sFrequencyTypeId == Common.FrequencyType.SemesterFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_TYPE_ID = Common.FrequencyType.SemesterFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.MessFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_TYPE_ID = Common.FrequencyType.MessFee, FEE_ROOT_ID = Common.FeeRoot.MessFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FecthMessFeesByStudentIdAndFeeRootId), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.HostelFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_TYPE_ID = Common.FrequencyType.HostelFee, FEE_ROOT_ID = Common.FeeRoot.HostelFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FecthMessFeesByStudentIdAndFeeRootId), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.ExamFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_TYPE_ID = Common.FrequencyType.ExamFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.AdmissionFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_TYPE_ID = Common.FrequencyType.AdmissionFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.LibraryFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_TYPE_ID = Common.FrequencyType.LibraryFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.ConvocationFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_TYPE_ID = Common.FrequencyType.ConvocationFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.Supplementary)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_TYPE_ID = Common.FrequencyType.Supplementary, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.RevaluationFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_TYPE_ID = Common.FrequencyType.RevaluationFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.Certificate)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_TYPE_ID = Common.FrequencyType.Certificate, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.Term_II)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_TYPE_ID = Common.FrequencyType.Term_II, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    liFeeTransactions.liFeeStudentAccount = studentAcountList;
                    liFeeTransactions.liFeeStudentAccount.FirstOrDefault().ACADEMIC_YEAR = sAcademicYear;
                    if (studentAcountList.Sum(o => decimal.Parse(o.BALANCE)) <= 0)
                    {
                        return RedirectToAction("STUDENT", "dashboard");
                    }
                    var FetchOrderInfo = (List<FEE_RAZORPAY_ORDER_INFO>)CMSHandler.FetchData<FEE_RAZORPAY_ORDER_INFO>(new FEE_RAZORPAY_ORDER_INFO()
                    {
                        UDF1 = Session[Common.SESSION_VARIABLES.USER_ID].ToString(),
                        UDF2 = sFrequencyTypeId,
                        UDF5 = sFeeRootId,
                        UDF3 = sFrequencyId, //FetchFrequency.FirstOrDefault().FREQUENCY_ID,
                        AMOUNT = studentAcountList.Sum(x => decimal.Parse(x.BALANCE) * 100).ToString()
                    }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayOrderInfoByUDF), sAcademicYear).DataSource.Data;
                    if (FetchOrderInfo != null && FetchOrderInfo.Count > 0)
                    {
                        objOrderInfo.id = FetchOrderInfo.FirstOrDefault().ID;
                        objOrderInfo.amount = (decimal.Parse(FetchOrderInfo.FirstOrDefault().AMOUNT)).ToString();
                        objOrderInfo.notes.udf1 = FetchOrderInfo.FirstOrDefault().UDF1;
                        objOrderInfo.notes.udf2 = FetchOrderInfo.FirstOrDefault().UDF2;
                        objOrderInfo.notes.udf3 = FetchOrderInfo.FirstOrDefault().UDF3;
                        objOrderInfo.notes.udf4 = FetchOrderInfo.FirstOrDefault().UDF4;
                        objOrderInfo.notes.udf5 = FetchOrderInfo.FirstOrDefault().UDF5;
                        liFeeTransactions.objOrderId = objOrderInfo;
                    }
                    else
                    {
                        notes.udf1 = studentAcountList.FirstOrDefault().STUDENT_ID;
                        notes.udf2 = sFrequencyTypeId;
                        notes.udf3 = sFrequencyId;// FetchFrequency.FirstOrDefault().FREQUENCY_ID;
                        notes.udf4 = studentAcountList.FirstOrDefault().REGISTER_NO;
                        notes.udf5 = sFeeRootId;
                        input.Add("amount", studentAcountList.Sum(x => decimal.Parse(x.BALANCE) * 100)); // this amount should be same as transaction amount
                        input.Add("currency", "INR");
                        //input.Add("receipt", "12121");
                        input.Add("payment_capture", 1);
                        input.Add("notes", notes);
                        RazorpayClient client = new RazorpayClient(key, secret);
                        var i = client.Order.Create(input).Attributes;
                        liFeeTransactions.objOrderId = JsonConvert.DeserializeObject<OrderIdResponse>(((Newtonsoft.Json.Linq.JToken)client.Order.Create(input).Attributes).Root.ToString());
                        if (liFeeTransactions.objOrderId != null)
                        {
                            var result = CMSHandler.SaveOrUpdate(new FEE_RAZORPAY_ORDER_INFO()
                            {
                                AMOUNT = liFeeTransactions.objOrderId.amount,
                                AMOUNT_DUE = liFeeTransactions.objOrderId.amount_due,
                                AMOUNT_PAID = liFeeTransactions.objOrderId.amount_paid,
                                ATTEMPTS = liFeeTransactions.objOrderId.attempts,
                                CREATED_AT = liFeeTransactions.objOrderId.created_at,
                                ENTITY = liFeeTransactions.objOrderId.entity,
                                ID = liFeeTransactions.objOrderId.id,
                                OFFER_ID = liFeeTransactions.objOrderId.offer_id,
                                RECEIPT = liFeeTransactions.objOrderId.receipt,
                                STATUS = liFeeTransactions.objOrderId.status,
                                CURRENCY = liFeeTransactions.objOrderId.currency,
                                UDF1 = liFeeTransactions.objOrderId.notes.udf1,
                                UDF2 = liFeeTransactions.objOrderId.notes.udf2,
                                UDF3 = liFeeTransactions.objOrderId.notes.udf3,
                                UDF4 = liFeeTransactions.objOrderId.notes.udf4,
                                UDF5 = liFeeTransactions.objOrderId.notes.udf5,

                            }, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveRazorpayOrderInfo), sAcademicYear);
                            if (result.Success)
                            {
                                return View(liFeeTransactions);
                            }
                            else
                            {

                            }
                        }
                        else
                        {

                        }

                    }
                    liFeeTransactions.objOrderId.key = key;
                    liFeeTransactions.objOrderId.secret = secret;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
                // return View(studentAcountList);

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "RazorPayPaymentGate(string sFrequencyTypeId, string sFeeRootId)", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "RazorPayPaymentGate(string sFrequencyTypeId, string sFeeRootId)", ex.Message);
                }
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(liFeeTransactions);
        }
        // this method is response point for razor pay
        public ActionResult RazorPayPaymentResponse(string PaymentId, string sAcademicYear)
        {
            FeeTransactionViewModel liFeeTransactions = new FeeTransactionViewModel();
            OrderIdResponse objOrderInfo = new OrderIdResponse();
            List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
            Dictionary<string, object> input = new Dictionary<string, object>();
            Notes notes = new Notes();
            string key = string.Empty;
            string secret = string.Empty;
            string sRazorpayPaymentId = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var FetchMarchantAccount = (List<FEE_RAZORPAY_ACCOUNTS>)CMSHandler.FetchData<FEE_RAZORPAY_ACCOUNTS>(new FEE_RAZORPAY_ACCOUNTS() { RAZORPAY_ACCOUNT_TYPE_ID = Common.RazorpayAccountType.MarchantAccount }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayAccountInfo), sAcademicYear).DataSource.Data;
                    if (FetchMarchantAccount != null & FetchMarchantAccount.Count > 0)
                    {
                        key = FetchMarchantAccount.FirstOrDefault().KEY;
                        secret = FetchMarchantAccount.FirstOrDefault().SECRET_KEY;
                    }
                    RazorpayClient client = new RazorpayClient(key, secret);
                    Payment paymentResponse = client.Payment.Fetch(PaymentId);
                    PaymentResponse objPaymentInfo = new PaymentResponse();
                    objPaymentInfo = JsonConvert.DeserializeObject<PaymentResponse>(((Newtonsoft.Json.Linq.JToken)paymentResponse.Attributes).Root.ToString());
                    var o = (FEE_RAZORPAY_PAYMENT_INFO)CommonMethods.PropertyMapping<PaymentResponse, FEE_RAZORPAY_PAYMENT_INFO>(objPaymentInfo, null);

                    o.UDF1 = objPaymentInfo.notes.udf1;
                    o.UDF2 = objPaymentInfo.notes.udf2;
                    o.UDF3 = objPaymentInfo.notes.udf3;
                    o.UDF4 = objPaymentInfo.notes.udf4;
                    o.UDF5 = objPaymentInfo.notes.udf5;
                    var isExisting = (List<FEE_RAZORPAY_PAYMENT_INFO>)CMSHandler.FetchData<FEE_RAZORPAY_PAYMENT_INFO>(new FEE_RAZORPAY_PAYMENT_INFO() { ID = PaymentId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayPaymentInfoById), sAcademicYear).DataSource.Data;
                    if (isExisting != null && isExisting.Count > 0)
                    {
                        return View(studentAcountList);
                    }
                    var restult = CMSHandler.SaveOrUpdate(o, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveRazorpayPaymentInfo), sAcademicYear);

                    if (restult.Success)
                    {
                        var FetchRazorpayPaymentInfo = (List<FEE_RAZORPAY_PAYMENT_INFO>)CMSHandler.FetchData<FEE_RAZORPAY_PAYMENT_INFO>(new FEE_RAZORPAY_PAYMENT_INFO() { ID = o.ID }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayPaymentInfoById), sAcademicYear).DataSource.Data;
                        sRazorpayPaymentId = (FetchRazorpayPaymentInfo != null && FetchRazorpayPaymentInfo.Count > 0) ? FetchRazorpayPaymentInfo.FirstOrDefault().RAZORPAY_PAMENT_ID : string.Empty;
                        var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = objPaymentInfo.notes.udf2 }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                        var result = FeeRazorpayCollection(o, sRazorpayPaymentId, sAcademicYear);

                        if (!string.IsNullOrEmpty(result.RECEIPT_NO))
                        {
                            FeeRazorpayTransferAccount(sRazorpayPaymentId, o.UDF5, sAcademicYear);
                            if (o.UDF5 == Common.FeeRoot.CollegeFee)
                                studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_RAZORPAY_PAYMENT_INFO() { RAZORPAY_PAMENT_ID = sRazorpayPaymentId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayPaymentStatusForCollegeTranctions), sAcademicYear).DataSource.Data;
                            else if (o.UDF5 == Common.FeeRoot.MessFee)
                                studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_RAZORPAY_PAYMENT_INFO() { RAZORPAY_PAMENT_ID = sRazorpayPaymentId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayPaymentStatusForNonCollegeTransactions), sAcademicYear).DataSource.Data;
                            else if (o.UDF5 == Common.FeeRoot.HostelFee)
                                studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_RAZORPAY_PAYMENT_INFO() { RAZORPAY_PAMENT_ID = sRazorpayPaymentId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayPaymentStatusForNonCollegeTransactions), sAcademicYear).DataSource.Data;
                            else if (o.UDF5 == Common.FeeRoot.HostelFee)
                                studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_RAZORPAY_PAYMENT_INFO() { RAZORPAY_PAMENT_ID = sRazorpayPaymentId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayPaymentStatusForNonCollegeTransactions), sAcademicYear).DataSource.Data;
                            else if (o.UDF5 == Common.FeeRoot.HostelAndMess)
                            {
                            }
                        }
                    }
                    else
                    {
                    }
                }
                catch (Exception ex)
                {

                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "RazorPayPaymentResponse(string PaymentId)", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "RazorPayPaymentResponse(string PaymentId)", ex.Message);
                    }
                }


            }
            else
                return RedirectToAction("ErrorMessage", "Error");

            return View(studentAcountList);
        }
        public void RazorPayPaymentResponseEntryBySystem(string PaymentId)
        {
            FeeTransactionViewModel liFeeTransactions = new FeeTransactionViewModel();
            OrderIdResponse objOrderInfo = new OrderIdResponse();
            List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
            Dictionary<string, object> input = new Dictionary<string, object>();
            Notes notes = new Notes();
            string key = string.Empty;
            string secret = string.Empty;
            string sRazorpayPaymentId = string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var FetchMarchantAccount = (List<FEE_RAZORPAY_ACCOUNTS>)CMSHandler.FetchData<FEE_RAZORPAY_ACCOUNTS>(new FEE_RAZORPAY_ACCOUNTS() { RAZORPAY_ACCOUNT_TYPE_ID = Common.RazorpayAccountType.MarchantAccount }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayAccountInfo), sAcademicYear).DataSource.Data;
                    if (FetchMarchantAccount != null & FetchMarchantAccount.Count > 0)
                    {
                        key = FetchMarchantAccount.FirstOrDefault().KEY;
                        secret = FetchMarchantAccount.FirstOrDefault().SECRET_KEY;
                    }
                    RazorpayClient client = new RazorpayClient(key, secret);
                    Payment paymentResponse = client.Payment.Fetch(PaymentId);
                    PaymentResponse objPaymentInfo = new PaymentResponse();
                    objPaymentInfo = JsonConvert.DeserializeObject<PaymentResponse>(((Newtonsoft.Json.Linq.JToken)paymentResponse.Attributes).Root.ToString());
                    var o = (FEE_RAZORPAY_PAYMENT_INFO)CommonMethods.PropertyMapping<PaymentResponse, FEE_RAZORPAY_PAYMENT_INFO>(objPaymentInfo, null);

                    o.UDF1 = objPaymentInfo.notes.udf1;
                    o.UDF2 = objPaymentInfo.notes.udf2;
                    o.UDF3 = objPaymentInfo.notes.udf3;
                    o.UDF4 = objPaymentInfo.notes.udf4;
                    o.UDF5 = objPaymentInfo.notes.udf5;
                    var isExisting = (List<FEE_RAZORPAY_PAYMENT_INFO>)CMSHandler.FetchData<FEE_RAZORPAY_PAYMENT_INFO>(new FEE_RAZORPAY_PAYMENT_INFO() { ID = PaymentId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayPaymentInfoById), sAcademicYear).DataSource.Data;
                    if (isExisting == null)
                    {
                        var restult = CMSHandler.SaveOrUpdate(o, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveRazorpayPaymentInfo), sAcademicYear);
                        if (restult.Success)
                        {
                            var FetchRazorpayPaymentInfo = (List<FEE_RAZORPAY_PAYMENT_INFO>)CMSHandler.FetchData<FEE_RAZORPAY_PAYMENT_INFO>(new FEE_RAZORPAY_PAYMENT_INFO() { ID = o.ID }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayPaymentInfoById), sAcademicYear).DataSource.Data;
                            sRazorpayPaymentId = (FetchRazorpayPaymentInfo != null && FetchRazorpayPaymentInfo.Count > 0) ? FetchRazorpayPaymentInfo.FirstOrDefault().RAZORPAY_PAMENT_ID : string.Empty;
                            var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = objPaymentInfo.notes.udf2 }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                            var result = FeeRazorpayCollection(o, sRazorpayPaymentId, sAcademicYear);

                            if (!string.IsNullOrEmpty(result.RECEIPT_NO))
                            {
                                FeeRazorpayTransferAccount(sRazorpayPaymentId, o.UDF5, sAcademicYear);
                                if (o.UDF5 == Common.FeeRoot.CollegeFee)
                                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_RAZORPAY_PAYMENT_INFO() { RAZORPAY_PAMENT_ID = sRazorpayPaymentId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayPaymentStatusForCollegeTranctions), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                                else if (o.UDF5 == Common.FeeRoot.MessFee)
                                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_RAZORPAY_PAYMENT_INFO() { RAZORPAY_PAMENT_ID = sRazorpayPaymentId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayPaymentStatusForNonCollegeTransactions), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                                else if (o.UDF5 == Common.FeeRoot.HostelFee)
                                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_RAZORPAY_PAYMENT_INFO() { RAZORPAY_PAMENT_ID = sRazorpayPaymentId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayPaymentStatusForNonCollegeTransactions), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                                else if (o.UDF5 == Common.FeeRoot.HostelAndMess)
                                {
                                }
                            }
                        }
                        else
                        {
                        }
                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "RazorPayPaymentResponseEntryBySystem(string PaymentId)", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "RazorPayPaymentResponseEntryBySystem(string PaymentId)", ex.Message);
                    }
                }


            }
            else { }
            // return RedirectToAction("ErrorMessage", "Error");

            //return View(studentAcountList);
        }
        //this method is make route transactions
        private bool FeeRazorpayTransferAccount(string sRazorpayId, string sFeeRootId, string sAcademicYear)
        {
            bool bResult = false;
            try
            {
                List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
                ArrayList lstTransfer = new ArrayList();
                Dictionary<string, object> options = new Dictionary<string, object>();
                string key = string.Empty;
                string secret = string.Empty;
                var FetchMarchantAccount = (List<FEE_RAZORPAY_ACCOUNTS>)CMSHandler.FetchData<FEE_RAZORPAY_ACCOUNTS>(new FEE_RAZORPAY_ACCOUNTS() { RAZORPAY_ACCOUNT_TYPE_ID = Common.RazorpayAccountType.MarchantAccount }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayAccountInfo), sAcademicYear).DataSource.Data;
                if (FetchMarchantAccount != null & FetchMarchantAccount.Count > 0)
                {
                    key = FetchMarchantAccount.FirstOrDefault().KEY;
                    secret = FetchMarchantAccount.FirstOrDefault().SECRET_KEY;
                }
                if (sFeeRootId == Common.FeeRoot.CollegeFee)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_RAZORPAY_PAYMENT_INFO() { RAZORPAY_PAMENT_ID = sRazorpayId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayTransferListForCollegeTransactoins), sAcademicYear).DataSource.Data;
                else if (sFeeRootId == Common.FeeRoot.MessFee)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_RAZORPAY_PAYMENT_INFO() { RAZORPAY_PAMENT_ID = sRazorpayId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayTransferListForNonCollegeTransactoins), sAcademicYear).DataSource.Data;
                else if (sFeeRootId == Common.FeeRoot.HostelFee)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_RAZORPAY_PAYMENT_INFO() { RAZORPAY_PAMENT_ID = sRazorpayId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayTransferListForNonCollegeTransactoins), sAcademicYear).DataSource.Data;
                else if (sFeeRootId == Common.FeeRoot.HostelAndMess)
                {
                }
                if (studentAcountList != null && studentAcountList.Count > 0)
                {
                    foreach (var item in studentAcountList)
                    {
                        ListDictionary listDictionary = new ListDictionary();
                        listDictionary.Add(Common.RazorpayKeyWords.AMOUNT, (decimal.Parse(item.BALANCE) * 100).ToString());
                        listDictionary.Add(Common.RazorpayKeyWords.account, item.ACCOUNT_ID);
                        listDictionary.Add(Common.RazorpayKeyWords.CURRENCY, Common.RazorpayKeyWords.INR);
                        listDictionary.Add(Common.RazorpayKeyWords.NOTES, new Notes() { udf1 = item.STUDENT_ID, udf2 = item.RAZORPAY_ID, udf3 = item.MAIN_HEAD_ID, udf4 = item.BANK_ACCOUNT_ID });
                        lstTransfer.Add(listDictionary);
                    }

                    options.Add(Common.RazorpayKeyWords.TRANSFERS, lstTransfer);
                    RazorpayClient client = new RazorpayClient(key, secret);
                    Payment payments = client.Payment.Fetch(studentAcountList.FirstOrDefault().ID);
                    List<Transfer> transfer = payments.Transfer(options);
                    if (transfer != null && transfer.Count > 0)
                    {
                        string sSQL = @"INSERT INTO `FEE_RAZORPAY_TRANSFER_INFO_?AC_YEAR`
                                    (
                                    `ID`,
                                    `ENTITY`,
                                    `SOURCE`,
                                    `RECIPIENT`,
                                    `AMOUNT`,
                                    `CURRENCY`,
                                    `AMOUNT_REVERSED`,
                                    `FEES`,
                                    `TAX`,
                                    `ON_HOLD`,
                                    `ON_HOLD_UNTIL`,
                                    `RECIPIENT_SETTLEMENT_ID`,
                                    `CREATED_AT`,
                                    `UDF1`,
                                    `UDF2`,
                                    `UDF3`,
                                    `UDF4`,
                                    `UDF5`,
                                    `UDF6`)
                                    VALUES ";
                        foreach (var transferItem in transfer)
                        {
                            TransferItemResponse item = JsonConvert.DeserializeObject<TransferItemResponse>(transferItem.Attributes.ToString());
                            sSQL += @"('" + item.id + "','" + item.entity + "','" + item.source + "','" + item.recipient + "','" + item.amount + "','" + item.currency + "'" +
                                ",'" + item.amount_reversed + "','" + item.fees + "','" + item.tax + "','" + item.on_hold + "','" + item.on_hold_until + "'" +
                                ",'" + item.recipient_settlement_id + "','" + item.created_at + "','" + item.notes.udf1 + "','" + item.notes.udf2 + "','" + item.notes.udf3 + "'" +
                                ",'" + item.notes.udf4 + "','" + item.notes.udf5 + "','" + item.notes.udf6 + "'),";
                        }
                        sSQL = sSQL.TrimEnd(',');
                        var result = CMSHandler.SaveOrUpdate(null, sSQL, sAcademicYear);
                        bResult = result.Success;
                    }

                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "FeeRazorpayTransferAccount(string sRazorpayId, string sFeeRootId,string sAcademicYear)", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "FeeRazorpayTransferAccount(string sRazorpayId, string sFeeRootId,string sAcademicYear)", ex.Message);
                }

            }


            return bResult;
        }
        // test
        public JsonResult FeeTransferInsert()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            try
            {
                List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
                ArrayList lstTransfer = new ArrayList();
                Dictionary<string, object> options = new Dictionary<string, object>();
                string key = string.Empty;
                string secret = string.Empty;
                var FetchMarchantAccount = (List<FEE_RAZORPAY_ACCOUNTS>)CMSHandler.FetchData<FEE_RAZORPAY_ACCOUNTS>(new FEE_RAZORPAY_ACCOUNTS() { RAZORPAY_ACCOUNT_TYPE_ID = Common.RazorpayAccountType.MarchantAccount }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayAccountInfo), sAcademicYear).DataSource.Data;
                if (FetchMarchantAccount != null & FetchMarchantAccount.Count > 0)
                {
                    key = FetchMarchantAccount.FirstOrDefault().KEY;
                    secret = FetchMarchantAccount.FirstOrDefault().SECRET_KEY;
                }
                var lipaymentlist = (List<FEE_RAZORPAY_PAYMENT_INFO>)CMSHandler.FetchData<FEE_RAZORPAY_PAYMENT_INFO>(null, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchPaymentids), sAcademicYear).DataSource.Data;
                if (lipaymentlist != null && lipaymentlist.Count > 0)
                {
                    foreach (var supitem in lipaymentlist)
                    {
                        RazorpayClient client = new RazorpayClient(key, secret);
                        Payment payments = client.Payment.Fetch(supitem.ID);
                        List<Transfer> transfer = payments.Transfers();
                        if (transfer != null && transfer.Count > 0)
                        {
                            string sSQL = @"INSERT INTO `FEE_RAZORPAY_TRANSFER_INFO_?AC_YEAR`
                                    (
                                    `ID`,
                                    `ENTITY`,
                                    `SOURCE`,
                                    `RECIPIENT`,
                                    `AMOUNT`,
                                    `CURRENCY`,
                                    `AMOUNT_REVERSED`,
                                    `FEES`,
                                    `TAX`,
                                    `ON_HOLD`,
                                    `ON_HOLD_UNTIL`,
                                    `RECIPIENT_SETTLEMENT_ID`,
                                    `CREATED_AT`,
                                    `UDF1`,
                                    `UDF2`,
                                    `UDF3`,
                                    `UDF4`,
                                    `UDF5`,
                                    `UDF6`)
                                    VALUES ";
                            foreach (var transferItem in transfer)
                            {
                                TransferItemResponse item = JsonConvert.DeserializeObject<TransferItemResponse>(transferItem.Attributes.ToString());
                                sSQL += @"('" + item.id + "','" + item.entity + "','" + item.source + "','" + item.recipient + "','" + item.amount + "','" + item.currency + "'" +
                                    ",'" + item.amount_reversed + "','" + (!string.IsNullOrEmpty(item.fees) ? item.fees : "0") + "','" + (!string.IsNullOrEmpty(item.tax) ? item.tax : "0") + "','" + item.on_hold + "','" + item.on_hold_until + "'" +
                                    ",'" + item.recipient_settlement_id + "','" + item.created_at + "','" + item.notes.udf1 + "','" + item.notes.udf2 + "','" + item.notes.udf3 + "'" +
                                    ",'" + item.notes.udf4 + "','" + item.notes.udf5 + "','" + item.notes.udf6 + "'),";
                            }
                            sSQL = sSQL.TrimEnd(',');
                            var result = CMSHandler.SaveOrUpdate(null, sSQL, sAcademicYear);
                        }

                    }

                }


            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "FeeRazorpayTransferAccount(string sRazorpayId, string sFeeRootId,string sAcademicYear)", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "FeeRazorpayTransferAccount(string sRazorpayId, string sFeeRootId,string sAcademicYear)", ex.Message);
                }

            }


            return Json("Done", JsonRequestBehavior.AllowGet);
        }
        //This method to enter fee transactions 
        private FEE_RAZORPAY_PAYMENT_INFO FeeRazorpayCollection(FEE_RAZORPAY_PAYMENT_INFO objPaymentInfo, string sRazorpayPaymentId, string sAcademicYear)
        {
            try
            {
                string sStudentId = objPaymentInfo.UDF1;
                string sFrequencyId = objPaymentInfo.UDF3;
                string sFrequencyType = objPaymentInfo.UDF2;
                string sAmount = (((!string.IsNullOrEmpty(objPaymentInfo.AMOUNT)) ? decimal.Parse(objPaymentInfo.AMOUNT) : 0) - ((!string.IsNullOrEmpty(objPaymentInfo.FEE)) ? decimal.Parse(objPaymentInfo.FEE) : 0)).ToString();
                string sSQL = string.Empty;
                sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFrequencyId).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, sFrequencyId);
                var FetchFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId }, sSQL, sAcademicYear).DataSource.Data;

                if (FetchFeeStudentAccount != null && FetchFeeStudentAccount.Count > 0)
                {
                    FetchFeeStudentAccount.RemoveAll(r => r.BALANCE == "0");
                    if (objPaymentInfo.CAPTURED.ToLower() == "true")
                    {
                        int ReceiptNo = 0;
                        string Prefix = "00";
                        string TransactionId = string.Empty;
                        FEE_STUDENT_ACCOUNT objStudentAccountModel = new FEE_STUDENT_ACCOUNT();
                        FEE_TRANSACTION objFeeTransactionModel = new FEE_TRANSACTION();
                        AUTO_GENERATE_NUMBERS objAutoGenerateNumber = new AUTO_GENERATE_NUMBERS();
                        if (!string.IsNullOrEmpty(sAcademicYear))
                        {
                            if (FetchFeeStudentAccount != null && FetchFeeStudentAccount.Count > 0)
                            {
                                decimal d = FetchFeeStudentAccount.Sum(x => decimal.Parse(x.BALANCE) * 100);
                                if (decimal.Parse(sAmount) == FetchFeeStudentAccount.Sum(x => decimal.Parse(x.BALANCE) * 100))
                                {
                                    var GroupFeeStudentAccount = FetchFeeStudentAccount.GroupBy(x => x.FREQUENCY_ID);
                                    foreach (var FrequencyItem in GroupFeeStudentAccount)
                                    {
                                        var c = GroupFeeStudentAccount.Count();
                                        sFrequencyId = FrequencyItem.FirstOrDefault().FREQUENCY_ID;
                                        var feeTransactions = FetchFeeStudentAccount.Where(x => x.FREQUENCY_ID == sFrequencyId);
                                        decimal balance = 0;
                                        balance = feeTransactions.Sum(x => decimal.Parse(x.BALANCE));
                                        if (balance > 0)
                                        {
                                            objFeeTransactionModel.STUDENT_ID = sStudentId;
                                            objFeeTransactionModel.CLASS = string.Empty;
                                            objFeeTransactionModel.FREQUENCY = sFrequencyId;
                                            objFeeTransactionModel.FREQUENCY_TO = sFrequencyId;
                                            objFeeTransactionModel.PAYMENT_DATE = DateTime.Today.ToString("yyyy-MM-dd");
                                            objFeeTransactionModel.USERNAME = sStudentId; // Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                                            objFeeTransactionModel.COLLECTED = feeTransactions.Sum(x => decimal.Parse(x.BALANCE)).ToString();
                                            var FetchReceiptNo = (List<AUTO_GENERATE_NUMBERS>)CMSHandler.FetchData<AUTO_GENERATE_NUMBERS>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAutoGenerateNumber)).DataSource.Data;
                                            if (FetchReceiptNo != null && FetchReceiptNo.Count > 0)
                                                ReceiptNo = Convert.ToInt32(FetchReceiptNo.FirstOrDefault().exam_receipt_no);
                                            else
                                            {
                                                // objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                                //   objResult.Message = Common.ErrorMessage.ExpectationFailed;
                                            }
                                            objAutoGenerateNumber.exam_receipt_no = Convert.ToString(ReceiptNo + 1);
                                            objAutoGenerateNumber.exam_receipt_no = Prefix + objAutoGenerateNumber.exam_receipt_no;
                                            var UpdateReceiptNo = CMSHandler.SaveOrUpdate(objAutoGenerateNumber, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateAutoGenerateNumber));
                                            if (UpdateReceiptNo.Success)
                                            {
                                                objFeeTransactionModel.PAYMENT_MODE = Common.PaymentMode.Online;
                                                objFeeTransactionModel.RECEIPT_NO = objAutoGenerateNumber.exam_receipt_no;
                                                objPaymentInfo.RECEIPT_NO = objAutoGenerateNumber.exam_receipt_no;
                                                objFeeTransactionModel.RAZORPAY_ID = sRazorpayPaymentId;
                                                var SaveFeeTransaction = CMSHandler.SaveOrUpdate(objFeeTransactionModel, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeTransaction), sAcademicYear, true);
                                                if (SaveFeeTransaction.Success)
                                                {
                                                    //Fee Paid Message
                                                    //var MessageSetting = (List<SMS_TEMPLATE_MODULE>)CMSHandler.FetchData<SMS_TEMPLATE_MODULE>(new SMS_TEMPLATE_MODULE {SMS_TEMPLATE_ID=Common.SMS_TEMPLATE.FeePaidSMS }, SQL.Communication.CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSMSTemplateActiveByTemplateId)).DataSource.Data;
                                                    //if (MessageSetting!=null && MessageSetting.Count>0)
                                                    //{                                                        
                                                    //    string sFeeMessage = string.Empty;
                                                    //    string sCount = string.Empty;
                                                    //    string sCreditCount = string.Empty;
                                                    //    var MessageContent = (List<SMS_USERDEFINED_TEMPLATE>)CMSHandler.FetchData<SMS_USERDEFINED_TEMPLATE>(new SMS_USERDEFINED_TEMPLATE() { TEMPLATE_ID = Common.SMS_TEMPLATE.DailyAbsenteesSMS }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchsmsTemplateByTemplateId), sAcademicYear).DataSource.Data;
                                                    //    var StudentInfo= (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(new StuPersonalInfo() { STUDENT_ID = sStudentId }, SQL.Communication.CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands .FetchStudentDetailsForSMSByStudentId), sAcademicYear).DataSource.Data;
                                                    //    sFeeMessage = MessageContent.FirstOrDefault().TEMPLATE_TEXT.Replace(Common.Delimiter.AT + Common.STU_PERSONAL_INFO.FIRST_NAME, StudentInfo.FirstOrDefault().FIRST_NAME).Replace(Common.Delimiter.AT + Common.CP_CLASSES_2017.CLASS_NAME, StudentInfo.FirstOrDefault().CLASS_NAME);
                                                    //    string sSmsMode = Common.MessageType.FEE;
                                                    //    using (CommunicationController objComcontroler = new CommunicationController())
                                                    //    {
                                                    //       sCount = objComcontroler.ValidateCreditCount((sFeeMessage.Length));
                                                    //    }
                                                    //    sCreditCount = sCount;                                                       
                                                    //    if (!string.IsNullOrEmpty(StudentInfo.FirstOrDefault().MOBILE))
                                                    //    {
                                                    //        using (CommunicationController objComcontroler = new CommunicationController())
                                                    //        {
                                                    //            objComcontroler.liSMS.Add(new MessageResult.sms() { to = StudentInfo.FirstOrDefault().MOBILE, message = sFeeMessage, custom = sStudentId, custom1 = StudentInfo.FirstOrDefault().CLASS_ID, CreditCount = sCount });
                                                    //            objComcontroler.SendMessage();
                                                    //        }

                                                    //    }

                                                    //}
                                                    //TransactionId = SaveFeeTransaction.RowUniqueId.ToString();
                                                    var FetchTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(new FEE_TRANSACTION() { RAZORPAY_ID = sRazorpayPaymentId, FREQUENCY = sFrequencyId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTransactionByRazorpayId), sAcademicYear).DataSource.Data;
                                                    TransactionId = (FetchTransaction != null && FetchTransaction.Count > 0) ? FetchTransaction.FirstOrDefault().TRANSACTION_ID : SaveFeeTransaction.RowUniqueId.ToString();
                                                    //   feePayUResponse.txnid = TransactionId;
                                                    decimal dAmount = feeTransactions.Sum(x => decimal.Parse(x.BALANCE));
                                                    decimal dCredit = 0;
                                                    foreach (var item in feeTransactions)
                                                    {
                                                        if (dAmount > 0 && Convert.ToDecimal(item.BALANCE) > 0)
                                                        {
                                                            dCredit = Convert.ToDecimal(item.BALANCE);
                                                            item.DEBIT = (dAmount >= dCredit) ? item.BALANCE : (dCredit - dAmount).ToString();
                                                            dAmount = dAmount - dCredit;
                                                        }
                                                        else
                                                        {
                                                            continue;
                                                        }
                                                        item.FREQUENCY = item.FREQUENCY_ID;
                                                        item.TRANSACTION_ID = TransactionId;
                                                        item.PAID_AMOUNT = item.DEBIT;
                                                        item.RECEIPT_NO = objFeeTransactionModel.RECEIPT_NO;
                                                        item.HEAD = item.HEAD_ID;


                                                        var SaveFeeCollection = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeCollection));
                                                        if (SaveFeeCollection.Success)
                                                        {
                                                            var SaveStudentAccount = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeStudentAccount), sAcademicYear);
                                                            if (SaveStudentAccount.Success)
                                                            {

                                                            }
                                                            else
                                                            {
                                                                //objResult.Message = Common.Messages.FailedToSaveRecords;
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
                                        else
                                        {

                                            objPaymentInfo.ERROR_CODE = "Pending";
                                            objPaymentInfo.ERROR_DESCRIPTION = "Please note Transaction Id and enquire with admin !!";
                                        }


                                    }
                                }
                                else
                                {
                                    objPaymentInfo.ERROR_CODE = "Pending";
                                    objPaymentInfo.ERROR_DESCRIPTION = "Please note Transaction Id and enquire with admin !!";
                                }
                            }
                            else
                            {
                                objPaymentInfo.ERROR_CODE = "Pending";
                                objPaymentInfo.ERROR_DESCRIPTION = "Please note Transaction Id and enquire with admin !!";
                            }
                        }
                        else
                        {
                            objPaymentInfo.ERROR_CODE = "Pending";
                            objPaymentInfo.ERROR_DESCRIPTION = "Please note Transaction Id and enquire with admin !!";
                        }

                    }
                    else
                    {
                        objPaymentInfo.ERROR_CODE = "Pending";
                        objPaymentInfo.ERROR_DESCRIPTION = "Please note Transaction Id and enquire with admin !!";
                    }

                }
                else
                {
                    objPaymentInfo.ERROR_CODE = "Pending";
                    objPaymentInfo.ERROR_DESCRIPTION = "Please note Transaction Id and enquire with admin !!";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "FeeRazorpayCollection(FEE_RAZORPAY_PAYMENT_INFO objPaymentInfo, string sRazorpayPaymentId, string sAcademicYear)", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "FeeRazorpayCollection(FEE_RAZORPAY_PAYMENT_INFO objPaymentInfo, string sRazorpayPaymentId, string sAcademicYear)", ex.Message);
                }
            }
            return objPaymentInfo;
        }
        #endregion
        // GET: Fee
        #region College FeeMainHead
        [UserRights("EXAMINCHARGE")]
        public ActionResult ListFeeMainHead()
        {
            FeeMainHeadViewModel MainHeadView = new FeeMainHeadViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    MainHeadView.FeeMainHeadList = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(new FeeMainHead() { FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeads), sAcademicYear).DataSource.Data;
                    MainHeadView.SupShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift), sAcademicYear).DataSource.Data;
                    MainHeadView.SupProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode), sAcademicYear).DataSource.Data;
                    MainHeadView.SupFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeMode), sAcademicYear).DataSource.Data;
                    MainHeadView.LiSupFeeRoot = (List<SUP_FEE_ROOT>)CMSHandler.FetchData<SUP_FEE_ROOT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FectchFeeRoot), sAcademicYear).DataSource.Data;
                    MainHeadView.LiSupApplictionType = (List<SUP_APPLICTION_TYPE>)CMSHandler.FetchData<SUP_APPLICTION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType), sAcademicYear).DataSource.Data;
                    MainHeadView.SubHeads = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupHeadList)).DataSource.Data;
                    MainHeadView.MainHeads = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeMainHead)).DataSource.Data;
                    MainHeadView.LiSupBankAccount = (List<SUP_BANK_ACCOUNT>)CMSHandler.FetchData<SUP_BANK_ACCOUNT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupBankAccounts)).DataSource.Data;
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "ListFeeMainHead", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "ListFeeMainHead", Ex.Message);
                }
            }
            return View(MainHeadView);
        }
        public ActionResult ListHostelFeeMainHead()
        {
            FeeMainHeadViewModel MainHeadView = new FeeMainHeadViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    MainHeadView.FeeMainHeadList = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(new FeeMainHead() { FEE_ROOT_ID = Common.FeeRoot.HostelFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeads), sAcademicYear).DataSource.Data;
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "ListFeeMainHead", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "ListFeeMainHead", Ex.Message);
                }
            }
            return View(MainHeadView);
        }
        public ActionResult ListMessFeeMainHead()
        {
            FeeMainHeadViewModel MainHeadView = new FeeMainHeadViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    MainHeadView.FeeMainHeadList = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(new FeeMainHead() { FEE_ROOT_ID = Common.FeeRoot.MessFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeads), sAcademicYear).DataSource.Data;
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "ListFeeMainHead", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "ListFeeMainHead", Ex.Message);
                }
            }
            return View(MainHeadView);
        }
        public ActionResult CollegeFeeMainHead()
        {
            FeeMainHeadViewModel MainHeadView = new FeeMainHeadViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    MainHeadView.FeeMainHeadList = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(new FeeMainHead() { FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeads), sAcademicYear).DataSource.Data;
                    MainHeadView.SupShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift), sAcademicYear).DataSource.Data;
                    MainHeadView.SupProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode), sAcademicYear).DataSource.Data;
                    MainHeadView.SupFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeMode), sAcademicYear).DataSource.Data;
                    MainHeadView.LiSupFeeRoot = (List<SUP_FEE_ROOT>)CMSHandler.FetchData<SUP_FEE_ROOT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FectchFeeRoot), sAcademicYear).DataSource.Data;
                    MainHeadView.LiSupApplictionType = (List<SUP_APPLICTION_TYPE>)CMSHandler.FetchData<SUP_APPLICTION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType), sAcademicYear).DataSource.Data;
                    MainHeadView.SubHeads = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupHeadList)).DataSource.Data;
                    MainHeadView.MainHeads = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeMainHead)).DataSource.Data;
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "CollegeFeeMainHead", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "CollegeFeeMainHead", Ex.Message);
                }
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(MainHeadView);
        }
        public ActionResult BindCollegeFeeMainHead(string sFeeRoot, string sApplicationType, string sShift, string sProgrammeMode, string sFeeMode, string sMainHead, string sSubHeads)
        {
            FeeMainHeadViewModel MainHeadView = new FeeMainHeadViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            string sSql = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                FeeMainHead objFeeMainHead = new FeeMainHead();
                try
                {
                    objFeeMainHead.FEE_ROOT_ID = sFeeRoot;
                    objFeeMainHead.SHIFT = sShift;
                    objFeeMainHead.PROGRAMME_MODE = sProgrammeMode;
                    objFeeMainHead.FREQUENCY_ID = sFeeMode;
                    objFeeMainHead.APPLICATION_TYPE_ID = sApplicationType;
                    objFeeMainHead.MAIN_HEAD_ID = sMainHead;
                    sSql = FeeSQL.GetFeeSQL(FeeSQLCommands.BindFeeMainHeadBySubHead).Replace(Common.Delimiter.QUS + Common.SUP_HEAD.HEAD_ID, sSubHeads);
                    MainHeadView.LiSupBankAccount = (List<SUP_BANK_ACCOUNT>)CMSHandler.FetchData<SUP_BANK_ACCOUNT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupBankAccounts)).DataSource.Data;
                    MainHeadView.FeeCategory = (List<Sup_Fee_Category>)CMSHandler.FetchData<Sup_Fee_Category>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeeCategory)).DataSource.Data;
                    MainHeadView.FeeMainHeadList = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(objFeeMainHead, sSql, sAcademicYear).DataSource.Data;
                }
                catch (Exception Ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "BindCollegeFeeMainHead", "Err MSg: " + Ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "BindCollegeFeeMainHead", Ex.Message);
                    }
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");

            return View(MainHeadView);
        }
        //This method Removes the Mapped SubHead using this Conditions
        public JsonResult LoadSubheads(string sMainHead, string sShiftId, string sProgrammeModeId, string sFrequencyId, string sFeeRootId, string sApplicationTypeId)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    FeeMainHead objFeeMainHead = new FeeMainHead();
                    objFeeMainHead.MAIN_HEAD_ID = sMainHead;
                    objFeeMainHead.SHIFT = sShiftId;
                    objFeeMainHead.PROGRAMME_MODE = sProgrammeModeId;
                    objFeeMainHead.FREQUENCY_ID = sFrequencyId;
                    objFeeMainHead.FEE_ROOT_ID = sFeeRootId;
                    objFeeMainHead.APPLICATION_TYPE_ID = sApplicationTypeId;
                    var subheads = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(objFeeMainHead, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchSubHeadsByMainHeadId), sAcademicYear).DataSource.Data;
                    var MappedHeads = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(objFeeMainHead, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMappedSupHead), sAcademicYear).DataSource.Data;
                    if (MappedHeads != null)
                    {
                        foreach (var Mapitem in MappedHeads)
                        {
                            subheads.RemoveAll(x => x.HEAD_ID == Mapitem.HEAD_ID);
                        }
                    }
                    if (subheads != null)
                    {
                        foreach (var item in subheads)
                        {
                            objResult.sResult += "<option value='" + item.HEAD_ID + "' " + item.STATUS + ">" + item.HEAD + "</option>";
                        }

                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "LoadSubheads", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "LoadSubheads", Ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOrUpdateFeeMainHead(string sJsonFeeMainHeads)
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    FeeMainHead objModel = new FeeMainHead();
                    var FetchFeeMainHeadById = new List<FeeMainHead>();
                    var TempList = new List<FeeMainHead>();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var obj = serializer.Deserialize<FEE_STRUCTURE_LIST>(sJsonFeeMainHeads);
                    foreach (var item in obj.JSON_FEE_MAIN_HEAD)
                    {
                        objModel.MAIN_HEAD_ID = item.MAIN_HEAD_ID;
                        objModel.SHIFT = item.SHIFT;
                        objModel.PROGRAMME_MODE = item.PROGRAMME_MODE;
                        objModel.FREQUENCY_ID = item.FREQUENCY_ID;
                        objModel.APPLICATION_TYPE_ID = item.APPLICATION_TYPE_ID;
                        objModel.FEE_ROOT_ID = item.FEE_ROOT_ID;
                    }
                    FetchFeeMainHeadById = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeadsByMainHeadId), sAcademicYear).DataSource.Data;
                    //this loop removes existing main heads 
                    if (FetchFeeMainHeadById != null && FetchFeeMainHeadById.Count() > 0)
                    {
                        if (obj.JSON_FEE_MAIN_HEAD != null && obj.JSON_FEE_MAIN_HEAD.Count() > 0)
                        {
                            foreach (var items in obj.JSON_FEE_MAIN_HEAD)
                            {
                                if (!string.IsNullOrEmpty(items.FEE_MAIN_HEAD_ID))
                                {
                                    items.ACADEMIC_YEAR = sAcademicYear;
                                    var SaveMainHead = CMSHandler.SaveOrUpdate(items, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateFeeMainHead));
                                    if (SaveMainHead.Success)
                                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                    else
                                        objResult.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                            foreach (var list in FetchFeeMainHeadById)
                            {
                                TempList.Add(obj.JSON_FEE_MAIN_HEAD.SingleOrDefault(S => S.MAIN_HEAD_ID == list.MAIN_HEAD_ID && S.HEAD_ID == list.HEAD_ID));
                                obj.JSON_FEE_MAIN_HEAD.Remove(obj.JSON_FEE_MAIN_HEAD.SingleOrDefault(S => S.MAIN_HEAD_ID == list.MAIN_HEAD_ID && S.HEAD_ID == list.HEAD_ID));
                            }
                            foreach (var item in TempList)
                            {
                                if (item != null)
                                    FetchFeeMainHeadById.Remove(FetchFeeMainHeadById.SingleOrDefault(S => S.MAIN_HEAD_ID == item.MAIN_HEAD_ID && S.HEAD_ID == item.HEAD_ID));
                            }
                            //this loop deletes records
                            foreach (var item in FetchFeeMainHeadById)
                            {
                                //Delete Records
                                item.ACADEMIC_YEAR = sAcademicYear;
                                var DeleteRecord = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(item, FeeSQL.GetFeeSQL(FeeSQLCommands.CheckIsSubHeadMapped), sAcademicYear).DataSource.Data;
                                if (DeleteRecord.FirstOrDefault().STATUS != "1")
                                {
                                    var o = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeMainHeadByMainHeadIdAndHeadId));
                                    if (o.Success)
                                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    objResult.Message = Common.Messages.LedgersAreMapped;
                                }

                            }
                        }
                    }
                    //this loop saves records
                    foreach (var item in obj.JSON_FEE_MAIN_HEAD)
                    {
                        item.ACADEMIC_YEAR = sAcademicYear;
                        var SaveMainHead = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeMainHead));
                        if (SaveMainHead.Success)
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "SaveFeeMainHead", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "SaveFeeMainHead", Ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadFeeMainHead()
        {
            FeeMainHeadViewModel MainHeadView = new FeeMainHeadViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    MainHeadView.FeeMainHeadList = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeads), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "LoadFeeMainHead", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "LoadFeeMainHead", Ex.Message);
                    return View(MainHeadView);
                }
            }
            return View(MainHeadView);
        }
        public JsonResult DeleteFeeMainHead(string sFeeMainHead)
        {
            JsonResultData objResult = new JsonResultData();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (sFeeMainHead != null)
                    {
                        FeeMainHead objModel = new FeeMainHead();
                        objModel.FEE_MAIN_HEAD_ID = sFeeMainHead;
                        var MainHead = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeemainHeadbyId), sAcademicYear).DataSource.Data;
                        if (MainHead != null && MainHead.Count > 0)
                        {
                            objModel.FREQUENCY_ID = MainHead.FirstOrDefault().FREQUENCY_ID;
                            objModel.HEAD_ID = MainHead.FirstOrDefault().HEAD_ID;
                            var DeleteRecord = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.CheckIsSubHeadMapped), sAcademicYear).DataSource.Data;
                            if (DeleteRecord != null && DeleteRecord.Count > 0)
                            {
                                if (DeleteRecord.FirstOrDefault().STATUS != "1")
                                {
                                    var o = CMSHandler.SaveOrUpdate(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeMainHead));
                                    if (o.Success)
                                        objResult.Message = Common.Messages.RecordDeletedSuccessfully;
                                }
                                else
                                {
                                    objResult.Message = Common.Messages.LedgersAreMapped;
                                }
                            }
                            else
                            {
                                objResult.Message = Common.Messages.FailedToDeletedTryAgain;
                            }
                        }
                        else
                        {
                            objResult.Message = Common.Messages.FailedToDeletedTryAgain;
                        }

                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "DeleteFeeMainHead", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "DeleteFeeMainHead", Ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Hostel&MessFeeMainHead
        public ActionResult HostelFeeMainHead()
        {
            FeeMainHeadViewModel MainHeadView = new FeeMainHeadViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    // MainHeadView.FeeMainHeadList = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeads), sAcademicYear).DataSource.Data;
                    // MainHeadView.SupShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift), sAcademicYear).DataSource.Data;
                    //MainHeadView.SupProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode), sAcademicYear).DataSource.Data;

                    MainHeadView.SupFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(new SUP_FEE_FREQUENCY() { FEE_ROOT_ID = Common.FeeRoot.HostelFee }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeModeAndFeeRootId), sAcademicYear).DataSource.Data;
                    MainHeadView.LiSupFeeRoot = (List<SUP_FEE_ROOT>)CMSHandler.FetchData<SUP_FEE_ROOT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FectchFeeRoot), sAcademicYear).DataSource.Data;
                    //MainHeadView.LiSupApplictionType = (List<SUP_APPLICTION_TYPE>)CMSHandler.FetchData<SUP_APPLICTION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType), sAcademicYear).DataSource.Data;
                    MainHeadView.SubHeads = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupHeadList)).DataSource.Data;
                    MainHeadView.MainHeads = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeMainHead)).DataSource.Data;
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "CollegeFeeMainHead", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "CollegeFeeMainHead", Ex.Message);
                }
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(MainHeadView);
        }
        public ActionResult BindHostelFeeMainHead(string sFeeRoot, string sFeeMode, string sMainHead, string sSubHeads)
        {
            FeeMainHeadViewModel MainHeadView = new FeeMainHeadViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            string sSql = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                FeeMainHead objFeeMainHead = new FeeMainHead();
                try
                {
                    objFeeMainHead.FEE_ROOT_ID = sFeeRoot;
                    objFeeMainHead.FREQUENCY_ID = sFeeMode;
                    objFeeMainHead.MAIN_HEAD_ID = sMainHead;
                    sSql = FeeSQL.GetFeeSQL(FeeSQLCommands.BindFeeMainHeadBySubHeadForHostel).Replace(Common.Delimiter.QUS + Common.SUP_HEAD.HEAD_ID, sSubHeads);
                    MainHeadView.LiSupBankAccount = (List<SUP_BANK_ACCOUNT>)CMSHandler.FetchData<SUP_BANK_ACCOUNT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupBankAccounts)).DataSource.Data;
                    MainHeadView.FeeCategory = (List<Sup_Fee_Category>)CMSHandler.FetchData<Sup_Fee_Category>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeeCategory)).DataSource.Data;
                    MainHeadView.FeeMainHeadList = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(objFeeMainHead, sSql, sAcademicYear).DataSource.Data;
                }
                catch (Exception Ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "BindCollegeFeeMainHead", "Err MSg: " + Ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "BindCollegeFeeMainHead", Ex.Message);
                    }
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");

            return View(MainHeadView);
        }
        public JsonResult LoadSubheadsForHostel(string sMainHead, string sFrequencyId, string sFeeRootId)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    FeeMainHead objFeeMainHead = new FeeMainHead();
                    objFeeMainHead.MAIN_HEAD_ID = sMainHead;
                    objFeeMainHead.FREQUENCY_ID = sFrequencyId;
                    objFeeMainHead.FEE_ROOT_ID = sFeeRootId;
                    var subheads = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(objFeeMainHead, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchSubHeadsByMainHeadIdForHostel), sAcademicYear).DataSource.Data;
                    var MappedHeads = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(objFeeMainHead, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMappedSupHeadForHostel), sAcademicYear).DataSource.Data;
                    if (MappedHeads != null)
                    {
                        foreach (var Mapitem in MappedHeads)
                        {
                            subheads.RemoveAll(x => x.HEAD_ID == Mapitem.HEAD_ID);
                        }
                    }
                    if (subheads != null)
                    {
                        foreach (var item in subheads)
                        {
                            objResult.sResult += "<option value='" + item.HEAD_ID + "' " + item.STATUS + ">" + item.HEAD + "</option>";
                        }

                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "LoadSubheads", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "LoadSubheads", Ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOrUpdateFeeMainHeadHostel(string sJsonFeeMainHeads)
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    FeeMainHead objModel = new FeeMainHead();
                    var FetchFeeMainHeadById = new List<FeeMainHead>();
                    var TempList = new List<FeeMainHead>();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var obj = serializer.Deserialize<FEE_STRUCTURE_LIST>(sJsonFeeMainHeads);
                    foreach (var item in obj.JSON_FEE_MAIN_HEAD)
                    {
                        objModel.MAIN_HEAD_ID = item.MAIN_HEAD_ID;
                        objModel.FREQUENCY_ID = item.FREQUENCY_ID;
                        objModel.FEE_ROOT_ID = item.FEE_ROOT_ID;
                    }
                    FetchFeeMainHeadById = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeadsByMainHeadIdForHostel), sAcademicYear).DataSource.Data;
                    //this loop removes existing main heads 
                    if (FetchFeeMainHeadById != null && FetchFeeMainHeadById.Count() > 0)
                    {
                        if (obj.JSON_FEE_MAIN_HEAD != null && obj.JSON_FEE_MAIN_HEAD.Count() > 0)
                        {
                            foreach (var items in obj.JSON_FEE_MAIN_HEAD)
                            {
                                if (!string.IsNullOrEmpty(items.FEE_MAIN_HEAD_ID))
                                {
                                    items.ACADEMIC_YEAR = sAcademicYear;
                                    var SaveMainHead = CMSHandler.SaveOrUpdate(items, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateFeeMainHead));
                                    if (SaveMainHead.Success)
                                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                    else
                                        objResult.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                            foreach (var list in FetchFeeMainHeadById)
                            {
                                TempList.Add(obj.JSON_FEE_MAIN_HEAD.SingleOrDefault(S => S.MAIN_HEAD_ID == list.MAIN_HEAD_ID && S.HEAD_ID == list.HEAD_ID));
                                obj.JSON_FEE_MAIN_HEAD.Remove(obj.JSON_FEE_MAIN_HEAD.SingleOrDefault(S => S.MAIN_HEAD_ID == list.MAIN_HEAD_ID && S.HEAD_ID == list.HEAD_ID));
                            }
                            foreach (var item in TempList)
                            {
                                if (item != null)
                                    FetchFeeMainHeadById.Remove(FetchFeeMainHeadById.SingleOrDefault(S => S.MAIN_HEAD_ID == item.MAIN_HEAD_ID && S.HEAD_ID == item.HEAD_ID));
                            }
                            //this loop deletes records
                            foreach (var item in FetchFeeMainHeadById)
                            {
                                //Delete Records
                                item.ACADEMIC_YEAR = sAcademicYear;
                                var DeleteRecord = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(item, FeeSQL.GetFeeSQL(FeeSQLCommands.CheckIsSubHeadMapped), sAcademicYear).DataSource.Data;
                                if (DeleteRecord.FirstOrDefault().STATUS != "1")
                                {
                                    var o = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeMainHeadByMainHeadIdAndHeadId));
                                    if (o.Success)
                                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    objResult.Message = Common.Messages.LedgersAreMapped;
                                }

                            }
                        }
                    }
                    //this loop saves records
                    foreach (var item in obj.JSON_FEE_MAIN_HEAD)
                    {
                        item.ACADEMIC_YEAR = sAcademicYear;
                        var SaveMainHead = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeMainHeadForHostel));
                        if (SaveMainHead.Success)
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "SaveFeeMainHead", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "SaveFeeMainHead", Ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MessFeeMainHead()
        {
            FeeMainHeadViewModel MainHeadView = new FeeMainHeadViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    // MainHeadView.FeeMainHeadList = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeads), sAcademicYear).DataSource.Data;
                    // MainHeadView.SupShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift), sAcademicYear).DataSource.Data;
                    //MainHeadView.SupProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode), sAcademicYear).DataSource.Data;
                    MainHeadView.SupFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(new SUP_FEE_FREQUENCY() { FEE_ROOT_ID = Common.FeeRoot.MessFee }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeModeAndFeeRootId), sAcademicYear).DataSource.Data;
                    MainHeadView.LiSupFeeRoot = (List<SUP_FEE_ROOT>)CMSHandler.FetchData<SUP_FEE_ROOT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FectchFeeRoot), sAcademicYear).DataSource.Data;
                    //MainHeadView.LiSupApplictionType = (List<SUP_APPLICTION_TYPE>)CMSHandler.FetchData<SUP_APPLICTION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType), sAcademicYear).DataSource.Data;
                    MainHeadView.SubHeads = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupHeadList)).DataSource.Data;
                    MainHeadView.MainHeads = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeMainHead)).DataSource.Data;
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "CollegeFeeMainHead", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "CollegeFeeMainHead", Ex.Message);
                }
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(MainHeadView);
        }
        #endregion

        #region Student Transaction And Account
        [UserRights("EXAMINCHARGE")]
        public ActionResult ListStudentAccount()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            var FetchFrequency = (List<sup_frequency_type>)CMSHandler.FetchData<sup_frequency_type>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFrequencytype), sAcademicYear).DataSource.Data;
            // var FetchStudent = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllStudent)).DataSource.Data;
            var classes = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchclass), sAcademicYear).DataSource.Data;
            if (FetchFrequency != null && FetchFrequency.Count > 0)
                objViewModel.liSupFeeFrequency = new SelectList(FetchFrequency, Common.SUP_FREQUENCY_TYPE.FREQUENCY_TYPE_ID, Common.SUP_FREQUENCY_TYPE.FREQUENCY_TYPE);
            //if (FetchStudent != null && FetchStudent.Count > 0)
            //  objViewModel.liStudentPersonalInfo = new SelectList(FetchStudent, Common.STU_PERSONAL_INFO.STUDENT_ID, Common.STU_PERSONAL_INFO.FIRST_NAME);
            if (classes != null && classes.Count > 0)
                objViewModel.liClassList = new SelectList(classes, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
            return View(objViewModel);
        }

        public JsonResult BindStudentListByClassId(string sClassId)
        {
            JsonResultData objResult = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sOption = string.Empty;
                var studentList = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(new cp_Classes_2017() { CLASS_ID = sClassId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByClassID), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                if (studentList != null && studentList.Count > 0)
                {
                    sOption = "<option value='0'>--Select--</option>";
                    foreach (var item in studentList)
                    {
                        sOption += "<option value='" + item.STUDENT_ID + "'>" + item.FIRST_NAME + "</option>";
                    }
                    objResult.sResult = sOption;
                }
            }
            else
            {
                objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }

            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindStudnetAccountDetails(string sFrequencyId, string sStudentId)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT();
            objFeeStudentAccountModel.FREQUENCY_ID = sFrequencyId;
            objFeeStudentAccountModel.STUDENT_ID = sStudentId;
            var FetchStudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(objFeeStudentAccountModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByStudentId), sAcademicYear).DataSource.Data;
            var FetchCreditAndBalance = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objFeeStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchCreditByStudentId), sAcademicYear).DataSource.Data;
            var FetchFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objFeeStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFrequencyId), sAcademicYear).DataSource.Data;
            if (FetchFeeStudentAccount != null && FetchFeeStudentAccount.Count > 0)
                objViewModel.liFeeStudentAccount = FetchFeeStudentAccount;
            if (FetchCreditAndBalance != null && FetchCreditAndBalance.Count > 0)
                objViewModel.liCreditAndDebit = FetchCreditAndBalance;
            if (FetchStudentInfo != null && FetchStudentInfo.Count > 0)
                objViewModel.lstStudentPersonalInfo = FetchStudentInfo;
            return View(objViewModel);
        }
        public JsonResult SaveStudentAccount(string sStuTransaction, string sFrequencyId, string sStudentId, string sTotal)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                int ReceiptNo = 0;
                string Prefix = "00";
                string TransactionId = string.Empty;
                FEE_STUDENT_ACCOUNT objStudentAccountModel = new FEE_STUDENT_ACCOUNT();
                FEE_TRANSACTION objFeeTransactionModel = new FEE_TRANSACTION();
                AUTO_GENERATE_NUMBERS objAutoGenerateNumber = new AUTO_GENERATE_NUMBERS();
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    JsonFeeStudentAccountList objJson = JsonConvert.DeserializeObject<JsonFeeStudentAccountList>(sStuTransaction);
                    objStudentAccountModel.STUDENT_ID = sStudentId;
                    var FetchStudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(objStudentAccountModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByStudentId), sAcademicYear).DataSource.Data;
                    if (FetchStudentInfo != null && FetchStudentInfo.Count > 0)
                    {
                        objFeeTransactionModel.STUDENT_ID = FetchStudentInfo.FirstOrDefault().STUDENT_ID;
                        objFeeTransactionModel.CLASS = FetchStudentInfo.FirstOrDefault().CLASS_ID;
                        objFeeTransactionModel.FREQUENCY = sFrequencyId;
                        objFeeTransactionModel.FREQUENCY_TO = sFrequencyId;
                        objFeeTransactionModel.PAYMENT_DATE = DateTime.Today.ToString("yyyy-MM-dd");
                        objFeeTransactionModel.USERNAME = Session[Common.SESSION_VARIABLES.USER_CODE].ToString();
                        objFeeTransactionModel.COLLECTED = sTotal;
                        var FetchReceiptNo = (List<AUTO_GENERATE_NUMBERS>)CMSHandler.FetchData<AUTO_GENERATE_NUMBERS>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAutoGenerateNumber)).DataSource.Data;
                        if (FetchReceiptNo != null && FetchReceiptNo.Count > 0)
                            foreach (var item in FetchReceiptNo)
                            {
                                ReceiptNo = Convert.ToInt32(item.exam_receipt_no);
                            }
                        else
                        {
                            objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                            objResult.Message = Common.ErrorMessage.ExpectationFailed;
                        }
                        objAutoGenerateNumber.exam_receipt_no = Convert.ToString(ReceiptNo + 1);
                        objAutoGenerateNumber.exam_receipt_no = Prefix + objAutoGenerateNumber.exam_receipt_no;
                        var UpdateReceiptNo = CMSHandler.SaveOrUpdate(objAutoGenerateNumber, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateAutoGenerateNumber));
                        if (UpdateReceiptNo.Success)
                        {
                            objFeeTransactionModel.PAYMENT_MODE = Common.PaymentMode.Cash;
                            objFeeTransactionModel.RECEIPT_NO = objAutoGenerateNumber.exam_receipt_no;
                            var SaveFeeTransaction = CMSHandler.SaveOrUpdate(objFeeTransactionModel, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeTransaction), sAcademicYear, true);
                            if (SaveFeeTransaction.Success)
                            {
                                TransactionId = SaveFeeTransaction.RowUniqueId.ToString();
                                objResult.sResult = TransactionId;
                                foreach (var item in objJson.StudentAccountList)
                                {
                                    item.FREQUENCY = item.FREQUENCY_ID;
                                    item.TRANSACTION_ID = TransactionId;
                                    item.PAID_AMOUNT = item.DEBIT;
                                    item.RECEIPT_NO = objFeeTransactionModel.RECEIPT_NO;
                                    var SaveFeeCollection = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeCollection));
                                    if (SaveFeeCollection.Success)
                                    {
                                        var SaveStudentAccount = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeStudentAccount), sAcademicYear);
                                        if (SaveStudentAccount.Success)
                                        {
                                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                        }
                                        else
                                        {
                                            objResult.Message = Common.Messages.FailedToSaveRecords;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                objResult.Message = Common.ErrorMessage.ExpectationFailed;
                            }
                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                        objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    }

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "ListStudentAccount", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "ListStudentAccount", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GenerateFeeReceipt(string sFrequencyId, string sStudentId, string sTransactionId)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT();
                    objFeeStudentAccountModel.FREQUENCY_ID = sFrequencyId;
                    objFeeStudentAccountModel.STUDENT_ID = sStudentId;
                    objFeeStudentAccountModel.TRANSACTION_ID = sTransactionId;
                    var FetchCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    var FetchStudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(objFeeStudentAccountModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByStudentId), sAcademicYear).DataSource.Data;
                    var FetchTotalBalance = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTotalDebitByTransactionId)).DataSource.Data;
                    if (FetchTotalBalance != null && FetchTotalBalance.Count > 0)
                        Number = Convert.ToInt32(FetchTotalBalance.FirstOrDefault().COLLECTED);
                    var FetchFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objFeeStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByTransactionId), sAcademicYear).DataSource.Data;
                    if (FetchCollegeDetails != null && FetchCollegeDetails.Count > 0)
                        objViewModel.liCollegeDetails = FetchCollegeDetails;
                    if (FetchStudentInfo != null && FetchStudentInfo.Count > 0)
                        objViewModel.lstStudentPersonalInfo = FetchStudentInfo;
                    if (FetchTotalBalance != null && FetchTotalBalance.Count > 0)
                        objViewModel.liFeeTransaction = FetchTotalBalance;
                    if (FetchFeeStudentAccount != null && FetchFeeStudentAccount.Count > 0)
                        objViewModel.liFeeStudentAccount = FetchFeeStudentAccount;
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "GenerateFeeReceipt", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "GenerateFeeReceipt", Ex.Message);
                }
            }
            return View(objViewModel);
        }
        #endregion

        #region Class Wise Paid Status
        // [UserRights("EXAMINCHARGE")]
        public ActionResult ListClassWiseStudentPaidStatus()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var FetchClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    var FetchFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(new SUP_FEE_FREQUENCY() { FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeModeAndFeeRootId), sAcademicYear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                        objViewModel.liSupFeeFrequency = new SelectList(FetchFrequency, Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, Common.SUP_FEE_FREQUENCY.FREQUENCY_NAME);
                    if (FetchClass != null && FetchClass.Count > 0)
                        objViewModel.liClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");

            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "ListStudentAccount", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "ListStudentAccount", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindClassWiseFeeDetails(string sFrequencyId, string sClassId)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var objFeeStudentAccountModel = new FEE_TRANSACTION();
                objFeeStudentAccountModel.FREQUENCY = sFrequencyId;
                objFeeStudentAccountModel.CLASS = sClassId;
                var FetchCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                var FetchClassWiseFeeDetails = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchClassWiseFeeDetails), sAcademicYear).DataSource.Data;
                if (FetchClassWiseFeeDetails != null && FetchClassWiseFeeDetails.Count > 0)
                    objViewModel.liFeeTransaction = FetchClassWiseFeeDetails;
                if (FetchClassWiseFeeDetails != null && FetchClassWiseFeeDetails.Count > 0)
                    objViewModel.liCollegeDetails = FetchCollegeDetails;
            }
            else
            {

            }
            return View(objViewModel);
        }
        public ActionResult StudentPaidStatus()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var FetchFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(new SUP_FEE_FREQUENCY() { FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeModeAndFeeRootId), sAcademicYear).DataSource.Data;
                    // var FetchFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFrequencytype), sAcademicYear).DataSource.Data;
                    var FetchClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    var SupShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift), sAcademicYear).DataSource.Data;
                    var SupProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode), sAcademicYear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                        objViewModel.liFrequencyType = new SelectList(FetchFrequency, Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, Common.SUP_FEE_FREQUENCY.FREQUENCY_NAME);
                    if (SupShift != null && SupShift.Count > 0)
                        objViewModel.liShift = new SelectList(SupShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    if (SupProgrammeMode != null && SupProgrammeMode.Count > 0)
                        objViewModel.liProgrammeModeList = new SelectList(SupProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");

            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "ListStudentAccount", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "ListStudentAccount", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindStudentPaidStatus(string sFrequencyId, string sClassId)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            JsonResultData objResult = new JsonResultData();
            string sSql = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var objFeeStudentAccountModel = new FEE_TRANSACTION();
                    objFeeStudentAccountModel.FREQUENCY = sFrequencyId;
                    var FetchCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    sSql = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchClassWiseFeeDetails).Replace(Common.Delimiter.QUS + "CLASS", sClassId);
                    var FetchClassWiseFeeDetails = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, sSql, sAcademicYear).DataSource.Data;
                    if (FetchClassWiseFeeDetails != null && FetchClassWiseFeeDetails.Count > 0)
                        objViewModel.liFeeTransaction = FetchClassWiseFeeDetails;
                    if (FetchClassWiseFeeDetails != null && FetchClassWiseFeeDetails.Count > 0)
                        objViewModel.liCollegeDetails = FetchCollegeDetails;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");

            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "ListStudentAccount", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "ListStudentAccount", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public JsonResult FetchClassByProgrammeGroupId(string sProgrammeGroupIds)
        {
            JsonResultData objResult = new JsonResultData();
            cp_Classes_2017 objModel = new cp_Classes_2017();
            string sSql = string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                objModel.PROGRAMME_GROUP_ID = sProgrammeGroupIds;
                sSql = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByProgrammeGroupId).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.PROGRAMME_GROUP_ID, sProgrammeGroupIds);
                var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(objModel, sSql, sAcademicyear).DataSource.Data;
                if (FetchClass != null && FetchClass.Count > 0)
                {
                    foreach (var item in FetchClass)
                        objResult.sResult += "<option value='" + item.CLASS_ID + "'>" + item.CLASS_NAME + "</option>";
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                objResult.Message = Common.ErrorMessage.RequestTimeout;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindStudentAmountByRazorAccount(string sClassYearId, string sProgrammeGroupId, string sFrequencyId, string sRazorPaymentId)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            JsonResultData objResult = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var objFeeStudentAccountModel = new FEE_TRANSACTION();
                    objFeeStudentAccountModel.FREQUENCY = sFrequencyId;
                    objFeeStudentAccountModel.CLASS_YEAR = sClassYearId;
                    objFeeStudentAccountModel.PROGRAMME_GROUP_ID = sProgrammeGroupId;
                    objFeeStudentAccountModel.RAZOR_PAY_ID = sRazorPaymentId;
                    objViewModel.liFeeReport = (List<FEE_REPORT>)CMSHandler.FetchData<FEE_REPORT>(objFeeStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAmountByPaymentId), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");

            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "ListStudentAccount", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "ListStudentAccount", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        #endregion

        #region Monthly Status For Student Exam Fee Payment
        [UserRights("EXAMINCHARGE")]
        public ActionResult ListMonthlyStatusForExamFeePayment()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var FetchFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllFrequency), sAcademicYear).DataSource.Data;
                    var FetchProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                        objViewModel.liSupFeeFrequency = new SelectList(FetchFrequency, Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, Common.SUP_FEE_FREQUENCY.FREQUENCY_NAME);
                    if (FetchProgrammeMode != null && FetchProgrammeMode.Count > 0)
                        objViewModel.liProgrammeModeList = new SelectList(FetchProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "ListStudentAccount", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "ListStudentAccount", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindMonthWiseFeeDetails(string sFrequencyId, string sProgrammeMode, string sDateFrom, string sDateTo)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            var objFeeStudentAccountModel = new FEE_TRANSACTION();
            objFeeStudentAccountModel.FREQUENCY = sFrequencyId;
            objFeeStudentAccountModel.PROGRAMME_MODE = sProgrammeMode;
            objFeeStudentAccountModel.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(sDateFrom);
            objFeeStudentAccountModel.DATE_TO = CommonMethods.ConvertdatetoyearFromat(sDateTo);
            var FetchCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
            var FetchClassWiseFeeDetails = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMonthlyStudentFeeDetails), sAcademicYear).DataSource.Data;
            var FetchMonth = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentFeeDetailsOnlyMonth), sAcademicYear).DataSource.Data;
            if (FetchClassWiseFeeDetails != null && FetchClassWiseFeeDetails.Count > 0)
                objViewModel.liFeeTransaction = FetchClassWiseFeeDetails;
            if (FetchCollegeDetails != null && FetchCollegeDetails.Count > 0)
                objViewModel.liCollegeDetails = FetchCollegeDetails;
            if (FetchMonth != null && FetchMonth.Count > 0)
                objViewModel.liMonth = FetchMonth;
            return View(objViewModel);
        }

        public ActionResult FeeSummary(string sFrequencyId)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                if (!string.IsNullOrEmpty(sFrequencyId))
                {
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string sStudentId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT();
                    objFeeStudentAccountModel.FREQUENCY_ID = sFrequencyId;
                    objFeeStudentAccountModel.STUDENT_ID = sStudentId;
                    var FetchStudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(objFeeStudentAccountModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByStudentId), sAcademicYear).DataSource.Data;
                    var FetchTotalBalance = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objFeeStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeSummaryByStudentId), sAcademicYear).DataSource.Data;
                    Number = Convert.ToInt32(FetchTotalBalance.FirstOrDefault().DEBIT);
                    // var FetchFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objFeeStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByTransactionId), sAcademicYear).DataSource.Data;
                    objViewModel.lstStudentPersonalInfo = FetchStudentInfo;
                    objViewModel.liFeeStudentAccount = FetchTotalBalance;
                }
                else
                {
                    objViewModel.sMessage = "Invalid input(s) Please enquire with admin !!!";
                }
            }
            else
            {
                objViewModel.sMessage = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }

            return View(objViewModel);
        }
        #endregion

        #region Bulk Fee Collection
        public ActionResult ListBulkFeeCollection()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var FetchFrequency = (List<sup_frequency_type>)CMSHandler.FetchData<sup_frequency_type>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFrequencytype), sAcademicYear).DataSource.Data;
                    var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchclass), sAcademicYear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                        objViewModel.liSupFeeFrequency = new SelectList(FetchFrequency, Common.SUP_FREQUENCY_TYPE.FREQUENCY_TYPE_ID, Common.SUP_FREQUENCY_TYPE.FREQUENCY_TYPE);
                    if (FetchClass != null && FetchClass.Count > 0)
                        objViewModel.liClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");

            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "ListBulkFeeCollection", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "ListBulkFeeCollection", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindBulkFeeCollectionDetails(string sFrequencyId, string sClassId)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            var objFeeStudentAccountModel = new FEE_TRANSACTION();
            objFeeStudentAccountModel.FREQUENCY = sFrequencyId;
            objFeeStudentAccountModel.CLASS = sClassId;
            var FetchBulkFeeDetails = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchClassWiseFeeDetails), sAcademicYear).DataSource.Data;
            if (FetchBulkFeeDetails != null && FetchBulkFeeDetails.Count > 0)
                objViewModel.liFeeTransaction = FetchBulkFeeDetails;
            return View(objViewModel);
        }
        public JsonResult SaveBulkFeeCollection(string sStudentId)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                int ReceiptNo = 0;
                string Prefix = "00";
                string TransactionId = string.Empty;
                FEE_STUDENT_ACCOUNT objStudentAccountModel = new FEE_STUDENT_ACCOUNT();
                FEE_TRANSACTION objFeeTransactionModel = new FEE_TRANSACTION();
                AUTO_GENERATE_NUMBERS objAutoGenerateNumber = new AUTO_GENERATE_NUMBERS();
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    JsonFeeTransactionList objJson = JsonConvert.DeserializeObject<JsonFeeTransactionList>(sStudentId);
                    if (sStudentId != null)
                    {
                        foreach (var item in objJson.FeeTransactionList)
                        {
                            objStudentAccountModel.STUDENT_ID = item.STUDENT_ID;
                            var FetchStudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(objStudentAccountModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByStudentId), sAcademicYear).DataSource.Data;
                            if (FetchStudentInfo != null && FetchStudentInfo.Count > 0)
                            {
                                objFeeTransactionModel.STUDENT_ID = FetchStudentInfo.FirstOrDefault().STUDENT_ID;
                                objFeeTransactionModel.CLASS = FetchStudentInfo.FirstOrDefault().CLASS_ID;
                                objFeeTransactionModel.FREQUENCY = item.FREQUENCY;
                                objFeeTransactionModel.FREQUENCY_TO = item.FREQUENCY;
                                objFeeTransactionModel.PAYMENT_DATE = DateTime.Today.ToString("yyyy-MM-dd");
                                objFeeTransactionModel.USERNAME = Session[Common.SESSION_VARIABLES.USER_CODE].ToString();
                                objFeeTransactionModel.COLLECTED = item.COLLECTED;
                                var FetchReceiptNo = (List<AUTO_GENERATE_NUMBERS>)CMSHandler.FetchData<AUTO_GENERATE_NUMBERS>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAutoGenerateNumber)).DataSource.Data;
                                if (FetchReceiptNo != null && FetchReceiptNo.Count > 0)
                                    ReceiptNo = Convert.ToInt32(FetchReceiptNo.SingleOrDefault().exam_receipt_no);
                                else
                                {
                                    objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                    objResult.Message = Common.ErrorMessage.ExpectationFailed;
                                }
                                objAutoGenerateNumber.exam_receipt_no = Convert.ToString(ReceiptNo + 1);
                                objAutoGenerateNumber.exam_receipt_no = Prefix + objAutoGenerateNumber.exam_receipt_no;
                                var UpdateReceiptNo = CMSHandler.SaveOrUpdate(objAutoGenerateNumber, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateAutoGenerateNumber));
                                if (UpdateReceiptNo.Success)
                                {
                                    objFeeTransactionModel.PAYMENT_MODE = Common.PaymentMode.Cash;
                                    objFeeTransactionModel.RECEIPT_NO = objAutoGenerateNumber.exam_receipt_no;
                                    var SaveFeeTransaction = CMSHandler.SaveOrUpdate(objFeeTransactionModel, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeTransaction), sAcademicYear, true);
                                    if (SaveFeeTransaction.Success)
                                    {
                                        TransactionId = SaveFeeTransaction.RowUniqueId.ToString();
                                        objResult.sResult += "," + TransactionId;
                                        objStudentAccountModel.FREQUENCY_ID = item.FREQUENCY;
                                        var FetchStudentAccountOnlyCredit = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountOnlyCredit), sAcademicYear).DataSource.Data;
                                        if (FetchStudentAccountOnlyCredit != null && FetchStudentAccountOnlyCredit.Count > 0)
                                        {
                                            foreach (var dataitem in FetchStudentAccountOnlyCredit)
                                            {
                                                objStudentAccountModel.PAID_AMOUNT = dataitem.CREDIT;
                                                objStudentAccountModel.HEAD = dataitem.HEAD;
                                                objStudentAccountModel.TRANSACTION_ID = TransactionId;
                                                objStudentAccountModel.FREQUENCY = item.FREQUENCY;
                                                objStudentAccountModel.RECEIPT_NO = objFeeTransactionModel.RECEIPT_NO;
                                                objStudentAccountModel.DEBIT = dataitem.CREDIT;
                                                var SaveFeeCollection = CMSHandler.SaveOrUpdate(objStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeCollection));
                                                if (SaveFeeCollection.Success)
                                                {
                                                    var SaveStudentAccount = CMSHandler.SaveOrUpdate(objStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeStudentAccount), sAcademicYear);
                                                    if (SaveStudentAccount.Success)
                                                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                                    else
                                                    {
                                                        var DeleteFeeStudentAccount = CMSHandler.SaveOrUpdate(objStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeStudentAccount));
                                                        if (DeleteFeeStudentAccount.Success)
                                                        {
                                                            var DeleteFeeCollection = CMSHandler.SaveOrUpdate(objStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeCollection));
                                                            if (DeleteFeeCollection.Success)
                                                            {
                                                                var DeleteFeeTransaction = CMSHandler.SaveOrUpdate(objStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeTransaction));
                                                                if (DeleteFeeTransaction.Success)
                                                                    objResult.Message = Common.Messages.FailedToSaveRecords;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var DeleteFeeStudentAccount = CMSHandler.SaveOrUpdate(objStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeStudentAccount));
                                                    if (DeleteFeeStudentAccount.Success)
                                                    {
                                                        var DeleteFeeCollection = CMSHandler.SaveOrUpdate(objStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeCollection));
                                                        if (DeleteFeeCollection.Success)
                                                        {
                                                            var DeleteFeeTransaction = CMSHandler.SaveOrUpdate(objStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeTransaction));
                                                            if (DeleteFeeTransaction.Success)
                                                                objResult.Message = Common.Messages.FailedToSaveRecords;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                            objResult.Message = Common.ErrorMessage.ExpectationFailed;
                                        }
                                    }
                                    else
                                        objResult.Message = Common.Messages.FailedToSaveRecords;
                                }
                                else
                                {
                                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                                }
                            }
                            else
                            {
                                objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                                objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                            }
                        }
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
                objResult.sResult = objResult.sResult.Substring(1);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "ListBulkFeeCollection", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "ListBulkFeeCollection", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GenerateBulkFeeReceipt(string sStudentId)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            JsonResultData objResult = new JsonResultData();
            var StudentInfo = new List<Student_Personal_Info>();
            var FetchCollegeDetails = new List<CollegeDetails>();
            var BalanceInfo = new List<FEE_TRANSACTION>();
            var FeeStudentAccount = new List<FEE_STUDENT_ACCOUNT>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    JsonFeeTransactionList objJson = JsonConvert.DeserializeObject<JsonFeeTransactionList>(sStudentId);
                    foreach (var item in objJson.FeeTransactionList)
                    {
                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                        var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT();
                        FetchCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                        var FetchStudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(item, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByStudentId), sAcademicYear).DataSource.Data;
                        foreach (var list in FetchStudentInfo)
                            StudentInfo.Add(list);
                        var FetchTotalBalance = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(item, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTotalDebitByTransactionId)).DataSource.Data;
                        foreach (var dataitem in FetchTotalBalance)
                        {
                            if (FetchTotalBalance != null && FetchTotalBalance.Count > 0)
                                Number = Convert.ToInt32(FetchTotalBalance.FirstOrDefault().COLLECTED);
                            BalanceInfo.Add(dataitem);
                        }
                        var FetchFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(item, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByTransactionId), sAcademicYear).DataSource.Data;
                        foreach (var listitem in FetchFeeStudentAccount)
                            FeeStudentAccount.Add(listitem);
                    }
                    if (FetchCollegeDetails != null && FetchCollegeDetails.Count > 0)
                        objViewModel.liCollegeDetails = FetchCollegeDetails;
                    if (StudentInfo != null && StudentInfo.Count > 0)
                        objViewModel.lstStudentPersonalInfo = StudentInfo;
                    if (BalanceInfo != null && BalanceInfo.Count > 0)
                        objViewModel.liFeeTransaction = BalanceInfo;
                    if (FeeStudentAccount != null && FeeStudentAccount.Count > 0)
                        objViewModel.liFeeStudentAccount = FeeStudentAccount;
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "GenerateBulkFeeReceipt", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "GenerateBulkFeeReceipt", Ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult GenerateIndividualFeeReceipt(string sFrequencyId, string sStudentId)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT();
                    objFeeStudentAccountModel.FREQUENCY_ID = sFrequencyId;
                    objFeeStudentAccountModel.FREQUENCY = sFrequencyId;
                    objFeeStudentAccountModel.STUDENT_ID = sStudentId;
                    var FetchCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    var FetchStudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(objFeeStudentAccountModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByStudentId), sAcademicYear).DataSource.Data;
                    var FetchTotalBalance = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTotalDebitByStudentIdAndFrequencyId)).DataSource.Data;
                    if (FetchTotalBalance != null && FetchTotalBalance.Count > 0)
                        Number = Convert.ToInt32(FetchTotalBalance.FirstOrDefault().COLLECTED);
                    var FetchFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objFeeStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountForReceiptByStudentIdAndFrequencyId), sAcademicYear).DataSource.Data;
                    if (FetchCollegeDetails != null && FetchCollegeDetails.Count > 0)
                        objViewModel.liCollegeDetails = FetchCollegeDetails;
                    if (FetchStudentInfo != null && FetchStudentInfo.Count > 0)
                        objViewModel.lstStudentPersonalInfo = FetchStudentInfo;
                    if (FetchTotalBalance != null && FetchTotalBalance.Count > 0)
                        objViewModel.liFeeTransaction = FetchTotalBalance;
                    if (FetchFeeStudentAccount != null && FetchFeeStudentAccount.Count > 0)
                        objViewModel.liFeeStudentAccount = FetchFeeStudentAccount;
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "GenerateIndividualFeeReceipt", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "GenerateIndividualFeeReceipt", Ex.Message);
                }
            }
            return View(objViewModel);
        }
        #endregion

        #region PayUIntegration
        public ActionResult PayUSettlement()
        {
            return View();
        }

        public ActionResult GetSettlement(string sDate)
        {

            PayUSettleAPI(sDate);

            return View();
        }

        // PayU API .
        private void PayUSettleAPI(string sDate)
        {
            string hash, sInsertScripts = string.Empty;
            sDate = CommonMethods.DateConvertionForPayU(sDate);
            var APIs = (List<FEE_MERCHANT_ACCOUNT_INFO>)CMSHandler.FetchData<FEE_MERCHANT_ACCOUNT_INFO>(new FEE_MERCHANT_ACCOUNT_INFO() { ACCOUNT_TYPE = Common.FrequencyType.ExamFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchPayUSettleAPI)).DataSource.Data;
            if (APIs != null && APIs.Count > 0)
            {
                foreach (var item in APIs)
                {
                    if (!string.IsNullOrEmpty(item.HASH_SEQUENCE))
                    {
                        hash = GetHashValueForAPI(item.HASH_SEQUENCE, item.KEY, sDate, item.curl, item.SALT);
                        var value = new Dictionary<string, string>
                                            {
                                               { "key", item.KEY },
                                               { "command", item.curl },
                                               { "hash", hash },
                                               { "var1",  sDate}
                                            };

                        var apiResponse = CommonMethods.HttpPostMethod(item.BASE_URL, value);
                        if (apiResponse != null)
                        {
                            SettlementInfo settlementInfo = JsonConvert.DeserializeObject<SettlementInfo>(apiResponse.ToString());

                            if (settlementInfo != null && settlementInfo.Txn_details != null && settlementInfo.Txn_details.Count > 0)
                            {
                                //                                var isExisting=CMSHandler.FetchData<fee_payu_settlement_list>(new fee_payu_settlement_list() { })
                                sInsertScripts = @"INSERT INTO `fee_payu_settlement_list`
(
`payuid`,
`txnid`,
`txndate`,
`mode`,
`amount`,
`requestid`,
`requestdate`,
`requestaction`,
`requestamount`,
`mer_utr`,
`mer_service_fee`,
`mer_service_tax`,
`mer_net_amount`,
`bank_name`,
`issuing_bank`,
`merchant_subvention_amount`,
`cgst`,
`igst`,
`sgst`,`transaction_date`,`settlement_date`,`actual_settlement_date`)
VALUES";

                                foreach (var txnDetails in settlementInfo.Txn_details)
                                {
                                    sInsertScripts += @"(
                                                            '" + txnDetails.payuid + "',"
                                                            + "'" + txnDetails.txnid + "', "
                                                             + "'" + txnDetails.txndate + "',"
                                                             + "'" + txnDetails.mode + "',"
                                                             + "'" + txnDetails.amount + "',"
                                                             + "'" + txnDetails.requestid + "',"
                                                             + "'" + txnDetails.requestdate + "',"
                                                             + "'" + txnDetails.requestaction + "',"
                                                             + "'" + txnDetails.requestamount + "',"
                                                             + "'" + txnDetails.mer_utr + "',"
                                                             + "'" + txnDetails.mer_service_fee + "',"
                                                             + "'" + txnDetails.mer_service_tax + "',"
                                                             + "'" + txnDetails.mer_net_amount + "',"
                                                             + "'" + txnDetails.bank_name + "',"
                                                             + "'" + txnDetails.issuing_bank + "',"
                                                             + "'" + txnDetails.merchant_subvention_amount + "',"
                                                             + "'" + txnDetails.cgst + "',"
                                                             + "'" + txnDetails.igst + "',"
                                                             + "'" + txnDetails.sgst + "',"
                                                             + "'" + txnDetails.txndate + "',"
                                                             + "'" + txnDetails.requestdate + "',"
                                                             + "'" + sDate + "'),";
                                }
                                sInsertScripts = sInsertScripts.TrimEnd(',') + ";";

                                var result = new MySQLHandler().ExecuteAsScripts(sInsertScripts);
                            }
                            else
                            {

                            }
                        }
                    }
                    else
                    {

                    }
                }

            }
        }

        // API Hash Value .
        private string GetHashValueForAPI(string hashSeq, string sMerchant_key, string txids, string sCommand, string sSalt)
        {
            string hash_string = string.Empty;
            string hash = string.Empty;
            string[] hashVarsSeq = hashSeq.Split('|'); // spliting hash sequence from config       
            foreach (string hash_var in hashVarsSeq)
            {
                if (hash_var == "key")
                {
                    hash_string = hash_string + sMerchant_key;
                    hash_string = hash_string + '|';
                }
                else if (hash_var == "command")
                {
                    hash_string = hash_string + sCommand;
                    hash_string = hash_string + '|';
                }


            }
            hash_string = hash_string + txids;
            hash_string = hash_string + '|';
            hash_string += sSalt;// appending SALT

            hash = CommonMethods.Generatehash512(hash_string).ToLower();         //generating hash
            return hash;
        }

        // PayU Refund .
        public ActionResult PayURefundList()
        {

            var result = (List<fee_payu_response>)CMSHandler.FetchData<fee_payu_response>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRefundList)).DataSource.Data;
            return View(result);
        }

        // PayU Response .
        public ActionResult PayURepsponse(FormCollection payUResponse)
        {
            fee_payu_response feePayUResponse = new fee_payu_response();
            List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sGroupFrequency = string.Empty;
                    string sSQL = string.Empty;
                    if (payUResponse != null)
                    {
                        JsonResultData objResult = new JsonResultData();
                        string PayUResponse_Id = string.Empty;
                        string status = payUResponse["status"].ToString(); feePayUResponse = FormCollectionToClass(payUResponse);
                        objResult.oResult = feePayUResponse;
                        var isExisting = (List<fee_payu_response>)CMSHandler.FetchData<fee_payu_response>(feePayUResponse, FeeSQL.GetFeeSQL(FeeSQLCommands.IsPayUResponseExist), sAcademicYear).DataSource.Data;
                        if (isExisting != null && isExisting.Count > 0)
                        {
                            feePayUResponse.PAYU_RESPONSE_ID = isExisting.FirstOrDefault().PAYU_RESPONSE_ID;
                            var updateState = CMSHandler.SaveOrUpdate(feePayUResponse, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdatePayUResponse), sAcademicYear);
                            return View(feePayUResponse);
                        }
                        var s = payUResponse["udf5"];

                        var result = CMSHandler.SaveOrUpdate(feePayUResponse, FeeSQL.GetFeeSQL(FeeSQLCommands.SavePayUResponse), sAcademicYear, true);
                        var lastInsertId = (List<fee_payu_response>)CMSHandler.FetchData<fee_payu_response>(new fee_payu_response() { txnid = feePayUResponse.txnid },
                             FeeSQL.GetFeeSQL(FeeSQLCommands.FetchPayUresponseByTxnid), sAcademicYear).DataSource.Data;
                        PayUResponse_Id = (lastInsertId != null && lastInsertId.Count > 0) ? lastInsertId.FirstOrDefault().PAYU_RESPONSE_ID : string.Empty;
                        if (status.Equals("success"))
                        {
                            if (payUResponse["udf2"].ToString().Equals(Common.FrequencyType.HostelFee))
                            {
                                //var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.HostelFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                                //if (FetchFrequency != null && FetchFrequency.Count > 0)
                                //{
                                //    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = payUResponse["udf1"].ToString(), FREQUENCY_ID = FetchFrequency.FirstOrDefault().FREQUENCY_ID }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentId), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                                //    if (studentAcountList != null && studentAcountList.Count > 0)
                                //    {
                                feePayUResponse = FeeCollection(feePayUResponse, payUResponse, PayUResponse_Id, sAcademicYear);
                                //    studentAcountList.FirstOrDefault().STATUS = feePayUResponse.status;
                                //    studentAcountList.FirstOrDefault().RECEIPT_NO = feePayUResponse.RECEIPT_NO;

                                //}
                                // }
                                // feePayUResponse = AdmissionFeeCollection(feePayUResponse, payUResponse, PayUResponse_Id, sAcademicYear);
                                // return RedirectToAction("AdmissionFeeStatus", feePayUResponse);
                                //string sJson = JsonConvert.SerializeObject(studentAcountList);
                                //Session[Common.SESSION_VARIABLES.PAYMENT_RESPONSE] = sJson;
                                return RedirectToAction("PaymentStatus", new { PayUResponse_Id = CommonMethods.EnryptString(PayUResponse_Id), sFeeRootId = CommonMethods.EnryptString(Common.FeeRoot.HostelFee) });
                            }
                            else if (payUResponse["udf2"].ToString().Equals(Common.FrequencyType.SemesterFee))
                            {
                                // var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.SemesterFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                                //if (FetchFrequency != null && FetchFrequency.Count > 0)
                                //{
                                //sGroupFrequency = FetchFrequency.FirstOrDefault().FREQUENCY_ID.TrimEnd(',');
                                //sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdForSemesterFee).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, sGroupFrequency);
                                //studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = payUResponse["udf1"].ToString(), FREQUENCY_ID = sGroupFrequency, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, sSQL, Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                                //if (studentAcountList != null && studentAcountList.Count > 0)
                                //{
                                feePayUResponse = FeeCollection(feePayUResponse, payUResponse, PayUResponse_Id, sAcademicYear);
                                //}
                                //   }                              
                                return RedirectToAction("PaymentStatus", new { PayUResponse_Id = CommonMethods.EnryptString(PayUResponse_Id), sFeeRootId = CommonMethods.EnryptString(Common.FeeRoot.CollegeFee) });
                            }
                            else if (payUResponse["udf2"].ToString().Equals(Common.FrequencyType.MessFee))
                            {
                                //var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.MessFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                                //if (FetchFrequency != null && FetchFrequency.Count > 0)
                                //{
                                feePayUResponse = FeeCollection(feePayUResponse, payUResponse, PayUResponse_Id, sAcademicYear);
                                // }
                                return RedirectToAction("PaymentStatus", new { PayUResponse_Id = CommonMethods.EnryptString(PayUResponse_Id), sFeeRootId = CommonMethods.EnryptString(Common.FeeRoot.MessFee) });
                            }
                            else if (payUResponse["udf2"].ToString().Equals(Common.FrequencyType.ExamFee))
                            {
                                //var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(
                                //    new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.ExamFee },
                                //    FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;

                                //if (FetchFrequency != null && FetchFrequency.Count > 0)
                                //{
                                //    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(
                                //        new FEE_STUDENT_ACCOUNT() { STUDENT_ID = payUResponse["udf1"].ToString(), FREQUENCY_ID = FetchFrequency.FirstOrDefault().FREQUENCY_ID },
                                //        FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentId), sAcademicYear).DataSource.Data;
                                //    if (studentAcountList != null && studentAcountList.Count > 0)
                                //    {
                                feePayUResponse = FeeCollection(feePayUResponse, payUResponse, PayUResponse_Id, sAcademicYear);
                                //    studentAcountList.FirstOrDefault().STATUS = feePayUResponse.status;
                                //    studentAcountList.FirstOrDefault().RECEIPT_NO = feePayUResponse.RECEIPT_NO;
                                //}
                                //  }
                                // feePayUResponse = AdmissionFeeCollection(feePayUResponse, payUResponse, PayUResponse_Id, sAcademicYear);
                                // return RedirectToAction("AdmissionFeeStatus", feePayUResponse);
                                //  string sJson = JsonConvert.SerializeObject(studentAcountList);
                                // Session[Common.SESSION_VARIABLES.PAYMENT_RESPONSE] = sJson;
                                return RedirectToAction("PaymentStatus", new { PayUResponse_Id = CommonMethods.EnryptString(PayUResponse_Id), sFeeRootId = CommonMethods.EnryptString(Common.FeeRoot.CollegeFee) });
                            }
                            else
                            {
                                feePayUResponse.status = "Pending";
                                feePayUResponse.error_Message = "If Money is debited from your account, Please contact us with your Transaction ID";
                            }
                        }
                        else
                        {
                            feePayUResponse.status = "Failed";
                        }
                    }
                    else
                    {
                        feePayUResponse.status = "Failed";
                        feePayUResponse.error_Message = "Null response, If Money is debited from your account, Please contact us with your Transaction ID ";
                    }
                }
                else
                {
                    feePayUResponse.status = "Failed";
                    feePayUResponse.error_Message = "Session Expired,If Money is debited from your account, Please contact us with your Transaction ID ";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    var LineNumber = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                    objHandler.WriteError("Fee", "PayURepsponse", "Err MSg: " + ex.Message, "LineNumber:" + LineNumber);
                }
                feePayUResponse.error_Message = "Sorry for the trouble,Please note Transaction Id and enquire with admin !!";
                Session[Common.SESSION_VARIABLES.USER_ID] = null;
                return View(feePayUResponse);

            }
            return View(feePayUResponse);
        }

        // PayU Request Insert.
        public JsonResult PayURequest(string objInput)
        {
            JsonResultData objResult = new JsonResultData();
            FEE_PAYU_REQUEST obj = JsonConvert.DeserializeObject<FEE_PAYU_REQUEST>(objInput);
            var fresult = (List<FEE_PAYU_REQUEST>)CMSHandler.FetchData<FEE_PAYU_REQUEST>(obj, FeeSQL.GetFeeSQL(FeeSQLCommands.IsPayURequestExist)).DataSource.Data;
            if (fresult != null && fresult.Count > 0)
            {
                objResult.Message = "Sorry !! Invalid attempt, try again.Please enquire with admin For more information.";
                return Json(objResult);
            }
            var result = CMSHandler.SaveOrUpdate(obj, FeeSQL.GetFeeSQL(FeeSQLCommands.SavePayURequest));
            objResult.Message = "";
            if (!result.Success)
                objResult.Message = "Sorry !! Failed to process, Please enquire with admin.";
            return Json(objResult);
        }

        public ActionResult PaymentOption(string sStudentId, string sFrequencyId)
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                PayUPost objPayUPost = new PayUPost();
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    decimal TotalAmount = 0;
                    var FetchFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { FREQUENCY_ID = sFrequencyId, STUDENT_ID = sStudentId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFrequencyId), sAcademicYear).DataSource.Data;
                    var payUPaymentInfo = (List<FEE_MERCHANT_ACCOUNT_INFO>)CMSHandler.FetchData<FEE_MERCHANT_ACCOUNT_INFO>(new FEE_MERCHANT_ACCOUNT_INFO() { API_TYPE = Common.PayUMoneyAPIsType.BaseUrl, ACCOUNT_TYPE = Common.FrequencyType.ExamFee, STUDENT_ID = sStudentId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchPayUPaymentAccountInfo)).DataSource.Data;
                    if (!string.IsNullOrEmpty(sStudentId) && !string.IsNullOrEmpty(sFrequencyId))
                    {
                        if (FetchFeeStudentAccount != null && FetchFeeStudentAccount.Count > 0)
                        {
                            TotalAmount = FetchFeeStudentAccount.Sum(o => Convert.ToDecimal(o.BALANCE));

                            if (TotalAmount > 0)
                            {
                                if (payUPaymentInfo != null && payUPaymentInfo.Count > 0)
                                {
                                    objPayUPost.productinfo = "Exam Fee";
                                    objPayUPost.key = payUPaymentInfo.FirstOrDefault().KEY;
                                    objPayUPost.salt = payUPaymentInfo.FirstOrDefault().SALT;
                                    objPayUPost.udf1 = payUPaymentInfo.FirstOrDefault().STUDENT_ID;
                                    objPayUPost.email = payUPaymentInfo.FirstOrDefault().STU_EMAILID;
                                    objPayUPost.udf5 = payUPaymentInfo.FirstOrDefault().STU_MOBILENO;
                                    objPayUPost.firstname = payUPaymentInfo.FirstOrDefault().FIRST_NAME;
                                    objPayUPost.udf3 = payUPaymentInfo.FirstOrDefault().ROLL_NO.Trim();
                                    objPayUPost.udf4 = payUPaymentInfo.FirstOrDefault().REGISTER_NO.Trim();
                                    objPayUPost.udf2 = sFrequencyId;
                                    objPayUPost.txnid = CommonMethods.CommonTransactionId(sStudentId);
                                    objPayUPost.amount = Convert.ToDecimal(TotalAmount).ToString("g29");
                                    objPayUPost.hash = GetHashValue(payUPaymentInfo.FirstOrDefault().HASH_SEQUENCE, payUPaymentInfo.FirstOrDefault().KEY, objPayUPost);
                                    objPayUPost.surl = payUPaymentInfo.FirstOrDefault().surl;
                                    objPayUPost.furl = payUPaymentInfo.FirstOrDefault().furl;
                                    objPayUPost.curl = payUPaymentInfo.FirstOrDefault().curl;
                                    objPayUPost.phone = payUPaymentInfo.FirstOrDefault().STU_MOBILENO;
                                    objPayUPost.BASE_URL = payUPaymentInfo.FirstOrDefault().BASE_URL.Trim();
                                    return View(objPayUPost);
                                }
                                else
                                {
                                    objPayUPost.sMessage = "Missing Fields,Please enquire with Admin..!!!";
                                }
                            }
                            else
                            {
                                objPayUPost.sMessage = "Sorry !!! Invalid Amount.. Please enquire with Admin!!!";
                            }

                        }
                        else
                        {
                            objPayUPost.sMessage = "Not Registered ,Please enquire with Admin..!!!";

                        }
                    }
                    else
                    {
                        objPayUPost.sMessage = "Missing Fields, Please enquire with Admin..!!!";
                    }

                    return View(objPayUPost);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "PaymentOption(string sStudentId, string sFrequencyId)", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "PaymentOption(string sStudentId, string sFrequencyId)", ex.Message);
                    }
                    objPayUPost.sMessage = " Exception Occured...Sorry for the trouble,Please enquire with Admin..!!!";
                    return View(objPayUPost);
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }

        }

        private fee_payu_response FormCollectionToClass(FormCollection collection)
        {

            fee_payu_response obj = new fee_payu_response();
            foreach (var key in collection.AllKeys)
            {
                Type t = obj.GetType();
                System.Reflection.PropertyInfo propertyInfo = obj.GetType().GetProperty(key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(obj, collection[key], null);
                }

            }
            return obj;
        }

        public JsonResult GetHashValue(string sStudentId, string sFrequencyId, string sEmail, string sMobile, string sTxnid)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                PayUPost objPayUPost = new PayUPost();
                if (!string.IsNullOrEmpty(sStudentId) && !string.IsNullOrEmpty(sFrequencyId) && !string.IsNullOrEmpty(sEmail) && !string.IsNullOrEmpty(sMobile))
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { FREQUENCY_ID = sFrequencyId, STUDENT_ID = sStudentId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFrequencyId), sAcademicYear).DataSource.Data;
                    var payUPaymentInfo = (List<FEE_MERCHANT_ACCOUNT_INFO>)CMSHandler.FetchData<FEE_MERCHANT_ACCOUNT_INFO>(new FEE_MERCHANT_ACCOUNT_INFO() { API_TYPE = Common.PayUMoneyAPIsType.BaseUrl, ACCOUNT_TYPE = Common.FrequencyType.ExamFee, STUDENT_ID = sStudentId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchPayUPaymentAccountInfo)).DataSource.Data;

                    if (FetchFeeStudentAccount != null && FetchFeeStudentAccount.Count > 0)
                    {
                        if (payUPaymentInfo != null && payUPaymentInfo.Count > 0)
                        {
                            objPayUPost.productinfo = "Exam Fee";
                            objPayUPost.key = payUPaymentInfo.FirstOrDefault().KEY;
                            objPayUPost.salt = payUPaymentInfo.FirstOrDefault().SALT;
                            objPayUPost.udf1 = payUPaymentInfo.FirstOrDefault().STUDENT_ID;
                            objPayUPost.email = sEmail.Trim();
                            objPayUPost.udf5 = payUPaymentInfo.FirstOrDefault().STU_MOBILENO;
                            objPayUPost.firstname = payUPaymentInfo.FirstOrDefault().FIRST_NAME;
                            objPayUPost.udf3 = payUPaymentInfo.FirstOrDefault().ROLL_NO.Trim();
                            objPayUPost.udf4 = payUPaymentInfo.FirstOrDefault().REGISTER_NO.Trim();
                            objPayUPost.udf2 = sFrequencyId;
                            objPayUPost.txnid = sTxnid;
                            objPayUPost.phone = sMobile.Trim();
                            objPayUPost.amount = Convert.ToDecimal(FetchFeeStudentAccount.Sum(o => Convert.ToDecimal(o.BALANCE))).ToString("g29");
                            objPayUPost.hash = GetHashValue(payUPaymentInfo.FirstOrDefault().HASH_SEQUENCE, payUPaymentInfo.FirstOrDefault().KEY, objPayUPost);
                            objResultData.sResult = objPayUPost.hash;
                            objResultData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            objResultData.ErrorCode = Common.ErrorCode.FailedDependency;
                            objResultData.Message = "Missing Fields,Please Admin..!!!";
                        }
                    }
                    else
                    {
                        objResultData.ErrorCode = Common.ErrorCode.FailedDependency;
                        objResultData.Message = "Not Registered ,Please Admin..!!!";
                    }
                }
                else
                {
                    objResultData.ErrorCode = Common.ErrorCode.FailedDependency;
                    objResultData.Message = Common.ErrorMessage.FailedDependency;
                }
            }
            else
            {
                objResultData.ErrorCode = Common.ErrorCode.RequestTimeout;
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(objResultData);
        }

        private string GetHashValue(string hashSeq, string sMerchant_key, PayUPost objPost)
        {
            string hash_string = string.Empty;
            string hash = string.Empty;
            string[] hashVarsSeq = hashSeq.Split('|'); // spliting hash sequence from config       
            foreach (string hash_var in hashVarsSeq)
            {
                if (hash_var == "key")
                {
                    hash_string = hash_string + sMerchant_key;
                    hash_string = hash_string + '|';
                }
                else if (hash_var == "txnid")
                {
                    hash_string = hash_string + objPost.txnid;
                    hash_string = hash_string + '|';
                }
                else if (hash_var == "amount")
                {
                    hash_string = hash_string + Convert.ToDecimal(objPost.amount).ToString("g29");
                    hash_string = hash_string + '|';
                }
                else
                {
                    Type t = objPost.GetType();
                    hash_string = hash_string + (objPost.GetType().GetProperty(hash_var).GetValue(objPost, null) != null ? objPost.GetType().GetProperty(hash_var).GetValue(objPost, null) : "");// isset if else
                    hash_string = hash_string + '|';
                }
            }

            hash_string += objPost.salt;// appending SALT

            hash = CommonMethods.Generatehash512(hash_string).ToLower();         //generating hash
            return hash;
        }
        #endregion

        #region Fee Structure
        public ActionResult FeeStructure()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    var FetchProgrammeGroup = (List<SUP_APPTYPE_PROG_GROUPS>)CMSHandler.FetchData<SUP_APPTYPE_PROG_GROUPS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                    if (FetchProgrammeGroup != null && FetchProgrammeGroup.Count > 0)
                        objViewModel.ProgrammeGroupList = new SelectList(FetchProgrammeGroup, Common.SUP_APPTYPE_PROG_GROUPS.PROGRAMME_GROUP_ID, Common.SUP_APPTYPE_PROG_GROUPS.PROGRAMME_NAME);
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    var FetchClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear), sAcademicYear).DataSource.Data;
                    if (FetchClassYear != null && FetchClassYear.Count > 0)
                        objViewModel.ClassYearList = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.CP_CLASSES_2017.CLASS_YEAR);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStructure", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStructure", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult AddOrEditFeeStructure()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear), sAcademicYear).DataSource.Data;
                    //List<SUP_FEE_FREQUENCY> liFrequency = new List<SUP_FEE_FREQUENCY>();
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    var FetchProgrammeGroup = (List<SUP_APPTYPE_PROG_GROUPS>)CMSHandler.FetchData<SUP_APPTYPE_PROG_GROUPS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                    if (FetchProgrammeGroup != null && FetchProgrammeGroup.Count > 0)
                        objViewModel.ProgrammeGroupList = new SelectList(FetchProgrammeGroup, Common.SUP_APPTYPE_PROG_GROUPS.PROGRAMME_GROUP_ID, Common.SUP_APPTYPE_PROG_GROUPS.PROGRAMME_NAME);
                    if (FetchClassYear != null && FetchClassYear.Count > 0)
                        objViewModel.ClassYearList = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.CP_CLASSES_2017.CLASS_YEAR);
                    //liFrequency.Add(new SUP_FEE_FREQUENCY() { FREQUENCY_ID = "0", FREQUENCY_NAME = "---Select---" });
                    //objViewModel.FrequencyList = new SelectList(liFrequency, Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, Common.SUP_FEE_FREQUENCY.FREQUENCY_NAME);
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AddOrEditFeeStructure", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AddOrEditFeeStructure", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult AddOrEditFeeHostelAndMessStructure()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();


                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AddOrEditFeeHostelAndMessStructure", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AddOrEditFeeHostelAndMessStructure", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult AddOrEditFeeMessStructure()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();


                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AddOrEditFeeMessStructure", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AddOrEditFeeMessStructure", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchFrequencyByAcademicYear(string Academicyear)
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;

                    var FetchFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(new SUP_FEE_FREQUENCY() { FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeModeAndFeeRootId), sAcademicYear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                    {
                        foreach (var item in FetchFrequency)
                            objResult.sResult += "<option value='" + item.FREQUENCY_ID + "' cms-FrequencyType=" + item.FEE_MODE + ">" + item.FREQUENCY_NAME + "</option>";
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
                    objHandler.WriteError("FetchFrequencyByAcademicYear", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchFrequencyByAcademicYear", "Fee", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchHostelFrequencyByAcademicYear(string Academicyear)
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;

                    var FetchFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(new SUP_FEE_FREQUENCY() { FEE_ROOT_ID = Common.FeeRoot.HostelFee }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeModeAndFeeRootId), sAcademicYear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                    {
                        foreach (var item in FetchFrequency)
                            objResult.sResult += "<option value='" + item.FREQUENCY_ID + "' cms-FrequencyType=" + item.FEE_MODE + ">" + item.FREQUENCY_NAME + "</option>";
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
                    objHandler.WriteError("FetchHostelFrequencyByAcademicYear", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchHostelFrequencyByAcademicYear", "Fee", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchMessFrequencyByAcademicYear(string Academicyear)
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;

                    var FetchFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(new SUP_FEE_FREQUENCY() { FEE_ROOT_ID = Common.FeeRoot.MessFee }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeModeAndFeeRootId), sAcademicYear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                    {
                        foreach (var item in FetchFrequency)
                            objResult.sResult += "<option value='" + item.FREQUENCY_ID + "' cms-FrequencyType=" + item.FEE_MODE + ">" + item.FREQUENCY_NAME + "</option>";
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
                    objHandler.WriteError("FetchMessFrequencyByAcademicYear", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchMessFrequencyByAcademicYear", "Fee", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchHostelAndMessMainHeadByFrequencyId(string sFrequencyId, string Academicyear)
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;
                    var FetchFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(new SUP_FEE_FREQUENCY() { FREQUENCY_ID = sFrequencyId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMainHeadsByFrequencyIdForMessAndHostel), sAcademicYear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                    {
                        foreach (var item in FetchFrequency)
                            objResult.sResult += "<option value='" + item.MAIN_HEAD_ID + "'>" + item.MAIN_HEAD + "</option>";
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
                    objHandler.WriteError("FetchHostelAndMessMainHeadByFrequencyId", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchHostelAndMessMainHeadByFrequencyId", "Fee", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchHostelHeadsByFrequencyId(string sFrequencyId, string Academicyear)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;
                    string query = @"SELECT 
                                        MH.HEAD_ID AS HEAD_ID,
                                        SH.HEAD AS HEAD_NAME,
                                        SH.HEAD_CODE AS HEAD_CODE
                                    FROM
                                        FEE_MAIN_HEADS AS MH
                                            INNER JOIN
                                        SUP_HEAD AS SH ON SH.HEAD_ID = MH.HEAD_ID
                                    WHERE
                                        MH.ACADEMIC_YEAR = @AC_YEAR
                                            AND MH.FREQUENCY_ID = @FREQUENCY_ID
                                            AND MH.IS_DELETED != 1
                                    GROUP BY MH.HEAD_ID";

                    var lstHeads = DapperHandler.FetchAll<dynamic>(query, new { AC_YEAR = sAcademicYear, FREQUENCY_ID = sFrequencyId }).ToList();
                    if (lstHeads != null && lstHeads.Count > 0)
                    {
                        foreach (var item in lstHeads)
                        {
                            objResult.sResult += "<option value='" + item.HEAD_ID + "'>" + item.HEAD_NAME + " (" + item.HEAD_CODE + ")</option>";
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
                    objHandler.WriteError("FetchHostelHeadsByFrequencyId", "Fee", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindFeeStructureByFrequencyIdAndClassIdForAddOrEdit(string sProgrammeGroupId, string sFrequency, string sClassYearId, string Academicyear)
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;
                    objModel.PROGRAMME = sProgrammeGroupId;
                    objModel.FREQUENCY = sFrequency;
                    objModel.CLASS_YEAR_ID = sClassYearId;
                    objViewModel.lstFeeStructure = (List<FEE_STRUCTURE>)CMSHandler.FetchData<FEE_STRUCTURE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.BindFeeStructureForEditByClass), sAcademicYear).DataSource.Data;
                    objViewModel.Bank = (List<SUP_BANK_ACCOUNT>)CMSHandler.FetchData<SUP_BANK_ACCOUNT>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupBankAccounts), sAcademicYear).DataSource.Data;
                    objViewModel.FeeCategory = (List<Sup_Fee_Category>)CMSHandler.FetchData<Sup_Fee_Category>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeeCategory), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindFeeStructureByFrequencyIdAndClassIdForAddOrEdit", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindFeeStructureByFrequencyIdAndClassIdForAddOrEdit", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindHostelAndMessFeeStructureByFrequencyIdForAddOrEdit(string sFrequency, string Academicyear)
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;
                    objModel.FREQUENCY = sFrequency;
                    objViewModel.lstFeeStructure = (List<FEE_STRUCTURE>)CMSHandler.FetchData<FEE_STRUCTURE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.BindFeeHostelStructure), sAcademicYear).DataSource.Data;
                    objViewModel.Bank = (List<SUP_BANK_ACCOUNT>)CMSHandler.FetchData<SUP_BANK_ACCOUNT>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupBankAccounts), sAcademicYear).DataSource.Data;
                    objViewModel.FeeCategory = (List<Sup_Fee_Category>)CMSHandler.FetchData<Sup_Fee_Category>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeeCategory), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindHostelAndMessFeeStructureByFrequencyIdForAddOrEdit", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindHostelAndMessFeeStructureByFrequencyIdForAddOrEdit", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindFeeStructureByFrequencyIdAndClassId(string sProgrammeGroupId, string sFrequency, string sClassYearId, string Academicyear)
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.PROGRAMME = sProgrammeGroupId;
                    objModel.FREQUENCY = sFrequency;
                    objModel.CLASS_YEAR_ID = sClassYearId;
                    objViewModel.lstFeeStructure = (List<FEE_STRUCTURE>)CMSHandler.FetchData<FEE_STRUCTURE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.BindFeeStructureForEditByClass), Academicyear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindFeeStructureByFrequencyIdAndClassId", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindFeeStructureByFrequencyIdAndClassId", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindFeeHosteAndMesslStructureByFrequencyId(string sFrequency, string Academicyear)
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {

                    objModel.FREQUENCY = sFrequency;
                    objViewModel.lstFeeStructure = (List<FEE_STRUCTURE>)CMSHandler.FetchData<FEE_STRUCTURE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.BindFeeHostelStructure), Academicyear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindFeeHosteAndMesslStructureByFrequencyId", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindFeeHosteAndMesslStructureByFrequencyId", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchHeadsByFeeCategory(string sFeeCategory)
        {
            JsonResultData objResult = new JsonResultData();
            SUP_HEAD objModel = new SUP_HEAD();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                objModel.FEE_CATEGORY = sFeeCategory;
                var FetchHeads = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHeadByFeeCategory), sAcademicyear).DataSource.Data;
                if (FetchHeads != null && FetchHeads.Count > 0)
                {
                    foreach (var item in FetchHeads)
                        objResult.sResult += "<option value='" + item.HEAD_ID + "'>" + item.HEAD + "</option>";
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                objResult.Message = Common.ErrorMessage.RequestTimeout;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchClassByProgrammeId(string sProgramme)
        {
            JsonResultData objResult = new JsonResultData();
            cp_Classes_2017 objModel = new cp_Classes_2017();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                objModel.PROGRAMME = sProgramme;
                objModel.PROGRAMME_GROUP_ID = sProgramme;
                var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByProgrammeGroupId), sAcademicyear).DataSource.Data;
                if (FetchClass != null && FetchClass.Count > 0)
                {
                    foreach (var item in FetchClass)
                        objResult.sResult += "<option value='" + item.CLASS_ID + "'>" + item.CLASS_NAME + "</option>";
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                objResult.Message = Common.ErrorMessage.RequestTimeout;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchFeeStructureById(string sFeeStructureId)
        {
            JsonResultData objResult = new JsonResultData();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                if (sFeeStructureId != null)
                {
                    string sOption = string.Empty;
                    objModel.FEE_STRUCTURE_ID = sFeeStructureId;
                    var FetchFeeStructure = (List<FEE_STRUCTURE>)CMSHandler.FetchData<FEE_STRUCTURE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStructureById)).DataSource.Data;
                    return Json(FetchFeeStructure);
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                    objResult.Message = Common.ErrorMessage.BadRequest;
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                objResult.Message = Common.ErrorMessage.RequestTimeout;
                return Json(objResult, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SaveOrUpdateFeeStructure(string sFeeStructure)
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                if (sFeeStructure != null)
                {
                    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                    {
                        var Result = serializer.Deserialize<FEE_STRUCTURE_LIST>(sFeeStructure);
                        foreach (var item in Result.FEE_STRUCTURE)
                        {
                            //var FetchProgramme = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(item, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchClassInfoById)).DataSource.Data;                          
                            if (!string.IsNullOrEmpty(item.FEE_STRUCTURE_ID))
                            {
                                if (!string.IsNullOrEmpty(item.AMOUNT))
                                {
                                    var sresultArgs = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateFeeStructure));
                                    objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                }
                                else
                                {
                                    var DeleteFeeStructure = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeStructure));
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.AMOUNT))
                                {
                                    var sresultArgs = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeStructure));
                                    objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                }
                                else
                                {

                                }
                            }

                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                        objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                    objResult.Message = Common.ErrorMessage.BadRequest;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveOrUpdateFeeStructure", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveOrUpdateFeeStructure", "Fee", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOrUpdateFeeHostelAndMessStructure(string sFeeStructure)
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                if (sFeeStructure != null)
                {
                    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                    {
                        var Result = serializer.Deserialize<FEE_STRUCTURE_LIST>(sFeeStructure);
                        foreach (var item in Result.FEE_STRUCTURE)
                        {
                            //var FetchProgramme = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(item, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchClassInfoById)).DataSource.Data;                          
                            if (!string.IsNullOrEmpty(item.FEE_STRUCTURE_ID))
                            {
                                if (!string.IsNullOrEmpty(item.AMOUNT))
                                {
                                    var sresultArgs = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateFeeStructureForHostelAndMess));
                                    objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                }
                                else
                                {
                                    var DeleteFeeStructure = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeHostelAndMessStructure));
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.AMOUNT))
                                {
                                    var sresultArgs = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveHostelAndMessFeeStructure));
                                    objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                }
                                else
                                {

                                }
                            }

                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                        objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                    objResult.Message = Common.ErrorMessage.BadRequest;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveOrUpdateFeeStructure", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveOrUpdateFeeStructure", "Fee", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteFeeStructure(string sFeeStructureId)
        {
            JsonResultData ObjResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (!string.IsNullOrEmpty(sFeeStructureId))
                    {
                        var objModel = new FEE_STRUCTURE();
                        objModel.FEE_STRUCTURE_ID = sFeeStructureId;
                        var sresultArgs = CMSHandler.SaveOrUpdate(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeStructure));
                        ObjResult.Message = (sresultArgs.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                        ObjResult.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        ObjResult.ErrorCode = Common.ErrorCode.BadRequest;
                        ObjResult.Message = Common.ErrorMessage.ExpectationFailed;
                    }
                }
                else
                {
                    ObjResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    ObjResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveOrUpdateFeeStructure", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveOrUpdateFeeStructure", "Fee", ex.Message);
                }
            }
            return Json(ObjResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteHostelAndMessFeeStructure(string sFeeStructureId)
        {
            JsonResultData ObjResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (!string.IsNullOrEmpty(sFeeStructureId))
                    {
                        var objModel = new FEE_STRUCTURE();
                        objModel.FEE_STRUCTURE_ID = sFeeStructureId;
                        var sresultArgs = CMSHandler.SaveOrUpdate(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeHostelAndMessStructure));
                        ObjResult.Message = (sresultArgs.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                        ObjResult.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        ObjResult.ErrorCode = Common.ErrorCode.BadRequest;
                        ObjResult.Message = Common.ErrorMessage.ExpectationFailed;
                    }
                }
                else
                {
                    ObjResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    ObjResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveOrUpdateFeeStructure", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveOrUpdateFeeStructure", "Fee", ex.Message);
                }
            }
            return Json(ObjResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Fee Discount
        public ActionResult ListFeeDiscount()
        {
            FeeDiscountViewModel objViewModel = new FeeDiscountViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                objViewModel.lstFeeDiscount = (List<FEE_DISCOUNT>)CMSHandler.FetchData<FEE_DISCOUNT>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeDiscount), sAcademicYear).DataSource.Data;
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objViewModel);
        }
        public JsonResult SaveOrUpdateFeeDiscount(string sDiscount, string sFrequency)
        {
            JsonResultData objResult = new JsonResultData();
            if (sDiscount != null)
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var Result = JsonConvert.DeserializeObject<FEE_DISCOUNT>(sDiscount);
                    if (!string.IsNullOrEmpty(Result.DISCOUNT_ID))
                    {
                        var sresultArgs = CMSHandler.SaveOrUpdate(Result, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateFeeDiscount));
                        if (sresultArgs.Success)
                        {
                            var obj = JsonConvert.DeserializeObject<List<FEE_DISCOUNT_FREQUENCY>>(sFrequency);
                            var TempList = new List<FEE_DISCOUNT_FREQUENCY>();
                            var FetchFeeDiscountFrequency = (List<FEE_DISCOUNT_FREQUENCY>)CMSHandler.FetchData<FEE_DISCOUNT_FREQUENCY>(Result, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeDiscountFrequencyByDiscountId)).DataSource.Data;
                            //this loop removes existing main heads 
                            if (FetchFeeDiscountFrequency != null && FetchFeeDiscountFrequency.Count() > 0)
                            {
                                if (obj != null && obj.Count > 0)
                                {
                                    foreach (var list in FetchFeeDiscountFrequency)
                                    {

                                        TempList.Add(obj.SingleOrDefault(S => S.DISCOUNT_ID == list.DISCOUNT_ID && S.FREQUENCY_ID == list.FREQUENCY_ID));
                                        obj.Remove(obj.SingleOrDefault(S => S.DISCOUNT_ID == list.DISCOUNT_ID && S.FREQUENCY_ID == list.FREQUENCY_ID));
                                    }
                                    foreach (var item in TempList)
                                    {
                                        if (item != null)
                                            FetchFeeDiscountFrequency.Remove(FetchFeeDiscountFrequency.SingleOrDefault(S => S.DISCOUNT_ID == item.DISCOUNT_ID && S.FREQUENCY_ID == item.FREQUENCY_ID));
                                    }
                                    //this loop deletes records
                                    foreach (var item in FetchFeeDiscountFrequency)
                                    {
                                        var o = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeDiscountFrequency));
                                    }
                                }
                            }
                            //this loop saves records
                            foreach (var item in obj)
                            {
                                item.DISCOUNT_ID = Result.DISCOUNT_ID;
                                var SaveFeeFrequency = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeDiscountFrequency));
                                if (SaveFeeFrequency.Success)
                                {
                                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    objResult.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        var sresultArgs = CMSHandler.SaveOrUpdate(Result, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeDiscount), null, true);
                        if (sresultArgs.Success)
                        {
                            var s = sresultArgs.RowUniqueId.ToString();
                            var obj = JsonConvert.DeserializeObject<List<FEE_DISCOUNT_FREQUENCY>>(sFrequency);
                            foreach (var item in obj)
                            {
                                item.DISCOUNT_ID = sresultArgs.RowUniqueId.ToString();
                                var SaveFeeDiscountFrequency = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeDiscountFrequency));
                                objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.BadRequest : Common.ErrorCode.BadRequest;
                            }
                        }
                        else
                        {

                        }
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = Common.ErrorMessage.BadRequest;
            }

            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteFeeDiscount(string sDiscountId)
        {
            JsonResultData ObjResult = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                if (!string.IsNullOrEmpty(sDiscountId))
                {
                    var objModel = new FEE_DISCOUNT();
                    objModel.DISCOUNT_ID = sDiscountId;
                    var sresultArgs = CMSHandler.SaveOrUpdate(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeDiscount));
                    if (sresultArgs.Success)
                    {
                        var sresultArg = CMSHandler.SaveOrUpdate(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeDiscountFrequency));
                        if (sresultArg.Success)
                        {
                            var resultArg = CMSHandler.SaveOrUpdate(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeDiscountHeadByDiscountId));
                            ObjResult.sResult = (resultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                        }
                        else
                        {
                            ObjResult.sResult = Common.Messages.FailedToDeletedTryAgain;
                            ObjResult.ErrorCode = Common.ErrorCode.Ok;
                            ObjResult.Message = Common.ErrorMessage.Ok;
                        }
                    }
                    else
                    {
                        ObjResult.sResult = Common.Messages.FailedToDeletedTryAgain;
                        ObjResult.ErrorCode = Common.ErrorCode.Ok;
                        ObjResult.Message = Common.ErrorMessage.Ok;
                    }
                }
                else
                {
                    ObjResult.ErrorCode = Common.ErrorCode.BadRequest;
                    ObjResult.Message = Common.ErrorMessage.ExpectationFailed;
                }
            }
            else
            {
                ObjResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                ObjResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchFeeDiscountById(string sDiscountId)
        {
            JsonResultData objResult = new JsonResultData();
            FEE_DISCOUNT objModel = new FEE_DISCOUNT();
            if (sDiscountId != null)
            {
                string sOption = string.Empty;
                objModel.DISCOUNT_ID = sDiscountId;
                var FetchFrequency = (List<FEE_DISCOUNT>)CMSHandler.FetchData<FEE_DISCOUNT>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeDiscountById)).DataSource.Data;
                return Json(FetchFrequency);
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = Common.ErrorMessage.BadRequest;
                return Json(objResult, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult FetchFrequecyByType(string sFrequencyType, string sDiscountId)
        {
            JsonResultData objResult = new JsonResultData();
            SUP_FEE_FREQUENCY objModel = new SUP_FEE_FREQUENCY();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                objModel.TYPE = sFrequencyType;
                objModel.DISCOUNT_ID = sDiscountId;
                if (!string.IsNullOrEmpty(sDiscountId))
                {
                    var FetchFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFrequencyByTypeAndDiscountId), sAcademicyear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                    {
                        foreach (var item in FetchFrequency)
                            objResult.sResult += "<option value='" + item.FREQUENCY_ID + "' " + item.STATUS + ">" + item.FREQUENCY_NAME + "</option>";
                    }
                }
                else
                {
                    var FetchFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFrequencyByType), sAcademicyear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                    {
                        foreach (var item in FetchFrequency)
                            objResult.sResult += "<option value='" + item.FREQUENCY_ID + "'>" + item.FREQUENCY_NAME + "</option>";
                    }
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                objResult.Message = Common.ErrorMessage.RequestTimeout;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Fee disocunt Allotment
        public ActionResult FeeDiscountAllotment()
        {
            FeeDiscountAllotmentviewModel objViewModel = new FeeDiscountAllotmentviewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByAcademicYear), sAcademicyear).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    if (FetchClass != null && FetchClass.Count > 0)
                        objViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeDiscountAllotment", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeDiscountAllotment", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindFeeDiscountAllotmentByClassIdAndAcademicyear(string sClassId, string AcademicYear)
        {
            FeeDiscountAllotmentviewModel objViewModel = new FeeDiscountAllotmentviewModel();
            FEE_DISCOUNT_ALLOTMENT objModel = new FEE_DISCOUNT_ALLOTMENT();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.ACCADEMIC_YEAR = AcademicYear;
                    objModel.CLASS_ID = sClassId;
                    objViewModel.lstFeeDiscountAllotment = (List<FEE_DISCOUNT_ALLOTMENT>)CMSHandler.FetchData<FEE_DISCOUNT_ALLOTMENT>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeDiscountAllotmentByClassIdAndAcademicyear)).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindFeeDiscountAllotmentByClassIdAndAcademicyear", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindFeeDiscountAllotmentByClassIdAndAcademicyear", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchAcademicYearAndClassAndDiscount(string sClassId)
        {
            JsonResultData objResult = new JsonResultData();
            Student_Personal_Info objModel = new Student_Personal_Info();
            string Academicyear = string.Empty;
            string sClass = string.Empty;
            string sDiscount = string.Empty;
            string sFeeCategory = string.Empty;
            string sFrequency = string.Empty;
            string sHead = string.Empty;
            string sProgramme = string.Empty;
            string sStudent = string.Empty;
            string JsonData = string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                objModel.CLASS = sClassId;
                objModel.CLASS_ID = sClassId;
                var liClass = new List<cp_Classes_2017>();
                liClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByAcademicYear), sAcademicyear).DataSource.Data;
                var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                var FeeCategory = (List<Sup_Fee_Category>)CMSHandler.FetchData<Sup_Fee_Category>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeeCategory)).DataSource.Data;
                var FetchProgramme = (List<cp_Program_2017>)CMSHandler.FetchData<cp_Program_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgram), sAcademicyear).DataSource.Data;
                var FetchDiscount = (List<FEE_DISCOUNT>)CMSHandler.FetchData<FEE_DISCOUNT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDiscount)).DataSource.Data;
                var FetchFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllFrequency), sAcademicyear).DataSource.Data;
                var FetchHead = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHead), sAcademicyear).DataSource.Data;
                var FetchStudent = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByClassID), sAcademicyear).DataSource.Data;
                if (liClass != null && liClass.Count > 0)
                {
                    foreach (var item in liClass)
                        sClass += "<option value='" + item.CLASS_ID + "'>" + item.CLASS_NAME + "</option>";
                }
                if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                {
                    foreach (var item in FetchAcademicYear)
                        Academicyear += "<option value='" + item.ACADEMIC_YEAR_ID + "'>" + item.AC_YEAR + "</option>";
                }
                if (FetchDiscount != null && FetchDiscount.Count > 0)
                {
                    foreach (var item in FetchDiscount)
                        sDiscount += "<option value='" + item.DISCOUNT_ID + "'>" + item.DISCOUNT_NAME + "</option>";
                }
                if (FeeCategory != null && FeeCategory.Count > 0)
                {
                    foreach (var item in FeeCategory)
                        sFeeCategory += "<option value='" + item.FEE_CATEGORY_ID + "'>" + item.FEE_CATEGORY + "</option>";
                }
                if (FetchProgramme != null && FetchProgramme.Count > 0)
                {
                    foreach (var item in FetchProgramme)
                        sProgramme += "<option value='" + item.PROGRAMME_ID + "'>" + item.PROGRAMME_NAME + "</option>";
                }
                if (FetchFrequency != null && FetchFrequency.Count > 0)
                {
                    foreach (var item in FetchFrequency)
                        sFrequency += "<option value='" + item.FREQUENCY_ID + "'>" + item.FREQUENCY_NAME + "</option>";
                }
                if (FetchHead != null && FetchHead.Count > 0)
                {
                    foreach (var item in FetchHead)
                        sHead += "<option value='" + item.HEAD_ID + "'>" + item.HEAD + "</option>";
                }
                if (FetchStudent != null && FetchStudent.Count > 0)
                {
                    foreach (var item in FetchStudent)
                        sStudent += "<option value='" + item.STUDENT_ID + "'>" + item.FIRST_NAME + "</option>";
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                objResult.Message = Common.ErrorMessage.RequestTimeout;
            }
            var objJsonData = new FEE_DISCOUNT_ALLOTMENT() { DISCOUNT_ID = sDiscount, CLASS_ID = sClass, ACCADEMIC_YEAR = Academicyear, PROGRAMME = sProgramme, FREQUENCY = sFrequency, FEE_CATEGORY = sFeeCategory, HEAD = sHead, STUDENT_ID = sStudent };
            JsonData = JsonConvert.SerializeObject(objJsonData);
            return Json(JsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchStudentByClassId(string sClassId, string sDiscountId)
        {
            JsonResultData objResult = new JsonResultData();
            cp_Classes_2017 objModel = new cp_Classes_2017();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                objModel.CLASS_ID = sClassId;
                objModel.DISCOUNT_ID = sDiscountId;
                var FetchStudent = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByClassIdAndDiscountId), sAcademicyear).DataSource.Data;
                if (FetchStudent != null && FetchStudent.Count > 0)
                {
                    foreach (var item in FetchStudent)
                        objResult.sResult += "<option value='" + item.STUDENT_ID + "' " + item.STATUS + ">" + item.FIRST_NAME + "</option>";
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                objResult.Message = Common.ErrorMessage.RequestTimeout;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveDiscountAllotment(string sDiscountAllotment)
        {
            JsonResultData objResult = new JsonResultData();
            if (sDiscountAllotment != null)
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    FEE_DISCOUNT_ALLOTMENT objModel = new FEE_DISCOUNT_ALLOTMENT();
                    var TempList = new List<FEE_DISCOUNT_ALLOTMENT>();
                    //string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var obj = JsonConvert.DeserializeObject<List<FEE_DISCOUNT_ALLOTMENT>>(sDiscountAllotment);
                    foreach (var item in obj)
                    {
                        objModel.DISCOUNT_ID = item.DISCOUNT_ID;
                        objModel.CLASS_ID = item.CLASS_ID;
                    }
                    objModel.ACCADEMIC_YEAR = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchFeeDiscountAllotmentByDiscountId = (List<FEE_DISCOUNT_ALLOTMENT>)CMSHandler.FetchData<FEE_DISCOUNT_ALLOTMENT>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeDiscountByClassIdAndDiscountId)).DataSource.Data;
                    //this loop removes existing main heads 
                    if (FetchFeeDiscountAllotmentByDiscountId != null && FetchFeeDiscountAllotmentByDiscountId.Count() > 0)
                    {
                        if (obj != null && obj.Count > 0)
                        {
                            foreach (var list in FetchFeeDiscountAllotmentByDiscountId)
                            {
                                TempList.Add(obj.SingleOrDefault(S => S.STUDENT_ID == list.STUDENT_ID && S.DISCOUNT_ID == list.DISCOUNT_ID && S.CLASS_ID == list.CLASS_ID));
                                obj.Remove(obj.SingleOrDefault(S => S.STUDENT_ID == list.STUDENT_ID && S.DISCOUNT_ID == list.DISCOUNT_ID && S.CLASS_ID == list.CLASS_ID));
                            }
                            foreach (var item in TempList)
                            {
                                if (item != null)
                                    FetchFeeDiscountAllotmentByDiscountId.Remove(FetchFeeDiscountAllotmentByDiscountId.SingleOrDefault(S => S.DISCOUNT_ID == item.DISCOUNT_ID && S.CLASS_ID == item.CLASS_ID && S.STUDENT_ID == item.STUDENT_ID));
                            }
                            //this loop deletes records
                            foreach (var item in FetchFeeDiscountAllotmentByDiscountId)
                            {
                                item.ACCADEMIC_YEAR = objModel.ACCADEMIC_YEAR;
                                var o = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeDiscountAllotment));

                            }
                        }
                    }
                    //this loop saves records
                    foreach (var item in obj)
                    {
                        item.ACCADEMIC_YEAR = objModel.ACCADEMIC_YEAR;
                        var SaveFeeDiscountAllotment = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeDiscountAllotment));
                        if (SaveFeeDiscountAllotment.Success)
                        {
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        }
                        else
                        {
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = Common.ErrorMessage.BadRequest;
            }

            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteFeeDiscountAllotment(string sDiscountAllotmentId)
        {
            JsonResultData ObjResult = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                if (!string.IsNullOrEmpty(sDiscountAllotmentId))
                {
                    var objModel = new FEE_DISCOUNT_ALLOTMENT();
                    objModel.DISCOUNT_ALLOTMENT_ID = sDiscountAllotmentId;
                    var sresultArgs = CMSHandler.SaveOrUpdate(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeDiscountAllotment));
                    ObjResult.Message = (sresultArgs.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjResult.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    ObjResult.ErrorCode = Common.ErrorCode.BadRequest;
                    ObjResult.Message = Common.ErrorMessage.ExpectationFailed;
                }
            }
            else
            {
                ObjResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                ObjResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Fee Discount Head
        public ActionResult ListFeeDiscountHead()
        {
            FeeDiscountHeadViewModel objViewModel = new FeeDiscountHeadViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objViewModel.lstFeeDiscountHead = (List<FEE_DISCOUNT_HEAD>)CMSHandler.FetchData<FEE_DISCOUNT_HEAD>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeDiscountHead)).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListFeeDiscountHead", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListFeeDiscountHead", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchFeeFrequencyByDiscountId(string sDiscountId)
        {
            JsonResultData objResult = new JsonResultData();
            FEE_DISCOUNT_FREQUENCY objModel = new FEE_DISCOUNT_FREQUENCY();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                objModel.DISCOUNT_ID = sDiscountId;
                var FetchFeeFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFrequencyByDiscountId), sAcademicyear).DataSource.Data;
                if (FetchFeeFrequency != null && FetchFeeFrequency.Count > 0)
                {
                    objResult.sResult = "<option value='0'>--Select--</option>";
                    foreach (var item in FetchFeeFrequency)
                        objResult.sResult += "<option value='" + item.FREQUENCY_ID + "'>" + item.FREQUENCY_NAME + "</option>";
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                objResult.Message = Common.ErrorMessage.RequestTimeout;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchHeadByFrequencyIdAndDiscountId(string sFrequencyId, string sDiscountId)
        {
            JsonResultData objResult = new JsonResultData();
            FEE_DISCOUNT_FREQUENCY objModel = new FEE_DISCOUNT_FREQUENCY();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                objModel.FREQUENCY_ID = sFrequencyId;
                objModel.DISCOUNT_ID = sDiscountId;
                var FetchHead = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHeadByDiscountIdAndFrequencyId), sAcademicyear).DataSource.Data;
                if (FetchHead != null && FetchHead.Count > 0)
                {
                    foreach (var item in FetchHead)
                        objResult.sResult += "<option value='" + item.HEAD_ID + "' " + item.STATUS + ">" + item.HEAD + "</option>";
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                objResult.Message = Common.ErrorMessage.RequestTimeout;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveFeeDiscountHead(string sDiscountHead)
        {
            JsonResultData objResult = new JsonResultData();
            if (sDiscountHead != null)
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    FEE_DISCOUNT_HEAD objModel = new FEE_DISCOUNT_HEAD();
                    var TempList = new List<FEE_DISCOUNT_HEAD>();
                    //string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var obj = JsonConvert.DeserializeObject<List<FEE_DISCOUNT_HEAD>>(sDiscountHead);
                    foreach (var item in obj)
                    {
                        objModel.DISCOUNT_ID = item.DISCOUNT_ID;
                        objModel.FREQUENCY_ID = item.FREQUENCY_ID;
                    }
                    //objModel.ACCADEMIC_YEAR = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchFeeDiscountAllotmentByDiscountId = (List<FEE_DISCOUNT_HEAD>)CMSHandler.FetchData<FEE_DISCOUNT_HEAD>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeDiscountHeadByDiscountIdAndFrequencyId)).DataSource.Data;
                    //this loop removes existing main heads 
                    if (FetchFeeDiscountAllotmentByDiscountId != null && FetchFeeDiscountAllotmentByDiscountId.Count() > 0)
                    {
                        if (obj != null && obj.Count > 0)
                        {
                            foreach (var list in FetchFeeDiscountAllotmentByDiscountId)
                            {
                                TempList.Add(obj.SingleOrDefault(S => S.HEAD_ID == list.HEAD_ID && S.DISCOUNT_ID == list.DISCOUNT_ID && S.FREQUENCY_ID == list.FREQUENCY_ID));
                                obj.Remove(obj.SingleOrDefault(S => S.HEAD_ID == list.HEAD_ID && S.DISCOUNT_ID == list.DISCOUNT_ID && S.FREQUENCY_ID == list.FREQUENCY_ID));
                            }
                            foreach (var item in TempList)
                            {
                                if (item != null)
                                    FetchFeeDiscountAllotmentByDiscountId.Remove(FetchFeeDiscountAllotmentByDiscountId.SingleOrDefault(S => S.DISCOUNT_ID == item.DISCOUNT_ID && S.HEAD_ID == item.HEAD_ID && S.FREQUENCY_ID == item.FREQUENCY_ID));
                            }
                            //this loop deletes records
                            foreach (var item in FetchFeeDiscountAllotmentByDiscountId)
                            {
                                //item.ACCADEMIC_YEAR = objModel.ACCADEMIC_YEAR;
                                var o = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeDiscountHead));

                            }
                        }
                    }
                    //this loop saves records
                    foreach (var item in obj)
                    {
                        //item.ACCADEMIC_YEAR = objModel.ACCADEMIC_YEAR;
                        var SaveFeeDiscountAllotment = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeDiscountHead));
                        if (SaveFeeDiscountAllotment.Success)
                        {
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        }
                        else
                        {
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = Common.ErrorMessage.BadRequest;
            }

            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteFeeDiscountHead(string sDiscountHead)
        {
            JsonResultData ObjResult = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                if (!string.IsNullOrEmpty(sDiscountHead))
                {
                    var objModel = new FEE_DISCOUNT_HEAD();
                    objModel.DISCOUNT_HEAD_ID = sDiscountHead;
                    var sresultArgs = CMSHandler.SaveOrUpdate(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeDiscountHead));
                    ObjResult.Message = (sresultArgs.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjResult.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    ObjResult.ErrorCode = Common.ErrorCode.BadRequest;
                    ObjResult.Message = Common.ErrorMessage.ExpectationFailed;
                }
            }
            else
            {
                ObjResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                ObjResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Advance
        public ActionResult FeeAdavancePayment()
        {
            FeeAdvancePaymentViewModel objViewModel = new FeeAdvancePaymentViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    //var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByAcademicYear), sAcademicyear).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    //if (FetchClass != null && FetchClass.Count > 0)
                    //    objViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeAdavancePayment", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeAdavancePayment", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult SaveOrUpdateFeeAdvancePayment(string sAdvancePayment)
        {
            JsonResultData objResult = new JsonResultData();
            if (sAdvancePayment != null)
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var Result = JsonConvert.DeserializeObject<FEE_ADVANCE_PAYMENT>(sAdvancePayment);
                    Result.ACADEMIC_YEAR = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    AUTO_GENERATE_NUMBERS objAutoGenerateNumber = new AUTO_GENERATE_NUMBERS();
                    int ReceiptNo = 0;
                    if (string.IsNullOrEmpty(Result.ADVANCE_ID))
                    {
                        Result.PAYMENT_DATE = DateTime.Today.ToString("yyyy-MM-dd");
                        Result.USERNAME = Session[Common.SESSION_VARIABLES.USER_CODE].ToString();
                        Result.PAYMENT_MODE = Common.PaymentMode.Cash;
                        var FetchReceiptNo = (List<AUTO_GENERATE_NUMBERS>)CMSHandler.FetchData<AUTO_GENERATE_NUMBERS>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAutoGenerateNumber)).DataSource.Data;
                        if (FetchReceiptNo != null && FetchReceiptNo.Count > 0)
                        {
                            ReceiptNo = Convert.ToInt32(FetchReceiptNo.FirstOrDefault().advance_receipt_no);
                            objAutoGenerateNumber.exam_receipt_no = FetchReceiptNo.FirstOrDefault().exam_receipt_no;
                            objAutoGenerateNumber.exam_receipt_prefix = FetchReceiptNo.FirstOrDefault().exam_receipt_prefix;
                            objAutoGenerateNumber.advance_receipt_prefix = FetchReceiptNo.FirstOrDefault().advance_receipt_prefix;
                        }
                        else
                        {
                            objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                            objResult.Message = Common.ErrorMessage.ExpectationFailed;
                        }
                        objAutoGenerateNumber.advance_receipt_no = Convert.ToString(ReceiptNo + 1);
                        var UpdateReceiptNo = CMSHandler.SaveOrUpdate(objAutoGenerateNumber, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateAutoGenerateNumber));
                        objAutoGenerateNumber.advance_receipt_no = FetchReceiptNo.FirstOrDefault().advance_receipt_prefix + objAutoGenerateNumber.advance_receipt_no;
                        Result.RECEIPT_NO = objAutoGenerateNumber.advance_receipt_no;
                        var sresultArgs = CMSHandler.SaveOrUpdate(Result, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeAdvancePayment));
                        objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                    else
                    {
                        var sresultArgs = CMSHandler.SaveOrUpdate(Result, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateFeeStructure));
                        objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = Common.ErrorMessage.BadRequest;
            }

            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindFeeAdvanceByClassIdAndAcademicYear(string sClassId, string AcademicYear)
        {
            FeeAdvancePaymentViewModel objViewModel = new FeeAdvancePaymentViewModel();
            FEE_ADVANCE_PAYMENT objModel = new FEE_ADVANCE_PAYMENT();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.ACADEMIC_YEAR = AcademicYear;
                    objModel.CLASS = sClassId;
                    objViewModel.lstFeeAdvancePayment = (List<FEE_ADVANCE_PAYMENT>)CMSHandler.FetchData<FEE_ADVANCE_PAYMENT>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeAdvancePaymentByClassIdAndAcademicYear)).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindFeeAdvanceByClassIdAndAcademicYear", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindFeeAdvanceByClassIdAndAcademicYear", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult DeleteFeeAdvancePayment(string sFeeAdvanceId)
        {
            JsonResultData ObjResult = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                if (!string.IsNullOrEmpty(sFeeAdvanceId))
                {
                    var objModel = new FEE_ADVANCE_PAYMENT();
                    objModel.ADVANCE_ID = sFeeAdvanceId;
                    var sresultArgs = CMSHandler.SaveOrUpdate(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeAdvancePayment));
                    ObjResult.Message = (sresultArgs.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjResult.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    ObjResult.ErrorCode = Common.ErrorCode.BadRequest;
                    ObjResult.Message = Common.ErrorMessage.ExpectationFailed;
                }
            }
            else
            {
                ObjResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                ObjResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Hall Ticket Validation Crude
        public ActionResult ListHallTicketValidation()
        {
            string sSQL = string.Empty;
            HallTicketValidationViewModel objViewModel = new HallTicketValidationViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHallTicketValidationByACYear);
                    objViewModel.lstHallTicketValidation = (List<HALL_TICKET_VALIDATION>)CMSHandler.FetchData<HALL_TICKET_VALIDATION>(null, sSQL, sAcademicYear).DataSource.Data;
                    var studentList = (List<STUDENT_PERSONAL_INFO>)CMSHandler.FetchData<STUDENT_PERSONAL_INFO>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentbyAcYear), sAcademicYear).DataSource.Data;
                    var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    if (FetchClass != null && FetchClass.Count > 0)
                    {
                        objViewModel.liClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                    if (studentList != null && studentList.Count > 0)
                    {
                        objViewModel.StudentList = new SelectList(studentList, Common.STUDENT_INFO.STUDENT_ID, Common.STUDENT_INFO.FIRST_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "ListHallTicketValidation", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "ListHallTicketValidation", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindHallTicketValidation(string sClassId)
        {
            HallTicketValidationViewModel objViewModel = new HallTicketValidationViewModel();
            string sSQL = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var objHallTicketModel = new HALL_TICKET_VALIDATION();
                    objHallTicketModel.CLASS_ID = sClassId;
                    sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHallTicketValidation).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, objHallTicketModel.CLASS_ID);
                    var FetchHallTicket = (List<HALL_TICKET_VALIDATION>)CMSHandler.FetchData<HALL_TICKET_VALIDATION>(null, sSQL, sAcademicYear).DataSource.Data;
                    if (FetchHallTicket != null && FetchHallTicket.Count > 0)
                        objViewModel.lstHallTicketValidation = FetchHallTicket;
                    var liFetchStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(objHallTicketModel,
                         SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByClassId), sAcademicYear).DataSource.Data;
                    if (liFetchStudent != null && liFetchStudent.Count > 0)
                    {
                        objViewModel.StudentList = new SelectList(liFetchStudent, Common.STU_PERSONAL_INFO.STUDENT_ID, Common.STU_PERSONAL_INFO.FIRST_NAME);
                    }
                    //if (FetchClassWiseFeeDetails != null && FetchClassWiseFeeDetails.Count > 0)
                    //    objViewModel.liCollegeDetails = FetchCollegeDetails;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "ListHallTicketValidation", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "ListHallTicketValidation", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public JsonResult DeleteHallTicketValidation(string sValidationId)
        {
            JsonResultData ObjResult = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                if (!string.IsNullOrEmpty(sValidationId))
                {
                    var objModel = new HALL_TICKET_VALIDATION();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.HALL_TICKET_VALIDATION_ID = sValidationId;
                    var sresultArgs = CMSHandler.SaveOrUpdate(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteHalTicketValidation), sAcademicYear);
                    ObjResult.Message = (sresultArgs.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjResult.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    ObjResult.ErrorCode = Common.ErrorCode.BadRequest;
                    ObjResult.Message = Common.ErrorMessage.ExpectationFailed;
                }
            }
            else
            {
                ObjResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                ObjResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateHallTicketValidationPaidStatus(string sValidationId)
        {
            JsonResultData ObjResult = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                if (!string.IsNullOrEmpty(sValidationId))
                {
                    var objModel = new HALL_TICKET_VALIDATION();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.HALL_TICKET_VALIDATION_ID = sValidationId;
                    var sresultArgs = CMSHandler.SaveOrUpdate(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateHallTicketValidationPaidStatus), sAcademicYear);
                    ObjResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    ObjResult.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    ObjResult.ErrorCode = Common.ErrorCode.BadRequest;
                    ObjResult.Message = Common.ErrorMessage.ExpectationFailed;
                }
            }
            else
            {
                ObjResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                ObjResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOrUpdateHallTicketValidation(string sValidation)
        {
            JsonResultData objResult = new JsonResultData();
            if (sValidation != null)
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var Result = JsonConvert.DeserializeObject<HALL_TICKET_VALIDATION>(sValidation);
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (string.IsNullOrEmpty(Result.HALL_TICKET_VALIDATION_ID))
                    {
                        var sStudentIds = Result.STUDENT_ID.Split(',');
                        int nSuccessCount = 0;
                        int nFailedCount = 0;
                        foreach (var item in sStudentIds)
                        {
                            Result.STUDENT_ID = item;
                            var CheckAlredyExistOrNot = (List<HALL_TICKET_VALIDATION>)CMSHandler.FetchData<HALL_TICKET_VALIDATION>(Result, FeeSQL.GetFeeSQL(FeeSQLCommands.CheckIsHallTicketValidationExisting), sAcademicYear).DataSource.Data;
                            if (CheckAlredyExistOrNot == null)
                            {
                                var sresultArgs = CMSHandler.SaveOrUpdate(Result, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveHallTicketValidation), sAcademicYear);
                                if (sresultArgs.Success)
                                {
                                    nSuccessCount++;
                                }
                                else
                                {
                                    nFailedCount++;
                                }
                            }
                            else
                            {
                                nFailedCount++;
                            }
                        }
                        objResult.Message = "(" + nSuccessCount + ") Request Are Completed Successfully ! /n (" + nFailedCount + ") Request Are Failed Completed !.";
                    }
                    else
                    {
                        var sresultArgs = CMSHandler.SaveOrUpdate(Result, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateHallTicketValidation), sAcademicYear);
                        // objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = Common.ErrorMessage.BadRequest;
            }

            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchClassByAcademicYear(string AcademicYear)
        {
            JsonResultData objResult = new JsonResultData();
            cp_Classes_2017 objModel = new cp_Classes_2017();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicyear = AcademicYear;
                var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByAcademicYear), sAcademicyear).DataSource.Data;
                if (FetchClass != null && FetchClass.Count > 0)
                {
                    foreach (var item in FetchClass)
                        objResult.sResult += "<option value='" + item.CLASS_ID + "'>" + item.CLASS_NAME + "</option>";
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                objResult.Message = Common.ErrorMessage.RequestTimeout;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Fee Collections
        private fee_payu_response FeeCollection(fee_payu_response feePayUResponse, FormCollection payUResponse, string PayUResponse_Id, string sAcademicYear)
        {
            string[] merc_hash_vars_seq;
            string merc_hash_string = string.Empty;
            string merc_hash = string.Empty;
            string sStudentId = payUResponse["udf1"].ToString();
            string sFrequencyId = payUResponse["udf3"].ToString();
            string sFrequencyType = payUResponse["udf2"].ToString();
            string sAmount = payUResponse["amount"].ToString();
            string sSQL = string.Empty;
            sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFrequencyId).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, sFrequencyId);
            var FetchFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId }, sSQL, sAcademicYear).DataSource.Data;
            var payUPaymentInfo = (List<FEE_MERCHANT_ACCOUNT_INFO>)CMSHandler.FetchData<FEE_MERCHANT_ACCOUNT_INFO>(new FEE_MERCHANT_ACCOUNT_INFO() { API_TYPE = Common.PayUMoneyAPIsType.BaseUrl }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMerchantInfo)).DataSource.Data;
            if (FetchFeeStudentAccount != null && FetchFeeStudentAccount.Count > 0)
            {
                FetchFeeStudentAccount.RemoveAll(r => r.BALANCE == "0");
                if (payUPaymentInfo != null && payUPaymentInfo.Count > 0)
                {
                    merc_hash_vars_seq = payUPaymentInfo.FirstOrDefault().HASH_SEQUENCE.Split('|');
                    Array.Reverse(merc_hash_vars_seq);
                    if (payUResponse.AllKeys.Contains("additionalCharges"))
                    {
                        merc_hash_string = payUResponse["additionalCharges"].ToString() + "|";
                    }
                    merc_hash_string += payUPaymentInfo.FirstOrDefault().SALT + "|" + payUResponse["status"].ToString();
                    foreach (var merc_hash_var in merc_hash_vars_seq)
                    {
                        merc_hash_string += "|";
                        merc_hash_string = merc_hash_string + (payUResponse[merc_hash_var] != null ? payUResponse[merc_hash_var] : "");
                    }
                    merc_hash = CommonMethods.Generatehash512(merc_hash_string).ToLower();
                    // string s = payUResponse["hash"];
                    if (merc_hash == payUResponse["hash"])
                    {
                        int ReceiptNo = 0;
                        string Prefix = "00";
                        string TransactionId = string.Empty;
                        FEE_STUDENT_ACCOUNT objStudentAccountModel = new FEE_STUDENT_ACCOUNT();
                        FEE_TRANSACTION objFeeTransactionModel = new FEE_TRANSACTION();
                        AUTO_GENERATE_NUMBERS objAutoGenerateNumber = new AUTO_GENERATE_NUMBERS();
                        if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                        {
                            if (FetchFeeStudentAccount != null && FetchFeeStudentAccount.Count > 0)
                            {
                                if (decimal.Parse(sAmount) == FetchFeeStudentAccount.Sum(x => decimal.Parse(x.BALANCE)))
                                {
                                    var GroupFeeStudentAccount = FetchFeeStudentAccount.GroupBy(x => x.FREQUENCY_ID);
                                    foreach (var FrequencyItem in GroupFeeStudentAccount)
                                    {
                                        var c = GroupFeeStudentAccount.Count();
                                        sFrequencyId = FrequencyItem.FirstOrDefault().FREQUENCY_ID;
                                        var feeTransactions = FetchFeeStudentAccount.Where(x => x.FREQUENCY_ID == sFrequencyId);
                                        decimal balance = 0;
                                        balance = feeTransactions.Sum(x => decimal.Parse(x.BALANCE));
                                        if (balance > 0)
                                        {
                                            objFeeTransactionModel.STUDENT_ID = sStudentId;
                                            objFeeTransactionModel.CLASS = string.Empty;
                                            objFeeTransactionModel.FREQUENCY = sFrequencyId;
                                            objFeeTransactionModel.FREQUENCY_TO = sFrequencyId;
                                            objFeeTransactionModel.PAYMENT_DATE = DateTime.Today.ToString("yyyy-MM-dd");
                                            objFeeTransactionModel.USERNAME = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                                            objFeeTransactionModel.COLLECTED = feeTransactions.Sum(x => decimal.Parse(x.BALANCE)).ToString();
                                            var FetchReceiptNo = (List<AUTO_GENERATE_NUMBERS>)CMSHandler.FetchData<AUTO_GENERATE_NUMBERS>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAutoGenerateNumber)).DataSource.Data;
                                            if (FetchReceiptNo != null && FetchReceiptNo.Count > 0)
                                                ReceiptNo = Convert.ToInt32(FetchReceiptNo.FirstOrDefault().exam_receipt_no);
                                            else
                                            {
                                                // objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                                //   objResult.Message = Common.ErrorMessage.ExpectationFailed;
                                            }
                                            objAutoGenerateNumber.exam_receipt_no = Convert.ToString(ReceiptNo + 1);
                                            objAutoGenerateNumber.exam_receipt_no = Prefix + objAutoGenerateNumber.exam_receipt_no;
                                            var UpdateReceiptNo = CMSHandler.SaveOrUpdate(objAutoGenerateNumber, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateAutoGenerateNumber));
                                            if (UpdateReceiptNo.Success)
                                            {
                                                objFeeTransactionModel.PAYMENT_MODE = Common.PaymentMode.Online;
                                                objFeeTransactionModel.RECEIPT_NO = objAutoGenerateNumber.exam_receipt_no;
                                                feePayUResponse.RECEIPT_NO = objAutoGenerateNumber.exam_receipt_no;
                                                objFeeTransactionModel.PayUResponse_Id = PayUResponse_Id;
                                                var SaveFeeTransaction = CMSHandler.SaveOrUpdate(objFeeTransactionModel, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeTransaction), sAcademicYear, true);
                                                if (SaveFeeTransaction.Success)
                                                {
                                                    TransactionId = SaveFeeTransaction.RowUniqueId.ToString();
                                                    //   feePayUResponse.txnid = TransactionId;
                                                    decimal dAmount = feeTransactions.Sum(x => decimal.Parse(x.BALANCE));
                                                    decimal dCredit = 0;
                                                    foreach (var item in feeTransactions)
                                                    {
                                                        if (dAmount > 0 && Convert.ToDecimal(item.BALANCE) > 0)
                                                        {
                                                            dCredit = Convert.ToDecimal(item.CREDIT);
                                                            item.DEBIT = (dAmount >= dCredit) ? item.CREDIT : (dCredit - dAmount).ToString();
                                                            dAmount = dAmount - dCredit;
                                                        }
                                                        else
                                                        {
                                                            continue;
                                                        }
                                                        item.FREQUENCY = item.FREQUENCY_ID;
                                                        item.TRANSACTION_ID = TransactionId;
                                                        item.PAID_AMOUNT = item.DEBIT;
                                                        item.RECEIPT_NO = objFeeTransactionModel.RECEIPT_NO;
                                                        item.HEAD = item.HEAD_ID;

                                                        var SaveFeeCollection = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeCollection));
                                                        if (SaveFeeCollection.Success)
                                                        {
                                                            var SaveStudentAccount = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeStudentAccount), sAcademicYear);
                                                            if (SaveStudentAccount.Success)
                                                            {

                                                            }
                                                            else
                                                            {
                                                                //objResult.Message = Common.Messages.FailedToSaveRecords;
                                                            }
                                                        }
                                                    }
                                                    payUResponse.Clear();
                                                }
                                                else
                                                {
                                                    // objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                                    //  objResult.Message = Common.ErrorMessage.ExpectationFailed;
                                                }
                                            }
                                            //  var ObjHostelModel = new HOSTEL_REGISTRATION();
                                            //   var FetchHostelInfo = (List<HOSTEL_REGISTRATION>)CMSHandler.FetchData<HOSTEL_REGISTRATION>(new HOSTEL_REGISTRATION() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.FetchHostelInfoByStudentId), sAcademicYear).DataSource.Data;
                                            //   if (FetchHostelInfo != null && FetchHostelInfo.Count > 0)
                                            //   {
                                            //       ObjHostelModel.RENEWAL_REGISTRATION_ID = FetchHostelInfo.FirstOrDefault().RENEWAL_REGISTRATION_ID;
                                            //       var UpdateSubmitted = CMSHandler.SaveOrUpdate(ObjHostelModel, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.UpdateFeePaidForHostel));
                                            //   }
                                        }
                                        else
                                        {
                                            feePayUResponse.status = "Pending";
                                            feePayUResponse.error_Message = "Please note Transaction Id and enquire with admin !!";
                                        }


                                    }
                                }
                                else
                                {
                                    feePayUResponse.status = "Pending";
                                    feePayUResponse.error_Message = "Please note Transaction Id and enquire with admin";
                                }
                            }
                            else
                            {
                                feePayUResponse.status = "Pending";
                                feePayUResponse.error_Message = "Please note Transaction Id and enquire with admin";
                            }
                        }
                        else
                        {
                            feePayUResponse.status = Common.ErrorCode.RequestTimeout;
                            feePayUResponse.error_Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                        }

                    }
                    else
                    {
                        feePayUResponse.status = "Pending";
                        feePayUResponse.error_Message = "Please note Transaction Id and enquire with admin !!";
                    }
                }
                else
                {
                    feePayUResponse.status = "Pending";
                    feePayUResponse.error_Message = "Please note Transaction Id and enquire with admin !!";
                }
            }
            else
            {
                feePayUResponse.status = "Pending";
                feePayUResponse.error_Message = "Please note Transaction Id and enquire with admin !!";
            }


            return feePayUResponse;
        }
        #endregion

        #region Payment Status
        public ActionResult PaymentStatus(string PayUResponse_Id, string sFeeRootId)
        {
            PayUResponse_Id = CommonMethods.DecryptString(PayUResponse_Id);
            sFeeRootId = CommonMethods.DecryptString(sFeeRootId);
            List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                if (!string.IsNullOrEmpty(PayUResponse_Id))
                {
                    if (sFeeRootId == Common.FeeRoot.CollegeFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { PAYU_RESPONSE_ID = PayUResponse_Id }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStatusByPayUResponseId), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                    else if (sFeeRootId == Common.FeeRoot.MessFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { PAYU_RESPONSE_ID = PayUResponse_Id }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStatusForHostelAndMessByPayUResponseId), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                    else if (sFeeRootId == Common.FeeRoot.HostelFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { PAYU_RESPONSE_ID = PayUResponse_Id }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStatusForHostelAndMessByPayUResponseId), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                    else if (sFeeRootId == Common.FeeRoot.HostelAndMess)
                    {
                    }
                }
            }
            return View(studentAcountList);
        }
        #endregion

        #region  Fee Receipt
        public ActionResult GetHostelFeeReceipt()
        {
            //FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            //JsonResultData objResult = new JsonResultData();
            //var FetchCollegeDetails = new List<CollegeDetails>();
            //var BalanceInfo = new List<FEE_TRANSACTION>();
            //var FeeStudentAccount = new List<FEE_STUDENT_ACCOUNT>();
            //try
            //{
            //    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            //    {
            //        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            //        var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT();
            //        FetchCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
            //        var FetchStudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(new Student_Personal_Info() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByUserId), sAcademicYear).DataSource.Data;
            //        var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.HostelFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
            //        var FetchTotalBalance = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(new FEE_TRANSACTION() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY = FetchFrequency.FirstOrDefault().FREQUENCY_ID }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTotalAndDebitByUserIdAndFrequencyId), sAcademicYear).DataSource.Data;
            //        if (FetchTotalBalance != null && FetchTotalBalance.Count > 0)
            //        {
            //            foreach (var dataitem in FetchTotalBalance)
            //            {
            //                if (FetchTotalBalance != null && FetchTotalBalance.Count > 0)
            //                    Number = Convert.ToInt32(FetchTotalBalance.FirstOrDefault().COLLECTED);
            //                BalanceInfo.Add(dataitem);
            //                var FetchFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { TRANSACTION_ID = dataitem.TRANSACTION_ID, FREQUENCY_ID = FetchFrequency.FirstOrDefault().FREQUENCY_ID }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByTransactionIdAndFrequencyId), sAcademicYear).DataSource.Data;
            //                if (FetchFeeStudentAccount != null && FetchFeeStudentAccount.Count > 0)
            //                {
            //                    foreach (var listitem in FetchFeeStudentAccount)
            //                        FeeStudentAccount.Add(listitem);
            //                }
            //            }
            //        }
            //        if (FetchCollegeDetails != null && FetchCollegeDetails.Count > 0)
            //            objViewModel.liCollegeDetails = FetchCollegeDetails;
            //        if (FetchStudentInfo != null && FetchStudentInfo.Count > 0)
            //            objViewModel.lstStudentPersonalInfo = FetchStudentInfo;
            //        if (BalanceInfo != null && BalanceInfo.Count > 0)
            //            objViewModel.liFeeTransaction = BalanceInfo;
            //        if (FeeStudentAccount != null && FeeStudentAccount.Count > 0)
            //            objViewModel.liFeeStudentAccount = FeeStudentAccount;
            //    }
            //    else
            //    {
            //        objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
            //        objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            //    }
            //}
            //catch (Exception Ex)
            //{
            //    using (ErrorLog objHandler = new ErrorLog())
            //    {
            //        objHandler.WriteError("Fee", "GetHostelFeeReceipt", "Err MSg: " + Ex.Message, "Query is empty!");
            //        objHandler.WriteError("Fee", "GetHostelFeeReceipt", Ex.Message);
            //    }
            //}

            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.HostelFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                    objViewModel.liFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FEE_ROOT_ID = Common.FeeRoot.HostelFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMessCollectionList).Replace("?FREQUENCY", FetchFrequency.FirstOrDefault().FREQUENCY_ID), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "MessFeeReport", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "MessFeeReport", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(objViewModel);
        }

        public ActionResult SemesterFeeReceipt()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT();
                    var BalanceInfo = new List<FEE_TRANSACTION>();
                    var FeeStudentAccount = new List<FEE_STUDENT_ACCOUNT>();
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    var FetchStudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(new Student_Personal_Info() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByUserId), sAcademicYear).DataSource.Data;
                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.SemesterFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                    objViewModel.liFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FEE_ROOT_ID = Common.FeeRoot.CollegeFee },
                        FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTotalAndDebitByUserIdAndFrequencyId).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, FetchFrequency.FirstOrDefault().FREQUENCY_ID), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "SemesterFeeReceipt", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "SemesterFeeReceipt", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(objViewModel);
        }

        public ActionResult MessFeeReport()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.MessFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                    objViewModel.liFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY = FetchFrequency.FirstOrDefault().FREQUENCY_ID, FEE_ROOT_ID = Common.FeeRoot.MessFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMessCollectionList), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "MessFeeReport", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "MessFeeReport", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(objViewModel);
        }

        public ActionResult ExamFeeReceipt()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT();
                    var BalanceInfo = new List<FEE_TRANSACTION>();
                    var FeeStudentAccount = new List<FEE_STUDENT_ACCOUNT>();
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    var FetchStudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(new Student_Personal_Info() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByUserId), sAcademicYear).DataSource.Data;
                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.ExamFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                    objViewModel.liFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FEE_ROOT_ID = Common.FeeRoot.CollegeFee },
                        FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTotalAndDebitByUserIdAndFrequencyId).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, FetchFrequency.FirstOrDefault().FREQUENCY_ID), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "SemesterFeeReceipt", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "SemesterFeeReceipt", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(objViewModel);
        }
        public ActionResult ConvocationReceipt()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT();
                    var BalanceInfo = new List<FEE_TRANSACTION>();
                    var FeeStudentAccount = new List<FEE_STUDENT_ACCOUNT>();
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    var FetchStudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(new Student_Personal_Info() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByUserId), sAcademicYear).DataSource.Data;
                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.ConvocationFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                    objViewModel.liFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FEE_ROOT_ID = Common.FeeRoot.CollegeFee },
                        FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTotalAndDebitByUserIdAndFrequencyId).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, FetchFrequency.FirstOrDefault().FREQUENCY_ID), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "SemesterFeeReceipt", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "SemesterFeeReceipt", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(objViewModel);
        }

        public ActionResult SupplementryReceipt()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT();
                    var BalanceInfo = new List<FEE_TRANSACTION>();
                    var FeeStudentAccount = new List<FEE_STUDENT_ACCOUNT>();
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    var FetchStudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(new Student_Personal_Info() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByUserId), sAcademicYear).DataSource.Data;
                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.Supplementary }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                    objViewModel.liFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FEE_ROOT_ID = Common.FeeRoot.CollegeFee },
                        FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTotalAndDebitByUserIdAndFrequencyId).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, FetchFrequency.FirstOrDefault().FREQUENCY_ID), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "SemesterFeeReceipt", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "SemesterFeeReceipt", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(objViewModel);
        }
        #endregion

        #region Fee Frequency Fee Mode
        public ActionResult ListFeeFrequencyFeeMode()
        {
            FeeFrequencyFeeModeViewModel MainHeadView = new FeeFrequencyFeeModeViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    MainHeadView.FeeFrequencyFeeModeList = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeFrequencyFeeMode), sAcademicYear).DataSource.Data;
                    MainHeadView.SupFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFrequency), sAcademicYear).DataSource.Data;
                    MainHeadView.SupFeeMode = (List<sup_frequency_type>)CMSHandler.FetchData<sup_frequency_type>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFrequencytype)).DataSource.Data;
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "ListFeeFrequencyFeeMode", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "ListFeeFrequencyFeeMode", Ex.Message);
                }
            }
            return View(MainHeadView);
        }
        public JsonResult LoadFeeMode(string sFrequencyId)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    FeeMainHead objFeeMainHead = new FeeMainHead();
                    objFeeMainHead.FREQUENCY_ID = sFrequencyId;
                    var FeeMode = (List<sup_frequency_type>)CMSHandler.FetchData<sup_frequency_type>(objFeeMainHead, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeModeByFrequencyId), sAcademicYear).DataSource.Data;
                    if (FeeMode != null)
                    {
                        foreach (var item in FeeMode)
                            objResult.sResult += "<option value='" + item.FREQUENCY_TYPE_ID + "' " + item.STATUS + ">" + item.FREQUENCY_TYPE + "</option>";
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "LoadFeeMode", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "LoadFeeMode", Ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveFeeFrequencyMode(string sJsonFeeFrequencyFeeMode)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    FEE_FREQUENCY_FEE_MODE objModel = new FEE_FREQUENCY_FEE_MODE();
                    var FetchFeeFrequencyFeeModeById = new List<FEE_FREQUENCY_FEE_MODE>();
                    var TempList = new List<FEE_FREQUENCY_FEE_MODE>();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var obj = JsonConvert.DeserializeObject<List<FEE_FREQUENCY_FEE_MODE>>(sJsonFeeFrequencyFeeMode);
                    //foreach (var item in obj)
                    //{
                    //    objModel.FEE_MODE = item.FEE_MODE;
                    //    objModel.FREQUENCY_ID = item.FREQUENCY_ID;

                    //}
                    if (obj != null && obj.Count > 0)
                    {
                        objModel.FREQUENCY_ID = obj.FirstOrDefault().FREQUENCY_ID;
                        FetchFeeFrequencyFeeModeById = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeFrequencyFeeModeByFrequencyId), sAcademicYear).DataSource.Data;
                    }

                    //this loop removes existing main heads 
                    if (FetchFeeFrequencyFeeModeById != null && FetchFeeFrequencyFeeModeById.Count() > 0)
                    {
                        if (obj != null && obj.Count > 0)
                        {
                            foreach (var list in FetchFeeFrequencyFeeModeById)
                            {

                                //TempList.Add(obj.SingleOrDefault(S => S.FREQUENCY_ID == list.FREQUENCY_ID && S.FEE_MODE == list.FEE_MODE));
                                //obj.Remove(obj.SingleOrDefault(S => S.FREQUENCY_ID == list.FREQUENCY_ID && S.FEE_MODE == list.FEE_MODE));
                                obj.RemoveAll(x => x.FREQUENCY_ID == list.FREQUENCY_ID && x.FEE_MODE == list.FEE_MODE);
                            }
                            //foreach (var item in TempList)
                            //{
                            //    if (item != null)
                            //        FetchFeeFrequencyFeeModeById.Remove(FetchFeeFrequencyFeeModeById.SingleOrDefault(S => S.FREQUENCY_ID == item.FREQUENCY_ID && S.FEE_MODE == item.FEE_MODE));
                            //}
                            //this loop deletes records
                            //foreach (var item in FetchFeeMainHeadById)
                            //{
                            //    item.ACADEMIC_YEAR = sAcademicYear;
                            //    var o = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteFeeMainHeadByMainHeadIdAndHeadId));
                            //    if (o.Success)
                            //        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                            //}
                        }
                    }
                    if (obj.Count > 0)
                    {
                        //this loop saves records
                        foreach (var item in obj)
                        {
                            item.ACADEMIC_YEAR = sAcademicYear;
                            var SaveMainHead = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeFrequencyFeeMode));
                            if (SaveMainHead.Success)
                                objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                            else
                                objResult.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }
                    else
                        objResult.Message = "No Changes Made !";
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "SaveFeeFrequencyMode", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "SaveFeeFrequencyMode", Ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create Fee Student Account Create
        public ActionResult FeeStudentAccountCreation()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear), sAcademicYear).DataSource.Data;
                    //List<SUP_FEE_FREQUENCY> liFrequency = new List<SUP_FEE_FREQUENCY>();
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    var FetchProgrammeGroup = (List<SUP_APPTYPE_PROG_GROUPS>)CMSHandler.FetchData<SUP_APPTYPE_PROG_GROUPS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                    if (FetchProgrammeGroup != null && FetchProgrammeGroup.Count > 0)
                        objViewModel.ProgrammeGroupList = new SelectList(FetchProgrammeGroup, Common.SUP_APPTYPE_PROG_GROUPS.PROGRAMME_GROUP_ID, Common.SUP_APPTYPE_PROG_GROUPS.PROGRAMME_NAME);
                    if (FetchClassYear != null && FetchClassYear.Count > 0)
                        objViewModel.ClassYearList = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.CP_CLASSES_2017.CLASS_YEAR);
                    //liFrequency.Add(new SUP_FEE_FREQUENCY() { FREQUENCY_ID = "0", FREQUENCY_NAME = "---Select---" });
                    //objViewModel.FrequencyList = new SelectList(liFrequency, Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, Common.SUP_FEE_FREQUENCY.FREQUENCY_NAME);
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindStudentsByAcademicYearAndClassId(string sProgrammeGroupId, string sClassYearId, string Academicyear)
        {
            //string sFrequency,
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;
                    objModel.PROGRAMME = sProgrammeGroupId;
                    objModel.CLASS_YEAR_ID = sClassYearId;
                    objViewModel.lstStudentPersonalInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentListByClassIdForFeeStudentAccount), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindStudentsByAcademicYearAndClassId", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindStudentsByAcademicYearAndClassId", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult SaveOrUpdateFeeStudentAccount(string sFeeStructure, string sProgrammeGroupId, string sFrequency, string sClassYearId, string Academicyear)
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            string sSQL = string.Empty;
            try
            {
                if (sFeeStructure != null)
                {
                    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                    {
                        var sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                        var Result = serializer.Deserialize<JsonFeeStudentAccountList>(sFeeStructure);
                        objModel.PROGRAMME = sProgrammeGroupId;
                        objModel.CLASS_YEAR_ID = sClassYearId;
                        objModel.FREQUENCY = sFrequency;
                        objModel.FREQUENCY_ID = sFrequency;
                        sAcademicyear = Academicyear;
                        var FetchFeeStructure = (List<FEE_STRUCTURE>)CMSHandler.FetchData<FEE_STRUCTURE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStructureByAcademicYearAndFrequencyId), sAcademicyear).DataSource.Data;
                        if (FetchFeeStructure != null && FetchFeeStructure.Count > 0)
                        {
                            foreach (var item in Result.StudentAccountList)
                            {
                                objModel.STUDENT_ID = item.STUDENT_ID;
                                var FetchFeeStudentAccountExisting = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFrequencyId), sAcademicyear).DataSource.Data;
                                if (FetchFeeStudentAccountExisting != null && FetchFeeStudentAccountExisting.Count > 0)
                                {
                                    //var sresultArgs = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateFeeStructure));
                                    //objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    //objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                    objResult.Message = "Account Already Exist !";
                                }
                                else
                                {
                                    sSQL = string.Empty;
                                    sSQL = @"INSERT INTO FEE_STUDENT_ACCOUNT
                                (
                                STUDENT_ID,
                                ACADEMIC_YEAR,
                                FREQUENCY_ID,
                                HEAD,
                                CREDIT,FEE_MAIN_HEAD_ID)
                                VALUES
                                 ";
                                    foreach (var list in FetchFeeStructure)
                                    {
                                        objModel.ACADEMIC_YEAR = sAcademicyear;
                                        objModel.CREDIT = list.AMOUNT;
                                        objModel.HEAD = list.HEAD_ID;
                                        sSQL += "('" + objModel.STUDENT_ID + "','" + sAcademicyear + "','" + objModel.FREQUENCY_ID + "','" + list.HEAD_ID + "','" + list.AMOUNT + "','" + list.FEE_MAIN_HEAD_ID + "'),";

                                    }
                                    sSQL = sSQL.TrimEnd(',');
                                    var sresultArgs = CMSHandler.SaveOrUpdate(objModel, sSQL, sAcademicyear);
                                    objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                    if (sresultArgs.Success)
                                    {
                                        var MessageSetting = (List<SMS_TEMPLATE_MODULE>)CMSHandler.FetchData<SMS_TEMPLATE_MODULE>(new SMS_TEMPLATE_MODULE { SMS_TEMPLATE_ID = Common.SMS_TEMPLATE.FeeStructureSMS }, SQL.Communication.CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSMSTemplateActiveByTemplateId)).DataSource.Data;
                                        if (MessageSetting != null && MessageSetting.Count > 0)
                                        {
                                            string sFeeMessage = string.Empty;
                                            string sCount = string.Empty;
                                            string sCreditCount = string.Empty;
                                            var MessageContent = (List<SMS_USERDEFINED_TEMPLATE>)CMSHandler.FetchData<SMS_USERDEFINED_TEMPLATE>(new SMS_USERDEFINED_TEMPLATE() { TEMPLATE_ID = Common.SMS_TEMPLATE.DailyAbsenteesSMS }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchsmsTemplateByTemplateId), sAcademicyear).DataSource.Data;
                                            var StudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(new StuPersonalInfo() { STUDENT_ID = item.STUDENT_ID }, SQL.Communication.CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchStudentDetailsForSMSByStudentId), sAcademicyear).DataSource.Data;
                                            sFeeMessage = MessageContent.FirstOrDefault().TEMPLATE_TEXT.Replace(Common.Delimiter.AT + Common.STU_PERSONAL_INFO.FIRST_NAME, StudentInfo.FirstOrDefault().FIRST_NAME).Replace(Common.Delimiter.AT + Common.CP_CLASSES_2017.CLASS_NAME, StudentInfo.FirstOrDefault().CLASS_NAME);
                                            string sSmsMode = Common.MessageType.FEE;
                                            using (CommunicationController objComcontroler = new CommunicationController())
                                            {
                                                sCount = objComcontroler.ValidateCreditCount((sFeeMessage.Length));
                                            }
                                            sCreditCount = sCount;
                                            if (!string.IsNullOrEmpty(item.STUDENT_ID))
                                            {
                                                using (CommunicationController objComcontroller = new CommunicationController())
                                                {
                                                    objComcontroller.liSMS.Add(new MessageResult.sms() { to = StudentInfo.FirstOrDefault().MOBILE, message = sFeeMessage, custom = item.STUDENT_ID, custom1 = StudentInfo.FirstOrDefault().CLASS_ID, CreditCount = sCount });
                                                    objComcontroller.SendMessage();
                                                }

                                            }

                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            objResult.ErrorCode = Common.ErrorCode.NoContent;
                            objResult.Message = "Please Define The Fee Structure For This Class...!!!";
                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                        objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                    objResult.Message = Common.ErrorMessage.BadRequest;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOrUpdateHostelAndMessFeeStudentAccount(string sFeeStructure, string sFrequency, string sFeeMainHeadId, string Academicyear)
        {
            //pending
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            string sSQL = string.Empty;
            try
            {
                if (sFeeStructure != null)
                {
                    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                    {
                        var sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                        var Result = serializer.Deserialize<JsonFeeStudentAccountList>(sFeeStructure);
                        objModel.FREQUENCY = sFrequency;
                        objModel.FREQUENCY_ID = sFrequency;
                        sAcademicyear = Academicyear;
                        objModel.ACADEMIC_YEAR = Academicyear;
                        objModel.MAIN_HEAD_ID = sFeeMainHeadId;
                        sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.BindHostelAndMessStructureForAccountCreation).Replace(Common.Delimiter.QUS + Common.FEE_MAIN_HEADS.MAIN_HEAD_ID, sFeeMainHeadId);
                        var FetchFeeStructure = (List<FEE_STRUCTURE>)CMSHandler.FetchData<FEE_STRUCTURE>(objModel, sSQL, sAcademicyear).DataSource.Data;
                        if (FetchFeeStructure != null && FetchFeeStructure.Count > 0)
                        {
                            foreach (var item in Result.StudentAccountList)
                            {
                                objModel.STUDENT_ID = item.STUDENT_ID;
                                var FetchFeeStudentAccountExisting = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.CheckIsExistingAccountForHostelAndMess), sAcademicyear).DataSource.Data;
                                if (FetchFeeStudentAccountExisting != null && FetchFeeStudentAccountExisting.Count > 0)
                                {
                                    //var sresultArgs = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateFeeStructure));
                                    //objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    //objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                    objResult.Message = "Account Already Exist !";
                                }
                                else
                                {
                                    sSQL = string.Empty;
                                    sSQL = @"INSERT INTO FEE_STUDENT_ACCOUNT
                                (
                                STUDENT_ID,
                                ACADEMIC_YEAR,
                                FREQUENCY_ID,
                                HEAD,
                                CREDIT,FEE_MAIN_HEAD_ID)
                                VALUES
                                 ";
                                    foreach (var list in FetchFeeStructure)
                                    {
                                        objModel.ACADEMIC_YEAR = sAcademicyear;
                                        objModel.CREDIT = list.AMOUNT;
                                        objModel.HEAD = list.HEAD_ID;
                                        sSQL += "('" + objModel.STUDENT_ID + "','" + sAcademicyear + "','" + objModel.FREQUENCY_ID + "','" + list.HEAD_ID + "','" + list.AMOUNT + "','" + list.FEE_MAIN_HEAD_ID + "'),";

                                    }
                                    sSQL = sSQL.TrimEnd(',');
                                    var sresultArgs = CMSHandler.SaveOrUpdate(objModel, sSQL, sAcademicyear);
                                    objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                    if (sresultArgs.Success)
                                    {
                                        //var smsStuinfo = (List<HOSTEL_STUDENT_INFO>)CMSHandler.FetchData<HOSTEL_STUDENT_INFO>(objModel, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.FetchStudentInfoForSMS), sAcademicyear).DataSource.Data;                                       
                                        //var MessageContent = (List<SMS_USERDEFINED_TEMPLATE>)CMSHandler.FetchData<SMS_USERDEFINED_TEMPLATE>(new SMS_USERDEFINED_TEMPLATE() { TEMPLATE_ID = Common.SMS_TEMPLATE.HostelSelection }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchsmsTemplateByTemplateId), sAcademicyear).DataSource.Data;
                                        //string sContent = MessageContent.FirstOrDefault().TEMPLATE_TEXT.Replace(Common.Delimiter.AT + Common.STUDENT_INFO.FIRST_NAME, smsStuinfo.FirstOrDefault().FIRST_NAME).Replace(Common.Delimiter.AT + Common.SUP_HOSTEL.HOSTEL_NAME, smsStuinfo.FirstOrDefault().HOSTEL_NAME);                                      
                                        //using (CommunicationController com = new CommunicationController())
                                        //{
                                        //    com.SendEmailAndTextMessage(sContent, smsStuinfo.FirstOrDefault().STU_MOBILENO);
                                        //}
                                    }
                                }
                            }
                        }
                        else
                        {
                            objResult.ErrorCode = Common.ErrorCode.NoContent;
                            objResult.Message = "Please Define The Fee Structure For This Class...!!!";
                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                        objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                    objResult.Message = Common.ErrorMessage.BadRequest;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FetchSelectedStudentFeeAccount(string sStudentId, string Academicyear, string sFrequencyId)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            FEE_STUDENT_ACCOUNT objModel = new FEE_STUDENT_ACCOUNT();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;
                    objModel.STUDENT_ID = sStudentId;
                    objModel.FREQUENCY_ID = sFrequencyId;
                    objViewModel.liFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFrequencyId), sAcademicYear).DataSource.Data;
                    objViewModel.lstStudentPersonalInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByUserId), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchSelectedStudentFeeAccount", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchSelectedStudentFeeAccount", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult FetchSelectedStudentFeeAccountForHostelandMess(string sStudentId, string Academicyear, string sFrequencyId, string sFeeMainHeadId)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            FEE_STUDENT_ACCOUNT objModel = new FEE_STUDENT_ACCOUNT();
            string sSQL = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;
                    objModel.STUDENT_ID = sStudentId;
                    objModel.FREQUENCY_ID = sFrequencyId;
                    objModel.MAIN_HEAD_ID = sFeeMainHeadId;
                    objModel.ACADEMIC_YEAR = Academicyear;
                    sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.CheckIsExistingAccountForHostelAndMess).Replace(Common.Delimiter.QUS + Common.FEE_MAIN_HEADS.MAIN_HEAD_ID, sFeeMainHeadId);
                    objViewModel.liFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objModel, sSQL, sAcademicYear).DataSource.Data;
                    objViewModel.lstStudentPersonalInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByUserId), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchSelectedStudentFeeAccount", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchSelectedStudentFeeAccount", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult UpdateHostelAndMessFeeStudentAccount(string sFeeStudentAccount)
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            string sSQL = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = serializer.Deserialize<JsonFeeStudentAccountList>(sFeeStudentAccount);
                    if (Result != null)
                    {
                        foreach (var item in Result.StudentAccountList)
                        {
                            if (!string.IsNullOrEmpty(item.F_STUDENT_AC_ID))
                            {
                                var sresultArgs = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateFeeStudenAccountForHostelAndMess), sAcademicYear);
                                objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("UpdateHostelAndMessFeeStudentAccount", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("UpdateHostelAndMessFeeStudentAccount", "Fee", ex.Message);
                    }
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region College Fee Payment By RazorPay
        [UserRights("Student")]
        public ActionResult FeeBalanceList()
        {
            FeeTransactionViewModel liFeeTransactions = new FeeTransactionViewModel();

            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {

                    liFeeTransactions.liFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() },
                        ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchAllFeeTransaction), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                    //if (Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() == "17")
                    //{
                    //    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    //    var Academicyear = (List<AcademicBatches>)CMSHandler.FetchData<AcademicBatches>(new GET_PROGRAMME_AND_BATCH() { STUDENT_ID = sUserId }, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchAcademicYearstudentId)).DataSource.Data;
                    //    var LiSupExamFee = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), ACADEMIC_YEAR = Academicyear.FirstOrDefault().ACADEMIC_YEAR_ID },
                    //        ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchSupFeeTransaction), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                    //    liFeeTransactions.liFeeStudentAccount = LiSupExamFee;
                    //}


                }
                catch (Exception e)
                {
                    string sErrorMessage = e.Message;
                }

            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }

            return View(liFeeTransactions);
        }
        public ActionResult FeeReceipt(string sFrequencytypeId, string sFrequencyId, string sAcademicYear)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT();
            var objFeeReceipt = new FEE_REPORT();
            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {

                //  string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                try
                {
                    objFeeReceipt.STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    objFeeReceipt.FREQUENCY_TYPE = sFrequencytypeId;
                    objFeeReceipt.FREQUENCY_ID = sFrequencyId;
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    objViewModel.liFeeReport = (List<FEE_REPORT>)CMSHandler.FetchData<FEE_REPORT>(objFeeReceipt, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentTransactionByFrequencytype), sAcademicYear).DataSource.Data;
                    if (objViewModel.liFeeReport != null && objViewModel.liFeeReport.Count > 0)
                    {
                        int COUNT = 0;
                        foreach (var item in objViewModel.liFeeReport)
                        {

                            objFeeStudentAccountModel.TRANSACTION_ID = item.TRANSACTION_ID;
                            objFeeStudentAccountModel.FREQUENCY_ID = item.FREQUENCY_ID;
                            objFeeStudentAccountModel.ACADEMIC_YEAR = sAcademicYear;
                            var LstfeeTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeReceiptByTransactionId), sAcademicYear).DataSource.Data;
                            if (LstfeeTransaction != null && LstfeeTransaction.Count > 0)
                            {
                                if (COUNT == 0)
                                {
                                    objViewModel.liFeeTransaction = LstfeeTransaction;
                                }
                                else
                                {
                                    objViewModel.liFeeTransaction = objViewModel.liFeeTransaction.Concat(LstfeeTransaction).ToList();
                                }
                                COUNT++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AdmissionController", "HostelFeeReceipt", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("AdmissionController", "HostelFeeReceipt", ex.Message);
                    }

                }
            }
            return View(objViewModel);
        }
        public ActionResult PayCollegeFee()
        {
            List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                //ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                //var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.SemesterFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                //if (FetchFrequency != null && FetchFrequency.Count > 0)
                studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_ID = Common.FrequencyType.SemesterFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(studentAcountList);
        }
        public ActionResult PaymentForCollegeFee()
        {
            PayUPost objPayUPost = new PayUPost();
            string sGroupFrequency = string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.SemesterFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                if (FetchFrequency != null && FetchFrequency.Count > 0)
                {
                    var FetchStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_ID = Common.FrequencyType.SemesterFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    if (FetchStudentAccount != null && FetchStudentAccount.Count > 0)
                    {

                        var person = FetchStudentAccount.FirstOrDefault();
                        var GroupFrequencyId = FetchStudentAccount.GroupBy(x => x.FREQUENCY_ID).ToList();
                        foreach (var item in GroupFrequencyId)
                        {
                            sGroupFrequency += item.FirstOrDefault().FREQUENCY_ID + ",";
                        }
                        sGroupFrequency = sGroupFrequency.TrimEnd(',');
                        var payUPaymentInfo = (List<FEE_MERCHANT_ACCOUNT_INFO>)CMSHandler.FetchData<FEE_MERCHANT_ACCOUNT_INFO>(new FEE_MERCHANT_ACCOUNT_INFO() { ACCOUNT_TYPE = Common.FrequencyType.HostelFee, API_TYPE = Common.PayUMoneyAPIsType.BaseUrl }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMerchantInfo)).DataSource.Data;
                        if (payUPaymentInfo != null && payUPaymentInfo.Count > 0)
                        {
                            objPayUPost.productinfo = "Semester Fee";
                            objPayUPost.key = payUPaymentInfo.FirstOrDefault().KEY;
                            objPayUPost.salt = payUPaymentInfo.FirstOrDefault().SALT;
                            objPayUPost.udf1 = person.STUDENT_ID;
                            objPayUPost.email = person.STU_EMAILID;
                            objPayUPost.udf5 = Common.FeeRoot.CollegeFee;
                            // objPayUPost.udf6 = ;
                            objPayUPost.firstname = person.FIRST_NAME;
                            //  objPayUPost.udf3 = person.FREQUENCY_ID; old fee flow
                            objPayUPost.udf3 = sGroupFrequency;
                            objPayUPost.udf4 = person.REGISTER_NO;
                            objPayUPost.udf2 = Common.FrequencyType.SemesterFee;
                            objPayUPost.txnid = CommonMethods.CommonTransactionId(person.STUDENT_ID);
                            objPayUPost.amount = Convert.ToDecimal(FetchStudentAccount.Sum(s => Convert.ToDecimal(s.BALANCE))).ToString("g29");
                            bool sLoop = true;
                            while (sLoop)
                            {
                                var CheckIsRequestExist = (List<FEE_PAYU_REQUEST>)CMSHandler.FetchData<FEE_PAYU_REQUEST>(new FEE_PAYU_REQUEST() { txnid = objPayUPost.txnid }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.IsPayURequestExist), sAcademicYear).DataSource.Data;
                                if (CheckIsRequestExist != null && CheckIsRequestExist.Count > 0)
                                {
                                    objPayUPost.txnid = CommonMethods.CommonTransactionId(person.STUDENT_ID);
                                }
                                else
                                {
                                    var SavePayURequest = CMSHandler.SaveOrUpdate(objPayUPost, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.SavePayURequest), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString());
                                    if (SavePayURequest.Success)
                                        sLoop = false;
                                }
                            }
                            objPayUPost.hash = GetHashValue(payUPaymentInfo.FirstOrDefault().HASH_SEQUENCE, payUPaymentInfo.FirstOrDefault().KEY, objPayUPost);
                            objPayUPost.surl = payUPaymentInfo.FirstOrDefault().surl;
                            objPayUPost.furl = payUPaymentInfo.FirstOrDefault().furl;
                            objPayUPost.curl = payUPaymentInfo.FirstOrDefault().curl;
                            objPayUPost.phone = person.STU_MOBILENO;
                            objPayUPost.BASE_URL = payUPaymentInfo.FirstOrDefault().BASE_URL.Trim();
                            return View(objPayUPost);
                        }
                    }
                    else
                    {
                        objPayUPost.sMessage = "Sorry for the inconvenience,Please Close your browser and register again. ";
                    }
                }
                else
                    objPayUPost.sMessage = Common.Messages.AccountNotAvailable;
            }
            else
                objPayUPost.sMessage = Common.Messages.RequestTimeOut;
            return View(objPayUPost);
        }
        #endregion

        #region College Fee Payment For SJC
        [UserRights("Student")]
        public ActionResult FeeBalanceListForSJC()
        {
            FeeTransactionViewModel liFeeTransactions = new FeeTransactionViewModel();

            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {

                    liFeeTransactions.liFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() },
                        ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchFeeTransaction), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;

                    liFeeTransactions.liFeeTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() },
                        ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchUrNumberByStudentId), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                }
                catch (Exception e)
                {
                    string sErrorMessage = e.Message;
                }

            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }

            return View(liFeeTransactions);
        }

        public ActionResult FeeBalanceListParentForSJC()
        {
            FeeTransactionViewModel ObjViewModel = new FeeTransactionViewModel();
            var sUserId = Session[Common.SESSION_VARIABLES.STUDENT_ID].ToString();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sSql = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentRollNoByStudentId).Replace(Common.Delimiter.QUS + Common.STU_PERSONAL_INFO.STUDENT_ID, sUserId);
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    ObjViewModel.lstStudentPersonalInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(null, sSql, sAcademicYear).DataSource.Data;
                    var sStudent_id = sUserId.Split(',');
                    foreach (var item in sStudent_id)
                    {
                        var objModel = new FEE_STUDENT_ACCOUNT();
                        objModel.STUDENT_ID = item;

                        sSql = string.Empty;
                        sSql = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchFeeTransaction);
                        var liFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objModel, sSql, sAcademicYear).DataSource.Data;

                        if (liFeeStudentAccount != null && liFeeStudentAccount.Count > 0)
                        {
                            ObjViewModel.liFeeStudentAccount = (ObjViewModel.liFeeStudentAccount != null && ObjViewModel.liFeeStudentAccount.Count > 0) ? ObjViewModel.liFeeStudentAccount.Concat(liFeeStudentAccount).ToList() : liFeeStudentAccount;
                        }

                        var liFeeTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objModel, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchUrNumberByStudentId), sAcademicYear).DataSource.Data;

                        if (liFeeTransaction != null && liFeeTransaction.Count > 0)
                        {
                            ObjViewModel.liFeeTransaction = (ObjViewModel.liFeeTransaction != null && ObjViewModel.liFeeTransaction.Count > 0) ? ObjViewModel.liFeeTransaction.Concat(liFeeTransaction).ToList() : liFeeTransaction;
                        }
                    }
                }
                catch (Exception e)
                {
                    string sErrorMessage = e.Message;
                }

            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }

            return View(ObjViewModel);
        }
        #endregion

        #region Exam Fee Payment 
        //this is old fee flow should not use.10/12/2018
        public ActionResult PayExamFee()
        {
            List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.ExamFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                if (FetchFrequency != null && FetchFrequency.Count > 0)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_ID = FetchFrequency.FirstOrDefault().FREQUENCY_ID }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(studentAcountList);
        }
        public ActionResult PaymentForExamFee()
        {
            PayUPost objPayUPost = new PayUPost();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.ExamFee },
                        FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;

                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                    {
                        var FetchStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_ID = FetchFrequency.FirstOrDefault().FREQUENCY_ID },
                            FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;

                        if (FetchStudentAccount != null && FetchStudentAccount.Count > 0)
                        {
                            var person = FetchStudentAccount.FirstOrDefault();
                            var payUPaymentInfo = (List<FEE_MERCHANT_ACCOUNT_INFO>)CMSHandler.FetchData<FEE_MERCHANT_ACCOUNT_INFO>(null,
                                 FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMerchantInfo)).DataSource.Data;

                            if (payUPaymentInfo != null && payUPaymentInfo.Count > 0)
                            {
                                objPayUPost.productinfo = "Exam Fee";
                                objPayUPost.key = payUPaymentInfo.FirstOrDefault().KEY;
                                objPayUPost.salt = payUPaymentInfo.FirstOrDefault().SALT;
                                objPayUPost.udf1 = person.STUDENT_ID;
                                objPayUPost.email = person.STU_EMAILID;
                                objPayUPost.udf5 = person.STU_MOBILENO;
                                objPayUPost.firstname = person.FIRST_NAME;
                                objPayUPost.udf3 = person.FREQUENCY_ID;
                                objPayUPost.udf4 = person.REGISTER_NO;
                                objPayUPost.udf2 = Common.FrequencyType.ExamFee;
                                objPayUPost.txnid = CommonMethods.CommonTransactionId(person.STUDENT_ID);
                                objPayUPost.amount = Convert.ToDecimal(FetchStudentAccount.Sum(s => Convert.ToDecimal(s.BALANCE))).ToString("g29");
                                bool sLoop = true;
                                while (sLoop)
                                {
                                    var CheckIsRequestExist = (List<FEE_PAYU_REQUEST>)CMSHandler.FetchData<FEE_PAYU_REQUEST>(new FEE_PAYU_REQUEST() { txnid = objPayUPost.txnid },
                                          FeeSQL.GetFeeSQL(FeeSQLCommands.IsPayURequestExist), sAcademicYear).DataSource.Data;
                                    if (CheckIsRequestExist != null && CheckIsRequestExist.Count > 0)
                                    {
                                        objPayUPost.txnid = CommonMethods.CommonTransactionId(person.STUDENT_ID);
                                    }
                                    else
                                    {
                                        var SavePayURequest = CMSHandler.SaveOrUpdate(objPayUPost,
                                              FeeSQL.GetFeeSQL(FeeSQLCommands.SavePayURequest), sAcademicYear);
                                        if (SavePayURequest.Success)
                                            sLoop = false;
                                    }
                                }
                                objPayUPost.hash = GetHashValue(payUPaymentInfo.FirstOrDefault().HASH_SEQUENCE, payUPaymentInfo.FirstOrDefault().KEY, objPayUPost);
                                objPayUPost.surl = payUPaymentInfo.FirstOrDefault().surl;
                                objPayUPost.furl = payUPaymentInfo.FirstOrDefault().furl;
                                objPayUPost.curl = payUPaymentInfo.FirstOrDefault().curl;
                                objPayUPost.phone = person.STU_MOBILENO;
                                objPayUPost.BASE_URL = payUPaymentInfo.FirstOrDefault().BASE_URL.Trim();
                                return View(objPayUPost);
                            }
                        }
                        else
                        {
                            objPayUPost.sMessage = "Sorry for the inconvenience,Please Close your browser and register again. ";
                        }
                    }
                    else
                        objPayUPost.sMessage = Common.Messages.AccountNotAvailable;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("Fee", "PaymentForExamFee", ex.Message);
                    }
                }
            }
            else
                objPayUPost.sMessage = Common.Messages.RequestTimeOut;
            return View(objPayUPost);
        }
        #endregion

        #region Fee Collection Report
        public ActionResult DayWiseFeeReport()
        {
            FeeReportViewModel ObjViewModel = new FeeReportViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liFeeFrequency = (List<sup_frequency_type>)CMSHandler.FetchData<sup_frequency_type>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFrequencytype)).DataSource.Data;
                    if (liFeeFrequency != null && liFeeFrequency.Count > 0)
                    {
                        ObjViewModel.liFrequency = new SelectList(liFeeFrequency, Common.SUP_FREQUENCY_TYPE.FREQUENCY_TYPE_ID, Common.SUP_FREQUENCY_TYPE.FREQUENCY_TYPE);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "DayWiseFeeReport", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "DayWiseFeeReport", ex.Message);
                    }
                }
                return View(ObjViewModel);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        // Fetch DayWiseCollection By Frequency And Date ...
        public ActionResult FetchDayWiseReport(string sDate, string sFrequency)
        {
            FeeReportViewModel ObjViewModel = new FeeReportViewModel();
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sDate) && !string.IsNullOrEmpty(sFrequency))
                    {
                        sDate = CommonMethods.ConvertdatetoyearFromat(sDate);
                        ObjViewModel.liFeeReport = (List<FEE_REPORT>)CMSHandler.FetchData<FEE_REPORT>(new FEE_REPORT() { PAYMENT_DATE = sDate, FREQUENCY = sFrequency }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDaywiseFeeReport)).DataSource.Data;
                        return View(ObjViewModel);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "FetchDayWiseReport", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "FetchDayWiseReport", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        // Fetch Daywise FeeReport By Transaction Id ...
        public ActionResult DaywiseReportByTransactionId(string sTransactionId)
        {
            FeeReportViewModel ObjViewModel = new FeeReportViewModel();
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sTransactionId))
                {
                    try
                    {
                        ObjViewModel.liFeeReport = (List<FEE_REPORT>)CMSHandler.FetchData<FEE_REPORT>(new FEE_REPORT() { TRANSACTION_ID = sTransactionId }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDaywiseFeeReportByTransactionId)).DataSource.Data;
                        return View(ObjViewModel);
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("Fee", "FetchDaywiseReportByTransactionId", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("Fee", "FetchDaywiseReportByTransactionId", ex.Message);
                        }
                        ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
        // DateWise Fee Report ...
        public ActionResult DatewiseFeeReport()
        {
            FeeReportViewModel ObjViewModel = new FeeReportViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liFrequency = (List<sup_frequency_type>)CMSHandler.FetchData<sup_frequency_type>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFrequencytype)).DataSource.Data;
                    if (liFrequency != null && liFrequency.Count > 0)
                    {
                        ObjViewModel.liFrequency = new SelectList(liFrequency, Common.SUP_FREQUENCY_TYPE.FREQUENCY_TYPE_ID, Common.SUP_FREQUENCY_TYPE.FREQUENCY_TYPE);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "FetchDaywiseReportByTransactionId", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "FetchDaywiseReportByTransactionId", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindDateWiseFeeReport(string FromDate, string ToDate, string sFrequency)
        {
            FeeReportViewModel ObjViewModel = new FeeReportViewModel();
            JsonResultData ObjJsonData = new JsonResultData();
            var FeeReport = new FEE_REPORT();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                FeeReport.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(FromDate);
                FeeReport.DATE_TO = CommonMethods.ConvertdatetoyearFromat(ToDate);
                FeeReport.FREQUENCY = sFrequency;
                ObjViewModel.liFeeReport = (List<FEE_REPORT>)CMSHandler.FetchData<FEE_REPORT>(FeeReport, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDatewiseFeeReport), sAcademicYear).DataSource.Data;
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }

        // Hostel Student Fee Report ...
        public ActionResult HostelStudentFeeReport()
        {
            FeeReportViewModel ObjViewModel = new FeeReportViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        ObjViewModel.AcademicYearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);

                    var FetchHostelList = (List<SUP_HOSTEL>)CMSHandler.FetchData<SUP_HOSTEL>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHostelDDL)).DataSource.Data;
                    if (FetchHostelList != null && FetchHostelList.Count > 0)
                        ObjViewModel.HostelList = new SelectList(FetchHostelList, "HOSTEL_ID", "HOSTEL_NAME");
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "HostelStudentFeeReport", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindHostelStudentFeeReport(string sTab, string sAcYear, string FromDate, string ToDate, string sFrequency, string sHead, string sHostel)
        {
            FeeReportViewModel ObjViewModel = new FeeReportViewModel();
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = sAcYear;
            if (string.IsNullOrEmpty(sAcademicYear) && Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            }

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    HOSTEL_FEE_REPORT objModel = new HOSTEL_FEE_REPORT();
                    objModel.ACADEMIC_YEAR = sAcademicYear;
                    if (!string.IsNullOrEmpty(FromDate))
                        objModel.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(FromDate);
                    if (!string.IsNullOrEmpty(ToDate))
                        objModel.DATE_TO = CommonMethods.ConvertdatetoyearFromat(ToDate);

                    string sSQL = "";
                    if (sTab == "paidwise")
                    {
                        sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelStudentPaidReportDateWise);
                    }
                    else if (sTab == "balance" || sTab == "balance_headwise")
                    {
                        sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelStudentBalanceReport);
                    }
                    else if (sTab == "freqwise")
                    {
                        sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelStudentFrequencyWiseReport);
                    }
                    else if (sTab == "freqheadwise")
                    {
                        sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelStudentFrequencyAndHeadWiseReport);
                    }

                    // Replace placeholders
                    sSQL = sSQL.Replace("?FREQUENCY_CONDITION", (!string.IsNullOrEmpty(sFrequency) && sFrequency != "0" && sFrequency != "null") ? "AND FFF.FREQUENCY_ID = '" + sFrequency + "'" : "");
                    
                    string headCondition = "";
                    if (!string.IsNullOrEmpty(sHead) && sHead != "0" && sHead != "null")
                    {
                        string columnAlias = (sTab == "paidwise") ? "FC.HEAD" : "FS.HEAD";
                        if (sHead.Contains(","))
                        {
                            var heads = sHead.Split(',').Select(h => "'" + h.Trim() + "'");
                            headCondition = "AND " + columnAlias + " IN (" + string.Join(",", heads) + ")";
                        }
                        else
                        {
                            headCondition = "AND " + columnAlias + " = '" + sHead + "'";
                        }
                    }
                    sSQL = sSQL.Replace("?HEAD_CONDITION", headCondition);
                    sSQL = sSQL.Replace("?HOSTEL_CONDITION", (!string.IsNullOrEmpty(sHostel) && sHostel != "0" && sHostel != "null") ? "AND HS.HOSTEL_ID = '" + sHostel + "'" : "");

                    ObjViewModel.liHostelFeeReport = (List<HOSTEL_FEE_REPORT>)CMSHandler.FetchData<HOSTEL_FEE_REPORT>(objModel, sSQL, sAcademicYear).DataSource.Data;

                    if (sTab == "balance_headwise")
                    {
                        var headList = new List<string>();

                        if (!string.IsNullOrEmpty(sFrequency) && sFrequency != "0" && sFrequency != "null")
                        {
                            try
                            {
                                string qHeads = @"SELECT DISTINCT SH.HEAD_CODE 
                                                  FROM FEE_MAIN_HEADS MH 
                                                  INNER JOIN SUP_HEAD SH ON SH.HEAD_ID = MH.HEAD_ID 
                                                  WHERE MH.ACADEMIC_YEAR = @AC_YEAR AND MH.FREQUENCY_ID = @FREQ_ID AND MH.IS_DELETED != 1";
                                var fHeads = DapperHandler.FetchAll<string>(qHeads, new { AC_YEAR = sAcademicYear, FREQ_ID = sFrequency }).ToList();
                                if (fHeads != null && fHeads.Count > 0)
                                {
                                    headList.AddRange(fHeads.Where(x => !string.IsNullOrEmpty(x)));
                                }
                            }
                            catch { }
                        }

                        if (ObjViewModel.liHostelFeeReport != null && ObjViewModel.liHostelFeeReport.Count > 0)
                        {
                            var reportHeads = ObjViewModel.liHostelFeeReport
                                .Where(x => !string.IsNullOrEmpty(x.HEAD_CODE))
                                .Select(x => x.HEAD_CODE.Trim())
                                .Distinct();
                            headList = headList.Union(reportHeads).ToList();
                        }

                        if (!string.IsNullOrEmpty(sHead) && sHead != "0" && sHead != "null")
                        {
                            try
                            {
                                var headIds = sHead.Split(',').Select(x => x.Trim()).ToList();
                                string qSel = "SELECT DISTINCT HEAD_CODE FROM SUP_HEAD WHERE HEAD_ID IN @IDS";
                                var selCodes = DapperHandler.FetchAll<string>(qSel, new { IDS = headIds }).ToList();
                                if (selCodes != null && selCodes.Count > 0)
                                {
                                    headList = headList.Where(h => selCodes.Any(sc => sc.Equals(h, StringComparison.OrdinalIgnoreCase))).ToList();
                                }
                            }
                            catch { }
                        }

                        string[] monthOrder = { "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC", "JAN", "FEB", "MAR", "APR", "MAY" };
                        headList = headList.Distinct().OrderBy(h => {
                            int idx = Array.FindIndex(monthOrder, m => m.Equals(h, StringComparison.OrdinalIgnoreCase));
                            return idx >= 0 ? idx : 100;
                        }).ThenBy(h => h).ToList();

                        ViewBag.HeadCodes = headList;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "BindHostelStudentFeeReport", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            ViewBag.SelectedTab = sTab;
            return View(ObjViewModel);
        }
        #endregion

        #region Bus Fee Report
        public ActionResult BusFeeReport()
        {
            FeeReportViewModel ObjViewModel = new FeeReportViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        ObjViewModel.AcademicYearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "BusFeeReport", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult FetchBusFrequencyByAcademicYear(string Academicyear)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;
                    string query = @"SELECT FFF.FEE_FREQUENCY_FEE_MODE_ID AS FREQUENCY_ID, SFF.FREQUENCY_NAME AS FREQUENCY_NAME
                                     FROM FEE_FREQUENCY_FEE_MODE AS FFF
                                     INNER JOIN SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                     WHERE FFF.FEE_MODE = '8' AND FFF.ACADEMIC_YEAR = @AC_YEAR";

                    var lstFreq = DapperHandler.FetchAll<dynamic>(query, new { AC_YEAR = sAcademicYear }).ToList();
                    if (lstFreq != null && lstFreq.Count > 0)
                    {
                        foreach (var item in lstFreq)
                        {
                            objResult.sResult += "<option value='" + item.FREQUENCY_ID + "'>" + item.FREQUENCY_NAME + "</option>";
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
                    objHandler.WriteError("Fee", "FetchBusFrequencyByAcademicYear", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchBusHeadsByFrequencyId(string sFrequencyId, string Academicyear)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;
                    string query = @"SELECT 
                                        MH.HEAD_ID AS HEAD_ID,
                                        SH.HEAD AS HEAD_NAME,
                                        SH.HEAD_CODE AS HEAD_CODE
                                    FROM
                                        FEE_MAIN_HEADS AS MH
                                            INNER JOIN
                                        SUP_HEAD AS SH ON SH.HEAD_ID = MH.HEAD_ID
                                    WHERE
                                        MH.ACADEMIC_YEAR = @AC_YEAR
                                            AND MH.FREQUENCY_ID = @FREQUENCY_ID
                                            AND MH.IS_DELETED != 1
                                    GROUP BY MH.HEAD_ID";

                    var lstHeads = DapperHandler.FetchAll<dynamic>(query, new { AC_YEAR = sAcademicYear, FREQUENCY_ID = sFrequencyId }).ToList();
                    if (lstHeads != null && lstHeads.Count > 0)
                    {
                        foreach (var item in lstHeads)
                        {
                            objResult.sResult += "<option value='" + item.HEAD_ID + "'>" + item.HEAD_NAME + " (" + item.HEAD_CODE + ")</option>";
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
                    objHandler.WriteError("Fee", "FetchBusHeadsByFrequencyId", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BindBusFeeReport(string sTab, string sAcYear, string FromDate, string ToDate, string sFrequency, string sHead)
        {
            FeeReportViewModel ObjViewModel = new FeeReportViewModel();
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = sAcYear;
            if (string.IsNullOrEmpty(sAcademicYear) && Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            }

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    BUS_FEE_REPORT objModel = new BUS_FEE_REPORT();
                    objModel.ACADEMIC_YEAR = sAcademicYear;
                    if (!string.IsNullOrEmpty(FromDate))
                        objModel.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(FromDate);
                    if (!string.IsNullOrEmpty(ToDate))
                        objModel.DATE_TO = CommonMethods.ConvertdatetoyearFromat(ToDate);

                    string sSQL = "";
                    if (sTab == "student_paid")
                    {
                        sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusFeeStudentPaidReportDateWise);
                    }
                    else if (sTab == "student_balance")
                    {
                        sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusFeeStudentBalanceReport);
                    }
                    else if (sTab == "student_freq")
                    {
                        sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusFeeStudentFrequencyWiseReport);
                    }
                    else if (sTab == "staff_paid")
                    {
                        sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusFeeStaffPaidReportDateWise);
                    }
                    else if (sTab == "staff_balance")
                    {
                        sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusFeeStaffBalanceReport);
                    }
                    else if (sTab == "staff_freq")
                    {
                        sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusFeeStaffFrequencyWiseReport);
                    }

                    // Frequency condition
                    sSQL = sSQL.Replace("?FREQUENCY_CONDITION", (!string.IsNullOrEmpty(sFrequency) && sFrequency != "0" && sFrequency != "null") ? "AND FFF.FREQUENCY_ID = '" + sFrequency + "'" : "");

                    // Head condition
                    string headCondition = "";
                    if (!string.IsNullOrEmpty(sHead) && sHead != "0" && sHead != "null")
                    {
                        string columnAlias = (sTab == "student_paid" || sTab == "staff_paid") ? "FC.HEAD" : "FS.HEAD";
                        if (sHead.Contains(","))
                        {
                            var heads = sHead.Split(',').Select(h => "'" + h.Trim() + "'");
                            headCondition = "AND " + columnAlias + " IN (" + string.Join(",", heads) + ")";
                        }
                        else
                        {
                            headCondition = "AND " + columnAlias + " = '" + sHead + "'";
                        }
                    }
                    sSQL = sSQL.Replace("?HEAD_CONDITION", headCondition);

                    ObjViewModel.liBusFeeReport = (List<BUS_FEE_REPORT>)CMSHandler.FetchData<BUS_FEE_REPORT>(objModel, sSQL, sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "BindBusFeeReport", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            ViewBag.SelectedTab = sTab;
            return View(ObjViewModel);
        }
        #endregion

        #region MessFee
        [UserRights("STUDENT")]
        public ActionResult PayMessFee()
        {
            List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_TYPE_ID = Common.FrequencyType.MessFee, FEE_ROOT_ID = Common.FeeRoot.MessFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FecthMessFeesByStudentIdAndFeeRootId), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(studentAcountList);
        }
        public ActionResult PaymentForMessFee()
        {
            PayUPost objPayUPost = new PayUPost();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.MessFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                if (FetchFrequency != null && FetchFrequency.Count > 0)
                {
                    var FetchStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_TYPE_ID = Common.FrequencyType.MessFee, FEE_ROOT_ID = Common.FeeRoot.MessFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FecthMessFeesByStudentIdAndFeeRootId), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                    if (FetchStudentAccount != null && FetchStudentAccount.Count > 0)
                    {
                        var person = FetchStudentAccount.FirstOrDefault();
                        var payUPaymentInfo = (List<FEE_MERCHANT_ACCOUNT_INFO>)CMSHandler.FetchData<FEE_MERCHANT_ACCOUNT_INFO>(new FEE_MERCHANT_ACCOUNT_INFO() { ACCOUNT_TYPE = Common.FrequencyType.MessFee, API_TYPE = Common.PayUMoneyAPIsType.BaseUrl }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMerchantInfo)).DataSource.Data;
                        if (payUPaymentInfo != null && payUPaymentInfo.Count > 0)
                        {
                            objPayUPost.productinfo = "Mess Fee";
                            objPayUPost.key = payUPaymentInfo.FirstOrDefault().KEY;
                            objPayUPost.salt = payUPaymentInfo.FirstOrDefault().SALT;
                            objPayUPost.udf1 = person.STUDENT_ID;
                            objPayUPost.email = person.STU_EMAILID;
                            objPayUPost.udf5 = Common.FeeRoot.MessFee;
                            objPayUPost.firstname = person.FIRST_NAME;
                            objPayUPost.udf3 = FetchFrequency.FirstOrDefault().FREQUENCY_ID;
                            objPayUPost.udf4 = person.REGISTER_NO;
                            objPayUPost.udf2 = Common.FrequencyType.MessFee;
                            objPayUPost.txnid = CommonMethods.CommonTransactionId(person.STUDENT_ID);
                            objPayUPost.amount = Convert.ToDecimal(FetchStudentAccount.Sum(s => Convert.ToDecimal(s.BALANCE))).ToString("g29");
                            bool sLoop = true;
                            while (sLoop)
                            {
                                var CheckIsRequestExist = (List<FEE_PAYU_REQUEST>)CMSHandler.FetchData<FEE_PAYU_REQUEST>(new FEE_PAYU_REQUEST() { txnid = objPayUPost.txnid }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.IsPayURequestExist), sAcademicYear).DataSource.Data;
                                if (CheckIsRequestExist != null && CheckIsRequestExist.Count > 0)
                                {
                                    objPayUPost.txnid = CommonMethods.CommonTransactionId(person.STUDENT_ID);
                                }
                                else
                                {
                                    var SavePayURequest = CMSHandler.SaveOrUpdate(objPayUPost, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.SavePayURequest), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString());
                                    if (SavePayURequest.Success)
                                        sLoop = false;
                                }
                            }
                            objPayUPost.hash = GetHashValue(payUPaymentInfo.FirstOrDefault().HASH_SEQUENCE, payUPaymentInfo.FirstOrDefault().KEY, objPayUPost);
                            objPayUPost.surl = payUPaymentInfo.FirstOrDefault().surl;
                            objPayUPost.furl = payUPaymentInfo.FirstOrDefault().furl;
                            objPayUPost.curl = payUPaymentInfo.FirstOrDefault().curl;
                            objPayUPost.phone = person.STU_MOBILENO;
                            objPayUPost.BASE_URL = payUPaymentInfo.FirstOrDefault().BASE_URL.Trim();
                            return View(objPayUPost);
                        }
                    }
                    else
                    {
                        objPayUPost.sMessage = Common.Messages.AccountNotAvailable;
                    }
                }
                else
                    objPayUPost.sMessage = Common.Messages.AccountNotAvailable;
            }
            else
                objPayUPost.sMessage = Common.Messages.RequestTimeOut;
            return View(objPayUPost);
        }
        #endregion

        #region DayWiseFeeCollectionPrincipalView

        public ActionResult DayWiseFeeCollectionPrincipalView(string sDateFrom, string sDateTo)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objViewModel.FeeTransaction.DATE_FROM = sDateFrom;
                    objViewModel.FeeTransaction.DATE_TO = sDateTo;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "DayWiseFeeCollectionPrincipalView", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "DayWiseFeeCollectionPrincipalView", Ex.Message);
                }

            }
            return View(objViewModel);
        }

        public ActionResult BindDayWiseFeeCollectionPrincipalView(string sDateFrom, string sDateTo)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            var objFeeStudentAccountModel = new FEE_TRANSACTION();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objViewModel.FeeTransaction.DATE_FROM = sDateFrom;
                    objViewModel.FeeTransaction.DATE_TO = sDateTo;
                    objFeeStudentAccountModel.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(sDateFrom);
                    objFeeStudentAccountModel.DATE_TO = CommonMethods.ConvertdatetoyearFromat(sDateTo);
                    var FetchCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    var FetchClassWiseFeeDetails = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDayWiseFeeCollection), sAcademicYear).DataSource.Data;
                    if (FetchClassWiseFeeDetails != null && FetchClassWiseFeeDetails.Count > 0)
                        objViewModel.liFeeTransaction = FetchClassWiseFeeDetails;
                    if (FetchCollegeDetails != null && FetchCollegeDetails.Count > 0)
                        objViewModel.liCollegeDetails = FetchCollegeDetails;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "BindDayWiseFeeCollectionPrincipalView", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "BindDayWiseFeeCollectionPrincipalView", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult ViewPaymentDetailsByDate(string sPaymentDate, string sDateFrom, string sDateTo)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            var objFeeStudentAccountModel = new FEE_TRANSACTION();
            try
            {
                if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    objViewModel.FeeTransaction.DATE_FROM = sDateFrom;
                    objViewModel.FeeTransaction.DATE_TO = sDateTo;
                    objFeeStudentAccountModel.PAYMENT_DATE = CommonMethods.ConvertdatetoyearFromat(sPaymentDate);
                    var FetchClassWiseFeeDetails = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeePaymentDetailsByDate), sAcademicYear).DataSource.Data;
                    if (FetchClassWiseFeeDetails != null && FetchClassWiseFeeDetails.Count > 0)
                        objViewModel.liFeeTransaction = FetchClassWiseFeeDetails;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "ViewPaymentDetailsByDate", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "ViewPaymentDetailsByDate", ex.Message);
                }
            }
            return View(objViewModel);
        }
        #endregion

        #region Head Wise Fee Report
        public ActionResult HeadWiseFeeReport()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var FetchFrequency = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    var FetchProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;
                    var FetchClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data;
                    var FetchClassLevel = (List<SupClassLevel>)CMSHandler.FetchData<SupClassLevel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassLevel)).DataSource.Data;
                    var FetchAcademicyear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicyear != null && FetchAcademicyear.Count > 0)
                        objViewModel.liAcademicYear = new SelectList(FetchAcademicyear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    if (FetchProgrammeMode != null && FetchProgrammeMode.Count > 0)
                        objViewModel.liProgrammeModeList = new SelectList(FetchProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                    if (FetchClassLevel != null && FetchClassLevel.Count > 0)
                        objViewModel.liClassLevel = new SelectList(FetchClassLevel, Common.SUP_CLASS_LEVEL.CLASSLEVELID, Common.SUP_CLASS_LEVEL.CLASSLEVEL);
                    if (FetchClassYear != null && FetchClassYear.Count > 0)
                        objViewModel.liClassYear = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "HeadWiseFeeReport", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "HeadWiseFeeReport", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public JsonResult GetFeeFrequencyByAcademicyear(string AcYear)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicyear = AcYear;
                    var FetchFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeeFrequency), sAcademicyear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                    {
                        foreach (var item in FetchFrequency)
                            objResult.sResult += "<option value='" + item.FREQUENCY_ID + "'>" + item.FREQUENCY_NAME + "</option>";
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "GetFeeFrequencyByAcademicyear", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "GetFeeFrequencyByAcademicyear", Ex.Message);
                }

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSupHeadByFreqencyAndProgramme(string AcYear, string ProgrammeMode, string Frequency)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicyear = AcYear;
                    FEE_STRUCTURE objModel = new FEE_STRUCTURE();
                    objModel.FREQUENCY_ID = Frequency;
                    objModel.PROGRAMME_MODE = ProgrammeMode;
                    var FetchFrequency = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHeadByFrequencyAndProgrammeMode), sAcademicyear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                    {
                        foreach (var item in FetchFrequency)
                            objResult.sResult += "<option value='" + item.HEAD_ID + "'>" + item.HEAD + "</option>";
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "GetSupHeadByFreqencyAndProgramme", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "GetSupHeadByFreqencyAndProgramme", Ex.Message);
                }

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindHeadWiseFeeCollection(string sDateFrom, string sDateTo, string AcYear, string Frequency, string ProgrammeMode, string Head, string ClassYear, string ClassLevel)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = AcYear;
            var objFeeStudentAccountModel = new FEE_TRANSACTION();
            string sSQL = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objViewModel.FeeTransaction.DATE_FROM = sDateFrom;
                    objViewModel.FeeTransaction.DATE_TO = sDateTo;
                    objFeeStudentAccountModel.FREQUENCY = Frequency;
                    objFeeStudentAccountModel.PROGRAMME_MODE = ProgrammeMode;
                    objFeeStudentAccountModel.CLASS_LEVEL = ClassLevel;
                    objFeeStudentAccountModel.CLASS_YEAR = ClassYear;
                    objFeeStudentAccountModel.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(sDateFrom);
                    objFeeStudentAccountModel.DATE_TO = CommonMethods.ConvertdatetoyearFromat(sDateTo);
                    var FetchDates = (List<DayOrderCalender>)CMSHandler.FetchData<DayOrderCalender>(objFeeStudentAccountModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDatesByDates), sAcademicYear).DataSource.Data;
                    var FetchStudentIds = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objFeeStudentAccountModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentIds), sAcademicYear).DataSource.Data;
                    var FetchCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupHeadsById).Replace(Common.Delimiter.QUS + Common.SUP_HEAD.HEAD_ID, Head);
                    var FetchSupHeads = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(null, sSQL, sAcademicYear).DataSource.Data;
                    sSQL = string.Empty;
                    sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHeadwiseFeeReport).Replace(Common.Delimiter.QUS + Common.SUP_HEAD.HEAD, Head);
                    var FetchHeadWiseFeeDetails = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, sSQL, sAcademicYear).DataSource.Data;
                    if (FetchHeadWiseFeeDetails != null && FetchHeadWiseFeeDetails.Count > 0)
                        objViewModel.liFeeTransaction = FetchHeadWiseFeeDetails;
                    if (FetchCollegeDetails != null && FetchCollegeDetails.Count > 0)
                        objViewModel.liCollegeDetails = FetchCollegeDetails;
                    if (FetchDates != null && FetchDates.Count > 0)
                        objViewModel.liDate = FetchDates;
                    if (FetchSupHeads != null && FetchSupHeads.Count > 0)
                        objViewModel.lstSupHead = FetchSupHeads;
                    if (FetchStudentIds != null && FetchStudentIds.Count > 0)
                        objViewModel.liCreditAndDebit = FetchStudentIds;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "BindHeadWiseFeeCollection", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "BindHeadWiseFeeCollection", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        #endregion

        #region Main Head Wise Fee Report
        // NOT IN USE
        public ActionResult MainHeadWiseFeeReport()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchFrequency = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    var FetchProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;
                    var FetchClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data;
                    var FetchClassLevel = (List<SupClassLevel>)CMSHandler.FetchData<SupClassLevel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassLevel)).DataSource.Data;
                    var FetchShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    var FetchAcademicyear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicyear != null && FetchAcademicyear.Count > 0)
                        objViewModel.liAcademicYear = new SelectList(FetchAcademicyear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    if (FetchProgrammeMode != null && FetchProgrammeMode.Count > 0)
                        objViewModel.liProgrammeModeList = new SelectList(FetchProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                    if (FetchClassLevel != null && FetchClassLevel.Count > 0)
                        objViewModel.liClassLevel = new SelectList(FetchClassLevel, Common.SUP_CLASS_LEVEL.CLASSLEVELID, Common.SUP_CLASS_LEVEL.CLASSLEVEL);
                    if (FetchClassYear != null && FetchClassYear.Count > 0)
                        objViewModel.liClassYear = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                    if (FetchShift != null && FetchShift.Count > 0)
                        objViewModel.liShift = new SelectList(FetchShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "MainHeadWiseFeeReport", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "MainHeadWiseFeeReport", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindMainHeadWiseFeeCollection(string sDateFrom, string sDateTo, string AcYear, string Frequency, string ProgrammeMode, string Head, string ClassYear, string ClassLevel, string Shift)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = AcYear;
            var objFeeStudentAccountModel = new FEE_TRANSACTION();
            string sSQL = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objViewModel.FeeTransaction.DATE_FROM = sDateFrom;
                    objViewModel.FeeTransaction.DATE_TO = sDateTo;
                    objFeeStudentAccountModel.FREQUENCY = Frequency;
                    objFeeStudentAccountModel.PROGRAMME_MODE = ProgrammeMode;
                    objFeeStudentAccountModel.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(sDateFrom);
                    objFeeStudentAccountModel.DATE_TO = CommonMethods.ConvertdatetoyearFromat(sDateTo);
                    objFeeStudentAccountModel.SHIFT = Shift;
                    objFeeStudentAccountModel.CLASS_YEAR = ClassYear;
                    objFeeStudentAccountModel.CLASS_LEVEL = ClassLevel;
                    var FetchDates = (List<DayOrderCalender>)CMSHandler.FetchData<DayOrderCalender>(objFeeStudentAccountModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDatesByDates), sAcademicYear).DataSource.Data;
                    var FetchProgrammeGroupIds = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objFeeStudentAccountModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupIds), sAcademicYear).DataSource.Data;
                    var FetchCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupMainHeadById).Replace(Common.Delimiter.QUS + Common.SUP_FEE_MAIN_HEAD.MAIN_HEAD_ID, Head);
                    var FetchSupMainHeads = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(null, sSQL, sAcademicYear).DataSource.Data;
                    sSQL = string.Empty;
                    sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMainHeadwiseFeeReport).Replace(Common.Delimiter.QUS + Common.SUP_FEE_MAIN_HEAD.MAIN_HEAD_ID, Head);
                    var FetchHeadWiseFeeDetails = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, sSQL, sAcademicYear).DataSource.Data;
                    if (FetchHeadWiseFeeDetails != null && FetchHeadWiseFeeDetails.Count > 0)
                        objViewModel.liFeeTransaction = FetchHeadWiseFeeDetails;
                    if (FetchCollegeDetails != null && FetchCollegeDetails.Count > 0)
                        objViewModel.liCollegeDetails = FetchCollegeDetails;
                    if (FetchDates != null && FetchDates.Count > 0)
                        objViewModel.liDate = FetchDates;
                    if (FetchSupMainHeads != null && FetchSupMainHeads.Count > 0)
                        objViewModel.lstSupMainHead = FetchSupMainHeads;
                    if (FetchProgrammeGroupIds != null && FetchProgrammeGroupIds.Count > 0)
                        objViewModel.liCreditAndDebit = FetchProgrammeGroupIds;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "BindMainHeadWiseFeeCollection", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "BindMainHeadWiseFeeCollection", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public JsonResult GetSupMainHeadByFreqencyAndProgrammeAndShift(string AcYear, string ProgrammeMode, string Frequency, string Shift)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicyear = AcYear;
                    FEE_STRUCTURE objModel = new FEE_STRUCTURE();
                    objModel.FREQUENCY_ID = Frequency;
                    objModel.PROGRAMME_MODE = ProgrammeMode;
                    objModel.SHIFT = Shift;
                    var FetchFrequency = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupMainHeadByFrequencyAndProgrammeMode), sAcademicyear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                    {
                        foreach (var item in FetchFrequency)
                            objResult.sResult += "<option value='" + item.MAIN_HEAD_ID + "'>" + item.MAIN_HEAD + "</option>";
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "GetSupMainHeadByFreqencyAndProgrammeAndShift", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "GetSupMainHeadByFreqencyAndProgrammeAndShift", Ex.Message);
                }

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMainHeadByFreqencyAndProgramme(string AcYear, string ProgrammeMode, string Frequency)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicyear = AcYear;
                    FEE_STRUCTURE objModel = new FEE_STRUCTURE();
                    objModel.FREQUENCY_ID = Frequency;
                    objModel.PROGRAMME_MODE = ProgrammeMode;
                    var FetchFrequency = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupHeadByFrequencyAndProgrammeMode), sAcademicyear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                    {
                        foreach (var item in FetchFrequency)
                            objResult.sResult += "<option value='" + item.HEAD_ID + "'>" + item.HEAD + "</option>";
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "GetMainHeadByFreqencyAndProgramme", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "GetMainHeadByFreqencyAndProgramme", Ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Query Construction
        public ActionResult QueryConstruction()
        {
            string QueryConstruction = string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            var FetchStudentIds = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentIdsForQueryConstruction)).DataSource.Data;
            foreach (var item in FetchStudentIds)
            {
                // need to meet me by paul
                QueryConstruction += "(" + item.STUDENT_ID + "," + sAcademicYear + "," + item.TEMP_ID + ",110,310)," + "(" + item.STUDENT_ID + "," + sAcademicYear + "," + item.TEMP_ID + ",111,100)," + "(" + item.STUDENT_ID + "," + sAcademicYear + "," + item.TEMP_ID + ",112,219)," + "(" + item.STUDENT_ID + "," + sAcademicYear + "," + item.TEMP_ID + ",113,104)," + "(" + item.STUDENT_ID + "," + sAcademicYear + "," + item.TEMP_ID + ",114,48)," + "(" + item.STUDENT_ID + "," + sAcademicYear + "," + item.TEMP_ID + ",115,213)," + "(" + item.STUDENT_ID + "," + sAcademicYear + "," + item.TEMP_ID + ",116,181)," + "(" + item.STUDENT_ID + "," + sAcademicYear + "," + item.TEMP_ID + ",99,300)," + "(" + item.STUDENT_ID + "," + sAcademicYear + "," + item.TEMP_ID + ",117,25),";

            }
            return View();
        }
        #endregion

        #region Mess Fee Report
        public ActionResult MessFeeReportForEachMonth()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var SupFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(new SUP_FEE_FREQUENCY() { FEE_ROOT_ID = Common.FeeRoot.MessFee }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeModeAndFeeRootId), sAcademicYear).DataSource.Data;
                    objViewModel.LiSupFeeRoot = (List<SUP_FEE_ROOT>)CMSHandler.FetchData<SUP_FEE_ROOT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FectchFeeRoot), sAcademicYear).DataSource.Data;
                    var FetchAcademicyear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicyear != null && FetchAcademicyear.Count > 0)
                        objViewModel.liAcademicYear = new SelectList(FetchAcademicyear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    if (SupFrequency != null && SupFrequency.Count > 0)
                        objViewModel.liFrequencyType = new SelectList(SupFrequency, Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, Common.SUP_FEE_FREQUENCY.FREQUENCY_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "MessFeeReportForEachMonth", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "MessFeeReportForEachMonth", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindMessFeeCollection(string sFrequencyId, string sAcademicYear, string sFeeRoot, string sHeads)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            var objFeeStudentAccountModel = new FEE_STUDENT_ACCOUNT();
            string sSQL = string.Empty;
            string SQL = string.Empty;
            try
            {
                objFeeStudentAccountModel.FREQUENCY_ID = sFrequencyId;
                objFeeStudentAccountModel.ACADEMIC_YEAR = sAcademicYear;
                objFeeStudentAccountModel.FEE_ROOT_ID = sFeeRoot;
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    SQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMessFeePaidDate).Replace(Common.Delimiter.QUS + Common.SUP_FEE_MAIN_HEAD.MAIN_HEAD_ID, sHeads);
                    objViewModel.liFeeTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, SQL, sAcademicYear).DataSource.Data;
                    sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMessFeeReport).Replace(Common.Delimiter.QUS + Common.SUP_FEE_MAIN_HEAD.MAIN_HEAD_ID, sHeads);
                    objViewModel.liFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objFeeStudentAccountModel, sSQL, sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "BindMessFeeCollection", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "BindMessFeeCollection", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public JsonResult GetHeadByFreqencyForMess(string sAcademicYear, string sFeeRoot, string sFrequency)
        {
            JsonResultData objResult = new JsonResultData();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.FREQUENCY_ID = sFrequency;
                    objModel.ACADEMIC_YEAR = sAcademicYear;
                    objModel.FEE_ROOT_ID = sFeeRoot;
                    var FetchMainHead = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupHeadByFrequencyAndFeeRoot)).DataSource.Data;
                    if (FetchMainHead != null && FetchMainHead.Count > 0)
                    {
                        foreach (var item in FetchMainHead)
                            objResult.sResult += "<option value='" + item.MAIN_HEAD_ID + "'>" + item.MAIN_HEAD + "</option>";
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "GetHeadByFreqencyForMess", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "GetHeadByFreqencyForMess", Ex.Message);
                }

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult HostelFeePaidStatus()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var SupFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(new SUP_FEE_FREQUENCY() { FEE_ROOT_ID = Common.FeeRoot.HostelFee }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeModeAndFeeRootId), sAcademicYear).DataSource.Data;
                    objViewModel.LiSupFeeRoot = (List<SUP_FEE_ROOT>)CMSHandler.FetchData<SUP_FEE_ROOT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FectchFeeRoot), sAcademicYear).DataSource.Data;
                    var FetchAcademicyear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicyear != null && FetchAcademicyear.Count > 0)
                        objViewModel.liAcademicYear = new SelectList(FetchAcademicyear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    if (SupFrequency != null && SupFrequency.Count > 0)
                        objViewModel.liFrequencyType = new SelectList(SupFrequency, Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, Common.SUP_FEE_FREQUENCY.FREQUENCY_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "MessFeeReportForEachMonth", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "MessFeeReportForEachMonth", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindHostelFeePaidStatus(string sAcademicYear, string sFeeRoot, string sFrequency)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            var objModel = new FEE_STRUCTURE();
            string sSQL = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.ACADEMIC_YEAR = sAcademicYear;
                    objModel.FEE_ROOT_ID = sFeeRoot;
                    objModel.FREQUENCY_ID = sFrequency;
                    objViewModel.liFeeTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelFeePaidDate), sAcademicYear).DataSource.Data;
                    objViewModel.liFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHosteFeePaidReport), sAcademicYear).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "BindHostelFeePaidStatus", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "BindHostelFeePaidStatus", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        #endregion

        #region Main Head Wise Fee Report By Date
        // NOT IN USE
        public ActionResult MainHeadWiseFeeReportByDate()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchFrequency = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    var FetchProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;
                    var FetchClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data;
                    var FetchClassLevel = (List<SupClassLevel>)CMSHandler.FetchData<SupClassLevel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassLevel)).DataSource.Data;
                    var FetchShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    var FetchAcademicyear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicyear != null && FetchAcademicyear.Count > 0)
                        objViewModel.liAcademicYear = new SelectList(FetchAcademicyear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    if (FetchProgrammeMode != null && FetchProgrammeMode.Count > 0)
                        objViewModel.liProgrammeModeList = new SelectList(FetchProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                    if (FetchClassLevel != null && FetchClassLevel.Count > 0)
                        objViewModel.liClassLevel = new SelectList(FetchClassLevel, Common.SUP_CLASS_LEVEL.CLASSLEVELID, Common.SUP_CLASS_LEVEL.CLASSLEVEL);
                    if (FetchClassYear != null && FetchClassYear.Count > 0)
                        objViewModel.liClassYear = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                    if (FetchShift != null && FetchShift.Count > 0)
                        objViewModel.liShift = new SelectList(FetchShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "MainHeadWiseFeeReportByDate", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "MainHeadWiseFeeReportByDate", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindMainHeadWiseFeeCollectionByDate(string sDateFrom, string sDateTo, string AcYear, string Frequency, string ProgrammeMode, string Head, string ClassYear, string ClassLevel, string Shift)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = AcYear;
            var objFeeStudentAccountModel = new FEE_TRANSACTION();
            string sSQL = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objViewModel.FeeTransaction.DATE_FROM = sDateFrom;
                    objViewModel.FeeTransaction.DATE_TO = sDateTo;
                    objFeeStudentAccountModel.FREQUENCY = Frequency;
                    objFeeStudentAccountModel.PROGRAMME_MODE = ProgrammeMode;
                    objFeeStudentAccountModel.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(sDateFrom);
                    objFeeStudentAccountModel.DATE_TO = CommonMethods.ConvertdatetoyearFromat(sDateTo);
                    objFeeStudentAccountModel.SHIFT = Shift;
                    objFeeStudentAccountModel.CLASS_YEAR = ClassYear;
                    objFeeStudentAccountModel.CLASS_LEVEL = ClassLevel;
                    var FetchDates = (List<DayOrderCalender>)CMSHandler.FetchData<DayOrderCalender>(objFeeStudentAccountModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDatesByDates), sAcademicYear).DataSource.Data;
                    var FetchProgrammeGroupIds = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objFeeStudentAccountModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupIds), sAcademicYear).DataSource.Data;
                    var FetchCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupMainHeadById).Replace(Common.Delimiter.QUS + Common.SUP_FEE_MAIN_HEAD.MAIN_HEAD_ID, Head);
                    var FetchSupMainHeads = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(null, sSQL, sAcademicYear).DataSource.Data;
                    sSQL = string.Empty;
                    sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMainHeadwiseFeeReportByDate).Replace(Common.Delimiter.QUS + Common.SUP_FEE_MAIN_HEAD.MAIN_HEAD_ID, Head);
                    var FetchHeadWiseFeeDetails = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, sSQL, sAcademicYear).DataSource.Data;
                    if (FetchHeadWiseFeeDetails != null && FetchHeadWiseFeeDetails.Count > 0)
                        objViewModel.liFeeTransaction = FetchHeadWiseFeeDetails;
                    if (FetchCollegeDetails != null && FetchCollegeDetails.Count > 0)
                        objViewModel.liCollegeDetails = FetchCollegeDetails;
                    if (FetchDates != null && FetchDates.Count > 0)
                        objViewModel.liDate = FetchDates;
                    if (FetchSupMainHeads != null && FetchSupMainHeads.Count > 0)
                        objViewModel.lstSupMainHead = FetchSupMainHeads;
                    if (FetchProgrammeGroupIds != null && FetchProgrammeGroupIds.Count > 0)
                        objViewModel.liCreditAndDebit = FetchProgrammeGroupIds;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "BindMainHeadWiseFeeCollectionByDate", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "BindMainHeadWiseFeeCollectionByDate", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        #endregion

        #region Head Wise Fee Report By Date
        // NOT IN USE
        public ActionResult HeadWiseFeeReportByDate()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchFrequency = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    var FetchProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;
                    var FetchClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data;
                    var FetchClassLevel = (List<SupClassLevel>)CMSHandler.FetchData<SupClassLevel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassLevel)).DataSource.Data;
                    var FetchAcademicyear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicyear != null && FetchAcademicyear.Count > 0)
                        objViewModel.liAcademicYear = new SelectList(FetchAcademicyear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    if (FetchProgrammeMode != null && FetchProgrammeMode.Count > 0)
                        objViewModel.liProgrammeModeList = new SelectList(FetchProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                    if (FetchClassLevel != null && FetchClassLevel.Count > 0)
                        objViewModel.liClassLevel = new SelectList(FetchClassLevel, Common.SUP_CLASS_LEVEL.CLASSLEVELID, Common.SUP_CLASS_LEVEL.CLASSLEVEL);
                    if (FetchClassYear != null && FetchClassYear.Count > 0)
                        objViewModel.liClassYear = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "HeadWiseFeeReportByDate", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "HeadWiseFeeReportByDate", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindHeadWiseFeeCollectionByDate(string sDateFrom, string sDateTo, string AcYear, string Frequency, string ProgrammeMode, string Head, string ClassYear, string ClassLevel)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = AcYear;
            var objFeeStudentAccountModel = new FEE_TRANSACTION();
            string sSQL = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objViewModel.FeeTransaction.DATE_FROM = sDateFrom;
                    objViewModel.FeeTransaction.DATE_TO = sDateTo;
                    objFeeStudentAccountModel.FREQUENCY = Frequency;
                    objFeeStudentAccountModel.PROGRAMME_MODE = ProgrammeMode;
                    objFeeStudentAccountModel.CLASS_LEVEL = ClassLevel;
                    objFeeStudentAccountModel.CLASS_YEAR = ClassYear;
                    objFeeStudentAccountModel.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(sDateFrom);
                    objFeeStudentAccountModel.DATE_TO = CommonMethods.ConvertdatetoyearFromat(sDateTo);
                    var FetchDates = (List<DayOrderCalender>)CMSHandler.FetchData<DayOrderCalender>(objFeeStudentAccountModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDatesByDates), sAcademicYear).DataSource.Data;
                    var FetchStudentIds = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(objFeeStudentAccountModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentIds), sAcademicYear).DataSource.Data;
                    var FetchCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupHeadsById).Replace(Common.Delimiter.QUS + Common.SUP_HEAD.HEAD_ID, Head);
                    var FetchSupHeads = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(null, sSQL, sAcademicYear).DataSource.Data;
                    sSQL = string.Empty;
                    sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHeadwiseFeeReport).Replace(Common.Delimiter.QUS + Common.SUP_HEAD.HEAD, Head);
                    var FetchHeadWiseFeeDetails = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(objFeeStudentAccountModel, sSQL, sAcademicYear).DataSource.Data;
                    if (FetchHeadWiseFeeDetails != null && FetchHeadWiseFeeDetails.Count > 0)
                        objViewModel.liFeeTransaction = FetchHeadWiseFeeDetails;
                    if (FetchCollegeDetails != null && FetchCollegeDetails.Count > 0)
                        objViewModel.liCollegeDetails = FetchCollegeDetails;
                    if (FetchDates != null && FetchDates.Count > 0)
                        objViewModel.liDate = FetchDates;
                    if (FetchSupHeads != null && FetchSupHeads.Count > 0)
                        objViewModel.lstSupHead = FetchSupHeads;
                    if (FetchStudentIds != null && FetchStudentIds.Count > 0)
                        objViewModel.liCreditAndDebit = FetchStudentIds;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Fee", "BindHeadWiseFeeCollectionByDate", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "BindHeadWiseFeeCollectionByDate", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        #endregion

        #region Exam Fee Paid and Unpaid Report
        public ActionResult ExamFeePaidUnpaidReport()
        {
            FeeReportViewModel ObjViewModel = new FeeReportViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        ObjViewModel.AcademicYearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);

                    var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByAcademicYear), sAcademicYear).DataSource.Data;
                    if (FetchClass != null && FetchClass.Count > 0)
                        ObjViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "ExamFeePaidUnpaidReport", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult ExamFeeReport()
        {
            return RedirectToAction("ExamFeePaidUnpaidReport");
        }

        public JsonResult FetchExamFeeFrequencyByAcademicYear(string Academicyear)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;
                    string query = @"SELECT FFF.FEE_FREQUENCY_FEE_MODE_ID AS FREQUENCY_ID, SFF.FREQUENCY_NAME AS FREQUENCY_NAME
                                     FROM FEE_FREQUENCY_FEE_MODE AS FFF
                                     INNER JOIN SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                     WHERE FFF.FEE_MODE = '1' AND FFF.ACADEMIC_YEAR = @AC_YEAR";

                    var lstFreq = DapperHandler.FetchAll<dynamic>(query, new { AC_YEAR = sAcademicYear }).ToList();
                    if (lstFreq != null && lstFreq.Count > 0)
                    {
                        foreach (var item in lstFreq)
                        {
                            objResult.sResult += "<option value='" + item.FREQUENCY_ID + "'>" + item.FREQUENCY_NAME + "</option>";
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
                    objHandler.WriteError("Fee", "FetchExamFeeFrequencyByAcademicYear", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchExamFeeHeadsByFrequencyId(string sFrequencyId, string Academicyear)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;
                    string query = @"SELECT 
                                        MH.HEAD_ID AS HEAD_ID,
                                        SH.HEAD AS HEAD_NAME,
                                        SH.HEAD_CODE AS HEAD_CODE
                                    FROM
                                        FEE_MAIN_HEADS AS MH
                                            INNER JOIN
                                        SUP_HEAD AS SH ON SH.HEAD_ID = MH.HEAD_ID
                                    WHERE
                                        MH.ACADEMIC_YEAR = @AC_YEAR
                                            AND MH.FREQUENCY_ID = @FREQUENCY_ID
                                            AND MH.IS_DELETED != 1
                                    GROUP BY MH.HEAD_ID";

                    var lstHeads = DapperHandler.FetchAll<dynamic>(query, new { AC_YEAR = sAcademicYear, FREQUENCY_ID = sFrequencyId }).ToList();
                    if (lstHeads != null && lstHeads.Count > 0)
                    {
                        foreach (var item in lstHeads)
                        {
                            objResult.sResult += "<option value='" + item.HEAD_ID + "'>" + item.HEAD_NAME + " (" + item.HEAD_CODE + ")</option>";
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
                    objHandler.WriteError("Fee", "FetchExamFeeHeadsByFrequencyId", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BindExamFeeReport(string sTab, string sAcYear, string FromDate, string ToDate, string sFrequency, string sHead, string sClassId)
        {
            FeeReportViewModel ObjViewModel = new FeeReportViewModel();
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = sAcYear;
            if (string.IsNullOrEmpty(sAcademicYear) && Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            }

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("AC_YEAR", sAcademicYear);

                    string sSQL = "";
                    if (sTab == "student_paid")
                    {
                        sSQL = @"SELECT 
                                    FT.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    SPI.REGISTER_NO,
                                    UPPER(CONCAT(IFNULL(SPI.FIRST_NAME, ''), ' ', IFNULL(SPI.LAST_NAME, ''))) AS FIRST_NAME,
                                    CLS.CLASS_ID,
                                    CLS.CLASS_NAME,
                                    SPI.STU_MOBILENO AS MOBILE_NO,
                                    SH.HEAD_ID,
                                    SH.HEAD_CODE,
                                    SH.HEAD AS HEAD_NAME,
                                    FFF.FREQUENCY_ID,
                                    FF.FREQUENCY_NAME,
                                    FC.PAID_AMOUNT AS DEBIT,
                                    0 AS CREDIT,
                                    0 AS BALANCE,
                                    DATE_FORMAT(FT.PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,
                                    FT.RECEIPT_NO,
                                    PM.PAYMENT_MODE,
                                    FT.ACADEMIC_YEAR
                                FROM
                                    FEE_TRANSACTION AS FT
                                        INNER JOIN
                                    FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = FT.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID AND SC.ACADEMIC_YEAR = FT.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FT.FREQUENCY AND FFF.ACADEMIC_YEAR = FT.ACADEMIC_YEAR
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FREQUENCY_TYPE_ID = '1'
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FC.HEAD
                                        LEFT JOIN
                                    SUP_PAYMENT_MODE AS PM ON PM.PAYMENT_MODE_ID = FT.PAYMENT_MODE
                                WHERE
                                    FT.IS_DELETED != 1
                                    AND FC.PAID_AMOUNT > 0
                                    AND FT.ACADEMIC_YEAR = @AC_YEAR";

                        if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                        {
                            string dFrom = CommonMethods.ConvertdatetoyearFromat(FromDate);
                            string dTo = CommonMethods.ConvertdatetoyearFromat(ToDate);
                            sSQL += " AND FT.PAYMENT_DATE BETWEEN @DATE_FROM AND @DATE_TO";
                            parameters.Add("DATE_FROM", dFrom);
                            parameters.Add("DATE_TO", dTo);
                        }

                        if (!string.IsNullOrEmpty(sFrequency) && sFrequency != "0" && sFrequency != "null")
                        {
                            sSQL += " AND FFF.FEE_FREQUENCY_FEE_MODE_ID = @FREQUENCY_ID";
                            parameters.Add("FREQUENCY_ID", sFrequency);
                        }

                        if (!string.IsNullOrEmpty(sHead) && sHead != "0" && sHead != "null")
                        {
                            if (sHead.Contains(","))
                            {
                                var headArr = sHead.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(h => h.Trim()).ToList();
                                sSQL += " AND FC.HEAD IN @HEAD_IDS";
                                parameters.Add("HEAD_IDS", headArr);
                            }
                            else
                            {
                                sSQL += " AND FC.HEAD = @HEAD_ID";
                                parameters.Add("HEAD_ID", sHead);
                            }
                        }

                        if (!string.IsNullOrEmpty(sClassId) && sClassId != "0" && sClassId != "null")
                        {
                            sSQL += " AND CLS.CLASS_ID = @CLASS_ID";
                            parameters.Add("CLASS_ID", sClassId);
                        }

                        sSQL += " ORDER BY FT.PAYMENT_DATE DESC, CLS.CLASS_NAME, SPI.ROLL_NO, FF.FREQUENCY_NAME;";
                    }
                    else if (sTab == "student_balance")
                    {
                        sSQL = @"SELECT 
                                    FS.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    SPI.REGISTER_NO,
                                    UPPER(CONCAT(IFNULL(SPI.FIRST_NAME, ''), ' ', IFNULL(SPI.LAST_NAME, ''))) AS FIRST_NAME,
                                    CLS.CLASS_ID,
                                    CLS.CLASS_NAME,
                                    SPI.STU_MOBILENO AS MOBILE_NO,
                                    SH.HEAD_ID,
                                    SH.HEAD_CODE,
                                    SH.HEAD AS HEAD_NAME,
                                    FFF.FREQUENCY_ID,
                                    FF.FREQUENCY_NAME,
                                    SUM(IFNULL(FS.CREDIT, 0)) AS CREDIT,
                                    SUM(IFNULL(FS.DEBIT, 0)) AS DEBIT,
                                    (SUM(IFNULL(FS.CREDIT, 0)) - SUM(IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    FS.ACADEMIC_YEAR
                                FROM
                                    FEE_STUDENT_ACCOUNT AS FS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = FS.STUDENT_ID AND SPI.IS_DELETED != 1 AND SPI.IS_LEFT != 1
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID AND SC.ACADEMIC_YEAR = FS.ACADEMIC_YEAR AND SC.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID AND FFF.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FREQUENCY_TYPE_ID = '1'
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FS.HEAD
                                WHERE
                                    FS.IS_DELETED != 1
                                    AND FS.ACADEMIC_YEAR = @AC_YEAR";

                        if (!string.IsNullOrEmpty(sFrequency) && sFrequency != "0" && sFrequency != "null")
                        {
                            sSQL += " AND FFF.FEE_FREQUENCY_FEE_MODE_ID = @FREQUENCY_ID";
                            parameters.Add("FREQUENCY_ID", sFrequency);
                        }

                        if (!string.IsNullOrEmpty(sHead) && sHead != "0" && sHead != "null")
                        {
                            if (sHead.Contains(","))
                            {
                                var headArr = sHead.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(h => h.Trim()).ToList();
                                sSQL += " AND FS.HEAD IN @HEAD_IDS";
                                parameters.Add("HEAD_IDS", headArr);
                            }
                            else
                            {
                                sSQL += " AND FS.HEAD = @HEAD_ID";
                                parameters.Add("HEAD_ID", sHead);
                            }
                        }

                        if (!string.IsNullOrEmpty(sClassId) && sClassId != "0" && sClassId != "null")
                        {
                            sSQL += " AND CLS.CLASS_ID = @CLASS_ID";
                            parameters.Add("CLASS_ID", sClassId);
                        }

                        sSQL += @" GROUP BY FS.STUDENT_ID, FS.FREQUENCY_ID, FS.HEAD
                                   HAVING BALANCE > 0
                                   ORDER BY CLS.CLASS_NAME, SPI.ROLL_NO, FF.FREQUENCY_NAME;";
                    }
                    else if (sTab == "student_freq")
                    {
                        sSQL = @"SELECT 
                                    FS.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    SPI.REGISTER_NO,
                                    UPPER(CONCAT(IFNULL(SPI.FIRST_NAME, ''), ' ', IFNULL(SPI.LAST_NAME, ''))) AS FIRST_NAME,
                                    CLS.CLASS_ID,
                                    CLS.CLASS_NAME,
                                    SPI.STU_MOBILENO AS MOBILE_NO,
                                    SH.HEAD_ID,
                                    SH.HEAD_CODE,
                                    SH.HEAD AS HEAD_NAME,
                                    FFF.FREQUENCY_ID,
                                    FF.FREQUENCY_NAME,
                                    SUM(IFNULL(FS.CREDIT, 0)) AS CREDIT,
                                    SUM(IFNULL(FS.DEBIT, 0)) AS DEBIT,
                                    (SUM(IFNULL(FS.CREDIT, 0)) - SUM(IFNULL(FS.DEBIT, 0))) AS BALANCE,
                                    FS.ACADEMIC_YEAR
                                FROM
                                    FEE_STUDENT_ACCOUNT AS FS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = FS.STUDENT_ID AND SPI.IS_DELETED != 1 AND SPI.IS_LEFT != 1
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID AND SC.ACADEMIC_YEAR = FS.ACADEMIC_YEAR AND SC.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID AND FFF.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                        INNER JOIN
                                    SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FREQUENCY_TYPE_ID = '1'
                                        INNER JOIN
                                    SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        LEFT JOIN
                                    SUP_HEAD AS SH ON SH.HEAD_ID = FS.HEAD
                                WHERE
                                    FS.IS_DELETED != 1
                                    AND FS.ACADEMIC_YEAR = @AC_YEAR";

                        if (!string.IsNullOrEmpty(sFrequency) && sFrequency != "0" && sFrequency != "null")
                        {
                            sSQL += " AND FFF.FEE_FREQUENCY_FEE_MODE_ID = @FREQUENCY_ID";
                            parameters.Add("FREQUENCY_ID", sFrequency);
                        }

                        if (!string.IsNullOrEmpty(sHead) && sHead != "0" && sHead != "null")
                        {
                            if (sHead.Contains(","))
                            {
                                var headArr = sHead.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(h => h.Trim()).ToList();
                                sSQL += " AND FS.HEAD IN @HEAD_IDS";
                                parameters.Add("HEAD_IDS", headArr);
                            }
                            else
                            {
                                sSQL += " AND FS.HEAD = @HEAD_ID";
                                parameters.Add("HEAD_ID", sHead);
                            }
                        }

                        if (!string.IsNullOrEmpty(sClassId) && sClassId != "0" && sClassId != "null")
                        {
                            sSQL += " AND CLS.CLASS_ID = @CLASS_ID";
                            parameters.Add("CLASS_ID", sClassId);
                        }

                        sSQL += @" GROUP BY FS.STUDENT_ID, FS.FREQUENCY_ID
                                   ORDER BY CLS.CLASS_NAME, SPI.ROLL_NO, FF.FREQUENCY_NAME;";
                    }

                    ObjViewModel.liExamFeeReport = DapperHandler.FetchAll<EXAM_FEE_REPORT>(sSQL, parameters) ?? new List<EXAM_FEE_REPORT>();
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "BindExamFeeReport", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            ViewBag.SelectedTab = sTab;
            return View(ObjViewModel);
        }
        #endregion

        #region Bulk Fee Payment
        public ActionResult BulkFeePayment()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new FeeTransactionViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ObjViewModel.liFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId },
                         ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchFeeTransaction), sAcademicYear).DataSource.Data;
                ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
            }
            return View(ObjViewModel);
        }
        #endregion

        #region All Office Fee Report 
        //Changed.. not in use Term Fee Report
        public ActionResult MainHeadwiseReportForCollege()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeeTransactionViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    ObjViewModel.liAcademicYear = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                var FetchClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear), sAcademicYear).DataSource.Data;
                if (FetchClassYear != null && FetchClassYear.Count > 0)
                    ObjViewModel.liClassYear = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.CP_CLASSES_2017.CLASS_YEAR);
                var FetchProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;
                if (FetchProgrammeMode != null && FetchProgrammeMode.Count > 0)
                    ObjViewModel.liProgrammeModeList = new SelectList(FetchProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                var FetchShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                if (FetchShift != null && FetchShift.Count > 0)
                    ObjViewModel.liShift = new SelectList(FetchShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                return View(ObjViewModel);
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
        }
        public ActionResult BindHeadwiseFeeReport(string sAcYear, string sProgrammeGroupId, string sFrequencyMode, string sFrequencyType, string sClassYear, string sDateFrom, string sDateTo, string sShift, string sProgrammeMode)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeeTransactionViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new FEE_REPORT()
                {
                    SHIFT = sShift,
                    PROGRAMME_MODE = sProgrammeMode,
                    FREQUENCY = sFrequencyMode,
                    FREQUENCY_TYPE = sFrequencyType,
                    ACADEMIC_YEAR = sAcYear,
                    FEE_ROOT_ID = Common.FeeRoot.CollegeFee,
                    CLASS_YEAR = sClassYear,
                    DATE_FROM = CommonMethods.ConvertdatetoyearFromat(sDateFrom),
                    DATE_TO = CommonMethods.ConvertdatetoyearFromat(sDateTo)
                };

                // Fetch Student Details ..
                string sSQL = string.Empty;
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByProgrammeGroupId).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                ObjViewModel.lstStudentPersonalInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(ObjModel, sSQL, sAcYear).DataSource.Data;

                // Fetch Main Heads ..
                sSQL = string.Empty;
                sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMainHeadsByPRogrammeGroupIdAndFrequencyId).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                ObjViewModel.lstSupMainHead = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(ObjModel, sSQL, sAcYear).DataSource.Data;

                // Fetch SubHeads By ProgrammeGroupId ..
                sSQL = string.Empty;
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHeadsByFeeRootIdAndProgrammeGroupId).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                ObjViewModel.lstSupHead = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(ObjModel, sSQL, sAcYear).DataSource.Data;

                // Fetch Headswise Fee Report ..
                sSQL = string.Empty;
                //sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMainHeadwiseFeeReportByFeeRootId).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);               
                //sSQL = @"CALL TEST('2018',1,14,1,'2019-01-18','2019-02-27');";
                sSQL = @"CALL FetchTermFeeReport('" + sAcYear + "','" + Common.FeeRoot.CollegeFee + "','" + sFrequencyMode + "','" + sClassYear + "','" + CommonMethods.ConvertdatetoyearFromat(sDateFrom) + "','" + CommonMethods.ConvertdatetoyearFromat(sDateTo) + "','" + sShift + "','" + sProgrammeMode + "');";
                ObjViewModel.liFeeReport = (List<FEE_REPORT>)CMSHandler.FetchData<FEE_REPORT>(ObjModel, sSQL).DataSource.Data;

            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        //Term Fee Report Fee Category
        public ActionResult MainHeadwiseReportByFeecategoryForCollege()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeeTransactionViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    ObjViewModel.liAcademicYear = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                var FetchClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear), sAcademicYear).DataSource.Data;
                if (FetchClassYear != null && FetchClassYear.Count > 0)
                    ObjViewModel.liClassYear = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.CP_CLASSES_2017.CLASS_YEAR);
                var FetchProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;
                if (FetchProgrammeMode != null && FetchProgrammeMode.Count > 0)
                    ObjViewModel.liProgrammeModeList = new SelectList(FetchProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                var FetchShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                if (FetchShift != null && FetchShift.Count > 0)
                    ObjViewModel.liShift = new SelectList(FetchShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                var FectchFeeCategory = (List<Sup_Fee_Category>)CMSHandler.FetchData<Sup_Fee_Category>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeeCategory)).DataSource.Data;
                if (FectchFeeCategory != null && FectchFeeCategory.Count > 0)
                    ObjViewModel.FeeCategory = new SelectList(FectchFeeCategory, Common.SUP_FEE_CATEGORY.FEE_CATEGORY_ID, Common.SUP_FEE_CATEGORY.FEE_CATEGORY);
                var FetchApplicationType = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType)).DataSource.Data;
                if (FetchApplicationType != null && FetchApplicationType.Count > 0)
                    ObjViewModel.liApplicationType = new SelectList(FetchApplicationType, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                return View(ObjViewModel);
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
        }
        public ActionResult BindHeadwiseFeeReportByFeecategory(string sAcYear, string sProgrammeGroupId, string sFrequency, string sFrequencyType, string sClassYear, string sFeeCategoryId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeeTransactionViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new FEE_REPORT()
                {
                    FREQUENCY_TYPE = sFrequencyType,
                    FREQUENCY = sFrequency,
                    ACADEMIC_YEAR = sAcYear,
                    FEE_ROOT_ID = Common.FeeRoot.CollegeFee,
                    CLASS_YEAR = sClassYear,
                    // DATE_FROM = CommonMethods.ConvertdatetoyearFromat(sDateFrom),
                    // DATE_TO = CommonMethods.ConvertdatetoyearFromat(sDateTo),
                    FEE_CATEGORY_ID = sFeeCategoryId
                };

                // Fetch Student Details ..
                string sSQL = string.Empty;
                sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentbyProgrammeGroupAndClassYearForFee).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                ObjViewModel.lstStudentPersonalInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(ObjModel, sSQL, sAcYear).DataSource.Data;

                // Fetch Main Heads ..
                sSQL = string.Empty;
                sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMainHeadsByPRogrammeGroupIdAndFrequencyIdAndFeeCategory).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.FEE_MAIN_HEADS.FEE_CATEGORY_ID, ObjModel.FEE_CATEGORY_ID);
                ObjViewModel.lstSupMainHead = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(ObjModel, sSQL, sAcYear).DataSource.Data;

                // Fetch SubHeads By ProgrammeGroupId ..
                sSQL = string.Empty;
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHeadsByFeeRootIdAndProgrammeGroupIdAndFeeCategory).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.FEE_MAIN_HEADS.FEE_CATEGORY_ID, ObjModel.FEE_CATEGORY_ID);
                ObjViewModel.lstSupHead = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(ObjModel, sSQL, sAcYear).DataSource.Data;

                // Fetch Headswise Fee Report ..
                sSQL = string.Empty;
                sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMainHeadwiseFeeReportByFeeRootIdAndFeeCategory).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.FEE_MAIN_HEADS.FEE_CATEGORY_ID, ObjModel.FEE_CATEGORY_ID);
                ObjViewModel.liFeeReport = (List<FEE_REPORT>)CMSHandler.FetchData<FEE_REPORT>(ObjModel, sSQL, sAcYear).DataSource.Data;

            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        //Date Wise Report  Fee Category
        public ActionResult MainHeadwiseReportByDate()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeeTransactionViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    ObjViewModel.liAcademicYear = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                var FetchClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear), sAcademicYear).DataSource.Data;
                if (FetchClassYear != null && FetchClassYear.Count > 0)
                    ObjViewModel.liClassYear = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.CP_CLASSES_2017.CLASS_YEAR);
                var FetchProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;
                if (FetchProgrammeMode != null && FetchProgrammeMode.Count > 0)
                    ObjViewModel.liProgrammeModeList = new SelectList(FetchProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                var FetchShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                if (FetchShift != null && FetchShift.Count > 0)
                    ObjViewModel.liShift = new SelectList(FetchShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                var FectchFeeCategory = (List<Sup_Fee_Category>)CMSHandler.FetchData<Sup_Fee_Category>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeeCategory)).DataSource.Data;
                if (FectchFeeCategory != null && FectchFeeCategory.Count > 0)
                    ObjViewModel.FeeCategory = new SelectList(FectchFeeCategory, Common.SUP_FEE_CATEGORY.FEE_CATEGORY_ID, Common.SUP_FEE_CATEGORY.FEE_CATEGORY);
                return View(ObjViewModel);
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
        }
        public ActionResult BindHeadwiseReportByDate(string sAcYear, string sProgrammeGroupId, string sFrequency, string sFrequencyType, string sDateFrom, string sDateTo, string sProgrameMode, string sFeeCategoryId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeeTransactionViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new FEE_REPORT()
                {
                    FREQUENCY = sFrequency,
                    ACADEMIC_YEAR = sAcYear,
                    FEE_ROOT_ID = Common.FeeRoot.CollegeFee,
                    DATE_FROM = CommonMethods.ConvertdatetoyearFromat(sDateFrom),
                    DATE_TO = CommonMethods.ConvertdatetoyearFromat(sDateTo),
                    FEE_CATEGORY_ID = sFeeCategoryId
                };

                // Fetch Student Details ..
                string sSQL = string.Empty;
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByProgrammeGroupIdForDateReport).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                ObjViewModel.lstStudentPersonalInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(ObjModel, sSQL, sAcYear).DataSource.Data;

                // Fetch Main Heads By ProgrammeGroupId ..
                sSQL = string.Empty;
                sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMainHeadsByPRogrammeGroupIdAndFrequencyIdAndFeeCategory).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.FEE_MAIN_HEADS.FEE_CATEGORY_ID, ObjModel.FEE_CATEGORY_ID);
                ObjViewModel.lstSupMainHead = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(ObjModel, sSQL, sAcYear).DataSource.Data;

                // Fetch SubHeads By ProgrammeGroupId ..
                sSQL = string.Empty;
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHeadsByFeeRootIdAndProgrammeGroupIdAndFeeCategory).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.FEE_MAIN_HEADS.FEE_CATEGORY_ID, ObjModel.FEE_CATEGORY_ID);
                ObjViewModel.lstSupHead = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(ObjModel, sSQL, sAcYear).DataSource.Data;

                // Fetch Headswise Fee Report ..
                sSQL = string.Empty;
                sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMainHeadwiseFeeReportByFeeRootIdForDateWiseReport).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.FEE_MAIN_HEADS.FEE_CATEGORY_ID, ObjModel.FEE_CATEGORY_ID);
                ObjViewModel.liFeeReport = (List<FEE_REPORT>)CMSHandler.FetchData<FEE_REPORT>(ObjModel, sSQL, sAcYear).DataSource.Data;
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        //DateWiseAbstract Fee Category
        public ActionResult HeadWiseAbstractByDate()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeeTransactionViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    ObjViewModel.liAcademicYear = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                var FetchClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear), sAcademicYear).DataSource.Data;
                if (FetchClassYear != null && FetchClassYear.Count > 0)
                    ObjViewModel.liClassYear = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.CP_CLASSES_2017.CLASS_YEAR);
                var FetchProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;
                if (FetchProgrammeMode != null && FetchProgrammeMode.Count > 0)
                    ObjViewModel.liProgrammeModeList = new SelectList(FetchProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                var FetchShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                if (FetchShift != null && FetchShift.Count > 0)
                    ObjViewModel.liShift = new SelectList(FetchShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                var FectchFeeCategory = (List<Sup_Fee_Category>)CMSHandler.FetchData<Sup_Fee_Category>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeeCategory)).DataSource.Data;
                if (FectchFeeCategory != null && FectchFeeCategory.Count > 0)
                    ObjViewModel.FeeCategory = new SelectList(FectchFeeCategory, Common.SUP_FEE_CATEGORY.FEE_CATEGORY_ID, Common.SUP_FEE_CATEGORY.FEE_CATEGORY);
                return View(ObjViewModel);
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
        }
        public ActionResult BindHeadWiseAbstractByDate(string sAcYear, string sProgrammeGroupId, string sFrequency, string sFrequencyType, string sDateFrom, string sDateTo, string sProgrameMode, string sFeeCategoryId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeeTransactionViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new FEE_REPORT()
                {
                    FREQUENCY_TYPE = sFrequencyType,
                    FREQUENCY = sFrequency,
                    ACADEMIC_YEAR = sAcYear,
                    FEE_ROOT_ID = Common.FeeRoot.CollegeFee,
                    DATE_FROM = CommonMethods.ConvertdatetoyearFromat(sDateFrom),
                    DATE_TO = CommonMethods.ConvertdatetoyearFromat(sDateTo),
                    PROGRAMME_MODE = sProgrameMode,
                    FEE_CATEGORY_ID = sFeeCategoryId
                };

                // Fetch Student Details ..
                string sSQL = string.Empty;
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByProgrammeGroupIdForDateReport).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                ObjViewModel.lstStudentPersonalInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(ObjModel, sSQL, sAcYear).DataSource.Data;

                // Fetch Main Heads ..
                sSQL = string.Empty;
                sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMainHeadsByPRogrammeGroupIdAndFrequencyIdAndFeeCategory).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.FEE_MAIN_HEADS.FEE_CATEGORY_ID, ObjModel.FEE_CATEGORY_ID);
                ObjViewModel.lstSupMainHead = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(ObjModel, sSQL, sAcYear).DataSource.Data;

                // Fetch SubHeads By ProgrammeGroupId ..
                sSQL = string.Empty;
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHeadsByFeeRootIdAndProgrammeGroupIdAndFeeCategory).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.FEE_MAIN_HEADS.FEE_CATEGORY_ID, ObjModel.FEE_CATEGORY_ID);
                ObjViewModel.lstSupHead = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(ObjModel, sSQL, sAcYear).DataSource.Data;

                // Fetch Headswise Fee Report ..
                sSQL = string.Empty;
                sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMainHeadwiseFeeReportByFeeRootIdForDateWiseReport).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.FEE_MAIN_HEADS.FEE_CATEGORY_ID, ObjModel.FEE_CATEGORY_ID);
                ObjViewModel.liFeeReport = (List<FEE_REPORT>)CMSHandler.FetchData<FEE_REPORT>(ObjModel, sSQL, sAcYear).DataSource.Data;

            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        // TermFee Abstract Fee Category
        public ActionResult TermFeeAbstractReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeeTransactionViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        ObjViewModel.liAcademicYear = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    var FetchClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear), sAcademicYear).DataSource.Data;
                    if (FetchClassYear != null && FetchClassYear.Count > 0)
                        ObjViewModel.liClassYear = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.CP_CLASSES_2017.CLASS_YEAR);
                    var FetchProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;
                    if (FetchProgrammeMode != null && FetchProgrammeMode.Count > 0)
                        ObjViewModel.liProgrammeModeList = new SelectList(FetchProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                    var FetchShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (FetchShift != null && FetchShift.Count > 0)
                        ObjViewModel.liShift = new SelectList(FetchShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    var FectchFeeCategory = (List<Sup_Fee_Category>)CMSHandler.FetchData<Sup_Fee_Category>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeeCategory)).DataSource.Data;
                    if (FectchFeeCategory != null && FectchFeeCategory.Count > 0)
                        ObjViewModel.FeeCategory = new SelectList(FectchFeeCategory, Common.SUP_FEE_CATEGORY.FEE_CATEGORY_ID, Common.SUP_FEE_CATEGORY.FEE_CATEGORY);
                    var FetchApplicationType = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType)).DataSource.Data;
                    if (FetchApplicationType != null && FetchApplicationType.Count > 0)
                        ObjViewModel.liApplicationType = new SelectList(FetchApplicationType, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "TermFeeAbstractReport", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindTermFeeAbstractReport(string sAcYear, string sProgrammeGroupId, string sProgrammeLevelId, string sFrequencyId, string sFrequencyType, string sFeeCategoryId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeeTransactionViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new FEE_REPORT()
                    {
                        FREQUENCY = sFrequencyId,
                        FREQUENCY_TYPE = sFrequencyId,
                        ACADEMIC_YEAR = sAcYear,
                        FEE_ROOT_ID = Common.FeeRoot.CollegeFee,
                        CLASS_YEAR = sProgrammeLevelId,
                        FEE_CATEGORY_ID = sFeeCategoryId
                    };
                    // Fetch Student Details ..
                    string sSQL = string.Empty;
                    sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentbyProgrammeGroupAndClassYearForFee).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                    ObjViewModel.lstStudentPersonalInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(ObjModel, sSQL, sAcYear).DataSource.Data;

                    // Fetch Main Heads ..
                    sSQL = string.Empty;
                    sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMainHeadsByPRogrammeGroupIdAndFrequencyIdAndFeeCategory).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.FEE_MAIN_HEADS.FEE_CATEGORY_ID, ObjModel.FEE_CATEGORY_ID);
                    ObjViewModel.lstSupMainHead = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(ObjModel, sSQL, sAcYear).DataSource.Data;

                    // Fetch SubHeads By ProgrammeGroupId ..
                    sSQL = string.Empty;
                    sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHeadsByFeeRootIdAndProgrammeGroupIdAndFeeCategory).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.FEE_MAIN_HEADS.FEE_CATEGORY_ID, ObjModel.FEE_CATEGORY_ID);
                    ObjViewModel.lstSupHead = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(ObjModel, sSQL, sAcYear).DataSource.Data;

                    // Fetch Fee Abstract Report ,..
                    sSQL = string.Empty;
                    sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTermFeeAbstractByProgrammeGroupIds).Replace(Common.Delimiter.QUS + Common.CP_COURSE_PATTERN.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.FEE_MAIN_HEADS.FEE_CATEGORY_ID, ObjModel.FEE_CATEGORY_ID);
                    ObjViewModel.liFeeReport = (List<FEE_REPORT>)CMSHandler.FetchData<FEE_REPORT>(ObjModel, sSQL, sAcYear).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "TermFeeAbstractReport", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        #endregion

        #region FeeStructureReportByProgramme
        public ActionResult FeeStructureReportByProgramme()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    var SupShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift), sAcademicYear).DataSource.Data;
                    var SupProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode), sAcademicYear).DataSource.Data;
                    var FetchProgrammeGroup = (List<SUP_APPTYPE_PROG_GROUPS>)CMSHandler.FetchData<SUP_APPTYPE_PROG_GROUPS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                    var SupFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeMode), sAcademicYear).DataSource.Data;
                    var FetchClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear), sAcademicYear).DataSource.Data;
                    if (FetchProgrammeGroup != null && FetchProgrammeGroup.Count > 0)
                        objViewModel.ProgrammeGroupList = new SelectList(FetchProgrammeGroup, Common.SUP_APPTYPE_PROG_GROUPS.PROGRAMME_GROUP_ID, Common.SUP_APPTYPE_PROG_GROUPS.PROGRAMME_NAME);
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    if (FetchClassYear != null && FetchClassYear.Count > 0)
                        objViewModel.ClassYearList = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.CP_CLASSES_2017.CLASS_YEAR);
                    if (SupShift != null && SupShift.Count > 0)
                        objViewModel.ShiftList = new SelectList(SupShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    if (SupProgrammeMode != null && SupProgrammeMode.Count > 0)
                        objViewModel.ProgrammeModeList = new SelectList(SupProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                    if (SupFrequency != null && SupFrequency.Count > 0)
                        objViewModel.FrequencyList = new SelectList(SupFrequency, Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, Common.SUP_FEE_FREQUENCY.FREQUENCY_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStructureReportByProgramme", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStructureReportByProgramme", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult BindProgrammeGroupByShiftAndProgrammeMode(string sShift, string sProgrammeMode)
        {
            string sOption = string.Empty;
            CP_PROGRAMME_GROUP objModel = new CP_PROGRAMME_GROUP();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.SHIFT = sShift;
                    objModel.PROGRAMME_MODE = sProgrammeMode;
                    var Programme = (List<CP_PROGRAMME_2017>)CMS.DAO.CMSHandler.FetchData<CP_PROGRAMME_2017>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupByShiftAndProgrammeMode)).DataSource.Data;
                    if (Programme != null && Programme.Count > 0)
                    {
                        foreach (var item in Programme)
                        {
                            sOption += "<option value='" + item.PROGRAMME_GROUP_ID + "' >" + item.PROGRAMME_NAME + "</option>";
                        }
                    }
                }
                else
                    return Json(sOption);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindProgrammeGroupByShiftAndProgrammeMode", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindProgrammeGroupByShiftAndProgrammeMode", "Fee", ex.Message);
                }
            }
            return Json(sOption);
        }
        public JsonResult BindProgrammeGroupByShiftAndProgrammeModeAndAppType(string sShift, string sProgrammeMode, string sApplicationType)
        {
            string sOption = string.Empty;
            CP_PROGRAMME_GROUP objModel = new CP_PROGRAMME_GROUP();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.SHIFT = sShift;
                    objModel.PROGRAMME_MODE = sProgrammeMode;
                    objModel.APPTYPE_ID = sApplicationType;
                    var Programme = (List<CP_PROGRAMME_2017>)CMS.DAO.CMSHandler.FetchData<CP_PROGRAMME_2017>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupByShiftAndProgrammeModeAndApplicationType)).DataSource.Data;
                    if (Programme != null && Programme.Count > 0)
                    {
                        foreach (var item in Programme)
                        {
                            sOption += "<option value='" + item.PROGRAMME_GROUP_ID + "' >" + item.PROGRAMME_NAME + "</option>";
                        }
                    }
                }
                else
                    return Json(sOption);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindProgrammeGroupByShiftAndProgrammeMode", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindProgrammeGroupByShiftAndProgrammeMode", "Fee", ex.Message);
                }
            }
            return Json(sOption);
        }
        public ActionResult BindFeeStructureReportByProgramme(string sAcademicYearId, string sFrequecyId, string sClassYear, string sShift, string sProgrammeMode, string sProgrammeId)
        {
            FeeStructureViewModel ObjViewModel = new FeeStructureViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            string sSql = string.Empty;
            string Sql = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                FEE_STRUCTURE objModel = new FEE_STRUCTURE();
                try
                {
                    objModel.ACADEMIC_YEAR = sAcademicYearId;
                    objModel.FREQUENCY_ID = sFrequecyId;
                    objModel.SHIFT = sShift;
                    objModel.PROGRAMME_MODE = sProgrammeMode;
                    sSql = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStructureReportByProgrammeWiseReport).Replace(Common.Delimiter.QUS + "CLASS_YEAR_ID", sClassYear);
                    sSql = sSql.Replace(Common.Delimiter.QUS + Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, sProgrammeId);
                    Sql = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeadForFeeStructureByGroup).Replace(Common.Delimiter.QUS + "CLASS_YEAR_ID", sClassYear);
                    Sql = Sql.Replace(Common.Delimiter.QUS + Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, sProgrammeId);
                    ObjViewModel.lstFeeStructure = (List<FEE_STRUCTURE>)CMSHandler.FetchData<FEE_STRUCTURE>(objModel, sSql, sAcademicYear).DataSource.Data;
                    ObjViewModel.LiFeeStructure = (List<FEE_STRUCTURE>)CMSHandler.FetchData<FEE_STRUCTURE>(objModel, Sql, sAcademicYear).DataSource.Data;
                }
                catch (Exception Ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "BindFeeStructureReportByProgramme", "Err MSg: " + Ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "BindFeeStructureReportByProgramme", Ex.Message);
                    }
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");

            return View(ObjViewModel);
        }
        #endregion

        #region FeeAccountDateWiseReport
        [UserRights("office")]
        public ActionResult AccountWiseSettlement()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.liAcademicYear = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    var LiSupBankAccount = (List<SUP_BANK_ACCOUNT>)CMSHandler.FetchData<SUP_BANK_ACCOUNT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupBankAccounts)).DataSource.Data;
                    objViewModel.liBankAccount = new SelectList(LiSupBankAccount, Common.SUP_BANK_ACCOUNT.BANK_ACCOUNT_ID, Common.SUP_BANK_ACCOUNT.PASSBOOK_NO);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AccountWiseSettlement", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountWiseSettlement", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindAccountWiseSettlement(string sFrom, string sTo, string sBankAccount, string sAcademicYear)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            FEE_REPORT objModel = new FEE_REPORT();
            try
            {
                objModel.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(sFrom);
                objModel.DATE_TO = CommonMethods.ConvertdatetoyearFromat(sTo);
                objModel.BANK_ACCOUNT_ID = sBankAccount;
                objViewModel.liFeeReport = (List<FEE_REPORT>)CMSHandler.FetchData<FEE_REPORT>(objModel, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchSettlementByDateWise), sAcademicYear).DataSource.Data;

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindAccountWiseSettlement", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindAccountWiseSettlement", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindAccountWiseSettlementByDate(string sSettlementID, string sBankAccount, string sAcademicYear)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            FEE_REPORT objModel = new FEE_REPORT();
            try
            {
                objViewModel.liFeeReport = (List<FEE_REPORT>)CMSHandler.FetchData<FEE_REPORT>(new FEE_REPORT() { RECIPIENT_SETTLEMENT_ID = sSettlementID }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchSettlementByDateWiseAndAccount), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindAccountWiseSettlementByDate", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindAccountWiseSettlementByDate", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        #endregion  


        #region Excel Import ( Bank Copy )
        public ActionResult FeePaymentExcelUpdate()
        {
            FeeTransactionViewModel ObjViewModel = new FeeTransactionViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.liAcademicYear = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeePaymentExcelUpdate", "Fee", ex.Message);
                }
            }
            return View(ObjViewModel);
        }

        public JsonResult SaveFeePaymentUpdates()
        {
            FeeTransactionViewModel ObjViewModel = new FeeTransactionViewModel();
            var ObjJsonData = new JsonResultData();
            DataTable dt = new DataTable();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string extension = System.IO.Path.GetExtension(Request.Files[0].FileName).ToLower();
                    string connString = string.Empty;
                    string sAcYear = Request.Form["AC_YEAR"];
                    string FrequencyId = Request.Form["FREQUENCY_ID"];
                    string Frequency = Request.Form["FREQUENCY"];
                    string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
                    string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/Uploads/Fee/" + FrequencyId), Request.Files[0].FileName);

                    if (!Directory.Exists(path1))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/Uploads/Fee/" + FrequencyId));
                    }

                    // Save File ,...
                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);
                    }
                    Request.Files[0].SaveAs(path1);

                    if (validFileTypes.Contains(extension))
                    {
                        if (extension == ".csv")
                        {
                            dt = CommonMethods.ConvertCSVtoDataTable(path1);
                        }
                        //Connection String to Excel Workbook  
                        else if (extension.Trim() == ".xls")
                        {
                            connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=Excel 8.0";// string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=\"Excel 12.0 Xml;HDR=No;IMEX=1\";", path1);
                            dt = CommonMethods.ConvertXSLXtoDataTable(path1, connString);
                        }
                        else if (extension.Trim() == ".xlsx")
                        {
                            connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=Excel 8.0";// string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=\"Excel 12.0 Xml;HDR=No;IMEX=1\";" , path1);
                            dt = CommonMethods.ConvertXSLXtoDataTable(path1, connString);
                        }


                        // DataBase .....,
                        string sAccountSQL = string.Empty;
                        sAccountSQL = @"INSERT INTO FEE_STUDENT_ACCOUNT 
                                    (
	                                    STUDENT_ID,
	                                    ACADEMIC_YEAR,
	                                    FREQUENCY_ID,
	                                    HEAD,
	                                    DEBIT,
	                                    TRANSACTION_ID,
	                                    FEE_MAIN_HEAD_ID
                                    ) VALUES ";

                        string sCollectionSQL = string.Empty;
                        sCollectionSQL = @"INSERT INTO FEE_COLLECTION 
                                        (
	                                        TRANSACTION_ID,
	                                        HEAD,
	                                        PAID_AMOUNT,
	                                        FREQUENCY,
	                                        RECEIPT_NO,
	                                        FEE_MAIN_HEAD_ID
                                        ) VALUES ";

                        var ObjTransactionModel = new FEE_TRANSACTION();
                        var liStudentList = (List<Fee_Account>)CMSHandler.FetchData<Fee_Account>(new Fee_Account() { FREQUENCY_ID = FrequencyId }, SQL.Student.StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentFeeDetailsByFrequencyId), sAcYear).DataSource.Data;
                        if (liStudentList != null && liStudentList.Count > 0)
                        {
                            int nReceiptNo = 0; bool sFalg = false; int nCount = 0;
                            var FetchReceiptNo = (List<AUTO_GENERATE_NUMBERS>)CMSHandler.FetchData<AUTO_GENERATE_NUMBERS>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAutoGenerateNumber)).DataSource.Data;
                            if (FetchReceiptNo != null && FetchReceiptNo.Count > 0)
                            {
                                nReceiptNo = Convert.ToInt32(FetchReceiptNo.FirstOrDefault().exam_receipt_no);
                            }

                            if (dt != null && dt.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt.Rows)
                                {
                                    var liAccountExist = liStudentList.Where(x => x.ROLL_NO == dr["Roll No"].ToString()).ToList();
                                    if (liAccountExist != null && liAccountExist.Count > 0)
                                    {
                                        nReceiptNo++; string s = dr["Roll No"].ToString();
                                        // Fee Transaction ....,
                                        ObjTransactionModel.STUDENT_ID = liAccountExist.FirstOrDefault().STUDENT_ID;
                                        ObjTransactionModel.FREQUENCY = FrequencyId;
                                        ObjTransactionModel.FREQUENCY_ID = FrequencyId;
                                        ObjTransactionModel.CLASS = liAccountExist.FirstOrDefault().CLASS_ID;
                                        ObjTransactionModel.PAYMENT_MODE = "6";
                                        ObjTransactionModel.COLLECTED = dr["Amount"].ToString();
                                        ObjTransactionModel.USERNAME = sStaffId;
                                        string sLateFee = dr["Late Fee"].ToString();
                                        if (!string.IsNullOrEmpty(sLateFee) && sLateFee != "0")
                                        {
                                            ObjTransactionModel.CREDIT = sLateFee;
                                            ObjTransactionModel.HEAD = "16";

                                            var liLateFeeExist = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(ObjTransactionModel, FeeSQL.GetFeeSQL(FeeSQLCommands.CheckIsStudentAccountExist), sAcYear).DataSource.Data;
                                            if (liLateFeeExist != null && liLateFeeExist.Count > 0)
                                            {
                                                // Already Exist
                                            }
                                            else
                                            {
                                                var liFeeMainHead = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(ObjTransactionModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeadLateFee), sAcYear).DataSource.Data;
                                                if (liFeeMainHead != null && liFeeMainHead.Count > 0)
                                                {
                                                    ObjTransactionModel.FEE_MAIN_HEAD_ID = liFeeMainHead.FirstOrDefault().FEE_MAIN_HEAD_ID;
                                                    var SaveLateFee = CMSHandler.SaveOrUpdate(ObjTransactionModel, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeStudentAccountForBankExcelUpload), sAcYear);
                                                    if (SaveLateFee.Success)
                                                    {
                                                        liAccountExist.Add(new Fee_Account()
                                                        {
                                                            STUDENT_ID = ObjTransactionModel.STUDENT_ID,
                                                            FREQUENCY_ID = FrequencyId,
                                                            HEAD = ObjTransactionModel.HEAD,
                                                            DEBIT = ObjTransactionModel.CREDIT,
                                                            FEE_MAIN_HEAD_ID = liFeeMainHead.FirstOrDefault().FEE_MAIN_HEAD_ID
                                                        });
                                                    }
                                                }
                                            }
                                        }
                                        var dateTime = DateTime.ParseExact(dr["Clearance Date"].ToString(), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                        string output = dateTime.ToString("yyyy-MM-dd");
                                        ObjTransactionModel.PAYMENT_DATE = output;
                                        ObjTransactionModel.RECEIPT_NO = "00" + nReceiptNo.ToString();
                                        ObjTransactionModel.FREQUENCY_TO = FrequencyId;
                                        ObjTransactionModel.FEE_TRANSACTION_BANK = dr["URNO"].ToString();

                                        var SaveFeeTransaction = CMSHandler.SaveOrUpdate(ObjTransactionModel, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeTransactionForBankExcelUpload), sAcYear, true);
                                        if (SaveFeeTransaction.Success && SaveFeeTransaction.RowsAffected != 0)
                                        {
                                            nCount++;
                                            string sTransactionId = string.Empty;
                                            sTransactionId = SaveFeeTransaction.RowUniqueId.ToString();
                                            sFalg = true;
                                            foreach (var item in liAccountExist)
                                            {
                                                // Fee Studnet Account Updation ......,
                                                sAccountSQL += "('" + item.STUDENT_ID + "','" + sAcademicYear + "','" + item.FREQUENCY_ID + "','" + item.HEAD + "','" + item.DEBIT + "','" + sTransactionId + "','" + item.FEE_MAIN_HEAD_ID + "'),";

                                                // Fee Collection .....,
                                                sCollectionSQL += "('" + sTransactionId + "','" + item.HEAD + "','" + item.DEBIT + "','" + item.FREQUENCY_ID + "','" + "00" + nReceiptNo.ToString() + "','" + item.FEE_MAIN_HEAD_ID + "'),";
                                            }
                                            liStudentList.RemoveAll(x => x.ROLL_NO == dr["Roll No"].ToString());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ObjJsonData.Message = "Oops! File Con't be Read";
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            }

                            if (sFalg)
                            {



                                // Save Account ,...
                                var UpdateAutoincrement = CMSHandler.SaveOrUpdate(new AUTO_GENERATE_NUMBERS() { exam_receipt_no = nReceiptNo.ToString() }, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateAutoGenerateNumber), sAcademicYear, true);
                                sAccountSQL = sAccountSQL.TrimEnd(',');
                                sAccountSQL = sAccountSQL + ";";

                                sCollectionSQL = sCollectionSQL.TrimEnd(',');
                                sCollectionSQL = sCollectionSQL + ";";

                                using (ExaminationViewModel objExamination = new ExaminationViewModel())
                                {
                                    var SaveAccount = objExamination.ExcuteCIA_Marks(sAccountSQL);
                                    if (SaveAccount.Success && SaveAccount.RowsAffected != 0)
                                    {
                                        var SaveCollection = objExamination.ExcuteCIA_Marks(sCollectionSQL);
                                        ObjJsonData.Message = (SaveCollection.Success && SaveCollection.RowsAffected != 0) ? "(" + nCount.ToString() + ") " + Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                        ObjJsonData.ErrorCode = (SaveCollection.Success && SaveCollection.RowsAffected != 0) ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                                    }
                                    else
                                    {
                                        ObjJsonData.Message = "( Student Account )" + Common.Messages.FailedToSaveRecords;
                                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    }
                                }
                            }
                            else
                            {
                                ObjJsonData.Message = "No Changes Made !";
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            }
                        }
                        else
                        {
                            ObjJsonData.Message = "No Student's Account Found For This Frequency";
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Fee", "SaveFeePaymentUpdates", ex.Message);
                    }
                }
            }

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData);
        }
        #endregion  

        #region Bank Fee List (Excel Copy To Bank)
        public ActionResult BankFeeList()
        {
            FeeTransactionViewModel ObjViewModel = new FeeTransactionViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    var liFeeRoot = (List<SUP_FEE_ROOT>)CMSHandler.FetchData<SUP_FEE_ROOT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FectchFeeRoot)).DataSource.Data;
                    if (liFeeRoot != null && liFeeRoot.Count > 0)
                    {
                        ObjViewModel.FeeRoot = new SelectList(liFeeRoot, Common.SUP_FEE_ROOT.FEE_ROOT_ID, Common.SUP_FEE_ROOT.FEE_ROOT);
                    }
                    var liFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(new SUP_FEE_FREQUENCY() { FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeModeAndFeeRootId), sAcademicYear).DataSource.Data;
                    if (liFrequency != null && liFrequency.Count > 0)
                    {
                        ObjViewModel.liFrequencyType = new SelectList(liFrequency, Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, Common.SUP_FEE_FREQUENCY.FREQUENCY_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BankFeeList", "Fee", ex.Message);
                }
            }
            return View(ObjViewModel);
        }

        public ActionResult BindBankFeeList(string sFeeRootId, string sFrequencyId)
        {
            FeeTransactionViewModel ObjViewModel = new FeeTransactionViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new FEE_STUDENT_ACCOUNT();
                    ObjModel.FEE_ROOT_ID = sFeeRootId;
                    ObjModel.FREQUENCY_ID = sFrequencyId;
                    // Fetch Main Heads ,...
                    ObjViewModel.liFeeMainHeads = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeadsByFrequencyId), sAcademicYear).DataSource.Data;
                    ObjViewModel.liFeeAccountList = (List<Fee_Account>)CMSHandler.FetchData<Fee_Account>(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentsAccountByFrequencyId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("BindBankFeeList", "Fee", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(ObjViewModel);
        }
        #endregion

        #region Overall Fee Report
        public ActionResult FeeOverallReport()
        {
            var ObjfeeviewModel = new FeeViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Listu = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentInfo), sAcademicYear).DataSource.Data;
                    if (Listu != null && Listu.Count > 0)
                    {
                        ObjfeeviewModel.listudent = Listu;
                    }
                    var Lifeeaccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccount), sAcademicYear).DataSource.Data;
                    if (Lifeeaccount != null && Lifeeaccount.Count > 0)
                    {
                        ObjfeeviewModel.lifee_stu = Lifeeaccount;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "FeeOverallReport", ex.Message);
                    }
                }
            }
            return View(ObjfeeviewModel);
        }
        #endregion

        #region Gate Fee Entry (Student)
        public ActionResult GateFeeEntry()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            var ObjViewModel = new FeeViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ObjViewModel.sMessage = sAcademicYear;
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindStudentLateFeeInfo(string sRollNo)
        {
            sRollNo = sRollNo.Replace("*", "");
            var ObjViewModel = new FeeViewModel();
            var ObjModel = new FEE_REPORT();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.ROLL_NO = sRollNo;
                    ObjModel.FEE_MODE = Common.FrequencyType.GateFee;
                    var liStudent = (List<FEE_REPORT>)CMSHandler.FetchData<FEE_REPORT>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByRollNoForGateFee), sAcademicYear).DataSource.Data;
                    if (liStudent != null && liStudent.Count > 0)
                    {
                        ObjModel.STUDENT_ID = liStudent.FirstOrDefault().STUDENT_ID;
                        ObjModel.FREQUENCY_ID = liStudent.FirstOrDefault().FREQUENCY_ID;
                        ObjModel.FEE_MAIN_HEAD_ID = liStudent.FirstOrDefault().FEE_MAIN_HEAD_ID;
                        ObjModel.FIRST_NAME = liStudent.FirstOrDefault().FIRST_NAME;
                        ObjModel.CLASS = liStudent.FirstOrDefault().CLASS;
                        //var liIsFineAmount = (List<FEE_REPORT>)CMSHandler.FetchData<FEE_REPORT>(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchCreditByStudentId), sAcademicYear).DataSource.Data;
                        var liStuLateFee = (List<STU_LATE_ENTRY>)CMSHandler.FetchData<STU_LATE_ENTRY>(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchGateFeeByStudentId), sAcademicYear).DataSource.Data;
                        ObjViewModel.liStuLateFee = liStuLateFee;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "BindStudentLateFeeInfo", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }

        public JsonResult SaveGateFeeEntry(string sRollNo, string sFineAmount)
        {
            sRollNo = sRollNo.Replace("*", "");
            var ObjJsonData = new JsonResultData();
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sUserId))
            {
                try
                {
                    var ObjModel = new FEE_REPORT();
                    var ObjFeeModel = new STU_LATE_ENTRY();
                    ObjModel.ROLL_NO = sRollNo;
                    ObjModel.FEE_MODE = Common.FrequencyType.GateFee;

                    var liStudent = (List<FEE_REPORT>)CMSHandler.FetchData<FEE_REPORT>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByRollNoForGateFee), sAcademicYear).DataSource.Data;
                    if (liStudent != null && liStudent.Count > 0)
                    {
                        /// 1 => 5   (No Fine) .
                        /// Above 5  Fine Rs.20 .
                        /// Calculate Leave For Each 5 Entry .
                        ObjFeeModel.FINE_AMOUNT = "0";
                        ObjFeeModel.IS_LEAVE_ADDED = "0";
                        ObjFeeModel.IS_FINE_ADDED = "0";

                        ObjModel.STUDENT_ID = liStudent.FirstOrDefault().STUDENT_ID;
                        ObjModel.FREQUENCY_ID = liStudent.FirstOrDefault().FREQUENCY_ID;
                        ObjModel.FEE_MAIN_HEAD_ID = liStudent.FirstOrDefault().FEE_MAIN_HEAD_ID;
                        ObjModel.HEAD = liStudent.FirstOrDefault().HEAD_ID;
                        ObjModel.FIRST_NAME = liStudent.FirstOrDefault().FIRST_NAME;
                        ObjModel.CLASS = liStudent.FirstOrDefault().CLASS;
                        ObjModel.CREDIT = sFineAmount;


                        // Save Fee Account .
                        ObjFeeModel.STUDENT_ID = liStudent.FirstOrDefault().STUDENT_ID;
                        ObjFeeModel.ENTRY_ID = sUserId;
                        ObjFeeModel.SEMESTER_TYPE = sSemesterType;
                        ObjFeeModel.FREQUENCY_ID = liStudent.FirstOrDefault().FREQUENCY_ID;
                        ObjFeeModel.ACADEMIC_YEAR = sAcademicYear;


                        // Calculated Fetch For No Of Days .
                        var liStuLateFee = (List<STU_LATE_ENTRY>)CMSHandler.FetchData<STU_LATE_ENTRY>(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchGateFeeByStudentIdAndFrequency), sAcademicYear).DataSource.Data;
                        if (liStuLateFee != null && liStuLateFee.Count > 0)
                        {
                            if (liStuLateFee.FirstOrDefault().IS_CURDATE == "1")
                            {
                                ObjJsonData.Message = "Record Captured For Today !";
                                ObjJsonData.ErrorCode = Common.ErrorCode.Conflict;
                                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                            }
                            int nCount = liStuLateFee.Count() + 1;
                            if (nCount % 5 == 0)
                            {
                                ObjFeeModel.IS_LEAVE_ADDED = "1";
                            }

                            if (nCount > 5)
                            {
                                ObjFeeModel.IS_FINE_ADDED = "1";
                                ObjFeeModel.FINE_AMOUNT = sFineAmount;

                                // Fetch Credit Details For Fine Frequency .
                                //var liCreditInfo = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchCreditOnlyByStudentIdAndFrequencyId), sAcademicYear).DataSource.Data;
                                //if (liCreditInfo != null && liCreditInfo.Count > 0)
                                //{
                                //    ObjModel.F_STUDENT_AC_ID = liCreditInfo.FirstOrDefault().F_STUDENT_AC_ID;
                                //    // Update Credit Anount With Older One .
                                //    var UpdateFeeStudentAccount = CMSHandler.SaveOrUpdate(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateFeeStudentAccount), sAcademicYear);
                                //    if (UpdateFeeStudentAccount.Success)
                                //    {
                                //        ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                //        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                //    }
                                //    else
                                //    {
                                //        ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                                //        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                //    }
                                //}
                                //else
                                //{
                                //    // Insert Credit
                                //    var CreateFeeStudentAccount = CMSHandler.SaveOrUpdate(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeStudentAccountCreation), sAcademicYear);
                                //    if (CreateFeeStudentAccount.Success)
                                //    {
                                //        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                //        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                //    }
                                //    else
                                //    {
                                //        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                //        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                //    }
                                //}
                            }
                        }
                        else
                        {
                            ObjFeeModel.IS_LEAVE_ADDED = "0";
                            ObjFeeModel.IS_FINE_ADDED = "0";
                        }
                        CommunicationController Communition = new CommunicationController();
                        // Save Late Fee .
                        var SaveLateFee = CMSHandler.SaveOrUpdate(ObjFeeModel, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveStudentLateEntry), sAcademicYear);
                        if (SaveLateFee.Success)
                        {
                            //var studentDetails = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(ObjFeeModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentMobileNoSMS), sAcademicYear).DataSource.Data;

                            //if (studentDetails != null && studentDetails.Count > 0)
                            //{
                            //    var AttendanceDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            //    string sCount = string.Empty;
                            //    //int sCount = studentDetails.Count;
                            //    foreach (var item in studentDetails)
                            //    {

                            //        string sAbsentMessage = string.Empty;
                            //        var MessageContent = (List<SMS_USERDEFINED_TEMPLATE>)CMSHandler.FetchData<SMS_USERDEFINED_TEMPLATE>(new SMS_USERDEFINED_TEMPLATE() { TEMPLATE_ID = Common.SMS_TEMPLATE.LateComers }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSMSTemplateText), sAcademicYear).DataSource.Data;
                            //        if (MessageContent != null && MessageContent.Count > 0)
                            //        {
                            //            sAbsentMessage = MessageContent.FirstOrDefault().TEMPLATE_TEXT.Replace(Common.Delimiter.AT + Common.STU_PERSONAL_INFO.FIRST_NAME, item.FIRST_NAME).Replace(Common.Delimiter.AT + Common.CP_CLASSES_2017.CLASS_NAME, item.CLASS_NAME).Replace(Common.Delimiter.AT + "ATTENDANCE_DATE", AttendanceDate);
                            //            sCount = Communition.ValidateCreditCount(sAbsentMessage.Length);
                            //            Communition.sCreditCount = Convert.ToString(sCount);
                            //        }
                            //        else
                            //        {
                            //            ObjJsonData.Message = "No Absentees Template Found ! Please Meet ERP Admin !";
                            //            ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                            //        }
                            //        sSmsMode = Common.MessageType.LATE_ENTRY;
                            //        if (!string.IsNullOrEmpty(item.MOBILE_NO))
                            //        {
                            //            Communition.liSMS.Add(new MessageResult.sms() { to = item.MOBILE_NO, message = sAbsentMessage, custom = item.STUDENT_ID, custom1 = item.CLASS_ID, CreditCount = Convert.ToString(sCount) });
                            //            Communition.listbulkSMS.Add(new MessageResult.listsms() { mobiles = item.MOBILE_NO, sms = sAbsentMessage, class_id = item.CLASS_ID, receipient_id = item.STUDENT_ID });
                            //        }
                            //        else
                            //        {
                            //        }
                            //    }
                            //    Communition.SendMessageV1();
                            //}

                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;

                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                    else
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "SaveGateFeeEntry", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = "No Changes Made !";
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LateFeeEntry()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            var ObjViewModel = new FeeViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ObjViewModel.sMessage = sAcademicYear;
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindStudentLateFeeInfoByDate()
        {
            var ObjViewModel = new FeeViewModel();
            var ObjModel = new FEE_REPORT();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.FEE_MODE = Common.FrequencyType.GateFee;
                    var liStuLateFee = (List<STU_LATE_ENTRY>)CMSHandler.FetchData<STU_LATE_ENTRY>(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchLateFeeByDate), sAcademicYear).DataSource.Data;
                    ObjViewModel.liStuLateFee = liStuLateFee;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "BindStudentLateFeeInfo", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        public JsonResult DeleteStuLateEntry(string lateId)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new STU_LATE_ENTRY();
                    ObjModel.LATE_FEE_ID = lateId;
                    ObjModel.IS_DELETED = Common.DeleteFlag.Deleted;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeletedStuLateEntrybyId));
                    if (DeleteQuery.Success)
                    {
                        ObjJsonData.Message = Common.Messages.RecordDeletedSuccessfully;
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.FailedToDeletedTryAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "DeleteProject", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SendSMS()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    CommunicationController Communition = new CommunicationController();
                    var studentDetails = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentInfoforLateEntrySMS), sAcademicYear).DataSource.Data;

                    if (studentDetails != null && studentDetails.Count > 0)
                    {
                        var AttendanceDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        string sCount = string.Empty;
                        //int sCount = studentDetails.Count;
                        foreach (var item in studentDetails)
                        {

                            string sAbsentMessage = string.Empty;
                            var MessageContent = (List<SMS_USERDEFINED_TEMPLATE>)CMSHandler.FetchData<SMS_USERDEFINED_TEMPLATE>(new SMS_USERDEFINED_TEMPLATE() { TEMPLATE_ID = Common.SMS_TEMPLATE.LateComers }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSMSTemplateText), sAcademicYear).DataSource.Data;
                            if (MessageContent != null && MessageContent.Count > 0)
                            {
                                sAbsentMessage = MessageContent.FirstOrDefault().TEMPLATE_TEXT.Replace(Common.Delimiter.AT + Common.STU_PERSONAL_INFO.FIRST_NAME, item.FIRST_NAME).Replace(Common.Delimiter.AT + Common.CP_CLASSES_2017.CLASS_NAME, item.CLASS_NAME).Replace(Common.Delimiter.AT + "ATTENDANCE_DATE", AttendanceDate);
                                sCount = Communition.ValidateCreditCount(sAbsentMessage.Length);
                                Communition.sCreditCount = Convert.ToString(sCount);
                            }
                            else
                            {
                                ObjJsonData.Message = "No Absentees Template Found ! Please Meet ERP Admin !";
                                ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                            }
                            sSmsMode = Common.MessageType.LATE_ENTRY;
                            if (!string.IsNullOrEmpty(item.MOBILE_NO))
                            {
                                Communition.liSMS.Add(new MessageResult.sms() { to = item.MOBILE_NO, message = sAbsentMessage, custom = item.STUDENT_ID, custom1 = item.CLASS_ID, CreditCount = Convert.ToString(sCount) });
                                Communition.listbulkSMS.Add(new MessageResult.listsms() { mobiles = item.MOBILE_NO, sms = sAbsentMessage, class_id = item.CLASS_ID, receipient_id = item.STUDENT_ID });
                            }
                            else
                            {
                            }
                        }
                        Communition.SendMessageV1();

                        if (Communition.sResultMessage == "200")
                        {
                            var liUpdateSMS = CMSHandler.SaveOrUpdate(null, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateLateFeeSMSUpdate), sAcademicYear);
                            ObjJsonData.Message = "SMS Sent Successfully !";
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message = "Sorry ! Failed To Sent Messages .!";
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = "SMS has already been sent.!";
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                    //var liSendSMS = (List<MessageResult.sms>)CMSHandler.FetchData<MessageResult.sms>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchLateFeeInfoForSMSByDate), sAcademicYear).DataSource.Data;
                    //if (liSendSMS != null && liSendSMS.Count > 0)
                    //{
                    //    //liSendSMS.Add(new MessageResult.sms() { to = "9443246856", message =  "Today Late Message Sent Successfully !", custom = "0", custom1 = "0", custom2 = "0", CreditCount = "1" });
                    //    var ObjControllerModel = new CommunicationController();
                    //    ObjControllerModel.liSMS = liSendSMS;
                    //    ObjControllerModel.SendMessage();


                    //    if (ObjControllerModel.sResultMessage == "200")
                    //    {
                    //        var liUpdateSMS = CMSHandler.SaveOrUpdate(null, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateLateFeeSMSUpdate), sAcademicYear);
                    //        ObjJsonData.Message = "SMS Sent Successfully !";
                    //        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    //    }
                    //    else
                    //    {
                    //        ObjJsonData.Message = "Sorry ! Failed To Sent Messages .!";
                    //        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "SendSMS", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StudentLateEntryReport()
        {
            FeeViewModel objViewModel = new FeeViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";

                    var ClassList = (List<ClassInfo>)CMSHandler.FetchData<ClassInfo>(new StaffList(), FeeSQL.GetFeeSQL(FeeSQLCommands.FetchClassList), sAcademicYear).DataSource.Data;
                    if (ClassList != null && ClassList.Count > 0)
                    {

                        objViewModel.ClassList = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);

                    }


                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "PedagogicMethodInputSystem", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "PedagogicMethodInputSystem", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindStuLateEntryReportDateWise(string sdate, string edate)
        {
            FeeViewModel objViewModel = new FeeViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string Date = CommonMethods.DateConvertionForPayU(sdate);
                    string ToDate = CommonMethods.DateConvertionForPayU(edate);
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";

                    objViewModel.liStuLateFee = (List<STU_LATE_ENTRY>)CMSHandler.FetchData<STU_LATE_ENTRY>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchLateEntryReportDateWise).Replace("?FROM_DATE", Date).Replace("?TODATE", ToDate), sAcademicYear).DataSource.Data;


                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "BindPedagogicMethodInputSystem", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "BindPedagogicMethodInputSystem", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindStuLateEntryReportClassWise(string ClassId)
        {
            FeeViewModel objViewModel = new FeeViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {

                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";

                    objViewModel.liStuLateFee = (List<STU_LATE_ENTRY>)CMSHandler.FetchData<STU_LATE_ENTRY>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchLateEntryStudentClassWise).Replace("?CLASS_ID", ClassId), sAcademicYear).DataSource.Data;


                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "BindPedagogicMethodInputSystem", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "BindPedagogicMethodInputSystem", Ex.Message);
                }
            }
            return View(objViewModel);
        }

        public ActionResult StudentLateEntryAbsentReport()
        {
            FeeViewModel objViewModel = new FeeViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    {
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var liSemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
                    if (liSemesterType != null && liSemesterType.Count > 0)
                    {
                        objViewModel.SemesterTypeList = new SelectList(liSemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    }
                    var liShift = (List<Sup_Shift>)CMS.DAO.CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    objViewModel.ShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);

                    var classYear = (List<CMS.ViewModel.Model.SubClassYear>)CMSHandler.FetchData<CMS.ViewModel.Model.SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data;
                    if (classYear != null && classYear.Count > 0)
                    {
                        objViewModel.ClassYearList = new SelectList(classYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                    }
                    var programmeLevel = (List<SupProgrammeLevel>)CMSHandler.FetchData<SupProgrammeLevel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel)).DataSource.Data;
                    if (programmeLevel != null && programmeLevel.Count > 0)
                    {
                        objViewModel.programmeLevel = new SelectList(programmeLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
                    }

                    ViewBag.ActiveAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null)
                    {
                        ViewBag.ActiveSemesterType = Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString();
                    }
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeController", "StudentLateEntryAbsentReport", Ex.Message);
                }
            }
            return View(objViewModel);
        }

        public ActionResult BindStudentLateEntryAbsentReport(string sAcademicYear, string sSemesterType, string sClassId)
        {
            FeeViewModel objViewModel = new FeeViewModel();
            try
            {
                var objLateEntry = new STU_LATE_ENTRY { SEMESTER_TYPE = sSemesterType };
                string acYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                string semType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : "";
                string query = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentLateEntryAbsentReport);
                if (!string.IsNullOrEmpty(sClassId) && sClassId != "null" && sClassId != "0" && sClassId != "undefined")
                {
                    query = query.Replace("?CLASS_ID_CONDITION", "AND SC.CLASS_ID IN (" + sClassId + ")");
                }
                else
                {
                    sAcademicYear = acYear;
                    sSemesterType = semType;
                    query = query.Replace("?CLASS_ID_CONDITION", "");
                }

                objViewModel.liStuLateFee = (List<STU_LATE_ENTRY>)CMSHandler.FetchData<STU_LATE_ENTRY>(
                    objLateEntry,
                    query,
                    sAcademicYear
                ).DataSource.Data;
                ViewBag.AcYear = sAcademicYear;
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeController", "BindStudentLateEntryAbsentReport", Ex.Message);
                }
            }
            return PartialView("BindStudentLateEntryAbsentReport", objViewModel);
        }
        #endregion

        #region Commmon Send SMS Method 
        public void SendMessage()
        {
            try
            {
                var liVendordetails = DapperHandler.FetchAll<SMS_VENDOR_DETAILS>(CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchActiveSMSvendors)).ToList();
                if (liVendordetails != null && liVendordetails.Count > 0)
                {
                    if (liVendordetails.FirstOrDefault().SMS_VENDOR_ID == Common.sms_vendor.DoveSoft)
                    {
                        SendBulkSMSFromDovesoft();
                    }
                }
                else
                {
                    resultArgsSMS.Success = false;
                    resultArgsSMS.Message = "SMS Vendors not found ...!";
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommunicationController", "SendMessage", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommunicationController", "SendMessage", ex.Message);
                }
                ObjJsonDataSMS.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonDataSMS.ErrorCode = Common.ErrorCode.InternalServerError;
            }

        }
        public void SendBulkSMSFromDovesoft()
        {
            bool bFlag = true;
            int ncount = 0;
            //sSmsCount = listbulkSMS.Count;
            int SMSSkipcount = 0;
            var list = new List<MessageResult.listsms>();
            string TotalresponseString = string.Empty;
            var lastInsertID = "";
            try
            {
                var listBulksmsList = listbulkSMS.GroupBy(o => o.class_id)
                                   .OrderBy(o => o.Key)
                                   .ToList();
                var liBalance = (List<SMSBalence>)CMSHandler.FetchData<SMSBalence>(null, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSMSBalance)).DataSource.Data;
                if (liBalance != null && liBalance.Count > 0)
                {
                    iReceivedCredits = Convert.ToInt32(liBalance.FirstOrDefault().RECEIVED_CREDITS);
                    iSMSLimit = Convert.ToInt32(liBalance.FirstOrDefault().SMS_LIMIT);
                    iUsedCredit = Convert.ToInt32(liBalance.FirstOrDefault().USED_CREDITS);
                    //iSMSSent = (string.IsNullOrEmpty(sCreditCount)) ? (1 * Convert.ToInt16(sSmsCount)) : (Convert.ToInt16(sCreditCount) * Convert.ToInt16(sSmsCount));
                    iSMSSent = listbulkSMS.Count();
                    iTotalSendCreditCount = iSMSSent + iUsedCredit;
                    if (iSMSLimit >= iTotalSendCreditCount && iSMSLimit != 0)
                        sResult = Common.ErrorCode.Ok;
                    else
                        ObjJsonDataSMS.Message = Common.Messages.LowMessageLimit;
                    var ObjUpdateBls = new UpdateSMSBalance();
                    ObjUpdateBls.BALANCE_CREDITS = Convert.ToString(iSMSLimit - iTotalSendCreditCount);
                    ObjUpdateBls.USED_CREDITS = Convert.ToString(iSMSSent + iUsedCredit);
                    ObjUpdateBls.IS_ACTIVE = Common.IsActiveFalg.IsActive;
                    var resultArgs = CMSHandler.SaveOrUpdate(ObjUpdateBls, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.UpdateSMSBalance));
                }

                foreach (var liItem in listBulksmsList)
                {
                    sSmsCount = liItem.Count();
                    nLogcount = 0;

                    // Fetch SMS Settings Once for Each Class Group
                    liSMSSetting = CMSHandler.FetchData<SMS_SETTING>(new SMS_SETTING
                    {
                        API_METHOD = Common.API_METHOD.BULK_SMS
                    }, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchDovesoftSettings)).DataSource.Data as List<SMS_SETTING>;

                    if (liSMSSetting == null || liSMSSetting.Count == 0) continue;

                    sAPIKey = liSMSSetting.First().API_KEY;
                    sBaseUrl = liSMSSetting.First().BASE_URL;
                    sFormat = liSMSSetting.First().FORMAT;
                    sMethod = liSMSSetting.First().METHOD;
                    sSender = liSMSSetting.First().SENDER;
                    sUser = liSMSSetting.First().USER;
                    sAccusage = liSMSSetting.First().ACCUSAGE;
                    sEntityId = liSMSSetting.First().ENTITY_ID;

                    int nSMSCount = Convert.ToInt32(ConfigurationManager.AppSettings["SMSCount"]);

                    // Process SMS for the Current Class Group
                    var sortedClassItems = liItem.OrderBy(o => o.receipient_id).ToList();

                    foreach (var item in sortedClassItems)
                    {
                        list = sortedClassItems.Where(o => o.receipient_id == item.receipient_id).Take(1).ToList();

                        if (list == null || list.Count == 0)
                        {
                            bFlag = false;
                            break;
                        }

                        // Assign Values To Send Bulk SMS
                        var composeClass = new MessageResult.SMSRequestCompose
                        {
                            listsms = list,
                            user = sUser,
                            password = sAPIKey
                        };

                        var smsItem = composeClass.listsms.First();
                        smsItem.senderid = sSender;
                        smsItem.accountusagetypeid = sAccusage;
                        if (string.IsNullOrEmpty(smsItem.entityid) && !string.IsNullOrEmpty(sEntityId))
                        {
                            smsItem.entityid = sEntityId;
                        }

                        // Convert Records to JSON
                        sJsonString = new JavaScriptSerializer().Serialize(composeClass);

                        // Prepare and Send HTTP Request
                        try
                        {
                            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sBaseUrl);
                            byte[] data = new UTF8Encoding().GetBytes(sJsonString);

                            httpWReq.Method = sMethod;
                            httpWReq.ContentType = "application/json";
                            httpWReq.ContentLength = data.Length;

                            using (Stream stream = httpWReq.GetRequestStream())
                            {
                                stream.Write(data, 0, data.Length);
                            }

                            using (HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse())
                            {
                                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                                {
                                    string responseString = reader.ReadToEnd();
                                    if (nLogcount == 0)
                                    {
                                        lastInsertID = SaveSentItemFromDoveSoft(responseString, item.class_id);
                                    }
                                    SaveSendSMSListFromDoveSoft(responseString, lastInsertID, item.sms, item.class_id, item.receipient_id);

                                    ObjJsonDataSMS.Message = $"{listbulkSMS.Count} Messages Submitted successfully.";
                                    ObjJsonDataSMS.ErrorCode = Common.ErrorCode.Ok;
                                    sResultMessageSMS = Common.ErrorCode.Ok;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log Error (Handle Exception Appropriately)
                            Console.WriteLine($"Error Sending SMS: {ex.Message}");
                        }

                        nLogcount++;
                        SMSSkipcount++;
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommunicationController", "SendBulkSMSFromDovesoft", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommunicationController", "SendBulkSMSFromDovesoft", ex.Message);
                }
                ObjJsonDataSMS.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsonDataSMS.Message = Common.ErrorMessage.InternalServerError;
            }
        }
        public string SaveSentItemFromDoveSoft(string JsonResponse, string class_id)
        {
            try
            {
                // Convert Form Json To Model ....
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var ObjSMS = serializer.Deserialize<MessageResult.SMSResponse>(JsonResponse);
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";

                // Checking SMS Count ....
                if (ObjSMS.smslist.sms != null && ObjSMS.smslist.sms.Count > 0)
                {
                    ObjJsonDataSMS.Message = " Messages Submitted successfully.";
                    //  ObjJsonData.Message = ObjSMS.message;
                    var ClassIds = liSMS.GroupBy(o => o.custom1).Select(x => new MessageResult.sms() { custom1 = x.FirstOrDefault().custom1 }).ToList();
                    //sClassIds = string.Join(",", ClassIds.Select(o => o.custom1).ToArray());
                    sReceipientNo += string.Join(",", liSMS.Select(o => o.to).ToArray());
                    // sClassIds = sClassIds.TrimEnd(',');
                    // sReceipientNo = sReceipientNo.TrimEnd(',');
                    var ObjSaveRecord = new SentMessage();
                    ObjSaveRecord.CLASS_IDS = class_id;
                    ObjSaveRecord.START_TIME = sStartTime;
                    ObjSaveRecord.END_TIME = DateTime.Now.ToString(Common.Formats.TimeFormat);
                    if (sSmsMode == Common.SUP_MESSAGE_TYPE_ID.ATTENDANCE)
                    {
                        ObjSaveRecord.CONTENT = "Absentees List";
                    }
                    else if (sSmsMode == Common.SUP_MESSAGE_TYPE_ID.LATE_ENTRY)
                    {
                        ObjSaveRecord.CONTENT = "Late Comers";
                    }
                    else
                    {
                        //ObjSaveRecord.CONTENT = sTextMessage;
                        if (!string.IsNullOrEmpty(sTextMessage))
                        {
                            ObjSaveRecord.CONTENT = sTextMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                        }
                        if (sSmsMode == Common.SUP_MESSAGE_TYPE_ID.FEE_DUE_MESSAGE)
                        {
                            ObjSaveRecord.CONTENT = "Fee Due Message";
                        }
                    }
                    ObjSaveRecord.SMS_COUNT = Convert.ToString(sSmsCount);
                    ObjSaveRecord.STATUS = ObjSMS.smslist.sms.FirstOrDefault().status;
                    ObjSaveRecord.CREDIT_COUNT = sCreditCount;
                    ObjSaveRecord.SMS_MODE = sSmsMode;
                    ObjSaveRecord.RECEIPIENT_NOS = sReceipientNo;
                    ObjSaveRecord.ACADEMIC_YEAR_ID = sAcademicYear;
                    resultArgsSMS = CMSHandler.SaveOrUpdate(ObjSaveRecord, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.SaveSentMessages), "", true);
                }
                else
                {
                    var objsms = serializer.Deserialize<MessageResult.SMSSingleResponse>(JsonResponse);
                    //  ObjJsonData.Message = ObjSMS.message;
                    var ClassIds = liSMS.GroupBy(o => o.custom1).Select(x => new MessageResult.sms() { custom1 = x.FirstOrDefault().custom1 }).ToList();
                    //sClassIds = string.Join(",", ClassIds.Select(o => o.custom1).ToArray());
                    sReceipientNo += string.Join(",", liSMS.Select(o => o.to).ToArray());
                    // sClassIds = sClassIds.TrimEnd(',');
                    // sReceipientNo = sReceipientNo.TrimEnd(',');
                    var ObjSaveRecord = new SentMessage();
                    ObjSaveRecord.CLASS_IDS = class_id;
                    ObjSaveRecord.START_TIME = sStartTime;
                    ObjSaveRecord.END_TIME = DateTime.Now.ToString(Common.Formats.TimeFormat);
                    if (sSmsMode == Common.SUP_MESSAGE_TYPE_ID.ATTENDANCE)
                    {
                        ObjSaveRecord.CONTENT = "Absentees List";
                    }
                    else if (sSmsMode == Common.SUP_MESSAGE_TYPE_ID.LATE_ENTRY)
                    {
                        ObjSaveRecord.CONTENT = "Late Comers";
                    }
                    else
                    {
                        //ObjSaveRecord.CONTENT = sTextMessage;
                        if (!string.IsNullOrEmpty(sTextMessage))
                        {
                            ObjSaveRecord.CONTENT = sTextMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                        }
                        if (sSmsMode == Common.SUP_MESSAGE_TYPE_ID.FEE_DUE_MESSAGE)
                        {
                            ObjSaveRecord.CONTENT = "Fee Due Message";
                        }
                    }
                    ObjJsonDataSMS.Message = " Messages Submitted successfully.";
                    ObjSaveRecord.SMS_COUNT = Convert.ToString(sSmsCount);
                    ObjSaveRecord.STATUS = objsms.smslist.sms.status;
                    ObjSaveRecord.CREDIT_COUNT = sCreditCount;
                    ObjSaveRecord.SMS_MODE = sSmsMode;
                    ObjSaveRecord.RECEIPIENT_NOS = sReceipientNo;
                    ObjSaveRecord.ACADEMIC_YEAR_ID = sAcademicYear;
                    resultArgsSMS = CMSHandler.SaveOrUpdate(ObjSaveRecord, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.SaveSentMessages), "", true);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeController", "SaveSentItemFromDoveSoft", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeController", "SaveSentItemFromDoveSoft", ex.Message);
                }
                ObjJsonDataSMS.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsonDataSMS.Message = Common.ErrorMessage.InternalServerError;
            }
            string lstInsertedId = resultArgsSMS.RowUniqueId.ToString();

            return lstInsertedId;
        }
        static string FixJsonToArray(string jsonResponse)
        {
            // Fix concatenated objects into a valid JSON array
            string fixedJson = "[" + Regex.Replace(jsonResponse, "},\\s*\\{", "},{") + "]";
            return fixedJson;
        }
        private void SaveSendSMSListFromDoveSoft(string JsonResponse, string lastInsertID, string sms, string ClassId, string recipient)
        {
            try
            {
                // Convert Form Json To Model ....
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                JsonResponse = JsonResponse.TrimEnd(',');
                //JsonResponse = FixJsonToArray(JsonResponse);
                var ObjSMS = serializer.Deserialize<MessageResult.SMSResponse>(JsonResponse);
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";

                var todayDate = DateTime.Now.ToString("yyyy-MM-dd");
                var CurrentTime = DateTime.Now.ToString("HH:mm:ss");
                // Checking SMS Count ....
                if (ObjSMS.smslist.sms != null && ObjSMS.smslist.sms.Count > 0)
                {
                    var ObjSaveRecord = new SentMessage();
                    ObjJsonDataSMS.Message = "Messages Submitted successfully.";
                    string sSQL = string.Empty;
                    sSQL = @"INSERT INTO SENT_SMS_LIST (" + Common.SENT_SMS_LIST.SENT_ITEM_ID + "," + Common.SENT_SMS_LIST.RECEIPIENT_ID + ","
                           + Common.SENT_SMS_LIST.CLASS_ID + "," + Common.SENT_SMS_LIST.CONTENT + "," + Common.SENT_SMS_LIST.IS_STAFF + ","
                           + Common.SENT_SMS_LIST.MOBILE_NO + "," + Common.SENT_SMS_LIST.TRANSACTION_ID + "," + Common.SENT_SMS_LIST.STATUS + ","
                           + Common.SENT_SMS_LIST.CREDIT_COUNT + ")VALUES";
                    //string lstInsertedId = resultArgs.RowUniqueId.ToString();
                    string sTempMessage = string.Empty;
                    if (!string.IsNullOrEmpty(lastInsertID))
                    {
                        foreach (var item in ObjSMS.smslist.sms)
                        {
                            var customid = item.clientsmsid.ToString().Split(',');
                            var status = item.status.ToString();
                            SMSCount++;

                            var liSave = liSMS.Where(s => s.custom1 == customid[0]).ToList();
                            if (liSave != null && liSave.Count > 0)
                            {
                                foreach (var lstsave in liSave)
                                {

                                    sTempMessage = (!string.IsNullOrEmpty(sTextMessage)) ? sTextMessage : lstsave.message;
                                    sTempMessage = sTempMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                                    sSQL += @"('" + lastInsertID + "','" + lstsave.custom + "','" + lstsave.custom1 + "','" + sTempMessage + "','" + lstsave.custom2 + "','" + lstsave.to + "','" + 0 + "','" + status + "','" + lstsave.CreditCount + "'),";
                                }
                            }
                            else
                            {
                                sTextMessage = sTextMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                                sSQL += @"('" + lastInsertID + "','0','0','" + sTextMessage + "','0','" + item.mobileno + "','" + 0 + "','" + status + "','" + sCreditCount + "'),";
                            }
                        }
                        sSQL = sSQL.TrimEnd(',');

                        using (MySQLHandler ObjHandler = new MySQLHandler())
                        {
                            var resultArg = ObjHandler.ExecuteAsScripts(sSQL);
                            ObjJsonDataSMS.Message = SMSCount + " Messages Submitted successfully.";
                        }
                    }
                    else
                    {
                        ObjJsonDataSMS.Message = "Failed to send sms";
                    }
                }
                else
                {
                    var objsms = serializer.Deserialize<MessageResult.SMSSingleResponse>(JsonResponse);
                    var ObjSaveRecord = new SentMessage();
                    ObjJsonDataSMS.Message = " Messages Submitted successfully.";
                    string sSQL = string.Empty;
                    sSQL = @"INSERT INTO SENT_SMS_LIST (" + Common.SENT_SMS_LIST.SENT_ITEM_ID + "," + Common.SENT_SMS_LIST.CONTENT + "," + Common.SENT_SMS_LIST.IS_STAFF + ","
                           + Common.SENT_SMS_LIST.MOBILE_NO + "," + Common.SENT_SMS_LIST.TRANSACTION_ID + "," + Common.SENT_SMS_LIST.STATUS + ","
                           + Common.SENT_SMS_LIST.CREDIT_COUNT + ","
                           + Common.SENT_SMS_LIST.SENT_DATE + ","
                           + Common.SENT_SMS_LIST.ACADEMIC_YEAR + ","
                           + Common.SENT_SMS_LIST.SENT_TIME + ","
                           + Common.SENT_SMS_LIST.TEMPLATE_ID + ","
                           + Common.SENT_SMS_LIST.CLASS_ID + ","
                           + Common.SENT_SMS_LIST.RECEIPIENT_ID + ")VALUES";
                    //string lstInsertedId = resultArgs.RowUniqueId.ToString();
                    string sTempMessage = string.Empty;
                    if (!string.IsNullOrEmpty(lastInsertID))
                    {

                        var customids = objsms.smslist.sms.clientsmsid.ToString().Split(',');
                        var status = objsms.smslist.sms.status.ToString();
                        SMSCount++;
                        //if (liSMS != null && liSMS.Count > 0)
                        //{
                        var liSave = liSMS.Where(s => s.custom == customids[0]).ToList();
                        if (liSave != null && liSave.Count > 0)
                        {
                            foreach (var lstsave in liSave)
                            {
                                sTempMessage = (!string.IsNullOrEmpty(sTextMessage)) ? sTextMessage : lstsave.message;
                                sTempMessage = sTempMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                                sSQL += @"('" + lastInsertID + "','" + sTempMessage + "','" + lstsave.custom2 + "','" + lstsave.to + "','" + 0 + "','" + status + "','" + lstsave.CreditCount + "','" + todayDate + "','" + sAcademicYear + "','" + CurrentTime + "','0','" + ClassId + "','" + recipient + "'),";
                            }
                        }
                        else
                        {
                            sTextMessage = sms;
                            sSQL += @"('" + lastInsertID + "','" + sTextMessage + "','0','" + objsms.smslist.sms.mobileno + "','" + 0 + "','" + status + "','1','" + todayDate + "','" + sAcademicYear + "','" + CurrentTime + "','0','" + ClassId + "','" + recipient + "'),";
                        }
                        //}
                        //else
                        //{
                        //    ObjJsonData.Message = Common.Messages.NoMobileNumbersAvailable;
                        //}
                        sSQL = sSQL.TrimEnd(',');
                        sSQL = sSQL.Replace("'", "\"");
                        sSQL = sSQL.Replace("St.Joseph\"s", "St.Joseph's");
                        //resultArgs = CMSHandler.SaveOrUpdate(ObjSaveRecord, sSQL, "", true);

                        using (MySQLHandler ObjHandler = new MySQLHandler())
                        {
                            var resultArg = ObjHandler.ExecuteAsScripts(sSQL);
                            ObjJsonDataSMS.Message = SMSCount + " Messages Submitted successfully.";
                        }
                    }
                    else
                    {

                        //var ErrorResponce = JsonConvert.DeserializeObject<MessageResult.ErrorResponceFromSolutionInfi>(JsonResponse);
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("SMSController", "SaveSentItem", "Err MSg: " + JsonResponse, "Query is empty!");
                            // objHandler.WriteError("SMSController", "SaveSentItem", ErrorResponce.message);
                        }
                        ObjJsonDataSMS.Message = "FailedToSendMessage";
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SMSController", "SaveSendSMSList", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SMSController", "SaveSendSMSList", ex.Message);
                }
            }

        }
        #endregion

        #region Offline Fee Collection
        public ActionResult OfflineFeeCollection()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeeViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "OfflineFeeCollection", ex.Message);
                    }
                }
            }
            else
            {
                return View("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindFeeDetailsByRollNo(string sRollNo)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeeViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new STU_PERSONAL_INFO();
                    ObjModel.ROLL_NO = sRollNo;
                    ObjViewModel.liFeeStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeTransactionByRollNo), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "BindFeeDetailsByRollNo", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }

        public ActionResult FetchFeeDetailsByFrequencyId(string sStudentId, string sFrequencyTypeId)
        {
            var ObjViewModel = new FeeViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (sFrequencyTypeId == Common.FrequencyType.SemesterFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_ID = Common.FrequencyType.SemesterFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.MessFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_ID = Common.FrequencyType.MessFee, FEE_ROOT_ID = Common.FeeRoot.MessFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FecthMessFeesByStudentIdAndFeeRootId), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.HostelFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_ID = Common.FrequencyType.HostelFee, FEE_ROOT_ID = Common.FeeRoot.HostelFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FecthMessFeesByStudentIdAndFeeRootId), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.ExamFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_ID = Common.FrequencyType.ExamFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.AdmissionFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_ID = Common.FrequencyType.AdmissionFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.LibraryFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_ID = Common.FrequencyType.LibraryFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.ConvocationFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_ID = Common.FrequencyType.ConvocationFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.Supplementary)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_ID = Common.FrequencyType.Supplementary, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    else if (sFrequencyTypeId == Common.FrequencyType.RevaluationFee)
                        studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_ID = Common.FrequencyType.RevaluationFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), sAcademicYear).DataSource.Data;
                    ObjViewModel.liFeeStudentAccount = studentAcountList;
                    if (studentAcountList.Sum(o => decimal.Parse(o.BALANCE)) <= 0)
                    {
                        ObjJsonData.Message = "All Fee Paid";
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "FetchFeeDetailsByFrequencyId", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }

        public JsonResult SaveOfflineCollection(string sJson)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "SaveOfflineCollection", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Certificates
        public ActionResult ListCertificateFeeStructure()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    try
                    {
                        if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                        {
                            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                            var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                            if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                            {
                                objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);

                            }
                            var SupShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift), sAcademicYear).DataSource.Data;
                            if (SupShift != null && SupShift.Count > 0)
                            {
                                objViewModel.ShiftList = new SelectList(SupShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                            }
                            var SupProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode), sAcademicYear).DataSource.Data;
                            if (SupProgrammeMode != null && SupProgrammeMode.Count > 0)
                            {
                                objViewModel.ProgrammeModeList = new SelectList(SupProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                            }

                        }
                        else
                            return RedirectToAction("ErrorMessage", "error");
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("ListCertificateFeeStructure", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("ListCertificateFeeStructure", "Fee", ex.Message);
                        }
                    }
                    return View(objViewModel);

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListCertificateFeeStructure", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListCertificateFeeStructure", "Fee", ex.Message);
                }
            }
            return View(objViewModel);

        }
        public ActionResult BindFeeStructureByFrequencyIdAndModeId(string sProgrammeMode, string sFrequency, string sShift, string Academicyear)
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.PROGRAMME_MODE = sProgrammeMode;
                    objModel.FREQUENCY = sFrequency;
                    objModel.SHIFT = sShift;
                    objViewModel.lstFeeStructure = (List<FEE_STRUCTURE>)CMSHandler.FetchData<FEE_STRUCTURE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.BindFeeStructureForEditByProgrammeMode), Academicyear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindFeeStructureByFrequencyIdAndModeId", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindFeeStructureByFrequencyIdAndModeId", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult AddOrEditCertificateFeeStructure()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    {
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);

                    }
                    var SupShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift), sAcademicYear).DataSource.Data;
                    if (SupShift != null && SupShift.Count > 0)
                    {
                        objViewModel.ShiftList = new SelectList(SupShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    var SupProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode), sAcademicYear).DataSource.Data;
                    if (SupProgrammeMode != null && SupProgrammeMode.Count > 0)
                    {
                        objViewModel.ProgrammeModeList = new SelectList(SupProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AddOrEditCertificateFeeStructure", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AddOrEditCertificateFeeStructure", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindFeeStructureByFrequencyIdAndModeIdForAddOrEdit(string sProgrammeMode, string sFrequency, string sShift, string Academicyear)
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Academicyear;
                    objModel.PROGRAMME_MODE = sProgrammeMode;
                    objModel.FREQUENCY = sFrequency;
                    objModel.SHIFT = sShift;
                    objViewModel.lstFeeStructure = (List<FEE_STRUCTURE>)CMSHandler.FetchData<FEE_STRUCTURE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.BindFeeStructureForEditByModeandShift), sAcademicYear).DataSource.Data;
                    objViewModel.Bank = (List<SUP_BANK_ACCOUNT>)CMSHandler.FetchData<SUP_BANK_ACCOUNT>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupBankAccounts), sAcademicYear).DataSource.Data;
                    objViewModel.FeeCategory = (List<Sup_Fee_Category>)CMSHandler.FetchData<Sup_Fee_Category>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeeCategory), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindFeeStructureByFrequencyIdAndModeIdForAddOrEdit", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindFeeStructureByFrequencyIdAndModeIdForAddOrEdit", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult SaveOrUpdateCertificateFeeStructure(string sFeeStructure)
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                if (sFeeStructure != null)
                {
                    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                    {
                        var Result = serializer.Deserialize<FEE_STRUCTURE_LIST>(sFeeStructure);
                        foreach (var item in Result.FEE_STRUCTURE)
                        {

                            if (!string.IsNullOrEmpty(item.FEE_STRUCTURE_ID))
                            {
                                if (!string.IsNullOrEmpty(item.AMOUNT))
                                {
                                    var sresultArgs = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateCertificateFeeStructure));
                                    objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                }
                                else
                                {
                                    var DeleteFeeStructure = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteCertificateFeeStructure));
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.AMOUNT))
                                {
                                    var sresultArgs = CMSHandler.SaveOrUpdate(item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveCertificateFeeStructure));
                                    objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                }
                                else
                                {

                                }
                            }

                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                        objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                    objResult.Message = Common.ErrorMessage.BadRequest;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveOrUpdateFeeStructure", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveOrUpdateFeeStructure", "Fee", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  Student Hostel Fee Due Ui Excel Upload Bulk Filed  
        public ActionResult HostelFeeStudentAccountFileUpload()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelFeeDetails), sAcademicYear).DataSource.Data;
                    var AcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcademicYear != null && AcademicYear.Count > 0)
                    {
                        objViewModel.liAcademicYear = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var Head = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeeSupheadList)).DataSource.Data;
                    if (Head != null && Head.Count > 0)
                    {
                        objViewModel.liHead = new SelectList(Head, Common.SUP_HEAD.HEAD_ID, Common.SUP_HEAD.HEAD);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        /// <summary>
        /// Bind Academic year wise  hostel student account
        /// </summary>
        /// <author>Yesudass</author>
        /// <date>2025-06-30</date>
        /// <param name="Academicyear"></param>
        /// <returns></returns>
        public ActionResult BindHostelFeeStudentAccountbyAcYear(string Academicyear)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelFeeDetails), Academicyear).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        [HttpPost]
        public JsonResult GetHostelFeeDueByAcYear(string sAcademicYear)
        {
            if (string.IsNullOrEmpty(sAcademicYear))
                return Json(new { Success = false, Message = "Please select an academic year." }, JsonRequestBehavior.AllowGet);
            try
            {
                var data = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelFeeDetails), sAcademicYear).DataSource.Data;
                return Json(new { Success = true, Data = data ?? new List<FEE_STUDENT_DUE>() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                    objHandler.WriteError("Fee", "GetHostelFeeDueByAcYear", ex.Message);
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult FetchHostelDetailsById(string sDueID)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            try
            {
                objResultData.oResult = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { F_STUDENT_AC_ID = sDueID }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelFeeById), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("WebsiteController", "FetchAchievementsById", ex.Message);
                }
                objResultData.Message = Common.ErrorMessage.InternalServerError;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOrUpdateHostelFeeStudentAccount()
        {
            JsonResultData objResult = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FEE_STUDENT_DUE objModel = new FEE_STUDENT_DUE();
            string sSQL = string.Empty;
            try
            {
                var sJson = Request.Form["jsonData"];
                var AcademicYear = Request.Form["AcademicYear"];
                if (!string.IsNullOrEmpty(AcademicYear))
                {
                    sAcademicYear = AcademicYear;
                }
                var result = JsonConvert.DeserializeObject<FEE_STUDENT_DUE>(sJson);
                if (result != null)
                {
                    var UserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var currentDate = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    result.ENTRY_ID = UserId;
                    result.ENTRY_DATE = currentDate;
                    var UpdateDueList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateHostelFeeAccountByID));
                    if (UpdateDueList.Success)
                    {
                        result.IS_DELETED = Common.DeleteFlag.Deleted;
                        var liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(result, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters), sAcademicYear).DataSource.Data;
                        if (liFeeDueReport != null && liFeeDueReport.Count > 0)
                        {

                        }
                        else
                        {
                            var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                            var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersList));
                            var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                            var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                        }
                        objResult.ErrorCode = Common.ErrorCode.Ok;
                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;

                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                        objResult.Message = Common.Messages.FailedToSaveRecords;
                    }

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                    objResult.Message = Common.ErrorMessage.BadRequest;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", ex.Message);
                }
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = ex.Message;
                return Json(objResult, JsonRequestBehavior.AllowGet);

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SaveHostelFeeStudentAccount()
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FEE_STUDENT_DUE objModel = new FEE_STUDENT_DUE();
            var ObjViewModel = new StudentViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            string sSQL = string.Empty;
            try
            {
                var sJson = Request.Form["jsonData"];
                var acYear = Request.Form["academicYear"];
                var result = JsonConvert.DeserializeObject<FEE_STUDENT_DUE>(sJson);


                if (result != null)
                {
                    if (!string.IsNullOrEmpty(acYear) && acYear != "--select--")
                    {
                        sAcademicYear = acYear;
                    }

                    var UserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var currentDate = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    result.ENTRY_ID = UserId;
                    result.ACADEMIC_YEAR = sAcademicYear;
                    result.ENTRY_DATE = currentDate;

                    var liStuHostelFeeDueList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { HEAD = result.HEAD, ROLL_NO = result.ROLL_NO, ACADEMIC_YEAR = result.ACADEMIC_YEAR }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.HostelUpdateFeeStudentAccountByIDandTerm), sAcademicYear).DataSource.Data;

                    var liStuFeeDueList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { HEAD = result.HEAD, ROLL_NO = result.ROLL_NO, ACADEMIC_YEAR = result.ACADEMIC_YEAR }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.HostelFeeStudentAccountByIDandTerm), sAcademicYear).DataSource.Data;
                    if (liStuFeeDueList != null && liStuFeeDueList.Count > 0)
                    {

                        if (liStuHostelFeeDueList != null && liStuHostelFeeDueList.Count > 0)
                        {
                            //Update Fee Detail
                            result.STUDENT_ID = liStuHostelFeeDueList.FirstOrDefault().STUDENT_ID;
                            result.F_STUDENT_AC_ID = liStuHostelFeeDueList.FirstOrDefault().F_STUDENT_AC_ID;

                            var UpdateDueList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.HostelUpdateFeeStudent));
                            if (UpdateDueList.Success)
                            {

                                objResult.ErrorCode = Common.ErrorCode.Ok;
                                objResult.Message = Common.Messages.RecordsUpdatedSuccessfully;

                            }

                            else
                            {
                                objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                objResult.Message = Common.Messages.FailedToUpdatedRecords;
                            }
                        }
                        else
                        {

                            result.STUDENT_ID = liStuFeeDueList.FirstOrDefault().STUDENT_ID;
                            result.ACADEMIC_YEAR = sAcademicYear;
                            var InsertDueList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.AddHostelFeeAccountByID));
                            if (InsertDueList.Success)
                            {

                                objResult.ErrorCode = Common.ErrorCode.Ok;
                                objResult.Message = Common.Messages.RecordsSavedSuccessfully;

                            }

                            else
                            {
                                objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                objResult.Message = Common.Messages.FailedToSaveRecords;
                            }
                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.NotFound;
                        objResult.Message = "Give Roll No or Staff Code Not Exist";
                        return Json(objResult, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                    objResult.Message = Common.ErrorMessage.BadRequest;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", ex.Message);
                }
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = ex.Message;
                return Json(objResult, JsonRequestBehavior.AllowGet);

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteHostelFeeAccount(string fID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new FEE_STUDENT_DUE();
                    ObjModel.F_STUDENT_AC_ID = fID;
                    ObjModel.IS_DELETED = Common.DeleteFlag.Deleted;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeletedHostelFeeAccountByID));
                    if (DeleteQuery.Success)
                    {
                        ObjJsonData.Message = Common.Messages.RecordDeletedSuccessfully;
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.FailedToDeletedTryAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "DeleteFeeDueDetails", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveHostelFeeDueDetails()
        {
            FeeTransactionViewModel ObjViewModel = new FeeTransactionViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var filename = Request.Files[0].FileName;
            HttpPostedFileBase file = Request.Files[filename];
            var sFilename = Request.Files[0].FileName.Split('.');
            string columnName = string.Empty;
            string paidDateColumn = string.Empty;
            int SuccessCount = 0;
            int FailCount = 0;
            int IncorrectData = 0;
            string sMessage = string.Empty;
            string updateSQL = string.Empty;
            string insertSQL = string.Empty;
            var CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string PaymentStatus = Request.Form["Status"];
                    string extension = System.IO.Path.GetExtension(Request.Files[0].FileName).ToLower();

                    string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
                    string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/Uploads/Fee/" + sAcademicYear), Request.Files[0].FileName);

                    if (!Directory.Exists(path1))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/Uploads/Fee/" + sAcademicYear));
                    }
                    // Save File
                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);
                    }
                    Request.Files[0].SaveAs(path1);

                    byte[] fileBytes;
                    using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                    {
                        fileBytes = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                    }

                    if (validFileTypes.Contains(extension))
                    {
                        var objFeeDueList = new FEE_STUDENT_DUE();
                        var liStudentList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentDetailsFroFeeDues), sAcademicYear).DataSource.Data;
                        if (liStudentList != null && liStudentList.Count > 0)
                        {
                            using (var package = new ExcelPackage(new MemoryStream(fileBytes)))
                            {
                                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                                var worksheet = package.Workbook.Worksheets[0];
                                var studentFeeDueDetails = new List<FEE_STUDENT_DUE>();
                                if (worksheet.Dimension.End.Row <= 1 || worksheet.Dimension.End.Column < 2)
                                {
                                    ObjJsonData.Message = "The uploaded file is empty or improperly formatted.";
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    return Json(ObjJsonData);
                                }
                                else
                                {
                                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                                    {
                                        bool isValidRow = !string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text);
                                        if (isValidRow)
                                        {
                                            var lifeeDueList = new FEE_STUDENT_DUE
                                            {
                                                ROLL_NO = worksheet.Cells[row, 1].Text,
                                                HEAD = worksheet.Cells[row, 2].Text,
                                                FeeAmount = worksheet.Cells[row, 3].Text,
                                                ESTABLISHMENT = worksheet.Cells[row, 4].Text,
                                            };
                                            studentFeeDueDetails.Add(lifeeDueList);
                                        }
                                    }
                                }//

                                if (studentFeeDueDetails != null && studentFeeDueDetails.Count > 0)
                                {
                                    var supHead = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(null, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchsupHeadOnlyHeadCode), sAcademicYear).DataSource.Data;

                                    bool isPaid = true;
                                    foreach (var dr in studentFeeDueDetails)
                                    {
                                        dr.HEAD = dr.HEAD.Replace(" ", "");
                                        dr.ROLL_NO = dr.ROLL_NO.Replace(" ", "");
                                        dr.FeeAmount = dr.FeeAmount.Replace(" ", "");
                                        dr.ESTABLISHMENT = dr.ESTABLISHMENT.Replace(" ", "");
                                        var SupheadId = supHead.Where(x => x.HEAD_CODE == dr.HEAD).ToList();

                                        var liStuFeeDueList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { HEAD = SupheadId.FirstOrDefault().HEAD_ID, ROLL_NO = dr.ROLL_NO }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelDueByRollNO), sAcademicYear).DataSource.Data;

                                        switch (PaymentStatus)
                                        {
                                            case "1":
                                                columnName = "DEBIT";
                                                paidDateColumn = "PAYMENT_DATE";
                                                isPaid = true;
                                                break;
                                            case "2":
                                                columnName = "CREDIT";
                                                paidDateColumn = null;
                                                isPaid = false;
                                                break;
                                        }
                                        var SupESTheadId = supHead.Where(x => x.HEAD_CODE == "ESTFEE").ToList();
                                        var liStuFeeDueListForEst = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, HEAD = SupESTheadId.FirstOrDefault().HEAD_ID }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelDueByRollNO), sAcademicYear).DataSource.Data;
                                        if (liStuFeeDueListForEst != null)
                                        {
                                            if (dr.ESTABLISHMENT != "0")
                                            {
                                                updateSQL = isPaid ? "UPDATE hostel_fee_student_Account SET " + columnName + " = " + dr.ESTABLISHMENT +
                                      ",PAYMENT_DATE = '" + CurrentDate + "' WHERE F_STUDENT_AC_ID = " + liStuFeeDueListForEst.FirstOrDefault().F_STUDENT_AC_ID + ";" : "UPDATE hostel_fee_student_Account SET " + columnName + " = " + dr.ESTABLISHMENT + " WHERE F_STUDENT_AC_ID = " + liStuFeeDueListForEst.FirstOrDefault().F_STUDENT_AC_ID + ";";
                                                var UpdateEstDueList = CMSHandler.SaveOrUpdate(objFeeDueList, updateSQL);
                                                if (UpdateEstDueList.Success)
                                                {
                                                    var liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters), sAcademicYear).DataSource.Data;
                                                    if (liFeeDueReport != null && liFeeDueReport.Count > 0)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                                                        var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersList));
                                                        var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                                                        var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                                                    }
                                                    SuccessCount++;
                                                }
                                                else
                                                {
                                                    FailCount++;
                                                    sMessage = "Fail To Update Record Roll NO : " + dr.ROLL_NO + ":: Amount : " + dr.FeeAmount;

                                                    using (ErrorLog ObjLog = new ErrorLog())
                                                    {
                                                        ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (dr.ESTABLISHMENT != "0")
                                            {
                                                var liStudentListbyRollNO = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentDetailsFroFeeDuesByRollNo), sAcademicYear).DataSource.Data;
                                                if (liStudentListbyRollNO != null)
                                                {
                                                    insertSQL = isPaid ? "INSERT INTO hostel_fee_student_Account (STUDENT_ID, " + columnName + ", ACADEMIC_YEAR, " + paidDateColumn +
                                       ",HEAD, ENTRY_ID, ENTRY_DATE) VALUES (" + liStudentListbyRollNO.FirstOrDefault().STUDENT_ID + ", " + dr.ESTABLISHMENT + ", " +
                                       sAcademicYear + ", '" + CurrentDate + "', '" + SupESTheadId.FirstOrDefault().HEAD_ID + "'," + sStaffId + ", '" + CurrentDate + "');" :

                                       "INSERT INTO hostel_fee_student_Account (STUDENT_ID, " + columnName +
                                       ", ACADEMIC_YEAR,HEAD, ENTRY_ID, ENTRY_DATE) VALUES (" +
                                       liStudentListbyRollNO.FirstOrDefault().STUDENT_ID + ", " + dr.ESTABLISHMENT + ", " + sAcademicYear + ",'" + SupESTheadId.FirstOrDefault().HEAD_ID + "'," + sStaffId + ", '" + CurrentDate + "');";

                                                    var saveEstDueList = CMSHandler.SaveOrUpdate(objFeeDueList, insertSQL);
                                                    if (saveEstDueList.Success)
                                                    {
                                                        var liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters), sAcademicYear).DataSource.Data;
                                                        if (liFeeDueReport != null && liFeeDueReport.Count > 0)
                                                        {

                                                        }
                                                        else
                                                        {
                                                            var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                                                            var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersList));
                                                            var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                                                            var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                                                        }
                                                        SuccessCount++;
                                                    }
                                                    else
                                                    {
                                                        FailCount++;
                                                        sMessage = "Fail To Save Record Roll NO : " + dr.ROLL_NO + ":: Amount : " + dr.FeeAmount;

                                                        using (ErrorLog ObjLog = new ErrorLog())
                                                        {
                                                            ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (liStuFeeDueList != null && liStuFeeDueList.Count > 0)
                                        {
                                            objFeeDueList.STUDENT_ID = liStuFeeDueList.FirstOrDefault().STUDENT_ID;
                                            objFeeDueList.F_STUDENT_AC_ID = liStuFeeDueList.FirstOrDefault().F_STUDENT_AC_ID;
                                            objFeeDueList.HEAD = dr.HEAD;
                                            objFeeDueList.FeeAmount = dr.FeeAmount;
                                            objFeeDueList.ENTRY_ID = sStaffId;
                                            objFeeDueList.ENTRY_DATE = CurrentDate;
                                            objFeeDueList.ACADEMIC_YEAR = sAcademicYear;
                                            updateSQL = isPaid ? "UPDATE hostel_fee_student_Account SET " + columnName + " = " + objFeeDueList.FeeAmount +
                                      ", PAYMENT_DATE = '" + CurrentDate + "' WHERE F_STUDENT_AC_ID = " + objFeeDueList.F_STUDENT_AC_ID + ";" : "UPDATE hostel_fee_student_Account SET " + columnName + " = " + objFeeDueList.FeeAmount + " WHERE F_STUDENT_AC_ID = " + objFeeDueList.F_STUDENT_AC_ID + ";";

                                            var UpdateDueList = CMSHandler.SaveOrUpdate(objFeeDueList, updateSQL);

                                            string sSql = string.Empty;
                                            if (UpdateDueList.Success)
                                            {
                                                var liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters), sAcademicYear).DataSource.Data;
                                                if (liFeeDueReport != null && liFeeDueReport.Count > 0)
                                                {

                                                }
                                                else
                                                {
                                                    var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                                                    var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersList));
                                                    var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                                                    var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                                                }
                                                SuccessCount++;
                                            }
                                            else
                                            {
                                                FailCount++;
                                                sMessage = "Fail To Update Record Roll No :" + dr.ROLL_NO + ":: Amount : " + dr.FeeAmount;

                                                using (ErrorLog ObjLog = new ErrorLog())
                                                {
                                                    ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var liStudentListbyRollNO = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentDetailsFroFeeDuesByRollNo), sAcademicYear).DataSource.Data;
                                            if (liStudentListbyRollNO != null)
                                            {
                                                objFeeDueList.STUDENT_ID = liStudentListbyRollNO.FirstOrDefault().STUDENT_ID;
                                                objFeeDueList.F_STUDENT_AC_ID = liStudentListbyRollNO.FirstOrDefault().F_STUDENT_AC_ID;
                                                objFeeDueList.MONTH = dr.MONTH;
                                                objFeeDueList.FeeAmount = dr.FeeAmount;
                                                objFeeDueList.ENTRY_ID = sStaffId;
                                                objFeeDueList.HEAD = SupheadId.FirstOrDefault().HEAD_ID;
                                                objFeeDueList.ENTRY_DATE = CurrentDate;
                                                objFeeDueList.ACADEMIC_YEAR = sAcademicYear;
                                                if (dr.FeeAmount != "0")
                                                {
                                                    insertSQL = isPaid ? "INSERT INTO hostel_fee_student_Account (STUDENT_ID, " + columnName + ", ACADEMIC_YEAR, " + paidDateColumn +
                                              ",HEAD, ENTRY_ID, ENTRY_DATE) VALUES (" + objFeeDueList.STUDENT_ID + ", " + objFeeDueList.FeeAmount + ", " +
                                              sAcademicYear + ", '" + CurrentDate + "', '" + objFeeDueList.HEAD + "'," + sStaffId + ", '" + CurrentDate + "');" :

                                              "INSERT INTO hostel_fee_student_Account (STUDENT_ID, " + columnName +
                                              ", ACADEMIC_YEAR,HEAD, ENTRY_ID, ENTRY_DATE) VALUES (" +
                                              objFeeDueList.STUDENT_ID + ", " + objFeeDueList.FeeAmount + ", " + sAcademicYear + ",'" + objFeeDueList.HEAD + "'," + sStaffId + ", '" + CurrentDate + "');";

                                                    var InsertDueList = CMSHandler.SaveOrUpdate(objFeeDueList, insertSQL);

                                                    string sSql = string.Empty;
                                                    if (InsertDueList.Success)
                                                    {
                                                        var liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters), sAcademicYear).DataSource.Data;
                                                        if (liFeeDueReport != null && liFeeDueReport.Count > 0)
                                                        {

                                                        }
                                                        else
                                                        {
                                                            var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                                                            var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersList));
                                                            var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                                                            var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                                                        }
                                                        SuccessCount++;
                                                        ObjJsonData.ErrorCode = (InsertDueList.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                                        ObjJsonData.Message = (InsertDueList.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                                    }
                                                    else
                                                    {
                                                        FailCount++;
                                                        sMessage = "Fail To Save Record Roll No : " + dr.ROLL_NO + ":: Month : " + dr.HEAD; ;

                                                        using (ErrorLog ObjLog = new ErrorLog())
                                                        {
                                                            ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    FailCount++;
                                                    sMessage = "Amount is 0. Failed to save record. Roll No: " + dr.ROLL_NO + ", Month: " + dr.HEAD + ".";

                                                    using (ErrorLog ObjLog = new ErrorLog())
                                                    {
                                                        ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                sMessage = "This Roll Number Not  Exist in Student Details" + "Roll No : " + dr.ROLL_NO;

                                                using (ErrorLog ObjLog = new ErrorLog())
                                                {
                                                    ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                                                }
                                            }
                                        }
                                    }
                                    ObjJsonData.ErrorCode = (SuccessCount != 0) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    ObjJsonData.Message = SuccessCount + " Record(s) Saved Successfully !!! , " + FailCount + " Failed To Save Record(s) !!!";

                                }
                                else
                                {
                                    ObjJsonData.Message = "Oops! File Con't be Read";
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                }

                            }
                        }
                        else
                        {
                            ObjJsonData.Message = "No Student's Account Found For This Frequency";
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    sMessage = ex.Message;

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                    }
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Fee", "SaveFeePaymentUpdates", ex.Message);
                        ObjJsonData.Message = ex.Message;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;

                        return Json(ObjJsonData);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData);
        }
        #endregion

        #region  Student Bus Fee Due Ui Excel Upload Bulk Filed  
        public ActionResult BusFeeStudentAccountFileUpload()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var AcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcademicYear != null && AcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYear = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusFeeStudentAccountbyAcYear), sAcademicYear).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindAcYearWiseBusDueListList(string sAcYear)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();

            if (!string.IsNullOrEmpty(sAcYear))
            {
                objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusFeeStudentAccountbyAcYear), sAcYear).DataSource.Data;
                // fetch project type

                var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                if (liAcademicYear != null)
                {
                    objViewModel.AcademicYear = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
            }

            return View(objViewModel);
        }
        public JsonResult FetchBusDetailsById(string sDueID)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            try
            {
                objResultData.oResult = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { STUDENT_AC_ID = sDueID }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusFeeById), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("WebsiteController", "FetchAchievementsById", ex.Message);
                }
                objResultData.Message = Common.ErrorMessage.InternalServerError;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOrUpdateBusFeeStudentAccount()
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FEE_STUDENT_DUE objModel = new FEE_STUDENT_DUE();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            string sSQL = string.Empty;
            try
            {
                var sJson = Request.Form["jsonData"];
                var acYear = Request.Form["academicYear"];
                var result = JsonConvert.DeserializeObject<FEE_STUDENT_DUE>(sJson);
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(acYear) && acYear != "--select--")
                    {
                        sAcademicYear = acYear;
                    }
                    var UserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var currentDate = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    result.ENTRY_ID = UserId;
                    result.ENTRY_DATE = currentDate;
                    var UpdateDueList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateBusFeeAccountByID));
                    if (UpdateDueList.Success)
                    {
                        result.IS_DELETED = Common.DeleteFlag.Deleted;
                        var liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(result, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters), sAcademicYear).DataSource.Data;
                        if (liFeeDueReport != null && liFeeDueReport.Count > 0)
                        {

                        }
                        else
                        {
                            var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                            var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersList));
                            var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                            var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                        }
                        objResult.ErrorCode = Common.ErrorCode.Ok;
                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;

                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                        objResult.Message = Common.Messages.FailedToSaveRecords;
                    }

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                    objResult.Message = Common.ErrorMessage.BadRequest;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", ex.Message);
                }
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = ex.Message;
                return Json(objResult, JsonRequestBehavior.AllowGet);

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveBusFeeStudentAccount()
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FEE_STUDENT_DUE objModel = new FEE_STUDENT_DUE();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            string sSQL = string.Empty;
            try
            {
                var sJson = Request.Form["jsonData"];
                var acYear = Request.Form["academicYear"];
                var result = JsonConvert.DeserializeObject<FEE_STUDENT_DUE>(sJson);
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(acYear) && acYear != "--select--")
                    {
                        sAcademicYear = acYear;
                    }
                    var UserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var currentDate = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    result.ENTRY_ID = UserId;
                    result.ACADEMIC_YEAR = sAcademicYear;
                    result.ENTRY_DATE = currentDate;
                    var busHead = "137";
                    result.HEAD = busHead;

                    var liStuFeeDueList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { TERM = result.TERM, ROLL_NO = result.ROLL_NO, ACADEMIC_YEAR = result.ACADEMIC_YEAR }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.BusFeeStudentAccountByIDandTerm), sAcademicYear).DataSource.Data;

                    var liStfandstuFeeDueList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { TERM = result.TERM, ROLL_NO = result.ROLL_NO }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.BusFeeStudentandstaffAccountByIDandTerm), sAcademicYear).DataSource.Data;
                    if (liStfandstuFeeDueList != null && liStfandstuFeeDueList.Count > 0)
                    {
                        if (liStuFeeDueList != null && liStuFeeDueList.Count > 0)
                        {
                            //Update Fee Detail
                            result.HEAD = result.CREDIT;
                            result.HEAD = result.DEBIT;
                            result.STUDENT_ID = liStuFeeDueList.FirstOrDefault().STUDENT_ID;
                            result.STUDENT_AC_ID = liStuFeeDueList.FirstOrDefault().STUDENT_AC_ID;
                            var UpdateDueList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateBusFees));
                            if (UpdateDueList.Success)
                            {

                                objResult.ErrorCode = Common.ErrorCode.Ok;
                                objResult.Message = Common.Messages.RecordsUpdatedSuccessfully;

                            }

                            else
                            {
                                objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                objResult.Message = Common.Messages.FailedToUpdatedRecords;
                            }

                        }
                        else
                        {
                            result.STUDENT_ID = liStfandstuFeeDueList.FirstOrDefault().STUDENT_ID;
                            result.ROLE = liStfandstuFeeDueList.FirstOrDefault().ROLE;
                            result.ACADEMIC_YEAR = sAcademicYear;
                            var InsertDueList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.AddBusFeeAccountByID));
                            if (InsertDueList.Success)
                            {
                                //result.IS_DELETED = Common.DeleteFlag.Deleted;
                                //var liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(result, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters), sAcademicYear).DataSource.Data;
                                //if (liFeeDueReport != null && liFeeDueReport.Count > 0)

                                objResult.ErrorCode = Common.ErrorCode.Ok;
                                objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                            }
                            //else
                            //{
                            //    var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                            //    var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersList));
                            //    var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                            //    var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                            //}



                            else
                            {
                                objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                objResult.Message = Common.Messages.FailedToSaveRecords;
                            }

                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.NotFound;
                        objResult.Message = "Give Roll No or Staff Code Not Exist";
                        return Json(objResult, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                    objResult.Message = Common.ErrorMessage.BadRequest;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", ex.Message);
                }
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = ex.Message;
                return Json(objResult, JsonRequestBehavior.AllowGet);

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteBusFeeAccount(string fID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new FEE_STUDENT_DUE();
                    ObjModel.STUDENT_AC_ID = fID;
                    ObjModel.IS_DELETED = Common.DeleteFlag.Deleted;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeletedBusFeeAccountByID));
                    if (DeleteQuery.Success)
                    {
                        ObjJsonData.Message = Common.Messages.RecordDeletedSuccessfully;
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.FailedToDeletedTryAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "DeleteBusFeeAccount", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveBusFeeDueDetails()
        {
            FeeTransactionViewModel ObjViewModel = new FeeTransactionViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var filename = Request.Files[0].FileName;
            HttpPostedFileBase file = Request.Files[filename];
            var sFilename = Request.Files[0].FileName.Split('.');
            string columnName = string.Empty;
            string paidDateColumn = string.Empty;
            int SuccessCount = 0;
            int FailCount = 0;
            string sMessage = string.Empty;
            string updateSQL = string.Empty;
            string insertSQL = string.Empty;
            var CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string PaymentStatus = Request.Form["Status"];
                    string extension = System.IO.Path.GetExtension(Request.Files[0].FileName).ToLower();

                    string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
                    string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/Uploads/Fee/" + sAcademicYear), Request.Files[0].FileName);

                    if (!Directory.Exists(path1))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/Uploads/Fee/" + sAcademicYear));
                    }
                    // Save File
                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);
                    }
                    Request.Files[0].SaveAs(path1);

                    byte[] fileBytes;
                    using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                    {
                        fileBytes = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                    }

                    if (validFileTypes.Contains(extension))
                    {
                        var objFeeDueList = new FEE_STUDENT_DUE();

                        using (var package = new ExcelPackage(new MemoryStream(fileBytes)))
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            var worksheet = package.Workbook.Worksheets[0];
                            var studentFeeDueDetails = new List<FEE_STUDENT_DUE>();
                            if (worksheet.Dimension.End.Row <= 1 || worksheet.Dimension.End.Column < 2)
                            {
                                ObjJsonData.Message = "The uploaded file is empty or improperly formatted.";
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                return Json(ObjJsonData);
                            }
                            else
                            {
                                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                                {
                                    bool isValidRow = !string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text);
                                    if (isValidRow)
                                    {
                                        var lifeeDueList = new FEE_STUDENT_DUE
                                        {
                                            ROLL_NO = worksheet.Cells[row, 1].Text,
                                            TERM = worksheet.Cells[row, 2].Text,
                                            BUS_ROUTE = worksheet.Cells[row, 4].Text,
                                            FeeAmount = worksheet.Cells[row, 5].Text,
                                        };
                                        studentFeeDueDetails.Add(lifeeDueList);
                                    }
                                }
                            }//

                            if (studentFeeDueDetails != null && studentFeeDueDetails.Count > 0)
                            {

                                bool isPaid = true;
                                var busHead = "137";
                                foreach (var dr in studentFeeDueDetails)
                                {
                                    dr.TERM = dr.TERM.Replace(" ", "");
                                    dr.TERM = dr.TERM[dr.TERM.Length - 1].ToString();
                                    dr.ROLL_NO = dr.ROLL_NO.Replace(" ", "");
                                    dr.FeeAmount = dr.FeeAmount.Replace(" ", "");
                                    dr.BUS_ROUTE = dr.BUS_ROUTE.Replace(" ", "");
                                    dr.HEAD = busHead;

                                    var liStuFeeDueList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { TERM = dr.TERM, HEAD = busHead, ROLL_NO = dr.ROLL_NO }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.BusFeeStudentAccountByIDandTerm), sAcademicYear).DataSource.Data;

                                    switch (PaymentStatus)
                                    {
                                        case "1":
                                            columnName = "DEBIT";
                                            paidDateColumn = "PAYMENT_DATE";
                                            isPaid = true;
                                            break;
                                        case "2":
                                            columnName = "CREDIT";
                                            paidDateColumn = null;
                                            isPaid = false;
                                            break;
                                    }
                                    if (liStuFeeDueList != null && liStuFeeDueList.Count > 0)
                                    {
                                        objFeeDueList.STUDENT_ID = liStuFeeDueList.FirstOrDefault().STUDENT_ID;
                                        objFeeDueList.STUDENT_AC_ID = liStuFeeDueList.FirstOrDefault().STUDENT_AC_ID;
                                        objFeeDueList.HEAD = dr.HEAD;
                                        objFeeDueList.FeeAmount = dr.FeeAmount;
                                        objFeeDueList.BUS_ROUTE = dr.BUS_ROUTE;
                                        objFeeDueList.ENTRY_ID = sStaffId;
                                        objFeeDueList.ENTRY_DATE = CurrentDate;
                                        objFeeDueList.ACADEMIC_YEAR = sAcademicYear;
                                        updateSQL = isPaid ? "UPDATE bus_fee_student_Account SET " + columnName + " = " + objFeeDueList.FeeAmount +
                                  ",PAYMENT_DATE = '" + CurrentDate + "' WHERE STUDENT_AC_ID = " + objFeeDueList.STUDENT_AC_ID + ";" : "UPDATE bus_fee_student_Account SET " + columnName + " = " + objFeeDueList.FeeAmount + " WHERE STUDENT_AC_ID = " + objFeeDueList.STUDENT_AC_ID + ";";

                                        var UpdateDueList = CMSHandler.SaveOrUpdate(objFeeDueList, updateSQL);

                                        string sSql = string.Empty;
                                        if (UpdateDueList.Success)
                                        {
                                            var liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters), sAcademicYear).DataSource.Data;
                                            if (liFeeDueReport != null && liFeeDueReport.Count > 0)
                                            {

                                            }
                                            else
                                            {
                                                var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                                                var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersList));
                                                var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                                                var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                                            }
                                            SuccessCount++;
                                        }
                                        else
                                        {
                                            FailCount++;
                                            sMessage = "Fail To Update Record Roll No :" + dr.ROLL_NO + ":: Amount : " + dr.FeeAmount;

                                            using (ErrorLog ObjLog = new ErrorLog())
                                            {
                                                ObjLog.BusFeeDueFileLog(sMessage, CurrentDate);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var liStudentListbyRollNO = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchPassengerDetailsByRollNo), sAcademicYear).DataSource.Data;
                                        if (liStudentListbyRollNO != null)
                                        {
                                            objFeeDueList.STUDENT_ID = liStudentListbyRollNO.FirstOrDefault().STUDENT_ID;
                                            objFeeDueList.ROLE = liStudentListbyRollNO.FirstOrDefault().ROLE;
                                            objFeeDueList.FeeAmount = dr.FeeAmount;
                                            objFeeDueList.TERM = dr.TERM;
                                            objFeeDueList.ENTRY_ID = sStaffId;
                                            objFeeDueList.BUS_ROUTE = dr.BUS_ROUTE;
                                            objFeeDueList.HEAD = dr.HEAD;
                                            objFeeDueList.ENTRY_DATE = CurrentDate;
                                            objFeeDueList.ACADEMIC_YEAR = sAcademicYear;
                                            if (dr.FeeAmount != "0")
                                            {
                                                insertSQL = isPaid ? "INSERT INTO bus_fee_student_account (STUDENT_ID, " + columnName + ", ACADEMIC_YEAR, " + paidDateColumn +
                                          ",HEAD,TERM,BUS_ROUTE,ROLE, ENTRY_ID, ENTRY_DATE) VALUES (" + objFeeDueList.STUDENT_ID + ", " + objFeeDueList.FeeAmount + ", " +
                                          sAcademicYear + ", '" + CurrentDate + "', '" + objFeeDueList.HEAD + "', '" + objFeeDueList.TERM + "', '" + objFeeDueList.BUS_ROUTE + "', '" + objFeeDueList.ROLE + "'," + sStaffId + ", '" + CurrentDate + "');" :

                                          "INSERT INTO bus_fee_student_account (STUDENT_ID, " + columnName +
                                          ", ACADEMIC_YEAR,HEAD,TERM,BUS_ROUTE,ROLE, ENTRY_ID, ENTRY_DATE) VALUES (" +
                                          objFeeDueList.STUDENT_ID + ", " + objFeeDueList.FeeAmount + ", " + sAcademicYear + ",'" + objFeeDueList.HEAD + "', '" + objFeeDueList.TERM + "', '" + objFeeDueList.BUS_ROUTE + "', '" + objFeeDueList.ROLE + "'," + sStaffId + ", '" + CurrentDate + "');";

                                                var InsertDueList = CMSHandler.SaveOrUpdate(objFeeDueList, insertSQL);

                                                string sSql = string.Empty;
                                                if (InsertDueList.Success)
                                                {
                                                    var liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters), sAcademicYear).DataSource.Data;
                                                    if (liFeeDueReport != null && liFeeDueReport.Count > 0)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                                                        var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersList));
                                                        var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                                                        var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                                                    }
                                                    SuccessCount++;
                                                    ObjJsonData.ErrorCode = (InsertDueList.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                                    ObjJsonData.Message = (InsertDueList.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                                }
                                                else
                                                {
                                                    FailCount++;
                                                    sMessage = "Fail To Save Record Roll No : " + dr.ROLL_NO + ":: Month : " + dr.HEAD; ;

                                                    using (ErrorLog ObjLog = new ErrorLog())
                                                    {
                                                        ObjLog.BusFeeDueFileLog(sMessage, CurrentDate);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                FailCount++;
                                                sMessage = "Amount is 0. Failed to save record. Roll No: " + dr.ROLL_NO + ", Month: " + dr.HEAD + ".";

                                                using (ErrorLog ObjLog = new ErrorLog())
                                                {
                                                    ObjLog.BusFeeDueFileLog(sMessage, CurrentDate);
                                                }
                                            }

                                        }
                                        else
                                        {
                                            sMessage = "This Roll Number Not  Exist in Student Details" + "Roll No : " + dr.ROLL_NO;

                                            using (ErrorLog ObjLog = new ErrorLog())
                                            {
                                                ObjLog.BusFeeDueFileLog(sMessage, CurrentDate);
                                            }
                                        }
                                    }
                                }
                                ObjJsonData.ErrorCode = (SuccessCount != 0) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                ObjJsonData.Message = SuccessCount + " Record(s) Saved Successfully !!! , " + FailCount + " Failed To Save Record(s) !!!";

                            }
                            else
                            {
                                ObjJsonData.Message = "Oops! File Con't be Read";
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            }

                        }

                    }
                    else
                    {
                        ObjJsonData.Message = "In Valid File Type Oops! { '.xls', '.xlsx', '.csv' } This format Only Accepted";
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    sMessage = ex.Message;

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                    }
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Fee", "SaveFeePaymentUpdates", ex.Message);
                        ObjJsonData.Message = ex.Message;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;

                        return Json(ObjJsonData);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData);
        }
        #endregion

        #region Student Fee Due Ui Excel Upload Bulk Filed  

        public ActionResult FeeStudentDueDetails()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var liShift = (List<Sup_Shift>)CMS.DAO.CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    objViewModel.ShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    // Program ...

                    var AcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcademicYear != null && AcademicYear.Count > 0)
                    {
                        objViewModel.AcademicyearList = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var classYear = (List<CMS.ViewModel.Model.SubClassYear>)CMSHandler.FetchData<CMS.ViewModel.Model.SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data;
                    if (classYear != null && classYear.Count > 0)
                    {
                        objViewModel.ClassYearList = new SelectList(classYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                    }
                    var programmeLevel = (List<SupProgrammeLevel>)CMSHandler.FetchData<SupProgrammeLevel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel)).DataSource.Data;
                    if (programmeLevel != null && programmeLevel.Count > 0)
                    {
                        objViewModel.programmeLevel = new SelectList(programmeLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult BindDDLClassListbyClassYear(string sProgramId, string sShiftId, string classyear)
        {
            string sClass = string.Empty;
            string sAcademicYear = string.Empty;
            cp_Classes_2017 objModel = new cp_Classes_2017();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    objModel.SHIFT = sShiftId;
                    var Class = (List<cp_Classes_2017>)CMS.DAO.CMSHandler.FetchData<cp_Classes_2017>(objModel, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchClassByShiftMAndProgrammeGroupIdandClassYearM).Replace("?PROGRAMME_GROUP_ID", sProgramId).Replace("?CLASS_YEAR", classyear).Replace("?SHIFT", sShiftId), sAcademicYear).DataSource.Data;
                    foreach (var item in Class)
                    {
                        sClass += "<option value='" + item.CLASS_ID + "'>" + item.CLASS_NAME + "</option>";
                    }
                }
                else
                    return Json(sClass);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ConductCertificate", "StudentController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ConductCertificate", "StudentController", ex.Message);
                }
            }
            return Json(sClass);
        }
        public JsonResult GetProgramByShiftclasslevel(string sShiftId, string programmeLevel)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sOption = string.Empty;
            var liProgram = new List<CP_PROGRAMME_2017>();
            var ObjShift = new Sup_Shift();
            ObjShift.SHIFT_ID = sShiftId;
            liProgram = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchProgrammeGroupByShiftIdMAndProgrammeLevelM).Replace("?PROGRAMME_LEVEL", programmeLevel).Replace("?SHIFT", sShiftId), sAcademicYear).DataSource.Data;
            if (liProgram != null && liProgram.Count > 0)
            {
                foreach (var item in liProgram)
                {
                    sOption += "<option value='" + item.PROGRAMME_ID + "'>" + item.PROGRAMME_NAME + "</option>";
                }
            }
            return Json(sOption);
        }
        public ActionResult BindClassWiseStudentDueDetails(string sClassId, string Academicyear)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            objViewModel.liFeeDueReport = new List<FEE_STUDENT_DUE>();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (string.IsNullOrEmpty(Academicyear) || Academicyear.Contains("--select--"))
            {
                Academicyear = sAcademicYear;
            }

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.CLASS_ID = sClassId;
                    if (!string.IsNullOrEmpty(sClassId) && sClassId != "null" && sClassId != "0" && !sClassId.Contains("--select--"))
                    {
                        // Clean up multi-select class ids
                        string cleanedClassId = string.Join(",", sClassId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()));
                        if (!string.IsNullOrEmpty(cleanedClassId))
                        {
                            objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentFeeDueDetailsClasswise).Replace("?CLASS_ID", cleanedClassId), Academicyear).DataSource.Data;
                        }
                    }
                    else
                    {
                        // Default: fetch all student due details for the academic year
                        objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAllFeeDueDetailsWitharrearAmmount), Academicyear).DataSource.Data;
                    }
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "error");
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindStudentsByAcademicYearAndClassId", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindStudentsByAcademicYearAndClassId", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult UpdateStudentFeeDueDetails()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear), sAcademicYear).DataSource.Data;
                    //List<SUP_FEE_FREQUENCY> liFrequency = new List<SUP_FEE_FREQUENCY>();
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    var FetchProgrammeGroup = (List<SUP_APPTYPE_PROG_GROUPS>)CMSHandler.FetchData<SUP_APPTYPE_PROG_GROUPS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                    if (FetchProgrammeGroup != null && FetchProgrammeGroup.Count > 0)
                        objViewModel.ProgrammeGroupList = new SelectList(FetchProgrammeGroup, Common.SUP_APPTYPE_PROG_GROUPS.PROGRAMME_GROUP_ID, Common.SUP_APPTYPE_PROG_GROUPS.PROGRAMME_NAME);
                    if (FetchClassYear != null && FetchClassYear.Count > 0)
                        objViewModel.ClassYearList = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.CP_CLASSES_2017.CLASS_YEAR);
                    //liFrequency.Add(new SUP_FEE_FREQUENCY() { FREQUENCY_ID = "0", FREQUENCY_NAME = "---Select---" });
                    //objViewModel.FrequencyList = new SelectList(liFrequency, Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, Common.SUP_FEE_FREQUENCY.FREQUENCY_NAME);
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchFeeDueDetailsById(string sDueID)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            try
            {
                objResultData.oResult = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { STU_DUE_ID = sDueID }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeDueListById), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("WebsiteController", "FetchAchievementsById", ex.Message);
                }
                objResultData.Message = Common.ErrorMessage.InternalServerError;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOrUpdateFeeStudentDueDetails(string sFeeStructure, string sProgrammeGroupId, string sFrequency, string sClassYearId, string Academicyear)
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FEE_STUDENT_DUE objModel = new FEE_STUDENT_DUE();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            string sSQL = string.Empty;
            try
            {
                var AcademicYear = Request.Form["Academicyear"];
                if (!string.IsNullOrEmpty(AcademicYear))
                {
                    sAcademicYear = Academicyear;
                }
                var sJson = Request.Form["jsonData"];
                var result = JsonConvert.DeserializeObject<FEE_STUDENT_DUE>(sJson);
                if (result != null)
                {
                    var UserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    result.ENTRY_ID = UserId;
                    var UpdateDueList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateStuDueFeebyID));
                    if (UpdateDueList.Success)
                    {
                        result.IS_DELETED = Common.DeleteFlag.Deleted;
                        var liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(result, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters), sAcademicYear).DataSource.Data;
                        if (liFeeDueReport != null && liFeeDueReport.Count > 0)
                        {

                        }
                        else
                        {
                            var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                            var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersList));
                            var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                            var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                        }

                        objResult.ErrorCode = Common.ErrorCode.Ok;
                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                        objResult.Message = Common.Messages.FailedToSaveRecords;
                    }

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                    objResult.Message = Common.ErrorMessage.BadRequest;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", ex.Message);
                }
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = ex.Message;
                return Json(objResult, JsonRequestBehavior.AllowGet);

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveFeeNoDueFileUpload()
        {
            FeeTransactionViewModel ObjViewModel = new FeeTransactionViewModel();
            var ObjJsonData = new JsonResultData();
            DataTable dt = new DataTable();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var filename = Request.Files[0].FileName;
            HttpPostedFileBase file = Request.Files[filename];
            int SuccessCount = 0;
            int FailCount = 0;
            string sMessage = string.Empty;
            var CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string extension = System.IO.Path.GetExtension(Request.Files[0].FileName).ToLower();
                    string connString = string.Empty;

                    string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
                    string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/Uploads/Fee/" + sAcademicYear), Request.Files[0].FileName);

                    if (!Directory.Exists(path1))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/Uploads/Fee/" + sAcademicYear));
                    }

                    // Save File ,...
                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);
                    }
                    Request.Files[0].SaveAs(path1);

                    byte[] fileBytes;
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        fileBytes = binaryReader.ReadBytes(file.ContentLength);
                    }

                    if (validFileTypes.Contains(extension))
                    {

                        var objFeeDueList = new FEE_STUDENT_DUE();
                        var liStudentList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentDetailsFroFeeDues), sAcademicYear).DataSource.Data;
                        if (liStudentList != null && liStudentList.Count > 0)
                        {
                            using (var package = new ExcelPackage(new MemoryStream(fileBytes)))
                            {
                                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                                var worksheet = package.Workbook.Worksheets[0];
                                var studentFeeDueDetails = new List<FEE_STUDENT_DUE>();
                                if (worksheet.Dimension.End.Row <= 1 || worksheet.Dimension.End.Column < 2)
                                {
                                    ObjJsonData.Message = "The uploaded file is empty or improperly formatted.";
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    return Json(ObjJsonData);
                                }
                                else
                                {
                                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                                    {
                                        bool isValidRow = !string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text);
                                        if (isValidRow)
                                        {
                                            var lifeeDueList = new FEE_STUDENT_DUE
                                            {
                                                ROLL_NO = worksheet.Cells[row, 1].Text,
                                                TERMFEE1 = worksheet.Cells[row, 5].Text,
                                                TERMFEE1PAID = worksheet.Cells[row, 6].Text,
                                                TERMFEE1DUE = worksheet.Cells[row, 7].Text,
                                                TERMFEE2 = worksheet.Cells[row, 8].Text,
                                                TERMFEE2PAID = worksheet.Cells[row, 9].Text,
                                                TERMFEE2DUE = worksheet.Cells[row, 10].Text,
                                                EXAMFEE = worksheet.Cells[row, 11].Text,
                                                EXAMFEEPAID = worksheet.Cells[row, 12].Text,
                                                EXAMFEEDUE = worksheet.Cells[row, 13].Text,
                                                BUSFEE = worksheet.Cells[row, 14].Text,
                                                BUSFEEPAID = worksheet.Cells[row, 15].Text,
                                                BUSFEEDUE = worksheet.Cells[row, 16].Text,
                                                HOSTELFEE = worksheet.Cells[row, 17].Text,
                                                HOSTELFEEPAID = worksheet.Cells[row, 18].Text,
                                                HOSTELFEEDUE = worksheet.Cells[row, 19].Text,
                                            };

                                            studentFeeDueDetails.Add(lifeeDueList);
                                        }
                                    }
                                }//

                                if (studentFeeDueDetails != null && studentFeeDueDetails.Count > 0)
                                {
                                    var liStuFeeDueList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeDueInfoforUpdateDueList), sAcademicYear).DataSource.Data;
                                    var liFeeDueExist = new List<FEE_STUDENT_DUE>();
                                    foreach (var dr in studentFeeDueDetails)
                                    {

                                        if (liStuFeeDueList != null && liStuFeeDueList.Count > 0)
                                        {
                                            liFeeDueExist = liStuFeeDueList.Where(x => x.ROLL_NO == dr.ROLL_NO).ToList();

                                        }
                                        else
                                        {
                                            liFeeDueExist = null;
                                        }
                                        //var liFeeDueExist = liStuFeeDueList.Where(x => x.ROLL_NO == dr.ROLL_NO).ToList();
                                        if (liFeeDueExist != null && liFeeDueExist.Count > 0)
                                        {
                                            objFeeDueList.STUDENT_ID = liFeeDueExist.FirstOrDefault().STUDENT_ID;
                                            objFeeDueList.STU_DUE_ID = liFeeDueExist.FirstOrDefault().STU_DUE_ID;
                                            objFeeDueList.TERMFEE1DUE = dr.TERMFEE1DUE;
                                            objFeeDueList.TERMFEE1PAID = dr.TERMFEE1PAID;
                                            objFeeDueList.TERMFEE2 = dr.TERMFEE2;
                                            objFeeDueList.TERMFEE2DUE = dr.TERMFEE2DUE;
                                            objFeeDueList.TERMFEE2PAID = dr.TERMFEE2PAID;
                                            objFeeDueList.EXAMFEE = dr.EXAMFEE;
                                            objFeeDueList.EXAMFEEDUE = dr.EXAMFEEDUE;
                                            objFeeDueList.EXAMFEEPAID = dr.EXAMFEEPAID;
                                            objFeeDueList.BUSFEE = dr.BUSFEE;
                                            objFeeDueList.BUSFEEPAID = dr.BUSFEEPAID;
                                            objFeeDueList.BUSFEEDUE = dr.BUSFEEDUE;
                                            objFeeDueList.HOSTELFEE = dr.HOSTELFEE;
                                            objFeeDueList.HOSTELFEEPAID = dr.HOSTELFEEPAID;
                                            objFeeDueList.HOSTELFEEDUE = dr.HOSTELFEEDUE;
                                            objFeeDueList.ACADEMIC_YEAR = sAcademicYear;
                                            objFeeDueList.ENTRY_ID = sStaffId;
                                            objFeeDueList.ENTRY_DATE = CurrentDate;

                                            var UpdateDueList = CMSHandler.SaveOrUpdate(objFeeDueList, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateStuFeeDueInfo));

                                            string sSql = string.Empty;
                                            if (UpdateDueList.Success)
                                            {
                                                SuccessCount++;

                                            }
                                            else
                                            {
                                                FailCount++;
                                                sMessage = "Fail To Update Record Roll NO :" + dr.ROLL_NO;

                                                using (ErrorLog ObjLog = new ErrorLog())
                                                {
                                                    ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var liStudentListbyRollNO = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentDetailsFroFeeDuesByRollNo), sAcademicYear).DataSource.Data;
                                            if (liStudentListbyRollNO != null)
                                            {
                                                objFeeDueList.STUDENT_ID = liStudentListbyRollNO.FirstOrDefault().STUDENT_ID;
                                                objFeeDueList.TERMFEE1DUE = dr.TERMFEE1DUE;
                                                objFeeDueList.TERMFEE2 = dr.TERMFEE2;
                                                objFeeDueList.TERMFEE2DUE = dr.TERMFEE2DUE;
                                                objFeeDueList.EXAMFEE = dr.EXAMFEE;
                                                objFeeDueList.EXAMFEEDUE = dr.EXAMFEEDUE;
                                                objFeeDueList.BUSFEE = dr.BUSFEE;
                                                objFeeDueList.BUSFEEDUE = dr.BUSFEEDUE;
                                                objFeeDueList.HOSTELFEE = dr.HOSTELFEE;
                                                objFeeDueList.HOSTELFEEDUE = dr.HOSTELFEEDUE;
                                                objFeeDueList.HOSTELFEEPAID = dr.HOSTELFEEPAID;
                                                objFeeDueList.BUSFEEPAID = dr.BUSFEEPAID;
                                                objFeeDueList.TERMFEE2PAID = dr.TERMFEE2PAID;
                                                objFeeDueList.TERMFEE1PAID = dr.TERMFEE1PAID;
                                                objFeeDueList.EXAMFEEPAID = dr.EXAMFEEPAID;
                                                objFeeDueList.ACADEMIC_YEAR = sAcademicYear;
                                                objFeeDueList.ENTRY_ID = sStaffId;
                                                objFeeDueList.ENTRY_DATE = CurrentDate;

                                                var InsertDueList = CMSHandler.SaveOrUpdate(objFeeDueList, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveStuFeeDueInfo));

                                                string sSql = string.Empty;
                                                if (InsertDueList.Success)
                                                {
                                                    SuccessCount++;
                                                    ObjJsonData.ErrorCode = (InsertDueList.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                                    ObjJsonData.Message = (InsertDueList.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                                }
                                                else
                                                {
                                                    FailCount++;
                                                    sMessage = "Fail To Save Record Roll NO :" + dr.ROLL_NO;

                                                    using (ErrorLog ObjLog = new ErrorLog())
                                                    {
                                                        ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                sMessage = "This Roll No Exist in Student Details" + "Roll NO :" + dr.ROLL_NO;

                                                using (ErrorLog ObjLog = new ErrorLog())
                                                {
                                                    ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                                                }
                                            }
                                        }
                                    }
                                    ObjJsonData.ErrorCode = (SuccessCount != 0) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    ObjJsonData.Message = SuccessCount + " Record(s) Saved Successfully !!! , " + FailCount + " Failed To Save Record(s) !!!";

                                }
                                else
                                {
                                    ObjJsonData.Message = "Oops! File Con't be Read";
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                }

                            }
                        }
                        else
                        {
                            ObjJsonData.Message = "No Student's Account Found For This Frequency";
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    sMessage = ex.Message;

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                    }
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Fee", "SaveFeePaymentUpdates", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData);
        }

        public JsonResult SaveHeadWiseFeeNoDueFileUpload()
        {
            FeeTransactionViewModel ObjViewModel = new FeeTransactionViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var filename = Request.Files[0].FileName;
            HttpPostedFileBase file = Request.Files[filename];
            var sFilename = Request.Files[0].FileName.Split('.');
            int SuccessCount = 0;
            int FailCount = 0;
            int IncorrectData = 0;
            var CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string Head = Request.Form["HeadList"];
                    string PaymentStatus = Request.Form["Status"];
                    string extension = System.IO.Path.GetExtension(Request.Files[0].FileName).ToLower();

                    string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
                    string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/Uploads/Fee/" + sAcademicYear), Request.Files[0].FileName);

                    if (!Directory.Exists(path1))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/Uploads/Fee/" + sAcademicYear));
                    }

                    // Save File
                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);
                    }
                    Request.Files[0].SaveAs(path1);

                    byte[] fileBytes;
                    using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                    {
                        fileBytes = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                    }

                    if (validFileTypes.Contains(extension))
                    {
                        var objFeeDueList = new FEE_STUDENT_DUE();
                        var liStudentList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentDetailsFroFeeDues), sAcademicYear).DataSource.Data;
                        if (liStudentList != null && liStudentList.Count > 0)
                        {
                            using (var package = new ExcelPackage(new MemoryStream(fileBytes)))
                            {
                                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                                var worksheet = package.Workbook.Worksheets[0];

                                if (worksheet.Dimension.End.Row <= 1 || worksheet.Dimension.End.Column < 2)
                                {
                                    ObjJsonData.Message = "The uploaded file is empty or improperly formatted.";
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    return Json(ObjJsonData);
                                }

                                var liStuFeeDueList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeDueInfoforUpdateDueList), sAcademicYear).DataSource.Data;

                                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                                {
                                    bool isValidRow = !string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text)
                                        && !string.IsNullOrWhiteSpace(worksheet.Cells[row, 5].Text);
                                    if (isValidRow)
                                    {
                                        string RollNo = worksheet.Cells[row, 1].Text;
                                        string FeeAmount = worksheet.Cells[row, 5].Text;
                                        RollNo = RollNo.Replace(" ", "");
                                        FeeAmount = FeeAmount.Replace(" ", "");

                                        // Determine the fee type based on Head and PaymentStatus
                                        if (PaymentStatus == "1")
                                        {
                                            ProcessFee(RollNo, FeeAmount, true, Head, liStuFeeDueList, ref SuccessCount, ref FailCount, ref ObjJsonData);
                                        }
                                        else if (PaymentStatus == "2")
                                        {
                                            ProcessFee(RollNo, FeeAmount, false, Head, liStuFeeDueList, ref SuccessCount, ref FailCount, ref ObjJsonData);
                                        }
                                    }
                                    else
                                    {
                                        IncorrectData++;
                                    }
                                }
                                ObjJsonData.ErrorCode = (SuccessCount != 0) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                ObjJsonData.Message = SuccessCount + " Record(s) Saved Successfully !!! , " + FailCount + " Failed To Save Record(s) !!!  ," + IncorrectData + " Data Formet Incorrect !!!";
                            }
                        }
                        else
                        {
                            ObjJsonData.Message = "No Student's Account Found For This Frequency";
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    string sMessage = string.Empty;

                    sMessage = ex.Message;

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                    }
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Fee", "SaveFeePaymentUpdates", ex.Message);
                        ObjJsonData.Message = ex.Message;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;

                        return Json(ObjJsonData);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData);
        }



        private JsonResult ProcessFee(string rollNo, string feeAmount, bool isPaid, string head, List<FEE_STUDENT_DUE> liStuFeeDueList, ref int successCount, ref int failCount, ref JsonResultData objJsonData)
        {
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var CurrentDate = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var lifeeDueList = new FEE_STUDENT_DUE { ROLL_NO = rollNo };
            string columnName = string.Empty;
            string paidDateColumn = string.Empty;
            string sMessage = string.Empty;
            //var liStuFeeDueList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeDueInfoforUpdateDueList), sAcademicYear).DataSource.Data;
            try
            {
                switch (head)
                {
                    case "1":
                        columnName = isPaid ? "TERMFEE1PAID" : "TERMFEE1";
                        paidDateColumn = isPaid ? "TERMFEEPAID_DATE" : null;
                        lifeeDueList.TERMFEE1PAID = isPaid ? feeAmount : null;
                        lifeeDueList.TERMFEE1 = isPaid ? null : feeAmount;
                        break;
                    case "2":
                        columnName = isPaid ? "TERMFEE2PAID" : "TERMFEE2";
                        paidDateColumn = isPaid ? "TERMFEEPAID_DATE" : null;
                        lifeeDueList.TERMFEE2PAID = isPaid ? feeAmount : null;
                        lifeeDueList.TERMFEE2 = isPaid ? null : feeAmount;
                        break;
                    case "3":
                        columnName = isPaid ? "EXAMFEE1PAID" : "EXAMFEE1";
                        paidDateColumn = isPaid ? "EXAMFEE1PAID_DATE" : null;
                        lifeeDueList.EXAMFEEPAID = isPaid ? feeAmount : null;
                        lifeeDueList.EXAMFEE = isPaid ? null : feeAmount;
                        break;
                    case "4":
                        columnName = isPaid ? "BUSFEEPAID" : "BUSFEE";
                        paidDateColumn = isPaid ? "BUSFEEPAID_DATE" : null;
                        lifeeDueList.BUSFEEPAID = isPaid ? feeAmount : null;
                        lifeeDueList.BUSFEE = isPaid ? null : feeAmount;
                        break;
                    case "5":
                        columnName = isPaid ? "HOSTELFEEPAID" : "HOSTELFEE";
                        paidDateColumn = isPaid ? "HOSTELFEEPAID_DATE" : null;
                        lifeeDueList.HOSTELFEEPAID = isPaid ? feeAmount : null;
                        lifeeDueList.HOSTELFEE = isPaid ? null : feeAmount;
                        break;
                    case "6":
                        columnName = isPaid ? "EXAMFEE2PAID" : "EXAMFEE2";
                        paidDateColumn = isPaid ? "EXAMFEE2PAID_DATE" : null;
                        lifeeDueList.HOSTELFEEPAID = isPaid ? feeAmount : null;
                        lifeeDueList.HOSTELFEE = isPaid ? null : feeAmount;
                        break;
                }

                List<FEE_STUDENT_DUE> liFeeDueExist = null;
                if (liStuFeeDueList != null)
                {
                    liFeeDueExist = liStuFeeDueList.Where(x => x.ROLL_NO == rollNo).ToList();
                }

                if (liFeeDueExist != null && liFeeDueExist.Count > 0)
                {

                    string updateSQL = isPaid ? "UPDATE stu_fee_due_info SET " + columnName + " = " + feeAmount +
                                       ", " + paidDateColumn + " = '" + CurrentDate +
                                       "', ENTRY_ID = " + sStaffId +
                                       ", ENTRY_DATE = '" + CurrentDate +
                                       "' WHERE STU_DUE_ID = " + liFeeDueExist.First().STU_DUE_ID + ";" : "UPDATE stu_fee_due_info SET " + columnName + " = " + feeAmount +
                                       ", ENTRY_ID = " + sStaffId +
                                       ", ENTRY_DATE = '" + CurrentDate +
                                       "' WHERE STU_DUE_ID = " + liFeeDueExist.First().STU_DUE_ID + ";";
                    //string updateSQL = "UPDATE stu_fee_due_info SET " + columnName + " = " + feeAmount +
                    //                   ", " + paidDateColumn + " = '" + DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) +
                    //                   "', ENTRY_ID = " + sStaffId +
                    //                   ", ENTRY_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) +
                    //                   "' WHERE STU_DUE_ID = " + liFeeDueExist.First().STU_DUE_ID + ";";
                    var updateResult = CMSHandler.SaveOrUpdate(lifeeDueList, updateSQL);
                    if (updateResult.Success)
                    {
                        var liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = rollNo }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters), sAcademicYear).DataSource.Data;

                        if (liFeeDueReport != null && liFeeDueReport.Count > 0)
                        {

                        }
                        else
                        {
                            var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                            var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = rollNo, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersList));
                            var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = rollNo, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                            var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                        }
                        successCount++;
                        return Json("1");
                    }
                    else
                    {
                        failCount++;
                        sMessage = "This record Not Update" + "Roll NO :" + rollNo + ":: Amount : " + feeAmount;

                        using (ErrorLog ObjLog = new ErrorLog())
                        {
                            ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                        }
                        return Json("0");
                    }
                }
                else
                {
                    var liStudentList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = rollNo }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentDetailsFroFeeDuesByRollNo), sAcademicYear).DataSource.Data;
                    if (liStudentList != null)
                    {
                        var StudentId = liStudentList.FirstOrDefault().STUDENT_ID;

                        var liStudentExist = liStudentList.Where(x => x.ROLL_NO == rollNo).ToList();
                        string insertSQL = isPaid ? "INSERT INTO stu_fee_due_info (STUDENT_ID, " + columnName + ", ACADEMIC_YEAR, " + paidDateColumn +
                                           ", ENTRY_ID, ENTRY_DATE) VALUES (" + StudentId + ", " + feeAmount + ", " +
                                           sAcademicYear + ", '" + CurrentDate + "', " + sStaffId + ", '" + CurrentDate + "');" :

                                           "INSERT INTO stu_fee_due_info (STUDENT_ID, " + columnName +
                                           ", ACADEMIC_YEAR, ENTRY_ID, ENTRY_DATE) VALUES (" +
                                           StudentId + ", " + feeAmount + ", " + sAcademicYear + "," + sStaffId + ", '" + CurrentDate + "');";

                        //string insertSQL = "INSERT INTO stu_fee_due_info (STUDENT_ID, " + columnName +
                        //                   ", ACADEMIC_YEAR, " + paidDateColumn +
                        //                   ", ENTRY_ID, ENTRY_DATE) VALUES (" +
                        //                   StudentId + ", " +
                        //feeAmount + ", " +
                        //                   sAcademicYear + ", '" +
                        //                   DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "', " +
                        //                   sStaffId + ", '" +
                        //                   DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "');";
                        var insertResult = CMSHandler.SaveOrUpdate(lifeeDueList, insertSQL);
                        if (insertResult.Success)
                        {
                            var liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = rollNo }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters), sAcademicYear).DataSource.Data;

                            if (liFeeDueReport != null && liFeeDueReport.Count > 0)
                            {

                            }
                            else
                            {
                                var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                                var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = rollNo, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersList));
                                var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = rollNo, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                                var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                            }
                            successCount++;
                            return Json("1");
                        }
                        else
                        {
                            failCount++;
                            sMessage = "This record Not Update" + "Roll NO :" + rollNo + ":: Amount : " + feeAmount;

                            using (ErrorLog ObjLog = new ErrorLog())
                            {
                                ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                            }
                            return Json("0");
                        }
                    }
                    else
                    {
                        sMessage = "This Roll No not Exist in Student Details Roll NO :" + rollNo + ":: Amount : " + feeAmount;

                        using (ErrorLog ObjLog = new ErrorLog())
                        {
                            ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                        }
                        return Json("0");
                    }

                }

            }
            catch (Exception ex)
            {
                sMessage = ex.Message + "Roll No :" + rollNo + ":: Amount : " + feeAmount;

                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                }
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Fee", "SaveFeePaymentUpdates", ex.Message);
                    ObjJsonData.Message = ex.Message;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    return Json(ObjJsonData);
                }
            }

        }

        //public JsonResult DeleteFeeDueDetails(string fID,string sStudentID)
        //{
        public JsonResult DeleteFeeDueDetails(string fID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new FEE_STUDENT_DUE();
                    ObjModel.STU_DUE_ID = fID;
                    ObjModel.IS_DELETED = Common.DeleteFlag.Deleted;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeletedFeeStuDueDetails));
                    if (DeleteQuery.Success)
                    {
                        //ObjModel.ACADEMIC_YEAR = sAcademicYear;
                        //ObjModel.SEMESTER_TYPE = sSemesterType;
                        //ObjModel.STUDENT_ID = sStudentID;
                        //var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                        //var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteBusDefaultersList));
                        //var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteHostelDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                        //var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));

                        ObjJsonData.Message = Common.Messages.RecordDeletedSuccessfully;
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.FailedToDeletedTryAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "DeleteFeeDueDetails", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region    Fee Due Details For HOD
        public ActionResult StudentFeeDueDEtailsForHOD()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var lidepartment = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new stf_Personal_Info() { STAFF_ID = sUserId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAcademicYear).DataSource.Data;
                    if (lidepartment != null)
                    {
                        var departmnet = lidepartment.FirstOrDefault().DEPARTMENT;


                        var AcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                        if (AcademicYear != null && AcademicYear.Count > 0)
                        {
                            objViewModel.AcademicyearList = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                        }

                        var ProgrammeGroup = (List<CP_PROGRAMME_GROUP>)CMSHandler.FetchData<CP_PROGRAMME_GROUP>(new CP_PROGRAMME_GROUP() { DEPARTMENT = departmnet }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchProgrammeGroupByDepartment)).DataSource.Data;
                        if (ProgrammeGroup != null && ProgrammeGroup.Count > 0)
                        {
                            objViewModel.ProgrammeGroupList = new SelectList(ProgrammeGroup, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                        }
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }

        public ActionResult BindStudentDueDetailsForHOD(string sClassId, string Academicyear)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            objViewModel.liFeeDueReport = new List<FEE_STUDENT_DUE>();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            if (string.IsNullOrEmpty(Academicyear) || Academicyear.Contains("--select--"))
            {
                Academicyear = sAcademicYear;
            }

            try
            {
                var lidepartment = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new stf_Personal_Info() { STAFF_ID = sUserId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAcademicYear).DataSource.Data;
                if (lidepartment != null && lidepartment.Count > 0)
                {
                    var departmnet = lidepartment.FirstOrDefault().DEPARTMENT;
                    objModel.CLASS_ID = sClassId;
                    objModel.DEPARTMENT = departmnet;
                    if (!string.IsNullOrEmpty(sClassId) && sClassId != "null" && sClassId != "0" && !sClassId.Contains("--select--"))
                    {
                        string cleanedClassId = string.Join(",", sClassId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()));
                        if (!string.IsNullOrEmpty(cleanedClassId))
                        {
                            objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentFeeDueDetailsClasswise).Replace("?CLASS_ID", cleanedClassId), Academicyear).DataSource.Data;
                        }
                    }
                    else
                    {
                        objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeDueDetailsByDepartmentIdArrear), Academicyear).DataSource.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindStudentDueDetailsForHOD", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindStudentDueDetailsForHOD", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult BindDDLClassListbyProgramme(string sProgramId)
        {
            string sClass = string.Empty;
            string sAcademicYear = string.Empty;
            cp_Classes_2017 objModel = new cp_Classes_2017();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    if (!string.IsNullOrEmpty(sProgramId) && sProgramId != "null" && sProgramId != "0")
                    {
                        string cleanedProgramId = string.Join(",", sProgramId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()));
                        var Class = (List<cp_Classes_2017>)CMS.DAO.CMSHandler.FetchData<cp_Classes_2017>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchClassListByProgrammeGroupId).Replace("?PROGRAMME_GROUP_ID", cleanedProgramId), sAcademicYear).DataSource.Data;
                        if (Class != null && Class.Count > 0)
                        {
                            foreach (var item in Class)
                            {
                                sClass += "<option value='" + item.CLASS_ID + "'>" + item.CLASS_NAME + "</option>";
                            }
                        }
                        else
                        {
                            sClass += "<option value='0'>--- Select --- </option>";
                        }
                    }
                    else
                    {
                        sClass += "<option value='0'>--- Select --- </option>";
                    }
                }
                else
                    return Json(sClass);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindDDLClassListbyProgramme", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindDDLClassListbyProgramme", "Fee", ex.Message);
                }
            }
            return Json(sClass);
        }

        #endregion

        #region Fee Due Details For Class Mentor
        public ActionResult FeeDueDetailsForClassMentor()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var lidepartment = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new stf_Personal_Info() { STAFF_ID = sUserId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAcademicYear).DataSource.Data;
                    if (lidepartment != null)
                    {
                        var departmnet = lidepartment.FirstOrDefault().DEPARTMENT;


                        var AcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                        if (AcademicYear != null && AcademicYear.Count > 0)
                        {
                            objViewModel.AcademicyearList = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                        }

                        var ProgrammeGroup = (List<CP_PROGRAMME_GROUP>)CMSHandler.FetchData<CP_PROGRAMME_GROUP>(new CP_PROGRAMME_GROUP() { DEPARTMENT = departmnet }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchProgrammeGroupByDepartment)).DataSource.Data;
                        if (ProgrammeGroup != null && ProgrammeGroup.Count > 0)
                        {
                            objViewModel.ProgrammeGroupList = new SelectList(ProgrammeGroup, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                        }
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindFeeDueDetailsForClassMentor(string sClassId, string Academicyear)
        {
            //string sFrequency,
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            try
            {
                var lidepartment = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new stf_Personal_Info() { STAFF_ID = sUserId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAcademicYear).DataSource.Data;
                if (lidepartment != null)
                {
                    var departmnet = lidepartment.FirstOrDefault().DEPARTMENT;
                    // objModel.PROGRAMME_GROUP_ID = sProgrammeGroupId;
                    objModel.CLASS_ID = sClassId;
                    objModel.DEPARTMENT = departmnet;
                    if (sClassId != null && sClassId != "null")
                    {
                        objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentFeeDueDetails).Replace("?CLASS_ID", sClassId), Academicyear).DataSource.Data;
                    }
                    else
                    {
                        objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeDueDetailsByDepartmentId), sAcademicYear).DataSource.Data;
                    }

                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindStudentsByAcademicYearAndClassId", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindStudentsByAcademicYearAndClassId", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        #endregion

        #region Exam Fee Report For COE
        public ActionResult ExamFeeReportForCOE()
        {
            string sAcademicYear = string.Empty;
            sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    TransferCertificate objTransferCertificate = new TransferCertificate();
                    List<Sup_Shift> liShift = new List<Sup_Shift>();
                    liShift = (List<Sup_Shift>)CMS.DAO.CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    objTransferCertificate.ShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    // Program ...
                    DataTable dtProgram = new DataTable();
                    dtProgram = SupportDataMethod.FetchProgram(sAcademicYear).DataSource.Table;
                    List<cp_Program_2017> liProgram = new List<cp_Program_2017>();
                    if (dtProgram != null && dtProgram.Rows.Count > 0)
                    {
                        liProgram = (from DataRow dr in dtProgram.Rows
                                     select new cp_Program_2017()
                                     {
                                         PROGRAMME_ID = dr[Common.CP_PROGRAMME_2017.PROGRAMME_ID].ToString(),
                                         PROGRAMME_NAME = dr[Common.CP_PROGRAMME_2017.PROGRAMME_NAME].ToString()
                                     }).ToList();
                        objTransferCertificate.ProgramName = new SelectList(liProgram, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    }
                    var ExamFeeLastpdate = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchExamLastUpdate), sAcademicYear).DataSource.Data;
                    if (ExamFeeLastpdate != null && ExamFeeLastpdate.Count > 0)
                    {
                        ViewBag.ExamLastUpdateDate = ExamFeeLastpdate.FirstOrDefault().EXAMFEEPAID_DATE;
                    }
                    var AcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcademicYear != null && AcademicYear.Count > 0)
                    {
                        objTransferCertificate.AcademicYear = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var classYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data;
                    if (classYear != null && classYear.Count > 0)
                    {
                        objTransferCertificate.ClassYear = new SelectList(classYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                    }
                    var programmeLevel = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationTypeNew)).DataSource.Data;
                    if (programmeLevel != null && programmeLevel.Count > 0)
                    {
                        objTransferCertificate.programmeLevel = new SelectList(programmeLevel, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                    }
                    var liclassLevel = DapperHandler.FetchAll<SupClassLevel>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassLevel)).ToList();
                    if (liclassLevel != null && liclassLevel.Count > 0)
                    {
                        objTransferCertificate.ClassLevel = new SelectList(liclassLevel, Common.SUP_CLASS_LEVEL.CLASSLEVELID, Common.SUP_CLASS_LEVEL.CLASSLEVEL);
                    }
                    return View(objTransferCertificate);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StudentController", "TransferCertificateSerialNo", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StudentController", "TransferCertificateSerialNo", ex.Message);
                    }
                    sResult = "Fail To Fetch Record(s) ...!";
                    return Json(sResult, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
        }

        public ActionResult BindExamFeePaiUnPiadData(string sClassId, string sProgramId, string sAcademicYear, string sStatus, string feeType)
        {
            //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new StudentViewModel();
            var ObjJsoneData = new JsonResultData();
            ViewBag.FeeType = feeType;
            //string sFrequency,
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            try
            {
                objModel.CLASS_ID = sClassId;
                if (sClassId != null && sClassId != "null")
                {
                    if (sStatus == "1")
                    {
                        if (feeType == "1")
                        {
                            objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchExamFeepaidStatus).Replace("-- paid", "").Replace("?CLASS_ID", sClassId), sAcademicYear).DataSource.Data;

                        }
                        else
                        {
                            objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchExamFeepaidStatus).Replace("-- paid2", "").Replace("?CLASS_ID", sClassId), sAcademicYear).DataSource.Data;

                        }
                    }
                    else if (sStatus == "2")
                    {
                        if (feeType == "1")
                        {
                            objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchExamFeepaidStatus).Replace("-- unpaid", "").Replace("?CLASS_ID", sClassId), sAcademicYear).DataSource.Data;

                        }
                        else
                        {
                            objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchExamFeepaidStatus).Replace("-- unpaid2", "").Replace("?CLASS_ID", sClassId), sAcademicYear).DataSource.Data;

                        }
                    }

                    else
                    {
                        objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(objModel, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchExamFeepaidStatus).Replace("?CLASS_ID", sClassId), sAcademicYear).DataSource.Data;

                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindStudentsByAcademicYearAndClassId", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindStudentsByAcademicYearAndClassId", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        #endregion
        public JsonResult loadFrequencyByAcademicYearForMessAndHostel(string sAcademicYear, string sFeeRoot)
        {
            JsonResultData objResult = new JsonResultData();
            cp_Classes_2017 objModel = new cp_Classes_2017();
            string sSql = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var SupFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(new SUP_FEE_FREQUENCY() { FEE_ROOT_ID = sFeeRoot }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeModeAndFeeRootId), sAcademicYear).DataSource.Data;
                objResult.sResult += "<option value='0'>--select--</option>";
                if (SupFrequency != null && SupFrequency.Count > 0)
                {
                    foreach (var item in SupFrequency)
                        objResult.sResult += "<option value='" + item.FREQUENCY_ID + "'>" + item.FREQUENCY_NAME + "</option>";
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                objResult.Message = Common.ErrorMessage.RequestTimeout;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public void UpdateTransferByPaymentId(string sAcademicYear)
        {
            JsonResultData oResult = new JsonResultData();
            string sSQL = string.Empty;

            string secret = string.Empty; string key = string.Empty;
            //sAcademicYear = sAcademicYear;

            sSQL = @"SELECT 
                          p.ID
                        FROM
                            fee_razorpay_payment_info_?AC_YEAR AS p
                                LEFT JOIN
                            fee_razorpay_transfer_info_?AC_YEAR AS t ON t.SOURCE = p.id
                        WHERE
                            t.RAZORPAY_TRANSFER_ID IS NULL; ";


            List<FEE_RAZORPAY_PAYMENT_INFO> payments = new List<FEE_RAZORPAY_PAYMENT_INFO>();
            payments = (List<FEE_RAZORPAY_PAYMENT_INFO>)CMSHandler.FetchData<FEE_RAZORPAY_PAYMENT_INFO>(null, sSQL, sAcademicYear).DataSource.Data;
            var FetchMarchantAccount = (List<FEE_RAZORPAY_ACCOUNTS>)CMSHandler.FetchData<FEE_RAZORPAY_ACCOUNTS>(new FEE_RAZORPAY_ACCOUNTS() { RAZORPAY_ACCOUNT_TYPE_ID = Common.RazorpayAccountType.MarchantAccount }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRazorpayAccountInfo), sAcademicYear).DataSource.Data;

            if (FetchMarchantAccount != null & FetchMarchantAccount.Count() > 0)
            {
                key = FetchMarchantAccount.FirstOrDefault().KEY;
                secret = FetchMarchantAccount.FirstOrDefault().SECRET_KEY;

                if (payments != null && payments.Count > 0)
                {
                    foreach (var pitem in payments)
                    {
                        TransferResponse TransferResponse = JsonConvert.DeserializeObject<TransferResponse>(CommonMethods.GetMethodForRazorpay("https://api.razorpay.com/v1/payments/" + pitem.ID + "/transfers", key, secret));
                        if (TransferResponse != null)
                        {
                            if (TransferResponse.items != null && TransferResponse.items.Count > 0)
                            {
                                string sTransferSQL = @"INSERT INTO `FEE_RAZORPAY_TRANSFER_INFO_?AC_YEAR`
                                    (
                                    `ID`,
                                    `ENTITY`,
                                    `SOURCE`,
                                    `RECIPIENT`,
                                    `AMOUNT`,
                                    `CURRENCY`,
                                    `AMOUNT_REVERSED`,
                                    `FEES`,
                                    `TAX`,
                                    `ON_HOLD`,
                                    `ON_HOLD_UNTIL`,
                                    `RECIPIENT_SETTLEMENT_ID`,
                                    `CREATED_AT`,
                                    `UDF1`,
                                    `UDF2`,
                                    `UDF3`,
                                    `UDF4`,
                                    `UDF5`,
                                    `UDF6`)
                                    VALUES ";
                                foreach (var item in TransferResponse.items)
                                {

                                    //   TransferItemResponse item = JsonConvert.DeserializeObject<TransferItemResponse>(transferItem.Attributes.ToString());
                                    sTransferSQL += @"('" + item.id + "','" + item.entity + "','" + item.source + "','" + item.recipient + "','" + item.amount + "','" + item.currency + "'" +
                                        ",'" + item.amount_reversed + "','" + item.fees + "','" + item.tax + "','" + item.on_hold + "','" + item.on_hold_until + "'" +
                                        ",'" + item.recipient_settlement_id + "','" + item.created_at + "','" + item.notes.udf1 + "','" + item.notes.udf2 + "','" + item.notes.udf3 + "'" +
                                        ",'" + item.notes.udf4 + "','" + item.notes.udf5 + "','" + item.notes.udf6 + "'),";
                                }
                                //sSQL = sSQL.TrimEnd(',');
                                sTransferSQL = sTransferSQL.TrimEnd(',');
                                var result = CMSHandler.SaveOrUpdate(null, sTransferSQL, sAcademicYear);
                                // bResult = result.Success;
                            }
                        }
                    }
                    UpdateSettlementsInfo(sAcademicYear);
                }


            }

        }
        #region ADD BUTTON
        public JsonResult AddTermAndExamFee(string FeeStudent)
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FEE_STUDENT_DUE objModel = new FEE_STUDENT_DUE();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSQL = string.Empty;
            string sUpdateSQL = string.Empty;

            //string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var CurrentDate = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            //var lifeeDueList = new FEE_STUDENT_DUE { ROLL_NO = FeeStudent };
            string columnName = string.Empty;
            string paidDateColumn = string.Empty;
            string sMessage = string.Empty;


            try
            {

                var acYear = Request.Form["academicYear"];
                var result = JsonConvert.DeserializeObject<FEE_STUDENT_DUE>(FeeStudent);
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(acYear) && acYear != "--select--")
                    {
                        sAcademicYear = acYear;
                    }
                    var UserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var currentDate = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    result.ENTRY_ID = UserId;
                    result.ENTRY_DATE = currentDate;

                    var liStuFeeDueList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { SEMESTER_TYPE = result.SEMESTER_TYPE, ROLL_NO = result.ROLL_NO }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentIDByRollId), sAcademicYear).DataSource.Data;
                    var liStfandstuFeeDueList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { TERM = result.TERM, ROLL_NO = result.ROLL_NO }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentByRollId), sAcademicYear).DataSource.Data;
                    if (liStfandstuFeeDueList != null && liStfandstuFeeDueList.Count > 0)
                    {
                        if (liStuFeeDueList != null && liStuFeeDueList.Count > 0)
                        {
                            // update fee detaisl 

                            switch (result.TERM)
                            {
                                case "1":
                                    sUpdateSQL = SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateTerm1Fees);
                                    result.TERMFEE1 = result.CREDIT;
                                    result.TERMFEE1PAID = result.DEBIT;
                                    break;
                                case "2":
                                    sUpdateSQL = SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateTerm2Fees_list);
                                    result.TERMFEE2 = result.CREDIT;
                                    result.TERMFEE2PAID = result.DEBIT;
                                    break;
                                case "3":
                                    sUpdateSQL = SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateExamFees);
                                    result.EXAMFEE = result.CREDIT;
                                    result.EXAMFEEPAID = result.DEBIT;
                                    break;

                            }

                            result.STU_DUE_ID = liStuFeeDueList.FirstOrDefault().STU_DUE_ID;
                            result.STUDENT_ID = liStfandstuFeeDueList.FirstOrDefault().STUDENT_ID;
                            result.ACADEMIC_YEAR = sAcademicYear;
                            var UpdateDueList = CMSHandler.SaveOrUpdate(result, sUpdateSQL);
                            if (UpdateDueList.Success)
                            {
                                result.IS_DELETED = Common.DeleteFlag.Deleted;
                                var liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(result, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters), sAcademicYear).DataSource.Data;
                                if (liFeeDueReport != null && liFeeDueReport.Count > 0)
                                {

                                }
                                else
                                {
                                    var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                                    var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersList));
                                    var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                                    var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                                }
                                objResult.ErrorCode = Common.ErrorCode.Ok;
                                objResult.Message = Common.Messages.RecordsSavedSuccessfully;

                            }

                            else
                            {
                                objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                objResult.Message = Common.Messages.FailedToSaveRecords;
                            }
                        }
                        else
                        {

                            switch (result.TERM)
                            {
                                case "1":
                                    sSQL = SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.AddTerm1Fees);
                                    result.TERMFEE1 = result.CREDIT;
                                    result.TERMFEE1PAID = result.DEBIT;
                                    break;
                                case "2":
                                    sSQL = SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.AddTerm2Fees_list);
                                    result.TERMFEE2 = result.CREDIT;
                                    result.TERMFEE2PAID = result.DEBIT;
                                    break;
                                case "3":
                                    sSQL = SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.AddExamFees);
                                    result.EXAMFEE = result.CREDIT;
                                    result.EXAMFEEPAID = result.DEBIT;
                                    break;

                            }
                            result.STUDENT_ID = liStfandstuFeeDueList.FirstOrDefault().STUDENT_ID;
                            result.ACADEMIC_YEAR = sAcademicYear;
                            var InsertDueList = CMSHandler.SaveOrUpdate(result, sSQL);
                            if (InsertDueList.Success)
                            {

                                objResult.ErrorCode = Common.ErrorCode.Ok;
                                objResult.Message = Common.Messages.RecordsSavedSuccessfully;

                            }

                            else
                            {
                                objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                objResult.Message = Common.Messages.FailedToSaveRecords;
                            }
                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.NotFound;
                        objResult.Message = "Give Roll No Not Exist";
                        return Json(objResult, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                    objResult.Message = Common.ErrorMessage.BadRequest;
                }
            }



            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", ex.Message);
                }
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = ex.Message;
                return Json(objResult, JsonRequestBehavior.AllowGet);

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }






        #endregion
        public string test()
        {

            string[] arry = @"pay_FudteB5EpQP5wO,
pay_FueeuHrXxLwLnX,
pay_FvPiCAuuzRXLDG,
pay_FvRYYXcHHk3Tn6,
pay_FvVdZQXBPzwMFn,
pay_FvZ6v0KKwa4HQ8,
pay_FvZHUunIqtpN6S

".Split(',')
        ;


            for (int i = 0; i < arry.Length; i++)
            {
                RazorPayPaymentResponse(arry[i].TrimEnd(), "2020");
            }



            return "done";
        }

        public ActionResult HostelFeeStudentAccountFileUploadsjctnc()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { FREQUENCY_TYPE = Common.FrequencyType.HostelFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelFeeDetailsSjctnc), sAcademicYear).DataSource.Data;
                    var AcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcademicYear != null && AcademicYear.Count > 0)
                    {
                        objViewModel.liAcademicYear = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var Head = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeeSupheadList)).DataSource.Data;
                    if (Head != null && Head.Count > 0)
                    {
                        objViewModel.liHead = new SelectList(Head, Common.SUP_HEAD.HEAD_ID, Common.SUP_HEAD.HEAD);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }

        public ActionResult BindHostelFeeStudentAccountbyAcYearsjctnc(string Academicyear)
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelFeeDetailsSjctnc), Academicyear).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }

        public JsonResult FetchHostelDetailsByIdsjctnc(string sDueID)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            try
            {
                objResultData.oResult = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { F_STUDENT_AC_ID = sDueID }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelFeeByIdSjctnc), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("WebsiteController", "FetchAchievementsById", ex.Message);
                }
                objResultData.Message = Common.ErrorMessage.InternalServerError;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveOrUpdateHostelFeeStudentAccountsjctnc()
        {
            JsonResultData objResult = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FEE_STUDENT_DUE objModel = new FEE_STUDENT_DUE();
            string sSQL = string.Empty;
            try
            {
                var sJson = Request.Form["jsonData"];
                var AcademicYear = Request.Form["AcademicYear"];
                if (!string.IsNullOrEmpty(AcademicYear))
                {
                    sAcademicYear = AcademicYear;
                }
                var result = JsonConvert.DeserializeObject<FEE_STUDENT_DUE>(sJson);
                if (result != null)
                {
                    var UserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var currentDate = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    result.ENTRY_ID = UserId;
                    result.ENTRY_DATE = currentDate;

                    var frequencyList = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.HostelFee, ACADEMIC_YEAR = sAcademicYear }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyByFeeModeAndYear)).DataSource.Data;
                    if (frequencyList == null || frequencyList.Count == 0)
                    {
                        frequencyList = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { ACADEMIC_YEAR = sAcademicYear, FEE_MODE = Common.FrequencyType.HostelFee }, "SELECT FREQUENCY_ID FROM FEE_FREQUENCY_FEE_MODE WHERE FEE_MODE = ?FEE_MODE AND ACADEMIC_YEAR=?AC_YEAR ORDER BY FREQUENCY_ID DESC;").DataSource.Data;
                    }
                    if (frequencyList != null && frequencyList.Count > 0)
                    {
                        result.FREQUENCY_ID = frequencyList.FirstOrDefault().FREQUENCY_ID;
                    }
                    var supHead = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(new SUP_HEAD() { HEAD_ID = result.HEAD, FEE_MODE = Common.FrequencyType.HostelFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeadbyFeeMode), sAcademicYear).DataSource.Data;
                    if (supHead != null && supHead.Count > 0)
                    {
                        result.FEE_MAIN_HEAD_ID = supHead.FirstOrDefault().FEE_MAIN_HEAD_ID;
                    }

                    if (string.IsNullOrEmpty(result.F_STUDENT_AC_ID) || result.F_STUDENT_AC_ID == "0")
                    {
                        var liStudentListbyRollNO = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = result.ROLL_NO }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentByRollId)).DataSource.Data;
                        if (liStudentListbyRollNO != null && liStudentListbyRollNO.Count > 0)
                        {
                            result.STUDENT_ID = liStudentListbyRollNO.FirstOrDefault().STUDENT_ID;
                            string creditAmount = string.IsNullOrEmpty(result.CREDIT) ? "0" : result.CREDIT;
                            string debitAmount = string.IsNullOrEmpty(result.DEBIT) ? "0" : result.DEBIT;

                            string insertSQL = "INSERT INTO fee_student_account (STUDENT_ID, CREDIT, DEBIT, ACADEMIC_YEAR, HEAD, FREQUENCY_ID, FEE_MAIN_HEAD_ID) VALUES (" +
                                result.STUDENT_ID + ", '" + creditAmount + "', '" + debitAmount + "', " + sAcademicYear + ",'" + result.HEAD + "', " + result.FREQUENCY_ID + ", '" + result.FEE_MAIN_HEAD_ID + "');";

                            var InsertDueList = CMSHandler.SaveOrUpdate(result, insertSQL);
                            if (InsertDueList.Success)
                            {
                                objResult.ErrorCode = Common.ErrorCode.Ok;
                                objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                            }
                            else
                            {
                                objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                objResult.Message = Common.Messages.FailedToSaveRecords;
                            }
                        }
                        else
                        {
                            objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                            objResult.Message = "Student Roll No not found!";
                        }
                    }
                    else
                    {
                        var UpdateDueList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateHostelFeeAccountByIDSjctnc));
                        if (UpdateDueList.Success)
                        {
                            result.IS_DELETED = Common.DeleteFlag.Deleted;
                            var liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(result, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters), sAcademicYear).DataSource.Data;
                            if (liFeeDueReport != null && liFeeDueReport.Count > 0)
                            {

                            }
                            else
                            {
                                var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                                var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersList));
                                var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(result, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                                var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                            }
                            objResult.ErrorCode = Common.ErrorCode.Ok;
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;

                        }
                        else
                        {
                            objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                    objResult.Message = Common.ErrorMessage.BadRequest;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveOrUpdateFeeStudentAccount", "Fee", ex.Message);
                }
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = ex.Message;
                return Json(objResult, JsonRequestBehavior.AllowGet);

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteHostelFeeAccountsjctnc(string fID, string Academicyear = "")
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (!string.IsNullOrEmpty(Academicyear)) ? Academicyear : ((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new FEE_STUDENT_DUE();
                    ObjModel.F_STUDENT_AC_ID = fID;
                    ObjModel.IS_DELETED = Common.DeleteFlag.Deleted;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeletedHostelFeeAccountByIDSjctnc), sAcademicYear);
                    if (DeleteQuery.Success)
                    {
                        ObjJsonData.Message = Common.Messages.RecordDeletedSuccessfully;
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.FailedToDeletedTryAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "DeleteFeeDueDetails", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteUnpaidHostelFeeAccountItem(string fID, string Academicyear = "")
        {
            return DeleteHostelFeeAccountsjctnc(fID, Academicyear);
        }

        public JsonResult DeleteAllUnpaidHostelFeeForStudent(string sStudentId, string sFrequencyId, string Academicyear)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (!string.IsNullOrEmpty(Academicyear)) ? Academicyear : ((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new FEE_STUDENT_ACCOUNT();
                    ObjModel.STUDENT_ID = sStudentId;
                    ObjModel.FREQUENCY_ID = sFrequencyId;
                    ObjModel.ACADEMIC_YEAR = sAcademicYear;
                    var sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteAllUnpaidHostelFeeForStudent);
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, sSQL, sAcademicYear);
                    if (DeleteQuery.Success)
                    {
                        ObjJsonData.Message = "Unpaid fee account(s) deleted successfully!";
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.FailedToDeletedTryAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeeController", "DeleteAllUnpaidHostelFeeForStudent", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetHostelHeadCodes(string academicYear, string frequencyId, string mainHeadId)
        {
            try
            {
                string sAcademicYear = academicYear;
                if (string.IsNullOrEmpty(sAcademicYear) && Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                }

                if (string.IsNullOrEmpty(frequencyId) || frequencyId == "0")
                {
                    return Json(new { Success = false, Message = "Please select Frequency." }, JsonRequestBehavior.AllowGet);
                }

                string mainHeadFilter = "";
                if (!string.IsNullOrEmpty(mainHeadId) && mainHeadId != "0")
                {
                    var mainHeadIds = mainHeadId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(x => x.Trim())
                                                .Where(x => !string.IsNullOrEmpty(x) && x != "0");
                    string idsList = string.Join(",", mainHeadIds);
                    if (!string.IsNullOrEmpty(idsList))
                    {
                        mainHeadFilter = " AND fmh.MAIN_HEAD_ID IN (" + idsList + ") ";
                    }
                }

                string yearFilter = "";
                if (!string.IsNullOrEmpty(sAcademicYear) && sAcademicYear != "0" && !sAcademicYear.Contains("--"))
                {
                    yearFilter = " AND fmh.ACADEMIC_YEAR = @AC_YEAR ";
                }

                string query = @"
                    SELECT 
                        fmh.FEE_MAIN_HEAD_ID,
                        fmh.MAIN_HEAD_ID,
                        fmh.FREQUENCY_ID,
                        fmh.HEAD_ID,
                        IFNULL(sh.HEAD_CODE, '') AS HEAD_CODE,
                        IFNULL(sh.HEAD, '') AS HEAD_NAME
                    FROM fee_main_heads fmh
                    INNER JOIN sup_head sh ON sh.HEAD_ID = fmh.HEAD_ID
                    WHERE fmh.FREQUENCY_ID = @FREQUENCY_ID
                      " + yearFilter + @"
                      " + mainHeadFilter + @"
                      AND fmh.IS_DELETED != 1
                      AND sh.IS_DELETED != 1
                    ORDER BY sh.HEAD_CODE, sh.HEAD;";

                var list = DapperHandler.FetchAll<HOSTEL_HEAD_CODE_ITEM>(query, new { AC_YEAR = sAcademicYear, FREQUENCY_ID = frequencyId }).ToList();
                var cleanList = list.Select(x => new
                {
                    FEE_MAIN_HEAD_ID = x.FEE_MAIN_HEAD_ID,
                    MAIN_HEAD_ID = x.MAIN_HEAD_ID,
                    FREQUENCY_ID = x.FREQUENCY_ID,
                    HEAD_ID = x.HEAD_ID,
                    HEAD_CODE = x.HEAD_CODE,
                    HEAD_NAME = x.HEAD_NAME
                }).ToList();

                return Json(new { Success = true, Data = cleanList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveSingleHostelFeeAccount(long studentId, string academicYear, string frequencyId, string headId, string feeMainHeadId, decimal amount)
        {
            try
            {
                if (studentId <= 0)
                {
                    return Json(new { Success = false, Message = "Please enter and verify a valid Student Roll No." });
                }
                if (amount <= 0)
                {
                    return Json(new { Success = false, Message = "Fee amount must be greater than zero." });
                }
                if (string.IsNullOrEmpty(academicYear) || string.IsNullOrEmpty(frequencyId) || frequencyId == "0")
                {
                    return Json(new { Success = false, Message = "Academic Year and Frequency are required." });
                }
                if (string.IsNullOrEmpty(headId) || headId == "0")
                {
                    return Json(new { Success = false, Message = "Please select a valid Head Code." });
                }

                // Fetch student info for clear messaging
                var studentInfo = DapperHandler.FetchAll<dynamic>("SELECT STUDENT_ID, ROLL_NO, UPPER(CONCAT(IFNULL(FIRST_NAME, ''), ' ', IFNULL(LAST_NAME, ''))) AS NAME FROM stu_personal_info WHERE STUDENT_ID = @STUDENT_ID LIMIT 1;", new { STUDENT_ID = studentId }).FirstOrDefault();
                string rollNo = studentInfo != null ? Convert.ToString(studentInfo.ROLL_NO) : studentId.ToString();
                string studentName = studentInfo != null ? Convert.ToString(studentInfo.NAME) : "";

                // Fetch head info
                var headInfo = DapperHandler.FetchAll<dynamic>("SELECT HEAD_ID, HEAD_CODE, HEAD AS HEAD_NAME FROM sup_head WHERE HEAD_ID = @HEAD_ID LIMIT 1;", new { HEAD_ID = headId }).FirstOrDefault();
                string headCode = headInfo != null ? Convert.ToString(headInfo.HEAD_CODE) : "";
                string headName = headInfo != null ? Convert.ToString(headInfo.HEAD_NAME) : "";
                string headDisplay = !string.IsNullOrEmpty(headCode) ? headCode : headName;

                // Resolve FEE_MAIN_HEAD_ID if not passed
                string realFeeMainHeadId = feeMainHeadId;
                if (string.IsNullOrEmpty(realFeeMainHeadId) || realFeeMainHeadId == "0")
                {
                    var fmhInfo = DapperHandler.FetchAll<dynamic>("SELECT FEE_MAIN_HEAD_ID FROM fee_main_heads WHERE HEAD_ID = @HEAD_ID AND FREQUENCY_ID = @FREQ_ID AND ACADEMIC_YEAR = @AC_YEAR AND IS_DELETED != 1 LIMIT 1;", new { HEAD_ID = headId, FREQ_ID = frequencyId, AC_YEAR = academicYear }).FirstOrDefault();
                    if (fmhInfo != null)
                    {
                        realFeeMainHeadId = Convert.ToString(fmhInfo.FEE_MAIN_HEAD_ID);
                    }
                }

                // Check for duplicate account: SAME STUDENT, SAME FREQUENCY, SAME ACADEMIC YEAR, SAME HEAD CODE where IS_DELETED != 1
                string checkSql = @"SELECT F_STUDENT_AC_ID, CREDIT, IFNULL(DEBIT, 0) AS DEBIT, IS_DELETED 
                                    FROM fee_student_account 
                                    WHERE STUDENT_ID = @STUDENT_ID 
                                      AND ACADEMIC_YEAR = @AC_YEAR 
                                      AND FREQUENCY_ID = @FREQ_ID 
                                      AND HEAD = @HEAD_ID 
                                      AND IS_DELETED != 1 
                                    LIMIT 1;";
                var existing = DapperHandler.FetchAll<dynamic>(checkSql, new { STUDENT_ID = studentId, AC_YEAR = academicYear, FREQ_ID = frequencyId, HEAD_ID = headId }).ToList();
                if (existing != null && existing.Count > 0)
                {
                    return Json(new { 
                        Success = false, 
                        Message = "Hostel fee account already exists for " + studentName + " (" + rollNo + ") for Head Code '" + headDisplay + "' in Academic Year " + academicYear + "!" 
                    });
                }

                // Check if a previously deleted record exists to reactivate
                string checkDeletedSql = @"SELECT F_STUDENT_AC_ID 
                                           FROM fee_student_account 
                                           WHERE STUDENT_ID = @STUDENT_ID 
                                             AND ACADEMIC_YEAR = @AC_YEAR 
                                             AND FREQUENCY_ID = @FREQ_ID 
                                             AND HEAD = @HEAD_ID 
                                             AND IS_DELETED = 1 
                                           LIMIT 1;";
                var deletedList = DapperHandler.FetchAll<dynamic>(checkDeletedSql, new { STUDENT_ID = studentId, AC_YEAR = academicYear, FREQ_ID = frequencyId, HEAD_ID = headId }).ToList();
                if (deletedList != null && deletedList.Count > 0)
                {
                    long fStudentAcId = Convert.ToInt64(deletedList[0].F_STUDENT_AC_ID);
                    string updateSql = "UPDATE fee_student_account SET CREDIT = '" + amount + "', FEE_MAIN_HEAD_ID = '" + realFeeMainHeadId + "', IS_DELETED = 0 WHERE F_STUDENT_AC_ID = " + fStudentAcId;
                    CMSHandler.SaveOrUpdate(null, updateSql, academicYear);
                }
                else
                {
                    string insertSql = @"INSERT INTO fee_student_account (STUDENT_ID, ACADEMIC_YEAR, FREQUENCY_ID, HEAD, FEE_MAIN_HEAD_ID, CREDIT, DEBIT, IS_DELETED) 
                                         VALUES (" + studentId + ", '" + academicYear + "', '" + frequencyId + "', '" + headId + "', '" + realFeeMainHeadId + "', '" + amount + "', '0', '0');";
                    CMSHandler.SaveOrUpdate(null, insertSql, academicYear);
                }

                // Ensure student exists in hostel_student_info for this academic year
                string checkHostelSql = @"SELECT STU_HOSTEL_ID, IS_DELETED 
                                          FROM hostel_student_info 
                                          WHERE STUDENT_ID = @STUDENT_ID 
                                            AND ACADEMIC_YEAR = @AC_YEAR 
                                          LIMIT 1;";
                var hostelExist = DapperHandler.FetchAll<dynamic>(checkHostelSql, new { STUDENT_ID = studentId, AC_YEAR = academicYear }).ToList();
                if (hostelExist != null && hostelExist.Count > 0)
                {
                    long stuHostelId = Convert.ToInt64(hostelExist[0].STU_HOSTEL_ID);
                    int isDel = Convert.ToInt32(hostelExist[0].IS_DELETED ?? 0);
                    if (isDel == 1)
                    {
                        string reactivateHostelSql = "UPDATE hostel_student_info SET IS_DELETED = 0, IS_LEFT = 0 WHERE STU_HOSTEL_ID = " + stuHostelId;
                        CMSHandler.SaveOrUpdate(null, reactivateHostelSql, academicYear);
                    }
                }
                else
                {
                    var prevHostelInfo = DapperHandler.FetchAll<dynamic>(
                        "SELECT HOSTEL_ID, BLOCK_ID, ROOM_NO FROM hostel_student_info WHERE STUDENT_ID = @STUDENT_ID AND IS_DELETED != 1 ORDER BY STU_HOSTEL_ID DESC LIMIT 1;", 
                        new { STUDENT_ID = studentId }).FirstOrDefault();

                    string hostelIdVal = "1";
                    string blockIdVal = "NULL";
                    string roomNoVal = "NULL";

                    if (prevHostelInfo != null && prevHostelInfo.HOSTEL_ID != null)
                    {
                        hostelIdVal = Convert.ToString(prevHostelInfo.HOSTEL_ID);
                        if (prevHostelInfo.BLOCK_ID != null)
                        {
                            blockIdVal = "'" + Convert.ToString(prevHostelInfo.BLOCK_ID) + "'";
                        }
                        if (prevHostelInfo.ROOM_NO != null)
                        {
                            roomNoVal = "'" + Convert.ToString(prevHostelInfo.ROOM_NO).Replace("'", "''") + "'";
                        }
                    }
                    else
                    {
                        var genderInfo = DapperHandler.FetchAll<dynamic>(
                            "SELECT GENDER FROM stu_personal_info WHERE STUDENT_ID = @STUDENT_ID LIMIT 1;", 
                            new { STUDENT_ID = studentId }).FirstOrDefault();
                        if (genderInfo != null && Convert.ToString(genderInfo.GENDER) == "2")
                        {
                            hostelIdVal = "2";
                        }
                    }

                    string insertHostelSql = string.Format(
                        @"INSERT INTO hostel_student_info (STUDENT_ID, HOSTEL_ID, BLOCK_ID, ROOM_NO, ADMITTED_YEAR, ACADEMIC_YEAR, IS_LEFT, IS_DELETED) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {4}, 0, 0);",
                        studentId, hostelIdVal, blockIdVal, roomNoVal, academicYear);
                    CMSHandler.SaveOrUpdate(null, insertHostelSql, academicYear);
                }

                return Json(new { 
                    Success = true, 
                    Message = "Hostel fee account created successfully for " + studentName + " (" + rollNo + ") for Head Code '" + headDisplay + "'." 
                });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        public JsonResult SaveHostelFeeDueDetailssjctnc()
        {
            FeeTransactionViewModel ObjViewModel = new FeeTransactionViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = Request.Form["AcademicYear"];
            if (string.IsNullOrEmpty(sAcademicYear))
            {
                sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            }
            string sParamFrequencyId = Request.Form["FrequencyId"];
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var filename = Request.Files[0].FileName;
            HttpPostedFileBase file = Request.Files[filename];
            var sFilename = Request.Files[0].FileName.Split('.');
            string columnName = string.Empty;
            string paidDateColumn = string.Empty;
            int SuccessCount = 0;
            int FailCount = 0;
            int IncorrectData = 0;
            string sMessage = string.Empty;
            string updateSQL = string.Empty;
            string insertSQL = string.Empty;
            var CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string PaymentStatus = "2"; // Hardcoded to Pay Amount (CREDIT) as Status dropdown is removed
                    string extension = System.IO.Path.GetExtension(Request.Files[0].FileName).ToLower();

                    string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
                    string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/Uploads/Fee/" + sAcademicYear), Request.Files[0].FileName);

                    if (!Directory.Exists(path1))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/Uploads/Fee/" + sAcademicYear));
                    }
                    // Save File
                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);
                    }
                    Request.Files[0].SaveAs(path1);

                    byte[] fileBytes;
                    using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                    {
                        fileBytes = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                    }

                    if (validFileTypes.Contains(extension))
                    {
                        var objFeeDueList = new FEE_STUDENT_DUE();
                        var liStudentList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentDetailsFroFeeDues), sAcademicYear).DataSource.Data;
                        if (liStudentList != null && liStudentList.Count > 0)
                        {
                            using (var package = new ExcelPackage(new MemoryStream(fileBytes)))
                            {
                                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                                var worksheet = package.Workbook.Worksheets[0];
                                var studentFeeDueDetails = new List<FEE_STUDENT_DUE>();
                                if (worksheet.Dimension.End.Row <= 1 || worksheet.Dimension.End.Column < 2)
                                {
                                    ObjJsonData.Message = "The uploaded file is empty or improperly formatted.";
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    return Json(ObjJsonData);
                                }
                                else
                                {
                                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                                    {
                                        bool isValidRow = !string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text);
                                        if (isValidRow)
                                        {
                                            var lifeeDueList = new FEE_STUDENT_DUE
                                            {
                                                ROLL_NO = worksheet.Cells[row, 1].Text,
                                                HEAD = worksheet.Cells[row, 2].Text,
                                                FeeAmount = worksheet.Cells[row, 3].Text,
                                                ESTABLISHMENT = worksheet.Cells[row, 4].Text,
                                            };
                                            studentFeeDueDetails.Add(lifeeDueList);
                                        }
                                    }
                                }//

                                if (studentFeeDueDetails != null && studentFeeDueDetails.Count > 0)
                                {
                                    var supHead = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(null, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchsupHeadOnlyHeadCode), sAcademicYear).DataSource.Data;

                                    bool isPaid = true;
                                    HashSet<string> processedCombinations = new HashSet<string>();
                                    foreach (var dr in studentFeeDueDetails)
                                    {
                                        dr.HEAD = dr.HEAD.Replace(" ", "");
                                        dr.ROLL_NO = dr.ROLL_NO.Replace(" ", "");
                                        dr.FeeAmount = dr.FeeAmount.Replace(" ", "");
                                        dr.ESTABLISHMENT = dr.ESTABLISHMENT.Replace(" ", "");

                                        string combinationKey = dr.ROLL_NO.ToUpper() + "_" + dr.HEAD.ToUpper();
                                        if (processedCombinations.Contains(combinationKey))
                                        {
                                            continue; // Skip duplicate records in Excel file
                                        }
                                        processedCombinations.Add(combinationKey);

                                        var SupheadId = supHead.Where(x => x.HEAD_CODE == dr.HEAD).ToList();
                                        if (SupheadId == null || SupheadId.Count == 0)
                                        {
                                            FailCount++;
                                            string err = "Invalid Head Code: " + dr.HEAD + " for Roll No: " + dr.ROLL_NO;
                                            using (ErrorLog ObjLog = new ErrorLog())
                                            {
                                                ObjLog.FeeDueFileLog(err, CurrentDate);
                                            }
                                            continue;
                                        }

                                        var liStuFeeDueList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { HEAD = SupheadId.FirstOrDefault().HEAD_ID, ROLL_NO = dr.ROLL_NO, ACADEMIC_YEAR= sAcademicYear }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.HostelUpdateFeeStudentAccountByIDandTermNew), sAcademicYear).DataSource.Data;


                                        // SJCTNC Added FREQUENCY_ID logic
                                        string sFrequencyId = sParamFrequencyId;
                                        if (string.IsNullOrEmpty(sFrequencyId))
                                        {
                                            var frequencyList = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.HostelFee, ACADEMIC_YEAR = sAcademicYear }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyByFeeModeAndYear)).DataSource.Data;
                                            if (frequencyList == null || frequencyList.Count == 0)
                                            {
                                                frequencyList = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.HostelFee, ACADEMIC_YEAR = sAcademicYear }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyByFeeModeAndYear), sAcademicYear).DataSource.Data;
                                            }
                                            if (frequencyList != null && frequencyList.Count > 0)
                                            {
                                                sFrequencyId = frequencyList.FirstOrDefault().FREQUENCY_ID;
                                            }
                                        }

                                        var supHeadSingle = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(new SUP_HEAD() { HEAD_ID = SupheadId.FirstOrDefault().HEAD_ID, FREQUENCY_ID = sFrequencyId }, "SELECT FEE_MAIN_HEAD_ID FROM FEE_MAIN_HEADS WHERE FREQUENCY_ID = ?FREQUENCY_ID AND HEAD_ID = ?HEAD_ID LIMIT 1;").DataSource.Data;
                                        string sFeeMainHeadId = "";
                                        if (supHeadSingle != null && supHeadSingle.Count > 0)
                                        {
                                            sFeeMainHeadId = supHeadSingle.FirstOrDefault().FEE_MAIN_HEAD_ID;
                                        }

                                        // End SJCTNC logic

                                        switch (PaymentStatus)
                                        {
                                            case "1":
                                                columnName = "DEBIT";
                                                paidDateColumn = "PAYMENT_DATE";
                                                isPaid = true;
                                                break;
                                            case "2":
                                                columnName = "CREDIT";
                                                paidDateColumn = null;
                                                isPaid = false;
                                                break;
                                        }
                                        var SupESTheadId = supHead.Where(x => x.HEAD_CODE == "ESTFEE").ToList();
                                        string sFeeMainHeadIdEst = "";
                                        List<FEE_STUDENT_DUE> liStuFeeDueListForEst = null;
                                        if (SupESTheadId != null && SupESTheadId.Count > 0)
                                        {
                                            var supHeadEst = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(new SUP_HEAD() { FREQUENCY_ID= sFrequencyId, HEAD_ID = SupESTheadId.FirstOrDefault().HEAD_ID }, "SELECT FEE_MAIN_HEAD_ID FROM FEE_MAIN_HEADS WHERE FREQUENCY_ID = ?FREQUENCY_ID AND HEAD_ID = ?HEAD_ID LIMIT 1;").DataSource.Data;
                                            if (supHeadEst != null && supHeadEst.Count > 0)
                                            {
                                                sFeeMainHeadIdEst = supHeadEst.FirstOrDefault().FEE_MAIN_HEAD_ID;
                                            }
                                            liStuFeeDueListForEst = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, HEAD = SupESTheadId.FirstOrDefault().HEAD_ID, ACADEMIC_YEAR = sAcademicYear }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.HostelUpdateFeeStudentAccountByIDandTermNew), sAcademicYear).DataSource.Data;
                                        }
                                        if (liStuFeeDueListForEst != null && liStuFeeDueListForEst.Count > 0)
                                        {
                                            if (dr.ESTABLISHMENT != "0")
                                            {
                                                updateSQL = isPaid ? "UPDATE fee_student_account SET " + columnName + " = " + dr.ESTABLISHMENT +
                                      ",PAYMENT_DATE = '" + CurrentDate + "' WHERE F_STUDENT_AC_ID = " + liStuFeeDueListForEst.FirstOrDefault().F_STUDENT_AC_ID + ";" : "UPDATE fee_student_account SET " + columnName + " = " + dr.ESTABLISHMENT + " WHERE F_STUDENT_AC_ID = " + liStuFeeDueListForEst.FirstOrDefault().F_STUDENT_AC_ID + ";";
                                                var UpdateEstDueList = CMSHandler.SaveOrUpdate(objFeeDueList, updateSQL);
                                                if (UpdateEstDueList.Success)
                                                {

                                                    SuccessCount++;
                                                }
                                                else
                                                {
                                                    FailCount++;
                                                    sMessage = "Fail To Update Record Roll NO : " + dr.ROLL_NO + ":: Amount : " + dr.FeeAmount;

                                                    using (ErrorLog ObjLog = new ErrorLog())
                                                    {
                                                        ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (dr.ESTABLISHMENT != "0" && SupESTheadId != null && SupESTheadId.Count > 0)
                                            {
                                                var liStudentListbyRollNO = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentDetailsFroFeeDuesByRollNo), sAcademicYear).DataSource.Data;
                                                if (liStudentListbyRollNO != null)
                                                {
                                                    insertSQL = isPaid ? "INSERT INTO fee_student_account (STUDENT_ID, " + columnName + ", ACADEMIC_YEAR, " + paidDateColumn +
                                       ",HEAD, CREATED_BY, FREQUENCY_ID, FEE_MAIN_HEAD_ID) VALUES (" + liStudentListbyRollNO.FirstOrDefault().STUDENT_ID + ", " + dr.ESTABLISHMENT + ", " +
                                       sAcademicYear + ", '" + CurrentDate + "', '" + SupESTheadId.FirstOrDefault().HEAD_ID + "', '" + CurrentDate + "', '" + sFrequencyId + "', '" + sFeeMainHeadIdEst + "');" :

                                       "INSERT INTO fee_student_account (STUDENT_ID, " + columnName +
                                       ", ACADEMIC_YEAR,HEAD, CREATED_BY, FREQUENCY_ID, FEE_MAIN_HEAD_ID) VALUES (" +
                                       liStudentListbyRollNO.FirstOrDefault().STUDENT_ID + ", " + dr.ESTABLISHMENT + ", " + sAcademicYear + ",'" + SupESTheadId.FirstOrDefault().HEAD_ID + "', '" + CurrentDate + "', '" + sFrequencyId + "', '" + sFeeMainHeadIdEst + "');";

                                                    var saveEstDueList = CMSHandler.SaveOrUpdate(objFeeDueList, insertSQL);
                                                    if (saveEstDueList.Success)
                                                    {

                                                        SuccessCount++;
                                                    }
                                                    else
                                                    {
                                                        FailCount++;
                                                        sMessage = "Fail To Save Record Roll NO : " + dr.ROLL_NO + ":: Amount : " + dr.FeeAmount;

                                                        using (ErrorLog ObjLog = new ErrorLog())
                                                        {
                                                            ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (liStuFeeDueList != null && liStuFeeDueList.Count > 0)
                                        {
                                            objFeeDueList.STUDENT_ID = liStuFeeDueList.FirstOrDefault().STUDENT_ID;
                                            objFeeDueList.F_STUDENT_AC_ID = liStuFeeDueList.FirstOrDefault().F_STUDENT_AC_ID;
                                            objFeeDueList.HEAD = dr.HEAD;
                                            objFeeDueList.FeeAmount = dr.FeeAmount;
                                            objFeeDueList.ENTRY_ID = sStaffId;
                                            objFeeDueList.ENTRY_DATE = CurrentDate;
                                            objFeeDueList.ACADEMIC_YEAR = sAcademicYear;
                                            updateSQL = isPaid ? "UPDATE fee_student_account SET " + columnName + " = " + objFeeDueList.FeeAmount +
                                      ", PAYMENT_DATE = '" + CurrentDate + "' WHERE F_STUDENT_AC_ID = " + objFeeDueList.F_STUDENT_AC_ID + ";" : "UPDATE fee_student_account SET " + columnName + " = " + objFeeDueList.FeeAmount + " WHERE F_STUDENT_AC_ID = " + objFeeDueList.F_STUDENT_AC_ID + ";";

                                            var UpdateDueList = CMSHandler.SaveOrUpdate(objFeeDueList, updateSQL);

                                            string sSql = string.Empty;
                                            if (UpdateDueList.Success)
                                            {

                                                SuccessCount++;
                                            }
                                            else
                                            {
                                                FailCount++;
                                                sMessage = "Fail To Update Record Roll No :" + dr.ROLL_NO + ":: Amount : " + dr.FeeAmount;

                                                using (ErrorLog ObjLog = new ErrorLog())
                                                {
                                                    ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var liStudentListbyRollNO = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentDetailsFroFeeDuesByRollNo), sAcademicYear).DataSource.Data;
                                            if (liStudentListbyRollNO != null)
                                            {
                                                objFeeDueList.STUDENT_ID = liStudentListbyRollNO.FirstOrDefault().STUDENT_ID;
                                                objFeeDueList.F_STUDENT_AC_ID = liStudentListbyRollNO.FirstOrDefault().F_STUDENT_AC_ID;
                                                objFeeDueList.MONTH = dr.MONTH;
                                                objFeeDueList.FeeAmount = dr.FeeAmount;
                                                objFeeDueList.ENTRY_ID = sStaffId;
                                                objFeeDueList.HEAD = SupheadId.FirstOrDefault().HEAD_ID;
                                                objFeeDueList.ENTRY_DATE = CurrentDate;
                                                objFeeDueList.ACADEMIC_YEAR = sAcademicYear;
                                                if (dr.FeeAmount != "0")
                                                {
                                                    insertSQL = isPaid ? "INSERT INTO fee_student_account (STUDENT_ID, " + columnName + ", ACADEMIC_YEAR, " + paidDateColumn +
                                              ",HEAD, FREQUENCY_ID, FEE_MAIN_HEAD_ID) VALUES (" + objFeeDueList.STUDENT_ID + ", " + objFeeDueList.FeeAmount + ", " +
                                              sAcademicYear + ", '" + objFeeDueList.HEAD + "','" + sFrequencyId + "', '" + sFeeMainHeadId + "');" :

                                              "INSERT INTO fee_student_account (STUDENT_ID, " + columnName +
                                              ", ACADEMIC_YEAR,HEAD, FREQUENCY_ID, FEE_MAIN_HEAD_ID) VALUES (" +
                                              objFeeDueList.STUDENT_ID + ", " + objFeeDueList.FeeAmount + ", " + sAcademicYear + ",'" + objFeeDueList.HEAD + "', '" + sFrequencyId + "', '" + sFeeMainHeadId + "');";

                                                    var InsertDueList = CMSHandler.SaveOrUpdate(objFeeDueList, insertSQL);

                                                    string sSql = string.Empty;
                                                    if (InsertDueList.Success)
                                                    {
                                                        //var liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDueListByRollNoForUpdateExamDefaulters), sAcademicYear).DataSource.Data;
                                                        //if (liFeeDueReport != null && liFeeDueReport.Count > 0)
                                                        //{

                                                        //}
                                                        //else
                                                        //{
                                                        //    var UnSafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                                                        //    var UpdatExamDEfaultersList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersList));
                                                        //    var UpdateexamResultExamDefalutersDueList = CMSHandler.SaveOrUpdate(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO, IS_DELETED = Common.DeleteFlag.Deleted }, FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteExamDefaultersListfromSjcExamResultDB).Replace("?RESULT_DB", Common.DATABASE_NAMES.RESULT_DB));
                                                        //    var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                                                        //}
                                                        SuccessCount++;
                                                        ObjJsonData.ErrorCode = (InsertDueList.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                                        ObjJsonData.Message = (InsertDueList.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                                    }
                                                    else
                                                    {
                                                        FailCount++;
                                                        sMessage = "Fail To Save Record Roll No : " + dr.ROLL_NO + ":: Month : " + dr.HEAD; ;

                                                        using (ErrorLog ObjLog = new ErrorLog())
                                                        {
                                                            ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    FailCount++;
                                                    sMessage = "Amount is 0. Failed to save record. Roll No: " + dr.ROLL_NO + ", Month: " + dr.HEAD + ".";

                                                    using (ErrorLog ObjLog = new ErrorLog())
                                                    {
                                                        ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                sMessage = "This Roll Number Not  Exist in Student Details" + "Roll No : " + dr.ROLL_NO;

                                                using (ErrorLog ObjLog = new ErrorLog())
                                                {
                                                    ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                                                }
                                            }
                                        }
                                    }
                                    ObjJsonData.ErrorCode = (SuccessCount != 0) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    ObjJsonData.Message = SuccessCount + " Record(s) Saved Successfully !!! , " + FailCount + " Failed To Save Record(s) !!!";

                                }
                                else
                                {
                                    ObjJsonData.Message = "Oops! File Con't be Read";
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                }

                            }
                        }
                        else
                        {
                            ObjJsonData.Message = "No Student's Account Found For This Frequency";
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    sMessage = ex.Message;

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.FeeDueFileLog(sMessage, CurrentDate);
                    }
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Fee", "SaveFeePaymentUpdates", ex.Message);
                        ObjJsonData.Message = ex.Message;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;

                        return Json(ObjJsonData);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData);
        }


        public ActionResult BusFeeStudentAccountFileUploadsjctnc()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var AcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcademicYear != null && AcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYear = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusFeeStudentAccountbyAcYearsjctnc), sAcademicYear).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }

        public ActionResult BindAcYearWiseBusDueListListsjctnc(string sAcYear)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();

            if (!string.IsNullOrEmpty(sAcYear))
            {
                objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusFeeStudentAccountbyAcYearsjctnc), sAcYear).DataSource.Data;
                var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                if (liAcademicYear != null)
                {
                    objViewModel.AcademicYear = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
            }
            return View(objViewModel);
        }

        public JsonResult FetchBusDetailsByIdsjctnc(string sDueID)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            try
            {
                objResultData.oResult = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { STUDENT_AC_ID = sDueID }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusFeeByIdsjctnc), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                objResultData.Message = Common.ErrorMessage.InternalServerError;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteBusFeeAccountsjctnc(string fID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new FEE_STUDENT_DUE();
                    ObjModel.STUDENT_AC_ID = fID;
                    ObjModel.IS_DELETED = Common.DeleteFlag.Deleted;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, FeeSQL.GetFeeSQL(FeeSQLCommands.DeletedBusFeeAccountByIDSjctnc));
                    if (DeleteQuery.Success)
                    {
                        ObjJsonData.Message = Common.Messages.RecordDeletedSuccessfully;
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.FailedToDeletedTryAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveOrUpdateBusFeeStudentAccountsjctnc()
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FEE_STUDENT_DUE objModel = new FEE_STUDENT_DUE();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            try
            {
                var sJson = Request.Form["jsonData"];
                var acYear = Request.Form["academicYear"];
                var result = JsonConvert.DeserializeObject<FEE_STUDENT_DUE>(sJson);
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(acYear) && acYear != "--select--")
                    {
                        sAcademicYear = acYear;
                    }
                    var UserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var currentDate = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                    // Map Term to Frequency ID
                    string frequencyId = result.TERM == "1" ? "27" : "28";

                    var frequencyList = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = "8", ACADEMIC_YEAR = sAcademicYear }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyByFeeModeAndYear)).DataSource.Data;
                    if (frequencyList == null || frequencyList.Count == 0)
                        frequencyList = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = "8" }, "SELECT FREQUENCY_ID FROM FEE_FREQUENCY_FEE_MODE WHERE FEE_MODE = ?FEE_MODE ORDER BY FREQUENCY_ID DESC;").DataSource.Data;

                    // We will just use the mapped frequency ID directly to be safe, but fallback to 8 if necessary
                    // Wait, the user specifically said: "FREQUNCY ID FOR BUS 27 - FEE_MODE = 8"

                    string feeMainHeadId = "";
                    var feeMainHeads = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(null, "SELECT FEE_MAIN_HEAD_ID FROM FEE_MAIN_HEADS WHERE FREQUENCY_ID = '" + frequencyId + "' AND HEAD_ID = '137'").DataSource.Data;
                    if (feeMainHeads != null && feeMainHeads.Count > 0)
                        feeMainHeadId = feeMainHeads[0].FEE_MAIN_HEAD_ID.ToString();

                    result.ENTRY_ID = UserId;
                    result.ENTRY_DATE = currentDate;
                    result.ACADEMIC_YEAR = sAcademicYear;
                    result.HEAD = "137";
                    result.FREQUENCY_ID = frequencyId;

                    // If STUDENT_AC_ID is provided, it's an UPDATE.
                    if (!string.IsNullOrEmpty(result.STUDENT_AC_ID) && result.STUDENT_AC_ID != "0" && result.STUDENT_AC_ID != "undefined")
                    {
                        // Update fee_student_account
                        string updateSQL = "UPDATE fee_student_account SET CREDIT='" + result.CREDIT + "', DEBIT='" + result.DEBIT + "' WHERE F_STUDENT_AC_ID=" + result.STUDENT_AC_ID + " AND ACADEMIC_YEAR = '" + sAcademicYear + "';";
                        var UpdateDueList = CMSHandler.SaveOrUpdate(result, updateSQL);

                        if (UpdateDueList.Success)
                        {
                            // Update student_bus_route_details
                            string routeUpdateSQL = "UPDATE student_bus_route_details SET BUS_ROUTE='" + result.BUS_ROUTE + "', BUS_NO='" + result.BUS_NO + "', TERM='" + result.TERM + "' WHERE STUDENT_ID=" + result.STUDENT_ID + " AND ACADEMIC_YEAR = '" + sAcademicYear + "' AND TERM = '" + result.TERM + "';";
                            CMSHandler.SaveOrUpdate(result, routeUpdateSQL);

                            objResult.ErrorCode = Common.ErrorCode.Ok;
                            objResult.Message = Common.Messages.RecordsUpdatedSuccessfully;
                        }
                        else
                        {
                            objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                            objResult.Message = Common.Messages.FailedToUpdatedRecords;
                        }
                    }
                    else
                    {
                        // INSERT New
                        var liStfandstuFeeDueList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { TERM = result.TERM, ROLL_NO = result.ROLL_NO }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.BusFeeStudentandstaffAccountByIDandTerm), sAcademicYear).DataSource.Data;
                        if (liStfandstuFeeDueList != null && liStfandstuFeeDueList.Count > 0)
                        {
                            string studentId = liStfandstuFeeDueList.FirstOrDefault().STUDENT_ID;

                            string insertFSA = "INSERT INTO fee_student_account (STUDENT_ID, ACADEMIC_YEAR, FREQUENCY_ID, HEAD, CREDIT, DEBIT, TRANSACTION_DATE, FEE_MAIN_HEAD_ID) VALUES ('" + studentId + "', '" + sAcademicYear + "', '" + frequencyId + "', '137', '" + result.CREDIT + "', '" + result.DEBIT + "', '" + currentDate + "', '" + feeMainHeadId + "');";
                            var InsertDueList = CMSHandler.SaveOrUpdate(null, insertFSA);

                            if (InsertDueList.Success)
                            {
                                string insertRoute = "INSERT INTO student_bus_route_details (STUDENT_ID, ACADEMIC_YEAR, BUS_ROUTE, BUS_NO, TERM, ENTRY_ID, ENTRY_DATE) VALUES ('" + studentId + "', '" + sAcademicYear + "', '" + result.BUS_ROUTE + "', '" + result.BUS_NO + "', '" + result.TERM + "', '" + UserId + "', '" + currentDate + "');";
                                CMSHandler.SaveOrUpdate(null, insertRoute);

                                objResult.ErrorCode = Common.ErrorCode.Ok;
                                objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                            }
                            else
                            {
                                objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                objResult.Message = Common.Messages.FailedToSaveRecords;
                            }
                        }
                        else
                        {
                            objResult.ErrorCode = Common.ErrorCode.NotFound;
                            objResult.Message = "Roll No Not Exist";
                        }
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                    objResult.Message = Common.ErrorMessage.BadRequest;
                }
            }
            catch (Exception ex)
            {
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = ex.Message;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveBusFeeDueDetailssjctnc()
        {
            FeeTransactionViewModel ObjViewModel = new FeeTransactionViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            int SuccessCount = 0;
            var CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string PaymentStatus = Request.Form["Status"];
                    string extension = System.IO.Path.GetExtension(Request.Files[0].FileName).ToLower();
                    string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
                    string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/Uploads/Fee/" + sAcademicYear), Request.Files[0].FileName);

                    if (!Directory.Exists(Server.MapPath("~/Content/Uploads/Fee/" + sAcademicYear)))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/Uploads/Fee/" + sAcademicYear));
                    }
                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);
                    }
                    Request.Files[0].SaveAs(path1);

                    byte[] fileBytes;
                    using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                    {
                        fileBytes = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                    }

                    if (validFileTypes.Contains(extension))
                    {
                        using (var package = new ExcelPackage(new MemoryStream(fileBytes)))
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            var worksheet = package.Workbook.Worksheets[0];
                            var studentFeeDueDetails = new List<FEE_STUDENT_DUE>();
                            if (worksheet.Dimension.End.Row <= 1 || worksheet.Dimension.End.Column < 2)
                            {
                                ObjJsonData.Message = "The uploaded file is empty or improperly formatted.";
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                return Json(ObjJsonData);
                            }
                            else
                            {
                                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                                {
                                    bool isValidRow = !string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text);
                                    if (isValidRow)
                                    {
                                        var lifeeDueList = new FEE_STUDENT_DUE
                                        {
                                            ROLL_NO = worksheet.Cells[row, 1].Text,
                                            TERM = worksheet.Cells[row, 2].Text,
                                            BUS_ROUTE = worksheet.Cells[row, 4].Text,
                                            FeeAmount = worksheet.Cells[row, 5].Text,
                                        };
                                        studentFeeDueDetails.Add(lifeeDueList);
                                    }
                                }
                            }

                            if (studentFeeDueDetails != null && studentFeeDueDetails.Count > 0)
                            {
                                var busHead = "137";
                                foreach (var dr in studentFeeDueDetails)
                                {
                                    dr.ROLL_NO = dr.ROLL_NO.Replace(" ", "");
                                    dr.FeeAmount = dr.FeeAmount.Replace(" ", "");
                                    dr.BUS_ROUTE = dr.BUS_ROUTE.Replace(" ", "");

                                    dr.TERM = dr.TERM.Replace(" ", "").ToUpper();
                                    string termNumber = dr.TERM.Contains("2") ? "2" : "1";
                                    dr.TERM = termNumber;

                                    string frequencyId = termNumber == "1" ? "27" : "28";
                                    string feeMainHeadId = "";
                                    var feeMainHeads = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(null, "SELECT FEE_MAIN_HEAD_ID FROM FEE_MAIN_HEADS WHERE FREQUENCY_ID = '" + frequencyId + "' AND HEAD_ID = '137'").DataSource.Data;
                                    if (feeMainHeads != null && feeMainHeads.Count > 0)
                                        feeMainHeadId = feeMainHeads[0].FEE_MAIN_HEAD_ID.ToString();

                                    var liStfandstuFeeDueList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(new FEE_STUDENT_DUE() { ROLL_NO = dr.ROLL_NO }, "SELECT STUDENT_ID FROM stu_personal_info WHERE ROLL_NO = ?ROLL_NO").DataSource.Data;
                                    if (liStfandstuFeeDueList != null && liStfandstuFeeDueList.Count > 0)
                                    {
                                        string studentId = liStfandstuFeeDueList.FirstOrDefault().STUDENT_ID;

                                        // Check if fee student account already exists
                                        var liStuFeeDueList = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, "SELECT F_STUDENT_AC_ID AS STUDENT_AC_ID FROM fee_student_account WHERE STUDENT_ID = '" + studentId + "' AND ACADEMIC_YEAR = '" + sAcademicYear + "' AND FREQUENCY_ID = '" + frequencyId + "' AND HEAD = '" + busHead + "'").DataSource.Data;

                                        string credit = PaymentStatus == "2" ? dr.FeeAmount : "0";
                                        string debit = PaymentStatus == "1" ? dr.FeeAmount : "0";

                                        if (liStuFeeDueList != null && liStuFeeDueList.Count > 0)
                                        {
                                            // Update
                                            string studentAcId = liStuFeeDueList.FirstOrDefault().STUDENT_AC_ID;
                                            string updateSQL = PaymentStatus == "1" ? "UPDATE fee_student_account SET DEBIT = '" + debit + "', TRANSACTION_DATE = '" + CurrentDate + "' WHERE F_STUDENT_AC_ID = " + studentAcId + ";" : "UPDATE fee_student_account SET CREDIT = '" + credit + "' WHERE F_STUDENT_AC_ID = " + studentAcId + ";";
                                            CMSHandler.SaveOrUpdate(null, updateSQL);

                                            string routeUpdateSQL = "UPDATE student_bus_route_details SET BUS_ROUTE='" + dr.BUS_ROUTE + "', TERM='" + dr.TERM + "' WHERE STUDENT_ID=" + studentId + " AND ACADEMIC_YEAR = '" + sAcademicYear + "' AND TERM = '" + dr.TERM + "';";
                                            CMSHandler.SaveOrUpdate(null, routeUpdateSQL);
                                        }
                                        else
                                        {
                                            // Insert
                                            string insertFSA = "INSERT INTO fee_student_account (STUDENT_ID, ACADEMIC_YEAR, FREQUENCY_ID, HEAD, CREDIT, DEBIT, TRANSACTION_DATE, FEE_MAIN_HEAD_ID) VALUES ('" + studentId + "', '" + sAcademicYear + "', '" + frequencyId + "', '" + busHead + "', '" + credit + "', '" + debit + "', '" + CurrentDate + "', '" + feeMainHeadId + "');";
                                            CMSHandler.SaveOrUpdate(null, insertFSA);

                                            string insertRoute = "INSERT INTO student_bus_route_details (STUDENT_ID, ACADEMIC_YEAR, BUS_ROUTE, TERM, ENTRY_ID, ENTRY_DATE) VALUES ('" + studentId + "', '" + sAcademicYear + "', '" + dr.BUS_ROUTE + "', '" + dr.TERM + "', '" + sStaffId + "', '" + CurrentDate + "');";
                                            CMSHandler.SaveOrUpdate(null, insertRoute);
                                        }
                                        SuccessCount++;
                                    }
                                }
                                ObjJsonData.Message = SuccessCount + " Records Uploaded Successfully.";
                                ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            }
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = "Please Upload Valid Excel File";
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    ObjJsonData.Message = ex.Message;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        #region Bus Fee Creation Module (Transport Module)
        public ActionResult BusFeeStudentAccountCreation()
        {
            FeeStructureViewModel objViewModel = new FeeStructureViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.AcademicyearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BusFeeStudentAccountCreation", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BusFeeStudentAccountCreation", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }

        [HttpGet]
        public JsonResult GetBusList()
        {
            try
            {
                string sQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusList);
                var list = (List<SUP_BUS>)CMSHandler.FetchData<SUP_BUS>(null, sQuery).DataSource.Data;
                return Json(new { Success = true, Data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveOrUpdateBus(SUP_BUS model)
        {
            try
            {
                ResultArgs result;
                if (Convert.ToInt32(model.BUS_ID ?? "0") > 0)
                {
                    string sQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateBus);
                    result = CMSHandler.SaveOrUpdate(model, sQuery);
                }
                else
                {
                    string sQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.SaveBus);
                    result = CMSHandler.SaveOrUpdate(model, sQuery);
                }
                return Json(new { Success = result.Success, Message = result.Success ? "Bus details saved successfully." : result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteBus(int id)
        {
            try
            {
                string sQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteBus);
                var result = CMSHandler.SaveOrUpdate(new SUP_BUS { BUS_ID = id.ToString() }, sQuery);
                return Json(new { Success = result.Success, Message = result.Success ? "Bus deleted successfully." : result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetBoardingStages(int? busId, string academicYear)
        {
            try
            {
                string sQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBoardingStages);
                if (busId.HasValue && busId.Value > 0)
                {
                    sQuery += " AND BS.BUS_ID = " + busId.Value;
                }
                if (!string.IsNullOrEmpty(academicYear))
                {
                    sQuery += " AND BS.ACADEMIC_YEAR = '" + academicYear + "'";
                }
                var list = (List<TRANSPORT_BUS_STAGE>)CMSHandler.FetchData<TRANSPORT_BUS_STAGE>(null, sQuery).DataSource.Data;
                return Json(new { Success = true, Data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveOrUpdateBoardingStage(TRANSPORT_BUS_STAGE model)
        {
            try
            {
                ResultArgs result;
                if (Convert.ToInt32(model.BUS_STAGE_ID ?? "0") > 0)
                {
                    string sQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateBoardingStage);
                    result = CMSHandler.SaveOrUpdate(model, sQuery);
                }
                else
                {
                    string sQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.SaveBoardingStage);
                    result = CMSHandler.SaveOrUpdate(model, sQuery);
                }
                return Json(new { Success = result.Success, Message = result.Success ? "Boarding stage saved successfully." : result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteBoardingStage(int id)
        {
            try
            {
                string sQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.DeleteBoardingStage);
                var result = CMSHandler.SaveOrUpdate(new TRANSPORT_BUS_STAGE { BUS_STAGE_ID = id.ToString() }, sQuery);
                return Json(new { Success = result.Success, Message = result.Success ? "Boarding stage deleted successfully." : result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetBusFrequencies(string academicYear)
        {
            try
            {
                string sQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusFrequencies);
                var model = new FEE_FREQUENCY_FEE_MODE { ACADEMIC_YEAR = academicYear };
                var list = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(model, sQuery).DataSource.Data;
                return Json(new { Success = true, Data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetBusMainHeads(string frequencyId, string academicYear)
        {
            try
            {
                string sQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusMainHeads);
                var model = new FeeMainHead { ACADEMIC_YEAR = academicYear, FREQUENCY_ID = frequencyId };
                var list = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(model, sQuery).DataSource.Data;
                return Json(new { Success = true, Data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetBusAccountsList(string academicYear, string frequencyId, string mainHeadId)
        {
            try
            {
                string sAcademicYear = academicYear;
                if (string.IsNullOrEmpty(sAcademicYear) && Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                }

                if (string.IsNullOrEmpty(sAcademicYear) || string.IsNullOrEmpty(frequencyId) || frequencyId == "0")
                {
                    return Json(new { Success = false, Message = "Please select Academic Year and Frequency." }, JsonRequestBehavior.AllowGet);
                }

                string mainHeadFilterStudent = "";
                string mainHeadFilterStaff = "";
                if (!string.IsNullOrEmpty(mainHeadId) && mainHeadId != "0")
                {
                    mainHeadFilterStudent = " AND (FSA.FEE_MAIN_HEAD_ID = @MAIN_HEAD_ID OR FSA.FEE_MAIN_HEAD_ID IN (SELECT FEE_MAIN_HEAD_ID FROM FEE_MAIN_HEADS WHERE MAIN_HEAD_ID = @MAIN_HEAD_ID AND FREQUENCY_ID = @FREQUENCY_ID AND ACADEMIC_YEAR = @AC_YEAR))";
                    mainHeadFilterStaff = " AND (FST.FEE_MAIN_HEAD_ID = @MAIN_HEAD_ID OR FST.FEE_MAIN_HEAD_ID IN (SELECT FEE_MAIN_HEAD_ID FROM FEE_MAIN_HEADS WHERE MAIN_HEAD_ID = @MAIN_HEAD_ID AND FREQUENCY_ID = @FREQUENCY_ID AND ACADEMIC_YEAR = @AC_YEAR))";
                }

                string query = @"
                  SELECT 
    'STUDENT' AS USER_TYPE,
    FSA.F_STUDENT_AC_ID AS ACCOUNT_ID,
    FSA.STUDENT_ID AS USER_ID,
    SPI.ROLL_NO AS CODE,
    UPPER(CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                    ' ',
                    IFNULL(SPI.LAST_NAME, ''))) AS NAME,
    IFNULL(CLS.CLASS_NAME, 'N/A') AS DETAILS,
    CAST(SUM(IFNULL(FSA.CREDIT, 0)) AS DECIMAL (10 , 2 )) AS ALLOCATED_AMOUNT,
    CAST(SUM(IFNULL(FSA.DEBIT, 0)) AS DECIMAL (10 , 2 )) AS PAID_AMOUNT,
    CAST((SUM(IFNULL(FSA.CREDIT, 0)) - SUM(IFNULL(FSA.DEBIT, 0)))
        AS DECIMAL (10 , 2 )) AS BALANCE_AMOUNT,
    FSA.ACADEMIC_YEAR,
    FSA.FREQUENCY_ID,
    CASE
        WHEN (IFNULL(FSA.DEBIT, 0) = 0) THEN 1
        ELSE 0
    END AS CAN_EDIT_DELETE
FROM
    fee_student_account AS FSA
        INNER JOIN
    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = FSA.STUDENT_ID
        LEFT JOIN
    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
        AND SC.ACADEMIC_YEAR = FSA.ACADEMIC_YEAR
        LEFT JOIN
    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
WHERE
    FSA.HEAD = '137' AND FSA.IS_DELETED != 1
        AND FSA.ACADEMIC_YEAR = @AC_YEAR
        AND FSA.FREQUENCY_ID = @FREQUENCY_ID " + mainHeadFilterStudent + @" GROUP BY FSA.STUDENT_ID


                    UNION ALL

                    SELECT 
                        'STAFF' AS USER_TYPE,
                        FST.F_STAFF_AC_ID AS ACCOUNT_ID,
                        FST.STAFF_ID AS USER_ID,
                        SP.STAFF_CODE AS CODE,
                        UPPER(CONCAT(IFNULL(SP.FIRST_NAME, ''), ' ', IFNULL(SP.LAST_NAME, ''))) AS NAME,
                        'Staff' AS DETAILS,
                        CAST(IFNULL(FST.CREDIT, 0) AS DECIMAL(10,2)) AS ALLOCATED_AMOUNT,
                        CAST(IFNULL(FST.DEBIT, 0) AS DECIMAL(10,2)) AS PAID_AMOUNT,
                        CAST((IFNULL(FST.CREDIT, 0) - IFNULL(FST.DEBIT, 0)) AS DECIMAL(10,2)) AS BALANCE_AMOUNT,
                        FST.ACADEMIC_YEAR,
                        FST.FREQUENCY_ID,
                        CASE WHEN (IFNULL(FST.DEBIT, 0) = 0) THEN 1 ELSE 0 END AS CAN_EDIT_DELETE
                    FROM fee_staff_account AS FST
                    INNER JOIN STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = FST.STAFF_ID AND SP.IS_DELETED != 1
                    WHERE FST.HEAD = '137' 
                      AND FST.IS_DELETED != 1
                      AND FST.ACADEMIC_YEAR = @AC_YEAR
                      AND FST.FREQUENCY_ID = @FREQUENCY_ID
                      " + mainHeadFilterStaff + @"
                   GROUP BY SP.STAFF_ID
                    ORDER BY USER_TYPE DESC, CODE ASC;";

                var list = DapperHandler.FetchAll<BUS_FEE_ACCOUNT_ITEM>(query, new { AC_YEAR = sAcademicYear, FREQUENCY_ID = frequencyId, MAIN_HEAD_ID = mainHeadId }).ToList();
                var jsonResult = Json(new { Success = true, Data = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult FindStudentOrStaffInfo(string userType, string code, string academicYear)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return Json(new { Success = false, Message = "Please enter a Roll No or Staff Code." }, JsonRequestBehavior.AllowGet);
                }

                string sCode = code.Trim();
                if (userType == "STUDENT")
                {
                    string sql = @"
                        SELECT 
                            SPI.STUDENT_ID AS Id, 
                            SPI.ROLL_NO AS Code, 
                            UPPER(CONCAT(IFNULL(SPI.FIRST_NAME, ''), ' ', IFNULL(SPI.LAST_NAME, ''))) AS Name,
                            IFNULL(CLS.CLASS_NAME, 'N/A') AS Details
                        FROM stu_personal_info SPI
                        LEFT JOIN stu_class SC ON SC.STUDENT_ID = SPI.STUDENT_ID AND SC.ACADEMIC_YEAR = @AC_YEAR
                        LEFT JOIN cp_classes CLS ON CLS.CLASS_ID = SC.CLASS_ID
                        WHERE SPI.IS_DELETED != 1 
                          AND (SPI.ROLL_NO = @CODE OR SPI.ADMISSION_NO = @CODE)
                        LIMIT 1;";
                    var data = DapperHandler.FetchAll<dynamic>(sql, new { CODE = sCode, AC_YEAR = academicYear }).ToList();
                    if (data != null && data.Count > 0)
                    {
                        return Json(new { Success = true, Id = data[0].Id, Code = data[0].Code, Name = data[0].Name, Details = data[0].Details }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { Success = false, Message = "No active student found with Roll No / Admission No '" + sCode + "'." }, JsonRequestBehavior.AllowGet);
                }
                else if (userType == "STAFF")
                {
                    string sql = @"
                        SELECT 
                            STAFF_ID AS Id, 
                            STAFF_CODE AS Code, 
                            UPPER(CONCAT(IFNULL(FIRST_NAME, ''), ' ', IFNULL(LAST_NAME, ''))) AS Name,
                            'Staff' AS Details
                        FROM stf_personal_info
                        WHERE IS_DELETED != 1 
                          AND STAFF_CODE = @CODE
                        LIMIT 1;";
                    var data = DapperHandler.FetchAll<dynamic>(sql, new { CODE = sCode }).ToList();
                    if (data != null && data.Count > 0)
                    {
                        return Json(new { Success = true, Id = data[0].Id, Code = data[0].Code, Name = data[0].Name, Details = data[0].Details }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { Success = false, Message = "No active staff member found with Staff Code '" + sCode + "'." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Success = false, Message = "Invalid user type." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveSingleBusFeeAccount(string userType, long userId, string academicYear, string frequencyId, string mainHeadId, decimal amount)
        {
            try
            {
                if (userId <= 0)
                {
                    return Json(new { Success = false, Message = "Please verify and select a valid Student or Staff member." });
                }
                if (amount <= 0)
                {
                    return Json(new { Success = false, Message = "Allocated amount must be greater than zero." });
                }
                if (string.IsNullOrEmpty(academicYear) || string.IsNullOrEmpty(frequencyId) || frequencyId == "0")
                {
                    return Json(new { Success = false, Message = "Academic Year and Frequency are required." });
                }

                string realFeeMainHeadId = mainHeadId;
                if (!string.IsNullOrEmpty(mainHeadId) && mainHeadId != "0")
                {
                    string queryFeeMainHead = "SELECT FEE_MAIN_HEAD_ID FROM FEE_MAIN_HEADS WHERE MAIN_HEAD_ID = '" + mainHeadId + "' AND FREQUENCY_ID = '" + frequencyId + "' AND ACADEMIC_YEAR = '" + academicYear + "' LIMIT 1;";
                    var feeMainHeads = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(null, queryFeeMainHead).DataSource.Data;
                    if (feeMainHeads != null && feeMainHeads.Count > 0)
                    {
                        realFeeMainHeadId = feeMainHeads[0].FEE_MAIN_HEAD_ID.ToString();
                    }
                }

                if (userType == "STUDENT")
                {
                    string checkSql = "SELECT F_STUDENT_AC_ID, IFNULL(DEBIT, 0) AS DEBIT, IS_DELETED FROM fee_student_account WHERE STUDENT_ID = " + userId + " AND ACADEMIC_YEAR = '" + academicYear + "' AND FREQUENCY_ID = '" + frequencyId + "' AND HEAD = '137' LIMIT 1;";
                    var existing = DapperHandler.FetchAll<dynamic>(checkSql, null).ToList();
                    if (existing != null && existing.Count > 0)
                    {
                        long accId = Convert.ToInt64(existing[0].F_STUDENT_AC_ID);
                        decimal debit = Convert.ToDecimal(existing[0].DEBIT);
                        if (debit > 0)
                        {
                            return Json(new { Success = false, Message = "A bus fee account already exists for this student and has paid transactions." });
                        }
                        string updateSql = "UPDATE fee_student_account SET CREDIT = '" + amount + "', FEE_MAIN_HEAD_ID = '" + realFeeMainHeadId + "', IS_DELETED = 0 WHERE F_STUDENT_AC_ID = " + accId;
                        CMSHandler.SaveOrUpdate(null, updateSql);
                        return Json(new { Success = true, Message = "Bus fee account updated successfully for the student." });
                    }
                    else
                    {
                        string insertSql = "INSERT INTO fee_student_account (STUDENT_ID, ACADEMIC_YEAR, FREQUENCY_ID, HEAD, FEE_MAIN_HEAD_ID, CREDIT, DEBIT, IS_DELETED) VALUES (" +
                                           userId + ", '" + academicYear + "', '" + frequencyId + "', '137', '" + realFeeMainHeadId + "', '" + amount + "', '0', '0')";
                        CMSHandler.SaveOrUpdate(null, insertSql);
                        return Json(new { Success = true, Message = "Bus fee account created successfully for the student." });
                    }
                }
                else if (userType == "STAFF")
                {
                    string checkSql = "SELECT F_STAFF_AC_ID, IFNULL(DEBIT, 0) AS DEBIT, IS_DELETED FROM fee_staff_account WHERE STAFF_ID = " + userId + " AND ACADEMIC_YEAR = '" + academicYear + "' AND FREQUENCY_ID = '" + frequencyId + "' AND HEAD = '137' LIMIT 1;";
                    var existing = DapperHandler.FetchAll<dynamic>(checkSql, null).ToList();
                    if (existing != null && existing.Count > 0)
                    {
                        long accId = Convert.ToInt64(existing[0].F_STAFF_AC_ID);
                        decimal debit = Convert.ToDecimal(existing[0].DEBIT);
                        if (debit > 0)
                        {
                            return Json(new { Success = false, Message = "A bus fee account already exists for this staff member and has paid transactions." });
                        }
                        string updateSql = "UPDATE fee_staff_account SET CREDIT = '" + amount + "', FEE_MAIN_HEAD_ID = '" + realFeeMainHeadId + "', IS_DELETED = 0 WHERE F_STAFF_AC_ID = " + accId;
                        CMSHandler.SaveOrUpdate(null, updateSql);
                        return Json(new { Success = true, Message = "Bus fee account updated successfully for the staff member." });
                    }
                    else
                    {
                        string insertSql = "INSERT INTO fee_staff_account (STAFF_ID, ACADEMIC_YEAR, FREQUENCY_ID, HEAD, FEE_MAIN_HEAD_ID, CREDIT, DEBIT, IS_DELETED) VALUES (" +
                                           userId + ", '" + academicYear + "', '" + frequencyId + "', '137', '" + realFeeMainHeadId + "', '" + amount + "', '0', '0')";
                        CMSHandler.SaveOrUpdate(null, insertSql);
                        return Json(new { Success = true, Message = "Bus fee account created successfully for the staff member." });
                    }
                }
                return Json(new { Success = false, Message = "Invalid user type." });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateBusAccountAmount(string userType, int accountId, decimal newAmount)
        {
            try
            {
                if (newAmount < 0)
                {
                    return Json(new { Success = false, Message = "Amount cannot be negative." });
                }

                if (userType == "STUDENT")
                {
                    string checkQuery = "SELECT IFNULL(DEBIT, 0) AS DEBIT FROM fee_student_account WHERE F_STUDENT_AC_ID = @ID AND IS_DELETED != 1";
                    var debitList = DapperHandler.FetchAll<dynamic>(checkQuery, new { ID = accountId }).ToList();
                    if (debitList == null || debitList.Count == 0)
                    {
                        return Json(new { Success = false, Message = "Account record not found." });
                    }
                    decimal debit = Convert.ToDecimal(debitList[0].DEBIT);
                    if (debit > 0)
                    {
                        return Json(new { Success = false, Message = "Cannot update amount: Fee has already been paid for this student." });
                    }

                    string updateQuery = "UPDATE fee_student_account SET CREDIT = '" + newAmount + "' WHERE F_STUDENT_AC_ID = " + accountId;
                    CMSHandler.SaveOrUpdate(null, updateQuery);
                }
                else if (userType == "STAFF")
                {
                    string checkQuery = "SELECT IFNULL(DEBIT, 0) AS DEBIT FROM fee_staff_account WHERE F_STAFF_AC_ID = @ID AND IS_DELETED != 1";
                    var debitList = DapperHandler.FetchAll<dynamic>(checkQuery, new { ID = accountId }).ToList();
                    if (debitList == null || debitList.Count == 0)
                    {
                        return Json(new { Success = false, Message = "Account record not found." });
                    }
                    decimal debit = Convert.ToDecimal(debitList[0].DEBIT);
                    if (debit > 0)
                    {
                        return Json(new { Success = false, Message = "Cannot update amount: Fee has already been paid for this staff member." });
                    }

                    string updateQuery = "UPDATE fee_staff_account SET CREDIT = '" + newAmount + "' WHERE F_STAFF_AC_ID = " + accountId;
                    CMSHandler.SaveOrUpdate(null, updateQuery);
                }
                else
                {
                    return Json(new { Success = false, Message = "Invalid user type." });
                }

                return Json(new { Success = true, Message = "Allocated amount updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteBusAccount(string userType, int accountId)
        {
            try
            {
                if (userType == "STUDENT")
                {
                    string checkQuery = "SELECT IFNULL(DEBIT, 0) AS DEBIT FROM fee_student_account WHERE F_STUDENT_AC_ID = @ID AND IS_DELETED != 1";
                    var debitList = DapperHandler.FetchAll<dynamic>(checkQuery, new { ID = accountId }).ToList();
                    if (debitList == null || debitList.Count == 0)
                    {
                        return Json(new { Success = false, Message = "Account record not found." });
                    }
                    decimal debit = Convert.ToDecimal(debitList[0].DEBIT);
                    if (debit > 0)
                    {
                        return Json(new { Success = false, Message = "Cannot delete account: Fee has already been paid for this student." });
                    }

                    string updateQuery = "UPDATE fee_student_account SET IS_DELETED = 1 WHERE F_STUDENT_AC_ID = " + accountId;
                    CMSHandler.SaveOrUpdate(null, updateQuery);
                }
                else if (userType == "STAFF")
                {
                    string checkQuery = "SELECT IFNULL(DEBIT, 0) AS DEBIT FROM fee_staff_account WHERE F_STAFF_AC_ID = @ID AND IS_DELETED != 1";
                    var debitList = DapperHandler.FetchAll<dynamic>(checkQuery, new { ID = accountId }).ToList();
                    if (debitList == null || debitList.Count == 0)
                    {
                        return Json(new { Success = false, Message = "Account record not found." });
                    }
                    decimal debit = Convert.ToDecimal(debitList[0].DEBIT);
                    if (debit > 0)
                    {
                        return Json(new { Success = false, Message = "Cannot delete account: Fee has already been paid for this staff member." });
                    }

                    string updateQuery = "UPDATE fee_staff_account SET IS_DELETED = 1 WHERE F_STAFF_AC_ID = " + accountId;
                    CMSHandler.SaveOrUpdate(null, updateQuery);
                }
                else
                {
                    return Json(new { Success = false, Message = "Invalid user type." });
                }

                return Json(new { Success = true, Message = "Bus fee account deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetRegisteredStudents(int stageId, string academicYear)
        {
            try
            {
                string sQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchRegisteredStudentsForBusStage);
                var model = new TRANSPORT_BUS_REGISTRATION { STAGE_ID = stageId.ToString(), ACADEMIC_YEAR = academicYear };
                var list = (List<TRANSPORT_BUS_REGISTRATION>)CMSHandler.FetchData<TRANSPORT_BUS_REGISTRATION>(model, sQuery).DataSource.Data;
                return Json(new { Success = true, Data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveBusFeeStudentAccounts(string studentIds, string academicYear, string frequencyId, string mainHeadId, int stageId)
        {
            var objResult = new JsonResultData();
            try
            {
                if (string.IsNullOrEmpty(studentIds))
                {
                    objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                    objResult.Message = "Please select at least one student.";
                    return Json(objResult);
                }

                // Get amount from boarding stage
                string amountQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBoardingStages) + " AND BS.BUS_STAGE_ID = " + stageId;
                var stageList = (List<TRANSPORT_BUS_STAGE>)CMSHandler.FetchData<TRANSPORT_BUS_STAGE>(null, amountQuery).DataSource.Data;
                if (stageList == null || stageList.Count == 0)
                {
                    objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                    objResult.Message = "Invalid boarding stage selected.";
                    return Json(objResult);
                }
                int amount = Convert.ToInt32(stageList.FirstOrDefault().AMOUNT ?? "0");

                string realFeeMainHeadId = mainHeadId;
                string queryFeeMainHead = "SELECT FEE_MAIN_HEAD_ID FROM FEE_MAIN_HEADS WHERE MAIN_HEAD_ID = '" + mainHeadId + "' AND FREQUENCY_ID = '" + frequencyId + "' AND ACADEMIC_YEAR = '" + academicYear + "' LIMIT 1;";
                var feeMainHeads = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(null, queryFeeMainHead).DataSource.Data;
                if (feeMainHeads != null && feeMainHeads.Count > 0)
                {
                    realFeeMainHeadId = feeMainHeads[0].FEE_MAIN_HEAD_ID.ToString();
                }

                string[] ids = studentIds.Split(',');
                int successCount = 0;
                int skipCount = 0;

                foreach (string idStr in ids)
                {
                    if (string.IsNullOrEmpty(idStr)) continue;
                    int studentId = Convert.ToInt32(idStr);

                    // Check if already exists
                    string checkQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.CheckExistingFeeStudentAccountForBus);
                    var checkModel = new FEE_STUDENT_ACCOUNT
                    {
                        STUDENT_ID = studentId.ToString(),
                        ACADEMIC_YEAR = academicYear,
                        FREQUENCY_ID = frequencyId
                    };
                    var existing = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(checkModel, checkQuery).DataSource.Data;
                    if (existing != null && existing.Count > 0)
                    {
                        skipCount++;
                        continue;
                    }

                    // Insert
                    var model = new FEE_STUDENT_ACCOUNT
                    {
                        STUDENT_ID = studentId.ToString(),
                        ACADEMIC_YEAR = academicYear,
                        FREQUENCY_ID = frequencyId,
                        HEAD = "137",
                        CREDIT = amount.ToString(),
                        FEE_MAIN_HEAD_ID = realFeeMainHeadId
                    };
                    string insertQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.SaveBusFeeStudentAccount);
                    var res = CMSHandler.SaveOrUpdate(model, insertQuery);
                    if (res.Success)
                    {
                        successCount++;
                    }
                }

                objResult.ErrorCode = Common.ErrorCode.Ok;
                objResult.Message = string.Format("Accounts created successfully for {0} students. {1} students skipped as they already had accounts.", successCount, skipCount);
            }
            catch (Exception ex)
            {
                objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                objResult.Message = ex.Message;
            }
            return Json(objResult);
        }

        [HttpPost]
        public JsonResult UploadBusFeeExcel()
        {
            var objResult = new JsonResultData();
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            
            try
            {
                if (Request.Files.Count == 0)
                {
                    return Json(new { Success = false, Message = "Please upload an Excel file." });
                }

                // Retrieve form values from UI dropdown selections
                string sAcademicYear = Request.Form["AcademicYear"];
                string sFrequencyId = Request.Form["FrequencyId"];
                string sMainHeadId = Request.Form["MainHeadId"];
                string sBusId = Request.Form["BusId"];
                string sStageId = Request.Form["StageId"];

                if (string.IsNullOrEmpty(sAcademicYear))
                {
                    sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                }

                string realFeeMainHeadId = sMainHeadId;
                if (!string.IsNullOrEmpty(sMainHeadId) && !string.IsNullOrEmpty(sFrequencyId) && !string.IsNullOrEmpty(sAcademicYear))
                {
                    string queryFeeMainHead = "SELECT FEE_MAIN_HEAD_ID FROM FEE_MAIN_HEADS WHERE MAIN_HEAD_ID = '" + sMainHeadId + "' AND FREQUENCY_ID = '" + sFrequencyId + "' AND ACADEMIC_YEAR = '" + sAcademicYear + "' LIMIT 1;";
                    var feeMainHeads = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(null, queryFeeMainHead).DataSource.Data;
                    if (feeMainHeads != null && feeMainHeads.Count > 0)
                    {
                        realFeeMainHeadId = feeMainHeads[0].FEE_MAIN_HEAD_ID.ToString();
                    }
                }

                var file = Request.Files[0];
                string extension = System.IO.Path.GetExtension(file.FileName).ToLower();
                string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
                
                if (!validFileTypes.Contains(extension))
                {
                    return Json(new { Success = false, Message = "Please upload a valid Excel file." });
                }

                byte[] fileBytes;
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    fileBytes = binaryReader.ReadBytes(file.ContentLength);
                }

                int successCount = 0;
                var invalidRecords = new List<dynamic>();

                using (var package = new ExcelPackage(new MemoryStream(fileBytes)))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.End.Row;

                    if (rowCount <= 1)
                    {
                        return Json(new { Success = false, Message = "The uploaded file is empty or has no data rows." });
                    }

                    // Preload lookup data to prevent querying inside loop
                    string busSql = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusList);
                    var busResult = CMSHandler.FetchData<SUP_BUS>(null, busSql);
                    var busListRaw = (busResult != null && busResult.DataSource != null && busResult.DataSource.Data != null)
                                     ? (List<SUP_BUS>)busResult.DataSource.Data
                                     : new List<SUP_BUS>();
                    var busList = busListRaw.Where(b => !string.IsNullOrEmpty(b.BUS_NAME))
                                            .GroupBy(b => b.BUS_NAME.Trim().ToLower())
                                            .ToDictionary(g => g.Key, g => g.First().BUS_ID);

                    string stageSql = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBoardingStages);
                    var stageResult = CMSHandler.FetchData<TRANSPORT_BUS_STAGE>(null, stageSql);
                    var stageList = (stageResult != null && stageResult.DataSource != null && stageResult.DataSource.Data != null)
                                    ? (List<TRANSPORT_BUS_STAGE>)stageResult.DataSource.Data
                                    : new List<TRANSPORT_BUS_STAGE>();

                    string regSql = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAllActiveBusRegistrations);
                    var regResult = CMSHandler.FetchData<TRANSPORT_BUS_REGISTRATION>(null, regSql);
                    var regList = (regResult != null && regResult.DataSource != null && regResult.DataSource.Data != null)
                                  ? (List<TRANSPORT_BUS_REGISTRATION>)regResult.DataSource.Data
                                  : new List<TRANSPORT_BUS_REGISTRATION>();

                    // Load Students
                    string stuSql = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAllStudentsPersonalInfo);
                    var studentResult = CMSHandler.FetchData<TRANSPORT_BUS_REGISTRATION>(null, stuSql);
                    var studentList = (studentResult != null && studentResult.DataSource != null && studentResult.DataSource.Data != null)
                                      ? (List<TRANSPORT_BUS_REGISTRATION>)studentResult.DataSource.Data
                                      : new List<TRANSPORT_BUS_REGISTRATION>();
                    var studentByRoll = studentList.Where(s => !string.IsNullOrEmpty(s.ROLL_NO))
                                                   .GroupBy(s => s.ROLL_NO.Trim().ToLower())
                                                   .ToDictionary(g => g.Key, g => g.First());
                    var studentByAdm = studentList.Where(s => !string.IsNullOrEmpty(s.ADMISSION_NO))
                                                  .GroupBy(s => s.ADMISSION_NO.Trim().ToLower())
                                                  .ToDictionary(g => g.Key, g => g.First());

                    // Load Staff (stf_personal_info)
                    string staffSql = "SELECT STAFF_ID, STAFF_CODE, FIRST_NAME AS STAFF_NAME FROM stf_personal_info WHERE IS_DELETED != 1;";
                    var staffResult = CMSHandler.FetchData<FEE_STAFF_ACCOUNT>(null, staffSql);
                    var staffList = (staffResult != null && staffResult.DataSource != null && staffResult.DataSource.Data != null)
                                    ? (List<FEE_STAFF_ACCOUNT>)staffResult.DataSource.Data
                                    : new List<FEE_STAFF_ACCOUNT>();
                    var staffByCode = staffList.Where(s => !string.IsNullOrEmpty(s.STAFF_CODE))
                                               .GroupBy(s => s.STAFF_CODE.Trim().ToLower())
                                               .ToDictionary(g => g.Key, g => g.First());

                    string freqSql = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAllBusFrequencies);
                    var freqResult = CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(null, freqSql);
                    var freqList = (freqResult != null && freqResult.DataSource != null && freqResult.DataSource.Data != null)
                                   ? (List<FEE_FREQUENCY_FEE_MODE>)freqResult.DataSource.Data
                                   : new List<FEE_FREQUENCY_FEE_MODE>();

                    string mainHeadSql = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAllBusMainHeads);
                    var mainHeadResult = CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(null, mainHeadSql);
                    var mainHeadList = (mainHeadResult != null && mainHeadResult.DataSource != null && mainHeadResult.DataSource.Data != null)
                                       ? (List<FEE_STUDENT_ACCOUNT>)mainHeadResult.DataSource.Data
                                       : new List<FEE_STUDENT_ACCOUNT>();

                    // Existing Student Accounts
                    string existingAccSql = "SELECT F_STUDENT_AC_ID, STUDENT_ID, ACADEMIC_YEAR, FREQUENCY_ID, HEAD FROM fee_student_account WHERE HEAD = '137' AND IS_DELETED != 1";
                    if (!string.IsNullOrEmpty(sAcademicYear))
                    {
                        existingAccSql += " AND ACADEMIC_YEAR = '" + sAcademicYear + "'";
                    }
                    if (!string.IsNullOrEmpty(sFrequencyId) && sFrequencyId != "0")
                    {
                        existingAccSql += " AND FREQUENCY_ID = '" + sFrequencyId + "'";
                    }
                    var existingAccResult = CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(null, existingAccSql);
                    var existingAccs = (existingAccResult != null && existingAccResult.DataSource != null && existingAccResult.DataSource.Data != null)
                                       ? (List<FEE_STUDENT_ACCOUNT>)existingAccResult.DataSource.Data
                                       : new List<FEE_STUDENT_ACCOUNT>();
                    var existingAccDict = existingAccs
                                    .GroupBy(e => string.Format("{0}_{1}_{2}_{3}", e.STUDENT_ID, e.ACADEMIC_YEAR, e.FREQUENCY_ID, e.HEAD))
                                    .ToDictionary(g => g.Key, g => g.First().F_STUDENT_AC_ID);

                    // Existing Staff Accounts
                    string existingStaffAccSql = "SELECT F_STAFF_AC_ID, STAFF_ID, ACADEMIC_YEAR, FREQUENCY_ID, HEAD FROM fee_staff_account WHERE HEAD = '137' AND IS_DELETED != 1";
                    if (!string.IsNullOrEmpty(sAcademicYear))
                    {
                        existingStaffAccSql += " AND ACADEMIC_YEAR = '" + sAcademicYear + "'";
                    }
                    if (!string.IsNullOrEmpty(sFrequencyId) && sFrequencyId != "0")
                    {
                        existingStaffAccSql += " AND FREQUENCY_ID = '" + sFrequencyId + "'";
                    }
                    var existingStaffAccResult = CMSHandler.FetchData<FEE_STAFF_ACCOUNT>(null, existingStaffAccSql);
                    var existingStaffAccs = (existingStaffAccResult != null && existingStaffAccResult.DataSource != null && existingStaffAccResult.DataSource.Data != null)
                                            ? (List<FEE_STAFF_ACCOUNT>)existingStaffAccResult.DataSource.Data
                                            : new List<FEE_STAFF_ACCOUNT>();
                    var existingStaffAccDict = existingStaffAccs
                                    .GroupBy(e => string.Format("{0}_{1}_{2}_{3}", e.STAFF_ID, e.ACADEMIC_YEAR, e.FREQUENCY_ID, e.HEAD))
                                    .ToDictionary(g => g.Key, g => g.First().F_STAFF_AC_ID);

                    for (int row = 2; row <= rowCount; row++)
                    {
                        string admissionOrRoll = worksheet.Cells[row, 1].Text.Trim();
                        string name = worksheet.Cells[row, 3].Text.Trim();
                        if (string.IsNullOrEmpty(name))
                        {
                            name = worksheet.Cells[row, 2].Text.Trim();
                        }

                        if (string.IsNullOrEmpty(admissionOrRoll))
                        {
                            continue;
                        }

                        // Determine Academic Year, Frequency ID and Main Head ID
                        string acYear = sAcademicYear;
                        string freqId = sFrequencyId;
                        string finalFeeMainHeadId = realFeeMainHeadId;

                        List<string> errors = new List<string>();
                        TRANSPORT_BUS_REGISTRATION student = null;
                        FEE_STAFF_ACCOUNT staff = null;

                        string keyLower = admissionOrRoll.ToLower();
                        if (studentByRoll.ContainsKey(keyLower))
                        {
                            student = studentByRoll[keyLower];
                        }
                        else if (studentByAdm.ContainsKey(keyLower))
                        {
                            student = studentByAdm[keyLower];
                        }
                        else if (staffByCode.ContainsKey(keyLower))
                        {
                            staff = staffByCode[keyLower];
                        }
                        else
                        {
                            errors.Add("Roll No / Staff Code '" + admissionOrRoll + "' not found in student or staff records.");
                        }

                        // Determine Bus and Stage (either from dropdown or Excel row) - only relevant for student bus registration
                        string busId = sBusId;
                        string stageId = sStageId;
                        if ((string.IsNullOrEmpty(busId) || busId == "0") && worksheet.Dimension.End.Column >= 4 && student != null)
                        {
                            string routeName = worksheet.Cells[row, 4].Text.Trim();
                            if (!string.IsNullOrEmpty(routeName))
                            {
                                var stage = stageList.FirstOrDefault(s => s.STAGE_NAME.Trim().ToLower() == routeName.ToLower() && s.ACADEMIC_YEAR == acYear);
                                if (stage != null)
                                {
                                    stageId = stage.BUS_STAGE_ID;
                                    busId = stage.BUS_ID;
                                }
                            }
                        }

                        // Auto-register student if Bus and Boarding stage are selected/configured
                        if (!string.IsNullOrEmpty(busId) && busId != "0" && !string.IsNullOrEmpty(stageId) && stageId != "0" && student != null)
                        {
                            bool isReg = regList.Any(r => r.STUDENT_ID == student.STUDENT_ID && r.STAGE_ID == stageId && r.ACADEMIC_YEAR == acYear);
                            if (!isReg)
                            {
                                string regInsertQuery = "INSERT INTO transport_bus_registration (STUDENT_ID, STAGE_ID, ACADEMIC_YEAR, IS_ACTIVE, IS_DELETED, STATUS, ROLE_ID, ENTRY_TIME) VALUES (" +
                                                        student.STUDENT_ID + ", " + stageId + ", '" + acYear + "', '1', '0', '1', '1', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                var regRes = CMSHandler.SaveOrUpdate(null, regInsertQuery);
                                if (regRes.Success)
                                {
                                    regList.Add(new TRANSPORT_BUS_REGISTRATION
                                    {
                                        STUDENT_ID = student.STUDENT_ID,
                                        STAGE_ID = stageId,
                                        ACADEMIC_YEAR = acYear,
                                        IS_ACTIVE = "1",
                                        IS_DELETED = "0"
                                    });
                                }
                                else
                                {
                                    errors.Add("Failed to auto-register student: " + regRes.Message);
                                }
                            }
                        }

                        // Determine Frequency if not passed from form
                        if (string.IsNullOrEmpty(freqId) || freqId == "0")
                        {
                            string freqName = worksheet.Cells[row, 2].Text.Trim();
                            FEE_FREQUENCY_FEE_MODE freq = freqList.FirstOrDefault(f => f.FREQUENCY_NAME.Trim().ToLower() == freqName.ToLower() && f.ACADEMIC_YEAR == acYear);
                            if (freq != null)
                            {
                                freqId = freq.FREQUENCY_ID;
                                var mHead = mainHeadList.FirstOrDefault(mh => mh.FREQUENCY_ID == freq.FREQUENCY_ID && mh.ACADEMIC_YEAR == acYear);
                                if (mHead != null)
                                {
                                    finalFeeMainHeadId = mHead.FEE_MAIN_HEAD_ID;
                                }
                            }
                            else
                            {
                                errors.Add("Fee frequency '" + freqName + "' not found for academic year " + acYear);
                            }
                        }

                        if (string.IsNullOrEmpty(finalFeeMainHeadId) || finalFeeMainHeadId == "0")
                        {
                            errors.Add("Main fee head not configured for frequency ID '" + freqId + "' and academic year " + acYear);
                        }

                        // Determine Amount
                        string amountStr = "";
                        if (worksheet.Dimension.End.Column >= 5)
                        {
                            amountStr = worksheet.Cells[row, 5].Text.Trim();
                        }
                        else
                        {
                            amountStr = worksheet.Cells[row, worksheet.Dimension.End.Column].Text.Trim();
                        }

                        int amount = 0;
                        if (!int.TryParse(amountStr, out amount) || amount < 0)
                        {
                            errors.Add("Invalid amount: '" + amountStr + "'");
                        }

                        // Restrict creation of duplicate accounts for same frequency, head and year
                        string freqIdToCheck = (!string.IsNullOrEmpty(sFrequencyId) && sFrequencyId != "0") ? sFrequencyId : freqId;
                        if (student != null && !string.IsNullOrEmpty(freqIdToCheck) && freqIdToCheck != "0")
                        {
                            string dupKey = string.Format("{0}_{1}_{2}_{3}", student.STUDENT_ID, acYear, freqIdToCheck, "137");
                            if (existingAccDict.ContainsKey(dupKey))
                            {
                                errors.Add("Bus Fee account already exists for this student, year, and frequency.");
                            }
                        }
                        else if (staff != null && !string.IsNullOrEmpty(freqIdToCheck) && freqIdToCheck != "0")
                        {
                            string dupKey = string.Format("{0}_{1}_{2}_{3}", staff.STAFF_ID, acYear, freqIdToCheck, "137");
                            if (existingStaffAccDict.ContainsKey(dupKey))
                            {
                                errors.Add("Bus Fee account already exists for this staff, year, and frequency.");
                            }
                        }

                        if (errors.Count > 0)
                        {
                            string displayName = name;
                            if (string.IsNullOrEmpty(displayName))
                            {
                                if (student != null) displayName = student.FIRST_NAME;
                                else if (staff != null) displayName = staff.STAFF_NAME;
                            }
                            invalidRecords.Add(new
                            {
                                Row = row,
                                AdmissionNo = admissionOrRoll,
                                Name = displayName,
                                Reason = string.Join("; ", errors)
                            });
                        }
                        else
                        {
                            if (student != null)
                            {
                                // Insert new student account directly
                                var newAcc = new FEE_STUDENT_ACCOUNT
                                {
                                    STUDENT_ID = student.STUDENT_ID.ToString(),
                                    ACADEMIC_YEAR = acYear,
                                    FREQUENCY_ID = freqId,
                                    HEAD = "137",
                                    CREDIT = amount.ToString(),
                                    FEE_MAIN_HEAD_ID = finalFeeMainHeadId
                                };
                                string insertQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.SaveBusFeeStudentAccount);
                                var res = CMSHandler.SaveOrUpdate(newAcc, insertQuery);
                                if (res.Success)
                                {
                                    successCount++;
                                    string dupKey = string.Format("{0}_{1}_{2}_{3}", student.STUDENT_ID, acYear, freqIdToCheck, "137");
                                    existingAccDict[dupKey] = newAcc.F_STUDENT_AC_ID;
                                }
                                else
                                {
                                    invalidRecords.Add(new
                                    {
                                        Row = row,
                                        AdmissionNo = admissionOrRoll,
                                        Name = student.FIRST_NAME,
                                        Reason = "DB Insert Error: " + res.Message
                                    });
                                }
                            }
                            else if (staff != null)
                            {
                                // Insert new staff account directly
                                var newStaffAcc = new FEE_STAFF_ACCOUNT
                                {
                                    STAFF_ID = staff.STAFF_ID.ToString(),
                                    ACADEMIC_YEAR = acYear,
                                    FREQUENCY_ID = freqId,
                                    HEAD = "137",
                                    CREDIT = amount.ToString(),
                                    FEE_MAIN_HEAD_ID = finalFeeMainHeadId,
                                    TRANSACTION_DATE = DateTime.Now.ToString("yyyy-MM-dd")
                                };
                                string insertStaffQuery = "INSERT INTO fee_staff_account (STAFF_ID, ACADEMIC_YEAR, FREQUENCY_ID, HEAD, CREDIT, FEE_MAIN_HEAD_ID, TRANSACTION_DATE, IS_DELETED) VALUES (?STAFF_ID, ?ACADEMIC_YEAR, ?FREQUENCY_ID, ?HEAD, ?CREDIT, ?FEE_MAIN_HEAD_ID, ?TRANSACTION_DATE, 0)";
                                var res = CMSHandler.SaveOrUpdate(newStaffAcc, insertStaffQuery);
                                if (res.Success)
                                {
                                    successCount++;
                                    string dupKey = string.Format("{0}_{1}_{2}_{3}", staff.STAFF_ID, acYear, freqIdToCheck, "137");
                                    existingStaffAccDict[dupKey] = newStaffAcc.F_STAFF_AC_ID;
                                }
                                else
                                {
                                    invalidRecords.Add(new
                                    {
                                        Row = row,
                                        AdmissionNo = admissionOrRoll,
                                        Name = staff.STAFF_NAME,
                                        Reason = "DB Insert Error: " + res.Message
                                    });
                                }
                            }
                        }
                    }
                }

                return Json(new { Success = true, Message = string.Format("Successfully inserted {0} valid records.", successCount), InvalidRecords = invalidRecords, SuccessCount = successCount });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }
        #endregion

        #region All-in-One Dues Payment

        public ActionResult PayStudentDues()
        {
            List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                var res = CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAllFeeDuesByStudentId), sAcademicYear);
                if (res != null && res.DataSource != null && res.DataSource.Data != null)
                {
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)res.DataSource.Data;
                    studentAcountList = studentAcountList.Where(o => o.FREQUENCY_TYPE_ID == Common.FrequencyType.BusFee).ToList();
                    studentAcountList = studentAcountList
                        .OrderBy(s => (Convert.ToDouble(string.IsNullOrEmpty(s.BALANCE) ? "0" : s.BALANCE) <= 0 ? 1 : 0))
                        .ThenBy(s => s.HEAD)
                        .ToList();
                }
            }
            return View(studentAcountList);
        }

        public ActionResult PayStaffDues()
        {
            List<FEE_STUDENT_ACCOUNT> staffAccountList = new List<FEE_STUDENT_ACCOUNT>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                var res = CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAllFeeDuesByStaffId), sAcademicYear);
                if (res != null && res.DataSource != null && res.DataSource.Data != null)
                {
                    staffAccountList = (List<FEE_STUDENT_ACCOUNT>)res.DataSource.Data;
                    staffAccountList = staffAccountList
                        .OrderBy(s => (Convert.ToDouble(string.IsNullOrEmpty(s.BALANCE) ? "0" : s.BALANCE) <= 0 ? 1 : 0))
                        .ThenBy(s => s.HEAD)
                        .ToList();
                }
            }
            return View(staffAccountList);
        }

        #endregion

    }
}

