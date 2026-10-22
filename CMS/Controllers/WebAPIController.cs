using CMS.DAO;
using CMS.SQL.Library;
using CMS.Utility;
using CMS.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.Controllers
{
    public class WebAPIController : Controller
    {
        // GET: WebAPI
        public ActionResult Index()
        {
            return View();
        }

        #region Library Login From Windows Application
        public JsonResult LibraryLogin(string sScanId)
        {
            var ObjJsonData = new JsonResultData();
            bool sFlag = false;
            try
            {
                if (!string.IsNullOrEmpty(sScanId))
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
                    }

                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("WebAPIController", "LibraryLogin", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
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
                    ObjLog.WriteError("WebAPIController", "ValidateCheckIn", ex.Message);
                }
            }
            return sCheckInId;
        }
        #endregion
    }
}