using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CMS.DAO;
using CMS.Services;
using CMS.SQL.Fee;
using CMS.SQL.PaymentGateway;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using RestSharp;
using ErrorLog = CMS.Utility.ErrorLog;
using CMS.SQL.SupportData;
using System.Globalization;
using static CMS.Utility.Common;
namespace CMS.Controllers
{
    public class EdvironPaymentController : Controller
    {
        DataValue dv = new DataValue();
        ResultArgs resultArgs = new ResultArgs();
        JsonResultData objJsonData = new JsonResultData();
        #region Create Order

        [HttpPost]
        public async Task<JsonResult> CreateOrder(string sFrequencyTypeId, string sFeeRootId, string sFrequencyId, string sAcademicYear, string sStudentId, string sPaymentMode, string sAmount, string sSelectionID = null)
        {
            try
            {

                //sAcademicYear = "2025";
                if (string.IsNullOrEmpty(sPaymentMode))
                {
                    return Json(new { status = false, Message = "Please select a payment mode." });
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                string sIsPGLive = System.Configuration.ConfigurationManager.AppSettings["ISPG_LIVE"] ?? "0";

                // 1. Fetch Edviron settings
                var settings = DapperHandler.FetchAll<FEE_EDVIRON_SETTING_INFO>(
                    EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchEdvironSettings),
                    new { IS_LIVE = sIsPGLive }).ToList();

                if (settings == null || settings.Count == 0)
                {
                    return Json(new { status = false, Message = "Edviron settings not configured. Please contact admin." });
                }

                var setting = settings.FirstOrDefault();

                // Check outstanding fee dues & permissions for Exam Fee
                if (sFrequencyTypeId == Common.FrequencyType.ExamFee)
                {
                    var dueSummary = FeeDueHelper.GetStudentFeeDueSummary(sStudentId, sAcademicYear);
                    if (dueSummary.HasDues && !FeeDueHelper.HasActiveExamFeePermission(sStudentId, null, sAcademicYear))
                    {
                        return Json(new { status = false, Message = "Exam fee payment is blocked due to outstanding fee dues (College/Hostel/Bus/Old Fee). Please clear your dues or contact the administration." });
                    }
                }

                // 2. Fetch student fee details (reuse existing fee SQL)
                List<FEE_STUDENT_ACCOUNT> studentAcountList = FetchStudentFeeAccount(sStudentId, sFrequencyTypeId, sFrequencyId, sAcademicYear);

                if (studentAcountList != null && studentAcountList.Count > 0 && !string.IsNullOrEmpty(sSelectionID))
                {
                    var selectedHeads = sSelectionID.Split(',').ToList();
                    studentAcountList = studentAcountList.Where(s => selectedHeads.Contains(s.HEAD_ID) || selectedHeads.Contains(s.HEAD)).ToList();
                }

                if (studentAcountList == null || studentAcountList.Count == 0 || studentAcountList.Sum(o => decimal.Parse(o.BALANCE)) <= 0)
                {
                    return Json(new { status = false, Message = "No pending fees found." });
                }

                decimal actualFeeAmount = 0;
                decimal dAmount = 0;
                if (!string.IsNullOrEmpty(sAmount) && decimal.TryParse(sAmount, out dAmount) && dAmount > 0)
                {
                    var orderedList = GetPrioritizedHeads(studentAcountList);

                    // 2. Distribute amount
                    decimal sbalance = dAmount;
                    List<FEE_STUDENT_ACCOUNT> distributedHeads = new List<FEE_STUDENT_ACCOUNT>();

                    foreach (var item in orderedList)
                    {
                        if (sbalance <= 0) break;
                        if (decimal.Parse(item.BALANCE) <= 0) continue;

                        decimal requiredAmount = decimal.Parse(item.BALANCE);
                        if (requiredAmount <= sbalance)
                        {
                            item.BALANCE = requiredAmount.ToString();
                            sbalance -= requiredAmount;
                        }
                        else
                        {
                            item.BALANCE = sbalance.ToString();
                            sbalance = 0;
                        }
                        distributedHeads.Add(item);
                    }
                    studentAcountList = distributedHeads;
                    actualFeeAmount = studentAcountList.Sum(x => decimal.Parse(x.BALANCE));
                }
                else
                {
                    actualFeeAmount = studentAcountList.Sum(x => decimal.Parse(x.BALANCE));
                }

                // Calculate payment charges by payment mode id
                var fetchedModes = DapperHandler.FetchAll<FEE_PAYMENT_MODE_CHARGES>(
                    PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchPaymentModes)).ToList();

                var matchedCharge = fetchedModes.FirstOrDefault(o =>
                    !string.IsNullOrEmpty(o.PAYMENT_MODE_ID) &&
                    o.PAYMENT_MODE_ID.Equals(sPaymentMode, StringComparison.OrdinalIgnoreCase) &&
                    decimal.Parse(string.IsNullOrEmpty(o.FROM_CHARGES_AMOUNT) ? "0" : o.FROM_CHARGES_AMOUNT) <= actualFeeAmount &&
                    decimal.Parse(string.IsNullOrEmpty(o.TO_CHARGES_AMOUNT) ? "999999999" : o.TO_CHARGES_AMOUNT) >= actualFeeAmount);

                decimal calculatedTotalAmount = actualFeeAmount;
                if (matchedCharge != null)
                {
                    decimal transactionFee = 0;
                    if (matchedCharge.IS_PERCENTAGE_OR_FLAT == "1") // Percentage
                    {
                        transactionFee = actualFeeAmount * decimal.Parse(matchedCharge.PERCENTAGE_OR_FLAT) / 100;
                    }
                    else // Flat
                    {
                        transactionFee = decimal.Parse(matchedCharge.PERCENTAGE_OR_FLAT);
                    }

                    decimal gstAmount = transactionFee * decimal.Parse(matchedCharge.GST_ON_TRANSACTION_FEE) / 100;
                    calculatedTotalAmount = actualFeeAmount + transactionFee + gstAmount;
                }

                decimal totalAmount = calculatedTotalAmount;

                // 3. Check for existing unpaid orders
                var existingOrders = DapperHandler.FetchAll<EDVIRON_ORDER_INFO>(
                    EdvironSQL.GetEdvironSQL(EdvironSQLCommands.IsOrderExisting),
                    new
                    {
                        STUDENT_ID = sStudentId,
                        FREQUENCY_ID = sFrequencyId,
                        ACADEMIC_YEAR = sAcademicYear
                    }).ToList();




                if (existingOrders != null && existingOrders.Count > 0)
                {
                    var existing = existingOrders.FirstOrDefault();
                    if ((existing.STATUS != null && existing.STATUS.ToUpper() == "SUCCESS") || (existing.STATUS != null && existing.STATUS.ToUpper() == "PAID"))
                    {
                        return Json(new { status = false, Message = "Payment already successful.", PaymentList = "" });
                    }

                    // Reuse existing URL if available and amount matches
                    if (!string.IsNullOrEmpty(existing.COLLECT_REQUEST_URL ))
                    {
                        decimal existingAmount = 0;
                        decimal.TryParse(existing.AMOUNT, out existingAmount);
                        decimal currentAmount = Convert.ToDecimal(sAmount);

                        if (Math.Abs(currentAmount - existingAmount) < 0.01m && existing.STATUS.ToUpper() == "PAID") 
                        {
                            return Json(new { status = true, PaymentList = existing.COLLECT_REQUEST_URL });
                        }
                    }

                    // Delete old record if amount changed
                    DapperHandler.SaveOrUpdate(
                        EdvironSQL.GetEdvironSQL(EdvironSQLCommands.DeleteOrderInfo),
                        new { CUSTOM_ORDER_ID = existing.CUSTOM_ORDER_ID });
                }

                // 4. Prepare new order
                string customOrderId = CommonMethods.GetUniqueCodeForPaymentGateway("EDV");

                // Fetch student info for name/email
                var studentInfo = DapperHandler.FetchAll<EDVIRON_ORDER_INFO>(
                    EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchStudentInfo),
                    new { STUDENT_ID = sStudentId }).ToList();

                string studentName = (studentInfo != null && studentInfo.Count > 0 && studentInfo[0].STUDENT_NAME != null) ? studentInfo[0].STUDENT_NAME : "Student";
                string studentEmail = (studentInfo != null && studentInfo.Count > 0 && studentInfo[0].STUDENT_EMAIL != null) ? studentInfo[0].STUDENT_EMAIL : "payment@college.edu.in";

                // Fetch vendor accounts for split payment
                var vendorAccounts = DapperHandler.FetchAll<EdvironVendorAccountModel>(
                    EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchVendorAccounts), null).ToList();
                // 4. Resolve payment mode for Edviron if specified
                string[] enabledModes = null;
                string[] disabledModes = null;
                bool isUpi = false;
                if (!string.IsNullOrEmpty(sPaymentMode) && sPaymentMode.ToUpper() != "EDVIRON")
                {
                    var selectedMode = fetchedModes.FirstOrDefault(m => m.PAYMENT_MODE_ID == sPaymentMode || m.PAYMENT_MODE_NAME.Equals(sPaymentMode, StringComparison.OrdinalIgnoreCase));
                    if (selectedMode != null)
                    {
                        string modeName = selectedMode.PAYMENT_MODE_NAME.ToUpper();
                        if (modeName.Contains("UPI"))
                        {
                            enabledModes = new[] { "UPI" };
                            isUpi = true;
                        }
                        else if (modeName.Contains("NET_BANKING") || modeName.Contains("NET BANKING") || modeName.Contains("NETBANKING") || modeName.Contains("NET") || modeName.Contains("BANK"))
                        {
                            enabledModes = new[] { "NET_BANKING" };
                        }
                        else if (modeName.Contains("CREDIT_CARD") || modeName.Contains("CREDIT CARD"))
                        {
                            enabledModes = new[] { "CREDIT_CARD" };
                        }
                        else if (modeName.Contains("DEBIT_CARD") || modeName.Contains("DEBIT CARD"))
                        {
                            enabledModes = new[] { "DEBIT_CARD" };
                        }
                        else if (modeName.Contains("CARD"))
                        {
                            enabledModes = new[] { "CARD" };
                        }
                        else if (modeName.Contains("WALLET"))
                        {
                            enabledModes = new[] { "WALLET" };
                        }
                        else if (modeName.Contains("PAY_LATER") || modeName.Contains("PAYLATER"))
                        {
                            enabledModes = new[] { "PAY_LATER" };
                        }
                        else if (modeName.Contains("CARDLESS"))
                        {
                            enabledModes = new[] { "CARDLESS" };
                        }
                        else if (modeName.Contains("NEFT") || modeName.Contains("RTGS") || modeName.Contains("IMPS"))
                        {
                            enabledModes = new[] { "NEFT_RTGS_IMPS" };
                        }

                        if (enabledModes != null && enabledModes.Length > 0)
                        {
                            var allAvailableModes = new List<string> { "UPI", "NET_BANKING", "CREDIT_CARD", "DEBIT_CARD", "CARD", "WALLET", "PAY_LATER", "CARDLESS", "NEFT_RTGS_IMPS" };
                            disabledModes = allAvailableModes.Where(m => !enabledModes.Contains(m)).ToArray();
                        }
                    }
                }
                var vendorsInfo = new List<EdvironVendorInfoModel>();
                if (vendorAccounts != null && vendorAccounts.Count > 0)
                {
                    var groupingByBank = studentAcountList
                        .Where(x => !string.IsNullOrEmpty(x.BANK_ACCOUNT_ID))
                        .GroupBy(x => x.BANK_ACCOUNT_ID)
                        .Select(g => new { BankAccountId = g.Key, Amount = g.Sum(x => decimal.Parse(x.BALANCE)) });

                    foreach (var group in groupingByBank)
                    {
                        var vendor = vendorAccounts.FirstOrDefault(v => v.BANK_ACCOUNT_ID == group.BankAccountId);
                        if (vendor != null && !string.IsNullOrEmpty(vendor.VENDOR_ID))
                        {
                            vendorsInfo.Add(new EdvironVendorInfoModel
                            {
                                vendor_id = vendor.VENDOR_ID,
                                amount = (double)group.Amount
                            });
                        }
                    }
                }


                // Save order to DB
                var orderToSave = new EDVIRON_ORDER_INFO
                {
                    SCHOOL_ID = setting.SCHOOL_ID,
                    AMOUNT = totalAmount.ToString("0.##"),
                    ACTUAL_AMOUNT = actualFeeAmount.ToString("0.##"),
                    CALLBACK_URL = "",
                    STUDENT_ID = sStudentId,
                    ACADEMIC_YEAR = sAcademicYear,
                    STUDENT_NAME = studentName,
                    STUDENT_EMAIL = studentEmail,
                    FREQUENCY_ID = sFrequencyId,
                    FREQUENCY_TYPE_ID = sFrequencyTypeId,
                    CUSTOM_ORDER_ID = customOrderId,
                    STATUS = "NEW",
                    SPLIT_PAYMENTS = vendorsInfo.Count > 0 ? JsonConvert.SerializeObject(vendorsInfo) : "",
                    INSTITUTE_CODE = sSelectionID ?? "",
                    PAYMENT_MODE_ID = sPaymentMode,
                    REQ_WEBHOOK_URLS = ""
                };

                DapperHandler.SaveOrUpdate(EdvironSQL.GetEdvironSQL(EdvironSQLCommands.SavePaymentOrderInfo), orderToSave);

                // 5. Generate JWT sign and call Edviron API
                string amountStr = totalAmount % 1 == 0 ? totalAmount.ToString("0") : totalAmount.ToString("0.##");
                string jwt = GenerateOrderJwt(amountStr, setting.PG_KEY, setting.RETURN_URL_SUCCESS, setting.SCHOOL_ID);

                var apiRequest = new EdvironOrderRequestModel
                {
                    school_id = setting.SCHOOL_ID,
                    amount = amountStr,
                    callback_url = setting.RETURN_URL_SUCCESS,
                    student_id = sStudentId,
                    student_name = studentName,
                    student_email = studentEmail,
                    additional_data = new { uid = sStudentId, acyear = sAcademicYear },
                    req_webhook_urls = !string.IsNullOrEmpty(setting.WEBHOOK_URL) ? new[] { setting.WEBHOOK_URL } : null,
                    receipt = "REC_" + customOrderId,
                    custom_order_id = customOrderId,
                    sign = jwt,
                    split_payments = vendorsInfo.Count > 0,
                    vendors_info = vendorsInfo.Count > 0 ? vendorsInfo : null,
                    enabled_modes = enabledModes,
                    disabled_modes = disabledModes
                };

                string url = setting.BASE_URL.TrimEnd('/') + "/create-collect-request/";
                string responseJson = await SendPostRequest(url, setting.SECRET_ID, apiRequest);

                var collectResponse = JsonConvert.DeserializeObject<EdvironCollectResponseModel>(responseJson);

                if (collectResponse != null && !string.IsNullOrEmpty(collectResponse.Collect_Request_Id))
                {
                    // Update order with collect request details
                    var orderUpdate = new EDVIRON_ORDER_INFO
                    {
                        CUSTOM_ORDER_ID = customOrderId,
                        COLLECT_REQUEST_ID = collectResponse.Collect_Request_Id,
                        COLLECT_REQUEST_URL = collectResponse.Collect_Request_Url,
                        SIGN = collectResponse.Sign ?? jwt,
                        STATUS = "CREATED",
                        RESPONSE_JSON = responseJson
                    };

                    DapperHandler.SaveOrUpdate(EdvironSQL.GetEdvironSQL(EdvironSQLCommands.UpdatePaymentOrderUrl), orderUpdate);

                    // Return payment link for AJAX to redirect
                    return Json(new { status = true, PaymentList = collectResponse.Collect_Request_Url });
                }

                return Json(new { status = false, Message = "Failed to generate payment link. Please try again." });
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {

                    objHandler.WriteError("EdvironPaymentController", "CreateOrder", ex.Message);
                }
                return Json(new { status = false, Message = "An error occurred while creating payment. Please try again." });
            }
        }

        #endregion

        #region Payment Callback

        /// <summary>
        /// Edviron redirects here after payment completion.
        /// Verifies the payment status and processes fee collection if PAID.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> PaymentCallback(string EdvironCollectRequestId, string status)
        {
            var viewModel = new EdvironPaymentResponseViewModel();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                string sIsPGLive = System.Configuration.ConfigurationManager.AppSettings["ISPG_LIVE"] ?? "0";

                // 1. Find the order
                EDVIRON_ORDER_INFO order = null;
                string sActionName = "AdmissionPaymentResponse";

                if (!string.IsNullOrEmpty(EdvironCollectRequestId))
                {
                    var orders = DapperHandler.FetchAll<EDVIRON_ORDER_INFO>(
                        EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchOrderByCollectId),
                        new { COLLECT_REQUEST_ID = EdvironCollectRequestId }).ToList();
                    order = (orders != null && orders.Count > 0) ? orders[0] : null;
                    sActionName = (order != null && order.FREQUENCY_TYPE_ID == "11") ? "ConvocationPaymentResponse" : "AdmissionPaymentResponse";
                }


                if (order == null)
                {
                    viewModel.Status = "FAILED";
                    viewModel.Message = "Order not found.";
                    return View("PaymentResponse", viewModel);
                }

                // Already processed
                if ((order.STATUS != null && order.STATUS.ToUpper() == "PAID") || (order.STATUS != null && order.STATUS.ToUpper() == "SUCCESS"))
                {
                    return RedirectToAction(sActionName, "PaymentGateway", new { sOrderId = order.CUSTOM_ORDER_ID });
                }

                // 2. Verify with Edviron API
                var settings = DapperHandler.FetchAll<FEE_EDVIRON_SETTING_INFO>(
                    EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchEdvironSettings),
                    new { IS_LIVE = sIsPGLive }).ToList();

                var setting = (settings != null && settings.Count > 0) ? settings[0] : null;
                if (setting == null)
                {
                    return RedirectToAction(sActionName, "PaymentGateway", new { sOrderId = order.CUSTOM_ORDER_ID });
                }

                string collectId = order.COLLECT_REQUEST_ID;
                string signJwt = GenerateStatusJwt(setting.PG_KEY, collectId, setting.SCHOOL_ID);
                string verifyUrl = setting.BASE_URL.TrimEnd('/') + "/collect-request/" + collectId + "?school_id=" + setting.SCHOOL_ID + "&sign=" + signJwt;

                var client = new RestSharp.RestClient(verifyUrl);
                var request = new RestRequest(verifyUrl, Method.Get);
                request.AddHeader("Authorization", "Bearer " + setting.SECRET_ID);

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var statusResponse = JsonConvert.DeserializeObject<EdvironCollectResponseModel>(response.Content);

                    if (statusResponse != null && statusResponse.status != null && statusResponse.status.ToUpper() == "SUCCESS")
                    {
                        // Update order status
                        DapperHandler.SaveOrUpdate(
                            EdvironSQL.GetEdvironSQL(EdvironSQLCommands.UpdateOrderStatus),
                            new { CUSTOM_ORDER_ID = order.CUSTOM_ORDER_ID, STATUS = "PAID" });

                        // Acquire processing lock to prevent duplicates
                        int lockRowsAffected = Convert.ToInt32(DapperHandler.SaveOrUpdate(
                            EdvironSQL.GetEdvironSQL(EdvironSQLCommands.AcquireProcessingLock),
                            new { CUSTOM_ORDER_ID = order.CUSTOM_ORDER_ID }));

                        if (lockRowsAffected > 0)
                        {
                            // Process fee collection
                            string receiptNo = ProcessFeeCollection(order);
                            viewModel.ReceiptNo = receiptNo;

                            // Fetch and save transaction details
                            await FetchAndSaveTransactionInfo(setting, collectId, signJwt, order);
                        }

                        return RedirectToAction(sActionName, "PaymentGateway", new { sOrderId = order.CUSTOM_ORDER_ID });
                    }
                    else
                    {
                        string apiStatus = (statusResponse != null && statusResponse.status != null) ? statusResponse.status : (status != null ? status : "UNKNOWN");
                        DapperHandler.SaveOrUpdate(
                            EdvironSQL.GetEdvironSQL(EdvironSQLCommands.UpdateOrderStatus),
                            new { CUSTOM_ORDER_ID = order.CUSTOM_ORDER_ID, STATUS = apiStatus });

                        viewModel.Status = "FAILED";
                        viewModel.Message = "Payment was not successful. Status: " + apiStatus;
                        viewModel.CollectRequestId = collectId;
                        viewModel.CustomOrderId = order.CUSTOM_ORDER_ID;
                        viewModel.Amount = order.AMOUNT;

                        return RedirectToAction(sActionName, "PaymentGateway", new { sOrderId = order.CUSTOM_ORDER_ID });
                    }
                }
                else
                {
                    viewModel.Status = "PENDING";
                    viewModel.Message = "Could not verify payment status. Please check later or contact admin.";
                    viewModel.CollectRequestId = collectId;
                    viewModel.CustomOrderId = order.CUSTOM_ORDER_ID;
                    return RedirectToAction(sActionName, "PaymentGateway", new { sOrderId = order.CUSTOM_ORDER_ID });
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EdvironPaymentController", "PaymentCallback", ex.Message);
                }
                viewModel.Status = "ERROR";
                viewModel.Message = "An error occurred while verifying payment.";

                string sErrorActionName = "AdmissionPaymentResponse";
                try
                {
                    var ordersCheck = DapperHandler.FetchAll<EDVIRON_ORDER_INFO>(
                        EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchOrderByCollectId),
                        new { COLLECT_REQUEST_ID = EdvironCollectRequestId }).ToList();
                    if (ordersCheck != null && ordersCheck.Count > 0 && ordersCheck[0].FREQUENCY_TYPE_ID == "11")
                    {
                        sErrorActionName = "ConvocationPaymentResponse";
                    }
                }
                catch {}
                
                return RedirectToAction(sErrorActionName, "PaymentGateway", new { sOrderId = EdvironCollectRequestId });
            }
        }

        #endregion

        #region Webhooks

        /// <summary>
        /// Receives transaction webhook from Edviron.
        /// This is the primary mechanism for processing successful payments.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> TransactionWebhook()
        {
            string rawJson = string.Empty;
            try
            {
                using (var reader = new StreamReader(Request.InputStream))
                {
                    rawJson = await reader.ReadToEndAsync();
                }

                // Save webhook history for audit
                SaveWebhookHistory("TRANSACTION", rawJson);

                var payload = JsonConvert.DeserializeObject<EdvironTransactionInfoModel>(rawJson);
                if (payload == null)
                    return Json(new { status = "FAIL", message = "Invalid payload" }, JsonRequestBehavior.AllowGet);

                // Resolve student_id from additional_data if missing
                if (string.IsNullOrEmpty(payload.student_id) && payload.additional_data != null)
                {
                    if (payload.additional_data.student_details != null)
                        payload.student_id = payload.additional_data.student_details.student_id;
                    else if (!string.IsNullOrEmpty(payload.additional_data.uid))
                        payload.student_id = payload.additional_data.uid;
                }

                // Find the order by collect_id
                EDVIRON_ORDER_INFO order = null;
                if (!string.IsNullOrEmpty(payload.collect_id))
                {
                    var orders = DapperHandler.FetchAll<EDVIRON_ORDER_INFO>(
                        EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchOrderByCollectId),
                        new { COLLECT_REQUEST_ID = payload.collect_id }).ToList();
                    order = (orders != null && orders.Count > 0) ? orders[0] : null;
                }

                if (order == null && !string.IsNullOrEmpty(payload.custom_order_id))
                {
                    var orders = DapperHandler.FetchAll<EDVIRON_ORDER_INFO>(
                        EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchOrderByCustomOrderId),
                        new { CUSTOM_ORDER_ID = payload.custom_order_id }).ToList();
                    order = (orders != null && orders.Count > 0) ? orders[0] : null;
                }

                if (order == null)
                    return Json(new { status = "FAIL", message = "Order not found" }, JsonRequestBehavior.AllowGet);

                // Save transfer info
                var transferInfo = new EDVIRON_TRANSFER_INFO_MODEL
                {
                    STATUS = payload.status,
                    TRANSACTION_AMOUNT = payload.transaction_amount,
                    CUSTOM_ORDER_ID = (payload.custom_order_id != null) ? payload.custom_order_id : order.CUSTOM_ORDER_ID,
                    COLLECT_REQUEST_ID = payload.collect_id,
                    SCHOOL_ID = payload.school_id,
                    STUDENT_ID = (payload.student_id != null) ? payload.student_id : order.STUDENT_ID,
                    BANK_REFERENCE = payload.bank_reference,
                    PAYMENT_METHOD = payload.payment_method,
                    CREATED_AT = payload.createdAt,
                    UPDATED_AT = (payload.UpdatedAt != null) ? payload.UpdatedAt : payload.transaction_time,
                    PAYMENT_DETAILS = payload.payment_details,
                    ACADEMIC_YEAR_ID = order.ACADEMIC_YEAR,
                    AMOUNT = (payload.amount != null) ? payload.amount : order.AMOUNT,
                    STUDENT_NAME = (payload.student_name != null) ? payload.student_name : order.STUDENT_NAME,
                    STUDENT_EMAIL = (payload.student_email != null) ? payload.student_email : order.STUDENT_EMAIL,
                    CURRENCY = "INR"
                };

                DapperHandler.SaveOrUpdate(EdvironSQL.GetEdvironSQL(EdvironSQLCommands.SaveTransferInfo), transferInfo);

                // Process fee collection if SUCCESS and not already PAID
                if ((payload.status != null && payload.status.ToUpper() == "SUCCESS") && (order.STATUS != null && order.STATUS.ToUpper() != "PAID"))
                {
                    DapperHandler.SaveOrUpdate(
                        EdvironSQL.GetEdvironSQL(EdvironSQLCommands.UpdateOrderStatus),
                        new { CUSTOM_ORDER_ID = order.CUSTOM_ORDER_ID, STATUS = "PAID" });

                    int lockRowsAffected = Convert.ToInt32(DapperHandler.SaveOrUpdate(
                        EdvironSQL.GetEdvironSQL(EdvironSQLCommands.AcquireProcessingLock),
                        new { CUSTOM_ORDER_ID = order.CUSTOM_ORDER_ID }));

                    if (lockRowsAffected > 0)
                    {
                        ProcessFeeCollection(order);
                    }
                }

                return Json(new { status = "SUCCESS" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EdvironPaymentController", "TransactionWebhook", ex.Message + " | " + rawJson);
                }
                return Json(new { status = "FAIL", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Receives settlement webhook from Edviron
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> SettlementWebhook()
        {
            string rawJson = string.Empty;
            try
            {
                using (var reader = new StreamReader(Request.InputStream))
                {
                    rawJson = await reader.ReadToEndAsync();
                }
                SaveWebhookHistory("SETTLEMENT", rawJson);
                return Json(new { status = "SUCCESS" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EdvironPaymentController", "SettlementWebhook", ex.Message + " | " + rawJson);
                }
                return Json(new { status = "FAIL" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Receives refund webhook from Edviron
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> RefundWebhook()
        {
            string rawJson = string.Empty;
            try
            {
                using (var reader = new StreamReader(Request.InputStream))
                {
                    rawJson = await reader.ReadToEndAsync();
                }
                SaveWebhookHistory("REFUND", rawJson);
                return Json(new { status = "SUCCESS" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EdvironPaymentController", "RefundWebhook", ex.Message + " | " + rawJson);
                }
                return Json(new { status = "FAIL" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Fee Collection Processing

        /// <summary>
        /// Processes fee collection after successful payment.
        /// Adapted from PaymentGatewayController.FeeCashFreeCollection.
        /// Returns receipt number on success.
        /// </summary>
        private string ProcessFeeCollection(EDVIRON_ORDER_INFO order)
        {

            string sSelectionId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            string receiptNo = string.Empty;
            try
            {
                string sStudentId = order.STUDENT_ID;
                string sFrequencyId = order.FREQUENCY_ID;
                string sFrequencyTypeId = order.FREQUENCY_TYPE_ID;
                string sAcademicYear = order.ACADEMIC_YEAR;
                string sAmount = order.ACTUAL_AMOUNT ?? order.AMOUNT;

                // Fetch student fee account
                List<FEE_STUDENT_ACCOUNT> studentAcountList = FetchStudentFeeAccount(sStudentId, sFrequencyTypeId, sFrequencyId, sAcademicYear);

                if (studentAcountList != null && studentAcountList.Count > 0 && !string.IsNullOrEmpty(order.INSTITUTE_CODE))
                {
                    var selectedHeads = order.INSTITUTE_CODE.Split(',').ToList();
                    studentAcountList = studentAcountList.Where(s => selectedHeads.Contains(s.HEAD_ID) || selectedHeads.Contains(s.HEAD)).ToList();
                }

                if (studentAcountList == null || studentAcountList.Count == 0) return receiptNo;

                // Also fetch existing student account for context
                string sSQL = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFrequencyIdNew)
                    .Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, sFrequencyId);
                var FetchFeeStudentAccount = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(
                    sSQL, new { STUDENT_ID = sStudentId }, sAcademicYear).ToList();

                // Filter heads with positive balance
                List<FEE_STUDENT_ACCOUNT> filteredHeads = new List<FEE_STUDENT_ACCOUNT>();
                foreach (var item in GetPrioritizedHeads(studentAcountList))
                {
                    if (!string.IsNullOrEmpty(item.BALANCE))
                    {
                        decimal balance = 0;
                        if (decimal.TryParse(item.BALANCE, out balance) && balance > 0)
                        {
                            filteredHeads.Add(item);
                        }
                    }
                }

                // Distribute amount across heads
                decimal dAmount = Convert.ToDecimal(!string.IsNullOrEmpty(order.ACTUAL_AMOUNT) ? order.ACTUAL_AMOUNT : order.AMOUNT);
                List<FEE_STUDENT_ACCOUNT> updatedHeads = new List<FEE_STUDENT_ACCOUNT>();
                foreach (var head in filteredHeads)
                {
                    decimal balance = decimal.Parse(head.BALANCE);
                    if (dAmount <= 0) break;

                    if (balance <= dAmount)
                    {
                        head.BALANCE = balance.ToString();
                        dAmount -= balance;
                    }
                    else
                    {
                        head.BALANCE = dAmount.ToString();
                        dAmount = 0;
                    }
                    updatedHeads.Add(head);
                }

                studentAcountList = updatedHeads;
                studentAcountList.RemoveAll(r => r.BALANCE == "0");

                if (studentAcountList.Count == 0) return receiptNo;

                // Group by frequency and process each
                var GroupFeeStudentAccount = studentAcountList.GroupBy(x => x.FREQUENCY_ID);

                foreach (var FrequencyItem in GroupFeeStudentAccount)
                {
                    sFrequencyId = FrequencyItem.FirstOrDefault().FREQUENCY_ID;
                    var feeTransactions = FrequencyItem.ToList();
                    decimal balance = feeTransactions.Sum(x => decimal.Parse(x.BALANCE));

                    if (balance > 0)
                    {
                        var objFeeTransactionModel = new FEE_TRANSACTION
                        {
                            STUDENT_ID = sStudentId,
                            CLASS = string.Empty,
                            FREQUENCY = sFrequencyId,
                            FREQUENCY_TO = sFrequencyId,
                            ACADEMIC_YEAR = sAcademicYear,
                            PAYMENT_DATE = DateTime.Today.ToString("yyyy-MM-dd"),
                            USERNAME = sStudentId,
                            COLLECTED = balance.ToString(),
                            CF_ORDER_ID = order.CUSTOM_ORDER_ID,
                            PAYMENT_MODE = Common.PaymentMode.Online
                        };

                        // Generate receipt number
                        var FetchReceiptNo = DapperHandler.FetchAll<AUTO_GENERATE_NUMBERS>(
                            FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAutoGenerateNumber),
                            new { STUDENT_ID = sStudentId }, string.Empty).ToList();

                        if (FetchReceiptNo != null && FetchReceiptNo.Count > 0)
                        {
                            int iReceiptNo = Convert.ToInt32(FetchReceiptNo.FirstOrDefault().exam_receipt_no);
                            AUTO_GENERATE_NUMBERS objAutoGenerateNumber = new AUTO_GENERATE_NUMBERS
                            {
                                exam_receipt_no = (iReceiptNo + 1).ToString(),
                                auto_generate_numbers_id = "1"
                            };
                            objFeeTransactionModel.RECEIPT_NO = "00" + objAutoGenerateNumber.exam_receipt_no;
                            receiptNo = objFeeTransactionModel.RECEIPT_NO;

                            var UpdateReceiptNo = CMSHandler.SaveOrUpdate(
                                objAutoGenerateNumber,
                                FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateAutoGenerateNumber)
                                    .Replace("?exam_receipt_no", objAutoGenerateNumber.exam_receipt_no)
                                    .Replace("?auto_generate_numbers_id", objAutoGenerateNumber.auto_generate_numbers_id),
                                string.Empty, false);

                            if (UpdateReceiptNo.Success)
                            {
                                //var SaveFeeTransaction = CMSHandler.SaveOrUpdate(
                                //    objFeeTransactionModel,
                                //    FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeTransaction),
                                //    sAcademicYear, true);


                               string sQuery = FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeTransaction)
    .Replace("?STUDENT_ID", objFeeTransactionModel.STUDENT_ID)
    .Replace("?FREQUENCY", objFeeTransactionModel.FREQUENCY)
    //.Replace("?CLASS", objFeeTransactionModel.CLASS)
.Replace("?PAYMENT_DATE", "'" + objFeeTransactionModel.PAYMENT_DATE + "'")
.Replace("?RECEIPT_NO", objFeeTransactionModel.RECEIPT_NO)
    .Replace("?PAYMENT_MODE", objFeeTransactionModel.PAYMENT_MODE)
    .Replace("?COLLECTED", objFeeTransactionModel.COLLECTED)
    .Replace("?USERNAME", objFeeTransactionModel.USERNAME)
    .Replace("?AC_YEAR", sAcademicYear)
    .Replace("?FREQUENCY", objFeeTransactionModel.FREQUENCY_TO)
    //.Replace("?PAYURESPONSE_ID", objFeeTransactionModel.PAYURESPONSE_ID)
    .Replace("?RAZORPAY_ID", objFeeTransactionModel.RAZORPAY_ID)
    .Replace("?CF_ORDER_ID", objFeeTransactionModel.CF_ORDER_ID);

                                var SaveFeeTransaction = CMSHandler.SaveOrUpdate(
                                    objFeeTransactionModel,
                                    sQuery,
                                    sAcademicYear,
                                    true
                                );




                                if (SaveFeeTransaction.Success)
                                {
                                    var FetchTransaction = DapperHandler.FetchAll<FEE_TRANSACTION>(
                                        FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTransactionByCashFreeId),
                                        new { CF_ORDER_ID = order.CUSTOM_ORDER_ID, FREQUENCY = sFrequencyId }, sAcademicYear).ToList();

                                    string TransactionId = (FetchTransaction != null && FetchTransaction.Count > 0)
                                        ? FetchTransaction.FirstOrDefault().TRANSACTION_ID
                                        : SaveFeeTransaction.RowUniqueId.ToString();

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
                                        item.RAZORPAY_ID = objFeeTransactionModel.CF_ORDER_ID;
                                        item.STUDENT_ID = sStudentId;

                                        //var SaveFeeCollection = CMSHandler.SaveOrUpdate(
                                        //    item, FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeCollection), string.Empty, false);


                                        string query = FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeCollection)

                                        .Replace("?PAID_AMOUNT", item.BALANCE.ToString())
                                        .Replace("?HEAD", item.HEAD)
                                        .Replace("?FEE_MAIN_HEAD_ID", item.FEE_MAIN_HEAD_ID.ToString())
                                        .Replace("?TRANSACTION_ID", TransactionId)
                                        .Replace("?RECEIPT_NO", objFeeTransactionModel.RECEIPT_NO)
                                        .Replace("?FREQUENCY", sFrequencyId.ToString())
                                        .Replace("?STUDENT_ID", sStudentId.ToString());

                                        var SaveFeeCollection = CMSHandler.SaveOrUpdate(
                                            item,
                                            query,
                                            string.Empty,
                                            false
                                        );


                                        if (SaveFeeCollection.Success)
                                        {
                                            if (sFrequencyTypeId == "12")
                                            {
                                                var result1 = DapperHandler.SaveOrUpdate(
    FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeStudentAccount),
    item,
    sAcademicYear
);

                                                var result2 = DapperHandler.SaveOrUpdate(
    FeeSQL.GetFeeSQL(FeeSQLCommands.updatepaidstatusforaarearcourse),
    item,
    sAcademicYear
);

                                                //                                                var result2 = DapperHandler.SaveOrUpdate(
                                                //    FeeSQL.GetFeeSQL(FeeSQLCommands.updateissuedapplications),
                                                //    item,
                                                //    sAcademicYear
                                                //);


                                                //DapperHandler.SaveOrUpdate(FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeStudentAccount), item, sAcademicYear);
                                                //DapperHandler.SaveOrUpdate(FeeSQL.GetFeeSQL(FeeSQLCommands.updateissuedapplications), item, sAcademicYear);

                                            }
                                            else
                                            {
                                                var result1 = DapperHandler.SaveOrUpdate(
                                                    FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeStudentAccount),
                                                    item,
                                                    sAcademicYear
                                                );

                                            }
                                        }
                                    }
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
                    objHandler.WriteError("EdvironPaymentController", "ProcessFeeCollection", ex.Message);
                }
            }

            return receiptNo;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Fetches student fee account based on frequency type.
        /// Reuses existing FeeSQL queries (same as PaymentGatewayController).
        /// </summary>
        private List<FEE_STUDENT_ACCOUNT> FetchStudentFeeAccount(string sStudentId, string sFrequencyTypeId, string sFrequencyId, string sAcademicYear)
        {
            List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
            try
            {
                if (sFrequencyTypeId == Common.FrequencyType.SemesterFee)
                    studentAcountList = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), new { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.SemesterFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, sAcademicYear).ToList();
                else if (sFrequencyTypeId == Common.FrequencyType.AdmissionFee)
                    studentAcountList = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), new { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.AdmissionFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, sAcademicYear).ToList();
                else if (sFrequencyTypeId == Common.FrequencyType.AdmissionApplicationFee)
                    studentAcountList = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), new { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.AdmissionApplicationFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, sAcademicYear).ToList();
                else if (sFrequencyTypeId == Common.FrequencyType.Arrerfee)
                    studentAcountList = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(FeeSQL.GetFeeSQL(FeeSQLCommands.FetchArrearStudentAccountFeeInfo), new { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.Arrerfee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, sAcademicYear).ToList();
                    //studentAcountList = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeInfo), new { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.Arrerfee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, sAcademicYear).ToList();
                else if (sFrequencyTypeId == Common.FrequencyType.HostelFee)
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FEE_ROOT_ID = Common.FeeRoot.HostelFee, FREQUENCY_TYPE_ID = sFrequencyTypeId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelFeeStudentAccountInfoWithoutHead), sAcademicYear).DataSource.Data;
                    //studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId, FREQUENCY_ID = sFrequencyId, FEE_ROOT_ID = Common.FeeRoot.HostelFee, FREQUENCY_TYPE_ID = sFrequencyTypeId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelFeeStudentAccountInfo), sAcademicYear).DataSource.Data;
                else if (sFrequencyTypeId == Common.FrequencyType.ConvocationFee)
                    studentAcountList = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(FeeSQL.GetFeeSQL(FeeSQLCommands.FetchStudentAccountFeeDueInfo), new { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.ConvocationFee, FEE_ROOT_ID = Common.FeeRoot.ConvocationFee, FREQUENCY_ID = sFrequencyId }, sAcademicYear).ToList();
                else if (sFrequencyTypeId == Common.FrequencyType.BusFee)
                    studentAcountList = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(FeeSQL.GetFeeSQL(FeeSQLCommands.FetchBusStudentAccountFeeInfo), new { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.BusFee, FEE_ROOT_ID = Common.FeeRoot.BusFee, FREQUENCY_ID = sFrequencyId }, sAcademicYear).ToList();
                else if (sFrequencyTypeId == Common.FrequencyType.ExamFee)
                {
                    studentAcountList = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(FeeSQL.GetFeeSQL(FeeSQLCommands.FetchExamStudentAccountFeeInfo), new { STUDENT_ID = sStudentId, FREQUENCY_TYPE_ID = Common.FrequencyType.ExamFee, FEE_ROOT_ID = Common.FeeRoot.CollegeFee, FREQUENCY_ID = sFrequencyId }, sAcademicYear).ToList();
                    if (studentAcountList == null || studentAcountList.Count == 0)
                    {
                        var res = CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAllFeeDuesByStudentId), sAcademicYear);
                        if (res != null && res.DataSource != null && res.DataSource.Data != null)
                        {
                            var allList = (List<FEE_STUDENT_ACCOUNT>)res.DataSource.Data;
                            studentAcountList = allList.Where(x => x.FREQUENCY_TYPE_ID == sFrequencyTypeId && (string.IsNullOrEmpty(sFrequencyId) || x.FREQUENCY_ID == sFrequencyId)).ToList();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EdvironPaymentController", "FetchStudentFeeAccount", ex.Message);
                }
            }
            return studentAcountList;
        }

        /// <summary>
        /// Generate JWT for create order (amount, callback_url, school_id claims).
        /// Uses manual HMAC-SHA256 JWT generation (no System.IdentityModel.Tokens.Jwt needed).
        /// </summary>
        private string GenerateOrderJwt(string amount, string pgKey, string callbackUrl, string schoolId)
        {
            var payload = new Dictionary<string, object>
            {
                { "callback_url", callbackUrl },
                { "school_id", schoolId },
                { "amount", amount }
            };
            return GenerateHmacSha256Jwt(payload, pgKey);
        }

        /// <summary>
        /// Generate JWT for status check (school_id, collect_request_id claims).
        /// </summary>
        private string GenerateStatusJwt(string pgKey, string collectId, string schoolId)
        {
            var payload = new Dictionary<string, object>
            {
                { "school_id", schoolId },
                { "collect_request_id", collectId }
            };
            return GenerateHmacSha256Jwt(payload, pgKey);
        }

        /// <summary>
        /// Manual JWT generation using HMAC-SHA256. 
        /// Produces a valid JWT token without System.IdentityModel.Tokens.Jwt dependency.
        /// </summary>
        private string GenerateHmacSha256Jwt(Dictionary<string, object> payload, string secret)
        {
            // Header
            var header = new { alg = "HS256", typ = "JWT" };
            string headerJson = JsonConvert.SerializeObject(header);
            string headerBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));

            // Payload
            string payloadJson = JsonConvert.SerializeObject(payload);
            string payloadBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

            // Signature
            string toSign = headerBase64 + "." + payloadBase64;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                byte[] signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(toSign));
                string signatureBase64 = Base64UrlEncode(signatureBytes);
                return toSign + "." + signatureBase64;
            }
        }

        /// <summary>
        /// Base64Url encode (JWT-compliant)
        /// </summary>
        private string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        /// <summary>
        /// Send POST request to Edviron API using RestSharp
        /// </summary>
        private async Task<string> SendPostRequest(string url, string secret, object payload)
        {
            var client = new RestSharp.RestClient(url);
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("Authorization", "Bearer " + secret);
            request.AddJsonBody(payload);

            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EdvironPaymentController", "SendPostRequest",
                        "URL: " + url + ", Status: " + response.StatusCode + ", Response: " + response.Content);
                }
                throw new Exception("Edviron API request failed: " + response.StatusCode + " - " + response.Content);
            }
            return response.Content ?? "";
        }

        /// <summary>
        /// Fetch and save detailed transaction info from Edviron API
        /// </summary>
        private async Task FetchAndSaveTransactionInfo(FEE_EDVIRON_SETTING_INFO settings, string collectId, string sign, EDVIRON_ORDER_INFO order)
        {
            try
            {
                string url = settings.BASE_URL.TrimEnd('/') + "/transaction-info?collect_request_id=" + collectId + "&sign=" + sign + "&school_id=" + settings.SCHOOL_ID;

                var client = new RestSharp.RestClient(url);
                var request = new RestRequest(url, Method.Get);
                request.AddHeader("Authorization", "Bearer " + settings.SECRET_ID);

                var response = await client.ExecuteAsync(request);
                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var details = JsonConvert.DeserializeObject<EdvironTransactionInfoModel>(response.Content);
                    if (details != null)
                    {
                        var transfer = new EDVIRON_TRANSFER_INFO_MODEL
                        {
                            STATUS = details.status,
                            TRANSACTION_AMOUNT = details.transaction_amount,
                            CUSTOM_ORDER_ID = details.custom_order_id ?? order.CUSTOM_ORDER_ID,
                            COLLECT_REQUEST_ID = details.collect_id ?? collectId,
                            SCHOOL_ID = details.school_id,
                            STUDENT_ID = details.student_id ?? order.STUDENT_ID,
                            BANK_REFERENCE = details.bank_reference,
                            PAYMENT_METHOD = details.payment_method,
                            CREATED_AT = details.createdAt,
                            UPDATED_AT = details.UpdatedAt ?? details.transaction_time,
                            PAYMENT_DETAILS = details.payment_details,
                            ACADEMIC_YEAR_ID = order.ACADEMIC_YEAR,
                            AMOUNT = details.amount ?? order.AMOUNT,
                            STUDENT_NAME = details.student_name ?? order.STUDENT_NAME,
                            STUDENT_EMAIL = details.student_email ?? order.STUDENT_EMAIL,
                            CURRENCY = "INR"
                        };

                        DapperHandler.SaveOrUpdate(EdvironSQL.GetEdvironSQL(EdvironSQLCommands.SaveTransferInfo), transfer);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EdvironPaymentController", "FetchAndSaveTransactionInfo", ex.Message);
                }
            }
        }

        /// <summary>
        /// Save webhook raw payload for audit
        /// </summary>
        private void SaveWebhookHistory(string webhookType, string rawPayload)
        {
            try
            {
                // Extract collect_request_id and custom_order_id from payload for indexing
                string collectId = "";
                string customOrderId = "";
                string status = "";
                try
                {
                    dynamic parsed = JsonConvert.DeserializeObject(rawPayload);
                    collectId = (parsed != null && parsed.collect_id != null) ? parsed.collect_id.ToString() : "";
                    customOrderId = (parsed != null && parsed.custom_order_id != null) ? parsed.custom_order_id.ToString() : "";
                    status = (parsed != null && parsed.status != null) ? parsed.status.ToString() : "";
                }
                catch { }

                var history = new EDVIRON_WEBHOOK_HISTORY
                {
                    WEBHOOK_TYPE = webhookType,
                    RAW_PAYLOAD = rawPayload,
                    COLLECT_REQUEST_ID = collectId,
                    CUSTOM_ORDER_ID = customOrderId,
                    STATUS = status
                };

                DapperHandler.SaveOrUpdate(EdvironSQL.GetEdvironSQL(EdvironSQLCommands.SaveWebhookHistory), history);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EdvironPaymentController", "SaveWebhookHistory", ex.Message);
                }
            }
        }

        /// <summary>
        /// Fetches tail heads (fines/penalties) dynamically from the database.
        /// </summary>
        private List<int> GetTailHeadIds()
        {
            List<int> lastHeads = new List<int> { 5, 17, 18, 19, 20, 21 }; // Default fallback
            try
            {
                var dynamicHeads = DapperHandler.FetchAll<int>(FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDynamicTailHeads), null).ToList();
                if (dynamicHeads != null && dynamicHeads.Count > 0)
                {
                    lastHeads = dynamicHeads;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EdvironPaymentController", "GetTailHeadIds", ex.Message);
                }
            }
            return lastHeads;
        }

        #region Staff Payment Integration

        private string GetEdvironSQLForStaff(EdvironSQLCommands cmd)
        {
            string sql = EdvironSQL.GetEdvironSQL(cmd);
            sql = sql.Replace("FEE_EDVIRON_ORDER_INFO", "staff_fee_edviron_order_info")
                     .Replace("FEE_EDVIRON_TRANSFER_INFO", "staff_fee_edviron_transfer_info")
                     .Replace("fee_transaction", "staff_fee_transaction");
            return sql;
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
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EdvironPaymentController", "CalculateTransactionFee", ex.Message);
                }
            }
            return feePaymentModeCharges;
        }

        public ActionResult StaffPaymentModeForAdmin(string sFrequencyTypeId, string sFrequencyId, string sFeeRootId, string sStudentId, string sAmount, string sAcademicyear, string sHead = null, string sSelectionID = null)
        {
            FeeTransactionViewModel liFeeTransactions = new FeeTransactionViewModel();
            try
            {
                if (string.IsNullOrEmpty(sHead))
                {
                    sHead = sSelectionID;
                }
                List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
                if (string.IsNullOrEmpty(sStudentId))
                {
                    sStudentId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                }

                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    liFeeTransactions.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;

                    var res = CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStudentId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAllFeeDuesByStaffId), sAcademicyear);
                    studentAcountList = (res != null && res.DataSource != null && res.DataSource.Data != null)
                                        ? (List<FEE_STUDENT_ACCOUNT>)res.DataSource.Data
                                        : new List<FEE_STUDENT_ACCOUNT>();

                    if (studentAcountList != null && studentAcountList.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(sHead))
                        {
                            var selectedHeads = sHead.Split(',').ToList();
                            studentAcountList = studentAcountList.Where(s => selectedHeads.Contains(s.HEAD_ID) || selectedHeads.Contains(s.HEAD)).ToList();
                        }
                    }

                    if (studentAcountList != null && studentAcountList.Count > 0)
                    {
                        studentAcountList.FirstOrDefault().FREQUENCY_TYPE_ID = sFrequencyTypeId;
                        studentAcountList.FirstOrDefault().FEE_ROOT_ID = sFeeRootId;
                        studentAcountList.FirstOrDefault().FREQUENCY_ID = sFrequencyId;
                        studentAcountList.FirstOrDefault().STUDENT_ID = sStudentId;
                        studentAcountList.FirstOrDefault().ACADEMIC_YEAR = sAcademicyear;
                        studentAcountList.FirstOrDefault().PAYMENTTYPEID = "3";
                        liFeeTransactions.liFeeStudentAccount = studentAcountList;

                        var chargesRes = CMSHandler.FetchData<FEE_PAYMENT_MODE_CHARGES>(null, PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchPaymentModes));
                        var modeCharges = (chargesRes != null && chargesRes.DataSource != null && chargesRes.DataSource.Data != null)
                                          ? (List<FEE_PAYMENT_MODE_CHARGES>)chargesRes.DataSource.Data
                                          : new List<FEE_PAYMENT_MODE_CHARGES>();

                        liFeeTransactions.liPAYMENT_MODE_CHARGES = CalculateTransactionFee(studentAcountList, modeCharges);

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
                    objHandler.WriteError("EdvironPaymentController", "StaffPaymentModeForAdmin", ex.Message);
                }
            }
            return View("~/Views/PaymentGateway/StaffPaymentModeForAdmin.cshtml", liFeeTransactions);
        }

        [HttpPost]
        public async Task<JsonResult> CreateStaffOrder(string sFrequencyTypeId, string sFeeRootId, string sFrequencyId, string sAcademicYear, string sStudentId, string sPaymentMode, string sAmount, string sSelectionID = null)
        {
            try
            {
                if (string.IsNullOrEmpty(sPaymentMode))
                {
                    return Json(new { status = false, Message = "Please select a payment mode." });
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                string sIsPGLive = System.Configuration.ConfigurationManager.AppSettings["ISPG_LIVE"] ?? "0";

                var settings = DapperHandler.FetchAll<FEE_EDVIRON_SETTING_INFO>(
                    EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchEdvironSettings),
                    new { IS_LIVE = sIsPGLive }).ToList();

                if (settings == null || settings.Count == 0)
                {
                    return Json(new { status = false, Message = "Edviron settings not configured." });
                }

                var setting = settings.FirstOrDefault();

                List<FEE_STUDENT_ACCOUNT> staffAccountList = FetchStaffFeeAccount(sStudentId, sFrequencyTypeId, sFrequencyId, sAcademicYear);

                if (staffAccountList != null && staffAccountList.Count > 0 && !string.IsNullOrEmpty(sSelectionID))
                {
                    var selectedHeads = sSelectionID.Split(',').ToList();
                    staffAccountList = staffAccountList.Where(s => selectedHeads.Contains(s.HEAD_ID) || selectedHeads.Contains(s.HEAD)).ToList();
                }

                if (staffAccountList == null || staffAccountList.Count == 0 || staffAccountList.Sum(o => decimal.Parse(o.BALANCE)) <= 0)
                {
                    return Json(new { status = false, Message = "No pending fees found." });
                }

                decimal actualFeeAmount = staffAccountList.Sum(x => decimal.Parse(x.BALANCE));

                var fetchedModes = DapperHandler.FetchAll<FEE_PAYMENT_MODE_CHARGES>(
                    PaymentGatewaySQL.GetPaymentGatewaySQL(PaymentGatewaySQLCommands.FetchPaymentModes)).ToList();

                var matchedCharge = fetchedModes.FirstOrDefault(o =>
                    !string.IsNullOrEmpty(o.PAYMENT_MODE_ID) &&
                    o.PAYMENT_MODE_ID.Equals(sPaymentMode, StringComparison.OrdinalIgnoreCase) &&
                    decimal.Parse(string.IsNullOrEmpty(o.FROM_CHARGES_AMOUNT) ? "0" : o.FROM_CHARGES_AMOUNT) <= actualFeeAmount &&
                    decimal.Parse(string.IsNullOrEmpty(o.TO_CHARGES_AMOUNT) ? "999999999" : o.TO_CHARGES_AMOUNT) >= actualFeeAmount);

                decimal calculatedTotalAmount = actualFeeAmount;
                if (matchedCharge != null)
                {
                    decimal transactionFee = 0;
                    if (matchedCharge.IS_PERCENTAGE_OR_FLAT == "1") // Percentage
                    {
                        transactionFee = actualFeeAmount * decimal.Parse(matchedCharge.PERCENTAGE_OR_FLAT) / 100;
                    }
                    else // Flat
                    {
                        transactionFee = decimal.Parse(matchedCharge.PERCENTAGE_OR_FLAT);
                    }

                    decimal gstAmount = transactionFee * decimal.Parse(matchedCharge.GST_ON_TRANSACTION_FEE) / 100;
                    calculatedTotalAmount = actualFeeAmount + transactionFee + gstAmount;
                }

                decimal totalAmount = calculatedTotalAmount;

                // Check for existing unpaid order in staff order info
                string isOrderExistingSql = GetEdvironSQLForStaff(EdvironSQLCommands.IsOrderExisting);
                var existingOrders = DapperHandler.FetchAll<STAFF_FEE_EDVIRON_ORDER_INFO>(
                    isOrderExistingSql,
                    new
                    {
                        STUDENT_ID = sStudentId,
                        FREQUENCY_ID = sFrequencyId,
                        ACADEMIC_YEAR = sAcademicYear
                    }).ToList();

                if (existingOrders != null && existingOrders.Count > 0)
                {
                    var existing = existingOrders.FirstOrDefault();
                    if ((existing.STATUS != null && existing.STATUS.ToUpper() == "SUCCESS") || (existing.STATUS != null && existing.STATUS.ToUpper() == "PAID"))
                    {
                        return Json(new { status = false, Message = "Payment already successful.", PaymentList = "" });
                    }

                    if (!string.IsNullOrEmpty(existing.COLLECT_REQUEST_URL))
                    {
                        return Json(new { status = true, PaymentList = existing.COLLECT_REQUEST_URL });
                    }

                    // Delete old record
                    DapperHandler.SaveOrUpdate(
                        GetEdvironSQLForStaff(EdvironSQLCommands.DeleteOrderInfo),
                        new { CUSTOM_ORDER_ID = existing.CUSTOM_ORDER_ID });
                }

                string customOrderId = CommonMethods.GetUniqueCodeForPaymentGateway("EDV");

                // Get staff name and email
                var staffInfo = staffAccountList.FirstOrDefault();
                string staffName = staffInfo != null ? staffInfo.FIRST_NAME : "Staff Member";
                string staffEmail = staffInfo != null ? staffInfo.STU_EMAILID : "payment@college.edu.in";
                if (string.IsNullOrEmpty(staffEmail)) staffEmail = "payment@college.edu.in";

                var vendorAccounts = DapperHandler.FetchAll<EdvironVendorAccountModel>(
                    EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchVendorAccounts), null).ToList();

                string[] enabledModes = null;
                string[] disabledModes = null;
                bool isUpi = false;
                if (!string.IsNullOrEmpty(sPaymentMode) && sPaymentMode.ToUpper() != "EDVIRON")
                {
                    var selectedMode = fetchedModes.FirstOrDefault(m => m.PAYMENT_MODE_ID == sPaymentMode || m.PAYMENT_MODE_NAME.Equals(sPaymentMode, StringComparison.OrdinalIgnoreCase));
                    if (selectedMode != null)
                    {
                        string modeName = selectedMode.PAYMENT_MODE_NAME.ToUpper();
                        if (modeName.Contains("UPI"))
                        {
                            enabledModes = new[] { "UPI" };
                            isUpi = true;
                        }
                        else if (modeName.Contains("NET_BANKING") || modeName.Contains("NET BANKING") || modeName.Contains("NETBANKING") || modeName.Contains("NET") || modeName.Contains("BANK"))
                        {
                            enabledModes = new[] { "NET_BANKING" };
                        }
                        else if (modeName.Contains("CREDIT_CARD") || modeName.Contains("CREDIT CARD"))
                        {
                            enabledModes = new[] { "CREDIT_CARD" };
                        }
                        else if (modeName.Contains("DEBIT_CARD") || modeName.Contains("DEBIT CARD"))
                        {
                            enabledModes = new[] { "DEBIT_CARD" };
                        }
                        else if (modeName.Contains("CARD"))
                        {
                            enabledModes = new[] { "CARD" };
                        }
                        else if (modeName.Contains("WALLET"))
                        {
                            enabledModes = new[] { "WALLET" };
                        }
                        else if (modeName.Contains("PAY_LATER") || modeName.Contains("PAYLATER"))
                        {
                            enabledModes = new[] { "PAY_LATER" };
                        }
                        else if (modeName.Contains("CARDLESS"))
                        {
                            enabledModes = new[] { "CARDLESS" };
                        }
                        else if (modeName.Contains("NEFT") || modeName.Contains("RTGS") || modeName.Contains("IMPS"))
                        {
                            enabledModes = new[] { "NEFT_RTGS_IMPS" };
                        }

                        if (enabledModes != null && enabledModes.Length > 0)
                        {
                            var allAvailableModes = new List<string> { "UPI", "NET_BANKING", "CREDIT_CARD", "DEBIT_CARD", "CARD", "WALLET", "PAY_LATER", "CARDLESS", "NEFT_RTGS_IMPS" };
                            disabledModes = allAvailableModes.Where(m => !enabledModes.Contains(m)).ToArray();
                        }
                    }
                }

                var vendorsInfo = new List<EdvironVendorInfoModel>();
                if (vendorAccounts != null && vendorAccounts.Count > 0)
                {
                    var groupingByBank = staffAccountList
                        .Where(x => !string.IsNullOrEmpty(x.BANK_ACCOUNT_ID))
                        .GroupBy(x => x.BANK_ACCOUNT_ID)
                        .Select(g => new { BankAccountId = g.Key, Amount = g.Sum(x => decimal.Parse(x.BALANCE)) });

                    foreach (var group in groupingByBank)
                    {
                        var vendor = vendorAccounts.FirstOrDefault(v => v.BANK_ACCOUNT_ID == group.BankAccountId);
                        if (vendor != null && !string.IsNullOrEmpty(vendor.VENDOR_ID))
                        {
                            vendorsInfo.Add(new EdvironVendorInfoModel
                            {
                                vendor_id = vendor.VENDOR_ID,
                                amount = (double?)group.Amount
                            });
                        }
                    }
                }

                string callbackUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("StaffPaymentCallback", "EdvironPayment");
                string amountStr = totalAmount % 1 == 0 ? totalAmount.ToString("0") : totalAmount.ToString("0.##");
                string signJwt = GenerateOrderJwt(amountStr, setting.PG_KEY, callbackUrl, setting.SCHOOL_ID);

                var apiPayload = new EdvironOrderRequestModel
                {
                    school_id = setting.SCHOOL_ID,
                    amount = amountStr,
                    callback_url = callbackUrl,
                    student_id = sStudentId,
                    student_name = staffName,
                    student_email = staffEmail,
                    additional_data = new { uid = sStudentId, acyear = sAcademicYear },
                    req_webhook_urls = !string.IsNullOrEmpty(setting.WEBHOOK_URL) ? new[] { setting.WEBHOOK_URL } : null,
                    receipt = "REC_" + customOrderId,
                    custom_order_id = customOrderId,
                    sign = signJwt,
                    split_payments = vendorsInfo.Count > 0,
                    vendors_info = vendorsInfo.Count > 0 ? vendorsInfo : null,
                    enabled_modes = enabledModes,
                    disabled_modes = disabledModes
                };

                string insertOrderSql = "INSERT INTO staff_fee_edviron_order_info (SCHOOL_ID, AMOUNT, ACTUAL_AMOUNT, CALLBACK_URL, STUDENT_ID, ACADEMIC_YEAR, STUDENT_NAME, STUDENT_EMAIL, FREQUENCY_ID, FREQUENCY_TYPE_ID, CUSTOM_ORDER_ID, STATUS, IS_PAID, INSTITUTE_CODE) VALUES (?SCHOOL_ID, ?AMOUNT, ?ACTUAL_AMOUNT, ?CALLBACK_URL, ?STUDENT_ID, ?ACADEMIC_YEAR, ?STUDENT_NAME, ?STUDENT_EMAIL, ?FREQUENCY_ID, ?FREQUENCY_TYPE_ID, ?CUSTOM_ORDER_ID, 'NEW', 0, ?INSTITUTE_CODE)";
                DapperHandler.SaveOrUpdate(insertOrderSql, new
                {
                    SCHOOL_ID = apiPayload.school_id,
                    AMOUNT = decimal.Parse(apiPayload.amount),
                    ACTUAL_AMOUNT = actualFeeAmount,
                    CALLBACK_URL = apiPayload.callback_url,
                    STUDENT_ID = apiPayload.student_id,
                    ACADEMIC_YEAR = int.Parse(sAcademicYear),
                    STUDENT_NAME = apiPayload.student_name,
                    STUDENT_EMAIL = apiPayload.student_email,
                    FREQUENCY_ID = sFrequencyId,
                    FREQUENCY_TYPE_ID = sFrequencyTypeId,
                    CUSTOM_ORDER_ID = apiPayload.custom_order_id,
                    INSTITUTE_CODE = sSelectionID
                });

                string url = setting.BASE_URL.TrimEnd('/') + "/create-collect-request/";
                string responseJson = await SendPostRequest(url, setting.SECRET_ID, apiPayload);

                var apiResponse = JsonConvert.DeserializeObject<EdvironCollectResponseModel>(responseJson);
                if (apiResponse != null && !string.IsNullOrEmpty(apiResponse.Collect_Request_Id))
                {
                    string updateUrlSql = "UPDATE staff_fee_edviron_order_info SET COLLECT_REQUEST_ID = ?COLLECT_REQUEST_ID, COLLECT_REQUEST_URL = ?COLLECT_REQUEST_URL, SIGN = ?SIGN, STATUS = ?STATUS, RESPONSE_JSON = ?RESPONSE_JSON WHERE CUSTOM_ORDER_ID = ?CUSTOM_ORDER_ID";
                    DapperHandler.SaveOrUpdate(updateUrlSql, new
                    {
                        COLLECT_REQUEST_ID = apiResponse.Collect_Request_Id,
                        COLLECT_REQUEST_URL = apiResponse.Collect_Request_Url,
                        SIGN = apiResponse.Sign ?? signJwt,
                        STATUS = "CREATED",
                        RESPONSE_JSON = responseJson,
                        CUSTOM_ORDER_ID = customOrderId
                    });

                    return Json(new { status = true, PaymentList = apiResponse.Collect_Request_Url });
                }

                return Json(new { status = false, Message = "Failed to generate payment link. Please try again." });
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EdvironPaymentController", "CreateStaffOrder", ex.Message);
                }
                return Json(new { status = false, Message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> StaffPaymentCallback(string EdvironCollectRequestId, string status)
        {
            var viewModel = new EdvironPaymentResponseViewModel();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                string sIsPGLive = System.Configuration.ConfigurationManager.AppSettings["ISPG_LIVE"] ?? "0";

                STAFF_FEE_EDVIRON_ORDER_INFO order = null;

                if (!string.IsNullOrEmpty(EdvironCollectRequestId))
                {
                    var orders = DapperHandler.FetchAll<STAFF_FEE_EDVIRON_ORDER_INFO>(
                        GetEdvironSQLForStaff(EdvironSQLCommands.FetchOrderByCollectId),
                        new { COLLECT_REQUEST_ID = EdvironCollectRequestId }).ToList();
                    order = (orders != null && orders.Count > 0) ? orders[0] : null;
                }

                if (order == null)
                {
                    viewModel.Status = "FAILED";
                    viewModel.Message = "Order not found.";
                    return View("PaymentResponse", viewModel);
                }

                if ((order.STATUS != null && order.STATUS.ToUpper() == "PAID") || (order.STATUS != null && order.STATUS.ToUpper() == "SUCCESS"))
                {
                    return RedirectToAction("AdmissionPaymentResponse", "PaymentGateway", new { sOrderId = order.CUSTOM_ORDER_ID });
                }

                var settings = DapperHandler.FetchAll<FEE_EDVIRON_SETTING_INFO>(
                    EdvironSQL.GetEdvironSQL(EdvironSQLCommands.FetchEdvironSettings),
                    new { IS_LIVE = sIsPGLive }).ToList();

                var setting = (settings != null && settings.Count > 0) ? settings[0] : null;
                if (setting == null)
                {
                    return RedirectToAction("AdmissionPaymentResponse", "PaymentGateway", new { sOrderId = order.CUSTOM_ORDER_ID });
                }

                string collectId = order.COLLECT_REQUEST_ID;
                string signJwt = GenerateStatusJwt(setting.PG_KEY, collectId, setting.SCHOOL_ID);
                string verifyUrl = setting.BASE_URL.TrimEnd('/') + "/collect-request/" + collectId + "?school_id=" + setting.SCHOOL_ID + "&sign=" + signJwt;

                var client = new RestSharp.RestClient(verifyUrl);
                var request = new RestRequest(verifyUrl, Method.Get);
                request.AddHeader("Authorization", "Bearer " + setting.SECRET_ID);

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var statusResponse = JsonConvert.DeserializeObject<EdvironCollectResponseModel>(response.Content);

                    if (statusResponse != null && statusResponse.status != null && statusResponse.status.ToUpper() == "SUCCESS")
                    {
                        DapperHandler.SaveOrUpdate(
                            GetEdvironSQLForStaff(EdvironSQLCommands.UpdateOrderStatus),
                            new { CUSTOM_ORDER_ID = order.CUSTOM_ORDER_ID, STATUS = "PAID" });

                        int lockRowsAffected = Convert.ToInt32(DapperHandler.SaveOrUpdate(
                            GetEdvironSQLForStaff(EdvironSQLCommands.AcquireProcessingLock),
                            new { CUSTOM_ORDER_ID = order.CUSTOM_ORDER_ID }));

                        if (lockRowsAffected > 0)
                        {
                            string receiptNo = ProcessStaffFeeCollection(order);
                            viewModel.ReceiptNo = receiptNo;

                            await FetchAndSaveStaffTransactionInfo(setting, collectId, signJwt, order);
                        }

                        return RedirectToAction("AdmissionPaymentResponse", "PaymentGateway", new { sOrderId = order.CUSTOM_ORDER_ID });
                    }
                    else
                    {
                        string apiStatus = (statusResponse != null && statusResponse.status != null) ? statusResponse.status : (status != null ? status : "UNKNOWN");
                        DapperHandler.SaveOrUpdate(
                            GetEdvironSQLForStaff(EdvironSQLCommands.UpdateOrderStatus),
                            new { CUSTOM_ORDER_ID = order.CUSTOM_ORDER_ID, STATUS = apiStatus });
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EdvironPaymentController", "StaffPaymentCallback", ex.Message);
                }
            }

            return RedirectToAction("AdmissionPaymentResponse", "PaymentGateway", new { sOrderId = viewModel.ReceiptNo });
        }

        private string ProcessStaffFeeCollection(STAFF_FEE_EDVIRON_ORDER_INFO order)
        {
            string receiptNo = string.Empty;
            try
            {
                string sStaffId = order.STUDENT_ID;
                string sFrequencyId = order.FREQUENCY_ID;
                string sFrequencyTypeId = order.FREQUENCY_TYPE_ID;
                string sAcademicYear = order.ACADEMIC_YEAR;
                string sAmount = order.ACTUAL_AMOUNT ?? order.AMOUNT;

                List<FEE_STUDENT_ACCOUNT> staffAccountList = FetchStaffFeeAccount(sStaffId, sFrequencyTypeId, sFrequencyId, sAcademicYear);

                if (staffAccountList != null && staffAccountList.Count > 0 && !string.IsNullOrEmpty(order.INSTITUTE_CODE))
                {
                    var selectedHeads = order.INSTITUTE_CODE.Split(',').ToList();
                    staffAccountList = staffAccountList.Where(s => selectedHeads.Contains(s.HEAD_ID) || selectedHeads.Contains(s.HEAD)).ToList();
                }

                if (staffAccountList == null || staffAccountList.Count == 0) return receiptNo;

                List<FEE_STUDENT_ACCOUNT> filteredHeads = new List<FEE_STUDENT_ACCOUNT>();
                foreach (var item in GetPrioritizedHeads(staffAccountList))
                {
                    if (!string.IsNullOrEmpty(item.BALANCE))
                    {
                        decimal balance = 0;
                        if (decimal.TryParse(item.BALANCE, out balance) && balance > 0)
                        {
                            filteredHeads.Add(item);
                        }
                    }
                }

                decimal dAmount = Convert.ToDecimal(!string.IsNullOrEmpty(order.ACTUAL_AMOUNT) ? order.ACTUAL_AMOUNT : order.AMOUNT);
                List<FEE_STUDENT_ACCOUNT> updatedHeads = new List<FEE_STUDENT_ACCOUNT>();
                foreach (var head in filteredHeads)
                {
                    decimal balance = decimal.Parse(head.BALANCE);
                    if (dAmount <= 0) break;

                    if (balance <= dAmount)
                    {
                        head.BALANCE = balance.ToString();
                        dAmount -= balance;
                    }
                    else
                    {
                        head.BALANCE = dAmount.ToString();
                        dAmount = 0;
                    }
                    updatedHeads.Add(head);
                }

                staffAccountList = updatedHeads;
                staffAccountList.RemoveAll(r => r.BALANCE == "0");

                if (staffAccountList.Count == 0) return receiptNo;

                var GroupFeeStaffAccount = staffAccountList.GroupBy(x => x.FREQUENCY_ID);

                foreach (var FrequencyItem in GroupFeeStaffAccount)
                {
                    sFrequencyId = FrequencyItem.FirstOrDefault().FREQUENCY_ID;
                    var feeTransactions = FrequencyItem.ToList();
                    decimal balance = feeTransactions.Sum(x => decimal.Parse(x.BALANCE));

                    string getAcYearSql = "SELECT ACADEMIC_YEAR FROM fee_frequency_fee_mode WHERE FEE_FREQUENCY_FEE_MODE_ID = @FEE_FREQUENCY_FEE_MODE_ID";
                    var acYearResult = DapperHandler.FetchAll<dynamic>(getAcYearSql, new { FEE_FREQUENCY_FEE_MODE_ID = sFrequencyId }, null).FirstOrDefault();
                    string itemAcademicYear = sAcademicYear;
                    if (acYearResult != null)
                    {
                        itemAcademicYear = Convert.ToString(acYearResult.ACADEMIC_YEAR);
                    }

                    if (balance > 0)
                    {
                        var objFeeTransactionModel = new FEE_STAFF_TRANSACTION
                        {
                            STAFF_ID = sStaffId,
                            FREQUENCY = sFrequencyId,
                            FREQUENCY_TO = sFrequencyId,
                            ACADEMIC_YEAR = itemAcademicYear,
                            PAYMENT_DATE = DateTime.Today.ToString("yyyy-MM-dd"),
                            USERNAME = sStaffId,
                            COLLECTED = balance.ToString(),
                            CF_ORDER_ID = order.CUSTOM_ORDER_ID,
                            PAYMENT_MODE = Common.PaymentMode.Online
                        };

                        var FetchReceiptNo = DapperHandler.FetchAll<AUTO_GENERATE_NUMBERS>(
                            FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAutoGenerateNumber),
                            new { STUDENT_ID = sStaffId }, string.Empty).ToList();

                        if (FetchReceiptNo != null && FetchReceiptNo.Count > 0)
                        {
                            int iReceiptNo = Convert.ToInt32(FetchReceiptNo.FirstOrDefault().exam_receipt_no);
                            AUTO_GENERATE_NUMBERS objAutoGenerateNumber = new AUTO_GENERATE_NUMBERS
                            {
                                exam_receipt_no = (iReceiptNo + 1).ToString(),
                                auto_generate_numbers_id = "1"
                            };
                            objFeeTransactionModel.RECEIPT_NO = "STF00" + objAutoGenerateNumber.exam_receipt_no;
                            receiptNo = objFeeTransactionModel.RECEIPT_NO;

                            var UpdateReceiptNo = CMSHandler.SaveOrUpdate(
                                objAutoGenerateNumber,
                                FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateAutoGenerateNumber)
                                    .Replace("?exam_receipt_no", objAutoGenerateNumber.exam_receipt_no)
                                    .Replace("?auto_generate_numbers_id", objAutoGenerateNumber.auto_generate_numbers_id),
                                string.Empty, false);

                            if (UpdateReceiptNo.Success)
                            {
                                string insertTransQuery = "INSERT INTO staff_fee_transaction (STAFF_ID, FREQUENCY, FREQUENCY_TO, PAYMENT_DATE, RECEIPT_NO, PAYMENT_MODE, COLLECTED, USERNAME, ACADEMIC_YEAR, RAZORPAY_ID,CF_ORDER_ID) VALUES (?STAFF_ID, ?FREQUENCY, ?FREQUENCY_TO, ?PAYMENT_DATE, ?RECEIPT_NO, ?PAYMENT_MODE, ?COLLECTED, ?USERNAME, ?ACADEMIC_YEAR, ?RAZORPAY_ID,?CF_ORDER_ID)";
                                
                                var SaveFeeTransaction = DapperHandler.SaveOrUpdate(
                                    insertTransQuery,
                                    new {
                                        STAFF_ID = objFeeTransactionModel.STAFF_ID,
                                        FREQUENCY = int.Parse(objFeeTransactionModel.FREQUENCY),
                                        FREQUENCY_TO = int.Parse(objFeeTransactionModel.FREQUENCY_TO),
                                        PAYMENT_DATE = DateTime.Parse(objFeeTransactionModel.PAYMENT_DATE),
                                        RECEIPT_NO = objFeeTransactionModel.RECEIPT_NO,
                                        PAYMENT_MODE = int.Parse(objFeeTransactionModel.PAYMENT_MODE),
                                        COLLECTED = double.Parse(objFeeTransactionModel.COLLECTED),
                                        USERNAME = objFeeTransactionModel.USERNAME,
                                        ACADEMIC_YEAR = int.Parse(itemAcademicYear),
                                        RAZORPAY_ID = 0,
                                        CF_ORDER_ID = order.CUSTOM_ORDER_ID,
                                    });

                                var lastTrans = DapperHandler.FetchAll<FEE_STAFF_TRANSACTION>("SELECT TRANSACTION_ID FROM staff_fee_transaction WHERE RECEIPT_NO = ?RECEIPT_NO", new { RECEIPT_NO = objFeeTransactionModel.RECEIPT_NO }).ToList();
                                string TransactionId = (lastTrans != null && lastTrans.Count > 0) ? lastTrans.FirstOrDefault().TRANSACTION_ID : "0";

                                foreach (var item in feeTransactions)
                                {
                                     int nTransId = 0; int.TryParse(TransactionId, out nTransId);
                                     int nHeadId = 0; int.TryParse(item.HEAD_ID, out nHeadId);
                                     double dPaidAmount = 0; double.TryParse(item.BALANCE, out dPaidAmount);
                                     int nFreqId = 0; int.TryParse(sFrequencyId, out nFreqId);
                                     int nFeeMainHeadId = 0; int.TryParse(item.FEE_MAIN_HEAD_ID, out nFeeMainHeadId);

                                     string insertCollQuery = "INSERT INTO staff_fee_collection (TRANSACTION_ID, HEAD, PAID_AMOUNT, FREQUENCY, RECEIPT_NO, FEE_MAIN_HEAD_ID) VALUES (?TRANSACTION_ID, ?HEAD, ?PAID_AMOUNT, ?FREQUENCY, ?RECEIPT_NO, ?FEE_MAIN_HEAD_ID)";
                                     DapperHandler.SaveOrUpdate(insertCollQuery, new
                                     {
                                         TRANSACTION_ID = nTransId,
                                         HEAD = nHeadId,
                                         PAID_AMOUNT = dPaidAmount,
                                         FREQUENCY = nFreqId,
                                         RECEIPT_NO = objFeeTransactionModel.RECEIPT_NO,
                                         FEE_MAIN_HEAD_ID = nFeeMainHeadId
                                     });

                                     // Insert debit row into fee_staff_account to adjust balance
                                     string insertStaffAccQuery = "INSERT INTO fee_staff_account (STAFF_ID, ACADEMIC_YEAR, FREQUENCY_ID, HEAD, DEBIT, CREDIT, FEE_MAIN_HEAD_ID, TRANSACTION_DATE, IS_DELETED) VALUES (?STAFF_ID, ?ACADEMIC_YEAR, ?FREQUENCY_ID, ?HEAD, ?DEBIT, 0, ?FEE_MAIN_HEAD_ID, ?TRANSACTION_DATE, 0)";
                                     DapperHandler.SaveOrUpdate(insertStaffAccQuery, new
                                     {
                                         STAFF_ID = sStaffId,
                                         ACADEMIC_YEAR = itemAcademicYear,
                                         FREQUENCY_ID = sFrequencyId,
                                         HEAD = item.HEAD_ID,
                                         DEBIT = dPaidAmount,
                                         FEE_MAIN_HEAD_ID = nFeeMainHeadId,
                                         TRANSACTION_DATE = DateTime.Today.ToString("yyyy-MM-dd")
                                     });
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
                    objHandler.WriteError("EdvironPaymentController", "ProcessStaffFeeCollection", ex.Message);
                }
            }
            return receiptNo;
        }

        private async Task FetchAndSaveStaffTransactionInfo(FEE_EDVIRON_SETTING_INFO settings, string collectId, string sign, STAFF_FEE_EDVIRON_ORDER_INFO order)
        {
            try
            {
                string url = settings.BASE_URL.TrimEnd('/') + "/transaction-info?collect_request_id=" + collectId + "&sign=" + sign + "&school_id=" + settings.SCHOOL_ID;

                var client = new RestSharp.RestClient(url);
                var request = new RestRequest(url, Method.Get);
                request.AddHeader("Authorization", "Bearer " + settings.SECRET_ID);

                var response = await client.ExecuteAsync(request);
                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var details = JsonConvert.DeserializeObject<EdvironTransactionInfoModel>(response.Content);
                    if (details != null)
                    {
                        var transfer = new STAFF_FEE_EDVIRON_TRANSFER_INFO
                        {
                            STATUS = details.status,
                            TRANSACTION_AMOUNT = details.transaction_amount,
                            CUSTOM_ORDER_ID = details.custom_order_id ?? order.CUSTOM_ORDER_ID,
                            COLLECT_REQUEST_ID = details.collect_id ?? collectId,
                            SCHOOL_ID = details.school_id,
                            STUDENT_ID = details.student_id ?? order.STUDENT_ID,
                            BANK_REFERENCE = details.bank_reference,
                            PAYMENT_METHOD = details.payment_method,
                            CREATED_AT = details.createdAt,
                            UPDATED_AT = details.UpdatedAt ?? details.transaction_time,
                            PAYMENT_DETAILS = details.payment_details,
                            ACADEMIC_YEAR_ID = order.ACADEMIC_YEAR,
                            AMOUNT = details.amount ?? order.AMOUNT,
                            STUDENT_NAME = details.student_name ?? order.STUDENT_NAME,
                            STUDENT_EMAIL = details.student_email ?? order.STUDENT_EMAIL,
                            CURRENCY = "INR"
                        };

                        DapperHandler.SaveOrUpdate(GetEdvironSQLForStaff(EdvironSQLCommands.SaveTransferInfo), transfer);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EdvironPaymentController", "FetchAndSaveStaffTransactionInfo", ex.Message);
                }
            }
        }

        private List<FEE_STUDENT_ACCOUNT> FetchStaffFeeAccount(string sStaffId, string sFrequencyTypeId, string sFrequencyId, string sAcademicYear)
        {
            List<FEE_STUDENT_ACCOUNT> staffAccountList = new List<FEE_STUDENT_ACCOUNT>();
            try
            {
                var res = CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sStaffId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAllFeeDuesByStaffId), sAcademicYear);
                if (res != null && res.DataSource != null && res.DataSource.Data != null)
                {
                    staffAccountList = (List<FEE_STUDENT_ACCOUNT>)res.DataSource.Data;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EdvironPaymentController", "FetchStaffFeeAccount", ex.Message);
                }
            }
            return staffAccountList;
        }

        #endregion

        /// <summary>
        /// Logic to sort fee heads: Normal heads first, Tail heads (fines) last.
        /// </summary>
        private List<FEE_STUDENT_ACCOUNT> GetPrioritizedHeads(List<FEE_STUDENT_ACCOUNT> heads)
        {
            if (heads == null || heads.Count == 0) return heads;

            List<int> lastHeads = GetTailHeadIds();

            var normalHeads = heads
                .Where(x => !lastHeads.Contains(Convert.ToInt32(x.HEAD_ID)))
                .OrderBy(x => Convert.ToInt32(x.HEAD_ID));

            var tailHeads = heads
                .Where(x => lastHeads.Contains(Convert.ToInt32(x.HEAD_ID)))
                .OrderBy(x => lastHeads.IndexOf(Convert.ToInt32(x.HEAD_ID)));

            return normalHeads.Concat(tailHeads).ToList();
        }

        #endregion
         

    }
}

