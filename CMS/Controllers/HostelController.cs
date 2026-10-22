using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using CMS.ViewModel.ViewModel;
using CMS.Utility;
using CMS.ViewModel.Model;
using Newtonsoft.Json;
using CMS.Models;
using CMS.SQL.Academics;
using CMS.DAO;
using System.IO;
using CMS.SQL.Hostel;
using CMS.SQL.Fee;

namespace CMS.Controllers
{
    public class HostelController : Controller
    {
        // GET: Hostel
        public ActionResult Index()
        {
            return View();
        }

        #region Hostel Registration
        // Hostel Registration Main Page ....
        public ActionResult HostelRegistration()
        {
            HostelViewModel ObjViewModel = new HostelViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sUserId))
            {
                try
                {
                    ObjViewModel.liStudentInfo = (List<STUDENT_PERSONAL_INFO>)CMSHandler.FetchData<STUDENT_PERSONAL_INFO>(new STUDENT_PERSONAL_INFO() { STUDENT_ID = sUserId }, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.FetchStudentList),sAcademicYear).DataSource.Data;
                    return View(ObjViewModel);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("HostelController", "HostelRegistration", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("HostelController", "HostelRegistration", ex.Message);
                        return View();
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "error");
            }

        }
        // Fetch Student Details ....
        public JsonResult FetchHostelRegistrationById()
        {
            JsonResultData ObjJsonData = new JsonResultData();
            HostelViewModel ObjViewModel = new HostelViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(sUserId))
                {
                    var liStudent = (List<STUDENT_PERSONAL_INFO>)CMSHandler.FetchData<STUDENT_PERSONAL_INFO>(new STUDENT_PERSONAL_INFO() { STUDENT_ID = sUserId }, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.FetchStudentList),sAcademicYear).DataSource.Data;
                    return Json(liStudent, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("HostelController", "FetchHostelRegistrationById", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("HostelController", "FetchHostelRegistrationById", ex.Message);
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
        }
        // FetchRelationDetails ....
        public ActionResult FetchRelationsInfo()
        {
            JsonResultData objResult = new JsonResultData();
            var ObjHostelModel = new HostelViewModel();
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(sUserId))
                {
                    ObjHostelModel.liStuRelation = (List<STU_RELATIONS>)CMSHandler.FetchData<STU_RELATIONS>(new STU_RELATIONS() { STUDENT_ID = sUserId }, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.FetchStudentRelationsByStudentId)).DataSource.Data;
                    ObjHostelModel.liRelation = (List<SUP_RELATION>)CMSHandler.FetchData<SUP_RELATION>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchRelaion)).DataSource.Data;
                    ObjHostelModel.liOccupation = (List<Sup_Occupation>)CMSHandler.FetchData<Sup_Occupation>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOccupation)).DataSource.Data;
                    return View(ObjHostelModel);
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("HostelController", "FetchRelationsInfo", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("HostelController", "FetchRelationsInfo", ex.Message);
                    return View();
                }
            }
        }
        // Save Personal Info ....
        public JsonResult SaveOrUpdateHostelPersonalInfo()
        {
            JsonResultData ObjResult = new JsonResultData();
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

                string filePath = string.Empty;
                string isSubmitClick = string.Empty;
                string sImagePath = string.Empty;
                string _sDirectorypath = string.Empty;
                string ProgrammeCode = string.Empty;
                string sClassCode = string.Empty;
                var sHostelPersonal = Request.Form["sHostelPersonal"];
                var sStudentId = Request.Form["Student_Id"];
                string sAdmissionNo = Request.Form["sAdmission_No"];
                //var liProgram = (List<FetchClassAndProgram>)CMSHandler.FetchData<FetchClassAndProgram>(new FetchClassAndProgram() { STUDENT_ID = sStudentId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassAndProgramByStudentId)).DataSource.Data;
                //if (liProgram != null && liProgram.Count > 0)
                //{
                //    ProgrammeCode = liProgram.FirstOrDefault().PROGRAMME_CODE;
                //    sClassCode = liProgram.FirstOrDefault().CLASS_CODE;
                //    sClassCode = sClassCode.Replace(" ","-");
                //}
                //_sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.STUDENT_HOSTEL_PATH + "\\" + sAcademicYear + "\\" + ProgrammeCode + "\\" + sClassCode;
                //if (!Directory.Exists(_sDirectorypath))
                //{
                //    Directory.CreateDirectory(_sDirectorypath);
                //}


                if (!string.IsNullOrEmpty(sUserId))
                {
                    var Result = JsonConvert.DeserializeObject<STUDENT_PERSONAL_INFO>(sHostelPersonal);

                    //if (Request.Files.Count > 0)
                    //{
                    //    var File = Request.Files[0];
                    //    string ImgName = File.FileName;
                    //    var Img = ImgName.Split('.');
                    //    string fileName = ImgName.Replace(Img[0], sAdmissionNo);

                    //    if (!string.IsNullOrEmpty(fileName))
                    //    {
                    //        filePath = _sDirectorypath + "\\" + fileName;
                    //        sImagePath = "\\" + Common.URLPages.STUDENT_HOSTEL_PATH + "\\" + sAcademicYear + "\\" + ProgrammeCode + "\\" + sClassCode + "\\" + fileName;
                    //        File.SaveAs(filePath);
                    //        Result.PHOTO= sImagePath;
                    //        var PhotoUpload = CMSHandler.SaveOrUpdate(Result, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.UploadStudentImageByStudentId));
                    //    }
                    //}


                    //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    var sresultArgs = CMSHandler.SaveOrUpdate(Result, SQL.Hostel.HostelSQL.GetHostelSQL((!string.IsNullOrEmpty(Result.RENEWAL_REGISTRATION_ID)) ? HostelSQLCommands.UpdateHostelRegistration : HostelSQLCommands.SaveHostelRegistration), sAcademicYear, true);
                    if (sresultArgs.Success)
                    {
                        if (string.IsNullOrEmpty(Result.RENEWAL_REGISTRATION_ID))
                            ObjResult.sResult = sresultArgs.RowUniqueId.ToString();
                        Result.DATE_OF_BIRTH = CommonMethods.ConvertdatetoyearFromat(Result.DATE_OF_BIRTH);
                        var SaveStudentPersonal = CMSHandler.SaveOrUpdate(Result, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.UpdateStuPersonalByStudentId), sAcademicYear);
                        if (SaveStudentPersonal.Success)
                        {
                            if (!string.IsNullOrEmpty(Result.C_DOOR_DETAIL))
                            {
                                var SaveStudentAddress = CMSHandler.SaveOrUpdate(Result, SQL.Student.StudentSQL.GetStudentSQL((!string.IsNullOrEmpty(Result.ADDRESS_ID)) ? StudentSQLCommads.UpdateAddress : StudentSQLCommads.InsertStudentAddressDetails), sAcademicYear, true);
                                if (string.IsNullOrEmpty(Result.ADDRESS_ID))
                                {
                                    ObjResult.ErrorCode = SaveStudentAddress.RowUniqueId.ToString();
                                }

                                ObjResult.Message = (SaveStudentAddress.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            }
                            else
                            {
                                ObjResult.Message = Common.Messages.RecordsSavedSuccessfully;
                            }

                        }
                    }
                    else
                        ObjResult.Message = Common.Messages.FailedToSaveRecords;
                }
                else
                {
                    ObjResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("HostelController", "SaveOrUpdateHostelPersonalInfo", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("HostelController", "SaveOrUpdateHostelPersonalInfo", ex.Message);
                }

            }
            return Json(ObjResult, JsonRequestBehavior.AllowGet);
        }
        // Save Family Details ....
        public JsonResult SaveOrUpdateFamilyDetails(string sRelationJson, string sIsSubmitted)
        {
            JsonResultData objResult = new JsonResultData();
            var ObjHostelModel = new HOSTEL_REGISTRATION();
            string FrequencyId = string.Empty;
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(sUserId))
                {
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    HOSTEL_JSON_MODEL objJson = serializer.Deserialize<HOSTEL_JSON_MODEL>(sRelationJson);
                    if (objJson.liRelation != null && objJson.liRelation.Count > 0)
                    {
                        foreach (var item in objJson.liRelation)
                        {
                            var sresultArgs = CMSHandler.SaveOrUpdate(item, SQL.Hostel.HostelSQL.GetHostelSQL((!string.IsNullOrEmpty(item.RELATION_ID)) ? HostelSQLCommands.UpdateStudentRelation : HostelSQLCommands.InsertStudentRelation), sAcademicYear, true);
                            objResult.Message = (sresultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : objResult.Message = Common.Messages.FailedToSaveRecords;
                        }
                        if (sIsSubmitted.Equals("1"))
                        {
                            ObjHostelModel.STATUS = Common.STATUS.Submitted;
                            ObjHostelModel.STUDENT_ID = sUserId;
                            ObjHostelModel.IS_SUBMITTED = Common.IsActiveFalg.IsActive;
                            var FetchHostelInfo = (List<HOSTEL_REGISTRATION>)CMSHandler.FetchData<HOSTEL_REGISTRATION>(new HOSTEL_REGISTRATION() { STUDENT_ID = sUserId }, HostelSQL.GetHostelSQL(HostelSQLCommands.FetchHostelInfoByStudentId), sAcademicYear).DataSource.Data;
                            if (FetchHostelInfo != null && FetchHostelInfo.Count > 0)
                            {
                                ObjHostelModel.RENEWAL_REGISTRATION_ID = FetchHostelInfo.FirstOrDefault().RENEWAL_REGISTRATION_ID;
                                var UpdateSubmitted = CMSHandler.SaveOrUpdate(ObjHostelModel, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.UpdateApplicationSubmittionForHostel));
                                if (UpdateSubmitted.Success)
                                {
                                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.HostelFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                                        FrequencyId = FetchFrequency.FirstOrDefault().FREQUENCY_ID;
                                    var FetchStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = sUserId, FREQUENCY_ID = FrequencyId }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.CheckIsExistingAccount), sAcademicYear).DataSource.Data;
                                    if (FetchStudentAccount == null)
                                    {
                                        //SaveStudentAccount(sUserId, FrequencyId);
                                    }
                                    //var FetchFeeStudentAccount = (List<ADM_ISSUE_APPLICATION_2018>)CMSHandler.FetchData<ADM_ISSUE_APPLICATION_2018>(new ADM_ISSUE_APPLICATION_2018() { ISSUE_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, SQL.Admission.AdmissionSQL.GetAdmissionSQL(AdmissionSQLCommands.FetchIssueApplicationById), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                                    //if (FetchFeeStudentAccount != null && FetchFeeStudentAccount.Count > 0)
                                    //{
                                    //    sContent = "Dear " + FetchFeeStudentAccount.FirstOrDefault().FIRST_NAME + ",\n\nGreetings,\n Your Application has been submitted Successfully and the submitted Application No is  " + FetchFeeStudentAccount.FirstOrDefault().APPLICATION_NO + ". \n Note:\n If your application is selected you will receive a message and email for your confirmation.  \nBest Wishes,\nAdmission Team,\nHoly Cross College,Trichy.\nPh:0431-2700637. ";
                                    //    SendEmailAndTextMessage(sContent, FetchFeeStudentAccount.FirstOrDefault().CONTACT_NO, FetchFeeStudentAccount.FirstOrDefault().EMAIL_ID, "Application is submitted successfully.");
                                    //}
                                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                    objResult.Message = Common.Messages.FailedToSaveRecords;
                            }
                            else
                                objResult.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.ErrorMessage.ExpectationFailed;
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
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("HostelController", "SaveOrUpdateFamilyDetails", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("HostelController", "SaveOrUpdateFamilyDetails", ex.Message);
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        // Delete Student Relation ....
        public JsonResult DeleteStudentRelation(string sRelationId)
        {
            JsonResultData objResult = new JsonResultData(); SUP_RELATION ObjModel = new SUP_RELATION();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sUserId))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sRelationId))
                    {
                        ObjModel.RELATION_ID = sRelationId;
                        var sresultArgs = CMSHandler.SaveOrUpdate(ObjModel, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.DeleteStudentRelation));
                        objResult.Message = (sresultArgs.Success) ? Common.ErrorCode.Ok : objResult.Message = Common.Messages.FailedToSaveRecords;
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.ErrorMessage.ExpectationFailed;
                        return Json(objResult, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("HostelController", "DeleteStudentRelation", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("HostelController", "DeleteStudentRelation", ex.Message);
                        return Json(objResult, JsonRequestBehavior.AllowGet);
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
        // Save Student Relation Photo ...
        public JsonResult SaveOrUpdatedPhotoUploads()
        {
            JsonResultData objResult = new JsonResultData();
            var ObjHostelModel = new HOSTEL_REGISTRATION();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(sUserId))
                {
                    string filePath = string.Empty;
                    string isSubmitClick = string.Empty;
                    string sImagePath = string.Empty;
                    string _sDirectorypath = string.Empty;
                    string sContent = string.Empty;
                    string ProgrammeCode = string.Empty;
                    string sClassCode = string.Empty;
                    ObjHostelModel.IS_SUBMITTED = Request.Form["IS_SUBMITTED"];
                    isSubmitClick = Request.Form["Is_Submit_Click"];
                    string sAdmissionNo = Request.Form["sAdmission_No"];
                    var sStudentId = Request.Form["Student_Id"];
                    ObjHostelModel.STUDENT_ID = sStudentId;
                    var liProgram = (List<FetchClassAndProgram>)CMSHandler.FetchData<FetchClassAndProgram>(new FetchClassAndProgram() { STUDENT_ID = sStudentId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassAndProgramByStudentId)).DataSource.Data;
                    if (liProgram != null && liProgram.Count > 0)
                    {
                        ProgrammeCode = liProgram.FirstOrDefault().PROGRAMME_CODE;
                        sClassCode = liProgram.FirstOrDefault().CLASS_CODE;
                        sClassCode = sClassCode.Replace(" ", "-");
                    }
                    _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.STUDENT_HOSTEL_PATH + "\\" + sAcademicYear + "\\" + ProgrammeCode + "\\" + sClassCode;
                    //if (!Directory.Exists(_sDirectorypath))
                    //{
                    //    Directory.CreateDirectory(_sDirectorypath);
                    //}
                    if (isSubmitClick.Equals("1"))
                    {
                        ObjHostelModel.STATUS = Common.STATUS.Submitted;
                        var UpdateSubmitted = CMSHandler.SaveOrUpdate(ObjHostelModel, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.UpdateApplicationSubmittionForHostel));
                        if (UpdateSubmitted.Success)
                        {
                            //var FetchFeeStudentAccount = (List<ADM_ISSUE_APPLICATION_2018>)CMSHandler.FetchData<ADM_ISSUE_APPLICATION_2018>(new ADM_ISSUE_APPLICATION_2018() { ISSUE_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, SQL.Admission.AdmissionSQL.GetAdmissionSQL(AdmissionSQLCommands.FetchIssueApplicationById), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                            //if (FetchFeeStudentAccount != null && FetchFeeStudentAccount.Count > 0)
                            //{
                            //    sContent = "Dear " + FetchFeeStudentAccount.FirstOrDefault().FIRST_NAME + ",\n\nGreetings,\n Your Application has been submitted Successfully and the submitted Application No is  " + FetchFeeStudentAccount.FirstOrDefault().APPLICATION_NO + ". \n Note:\n If your application is selected you will receive a message and email for your confirmation.  \nBest Wishes,\nAdmission Team,\nHoly Cross College,Trichy.\nPh:0431-2700637. ";
                            //    SendEmailAndTextMessage(sContent, FetchFeeStudentAccount.FirstOrDefault().CONTACT_NO, FetchFeeStudentAccount.FirstOrDefault().EMAIL_ID, "Application is submitted successfully.");
                            //}
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        }
                        else
                        {
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }
                    //if (Request.Files.Count > 0)
                    //{
                    //        var File = Request.Files[0];
                    //        string ImgName = File.FileName;
                    //        var Img = ImgName.Split('.');
                    //        string fileName = ImgName.Replace(Img[0], sAdmissionNo + "_FAMILY");

                    //        if (!string.IsNullOrEmpty(fileName))
                    //        {
                    //            filePath = _sDirectorypath + "\\" + fileName;
                    //            sImagePath = "\\" + Common.URLPages.STUDENT_HOSTEL_PATH + "\\" + sAcademicYear + "\\" + ProgrammeCode + "\\" + sClassCode + "\\" + fileName;
                    //            File.SaveAs(filePath);
                    //            ObjHostelModel.FAMILY_PHOTO = sImagePath;
                    //            var resultArgs = CMSHandler.SaveOrUpdate(ObjHostelModel, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.UpdateFamilyPhoto), sAcademicYear);
                    //            objResult.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    //        }
                    //}
                    //else
                    //    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
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
                    objHandler.WriteError("HostelController", "SaveOrUpdatedPhotoUploads", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("HostelController", "SaveOrUpdatedPhotoUploads", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create Fee Account For Hostel

        #endregion
        #region Hostel Application Print View
        public ActionResult HostelRegistrationPrintView()
        {
            HostelViewModel ObjViewModel = new HostelViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sUserId))
            {
                try
                {
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    ObjViewModel.liStuRelation = (List<STU_RELATIONS>)CMSHandler.FetchData<STU_RELATIONS>(new STU_RELATIONS() { STUDENT_ID = sUserId }, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.FetchStudentRelationsByStudentId)).DataSource.Data;
                    ObjViewModel.liStudentInfo = (List<STUDENT_PERSONAL_INFO>)CMSHandler.FetchData<STUDENT_PERSONAL_INFO>(new STUDENT_PERSONAL_INFO() { STUDENT_ID = sUserId }, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.FetchStudentListForPrint), sAcademicYear).DataSource.Data;
                    return View(ObjViewModel);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("HostelController", "HostelRegistrationPrintView", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("HostelController", "HostelRegistrationPrintView", ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }
        #endregion
        #region Hostel Fee Payment
        public ActionResult PayHostelFee()
        {
            List<FEE_STUDENT_ACCOUNT> studentAcountList = new List<FEE_STUDENT_ACCOUNT>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.HostelFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                if (FetchFrequency != null && FetchFrequency.Count > 0)
                {
                    studentAcountList = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT()
                    { STUDENT_ID = sUserId, FREQUENCY_TYPE_ID = Common.FrequencyType.HostelFee, FEE_ROOT_ID = Common.FeeRoot.HostelFee }, 
                        SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FecthMessFeesByStudentIdAndFeeRootId), sAcademicYear).DataSource.Data;

                    if (studentAcountList != null && studentAcountList.Count > 0)
                    {
                        studentAcountList = studentAcountList
                            .OrderBy(s => (Convert.ToDouble(string.IsNullOrEmpty(s.BALANCE) ? "0" : s.BALANCE) <= 0 ? 1 : 0))
                            .ThenBy(s => s.HEAD)
                            .ToList();
                    }
                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(studentAcountList);
        }
        public ActionResult PaymentForHostelFee(string[] selectedHeads)
        {
            PayUPost objPayUPost = new PayUPost();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.HostelFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeMode), sAcademicYear).DataSource.Data;
                if (FetchFrequency != null && FetchFrequency.Count > 0)
                {
                    var FetchStudentAccount = (List<FEE_STUDENT_ACCOUNT>)CMSHandler.FetchData<FEE_STUDENT_ACCOUNT>(new FEE_STUDENT_ACCOUNT()
                    { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), FREQUENCY_TYPE_ID = Common.FrequencyType.HostelFee, FEE_ROOT_ID = Common.FeeRoot.HostelFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FecthMessFeesByStudentIdAndFeeRootId), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                    
                    if (FetchStudentAccount != null && FetchStudentAccount.Count > 0)
                    {
                        if (selectedHeads != null && selectedHeads.Length > 0)
                        {
                            FetchStudentAccount = FetchStudentAccount.Where(s => selectedHeads.Contains(s.HEAD)).ToList();
                        }

                        var person = FetchStudentAccount.FirstOrDefault();
                        var payUPaymentInfo = (List<FEE_MERCHANT_ACCOUNT_INFO>)CMSHandler.FetchData<FEE_MERCHANT_ACCOUNT_INFO>(new FEE_MERCHANT_ACCOUNT_INFO() { ACCOUNT_TYPE = Common.FrequencyType.MessFee, API_TYPE = Common.PayUMoneyAPIsType.BaseUrl }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMerchantInfo)).DataSource.Data;
                        if (payUPaymentInfo != null && payUPaymentInfo.Count > 0)
                        {
                            objPayUPost.productinfo = "Hostel Fee";
                            objPayUPost.key = payUPaymentInfo.FirstOrDefault().KEY;
                            objPayUPost.salt = payUPaymentInfo.FirstOrDefault().SALT;
                            objPayUPost.udf1 = person.STUDENT_ID;
                            objPayUPost.email = person.STU_EMAILID;
                            objPayUPost.udf5 = Common.FeeRoot.HostelFee;
                            objPayUPost.firstname = person.FIRST_NAME;
                            objPayUPost.udf3 = FetchFrequency.FirstOrDefault().FREQUENCY_ID;
                            objPayUPost.udf4 = person.REGISTER_NO;
                            objPayUPost.udf2 = Common.FrequencyType.HostelFee;
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
        private string GetHashValue(string hashSeq, string sMerchant_key, PayUPost objPost)
        {
            string hash = string.Empty;
            string hash_string = string.Empty;
            string[] hashVarsSeq = hashSeq.Split('|'); // spliting hash sequence from config
            foreach (string hash_var in hashVarsSeq)
            {
                if (hash_var=="key")
                {
                    hash_string = hash_string + sMerchant_key;
                    hash_string = hash_string + '|';
                }
                else if (hash_var=="txnid")
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
            hash = CommonMethods.Generatehash512(hash_string).ToLower();//generating hash
            return hash;
        }
        [HttpGet]
        public JsonResult GetHostelFeeReceipt(string transactionId)
        {
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString() ?? "";
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString() ?? "";

                if (string.IsNullOrEmpty(sUserId) || string.IsNullOrEmpty(transactionId))
                {
                    return Json(new { status = false, Message = "Invalid request or session expired." }, JsonRequestBehavior.AllowGet);
                }

                // 1. Fetch College Details
                var collegeDetailsList = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(
                    null, 
                    SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), 
                    sAcademicYear).DataSource.Data;
                var collegeDetails = collegeDetailsList?.FirstOrDefault();

                // 2. Fetch Student Info
                var studentInfoList = DapperHandler.FetchAll<dynamic>(
                    "SELECT FIRST_NAME, LAST_NAME, REGISTER_NO, ROLL_NO FROM STU_PERSONAL_INFO WHERE STUDENT_ID = @STUDENT_ID", 
                    new { STUDENT_ID = sUserId });
                var student = studentInfoList?.FirstOrDefault();

                // 3. Fetch Transaction Info
                var transactionList = DapperHandler.FetchAll<dynamic>(
                    "SELECT TRANSACTION_ID, RECEIPT_NO, COLLECTED, DATE_FORMAT(PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE, PAYMENT_MODE, CF_ORDER_ID FROM FEE_TRANSACTION WHERE TRANSACTION_ID = @TRANSACTION_ID",
                    new { TRANSACTION_ID = transactionId });
                var transaction = transactionList?.FirstOrDefault();

                if (transaction == null)
                {
                    return Json(new { status = false, Message = "Transaction details not found." }, JsonRequestBehavior.AllowGet);
                }

                // 4. Fetch Fee Breakdowns for this transaction (FEE_COLLECTION joining SUP_HEAD)
                string breakdownSql = @"
                    SELECT 
                        FC.PAID_AMOUNT, 
                        H.HEAD AS HEAD_NAME
                    FROM 
                        FEE_COLLECTION FC
                    INNER JOIN 
                        SUP_HEAD H ON H.HEAD_ID = FC.HEAD
                    WHERE 
                        FC.TRANSACTION_ID = @TRANSACTION_ID 
                        AND FC.IS_DELETED != 1";
                var breakdowns = DapperHandler.FetchAll<dynamic>(breakdownSql, new { TRANSACTION_ID = transactionId });

                return Json(new {
                    status = true,
                    college = new {
                        name = collegeDetails?.COLLEGE_NAME,
                        address = collegeDetails?.COLLEGE_ADDRESS,
                        phone = "",
                        email = ""
                    },
                    student = new {
                        name = student != null ? (Convert.ToString(student.FIRST_NAME) + " " + Convert.ToString(student.LAST_NAME)).Trim() : "",
                        registerNo = student != null ? Convert.ToString(student.REGISTER_NO) : "",
                        rollNo = student != null ? Convert.ToString(student.ROLL_NO) : ""
                    },
                    payment = new {
                        transactionId = Convert.ToString(transaction.TRANSACTION_ID),
                        receiptNo = Convert.ToString(transaction.RECEIPT_NO),
                        amount = Convert.ToString(transaction.COLLECTED),
                        date = Convert.ToString(transaction.PAYMENT_DATE),
                        mode = Convert.ToString(transaction.PAYMENT_MODE),
                        orderId = Convert.ToString(transaction.CF_ORDER_ID)
                    },
                    items = breakdowns
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("HostelController", "GetHostelFeeReceipt", ex.Message);
                }
                return Json(new { status = false, Message = "An error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Create Account
        public string SaveStudentAccount(string StudentId, string FrequencyId)
        {
            string sResult;
            ResultArgs sresultArgs = new ResultArgs();
            try
            {
                StuPersonalInfo student = new StuPersonalInfo();
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                student.TYPE = Common.FrequencyType.HostelFee;
                student.STUDENT_ID = StudentId;
                student.FREQUENCY = FrequencyId;
                var HeadsWithAmount = (List<FEE_STRUCTURE>)CMSHandler.FetchData<FEE_STRUCTURE>(student, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStructureForStudentAccount), sAcademicYear).DataSource.Data;
                var StudentInfo = (List<STUDENT_PERSONAL_INFO>)CMSHandler.FetchData<STUDENT_PERSONAL_INFO>(new STUDENT_PERSONAL_INFO() { STUDENT_ID = StudentId }, SQL.Student.StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentInfo), sAcademicYear).DataSource.Data;
                if (HeadsWithAmount != null && HeadsWithAmount.Count > 0)
                {
                    foreach (var item in HeadsWithAmount)
                    {
                        item.ACADEMIC_YEAR = sAcademicYear;
                        item.FREQUENCY_ID = item.FREQUENCY;
                        item.CREDIT = item.AMOUNT;
                        sresultArgs = CMSHandler.SaveOrUpdate(item, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.SaveExamCreditAmountToStudentAccount), sAcademicYear);
                        if (sresultArgs.Success)
                            sResult = Common.Messages.RecordsSavedSuccessfully;
                        else
                            sResult = Common.Messages.FailedToSaveRecords;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("HostelController", "SaveStudentAccount", "Err MSg: " + ex.Message, "Query is empty!");
                }
                sResult = Common.Messages.FailedToSaveRecords;
            }
            return StudentId;
        }
        #endregion
        #region Hostel Registration Print View
        public ActionResult HostelRegistrationPrint()
        {
            HostelViewModel ObjViewModel = new HostelViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sUserId))
            {
                try
                {
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    ObjViewModel.liStuRelation = (List<STU_RELATIONS>)CMSHandler.FetchData<STU_RELATIONS>(new STU_RELATIONS() { STUDENT_ID = sUserId }, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.FetchStudentRelationsByStudentId)).DataSource.Data;
                    ObjViewModel.liStudentInfo = (List<STUDENT_PERSONAL_INFO>)CMSHandler.FetchData<STUDENT_PERSONAL_INFO>(new STUDENT_PERSONAL_INFO() { STUDENT_ID = sUserId }, SQL.Hostel.HostelSQL.GetHostelSQL(HostelSQLCommands.FetchStudentListForPrint), sAcademicYear).DataSource.Data;
                    return View(ObjViewModel);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("HostelController", "HostelRegistrationPrint", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("HostelController", "HostelRegistrationPrint", ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }
        #endregion

        #region Hostel Add / Edit Students
        public ActionResult ListHostelStudents()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new HostelViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liHostelStudentList = (List<HOSTEL_STUDENT_INFO>)CMSHandler.FetchData<HOSTEL_STUDENT_INFO>(null,
                            HostelSQL.GetHostelSQL(HostelSQLCommands.ListHostelStudents),sAcademicYear).DataSource.Data;

                    // Class List ...
                    var liClass = (List<ClassList>)CMSHandler.FetchData<ClassList>(null,SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses),sAcademicYear).DataSource.Data;
                    if (liClass!=null && liClass.Count>0)
                    {
                        ObjViewModel.ClassList = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }

                    // Year List ...
                    var liYear= (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    if (liYear != null && liYear.Count > 0)
                    {
                        ObjViewModel.YearList = new SelectList(liYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    // Student List ...
                    //var liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByAcademicYear), sAcademicYear).DataSource.Data;
                    //if (liStudent != null && liStudent.Count > 0)
                    //{
                    //    ObjViewModel.StudentList = new SelectList(liStudent, Common.STU_PERSONAL_INFO.STUDENT_ID, Common.STU_PERSONAL_INFO.LAST_NAME );
                    //}

                    // Hostel List  ...
                    var liHostel = (List<SUP_HOSTEL>)CMSHandler.FetchData<SUP_HOSTEL>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHostelDDL), sAcademicYear).DataSource.Data;
                    if (liHostel != null && liHostel.Count > 0)
                    {
                        ObjViewModel.HostelList = new SelectList(liHostel, Common.SUP_HOSTEL.HOSTEL_ID, Common.SUP_HOSTEL.HOSTEL_NAME);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog=new ErrorLog())
                    {
                        ObjLog.WriteError("HostelController", "ListHostelStudents", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage","Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult FetchHostelStudentById(string sStuHostelId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liHostelStudents = (List<HOSTEL_STUDENT_INFO>)CMSHandler.FetchData<HOSTEL_STUDENT_INFO>(new HOSTEL_STUDENT_INFO() { STU_HOSTEL_ID=sStuHostelId},
                            HostelSQL.GetHostelSQL(HostelSQLCommands.FetchHostelStudentInfoById),sAcademicYear).DataSource.Data;
                    if (liHostelStudents!=null && liHostelStudents.Count>0)
                    {
                        return Json(liHostelStudents,JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("HostelController", "FetchHostelStudentById", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData,JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchBlockListByHostelId(string sHostelId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liHostelStudents = (List<SUP_BLOCK>)CMSHandler.FetchData<SUP_BLOCK>(new SUP_BLOCK() { HOSTEL_ID=sHostelId},
                            HostelSQL.GetHostelSQL(HostelSQLCommands.GetBlockByHostelId), sAcademicYear).DataSource.Data;
                    if (liHostelStudents!=null && liHostelStudents.Count>0)
                    {
                        foreach (var item in liHostelStudents)
                        {
                            ObjJsonData.sResult += "<option value=" + item.BLOCK_ID + ">" + item.BLOCK_NAME + "</option>";
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("HostelController", "FetchBlockListByHostelId", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveHostelStudent(string sJsonData)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<HOSTEL_STUDENT_INFO>(sJsonData);
                    Result.ADMITTED_DATE = CommonMethods.ConvertdatetoyearFromat(Result.ADMITTED_DATE);
                   
                    var IsExist = (List<HOSTEL_STUDENT_INFO>)CMSHandler.FetchData<HOSTEL_STUDENT_INFO>(Result,
                            HostelSQL.GetHostelSQL(HostelSQLCommands.IsExistHostelStudent),sAcademicYear).DataSource.Data;
                    if (IsExist!=null && IsExist.Count>0)
                    {
                        // Update ...
                        var sUpdateHostelStudentInfo = CMSHandler.SaveOrUpdate(Result,HostelSQL.GetHostelSQL(HostelSQLCommands.UpdateHostelStudent),sAcademicYear);
                        if (sUpdateHostelStudentInfo.Success)
                        {
                            ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message= Common.Messages.FailedToUpdatedRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                       // ObjJsonData.Message = (sUpdateHostelStudentInfo.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    }
                    else
                    {
                        // Insert ...
                        var sSaveHostelStudentIndo = CMSHandler.SaveOrUpdate(Result, HostelSQL.GetHostelSQL(HostelSQLCommands.SaveHostelStudent), sAcademicYear);
                        if (sSaveHostelStudentIndo.Success)
                        {
                            ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                        //ObjJsonData.Message = (sSaveHostelStudentIndo.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("HostelController", "SaveHostelStudent", ex.Message);
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

        public JsonResult DeleteHostelStudent(string sStuHostelId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var sDeleteHostelStudent = CMSHandler.SaveOrUpdate(new HOSTEL_STUDENT_INFO() { STU_HOSTEL_ID=sStuHostelId},
                            HostelSQL.GetHostelSQL(HostelSQLCommands.DeleteHostelStudent),sAcademicYear);
                    ObjJsonData.Message = (sDeleteHostelStudent.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("HostelController", "DeleteHostelStudent", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Hostel StudentPromotion
        public ActionResult HostelStudentPromotion()
        {
            HostelViewModel objViewModel = new HostelViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    var AcademicYearList= (List<SUP_ACADEMIC_YEAR_LIST>)CMSHandler.FetchData<SUP_ACADEMIC_YEAR_LIST>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    objViewModel.AcademicYear = new SelectList(AcademicYearList, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    var ListHostel= (List<SUP_HOSTEL>)CMSHandler.FetchData<SUP_HOSTEL>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHostelDDL)).DataSource.Data;
                    objViewModel.HostelList= new SelectList(ListHostel, Common.SUP_HOSTEL.HOSTEL_ID, Common.SUP_HOSTEL.HOSTEL_NAME);
                    var ClassYearList = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data;
                    objViewModel.YearList = new SelectList(ClassYearList, Common.SubClassYear.CLASS_YEAR_ID, Common.SubClassYear.CLASS_YEAR);

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("HostelController", "HostelStudentPromotion", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("HostelController", "HostelStudentPromotion", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindStudentForPromotion(string sClassyear,string sAcyear,string sHostel)
        {
            HostelViewModel objViewModel = new HostelViewModel();
            string sSql = string.Empty;
            //int sCurrentYear = 0;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                   // sCurrentYear = Convert.ToInt32(Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]) - Convert.ToInt32(Common.IsActiveFalg.IsActive);
                    if (!string.IsNullOrEmpty(sClassyear))
                    {
                        sSql = HostelSQL.GetHostelSQL(HostelSQLCommands.FetchHostelStudentByClassYear).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_YEAR, sClassyear);
                        var StudentList = (List<HOSTEL_STUDENT_INFO>)CMSHandler.FetchData<HOSTEL_STUDENT_INFO>(new HOSTEL_STUDENT_INFO() { CLASS_YEAR = sClassyear, ACADEMIC_YEAR =sAcyear,HOSTEL_ID=sHostel }, sSql).DataSource.Data;
                        if (StudentList != null && StudentList.Count > 0)
                        {
                            objViewModel.liHostelStudentList = StudentList;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("HostelController", "BindStudentForPromotion", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("HostelController", "BindStudentForPromotion", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }
        public JsonResult SaveorUpdateHostelPromotion(string sStudent)
        {
            JsonResultData objJsonData = new JsonResultData();
            HOSTEL_STUDENT_INFO objModel = new HOSTEL_STUDENT_INFO();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                JsonStudentPromotion objJson = JsonConvert.DeserializeObject<JsonStudentPromotion>(sStudent);
                string Exists = string.Empty;

                if (objJson != null)
                {
                    foreach (var item in objJson.StudentPromotionList)
                    {
                        objModel.STUDENT_ID = item.STUDENT_ID;
                        objModel.ADMITTED_DATE = sAcademicYear;
                        objModel.HOSTEL_ID = item.HOSTEL_ID;
                        objModel.ACADEMIC_YEAR = item.ACADEMIC_YEAR;
                        objModel.ADMITTED_YEAR = item.ACADEMIC_YEAR;
                        objModel.NAME = item.NAME;
                        //var IsStuClassPromoted = (List<HOSTEL_STUDENT_INFO>)CMSHandler.FetchData<HOSTEL_STUDENT_INFO>(objModel, HostelSQL.GetHostelSQL(HostelSQLCommands.IsStuClassPromoted), sAcademicYear).DataSource.Data;
                        //if (IsStuClassPromoted != null && IsStuClassPromoted.Count > 0)
                        //{
                        var IsExist = (List<HOSTEL_STUDENT_INFO>)CMSHandler.FetchData<HOSTEL_STUDENT_INFO>(objModel, HostelSQL.GetHostelSQL(HostelSQLCommands.IsStudentExist),item.ACADEMIC_YEAR).DataSource.Data;
                        if (IsExist != null && IsExist.Count > 0)
                        {
                            new MySQLHandler().ExecuteAsScripts(SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                            var Result = CMSHandler.SaveOrUpdate(objModel, HostelSQL.GetHostelSQL(HostelSQLCommands.UpdateStudentPromotion));
                            objJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            new MySQLHandler().ExecuteAsScripts(SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                            //Exists += item.NAME + ',';
                        }
                        else
                        {
                            var Result = CMSHandler.SaveOrUpdate(objModel, HostelSQL.GetHostelSQL(HostelSQLCommands.SaveStudentPromotion), sAcademicYear);
                            if (Result.Success)
                                objJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            else
                                objJsonData.Message = Common.Messages.FailedToSaveRecords;
                        }
                    
                        //}
                       
                        if (!string.IsNullOrEmpty(Exists))
                        {
                            objJsonData.Message = "These are Student Exist Already" + Exists;
                        }
                    }
                }
            }
            return Json(objJsonData, JsonRequestBehavior.AllowGet);

        }
        #endregion


        #region Hostel Fee Details For Warden 

        public ActionResult HostelFeeStudentDetailsForWarden()
        {
            FeeTransactionViewModel objViewModel = new FeeTransactionViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objViewModel.liFeeDueReport = (List<FEE_STUDENT_DUE>)CMSHandler.FetchData<FEE_STUDENT_DUE>(null, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchHostelFeeDetails), sAcademicYear).DataSource.Data;

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
    }
}