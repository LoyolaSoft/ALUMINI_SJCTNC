using CMS.DAO;
using CMS.Models;
using CMS.SQL.Alumnae;
using CMS.SQL.Attendance;
using CMS.SQL.Examination;
using CMS.SQL.SupportData;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
//using IronBarCode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace CMS.Controllers
{
    public class SupportDataController : Controller
    {
        ResultArgs resultArgs = new ResultArgs();
        public SupBankAccount ObjSupBankViewModel { get; private set; }
        #region Sup_Head List
        /// <summary>
        /// This Action is to show Sup head list                        
        /// </summary>
        /// <returns>FeeViewModel</returns>
        /// 
        [UserRights("EXAMINCHARGE")]
        public ActionResult SupHeadList()
        {
            FeeViewModel ObjFeeViewModel = new FeeViewModel();
            ObjFeeViewModel.liSup_Head = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupHeadList)).DataSource.Data;
            return View(ObjFeeViewModel);
        }
        /// <summary>
        /// This 
        /// </summary>
        /// <returns></returns>
        public JsonResult FetchddlCategory()
        {
            JsonResultData objResult = new JsonResultData();
            string sOption = string.Empty;
            var liFee = new List<Sup_Fee_Category>();
            liFee = (List<Sup_Fee_Category>)CMSHandler.FetchData<Sup_Fee_Category>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchddlCategory)).DataSource.Data;
            if (liFee != null)
            {
                foreach (var item in liFee)
                {
                    sOption += "<option value='" + item.FEE_CATEGORY_ID + "'>" + item.FEE_CATEGORY + "</option>";
                }
                objResult.ErrorCode = Common.ErrorCode.Ok;
                objResult.Message = Common.ErrorMessage.Ok;
                objResult.sResult = sOption;
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.NoContent;
                objResult.Message = Common.ErrorMessage.NoContent;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// this action to fetch suphed info by headid 
        /// </summary>
        /// <param name="HeadId"></param>
        /// <returns></returns>
        public JsonResult EditSupHead(string HeadId)
        {
            var ObjHead = new SUP_HEAD();
            var liHead = new List<SUP_HEAD>();
            ObjHead.HEAD_ID = HeadId;
            liHead = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(ObjHead, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupHeadForEdit)).DataSource.Data;
            return Json(liHead, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// this action to Save Records ...
        /// </summary>
        /// <param name="HeadId"></param>
        /// <returns></returns>
        public JsonResult SaveSupHead(string HeadId, string Head, string category)
        {
            JsonResultData objResult = new JsonResultData();
            if (!string.IsNullOrEmpty(Head) && !string.IsNullOrEmpty(category))
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var ObjHead = new SUP_HEAD() { HEAD = Head, HEAD_ID = HeadId, FEE_CATEGORY = category };
                    var liHead = new List<SUP_HEAD>();
                    liHead = (List<SUP_HEAD>)CMSHandler.FetchData<SUP_HEAD>(ObjHead, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.IsSupHeadExist)).DataSource.Data;
                    if (liHead == null)
                    {
                        var sresultArgs = CMSHandler.SaveOrUpdate(ObjHead, SupportDataSQL.GetSupportDataSQL((!string.IsNullOrEmpty(HeadId)) ? SupportDataSQLCommands.UpdateSupHead : SupportDataSQLCommands.SaveSupHead));
                        objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                    else
                    {
                        objResult.Message = "Allready Exits ...!";
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

        public JsonResult DeleteSupHead(string HeadId)
        {
            JsonResultData ObjResult = new JsonResultData();
            if (!string.IsNullOrEmpty(HeadId))
            {
                var ObjHead = new SUP_HEAD();
                ObjHead.HEAD_ID = HeadId;
                var sresultArgs = CMSHandler.SaveOrUpdate(ObjHead, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteSupHead));
                ObjResult.sResult = (sresultArgs.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                ObjResult.ErrorCode = Common.ErrorCode.Ok;
                ObjResult.Message = Common.ErrorMessage.Ok;
            }
            else
            {
                ObjResult.ErrorCode = Common.ErrorCode.BadRequest;
                ObjResult.Message = Common.ErrorMessage.ExpectationFailed;
            }
            return Json(ObjResult, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Sup_Main_head

        #endregion
        public ActionResult Index()
        {
            return View();
        }
        #region Fee Sub Fee Main Head
        [UserRights("EXAMINCHARGE")]
        public async Task<ActionResult> SupFeeMainHeadList()
        {
            SupFeeMainHead objViewModel = new SupFeeMainHead();
            var ListSupFeeMainHead = await Task.Run(() => (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeMainHead)).DataSource.Data);
            objViewModel.lstSupFeeMainHead = ListSupFeeMainHead;
            return View(objViewModel);
        }
        public JsonResult FetchSupFeeMainHeadById(string sMainHeadId)
        {
            string sOption = string.Empty;
            SUP_FEE_MAIN_HEAD objModel = new SUP_FEE_MAIN_HEAD();
            objModel.MAIN_HEAD_ID = sMainHeadId;
            var FetchExistingFeeHead = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeMainHeadById)).DataSource.Data;
            return Json(FetchExistingFeeHead);
        }
        public JsonResult SaveSupFeeMainHead(string sMainHead)
        {
            JsonResultData objResult = new JsonResultData();
            string sOption = string.Empty;
            if (!string.IsNullOrEmpty(sMainHead))
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    SUP_FEE_MAIN_HEAD objModel = new SUP_FEE_MAIN_HEAD();
                    objModel.MAIN_HEAD = sMainHead;
                    var FetchExistingFeeHead = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.IsSupFeeMainHeadExisting)).DataSource.Data;
                    if (FetchExistingFeeHead != null && FetchExistingFeeHead.Count > 0)
                    {
                        objResult.ErrorCode = Common.ErrorCode.Ok;
                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        //objResult.ErrorCode = "This Sup Fee Main Head is Existing...!!!";
                    }
                    else
                    {
                        var ResultArg = CMSHandler.SaveOrUpdate(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SaveSupFeeMainHead));
                        if (ResultArg.Success)
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
        public JsonResult UpdateSupFeeMainHead(string sFeeMainHead)
        {
            JsonResultData objResult = new JsonResultData();
            string sOption = string.Empty;
            if (!string.IsNullOrEmpty(sFeeMainHead))
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var Result = JsonConvert.DeserializeObject<SUP_FEE_MAIN_HEAD>(sFeeMainHead);
                    var FetchExistingFeeHead = (List<SUP_FEE_MAIN_HEAD>)CMSHandler.FetchData<SUP_FEE_MAIN_HEAD>(Result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.IsSupFeeMainHeadExisting)).DataSource.Data;
                    if (FetchExistingFeeHead != null && FetchExistingFeeHead.Count > 0)
                    {
                        objResult.ErrorCode = Common.ErrorCode.Ok;
                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                    }
                    else
                    {
                        var ResultArg = CMSHandler.SaveOrUpdate(Result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateSupFeeMainHead));
                        if (ResultArg.Success)
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
        public JsonResult DeleteSupFeeMainHead(string sFeeMainHead)
        {
            JsonResultData objResult = new JsonResultData();
            if (sFeeMainHead != null)
            {
                SUP_FEE_MAIN_HEAD objModel = new SUP_FEE_MAIN_HEAD();
                objModel.MAIN_HEAD_ID = sFeeMainHead;
                ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteSupFeeMainHead));
                if (ResultArg.Success)
                {
                    objResult.ErrorCode = Common.ErrorCode.Ok;
                    objResult.Message = Common.Messages.RecordDeletedSuccessfully;
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                    objResult.Message = Common.Messages.FailedToDeletedTryAgain;
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Sup Bank
        [UserRights("EXAMINCHARGE")]
        public ActionResult SupBankList()
        {
            SupBank ObjSupBankViewModel = new SupBank();
            ObjSupBankViewModel.lstSupBank = (List<SUP_BANK>)CMSHandler.FetchData<SUP_BANK>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupBank)).DataSource.Data;
            return View(ObjSupBankViewModel);
        }
        public JsonResult DeleteSupBank(string sBankId)
        {
            JsonResultData objResult = new JsonResultData();
            if (sBankId != null)
            {
                SUP_BANK objModel = new SUP_BANK();
                objModel.BANK_ID = sBankId;
                ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteSupBank));
                if (ResultArg.Success)
                {
                    objResult.ErrorCode = Common.ErrorCode.Ok;
                    objResult.Message = Common.Messages.RecordDeletedSuccessfully;
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                    objResult.Message = Common.Messages.FailedToDeletedTryAgain;
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SupBank(string sBankId)
        {
            SupBank objViewModel = new SupBank();
            SUP_BANK objModel = new SUP_BANK();
            objModel.BANK_ID = sBankId;
            if (sBankId != null)
            {
                var SupBankList = (List<SUP_BANK>)CMSHandler.FetchData<SUP_BANK>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupBankById)).DataSource.Data;
                objViewModel.Bank.BANK_NAME = SupBankList.FirstOrDefault().BANK_NAME;
                objViewModel.Bank.ADDRESS = SupBankList.FirstOrDefault().ADDRESS;
                objViewModel.Bank.PHONE = SupBankList.FirstOrDefault().PHONE;
                objViewModel.Bank.BANK_ID = sBankId;
                return View(objViewModel);
            }
            return View();
        }
        public JsonResult SaveOrUpdateSupBank(string sBank)
        {
            JsonResultData objResult = new JsonResultData();
            SUP_BANK objModel = new SUP_BANK();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                if (!string.IsNullOrEmpty(sBank))
                {
                    var BankDetails = JsonConvert.DeserializeObject<SUP_BANK>(sBank);
                    if (BankDetails != null)
                    {
                        var result = CMSHandler.SaveOrUpdate(BankDetails, SupportDataSQL.GetSupportDataSQL((!string.IsNullOrEmpty(BankDetails.BANK_ID)) ? SupportDataSQLCommands.UpdateSupBank : SupportDataSQLCommands.SaveSupBank));
                        objResult.ErrorCode = Common.ErrorCode.Ok;
                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                    }
                }
            }
            else
            {
                objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Sup Bank Account

        public ActionResult SupBankAccount(string sAccountBankId)
        {
            var BankId = string.Empty;
            SupBankAccount ObjSupBankViewModel = new SupBankAccount();
            SUP_BANK_ACCOUNT objModel = new SUP_BANK_ACCOUNT();
            objModel.BANK_ACCOUNT_ID = sAccountBankId;
            var BankList = (List<SUP_BANK>)CMSHandler.FetchData<SUP_BANK>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchBankList)).DataSource.Data;
            if (!string.IsNullOrEmpty(objModel.BANK_ACCOUNT_ID))
            {
                var SupBankAccountList = (List<SUP_BANK_ACCOUNT>)CMSHandler.FetchData<SUP_BANK_ACCOUNT>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupBankAcountById)).DataSource.Data;
                if (SupBankAccountList != null && SupBankAccountList.Count > 0)
                {
                    BankId = SupBankAccountList.FirstOrDefault().BANK;
                    ObjSupBankViewModel.BankAccount.BANK = BankId;
                    ObjSupBankViewModel.BankAccount.ACCOUNT_PURPOSE = SupBankAccountList.FirstOrDefault().ACCOUNT_PURPOSE;
                    ObjSupBankViewModel.BankAccount.PASSBOOK_NO = SupBankAccountList.FirstOrDefault().PASSBOOK_NO;
                    ObjSupBankViewModel.BankAccount.STARTED_DATE = SupBankAccountList.FirstOrDefault().STARTED_DATE;
                    ObjSupBankViewModel.BankAccount.CLOSED_DATE = SupBankAccountList.FirstOrDefault().CLOSED_DATE;
                    ObjSupBankViewModel.BankAccount.BANK_ACCOUNT_ID = sAccountBankId;
                    ObjSupBankViewModel.BankList = new SelectList(BankList, Common.SUP_BANK.BANK_ID, Common.SUP_BANK.BANK_NAME, BankId);
                }
            }
            else
            {
                ObjSupBankViewModel.BankList = new SelectList(BankList, Common.SUP_BANK.BANK_ID, Common.SUP_BANK.BANK_NAME);
            }
            return View(ObjSupBankViewModel);
        }
        [UserRights("EXAMINCHARGE")]
        public ActionResult ListSupBankAccount()
        {
            SupBankAccount ObjSupBankViewModel = new SupBankAccount();
            ObjSupBankViewModel.lstSupBankAccount = (List<SUP_BANK_ACCOUNT>)CMSHandler.FetchData<SUP_BANK_ACCOUNT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupBankAccount)).DataSource.Data;
            return View(ObjSupBankViewModel);
        }
        public JsonResult DeleteSupBankAccount(string sBankAccountId)
        {
            JsonResultData objResult = new JsonResultData();
            if (sBankAccountId != null)
            {
                SUP_BANK_ACCOUNT objModel = new SUP_BANK_ACCOUNT();
                objModel.BANK_ACCOUNT_ID = sBankAccountId;
                ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteSupBankAccount));
                if (ResultArg.Success)
                {
                    objResult.ErrorCode = Common.ErrorCode.Ok;
                    objResult.Message = Common.Messages.RecordDeletedSuccessfully;
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                    objResult.Message = Common.Messages.FailedToDeletedTryAgain;
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOrUpdateSupBankAccount(string sBankAccount)
        {
            JsonResultData objResult = new JsonResultData();
            SUP_BANK_ACCOUNT objModel = new SUP_BANK_ACCOUNT();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                if (!string.IsNullOrEmpty(sBankAccount))
                {
                    var AccountDetails = JsonConvert.DeserializeObject<SUP_BANK_ACCOUNT>(sBankAccount);
                    if (AccountDetails != null)
                    {
                        if (string.IsNullOrEmpty(AccountDetails.BANK_ACCOUNT_ID))
                        {
                            AccountDetails.STARTED_DATE = CommonMethods.ConvertdatetoyearFromat(AccountDetails.STARTED_DATE);
                            AccountDetails.CLOSED_DATE = CommonMethods.ConvertdatetoyearFromat(AccountDetails.CLOSED_DATE);
                            var FetchExistingAccount = (List<SUP_BANK_ACCOUNT>)CMSHandler.FetchData<SUP_BANK_ACCOUNT>(AccountDetails, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupBankAcountByAccountId)).DataSource.Data;
                            if (FetchExistingAccount != null && FetchExistingAccount.Count > 0)
                            {
                                objResult.ErrorCode = Common.ErrorCode.Ok;
                                objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                            }
                            else
                            {
                                var result = CMSHandler.SaveOrUpdate(AccountDetails, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SaveSupBankAccount));
                                if (result.Success)
                                {
                                    objResult.ErrorCode = Common.ErrorCode.Ok;
                                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                                    objResult.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                        else
                        {
                            AccountDetails.STARTED_DATE = CommonMethods.ConvertdatetoyearFromat(AccountDetails.STARTED_DATE);
                            AccountDetails.CLOSED_DATE = CommonMethods.ConvertdatetoyearFromat(AccountDetails.CLOSED_DATE);
                            var result = CMSHandler.SaveOrUpdate(AccountDetails, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateSupBankAccount));
                            if (result.Success)
                            {
                                objResult.ErrorCode = Common.ErrorCode.Ok;
                                objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                            }
                            else
                            {
                                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                                objResult.Message = Common.Messages.FailedToSaveRecords;
                            }
                        }
                    }
                }
            }
            else
            {
                objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Frequency Crude
        public ActionResult SubFrequencyList()
        {
            SubFrequencyViewModel objViewModel = new SubFrequencyViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                objViewModel.lstSupFeeFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSubFeeFrequency)).DataSource.Data;
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objViewModel);
        }
        public JsonResult FetchFrequencyType()
        {
            JsonResultData objResult = new JsonResultData();
            string sOption = string.Empty;
            string sSemesterType = string.Empty;
            string sFrequency = string.Empty;
            string JsonData = string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                var liFrequencyType = new List<sup_frequency_type>();
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                liFrequencyType = (List<sup_frequency_type>)CMSHandler.FetchData<sup_frequency_type>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFrequencytype)).DataSource.Data;
                var liSemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
                var liFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFrequency), sAcademicYear).DataSource.Data;
                if (liFrequencyType != null)
                {
                    foreach (var item in liFrequencyType)
                        sOption += "<option value='" + item.FREQUENCY_TYPE_ID + "'>" + item.FREQUENCY_TYPE + "</option>";
                }
                if (liSemesterType != null && liSemesterType.Count > 0)
                {
                    foreach (var item in liSemesterType)
                        sSemesterType += "<option value='" + item.SEMESTER_TYPE_ID + "'>" + item.SEMESTER_TYPE + "</option>";
                }
                if (liFrequency != null && liFrequency.Count > 0)
                {
                    foreach (var item in liFrequency)
                        sFrequency += "<option value='" + item.FREQUENCY_ID + "'>" + item.FREQUENCY_NAME + "</option>";
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                objResult.Message = Common.ErrorMessage.RequestTimeout;
            }
            var objJsonData = new SUP_FEE_FREQUENCY() { TYPE = sOption, SEMESTER_TYPE = sSemesterType, FREQUENCY_ID = sFrequency };
            JsonData = JsonConvert.SerializeObject(objJsonData);
            return Json(JsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOrUpdateSubFrequency(string sFrequency)
        {
            JsonResultData objResult = new JsonResultData();
            var liFrequency = new List<SUP_FEE_FREQUENCY>();
            if (sFrequency != null)
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var Result = JsonConvert.DeserializeObject<SUP_FEE_FREQUENCY>(sFrequency);
                    Result.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(Result.DATE_FROM);
                    Result.DATE_TO = CommonMethods.ConvertdatetoyearFromat(Result.DATE_TO);
                    Result.LAST_DATE_OF_PAYMENT = CommonMethods.ConvertdatetoyearFromat(Result.LAST_DATE_OF_PAYMENT);
                    Result.ACADEMIC_YEAR = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (string.IsNullOrEmpty(Result.FREQUENCY_ID))
                    {
                        liFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(Result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.CheckIsFrequencyExisting)).DataSource.Data;
                    }
                    else
                    {
                        var sresultArgs = CMSHandler.SaveOrUpdate(Result, SupportDataSQL.GetSupportDataSQL((!string.IsNullOrEmpty(Result.FREQUENCY_ID)) ? SupportDataSQLCommands.UpdateSupFeeFrequency : SupportDataSQLCommands.SaveSubFeeFrequency));
                        objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                    if (liFrequency == null)
                    {
                        var sresultArgs = CMSHandler.SaveOrUpdate(Result, SupportDataSQL.GetSupportDataSQL((!string.IsNullOrEmpty(Result.FREQUENCY_ID)) ? SupportDataSQLCommands.UpdateSupFeeFrequency : SupportDataSQLCommands.SaveSubFeeFrequency));
                        objResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                    else
                    {
                        objResult.Message = "Allready Exits ...!";
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
        public JsonResult FetchSubFrequencyById(string sFrequencyId)
        {
            JsonResultData objResult = new JsonResultData();
            SUP_FEE_FREQUENCY objModel = new SUP_FEE_FREQUENCY();
            if (sFrequencyId != null)
            {
                string sOption = string.Empty;
                objModel.FREQUENCY_ID = sFrequencyId;
                var FetchFrequency = (List<SUP_FEE_FREQUENCY>)CMSHandler.FetchData<SUP_FEE_FREQUENCY>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupFeeFrequencyById)).DataSource.Data;
                return Json(FetchFrequency);
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = Common.ErrorMessage.BadRequest;
                return Json(objResult, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteSupFrequency(string sFrequencyId)
        {
            JsonResultData ObjResult = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                if (!string.IsNullOrEmpty(sFrequencyId))
                {
                    var objModel = new SUP_FEE_FREQUENCY();
                    objModel.FREQUENCY_ID = sFrequencyId;
                    var sresultArgs = CMSHandler.SaveOrUpdate(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteSupFeeFrequency));
                    ObjResult.sResult = (sresultArgs.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjResult.ErrorCode = Common.ErrorCode.Ok;
                    ObjResult.Message = Common.ErrorMessage.Ok;
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

        #region Fetch All The Dropdowns
        public JsonResult FetchSupportDataForDropDown()
        {
            JsonResultData objResult = new JsonResultData();
            Student_Personal_Info objModel = new Student_Personal_Info();
            string sMotherTongue = string.Empty;
            string sReligion = string.Empty;
            string sCaste = string.Empty;
            string sCountry = string.Empty;
            string sGender = string.Empty;
            string sHSSGroups = string.Empty;
            string sOccupation = string.Empty;
            string sClass = string.Empty;
            string sADMClass = string.Empty;
            string sProgrammeGroups = string.Empty;
            string sBloodGroup = string.Empty;
            string sUniversity = string.Empty;
            string sState = string.Empty;
            string sMainCommunity = string.Empty;
            string JsonData = string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                //objModel.CLASS = sClassId;
                var FetchMotherTongue = (List<SupMotherTongue>)CMSHandler.FetchData<SupMotherTongue>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMotherTongue)).DataSource.Data;
                var FetchReligion = (List<SUP_RELIGION>)CMSHandler.FetchData<SUP_RELIGION>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchReligion), sAcademicyear).DataSource.Data;
                var FetchCaste = (List<SupCommunity>)CMSHandler.FetchData<SupCommunity>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCommunity), sAcademicyear).DataSource.Data;
                var FetchCountry = (List<SUP_COUNTRY>)CMSHandler.FetchData<SUP_COUNTRY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCountry), sAcademicyear).DataSource.Data;
                var FetchGender = (List<SupGender>)CMSHandler.FetchData<SupGender>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchGender), sAcademicyear).DataSource.Data;
                // var FetchHSSGroups = (List<SUP_HSS_GROUPS>)CMSHandler.FetchData<SUP_HSS_GROUPS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHSSGroups), sAcademicyear).DataSource.Data;
                var FetchOccupation = (List<Sup_Occupation>)CMSHandler.FetchData<Sup_Occupation>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOccupation), sAcademicyear).DataSource.Data;
                var FetchProgrammeGroups = (List<SUP_APPTYPE_PROG_GROUPS>)CMSHandler.FetchData<SUP_APPTYPE_PROG_GROUPS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeByApprogramme), sAcademicyear).DataSource.Data;
                var Fetchclass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchclass), sAcademicyear).DataSource.Data;
                var FetchADMclass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchADMClass), sAcademicyear).DataSource.Data;
                var FetchBloodGroup = (List<SupBloodGroup>)CMSHandler.FetchData<SupBloodGroup>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchBloodGroup), sAcademicyear).DataSource.Data;
                var FetchUniversity = (List<SUP_UNIVERSITY>)CMSHandler.FetchData<SUP_UNIVERSITY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchUniversity), sAcademicyear).DataSource.Data;
                var FetchState = (List<SUP_STATE>)CMSHandler.FetchData<SUP_STATE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchState), sAcademicyear).DataSource.Data;
                var FetchMainCommunity = (List<MainCommunity>)CMSHandler.FetchData<MainCommunity>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMainCommunity), sAcademicyear).DataSource.Data;
                if (FetchMotherTongue != null && FetchMotherTongue.Count > 0)
                {
                    foreach (var item in FetchMotherTongue)
                        sMotherTongue += "<option value='" + item.MOTHER_TONGUE_ID + "'>" + item.MOTHER_TONGUE_NAME + "</option>";
                }
                if (FetchReligion != null && FetchReligion.Count > 0)
                {
                    foreach (var item in FetchReligion)
                        sReligion += "<option value='" + item.RELIGIONID + "'>" + item.RELIGION + "</option>";
                }
                if (FetchCaste != null && FetchCaste.Count > 0)
                {
                    foreach (var item in FetchCaste)
                        sCaste += "<option value='" + item.COMMUNITYID + "'>" + item.COMMUNITY + "</option>";
                }
                if (FetchCountry != null && FetchCountry.Count > 0)
                {
                    foreach (var item in FetchCountry)
                        sCountry += "<option value='" + item.COUNTRY_ID + "'>" + item.COUNTRY_NAME + "</option>";
                }
                if (FetchGender != null && FetchGender.Count > 0)
                {
                    foreach (var item in FetchGender)
                        sGender += "<option value='" + item.GENDER_ID + "'>" + item.GENDER_NAME + "</option>";
                }
                //if (FetchHSSGroups != null && FetchHSSGroups.Count > 0)
                //{
                //    foreach (var item in FetchHSSGroups)
                //        sHSSGroups += "<option value='" + item.HSS_GROUP_ID + "'>" + item.HSS_GROUP + "</option>";
                //}
                if (FetchOccupation != null && FetchOccupation.Count > 0)
                {
                    foreach (var item in FetchOccupation)
                        sOccupation += "<option value='" + item.OCCUPATION_ID + "'>" + item.OCCUPATION_NAME + "</option>";
                }
                if (FetchProgrammeGroups != null && FetchProgrammeGroups.Count > 0)
                {
                    foreach (var item in FetchProgrammeGroups)
                        sProgrammeGroups += "<option value='" + item.PROGRAMME_GROUP_ID + "'>" + item.PROGRAMME_NAME + "</option>";
                }
                if (Fetchclass != null && Fetchclass.Count > 0)
                {
                    foreach (var item in Fetchclass)
                        sClass += "<option value='" + item.CLASS_ID + "'>" + item.CLASS_NAME + "</option>";
                }
                if (FetchBloodGroup != null && FetchBloodGroup.Count > 0)
                {
                    foreach (var item in FetchBloodGroup)
                        sBloodGroup += "<option value='" + item.BLOOD_GROUP_ID + "'>" + item.BLOOD_GROUP_NAME + "</option>";
                }
                if (FetchUniversity != null && FetchUniversity.Count > 0)
                {
                    foreach (var item in FetchUniversity)
                        sUniversity += "<option value='" + item.UNIVERSITY_ID + "'>" + item.UNIVERSITY + "</option>";
                }
                if (FetchState != null && FetchState.Count > 0)
                {
                    foreach (var item in FetchState)
                        sState += "<option value='" + item.STATE_ID + "'>" + item.STATE + "</option>";
                }
                if (FetchMainCommunity != null && FetchMainCommunity.Count > 0)
                {
                    foreach (var item in FetchMainCommunity)
                        sMainCommunity += "<option value='" + item.MAIN_COMMUNITY_ID + "'>" + item.MAIN_COMMUNITY + "</option>";
                }
                if (FetchADMclass != null && FetchADMclass.Count > 0)
                {
                    foreach (var item in FetchADMclass)
                        sADMClass += "<option value='" + item.CLASS_ID + "'>" + item.CLASS_NAME + "</option>";
                }
                var objJsonData = new DropDownListModel() { MOTHER_TONGUE = sMotherTongue, RELIGION_ID = sReligion, CASTE = sCaste, CCOUNTRY = sCountry, GENDER = sGender, HSS_GROUP_ID = sHSSGroups, OCCUPATION = sOccupation, PROGRAMME_GROUP_ID = sProgrammeGroups, CLASS_ID = sClass, BLOOD_GROUP = sBloodGroup, UNIVERSITY = sUniversity, CSTATE = sState, MAIN_COMMUNITY = sMainCommunity, ADM_CLASS = sADMClass };
                objResult.sResult = JsonConvert.SerializeObject(objJsonData);
            }
            else
            {
                objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResult.ErrorCode = Common.ErrorMessage.RequestTimeout;
            }

            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ImageCompression
        public ActionResult CompressImage()
        {
            return View();
        }

        public JsonResult CompressingImagesize()
        {
            JsonResultData ObjJsondata = new JsonResultData();
            var sFile = Request.Files[0];
            //DirectoryInfo Folder;
            //FileInfo[] Images;

            if (Request.Files.Count > 0)
            {
                string _sDirectorypath = string.Empty;

                Stream strm = sFile.InputStream;
                _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.STUDENT_IMGAE_PATH;
                if (!Directory.Exists(_sDirectorypath))
                {
                    Directory.CreateDirectory(_sDirectorypath);
                }
                string filePath = _sDirectorypath + sFile.FileName;
                string sSavePath = "\\" + Common.URLPages.STUDENT_IMGAE_PATH + sFile.FileName;
                // Compression Method ....
                string sFilePath = CommonMethods.CompressImageQuality(strm, filePath, sFile.FileName, sSavePath);

            }
            return Json(ObjJsondata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UploadFileBasedImage()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            JsonResultData ObjJsonData = new JsonResultData();
            int count = 0;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string _sDirectorypath = string.Empty;
                    if (Request.Files.Count > 0)
                    {

                        var ObjStudent = new STU_PERSONAL_INFO();
                        var UserAccount = new USER_ACCOUNT_INFO();
                        var liStudent = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFirstYearStudentToInsertImage), sAcademicYear).DataSource.Data;
                        if (liStudent != null && liStudent.Count > 0)
                        {
                            for (int i = 0; i < Request.Files.Count; i++)
                            {
                                var sFile = Request.Files[i];
                                var sImages = sFile.FileName.Split('.');
                                Stream strm = sFile.InputStream;
                                var liReplaceImage = liStudent.Where(x => x.ADMISSION_NO == sImages[0]).ToList();
                                if (liReplaceImage != null && liReplaceImage.Count > 0)
                                {
                                    _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.STUDENT_IMGAE_PATH + "\\" + liReplaceImage.FirstOrDefault().DEPARTMENT + "\\" + liReplaceImage.FirstOrDefault().CLASS_NAME;
                                    if (!Directory.Exists(_sDirectorypath))
                                    {
                                        Directory.CreateDirectory(_sDirectorypath);
                                    }

                                    string filePath = _sDirectorypath;
                                    string sFileName = sFile.FileName.Replace(sImages[0], liReplaceImage.FirstOrDefault().ROLL_NO);
                                    string sSavePath = "\\" + Common.URLPages.STUDENT_IMGAE_PATH + "\\" + liReplaceImage.FirstOrDefault().DEPARTMENT + "\\" + liReplaceImage.FirstOrDefault().CLASS_NAME + "\\" + sFileName;
                                    string sFilePath = CommonMethods.CompressImageQuality(strm, filePath + "\\" + sFileName, sFileName, sSavePath);
                                    var PhotoPath = sFilePath.Split(',');

                                    ObjStudent.STUDENT_ID = liReplaceImage.FirstOrDefault().STUDENT_ID;
                                    ObjStudent.PHOTO = PhotoPath[0];

                                    UserAccount.USER_ID = liReplaceImage.FirstOrDefault().STUDENT_ID;
                                    UserAccount.ROLE = "5";
                                    UserAccount.PHOTO = PhotoPath[0];
                                    var UpdatePersonalInfo = CMSHandler.SaveOrUpdate(ObjStudent, SQL.Student.StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateStuPersonalImages), sAcademicYear);
                                    var UnSafeUpdateMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                                    var sSQL = CMSHandler.SaveOrUpdate(UserAccount, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.UpdateUserAccountImage));
                                    var SafeUpdateMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                                    count++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("UploadFileBasedImage", "SupportDataController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("UploadFileBasedImage", "SupportDataController", ex.Message);
                    }
                    ObjJsonData.Message = ex.Message;
                }
                ObjJsonData.sResult = count.ToString();
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            //string sFolderName = @"C:\Users\HCCERP01\Desktop\StudentImages\Student";
            //string _sDirectorypath = string.Empty;


            //if (!string.IsNullOrEmpty(sAcademicYear))
            //{
            //    var sFile = Request.Files[0];
            //    Stream strm = sFile.InputStream;
            //    var liStudent = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFirstYearStudentToInsertImage),sAcademicYear).DataSource.Data;
            //    if (liStudent!=null && liStudent.Count>0)
            //    {
            //        DirectoryInfo Folder;
            //        FileInfo[] Images;
            //        Folder = new DirectoryInfo(sFolderName);
            //        Images = Folder.GetFiles();

            //        // List<String> imagesList = new List<String>();
            //       // var sFile = Images;

            //        for (int i = 0; i < Images.Length; i++)
            //        {
            //            var sImages = Images[i].Name.Split('.');
            //            var liReplaceImage = liStudent.Where(x => x.ADMISSION_NO == sImages[0]).ToList();
            //            if (liReplaceImage!=null && liReplaceImage.Count>0)
            //            {
            //                _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.STUDENT_IMGAE_PATH + "\\" + liReplaceImage.FirstOrDefault().DEPARTMENT + "\\" + liReplaceImage.FirstOrDefault().CLASS_NAME;
            //                if (!Directory.Exists(_sDirectorypath))
            //                {
            //                    Directory.CreateDirectory(_sDirectorypath);
            //                }

            //               // imagesList.Add(String.Format(@"{0}/{1}", sFolderName, Images[i].Name));
            //            }

            //        }
            //    }
            //}
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveStudentImages()
        {
            var ObjJsonData = new JsonResultData();
            var ObjStudent = new STU_PERSONAL_INFO();
            var UserAccount = new USER_ACCOUNT_INFO();
            string _sDirectorypath = string.Empty;
            int COUNT = 0;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                DirectoryInfo d = new DirectoryInfo(@"F:\ERP DATA\coe\James sir\APRIL-2020 CONVOCATION");//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles(); //Getting Text files

                //var IsExist = new List<ListStudents>();
                //var liStudent = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(null,SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByAcademicYear), sAcademicYear).DataSource.Data;
                var IsExist = new List<Demo_test_table>();
                var liStudent = (List<Demo_test_table>)CMSHandler.FetchData<Demo_test_table>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDemoTestTable), sAcademicYear).DataSource.Data;
                string sStudentQuery = string.Empty;
                string sUserQuery = string.Empty;
                foreach (FileInfo file in Files)
                {
                    var sImage = file.Name.Split('.');
                    
                    if (liStudent != null && liStudent.Count > 0)
                    {
                        //int quality = 5;
                        _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.STUDENT_IMGAE_PATH + "\\" + "Convocation2021";
                        if (!Directory.Exists(_sDirectorypath))
                        {
                            Directory.CreateDirectory(_sDirectorypath);
                        }
                        IsExist = liStudent.Where(x=>x.TEXT1== sImage[0]).ToList();
                        if (IsExist!=null && IsExist.Count>0)
                        {
                            string s = file.Name.Replace(sImage[0], IsExist.FirstOrDefault().TEXT1);
                            liStudent.RemoveAll(x=>x.TEXT1== IsExist.FirstOrDefault().TEXT1);
                            string sFilePath = _sDirectorypath + "\\" + file.Name.Replace(sImage[0], IsExist.FirstOrDefault().TEXT1);
                            Bitmap b = new Bitmap(file.DirectoryName + "\\" + file.Name);
                            //Bitmap sResize = new Bitmap(b,new Size(132,170));
                            //sResize.Save(sFilePath);
                            b.Save(sFilePath);
                            COUNT++;
                        }
                        else
                        {
                            sUserQuery += sImage[0]+", ";
                        }
                    }
                }
                if (liStudent != null && liStudent.Count > 0)
                {

                }
            }

            
            ObjJsonData.Message = "(" + COUNT + ")" + "Request Completed Successfully :) !.";
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveStudentImage()
        {
            var ObjJsonData = new JsonResultData();
            var ObjStudent = new STU_PERSONAL_INFO();
            var UserAccount = new USER_ACCOUNT_INFO();
            string _sDirectorypath = string.Empty;
            int COUNT = 0;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                DirectoryInfo d = new DirectoryInfo(@"D:\2019");//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles(); //Getting Text files

                var IsExist = new List<ListStudents>();
                var liStudent = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(null,
                     SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByAcyear), sAcademicYear).DataSource.Data;
                string sStudentQuery = string.Empty;
                string sUserQuery = string.Empty;
                foreach (FileInfo file in Files)
                {
                    var sImage = file.Name.Split('.');
                    // Save Image By Roll No 
                    IsExist = liStudent.Where(o => o.ROLL_NO == sImage[0]).ToList();
                    if (IsExist.Count == 0)
                    {
                        // Save Image By Register No ....
                        IsExist = liStudent.Where(o => o.REGISTER_NO == sImage[0]).ToList();
                        if (IsExist.Count == 0)
                        {
                            // Save Image By Admission No For First Year .
                            IsExist = liStudent.Where(o => o.ADMISSION_NO == sImage[0]).ToList();
                        }
                    }


                    if (IsExist != null && IsExist.Count > 0)
                    {
                        _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.STUDENT_IMGAE_PATH + "\\" + IsExist.FirstOrDefault().DEPARTMENT;
                        if (!Directory.Exists(_sDirectorypath))
                        {
                            Directory.CreateDirectory(_sDirectorypath);
                        }
                        string sFilePath = _sDirectorypath + "\\" + file.Name.Replace(sImage[0], IsExist.FirstOrDefault().ROLL_NO);
                        System.Drawing.Bitmap b = new System.Drawing.Bitmap(file.DirectoryName + "\\" + file.Name);

                        b.Save(sFilePath);

                        // Save Personal Info Image .
                        ObjStudent.PHOTO = "\\" + Common.URLPages.STUDENT_IMGAE_PATH + "\\" + IsExist.FirstOrDefault().DEPARTMENT + "\\" + file.Name.Replace(sImage[0], IsExist.FirstOrDefault().ROLL_NO);
                        ObjStudent.STUDENT_ID = IsExist.FirstOrDefault().STUDENT_ID;
                        var UpdatePersonalInfo = CMSHandler.SaveOrUpdate(ObjStudent, SQL.Student.StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateStuPersonalImages), sAcademicYear);
                        sStudentQuery += "UPDATE STU_PERSONAL_INFO SET PHOTO ='" + ObjStudent.PHOTO + "' WHERE STUDENT_ID =" + ObjStudent.STUDENT_ID + ";";
                        // Save User Account .
                        UserAccount.USER_ID = IsExist.FirstOrDefault().STUDENT_ID;
                        UserAccount.PHOTO = "\\" + Common.URLPages.STUDENT_IMGAE_PATH + "\\" + IsExist.FirstOrDefault().DEPARTMENT + "\\" + file.Name.Replace(sImage[0], IsExist.FirstOrDefault().ROLL_NO);
                        UserAccount.ROLE = "5";
                        var UnSafeUpdateMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                        var sSQL = CMSHandler.SaveOrUpdate(UserAccount, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.UpdateUserAccountImage));
                        var SafeUpdateMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                        COUNT++;
                        sUserQuery += "UPDATE USER_ACCOUNT SET PHOTO='" + UserAccount.PHOTO + "' WHERE USER_ID=" + UserAccount.USER_ID + " AND ROLE=" + UserAccount.ROLE + ";";
                        ObjJsonData.sResult += "'" + IsExist.FirstOrDefault().ROLL_NO + "',\n";

                    }
                }
            }
            ObjJsonData.Message = "(" + COUNT + ")" + "Request Completed Successfully :) !.";
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public JsonResult SaveStaffImages()
        {
            var ObjJsonData = new JsonResultData();
            var ObjStaff = new STF_PERSONAL_INFO();
            var UserAccount = new USER_ACCOUNT_INFO();
            string _sDirectorypath = string.Empty;
            int COUNT = 0;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                DirectoryInfo d = new DirectoryInfo(@"D:\Teaching staff Photos");//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles(); //Getting Text files

                var IsExist = new List<STF_PERSONAL_INFO>();
                var listaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null,
                     SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff), sAcademicYear).DataSource.Data;

                var liTest = (List<Demo_test_table>)CMSHandler.FetchData<Demo_test_table>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDemoTestTable)).DataSource.Data;
                string sStudentQuery = string.Empty;
                string sUserQuery = string.Empty;
                var UnSafeUpdateMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                foreach (FileInfo file in Files)
                {
                    var sImage = file.Name.Split('.');
                    IsExist = listaff.Where(x => x.STAFF_CODE == sImage[0]).ToList();
                    //var lilist = liTest.Where(x => x.TEXT1 == sImage[0]).ToList();
                    //if (lilist != null && lilist.Count > 0)
                    //{
                    //    IsExist = listaff.Where(x => x.STAFF_CODE == lilist.FirstOrDefault().TEXT2).ToList();
                    //}
                    //else
                    //{
                    //    lilist = liTest.Where(x => x.TEXT2 == sImage[0]).ToList();
                    //    if (lilist != null && lilist.Count > 0)
                    //    {
                    //        IsExist = listaff.Where(x => x.STAFF_CODE == lilist.FirstOrDefault().TEXT2).ToList();
                    //    }
                    //}
                    if (IsExist != null && IsExist.Count > 0)
                    {
                        _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.STAFF_IMGAE_PATH + "\\" + IsExist.FirstOrDefault().DEPARTMENT;
                        if (!Directory.Exists(_sDirectorypath))
                        {
                            Directory.CreateDirectory(_sDirectorypath);
                        }
                        string sFilePath = _sDirectorypath + "\\" + file.Name.Replace(sImage[0], IsExist.FirstOrDefault().STAFF_CODE);
                        System.Drawing.Bitmap b = new System.Drawing.Bitmap(file.DirectoryName + "\\" + file.Name);

                        b.Save(sFilePath);

                        // Save Personal Info Image .
                        ObjStaff.PHOTO = "\\" + Common.URLPages.STAFF_IMGAE_PATH + "\\" + IsExist.FirstOrDefault().DEPARTMENT + "\\" + file.Name.Replace(sImage[0], IsExist.FirstOrDefault().STAFF_CODE);
                        ObjStaff.STAFF_ID = IsExist.FirstOrDefault().STAFF_ID;
                        // var UpdatePersonalInfo = CMSHandler.SaveOrUpdate(ObjStaff, SQL.StaffSQL.StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateStaffPhotos), sAcademicYear);
                        sStudentQuery += "UPDATE STF_PERSONAL_INFO SET PHOTO ='" + ObjStaff.PHOTO + "' WHERE STAFF_ID =" + ObjStaff.STAFF_ID + ";";
                        // Save User Account .
                        UserAccount.USER_ID = IsExist.FirstOrDefault().STAFF_ID;
                        UserAccount.PHOTO = "\\" + Common.URLPages.STAFF_IMGAE_PATH + "\\" + IsExist.FirstOrDefault().DEPARTMENT + "\\" + file.Name.Replace(sImage[0], IsExist.FirstOrDefault().STAFF_CODE);
                        UserAccount.ROLE = "5";
                        UserAccount.USER_ACCOUNT_ID = IsExist.FirstOrDefault().USER_ACCOUNT_ID;
                        // var sSQL = CMSHandler.SaveOrUpdate(UserAccount, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.UpdateUserAccountImage));
                        if (!string.IsNullOrEmpty(UserAccount.USER_ACCOUNT_ID))
                        {
                            COUNT++;
                            sUserQuery += "UPDATE USER_ACCOUNT SET PHOTO='" + UserAccount.PHOTO + "' WHERE USER_ACCOUNT_ID=" + UserAccount.USER_ACCOUNT_ID + ";";
                            ObjJsonData.sResult = "'" + IsExist.FirstOrDefault().STAFF_ID + "',\n";
                        }

                    }
                }
                using (ExaminationViewModel objExamination = new ExaminationViewModel())
                {

                    var resultArgs = objExamination.ExcuteCIA_Marks(sStudentQuery);

                    resultArgs = objExamination.ExcuteCIA_Marks(sUserQuery);
                }
                var SafeUpdateMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
            }
            ObjJsonData.Message = "(" + COUNT + ")" + "Request Completed Successfully :) !.";
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Sup CIA Component
        public ActionResult ListCIAComponent()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new SupportDataViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ObjViewModel.liSupCIAComponents = (List<SUP_CIA_COMPONENTS>)CMSHandler.FetchData<SUP_CIA_COMPONENTS>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCiaComponent)).DataSource.Data;
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        #endregion

        #region SUP_USER_TYPE

        public ActionResult FetchUserType()
        {

            string sAcademiYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            SupUserType objUserType = new SupUserType();
            List<DataPoint> dataPoints = new List<DataPoint>();
            List<DataPoint1> dataPoints1 = new List<DataPoint1>();
            List<DataLabel> dataLabel = new List<DataLabel>();
            List<DataY> dataY = new List<DataY>();
            string[] intArray ;
            string[] labelArray;
            if (!string.IsNullOrEmpty(sAcademiYear))
            {
                objUserType.liUserType = (List<SUP_USER_TYPE>)CMSHandler.FetchData<SUP_USER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupUserType), string.Empty).DataSource.Data;

                // this is to test chartJs
                dataPoints1 = (List<DataPoint1>)CMSHandler.FetchData<DataPoint1>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTransactionAmountForBarChart), string.Empty).DataSource.Data;
                intArray = new string[dataPoints1.Count];
                int i = 0;
                labelArray = new string[dataPoints1.Count];
                foreach (var item in dataPoints1)
                {
                    dataPoints.Add(new DataPoint() { y = Convert.ToDouble(item.y), label = item.label });
                    dataLabel.Add(new DataLabel() { label = item.label });
                    dataY.Add(new DataY() { y = Convert.ToDouble(item.y) });
                    intArray[i]= item.label;
                    labelArray[i] = item.y;
                    i++;

                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }


            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            ViewBag.DataLabel = JsonConvert.SerializeObject(intArray) ;
            ViewBag.DataY = JsonConvert.SerializeObject(labelArray) ;
            return View(objUserType);
        }


        #endregion        
        public ActionResult SupRoomList()
        {
            SupportDataViewModel ObjViewModel = new SupportDataViewModel();
            string sAcademiYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    var SupCollgeBlock = (List<SUP_COLLEGE_BLOCK>)CMSHandler.FetchData<SUP_COLLEGE_BLOCK>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupCollgeBlock), sAcademiYear).DataSource.Data;
                    var SupExamType = (List<SUP_EXAM_TYPE>)CMSHandler.FetchData<SUP_EXAM_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupExamType), sAcademiYear).DataSource.Data;
                    if (SupCollgeBlock != null && SupCollgeBlock.Count > 0)
                    {
                        ObjViewModel.CollgeBlock = new SelectList(SupCollgeBlock, Common.SUP_COLLEGE_BLOCK.COLLEGE_BLOCK_ID, Common.SUP_COLLEGE_BLOCK.BLOCK_NAME);
                    }
                    if (SupExamType != null && SupExamType.Count > 0)
                    {
                        ObjViewModel.ExamType = new SelectList(SupExamType, Common.SUP_EXAM_TYPE.EXAM_TYPE, Common.SUP_EXAM_TYPE.EXAM_TYPE_NAME);
                    }
                    ObjViewModel.liSupRoom = (List<SUP_ROOM>)CMSHandler.FetchData<SUP_ROOM>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRoomList), sAcademiYear).DataSource.Data;
                }
                catch (Exception Ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SupportDataController", "SupRoomList", "Err MSg: " + Ex.Message, "Query is empty!");
                        objHandler.WriteError("SupportDataController", "SupRoomList", Ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchRoomByRoomId(string sRoomId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liRooms = (List<SUP_ROOM>)CMSHandler.FetchData<SUP_ROOM>(new SUP_ROOM() { ROOM_ID = sRoomId },
                            SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchRoomByRoomId), sAcademicYear).DataSource.Data;
                    if (liRooms != null && liRooms.Count > 0)
                        return Json(liRooms, JsonRequestBehavior.AllowGet);
                    else
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDataController", "FetchRoomByRoomId", "Err MSg: " + ex.Message, "Query is empty!");
                        ObjLog.WriteError("SupportDataController", "FetchRoomByRoomId", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteSupRoom(string sRoomId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var sDeleteRoom = CMSHandler.SaveOrUpdate(new SUP_ROOM() { ROOM_ID = sRoomId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteSupRoom), sAcademicYear);
                    ObjJsonData.Message = (sDeleteRoom.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDataController", "DeleteSupRoom", "Err MSg: " + ex.Message, "Query is empty!");
                        ObjLog.WriteError("SupportDataController", "DeleteSupRoom", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveSupRoom(string sJsonRoom)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjModel = new EXAM_SEATINGS();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<SUP_ROOM>(sJsonRoom);          
                    if (!string.IsNullOrEmpty(Result.ROOM_ID))
                    {
                        //UpdateRooms 
                        var Rooms = (List<SUP_ROOM>)CMSHandler.FetchData<SUP_ROOM>(Result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchRoomByRoomId), sAcademicYear).DataSource.Data;
                        var UpdateRooms = CMSHandler.SaveOrUpdate(Result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateSupRoom), sAcademicYear);
                        if (UpdateRooms.Success)
                        {
                            //Update ExamSeatings                                                       
                            if (Rooms.FirstOrDefault().ROOM_NO != Result.ROOM_NO || Rooms.FirstOrDefault().ROW != Result.ROW || Rooms.FirstOrDefault().COLUMN != Result.COLUMN || Rooms.FirstOrDefault().EXAM_CAPACITY != Result.EXAM_CAPACITY)
                            {
                                var UnsafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                                var UpdateExamSeatings = CMSHandler.SaveOrUpdate(Result, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.UpdateIsDeletedInExamSeatingsByRoomId), sAcademicYear);
                                var SafeMode = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                                int k = 1;
                                for (int i = 1; i <= Convert.ToInt16(Result.COLUMN); i++)
                                {
                                    for (int j = 1; j <= Convert.ToInt16(Result.ROW); j++)
                                    {
                                        ObjModel.ROOM_ID = Result.ROOM_ID;
                                        ObjModel.COLUMN_NO = i.ToString();
                                        ObjModel.ROW_NO = j.ToString();
                                        ObjModel.SEAT_POSITION = string.Concat(i.ToString(), ",", j.ToString());
                                        ObjModel.SEAT_CODE = Rooms.FirstOrDefault().BLOCK_CODE + Common.Delimiter.IPHEN + Result.ROOM_ID + Common.Delimiter.IPHEN + k.ToString();
                                        ObjModel.EXAM_TYPE = Result.EXAM_TYPE;
                                        var UpdateSeatings = CMSHandler.SaveOrUpdate(ObjModel, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.InsertExamSeating), sAcademicYear);
                                        ObjJsonData.Message = (UpdateSeatings.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                        k++;
                                    }
                                }
                            }
                        }
                        ObjJsonData.Message = (UpdateRooms.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    }
                    else
                    {
                        //InsertRooms
                        var IsExist=(List<SUP_ROOM>)CMSHandler.FetchData<SUP_ROOM>(Result,
                            SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.CheckRoomAlreadyExist), sAcademicYear).DataSource.Data;
                        if (IsExist!=null && IsExist.Count()>0)
                        {
                            ObjJsonData.Message = Common.Messages.RecordsExistAlready;
                        }
                        else
                        {
                            var InsertRooms = CMSHandler.SaveOrUpdate(Result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.InsertSupRoom), sAcademicYear,true);
                            if (InsertRooms.Success)
                            {
                                var CollegeBlock = (List<SUP_ROOM>)CMSHandler.FetchData<SUP_ROOM>(Result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupCollgeBlockById), sAcademicYear).DataSource.Data;
                                int k = 1;
                                for (int i = 1; i <= Convert.ToInt16(Result.COLUMN); i++)
                                {
                                    for (int j = 1; j <= Convert.ToInt16(Result.ROW); j++)
                                    {
                                        ObjModel.ROOM_ID = InsertRooms.RowUniqueId.ToString();
                                        ObjModel.COLUMN_NO = i.ToString();
                                        ObjModel.ROW_NO = j.ToString();
                                        ObjModel.SEAT_POSITION = string.Concat(i.ToString(), Common.Delimiter.COMMA, j.ToString());
                                        ObjModel.SEAT_CODE = CollegeBlock.FirstOrDefault().BLOCK_CODE + Common.Delimiter.IPHEN + Result.ROOM_NO + Common.Delimiter.IPHEN + k.ToString();
                                        ObjModel.EXAM_TYPE = Result.EXAM_TYPE;
                                        var InsertSeatings = CMSHandler.SaveOrUpdate(ObjModel, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.InsertExamSeating), sAcademicYear);
                                        ObjJsonData.Message = (InsertSeatings.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                        k++;
                                    }
                                }
                            }
                            ObjJsonData.Message = (InsertRooms.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;                            
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDataController", "SaveSupRoom", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }


        #region Menu Items
        public ActionResult ListMenuItems()
        {
            SupportDataViewModel objViewModel = new SupportDataViewModel();
            string sAcademiYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    //fetch menu list items
                    objViewModel.liMenuItems = (List<MENU_ITEMS>)CMSHandler.FetchData<MENU_ITEMS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMenuItems), sAcademiYear).DataSource.Data;

                    //modules
                    var liModules= (List<MODULES>)CMSHandler.FetchData<MODULES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchModules), sAcademiYear).DataSource.Data;

                    if(liModules!=null && liModules.Count>0)
                    {
                        objViewModel.ModuleList = new SelectList(liModules, Common.MODULES.MODULES_ID, Common.MODULES.MODULE_NAME);
                    }
                }
                catch (Exception Ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SupportDataController", "ListMenuItems", "Err MSg: " + Ex.Message, "Query is empty!");
                        objHandler.WriteError("SupportDataController", "ListMenuItems", Ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(objViewModel);
        }

        public JsonResult FetchMenuItemById(string sMenuId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sMenuId))
                {
                    // fetch setting 
                    objResultData.oResult = (List<MENU_ITEMS>)CMS.DAO.CMSHandler.FetchData<MENU_ITEMS>(new MENU_ITEMS() { MENU_ID = sMenuId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMenuItemById), sAcademicYear).DataSource.Data;
                }
            }
            return Json(objResultData);
        }

        public JsonResult SaveorUpdateMenuItem(string sJsonData)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<MENU_ITEMS>(sJsonData);
                    if (result != null)
                    {
                      
                      
                        if (result.MENU_ID != null && result.MENU_ID != "")
                        {
                            // UPDATE menu item 
                            resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateMenuItemById), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                           
                            // ADD menu item 
                            resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SaveMenuItem), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDataController", "SaveorUpdateMenuItem", ex.Message);
                    }
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(objResultData);
        }
        public JsonResult DeleteMenuItemById(string sMenuId)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sMenuId))
                    {
                        // fetch setting 
                        resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new MENU_ITEMS() { MENU_ID = sMenuId, IS_DELETED = Common.DeleteFlag.Deleted }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteMenuItemById), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDataController", "DeleteMenuItemById", ex.Message);
                    }
                }

            }
            return Json(objResultData);
        }

        

        #endregion

        #region User Roles Rights

        public ActionResult ListUserRolesRights()
        {
            SupportDataViewModel objViewModel = new SupportDataViewModel();
            string sAcademiYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    //fetch menu list items
                    objViewModel.liUserRolesRights = (List<USER_ROLES_RIGHTS>)CMSHandler.FetchData<USER_ROLES_RIGHTS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchUserRolesRights), sAcademiYear).DataSource.Data;

                    //menu items
                    var liMenuItems = (List<MENU_ITEMS>)CMSHandler.FetchData<MENU_ITEMS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMenuItems), sAcademiYear).DataSource.Data;

                    if (liMenuItems != null && liMenuItems.Count > 0)
                    {
                        objViewModel.MenuItemList = new SelectList(liMenuItems, Common.MENU_ITEMS.MENU_ID, Common.MENU_ITEMS.MENU_NAME);
                    }
                    //sup role
                    var liSupRole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRole), sAcademiYear).DataSource.Data;

                    if (liSupRole != null && liSupRole.Count > 0)
                    {
                        objViewModel.RoleList = new SelectList(liSupRole, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);
                    }

                }
                catch (Exception Ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SupportDataController", "ListUserRolesRights", "Err MSg: " + Ex.Message, "Query is empty!");
                        objHandler.WriteError("SupportDataController", "ListUserRolesRights", Ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(objViewModel);
        }

        public JsonResult FetchUserRolesRightsById(string sUserRightsId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sUserRightsId))
                {
                    // fetch setting 
                    objResultData.oResult = (List<USER_ROLES_RIGHTS>)CMS.DAO.CMSHandler.FetchData<USER_ROLES_RIGHTS>(new USER_ROLES_RIGHTS() { USER_RIGHTS_ID = sUserRightsId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchUserRolesRightsById), sAcademicYear).DataSource.Data;
                }
            }
            return Json(objResultData);
        }

        public JsonResult SaveorUpdateUserRolesRights(string sJsonData)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<USER_ROLES_RIGHTS>(sJsonData);
                    if (result != null)
                    {


                        if (result.USER_RIGHTS_ID != null && result.USER_RIGHTS_ID != "")
                        {
                            // UPDATE user roles rights
                            resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateUserRolesRightsById), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            //check if records already exists or not
                            var isExist =(List<USER_ROLES_RIGHTS>) CMS.DAO.CMSHandler.FetchData<USER_ROLES_RIGHTS>(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchUserRolesRightsByMenuItemIdandRoleId), sAcademicYear).DataSource.Data;
                            if(isExist!=null && isExist.Count>0)
                            {
                                objResultData.Message = Common.Messages.RecordsExistAlready;
                                objResultData.ErrorCode = Common.ErrorCode.AlreadyReported;
                                return Json(objResultData);
                            }

                            // ADD user roles rights
                            resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SaveUserRolesRights), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDataController", "SaveorUpdateUserRolesRights", ex.Message);
                    }
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(objResultData);
        }
        public JsonResult DeleteUserRolesRightsById(string sUserRightsId)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sUserRightsId))
                    {
                        // fetch setting 
                        resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new USER_ROLES_RIGHTS() { USER_RIGHTS_ID = sUserRightsId, IS_DELETED = Common.DeleteFlag.Deleted }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteUserRolesRightsById), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDataController", "DeleteUserRolesRightsById", ex.Message);
                    }
                }

            }
            return Json(objResultData);
        }


        public JsonResult ActivateorDeactivateUserRolesRightsById(string sUserRightsId,string sMode)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sUserRightsId))
                    {
                        // fetch setting 
                        resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new USER_ROLES_RIGHTS() { USER_RIGHTS_ID = sUserRightsId, IS_ACTIVE = sMode }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.ActivateorDeactivateUserRolesRightsById), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDataController", "ActivateorDeactivateUserRolesRightsById", ex.Message);
                    }
                }

            }
            return Json(objResultData);
        }


        #endregion

        #region Fetch Staff By DepartmentId
        public JsonResult FetchStaffByDepartmentById(string sDepartmentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new stf_Personal_Info();
                ObjModel.DEPARTMENT_ID = sDepartmentId;
                var liStaff = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(ObjModel,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTeachingStaffOnlyByDepartment)).DataSource.Data;
                string sStaff = string.Empty;
                if (liStaff != null && liStaff.Count > 0)
                {
                    sStaff = "<option value='0'> -- Select -- </option> ";
                    foreach (var item in liStaff)
                    {
                        sStaff += "<option value=" + item.STAFF_ID + ">" + item.FIRST_NAME + "</option>";
                    }
                    sJsonData.sResult = sStaff;
                    sJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    sJsonData.Message = Common.Messages.NoRecordsfound;
                    sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(sJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchStaffByDepartmentByIdforReport(string sDepartmentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                string sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTeachingStaffOnlyByDepartment).Replace(Common.Delimiter.QUS + Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, sDepartmentId);
                var liStaff = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(null, sSQL).DataSource.Data;

                if (liStaff != null && liStaff.Count > 0)
                {
                    //sStaff = "<option value='0'> -- Select -- </option> ";
                    foreach (var item in liStaff)
                    {
                        sJsonData.sResult += "<option value=" + item.STAFF_ID + ">" + item.FIRST_NAME + "</option>";
                    }

                    sJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    sJsonData.Message = Common.Messages.NoRecordsfound;
                    sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(sJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Fetch Class By Staff Id
        public JsonResult FetchClassByStaffId(string sStaffId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new stf_Personal_Info();
                ObjModel.STAFF_ID = sStaffId;
                var liClass = (List<CourseWiseStaffInfo>)CMSHandler.FetchData<CourseWiseStaffInfo>(ObjModel,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassesByStaffId), sAcademicYear).DataSource.Data;
                string sStaff = string.Empty;
                if (liClass != null && liClass.Count > 0)
                {
                    sStaff = "<option value='0'> -- Select -- </option> ";
                    foreach (var item in liClass)
                    {
                        sStaff += "<option value=" + item.CLASS_ID + ">" + item.CLASS_NAME + "</option>";
                    }
                    sJsonData.sResult = sStaff;
                    sJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    sJsonData.Message = Common.Messages.NoRecordsfound;
                    sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(sJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Fetch Class By Staff Id & FeedBack Id
        public JsonResult FetchClassByStaffIdAndFeedBackId(string sStaffId,string sFeedBackId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new FBCLASS_WISE_STAFF();
                ObjModel.STAFF_ID = sStaffId;
                ObjModel.FEEDBACK_ID = sFeedBackId;
                var liClass = (List<CourseWiseStaffInfo>)CMSHandler.FetchData<CourseWiseStaffInfo>(ObjModel,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffClassByFeedBackSettingId), sAcademicYear).DataSource.Data;
                string sStaff = string.Empty;
                if (liClass != null && liClass.Count > 0)
                {
                    sStaff = "<option value='0'> -- Select -- </option> ";
                    foreach (var item in liClass)
                    {
                        sStaff += "<option value=" + item.CLASS_ID + ">" + item.CLASS_NAME + "</option>";
                    }
                    sJsonData.sResult = sStaff;
                    sJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    sJsonData.Message = Common.Messages.NoRecordsfound;
                    sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(sJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchClassByStaffIdAndFeedBackIdAndAcYear(string sStaffId, string sFeedBackId, string sAcademicYear)
        {
            //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            var sJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new FBCLASS_WISE_STAFF();
                ObjModel.STAFF_ID = sStaffId;
                ObjModel.FEEDBACK_ID = sFeedBackId;
                var liClass = (List<CourseWiseStaffInfo>)CMSHandler.FetchData<CourseWiseStaffInfo>(ObjModel,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffClassByFeedBackSettingId), sAcademicYear).DataSource.Data;
                string sStaff = string.Empty;
                if (liClass != null && liClass.Count > 0)
                {
                    sStaff = "<option value='0'> -- Select -- </option> ";
                    foreach (var item in liClass)
                    {
                        sStaff += "<option value=" + item.CLASS_ID + ">" + item.CLASS_NAME + "</option>";
                    }
                    sJsonData.sResult = sStaff;
                    sJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    sJsonData.Message = Common.Messages.NoRecordsfound;
                    sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(sJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Fetch Course By Class Id
        public JsonResult FetchCourseByClassId(string sClassId,string sStaffId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new stf_Personal_Info();
                ObjModel.CLASS_ID = sClassId;
                ObjModel.STAFF_ID = sStaffId;
                var liCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(ObjModel,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByStaffIdAndClassId), sAcademicYear).DataSource.Data;
                string sStaff = string.Empty;
                if (liCourse != null && liCourse.Count > 0)
                {
                    sStaff = "<option value='0'> -- Select -- </option> ";
                    foreach (var item in liCourse)
                    {
                        sStaff += "<option value=" + item.COURSE_ROOT_ID + ">" + item.COURSE_TITLE + "</option>";
                    }
                    sJsonData.sResult = sStaff;
                    sJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    sJsonData.Message = Common.Messages.NoRecordsfound;
                    sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(sJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Fetch Course By Class Id & FeedBack Id
        public JsonResult FetchCourseByClassIdAndFeedBackId(string sClassId, string sStaffId, string sFeedBackId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new FBCLASS_WISE_STAFF();
                ObjModel.CLASS_ID = sClassId;
                ObjModel.STAFF_ID = sStaffId;
                ObjModel.FEEDBACK_ID = sFeedBackId;


                var liacyear = (List<FBFEEDBACK_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(new FBFEEDBACK_SETTINGS() { FEEDBACK_ID = sFeedBackId },
                        SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackYearBySettingId)).DataSource.Data;

                var liCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(ObjModel,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffCourseByFeedBackSettingId), liacyear.FirstOrDefault().ACADEMIC_YEAR).DataSource.Data;
                string sStaff = string.Empty;
                if (liCourse != null && liCourse.Count > 0)
                {
                    sStaff = "<option value='0'> -- Select -- </option> ";
                    foreach (var item in liCourse)
                    {
                        sStaff += "<option value=" + item.COURSE_ROOT_ID + ">" + item.COURSE_TITLE + "</option>";
                    }
                    sJsonData.sResult = sStaff;
                    sJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    sJsonData.Message = Common.Messages.NoRecordsfound;
                    sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(sJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Fetch Course By Staff & AcYear & SemesterType Id
        public JsonResult FetchCourseByStaffIdAndSemesterTypeId(string sAcYear, string sSemesterTypeId, string sStaffId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new CourseWiseStaffList();
                    ObjModel.SEMESTER_TYPE_ID = sSemesterTypeId;
                    ObjModel.STAFF_ID = sStaffId;
                    var liCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByStaffIdAndSemesterTypeId), sAcYear).DataSource.Data;
                    if (liCourse != null && liCourse.Count > 0)
                    {
                        sJsonData.sResult = "<option value='0'> -- Select -- </option> ";
                        foreach (var item in liCourse)
                        {
                            sJsonData.sResult += "<option value=" + item.COURSE_ROOT_ID + ">" + item.COURSE_TITLE + "</option>";
                        }
                        sJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        sJsonData.Message = Common.Messages.NoRecordsfound;
                        sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog=new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDataController", "FetchCourseByStaffIdAndSemesterTypeId", ex.Message);
                    }
                    sJsonData.Message = Common.ErrorMessage.InternalServerError;
                    sJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            if (string.IsNullOrEmpty(sJsonData.ErrorCode))
            {
                sJsonData.Message = Common.Messages.NoRecordsfound;
                sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(sJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Fetch Class By Staff & AcYear & SemesterType Id
        public JsonResult FetchClassByStaffAndAcYearAndSemtypeId(string sStaffId,string sAcYear, string sSemesterTypeId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liClass = DapperHandler.FetchAll<CP_CLASSES>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByStaffAcyearAndSemestertypeId),new { STAFF_ID = sStaffId, SEMESTER_TYPE_ID = sSemesterTypeId },sAcYear).ToList();
                if (liClass != null && liClass.Count > 0)
                {
                    sJsonData.sResult = "<option value='0'> -- Select -- </option> ";
                    foreach (var item in liClass)
                    {
                        sJsonData.sResult += "<option value=" + item.CLASS_ID + ">" + item.CLASS_NAME + "</option>";
                    }
                    sJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    sJsonData.Message = Common.Messages.NoRecordsfound;
                    sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(sJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Fetch Course By Class & Staff & AcYear & SemesterType Id
        public JsonResult FetchCourseByClassIdStaffAndAcYearAndSemtypeId(string sClassId, string sStaffId, string sAcYear, string sSemesterTypeId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liCourse = DapperHandler.FetchAll<CP_COURSE_ROOT>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByClassIdStaffAcyearAndSemestertypeId), new { CLASS_ID=sClassId, STAFF_ID = sStaffId, SEMESTER_TYPE_ID = sSemesterTypeId }, sAcYear).ToList();
                if (liCourse != null && liCourse.Count > 0)
                {
                    sJsonData.sResult = "<option value='0'> -- Select -- </option> ";
                    foreach (var item in liCourse)
                    {
                        sJsonData.sResult += "<option value=" + item.COURSE_ROOT_ID + ">" + item.COURSE_TITLE + "</option>";
                    }
                    sJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    sJsonData.Message = Common.Messages.NoRecordsfound;
                    sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(sJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Fetch Class By Program Group Id
        public JsonResult FetchClassByProgramGroupId(string sProgramGroupId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sJsonData = new JsonResultData();
            string sClass = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                string sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByProgrammeGroupId).Replace(Common.Delimiter.QUS+Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, sProgramGroupId);
                var liClass = DapperHandler.FetchAll<CP_CLASSES>(sSQL, null).ToList();
                if (liClass != null && liClass.Count > 0)
                {
                    //sClass = "<option value='0'> -- Select -- </option> ";
                    foreach (var item in liClass)
                    {
                        sClass += "<option value=" + item.CLASS_ID + ">" + item.CLASS_NAME + "</option>";
                    }
                    sJsonData.sResult = sClass;
                    sJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    sJsonData.Message = Common.Messages.NoRecordsfound;
                    sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(sJsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Fetch Class By Shift Id
        public JsonResult FetchClassByShiftId(string sShiftId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new stf_Personal_Info();
                ObjModel.SHIFT = sShiftId;
                var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(ObjModel,SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByShiftId)).DataSource.Data;
                if (liClass != null && liClass.Count > 0)
                {
                    sJsonData.sResult = "<option value='0'> -- Select -- </option> ";
                    foreach (var item in liClass)
                    {
                        sJsonData.sResult += "<option value=" + item.CLASS_ID + ">" + item.CLASS_NAME + "</option>";
                    }
                    sJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    sJsonData.Message = Common.Messages.NoRecordsfound;
                    sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(sJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Fetch All Course By DepartmentId
        public JsonResult FetchCourseByDepartmentIdandYear(string sDepartmentId,string sAcyear)
        {
            //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcyear))
            {
                try
                {
                    var liCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new CP_COURSE_ROOT() { DEPARTMENT=sDepartmentId}, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllCoursesByDepartIdandAcyear), sAcyear).DataSource.Data;
                    if (liCourse!=null && liCourse.Count>0)
                    {
                        ObjJsonData.sResult += "<option> -- Select -- </option>";
                        foreach (var item in liCourse)
                        {
                            ObjJsonData.sResult += "<option value='" + item.COURSE_ROOT_ID + "'>" + item.COURSE_TITLE + "</option>";
                        }
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog=new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDataController", "FetchCourseByDepartmentId", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.GatewayTimeout;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData,JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchCourseByDepartmentId(string sDepartmentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new CP_COURSE_ROOT() { DEPARTMENT = sDepartmentId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllCoursesByDepartmnetId)).DataSource.Data;
                    if (liCourse != null && liCourse.Count > 0)
                    {
                        ObjJsonData.sResult += "<option> -- Select -- </option>";
                        foreach (var item in liCourse)
                        {
                            ObjJsonData.sResult += "<option value='" + item.COURSE_ROOT_ID + "'>" + item.COURSE_TITLE + "</option>";
                        }
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDataController", "FetchCourseByDepartmentId", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.GatewayTimeout;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Fetch Exam Setting By Exam Type
        public JsonResult FetchExamSettingByExamType(string sExamTypeId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new ExaminationViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(new EXAM_SETTING() { EXAM_TYPE = sExamTypeId },
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchExamSettingByExamTypeId), sAcademicYear).DataSource.Data;
                    if (liExamSetting != null && liExamSetting.Count > 0)
                    {
                        foreach (var item in liExamSetting)
                        {
                            ObjJsonData.sResult += "<option value='" + item.EXAM_SETTING_ID + "'>" + item.EXAM_NAME + "</option>";
                        }
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "FetchExamSettingByExamType", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public void GenerateBacode()
        {
            string path = string.Empty;
            var listudent = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllthestudent)).DataSource.Data;
            if(listudent!=null && listudent.Count>0)
            {
                foreach (var item in listudent)
                {
                    path = Server.MapPath("\\" + Common.FilePath.BARCODE_IMAGE_PATH) + "\\" +item.REGISTER_NO +".png";
                   // GeneratedBarcode MyBarCode = IronBarCode.BarcodeWriter.CreateBarcode(item.REGISTER_NO, BarcodeWriterEncoding.Code128);
                  //  MyBarCode.SaveAsPng(path);
                }
            }
            
        }
        #region Fetch Course By Class  & AcYear & SemesterType Id
        public JsonResult FetchCourseByClassIdAndAcYearAndSemtypeId(string sClassId, string sAcYear, string sSemesterTypeId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liCourse = DapperHandler.FetchAll<CP_COURSE_ROOT>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByClassIdAcyearAndSemestertypeId), new { CLASS_ID = sClassId, SEMESTER_TYPE_ID = sSemesterTypeId }, sAcYear).ToList();
                if (liCourse != null && liCourse.Count > 0)
                {
                    sJsonData.sResult = "<option value='0'> -- Select -- </option> ";
                    foreach (var item in liCourse)
                    {
                        sJsonData.sResult += "<option value=" + item.COURSE_ROOT_ID + ">" + item.COURSE_TITLE + "</option>";
                    }
                    sJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    sJsonData.Message = Common.Messages.NoRecordsfound;
                    sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
                
            }
            return Json(sJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchCourseByAcYearAndSemtypeId( string sAcYear, string sSemesterTypeId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liCourse = DapperHandler.FetchAll<CP_COURSE_ROOT>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByAcyearAndSemestertypeId), new {  SEMESTER_TYPE_ID = sSemesterTypeId }, sAcYear).ToList();
                if (liCourse != null && liCourse.Count > 0)
                {
                    sJsonData.sResult = "<option value='0'> -- Select -- </option> ";
                    foreach (var item in liCourse)
                    {
                        sJsonData.sResult += "<option value=" + item.COURSE_ROOT_ID + ">" + item.COURSE_TITLE + "</option>";
                    }
                    sJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    sJsonData.Message = Common.Messages.NoRecordsfound;
                    sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }

            }
            return Json(sJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchClassByAcYearAndSemtypeId(string sCourse, string sAcYear, string sSemesterTypeId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liCourse = DapperHandler.FetchAll<CP_COURSE_ROOT>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByAcyearAndSemestertypeId), new { COURSE_ROOT_ID= sCourse, SEMESTER_TYPE_ID = sSemesterTypeId }, sAcYear).ToList();
                if (liCourse != null && liCourse.Count > 0)
                {
                    sJsonData.sResult = "<option value='0'> -- Select -- </option> ";
                    foreach (var item in liCourse)
                    {
                        sJsonData.sResult += "<option value=" + item.CLASS_ID + ">" + item.CLASS_NAME + "</option>";
                    }
                    sJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    sJsonData.Message = Common.Messages.NoRecordsfound;
                    sJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }

            }
            return Json(sJsonData, JsonRequestBehavior.AllowGet);
        }



        #endregion
        #region Sup_attendance_type
        public ActionResult SupAttendanceType()
        {
            SupportDataViewModel viewmodal = new SupportDataViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                viewmodal.lisupAttendanceType = (List<SUP_ATTENDANCE_TYPE>)CMSHandler.FetchData<SUP_ATTENDANCE_TYPE>(null, AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchAttendance), sAcademicYear).DataSource.Data;

            }
            return View(viewmodal);
        }

        public JsonResult SaveSupAttendanceType(string Attendance)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = JsonConvert.DeserializeObject<SUP_ATTENDANCE_TYPE>(Attendance);

                    if (result.ATTENDANCE_TYPE_ID != "" && result.ATTENDANCE_TYPE_ID != null)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.UpdateAttendance), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {

                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.AddAttendance), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                    }
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SupportDate", "SaveSupAttendanceType", ex.ToString());
                    }
                }

            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(objResultData);
        }

        public JsonResult FetchAttendanceById(string AttendanceID)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchExamInfo = (List<SUP_ATTENDANCE_TYPE>)CMSHandler.FetchData<SUP_ATTENDANCE_TYPE>(new SUP_ATTENDANCE_TYPE() { ATTENDANCE_TYPE_ID = AttendanceID },
                          AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchAttendancbyId), sAcademicYear).DataSource.Data;
                    if (FetchExamInfo != null && FetchExamInfo.Count > 0)
                    {
                        return Json(FetchExamInfo, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDateController", "FetchAttendanceById", ex.Message);
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

        public JsonResult DeleteAttendance(string sAddendanceId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DeleteAttendance = CMSHandler.SaveOrUpdate(new SUP_ATTENDANCE_TYPE() { ATTENDANCE_TYPE_ID = sAddendanceId },
                        AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.DeleteAttendancById), sAcademicYear);
                    ObjJsonData.Message = (DeleteAttendance.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.ErrorCode = (DeleteAttendance.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDateController", "DeleteAttendance", ex.Message);
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
        #region Sup_Designation
        public ActionResult SupDesignation()
        {
            SupportDataViewModel viewmodal = new SupportDataViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                viewmodal.lisupDesignation = (List<Sup_Designation>)CMSHandler.FetchData<Sup_Designation>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDesignation), sAcademicYear).DataSource.Data;

            }
            return View(viewmodal);
        }


        public JsonResult SaveDesignation(string Designation)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = JsonConvert.DeserializeObject<Sup_Designation>(Designation);

                    if (result.DESIGNATIONID != "" && result.DESIGNATIONID != null)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateDesignation), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {


                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.AddDesignation), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                    }
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SupportDate", "SaveDesignation", ex.ToString());
                    }
                }

            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(objResultData);
        }

        public JsonResult FetchDesignationById(string DesignationID)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchExamInfo = (List<Sup_Designation>)CMSHandler.FetchData<Sup_Designation>(new Sup_Designation() { DESIGNATIONID = DesignationID },
                          SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDesignationbyId), sAcademicYear).DataSource.Data;
                    if (FetchExamInfo != null && FetchExamInfo.Count > 0)
                    {
                        return Json(FetchExamInfo, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDateController", "FetchDesignationById", ex.Message);
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


        public JsonResult DeleteDesignation(string DesignationID)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DeleteDesignation = CMSHandler.SaveOrUpdate(new Sup_Designation() { DESIGNATIONID = DesignationID ,ISDELETED=Common.DeleteFlag.Deleted},
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteDesignationById), sAcademicYear);
                    ObjJsonData.Message = (DeleteDesignation.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.ErrorCode = (DeleteDesignation.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDateController", "DeleteDesignation", ex.Message);
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

        #region Methodology

        public ActionResult SupMethodologyList()
        {
            SupportDataViewModel viewmodal = new SupportDataViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                viewmodal.liMethodology = (List<SUP_METHODOLOGY>)CMSHandler.FetchData<SUP_METHODOLOGY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMethodology), sAcademicYear).DataSource.Data;

            }
            return View(viewmodal);
        }
        public JsonResult SaveMethodology(string Methodology)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = JsonConvert.DeserializeObject<SUP_METHODOLOGY>(Methodology);

                    if (result.METHODOLOGY_ID != "" && result.METHODOLOGY_ID != null)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateMethodology), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {

                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.AddMethodology), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                    }
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SupportDate", "SaveMethodology", ex.ToString());
                    }
                }

            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(objResultData);
        }
        public JsonResult FetchMethodologybyId(string MethodologyID)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchMethodology = (List<SUP_METHODOLOGY>)CMSHandler.FetchData<SUP_METHODOLOGY>(new SUP_METHODOLOGY() { METHODOLOGY_ID = MethodologyID },
                          SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMethodologybyId), sAcademicYear).DataSource.Data;
                    if (FetchMethodology != null && FetchMethodology.Count > 0)
                    {
                        return Json(FetchMethodology, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDateController", "FetchMethodologybyId", ex.Message);
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
        public JsonResult DeleteMethodologyById(string MethodologyID)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DeleteMethodology = CMSHandler.SaveOrUpdate(new SUP_METHODOLOGY() { METHODOLOGY_ID = MethodologyID, IS_DELETED = Common.DeleteFlag.Deleted },
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteMethodologyById), sAcademicYear);
                    ObjJsonData.Message = (DeleteMethodology.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.ErrorCode = (DeleteMethodology.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDateController", "DeleteMethodologyById", ex.Message);
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


        #region Sup Shift   
        public ActionResult SupShift()
        {
            SupportDataViewModel viewmodal = new SupportDataViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                viewmodal.liShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupShift), sAcademicYear).DataSource.Data;

            }
            return View(viewmodal);
        }

        public JsonResult SaveShift(string Shift)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = JsonConvert.DeserializeObject<Sup_Shift>(Shift);
                    
                    if (result.SHIFT_ID != "" && result.SHIFT_ID != null)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateShift), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {

                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.AddShift), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                    }
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SupportDate", "SaveShift", ex.ToString());
                    }
                }

            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(objResultData);
        }
        public JsonResult FetchShiftbyId(string ShiftID)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchMethodology = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(new Sup_Shift() { SHIFT_ID = ShiftID },
                          SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShiftbyId), sAcademicYear).DataSource.Data;
                    if (FetchMethodology != null && FetchMethodology.Count > 0)
                    {
                        return Json(FetchMethodology, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDateController", "FetchShiftbyId", ex.Message);
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
        public JsonResult DeleteShiftById(string ShiftID)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {   
                try
                {
                    var DeleteShift = CMSHandler.SaveOrUpdate(new Sup_Shift() { SHIFT_ID = ShiftID, IS_DELETED = Common.DeleteFlag.Deleted },
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteShiftById), sAcademicYear);
                    ObjJsonData.Message = (DeleteShift.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.ErrorCode = (DeleteShift.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDateController", "DeleteShiftById", ex.Message);
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

        #region
        public ActionResult SupLmsUnit()
        {
            SupportDataViewModel viewmodal = new SupportDataViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                viewmodal.liLms = (List<SUP_LMS_UNIT>)CMSHandler.FetchData<SUP_LMS_UNIT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLmsUnit), sAcademicYear).DataSource.Data;

            }
            return View(viewmodal);
        }


        public JsonResult SaveLms(string Lms)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = JsonConvert.DeserializeObject<SUP_LMS_UNIT>(Lms);

                    if (result.LMS_UNIT_ID != "" && result.LMS_UNIT_ID != null)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateLms), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {

                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.AddLms), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                    }
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SupportDate", "SaveLms", ex.ToString());
                    }
                }

            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(objResultData);
        }

        public JsonResult FetchLmsbyId(string LmsID)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchLms = (List<SUP_LMS_UNIT>)CMSHandler.FetchData<SUP_LMS_UNIT>(new SUP_LMS_UNIT() { LMS_UNIT_ID = LmsID },
                          SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLmsbyId), sAcademicYear).DataSource.Data;
                    if (FetchLms != null && FetchLms.Count > 0)
                    {
                        return Json(FetchLms, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDateController", "FetchLmsbyId", ex.Message);
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

        public JsonResult DeleteLmsById(string LmsID)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DeleteLms = CMSHandler.SaveOrUpdate(new SUP_LMS_UNIT() { LMS_UNIT_ID = LmsID, IS_DELETED = Common.DeleteFlag.Deleted },
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteLmsById), sAcademicYear);
                    ObjJsonData.Message = (DeleteLms.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.ErrorCode = (DeleteLms.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDateController", "DeleteLmsById", ex.Message);
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


        #region Sup_non Teaching staff

        public ActionResult SupNonTeachingStaff()
        {
            SupportDataViewModel viewmodal = new SupportDataViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                viewmodal.liSupNonTeachingStaff = (List<SUP_NON_TEACHING_CATEGORY>)CMSHandler.FetchData<SUP_NON_TEACHING_CATEGORY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupNonTeachingStaff), sAcademicYear).DataSource.Data;

            }
            return View(viewmodal);
        }

        public JsonResult SaveNonTeachingStaff(string SupNonTeachingStaff)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = JsonConvert.DeserializeObject<SUP_NON_TEACHING_CATEGORY>(SupNonTeachingStaff);

                    if (result.NON_TEACHING_CATEGORY_ID != "" && result.NON_TEACHING_CATEGORY_ID != null)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateNonTeachingStaff), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {

                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.AddNonTeachingStaff), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                    }
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SupportDate", "SaveNonTeachingStaff", ex.ToString());
                    }
                }

            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(objResultData);
        }

        public JsonResult FetchNonTeachingStaffbyId(string NonTeachingStaffbyId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchNonTeachingStaff = (List<SUP_NON_TEACHING_CATEGORY>)CMSHandler.FetchData<SUP_NON_TEACHING_CATEGORY>(new SUP_NON_TEACHING_CATEGORY() { NON_TEACHING_CATEGORY_ID = NonTeachingStaffbyId },
                          SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchNonTeachingStaffbyId), sAcademicYear).DataSource.Data;
                    if (FetchNonTeachingStaff != null && FetchNonTeachingStaff.Count > 0)
                    {
                        return Json(FetchNonTeachingStaff, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDateController", "FetchNonTeachingStaffbyId", ex.Message);
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

        public JsonResult DeleteNonTeachingStaffById(string NonTeachingStaffById)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DeleteNonTeachingStaff = CMSHandler.SaveOrUpdate(new SUP_NON_TEACHING_CATEGORY() { NON_TEACHING_CATEGORY_ID = NonTeachingStaffById, IS_DELETE = Common.DeleteFlag.Deleted },
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteNonTeachingStaffById), sAcademicYear);
                    ObjJsonData.Message = (DeleteNonTeachingStaff.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.ErrorCode = (DeleteNonTeachingStaff.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SupportDateController", "DeleteNonTeachingStaffById", ex.Message);
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

        #region Semester Configuration
        public ActionResult ListSemesterConfiguration()
        {
            SupportDataViewModel objViewModel = new SupportDataViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null 
                ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() 
                : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null 
                    ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() 
                    : string.Empty);

            try
            {
                // 1. Fetch Semester Types
                objViewModel.liSemesterType = (List<SEMESTER_TYPE>)CMSHandler.FetchData<SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterTypeList), sAcademicYear).DataSource.Data;

                // 2. Fetch Support Semester Types
                objViewModel.liSupSemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupSemesterTypeList), sAcademicYear).DataSource.Data;
                if (objViewModel.liSupSemesterType != null && objViewModel.liSupSemesterType.Count > 0)
                {
                    var activeSupSemesterTypes = objViewModel.liSupSemesterType.Where(x => x.IS_ACTIVE == "1").ToList();
                    objViewModel.SupSemesterTypeList = new SelectList(activeSupSemesterTypes.Count > 0 ? activeSupSemesterTypes : objViewModel.liSupSemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                }

                // 3. Fetch Support Semesters (joined with Sup Semester Type)
                objViewModel.liSupSemester = (List<SUP_SEMESTER>)CMSHandler.FetchData<SUP_SEMESTER>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupSemesterList), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SupportDataController", "ListSemesterConfiguration", ex.Message);
                }
            }
            return View(objViewModel);
        }

        // =========================================================================
        // SEMESTER TYPE CRUD (Table: semester_type)
        // =========================================================================
        public JsonResult FetchSemesterTypeById(string sSemesterTypeId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sSemesterTypeId))
            {
                objResultData.oResult = (List<SEMESTER_TYPE>)CMSHandler.FetchData<SEMESTER_TYPE>(new SEMESTER_TYPE() { SEMESTER_TYPE_ID = sSemesterTypeId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterTypeById), sAcademicYear).DataSource.Data;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveorUpdateSemesterType(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            try
            {
                var result = JsonConvert.DeserializeObject<SEMESTER_TYPE>(sJsonData);
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(result.SEMESTER_TYPE_ID))
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateSemesterTypeById), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SaveSemesterType), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("SupportDataController", "SaveorUpdateSemesterType", ex.Message);
                }
                objResultData.Message = ex.Message;
                objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSemesterTypeById(string sSemesterTypeId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            try
            {
                if (!string.IsNullOrEmpty(sSemesterTypeId))
                {
                    resultArgs = CMSHandler.SaveOrUpdate(new SEMESTER_TYPE() { SEMESTER_TYPE_ID = sSemesterTypeId, IS_DELETED = Common.DeleteFlag.Deleted }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteSemesterTypeById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("SupportDataController", "DeleteSemesterTypeById", ex.Message);
                }
                objResultData.Message = ex.Message;
                objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ToggleSemesterTypeStatus(string sSemesterTypeId, string sMode)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            try
            {
                if (!string.IsNullOrEmpty(sSemesterTypeId))
                {
                    resultArgs = CMSHandler.SaveOrUpdate(new SEMESTER_TYPE() { SEMESTER_TYPE_ID = sSemesterTypeId, IS_ACTIVE = sMode }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.ToggleSemesterTypeStatusById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("SupportDataController", "ToggleSemesterTypeStatus", ex.Message);
                }
                objResultData.Message = ex.Message;
                objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        // =========================================================================
        // SUPPORT SEMESTER TYPE CRUD (Table: sup_semester_type)
        // =========================================================================
        public JsonResult FetchSupSemesterTypeById(string sSemesterTypeId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sSemesterTypeId))
            {
                objResultData.oResult = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(new SUP_SEMESTER_TYPE() { SEMESTER_TYPE_ID = sSemesterTypeId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupSemesterTypeById), sAcademicYear).DataSource.Data;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveorUpdateSupSemesterType(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            try
            {
                var result = JsonConvert.DeserializeObject<SUP_SEMESTER_TYPE>(sJsonData);
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(result.SEMESTER_TYPE_ID))
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateSupSemesterTypeById), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SaveSupSemesterType), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("SupportDataController", "SaveorUpdateSupSemesterType", ex.Message);
                }
                objResultData.Message = ex.Message;
                objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSupSemesterTypeById(string sSemesterTypeId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            try
            {
                if (!string.IsNullOrEmpty(sSemesterTypeId))
                {
                    resultArgs = CMSHandler.SaveOrUpdate(new SUP_SEMESTER_TYPE() { SEMESTER_TYPE_ID = sSemesterTypeId, IS_DELETED = Common.DeleteFlag.Deleted }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteSupSemesterTypeById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("SupportDataController", "DeleteSupSemesterTypeById", ex.Message);
                }
                objResultData.Message = ex.Message;
                objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ToggleSupSemesterTypeStatus(string sSemesterTypeId, string sMode)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            try
            {
                if (!string.IsNullOrEmpty(sSemesterTypeId))
                {
                    resultArgs = CMSHandler.SaveOrUpdate(new SUP_SEMESTER_TYPE() { SEMESTER_TYPE_ID = sSemesterTypeId, IS_ACTIVE = sMode }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.ToggleSupSemesterTypeStatusById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("SupportDataController", "ToggleSupSemesterTypeStatus", ex.Message);
                }
                objResultData.Message = ex.Message;
                objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        // =========================================================================
        // SUPPORT SEMESTER CRUD (Table: sup_semester)
        // =========================================================================
        public JsonResult FetchSupSemesterById(string sSupSemesterId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sSupSemesterId))
            {
                objResultData.oResult = (List<SUP_SEMESTER>)CMSHandler.FetchData<SUP_SEMESTER>(new SUP_SEMESTER() { SUP_SEMESTER_ID = sSupSemesterId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupSemesterById), sAcademicYear).DataSource.Data;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveorUpdateSupSemester(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            try
            {
                var result = JsonConvert.DeserializeObject<SUP_SEMESTER>(sJsonData);
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(result.SUP_SEMESTER_ID))
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateSupSemesterById), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SaveSupSemester), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("SupportDataController", "SaveorUpdateSupSemester", ex.Message);
                }
                objResultData.Message = ex.Message;
                objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSupSemesterById(string sSupSemesterId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            try
            {
                if (!string.IsNullOrEmpty(sSupSemesterId))
                {
                    resultArgs = CMSHandler.SaveOrUpdate(new SUP_SEMESTER() { SUP_SEMESTER_ID = sSupSemesterId, IS_DELETED = Common.DeleteFlag.Deleted }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteSupSemesterById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("SupportDataController", "DeleteSupSemesterById", ex.Message);
                }
                objResultData.Message = ex.Message;
                objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ToggleSupSemesterStatus(string sSupSemesterId, string sMode)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            try
            {
                if (!string.IsNullOrEmpty(sSupSemesterId))
                {
                    resultArgs = CMSHandler.SaveOrUpdate(new SUP_SEMESTER() { SUP_SEMESTER_ID = sSupSemesterId, IS_ACTIVE = sMode }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.ToggleSupSemesterStatusById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("SupportDataController", "ToggleSupSemesterStatus", ex.Message);
                }
                objResultData.Message = ex.Message;
                objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}