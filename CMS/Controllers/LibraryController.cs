using CMS.DAO;
using CMS.Models;
using CMS.SQL.Examination;
using CMS.SQL.Library;
using CMS.SQL.LMS;
using CMS.SQL.Student;
using CMS.SQL.SupportData;
using CMS.SQL.Website;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace CMS.Controllers
{
    public class LibraryController : Controller
    {
        // GET: Library
        ResultArgs resultArgs = new ResultArgs();
        public string STATUS;

        public ActionResult Index()
        {
            return View();
        }

        #region User Details

        public ActionResult LiUserList()
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                // objViewModel.liExternalUserInfo = (List<LI_EXTERNAL_USER_INFO>)CMSHandler.FetchData<LI_EXTERNAL_USER_INFO>(null, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiMemberDetails), sAcademicYear).DataSource.Data;
            }
            return View(objViewModel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult AddLiMember(string sUserId)
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                //var MemberCategory = (List<SUP_USER_TYPE>)CMSHandler.FetchData<SUP_USER_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiMemberCategory), sAcademicYear).DataSource.Data;
                var liUserType = (List<SUP_USER_TYPE>)CMSHandler.FetchData<SUP_USER_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupUserType)).DataSource.Data;

                if (liUserType != null && liUserType.Count > 0)
                {
                    objViewModel.UserType = new SelectList(liUserType, Common.SUP_USER_TYPE.USER_TYPE_ID, Common.SUP_USER_TYPE.USER_TYPE_NAME);
                }

                var FetchBloodGroup = (List<SupBloodGroup>)CMSHandler.FetchData<SupBloodGroup>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchBloodGroup), sAcademicYear).DataSource.Data;
                if (FetchBloodGroup != null && FetchBloodGroup.Count > 0)
                {
                    objViewModel.BloodGroup = new SelectList(FetchBloodGroup, Common.SUP_BLOOD_GROUP.BLOOD_GROUP_ID, Common.SUP_BLOOD_GROUP.BLOOD_GROUP_NAME);
                }

                //Fetch Degree
                var Degree = (List<SUP_DEGREE>)CMSHandler.FetchData<SUP_DEGREE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDegree), sAcademicYear).DataSource.Data;
                if (Degree != null && Degree.Count > 0)
                {
                    objViewModel.Degree = new SelectList(Degree, Common.SUP_DEGREE.DEGREE_ID, Common.SUP_DEGREE.DEGREE);
                }

                if (!string.IsNullOrEmpty(sUserId))
                {
                    objViewModel.ObjExternalUserInfo = FetchLiUserDetailsById(sUserId);
                }
            }
            return View(objViewModel);
        }

        /// <summary>
        /// for fetching user details either student roll no or staff code 
        /// </summary>
        /// <param name="sId"></param>
        /// <returns>
        /// it returns user details
        /// </returns>
        public JsonResult FetchUserInformationByRollnoandStaffCode(string sId)
        {
            JsonResultData objResultData = new JsonResultData();

            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                //Fetch user details by roll no
                var UserDetails = (List<USER_DETAILS_FIELDS>)CMSHandler.FetchData<USER_DETAILS_FIELDS>(new STU_PERSONAL_INFO { ROLL_NO = sId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserDetailsByRollno), sAcademicYear).DataSource.Data;

                if (UserDetails == null || UserDetails.Count < 0)
                {
                    //fetch UserDetails by StaffCode
                    var UserDetails1 = (List<USER_DETAILS_FIELDS>)CMSHandler.FetchData<USER_DETAILS_FIELDS>(new STF_PERSONAL_INFO() { STAFF_CODE = sId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserDetailsByStaffCode), sAcademicYear).DataSource.Data;
                    if (UserDetails1 == null || UserDetails1.Count < 0)
                    {
                        objResultData.Message = "No Matching Record Found";
                        return Json(objResultData);
                    }
                    else
                    {
                        //UserDetails.FirstOrDefault().MEMBER_TYPE = Common.LI_MEMBER_CATEGORY.TEACHING_STAFF;
                        objResultData.oResult = UserDetails1;
                    }
                }
                else
                {
                    //UserDetails.FirstOrDefault().MEMBER_TYPE =Common.LI_MEMBER_CATEGORY.STUDENT;
                    objResultData.oResult = UserDetails;
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(objResultData);
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchUserDetailsByUniqueFileds(string sFlag, string sFiledValue)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var liUserInfo = new List<LI_EXTERNAL_USER_INFO>();
            try
            {
                switch (sFlag)
                {
                    case "1":
                        {
                            liUserInfo = (List<LI_EXTERNAL_USER_INFO>)CMSHandler.FetchData<LI_EXTERNAL_USER_INFO>(new LI_USER_DETAILS() { RFID = sFiledValue }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiMemberDetailsByRfid)).DataSource.Data;
                            break;
                        }
                    case "2":
                        {
                            liUserInfo = (List<LI_EXTERNAL_USER_INFO>)CMSHandler.FetchData<LI_EXTERNAL_USER_INFO>(new LI_USER_DETAILS() { BAR_CODE = sFiledValue }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiMemberDetailsByBarCode)).DataSource.Data;
                            break;
                        }
                    case "3":
                        {
                            liUserInfo = (List<LI_EXTERNAL_USER_INFO>)CMSHandler.FetchData<LI_EXTERNAL_USER_INFO>(new LI_USER_DETAILS() { BOID = sFiledValue }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiMemberDetailsByBoid)).DataSource.Data;
                            break;
                        }
                    default:
                        break;
                }
                objResultData.oResult = liUserInfo;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("LibraryController", "FetchUserDetailsByUniqueFileds", ex.Message);
                }
            }
            if (liUserInfo == null)
            {
                objResultData.Message = Common.Messages.NoRecordsfound;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchUserDetailsByBoid(string sBoid)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sBoid))
            {
                var Details = (List<LI_USER_DETAILS>)CMSHandler.FetchData<LI_USER_DETAILS>(new LI_USER_DETAILS()
                {
                    BOID = sBoid
                }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiMemberDetailsByBoid)).DataSource.Data;

                if (Details != null && Details.Count > 0)
                {
                    objResultData.oResult = Details;


                    //check if the user is student
                    var userDetails = (List<USER_DETAILS_FIELDS>)CMSHandler.FetchData<USER_DETAILS_FIELDS>(new STU_PERSONAL_INFO() { BOID_NUMBER = Details.FirstOrDefault().BOID },
                        SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchStudentDetailsByBoid), sAcademicYear).DataSource.Data;

                    if (userDetails != null && userDetails.Count > 0)
                    {
                        // check if student left
                        if (userDetails.FirstOrDefault().STATUS == "1")
                        {
                            objResultData.Message = "The student was left";
                            return Json(objResultData, JsonRequestBehavior.AllowGet);
                        }
                        if (userDetails.FirstOrDefault().STATUS1 == "1")
                        {
                            objResultData.Message = "The student was drop";
                            return Json(objResultData, JsonRequestBehavior.AllowGet);
                        }

                        Details.FirstOrDefault().STUDENT_ID = userDetails.FirstOrDefault().USER_ID;
                    }
                    else
                    {
                        //check if the user is staff
                        var userDetails1 = (List<USER_DETAILS_FIELDS>)CMSHandler.FetchData<USER_DETAILS_FIELDS>(new STF_PERSONAL_INFO() { BOID_NUMBER = Details.FirstOrDefault().BOID },
                            SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchStaffDetailsByBoid), sAcademicYear).DataSource.Data;

                        if (userDetails != null && userDetails.Count > 0)
                        {
                            // check if staff left
                            if (userDetails1.FirstOrDefault().STATUS == "1")
                            {
                                objResultData.Message = "The staff was left";
                                return Json(objResultData, JsonRequestBehavior.AllowGet);
                            }
                            if (userDetails1.FirstOrDefault().STATUS1 == "1")
                            {
                                objResultData.Message = "The staff was drop";
                                return Json(objResultData, JsonRequestBehavior.AllowGet);
                            }

                            Details.FirstOrDefault().STAFF_ID = userDetails1.FirstOrDefault().USER_ID;
                        }
                    }
                }
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveLiMember()
        {
            string sDirectorypath = string.Empty, sFilePath = string.Empty;
            var path = string.Empty;
            var StudentorStaffId = string.Empty;
            var liDetails = new List<LI_USER_DETAILS>();
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sJsonData = Request.Form["sJsonData"];
                    var result = JsonConvert.DeserializeObject<LI_EXTERNAL_USER_INFO>(sJsonData);
                    if (result != null)
                    {
                        //generate password
                        if (!string.IsNullOrEmpty(result.PASSWORD))
                        {
                            result.PASSWORD = CommonMethods.GetSha256FromString(result.PASSWORD);
                        }

                        result.PHOTO = Request.Form["PHOTO"];
                        //
                        if (!string.IsNullOrEmpty(result.REGISTERED_DATE))
                        {
                            result.REGISTERED_DATE = CommonMethods.ConvertdatetoyearFromat(result.REGISTERED_DATE);
                        }
                        if (!string.IsNullOrEmpty(result.DATE_OF_BIRTH))
                        {
                            result.DATE_OF_BIRTH = CommonMethods.ConvertdatetoyearFromat(result.DATE_OF_BIRTH);
                        }

                        //Photo savings
                        if (Request.Files.Count > 0)
                        {
                            sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.LIBRARY_MEMBER_IMAGE_PATH + "\\";
                            if (!Directory.Exists(sDirectorypath))
                            {
                                Directory.CreateDirectory(sDirectorypath);
                            }
                            var file = Request.Files[0];
                            var fileName = file.FileName;
                            var sFile = fileName.Split('.');
                            sFilePath = sDirectorypath + "\\" + result.USERNAME + "." + sFile[1];
                            string sSavePath = "\\" + Common.URLPages.LIBRARY_MEMBER_IMAGE_PATH + "\\" + result.USERNAME + "." + sFile[1];
                            file.SaveAs(sFilePath);
                            result.PHOTO = sSavePath;
                        }

                        //check if the rule was added or not
                        string sMessage = string.Empty; //CheckRulesByUserType(result.USER_TYPE);
                        var liUserType = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(result, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.IsRulesExistForUser), sAcademicYear).DataSource.Data;
                        if (liUserType == null || liUserType.Count <= 0)
                        {
                            objResultData.Message = "Oops ! No Rules Found For This User Type !";
                            objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            return Json(objResultData);
                        }


                        if (!string.IsNullOrEmpty(result.USER_ID))
                        {
                            // Update External User
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateExternalUser), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotModified;
                        }
                        else
                        {
                            // Save External User
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveExternalUser), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotModified;
                        }
                    }
                    else
                    {
                        objResultData.Message = Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveLiMember", ex.Message);
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

        public List<LI_USER_DETAILS> ValidateMembership(string sBoidno, string sRfid)
        {
            var result = new List<LI_USER_DETAILS>();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sBoidno))
            {
                result = (List<LI_USER_DETAILS>)CMSHandler.FetchData<LI_USER_DETAILS>(new LI_USER_DETAILS() { BOID = sBoidno }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserDetailsByBoidId), sAcademicYear).DataSource.Data;
                if (result != null && result.Count > 0)
                {
                    //objResultData.Message = "User Already Existed!";
                    return result;
                }
            }

            if (!string.IsNullOrEmpty(sRfid))
            {
                result = (List<LI_USER_DETAILS>)CMSHandler.FetchData<LI_USER_DETAILS>(new LI_USER_DETAILS() { RFID = sRfid }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserDetailsByRfid), sAcademicYear).DataSource.Data;
                if (result != null && result.Count > 0)
                {
                    //objResultData.Message = "User Already Existed!";
                    return result;
                }


            }
            return result;
        }

        // Update Boid and Rfid
        public ResultArgs UpdateBoidandRfid(string sId, LI_EXTERNAL_USER_INFO UserDetails)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {


                if (UserDetails.USER_TYPE.Equals(Common.LI_MEMBER_CATEGORY.STUDENT))
                {



                    resultArgs = CMSHandler.SaveOrUpdate(new STU_PERSONAL_INFO()
                    {
                        BOID_NUMBER = UserDetails.BOID,
                        RFID = UserDetails.RFID,
                        STUDENT_ID = sId,
                        FIRST_NAME = UserDetails.FIRST_NAME,
                        LAST_NAME = UserDetails.LAST_NAME,
                        DATE_OF_BIRTH = UserDetails.DATE_OF_BIRTH,
                        BLOOD_GROUP = UserDetails.BLOOD_GROUP_ID,
                        PHOTO = UserDetails.PHOTO,
                        STU_MOBILENO = UserDetails.MOBILE,
                        STU_EMAILID = UserDetails.EMAIL
                    }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBoidandRfidByStudentId), sAcademicYear);

                }



                if (UserDetails.USER_TYPE.Equals(Common.LI_MEMBER_CATEGORY.TEACHING_STAFF))
                {


                    resultArgs = CMSHandler.SaveOrUpdate(new STF_PERSONAL_INFO()
                    {
                        BOID_NUMBER = UserDetails.BOID,
                        RFID = UserDetails.RFID,
                        STAFF_ID = sId,
                        FIRST_NAME = UserDetails.FIRST_NAME,
                        LAST_NAME = UserDetails.LAST_NAME,
                        DATE_OF_BIRTH = UserDetails.DATE_OF_BIRTH,
                        PHOTO = UserDetails.PHOTO,
                        BLOOD_GROUP = UserDetails.BLOOD_GROUP_ID
                    }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBoidandRfidByStffId), sAcademicYear);


                }


            }
            return resultArgs;
        }

        public LI_EXTERNAL_USER_INFO FetchLiUserDetailsById(string sUserId)
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && sUserId != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objViewModel.liExternalUserInfo = (List<LI_EXTERNAL_USER_INFO>)CMSHandler.FetchData<LI_EXTERNAL_USER_INFO>(new LI_USER_DETAILS() { USER_ID = sUserId }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiMemberDetailsById), sAcademicYear).DataSource.Data;

                    if (objViewModel.liUserDetails != null && objViewModel.liUserDetails.Count > 0)
                    {
                        //check if the user is student
                        var userDetails = (List<USER_DETAILS_FIELDS>)CMSHandler.FetchData<USER_DETAILS_FIELDS>(new STU_PERSONAL_INFO() { BOID_NUMBER = objViewModel.liUserDetails[0].BOID },
                            SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchStudentDetailsByBoid), sAcademicYear).DataSource.Data;

                        if (userDetails != null && userDetails.Count > 0)
                        {
                            objViewModel.liUserDetails[0].STUDENT_ID = userDetails.FirstOrDefault().USER_ID;
                        }
                        else
                        {
                            //check if the user is staff
                            var userDetails1 = (List<USER_DETAILS_FIELDS>)CMSHandler.FetchData<USER_DETAILS_FIELDS>(new STF_PERSONAL_INFO() { BOID_NUMBER = objViewModel.liUserDetails[0].BOID },
                                SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchStaffDetailsByBoid), sAcademicYear).DataSource.Data;

                            if (userDetails1 != null && userDetails1.Count > 0)
                            {
                                objViewModel.liUserDetails[0].STAFF_ID = userDetails1.FirstOrDefault().USER_ID;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Library", "FetchLiUserDetailsById", ex.Message);
                    }
                }
            }
            return objViewModel.liExternalUserInfo[0];
        }

        public JsonResult DeleteMemberById(string sUserId)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && sUserId != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    resultArgs = CMSHandler.SaveOrUpdate(new LI_USER_DETAILS() { USER_ID = sUserId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.DeleteExternalUser), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Library", "DeleteMemberById", ex.Message);
                    }
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData);
        }

        #endregion

        #region LogIn Details


        public ActionResult Login()
        {
            LI_LOGIN_FIELDS loginFields = new LI_LOGIN_FIELDS();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

            }
            return View(loginFields);
        }


        public JsonResult FetchUserCredentials(string Id)
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            JsonResultData ObjJsonData = new JsonResultData();
            bool sFlag = false;
            var UserDetails = new List<LI_LOGIN_FIELDS>();
            var UserLoginDetails = new List<LI_LOGIN_DETAILS>();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sScanId = Id.Replace("*", "");
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(Id))
            {
                try
                {
                    var liUserDetails = new List<USER_ACCOUNT>();
                    var ObjModel = new USER_ACCOUNT();
                    ObjModel.SCAN_ID = sScanId;
                    liUserDetails = (List<USER_ACCOUNT>)CMSHandler.FetchData<USER_ACCOUNT>(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserCredentialByRFIDLibrary)).DataSource.Data;
                    if (liUserDetails != null && liUserDetails.Count > 0)
                    {
                        ObjJsonData.Message = "Dear " + liUserDetails.FirstOrDefault().FIRST_NAME + " ";
                        ObjModel.USER_ID = liUserDetails.FirstOrDefault().USER_ID;
                        ObjModel.USER_TYPE = liUserDetails.FirstOrDefault().USER_TYPE;
                        ObjModel.ROLE = liUserDetails.FirstOrDefault().ROLE;
                        ObjModel.PROGRAMME_LEVEL = liUserDetails.FirstOrDefault().PROGRAMME_LEVEL;
                        ObjModel.SEMESTER_TYPE = liUserDetails.FirstOrDefault().SEMESTER_TYPE;
                        ObjModel.SEMESTER = liUserDetails.FirstOrDefault().SEMESTER;
                        ObjModel.AC_YEAR = liUserDetails.FirstOrDefault().AC_YEAR;
                        var liStudentActiveSem = (List<ACADEMIC_SEMESTER>)CMSHandler.FetchData<ACADEMIC_SEMESTER>(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchActiveSemesterByUserId), ObjModel.AC_YEAR).DataSource.Data;
                        if (liStudentActiveSem != null && liStudentActiveSem.Count > 0)
                        {
                            ObjModel.SEMESTER = liStudentActiveSem.FirstOrDefault().SEMESTER;
                        }
                        ObjModel.CHECKIN_ID = ValidateCheckIn(ObjModel.USER_ID, ObjModel.USER_TYPE, ObjModel.AC_YEAR);

                        if (!string.IsNullOrEmpty(ObjModel.CHECKIN_ID))
                        {
                            // For Check Out ..,
                            var SaveCheckOut = CMSHandler.SaveOrUpdate(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveCheckOutEntry), ObjModel.AC_YEAR);
                            if (SaveCheckOut.Success && SaveCheckOut.RowsAffected != 0)
                            {
                                ObjJsonData.Message += "Your Log Out Successfully ! :)";
                                sFlag = true;
                            }
                            else
                            {
                                ObjJsonData.Message += " Sorry Failed To Log Out ! Try Again ! :(";
                            }
                        }
                        else
                        {
                            // For Check In ..,
                            var SaveCheckIn = CMSHandler.SaveOrUpdate(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveCheckInEntry), ObjModel.AC_YEAR);
                            if (SaveCheckIn.Success && SaveCheckIn.RowsAffected != 0)
                            {
                                ObjJsonData.Message += "Your Loged In Successfully ! :)";
                                sFlag = true;
                            }
                            else
                            {
                                ObjJsonData.Message += " Sorry Failed To Log In ! Try Again ! :(";
                            }
                        }
                    }
                    else
                    {
                        liUserDetails = (List<USER_ACCOUNT>)CMSHandler.FetchData<USER_ACCOUNT>(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserCredentialByBARCODELibrary)).DataSource.Data;
                        if (liUserDetails != null && liUserDetails.Count > 0)
                        {
                            ObjJsonData.Message = "Dear " + liUserDetails.FirstOrDefault().FIRST_NAME + " ";
                            ObjModel.USER_ID = liUserDetails.FirstOrDefault().USER_ID;
                            ObjModel.USER_TYPE = liUserDetails.FirstOrDefault().USER_TYPE;
                            ObjModel.ROLE = liUserDetails.FirstOrDefault().ROLE;
                            ObjModel.PROGRAMME_LEVEL = liUserDetails.FirstOrDefault().PROGRAMME_LEVEL;
                            ObjModel.SEMESTER_TYPE = liUserDetails.FirstOrDefault().SEMESTER_TYPE;
                            ObjModel.SEMESTER = liUserDetails.FirstOrDefault().SEMESTER;
                            ObjModel.AC_YEAR = liUserDetails.FirstOrDefault().AC_YEAR;
                            var liStudentActiveSem = (List<ACADEMIC_SEMESTER>)CMSHandler.FetchData<ACADEMIC_SEMESTER>(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchActiveSemesterByUserId), ObjModel.AC_YEAR).DataSource.Data;
                            if (liStudentActiveSem != null && liStudentActiveSem.Count > 0)
                            {
                                ObjModel.SEMESTER = liStudentActiveSem.FirstOrDefault().SEMESTER;
                            }
                            ObjModel.CHECKIN_ID = ValidateCheckIn(ObjModel.USER_ID, ObjModel.USER_TYPE, ObjModel.AC_YEAR);

                            if (!string.IsNullOrEmpty(ObjModel.CHECKIN_ID))
                            {
                                // For Check Out ..,
                                var SaveCheckOut = CMSHandler.SaveOrUpdate(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveCheckOutEntry), ObjModel.AC_YEAR);
                                if (SaveCheckOut.Success && SaveCheckOut.RowsAffected != 0)
                                {
                                    ObjJsonData.Message += "Your Log Out Successfully ! :)";
                                    sFlag = true;
                                }
                                else
                                {
                                    ObjJsonData.Message += " Sorry Failed To Log Out ! Try Again ! :(";
                                }
                            }
                            else
                            {
                                // For Check In ..,
                                var SaveCheckIn = CMSHandler.SaveOrUpdate(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveCheckInEntry), ObjModel.AC_YEAR);
                                if (SaveCheckIn.Success && SaveCheckIn.RowsAffected != 0)
                                {
                                    ObjJsonData.Message += "Your Loged In Successfully ! :)";
                                    sFlag = true;
                                }
                                else
                                {
                                    ObjJsonData.Message += " Sorry Failed To Log In ! Try Again ! :(";
                                }
                            }
                        }
                        else
                        {
                            ObjJsonData.Message += "No Matches Found ! :(";
                        }
                    }

                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    if (sFlag)
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;

                        ObjJsonData.oResult = (List<LI_LOGIN_FIELDS>)CMSHandler.FetchData<LI_LOGIN_FIELDS>(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserLoginDetails), ObjModel.AC_YEAR).DataSource.Data;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchUserCredentials", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
                //return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }


        public string FetchTotalVisitors()
        {
            //Fetch Total Visitors
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string TotalVisitors = string.Empty;
            var visitor = (List<LI_LOGIN_FIELDS>)CMSHandler.FetchData<LI_LOGIN_FIELDS>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalVisitorsByDate), sAcademicYear).DataSource.Data;
            if (visitor != null && visitor.Count > 0)
                TotalVisitors = visitor[0].VISITORS;
            return TotalVisitors;
        }

        public ResultArgs AddLoginDetails(List<LI_LOGIN_FIELDS> UserDetails)
        {
            // add login details
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            LI_LOGIN_DETAILS logDetails = new LI_LOGIN_DETAILS
            {
                USER_ID = UserDetails[0].USER_ID,
                NO_OF_ATTEMPT = 1.ToString(),
                IS_ACTIVE = Common.IsActiveFalg.IsActive

            };
            resultArgs = CMSHandler.SaveOrUpdate(logDetails, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.AddLoginDetails), sAcademicYear);
            if (resultArgs.Success)
                resultArgs = CMSHandler.SaveOrUpdate(new LI_USER_DETAILS() { USER_ID = logDetails.USER_ID }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLastLoginDetailsByUserId), sAcademicYear);
            return resultArgs;
        }
        public ResultArgs UpdateLoginDetails(List<LI_LOGIN_FIELDS> UserDetails)
        {
            // add login details
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            LI_LOGIN_DETAILS logDetails = new LI_LOGIN_DETAILS
            {
                USER_ID = UserDetails[0].USER_ID


            };
            //FETCH LOGIN ID
            var result = (List<LI_LOGIN_DETAILS>)CMSHandler.FetchData<LI_LOGIN_DETAILS>(logDetails, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLoginIdByUserIdandDate), sAcademicYear).DataSource.Data;

            if (result != null && result.Count > 0)
            {
                resultArgs = CMSHandler.SaveOrUpdate(new LI_LOGIN_DETAILS() { LOGIN_ID = result[0].LOGIN_ID, IS_ACTIVE = Common.IsActiveFalg.IsActive }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLoginDetails), sAcademicYear);
                if (resultArgs.Success)
                    resultArgs = CMSHandler.SaveOrUpdate(logDetails, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLastLoginDetailsByUserId), sAcademicYear);
            }
            return resultArgs;
        }

        public ResultArgs Logout(List<LI_LOGIN_FIELDS> UserDetails)
        {
            STATUS = "0";
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            LI_LOGIN_DETAILS logDetails = new LI_LOGIN_DETAILS
            {
                USER_ID = UserDetails[0].USER_ID
            };
            //FETCH LOGIN ID
            var result = (List<LI_LOGIN_DETAILS>)CMSHandler.FetchData<LI_LOGIN_DETAILS>(logDetails, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLoginIdByUserIdandDate), sAcademicYear).DataSource.Data;

            if (result != null && result.Count > 0)
            {
                resultArgs = CMSHandler.SaveOrUpdate(new LI_LOGIN_DETAILS() { LOGIN_ID = result[0].LOGIN_ID, IS_ACTIVE = Common.IsActiveFalg.IsNotActive }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.LogoutUserLoginDetails), sAcademicYear);

                if (resultArgs.Success)
                    resultArgs = CMSHandler.SaveOrUpdate(new LI_LOGIN_DETAILS() { LOGIN_ID = result[0].LOGIN_ID }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateTotalSpentTimeByLoginId), sAcademicYear);
            }
            return resultArgs;
        }





        #endregion

        #region Maintain Rack Details
        public ActionResult ListRackInfo()
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                objViewModel.liRackInfo = (List<LI_RACK_INFO>)CMSHandler.FetchData<LI_RACK_INFO>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.ListLibRackInfo), sAcademicYear).DataSource.Data;

                var SupRack = (List<SUP_LI_RACKS>)CMSHandler.FetchData<SUP_LI_RACKS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiRacks), sAcademicYear).DataSource.Data;
                var SupCells = (List<SUP_LI_CELLS>)CMSHandler.FetchData<SUP_LI_CELLS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiCells), sAcademicYear).DataSource.Data;

                if (SupRack != null && SupRack.Count > 0)
                {
                    objViewModel.sRack = new SelectList(SupRack, Common.SUP_LI_RACKS.RACK_ID, Common.SUP_LI_RACKS.RACK_NAME);
                }
                if (SupCells != null && SupCells.Count > 0)
                {
                    objViewModel.sCell = new SelectList(SupCells, Common.SUP_LI_CELLS.CELL_ID, Common.SUP_LI_CELLS.CELL_NAME);
                }
            }
            return View(objViewModel);
        }

        public JsonResult FetchLiRackInfoById(string sRackInfoId)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && sRackInfoId != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = (List<LI_RACK_INFO>)CMSHandler.FetchData<LI_RACK_INFO>(new LI_RACK_INFO() { RACK_INFO_ID = sRackInfoId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiRackInfobyId), sAcademicYear).DataSource.Data;
                    if (result != null && result.Count > 0)
                        objResultData.oResult = result;
                    else
                        objResultData.Message = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Library", "FetchRackInfoById", ex.Message);
                    }
                }

            }
            else
            {
                objResultData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveLiRackInfo(string sJsonData)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = JsonConvert.DeserializeObject<LI_RACK_INFO>(sJsonData);


                    if (result.RACK_INFO_ID != "" && result.RACK_INFO_ID != null)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiRackInfo), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {

                        var Existing = (List<LI_RACK_INFO>)CMSHandler.FetchData<LI_RACK_INFO>(result, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchRackInfoByRackAndCell), sAcademicYear).DataSource.Data;
                        if (Existing != null && Existing.Count > 0)
                        {
                            //  var IsExist = Existing.Where(x => x.MEM_CATEGORY == result.MEM_CATEGORY).ToList();
                            objResultData.Message = "The rack  was already added!";
                            objResultData.ErrorCode = Common.ErrorCode.NoContent;
                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.AddLiRackInfo), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                        }


                    }
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Library", "SaveRackInfo", ex.Message);
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

        public JsonResult DeleteLiRackInfoById(string sRackInfoId)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && sRackInfoId != "")
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    //Fetch Member by membertype
                    //var IsExist = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(new LI_RULES { RULE_ID = sRuleId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchMemberByMemberType), sAcademicYear).DataSource.Data;
                    //if (IsExist != null && IsExist.Count > 0)
                    //{
                    //    objResultData.Message = "Can't delete the rule Becaues Members are added in this Type!";
                    //}


                    resultArgs = CMSHandler.SaveOrUpdate(new LI_RACK_INFO { RACK_INFO_ID = sRackInfoId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.DeleteLiRackInfoById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Library", "DeleteRackInfo", ex.Message);
                    }
                }

            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Author Details
        public ActionResult ListAuthorInfo()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModal = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModal.liAuthorInfo = (List<LI_AUTHOR_INFO>)CMSHandler.FetchData<LI_AUTHOR_INFO>(null, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.ListAuthorInfo), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "ListAuthorInfo", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModal);
        }

        public JsonResult FetchAuthorInfoById(string AuthorId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModal = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liFetchAuthorInfo = (List<LI_AUTHOR_INFO>)CMSHandler.FetchData<LI_AUTHOR_INFO>(new LI_AUTHOR_INFO() { AUTHOR_ID = AuthorId }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchAuthorInfoById), sAcademicYear).DataSource.Data;
                    if (liFetchAuthorInfo != null && liFetchAuthorInfo.Count > 0)
                    {
                        return Json(liFetchAuthorInfo, JsonRequestBehavior.AllowGet);
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
                        ObjLog.WriteError("LibraryController", "FetchAuthorInfoById", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveAuthorInfo(string sJsonAuthorInfo)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModal = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<LI_AUTHOR_INFO>(sJsonAuthorInfo);
                    Result.ENTRY_ID = sUserId;

                    if (!string.IsNullOrEmpty(Result.AUTHOR_ID))
                    {
                        // Update ,..
                        var UpdateAuthorInfo = CMSHandler.SaveOrUpdate(Result, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateAuthorInfo), sAcademicYear);
                        if (UpdateAuthorInfo.Success)
                        {
                            ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                    else
                    {
                        // Insert ,..
                        var SaveAuthorInfo = CMSHandler.SaveOrUpdate(Result, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveAuthorInfo), sAcademicYear);
                        if (SaveAuthorInfo.Success)
                        {
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "AddOrEditAuthor", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAuthorInfo(string sAuthorId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModal = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DeleteAuthorInfo = CMSHandler.SaveOrUpdate(new LI_AUTHOR_INFO() { AUTHOR_ID = sAuthorId }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.DeleteAuthorInfo), sAcademicYear);
                    if (DeleteAuthorInfo.Success)
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
                        ObjLog.WriteError("LibraryController", "DeleteAuthorInfo", ex.Message);
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

        #region Book Details
        public ActionResult ListBookDetails()
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(null, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.ListBookDetails), sAcademicyear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "ListBookDetails", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult AddBookDetails(string sBookId)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {

                    // Fetch last access no 
                    var liAccessNo = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(null, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLastAccessNo)).DataSource.Data;

                    if (liAccessNo != null && liAccessNo.Count > 0)
                    {
                        //string sLastBookId = (!string.IsNullOrEmpty(liAccessNo.FirstOrDefault().BOOK_ID)) ? Convert.ToString(Convert.ToInt32(liAccessNo.FirstOrDefault().BOOK_ID) + 1) : "1";
                        ObjViewModel.ObjBookDetails.ACCESS_NO = liAccessNo.FirstOrDefault().ACCESS_NO;
                        //ObjViewModel.sLastIndexBookId = sLastBookId;
                    }
                    else
                    {
                        //string sLastBookId = liAccessNo != null ? Convert.ToString(Convert.ToInt32(liAccessNo.FirstOrDefault().BOOK_ID) + 1) : "1";
                        ObjViewModel.ObjBookDetails.ACCESS_NO = "0";
                        //ObjViewModel.sLastIndexBookId = sLastBookId;
                    }



                    //  SubAc ,...
                    var liAccount = (List<SUP_LI_ACCOUNT>)CMSHandler.FetchData<SUP_LI_ACCOUNT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAccountDDL)).DataSource.Data;
                    if (liAccount != null && liAccount.Count > 0)
                    {
                        ObjViewModel.sSupAcDDL = new SelectList(liAccount, Common.SUP_LI_ACCOUNT.ACCOUNT_ID, Common.SUP_LI_ACCOUNT.ACCOUNT_NAME);
                    }

                    //status
                    var liStatus = (List<SUP_LI_STATUS>)CMSHandler.FetchData<SUP_LI_STATUS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiStatus)).DataSource.Data;
                    if (liStatus != null && liStatus.Count > 0)
                    {
                        ObjViewModel.sStatusDDL = new SelectList(liStatus, Common.SUP_LI_STATUS.STATUS_ID, Common.SUP_LI_STATUS.STATUS_NAME);

                    }

                    //illustration
                    var liIllustration = (List<SUP_LI_ILLUSTRATION>)CMSHandler.FetchData<SUP_LI_ILLUSTRATION>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiIllustration)).DataSource.Data;
                    if (liIllustration != null && liIllustration.Count > 0)
                    {
                        ObjViewModel.sIllustrationDDL = new SelectList(liIllustration, Common.SUP_LI_ILLUSTRATION.ILLUSTRATE_ID, Common.SUP_LI_ILLUSTRATION.ILLUSTRATE);
                    }

                    //Document Type
                    var liDocType = (List<SUP_LI_BOOK_TYPE>)CMSHandler.FetchData<SUP_LI_BOOK_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiBookType)).DataSource.Data;
                    if (liDocType != null && liDocType.Count > 0)
                    {
                        ObjViewModel.sDocumentTypeDDL = new SelectList(liDocType, Common.SUP_LI_BOOK_TYPE.TYPE_ID, Common.SUP_LI_BOOK_TYPE.BOOK_TYPE);
                    }

                    //var liDocType = (List<SUP_LI_DOCTYPE>)CMSHandler.FetchData<SUP_LI_DOCTYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiDoctype)).DataSource.Data;
                    //if (liDocType != null && liDocType.Count > 0)
                    //{
                    //    ObjViewModel.sDocumentTypeDDL = new SelectList(liDocType, Common.SUP_LI_DOCUMENT_TYPE.DOCUMENT_TYPE_ID, Common.SUP_LI_DOCUMENT_TYPE.DOCUMENT_TYPE);
                    //}



                    //AutoClass
                    //var liAutoclass = (List<SUP_LI_AUTOCLASS>)CMSHandler.FetchData<SUP_LI_AUTOCLASS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiAutoclass)).DataSource.Data;
                    //if (liAutoclass != null && liAutoclass.Count > 0)
                    //{
                    //    ObjViewModel.AutoClassDDL = new SelectList(liAutoclass, Common.SUP_LI_AUTOCLASS.AUTO_CLASS_ID, Common.SUP_LI_AUTOCLASS.AUTO_CLASS);
                    //}


                    //discipline
                    var liDiscipline = (List<SUP_LI_DISCIPLINE>)CMSHandler.FetchData<SUP_LI_DISCIPLINE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiDiscipline)).DataSource.Data;
                    if (liDiscipline != null && liDiscipline.Count > 0)
                    {
                        ObjViewModel.sDisciplineDDL = new SelectList(liDiscipline, Common.SUP_LI_DISCIPLINE.DISCIPLINE_ID, Common.SUP_LI_DISCIPLINE.DISCIPLINE_NAME);
                    }

                    //keywords
                    var liKeywords = (List<SUP_LI_KEYWORDS>)CMSHandler.FetchData<SUP_LI_KEYWORDS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLikeywords)).DataSource.Data;
                    if (liKeywords != null && liKeywords.Count > 0)
                    {
                        ObjViewModel.sKeywordsDDL = new SelectList(liKeywords, Common.SUP_LI_KEYWORDS.KEYWORD_ID, Common.SUP_LI_KEYWORDS.KEYWORD);
                    }

                    // Series ,..
                    var liSeries = (List<SUP_LI_SERIES>)CMSHandler.FetchData<SUP_LI_SERIES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSeriesDDL)).DataSource.Data;
                    if (liSeries != null && liSeries.Count > 0)
                    {
                        ObjViewModel.sSeriesDDL = new SelectList(liSeries, Common.SUP_LI_SERIES.SERIES_ID, Common.SUP_LI_SERIES.SERIES_NAME);
                    }


                    // language ,...
                    var liLanguage = (List<Sup_Language>)CMSHandler.FetchData<Sup_Language>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLanguage)).DataSource.Data;
                    if (liLanguage != null && liLanguage.Count > 0)
                    {
                        ObjViewModel.LanguageDDL = new SelectList(liLanguage, Common.SUP_LANGUAGE.LANGUAGE_ID, Common.SUP_LANGUAGE.LANGUAGE_NAME);
                    }

                    //Author info
                    //var Author = (List<LI_AUTHOR_INFO>)CMSHandler.FetchData<LI_AUTHOR_INFO>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLiAuthors)).DataSource.Data;
                    //if (Author != null && Author.Count > 0)
                    //{
                    //    ObjViewModel.sAuthor = new SelectList(Author, Common.LI_AUTHOR_INFO.AUTHOR_ID, Common.LI_AUTHOR_INFO.FIRST_NAME);
                    //}


                    //fetch class no
                    var liClassNo = (List<SUP_LI_CLASS_NO>)CMSHandler.FetchData<SUP_LI_CLASS_NO>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiClassNo)).DataSource.Data;
                    if (liClassNo != null && liClassNo.Count > 0)
                    {
                        ObjViewModel.sClassNo = new SelectList(liClassNo, Common.SUP_LI_CLASS_NO.CLASS_NO_ID, Common.SUP_LI_CLASS_NO.CLASS_NO);
                    }

                    //fetch auto class
                    //var liAutoClass = (List<SUP_LI_AUTOCLASS>)CMSHandler.FetchData<SUP_LI_AUTOCLASS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiAutoclass)).DataSource.Data;
                    //if (liAutoClass != null && liAutoClass.Count > 0)
                    //{
                    //    ObjViewModel.sAutoclass = new SelectList(liAutoClass, Common.SUP_LI_AUTOCLASS.AUTO_CLASS_ID, Common.SUP_LI_AUTOCLASS.AUTO_CLASS);
                    //}

                    //fetch lib code
                    //var liLibCode = (List<SUP_LI_LIB_CODE>)CMSHandler.FetchData<SUP_LI_LIB_CODE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiLibCode)).DataSource.Data;
                    //if (liLibCode != null && liLibCode.Count > 0)
                    //{
                    //    ObjViewModel.sLibCode = new SelectList(liLibCode, Common.SUP_LI_LIB_CODE.LIB_CODE_ID, Common.SUP_LI_LIB_CODE.LIB_DESCRIPTION);
                    //}


                    // Fetch Library Code 
                    var liLibCode = (List<SUP_LI_DOC_CATEGORY>)CMSHandler.FetchData<SUP_LI_DOC_CATEGORY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiDocCategory)).DataSource.Data;
                    if (liLibCode != null && liLibCode.Count > 0)
                    {
                        ObjViewModel.sLibCode = new SelectList(liLibCode, Common.SUP_LI_DOC_CATEGORY.DOC_CATEGORY_ID, Common.SUP_LI_DOC_CATEGORY.DOC_CATEGORY);
                    }

                    //fetch Rack Info
                    var liRackInfo = (List<LI_RACK_INFO>)CMSHandler.FetchData<LI_RACK_INFO>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchRackInfo)).DataSource.Data;
                    if (liRackInfo != null && liRackInfo.Count > 0)
                    {
                        ObjViewModel.sRack = new SelectList(liRackInfo, Common.LI_RACK_INFO.RACK_INFO_ID, Common.LI_RACK_INFO.RACK_INFO);
                    }

                    //Book Status
                    var liBookStatus = (List<SUP_LI_STATUS>)CMSHandler.FetchData<SUP_LI_STATUS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiBookStatus)).DataSource.Data;
                    if (liBookStatus != null && liBookStatus.Count > 0)
                    {
                        ObjViewModel.sBookStatus = new SelectList(liBookStatus, Common.SUP_LI_STATUS.STATUS_ID, Common.SUP_LI_STATUS.STATUS_NAME);
                    }

                    // Book Invoice
                    var liBookInvoice = (List<SUP_LI_BOOK_INVOICE>)CMSHandler.FetchData<SUP_LI_BOOK_INVOICE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiBookInvoice)).DataSource.Data;
                    if (liBookInvoice != null && liBookInvoice.Count > 0)
                    {
                        ObjViewModel.sBookInvoice = new SelectList(liBookInvoice, Common.SUP_LI_BOOK_INVOICE.INVOICE_ID, Common.SUP_LI_BOOK_INVOICE.VENDOR);
                    }

                    var liTaps = (List<SUP_LI_TAPS>)CMSHandler.FetchData<SUP_LI_TAPS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiTaps)).DataSource.Data;
                    if (liTaps != null && liTaps.Count > 0)
                    {
                        ObjViewModel.sTapsDDL = new SelectList(liTaps, Common.SUP_LI_TAPS.TAPS_ID, Common.SUP_LI_TAPS.TAPS_NAME);
                    }

                    // Details For Edit ,...
                    if (!string.IsNullOrEmpty(sBookId))
                    {
                        var liFetchBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { BOOK_ID = sBookId },
                                LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsById), sAcademicyear).DataSource.Data;
                        if (liFetchBookDetails != null && liFetchBookDetails.Count > 0)
                        {
                            ObjViewModel.ObjBookDetails.BOOK_ID = liFetchBookDetails.FirstOrDefault().BOOK_ID;
                            ObjViewModel.ObjBookDetails.RACK_ID = liFetchBookDetails.FirstOrDefault().RACK_ID;
                            ObjViewModel.ObjBookDetails.ACCESS_NO = liFetchBookDetails.FirstOrDefault().ACCESS_NO;
                            ObjViewModel.ObjBookDetails.CLASS_NO = liFetchBookDetails.FirstOrDefault().CLASS_NO;
                            ObjViewModel.ObjBookDetails.BOOK_NO = liFetchBookDetails.FirstOrDefault().BOOK_NO;
                            ObjViewModel.ObjBookDetails.LANGUAGE = liFetchBookDetails.FirstOrDefault().LANGUAGE;
                            ObjViewModel.ObjBookDetails.PERSONAL_AUTHOR1 = liFetchBookDetails.FirstOrDefault().PERSONAL_AUTHOR1;
                            ObjViewModel.ObjBookDetails.PERSONAL_AUTHOR2 = liFetchBookDetails.FirstOrDefault().PERSONAL_AUTHOR2;
                            ObjViewModel.ObjBookDetails.JOINED_AUTHOR1 = liFetchBookDetails.FirstOrDefault().JOINED_AUTHOR1;
                            ObjViewModel.ObjBookDetails.JOINED_AUTHOR2 = liFetchBookDetails.FirstOrDefault().JOINED_AUTHOR2;
                            ObjViewModel.ObjBookDetails.CORPORATE_AUTHOR1 = liFetchBookDetails.FirstOrDefault().CORPORATE_AUTHOR1;
                            ObjViewModel.ObjBookDetails.CORPORATE_AUTHOR2 = liFetchBookDetails.FirstOrDefault().CORPORATE_AUTHOR2;
                            ObjViewModel.ObjBookDetails.DOC_TITLE = liFetchBookDetails.FirstOrDefault().DOC_TITLE;
                            ObjViewModel.ObjBookDetails.PARALLEL_TITLE = liFetchBookDetails.FirstOrDefault().PARALLEL_TITLE;
                            ObjViewModel.ObjBookDetails.NOTES = liFetchBookDetails.FirstOrDefault().NOTES;
                            ObjViewModel.ObjBookDetails.PHYDESC = liFetchBookDetails.FirstOrDefault().PHYDESC;
                            ObjViewModel.ObjBookDetails.ILLUSTRATE = liFetchBookDetails.FirstOrDefault().ILLUSTRATE;
                            ObjViewModel.ObjBookDetails.STATUS = liFetchBookDetails.FirstOrDefault().STATUS;
                            ObjViewModel.ObjBookDetails.PRICE = liFetchBookDetails.FirstOrDefault().PRICE;
                            ObjViewModel.ObjBookDetails.DOP = liFetchBookDetails.FirstOrDefault().DOP;
                            ObjViewModel.ObjBookDetails.VC = liFetchBookDetails.FirstOrDefault().VC;
                            ObjViewModel.ObjBookDetails.ISBN_NO = liFetchBookDetails.FirstOrDefault().ISBN_NO;
                            ObjViewModel.ObjBookDetails.DISIPLINE = liFetchBookDetails.FirstOrDefault().DISIPLINE;
                            ObjViewModel.ObjBookDetails.NAME_OF_PUBLISHER = liFetchBookDetails.FirstOrDefault().NAME_OF_PUBLISHER;
                            ObjViewModel.ObjBookDetails.PLACE_OF_PUBLISHER = liFetchBookDetails.FirstOrDefault().PLACE_OF_PUBLISHER;
                            ObjViewModel.ObjBookDetails.SERIES = liFetchBookDetails.FirstOrDefault().SERIES;
                            ObjViewModel.ObjBookDetails.DOC_CATEGORY = liFetchBookDetails.FirstOrDefault().DOC_CATEGORY;
                            ObjViewModel.ObjBookDetails.BOOK_PUBLISH_YEAR = liFetchBookDetails.FirstOrDefault().BOOK_PUBLISH_YEAR;
                            ObjViewModel.ObjBookDetails.SUB_ACCOUNT = liFetchBookDetails.FirstOrDefault().SUB_ACCOUNT;
                            ObjViewModel.ObjBookDetails.KEYWORD = liFetchBookDetails.FirstOrDefault().KEYWORD;
                            ObjViewModel.ObjBookDetails.INVOICE = liFetchBookDetails.FirstOrDefault().INVOICE;
                            ObjViewModel.ObjBookDetails.TAPS = liFetchBookDetails.FirstOrDefault().TAPS;
                            ObjViewModel.ObjBookDetails.RFID = liFetchBookDetails.FirstOrDefault().RFID;
                            ObjViewModel.ObjBookDetails.BARCODE = liFetchBookDetails.FirstOrDefault().BARCODE;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "AddBookDetails", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult SaveBookDetails(string sJsonBookDetails)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LibraryViewModel();
            int aSuccessRecords = 0;
            int aFailedRecords = 0;
            int uSuccessRecords = 0;
            int uFailedRecords = 0;
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Save_Json_List>(sJsonBookDetails);


                    //validate book info before add
                    foreach (var item in Result.liBookDetails)
                    {
                        if (string.IsNullOrEmpty(item.BOOK_ID))
                        {
                            ObjJsonData.Message = ValidateBookInfo(item);
                            if (!string.IsNullOrEmpty(ObjJsonData.Message))
                            {
                                ObjJsonData.ErrorCode = Common.ErrorCode.AlreadyReported;
                                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }




                    foreach (var item in Result.liBookDetails)
                    {
                        // formating date
                        if (!string.IsNullOrEmpty(item.DOP))
                        {
                            item.DOP = CommonMethods.ConvertdatetoyearFromat(item.DOP);
                        }

                        if (!string.IsNullOrEmpty(item.BOOK_ID))
                        {
                            var liUpdateBookDetails = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookDetails), sAcademicyear);
                            if (liUpdateBookDetails.Success)
                            {
                                uSuccessRecords++;
                                ObjJsonData.Message = uSuccessRecords + Common.Messages.RecordsUpdatedSuccessfully;
                                ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                ObjJsonData.sResult = item.BOOK_ID;
                            }
                            else
                            {
                                uFailedRecords++;
                                ObjJsonData.Message = uFailedRecords + Common.Messages.FailedToSaveRecords;
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            }
                        }
                        else
                        {


                            var liSaveBookDetails = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveBookDetails), sAcademicyear, true);
                            if (liSaveBookDetails.Success)
                            {
                                // update access no into auto generate number table
                                var autoGenerateResult = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateAutoGenerateNumber), sAcademicyear, true);


                                aSuccessRecords++;
                                //var sUpdateAccessNo = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateAccessNoInAutoGenerate));
                                ObjJsonData.Message = aSuccessRecords + Common.Messages.RecordsSavedSuccessfully;
                                ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                ObjJsonData.sResult = liSaveBookDetails.RowUniqueId.ToString();
                            }
                            else
                            {
                                aFailedRecords++;
                                ObjJsonData.Message = aFailedRecords + Common.Messages.FailedToSaveRecords;
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            }
                        }
                    }



                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "ListBookDetails", ex.Message);
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

        public JsonResult FetchPublisherDetailsByIsbn(string sIsbn)
        {
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sIsbn))
            {

                string sQl = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookPublisherDetailsByIsbn);
                var Details = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS()
                {
                    ISBN_NO = sIsbn
                }, sQl.Replace(Common.Delimiter.QUS + Common.LI_BOOK_DETAILS.ISBN_NO, sIsbn)).DataSource.Data;

                if (Details != null && Details.Count > 0)
                {
                    objResultData.oResult = Details;
                }
                else
                {
                    objResultData.Message = Common.Messages.NoRecordsfound;
                }
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteBookDetails(string sBookId)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    var DeleteBookDetails = CMSHandler.SaveOrUpdate(new LI_BOOK_DETAILS() { BOOK_ID = sBookId }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.DeleteBookDetails), sAcademicyear);
                    if (DeleteBookDetails.Success)
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
                        ObjLog.WriteError("LibraryController", "DeleteBookDetails", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddorUpdateLibCode(string sJsonData)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();

            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<SUP_LI_DOC_CATEGORY>(sJsonData);

                    if (result != null)
                    {
                        if (result.DOC_CATEGORY_ID != null && result.DOC_CATEGORY_ID != "")
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateSupLiLibCode));
                            ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.AddSupLiLibCode));
                            ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "DeleteBookDetails", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);

        }

        public JsonResult AddorUpdateClassNo(string sJsonData)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();

            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<SUP_LI_CLASS_NO>(sJsonData);

                    if (result != null)
                    {
                        if (result.CLASS_NO_ID != null && result.CLASS_NO_ID != "")
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateSupLiClassNo));
                            ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords; ;
                            ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.AddSupLiClassNo));
                            ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords; ;
                            ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "AddorUpdateClassNo", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);

        }

        public JsonResult AddorUpdateSupLiStatus(string sJsonData)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();

            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<SUP_LI_STATUS>(sJsonData);

                    if (result != null)
                    {
                        if (result.STATUS_ID != null && result.STATUS_ID != "")
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateSupLiStatus));
                            ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords; ;
                            ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.AddSupLiStatus));
                            ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords; ;
                            ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "AddorUpdateSupLiStatus", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);

        }

        public JsonResult AddorUpdateSupLiAccount(string sJsonData)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();

            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<SUP_LI_ACCOUNT>(sJsonData);

                    if (result != null)
                    {
                        if (result.ACCOUNT_ID != null && result.ACCOUNT_ID != "")
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateSupLiAccount));
                            ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords; ;
                            ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.AddSupLiAccount));
                            ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords; ;
                            ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "AddorUpdateSupLiAccount", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddorUpdateSupLiIllustrate(string sJsonData)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();

            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<SUP_LI_ILLUSTRATION>(sJsonData);

                    if (result != null)
                    {
                        if (result.ILLUSTRATE_ID != null && result.ILLUSTRATE_ID != "")
                        {

                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.AddSupLiIllustrate));
                            ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "btnIllustrateModalSubmit", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddorUpdateSupLiDoc(string sJsonData)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();

            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<SUP_LI_BOOK_TYPE>(sJsonData);

                    if (result != null)
                    {
                        if (result.TYPE_ID != null && result.TYPE_ID != "")
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateBookType));
                            ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.AddBookType));
                            ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "AddorUpdateSupLiDoc", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddorUpdateSupLiDescipline(string sJsonData)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();

            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<SUP_LI_DISCIPLINE>(sJsonData);

                    if (result != null)
                    {
                        if (result.DISCIPLINE_ID != null && result.DISCIPLINE_ID != "")
                        {

                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.AddSupLiDescipline));
                            ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords; ;
                            ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "AddorUpdateSupLiDescipline", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddorUpdateSupLiKeywords(string sJsonData)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();

            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<SUP_LI_KEYWORDS>(sJsonData);

                    if (result != null)
                    {
                        if (result.KEYWORD_ID != null && result.KEYWORD_ID != "")
                        {

                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.AddSupLiKeywords));
                            ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords; ;
                            ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "AddorUpdateSupLiKeywords", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddorUpdateSupLiSeries(string sJsonData)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();

            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<SUP_LI_SERIES>(sJsonData);

                    if (result != null)
                    {
                        if (result.SERIES_ID != null && result.SERIES_ID != "")
                        {

                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.AddSupLiSeries));
                            ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords; ;
                            ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "AddorUpdateSupLiSeries", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public string FetchDetailsafterUpdate(string sOption)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sOptions = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sOption))
                    {
                        switch (sOption)
                        {
                            case "1":
                                {
                                    var sDetails = (List<SUP_LI_STATUS>)CMSHandler.FetchData<SUP_LI_STATUS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiStatus), sAcademicyear).DataSource.Data;
                                    if (sDetails != null && sDetails.Count > 0)
                                    {
                                        foreach (var item in sDetails)
                                        {
                                            sOptions += "<option value=" + item.STATUS_ID + ">" + item.STATUS_NAME + "</option>";
                                        }
                                    }
                                    break;
                                }
                            case "2":
                                {
                                    var sDetails = (List<SUP_LI_DOC_CATEGORY>)CMSHandler.FetchData<SUP_LI_DOC_CATEGORY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiDocCategory)).DataSource.Data;
                                    if (sDetails != null && sDetails.Count > 0)
                                    {
                                        foreach (var item in sDetails)
                                        {
                                            sOptions += "<option value=" + item.DOC_CATEGORY_ID + ">" + item.DOC_CATEGORY + "</option>";
                                        }
                                    }
                                    break;
                                }
                            case "3":
                                {
                                    var sDetails = (List<SUP_LI_CLASS_NO>)CMSHandler.FetchData<SUP_LI_CLASS_NO>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiClassNo), sAcademicyear).DataSource.Data;
                                    if (sDetails != null && sDetails.Count > 0)
                                    {
                                        foreach (var item in sDetails)
                                        {
                                            sOptions += "<option value=" + item.CLASS_NO_ID + ">" + item.CLASS_NO + "</option>";
                                        }
                                    }
                                    break;
                                }
                            case "4":
                                {
                                    var sDetails = (List<LI_AUTHOR_INFO>)CMSHandler.FetchData<LI_AUTHOR_INFO>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLiAuthors), sAcademicyear).DataSource.Data;
                                    if (sDetails != null && sDetails.Count > 0)
                                    {
                                        foreach (var item in sDetails)
                                        {
                                            sOptions += "<option value=" + item.AUTHOR_ID + ">" + item.FIRST_NAME + "</option>";
                                        }
                                    }
                                    break;
                                }
                            case "5":
                                {
                                    var sDetails = (List<SUP_LI_ACCOUNT>)CMSHandler.FetchData<SUP_LI_ACCOUNT>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiAccount), sAcademicyear).DataSource.Data;
                                    if (sDetails != null && sDetails.Count > 0)
                                    {
                                        foreach (var item in sDetails)
                                        {
                                            sOptions += "<option value=" + item.ACCOUNT_ID + ">" + item.ACCOUNT_NAME + "</option>";
                                        }
                                    }
                                    break;
                                }
                            case "6":
                                {
                                    var sDetails = (List<SUP_LI_ILLUSTRATION>)CMSHandler.FetchData<SUP_LI_ILLUSTRATION>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiIllustration), sAcademicyear).DataSource.Data;
                                    if (sDetails != null && sDetails.Count > 0)
                                    {
                                        foreach (var item in sDetails)
                                        {
                                            sOptions += "<option value=" + item.ILLUSTRATE_ID + ">" + item.ILLUSTRATE + "</option>";
                                        }
                                    }
                                    break;
                                }
                            case "7":
                                {
                                    var sDetails = (List<SUP_LI_BOOK_TYPE>)CMSHandler.FetchData<SUP_LI_BOOK_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiBookType)).DataSource.Data;
                                    if (sDetails != null && sDetails.Count > 0)
                                    {
                                        foreach (var item in sDetails)
                                        {
                                            sOptions += "<option value=" + item.TYPE_ID + ">" + item.BOOK_TYPE + "</option>";
                                        }
                                    }
                                    break;
                                }
                            case "8":
                                {
                                    var sDetails = (List<SUP_LI_DISCIPLINE>)CMSHandler.FetchData<SUP_LI_DISCIPLINE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiDiscipline), sAcademicyear).DataSource.Data;
                                    if (sDetails != null && sDetails.Count > 0)
                                    {
                                        foreach (var item in sDetails)
                                        {
                                            sOptions += "<option value=" + item.DISCIPLINE_ID + ">" + item.DISCIPLINE_NAME + "</option>";
                                        }
                                    }
                                    break;
                                }
                            case "9":
                                {
                                    var sDetails = (List<SUP_LI_KEYWORDS>)CMSHandler.FetchData<SUP_LI_KEYWORDS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLikeywords), sAcademicyear).DataSource.Data;
                                    if (sDetails != null && sDetails.Count > 0)
                                    {
                                        foreach (var item in sDetails)
                                        {
                                            sOptions += "<option value=" + item.KEYWORD_ID + ">" + item.KEYWORD + "</option>";
                                        }
                                    }
                                    break;
                                }
                            case "10":
                                {
                                    var sDetails = (List<SUP_LI_SERIES>)CMSHandler.FetchData<SUP_LI_SERIES>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSeriesDDL), sAcademicyear).DataSource.Data;
                                    if (sDetails != null && sDetails.Count > 0)
                                    {
                                        foreach (var item in sDetails)
                                        {
                                            sOptions += "<option value=" + item.SERIES_ID + ">" + item.SERIES_NAME + "</option>";
                                        }
                                    }
                                    break;
                                }

                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Library", "FetchDetailsafterUpdate", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Library", "FetchDetailsafterUpdate", ex.Message);
                    }
                }
            }

            return sOptions;
        }




        public string ValidateBookInfo(LI_BOOK_DETAILS objBookDetails)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            string sMessge = string.Empty;
            //first validate rfid
            var Exist = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { RFID = objBookDetails.RFID }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByRfid), sAcademicyear).DataSource.Data;
            if (Exist != null && Exist.Count > 0)
            {
                sMessge = "Please change RFID";
                return sMessge;
            }

            //first validate barcode
            var Exist2 = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { BARCODE = objBookDetails.BARCODE }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByBarCode), sAcademicyear).DataSource.Data;
            if (Exist2 != null && Exist2.Count > 0)
            {
                sMessge = "Please change BarCode";
                return sMessge;
            }

            //first validate book no
            var Exist1 = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { BOOK_NO = objBookDetails.BOOK_NO }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByBookNo), sAcademicyear).DataSource.Data;
            if (Exist != null && Exist.Count > 0)
            {
                sMessge = "Please change Book No";
                return sMessge;
            }
            return sMessge;
        }

        //rfid checking 
        public JsonResult ValidateRfid(string sRfid)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            string sMessge = string.Empty;
            //first validate rfid
            var Exist = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { RFID = sRfid }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByRfid), sAcademicyear).DataSource.Data;
            if (Exist != null && Exist.Count > 0)
            {
                sMessge = "The Entered RFID is already exists!";
            }


            return Json(sMessge, JsonRequestBehavior.AllowGet);
        }

        //rfid checking 
        public JsonResult ValidateBarCode(string sBarCode)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            string sMessge = string.Empty;
            //first validate rfid
            var Exist = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { BARCODE = sBarCode }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByBarCode), sAcademicyear).DataSource.Data;
            if (Exist != null && Exist.Count > 0)
            {
                sMessge = "The Entered BarCode is already exists!";
            }


            return Json(sMessge, JsonRequestBehavior.AllowGet);
        }

        //fetch book details by rfid

        public JsonResult FetchBookDetailsByUniqueValue(string sFlag, string sFiledValue)
        {
            JsonResultData objResultData = new JsonResultData();
            var liBookInfo = new List<LI_BOOK_INFO>();
            try
            {
                switch (sFlag)
                {
                    case "1":
                        {
                            liBookInfo = (List<LI_BOOK_INFO>)CMSHandler.FetchData<LI_BOOK_INFO>(new LI_BOOK_DETAILS() { RFID = sFiledValue }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByRfid)).DataSource.Data;
                            break;
                        }
                    case "2":
                        {
                            liBookInfo = (List<LI_BOOK_INFO>)CMSHandler.FetchData<LI_BOOK_INFO>(new LI_BOOK_DETAILS() { ACCESS_NO = sFiledValue }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByAccessNo)).DataSource.Data;
                            break;
                        }
                    case "3":
                        {
                            liBookInfo = (List<LI_BOOK_INFO>)CMSHandler.FetchData<LI_BOOK_INFO>(new LI_BOOK_DETAILS() { BOOK_NO = sFiledValue }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByBookNo)).DataSource.Data;
                            break;
                        }
                    case "4":
                        {
                            liBookInfo = (List<LI_BOOK_INFO>)CMSHandler.FetchData<LI_BOOK_INFO>(new LI_BOOK_DETAILS() { BARCODE = sFiledValue }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByBarCode)).DataSource.Data;
                            break;
                        }
                    default:
                        break;
                }
                objResultData.oResult = liBookInfo;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("LibraryController", "FetchBookDetailsByUniqueValue", ex.Message);
                }
            }

            if (liBookInfo == null)
            {
                objResultData.Message = Common.Messages.NoRecordsfound;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        //fetch book details by Access No

        public JsonResult FetchBookDetailsByAccessNo(string sAccessNo)
        {
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAccessNo))
            {
                var Details = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS()
                {
                    ACCESS_NO = sAccessNo
                }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByAccessNo)).DataSource.Data;

                if (Details != null && Details.Count > 0)
                {
                    objResultData.oResult = Details;
                }
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Book Relocate
        public ActionResult BookRelocate()
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            LibraryViewModel objViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {



            }
            return View(objViewModel);
        }

        public ActionResult BindBookInforForRelocate(string sScanId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sScanId))
            {
                try
                {
                    //get book details by rfid
                    var liBookInfo = GetBookDetailsByRfid(sScanId);

                    if (liBookInfo != null && liBookInfo.Count > 0)
                    {
                        ObjViewModel.ObjBookDetails.BOOK_ID = liBookInfo.FirstOrDefault().BOOK_ID;
                        //status
                        var liStatus = (List<SUP_LI_STATUS>)CMSHandler.FetchData<SUP_LI_STATUS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiStatus)).DataSource.Data;
                        ObjViewModel.liStatus = liStatus;
                        ObjViewModel.ObjBookDetails.STATUS = liBookInfo.FirstOrDefault().STATUS;

                        //discipline
                        var liDiscipline = (List<SUP_LI_DISCIPLINE>)CMSHandler.FetchData<SUP_LI_DISCIPLINE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiDiscipline)).DataSource.Data;
                        ObjViewModel.liDiscipline = liDiscipline;
                        ObjViewModel.ObjBookDetails.DISIPLINE = liBookInfo.FirstOrDefault().DISIPLINE;

                    }
                    else
                    {
                        //get book details by accessno
                        var BookInfo = GetBookDetailsByAccessNo(sScanId);
                        if (BookInfo != null && BookInfo.Count > 0)
                        {
                            ObjViewModel.ObjBookDetails.BOOK_ID = liBookInfo.FirstOrDefault().BOOK_ID;
                            //status
                            var liStatus = (List<SUP_LI_STATUS>)CMSHandler.FetchData<SUP_LI_STATUS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiStatus)).DataSource.Data;
                            ObjViewModel.liStatus = liStatus;
                            ObjViewModel.ObjBookDetails.STATUS = liBookInfo.FirstOrDefault().STATUS;
                            //discipline

                            var liDiscipline = (List<SUP_LI_DISCIPLINE>)CMSHandler.FetchData<SUP_LI_DISCIPLINE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiDiscipline)).DataSource.Data;
                            ObjViewModel.liDiscipline = liDiscipline;
                            ObjViewModel.ObjBookDetails.DISIPLINE = liBookInfo.FirstOrDefault().DISIPLINE;
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.NoRecordsfound;
                            ObjJsonData.ErrorCode = Common.ErrorCode.AlreadyReported;
                            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                        }
                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "BindBookIssue", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult SaveBookRelocate(string sJsonBookDetails)
        {
            string sAcadenmicYear = FetchAcademicYear();
            // string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcadenmicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<LI_BOOK_DETAILS>(sJsonBookDetails);
                    if (Result != null)
                    {
                        var SaveBookRelocate = CMSHandler.SaveOrUpdate(Result, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveBookRelocate), sAcadenmicYear);
                        ObjJsonData.ErrorCode = SaveBookRelocate.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        ObjJsonData.Message = SaveBookRelocate.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveBookRelocate", ex.Message);
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

        #region Rules
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns> 
        public ActionResult Rules()
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                objViewModel.liRules = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.ListLibRules), sAcademicYear).DataSource.Data;

                var UserType = (List<SUP_USER_TYPE>)CMSHandler.FetchData<SUP_USER_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupUserType), sAcademicYear).DataSource.Data;

                // Application Type ,...
                var liApplicationType = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType)).DataSource.Data;


                //var ProgrammeLevel = (List<SUP_PROGRAMME_LEVEL>)CMSHandler.FetchData<SUP_PROGRAMME_LEVEL>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel), sAcademicYear).DataSource.Data;

                var Suprole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRole), sAcademicYear).DataSource.Data;

                if (UserType != null && UserType.Count > 0)
                {
                    var SelectedUserType = UserType.Where(x => x.USER_TYPE_ID == Common.USER_TYPE.TEACHING_STAFF || x.USER_TYPE_ID == Common.USER_TYPE.STUDENT || x.USER_TYPE_ID == Common.USER_TYPE.EXTERNAL_USER_FOR_LIBRARY || x.USER_TYPE_ID == Common.USER_TYPE.NON_TEACHING_STAFF).ToList();

                    objViewModel.UserType = new SelectList(SelectedUserType, Common.SUP_USER_TYPE.USER_TYPE_ID, Common.SUP_USER_TYPE.USER_TYPE_NAME);
                }
                if (liApplicationType != null && liApplicationType.Count > 0)
                {
                    objViewModel.ApplicationType = new SelectList(liApplicationType, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                }
                if (Suprole != null && Suprole.Count > 0)
                {
                    objViewModel.SupRole = new SelectList(Suprole, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);
                }




            }
            return View(objViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public ActionResult AddRules()
        //{
        //    LibraryViewModel objViewModel = new LibraryViewModel();
        //    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
        //    {
        //        string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

        //        //Fetching Memeber Category

        //        var MemberCategory = (List<SUP_LI_MEMBER_CATEGORY>)CMSHandler.FetchData<SUP_LI_MEMBER_CATEGORY>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiMemberCategory), sAcademicYear).DataSource.Data;

        //        if (MemberCategory != null && MemberCategory.Count > 0)
        //        {
        //            objViewModel.MemberCategory = new SelectList(MemberCategory, Common.SUP_LI_MEMBER_CATEGORY.CATEGORY_ID, Common.SUP_LI_MEMBER_CATEGORY.CATEGORY_NAME);
        //        }

        //        //Fetch Application Type
        //        var FechProgrammeType = (List<SUP_APPLICTION_TYPE>)CMSHandler.FetchData<SUP_APPLICTION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType), sAcademicYear).DataSource.Data;
        //        objViewModel.ApplicationType = new SelectList(FechProgrammeType, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);


        //    }
        //    return View(objViewModel);

        //}
        /// <summary>
        /// If data exits validate or add it
        /// </summary>
        /// <param name="sJsonData"></param>
        /// <returns></returns>
        public JsonResult SaveRules(string sJsonData)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = JsonConvert.DeserializeObject<LI_RULES>(sJsonData);

                    if (result.RULE_ID != "" && result.RULE_ID != null)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiRules), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {
                        //fetch existing  member values
                        var Existing = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(result, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchRulesByUserType), sAcademicYear).DataSource.Data;
                        if (Existing != null && Existing.Count > 0)
                        {
                            //  var IsExist = Existing.Where(x => x.MEM_CATEGORY == result.MEM_CATEGORY).ToList();
                            objResultData.Message = "The rule was already added for this member!";
                            objResultData.ErrorCode = Common.ErrorCode.AlreadyReported;
                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.AddLiRules), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                    }
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Library", "SaveRules", ex.Message);
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
        /// <summary>
        /// Fetch rule by rule id
        /// </summary>
        /// <param name="sRuleId"></param>
        /// <returns></returns>

        public JsonResult FetchLiRuleById(string sRuleId)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && sRuleId != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(new LI_RULES() { RULE_ID = sRuleId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiRulebyId), sAcademicYear).DataSource.Data;
                    if (result != null && result.Count > 0)
                        objResultData.oResult = result;
                    else
                        objResultData.Message = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Library", "FetchLiRuleById", ex.Message);
                    }
                }

            }
            else
            {
                objResultData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Delete Rule if member added for this category wont be deleted
        /// </summary>
        /// <param name="sRuleId"></param>
        /// <returns></returns>
        public JsonResult DeleteLiRuleById(string sRuleId)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && sRuleId != "")
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    //Fetch Member by membertype
                    //var IsExist = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(new LI_RULES { RULE_ID = sRuleId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchMemberByMemberType), sAcademicYear).DataSource.Data;
                    //if (IsExist != null && IsExist.Count > 0)
                    //{
                    //    objResultData.Message = "Can't delete the rule Becaues Members are added in this Type!";
                    //}


                    resultArgs = CMSHandler.SaveOrUpdate(new LI_RULES { RULE_ID = sRuleId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.DeleteLiRuleById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Library", "DeleteLiRuleById", ex.Message);
                    }
                }

            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Periods
        public ActionResult Periods()
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                objViewModel.liPeriods = (List<LI_PERIODS>)CMSHandler.FetchData<LI_PERIODS>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.ListLibPeriods), sAcademicYear).DataSource.Data;

                var UserType = (List<SUP_USER_TYPE>)CMSHandler.FetchData<SUP_USER_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupUserType), sAcademicYear).DataSource.Data;
                //var ProgrammeLevel = (List<SUP_PROGRAMME_LEVEL>)CMSHandler.FetchData<SUP_PROGRAMME_LEVEL>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel), sAcademicYear).DataSource.Data;
                // Application Type ,...
                var liApplicationType = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType)).DataSource.Data;
                var Suprole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRole), sAcademicYear).DataSource.Data;
                var SupSemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType), sAcademicYear).DataSource.Data;

                if (UserType != null && UserType.Count > 0)
                {
                    var SelectedUserType = UserType.Where(x => x.USER_TYPE_ID == Common.USER_TYPE.TEACHING_STAFF || x.USER_TYPE_ID == Common.USER_TYPE.STUDENT || x.USER_TYPE_ID == Common.USER_TYPE.EXTERNAL_USER_FOR_LIBRARY || x.USER_TYPE_ID == Common.USER_TYPE.NON_TEACHING_STAFF).ToList();

                    objViewModel.UserType = new SelectList(SelectedUserType, Common.SUP_USER_TYPE.USER_TYPE_ID, Common.SUP_USER_TYPE.USER_TYPE_NAME);
                }
                if (liApplicationType != null && liApplicationType.Count > 0)
                {
                    objViewModel.ApplicationType = new SelectList(liApplicationType, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                }
                if (Suprole != null && Suprole.Count > 0)
                {
                    objViewModel.SupRole = new SelectList(Suprole, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);
                }
                if (SupSemesterType != null && SupSemesterType.Count > 0)
                {
                    objViewModel.SupSemesterType = new SelectList(SupSemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                }

            }
            return View(objViewModel);
        }
        public JsonResult SavePeriods(string sJsonData)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = JsonConvert.DeserializeObject<LI_PERIODS>(sJsonData);
                    if (!string.IsNullOrEmpty(result.SEM_BEGIN_DATE))
                    {
                        result.SEM_BEGIN_DATE = CommonMethods.ConvertdatetoyearFromat(result.SEM_BEGIN_DATE);
                    }
                    if (!string.IsNullOrEmpty(result.SEM_END_DATE))
                    {
                        result.SEM_END_DATE = CommonMethods.ConvertdatetoyearFromat(result.SEM_END_DATE);
                    }


                    if (result.PERIOD_ID != "" && result.PERIOD_ID != null)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiPeriods), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {

                        //fetch existing  member values
                        var Existing = (List<LI_PERIODS>)CMSHandler.FetchData<LI_PERIODS>(result, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchPeriodByUserType), sAcademicYear).DataSource.Data;
                        if (Existing != null && Existing.Count > 0)
                        {
                            //  var IsExist = Existing.Where(x => x.MEM_CATEGORY == result.MEM_CATEGORY).ToList();
                            objResultData.Message = "The period was already added for this member!";
                            objResultData.ErrorCode = Common.ErrorCode.NoContent;
                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.AddLiPeriods), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                    }



                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Library", "SavePeriods", ex.Message);
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
        public JsonResult FetchLiPeriodById(string sPeriodId)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && sPeriodId != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = (List<LI_PERIODS>)CMSHandler.FetchData<LI_PERIODS>(new LI_PERIODS() { PERIOD_ID = sPeriodId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiPeriodbyId), sAcademicYear).DataSource.Data;
                    if (result != null && result.Count > 0)
                        objResultData.oResult = result;
                    else
                        objResultData.Message = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Library", "FetchLiPeriodById", ex.Message);
                    }
                }

            }
            else
            {
                objResultData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteLiPeriodById(string sPeriodId)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && sPeriodId != "")
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    //Fetch Member by membertype
                    //var IsExist = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(new LI_RULES { RULE_ID = sRuleId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchMemberByMemberType), sAcademicYear).DataSource.Data;
                    //if (IsExist != null && IsExist.Count > 0)
                    //{
                    //    objResultData.Message = "Can't delete the rule Becaues Members are added in this Type!";
                    //}


                    resultArgs = CMSHandler.SaveOrUpdate(new LI_PERIODS { PERIOD_ID = sPeriodId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.DeleteLiPeriodById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Library", "DeleteLiPeriodById", ex.Message);
                    }
                }

            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Book Circulation For Admin
        public ActionResult ListBookCirculation()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sMaxDay = string.Empty;
                    string sCurDate = string.Empty;
                    // List Book Details ,...
                    ObjViewModel.liBookIssueList = (List<ListBookIssue>)CMSHandler.FetchData<ListBookIssue>(null,
                           LibrarySQL.GetLibrarySQL(LibrarySQLCommands.ListBookIssue), sAcademicYear).DataSource.Data;




                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "ListBookIssue", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BookCirculation()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "BookCirculation", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult FetchStudentDetails(string sScanId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Fetch User Credentials
                    var circulationFields = FetchCredentials(sScanId.Replace("*", ""));
                    ObjJsonData.oResult = circulationFields;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchStudentDetails", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public CIRCULATION_FIELDS FetchCredentials(string Id)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var CirculationFields = new CIRCULATION_FIELDS();
            var ObjResultData = new JsonResultData();
            //      var UserCheckinInfo = new LI_CHECKIN_INFO();
            CirculationArgs circulationArgs = new CirculationArgs();


            if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(sAcademicYear))
            {
                // check user details

                circulationArgs = FetchUserDetails(Id);

                if (circulationArgs.liUserAccount != null && circulationArgs.liUserAccount.Count > 0)
                {
                    //check check in details

                    var chekStatus = ValidateCheckIn(circulationArgs.liUserAccount.FirstOrDefault().USER_ID, circulationArgs.liUserAccount.FirstOrDefault().USER_TYPE, sAcademicYear);
                    if (!string.IsNullOrEmpty(chekStatus))
                    {

                        if (!string.IsNullOrEmpty(circulationArgs.liUserAccount.FirstOrDefault().USER_ID) && !string.IsNullOrEmpty(circulationArgs.liUserAccount.FirstOrDefault().USER_TYPE))
                        {
                            if (circulationArgs.liUserAccount.FirstOrDefault().USER_TYPE == Common.USER_TYPE.EXTERNAL_USER_FOR_LIBRARY)
                            {
                                //fetch total circulation
                                CirculationFields.liBookCirculation = (List<LI_BOOK_CIRCULATION>)CMSHandler.FetchData<LI_BOOK_CIRCULATION>(new LI_USER_ACCOUNT() { USER_ID = circulationArgs.liUserAccount.FirstOrDefault().USER_ID, USER_TYPE = circulationArgs.liUserAccount.FirstOrDefault().USER_TYPE }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalBookIssuedForExternal), sAcademicYear).DataSource.Data;

                                //fetch reservation 
                                CirculationFields.liBookReservation = (List<LI_BOOK_RESERVE>)CMSHandler.FetchData<LI_BOOK_RESERVE>(new LI_USER_ACCOUNT() { USER_ID = circulationArgs.liUserAccount.FirstOrDefault().USER_ID, USER_TYPE = circulationArgs.liUserAccount.FirstOrDefault().USER_TYPE }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalBookReservedByUserId), sAcademicYear).DataSource.Data;
                            }
                            else
                            {
                                //fetch total circulation
                                CirculationFields.liBookCirculation = (List<LI_BOOK_CIRCULATION>)CMSHandler.FetchData<LI_BOOK_CIRCULATION>(new LI_USER_ACCOUNT() { USER_ID = circulationArgs.liUserAccount.FirstOrDefault().USER_ID, USER_TYPE = circulationArgs.liUserAccount.FirstOrDefault().USER_TYPE }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalBookIssuedByUserId), sAcademicYear).DataSource.Data;

                                //fetch reservation
                                CirculationFields.liBookReservation = (List<LI_BOOK_RESERVE>)CMSHandler.FetchData<LI_BOOK_RESERVE>(new LI_USER_ACCOUNT() { USER_ID = circulationArgs.liUserAccount.FirstOrDefault().USER_ID, USER_TYPE = circulationArgs.liUserAccount.FirstOrDefault().USER_TYPE }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalBookReservedByUserId), sAcademicYear).DataSource.Data;
                            }

                            //assign user info
                            CirculationFields.objUserAccount = circulationArgs.liUserAccount.FirstOrDefault();

                        }
                        else
                        {
                            CirculationFields.sMessage = "Authentication Problem!";
                            CirculationFields.sErrorCode = Common.ErrorCode.NotFound;
                        }
                    }
                    else
                    {
                        CirculationFields.sMessage = "You are not checked in!";
                        CirculationFields.sErrorCode = Common.ErrorCode.NotFound;
                    }

                }
                else
                {
                    CirculationFields.sMessage = "Invalid user Credentials!";
                    CirculationFields.sErrorCode = Common.ErrorCode.NotFound;
                }

            }
            return CirculationFields;
        }


        public CirculationArgs FetchUserDetails(string Id)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            CirculationArgs circulationArgs = new CirculationArgs();
            if (!string.IsNullOrEmpty(Id))
            {
                //check user by barcode
                var UserInfo = (List<LI_USER_ACCOUNT>)CMSHandler.FetchData<LI_USER_ACCOUNT>(new LI_USER_ACCOUNT() { SCAN_ID = Id }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserCredentialByBARCODE), sAcademicYear).DataSource.Data;
                if (UserInfo != null && UserInfo.Count > 0)
                {
                    circulationArgs.liUserAccount = UserInfo;

                }
                else
                {
                    //check user by rfid
                    var UserInfo1 = (List<LI_USER_ACCOUNT>)CMSHandler.FetchData<LI_USER_ACCOUNT>(new LI_USER_ACCOUNT() { SCAN_ID = Id }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserCredentialByRFID), sAcademicYear).DataSource.Data;
                    if (UserInfo1 != null && UserInfo1.Count > 0)
                    {
                        circulationArgs.liUserAccount = UserInfo1;
                    }
                }

            }
            return circulationArgs;
        }

        public string ValidateCheckIn(string sUserId, string sUserType, string sAcYear)
        {
            string sCheckInId = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(sUserId) && !string.IsNullOrEmpty(sUserType) && !string.IsNullOrEmpty(sAcYear))
                {
                    var ObjModel = new LI_CHECKIN_INFO();
                    ObjModel.USER_ID = sUserId;
                    ObjModel.USER_TYPE = sUserType;
                    var liValidate = (List<LI_CHECKIN_INFO>)CMSHandler.FetchData<LI_CHECKIN_INFO>(ObjModel, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchValidateCheckIn), sAcYear).DataSource.Data;
                    if (liValidate != null && liValidate.Count > 0)
                    {
                        sCheckInId = liValidate.FirstOrDefault().CHECKIN_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("ValidateCheckIn", "FormCheckInAndOut", ex.Message);
                }
            }
            return sCheckInId;
        }
        public string ValidateBulkCheckIn(string sUserId, string sUserType, string sAcYear, string CHECKIN_DATE )
        {
            string sCheckInId = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(sUserId) && !string.IsNullOrEmpty(sUserType) && !string.IsNullOrEmpty(sAcYear))
                {
                    var ObjModel = new LI_CHECKIN_INFO();
                    ObjModel.USER_ID = sUserId;
                    ObjModel.USER_TYPE = sUserType;
                    ObjModel.CHECKIN_DATE = CHECKIN_DATE;
                    var liValidate = (List<LI_CHECKIN_INFO>)CMSHandler.FetchData<LI_CHECKIN_INFO>(ObjModel, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchValidateCheckInbulk), sAcYear).DataSource.Data;
                    if (liValidate != null && liValidate.Count > 0)
                    {
                        sCheckInId = liValidate.FirstOrDefault().CHECKIN_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("ValidateCheckIn", "FormCheckInAndOut", ex.Message);
                }
            }
            return sCheckInId;
        }

        public ActionResult BindCirculation(List<LI_BOOK_CIRCULATION> sData)
        {
            var ObjViewModel = new LibraryViewModel();
            if (sData != null)
            {
                ObjViewModel.liBookCirculation = sData;
            }


            return View(ObjViewModel);
        }

        public void MakeZeroForRCount()
        {
            IssueAppend.rCount = 0;

        }


        public JsonResult FetchBookDetailsForIssueByRfidorBarcode(string sScanId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sScanId))
            {
                try
                {
                    IssueAppend.rCount = IssueAppend.rCount + 1;
                    //first rfid
                    var liBookInfo = (List<LI_BOOK_INFO>)CMSHandler.FetchData<LI_BOOK_INFO>(new LI_BOOK_INFO() { RFID = sScanId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.ListBooksForIssueByRfid), sAcademicYear).DataSource.Data;

                    if (liBookInfo != null && liBookInfo.Count > 0)
                    {
                        string sVal = string.Empty;
                        //if (liBookInfo.FirstOrDefault().C_STATUS!="1")
                        //{
                        sVal += "<tr cms-book-id=" + liBookInfo.FirstOrDefault().BOOK_ID + ">" +
                            "<td>" + IssueAppend.rCount + "</td>" +
                       "<td>" +
                           "<div class='checkbox checkbox-css'>" +
                                "<input type ='checkbox' id='cbx_" + IssueAppend.rCount + "' />" +
                                "<label for='cbx_" + IssueAppend.rCount + "'></label>" +
                             "</div>" +
                       "</td>" +
                       "<td>" + liBookInfo.FirstOrDefault().ACCESS_NO + "</td>" +
                       "<td>" + liBookInfo.FirstOrDefault().DOC_TITLE + "</td>" +
                       "<td>" + liBookInfo.FirstOrDefault().AUTHOR + "</td>" +
                       "<td>" + liBookInfo.FirstOrDefault().ISBN_NO + "</td>" +
                       //"<td>" + liBookInfo.FirstOrDefault().STATUS_MESSAGE + "</td>" +
                       "<td class='ms-lastCell hidden'></td></tr>";
                        //}
                        //else
                        //{
                        //    sVal += "<tr cms-book-id=" + liBookInfo.FirstOrDefault().BOOK_ID + ">" +
                        //           "<td>" + IssueAppend.rCount + "</td>" +
                        //       "<td>" +
                        //       "<div class='checkbox checkbox-css'>" +
                        //            "<input type ='checkbox' id='cbx_" + IssueAppend.rCount + "'/>" +
                        //            "<label for='cbx_" + IssueAppend.rCount + "'></label>" +
                        //         "</div>" +
                        //   "</td>" +
                        //  "<td>" + liBookInfo.FirstOrDefault().ACCESS_NO + "</td>" +
                        //  "<td>" + liBookInfo.FirstOrDefault().DOC_TITLE + "</td>" +
                        //  "<td>" + liBookInfo.FirstOrDefault().AUTHOR + "</td>" +
                        //  "<td>" + liBookInfo.FirstOrDefault().ISBN_NO + "</td>" +
                        //  "<td>" + liBookInfo.FirstOrDefault().STATUS_MESSAGE + "</td>"+
                        //   "<td class='ms-lastCell hidden'></td></tr>";
                        //}
                        objResultData.sResult = sVal;
                        objResultData.oResult = liBookInfo.FirstOrDefault().BOOK_ID;
                    }
                    else
                    {
                        //second barcode
                        var liBookInfo1 = (List<LI_BOOK_INFO>)CMSHandler.FetchData<LI_BOOK_INFO>(new LI_BOOK_INFO() { BARCODE = sScanId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.ListBooksForIssueByBarcode), sAcademicYear).DataSource.Data;

                        if (liBookInfo1 != null && liBookInfo1.Count > 0)
                        {
                            string sVal = string.Empty;
                            //if (liBookInfo1.FirstOrDefault().C_STATUS != "1")
                            //{
                            sVal += "<tr cms-book-id='" + liBookInfo1.FirstOrDefault().BOOK_ID + "'>" +
                                 "<td>" + IssueAppend.rCount + "</td>" +
                            "<td>" +
                           "<div class='checkbox checkbox-css'>" +
                                "<input type ='checkbox' id='cbx_" + IssueAppend.rCount + "' />" +
                                "<label for='cbx_" + IssueAppend.rCount + "'></label>" +
                             "</div>" +
                       "</td>" +
                           "<td>" + liBookInfo1.FirstOrDefault().ACCESS_NO + "</td>" +
                           "<td>" + liBookInfo1.FirstOrDefault().DOC_TITLE + "</td>" +
                           "<td>" + liBookInfo1.FirstOrDefault().AUTHOR + "</td>" +
                           "<td>" + liBookInfo1.FirstOrDefault().ISBN_NO + "</td>" +
                            //"<td>" + liBookInfo1.FirstOrDefault().STATUS_MESSAGE + "</td>";
                            "<td class='ms-lastCell hidden'></td></tr>";
                            //}
                            //else
                            //{
                            //    sVal += "<tr cms-id='" + liBookInfo1.FirstOrDefault().BOOK_ID + "'>" +
                            //         "<td>" + IssueAppend.rCount + "</td>" +
                            //       "<td>" +
                            //       "<div class='checkbox checkbox-css'>" +
                            //            "<input type ='checkbox' />" +
                            //            "<label for=''></label>" +
                            //         "</div>" +
                            //   "</td>" +
                            //  "<td>" + liBookInfo1.FirstOrDefault().ACCESS_NO + "</td>" +
                            //  "<td>" + liBookInfo1.FirstOrDefault().DOC_TITLE + "</td>" +
                            //  "<td>" + liBookInfo1.FirstOrDefault().AUTHOR + "</td>" +
                            //  "<td>" + liBookInfo1.FirstOrDefault().ISBN_NO + "</td>" +
                            //  "<td>" + liBookInfo1.FirstOrDefault().STATUS_MESSAGE + "</td>";
                            //}
                            objResultData.sResult = sVal;
                            objResultData.oResult = liBookInfo1.FirstOrDefault().BOOK_ID;
                        }
                        else
                        {
                            objResultData.Message = Common.Messages.NoRecordsfound;
                            objResultData.ErrorCode = Common.ErrorCode.NotFound;
                        }
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchBookDetailsForIssueByRfidorAccessNo", ex.Message);
                    }
                }
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        // Fetch Book List For Issue ,..
        public ActionResult FetchTotalBookForIssue()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(null,
                            LibrarySQL.GetLibrarySQL(LibrarySQLCommands.ListBooksForIssue), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchTotalBookForIssue", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        // Book Circulation ,..
        public JsonResult SaveBookCirculation(string sJsonBookDetails, string sMode)
        {

            string sAcadenmicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var objCirculationFields = new CIRCULATION_FIELDS();
            string sSemesterId = string.Empty;
            if (!string.IsNullOrEmpty(sAcadenmicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Save_Json_List>(sJsonBookDetails);
                    if (sMode == "0")
                    {
                        objCirculationFields = Issue(Result);
                    }
                    if (sMode == "1")
                    {
                        objCirculationFields = Return(Result);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveBookCirculation", ex.Message);
                    }
                }
            }
            else
            {
                objCirculationFields.sMessage = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objCirculationFields.sErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(objCirculationFields, JsonRequestBehavior.AllowGet);
        }
        private CIRCULATION_FIELDS Issue(Save_Json_List objJsonList)

        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string NoOfBok = string.Empty;
            string sReturnDate = string.Empty;
            var objCirculationFields = new CIRCULATION_FIELDS();
            string sMessage = string.Empty;

            if (objJsonList.liBookIssue != null && objJsonList.liBookIssue.Count > 0)
            {

                var liRule = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(new LI_RULES()
                {
                    USER_TYPE = objJsonList.liBookIssue.FirstOrDefault().USER_TYPE,
                    ROLE = !string.IsNullOrEmpty(objJsonList.liBookIssue.FirstOrDefault().ROLE) ? objJsonList.liBookIssue.FirstOrDefault().ROLE : "0",
                    PROGRAMME_LEVEL = !string.IsNullOrEmpty(objJsonList.liBookIssue.FirstOrDefault().PROGRAMME_LEVEL) ? objJsonList.liBookIssue.FirstOrDefault().PROGRAMME_LEVEL : "0"
                }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchRuleByUserTypeAndRole), sAcademicYear).DataSource.Data;

                if (liRule != null && liRule.Count > 0)
                {

                    //return date
                    //this is for fetching total calender days
                    var liFetchCalender = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchCalenderDetails), sAcademicYear).DataSource.Data;
                    List<CALENDER_2017> liCalender = new List<CALENDER_2017>();
                    if (liFetchCalender != null && liFetchCalender.Count > 0)
                    {
                        //filter only working days
                        liCalender = liFetchCalender.Where(x => x.IS_HOLIDAY != "1").ToList();
                    }
                    var liCurdate = liCalender.Where(x => x.IS_CURDATE == "1").ToList();

                    var liDueDates = new List<CALENDER_2017>();
                    if (liCurdate != null && liCurdate.Count > 0)
                    {
                        liDueDates = liCalender.Where(x => Convert.ToInt32(x.DAY_ID) > Convert.ToInt32(liCurdate.FirstOrDefault().DAY_ID)).Take(Convert.ToInt32(liRule.FirstOrDefault().NO_OF_DAYS)).ToList();
                    }
                    else
                    {
                        objCirculationFields.sMessage = "Today is holiday! ";
                        objCirculationFields.sErrorCode = Common.ErrorCode.Conflict;
                        return objCirculationFields;
                    }

                    //check library period
                    var liPeriods = (List<LI_PERIODS>)CMSHandler.FetchData<LI_PERIODS>(new LI_PERIODS()
                    {
                        USER_TYPE = objJsonList.liBookIssue.FirstOrDefault().USER_TYPE,
                        ROLE = !string.IsNullOrEmpty(objJsonList.liBookIssue.FirstOrDefault().ROLE) ? objJsonList.liBookIssue.FirstOrDefault().ROLE : "0",
                        PROGRAMME_LEVEL = !string.IsNullOrEmpty(objJsonList.liBookIssue.FirstOrDefault().PROGRAMME_LEVEL) ? objJsonList.liBookIssue.FirstOrDefault().PROGRAMME_LEVEL : "0"

                    }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchPeriodByUserType), sAcademicYear).DataSource.Data;

                    if (liPeriods != null && liPeriods.Count > 0)
                    {
                        var sReturnDayId = string.Empty;
                        var sSemStartDayId = string.Empty;
                        var sSemEndDayId = string.Empty;

                        sReturnDayId = liDueDates.LastOrDefault().DAY_ID;
                        var liSem = liCalender.Where(x => x.DAY_ORDER_DATE == liPeriods.FirstOrDefault().SEM_BEGIN_DATE).ToList();
                        if (liSem != null && liSem.Count > 0)
                        {
                            sSemStartDayId = liSem.FirstOrDefault().DAY_ID;
                        }
                        var liSem1 = liCalender.Where(x => x.DAY_ORDER_DATE == liPeriods.FirstOrDefault().SEM_END_DATE).ToList();

                        if (liSem1 != null && liSem1.Count > 0)
                        {
                            sSemEndDayId = liSem1.FirstOrDefault().DAY_ID;
                        }

                        if (string.IsNullOrEmpty(sSemStartDayId) || string.IsNullOrEmpty(sSemEndDayId))
                        {
                            objCirculationFields.sMessage = "library period was not valid dated! ";
                            objCirculationFields.sErrorCode = Common.ErrorCode.Conflict;
                            return objCirculationFields;
                        }
                        if (Convert.ToInt16(sReturnDayId) >= Convert.ToInt16(sSemStartDayId) && Convert.ToInt16(sReturnDayId) <= Convert.ToInt16(sSemEndDayId))
                        {
                            sReturnDate = liDueDates.LastOrDefault().DAY_ORDER_DATE;
                        }
                        else
                        {
                            objCirculationFields.sMessage = "library period is finished! ";
                            objCirculationFields.sErrorCode = Common.ErrorCode.Conflict;
                            return objCirculationFields;
                        }
                    }
                    else
                    {
                        objCirculationFields.sMessage = "library period was not added! ";
                        objCirculationFields.sErrorCode = Common.ErrorCode.Conflict;
                        return objCirculationFields;
                    }

                    //fetch previous transactions
                    var liTransaction = (List<LI_BOOK_ISSUE>)CMSHandler.FetchData<LI_BOOK_ISSUE>(new LI_USER_ACCOUNT() { USER_ID = objJsonList.liBookIssue.FirstOrDefault().USER_ID, USER_TYPE = objJsonList.liBookIssue.FirstOrDefault().USER_TYPE }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalBookIssuedByUserId), sAcademicYear).DataSource.Data;

                    if (liTransaction != null && liTransaction.Count > 0)
                    {
                        //check it first rule no of books
                        //NoOfBok = Convert.ToString(liTransaction.Where(x => x.STATUS == Common.LI_BOOK_STATUS.ISSUED || x.STATUS == Common.LI_BOOK_STATUS.RETURNED).ToList().Count);
                        NoOfBok = Convert.ToString(liTransaction.Count);
                        if (Convert.ToInt16(NoOfBok) + objJsonList.liBookIssue.Count > Convert.ToInt16(liRule.FirstOrDefault().NO_OF_BOOKS))
                        {

                            objCirculationFields.sMessage = "Sorry you are taking the book beyond the limit";
                            objCirculationFields.sErrorCode = Common.ErrorCode.Conflict;
                            return objCirculationFields;

                        }
                    }
                }
                else
                {
                    objCirculationFields.sMessage = "Library rule was not added ! ";
                    objCirculationFields.sErrorCode = Common.ErrorCode.Conflict;
                    return objCirculationFields;
                }


                foreach (var item in objJsonList.liBookIssue)
                {
                    if (!string.IsNullOrEmpty(item.USER_ID) && !string.IsNullOrEmpty(item.USER_TYPE))
                    {
                        //issue  books

                        //check if the book already reserved by anyone
                        //var liReserve = (List<LI_BOOK_RESERVE>)CMSHandler.FetchData<LI_BOOK_RESERVE>(new LI_BOOK_RESERVE() { BOOK_ID = item.BOOK_ID }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchActiveReservedBooksByBookId), sAcademicYear).DataSource.Data;
                        //if (liReserve != null && liReserve.Count > 0)
                        //{
                        //    objCirculationFields.liGridClass.Add(new GRID_CLASS() { BOOK_ID = item.BOOK_ID, STATUS = "Already reserved", COLOR = Common.COLOR.AQUA });
                        //    continue;
                        //}
                        //check book status is this circulation book
                        var liBookInfo = (List<LI_BOOK_INFO>)CMSHandler.FetchData<LI_BOOK_INFO>(new LI_BOOK_INFO() { BOOK_ID = item.BOOK_ID }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsById), sAcademicYear).DataSource.Data;
                        //FOR ADMIN LEVEL NO BOOK STATUS CHECK
                        if (liBookInfo != null && liBookInfo.Count > 0)
                        {
                            if (liBookInfo.FirstOrDefault().STATUS != Common.CIRCULATION_MODE.IS_CIRCULATION)
                            {
                                // objCirculationFields.liGridClass.Add(new GRID_CLASS() { BOOK_ID = item.BOOK_ID, STATUS = "Not a circulation book", COLOR = Common.COLOR.AQUA });
                                // continue;
                                sMessage = liBookInfo.FirstOrDefault().STATUS_NAME;
                            }
                        }
                        switch (liBookInfo.FirstOrDefault().LI_BOOK_STATUS)
                        {
                            case Common.LI_BOOK_STATUS.CIRCULATION:
                                break;
                            case Common.LI_BOOK_STATUS.DAMAGED:
                                objCirculationFields.liGridClass.Add(new GRID_CLASS() { BOOK_ID = item.BOOK_ID, STATUS = "this is damaged book", COLOR = Common.COLOR.NORMAL });
                                continue;
                            // break;
                            case Common.LI_BOOK_STATUS.ISSUED:
                                objCirculationFields.liGridClass.Add(new GRID_CLASS() { BOOK_ID = item.BOOK_ID, STATUS = "this is already issued book", COLOR = Common.COLOR.NORMAL });
                                continue;
                            // break;
                            case Common.LI_BOOK_STATUS.MISSED:
                                objCirculationFields.liGridClass.Add(new GRID_CLASS() { BOOK_ID = item.BOOK_ID, STATUS = "this is missed book", COLOR = Common.COLOR.NORMAL });
                                continue;
                            // break;
                            case Common.LI_BOOK_STATUS.RENEWED:
                                objCirculationFields.liGridClass.Add(new GRID_CLASS() { BOOK_ID = item.BOOK_ID, STATUS = "this is renewed book", COLOR = Common.COLOR.NORMAL });
                                continue;
                            // break;
                            case Common.LI_BOOK_STATUS.RESERVED:
                                objCirculationFields.liGridClass.Add(new GRID_CLASS() { BOOK_ID = item.BOOK_ID, STATUS = "this is reserved book", COLOR = Common.COLOR.NORMAL });
                                continue;
                            //break;
                            case Common.LI_BOOK_STATUS.RESERVE_APPROVED:

                                if (item.IS_RESERVE_APPROVED == Common.Delimiter.ONE)
                                {
                                    // book issuing from reserve
                                }
                                else
                                {
                                    objCirculationFields.liGridClass.Add(new GRID_CLASS() { BOOK_ID = item.BOOK_ID, STATUS = "this is reserve approved book", COLOR = Common.COLOR.NORMAL });
                                    continue;
                                }
                                break;
                            case Common.LI_BOOK_STATUS.RESERVE_CANCELLED:
                                objCirculationFields.liGridClass.Add(new GRID_CLASS() { BOOK_ID = item.BOOK_ID, STATUS = "this is reserve cancelled book", COLOR = Common.COLOR.NORMAL });
                                continue;
                            //break;
                            case Common.LI_BOOK_STATUS.RESERVE_REQUESTED:
                                objCirculationFields.liGridClass.Add(new GRID_CLASS() { BOOK_ID = item.BOOK_ID, STATUS = "this is reserve requested book", COLOR = Common.COLOR.NORMAL });
                                continue;
                                // break;
                        }

                        //save book issue
                        resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_ISSUE()
                        {
                            BOOK_ID = item.BOOK_ID,
                            USER_ID = item.USER_ID,
                            USER_TYPE = item.USER_TYPE,
                            ROLE = item.ROLE,
                            SEMESTER_TYPE = item.SEMESTER_TYPE,
                            SEMESTER_ID = item.SEMESTER_ID,
                            RETURN_DATE = sReturnDate,
                            ISSUE_BY = sUserId

                        }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveBookIssue), sAcademicYear);
                        if (resultArgs.Success)
                        {
                            objCirculationFields.liGridClass.Add(new GRID_CLASS() { BOOK_ID = item.BOOK_ID, STATUS = "Issued", COLOR = Common.COLOR.GREEN, IS_TRANSACTION_COMPLETED = Common.Delimiter.ONE });
                            //update book status
                            resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_INFO()
                            {
                                BOOK_ID = item.BOOK_ID,
                                LI_BOOK_STATUS = Common.LI_BOOK_STATUS.ISSUED

                            }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookInfoLiBookStatus), sAcademicYear);
                        }
                        else
                        {
                            objCirculationFields.liGridClass.Add(new GRID_CLASS() { BOOK_ID = item.BOOK_ID, STATUS = "Failed", COLOR = Common.COLOR.RED });
                        }
                        if (!string.IsNullOrEmpty(sMessage))
                        {
                            if (objCirculationFields.liGridClass != null && objCirculationFields.liGridClass.Count > 0)
                            {
                                objCirculationFields.liGridClass.FirstOrDefault().STATUS = objCirculationFields.liGridClass.FirstOrDefault().STATUS + " But this is " + sMessage + " book.";
                            }
                        }
                    }
                }
            }
            else
            {
                objCirculationFields.sMessage = Common.Messages.NoRecordsfound;
                objCirculationFields.sErrorCode = Common.ErrorCode.NoContent;
            }
            return objCirculationFields;
        }

        private CIRCULATION_FIELDS Return(Save_Json_List objJsonList)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string NoOfBok = string.Empty;
            string sReturnDate = string.Empty;
            var objCirculationFields = new CIRCULATION_FIELDS();

            try
            {
                if (objJsonList.liBookIssue.Count > 0)
                {
                    if (!string.IsNullOrEmpty(objJsonList.liBookIssue.FirstOrDefault().USER_ID) && !string.IsNullOrEmpty(objJsonList.liBookIssue.FirstOrDefault().USER_TYPE))
                    {

                        var liRule = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(new LI_RULES()
                        {
                            USER_TYPE = objJsonList.liBookIssue.FirstOrDefault().USER_TYPE,
                            ROLE = !string.IsNullOrEmpty(objJsonList.liBookIssue.FirstOrDefault().ROLE) ? objJsonList.liBookIssue.FirstOrDefault().ROLE : "0",
                            PROGRAMME_LEVEL = !string.IsNullOrEmpty(objJsonList.liBookIssue.FirstOrDefault().PROGRAMME_LEVEL) ? objJsonList.liBookIssue.FirstOrDefault().PROGRAMME_LEVEL : "0"
                        }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchRuleByUserTypeAndRole), sAcademicYear).DataSource.Data;


                        //this is for fetching total calender days
                        var liFetchCalender = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchCalenderDetails), sAcademicYear).DataSource.Data;
                        List<CALENDER_2017> liCalender = new List<CALENDER_2017>();
                        if (liFetchCalender != null && liFetchCalender.Count > 0)
                        {
                            //filter only working days
                            liCalender = liFetchCalender.Where(x => x.IS_HOLIDAY != "1").ToList();
                        }
                        var liCurdate = liCalender.Where(x => x.IS_CURDATE == "1").ToList();

                        if (liCurdate != null && liCurdate.Count > 0)
                        {

                        }
                        else
                        {
                            objCirculationFields.sMessage = "Today is holiday! ";
                            objCirculationFields.sErrorCode = Common.ErrorCode.Conflict;
                            return objCirculationFields;
                        }

                        if (liRule != null && liRule.Count > 0)
                        {
                            //return current books
                            foreach (var item in objJsonList.liBookIssue)
                            {
                                string sSuccessStatus = "Returned and wait for the admin confirmation!";
                                //fetch issue id
                                var liIssue = (List<LI_BOOK_ISSUE>)CMSHandler.FetchData<LI_BOOK_ISSUE>(new LI_BOOK_ISSUE()
                                {
                                    BOOK_ID = item.BOOK_ID,
                                    USER_ID = item.USER_ID,
                                    USER_TYPE = item.USER_TYPE

                                }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchIssueIdByBookIdandUserId), sAcademicYear).DataSource.Data;


                                if (liIssue != null && liIssue.Count > 0)
                                {
                                    //check if it any pending fine amound
                                    if (!string.IsNullOrEmpty(liIssue.FirstOrDefault().FINE) && Convert.ToInt16(liIssue.FirstOrDefault().FINE) > 0)
                                    {
                                        sSuccessStatus += "Need to pay fine";
                                    }

                                    //save book return
                                    resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_ISSUE()
                                    {

                                        IS_RETURNED = Common.Delimiter.ONE,
                                        ISSUE_ID = liIssue.FirstOrDefault().ISSUE_ID,
                                        RECEIVED_BY = sUserId

                                    }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveBookReturn), sAcademicYear);
                                    if (resultArgs.Success)
                                    {
                                        objCirculationFields.liGridClass.Add(new GRID_CLASS() { BOOK_ID = item.BOOK_ID, STATUS = sSuccessStatus, COLOR = Common.COLOR.GREEN });
                                        //update book status
                                        resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_INFO()
                                        {
                                            BOOK_ID = item.BOOK_ID,
                                            LI_BOOK_STATUS = Common.LI_BOOK_STATUS.CIRCULATION

                                        }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookInfoLiBookStatus), sAcademicYear);

                                    }
                                    else
                                    {
                                        objCirculationFields.liGridClass.Add(new GRID_CLASS() { BOOK_ID = item.BOOK_ID, STATUS = "Failed", COLOR = Common.COLOR.RED });
                                    }
                                }
                                else
                                {
                                    objCirculationFields.liGridClass.Add(new GRID_CLASS() { BOOK_ID = item.BOOK_ID, STATUS = "not issued", COLOR = Common.COLOR.AQUA });
                                }
                            }
                        }
                        else
                        {

                            objCirculationFields.sMessage = "Library rule was not added!";
                            objCirculationFields.sErrorCode = Common.ErrorCode.NoContent;
                        }
                    }
                    else
                    {

                        objCirculationFields.sMessage = "Something went wrong Please try again later!";
                        objCirculationFields.sErrorCode = Common.ErrorCode.NoContent;
                    }
                    //objCirculationFields.sMessage = "Wait for the admin confirmation!";
                    //objCirculationFields.sErrorCode = Common.ErrorCode.NoContent;
                }
                else
                {

                    objCirculationFields.sMessage = "No books were founded!";
                    objCirculationFields.sErrorCode = Common.ErrorCode.NoContent;

                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objLog = new ErrorLog())
                {
                    objLog.WriteError("Circulation", "Return", ex.Message);
                }
            }
            finally
            {

            }
            return objCirculationFields;
        }

        public ActionResult ReturnedTransaction()
        {

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //fetch all returned transaction which is not confirmed by admin
                    var liTransaction = (List<LI_BOOK_ISSUE>)CMSHandler.FetchData<LI_BOOK_ISSUE>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchAllReturnedTransactionWhichNotConfirmed), sAcademicYear).DataSource.Data;
                    if (liTransaction != null && liTransaction.Count > 0)
                    {
                        ObjViewModel.liBookIssue = liTransaction;
                    }

                    //fetch library book status
                    var Status = (List<SUP_LI_BOOK_STATUS>)CMSHandler.FetchData<SUP_LI_BOOK_STATUS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStatusDDL), sAcademicYear).DataSource.Data;
                    // here unwanted status
                    if (Status != null && Status.Count > 0)
                    {
                        Status.RemoveAll(x => x.STATUS_ID == Common.LI_BOOK_STATUS.ISSUED || x.STATUS_ID == Common.LI_BOOK_STATUS.MISSED || x.STATUS_ID == Common.LI_BOOK_STATUS.CIRCULATION || x.STATUS_ID == Common.LI_BOOK_STATUS.RENEWED
                  || x.STATUS_ID == Common.LI_BOOK_STATUS.RESERVE_APPROVED || x.STATUS_ID == Common.LI_BOOK_STATUS.RESERVE_CANCELLED || x.STATUS_ID == Common.LI_BOOK_STATUS.RESERVE_REQUESTED
                  || x.STATUS_ID == Common.LI_BOOK_STATUS.RESERVED
                  );
                        ObjViewModel.sStatusDDL = new SelectList(Status, Common.SUP_LI_BOOK_STATUS.STATUS_ID, Common.SUP_LI_BOOK_STATUS.STATUS_NAME, Common.LI_BOOK_STATUS.RETURNED);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "ReturnedTransaction", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult SaveBookReturnedTransaction(string sJsonTransactionDetails)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var liIssueConfirmed = new List<string>();
            var liIssueDamaged = new List<LI_BOOK_TRANSACTION>();
            //  string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            int sCount = 0;
            int fCount = 0;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Save_Json_List>(sJsonTransactionDetails);
                    if (Result.liBookTransaction != null && Result.liBookTransaction.Count > 0)
                    {
                        foreach (var item in Result.liBookTransaction)
                        {
                            if (item.BOOK_STATUS == Common.LI_BOOK_STATUS.DAMAGED)
                            {
                                //update  return is_confirmed 
                                //resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_ISSUE()
                                //{
                                //    ISSUE_ID = item.ISSUE_ID,
                                //    IS_RETURN_CONFIRMED = Common.Delimiter.ONE
                                //}, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateIssueReturnConfirmedByIssueId), sAcademicYear);

                                // and update return is_damaged field

                                resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_ISSUE()
                                {
                                    ISSUE_ID = item.ISSUE_ID,
                                    IS_DAMAGED = Common.Delimiter.ONE
                                }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateIssueReturnDamagedByIssueId), sAcademicYear);

                                var liDues = (List<LI_ISSUE_DUES>)CMSHandler.FetchData<LI_ISSUE_DUES>(new LI_BOOK_ISSUE() { ISSUE_ID = item.ISSUE_ID },
                              LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchDuesIdByIssueId), sAcademicYear).DataSource.Data;

                                if (resultArgs.Success)
                                {
                                    sCount++;
                                }
                                else
                                {
                                    fCount++;
                                }

                                if (liDues != null && liDues.Count > 0)
                                {
                                    //update fine in damage fine in dues table
                                    resultArgs = CMSHandler.SaveOrUpdate(new LI_ISSUE_DUES()
                                    {
                                        ISSUE_ID = item.ISSUE_ID,
                                        DAMAGE_FINE_AMOUNT = item.FINE
                                    }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiDuesForDamageFine), sAcademicYear);
                                }
                                else
                                {
                                    var InsertDues = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveDues), sAcademicYear);
                                    if (InsertDues.Success)
                                    {
                                        //update fine in damage fine in dues table
                                        resultArgs = CMSHandler.SaveOrUpdate(new LI_ISSUE_DUES()
                                        {
                                            ISSUE_ID = item.ISSUE_ID,
                                            DAMAGE_FINE_AMOUNT = item.FINE
                                        }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiDuesForDamageFine), sAcademicYear);
                                    }
                                }

                                //add update book status is damaged
                                //fetch book id by issue id
                                var liBookInfo = (List<LI_BOOK_INFO>)CMSHandler.FetchData<LI_BOOK_INFO>(new LI_BOOK_ISSUE() { ISSUE_ID = item.ISSUE_ID }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookInfoByIssueId), sAcademicYear).DataSource.Data;
                                if (liBookInfo != null && liBookInfo.Count > 0)
                                {
                                    resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_INFO()
                                    {
                                        BOOK_ID = liBookInfo.FirstOrDefault().BOOK_ID,
                                        LI_BOOK_STATUS = Common.LI_BOOK_STATUS.DAMAGED

                                    }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookInfoLiBookStatus), sAcademicYear);
                                }

                                //add issue id for sending sms
                                liIssueDamaged.Add(item);
                                //objCirculationFields.liGridClass.Add(new GRID_CLASS() { ISSUE_ID = item.ISSUE_ID, STATUS = "success", COLOR = Common.COLOR.AQUA });

                            }
                            else
                            {

                                //update  issue is_confirmed
                                resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_ISSUE()
                                {
                                    ISSUE_ID = item.ISSUE_ID,
                                    IS_RETURN_CONFIRMED = Common.Delimiter.ONE
                                }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateIssueReturnConfirmedByIssueId), sAcademicYear);

                                if (resultArgs.Success)
                                {
                                    //add issue_id for sending sms
                                    liIssueConfirmed.Add(item.ISSUE_ID);
                                    sCount++;


                                }
                                else
                                {
                                    fCount++;

                                }
                            }
                        }

                        if (sCount != 0)
                        {
                            ObjJsonData.Message = "( " + sCount.ToString() + " ) " + Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        if (fCount != 0)
                        {
                            ObjJsonData.Message = "( " + fCount.ToString() + " ) " + Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }

                        if (liIssueConfirmed != null && liIssueConfirmed.Count > 0)
                        {
                            //sent sms
                            SendSmsForIsReturnConfirmed(liIssueConfirmed);
                        }
                        if (liIssueDamaged != null && liIssueDamaged.Count > 0)
                        {
                            //sent sms
                            SendSmsForIsReturnDamaged(liIssueDamaged);
                        }

                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveBookReturnedTransaction", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IssuedTransaction()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //fetch all issued transaction which is not returned 

                    var liIssue = new List<LI_BOOK_ISSUE>();

                    var liTransactionForStudents = (List<LI_BOOK_ISSUE>)CMSHandler.FetchData<LI_BOOK_ISSUE>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchAllReturnedTransactionWhichIsNotReturned), sAcademicYear).DataSource.Data;
                    if (liTransactionForStudents != null && liTransactionForStudents.Count > 0)
                    {
                        ObjViewModel.liBookIssue = liTransactionForStudents;
                    }

                    //fetch library book status
                    var Status = (List<SUP_LI_BOOK_STATUS>)CMSHandler.FetchData<SUP_LI_BOOK_STATUS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStatusDDL), sAcademicYear).DataSource.Data;
                    // here unwanted status

                    if (Status != null && Status.Count > 0)
                    {
                        Status.RemoveAll(x => x.STATUS_ID == Common.LI_BOOK_STATUS.ISSUED || x.STATUS_ID == Common.LI_BOOK_STATUS.DAMAGED || x.STATUS_ID == Common.LI_BOOK_STATUS.CIRCULATION || x.STATUS_ID == Common.LI_BOOK_STATUS.RENEWED
                   || x.STATUS_ID == Common.LI_BOOK_STATUS.RESERVE_APPROVED || x.STATUS_ID == Common.LI_BOOK_STATUS.RESERVE_CANCELLED || x.STATUS_ID == Common.LI_BOOK_STATUS.RESERVE_REQUESTED
                   || x.STATUS_ID == Common.LI_BOOK_STATUS.RESERVED || x.STATUS_ID == Common.LI_BOOK_STATUS.RETURNED
                   );
                        ObjViewModel.sStatusDDL = new SelectList(Status, Common.SUP_LI_BOOK_STATUS.STATUS_ID, Common.SUP_LI_BOOK_STATUS.STATUS_NAME, Common.LI_BOOK_STATUS.RETURNED);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "IssuedTransaction", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult SaveBookIssuedTransaction(string sJsonTransactionDetails)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var liIssueMissing = new List<LI_BOOK_TRANSACTION>();
            //  string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            var objCirculationFields = new CIRCULATION_FIELDS();
            int sCount = 0;
            int fCount = 0;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Save_Json_List>(sJsonTransactionDetails);
                    if (Result.liBookTransaction != null && Result.liBookTransaction.Count > 0)
                    {
                        foreach (var item in Result.liBookTransaction)
                        {
                            if (item.BOOK_STATUS == Common.LI_BOOK_STATUS.MISSED)
                            {
                                //update  issue is_returned 
                                //resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_ISSUE()
                                //{
                                //    ISSUE_ID = item.ISSUE_ID,
                                //    IS_RETURNED = Common.Delimiter.ONE
                                //}, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveBookReturn), sAcademicYear);

                                // and update issue is_missed field
                                resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_ISSUE()
                                {
                                    ISSUE_ID = item.ISSUE_ID,
                                    IS_MISSED = Common.Delimiter.ONE
                                }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateIssueReturnMisseddByIssueId), sAcademicYear);

                                if (resultArgs.Success)
                                {
                                    sCount++;

                                }
                                else
                                {
                                    fCount++;
                                }

                                var liDues = (List<LI_ISSUE_DUES>)CMSHandler.FetchData<LI_ISSUE_DUES>(new LI_BOOK_ISSUE() { ISSUE_ID = item.ISSUE_ID },
                             LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchDuesIdByIssueId), sAcademicYear).DataSource.Data;

                                if (liDues != null && liDues.Count > 0)
                                {
                                    //update fine in damage fine in dues table
                                    resultArgs = CMSHandler.SaveOrUpdate(new LI_ISSUE_DUES()
                                    {
                                        ISSUE_ID = item.ISSUE_ID,
                                        MISSING_FINE_AMOUNT = item.FINE
                                    }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiDuesForMissingFine), sAcademicYear);

                                }
                                else
                                {
                                    var InsertDues = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveDues), sAcademicYear);
                                    if (InsertDues.Success)
                                    {
                                        //add fine in damage fine in dues table
                                        resultArgs = CMSHandler.SaveOrUpdate(new LI_ISSUE_DUES()
                                        {
                                            ISSUE_ID = item.ISSUE_ID,
                                            MISSING_FINE_AMOUNT = item.FINE
                                        }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiDuesForMissingFine), sAcademicYear);

                                    }
                                }
                                //add update book status is damaged
                                //fetch book id by issue id
                                var liBookInfo = (List<LI_BOOK_INFO>)CMSHandler.FetchData<LI_BOOK_INFO>(new LI_BOOK_ISSUE() { ISSUE_ID = item.ISSUE_ID }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookInfoByIssueId), sAcademicYear).DataSource.Data;
                                if (liBookInfo != null && liBookInfo.Count > 0)
                                {
                                    resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_INFO()
                                    {
                                        BOOK_ID = liBookInfo.FirstOrDefault().BOOK_ID,
                                        LI_BOOK_STATUS = Common.LI_BOOK_STATUS.MISSED

                                    }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookInfoLiBookStatus), sAcademicYear);
                                }

                                //add issue id for sending sms
                                liIssueMissing.Add(item);

                            }
                        }
                        if (sCount != 0)
                        {
                            ObjJsonData.Message = "( " + sCount.ToString() + " ) " + Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        if (fCount != 0)
                        {
                            ObjJsonData.Message = "( " + fCount.ToString() + " ) " + Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }

                        if (liIssueMissing != null && liIssueMissing.Count > 0)
                        {
                            //sent sms
                            SendSmsForBookMissing(liIssueMissing);
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveBookReturnedTransaction", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Book Circulation For User

        public ActionResult BookCirculationForUser()
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserType = (Session[Common.SESSION_VARIABLES.USER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LibraryViewModel();

            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    //fetch reservation 
                    ObjViewModel.liBookCirculation = (List<LI_BOOK_CIRCULATION>)CMSHandler.FetchData<LI_BOOK_CIRCULATION>(new LI_USER_ACCOUNT() { USER_ID = sUserId, USER_TYPE = sUserType }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalBookIssuedByUserId), sAcademicyear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "BindCirculationForUser", ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }
        #endregion

        #region Book Renewal
        public ActionResult FetchBookForRenewal(string sUserId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //check user expired or not
                    var Expired = CheckUserExpiredorNot(sUserId);
                    if (Expired != null && Expired.Count > 0)
                    {
                        ObjViewModel.Message = "You are expired";

                        return Json(ObjViewModel, JsonRequestBehavior.AllowGet);
                    }
                    var liLoginDetails = ValidateUserLogin(sUserId);
                    if (liLoginDetails != null && liLoginDetails.Count > 0)
                    {
                        //check if user logged in or not by is_active field
                        if (liLoginDetails.FirstOrDefault().IS_ACTIVE == Common.IsActiveFalg.IsActive)
                        {
                        }
                        else
                        {
                            ObjViewModel.Message = "You are not looged in ";
                            return Json(ObjViewModel, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        ObjViewModel.Message = "You are not looged in ";
                        return Json(ObjViewModel, JsonRequestBehavior.AllowGet);
                    }




                    var liBookList = (List<ListBookIssue>)CMSHandler.FetchData<ListBookIssue>(new LI_BOOK_ISSUE() { USER_ID = sUserId, STATUS = Common.Sup_li_Status.Issue },
                            LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookIssueDetailsById), sAcademicYear).DataSource.Data;
                    ObjViewModel.liBookIssueList = liBookList;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchBookForRenewal", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult SaveBookRenew(string sJsonBookDetails)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            // string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            int nCount = 0;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Save_Json_List>(sJsonBookDetails);
                    if (Result.liBookIssue != null && Result.liBookIssue.Count > 0)
                    {
                        foreach (var item in Result.liBookIssue)
                        {
                            nCount++;

                            //check if book already renewed or not in this semester
                            var isExist = (List<LI_BOOK_ISSUE>)CMSHandler.FetchData<LI_BOOK_ISSUE>(new LI_BOOK_ISSUE() { BOOK_ID = item.BOOK_ID, USER_ID = item.USER_ID },
                                       SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookIssueBySemesterAndBookId), sAcademicYear).DataSource.Data;
                            if (isExist != null && isExist.Count > 0)
                            {

                                ObjJsonData.Message = "Per semester only one time is permitted for renew! ";
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                            }

                            //check if it any fine 
                            if (!string.IsNullOrEmpty(item.FINE))
                            {
                                if (Convert.ToInt16(item.FINE) > 0)
                                {
                                    ObjJsonData.Message = "Please pay the fine " + item.FINE + " ! ";
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                                }
                            }

                            //fech renew day id for adding extra days based on user type
                            var RenewId = GetRenewDayByUserId(item.USER_ID);
                            if (!string.IsNullOrEmpty(RenewId))
                            {
                                item.RECEIVED_BY = RenewId;
                                var SaveBookRenew = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookIssueForRenewal), sAcademicYear);

                                ObjJsonData.ErrorCode = SaveBookRenew.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                            }
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = "Failed to Renew book";
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveBookRetrun", ex.Message);
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

        #region Book Reserve For Admin
        public ActionResult BookReserve()
        {
            string sAcadenmicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcadenmicYear))
            {
                try
                {

                    //fetch library book status
                    var Status = (List<SUP_LI_BOOK_STATUS>)CMSHandler.FetchData<SUP_LI_BOOK_STATUS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStatusDDL), sAcadenmicYear).DataSource.Data;
                    // here unwanted status
                    Status.RemoveAll(x => x.STATUS_ID == Common.LI_BOOK_STATUS.ISSUED || x.STATUS_ID == Common.LI_BOOK_STATUS.MISSED || x.STATUS_ID == Common.LI_BOOK_STATUS.CIRCULATION || x.STATUS_ID == Common.LI_BOOK_STATUS.RENEWED
                    || x.STATUS_ID == Common.LI_BOOK_STATUS.RETURNED || x.STATUS_ID == Common.LI_BOOK_STATUS.RESERVE_REQUESTED
                    || x.STATUS_ID == Common.LI_BOOK_STATUS.RESERVED || x.STATUS_ID == Common.LI_BOOK_STATUS.DAMAGED
                    );
                    if (Status != null && Status.Count > 0)
                    {
                        ObjViewModel.sStatusDDL = new SelectList(Status, Common.SUP_LI_BOOK_STATUS.STATUS_ID, Common.SUP_LI_BOOK_STATUS.STATUS_NAME, Common.LI_BOOK_STATUS.RESERVE_APPROVED);
                    }


                    //fetch reserve requested 

                    ObjViewModel.liBookReserve = (List<LI_BOOK_RESERVE>)CMSHandler.FetchData<LI_BOOK_RESERVE>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchAllReserveRequestedTransaction), sAcadenmicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "BookReserve", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult SaveBookReservationTransaction(string sJsonTransactionDetails)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var liReserveApproved = new List<string>();
            var liReserveCancelled = new List<LI_BOOK_TRANSACTION>();
            //  string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            var objCirculationFields = new CIRCULATION_FIELDS();
            int sCount = 0;
            int fCount = 0;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Save_Json_List>(sJsonTransactionDetails);
                    if (Result.liBookReserve != null && Result.liBookReserve.Count > 0)
                    {
                        foreach (var item in Result.liBookReserve)
                        {
                            if (item.IS_CANCELLED == Common.Delimiter.ONE)
                            {
                                item.CANCELLED_BY = sUserId;
                                //update  is_cancelled in reservation table
                                resultArgs = CMSHandler.SaveOrUpdate(item, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateReserveCancelledByReserveId), sAcademicYear);
                                if (resultArgs.Success)
                                {
                                    sCount++;

                                }
                                else
                                {
                                    fCount++;
                                }

                                //add update book status is circulation
                                //fetch book id by reserve id
                                var liBookInfo = (List<LI_BOOK_INFO>)CMSHandler.FetchData<LI_BOOK_INFO>(new LI_BOOK_RESERVE() { RESERVE_ID = item.RESERVE_ID }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookInfoByReserveId), sAcademicYear).DataSource.Data;
                                if (liBookInfo != null && liBookInfo.Count > 0)
                                {
                                    resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_INFO()
                                    {
                                        BOOK_ID = liBookInfo.FirstOrDefault().BOOK_ID,
                                        LI_BOOK_STATUS = Common.LI_BOOK_STATUS.CIRCULATION

                                    }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookInfoLiBookStatus), sAcademicYear);
                                }

                                //add issue id for sending sms
                                liReserveCancelled.Add(new LI_BOOK_TRANSACTION()
                                {
                                    RESERVE_ID = item.RESERVE_ID,
                                    RESERVE_CANCEL_REASON = item.CANCEL_REASON
                                });

                                objCirculationFields.liGridClass.Add(new GRID_CLASS() { RESERVE_ID = item.RESERVE_ID, STATUS = "success", COLOR = Common.COLOR.AQUA });

                            }
                            else
                            {
                                item.APPROVED_BY = sUserId;
                                //update  reserve  is approved
                                resultArgs = CMSHandler.SaveOrUpdate(item, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateReserveApprovedByReserveId), sAcademicYear);

                                if (resultArgs.Success)
                                {

                                    //add update book status is approved

                                    //fetch book id by reserve id
                                    var liBookInfo = (List<LI_BOOK_INFO>)CMSHandler.FetchData<LI_BOOK_INFO>(new LI_BOOK_RESERVE() { RESERVE_ID = item.RESERVE_ID }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookInfoByReserveId), sAcademicYear).DataSource.Data;
                                    if (liBookInfo != null && liBookInfo.Count > 0)
                                    {
                                        resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_INFO()
                                        {
                                            BOOK_ID = liBookInfo.FirstOrDefault().BOOK_ID,
                                            LI_BOOK_STATUS = Common.LI_BOOK_STATUS.RESERVE_APPROVED

                                        }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookInfoLiBookStatus), sAcademicYear);
                                    }
                                    //add issue_id for sending sms
                                    liReserveApproved.Add(item.RESERVE_ID);
                                    sCount++;

                                }
                                else
                                {
                                    //objCirculationFields.liGridClass.Add(new GRID_CLASS() { RESERVE_ID = item.RESERVE_ID, STATUS = "failed", COLOR = Common.COLOR.AQUA });
                                    fCount++;

                                }
                            }
                        }

                        if (sCount != 0)
                        {
                            ObjJsonData.Message = "( " + sCount.ToString() + " ) " + Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        if (fCount != 0)
                        {
                            ObjJsonData.Message = "( " + fCount.ToString() + " ) " + Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                        if (liReserveApproved != null && liReserveApproved.Count > 0)
                        {
                            //sent sms
                            SendSmsForBookReserveApproved(liReserveApproved);
                            //ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            //ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;

                        }
                        if (liReserveCancelled != null && liReserveCancelled.Count > 0)
                        {
                            //sent sms
                            SendSmsForBookReserveCancelling(liReserveCancelled);
                            //ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            //ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        }

                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveBookReturnedTransaction", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reserve()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "Reserve", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindReservation(List<LI_BOOK_RESERVE> sData)
        {
            var ObjViewModel = new LibraryViewModel();
            if (sData != null)
            {
                ObjViewModel.liBookReserve = sData;
            }


            return View(ObjViewModel);
        }

        public JsonResult SaveBookReservationTransactionForCollected(string sJsonTransactionDetails)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var sStatus = 0;
            //  string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Save_Json_List>(sJsonTransactionDetails);
                    if (Result.liBookReserve != null && Result.liBookReserve.Count > 0)
                    {
                        foreach (var item in Result.liBookReserve)
                        {
                            if (!string.IsNullOrEmpty(item.RESERVE_ID))
                            {
                                item.COLLECTED_BY = sUserId;
                                //update  is_collected in reservation table
                                resultArgs = CMSHandler.SaveOrUpdate(item, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateReserveCollectedByReserveId), sAcademicYear);
                                sStatus++;
                            }
                        }
                        if (sStatus > 0)
                        {
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            ObjJsonData.Message = "" + sStatus + "Books Reserved Successfully";
                        }
                    }
                    else
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                        ObjJsonData.Message = "" + sStatus + "Books Reserved Successfully";
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveBookReservationTransactionForCollected", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FetchBookReserveByUser(string sUserId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchBookReserveByUser", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult SaveBookReservation(string sJsonReserveDetails)
        {
            string sAcadenmicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sRole = (Session[Common.SESSION_VARIABLES.USER_ROLE_NAME] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_NAME].ToString() : string.Empty;
            string sUserType = (Session[Common.SESSION_VARIABLES.USER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            var liDetails = new List<LI_USER_DETAILS>();

            if (!string.IsNullOrEmpty(sAcadenmicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Save_Json_List>(sJsonReserveDetails);
                    if (Result.liBookReserve != null && Result.liBookReserve.Count > 0)
                    {

                        // fetch boid or rfid 
                        var UserDetails = FetchBoidorRfidByUserId(sUserId, sRole);
                        if (UserDetails != null && UserDetails.Count > 0)
                        {
                            liDetails = ValidateMembership(UserDetails.FirstOrDefault().BOID, UserDetails.FirstOrDefault().RFID);
                            if (liDetails != null && liDetails.Count > 0)
                            {

                                //validate user expired or not
                                var isExpired = CheckUserExpiredorNot(liDetails.FirstOrDefault().USER_ID);
                                if (isExpired != null && isExpired.Count > 0)
                                {
                                    ObjJsonData.Message = "You are Expired!";
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Forbidden;
                                }
                                else
                                {

                                    foreach (var item in Result.liBookReserve)
                                    {
                                        item.USER_ID = liDetails.FirstOrDefault().USER_ID;

                                        //check per semester one time only reservation is possible
                                        var liReserveDetails = new List<LI_BOOK_RESERVE>();



                                        //fetch member type
                                        var liMemberType = (List<LI_USER_DETAILS>)CMSHandler.FetchData<LI_USER_DETAILS>(new LI_USER_DETAILS() { USER_ID = item.USER_ID },
                                                SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiMemberTypeByLiUserId), sAcadenmicYear).DataSource.Data;




                                        if (liMemberType != null && liMemberType.Count > 0)
                                        {
                                            switch (liMemberType.FirstOrDefault().MEMBER_TYPE)
                                            {


                                                case Common.LI_MEMBER_CATEGORY.STUDENT:
                                                    {
                                                        liReserveDetails = (List<LI_BOOK_RESERVE>)CMSHandler.FetchData<LI_BOOK_RESERVE>(new LI_BOOK_RESERVE() { BOOK_ID = item.BOOK_ID, USER_ID = item.USER_ID },
                                                         SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookReserveBySemesterAndBookIdForStudent), sAcadenmicYear).DataSource.Data;
                                                        if (liReserveDetails != null && liReserveDetails.Count > 0)
                                                        {
                                                            ObjJsonData.Message = "Per semester only one time is permitted for reserve! ";
                                                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                                            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                                                        }

                                                        //fetch current semester for student
                                                        item.SEMESTER_ID = GetCurrentSemesterByLiUserId(item.USER_ID);
                                                        break;
                                                    }
                                                case Common.LI_MEMBER_CATEGORY.TEACHING_STAFF:
                                                    {

                                                        liReserveDetails = (List<LI_BOOK_RESERVE>)CMSHandler.FetchData<LI_BOOK_RESERVE>(new LI_BOOK_RESERVE() { BOOK_ID = item.BOOK_ID, USER_ID = item.USER_ID },
                                                         SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookReserveBySemesterAndBookIdForStaff), sAcadenmicYear).DataSource.Data;
                                                        if (liReserveDetails != null && liReserveDetails.Count > 0)
                                                        {
                                                            ObjJsonData.Message = "Per semester only one time is permitted for reserve! ";
                                                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                                            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                                                        }

                                                        //fetch current semester for student
                                                        item.SEMESTER_TYPE = GetCurrentSemesteTyper();


                                                        break;
                                                    }
                                            }

                                        }
                                        else
                                        {

                                        }

                                        //add reservation
                                        var SaveBookReserve = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.AddBookReserve), sAcadenmicYear);
                                        if (SaveBookReserve.Success)
                                        {
                                            var UpdateBookDetailsStatus = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookInfoLiBookStatus), sAcadenmicYear);
                                            ObjJsonData.ErrorCode = UpdateBookDetailsStatus.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                            ObjJsonData.Message = UpdateBookDetailsStatus.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                        }
                                        else
                                        {
                                            ObjJsonData.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ObjJsonData.Message = "You are not a library member!";
                                ObjJsonData.ErrorCode = Common.ErrorCode.Forbidden;
                            }
                        }
                    }
                    else
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.Forbidden;
                        ObjJsonData.Message = "No Changes Made !";
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveBookReservation", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public List<LI_USER_DETAILS> FetchBoidorRfidByUserId(string sId, string sRole)
        {
            var result = new List<LI_USER_DETAILS>();
            string sAcadenmicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sId) && !string.IsNullOrEmpty(sRole) && !string.IsNullOrEmpty(sAcadenmicYear))
            {
                // fetch for staff
                if (sRole == Common.USER_ROLES.STAFF)
                {
                    var StaffInfo = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO() { STAFF_ID = sId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoByStaffId), sAcadenmicYear).DataSource.Data;
                    if (StaffInfo != null && StaffInfo.Count > 0)
                    {
                        result.Add(new LI_USER_DETAILS()
                        {
                            BOID = StaffInfo.FirstOrDefault().BOID_NUMBER,
                            RFID = StaffInfo.FirstOrDefault().RFID
                        });

                    }
                }
                if (sRole == Common.USER_ROLES.STUDENT)
                {
                    var StudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(new STU_PERSONAL_INFO() { STUDENT_ID = sId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByStudentId), sAcadenmicYear).DataSource.Data;
                    if (StudentInfo != null && StudentInfo.Count > 0)
                    {
                        result.Add(new LI_USER_DETAILS()
                        {
                            BOID = StudentInfo.FirstOrDefault().BIOD_NUMBER,
                            RFID = StudentInfo.FirstOrDefault().RFID
                        });
                    }
                }

            }

            return result;
        }

        #endregion

        #region Book Circulation  Cancel
        public ActionResult BookCirculationForCancel()
        {
            string sAcadenmicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcadenmicYear))
            {
                try
                {
                    ObjViewModel.liBookCirculation = (List<LI_BOOK_CIRCULATION>)CMSHandler.FetchData<LI_BOOK_CIRCULATION>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchAllReservedBooks), sAcadenmicYear).DataSource.Data;
                    ObjViewModel.liBookCirculation1 = (List<LI_BOOK_CIRCULATION>)CMSHandler.FetchData<LI_BOOK_CIRCULATION>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchAllRenewedBooks), sAcadenmicYear).DataSource.Data;


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "BookCirculationForCancel", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sJsonReserveDetails"></param>
        /// <param name="sMode"></param>
        /// <returns></returns>
        public JsonResult SaveBookCirculationForCancel(string sJsonReserveDetails, string sMode)
        {
            string sAcadenmicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            List<LI_BOOK_RESERVE> liBookReserve = new List<LI_BOOK_RESERVE>();
            List<LI_BOOK_ISSUE> liBookIssue = new List<LI_BOOK_ISSUE>();
            if (!string.IsNullOrEmpty(sAcadenmicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Save_Json_List>(sJsonReserveDetails);
                    if (sMode == "1")
                    {
                        if (Result.liBookReserve != null && Result.liBookReserve.Count > 0)
                        {

                            foreach (var item in Result.liBookReserve)
                            {


                                //ubook issue
                                LI_BOOK_RESERVE obReserve = new LI_BOOK_RESERVE()
                                {
                                    RESERVE_ID = item.RESERVE_ID,
                                    //IS_CANCELLED = item.IS_CANCELLED


                                };
                                var UpdatebookReserve = CMSHandler.SaveOrUpdate(obReserve, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookReserveDetailsForCancel), sAcadenmicYear);
                                if (UpdatebookReserve.Success)
                                {
                                    var UpdateBookDetailsStatus = CMSHandler.SaveOrUpdate(new LI_BOOK_DETAILS() { BOOK_ID = item.BOOK_ID, BOOK_STATUS = item.ACADEMIC_YEAR }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookInfoLiBookStatus), sAcadenmicYear);

                                    ObjJsonData.ErrorCode = UpdateBookDetailsStatus.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    ObjJsonData.Message = UpdateBookDetailsStatus.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;

                                    //for sending sms
                                    liBookReserve.Add(new LI_BOOK_RESERVE() { RESERVE_ID = item.RESERVE_ID });

                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                            if (liBookReserve != null && liBookReserve.Count > 0)
                            {
                                //SendSmsForBookReservedCancel(liBookReserve);
                            }
                        }
                        else
                        {
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            ObjJsonData.Message = "No Changes Made !";
                        }
                    }
                    if (sMode == "2")
                    {
                        if (Result.liBookIssue != null && Result.liBookIssue.Count > 0)
                        {
                            foreach (var item in Result.liBookIssue)
                            {
                                //ubook issue
                                LI_BOOK_ISSUE obIssue = new LI_BOOK_ISSUE()
                                {
                                    ISSUE_ID = item.ISSUE_ID,
                                    //IS_RENEW_CANCELLED = item.IS_RENEW_CANCELLED
                                };
                                var UpdatebookIssue = CMSHandler.SaveOrUpdate(obIssue, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookRenewDetailsForCancel), sAcadenmicYear);
                                if (UpdatebookIssue.Success)
                                {
                                    var UpdateBookDetailsStatus = CMSHandler.SaveOrUpdate(new LI_BOOK_DETAILS() { BOOK_ID = item.BOOK_ID, BOOK_STATUS = item.BOOK_STATUS }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookInfoLiBookStatus), sAcadenmicYear);

                                    ObjJsonData.ErrorCode = UpdateBookDetailsStatus.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    ObjJsonData.Message = UpdateBookDetailsStatus.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;


                                    //for sending sms
                                    liBookIssue.Add(new LI_BOOK_ISSUE() { ISSUE_ID = item.ISSUE_ID });
                                }
                            }
                            if (liBookIssue != null && liBookIssue.Count > 0)
                            {
                                SendSmsForBookRenewedCancel(liBookIssue);
                            }
                        }
                        else
                        {
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveBookReservation", ex.Message);
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

        #region Recall

        #endregion

        #region Dues
        public ActionResult DuesList()
        {

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "DuesList", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindDues(List<LI_BOOK_CIRCULATION> sData)
        {
            var ObjViewModel = new LibraryViewModel();
            if (sData != null)
            {
                ObjViewModel.liBookCirculation = sData;
            }


            return View(ObjViewModel);
        }
        public ActionResult BindPay(LI_ISSUE_DUES sData)
        {
            var ObjViewModel = new LibraryViewModel();
            if (sData != null)
            {
                ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                ObjViewModel.liIssueDues.Add(sData);
            }


            return View(ObjViewModel);
        }
        public ActionResult BindReciept(LI_ISSUE_DUES sData)
        {
            var ObjViewModel = new LibraryViewModel();
            if (sData != null)
            {
                ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                ObjViewModel.liIssueDues.Add(sData);
            }


            return View(ObjViewModel);
        }


        public JsonResult FetchDueDetails(string sDuesId)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sDuesId))
            {
                //fetch all returned transaction which is not confirmed by admin
                var liDueDetails = (List<LI_ISSUE_DUES>)CMSHandler.FetchData<LI_ISSUE_DUES>(new LI_ISSUE_DUES() { DUES_ID = sDuesId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchDueDetailsForPaymentByDuesId), sAcademicYear).DataSource.Data;
                if (liDueDetails != null && liDueDetails.Count > 0)
                {
                    objResultData.oResult = liDueDetails.FirstOrDefault();
                }
                else
                {
                    objResultData.Message = Common.Messages.NoRecordsfound;
                    objResultData.ErrorCode = Common.ErrorCode.NoContent;
                }
            }
            return Json(objResultData);
        }
        public JsonResult FetchDueDetailsForReceipt(string sDuesId)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sDuesId))
            {
                //fetch all returned transaction which is not confirmed by admin
                var liDueDetails = (List<LI_ISSUE_DUES>)CMSHandler.FetchData<LI_ISSUE_DUES>(new LI_ISSUE_DUES() { DUES_ID = sDuesId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchDueDetailsForRecieptByDuesId), sAcademicYear).DataSource.Data;
                if (liDueDetails != null && liDueDetails.Count > 0)
                {
                    objResultData.oResult = liDueDetails.FirstOrDefault();
                }
                else
                {
                    objResultData.Message = Common.Messages.NoRecordsfound;
                    objResultData.ErrorCode = Common.ErrorCode.NoContent;
                }
            }
            return Json(objResultData);
        }

        public JsonResult PayFineByDuesId(string sDuesId, string sFineAmount)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sDuesId) && !string.IsNullOrEmpty(sFineAmount))
            {
                //before pay fine check the book returned or not

                var Exist = (List<LI_ISSUE_DUES>)CMSHandler.FetchData<LI_ISSUE_DUES>(new LI_ISSUE_DUES() { DUES_ID = sDuesId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.CheckUserReturnedBookorNot), sAcademicYear).DataSource.Data;
                if (Exist != null && Exist.Count > 0)
                {

                    resultArgs = CMSHandler.SaveOrUpdate(new LI_ISSUE_DUES() { DUES_ID = sDuesId, IS_PAID = Common.Delimiter.ONE, DEBIT = sFineAmount }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.PayFineAmountByDuesId), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? "Successfully Paid The Fine" : "Not Successfully Paid The Fine";
                    objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                else
                {
                    objResultData.Message = "Without returning the book can't pay the fee";
                    objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }


            return Json(objResultData);
        }
        #endregion

        #region Report

        public ActionResult LibraryDailyReport()
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            try
            {
                var resAcademicYear = CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList));
                if (resAcademicYear != null && resAcademicYear.DataSource != null && resAcademicYear.DataSource.Data != null)
                {
                    var Academicyear = (List<ACADEMIC_YEAR>)resAcademicYear.DataSource.Data;
                    if (Academicyear != null && Academicyear.Count > 0)
                    {
                        objViewModel.sYear = new SelectList(Academicyear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                }

                var resSubject = CMSHandler.FetchData<SUP_LI_SUBJECT_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSubjectTypeDDL));
                if (resSubject != null && resSubject.DataSource != null && resSubject.DataSource.Data != null)
                {
                    var liSubject = (List<SUP_LI_SUBJECT_TYPE>)resSubject.DataSource.Data;
                    if (liSubject != null && liSubject.Count > 0)
                    {
                        objViewModel.SubjectType = new SelectList(liSubject, Common.SUP_LI_SUBJECT_TYPE.SUBJECT_ID, Common.SUP_LI_SUBJECT_TYPE.SUBJECT);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError("LibraryController", "LibraryDailyReport", "FetchDropdowns", ex.Message);
                }
            }
            return View(objViewModel);
        }
        private string FormatDateForLibraryReport(string sDate)
        {
            if (string.IsNullOrEmpty(sDate)) return string.Empty;
            DateTime dt;
            string[] formats = new string[] { "dd/MM/yyyy", "dd-MM-yyyy", "yyyy-MM-dd", "yyyy/MM/dd" };
            if (DateTime.TryParseExact(sDate.Trim(), formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt))
            {
                return dt.ToString("yyyy-MM-dd");
            }
            return sDate.Trim();
        }

        public ActionResult BindIssuedBooks(string sFromDate, string sToDate, string sDate = "")
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (string.IsNullOrEmpty(sAcademicYear)) sAcademicYear = FetchAcademicYear();

                if (string.IsNullOrEmpty(sFromDate) && !string.IsNullOrEmpty(sDate)) sFromDate = sDate;
                if (string.IsNullOrEmpty(sToDate)) sToDate = sFromDate;
                if (string.IsNullOrEmpty(sFromDate)) sFromDate = DateTime.Now.ToString("dd/MM/yyyy");
                if (string.IsNullOrEmpty(sToDate)) sToDate = sFromDate;

                ViewBag.FromDate = sFromDate;
                ViewBag.ToDate = sToDate;

                // Fetch College Details ....
                var resCollege = CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear);
                if (resCollege != null && resCollege.DataSource != null && resCollege.DataSource.Data != null)
                {
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)resCollege.DataSource.Data;
                }

                string sFormattedFromDate = FormatDateForLibraryReport(sFromDate);
                string sFormattedToDate = FormatDateForLibraryReport(sToDate);

                var paramObj = new
                {
                    FROM_DATE = sFormattedFromDate,
                    TO_DATE = sFormattedToDate,
                    ISSUE_DATE = sFormattedFromDate
                };

                var res = CMSHandler.FetchData<LI_REPORT_FIELDS>(paramObj, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalIssuedBooksByDate), sAcademicYear);
                if (res != null && res.DataSource != null && res.DataSource.Data != null)
                {
                    var result = (List<LI_REPORT_FIELDS>)res.DataSource.Data;
                    if (result != null && result.Count > 0)
                    {
                        objViewModel.liReportFields = result;
                    }
                    else
                    {
                        objViewModel.Message = Common.Messages.NoRecordsfound;
                    }
                }
                else
                {
                    objViewModel.Message = Common.Messages.NoRecordsfound;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError("LibraryController", "BindIssuedBooks", "FetchReport", ex.Message);
                }
                objViewModel.Message = Common.Messages.NoRecordsfound;
            }
            return View(objViewModel);
        }

        public ActionResult BindReturnedBooks(string sFromDate, string sToDate, string sDate = "")
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (string.IsNullOrEmpty(sAcademicYear)) sAcademicYear = FetchAcademicYear();

                if (string.IsNullOrEmpty(sFromDate) && !string.IsNullOrEmpty(sDate)) sFromDate = sDate;
                if (string.IsNullOrEmpty(sToDate)) sToDate = sFromDate;
                if (string.IsNullOrEmpty(sFromDate)) sFromDate = DateTime.Now.ToString("dd/MM/yyyy");
                if (string.IsNullOrEmpty(sToDate)) sToDate = sFromDate;

                ViewBag.FromDate = sFromDate;
                ViewBag.ToDate = sToDate;

                // Fetch College Details ....
                var resCollege = CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear);
                if (resCollege != null && resCollege.DataSource != null && resCollege.DataSource.Data != null)
                {
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)resCollege.DataSource.Data;
                }

                string sFormattedFromDate = FormatDateForLibraryReport(sFromDate);
                string sFormattedToDate = FormatDateForLibraryReport(sToDate);

                var paramObj = new
                {
                    FROM_DATE = sFormattedFromDate,
                    TO_DATE = sFormattedToDate,
                    ACTUAL_RETURNED_DATE = sFormattedFromDate
                };

                var res = CMSHandler.FetchData<LI_REPORT_FIELDS>(paramObj, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalReturnedBooksByDate), sAcademicYear);
                if (res != null && res.DataSource != null && res.DataSource.Data != null)
                {
                    var result = (List<LI_REPORT_FIELDS>)res.DataSource.Data;
                    if (result != null && result.Count > 0)
                    {
                        objViewModel.liReportFields = result;
                    }
                    else
                    {
                        objViewModel.Message = Common.Messages.NoRecordsfound;
                    }
                }
                else
                {
                    objViewModel.Message = Common.Messages.NoRecordsfound;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError("LibraryController", "BindReturnedBooks", "FetchReport", ex.Message);
                }
                objViewModel.Message = Common.Messages.NoRecordsfound;
            }
            return View(objViewModel);
        }

        public ActionResult BindReservedBooks(string sFromDate, string sToDate, string sDate = "")
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (string.IsNullOrEmpty(sAcademicYear)) sAcademicYear = FetchAcademicYear();

                if (string.IsNullOrEmpty(sFromDate) && !string.IsNullOrEmpty(sDate)) sFromDate = sDate;
                if (string.IsNullOrEmpty(sToDate)) sToDate = sFromDate;
                if (string.IsNullOrEmpty(sFromDate)) sFromDate = DateTime.Now.ToString("dd/MM/yyyy");
                if (string.IsNullOrEmpty(sToDate)) sToDate = sFromDate;

                ViewBag.FromDate = sFromDate;
                ViewBag.ToDate = sToDate;

                // Fetch College Details ....
                var resCollege = CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear);
                if (resCollege != null && resCollege.DataSource != null && resCollege.DataSource.Data != null)
                {
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)resCollege.DataSource.Data;
                }

                string sFormattedFromDate = FormatDateForLibraryReport(sFromDate);
                string sFormattedToDate = FormatDateForLibraryReport(sToDate);

                var paramObj = new
                {
                    FROM_DATE = sFormattedFromDate,
                    TO_DATE = sFormattedToDate,
                    ENTRY_DATE = sFormattedFromDate
                };

                var res = CMSHandler.FetchData<LI_REPORT_FIELDS>(paramObj, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalReservedBooksByDate), sAcademicYear);
                if (res != null && res.DataSource != null && res.DataSource.Data != null)
                {
                    var result = (List<LI_REPORT_FIELDS>)res.DataSource.Data;
                    if (result != null && result.Count > 0)
                    {
                        objViewModel.liReportFields = result;
                    }
                    else
                    {
                        objViewModel.Message = Common.Messages.NoRecordsfound;
                    }
                }
                else
                {
                    objViewModel.Message = Common.Messages.NoRecordsfound;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError("LibraryController", "BindReservedBooks", "FetchReport", ex.Message);
                }
                objViewModel.Message = Common.Messages.NoRecordsfound;
            }
            return View(objViewModel);
        }

        public ActionResult BindLibraryVisitors(string sFromDate, string sToDate, string sDate = "")
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (string.IsNullOrEmpty(sAcademicYear)) sAcademicYear = FetchAcademicYear();

                if (string.IsNullOrEmpty(sFromDate) && !string.IsNullOrEmpty(sDate)) sFromDate = sDate;
                if (string.IsNullOrEmpty(sToDate)) sToDate = sFromDate;
                if (string.IsNullOrEmpty(sFromDate)) sFromDate = DateTime.Now.ToString("dd/MM/yyyy");
                if (string.IsNullOrEmpty(sToDate)) sToDate = sFromDate;

                ViewBag.FromDate = sFromDate;
                ViewBag.ToDate = sToDate;

                // Fetch College Details ....
                var resCollege = CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear);
                if (resCollege != null && resCollege.DataSource != null && resCollege.DataSource.Data != null)
                {
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)resCollege.DataSource.Data;
                }

                string sFormattedFromDate = FormatDateForLibraryReport(sFromDate);
                string sFormattedToDate = FormatDateForLibraryReport(sToDate);

                var paramObj = new
                {
                    FROM_DATE = sFormattedFromDate,
                    TO_DATE = sFormattedToDate,
                    CHECKIN_DATE = sFormattedFromDate
                };

                var res = CMSHandler.FetchData<LI_REPORT_FIELDS>(paramObj, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.BindTotalVisitorsByDate), sAcademicYear);
                if (res != null && res.DataSource != null && res.DataSource.Data != null)
                {
                    var result = (List<LI_REPORT_FIELDS>)res.DataSource.Data;
                    if (result != null && result.Count > 0)
                    {
                        objViewModel.liReportFields = result;
                    }
                    else
                    {
                        objViewModel.Message = Common.Messages.NoRecordsfound;
                    }
                }
                else
                {
                    objViewModel.Message = Common.Messages.NoRecordsfound;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError("LibraryController", "BindLibraryVisitors", "FetchReport", ex.Message);
                }
                objViewModel.Message = Common.Messages.NoRecordsfound;
            }
            return View(objViewModel);
        }

        public ActionResult BindLibraryCheckoutEntry(string sFromDate, string sToDate, string sDate = "")
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (string.IsNullOrEmpty(sAcademicYear)) sAcademicYear = FetchAcademicYear();

                if (string.IsNullOrEmpty(sFromDate) && !string.IsNullOrEmpty(sDate)) sFromDate = sDate;
                if (string.IsNullOrEmpty(sToDate)) sToDate = sFromDate;
                if (string.IsNullOrEmpty(sFromDate)) sFromDate = DateTime.Now.ToString("dd/MM/yyyy");
                if (string.IsNullOrEmpty(sToDate)) sToDate = sFromDate;

                ViewBag.FromDate = sFromDate;
                ViewBag.ToDate = sToDate;

                // Fetch College Details ....
                var resCollege = CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear);
                if (resCollege != null && resCollege.DataSource != null && resCollege.DataSource.Data != null)
                {
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)resCollege.DataSource.Data;
                }

                string sFormattedFromDate = FormatDateForLibraryReport(sFromDate);
                string sFormattedToDate = FormatDateForLibraryReport(sToDate);

                var paramObj = new
                {
                    FROM_DATE = sFormattedFromDate,
                    TO_DATE = sFormattedToDate,
                    CHECKIN_DATE = sFormattedFromDate
                };

                var res = CMSHandler.FetchData<LI_REPORT_FIELDS>(paramObj, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.BindLibraryCheckoutEntryByDate), sAcademicYear);
                if (res != null && res.DataSource != null && res.DataSource.Data != null)
                {
                    var result = (List<LI_REPORT_FIELDS>)res.DataSource.Data;
                    if (result != null && result.Count > 0)
                    {
                        objViewModel.liReportFields = result;
                    }
                    else
                    {
                        objViewModel.Message = Common.Messages.NoRecordsfound;
                    }
                }
                else
                {
                    objViewModel.Message = Common.Messages.NoRecordsfound;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError("LibraryController", "BindLibraryCheckoutEntry", "FetchReport", ex.Message);
                }
                objViewModel.Message = Common.Messages.NoRecordsfound;
            }
            return View(objViewModel);
        }
        //public JsonResult UpdateLibraryBulkCheckout(string sJsonArticleDetails)
        //{
        //    string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
        //        ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()
        //        : string.Empty;

        //    var ObjJsonData = new JsonResultData();

        //    if (!string.IsNullOrEmpty(sAcademicyear))
        //    {
        //        try
        //        {
        //            // Deserialize LIST
        //            var checkoutList = JsonConvert.DeserializeObject<List<LI_CHECKIN_INFO>>(sJsonArticleDetails);

        //            if (checkoutList != null && checkoutList.Count > 0)
        //            {
        //                int timedu = 0;
        //                foreach (var item in checkoutList)
        //                {
        //                    timedu +=2000;
        //                    item.CHECKOUT_DATE = DateTime.Now.AddMilliseconds(timedu).ToString("yyyy-M-dd HH:mm:ss");

        //                    var liUpdateCheckout =
        //                        CMSHandler.SaveOrUpdate(
        //                            item,
        //                            LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLibraryCheckout),
        //                            sAcademicyear
        //                        );

        //                    if (!liUpdateCheckout.Success)
        //                    {
        //                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
        //                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
        //                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        //                    }
        //                }

        //                ObjJsonData.Message = "Checkout updated successfully";
        //                ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
        //            }
        //            else
        //            {
        //                ObjJsonData.Message = "No records selected";
        //                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            using (ErrorLog ObjLog = new ErrorLog())
        //            {
        //                ObjLog.WriteError("LibraryController", "LibraryBulkCheckout", ex.Message);
        //            }

        //            ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
        //            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
        //        }
        //    }
        //    else
        //    {
        //        ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
        //        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
        //    }

        //    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        //}



        public JsonResult UpdateLibraryBulkCheckout(string sJsonArticleDetails)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()
                : string.Empty;

            var ObjJsonData = new JsonResultData();

            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    var checkoutList = JsonConvert.DeserializeObject<List<LI_CHECKIN_INFO>>(sJsonArticleDetails);

                    if (checkoutList != null && checkoutList.Count > 0)
                    {
                        // ✅ Capture base time ONCE
                        DateTime baseTime = DateTime.Now;
                        int timeDelaySeconds = 0;

                        foreach (var item in checkoutList)
                        {
                            // ✅ Increase by 2 seconds each record
                            item.CHECKOUT_DATE = baseTime
                                .AddSeconds(timeDelaySeconds)
                                .ToString("yyyy-MM-dd HH:mm:ss");

                            timeDelaySeconds += 2;

                            var liUpdateCheckout = CMSHandler.SaveOrUpdate(
                                item,
                                LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLibraryCheckout),
                                sAcademicyear
                            );

                            if (!liUpdateCheckout.Success)
                            {
                                ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                            }
                        }

                        ObjJsonData.Message = "Checkout updated successfully";
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        ObjJsonData.Message = "No records selected";
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "UpdateLibraryBulkCheckout", ex.Message);
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


        public ActionResult BindLibraryFineList(string sDate)
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sDate))
            {
                // Fetch College Details ....
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                if (!string.IsNullOrEmpty(sDate))
                {
                    sDate = CommonMethods.DateConvertionForPayU(sDate);
                }
                objViewModel.liReportFields = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_DUES() { PAYMENT_DATE = sDate }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalFineNotPaidListByDate), sAcademicYear).DataSource.Data;
            }
            return View(objViewModel);
        }
        public ActionResult BindLibraryFinePaidList(string sFromDate, string sToDate, string sDate = "")
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (string.IsNullOrEmpty(sAcademicYear)) sAcademicYear = FetchAcademicYear();

                if (string.IsNullOrEmpty(sFromDate) && !string.IsNullOrEmpty(sDate)) sFromDate = sDate;
                if (string.IsNullOrEmpty(sToDate)) sToDate = sFromDate;
                if (string.IsNullOrEmpty(sFromDate)) sFromDate = DateTime.Now.ToString("dd/MM/yyyy");
                if (string.IsNullOrEmpty(sToDate)) sToDate = sFromDate;

                ViewBag.FromDate = sFromDate;
                ViewBag.ToDate = sToDate;

                // Fetch College Details ....
                var resCollege = CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear);
                if (resCollege != null && resCollege.DataSource != null && resCollege.DataSource.Data != null)
                {
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)resCollege.DataSource.Data;
                }

                string sFormattedFromDate = FormatDateForLibraryReport(sFromDate);
                string sFormattedToDate = FormatDateForLibraryReport(sToDate);

                var paramObj = new
                {
                    FROM_DATE = sFormattedFromDate,
                    TO_DATE = sFormattedToDate,
                    PAYMENT_DATE = sFormattedFromDate
                };

                var res = CMSHandler.FetchData<LI_REPORT_FIELDS>(paramObj, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalFinePaidListByDate), sAcademicYear);
                if (res != null && res.DataSource != null && res.DataSource.Data != null)
                {
                    var result = (List<LI_REPORT_FIELDS>)res.DataSource.Data;
                    if (result != null && result.Count > 0)
                    {
                        objViewModel.liReportFields = result;
                    }
                    else
                    {
                        objViewModel.Message = Common.Messages.NoRecordsfound;
                    }
                }
                else
                {
                    objViewModel.Message = Common.Messages.NoRecordsfound;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError("LibraryController", "BindLibraryFinePaidList", "FetchReport", ex.Message);
                }
                objViewModel.Message = Common.Messages.NoRecordsfound;
            }
            return View(objViewModel);
        }
        public ActionResult IndividualStudentReport()
        {
            return View();
        }


        // Year wise Report
        public ActionResult LibraryYearReport()
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
             SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
            if (liAcademicYear != null && liAcademicYear.Count > 0)
            {
                objViewModel.sAcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
            }
            return View(objViewModel);
        }
        public ActionResult BindYearIssuedBooks(string sYear, string sDate)
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sYear))
            {
                // Fetch College Details ....
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                if (!string.IsNullOrEmpty(sDate))
                {
                    sDate = CommonMethods.ConvertdatetoyearFromat(sDate);
                    // sDate = "2020-07-09";
                }
                var result = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_BOOK_ISSUE() { ACADEMIC_YEAR = sYear, ISSUE_DATE = sDate }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalIssuedBooksByYear), sYear).DataSource.Data;

                if (result != null && result.Count > 0)
                {
                    objViewModel.liReportFields = result;
                }
                else
                {

                    objViewModel.Message = Common.Messages.NoRecordsfound;

                }

            }
            return View(objViewModel);
        }
        public ActionResult BindYearReturnedBooks(string sYear, string sDate)
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sYear))
            {
                if (!string.IsNullOrEmpty(sDate))
                {
                    sDate = CommonMethods.ConvertdatetoyearFromat(sDate);
                    // sDate = "2020-07-09";
                }
                // Fetch College Details ....
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;

                objViewModel.liReportFields = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_BOOK_ISSUE() { ACADEMIC_YEAR = sYear, ACTUAL_RETURNED_DATE = sDate }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalReturnedBooksByYear), sYear).DataSource.Data;
            }
            return View(objViewModel);
        }
        public ActionResult BindYearReservedBooks(string sYear, string sDate)
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sYear))
            {
                if (!string.IsNullOrEmpty(sDate))
                {
                    sDate = CommonMethods.ConvertdatetoyearFromat(sDate);
                    // sDate = "2020-07-09";
                }
                // Fetch College Details ....
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;

                objViewModel.liReportFields = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_BOOK_RESERVE() { ACADEMIC_YEAR = sYear, ENTRY_DATE = sDate }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalReservedBooksByYear), sYear).DataSource.Data;
            }
            return View(objViewModel);
        }
        public ActionResult BindYearLibraryFinePaidList(string sYear, string sDate)
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sYear))
            {
                if (!string.IsNullOrEmpty(sDate))
                {
                    sDate = CommonMethods.ConvertdatetoyearFromat(sDate);
                    // sDate = "2020-07-09";
                }
                // Fetch College Details ....
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;

                objViewModel.liReportFields = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_DUES() { ACADEMIC_YEAR = sYear, PAYMENT_DATE = sDate }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalFinePaidListByYear), sYear).DataSource.Data;
            }
            return View(objViewModel);
        }
        public ActionResult BindYearLibraryVisitors(string sYear, string sDate)
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sYear))
            {
                if (!string.IsNullOrEmpty(sDate))
                {
                    sDate = CommonMethods.ConvertdatetoyearFromat(sDate);
                    // sDate = "2020-07-09";
                }
                // Fetch College Details ....
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                if (!string.IsNullOrEmpty(sYear))
                    objViewModel.liReportFields = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_CHECKIN_INFO() { ACADEMIC_YEAR = sYear, CHECKIN_DATE = sDate }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.BindTotalVisitorsByYear), sYear).DataSource.Data;
            }
            return View(objViewModel);
        }



        // occasional report 

        public ActionResult OccasionalReport()
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                //var supjectType =(List<SUP_LI_SUBJECTS>) CMSHandler.FetchData<SUP_LI_SUBJECTS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLiSubjectType), sAcademicYear).DataSource.Data;
                //if(supjectType!=null && supjectType.Count>0)
                //{
                //    objViewModel.SubjectType = new SelectList(supjectType, Common.SUP_LI_SUBJECTS.LI_SUBJECT_ID, Common.SUP_LI_SUBJECTS.SUBJECT_CODE);
                //}
            }
            return View(objViewModel);
        }
        public ActionResult BindLibraryMembership()
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                // Fetch College Details ....
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;

                objViewModel.liReportFields = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalLibraryMembers), sAcademicYear).DataSource.Data;
            }
            return View(objViewModel);
        }
        public ActionResult BindTotalBookDetails()
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                // Fetch College Details ....
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;

                objViewModel.liReportFields = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.BindTotalBookDetails), sAcademicYear).DataSource.Data;
            }
            return View(objViewModel);
        }
        public ActionResult BindTotalBookDetailsByOccasional(string sField, string sBeginValue, string sEndValue)
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sField) && !string.IsNullOrEmpty(sBeginValue) && !string.IsNullOrEmpty(sEndValue))
            {
                // Fetch College Details ....
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;



                //fetch total book details by field begin and end value



                objViewModel.liReportFields = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.BindTotalBookDetails), sAcademicYear).DataSource.Data;
            }
            return View(objViewModel);
        }
        public ActionResult BindLibraryExpiredMembers()
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                // Fetch College Details ....
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;

                objViewModel.liReportFields = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalExpiredMembersByToday), sAcademicYear).DataSource.Data;
            }
            return View(objViewModel);
        }

        //public ActionResult BindLibraryHomeReading()
        //{
        //    LibraryViewModel objViewModel = new LibraryViewModel();
        //    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
        //    if (!string.IsNullOrEmpty(sAcademicYear))
        //    {
        //        // Fetch College Details ....
        //        objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
        //            ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;

        //        objViewModel.liReportFields = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalHomeReaders), sAcademicYear).DataSource.Data;
        //    }
        //    return View(objViewModel);
        //}

        public ActionResult BindLibraryFrequentIssue(string sStartDate, string sEndDate)
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sStartDate) && !string.IsNullOrEmpty(sEndDate))
            {
                // Fetch College Details ....
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                if (!string.IsNullOrEmpty(sStartDate))
                {
                    sStartDate = CommonMethods.ConvertdatetoyearFromat(sStartDate);
                }
                if (!string.IsNullOrEmpty(sEndDate))
                {
                    sEndDate = CommonMethods.ConvertdatetoyearFromat(sEndDate);
                }
                objViewModel.liReportFields = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_BOOK_ISSUE() { START_DATE = sStartDate, END_DATE = sEndDate }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchFrequentIssueByStartDateandEndDate), sAcademicYear).DataSource.Data;
            }
            return View(objViewModel);
        }
        public ActionResult BindLibraryFrequentUsers(string sStartDate, string sEndDate)
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sStartDate) && !string.IsNullOrEmpty(sEndDate))
            {
                // Fetch College Details ....
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                if (!string.IsNullOrEmpty(sStartDate))
                {
                    sStartDate = CommonMethods.ConvertdatetoyearFromat(sStartDate);
                }
                if (!string.IsNullOrEmpty(sEndDate))
                {
                    sEndDate = CommonMethods.ConvertdatetoyearFromat(sEndDate);
                }
                objViewModel.liReportFields = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_LOGIN_DETAILS() { START_DATE = sStartDate, END_DATE = sEndDate }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchFrequentVisitorsByStartandEndDate), sAcademicYear).DataSource.Data;
            }
            return View(objViewModel);
        }

        public ActionResult BindLibrarySubjectWiseBook(string sBookNo)
        {
            LibraryViewModel objViewModel = new LibraryViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                // Fetch College Details ....
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                var sSQL = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.BindTotalBookDetailsBySubjectWise);
                objViewModel.liReportFields = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_BOOK_DETAILS() { BOOK_NO = sBookNo }, sSQL.Replace(Common.Delimiter.QUS + Common.LI_BOOK_DETAILS.BOOK_NO, sBookNo), sAcademicYear).DataSource.Data;
            }
            return View(objViewModel);
        }
        //public ActionResult BindLibraryFinePaidList()
        //{
        //    LibraryViewModel objViewModel = new LibraryViewModel();
        //    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
        //    if (!string.IsNullOrEmpty(sAcademicYear))
        //    {


        //        // Fetch College Details ....
        //        objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
        //            ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;


        //        objViewModel.liReportFields = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalFinePaidList), sAcademicYear).DataSource.Data;

        //    }
        //    return View(objViewModel);
        //}
        #endregion

        #region Articles
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ListArticlesDetails()
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    ObjViewModel.liBookCirculation = (List<LI_BOOK_CIRCULATION>)CMSHandler.FetchData<LI_BOOK_CIRCULATION>(null, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchTotalArticles), sAcademicyear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "ListArticlesDetails", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult AddArticleDetails(string sBookId)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {

                    // Series ,..
                    var liSeries = (List<SUP_LI_SERIES>)CMSHandler.FetchData<SUP_LI_SERIES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSeriesDDL)).DataSource.Data;
                    if (liSeries != null && liSeries.Count > 0)
                    {
                        ObjViewModel.sSeriesDDL = new SelectList(liSeries, Common.SUP_LI_SERIES.SERIES_ID, Common.SUP_LI_SERIES.SERIES_NAME);
                    }

                    //rack info
                    var Rack = (List<LI_RACK_INFO>)CMSHandler.FetchData<LI_RACK_INFO>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLiRacks)).DataSource.Data;
                    if (Rack != null && Rack.Count > 0)
                    {
                        //  ObjViewModel.sRack = new SelectList(Rack, Common.LI_RACK_INFO.RACK_ID, Common.LI_RACK_INFO.RACK_NO);
                    }

                    //Article type
                    var liArticle = (List<SUP_LI_ARTICLE_TYPE>)CMSHandler.FetchData<SUP_LI_ARTICLE_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchArticleType)).DataSource.Data;
                    if (liArticle != null && liArticle.Count > 0)
                    {
                        ObjViewModel.ArticleType = new SelectList(liArticle, Common.SUP_LI_ARTICLE_TYPE.ARTICLE_TYPE_ID, Common.SUP_LI_ARTICLE_TYPE.ARTICLE_TYPE_NAME);
                    }
                    //Author info
                    var Author = (List<LI_AUTHOR_INFO>)CMSHandler.FetchData<LI_AUTHOR_INFO>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLiAuthors)).DataSource.Data;
                    if (Author != null && Author.Count > 0)
                    {
                        ObjViewModel.sAuthor = new SelectList(Author, Common.LI_AUTHOR_INFO.AUTHOR_ID, Common.LI_AUTHOR_INFO.FIRST_NAME);
                    }

                    // Details For Edit ,...
                    if (!string.IsNullOrEmpty(sBookId))
                    {
                        var liFetchBookDetails = (List<LI_BOOK_CIRCULATION>)CMSHandler.FetchData<LI_BOOK_CIRCULATION>(new LI_BOOK_DETAILS() { BOOK_ID = sBookId },
                                LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchArticlesById), sAcademicyear).DataSource.Data;
                        if (liFetchBookDetails != null && liFetchBookDetails.Count > 0)
                        {
                            //ObjViewModel.ObjBookDetails.BOOK_ID = liFetchBookDetails.FirstOrDefault().BOOK_ID;
                            //ObjViewModel.ObjBookDetails.BOOK_NO = liFetchBookDetails.FirstOrDefault().BOOK_NO;
                            //ObjViewModel.ObjBookDetails.AUTHOR_ID = liFetchBookDetails.FirstOrDefault().AUTHOR_ID;
                            //ObjViewModel.ObjBookDetails.RACK_ID = liFetchBookDetails.FirstOrDefault().RACK_ID;
                            //ObjViewModel.ObjBookDetails.BOOK_TITLE = liFetchBookDetails.FirstOrDefault().BOOK_TITLE;
                            //ObjViewModel.ObjBookDetails.SERIESCODE = liFetchBookDetails.FirstOrDefault().SERIESCODE;
                            //ObjViewModel.ObjBookDetails.SERIES = liFetchBookDetails.FirstOrDefault().SERIES;
                            //ObjViewModel.ObjBookDetails.BOOK_PUBLISH_YEAR = liFetchBookDetails.FirstOrDefault().BOOK_PUBLISH_YEAR;
                            //ObjViewModel.ObjBookDetails.ARTICLE_TYPE = liFetchBookDetails.FirstOrDefault().ARTICLE_TYPE;
                            //ObjViewModel.ObjBookDetails.PAGES = liFetchBookDetails.FirstOrDefault().PAGES;
                            //ObjViewModel.ObjBookDetails.ABSTRACT = liFetchBookDetails.FirstOrDefault().ABSTRACT;
                            //ObjViewModel.ObjBookDetails.RFID = liFetchBookDetails.FirstOrDefault().RFID;

                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "AddArticleDetails", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult SaveArticleDetails(string sJsonArticleDetails)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<LI_BOOK_DETAILS>(sJsonArticleDetails);
                    if (!string.IsNullOrEmpty(Result.BOOK_ID))
                    {
                        var liUpdateArticleDetails = CMSHandler.SaveOrUpdate(Result, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateArticleDetailsById), sAcademicyear);
                        if (liUpdateArticleDetails.Success)
                        {
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            ObjJsonData.sResult = Result.BOOK_ID;
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                    else
                    {
                        var Exist = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { RFID = Result.RFID }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByRfid), sAcademicyear).DataSource.Data;
                        if (Exist != null && Exist.Count > 0)
                        {
                            ObjJsonData.Message = "Please Change RFID Its Already Existed";
                            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                        }


                        var liSaveArticleDetails = CMSHandler.SaveOrUpdate(Result, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveArticleDetails), sAcademicyear, true);
                        if (liSaveArticleDetails.Success)
                        {
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            ObjJsonData.sResult = liSaveArticleDetails.RowUniqueId.ToString();
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveArticleDetails", ex.Message);
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

        public JsonResult DeleteArticleDetails(string sBookId)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_DETAILS() { BOOK_ID = sBookId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.DeleteArticleById), sAcademicyear);
                    ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "DeleteArticleDetails", ex.Message);
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

        #region Library Admin new Layout
        public ActionResult AdminIndex()
        {
            return View();
        }
        public ActionResult CirulationIndex()
        {
            return RedirectToAction("AdminIndex");
        }
        public string FetchAcademicYear()
        {
            string sAcademicYear = string.Empty;
            var Academic = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchActiveYear)).DataSource.Data;
            if (Academic != null && Academic.Count > 0)
            {
                sAcademicYear = Academic[0].AC_YEAR;
            }

            return sAcademicYear;

        }
        public ActionResult BindIssue(string sUserId)
        {

            string sAcademicyear = FetchAcademicYear();
            //var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LibraryViewModel();
            var result = new List<LI_USER_DETAILS>();
            var UserDetails = new List<LI_LOGIN_FIELDS>();
            var liDetails = new List<LI_USER_DETAILS>();
            if (!string.IsNullOrEmpty(sAcademicyear) && !string.IsNullOrEmpty(sUserId))
            {
                try
                {
                    //fetch library status
                    var Status = (List<SUP_LI_BOOK_STATUS>)CMSHandler.FetchData<SUP_LI_BOOK_STATUS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStatusDDL), sAcademicyear).DataSource.Data;
                    // here unwanted status
                    Status.RemoveAll(x => x.STATUS_ID == Common.Sup_li_Status.Missing || x.STATUS_ID == Common.Sup_li_Status.Damaged || x.STATUS_ID == Common.Sup_li_Status.Reserved || x.STATUS_ID == Common.Sup_li_Status.Retrun);
                    if (Status != null && Status.Count > 0)
                    {
                        ObjViewModel.sStatusDDL = new SelectList(Status, Common.SUP_LI_BOOK_STATUS.STATUS_ID, Common.SUP_LI_BOOK_STATUS.STATUS_NAME);
                    }

                    //check user by rfid
                    UserDetails = (List<LI_LOGIN_FIELDS>)CMSHandler.FetchData<LI_LOGIN_FIELDS>(new LI_USER_DETAILS() { RFID = sUserId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserDetailsByRfid), sAcademicyear).DataSource.Data;
                    if (UserDetails != null && UserDetails.Count > 0)
                    {
                        //check user expired or not
                        //liDetails = ValidateMembership(result.BOID, result.RFID);

                        result = CheckUserExpiredorNot(UserDetails.FirstOrDefault().USER_ID);
                        if (result != null && result.Count > 0)
                        {
                            ObjViewModel.Message = "You are expired!";

                            return View(ObjViewModel);
                        }
                        else
                        {
                            var ObjbookCirculation = (List<LI_BOOK_CIRCULATION>)CMSHandler.FetchData<LI_BOOK_CIRCULATION>(null, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiBookForCirculation), sAcademicyear).DataSource.Data;
                            ObjViewModel.liBookCirculation = ObjbookCirculation.Where(x => x.BOOK_ID != null && x.BOOK_ID != "").ToList();
                            ObjViewModel.objLiUserDetails.USER_ID = UserDetails[0].USER_ID;
                            ObjViewModel.objLiUserDetails.RFID = UserDetails[0].RFID;
                        }
                    }
                    else
                    {
                        //fetch UserDetails by Boid
                        UserDetails = (List<LI_LOGIN_FIELDS>)CMSHandler.FetchData<LI_LOGIN_FIELDS>(new LI_USER_DETAILS() { BOID = sUserId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserDetailsByBoidId), sAcademicyear).DataSource.Data;
                        if (UserDetails != null && UserDetails.Count > 0)
                        {
                            //check user expired or not
                            result = CheckUserExpiredorNot(UserDetails.FirstOrDefault().USER_ID);
                            if (result != null && result.Count > 0)
                            {
                                ObjViewModel.Message = "You are expired!";
                                return View(ObjViewModel);
                            }
                            else
                            {
                                var ObjbookCirculation = (List<LI_BOOK_CIRCULATION>)CMSHandler.FetchData<LI_BOOK_CIRCULATION>(null, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiBookForCirculation), sAcademicyear).DataSource.Data;
                                ObjViewModel.liBookCirculation = ObjbookCirculation.Where(x => x.BOOK_ID != null && x.BOOK_ID != "").ToList();
                                ObjViewModel.objLiUserDetails.USER_ID = UserDetails[0].USER_ID;
                                ObjViewModel.objLiUserDetails.BOID = UserDetails[0].BOID;
                            }
                        }
                        else
                        {
                            ObjViewModel.Message = Common.Messages.NoRecordsfound;
                            return View(ObjViewModel);
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "BindIssue", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindReturn(string sUserId)
        {
            string sAcademicyear = FetchAcademicYear();
            // var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LibraryViewModel();
            var result = new List<LI_USER_DETAILS>();
            var UserDetails = new List<LI_LOGIN_FIELDS>();
            if (!string.IsNullOrEmpty(sAcademicyear) && !string.IsNullOrEmpty(sUserId))
            {
                try
                {
                    //fetch library status
                    var Status = (List<SUP_LI_BOOK_STATUS>)CMSHandler.FetchData<SUP_LI_BOOK_STATUS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStatusDDL), sAcademicyear).DataSource.Data;
                    // here unwanted status
                    Status.RemoveAll(x => x.STATUS_ID == Common.Sup_li_Status.Issue || x.STATUS_ID == Common.Sup_li_Status.Reserved || x.STATUS_ID == Common.Sup_li_Status.Reading);
                    if (Status != null && Status.Count > 0)
                    {
                        ObjViewModel.sStatusDDL = new SelectList(Status, Common.SUP_LI_BOOK_STATUS.STATUS_ID, Common.SUP_LI_BOOK_STATUS.STATUS_NAME);
                    }

                    //check user by rfid
                    UserDetails = (List<LI_LOGIN_FIELDS>)CMSHandler.FetchData<LI_LOGIN_FIELDS>(new LI_USER_DETAILS() { RFID = sUserId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserDetailsByRfid), sAcademicyear).DataSource.Data;
                    if (UserDetails != null && UserDetails.Count > 0)
                    {
                        //check user expired or not
                        result = CheckUserExpiredorNot(UserDetails.FirstOrDefault().USER_ID);
                        if (result != null && result.Count > 0)
                        {
                            ObjViewModel.Message = "You are expired!";
                            return View(ObjViewModel);
                        }
                        else
                        {
                            var liBookList = (List<LI_BOOK_CIRCULATION>)CMSHandler.FetchData<LI_BOOK_CIRCULATION>(new LI_BOOK_ISSUE() { USER_ID = UserDetails[0].USER_ID },
                               LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLibookForReturn), sAcademicyear).DataSource.Data;
                            if (liBookList != null && liBookList.Count > 0)
                            {
                                ObjViewModel.liBookCirculation = liBookList.Where(x => x.BOOK_ID != null && x.BOOK_ID != "").ToList();
                                ObjViewModel.objLiUserDetails.USER_ID = UserDetails[0].USER_ID;
                                ObjViewModel.objLiUserDetails.RFID = UserDetails[0].RFID;
                            }
                        }
                    }
                    else
                    {
                        //fetch UserDetails by Boid
                        UserDetails = (List<LI_LOGIN_FIELDS>)CMSHandler.FetchData<LI_LOGIN_FIELDS>(new LI_USER_DETAILS() { BOID = sUserId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserDetailsByBoidId), sAcademicyear).DataSource.Data;
                        if (UserDetails != null && UserDetails.Count > 0)
                        {
                            //check user expired or not
                            result = CheckUserExpiredorNot(UserDetails.FirstOrDefault().USER_ID);
                            if (result != null && result.Count > 0)
                            {
                                ObjViewModel.Message = "You are expired!";
                                return View(ObjViewModel);
                            }
                            else
                            {
                                var liBookList = (List<LI_BOOK_CIRCULATION>)CMSHandler.FetchData<LI_BOOK_CIRCULATION>(new LI_BOOK_ISSUE() { USER_ID = UserDetails[0].USER_ID },
                               LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLibookForReturn), sAcademicyear).DataSource.Data;
                                if (liBookList != null && liBookList.Count > 0)
                                {
                                    ObjViewModel.liBookCirculation = liBookList.Where(x => x.BOOK_ID != null && x.BOOK_ID != "").ToList();
                                    ObjViewModel.objLiUserDetails.USER_ID = UserDetails[0].USER_ID;
                                    ObjViewModel.objLiUserDetails.BOID = UserDetails[0].BOID;
                                }
                            }
                        }
                        else
                        {
                            ObjViewModel.Message = Common.Messages.NoRecordsfound;
                            return View(ObjViewModel);
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "BindIssue", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        /// <summary>
        /// save book issue circulation
        /// </summary>
        /// <param name="sJsonBookIssueDetails"></param>
        /// <returns></returns>
        public JsonResult SaveBookIssueCirculation(string sJsonBookIssueDetails)
        {
            string sAcadenmicYear = FetchAcademicYear();
            //  string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcadenmicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Save_Json_List>(sJsonBookIssueDetails);
                    if (Result.liBookIssue != null && Result.liBookIssue.Count > 0)
                    {
                        foreach (var item in Result.liBookIssue)
                        {
                            // item.ISSUE_BY = sUserId;
                            if (item.STATUS == Common.Sup_li_Status.Issue)
                            {
                                var SaveBookIssue = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveBookIssue), sAcadenmicYear);
                                if (SaveBookIssue.Success)
                                {
                                    var UpdateBookDetailsStatus = CMSHandler.SaveOrUpdate(new LI_BOOK_DETAILS() { BOOK_ID = item.BOOK_ID, BOOK_STATUS = item.STATUS }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookInfoLiBookStatus), sAcadenmicYear);
                                    //if (UpdateBookDetailsStatus.Success)
                                    //{
                                    //    var UpdateBookReservesStatus = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookReserveStatusById), sAcadenmicYear);
                                    //    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    //    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                    //}
                                    ObjJsonData.ErrorCode = UpdateBookDetailsStatus.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    ObjJsonData.Message = UpdateBookDetailsStatus.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                            if (item.STATUS == Common.Sup_li_Status.Reading)
                            {
                                var SaveBookReading = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveBookReading), sAcadenmicYear);
                                if (SaveBookReading.Success)
                                {
                                    var UpdateBookDetailsStatus = CMSHandler.SaveOrUpdate(new LI_BOOK_DETAILS() { BOOK_ID = item.BOOK_ID, STATUS = item.STATUS }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookInfoLiBookStatus), sAcadenmicYear);
                                    //if (UpdateBookDetailsStatus.Success)
                                    //{
                                    //    var UpdateBookReservesStatus = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookReserveStatusById), sAcadenmicYear);
                                    //    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    //    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                    //}
                                    ObjJsonData.ErrorCode = UpdateBookDetailsStatus.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                    ObjJsonData.Message = UpdateBookDetailsStatus.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }


                    }
                    else
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        ObjJsonData.Message = "No Changes Made !";
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveBookReservation", ex.Message);
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

        #region Opac
        public ActionResult Library()
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserType = (Session[Common.SESSION_VARIABLES.USER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LibraryViewModel();
            var ObjModel = new LI_BOOK_RESERVE();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    ObjModel.STUDENT_ID = sUserId;
                    ObjModel.USER_TYPE = sUserType;
                    ObjModel.USER_ID = sUserId;

                    //var Expired = CheckUserExpiredorNot(sUserId);
                    //if (Expired != null && Expired.Count > 0)
                    //{
                    //    ObjViewModel.Message = "You are expired";
                    //}
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    var liSearch = (List<SUP_LI_SEARCH_TEXT>)CMSHandler.FetchData<SUP_LI_SEARCH_TEXT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupSearch)).DataSource.Data;
                    if (liSearch != null && liSearch.Count > 0)
                    {
                        ObjViewModel.sSearch = new SelectList(liSearch, Common.SUP_LI_SEARCH_TEXT.FIELD_NAME, Common.SUP_LI_SEARCH_TEXT.SEARCH_TEXT);
                    }

                    ObjViewModel.liRules = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchRulesByUserId), sAcademicyear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "Library (Opac)", ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }

        public ActionResult BindBooksBySearch(string sCategory, string sSearchText)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserType = (Session[Common.SESSION_VARIABLES.USER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new LI_BOOK_RESERVE();
                    ObjModel.USER_ID = sUserId;
                    ObjModel.USER_TYPE = sUserType;
                    sSearchText = sSearchText.Replace("*", "");
                    string sSQL = LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookInfoBySearchText).Replace("?FILED", sCategory);
                    sSQL = sSQL.Replace("?SEARCH_TEXT", sSearchText);
                    ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(ObjModel, sSQL, sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "BindBooksBySearch", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult SaveBookReservationByUser(string sJson)
        {
            string sAcadenmicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sRole = (Session[Common.SESSION_VARIABLES.USER_ROLE_NAME] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_NAME].ToString() : string.Empty;
            string sUserType = (Session[Common.SESSION_VARIABLES.USER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            var liDetails = new List<LI_USER_DETAILS>();
            if (!string.IsNullOrEmpty(sAcadenmicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Save_Json_List>(sJson);
                    if (Result.liBookReserve != null && Result.liBookReserve.Count > 0)
                    {
                        int sCount = 0;
                        int fCount = 0;
                        foreach (var item in Result.liBookReserve)
                        {
                            item.USER_ID = sUserId;
                            if (!string.IsNullOrEmpty(item.RESERVE_ID))
                            {
                                if (item.IS_USER_CANCELLED == "1")
                                {
                                    string sMessage = ChangeBookStatus(item.BOOK_ID, Common.LI_BOOK_STATUS.CIRCULATION);
                                    if (sMessage == "200")
                                    {
                                        var SaveBookReserve = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.CancelBookReservation), sAcadenmicYear);
                                        if (SaveBookReserve.Success && SaveBookReserve.RowsAffected != 0)
                                            sCount++;
                                        else
                                            fCount++;
                                    }
                                }
                            }
                            else
                            {
                                string sMessage = ChangeBookStatus(item.BOOK_ID, Common.LI_BOOK_STATUS.RESERVE_REQUESTED);
                                if (sMessage == "200")
                                {
                                    var SaveBookReserve = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.AddBookReserve), sAcadenmicYear);
                                    if (SaveBookReserve.Success && SaveBookReserve.RowsAffected != 0)
                                        sCount++;
                                    else
                                        fCount++;
                                }
                            }
                        }
                        if (sCount != 0)
                        {
                            ObjJsonData.Message = "( " + sCount.ToString() + " ) " + Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        if (fCount != 0)
                        {
                            ObjJsonData.Message = "( " + fCount.ToString() + " ) " + Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveBookReservationByUser", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BookReservationList()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserType = (Session[Common.SESSION_VARIABLES.USER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new LI_BOOK_RESERVE();
                    ObjModel.USER_ID = sUserId;
                    ObjModel.USER_TYPE = sUserType;
                    ObjViewModel.liBookReserve = (List<LI_BOOK_RESERVE>)CMSHandler.FetchData<LI_BOOK_RESERVE>(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.BookReservationListForUserById), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "BookReservationList", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult CancelReservedBookByUser(string sReserveId, string sBookId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserType = (Session[Common.SESSION_VARIABLES.USER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new LI_BOOK_RESERVE();
                    ObjModel.USER_ID = sUserId;
                    ObjModel.USER_TYPE = sUserType;
                    ObjModel.RESERVE_ID = sReserveId;

                    string sMessage = ChangeBookStatus(sBookId, Common.LI_BOOK_STATUS.CIRCULATION);
                    if (sMessage == "200")
                    {
                        var CancelReservedBook = CMSHandler.SaveOrUpdate(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.CancelBookReservation), sAcademicYear);
                        if (CancelReservedBook.Success && CancelReservedBook.RowsAffected != 0)
                        {
                            ObjJsonData.Message = "Request Accepted ! Your Reservation Is Cancelled !";
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message = "Failed To Cancel Reservation ! Try Again !";
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "BookReservationList", ex.Message);
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
                ObjJsonData.Message = "No Changes made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteReservedBookByUser(string sReserveId, string sBookId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserType = (Session[Common.SESSION_VARIABLES.USER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new LI_BOOK_RESERVE();
                    ObjModel.USER_ID = sUserId;
                    ObjModel.RESERVE_ID = sReserveId;
                    ObjModel.IS_DELETED = Common.DeleteFlag.Deleted;

                    //string sMessage = ChangeBookStatus(sBookId,Common.LI_BOOK_STATUS.CIRCULATION);

                    var DeleteReservedBook = CMSHandler.SaveOrUpdate(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.DeleteBookReserveById), sAcademicYear);
                    if (DeleteReservedBook.Success && DeleteReservedBook.RowsAffected != 0)
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
                        ObjLog.WriteError("LibraryController", "DeleteReservedBookByUser", ex.Message);
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
                ObjJsonData.Message = "No Changes made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public string ChangeBookStatus(string sBookId, string sStatus)
        {
            string sMessage = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(sBookId) && !string.IsNullOrEmpty(sStatus))
                {
                    var ObjModel = new LI_BOOK_INFO();
                    ObjModel.LI_BOOK_STATUS = sStatus;
                    ObjModel.BOOK_ID = sBookId;
                    var ChangeBookStatus = CMSHandler.SaveOrUpdate(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookInfoLiBookStatus));
                    if (ChangeBookStatus.Success && ChangeBookStatus.RowsAffected != 0)
                    {
                        sMessage = Common.ErrorCode.Ok;
                    }
                }
                else
                {
                    sMessage = Common.ErrorCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("LibraryController", "ChangeBookStatus", ex.Message);
                }
                sMessage = Common.ErrorCode.InternalServerError;
            }
            return sMessage;
        }

        // Not In Use ....
        public ActionResult FetchBookDetailsByAuthor(string sAuthor)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sAuthor))
            {
                try
                {
                    ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { PERSONAL_AUTHOR1 = sAuthor },
                            LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByAuthor), sAcademicYear).DataSource.Data;

                    if (ObjViewModel.liBookDetails != null && ObjViewModel.liBookDetails.Count > 0)
                    {

                    }
                    else
                    {
                        ObjViewModel.Message = Common.Messages.NoRecordsfound;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchTotalBookForIssue", ex.Message);
                    }
                }
            }
            else
            {
                ObjViewModel.Message = Common.Messages.NoRecordsfound;
            }
            return View(ObjViewModel);
        }
        public ActionResult FetchBookDetailsBydocTitle(string sDocTitle)
        {

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sDocTitle))
            {
                try
                {
                    string sSql = LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByDocTitle);

                    ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { DOC_TITLE = sDocTitle },
                           sSql.Replace(Common.Delimiter.QUS + Common.LI_BOOK_DETAILS.DOC_TITLE, sDocTitle), sAcademicYear).DataSource.Data;

                    if (ObjViewModel.liBookDetails != null && ObjViewModel.liBookDetails.Count > 0)
                    {

                    }
                    else
                    {
                        ObjViewModel.Message = Common.Messages.NoRecordsfound;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchTotalBookForIssue", ex.Message);
                    }
                }
            }
            else
            {
                ObjViewModel.Message = Common.Messages.NoRecordsfound;
            }
            return View(ObjViewModel);
        }
        public ActionResult FetchBookDetailsBySeries(string sSeries)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sSeries))
            {
                try
                {
                    ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { SERIES = sSeries },
                            LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsBySeries), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchTotalBookForIssue", ex.Message);
                    }
                }
            }
            else
            {
                ObjViewModel.Message = Common.Messages.NoRecordsfound;
            }
            return View(ObjViewModel);
        }

        public ActionResult FetchBookDetailsByAnywords(string sDocTitle)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sDocTitle))
            {
                try
                {
                    string sSql = LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByDocTitle);

                    ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { DOC_TITLE = sDocTitle },
                           sSql.Replace(Common.Delimiter.QUS + Common.LI_BOOK_DETAILS.DOC_TITLE, sDocTitle), sAcademicYear).DataSource.Data;

                    if (ObjViewModel.liBookDetails != null && ObjViewModel.liBookDetails.Count > 0)
                    {

                    }
                    else
                    {
                        ObjViewModel.Message = Common.Messages.NoRecordsfound;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchTotalBookForIssue", ex.Message);
                    }
                }
            }
            else
            {
                ObjViewModel.Message = Common.Messages.NoRecordsfound;
            }
            return View(ObjViewModel);
        }

        public ActionResult FetchBookDetailsByKeywords(string sKeyword)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sKeyword))
            {
                try
                {
                    ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { KEYWORD = sKeyword },
                            LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByKeywords), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchBookDetailsByKeywords", ex.Message);
                    }
                }
            }
            else
            {
                ObjViewModel.Message = Common.Messages.NoRecordsfound;
            }
            return View(ObjViewModel);
        }

        public ActionResult FetchBookDetailsByIsbn(string sScanId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sScanId))
            {
                try
                {
                    //by isbn
                    string sSql = LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByIsbnForopac);
                    ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(null,
                            sSql.Replace(Common.Delimiter.QUS + Common.LI_BOOK_DETAILS.ISBN_NO, sScanId), sAcademicYear).DataSource.Data;

                    if (ObjViewModel.liBookDetails != null && ObjViewModel.liBookDetails.Count > 0)
                    {

                    }
                    else
                    {
                        //by access no
                        ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { ACCESS_NO = sScanId },
                           LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByAccessNoforopac), sAcademicYear).DataSource.Data;
                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchBookDetailsByIsbn", ex.Message);
                    }
                }
            }
            else
            {
                ObjViewModel.Message = Common.Messages.NoRecordsfound;
            }
            return View(ObjViewModel);

        }
        public ActionResult FetchBookDetailsByDocumentType(string sDocType)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sDocType))
            {
                try
                {
                    ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { DOC_CATEGORY = sDocType },
                            LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByDocType), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchBookDetailsByDocumentType", ex.Message);
                    }
                }
            }
            else
            {
                ObjViewModel.Message = Common.Messages.NoRecordsfound;
            }
            return View(ObjViewModel);

        }

        public ActionResult FetchBookDetailsByLanguage(string sLanguage)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sLanguage))
            {
                try
                {
                    ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { LANGUAGE = sLanguage },
                            LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsBylanguage), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchBookDetailsByLanguage", ex.Message);
                    }
                }
            }
            else
            {
                ObjViewModel.Message = Common.Messages.NoRecordsfound;
            }
            return View(ObjViewModel);

        }

        public ActionResult FetchBookDetailsByPublisher(string sPublisher)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sPublisher))
            {
                try
                {
                    string sSql = LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByPublisher);

                    ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(null,
                            sSql.Replace(Common.Delimiter.QUS + Common.LI_BOOK_DETAILS.NAME_OF_PUBLISHER, sPublisher), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchBookDetailsByPublisher", ex.Message);
                    }
                }
            }
            else
            {
                ObjViewModel.Message = Common.Messages.NoRecordsfound;
            }
            return View(ObjViewModel);

        }

        public LI_USER_DETAILS GetLibUserIdByCmsUserid(string scmsUserId, string sMemberType)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string userId = string.Empty;
            var liUser = new LI_USER_DETAILS();
            if (!string.IsNullOrEmpty(scmsUserId) && !string.IsNullOrEmpty(sMemberType))
            {
                try
                {
                    switch (sMemberType)
                    {
                        case Common.USER_TYPE.STUDENT:
                            {
                                var liUserDetails = (List<LI_USER_DETAILS>)CMSHandler.FetchData<LI_USER_DETAILS>(new STU_PERSONAL_INFO() { STUDENT_ID = scmsUserId },
                                    SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLiUserIdByStudentId), sAcademicYear).DataSource.Data;
                                if (liUserDetails != null && liUserDetails.Count > 0)
                                {
                                    liUser = liUserDetails[0];
                                }

                                break;
                            }
                        case Common.USER_TYPE.TEACHING_STAFF:
                            {

                                var liUserDetails = (List<LI_USER_DETAILS>)CMSHandler.FetchData<LI_USER_DETAILS>(new STF_PERSONAL_INFO() { STAFF_ID = scmsUserId },
                                   SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLiUserIdByStaffId), sAcademicYear).DataSource.Data;
                                if (liUserDetails != null && liUserDetails.Count > 0)
                                {
                                    liUser = liUserDetails[0];
                                }
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("LibraryController", "GetLibUserIdByCmsUserid", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("LibraryController", "GetLibUserIdByCmsUserid", ex.Message);
                    }

                }

            }
            return liUser;
        }

        public ActionResult FetchBookDetailsForReserve(string sScanId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserType = (Session[Common.SESSION_VARIABLES.USER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sUserId))
                    {

                        var objUserDetails = GetLibUserIdByCmsUserid(sUserId, sUserType);
                        if (!string.IsNullOrEmpty(objUserDetails.USER_ID))
                        {
                            //check user expired or not
                            var Expired = CheckUserExpiredorNot(objUserDetails.USER_ID);
                            if (Expired != null && Expired.Count > 0)
                            {
                                ObjViewModel.Message = "You are expired";
                                return Json(ObjViewModel, JsonRequestBehavior.AllowGet);
                            }


                            //by access no
                            ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { ACCESS_NO = sScanId },
                                    LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsForReserveByAccessNo), sAcademicYear).DataSource.Data;


                            if (ObjViewModel.liBookDetails != null && ObjViewModel.liBookDetails.Count > 0)
                            {

                            }
                            else
                            {
                                //by rfid

                                ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS() { RFID = sScanId },
                                    LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsForReserveByRfid), sAcademicYear).DataSource.Data;

                            }

                        }
                        else
                        {
                            ObjViewModel.Message = "You are not a library user";
                            return Json(ObjViewModel, JsonRequestBehavior.AllowGet);
                        }

                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchBookDetailsForIssueByRfidorAccessNo", ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }
        // End ......
        [UserRights("STUDENT")]
        public ActionResult FetchBookDetailsForRenew()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserType = (Session[Common.SESSION_VARIABLES.USER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sUserId))
                    {
                        var objUserDetails = GetLibUserIdByCmsUserid(sUserId, sUserType);
                        if (!string.IsNullOrEmpty(objUserDetails.USER_ID))
                        {
                            //check user expired or not
                            var Expired = CheckUserExpiredorNot(objUserDetails.USER_ID);
                            if (Expired != null && Expired.Count > 0)
                            {
                                ObjViewModel.Message = "You are expired";
                                return Json(ObjViewModel, JsonRequestBehavior.AllowGet);
                            }
                            var liBookList = (List<ListBookIssue>)CMSHandler.FetchData<ListBookIssue>(new LI_BOOK_ISSUE() { USER_ID = objUserDetails.USER_ID },
                              LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsForRenewByUserId), sAcademicYear).DataSource.Data;
                            ObjViewModel.liBookIssueList = liBookList;
                            //
                            ObjViewModel.objLiUserDetails.USER_ID = objUserDetails.USER_ID;
                        }
                        else
                        {
                            ObjViewModel.Message = "You are not a library user";
                            return Json(ObjViewModel, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "FetchBookDetailsForRenew", ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }
        public ActionResult FetchBookInfoBySubjectWise(string subjecttype, string sAcYear, string ssacyear)
        {
            JsonResultData objResultData = new JsonResultData();
            LibraryViewModel objViewModel = new LibraryViewModel();
            var sSQL = string.Empty;
            var liBookInfo = new List<LI_REPORT_FIELDS>();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (string.IsNullOrEmpty(sAcademicYear)) sAcademicYear = FetchAcademicYear();
            try
            {
                ViewBag.selectedyear = ssacyear;
                // Fetch College Details ....
                var resCollege = CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear);
                if (resCollege != null && resCollege.DataSource != null && resCollege.DataSource.Data != null)
                {
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)resCollege.DataSource.Data;
                }

                //Access No
                sSQL = LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookInfoBySubjectid).Replace(Common.Delimiter.QUS + Common.ACDEMIC_YEAR.ACADEMIC_YEAR_ID, sAcYear ?? "").Replace(Common.Delimiter.QUS + Common.ACDEMIC_YEAR.AC_YEAR, sAcademicYear ?? "")
                .Replace(Common.Delimiter.QUS + Common.SUP_SUBJECT.SUBJECT_ID, subjecttype ?? "");
                var resBookInfo = CMSHandler.FetchData<LI_REPORT_FIELDS>(null, sSQL);
                if (resBookInfo != null && resBookInfo.DataSource != null && resBookInfo.DataSource.Data != null)
                {
                    liBookInfo = (List<LI_REPORT_FIELDS>)resBookInfo.DataSource.Data;
                }

                objViewModel.liReportFields = liBookInfo;
                string Sql = LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookInfoByAuthorForDocTitleCountSubject).Replace(Common.Delimiter.QUS + Common.LI_BOOK_DETAILS.SUBJECT_ID, subjecttype ?? "").Replace(Common.Delimiter.QUS + Common.LI_BOOK_DETAILS.ACADEMIC_YEAR, sAcYear ?? "");
                var resAuthor = CMSHandler.FetchData<LI_REPORT_FIELDS>(null, Sql);
                if (resAuthor != null && resAuthor.DataSource != null && resAuthor.DataSource.Data != null)
                {
                    objViewModel.liTotalAutherCount = (List<LI_REPORT_FIELDS>)resAuthor.DataSource.Data;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("LibraryController", "FetchBookDetailsByUniqueValue", ex.Message);
                }
            }

            if (liBookInfo == null || liBookInfo.Count == 0)
            {
                objResultData.Message = Common.Messages.NoRecordsfound;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(objViewModel);
        }
        #endregion

        #region User Provisions 

        public ActionResult UserProvitions()
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            LibraryViewModel objViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {

            }
            return View(objViewModel);
        }

        public ActionResult BindUserInfoForExtraBooks(string sScanId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sScanId))
            {
                try
                {
                    //get user details by rfid
                    var liUserDetails = new List<LI_USER_PROVISIONS>();
                    if (!string.IsNullOrEmpty(sScanId))
                    {

                        liUserDetails = (List<LI_USER_PROVISIONS>)CMSHandler.FetchData<LI_USER_PROVISIONS>(new LI_USER_PROVISIONS() { RFID = sScanId },
                        LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiUserInfoByRfidForUserProvisios), sAcademicYear).DataSource.Data;
                    }
                    //get user details by boid
                    if (!string.IsNullOrEmpty(sScanId) && liUserDetails == null)
                    {

                        liUserDetails = (List<LI_USER_PROVISIONS>)CMSHandler.FetchData<LI_USER_PROVISIONS>(new LI_USER_PROVISIONS() { BOID = sScanId },
                        LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiUserInfoByBoidForUserProvisios), sAcademicYear).DataSource.Data;

                    }
                    if (liUserDetails != null && liUserDetails.Count > 0)
                    {
                        ObjViewModel.LiUserProvisions = liUserDetails;
                        ObjViewModel.objUserProvision.PROVISION_ID = !string.IsNullOrEmpty(liUserDetails.FirstOrDefault().PROVISION_ID) ? liUserDetails.FirstOrDefault().PROVISION_ID : string.Empty;
                        return View(ObjViewModel);
                    }
                    else
                    {
                        ObjViewModel.Message = Common.Messages.NoRecordsfound;
                        return Json(ObjViewModel, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "BindUserInfoForExtraBooks", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult SaveLiUserProvisions(string sJsonLiUserProvisions)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            // string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Save_Json_List>(sJsonLiUserProvisions);
                    if (Result.liUserProvision != null && Result.liUserProvision.Count > 0)
                    {
                        foreach (var item in Result.liUserProvision)
                        {
                            if (!string.IsNullOrEmpty(item.PROVISION_ID))
                            {
                                var UpdateLiUserProvisions = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiUserProvisions), sAcademicYear);
                                ObjJsonData.ErrorCode = UpdateLiUserProvisions.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                ObjJsonData.Message = UpdateLiUserProvisions.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            }
                            else
                            {
                                //validate li user provision
                                var isExit = (List<LI_USER_PROVISIONS>)CMSHandler.FetchData<LI_USER_PROVISIONS>(new LI_USER_PROVISIONS() { USER_ID = item.USER_ID },
                                    SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiUserProvisionByUserId), sAcademicYear).DataSource.Data;
                                if (isExit != null && isExit.Count > 0)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.ExpectationFailed;
                                    ObjJsonData.Message = Common.Messages.RecordsExistAlready;
                                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                                }

                                var SaveLiUserProvisions = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveLiUserProvisions), sAcademicYear);
                                ObjJsonData.ErrorCode = SaveLiUserProvisions.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                ObjJsonData.Message = SaveLiUserProvisions.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;

                            }


                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveLiUserProvisions", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteLiUserProvisions(string sProvisionId)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && sProvisionId != null)
            {
                try
                {

                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    resultArgs = CMSHandler.SaveOrUpdate(new LI_USER_PROVISIONS() { PROVISION_ID = sProvisionId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.DeleteLiUserProvisions), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Library", "DeleteLiUserProvisions", ex.Message);
                    }
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(objResultData);
        }
        #endregion

        #region SMS
        /// <summary>
        /// send sms for due alert 
        /// </summary>
        /// <param name="liBookIssue"></param>
        public void SendSmsForDues(List<LI_BOOK_ISSUE> liBookIssue)
        {
            try
            {
                string sAcadenmicYear = FetchAcademicYear();
                List<MessageResult.sms> liSMS = new List<MessageResult.sms>();
                string sMessage = string.Empty;
                //
                if (liBookIssue != null && liBookIssue.Count > 0)
                {
                    string sSql = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserForDueSendigSms);
                    string sIssueId = string.Empty;
                    foreach (var item in liBookIssue)
                    {
                        sIssueId += item.ISSUE_ID + ",";
                    }
                    if (!string.IsNullOrEmpty(sIssueId))
                        sIssueId = sIssueId.Trim(',');
                    var DueUsers = (List<LI_SMS_FIELDS>)CMSHandler.FetchData<LI_SMS_FIELDS>(null, sSql.Replace(Common.Delimiter.QUS + Common.LI_BOOK_ISSUE.ISSUE_ID, sIssueId), sAcadenmicYear).DataSource.Data;
                    if (DueUsers != null && DueUsers.Count > 0)
                    {
                        foreach (var item in DueUsers)
                        {
                            liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = item.MESSAGE_CONTENT, custom = "0", custom1 = "0", custom2 = "0" });
                        }
                        using (CommunicationController objHandler = new CommunicationController())
                        {
                            sMessage = objHandler.LibrarySmsSending(liSMS);
                        }
                    }

                    //update is sms send in issue
                    string sSql1 = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiBookIssueForSmsSend);
                    var result = CMSHandler.SaveOrUpdate(null, sSql1.Replace(Common.Delimiter.QUS + Common.LI_BOOK_ISSUE.ISSUE_ID, sIssueId), sAcadenmicYear);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("LibraryController", "SendSmsForDues", ex.Message);
                }
            }
        }
        public void SendSmsForIsReturnConfirmed(List<string> liBookIssue)
        {
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                List<MessageResult.sms> liSMS = new List<MessageResult.sms>();
                string sMessage = string.Empty;
                //
                if (liBookIssue != null && liBookIssue.Count > 0)
                {
                    string sSql = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserForIssueReturnConfirmedSendingSms);
                    string sIssueId = string.Empty;
                    foreach (var item in liBookIssue)
                    {
                        sIssueId += item + ",";
                    }
                    if (!string.IsNullOrEmpty(sIssueId))
                        sIssueId = sIssueId.Trim(',');
                    var DueUsers = (List<LI_SMS_FIELDS>)CMSHandler.FetchData<LI_SMS_FIELDS>(null, sSql.Replace(Common.Delimiter.QUS + Common.LI_BOOK_ISSUE.ISSUE_ID, sIssueId), sAcademicYear).DataSource.Data;
                    if (DueUsers != null && DueUsers.Count > 0)
                    {
                        foreach (var item in DueUsers)
                        {
                            liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = item.MESSAGE_CONTENT, custom = "0", custom1 = "0", custom2 = "0" });
                        }
                        using (CommunicationController objHandler = new CommunicationController())
                        {
                            sMessage = objHandler.LibrarySmsSending(liSMS);
                        }
                    }

                    //update is sms_sent in issue table
                    string sSql1 = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiBookIssueForReturnedConfirmedSmsSent);
                    var result = CMSHandler.SaveOrUpdate(new LI_BOOK_ISSUE() { IS_RCSMS_SENT = Common.Delimiter.ONE }, sSql1.Replace(Common.Delimiter.QUS + Common.LI_BOOK_ISSUE.ISSUE_ID, sIssueId), sAcademicYear);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("LibraryController", "SendSmsForIsReturnConfirmed", ex.Message);
                }
            }
        }
        private void SendSmsForIsReturnDamaged(List<LI_BOOK_TRANSACTION> liBookIssue)
        {
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                List<MessageResult.sms> liSMS = new List<MessageResult.sms>();
                string sMessage = string.Empty;
                //
                if (liBookIssue != null && liBookIssue.Count > 0)
                {
                    string sSql = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserForIssueReturnDamageSendingSms);
                    string sIssueId = string.Empty;
                    foreach (var item in liBookIssue)
                    {
                        sIssueId += item.ISSUE_ID + ",";
                    }
                    if (!string.IsNullOrEmpty(sIssueId))
                        sIssueId = sIssueId.Trim(',');
                    foreach (var item in liBookIssue)
                    {

                        var DueUsers = (List<LI_SMS_FIELDS>)CMSHandler.FetchData<LI_SMS_FIELDS>(item, sSql.Replace(Common.Delimiter.QUS + "FINE", item.FINE), sAcademicYear).DataSource.Data;
                        if (DueUsers != null && DueUsers.Count > 0)
                        {
                            foreach (var item1 in DueUsers)
                            {
                                liSMS.Add(new MessageResult.sms() { to = item1.MOBILE, message = item1.MESSAGE_CONTENT, custom = "0", custom1 = "0", custom2 = "0" });
                            }
                        }
                    }
                    if (liSMS != null && liSMS.Count > 0)
                    {
                        using (CommunicationController objHandler = new CommunicationController())
                        {
                            sMessage = objHandler.LibrarySmsSending(liSMS);
                        }
                    }
                    //update is sms_sent in issue table
                    string sSql1 = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiBookIssueForReturnedDamagedSmsSent);
                    var result = CMSHandler.SaveOrUpdate(new LI_BOOK_ISSUE() { IS_DSMS_SENT = Common.Delimiter.ONE }, sSql1.Replace(Common.Delimiter.QUS + Common.LI_BOOK_ISSUE.ISSUE_ID, sIssueId), sAcademicYear);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("LibraryController", "SendSmsForIsReturnDamaged", ex.Message);
                }
            }
        }

        private void SendSmsForBookMissing(List<LI_BOOK_TRANSACTION> liBookIssue)
        {
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                List<MessageResult.sms> liSMS = new List<MessageResult.sms>();
                string sMessage = string.Empty;
                //
                if (liBookIssue != null && liBookIssue.Count > 0)
                {
                    string sSql = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserForIssueReturnMissingSendingSms);
                    string sIssueId = string.Empty;
                    foreach (var item in liBookIssue)
                    {
                        sIssueId += item.ISSUE_ID + ",";
                    }
                    if (!string.IsNullOrEmpty(sIssueId))
                        sIssueId = sIssueId.Trim(',');
                    foreach (var item in liBookIssue)
                    {
                        var DueUsers = (List<LI_SMS_FIELDS>)CMSHandler.FetchData<LI_SMS_FIELDS>(item, sSql.Replace(Common.Delimiter.QUS + "FINE", item.FINE), sAcademicYear).DataSource.Data;
                        if (DueUsers != null && DueUsers.Count > 0)
                        {
                            foreach (var item1 in DueUsers)
                            {
                                liSMS.Add(new MessageResult.sms() { to = item1.MOBILE, message = item1.MESSAGE_CONTENT, custom = "0", custom1 = "0", custom2 = "0" });
                            }
                        }
                    }
                    if (liSMS != null && liSMS.Count > 0)
                    {
                        using (CommunicationController objHandler = new CommunicationController())
                        {
                            sMessage = objHandler.LibrarySmsSending(liSMS);
                        }
                    }
                    //update is sms_sent in issue table
                    string sSql1 = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiBookIssueForMissingSmsSent);
                    var result = CMSHandler.SaveOrUpdate(new LI_BOOK_ISSUE() { IS_MSMS_SENT = Common.Delimiter.ONE }, sSql1.Replace(Common.Delimiter.QUS + Common.LI_BOOK_ISSUE.ISSUE_ID, sIssueId), sAcademicYear);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("LibraryController", "SendSmsForBookMissing", ex.Message);
                }
            }
        }

        public void SendSmsForBookReserveApproved(List<string> liBookReserve)
        {
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                List<MessageResult.sms> liSMS = new List<MessageResult.sms>();
                string sMessage = string.Empty;
                //
                if (liBookReserve != null && liBookReserve.Count > 0)
                {
                    string sSql = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserForReserveApprovedSendingSms);
                    string sIssueId = string.Empty;
                    foreach (var item in liBookReserve)
                    {
                        sIssueId += item + ",";
                    }
                    if (!string.IsNullOrEmpty(sIssueId))
                        sIssueId = sIssueId.Trim(',');
                    var DueUsers = (List<LI_SMS_FIELDS>)CMSHandler.FetchData<LI_SMS_FIELDS>(null, sSql.Replace(Common.Delimiter.QUS + Common.LI_BOOK_RESERVE.RESERVE_ID, sIssueId), sAcademicYear).DataSource.Data;
                    if (DueUsers != null && DueUsers.Count > 0)
                    {
                        foreach (var item in DueUsers)
                        {
                            liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = item.MESSAGE_CONTENT, custom = "0", custom1 = "0", custom2 = "0" });
                        }
                        using (CommunicationController objHandler = new CommunicationController())
                        {
                            sMessage = objHandler.LibrarySmsSending(liSMS);
                        }
                    }

                    //update is sms_sent in issue table
                    string sSql1 = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiBookReserveApprovedSmsSent);
                    var result = CMSHandler.SaveOrUpdate(new LI_BOOK_RESERVE() { IS_ASMS_SENT = Common.Delimiter.ONE }, sSql1.Replace(Common.Delimiter.QUS + Common.LI_BOOK_RESERVE.RESERVE_ID, sIssueId), sAcademicYear);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("LibraryController", "SendSmsForBookReserveApproved", ex.Message);
                }
            }
        }

        private void SendSmsForBookReserveCancelling(List<LI_BOOK_TRANSACTION> liBookReserve)
        {
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                List<MessageResult.sms> liSMS = new List<MessageResult.sms>();
                string sMessage = string.Empty;
                //
                if (liBookReserve != null && liBookReserve.Count > 0)
                {
                    string sSql = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserForReserveCancellingSendingSms);
                    string sIssueId = string.Empty;
                    foreach (var item in liBookReserve)
                    {
                        sIssueId += item.RESERVE_ID + ",";
                    }
                    if (!string.IsNullOrEmpty(sIssueId))
                        sIssueId = sIssueId.Trim(',');
                    foreach (var item in liBookReserve)
                    {
                        var DueUsers = (List<LI_SMS_FIELDS>)CMSHandler.FetchData<LI_SMS_FIELDS>(item, sSql.Replace(Common.Delimiter.QUS + "CANCEL_REASON", item.RESERVE_CANCEL_REASON), sAcademicYear).DataSource.Data;
                        if (DueUsers != null && DueUsers.Count > 0)
                        {
                            foreach (var item1 in DueUsers)
                            {
                                liSMS.Add(new MessageResult.sms() { to = item1.MOBILE, message = item1.MESSAGE_CONTENT, custom = "0", custom1 = "0", custom2 = "0" });
                            }
                        }
                    }
                    if (liSMS != null && liSMS.Count > 0)
                    {
                        using (CommunicationController objHandler = new CommunicationController())
                        {
                            sMessage = objHandler.LibrarySmsSending(liSMS);
                        }
                    }
                    //update is sms_sent in issue table
                    string sSql1 = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiBookReserveCancelSmsSent);
                    var result = CMSHandler.SaveOrUpdate(new LI_BOOK_RESERVE() { IS_CSMS_SENT = Common.Delimiter.ONE }, sSql1.Replace(Common.Delimiter.QUS + Common.LI_BOOK_RESERVE.RESERVE_ID, sIssueId), sAcademicYear);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("LibraryController", "SendSmsForBookReserveCancelling", ex.Message);
                }
            }
        }

        public void SendSmsForBookReserveCancellingBySystem(List<LI_BOOK_RESERVE> liBookReserve)
        {
            try
            {
                string sAcadenmicYear = FetchAcademicYear();
                List<MessageResult.sms> liSMS = new List<MessageResult.sms>();
                string sMessage = string.Empty;
                //
                if (liBookReserve != null && liBookReserve.Count > 0)
                {
                    string sSql = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserForSystemReserveCancellingSendingSms);
                    string sIssueId = string.Empty;
                    foreach (var item in liBookReserve)
                    {
                        sIssueId += item.RESERVE_ID + ",";
                    }
                    if (!string.IsNullOrEmpty(sIssueId))
                        sIssueId = sIssueId.Trim(',');
                    var DueUsers = (List<LI_SMS_FIELDS>)CMSHandler.FetchData<LI_SMS_FIELDS>(null, sSql.Replace(Common.Delimiter.QUS + Common.LI_BOOK_RESERVE.RESERVE_ID, sIssueId), sAcadenmicYear).DataSource.Data;
                    if (DueUsers != null && DueUsers.Count > 0)
                    {
                        foreach (var item in DueUsers)
                        {
                            liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = item.MESSAGE_CONTENT, custom = "0", custom1 = "0", custom2 = "0" });
                        }
                        using (CommunicationController objHandler = new CommunicationController())
                        {
                            sMessage = objHandler.LibrarySmsSending(liSMS);
                        }
                    }

                    //update is sms_sent in issue table
                    //string sSql1 = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiBookReserveApprovedSmsSent);
                    //var result = CMSHandler.SaveOrUpdate(new LI_BOOK_RESERVE() { IS_ASMS_SENT = Common.Delimiter.ONE }, sSql1.Replace(Common.Delimiter.QUS + Common.LI_BOOK_RESERVE.RESERVE_ID, sIssueId));
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("LibraryController", "SendSmsForBookReserveCancellingBySystem", ex.Message);
                }
            }
        }

        public void SendSmsForBookRenewedCancel(List<LI_BOOK_ISSUE> liBookIssue)
        {
            try
            {
                List<MessageResult.sms> liSMS = new List<MessageResult.sms>();
                string sMessage = string.Empty;
                //
                if (liBookIssue != null && liBookIssue.Count > 0)
                {
                    string sSql = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserForRenewedCancelSendingSms);
                    string sIssueId = string.Empty;
                    foreach (var item in liBookIssue)
                    {
                        sIssueId += item.ISSUE_ID + ",";
                    }
                    if (!string.IsNullOrEmpty(sIssueId))
                        sIssueId = sIssueId.Trim(',');
                    var DueUsers = (List<LI_SMS_FIELDS>)CMSHandler.FetchData<LI_SMS_FIELDS>(null, sSql.Replace(Common.Delimiter.QUS + Common.LI_BOOK_ISSUE.ISSUE_ID, sIssueId)).DataSource.Data;
                    if (DueUsers != null && DueUsers.Count > 0)
                    {
                        foreach (var item in DueUsers)
                        {
                            liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = item.MESSAGE_CONTENT, custom = "0", custom1 = "0", custom2 = "0" });
                        }
                        using (CommunicationController objHandler = new CommunicationController())
                        {
                            sMessage = objHandler.LibrarySmsSending(liSMS);
                        }
                    }

                    //update is sms send in issue
                    string sSql1 = SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiBookIssueForRenewedCancelSmsSend);
                    var result = CMSHandler.SaveOrUpdate(new LI_BOOK_ISSUE() { IS_RENEWED = "1" }, sSql1.Replace(Common.Delimiter.QUS + Common.LI_BOOK_ISSUE.ISSUE_ID, sIssueId));
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("LibraryController", "SendSmsForBookRenewedCancel", ex.Message);
                }
            }



        }


        #endregion

        #region Common
        //fetch book details by rfid

        public List<LI_BOOK_DETAILS> GetBookDetailsByRfid(string sRfid)
        {
            var Details = new List<LI_BOOK_DETAILS>();
            if (!string.IsNullOrEmpty(sRfid))
            {
                Details = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS()
                {
                    RFID = sRfid
                }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByRfid)).DataSource.Data;
            }
            return Details;
        }

        //fetch book details by Access No

        public List<LI_BOOK_DETAILS> GetBookDetailsByAccessNo(string sAccessNo)
        {
            var Details = new List<LI_BOOK_DETAILS>();
            if (!string.IsNullOrEmpty(sAccessNo))
            {
                Details = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(new LI_BOOK_DETAILS()
                {
                    ACCESS_NO = sAccessNo
                }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookDetailsByAccessNo)).DataSource.Data;
            }
            return Details;
        }

        public string GetCurrentSemesterByLiUserId(string sUserId)
        {
            string sSemester = string.Empty;
            string sAcadenmicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sUserId))
            {
                var Details = (List<ACADEMIC_SEMESTER>)CMSHandler.FetchData<ACADEMIC_SEMESTER>(new LI_USER_DETAILS()
                {
                    USER_ID = sUserId
                }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchAcademicSemesterByLiUserId), sAcadenmicYear).DataSource.Data;

                if (Details != null && Details.Count > 0)
                {
                    sSemester = Details.FirstOrDefault().SEMESTER;
                }
            }
            return sSemester;
        }

        /// <summary>
        /// fetch extra days for renewal book
        /// </summary>
        /// <param name="sUserId"></param>
        /// <returns></returns>
        public string GetRenewDayByUserId(string sUserId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string RenewDayId = string.Empty;
            if (!string.IsNullOrEmpty(sUserId))
            {

                //fetch renew day id by  user
                var liRenewDay = (List<SUP_LI_RENEW_DAYS>)CMSHandler.FetchData<SUP_LI_RENEW_DAYS>(new LI_USER_DETAILS() { USER_ID = sUserId }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchRenewDayIdByUserId), sAcademicYear).DataSource.Data;

                if (liRenewDay != null && liRenewDay.Count > 0)
                {
                    RenewDayId = liRenewDay.FirstOrDefault().RENEW_DAY_ID;
                }

            }
            return RenewDayId;
        }
        /// <summary>
        /// check if user exired or not
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<LI_USER_DETAILS> CheckUserExpiredorNot(string UserId)
        {
            var result = new List<LI_USER_DETAILS>();
            if (!string.IsNullOrEmpty(UserId))
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                result = (List<LI_USER_DETAILS>)CMSHandler.FetchData<LI_USER_DETAILS>(new LI_USER_DETAILS() { USER_ID = UserId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.CheckUserExpiredorNot), sAcademicYear).DataSource.Data;
            }

            return result;
        }
        /// <summary>
        /// validate user login
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<LI_LOGIN_DETAILS> ValidateUserLogin(string UserId)
        {
            var result = new List<LI_LOGIN_DETAILS>();

            if (!string.IsNullOrEmpty(UserId))
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                result = (List<LI_LOGIN_DETAILS>)CMSHandler.FetchData<LI_LOGIN_DETAILS>(new LI_LOGIN_DETAILS() { USER_ID = UserId }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.CheckUserLoggedAlready), sAcademicYear).DataSource.Data;
            }

            return result;
        }

        public List<LI_RULES> GetRulesLiUserId(string sUserId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var liRules = new List<LI_RULES>();
            if (!string.IsNullOrEmpty(sUserId))
            {

                //fetch renew day id by  user
                liRules = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(new LI_USER_DETAILS() { USER_ID = sUserId }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiRuleByLiUserId), sAcademicYear).DataSource.Data;



            }
            return liRules;
        }

        public string CheckRuleAddedorNotByMemberType(string sId, LI_EXTERNAL_USER_INFO UserDetails)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sMessage = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {


                if (UserDetails.USER_TYPE.Equals(Common.LI_MEMBER_CATEGORY.STUDENT))
                {

                    //CHECK THE LIBRARY RULE WAS ADDED OR NOT FOR THE STUDENT

                    var ExisRuleMember = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(new LI_RULES() { USER_TYPE = UserDetails.USER_TYPE }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchRuleByMembeForStudent), sAcademicYear).DataSource.Data;
                    if (ExisRuleMember == null || ExisRuleMember.Count < 0)
                    {
                        sMessage = "The Rule was not added for this member?";
                    }

                }

                if (UserDetails.USER_TYPE.Equals(Common.LI_MEMBER_CATEGORY.TEACHING_STAFF))
                {
                    //CHEK THE RULE WAS ADDED OR NOT FOR THE STAFF
                    // check rule was added or not for the membership
                    //fetch existing  member values
                    var ExisRuleMember = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(new LI_RULES() { USER_TYPE = UserDetails.USER_TYPE }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchRulesByUserType), sAcademicYear).DataSource.Data;
                    if (ExisRuleMember == null || ExisRuleMember.Count <= 0)
                    {
                        sMessage = "The Rule was not added for this member?";
                    }
                }

                if (UserDetails.USER_TYPE.Equals(Common.LI_MEMBER_CATEGORY.EXTERNAL_USER) || UserDetails.USER_TYPE.Equals(Common.LI_MEMBER_CATEGORY.OTHERS))
                {
                    //CHEK THE RULE WAS ADDED OR NOT FOR THE STAFF
                    // check rule was added or not for the membership
                    //fetch existing  member values
                    var ExisRuleMember = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(new LI_RULES() { USER_TYPE = UserDetails.USER_TYPE }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchRulesByUserType), sAcademicYear).DataSource.Data;
                    if (ExisRuleMember == null || ExisRuleMember.Count <= 0)
                    {
                        sMessage = "The Rule was not added for this member?";
                    }
                }
            }
            return sMessage;
        }

        public string CheckRulesByUserType(string sUsertype)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sMessage = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liUserType = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(new LI_RULES() { USER_TYPE = sUsertype }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchRulesByUserType), sAcademicYear).DataSource.Data;
                    if (liUserType == null || liUserType.Count <= 0)
                    {
                        sMessage = "The Rule was not added for this member?";
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "CheckRulesByUserType", ex.Message);
                    }
                    sMessage = Common.ErrorMessage.InternalServerError;
                }
            }
            return sMessage;
        }

        public string GetCurrentSemesteTyper()
        {
            string sSemesterType = string.Empty;
            string sAcadenmicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            //fetch odd or even semster
            var liSemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null,
              SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType), sAcadenmicYear).DataSource.Data;
            if (liSemesterType != null && liSemesterType.Count > 0)
            {
                var liActiveSemType = liSemesterType.Where(x => x.IS_ACTIVE == "1").ToList();
                sSemesterType = liActiveSemType.FirstOrDefault().SEMESTER_TYPE.ToString();
            }

            return sSemesterType;
        }

        #endregion



        #region  Review Issued Book By User
        /// <summary>
        /// Student Side issued book review entry page 
        /// </summary>
        /// <author>Yesudasss</author>
        /// <date>2025-06-23</date>
        /// <returns></returns>
        public ActionResult ReviewIssuedBookByUser()
        {
            LibraryViewModel ObjViewModel = new LibraryViewModel();
            LI_REPORT_FIELDS objModel = new LI_REPORT_FIELDS();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string userID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objModel.USER_ID = userID;
                    objModel.ACADEMIC_YEAR = sAcademicYear;
                    ObjViewModel.libookReview = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchIssuedBookListForReview), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("LibraryController", "ReviewIssuedBookByUser", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("LibraryController", "ReviewIssuedBookByUser", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult SaveIssueBookReView(string JsonIssueBookRemark)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    LI_REPORT_FIELDS objModel = new LI_REPORT_FIELDS();
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var objJson = serializer.Deserialize<JSONLibraryModel>(JsonIssueBookRemark);
                    if (objJson.LiJsonIssueBookRemark != null && objJson.LiJsonIssueBookRemark.Count > 0)
                    {
                        foreach (var item in objJson.LiJsonIssueBookRemark)
                        {
                            objModel.USER_ID = item.USER_ID;
                            objModel.ACADEMIC_YEAR = sAcademicYear;
                            objModel.ISSUE_ID = item.ISSUE_ID;
                            objModel.REVIEW_ID = item.REVIEW_ID;
                            objModel.REVIEW = item.REVIEW;
                            objModel.SEMESTER_TYPE = sSemesterType;
                            if (string.IsNullOrEmpty(objModel.REVIEW_ID))
                            {
                                if (string.IsNullOrEmpty(item.REVIEW))
                                    continue;
                                resultArgs = CMSHandler.SaveOrUpdate(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveLiUserReviewInfo), sAcademicYear);
                                if (resultArgs.Success)
                                {
                                    var reviewID = resultArgs.RowUniqueId;
                                    //var fetchEsSettings = 
                                }
                            }
                            else
                            {
                                resultArgs = CMSHandler.SaveOrUpdate(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateLiUserReviewInfo), sAcademicYear);
                            }
                        }
                        ObjJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
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
                        objHandler.WriteError("LibraryController", "SaveIssueBookReView", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("LibraryController", "SaveIssueBookReView", ex.Message);
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


        #endregion

        #region Studnet issued Writing Component mark entry page 
        /// <summary>
        /// Extensive Writing Assessment: Writing Component Mark Entry Page Development
        /// </summary>
        /// <author>Yesudasss</author>
        /// <date>2025-06-25</date>
        /// <returns></returns>
        public ActionResult ListStuIssuedBookRemark()
        {
            StaffViewModel ObjViewModel = new StaffViewModel();
            STU_COURSE_REMARK objModel = new STU_COURSE_REMARK();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    var ClassList = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchClassListERA), sAcademicYear).DataSource.Data;
                    if (ClassList != null && ClassList.Count > 0)
                    {
                        ObjViewModel.liLCasslist = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StaffController", "ListStuCourseRemark", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StaffController", "ListStuCourseRemark", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public async Task<string> GetCourseListByClassIdAndStaffId(string sClassId)
        {
            string sOption = string.Empty;
            DataTable dtCourseList = new DataTable();
            DataTable dtClassList = new DataTable();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                using (ExaminationViewModel objExamination = new ExaminationViewModel())
                {
                    try
                    {
                        //objExamination.dv.Clear();
                        //objExamination.dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.STAFF_ID, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);
                        //objExamination.dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.CLASS_ID, sClassId, EnumCommand.DataType.String);
                        //dtCourseList = await Task.Run(() => objExamination.FetchCourseList((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table);
                        var liCourseList = await Task.Run(() => (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new CourseWiseStaffInfo() { CLASS_ID = sClassId, STAFF_ID = sUserId },
                            LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchCourseListERA), sAcademicYear).DataSource.Data);
                        if (liCourseList != null && liCourseList.Count > 0)
                        {
                            var liCiaBlockList = await Task.Run(() => (List<CIA_BLOCK_LIST>)CMSHandler.FetchData<CIA_BLOCK_LIST>(null,
                                SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCIABlockList), sAcademicYear).DataSource.Data);

                            foreach (var item in liCourseList)
                            {
                                if (liCiaBlockList != null && liCiaBlockList.Count > 0)
                                {
                                    // CourseWise Cia Block List ...
                                    var liCourseWise = liCiaBlockList.Where(x => x.S_COURSE_ID == item.COURSE_ID).ToList();
                                    if (liCourseWise != null && liCourseWise.Count > 0)
                                    {
                                        item.CIA_DISABLE = "disabled";
                                    }
                                    else
                                    {
                                        // Staff And Class And Course Wise Cia Block List ...
                                        var liStaffAndClassWise = liCiaBlockList.Where(x => x.STAFF_ID == sUserId && x.CLASS_ID == sClassId && x.COURSE_ID == item.COURSE_ID).ToList();
                                        if (liStaffAndClassWise != null && liStaffAndClassWise.Count > 0)
                                        {
                                            item.CIA_DISABLE = "disabled";
                                        }
                                    }
                                } // End If Cia Block List ....

                                if (!string.IsNullOrEmpty(item.CIA_DISABLE))
                                    sOption += "<option value=" + item.COURSE_ID + " " + item.CIA_DISABLE + " data-icon='fa fa-ban' class='text-danger'>" + item.COURSE_TITLE + "</option>";
                                else
                                    sOption += "<option value=" + item.COURSE_ID + " " + item.CIA_DISABLE + " >" + item.COURSE_TITLE + " </option>";

                            }// End Loop ....
                        }// End If Course List ....
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("ExaminationController", "GetCourseListByClassIdAndStaffId", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("ExaminationController", "GetCourseListByClassIdAndStaffId", ex.Message);
                        }

                    }
                }
            }
            else
            {
                sOption = "204";

            }
            return sOption;
        }

        public JsonResult FetchSupCIAComponentbyWritingCourseId(string sCourseId)
        {
            string sOption = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var FetchProgramme = (List<CIA_COMPONENT>)CMSHandler.FetchData<CIA_COMPONENT>(new CIA_TOTAL { COURSE_ID = sCourseId }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchSupCIAComponentbyCourseIdAndERASetting), sAcademicYear).DataSource.Data;
                    if (FetchProgramme != null && FetchProgramme.Count > 0)
                    {
                        foreach (var item in FetchProgramme)
                        {
                            sOption += "<option value='" + item.COMPONENT_ID + "' >" + item.COMPONENT + "</option>";
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
                    objHandler.WriteError("FetchSupCIAComponentbyCourseId", "Library", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchSupCIAComponentbyCourseId", "Library", ex.Message);
                }
            }
            return Json(sOption);
        }
        public ActionResult FetchStuBookReviewByCourseID(string sCourseId, string sClassId, string sComponent)
        {
            LibraryViewModel ObjViewModel = new LibraryViewModel();
            LI_REPORT_FIELDS objModel = new LI_REPORT_FIELDS();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    objModel.COURSE_ID = sCourseId;
                    objModel.CLASS_ID = sClassId;
                    objModel.COMPONENT_ID = sComponent;
                    var listudentList = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchERAStudentListForMarkEntry), sAcademicYear).DataSource.Data;

                    var listudentReview = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_REPORT_FIELDS() { CLASS_ID = sClassId, COURSE_ID = sCourseId, COMPONENT_ID = sComponent }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchReviewedStudentList), sAcademicYear).DataSource.Data;
                    List<LI_REPORT_FIELDS> liReview = new List<LI_REPORT_FIELDS>();

                    if (listudentList != null && listudentList.Count > 0)
                    {

                        foreach (var stu in listudentList)
                        {
                            if (listudentReview != null && listudentReview.Count > 0)
                            {
                                var IsExist = listudentReview.Where(o => o.STUDENT_ID == stu.STUDENT_ID).ToList();

                                if (IsExist != null && IsExist.Count > 0)
                                {
                                    liReview.Add(new LI_REPORT_FIELDS()
                                    {
                                        STUDENT_ID = stu.STUDENT_ID,
                                        STUDENT_NAME = stu.STUDENT_NAME,
                                        ROLL_NO = stu.ROLL_NO,
                                        COURSE_ID = stu.COURSE_ID,
                                        MAX_MARK = stu.MAX_MARK,
                                        SEC_MARK = IsExist.FirstOrDefault().COURSE_GROUP_MARK,
                                        SUBMITED = "1",
                                        COMPONENT = stu.COMPONENT,
                                        COMPONENT_ID = stu.COMPONENT_ID,
                                        COUNT = IsExist.FirstOrDefault().COUNT,

                                    });
                                }
                                else
                                {
                                    liReview.Add(new LI_REPORT_FIELDS()
                                    {
                                        STUDENT_ID = stu.STUDENT_ID,
                                        STUDENT_NAME = stu.STUDENT_NAME,
                                        ROLL_NO = stu.ROLL_NO,
                                        COURSE_ID = stu.COURSE_ID,
                                        MAX_MARK = stu.MAX_MARK,
                                        SEC_MARK = stu.COURSE_GROUP_MARK,
                                        SUBMITED = "0",
                                        COMPONENT = stu.COMPONENT,
                                        COMPONENT_ID = stu.COMPONENT_ID,
                                        COUNT = "0",
                                    });
                                }
                            }
                            else
                            {
                                liReview.Add(new LI_REPORT_FIELDS()
                                {
                                    STUDENT_ID = stu.STUDENT_ID,
                                    STUDENT_NAME = stu.STUDENT_NAME,
                                    ROLL_NO = stu.ROLL_NO,
                                    COURSE_ID = stu.COURSE_ID,
                                    MAX_MARK = stu.MAX_MARK,
                                    SEC_MARK = stu.COURSE_GROUP_MARK,
                                    SUBMITED = "0",
                                    COMPONENT = stu.COMPONENT,
                                    COMPONENT_ID = stu.COMPONENT_ID,
                                    COUNT = "0",
                                });
                            }
                            ObjViewModel.liStudentDetails = liReview;
                        }

                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StaffController", "ListStuCourseRemark", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StaffController", "ListStuCourseRemark", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchStuReviewSubmission(string sStusubid, string sComponentID, string sCourseID)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liFetchstuAssignment = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_REPORT_FIELDS() { STUDENT_ID = sStusubid, COMPONENT_ID = sComponentID, COURSE_ID = sCourseID, STAFF_ID = sUserId },
                    LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchReviewedListById), sAcademicYear).DataSource.Data;
                if (liFetchstuAssignment != null && liFetchstuAssignment.Count > 0)
                {
                    return Json(liFetchstuAssignment, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveStuReviewMark(string sJsonData, string sReviewID, string sERASeting, string sStudentID, string sGroupComponetId, string sComponentID, string sCourseID)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string userID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    LI_REPORT_FIELDS objModel = new LI_REPORT_FIELDS();
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                    List<LI_REPORT_FIELDS> objJson = serializer.Deserialize<List<LI_REPORT_FIELDS>>(sJsonData);

                    if (objJson != null && objJson.Count > 0)
                    {
                        foreach (var item in objJson)
                        {
                            objModel.COURSE_ID = sCourseID;
                            objModel.STAFF_ID = userID;
                            objModel.ENTRY_ID = userID;
                            objModel.STUDENT_ID = sStudentID;
                            objModel.ACADEMIC_YEAR = sAcademicYear;
                            objModel.REVIEW_ID = item.REVIEW_ID;
                            objModel.COURSE_GROUP_MARK = item.SEC_MARK;
                            objModel.COMPONENT_ID = sComponentID;
                            objModel.ERA_SETTING_ID = sERASeting;
                            objModel.COURSE_GROUP_ID = sGroupComponetId;
                            objModel.ERA_COMPONENT_ID = item.ERA_COMPONENT_ID;
                            if (string.IsNullOrEmpty(objModel.ERA_COMPONENT_ID))
                            {

                                if (!string.IsNullOrEmpty(item.SEC_MARK) && item.SEC_MARK != "0")
                                {
                                    var fetchERAMarkCourseWise = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_REPORT_FIELDS() { STUDENT_ID = sStudentID, REVIEW_ID = item.REVIEW_ID, COURSE_ID = sCourseID, COURSE_GROUP_ID = objModel.COURSE_GROUP_ID },
                                                LibrarySQL.GetLibrarySQL(LibrarySQLCommands.CheckComponentAlreadyExistorNot), sAcademicYear).DataSource.Data;
                                    if (fetchERAMarkCourseWise != null && fetchERAMarkCourseWise.Count > 0)
                                    {
                                        objModel.ERA_COMPONENT_ID = fetchERAMarkCourseWise.FirstOrDefault().ERA_COMPONENT_ID;
                                        resultArgs = CMSHandler.SaveOrUpdate(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateERAMarkCourseWise), sAcademicYear);
                                    }
                                    else
                                    {
                                        resultArgs = CMSHandler.SaveOrUpdate(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveERAMarkCourseWise), sAcademicYear);

                                    }
                                }
                                else
                                {
                                    continue;

                                }
                            }
                            else
                            {
                                resultArgs = CMSHandler.SaveOrUpdate(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateERAMarkCourseWise), sAcademicYear);
                            }
                            var feERAMarkCourseWise = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchERAComponentMark), sAcademicYear).DataSource.Data;
                            if (feERAMarkCourseWise != null && feERAMarkCourseWise.Count > 0)
                            {
                                objModel.COURSE_GROUP_MARK = feERAMarkCourseWise.FirstOrDefault().COURSE_GROUP_MARK;
                                objModel.MAX_MARK = feERAMarkCourseWise.FirstOrDefault().MAX_MARK;
                                objModel.SEMESTER_ID = feERAMarkCourseWise.FirstOrDefault().SEMESTER_ID;
                                if (Convert.ToDecimal(objModel.COURSE_GROUP_MARK) <= Convert.ToDecimal(objModel.MAX_MARK))
                                {
                                    var fetchCiaGroupMark = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(objModel,
                                                                            LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchCIAGroupCMarks), sAcademicYear).DataSource.Data;
                                    if (fetchCiaGroupMark != null && fetchCiaGroupMark.Count > 0)
                                    {
                                        objModel.CIA_GROUP_MARK_ID = fetchCiaGroupMark.FirstOrDefault().CIA_GROUP_MARK_ID;
                                        resultArgs = CMSHandler.SaveOrUpdate(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateCIAGroupCMarks), sAcademicYear);

                                    }
                                    else
                                    {
                                        resultArgs = CMSHandler.SaveOrUpdate(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveCIAGroupCMarks), sAcademicYear);

                                    }
                                }
                            }
                            objModel.ERA_COMPONENT_ID = "";
                        }

                        if (resultArgs.Success)
                        {
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
                        ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("STAFF", "SaveStuCourseRemark", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("STAFF", "SaveStuCourseRemark", ex.Message);
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
        #endregion

        #region Student Extensive Reading Component mark entry page 
        /// <summary>
        /// Extensive Writing Assessment: Writing Component Mark Entry Page Development
        /// </summary>
        /// <author>Yesudasss</author>
        /// <date>2025-06-26</date>
        /// <returns></returns>
        public ActionResult ListStuIssuedBookforReading()
        {
            StaffViewModel ObjViewModel = new StaffViewModel();
            STU_COURSE_REMARK objModel = new STU_COURSE_REMARK();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    var ClassList = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(objModel, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchClassList), sAcademicYear).DataSource.Data;
                    if (ClassList != null && ClassList.Count > 0)
                    {
                        ObjViewModel.liLCasslist = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StaffController", "ListStuCourseRemark", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StaffController", "ListStuCourseRemark", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public async Task<string> GetCourseListByClassIdAndStaffIdByERA(string sClassId)
        {
            string sOption = string.Empty;
            DataTable dtCourseList = new DataTable();
            DataTable dtClassList = new DataTable();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                using (ExaminationViewModel objExamination = new ExaminationViewModel())
                {
                    try
                    {

                        var liCourseList = await Task.Run(() => (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new CourseWiseStaffInfo() { CLASS_ID = sClassId, STAFF_ID = sUserId },
                            LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchCourseListERA), sAcademicYear).DataSource.Data);
                        if (liCourseList != null && liCourseList.Count > 0)
                        {
                            var liCiaBlockList = await Task.Run(() => (List<CIA_BLOCK_LIST>)CMSHandler.FetchData<CIA_BLOCK_LIST>(null,
                                SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCIABlockList), sAcademicYear).DataSource.Data);

                            foreach (var item in liCourseList)
                            {
                                if (liCiaBlockList != null && liCiaBlockList.Count > 0)
                                {
                                    // CourseWise Cia Block List ...
                                    var liCourseWise = liCiaBlockList.Where(x => x.S_COURSE_ID == item.COURSE_ID).ToList();
                                    if (liCourseWise != null && liCourseWise.Count > 0)
                                    {
                                        item.CIA_DISABLE = "disabled";
                                    }
                                    else
                                    {
                                        // Staff And Class And Course Wise Cia Block List ...
                                        var liStaffAndClassWise = liCiaBlockList.Where(x => x.STAFF_ID == sUserId && x.CLASS_ID == sClassId && x.COURSE_ID == item.COURSE_ID).ToList();
                                        if (liStaffAndClassWise != null && liStaffAndClassWise.Count > 0)
                                        {
                                            item.CIA_DISABLE = "disabled";
                                        }
                                    }
                                } // End If Cia Block List ....

                                if (!string.IsNullOrEmpty(item.CIA_DISABLE))
                                    sOption += "<option value=" + item.COURSE_ID + " " + item.CIA_DISABLE + " data-icon='fa fa-ban' class='text-danger'>" + item.COURSE_TITLE + "</option>";
                                else
                                    sOption += "<option value=" + item.COURSE_ID + " " + item.CIA_DISABLE + " >" + item.COURSE_TITLE + " </option>";

                            }// End Loop ....
                        }// End If Course List ....
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("ExaminationController", "GetCourseListByClassIdAndStaffId", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("ExaminationController", "GetCourseListByClassIdAndStaffId", ex.Message);
                        }

                    }
                }
            }
            else
            {
                sOption = "204";

            }
            return sOption;
        }

        public JsonResult FetchSupCIAComponentbyReadingCourseId(string sCourseId)
        {
            string sOption = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var FetchProgramme = (List<CIA_COMPONENT>)CMSHandler.FetchData<CIA_COMPONENT>(new CIA_TOTAL { COURSE_ID = sCourseId }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchSupREADINGComponentbyCourseIdAndERASetting), sAcademicYear).DataSource.Data;
                    if (FetchProgramme != null && FetchProgramme.Count > 0)
                    {
                        foreach (var item in FetchProgramme)
                        {
                            sOption += "<option value='" + item.COMPONENT_ID + "' >" + item.COMPONENT + "</option>";
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
                    objHandler.WriteError("FetchSupCIAComponentbyCourseId", "Library", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchSupCIAComponentbyCourseId", "Library", ex.Message);
                }
            }
            return Json(sOption);
        }
        public ActionResult FetchStudentIssuedBook(string sCourseId, string sClassId, string sComponent)
        {
            LibraryViewModel ObjViewModel = new LibraryViewModel();
            LI_REPORT_FIELDS objModel = new LI_REPORT_FIELDS();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    objModel.COURSE_ID = sCourseId;
                    objModel.CLASS_ID = sClassId;
                    objModel.COMPONENT_ID = sComponent;
                    var listudentList = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchERAStudentListForMarkEntry), sAcademicYear).DataSource.Data;

                    var listudentReview = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_REPORT_FIELDS() { CLASS_ID = sClassId, COURSE_ID = sCourseId, COMPONENT_ID = sComponent }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchliIssuedBookListbyStudent), sAcademicYear).DataSource.Data;
                    List<LI_REPORT_FIELDS> liReview = new List<LI_REPORT_FIELDS>();

                    if (listudentList != null && listudentList.Count > 0)
                    {

                        foreach (var stu in listudentList)
                        {
                            if (listudentReview != null && listudentReview.Count > 0)
                            {
                                var IsExist = listudentReview.Where(o => o.STUDENT_ID == stu.STUDENT_ID).ToList();

                                if (IsExist != null && IsExist.Count > 0)
                                {
                                    liReview.Add(new LI_REPORT_FIELDS()
                                    {
                                        STUDENT_ID = stu.STUDENT_ID,
                                        STUDENT_NAME = stu.STUDENT_NAME,
                                        ROLL_NO = stu.ROLL_NO,
                                        COURSE_ID = stu.COURSE_ID,
                                        MAX_MARK = stu.MAX_MARK,
                                        SEC_MARK = IsExist.FirstOrDefault().COURSE_GROUP_MARK,
                                        SUBMITED = "1",
                                        COMPONENT = stu.COMPONENT,
                                        COMPONENT_ID = stu.COMPONENT_ID,
                                        COUNT = IsExist.FirstOrDefault().COUNT,
                                        PER_BOOK_MARK = stu.PER_BOOK_MARK
                                    });
                                }
                                else
                                {
                                    liReview.Add(new LI_REPORT_FIELDS()
                                    {
                                        STUDENT_ID = stu.STUDENT_ID,
                                        STUDENT_NAME = stu.STUDENT_NAME,
                                        ROLL_NO = stu.ROLL_NO,
                                        COURSE_ID = stu.COURSE_ID,
                                        MAX_MARK = stu.MAX_MARK,
                                        SEC_MARK = stu.COURSE_GROUP_MARK,
                                        SUBMITED = "0",
                                        COMPONENT = stu.COMPONENT,
                                        COMPONENT_ID = stu.COMPONENT_ID,
                                        COUNT = "0",
                                        PER_BOOK_MARK = stu.PER_BOOK_MARK
                                    });
                                }
                            }
                            else
                            {
                                liReview.Add(new LI_REPORT_FIELDS()
                                {
                                    STUDENT_ID = stu.STUDENT_ID,
                                    STUDENT_NAME = stu.STUDENT_NAME,
                                    ROLL_NO = stu.ROLL_NO,
                                    COURSE_ID = stu.COURSE_ID,
                                    MAX_MARK = stu.MAX_MARK,
                                    SEC_MARK = stu.COURSE_GROUP_MARK,
                                    SUBMITED = "0",
                                    COMPONENT = stu.COMPONENT,
                                    COMPONENT_ID = stu.COMPONENT_ID,
                                    COUNT = "0",
                                    PER_BOOK_MARK = stu.PER_BOOK_MARK
                                });
                            }
                            ObjViewModel.liStudentDetails = liReview;
                        }

                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StaffController", "ListStuCourseRemark", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StaffController", "ListStuCourseRemark", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchStuReadingBookDetails(string sStusubid, string sComponentID, string sCourseID)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liFetchstuAssignment = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_REPORT_FIELDS() { STUDENT_ID = sStusubid, COMPONENT_ID = sComponentID, COURSE_ID = sCourseID, STAFF_ID = sUserId },
                    LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchliIssuedBookDetailsbyStudentID), sAcademicYear).DataSource.Data;
                if (liFetchstuAssignment != null && liFetchstuAssignment.Count > 0)
                {
                    return Json(liFetchstuAssignment, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveStuCIAGroupComponentMark(string sJsonData, string sComponentID, string sCourseID)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string userID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    LI_REPORT_FIELDS objModel = new LI_REPORT_FIELDS();
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                    List<LI_REPORT_FIELDS> objJson = serializer.Deserialize<List<LI_REPORT_FIELDS>>(sJsonData);

                    if (objJson != null && objJson.Count > 0)
                    {
                        foreach (var item in objJson)
                        {
                            objModel.COURSE_ID = sCourseID;
                            objModel.STAFF_ID = userID;
                            objModel.ENTRY_ID = userID;
                            objModel.STUDENT_ID = item.STUDENT_ID;
                            objModel.ACADEMIC_YEAR = sAcademicYear;
                            objModel.REVIEW_ID = item.REVIEW_ID;
                            objModel.COURSE_GROUP_MARK = item.SEC_MARK;
                            objModel.MAX_MARK = item.MAX_MARK;
                            objModel.COMPONENT_ID = sComponentID;

                            var FetchCIAGroupIDByCourseID = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchCIAGroupComponentIDbyCourseId), sAcademicYear).DataSource.Data;
                            if (FetchCIAGroupIDByCourseID != null && FetchCIAGroupIDByCourseID.Count > 0)
                            {
                                objModel.COURSE_GROUP_ID = FetchCIAGroupIDByCourseID.FirstOrDefault().COURSE_GROUP_ID;
                                objModel.MAX_MARK = FetchCIAGroupIDByCourseID.FirstOrDefault().MAX_MARK;
                                objModel.SEMESTER_ID = FetchCIAGroupIDByCourseID.FirstOrDefault().SEMESTER_ID;
                                if (Convert.ToDecimal(objModel.COURSE_GROUP_MARK) <= Convert.ToDecimal(objModel.MAX_MARK))
                                {

                                }
                                else
                                {
                                    objModel.COURSE_GROUP_MARK = objModel.MAX_MARK;
                                }

                                var fetchCiaGroupMark = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(objModel,
                                                                        LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchCIAGroupComponentMarkbyCoursID), sAcademicYear).DataSource.Data;
                                if (fetchCiaGroupMark != null && fetchCiaGroupMark.Count > 0)
                                {
                                    objModel.CIA_GROUP_MARK_ID = fetchCiaGroupMark.FirstOrDefault().CIA_GROUP_MARK_ID;
                                    resultArgs = CMSHandler.SaveOrUpdate(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateCIAGroupCMarks), sAcademicYear);

                                }
                                else
                                {
                                    resultArgs = CMSHandler.SaveOrUpdate(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveCIAGroupCMarks), sAcademicYear);

                                }

                            }

                        }

                        if (resultArgs.Success)
                        {
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
                        ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("STAFF", "SaveStuCourseRemark", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("STAFF", "SaveStuCourseRemark", ex.Message);
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
        #endregion

        #region Student Issued Book List for Staff
        /// <summary>
        /// Student Issued Book List for Staff
        /// </summary>
        /// <author>Yesudasss</author>
        /// <date>2025-06-26</date>
        /// <returns></returns>
        public ActionResult ListStudentIssuedBook()
        {
            StaffViewModel ObjViewModel = new StaffViewModel();
            STU_COURSE_REMARK objModel = new STU_COURSE_REMARK();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    var ClassList = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchClassListbyClassCourse), sAcademicYear).DataSource.Data;
                    if (ClassList != null && ClassList.Count > 0)
                    {
                        ObjViewModel.liLCasslist = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("LibraryController", "ListStudentIssuedBook", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("LibraryController", "ListStudentIssuedBook", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult FetchStudentIssuedBookDetails(string sClassId)
        {
            LibraryViewModel ObjViewModel = new LibraryViewModel();
            LI_REPORT_FIELDS objModel = new LI_REPORT_FIELDS();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    objModel.CLASS_ID = sClassId;
                    var listudentList = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchStudentListByClassID), sAcademicYear).DataSource.Data;

                    var listudentIssuedBook = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_REPORT_FIELDS() { CLASS_ID = sClassId }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchStudentIssuedBookListByClassID), sAcademicYear).DataSource.Data;
                    List<LI_REPORT_FIELDS> liReview = new List<LI_REPORT_FIELDS>();

                    ObjViewModel.liStudentDetails = listudentList;
                    ObjViewModel.libookReview = listudentIssuedBook;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("LibraryController", "FetchStudentIssuedBookDetails", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("LibraryController", "FetchStudentIssuedBookDetails", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        #endregion

        #region LIbrary Book Collection UI  

        /// <summary>
        /// Add Book Collection 
        /// </summary>
        /// <author>Yesudasss</author>
        /// <date>2025-06-28</date>
        /// <returns></returns>

        public ActionResult AddBookCollection()
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserType = (Session[Common.SESSION_VARIABLES.USER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LibraryViewModel();
            var ObjModel = new LI_BOOK_RESERVE();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    ObjModel.STUDENT_ID = sUserId;
                    ObjModel.USER_TYPE = sUserType;
                    ObjModel.USER_ID = sUserId;

                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    var liSearch = (List<SUP_LI_SEARCH_TEXT>)CMSHandler.FetchData<SUP_LI_SEARCH_TEXT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupSearch)).DataSource.Data;
                    if (liSearch != null && liSearch.Count > 0)
                    {
                        ObjViewModel.sSearch = new SelectList(liSearch, Common.SUP_LI_SEARCH_TEXT.FIELD_NAME, Common.SUP_LI_SEARCH_TEXT.SEARCH_TEXT);
                    }
                    ObjViewModel.licollection = (List<SUP_LI_BOOK_COLLECTION>)CMSHandler.FetchData<SUP_LI_BOOK_COLLECTION>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLibookCollection)).DataSource.Data;

                    ObjViewModel.liRules = (List<LI_RULES>)CMSHandler.FetchData<LI_RULES>(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchRulesByUserId), sAcademicyear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "Library (Opac)", ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }

        public ActionResult BindBooksBySearchVal(string sCategory, string sSearchText)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserType = (Session[Common.SESSION_VARIABLES.USER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new LI_BOOK_RESERVE();
                    ObjModel.USER_ID = sUserId;
                    ObjModel.USER_TYPE = sUserType;
                    sSearchText = sSearchText.Replace("*", "");
                    string sSQL = LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchBookInfoBySearchTextforCollection).Replace("?FILED", sCategory);
                    sSQL = sSQL.Replace("?SEARCH_TEXT", sSearchText);
                    ObjViewModel.liBookDetails = (List<LI_BOOK_DETAILS>)CMSHandler.FetchData<LI_BOOK_DETAILS>(ObjModel, sSQL, sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "BindBooksBySearch", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult UpdateBookCollectionInfo(string sJson, string liBookCollectionID)
        {
            string sAcadenmicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sRole = (Session[Common.SESSION_VARIABLES.USER_ROLE_NAME] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_NAME].ToString() : string.Empty;
            string sUserType = (Session[Common.SESSION_VARIABLES.USER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            var ObjViewModel = new LibraryViewModel();
            var ObjJsonData = new JsonResultData();
            var liDetails = new List<LI_USER_DETAILS>();
            if (!string.IsNullOrEmpty(sAcadenmicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Save_Json_List>(sJson);
                    if (Result.liBookReserve != null && Result.liBookReserve.Count > 0)
                    {
                        int sCount = 0;
                        int fCount = 0;
                        foreach (var item in Result.liBookReserve)
                        {
                            item.USER_ID = sUserId;
                            item.LI_BOOK_COLLECTION_ID = liBookCollectionID;

                            var SaveBookReserve = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookCollectionbyBookId), sAcadenmicYear);
                            if (SaveBookReserve.Success && SaveBookReserve.RowsAffected != 0)
                                sCount++;
                            else
                                fCount++;
                        }
                        if (sCount != 0)
                        {
                            ObjJsonData.Message = "( " + sCount.ToString() + " ) " + Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        if (fCount != 0)
                        {
                            ObjJsonData.Message = "( " + fCount.ToString() + " ) " + Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveBookReservationByUser", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
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

        #region ERA Setting List
        /// <summary>
        /// Extensive reading and writing component setting for mark entry
        /// </summary>
        /// <author>Yesudass</author>
        /// <date>2025-07-4</date>
        /// <returns></returns>
        public ActionResult ERASettingList()
        {
            string sAcademicYear = string.Empty;
            sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    LibraryViewModel objViewModel = new LibraryViewModel();
                    var AcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcademicYear != null && AcademicYear.Count > 0)
                    {
                        objViewModel.sAcademicYearList = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var semesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
                    if (semesterType != null && semesterType.Count > 0)
                    {
                        objViewModel.SupSemesterType = new SelectList(semesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    }
                    var ciaComponent = (List<SUP_CIA_COMPONENTS>)CMSHandler.FetchData<SUP_CIA_COMPONENTS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCiaComponent)).DataSource.Data;
                    if (ciaComponent != null && ciaComponent.Count > 0)
                    {
                        objViewModel.ComponentList = new SelectList(ciaComponent, Common.SUP_CIA_COMPONENTS.COMPONENT_ID, Common.SUP_CIA_COMPONENTS.COMPONENT);
                    }
                    return View(objViewModel);
                }
                catch (Exception ex)
                {
                    string sResult = string.Empty;
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("LibraryController", "ERASettingList", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("LibraryController", "ERASettingList", ex.Message);
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
        public ActionResult BindERASettingList(string sAcademicYear, string sSemType)
        {
            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            string academicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new StudentViewModel();
            var ObjJsoneData = new JsonResultData();
            LibraryViewModel objViewModel = new LibraryViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (!string.IsNullOrEmpty(sAcademicYear) && sAcademicYear != "--select--")
                    {
                        objViewModel.EraSetting = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_REPORT_FIELDS() { ACADEMIC_YEAR = sAcademicYear, SEMESTER_TYPE = sSemType }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchERASettingYearWise), sAcademicYear).DataSource.Data;
                    }
                    else
                    {
                        objViewModel.EraSetting = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_REPORT_FIELDS() { ACADEMIC_YEAR = academicYear, SEMESTER_TYPE = sSemesterType }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchERASettingYearWise), academicYear).DataSource.Data;

                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindERASettingList", "LibraryController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindERASettingList", "LibraryController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult GetComponentByCourseCode(string sCourseCode)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sOption = string.Empty;
            var ComponentList = new List<SUP_CIA_COMPONENTS>();
            ComponentList = (List<SUP_CIA_COMPONENTS>)CMSHandler.FetchData<SUP_CIA_COMPONENTS>(new CP_COURSE_ROOT() { COURSE_CODE = sCourseCode }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchComponentByCourseCode), sAcademicYear).DataSource.Data;
            if (ComponentList != null && ComponentList.Count > 0)
            {
                foreach (var item in ComponentList)
                {
                    sOption += "<option value='" + item.COMPONENT_ID + "'>" + item.COMPONENT + "</option>";
                }
            }

            return Json(sOption);
        }
        public JsonResult UpdateActiveERASetting(string sSettingId, string sActive)
        {

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var UpateActiveSetting = CMSHandler.SaveOrUpdate(new LI_REPORT_FIELDS() { ERA_SETTING_ID = sSettingId, IS_ACTIVE = sActive },
                      LibrarySQL.GetLibrarySQL(LibrarySQLCommands.ActiveOrDeactiveERASetting), sAcademicYear);
                if (UpateActiveSetting.Success)
                {
                    ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;

            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public string CheckCourseCodeExist(string sCourseCode)
        {

            string sResult = string.Empty;
            string sAcademicYear = string.Empty;
            var AcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchActiveYear)).DataSource.Data;
            if (AcademicYear != null)
            {
                sAcademicYear = AcademicYear.FirstOrDefault().AC_YEAR;
            }

            var IsexistCourseCode = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new CP_COURSE_ROOT { COURSE_CODE = sCourseCode }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.CheckCourseCodeExistOrNots), sAcademicYear).DataSource.Data;
            if (IsexistCourseCode != null && IsexistCourseCode.Count > 0)
            {
                sResult = "Exist";
                return sResult;
            }

            return sResult;
        }
        public JsonResult FetchERASettingById(string EraSettingIds)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            try
            {
                var liERASetting = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_REPORT_FIELDS() { ERA_SETTING_ID = EraSettingIds }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchERASettingById), sAcademicYear).DataSource.Data;
                if (liERASetting != null && liERASetting.Count > 0)
                {

                    return Json(liERASetting, JsonRequestBehavior.AllowGet);
                }
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
        public JsonResult DeleteERASettingByID(string sSettingID)
        {
            JsonResultData ObjResult = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                if (!string.IsNullOrEmpty(sSettingID))
                {
                    var objModel = new LI_REPORT_FIELDS();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.ERA_SETTING_ID = sSettingID;
                    objModel.IS_DELETED = Common.DeleteFlag.Deleted;
                    var sresultArgs = CMSHandler.SaveOrUpdate(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.DeleteERASettingById), sAcademicYear);
                    ObjResult.Message = (sresultArgs.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjResult.ErrorCode = (sresultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.BadRequest;
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
        public JsonResult SaveOrUpdateERASetting(string sERASetting, string sSemesterType, string sComponentId, string sCourseCode, string IsReading, string IsWriting, string IsActive, string sBookMark)
        {
            ExaminationViewModel ObjViewModel = new ExaminationViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            string sMessage = string.Empty;

            var CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var objERASettings = new LI_REPORT_FIELDS();

                    var liCourseList = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new CP_COURSE_ROOT() { COURSE_CODE = sCourseCode }, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCourseIdbyCourseCode), sAcademicYear).DataSource.Data;

                    if (liCourseList != null && liCourseList.Count > 0)
                    {
                        var courseID = liCourseList[0].COURSE_ROOT_ID;
                        objERASettings.COURSE_ID = courseID;
                        objERASettings.COMPONENT_ID = sComponentId;
                        objERASettings.ERA_SETTING_ID = sERASetting;
                        objERASettings.PER_BOOK_MARK = sBookMark;
                        objERASettings.IS_READING = IsReading;
                        objERASettings.IS_WRITING = IsWriting;
                        objERASettings.IS_ACTIVE = IsActive;
                        objERASettings.ACADEMIC_YEAR = sAcademicYear;
                        objERASettings.SEMESTER_TYPE = sSemesterType;
                        objERASettings.ENTRY_ID = sStaffId;

                        if (!string.IsNullOrEmpty(sERASetting))
                        {

                            objERASettings.ERA_SETTING_ID = sERASetting;
                            var UpdateCIAMalpracticeList = CMSHandler.SaveOrUpdate(objERASettings, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateERASetting));

                            if (UpdateCIAMalpracticeList.Success)
                            {
                                ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            }
                            else
                            {
                                ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                                ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                            }
                        }
                        else
                        {
                            var chekcExistorNot = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(objERASettings, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.CheckSameERASettingExist), sAcademicYear).DataSource.Data;
                            if (chekcExistorNot != null && chekcExistorNot.Count > 0)
                            {
                                sMessage = "A record for the same Course and Component already exists.";
                                ObjJsonData.Message = sMessage;
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotFound;
                                return Json(ObjJsonData);
                            }
                            var saveCIAMalpracticeList = CMSHandler.SaveOrUpdate(objERASettings, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveERASetting));

                            if (saveCIAMalpracticeList.Success)
                            {
                                ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            }
                            else
                            {
                                ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                            }
                        }
                    }


                    else
                    {
                        sMessage = "The given Course Code  does not exist. Please check the Course Code.";
                        ObjJsonData.Message = sMessage;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotFound;
                        return Json(ObjJsonData);

                    }


                }
                catch (Exception ex)
                {
                    sMessage = ex.Message;


                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Examination", "SaveOrUpdateCIAMalpractice", ex.Message);
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

        #region Worksheet Links
        /// <summary>
        /// Worksheet link for effective english assignment submission 
        /// </summary>
        /// <author>Yesudass</author>
        /// <date>2025/07/15</date>
        /// <returns></returns>
        public ActionResult WorkSheetGuideline()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var objViewModel = new WebsiteViewModel();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new STU_PERSONAL_INFO();
                ObjModel.STUDENT_ID = sUserId;
                var liStudentInfo = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.fetchStudentClassAndShift), sAcademicYear).DataSource.Data;

                if (liStudentInfo != null && liStudentInfo.Count > 0)
                {
                    ViewBag.shift = liStudentInfo.FirstOrDefault().SHIFT;
                    ViewBag.classYear = liStudentInfo.FirstOrDefault().CLASS_YEAR;
                    ViewBag.classLevel = liStudentInfo.FirstOrDefault().CLASS_LEVEL;
                }
            }
            return View(objViewModel);
        }
        #endregion

        #region Student Review Book List with MarkFor Staff
        /// <summary>
        /// Student Issued Book List for Staff
        /// </summary>
        /// <author>Yesudasss</author>
        /// <date>2025-06-26</date>
        /// <returns></returns>
        public ActionResult ListStudentIssuBookWithMark()
        {
            StaffViewModel ObjViewModel = new StaffViewModel();
            STU_COURSE_REMARK objModel = new STU_COURSE_REMARK();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    var ClassList = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchClassListbyClassCourse), sAcademicYear).DataSource.Data;
                    if (ClassList != null && ClassList.Count > 0)
                    {
                        ObjViewModel.liLCasslist = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("LibraryController", "ListStudentIssuedBook", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("LibraryController", "ListStudentIssuedBook", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindStudentIssuedBookDetailsWithMark(string sClassId)
        {
            LibraryViewModel ObjViewModel = new LibraryViewModel();
            LI_REPORT_FIELDS objModel = new LI_REPORT_FIELDS();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    objModel.CLASS_ID = sClassId;
                    var listudentList = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchStudentListByClassID), sAcademicYear).DataSource.Data;

                    var listudentIssuedBook = (List<LI_REPORT_FIELDS>)CMSHandler.FetchData<LI_REPORT_FIELDS>(new LI_REPORT_FIELDS() { CLASS_ID = sClassId }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchStudentLiBookIssueAndreviewByClassID), sAcademicYear).DataSource.Data;
                    List<LI_REPORT_FIELDS> liReview = new List<LI_REPORT_FIELDS>();

                    ObjViewModel.liStudentDetails = listudentList;
                    ObjViewModel.libookReview = listudentIssuedBook;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("LibraryController", "FetchStudentIssuedBookDetails", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("LibraryController", "FetchStudentIssuedBookDetails", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        #endregion

        #region Listening Audio Files for student 
        /// <summary>
        /// listening audio files for student listening audio component in library
        /// </summary>
        /// <author>Yesudasss</author>
        /// <date>2025-07-24</date>
        /// <returns></returns>
        public ActionResult ListAudioForStudenListeningAudio()
        {
            LibraryViewModel ObjViewModel = new LibraryViewModel();

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string smester_type = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            string userId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    LISTENING_AUDIO_MODEL objModel = new LISTENING_AUDIO_MODEL();
                    objModel.ACADEMIC_YEAR = sAcademicYear;
                    objModel.SEMESTER_TYPE = smester_type;
                    objModel.STUDENT_ID = userId;
                    var liStudentInfo = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new STU_PERSONAL_INFO() { STUDENT_ID = userId }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.fetchStudentClassAndShift), sAcademicYear).DataSource.Data;
                    if (liStudentInfo != null && liStudentInfo.Count > 0)
                    {
                        ViewBag.shift = liStudentInfo.FirstOrDefault().SHIFT;
                        ViewBag.classYear = liStudentInfo.FirstOrDefault().CLASS_YEAR;
                        ViewBag.classLevel = liStudentInfo.FirstOrDefault().CLASS_LEVEL;
                        objModel.CLASS_YEAR = liStudentInfo.FirstOrDefault().CLASS_YEAR;
                    }
                    ObjViewModel.liListeningAudio = (List<LISTENING_AUDIO_MODEL>)CMSHandler.FetchData<LISTENING_AUDIO_MODEL>(objModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchListeningAudioFiles), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StaffController", "ListStuCourseRemark", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StaffController", "ListStuCourseRemark", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public async Task<ActionResult> FetchQuizQuestionByListeningId(string listeningId)
        {
            string sOption = string.Empty;
            LibraryViewModel libraryViewModel = new LibraryViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    libraryViewModel.liQuizQuestion = await Task.Run(() => (List<LISTENING_QUIZ_QESTION>)CMSHandler.FetchData<LISTENING_QUIZ_QESTION>(new LISTENING_QUIZ_QESTION() { LISTENING_ID = listeningId },
                        LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchQuizQuestionByListeningId), sAcademicYear).DataSource.Data);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ExaminationController", "GetCourseListByClassIdAndStaffId", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ExaminationController", "GetCourseListByClassIdAndStaffId", ex.Message);
                    }
                }
            }
            else
            {
                sOption = "204";
            }
            return View(libraryViewModel);
        }
        public async Task<ActionResult> FetchQuizAnswerbyListernerId(string listeningId)
        {
            string sOption = string.Empty;
            LibraryViewModel libraryViewModel = new LibraryViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                try
                {
                    libraryViewModel.liQuizQuestion = await Task.Run(() => (List<LISTENING_QUIZ_QESTION>)CMSHandler.FetchData<LISTENING_QUIZ_QESTION>(new LISTENING_QUIZ_QESTION() { LISTENING_ID = listeningId, STUDENT_ID = sUserId },
                        LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchQuizQuestionAnswerByListeningId), sAcademicYear).DataSource.Data);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ExaminationController", "GetCourseListByClassIdAndStaffId", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ExaminationController", "GetCourseListByClassIdAndStaffId", ex.Message);
                    }

                }

            }
            else
            {
                sOption = "204";

            }
            return View(libraryViewModel);
        }

        [HttpPost]
        public JsonResult SubmitQuizAnswers(List<LISTENING_QUIZ_ANSWER> answers, string listeningId)
        {
            var result = new { success = false, message = "Failed to save answers." };
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string userId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            if (answers != null && answers.Count > 0 && !string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    foreach (var ans in answers)
                    {
                        LISTENING_QUIZ_ANSWER obj = new LISTENING_QUIZ_ANSWER
                        {
                            LISTENING_ID = listeningId,
                            QUESTION_ID = ans.QUESTION_ID,
                            SELECTED_ANSWER = ans.SELECTED_ANSWER,
                            STUDENT_ID = userId,
                            ACADEMIC_YEAR = sAcademicYear
                        };

                        CMSHandler.SaveOrUpdate(obj,
                            LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveListeningQuizAnswer),
                            sAcademicYear);
                    }

                    result = new { success = true, message = "Quiz answers submitted successfully!" };
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("LibraryController", "SubmitQuizAnswers", ex.Message);
                    }
                    result = new { success = false, message = "An error occurred while saving answers." };
                }
            }
            else
            {
                result = new { success = false, message = "No answers submitted or session expired." };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region LibraryBulkChickin

        public ActionResult LibraryBulkChickin()
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
                        ObjLog.WriteError("LibraryController", "LibraryBulkChickin", ex.Message);
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
            liProgram = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(new CP_PROGRAMME_2017() { PROGRAMME_LEVEL = programmeLevel, SHIFT = sShiftId}, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupByShiftIdAndProgrammeLevel), sAcademicYear).DataSource.Data;
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


        public ActionResult BindStudentListforBulkCheckin(string sClassId, string sProgramName, string ReturnDate)
        {
            CommunicationViewModel objViewModel = new CommunicationViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sClassId) && !string.IsNullOrEmpty(sProgramName) && !string.IsNullOrEmpty(ReturnDate))
            {
                try
                {
                    objViewModel.LIStudentDetails = (List<Student_Personal_Info>)CMS.DAO.CMSHandler.FetchData<Student_Personal_Info>(new cp_Classes_2017() { PROGRAMME_GROUP_ID = sProgramName, CHECKIN_DATE= ReturnDate }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentListForBulkchickin).Replace("?CLASS_ID", sClassId), sAcademicYear).DataSource.Data;
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




        [HttpPost]
        public JsonResult SaveLibraryBulkChickin(string StudentIds, string ReturnDate)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();

          var liUserDetails = new List<USER_ACCOUNT>();
          var ObjModel = new USER_ACCOUNT();

            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try 
                {
                   if(!string.IsNullOrEmpty(StudentIds))
                   {
                       string[] Ids = StudentIds.Split(',');
                       int SuccessCount = 0;
                       int FailCount = 0;
                       DateTime parsedDate;
                       if (DateTime.TryParseExact(ReturnDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate))
                       {
                       }
                       else if (!DateTime.TryParse(ReturnDate, out parsedDate))
                       {
                           parsedDate = DateTime.Now;
                       }

                       int timeDelaySeconds = 0;
                       foreach (var StudentId in Ids)
                       {
                            if (string.IsNullOrEmpty(StudentId)) continue;

                            // ✅ Use Current Time for the timestamp part to ensure the 3s gap is real
                            ObjModel.CHECKIN_DATE = parsedDate.ToString("yyyy-MM-dd") + " " + DateTime.Now.AddSeconds(timeDelaySeconds).ToString("HH:mm:ss");
                            timeDelaySeconds += 3;
                            ObjModel.STUDENT_ID = StudentId;

                            liUserDetails = (List<USER_ACCOUNT>)CMSHandler.FetchData<USER_ACCOUNT>(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchUserCredentialByStudentLibrary), sAcademicyear).DataSource.Data;
                            if (liUserDetails != null && liUserDetails.Count > 0)
                            {
                                ObjJsonData.Message = "Dear " + liUserDetails.FirstOrDefault().FIRST_NAME + " ";
                                ObjModel.USER_ID = liUserDetails.FirstOrDefault().USER_ID;
                                ObjModel.USER_TYPE = liUserDetails.FirstOrDefault().USER_TYPE;
                                ObjModel.ROLE = liUserDetails.FirstOrDefault().ROLE;
                                ObjModel.PROGRAMME_LEVEL = liUserDetails.FirstOrDefault().PROGRAMME_LEVEL;
                                ObjModel.SEMESTER_TYPE = liUserDetails.FirstOrDefault().SEMESTER_TYPE;
                                ObjModel.SEMESTER = liUserDetails.FirstOrDefault().SEMESTER;
                                ObjModel.AC_YEAR = liUserDetails.FirstOrDefault().AC_YEAR;
                                var liStudentActiveSem = (List<ACADEMIC_SEMESTER>)CMSHandler.FetchData<ACADEMIC_SEMESTER>(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchActiveSemesterByUserId), ObjModel.AC_YEAR).DataSource.Data;
                                if (liStudentActiveSem != null && liStudentActiveSem.Count > 0)
                                {
                                    ObjModel.SEMESTER = liStudentActiveSem.FirstOrDefault().SEMESTER;
                                }
                                ObjModel.CHECKIN_ID = ValidateBulkCheckIn(ObjModel.USER_ID, ObjModel.USER_TYPE, ObjModel.AC_YEAR, ObjModel.CHECKIN_DATE);

                                if (!string.IsNullOrEmpty(ObjModel.CHECKIN_ID))
                                {
                                    // For Check Out ..,
                                    //var SaveCheckOut = CMSHandler.SaveOrUpdate(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveCheckOutEntry), ObjModel.AC_YEAR);
                                    //if (SaveCheckOut.Success && SaveCheckOut.RowsAffected != 0)
                                    //{
                                    //    ObjJsonData.Message += "Your Log Out Successfully ! :)";
                                    //    SuccessCount++;
                                    //}
                                    //else
                                    //{
                                    //    ObjJsonData.Message += " Sorry Failed To Log Out ! Try Again ! :(";
                                    //    FailCount++;
                                    //}
                                    ObjJsonData.Message += "Already checked in ! :)";
                                    FailCount++;

                                }
                                else
                                {

                                    // For Check In ..,
                                    var SaveCheckIn = CMSHandler.SaveOrUpdate(ObjModel, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveBulkCheckInEntry), ObjModel.AC_YEAR);
                                    if (SaveCheckIn.Success && SaveCheckIn.RowsAffected != 0)
                                    {
                                        ObjJsonData.Message += "Your Loged In Successfully ! :)";
                                        SuccessCount++;
                                    }
                                    else
                                    {
                                        ObjJsonData.Message += " Sorry Failed To Log In ! Try Again ! :(";
                                        FailCount++;
                                    }
                                }
                            }
                            // Fetch Student Details
                           // var StudentDetails = (List<LI_CHECKIN_INFO>)CMSHandler.FetchData<LI_CHECKIN_INFO>(new LI_CHECKIN_INFO() { USER_ID = StudentId }, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchStudentDetailsForCheckin), sAcademicyear).DataSource.Data;

                           //if(StudentDetails != null && StudentDetails.Count > 0)
                           //{
                           //    var distinctStudent = StudentDetails.FirstOrDefault();
                           //    distinctStudent.CHECKIN_ID = Guid.NewGuid().ToString();
                           //    distinctStudent.CHECKIN_DATE = ReturnDate;
                           //    distinctStudent.ACADEMIC_YEAR = sAcademicyear;

                           //    var Result = CMSHandler.SaveOrUpdate(distinctStudent, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveLibraryCheckin), sAcademicyear);
                               
                           //    if(Result.Success) SuccessCount++;
                           //    else FailCount++;
                           //}
                           //else 
                           //{
                           //    FailCount++; // Student details not found
                           //}
                            // ✅ Real-time delay of 3 seconds as requested
                            System.Threading.Thread.Sleep(3000);
                       }
                       ObjJsonData.Message = "Processed. Check-In Saved for " + SuccessCount + " Students.";
                       ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                   }
                   else
                   {
                        ObjJsonData.Message = "Please select students.";
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                   }
                }
                catch(Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LibraryController", "SaveLibraryBulkChickin", ex.Message);
                    }
                    ObjJsonData.Message = "Error: " + ex.Message;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
             else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.Unauthorized;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }

    #region Service
    //for sending due sms
    public class TaskQueue2 : Controller, IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {

                List<LI_BOOK_ISSUE> listUD = new List<LI_BOOK_ISSUE>();
                string sExactReturnDayId = string.Empty;
                string sExactReturnDate = string.Empty;
                string sCurDate = string.Empty;
                LibraryController ob = new LibraryController();
                string sAcadenmicYear = ob.FetchAcademicYear();
                ResultArgs resultArgs = new ResultArgs();
                var ObjViewModel = new LibraryViewModel();
                // List Book issue info sms send for submitting book
                ObjViewModel.liBookIssueList = (List<ListBookIssue>)CMSHandler.FetchData<ListBookIssue>(null,
                       LibrarySQL.GetLibrarySQL(LibrarySQLCommands.ListBookIssueForSendSms), sAcadenmicYear).DataSource.Data;

                if (ObjViewModel.liBookIssueList != null && ObjViewModel.liBookIssueList.Count > 0)
                {
                    //this is for fetching total calender days
                    var liFetchCalender = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchCalenderDetails), sAcadenmicYear).DataSource.Data;
                    List<CALENDER_2017> liCalender = new List<CALENDER_2017>();

                    if (liFetchCalender != null && liFetchCalender.Count > 0)
                    {
                        //filter only working days
                        liCalender = liFetchCalender.Where(x => x.IS_HOLIDAY != "1").ToList();
                    }
                    //var liDues = (List<LI_DUES>)CMSHandler.FetchData<LI_DUES>(null,
                    //         LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchDuesList), sAcadenmicYear).DataSource.Data;
                    foreach (var item in ObjViewModel.liBookIssueList)
                    {
                        //get exact returned date by load period of user from issue date
                        //here item.dayid is when they have taken issue day id
                        //here load period is the total day for book is permitted to have
                        var liDueDates = new List<CALENDER_2017>();
                        if (!string.IsNullOrEmpty(item.NO_OF_DAYS) && !string.IsNullOrEmpty(item.IS_RENEWED) && item.IS_RENEWED == Common.Delimiter.ONE)
                        {
                            liDueDates = liCalender.Where(x => Convert.ToInt32(x.DAY_ID) > Convert.ToInt32(item.DAY_ID)).Take(Convert.ToInt32(item.NO_OF_DAYS) + Convert.ToInt32(item.NO_OF_RENEW_DAYS)).ToList();
                        }
                        else
                        {
                            liDueDates = liCalender.Where(x => Convert.ToInt32(x.DAY_ID) > Convert.ToInt32(item.DAY_ID)).Take(Convert.ToInt32(item.NO_OF_DAYS)).ToList();
                        }

                        if (liDueDates != null && liDueDates.Count > 0)
                        {
                            //the exact return date
                            sExactReturnDate = liDueDates.Max(x => x.DAY_ORDER_DATE).ToString();

                            // fetch exact day id for return date
                            sExactReturnDayId = liDueDates.Max(x => x.DAY_ID).ToString();

                            //fetch current date
                            var liCurdate = liCalender.Where(x => x.IS_CURDATE == "1").ToList();

                            //get total maximum days for counting  fine apart from load period
                            var liCalFine = liCalender.Where(x => Convert.ToInt32(x.DAY_ID) >= Convert.ToInt32(sExactReturnDayId)).ToList();
                            if (liCalFine != null && liCalFine.Count > 0)
                            {
                                //fetch total fine days from book issue date to up to now
                                var liFineDates = liCalFine.Where(x => Convert.ToInt32(x.DAY_ID) <= Convert.ToInt32(liCurdate.FirstOrDefault().DAY_ID) && (Convert.ToInt32(x.DAY_ID) > Convert.ToInt32(item.DAY_ID))).ToList();
                                if (liFineDates != null && liFineDates.Count > 0)
                                {
                                    listUD.Add(new LI_BOOK_ISSUE() { ISSUE_ID = item.ISSUE_ID });
                                }
                            }
                        }
                    }
                }

                if (listUD != null && listUD.Count > 0)
                {
                    ob.SendSmsForDues(listUD);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("TaskQueue", "TaskQueue2", ex.Message);
                }
            }

            //throw new NotImplementedException(" is not yet implemented.");
        }
    }

    /// <summary>
    ///    for cancelling book reservation
    /// </summary>
    public class TaskQueue : Controller, IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                string sMaxDay = string.Empty;
                string sCurDate = string.Empty;
                LibraryController ob = new LibraryController();
                List<LI_BOOK_RESERVE> listUD = new List<LI_BOOK_RESERVE>();
                string sAcadenmicYear = ob.FetchAcademicYear();
                ResultArgs resultArgs = new ResultArgs();

                //fetch all reserved book
                var TotalUsers = (List<LI_BOOK_RESERVE>)CMSHandler.FetchData<LI_BOOK_RESERVE>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchFailedtoTakeBookUsersWithingOneday), sAcadenmicYear).DataSource.Data;

                if (TotalUsers != null && TotalUsers.Count > 0)
                {
                    foreach (var item in TotalUsers)
                    {
                        listUD.Add(new LI_BOOK_RESERVE() { RESERVE_ID = item.RESERVE_ID });

                        resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_RESERVE()
                        {
                            RESERVE_ID = item.RESERVE_ID,
                            IS_SYSTEM_CANCELLED = Common.Delimiter.ONE
                        }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateSystemReserveCancelByReserveId));
                        //book status change as circulation
                        if (resultArgs.Success && !string.IsNullOrEmpty(item.BOOK_ID))
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_INFO() { BOOK_ID = item.BOOK_ID, LI_BOOK_STATUS = Common.LI_BOOK_STATUS.CIRCULATION }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookInfoLiBookStatus), sAcadenmicYear);
                        }
                    }

                }
                if (listUD != null && listUD.Count > 0)
                {
                    ob.SendSmsForBookReserveCancellingBySystem(listUD);
                }


                //if (TotalUsers != null && TotalUsers.Count > 0)
                //{

                //    //this is for fetching total calender days
                //    var liFetchCalender = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchCalenderDetails), sAcadenmicYear).DataSource.Data;
                //    List<CALENDER_2017> liCalender = new List<CALENDER_2017>();

                //    if (liFetchCalender != null && liFetchCalender.Count > 0)
                //    {
                //        //filter only working days
                //        liCalender = liFetchCalender.Where(x => x.IS_HOLIDAY != "1").ToList();
                //    }


                //    foreach (var item in TotalUsers)
                //    {
                //        //get exact returned date by load period of user from issue date
                //        var liRemainingDates = liCalender.Where(x => Convert.ToInt32(x.DAY_ID) > Convert.ToInt32(item.DAY_ID)).Take(Convert.ToInt32(item.DAYS)).ToList();

                //        if (liRemainingDates != null && liRemainingDates.Count > 0)
                //        {
                //            item.RESERVE_ID = liRemainingDates.Max(x => x.DAY_ORDER_DATE).ToString();

                //            // fetch the max day id from this we can calculate for fine
                //            sMaxDay = liRemainingDates.Max(x => x.DAY_ID).ToString();

                //            //fetch current date
                //            var liCurdate = liCalender.Where(x => x.IS_CURDATE == "1").ToList();

                //            //get total maximum days for counting fine
                //            var liCalExtraRemainingDays = liCalender.Where(x => Convert.ToInt32(x.DAY_ID) > Convert.ToInt32(sMaxDay)).ToList();

                //            if (liCalExtraRemainingDays != null && liCalExtraRemainingDays.Count > 0 && liCurdate != null && liCurdate.Count > 0)
                //            {
                //                //fetch total fine days from book issue date to up to now
                //                var liRemainDates = liCalender.Where(x => Convert.ToInt32(x.DAY_ID) <= Convert.ToInt32(liCurdate.FirstOrDefault().DAY_ID) && (Convert.ToInt32(x.DAY_ID) > Convert.ToInt32(item.DAY_ID))).ToList();
                //                if (liRemainDates != null && liRemainDates.Count > 0)
                //                {
                //                    listUD.Add(new LI_BOOK_RESERVE() { RESERVE_ID = item.RESERVE_ID });



                //                    resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_RESERVE()
                //                    {
                //                        RESERVE_ID = item.RESERVE_ID,
                //                        IS_ACTIVE = Common.DeleteFlag.Deleted
                //                    }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.DeleteBookReserveById));
                //                    // if book reservation is cancelled and change the status in book details
                //                    if (resultArgs.Success && !string.IsNullOrEmpty(item.BOOK_ID))
                //                    {
                //                        resultArgs = CMSHandler.SaveOrUpdate(new LI_BOOK_DETAILS() { BOOK_ID = item.BOOK_ID, BOOK_STATUS = "0" }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateBookInfoLiBookStatus));
                //                    }
                //                }
                //            }
                //        }
                //    }

                //    if (listUD != null && listUD.Count > 0)
                //    {
                //      //  ob.SendSmsForBookReservedCancel(listUD);
                //    }

                //}
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("TaskQueue", "TaskQueue", ex.Message);
                }
            }

            //throw new NotImplementedException(" is not yet implemented.");
        }



        /// <summary>
        /// for adding fine
        /// </summary>
        /// 


    }


    public class TaskQueue1 : Controller, IJob
    {
        LibraryController ob = new LibraryController();
        public async Task Execute(IJobExecutionContext context)
        {
            string sAcadenmicYear = ob.FetchAcademicYear();
            ResultArgs resultArgs = new ResultArgs();
            var ObjViewModel = new LibraryViewModel();
            try
            {
                string sExactReturnDateDayId = string.Empty;
                string sCurDate = string.Empty;
                // List Book Details ,...
                ObjViewModel.liBookIssueList = (List<ListBookIssue>)CMSHandler.FetchData<ListBookIssue>(null,
                       LibrarySQL.GetLibrarySQL(LibrarySQLCommands.ListBookIssueForAddDue), sAcadenmicYear).DataSource.Data;

                if (ObjViewModel.liBookIssueList != null && ObjViewModel.liBookIssueList.Count > 0)
                {
                    //this is for fetching total calender days
                    var liFetchCalender = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchCalenderDetails), sAcadenmicYear).DataSource.Data;
                    List<CALENDER_2017> liCalender = new List<CALENDER_2017>();
                    if (liFetchCalender != null && liFetchCalender.Count > 0)
                    {
                        //filter only working days
                        liCalender = liFetchCalender.Where(x => x.IS_HOLIDAY != "1").ToList();
                    }
                    var liDues = (List<LI_ISSUE_DUES>)CMSHandler.FetchData<LI_ISSUE_DUES>(null,
                               LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchDuesList), sAcadenmicYear).DataSource.Data;
                    foreach (var item in ObjViewModel.liBookIssueList)
                    {
                        //get exact returned date by load period of user from issue date
                        var liDueDates = new List<CALENDER_2017>();
                        if (!string.IsNullOrEmpty(item.NO_OF_RENEW_DAYS) && !string.IsNullOrEmpty(item.NO_OF_DAYS) && item.IS_RENEWED == Common.Delimiter.ONE)
                        {
                            liDueDates = liCalender.Where(x => Convert.ToInt32(x.DAY_ID) > Convert.ToInt32(item.DAY_ID)).Take(Convert.ToInt32(item.NO_OF_DAYS) + Convert.ToInt32(item.NO_OF_RENEW_DAYS)).ToList();
                        }
                        else
                        {
                            liDueDates = liCalender.Where(x => Convert.ToInt32(x.DAY_ID) > Convert.ToInt32(item.DAY_ID)).Take(Convert.ToInt32(item.NO_OF_DAYS)).ToList();
                        }



                        if (liDueDates != null && liDueDates.Count > 0)
                        {
                            //the exact return date
                            item.RETURN_DATE = liDueDates.Max(x => x.DAY_ORDER_DATE).ToString();

                            // the exact retun day id
                            sExactReturnDateDayId = liDueDates.Max(x => x.DAY_ID).ToString();

                            //fetch current date
                            var liCurdate = liCalender.Where(x => x.IS_CURDATE == "1").ToList();

                            //get total maximum days for counting  fine apart from the load period
                            var liCalFine = liCalender.Where(x => Convert.ToInt32(x.DAY_ID) > Convert.ToInt32(sExactReturnDateDayId)).ToList();

                            if (liCalFine != null && liCalFine.Count > 0 && liCurdate != null && liCurdate.Count > 0)
                            {
                                //fetch total fine days from book issue date to up to now
                                var liFineDates = liCalFine.Where(x => Convert.ToInt32(x.DAY_ID) <= Convert.ToInt32(liCurdate.FirstOrDefault().DAY_ID) && (Convert.ToInt32(x.DAY_ID) > Convert.ToInt32(item.DAY_ID))).ToList();

                                if (liFineDates != null && liFineDates.Count > 0)
                                {
                                    //ckeck if it is any asd fine 
                                    if (!string.IsNullOrEmpty(item.REVISED_DUE_DATES) && item.REVISED_DUE_DATES != "0")
                                    {
                                        if (liFineDates.Count > Convert.ToInt16(item.REVISED_DUE_DATES))
                                        {
                                            //add revised due  fine
                                            item.REVISED_DUE_FINE_AMOUNT = (liFineDates.Count * Convert.ToInt32(item.REVISED_DUE_COST)).ToString();
                                            item.DUES = (liFineDates.Count * Convert.ToInt32(item.REVISED_DUE_COST)).ToString();
                                        }
                                        else
                                        {
                                            //add normal fine
                                            item.DUE_FINE_AMOUNT = (liFineDates.Count * Convert.ToInt32(item.PER_DAY_DUE_COST)).ToString();
                                            item.DUES = (liFineDates.Count * Convert.ToInt32(item.PER_DAY_DUE_COST)).ToString();
                                        }
                                    }
                                    else
                                    {
                                        //add normal fine
                                        item.DUE_FINE_AMOUNT = (liFineDates.Count * Convert.ToInt32(item.PER_DAY_DUE_COST)).ToString();
                                        item.DUES = (liFineDates.Count * Convert.ToInt32(item.PER_DAY_DUE_COST)).ToString();
                                    }
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(item.DUES))
                        {
                            if (item.DUES != "0")
                            {
                                item.REASON = "OVER DUES";
                                if (liDues != null && liDues.Count > 0)
                                {
                                    item.DAY_ORDER_DATE = item.RETURN_DATE;
                                    var IsExist = liDues.Where(x => x.ISSUE_ID == item.ISSUE_ID).ToList();
                                    if (IsExist != null && IsExist.Count > 0)
                                    {
                                        // Update If Already Exist ,...
                                        item.DUES_ID = IsExist.FirstOrDefault().DUES_ID;
                                        item.REASON = IsExist.FirstOrDefault().REASON;
                                        int fine = Convert.ToInt16(item.DUE_FINE_AMOUNT != "" ? item.DUE_FINE_AMOUNT : "0") + Convert.ToInt16(item.REVISED_DUE_FINE_AMOUNT != "" ? item.REVISED_DUE_FINE_AMOUNT : "0");
                                        item.DUES = fine.ToString();

                                        // update Library Dues ,...
                                        var UpdateDues = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateDues), sAcadenmicYear);
                                    }
                                    else
                                    {
                                        // Insert Library Dues ,...
                                        var InsertDues = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveDues), sAcadenmicYear);
                                    }
                                }
                                else
                                {
                                    // Insert Library Dues ,...
                                    var InsertDues = CMSHandler.SaveOrUpdate(item, LibrarySQL.GetLibrarySQL(LibrarySQLCommands.SaveDues), sAcadenmicYear);
                                }
                            }
                        }
                        else
                        {
                            item.DUES = "0";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("TaskQueue1", "Execute", ex.Message);
                }
            }
            //throw new NotImplementedException();
        }
    }

    /// <summary>
    /// for sytem check out at 12 pm 
    /// </summary>
    public class TaskQueue3 : Controller, IJob
    {
        LibraryController ob = new LibraryController();
        public async Task Execute(IJobExecutionContext context)
        {
            string sAcadenmicYear = ob.FetchAcademicYear();
            ResultArgs resultArgs = new ResultArgs();
            var ObjViewModel = new LibraryViewModel();
            try
            {
                string sExactReturnDateDayId = string.Empty;
                string sCurDate = string.Empty;
                // List Book Details ,...
                var liCheckInDetails = (List<LI_CHECKIN_INFO>)CMSHandler.FetchData<LI_CHECKIN_INFO>(null,
                       LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchNotCheckedoutUsersForSystemCheckout), sAcadenmicYear).DataSource.Data;

                if (liCheckInDetails != null && liCheckInDetails.Count > 0)
                {
                    foreach (var item in liCheckInDetails)
                    {
                        if (!string.IsNullOrEmpty(item.CHECKIN_ID))
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(new LI_CHECKIN_INFO()
                            {

                                CHECKIN_ID = item.CHECKIN_ID,
                                IS_SYSTEM_CHECKOUT = Common.Delimiter.ONE

                            }, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.UpdateIsSystemCheckoutByCheckInId), sAcadenmicYear);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("TaskQueue3", "Execute", ex.Message);
                }
            }
            //throw new NotImplementedException();
        }
    }
    public class JobScheduler
    {
        public static void Start()
        {
            try
            {
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();

                string sDuesHour = string.Empty;
                string sDuesMinute = string.Empty;
                string sReserveCancelHour = string.Empty;
                string sReserveCancelMinute = string.Empty;
                string sFineHour = string.Empty;
                string sFineMinute = string.Empty;
                string sSystemCheckoutHour = string.Empty;
                string sSystemCheckoutMinute = string.Empty;

                //fetch sup li service time 
                var liService = (List<SUP_LI_SERVICE_TIME>)CMSHandler.FetchData<SUP_LI_SERVICE_TIME>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupLiServiceTime)).DataSource.Data;
                if (liService != null && liService.Count > 0)
                {
                    var Dues = liService.Where(x => x.SERVICE_ID == Common.LI_SERVICE_TIME.DUES).ToList();
                    if (Dues != null && Dues.Count > 0)
                    {
                        sDuesHour = Dues.FirstOrDefault().S_HOUR;
                        sDuesMinute = Dues.FirstOrDefault().S_MINUTE;
                    }
                    var ReserveCancel = liService.Where(x => x.SERVICE_ID == Common.LI_SERVICE_TIME.RESERVE_CANCEL).ToList();
                    if (ReserveCancel != null && ReserveCancel.Count > 0)
                    {
                        sReserveCancelHour = ReserveCancel.FirstOrDefault().S_HOUR;
                        sReserveCancelMinute = ReserveCancel.FirstOrDefault().S_MINUTE;
                    }
                    var Fine = liService.Where(x => x.SERVICE_ID == Common.LI_SERVICE_TIME.FINE).ToList();
                    if (Fine != null && Fine.Count > 0)
                    {
                        sFineHour = Fine.FirstOrDefault().S_HOUR;
                        sFineMinute = Fine.FirstOrDefault().S_MINUTE;
                    }

                    var SystemCheckout = liService.Where(x => x.SERVICE_ID == Common.LI_SERVICE_TIME.SYSTEM_CHECKOUT).ToList();
                    if (SystemCheckout != null && SystemCheckout.Count > 0)
                    {
                        sSystemCheckoutHour = SystemCheckout.FirstOrDefault().S_HOUR;
                        sSystemCheckoutMinute = SystemCheckout.FirstOrDefault().S_MINUTE;
                    }
                }


                // for book reserve cancel service
                ITrigger trigger = TriggerBuilder.Create()
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(Convert.ToInt16(sReserveCancelHour), Convert.ToInt16(sReserveCancelMinute)))
                      )
                    .Build();

                // for due book fine service
                ITrigger trigger1 = TriggerBuilder.Create()
                   .WithDailyTimeIntervalSchedule
                     (s =>
                        s.WithIntervalInHours(24)
                       .OnEveryDay()
                       .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(Convert.ToInt16(sFineHour), Convert.ToInt16(sFineMinute)))
                     )
                   .Build();


                // for sending due sms
                ITrigger trigger2 = TriggerBuilder.Create()
                   .WithDailyTimeIntervalSchedule
                     (s =>
                        s.WithIntervalInHours(24)
                       .OnEveryDay()
                       .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(Convert.ToInt16(sDuesHour), Convert.ToInt16(sDuesMinute)))
                     )
                   .Build();

                // for system checkout
                ITrigger trigger3 = TriggerBuilder.Create()
                   .WithDailyTimeIntervalSchedule
                     (s =>
                        s.WithIntervalInHours(24)
                       .OnEveryDay()
                       .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(Convert.ToInt16(sSystemCheckoutHour), Convert.ToInt16(sSystemCheckoutMinute)))
                     )
                   .Build();

                // jobs
                IJobDetail job = JobBuilder.Create<TaskQueue>().Build();
                IJobDetail job1 = JobBuilder.Create<TaskQueue1>().Build();
                IJobDetail job2 = JobBuilder.Create<TaskQueue2>().Build();
                IJobDetail job3 = JobBuilder.Create<TaskQueue3>().Build();

                scheduler.ScheduleJob(job, trigger);
                scheduler.ScheduleJob(job1, trigger1);
                scheduler.ScheduleJob(job2, trigger2);
                scheduler.ScheduleJob(job3, trigger3);
                scheduler.Start();

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("JobScheduler", "Start", ex.Message);
                }
            }


        }
    }

    #endregion

}