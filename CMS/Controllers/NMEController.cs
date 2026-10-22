using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CMS.ViewModel.ViewModel;
using CMS.Utility;
using System.Data;
using CMS.ViewModel.Model;
using CMS.Models;
using CMS.DAO;
using Newtonsoft.Json;
using CMS.SQL;
using CMS.SQL.SupportData;

namespace CMS.Controllers
{
    public class NMEController : Controller
    {

        #region NME Registration By Student
        // GET: NME
        // [UserRights("STUDENT")]

        // NME Registration ...
        public ActionResult NMERegistration()
        {
            return View();
        }

        public string CheckNMEAvailability(string sClassId, string sCourseId, string sSettingId)
        {
            string sResult = string.Empty;
            try
            {
                DataValue dv = new DataValue();
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                ResultArgs resultArgs = new ResultArgs();
                dv.Clear();
                dv.Add(Common.NME_STUDENT_REGISTRATION_2017.CLASS_ID, sClassId, EnumCommand.DataType.String);
                dv.Add(Common.NME_STUDENT_REGISTRATION_2017.COURSE_ID, sCourseId, EnumCommand.DataType.String);
                dv.Add(Common.NME_COURSE_REGISTRATION_2017.SETTINGS_ID, sSettingId, EnumCommand.DataType.String);
                NMEViewModel objNME = new NMEViewModel();
                resultArgs = objNME.CheckNMEAvailability(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                sResult = resultArgs.StringResult;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CheckNMEAvailability", "NMEController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CheckNMEAvailability", "NMEController", ex.Message);
                }
            }
            return sResult;
        }
        // Save Student Choosen NME Course ...
        public string RegisterNMECourse(string sClassId, string sCourseId, string sSettingId, string semester, string sSClassId)
        {
            string sResult = string.Empty;
            string sCheckCourse = string.Empty;
            DataValue dv = new DataValue();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ResultArgs resultArgs = new ResultArgs();
                try
                {
                    dv.Clear();
                    dv.Add(Common.NME_STUDENT_REGISTRATION_2017.CLASS_ID, sClassId, EnumCommand.DataType.String);
                    dv.Add(Common.NME_STUDENT_REGISTRATION_2017.COURSE_ID, sCourseId, EnumCommand.DataType.String);
                    sCheckCourse = CheckNMEAvailability(sClassId, sCourseId, sSettingId);
                    dv.Add(Common.NME_SETTINGS.SETTINGS_ID, sSettingId, EnumCommand.DataType.String);
                    dv.Add(Common.NME_STUDENT_REGISTRATION_2017.STUDENT_ID, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);
                    dv.Add(Common.NME_STUDENT_REGISTRATION_2017.SEMESTER, semester, EnumCommand.DataType.String);
                    dv.Add(Common.NME_STUDENT_REGISTRATION_2017.S_CLASS_ID, sSClassId, EnumCommand.DataType.String);
                    NMEViewModel objNME = new NMEViewModel();

                    if (sCheckCourse.Equals("1"))
                    {
                        sResult = ValidateStudent();
                        if (sResult != "1")
                        {
                            resultArgs = objNME.RegisterNMECourse(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
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
                        objHandler.WriteError("RegisterNMECourse", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                        objHandler.WriteError("RegisterNMECourse", "NMEController", ex.Message);
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
        // Load NME Course For Student ...
        public string LoadNMECourses()
        {
            ResultArgs resultArgs = new ResultArgs();
            string sResult = string.Empty;
            try
            {
                sResult = ValidateStudent();
                if (string.IsNullOrEmpty(sResult))
                {
                    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                    {
                        DataValue dv = new DataValue();
                        DataTable dt = new DataTable();
                        string sClassId = string.Empty;

                        string sShiftId = string.Empty;
                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                        string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                        var liStudent = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(new STU_CLASS() { STUDENT_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByStudentId), sAcademicYear).DataSource.Data;
                        if (liStudent != null && liStudent.Count > 0)
                        {
                            sClassId = liStudent.FirstOrDefault().CLASS_ID;
                            sShiftId = liStudent.FirstOrDefault().SHIFT_ID;
                        }
                        dv.Clear();
                        dv.Add(Common.NME_SETTINGS.CLASS_ID, sClassId, EnumCommand.DataType.String);
                        dv.Add(Common.STU_PERSONAL_INFO.SHIFT_ID, sShiftId, EnumCommand.DataType.String);
                        NMEViewModel objNME = new NMEViewModel();
                        resultArgs = objNME.LoadNMECourses(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                        if (resultArgs.Success)
                        {
                            dt = resultArgs.DataSource.Table;
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                sResult = string.Empty;
                                // CLASS_ID, NC.COURSE_ID, NC.SETTINGS_ID, CR.COURSE_TITLE
                                foreach (DataRow item in dt.Rows)
                                {

                                    var SumOfClassCourseQuota = (List<NMERegisteredStudent>)CMSHandler.FetchData<NMERegisteredStudent>(new NMERegisteredStudent() { SHIFT_ID = sShiftId, CLASS_ID = item[Common.NME_CLASS_COURSE_2017.CLASS_ID].ToString(), COURSE_ID = item[Common.NME_CLASS_COURSE_2017.COURSE_ID].ToString() }, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchSumOfNMEClassQuotaByClassIdAndCourseId), sAcademicYear).DataSource.Data;
                                    if (SumOfClassCourseQuota != null)
                                    {
                                        if (Convert.ToInt32(SumOfClassCourseQuota.FirstOrDefault().ALLOTED_SEATS) > Convert.ToInt32(SumOfClassCourseQuota.FirstOrDefault().SUMOFQUTOA))
                                        {
                                            sResult += "<div class='form-group'> <input type='radio' cms-semester='" + item[Common.CP_COURSE_ROOT_2017.SEMESTER_ID].ToString() + "' cms-classid='" + item[Common.NME_CLASS_COURSE_2017.CLASS_ID].ToString() + "' cms-nmeclassid='" + item["NME_CLASS"].ToString() + "' cms-courseid='" + item[Common.NME_CLASS_COURSE_2017.COURSE_ID].ToString() + "'  name='NMECourse' id='" + item[Common.NME_CLASS_COURSE_2017.COURSE_ID].ToString() + "' value='" + item[Common.NME_CLASS_COURSE_QUOTA_2017.SETTINGS_ID] + "' > <label for='" + item[Common.NME_CLASS_COURSE_2017.COURSE_ID].ToString() + "'> " + item[Common.CP_COURSE_ROOT_2017.COURSE_TITLE].ToString() + " - " + item[Common.CP_COURSE_ROOT_2017.COURSE_CODE].ToString() + " [ <label class='label label-success'> Alloted-" + SumOfClassCourseQuota.FirstOrDefault().ALLOTED_SEATS + "</label> -" + "<label class='label label-warning'> Applied -" + SumOfClassCourseQuota.FirstOrDefault().SUMOFQUTOA + "" + "</label> ]" + "</lable></div>";
                                        }
                                        else
                                        {
                                            sResult += "<div class='form-group'> <input type='radio' disabled cms-semester='" + item[Common.CP_COURSE_ROOT_2017.SEMESTER_ID].ToString() + "' cms-classid='" + item[Common.NME_CLASS_COURSE_2017.CLASS_ID].ToString() + "' cms-nmeclassid='" + item["NME_CLASS"].ToString() + "' cms-courseid='" + item[Common.NME_CLASS_COURSE_2017.COURSE_ID].ToString() + "'  name='NMECourse' id='" + item[Common.NME_CLASS_COURSE_2017.COURSE_ID].ToString() + "' value='" + item[Common.NME_CLASS_COURSE_QUOTA_2017.SETTINGS_ID] + "' > <label for='" + item[Common.NME_CLASS_COURSE_2017.COURSE_ID].ToString() + "'> " + item[Common.CP_COURSE_ROOT_2017.COURSE_TITLE].ToString() + " [ <label class='label label-success'> Alloted-" + SumOfClassCourseQuota.FirstOrDefault().ALLOTED_SEATS + "</label> -" + "<label class='label label-warning'> Applied -" + SumOfClassCourseQuota.FirstOrDefault().SUMOFQUTOA + "" + "</label>]- <label class='label label-danger'>Closed</label>" + "</lable></div>";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                sResult = string.Empty;
                            }

                        }
                    }
                }
                else
                {
                    //sResult = "2";
                    sResult = @"<h5>Registered NME Course : <span class='text-primary'> " + sResult + "</span></h5><script>$('#btnSubmit').addClass('hidden')</script>";
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("LoadNMECourses", "NMEController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("LoadNMECourses", "NMEController", ex.Message);
                }


            }
            return sResult;
        }

        // Validate The Student Registered for this semester ...
        public string ValidateStudent()
        {
            string sResult = string.Empty;

            string sCheckCourse = string.Empty;
            DataValue dv = new DataValue();
            ResultArgs resultArgs = new ResultArgs();

            try
            {
                dv.Clear();
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                dv.Add(Common.NME_STUDENT_REGISTRATION_2017.STUDENT_ID, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);
                NMEViewModel objNME = new NMEViewModel();
                resultArgs = objNME.ValidateStudents(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                sResult = resultArgs.StringResult;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ValidateStudent", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ValidateStudent", "NMEController", ex.Message);
                }
            }

            return sResult;

        }
        #endregion

        #region NME Course Registration 
        // List NME Registered Student List ...
        public async Task<ActionResult> NMECourseRegistrationList()
        {
            NMEViewModel objViewModel = new NMEViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
                    var NMECourseRegistration = new NMECourseRegistration();
                    string sSQL = SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMECourseRegistration).Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                    var ListNMECourseRegistration = await Task.Run(() => (List<NMECourseRegistration>)CMSHandler.FetchData<NMECourseRegistration>(null,
                          sSQL, sAcademicYear).DataSource.Data);
                    objViewModel.lstNMECourseRegistration = ListNMECourseRegistration;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("NMECourseRegistrationList", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("NMECourseRegistrationList", "NMEController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        // Fetch NME Courses ...
        public string NMECourseRegistration()
        {
            NMEViewModel objViewModel = new NMEViewModel();
            string sOption = string.Empty;
            string objJsonData = string.Empty;
            var Course = new List<cp_Course_Root_2017>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    Course = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOnlyNMECoursesList), sAcademicYear).DataSource.Data;
                    if (Course != null && Course.Count > 0)
                    {
                        foreach (var item in Course)
                        {
                            sOption += "<option value='" + item.COURSE_ROOT_ID + "' " + item.SELECTED + ">" + item.COURSE_TITLE + "</option>";
                        }
                    }
                    var JsonData = new SaveNMECourseRegistration() { COURSE_ID = sOption };
                    objJsonData = JsonConvert.SerializeObject(JsonData);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("NMECourseRegistration", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("NMECourseRegistration", "NMEController", ex.Message);
                }
            }
            return objJsonData;
        }
        // Get NME Class...
        public JsonResult GetClassId(string sCourseId)
        {
            string sOption = string.Empty;
            string sCourseTypeId = string.Empty;
            string sCourseType = string.Empty;
            NMEViewModel objViewModel = new NMEViewModel();
            NMECourseRegistration objModel = new NMECourseRegistration();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objModel.COURSE_ROOT_ID = sCourseId;
                    var NMECourseType = new List<NMECourseRegistration>();
                    NMECourseType = (List<NMECourseRegistration>)CMSHandler.FetchData<NMECourseRegistration>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.GetNMECourseTypeByCourseId), sAcademicYear).DataSource.Data;
                    sCourseTypeId = NMECourseType[0].COURSE_TYPE_ID;
                    sCourseType = NMECourseType[0].COURSE_TYPE;
                    FetchNMECourseRegistration objFetchModel = new FetchNMECourseRegistration() { COURSE_TYPE_ID = sCourseTypeId, COURSE_TYPE = sCourseType };
                    //    NMEClass = (List<NMEClassCourse>)CMSHandler.FetchData<NMEClassCourse>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.GetSelectedNMEClassId), sAcademicYear).DataSource.Data;
                    return Json(objFetchModel, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    sOption = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    return Json(sOption, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("GetClassId", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("GetClassId", "NMEController", ex.Message);
                    return Json(objViewModel, JsonRequestBehavior.AllowGet);
                }
            }
        }
        // Save NME Course Registration ...
        public string SaveNMECourseRegistration(string sCourseId)
        {
            var sResult = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var objModel = new SaveNMECourseRegistration();
                    if (sCourseId != null)
                    {
                        var NMECourses = (List<NMECourseRegistration>)CMSHandler.FetchData<NMECourseRegistration>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMECourseRegistrationExisting), sAcademicYear).DataSource.Data;
                        string[] sArray = sCourseId.Split(',');
                        for (int i = 0; i < sArray.Length; i++)
                        {
                            objModel.COURSE_ID = sArray[i];
                            objModel.COURSE_ROOT_ID = sArray[i];
                            var FetchNMEClass = (List<NMECourseRegistration>)CMSHandler.FetchData<NMECourseRegistration>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMEClassByCourseId), sAcademicYear).DataSource.Data;
                            if (FetchNMEClass != null && FetchNMEClass.Count > 0)
                            {
                                var FetchNMECoruseRegistration = (List<NMECourseRegistration>)CMSHandler.FetchData<NMECourseRegistration>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.IsNMECourseRegistrationExisting), sAcademicYear).DataSource.Data;
                                if (FetchNMECoruseRegistration == null)
                                {
                                    var FetchCoursetype = (List<NMECourseRegistration>)CMSHandler.FetchData<NMECourseRegistration>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchCourseTypeByCourseId), sAcademicYear).DataSource.Data;
                                    if (FetchCoursetype != null && FetchCoursetype.Count > 0)
                                        objModel.COURSE_TYPE = FetchCoursetype.FirstOrDefault().COURSE_TYPE;
                                    var SaveNMECourseRegistration = CMSHandler.SaveOrUpdate(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.SaveNMECourseRegistration), sAcademicYear);
                                    if (SaveNMECourseRegistration.Success)
                                        sResult = Common.Messages.RecordsSavedSuccessfully;
                                    else
                                        sResult = Common.Messages.FailedToSaveRecords;
                                }
                            }
                            else
                                sResult = "Please Add This Course In Classes As Optional Class...!!!";
                        }
                        string sSQL = string.Empty;
                        sSQL = SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.DeleteNMECourseRegistrationByCourseId).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.COURSE_ID, sCourseId);
                        var ResultArg = CMSHandler.SaveOrUpdate(objModel, sSQL, sAcademicYear);
                        sResult = (ResultArg.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;

                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveNMECourseRegistration", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveNMECourseRegistration", "NMEController", ex.Message);
                }
            }
            return sResult;
        }
        // Update NME Course Registration ...
        public JsonResult UpdateNMECourseRegistration(string NMECourseRegistration)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            var sResult = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (NMECourseRegistration != null)
                    {
                        var Result = JsonConvert.DeserializeObject<SaveNMECourseRegistration>(NMECourseRegistration);
                        Result.REGISTRATION_ID = (Session[Common.SESSION_VARIABLES.REGISTRATION_ID] != null) ? Session[Common.SESSION_VARIABLES.REGISTRATION_ID].ToString() : "";
                        var FetchNMEClass = (List<NMECourseRegistration>)CMSHandler.FetchData<NMECourseRegistration>(Result, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMEClassByCourseId), sAcademicYear).DataSource.Data;
                        if (FetchNMEClass != null && FetchNMEClass.Count > 0)
                        {
                            var FetchNMECoruseRegistration = (List<NMECourseRegistration>)CMSHandler.FetchData<NMECourseRegistration>(Result, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.IsNMECourseRegistrationExisting), sAcademicYear).DataSource.Data;
                            if (FetchNMECoruseRegistration != null)
                                sResult = "This Course Is Existing Already...!";
                            else
                            {
                                ResultArgs ResultArg = CMSHandler.SaveOrUpdate(Result, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.UpdateNMECourseRegistration), sAcademicYear);
                                if (ResultArg.Success)
                                    sResult = Common.Messages.RecordsSavedSuccessfully;
                                else
                                    sResult = Common.Messages.FailedToSaveRecords;
                            }
                        }
                        else
                            sResult = "Please Add This Course In Classes As Optional Class...!!!";
                    }
                }
                else
                    sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateNMECourseRegistration", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateNMECourseRegistration", "NMEController", ex.Message);
                }
                sResult = Common.ErrorMessage.InternalServerError;
            }
            return Json(sResult);
        }
        // Delete NME Course Registration ...
        public JsonResult DeleteNMECourseRegistration(int id)
        {

            string sResult = string.Empty;
            DeleteNMECourseRegistration objModel = new DeleteNMECourseRegistration();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.REGISTRATION_ID = Convert.ToString(id);

                    ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.DeleteNMECourseRegistration), sAcademicYear);

                    sResult = (ResultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
                else
                    sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateNMECourseRegistration", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateNMECourseRegistration", "NMEController", ex.Message);
                }
                sResult = Common.ErrorMessage.InternalServerError;
            }
            return Json(sResult);
        }
        // Edit NME Course Registation ...
        public async Task<ActionResult> EditNMECoruseRegistration(int id)
        {
            NMEViewModel objViewModel = new NMEViewModel();
            var Course = new List<cp_Course_Root_2017>();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            Course = await Task.Run(() => (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOnlyNMECoursesList), sAcademicYear).DataSource.Data);
            objViewModel.NMECourseList = new SelectList(Course, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_TITLE);
            return View(objViewModel);
        }
        public JsonResult FetchNMECourseRegistrationById(string sRegistrationId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            Session[Common.SESSION_VARIABLES.REGISTRATION_ID] = sRegistrationId;
            SaveNMECourseRegistration objModel = new SaveNMECourseRegistration();
            objModel.REGISTRATION_ID = sRegistrationId;
            var ListNMECourseRegistration = (List<SaveNMECourseRegistration>)CMSHandler.FetchData<SaveNMECourseRegistration>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMECourseRegistrationById), sAcademicYear).DataSource.Data;
            foreach (var item in ListNMECourseRegistration)
            {
                objModel.COURSE_ID = item.COURSE_ID;
                objModel.COURSE_TYPE = item.COURSE_TYPE;
                objModel.COURSE_TYPE_ID = item.COURSE_TYPE_ID;
            }
            return Json(objModel);
        }
        #endregion 
        public ActionResult NMEStudentList()
        {
            NMEViewModel objViewModel = new NMEViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            try
            {
                DataTable dtNMEclassList = new DataTable();
                List<NMEClassList> iClassList = new List<NMEClassList>();
                dtNMEclassList = objViewModel.FetchNMEClassList((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                iClassList = (from DataRow dr in dtNMEclassList.Rows select new NMEClassList() { CLASS_ID = dr["CLASS_ID"].ToString(), CLASS_NAME = dr["CLASS_NAME"].ToString() }).ToList();
                objViewModel.NMEClassList = new SelectList(iClassList, "CLASS_ID", "CLASS_NAME");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ValidateStudent", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ValidateStudent", "NMEController", ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindNMEStudentList(string sClassID)
        {
            DataValue dv = new DataValue();
            NMEViewModel objViewModel = new NMEViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            try
            {
                DataTable dtStudentList = new DataTable();
                List<NMEClassWiseStudent> iClassList = new List<NMEClassWiseStudent>();
                dv.Add(Common.CP_CLASSES_2017.CLASS_ID, sClassID, EnumCommand.DataType.String);
                dtStudentList = objViewModel.FetchNMEStudentsByClassId(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                iClassList = (from DataRow dr in dtStudentList.Rows
                              select new NMEClassWiseStudent()
                              {
                                  ADMISSION_NO = dr[Common.STU_PERSONAL_INFO.ADMISSION_NO].ToString(),
                                  CLASS_NAME = dr["CLASS_NAME"].ToString(),
                                  COURSE_TITLE = dr[Common.CP_COURSE_ROOT_2017.COURSE_TITLE].ToString(),
                                  FIRST_NAME = dr[Common.STU_PERSONAL_INFO.FIRST_NAME].ToString(),
                                  REGISTER_NO = dr[Common.STU_PERSONAL_INFO.REGISTER_NO].ToString()
                              }).ToList();
                objViewModel.NMEClassWiseStudents = iClassList;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindNMEStudentList", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindNMEStudentList", "NMEController", ex.Message);
                }

            }
            return View(objViewModel);
        }
        #region NME Settings
        public async Task<ActionResult> NMESettingsList()
        {
            NMEViewModel objViewModel = new NMEViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                var NMESettings = new NMESettings();
                var ListNMESettings = await Task.Run(() => (List<NMESettings>)CMSHandler.FetchData<NMESettings>(null, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMESettingsList), sAcademicYear).DataSource.Data);
                objViewModel.lstNMESettings = ListNMESettings;
                return View(objViewModel);
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
        }
        public async Task<ActionResult> NMESettings()
        {
            NMEViewModel objViewModel = new NMEViewModel();
            //var Class = new List<cp_Classes_2017>();
            var Semester = new List<SupSemester>();
            var Shift = new List<Sup_Shift>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    //Class = await Task.Run(() => (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data);
                    Semester = await Task.Run(() => (List<SupSemester>)CMSHandler.FetchData<SupSemester>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data);
                    Shift = await Task.Run(() => (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data);
                    //if (Class != null && Class.Count > 0)
                    //    objViewModel.ClassList = new SelectList(Class, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    if (Semester != null && Semester.Count > 0)
                        objViewModel.SemesterList = new SelectList(Semester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    if (Shift != null && Shift.Count > 0)
                        objViewModel.ShiftList = new SelectList(Shift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("NMESettings", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("NMESettings", "NMEController", ex.Message);
                }

            }
            return View(objViewModel);
        }
        public string GetClassIdByShiftId(string sShiftId)
        {
            string sOption = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    NMEViewModel objViewModel = new NMEViewModel();
                    SaveNMESettings objModel = new SaveNMESettings();
                    objModel.SHIFT = sShiftId;
                    var Class = new List<cp_Classes_2017>();
                    Class = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(objModel, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOrdinaryClassesByShiftId), sAcademicYear).DataSource.Data;
                    if (Class != null && Class.Count > 0)
                    {
                        foreach (var List in Class)
                            sOption += "<option value='" + List.CLASS_ID + "' >" + List.CLASS_NAME + "</option>";
                    }
                }
                else
                    sOption = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("GetClassIdByShiftId", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("GetClassIdByShiftId", "NMEController", ex.Message);
                }
            }
            return sOption;
        }
        public string SaveNMESetting(string NMESettings)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            var sResult = string.Empty;
            try
            {
                if (NMESettings != null)
                {
                    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                    {
                        var Result = JsonConvert.DeserializeObject<SaveNMESettings>(NMESettings);
                        Result.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(Result.DATE_FROM);
                        Result.DATE_TO = CommonMethods.ConvertdatetoyearFromat(Result.DATE_TO);
                        Result.IS_ALLOWED = "1";
                        var FetchExistingSetting = (List<NMESettings>)CMSHandler.FetchData<NMESettings>(Result, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.IsNMESettingExisting), sAcademicYear).DataSource.Data;
                        if (FetchExistingSetting != null)
                        {
                            sResult = "This Setting Is Existing Already...!";
                        }
                        else
                        {
                            ResultArgs ResultArg = CMSHandler.SaveOrUpdate(Result, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.SaveNMESettings), sAcademicYear);
                            if (ResultArg.Success)
                            {
                                sResult = Common.Messages.RecordsSavedSuccessfully;
                            }
                            else
                            {
                                sResult = Common.Messages.FailedToSaveRecords;
                            }
                        }
                    }
                    else
                        sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveNMESetting", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveNMESetting", "NMEController", ex.Message);
                }
            }
            return sResult;
        }
        public async Task<ActionResult> EditNMESettings(int id)
        {
            NMEViewModel objViewModel = new NMEViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    Session[Common.SESSION_VARIABLES.SETTINGS_ID] = id;
                    var Class = new List<cp_Classes_2017>();
                    var Semester = new List<SupSemester>();
                    var Shift = new List<Sup_Shift>();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    Class = await Task.Run(() => (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data);
                    Semester = await Task.Run(() => (List<SupSemester>)CMSHandler.FetchData<SupSemester>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data);
                    Shift = await Task.Run(() => (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data);
                    if (Class != null && Class.Count > 0)
                        objViewModel.ClassList = new SelectList(Class, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    if (Semester != null && Semester.Count > 0)
                        objViewModel.SemesterList = new SelectList(Semester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    if (Shift != null && Shift.Count > 0)
                        objViewModel.ShiftList = new SelectList(Shift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EditNMESettings", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EditNMESettings", "NMEController", ex.Message);
                }

            }
            return View(objViewModel);
        }
        public JsonResult FetchSettingsById(string sSettingsId)
        {
            JsonResultData sResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    SaveNMESettings objModel = new SaveNMESettings();
                    objModel.SETTINGS_ID = sSettingsId;
                    var ListNMESettings = (List<SaveNMESettings>)CMSHandler.FetchData<SaveNMESettings>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMESettingsById), sAcademicYear).DataSource.Data;
                    return Json(ListNMESettings);
                }
                else
                {
                    sResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    return Json(sResult);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchSettingsById", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchSettingsById", "NMEController", ex.Message);
                    return Json(sResult);
                }
            }
        }
        public string UpdateRegistration(string sSettingsId)
        {
            string Result = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (sSettingsId != null)
                    {
                        string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                        NMESettings objModel = new NMESettings();
                        objModel.SETTINGS_ID = sSettingsId;
                        ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.NMESettingsRegistrationClose), sAcademicYear);
                        if (ResultArg.Success)
                            Result = "Registration has been closed successfully";
                        else
                            Result = "Failed to close Registration !!!";
                    }
                    else
                        Result = "Failed to close Registration !!!";
                }
                else
                    Result = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateRegistration", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateRegistration", "NMEController", ex.Message);
                }
            }
            return Result;
        }
        public JsonResult UpdateNMESettings(string NMESettings)
        {
            var sResult = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (NMESettings != null)
                    {
                        var Result = JsonConvert.DeserializeObject<SaveNMESettings>(NMESettings);
                        Result.SETTINGS_ID = (Session[Common.SESSION_VARIABLES.SETTINGS_ID] != null) ? Session[Common.SESSION_VARIABLES.SETTINGS_ID].ToString() : "";
                        Result.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(Result.DATE_FROM);
                        Result.DATE_TO = CommonMethods.ConvertdatetoyearFromat(Result.DATE_TO);
                        //var FetchExistingSetting = (List<NMESettings>)CMSHandler.FetchData<NMESettings>(Result, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.IsNMESettingExisting), sAcademicYear).DataSource.Data;
                        //if (FetchExistingSetting != null)
                        //    sResult = "This Setting Is Existing Already...!";
                        //else
                        //{
                        ResultArgs ResultArg = CMSHandler.SaveOrUpdate(Result, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.UpdateNMESettings), sAcademicYear);
                        if (ResultArg.Success)
                            sResult = Common.Messages.RecordsUpdatedSuccessfully;
                        else
                            sResult = Common.Messages.FailedToSaveRecords;
                        //}
                    }
                    return Json(sResult);
                }
                else
                    sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateRegistration", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateRegistration", "NMEController", ex.Message);
                }
                sResult = Common.ErrorMessage.InternalServerError;
            }
            return Json(sResult);
        }
        public JsonResult DeleteNMESettings(int id)
        {
            string sResult = string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                DeleteNMESettings objModel = new DeleteNMESettings();
                objModel.SETTINGS_ID = Convert.ToString(id);
                ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.DeleteNMESettings), sAcademicYear);
                sResult = (ResultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
            }
            else
                sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            return Json(sResult);
        }
        #endregion

        #region NME Class Course
        public async Task<ActionResult> NMEClassCourseList()
        {
            NMEViewModel objViewModel = new NMEViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                var NMEClassCourse = new NMEClassCourse();
                var ListClassCourse = await Task.Run(() => (List<NMEClassCourse>)CMSHandler.FetchData<NMEClassCourse>(null, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMEClassCourseList), sAcademicYear).DataSource.Data);
                objViewModel.lstNMEClassCourse = ListClassCourse;
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objViewModel);
        }
        public ActionResult NMEClassCourse()
        {
            NMEViewModel objViewModel = new NMEViewModel();
            var NMESetting = new List<NME_Setting>();
            var Course = new List<cp_Course_Root_2017>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                NMESetting = (List<NME_Setting>)CMSHandler.FetchData<NME_Setting>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchNMESetting), sAcademicYear).DataSource.Data;
                Course = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOnlyNMECoursesList), sAcademicYear).DataSource.Data;
                if (NMESetting != null && NMESetting.Count > 0)
                    objViewModel.NMESettingList = new SelectList(NMESetting, Common.NME_SETTINGS.SETTINGS_ID, Common.NME_SETTINGS.SETTINGS_NAME);
                //if (Course != null && Course.Count > 0)
                // objViewModel.NMECourseList = new SelectList(Course, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_TITLE);
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objViewModel);
        }
        public ActionResult GetNMEClassCourseWithQuota(string sSettingId, string SClassId, string SCourseId)
        {
            NMEViewModel objViewModel = new NMEViewModel();
            BindNMEClassCourseQuota objBindClassCourseQuotaModel = new BindNMEClassCourseQuota();
            DeleteNMEClassCourse objDeleteBulkNMEClassCourse = new DeleteNMEClassCourse();
            DeleteNMEClassCourseQuota objDeleteBulkNMEClassCourseQuota = new DeleteNMEClassCourseQuota();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objDeleteBulkNMEClassCourse.CLASS_ID = SClassId;
                    objBindClassCourseQuotaModel.CLASS_ID = SClassId;
                    objDeleteBulkNMEClassCourseQuota.CLASS_ID = SClassId;
                    objBindClassCourseQuotaModel.COURSE_ROOT_ID = SCourseId;

                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var NMEClassCourse = new NMEClassCourse();
                    // Fetch Class & Shift ....
                    var FetchShiftByClassId = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(objBindClassCourseQuotaModel, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShiftByClassId), sAcademicYear).DataSource.Data;
                    if (FetchShiftByClassId != null && FetchShiftByClassId.Count > 0)
                        objBindClassCourseQuotaModel.SHIFT = FetchShiftByClassId.FirstOrDefault().SHIFT;

                    // Fetch Class Course Quota ...
                    string sClassCourseQuotaSQL = SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.BindNMEClassCourseQuota);
                    sClassCourseQuotaSQL = sClassCourseQuotaSQL.Replace(Common.Delimiter.QUS + Common.NME_CLASS_COURSE_2017.COURSE_ID, SCourseId);
                    sClassCourseQuotaSQL = sClassCourseQuotaSQL.Replace(Common.Delimiter.QUS + Common.SUP_SHIFT.SHIFT_ID, objBindClassCourseQuotaModel.SHIFT);
                    var ListClassCourse = (List<BindNMEClassCourseQuota>)CMSHandler.FetchData<BindNMEClassCourseQuota>(objBindClassCourseQuotaModel, sClassCourseQuotaSQL, sAcademicYear).DataSource.Data;
                    objViewModel.lstBindNMEClassCourse = ListClassCourse;

                    var liUnSafeMode = CMSHandler.SaveOrUpdate(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));

                    //// Delete NME Class Course ...
                    //string sDeleteBulkNMEClassCourse = SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.DeleteBulkNMEClassCourse);
                    //sDeleteBulkNMEClassCourse = sDeleteBulkNMEClassCourse.Replace(Common.Delimiter.QUS + Common.NME_CLASS_COURSE_2017.COURSE_ID, SCourseId);
                    //ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objDeleteBulkNMEClassCourse, sDeleteBulkNMEClassCourse, sAcademicYear);

                    //// Delete NME Class Course Quota ...
                    //string sDeleteBulkNMEClassCourseQuota = SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.DeleteBulkNMEClassCourseQuota);
                    //sDeleteBulkNMEClassCourseQuota = sDeleteBulkNMEClassCourseQuota.Replace(Common.Delimiter.QUS + Common.NME_CLASS_COURSE_QUOTA_2017.COURSE_ID, SCourseId);
                    //ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(objDeleteBulkNMEClassCourseQuota, sDeleteBulkNMEClassCourseQuota, sAcademicYear);

                    var liSafeMode = CMSHandler.SaveOrUpdate(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("GetNMEClassCourseWithQuota", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("GetNMEClassCourseWithQuota", "NMEController", ex.Message);
                }

            }
            return View(objViewModel);
        }
        public string SaveNMEClassCourse(string sClassCourse)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sResult = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    if (!string.IsNullOrEmpty(sClassCourse))
                    {
                        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                        JsonNMEClassCourse objJson = JsonConvert.DeserializeObject<JsonNMEClassCourse>(sClassCourse);
                        foreach (var item in objJson.NMEClassCourse)
                        {

                            //NMEViewModel objViewModel = new NMEViewModel();
                            //var NMEClassCourse = new NMEClassCourse();
                            var ListClassCourse = (List<SaveNMEClassCourse>)CMSHandler.FetchData<SaveNMEClassCourse>(item, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.IsNMEClassCourseExisting), sAcademicYear).DataSource.Data;
                            var ListClassCourseQuota = (List<SaveNMEClassCourseQuota>)CMSHandler.FetchData<SaveNMEClassCourseQuota>(item, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.IsNMEClassCourseQuotaExisting), sAcademicYear).DataSource.Data;
                            if (ListClassCourse != null)
                            {
                                //foreach (var List in ListClassCourse)
                                //{
                                //    item.NME_CLASS_COURSE_ID = List.NME_CLASS_COURSE_ID;
                                //    ResultArgs ResultArg = CMSHandler.SaveOrUpdate(item, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.UpdateNMEClassCourse), sAcademicYear);
                                //    //ResultArgs Resultarg = CMSHandler.SaveOrUpdate(item, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.UpdateNMEClassCourseQuota), sAcademicYear);
                                //    if (ResultArg.Success)
                                //    {
                                //        sResult = Common.Messages.RecordsSavedSuccessfully;
                                //    }
                                //    else
                                //    {
                                //        sResult = Common.Messages.FailedToSaveRecords;
                                //    }
                                //}
                            }
                            else
                            {
                                ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(item, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.SaveNMEClassCourse), sAcademicYear);
                            }
                            if (ListClassCourseQuota != null && ListClassCourseQuota.Count > 0)
                            {
                                foreach (var dataitem in ListClassCourseQuota)
                                {
                                    item.NME_CLASS_COURSE_QUOTA_ID = dataitem.NME_CLASS_COURSE_QUOTA_ID;
                                    int nQuota = Convert.ToInt32(item.QUOTA) + Convert.ToInt32(dataitem.QUOTA);
                                    item.QUOTA = Convert.ToString(nQuota);
                                    //ResultArgs ResultArg = CMSHandler.SaveOrUpdate(item, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.UpdateNMEClassCourse), sAcademicYear);
                                    ResultArgs Resultarg = CMSHandler.SaveOrUpdate(item, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.UpdateNMEClassCourseQuota), sAcademicYear);
                                    if (Resultarg.Success)
                                    {
                                        sResult = Common.Messages.RecordsSavedSuccessfully;
                                    }
                                    else
                                    {
                                        sResult = Common.Messages.FailedToSaveRecords;
                                    }
                                }
                            }


                            else
                            {
                                //ResultArgs ResultArg = CMSHandler.SaveOrUpdate(item, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.SaveNMEClassCourse), sAcademicYear);
                                ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(item, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.SaveNMEClassCourseQuota), sAcademicYear);
                                if (ResultArgs.Success)
                                {
                                    if (ResultArgs.Success)
                                        sResult = Common.Messages.RecordsSavedSuccessfully;
                                    else
                                        sResult = Common.Messages.FailedToSaveRecords;
                                }
                                else
                                {
                                    sResult = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                    else
                    {
                        sResult = Common.Messages.FailedToSaveRecords;
                    }
                }
                else
                {
                    sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveNMEClassCourse", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveNMEClassCourse", "NMEController", ex.Message);
                }
            }
            return sResult;
        }
        public JsonResult GetCourseId(string sSettingId)
        {
            string sOption = string.Empty;
            string sClass = string.Empty;
            string sClassId = string.Empty;
            NMEViewModel objViewModel = new NMEViewModel();
            NMEClassCourse objModel = new NMEClassCourse();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.SETTINGS_ID = sSettingId;
                    var NMECourse = new List<NMEClassCourse>();
                    var Class = new List<NMEClassCourse>();
                    var Course = new List<cp_Course_Root_2017>();
                    Course = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.GetRegisteredCoursesFromCourseAllotment), sAcademicYear).DataSource.Data;
                    NMECourse = (List<NMEClassCourse>)CMSHandler.FetchData<NMEClassCourse>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.GetSelectedNMECourseId), sAcademicYear).DataSource.Data;
                    Class = (List<NMEClassCourse>)CMSHandler.FetchData<NMEClassCourse>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.GetSelectedClassIDBySettingsId), sAcademicYear).DataSource.Data;
                    if (Class != null && Class.Count > 0)
                    {
                        sClassId = Class[0].CLASS_ID;
                        sClass = Class[0].CLASS_CODE;
                    }
                    //    NMEClass = (List<NMEClassCourse>)CMSHandler.FetchData<NMEClassCourse>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.GetSelectedNMEClassId), sAcademicYear).DataSource.Data;
                    if (NMECourse != null && NMECourse.Count > 0)
                    {
                        objViewModel.SelectedNMECourseList = new SelectList(NMECourse, Common.NME_CLASS_COURSE_2017.COURSE_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE, objModel.SELECTED);
                        foreach (var List in NMECourse)
                        {
                            sOption += "<option value='" + List.COURSE_ID + "' " + List.SELECTED + " >" + List.COURSE_CODE + "</option>";
                            Course.Remove(Course.SingleOrDefault(o => o.COURSE_ROOT_ID == List.COURSE_ID));
                        }
                        sClassId = NMECourse[0].CLASS_ID;
                        sClass = NMECourse[0].CLASS_CODE;
                    }
                    if (Course != null && Course.Count > 0)
                    {
                        foreach (var List in Course)
                            sOption += "<option value='" + List.COURSE_ROOT_ID + "' >" + List.COURSE_TITLE + "</option>";
                    }
                }
                else
                {
                    sOption = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    return Json(sOption, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("GetCourseId", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("GetCourseId", "NMEController", ex.Message);
                }

            }
            FetchNMEModel objFetchModel = new FetchNMEModel() { CLASS = sClass, COURSE = sOption, CLASS_ID = sClassId };
            return Json(objFetchModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteNMEClassCourse(string id)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sResult = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    DeleteNMEClassCourse objModel = new DeleteNMEClassCourse();
                    DeleteNMEClassCourseQuota objCourseAllotment = new DeleteNMEClassCourseQuota();
                    NMEClassCourse objClassCourse = new NMEClassCourse();
                    objModel.NME_CLASS_COURSE_ID = id;

                    // Delete NME Courses ...
                    ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.DeleteNMEClassCourse), sAcademicYear);
                    if (ResultArg.Success)
                    {
                        // Fetch NME Class Course ...
                        var FetchNMEClassCourse = (List<NMEClassCourse>)CMSHandler.FetchData<NMEClassCourse>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMEClassCourseById), sAcademicYear).DataSource.Data;
                        if (FetchNMEClassCourse != null && FetchNMEClassCourse.Count > 0)
                        {
                            objClassCourse.CLASS_ID = FetchNMEClassCourse.FirstOrDefault().CLASS_ID;
                            objClassCourse.COURSE_ID = FetchNMEClassCourse.FirstOrDefault().COURSE_ID;
                            objClassCourse.SETTINGS_ID = FetchNMEClassCourse.FirstOrDefault().SETTINGS_ID;

                            // Fetch NME Class Course Quata ...
                            var ListNMEClassCourseQuota = (List<NMEClassCourseQuota>)CMSHandler.FetchData<NMEClassCourseQuota>(objClassCourse, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchClassCourseQuotaByClassIdAndCourseIdAndSettingId), sAcademicYear).DataSource.Data;
                            if (ListNMEClassCourseQuota != null)
                            {
                                // Delete NME Class Course Quata ...
                                objCourseAllotment.NME_CLASS_COURSE_QUOTA_ID = ListNMEClassCourseQuota[0].NME_CLASS_COURSE_QUOTA_ID;
                                ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(objCourseAllotment, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.DeleteNMEClassCourseQuotaFromClassCourse), sAcademicYear);

                            }
                            sResult = (ResultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DeleteNMEClassCourse", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DeleteNMEClassCourse", "NMEController", ex.Message);
                    }
                    sResult = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(sResult);
        }
        #endregion

        #region NME Course Allotment
        public async Task<ActionResult> NMECourseAllotmentList()
        {
            NMEViewModel objViewModel = new NMEViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                var NMECourseAllotment = new NMECourseAllotment();
                var ListNMECourseAllotment = await Task.Run(() => (List<NMECourseAllotment>)CMSHandler.FetchData<NMECourseAllotment>(null, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMECourseAllotment), sAcademicYear).DataSource.Data);
                objViewModel.lstNMECourseAllotment = ListNMECourseAllotment;
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objViewModel);
        }
        public ActionResult NMECourseAllotment()
        {
            NMEViewModel objViewModel = new NMEViewModel();
            var Shift = new List<Sup_Shift>();
            var Course = new List<cp_Course_Root_2017>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    Shift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    Course = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(null, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.GetRegisteredCoursesFromCourseRegistration), sAcademicYear).DataSource.Data;
                    if (Course != null && Course.Count > 0)
                        objViewModel.NMECourseList = new SelectList(Course, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_TITLE);
                    if (Shift != null && Shift.Count > 0)
                        objViewModel.ShiftList = new SelectList(Shift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("NMECourseAllotment", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("NMECourseAllotment", "NMEController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public string SaveNMECourseAllotment(string NMECourseAllotment)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            var sResult = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (NMECourseAllotment != null)
                    {
                        var Result = JsonConvert.DeserializeObject<SaveNMECourseAllotment>(NMECourseAllotment);
                        Result.ACADEMIC_YEAR = sAcademicYear;
                        var FetchExistingCourse = (List<NMECourseAllotment>)CMSHandler.FetchData<NMECourseAllotment>(Result, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.IsNMECourseAllotmentExisting), sAcademicYear).DataSource.Data;
                        if (FetchExistingCourse != null)
                        {
                            sResult = "This Course Is Existing Already...!";
                        }
                        else
                        {
                            ResultArgs ResultArg = CMSHandler.SaveOrUpdate(Result, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.SaveNMECourseAllotment), sAcademicYear);
                            if (ResultArg.Success)
                            {
                                sResult = Common.Messages.RecordsSavedSuccessfully;
                            }
                            else
                            {
                                sResult = Common.Messages.FailedToSaveRecords;
                            }
                        }
                    }
                }
                else
                    sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveNMECourseAllotment", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveNMECourseAllotment", "NMEController", ex.Message);
                }
            }
            return sResult;
        }
        public ActionResult EditNMECourseAllotment(int id)
        {
            NMEViewModel objViewModel = new NMEViewModel();
            var Shift = new List<Sup_Shift>();
            var Course = new List<cp_Course_Root_2017>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    Session[Common.SESSION_VARIABLES.ALLOTMENT_ID] = id;
                    Shift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    Course = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(null, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.GetRegisteredCoursesFromCourseRegistration), sAcademicYear).DataSource.Data;
                    if (Course != null && Course.Count > 0)
                        objViewModel.NMECourseList = new SelectList(Course, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_TITLE);
                    if (Shift != null && Shift.Count > 0)
                        objViewModel.ShiftList = new SelectList(Shift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EditNMECourseAllotment", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EditNMECourseAllotment", "NMEController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public async Task<JsonResult> FetchNMECourseAllotmentById(string sCourseAllotmentId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            SaveNMECourseAllotment objModel = new SaveNMECourseAllotment();
            var sResult = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.ALLOTMENT_ID = sCourseAllotmentId;
                    var ListNMECourseAllotment = await Task.Run(() => (List<SaveNMECourseAllotment>)CMSHandler.FetchData<SaveNMECourseAllotment>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMECourseAllotmentById), sAcademicYear).DataSource.Data);
                    return Json(ListNMECourseAllotment);
                }
                else
                {
                    sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    return Json(sResult);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EditNMECourseAllotment", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EditNMECourseAllotment", "NMEController", ex.Message);
                    return Json(sResult);
                }
            }
        }
        public JsonResult UpdateNMECourseAllotment(string NMECourseAllotment)
        {
            string sResult = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (NMECourseAllotment != null)
                    {
                        var Result = JsonConvert.DeserializeObject<SaveNMECourseAllotment>(NMECourseAllotment);
                        Result.ALLOTMENT_ID = (Session[Common.SESSION_VARIABLES.ALLOTMENT_ID] != null) ? Session[Common.SESSION_VARIABLES.ALLOTMENT_ID].ToString() : "";
                        Result.ACADEMIC_YEAR = sAcademicYear;
                        ResultArgs ResultArg = CMSHandler.SaveOrUpdate(Result, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.UpdateNMECourseAllotment), sAcademicYear);
                        if (ResultArg.Success)
                            sResult = Common.Messages.RecordsSavedSuccessfully;
                        else
                            sResult = Common.Messages.FailedToSaveRecords;
                    }
                    else
                        sResult = Common.ErrorMessage.BadRequest;
                }
                else
                    sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateNMECourseAllotment", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateNMECourseAllotment", "NMEController", ex.Message);
                }
                sResult = Common.ErrorMessage.InternalServerError;
            }
            return Json(sResult);
        }
        public JsonResult DeleteNMECourseAllotment(int id)
        {

            string sResult = string.Empty;
            DeleteNMECourseAllotment objModel = new DeleteNMECourseAllotment();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.ALLOTMENT_ID = Convert.ToString(id);
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.DeleteNMECourseAllotment), sAcademicYear);

                    sResult = (ResultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
                else
                    sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DeleteNMECourseAllotment", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DeleteNMECourseAllotment", "NMEController", ex.Message);
                }
                sResult = Common.ErrorMessage.InternalServerError;
            }
            return Json(sResult);
        }
        #endregion 

        #region NME Registration By Admin
        public ActionResult NMERegisteredStudentList()
        {
            //NMEViewModel objViewModel = new NMEViewModel();
            //var Shift = new List<Sup_Shift>();
            //var Class = new List<cp_Classes_2017>();
            //string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            //Shift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
            //Class = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOnlyOrdinaryClass), sAcademicYear).DataSource.Data;
            //if (Shift != null && Shift.Count > 0)
            //    objViewModel.ShiftList = new SelectList(Shift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
            //if (Class != null && Class.Count > 0)
            //    objViewModel.ClassList = new SelectList(Class, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_CODE);
            //return View(objViewModel);
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                NMEViewModel objViewModel = new NMEViewModel();
                var NMERegisteredStudent = new NMERegisteredStudent();
                //NMERegisteredStudent.CLASS_ID = sClassId;
                string sSQL = SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMERegisteredStudentByClassId).Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                var ListNMERegisteredStudent = (List<NMERegisteredStudent>)CMSHandler.FetchData<NMERegisteredStudent>(null, sSQL, sAcademicYear).DataSource.Data;
                objViewModel.lstNMERegisteredStudent = ListNMERegisteredStudent;

                var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null,
                      SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                if (liClass != null && liClass.Count > 0)
                {
                    objViewModel.ClassList = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
                return View(objViewModel);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
        public async Task<ActionResult> BindNMERegisteredStudent(string sClassId)
        {
            NMEViewModel objViewModel = new NMEViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            var NMERegisteredStudent = new NMERegisteredStudent();
            NMERegisteredStudent.CLASS_ID = sClassId;
            var ListNMERegisteredStudent = await Task.Run(() => (List<NMERegisteredStudent>)CMSHandler.FetchData<NMERegisteredStudent>(NMERegisteredStudent, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMERegisteredStudentByClassId), sAcademicYear).DataSource.Data);
            objViewModel.lstNMERegisteredStudent = ListNMERegisteredStudent;
            return View(objViewModel);
        }
        public JsonResult FetchNMERegisteredStudentById(string sNMERegiseteredStudent)
        {
            NMEViewModel objViewModel = new NMEViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            Session[Common.SESSION_VARIABLES.REGISTRATION_ID] = sNMERegiseteredStudent;
            NMERegisteredStudent objModel = new NMERegisteredStudent();
            objModel.REGISTRATION_ID = sNMERegiseteredStudent;
            var FetchNMERegisteredStudent = (List<NMERegisteredStudent>)CMSHandler.FetchData<NMERegisteredStudent>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMERegisteredStudentById), sAcademicYear).DataSource.Data;
            if (FetchNMERegisteredStudent != null && FetchNMERegisteredStudent.Count > 0)
            {
                foreach (var item in FetchNMERegisteredStudent)
                {
                    objModel.CLASS_ID = item.CLASS_ID;
                    objModel.CLASS_CODE = item.CLASS_CODE;
                    objModel.STUDENT_ID = item.STUDENT_ID;
                    objModel.FIRST_NAME = item.FIRST_NAME;
                    objModel.ROLL_NO = item.ROLL_NO;
                    objModel.COURSE_ID = item.COURSE_ID;
                    objModel.SUMOFQUTOA = item.SUMOFQUTOA;
                    objModel.SHIFT_ID = item.SHIFT_ID;
                    objModel.REGISTRATION_ID = item.REGISTRATION_ID;

                }

                var Course = new List<NMERegisteredStudent>();
                Course = (List<NMERegisteredStudent>)CMSHandler.FetchData<NMERegisteredStudent>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMECourseByClassId), sAcademicYear).DataSource.Data;
                //objViewModel.NMECourseList = new SelectList(Course, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
                if (Course != null && Course.Count > 0)
                {

                    foreach (var item in Course)
                    {
                        var SumOfClassCourseQuota = (List<NMERegisteredStudent>)CMSHandler.FetchData<NMERegisteredStudent>(new NMERegisteredStudent() {SHIFT_ID=objModel.SHIFT_ID, CLASS_ID = objModel.CLASS_ID, COURSE_ID = item.COURSE_ID }, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchSumOfNMEClassQuotaByClassIdAndCourseId), sAcademicYear).DataSource.Data;
                        if (SumOfClassCourseQuota != null)
                        {
                            objModel.COURSE_CODE += "<option value='" + item.COURSE_ID + "'>" + item.COURSE_CODE + "<span style='background-color: #05ffb0;'>" + "(Alotted-" + SumOfClassCourseQuota.FirstOrDefault().ALLOTED_SEATS + "/Applied-" + SumOfClassCourseQuota.FirstOrDefault().SUMOFQUTOA + ")" + "</span>" + "</option>";
                        }
                    }
                }
            }
            //var Course = new List<NMERegisteredStudent>();
            //Course = (List<NMERegisteredStudent>)CMSHandler.FetchData<NMERegisteredStudent>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMECourseByClassId), sAcademicYear).DataSource.Data;
            //objViewModel.NMECourseList = new SelectList(Course, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
            return Json(objModel);
        }
        public string GetNMECourseListByClassId(string sClassId)
        {
            string sOption = string.Empty;
            string objJsonData = string.Empty;
            NMEViewModel objViewModel = new NMEViewModel();
            NMERegisteredStudent objModel = new NMERegisteredStudent();
            objModel.CLASS_ID = sClassId;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            var Course = new List<NMERegisteredStudent>();
            Course = (List<NMERegisteredStudent>)CMSHandler.FetchData<NMERegisteredStudent>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMECourseByClassId), sAcademicYear).DataSource.Data;
            //objViewModel.NMECourseList = new SelectList(Course, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
            if (Course != null && Course.Count > 0)
            {
                foreach (var item in Course)
                {
                    sOption += "<option value='" + item.COURSE_ID + "'>" + item.COURSE_CODE + "</option>";
                }
            }
            var JsonData = new SaveNMECourseRegistration() { COURSE_ID = sOption };
            objJsonData = JsonConvert.SerializeObject(JsonData);
            return objJsonData;
        }
        public JsonResult GetNMEClassCourseQuotaByCourseId(string sCourseId, string sClassId)
        {
            string sOption = string.Empty;
            NMEViewModel objViewModel = new NMEViewModel();
            NMERegisteredStudent objModel = new NMERegisteredStudent();
            objModel.COURSE_ID = sCourseId;
            objModel.CLASS_ID = sClassId;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            var SumOfClassCourseQuota = new List<NMERegisteredStudent>();
            SumOfClassCourseQuota = (List<NMERegisteredStudent>)CMSHandler.FetchData<NMERegisteredStudent>(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchSumOfNMEClassQuotaByClassIdAndCourseId), sAcademicYear).DataSource.Data;
            if (SumOfClassCourseQuota != null && SumOfClassCourseQuota.Count > 0)
            {
                foreach (var item in SumOfClassCourseQuota)
                {
                    objModel.SUMOFQUTOA = item.SUMOFQUTOA;
                    objModel.ALLOTED_SEATS = item.ALLOTED_SEATS;
                }
            }
            return Json(objModel);
        }
        public JsonResult UpdateNMERegisteredStudent(string NMERegisteredStudent)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sResult = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (NMERegisteredStudent != null)
                    {
                        var Result = JsonConvert.DeserializeObject<NMERegisteredStudent>(NMERegisteredStudent);
                        var FetchSelectedClassByStudentIdAndCourseId = (List<NMERegisteredStudent>)CMSHandler.FetchData<NMERegisteredStudent>(Result, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchSelectedClassIdByStudentIdAndCourseId), sAcademicYear).DataSource.Data;
                        if (FetchSelectedClassByStudentIdAndCourseId != null && FetchSelectedClassByStudentIdAndCourseId.Count > 0)
                        {
                            foreach (var item in FetchSelectedClassByStudentIdAndCourseId)
                                Result.S_CLASS_ID = item.CLASS_ID;
                        }
                        ResultArgs ResultArg = CMSHandler.SaveOrUpdate(Result, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.UpdateNMERegisteredStudent), sAcademicYear);
                        if (ResultArg.Success)
                        {
                            sResult = Common.Messages.RecordsSavedSuccessfully;
                        }
                        else
                        {
                            sResult = Common.Messages.FailedToSaveRecords;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("UpdateNMERegisteredStudent", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                        objHandler.WriteError("UpdateNMERegisteredStudent", "NMEController", ex.Message);
                    }
                    sResult = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }

            return Json(sResult);
        }
        public JsonResult DeleteNMERegisteredStudent(int id)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sResult = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                DeleteNMECourseRegistration objModel = new DeleteNMECourseRegistration();
                objModel.REGISTRATION_ID = Convert.ToString(id);

                ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.DeleteNMECourseRegisteredStudent), sAcademicYear);
                sResult = (ResultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
            }
            else
            {
                sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }

            return Json(sResult);
        }
        public string FetchNMERegisteredStudentAndUpdateStuCourseInfo()
        {
            string sResult = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                SaveNMERegisteredStudent objModel = new SaveNMERegisteredStudent();
                string sSQL = SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMERegisteredStudentToPush).
                    Replace(Common.Delimiter.QUS + Common.SEMESTER_MARKS_2017.SEMESTER, sActiveSemester);
                int nCount = 0;
                // Fetch Registered Student For NME Registration By Current Semester ....
                var FetchNMERegisteredStudentToPush = (List<NMERegisteredStudent>)CMSHandler.FetchData<NMERegisteredStudent>(null, sSQL, sAcademicYear).DataSource.Data;
                if (FetchNMERegisteredStudentToPush != null && FetchNMERegisteredStudentToPush.Count > 0)
                {
                    foreach (var item in FetchNMERegisteredStudentToPush)
                    {
                        objModel.CLASS_ID = item.CLASS_ID;
                        objModel.COURSE_ID = item.COURSE_ID;
                        objModel.STUDENT_ID = item.STUDENT_ID;
                        objModel.S_CLASS_ID = item.S_CLASS_ID;
                        objModel.SEMESTER = item.SEMESTER;
                        var FetchStuCourseInfo = (List<NMERegisteredStudent>)CMSHandler.FetchData<NMERegisteredStudent>(objModel,
                            SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchStuCourseInfo), sAcademicYear).DataSource.Data;
                        if (FetchStuCourseInfo == null)
                        {
                            ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.PushNMERegisteredStudentToStuCourseInfo), sAcademicYear);
                            sResult = (ResultArg.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        }
                        else
                        {
                            nCount++;
                        }
                    }
                    if (FetchNMERegisteredStudentToPush.Count == nCount)
                    {
                        sResult = "No Changes Made !";
                    }
                }
            }
            else
            {
                sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return sResult;
        }

        public JsonResult GetStudentAndNMECourseList(string sClassId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                // Fetch Student By Class Id ...
                var liStudentList = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new STU_CLASS() { CLASS_ID = sClassId },
                      SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMENotRegisteredStudentForDDL), sAcademicYear).DataSource.Data;
                if (liStudentList != null && liStudentList.Count > 0)
                {
                    foreach (var item in liStudentList)
                    {
                        ObjJsonData.sResult += "<option value=" + item.STUDENT_ID + "> " + item.FIRST_NAME + " </option>";
                    }
                }

                // Fetch NME Course By Class Id ...
                string sSQL = SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.LoadNMECourseList).Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                var liCourseList = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(new STU_CLASS() { CLASS_ID = sClassId }, sSQL, sAcademicYear).DataSource.Data;
                //var liCourseList = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(new STU_CLASS() {CLASS_ID=sClassId },
                //      SQL.Student.StudentSQL.GetStudentSQL(StudentSQLCommads.FetchOptionalCoursebyShiftIdAndClassId),sAcademicYear).DataSource.Data;
                if (liCourseList != null && liCourseList.Count > 0)
                {
                    ObjJsonData.ErrorCode = "<option value=0> -- Select -- </option>";
                    foreach (var item in liCourseList)
                    {
                        ObjJsonData.ErrorCode += "<option value=" + item.COURSE_ROOT_ID + " cms-Semster=" + item.SEMESTER_ID + " cms-sClassId=" + item.NME_CLASS_ID + " cms-SettingsId=" + item.SETTINGS_ID + " " + item.QUOTA + ">" + item.COURSE_TITLE + "</option>";
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveNMERegistration(string sClassId, string sCourseId, string sStudentId, string sSelectedClassId, string sSemesterId, string sSettingId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new SaveNMERegisteredStudent();
                ObjModel.CLASS_ID = sClassId;
                ObjModel.COURSE_ID = sCourseId;
                ObjModel.S_CLASS_ID = sSelectedClassId;
                ObjModel.SEMESTER = sSemesterId;
                ObjModel.SETTINGS_ID = sSettingId;
                if (string.IsNullOrEmpty(sSettingId))
                {
                    ObjJsonData.Message = "Classes Are Not Mapped !";
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
                var sStudentIds = sStudentId.Split(',');
                string sSQL = SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.IsNMEStudentRegistrationExist).Replace(Common.Delimiter.QUS + Common.SEMESTER_MARKS_2017.SEMESTER, sActiveSemester);
                foreach (var item in sStudentIds)
                {
                    ObjModel.STUDENT_ID = item;

                    // Check Registration Exist ...

                    var IsRegistrationExist = (List<SaveNMERegisteredStudent>)CMSHandler.FetchData<SaveNMERegisteredStudent>(ObjModel, sSQL, sAcademicYear).DataSource.Data;
                    if (IsRegistrationExist == null)
                    {
                        // Save NME Student Registration ...
                        var SaveNMERegistration = CMSHandler.SaveOrUpdate(ObjModel, SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.SaveNMEStudentRegistration), sAcademicYear);
                        ObjJsonData.Message = (SaveNMERegistration.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                }
                if (string.IsNullOrEmpty(ObjJsonData.Message))
                {
                    ObjJsonData.Message = "No Changes Made !";
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region  NME Student Registered not Registered Report
        /// <summary>
        /// NME Student Registered not Registered Report
        /// </summary>
        /// <author>Yesudass</author>
        /// <date>2025-06-26</date>
        /// <returns></returns>

        public ActionResult NMERegisteredAndNotRegisteredStudentList()
        {

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                NMEViewModel objViewModel = new NMEViewModel();
                var NMERegisteredStudent = new NMERegisteredStudent();
                // Fetch  Academic Year List ,..
                var liAcademicYearList = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
                    SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                objViewModel.AcademicYearList = new SelectList(liAcademicYearList, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                var SemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
                objViewModel.SemesterList = new SelectList(SemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);


                return View(objViewModel);
            }
            else
            {
                return RedirectToAction("NME", "NMEANDSLCRegisteredStudentList");
            }
        }
        public ActionResult BindNMERegisteredStudentList(string sAcademicYear, string sSemestertype, string sType)
        {
            NMEViewModel objViewModel = new NMEViewModel();
            string sSemesterIds = string.Empty;
            string sSQL = string.Empty;
            string registType = string.Empty;
            try
            {
                if (sType == "1")
                {
                    registType = "-- R";
                }
                else
                {
                    registType = "-- N";
                }
                if (sSemestertype == Common.IsActiveFalg.IsActive)
                    sSemesterIds = Common.SUP_SEMESTER_TYPE.ODD;
                else
                    sSemesterIds = Common.SUP_SEMESTER_TYPE.EVEN;
                sSQL = SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMERegisteredListBySemestertype).Replace(registType,"").Replace(Common.Delimiter.QUS + Common.SESSION_VARIABLES.SEMESTER_ID, sSemesterIds);
                var ListNMERegisteredStudent = (List<NMERegisteredStudent>)CMSHandler.FetchData<NMERegisteredStudent>(new SUP_SEMESTER_TYPE() { SEMESTER_TYPE = sSemestertype }, sSQL, sAcademicYear).DataSource.Data;
                objViewModel.lstNMERegisteredStudent = ListNMERegisteredStudent;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("NME", "BindNMEANDSLCRegisteredStudent", ex.Message);
                }
            }
            return View(objViewModel);
        }
        #endregion
    }
}