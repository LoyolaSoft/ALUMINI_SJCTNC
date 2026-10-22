using CMS.DAO;
using CMS.Models;
using CMS.SQL.Alumnae;
using CMS.SQL.Communication;
using CMS.SQL.Examination;
using CMS.SQL.Placement;
using CMS.SQL.SupportData;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.SQL.UserAccount;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mail;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.Net.Http;
using CMS.SQL.Fee;
using CMS.SQL.Student;
using CMS.SQL.StaffSQL;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
namespace CMS.Controllers
{
    public class AlumnaeController : Controller
    {
           ResultArgs resultArgs = new ResultArgs();
        JsonResultData objJsonData = new JsonResultData();

        private string FetchAcademicYear()
        {
            string sAcademicYear = string.Empty;
            var Academic = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchActiveYear)).DataSource.Data;
            if (Academic != null && Academic.Count > 0)
            {
                sAcademicYear = Academic[0].AC_YEAR;
            }
            return sAcademicYear;
        }

        /// <summary>
        ///    
        /// </summary>
        /// <returns></returns>
        // GET: Alumnae
        public ActionResult Index()
        {
            AlumnaeViewModel objViewModel = new AlumnaeViewModel();
            string sAcademicYear = FetchAcademicYear();


            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //fetch department
                    var Department = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    objViewModel.DepartmentList = new SelectList(Department, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);

                    //fetch all programme
                    var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                           SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammeGroup)).DataSource.Data;
                    if (liProgramme != null && liProgramme.Count > 0)
                    {


                        objViewModel.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    }

                    //fetch country
                    var liCountry = (List<SUP_COUNTRY>)CMSHandler.FetchData<SUP_COUNTRY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCountry)).DataSource.Data;
                    if (liCountry != null && liCountry.Count > 0)
                    {
                        objViewModel.CountryList = new SelectList(liCountry, Common.SUP_COUNTRY.COUNTRY_ID, Common.SUP_COUNTRY.COUNTRY_NAME);
                    }

                    //fetch competitive exam
                    var liCompetitveExam = (List<SUP_COMPETITIVE_EXAM>)CMSHandler.FetchData<SUP_COMPETITIVE_EXAM>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupCompetitiveExam)).DataSource.Data;
                    if (liCompetitveExam != null && liCompetitveExam.Count > 0)
                    {
                        objViewModel.CompetitiveExamList = new SelectList(liCompetitveExam, Common.SUP_COMPETITIVE_EXAM.COMPETITIVE_EXAM_ID, Common.SUP_COMPETITIVE_EXAM.COMPETITIVE_EXAM);
                    }


                    //fetch sup contribution type
                    var liContributionType = (List<SUP_ALU_CONTRIBUTION_TYPE>)CMSHandler.FetchData<SUP_ALU_CONTRIBUTION_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupAluContributionType)).DataSource.Data;
                    if (liContributionType != null && liContributionType.Count > 0)
                    {
                        objViewModel.liSupAluContriputionType = liContributionType;
                    }

                    //fetch bacth
                    var FetchBatch = (List<sup_Batch>)CMSHandler.FetchData<sup_Batch>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchBatch)).DataSource.Data;
                    if (FetchBatch != null && FetchBatch.Count > 0)
                    {
                        objViewModel.Batch = new SelectList(FetchBatch, Common.SUP_BATCHES.BATCH_ID, Common.SUP_BATCHES.BATCH);
                    }


                    //fetch marital status
                    var liMaritalStatus = (List<SupMaritalStatus>)CMSHandler.FetchData<SupMaritalStatus>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMarritalStatus)).DataSource.Data;
                    if (liMaritalStatus != null && liMaritalStatus.Count > 0)
                    {
                        objViewModel.MarritalStatusList = new SelectList(liMaritalStatus, Common.SUP_MARRITAL_STATUS.MARITAL_STAUS_ID, Common.SUP_MARRITAL_STATUS.MARITAL_STATUS_NAME);
                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "Index", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }


        /// <summary>
        ///  this is for saving alumnae registration
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveAluRegistration()
        {
            string sDirectorypath = string.Empty, sFilePath = string.Empty;
            var path = string.Empty;
            int Count = 0;
            JsonResultData objResultData = new JsonResultData();
            try
            {
                var result = JsonConvert.DeserializeObject<ALUMNAE_SAVE_FIELDS>(Request.Form["sJsonData"]);

                string sSQL = string.Empty;
                string sAcademicYear = FetchAcademicYear();
                if (result.objAluRegistration != null && !String.IsNullOrEmpty(sAcademicYear))
                {
                    // add alumnae regirstation
                    // formating date
                    if (!string.IsNullOrEmpty(result.objAluRegistration.DATE_OF_BIRTH))
                    {
                        result.objAluRegistration.DATE_OF_BIRTH = CommonMethods.ConvertdatetoyearFromat(result.objAluRegistration.DATE_OF_BIRTH);
                    }
                    if (!string.IsNullOrEmpty(result.objAluRegistration.VISIT_OUR_CAMPUS))
                    {
                        result.objAluRegistration.VISIT_OUR_CAMPUS = CommonMethods.ConvertdatetoyearFromat(result.objAluRegistration.VISIT_OUR_CAMPUS);
                    }



                    //check if user is existed or not
                    if (UserExistedorNot(result.objAluRegistration.PROGRAMME_ID, result.objAluRegistration.BATCH_BEGIN_YEAR, result.objAluRegistration.EMAIL_ID))
                    {
                        objResultData.Message = Common.Messages.RecordsExistAlready;
                        return Json(objResultData);
                    }



                    path = Request.Form["sPhoto"];
                    if (!string.IsNullOrEmpty(path))
                    {
                        //Photo savings
                        if (Request.Files.Count > 0)
                        {
                            var files = Request.Files;
                            int position = files.Count;


                            sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.ALUMNAE_MEMBER_IMAGE_PATH + "\\";
                            if (!Directory.Exists(sDirectorypath))
                            {
                                Directory.CreateDirectory(sDirectorypath);
                            }
                            var file = Request.Files[position - 1];
                            var fileName = file.FileName;
                            var sFile = fileName.Split('.');
                            sFilePath = sDirectorypath + "\\" + sFile[0] + "_" + result.objAluRegistration.FIRST_NAME + "." + sFile[1];
                            string sSavePath = "\\" + Common.URLPages.ALUMNAE_MEMBER_IMAGE_PATH + "\\" + sFile[0] + "_" + result.objAluRegistration.FIRST_NAME + "." + sFile[1];
                            file.SaveAs(sFilePath);
                            result.objAluRegistration.PHOTO = sSavePath;
                            Count++;

                        }
                    }



                    resultArgs = CMSHandler.SaveOrUpdate(result.objAluRegistration, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAluRegistrationInfo), sAcademicYear);
                    if (resultArgs.Success)
                    {
                        var AluRegistration = (List<ALU_REGISTRATION_INFO>)CMSHandler.FetchData<ALU_REGISTRATION_INFO>(null, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchLastIndexAluRegistrationInfo)).DataSource.Data;
                        if (AluRegistration != null && AluRegistration.Count > 0)
                        {
                            ////add contribution details
                            //result.objAluContributionDetails.ALU_REGISTER_ID = AluRegistration.FirstOrDefault().ALU_REGISTER_ID;
                            //resultArgs = CMSHandler.SaveOrUpdate(result.objAluContributionDetails, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAluContributionDetails));

                            if (result.objAluRegistration.IS_HIGHER == "1")
                            {
                                result.objAluHigherDetails.ALU_REGISTER_ID = AluRegistration.FirstOrDefault().ALU_REGISTER_ID;
                                //check is already alu register id there or not
                                if (!ValidateAluRegistration<ALU_HIGHER_DETAILS>(AlumnaeSQLCommands.FetchAluHigherDetailsByAluRegisterId, result.objAluHigherDetails))
                                {
                                    resultArgs = CMSHandler.SaveOrUpdate(result.objAluHigherDetails, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAluHigheDetails));
                                }


                            }
                            if (result.objAluRegistration.IS_RESEARCH == "1")
                            {
                                result.objAluResearchDetails.ALU_REGISTER_ID = AluRegistration.FirstOrDefault().ALU_REGISTER_ID;
                                //check is already alu register id there or not
                                if (!ValidateAluRegistration<ALU_RESEARCH_DETAILS>(AlumnaeSQLCommands.FetchAluResearchDetailsByAluRegisterId, result.objAluResearchDetails))
                                {
                                    resultArgs = CMSHandler.SaveOrUpdate(result.objAluResearchDetails, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAluResearchDetails));
                                }

                            }
                            if (result.objAluRegistration.IS_PLACEMENT == "1")
                            {
                                result.objAluPlacementDetails.ALU_REGISTER_ID = AluRegistration.FirstOrDefault().ALU_REGISTER_ID;
                                //check is already alu register id there or not
                                if (!ValidateAluRegistration<ALU_PLACEMENT_DETAILS>(AlumnaeSQLCommands.FetchAluPlacementDetailsByAluRegisterId, result.objAluPlacementDetails))
                                {
                                    resultArgs = CMSHandler.SaveOrUpdate(result.objAluPlacementDetails, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAluPlacementDetails));
                                }
                            }
                            if (result.objAluRegistration.IS_ENTREPRENEUR == "1")
                            {
                                result.objAluEntreprenuerDetails.ALU_REGISTER_ID = AluRegistration.FirstOrDefault().ALU_REGISTER_ID;
                                //check is already alu register id there or not
                                if (!ValidateAluRegistration<ALU_ENTREPRENEUR_DETAILS>(AlumnaeSQLCommands.FetchAluEntreprenuerDetailsByAluRegisterId, result.objAluEntreprenuerDetails))
                                {
                                    resultArgs = CMSHandler.SaveOrUpdate(result.objAluEntreprenuerDetails, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAluEntreprenuerDetails));
                                }

                            }


                        }
                    }
                    else
                    {
                        objResultData.Message = Common.Messages.FailedToSaveRecords;
                    }
                    objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    objResultData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;

                }


            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "SaveAluRegistration", ex.Message);
                }
            }
            return Json(objResultData);
        }

        // validate 
        public bool ValidateAluRegistration<T>(AlumnaeSQLCommands sCommand, dynamic Item)
        {
            bool Status = false;
            if (sCommand != null && Item != null)
            {
                var item = (List<ViewModel.Model.sup_Batch>)DAO.CMSHandler.FetchData<ViewModel.Model.sup_Batch>(Item, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(sCommand)).DataSource.Data;
                if (item != null && item.Count > 0)
                {
                    Status = true;
                }
                else
                {
                    Status = false;
                }
            }
            return Status;
        }
        /// <summary>
        /// validate user by programme,batch,and emailid
        /// </summary>
        /// <returns></returns>
        public bool UserExistedorNot(string sProgramme, string sBatchBegin, string sEmailId)
        {
            bool Status = false;
            if (!string.IsNullOrEmpty(sProgramme) && !string.IsNullOrEmpty(sBatchBegin) && !string.IsNullOrEmpty(sEmailId))
            {
                var item = (List<ALU_REGISTRATION_INFO>)DAO.CMSHandler.FetchData<ALU_REGISTRATION_INFO>(new ALU_REGISTRATION_INFO() { PROGRAMME_ID = sProgramme, BATCH_BEGIN_YEAR = sBatchBegin, EMAIL_ID = sEmailId }, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAluRegistrationInfoByProgrammeandBatchandEmail)).DataSource.Data;
                if (item != null && item.Count > 0)
                {
                    Status = true;
                }
                else
                {
                    Status = false;
                }
            }
            return Status;
        }

        #region Report
        [UserRights("ADMIN")]
        public ActionResult AlumnaeReport(string sBatch, string sProgramme, string sDepartment)
        {
            AlumnaeViewModel objViewModel = new AlumnaeViewModel();
            string sAcademicYear = FetchAcademicYear();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //fetch department
                    var Department = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    objViewModel.DepartmentList = new SelectList(Department, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);

                    //fetch all programme
                    var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                           SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                    if (liProgramme != null && liProgramme.Count > 0)
                    {
                        objViewModel.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    }

                    //fetch bacth
                    var FetchBatch = (List<sup_Batch>)CMSHandler.FetchData<sup_Batch>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchBatch)).DataSource.Data;
                    if (FetchBatch != null && FetchBatch.Count > 0)
                    {
                        objViewModel.Batch = new SelectList(FetchBatch, Common.SUP_BATCHES.BATCH_ID, Common.SUP_BATCHES.BATCH);
                    }


                    objViewModel.sBatch = sBatch;

                    objViewModel.sProgramme = sProgramme;

                    objViewModel.sDept = sDepartment;


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "AlumnaeReport", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }
        /// <summary>
        /// load alumnae report based on batch , programme, department
        /// </summary>
        /// <returns></returns>
        public ActionResult BindAlumnaeReport(string sBatch, string sProgramme, string sDepartment)
        {
            AlumnaeViewModel objViewModel = new AlumnaeViewModel();
            string sAcademicYear = FetchAcademicYear();
            string sQL = string.Empty;
            string AluRegisterId = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                objViewModel.liAluRegistrationInfo = (List<ALU_REGISTRATION_INFO>)CMSHandler.FetchData<ALU_REGISTRATION_INFO>(new ALU_REGISTRATION_INFO() { BATCH = sBatch, PROGRAMME_ID = sProgramme, DEPARTMENT = sDepartment },
                SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAluRegistrationInfoByBatchandProgrammeandDepartment), sAcademicYear).DataSource.Data;

                if (objViewModel.liAluRegistrationInfo != null && objViewModel.liAluRegistrationInfo.Count > 0)
                {
                    var IsHigherList = objViewModel.liAluRegistrationInfo.Where(x => x.IS_HIGHER == "1").ToList();
                    var IsResearchList = objViewModel.liAluRegistrationInfo.Where(x => x.IS_RESEARCH == "1").ToList();
                    var IsPlacementList = objViewModel.liAluRegistrationInfo.Where(x => x.IS_PLACEMENT == "1").ToList();
                    var IsEntrpreneurList = objViewModel.liAluRegistrationInfo.Where(x => x.IS_ENTREPRENEUR == "1").ToList();

                    if (IsHigherList != null && IsHigherList.Count > 0)
                    {
                        var AluRegistrationId = IsHigherList.Select(x => x.ALU_REGISTER_ID).ToList();
                        AluRegisterId = string.Empty;
                        foreach (var item in AluRegistrationId)
                        {
                            AluRegisterId += "," + item;
                        }
                        AluRegisterId = AluRegisterId.TrimStart(',');
                        sQL = SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAluHigherDetailsByBulkAluRegistraionId);
                        objViewModel.liAluHigherDetails = (List<ALU_HIGHER_DETAILS>)CMSHandler.FetchData<ALU_HIGHER_DETAILS>(null, sQL.Replace(Common.Delimiter.QUS + Common.ALU_CONTRIBUTION_DETAILS.ALU_REGISTER_ID, AluRegisterId)).DataSource.Data;
                    }
                    if (IsResearchList != null && IsResearchList.Count > 0)
                    {
                        var AluRegistrationId = IsResearchList.Select(x => x.ALU_REGISTER_ID).ToList();
                        AluRegisterId = string.Empty;
                        foreach (var item in AluRegistrationId)
                        {
                            AluRegisterId += "," + item;
                        }
                        AluRegisterId = AluRegisterId.TrimStart(',');
                        sQL = SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAluResearchDetailsByBulkAluRegistraionId);
                        objViewModel.liAluResearchDetails = (List<ALU_RESEARCH_DETAILS>)CMSHandler.FetchData<ALU_RESEARCH_DETAILS>(null, sQL.Replace(Common.Delimiter.QUS + Common.ALU_CONTRIBUTION_DETAILS.ALU_REGISTER_ID, AluRegisterId)).DataSource.Data;
                    }
                    if (IsPlacementList != null && IsPlacementList.Count > 0)
                    {
                        var AluRegistrationId = IsResearchList.Select(x => x.ALU_REGISTER_ID).ToList();
                        AluRegisterId = string.Empty;
                        foreach (var item in AluRegistrationId)
                        {
                            AluRegisterId += "," + item;
                        }
                        AluRegisterId = AluRegisterId.TrimStart(',');
                        sQL = SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAluResearchDetailsByBulkAluRegistraionId);
                        objViewModel.liAluResearchDetails = (List<ALU_RESEARCH_DETAILS>)CMSHandler.FetchData<ALU_RESEARCH_DETAILS>(null, sQL.Replace(Common.Delimiter.QUS + Common.ALU_CONTRIBUTION_DETAILS.ALU_REGISTER_ID, AluRegisterId)).DataSource.Data;
                    }
                    if (IsEntrpreneurList != null && IsEntrpreneurList.Count > 0)
                    {
                        var AluRegistrationId = IsEntrpreneurList.Select(x => x.ALU_REGISTER_ID).ToList();
                        AluRegisterId = string.Empty;
                        foreach (var item in AluRegistrationId)
                        {
                            AluRegisterId += "," + item;
                        }
                        AluRegisterId = AluRegisterId.TrimStart(',');
                        sQL = SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAluEntrepreneurDetailsByBulkAluRegistraionId);
                        objViewModel.liAluEntrepreneurDetails = (List<ALU_ENTREPRENEUR_DETAILS>)CMSHandler.FetchData<ALU_ENTREPRENEUR_DETAILS>(null, sQL.Replace(Common.Delimiter.QUS + Common.ALU_CONTRIBUTION_DETAILS.ALU_REGISTER_ID, AluRegisterId)).DataSource.Data;
                    }
                }
                else
                {
                    objViewModel.Message = Common.Messages.NoRecordsfound;
                }

            }
            return View(objViewModel);
        }

        #endregion

        #region Alumnuae passedout registration
        public ActionResult AlumnaePassedoutRegistration()
        {
            AlumnaeViewModel objViewModel = new AlumnaeViewModel();
            string sAcademicYear = FetchAcademicYear();


            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //fetch department
                    var Department = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    objViewModel.DepartmentList = new SelectList(Department, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);

                    //fetch all programme
                    var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                           SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammeGroup)).DataSource.Data;
                    if (liProgramme != null && liProgramme.Count > 0)
                    {


                        objViewModel.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    }

                    // fetch scholorship type
                    var liScholorshipType = (List<SUP_ALU_SCHOLORSHIP_TYPE>)CMSHandler.FetchData<SUP_ALU_SCHOLORSHIP_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupAluScholorshipType)).DataSource.Data;
                    if (liScholorshipType != null && liScholorshipType.Count > 0)
                    {
                        objViewModel.ScholorshipType = new SelectList(liScholorshipType, Common.SUP_ALU_SCHOLORSHIP_TYPE.ALU_SCHOLORSHIP_TYPE_ID, Common.SUP_ALU_SCHOLORSHIP_TYPE.SCHOLORSHIP_NAME);

                    }

                    //// fetch scholorship scheme
                    //var liScholorshipScheme =(List<SUP_ALU_SCHOLORSHIP_SCHEME>)CMSHandler.FetchData<SUP_ALU_SCHOLORSHIP_SCHEME>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupAluScholorshipScheme)).DataSource.Data;
                    //if(liScholorshipScheme!=null && liScholorshipScheme.Count>0)
                    //{
                    //    objViewModel.ScholorshipScheme = new SelectList(liScholorshipScheme, Common.SUP_ALU_SCHOLORSHIP_SCHEME.ALU_SCHOLORSHIP_SCHEME_ID, Common.SUP_ALU_SCHOLORSHIP_SCHEME.SCHEME);

                    //}


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "AlumnaePassedoutRegistration", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }

        public JsonResult SaveAluPassedOutRegistration()
        {
            string sDirectorypath = string.Empty, sFilePath = string.Empty, sFileName = string.Empty, sSavePath = string.Empty, sFileUrl = string.Empty;
            var path = string.Empty;
            int Count = 0;
            JsonResultData objResultData = new JsonResultData();
            try
            {
                var result = JsonConvert.DeserializeObject<ALUMNAE_PASSEDOUT_SAVE_FIELDS>(Request.Form["sJsonData"]);

                string sSQL = string.Empty;
                string sAcademicYear = FetchAcademicYear();
                if (result.objAluPassedoutRegistration != null && !String.IsNullOrEmpty(sAcademicYear))
                {

                    sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.ALUMNAE_PASSEDOUT_IMAGE_PATH + "\\";
                    sFileUrl = Common.URLPages.ALUMNAE_PASSEDOUT_IMAGE_PATH;

                    // add alumnae passedout regirstation
                    // formating date of birth date
                    if (!string.IsNullOrEmpty(result.objAluPassedoutRegistration.DATE_OF_BIRTH))
                    {
                        result.objAluPassedoutRegistration.DATE_OF_BIRTH = CommonMethods.ConvertdatetoyearFromat(result.objAluPassedoutRegistration.DATE_OF_BIRTH);
                    }

                    //check if user is existed or not
                    if (UserExistedorNotForAlumnaePassedout(result.objAluPassedoutRegistration.PROGRAMME_ID, result.objAluPassedoutRegistration.BATCH_BEGIN_YEAR, result.objAluPassedoutRegistration.EMAIL_ID))
                    {
                        objResultData.Message = Common.Messages.RecordsExistAlready;
                        objResultData.ErrorCode = Common.ErrorCode.Created;
                        return Json(objResultData);
                    }

                    // date of birth photo copy 
                    if (!string.IsNullOrEmpty(result.objAluPassedoutRegistration.DOB_PROOF))
                    {
                        result.objAluPassedoutRegistration.DOB_PROOF = SaveFileandGetFileNameByFileKey(result.objAluPassedoutRegistration.DOB_PROOF, sDirectorypath, "DOB", result.objAluPassedoutRegistration.FIRST_NAME, sFileUrl);
                    }


                    //amount received photo
                    if (!string.IsNullOrEmpty(result.objAluPassedoutRegistration.SCHOLOR_BANK_PROOF))
                    {
                        var sScholorPath = result.objAluPassedoutRegistration.SCHOLOR_BANK_PROOF.Split(',');
                        result.objAluPassedoutRegistration.SCHOLOR_BANK_PROOF = "";
                        for (int i = 0; i < sScholorPath.Length; i++)
                        {

                            result.objAluPassedoutRegistration.SCHOLOR_BANK_PROOF += SaveFileandGetFileNameByFileKey(sScholorPath[i], sDirectorypath, "SCHOLORSHIP", result.objAluPassedoutRegistration.FIRST_NAME, sFileUrl) + ",";
                        }
                        result.objAluPassedoutRegistration.SCHOLOR_BANK_PROOF = result.objAluPassedoutRegistration.SCHOLOR_BANK_PROOF.Trim(',');

                    }



                    //competitive exam photo
                    if (!string.IsNullOrEmpty(result.objAluPassedoutRegistration.COMPETITIVE_EXAM_PROOF))
                    {
                        var sCExamPath = result.objAluPassedoutRegistration.COMPETITIVE_EXAM_PROOF.Split(',');
                        result.objAluPassedoutRegistration.COMPETITIVE_EXAM_PROOF = "";
                        for (int i = 0; i < sCExamPath.Length; i++)
                        {
                            result.objAluPassedoutRegistration.COMPETITIVE_EXAM_PROOF += SaveFileandGetFileNameByFileKey(sCExamPath[i], sDirectorypath, "COMPETITIVEEXAM", result.objAluPassedoutRegistration.FIRST_NAME, sFileUrl) + ",";
                        }
                        result.objAluPassedoutRegistration.COMPETITIVE_EXAM_PROOF = result.objAluPassedoutRegistration.COMPETITIVE_EXAM_PROOF.Trim(',');

                    }

                    //certificate course image
                    if (!string.IsNullOrEmpty(result.objAluPassedoutRegistration.CC_PROOF))
                    {
                        result.objAluPassedoutRegistration.CC_PROOF = SaveFileandGetFileNameByFileKey(result.objAluPassedoutRegistration.CC_PROOF, sDirectorypath, "CERTIFICATECOURSE", result.objAluPassedoutRegistration.FIRST_NAME, sFileUrl);

                    }

                    //save alumnae passedout registration
                    resultArgs = CMSHandler.SaveOrUpdate(result.objAluPassedoutRegistration, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAluPassedoutRegistrationInfo), sAcademicYear);

                    if (resultArgs.Success)
                    {
                        var AluPassedoutRegistration = (List<ALU_PASSEDOUT_REGISTER_INFO>)CMSHandler.FetchData<ALU_PASSEDOUT_REGISTER_INFO>(null, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchLastIndexAluPassedoutRegistrationInfo)).DataSource.Data;
                        if (AluPassedoutRegistration != null && AluPassedoutRegistration.Count > 0)
                        {
                            if (result.objAluPassedoutRegistration.IS_HIGHER == "1")
                            {
                                result.objAluPassedoutHigherDetails.ALU_PASSEDOUT_REGISTER_ID = AluPassedoutRegistration.FirstOrDefault().ALU_PASSEDOUT_REGISTER_ID;

                                //image for higer id proof
                                if (!string.IsNullOrEmpty(result.objAluPassedoutHigherDetails.ID_PROOF))
                                {
                                    result.objAluPassedoutHigherDetails.ID_PROOF = SaveFileandGetFileNameByFileKey(result.objAluPassedoutHigherDetails.ID_PROOF, sDirectorypath, "HIGHERDETAILS", result.objAluPassedoutRegistration.FIRST_NAME, sFileUrl);

                                }

                                //check is already alu register id there or not
                                if (!ValidateAluRegistration<ALU_PASSEDOUT_HIGHER_DETAILS>(AlumnaeSQLCommands.FetchAluPassedoutHigherDetailsByAluPassedoutRegisterId, result.objAluPassedoutHigherDetails))
                                {
                                    resultArgs = CMSHandler.SaveOrUpdate(result.objAluPassedoutHigherDetails, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAluPassedoutHigherDetails));
                                }
                            }
                            if (result.objAluPassedoutRegistration.IS_PLACEMENT == "1")
                            {
                                result.objAluPassedoutPlacementDetails.ALU_PASSEDOUT_REGISTER_ID = AluPassedoutRegistration.FirstOrDefault().ALU_PASSEDOUT_REGISTER_ID;
                                //check is already alu register id there or not
                                if (!ValidateAluRegistration<ALU_PLACEMENT_DETAILS>(AlumnaeSQLCommands.FetchAluPassedoutPlacementDetailsByAluPassedoutRegisterId, result.objAluPassedoutPlacementDetails))
                                {
                                    resultArgs = CMSHandler.SaveOrUpdate(result.objAluPassedoutPlacementDetails, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAluPassedoutPlacementDetails));
                                }

                                //fetch placement unique id
                                var AluPassedoutPlacementDetails = (List<ALU_PASSEDOUT_PLACEMENT_DETAILS>)CMSHandler.FetchData<ALU_PASSEDOUT_PLACEMENT_DETAILS>(null, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchLastIndexAluPassedoutPlacementInfo)).DataSource.Data;
                                if (AluPassedoutPlacementDetails != null && AluPassedoutPlacementDetails.Count > 0)
                                {
                                    if (AluPassedoutPlacementDetails.FirstOrDefault().STATUS == "1")
                                    {
                                        result.objAluPassedoutPlaOnCampusDetails.ALU_PASSEDOUT_PLACEMENT_ID = AluPassedoutPlacementDetails.FirstOrDefault().ALU_PASSEDOUT_PLACEMENT_ID;
                                        //images for on campus placement
                                        if (!string.IsNullOrEmpty(result.objAluPassedoutPlaOnCampusDetails.PROOF))
                                        {
                                            var sOnCampusPath = result.objAluPassedoutPlaOnCampusDetails.PROOF.Split(',');
                                            result.objAluPassedoutPlaOnCampusDetails.PROOF = "";
                                            for (int i = 0; i < sOnCampusPath.Length; i++)
                                            {
                                                result.objAluPassedoutPlaOnCampusDetails.PROOF += SaveFileandGetFileNameByFileKey(sOnCampusPath[i], sDirectorypath, "ONCAMPUSPLACEMENT", result.objAluPassedoutRegistration.FIRST_NAME, sFileUrl) + ",";
                                            }
                                            result.objAluPassedoutPlaOnCampusDetails.PROOF = result.objAluPassedoutPlaOnCampusDetails.PROOF.Trim(',');
                                        }

                                        //check is already alu register id there or not
                                        if (!ValidateAluRegistration<ALU_PASSEDOUT_PLA_ONCAMPUS_DETAILS>(AlumnaeSQLCommands.FetchAluPassedoutPlaOncampusDetailsByAluPassedoutPlacementId, result.objAluPassedoutPlaOnCampusDetails))
                                        {
                                            resultArgs = CMSHandler.SaveOrUpdate(result.objAluPassedoutPlaOnCampusDetails, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAluPassedoutPlacementOncampusDetails));
                                        }

                                    }
                                    if (AluPassedoutPlacementDetails.FirstOrDefault().STATUS == "2")
                                    {
                                        result.objAluPassedoutPlaOffCampusDetails.ALU_PASSEDOUT_PLACEMENT_ID = AluPassedoutPlacementDetails.FirstOrDefault().ALU_PASSEDOUT_PLACEMENT_ID;

                                        //images for of campus placement
                                        if (!string.IsNullOrEmpty(result.objAluPassedoutPlaOffCampusDetails.PROOF))
                                        {
                                            var sOffCampusPath = result.objAluPassedoutPlaOffCampusDetails.PROOF.Split(',');
                                            result.objAluPassedoutPlaOffCampusDetails.PROOF = "";
                                            for (int i = 0; i < sOffCampusPath.Length; i++)
                                            {
                                                result.objAluPassedoutPlaOffCampusDetails.PROOF += SaveFileandGetFileNameByFileKey(sOffCampusPath[i], sDirectorypath, "OFFCAMPUSPLACEMENT", result.objAluPassedoutRegistration.FIRST_NAME, sFileUrl) + ",";
                                            }
                                            result.objAluPassedoutPlaOffCampusDetails.PROOF = result.objAluPassedoutPlaOffCampusDetails.PROOF.Trim(',');
                                        }

                                        //check is already alu register id there or not
                                        if (!ValidateAluRegistration<ALU_PASSEDOUT_PLA_OFFCAMPUS_DETAILS>(AlumnaeSQLCommands.FetchAluPassedoutPlaOffcampusDetailsByAluPassedoutPlacementId, result.objAluPassedoutPlaOffCampusDetails))
                                        {
                                            resultArgs = CMSHandler.SaveOrUpdate(result.objAluPassedoutPlaOffCampusDetails, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAluPassedoutPlacementOffcampusDetails));
                                        }
                                    }
                                }
                            }
                            if (result.objAluPassedoutRegistration.IS_CULTURAL == "1")
                            {
                                var cCount = 0;
                                foreach (var item in result.objAluPassedoutCulturalDetails)
                                {
                                    item.ALU_PASSEDOUT_REGISTER_ID = AluPassedoutRegistration.FirstOrDefault().ALU_PASSEDOUT_REGISTER_ID;

                                    //images for cultural
                                    if (!string.IsNullOrEmpty(item.PROOF))
                                    {
                                        item.PROOF = SaveFileandGetFileNameByFileKey(item.PROOF, sDirectorypath, "CULTURAL", result.objAluPassedoutRegistration.FIRST_NAME, sFileUrl);

                                    }

                                    //check is already alu register id there or not
                                    if (cCount == 0)
                                    {
                                        if (!ValidateAluRegistration<ALU_PASSEDOUT_CULTURAL_DETAILS>(AlumnaeSQLCommands.FetchAluPassedoutCulturalDetailsByAluPassedoutRegisterId, item))
                                        {
                                            resultArgs = CMSHandler.SaveOrUpdate(item, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAluPassedoutCulturalDetails));
                                            cCount = cCount + 1;
                                        }
                                    }
                                    else
                                    {
                                        resultArgs = CMSHandler.SaveOrUpdate(item, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAluPassedoutCulturalDetails));
                                    }
                                }
                            }

                            if (result.objAluPassedoutRegistration.IS_SPORTS == "1")
                            {
                                var sCount = 0;
                                foreach (var item in result.objAluPassedoutSportsDetails)
                                {
                                    item.ALU_PASSEDOUT_REGISTER_ID = AluPassedoutRegistration.FirstOrDefault().ALU_PASSEDOUT_REGISTER_ID;

                                    //images for sports
                                    if (!string.IsNullOrEmpty(item.PROOF))
                                    {
                                        item.PROOF = SaveFileandGetFileNameByFileKey(item.PROOF, sDirectorypath, "SPORTS", result.objAluPassedoutRegistration.FIRST_NAME, sFileUrl);

                                    }

                                    //check is already alu register id there or not
                                    if (sCount == 0)
                                    {
                                        if (!ValidateAluRegistration<ALU_PASSEDOUT_SPORTS_DETAILS>(AlumnaeSQLCommands.FetchAluPassedoutSportsDetailsByAluPassedoutRegisterId, item))
                                        {
                                            resultArgs = CMSHandler.SaveOrUpdate(item, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAluPassedoutSportsDetails));
                                            sCount = sCount + 1;
                                        }
                                    }
                                    else
                                    {
                                        resultArgs = CMSHandler.SaveOrUpdate(item, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAluPassedoutSportsDetails));
                                    }
                                }
                            }
                        }
                        else
                        {
                            objResultData.Message = Common.Messages.FailedToSaveRecords;
                        }
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        objResultData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                    else
                    {
                        objResultData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                else
                {

                    objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "SaveAluPassedOutRegistration", ex.Message);
                }
            }
            return Json(objResultData);
        }


        public string SaveFileandGetFileNameByFileKey(string fileKey, string sDirectory, string sFolder, string sFileSaveName, string sFileUrl)
        {
            string sDirectorypath = string.Empty, sFilePath = string.Empty, sFileName = string.Empty, sSavePath = string.Empty;
            var path = string.Empty;

            if (!string.IsNullOrEmpty(fileKey) && !string.IsNullOrEmpty(sFolder) && !string.IsNullOrEmpty(sFileSaveName) && !string.IsNullOrEmpty(sDirectory) && !string.IsNullOrEmpty(sFileUrl))
            {
                try
                {
                    sDirectorypath = sDirectory + sFolder;
                    // AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.ALUMNAE_PASSEDOUT_IMAGE_PATH + "\\" + sFolder;
                    if (!Directory.Exists(sDirectorypath))
                    {
                        Directory.CreateDirectory(sDirectorypath);
                    }

                    //images
                    var likey = Request.Files.AllKeys.Where(s => s == fileKey).ToList();
                    var file = Request.Files[likey[0]];

                    sFileName = file.FileName;
                    var IPFile = sFileName.Split('.');
                    sFilePath = sDirectorypath + "\\" + IPFile[0].ToUpper() + "_" + sFileSaveName.ToUpper() + "." + IPFile[1];
                    //Common.URLPages.ALUMNAE_PASSEDOUT_IMAGE_PATH

                    if (!string.IsNullOrEmpty(sFolder))
                    {
                        sSavePath = "\\" + sFileUrl + "\\" + sFolder + "\\" + IPFile[0].ToUpper() + "_" + sFileSaveName.ToUpper() + "." + IPFile[1];
                    }
                    else
                    {
                        sSavePath = "\\" + sFileUrl + "\\" + IPFile[0].ToUpper() + "_" + sFileSaveName.ToUpper() + "." + IPFile[1];
                    }
                    file.SaveAs(sFilePath);
                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "SaveFileandGetFileNameByFileKey", ex.Message);
                    }
                }

            }

            return sSavePath;

        }

        public bool UserExistedorNotForAlumnaePassedout(string sProgramme, string sBatchBegin, string sEmailId)
        {
            bool Status = false;
            if (!string.IsNullOrEmpty(sProgramme) && !string.IsNullOrEmpty(sBatchBegin) && !string.IsNullOrEmpty(sEmailId))
            {
                var item = (List<ALU_PASSEDOUT_REGISTER_INFO>)DAO.CMSHandler.FetchData<ALU_PASSEDOUT_REGISTER_INFO>(new ALU_PASSEDOUT_REGISTER_INFO() { PROGRAMME_ID = sProgramme, BATCH_BEGIN_YEAR = sBatchBegin, EMAIL_ID = sEmailId }, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAluPassedoutRegistrationInfoByProgrammeandBatchandEmail)).DataSource.Data;
                if (item != null && item.Count > 0)
                {
                    Status = true;
                }
                else
                {
                    Status = false;
                }
            }
            return Status;
        }

        #endregion

        #region Certificate Distribution
        /// <summary>
        /// Certificate Request Form for student and Alumnae  for get certificate from COE 
        /// </summary>
        /// <author>Yesudass</author>
        /// <returns></returns>
        public ActionResult CertificateRequestFrom()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new ExaminationViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liCertificaeteContactNo = (List<COE_CERTIFICATE_DISTRIBUTION>)CMSHandler.FetchData<COE_CERTIFICATE_DISTRIBUTION>(new COE_CERTIFICATE_DISTRIBUTION() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchStuContactNoForCertificate), sAcademicYear).DataSource.Data;
                    ObjViewModel.liCertificaeteissueLeave = (List<CERTIFICATE_ISSUE_LEAVE>)CMSHandler.FetchData<CERTIFICATE_ISSUE_LEAVE>(new CERTIFICATE_ISSUE_LEAVE() { }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificateIssueLeaveDate), sAcademicYear).DataSource.Data;
                    ObjViewModel.liStudentAcademicInfo = (List<GET_PROGRAMME_AND_BATCH>)CMSHandler.FetchData<GET_PROGRAMME_AND_BATCH>(new GET_PROGRAMME_AND_BATCH() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchAcademicInfoBystudentId), sAcademicYear).DataSource.Data;
                    ObjViewModel.liCertificateDistribution = (List<COE_CERTIFICATE_DISTRIBUTION>)CMSHandler.FetchData<COE_CERTIFICATE_DISTRIBUTION>(new COE_CERTIFICATE_DISTRIBUTION() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificateRequestedByStudentId), sAcademicYear).DataSource.Data;
                    var liCertificate = (List<SUP_CERTIFICATE_LIST>)CMSHandler.FetchData<SUP_CERTIFICATE_LIST>(null, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchSupCertificateList), sAcademicYear).DataSource.Data;
                    if (liCertificate != null && liCertificate.Count > 0)
                    {
                        ObjViewModel.CertificateList = new SelectList(liCertificate, Common.SUP_CERTIFICATE_LIST.CERTIFICATE_ID, Common.SUP_CERTIFICATE_LIST.CERTIFICATE_NAME);
                    }
                    var liCertificateCategory = (List<SUP_CERTIFICATE_CATEGORY_LIST>)CMSHandler.FetchData<SUP_CERTIFICATE_CATEGORY_LIST>(null, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificateCategoryList), sAcademicYear).DataSource.Data;
                    if (liCertificateCategory != null && liCertificateCategory.Count > 0)
                    {
                        ObjViewModel.CertificateCategoryList = new SelectList(liCertificateCategory, Common.SUP_CERTIFICATE_CATEGORY_LIST.CATEGORY_ID, Common.SUP_CERTIFICATE_CATEGORY_LIST.CATEGORY_NAME);
                    }
                    var liAuthorizer = (List<SUP_CERTIFICATE_AUTHORIZE_PERSON>)CMSHandler.FetchData<SUP_CERTIFICATE_AUTHORIZE_PERSON>(null, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAuthorizerList), sAcademicYear).DataSource.Data;
                    if (liAuthorizer != null && liAuthorizer.Count > 0)
                    {
                        ObjViewModel.AuthorizerList = new SelectList(liAuthorizer, Common.SUP_CERTIFICATE_AUTHORIZE_PERSON.AUTHORIZER_ID, Common.SUP_CERTIFICATE_AUTHORIZE_PERSON.AUTHORIZER_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "ListMarkSheetRequest", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchCertificateRequestInfo(string sMarkSheetRequestId)
        {
            var objModel = new COE_CERTIFICATE_DISTRIBUTION();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sMarkSheetRequestId))
                {
                    try
                    {
                        var LiMarkquestInfo = (List<COE_CERTIFICATE_DISTRIBUTION>)CMSHandler.FetchData<COE_CERTIFICATE_DISTRIBUTION>(new CEO_MARKSHEET_DISTRIBUTION() { CERTIFICATE_DISTRIBUTION_ID = sMarkSheetRequestId }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificateRquestInfoById), sAcademicYear).DataSource.Data;
                        if (LiMarkquestInfo != null && LiMarkquestInfo.Count > 0)
                        {
                            objModel.CERTIFICATE_DISTRIBUTION_ID = LiMarkquestInfo.FirstOrDefault().CERTIFICATE_DISTRIBUTION_ID;
                            objModel.CERTIFICATE_ID = LiMarkquestInfo.FirstOrDefault().CERTIFICATE_ID;
                            objModel.REASON = LiMarkquestInfo.FirstOrDefault().REASON;
                            objModel.AUTHORIZE_PERSON = LiMarkquestInfo.FirstOrDefault().AUTHORIZE_PERSON;
                            objModel.AUTHORIZE_PERSON_NAME = LiMarkquestInfo.FirstOrDefault().AUTHORIZE_PERSON_NAME;
                            objModel.AUTHORIZE_REASON = LiMarkquestInfo.FirstOrDefault().AUTHORIZE_REASON;
                            objModel.STATUS_NAME = LiMarkquestInfo.FirstOrDefault().STATUS_NAME;
                            objModel.IS_CLEARED_ALL_SUBJECT = LiMarkquestInfo.FirstOrDefault().IS_CLEARED_ALL_SUBJECT;
                            objModel.CONTACT_NO = LiMarkquestInfo.FirstOrDefault().CONTACT_NO;
                            objModel.REGISTER_NO = LiMarkquestInfo.FirstOrDefault().REGISTER_NO;
                            objModel.CERTIFICATE_TYPE = LiMarkquestInfo.FirstOrDefault().CERTIFICATE_TYPE;
                            objModel.MONTH_OF_SEMESTER = LiMarkquestInfo.FirstOrDefault().MONTH_OF_SEMESTER;
                            objModel.YEAR_OF_SEMESTER = LiMarkquestInfo.FirstOrDefault().YEAR_OF_SEMESTER;


                        }
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog ObjLog = new ErrorLog())
                        {
                            ObjLog.WriteError("Alumnae", "FetchCertificateRequestInfo", ex.Message);
                        }
                        objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        objJsonData.Message = Common.ErrorMessage.InternalServerError;
                    }
                }
            }
            else
            {
                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                objJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateCertificateRequest(string sJsonData)
        {

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sJsonData))
                {
                    try
                    {
                        var Result = JsonConvert.DeserializeObject<COE_CERTIFICATE_DISTRIBUTION>(sJsonData);
                        Result.ACADEMIC_YEAR = sAcademicYear;
                        Result.STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                        if (!string.IsNullOrEmpty(Result.CERTIFICATE_DISTRIBUTION_ID))
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(Result, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UpdateCertificateRequest), sAcademicYear, true);
                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(Result, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveCertificateRequest), sAcademicYear, true);

                            if (resultArgs.Success)
                            {
                                var sCertificateRequestId = resultArgs.RowUniqueId.ToString();

                                var LiMarkquestInfo = (List<COE_CERTIFICATE_DISTRIBUTION>)CMSHandler.FetchData<COE_CERTIFICATE_DISTRIBUTION>(new CEO_MARKSHEET_DISTRIBUTION() { CERTIFICATE_DISTRIBUTION_ID = sCertificateRequestId, STUDENT_ID = Result.STUDENT_ID }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificateRequestListForSendEmail), sAcademicYear).DataSource.Data;
                                if (LiMarkquestInfo != null && LiMarkquestInfo.Count > 0)
                                {
                                    //byte[] bytes = Convert.FromBase64String(sEmailContent);
                                    //sEmailContent = System.Text.Encoding.UTF8.GetString(bytes);
                                    string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID]).ToString() : string.Empty;
                                    EMAIL_SETTING EmailSetting = new EMAIL_SETTING();
                                    EmailSetting.ACADEMIC_YEAR_ID = sAcademicYearId;
                                    var liEmailSetting = (List<EMAIL_SETTING>)CMSHandler.FetchData<EMAIL_SETTING>(EmailSetting, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchEmailSetting)).DataSource.Data;
                                    if (liEmailSetting != null && liEmailSetting.Count > 0)
                                    {
                                        var liCollegeInfo = DapperHandler.FetchAll<CollegeDetails>(SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), null).ToList();

                                        var CollegeName = liCollegeInfo.FirstOrDefault().COLLEGE_NAME;
                                        var CollegeAdd = liCollegeInfo.FirstOrDefault().PLACE;
                                        var CollegeLogo = liCollegeInfo.FirstOrDefault().COLLEGE_LOGO;
                                        string htmlBody = string.Empty;
                                        htmlBody = @"<table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color:#f1f1f1;font-family:Arial,sans-serif;padding:20px 0;'>
                                                              <tr>
                                                                <td align='center'>
                                                                  <!-- Container -->
                                                                  <table border='0' cellpadding='0' cellspacing='0' width='90%' style='max-width:680px;background:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 6px rgba(0,0,0,0.1);'>
        
                                                                    <!-- Header -->
                                                                    <tr style='background-color:#42dbf4;'>
                                                                      <td style='padding:16px;color:#000000;'>
                                                                        <span style='font-weight:bold;font-size:16px;'>" + CollegeName + @"</span><br>
                                                                        <span style='font-size:13px;'>" + CollegeAdd + @"</span>
                                                                      </td>
                                                                    </tr>
        
                                                                    <!-- Body -->
                                                                    <tr>
                                                                      <td style='padding:24px;color:#333333;font-size:14px;line-height:22px;'>
                                                                        <h2 style='margin:0;font-size:18px;color:#212121;'>Hello dear,</h2>
                                                                        <p style='margin:16px 0;'>
                                                                          A student has submitted a certificate request. Please find the details below:
                                                                        </p>
            
                                                                        <!-- Example Table -->
                                                                        <table border='0' cellpadding='8' cellspacing='0' width='100%' style='border:1px solid #e0e0e0;border-radius:6px;margin-top:10px;'>
                                                                          <tr style='background-color:#f9f9f9;font-weight:bold;color:#555;'>
                                                                            <td>Field</td>
                                                                            <td>Value</td>
                                                                          </tr>
                                                                          <tr>
                                                                            <td>Request ID</td>
                                                                            <td>" + LiMarkquestInfo.FirstOrDefault().ROLL_NO + @"</td>
                                                                          </tr>
                                                                        <tr>
                                                                            <td>Name</td>
                                                                            <td>" + LiMarkquestInfo.FirstOrDefault().FIRST_NAME + @"</td>
                                                                          </tr>
                                                                           <tr>
                                                                            <td>Month & Year</td>
                                                                            <td>" + LiMarkquestInfo.FirstOrDefault().MONTH_OF_SEMESTER + " " + LiMarkquestInfo.FirstOrDefault().YEAR_OF_SEMESTER + @"</td>
                                                                          </tr>
                                                                            <tr>
                                                                            <td>Certificate</td>
                                                                            <td>" + LiMarkquestInfo.FirstOrDefault().CERTIFICATE_NAME + @"</td>
                                                                          </tr>
<tr>
                                                                            <td>Mobile No</td>
                                                                            <td>" + LiMarkquestInfo.FirstOrDefault().CONTACT_NO + @"</td>
                                                                          </tr>
                                                                          <tr>
                                                                            <td>Link</td>
                                                                            <td><a href='https://erp.sjctnc.edu.in/' style='color:#42dbf4;text-decoration:none;'>View Details</a></td>
                                                                          </tr>
                                                                        </table>
            
                                                                        <p style='margin-top:20px;'>
                                                                          If you have any questions, feel free to contact our support team.
                                                                        </p>
                                                                      </td>
                                                                    </tr>
        
                                                                    <!-- Footer -->
                                                                    <tr style='background:#f1f1f1;'>
                                                                      <td style='padding:16px;text-align:center;font-size:12px;color:#666;'>
                                                                        &copy; " + DateTime.Now.Year + @" " + CollegeName + @". All rights reserved.<br>
                                                                        <a href='https://sjctnc.edu.in' style='color:#42dbf4;text-decoration:none;'>Visit Website</a>
                                                                      </td>
                                                                    </tr>
        
                                                                  </table>
                                                                </td>
                                                              </tr>
                                                            </table>";

                                        Task.Run(() =>
                                        {
                                            SendEmailNotification(htmlBody, sAcademicYear);
                                        });
                                    }
                                    else
                                    {
                                        //ObjJsonData.Message = "Please Configure E-mail Setting.!";
                                        //ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                                    }
                                }
                            }
                        }
                        objJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                        objJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog ObjLog = new ErrorLog())
                        {
                            ObjLog.WriteError("Examination", "SaveorUpdateCertificateRequest", ex.Message);
                        }
                        objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        objJsonData.Message = Common.ErrorMessage.InternalServerError;
                    }
                }
                else
                {
                    objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    objJsonData.Message = Common.ErrorMessage.BadRequest;
                }
            }
            else
            {
                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                objJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(objJsonData);
        }
        public ActionResult SendEmailNotification(string content, string sAcademicYear)
        {
            if (content == null)
            {
                TempData["EmailErrorMessage"] = "No email data available to send.";
                return RedirectToAction("ErrorView"); // Redirect or show an error view if data is null
            }

            if (ModelState.IsValid)
            {
                try
                {
                    EMAIL_SETTING EmailSetting = new EMAIL_SETTING();
                    var liEmailSetting = (List<EMAIL_SETTING>)CMSHandler.FetchData<EMAIL_SETTING>(EmailSetting, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchEmailSetting)).DataSource.Data;
                    if (liEmailSetting != null && liEmailSetting.Count > 0)
                    {
                        var liCollegeInfo = DapperHandler.FetchAll<CollegeDetails>(SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), null).ToList();

                        var CollegeName = liCollegeInfo.FirstOrDefault().COLLEGE_NAME;
                        var CollegeAdd = liCollegeInfo.FirstOrDefault().PLACE;
                        var CollegeLogo = liCollegeInfo.FirstOrDefault().COLLEGE_LOGO;
                        string sDomain = string.Empty;
                        //string fromEmail = "erpsupport@sjctnc.edu.in";
                        string fromEmail = liEmailSetting.FirstOrDefault().FROM_EMAIL_ID;
                        string fromPassword = liEmailSetting.FirstOrDefault().FROM_EMAIL_PASSWORD;
                        //string toEmail = "yesudass162000@gmail.com"; // Leaving this as requested in previous logic or can be dynamic
                        //string fromPassword = "qkrg yedt ipkm dzqq"; // or App Password should not change this 
                        string toEmail = "coecert@sjctnc.edu.in";
                        System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(fromEmail, toEmail);
                        message.Subject = "Message From " + CollegeName;
                        message.Body = content;
                        message.BodyEncoding = Encoding.UTF8;
                        message.IsBodyHtml = true;

                        string smtpHost = liEmailSetting.FirstOrDefault().SMTP_HOST ?? "smtp.gmail.com";
                        int smtpPort = 587;
                        int.TryParse(liEmailSetting.FirstOrDefault().SMTP_PORT, out smtpPort);
                        bool enableSsl = liEmailSetting.FirstOrDefault().SMTP_ENABLE == "1" || liEmailSetting.FirstOrDefault().SMTP_ENABLE?.ToLower() == "true";

                        using (SmtpClient client = new SmtpClient(smtpHost, smtpPort))
                        {
                            client.EnableSsl = enableSsl;
                            client.UseDefaultCredentials = false;
                            client.Credentials = new NetworkCredential(fromEmail, fromPassword);

                            client.Send(message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //TempData["EmailErrorMessage"] = "Failed to send email: " + ex.Message;
                    return RedirectToAction("ErrorView"); // Redirect or handle error
                }
            }
            TempData["EmailErrorMessage"] = "Invalid email data.";
            return View();
        }
        
        public JsonResult DeleteCertificateRequested(string sMarkSheetRequestId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    resultArgs = CMSHandler.SaveOrUpdate(new COE_CERTIFICATE_DISTRIBUTION() { CERTIFICATE_DISTRIBUTION_ID = sMarkSheetRequestId }, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.DeleteCertificateRequested));
                    objJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                    objJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Examination", "DeleteMarkSheetRequested", ex.Message);
                    }
                    objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    objJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                objJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(objJsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region List Requested Certificate Form  for admin
        //Role:Sataff
        /// <summary>
        /// Certificate Requested List  for admin approve ro reject the action certificate from COE 
        /// </summary>
        /// <author>Yesudass</author>
        /// <returns></returns>
        public ActionResult ListCertificateRequested()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new ExaminationViewModel();
            string sSQL = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //ObjViewModel.liMarksheetdistribution = (List<CEO_MARKSHEET_DISTRIBUTION>)CMSHandler.FetchData<CEO_MARKSHEET_DISTRIBUTION>(new CEO_MARKSHEET_DISTRIBUTION() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchMarkSheetRequestedList), sAcademicYear).DataSource.Data;
                    var listatus = (List<SUP_STATUS>)CMSHandler.FetchData<SUP_STATUS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupStatus), sAcademicYear).DataSource.Data;
                    if (listatus != null && listatus.Count > 0)
                    {
                        ObjViewModel.Status = new SelectList(listatus, Common.SUP_STATUS.STATUS_ID, Common.SUP_STATUS.STATUS_NAME);
                    }
                    sSQL = AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificateRequestedList);
                    ObjViewModel.liCertificateDistribution = (List<COE_CERTIFICATE_DISTRIBUTION>)CMSHandler.FetchData<COE_CERTIFICATE_DISTRIBUTION>(null, sSQL, sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "ListMarkSheetRequested", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindRequestedStudent(string sProgrammeIds)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSQL = string.Empty;
            var ObjViewModel = new ExaminationViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var listatus = (List<SUP_STATUS>)CMSHandler.FetchData<SUP_STATUS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupStatus), sAcademicYear).DataSource.Data;
                    if (listatus != null && listatus.Count > 0)
                    {
                        ObjViewModel.Status = new SelectList(listatus, Common.SUP_STATUS.STATUS_ID, Common.SUP_STATUS.STATUS_NAME);
                    }
                    var liexamsetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchExamSetting), "2019").DataSource.Data;
                    if (liexamsetting != null && liexamsetting.Count > 0)
                    {
                        ObjViewModel.ExamSetting = new SelectList(liexamsetting, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.EXAM_NAME);
                    }
                    var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                    if (liProgramme != null && liProgramme.Count > 0)
                    {
                        ObjViewModel.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "BindRequestedStudent", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult SaveorUpdateMarkSheetrequestedStatus(string sJsonData)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sJsonData) && !string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<JSON_EVALUATOR>(sJsonData);
                    if (Result != null)
                    {

                        foreach (var item in Result.JSON_CERTIFICATE_DISTRIBUTION)
                        {
                            item.APPROVER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                            item.TO_BE_COLLECTED_DATE = CommonMethods.DateConvertionForPayU(item.TO_BE_COLLECTED_DATE);

                            item.APPROVED_DATE = DateTime.Now.ToString("yyyy-MM-dd");
                            var ResultArg = CMSHandler.SaveOrUpdate(item, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UpdateCertificateRequestStatus));
                            if (ResultArg.Success)
                            {
                                objJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                objJsonData.ErrorCode = Common.ErrorCode.Ok;
                            }
                            else
                            {
                                objJsonData.Message = Common.Messages.FailedToSaveRecords;
                                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "SaveorUpdateOBEQuestions", ex.Message);
                    }

                }
            }
            return Json(objJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateLeaveRequestedStatus(string sJsonData)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sJsonData) && !string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<JSON_EVALUATOR>(sJsonData);
                    if (Result != null)
                    {
                        foreach (var item in Result.JSON_CERTIFICATE_ISSUE_LEAVE)
                        {
                            item.FROM_DATE = CommonMethods.DateConvertionForPayU(item.FROM_DATE);
                            item.TO_DATE = CommonMethods.DateConvertionForPayU(item.TO_DATE);
                            item.ENTRY_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                            item.CREATED_DATE = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                            item.UPDATED_DATE = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                            item.ACADEMIC_YEAR = sAcademicYear;
                            if (!string.IsNullOrEmpty(item.LEAVEREQUEST_ID))
                            {
                                item.UPDATED_BY = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                                var ResultArg = CMSHandler.SaveOrUpdate(item, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UpdateCertificateIssueLeave));
                                if (ResultArg.Success)
                                {
                                    objJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                    objJsonData.ErrorCode = Common.ErrorCode.Ok;
                                }
                                else
                                {
                                    objJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                                    objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                }
                            }
                            else
                            {
                                var ResultArg = CMSHandler.SaveOrUpdate(item, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveCertificateIssueLeave));
                                if (ResultArg.Success)
                                {
                                    objJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                    objJsonData.ErrorCode = Common.ErrorCode.Ok;
                                }
                                else
                                {
                                    objJsonData.Message = Common.Messages.FailedToSaveRecords;
                                    objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "SaveorUpdateOBEQuestions", ex.Message);
                    }
                }
            }
            return Json(objJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Report Certificate Requested
        /// <summary>
        ///Report Certificate Requested List   get report and and update leave for Certificate form 
        /// </summary>
        /// <author>Yesudass</author>
        /// <returns></returns>
        public ActionResult CertificateRequestedReport(string sBatch, string sProgramme, string sDepartment)
        {
            ExaminationViewModel objViewModel = new ExaminationViewModel();
            string sAcademicYear = FetchAcademicYear();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //fetch department
                    var Department = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    objViewModel.DepartmentList = new SelectList(Department, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);

                    //fetch all programme
                    var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                           SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                    if (liProgramme != null && liProgramme.Count > 0)
                    {
                        objViewModel.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    }
                    var FetchStatus = (List<SUP_STATUS>)CMSHandler.FetchData<SUP_STATUS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupStatus)).DataSource.Data;
                    if (FetchStatus != null && FetchStatus.Count > 0)
                    {
                        objViewModel.StatusList = new SelectList(FetchStatus, Common.SUP_STATUS.STATUS_ID, Common.SUP_STATUS.STATUS_NAME);
                    }
                    var AcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcademicYear != null && AcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYearList = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "AlumnaeReport", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindCertificateRequestedReportDateWise(string sStatus, string sStartDate, string sEndDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new ExaminationViewModel();
            var ObjJsoneData = new JsonResultData();
            try
            {
                sStartDate = CommonMethods.DateConvertionForPayU(sStartDate);
                sEndDate = CommonMethods.DateConvertionForPayU(sEndDate);
                ObjViewModel.liCertificateDistribution = (List<COE_CERTIFICATE_DISTRIBUTION>)CMSHandler.FetchData<COE_CERTIFICATE_DISTRIBUTION>(new { START_DATE = sStartDate, END_DATE = sEndDate }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificateRequestedListDateWise).Replace("?STATUS", sStatus)).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AttendanceController", "BindAttendancePercentage", ex.Message);
                }
                ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }

            return View(ObjViewModel);
        }
        public ActionResult BindCertificateLeaveUpdate(string sAcYear)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new ExaminationViewModel();
            var ObjJsoneData = new JsonResultData();

            try
            {
                ObjViewModel.liCertificaeteissueLeave = (List<CERTIFICATE_ISSUE_LEAVE>)CMSHandler.FetchData<CERTIFICATE_ISSUE_LEAVE>(null, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificateLeaveUpdateDate), sAcYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AttendanceController", "BindAttendancePercentage", ex.Message);
                }
                ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }

            return View(ObjViewModel);
        }

        public ActionResult BindCertificateRequestedReportAcademicYearWise(string sAcYear)
        {
            var ObjViewModel = new ExaminationViewModel();
            var ObjJsoneData = new JsonResultData();

            try
            {
                ObjViewModel.liCertificateDistribution = (List<COE_CERTIFICATE_DISTRIBUTION>)CMSHandler.FetchData<COE_CERTIFICATE_DISTRIBUTION>(null, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificateRequestedListAcademicYearWise), sAcYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "BindCertificateRequestedReportAcademicYearWise", ex.Message);
                }
                ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }

            return View("BindCertificateRequestedReportDateWise", ObjViewModel);
        }

        public JsonResult DeleteCertificateIssueLeave(string sLeaveRequestId)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new CERTIFICATE_ISSUE_LEAVE();
                    ObjModel.LEAVEREQUEST_ID = sLeaveRequestId;
                    ObjModel.IS_DELETED = Common.DeleteFlag.Deleted;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.DeleteCertificateLeaveUpdate));
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
                        ObjLog.WriteError("PlacementController", "deleted", ex.Message);
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
        public JsonResult FetcCertificateIssueLeaveById(string sLeaveRequestId)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            try
            {

                objResultData.oResult = (List<CERTIFICATE_ISSUE_LEAVE>)CMSHandler.FetchData<CERTIFICATE_ISSUE_LEAVE>(new CERTIFICATE_ISSUE_LEAVE() { LEAVEREQUEST_ID = sLeaveRequestId }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificateLeaveUpdateById), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("PlacementController", "FetchAchievementsById", ex.Message);
                }
                objResultData.Message = Common.ErrorMessage.InternalServerError;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Manual Certificate Issue

        /// <summary>
        /// Certificate Request Form For Admin
        /// </summary>
        /// <author>Yesudass</author>
        /// <date>2025-09-16</date>
        /// <returns></returns>
        public ActionResult CertificateRequestFormforAdmin()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new ExaminationViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liCertificaeteContactNo = (List<COE_CERTIFICATE_DISTRIBUTION>)CMSHandler.FetchData<COE_CERTIFICATE_DISTRIBUTION>(new COE_CERTIFICATE_DISTRIBUTION() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchStuContactNoForCertificate), sAcademicYear).DataSource.Data;
                    ObjViewModel.liCertificaeteissueLeave = (List<CERTIFICATE_ISSUE_LEAVE>)CMSHandler.FetchData<CERTIFICATE_ISSUE_LEAVE>(new CERTIFICATE_ISSUE_LEAVE() { }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificateIssueLeaveDate), sAcademicYear).DataSource.Data;
                    ObjViewModel.liStudentAcademicInfo = (List<GET_PROGRAMME_AND_BATCH>)CMSHandler.FetchData<GET_PROGRAMME_AND_BATCH>(new GET_PROGRAMME_AND_BATCH() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchAcademicInfoBystudentId), sAcademicYear).DataSource.Data;
                    ObjViewModel.liCertificateDistribution = (List<COE_CERTIFICATE_DISTRIBUTION>)CMSHandler.FetchData<COE_CERTIFICATE_DISTRIBUTION>(new COE_CERTIFICATE_DISTRIBUTION() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificateRequestedForAdmin), sAcademicYear).DataSource.Data;
                    var liCertificate = (List<SUP_CERTIFICATE_LIST>)CMSHandler.FetchData<SUP_CERTIFICATE_LIST>(null, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchSupCertificateList), sAcademicYear).DataSource.Data;
                    if (liCertificate != null && liCertificate.Count > 0)
                    {
                        ObjViewModel.CertificateList = new SelectList(liCertificate, Common.SUP_CERTIFICATE_LIST.CERTIFICATE_ID, Common.SUP_CERTIFICATE_LIST.CERTIFICATE_NAME);
                    }
                    var liCertificateCategory = (List<SUP_CERTIFICATE_CATEGORY_LIST>)CMSHandler.FetchData<SUP_CERTIFICATE_CATEGORY_LIST>(null, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificateCategoryList), sAcademicYear).DataSource.Data;
                    if (liCertificateCategory != null && liCertificateCategory.Count > 0)
                    {
                        ObjViewModel.CertificateCategoryList = new SelectList(liCertificateCategory, Common.SUP_CERTIFICATE_CATEGORY_LIST.CATEGORY_ID, Common.SUP_CERTIFICATE_CATEGORY_LIST.CATEGORY_NAME);
                    }
                    var liAuthorizer = (List<SUP_CERTIFICATE_AUTHORIZE_PERSON>)CMSHandler.FetchData<SUP_CERTIFICATE_AUTHORIZE_PERSON>(null, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAuthorizerList), sAcademicYear).DataSource.Data;
                    if (liAuthorizer != null && liAuthorizer.Count > 0)
                    {
                        ObjViewModel.AuthorizerList = new SelectList(liAuthorizer, Common.SUP_CERTIFICATE_AUTHORIZE_PERSON.AUTHORIZER_ID, Common.SUP_CERTIFICATE_AUTHORIZE_PERSON.AUTHORIZER_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "ListMarkSheetRequest", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult FetcNameRegisterNobyRollNo(string sRollNo)
        {

            string sResult = string.Empty;
            string sAcademicYear = string.Empty;
            var objModel = new COE_CERTIFICATE_DISTRIBUTION();

            var IsexistRollNo = (List<COE_CERTIFICATE_DISTRIBUTION>)CMSHandler.FetchData<COE_CERTIFICATE_DISTRIBUTION>(new COE_CERTIFICATE_DISTRIBUTION { ROLL_NO = sRollNo }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetcNameRegisterNobyRollNo), sAcademicYear).DataSource.Data;
            if (IsexistRollNo != null && IsexistRollNo.Count > 0)
            {
                objModel.REGISTER_NO = IsexistRollNo.FirstOrDefault().REGISTER_NO;
                objModel.FIRST_NAME = IsexistRollNo.FirstOrDefault().FIRST_NAME;
                objModel.CONTACT_NO = IsexistRollNo.FirstOrDefault().CONTACT_NO;
                objModel.STUDENT_ID = IsexistRollNo.FirstOrDefault().STUDENT_ID;
                return Json(objModel);
            }
            return Json(objModel);
        }
        public JsonResult FetchCertificateByCategoryId(string categoryId)
        {
            var objResultData = new JsonResultData();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liCertificate = (List<SUP_CERTIFICATE_LIST>)CMSHandler.FetchData<SUP_CERTIFICATE_LIST>(new SUP_CERTIFICATE_LIST { CATEGORY_ID = categoryId }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificateByCategoryId), sAcademicYear).DataSource.Data;
                return Json(liCertificate, JsonRequestBehavior.AllowGet);
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchCertificateRequestInfoforAdmin(string sMarkSheetRequestId)
        {
            var objModel = new COE_CERTIFICATE_DISTRIBUTION();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sMarkSheetRequestId))
                {
                    try
                    {
                        var LiMarkquestInfo = (List<COE_CERTIFICATE_DISTRIBUTION>)CMSHandler.FetchData<COE_CERTIFICATE_DISTRIBUTION>(new CEO_MARKSHEET_DISTRIBUTION() { CERTIFICATE_DISTRIBUTION_ID = sMarkSheetRequestId }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificateRquestInfoByIdForAdmin), sAcademicYear).DataSource.Data;
                        if (LiMarkquestInfo != null && LiMarkquestInfo.Count > 0)
                        {
                            objModel.CERTIFICATE_DISTRIBUTION_ID = LiMarkquestInfo.FirstOrDefault().CERTIFICATE_DISTRIBUTION_ID;
                            objModel.CERTIFICATE_ID = LiMarkquestInfo.FirstOrDefault().CERTIFICATE_ID;
                            objModel.REASON = LiMarkquestInfo.FirstOrDefault().REASON;
                            objModel.AUTHORIZE_PERSON = LiMarkquestInfo.FirstOrDefault().AUTHORIZE_PERSON;
                            objModel.AUTHORIZE_PERSON_NAME = LiMarkquestInfo.FirstOrDefault().AUTHORIZE_PERSON_NAME;
                            objModel.AUTHORIZE_REASON = LiMarkquestInfo.FirstOrDefault().AUTHORIZE_REASON;
                            objModel.STATUS_NAME = LiMarkquestInfo.FirstOrDefault().STATUS_NAME;
                            objModel.IS_CLEARED_ALL_SUBJECT = LiMarkquestInfo.FirstOrDefault().IS_CLEARED_ALL_SUBJECT;
                            objModel.CONTACT_NO = LiMarkquestInfo.FirstOrDefault().CONTACT_NO;
                            objModel.REGISTER_NO = LiMarkquestInfo.FirstOrDefault().REGISTER_NO;
                            objModel.CERTIFICATE_TYPE = LiMarkquestInfo.FirstOrDefault().CERTIFICATE_TYPE;
                            objModel.MONTH_OF_SEMESTER = LiMarkquestInfo.FirstOrDefault().MONTH_OF_SEMESTER;
                            objModel.YEAR_OF_SEMESTER = LiMarkquestInfo.FirstOrDefault().YEAR_OF_SEMESTER;
                            objModel.ROLL_NO = LiMarkquestInfo.FirstOrDefault().ROLL_NO;
                            objModel.FIRST_NAME = LiMarkquestInfo.FirstOrDefault().FIRST_NAME;
                        }
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog ObjLog = new ErrorLog())
                        {
                            ObjLog.WriteError("Alumnae", "FetchCertificateRequestInfo", ex.Message);
                        }
                        objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        objJsonData.Message = Common.ErrorMessage.InternalServerError;
                    }
                }
            }
            else
            {
                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                objJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateCertificateRequestforAdmin(string sJsonData)
        {

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sJsonData))
                {
                    try
                    {
                        var Result = JsonConvert.DeserializeObject<COE_CERTIFICATE_DISTRIBUTION>(sJsonData);
                        Result.ACADEMIC_YEAR = sAcademicYear;
                        //Result.STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                        if (!string.IsNullOrEmpty(Result.CERTIFICATE_DISTRIBUTION_ID))
                        {
                            Result.MODIFIED_BY = sUserId;
                            resultArgs = CMSHandler.SaveOrUpdate(Result, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UpdateCertificateRequest), sAcademicYear, true);

                            objJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                            objJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        }
                        else
                        {
                            Result.CREATED_BY = sUserId;
                            Result.APPROVER_ID = sUserId;
                            Result.STATUS = Common.status.Approved;
                            Result.APPROVED_DATE = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
                            resultArgs = CMSHandler.SaveOrUpdate(Result, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveCertificateRequest), sAcademicYear, true);
                            objJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                            objJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;

                        }

                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog ObjLog = new ErrorLog())
                        {
                            ObjLog.WriteError("Examination", "SaveorUpdateCertificateRequest", ex.Message);
                        }
                        objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        objJsonData.Message = Common.ErrorMessage.InternalServerError;
                    }
                }
                else
                {
                    objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    objJsonData.Message = Common.ErrorMessage.BadRequest;
                }
            }
            else
            {
                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                objJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(objJsonData);
        }

        #endregion

        #region sup_certificate_list
        public ActionResult SupCertificateList()
        {

            var viewmodal = new AlumnaeViewModel();
            //if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                string Certificate = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                viewmodal.liCertificateList = (List<CERTIFICATE_LIST>)CMSHandler.FetchData<CERTIFICATE_LIST>(null, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCertificate), sAcademicYear).DataSource.Data;


                //string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();


                var FetchSupCertificate = (List<CERTIFICATE_CATEGORY>)CMSHandler.FetchData<CERTIFICATE_CATEGORY>(null, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCategory), sAcademicYear).DataSource.Data;
                viewmodal.CertificateName = new SelectList(FetchSupCertificate, Common.SUP_CERTIFICATE_CATEGORY_LIST.CATEGORY_ID, Common.SUP_CERTIFICATE_CATEGORY_LIST.CATEGORY_NAME);
            }
            return View(viewmodal);
        }

        public JsonResult SaveSupCertificate(string certificate)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = JsonConvert.DeserializeObject<CERTIFICATE_LIST>(certificate);

                    if (result.CERTIFICATE_ID != "" && result.CERTIFICATE_ID != null)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UpdateCertificate), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {

                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.AddCertificate), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                    }
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Alumnae", "SaveCertificate", ex.ToString());
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


        public JsonResult FetchSupCertificateById(string CertificateID)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && CertificateID != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = (List<CERTIFICATE_LIST>)CMSHandler.FetchData<CERTIFICATE_LIST>(new CERTIFICATE_LIST() { CERTIFICATE_ID = CertificateID }, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchSupCertificatebyId), sAcademicYear).DataSource.Data;
                    if (result != null && result.Count > 0)
                        objResultData.oResult = result;
                    else
                        objResultData.Message = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Alumnae", "FetchSupCertificatebyId", ex.Message);
                    }
                }

            }
            else
            {
                objResultData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSupCertificateById(string CertificateID)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && CertificateID != "")
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();


                    resultArgs = CMSHandler.SaveOrUpdate(new CERTIFICATE_LIST { CERTIFICATE_ID = CertificateID, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.DeleteSupCertificateById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Alumnae", "DeleteSupCertificateById", ex.Message);
                    }
                }

            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region certificate category
        public ActionResult certificatecategoryList()
        {
            AlumnaeViewModel viewmodal = new AlumnaeViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                viewmodal.liCertificateCategoryList = (List<CERTIFICATE_CATEGORY>)CMSHandler.FetchData<CERTIFICATE_CATEGORY>(null, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchCategory), sAcademicYear).DataSource.Data;

            }
            return View(viewmodal);
        }

        public JsonResult SaveCertificateCategory(string category)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = JsonConvert.DeserializeObject<CERTIFICATE_CATEGORY>(category);

                    if (result.CATEGORY_ID != "" && result.CATEGORY_ID != null)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UpdateCertificateCategory), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {

                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.AddCertificateCategory), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                    }
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Alumnae", "SaveCertificateCategory", ex.ToString());
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


        public JsonResult FetchSupCertificateCategoryById(string CategoryID)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && CategoryID != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = (List<CERTIFICATE_CATEGORY>)CMSHandler.FetchData<CERTIFICATE_CATEGORY>(new CERTIFICATE_CATEGORY() { CATEGORY_ID = CategoryID }, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchSupCertificateCategorybyId), sAcademicYear).DataSource.Data;
                    if (result != null && result.Count > 0)
                        objResultData.oResult = result;
                    else
                        objResultData.Message = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Alumnae", "FetchSupCertificateCategorybyId", ex.Message);
                    }
                }

            }
            else
            {
                objResultData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult DeleteSupCertificateCategoryById(string CategoryID)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && CategoryID != "")
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();


                    resultArgs = CMSHandler.SaveOrUpdate(new CERTIFICATE_CATEGORY { CATEGORY_ID = CategoryID, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.DeleteSupCertificateCategoryById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Alumnae", "DeleteSupCertificateCategoryById", ex.Message);
                    }
                }

            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Certificate Sent SMS
        public ActionResult CertificateSMSSent()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;
                    var FetchApplicationType = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType)).DataSource.Data;
                    var FetchShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    var classYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data;
                    if (FetchProgrammeMode != null && FetchProgrammeMode.Count > 0)
                    {
                        ObjViewModel.ProgrammeMode = new SelectList(FetchProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                    }
                    if (FetchApplicationType != null && FetchApplicationType.Count > 0)
                    {
                        ObjViewModel.ApplicationType = new SelectList(FetchApplicationType, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                    }
                    if (FetchShift != null && FetchShift.Count > 0)
                    {
                        ObjViewModel.Shift = new SelectList(FetchShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    if (classYear != null && classYear.Count > 0)
                    {
                        ObjViewModel.ClassYear = new SelectList(classYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("CommunicationController", "DailyAbsenteesSMS", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult GetProgramByShiftclasslevel(string sShiftId, string programmeLevel)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sOption = string.Empty;
            var liProgram = new List<CP_PROGRAMME_2017>();
            var ObjShift = new Sup_Shift();
            ObjShift.SHIFT_ID = sShiftId;
            liProgram = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(new CP_PROGRAMME_2017() { PROGRAMME_LEVEL = programmeLevel, SHIFT = sShiftId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupByShiftIdAndProgrammeLevel), sAcademicYear).DataSource.Data;
            if (liProgram != null && liProgram.Count > 0)
            {
                foreach (var item in liProgram)
                {
                    sOption += "<option value='" + item.PROGRAMME_ID + "'>" + item.PROGRAMME_NAME + "</option>";
                }
            }

            return Json(sOption);
        }
        public JsonResult BindDDLClassList(string sProgramId, string sShiftId, string sClassYear)
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
                    var Class = (List<cp_Classes_2017>)CMS.DAO.CMSHandler.FetchData<cp_Classes_2017>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByShiftAndProgrammeGroup).Replace("?PROGRAMME_GROUP_ID", sProgramId).Replace("?CLASS_YEAR", sClassYear), sAcademicYear).DataSource.Data;
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
        public ActionResult BindStudentListforCertificateSentSMS(string sClassId, string sProgramName)
        {
            CommunicationViewModel objViewModel = new CommunicationViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sClassId) && !string.IsNullOrEmpty(sProgramName))
            {
                try
                {
                    objViewModel.LIStudentDetails = (List<Student_Personal_Info>)CMS.DAO.CMSHandler.FetchData<Student_Personal_Info>(new cp_Classes_2017() { PROGRAMME_GROUP_ID = sProgramName }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentListForCertificateSMS).Replace("?CLASS_ID", sClassId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StudentController", "BindStudentForTcSerialNo", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StudentController", "BindStudentForTcSerialNo", ex.Message);
                    }
                }
            }

            return View(objViewModel);
        }



        public JsonResult CertificateSentSMS(string StudentId, string Message)
        {
            AttendanceViewModel objAttendance = new AttendanceViewModel();
            JsonResultData ObjJsonData = new JsonResultData();
            CommunicationController communication = new CommunicationController();
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    if (StudentId != null)
                    {
                        StudentId = StudentId.TrimEnd(',');
                        StudentId = StudentId.Replace("undefined", "");
                        string sSQL = SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentMobileNoForCertificateSMS).Replace(Common.Delimiter.QUS + Common.STUDENT_INFO.STUDENT_ID, StudentId);
                        var studentDetails = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(null, sSQL, sAcademicYear).DataSource.Data;

                        if (studentDetails != null && studentDetails.Count > 0)
                        {
                            var AttendanceDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            string sCount = string.Empty;

                            foreach (var item in studentDetails)
                            {

                                string sAbsentMessage = string.Empty;
                                if (!string.IsNullOrEmpty(Message))
                                {
                                    // DLT Template: Dear {#var#}{#var#}{#var#}{#var#}{#var#}{#var#}{#var#}{#var#}{#var#}{#var#}. St.Joseph's College of Arts & Science (Autonomous) - Cuddalore.
                                    sAbsentMessage = "Dear " + item.FIRST_NAME + " " + Message + " . St.Joseph's College of Arts & Science (Autonomous) - Cuddalore.";
                                    sCount = communication.ValidateCreditCount(sAbsentMessage.Length);
                                    communication.sCreditCount = Convert.ToString(sCount);
                                }
                                else
                                {
                                    // DLT Template: Dear {#var#}{#var#}{#var#}{#var#}{#var#}{#var#}{#var#}{#var#}{#var#}{#var#}. St.Joseph's College of Arts & Science (Autonomous) - Cuddalore.
                                    sAbsentMessage = "Dear " + item.FIRST_NAME + " " + item.ROLL_NO + " " + item.CLASS_NAME + " . St.Joseph's College of Arts & Science (Autonomous) - Cuddalore.";
                                    sCount = communication.ValidateCreditCount(sAbsentMessage.Length);
                                    communication.sCreditCount = Convert.ToString(sCount);
                                }

                                communication.sSmsMode = Common.MessageType.ATTENDANCE;
                                if (!string.IsNullOrEmpty(item.MOBILE_NO))
                                {
                                    communication.liSMS.Add(new MessageResult.sms() { to = item.MOBILE_NO, message = sAbsentMessage, custom = item.STUDENT_ID, custom1 = item.CLASS_ID, CreditCount = Convert.ToString(sCount) });
                                    communication.listbulkSMS.Add(new MessageResult.listsms() { mobiles = item.MOBILE_NO, sms = sAbsentMessage, class_id = item.CLASS_ID, receipient_id = item.STUDENT_ID });
                                }
                            }
                            communication.SendMessageV1();
                            ObjJsonData.Message = "Message Sent Successfully";
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message = Common.ErrorMessage.NoContent;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.ErrorMessage.NoContent;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
                    ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "SendSMSAbsenteeslistByCurrentDate", ex.Message);
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }



        #endregion

        // sjc tnc
        #region Convocation private check
        private HASCONVOCATIONRECORDS HasRecords(string sUserId, string sAcademicYear)
        {
            var ObjViewModel = new StudentViewModel();
            if (ObjViewModel.listatusforconvocation == null)
            {
                ObjViewModel.listatusforconvocation = new List<HASCONVOCATIONRECORDS>();
            }
            var licurriculumpersonal = DapperHandler.FetchAll<CF_PERSONAL_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmitted), new { @STUDENT_ID = sUserId });
            var licurriculumfeedback = DapperHandler.FetchAll<ACF_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmittedacf), new { @STUDENT_ID = sUserId });
            var lialumnaeprogression = DapperHandler.FetchAll<AP_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmittedap), new { @STUDENT_ID = sUserId });
            var feeStudentAccount = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFeeModeId), new { STUDENT_ID = sUserId, FEE_ROOT_ID = Common.FeeRoot.ConvocationFee, FEE_MODE = Common.FrequencyType.ConvocationFee }, sAcademicYear).ToList();
            var accountCreationStatus = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.GetisAlreadyAccountCreatedbyFeeMode), new { @STUDENT_ID = sUserId, FEE_MODE = Common.FrequencyType.ConvocationFee });
            bool hasCurriculumPersonal = licurriculumpersonal != null && licurriculumpersonal.Count > 0;
            bool hasCurriculumFeedback = licurriculumfeedback != null && licurriculumfeedback.Count > 0;
            bool hasAlumnaeProgression = lialumnaeprogression != null && lialumnaeprogression.Count > 0;
            bool hasFeeStudentAccount;
            bool hasFeeStudentAccountExcit;
            if (feeStudentAccount.FirstOrDefault() != null && Convert.ToInt32(feeStudentAccount.FirstOrDefault().BALANCE) <= 0)
            {
                hasFeeStudentAccount = true;
            }
            else
            {
                if (feeStudentAccount.Count > 1)
                {
                    hasFeeStudentAccountExcit = true;
                }
                else
                {
                    hasFeeStudentAccountExcit = false;
                }
                hasFeeStudentAccount = false;
            }
            bool hasAccountCreationStatus = accountCreationStatus != null && accountCreationStatus.Count > 0;

            var status = new HASCONVOCATIONRECORDS
            {
                hasCurriculumPersonal = hasCurriculumPersonal,
                hasCurriculumFeedback = hasCurriculumFeedback,
                hasAlumnaeProgression = hasAlumnaeProgression,
                hasFeeStudentAccount = hasFeeStudentAccount,
                //hasFeeStudentAccountExcit = hasFeeStudentAccountExcit,
                hasAccountCreationStatus = hasAccountCreationStatus
            };

            ObjViewModel.listatusforconvocation.Add(status);

            return status;
        }
        #endregion

        #region Alumini Card
        public ActionResult Alumnaecard()
        {
            var ObjViewModel = new StudentViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();
                string sRoleid = Session[Common.SESSION_VARIABLES.USER_ROLE_ID]?.ToString();

                if (string.IsNullOrEmpty(sAcademicYear) || string.IsNullOrEmpty(sUserId) || string.IsNullOrEmpty(sRoleid))
                {
                    return View(ObjViewModel);
                }

                if (sRoleid != Common.SupRole.ALUMINI)
                {
                    return View(ObjViewModel);
                }
                var recordStatus = HasRecords(sUserId, sAcademicYear);
                ObjViewModel.liconvocationeligible = DapperHandler.FetchAll<CONVOCATION_ELIGIBLE>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Is_eligibleforconvocation), new { @STUDENT_ID = sUserId });

                ObjViewModel.licurriculumpersonal = DapperHandler.FetchAll<CF_PERSONAL_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmitted), new { @STUDENT_ID = sUserId });
                ObjViewModel.licurriculumfeedback = DapperHandler.FetchAll<ACF_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmittedacf), new { @STUDENT_ID = sUserId });
                ObjViewModel.lialumnaeprogression = DapperHandler.FetchAll<AP_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmittedap), new { @STUDENT_ID = sUserId });

                if (ObjViewModel.licurriculumpersonal != null && ObjViewModel.licurriculumpersonal.Count > 0 && ObjViewModel.licurriculumfeedback != null && ObjViewModel.licurriculumfeedback.Count > 0 && ObjViewModel.lialumnaeprogression != null && ObjViewModel.lialumnaeprogression.Count > 0)
                {
                    ObjViewModel.liCollegeDetails = DapperHandler.FetchAll<CollegeDetails>(SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), null);
                    ObjViewModel.liStudent = DapperHandler.FetchAll<STU_PERSONAL_INFO>(StudentSQL.GetStudentSQL(StudentSQLCommads.Getstudentacademicdetailsforalumnae), new { @STUDENT_ID = sUserId });
                    ObjViewModel.lialumnaeprogression = DapperHandler.FetchAll<AP_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.GetProgression), new { @STUDENT_ID = sUserId });
                    ObjViewModel.licurriculumpersonal = DapperHandler.FetchAll<CF_PERSONAL_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getprofileconvocation), new { @STUDENT_ID = sUserId });

                }
                if (ObjViewModel.liconvocationeligible != null && ObjViewModel.liconvocationeligible.Count > 0)
                {
                    if (ObjViewModel.liconvocationeligible.FirstOrDefault().IS_PAID == "1")
                    {
                        if (recordStatus == null || !recordStatus.hasCurriculumPersonal || !recordStatus.hasCurriculumFeedback || !recordStatus.hasAlumnaeProgression || !recordStatus.hasFeeStudentAccount || !recordStatus.hasAccountCreationStatus)
                        {
                            return View(ObjViewModel);
                        }
                    }
                    else
                    {
                        if (recordStatus == null || !recordStatus.hasCurriculumPersonal || !recordStatus.hasCurriculumFeedback || !recordStatus.hasAlumnaeProgression)
                        {
                            return View(ObjViewModel);
                        }
                    }

                }
                else
                {
                    return View(ObjViewModel);
                }

                return View(ObjViewModel);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Student", "Alumnaecard", ex.ToString());
                }
                return RedirectToAction("ErrorMessage", "error");
            }
        }

        #endregion

        #region Convocation Registration
        public ActionResult Convocationinfo()
        {
            string sSql = string.Empty;
            string sSql2 = string.Empty;
            string sSql3 = string.Empty;
            var ObjViewModel = new StudentViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();
                string sRoleid = Session[Common.SESSION_VARIABLES.USER_ROLE_ID]?.ToString();

                if (string.IsNullOrEmpty(sAcademicYear) || string.IsNullOrEmpty(sUserId) || string.IsNullOrEmpty(sRoleid))
                {
                    return View(ObjViewModel);
                }
                if (sRoleid == Common.SupRole.ALUMINI)
                {
                    ObjViewModel.liconvocationeligible = DapperHandler.FetchAll<CONVOCATION_ELIGIBLE>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Is_eligibleforconvocation), new { @STUDENT_ID = sUserId });
                    if (ObjViewModel.liconvocationeligible != null && ObjViewModel.liconvocationeligible.Count > 0)
                    {
                        if (Convert.ToDateTime(ObjViewModel.liconvocationeligible.FirstOrDefault().DATE_FROM) >= DateTime.Now)
                        {
                            ObjViewModel.liconvocationeligible.FirstOrDefault().ERROR = $"Registration will start from {Convert.ToDateTime(ObjViewModel.liconvocationeligible.FirstOrDefault().DATE_FROM)}...!";
                            ObjViewModel.liconvocationeligible.FirstOrDefault().TIME_PERIOD = ObjViewModel.liconvocationeligible.FirstOrDefault().DATE_FROM;
                            ObjViewModel.liStudentflag = DapperHandler.FetchAll<STU_PERSONAL_INFO>(StudentSQL.GetStudentSQL(StudentSQLCommads.Getstudentacademicdetailsforalumnae), new { @STUDENT_ID = sUserId });
                        }
                        else if (Convert.ToDateTime(ObjViewModel.liconvocationeligible.FirstOrDefault().TO_DATE) < DateTime.Now)
                        {
                            ObjViewModel.liconvocationeligible.FirstOrDefault().ERROR = "REGCLOSE"; // STRING USED IN VIEW PAGE DON'T CHANGE
                            ObjViewModel.liStudentflag = DapperHandler.FetchAll<STU_PERSONAL_INFO>(StudentSQL.GetStudentSQL(StudentSQLCommads.Getstudentacademicdetailsforalumnae), new { @STUDENT_ID = sUserId });
                        }
                        else if (ObjViewModel.liconvocationeligible.FirstOrDefault().IS_BLOCKED == "1")
                        {
                            ObjViewModel.liconvocationeligible.FirstOrDefault().ERROR = "CONVOCATION BLOCKED PLEASE CONTACT ADMINISTRATION...!";
                            ObjViewModel.liStudentflag = DapperHandler.FetchAll<STU_PERSONAL_INFO>(StudentSQL.GetStudentSQL(StudentSQLCommads.Getstudentacademicdetailsforalumnae), new { @STUDENT_ID = sUserId });
                        }
                        else
                        {
                            ObjViewModel.liconvocationeligible.FirstOrDefault().TIME_PERIOD_END = ObjViewModel.liconvocationeligible.FirstOrDefault().TO_DATE;
                            ObjViewModel.liStudent = DapperHandler.FetchAll<STU_PERSONAL_INFO>(StudentSQL.GetStudentSQL(StudentSQLCommads.Getstudentacademicdetailsforalumnae), new { @STUDENT_ID = sUserId });
                            ObjViewModel.liCollegeDetails = DapperHandler.FetchAll<CollegeDetails>(SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), null);
                            ObjViewModel.licurriculumpersonal = DapperHandler.FetchAll<CF_PERSONAL_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmitted), new { @STUDENT_ID = sUserId });
                            ObjViewModel.licurriculumfeedback = DapperHandler.FetchAll<ACF_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmittedacf), new { @STUDENT_ID = sUserId });
                            ObjViewModel.lialumnaeprogression = DapperHandler.FetchAll<AP_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmittedap), new { @STUDENT_ID = sUserId });
                            string ssql = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFrequencyId).Replace("?AC_YEAR", sAcademicYear);
                            ObjViewModel.lifeestudentaccount = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(ssql, new { STUDENT_ID = sUserId, FEE_ROOT_ID = Common.FeeRoot.ConvocationFee, FREQUENCY_ID = "23" }, sAcademicYear).ToList();
                        }

                        var student = new STU_PERSONAL_INFO();
                        if (ObjViewModel.liStudent != null && ObjViewModel.liStudent.Count > 0)
                        {
                            student = ObjViewModel.liStudent.FirstOrDefault();
                        }
                        else
                        {
                            student = ObjViewModel.liStudentflag.FirstOrDefault();
                        }

                        string apRemarks = string.IsNullOrWhiteSpace(student?.AP_REMARKS) ? "0" : student.AP_REMARKS;
                        string profileRemarks = string.IsNullOrWhiteSpace(student?.PROFILE_REMARKS) ? "0" : student.PROFILE_REMARKS;
                        string fbRemarks = string.IsNullOrWhiteSpace(student?.FB_REMARKS) ? "0" : student.FB_REMARKS;
                        sSql = AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationremarkstostu).Replace("?REMARK_ID", apRemarks);
                        sSql2 = AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationremarkstostu).Replace("?REMARK_ID", profileRemarks);
                        sSql3 = AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationremarkstostu).Replace("?REMARK_ID", fbRemarks);

                        ObjViewModel.liapremarks = DapperHandler.FetchAll<CONVOCATION_REMARKS>(sSql, null, sAcademicYear).ToList();
                        ObjViewModel.liprofileremarks = DapperHandler.FetchAll<CONVOCATION_REMARKS>(sSql2, null, sAcademicYear).ToList();
                        ObjViewModel.lifbremarks = DapperHandler.FetchAll<CONVOCATION_REMARKS>(sSql3, null, sAcademicYear).ToList();
                    }
                    else
                    {
                        ObjViewModel.liconvocationeligible = new List<CONVOCATION_ELIGIBLE>();
                        ObjViewModel.liconvocationeligible.Add(new CONVOCATION_ELIGIBLE { ERROR = "You are not eligible for convocation registration" });// STRING USED IN VIEW PAGE DON'T CHANGE
                        ObjViewModel.liStudentflag = DapperHandler.FetchAll<STU_PERSONAL_INFO>(StudentSQL.GetStudentSQL(StudentSQLCommads.Getstudentacademicdetailsforalumnae), new { @STUDENT_ID = sUserId });
                    }
                }
                return View(ObjViewModel);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Student", "Certificate", ex.ToString());
                }
                return RedirectToAction("ErrorMessage", "error"); ;
            }
        }

        public ActionResult ConvocationPaymentFlow()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();
            string sRoleid = Session[Common.SESSION_VARIABLES.USER_ROLE_ID]?.ToString();

            if (string.IsNullOrEmpty(sAcademicYear) || string.IsNullOrEmpty(sUserId) || sRoleid != Common.SupRole.ALUMINI)
            {
                return RedirectToAction("Login", "Login");
            }

            try
            {
                string ssql = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFeeModeId).Replace("?AC_YEAR", sAcademicYear);
                var lifeestudentaccount = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(ssql, new { STUDENT_ID = sUserId, FEE_ROOT_ID = Common.FeeRoot.ConvocationFee, FEE_MODE = Common.FrequencyType.ConvocationFee }, sAcademicYear).ToList();
                // 1. Check Eligibility
                var eligibility = DapperHandler.FetchAll<CONVOCATION_ELIGIBLE>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Is_eligibleforconvocation), new { @STUDENT_ID = sUserId });
                if (eligibility == null || eligibility.Count == 0)
                {
                    TempData["Message"] = "You are not eligible for convocation registration.";
                    return RedirectToAction("Convocationinfo");
                }

                var el = eligibility.FirstOrDefault();
                if (Convert.ToDateTime(el.DATE_FROM) > DateTime.Now)
                {
                    TempData["Message"] = $"Registration will start from {el.DATE_FROM}...!";
                    return RedirectToAction("Convocationinfo");
                }
                if (Convert.ToDateTime(el.TO_DATE) < DateTime.Now)
                {
                    TempData["Message"] = "Registration has closed.";
                    return RedirectToAction("Convocationinfo");
                }
                if (el.IS_BLOCKED == "1")
                {
                    TempData["Message"] = "Convocation blocked. Please contact administration.";
                    return RedirectToAction("Convocationinfo");
                }
                if (lifeestudentaccount.FirstOrDefault().BALANCE == "0")
                {
                    TempData["Message"] = "You have already paid the convocation fee.";
                    return RedirectToAction("Convocationinfo");
                }
                if (el.APPROVAL != "1")
                {
                    TempData["Message"] = "Your registration is waiting for administrative approval.";
                    return RedirectToAction("Convocationinfo");
                }

                // 2. Check Prerequisites (Personal Info, Feedback, Progression)
                var personal = DapperHandler.FetchAll<CF_PERSONAL_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmitted), new { @STUDENT_ID = sUserId });
                var feedback = DapperHandler.FetchAll<ACF_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmittedacf), new { @STUDENT_ID = sUserId });
                var progression = DapperHandler.FetchAll<AP_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmittedap), new { @STUDENT_ID = sUserId });

                if (personal == null || personal.Count == 0)
                {
                    TempData["Message"] = "First complete your Personal details.";
                    return RedirectToAction("Convocationinfo");
                }
                if (feedback == null || feedback.Count == 0)
                {
                    TempData["Message"] = "First complete Feedback on Curriculum.";
                    return RedirectToAction("Convocationinfo");
                }
                if (progression == null || progression.Count == 0)
                {
                    TempData["Message"] = "First complete Alumni Progression.";
                    return RedirectToAction("Convocationinfo");
                }

                // 3. Check Convocation Application Form (Register)
                //var appRegister = DapperHandler.FetchAll<CONVOCATION_APPLICATION_REGISTER>(
                //    SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.CheckIsConvacationRegisterExist), 
                //    new { @STUDENTID = sUserId });

                //if (appRegister == null || appRegister.Count == 0)
                //{
                //    TempData["Message"] = "First complete the Convocation Application Form.";
                //    return RedirectToAction("Convocationinfo");
                //}

                // 3. Create Fee Account if not already created
                // Check if account is already created
                var accountCreationStatus = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.GetisAlreadyAccountCreatedbyFeeMode), new { @STUDENT_ID = sUserId, FEE_MODE = Common.FrequencyType.ConvocationFee });
                if (accountCreationStatus == null || accountCreationStatus.Count == 0)
                {
                    // Create account logic (copied from createconvocationaccount)
                    var lifeemainhead = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(
                        new FeeMainHead() { FEE_ROOT_ID = Common.FeeRoot.ConvocationFee },
                        FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeads),
                        sAcademicYear).DataSource.Data;

                    if (lifeemainhead != null && lifeemainhead.Count > 0)
                    {
                        var first = lifeemainhead.First();
                        DapperHandler.SaveOrUpdate(
                            FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeStudentAccountCreation),
                            new
                            {
                                STUDENT_ID = sUserId,
                                FREQUENCY_ID = first.FREQUENCY_ID,
                                HEAD = first.HEAD_ID,
                                CREDIT = el.REGISTRATION_AMOUNT,
                                FEE_MAIN_HEAD_ID = first.FEE_MAIN_HEAD_ID
                            },
                            sAcademicYear);

                        DapperHandler.SaveOrUpdate(
                            SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UPDATEREMARKS),
                            new { REMARK = "", STUDENT_ID = sUserId },
                            sAcademicYear);
                    }
                }
                var accountCreation = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.GetisAlreadyAccountCreatedbyFeeMode), new { @STUDENT_ID = sUserId, FEE_MODE = Common.FrequencyType.ConvocationFee });
                string frequencyId = string.Empty;
                if(accountCreation !=null && accountCreation.Count > 0)
                {
                    frequencyId= accountCreation.FirstOrDefault().FREQUENCY_ID;
                }
                // 4. Redirect to Payment Gateway
                return RedirectToAction("PaymentMode", "PaymentGateway", new { sFrequencyTypeId = Common.FrequencyType.ConvocationFee, sFeeRootId = Common.FeeRoot.ConvocationFee, sFrequencyId = frequencyId, sAcademicyear = sAcademicYear });
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "ConvocationPaymentFlow", ex.ToString());
                }
                return RedirectToAction("ErrorMessage", "error");
            }
        }

        public ActionResult MbaConvocationRegistration()
        {
            string sSql = string.Empty;
            string sSql2 = string.Empty;
            string sSql3 = string.Empty;
            var ObjViewModel = new StudentViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();
                string sRoleid = Session[Common.SESSION_VARIABLES.USER_ROLE_ID]?.ToString();

                if (string.IsNullOrEmpty(sAcademicYear) || string.IsNullOrEmpty(sUserId) || string.IsNullOrEmpty(sRoleid))
                {
                    return View(ObjViewModel);
                }
                if (sRoleid == Common.SupRole.MBA_STUDENT)
                {
                    ObjViewModel.liconvocationeligible = DapperHandler.FetchAll<CONVOCATION_ELIGIBLE>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Is_eligibleforconvocation), new { @STUDENT_ID = sUserId });
                    if (ObjViewModel.liconvocationeligible != null && ObjViewModel.liconvocationeligible.Count > 0)
                    {
                        if (Convert.ToDateTime(ObjViewModel.liconvocationeligible.FirstOrDefault().DATE_FROM) >= DateTime.Now)
                        {
                            ObjViewModel.liconvocationeligible.FirstOrDefault().ERROR = $"Registration will start from {Convert.ToDateTime(ObjViewModel.liconvocationeligible.FirstOrDefault().DATE_FROM)}...!";
                            ObjViewModel.liconvocationeligible.FirstOrDefault().TIME_PERIOD = ObjViewModel.liconvocationeligible.FirstOrDefault().DATE_FROM;
                            ObjViewModel.liStudentflag = DapperHandler.FetchAll<STU_PERSONAL_INFO>(StudentSQL.GetStudentSQL(StudentSQLCommads.GetMBAstudentacademicdetailsforalumnae), new { @STUDENT_ID = sUserId });
                        }
                        else if (Convert.ToDateTime(ObjViewModel.liconvocationeligible.FirstOrDefault().TO_DATE) < DateTime.Now)
                        {
                            ObjViewModel.liconvocationeligible.FirstOrDefault().ERROR = "REGCLOSE"; // STRING USED IN VIEW PAGE DON'T CHANGE
                            ObjViewModel.liStudent = DapperHandler.FetchAll<STU_PERSONAL_INFO>(StudentSQL.GetStudentSQL(StudentSQLCommads.GetMBAstudentacademicdetailsforalumnae), new { @STUDENT_ID = sUserId });
                            ObjViewModel.liCollegeDetails = DapperHandler.FetchAll<CollegeDetails>(SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), null);
                            ObjViewModel.licurriculumpersonal = DapperHandler.FetchAll<CF_PERSONAL_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmitted), new { @STUDENT_ID = sUserId });
                            
                            // Mocking feedback & progression lists so they count as completed on front-end
                            ObjViewModel.licurriculumfeedback = new List<ACF_SUBMIT> { new ACF_SUBMIT() };
                            ObjViewModel.lialumnaeprogression = new List<AP_SUBMIT> { new AP_SUBMIT() };

                            string ssql = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFeeModeId).Replace("?AC_YEAR", sAcademicYear);
                            ObjViewModel.lifeestudentaccount = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(ssql, new { STUDENT_ID = sUserId, FEE_ROOT_ID = Common.FeeRoot.ConvocationFee, FEE_MODE= Common.FrequencyType.ConvocationFee}, sAcademicYear).ToList();
                        }
                        else if (ObjViewModel.liconvocationeligible.FirstOrDefault().IS_BLOCKED == "1")
                        {
                            ObjViewModel.liconvocationeligible.FirstOrDefault().ERROR = "CONVOCATION BLOCKED PLEASE CONTACT ADMINISTRATION...!";
                            ObjViewModel.liStudentflag = DapperHandler.FetchAll<STU_PERSONAL_INFO>(StudentSQL.GetStudentSQL(StudentSQLCommads.GetMBAstudentacademicdetailsforalumnae), new { @STUDENT_ID = sUserId });
                        }
                        else
                        {
                            ObjViewModel.liconvocationeligible.FirstOrDefault().TIME_PERIOD_END = ObjViewModel.liconvocationeligible.FirstOrDefault().TO_DATE;
                            ObjViewModel.liStudent = DapperHandler.FetchAll<STU_PERSONAL_INFO>(StudentSQL.GetStudentSQL(StudentSQLCommads.GetMBAstudentacademicdetailsforalumnae), new { @STUDENT_ID = sUserId });
                            ObjViewModel.liCollegeDetails = DapperHandler.FetchAll<CollegeDetails>(SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), null);
                            ObjViewModel.licurriculumpersonal = DapperHandler.FetchAll<CF_PERSONAL_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmitted), new { @STUDENT_ID = sUserId });
                            
                            // Mocking feedback & progression lists so they count as completed on front-end
                            ObjViewModel.licurriculumfeedback = new List<ACF_SUBMIT> { new ACF_SUBMIT() };
                            ObjViewModel.lialumnaeprogression = new List<AP_SUBMIT> { new AP_SUBMIT() };

                            string ssql = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFeeModeId).Replace("?AC_YEAR", sAcademicYear);
                            ObjViewModel.lifeestudentaccount = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(ssql, new { STUDENT_ID = sUserId, FEE_ROOT_ID = Common.FeeRoot.ConvocationFee, FEE_MODE= Common.FrequencyType.ConvocationFee}, sAcademicYear).ToList();
                        }

                        var student = new STU_PERSONAL_INFO();
                        if (ObjViewModel.liStudent != null && ObjViewModel.liStudent.Count > 0)
                        {
                            student = ObjViewModel.liStudent.FirstOrDefault();
                        }
                        else
                        {
                            student = ObjViewModel.liStudentflag.FirstOrDefault();
                        }

                        string apRemarks = string.IsNullOrWhiteSpace(student?.AP_REMARKS) ? "0" : student.AP_REMARKS;
                        string profileRemarks = string.IsNullOrWhiteSpace(student?.PROFILE_REMARKS) ? "0" : student.PROFILE_REMARKS;
                        string fbRemarks = string.IsNullOrWhiteSpace(student?.FB_REMARKS) ? "0" : student.FB_REMARKS;
                        sSql = AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationremarkstostu).Replace("?REMARK_ID", apRemarks);
                        sSql2 = AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationremarkstostu).Replace("?REMARK_ID", profileRemarks);
                        sSql3 = AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationremarkstostu).Replace("?REMARK_ID", fbRemarks);

                        ObjViewModel.liapremarks = DapperHandler.FetchAll<CONVOCATION_REMARKS>(sSql, null, sAcademicYear).ToList();
                        ObjViewModel.liprofileremarks = DapperHandler.FetchAll<CONVOCATION_REMARKS>(sSql2, null, sAcademicYear).ToList();
                        ObjViewModel.lifbremarks = DapperHandler.FetchAll<CONVOCATION_REMARKS>(sSql3, null, sAcademicYear).ToList();
                    }
                    else
                    {
                        ObjViewModel.liconvocationeligible = new List<CONVOCATION_ELIGIBLE>();
                        ObjViewModel.liconvocationeligible.Add(new CONVOCATION_ELIGIBLE { ERROR = "You are not eligible for convocation registration" });
                        ObjViewModel.liStudentflag = DapperHandler.FetchAll<STU_PERSONAL_INFO>(StudentSQL.GetStudentSQL(StudentSQLCommads.Getstudentacademicdetailsforalumnae), new { @STUDENT_ID = sUserId });
                    }
                }
                return View("MbaConvocationRegistration", ObjViewModel);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Student", "Certificate", ex.ToString());
                }
                return RedirectToAction("ErrorMessage", "error");
            }
        }

        public ActionResult MbaConvocationPaymentFlow()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();
            string sRoleid = Session[Common.SESSION_VARIABLES.USER_ROLE_ID]?.ToString();

            if (string.IsNullOrEmpty(sAcademicYear) || string.IsNullOrEmpty(sUserId) || sRoleid != Common.SupRole.MBA_STUDENT)
            {
                return RedirectToAction("Account", "Index");
            }

            try
            {
                string ssql = FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeStudentAccountByStudentIdAndFeeModeId).Replace("?AC_YEAR", sAcademicYear);
                var lifeestudentaccount = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(ssql, new { STUDENT_ID = sUserId, FEE_ROOT_ID = Common.FeeRoot.ConvocationFee, FEE_MODE = Common.FrequencyType.ConvocationFee }, sAcademicYear).ToList();
                var eligibility = DapperHandler.FetchAll<CONVOCATION_ELIGIBLE>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Is_eligibleforconvocation), new { @STUDENT_ID = sUserId });
                if (eligibility == null || eligibility.Count == 0)
                {
                    TempData["Message"] = "You are not eligible for convocation registration.";
                    return RedirectToAction("MbaConvocationRegistration");
                }

                var el = eligibility.FirstOrDefault();
                if (Convert.ToDateTime(el.DATE_FROM) > DateTime.Now)
                {
                    TempData["Message"] = $"Registration will start from {el.DATE_FROM}...!";
                    return RedirectToAction("MbaConvocationRegistration");
                }
                if (Convert.ToDateTime(el.TO_DATE) < DateTime.Now)
                {
                    TempData["Message"] = "Registration has closed.";
                    return RedirectToAction("MbaConvocationRegistration");
                }
                if (el.IS_BLOCKED == "1")
                {
                    TempData["Message"] = "Convocation blocked. Please contact administration.";
                    return RedirectToAction("MbaConvocationRegistration");
                }
                if (lifeestudentaccount.FirstOrDefault() != null && lifeestudentaccount.FirstOrDefault().BALANCE == "0")
                {
                    TempData["Message"] = "You have already paid the convocation fee.";
                    return RedirectToAction("MbaConvocationRegistration");
                }
                if (el.APPROVAL != "1")
                {
                    TempData["Message"] = "Your registration is waiting for administrative approval.";
                    return RedirectToAction("MbaConvocationRegistration");
                }

                var personal = DapperHandler.FetchAll<CF_PERSONAL_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmitted), new { @STUDENT_ID = sUserId });
                if (personal == null || personal.Count == 0)
                {
                    TempData["Message"] = "First complete your Personal details.";
                    return RedirectToAction("MbaConvocationRegistration");
                }
                var accountCreationStatus = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.GetisAlreadyAccountCreatedbyFeeMode), new { @STUDENT_ID = sUserId, FEE_MODE = Common.FrequencyType.ConvocationFee });
                if (accountCreationStatus == null || accountCreationStatus.Count == 0)
                {
                    var lifeemainhead = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(
                        new FeeMainHead() { FEE_ROOT_ID = Common.FeeRoot.ConvocationFee },
                        FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeads),
                        sAcademicYear).DataSource.Data;

                    if (lifeemainhead != null && lifeemainhead.Count > 0)
                    {
                        var first = lifeemainhead.First();
                        DapperHandler.SaveOrUpdate(
                            FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeStudentAccountCreation),
                            new
                            {
                                STUDENT_ID = sUserId,
                                FREQUENCY_ID = first.FREQUENCY_ID,
                                HEAD = first.HEAD_ID,
                                CREDIT = el.REGISTRATION_AMOUNT,
                                FEE_MAIN_HEAD_ID = first.FEE_MAIN_HEAD_ID
                            },
                            sAcademicYear);

                        DapperHandler.SaveOrUpdate(
                            SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UPDATEREMARKS),
                            new { REMARK = "", STUDENT_ID = sUserId },
                            sAcademicYear);
                    }

                    lifeestudentaccount = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(ssql, new { STUDENT_ID = sUserId, FEE_ROOT_ID = Common.FeeRoot.ConvocationFee,FEE_MODE=Common.FrequencyType.ConvocationFee }, sAcademicYear).ToList();
                }

                var account = lifeestudentaccount.FirstOrDefault();
                string sAmount = account != null ? account.BALANCE : el.REGISTRATION_AMOUNT;
                string frequencyId = account.FREQUENCY_ID;

                return RedirectToAction("PaymentModeForAdmin", "PaymentGateway", new { sFrequencyTypeId = Common.FrequencyType.ConvocationFee, sFeeRootId = Common.FeeRoot.ConvocationFee, sFrequencyId = frequencyId, sStudentId = sUserId, sAmount = sAmount, sAcademicyear = sAcademicYear });
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "MbaConvocationPaymentFlow", ex.ToString());
                }
                return RedirectToAction("ErrorMessage", "error");
            }
        }
        #region Alumini Card
        public ActionResult MBAConvocationCardcard()
        {
            var ObjViewModel = new StudentViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();
                string sRoleid = Session[Common.SESSION_VARIABLES.USER_ROLE_ID]?.ToString();

                if (string.IsNullOrEmpty(sAcademicYear) || string.IsNullOrEmpty(sUserId) || string.IsNullOrEmpty(sRoleid))
                {
                    return View(ObjViewModel);
                }

                if (sRoleid != Common.SupRole.MBA_STUDENT)
                {
                    return View(ObjViewModel);
                }
                var recordStatus = HasRecords(sUserId, sAcademicYear);
                ObjViewModel.liconvocationeligible = DapperHandler.FetchAll<CONVOCATION_ELIGIBLE>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Is_eligibleforconvocation), new { @STUDENT_ID = sUserId });

                ObjViewModel.licurriculumpersonal = DapperHandler.FetchAll<CF_PERSONAL_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmitted), new { @STUDENT_ID = sUserId });

                if (ObjViewModel.licurriculumpersonal != null && ObjViewModel.licurriculumpersonal.Count > 0)
                {
                    ObjViewModel.liCollegeDetails = DapperHandler.FetchAll<CollegeDetails>(SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), null);
                    ObjViewModel.liStudent = DapperHandler.FetchAll<STU_PERSONAL_INFO>(StudentSQL.GetStudentSQL(StudentSQLCommads.GetMBAstudentacademicdetailsforalumnae), new { @STUDENT_ID = sUserId });
                    ObjViewModel.licurriculumpersonal = DapperHandler.FetchAll<CF_PERSONAL_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getprofileconvocation), new { @STUDENT_ID = sUserId });

                }
                if (ObjViewModel.liconvocationeligible != null && ObjViewModel.liconvocationeligible.Count > 0)
                {
                    if (ObjViewModel.liconvocationeligible.FirstOrDefault().IS_PAID == "1")
                    {
                        if (recordStatus == null || !recordStatus.hasCurriculumPersonal || !recordStatus.hasFeeStudentAccount || !recordStatus.hasAccountCreationStatus)
                        {
                            return View(ObjViewModel);
                        }
                    }
                    else
                    {
                        if (recordStatus == null || !recordStatus.hasCurriculumPersonal)
                        {
                            return View(ObjViewModel);
                        }
                    }

                }
                else
                {
                    return View(ObjViewModel);
                }

                return View(ObjViewModel);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Student", "Alumnaecard", ex.ToString());
                }
                return RedirectToAction("ErrorMessage", "error");
            }
        }

        #endregion
        public JsonResult GetMbaConvocationOrderId()
        {
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();

                if (string.IsNullOrEmpty(sUserId) || string.IsNullOrEmpty(sAcademicYear))
                {
                    return Json(new { Success = false, Message = "Session expired.", OrderId = "" }, JsonRequestBehavior.AllowGet);
                }

                string sql = @"SELECT CUSTOM_ORDER_ID AS order_id
                               FROM EDVIRON_ORDER_INFO
                               WHERE STUDENT_ID = @STUDENT_ID
                                 AND ACADEMIC_YEAR = @AC_YEAR
                                 AND FREQUENCY_TYPE_ID = '11'
                                 AND (STATUS = 'PAID' OR STATUS = 'SUCCESS')
                               ORDER BY ID DESC
                               LIMIT 1;";

                var orderList = DapperHandler.FetchAll<dynamic>(sql, new { STUDENT_ID = sUserId, AC_YEAR = sAcademicYear });

                if (orderList != null && orderList.Count > 0)
                {
                    var firstOrder = orderList.FirstOrDefault();
                    string orderId = Convert.ToString(firstOrder.order_id);
                    return Json(new { Success = true, OrderId = orderId }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Success = false, Message = "No paid Edviron order found.", OrderId = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "GetMbaConvocationOrderId", ex.Message);
                }
                return Json(new { Success = false, Message = "Error retrieving payment details.", OrderId = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult savealumnaefeedbackpersonaldata()
        {
            JsonResultData objResult = new JsonResultData();

            try
            {
                var jsonData = Request.Form["sJsonData"];
                var result = JsonConvert.DeserializeObject<CF_PERSONAL_SUBMIT>(jsonData);
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();
                string sRoleid = Session[Common.SESSION_VARIABLES.USER_ROLE_ID]?.ToString();
                string sUsername = Session[Common.SESSION_VARIABLES.USERNAME]?.ToString();

                result.STUDENT_ID = sUserId;
                result.ACADEMIC_YEAR = sAcademicYear;

                if (string.IsNullOrEmpty(sAcademicYear) || string.IsNullOrEmpty(sUserId) || string.IsNullOrEmpty(sRoleid))
                {
                    objResult.Message = "Session Expired..!";
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }
                var recordStatus = HasRecords(sUserId, sAcademicYear);
                if (recordStatus.hasCurriculumPersonal)
                {
                    objResult.Message = "Record already exists!";
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }

                var resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.savealumnaefeedbackpersonal), null);
                var resultArgsstu = CMSHandler.SaveOrUpdate(result, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.updateStuMobileandEmailinStuPersonal), null);

                var remark = DapperHandler.SaveOrUpdate(SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UPDATEREMARKSFORPROFILE), new { REMARK = "0", STUDENT_ID = sUserId }, sAcademicYear);
                objResult.Message = resultArgs.Success ? "Records saved successfully!" : "Failed to save records";
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "SaveAlumnaeprogression", ex.Message);
                }
                objResult.Message = "Error occurred while saving the data.";
            }

            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AlumnaeFeedbackonCurriculum()
        {

            var ObjViewModel = new StudentViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();
                string sRoleid = Session[Common.SESSION_VARIABLES.USER_ROLE_ID]?.ToString();

                if (string.IsNullOrEmpty(sAcademicYear) || string.IsNullOrEmpty(sUserId) || string.IsNullOrEmpty(sRoleid))
                {
                    return View(ObjViewModel);
                }
                if (sRoleid == Common.SupRole.ALUMINI)
                {
                    var recordStatus = HasRecords(sUserId, sAcademicYear);
                    if (recordStatus.hasCurriculumFeedback)
                    {
                        return View(ObjViewModel);
                    }
                    ObjViewModel.liCollegeDetails = DapperHandler.FetchAll<CollegeDetails>(SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), null);
                    ObjViewModel.liStudent = DapperHandler.FetchAll<STU_PERSONAL_INFO>(StudentSQL.GetStudentSQL(StudentSQLCommads.Getstudentacademicdetailsforalumnae), new { @STUDENT_ID = sUserId });
                    ObjViewModel.licurriculumquestions = DapperHandler.FetchAll<CURRICULUM_FOR_CONVOCATION>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchquestionsforconvocation), null);
                    ObjViewModel.liobjective = DapperHandler.FetchAll<OBJECTIVE_FOR_CONVOCATION>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchobjectivesforconvocation), null);

                    var liapincome = (List<SUP_INCOME>)CMSHandler.FetchData<SUP_INCOME>(null, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.fetchapincome)).DataSource.Data;
                    if (liapincome != null && liapincome.Count > 0)
                    {
                        ObjViewModel.SUPAPINCOME = new SelectList(liapincome, Common.AP_IMCOME.INCOME_ID, Common.AP_IMCOME.INCOME);
                    }

                    var liapstatus = (List<SUP_AP_STATUS>)CMSHandler.FetchData<SUP_AP_STATUS>(null, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.fetchapstatus)).DataSource.Data;
                    if (liapstatus != null && liapstatus.Count > 0)
                    {
                        ObjViewModel.APSTATUS = new SelectList(liapstatus, Common.SUP_AP_STATUS.AP_STATUS, Common.SUP_AP_STATUS.STATUS);
                    }

                    var liapcolloboration = (List<SUP_COLLOBORATION>)CMSHandler.FetchData<SUP_COLLOBORATION>(null, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.fetchapcolloboration)).DataSource.Data;
                    if (liapcolloboration != null && liapcolloboration.Count > 0)
                    {
                        ObjViewModel.APCOLLOBORATION = new SelectList(liapcolloboration, Common.SUP_AP_COLLOBORATION.COLLOBORATION_ID, Common.SUP_AP_COLLOBORATION.COLLOBORATION);
                    }

                }
                return View(ObjViewModel);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Student", "Certificate", ex.ToString());
                }
                return RedirectToAction("ErrorMessage", "error"); ;
            }
        }
        [HttpPost]
        public JsonResult SaveAlumnaeFeedbackOnCurriculum(List<ACF_SUBMIT> feedbackData)
        {
            JsonResultData objResult = new JsonResultData();
            StudentViewModel ObjViewModel = new StudentViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();
                string sRoleid = Session[Common.SESSION_VARIABLES.USER_ROLE_ID]?.ToString();

                if (string.IsNullOrEmpty(sAcademicYear) || string.IsNullOrEmpty(sUserId) || string.IsNullOrEmpty(sRoleid))
                {
                    objResult.Message = "Session Expired..!";
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }
                var recordStatus = HasRecords(sUserId, sAcademicYear);
                if (recordStatus.hasCurriculumFeedback)
                {
                    objResult.Message = "Record Already Exists..!";
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }
                var lstFeedBack = feedbackData.Select(x => new ACF_SUBMIT
                {
                    QUESTION_ID = x.QUESTION_ID,
                    IS_SHORT_ANSWER = x.IS_SHORT_ANSWER,
                    ANSWER_ID = x.ANSWER_ID,
                    SHORT_ANSWER = x.SHORT_ANSWER,
                    ACADEMIC_YEAR = sAcademicYear,
                    STUDENT_ID = sUserId
                });
                var savefeedbackcurriculum = DapperHandler.SaveOrUpdate(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.savefeedbackcurriculum), lstFeedBack, sAcademicYear);
                var remark = DapperHandler.SaveOrUpdate(SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UPDATEREMARKSFORFEEDBACK), new { REMARK = "", STUDENT_ID = sUserId }, sAcademicYear);
                objResult.Message = "Feedback submitted successfully!";
            }
            catch (Exception ex)
            {
                objResult.Message = $"Error submitting feedback: {ex.Message}";
            }

            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Alumnaeprogression()
        {

            var ObjViewModel = new StudentViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();
                string sRoleid = Session[Common.SESSION_VARIABLES.USER_ROLE_ID]?.ToString();

                if (string.IsNullOrEmpty(sAcademicYear) || string.IsNullOrEmpty(sUserId) || string.IsNullOrEmpty(sRoleid))
                {
                    return View(ObjViewModel);
                }
                if (sRoleid != Common.SupRole.ALUMINI)
                {
                    return View(ObjViewModel);
                }
                var recordStatus = HasRecords(sUserId, sAcademicYear);
                if (recordStatus.hasAlumnaeProgression)
                {
                    return View(ObjViewModel);
                }
                ObjViewModel.liCollegeDetails = DapperHandler.FetchAll<CollegeDetails>(SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), null);
                ObjViewModel.liStudent = DapperHandler.FetchAll<STU_PERSONAL_INFO>(StudentSQL.GetStudentSQL(StudentSQLCommads.Getstudentacademicdetailsforalumnae), new { @STUDENT_ID = sUserId });
                ObjViewModel.liobjective = DapperHandler.FetchAll<OBJECTIVE_FOR_CONVOCATION>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchobjectivesforconvocation), null);

                var liapincome = (List<SUP_INCOME>)CMSHandler.FetchData<SUP_INCOME>(null, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.fetchapincome)).DataSource.Data;
                if (liapincome != null && liapincome.Count > 0)
                {
                    ObjViewModel.SUPAPINCOME = new SelectList(liapincome, Common.AP_IMCOME.INCOME_ID, Common.AP_IMCOME.INCOME);
                }

                var liapstatus = (List<SUP_AP_STATUS>)CMSHandler.FetchData<SUP_AP_STATUS>(null, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.fetchapstatus)).DataSource.Data;
                if (liapstatus != null && liapstatus.Count > 0)
                {
                    ObjViewModel.APSTATUS = new SelectList(liapstatus, Common.SUP_AP_STATUS.AP_STATUS, Common.SUP_AP_STATUS.STATUS);
                }

                var liapcolloboration = (List<SUP_COLLOBORATION>)CMSHandler.FetchData<SUP_COLLOBORATION>(null, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.fetchapcolloboration)).DataSource.Data;
                if (liapcolloboration != null && liapcolloboration.Count > 0)
                {
                    ObjViewModel.APCOLLOBORATION = new SelectList(liapcolloboration, Common.SUP_AP_COLLOBORATION.COLLOBORATION_ID, Common.SUP_AP_COLLOBORATION.COLLOBORATION);
                }
                return View(ObjViewModel);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Student", "Certificate", ex.ToString());
                }
                return RedirectToAction("ErrorMessage", "error"); ;
            }
        }
        public JsonResult createconvocationaccount()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();

            var objResult = new { Message = "", Success = false };


            var liconvocationeligible = DapperHandler.FetchAll<CONVOCATION_ELIGIBLE>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Is_eligibleforconvocation), new { @STUDENT_ID = sUserId });
            var recordStatus = HasRecords(sUserId, liconvocationeligible.FirstOrDefault().ACADEMIC_YEAR);
            if (recordStatus.hasAccountCreationStatus && recordStatus.hasFeeStudentAccount)
            {
                return Json(new { Message = "ALL DONE.", Success = true }, JsonRequestBehavior.AllowGet);
            }

            if (recordStatus.hasAccountCreationStatus)
            {
                return Json(new { Message = "Records saved successfully!", Success = true }, JsonRequestBehavior.AllowGet);
            }

            var lifeemainhead = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(
                new FeeMainHead() { FEE_ROOT_ID = Common.FeeRoot.ConvocationFee },
                FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeads),
                liconvocationeligible.FirstOrDefault().ACADEMIC_YEAR).DataSource.Data;

            if (lifeemainhead != null && lifeemainhead.Count > 0)
            {
                var first = lifeemainhead.First();
                DapperHandler.SaveOrUpdate(
                    FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeStudentAccountCreation),
                    new
                    {
                        STUDENT_ID = sUserId,
                        FREQUENCY_ID = first.FREQUENCY_ID,
                        HEAD = first.HEAD_ID,
                        CREDIT = liconvocationeligible.FirstOrDefault().REGISTRATION_AMOUNT,
                        FEE_MAIN_HEAD_ID = first.FEE_MAIN_HEAD_ID
                    },
                    liconvocationeligible.FirstOrDefault().ACADEMIC_YEAR);

                DapperHandler.SaveOrUpdate(
                    SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UPDATEREMARKS),
                    new { REMARK = "", STUDENT_ID = sUserId },
                    liconvocationeligible.FirstOrDefault().ACADEMIC_YEAR);
            }

            return Json(new { Message = "Account created successfully!", Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetConvocationOrderId()
        {
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();

                if (string.IsNullOrEmpty(sUserId) || string.IsNullOrEmpty(sAcademicYear))
                {
                    return Json(new { Success = false, Message = "Session expired.", OrderId = "" }, JsonRequestBehavior.AllowGet);
                }

                // Fetch the latest paid CF order for this student's convocation frequency
                string sql = @"SELECT cf_order_request->>'$.order_id' AS order_id
                               FROM FEE_CF_ORDER_INFO_?AC_YEAR
                               WHERE udf1 = @STUDENT_ID
                                 AND udf2 = '11'
                                 AND UPPER(JSON_UNQUOTE(JSON_EXTRACT(cf_order_response, '$.order_status'))) = 'PAID'
                                 AND IS_DELETED != 1
                               ORDER BY fee_cf_order_id DESC
                               LIMIT 1;".Replace("?AC_YEAR", sAcademicYear);

                var orderList = DapperHandler.FetchAll<CF_ORDER_INFO>(sql, new { STUDENT_ID = sUserId });

                if (orderList != null && orderList.Count > 0 && !string.IsNullOrEmpty(orderList.FirstOrDefault().order_id))
                {
                    return Json(new { Success = true, OrderId = orderList.FirstOrDefault().order_id }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Success = false, Message = "No paid order found.", OrderId = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "GetConvocationOrderId", ex.Message);
                }
                return Json(new { Success = false, Message = "Error occurred.", OrderId = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveAlumnaeprogression()
        {
            JsonResultData objResult = new JsonResultData();
            var ObjViewModel = new StudentViewModel();
            try
            {
                var jsonData = Request.Form["sJsonData"];
                var result = JsonConvert.DeserializeObject<AP_SUBMIT>(jsonData);
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();
                string sRoleid = Session[Common.SESSION_VARIABLES.USER_ROLE_ID]?.ToString();
                string sUsername = Session[Common.SESSION_VARIABLES.USERNAME]?.ToString();
                string sUsernameReg = Session[Common.SESSION_VARIABLES.USERNAME]?.ToString();

                var recordStatus = HasRecords(sUserId, sAcademicYear);
                if (recordStatus.hasAlumnaeProgression)
                {
                    objResult.Message = "Record already exists!";
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }

                result.STUDENT_ID = sUserId;
                result.ACADEMIC_YEAR = sAcademicYear;
                HttpPostedFileBase sProofFile = Request.Files["sProof"];
                HttpPostedFileBase cmpProofFile = Request.Files["cmpProof"];

                string sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.ALUMNAE_PROGRESSION + "\\";
                if (!Directory.Exists(sDirectorypath))
                {
                    Directory.CreateDirectory(sDirectorypath);
                }

                if (sProofFile != null && sProofFile.ContentLength > 0)
                {
                    string fileExtension = Path.GetExtension(sProofFile.FileName);
                    string sFilePath = sDirectorypath + sUsername + "_" + sUserId + "_" + sAcademicYear + fileExtension;
                    string sSavePath = "\\" + Common.URLPages.ALUMNAE_PROGRESSION + "\\" + sUsername + "_" + sUserId + "_" + sAcademicYear + fileExtension;

                    if (System.IO.File.Exists(sFilePath))
                    {
                        System.IO.File.Delete(sFilePath);
                    }

                    sProofFile.SaveAs(sFilePath);
                    result.FILE = sSavePath;
                }

                if (cmpProofFile != null && cmpProofFile.ContentLength > 0)
                {
                    string fileExtension = Path.GetExtension(cmpProofFile.FileName);
                    string sFilePath = sDirectorypath + sUsername + "_cmp_" + sUserId + "_" + sAcademicYear + fileExtension;
                    string sSavePath = "\\" + Common.URLPages.ALUMNAE_PROGRESSION + "\\" + sUsername + "_cmp_" + sUserId + "_" + sAcademicYear + fileExtension;

                    if (System.IO.File.Exists(sFilePath))
                    {
                        System.IO.File.Delete(sFilePath);
                    }

                    cmpProofFile.SaveAs(sFilePath);
                    result.CLEARED_FILE = sSavePath;
                }

                var resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.savealumnaeprogression), null);
                if (resultArgs.Success)
                {
                    ObjViewModel.liconvocationeligible = DapperHandler.FetchAll<CONVOCATION_ELIGIBLE>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Is_eligibleforconvocation), new { @STUDENT_ID = sUserId });
                    if (ObjViewModel.liconvocationeligible != null && ObjViewModel.liconvocationeligible.Count > 0)
                    {
                        if (ObjViewModel.liconvocationeligible.FirstOrDefault().IS_PAID == "1")
                        {
                            if (recordStatus.hasAccountCreationStatus && recordStatus.hasFeeStudentAccount)
                            {
                                objResult.Message = "ALL DONE.";
                                return Json(objResult, JsonRequestBehavior.AllowGet);
                            }

                            if (recordStatus.hasAccountCreationStatus)
                            {
                                objResult.Message = "Records saved successfully!";
                                return Json(objResult, JsonRequestBehavior.AllowGet);
                            }
                            var lifeemainhead = (List<FeeMainHead>)CMSHandler.FetchData<FeeMainHead>(new FeeMainHead() { FEE_ROOT_ID = Common.FeeRoot.ConvocationFee }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFeeMainHeads), sAcademicYear).DataSource.Data;
                            if (lifeemainhead != null && lifeemainhead.Count > 0)
                            {
                                var saveaccount = DapperHandler.SaveOrUpdate(FeeSQL.GetFeeSQL(FeeSQLCommands.SaveFeeStudentAccountCreation), new { STUDENT_ID = sUserId, FREQUENCY_ID = lifeemainhead.FirstOrDefault().FREQUENCY_ID, HEAD = lifeemainhead.FirstOrDefault().HEAD_ID, CREDIT = "800", FEE_MAIN_HEAD_ID = lifeemainhead.FirstOrDefault().FEE_MAIN_HEAD_ID }, sAcademicYear);
                                var remark = DapperHandler.SaveOrUpdate(SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UPDATEREMARKS), new { REMARK = "", STUDENT_ID = sUserId }, sAcademicYear);
                            }
                        }
                        else
                        {
                            objResult.Message = "Records saved successfully!";
                            objResult.sResult = "NOFEES";
                            return Json(objResult, JsonRequestBehavior.AllowGet);
                        }
                    }



                }
                objResult.Message = resultArgs.Success ? "Records saved successfully!" : "Failed to save records";
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "SaveAlumnaeprogression", ex.Message);
                }
                objResult.Message = "Error occurred while saving the data.";
            }

            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Approval for Convocation
        public ActionResult Approvalforconvocation()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //Fetch classess                 
                    var liprogramme = (List<cp_Program_2017>)CMSHandler.FetchData<cp_Program_2017>(new stf_Personal_Info() { STAFF_ID = sUserId },
                         StaffSQL.GetStaffSQL(StaffSQLCommands.FetchProgrammelistByStaffId), sAcademicYear).DataSource.Data;
                    if (liprogramme != null && liprogramme.Count > 0)
                    {
                        objViewModel.Programmelist = new SelectList(liprogramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION);
                    }
                    var liconvocation = (List<CONVOCATION_SETTING>)CMSHandler.FetchData<CONVOCATION_SETTING>(new CONVOCATION_SETTINGS() { IS_ACTIVE = "1" },
                         AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationsetting), sAcademicYear).DataSource.Data;
                    if (liconvocation != null && liconvocation.Count > 0)
                    {
                        objViewModel.Convocationsettinglist = new SelectList(liconvocation, Common.CON_SETTING.SETTING_ID, Common.CON_SETTING.SETTING_NAME);
                    }
                    objViewModel.liremarksconvocation = (List<CONVOCATION_REMARKS>)CMSHandler.FetchData<CONVOCATION_REMARKS>(new CONVOCATION_REMARKS(),
                         AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationremarks), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Alumnaeontroller", "Approvalforconvocation", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }

        public ActionResult BindApprovalforconvocation(string sProgrammeId, string sSETTING)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sProgrammeId) && !string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //Fetch class student                
                    objViewModel.liStudentList = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(new CP_PROGRAMME_2017() { PROGRAMME_ID = sProgrammeId, SETTING_ID = sSETTING, FEE_MODE = Common.FrequencyType.ConvocationFee }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammeWiseconvocationStudent), null).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Alumnaeontroller", "BindApprovalforconvocation", ex.Message);
                    }
                }


            }
            return View(objViewModel);
        }
        public ActionResult Getprofile(string sStudentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new StudentViewModel();
            if (!string.IsNullOrEmpty(sStudentId) && !string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.licurriculumpersonal = DapperHandler.FetchAll<CF_PERSONAL_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getprofileconvocation), new { @STUDENT_ID = sStudentId });
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "Getprofile", ex.Message);
                    }
                }


            }
            return View(ObjViewModel);
        }
        public ActionResult Getfeedback(string sStudentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new StudentViewModel();
            if (!string.IsNullOrEmpty(sStudentId) && !string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.licurriculumquestions = DapperHandler.FetchAll<CURRICULUM_FOR_CONVOCATION>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchStudentFEEDBACK), new { @STUDENT_ID = sStudentId });
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "Getfeedback", ex.Message);
                    }
                }


            }
            return View(ObjViewModel);
        }
        public ActionResult GetProgression(string sStudentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new StudentViewModel();
            if (!string.IsNullOrEmpty(sStudentId) && !string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.lialumnaeprogression = DapperHandler.FetchAll<AP_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.GetProgression), new { @STUDENT_ID = sStudentId });
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "GetProgression", ex.Message);
                    }
                }


            }
            return View(ObjViewModel);
        }

        [HttpPost]
        public JsonResult APPROVALCHANGES(string STUDENT_ID, string APPROVAL)
        {
            JsonResultData objResult = new JsonResultData();
            var ObjViewModel = new StudentViewModel();
            try
            {
                CONVOCATION_ELIGIBLE convocationEligible = new CONVOCATION_ELIGIBLE
                {
                    STUDENT_ID = STUDENT_ID,
                    APPROVAL = APPROVAL
                };
                var sUserId = STUDENT_ID;

                ObjViewModel.liconvocationeligible = DapperHandler.FetchAll<CONVOCATION_ELIGIBLE>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Is_eligibleforconvocation), new { @STUDENT_ID = sUserId });
                ObjViewModel.licurriculumpersonal = DapperHandler.FetchAll<CF_PERSONAL_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmitted), new { @STUDENT_ID = sUserId });
                ObjViewModel.licurriculumfeedback = DapperHandler.FetchAll<ACF_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmittedacf), new { @STUDENT_ID = sUserId });
                ObjViewModel.lialumnaeprogression = DapperHandler.FetchAll<AP_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadysubmittedap), new { @STUDENT_ID = sUserId });
                ObjViewModel.lifeestudentaccount = DapperHandler.FetchAll<FEE_STUDENT_ACCOUNT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getisalreadypaid), new { @STUDENT_ID = sUserId });


                if (ObjViewModel.liconvocationeligible != null && ObjViewModel.liconvocationeligible.Count > 0)
                {
                    if (ObjViewModel.liconvocationeligible.FirstOrDefault().IS_PAID != "1")
                    {
                        if (ObjViewModel.licurriculumpersonal != null && ObjViewModel.licurriculumpersonal.Count > 0 && ObjViewModel.licurriculumfeedback != null && ObjViewModel.licurriculumfeedback.Count > 0 && ObjViewModel.lialumnaeprogression != null && ObjViewModel.lialumnaeprogression.Count > 0)
                        {
                            var resultArgs = CMSHandler.SaveOrUpdate(convocationEligible, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UPDATEAPPROVAL), null);
                            if (resultArgs.Success)
                            {
                                if (APPROVAL == "1")
                                {
                                    objResult.Message = "Approval success ...!";
                                }
                                else
                                {
                                    objResult.Message = "Approval Cancelled ...!";
                                }
                            }
                            else
                            {
                                objResult.Message = "Failed to save records.";
                            }

                        }
                        else
                        {
                            objResult.Message = "Student not full filled.";
                        }
                    }
                    else
                    {
                        if (ObjViewModel.licurriculumpersonal != null && ObjViewModel.licurriculumpersonal.Count > 0 && ObjViewModel.licurriculumfeedback != null && ObjViewModel.licurriculumfeedback.Count > 0 && ObjViewModel.lialumnaeprogression != null && ObjViewModel.lialumnaeprogression.Count > 0 && ObjViewModel.lifeestudentaccount != null && ObjViewModel.lifeestudentaccount.Count > 0)
                        {
                            var resultArgs = CMSHandler.SaveOrUpdate(convocationEligible, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UPDATEAPPROVAL), null);
                            if (resultArgs.Success)
                            {
                                if (APPROVAL == "1")
                                {
                                    objResult.Message = "Approval success ...!";
                                }
                                else
                                {
                                    objResult.Message = "Approval Cancelled ...!";
                                }
                            }
                            else
                            {
                                objResult.Message = "Failed to save records.";
                            }

                        }
                        else
                        {
                            objResult.Message = "Student not full filled.";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "APPROVALCHANGES", ex.Message);
                }
                objResult.Message = "Error occurred while saving the data.";
            }

            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Deletebasedonselection(string STUDENT_ID, string RECORD_TYPE, string REMARK)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {

                CONVOCATION_ELIGIBLE convocationEligible = new CONVOCATION_ELIGIBLE
                {
                    STUDENT_ID = STUDENT_ID,
                    APPROVAL = "0",
                    REMARK = REMARK
                };

                if (RECORD_TYPE == "Progression")
                {
                    var resultArgs = CMSHandler.SaveOrUpdate(convocationEligible, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.DELETEPROGRESSION), null);
                    resultArgs = CMSHandler.SaveOrUpdate(convocationEligible, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UPDATEAPPROVAL), null);
                    resultArgs = CMSHandler.SaveOrUpdate(convocationEligible, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UPDATEREMARKS), null);
                    objResult.Message = "Record deleted successfully!";
                }
                else if (RECORD_TYPE == "Profile")
                {
                    var resultArgs = CMSHandler.SaveOrUpdate(convocationEligible, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.DELETEACFPERSONAL), null);
                    resultArgs = CMSHandler.SaveOrUpdate(convocationEligible, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UPDATEAPPROVAL), null);
                    resultArgs = CMSHandler.SaveOrUpdate(convocationEligible, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UPDATEREMARKSFORPROFILE), null);
                    objResult.Message = "Record deleted successfully!";
                }
                else if (RECORD_TYPE == "Feedback")
                {
                    var resultArgs = CMSHandler.SaveOrUpdate(convocationEligible, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.DELETEFEEDBACK), null);
                    resultArgs = CMSHandler.SaveOrUpdate(convocationEligible, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UPDATEAPPROVAL), null);
                    resultArgs = CMSHandler.SaveOrUpdate(convocationEligible, SQL.Alumnae.AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.UPDATEREMARKSFORFEEDBACK), null);
                    objResult.Message = "Record deleted successfully!";
                }
                else
                {
                    objResult.Message = "Student not full filled.";
                }


            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "APPROVALCHANGES", ex.Message);
                }
                objResult.Message = "Error occurred while saving the data.";
            }

            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult bindentiredata(string sStudentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new StudentViewModel();
            if (!string.IsNullOrEmpty(sStudentId) && !string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.licurriculumpersonal = DapperHandler.FetchAll<CF_PERSONAL_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Getprofileconvocation), new { @STUDENT_ID = sStudentId });
                    ObjViewModel.lialumnaeprogression = DapperHandler.FetchAll<AP_SUBMIT>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.GetProgression), new { @STUDENT_ID = sStudentId });
                    ObjViewModel.licurriculumquestions = DapperHandler.FetchAll<CURRICULUM_FOR_CONVOCATION>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchStudentFEEDBACK), new { @STUDENT_ID = sStudentId });
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "Getprofile", ex.Message);
                    }
                }


            }
            return View(ObjViewModel);
        }
        public ActionResult ApprovalforconvocationTEMPV1()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserRoleId = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //Fetch classess
                    if (sUserRoleId == Common.SupRole.ADMIN)
                    {
                        var liprogramme = (List<cp_Program_2017>)CMSHandler.FetchData<cp_Program_2017>(new stf_Personal_Info() { STAFF_ID = sUserId },
                       AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammelistfordqac), sAcademicYear).DataSource.Data;
                        if (liprogramme != null && liprogramme.Count > 0)
                        {
                            objViewModel.Programmelist = new SelectList(liprogramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION);
                        }
                    }
                    else
                    {
                        var liprogramme = (List<cp_Program_2017>)CMSHandler.FetchData<cp_Program_2017>(new stf_Personal_Info() { STAFF_ID = sUserId },
                        AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammelistfordqacforHOD), sAcademicYear).DataSource.Data;
                        if (liprogramme != null && liprogramme.Count > 0)
                        {
                            objViewModel.Programmelist = new SelectList(liprogramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION);
                        }
                    }

                    var liconvocation = (List<CONVOCATION_SETTING>)CMSHandler.FetchData<CONVOCATION_SETTING>(new CONVOCATION_SETTINGS() { IS_ACTIVE = "1" },
                         AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationsetting), sAcademicYear).DataSource.Data;
                    if (liconvocation != null && liconvocation.Count > 0)
                    {
                        objViewModel.Convocationsettinglist = new SelectList(liconvocation, Common.CON_SETTING.SETTING_ID, Common.CON_SETTING.SETTING_NAME);
                    }
                    objViewModel.liremarksconvocation = (List<CONVOCATION_REMARKS>)CMSHandler.FetchData<CONVOCATION_REMARKS>(new CONVOCATION_REMARKS(),
                         AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationremarks), sAcademicYear).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "ApprovalforconvocationTEMPV1", ex.Message);
                    }
                }

            }
            return View(objViewModel);
        }
        public ActionResult ConvocationPaymentReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserRoleId = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    List<Sup_Shift> liShift = new List<Sup_Shift>();
                    liShift = (List<Sup_Shift>)CMS.DAO.CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (liShift != null && liShift.Count > 0)
                    {
                        objViewModel.ShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    var programmeLevel = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType)).DataSource.Data;
                    if (programmeLevel != null && programmeLevel.Count > 0)
                    {
                        objViewModel.Programmelevel = new SelectList(programmeLevel, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                    }
                    var AcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcademicYear != null && AcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYear = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    if (sUserRoleId == Common.SupRole.HEAD_OF_THE_DEPARTMENT)
                    {

                        var liprogramme = (List<cp_Program_2017>)CMSHandler.FetchData<cp_Program_2017>(new stf_Personal_Info() { STAFF_ID = sUserId },
                        AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammelistfordqacforHOD), sAcademicYear).DataSource.Data;
                        if (liprogramme != null && liprogramme.Count > 0)
                        {
                            objViewModel.Programmelist = new SelectList(liprogramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION);
                        }
                    }
                    else
                    {
                        var liprogramme = (List<cp_Program_2017>)CMSHandler.FetchData<cp_Program_2017>(new stf_Personal_Info() { STAFF_ID = sUserId },
                     AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammelistfordqac), sAcademicYear).DataSource.Data;
                        if (liprogramme != null && liprogramme.Count > 0)
                        {
                            objViewModel.Programmelist = new SelectList(liprogramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION);
                        }
                    }

                    var liconvocation = (List<CONVOCATION_SETTING>)CMSHandler.FetchData<CONVOCATION_SETTING>(new CONVOCATION_SETTINGS() { IS_ACTIVE = "1" },
                         AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationsetting), sAcademicYear).DataSource.Data;
                    if (liconvocation != null && liconvocation.Count > 0)
                    {
                        objViewModel.Convocationsettinglist = new SelectList(liconvocation, Common.CON_SETTING.SETTING_ID, Common.CON_SETTING.SETTING_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "ConvocationPaymentReport", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }

        public ActionResult BindConvocationPaymentReport(string sProgrammeId, string sSETTING, string sFromDate, string sToDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            try
            {
                var frequenc_type = Common.FrequencyType.ConvocationFee;
                sFromDate = CommonMethods.ConvertdatetoyearFromat(sFromDate);
                sToDate = CommonMethods.ConvertdatetoyearFromat(sToDate);
                objViewModel.liStudentList = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(new ListStudents() { FEE_MODE = frequenc_type, SETTING_ID = sSETTING, PROGRAMME_ID = sProgrammeId }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchConvocationPaymentReport).Replace("?PROGRAMME_ID", sProgrammeId).Replace("?FROM_DATE", sFromDate).Replace("?TO_DATE", sToDate), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "BindConvocationPaymentReport", ex.Message);
                }
            }
            return PartialView(objViewModel);
        }

        public ActionResult ConvocationPaidOnlyReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserRoleId = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    List<Sup_Shift> liShift = new List<Sup_Shift>();
                    liShift = (List<Sup_Shift>)CMS.DAO.CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (liShift != null && liShift.Count > 0)
                    {
                        objViewModel.ShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    // Academic Year List ...
                    var AcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcademicYear != null && AcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYear = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var programmeLevel = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType)).DataSource.Data;
                    if (programmeLevel != null && programmeLevel.Count > 0)
                    {
                        objViewModel.Programmelevel = new SelectList(programmeLevel, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                    }
                    if (sUserRoleId == Common.SupRole.HEAD_OF_THE_DEPARTMENT)
                    {

                        var liprogramme = (List<cp_Program_2017>)CMSHandler.FetchData<cp_Program_2017>(new stf_Personal_Info() { STAFF_ID = sUserId },
                       AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammelistfordqacforHOD), sAcademicYear).DataSource.Data;
                        if (liprogramme != null && liprogramme.Count > 0)
                        {
                            objViewModel.Programmelist = new SelectList(liprogramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION);
                        }
                    }
                    else
                    {
                        var liprogramme = (List<cp_Program_2017>)CMSHandler.FetchData<cp_Program_2017>(new stf_Personal_Info() { STAFF_ID = sUserId },
                        AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammelistfordqac), sAcademicYear).DataSource.Data;
                        if (liprogramme != null && liprogramme.Count > 0)
                        {
                            objViewModel.Programmelist = new SelectList(liprogramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION);
                        }
                    }

                    var liconvocation = (List<CONVOCATION_SETTING>)CMSHandler.FetchData<CONVOCATION_SETTING>(new CONVOCATION_SETTINGS() { IS_ACTIVE = "1" },
                         AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationsetting), sAcademicYear).DataSource.Data;
                    if (liconvocation != null && liconvocation.Count > 0)
                    {
                        objViewModel.Convocationsettinglist = new SelectList(liconvocation, Common.CON_SETTING.SETTING_ID, Common.CON_SETTING.SETTING_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "ConvocationPaidOnlyReport", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetProgrammesByLevelandShift(string levelId, string shiftId)
        {
            try
            {
                string sUserRoleId = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
                string sUesrId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                shiftId = shiftId.TrimStart(',');
                levelId = levelId.TrimStart(',');
                if (!string.IsNullOrEmpty(levelId) && !string.IsNullOrEmpty(shiftId))
                {
                    if (sUserRoleId == Common.SupRole.HEAD_OF_THE_DEPARTMENT)
                    {
                        
                        var sql = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeByProgrammelevelAndAcYearandStaffId)
                        .Replace(Common.Delimiter.QUS + "PROGRAMME_LEVEL_ID", levelId)
                        .Replace(Common.Delimiter.QUS + "STAFF_ID", sUesrId)
                        .Replace(Common.Delimiter.QUS + "SHIFT_ID", shiftId);

                        var liProgrammes = DapperHandler.FetchAll<cp_Program_2017>(sql).ToList();
                        return Json(liProgrammes, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        var sql = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeByProgrammelevelAndAcYearMulti)
                       .Replace(Common.Delimiter.QUS + "PROGRAMME_LEVEL_ID", levelId)
                       .Replace(Common.Delimiter.QUS + "SHIFT_ID", shiftId);

                        var liProgrammes = DapperHandler.FetchAll<cp_Program_2017>(sql).ToList();
                        return Json(liProgrammes, JsonRequestBehavior.AllowGet);

                    }
                   
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "GetProgrammesByLevel", ex.Message);
                }
            }
            return Json(new List<cp_Program_2017>(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindConvocationPaidOnlyReport(string sProgrammeId, string sSETTING, string sFromDate, string sToDate, string sStatus)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            try
            {
                var frequenc_type = Common.FrequencyType.ConvocationFee;
                sFromDate = CommonMethods.ConvertdatetoyearFromat(sFromDate);
                sToDate = CommonMethods.ConvertdatetoyearFromat(sToDate);

                AlumnaeSQLCommands command = (sStatus == "REGISTER") ? AlumnaeSQLCommands.FetchConvocationRegisteredReport : AlumnaeSQLCommands.FetchConvocationPaidOnlyReport;

                var result = CMSHandler.FetchData<ListStudents>(new ListStudents() { FEE_MODE = frequenc_type, SETTING_ID = sSETTING, PROGRAMME_ID = sProgrammeId },
                    AlumnaeSQL.GetAlumnaeSQL(command).Replace("?PROGRAMME_ID", sProgrammeId).Replace("?FROM_DATE", sFromDate).Replace("?TO_DATE", sToDate), sAcademicYear);

                if (result != null && result.DataSource != null && result.DataSource.Data != null)
                {
                    objViewModel.liStudentList = (List<ListStudents>)result.DataSource.Data;
                }
                else
                {
                    objViewModel.liStudentList = new List<ListStudents>();
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "BindConvocationPaidOnlyReport", ex.Message);
                }
            }
            return PartialView(objViewModel);
        }


        #endregion


        #region Convocation Settings, Report, Stats and Print out copy
        public ActionResult Convocation_setting()
        {
            string sAcademicYear = string.Empty;
            StudentViewModel objViewModel = new StudentViewModel();
            sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchBatch = (List<sup_Batch>)CMSHandler.FetchData<sup_Batch>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchBatch)).DataSource.Data;
                    var FechProgrammeType = (List<SUP_APPLICTION_TYPE>)CMSHandler.FetchData<SUP_APPLICTION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType), sAcademicYear).DataSource.Data;
                    var FetchResultMonth = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterResultMonth), sAcademicYear).DataSource.Data;

                    objViewModel.Batch = new SelectList(FetchBatch, Common.SUP_BATCHES.BATCH_ID, Common.SUP_BATCHES.BATCH);
                    objViewModel.ApplicationType = new SelectList(FechProgrammeType, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                    objViewModel.ResultMonth = new SelectList(FetchResultMonth, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.M_EXAM);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Alumnae", "Convocation_setting", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Alumnae", "Convocation_setting", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(objViewModel);
        }
        public ActionResult ConvocationStarts()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //Fetch classess                 
                    var liprogramme = (List<cp_Program_2017>)CMSHandler.FetchData<cp_Program_2017>(new stf_Personal_Info() { STAFF_ID = sUserId },
                         AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammelistfordqac), sAcademicYear).DataSource.Data;
                    if (liprogramme != null && liprogramme.Count > 0)
                    {
                        objViewModel.Programmelist = new SelectList(liprogramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION);
                    }
                    var liconvocation = (List<CONVOCATION_SETTING>)CMSHandler.FetchData<CONVOCATION_SETTING>(new CONVOCATION_SETTINGS() { IS_ACTIVE = "1" },
                         AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationsetting), sAcademicYear).DataSource.Data;
                    if (liconvocation != null && liconvocation.Count > 0)
                    {
                        objViewModel.Convocationsettinglist = new SelectList(liconvocation, Common.CON_SETTING.SETTING_ID, Common.CON_SETTING.SETTING_NAME);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "ConvocationStarts", ex.Message);
                    }
                }


            }
            return View(objViewModel);
        }
        public ActionResult BindConvocationStarts(string sSETTING, string ignorearrear, string fieldfor, string reporttype)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objViewModel.UDF1 = fieldfor;
                    objViewModel.UDF2 = reporttype;

                    ResultArgs objResult = new ResultArgs();
                    if (ignorearrear == "1")
                    {
                        objResult = CMSHandler.FetchData<ListStudents>(new CP_PROGRAMME_2017() { SETTING_ID = sSETTING },
                           AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationdataforstatsignorearrear), sAcademicYear);
                    }
                    else
                    {
                        objResult = CMSHandler.FetchData<ListStudents>(new CP_PROGRAMME_2017() { SETTING_ID = sSETTING },
                           AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationdataforstats), sAcademicYear);
                    }

                    if (objResult != null && objResult.DataSource != null && objResult.DataSource.Data != null)
                    {
                        objViewModel.liStudentList = (List<ListStudents>)objResult.DataSource.Data;
                    }
                    else
                    {
                        objViewModel.liStudentList = new List<ListStudents>();
                    }

                    var collegeRes = DapperHandler.FetchAll<CollegeDetails>(SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), null);
                    if (collegeRes != null)
                    {
                        objViewModel.liCollegeDetails = collegeRes.ToList();
                    }
                    else
                    {
                        objViewModel.liCollegeDetails = new List<CollegeDetails>();
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "BindConvocationStarts", ex.Message);
                    }
                    objViewModel.liStudentList = new List<ListStudents>();
                    objViewModel.liCollegeDetails = new List<CollegeDetails>();
                }
            }
            return PartialView(objViewModel);
        }
        public ActionResult Printdataforconvocation()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liprogramme = (List<cp_Program_2017>)CMSHandler.FetchData<cp_Program_2017>(new stf_Personal_Info() { STAFF_ID = sUserId },
                         AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammelistfordqac), sAcademicYear).DataSource.Data;
                    if (liprogramme != null && liprogramme.Count > 0)
                    {
                        objViewModel.Programmelist = new SelectList(liprogramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION);
                    }
                    var liconvocation = (List<CONVOCATION_SETTING>)CMSHandler.FetchData<CONVOCATION_SETTING>(new CONVOCATION_SETTINGS() { IS_ACTIVE = "1" },
                         AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationsetting), sAcademicYear).DataSource.Data;
                    if (liconvocation != null && liconvocation.Count > 0)
                    {
                        objViewModel.Convocationsettinglist = new SelectList(liconvocation, Common.CON_SETTING.SETTING_ID, Common.CON_SETTING.SETTING_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "Printdataforconvocation", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }
        public ActionResult Binddatatoprint(string sprogrammeid, string sSetting)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new StudentViewModel();
            if (!string.IsNullOrEmpty(sprogrammeid) && !string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    string sSQL = AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationinforprint).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.PROGRAMME_GROUP_ID, sprogrammeid);
                    ObjViewModel.lialumnaeprogression = (List<AP_SUBMIT>)CMSHandler.FetchData<AP_SUBMIT>(new AP_SUBMIT() { SETTING_ID = sSetting }, sSQL).DataSource.Data;

                    string sSQL2 = AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationinforprintfeedback).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.PROGRAMME_GROUP_ID, sprogrammeid);
                    ObjViewModel.licurriculumquestions = (List<CURRICULUM_FOR_CONVOCATION>)CMSHandler.FetchData<CURRICULUM_FOR_CONVOCATION>(new CURRICULUM_FOR_CONVOCATION() { SETTING_ID = sSetting }, sSQL2).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "Binddatatoprint", ex.Message);
                    }
                }


            }
            return View(ObjViewModel);
        }

        #endregion

        #region Alumni Data Collection
        [AllowAnonymous]
        public ActionResult AlumniDataCollectionForm()
        {
            try
            {
                // Fetch Academic/Programme Levels
                var programmeLevel = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType)).DataSource.Data;
                if (programmeLevel != null && programmeLevel.Count > 0)
                {
                    ViewBag.ProgrammeLevelList = new SelectList(programmeLevel, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                }
                var supShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupShift)).DataSource.Data;
                if (supShift != null && supShift.Count > 0)
                {
                    ViewBag.shiftList = new SelectList(supShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }
                // Fetch Present Status from sup_alu_present_status
                var presentStatus = DapperHandler.FetchAll<Sup_Alu_Present_Status>(AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchSupAluPresentStatus)).ToList();
                if (presentStatus != null && presentStatus.Count > 0)
                {
                    ViewBag.PresentStatusList = new SelectList(presentStatus, Common.SUP_ALU_PRESENT_STATUS.PRESENT_STATUS_ID, Common.SUP_ALU_PRESENT_STATUS.PRESENT_STATUS_NAME);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "AlumniDataCollectionForm", ex.Message);
                }
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult SaveAlumniDataCollection()
        {
            return View("AlumniDataCollectionForm");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetProgrammesByLevel(string levelId, string shiftId)
        {
            try
            {
                if (!string.IsNullOrEmpty(levelId) && !string.IsNullOrEmpty(shiftId))
                {
                    var sql = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeByProgrammelevelAndAcYear)
                        .Replace(Common.Delimiter.QUS + "PROGRAMME_LEVEL_ID", levelId)
                        .Replace(Common.Delimiter.QUS + "SHIFT_ID", shiftId);

                    var liProgrammes = DapperHandler.FetchAll<cp_Program_2017>(sql).ToList();
                    return Json(liProgrammes, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "GetProgrammesByLevel", ex.Message);
                }
            }
            return Json(new List<cp_Program_2017>(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetBranchesByProgramme(string levelId, string shiftId, string programmeName)
        {
            try
            {
                if (!string.IsNullOrEmpty(levelId) && !string.IsNullOrEmpty(programmeName))
                {
                    var sql = @"SELECT DISTINCT
                                    CPG.PROGRAMME_GROUP_ID,
                                    CPG.PROGRAMME_DESCRIPTION
                                FROM
                                    CP_PROGRAMME_GROUP AS CPG
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                WHERE
                                    CP.PROGRAMME_LEVEL_ID = " + levelId + @"
                                    AND CP.SHIFT_ID = " + shiftId + @"
                                    AND CP.PROGRAMME_NAME = '" + programmeName.Replace("'", "''") + @"'
                                    AND CPG.IS_DELETED != 1
                                    AND CP.IS_DELETED != 1
                                ORDER BY CPG.PROGRAMME_DESCRIPTION;";
                    var liBranches = DapperHandler.FetchAll<cp_Program_2017>(sql).ToList();
                    return Json(liBranches, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "GetBranchesByProgramme", ex.Message);
                }
            }
            return Json(new List<cp_Program_2017>(), JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckRollNo(string sRollNo)
        {
            try
            {
                if (!string.IsNullOrEmpty(sRollNo))
                {
                    var searchObj = new ALUMNI_DATA_COLLECTION { ROLL_NO = sRollNo };
                    var DataSource = CMSHandler.FetchData<ALUMNI_DATA_COLLECTION>(searchObj, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAlumniDataCollectionInfoByRollNo));
                    if (DataSource.DataSource.Data != null)
                    {
                        var dataList = (List<ALUMNI_DATA_COLLECTION>)DataSource.DataSource.Data;
                        if (dataList != null && dataList.Count > 0)
                        {
                            objJsonData.ErrorCode = Common.ErrorCode.Accepted;
                            objJsonData.oResult = dataList[0];
                            objJsonData.Message = "Roll Number already exists.";
                        }
                        else
                        {
                            objJsonData.ErrorCode = Common.ErrorCode.NotFound;
                        }
                    }
                    else
                    {
                        objJsonData.ErrorCode = Common.ErrorCode.NotFound;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Alumnae", "CheckRollNo", ex.Message);
                }
                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                objJsonData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Content(JsonConvert.SerializeObject(objJsonData), "application/json");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckEmail(string sEmail)
        {
            try
            {
                if (!string.IsNullOrEmpty(sEmail))
                {
                    var searchObj = new ALUMNI_DATA_COLLECTION { EMAIL_ID = sEmail };
                    var DataSource = CMSHandler.FetchData<ALUMNI_DATA_COLLECTION>(searchObj, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAlumniDataCollectionInfoByEmail));
                    if (DataSource.DataSource.Data != null)
                    {
                        var dataList = (List<ALUMNI_DATA_COLLECTION>)DataSource.DataSource.Data;
                        if (dataList != null && dataList.Count > 0)
                        {
                            objJsonData.ErrorCode = Common.ErrorCode.Accepted;
                            objJsonData.oResult = dataList[0];
                            objJsonData.Message = "Email already exists.";
                        }
                        else
                        {
                            objJsonData.ErrorCode = Common.ErrorCode.NotFound;
                        }
                    }
                    else
                    {
                        objJsonData.ErrorCode = Common.ErrorCode.NotFound;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Alumnae", "CheckEmail", ex.Message);
                }
                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                objJsonData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Content(JsonConvert.SerializeObject(objJsonData), "application/json");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult SaveAlumniDataCollection(string sJsonData, HttpPostedFileBase PHOTO_FILE)
        {
            string sAcademicYear = FetchAcademicYear();
            if (!string.IsNullOrEmpty(sJsonData))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<ALUMNI_DATA_COLLECTION>(sJsonData);
                    Result.ACADEMIC_YEAR = sAcademicYear;

                    // Check if record already exists by ROLL_NO or EMAIL_ID to determine update vs insert
                    List<ALUMNI_DATA_COLLECTION> existList = null;
                    if (!string.IsNullOrEmpty(Result.EMAIL_ID))
                    {
                        var searchEmail = new ALUMNI_DATA_COLLECTION { EMAIL_ID = Result.EMAIL_ID };
                        var checkExistEmail = CMSHandler.FetchData<ALUMNI_DATA_COLLECTION>(searchEmail, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAlumniDataCollectionInfoByEmail));
                        if (checkExistEmail != null && checkExistEmail.DataSource != null && checkExistEmail.DataSource.Data != null)
                        {
                            existList = (List<ALUMNI_DATA_COLLECTION>)checkExistEmail.DataSource.Data;
                        }
                    }

                    if ((existList == null || existList.Count == 0) && !string.IsNullOrEmpty(Result.ROLL_NO))
                    {
                        var searchRoll = new ALUMNI_DATA_COLLECTION { ROLL_NO = Result.ROLL_NO };
                        var checkExistRoll = CMSHandler.FetchData<ALUMNI_DATA_COLLECTION>(searchRoll, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAlumniDataCollectionInfoByRollNo));
                        if (checkExistRoll != null && checkExistRoll.DataSource != null && checkExistRoll.DataSource.Data != null)
                        {
                            existList = (List<ALUMNI_DATA_COLLECTION>)checkExistRoll.DataSource.Data;
                        }
                    }

                    bool isUpdate = false;
                    if (existList != null && existList.Count > 0)
                    {
                        isUpdate = true;
                        // For update, if no new photo is provided, keep the existing one
                        if (PHOTO_FILE == null || PHOTO_FILE.ContentLength == 0)
                        {
                            Result.PHOTO = existList[0].PHOTO;
                        }
                    }

                    // Handle actual file upload (not base64)
                    if (PHOTO_FILE != null && PHOTO_FILE.ContentLength > 0)
                    {
                        string sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.ALUMNE_STUDENT_IMGAEPATH;
                        if (!Directory.Exists(sDirectorypath))
                        {
                            Directory.CreateDirectory(sDirectorypath);
                        }

                        string fileExtension = Path.GetExtension(PHOTO_FILE.FileName);
                        string uniqueIdentifier = string.Empty;

                        if (!string.IsNullOrEmpty(Result.ROLL_NO))
                        {
                            uniqueIdentifier = Result.ROLL_NO;
                        }
                        else
                        {
                            string namePart = !string.IsNullOrEmpty(Result.NAME) && Result.NAME.Length >= 3 ? Result.NAME.Substring(0, 3) : (!string.IsNullOrEmpty(Result.NAME) ? Result.NAME : "Unk");
                            string dobPart = string.Empty;
                            if (!string.IsNullOrEmpty(Result.DATE_OF_BIRTH))
                            {
                                string[] dobParts = Result.DATE_OF_BIRTH.Split('-');
                                if (dobParts.Length == 3) // Assuming yyyy-mm-dd format from type="date"
                                {
                                    dobPart = dobParts[2] + "_" + dobParts[1] + "_" + dobParts[0];
                                }
                                else
                                {
                                    dobPart = Result.DATE_OF_BIRTH.Replace("/", "_").Replace("-", "_");
                                }
                            }
                            uniqueIdentifier = namePart + "_" + dobPart;
                        }

                        string fileName = uniqueIdentifier + fileExtension;
                        string sFilePath = sDirectorypath + fileName;
                        string sSavePath = "\\" + Common.URLPages.ALUMNE_STUDENT_IMGAEPATH + fileName;

                        if (System.IO.File.Exists(sFilePath))
                        {
                            System.IO.File.Delete(sFilePath);
                        }

                        PHOTO_FILE.SaveAs(sFilePath);
                        Result.PHOTO = sSavePath;
                    }
                    else if (!isUpdate)
                    {
                        Result.PHOTO = string.Empty;
                    }

                    var sqlCommand = isUpdate ? AlumnaeSQLCommands.UpdateAlumniDataCollectionInfo : AlumnaeSQLCommands.SaveAlumniDataCollectionInfo;
                    resultArgs = CMSHandler.SaveOrUpdate(Result, AlumnaeSQL.GetAlumnaeSQL(sqlCommand), sAcademicYear, true);

                    objJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                    objJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;

                    if (resultArgs.Success)
                    {
                        objJsonData.oResult = new { PHOTO_PATH = Result.PHOTO };
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Alumnae", "SaveAlumniDataCollection", ex.Message);
                    }
                    objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    objJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                objJsonData.Message = Common.ErrorMessage.BadRequest;
            }
            return Content(JsonConvert.SerializeObject(objJsonData), "application/json");
        }

        [UserRights("ADMIN")]
        public ActionResult ListAlumniDataCollection()
        {
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserRoleId = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            var dataList = new List<ALUMNI_DATA_COLLECTION>();
            try
            {
                if (sUserRoleId == "7")
                {
                    var dataSource = CMSHandler.FetchData<ALUMNI_DATA_COLLECTION>(null, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAllAlumniDataCollectionforHOD));
                    if (dataSource != null && dataSource.DataSource.Data != null)
                    {
                        dataList = (List<ALUMNI_DATA_COLLECTION>)dataSource.DataSource.Data;
                    }
                }
                else
                {
                    var dataSource = CMSHandler.FetchData<ALUMNI_DATA_COLLECTION>(null, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAllAlumniDataCollection));
                    if (dataSource != null && dataSource.DataSource.Data != null)
                    {
                        dataList = (List<ALUMNI_DATA_COLLECTION>)dataSource.DataSource.Data;
                    }

                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Alumnae", "ListAlumniDataCollection", ex.Message);
                }
            }
            return View(dataList);
        }

        [UserRights("ADMIN")]
        public ActionResult ProgrammeWiseAlumniReport()
        {
            var objViewModel = new ExaminationViewModel();
            objViewModel.liProgrammeWiseAlumniReport = new List<PROGRAMME_WISE_ALUMNI_REPORT>();
            try
            {
                var dataSource = CMSHandler.FetchData<PROGRAMME_WISE_ALUMNI_REPORT>(null, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammeWiseAlumniReport));
                if (dataSource != null && dataSource.DataSource.Data != null)
                {
                    objViewModel.liProgrammeWiseAlumniReport = (List<PROGRAMME_WISE_ALUMNI_REPORT>)dataSource.DataSource.Data;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Alumnae", "ProgrammeWiseAlumniReport", ex.Message);
                }
            }
            return View(objViewModel);
        }

        public ActionResult ExportProgrammeWiseAlumniReportToExcel()
        {
            if (Session[Common.SESSION_VARIABLES.USER_ID] == null)
            {
                return RedirectToAction("Index", "Account");
            }
            try
            {
                var dataSource = CMSHandler.FetchData<PROGRAMME_WISE_ALUMNI_REPORT>(null, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammeWiseAlumniReport));
                var dataList = new List<PROGRAMME_WISE_ALUMNI_REPORT>();

                if (dataSource != null && dataSource.DataSource.Data != null)
                {
                    dataList = (List<PROGRAMME_WISE_ALUMNI_REPORT>)dataSource.DataSource.Data;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("<table border='1'>");

                // Add Title
                sb.Append("<tr><th colspan='4' style='background-color: #87CEEB; font-size: 16pt; font-weight: bold; text-align: center;'>PROGRAMME WISE ALUMNI REPORT</th></tr>");

                // Add Headers
                sb.Append("<tr style='background-color: #D3D3D3;'>");
                string[] headers = { "#", "Programme Name", "Application Type", "Count" };
                foreach (string header in headers)
                {
                    sb.Append("<th style='font-weight: bold;'>" + header + "</th>");
                }
                sb.Append("</tr>");

                // Add Data rows
                int sno = 1;
                int totalCount = 0;
                foreach (var item in dataList)
                {
                    int currentCount = !string.IsNullOrEmpty(item.COUNT) ? Convert.ToInt32(item.COUNT) : 0;
                    totalCount += currentCount;

                    sb.Append("<tr>");
                    sb.Append("<td style='text-align: center;'>" + (sno++) + "</td>");
                    sb.Append("<td>" + item.PROGRAMME_NAME + "</td>");
                    sb.Append("<td style='text-align: center;'>" + item.APPLICATION_TYPE + "</td>");
                    sb.Append("<td style='text-align: right;'>" + item.COUNT + "</td>");
                    sb.Append("</tr>");
                }

                // Add Summary Footer
                sb.Append("<tr style='background-color: #f9f9f9; font-weight: bold;'>");
                sb.Append("<td colspan='3' style='text-align: right;'>Total Registered Alumni:</td>");
                sb.Append("<td style='text-align: right; color: #2a72d4;'>" + totalCount + "</td>");
                sb.Append("</tr>");

                sb.Append("</table>");

                string fileName = "ProgrammeWiseAlumniReport_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();

                return View();
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Alumnae", "ExportProgrammeWiseAlumniReportToExcel", ex.Message);
                }
                return Content("Error exporting data: " + ex.Message);
            }
        }

        public ActionResult ExportAlumniDataToExcelByProgramme()
        {
            if (Session[Common.SESSION_VARIABLES.USER_ID] == null)
            {
                return RedirectToAction("Index", "Account");
            }
            try
            {
                var dataSource = CMSHandler.FetchData<ALUMNI_DATA_COLLECTION>(null, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAllAlumniDataCollection));
                var dataList = new List<ALUMNI_DATA_COLLECTION>();

                if (dataSource != null && dataSource.DataSource.Data != null)
                {
                    dataList = (List<ALUMNI_DATA_COLLECTION>)dataSource.DataSource.Data;
                }

                // Group strictly by PROGRAMME
                var grouped = dataList
                    .GroupBy(item => !string.IsNullOrWhiteSpace(item.PROGRAMME) ? item.PROGRAMME.Trim() : "General")
                    .OrderBy(g => g.Key, StringComparer.CurrentCultureIgnoreCase)
                    .ToList();

                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    // Sheet 1: Programme Summary Overview
                    var sumWs = package.Workbook.Worksheets.Add("Programme Summary");
                    PopulateAlumniDataSummaryWorksheet(sumWs, grouped);

                    // Dedicated separate sheet for each PROGRAMME
                    foreach (var group in grouped)
                    {
                        string sheetName = GetUniqueWorksheetName(package, group.Key);
                        var ws = package.Workbook.Worksheets.Add(sheetName);
                        PopulateAlumniDataProgrammeWorksheet(ws, group.Key, group.ToList());
                    }

                    using (var stream = new MemoryStream())
                    {
                        package.SaveAs(stream);
                        string fileName = "AlumniData_ProgrammeWise_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Alumnae", "ExportAlumniDataToExcelByProgramme", ex.Message);
                }
                return Content("Error exporting data: " + ex.Message);
            }
        }

        private string GetUniqueWorksheetName(ExcelPackage package, string progName)
        {
            string baseName = string.IsNullOrWhiteSpace(progName) ? "General" : progName.Trim();
            baseName = Regex.Replace(baseName, @"[\\/*?:\[\]]", "");
            baseName = baseName.Trim('\'').Trim();
            if (baseName.Length == 0) baseName = "General";
            if (baseName.Length > 28) baseName = baseName.Substring(0, 28).Trim();

            string uniqueName = baseName;
            int counter = 1;
            while (package.Workbook.Worksheets[uniqueName] != null)
            {
                string suffix = "_" + counter;
                int maxBaseLen = 31 - suffix.Length;
                string trimmedBase = (baseName.Length > maxBaseLen) ? baseName.Substring(0, maxBaseLen) : baseName;
                uniqueName = trimmedBase + suffix;
                counter++;
            }
            return uniqueName;
        }

        private void PopulateAlumniDataSummaryWorksheet(ExcelWorksheet ws, List<IGrouping<string, ALUMNI_DATA_COLLECTION>> grouped)
        {
            int totalAlumni = grouped.Sum(g => g.Count());

            // College Header Row 1
            ws.Cells[1, 1].Value = "ST. JOSEPH'S COLLEGE OF ARTS AND SCIENCE (AUTONOMOUS), CUDDALORE";
            ws.Cells[1, 1, 1, 3].Merge = true;
            ws.Cells[1, 1].Style.Font.Size = 13;
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.Font.Color.SetColor(Color.White);
            ws.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(30, 58, 138));
            ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(1).Height = 26;

            // Subtitle Row 2
            ws.Cells[2, 1].Value = "ALUMNI DATA COLLECTION - PROGRAMME-WISE SUMMARY";
            ws.Cells[2, 1, 2, 3].Merge = true;
            ws.Cells[2, 1].Style.Font.Size = 11;
            ws.Cells[2, 1].Style.Font.Bold = true;
            ws.Cells[2, 1].Style.Font.Color.SetColor(Color.FromArgb(30, 58, 138));
            ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(2).Height = 22;

            // Info Row 3
            ws.Cells[3, 1].Value = "Total Programmes: " + grouped.Count + " | Total Registered Alumni: " + totalAlumni + " | Generated On: " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt");
            ws.Cells[3, 1, 3, 3].Merge = true;
            ws.Cells[3, 1].Style.Font.Size = 9.5f;
            ws.Cells[3, 1].Style.Font.Italic = true;
            ws.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[3, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(3).Height = 20;

            // Table Header Row 5
            string[] sumHeaders = { "#", "Programme Name", "Registered Count" };
            for (int c = 0; c < sumHeaders.Length; c++)
            {
                var cell = ws.Cells[5, c + 1];
                cell.Value = sumHeaders[c];
                cell.Style.Font.Bold = true;
                cell.Style.Font.Color.SetColor(Color.White);
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 99, 235));
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(203, 213, 225));
                cell.Style.HorizontalAlignment = (c == 0 || c == 2) ? ExcelHorizontalAlignment.Center : ExcelHorizontalAlignment.Left;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            ws.Row(5).Height = 24;

            int rowIdx = 6;
            int sNo = 1;
            foreach (var grp in grouped)
            {
                ws.Cells[rowIdx, 1].Value = sNo++;
                ws.Cells[rowIdx, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells[rowIdx, 2].Value = grp.Key;

                ws.Cells[rowIdx, 3].Value = grp.Count();
                ws.Cells[rowIdx, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIdx, 3].Style.Font.Bold = true;

                for (int c = 1; c <= 3; c++)
                {
                    ws.Cells[rowIdx, c].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(226, 232, 240));
                    ws.Cells[rowIdx, c].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                rowIdx++;
            }

            // Total row
            ws.Cells[rowIdx, 1].Value = "";
            ws.Cells[rowIdx, 2].Value = "TOTAL REGISTERED ALUMNI";
            ws.Cells[rowIdx, 2].Style.Font.Bold = true;
            ws.Cells[rowIdx, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            ws.Cells[rowIdx, 3].Value = totalAlumni;
            ws.Cells[rowIdx, 3].Style.Font.Bold = true;
            ws.Cells[rowIdx, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIdx, 3].Style.Font.Color.SetColor(Color.FromArgb(30, 58, 138));

            for (int c = 1; c <= 3; c++)
            {
                ws.Cells[rowIdx, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[rowIdx, c].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(241, 245, 249));
                ws.Cells[rowIdx, c].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(203, 213, 225));
            }
            ws.Row(rowIdx).Height = 22;

            ws.Column(1).Width = 8;
            ws.Column(2).Width = 45;
            ws.Column(3).Width = 20;
        }

        private void PopulateAlumniDataProgrammeWorksheet(ExcelWorksheet ws, string progName, List<ALUMNI_DATA_COLLECTION> alumniList)
        {
            string[] headers = {
                "#", "Salutation", "Name", "Roll No", "Aadhar No", "Programme",
                "Branch UG", "Branch PG", "Branch MPhil", "Branch PhD", "Batch",
                "Stream", "DOB", "Mobile", "Email", "Status", "Designation",
                "Company", "Permanent Address", "Communication Address", "Other Details"
            };

            int maxCols = headers.Length;
            int totCount = alumniList.Count;

            // College Header Row 1
            ws.Cells[1, 1].Value = "ST. JOSEPH'S COLLEGE OF ARTS AND SCIENCE (AUTONOMOUS), CUDDALORE";
            ws.Cells[1, 1, 1, maxCols].Merge = true;
            ws.Cells[1, 1].Style.Font.Size = 13;
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.Font.Color.SetColor(Color.White);
            ws.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(30, 58, 138));
            ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(1).Height = 26;

            // Subtitle Row 2
            ws.Cells[2, 1].Value = progName.ToUpper() + " - REGISTERED ALUMNI";
            ws.Cells[2, 1, 2, maxCols].Merge = true;
            ws.Cells[2, 1].Style.Font.Size = 11;
            ws.Cells[2, 1].Style.Font.Bold = true;
            ws.Cells[2, 1].Style.Font.Color.SetColor(Color.FromArgb(30, 58, 138));
            ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(2).Height = 22;

            // Info Row 3
            ws.Cells[3, 1].Value = "Programme: " + progName + " | Total Registered: " + totCount + " | Generated On: " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt");
            ws.Cells[3, 1, 3, maxCols].Merge = true;
            ws.Cells[3, 1].Style.Font.Size = 9.5f;
            ws.Cells[3, 1].Style.Font.Italic = true;
            ws.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[3, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(3).Height = 20;

            // Table Header Row 5
            for (int col = 0; col < headers.Length; col++)
            {
                var cell = ws.Cells[5, col + 1];
                cell.Value = headers[col];
                cell.Style.Font.Bold = true;
                cell.Style.Font.Color.SetColor(Color.White);
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 99, 235));
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(203, 213, 225));
                cell.Style.HorizontalAlignment = (col == 0 || col == 3 || col == 4 || col == 10 || col == 11 || col == 12 || col == 13)
                    ? ExcelHorizontalAlignment.Center
                    : ExcelHorizontalAlignment.Left;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            ws.Row(5).Height = 24;

            // Order by Batch (chronological), then alphabetical Name (A to Z)
            var sortedList = alumniList
                .OrderBy(x => {
                    string b = (x.BATCH ?? "").Trim();
                    if (string.IsNullOrEmpty(b)) return 999999;
                    var m = System.Text.RegularExpressions.Regex.Match(b, @"\d+");
                    int y;
                    if (m.Success && int.TryParse(m.Value, out y)) return y;
                    return 999998;
                })
                .ThenBy(x => (x.BATCH ?? "").Trim(), StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(x => (x.NAME ?? "").Trim(), StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            int rIdx = 6;
            int sno = 1;
            foreach (var item in sortedList)
            {
                ws.Cells[rIdx, 1].Value = sno++;
                ws.Cells[rIdx, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells[rIdx, 2].Value = item.SALUTATION ?? "";
                ws.Cells[rIdx, 3].Value = item.NAME ?? "";
                ws.Cells[rIdx, 3].Style.Font.Bold = true;

                ws.Cells[rIdx, 4].Value = item.ROLL_NO ?? "";
                ws.Cells[rIdx, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells[rIdx, 5].Value = item.AADHAR_NO ?? "";
                ws.Cells[rIdx, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells[rIdx, 6].Value = item.PROGRAMME ?? "";
                ws.Cells[rIdx, 7].Value = item.BRANCH_UG ?? "";
                ws.Cells[rIdx, 8].Value = item.BRANCH_PG ?? "";
                ws.Cells[rIdx, 9].Value = item.BRANCH_MPHIL ?? "";
                ws.Cells[rIdx, 10].Value = item.BRANCH_PHD ?? "";

                ws.Cells[rIdx, 11].Value = item.BATCH ?? "";
                ws.Cells[rIdx, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rIdx, 11].Style.Font.Bold = true;

                ws.Cells[rIdx, 12].Value = item.STREAM ?? "";
                ws.Cells[rIdx, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells[rIdx, 13].Value = item.DATE_OF_BIRTH ?? "";
                ws.Cells[rIdx, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells[rIdx, 14].Value = item.MOBILE_NO ?? "";
                ws.Cells[rIdx, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells[rIdx, 15].Value = item.EMAIL_ID ?? "";
                ws.Cells[rIdx, 16].Value = item.PRESENT_STATUS ?? "";
                ws.Cells[rIdx, 17].Value = item.DESIGNATION ?? "";
                ws.Cells[rIdx, 18].Value = item.COMPANY ?? "";
                ws.Cells[rIdx, 19].Value = item.PERMANENT_ADDRESS ?? "";
                ws.Cells[rIdx, 20].Value = item.COMMUNICATION_ADDRESS ?? "";
                ws.Cells[rIdx, 21].Value = item.OTHER_DETAILS ?? "";

                for (int c = 1; c <= maxCols; c++)
                {
                    ws.Cells[rIdx, c].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(226, 232, 240));
                    ws.Cells[rIdx, c].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                rIdx++;
            }

            // Total row
            ws.Cells[rIdx, 1].Value = "";
            ws.Cells[rIdx, 2].Value = "TOTAL FOR " + progName.ToUpper();
            ws.Cells[rIdx, 2].Style.Font.Bold = true;
            ws.Cells[rIdx, 2, rIdx, 3].Merge = true;
            ws.Cells[rIdx, 4].Value = totCount;
            ws.Cells[rIdx, 4].Style.Font.Bold = true;
            ws.Cells[rIdx, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            for (int c = 1; c <= maxCols; c++)
            {
                ws.Cells[rIdx, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[rIdx, c].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(241, 245, 249));
                ws.Cells[rIdx, c].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(203, 213, 225));
            }
            ws.Row(rIdx).Height = 22;

            // Column auto-fit with constraints
            ws.Cells[5, 1, rIdx, maxCols].AutoFitColumns(12, 40);
            ws.Column(1).Width = 8;   // #
            ws.Column(4).Width = 16;  // Roll No
            ws.Column(11).Width = 14; // Batch
            ws.Column(14).Width = 16; // Mobile
        }

        public ActionResult ListPendingAlumniDataCollection()
        {
            var dataList = new List<ALUMNI_DATA_COLLECTION>();
            try
            {
                var dataSource = CMSHandler.FetchData<ALUMNI_DATA_COLLECTION>(null, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchPendingAlumniDataCollection));
                if (dataSource != null && dataSource.DataSource.Data != null)
                {
                    dataList = (List<ALUMNI_DATA_COLLECTION>)dataSource.DataSource.Data;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Alumnae", "ListPendingAlumniDataCollection", ex.Message);
                }
            }
            return View(dataList);
        }

        [HttpPost]
        public JsonResult ImportSelectedToAlumniDataCollection(List<string> rollNos)
        {
            JsonResultData objResult = new JsonResultData();
            int importedCount = 0;
            int skippedCount = 0;

            try
            {
                if (rollNos != null && rollNos.Count > 0)
                {
                    foreach (var rollNo in rollNos)
                    {
                        // 1. Fetch full profile and address data from ACF and Student tables (rollNo is REGISTER_NO here)
                        var searchObj = new ALUMNI_DATA_COLLECTION { ROLL_NO = rollNo };
                        var profileData = CMSHandler.FetchData<ALUMNI_DATA_COLLECTION>(searchObj, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchStudentInfoForAlumniImportByRollNo));

                        if (profileData != null && profileData.DataSource.Data != null)
                        {
                            var students = (List<ALUMNI_DATA_COLLECTION>)profileData.DataSource.Data;
                            if (students.Count > 0)
                            {
                                var student = students[0];

                                // 2. Check if student already exists in alumni_data_collection by actual ROLL_NO (e.g. A16MCH10)
                                // or their REGISTER_NO (e.g. 10723PENG012)
                                var checkExistRoll = CMSHandler.FetchData<ALUMNI_DATA_COLLECTION>(new ALUMNI_DATA_COLLECTION { ROLL_NO = student.ROLL_NO }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAlumniDataCollectionInfoByRollNo));
                                var checkExistReg = CMSHandler.FetchData<ALUMNI_DATA_COLLECTION>(new ALUMNI_DATA_COLLECTION { ROLL_NO = rollNo }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAlumniDataCollectionInfoByRollNo));

                                bool existsInAlumni = (checkExistRoll != null && checkExistRoll.DataSource.Data != null && ((List<ALUMNI_DATA_COLLECTION>)checkExistRoll.DataSource.Data).Count > 0) ||
                                                      (checkExistReg != null && checkExistReg.DataSource.Data != null && ((List<ALUMNI_DATA_COLLECTION>)checkExistReg.DataSource.Data).Count > 0);

                                if (existsInAlumni)
                                {
                                    skippedCount++;
                                    continue; // Skip already existing roll/register numbers
                                }

                                student.BRANCH_UG = "NA";
                                student.BRANCH_PG = "NA";
                                student.BRANCH_MPHIL = "NA";
                                student.BRANCH_PHD = "NA";
                                student.PRESENT_STATUS = "";

                                // 3. Insert into alumni_data_collection table
                                var saveResult = CMSHandler.SaveOrUpdate(student, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.SaveAlumniDataCollectionInfo), student.ACADEMIC_YEAR);

                                if (saveResult.Success)
                                {
                                    importedCount++;
                                }
                            }
                        }
                    }

                    objResult.ErrorCode = Common.ErrorCode.Ok;
                    objResult.Message = string.Format("Successfully imported {0} students. (Skipped {1} already existing).", importedCount, skippedCount);
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                    objResult.Message = "No students selected.";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Alumnae", "ImportSelectedToAlumniDataCollection", ex.Message);
                }
                objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                objResult.Message = "Internal Server Error: " + ex.Message;
            }

            return Json(objResult);
        }

        [HttpPost]
        public ActionResult GetAlumniDataCollectionProfile(string rollNo)
        {
            try
            {
                var searchObj = new ALUMNI_DATA_COLLECTION { ROLL_NO = rollNo };
                var dataSource = CMSHandler.FetchData<ALUMNI_DATA_COLLECTION>(searchObj, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAlumniDataCollectionInfoByRollNo));
                if (dataSource != null && dataSource.DataSource.Data != null)
                {
                    var dataList = (List<ALUMNI_DATA_COLLECTION>)dataSource.DataSource.Data;
                    if (dataList.Count > 0)
                    {
                        return PartialView("_AlumniDataCollectionProfile", dataList[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Alumnae", "GetAlumniDataCollectionProfile", ex.Message);
                }
            }
            return Content("Data not found");
        }
        public ActionResult ExportAlumniDataToExcel()
        {
            if (Session[Common.SESSION_VARIABLES.USER_ID] == null)
            {
                return RedirectToAction("Index", "Account");
            }
            try
            {
                var dataSource = CMSHandler.FetchData<ALUMNI_DATA_COLLECTION>(null, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAllAlumniDataCollection));
                var dataList = new List<ALUMNI_DATA_COLLECTION>();

                if (dataSource != null && dataSource.DataSource.Data != null)
                {
                    dataList = (List<ALUMNI_DATA_COLLECTION>)dataSource.DataSource.Data;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("<table border='1'>");

                // Add Title
                sb.Append("<tr><th colspan='19' style='background-color: #87CEEB; font-size: 16pt; font-weight: bold; text-align: center;'>ALUMNI DATA COLLECTION REPORT</th></tr>");

                // Add Headers
                sb.Append("<tr style='background-color: #D3D3D3;'>");
                string[] headers = { "#", "Salutation", "Name", "Roll No", "Programme", "Branch UG", "Branch PG", "Branch MPhil", "Branch PhD", "Batch", "Stream", "DOB", "Mobile", "Email", "Status", "Designation", "Company", "Permanent Address", "Communication Address" };
                foreach (string header in headers)
                {
                    sb.Append("<th style='font-weight: bold;'>" + header + "</th>");
                }
                sb.Append("</tr>");

                // Add Data rows
                var sortedDataList = dataList
                    .OrderBy(x => (x.PROGRAMME ?? "").Trim(), StringComparer.CurrentCultureIgnoreCase)
                    .ThenBy(x => {
                        string b = (x.BATCH ?? "").Trim();
                        if (string.IsNullOrEmpty(b)) return 999999;
                        var m = System.Text.RegularExpressions.Regex.Match(b, @"\d+");
                        int y;
                        if (m.Success && int.TryParse(m.Value, out y)) return y;
                        return 999998;
                    })
                    .ThenBy(x => (x.BATCH ?? "").Trim(), StringComparer.CurrentCultureIgnoreCase)
                    .ThenBy(x => (x.NAME ?? "").Trim(), StringComparer.CurrentCultureIgnoreCase)
                    .ToList();

                int sno = 1;
                foreach (var item in sortedDataList)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + sno++ + "</td>");
                    sb.Append("<td>" + item.SALUTATION + "</td>");
                    sb.Append("<td>" + item.NAME + "</td>");
                    sb.Append("<td>" + item.ROLL_NO + "</td>");
                    sb.Append("<td>" + item.PROGRAMME + "</td>");
                    sb.Append("<td>" + item.BRANCH_UG + "</td>");
                    sb.Append("<td>" + item.BRANCH_PG + "</td>");
                    sb.Append("<td>" + item.BRANCH_MPHIL + "</td>");
                    sb.Append("<td>" + item.BRANCH_PHD + "</td>");
                    sb.Append("<td>" + item.BATCH + "</td>");
                    sb.Append("<td>" + item.STREAM + "</td>");
                    sb.Append("<td>" + item.DATE_OF_BIRTH + "</td>");
                    sb.Append("<td>" + item.MOBILE_NO + "</td>");
                    sb.Append("<td>" + item.EMAIL_ID + "</td>");
                    sb.Append("<td>" + item.PRESENT_STATUS + "</td>");
                    sb.Append("<td>" + item.DESIGNATION + "</td>");
                    sb.Append("<td>" + item.COMPANY + "</td>");
                    sb.Append("<td>" + item.PERMANENT_ADDRESS + "</td>");
                    sb.Append("<td>" + item.COMMUNICATION_ADDRESS + "</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table>");

                string fileName = "AlumniDataCollection_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();

                return View();
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Alumnae", "ExportAlumniDataToExcel", ex.Message);
                }
                return Content("Error exporting data: " + ex.Message);
            }
        }

        public ActionResult AluminiReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserRoleId = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    List<Sup_Shift> liShift = new List<Sup_Shift>();
                    liShift = (List<Sup_Shift>)CMS.DAO.CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (liShift != null && liShift.Count > 0)
                    {
                        objViewModel.ShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    var programmeLevel = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType)).DataSource.Data;
                    if (programmeLevel != null && programmeLevel.Count > 0)
                    {
                        objViewModel.Programmelevel = new SelectList(programmeLevel, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                    }
                    if (sUserRoleId == Common.SupRole.HEAD_OF_THE_DEPARTMENT)
                    {

                        var liprogramme = (List<cp_Program_2017>)CMSHandler.FetchData<cp_Program_2017>(new stf_Personal_Info() { STAFF_ID = sUserId },
                       AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammelistfordqacforHOD), sAcademicYear).DataSource.Data;
                        if (liprogramme != null && liprogramme.Count > 0)
                        {
                            objViewModel.Programmelist = new SelectList(liprogramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION);
                        }
                    }
                    else
                    {
                        var liprogramme = (List<cp_Program_2017>)CMSHandler.FetchData<cp_Program_2017>(new stf_Personal_Info() { STAFF_ID = sUserId },
                        AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammelistfordqac), sAcademicYear).DataSource.Data;
                        if (liprogramme != null && liprogramme.Count > 0)
                        {
                            objViewModel.Programmelist = new SelectList(liprogramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION);
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "AluminiReport", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }

        [HttpPost]
        public ActionResult BindAluminiReport(string sShiftId, string sLevelId, string sProgrammeId, string sFromDate, string sToDate)
        {
            var dataList = new List<ALUMNI_DATA_COLLECTION>();
            try
            {
                sFromDate = !string.IsNullOrEmpty(sFromDate) ? DateTime.ParseExact(sFromDate.Replace("-", "/"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : string.Empty;
                sToDate = !string.IsNullOrEmpty(sToDate) ? DateTime.ParseExact(sToDate.Replace("-", "/"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : string.Empty;

                string sqlQuery = AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAlumniReportData)
                                  .Replace("?SHIFT_ID", sShiftId ?? string.Empty)
                                  .Replace("?PROGRAMME_ID", sProgrammeId ?? string.Empty)
                                  .Replace("?FROM_DATE", sFromDate ?? string.Empty)
                                  .Replace("?TO_DATE", sToDate ?? string.Empty);

                var dataSource = CMSHandler.FetchData<ALUMNI_DATA_COLLECTION>(null, sqlQuery);
                if (dataSource != null && dataSource.DataSource.Data != null)
                {
                    dataList = (List<ALUMNI_DATA_COLLECTION>)dataSource.DataSource.Data;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "BindAluminiReport", ex.Message);
                }
            }
            return PartialView("_BindAluminiReport", dataList);
        }

        [HttpGet]
        public ActionResult ExportFilteredAlumniDataToExcel(string sShiftId, string sLevelId, string sProgrammeId, string sFromDate, string sToDate)
        {
            if (Session[Common.SESSION_VARIABLES.USER_ID] == null)
            {
                return RedirectToAction("Index", "Account");
            }
            try
            {
                sFromDate = !string.IsNullOrEmpty(sFromDate) ? DateTime.ParseExact(sFromDate.Replace("-", "/"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : string.Empty;
                sToDate = !string.IsNullOrEmpty(sToDate) ? DateTime.ParseExact(sToDate.Replace("-", "/"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : string.Empty;

                string sqlQuery = AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchAlumniReportData)
                                  .Replace("?SHIFT_ID", sShiftId ?? string.Empty)
                                  .Replace("?PROGRAMME_ID", sProgrammeId ?? string.Empty)
                                  .Replace("?FROM_DATE", sFromDate ?? string.Empty)
                                  .Replace("?TO_DATE", sToDate ?? string.Empty);

                var dataSource = CMSHandler.FetchData<ALUMNI_DATA_COLLECTION>(null, sqlQuery);
                var dataList = new List<ALUMNI_DATA_COLLECTION>();

                if (dataSource != null && dataSource.DataSource.Data != null)
                {
                    dataList = (List<ALUMNI_DATA_COLLECTION>)dataSource.DataSource.Data;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("<table border='1'>");

                // Add Title
                sb.Append("<tr><th colspan='19' style='background-color: #87CEEB; font-size: 16pt; font-weight: bold; text-align: center;'>ALUMNI DATA COLLECTION REPORT</th></tr>");

                // Add Headers
                sb.Append("<tr style='background-color: #D3D3D3;'>");
                string[] headers = { "#", "Salutation", "Name", "Roll No", "Programme", "Branch UG", "Branch PG", "Branch MPhil", "Branch PhD", "Batch", "Stream", "DOB", "Mobile", "Email", "Status", "Designation", "Company", "Permanent Address", "Communication Address" };
                foreach (string header in headers)
                {
                    sb.Append("<th style='font-weight: bold;'>" + header + "</th>");
                }
                sb.Append("</tr>");

                // Add Data rows
                int sno = 1;
                foreach (var item in dataList)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + sno++ + "</td>");
                    sb.Append("<td>" + item.SALUTATION + "</td>");
                    sb.Append("<td>" + item.NAME + "</td>");
                    sb.Append("<td>" + item.ROLL_NO + "</td>");
                    sb.Append("<td>" + item.PROGRAMME + "</td>");
                    sb.Append("<td>" + item.BRANCH_UG + "</td>");
                    sb.Append("<td>" + item.BRANCH_PG + "</td>");
                    sb.Append("<td>" + item.BRANCH_MPHIL + "</td>");
                    sb.Append("<td>" + item.BRANCH_PHD + "</td>");
                    sb.Append("<td>" + item.BATCH + "</td>");
                    sb.Append("<td>" + item.SHIFT_NAME + "</td>");
                    sb.Append("<td>" + item.DATE_OF_BIRTH + "</td>");
                    sb.Append("<td>" + item.MOBILE_NO + "</td>");
                    sb.Append("<td>" + item.EMAIL_ID + "</td>");
                    sb.Append("<td>" + item.PRESENT_STATUS + "</td>");
                    sb.Append("<td>" + item.DESIGNATION + "</td>");
                    sb.Append("<td>" + item.COMPANY + "</td>");
                    sb.Append("<td>" + item.PERMANENT_ADDRESS + "</td>");
                    sb.Append("<td>" + item.COMMUNICATION_ADDRESS + "</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table>");

                string fileName = "Alumni_Report_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "ExportFilteredAlumniDataToExcel", ex.Message);
                }
                return Content("Error exporting data: " + ex.Message);
            }
        }



        #endregion


        #region convocation report class wise 
        public ActionResult ConvocationEligiPleStudentReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserRoleId = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    List<Sup_Shift> liShift = new List<Sup_Shift>();
                    liShift = (List<Sup_Shift>)CMS.DAO.CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (liShift != null && liShift.Count > 0)
                    {
                        objViewModel.ShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    var programmeLevel = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType)).DataSource.Data;
                    if (programmeLevel != null && programmeLevel.Count > 0)
                    {
                        objViewModel.Programmelevel = new SelectList(programmeLevel, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                    }
                    var AcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcademicYear != null && AcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYear = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    if (sUserRoleId == Common.SupRole.HEAD_OF_THE_DEPARTMENT)
                    {

                        var liprogramme = (List<cp_Program_2017>)CMSHandler.FetchData<cp_Program_2017>(new stf_Personal_Info() { STAFF_ID = sUserId },
                        AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammelistfordqacforHOD), sAcademicYear).DataSource.Data;
                        if (liprogramme != null && liprogramme.Count > 0)
                        {
                            objViewModel.Programmelist = new SelectList(liprogramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION);
                        }
                    }
                    else
                    {
                        var liprogramme = (List<cp_Program_2017>)CMSHandler.FetchData<cp_Program_2017>(new stf_Personal_Info() { STAFF_ID = sUserId },
                     AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchProgrammelistfordqac), sAcademicYear).DataSource.Data;
                        if (liprogramme != null && liprogramme.Count > 0)
                        {
                            objViewModel.Programmelist = new SelectList(liprogramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION);
                        }
                    }
                    var liconvocation = (List<CONVOCATION_SETTING>)CMSHandler.FetchData<CONVOCATION_SETTING>(new CONVOCATION_SETTINGS() { IS_ACTIVE = "1" },
                         AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.Fetchconvocationsetting), sAcademicYear).DataSource.Data;
                    if (liconvocation != null && liconvocation.Count > 0)
                    {
                        objViewModel.Convocationsettinglist = new SelectList(liconvocation, Common.CON_SETTING.SETTING_ID, Common.CON_SETTING.SETTING_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AlumnaeController", "ConvocationPaymentReport", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }

        public ActionResult BindConvocationEligibleStudentReport(string sProgrammeId, string sSETTING, string sFromDate, string sToDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            try
            {
                var frequenc_type = Common.FrequencyType.ConvocationFee;
                sFromDate = CommonMethods.ConvertdatetoyearFromat(sFromDate);
                sToDate = CommonMethods.ConvertdatetoyearFromat(sToDate);
                objViewModel.liStudentList = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(new ListStudents() { FEE_MODE = frequenc_type, SETTING_ID = sSETTING, PROGRAMME_ID = sProgrammeId }, AlumnaeSQL.GetAlumnaeSQL(AlumnaeSQLCommands.FetchConvocationEligbleStudnet).Replace("?PROGRAMME_ID", sProgrammeId).Replace("?FROM_DATE", sFromDate).Replace("?TO_DATE", sToDate), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AlumnaeController", "BindConvocationPaymentReport", ex.Message);
                }
            }
            return PartialView(objViewModel);
        }
        #endregion

    }
}