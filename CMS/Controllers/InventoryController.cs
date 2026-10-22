using CMS.Utility;
using CMS.ViewModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Newtonsoft.Json;
using System.Text;
using CMS.DAO;
using CMS.ViewModel.Model;
using CMS.SQL.Inventory;
using CMS.SQL.SupportData;

namespace CMS.Controllers
{
    public class InventoryController : Controller
    {
        InventoryViewModel objViewModel = new InventoryViewModel();
        JsonResultData ObjJsonData = new JsonResultData();
        ResultArgs ResultArg = new ResultArgs();
        #region Invenory Department
        public ActionResult ListDepartment()
        {

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                    objViewModel.liinventdepartment = (List<INVENT_DEPARTMENT>)CMSHandler.FetchData<INVENT_DEPARTMENT>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchInventorydepartment), sAcademicYear).DataSource.Data;


                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListDepartment", "Inventory", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListDepartment", "Inventory", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchdInventoryDepart(string sDepartmentId)
        {
            var objModel = new INVENT_DEPARTMENT();
            if (!string.IsNullOrEmpty(sDepartmentId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var lidepartment = (List<INVENT_DEPARTMENT>)CMSHandler.FetchData<INVENT_DEPARTMENT>(new INVENT_DEPARTMENT() { DEPARTMENT_ID = sDepartmentId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchInventorydepartmentById), sAcademicYear).DataSource.Data;
                    if (lidepartment != null && lidepartment.Count > 0)
                    {
                        objModel.DEPARTMENT_ID = lidepartment.FirstOrDefault().DEPARTMENT_ID;
                        objModel.DEPARTMENT_NAME = lidepartment.FirstOrDefault().DEPARTMENT_NAME;
                        objModel.IS_ACTIVE = lidepartment.FirstOrDefault().IS_ACTIVE;

                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Inventory", "FetchdInventoryDepart", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateInventoryDepartment(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<INVENT_DEPARTMENT>(sJsonData);
                    var SaveTaskSubmission = CMSHandler.SaveOrUpdate(Result, InventorySQL.GetInventorySQL((!string.IsNullOrEmpty(Result.DEPARTMENT_ID) ? InventoryEnumCommands.UpdateInventoryDepartment : InventoryEnumCommands.SaveInventoryDepartment)), sAcademicYear);
                    if (SaveTaskSubmission.Success)
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
                    objHandler.WriteError("Inventory", "SaveorUpdateDepartment", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Inventory", "SaveorUpdateDepartment", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult DeleteInventoryDepartment(string sDepartmentId)
        {
            if (!string.IsNullOrEmpty(sDepartmentId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new INVENT_DEPARTMENT() { DEPARTMENT_ID = sDepartmentId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.DeleteInventorydepartment));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Inventory", "DeleteInventoryDepartment", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Inventory Items
        public ActionResult ListInventoryItems()
        {

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                    objViewModel.liinventitems = (List<INVENT_ITEMS>)CMSHandler.FetchData<INVENT_ITEMS>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchInventoryItems), sAcademicYear).DataSource.Data;


                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListInventoryItems", "Inventory", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListInventoryItems", "Inventory", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchInventoryItems(string sItemId)
        {
            var objModel = new INVENT_ITEMS();
            if (!string.IsNullOrEmpty(sItemId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var liitems = (List<INVENT_ITEMS>)CMSHandler.FetchData<INVENT_ITEMS>(new INVENT_ITEMS() { ITEM_ID = sItemId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchInventoryItemsById), sAcademicYear).DataSource.Data;
                    if (liitems != null && liitems.Count > 0)
                    {
                        objModel.ITEM_ID = liitems.FirstOrDefault().ITEM_ID;
                        objModel.ITEM_NAME = liitems.FirstOrDefault().ITEM_NAME;
                        objModel.IS_ACTIVE = liitems.FirstOrDefault().IS_ACTIVE;

                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Inventory", "FetchInventoryItems", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateInventoryItems(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<INVENT_ITEMS>(sJsonData);
                    var SaveTaskSubmission = CMSHandler.SaveOrUpdate(Result, InventorySQL.GetInventorySQL((!string.IsNullOrEmpty(Result.ITEM_ID) ? InventoryEnumCommands.UpdateInventoryItems : InventoryEnumCommands.SaveInventoryItems)), sAcademicYear);
                    if (SaveTaskSubmission.Success)
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
                    objHandler.WriteError("Inventory", "SaveorUpdateInventoryItems", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Inventory", "SaveorUpdateInventoryItems", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult DeleteInventoryItems(string sItemId)
        {
            if (!string.IsNullOrEmpty(sItemId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new INVENT_ITEMS() { ITEM_ID = sItemId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.DeleteInventoryItems));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Inventory", "DeleteInventoryItems", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Vendor Details
        public ActionResult ListVendor()
        {

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objViewModel.livendors = (List<INVENT_VENDOR_DETAILS>)CMSHandler.FetchData<INVENT_VENDOR_DETAILS>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchAllVendor), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListVendor", "Inventory", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListVendor", "Inventory", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchVendor(string sVendorId)
        {
            var objModel = new INVENT_VENDOR_DETAILS();
            if (!string.IsNullOrEmpty(sVendorId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var liitems = (List<INVENT_VENDOR_DETAILS>)CMSHandler.FetchData<INVENT_VENDOR_DETAILS>(new INVENT_VENDOR_DETAILS() { VENDOR_ID = sVendorId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchVendorById), sAcademicYear).DataSource.Data;
                    if (liitems != null && liitems.Count > 0)
                    {
                        objModel.VENDOR_ID = liitems.FirstOrDefault().VENDOR_ID;
                        objModel.VENDOR_NAME = liitems.FirstOrDefault().VENDOR_NAME;
                        objModel.ADDRESS = liitems.FirstOrDefault().ADDRESS;
                        objModel.MOBILE = liitems.FirstOrDefault().MOBILE;
                        objModel.ORGANIZATION_NAME = liitems.FirstOrDefault().ORGANIZATION_NAME;
                        objModel.IS_ACTIVE = liitems.FirstOrDefault().IS_ACTIVE;

                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Inventory", "FetchInventoryItems", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateVendors(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<INVENT_VENDOR_DETAILS>(sJsonData);
                    var SaveTaskSubmission = CMSHandler.SaveOrUpdate(Result, InventorySQL.GetInventorySQL((!string.IsNullOrEmpty(Result.VENDOR_ID) ? InventoryEnumCommands.UpdateVendors : InventoryEnumCommands.SaveVendors)), sAcademicYear);
                    if (SaveTaskSubmission.Success)
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
                    objHandler.WriteError("Inventory", "SaveorUpdateInventoryItems", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Inventory", "SaveorUpdateInventoryItems", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult DeleteVendors(string sVendorId)
        {
            if (!string.IsNullOrEmpty(sVendorId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new INVENT_VENDOR_DETAILS() { VENDOR_ID = sVendorId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.DeleteVendors));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Inventory", "DeleteVendors", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region Inventory Purchase Details
        public ActionResult ListPurchaseDetails()
        {

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objViewModel.lipurchasedetails = (List<INVENT_PURCHASE_DETAILS>)CMSHandler.FetchData<INVENT_PURCHASE_DETAILS>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchPurchaseDetails), sAcademicYear).DataSource.Data;

                    var itemlist = (List<INVENT_ITEMS>)CMSHandler.FetchData<INVENT_ITEMS>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchInventItems), sAcademicYear).DataSource.Data;
                    ViewBag.liitems = itemlist;
                    if (itemlist != null && itemlist.Count > 0)
                    {
                        objViewModel.ITEMS = new SelectList(itemlist, Common.INVENT_ITEMS.ITEM_ID, Common.INVENT_ITEMS.ITEM_NAME);
                    }
                    var Vendorlist = (List<INVENT_VENDOR_DETAILS>)CMSHandler.FetchData<INVENT_VENDOR_DETAILS>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchInventVendors), sAcademicYear).DataSource.Data;
                    ViewBag.livendors = Vendorlist;
                    if (Vendorlist != null && Vendorlist.Count > 0)
                    {
                        objViewModel.VENDOR = new SelectList(Vendorlist, Common.INVENT_VENDOR_DETAILS.VENDOR_ID, Common.INVENT_VENDOR_DETAILS.VENDOR_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListPurchaseDetails", "Inventory", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListPurchaseDetails", "Inventory", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchPurchaseDetails(string sPurchaseId)
        {
            var objModel = new INVENT_PURCHASE_DETAILS();
            if (!string.IsNullOrEmpty(sPurchaseId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var lipurchasedetails = (List<INVENT_PURCHASE_DETAILS>)CMSHandler.FetchData<INVENT_PURCHASE_DETAILS>(new INVENT_PURCHASE_DETAILS() { PURCHASE_ID = sPurchaseId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchPurchaseDetailsById), sAcademicYear).DataSource.Data;
                    if (lipurchasedetails != null && lipurchasedetails.Count > 0)
                    {
                        objModel.VENDOR_ID = lipurchasedetails.FirstOrDefault().VENDOR_ID;
                        objModel.PURCHASE_ID = lipurchasedetails.FirstOrDefault().PURCHASE_ID;
                        objModel.ITEM_ID = lipurchasedetails.FirstOrDefault().ITEM_ID;
                        objModel.QUANTITY = lipurchasedetails.FirstOrDefault().QUANTITY;
                        objModel.PURCHASE_DATE = lipurchasedetails.FirstOrDefault().PURCHASE_DATE;
                        objModel.INVOICE_NO = lipurchasedetails.FirstOrDefault().INVOICE_NO;
                        objModel.INVOICE_DATE = lipurchasedetails.FirstOrDefault().INVOICE_DATE;
                        objModel.TOTAL_PRICE = lipurchasedetails.FirstOrDefault().TOTAL_PRICE;
                        objModel.MODEL_NAME = lipurchasedetails.FirstOrDefault().MODEL_NAME;


                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Inventory", "FetchPurchaseDetails", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SavePurchaseDetails(string sBasicData, string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {

                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var lipurchasedetails = JsonConvert.DeserializeObject<INVENT_PURCHASE_DETAILS>(sBasicData);
                    var Result = JsonConvert.DeserializeObject<JSON_ARRAY>(sJsonData);
                    if (Result != null)
                    {
                        foreach (var item in Result.LipurchaseDetails)
                        {
                            if (lipurchasedetails != null)
                            {
                                item.INVOICE_NO = lipurchasedetails.INVOICE_NO;
                                item.INVOICE_DATE = CommonMethods.ConvertdatetoyearFromat(lipurchasedetails.INVOICE_DATE);
                                item.PURCHASE_DATE = CommonMethods.ConvertdatetoyearFromat(lipurchasedetails.PURCHASE_DATE);
                                item.ACADEMIC_YEAR = sAcademicYear;

                            }
                            ResultArg = CMSHandler.SaveOrUpdate(item, InventorySQL.GetInventorySQL(InventoryEnumCommands.SavePurchaseDetails), sAcademicYear);
                        }


                    }
                    if (ResultArg.Success)
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
                    objHandler.WriteError("Inventory", "SaveorUpdateInventoryItems", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Inventory", "SaveorUpdateInventoryItems", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult UpdatePurchaseDetails(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<INVENT_PURCHASE_DETAILS>(sJsonData);
                    Result.INVOICE_DATE = CommonMethods.ConvertdatetoyearFromat(Result.INVOICE_DATE);
                    Result.PURCHASE_DATE = CommonMethods.ConvertdatetoyearFromat(Result.PURCHASE_DATE);

                    ResultArg = CMSHandler.SaveOrUpdate(Result, InventorySQL.GetInventorySQL(InventoryEnumCommands.UpdatePurchaseDetails), sAcademicYear);
                    if (ResultArg.Success)
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
                    objHandler.WriteError("Inventory", "SaveorUpdateInventoryItems", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Inventory", "SaveorUpdateInventoryItems", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult DeletePurchaseDetails(string sPurchaseId)
        {
            if (!string.IsNullOrEmpty(sPurchaseId))
            {
                try
                {
                    var ResultArg = CMSHandler.SaveOrUpdate(new INVENT_PURCHASE_DETAILS() { PURCHASE_ID = sPurchaseId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.DeletePurchaseDetails));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Inventory", "DeletePurchaseDetails", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Department Stock Request and approval
        public ActionResult ListStockRequest()
        {

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                    objViewModel.listockrequest = (List<INVENT_STOCK_REQUEST>)CMSHandler.FetchData<INVENT_STOCK_REQUEST>(new INVENT_STOCK_REQUEST() { STAFF_ID = sUserId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchStockRequestByUserId), sAcademicYear).DataSource.Data;

                    var itemlist = (List<INVENT_ITEMS>)CMSHandler.FetchData<INVENT_ITEMS>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchInventItems), sAcademicYear).DataSource.Data;

                    if (itemlist != null && itemlist.Count > 0)
                    {
                        objViewModel.ITEMS = new SelectList(itemlist, Common.INVENT_ITEMS.ITEM_ID, Common.INVENT_ITEMS.ITEM_NAME);
                    }
                    var departmentlist = (List<INVENT_DEPARTMENT>)CMSHandler.FetchData<INVENT_DEPARTMENT>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchInventDepartment), sAcademicYear).DataSource.Data;

                    if (departmentlist != null && departmentlist.Count > 0)
                    {
                        objViewModel.DEPARTMENT = new SelectList(departmentlist, Common.INVENT_DEPARTMENT.DEPARTMENT_ID, Common.INVENT_DEPARTMENT.DEPARTMENT_NAME);
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListStockRequest", "Inventory", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListStockRequest", "Inventory", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchStockRequest(string sStockrequestId)
        {
            var objModel = new INVENT_STOCK_REQUEST();
            if (!string.IsNullOrEmpty(sStockrequestId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var listockrequest = (List<INVENT_STOCK_REQUEST>)CMSHandler.FetchData<INVENT_STOCK_REQUEST>(new INVENT_STOCK_REQUEST() { STOCK_REQUEST_ID = sStockrequestId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchStockRequestById), sAcademicYear).DataSource.Data;
                    if (listockrequest != null && listockrequest.Count > 0)
                    {
                        objModel.STOCK_REQUEST_ID = listockrequest.FirstOrDefault().STOCK_REQUEST_ID;
                        objModel.DEP_ID = listockrequest.FirstOrDefault().DEP_ID;
                        objModel.ITEM_ID = listockrequest.FirstOrDefault().ITEM_ID;
                        objModel.QUANTITY = listockrequest.FirstOrDefault().QUANTITY;
                        objModel.PURPOSE = listockrequest.FirstOrDefault().PURPOSE;



                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Inventory", "FetchStockRequest", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateStockRequest(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {

                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var Result = JsonConvert.DeserializeObject<INVENT_STOCK_REQUEST>(sJsonData);
                    if (Result != null)
                    {
                        Result.STAFF_ID = sUserId;
                        Result.ACADEMIC_YEAR = sAcademicYear;
                        ResultArg = CMSHandler.SaveOrUpdate(Result, InventorySQL.GetInventorySQL((!string.IsNullOrEmpty(Result.STOCK_REQUEST_ID) ? InventoryEnumCommands.UpdateStockRequest : InventoryEnumCommands.SaveStockRequest)), sAcademicYear);
                    }

                }
                if (ResultArg.Success)
                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                else
                    objResult.Message = Common.Messages.FailedToSaveRecords;

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Inventory", "SaveorUpdateStockRequest", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Inventory", "SaveorUpdateStockRequest", ex.Message);
                }

            }
            return Json(objResult);
        }

        public JsonResult DeleteStockRequest(string sStockrequestId)
        {
            if (!string.IsNullOrEmpty(sStockrequestId))
            {
                try
                {
                    var ResultArg = CMSHandler.SaveOrUpdate(new INVENT_STOCK_REQUEST() { STOCK_REQUEST_ID = sStockrequestId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.DeleteStockRequest));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Inventory", "DeleteStockRequest", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListStockRequestApproval()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                    objViewModel.listockrequest = (List<INVENT_STOCK_REQUEST>)CMSHandler.FetchData<INVENT_STOCK_REQUEST>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchStockrequestforApproval), sAcademicYear).DataSource.Data;
                    var listatus = (List<SUP_INVENT_STATUS>)CMSHandler.FetchData<SUP_INVENT_STATUS>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchSupInventStatus), sAcademicYear).DataSource.Data;

                    if (listatus != null && listatus.Count > 0)
                    {
                        objViewModel.STATUS = new SelectList(listatus, Common.SUP_HALL_RESERVATION_STATUS.STATUS_ID, Common.SUP_HALL_RESERVATION_STATUS.STATUS_NAME);
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListStockRequestApproval", "Inventory", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListStockRequestApproval", "Inventory", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchStockRequestById(string sStockrequestId)
        {
            var objModel = new INVENT_STOCK_REQUEST();
            if (!string.IsNullOrEmpty(sStockrequestId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var listockrequest = (List<INVENT_STOCK_REQUEST>)CMSHandler.FetchData<INVENT_STOCK_REQUEST>(new INVENT_STOCK_REQUEST() { STOCK_REQUEST_ID = sStockrequestId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchStockRequestById), sAcademicYear).DataSource.Data;
                    if (listockrequest != null && listockrequest.Count > 0)
                    {
                        objModel.STOCK_REQUEST_ID = listockrequest.FirstOrDefault().STOCK_REQUEST_ID;
                        objModel.DEP_ID = listockrequest.FirstOrDefault().DEP_ID;
                        objModel.ITEM_ID = listockrequest.FirstOrDefault().ITEM_ID;
                        objModel.QUANTITY = listockrequest.FirstOrDefault().QUANTITY;
                        objModel.PURPOSE = listockrequest.FirstOrDefault().PURPOSE;
                        objModel.APPROVED_QUANTITY = listockrequest.FirstOrDefault().APPROVED_QUANTITY;
                        objModel.STATUS = listockrequest.FirstOrDefault().STATUS;


                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Inventory", "FetchStockRequestById", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateStockRequestApproval(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {

                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var Result = JsonConvert.DeserializeObject<INVENT_STOCK_REQUEST>(sJsonData);
                    if (Result != null)
                    {

                        ResultArg = CMSHandler.SaveOrUpdate(Result, InventorySQL.GetInventorySQL(InventoryEnumCommands.UpdateStockRequestApproval), sAcademicYear);
                    }

                }
                if (ResultArg.Success)
                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                else
                    objResult.Message = Common.Messages.FailedToSaveRecords;

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Inventory", "SaveorUpdateStockRequestApproval", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Inventory", "SaveorUpdateStockRequestApproval", ex.Message);
                }

            }
            return Json(objResult);
        }
        #endregion
        #region Inventory Stock Distribution
        public ActionResult InventoryStockDestribution()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    
                    objViewModel.listockrequest = (List<INVENT_STOCK_REQUEST>)CMSHandler.FetchData<INVENT_STOCK_REQUEST>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchAllStockRequested), sAcademicYear).DataSource.Data;

                    objViewModel.listockdistribution = (List<INVENT_STOCK_DISTRIBUTION>)CMSHandler.FetchData<INVENT_STOCK_DISTRIBUTION>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchStockDestributed), sAcademicYear).DataSource.Data;

                    var itemlist = (List<INVENT_ITEMS>)CMSHandler.FetchData<INVENT_ITEMS>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchInventItems), sAcademicYear).DataSource.Data;

                    if (itemlist != null && itemlist.Count > 0)
                    {
                        objViewModel.ITEMS = new SelectList(itemlist, Common.INVENT_ITEMS.ITEM_ID, Common.INVENT_ITEMS.ITEM_NAME);
                    }
                    var departmentlist = (List<INVENT_DEPARTMENT>)CMSHandler.FetchData<INVENT_DEPARTMENT>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchInventDepartment), sAcademicYear).DataSource.Data;

                    if (departmentlist != null && departmentlist.Count > 0)
                    {
                        objViewModel.DEPARTMENT = new SelectList(departmentlist, Common.INVENT_DEPARTMENT.DEPARTMENT_ID, Common.INVENT_DEPARTMENT.DEPARTMENT_NAME);
                    }
                    var Vendorlist = (List<INVENT_VENDOR_DETAILS>)CMSHandler.FetchData<INVENT_VENDOR_DETAILS>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchInventVendors), sAcademicYear).DataSource.Data;
                    ViewBag.livendors = Vendorlist;
                    if (Vendorlist != null && Vendorlist.Count > 0)
                    {
                        objViewModel.VENDOR = new SelectList(Vendorlist, Common.INVENT_VENDOR_DETAILS.VENDOR_ID, Common.INVENT_VENDOR_DETAILS.VENDOR_NAME);
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("InventoryStockDestribution", "Inventory", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("InventoryStockDestribution", "Inventory", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchStockDistributedById(string sStockDestributionId)
        {
            var objModel = new INVENT_STOCK_DISTRIBUTION();
            if (!string.IsNullOrEmpty(sStockDestributionId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var listockdistribution = (List<INVENT_STOCK_DISTRIBUTION>)CMSHandler.FetchData<INVENT_STOCK_DISTRIBUTION>(new INVENT_STOCK_DISTRIBUTION() { STOCK_DESTRIBUTION_ID = sStockDestributionId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchStockDistributionById), sAcademicYear).DataSource.Data;
                    if (listockdistribution != null && listockdistribution.Count > 0)
                    {
                        objModel.STOCK_DESTRIBUTION_ID = listockdistribution.FirstOrDefault().STOCK_DESTRIBUTION_ID;
                        objModel.DEP_ID = listockdistribution.FirstOrDefault().DEP_ID;
                        objModel.ITEM_ID = listockdistribution.FirstOrDefault().ITEM_ID;
                        objModel.QUANTITY = listockdistribution.FirstOrDefault().QUANTITY;
                        objModel.VENDOR_ID = listockdistribution.FirstOrDefault().VENDOR_ID;
                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Inventory", "FetchStockDistributedById", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchAvailableStocks(string sItemId,string sVendorId)
        {
            var objModel = new INVENT_STOCK_DISTRIBUTION();
            if (!string.IsNullOrEmpty(sItemId) &&(!string.IsNullOrEmpty(sVendorId)))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var Result = (List<INVENT_STOCK_DISTRIBUTION>)CMSHandler.FetchData<INVENT_STOCK_DISTRIBUTION>(new INVENT_STOCK_DISTRIBUTION() { ITEM_ID = sItemId,VENDOR_ID=sVendorId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchAvailableStock), sAcademicYear).DataSource.Data;
                    if(Result!=null && Result.Count>0)
                    objModel.QUANTITY = Result.FirstOrDefault().QUANTITY;
                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Inventory", "FetchAvailableStocks", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateStockDistribution(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {

                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var Result = JsonConvert.DeserializeObject<INVENT_STOCK_DISTRIBUTION>(sJsonData);
                    if (Result != null)
                    {
                        if((!string.IsNullOrEmpty(Result.STOCK_DESTRIBUTION_ID)))
                        {
                            ResultArg = CMSHandler.SaveOrUpdate(Result, InventorySQL.GetInventorySQL(InventoryEnumCommands.UpdateStockDistribution), sAcademicYear);
                            if (ResultArg.Success)
                            {
                                CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode), sAcademicYear);
                                Result.CREDIT = Result.QUANTITY;
                                Result.STOCK_DIS_ID = Result.STOCK_DESTRIBUTION_ID;
                                var Resultarg = CMSHandler.SaveOrUpdate(Result, InventorySQL.GetInventorySQL(InventoryEnumCommands.UpdateStockTranscation), sAcademicYear);
                                if (Resultarg.Success)
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                else
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            }
                        }
                        else
                        {
                            ResultArg = CMSHandler.SaveOrUpdate(Result, InventorySQL.GetInventorySQL(InventoryEnumCommands.SaveStockDistribution), sAcademicYear,true);
                            if (ResultArg.Success)
                            {
                                CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode), sAcademicYear);
                                Result.STOCK_DIS_ID = ResultArg.RowUniqueId.ToString();
                                Result.CREDIT = Result.QUANTITY;
                                var Resularg = CMSHandler.SaveOrUpdate(Result, InventorySQL.GetInventorySQL(InventoryEnumCommands.SaveStockTranscation), sAcademicYear);
                                if (Resularg.Success)
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                else
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                            }

                        }

                    }

                }
                if (ResultArg.Success)
                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                else
                    objResult.Message = Common.Messages.FailedToSaveRecords;

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Inventory", "SaveorUpdateStockRequestApproval", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Inventory", "SaveorUpdateStockRequestApproval", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult FetchStockRequestDetailsById(string sStockRequesId)
        {
            var objModel = new INVENT_STOCK_REQUEST();
            if (!string.IsNullOrEmpty(sStockRequesId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var listockrequest = (List<INVENT_STOCK_REQUEST>)CMSHandler.FetchData<INVENT_STOCK_REQUEST>(new INVENT_STOCK_REQUEST() { STOCK_REQUEST_ID = sStockRequesId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchStockRequestById), sAcademicYear).DataSource.Data;
                    if (listockrequest != null && listockrequest.Count > 0)
                    {
                        objModel.STOCK_REQUEST_ID = listockrequest.FirstOrDefault().STOCK_REQUEST_ID;
                        objModel.DEPARTMENT_NAME = listockrequest.FirstOrDefault().DEPARTMENT_NAME;
                        objModel.ITEM_NAME = listockrequest.FirstOrDefault().ITEM_NAME;
                        objModel.APPROVED_QUANTITY = listockrequest.FirstOrDefault().APPROVED_QUANTITY;
                        objModel.DEP_ID = listockrequest.FirstOrDefault().DEP_ID;
                        objModel.ITEM_ID = listockrequest.FirstOrDefault().ITEM_ID;


                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Inventory", "FetchStockRequestDetailsById", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindVendorsItemDetails(string sItemId)
        {
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                if (!string.IsNullOrEmpty(sItemId))
                {
                    objViewModel.listockrequest = (List<INVENT_STOCK_REQUEST>)CMSHandler.FetchData<INVENT_STOCK_REQUEST>(new INVENT_STOCK_REQUEST() { ITEM_ID = sItemId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchVendorItemsDetails), sAcademicYear).DataSource.Data;
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindVendorsItemDetails", "Inventory", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindVendorsItemDetails", "Inventory", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult SaveStockDestribution(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {

                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                  
                    var Result = JsonConvert.DeserializeObject<JSON_ARRAY>(sJsonData);
                    if (Result != null)
                    {
                        foreach (var item in Result.LiStockRequest)
                        {
                            ResultArg = CMSHandler.SaveOrUpdate(item, InventorySQL.GetInventorySQL(InventoryEnumCommands.SaveStockDistribution), sAcademicYear, true);
                            if (ResultArg.Success)
                            {
                                CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode), sAcademicYear);
                                item.STOCK_DIS_ID = ResultArg.RowUniqueId.ToString();
                                item.CREDIT = item.QUANTITY;
                                var Resularg = CMSHandler.SaveOrUpdate(item, InventorySQL.GetInventorySQL(InventoryEnumCommands.SaveStockTranscation), sAcademicYear);
                                if (Resularg.Success)
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                else
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                            }
                        }
                    }

                }
                if (ResultArg.Success)
                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                else
                    objResult.Message = Common.Messages.FailedToSaveRecords;

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Inventory", "SaveorUpdateStockRequest", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Inventory", "SaveorUpdateStockRequest", ex.Message);
                }

            }
            return Json(objResult);
        }
        #endregion
        #region Invent Stock Transcation
        public ActionResult InventoryStockTranscation()
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var departmentlist = (List<INVENT_DEPARTMENT>)CMSHandler.FetchData<INVENT_DEPARTMENT>(null, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchInventDepartment), sAcademicYear).DataSource.Data;

                    if (departmentlist != null && departmentlist.Count > 0)
                    {
                        objViewModel.DEPARTMENT = new SelectList(departmentlist, Common.INVENT_DEPARTMENT.DEPARTMENT_ID, Common.INVENT_DEPARTMENT.DEPARTMENT_NAME);
                    }
                    
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("InventoryStockTranscation", "Inventory", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("InventoryStockTranscation", "Inventory", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public  ActionResult FetchStockDistributionByDepartment(string sDepartmentId)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                  
                    objViewModel.listockdistribution = (List<INVENT_STOCK_DISTRIBUTION>)CMSHandler.FetchData<INVENT_STOCK_DISTRIBUTION>(new INVENT_STOCK_DISTRIBUTION() { DEP_ID= sDepartmentId }, InventorySQL.GetInventorySQL(InventoryEnumCommands.FetchStockDistributionByDepId), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchStockDistributionByDepartment", "Inventory", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchStockDistributionByDepartment", "Inventory", ex.Message);
                }
            }
            return View(objViewModel);
        }
        #endregion
        #region Inventory Reports
        #endregion
    }
}