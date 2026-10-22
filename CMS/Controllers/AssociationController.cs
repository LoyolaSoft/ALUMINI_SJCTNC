using CMS.DAO;
using CMS.Models;
using CMS.SQL.Association;
using CMS.SQL.CertificateCourses;
using CMS.SQL.SupportData;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.Controllers
{
    public class AssociationController : Controller
    {
        // GET: Association
        public ActionResult Index()
        {
            return View();
        }
        #region Association Group Setting
        //public ActionResult ListAssociationGroupSetting()
        //{
        //    AssociationViewModel objViewModel = new AssociationViewModel();
        //    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
        //    if (!string.IsNullOrEmpty(sAcademicYear))
        //    {
        //        objViewModel.liAssociationSetting = (List<ASSOCIATION_GROUP_SETTING>)CMSHandler.FetchData<ASSOCIATION_GROUP_SETTING>(null, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.ListAssociationGroupSetting), sAcademicYear).DataSource.Data;

        //        var liSemester = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null,SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
        //        if (liSemester!=null && liSemester.Count>0)
        //        {
        //            objViewModel.SemesterType = new SelectList(liSemester, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
        //        }
        //        return View(objViewModel);
        //    }
        //    else
        //        return RedirectToAction("ErrorMessage", "Error");
        //}

        //public JsonResult SaveOrUpdateGroupSetting(string sJson)
        //{
        //    var ObjJsonData = new JsonResultData();
        //    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
        //    if (!string.IsNullOrEmpty(sAcademicYear))
        //    {
        //        try
        //        {
        //            var ObjModel = JsonConvert.DeserializeObject<ASSOCIATION_GROUP_SETTING>(sJson);
        //            var SaveOrUpdate = CMSHandler.SaveOrUpdate((!string.IsNullOrEmpty(ObjModel.ASSOCIATION_GROUP_SETTING_ID) ? AssociationSQLCommands.UpdateAssociationStudentRegistration : AssociationSQLCommands.UpdateAssociationStudentRegistration), sAcademicYear);
        //            if (SaveOrUpdate.Success && SaveOrUpdate.RowsAffected!=0)
        //            {
        //                ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
        //                ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
        //            }
        //            else
        //            {
        //                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
        //                ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            using (ErrorLog ObjLog = new ErrorLog())
        //            {
        //                ObjLog.WriteError("SaveOrUpdateGroupSetting", "AssociationController", ex.Message);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
        //        ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
        //    }
        //    return Json(ObjJsonData,JsonRequestBehavior.AllowGet);
        //}
        #endregion
        #region AssociationGroup Student Registration
        public ActionResult AssociationRegistration()
        {
            return View();
        }
        //public string LoadAssociationGroups()
        //{
        //    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
        //    string sResult = string.Empty;
        //    ASSOCIATION_STUDENT_REGISTRATION objModel = new ASSOCIATION_STUDENT_REGISTRATION();
        //    if (!string.IsNullOrEmpty(sAcademicYear))
        //    {
        //        try
        //        {
        //            sResult = ValidateStudent();
        //            if (string.IsNullOrEmpty(sResult))
        //            {
        //                string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
        //                var liStudent = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(new STU_CLASS() { STUDENT_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByStudentId), sAcademicYear).DataSource.Data;
        //                if (liStudent != null && liStudent.Count > 0)
        //                {
        //                    objModel.STUDENT_ID = liStudent.FirstOrDefault().STUDENT_ID;
        //                    objModel.CLASS_ID = liStudent.FirstOrDefault().CLASS_ID;
        //                    var LoadAssociationGroups = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(objModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.LoadAssociationGroups), sAcademicYear).DataSource.Data;
        //                    if (LoadAssociationGroups != null && LoadAssociationGroups.Count > 0)
        //                    {
        //                        foreach (var item in LoadAssociationGroups)
        //                        {
        //                            var ClassCourseQuota = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(item, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.FetchClassQuotaForCourseByCourseIdandClassId), sAcademicYear).DataSource.Data;
        //                            var SumOfCourses = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(item, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.FetchSumofCourseRegisteredByCourseIdandClassId), sAcademicYear).DataSource.Data;
        //                            if (ClassCourseQuota != null && SumOfCourses != null)
        //                            {
        //                                if (Convert.ToInt32(ClassCourseQuota.FirstOrDefault().ALLOTED_SEATS) > Convert.ToInt32(SumOfCourses.FirstOrDefault().SUMOFQUTOA))
        //                                {
        //                                    sResult += "<div class='form-group'> <input type='radio' cms-classid='" + item.CLASS_ID + "' cms-courseid='" + item.COURSE_ID + "'  name='AssociationGroup' id='" + item.COURSE_ID + "' value='" + item.SETTING_ID + "' > <label for='" + item.COURSE_ID + "'> " + item.ASSOCIATION_GROUP + " [ <label class='label label-success'> Alloted-" + ClassCourseQuota.FirstOrDefault().ALLOTED_SEATS + "</label> -" + "<label class='label label-warning'> Applied -" + SumOfCourses.FirstOrDefault().SUMOFQUTOA + "" + "</label> ]" + "</lable></div>";
        //                                }
        //                                else
        //                                {
        //                                    sResult += "<div class='form-group'> <input type='radio' disabled cms-classid='" + item.CLASS_ID + "' cms-courseid='" + item.COURSE_ID + "'  name='AssociationGroup' id='" + item.COURSE_ID + "' value='" + item.SETTING_ID + "' > <label for='" + item.COURSE_ID + "'> " + item.ASSOCIATION_GROUP + " [ <label class='label label-success'> Alloted-" + ClassCourseQuota.FirstOrDefault().ALLOTED_SEATS + "</label> -" + "<label class='label label-warning'> Applied -" + SumOfCourses.FirstOrDefault().SUMOFQUTOA + "" + "</label>]- <label class='label label-danger'>Closed</label>" + "</lable></div>";
        //                                    // sResult += "<div class='form-group'> <input type='radio' disabled cms-semester='" + item[Common.CP_COURSE_ROOT_2017.SEMESTER_ID].ToString() + "' cms-classid='" + item[Common.NME_CLASS_COURSE_2017.CLASS_ID].ToString() + "' cms-nmeclassid='" + item["NME_CLASS"].ToString() + "' cms-courseid='" + item[Common.NME_CLASS_COURSE_2017.COURSE_ID].ToString() + "'  name='NMECourse' id='" + item[Common.NME_CLASS_COURSE_2017.COURSE_ID].ToString() + "' value='" + item[Common.NME_CLASS_COURSE_QUOTA_2017.SETTINGS_ID] + "' > <label for='" + item[Common.NME_CLASS_COURSE_2017.COURSE_ID].ToString() + "'> " + item[Common.CP_COURSE_ROOT_2017.COURSE_TITLE].ToString() + " [ <label class='label label-success'> Alloted-" + SumOfClassCourseQuota.FirstOrDefault().ALLOTED_SEATS + "</label> -" + "<label class='label label-warning'> Applied -" + SumOfClassCourseQuota.FirstOrDefault().SUMOFQUTOA + "" + "</label>]- <label class='label label-danger'>Closed</label>" + "</lable></div>";
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                //sResult = "2";
        //                sResult = @"<h5>Your service oriented course : <i class='text-success'> " + sResult + "</i></h5>";
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            using (ErrorLog objHandler = new ErrorLog())
        //            {
        //                objHandler.WriteError("LoadAssociationGroups", "Association", "Err MSg: " + ex.Message, "Query is empty!");
        //                objHandler.WriteError("LoadAssociationGroups", "Association", ex.Message);
        //            }


        //        }
        //    }
        //    else
        //    {
        //        sResult = string.Empty;
        //    }
        //    return sResult;
        //}
        public string ValidateStudent()
        {
            string sResult = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                try
                {
                    var Course = (List<ASSOCIATION_GROUPS>)CMSHandler.FetchData<ASSOCIATION_GROUPS>(new ASSOCIATION_STUDENT_REGISTRATION { STUDENT_ID = sUserId }, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.CheckStudentIsRegisterd), sAcademicYear).DataSource.Data;
                    if (Course != null && Course.Count > 0)
                    {
                        sResult = Course.FirstOrDefault().ASSOCIATION_GROUP;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ValidateStudent", "Association", "Err MSg : " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ValidateStudent", "Association", ex.Message);
                    }
                }
            }
            else
            {

            }
            return sResult;
        }
        //public string RegisterAssociationGroup(string sClassId, string sCourseId, string sSettingId, string sSemester)
        //{
        //    string sResult = string.Empty;
        //    string sCheckCourse = string.Empty;
        //    ASSOCIATION_STUDENT_REGISTRATION objModel = new ASSOCIATION_STUDENT_REGISTRATION();
        //    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
        //    if (!string.IsNullOrEmpty(sAcademicYear))
        //    {
        //        ResultArgs resultArgs = new ResultArgs();
        //        string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
        //        try
        //        {
        //            objModel.CLASS_ID = sClassId;
        //            objModel.COURSE_ID = sCourseId;
        //            objModel.SETTING_ID = sSettingId;
        //            objModel.SEMESTER_ID = sSemester;
        //            objModel.STUDENT_ID = sUserId;
        //            objModel.ACADEMIC_YEAR = sAcademicYear;
        //            sCheckCourse = checkAssociationAvailablity(sClassId, sCourseId, sSettingId);
        //            if (sCheckCourse.Equals("1"))
        //            {
        //                sResult = ValidateStudent();
        //                if (sResult != "1")
        //                {
        //                    resultArgs = CMSHandler.SaveOrUpdate(objModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.InsertAssociationStudentRegistration), sAcademicYear);
        //                    if (resultArgs.Success)
        //                    {
        //                        resultArgs.StringResult = "1";
        //                    }
        //                    else
        //                    {
        //                        resultArgs.StringResult = "0";
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                resultArgs.Success = false;
        //                resultArgs.StringResult = "3";
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //            using (ErrorLog objHandler = new ErrorLog())
        //            {
        //                objHandler.WriteError("RegisterAssociationGroup", "Association", "Err MSg : " + ex.Message, "Query is empty!");
        //                objHandler.WriteError("RegisterAssociationGroup", "Association", ex.Message);
        //            }
        //        }
        //        sResult = resultArgs.StringResult;
        //    }
        //    else
        //    {
        //        sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
        //    }

        //    return sResult;
        //}
        //public string checkAssociationAvailablity(string sClassId, string sCourseId, string sSettingId)
        //{
        //    string sResult = string.Empty;
        //    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
        //    if (!string.IsNullOrEmpty(sAcademicYear))
        //    {
        //        ASSOCIATION_STUDENT_REGISTRATION objModel = new ASSOCIATION_STUDENT_REGISTRATION();
        //        try
        //        {
        //            objModel.CLASS_ID = sClassId;
        //            objModel.COURSE_ID = sCourseId;
        //            objModel.SETTING_ID = sSettingId;
        //            var CheckAssociationAvailablity = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(objModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.CheckAssociationAvailablity), sAcademicYear).DataSource.Data;
        //            sResult = CheckAssociationAvailablity.FirstOrDefault().COURSEAVAILABILITY;
        //        }
        //        catch (Exception ex)
        //        {
        //            using (ErrorLog objHandler = new ErrorLog())
        //            {
        //                objHandler.WriteError("checkAssociationAvailablity", "Association", "Err MSg: " + ex.Message, "Query is empty!");
        //                objHandler.WriteError("checkAssociationAvailablity", "Association", ex.Message);
        //            }
        //        }
        //    }

        //    return sResult;
        //}
        #endregion

        #region Association Registraion Edit
        public ActionResult ListAssociationRegistered()
        {
            AssociationViewModel objViewModel = new AssociationViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var AssociationGroup = (List<ASSOCIATION_GROUPS>)CMSHandler.FetchData<ASSOCIATION_GROUPS>(null, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.FetchAssociationGroup), sAcademicYear).DataSource.Data;
                if (AssociationGroup != null && AssociationGroup.Count > 0)
                {
                    objViewModel.AssociationGroup = new SelectList(AssociationGroup, Common.ASSOCIATION_GROUPS.ASSOCICATION_GROUP_ID, Common.ASSOCIATION_GROUPS.ASSOCIATION_GROUP);
                }
                var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                if (liClass != null && liClass.Count > 0)
                {
                    objViewModel.Class = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }


                // Fetch Students For DDL ....
                var liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByAcademicYear), sAcademicYear).DataSource.Data;
                if (liStudent != null && liStudent.Count > 0)
                {
                    objViewModel.StudentList = new SelectList(liStudent, Common.STU_PERSONAL_INFO.STUDENT_ID, Common.STU_PERSONAL_INFO.FIRST_NAME);
                }

                //fetch courses



                objViewModel.liAssociationRegistration = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(null, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.ListAssociationRegistration), sAcademicYear).DataSource.Data;
                return View(objViewModel);
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
        }
        //public JsonResult FetchStudentRegistration(string sRegistrationId)
        //{
        //    JsonResultData ObjJsonData = new JsonResultData();
        //    var liRegistration = new List<ASSOCIATION_STUDENT_REGISTRATION>();
        //    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
        //    if (!string.IsNullOrEmpty(sAcademicYear))
        //    {
        //        try
        //        {
        //            liRegistration = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(
        //                new ASSOCIATION_STUDENT_REGISTRATION() { REGISTRATION_ID = sRegistrationId },
        //                AssociationSQL.GetAssociationSQL(AssociationSQLCommands.FetchRegistrationById), sAcademicYear).DataSource.Data;
        //        }
        //        catch (Exception ex)
        //        {
        //            using (ErrorLog objHandler = new ErrorLog())
        //            {
        //                objHandler.WriteError("FetchStudentRegistration", "AssociationController", "Err MSg: " + ex.Message, "Query is empty!");
        //                objHandler.WriteError("FetchStudentRegistration", "AssociationController", ex.Message);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
        //    }
        //    return Json(liRegistration, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult FetchAssociationGroupsByClassId(string sClassId)
        //{
        //    JsonResultData ObjJsonData = new JsonResultData();
        //    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

        //    if (!string.IsNullOrEmpty(sClassId))
        //    {
        //        try
        //        {
        //            var liAssociationGroups = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(
        //                            new SUP_CLASS() { CLASS_ID = sClassId},
        //                        AssociationSQL.GetAssociationSQL(AssociationSQLCommands.LoadAssociationGroups), sAcademicYear).DataSource.Data;
        //            if (liAssociationGroups != null && liAssociationGroups.Count > 0)
        //            {
        //                foreach (var item in liAssociationGroups)
        //                {
        //                    if (item.BALANCE_QUOTA == "0" || int.Parse(item.BALANCE_QUOTA) < 0)
        //                    {
        //                        ObjJsonData.sResult += "<option value='" + item.ASSOCICATION_GROUP_ID + "' cms-setting-id='"+ item.SETTING_ID+"'    disabled> " + item.ASSOCIATION_GROUP + " </option>";
        //                    }
        //                    else
        //                    {
        //                        ObjJsonData.sResult += "<option value='" + item.ASSOCICATION_GROUP_ID + "'   cms-setting-id='" + item.SETTING_ID + "'> " + item.ASSOCIATION_GROUP + " </option>";
        //                    }

        //                }
        //            }


        //        }
        //        catch (Exception ex)
        //        {
        //            using (ErrorLog objHandler = new ErrorLog())
        //            {
        //                objHandler.WriteError("FetchAssociationGroupsByClassId", "AssociationController", "Err MSg: " + ex.Message, "Query is empty!");
        //                objHandler.WriteError("FetchAssociationGroupsByClassId", "AssociationController", ex.Message);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
        //    }
        //    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult FetchStudentAndCourseForEdit(string sClassId)
        //{
        //    ASSOCIATION_STUDENT_REGISTRATION ObjModel = new ASSOCIATION_STUDENT_REGISTRATION();
        //    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
        //    var ObjJsonData = new JsonResultData();
        //    if (!string.IsNullOrEmpty(sAcademicYear))
        //    {
        //        ObjModel.CLASS_ID = sClassId;
        //        var liCourseList = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(ObjModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.FetchAssociationGroupByRegisteredCountAndClass), sAcademicYear).DataSource.Data;
        //        if (liCourseList != null && liCourseList.Count > 0)
        //        {
        //            ObjJsonData.oResult = "<option value=0> -- Select -- </option>";
        //            foreach (var item in liCourseList)
        //            {
        //                ObjJsonData.oResult += "<option value=" + item.COURSE_ID + " cms-sClassId=" + item.CLASS_ID + " cms-SettingsId=" + item.SETTING_ID + ">" + item.ASSOCIATION_GROUP + "( Alloted " + item.ALLOTED_SEATS + " Applied " + item.SUMOFQUTOA + " )" + "</option>";
        //            }
        //        }
        //        ObjModel.SETTING_ID = liCourseList.FirstOrDefault().SETTING_ID;
        //        var liStudentList = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(ObjModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.FetchNotRegisteredStudentsForddl), sAcademicYear).DataSource.Data;
        //        if (liStudentList != null && liStudentList.Count > 0)
        //        {
        //            ObjJsonData.sResult = "<option value=0> -- Select -- </option>";
        //            foreach (var item in liStudentList)
        //            {
        //                ObjJsonData.sResult += "<option value=" + item.STUDENT_ID + "> " + item.FIRST_NAME + " </option>";
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
        //    }
        //    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult SaveAssociationRegistration(string sClassId, string sCourseId, string sStudentId, string sSettingId, string sRegistrationId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            ASSOCIATION_STUDENT_REGISTRATION ObjModel = new ASSOCIATION_STUDENT_REGISTRATION();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ObjModel.CLASS_ID = sClassId;
                ObjModel.COURSE_ID = sCourseId;
                ObjModel.SETTING_ID = sSettingId;
                ObjModel.STUDENT_ID = sStudentId;
                if (string.IsNullOrEmpty(sRegistrationId))
                {
                        string sSQL = AssociationSQL.GetAssociationSQL(AssociationSQLCommands.CheckStudentIsRegisterd);
                        var IsRegistrationExist = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(ObjModel, sSQL, sAcademicYear).DataSource.Data;
                        if (IsRegistrationExist == null)
                        {
                            var SaveRegistration = CMSHandler.SaveOrUpdate(ObjModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.InsertAssociationStudentRegistration), sAcademicYear);
                            ObjJsonData.Message = (SaveRegistration.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = (SaveRegistration.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                       {
                        ObjJsonData.Message =Common.Messages.RecordsExistAlready;
                        ObjJsonData.ErrorCode = Common.ErrorCode.AlreadyReported;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                    
                }else
                {
                    ObjModel.REGISTRATION_ID = sRegistrationId;
                    var UpdateRegistraion = CMSHandler.SaveOrUpdate(ObjModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.UpdateAssociationStudentRegistration), sAcademicYear);
                    ObjJsonData.Message = (UpdateRegistraion.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToSaveRecords;
                    ObjJsonData.ErrorCode = (UpdateRegistraion.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                if (string.IsNullOrEmpty(ObjJsonData.Message))
                {
                    ObjJsonData.Message = "No Changes are Made !";
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotModified;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteRegistration(string sRegistrationId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DeleteCourseRoot = CMSHandler.SaveOrUpdate(new ASSOCIATION_STUDENT_REGISTRATION() { REGISTRATION_ID = sRegistrationId }, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.DeleteRegistration), sAcademicYear);
                    ObjJsonData.Message = (DeleteCourseRoot.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.ErrorCode = DeleteCourseRoot.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DeleteRegistration", "AssociationController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DeleteRegistration", "AssociationController", ex.Message);
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


        #region Association Groups
        // List Certificate Course Root ...
        [UserRights("ADMIN")]
        public ActionResult ListAssociationGroups()
        {
            CertificateCoursesViewModel ObjViewModel = new CertificateCoursesViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    //fetch setting
                    var liCertificateSetting = (List<CERTIFICATE_COURSE_SETTING>)CMSHandler.FetchData<CERTIFICATE_COURSE_SETTING>(null, SQL.CertificateCourses.CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchAssociationSetting), sAcademicYear).DataSource.Data;
                    if (liCertificateSetting != null && liCertificateSetting.Count > 0)
                    {
                        ObjViewModel.SettingList = new SelectList(liCertificateSetting, Common.CERTIFICATE_COURSE_SETTING.SETTING_ID, Common.CERTIFICATE_COURSE_SETTING.SETTING_NAME);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ListCertificateCourses", "AssociationController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ListCertificateCourses", "AssociationController", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindAssociationGroupsBySettingId(string sSettingId)
        {
            CertificateCoursesViewModel objViewModel = new CertificateCoursesViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                objViewModel.liAssociation = (List<ASSOCIATION_GROUPS>)CMSHandler.FetchData<ASSOCIATION_GROUPS>(
                           new CERTIFICATE_COURSE_ROOT() { SETTING_ID = sSettingId },
                           CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.ListAssociationBySetting),
                           sAcademicYear).DataSource.Data;

            }
            return View(objViewModel);
        }
        // Fetch association Courses By Id ....
        public JsonResult FetchAssociationGroupsById(string sCertificateCourseId)
        {
            var liCourseRoot = new List<ASSOCIATION_GROUPS>();
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    liCourseRoot = (List<ASSOCIATION_GROUPS>)CMSHandler.FetchData<ASSOCIATION_GROUPS>(
                        new ASSOCIATION_GROUPS() { ASSOCICATION_GROUP_ID = sCertificateCourseId },
                        CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchAssociationGroupById), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchAssociationGroupsById", "AssociationController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchAssociationGroupsById", "AssociationController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return Json(liCourseRoot, JsonRequestBehavior.AllowGet);
        }
        // Save Certificate Courses ...
        public JsonResult SaveAssociationGroups(string sCertificateCourses)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<ASSOCIATION_GROUPS>(sCertificateCourses);

                    if (!string.IsNullOrEmpty(Result.ASSOCICATION_GROUP_ID))
                    {

                        resultArgs = CMSHandler.SaveOrUpdate(Result, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.UpdateAssociationGroups), sAcademicYear);
                        ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {
                        //check if data existed already or  not
                        var liExist = (List<ASSOCIATION_GROUPS>)CMSHandler.FetchData<ASSOCIATION_GROUPS>(
                            Result, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.IsAssociationGroupsExist), sAcademicYear).DataSource.Data;
                        if (liExist != null && liExist.Count > 0)
                        {
                            ObjJsonData.Message = Common.Messages.RecordsExistAlready;
                            ObjJsonData.ErrorCode = Common.ErrorCode.AlreadyReported;
                            return Json(ObjJsonData);
                        }



                        resultArgs = CMSHandler.SaveOrUpdate(Result, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.SaveAssociationGroups), sAcademicYear);
                        ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SaveCertificateCourses", "AssociationController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("SaveCertificateCourses", "AssociationController", ex.Message);
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
        // Active DeActive
        public JsonResult ActivateAssociationGroups(string sCourseId, string sIsActive)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ActiveAndDeActive = CMSHandler.SaveOrUpdate(new ASSOCIATION_GROUPS() { ASSOCICATION_GROUP_ID = sCourseId, IS_ACTIVE = sIsActive }, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.ActivateDeactivateCertificateCourses), sAcademicYear);
                    ObjJsonData.Message = (ActiveAndDeActive.Success) ? "Activation Done Successfully !" : "Failed To Activate !";
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ActivateCertificateClassCourse", "AssociationController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ActivateCertificateClassCourse", "AssociationController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }


        // Delete Certificate Courses By Id ....
        public JsonResult DeleteAssociationGroups(string sCertificateCourseId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DeleteCourseRoot = CMSHandler.SaveOrUpdate(new ASSOCIATION_GROUPS() { ASSOCICATION_GROUP_ID = sCertificateCourseId }, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.DeleteAssociationGroups), sAcademicYear);
                    ObjJsonData.Message = DeleteCourseRoot.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.ErrorCode = DeleteCourseRoot.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DeleteCertificateCourses", "AssociationController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DeleteCertificateCourses", "AssociationController", ex.Message);
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
        #endregion 
        // Association group regitration module created by Nelson Datre :14-07-2025
        #region Association Group Setting
        public ActionResult ListAssociationGroupSetting()
        {
            AssociationViewModel objViewModel = new AssociationViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                objViewModel.liAssociationSetting = (List<ASSOCIATION_GROUP_SETTING>)CMSHandler.FetchData<ASSOCIATION_GROUP_SETTING>(null, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.ListAssociationGroupSetting), sAcademicYear).DataSource.Data;

                var liSemester = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
                if (liSemester != null && liSemester.Count > 0)
                {
                    objViewModel.SemesterType = new SelectList(liSemester, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                }
                return View(objViewModel);
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
        }
        public JsonResult FetchAssociationGroupSettingsById(string sSettingsId)
        {
            JsonResultData sResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    CERTIFICATE_COURSE_SETTING objModel = new CERTIFICATE_COURSE_SETTING();
                    objModel.SETTING_ID = sSettingsId;
                    sResult.oResult = (List<CERTIFICATE_COURSE_SETTING>)CMSHandler.FetchData<CERTIFICATE_COURSE_SETTING>(objModel, SQL.CertificateCourses.CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchAssociationSettingById), sAcademicYear).DataSource.Data;
                }
                else
                {
                    sResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;

                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchAssociationGroupSettingsById", "CertificateCourse", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchAssociationGroupSettingsById", "CertificateCourse", ex.Message);

                }
            }
            return Json(sResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOrUpdateGroupSetting(string sJson)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = JsonConvert.DeserializeObject<ASSOCIATION_GROUP_SETTING>(sJson);
                    ObjModel.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(ObjModel.DATE_FROM);
                    ObjModel.DATE_TO = CommonMethods.ConvertdatetoyearFromat(ObjModel.DATE_TO);
                    if (!string.IsNullOrEmpty(ObjModel.ASSOCIATION_GROUP_SETTING_ID))
                    {
                        //update 
                        var resultArgs = CMSHandler.SaveOrUpdate(ObjModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.UpdateAssociationGroupSetting), sAcademicYear);
                        ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {
                        //insert 
                        var resultArgs = CMSHandler.SaveOrUpdate(ObjModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.SaveAssociationSetting), sAcademicYear);
                        ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("SaveOrUpdateGroupSetting", "AssociationController", ex.Message);
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
        public JsonResult ActiveRegistration(string sSettingsId, string sMode)
        {
            string Result = string.Empty;
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (!string.IsNullOrEmpty(sSettingsId) && !string.IsNullOrEmpty(sMode))
                    {
                        string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                        CERTIFICATE_COURSE_SETTING objModel = new CERTIFICATE_COURSE_SETTING();
                        objModel.SETTING_ID = sSettingsId;
                        objModel.IS_ACTIVE = sMode;
                        resultArgs = CMSHandler.SaveOrUpdate(objModel, SQL.CertificateCourses.CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.AssociationSettingRegistrationClose), sAcademicYear);

                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                    }
                    else
                    {
                        objResultData.Message = "Failed to close Registration !!!";
                        objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
                    }

                }
                else
                {
                    objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateRegistration", "CertificateCourse", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateRegistration", "CertificateCourse", ex.Message);
                }
            }
            return Json(objResultData);
        }
        #endregion
        #region Association Class Course
        // List Association Course Root ....
        [UserRights("ADMIN")]
        public ActionResult ListAssociationClassCourses()
        {
            CertificateCoursesViewModel ObjViewModel = new CertificateCoursesViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //ObjViewModel.liClassCourse = (List<CERTIFICATE_CLASS_COURSE>)CMSHandler.FetchData<CERTIFICATE_CLASS_COURSE>(null, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.ListCertificateClassCourses), sAcademicYear).DataSource.Data;

                    ////fetch class
                    //var ClassList = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByAcademicYear), sAcademicYear).DataSource.Data;
                    //if (ClassList != null && ClassList.Count > 0)
                    //{
                    //    ObjViewModel.Class = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    //}


                    //fetch setting
                    var liCertificateSetting = (List<CERTIFICATE_COURSE_SETTING>)CMSHandler.FetchData<CERTIFICATE_COURSE_SETTING>(null, SQL.CertificateCourses.CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchAssociationSetting), sAcademicYear).DataSource.Data;
                    if (liCertificateSetting != null && liCertificateSetting.Count > 0)
                    {
                        ObjViewModel.SettingList = new SelectList(liCertificateSetting, Common.CERTIFICATE_COURSE_SETTING.SETTING_ID, Common.CERTIFICATE_COURSE_SETTING.SETTING_NAME);
                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ListCertificateClassCourses", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ListCertificateClassCourses", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindAssociationClassCoursesBySettingId(string sSettingId)
        {
            CertificateCoursesViewModel ObjViewModel = new CertificateCoursesViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liAssociationClassCourse = (List<ASSOCIATION_GROUP_CLASS_QUOTA>)CMSHandler.FetchData<ASSOCIATION_GROUP_CLASS_QUOTA>(new CERTIFICATE_CLASS_COURSE() { SETTING_ID = sSettingId }, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.ListAssociationClassCourses), sAcademicYear).DataSource.Data;

                    //fetch class
                    var ClassList = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(new CERTIFICATE_CLASS_COURSE() { SETTING_ID = sSettingId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByAcademicYear), sAcademicYear).DataSource.Data;
                    if (ClassList != null && ClassList.Count > 0)
                    {
                        ObjViewModel.Class = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ListCertificateClassCourses", "BindCertificateClassCourseBySetting", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ListCertificateClassCourses", "BindCertificateClassCourseBySetting", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        // Fetch Association Courses By Id .... 
        public string FetchAssociationCourseBySetting(string sSettingId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            var liClassCourse = new List<CERTIFICATE_COURSE_ROOT>();
            string sOption = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    liClassCourse = (List<CERTIFICATE_COURSE_ROOT>)CMSHandler.FetchData<CERTIFICATE_COURSE_ROOT>(
                        new CERTIFICATE_COURSE_ROOT() { SETTING_ID = sSettingId },
                        CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchAssociationCourseBySettingId), sAcademicYear).DataSource.Data;
                    if (liClassCourse != null && liClassCourse.Count > 0)
                    {
                        foreach (var item in liClassCourse)
                        {
                            sOption += "<option value=" + item.COURSE_ROOT_ID + "   cms-balance-quota=" + item.BALANCE_QUOTA + ">" + item.COURSE_TITLE + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchCertificateClassCourseById", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchCertificateClassCourseById", "CertificateCoursesController", ex.Message);
                    }
                }
            }

            return sOption;
        }

        public JsonResult FetchAssociationClassCourseById(string sCertificateClassCourseId)
        {
            var liCourseRoot = new List<ASSOCIATION_GROUP_CLASS_QUOTA>();
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    liCourseRoot = (List<ASSOCIATION_GROUP_CLASS_QUOTA>)CMSHandler.FetchData<ASSOCIATION_GROUP_CLASS_QUOTA>(
                        new ASSOCIATION_GROUP_CLASS_QUOTA() { ASSOCIATION_GROUP_CLASS_QUOTA_ID = sCertificateClassCourseId },
                        CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchAssociationClassCourseById), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchCertificateClassCourseById", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchCertificateClassCourseById", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return Json(liCourseRoot, JsonRequestBehavior.AllowGet);
        }
        // Fetch Association Courses By Id ....
        public JsonResult ActivateAssociationClassCourse(string sCertificateClassCourseId, string sIsActive)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ActiveAndDeActive = CMSHandler.SaveOrUpdate(new ASSOCIATION_GROUP_CLASS_QUOTA() { ASSOCIATION_GROUP_CLASS_QUOTA_ID = sCertificateClassCourseId, IS_ACTIVE = sIsActive }, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.ActivateAssociationClassCourse), sAcademicYear);
                    ObjJsonData.Message = (ActiveAndDeActive.Success) ? "Activation Done Successfully !" : "Failed To Activate !";
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ActivateCertificateClassCourse", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ActivateCertificateClassCourse", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        // Save Association Courses ...
        public JsonResult SaveAssociationClassCourses(string SCertificateClassCourses)
        {
            JsonResultData objResultData = new JsonResultData();
            CERTIFICATE_CLASS_COURSE ObjModel = new CERTIFICATE_CLASS_COURSE();
            MySQLHandler objhandler = new MySQLHandler();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<ASSOCIATION_GROUP_CLASS_QUOTA>(SCertificateClassCourses);

                    string[] courseids = Result.COURSE_ID.Split(',');
                    foreach (var courseitem in courseids)
                    {
                        Result.COURSE_ID = courseitem;
                        if (!string.IsNullOrEmpty(Result.ASSOCIATION_GROUP_CLASS_QUOTA_ID))
                        {
                            //update

                            var InsertCourseRoot = CMSHandler.SaveOrUpdate(Result, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.UpdateAssociationClassCoursess), sAcademicYear);
                            objResultData.Message = InsertCourseRoot.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = InsertCourseRoot.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                        }
                        else
                        {

                            //ObjModel.COURSE_ID = item;
                            var liExist = (List<ASSOCIATION_GROUP_CLASS_QUOTA>)CMSHandler.FetchData<ASSOCIATION_GROUP_CLASS_QUOTA>(Result, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.IsAssociationClassCourseExist), sAcademicYear).DataSource.Data;
                            if (liExist != null && liExist.Count > 0)
                            {
                                objResultData.Message = Common.Messages.RecordsExistAlready;
                                objResultData.ErrorCode = Common.ErrorCode.AlreadyReported;
                                return Json(objResultData, JsonRequestBehavior.AllowGet);

                            }

                            var InsertCourseRoot = CMSHandler.SaveOrUpdate(Result, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.SaveAssociationClassCourses), sAcademicYear);
                            objResultData.Message = InsertCourseRoot.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = InsertCourseRoot.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                        }
                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SaveCertificateClassCourses", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("SaveCertificateClassCourses", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        // Delete Association Courses By Id ....
        public JsonResult DeleteAssociationClassCourses(string sCertificateClassCourseId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DeleteCourseRoot = CMSHandler.SaveOrUpdate(new ASSOCIATION_GROUP_CLASS_QUOTA() { ASSOCIATION_GROUP_CLASS_QUOTA_ID = sCertificateClassCourseId }, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.DeleteAssociationClassCourses), sAcademicYear);
                    ObjJsonData.Message = DeleteCourseRoot.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.ErrorCode = DeleteCourseRoot.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DeleteCertificateClassCourses", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DeleteCertificateClassCourses", "CertificateCoursesController", ex.Message);
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


        public JsonResult BindCoursesBySetting(string sSettingId)
        {
            string sOption = string.Empty;
            JsonResultData ObjJsonData = new JsonResultData();
            CERTIFICATE_COURSE_SETTINGS objModel = new CERTIFICATE_COURSE_SETTINGS();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.SETTINGS_ID = sSettingId;
                    var FetchClasss = (List<CERTIFICATE_COURSE_ROOT>)CMSHandler.FetchData<CERTIFICATE_COURSE_ROOT>(objModel, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchCertificateCourseBySetting), sAcademicYear).DataSource.Data;
                    if (FetchClasss != null && FetchClasss.Count > 0)
                    {
                        foreach (var item in FetchClasss)
                        {
                            if (!string.IsNullOrEmpty(item.CLASS_NAME))
                            {
                                ObjJsonData.oResult = item.CLASS_NAME;
                            }
                            ObjJsonData.sResult += "<option value='" + item.COURSE_ROOT_ID + "' " + item.SELECTED + " >" + item.COURSE_TITLE + "</option>";
                        }
                    }
                }
                else
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindClassListByProgrammeGroupId", "StudentController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindClassListByProgrammeGroupId", "StudentController", ex.Message);
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region AssociationGroup Student Registration
   
        public string LoadAssociationGroups()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sResult = string.Empty;
            ASSOCIATION_STUDENT_REGISTRATION objModel = new ASSOCIATION_STUDENT_REGISTRATION();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    sResult = ValidateStudent();
                    if (string.IsNullOrEmpty(sResult))
                    {
                        string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                        var liStudent = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(new STU_CLASS() { STUDENT_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByStudentId), sAcademicYear).DataSource.Data;
                        if (liStudent != null && liStudent.Count > 0)
                        {
                            objModel.STUDENT_ID = liStudent.FirstOrDefault().STUDENT_ID;
                            objModel.CLASS_ID = liStudent.FirstOrDefault().CLASS_ID;
                            var LoadAssociationGroups = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(objModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.LoadAssociationGroups), sAcademicYear).DataSource.Data;
                            if (LoadAssociationGroups != null && LoadAssociationGroups.Count > 0)
                            {
                                foreach (var item in LoadAssociationGroups)
                                {
                                    var ClassCourseQuota = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(item, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.FetchClassQuotaForCourseByCourseIdandClassId), sAcademicYear).DataSource.Data;
                                    var SumOfCourses = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(item, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.FetchSumofCourseRegisteredByCourseIdandClassId), sAcademicYear).DataSource.Data;
                                    if (ClassCourseQuota != null && SumOfCourses != null)
                                    {
                                        if (Convert.ToInt32(ClassCourseQuota.FirstOrDefault().ALLOTED_SEATS) > Convert.ToInt32(SumOfCourses.FirstOrDefault().SUMOFQUTOA))
                                        {
                                            sResult += "<div class='form-group'> <input type='radio' cms-classid='" + item.CLASS_ID + "' cms-courseid='" + item.COURSE_ID + "'  name='AssociationGroup' id='" + item.COURSE_ID + "' value='" + item.SETTING_ID + "' > <label for='" + item.COURSE_ID + "'> " + item.ASSOCIATION_GROUP + " [ <label class='label label-success'> Alloted-" + ClassCourseQuota.FirstOrDefault().ALLOTED_SEATS + "</label> -" + "<label class='label label-warning'> Applied -" + SumOfCourses.FirstOrDefault().SUMOFQUTOA + "" + "</label> ]" + "</lable></div>";
                                        }
                                        else
                                        {
                                            sResult += "<div class='form-group'> <input type='radio' disabled cms-classid='" + item.CLASS_ID + "' cms-courseid='" + item.COURSE_ID + "'  name='AssociationGroup' id='" + item.COURSE_ID + "' value='" + item.SETTING_ID + "' > <label for='" + item.COURSE_ID + "'> " + item.ASSOCIATION_GROUP + " [ <label class='label label-success'> Alloted-" + ClassCourseQuota.FirstOrDefault().ALLOTED_SEATS + "</label> -" + "<label class='label label-warning'> Applied -" + SumOfCourses.FirstOrDefault().SUMOFQUTOA + "" + "</label>]- <label class='label label-danger'>Closed</label>" + "</lable></div>";
                                            // sResult += "<div class='form-group'> <input type='radio' disabled cms-semester='" + item[Common.CP_COURSE_ROOT_2017.SEMESTER_ID].ToString() + "' cms-classid='" + item[Common.NME_CLASS_COURSE_2017.CLASS_ID].ToString() + "' cms-nmeclassid='" + item["NME_CLASS"].ToString() + "' cms-courseid='" + item[Common.NME_CLASS_COURSE_2017.COURSE_ID].ToString() + "'  name='NMECourse' id='" + item[Common.NME_CLASS_COURSE_2017.COURSE_ID].ToString() + "' value='" + item[Common.NME_CLASS_COURSE_QUOTA_2017.SETTINGS_ID] + "' > <label for='" + item[Common.NME_CLASS_COURSE_2017.COURSE_ID].ToString() + "'> " + item[Common.CP_COURSE_ROOT_2017.COURSE_TITLE].ToString() + " [ <label class='label label-success'> Alloted-" + SumOfClassCourseQuota.FirstOrDefault().ALLOTED_SEATS + "</label> -" + "<label class='label label-warning'> Applied -" + SumOfClassCourseQuota.FirstOrDefault().SUMOFQUTOA + "" + "</label>]- <label class='label label-danger'>Closed</label>" + "</lable></div>";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //sResult = "2";
                        sResult = @"<h5>Your service oriented course : <i class='text-success'> " + sResult + "</i></h5>";
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("LoadAssociationGroups", "Association", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("LoadAssociationGroups", "Association", ex.Message);
                    }


                }
            }
            else
            {
                sResult = string.Empty;
            }
            return sResult;
        }
        
        public string RegisterAssociationGroup(string sClassId, string sCourseId, string sSettingId, string sSemester)
        {
            string sResult = string.Empty;
            string sCheckCourse = string.Empty;
            ASSOCIATION_STUDENT_REGISTRATION objModel = new ASSOCIATION_STUDENT_REGISTRATION();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ResultArgs resultArgs = new ResultArgs();
                string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                try
                {
                    objModel.CLASS_ID = sClassId;
                    objModel.COURSE_ID = sCourseId;
                    objModel.SETTING_ID = sSettingId;
                    objModel.SEMESTER_ID = sSemester;
                    objModel.STUDENT_ID = sUserId;
                    objModel.ACADEMIC_YEAR = sAcademicYear;
                    sCheckCourse = checkAssociationAvailablity(sClassId, sCourseId, sSettingId);
                    if (sCheckCourse.Equals("1"))
                    {
                        sResult = ValidateStudent();
                        if (sResult != "1")
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(objModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.InsertAssociationStudentRegistration), sAcademicYear);
                            if (resultArgs.Success)
                            {
                                resultArgs.StringResult = "1";
                            }
                            else
                            {
                                resultArgs.StringResult = "0";
                            }
                        }
                    }
                    else
                    {
                        resultArgs.Success = false;
                        resultArgs.StringResult = "3";
                    }
                }
                catch (Exception ex)
                {

                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("RegisterAssociationGroup", "Association", "Err MSg : " + ex.Message, "Query is empty!");
                        objHandler.WriteError("RegisterAssociationGroup", "Association", ex.Message);
                    }
                }
                sResult = resultArgs.StringResult;
            }
            else
            {
                sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }

            return sResult;
        }
        public string checkAssociationAvailablity(string sClassId, string sCourseId, string sSettingId)
        {
            string sResult = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ASSOCIATION_STUDENT_REGISTRATION objModel = new ASSOCIATION_STUDENT_REGISTRATION();
                try
                {
                    objModel.CLASS_ID = sClassId;
                    objModel.COURSE_ID = sCourseId;
                    objModel.SETTING_ID = sSettingId;
                    var CheckAssociationAvailablity = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(objModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.CheckAssociationAvailablity), sAcademicYear).DataSource.Data;
                    sResult = CheckAssociationAvailablity.FirstOrDefault().COURSEAVAILABILITY;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("checkAssociationAvailablity", "Association", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("checkAssociationAvailablity", "Association", ex.Message);
                    }
                }
            }

            return sResult;
        }
        #endregion
        #region Association Registraion Edit
        public ActionResult AssociationRegisteredList()
        {
            AssociationViewModel objViewModel = new AssociationViewModel();

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sUserId))
            {

                var liAcademicyear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                if (liAcademicyear != null && liAcademicyear.Count > 0)
                {
                    objViewModel.AcademicYearList = new SelectList(liAcademicyear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
                var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchRegularClasses), sAcademicYear).DataSource.Data;
                if (liClass != null && liClass.Count > 0)
                {
                    objViewModel.Class = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_CODE);
                }


                return View(objViewModel);
            }
            else
                return RedirectToAction("ErrorMessage", "Error");

        }
        public ActionResult ListAssociationRegistered(string sAcyear, string sCourseId)
        {
            AssociationViewModel objViewModel = new AssociationViewModel();
            if (!string.IsNullOrEmpty(sAcyear) && !string.IsNullOrEmpty(sCourseId))
            {
                var liAcademicyear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearByAcYear), sAcyear).DataSource.Data;
                if (liAcademicyear != null && liAcademicyear.Count > 0)
                {
                    objViewModel.AcademicYearList = new SelectList(liAcademicyear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }


                var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchRegularClasses), sAcyear).DataSource.Data;
                if (liClass != null && liClass.Count > 0)
                {
                    objViewModel.Class = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_CODE);
                }

                // Fetch Students For DDL ....
                var liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByAcademicYear), sAcyear).DataSource.Data;
                if (liStudent != null && liStudent.Count > 0)
                {
                    objViewModel.StudentList = new SelectList(liStudent, Common.STU_PERSONAL_INFO.STUDENT_ID, Common.STU_PERSONAL_INFO.FIRST_NAME);
                }

                var liAssociationgroup = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(null,
                           AssociationSQL.GetAssociationSQL(AssociationSQLCommands.FetchAssociationGroupsByAcyear), sAcyear).DataSource.Data;
                if (liAssociationgroup != null && liAssociationgroup.Count > 0)
                {
                    objViewModel.AssociationGroup = new SelectList(liAssociationgroup, Common.CP_COURSE_ROOT_2017.COURSE_ID, Common.ASSOCIATION_GROUPS.ASSOCIATION_GROUP);
                }



                //fetch courses

                string sql = string.Empty;
                sql = AssociationSQL.GetAssociationSQL(AssociationSQLCommands.AssociationRegistrationList).Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.COURSE_ID, sCourseId);

                objViewModel.liAssociationRegistration = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(null, sql, sAcyear).DataSource.Data;
                return View(objViewModel);
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
        }
        public ActionResult FetchClassStudentByClassID(string sAcyear, string sClassId)
        {
            AssociationViewModel objViewModel = new AssociationViewModel();
            if (!string.IsNullOrEmpty(sAcyear) && !string.IsNullOrEmpty(sClassId))
            {
                // Fetch Students For DDL ....
                objViewModel.ACADEMIC_YEAR = sAcyear;
                objViewModel.CLASS_ID = sClassId;
                var liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(objViewModel, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByAcademicYear), sAcyear).DataSource.Data;

                if (liStudent != null && liStudent.Count > 0)
                {
                    // Create a list of objects to return as JSON
                    var studentList = liStudent.Select(s => new
                    {
                        StudentId = s.STUDENT_ID,
                        FirstName = s.FIRST_NAME
                    }).ToList();

                    return Json(new
                    {
                        Success = true,
                        StudentList = studentList
                    }, JsonRequestBehavior.AllowGet); // Use JsonRequestBehavior.AllowGet for older versions of ASP.NET
                }
            }
            return Json(new { Success = false, Message = "No students found" });
        }

        public ActionResult FetchClassStudentByClassIDforCertificateCourse(string sAcyear, string sClassId)
        {
            AssociationViewModel objViewModel = new AssociationViewModel();
            if (!string.IsNullOrEmpty(sAcyear) && !string.IsNullOrEmpty(sClassId))
            {
                // Fetch Students For DDL ....
                objViewModel.ACADEMIC_YEAR = sAcyear;
                objViewModel.CLASS_ID = sClassId;
                var liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(objViewModel, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByAcademicYear), sAcyear).DataSource.Data;

                if (liStudent != null && liStudent.Count > 0)
                {
                    // Create a list of objects to return as JSON
                    var studentList = liStudent.Select(s => new
                    {
                        StudentId = s.STUDENT_ID,
                        FirstName = s.FIRST_NAME
                    }).ToList();

                    return Json(new
                    {
                        Success = true,
                        StudentList = studentList
                    }, JsonRequestBehavior.AllowGet); // Use JsonRequestBehavior.AllowGet for older versions of ASP.NET
                }
            }
            return Json(new { Success = false, Message = "No students found" });
        }
        public JsonResult FetchStudentRegistration(string sRegistrationId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            var liRegistration = new List<ASSOCIATION_STUDENT_REGISTRATION>();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    liRegistration = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(
                        new ASSOCIATION_STUDENT_REGISTRATION() { REGISTRATION_ID = sRegistrationId },
                        AssociationSQL.GetAssociationSQL(AssociationSQLCommands.FetchRegistrationById), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchStudentRegistration", "AssociationController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchStudentRegistration", "AssociationController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(liRegistration, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchAssociationGroupsByClassId(string sClassId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sClassId))
            {
                try
                {
                    var liAssociationGroups = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(
                                    new SUP_CLASS() { CLASS_ID = sClassId },
                                AssociationSQL.GetAssociationSQL(AssociationSQLCommands.LoadAssociationGroups), sAcademicYear).DataSource.Data;
                    if (liAssociationGroups != null && liAssociationGroups.Count > 0)
                    {
                        foreach (var item in liAssociationGroups)
                        {
                            //if (item.BALANCE_QUOTA == "0" || int.Parse(item.BALANCE_QUOTA) < 0)
                            //{
                            //    ObjJsonData.sResult += "<option value='" + item.ASSOCICATION_GROUP_ID + "' cms-setting-id='"+ item.SETTING_ID+"'    disabled> " + item.ASSOCIATION_GROUP + " </option>";
                            //}
                            //else
                            //{
                            ObjJsonData.sResult += "<option value='" + item.ASSOCICATION_GROUP_ID + "'   cms-setting-id='" + item.SETTING_ID + "'> " + item.ASSOCIATION_GROUP + " </option>";
                            //}

                        }
                    }
                    else
                    {
                        //Other classes
                        var liAssociationgroup = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(
                                    new SUP_CLASS() { CLASS_ID = sClassId },
                                AssociationSQL.GetAssociationSQL(AssociationSQLCommands.LoadAllAssociationGroups), sAcademicYear).DataSource.Data;
                        if (liAssociationgroup != null && liAssociationgroup.Count > 0)
                        {
                            foreach (var item in liAssociationgroup)
                            {

                                ObjJsonData.sResult += "<option value='" + item.ASSOCICATION_GROUP_ID + "'   cms-setting-id='" + item.SETTING_ID + "'> " + item.ASSOCIATION_GROUP + " </option>";


                            }
                        }
                    }



                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchAssociationGroupsByClassId", "AssociationController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchAssociationGroupsByClassId", "AssociationController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchStudentAndCourseForEdit(string sClassId)
        {
            ASSOCIATION_STUDENT_REGISTRATION ObjModel = new ASSOCIATION_STUDENT_REGISTRATION();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ObjModel.CLASS_ID = sClassId;
                var liCourseList = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(ObjModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.FetchAssociationGroupByRegisteredCountAndClass), sAcademicYear).DataSource.Data;
                if (liCourseList != null && liCourseList.Count > 0)
                {
                    ObjJsonData.oResult = "<option value=0> -- Select -- </option>";
                    foreach (var item in liCourseList)
                    {
                        ObjJsonData.oResult += "<option value=" + item.COURSE_ID + " cms-sClassId=" + item.CLASS_ID + " cms-SettingsId=" + item.SETTING_ID + ">" + item.ASSOCIATION_GROUP + "( Alloted " + item.ALLOTED_SEATS + " Applied " + item.SUMOFQUTOA + " )" + "</option>";
                    }
                }
                ObjModel.SETTING_ID = liCourseList.FirstOrDefault().SETTING_ID;
                var liStudentList = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(ObjModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.FetchNotRegisteredStudentsForddl), sAcademicYear).DataSource.Data;
                if (liStudentList != null && liStudentList.Count > 0)
                {
                    ObjJsonData.sResult = "<option value=0> -- Select -- </option>";
                    foreach (var item in liStudentList)
                    {
                        ObjJsonData.sResult += "<option value=" + item.STUDENT_ID + "> " + item.FIRST_NAME + " </option>";
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult SaveAssociationRegistration(string sAcyear, string sClassId, string sCourseId, string sStudentId, string sRegistrationId)
        //{
        //    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
        //    var ObjJsonData = new JsonResultData();
        //    ASSOCIATION_STUDENT_REGISTRATION ObjModel = new ASSOCIATION_STUDENT_REGISTRATION();
        //    if (!string.IsNullOrEmpty(sAcademicYear))
        //    {
        //        ObjModel.ACADEMIC_YEAR = sAcyear;
        //        ObjModel.CLASS_ID = sClassId;
        //        ObjModel.COURSE_ID = sCourseId;
        //        ObjModel.STUDENT_ID = sStudentId;
        //        var sSettingId = (List<ASSOCIATION_GROUP_SETTING>)CMSHandler.FetchData<ASSOCIATION_GROUP_SETTING>(ObjModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.FetchAssociationSettingByAcYear), sAcyear).DataSource.Data;
        //        ObjModel.SETTING_ID = sSettingId.FirstOrDefault().ASSOCIATION_GROUP_SETTING_ID;
        //        if (string.IsNullOrEmpty(sRegistrationId))
        //        {
        //            string sSQL = AssociationSQL.GetAssociationSQL(AssociationSQLCommands.CheckStudentIsRegisterd);
        //            var IsRegistrationExist = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(ObjModel, sSQL, sAcademicYear).DataSource.Data;
        //            if (IsRegistrationExist == null)
        //            {
        //                var SaveRegistration = CMSHandler.SaveOrUpdate(ObjModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.InsertAssociationStudentRegistration), sAcademicYear);
        //                ObjJsonData.Message = (SaveRegistration.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
        //                ObjJsonData.ErrorCode = (SaveRegistration.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
        //            }
        //            else
        //            {
        //                ObjJsonData.Message = Common.Messages.RecordsExistAlready;
        //                ObjJsonData.ErrorCode = Common.ErrorCode.AlreadyReported;
        //                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        //            }

        //        }
        //        else
        //        {
        //            ObjModel.REGISTRATION_ID = sRegistrationId;
        //            var UpdateRegistraion = CMSHandler.SaveOrUpdate(ObjModel, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.UpdateAssociationStudentRegistration), sAcademicYear);
        //            ObjJsonData.Message = (UpdateRegistraion.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToSaveRecords;
        //            ObjJsonData.ErrorCode = (UpdateRegistraion.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
        //        }
        //        if (string.IsNullOrEmpty(ObjJsonData.Message))
        //        {
        //            ObjJsonData.Message = "No Changes are Made !";
        //            ObjJsonData.ErrorCode = Common.ErrorCode.NotModified;
        //        }
        //    }
        //    else
        //    {
        //        ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
        //        ObjJsonData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
        //    }
        //    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult DeleteRegistration(string sRegistrationId)
        //{
        //    JsonResultData ObjJsonData = new JsonResultData();
        //    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
        //    if (!string.IsNullOrEmpty(sAcademicYear))
        //    {
        //        try
        //        {
        //            var DeleteCourseRoot = CMSHandler.SaveOrUpdate(new ASSOCIATION_STUDENT_REGISTRATION() { REGISTRATION_ID = sRegistrationId }, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.DeleteRegistration), sAcademicYear);
        //            ObjJsonData.Message = (DeleteCourseRoot.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
        //            ObjJsonData.ErrorCode = DeleteCourseRoot.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
        //        }
        //        catch (Exception ex)
        //        {
        //            using (ErrorLog objHandler = new ErrorLog())
        //            {
        //                objHandler.WriteError("DeleteRegistration", "AssociationController", "Err MSg: " + ex.Message, "Query is empty!");
        //                objHandler.WriteError("DeleteRegistration", "AssociationController", ex.Message);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
        //    }
        //    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        //}

        #endregion
        #region Registration View
        public ActionResult ListAssociationRegisteredForStaffIncharge()
        {
            AssociationViewModel objViewModel = new AssociationViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;



            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sUserId))
            {

                objViewModel.liAssociationRegistration = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(new STF_PERSONAL_INFO() { STAFF_ID = sUserId }, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.ListAssociationRegistrationByStaff), sAcademicYear).DataSource.Data;
                return View(objViewModel);
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
        }
        //Registration By Academic year and course wise
        public ActionResult ListAssociationRegisteredForStaff()
        {
            AssociationViewModel objViewModel = new AssociationViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sUserId))
            {

                var liAcademicyear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                if (liAcademicyear != null && liAcademicyear.Count > 0)
                {
                    objViewModel.AcademicYearList = new SelectList(liAcademicyear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }


                return View(objViewModel);
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
        }
        public JsonResult FetchAssocationgropus(string sAcyear)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sAcyear))
            {
                try
                {

                    //Other classes
                    var liAssociationgroup = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(null,
                            AssociationSQL.GetAssociationSQL(AssociationSQLCommands.FetchAssociationGroupsByAcyear), sAcyear).DataSource.Data;
                    if (liAssociationgroup != null && liAssociationgroup.Count > 0)
                    {
                        foreach (var item in liAssociationgroup)
                        {

                            ObjJsonData.sResult += "<option value='" + item.ASSOCICATION_GROUP_ID + "'   cms-setting-id='" + item.SETTING_ID + "'> " + item.ASSOCIATION_GROUP + " </option>";


                        }

                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchAssociationGroupsByClassId", "AssociationController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchAssociationGroupsByClassId", "AssociationController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchClass(string sAcyear)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sAcyear))
            {
                try
                {

                    //Other classes
                    var liAssociationgroup = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(null,
                            AssociationSQL.GetAssociationSQL(AssociationSQLCommands.FetchRegularClasses), sAcyear).DataSource.Data;
                    if (liAssociationgroup != null && liAssociationgroup.Count > 0)
                    {
                        foreach (var item in liAssociationgroup)
                        {

                            ObjJsonData.sResult += "<option value='" + item.CLASS_ID + item.CLASS_NAME + " </option>";


                        }

                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchAssociationGroupsByClassId", "AssociationController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchAssociationGroupsByClassId", "AssociationController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindAssocationRegisteredList(string sAcyear, string sCourseId)
        {
            AssociationViewModel objViewModel = new AssociationViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                objViewModel.liAssociationRegistration = (List<ASSOCIATION_STUDENT_REGISTRATION>)CMSHandler.FetchData<ASSOCIATION_STUDENT_REGISTRATION>(new ASSOCIATION_STUDENT_REGISTRATION() { ACADEMIC_YEAR = sAcyear, COURSE_ID = sCourseId }, AssociationSQL.GetAssociationSQL(AssociationSQLCommands.ListAssociationRegistrationByCourseId), sAcyear).DataSource.Data;
                return View(objViewModel);
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
        }
        #endregion
    }
}