using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Utility;
using CMS.ViewModel.ViewModel;
using CMS.ViewModel.Model;
using CMS.DAO;
using CMS.SQL.IQAC;
using Newtonsoft.Json;
using System.IO;

namespace CMS.Controllers
{
    public class IQACController : Controller
    {
        // GET: IQAC
        public ActionResult Index()
        {
            return View();
        }


        #region 1. Teaching Subjects With Workload (Workload)

        public ActionResult TeachingSubjectsWithWorkload()
        {
            var ObjViewModel = new IQACViewModel();

            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //ObjModel.STAFF_ID = sStaffId;
                    //var liCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(ObjModel, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByStaffId), sAcademicYear).DataSource.Data;
                    //if (liCourse != null && liCourse.Count > 0)
                    //{
                    //    ObjViewModel.sCourseList = new SelectList(liCourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
                    //}

                    ObjViewModel.liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumber), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "TeachingSubjectsWithWorkload", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindTeachingSubjectsWithWorkload(string sCourseId, string sMonthId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.COURSE_ID = sCourseId;
                    ObjModel.MONTH_ID = sMonthId;
                    ObjViewModel.liTeachingSubjectWorkload = (List<IQAC_TEACHING_SUBJECTS_WORKLOAD>)CMSHandler.FetchData<IQAC_TEACHING_SUBJECTS_WORKLOAD>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchTeachingSubjectWorkloadByStaffAndCourseAndMonthId), sAcademicYear).DataSource.Data;
                    ObjViewModel.liClassCourseStaff = (List<IQAC_TEACHING_SUBJECTS_WORKLOAD>)CMSHandler.FetchData<IQAC_TEACHING_SUBJECTS_WORKLOAD>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchClassCourseStaffByStaffAndCourseId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindTeachingSubjectsWithWorkload", ex.Message);
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
            return View(ObjViewModel);
        }

        public JsonResult SaveTeachingSubjectsWithWorkload(string sJsonData)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Json_Save>(sJsonData);
                    if (Result.liWorkload != null && Result.liWorkload.Count > 0)
                    {
                        foreach (var item in Result.liWorkload)
                        {
                            item.STAFF_ID = sStaffId;
                            if (!string.IsNullOrEmpty(item.WORKLOAD_ID))
                            {
                                var sUpdateData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateTeachingSubjectWorkload), sAcademicYear);
                                if (sUpdateData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                                }
                            }
                            else
                            {
                                var sSaveData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveTeachingSubjectWorkload), sAcademicYear);
                                if (sSaveData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveTeachingSubjectsWithWorkload", ex.Message);
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

        public JsonResult DeleteTeachingSubjectsWithWorkload(string sWorkloadId)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
                    ObjModel.WORKLOAD_ID = sWorkloadId;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteTeachingSubjectWorkload));
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
                        ObjLog.WriteError("IQACController", "DeleteTeachingSubjectsWithWorkload", ex.Message);
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

        #region 2. Preparation & Updation Of Notes (Teaching Learning)

        public ActionResult PreparationAndUpdationOfNotes()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //ObjModel.STAFF_ID = sStaffId;
                    //var liCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(ObjModel, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByStaffId), sAcademicYear).DataSource.Data;
                    //if (liCourse != null && liCourse.Count > 0)
                    //{
                    //    ObjViewModel.sCourseList = new SelectList(liCourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
                    //}

                    ObjViewModel.liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumber), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "PreparationAndUpdationOfNotes", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindPreparationAndUpdationOfNotes(string sCourseId, string sMonthId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.COURSE_ID = sCourseId;
                    ObjModel.MONTH_ID = sMonthId;
                    ObjViewModel.liPreparation = (List<IQAC_PREPARATION_AND_UPDATION_OF_NOTES>)CMSHandler.FetchData<IQAC_PREPARATION_AND_UPDATION_OF_NOTES>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPreparationAndUpdationOfNotesByStaffAndCourseAndMonthId), sAcademicYear).DataSource.Data;
                    ObjViewModel.liClassCourseStaff = (List<IQAC_TEACHING_SUBJECTS_WORKLOAD>)CMSHandler.FetchData<IQAC_TEACHING_SUBJECTS_WORKLOAD>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchClassCourseStaffByStaffAndCourseId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindPreparationAndUpdationOfNotes", ex.Message);
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

        public JsonResult SavePreparationAndUpdationOfNotes()
        {
            string sDirectorypath = string.Empty, sFilePath = string.Empty, sFileName = string.Empty, sSavePath = string.Empty, sFileUrl = string.Empty;

            ResultArgs resultArgs = new ResultArgs();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    int nSuccessCount = 0;
                    int nFailedCount = 0;
                    var Result = JsonConvert.DeserializeObject<Json_Save>(Request.Form["sJsonData"]);
                    if (Result.liPreparation != null && Result.liPreparation.Count > 0)
                    {

                        sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "\\" + sStaffId;
                        sFileUrl = Common.URLPages.IQAC_IMAGE_PATH + "\\" + sStaffId;
                        foreach (var item in Result.liPreparation)
                        {
                            item.STAFF_ID = sStaffId;
                            if (!string.IsNullOrEmpty(item.IQAC_ID))
                            {
                                var sUpdateData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdatePreparationAndUpdationOfNotes), sAcademicYear);
                                if (sUpdateData.Success)
                                {
                                    nSuccessCount++;
                                    string FilePath = string.Empty;
                                    var liIQACItem = Result.liUPreparation.Where(x => x.CLASS_ID == item.CLASS_ID && !string.IsNullOrEmpty(item.IQAC_ID)).ToList();
                                    if (liIQACItem != null && liIQACItem.Count > 0)
                                    {
                                        //save file
                                        foreach (var item1 in liIQACItem)
                                        {
                                            //for upload img path
                                            if (item1.IMG_PATH != "" && item1.IMG_PATH != null)
                                            {
                                                var imgPath = item1.IMG_PATH.Split(',');
                                                foreach (var item2 in imgPath)
                                                {
                                                    FilePath += DecryptString(item2) + ",";
                                                }
                                            }

                                            else
                                            {
                                                //for new file save
                                                var filePath = item1.FILE_PATH.Split(',');
                                                foreach (var item2 in filePath)
                                                {
                                                    FilePath += SaveFileandGetFileNameByFileKey(item2, sDirectorypath, "PREPARATION&UPDATES", sStaffId, sFileUrl) + ",";
                                                }
                                            }

                                        }
                                        if (!string.IsNullOrEmpty(FilePath))
                                        {
                                            sFilePath = FilePath.Trim(',');
                                            item.FILE_PATH = sFilePath;
                                            resultArgs = CMSHandler.SaveOrUpdate(item, SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.UpdatePreparationAndUpdationOfNotesByIQAcId), sAcademicYear);
                                        }
                                    }
                                }


                                else
                                    nFailedCount++;
                            }
                            else
                            {
                                var sSaveData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SavePreparationAndUpdationOfNotes), sAcademicYear);
                                if (sSaveData.Success)
                                {
                                    nSuccessCount++;
                                    string FilePath = string.Empty;
                                    var liIQACItem = Result.liUPreparation.Where(x => x.CLASS_ID == item.CLASS_ID && string.IsNullOrEmpty(x.IQAC_ID)).ToList();

                                    if (liIQACItem != null && liIQACItem.Count > 0)
                                    {
                                        IQAC_PREPARATION_AND_UPDATION_OF_NOTES itemFileds = new IQAC_PREPARATION_AND_UPDATION_OF_NOTES()
                                        {
                                            CLASS_ID = liIQACItem.FirstOrDefault().CLASS_ID,
                                            STAFF_ID = sStaffId,
                                            COURSE_ID = liIQACItem.FirstOrDefault().COURSE_ID,
                                            MONTH_ID = liIQACItem.FirstOrDefault().MONTH_ID
                                        };
                                        var liIQAC = (List<IQAC_PREPARATION_AND_UPDATION_OF_NOTES>)CMSHandler.FetchData<IQAC_PREPARATION_AND_UPDATION_OF_NOTES>(itemFileds
                                              , SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPreparationAndUpdationOfNotesByStaffAndClassAndMonthId), sAcademicYear).DataSource.Data;

                                        if (liIQAC != null && liIQAC.Count > 0)
                                        {
                                            //save file
                                            foreach (var item1 in liIQACItem)
                                            {
                                                var filePath = item1.FILE_PATH.Split(',');
                                                foreach (var item2 in filePath)
                                                {
                                                    FilePath += SaveFileandGetFileNameByFileKey(item2, sDirectorypath, "PREPARATION&UPDATES", sStaffId, sFileUrl) + ",";
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(FilePath))
                                            {
                                                sFilePath = FilePath.Trim(',');
                                                liIQAC[0].FILE_PATH = sFilePath;
                                                resultArgs = CMSHandler.SaveOrUpdate(liIQAC[0], SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.UpdatePreparationAndUpdationOfNotesByIQAcId), sAcademicYear);
                                            }
                                        }
                                    }
                                }
                                else
                                    nFailedCount++;
                            }
                        }
                    }

                    if (nSuccessCount != 0)
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        ObjJsonData.Message = "( " + nSuccessCount.ToString() + " )" + Common.Messages.RecordsSavedSuccessfully;
                    }
                    if (nFailedCount != 0)
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        ObjJsonData.Message += "( " + nFailedCount.ToString() + " )" + Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SavePreparationAndUpdationOfNotes", ex.Message);
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

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
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
                        sSavePath = sFileUrl + "\\" + sFolder + "\\" + IPFile[0].ToUpper() + "_" + sFileSaveName.ToUpper() + "." + IPFile[1];
                    }
                    else
                    {
                        sSavePath = sFileUrl + "\\" + IPFile[0].ToUpper() + "_" + sFileSaveName.ToUpper() + "." + IPFile[1];
                    }
                    file.SaveAs(sFilePath);
                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveFileandGetFileNameByFileKey", ex.Message);
                    }
                }

            }

            return sSavePath;

        }

        public string DecryptString(string encrString)
        {
            byte[] b;
            string decrypted;
            try
            {
                b = Convert.FromBase64String(encrString);
                decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);
            }
            catch (FormatException fe)
            {
                decrypted = "";
            }
            return decrypted;
        }

        public JsonResult EncryptStringForImagePath(string inputString)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(inputString);
            string encrypted = Convert.ToBase64String(b);
            return Json(encrypted, JsonRequestBehavior.AllowGet);
        }


        public JsonResult DeletePreparationAndUpdationOfNotes(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_EXTENSION_WORKS();
                    ObjModel.IQAC_ID = sIQACID;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeletePreparationAndUpdationOfNotes));
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
                        ObjLog.WriteError("IQACController", "DeletePreparationAndUpdationOfNotes", ex.Message);
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

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 3. Co - Curricular Aspects

        public ActionResult CurricularAspects()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    // Validate Co Ordinator ,...
                    var liCoordinatoeList = (List<IQAC_CURRICULAR_COORDINATOR>)CMSHandler.FetchData<IQAC_CURRICULAR_COORDINATOR>(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.ValidateCoOrdinator), sAcademicYear).DataSource.Data;
                    if (liCoordinatoeList != null && liCoordinatoeList.Count > 0)
                        ObjViewModel.sFlag = true;

                    ObjViewModel.liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumber), sAcademicYear).DataSource.Data;
                    //var liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumber), sAcademicYear).DataSource.Data;
                    //if (liMonthList != null && liMonthList.Count > 0)
                    //{
                    //    ObjViewModel.sMonthList = new SelectList(liMonthList, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    //}
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "CurricularAspects", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindCurricularAspects(string sMonthId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.MONTH_ID = sMonthId;
                    ObjViewModel.liCurricularAspects = (List<IQAC_CO_CURRICULAR_ASPECTS>)CMSHandler.FetchData<IQAC_CO_CURRICULAR_ASPECTS>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchCoCurricularAspectsByMonthAndStaffId), sAcademicYear).DataSource.Data;
                    ObjViewModel.liCommittee = (List<IQAC_CURRICULAR_COORDINATOR>)CMSHandler.FetchData<IQAC_CURRICULAR_COORDINATOR>(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.ValidateCoOrdinator), sAcademicYear).DataSource.Data;
                    //if (liCoordinatoeList!=null && liCoordinatoeList.Count>0)
                    //{
                    //    ObjViewModel.sCommitteeList = new SelectList(liCoordinatoeList, "IQAC_ID", "COMMITTEE_NAME");
                    //}
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindCurricularAspects", ex.Message);
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
            return View(ObjViewModel);
        }

        public JsonResult SaveCurricularAspects()
        {
            ResultArgs resultArgs = new ResultArgs();
            string sDirectorypath = string.Empty, sFilePath = string.Empty, sFileName = string.Empty, sSavePath = string.Empty, sFileUrl = string.Empty;
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //var Result = JsonConvert.DeserializeObject<Json_Save>(sJsonData);
                    var Result = JsonConvert.DeserializeObject<Json_Save>(Request.Form["sJsonData"]);
                    if (Result.liCurricularAspects != null && Result.liCurricularAspects.Count > 0)
                    {
                        sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "\\" + sStaffId;
                        sFileUrl = Common.URLPages.IQAC_IMAGE_PATH + "\\" + sStaffId;
                        foreach (var item in Result.liCurricularAspects)
                        {
                            item.STAFF_ID = sStaffId;
                            item.ACTIVITY_DATE = CommonMethods.ConvertdatetoyearFromat(item.ACTIVITY_DATE);
                            if (!string.IsNullOrEmpty(item.IQAC_ID))
                            {
                                string FilePath = string.Empty;


                                if (item.IMG_PATH != "" && item.IMG_PATH != null)
                                {
                                    var imgPath = item.IMG_PATH.Split(',');
                                    foreach (var item2 in imgPath)
                                    {
                                        FilePath += DecryptString(item2) + ",";
                                    }
                                }

                                if (item.FILE_PATH != "" && item.FILE_PATH != null)
                                {
                                    var filePath1 = item.FILE_PATH.Split(',');
                                    foreach (var item2 in filePath1)
                                    {
                                        FilePath += SaveFileandGetFileNameByFileKey(item2, sDirectorypath, "COCURRICULAR", sStaffId, sFileUrl) + ",";
                                    }
                                }
                                if (!string.IsNullOrEmpty(FilePath))
                                {
                                    sFilePath = FilePath.Trim(',');
                                    item.FILE_PATH = sFilePath;
                                }



                                var sUpdateData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateCurricularAspects), sAcademicYear);
                                if (sUpdateData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                                }
                            }
                            else
                            {
                                string FilePath = string.Empty;
                                if (!string.IsNullOrEmpty(item.FILE_PATH))
                                {
                                    var filePath = item.FILE_PATH.Split(',');
                                    foreach (var item2 in filePath)
                                    {
                                        FilePath += SaveFileandGetFileNameByFileKey(item2, sDirectorypath, "COCURRICULAR", sStaffId, sFileUrl) + ",";
                                    }
                                    if (!string.IsNullOrEmpty(FilePath))
                                    {
                                        sFilePath = FilePath.Trim(',');
                                        item.FILE_PATH = sFilePath;
                                    }
                                }

                                var sSaveData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveCurricularAspects), sAcademicYear);
                                if (sSaveData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveCurricularAspects", ex.Message);
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

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteCurricularAspects(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_EXTENSION_WORKS();
                    ObjModel.IQAC_ID = sIQACID;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteCurricularAspects));
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
                        ObjLog.WriteError("IQACController", "DeleteCurricularAspects", ex.Message);
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

        #region 4. Extension Works

        public ActionResult ExtensionWorks()
        {
            var ObjViewModel = new IQACViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumber), sAcademicYear).DataSource.Data;
                    //var liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumber), sAcademicYear).DataSource.Data;
                    //if (liMonthList != null && liMonthList.Count > 0)
                    //{
                    //    ObjViewModel.sMonthList = new SelectList(liMonthList, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    //}
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "ExtensionWorks", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindExtensionWorks(string sMonthId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.MONTH_ID = sMonthId;
                    ObjViewModel.liExtensionWorks = (List<IQAC_EXTENSION_WORKS>)CMSHandler.FetchData<IQAC_EXTENSION_WORKS>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchExtensionWorksByMonthAndStaffId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindExtensionWorks", ex.Message);
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
            return View(ObjViewModel);
        }

        public JsonResult SaveExtensionWorks()
        {
            ResultArgs resultArgs = new ResultArgs();
            string sDirectorypath = string.Empty, sFilePath = string.Empty, sFileName = string.Empty, sSavePath = string.Empty, sFileUrl = string.Empty;
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //var Result = JsonConvert.DeserializeObject<Json_Save>(sJsonData);
                    var Result = JsonConvert.DeserializeObject<Json_Save>(Request.Form["sJsonData"]);
                    if (Result.liExtensionWorks != null && Result.liExtensionWorks.Count > 0)
                    {
                        sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "\\" + sStaffId;
                        sFileUrl = Common.URLPages.IQAC_IMAGE_PATH + "\\" + sStaffId;

                        foreach (var item in Result.liExtensionWorks)
                        {
                            item.STAFF_ID = sStaffId;
                            item.ACTIVITY_DATE = CommonMethods.ConvertdatetoyearFromat(item.ACTIVITY_DATE);
                            if (!string.IsNullOrEmpty(item.IQAC_ID))
                            {
                                string FilePath = string.Empty;


                                if (item.IMG_PATH != "" && item.IMG_PATH != null)
                                {
                                    var imgPath = item.IMG_PATH.Split(',');
                                    foreach (var item2 in imgPath)
                                    {
                                        FilePath += DecryptString(item2) + ",";
                                    }
                                }

                                if (item.FILE_PATH != "" && item.FILE_PATH != null)
                                {
                                    var filePath1 = item.FILE_PATH.Split(',');
                                    foreach (var item2 in filePath1)
                                    {
                                        FilePath += SaveFileandGetFileNameByFileKey(item2, sDirectorypath, "EXTENSIONS", sStaffId, sFileUrl) + ",";
                                    }
                                }
                                if (!string.IsNullOrEmpty(FilePath))
                                {
                                    sFilePath = FilePath.Trim(',');
                                    item.FILE_PATH = sFilePath;
                                }

                                var sUpdateData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateExtensionWorks), sAcademicYear);
                                if (sUpdateData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                                }
                            }
                            else
                            {
                                string FilePath = string.Empty;
                                if (!string.IsNullOrEmpty(item.FILE_PATH))
                                {
                                    var filePath = item.FILE_PATH.Split(',');
                                    foreach (var item2 in filePath)
                                    {
                                        FilePath += SaveFileandGetFileNameByFileKey(item2, sDirectorypath, "EXTENSIONS", sStaffId, sFileUrl) + ",";
                                    }
                                    if (!string.IsNullOrEmpty(FilePath))
                                    {
                                        sFilePath = FilePath.Trim(',');
                                        item.FILE_PATH = sFilePath;
                                    }
                                }
                                var sSaveData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveExtensionWorks), sAcademicYear);
                                if (sSaveData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveQuestionNoFour", ex.Message);
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

        public JsonResult DeleteExtensionWorks(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_EXTENSION_WORKS();
                    ObjModel.IQAC_ID = sIQACID;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteExtensionWorks));
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
                        ObjLog.WriteError("IQACController", "DeleteExtensionWorks", ex.Message);
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

        #region 5. Personal Academic Activity (Curricular Aspects)

        public ActionResult PersonalAcademicActivity()
        {
            var ObjViewModel = new IQACViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumber), sAcademicYear).DataSource.Data;
                    //var liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumber), sAcademicYear).DataSource.Data;
                    //if (liMonthList != null && liMonthList.Count > 0)
                    //{
                    //    ObjViewModel.sMonthList = new SelectList(liMonthList, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    //}
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "PersonalAcademicActivity", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindPersonalAcademicActivity(string sMonthId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.MONTH_ID = sMonthId;
                    ObjViewModel.liPersonalAcademicActivity = (List<IQAC_PERSONAL_ACADEMIC_ACTIVITY>)CMSHandler.FetchData<IQAC_PERSONAL_ACADEMIC_ACTIVITY>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPersonalAcademicActivityByMonthAndStaffId), sAcademicYear).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindPersonalAcademicActivity", ex.Message);
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
            return View(ObjViewModel);
        }

        public JsonResult SavePersonalAcademicActivity(string sJsonData)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    int nSuccessCount = 0;
                    int nFailedCount = 0;
                    var Result = JsonConvert.DeserializeObject<Json_Save>(sJsonData);
                    if (Result.liPersonalAcademicActivity != null && Result.liPersonalAcademicActivity.Count > 0)
                    {
                        foreach (var item in Result.liPersonalAcademicActivity)
                        {
                            item.STAFF_ID = sStaffId;
                            item.ACTIVITY_DATE = CommonMethods.ConvertdatetoyearFromat(item.ACTIVITY_DATE);
                            if (!string.IsNullOrEmpty(item.IQAC_ID))
                            {
                                var sUpdateData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdatePersonalAcademicActivity), sAcademicYear);
                                if (sUpdateData.Success)
                                    nSuccessCount++;
                                else
                                    nFailedCount++;
                            }
                            else
                            {
                                var sSaveData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SavePersonalAcademicActivity), sAcademicYear);
                                if (sSaveData.Success)
                                    nSuccessCount++;
                                else
                                    nFailedCount++;
                            }
                        }

                        if (nSuccessCount != 0)
                        {
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            ObjJsonData.Message = "( " + nSuccessCount.ToString() + " )" + Common.Messages.RecordsSavedSuccessfully;
                        }
                        if (nFailedCount != 0)
                        {
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            ObjJsonData.Message += "( " + nFailedCount.ToString() + " )" + Common.Messages.FailedToSaveRecords;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SavePersonalAcademicActivity", ex.Message);
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

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletePersonalAcademicActivity(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_EXTENSION_WORKS();
                    ObjModel.IQAC_ID = sIQACID;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeletePersonalAcademicActivity));
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
                        ObjLog.WriteError("IQACController", "DeletePersonalAcademicActivity", ex.Message);
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

        // Not In Use
        #region 6. Work Done Data

        public ActionResult WorkDoneData()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    var liCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(ObjModel, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByStaffId), sAcademicYear).DataSource.Data;
                    if (liCourse != null && liCourse.Count > 0)
                    {
                        ObjViewModel.sCourseList = new SelectList(liCourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
                    }

                    ObjViewModel.liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumber), sAcademicYear).DataSource.Data;
                    //var liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumber), sAcademicYear).DataSource.Data;
                    //if (liMonthList != null && liMonthList.Count > 0)
                    //{
                    //    ObjViewModel.sMonthList = new SelectList(liMonthList, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    //}
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "WorkDoneData", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindWorkDoneData(string sCourseId, string sMonthId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.COURSE_ID = sCourseId;
                    ObjModel.MONTH_ID = sMonthId;
                    ObjViewModel.liWorkDoneData = (List<IQAC_WORK_DONE_DATA>)CMSHandler.FetchData<IQAC_WORK_DONE_DATA>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchWorkDoneDataByStaffAndCourseAndMonthId), sAcademicYear).DataSource.Data;
                    ObjViewModel.liClassCourseStaff = (List<IQAC_TEACHING_SUBJECTS_WORKLOAD>)CMSHandler.FetchData<IQAC_TEACHING_SUBJECTS_WORKLOAD>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchClassCourseStaffByStaffAndCourseId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindWorkDoneData", ex.Message);
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
            return View(ObjViewModel);
        }

        public JsonResult SaveWorkDoneData(string sJsonData)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Json_Save>(sJsonData);
                    if (Result.liWorkDoneData != null && Result.liWorkDoneData.Count > 0)
                    {
                        foreach (var item in Result.liWorkDoneData)
                        {
                            item.STAFF_ID = sStaffId;
                            if (!string.IsNullOrEmpty(item.IQAC_ID))
                            {
                                var sUpdateData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateWorkDoneData), sAcademicYear);
                                if (sUpdateData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                                }
                            }
                            else
                            {
                                var sSaveData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveWorkDoneData), sAcademicYear);
                                if (sSaveData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveWorkDoneData", ex.Message);
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
            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteWorkDoneData(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_EXTENSION_WORKS();
                    ObjModel.IQAC_ID = sIQACID;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteWorkDoneData));
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
                        ObjLog.WriteError("IQACController", "DeleteWorkDoneData", ex.Message);
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
        // Not In Use
        #region 7. Examination Duties Internal

        public ActionResult ExaminationDutiesInternal()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    var liCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(ObjModel, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByStaffId), sAcademicYear).DataSource.Data;
                    if (liCourse != null && liCourse.Count > 0)
                    {
                        ObjViewModel.sCourseList = new SelectList(liCourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
                    }

                    ObjViewModel.liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumber), sAcademicYear).DataSource.Data;
                    //var liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumber), sAcademicYear).DataSource.Data;
                    //if (liMonthList != null && liMonthList.Count > 0)
                    //{
                    //    ObjViewModel.sMonthList = new SelectList(liMonthList, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    //}
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "ExaminationDutiesInternal", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindExaminationDutiesInternal(string sCourseId, string sMonthId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.COURSE_ID = sCourseId;
                    ObjModel.MONTH_ID = sMonthId;
                    ObjViewModel.liExaminationDutiesInternal = (List<IQAC_EXAMINATION_DUTIES_INTERNAL>)CMSHandler.FetchData<IQAC_EXAMINATION_DUTIES_INTERNAL>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchExaminationDutiesInternalByStaffAndCourseAndMonthId), sAcademicYear).DataSource.Data;
                    ObjViewModel.liClassCourseStaff = (List<IQAC_TEACHING_SUBJECTS_WORKLOAD>)CMSHandler.FetchData<IQAC_TEACHING_SUBJECTS_WORKLOAD>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchClassCourseStaffByStaffAndCourseId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindExaminationDutiesInternal", ex.Message);
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
            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(ObjViewModel);
        }

        public JsonResult SaveExaminationDutiesInternal(string sJsonData)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Json_Save>(sJsonData);
                    if (Result.liExaminationDutiesInternal != null && Result.liExaminationDutiesInternal.Count > 0)
                    {
                        foreach (var item in Result.liExaminationDutiesInternal)
                        {
                            item.STAFF_ID = sStaffId;
                            item.EXAMINATION_DTATE = CommonMethods.ConvertdatetoyearFromat(item.EXAMINATION_DTATE);
                            if (!string.IsNullOrEmpty(item.IQAC_ID))
                            {
                                var sUpdateData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateExaminationDutiesInternal), sAcademicYear);
                                if (sUpdateData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                                }
                            }
                            else
                            {
                                var sSaveData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveExaminationDutiesInternal), sAcademicYear);
                                if (sSaveData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveExaminationDutiesInternal", ex.Message);
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
            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteExaminationDutiesInternal(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_EXTENSION_WORKS();
                    ObjModel.IQAC_ID = sIQACID;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteExaminationDutiesInternal));
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
                        ObjLog.WriteError("IQACController", "DeleteExaminationDutiesInternal", ex.Message);
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
        // Not In Use
        #region 8. Professional Academic Activities

        public ActionResult ProfessionalAcademicActivities()
        {
            var ObjViewModel = new IQACViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumber), sAcademicYear).DataSource.Data;
                    //var liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumber), sAcademicYear).DataSource.Data;
                    //if (liMonthList != null && liMonthList.Count > 0)
                    //{
                    //    ObjViewModel.sMonthList = new SelectList(liMonthList, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    //}
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "ProfessionalAcademicActivities", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindProfessionalAcademicActivities(string sMonthId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.MONTH_ID = sMonthId;
                    ObjViewModel.liProfessionalAcademicActivity = (List<IQAC_PROFESSIONAL_ACADEMIC_ACTIVITIES>)CMSHandler.FetchData<IQAC_PROFESSIONAL_ACADEMIC_ACTIVITIES>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchProfessionalAcademicActivityByMonthAndStaffId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindProfessionalAcademicActivities", ex.Message);
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
            return View(ObjViewModel);
        }

        public JsonResult SaveProfessionalAcademicActivities(string sJsonData)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Json_Save>(sJsonData);
                    if (Result.liProfessionalAcademicActivity != null && Result.liProfessionalAcademicActivity.Count > 0)
                    {
                        foreach (var item in Result.liProfessionalAcademicActivity)
                        {
                            item.STAFF_ID = sStaffId;
                            item.ACTIVITY_DATE = CommonMethods.ConvertdatetoyearFromat(item.ACTIVITY_DATE);
                            if (!string.IsNullOrEmpty(item.IQAC_ID))
                            {
                                var sUpdateData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateProfessionalAcademicActivity), sAcademicYear);
                                if (sUpdateData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                                }
                            }
                            else
                            {
                                var sSaveData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveProfessionalAcademicActivity), sAcademicYear);
                                if (sSaveData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveProfessionalAcademicActivities", ex.Message);
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

        public JsonResult DeleteProfessionalAcademicActivities(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_EXTENSION_WORKS();
                    ObjModel.IQAC_ID = sIQACID;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteProfessionalAcademicActivity));
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
                        ObjLog.WriteError("IQACController", "DeleteProfessionalAcademicActivities", ex.Message);
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
        // Not In Use
        #region 9. Research Projects

        public ActionResult ResearchProjects()
        {
            var ObjViewModel = new IQACViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthFromCalender)).DataSource.Data;
                    if (liMonthList != null && liMonthList.Count > 0)
                    {
                        ObjViewModel.sMonthList = new SelectList(liMonthList, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "ResearchProjects", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindResearchProjects(string sMonthId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.MONTH_ID = sMonthId;
                    ObjViewModel.liResearchProjects = (List<IQAC_RESEARCH_PROJECTS>)CMSHandler.FetchData<IQAC_RESEARCH_PROJECTS>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResearchProjectByMonthAndStaffId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindProfessionalAcademicActivities", ex.Message);
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
            return View(ObjViewModel);
        }

        public JsonResult SaveResearchProjects(string sJsonData)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Json_Save>(sJsonData);
                    if (Result.liResearchProject != null && Result.liResearchProject.Count > 0)
                    {
                        foreach (var item in Result.liResearchProject)
                        {
                            item.STAFF_ID = sStaffId;
                            if (!string.IsNullOrEmpty(item.IQAC_ID))
                            {
                                var sUpdateData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateResearchProject), sAcademicYear);
                                if (sUpdateData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                                }
                            }
                            else
                            {
                                var sSaveData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveResearchProject), sAcademicYear);
                                if (sSaveData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveResearchProjects", ex.Message);
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

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteResearchProjects(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_EXTENSION_WORKS();
                    ObjModel.IQAC_ID = sIQACID;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteResearchProject));
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
                        ObjLog.WriteError("IQACController", "DeleteResearchProjects", ex.Message);
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
        // Not In Use
        #region 10. A. Research And Academic Contribution

        public ActionResult ResearchAndAcademicContributionA()
        {
            var ObjViewModel = new IQACViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthFromCalender)).DataSource.Data;
                    if (liMonthList != null && liMonthList.Count > 0)
                    {
                        ObjViewModel.sMonthList = new SelectList(liMonthList, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "ResearchAndAcademicContributionA", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindResearchAndAcademicContributionA(string sMonthId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.MONTH_ID = sMonthId;
                    ObjViewModel.liResearchAcademicContributionA = (List<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_A>)CMSHandler.FetchData<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_A>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResearchAcademicContributionAByMonthAndStaffId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindResearchAndAcademicContributionA", ex.Message);
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
            return View(ObjViewModel);
        }

        public JsonResult SaveResearchAndAcademicContributionA(string sJsonData)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Json_Save>(sJsonData);
                    if (Result.liResearchAcademicContributionA != null && Result.liResearchAcademicContributionA.Count > 0)
                    {
                        foreach (var item in Result.liResearchAcademicContributionA)
                        {
                            item.STAFF_ID = sStaffId;
                            item.RESEARCH_DATE = CommonMethods.ConvertdatetoyearFromat(item.RESEARCH_DATE);
                            if (!string.IsNullOrEmpty(item.IQAC_ID))
                            {
                                var sUpdateData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateResearchAcademicContributionA), sAcademicYear);
                                if (sUpdateData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                                }
                            }
                            else
                            {
                                var sSaveData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveResearchAcademicContributionA), sAcademicYear);
                                if (sSaveData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveResearchAndAcademicContributionA", ex.Message);
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
            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteResearchAndAcademicContributionA(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_EXTENSION_WORKS();
                    ObjModel.IQAC_ID = sIQACID;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteResearchAcademicContributionA));
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
                        ObjLog.WriteError("IQACController", "DeleteResearchAndAcademicContributionA", ex.Message);
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
        // Not In Use
        #region 10. B. Research And Academic Contribution

        public ActionResult ResearchAndAcademicContributionB()
        {
            var ObjViewModel = new IQACViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthFromCalender)).DataSource.Data;
                    if (liMonthList != null && liMonthList.Count > 0)
                    {
                        ObjViewModel.sMonthList = new SelectList(liMonthList, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "ResearchAndAcademicContributionB", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindResearchAndAcademicContributionB(string sMonthId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.MONTH_ID = sMonthId;
                    ObjViewModel.liResearchAcademicContributionB = (List<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_B>)CMSHandler.FetchData<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_B>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResearchAcademicContributionBByMonthAndStaffId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindResearchAndAcademicContributionB", ex.Message);
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
            return View(ObjViewModel);
        }

        public JsonResult SaveResearchAndAcademicContributionB(string sJsonData)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Json_Save>(sJsonData);
                    if (Result.liResearchAcademicContributionB != null && Result.liResearchAcademicContributionB.Count > 0)
                    {
                        foreach (var item in Result.liResearchAcademicContributionB)
                        {
                            item.STAFF_ID = sStaffId;
                            if (!string.IsNullOrEmpty(item.IQAC_ID))
                            {
                                var sUpdateData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateResearchAcademicContributionB), sAcademicYear);
                                if (sUpdateData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                                }
                            }
                            else
                            {
                                var sSaveData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveResearchAcademicContributionB), sAcademicYear);
                                if (sSaveData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveResearchAndAcademicContributionB", ex.Message);
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
            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteResearchAndAcademicContributionB(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_EXTENSION_WORKS();
                    ObjModel.IQAC_ID = sIQACID;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteResearchAcademicContributionB));
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
                        ObjLog.WriteError("IQACController", "DeleteResearchAndAcademicContributionB", ex.Message);
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
        // Not In Usewl
        #region 10. C. Research And Academic Contribution

        public ActionResult ResearchAndAcademicContributionC()
        {
            var ObjViewModel = new IQACViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthFromCalender)).DataSource.Data;
                    if (liMonthList != null && liMonthList.Count > 0)
                    {
                        ObjViewModel.sMonthList = new SelectList(liMonthList, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "ResearchAndAcademicContributionC", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindResearchAndAcademicContributionC(string sMonthId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.MONTH_ID = sMonthId;
                    ObjViewModel.liResearchAcademicContributionC = (List<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_C>)CMSHandler.FetchData<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_C>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResearchAcademicContributionCByMonthAndStaffId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindResearchAndAcademicContributionC", ex.Message);
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
            return View(ObjViewModel);
        }

        public JsonResult SaveResearchAndAcademicContributionC(string sJsonData)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Json_Save>(sJsonData);
                    if (Result.liResearchAcademicContributionC != null && Result.liResearchAcademicContributionC.Count > 0)
                    {
                        foreach (var item in Result.liResearchAcademicContributionC)
                        {
                            item.STAFF_ID = sStaffId;
                            item.SEMINAR_DATE = CommonMethods.ConvertdatetoyearFromat(item.SEMINAR_DATE);
                            if (!string.IsNullOrEmpty(item.IQAC_ID))
                            {
                                var sUpdateData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateResearchAcademicContributionC), sAcademicYear);
                                if (sUpdateData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                                }
                            }
                            else
                            {
                                var sSaveData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveResearchAcademicContributionC), sAcademicYear);
                                if (sSaveData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveResearchAndAcademicContributionC", ex.Message);
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

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteResearchAndAcademicContributionC(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_EXTENSION_WORKS();
                    ObjModel.IQAC_ID = sIQACID;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteResearchAcademicContributionC));
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
                        ObjLog.WriteError("IQACController", "DeleteResearchAndAcademicContributionC", ex.Message);
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
        // Not In Use
        #region 10. D. Research And Academic Contribution

        public ActionResult ResearchAndAcademicContributionD()
        {
            var ObjViewModel = new IQACViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthFromCalender)).DataSource.Data;
                    if (liMonthList != null && liMonthList.Count > 0)
                    {
                        ObjViewModel.sMonthList = new SelectList(liMonthList, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "ResearchAndAcademicContributionD", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindResearchAndAcademicContributionD(string sMonthId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.MONTH_ID = sMonthId;
                    ObjViewModel.liResearchAcademicContributionD = (List<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_D>)CMSHandler.FetchData<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_D>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResearchAcademicContributionDByMonthAndStaffId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindResearchAndAcademicContributionD", ex.Message);
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
            return View(ObjViewModel);
        }

        public JsonResult SaveResearchAndAcademicContributionD(string sJsonData)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Json_Save>(sJsonData);
                    if (Result.liResearchAcademicContributionD != null && Result.liResearchAcademicContributionD.Count > 0)
                    {
                        foreach (var item in Result.liResearchAcademicContributionD)
                        {
                            item.STAFF_ID = sStaffId;
                            item.WORKSHOP_DATE = CommonMethods.ConvertdatetoyearFromat(item.WORKSHOP_DATE);
                            if (!string.IsNullOrEmpty(item.IQAC_ID))
                            {
                                var sUpdateData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateResearchAcademicContributionD), sAcademicYear);
                                if (sUpdateData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                                }
                            }
                            else
                            {
                                var sSaveData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveResearchAcademicContributionD), sAcademicYear);
                                if (sSaveData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveResearchAndAcademicContributionD", ex.Message);
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

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteResearchAndAcademicContributionD(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_EXTENSION_WORKS();
                    ObjModel.IQAC_ID = sIQACID;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteResearchAcademicContributionD));
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
                        ObjLog.WriteError("IQACController", "DeleteResearchAndAcademicContributionD", ex.Message);
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
        // Not In Use
        #region 10. E. Research And Academic Contribution

        public ActionResult ResearchAndAcademicContributionE()
        {
            var ObjViewModel = new IQACViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthFromCalender)).DataSource.Data;
                    if (liMonthList != null && liMonthList.Count > 0)
                    {
                        ObjViewModel.sMonthList = new SelectList(liMonthList, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "ResearchAndAcademicContributionE", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindResearchAndAcademicContributionE(string sMonthId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.MONTH_ID = sMonthId;
                    ObjViewModel.liResearchAcademicContributionE = (List<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_E>)CMSHandler.FetchData<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_E>(ObjModel,
                                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResearchAcademicContributionEByMonthAndStaffId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindResearchAndAcademicContributionE", ex.Message);
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
            return View(ObjViewModel);
        }

        public JsonResult SaveResearchAndAcademicContributionE(string sJsonData)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<Json_Save>(sJsonData);
                    if (Result.liResearchAcademicContributionE != null && Result.liResearchAcademicContributionE.Count > 0)
                    {
                        foreach (var item in Result.liResearchAcademicContributionE)
                        {
                            item.STAFF_ID = sStaffId;
                            if (!string.IsNullOrEmpty(item.IQAC_ID))
                            {
                                var sUpdateData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateResearchAcademicContributionE), sAcademicYear);
                                if (sUpdateData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                                }
                            }
                            else
                            {
                                var sSaveData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveResearchAcademicContributionE), sAcademicYear);
                                if (sSaveData.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveResearchAndAcademicContributionE", ex.Message);
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

        public JsonResult DeleteResearchAndAcademicContributionE(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_EXTENSION_WORKS();
                    ObjModel.IQAC_ID = sIQACID;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteResearchAcademicContributionE));
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
                        ObjLog.WriteError("IQACController", "SaveResearchAndAcademicContributionE", ex.Message);
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

        #region Lesson Plan
        // Lesson Plan Instent Of Work Done Data ,...
        public ActionResult ListLessonPlan()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "ListLessonPlan", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult FetchCourseByStaffAndSemesterType(string sSemesterTypeId)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new CourseWiseStaffList();
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.SEMESTER_TYPE_ID = sSemesterTypeId;
                    var liCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(ObjModel, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByStaffIdAndSemesterTypeId), sAcademicYear).DataSource.Data;
                    if (liCourse != null && liCourse.Count > 0)
                    {
                        ObjJsonData.sResult = "<option value=''> -- Select -- </option>";
                        foreach (var item in liCourse)
                        {
                            ObjJsonData.sResult += "<option value='" + item.COURSE_ROOT_ID + "'> " + item.COURSE_TITLE + " </option>";
                        }
                        //ObjViewModel.sCourseList = new SelectList(liCourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE, Common.SUP_SHIFT.SHIFT_ID);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "FetchCourseByStaffAndSemesterType", ex.Message);
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

        public ActionResult BindListLessonPlan(string sCourseId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.COURSE_ID = sCourseId;
                    ObjViewModel.liLessonPlan = (List<STF_LESSON_PLAN>)CMSHandler.FetchData<STF_LESSON_PLAN>(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchListLessonPlan), sAcademicYear).DataSource.Data;
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "LessonPlan", ex.Message);
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

        public ActionResult LessonPlan()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "LessonPlan", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindLessonPlan(string sCourseId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.COURSE_ID = sCourseId;
                    ObjViewModel.liLessonPlan = (List<STF_LESSON_PLAN>)CMSHandler.FetchData<STF_LESSON_PLAN>(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchLessonPlan), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindLessonPlan", ex.Message);
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

        public JsonResult SaveLessonPlan(string sJsonData)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    int nSuccessCount = 0;
                    int nFailedCount = 0;
                    var Result = JsonConvert.DeserializeObject<Json_Save>(sJsonData);
                    if (Result.liLessonPlan != null && Result.liLessonPlan.Count > 0)
                    {
                        foreach (var item in Result.liLessonPlan)
                        {
                            item.STAFF_ID = sStaffId;
                            if (!string.IsNullOrEmpty(item.IQAC_ID))
                            {
                                var sUpdateData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateLessonPlan), sAcademicYear);
                                if (sUpdateData.Success)
                                    nSuccessCount++;
                                else
                                    nFailedCount++;
                            }
                            else
                            {
                                var sSaveData = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveLessonPlan), sAcademicYear);
                                if (sSaveData.Success)
                                    nSuccessCount++;
                                else
                                    nFailedCount++;
                            }
                        }

                        if (nSuccessCount != 0)
                        {
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            ObjJsonData.Message = "( " + nSuccessCount.ToString() + " )" + Common.Messages.RecordsSavedSuccessfully;
                        }
                        if (nFailedCount != 0)
                        {
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            ObjJsonData.Message += "( " + nFailedCount.ToString() + " )" + Common.Messages.FailedToSaveRecords;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveLessonPlan", ex.Message);
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

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteLessonPlan(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_EXTENSION_WORKS();
                    ObjModel.IQAC_ID = sIQACID;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteLessonPlan));
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
                        ObjLog.WriteError("IQACController", "DeleteLessonPlan", ex.Message);
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

        #region Nav Bar For 8,9,10
        public ActionResult IQACActivity()
        {
            var ObjViewModel = new IQACViewModel();
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
                        ObjLog.WriteError("IQACController", "IQACActivity", ex.Message);
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

        #region 8. Achivements
        public ActionResult Achivements()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            string sAcademicYear =(Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    int nAcademicYear = Convert.ToInt32(sAcademicYear);
                    nAcademicYear--;
                    sAcademicYear = sAcademicYear + "," + nAcademicYear.ToString();

                    ObjModel.STAFF_ID = sStaffId;
                    ObjViewModel.liAwards = (List<IQAC_AWARDS>)CMSHandler.FetchData<IQAC_AWARDS>(ObjModel,
                            IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAwardsList), sAcademicYear).DataSource.Data;

                    ObjViewModel.liSubjectAssociation = (List<IQAC_SUBJECT_ASSOCIATION>)CMSHandler.FetchData<IQAC_SUBJECT_ASSOCIATION>(ObjModel,
                            IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSubjectAssociationList), sAcademicYear).DataSource.Data;

                    ObjViewModel.liHonor = (List<IQAC_HONOURS>)CMSHandler.FetchData<IQAC_HONOURS>(ObjModel,
                            IQACSQL.GetIQACSQL(IQACEnumCommands.FetchHonorList), sAcademicYear).DataSource.Data;

                    ObjViewModel.liMembership = (List<IQAC_MEMBERSHIP>)CMSHandler.FetchData<IQAC_MEMBERSHIP>(ObjModel,
                            IQACSQL.GetIQACSQL(IQACEnumCommands.FetchMembershipList), sAcademicYear).DataSource.Data;

                    var liProgrammeType = (List<SUP_PROGRAMME_TYPE>)CMSHandler.FetchData<SUP_PROGRAMME_TYPE>(null,
                               SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeType)).DataSource.Data;

                    if (liProgrammeType != null && liProgrammeType.Count > 0)
                    {
                        ObjViewModel.sProgrammeTypeList = new SelectList(liProgrammeType, "PROGRAMME_TYPE_ID", "PROGRAMME_TYPE");
                    }

                    var liLevel = (List<sup_Level>)CMSHandler.FetchData<sup_Level>(null,
                               SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLevel)).DataSource.Data;

                    if (liLevel != null && liLevel.Count > 0)
                    {
                        ObjViewModel.sLevelList = new SelectList(liLevel, Common.sup_LEVEL.LEVEL_ID, Common.sup_LEVEL.LEVEL);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "Achivements", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        // Awards ,...
        public JsonResult FetchAwardById(string sAwardId)
        {
            var ObjModel = new IQAC_AWARDS();
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.AWARD_ID = sAwardId;
                    var liAward = (List<IQAC_AWARDS>)CMSHandler.FetchData<IQAC_AWARDS>(ObjModel,
                            IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAwardsById), sAcademicYear).DataSource.Data;
                    if (liAward != null && liAward.Count > 0)
                    {
                        return Json(liAward, JsonRequestBehavior.AllowGet);
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
                        ObjLog.WriteError("IQACController", "FetchAwardById", ex.Message);
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
        public JsonResult SaveAward()
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //var s = Request.Form["sJsonData"];
                    var Result = JsonConvert.DeserializeObject<IQAC_AWARDS>(Request.Form["sJsonData"]);
                    Result.STAFF_ID = sStaffId;
                    Result.MONTH_ID = "0";
                    if (!string.IsNullOrEmpty(Result.AWARD_ID))
                    {
                        // Update ,....
                        var UpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateAwards), sAcademicYear);
                        if (UpdateQuery.Success)
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
                        // Insert ,...
                        var InsertQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveAwards), sAcademicYear, true);
                        if (InsertQuery.Success)
                        {
                            Result.AWARD_ID = InsertQuery.RowUniqueId.ToString();
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }

                    string simgPath = Request.Form["sPhoto"];
                    if (!string.IsNullOrEmpty(Result.FILE_PATH) || !string.IsNullOrEmpty(simgPath))
                    {
                        string sFilePath = string.Empty;

                        //update file 

                        if (!string.IsNullOrEmpty(simgPath))
                        {
                            //for upload img path
                            var imgPath = simgPath.Split(',');
                            foreach (var item2 in imgPath)
                            {
                                sFilePath += DecryptString(item2) + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(Result.FILE_PATH))
                        {
                            //for add img 
                            int Count = 0;
                            var fmgPath = Result.FILE_PATH.Split(',');
                            foreach (var item2 in fmgPath)
                            {
                                string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "Awards" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "Awards" + "\\" + sStaffId);
                                sFilePath += sPath + ","; Count++;
                            }
                        }
                        if (!string.IsNullOrEmpty(sFilePath))
                        {
                            Result.FILE_PATH = sFilePath.TrimEnd(',');
                        }


                        var FileUpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateAwardFile), sAcademicYear);

                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveAward", ex.Message);
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
        public JsonResult FetchAwards()
        {
            var ObjModel = new IQAC_AWARDS();
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    var liAward = (List<IQAC_AWARDS>)CMSHandler.FetchData<IQAC_AWARDS>(ObjModel,
                            IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAwardsList), sAcademicYear).DataSource.Data;
                    if (liAward != null && liAward.Count > 0)
                    {
                        return Json(liAward, JsonRequestBehavior.AllowGet);
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
                        ObjLog.WriteError("IQACController", "FetchAwards", ex.Message);
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
        public JsonResult DeleteAwards(string sAwardId)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_AWARDS();
                    ObjModel.AWARD_ID = sAwardId;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteAwards));
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
                        ObjLog.WriteError("IQACController", "DeleteAwards", ex.Message);
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

        // Subject Association ,...
        public JsonResult FetchSubjectAssociationById(string sSubjectAssociationId)
        {
            var ObjModel = new IQAC_SUBJECT_ASSOCIATION();
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.SUBJECT_ASSOCIATION_ID = sSubjectAssociationId;
                    var liSubjectAssociation = (List<IQAC_SUBJECT_ASSOCIATION>)CMSHandler.FetchData<IQAC_SUBJECT_ASSOCIATION>(ObjModel,
                            IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSubjectAssociationById), sAcademicYear).DataSource.Data;
                    if (liSubjectAssociation != null && liSubjectAssociation.Count > 0)
                    {
                        return Json(liSubjectAssociation, JsonRequestBehavior.AllowGet);
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
                        ObjLog.WriteError("IQACController", "BindSubjectAssociation", ex.Message);
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
        public JsonResult SaveSubjectAssociation()
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<IQAC_SUBJECT_ASSOCIATION>(Request.Form["sJsonData"]);
                    Result.STAFF_ID = sStaffId;
                    if (!string.IsNullOrEmpty(Result.SUBJECT_ASSOCIATION_ID))
                    {
                        // Update ,....
                        var UpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateSubjectAssociation), sAcademicYear);
                        if (UpdateQuery.Success)
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
                        // Insert ,...
                        var InsertQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveSubjectAssociation), sAcademicYear, true);
                        if (InsertQuery.Success)
                        {
                            Result.SUBJECT_ASSOCIATION_ID = InsertQuery.RowUniqueId.ToString();
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }

                    string simgPath = Request.Form["sPhoto"];
                    if (!string.IsNullOrEmpty(Result.FILE_PATH) || !string.IsNullOrEmpty(simgPath))
                    {
                        string sFilePath = string.Empty;

                        //update file 

                        if (!string.IsNullOrEmpty(simgPath))
                        {
                            //for upload img path
                            var imgPath = simgPath.Split(',');
                            foreach (var item2 in imgPath)
                            {
                                sFilePath += DecryptString(item2) + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(Result.FILE_PATH))
                        {
                            //for add img 
                            int Count = 0;
                            var fmgPath = Result.FILE_PATH.Split(',');
                            foreach (var item2 in fmgPath)
                            {
                                string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "subjectassociation" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "subjectassociation" + "\\" + sStaffId);
                                sFilePath += sPath + ","; Count++;
                            }
                        }
                        if (!string.IsNullOrEmpty(sFilePath))
                        {
                            Result.FILE_PATH = sFilePath.TrimEnd(',');
                        }


                        var FileUpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateSubjectAssociationFile), sAcademicYear);

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveSubjectAssociation", ex.Message);
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
        public JsonResult FetchSubjectAssociation()
        {
            var ObjModel = new IQAC_AWARDS();
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    var liModel = (List<IQAC_SUBJECT_ASSOCIATION>)CMSHandler.FetchData<IQAC_SUBJECT_ASSOCIATION>(ObjModel,
                            IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSubjectAssociationList), sAcademicYear).DataSource.Data;
                    if (liModel != null && liModel.Count > 0)
                    {
                        return Json(liModel, JsonRequestBehavior.AllowGet);
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
                        ObjLog.WriteError("IQACController", "FetchSubjectAssociation", ex.Message);
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
        public JsonResult DeleteSubjectAssociation(string sSubjectAssociationId)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_SUBJECT_ASSOCIATION();
                    ObjModel.SUBJECT_ASSOCIATION_ID = sSubjectAssociationId;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteSubjectAssociation));
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
                        ObjLog.WriteError("IQACController", "DeleteSubjectAssociation", ex.Message);
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

        // Honor ,..
        public JsonResult FetchHonorById(string sHonorId)
        {
            var ObjModel = new IQAC_HONOURS();
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.HONOR_ID = sHonorId;
                    var liHonor = (List<IQAC_HONOURS>)CMSHandler.FetchData<IQAC_HONOURS>(ObjModel,
                            IQACSQL.GetIQACSQL(IQACEnumCommands.FetchHonorById), sAcademicYear).DataSource.Data;
                    if (liHonor != null && liHonor.Count > 0)
                    {
                        ObjViewModel.liHonor = liHonor;
                        return Json(liHonor, JsonRequestBehavior.AllowGet);
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
                        ObjLog.WriteError("IQACController", "FetchHonorById", ex.Message);
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
        public JsonResult SaveHonors()
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<IQAC_HONOURS>(Request.Form["sJsonData"]);
                    Result.STAFF_ID = sStaffId;
                    Result.FROM_DATE = CommonMethods.ConvertdatetoyearFromat(Result.FROM_DATE);
                    Result.TO_DATE = CommonMethods.ConvertdatetoyearFromat(Result.TO_DATE);
                    if (!string.IsNullOrEmpty(Result.HONOR_ID))
                    {
                        // Update ,....
                        var UpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateHonor), sAcademicYear);
                        if (UpdateQuery.Success)
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
                        // Insert ,...
                        var InsertQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveHonor), sAcademicYear, true);
                        if (InsertQuery.Success)
                        {
                            Result.HONOR_ID = InsertQuery.RowUniqueId.ToString();
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }

                    string simgPath = Request.Form["sPhoto"];
                    if (!string.IsNullOrEmpty(Result.FILE_PATH) || !string.IsNullOrEmpty(simgPath))
                    {
                        string sFilePath = string.Empty;

                        //update file 

                        if (!string.IsNullOrEmpty(simgPath))
                        {
                            //for upload img path
                            var imgPath = simgPath.Split(',');
                            foreach (var item2 in imgPath)
                            {
                                sFilePath += DecryptString(item2) + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(Result.FILE_PATH))
                        {
                            //for add img 
                            int Count = 0;
                            var fmgPath = Result.FILE_PATH.Split(',');
                            foreach (var item2 in fmgPath)
                            {
                                string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "honors" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "honors" + "\\" + sStaffId);
                                sFilePath += sPath + ","; Count++;
                            }
                        }
                        if (!string.IsNullOrEmpty(sFilePath))
                        {
                            Result.FILE_PATH = sFilePath.TrimEnd(',');
                        }


                        var FileUpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateHonorFile), sAcademicYear);

                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveHonors", ex.Message);
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
        public JsonResult FetchHonor()
        {
            var ObjModel = new IQAC_AWARDS();
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    var liModel = (List<IQAC_HONOURS>)CMSHandler.FetchData<IQAC_HONOURS>(ObjModel,
                            IQACSQL.GetIQACSQL(IQACEnumCommands.FetchHonorList), sAcademicYear).DataSource.Data;
                    if (liModel != null && liModel.Count > 0)
                    {
                        return Json(liModel, JsonRequestBehavior.AllowGet);
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
                        ObjLog.WriteError("IQACController", "FetchHonor", ex.Message);
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
        public JsonResult DeleteHonor(string sHonorId)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_HONOURS();
                    ObjModel.HONOR_ID = sHonorId;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteHonor));
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
                        ObjLog.WriteError("IQACController", "DeleteHonor", ex.Message);
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

        // MemberShip ,..
        public JsonResult FetchMemberShipById(string sMemberShipId)
        {
            var ObjModel = new IQAC_MEMBERSHIP();
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.MEMBERSHIP_ID = sMemberShipId;
                    var liModel = (List<IQAC_MEMBERSHIP>)CMSHandler.FetchData<IQAC_MEMBERSHIP>(ObjModel,
                            IQACSQL.GetIQACSQL(IQACEnumCommands.FetchMembershipById), sAcademicYear).DataSource.Data;
                    if (liModel != null && liModel.Count > 0)
                    {
                        return Json(liModel, JsonRequestBehavior.AllowGet);
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
                        ObjLog.WriteError("IQACController", "FetchMemberShipById", ex.Message);
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
        public JsonResult SaveMemberShip()
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<IQAC_MEMBERSHIP>(Request.Form["sJsonData"]);
                    Result.STAFF_ID = sStaffId;
                    Result.MONTH_ID = "0";
                    if (!string.IsNullOrEmpty(Result.MEMBERSHIP_ID))
                    {
                        // Update ,....
                        var UpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateMembership), sAcademicYear);
                        if (UpdateQuery.Success)
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
                        // Insert ,...
                        var InsertQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveMembership), sAcademicYear, true);
                        if (InsertQuery.Success)
                        {
                            Result.MEMBERSHIP_ID = InsertQuery.RowUniqueId.ToString();
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                    string simgPath = Request.Form["sPhoto"];
                    if (!string.IsNullOrEmpty(Result.FILE_PATH) || !string.IsNullOrEmpty(simgPath))
                    {
                        string sFilePath = string.Empty;

                        //update file 

                        if (!string.IsNullOrEmpty(simgPath))
                        {
                            //for upload img path
                            var imgPath = simgPath.Split(',');
                            foreach (var item2 in imgPath)
                            {
                                sFilePath += DecryptString(item2) + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(Result.FILE_PATH))
                        {
                            //for add img 
                            int Count = 0;
                            var fmgPath = Result.FILE_PATH.Split(',');
                            foreach (var item2 in fmgPath)
                            {
                                string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "Membership" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "Membership" + "\\" + sStaffId);
                                sFilePath += sPath + ","; Count++;
                            }
                        }
                        if (!string.IsNullOrEmpty(sFilePath))
                        {
                            Result.FILE_PATH = sFilePath.TrimEnd(',');
                        }


                        var FileUpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateMembershipFilePath), sAcademicYear);

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveMemberShip", ex.Message);
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
        public JsonResult FetchMemberShip()
        {
            var ObjModel = new IQAC_AWARDS();
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    var liModel = (List<IQAC_MEMBERSHIP>)CMSHandler.FetchData<IQAC_MEMBERSHIP>(ObjModel,
                            IQACSQL.GetIQACSQL(IQACEnumCommands.FetchMembershipList), sAcademicYear).DataSource.Data;
                    if (liModel != null && liModel.Count > 0)
                    {
                        return Json(liModel, JsonRequestBehavior.AllowGet);
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
                        ObjLog.WriteError("IQACController", "FetchMemberShip", ex.Message);
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
        public JsonResult DeleteMemberShip(string sMemberShipId)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_MEMBERSHIP();
                    ObjModel.MEMBERSHIP_ID = sMemberShipId;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteMembership));
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
                        ObjLog.WriteError("IQACController", "DeleteMemberShip", ex.Message);
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


        // File Save ,...
        public string SaveFile(int Count, string sDirectory, string sSaveDirectory)
        {
            string sDirectorypath = string.Empty, sFilePath = string.Empty, sFileName = string.Empty, sSavePath = string.Empty;
            var path = string.Empty;

            if (!string.IsNullOrEmpty(sDirectory))
            {
                try
                {
                    sDirectorypath = sDirectory;
                    // AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.ALUMNAE_PASSEDOUT_IMAGE_PATH + "\\" + sFolder;
                    if (!Directory.Exists(sDirectorypath))
                    {
                        Directory.CreateDirectory(sDirectorypath);
                    }
                    var file = Request.Files[Count];
                    sFileName = file.FileName;
                    var IPFile = sFileName.Split('.');
                    string sPath = "\\" + DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss") + "_" + IPFile[0].ToUpper() + "." + IPFile[1];
                    sFilePath = sDirectorypath + sPath;
                    sSavePath = sSaveDirectory + sPath;
                    file.SaveAs(sFilePath);
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveFile", ex.Message);
                    }
                }
            }
            return sSavePath;
        }

        #endregion

        #region 9. Project & Research Gudience
        // Project 
        public ActionResult ProjectList()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var objViewModel = new IQACViewModel();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                int nAcademicYear = Convert.ToInt32(sAcademicYear);
                nAcademicYear--;
                sAcademicYear = sAcademicYear + "," + nAcademicYear.ToString();

                var ObjModel = new IQAC_PROJECT();
                ObjModel.STAFF_ID = sStaffId;
                var liIQACProject = (List<IQAC_PROJECT>)CMSHandler.FetchData<IQAC_PROJECT>(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchIQACProjects), sAcademicYear).DataSource.Data;

                if (liIQACProject != null && liIQACProject.Count > 0)
                {
                    objViewModel.liIQACProject = liIQACProject;
                }


                // fetch project type
                var ProjectType = (List<SUP_IQAC_PROJECT_TYPE>)CMS.DAO.CMSHandler.FetchData<SUP_IQAC_PROJECT_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchSupIQACProjectType), sAcademicYear).DataSource.Data;
                if (ProjectType != null && ProjectType.Count > 0)
                {
                    objViewModel.sIQACProjecttype = new SelectList(ProjectType, Common.SUP_IQAC_PROJECT_TYPE.PROJECT_TYPE_ID, Common.SUP_IQAC_PROJECT_TYPE.PROJECT_TYPE);
                }

                //fetch type of funding 
                var TypeOfFunding = (List<SUP_IQAC_TYPE_OF_FUNDING>)CMS.DAO.CMSHandler.FetchData<SUP_IQAC_TYPE_OF_FUNDING>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchSupIQACTypeOfFunding), sAcademicYear).DataSource.Data;
                if (TypeOfFunding != null && TypeOfFunding.Count > 0)
                {
                    objViewModel.sIQACTypeofFunding = new SelectList(TypeOfFunding, Common.SUP_IQAC_TYPE_OF_FUNDING.TYPE_FUNDING_ID, Common.SUP_IQAC_TYPE_OF_FUNDING.FUNDING_TYPE);
                }



            }
            return View(objViewModel);
        }

        public JsonResult FetchProjectById(string sIQACID)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objResultData.oResult = (List<IQAC_PROJECT>)CMS.DAO.CMSHandler.FetchData<IQAC_PROJECT>(new IQAC_PROJECT() { IQAC_PROJECT_ID = sIQACID }, SQL.IQAC.IQACSQL.GetIQACSQL(DAO.IQACEnumCommands.FetchIQACProjectsById), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "FetchProjectById", ex.Message);
                    }
                    objResultData.Message = Common.ErrorMessage.InternalServerError;
                    objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                objResultData.Message = Common.ErrorMessage.InternalServerError;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveProject()
        {
            ResultArgs resultArgs = new ResultArgs();
            var objResultData = new JsonResultData();
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sJsonData = Request.Form["sJsonData"];
                    var result = JsonConvert.DeserializeObject<IQAC_PROJECT>(sJsonData);
                    if (result != null)
                    {
                        result.STAFF_ID = sStaffId;
                        if (!string.IsNullOrEmpty(result.IQAC_PROJECT_ID))
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateIQACProjectsById), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.SaveIQACProjects), sAcademicYear, true);
                            result.IQAC_PROJECT_ID = resultArgs.RowUniqueId.ToString();
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }


                        string simgPath = Request.Form["sPhoto"];
                        if (!string.IsNullOrEmpty(result.FILE_PATH) || !string.IsNullOrEmpty(simgPath))
                        {
                            string sFilePath = string.Empty;

                            //update file 
                            if (!string.IsNullOrEmpty(simgPath))
                            {
                                //for upload img path
                                var imgPath = simgPath.Split(',');
                                foreach (var item2 in imgPath)
                                {
                                    sFilePath += DecryptString(item2) + ",";
                                }
                            }
                            if (!string.IsNullOrEmpty(result.FILE_PATH))
                            {
                                //for add img 
                                int Count = 0;
                                var fmgPath = result.FILE_PATH.Split(',');
                                foreach (var item2 in fmgPath)
                                {
                                    string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "Project" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "Project" + "\\" + sStaffId);
                                    sFilePath += sPath + ","; Count++;
                                }
                            }
                            if (!string.IsNullOrEmpty(sFilePath))
                            {
                                result.FILE_PATH = sFilePath.TrimEnd(',');
                            }
                            var UpdateSaveFile = CMSHandler.SaveOrUpdate(result, SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateIQACProjectsFilePathById), sAcademicYear);
                            //    if (!string.IsNullOrEmpty(result.IQAC_PROJECT_ID) && Request.Files.Count > 0)
                            //    {
                            //    int nCount = 0;
                            //    string sFilePath = string.Empty;
                            //    foreach (var item2 in Request.Files)
                            //    {
                            //        string sPath = SaveFile(nCount, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "Project" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "Project" + "\\" + sStaffId);
                            //        sFilePath += sPath + ","; nCount++;
                            //    }
                            //    result.FILE_PATH = sFilePath.TrimEnd(',');
                            //    var UpdateSaveFile = CMSHandler.SaveOrUpdate(result, SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateIQACProjectsFilePathById), sAcademicYear);
                            //}
                        }

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveProject", ex.Message);
                    }

                    objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            if (string.IsNullOrEmpty(objResultData.Message))
            {
                objResultData.Message = "No Changes Made";
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteProject(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_PROJECT();
                    ObjModel.IQAC_PROJECT_ID = sIQACID;
                    ObjModel.IS_DELETED = Common.DeleteFlag.Deleted;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeletedIQACProjectsById));
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

        // Research ,...
        public ActionResult ResearchGudienceList()
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var objViewModel = new IQACViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                int nAcademicYear = Convert.ToInt32(sAcademicYear);
                nAcademicYear--;
                sAcademicYear = sAcademicYear + "," + nAcademicYear.ToString();

                var ObjModel = new IQAC_PROJECT();
                ObjModel.STAFF_ID = sStaffId;
                var liIQACResearchGuidence = (List<IQAC_RESEARCH_GUIDENCE>)CMSHandler.FetchData<IQAC_RESEARCH_GUIDENCE>(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchIQACResearchGuidence), sAcademicYear).DataSource.Data;

                if (liIQACResearchGuidence != null && liIQACResearchGuidence.Count > 0)
                {
                    objViewModel.liIQACResearchGuidence = liIQACResearchGuidence;
                }


                // fetch IQAC research CATEGORY
                var IQACCategory = (List<SUP_IQAC_PHD_RESEARCH_CATEGORY>)CMS.DAO.CMSHandler.FetchData<SUP_IQAC_PHD_RESEARCH_CATEGORY>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchSupIQACResearchGuidenceCategory), sAcademicYear).DataSource.Data;
                if (IQACCategory != null && IQACCategory.Count > 0)
                {
                    objViewModel.sIQACCategory = new SelectList(IQACCategory, Common.SUP_IQAC_PHD_RESEARCH_CATEGORY.IQAC_CATEGORY_ID, Common.SUP_IQAC_PHD_RESEARCH_CATEGORY.CATEGORY_NAME);
                }




            }
            return View(objViewModel);
        }

        public JsonResult FetchResearchGudienceById(string sIQACID)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    objResultData.oResult = (List<IQAC_RESEARCH_GUIDENCE>)CMS.DAO.CMSHandler.FetchData<IQAC_RESEARCH_GUIDENCE>(new IQAC_RESEARCH_GUIDENCE() { IQAC_RESEARCH_GUDIENCE_ID = sIQACID }, SQL.IQAC.IQACSQL.GetIQACSQL(DAO.IQACEnumCommands.FetchIQACResearchGuidenceById), sAcademicYear).DataSource.Data;


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "FetchResearchGudienceById", ex.Message);
                    }
                    objResultData.Message = Common.ErrorMessage.InternalServerError;
                    objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                objResultData.Message = Common.ErrorMessage.InternalServerError;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveResearchGudience()
        {
            ResultArgs resultArgs = new ResultArgs();
            var objResultData = new JsonResultData();
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sJsonData = Request.Form["sJsonData"];
                    var result = JsonConvert.DeserializeObject<IQAC_RESEARCH_GUIDENCE>(sJsonData);
                    if (result != null)
                    {

                        if (!string.IsNullOrEmpty(result.IQAC_RESEARCH_GUDIENCE_ID))
                        {
                            result.STAFF_ID = sStaffId;
                            resultArgs = CMSHandler.SaveOrUpdate(result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateIQACResearchGuidenceById), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            result.STAFF_ID = sStaffId;
                            resultArgs = CMSHandler.SaveOrUpdate(result, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveIQACResearchGuidence), sAcademicYear, true);
                            result.IQAC_RESEARCH_GUDIENCE_ID = resultArgs.RowUniqueId.ToString();
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }

                        string simgPath = Request.Form["sPhoto"];
                        if (!string.IsNullOrEmpty(result.FILE_PATH) || !string.IsNullOrEmpty(simgPath))
                        {
                            string sFilePath = string.Empty;

                            //update file 
                            if (!string.IsNullOrEmpty(simgPath))
                            {
                                //for upload img path
                                var imgPath = simgPath.Split(',');
                                foreach (var item2 in imgPath)
                                {
                                    sFilePath += DecryptString(item2) + ",";
                                }
                            }
                            if (!string.IsNullOrEmpty(result.FILE_PATH))
                            {
                                //for add img 
                                int Count = 0;
                                var fmgPath = result.FILE_PATH.Split(',');
                                foreach (var item2 in fmgPath)
                                {
                                    string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "Research_Guidence" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "Research_Guidence" + "\\" + sStaffId);
                                    sFilePath += sPath + ","; Count++;
                                }
                            }
                            if (!string.IsNullOrEmpty(sFilePath))
                            {
                                result.FILE_PATH = sFilePath.TrimEnd(',');
                            }
                            var UpdateSaveFile = CMSHandler.SaveOrUpdate(result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateIQACResearchGuidenceFilePathById), sAcademicYear);
                        }   
                            //    if (!string.IsNullOrEmpty(result.IQAC_RESEARCH_GUDIENCE_ID) && Request.Files.Count > 0)
                            //    {
                            //    int nCount = 0;
                            //    string sFilePath = string.Empty;
                            //    foreach (var item2 in Request.Files)
                            //    {
                            //        string sPath = SaveFile(nCount, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "Research_Guidence" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "Research_Guidence" + "\\" + sStaffId);
                            //        sFilePath += sPath + ","; nCount++;
                            //    }
                            //    result.FILE_PATH = sFilePath.TrimEnd(',');
                            //    var UpdateSaveFile = CMSHandler.SaveOrUpdate(result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateIQACResearchGuidenceFilePathById), sAcademicYear);
                            //}
                        }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveResearchGudience", ex.Message);
                    }

                    objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteResearchGudience(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_RESEARCH_GUIDENCE();
                    ObjModel.IQAC_RESEARCH_GUDIENCE_ID = sIQACID;
                    ObjModel.IS_DELETED = Common.DeleteFlag.Deleted;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeletedIQACResearchGuidenceById));
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
                        ObjLog.WriteError("IQACController", "DeleteResearchGudience", ex.Message);
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


        // PHD Guidence ,,...
        public ActionResult PHDGuidenceList()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var objViewModel = new IQACViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                int nAcademicYear = Convert.ToInt32(sAcademicYear);
                nAcademicYear--;
                sAcademicYear = sAcademicYear + "," + nAcademicYear.ToString();

                var ObjModel = new IQAC_PROJECT();
                ObjModel.STAFF_ID = sStaffId;
                objViewModel.liIQACPhdGuidence = (List<IQAC_PHD_GUIDENCE>)CMSHandler.FetchData<IQAC_PHD_GUIDENCE>(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchIQACPhdGuidence), sAcademicYear).DataSource.Data;



                // fetch IQAC phd CATEGORY
                var IQACCategory = (List<SUP_IQAC_PHD_GUIDENCE_CATEGORY>)CMS.DAO.CMSHandler.FetchData<SUP_IQAC_PHD_GUIDENCE_CATEGORY>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchSupIQACPhdGuidenceCategory), sAcademicYear).DataSource.Data;
                if (IQACCategory != null && IQACCategory.Count > 0)
                {
                    objViewModel.sIQACCategory = new SelectList(IQACCategory, Common.SUP_IQAC_PHD_GUIDENCE_CATEGORY.IQAC_CATEGORY_ID, Common.SUP_IQAC_PHD_GUIDENCE_CATEGORY.CATEGORY_NAME);
                }

                // Status
                var liStatus = (List<SUP_IQAC_STATUS>)CMS.DAO.CMSHandler.FetchData<SUP_IQAC_STATUS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchIQACStatus), sAcademicYear).DataSource.Data;
                if (liStatus != null && liStatus.Count > 0)
                {
                    objViewModel.sStatusList = new SelectList(liStatus, Common.SUP_IQAC_STATUS.IQAC_STATUS_ID, Common.SUP_IQAC_STATUS.STATUS_NAME);
                }
            }
            return View(objViewModel);
        }

        public JsonResult FetchPHDGuidenceById(string sIQACID)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    objResultData.oResult = (List<IQAC_PHD_GUIDENCE>)CMS.DAO.CMSHandler.FetchData<IQAC_PHD_GUIDENCE>(new IQAC_PHD_GUIDENCE() { IQAC_PHD_GUDIENCE_ID = sIQACID }, SQL.IQAC.IQACSQL.GetIQACSQL(DAO.IQACEnumCommands.FetchIQACPhdGuidenceById), sAcademicYear).DataSource.Data;


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "FetchPHDGuidenceById", ex.Message);
                    }
                    objResultData.Message = Common.ErrorMessage.InternalServerError;
                    objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                objResultData.Message = Common.ErrorMessage.InternalServerError;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePHDGuidence()
        {
            ResultArgs resultArgs = new ResultArgs();
            var objResultData = new JsonResultData();
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sJsonData = Request.Form["sJsonData"];
                    var result = JsonConvert.DeserializeObject<IQAC_PHD_GUIDENCE>(sJsonData);
                    if (result != null)
                    {
                        // formating date
                        if (!string.IsNullOrEmpty(result.GUDIENCE_DATE))
                        {
                            result.GUDIENCE_DATE = CommonMethods.ConvertdatetoyearFromat(result.GUDIENCE_DATE);
                            result.MINI_GUDIENCE_DATE = CommonMethods.ConvertdatetoyearFromat(result.MINI_GUDIENCE_DATE);
                            result.MAXI_GUDIENCE_DATE = CommonMethods.ConvertdatetoyearFromat(result.MAXI_GUDIENCE_DATE);
                        }


                        if (!string.IsNullOrEmpty(result.IQAC_PHD_GUDIENCE_ID))
                        {
                            result.STAFF_ID = sStaffId;
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateIQACPhdGuidenceById), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            result.STAFF_ID = sStaffId;
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.SaveIQACPhdGuidence), sAcademicYear, true);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                            result.IQAC_PHD_GUDIENCE_ID = resultArgs.RowUniqueId.ToString();
                        }


                        string simgPath = Request.Form["sPhoto"];
                        if (!string.IsNullOrEmpty(result.FILE_PATH) || !string.IsNullOrEmpty(simgPath))
                        {
                            string sFilePath = string.Empty;

                            //update file 
                            if (!string.IsNullOrEmpty(simgPath))
                            {
                                //for upload img path
                                var imgPath = simgPath.Split(',');
                                foreach (var item2 in imgPath)
                                {
                                    sFilePath += DecryptString(item2) + ",";
                                }
                            }
                            if (!string.IsNullOrEmpty(result.FILE_PATH))
                            {
                                //for add img 
                                int Count = 0;
                                var fmgPath = result.FILE_PATH.Split(',');
                                foreach (var item2 in fmgPath)
                                {
                                    string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "PHD_Guidence" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "PHD_Guidence" + "\\" + sStaffId);
                                    sFilePath += sPath + ","; Count++;
                                }
                            }
                            if (!string.IsNullOrEmpty(sFilePath))
                            {
                                result.FILE_PATH = sFilePath.TrimEnd(',');
                            }
                            var UpdateSaveFile = CMSHandler.SaveOrUpdate(result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateIQACPhdGuidenceFilePathById), sAcademicYear);
                        }

                        //if (!string.IsNullOrEmpty(result.IQAC_PHD_GUDIENCE_ID) && Request.Files.Count > 0)
                        //{
                        //    int nCount = 0;
                        //    string sFilePath = string.Empty;
                        //    foreach (var item2 in Request.Files)
                        //    {
                        //        string sPath = SaveFile(nCount, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "PHD_Guidence" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "PHD_Guidence" + "\\" + sStaffId);
                        //        sFilePath += sPath + ","; nCount++;
                        //    }
                        //    result.FILE_PATH = sFilePath.TrimEnd(',');
                        //    var UpdateSaveFile = CMSHandler.SaveOrUpdate(result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateIQACPhdGuidenceFilePathById), sAcademicYear);
                        //}
                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SavePHDGuidence", ex.Message);
                    }

                    objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletePHDGuidence(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_PHD_GUIDENCE();
                    ObjModel.IQAC_PHD_GUDIENCE_ID = sIQACID;
                    ObjModel.IS_DELETED = Common.DeleteFlag.Deleted;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeletedIQACPhdGuidenceById));
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
                        ObjLog.WriteError("IQACController", "DeletePHDGuidence", ex.Message);
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

        #region 10 . Publication & Prasentation ( Staff Activity )

        // Publication & Prasentation ,....
        public ActionResult PublicationAndPrasentationList()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    int nAcademicYear = Convert.ToInt32(sAcademicYear);
                    nAcademicYear--;
                    sAcademicYear = sAcademicYear + "," + nAcademicYear.ToString();

                    var ObjModel = new IQAC_STAFF_ACTIVITY();
                    ObjModel.STAFF_ID = sStaffId;
                    ObjViewModel.liStaffActivity = (List<IQAC_STAFF_ACTIVITY>)CMSHandler.FetchData<IQAC_STAFF_ACTIVITY>(ObjModel,
                                    IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPublicationAndPrasentationList), sAcademicYear).DataSource.Data;
                    var liProgrammeType = (List<SUP_PROGRAMME_TYPE>)CMSHandler.FetchData<SUP_PROGRAMME_TYPE>(null,
                                    SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeType)).DataSource.Data;
                    if (liProgrammeType != null && liProgrammeType.Count > 0)
                    {
                        ObjViewModel.sProgrammeTypeList = new SelectList(liProgrammeType, Common.SUP_PROGRAMME_TYPE.PROGRAMME_TYPE_ID, Common.SUP_PROGRAMME_TYPE.PROGRAMME_TYPE);
                    }

                    var liLevel = (List<sup_Level>)CMSHandler.FetchData<sup_Level>(null,
                                    SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLevel)).DataSource.Data;
                    if (liLevel != null && liLevel.Count > 0)
                    {
                        ObjViewModel.sLevelList = new SelectList(liLevel, Common.sup_LEVEL.LEVEL_ID, Common.sup_LEVEL.LEVEL);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "PublicationAndPrasentationList", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Message");
            }
            return View(ObjViewModel);
        }

        public JsonResult FetchPublicationAndPrasentationById(string sActivityId)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_STAFF_ACTIVITY();
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.IQAC_STAFF_ACTIVITY_ID = sActivityId;
                    var liModel = (List<IQAC_STAFF_ACTIVITY>)CMSHandler.FetchData<IQAC_STAFF_ACTIVITY>(ObjModel,
                                IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPublicationAndPrasentationById), sAcademicYear).DataSource.Data;
                    if (liModel != null && liModel.Count > 0)
                    {
                        return Json(liModel, JsonRequestBehavior.AllowGet);
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
                        ObjLog.WriteError("IQACController", "FetchPublicationAndPrasentationById", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePublicationAndPrasentation()
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sJsonData = Request.Form["sJsonData"];
                    var Result = JsonConvert.DeserializeObject<IQAC_STAFF_ACTIVITY>(sJsonData);
                    Result.STAFF_ID = sStaffId;
                    Result.ACTIVITY_DATE = CommonMethods.ConvertdatetoyearFromat(Result.ACTIVITY_DATE);
                    Result.ACTIVITY_DATE_TO = CommonMethods.ConvertdatetoyearFromat(Result.ACTIVITY_DATE_TO);
                    if (!string.IsNullOrEmpty(Result.IQAC_STAFF_ACTIVITY_ID))
                    {
                        // Update ,.....

                        var UpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdatePublicationAndPrasentation), sAcademicYear);
                        if (UpdateQuery.Success)
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
                        // Insert ,......
                        var SaveQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.SavePublicationAndPrasentation), sAcademicYear, true);
                        if (SaveQuery.Success)
                        {
                            Result.IQAC_STAFF_ACTIVITY_ID = SaveQuery.RowUniqueId.ToString();
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }



                        //if (!string.IsNullOrEmpty(Result.IQAC_STAFF_ACTIVITY_ID) && Request.Files.Count > 0)
                        //{
                        //    int nCount = 0;
                        //    string sFilePath = string.Empty;
                        //    foreach (var item2 in Request.Files)
                        //    {
                        //        string sPath = SaveFile(nCount, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "ParticipationAndPrasentation" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "ParticipationAndPrasentation" + "\\" + sStaffId);
                        //        sFilePath += sPath + ","; nCount++;
                        //    }
                        //    Result.FILE_PATH = sFilePath;
                        //    var UpdateSaveFile = CMSHandler.SaveOrUpdate(Result, SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.UpdatePublicationAndPrasentationFilePath), sAcademicYear);
                        //}
                    }

                    string simgPath = Request.Form["sPhoto"];
                    if (!string.IsNullOrEmpty(Result.FILE_PATH) || !string.IsNullOrEmpty(simgPath))
                    {
                        string sFilePath = string.Empty;

                        //update file 
                        if (!string.IsNullOrEmpty(simgPath))
                        {
                            //for upload img path
                            var imgPath = simgPath.Split(',');
                            foreach (var item2 in imgPath)
                            {
                                sFilePath += DecryptString(item2) + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(Result.FILE_PATH))
                        {
                            //for add img 
                            int Count = 0;
                            var fmgPath = Result.FILE_PATH.Split(',');
                            foreach (var item2 in fmgPath)
                            {
                                string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "ParticipationAndPrasentation" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "ParticipationAndPrasentation" + "\\" + sStaffId);
                                sFilePath += sPath + ","; Count++;
                            }
                        }
                        if (!string.IsNullOrEmpty(sFilePath))
                        {
                            Result.FILE_PATH = sFilePath.TrimEnd(',');
                        }
                        var UpdateSaveFile = CMSHandler.SaveOrUpdate(Result, SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.UpdatePublicationAndPrasentationFilePath), sAcademicYear);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SavePublicationAndPrasentation", ex.Message);
                    }

                    ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
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

        public JsonResult DeletePublicationAndPrasentation(string sActivityId)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_STAFF_ACTIVITY();
                    ObjModel.IQAC_STAFF_ACTIVITY_ID = sActivityId;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeletePublicationAndPrasentation));
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
                        ObjLog.WriteError("IQACController", "DeletePublicationAndPrasentation", ex.Message);
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

        // Paper Publication ,...
        public ActionResult PaperPublicationList()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    int nAcademicYear = Convert.ToInt32(sAcademicYear);
                    nAcademicYear--;
                    sAcademicYear = sAcademicYear + "," + nAcademicYear.ToString();

                    var ObjModel = new IQAC_STAFF_ACTIVITY();
                    ObjModel.STAFF_ID = sStaffId;
                    ObjViewModel.liPaperPublication = (List<IQAC_PAPER_PUBLICATION>)CMSHandler.FetchData<IQAC_PAPER_PUBLICATION>(ObjModel,
                                    IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPaperPublicationList), sAcademicYear).DataSource.Data;

                    ObjViewModel.liSupMonthList = (List<SUP_MONTHS>)CMSHandler.FetchData<SUP_MONTHS>(ObjModel,
                                    IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSupMonths), sAcademicYear).DataSource.Data;                 
                }


                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "PaperPublicationList", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Message");
            }
            return View(ObjViewModel);
        }

        public JsonResult FetchPaperPublicationById(string sPaperPublicationId)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_PAPER_PUBLICATION();
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.IQAC_PAPER_PUBLICATION_ID = sPaperPublicationId;
                    var liModel = (List<IQAC_PAPER_PUBLICATION>)CMSHandler.FetchData<IQAC_PAPER_PUBLICATION>(ObjModel,
                                IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPaperPublicationById), sAcademicYear).DataSource.Data;
                    if (liModel != null && liModel.Count > 0)
                    {
                        return Json(liModel, JsonRequestBehavior.AllowGet);
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
                        ObjLog.WriteError("IQACController", "FetchPaperPublicationById", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePaperPublication()
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sJsonData = Request.Form["sJsonData"];
                    var Result = JsonConvert.DeserializeObject<IQAC_PAPER_PUBLICATION>(sJsonData);
                    Result.STAFF_ID = sStaffId;
                    if (!string.IsNullOrEmpty(Result.IQAC_PAPER_PUBLICATION_ID))
                    {
                        // Update ,.....
                        var UpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdatePaperPublication), sAcademicYear);
                        if (UpdateQuery.Success)
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
                        // Insert ,......
                        var SaveQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.SavePaperPublication), sAcademicYear, true);
                        if (SaveQuery.Success)
                        {
                            Result.IQAC_PAPER_PUBLICATION_ID = SaveQuery.RowUniqueId.ToString();
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }

                        //if (!string.IsNullOrEmpty(Result.IQAC_PAPER_PUBLICATION_ID) && Request.Files.Count > 0)
                        //{
                        //    int nCount = 0;
                        //    string sFilePath = string.Empty;
                        //    foreach (var item2 in Request.Files)
                        //    {
                        //        string sPath = SaveFile(nCount, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "PaperPublication" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "PaperPublication" + "\\" + sStaffId);
                        //        sFilePath += sPath + ","; nCount++;
                        //    }
                        //    Result.FILE_PATH = sFilePath;
                        //    var UpdateSaveFile = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdatePaperPublicationFilePath), sAcademicYear);
                        //}
                    }

                    string simgPath = Request.Form["sPhoto"];
                    if (!string.IsNullOrEmpty(Result.FILE_PATH) || !string.IsNullOrEmpty(simgPath))
                    {
                        string sFilePath = string.Empty;

                        //update file 
                        if (!string.IsNullOrEmpty(simgPath))
                        {
                            //for upload img path
                            var imgPath = simgPath.Split(',');
                            foreach (var item2 in imgPath)
                            {
                                sFilePath += DecryptString(item2) + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(Result.FILE_PATH))
                        {
                            //for add img 
                            int Count = 0;
                            var fmgPath = Result.FILE_PATH.Split(',');
                            foreach (var item2 in fmgPath)
                            {
                                string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "PaperPublication" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "PaperPublication" + "\\" + sStaffId);
                                sFilePath += sPath + ","; Count++;
                            }
                        }
                        if (!string.IsNullOrEmpty(sFilePath))
                        {
                            Result.FILE_PATH = sFilePath.TrimEnd(',');
                        }
                        var UpdateSaveFile = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdatePaperPublicationFilePath), sAcademicYear);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SavePaperPublication", ex.Message);
                    }

                    ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
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

        public JsonResult DeletePaperPublication(string sPaperPublicationId)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_PAPER_PUBLICATION();
                    ObjModel.IQAC_PAPER_PUBLICATION_ID = sPaperPublicationId;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeletePaperPublication));
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
                        ObjLog.WriteError("IQACController", "DeletePaperPublication", ex.Message);
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

        // Book Publicatoion ,..
        public ActionResult BookPublicationList()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    int nAcademicYear = Convert.ToInt32(sAcademicYear);
                    nAcademicYear--;
                    sAcademicYear = sAcademicYear + "," + nAcademicYear.ToString();

                    var ObjModel = new IQAC_BOOK_PUBLICATION();
                    ObjModel.STAFF_ID = sStaffId;
                    ObjViewModel.liBookPublication = (List<IQAC_BOOK_PUBLICATION>)CMSHandler.FetchData<IQAC_BOOK_PUBLICATION>(ObjModel,
                                    IQACSQL.GetIQACSQL(IQACEnumCommands.FetchBookPublicationList), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BookPublicationList", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Message");
            }
            return View(ObjViewModel);
        }

        public JsonResult FetchBookPublicationById(string sBookPublicationId)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_BOOK_PUBLICATION();
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.IQAC_BOOK_PUBLICATION_ID = sBookPublicationId;
                    var liModel = (List<IQAC_BOOK_PUBLICATION>)CMSHandler.FetchData<IQAC_BOOK_PUBLICATION>(ObjModel,
                                IQACSQL.GetIQACSQL(IQACEnumCommands.FetchBookPublicationById), sAcademicYear).DataSource.Data;
                    if (liModel != null && liModel.Count > 0)
                    {
                        return Json(liModel, JsonRequestBehavior.AllowGet);
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
                        ObjLog.WriteError("IQACController", "FetchBookPublicationById", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveBookPublication()
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sJsonData = Request.Form["sJsonData"];
                    var Result = JsonConvert.DeserializeObject<IQAC_BOOK_PUBLICATION>(sJsonData);
                    Result.STAFF_ID = sStaffId;
                    if (!string.IsNullOrEmpty(Result.IQAC_BOOK_PUBLICATION_ID))
                    {
                        // Update ,.....
                        var UpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateBookPublication), sAcademicYear);
                        if (UpdateQuery.Success)
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
                        // Insert ,......
                        var SaveQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveBookPublication), sAcademicYear, true);
                        if (SaveQuery.Success)
                        {
                            Result.IQAC_BOOK_PUBLICATION_ID = SaveQuery.RowUniqueId.ToString();
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }


                    string simgPath = Request.Form["sPhoto"];
                    if (!string.IsNullOrEmpty(Result.FILE_PATH) || !string.IsNullOrEmpty(simgPath))
                    {
                        string sFilePath = string.Empty;

                        //update file 
                        if (!string.IsNullOrEmpty(simgPath))
                        {
                            //for upload img path
                            var imgPath = simgPath.Split(',');
                            foreach (var item2 in imgPath)
                            {
                                sFilePath += DecryptString(item2) + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(Result.FILE_PATH))
                        {
                            //for add img 
                            int Count = 0;
                            var fmgPath = Result.FILE_PATH.Split(',');
                            foreach (var item2 in fmgPath)
                            {
                                string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "BookPublication" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "BookPublication" + "\\" + sStaffId);
                                sFilePath += sPath + ","; Count++;
                            }
                        }
                        if (!string.IsNullOrEmpty(sFilePath))
                        {
                            Result.FILE_PATH = sFilePath.TrimEnd(',');
                        }
                        var UpdateSaveFile = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateBookPublicationFilePath), sAcademicYear);
                    }
                    //if (!string.IsNullOrEmpty(Result.IQAC_BOOK_PUBLICATION_ID) && Request.Files.Count > 0)
                    //{
                    //    int nCount = 0;
                    //    string sFilePath = string.Empty;
                    //    foreach (var item2 in Request.Files)
                    //    {
                    //        string sPath = SaveFile(nCount, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "BookPublication" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "BookPublication" + "\\" + sStaffId);
                    //        sFilePath += sPath + ","; nCount++;
                    //    }
                    //    Result.FILE_PATH = sFilePath;
                    //    var UpdateSaveFile = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateBookPublicationFilePath), sAcademicYear);
                    //}
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveBookPublication", ex.Message);
                    }

                    ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
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

        public JsonResult DeleteBookPublication(string sBookPublicationId)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_BOOK_PUBLICATION();
                    ObjModel.IQAC_BOOK_PUBLICATION_ID = sBookPublicationId;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteBookPublication));
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
                        ObjLog.WriteError("IQACController", "DeleteBookPublication", ex.Message);
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

        #region Daily Activity (Department)
        public ActionResult DailyActivityList()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                   
                    var ObjModel = new DEPT_DAILY_ACTIVITY();
                    ObjModel.STAFF_ID = sStaffId;
                    ObjViewModel.liDailyActivity = (List<DEPT_DAILY_ACTIVITY>)CMSHandler.FetchData<DEPT_DAILY_ACTIVITY>(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDeptDailyActivityList), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "DailyActivityList", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchDailyActivityById(string sActivityId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new DEPT_DAILY_ACTIVITY();
                    ObjModel.ACTIVITY_ID = sActivityId;
                    var liActivity = (List<DEPT_DAILY_ACTIVITY>)CMSHandler.FetchData<DEPT_DAILY_ACTIVITY>(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDeptDailyActivityById), sAcademicYear).DataSource.Data;
                    if (liActivity != null && liActivity.Count > 0)
                    {
                        return Json(liActivity, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "FetchDailyActivityById", ex.Message);
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
        public JsonResult SaveDailyActivity()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sJsonData = Request.Form["JsonData"];
                    var Result = JsonConvert.DeserializeObject<DEPT_DAILY_ACTIVITY>(Request.Form["JsonData"]);
                    Result.ACTIVITY_DATE = CommonMethods.ConvertdatetoyearFromat(Result.ACTIVITY_DATE);
                    Result.STAFF_ID = sStaffId;
                    if (!string.IsNullOrEmpty(Result.ACTIVITY_ID))
                    {
                        var UpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateDeptDailyActivity), sAcademicYear);
                        if (UpdateQuery.Success)
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
                        var InsertQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveDeptDailyActivity), sAcademicYear, true);
                        if (InsertQuery.Success)
                        {
                            Result.ACTIVITY_ID = InsertQuery.RowUniqueId.ToString();
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }

                    string simgPath = Request.Form["sPhoto"];
                    if (!string.IsNullOrEmpty(Result.FILE_PATH) || !string.IsNullOrEmpty(simgPath))
                    {
                        string sFilePath = string.Empty;

                        //update file 
                        if (!string.IsNullOrEmpty(simgPath))
                        {
                            //for upload img path
                            var imgPath = simgPath.Split(',');
                            foreach (var item2 in imgPath)
                            {
                                sFilePath += DecryptString(item2) + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(Result.FILE_PATH))
                        {
                            //for add img 
                            int Count = 0;
                            var fmgPath = Result.FILE_PATH.Split(',');
                            foreach (var item2 in fmgPath)
                            {
                                string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "Dept_Activity" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "Dept_Activity" + "\\" + sStaffId);
                                sFilePath += sPath + ","; Count++;
                            }
                        }
                        if (!string.IsNullOrEmpty(sFilePath))
                        {
                            Result.FILE_PATH = sFilePath.TrimEnd(',');
                        }
                        var FileUpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateFileDeptDailyActivity));
                    }
                    //if (!string.IsNullOrEmpty(Result.ACTIVITY_ID))
                    //{
                    //    string sFilePath = string.Empty;
                    //    if (Request.Files.Count > 0)
                    //    {
                    //        for (int i = 0; i < Request.Files.Count; i++)
                    //        {
                    //            string sPath = SaveFile(i, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "Dept_Activity" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "Dept_Activity" + "\\" + sStaffId);
                    //            sFilePath += sPath + ",";
                    //        }
                    //        Result.FILE_PATH = sFilePath.TrimEnd(',');
                    //        var FileUpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateFileDeptDailyActivity));
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveDailyActivity", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorMessage.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteDailyActivity(string sActivityId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            var ObjModel = new DEPT_DAILY_ACTIVITY();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.ACTIVITY_ID = sActivityId;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteDeptDailyActivity));
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
                        ObjLog.WriteError("IQACController", "DeleteDailyActivity", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorMessage.NotAcceptable;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorMessage.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Seminar/Workshop Organized
        public ActionResult IQACSeminorWorkshopOrganizedList()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_SEMINOR_WORKSOP_ORGANIZED();
                    ObjModel.STAFF_ID = sStaffId;
                    ObjViewModel.liSeminorWorkshopOrganized = (List<IQAC_SEMINOR_WORKSOP_ORGANIZED>)CMSHandler.FetchData<IQAC_SEMINOR_WORKSOP_ORGANIZED>(ObjModel,
                            IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSeminorWorkshopOrganizedList), sAcademicYear).DataSource.Data;
                    var liProgrammeType = (List<SUP_PROGRAMME_TYPE>)CMSHandler.FetchData<SUP_PROGRAMME_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeType)).DataSource.Data;
                    if (liProgrammeType != null && liProgrammeType.Count > 0)
                    {
                        ObjViewModel.sProgrammeTypeList = new SelectList(liProgrammeType, Common.SUP_PROGRAMME_TYPE.PROGRAMME_TYPE_ID, Common.SUP_PROGRAMME_TYPE.PROGRAMME_TYPE);
                    }

                    var liLevel = (List<sup_Level>)CMSHandler.FetchData<sup_Level>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLevel)).DataSource.Data;
                    if (liLevel != null && liLevel.Count > 0)
                    {
                        ObjViewModel.sLevelList = new SelectList(liLevel, Common.sup_LEVEL.LEVEL_ID, Common.sup_LEVEL.LEVEL);
                    }

                    var liGuest = (List<SUP_IQAC_GUEST>)CMSHandler.FetchData<SUP_IQAC_GUEST>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupIQACGuest)).DataSource.Data;
                    if (liGuest != null && liGuest.Count > 0)
                    {
                        ObjViewModel.sGuestList = new SelectList(liGuest, Common.SUP_IQAC_GUEST.GUEST_ID, Common.SUP_IQAC_GUEST.GUEST_NAME);
                    }

                    var liOrganizer = (List<SUP_ORGANIZER>)CMSHandler.FetchData<SUP_ORGANIZER>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupOrganizer)).DataSource.Data;
                    if (liOrganizer != null && liOrganizer.Count > 0)
                    {
                        ObjViewModel.sOrganizerList = new SelectList(liOrganizer, Common.SUP_ORGANIZER.ORGANIZER_ID, Common.SUP_ORGANIZER.ORGANIZER_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "IQACSeminorWorkshopOrganizedList", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult FetchSeminorWorkshopOrganized(string IqacId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjectModel = new SUP_IQAC_RESOURCE_PERSON();
                    ObjectModel.PRIMARY_KEY_ID = IqacId;
                    ObjectModel.QUESTION_ID = "1";
                    ObjViewModel.liResourcePerson = (List<SUP_IQAC_RESOURCE_PERSON>)CMSHandler.FetchData<SUP_IQAC_RESOURCE_PERSON>(ObjectModel,
                                IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResourcePersonById), sAcademicYear).DataSource.Data;

                    return View(ObjViewModel);
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "FetchSeminorWorkshopOrganized", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;

                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;

            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FetchOrganizerById(string IqacId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjectModel = new SUP_IQAC_ORGANIZER();
                    ObjectModel.PRIMARY_ID = IqacId;
                    ObjViewModel.liOrganizer = (List<SUP_IQAC_ORGANIZER>)CMSHandler.FetchData<SUP_IQAC_ORGANIZER>(ObjectModel,
                                IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSupOrganizerListById), sAcademicYear).DataSource.Data;

                    return View(ObjViewModel);
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "FetchOrganizerById", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;

                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;

            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchSeminorWorkshopOrganizedById(string IqacId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_ASSOCIATION_ACTIVITIES();
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.IQAC_ID = IqacId;
                    ObjViewModel.liSeminorWorkshopOrganized = (List<IQAC_SEMINOR_WORKSOP_ORGANIZED>)CMSHandler.FetchData<IQAC_SEMINOR_WORKSOP_ORGANIZED>(ObjModel,
                                IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSeminorWorkshopOrganizedById), sAcademicYear).DataSource.Data;

                    var ObjectModel = new SUP_IQAC_RESOURCE_PERSON();
                    ObjectModel.PRIMARY_KEY_ID = IqacId;
                    ObjectModel.QUESTION_ID = "1";
                    ObjViewModel.liResourcePerson = (List<SUP_IQAC_RESOURCE_PERSON>)CMSHandler.FetchData<SUP_IQAC_RESOURCE_PERSON>(ObjectModel,
                                IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResourcePersonById), sAcademicYear).DataSource.Data;

                    var ObjModels = new SUP_IQAC_ORGANIZER();
                    ObjModels.PRIMARY_ID = IqacId;
                    ObjViewModel.liOrganizer = (List<SUP_IQAC_ORGANIZER>)CMSHandler.FetchData<SUP_IQAC_ORGANIZER>(ObjModels,
                        IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSupOrganizerById), sAcademicYear).DataSource.Data;

                    if (ObjViewModel.liSeminorWorkshopOrganized != null && ObjViewModel.liSeminorWorkshopOrganized.Count > 0)
                    {
                        return Json(ObjViewModel);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "FetchSeminorWorkshopOrganizedById", ex.Message);
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

        public JsonResult SaveSeminorWorkshopOrganized()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<IQAC_SEMINOR_WORKSOP_ORGANIZED>(Request.Form["JsonData"]);
                    var ResultList = JsonConvert.DeserializeObject<Json_Save>(Request.Form["JsonList"]);
                    var ResultOrganizerList = JsonConvert.DeserializeObject<Json_Save>(Request.Form["JsonOrganizer"]);
                    Result.STAFF_ID = sStaffId;
                    Result.ORGANIZED_DATE = CommonMethods.ConvertdatetoyearFromat(Result.ORGANIZED_DATE);
                    Result.TO_DATE = CommonMethods.ConvertdatetoyearFromat(Result.TO_DATE);
                    if (!string.IsNullOrEmpty(Result.IQAC_ID))
                    {
                        // Updtae ,...
                        var UpdateSeminorWorkshopOrganized = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateSeminorWorkshopOrganized), sAcademicYear);
                        if (UpdateSeminorWorkshopOrganized.Success)
                        {
                            ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            if (UpdateSeminorWorkshopOrganized.RowsAffected == 0)
                            {
                                ObjJsonData.Message = "No Changes Made !";
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            }
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                    else
                    {
                        // Insert ,...
                        var SaveSeminorWorkshopOrganized = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveSeminorWorkshopOrganized), sAcademicYear, true);
                        if (SaveSeminorWorkshopOrganized.Success)
                        {
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            Result.IQAC_ID = SaveSeminorWorkshopOrganized.RowUniqueId.ToString();
                            if (SaveSeminorWorkshopOrganized.RowsAffected == 0)
                            {
                                ObjJsonData.Message = "No Changes Made !";
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            }
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }

                    if (!string.IsNullOrEmpty(Result.IQAC_ID) && ResultList.liResourcePerson != null && ResultList.liResourcePerson.Count > 0)
                    {
                        foreach (var item in ResultList.liResourcePerson)
                        {
                            item.PRIMARY_KEY_ID = Result.IQAC_ID;
                            if (!string.IsNullOrEmpty(item.RESOURCE_PERSON_ID))
                            {
                                // Update ,..
                                var SaveResourcePerson = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateResourcePerson));
                            }
                            else
                            {
                                // Insert ,...
                                var SaveResourcePerson = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveResourcePerson));
                            }
                        }
                    }

                    if (ResultOrganizerList.liOrganizer != null && ResultOrganizerList.liOrganizer.Count > 0)
                    {
                        foreach (var item in ResultOrganizerList.liOrganizer)
                        {
                            item.PRIMARY_ID = Result.IQAC_ID;
                            var sSaveOrganizer = CMSHandler.SaveOrUpdate(item, !string.IsNullOrEmpty(item.SUP_ORGANIZER_ID) ? IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateOrganizer) : IQACSQL.GetIQACSQL(IQACEnumCommands.SaveOrganizer));
                        }
                    }


                    string simgPath = Request.Form["sPhoto"];
                    if (!string.IsNullOrEmpty(Result.FILE_PATH) || !string.IsNullOrEmpty(simgPath))
                    {
                        string sFilePath = string.Empty;

                        //update file 
                        if (!string.IsNullOrEmpty(simgPath))
                        {
                            //for upload img path
                            var imgPath = simgPath.Split(',');
                            foreach (var item2 in imgPath)
                            {
                                sFilePath += DecryptString(item2) + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(Result.FILE_PATH))
                        {
                            //for add img 
                            int Count = 0;
                            var fmgPath = Result.FILE_PATH.Split(',');
                            foreach (var item2 in fmgPath)
                            {
                                string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "Seminor_Workshop_organized" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "Seminor_Workshop_organized" + "\\" + sStaffId);
                                sFilePath += sPath + ","; Count++;
                            }
                        }
                        if (!string.IsNullOrEmpty(sFilePath))
                        {
                            Result.FILE_PATH = sFilePath.TrimEnd(',');
                        }
                        var FileUpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateFileSeminorWorkshopOrganized), sAcademicYear);
                    }
                    //string sFilePath = string.Empty;
                    //if (Request.Files.Count > 0)
                    //{
                    //    for (int i = 0; i < Request.Files.Count; i++)
                    //    {
                    //        string sPath = SaveFile(i, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "Seminor_Workshop_organized" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "Seminor_Workshop_organized" + "\\" + sStaffId);
                    //        sFilePath += sPath + ",";
                    //    }
                    //    Result.FILE_PATH = sFilePath.TrimEnd(',');
                    //    var FileUpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateFileSeminorWorkshopOrganized), sAcademicYear);
                    //}
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveSeminorWorkshopOrganized", ex.Message);
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

        public JsonResult DeleteSeminorWorkshopOrganized(string IqacId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_SEMINOR_WORKSOP_ORGANIZED();
                    ObjModel.IQAC_ID = IqacId;
                    var DeleteAssociation = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteSeminorWorkshopOrganized), sAcademicYear);
                    if (DeleteAssociation.Success)
                    {
                        ObjJsonData.Message = Common.Messages.RecordDeletedSuccessfully;
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        if (DeleteAssociation.RowsAffected == 0)
                        {
                            ObjJsonData.Message = "No Changes Made !";
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
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
                        ObjLog.WriteError("IQACController", "DeleteSeminorWorkshopOrganized", ex.Message);
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

        #region Association Activities
        public ActionResult IQACAssociationActivitiesList()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    int nAcademicYear = Convert.ToInt32(sAcademicYear);
                    nAcademicYear--;
                    sAcademicYear = sAcademicYear + "," + nAcademicYear.ToString();

                    var ObjModel = new IQAC_ASSOCIATION_ACTIVITIES();
                    ObjModel.STAFF_ID = sStaffId;
                    ObjViewModel.liAssociationActivity = (List<IQAC_ASSOCIATION_ACTIVITIES>)CMSHandler.FetchData<IQAC_ASSOCIATION_ACTIVITIES>(ObjModel,
                                 IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAssociationActivitiesList), sAcademicYear).DataSource.Data;

                    var liActivityType = (List<SUP_IQAC_ACTIVITY>)CMSHandler.FetchData<SUP_IQAC_ACTIVITY>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupIQACActivity)).DataSource.Data;
                    if (liActivityType != null && liActivityType.Count > 0)
                    {
                        ObjViewModel.sActivityType = new SelectList(liActivityType, Common.SUP_ACTIVITY.ACTIVITY_ID, Common.SUP_ACTIVITY.ACTIVITY_NAME);
                    }

                    var liShift = (List<SUP_STAFF_ASSOCIATION>)CMSHandler.FetchData<SUP_STAFF_ASSOCIATION>(ObjModel, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupStaffAssociation), sAcademicYear).DataSource.Data;
                    if (liShift != null && liShift.Count > 0)
                    {
                        ObjViewModel.sFlag = true;
                        ObjViewModel.sShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "IQACAssociationActivitiesList", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult FetchResourcePersonByAssociationId(string IqacId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new SUP_IQAC_RESOURCE_PERSON();
                    ObjModel.PRIMARY_KEY_ID = IqacId;
                    ObjModel.QUESTION_ID = "2";
                    ObjViewModel.liResourcePerson = (List<SUP_IQAC_RESOURCE_PERSON>)CMSHandler.FetchData<SUP_IQAC_RESOURCE_PERSON>(ObjModel,
                                IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResourcePersonById), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "FetchResourcePersonByAssociationId", ex.Message);
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
            return View(ObjViewModel);
        }

        public JsonResult FetchAssociationActivities(string IqacId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_ASSOCIATION_ACTIVITIES();
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.IQAC_ID = IqacId;
                    ObjViewModel.liAssociationActivity = (List<IQAC_ASSOCIATION_ACTIVITIES>)CMSHandler.FetchData<IQAC_ASSOCIATION_ACTIVITIES>(ObjModel,
                                IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAssociationActivitiesById), sAcademicYear).DataSource.Data;

                    var ObjectModel = new SUP_IQAC_RESOURCE_PERSON();
                    ObjectModel.PRIMARY_KEY_ID = IqacId;
                    ObjectModel.QUESTION_ID = "2";
                    ObjViewModel.liResourcePerson = (List<SUP_IQAC_RESOURCE_PERSON>)CMSHandler.FetchData<SUP_IQAC_RESOURCE_PERSON>(ObjectModel,
                                IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResourcePersonById), sAcademicYear).DataSource.Data;

                    if (ObjViewModel.liAssociationActivity != null && ObjViewModel.liAssociationActivity.Count > 0)
                    {
                        return Json(ObjViewModel);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "FetchAssociationActivities", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveAssociationActivities(string sJsonData, string sJsonList)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<IQAC_ASSOCIATION_ACTIVITIES>(Request.Form["sJsonData"]);
                    var ResultList = JsonConvert.DeserializeObject<Json_Save>(Request.Form["sJsonList"]);
                    Result.STAFF_ID = sStaffId;
                    Result.ACTIVITY_DATE = CommonMethods.ConvertdatetoyearFromat(Result.ACTIVITY_DATE);
                    Result.TO_DATE = CommonMethods.ConvertdatetoyearFromat(Result.TO_DATE);
                    string s = Request.Form["sJsonList"];
                    if (!string.IsNullOrEmpty(Result.IQAC_ID))
                    {
                        // Updtae ,...
                        var SaveAssociation = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateAssociationActivities), sAcademicYear);
                        if (SaveAssociation.Success)
                        {
                            ObjJsonData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            if (SaveAssociation.RowsAffected == 0)
                            {
                                ObjJsonData.Message = "No Changes Made !";
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            }
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToUpdatedRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                    else
                    {
                        // Insert ,...
                        var SaveAssociation = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveAssociationActivities), sAcademicYear, true);
                        if (SaveAssociation.Success)
                        {
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            Result.IQAC_ID = SaveAssociation.RowUniqueId.ToString();
                            if (SaveAssociation.RowsAffected == 0)
                            {
                                ObjJsonData.Message = "No Changes Made !";
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            }
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }

                    if (!string.IsNullOrEmpty(Result.IQAC_ID) && ResultList.liResourcePerson != null && ResultList.liResourcePerson.Count > 0)
                    {
                        foreach (var item in ResultList.liResourcePerson)
                        {
                            item.PRIMARY_KEY_ID = Result.IQAC_ID;
                            item.GUEST_ID = "0";
                            if (!string.IsNullOrEmpty(item.RESOURCE_PERSON_ID))
                            {
                                // Update ,..
                                var SaveResourcePerson = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateResourcePerson));
                            }
                            else
                            {
                                // Insert ,...
                                var SaveResourcePerson = CMSHandler.SaveOrUpdate(item, IQACSQL.GetIQACSQL(IQACEnumCommands.SaveResourcePerson));
                            }
                        }
                    }

                    string simgPath = Request.Form["sPhoto"];
                    if (!string.IsNullOrEmpty(Result.FILE_PATH) || !string.IsNullOrEmpty(simgPath))
                    {
                        string sFilePath = string.Empty;

                        //update file 
                        if (!string.IsNullOrEmpty(simgPath))
                        {
                            //for upload img path
                            var imgPath = simgPath.Split(',');
                            foreach (var item2 in imgPath)
                            {
                                sFilePath += DecryptString(item2) + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(Result.FILE_PATH))
                        {
                            //for add img 
                            int Count = 0;
                            var fmgPath = Result.FILE_PATH.Split(',');
                            foreach (var item2 in fmgPath)
                            {
                                string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "AssociationActivities" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "AssociationActivities" + "\\" + sStaffId);
                                sFilePath += sPath + ","; Count++;
                            }
                        }
                        if (!string.IsNullOrEmpty(sFilePath))
                        {
                            Result.FILE_PATH = sFilePath.TrimEnd(',');
                        }
                        var FileUpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateFileAssociationActivities), sAcademicYear);
                    }
                    //if (Request.Files.Count > 0 && !string.IsNullOrEmpty(Result.IQAC_ID))
                    //{
                    //    string sFilePath = string.Empty; int i = 0;
                    //    foreach (var item in Request.Files)
                    //    {
                    //        sFilePath += SaveFile(i, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "AssociationActivities" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "AssociationActivities" + "\\" + sStaffId);
                    //        i++;
                    //    }
                    //    Result.FILE_PATH = sFilePath;
                    //    var FileUpdateQuery = CMSHandler.SaveOrUpdate(Result, IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateFileAssociationActivities), sAcademicYear);
                    //}
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveAssociationActivities", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAssociationActivities(string IqacId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_ASSOCIATION_ACTIVITIES();
                    ObjModel.IQAC_ID = IqacId;
                    var DeleteAssociation = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeleteAssociationActivities), sAcademicYear);
                    if (DeleteAssociation.Success)
                    {
                        ObjJsonData.Message = Common.Messages.RecordDeletedSuccessfully;
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        if (DeleteAssociation.RowsAffected == 0)
                        {
                            ObjJsonData.Message = "No Changes Made !";
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
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
                        ObjLog.WriteError("IQACController", "DeleteAssociationActivities", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion


        // Reports ......
        #region IQAC Report (Month Wise)
        public ActionResult IQACOverAllReport()
        {
            var ObjViewModel = new IQACViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Academic Year ,...
                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.sAcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    // Department List
                    var Lidepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    if (Lidepartment != null && Lidepartment.Count > 0)
                    {
                        ObjViewModel.sDepartmentList = new SelectList(Lidepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    }
                    // Month List ,..
                    ObjViewModel.liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumberForReport), sAcademicYear).DataSource.Data;


                    // IQAC Questions
                    var liQuestionsList = (List<IQAC_QUESTION>)CMSHandler.FetchData<IQAC_QUESTION>(null, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchIQACQuestion), sAcademicYear).DataSource.Data;
                    if (liQuestionsList != null && liQuestionsList.Count > 0)
                    {
                        ObjViewModel.sIQACQuestionsList = new SelectList(liQuestionsList, "IQAC_QUESTION_ID", "IQAC_QUESTIONS");
                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "IQACOverAllReport", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindIQACOverAllReport(string sAcademicYearList, string sMonthList, string sIQACQuestionsList, string sDepartment, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    switch (sIQACQuestionsList)
                    {
                        case Common.IQAC_QUSTIONS.TEACHING_SUBJECTS:
                            {
                                return RedirectToAction("BindIQACTeachingSubjectsWithWorkload", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.TEACHING_LEARNING:
                            {
                                return RedirectToAction("BindIQACPreparationandUpdationOfNotes", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.CO_CURRICULAR_ASPECTS:
                            {
                                return RedirectToAction("BindIQACCoCurricularActivities", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });

                            }
                        case Common.IQAC_QUSTIONS.EXTENSION_WORKS:
                            {
                                return RedirectToAction("BindIQACExtensionWorks", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.CURRICULAR_ASPECTS:
                            {
                                return RedirectToAction("BindIQACPersonalAcademicActivity", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.AWARDS:
                            {
                                return RedirectToAction("BindIQACAwards", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.SUBJECT_ASSOCIATION:
                            {
                                return RedirectToAction("BindIQACSubjectAssociation", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.HONOURS:
                            {
                                return RedirectToAction("BindIQACHonor", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.MEMBERSHIP:
                            {
                                return RedirectToAction("BindIQACMemberShip", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.PROJECT:
                            {
                                return RedirectToAction("BindIQACProject", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.RESEARCH_GUIDENCE:
                            {
                                return RedirectToAction("BindIQACResearchGuidence", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.PHD_GUIDENCE:
                            {
                                return RedirectToAction("BindIQACPHDGuidence", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.PUBLICATION_AND_PRASENTATION:
                            {
                                return RedirectToAction("BindIQACPublicationAndPrasentation", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.PAPER_PUBLICATION:
                            {
                                return RedirectToAction("BindIQACPaperPublication", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.BOOK_PUBLICATION:
                            {
                                return RedirectToAction("BindIQACBookPublication", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.DEPARTMENT_DAILY_ACTIVITY:
                            {
                                return RedirectToAction("BindIQACDeptDailyActivity", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.ASSOCIATION_ACTIVITIES:
                            {
                                return RedirectToAction("BindIQACAssociationActivity", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                        case Common.IQAC_QUSTIONS.SEMINAR_WORKSHOP_ORGANIZED:
                            {
                                return RedirectToAction("BindIQACSeminarOrWorkshopOrganized", new { sAcademicYearList = sAcademicYearList, sMonthList = sMonthList, sDepartment = sDepartment, sStaff = sStaff });
                            }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACOverAllReport", ex.Message);
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

        public ActionResult BindIQACTeachingSubjectsWithWorkload(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchTeachingsubjectsworkloadForReportByAcademicYearandMonth) + " AND IPN.STAFF_ID IN (" + sStaff + ") AND IPN.MONTH_ID IN (" + sMonthList + ") GROUP BY IPN.WORKLOAD_ID ORDER BY STF.DEPARTMENT,STF.STAFF_ID; ";

                    //fetch teaching and subject workload
                    var liTeaching = (List<IQAC_TEACHING_SUBJECTS_WORKLOAD>)CMSHandler.FetchData<IQAC_TEACHING_SUBJECTS_WORKLOAD>(new IQAC_TEACHING_SUBJECTS_WORKLOAD()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYear).DataSource.Data;

                    if (liTeaching != null && liTeaching.Count > 0)
                    {

                        ObjViewModel.liTeachingSubjectWorkload = liTeaching;
                    }
                    else
                    {
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACTeachingSubjectsWithWorkload", ex.Message);
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

        public ActionResult BindIQACPreparationandUpdationOfNotes(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPreparationandUpdationNotesForReportByAcademicYearandMonth) + " AND IPN.STAFF_ID IN (" + sStaff + ") AND IPN.MONTH_ID IN (" + sMonthList + ") GROUP BY IPN.IQAC_ID   ORDER BY STF.DEPARTMENT,STF.STAFF_ID;";
                    //fetch preparation and updatoin notes
                    var liReport = (List<IQAC_PREPARATION_AND_UPDATION_OF_NOTES>)CMSHandler.FetchData<IQAC_PREPARATION_AND_UPDATION_OF_NOTES>(new IQAC_PREPARATION_AND_UPDATION_OF_NOTES()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYear).DataSource.Data;



                    if (liReport != null && liReport.Count > 0)
                    {

                        ObjViewModel.liPreparation = liReport;
                    }
                    else
                    {
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACPreparationandUpdationOfNotes", ex.Message);
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

        public ActionResult BindIQACCoCurricularActivities(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchCoCurrricularAspectsForReportByAcademicYearandMonth) + " AND IPN.STAFF_ID IN (" + sStaff + ") AND IPN.MONTH_ID IN (" + sMonthList + ") ORDER BY STF.DEPARTMENT,STF.STAFF_ID; ";
                    //fetch co curricular aspects
                    var liReport = (List<IQAC_CO_CURRICULAR_ASPECTS>)CMSHandler.FetchData<IQAC_CO_CURRICULAR_ASPECTS>(new IQAC_CO_CURRICULAR_ASPECTS()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYear).DataSource.Data;


                    if (liReport != null && liReport.Count > 0)
                    {

                        ObjViewModel.liCurricularAspects = liReport;
                    }
                    else
                    {
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACCoCurricularActivities", ex.Message);
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

        public ActionResult BindIQACExtensionWorks(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchExtensionWorksForReportByAcademicYearandMonth) + " AND IPN.STAFF_ID IN (" + sStaff + ") AND IPN.MONTH_ID IN (" + sMonthList + ") ORDER BY STF.DEPARTMENT,STF.STAFF_ID;";
                    //fetch extension works
                    var liReport = (List<IQAC_EXTENSION_WORKS>)CMSHandler.FetchData<IQAC_EXTENSION_WORKS>(new IQAC_EXTENSION_WORKS()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYear).DataSource.Data;


                    if (liReport != null && liReport.Count > 0)
                    {

                        ObjViewModel.liExtensionWorks = liReport;
                    }
                    else
                    {
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACExtensionWorks", ex.Message);
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

        public ActionResult BindIQACAwards(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string cur_month = string.Empty;
                    cur_month = (sMonthList == "1") ? "6" : (sMonthList == "2") ? "7" : (sMonthList == "3") ? "8" : (sMonthList == "4") ? "9" : (sMonthList == "5") ? "10" : (sMonthList == "6") ? "11" : (sMonthList == "7") ? "12" : (sMonthList == "8") ? "1" : (sMonthList == "9") ? "2" : (sMonthList == "10") ? "3" : (sMonthList == "11") ? "4" : (sMonthList == "12") ? "5" : "";
                    string sSQL = string.Empty;
                    if (sMonthList == "14")
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAwardsForReportByAcademicYearandMonth) + " AND IA.STAFF_ID IN (" + sStaff + ")  ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    else
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAwardsForReportByAcademicYearandMonth) + " AND IA.STAFF_ID IN (" + sStaff + ") AND MONTH(IA.ENTRY_DATE)=" + cur_month + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    var liReport = (List<IQAC_AWARDS>)CMSHandler.FetchData<IQAC_AWARDS>(new IQAC_EXTENSION_WORKS()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYear).DataSource.Data;


                    if (liReport != null && liReport.Count > 0)
                        ObjViewModel.liAwards = liReport;
                    else
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACAwards", ex.Message);
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

        public ActionResult BindIQACSubjectAssociation(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string cur_month = string.Empty;
                    cur_month = (sMonthList == "1") ? "6" : (sMonthList == "2") ? "7" : (sMonthList == "3") ? "8" : (sMonthList == "4") ? "9" : (sMonthList == "5") ? "10" : (sMonthList == "6") ? "11" : (sMonthList == "7") ? "12" : (sMonthList == "8") ? "1" : (sMonthList == "9") ? "2" : (sMonthList == "10") ? "3" : (sMonthList == "11") ? "4" : (sMonthList == "12") ? "5" : "";
                    string sSQL = string.Empty;
                    if (sMonthList == "14")
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSubjectAssociationForReportByAcademicYearandMonth) + " AND IA.STAFF_ID IN (" + sStaff + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    else
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSubjectAssociationForReportByAcademicYearandMonth) + " AND IA.STAFF_ID IN (" + sStaff + ") AND MONTH(IA.ENTRY_DATE)=" + cur_month + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    var liReport = (List<IQAC_SUBJECT_ASSOCIATION>)CMSHandler.FetchData<IQAC_SUBJECT_ASSOCIATION>(new IQAC_EXTENSION_WORKS()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYear).DataSource.Data;
                    if (liReport != null && liReport.Count > 0)
                        ObjViewModel.liSubjectAssociation = liReport;
                    else
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACSubjectAssociation", ex.Message);
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

        public ActionResult BindIQACHonor(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string cur_month = string.Empty;
                    cur_month = (sMonthList == "1") ? "6" : (sMonthList == "2") ? "7" : (sMonthList == "3") ? "8" : (sMonthList == "4") ? "9" : (sMonthList == "5") ? "10" : (sMonthList == "6") ? "11" : (sMonthList == "7") ? "12" : (sMonthList == "8") ? "1" : (sMonthList == "9") ? "2" : (sMonthList == "10") ? "3" : (sMonthList == "11") ? "4" : (sMonthList == "12") ? "5" : "";
                    string sSQL = string.Empty;
                    if (sMonthList == "14")
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchHonorForReportByAcademicYearandMonth) + " AND IA.STAFF_ID IN (" + sStaff + ")  ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    else
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchHonorForReportByAcademicYearandMonth) + " AND IA.STAFF_ID IN (" + sStaff + ") AND MONTH(IA.FROM_DATE)=" + cur_month + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    var liReport = (List<IQAC_HONOURS>)CMSHandler.FetchData<IQAC_HONOURS>(new IQAC_EXTENSION_WORKS()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYear).DataSource.Data;


                    if (liReport != null && liReport.Count > 0)
                        ObjViewModel.liHonor = liReport;
                    else
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACHonor", ex.Message);
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

        public ActionResult BindIQACMemberShip(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string cur_month = string.Empty;
                    cur_month = (sMonthList == "1") ? "6" : (sMonthList == "2") ? "7" : (sMonthList == "3") ? "8" : (sMonthList == "4") ? "9" : (sMonthList == "5") ? "10" : (sMonthList == "6") ? "11" : (sMonthList == "7") ? "12" : (sMonthList == "8") ? "1" : (sMonthList == "9") ? "2" : (sMonthList == "10") ? "3" : (sMonthList == "11") ? "4" : (sMonthList == "12") ? "5" : "";
                    string sSQL = string.Empty;
                    if (sMonthList == "14")
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchMemberShipForReportByAcademicYearandMonth) + "AND IA.STAFF_ID IN (" + sStaff + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    else
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchMemberShipForReportByAcademicYearandMonth) + "AND IA.STAFF_ID IN (" + sStaff + ") AND MONTH(IA.ENTRY_DATE)=" + cur_month + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    var liReport = (List<IQAC_MEMBERSHIP>)CMSHandler.FetchData<IQAC_MEMBERSHIP>(new IQAC_EXTENSION_WORKS()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYearList).DataSource.Data;


                    if (liReport != null && liReport.Count > 0)
                        ObjViewModel.liMembership = liReport;
                    else
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACMemberShip", ex.Message);
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

        public ActionResult BindIQACProject(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string cur_month = string.Empty;
                    cur_month = (sMonthList == "1") ? "6" : (sMonthList == "2") ? "7" : (sMonthList == "3") ? "8" : (sMonthList == "4") ? "9" : (sMonthList == "5") ? "10" : (sMonthList == "6") ? "11" : (sMonthList == "7") ? "12" : (sMonthList == "8") ? "1" : (sMonthList == "9") ? "2" : (sMonthList == "10") ? "3" : (sMonthList == "11") ? "4" : (sMonthList == "12") ? "5" : "";
                    string sSQL = string.Empty;
                    if (sMonthList == "14")
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchProjectForReportByAcademicYearandMonth) + " AND IP.STAFF_ID IN (" + sStaff + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    else
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchProjectForReportByAcademicYearandMonth) + " AND IP.STAFF_ID IN (" + sStaff + ") AND MONTH(I.ENTRY_DATE)=" + cur_month + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    var liReport = (List<IQAC_PROJECT>)CMSHandler.FetchData<IQAC_PROJECT>(new IQAC_EXTENSION_WORKS()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYearList).DataSource.Data;


                    if (liReport != null && liReport.Count > 0)
                        ObjViewModel.liIQACProject = liReport;
                    else
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACProject", ex.Message);
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

        public ActionResult BindIQACResearchGuidence(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string cur_month = string.Empty;
                    cur_month = (sMonthList == "1") ? "6" : (sMonthList == "2") ? "7" : (sMonthList == "3") ? "8" : (sMonthList == "4") ? "9" : (sMonthList == "5") ? "10" : (sMonthList == "6") ? "11" : (sMonthList == "7") ? "12" : (sMonthList == "8") ? "1" : (sMonthList == "9") ? "2" : (sMonthList == "10") ? "3" : (sMonthList == "11") ? "4" : (sMonthList == "12") ? "5" : "";
                    string sSQL = string.Empty;
                    if (sMonthList == "14")
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResearchGuidenceForReportByAcademicYearandMonth) + " AND I.STAFF_ID IN (" + sStaff + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    else
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResearchGuidenceForReportByAcademicYearandMonth) + " AND I.STAFF_ID IN (" + sStaff + ") AND MONTH(I.ENTRY_DATE)=" + cur_month + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    var liReport = (List<IQAC_RESEARCH_GUIDENCE>)CMSHandler.FetchData<IQAC_RESEARCH_GUIDENCE>(new IQAC_EXTENSION_WORKS()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYearList).DataSource.Data;

                    if (liReport != null && liReport.Count > 0)
                        ObjViewModel.liIQACResearchGuidence = liReport;
                    else
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACResearchGuidence", ex.Message);
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

        public ActionResult BindIQACPHDGuidence(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string cur_month = string.Empty;
                    cur_month = (sMonthList == "1") ? "6" : (sMonthList == "2") ? "7" : (sMonthList == "3") ? "8" : (sMonthList == "4") ? "9" : (sMonthList == "5") ? "10" : (sMonthList == "6") ? "11" : (sMonthList == "7") ? "12" : (sMonthList == "8") ? "1" : (sMonthList == "9") ? "2" : (sMonthList == "10") ? "3" : (sMonthList == "11") ? "4" : (sMonthList == "12") ? "5" : "";
                    string sSQL = string.Empty;
                    if (sMonthList == "14")
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPHDGuidenceForReportByAcademicYearandMonth) + " AND I.STAFF_ID IN (" + sStaff + ")  ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    else
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPHDGuidenceForReportByAcademicYearandMonth) + " AND I.STAFF_ID IN (" + sStaff + ") AND MONTH(I.GUDIENCE_DATE)=" + cur_month + "  ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    var liReport = (List<IQAC_PHD_GUIDENCE>)CMSHandler.FetchData<IQAC_PHD_GUIDENCE>(new IQAC_EXTENSION_WORKS()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYearList).DataSource.Data;


                    if (liReport != null && liReport.Count > 0)
                        ObjViewModel.liIQACPhdGuidence = liReport;
                    else
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACPHDGuidence", ex.Message);
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

        public ActionResult BindIQACPublicationAndPrasentation(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string cur_month = string.Empty;
                    cur_month = (sMonthList == "1") ? "6" : (sMonthList == "2") ? "7" : (sMonthList == "3") ? "8" : (sMonthList == "4") ? "9" : (sMonthList == "5") ? "10" : (sMonthList == "6") ? "11" : (sMonthList == "7") ? "12" : (sMonthList == "8") ? "1" : (sMonthList == "9") ? "2" : (sMonthList == "10") ? "3" : (sMonthList == "11") ? "4" : (sMonthList == "12") ? "5" : "";
                    string sSQL = string.Empty;
                    if (sMonthList == "14")
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPublicationAndPrasentationForReportByAcademicYearandMonth) + " AND I.STAFF_ID IN (" + sStaff + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    else
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPublicationAndPrasentationForReportByAcademicYearandMonth) + " AND I.STAFF_ID IN (" + sStaff + ") AND MONTH(I.ACTIVITY_DATE)=" + cur_month + "  ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    var liReport = (List<IQAC_STAFF_ACTIVITY>)CMSHandler.FetchData<IQAC_STAFF_ACTIVITY>(new IQAC_EXTENSION_WORKS()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYearList).DataSource.Data;


                    if (liReport != null && liReport.Count > 0)
                        ObjViewModel.liStaffActivity = liReport;
                    else
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACPublicationAndPrasentation", ex.Message);
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

        public ActionResult BindIQACPaperPublication(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string cur_month = string.Empty;
                    cur_month = (sMonthList == "1") ? "6" : (sMonthList == "2") ? "7" : (sMonthList == "3") ? "8" : (sMonthList == "4") ? "9" : (sMonthList == "5") ? "10" : (sMonthList == "6") ? "11" : (sMonthList == "7") ? "12" : (sMonthList == "8") ? "1" : (sMonthList == "9") ? "2" : (sMonthList == "10") ? "3" : (sMonthList == "11") ? "4" : (sMonthList == "12") ? "5" : "";
                    string sSQL = string.Empty;
                    if (sMonthList == "14")
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPaperPublicationForReportByAcademicYearandMonth) + " AND I.STAFF_ID IN (" + sStaff + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    else
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPaperPublicationForReportByAcademicYearandMonth) + " AND I.STAFF_ID IN (" + sStaff + ") AND MONTH(I.ENTRY_DATE)=" + cur_month + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    var liReport = (List<IQAC_PAPER_PUBLICATION>)CMSHandler.FetchData<IQAC_PAPER_PUBLICATION>(new IQAC_EXTENSION_WORKS()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYearList).DataSource.Data;


                    if (liReport != null && liReport.Count > 0)
                        ObjViewModel.liPaperPublication = liReport;
                    else
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACPaperPublication", ex.Message);
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

        public ActionResult BindIQACBookPublication(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string cur_month = string.Empty;
                    cur_month = (sMonthList == "1") ? "6" : (sMonthList == "2") ? "7" : (sMonthList == "3") ? "8" : (sMonthList == "4") ? "9" : (sMonthList == "5") ? "10" : (sMonthList == "6") ? "11" : (sMonthList == "7") ? "12" : (sMonthList == "8") ? "1" : (sMonthList == "9") ? "2" : (sMonthList == "10") ? "3" : (sMonthList == "11") ? "4" : (sMonthList == "12") ? "5" : "";
                    string sSQL = string.Empty;
                    if (sMonthList == "14")
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchBookPublicationForReportByAcademicYearandMonth) + " AND I.STAFF_ID IN (" + sStaff + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    else
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchBookPublicationForReportByAcademicYearandMonth) + " AND I.STAFF_ID IN (" + sStaff + ") AND MONTH(I.ENTRY_DATE)=" + cur_month + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    var liReport = (List<IQAC_BOOK_PUBLICATION>)CMSHandler.FetchData<IQAC_BOOK_PUBLICATION>(new IQAC_EXTENSION_WORKS()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYearList).DataSource.Data;


                    if (liReport != null && liReport.Count > 0)
                        ObjViewModel.liBookPublication = liReport;
                    else
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACBookPublication", ex.Message);
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


        public ActionResult BindIQACPersonalAcademicActivity(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string SQL = string.Empty;
                    SQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPersonalAcademicActivityForReportByAcademicYearandMonth) + " AND IPN.STAFF_ID IN (" + sStaff + ") AND IPN.MONTH_ID IN (" + sMonthList + ")";
                    //fetch personal academic activity
                    var liReport = (List<IQAC_PERSONAL_ACADEMIC_ACTIVITY>)CMSHandler.FetchData<IQAC_PERSONAL_ACADEMIC_ACTIVITY>(new IQAC_PERSONAL_ACADEMIC_ACTIVITY()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, SQL, sAcademicYear).DataSource.Data;



                    if (liReport != null && liReport.Count > 0)
                    {

                        ObjViewModel.liPersonalAcademicActivity = liReport;
                    }
                    else
                    {
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                    }



                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACPersonalAcademicActivity", ex.Message);
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
        // Not In Use
        public ActionResult BindIQACWorkDoneData(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //fetch personal academic activity
                    var liReport = (List<IQAC_WORK_DONE_DATA>)CMSHandler.FetchData<IQAC_WORK_DONE_DATA>(new IQAC_WORK_DONE_DATA()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.FetchWorkDoneDataActivityForReportByAcademicYearandMonth), sAcademicYear).DataSource.Data;



                    if (liReport != null && liReport.Count > 0)
                    {

                        ObjViewModel.liWorkDoneData = liReport;
                    }
                    else
                    {
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                    }




                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACWorkDoneData", ex.Message);
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
        // Not In Use
        public ActionResult BindIQacExaminationDutiesInternal(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    //fetch personal academic activity
                    var liReport = (List<IQAC_EXAMINATION_DUTIES_INTERNAL>)CMSHandler.FetchData<IQAC_EXAMINATION_DUTIES_INTERNAL>(new IQAC_EXAMINATION_DUTIES_INTERNAL()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.FetchExamainationDutiesInternalForReportByAcademicYearandMonth), sAcademicYear).DataSource.Data;



                    if (liReport != null && liReport.Count > 0)
                    {

                        ObjViewModel.liExaminationDutiesInternal = liReport;
                    }
                    else
                    {
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQacExaminationDutiesInternal", ex.Message);
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

        public ActionResult BindIQACDeptDailyActivity(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string cur_month = string.Empty;
                    cur_month = (sMonthList == "1") ? "6" : (sMonthList == "2") ? "7" : (sMonthList == "3") ? "8" : (sMonthList == "4") ? "9" : (sMonthList == "5") ? "10" : (sMonthList == "6") ? "11" : (sMonthList == "7") ? "12" : (sMonthList == "8") ? "1" : (sMonthList == "9") ? "2" : (sMonthList == "10") ? "3" : (sMonthList == "11") ? "4" : (sMonthList == "12") ? "5" : "";
                    string sSQL = string.Empty;
                    if (sMonthList == "14")
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDepartmentDailyActivityForReportByAcademicYearandMonth) + " AND I.STAFF_ID IN (" + sStaff + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    else
                    {
                        sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDepartmentDailyActivityForReportByAcademicYearandMonth) + " AND I.STAFF_ID IN (" + sStaff + ") AND MONTH(I.ACTIVITY_DATE)=" + cur_month + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    }
                    var liReport = (List<DEPT_DAILY_ACTIVITY>)CMSHandler.FetchData<DEPT_DAILY_ACTIVITY>(new IQAC_EXTENSION_WORKS()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYear).DataSource.Data;


                    if (liReport != null && liReport.Count > 0)
                        ObjViewModel.liDailyActivity = liReport;
                    else
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACDeptDailyActivity", ex.Message);
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

        public ActionResult BindIQACAssociationActivity(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string cur_month = string.Empty;
                    cur_month = (sMonthList == "1") ? "6" : (sMonthList == "2") ? "7" : (sMonthList == "3") ? "8" : (sMonthList == "4") ? "9" : (sMonthList == "5") ? "10" : (sMonthList == "6") ? "11" : (sMonthList == "7") ? "12" : (sMonthList == "8") ? "1" : (sMonthList == "9") ? "2" : (sMonthList == "10") ? "3" : (sMonthList == "11") ? "4" : (sMonthList == "12") ? "5" : "";
                    string sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAssociationActivityForReportByAcademicYearandMonth) + "ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    var liReport = (List<IQAC_ASSOCIATION_ACTIVITIES>)CMSHandler.FetchData<IQAC_ASSOCIATION_ACTIVITIES>(new IQAC_EXTENSION_WORKS()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYear).DataSource.Data;


                    if (liReport != null && liReport.Count > 0)
                        ObjViewModel.liAssociationActivity = liReport;
                    else
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACAssociationActivity", ex.Message);
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

        public ActionResult BindIQACSeminarOrWorkshopOrganized(string sAcademicYearList, string sMonthList, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string cur_month = string.Empty;
                    cur_month = (sMonthList == "1") ? "6" : (sMonthList == "2") ? "7" : (sMonthList == "3") ? "8" : (sMonthList == "4") ? "9" : (sMonthList == "5") ? "10" : (sMonthList == "6") ? "11" : (sMonthList == "7") ? "12" : (sMonthList == "8") ? "1" : (sMonthList == "9") ? "2" : (sMonthList == "10") ? "3" : (sMonthList == "11") ? "4" : (sMonthList == "12") ? "5" : "";
                    string sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSeminarWorkshopOrganizedForReportByAcademicYearandMonth) + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    var liReport = (List<IQAC_SEMINOR_WORKSOP_ORGANIZED>)CMSHandler.FetchData<IQAC_SEMINOR_WORKSOP_ORGANIZED>(new IQAC_EXTENSION_WORKS()
                    {
                        MONTH_ID = sMonthList,
                        ACADEMIC_YEAR = sAcademicYearList
                    }, sSQL, sAcademicYear).DataSource.Data;


                    if (liReport != null && liReport.Count > 0)
                        ObjViewModel.liSeminorWorkshopOrganized = liReport;
                    else
                        ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACSeminarOrWorkshopOrganized", ex.Message);
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

        public JsonResult BlockMonthListForIQAC(string sAcademicYearList, string sIQACQuestionsList, string sMonthList, string sStaff)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new CALENDER_2017();
                    var sMonth = sMonthList.Split(' ');
                    ObjModel.DAY_ORDER_MONTH = sMonth[0];
                    var DisableMonthForIQACEntry = CMSHandler.SaveOrUpdate(ObjModel, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DisableMonthForIQACEntry), sAcademicYearList);
                    if (DisableMonthForIQACEntry.Success && DisableMonthForIQACEntry.RowsAffected > 0)
                    {
                        ObjJsonData.Message = sMonthList + "Removed For IQAC Entry Successfully !";
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
                        ObjLog.WriteError("IQACController", "BlockMonthListForIQAC", ex.Message);
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

        #region IQAC Individual Report By Staff
        public ActionResult IQACIndividualReport()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
                            SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.sAcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    var liStaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null,
                            SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTeachingStaffOnly)).DataSource.Data;
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        ObjViewModel.sStaffList = new SelectList(liStaff, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    }

                    ObjViewModel.liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumberForReport), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "IQACIndividualReport", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindIQACIndividualReport(string sAcYear, string sStaff)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchTeachingsubjectsworkloadForReportByAcademicYearandMonth) + "AND IPN.STAFF_ID=" + sStaff + " ORDER BY STF.DEPARTMENT,STF.STAFF_ID; ";
                    ObjViewModel.liTeachingSubjectWorkload = (List<IQAC_TEACHING_SUBJECTS_WORKLOAD>)CMSHandler.FetchData<IQAC_TEACHING_SUBJECTS_WORKLOAD>(new IQAC_TEACHING_SUBJECTS_WORKLOAD()
                    {
                        //MONTH_ID = sMonthId,
                        ACADEMIC_YEAR = sAcYear,
                        STAFF_ID = sStaff
                    }, sSQL, sAcademicYear).DataSource.Data;

                    // ----------------------------------------------
                    sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPreparationandUpdationNotesForReportByAcademicYearandMonth) + "AND IPN.STAFF_ID=" + sStaff + " ORDER BY STF.DEPARTMENT,STF.STAFF_ID;";
                    ObjViewModel.liPreparation = (List<IQAC_PREPARATION_AND_UPDATION_OF_NOTES>)CMSHandler.FetchData<IQAC_PREPARATION_AND_UPDATION_OF_NOTES>(new IQAC_PREPARATION_AND_UPDATION_OF_NOTES()
                    {
                        //MONTH_ID = sMonthId,
                        ACADEMIC_YEAR = sAcYear,
                        STAFF_ID = sStaff
                    }, sSQL, sAcademicYear).DataSource.Data;

                    // -----------------------------------------------
                    sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchCoCurrricularAspectsForReportByAcademicYearandMonth) + "AND IPN.STAFF_ID=" + sStaff + " ORDER BY STF.DEPARTMENT,STF.STAFF_ID; ";
                    ObjViewModel.liCurricularAspects = (List<IQAC_CO_CURRICULAR_ASPECTS>)CMSHandler.FetchData<IQAC_CO_CURRICULAR_ASPECTS>(new IQAC_CO_CURRICULAR_ASPECTS()
                    {
                        //MONTH_ID = sMonthId,
                        ACADEMIC_YEAR = sAcYear,
                        STAFF_ID = sStaff
                    }, sSQL, sAcademicYear).DataSource.Data;

                    // --------------------------------------------
                    sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchExtensionWorksForReportByAcademicYearandMonth) + "AND IPN.STAFF_ID=" + sStaff + " ORDER BY STF.DEPARTMENT,STF.STAFF_ID;";
                    ObjViewModel.liExtensionWorks = (List<IQAC_EXTENSION_WORKS>)CMSHandler.FetchData<IQAC_EXTENSION_WORKS>(new IQAC_EXTENSION_WORKS()
                    {
                        //MONTH_ID = sMonthId,
                        ACADEMIC_YEAR = sAcYear,
                        STAFF_ID = sStaff
                    }, sSQL, sAcademicYear).DataSource.Data;

                    // ---------------------------------------------
                    sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAwardsForReportByAcademicYearandMonth) + "AND IA.STAFF_ID=" + sStaff + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";
                    ObjViewModel.liAwards = (List<IQAC_AWARDS>)CMSHandler.FetchData<IQAC_AWARDS>(new IQAC_EXTENSION_WORKS()
                    {
                        //MONTH_ID = sMonthId,
                        ACADEMIC_YEAR = sAcYear,
                        STAFF_ID = sStaff
                    }, sSQL, sAcademicYear).DataSource.Data;

                    // --------------------------------------------
                    sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSubjectAssociationForReportByAcademicYearandMonth) + "AND IA.STAFF_ID=" + sStaff + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";
                    ObjViewModel.liSubjectAssociation = (List<IQAC_SUBJECT_ASSOCIATION>)CMSHandler.FetchData<IQAC_SUBJECT_ASSOCIATION>(new IQAC_EXTENSION_WORKS()
                    {
                        //MONTH_ID = sMonthId,
                        ACADEMIC_YEAR = sAcYear,
                        STAFF_ID = sStaff
                    }, sSQL, sAcademicYear).DataSource.Data;

                    // -------------------------------------------
                    sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchHonorForReportByAcademicYearandMonth) + "AND IA.STAFF_ID=" + sStaff + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    ObjViewModel.liHonor = (List<IQAC_HONOURS>)CMSHandler.FetchData<IQAC_HONOURS>(new IQAC_EXTENSION_WORKS()
                    {
                        //MONTH_ID = sMonthId,
                        ACADEMIC_YEAR = sAcYear,
                        STAFF_ID = sStaff
                    }, sSQL, sAcademicYear).DataSource.Data;

                    // ------------------------------------------
                    sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchMemberShipForReportByAcademicYearandMonth) + "AND IA.STAFF_ID=" + sStaff + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    ObjViewModel.liMembership = (List<IQAC_MEMBERSHIP>)CMSHandler.FetchData<IQAC_MEMBERSHIP>(new IQAC_EXTENSION_WORKS()
                    {
                        //MONTH_ID = sMonthId,
                        ACADEMIC_YEAR = sAcYear,
                        STAFF_ID = sStaff
                    }, sSQL, sAcademicYear).DataSource.Data;

                    // ------------------------------------------
                    sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchProjectForReportByAcademicYearandMonth) + "AND IP.STAFF_ID=" + sStaff + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    ObjViewModel.liIQACProject = (List<IQAC_PROJECT>)CMSHandler.FetchData<IQAC_PROJECT>(new IQAC_EXTENSION_WORKS()
                    {
                        //MONTH_ID = sMonthId,
                        ACADEMIC_YEAR = sAcYear,
                        STAFF_ID = sStaff
                    }, sSQL, sAcademicYear).DataSource.Data;


                    // -----------------------------------------
                    sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResearchGuidenceForReportByAcademicYearandMonth) + "AND I.STAFF_ID=" + sStaff + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    ObjViewModel.liIQACResearchGuidence = (List<IQAC_RESEARCH_GUIDENCE>)CMSHandler.FetchData<IQAC_RESEARCH_GUIDENCE>(new IQAC_EXTENSION_WORKS()
                    {
                        //MONTH_ID = sMonthId,
                        ACADEMIC_YEAR = sAcYear,
                        STAFF_ID = sStaff
                    }, sSQL, sAcademicYear).DataSource.Data;

                    // ----------------------------------------
                    sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPHDGuidenceForReportByAcademicYearandMonth) + "AND I.STAFF_ID=" + sStaff + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    ObjViewModel.liIQACPhdGuidence = (List<IQAC_PHD_GUIDENCE>)CMSHandler.FetchData<IQAC_PHD_GUIDENCE>(new IQAC_EXTENSION_WORKS()
                    {
                        //MONTH_ID = sMonthId,
                        ACADEMIC_YEAR = sAcYear,
                        STAFF_ID = sStaff
                    }, sSQL, sAcademicYear).DataSource.Data;


                    // --------------------------------------
                    sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPublicationAndPrasentationForReportByAcademicYearandMonth) + "AND I.STAFF_ID=" + sStaff + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    ObjViewModel.liStaffActivity = (List<IQAC_STAFF_ACTIVITY>)CMSHandler.FetchData<IQAC_STAFF_ACTIVITY>(new IQAC_EXTENSION_WORKS()
                    {
                        //MONTH_ID = sMonthId,
                        ACADEMIC_YEAR = sAcYear,
                        STAFF_ID = sStaff
                    }, sSQL, sAcademicYear).DataSource.Data;


                    // --------------------------------------
                    sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPaperPublicationForReportByAcademicYearandMonth) + "AND I.STAFF_ID=" + sStaff + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    ObjViewModel.liPaperPublication = (List<IQAC_PAPER_PUBLICATION>)CMSHandler.FetchData<IQAC_PAPER_PUBLICATION>(new IQAC_EXTENSION_WORKS()
                    {
                        //MONTH_ID = sMonthId,
                        ACADEMIC_YEAR = sAcYear,
                        STAFF_ID = sStaff
                    }, sSQL, sAcademicYear).DataSource.Data;


                    // --------------------------------------
                    sSQL = string.Empty;
                    sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchBookPublicationForReportByAcademicYearandMonth) + "AND I.STAFF_ID=" + sStaff + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    ObjViewModel.liBookPublication = (List<IQAC_BOOK_PUBLICATION>)CMSHandler.FetchData<IQAC_BOOK_PUBLICATION>(new IQAC_EXTENSION_WORKS()
                    {
                        //MONTH_ID = sMonthId,
                        ACADEMIC_YEAR = sAcYear,
                        STAFF_ID = sStaff
                    }, sSQL, sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACIndividualReport", ex.Message);
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

        #region IQAC Report ( Date Wise )
        public ActionResult IQACDateWiseReportForIncharge()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
                            SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.sAcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    var liDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null,
                            SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    if (liDepartment != null && liDepartment.Count > 0)
                    {
                        ObjViewModel.sDepartmentList = new SelectList(liDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "IQACDateWiseReportForIncharge", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindIQACDateWiseReportForIncharge(string sJsonData)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sStaffId))
            {
                try
                {
                    var sResult = JsonConvert.DeserializeObject<IQACReport>(sJsonData);
                    sResult.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(sResult.DATE_FROM);
                    sResult.DATE_TO = CommonMethods.ConvertdatetoyearFromat(sResult.DATE_TO);
                    ObjViewModel.sMessage = sResult.QUESTION_ID;
                    //string sAcademicYear = sResult.ACADEMIC_YEAR.ToString();
                    string sAcademicYear = string.Empty;
                    string sSQL = string.Empty;
                    switch (sResult.QUESTION_ID)
                    {

                        case Common.IQAC_QUSTIONS.CO_CURRICULAR_ASPECTS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDateWiseReportForCoCurricularAspects).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liCurricularAspects = (List<IQAC_CO_CURRICULAR_ASPECTS>)CMSHandler.FetchData<IQAC_CO_CURRICULAR_ASPECTS>(sResult, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.EXTENSION_WORKS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDateWiseReportForExtensionWorks).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liExtensionWorks = (List<IQAC_EXTENSION_WORKS>)CMSHandler.FetchData<IQAC_EXTENSION_WORKS>(sResult, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.AWARDS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDateWiseReportforAwards).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liAwards = (List<IQAC_AWARDS>)CMSHandler.FetchData<IQAC_AWARDS>(sResult, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.SUBJECT_ASSOCIATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDateWiseReportSubjectAssociation).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liSubjectAssociation = (List<IQAC_SUBJECT_ASSOCIATION>)CMSHandler.FetchData<IQAC_SUBJECT_ASSOCIATION>(sResult, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.HONOURS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDateWiseReportForHonour).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liHonor = (List<IQAC_HONOURS>)CMSHandler.FetchData<IQAC_HONOURS>(sResult, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.MEMBERSHIP:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDateWiseReportForMembership).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liMembership = (List<IQAC_MEMBERSHIP>)CMSHandler.FetchData<IQAC_MEMBERSHIP>(sResult, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PROJECT:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDateWiseReportForProject).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liIQACProject = (List<IQAC_PROJECT>)CMSHandler.FetchData<IQAC_PROJECT>(sResult, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.RESEARCH_GUIDENCE:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDateWiseReportForResearchGuidence).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liIQACResearchGuidence = (List<IQAC_RESEARCH_GUIDENCE>)CMSHandler.FetchData<IQAC_RESEARCH_GUIDENCE>(sResult, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PHD_GUIDENCE:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDateWiseReportForPHDGuidence).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liIQACPhdGuidence = (List<IQAC_PHD_GUIDENCE>)CMSHandler.FetchData<IQAC_PHD_GUIDENCE>(sResult, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PUBLICATION_AND_PRASENTATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDateWiseReportForPrasentedAndParticipated).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liStaffActivity = (List<IQAC_STAFF_ACTIVITY>)CMSHandler.FetchData<IQAC_STAFF_ACTIVITY>(sResult, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PAPER_PUBLICATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDateWiseReportForPaperPublication).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liPaperPublication = (List<IQAC_PAPER_PUBLICATION>)CMSHandler.FetchData<IQAC_PAPER_PUBLICATION>(sResult, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.BOOK_PUBLICATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDateWiseReportForBookPublication).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liBookPublication = (List<IQAC_BOOK_PUBLICATION>)CMSHandler.FetchData<IQAC_BOOK_PUBLICATION>(sResult, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }

                        case Common.IQAC_QUSTIONS.DEPARTMENT_DAILY_ACTIVITY:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDateWiseReportForDepartmentDailyActivity).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liDailyActivity = (List<DEPT_DAILY_ACTIVITY>)CMSHandler.FetchData<DEPT_DAILY_ACTIVITY>(sResult, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.ASSOCIATION_ACTIVITIES:
                            {
                                sSQL = string.Empty;
                                //sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAssociationActivityForReportByAcademicYearandMonth) + " AND I.ACTIVITY_DATE BETWEEN '" + sResult.DATE_FROM + "' AND '" + sResult.DATE_TO + "' AND I.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY I.ACTIVITY_DATE,SP.DEPARTMENT,SP.STAFF_ID;";
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDateWiseReportForAssociationActivity).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liAssociationActivity = (List<IQAC_ASSOCIATION_ACTIVITIES>)CMSHandler.FetchData<IQAC_ASSOCIATION_ACTIVITIES>(sResult, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.SEMINAR_WORKSHOP_ORGANIZED:
                            {
                                sSQL = string.Empty;
                                //sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSeminarWorkshopOrganizedForReportByAcademicYearandMonth) + " AND I.ORGANIZED_DATE BETWEEN '" + sResult.DATE_FROM + "' AND '" + sResult.DATE_TO + "' AND I.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY I.ORGANIZED_DATE,SP.DEPARTMENT,SP.STAFF_ID;";
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDateWiseReportForSeminarWorkshopOrganized).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liSeminorWorkshopOrganized = (List<IQAC_SEMINOR_WORKSOP_ORGANIZED>)CMSHandler.FetchData<IQAC_SEMINOR_WORKSOP_ORGANIZED>(sResult, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        default:
                            break;
                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACDateWiseReportForIncharge", ex.Message);
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

        #region IQAC Report ( HOD'S )
        public ActionResult IQACReportForHOD()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new STF_PERSONAL_INFO();
                    ObjModel.STAFF_ID = sStaffId;
                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
                            SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.sAcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    var liStaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(ObjModel,
                            SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffUserDepartmentId), sAcademicYear).DataSource.Data;
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        ObjViewModel.sMessage = liStaff.FirstOrDefault().DEPARTMENT;
                        ObjViewModel.sStaffList = new SelectList(liStaff, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog Objlog = new ErrorLog())
                    {
                        Objlog.WriteError("IQACController", "IQACReportForHOD", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        // Not In Use
        public ActionResult BindIQACReportForHOD(string sJsonData)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var sResult = JsonConvert.DeserializeObject<IQACReport>(sJsonData);
                    sResult.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(sResult.DATE_FROM);
                    sResult.DATE_TO = CommonMethods.ConvertdatetoyearFromat(sResult.DATE_TO);
                    string sSQL = string.Empty;
                    string ACADEMIC_YEAR = string.Empty;
                    switch (sResult.QUESTION_ID)
                    {
                        case Common.IQAC_QUSTIONS.CO_CURRICULAR_ASPECTS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchCoCurrricularAspectsForReportByAcademicYearandMonth).Replace("?MONTH_ID", "1,2,3,4,5,6,7,8,9,10,11,12");
                                sSQL = sSQL + " AND IPN.ACTIVITY_DATE BETWEEN '" + sResult.DATE_FROM + "' AND '" + sResult.DATE_TO + "' AND IPN.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY STF.DEPARTMENT,STF.STAFF_ID; ";
                                ObjViewModel.liCurricularAspects = (List<IQAC_CO_CURRICULAR_ASPECTS>)CMSHandler.FetchData<IQAC_CO_CURRICULAR_ASPECTS>(new IQAC_CO_CURRICULAR_ASPECTS()
                                {
                                    ACADEMIC_YEAR = sResult.ACADEMIC_YEAR,
                                    STAFF_ID = sResult.STAFF_ID
                                }, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.EXTENSION_WORKS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchExtensionWorksForReportByAcademicYearandMonth).Replace("?MONTH_ID", "1,2,3,4,5,6,7,8,9,10,11,12");
                                sSQL = sSQL + " AND IPN.ACTIVITY_DATE BETWEEN '" + sResult.DATE_FROM + "' AND '" + sResult.DATE_TO + "'" + " AND IPN.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY STF.DEPARTMENT,STF.STAFF_ID;";
                                ObjViewModel.liExtensionWorks = (List<IQAC_EXTENSION_WORKS>)CMSHandler.FetchData<IQAC_EXTENSION_WORKS>(new IQAC_EXTENSION_WORKS()
                                {
                                    ACADEMIC_YEAR = sResult.ACADEMIC_YEAR,
                                    STAFF_ID = sResult.STAFF_ID
                                }, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.AWARDS:
                            {
                                //sSQL = string.Empty;
                                //sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAwardsForReportByAcademicYearandMonth) + " AND IA.YEAR BETWEEN '" + sResult.DATE_FROM.Substring(0, 4) + "' AND '" + sResult.DATE_TO.Substring(0, 4) + "' AND IA.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";
                                //ObjViewModel.liAwards = (List<IQAC_AWARDS>)CMSHandler.FetchData<IQAC_AWARDS>(new IQAC_EXTENSION_WORKS()
                                //{
                                //    ACADEMIC_YEAR = sResult.ACADEMIC_YEAR,
                                //    STAFF_ID = sResult.STAFF_ID
                                //}, sSQL, sAcademicYear).DataSource.Data;
                         
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAwardsForReportByAcademicYearandMonth).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sResult.STAFF_ID);
                                ObjViewModel.liAwards = (List<IQAC_AWARDS>)CMSHandler.FetchData<IQAC_AWARDS>(sResult, sSQL, ACADEMIC_YEAR).DataSource.Data;
                             
                                break;
                            }
                        case Common.IQAC_QUSTIONS.SUBJECT_ASSOCIATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSubjectAssociationForReportByAcademicYearandMonth) + " AND IA.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";
                                ObjViewModel.liSubjectAssociation = (List<IQAC_SUBJECT_ASSOCIATION>)CMSHandler.FetchData<IQAC_SUBJECT_ASSOCIATION>(new IQAC_EXTENSION_WORKS()
                                {
                                    ACADEMIC_YEAR = sResult.ACADEMIC_YEAR,
                                    STAFF_ID = sResult.STAFF_ID
                                }, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.MEMBERSHIP:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchMemberShipForReportByAcademicYearandMonth) + " AND IA.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";
                                ObjViewModel.liMembership = (List<IQAC_MEMBERSHIP>)CMSHandler.FetchData<IQAC_MEMBERSHIP>(new IQAC_EXTENSION_WORKS()
                                {
                                    ACADEMIC_YEAR = sResult.ACADEMIC_YEAR,
                                    STAFF_ID = sResult.STAFF_ID
                                }, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.HONOURS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchHonorForReportByAcademicYearandMonth) + " AND IA.STAFF_ID IN (" + sResult.STAFF_ID + ") AND IA.DATE BETWEEN '" + sResult.DATE_FROM + "' AND '" + sResult.DATE_TO + "'" + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";

                                ObjViewModel.liHonor = (List<IQAC_HONOURS>)CMSHandler.FetchData<IQAC_HONOURS>(new IQAC_EXTENSION_WORKS()
                                {
                                    ACADEMIC_YEAR = sResult.ACADEMIC_YEAR,
                                    STAFF_ID = sResult.STAFF_ID
                                }, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PROJECT:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchProjectForReportByAcademicYearandMonth) + " AND IP.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";
                                ObjViewModel.liIQACProject = (List<IQAC_PROJECT>)CMSHandler.FetchData<IQAC_PROJECT>(new IQAC_EXTENSION_WORKS()
                                {
                                    ACADEMIC_YEAR = sResult.ACADEMIC_YEAR,
                                    STAFF_ID = sResult.STAFF_ID
                                }, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.RESEARCH_GUIDENCE:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResearchGuidenceForReportByAcademicYearandMonth) + " AND I.REGISTRATION_YEAR BETWEEN '" + sResult.DATE_FROM.Substring(0, 4) + "' AND '" + sResult.DATE_TO.Substring(0, 4) + "'  AND I.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";
                                ObjViewModel.liIQACResearchGuidence = (List<IQAC_RESEARCH_GUIDENCE>)CMSHandler.FetchData<IQAC_RESEARCH_GUIDENCE>(new IQAC_EXTENSION_WORKS()
                                {
                                    ACADEMIC_YEAR = sResult.ACADEMIC_YEAR,
                                    STAFF_ID = sResult.STAFF_ID
                                }, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PHD_GUIDENCE:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPHDGuidenceForReportByAcademicYearandMonth) + " AND I.GUDIENCE_DATE BETWEEN '" + sResult.DATE_FROM + "' AND '" + sResult.DATE_TO + "' AND I.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";
                                ObjViewModel.liIQACPhdGuidence = (List<IQAC_PHD_GUIDENCE>)CMSHandler.FetchData<IQAC_PHD_GUIDENCE>(new IQAC_EXTENSION_WORKS()
                                {
                                    ACADEMIC_YEAR = sResult.ACADEMIC_YEAR,
                                    STAFF_ID = sResult.STAFF_ID
                                }, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PUBLICATION_AND_PRASENTATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPublicationAndPrasentationForReportByAcademicYearandMonth) + " AND I.ACTIVITY_DATE BETWEEN '" + sResult.DATE_FROM + "' AND '" + sResult.DATE_TO + "' AND I.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";
                                ObjViewModel.liStaffActivity = (List<IQAC_STAFF_ACTIVITY>)CMSHandler.FetchData<IQAC_STAFF_ACTIVITY>(new IQAC_EXTENSION_WORKS()
                                {
                                    ACADEMIC_YEAR = sResult.ACADEMIC_YEAR,
                                    STAFF_ID = sResult.STAFF_ID
                                }, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PAPER_PUBLICATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPaperPublicationForReportByAcademicYearandMonth) + " AND I.PUBLICATION_YEAR  BETWEEN '" + sResult.DATE_FROM.Substring(0, 4) + "' AND '" + sResult.DATE_TO.Substring(0, 4) + "'  AND I.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";
                                ObjViewModel.liPaperPublication = (List<IQAC_PAPER_PUBLICATION>)CMSHandler.FetchData<IQAC_PAPER_PUBLICATION>(new IQAC_EXTENSION_WORKS()
                                {
                                    ACADEMIC_YEAR = sResult.ACADEMIC_YEAR,
                                    STAFF_ID = sResult.STAFF_ID
                                }, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.BOOK_PUBLICATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchBookPublicationForReportByAcademicYearandMonth) + " AND I.PUBLICATION_YEAR BETWEEN '" + sResult.DATE_FROM.Substring(0, 4) + "' AND '" + sResult.DATE_TO.Substring(0, 4) + "'  AND I.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";
                                ObjViewModel.liBookPublication = (List<IQAC_BOOK_PUBLICATION>)CMSHandler.FetchData<IQAC_BOOK_PUBLICATION>(new IQAC_EXTENSION_WORKS()
                                {
                                    ACADEMIC_YEAR = sResult.ACADEMIC_YEAR,
                                    STAFF_ID = sResult.STAFF_ID
                                }, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.DEPARTMENT_DAILY_ACTIVITY:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchDepartmentDailyActivityForReportByAcademicYearandMonth) + " AND I.ACTIVITY_DATE BETWEEN '" + sResult.DATE_FROM + "' AND '" + sResult.DATE_TO + "' AND I.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";
                                ObjViewModel.liDailyActivity = (List<DEPT_DAILY_ACTIVITY>)CMSHandler.FetchData<DEPT_DAILY_ACTIVITY>(new IQAC_EXTENSION_WORKS()
                                {
                                    ACADEMIC_YEAR = sResult.ACADEMIC_YEAR,
                                    STAFF_ID = sResult.STAFF_ID
                                }, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.ASSOCIATION_ACTIVITIES:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAssociationActivityForReportByAcademicYearandMonth) + " AND I.ACTIVITY_DATE BETWEEN '" + sResult.DATE_FROM + "' AND '" + sResult.DATE_TO + "' AND I.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";
                                ObjViewModel.liAssociationActivity = (List<IQAC_ASSOCIATION_ACTIVITIES>)CMSHandler.FetchData<IQAC_ASSOCIATION_ACTIVITIES>(new IQAC_EXTENSION_WORKS()
                                {
                                    ACADEMIC_YEAR = sResult.ACADEMIC_YEAR,
                                    STAFF_ID = sResult.STAFF_ID
                                }, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.SEMINAR_WORKSHOP_ORGANIZED:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSeminarWorkshopOrganizedForReportByAcademicYearandMonth) + " AND I.ORGANIZED_DATE BETWEEN '" + sResult.DATE_FROM + "' AND '" + sResult.DATE_TO + "' AND I.STAFF_ID IN (" + sResult.STAFF_ID + ") ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";
                                ObjViewModel.liSeminorWorkshopOrganized = (List<IQAC_SEMINOR_WORKSOP_ORGANIZED>)CMSHandler.FetchData<IQAC_SEMINOR_WORKSOP_ORGANIZED>(new IQAC_EXTENSION_WORKS()
                                {
                                    ACADEMIC_YEAR = sResult.ACADEMIC_YEAR,
                                    STAFF_ID = sResult.STAFF_ID
                                }, sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACReportForHOD", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    return Json(ObjJsonData);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                return Json(ObjJsonData);
            }
            return View(ObjViewModel);
        }
        #endregion

        #region IQAC Report ( Submitted / Not Submitted )
        public ActionResult IQACStatus()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
                        SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.sAcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    var liDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null,
                            SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    if (liDepartment != null && liDepartment.Count > 0)
                    {
                        ObjViewModel.sDepartmentList = new SelectList(liDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    }

                    // StaffList
                    var Listaff = (List<IQACModel>)CMSHandler.FetchData<IQACModel>(null, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchTechingStaffOnly), sAcademicYear).DataSource.Data;
                    if (Listaff != null && Listaff.Count > 0)
                    {
                        ObjViewModel.sStaffList = new SelectList(Listaff, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STU_PERSONAL_INFO.FIRST_NAME);
                    }
                    // Month List ,..
                    ObjViewModel.liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumberForReport), sAcademicYear).DataSource.Data;


                    // IQAC Questions
                    var liQuestionsList = (List<IQAC_QUESTION>)CMSHandler.FetchData<IQAC_QUESTION>(null, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchIQACQuestion), sAcademicYear).DataSource.Data;
                    if (liQuestionsList != null && liQuestionsList.Count > 0)
                    {
                        ObjViewModel.sIQACQuestionsList = new SelectList(liQuestionsList, "IQAC_QUESTION_ID", "IQAC_QUESTIONS");
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "IQACStatusReport", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindIQACStatusForInchrage(string sJsonData)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sSQL = string.Empty;
                    var Result = JsonConvert.DeserializeObject<IQACReport>(sJsonData);
                    switch (Result.QUESTION_ID)
                    {
                        case Common.IQAC_QUSTIONS.TEACHING_SUBJECTS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchWorkloadStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.TEACHING_LEARNING:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchTeachingLearningStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.LESSON_PLAN:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchLessonPlanStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.CO_CURRICULAR_ASPECTS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchCoCurriculerAspectsStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.EXTENSION_WORKS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchExtentionWorkStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.CURRICULAR_ASPECTS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchCurriculerAspectsStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.AWARDS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAwardsStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.SUBJECT_ASSOCIATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSubjectAssociationStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.HONOURS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchHonourStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.MEMBERSHIP:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchMembershipStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PROJECT:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchProjectStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.RESEARCH_GUIDENCE:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResearchGuidenceStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PHD_GUIDENCE:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPHDGuidenceStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PUBLICATION_AND_PRASENTATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPublicationAndPrasentationStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PAPER_PUBLICATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPaperPublicationStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.BOOK_PUBLICATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchBookPublicationStatus1).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog Objlog = new ErrorLog())
                    {
                        Objlog.WriteError("IQACController", "BindIQACStatus", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
            }
            return View(ObjViewModel);
        }

        public ActionResult BindIQACStatus(string sJsonData)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
           
            if (!string.IsNullOrEmpty(sStaffId))
            {
                try
                {
                    string sSQL = string.Empty;
                    var Result = JsonConvert.DeserializeObject<IQACReport>(sJsonData);
                    string sAcademicYear = Result.ACADEMIC_YEAR;
                    switch (Result.QUESTION_ID)
                    {
                        case Common.IQAC_QUSTIONS.TEACHING_SUBJECTS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchWorkloadStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.TEACHING_LEARNING:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchTeachingLearningStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.LESSON_PLAN:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchLessonPlanStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.CO_CURRICULAR_ASPECTS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchCoCurriculerAspectsStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.EXTENSION_WORKS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchExtentionWorkStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.CURRICULAR_ASPECTS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchCurriculerAspectsStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.AWARDS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAwardsStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.SUBJECT_ASSOCIATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSubjectAssociationStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.HONOURS:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchHonourStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.MEMBERSHIP:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchMembershipStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PROJECT:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchProjectStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.RESEARCH_GUIDENCE:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResearchGuidenceStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PHD_GUIDENCE:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPHDGuidenceStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PUBLICATION_AND_PRASENTATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPublicationAndPrasentationStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.PAPER_PUBLICATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPaperPublicationStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        case Common.IQAC_QUSTIONS.BOOK_PUBLICATION:
                            {
                                sSQL = string.Empty;
                                sSQL = IQACSQL.GetIQACSQL(IQACEnumCommands.FetchBookPublicationStatus).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, Result.STAFF_ID);
                                ObjViewModel.liReport = (List<IQACReport>)CMSHandler.FetchData<IQACReport>(Result,
                                        sSQL, sAcademicYear).DataSource.Data;
                                break;
                            }
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog Objlog = new ErrorLog())
                    {
                        Objlog.WriteError("IQACController", "BindIQACStatus", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
            }
            return View(ObjViewModel);
        }

        public ActionResult IQACIndividualStatus()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
                        SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.sAcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    // Month List ,..
                    //ObjViewModel.liMonthList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthWithYearAndAutoNumberForReport), sAcademicYear).DataSource.Data;


                    // IQAC Questions ...
                    var liQuestionsList = (List<IQAC_QUESTION>)CMSHandler.FetchData<IQAC_QUESTION>(null, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchIQACQuestion), sAcademicYear).DataSource.Data;
                    if (liQuestionsList != null && liQuestionsList.Count > 0)
                    {
                        ObjViewModel.sIQACQuestionsList = new SelectList(liQuestionsList, "IQAC_QUESTION_ID", "IQAC_QUESTIONS");
                    }
                    ObjViewModel.sMessage = sUserId;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "IQACIndividualStatus", ex.Message);
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

        #region IQAC Report (Year Wise)
        public ActionResult IQACYearwiseReport()
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
                        SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.sAcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "IQACYearwiseReport", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindIQACYearwiseReport(string sAcYear, string sQuestionId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    switch (sQuestionId)
                    {
                        case Common.IQAC_QUSTIONS.AWARDS:
                            {
                                return RedirectToAction("BindIQACYearwiseReportForAwards", "IQAC", new { sAcYear = sAcYear, sQuestionId = sQuestionId });
                            }
                        case Common.IQAC_QUSTIONS.SUBJECT_ASSOCIATION:
                            {
                                return RedirectToAction("BindIQACYearwiseReportForSubjectAssociation", "IQAC", new { sAcYear = sAcYear, sQuestionId = sQuestionId });
                            }
                        case Common.IQAC_QUSTIONS.MEMBERSHIP:
                            {
                                return RedirectToAction("BindIQACYearwiseReportForMembership", "IQAC", new { sAcYear = sAcYear, sQuestionId = sQuestionId });
                            }
                        case Common.IQAC_QUSTIONS.PROJECT:
                            {
                                return RedirectToAction("BindIQACYearwiseReportForProject", "IQAC", new { sAcYear = sAcYear, sQuestionId = sQuestionId });
                            }
                        case Common.IQAC_QUSTIONS.RESEARCH_GUIDENCE:
                            {
                                return RedirectToAction("BindIQACYearwiseReportForResearchGuidence", "IQAC", new { sAcYear = sAcYear, sQuestionId = sQuestionId });
                            }
                        case Common.IQAC_QUSTIONS.PAPER_PUBLICATION:
                            {
                                return RedirectToAction("BindIQACYearwiseReportForPaperPublication", new { sAcYear = sAcYear, sQuestionId = sQuestionId });
                               
                            }
                        case Common.IQAC_QUSTIONS.BOOK_PUBLICATION:
                            {
                                return RedirectToAction("BindIQACYearwiseReportForBookPublication", "IQAC", new { sAcYear = sAcYear, sQuestionId = sQuestionId });
                            }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACYearwiseReport", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindIQACYearwiseReportForAwards(string sAcYear, string sQuestionId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liAwards = (List<IQAC_AWARDS>)CMSHandler.FetchData<IQAC_AWARDS>(null,IQACSQL.GetIQACSQL(IQACEnumCommands.FetchAwardsForReportByAcademicYear), sAcYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACYearwiseReportForAwards", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindIQACYearwiseReportForSubjectAssociation(string sAcYear, string sQuestionId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liSubjectAssociation = (List<IQAC_SUBJECT_ASSOCIATION>)CMSHandler.FetchData<IQAC_SUBJECT_ASSOCIATION>(null, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchSubjectAssociationForReportByAcademicYear), sAcYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACYearwiseReportForSubjectAssociation", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindIQACYearwiseReportForMembership(string sAcYear, string sQuestionId)
         {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liMembership = (List<IQAC_MEMBERSHIP>)CMSHandler.FetchData<IQAC_MEMBERSHIP>(null, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchMemberShipForReportByAcademicYear), sAcYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACYearwiseReportForMembership", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindIQACYearwiseReportForProject(string sAcYear, string sQuestionId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liIQACProject = (List<IQAC_PROJECT>)CMSHandler.FetchData<IQAC_PROJECT>(null, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchProjectForReportByAcademicYear), sAcYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACYearwiseReportForProject", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindIQACYearwiseReportForResearchGuidence(string sAcYear, string sQuestionId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liIQACResearchGuidence = (List<IQAC_RESEARCH_GUIDENCE>)CMSHandler.FetchData<IQAC_RESEARCH_GUIDENCE>(null, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchResearchGuidenceForReportByAcademicYear), sAcYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACYearwiseReportForResearchGuidence", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindIQACYearwiseReportForPaperPublication(string sAcYear, string sQuestionId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //string Sql = string.Empty;
                    //Sql= IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPaperPublicationForReportByAcademicYearandMonth) + " AND I.PUBLICATION_YEAR="+ sAcYear + " ORDER BY SP.DEPARTMENT,SP.STAFF_ID";
                    //var LiReport = (List<IQAC_PAPER_PUBLICATION>)CMSHandler.FetchData<IQAC_PAPER_PUBLICATION>(null, Sql, sAcademicYear).DataSource.Data;
                    //if (LiReport != null && LiReport.Count > 0)
                    //{
                    //    ObjViewModel.liPaperPublication = LiReport;
                    //}
                    //else
                    //    ObjViewModel.sMessage = Common.Messages.NoRecordsfound;
                    
                    ObjViewModel.liPaperPublication = (List<IQAC_PAPER_PUBLICATION>)CMSHandler.FetchData<IQAC_PAPER_PUBLICATION>(null, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchPaperPublicationForReportByAcademicYear), sAcYear).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())  
                    {
                        ObjLog.WriteError("IQACController", "BindIQACYearwiseReportForPaperPublication", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindIQACYearwiseReportForBookPublication(string sAcYear, string sQuestionId)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liBookPublication = (List<IQAC_BOOK_PUBLICATION>)CMSHandler.FetchData<IQAC_BOOK_PUBLICATION>(null, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchBookPublicationForReportByAcademicYear), sAcYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindIQACYearwiseReportForBookPublication", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        #endregion

        #region IQAC Report ( Lesson Plan )
        public ActionResult LessonPlanReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new IQACViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new STF_PERSONAL_INFO();
                    ObjModel.STAFF_ID = sStaffId;
                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
                            SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.sAcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var liSemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null,
                           SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType), sAcademicYear).DataSource.Data;
                    if (liSemesterType != null && liSemesterType.Count > 0)
                    {
                        ObjViewModel.sSemesterType = new SelectList(liSemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    }
                    var liStaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(ObjModel,
                            SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTeachingStaffOnly), sAcademicYear).DataSource.Data;
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        //ObjViewModel.sMessage = liStaff.FirstOrDefault().DEPARTMENT;
                        ObjViewModel.sStaffList = new SelectList(liStaff, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LessonPlan", "LessonPlanReportForHOD", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindlessonPlanReport(string sAcYear, string sStaffId, string sCourseId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new CourseWiseStaffInfo();
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.COURSE_ID = sCourseId;
                    ObjViewModel.liLessonPlan = (List<STF_LESSON_PLAN>)CMSHandler.FetchData<STF_LESSON_PLAN>(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchListLessonPlan), sAcYear).DataSource.Data;
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "BindlessonPlanReport", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
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
        public ActionResult BindLessonPlanYearWise( string sAcYear, string sSemestType)
        {
            var ObjViewModel = new IQACViewModel();
            var ObjJsonData = new JsonResultData();
            var ObjModel = new IQAC_TEACHING_SUBJECTS_WORKLOAD();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.ACADEMIC_YEAR = sAcYear;
                    ObjViewModel.liLessonPlan = (List<STF_LESSON_PLAN>)CMSHandler.FetchData<STF_LESSON_PLAN>(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.FetchLessonplanYearWiseReport).Replace("?SEMESTER_TYPE_ID", sSemestType), sAcademicYear).DataSource.Data;
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "LessonPlan", ex.Message);
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
        #endregion


    }
}