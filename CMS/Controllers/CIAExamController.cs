using CMS.DAO;
using CMS.SQL.Examination;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CMS.Controllers
{
    public class CIAExamController : Controller
    {
        ResultArgs resultArgs = new ResultArgs();
        // GET: CIAExam
        public ActionResult Index()
        {
            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                Session.Abandon();
                FormsAuthentication.SignOut();
            }

            var ObjViewModel = new AccountViewModel();
            ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;

            return View(ObjViewModel);
        }
        public ActionResult Login(ViewModel.ViewModel.AccountViewModel collection)
        {
            try
            {

                using (AccountViewModel objAccount = new AccountViewModel())
                {

                    ResultArgs resultArgs = new ResultArgs();
                    DataValue dv = new DataValue();
                    string sUserRoles = string.Empty;
                    string sUserRoleId = string.Empty;
                    dv.Clear();
                    dv.Add(Common.USER_ACCOUNT.USERNAME, collection.objLoginViewModel.Username, EnumCommand.DataType.String);
                    string sPassword = CommonMethods.GetSha256FromString(collection.objLoginViewModel.Password);
                    dv.Add(Common.USER_ACCOUNT.PASSWORD, sPassword, EnumCommand.DataType.String);

                    resultArgs =  objAccount.FetchUserCredential(dv);

                    if (resultArgs.Success)
                    {
                        if (resultArgs.DataSource.Table != null && resultArgs.DataSource.Table.Rows.Count > 0)
                        {
                                                                         
                            DataTable dtActiveYear = new DataTable();
                            dtActiveYear = objAccount.FetchActiveYear().DataSource.Table;
                            //Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] = dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.AC_YEAR].ToString();
                            //Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_NAME] = dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.ACADEMIC_NAME].ToString();
                            //Session[Common.SESSION_VARIABLES.USER_ID] = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.USER_ID];
                            //Session[Common.SESSION_VARIABLES.USER_ID] = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.USER_ID];

                            return RedirectToAction("OnlineQuizEntry", "CIAExam", new { @sUserId = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.USER_ID].ToString() , @sAcademicYear = dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.AC_YEAR].ToString(),@sName= resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.NAME] });
                        }
                       
                        else
                        {
                            TempData["ErrorMessage"] = Common.Messages.InvalidUserCredentials;
                            return RedirectToAction("Index");
                        }
                      
                    }
                    else
                    {
                        TempData["ErrorMessage"] = Common.Messages.InvalidUserCredentials;
                        return RedirectToAction("Index");
                    }
                }

                

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AccountController", "Login", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountController", "Login", ex.Message);
                }
                TempData["ErrorMessage"] = Common.Messages.InvalidUserCredentials;
                return RedirectToAction("Index"); ;
            }

           
        }

        public ActionResult OnlineQuizEntry(string sUserId,string sAcademicYear,string sName)
        {
            //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new ExaminationViewModel();
            var studentcontroller = new StudentController();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ObjViewModel.sAcyear = sAcademicYear;
                ObjViewModel.sUserId = sUserId;
                ObjViewModel.sName = sName;

                try
                {
                        //Get All The Regular Papers By Student_Id ...
                        var liRegularCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId },
                                SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamRegistration), sAcademicYear).DataSource.Data;
                        var liOptionalCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId },
                              SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;

                        // Loop Optional Papers  To Remove Unnecessary Courses...
                        if (liOptionalCourses != null && liOptionalCourses.Count > 0)
                        {
                            foreach (var item in liOptionalCourses)
                            {
                                var IsExist = liRegularCourses.Where(o => o.COURSE_ROOT_ID == item.COURSE_ROOT_ID).ToList();
                                if (IsExist != null && IsExist.Count > 0)
                                {
                                    liRegularCourses.RemoveAll(o => o.COURSE_STRUCTURE_ID == IsExist.FirstOrDefault().COURSE_STRUCTURE_ID);
                                    item.COURSE_STRUCTURE_ID = IsExist.FirstOrDefault().COURSE_STRUCTURE_ID;
                                    liRegularCourses.Add(item);
                                }
                            }
                        }

                        var liClass = liRegularCourses.GroupBy(x => x.CLASS_ID).Select(x => new EXAM_REGISTRATION() { CLASS_ID = x.FirstOrDefault().CLASS_ID }).ToList();
                        string sClassIds = string.Join(",", liClass.Select(x => x.CLASS_ID).ToArray());

                        var liCourse = liRegularCourses.GroupBy(x => x.COURSE_ROOT_ID).Select(x => new EXAM_REGISTRATION() { COURSE_ID = x.FirstOrDefault().COURSE_ROOT_ID }).ToList();
                        string sCourseIds = string.Join(",", liCourse.Select(x => x.COURSE_ID).ToArray());

                        string sSQL = SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchActiveSettingForStudentQuizEntryByClassAndCourseIds);
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassIds);
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_2017.COURSE_ID, sCourseIds);
                        var liFetchQuizSetting = (List<QUIZ_CLASS_COURSE_STAFF>)CMSHandler.FetchData<QUIZ_CLASS_COURSE_STAFF>(null, sSQL, sAcademicYear).DataSource.Data;
                        //if (liFetchQuizSetting != null && liFetchQuizSetting.Count > 0)
                        //{
                        //    foreach (var item in liFetchQuizSetting)
                        //    {
                        //        item.STATUS = studentcontroller.ValidateQuizForSetWise(item.SETTING_ID, sUserId, item.COURSE_ID, sAcademicYear);
                        //    }
                        //}
                        ObjViewModel.liQuizSetting = liFetchQuizSetting;
                        
                  
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("CIAExam", "OnlineQuizForStudentBySetWise", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public  ActionResult BindOnlineQuizForSetWise(string sClassId, string sCourseId, string sSettingId, string sStaffId, string sTime,string sUserId,string sAcademicYear)
        {
            //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new ExaminationViewModel();
            var ObjJsonData = new JsonResultData();
            var StudentController = new StudentController();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ObjViewModel.sAcyear = sAcademicYear;
                ObjViewModel.sUserId = sUserId;

                try
                {                                           
                            // Fetch Class & Course Info ...
                            var liFetchCourse = (List<CourseInfo>)CMSHandler.FetchData<CourseInfo>(new Stu_Course_Info() { STUDENT_ID = sUserId, COURSE_ID = sCourseId },
                                  SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCourseInfoForQuiz), sAcademicYear).DataSource.Data;
                            ObjViewModel.liCourseInfo = liFetchCourse;

                            #region Match The Following Questin & Answer
                            // Fetch Match The Following Quiz Questions ...
                            var liQuizMatchTheFollowingQuestions = (List<OnlineQuizTest>)CMSHandler.FetchData<OnlineQuizTest>(
                                new QUIZ_STUDENT_SHUFFLE() { STUDENT_ID = sUserId, SETTING_ID = sSettingId, COURSE_ID = sCourseId },
                                 SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchMatchTheFollowingQuizQuestions), sAcademicYear).DataSource.Data;

                            // Fetch Match The Following Quiz Options ...
                            var liQuizMatchTheFollowingOptions = (List<OnlineQuizTest>)CMSHandler.FetchData<OnlineQuizTest>(
                               new QUIZ_STUDENT_SHUFFLE() { STUDENT_ID = sUserId, SETTING_ID = sSettingId, COURSE_ID = sCourseId },
                                SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchMatchTheFollowingQuizOptions), sAcademicYear).DataSource.Data;

                            #endregion

                            #region Objective Type Questions And Answers

                            // Fetch Objective Quiz Questions ...
                            var liQuizObjectiveQuestions = (List<OnlineQuizTest>)CMSHandler.FetchData<OnlineQuizTest>(
                                new QUIZ_STUDENT_SHUFFLE() { STUDENT_ID = sUserId, SETTING_ID = sSettingId, COURSE_ID = sCourseId },
                                 SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchObjectivesQuizQuestions), sAcademicYear).DataSource.Data;
                            if (liQuizObjectiveQuestions != null && liQuizObjectiveQuestions.Count > 0)
                            {
                                liQuizMatchTheFollowingQuestions = (liQuizMatchTheFollowingQuestions != null && liQuizMatchTheFollowingQuestions.Count > 0) ? liQuizMatchTheFollowingQuestions.Concat(liQuizObjectiveQuestions).ToList() : liQuizObjectiveQuestions;
                            }

                            // Fetch Objective Quiz Options ...
                            var liQuizObjectiveOptions = (List<OnlineQuizTest>)CMSHandler.FetchData<OnlineQuizTest>(
                                new QUIZ_STUDENT_SHUFFLE() { STUDENT_ID = sUserId, SETTING_ID = sSettingId, COURSE_ID = sCourseId,STAFF_ID=sStaffId },
                                 SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchObjectivesQuizOptions), sAcademicYear).DataSource.Data;
                            if (liQuizObjectiveOptions != null && liQuizObjectiveOptions.Count > 0)
                            {
                                liQuizMatchTheFollowingOptions = (liQuizMatchTheFollowingOptions != null && liQuizMatchTheFollowingOptions.Count > 0) ? liQuizMatchTheFollowingOptions.Concat(liQuizObjectiveOptions).ToList() : liQuizObjectiveOptions;
                            }

                            #endregion

                            #region Fill In The Blank Questions And Answers

                            // Fetch Fill In The Blank Quiz Questions ...
                            var liQuizFillInTheBlankQuestions = (List<OnlineQuizTest>)CMSHandler.FetchData<OnlineQuizTest>(
                                    new QUIZ_STUDENT_SHUFFLE() { STUDENT_ID = sUserId, SETTING_ID = sSettingId, COURSE_ID = sCourseId },
                                     SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchFillInTheBlankQuizQuestions), sAcademicYear).DataSource.Data;
                            if (liQuizFillInTheBlankQuestions != null && liQuizFillInTheBlankQuestions.Count > 0)
                            {
                                liQuizMatchTheFollowingQuestions = (liQuizMatchTheFollowingQuestions != null && liQuizMatchTheFollowingQuestions.Count > 0) ? liQuizMatchTheFollowingQuestions.Concat(liQuizFillInTheBlankQuestions).ToList() : liQuizFillInTheBlankQuestions;
                            }
                            ObjViewModel.liOnlineQuizQuestions = liQuizMatchTheFollowingQuestions;

                            // Fetch Fill In The Blank Quiz Options ...
                            var liQuizFillInTheBlankOptions = (List<OnlineQuizTest>)CMSHandler.FetchData<OnlineQuizTest>(
                                new QUIZ_STUDENT_SHUFFLE() { STUDENT_ID = sUserId, SETTING_ID = sSettingId, COURSE_ID = sCourseId },
                                 SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchFillInTheBlankQuizOptions), sAcademicYear).DataSource.Data;
                            if (liQuizFillInTheBlankOptions != null && liQuizFillInTheBlankOptions.Count > 0)
                            {
                                liQuizMatchTheFollowingOptions = (liQuizMatchTheFollowingOptions != null && liQuizMatchTheFollowingOptions.Count > 0) ? liQuizMatchTheFollowingOptions.Concat(liQuizFillInTheBlankOptions).ToList() : liQuizFillInTheBlankOptions;
                            }
                            ObjViewModel.liOnlineQuizOptions = liQuizMatchTheFollowingOptions;

                            #endregion
                            ObjViewModel.sTime = (!string.IsNullOrEmpty(sTime)) ? sTime : "0";
                            ObjViewModel.ClassId = sClassId;
                            ObjViewModel.CourseId = sCourseId;
                            ObjViewModel.SettingId = sSettingId;
                            ObjViewModel.StaffId = sStaffId;
                        //Fetch Quiz count status
                        var lifetchquizcountstatus = (List<QUIZ_CLASS_COURSE_STAFF>)CMSHandler.FetchData<QUIZ_CLASS_COURSE_STAFF>(new QUIZ_STUDENT_SHUFFLE() { STUDENT_ID=sUserId,SETTING_ID=sSettingId}, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchQuizcountStatus), sAcademicYear).DataSource.Data;
                        if (lifetchquizcountstatus!=null && lifetchquizcountstatus.Count>0)
                        {
                          ObjViewModel.liQuizSetting = lifetchquizcountstatus;
                        }
                          
                        var time = (List<QUIZ_CLASS_COURSE_STAFF>)CMSHandler.FetchData<QUIZ_CLASS_COURSE_STAFF>(new QUIZ_STUDENT_SHUFFLE() { STUDENT_ID=sUserId,SETTING_ID=sSettingId}, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchTimestatus), sAcademicYear).DataSource.Data;
                        if (time!=null && time.Count>0)
                        {
                            ObjViewModel.sSpendTime = time.FirstOrDefault().MINUTES;
                        }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Student", "BindOnlineQuizForSetWise", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        public async Task<JsonResult> SaveQuizForSetWise(string sJsonSaveQuiz, string sSettingId, string sStaffId,string sUserId,string sAcademicYear)
        {
            var ObjJsonData = new JsonResultData();
            string sSQL = string.Empty;
            //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjJson = JsonConvert.DeserializeObject<JSON_SAVE_QUIZ>(sJsonSaveQuiz);

                    await Task.Run(() =>
                    {
                        if (ObjJson.liOnlineQuiz != null && ObjJson.liOnlineQuiz.Count > 0)
                        {

                            var limatchthefollowing = ObjJson.liOnlineQuiz.Where(o => o.QUESTION_TYPE == Common.Quiz_Question_Type.MatchTheFollowing).ToList();
                            if (limatchthefollowing != null && limatchthefollowing.Count > 0)
                            {
                                sSQL = SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.SaveOnlineQuizMatchTheFollowingByStudent);
                                foreach (var item in limatchthefollowing)
                                {
                                    sSQL += "('" + item.CLASS_ID + "','" + item.COURSE_ID + "','" + item.STUDENT_ID + "','" + item.QUIZ_ID + "','" + item.OPTION_ID + "','" + item.ANSWER_ID + "','" + item.STATUS + "','" + item.SETTING_ID + "','" + item.SET_ID + "','" + item.SEMESTER_ID + "','" + sAcademicYear + "','" + item.MINUTES + "'),";
                                }
                                sSQL = sSQL.TrimEnd(',');
                                sSQL = sSQL + ";";
                                resultArgs = CMSHandler.SaveOrUpdate(null, sSQL, sAcademicYear);
                            }
                            var lifillintheblank = ObjJson.liOnlineQuiz.Where(o => o.QUESTION_TYPE == Common.Quiz_Question_Type.FillInTheBlank).ToList();
                            if (lifillintheblank != null && lifillintheblank.Count > 0)
                            {
                                sSQL = string.Empty;
                                sSQL = SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.SaveFillInTheBlankQuizByStudent);
                                foreach (var item in lifillintheblank)
                                {
                                    sSQL += "('" + item.CLASS_ID + "','" + item.COURSE_ID + "','" + item.STUDENT_ID + "','" + item.QUIZ_ID + "','" + item.OPTION_ID + "','" + item.ANSWER_ID + "','" + item.STATUS + "','" + item.SETTING_ID + "','" + item.SET_ID + "','" + item.SEMESTER_ID + "','" + sAcademicYear + "','" + item.MINUTES + "'),";
                                }
                                sSQL = sSQL.TrimEnd(',');
                                sSQL = sSQL + ";";
                                resultArgs = CMSHandler.SaveOrUpdate(null, sSQL, sAcademicYear);
                            }
                            var liobjectivetype = ObjJson.liOnlineQuiz.Where(o => o.QUESTION_TYPE == Common.Quiz_Question_Type.ObjectiveType).ToList();
                            if (liobjectivetype != null && liobjectivetype.Count > 0)
                            {
                                sSQL = string.Empty;
                                sSQL = SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.SaveObjectiveQuizByStudent);
                                foreach (var item in liobjectivetype)
                                {
                                    sSQL += "('" + item.CLASS_ID + "','" + item.COURSE_ID + "','" + item.STUDENT_ID + "','" + item.QUIZ_ID + "','" + item.OPTION_ID + "','" + item.ANSWER_ID + "','" + item.STATUS + "','" + item.SETTING_ID + "','" + item.SET_ID + "','" + item.SEMESTER_ID + "','" + sAcademicYear + "','" + item.MINUTES + "'),";
                                }
                                sSQL = sSQL.TrimEnd(',');
                                sSQL = sSQL + ";";
                                resultArgs = CMSHandler.SaveOrUpdate(null, sSQL, sAcademicYear);
                            }
                            ObjJsonData.Message = (resultArgs.Success) ? "Answers Submitted Successfully !" : "Failed to Save Answers Try Again !";
                            ObjJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.BadRequest;
                            //Fetch Quiz count status
                            var lifetchquizcountstatus = (List<QUIZ_CLASS_COURSE_STAFF>)CMSHandler.FetchData<QUIZ_CLASS_COURSE_STAFF>(new QUIZ_STUDENT_SHUFFLE() { STUDENT_ID = sUserId, SETTING_ID = sSettingId }, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchQuizcountStatus), sAcademicYear).DataSource.Data;                      
                            ObjJsonData.sResult = lifetchquizcountstatus.FirstOrDefault().COUNT;

                        }
                    });
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Student", "SaveQuizForSetWise", ex.Message);
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
    }
}