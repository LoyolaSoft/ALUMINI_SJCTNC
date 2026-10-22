using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.ViewModel.ViewModel;
using System.Data;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CMS.DAO;
using CMS.SQL.SupportData;
using CMS.SQL.Examination;
using static CMS.Utility.EnumCommand;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Data.Entity.Infrastructure;


//using Rotativa;


namespace CMS.Controllers
{
    public class FeedbackController : Controller
    {
        #region Actions

        #endregion
        ResultArgs resultArgs = new ResultArgs();
        DataValue dv = new DataValue();
        // GET: Feedback
        [UserRights("STUDENT")]
        public async Task<ActionResult> Index()
        {
            try
            {
                FeedbackViewModel objFeedback = new FeedbackViewModel();
                objFeedback = await FeedbackIndexAsync();
                return View(objFeedback);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "Index", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "Index", ex.Message);
                }
                return RedirectToAction("Index", "Account");
            }

        }

        //public ActionResult ExportToPDF()
        //{
        //    ActionAsPdf result = new ActionAsPdf("PrintView")
        //    {
        //        FileName = Server.MapPath("~/PrintedFile/" + Session[Common.SESSION_VARIABLES.USER_CODE] + "/" + DateTime.Now.ToLongDateString())
        //    };
        //    return result;
        //}


        public async Task<FeedbackViewModel> FeedbackIndexAsync()
        {

            string sCourseIds = string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            DataTable dtClasswiseStaffList = new DataTable();
            DataTable dtObjectiveList = new DataTable();
            DataTable dtQuestionList = new DataTable();
            DataTable dtStudentCourseList = new DataTable();
            DataTable dtExistingCourse = new DataTable();
            FeedbackViewModel objFeedback = new FeedbackViewModel();
            List<FBStaffList> StaffList = new List<FBStaffList>();
            List<FBOBJECTIVES> ObjectiveList = new List<FBOBJECTIVES>();
            List<FBFEEDBACK_QUESTIONS> QuestoinList = new List<FBFEEDBACK_QUESTIONS>();
            List<StudentCourseList> StudentList = new List<StudentCourseList>();

            //  objFeedback.StaffList =  (from DataRow dr in dtCourseType.Rows select new SelectListItem { Value = dr["ID"].ToString(), Text = dr["Student"].ToString() });
            //  objFeedback.StaffList=new SelectList()
            // dv.Clear();
            // dv.Add("CLASS_ID", (Session[Common.SESSION_VARIABLES.CLASS_ID] != null) ? Session[Common.SESSION_VARIABLES.CLASS_ID].ToString() : "0", EnumCommand.DataType.String);
            // dtClasswiseStaffList = objFeedback.FetchClassWiseStaffList(dv).DataSource.Table;
            // dtObjectiveList = await Task.Run(() => objFeedback.FetchObjective().DataSource.Table);
            //  dtQuestionList = objFeedback.FetchQuestion().DataSource.Table;
            // dv.Add(Common.STU_PERSONAL_INFO.STUDENT_ID, (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "0", EnumCommand.DataType.String);
            //dtStudentCourseList = await Task.Run(() => objFeedback.FetchStudentCourseListByClassId(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table);
            string sStudentId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            // Fetch Regular Papers .
            var liRegularCourses = (List<EXAM_REGISTRATION>)CMS.DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sStudentId },
                    SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchExamRegistration), sAcademicYear).DataSource.Data;

            // Fetch Optional Papers .
            var liOptionalCourses = await Task.Run(() => (List<EXAM_REGISTRATION>)DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sStudentId },
                         SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data);

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

            objFeedback.StudentCourseList = new SelectList(liRegularCourses, "COURSE_ROOT_ID", "COURSE_TITLE", "---Select---");

            var liFetchObjectives = (List<FBOBJECTIVES>)DAO.CMSHandler.FetchData<FBOBJECTIVES>(new FBOBJECTIVES() { QUESTION_TYPE = Common.FrequencyType.AdmissionFee },
                SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFBObjectives), sAcademicYear).DataSource.Data;

            objFeedback.ObjectiveList = liFetchObjectives;

            // Fetch Course Wise Staff List ..
            var liClassWiseStaff = (List<CourseWiseStaffList>)DAO.CMSHandler.FetchData<CourseWiseStaffList>(new CourseWiseStaffList() { },
                 SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFBClasswiseStaff), sAcademicYear).DataSource.Data;
            //if (dtStudentCourseList != null && dtStudentCourseList.Rows.Count > 0)
            //{
            //    var selectedValues = string.Join(",", dtStudentCourseList.AsEnumerable().Select(s => s.Field<int>("COURSE_ID").ToString()).ToArray());
            //    sCourseIds = string.Join(",", selectedValues);
            //}
            //else
            //    sCourseIds = "0";

            //if (dtStudentCourseList != null && dtStudentCourseList.Rows.Count > 0)
            //{
            //    StudentList = await Task.Run(() => ((from DataRow dr in dtStudentCourseList.Rows
            //                                         select new StudentCourseList()
            //                                         {
            //                                             COURSE_ID = dr["COURSE_ID"].ToString(),
            //                                             COURSE_TITLE = dr["COURSE_TITLE"].ToString()
            //                                         }).ToList()));
            //}
            //else
            //{
            //    //SelectListItem item = new SelectListItem() { Text = "---Select Course---", Value = "0", Selected = true };

            //    //objFeedback.StudentCourseList = new SelectList(item,);
            //}

            //if (dtClasswiseStaffList != null && dtClasswiseStaffList.Rows.Count > 0)
            //{
            //    StaffList = await Task.Run(() => (from DataRow dr in dtClasswiseStaffList.Rows
            //                                      select new FBStaffList()
            //                                      {
            //                                          STAFF_ID = dr["STAFF_ID"].ToString(),
            //                                          STAFF_NAME = dr["STAFF_NAME"].ToString()
            //                                      }).ToList());

            //}
            objFeedback.ClasswiseStaffList = new SelectList(StaffList, "STAFF_ID", "STAFF_NAME", "--Select Staff--");

            if (dtObjectiveList != null && dtObjectiveList.Rows.Count > 0)
            {
                //ObjectiveList = await Task.Run(() => (from DataRow dr in dtObjectiveList.Rows
                //                                      select new FBOBJECTIVES()
                //                                      {
                //                                          OBJECTIVESHORTTERM = dr[Common.FBOBJECTIVES.OBJECTIVESHORTTERM].ToString(),
                //                                          OBJECTIVES = dr[Common.FBOBJECTIVES.OBJECTIVES].ToString(),
                //                                          RATING = dr[Common.FBOBJECTIVES.RATING].ToString(),
                //                                          OBJECTIVE_ID = dr[Common.FBOBJECTIVES.OBJECTIVE_ID].ToString()
                //                                      }
                //             ).ToList());

            }
            if (dtQuestionList != null && dtQuestionList.Rows.Count > 0)
            {
                //QuestoinList = await Task.Run(() => (from DataRow dr in dtQuestionList.Rows
                //                                     select new FBFEEDBACK_QUESTIONS()
                //                                     {
                //                                         QUESTION = dr[Common.FBFEEDBACK_QUESTIONS.QUESTION].ToString(),
                //                                         QUESTION_ID = dr[Common.FBFEEDBACK_QUESTIONS.QUESTION_ID].ToString(),
                //                                         QUESTION_TYPE = dr[Common.FBFEEDBACK_QUESTIONS.QUESTION_TYPE].ToString()

                //                                     }
                //        ).ToList());
                objFeedback.QuestionList = QuestoinList;
            }

            return objFeedback;
        }

        public async Task<ActionResult> QuestionView(string sInputs, string sAssessee)
        {
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                DataTable dtClasswiseStaffList = new DataTable();
                DataTable dtObjectiveList = new DataTable();
                DataTable dtQuestionList = new DataTable();
                DataTable dtExistingCourse = new DataTable();
                List<FBStaffList> StaffList = new List<FBStaffList>();
                List<FBOBJECTIVES> ObjectiveList = new List<FBOBJECTIVES>();
                List<FBFEEDBACK_QUESTIONS> QuestoinList = new List<FBFEEDBACK_QUESTIONS>();


                string str = Session[Common.SESSION_VARIABLES.USER_ROLE_NAME].ToString();
                FeedbackViewModel objFeedback = new FeedbackViewModel();

                dv.Clear();
                dv.Add(Common.FBEVALUATION2017.COURSE_ID, sInputs, EnumCommand.DataType.String);
                dv.Add("ASSESSOR", (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "0", EnumCommand.DataType.String);
                dv.Add(Common.FBEVALUATION2017.ASSESSEE, sAssessee, EnumCommand.DataType.String);
                objFeedback.sValidateStaff = await Task.Run(() => objFeedback.ValidateExisitingCourseList(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").StringResult);
                if (!string.IsNullOrEmpty(objFeedback.sValidateStaff) && objFeedback.sValidateStaff != "1")
                {
                    dv.Clear();
                    // dtClasswiseStaffList = objFeedback.FetchClassWiseStaffList().DataSource.Table;
                    //dtObjectiveList = objFeedback.FetchObjective().DataSource.Table;
                    dv.Clear();
                    dv.Add(Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, sInputs, EnumCommand.DataType.String);
                    dtQuestionList = objFeedback.FetchQuestion(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    ObjectiveList = await Task.Run(() => (from DataRow dr in dtObjectiveList.Rows
                                                          select new FBOBJECTIVES()
                                                          {
                                                              OBJECTIVESHORTTERM = dr[Common.FBOBJECTIVES.OBJECTIVESHORTTERM].ToString(),
                                                              OBJECTIVES = dr[Common.FBOBJECTIVES.OBJECTIVES].ToString(),
                                                              RATING = dr[Common.FBOBJECTIVES.RATING].ToString(),
                                                              OBJECTIVE_ID = dr[Common.FBOBJECTIVES.OBJECTIVE_ID].ToString()
                                                          }
                                 ).ToList());
                    objFeedback.ObjectiveList = ObjectiveList;

                    QuestoinList = await Task.Run(() => (from DataRow dr in dtQuestionList.Rows
                                                         select new FBFEEDBACK_QUESTIONS()
                                                         {
                                                             QUESTION = dr[Common.FBFEEDBACK_QUESTIONS.QUESTION].ToString(),
                                                             QUESTION_ID = dr[Common.FBFEEDBACK_QUESTIONS.QUESTION_ID].ToString(),
                                                             QUESTION_TYPE = dr[Common.FBFEEDBACK_QUESTIONS.QUESTION_TYPE].ToString(),
                                                             QUESTION_GROUP = dr[Common.FBFEEDBACK_QUESTIONS.QUESTION_GROUP].ToString()

                                                         }
                            ).ToList());
                    objFeedback.QuestionList = QuestoinList;
                }
                return View(objFeedback);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "QuestionView", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "QuestionView", ex.Message);
                }
                return RedirectToAction("Index", "Account");
            }

        }
        [HttpPost]
        public ActionResult Save(FormCollection objCollection)
        {
            var ObjViewModel = new FeedbackViewModel();
            try
            {
                resultArgs.Success = SaveFeedbackAsync(objCollection);
                //resultArgs.Success =  SaveFeedbackAsync(objCollection);
                if (resultArgs.Success)
                {
                    ObjViewModel.sErrorMessage = Common.Messages.RecordsSavedSuccessfully;
                    ObjViewModel.sErrorCode = Common.ErrorCode.Ok;
                    //return RedirectToAction("Index", "Feedback");
                }
                else
                {
                    ObjViewModel.sErrorMessage = Common.Messages.FailedToSaveRecords;
                    ObjViewModel.sErrorCode = Common.ErrorCode.NotAcceptable;
                    //TempData["Failed"] = "Failed to Save Records !!!";
                    //return RedirectToAction("Index", "Feedback");
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "Save", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "Save", ex.Message);
                }
                ObjViewModel.sErrorMessage = Common.Messages.FailedToSaveRecords;
                ObjViewModel.sErrorCode = Common.ErrorCode.NotAcceptable;
                //TempData["Failed"] = "Failed to Save Records !!!";
                //return RedirectToAction("Index", "Feedback");
            }
            objCollection = null;
            return View(ObjViewModel);
        }

        public bool SaveFeedbackAsync(FormCollection objCollection)
        {
            bool sResult = false;

            string sSQL = string.Empty;
            string sStudentId = string.Empty;
            string sStaffId = string.Empty;
            string sClassId = string.Empty;
            string sAnswer = string.Empty;
            string sQuestionId = string.Empty;
            string sCourseId = string.Empty;
            DataTable dtStudentInfo = new DataTable();
            FeedbackViewModel objFeedback = new FeedbackViewModel();
            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                sStudentId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "0";
                sCourseId = objCollection["StudentCourseList"].ToString();
                sClassId = (Session[Common.SESSION_VARIABLES.FBCLASS_ID] != null) ? Session[Common.SESSION_VARIABLES.FBCLASS_ID].ToString() : "0";
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                string FeedBackId = string.Empty;
                // Fetch Active FeedBack Setting Id .
                var liFeedBack = (List<FBFEEDBACK_SETTINGS>)DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null,
                      SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedbackInfo), sAcademicYear).DataSource.Data;
                if (liFeedBack != null && liFeedBack.Count > 0)
                {
                    FeedBackId = liFeedBack.FirstOrDefault().FEEDBACK_ID;
                }
                sSQL = @"INSERT INTO FBEVALUATION" + sAcademicYear + "(ASSESSOR,CLASS_ID,ASSESSEE,COURSE_ID,FEEDBACK_ID,QUESTION,ANSWER)VALUES";
                foreach (var item in objCollection.Keys)
                {
                    if (item.ToString() != "ClasswiseStaffList" && item.ToString() != "__RequestVerificationToken" && item.ToString() != "StudentCourseList")
                    {
                        sSQL += "('" + sStudentId + "','" + sClassId + "','" + objCollection["ClasswiseStaffList"] + "','" + sCourseId + "','" + FeedBackId + "','" + item.ToString() + "','" + objCollection[item.ToString()] + "'),";
                    }

                }
                sSQL = sSQL.Trim(',') + ";";
                // sSQL = sSQL.Replace(Common.Delimiter.QUS+Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                //  resultArgs = await Task.Run(() => objFeedback.SaveFeedback(sSQL));
                resultArgs = objFeedback.SaveFeedback(sSQL);
                sResult = resultArgs.Success;

            }


            return sResult;

        }

        public async Task<ActionResult> Result()
        {
            try
            {
                FeedbackViewModel objFeedback = new FeedbackViewModel();
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                {
                    List<FBCLASSES_BY_STAFF> iFbClass = new List<FBCLASSES_BY_STAFF>();
                    List<StudentCourseList> iFbList = new List<StudentCourseList>();
                    DataTable dtCourse = new DataTable();
                    DataTable dtClass = new DataTable();
                    dv.Clear();
                    dv.Add(Common.FBCLASS_WISE_STAFF2017.STAFF_ID, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);
                    string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    var liClassList = await Task.Run(() => (List<CP_CLASSES>)DAO.CMSHandler.FetchData<CP_CLASSES>(new CourseWiseStaffList() { STAFF_ID = sUserId },
                        SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchClassList), sAcademicYear).DataSource.Data);
                    //dtClass = await Task.Run(() => objFeedback.FetchFBClassesByStaffId(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table);
                    // dtCourse = await Task.Run(() => objFeedback.FetchCourseListByStaffId(dv).DataSource.Table);
                    //  iFbList = await Task.Run(() => (from DataRow dr in dtCourse.Rows select new StudentCourseList() { COURSE_ID = dr["COURSE_ID"].ToString(), COURSE_TITLE = dr["COURSE_TITLE"].ToString() }).ToList());
                    iFbClass = await Task.Run(() => (from DataRow dr in dtClass.Rows select new FBCLASSES_BY_STAFF() { CLASS_ID = dr["CLASS_ID"].ToString(), CLASS_DESCRIPTION = dr["CLASS_DESCRIPTION"].ToString() }).ToList());
                    iFbList.Add(new StudentCourseList { COURSE_ID = "0", COURSE_TITLE = "---Select---" });
                    if (iFbList != null)
                    {
                        objFeedback.CourseList = new SelectList(iFbList, "COURSE_ID", "COURSE_TITLE");

                    }
                    if (liClassList != null)
                    {
                        objFeedback.ClassList = new SelectList(liClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);

                    }
                }
                else
                {

                }
                return View(objFeedback);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "Result", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "Result", ex.Message);
                }
                return RedirectToAction("Index", "Account");
            }

        }

        public ActionResult PrintView()
        {
            return View("Hello world......");
        }

        public async Task<ActionResult> LoadFeedbackResult(string CourseId, string ClassId)
        {
            FeedbackViewModel objFeedback = new FeedbackViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                {
                    Session[Common.SESSION_VARIABLES.COURSE_ID] = CourseId;
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    List<FBRATING_BY_STAFF> rating = new List<FBRATING_BY_STAFF>();
                    DataTable dtCourseClassInfo = new DataTable();
                    DataTable dtStaffRatting = new DataTable();
                    DataTable dtFeedbackSetting = new DataTable();

                    dv.Clear();
                    dv.Add(Common.FBEVALUATION2017.COURSE_ID, CourseId, EnumCommand.DataType.String);
                    dv.Add(Common.FBCLASS_WISE_STAFF2017.STAFF_ID, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);
                    dv.Add(Common.FBEVALUATION2017.CLASS_ID, ClassId, EnumCommand.DataType.String);

                    //dtCourseClassInfo = await Task.Run(() => objFeedback.FetchFBClassCourseInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table);
                    dtFeedbackSetting = await Task.Run(() => objFeedback.FetchFBSettingInfo().DataSource.Table);
                    dv.Clear();
                    dv.Add(Common.FBEVALUATION2017.COURSE_ID, CourseId, EnumCommand.DataType.String);
                    dv.Add(Common.FBEVALUATION2017.ASSESSEE, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);
                    dv.Add(Common.FBEVALUATION2017.CLASS_ID, ClassId, EnumCommand.DataType.String);
                    //dtStaffRatting = await Task.Run(() => objFeedback.FetchFBResultByCourseId(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table);
                    dtStaffRatting = await Task.Run(() => objFeedback.FetchFBResultByCourseIdandRatingWise(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table);
                    if (dtFeedbackSetting != null && dtFeedbackSetting.Rows.Count > 0)
                    {
                        objFeedback.objFBSetting = new FBFEEDBACK_SETTINGS()
                        {
                            FEEDBACK_NAME = dtFeedbackSetting.Rows[0][Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME].ToString(),
                            FEEDBACK_ID = dtFeedbackSetting.Rows[0][Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID].ToString()
                        ,
                            ACADEMIC_YEAR = dtFeedbackSetting.Rows[0][Common.FBFEEDBACK_SETTINGS.ACADEMIC_YEAR].ToString()
                        };

                    }
                    if (dtCourseClassInfo != null && dtCourseClassInfo.Rows.Count > 0)
                    {
                        objFeedback.objCourseAndClassInfo = new FBCOURSE_AND_CLASS_INFO()
                        {
                            CLASS_NAME = dtCourseClassInfo.Rows[0][Common.COURSE_AND_COURSE_INFO_BY_STAFF_ID.CLASS_NAME].ToString(),
                            COURSE_CODE = dtCourseClassInfo.Rows[0][Common.COURSE_AND_COURSE_INFO_BY_STAFF_ID.COURSE_CODE].ToString(),
                            COURSE_TITLE = dtCourseClassInfo.Rows[0][Common.COURSE_AND_COURSE_INFO_BY_STAFF_ID.COURSE_TITLE].ToString(),
                            COURSE_TYPE = dtCourseClassInfo.Rows[0][Common.COURSE_AND_COURSE_INFO_BY_STAFF_ID.COURSE_TYPE].ToString(),
                            DEPARTMENT = dtCourseClassInfo.Rows[0][Common.COURSE_AND_COURSE_INFO_BY_STAFF_ID.DEPARTMENT].ToString(),
                            TOTAL_STUDENTS = dtCourseClassInfo.Rows[0][Common.COURSE_AND_COURSE_INFO_BY_STAFF_ID.TOTAL_STUDENTS].ToString()
                        };
                    }
                    if (dtStaffRatting != null && dtStaffRatting.Rows.Count > 0)
                    {

                        rating = await Task.Run(() => (from DataRow dr in dtStaffRatting.Rows
                                                       select new FBRATING_BY_STAFF()
                                                       {
                                                           ASSESSOR = dr[Common.FBRATING_BY_STAFF_BY_COURSE_ID.ASSESSOR].ToString(),
                                                           OBJECTIVE_ID = dr[Common.FBRATING_BY_STAFF_BY_COURSE_ID.OBJECTIVE_ID].ToString(),
                                                           QUESTION_ID = dr[Common.FBRATING_BY_STAFF_BY_COURSE_ID.QUESTION_ID].ToString(),
                                                           RATING = dr[Common.FBRATING_BY_STAFF_BY_COURSE_ID.RATING].ToString(),
                                                           GROUP_TITLE = dr[Common.FBRATING_BY_STAFF_BY_COURSE_ID.GROUP_TITLE].ToString(),
                                                           OBJECTIVES = dr[Common.FBRATING_BY_STAFF_BY_COURSE_ID.OBJECTIVES].ToString(),
                                                           QUESTION = dr[Common.FBRATING_BY_STAFF_BY_COURSE_ID.QUESTION].ToString(),
                                                       }).ToList());


                    }
                    objFeedback.LiFeedbackQuestionMapping = (List<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING>)CMSHandler.FetchData<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING>(objFeedback.objFBSetting, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbFeedbackQuestionsMappingByFeedbackId), sAcademicYear).DataSource.Data;

                    objFeedback.objRating = rating;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "LoadFeedbackResult", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "LoadFeedbackResult", ex.Message);
                }
            }

            return View(objFeedback);
        }

        public ActionResult StaffRating(string sStaffId)
        {
            try
            {
                FeedbackViewModel objFeedbackViewModel = new FeedbackViewModel();
                DataTable dtRattingByStaffId = new DataTable();
                List<FBRATING_BY_STAFF> FBRatingList = new List<FBRATING_BY_STAFF>();
                DataValue dv = new DataValue();

                dv.Add(Common.FBEVALUATION.ASSESSEE, sStaffId, EnumCommand.DataType.String);

                dtRattingByStaffId = new FeedbackViewModel().FetchOverallRatingByStaffId(dv).DataSource.Table;
                if (dtRattingByStaffId != null && dtRattingByStaffId.Rows.Count > 0)
                {
                    //FBRatingList = (from DataRow dr in dtRattingByStaffId.Rows
                    //                select new FBRATING_BY_STAFF()
                    //                {
                    //                    ASSESSOR = dr[Common.FBRATING_BY_STAFF.ASSESSOR].ToString(),
                    //                    CLASS_CODE = dr[Common.FBRATING_BY_STAFF.CLASS_CODE].ToString(),
                    //                    FIRST_NAME = dr[Common.FBRATING_BY_STAFF.FIRST_NAME].ToString(),
                    //                    OBJECTIVES_COUNT = dr[Common.FBRATING_BY_STAFF.OBJECTIVES_COUNT].ToString(),
                    //                    OBJECTIVES = dr[Common.FBRATING_BY_STAFF.OBJECTIVES].ToString(),
                    //                    STUDENT_COUNT = dr[Common.FBRATING_BY_STAFF.STUDENT_COUNT].ToString(),
                    //                    QUESTION = dr[Common.FBRATING_BY_STAFF.QUESTION].ToString()

                    //                }
                    //    ).ToList();
                    objFeedbackViewModel.StaffwiseRatingList = FBRatingList;
                }

            }
            catch (Exception ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "StaffRating", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "StaffRating", ex.Message);
                }
            }

            return View();
        }

        public ActionResult PrintOptions()
        {
            return View();
        }

        #region PrincipalView
        [UserRights("PRINCIPAL")]
        public async Task<ActionResult> FeedbackPricipalView()
        {
            FeedbackViewModel objFeedback = new FeedbackViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                {
                    List<Department> objDepartment = new List<Department>();
                    List<FBStaffList> objStaffList = new List<FBStaffList>();
                    DataTable dtDepartmentList = new DataTable();


                    dtDepartmentList = await Task.Run(() => objFeedback.FetchDepartmentList((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table);
                    if (dtDepartmentList != null && dtDepartmentList.Rows.Count > 0)
                    {
                        objDepartment = await Task.Run(() => (from DataRow dr in dtDepartmentList.Rows
                                                              select new Department()
                                                              {
                                                                  DEPARTMENT = dr[Common.CP_DEPARTMENT_2017.DEPARTMENT].ToString(),
                                                                  DEPARTMENT_ID = dr[Common.CP_DEPARTMENT_2017.DEPARTMENT_ID].ToString()
                                                              }).ToList());
                        ;
                    }
                    else
                    {
                        objDepartment.Add(new Department() { DEPARTMENT = "---Select---", DEPARTMENT_ID = "0" });
                    }

                    objFeedback.DepartmentList = new SelectList(objDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    objFeedback.StaffListByDepartmet = new SelectList(objStaffList, Common.STF_PERSONAL_INFO.STAFF_ID, "STAFF_NAME", "---Select---");
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FeedbackPricipalView", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FeedbackPricipalView", ex.Message);
                }
            }

            return View(objFeedback);
        }
        [UserRights("PRINCIPAL")]
        public async Task<ActionResult> FeedbackPricipalViewForDepartment()
        {
            FeedbackViewModel objFeedback = new FeedbackViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                {
                    List<Department> objDepartment = new List<Department>();
                    List<Sup_Shift> objShift = new List<Sup_Shift>();

                    DataTable dtDepartmentList = new DataTable();
                    DataTable dtShift = new DataTable();


                    dtShift = await Task.Run(() => objFeedback.FetchShiftList().DataSource.Table);
                    if (dtShift != null && dtShift.Rows.Count > 0)
                    {
                        objShift = await Task.Run(() => (from DataRow dr in dtShift.Rows
                                                         select new Sup_Shift()
                                                         {
                                                             SHIFT_ID = dr[Common.SUP_SHIFT.SHIFT_ID].ToString(),
                                                             SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString()
                                                         }).ToList());
                        ;
                    }
                    else
                    {
                        objShift.Add(new Sup_Shift() { SHIFT_ID = "---Select---", SHIFT_NAME = "0" });
                    }

                    objFeedback.ShiftList = new SelectList(objShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    objFeedback.DepartmentList = new SelectList(objDepartment, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME, "---Select---");
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FeedbackPricipalViewForDepartment", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FeedbackPricipalViewForDepartment", ex.Message);
                }
            }

            return View(objFeedback);
        }

        #endregion

        public async Task<string> StaffListBySubjectId(string sCourseId)
        {
            List<FBStaffList> StaffList = new List<FBStaffList>();
            DataTable dtStaffList = new DataTable();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            FeedbackViewModel objFeedbackViewModel = new FeedbackViewModel();
            string option = "<option value='0' >---Select Staff---</option>";
            try
            {
                dv.Clear();
                dv.Add("COURSE_ID", sCourseId, EnumCommand.DataType.String);
                dv.Add(Common.STU_PERSONAL_INFO.STUDENT_ID, (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "0", EnumCommand.DataType.String);
                resultArgs = await Task.Run(() => objFeedbackViewModel.FetchCourseWiseStaffList(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : ""));
                dtStaffList = resultArgs.DataSource.Table;
                if (dtStaffList != null && dtStaffList.Rows.Count > 0)
                {
                    Session[Common.SESSION_VARIABLES.FBCLASS_ID] = dtStaffList.Rows[0]["CLASS_ID"].ToString();
                }


                foreach (DataRow item in dtStaffList.Rows)
                {
                    option += "<option value='" + item["STAFF_ID"].ToString() + "' cms-class-id='" + item["CLASS_ID"].ToString() + "' >" + item["STAFF_NAME"] + "</option>";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "StaffListBySubjectId", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "StaffListBySubjectId", ex.Message);
                }
            }

            return option;
        }
        public string StaffListByDepartment(string sDepartmentId)
        {
            List<FBStaffList> StaffList = new List<FBStaffList>();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objFeedbackViewModel = new FeedbackViewModel();
            string option = string.Empty;
            try
            {

                var dtStaffList = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new cp_Department_2017() { DEPARTMENT_ID = sDepartmentId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStaffListByDepartment), sAcademicYear).DataSource.Data;

                //string option = "<option value='0' >---Select Staff---</option>";
                if (dtStaffList != null && dtStaffList.Count > 0)
                {
                    foreach (var item in dtStaffList)
                    {
                        option += "<option value='" + item.STAFF_ID + "' >" + item.FIRST_NAME + "</option>";
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "StaffListByDepartment", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "StaffListByDepartment", ex.Message);
                }
            }

            return option;
        }

        public async Task<string> DepartmentListByShift(string sShiftId)
        {
            List<Department> objDepartment = new List<Department>();
            DataTable dtDepartmentList = new DataTable();
            FeedbackViewModel objFeedbackViewModel = new FeedbackViewModel();
            string option = string.Empty;
            try
            {
                dv.Clear();
                dv.Add(Common.STF_PERSONAL_INFO.SHIFT, sShiftId, EnumCommand.DataType.String);
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                dtDepartmentList = await Task.Run(() => objFeedbackViewModel.FetchDepartmentByShiftId(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table);
                if (dtDepartmentList != null && dtDepartmentList.Rows.Count > 0)
                {
                    foreach (DataRow item in dtDepartmentList.Rows)
                    {
                        option += "<option value='" + item[Common.CP_DEPARTMENT_2017.DEPARTMENT_ID].ToString() + "' >" + item[Common.CP_DEPARTMENT_2017.DEPARTMENT] + "</option>";
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "DepartmentListByShift", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "DepartmentListByShift", ex.Message);
                }
            }

            return option;
        }

        public async Task<string> GetCourseListByClassId(string sClassId)
        {
            string option = string.Empty;
            DataTable dtCourseList = new DataTable();
            DataTable dtClassList = new DataTable();

            using (ExaminationViewModel objExamination = new ExaminationViewModel())
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objExamination.dv.Clear();
                    objExamination.dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.STAFF_ID, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);
                    objExamination.dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.CLASS_ID, sClassId, EnumCommand.DataType.String);
                    dtCourseList = await Task.Run(() => objExamination.FetchCourseList((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table);

                    foreach (DataRow item in dtCourseList.Rows)
                    {
                        option += "<option value='" + item[Common.CP_CLASS_COURSE_2017.COURSE_ID].ToString() + "' >" + item[Common.CP_COURSE_ROOT_2017.COURSE_TITLE].ToString() + "</option>";
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FeedbackController", "GetCourseListByClassId", "Err MSg: " + ex.Message, "");
                        objHandler.WriteError("FeedbackController", "GetCourseListByClassId", ex.Message);
                    }

                }
                return option;

            }


        }

        #region Examination FeedBack Form
        // Examiner FeedBack ....
        public ActionResult ExaminersFeedBack()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            var ObjModel = new FBFEEDBACK_SETTINGS();
            var Objectives = new FBOBJECTIVES();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                // Fetch Feed Back Setting Id ...
                string sFeedBackId = string.Empty;
                var liSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null,
                      SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchExaminersActiveFeedBack), sAcademicYear).DataSource.Data;
                if (liSetting != null && liSetting.Count > 0)
                {
                    sFeedBackId = liSetting.FirstOrDefault().FEEDBACK_ID;
                    ObjModel.FEEDBACK_ID = sFeedBackId;
                }

                Objectives.QUESTION_TYPE = "2";
                // Fetch Objectives ...
                ObjViewModel.ObjectiveList = (List<FBOBJECTIVES>)CMS.DAO.CMSHandler.FetchData<FBOBJECTIVES>(Objectives,
                   SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFBObjectives), sAcademicYear).DataSource.Data;

                // Validate Examiner FeedBack ...
                string sResult = ValidateExaminersFeedBack(sAcademicYear, sUserId, sFeedBackId);
                if (sResult == "1")
                {
                    // Fetch Questions By FeedBackId ....
                    ObjViewModel.QuestionList = (List<FBFEEDBACK_QUESTIONS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel,
                        SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchExaminersQuestionList), sAcademicYear).DataSource.Data;
                }
                else
                {
                    ObjViewModel.sErrorMessage = sResult;
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        // Save Examiner FeedBack ...
        [HttpPost]
        public ActionResult SaveExaminersFeedBack(FormCollection objCollection)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (SaveExaminersFeedBackMethod(ref objCollection))
                    {
                        TempData["Success"] = Common.Messages.RecordsSavedSuccessfully;
                    }
                    else
                    {
                        TempData["Failed"] = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("FeedBack", "SaveExaminersFeedBack", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("ExaminersFeedBack", "FeedBack");
        }

        // Save Method For Examiner Feedback ...
        public bool SaveExaminersFeedBackMethod(ref FormCollection objCollection)
        {
            bool sResult = false;

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sFeedBackId = string.Empty;
                    var liSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null,
                          SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchExaminersActiveFeedBack), sAcademicYear).DataSource.Data;
                    if (liSetting != null && liSetting.Count > 0)
                    {
                        sFeedBackId = liSetting.FirstOrDefault().FEEDBACK_ID;
                    }

                    string sSQL = @"INSERT INTO FB_EXAMINERS_EVALUATION_" + sAcademicYear + "(EXAMINERS_ID,FEEDBACK_ID,QUESTION,ANSWER)VALUES";
                    foreach (var item in objCollection.Keys)
                    {
                        if (item.ToString() != "__RequestVerificationToken")
                        {
                            sSQL += "('" + sUserId + "','" + sFeedBackId + "','" + item.ToString() + "','" + objCollection[item.ToString()] + "'),";
                        }

                    }
                    sSQL = sSQL.Trim(',') + ";";
                    // sSQL = sSQL.Replace(Common.Delimiter.QUS+Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                    //  resultArgs = await Task.Run(() => objFeedback.SaveFeedback(sSQL));
                    resultArgs = ObjViewModel.SaveFeedback(sSQL);
                    sResult = resultArgs.Success;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("FeedBack", "SaveExaminersFeedBackMethod", ex.Message);
                    }
                }
            }

            return sResult;
        }

        // Validate Examiner FeedBack ....
        public string ValidateExaminersFeedBack(string sAcademicYear, string sUserId, string sFeedBackId)
        {
            string sResult = string.Empty;
            var ObjModel = new FB_EXAMINERS_EVALUATION_2018();
            if (!string.IsNullOrEmpty(sFeedBackId))
            {
                ObjModel.FEEDBACK_ID = sFeedBackId;
                ObjModel.EXAMINERS_ID = sUserId;
                var liValidate = (List<FB_EXAMINERS_EVALUATION_2018>)CMS.DAO.CMSHandler.FetchData<FB_EXAMINERS_EVALUATION_2018>(ObjModel,
                    SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.ValidateExaminersIsExist), sAcademicYear).DataSource.Data;
                if (liValidate != null && liValidate.Count > 0)
                {
                    sResult = "You have given feedback to this semester !";
                }
                else
                {
                    sResult = "1";
                }
            }
            else
            {
                sResult = "Please Meet Admin ! Your FeedBack Not Available !";
            }
            return sResult;
        }

        // Examiner FeedBack Result ...
        public ActionResult ExaminerFeedBackResult()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            var ObjModel = new FBFEEDBACK_SETTINGS();
            var Objectives = new FBOBJECTIVES();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Fetch Feed Back Setting Id ...
                    string sFeedBackId = string.Empty;
                    var liSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null,
                          SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchExaminersActiveFeedBack), sAcademicYear).DataSource.Data;
                    if (liSetting != null && liSetting.Count > 0)
                    {
                        sFeedBackId = liSetting.FirstOrDefault().FEEDBACK_ID;
                        ObjModel.FEEDBACK_ID = sFeedBackId;
                    }

                    Objectives.QUESTION_TYPE = "2";
                    // Fetch Objectives ...
                    ObjViewModel.ObjectiveList = (List<FBOBJECTIVES>)CMS.DAO.CMSHandler.FetchData<FBOBJECTIVES>(Objectives,
                       SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFBObjectives), sAcademicYear).DataSource.Data;
                    ObjViewModel.objRating = (List<FBRATING_BY_STAFF>)CMS.DAO.CMSHandler.FetchData<FBRATING_BY_STAFF>(ObjModel,
                       SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.ResultForExaminersFeedack), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("FeedBack", "ExaminerFeedBackResult", ex.Message);
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

        #region Feedback setting

        /// <summary>
        ///  this is for adding and editing feedback settings
        /// </summary>
        /// <returns></returns>
        /// 

        public ActionResult AcademicInfo()
        {

            FeedbackViewModel objViewModel = new FeedbackViewModel();
            var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
                    SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
            if (liAcYear != null && liAcYear.Count > 0)
            {
                objViewModel.AcademicYearList = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
            }

            return View(objViewModel);
        }

        public JsonResult ChangeAcademicInfo(string sAcYear)
        {
            JsonResultData objResult = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcYear))
            {
                Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] = sAcYear;

                if (Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString().Equals(sAcYear))
                {
                    objResult.Message = "Academic Year " + Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] + " was Changed Successfully!";
                    objResult.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    objResult.Message = "Academic Year Not Changed!";
                }
            }
            else
            {
                objResult.Message = "Internal error!";
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        [UserRights("Admin")]
        public ActionResult ListFeedbackSetting()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                try
                {

                    FeedbackViewModel.sAcademicAttempt = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? 0 : FeedbackViewModel.sAcademicAttempt + 1;

                    //// question type
                    //var FbQuestionType = (List<SUP_FBFEEDBACK_QUESTION_TYPE>)CMS.DAO.CMSHandler.FetchData<SUP_FBFEEDBACK_QUESTION_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchFeedBackQuestionType), sAcademicYear).DataSource.Data;
                    //if (FbQuestionType != null && FbQuestionType.Count > 0)
                    //{
                    //    objViewModel.QuestionTypeList = new SelectList(FbQuestionType, Common.SUP_FBFEEDBACK_QUESTION_TYPE.QUESTION_TYPE_ID, Common.SUP_FBFEEDBACK_QUESTION_TYPE.QUESTION_TYPE);
                    //}

                    // question group list
                    // var QuestionGroup = (List<FBQUESTIONS_GROUP>)CMS.DAO.CMSHandler.FetchData<FBQUESTIONS_GROUP>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchFeedBackQuestionGroup), sAcademicYear).DataSource.Data;
                    //if (QuestionGroup != null && QuestionGroup.Count > 0)
                    //{
                    //  objViewModel.sQuestionGroupList = new SelectList(QuestionGroup, Common.FBQUESTIONS_GROUP.QUESTION_GROUP_ID, Common.FBQUESTIONS_GROUP.GROUP_TITLE);
                    //}
                    // fetch setting 
                    objViewModel.liFbSettings = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(new FBFEEDBACK_SETTINGS() { AC_YEAR = sAcademicYear }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedBackSettings), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "ListFeedbackSetting", ex.Message);
                    }
                }

            }
            return View(objViewModel);
        }

        public JsonResult FetchLiFeedBackSettingById(string sFeedBackId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sFeedBackId))
                {
                    // fetch setting 
                    objResultData.oResult = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(new FBFEEDBACK_SETTINGS() { FEEDBACK_ID = sFeedBackId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedBackSettingsById), sAcademicYear).DataSource.Data;
                }
            }
            return Json(objResultData);
        }

        public JsonResult SaveorUpdateFeedBackSetting(string sJsonData)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<FBFEEDBACK_SETTINGS>(sJsonData);
                    if (result != null)
                    {
                        // formating date
                        if (!string.IsNullOrEmpty(result.DATE_FROM))
                        {
                            result.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(result.DATE_FROM);
                        }
                        if (!string.IsNullOrEmpty(result.DATE_TO))
                        {
                            result.DATE_TO = CommonMethods.ConvertdatetoyearFromat(result.DATE_TO);
                        }

                        if (result.FEEDBACK_ID != null && result.FEEDBACK_ID != "")
                        {
                            // UPDATE setting 
                            resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.UpdateFeedBackSetting), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            // ADD setting 
                            resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.SaveFeedBackSetting), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "SaveorUpdateFeedBackSetting", ex.Message);
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
        public JsonResult DeleteLiFeedBackSettingById(string sFeedBackId)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sFeedBackId))
                    {
                        // fetch setting 
                        resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new FBFEEDBACK_SETTINGS() { FEEDBACK_ID = sFeedBackId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.DeleteFeedBackSettingById), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "DeleteLiFeedBackSettingById", ex.Message);
                    }
                }

            }
            return Json(objResultData);
        }

        public JsonResult ActivateorDeactivateFeedBackSettingBySettingId(string sFeedBackId, string sMode)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                if (!string.IsNullOrEmpty(sFeedBackId) && !string.IsNullOrEmpty(sMode))
                {
                    // fetch setting 
                    resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new FBFEEDBACK_SETTINGS() { FEEDBACK_ID = sFeedBackId, IS_ACTIVE = sMode, AC_YEAR = sAcademicYear }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.ActivateorDeactivateFeedBackSettingById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                }
            }
            return Json(objResultData);
        }
        #endregion

        #region Question Group
        [UserRights("Admin")]
        //adding question group for the feedback
        public ActionResult ListFeedbackQuestionGroup(string sFeedbackId)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                FeedbackViewModel.sAcademicAttempt = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? 0 : FeedbackViewModel.sAcademicAttempt + 1;


                // feed back
                var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                if (fbSetting != null && fbSetting.Count > 0)
                {
                    objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                }
                if (!string.IsNullOrEmpty(sFeedbackId))
                {
                    objViewModel.sFeedBackId = sFeedbackId;
                }
            }
            return View(objViewModel);
        }

        public ActionResult BindFeedbackQuestionGroupByFeedbackId(string sFeedBackId)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sFeedBackId))
            {
                try
                {

                    // feed back question group
                    var fbQuestionGroup = (List<FBQUESTIONS_GROUP>)CMSHandler.FetchData<FBQUESTIONS_GROUP>(new FBQUESTIONS_GROUP() { FEEDBACK_ID = sFeedBackId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbQuestionsGroupByFeedbackId), sAcademicYear).DataSource.Data;
                    if (fbQuestionGroup != null && fbQuestionGroup.Count > 0)
                    {
                        objViewModel.liFbQuestionGroup = fbQuestionGroup;
                    }
                    else
                    {
                        objViewModel.sErrorMessage = Common.Messages.NoRecordsfound;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FeedbackController", "BindFeedbackQuestionGroupByFeedbackId", "Err MSg: " + ex.Message);

                    }
                }

            }
            return View(objViewModel);
        }

        public JsonResult DeleteFeedBackQuestionGroup(string sFeedbackQuestionGroup)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sFeedbackQuestionGroup))
            {
                // fetch setting 
                resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new FBQUESTIONS_GROUP() { QUESTION_GROUP_ID = sFeedbackQuestionGroup, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.DeletedFbQuestionGroupByFBQuestionGroupId), sAcademicYear);
                objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
            }
            return Json(objResultData);

        }

        public ActionResult AddandUpdateFeedBackQuestionGroup(string sFeedbackId, string sFeedbackQuestionGroup)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sFeedbackId))
            {

                try
                {
                    objViewModel.sFeedBackId = sFeedbackId;
                    objViewModel.sFeedBackQuestionGroup = sFeedbackQuestionGroup;

                    // feed back setting
                    var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                    if (fbSetting != null && fbSetting.Count > 0)
                    {
                        objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                    }

                    //// feed back question group
                    //var fbQuestionGroup = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                    //if (fbQuestionGroup != null && fbQuestionGroup.Count > 0)
                    //{
                    //    objViewModel.sQuestionGroupList = new SelectList(Common.FBQUESTIONS_GROUP.QUESTION_GROUP_ID, Common.FBQUESTIONS_GROUP.GROUP_TITLE);
                    //}


                    if (!String.IsNullOrEmpty(sFeedbackQuestionGroup))
                    {
                        objViewModel.liFbQuestionGroup = (List<FBQUESTIONS_GROUP>)CMSHandler.FetchData<FBQUESTIONS_GROUP>(new FBQUESTIONS_GROUP() { QUESTION_GROUP_ID = sFeedbackQuestionGroup }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionGroupByQuestionGroupId), sAcademicYear).DataSource.Data;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "AddandUpdateFeedBackQuestionGroup", ex.Message);
                    }
                }

            }
            return View(objViewModel);

        }
        public JsonResult SaveFeedBackQuestionGroup(string sJsonData)
        {
            JsonResultData objJsonData = new JsonResultData();
            string query = string.Empty;
            int Qcount = 0;

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            // string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    var Result1 = JsonConvert.DeserializeObject<JSON_SAVE_QUESTION>(sJsonData);
                    query = @"INSERT INTO FBQUESTIONS_GROUP(GROUP_TITLE,FEEDBACK_ID,ACADEMIC_YEAR) VALUES";

                    foreach (var item in Result1.liFB_QuestionGroup)
                    {
                        if (item.QUESTION_GROUP_ID != "")
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(new FBQUESTIONS_GROUP()
                            {
                                QUESTION_GROUP_ID = item.QUESTION_GROUP_ID,
                                GROUP_TITLE = item.GROUP_TITLE,
                                FEEDBACK_ID = item.FEEDBACK_ID
                            }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.UpdateFbQuestionGroupByGroupId), sAcademicYear);
                            objJsonData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            query += "('" + item.GROUP_TITLE + "','" + item.FEEDBACK_ID + "','?AC_YEAR'),";
                            Qcount++;
                        }
                    }
                    if (Qcount > 0)
                    {
                        query = query.Trim(',') + ";";
                        resultArgs = CMSHandler.SaveOrUpdate(null, query, sAcademicYear);
                        objJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "SaveFeedBackQuestionGroup", ex.Message);
                    }
                }
            }
            else
            {
                objJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objJsonData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(objJsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region FeedBack  Role Assigning
        [UserRights("Admin")]
        public ActionResult ListFeedbackRoleSetting()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                FeedbackViewModel.sAcademicAttempt = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? 0 : FeedbackViewModel.sAcademicAttempt + 1;

                // feed back
                var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                if (fbSetting != null && fbSetting.Count > 0)
                {
                    objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                }

                // sup feed back role
                var fbFeedBAckRole = (List<SUP_FBFEEDBACK_ROLE>)CMS.DAO.CMSHandler.FetchData<SUP_FBFEEDBACK_ROLE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchFeedBackRole), sAcademicYear).DataSource.Data;
                if (fbFeedBAckRole != null && fbFeedBAckRole.Count > 0)
                {
                    objViewModel.FeedBackRoleList = new SelectList(fbFeedBAckRole, Common.SUP_FBFEEDBACK_ROLE.SUP_FEEDBACK_ROLE_ID, Common.SUP_FBFEEDBACK_ROLE.FEEDBACK_ROLE_NAME);
                }
                // fetch setting 
                objViewModel.liFbRoleSettings = (List<FBFEEDBACK_ROLE_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_ROLE_SETTINGS>(null, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedBackRoleSettings), sAcademicYear).DataSource.Data;
            }
            return View(objViewModel);
        }

        public JsonResult FetchLiFeedBackRoleSettingById(string sFeedBackRoleId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sFeedBackRoleId))
                {
                    // fetch setting 
                    objResultData.oResult = (List<FBFEEDBACK_ROLE_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_ROLE_SETTINGS>(new FBFEEDBACK_ROLE_SETTINGS() { FEEDBACK_ROLE_ID = sFeedBackRoleId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedBackRoleSettingsById), sAcademicYear).DataSource.Data;
                }
            }
            return Json(objResultData);
        }

        public JsonResult SaveorUpdateFeedBackRoleSetting(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<FBFEEDBACK_ROLE_SETTINGS>(sJsonData);
                    if (result != null)
                    {
                        if (result.FEEDBACK_ROLE_ID != null && result.FEEDBACK_ROLE_ID != "")
                        {
                            // UPDATE setting 
                            resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.UpdateFeedBackRoleSetting), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            // ADD setting 

                            // validate role setting if its existed
                            var IsExist = (List<FBFEEDBACK_ROLE_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_ROLE_SETTINGS>(new FBFEEDBACK_ROLE_SETTINGS() { FEEDBACK_ID = result.FEEDBACK_ID, SUP_FEEDBACK_ROLE_ID = result.SUP_FEEDBACK_ROLE_ID }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedBackRoleSettingByFeedBackandSupFeedBackRoleId), sAcademicYear).DataSource.Data;
                            if (IsExist != null && IsExist.Count > 0)
                            {
                                objResultData.Message = Common.Messages.RecordsExistAlready;
                                objResultData.ErrorCode = Common.ErrorCode.AlreadyReported;
                            }
                            else
                            {
                                resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.SaveFeedBackRoleSetting), sAcademicYear);
                                objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FeedbackController", "SaveorUpdateFeedBackRoleSetting", "Err MSg: " + ex.Message);

                    }
                }


            }
            return Json(objResultData);
        }

        public JsonResult DeleteLiFeedBackRoleSettingById(string sFeedBackRoleId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                if (!string.IsNullOrEmpty(sFeedBackRoleId))
                {
                    // delete role setting 
                    resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new FBFEEDBACK_ROLE_SETTINGS() { FEEDBACK_ROLE_ID = sFeedBackRoleId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.DeleteFeedBackRoleSettingById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
            }
            return Json(objResultData);
        }

        #endregion

        #region Feedback Faculty Performance Index
        [UserRights("Admin")]
        public ActionResult FacultyPerformanceIndex()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            try
            {
                FeedbackViewModel.sAcademicAttempt = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? 0 : FeedbackViewModel.sAcademicAttempt + 1;


                // feed back
                var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                if (fbSetting != null && fbSetting.Count > 0)
                {
                    objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                }
                // feedback faculty performance index
                var fbFacyltyPerformanceIndex = (List<FBFACULTY_PERFORMANCE_INDEX>)CMS.DAO.CMSHandler.FetchData<FBFACULTY_PERFORMANCE_INDEX>(null, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFbFacultyPerformanceIndex), sAcademicYear).DataSource.Data;
                if (fbFacyltyPerformanceIndex != null && fbFacyltyPerformanceIndex.Count > 0)
                {
                    objViewModel.liFbFacultyPerformanceIndex = fbFacyltyPerformanceIndex;
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("FeedBackController", "FacultyPerformanceIndex", ex.Message);
                }
            }


            return View(objViewModel);
        }
        public JsonResult SaveorUpdateFeedBackFacultyPerformanceIndex(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var result = JsonConvert.DeserializeObject<FBFACULTY_PERFORMANCE_INDEX>(sJsonData);
                if (result != null)
                {
                    if (result.FBFACULTY_PERFORMANCE_INDEX_ID != null && result.FBFACULTY_PERFORMANCE_INDEX_ID != "")
                    {
                        // UPDATE setting 
                        resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.UpdateFeedbackPerformanceIndex), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {
                        // ADD setting 

                        // validate role setting if its existed
                        var IsExist = (List<FBFACULTY_PERFORMANCE_INDEX>)CMSHandler.FetchData<FBFACULTY_PERFORMANCE_INDEX>(new FBFACULTY_PERFORMANCE_INDEX() { FEEDBACK_ID = result.FEEDBACK_ID, ITEM = result.ITEM }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedBackFacultyPerformanceIndexByFeedbackandItem), sAcademicYear).DataSource.Data;
                        if (IsExist != null && IsExist.Count > 0)
                        {
                            objResultData.Message = Common.Messages.RecordsExistAlready;
                            objResultData.ErrorCode = Common.ErrorCode.AlreadyReported;
                        }
                        else
                        {
                            resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.SaveFeedbackPerformanceIndex), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                    }

                }
            }
            return Json(objResultData);
        }

        public JsonResult FetchLiFeedBackFacultyPerformanceIndexByFeedbackFacultyperformanceIndex(string sFeedBackFacultyPerformanceIndex)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sFeedBackFacultyPerformanceIndex))
                {
                    // fetch setting 
                    objResultData.oResult = (List<FBFACULTY_PERFORMANCE_INDEX>)CMS.DAO.CMSHandler.FetchData<FBFACULTY_PERFORMANCE_INDEX>(new FBFACULTY_PERFORMANCE_INDEX() { FBFACULTY_PERFORMANCE_INDEX_ID = sFeedBackFacultyPerformanceIndex }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedbackFacultyPerformanceIndexByid), sAcademicYear).DataSource.Data;
                }
            }
            return Json(objResultData);
        }

        public JsonResult DeleteLiFeedBackFacultyPerformanceIndex(string sFeedBackFacultyPerformanceIndex)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                if (!string.IsNullOrEmpty(sFeedBackFacultyPerformanceIndex))
                {
                    // fetch setting 
                    resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new FBFACULTY_PERFORMANCE_INDEX() { FBFACULTY_PERFORMANCE_INDEX_ID = sFeedBackFacultyPerformanceIndex, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.DeleteFeedbackPerformanceIndex), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
            }
            return Json(objResultData);
        }
        //[UserRights("Staff")]
        public ActionResult FacultyPerformanceIndexEvaluation()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserType = Session[Common.SESSION_VARIABLES.USER_TYPE] != null ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            try
            {

                //fetch user role 
                string sRole = GetUserRolesByAcademicYearandUserTypeandUserId(sAcademicYear, sUserType, sUserId);
                if (!string.IsNullOrEmpty(sRole))
                {

                    FeedbackViewModel.sAcademicAttempt = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? 0 : FeedbackViewModel.sAcademicAttempt + 1;

                    // feed back
                    var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                    if (fbSetting != null && fbSetting.Count > 0)
                    {
                        objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("FeedBackController", "FacultyPerformanceIndexEvaluation", ex.Message);
                }
            }


            return View(objViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sFeedbackId"></param>
        /// <returns></returns>
        public ActionResult BindFbFacultyPerformanceIndexByFeedback(string sFeedbackId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            //fetch user id 
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sFeedbackId))
            {
                try
                {

                    // fetch feedback faculty performance index for evaluation

                    var liFeedfackPerformanceIndexFields = (List<FBFACULTY_PERFORMANCE_INDEX>)CMSHandler.FetchData<FBFACULTY_PERFORMANCE_INDEX>(new FBFACULTY_PERFORMANCE_INDEX_EVALUATION()
                    {
                        FEEDBACK_ID = sFeedbackId,
                        STAFF_ID = sUserId
                    }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackFacultyPerformanceIndexForEvaluationByStaffId), sAcademicYear
                    ).DataSource.Data;
                    if (liFeedfackPerformanceIndexFields != null && liFeedfackPerformanceIndexFields.Count > 0)
                    {
                        //find satus is already attended or not
                        bool sStatus = liFeedfackPerformanceIndexFields.FirstOrDefault().STATUS == "0" ? true : false;
                        if (sStatus == true)
                        {
                            objViewModel.liFbFacultyPerformanceIndex = liFeedfackPerformanceIndexFields;
                        }
                        else
                        {
                            objViewModel.sErrorMessage = "Already attended";
                        }

                    }
                    else
                    {
                        objViewModel.sErrorMessage = Common.Messages.NoRecordsfound;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "BindFbFacultyPerformanceIndexByFeedback", ex.Message);
                    }
                }
            }
            else
            {
                objViewModel.sErrorMessage = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return View(objViewModel);
        }

        public JsonResult SaveFeedbackFacultyPerformanceIndex(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            int sCount = 0;
            string sQuery = string.Empty;
            //fetch user id 
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<FB_JSON_FIELD>(sJsonData);
                    if (result.liFBFacultyPerformance != null && result.liFBFacultyPerformance.Count > 0)
                    {
                        //check if alreay attended
                        var IsExist = (List<FBFACULTY_PERFORMANCE_INDEX_EVALUATION>)CMSHandler.FetchData<FBFACULTY_PERFORMANCE_INDEX_EVALUATION>(new FBFACULTY_PERFORMANCE_INDEX_EVALUATION() { FBFACULTY_PERFORMANCE_INDEX_ID = result.liFBFacultyPerformance.FirstOrDefault().FBFACULTY_PERFORMANCE_INDEX_ID, STAFF_ID = sUserId },
                            SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbFacultyPerformanceIndexEvaluationByPerformanceIndexandStaff), sAcademicYear).DataSource.Data;

                        if (IsExist != null && IsExist.Count > 0)
                        {
                            objResultData.Message = "Already attended";
                            objResultData.ErrorCode = Common.ErrorCode.Created;
                            return Json(objResultData);
                        }



                        sQuery = @"INSERT INTO fbfaculty_performance_index_evaluation(FBFACULTY_PERFORMANCE_INDEX_ID,MARK,NUMBER,SCORE,STAFF_ID,ACADEMIC_YEAR,FEEDBACK_ID) VALUES";
                        foreach (var item in result.liFBFacultyPerformance)
                        {
                            sQuery += "('" + item.FBFACULTY_PERFORMANCE_INDEX_ID + "','" + item.MARK + "','" + item.NUMBER + "','" + item.SCORE + "','" + sUserId + "','?AC_YEAR','" + item.FEEDBACK_ID + "'),";
                            sCount++;
                        }
                        if (sCount > 0)
                        {
                            sQuery = sQuery.Trim(',') + ";";
                            resultArgs = CMSHandler.SaveOrUpdate(null, sQuery, sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                        }
                    }
                    else
                    {
                        objResultData.Message = Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "SaveFeedbackFacultyPerformanceIndex", ex.Message);
                    }
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Feedback Faculty Self Appraisal Type
        [UserRights("Admin")]
        public ActionResult ListFeedbackSelfAppraisalType(string sFeedbackId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                //FeedbackViewModel.sAcademicAttempt = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? 0 : FeedbackViewModel.sAcademicAttempt + 1;

                // feed back
                var fbSelfAppraisalType = (List<SUP_FBFACULTY_SELF_APPRAISAL_TYPE>)CMS.DAO.CMSHandler.FetchData<SUP_FBFACULTY_SELF_APPRAISAL_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchSupFbFacultyAppraisalType), sAcademicYear).DataSource.Data;
                if (fbSelfAppraisalType != null && fbSelfAppraisalType.Count > 0)
                {
                    objViewModel.liSupFacultyAppraisalType = fbSelfAppraisalType;
                }

            }
            return View(objViewModel);
        }

        public JsonResult FetchLiFeedBackAppraisalTypeById(string sFeedBackAppraisalTypeId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sFeedBackAppraisalTypeId))
                {
                    // fetch setting 
                    objResultData.oResult = (List<SUP_FBFACULTY_SELF_APPRAISAL_TYPE>)CMS.DAO.CMSHandler.FetchData<SUP_FBFACULTY_SELF_APPRAISAL_TYPE>(new SUP_FBFACULTY_SELF_APPRAISAL_TYPE() { FACULTY_SELF_APPRAISAL_TYPE_ID = sFeedBackAppraisalTypeId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedBackAppraisalTypeById), sAcademicYear).DataSource.Data;
                }
            }
            return Json(objResultData);
        }

        public JsonResult SaveorUpdateFeedBackAppraisalType(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<SUP_FBFACULTY_SELF_APPRAISAL_TYPE>(sJsonData);
                    if (result != null)
                    {

                        if (result.FACULTY_SELF_APPRAISAL_TYPE_ID != null && result.FACULTY_SELF_APPRAISAL_TYPE_ID != "")
                        {
                            // UPDATE appraisal type 
                            resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.UpdateFeedBackAppraisalType), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            //check if already exist or not 
                            if (ValidateFbFacultyAppraisalType(result.APPRAISAL_TYPE))
                            {
                                objResultData.Message = Common.Messages.RecordsExistAlready;
                                objResultData.ErrorCode = Common.ErrorCode.Created;
                            }
                            else
                            {
                                // ADD appraisal type 
                                resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.SaveFeedBackAppraisalType), sAcademicYear);
                                objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FeedbackController", "SaveorUpdateFeedBackAppraisalType", "Err MSg: " + ex.Message);

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

        public JsonResult DeleteLiFeedBackAppraisalType(string sFeedBackAppraisalTypeId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                if (!string.IsNullOrEmpty(sFeedBackAppraisalTypeId))
                {
                    // fetch setting 
                    resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new SUP_FBFACULTY_SELF_APPRAISAL_TYPE() { FACULTY_SELF_APPRAISAL_TYPE_ID = sFeedBackAppraisalTypeId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.DeletedBackAppraisalType), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
            }
            return Json(objResultData);
        }

        public bool ValidateFbFacultyAppraisalType(string sAppraisalType)
        {
            bool sResult = false;
            var ObjModel = new SUP_FBFACULTY_SELF_APPRAISAL_TYPE();
            if (!string.IsNullOrEmpty(sAppraisalType))
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
                var liValidate = (List<SUP_FBFACULTY_SELF_APPRAISAL_TYPE>)CMS.DAO.CMSHandler.FetchData<SUP_FBFACULTY_SELF_APPRAISAL_TYPE>(ObjModel.APPRAISAL_TYPE = sAppraisalType,
                    SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.ValidateFacultyAppraisalType), sAcademicYear).DataSource.Data;
                if (liValidate != null && liValidate.Count > 0)
                {
                    sResult = true;
                }
                else
                {
                    sResult = false;
                }
            }
            return sResult;
        }

        #endregion

        #region Feedback Faculty Self Appraisal
        [UserRights("Admin")]
        public ActionResult ListFacultySelfAppraisal()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            try
            {

                FeedbackViewModel.sAcademicAttempt = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? 0 : FeedbackViewModel.sAcademicAttempt + 1;
                // feed back
                var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                if (fbSetting != null && fbSetting.Count > 0)
                {
                    objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                }

                // feed back appraisal
                var fbappraisalType = (List<SUP_FBFACULTY_SELF_APPRAISAL_TYPE>)CMS.DAO.CMSHandler.FetchData<SUP_FBFACULTY_SELF_APPRAISAL_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchSupFbFacultyAppraisalType), sAcademicYear).DataSource.Data;
                if (fbappraisalType != null && fbappraisalType.Count > 0)
                {
                    objViewModel.AppraisalType = new SelectList(fbappraisalType, Common.SUP_FBFACULTY_SELF_APPRAISAL_TYPE.FACULTY_SELF_APPRAISAL_TYPE_ID, Common.SUP_FBFACULTY_SELF_APPRAISAL_TYPE.APPRAISAL_TYPE);
                }

                // feedback faculty self appraisal
                var fbFacyltyAppraisal = (List<FBFACULTY_SELF_APPRAISAL>)CMS.DAO.CMSHandler.FetchData<FBFACULTY_SELF_APPRAISAL>(null, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFbFacultySelfAppraisal), sAcademicYear).DataSource.Data;
                if (fbFacyltyAppraisal != null && fbFacyltyAppraisal.Count > 0)
                {
                    objViewModel.liFacultySelfAppraisal = fbFacyltyAppraisal;
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("FeedBackController", "ListFacultySelfAppraisal", ex.Message);
                }
            }


            return View(objViewModel);
        }
        public JsonResult SaveorUpdateFeedBackFacultySelfAppraisal(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var result = JsonConvert.DeserializeObject<FBFACULTY_SELF_APPRAISAL>(sJsonData);
                if (result != null)
                {
                    if (result.FBFACULTY_APPRAISAL_ID != null && result.FBFACULTY_APPRAISAL_ID != "")
                    {
                        // UPDATE feedback self appraisal
                        resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.UpdateFeedbackFacultyAppraisal), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {
                        // ADD feedback self appraisal 

                        // validate role setting if its existed
                        var IsExist = (List<FBFACULTY_SELF_APPRAISAL>)CMSHandler.FetchData<FBFACULTY_SELF_APPRAISAL>(new FBFACULTY_SELF_APPRAISAL() { FBFACULTY_APPRAISAL_ID = result.FBFACULTY_APPRAISAL_ID, APPRAISAL_TYPE_ID = result.APPRAISAL_TYPE_ID, ITEM = result.ITEM }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedBackFacultySelfAppraisalByFeedbackandItemandAppraisalType), sAcademicYear).DataSource.Data;
                        if (IsExist != null && IsExist.Count > 0)
                        {
                            objResultData.Message = Common.Messages.RecordsExistAlready;
                            objResultData.ErrorCode = Common.ErrorCode.AlreadyReported;
                        }
                        else
                        {
                            resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.SaveFeedbackFacultyAppraisal), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                    }

                }
            }
            return Json(objResultData);
        }

        public JsonResult FetchLiFeedBackSelfAppraisalByFeedbackFacultAppraisalId(string sFbFacultySelfAppraisalId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sFbFacultySelfAppraisalId))
                {
                    // fetch setting 
                    objResultData.oResult = (List<FBFACULTY_SELF_APPRAISAL>)CMS.DAO.CMSHandler.FetchData<FBFACULTY_SELF_APPRAISAL>(new FBFACULTY_SELF_APPRAISAL() { FBFACULTY_APPRAISAL_ID = sFbFacultySelfAppraisalId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedbackFacultySelfAppraisalByid), sAcademicYear).DataSource.Data;
                }
            }
            return Json(objResultData);
        }

        public JsonResult DeleteLiFeedBackFacultySelfAppraisal(string sFeedBackFacultySelfAppraisalId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                if (!string.IsNullOrEmpty(sFeedBackFacultySelfAppraisalId))
                {
                    // fetch setting 
                    resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new FBFACULTY_SELF_APPRAISAL() { FBFACULTY_APPRAISAL_ID = sFeedBackFacultySelfAppraisalId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.DeleteFeedbackFacultyAppraisalbyId), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
            }
            return Json(objResultData);
        }

        //[UserRights("Staff")]
        public ActionResult FeedbackFacultySelfAppraisalEvaluation()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserType = Session[Common.SESSION_VARIABLES.USER_TYPE] != null ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            try
            {
                //fetch user role 
                string sRole = GetUserRolesByAcademicYearandUserTypeandUserId(sAcademicYear, sUserType, sUserId);
                if (!string.IsNullOrEmpty(sRole))
                {
                    FeedbackViewModel.sAcademicAttempt = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? 0 : FeedbackViewModel.sAcademicAttempt + 1;


                    // feed back
                    var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                    if (fbSetting != null && fbSetting.Count > 0)
                    {
                        objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                    }
                }


            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("FeedBackController", "FeedbackFacultySelfAppraisalEvaluation", ex.Message);
                }
            }


            return View(objViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sFeedbackId"></param>
        /// <returns></returns>
        public ActionResult BindFbFacultySelfAppraisalByFeedback(string sFeedbackId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            //fetch user id 
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sFeedbackId))
            {
                try
                {

                    // fetch feedback faculty performance index for evaluation

                    var liFeedfackFacultySelfAppraisal = (List<FBFACULTY_SELF_APPRAISAL_EVALUATION>)CMSHandler.FetchData<FBFACULTY_SELF_APPRAISAL_EVALUATION>(new FBFACULTY_SELF_APPRAISAL_EVALUATION()
                    {
                        FEEDBACK_ID = sFeedbackId,
                        STAFF_ID = sUserId
                    }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackFacultySelfAppraisalForEvaluationByStaffId), sAcademicYear
                    ).DataSource.Data;
                    if (liFeedfackFacultySelfAppraisal != null && liFeedfackFacultySelfAppraisal.Count > 0)
                    {
                        //find satus is already attended or not
                        bool sStatus = liFeedfackFacultySelfAppraisal.FirstOrDefault().STATUS == "0" ? true : false;
                        if (sStatus == true)
                        {
                            objViewModel.liFacultySelfAppraisalEvaluation = liFeedfackFacultySelfAppraisal;
                        }
                        else
                        {
                            objViewModel.sErrorMessage = "Already attended";
                        }

                    }
                    else
                    {
                        objViewModel.sErrorMessage = Common.Messages.NoRecordsfound;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "BindFbFacultyPerformanceIndexByFeedback", ex.Message);
                    }
                }
            }
            else
            {
                objViewModel.sErrorMessage = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return View(objViewModel);
        }

        public JsonResult SaveFeedbackFacultySelfAppraisal(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            int sCount = 0;
            string sQuery = string.Empty;
            //fetch user id 
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<FB_JSON_FIELD>(sJsonData);
                    if (result.liFBFacultySelfAppraisal != null && result.liFBFacultySelfAppraisal.Count > 0)
                    {
                        //check if alreay attended
                        var IsExist = (List<FBFACULTY_SELF_APPRAISAL_EVALUATION>)CMSHandler.FetchData<FBFACULTY_SELF_APPRAISAL_EVALUATION>(new FBFACULTY_SELF_APPRAISAL_EVALUATION() { FBFACULTY_APPRAISAL_ID = result.liFBFacultySelfAppraisal.FirstOrDefault().FBFACULTY_APPRAISAL_ID, STAFF_ID = sUserId },
                            SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbFacultySelfAppraisalEvaluationByAppraisalIdandStaffId), sAcademicYear).DataSource.Data;

                        if (IsExist != null && IsExist.Count > 0)
                        {
                            objResultData.Message = "Already attended";
                            objResultData.ErrorCode = Common.ErrorCode.Created;
                            return Json(objResultData);
                        }


                        sQuery = @"INSERT INTO fbfaculty_self_appraisal_evaluation(FBFACULTY_APPRAISAL_ID,MARK,NUMBER,SCORE,STAFF_ID,ACADEMIC_YEAR,FEEDBACK_ID) VALUES";
                        foreach (var item in result.liFBFacultySelfAppraisal)
                        {
                            sQuery += "('" + item.FBFACULTY_APPRAISAL_ID + "','" + item.MARK + "','" + item.NUMBER + "','" + item.SCORE + "','" + sUserId + "','?AC_YEAR','" + item.FEEDBACK_ID + "'),";
                            sCount++;
                        }
                        if (sCount > 0)
                        {
                            sQuery = sQuery.Trim(',') + ";";
                            resultArgs = CMSHandler.SaveOrUpdate(null, sQuery, sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                        }
                    }
                    else
                    {
                        objResultData.Message = Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "SaveFeedbackFacultySelfAppraisal", ex.Message);
                    }
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Student Assessment of teacher
        [UserRights("Admin")]
        public ActionResult ListFeedbackAssesmentOfTeacher(string sFeedbackId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // feed back
                    var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                    if (fbSetting != null && fbSetting.Count > 0)
                    {
                        objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                    }

                    // feedback faculty assessment
                    var fbFacyltyAssessment = (List<FBFACULTY_ASSESSMENT>)CMS.DAO.CMSHandler.FetchData<FBFACULTY_ASSESSMENT>(null, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFbFacultyAssessment), sAcademicYear).DataSource.Data;
                    if (fbFacyltyAssessment != null && fbFacyltyAssessment.Count > 0)
                    {
                        objViewModel.liFacultyAssessment = fbFacyltyAssessment;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "ListFeedbackAssesmentOfTeacher", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }

        public JsonResult SaveorUpdateFeedBackFacultyAssessment(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var result = JsonConvert.DeserializeObject<FBFACULTY_ASSESSMENT>(sJsonData);
                if (result != null)
                {
                    if (result.FBFACULTY_ASSESSMENT_ID != null && result.FBFACULTY_ASSESSMENT_ID != "")
                    {
                        // UPDATE setting 
                        resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.UpdateFeedbackFacultyAssessment), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {
                        // ADD setting 

                        // validate role setting if its existed
                        var IsExist = (List<FBFACULTY_ASSESSMENT>)CMSHandler.FetchData<FBFACULTY_ASSESSMENT>(new FBFACULTY_ASSESSMENT() { FEEDBACK_ID = result.FEEDBACK_ID, ITEM = result.ITEM }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedBackFacultyAssessmentByFeedbackandItem), sAcademicYear).DataSource.Data;
                        if (IsExist != null && IsExist.Count > 0)
                        {
                            objResultData.Message = Common.Messages.RecordsExistAlready;
                            objResultData.ErrorCode = Common.ErrorCode.AlreadyReported;
                        }
                        else
                        {
                            resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.SaveFeedbackFacultyAssessment), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                    }

                }
            }
            return Json(objResultData);
        }


        public JsonResult DeleteLiFeedBackFacultyAssessment(string sfeedbackAssessmentId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                if (!string.IsNullOrEmpty(sfeedbackAssessmentId))
                {
                    // fetch setting 
                    resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new FBFACULTY_ASSESSMENT() { FBFACULTY_ASSESSMENT_ID = sfeedbackAssessmentId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.DeleteFeedbackFacultyAssessment), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
            }
            return Json(objResultData);
        }

        public JsonResult FetchLiFeedBackFacultyAssessmentByAssessmentId(string sfeedbackAssessmentId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sfeedbackAssessmentId))
                {
                    // fetch setting 
                    objResultData.oResult = (List<FBFACULTY_ASSESSMENT>)CMS.DAO.CMSHandler.FetchData<FBFACULTY_ASSESSMENT>(new FBFACULTY_ASSESSMENT() { FBFACULTY_ASSESSMENT_ID = sfeedbackAssessmentId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedbackFacultyAssessmentByid), sAcademicYear).DataSource.Data;
                }
            }
            return Json(objResultData);
        }

        //  [UserRights("Student")]
        public ActionResult FacultyAssessmentEvaluation()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            try
            {

                // feed back
                var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                if (fbSetting != null && fbSetting.Count > 0)
                {
                    objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                }

                //fetch all programme
                var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                if (liProgramme != null && liProgramme.Count > 0)
                {
                    objViewModel.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                }


            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("FeedBackController", "FacultyAssessmentEvaluation", ex.Message);
                }
            }


            return View(objViewModel);
        }

        public ActionResult BindFbFacultyAssessmentByFeedback(string sFeedbackId, string sAssesse)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            //fetch user id 
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sFeedbackId) && !string.IsNullOrEmpty(sAssesse))
            {
                try
                {

                    // fetch feedback faculty performance index for evaluation

                    var liFeedfackAssessmentFields = (List<FBFACULTY_ASSESSMENT>)CMSHandler.FetchData<FBFACULTY_ASSESSMENT>(new FBFACULTY_ASSESSMENT()
                    {
                        FEEDBACK_ID = sFeedbackId,
                        ASSESSOR = sUserId,
                        ASSESSE = sAssesse
                    }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackFacultyAssessmentForEvaluationByStaffId), sAcademicYear
                    ).DataSource.Data;
                    if (liFeedfackAssessmentFields != null && liFeedfackAssessmentFields.Count > 0)
                    {
                        //find satus is already attended or not
                        bool sStatus = liFeedfackAssessmentFields.FirstOrDefault().STATUS == "0" ? true : false;
                        if (sStatus == true)
                        {
                            objViewModel.liFacultyAssessment = liFeedfackAssessmentFields;
                        }
                        else
                        {
                            objViewModel.sErrorMessage = "Already attended";
                        }

                    }
                    else
                    {
                        objViewModel.sErrorMessage = Common.Messages.NoRecordsfound;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "BindFbFacultyAssessmentByFeedback", ex.Message);
                    }
                }
            }
            else
            {
                objViewModel.sErrorMessage = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return View(objViewModel);
        }

        public JsonResult SaveFeedbackFacultyAssessment(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            JsonResultData objResultData = new JsonResultData();
            int sCount = 0;
            string sQuery = string.Empty;
            //fetch user id 
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<FB_JSON_FIELD>(sJsonData);
                    if (result.liFBFacultyAssessmentEvaluation != null && result.liFBFacultyAssessmentEvaluation.Count > 0)
                    {
                        //check if alreay attended
                        var IsExist = (List<FBFACULTY_ASSESSMENT_EVALUATION>)CMSHandler.FetchData<FBFACULTY_ASSESSMENT_EVALUATION>(new FBFACULTY_ASSESSMENT_EVALUATION() { FBFACULTY_ASSESSMENT_ID = result.liFBFacultyAssessmentEvaluation.FirstOrDefault().FBFACULTY_ASSESSMENT_ID, ASSESSOR = sUserId, ASSESSE = result.liFBFacultyAssessmentEvaluation.FirstOrDefault().ASSESSE },
                            SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbFacultyAssessmentEvaluationByAssessmentIdandStaffId), sAcademicYear).DataSource.Data;

                        if (IsExist != null && IsExist.Count > 0)
                        {
                            objResultData.Message = "Already attended";
                            objResultData.ErrorCode = Common.ErrorCode.Created;
                            return Json(objResultData);
                        }



                        sQuery = @"INSERT INTO fbfaculty_assessment_evaluation(FBFACULTY_ASSESSMENT_ID,SCORE,ASSESSOR,ASSESSE,ACADEMIC_YEAR) VALUES";
                        foreach (var item in result.liFBFacultyAssessmentEvaluation)
                        {
                            sQuery += "('" + item.FBFACULTY_ASSESSMENT_ID + "','" + item.SCORE + "','" + sUserId + "','" + item.ASSESSE + "','?AC_YEAR'),";
                            sCount++;
                        }
                        if (sCount > 0)
                        {
                            sQuery = sQuery.Trim(',') + ";";
                            resultArgs = CMSHandler.SaveOrUpdate(null, sQuery, sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                        }
                    }
                    else
                    {
                        objResultData.Message = Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "SaveFeedbackFacultyAssessment", ex.Message);
                    }
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region FeedBack Question 

        [UserRights("Admin")]
        public ActionResult FeedBackQuestions(string sFeedBackId, string sFeedBackRoleId, string sFeedbackQuestionGroup)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                FeedbackViewModel.sAcademicAttempt = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? 0 : FeedbackViewModel.sAcademicAttempt + 1;

                // feed back
                var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                if (fbSetting != null && fbSetting.Count > 0)
                {
                    objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                }

                if (!string.IsNullOrEmpty(sFeedBackId))
                {
                    objViewModel.sFeedBackId = sFeedBackId;
                }
                if (!string.IsNullOrEmpty(sFeedBackRoleId))
                {
                    objViewModel.sFeedBackRoleSettingId = sFeedBackRoleId;
                }
                if (!string.IsNullOrEmpty(sFeedbackQuestionGroup))
                {
                    objViewModel.sFeedBackQuestionGroup = sFeedbackQuestionGroup;
                }
                //// fetch setting 
                //var FeedBackRoleSetting = (List<FBFEEDBACK_ROLE_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_ROLE_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchFeedBackRoleSettings), sAcademicYear).DataSource.Data;
                //if (FeedBackRoleSetting != null && FeedBackRoleSetting.Count > 0)
                //{
                //    objViewModel.FeedBackRoleSettingList = new SelectList(FeedBackRoleSetting, Common.FBFEEDBACK_ROLE_SETTINGS.FEEDBACK_ROLE_ID, Common.FBFEEDBACK_ROLE_SETTINGS.FEEDBACK_ROLE_NAME);
                //}
            }
            return View(objViewModel);

        }

        public string FetchFeedBackQuestionGroupByFeedBackId(string sFeedbackId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            string sOption = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sFeedbackId))
                {
                    var liQuestionGroup = (List<FBQUESTIONS_GROUP>)CMSHandler.FetchData<FBQUESTIONS_GROUP>(new FBQUESTIONS_GROUP() { FEEDBACK_ID = sFeedbackId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbQuestionGroupByFeedbackId), sAcademicYear).DataSource.Data;
                    if (liQuestionGroup != null && liQuestionGroup.Count > 0)
                    {
                        sOption = "<option value=''  >----Select-----</option>";
                        foreach (var item in liQuestionGroup)
                        {
                            sOption += "<option value=" + item.QUESTION_GROUP_ID + "  >" + item.GROUP_TITLE + "</option>";
                        }
                    }
                }
            }
            return sOption;

        }


        /// <summary>
        /// this is for showing list of feed back question based  on the feedback and feedback role setting
        /// </summary>
        /// <param name="sFeedBackId"></param>
        /// <param name="sFeedBackRoleId"></param>
        /// <returns></returns>
        public ActionResult ListFeedBackQuestions(string sFeedBackId, string sFeedBackRoleId, string sQuestionGroup)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sFeedBackId) && !string.IsNullOrEmpty(sFeedBackRoleId) && !string.IsNullOrEmpty(sQuestionGroup))
                {
                    objViewModel.sFeedBackId = sFeedBackId;
                    objViewModel.sFeedBackRoleSettingId = sFeedBackRoleId;
                    objViewModel.sFeedBackQuestionGroup = sQuestionGroup;
                    objViewModel.QuestionList = (List<FBFEEDBACK_QUESTIONS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_QUESTIONS() { FEEDBACK_ID = sFeedBackId, FEEDBACK_ROLE_ID = sFeedBackRoleId, QUESTION_GROUP = sQuestionGroup }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedBackQuestionBySettingAndFeedBackRoleAndQuestionGroup), sAcademicYear).DataSource.Data;
                }
            }
            return View(objViewModel);
        }
        /// <summary>
        /// this is for adding feed back question based on the feedback and feedback role setting 
        /// </summary>
        /// <param name="sFeedBackId"></param>
        /// <param name="sFeedBackRoleId"></param>
        /// <returns></returns>
        public ActionResult AddFeedBackQuestions(string sFeedBackId, string sFeedBackRoleId, string sQuestionGroup, string sQuestionId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sFeedBackId) && !string.IsNullOrEmpty(sFeedBackRoleId))
                {
                    objViewModel.sFeedBackId = sFeedBackId;
                    objViewModel.sFeedBackRoleSettingId = sFeedBackRoleId;
                    objViewModel.sFeedBackQuestionGroup = sQuestionGroup;
                    // fetch question type and question group

                    objViewModel.liFbSettings = (List<FBFEEDBACK_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(new FBFEEDBACK_QUESTIONS() { FEEDBACK_ID = sFeedBackId, FEEDBACK_ROLE_ID = sFeedBackRoleId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeedBackSettingByFeedBackIdAndFeedBackRoleId), sAcademicYear).DataSource.Data;

                    // fetch total count of question 

                    if (!string.IsNullOrEmpty(sQuestionId))
                    {
                        objViewModel.QuestionList = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_QUESTIONS() { QUESTION_ID = sQuestionId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedBackQuestionsByQuestionId), sAcademicYear).DataSource.Data;
                        if (objViewModel.QuestionList.Count > 0 && objViewModel.QuestionList != null)
                        {
                            objViewModel.Question = objViewModel.QuestionList.FirstOrDefault().QUESTION;
                            objViewModel.QuestionId = objViewModel.QuestionList.FirstOrDefault().QUESTION_ID;
                        }

                    }


                }
            }
            return View(objViewModel);
        }

        /// <summary>
        ///  this is for fetching feedback role setting based on feedback
        /// </summary>
        /// <param name="sFeedBackId"></param>
        /// <returns></returns>
        public string FetchFeedBackRoleSettingByFeedBack(string sFeedBackId)
        {
            string sOption = string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            var FeedBackRoleSetting = (List<FBFEEDBACK_ROLE_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_ROLE_SETTINGS>(new FBFEEDBACK_ROLE_SETTINGS() { FEEDBACK_ID = sFeedBackId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeedBackRoleSettingsByFeedBackid), sAcademicYear).DataSource.Data;
            if (FeedBackRoleSetting != null && FeedBackRoleSetting.Count > 0)
            {
                foreach (var item in FeedBackRoleSetting)
                {
                    sOption += "<option value=" + item.FEEDBACK_ROLE_ID + "  >" + item.FEEDBACK_ROLE_NAME + "</option>";
                }
            }

            return sOption;


        }
        /// <summary>
        /// this is for saving/update the feedback questions
        /// </summary>
        /// <param name="sJsonData"></param>
        /// <param name="sFeedBackQuestions"></param>
        /// <returns></returns>
        public JsonResult SaveFeedBackQuestions(string sJsonData, string sFeedBackQuestions)
        {
            JsonResultData objJsonData = new JsonResultData();
            string query = string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            // string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<JSON_SAVE_QUESTION>(sFeedBackQuestions);
                    var Result1 = JsonConvert.DeserializeObject<FBFEEDBACK_QUESTIONS>(sJsonData);


                    if (Result1.QUESTION_ID != "")
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(new FBFEEDBACK_QUESTIONS()
                        {
                            QUESTION_ID = Result1.QUESTION_ID,
                            FEEDBACK_ID = Result1.FEEDBACK_ID,
                            FEEDBACK_ROLE_ID = Result1.FEEDBACK_ROLE_ID,
                            QUESTION = Result.liFB_Question.FirstOrDefault().QUESTION.ToString(),
                            QUESTION_GROUP = Result1.QUESTION_GROUP
                        }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.UpdateFeedBackQuestionsByQuestionId), sAcademicYear);
                        objJsonData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    }
                    else
                    {
                        query = @"INSERT INTO FBFEEDBACK_QUESTIONS(FEEDBACK_ID,FEEDBACK_ROLE_ID,QUESTION,QUESTION_GROUP,ACADEMIC_YEAR) VALUES";

                        foreach (var item in Result.liFB_Question)
                        {
                            query += "('" + Result1.FEEDBACK_ID + "','" + Result1.FEEDBACK_ROLE_ID + "','?QUESTION','" + Result1.QUESTION_GROUP + "','?AC_YEAR'),";
                            query = query.Replace(Common.Delimiter.QUS + Common.FBFEEDBACK_QUESTIONS.QUESTION, item.QUESTION);
                        }
                        query = query.Trim(',') + ";";
                        resultArgs = CMSHandler.SaveOrUpdate(null, query, sAcademicYear);
                        objJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "SaveFeedBackQuestions", ex.Message);
                    }
                }
            }
            return Json(objJsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// this is for deleting the feedback question
        /// </summary>
        /// <returns></returns>
        public JsonResult DeleteFeedBackQuestion(string sQuestionId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sQuestionId))
                {
                    resultArgs = CMSHandler.SaveOrUpdate(new FBFEEDBACK_QUESTIONS() { QUESTION_ID = sQuestionId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.DeleteFeedBackQuestionByQuestionId), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);

        }


        #endregion

        #region Feedback Answer
        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index1()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel.sAcademicAttempt = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? 0 : FeedbackViewModel.sAcademicAttempt + 1;
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserType = Session[Common.SESSION_VARIABLES.USER_TYPE] != null ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            try
            {
                //fetch user role 
                string sRole = GetUserRolesByAcademicYearandUserTypeandUserId(sAcademicYear, sUserType, sUserId);
                if (!string.IsNullOrEmpty(sRole))
                {
                    if (sRole == "5")
                    {
                        var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(new STU_CLASS { STUDENT_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettingsForStudent), sAcademicYear).DataSource.Data;
                        if (fbSetting != null && fbSetting.Count > 0)
                        {
                            objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                        }
                        else
                        {
                            objViewModel.sErrorMessage = "There is no feedback For your Class";
                        }
                    }
                    else
                    {
                        var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                        if (fbSetting != null && fbSetting.Count > 0)
                        {
                            objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                        }
                    }
                    // feed back
                    //var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                    //if (fbSetting != null && fbSetting.Count > 0)
                    //{
                    //    objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                    //}
                }
                else
                {
                    objViewModel.sErrorMessage = "You are not a member in " + sAcademicYear + " year";
                }

                return View(objViewModel);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "Index1", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "Index1", ex.Message);
                }
                return RedirectToAction("Index1", "Account");
            }

        }

        public string GetUserRolesByAcademicYearandUserTypeandUserId(string sAcYear, string sUserType, string sUserId)
        {
            string sRoles = string.Empty;
            if (!string.IsNullOrEmpty(sAcYear) && !string.IsNullOrEmpty(sUserType) && !string.IsNullOrEmpty(sUserId))
            {
                var liRoles = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(new USER_ACCOUNT_INFO()
                {
                    USER_TYPE = sUserType,
                    USER_ID = sUserId

                }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchUserRolesByUserIdandAcYear), sAcYear).DataSource.Data;
                if (liRoles != null && liRoles.Count > 0)
                {
                    sRoles = liRoles.FirstOrDefault().ROLE;
                }
            }

            return sRoles;
        }

        /// <summary>
        /// this is fetch feedback role setting based on the feed back and user role
        /// </summary>
        /// <param name="sFeedBackId"></param>
        /// <returns></returns>
        public string FetchFeedBackRoleSettingByFeedBackAndRole(string sFeedBackId)
        {
            string sOption = string.Empty;
            string sMessage = string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserType = Session[Common.SESSION_VARIABLES.USER_TYPE] != null ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sFeedBackId))
            {
                try
                {
                    // assign feedback id in to sesssion
                    Session[Common.SESSION_VARIABLES.FEEDBACK_ID] = sFeedBackId;

                    //fetch user role 
                    string sRole = GetUserRolesByAcademicYearandUserTypeandUserId(sAcademicYear, sUserType, sUserId);
                    if (!string.IsNullOrEmpty(sRole))
                    {
                        string sQl = SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedBackRoleSettingsByFeedBackidAndRole);

                        var FeedBackRoleSetting = (List<FBFEEDBACK_ROLE_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_ROLE_SETTINGS>(new FBFEEDBACK_ROLE_SETTINGS() { FEEDBACK_ID = sFeedBackId }, sQl.Replace(Common.Delimiter.QUS + Common.SUP_FBFEEDBACK_ROLE.ASSESSOR_ROLE, sRole), sAcademicYear).DataSource.Data;
                        if (FeedBackRoleSetting != null && FeedBackRoleSetting.Count > 0)
                        {
                            foreach (var item in FeedBackRoleSetting)
                            {
                                sOption += "<option value=" + item.FEEDBACK_ROLE_ID + " cms-sup-fb-role-id=" + item.SUP_FEEDBACK_ROLE_ID + " cms-assessor-role=" + item.ASSESSOR_ROLE + "  cms-assessee-role=" + item.ASSESSEE_ROLE + ">" + item.FEEDBACK_ROLE_NAME + "</option>";
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FeedbackController", "FetchFeedBackRoleSettingByFeedBackAndRole", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FeedbackController", "FetchFeedBackRoleSettingByFeedBackAndRole", ex.Message);
                    }
                }
            }
            return sOption;
        }
        public string FetchFeedBackClassBasedId(string sClassId)
        {
            string sOption = string.Empty;
            string sMessage = string.Empty;
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserType = Session[Common.SESSION_VARIABLES.USER_TYPE] != null ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sClassId))
            {
                try
                {
                    // assign feedback id in to sesssion
                    Session[Common.SESSION_VARIABLES.CLASS_ID] = sClassId;

                    //fetch user role 
                    string sRole = GetUserRolesByAcademicYearandUserTypeandUserId(sAcademicYear, sUserType, sUserId);
                    if (!string.IsNullOrEmpty(sRole))
                    {
                        // Fetch Regular Papers.
                        var liRegularCourses = (List<EXAM_REGISTRATION>)CMS.DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { CLASS_ID = sClassId },
                            SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchExamRegistration1), sAcademicYear).DataSource.Data;

                        //Fetch Optional Papers .
                        var liOptionalCourses = (List<EXAM_REGISTRATION>)DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { CLASS_ID = sClassId },
                                     SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchOptionalCourses1), sAcademicYear).DataSource.Data;

                        //Loop Optional Papers  To Remove Unnecessary Courses...
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

                        objViewModel.StudentCourseList = new SelectList(liRegularCourses, "COURSE_ROOT_ID", "COURSE_TITLE", "---Select---");

                        var selectedValue = objViewModel.StudentCourseList.SelectedValue;

                        // Convert the selected value to a string
                        sClassId = selectedValue != null ? selectedValue.ToString() : string.Empty;

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FeedbackController", "FetchFeedBackRoleSettingByFeedBackAndRole", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FeedbackController", "FetchFeedBackRoleSettingByFeedBackAndRole", ex.Message);
                    }
                }
            }
            return sClassId;
        }

        /// <summary>
        /// based on the user feed back role return view
        /// </summary>
        /// <param name="sSupFeedBackRoleId"></param>
        /// <returns></returns>
        public ActionResult BindFeedBackAnswerFields(string sSupFeedBackRoleId)
        {
            if (!string.IsNullOrEmpty(sSupFeedBackRoleId))
            {
                try
                {
                    switch (sSupFeedBackRoleId)
                    {
                        case Common.FBFEEDBACK_ROLE.VICEPRINCIPAL_BY_PRINCIPAL:
                            {
                                return RedirectToAction("FeedBackPricipalToVicePrincipal");
                            }
                        case Common.FBFEEDBACK_ROLE.HOD_BY_PRINCIPAL:
                            {
                                return RedirectToAction("FeedBackPricipalToHod");
                            }
                        case Common.FBFEEDBACK_ROLE.STAFF_BY_PRINCIPAL:
                            {
                                return RedirectToAction("FeedBackPricipalToStaff");
                            }
                        case Common.FBFEEDBACK_ROLE.STAFF_BY_STUDENT:
                            {
                                //return RedirectToAction("FeedBackStudentToStaff");
                                return RedirectToAction("FeedBackStudentToStaffNew");
                            }
                        case Common.FBFEEDBACK_ROLE.STAFF_INCHARGE_BY_STUDENT:
                            {
                                return RedirectToAction("FeedBackStudentToStaffIncharge");
                            }
                        case Common.FBFEEDBACK_ROLE.HOD_BY_STAFF:
                            {
                                return RedirectToAction("FeedBackStaffToHod");
                            }
                        case Common.FBFEEDBACK_ROLE.PRINCIPAL_BY_STAFF:
                            {
                                return RedirectToAction("FeedBackStaffToPrincipal");
                            }
                        case Common.FBFEEDBACK_ROLE.DEEN_BY_STAFF:
                            {
                                return RedirectToAction("FeedBackFacultyToDean");
                            }
                        case Common.FBFEEDBACK_ROLE.VICEPRINCIPAL_BY_STAFF:
                            {
                                return RedirectToAction("FeedBackPricipalToVicePrincipal");
                            }
                        case Common.FBFEEDBACK_ROLE.STAFF_BY_HOD:
                            {
                                return RedirectToAction("FeedBackPricipalToStaff");
                            }

                        case Common.FBFEEDBACK_ROLE.STAFF_BY_DEEN:
                            {
                                return RedirectToAction("FeedBackPricipalToStaff");
                            }
                        case Common.FBFEEDBACK_ROLE.PRINCIPAL_BY_PEER:
                            {
                                return RedirectToAction("FeedBackPeerToPrincipal");
                            }
                        case Common.FBFEEDBACK_ROLE.HOD_BY_PEER:
                            {
                                return RedirectToAction("FeedBackPeerToHod");
                            }
                        case Common.FBFEEDBACK_ROLE.STAFF_BY_PEER:
                            {
                                return RedirectToAction("FeedBackPeerToFaculty");
                            }
                        case Common.FBFEEDBACK_ROLE.DEEN_BY_PEER:
                            {
                                return RedirectToAction("FeedBackPeerToDean");
                            }
                        case Common.FBFEEDBACK_ROLE.STAFF_SELF_ENTRY:
                            {
                                return RedirectToAction("FeedBackStaffSelfEntry");
                            }
                        case Common.FBFEEDBACK_ROLE.STUDENT_SELF_ENTRY:
                            {
                                return RedirectToAction("FeedBackStudentSelfEntry");
                            }
                        case Common.FBFEEDBACK_ROLE.STAFF_BY_COE:
                            {
                                return RedirectToAction("FeedBackPeerToPrincipal");
                            }
                        case Common.FBFEEDBACK_ROLE.STAFF_BY_MANAGER:
                            {
                                return RedirectToAction("FeedBackPeerToPrincipal");
                            }
                        case Common.FBFEEDBACK_ROLE.CLASS_WISE_FEEDBACK_FOR_STUDENTS:
                            {
                                return RedirectToAction("FeedBackClassWiseFeedbackForStudents");
                            }

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FeedbackController", "BindFeedBackAnswerFields", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FeedbackController", "BindFeedBackAnswerFields", ex.Message);
                    }

                }
            }

            return View();
        }
        public ActionResult FeedBackPricipalToVicePrincipal()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                // fetch vice pricipal 
                var VicePricipal = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchVicePrincipal), sAcademicYear).DataSource.Data;
                if (VicePricipal != null && VicePricipal.Count > 0)
                {
                    objViewModel.FeedbackMember = new SelectList(VicePricipal, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                }


            }
            return View(objViewModel);
        }
        public ActionResult FeedBackPricipalToHod()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                //fetch all programme
                var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                if (liProgramme != null && liProgramme.Count > 0)
                {
                    objViewModel.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                }
            }
            return View(objViewModel);
        }
        public ActionResult FeedBackPricipalToStaff()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                //fetch all programme
                var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                if (liProgramme != null && liProgramme.Count > 0)
                {
                    objViewModel.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                }
            }
            return View(objViewModel);
        }
        public ActionResult FeedBackStudentToStaff()
        {

            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            string sUserType = Session[Common.SESSION_VARIABLES.USER_TYPE] != null ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            try
            {

                //fetch user role 
                string sRole = GetUserRolesByAcademicYearandUserTypeandUserId(sAcademicYear, sUserType, sUserId);
                if (!string.IsNullOrEmpty(sRole))
                {
                    // Fetch Regular Papers.
                    var liRegularCourses = (List<EXAM_REGISTRATION>)CMS.DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId },
                        SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchExamRegistration), sAcademicYear).DataSource.Data;

                    //Fetch Optional Papers .
                    var liOptionalCourses = (List<EXAM_REGISTRATION>)DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId },
                                 SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;

                    //Loop Optional Papers  To Remove Unnecessary Courses...
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

                    objViewModel.StudentCourseList = new SelectList(liRegularCourses, "COURSE_ROOT_ID", "COURSE_TITLE", "---Select---");

                    //fetch all department
                    //var liDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null,
                    //      SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment)).DataSource.Data;
                    //if (liDepartment != null && liDepartment.Count > 0)
                    //{
                    //    objViewModel.DepartmentList = new SelectList(liDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    //}


                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FeedBackStudentToStaff", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FeedBackStudentToStaff", ex.Message);
                }
            }



            return View(objViewModel);
        }
        public ActionResult FeedBackStudentToStaffNew()
        {

            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            string sUserType = Session[Common.SESSION_VARIABLES.USER_TYPE] != null ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            try
            {
                //fetch user role 
                string sRole = GetUserRolesByAcademicYearandUserTypeandUserId(sAcademicYear, sUserType, sUserId);
                if (!string.IsNullOrEmpty(sRole))
                {
                    // Fetch Regular Papers.
                    var liRegularCourses = (List<EXAM_REGISTRATION>)CMS.DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId },
                        SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchCourseListForFeedBackEntry), sAcademicYear).DataSource.Data;

                    //Fetch Optional Papers .
                    var liOptionalCourses = (List<EXAM_REGISTRATION>)DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId },
                                 SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchOptionalCoursesForFeedBackEntry), sAcademicYear).DataSource.Data;

                    //Loop Optional Papers  To Remove Unnecessary Courses...
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
                            else
                            {
                                liRegularCourses.Add(item);
                            }
                        }
                    }

                    objViewModel.StudentCourseList = new SelectList(liRegularCourses, "COURSE_ROOT_ID", "COURSE_TITLE", "---Select---");

                    //fetch all department
                    //var liDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null,
                    //      SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment)).DataSource.Data;
                    //if (liDepartment != null && liDepartment.Count > 0)
                    //{
                    //    objViewModel.DepartmentList = new SelectList(liDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    //}


                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FeedBackStudentToStaff", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FeedBackStudentToStaff", ex.Message);
                }
            }



            return View(objViewModel);
        }

        public async Task<ActionResult> StudentModernFeedbackEntry(string sFeedBackId = "", string sSupFeedBackRoleId = "")
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            string sUserType = Session[Common.SESSION_VARIABLES.USER_TYPE] != null ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            try
            {

                // Fetch All Student Feedbacks (Settings)
                var liSetting = (List<FBFEEDBACK_SETTINGS>)DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(new STUDENT_PERSONAL_INFO { STUDENT_ID = sUserId },
                      SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettingsForStudent), sAcademicYear).DataSource.Data;

                List<SelectListItem> liFeedBackSetting = (from item in liSetting
                                                          select new SelectListItem
                                                          {
                                                              Text = item.FEEDBACK_NAME,
                                                              Value = item.FEEDBACK_ID
                                                          }).ToList();
                ViewBag.FeedBackSettingList = liFeedBackSetting;

                if (string.IsNullOrEmpty(sFeedBackId) && liFeedBackSetting.Count > 0)
                {
                    sFeedBackId = liFeedBackSetting.FirstOrDefault().Value;
                }

                if (!string.IsNullOrEmpty(sFeedBackId))
                {
                    // Fetch Roles for selected Feedback
                    var liRole = (List<FBFEEDBACK_ROLE_SETTINGS>)DAO.CMSHandler.FetchData<FBFEEDBACK_ROLE_SETTINGS>(new FBFEEDBACK_ROLE_SETTINGS() { FEEDBACK_ID = sFeedBackId, ASSESSOR_ROLE = Common.SupRole.STUDENT },
                        SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedBackRoleSettingsByFeedBackidAndRole), sAcademicYear).DataSource.Data;

                    if (liRole != null && liRole.Count > 0)
                    {
                        if (string.IsNullOrEmpty(sSupFeedBackRoleId))
                        {
                            sSupFeedBackRoleId = liRole.FirstOrDefault().SUP_FEEDBACK_ROLE_ID;
                        }
                        ViewData["FeedbackRoleSettings"] = liRole;
                    }
                }

                ViewBag.SelectedFeedBackId = sFeedBackId;
                ViewBag.SelectedSupFeedBackRoleId = sSupFeedBackRoleId;

                if (!string.IsNullOrEmpty(sFeedBackId) && !string.IsNullOrEmpty(sSupFeedBackRoleId))
                {
                    // Case 1: Staff By Student (Course Wise)
                    if (sSupFeedBackRoleId == Common.FBFEEDBACK_ROLE.STAFF_BY_STUDENT)
                    {
                        var liRegularCourses = (List<EXAM_REGISTRATION>)CMS.DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId },
                            SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchCourseListForFeedBackEntry), sAcademicYear).DataSource.Data;

                        List<FBEVALUATION> liEvaluation = new List<FBEVALUATION>();

                        if (liRegularCourses != null && liRegularCourses.Count > 0)
                        {
                            foreach (var course in liRegularCourses)
                            {
                                string courseId = course.COURSE_ROOT_ID;
                                DataValue dv = new DataValue();
                                dv.Add("COURSE_ID", courseId, EnumCommand.DataType.String);
                                dv.Add(Common.STU_PERSONAL_INFO.STUDENT_ID, sUserId, EnumCommand.DataType.String);

                                var result = await Task.Run(() => CMSHandler.FetchData<FBEVALUATION>(new { STUDENT_ID = sUserId, COURSE_ID = courseId },
                                    SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFBCoursewiseStaff), sAcademicYear));
                                var liStaff = (List<FBEVALUATION>)result.DataSource.Data;

                                if (liStaff != null && liStaff.Count > 0)
                                {
                                    foreach (var drStaff in liStaff)
                                    {
                                        FBEVALUATION eval = new FBEVALUATION();
                                        eval.COURSE_ID = courseId;
                                        eval.COURSE_TITLE = course.COURSE_TITLE;
                                        eval.STAFF_CODE = drStaff.STAFF_ID;
                                        eval.FIRST_NAME = drStaff.STAFF_NAME;
                                        eval.STAFF_PHOTO = drStaff.PHOTO;
                                        eval.CLASS_ID = drStaff.CLASS_ID;

                                        // Check Completion
                                        var isExistItem = (List<FBEVALUATION>)CMSHandler.FetchData<FBEVALUATION>(new FBEVALUATION() { FEEDBACK_ID = sFeedBackId, ASSESSOR = sUserId, ASSESSEE = eval.STAFF_CODE, COURSE_ID = courseId, SUP_FEEDBACK_ROLE_ID = sSupFeedBackRoleId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.ValidateCourseByStudentId), sAcademicYear).DataSource.Data;
                                        eval.IS_ATTENDED = (isExistItem != null && isExistItem.Count > 0) ? "1" : "0";

                                        liEvaluation.Add(eval);
                                    }
                                }
                            }
                        }
                        objViewModel.LiEvaluation = liEvaluation;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "StudentModernFeedbackEntry", ex.Message);
                }
            }
            return View(objViewModel);
        }

        public async Task<ActionResult> GetModernFeedbackGrid(string sFeedBackId, string sSupFeedBackRoleId = "")
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

            try
            {
                // Re-populate the settings list (needed for the partial's dropdown)
                var liSetting = (List<FBFEEDBACK_SETTINGS>)DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(new STUDENT_PERSONAL_INFO { STUDENT_ID = sUserId },
                      SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettingsForStudent), sAcademicYear).DataSource.Data;

                ViewBag.FeedBackSettingList = (from item in liSetting
                                               select new SelectListItem
                                               {
                                                   Text = item.FEEDBACK_NAME,
                                                   Value = item.FEEDBACK_ID
                                               }).ToList();

                if (!string.IsNullOrEmpty(sFeedBackId))
                {
                    // If Role ID is missing, fetch the first available role for this student
                    var liRole = (List<FBFEEDBACK_ROLE_SETTINGS>)DAO.CMSHandler.FetchData<FBFEEDBACK_ROLE_SETTINGS>(new FBFEEDBACK_ROLE_SETTINGS() { FEEDBACK_ID = sFeedBackId, ASSESSOR_ROLE = Common.SupRole.STUDENT },
                        SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedBackRoleSettingsByFeedBackidAndRole), sAcademicYear).DataSource.Data;

                    if (string.IsNullOrEmpty(sSupFeedBackRoleId) && liRole != null && liRole.Count > 0)
                    {
                        sSupFeedBackRoleId = liRole.FirstOrDefault().SUP_FEEDBACK_ROLE_ID;
                    }
                    ViewData["FeedbackRoleSettings"] = liRole;

                    if (!string.IsNullOrEmpty(sSupFeedBackRoleId))
                    {
                        // Case 1: Staff By Student (Course Wise) -> Needs Grid
                        if (sSupFeedBackRoleId == Common.FBFEEDBACK_ROLE.STAFF_BY_STUDENT)
                        {
                            var liRegularCourses = (List<EXAM_REGISTRATION>)CMS.DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId },
                                SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchCourseListForFeedBackEntry), sAcademicYear).DataSource.Data;

                            List<FBEVALUATION> liEvaluation = new List<FBEVALUATION>();

                            if (liRegularCourses != null && liRegularCourses.Count > 0)
                            {
                                foreach (var course in liRegularCourses)
                                {
                                    string courseId = course.COURSE_ROOT_ID;
                                    var result = await Task.Run(() => CMSHandler.FetchData<FBEVALUATION>(new { STUDENT_ID = sUserId, COURSE_ID = courseId },
                                        SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFBCoursewiseStaff), sAcademicYear));
                                    var liStaff = (List<FBEVALUATION>)result.DataSource.Data;

                                    if (liStaff != null && liStaff.Count > 0)
                                    {
                                        foreach (var drStaff in liStaff)
                                        {
                                            FBEVALUATION eval = new FBEVALUATION();
                                            eval.COURSE_ID = courseId;
                                            eval.COURSE_TITLE = course.COURSE_TITLE;
                                            eval.STAFF_CODE = drStaff.STAFF_ID;
                                            eval.FIRST_NAME = drStaff.STAFF_NAME;
                                            eval.STAFF_PHOTO = drStaff.PHOTO;
                                            eval.CLASS_ID = drStaff.CLASS_ID;

                                            // Staff-specific completion check
                                            var isExistItem = (List<FBEVALUATION>)CMSHandler.FetchData<FBEVALUATION>(new FBEVALUATION() { FEEDBACK_ID = sFeedBackId, ASSESSOR = sUserId, ASSESSEE = eval.STAFF_CODE, COURSE_ID = courseId, SUP_FEEDBACK_ROLE_ID = sSupFeedBackRoleId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.ValidateCourseByStudentId), sAcademicYear).DataSource.Data;
                                            eval.IS_ATTENDED = (isExistItem != null && isExistItem.Count > 0) ? "1" : "0";

                                            liEvaluation.Add(eval);
                                        }
                                    }
                                }
                            }
                            objViewModel.LiEvaluation = liEvaluation;
                        }
                        // Case 2: College/Hostel and others -> Global Completion Check
                        else
                        {
                            // Global completion check (ignores course/assessee)
                            // We use "0" as placeholders to satisfy the query signature while focusing on FEEDBACK_ID + ASSESSOR
                            var isExistGlobal = (List<FBEVALUATION>)CMSHandler.FetchData<FBEVALUATION>(new FBEVALUATION() { FEEDBACK_ID = sFeedBackId, ASSESSOR = sUserId, ASSESSEE = "0", COURSE_ID = "0", SUP_FEEDBACK_ROLE_ID = sSupFeedBackRoleId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.ValidateCourseByStudentId), sAcademicYear).DataSource.Data;

                            ViewBag.DirectLoad = true;
                            ViewBag.IsAttended = (isExistGlobal != null && isExistGlobal.Count > 0) ? "1" : "0";
                        }
                    }
                }
                ViewBag.SelectedFeedBackId = sFeedBackId;
                ViewBag.SelectedSupFeedBackRoleId = sSupFeedBackRoleId;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "GetModernFeedbackGrid", ex.Message + " | Stack: " + ex.StackTrace);
                }
                // Return internal error to AJAX
                Response.StatusCode = 500;
                return Json(new { Message = ex.Message });
            }
            return PartialView("_ModernFeedbackGrid", objViewModel);
        }

        public async Task<ActionResult> BindModernFeedbackQuestions(string sCourseId = "", string sAssessee = "", string sFeedBackId = "", string sSupFbRoleId = "")
        {
            FeedbackViewModel objFeedback = new FeedbackViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                string sUserType = Session[Common.SESSION_VARIABLES.USER_TYPE] != null ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;

                if (string.IsNullOrEmpty(sFeedBackId))
                {
                    var liSetting = (List<FBFEEDBACK_SETTINGS>)DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null,
                          SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedbackInfo), sAcademicYear).DataSource.Data;
                    if (liSetting != null && liSetting.Count > 0)
                    {
                        sFeedBackId = liSetting.FirstOrDefault().FEEDBACK_ID;
                    }
                }

                if (string.IsNullOrEmpty(sSupFbRoleId))
                {
                    sSupFbRoleId = Common.FBFEEDBACK_ROLE.STAFF_BY_STUDENT;
                }

                if (string.IsNullOrEmpty(sAssessee) && !string.IsNullOrEmpty(sCourseId) && sSupFbRoleId == Common.FBFEEDBACK_ROLE.STAFF_BY_STUDENT)
                {
                    DataValue dv = new DataValue();
                    dv.Add("COURSE_ID", sCourseId, EnumCommand.DataType.String);
                    dv.Add(Common.STU_PERSONAL_INFO.STUDENT_ID, sUserId, EnumCommand.DataType.String);
                    var result = await Task.Run(() => objFeedback.FetchCourseWiseStaffList(dv, sAcademicYear));
                    var dtStaffList = result.DataSource.Table;
                    if (dtStaffList != null && dtStaffList.Rows.Count > 0)
                    {
                        sAssessee = dtStaffList.Rows[0]["STAFF_ID"].ToString();
                    }
                }

                string sAssesor_role = GetUserRolesByAcademicYearandUserTypeandUserId(sAcademicYear, sUserType, sUserId);
                string sAssesee_role = Common.SupRole.STAFF;

                // Fetch questions using existing logic
                ViewBag.CourseId = sCourseId;
                ViewBag.Assessee = sAssessee;
                return await FeedBackQuestionView(sFeedBackId, sAssesee_role, sAssesor_role, sAssessee, sSupFbRoleId, "44", sCourseId, true);
            }
            catch (Exception ex)
            {
                return Json(new { Message = "Internal Server Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> FeedBackQuestionViewNew(string sFeedBackId, string sAssesee_role, string sAssesor_role, string sAssesee, string sSupFbRoleId, string sClassId, string sCourseId)
        {
            FeedbackViewModel objFeedback = new FeedbackViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                string sUserType = Session[Common.SESSION_VARIABLES.USER_TYPE] != null ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
                //fetch user role 
                string Assessor_role = GetUserRolesByAcademicYearandUserTypeandUserId(sAcademicYear, sUserType, sUserId);

                //  string sAssessor_role = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString();

                // string sAssessor_Name = !string.IsNullOrEmpty(Session[Common.SESSION_VARIABLES.USER_NAME].ToString()) ? Session[Common.SESSION_VARIABLES.USER_NAME].ToString() : string.Empty; ;
                DataTable dtObjectiveList = new DataTable();
                //DataTable dtQuestionList = new DataTable();
                List<FBOBJECTIVES> ObjectiveList = new List<FBOBJECTIVES>();
                List<FBFEEDBACK_QUESTIONS> QuestoinList = new List<FBFEEDBACK_QUESTIONS>();


                if (!string.IsNullOrEmpty(sUserId) && !string.IsNullOrEmpty(sAssesee) && !string.IsNullOrEmpty(sFeedBackId) && !string.IsNullOrEmpty(sSupFbRoleId))
                {
                    // validate if is attended already
                    //student to staff by class and course
                    if (!string.IsNullOrEmpty(sClassId) && !string.IsNullOrEmpty(sCourseId) && sClassId != "0" && sCourseId != "0")
                    {

                        var IsExist = (List<FBEVALUATION>)CMSHandler.FetchData<FBEVALUATION>(new FBEVALUATION() { FEEDBACK_ID = sFeedBackId, ASSESSOR = sUserId, ASSESSEE = sAssesee, COURSE_ID = sCourseId, SUP_FEEDBACK_ROLE_ID = sSupFbRoleId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.ValidateCourseByStudentId), sAcademicYear).DataSource.Data;
                        if (IsExist != null && IsExist.Count > 0)
                        {
                            objFeedback.sErrorMessage = "Already Attended!";
                            return View(objFeedback);
                        }
                    }
                    // other roles
                    else
                    {
                        if (ValidateFeedBackEvaluation(sFeedBackId, sUserId, sAssesee, sSupFbRoleId))
                        {
                            objFeedback.sErrorMessage = "Already Attended!";
                            return View(objFeedback);
                        }
                    }

                    //fetch objective
                    //dtObjectiveList = objFeedback.FetchObjective().DataSource.Table;
                    //ObjectiveList = await Task.Run(() => (from DataRow dr in dtObjectiveList.Rows
                    //                                      select new FBOBJECTIVES()
                    //                                      {
                    //                                          OBJECTIVESHORTTERM = dr[Common.FBOBJECTIVES.OBJECTIVESHORTTERM].ToString(),
                    //                                          OBJECTIVES = dr[Common.FBOBJECTIVES.OBJECTIVES].ToString(),
                    //                                          RATING = dr[Common.FBOBJECTIVES.RATING].ToString(),
                    //                                          OBJECTIVE_ID = dr[Common.FBOBJECTIVES.OBJECTIVE_ID].ToString()
                    //                                      }
                    //             ).ToList());

                    //fetch objective list by fbfeedbackquestion objective mapping
                    ObjectiveList = (List<FBOBJECTIVES>)CMSHandler.FetchData<FBOBJECTIVES>(new FBFEEDBACK_QUESTIONS() { FEEDBACK_ID = sFeedBackId, ASSESSOR_ROLE = sAssesor_role, ASSESSEE_ROLE = sAssesee_role, SUP_FEEDBACK_ROLE_ID = sSupFbRoleId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedBackQuestionObjectiveMappingByFeedbackAndAssessorAndAssesseeNew), sAcademicYear).DataSource.Data;
                    objFeedback.ObjectiveList = ObjectiveList;


                    //fetch question
                    var QuestionList = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_QUESTIONS() { FEEDBACK_ID = sFeedBackId, ASSESSOR_ROLE = sAssesor_role, ASSESSEE_ROLE = sAssesee_role, SUP_FEEDBACK_ROLE_ID = sSupFbRoleId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedBackQuestionViewByFeedbackAndAssessorAndAssesseeNew), sAcademicYear).DataSource.Data;
                    if (QuestionList != null && QuestionList.Count > 0)
                    {
                        objFeedback.QuestionList = QuestionList;
                    }
                    else
                    {
                        objFeedback.sErrorMessage = "No questions were added!";
                    }
                    //fetch assessee name
                    if (sAssesee_role == Common.SupRole.STUDENT)
                    {
                        var Student = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new STU_PERSONAL_INFO() { STUDENT_ID = sAssesee }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentInfo), sAcademicYear).DataSource.Data;
                        if (Student != null && Student.Count > 0)
                        {
                            objFeedback.AssesseeName = Student.FirstOrDefault().FIRST_NAME;
                            objFeedback.AssesseePhoto = Student.FirstOrDefault().PHOTO;
                        }
                    }
                    else
                    {
                        var Staff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO() { STAFF_ID = sAssesee }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoByStaffId)).DataSource.Data;
                        if (Staff != null && Staff.Count > 0)
                        {
                            objFeedback.AssesseeName = Staff.FirstOrDefault().FIRST_NAME;
                            objFeedback.AssesseePhoto = Staff.FirstOrDefault().PHOTO;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FeedBackQuestionView", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FeedBackQuestionView", ex.Message);
                }
                return RedirectToAction("Index1", "Account");
            }

            return View(objFeedback);
        }
        public ActionResult FeedBackStudentToStaffIncharge()
        {

            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            string sUserType = Session[Common.SESSION_VARIABLES.USER_TYPE] != null ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
            try
            {

                //fetch user role 
                string sRole = GetUserRolesByAcademicYearandUserTypeandUserId(sAcademicYear, sUserType, sUserId);
                if (!string.IsNullOrEmpty(sRole))
                {
                    // Fetch Class
                    var liClasses = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(new STU_CLASS() { STUDENT_ID = sUserId },
                        SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchClassesbyUserId), sAcademicYear).DataSource.Data;

                    if (liClasses != null && liClasses.Count > 0)
                    {
                        objViewModel.CLASS_ID = new SelectList(liClasses, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                        objViewModel.ADMITTED_CLASS = new SelectList(liClasses, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }


                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FeedBackStudentToStaff", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FeedBackStudentToStaff", ex.Message);
                }
            }



            return View(objViewModel);
        }
        public async Task<string> StaffListBySubjectIdByAcademicInfo(string sCourseId)
        {
            List<FBStaffList> StaffList = new List<FBStaffList>();
            DataTable dtStaffList = new DataTable();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objFeedbackViewModel = new FeedbackViewModel();
            string option = "<option value='' >---Select Staff---</option>";
            try
            {
                dv.Clear();
                dv.Add("COURSE_ID", sCourseId, EnumCommand.DataType.String);
                dv.Add(Common.STU_PERSONAL_INFO.STUDENT_ID, (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "0", EnumCommand.DataType.String);
                resultArgs = await Task.Run(() => objFeedbackViewModel.FetchCourseWiseStaffList(dv, (sAcademicYear != null) ? sAcademicYear : ""));
                dtStaffList = resultArgs.DataSource.Table;
                if (dtStaffList != null && dtStaffList.Rows.Count > 0)
                {
                    Session[Common.SESSION_VARIABLES.FBCLASS_ID] = dtStaffList.Rows[0]["CLASS_ID"].ToString();
                }

                foreach (DataRow item in dtStaffList.Rows)
                {
                    option += "<option value='" + item["STAFF_ID"].ToString() + "' cms-class-id='" + item["CLASS_ID"].ToString() + "' >" + item["STAFF_NAME"] + "</option>";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "StaffListBySubjectId", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "StaffListBySubjectId", ex.Message);
                }
            }

            return option;
        }

        public async Task<string> StaffListByClassId(string sClassId)
        {
            List<FBStaffList> StaffList = new List<FBStaffList>();
            DataTable dtStaffList = new DataTable();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objFeedbackViewModel = new FeedbackViewModel();
            string option = "<option value='' >---Select Staff---</option>";
            try
            {
                dv.Clear();
                dv.Add("CLASS_ID", sClassId, EnumCommand.DataType.String);
                dv.Add(Common.STU_PERSONAL_INFO.STUDENT_ID, (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "0", EnumCommand.DataType.String);
                resultArgs = await Task.Run(() => objFeedbackViewModel.FetchClassWiseStaffInchargeList(dv, (sAcademicYear != null) ? sAcademicYear : ""));
                dtStaffList = resultArgs.DataSource.Table;
                if (dtStaffList != null && dtStaffList.Rows.Count > 0)
                {
                    Session[Common.SESSION_VARIABLES.FBCLASS_ID] = dtStaffList.Rows[0]["CLASS_ID"].ToString();
                }

                foreach (DataRow item in dtStaffList.Rows)
                {
                    option += "<option value='" + item["STAFF_ID"].ToString() + "' cms-class-id='" + item["CLASS_ID"].ToString() + "' >" + item["STAFF_NAME"] + "</option>";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "StaffListBySubjectId", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "StaffListBySubjectId", ex.Message);
                }
            }

            return option;
        }
        /// <summary>
        ///fetch hods by staff 
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult FeedBackStaffToHod()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                //fetch all programme
                var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                if (liProgramme != null && liProgramme.Count > 0)
                {
                    objViewModel.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                }
            }
            return View(objViewModel);
        }
        /// <summary>
        /// fetch principal 
        /// </summary>
        /// <returns></returns>
        public ActionResult FeedBackStaffToPrincipal()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                //fetch principal
                var liPrincipal = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchPrincipal), sAcademicYear).DataSource.Data;
                if (liPrincipal != null && liPrincipal.Count > 0)
                {
                    objViewModel.FeedbackMember = new SelectList(liPrincipal, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                }
            }
            return View(objViewModel);
        }

        public ActionResult FeedBackFacultyToDean()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                //fetch dean 
                var liDeen = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDeen), sAcademicYear).DataSource.Data;
                if (liDeen != null && liDeen.Count > 0)
                {
                    objViewModel.FeedbackMember = new SelectList(liDeen, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                }
            }
            return View(objViewModel);
        }
        /// <summary>
        /// fetch vice principal
        /// </summary>
        /// <returns></returns>
        public ActionResult FeedBackStaffToVicePrincipal()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                //fetch all programme
                var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                if (liProgramme != null && liProgramme.Count > 0)
                {
                    objViewModel.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                }
            }
            return View(objViewModel);
        }

        public ActionResult FeedBackHodToStaff()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                //fetch all programme
                var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                if (liProgramme != null && liProgramme.Count > 0)
                {
                    objViewModel.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                }
            }
            return View(objViewModel);
        }

        public ActionResult FeedBackPeerToPrincipal()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sUserId))
            {
                //fetch programme gorup by userid


                //fetch feedback member by user department
                var liMemebr = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO()
                {
                    STAFF_ID = sUserId
                },
                       SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStaffByPrincipalRole), sAcademicYear).DataSource.Data;
                if (liMemebr != null && liMemebr.Count > 0)
                {
                    objViewModel.FeedbackMember = new SelectList(liMemebr, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    objViewModel.Member = liMemebr.FirstOrDefault().STAFF_ID;

                }
            }
            return View(objViewModel);
        }

        public ActionResult FeedBackPeerToHod()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sUserId))
            {
                //fetch programme gorup by userid


                //fetch feedback member by user department
                var liMemebr = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO()
                {
                    STAFF_ID = sUserId
                },
                       SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStaffByHodRole)).DataSource.Data;
                if (liMemebr != null && liMemebr.Count > 0)
                {
                    objViewModel.FeedbackMember = new SelectList(liMemebr, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    objViewModel.Member = liMemebr.FirstOrDefault().STAFF_ID;

                }
            }
            return View(objViewModel);
        }

        public ActionResult FeedBackPeerToFaculty()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sUserId))
            {
                //fetch feedback member by user department
                var liMemebr = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO()
                {
                    STAFF_ID = sUserId
                },
                       SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStaffByStaffRole), sAcademicYear).DataSource.Data;
                if (liMemebr != null && liMemebr.Count > 0)
                {
                    objViewModel.FeedbackMember = new SelectList(liMemebr, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    objViewModel.Member = liMemebr.FirstOrDefault().STAFF_ID;

                }
            }
            return View(objViewModel);
        }

        public ActionResult FeedBackPeerToDean()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sUserId))
            {
                //fetch programme gorup by userid


                //fetch feedback member by user department
                var liMemebr = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO()
                {
                    STAFF_ID = sUserId
                },
                       SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStaffByDeanRole)).DataSource.Data;
                if (liMemebr != null && liMemebr.Count > 0)
                {
                    objViewModel.FeedbackMember = new SelectList(liMemebr, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                }
            }
            return View(objViewModel);
        }

        public string FetchHodByDepartment(string sDepartment)
        {
            string sOption = string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(sDepartment))
            {
                try
                {
                    var HodList = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new CP_PROGRAMME_2017() { PROGRAMME_GROUP_ID = sDepartment }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHodByProgrammeGroupId), sAcademicYear).DataSource.Data;
                    if (HodList != null && HodList.Count > 0)
                    {
                        foreach (var item in HodList)
                        {
                            sOption += "<option value=" + item.STAFF_ID + ">" + item.FIRST_NAME + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FeedbackController", "FetchHodByProgramme", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FeedbackController", "FetchHodByProgramme", ex.Message);
                    }
                }
            }
            return sOption;
        }

        public ActionResult FeedBackStaffSelfEntry()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sUserId))
            {
                //fetch programme gorup by userid



                var liMemebr = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO()
                {
                    STAFF_ID = sUserId
                },
                       SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStaffByStaffId), sAcademicYear).DataSource.Data;
                if (liMemebr != null && liMemebr.Count > 0)
                {
                    objViewModel.FeedbackMember = new SelectList(liMemebr, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    objViewModel.Member = liMemebr.FirstOrDefault().STAFF_ID;

                }
            }
            return View(objViewModel);
        }


        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        public ActionResult FeedBackStudentSelfEntry()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sUserId))
            {

                var FetchStudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(new Student_Personal_Info() { STUDENT_ID = sUserId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByUserId), sAcademicYear).DataSource.Data;
                if (FetchStudentInfo != null && FetchStudentInfo.Count > 0)
                {
                    objViewModel.FeedbackMember = new SelectList(FetchStudentInfo, Common.STU_PERSONAL_INFO.STUDENT_ID, Common.STU_PERSONAL_INFO.FIRST_NAME);
                    objViewModel.Member = FetchStudentInfo.FirstOrDefault().STUDENT_ID;

                }
            }
            return View(objViewModel);
        }

        /// <summary>
        /// this is based on fbclasses
        /// </summary>
        /// <returns></returns>

        public ActionResult FeedBackClassWiseFeedbackForStudents()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sUserId))
            {

                var FetchStudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(new Student_Personal_Info() { STUDENT_ID = sUserId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByUserId), sAcademicYear).DataSource.Data;
                if (FetchStudentInfo != null && FetchStudentInfo.Count > 0)
                {

                    //check if fbclasses is assigned by feedback and class_id
                    var check = (List<FBCLASSES>)CMSHandler.FetchData<FBCLASSES>(new FBCLASSES() { CLASS_ID = FetchStudentInfo.FirstOrDefault().CLASS_ID, FEEDBACK_ID = Session[Common.SESSION_VARIABLES.FEEDBACK_ID].ToString() }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbClassesByClassIdAndFeedbackId)).DataSource.Data;

                    if (check != null && check.Count > 0)
                    {
                        objViewModel.FeedbackMember = new SelectList(FetchStudentInfo, Common.STU_PERSONAL_INFO.STUDENT_ID, Common.STU_PERSONAL_INFO.FIRST_NAME);
                        objViewModel.Member = FetchStudentInfo.FirstOrDefault().STUDENT_ID;
                    }
                }
            }
            return View(objViewModel);
        }

        /// <summary>
        /// this is for feedback question view for feedback entry by assessor and assesse
        /// </summary>
        /// <param name="sAssessor"></param>
        /// <param name="sAssessee"></param>
        /// <returns></returns>
        public async Task<ActionResult> FeedBackQuestionView(string sFeedBackId, string sAssesee_role, string sAssesor_role, string sAssesee, string sSupFbRoleId, string sClassId, string sCourseId, bool isModern = false)
        {
            FeedbackViewModel objFeedback = new FeedbackViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                string sUserType = Session[Common.SESSION_VARIABLES.USER_TYPE] != null ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
                //fetch user role 
                string Assessor_role = GetUserRolesByAcademicYearandUserTypeandUserId(sAcademicYear, sUserType, sUserId);

                //  string sAssessor_role = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString();

                // string sAssessor_Name = !string.IsNullOrEmpty(Session[Common.SESSION_VARIABLES.USER_NAME].ToString()) ? Session[Common.SESSION_VARIABLES.USER_NAME].ToString() : string.Empty; ;
                DataTable dtObjectiveList = new DataTable();
                //DataTable dtQuestionList = new DataTable();
                List<FBOBJECTIVES> ObjectiveList = new List<FBOBJECTIVES>();
                List<FBFEEDBACK_QUESTIONS> QuestoinList = new List<FBFEEDBACK_QUESTIONS>();


                if (!string.IsNullOrEmpty(sUserId) && !string.IsNullOrEmpty(sAssesee) && !string.IsNullOrEmpty(sFeedBackId) && !string.IsNullOrEmpty(sSupFbRoleId))
                {
                    // validate if is attended already
                    //student to staff by class and course
                    if (!string.IsNullOrEmpty(sClassId) && !string.IsNullOrEmpty(sCourseId) && sClassId != "0" && sCourseId != "0")
                    {

                        var IsExist = (List<FBEVALUATION>)CMSHandler.FetchData<FBEVALUATION>(new FBEVALUATION() { FEEDBACK_ID = sFeedBackId, ASSESSOR = sUserId, ASSESSEE = sAssesee, COURSE_ID = sCourseId, SUP_FEEDBACK_ROLE_ID = sSupFbRoleId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.ValidateCourseByStudentId), sAcademicYear).DataSource.Data;
                        if (IsExist != null && IsExist.Count > 0)
                        {
                            objFeedback.sErrorMessage = "Already Attended!";
                            return PartialView(isModern ? "_ModernFeedbackQuestions" : "FeedBackQuestionView", objFeedback);
                        }
                    }
                    // other roles
                    else
                    {
                        if (ValidateFeedBackEvaluation(sFeedBackId, sUserId, sAssesee, sSupFbRoleId))
                        {
                            objFeedback.sErrorMessage = "Already Attended!";
                            return View(objFeedback);
                        }
                    }
                    //fetch objective
                    //dtObjectiveList = objFeedback.FetchObjective().DataSource.Table;
                    //ObjectiveList = await Task.Run(() => (from DataRow dr in dtObjectiveList.Rows
                    //                                      select new FBOBJECTIVES()
                    //                                      {
                    //                                          OBJECTIVESHORTTERM = dr[Common.FBOBJECTIVES.OBJECTIVESHORTTERM].ToString(),
                    //                                          OBJECTIVES = dr[Common.FBOBJECTIVES.OBJECTIVES].ToString(),
                    //                                          RATING = dr[Common.FBOBJECTIVES.RATING].ToString(),
                    //                                          OBJECTIVE_ID = dr[Common.FBOBJECTIVES.OBJECTIVE_ID].ToString()
                    //                                      }
                    //             ).ToList());

                    //fetch objective list by fbfeedbackquestion objective mapping
                    ObjectiveList = (List<FBOBJECTIVES>)CMSHandler.FetchData<FBOBJECTIVES>(new FBFEEDBACK_QUESTIONS() { FEEDBACK_ID = sFeedBackId, ASSESSOR_ROLE = sAssesor_role, ASSESSEE_ROLE = sAssesee_role, SUP_FEEDBACK_ROLE_ID = sSupFbRoleId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedBackQuestionObjectiveMappingByFeedbackAndAssessorAndAssesseeNew), sAcademicYear).DataSource.Data;
                    objFeedback.ObjectiveList = ObjectiveList;

                    //fetch question
                    var QuestionList = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_QUESTIONS() { FEEDBACK_ID = sFeedBackId, ASSESSOR_ROLE = sAssesor_role, ASSESSEE_ROLE = sAssesee_role, SUP_FEEDBACK_ROLE_ID = sSupFbRoleId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedBackQuestionViewByFeedbackAndAssessorAndAssesseeNew), sAcademicYear).DataSource.Data;
                    if (QuestionList != null && QuestionList.Count > 0)
                    {
                        objFeedback.QuestionList = QuestionList;
                    }
                    else
                    {
                        objFeedback.sErrorMessage = "No questions were added!";
                    }
                    //fetch assessee name
                    if (sAssesee_role == Common.SupRole.STUDENT)
                    {
                        var Student = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new STU_PERSONAL_INFO() { STUDENT_ID = sAssesee }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentInfo), sAcademicYear).DataSource.Data;
                        if (Student != null && Student.Count > 0)
                        {
                            objFeedback.AssesseeName = Student.FirstOrDefault().FIRST_NAME;
                            objFeedback.AssesseePhoto = Student.FirstOrDefault().PHOTO;
                        }
                    }
                    else
                    {
                        var Staff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO() { STAFF_ID = sAssesee }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoByStaffId)).DataSource.Data;
                        if (Staff != null && Staff.Count > 0)
                        {
                            objFeedback.AssesseeName = Staff.FirstOrDefault().FIRST_NAME;
                            objFeedback.AssesseePhoto = Staff.FirstOrDefault().PHOTO;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(sFeedBackId) && !string.IsNullOrEmpty(sUserId))
                    {
                        var IsExist = (List<FBEVALUATION>)CMSHandler.FetchData<FBEVALUATION>(new FBEVALUATION() { FEEDBACK_ID = sFeedBackId, ASSESSOR = sUserId, SUP_FEEDBACK_ROLE_ID = sSupFbRoleId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.ValidateCommonFeedbackStudentId), sAcademicYear).DataSource.Data;
                        if (IsExist != null && IsExist.Count > 0)
                        {
                            objFeedback.sErrorMessage = "Already Attended!";
                            return PartialView(isModern ? "_ModernFeedbackQuestions" : "FeedBackQuestionView", objFeedback);
                        }
                    }


                    ObjectiveList = (List<FBOBJECTIVES>)CMSHandler.FetchData<FBOBJECTIVES>(new FBFEEDBACK_QUESTIONS() { FEEDBACK_ID = sFeedBackId, ASSESSOR_ROLE = sAssesor_role, ASSESSEE_ROLE = sAssesee_role, SUP_FEEDBACK_ROLE_ID = sSupFbRoleId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedBackQuestionObjectiveMappingByFeedbackAndAssessorNew), sAcademicYear).DataSource.Data;
                    objFeedback.ObjectiveList = ObjectiveList;

                    //fetch question
                    var QuestionList = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_QUESTIONS() { FEEDBACK_ID = sFeedBackId, ASSESSOR_ROLE = sAssesor_role, ASSESSEE_ROLE = sAssesee_role, SUP_FEEDBACK_ROLE_ID = sSupFbRoleId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedBackQuestionViewByFeedbackAndAssessor), sAcademicYear).DataSource.Data;
                    if (QuestionList != null && QuestionList.Count > 0)
                    {
                        objFeedback.QuestionList = QuestionList;
                    }
                    else
                    {
                        objFeedback.sErrorMessage = "No questions were added!";
                    }
                    //fetch assessee name
                    if (sAssesee_role == Common.SupRole.STUDENT)
                    {
                        var Student = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new STU_PERSONAL_INFO() { STUDENT_ID = sAssesee }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentInfo), sAcademicYear).DataSource.Data;
                        if (Student != null && Student.Count > 0)
                        {
                            objFeedback.AssesseeName = Student.FirstOrDefault().FIRST_NAME;
                            objFeedback.AssesseePhoto = Student.FirstOrDefault().PHOTO;
                        }
                    }
                    else
                    {
                        var Staff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO() { STAFF_ID = sAssesee }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoByStaffId)).DataSource.Data;
                        if (Staff != null && Staff.Count > 0)
                        {
                            objFeedback.AssesseeName = Staff.FirstOrDefault().FIRST_NAME;
                            objFeedback.AssesseePhoto = Staff.FirstOrDefault().PHOTO;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FeedBackQuestionView", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FeedBackQuestionView", ex.Message);
                }
                return RedirectToAction("Index", "Account");
            }

            return PartialView(isModern ? "_ModernFeedbackQuestions" : "FeedBackQuestionView", objFeedback);
        }

        /// <summary>
        /// validate if already answered  based on feedback and assessor and assesse
        /// </summary>
        /// <param name="sFeedBack"></param>
        /// <param name="sAssessor"></param>
        /// <param name="sAsssesse"></param>
        /// <returns></returns>
        public bool ValidateFeedBackEvaluation(string sFeedBack, string sAssessor, string sAsssesse, string sSupFbRoleId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            var isExist = (List<FBEVALUATION>)CMSHandler.FetchData<FBEVALUATION>(new FBEVALUATION() { FEEDBACK_ID = sFeedBack, ASSESSOR = sAssessor, ASSESSEE = sAsssesse, SUP_FEEDBACK_ROLE_ID = sSupFbRoleId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedBackEvaluationByFeedBackAndAssessorAndAssessee), sAcademicYear).DataSource.Data;
            if (isExist != null && isExist.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string FetchStaffByProgramme(string sProgrammeId)
        {
            string sOption = string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(sProgrammeId))
            {
                try
                {
                    var StaffList = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new CP_PROGRAMME_GROUP() { PROGRAMME_GROUP_ID = sProgrammeId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStaffByProgrammeGroup), sAcademicYear).DataSource.Data;
                    if (StaffList != null && StaffList.Count > 0)
                    {
                        foreach (var item in StaffList)
                        {
                            sOption += "<option value=" + item.STAFF_ID + ">" + item.FIRST_NAME + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FeedbackController", "FetchStaffByProgramme", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FeedbackController", "FetchStaffByProgramme", ex.Message);
                    }
                }
            }
            return sOption;
        }

        /// <summary>
        /// for saving feedback answer
        /// </summary>
        /// <param name="sJsonData"></param>
        /// <param name="sFeedBackAnswer"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveFeedBackAnswer(string sJsonData, string sFeedBackAnswer)
        {
            JsonResultData objJsonData = new JsonResultData();
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sUserId))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<JSON_SAVE_QUESTION>(sFeedBackAnswer);
                    var Result1 = JsonConvert.DeserializeObject<FBEVALUATION>(sJsonData);
                    if (!string.IsNullOrEmpty(Result1.FEEDBACK_ID) && !string.IsNullOrEmpty(Result1.ASSESSEE))
                    {
                        //validate if it is already existed or not
                        //for student to staff
                        if (!string.IsNullOrEmpty(Result1.CLASS_ID) && !string.IsNullOrEmpty(Result1.COURSE_ID) && Result1.CLASS_ID != "0" && Result1.COURSE_ID != "0")
                        {
                            var IsExist = (List<FBEVALUATION>)CMSHandler.FetchData<FBEVALUATION>(new FBEVALUATION() { FEEDBACK_ID = Result1.FEEDBACK_ID, ASSESSOR = sUserId, ASSESSEE = Result1.ASSESSEE, COURSE_ID = Result1.COURSE_ID, SUP_FEEDBACK_ROLE_ID = Result1.SUP_FEEDBACK_ROLE_ID }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.ValidateCourseByStudentId), sAcademicYear).DataSource.Data;
                            if (IsExist != null && IsExist.Count > 0)
                            {
                                objJsonData.Message = Common.Messages.RecordsExistAlready;
                                return Json(objJsonData, JsonRequestBehavior.AllowGet);
                            }
                        }

                        // for other roles
                        else
                        {
                            if (ValidateFeedBackEvaluation(Result1.FEEDBACK_ID, sUserId, Result1.ASSESSEE, Result1.SUP_FEEDBACK_ROLE_ID))
                            {
                                objJsonData.Message = Common.Messages.RecordsExistAlready;
                                return Json(objJsonData, JsonRequestBehavior.AllowGet);
                            }

                        }
                        String sSQL = @"INSERT INTO FBEVALUATION" + sAcademicYear + "(ASSESSOR,CLASS_ID,COURSE_ID,FEEDBACK_ROLE_ID,ROLE,ASSESSEE,FEEDBACK_ID,QUESTION,ANSWER,ANSWER_IN_TEXT)VALUES";
                        foreach (var item in Result.liFB_Answer)
                        {
                            Result1.ASSESSOR = sUserId;
                            if (!string.IsNullOrEmpty(item.QUESTION))
                            {
                                if (!string.IsNullOrEmpty(item.ANSWER_IN_TEXT))
                                {
                                    sSQL += "('" + Result1.ASSESSOR + "','" + Result1.CLASS_ID + "','" + Result1.COURSE_ID + "','" + Result1.FEEDBACK_ROLE_ID + "','" + Result1.ROLE + "','" + Result1.ASSESSEE + "','" + Result1.FEEDBACK_ID + "','" + item.QUESTION + "','0','" + item.ANSWER_IN_TEXT + "'),";
                                }
                                if (!string.IsNullOrEmpty(item.ANSWER))
                                {
                                    sSQL += "('" + Result1.ASSESSOR + "','" + Result1.CLASS_ID + "','" + Result1.COURSE_ID + "','" + Result1.FEEDBACK_ROLE_ID + "','" + Result1.ROLE + "','" + Result1.ASSESSEE + "','" + Result1.FEEDBACK_ID + "','" + item.QUESTION + "','" + item.ANSWER + "','null'),";
                                }
                            }

                        }
                        sSQL = sSQL.Trim(',') + ";";
                        // sSQL = sSQL.Replace(Common.Delimiter.QUS+Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                        //  resultArgs = await Task.Run(() => objFeedback.SaveFeedback(sSQL));
                        resultArgs = objViewModel.SaveFeedback(sSQL);
                        //if (resultArgs.Success)
                        //{
                        //    resultArgs = CMSHandler.SaveOrUpdate(new FBFEEDBACK_ROLE_SETTINGS() { FEEDBACK_ROLE_ID = Result1.FEEDBACK_ROLE_ID, IS_ATTENDED = "1" }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.UpdateFeedBackRoleSettingIsAttendedByFeedbackRoleId), sAcademicYear);
                        //}
                        objJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;

                    }
                    else
                    {

                        var IsExist = (List<FBEVALUATION>)CMSHandler.FetchData<FBEVALUATION>(new FBEVALUATION() { FEEDBACK_ID = Result1.FEEDBACK_ID, ASSESSOR = sUserId, FEEDBACK_ROLE_ID = Result1.FEEDBACK_ROLE_ID }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.ValidateCommonFeedbackyStudentId), sAcademicYear).DataSource.Data;
                        if (IsExist != null && IsExist.Count > 0)
                        {
                            objJsonData.Message = Common.Messages.RecordsExistAlready;
                            return Json(objJsonData, JsonRequestBehavior.AllowGet);
                        }
                        String sSQL=string.Empty;
                        var classList = (List<STUDENT_PERSONAL_INFO>)CMSHandler.FetchData<STUDENT_PERSONAL_INFO>(new FBEVALUATION() { STUDENT_ID = sUserId, ACADEMIC_YEAR = sAcademicYear }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchclassIdbyStudentAndAcademicYear), sAcademicYear).DataSource.Data;
                        if (classList != null && classList.Count > 0)
                        {
                             sSQL = @"INSERT INTO FBEVALUATION" + sAcademicYear + "(ASSESSOR,CLASS_ID,COURSE_ID,FEEDBACK_ROLE_ID,ROLE,ASSESSEE,FEEDBACK_ID,QUESTION,ANSWER,ANSWER_IN_TEXT)VALUES";
                            foreach (var item in Result.liFB_Answer)
                            {
                                Result1.ASSESSOR = sUserId;
                                Result1.ASSESSEE = sUserId;
                                if (!string.IsNullOrEmpty(item.QUESTION))
                                {
                                    if (!string.IsNullOrEmpty(item.ANSWER_IN_TEXT))
                                    {
                                        sSQL += "('" + Result1.ASSESSOR + "','" + classList.FirstOrDefault().CLASS_ID + "','" + 0 + "','" + Result1.FEEDBACK_ROLE_ID + "','" + Result1.ROLE + "','" + Result1.ASSESSEE + "','" + Result1.FEEDBACK_ID + "','" + item.QUESTION + "','0','" + item.ANSWER_IN_TEXT + "'),";
                                    }
                                    if (!string.IsNullOrEmpty(item.ANSWER))
                                    {
                                        sSQL += "('" + Result1.ASSESSOR + "','" + classList.FirstOrDefault().CLASS_ID + "','" + 0 + "','" + Result1.FEEDBACK_ROLE_ID + "','" + Result1.ROLE + "','" + Result1.ASSESSEE + "','" + Result1.FEEDBACK_ID + "','" + item.QUESTION + "','" + item.ANSWER + "','null'),";
                                    }
                                }

                            }
                        }
                        sSQL = sSQL.Trim(',') + ";";

                        resultArgs = objViewModel.SaveFeedback(sSQL);

                        objJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "SaveFeedBackAnswer", ex.Message);
                    }
                }
            }
            else
            {
                objJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(objJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Feedback Report

        public ActionResult FeedbackReport()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            try
            {

                string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);

                //fetch academic year
                var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    objViewModel.AcademicYearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);

                // feed back
                var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchStudentFeedbackSettingList), sAcademicYear).DataSource.Data;
                if (fbSetting != null && fbSetting.Count > 0)
                {
                    objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                }

                //class year
                var FetchClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear), sAcademicYear).DataSource.Data;
                if (FetchClassYear != null && FetchClassYear.Count > 0)
                    objViewModel.ClassYearList = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.CP_CLASSES_2017.CLASS_YEAR);


                var liApplicationType = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType)).DataSource.Data;
                // fetch application type
                if (liApplicationType != null && liApplicationType.Count > 0)
                {
                    objViewModel.ApplicationType = new SelectList(liApplicationType, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                }

                //fetch all programme
                var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme), sAcademicYear).DataSource.Data;
                if (liProgramme != null && liProgramme.Count > 0)
                {
                    objViewModel.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                }
                var liShift = (List<Sup_Shift>)CMS.DAO.CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                if (liShift != null && liShift.Count > 0)
                {
                    objViewModel.ShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }
                var liDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null,
                           SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                if (liDepartment != null && liDepartment.Count > 0)
                {
                    objViewModel.DepartmentList = new SelectList(liDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FeedbackReport", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FeedbackReport", ex.Message);
                }
                // return RedirectToAction("FeedbackReport", "Account");
            }
            return View(objViewModel);
        }

        public ActionResult FeedbackReportForStaff()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
                // feed back
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                    if (fbSetting != null && fbSetting.Count > 0)
                    {
                        objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                    }
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FeedbackReportForStaff", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FeedbackReportForStaff", ex.Message);
                }
                // return RedirectToAction("FeedbackReport", "Account");
            }
            return View(objViewModel);
        }

        public ActionResult BindFeedbackReport(string sStaffId, string sFeedbackId)
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            string sUserRole = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString();
            string StaffId = string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                if (!string.IsNullOrEmpty(sFeedbackId) && !string.IsNullOrEmpty(sStaffId) && sStaffId.Equals("staff"))
                {
                    StaffId = sUserId;
                }
                else if (!string.IsNullOrEmpty(sFeedbackId) && !string.IsNullOrEmpty(sStaffId))
                {
                    StaffId = sStaffId;
                }
                //fetch question count staff by student 
                //var QuestionCountByStaffByStudent = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_ROLE_SETTINGS()
                //{
                //    FEEDBACK_ID = sFeedbackId,
                //    SUP_FEEDBACK_ROLE_ID = Common.FBFEEDBACK_ROLE.STAFF_BY_STUDENT
                //}, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionCountByFeedbackAndSupFeedbackRole)).DataSource.Data;

                //if (QuestionCountByStaffByStudent != null && QuestionCountByStaffByStudent.Count > 0 && !string.IsNullOrEmpty(QuestionCountByStaffByStudent.FirstOrDefault().QUESTION_COUNT))
                //{
                //    objViewModel.objFeedbackQuestionFiled.QCSTAFF_BYSTUDENT = QuestionCountByStaffByStudent.FirstOrDefault().QUESTION_COUNT;
                //}

                // fetch no of student count in evaluation 

                var CountOfStudent = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_ROLE_SETTINGS()
                {
                    FEEDBACK_ID = sFeedbackId,
                    SUP_FEEDBACK_ROLE_ID = Common.FBFEEDBACK_ROLE.STAFF_BY_STUDENT,
                    ASSESSE = StaffId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchTotalUserCountOfEvaluationBySupFeedbackRoleId), sAcademicYear).DataSource.Data;

                if (CountOfStudent != null && CountOfStudent.Count > 0 && !string.IsNullOrEmpty(CountOfStudent.FirstOrDefault().TOTAL))
                {
                    objViewModel.objFeedbackQuestionFiled.NO_OF_STUDENT = CountOfStudent.FirstOrDefault().TOTAL;
                }

                //fetch question count staff by peer 
                //var QuestionCountByStaffByPeer = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_ROLE_SETTINGS()
                //{
                //    FEEDBACK_ID = sFeedbackId,
                //    SUP_FEEDBACK_ROLE_ID = Common.FBFEEDBACK_ROLE.STAFF_BY_PEER
                //}, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionCountByFeedbackAndSupFeedbackRole)).DataSource.Data;

                //if (QuestionCountByStaffByPeer != null && QuestionCountByStaffByPeer.Count > 0 && !string.IsNullOrEmpty(QuestionCountByStaffByPeer.FirstOrDefault().QUESTION_COUNT))
                //{
                //    objViewModel.objFeedbackQuestionFiled.QCSTAFF_BYPEER = QuestionCountByStaffByPeer.FirstOrDefault().QUESTION_COUNT;
                //}

                // fetch no of staff evalauted as peer

                var CountOfPeer = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_ROLE_SETTINGS()
                {
                    FEEDBACK_ID = sFeedbackId,
                    SUP_FEEDBACK_ROLE_ID = Common.FBFEEDBACK_ROLE.STAFF_BY_PEER,
                    ASSESSE = StaffId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchTotalUserCountOfEvaluationBySupFeedbackRoleId), sAcademicYear).DataSource.Data;

                if (CountOfPeer != null && CountOfPeer.Count > 0 && !string.IsNullOrEmpty(CountOfPeer.FirstOrDefault().TOTAL))
                {
                    objViewModel.objFeedbackQuestionFiled.NO_OF_STAFF = CountOfPeer.FirstOrDefault().TOTAL;
                }


                // fetch no of hod evalauted as hod

                var CountOfHod = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_ROLE_SETTINGS()
                {
                    FEEDBACK_ID = sFeedbackId,
                    SUP_FEEDBACK_ROLE_ID = Common.FBFEEDBACK_ROLE.STAFF_BY_HOD,
                    ASSESSE = StaffId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchTotalUserCountOfEvaluationBySupFeedbackRoleId), sAcademicYear).DataSource.Data;

                if (CountOfHod != null && CountOfHod.Count > 0 && !string.IsNullOrEmpty(CountOfHod.FirstOrDefault().TOTAL))
                {
                    objViewModel.objFeedbackQuestionFiled.NO_OF_HOD = CountOfHod.FirstOrDefault().TOTAL;
                }


                // fetch no of dean evalauted as DEAN

                var CountOfDean = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_ROLE_SETTINGS()
                {
                    FEEDBACK_ID = sFeedbackId,
                    SUP_FEEDBACK_ROLE_ID = Common.FBFEEDBACK_ROLE.STAFF_BY_DEEN,
                    ASSESSE = StaffId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchTotalUserCountOfEvaluationBySupFeedbackRoleId), sAcademicYear).DataSource.Data;

                if (CountOfDean != null && CountOfDean.Count > 0 && !string.IsNullOrEmpty(CountOfDean.FirstOrDefault().TOTAL))
                {
                    objViewModel.objFeedbackQuestionFiled.NO_OF_DEAN = CountOfDean.FirstOrDefault().TOTAL;
                }

                // fetch question count staff by hod
                //var QuestionCountByStaffByHod = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_ROLE_SETTINGS()
                //{
                //    FEEDBACK_ID = sFeedbackId,
                //    SUP_FEEDBACK_ROLE_ID = Common.FBFEEDBACK_ROLE.STAFF_BY_HOD
                //}, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionCountByFeedbackAndSupFeedbackRole)).DataSource.Data;

                //if (QuestionCountByStaffByHod != null && QuestionCountByStaffByHod.Count > 0 && !string.IsNullOrEmpty(QuestionCountByStaffByHod.FirstOrDefault().QUESTION_COUNT))
                //{
                //    objViewModel.objFeedbackQuestionFiled.QCSTAFF_BYHOD = QuestionCountByStaffByHod.FirstOrDefault().QUESTION_COUNT;
                //}

                // fetch question count staff by principal
                //var QuestionCountByStaffByPrincipal = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_ROLE_SETTINGS()
                //{
                //    FEEDBACK_ID = sFeedbackId,
                //    SUP_FEEDBACK_ROLE_ID = Common.FBFEEDBACK_ROLE.STAFF_BY_PRINCIPAL
                //}, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionCountByFeedbackAndSupFeedbackRole)).DataSource.Data;

                //if (QuestionCountByStaffByPrincipal != null && QuestionCountByStaffByPrincipal.Count > 0 && !string.IsNullOrEmpty(QuestionCountByStaffByPrincipal.FirstOrDefault().QUESTION_COUNT))
                //{
                //    objViewModel.objFeedbackQuestionFiled.QCSTAFF_BYPRINCIPAL = QuestionCountByStaffByPrincipal.FirstOrDefault().QUESTION_COUNT;
                //}

                // fetch question count staff by dean
                //var QuestionCountByStaffByDean =(List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_ROLE_SETTINGS()
                //{
                //    FEEDBACK_ID = sFeedbackId,
                //    SUP_FEEDBACK_ROLE_ID = Common.FBFEEDBACK_ROLE.STAFF_BY_DEEN
                //}, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionCountByFeedbackAndSupFeedbackRole)).DataSource.Data;

                //if (QuestionCountByStaffByDean != null && QuestionCountByStaffByDean.Count > 0 && !string.IsNullOrEmpty(QuestionCountByStaffByDean.FirstOrDefault().QUESTION_COUNT))
                //{
                //    objViewModel.objFeedbackQuestionFiled.QCSTAFF_BYDEEN = QuestionCountByStaffByDean.FirstOrDefault().QUESTION_COUNT;
                //}

                //fetch feedback evaluation 
                objViewModel.liFeedbackEvaluationForReport = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                {
                    ASSESSEE = StaffId,
                    FEEDBACK_ID = sFeedbackId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackEvaluationForRepoprtByFeedbackandStaff), sAcademicYear).DataSource.Data;

                //fetch feedback faculty performance index
                objViewModel.liFeedbackPerformanceIndexForReport = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                {
                    ASSESSEE = StaffId,
                    FEEDBACK_ID = sFeedbackId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackPerformanceIndexEvaluationForRepoprtByFeedbackandStaff), sAcademicYear).DataSource.Data;


                //fetch feedback faculty self appraisal
                objViewModel.liFeedbackSelfAppraisalForReport = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                {
                    ASSESSEE = StaffId,
                    FEEDBACK_ID = sFeedbackId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackSelfAppraisalEvaluationForRepoprtByFeedbackandStaff), sAcademicYear).DataSource.Data;

                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
            }
            else
            {
                objViewModel.sErrorMessage = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return View(objViewModel);
        }


        public ActionResult ClassWiseFeedbackReportForStaff()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            try
            {

                string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                string sUserType = Session[Common.SESSION_VARIABLES.USER_TYPE] != null ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
                // feed back
                var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                if (fbSetting != null && fbSetting.Count > 0)
                {
                    objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                }

                var liClasses = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null,
                        SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchClassesbyUserId1), sAcademicYear).DataSource.Data;

                if (liClasses != null && liClasses.Count > 0)
                {
                    objViewModel.CLASS_ID = new SelectList(liClasses, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    objViewModel.ADMITTED_CLASS = new SelectList(liClasses, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FeedbackReportForStaff", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FeedbackReportForStaff", ex.Message);
                }
                // return RedirectToAction("FeedbackReport", "Account");
            }
            return View(objViewModel);
        }

        public ActionResult BindclassWiseFeedbackReportForStaff(string sStaffId, string sFeedbackId, string sClassId)
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            string sUserRole = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString();
            string StaffId = string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                if (!string.IsNullOrEmpty(sFeedbackId) && !string.IsNullOrEmpty(sStaffId) && sStaffId.Equals("staff"))
                {
                    StaffId = sUserId;
                }
                else if (!string.IsNullOrEmpty(sFeedbackId) && !string.IsNullOrEmpty(sStaffId))
                {
                    StaffId = sStaffId;
                }

                var LiNotEntryList = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                {
                    CLASS_ID = sClassId,
                    FEEDBACK_ID = sFeedbackId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackNotEntryStudents), sAcademicYear).DataSource.Data;
                if (LiNotEntryList != null && LiNotEntryList.Count > 0)
                {
                    objViewModel.liFeedbackEvaluationForReport = LiNotEntryList;
                }

                //if (!string.IsNullOrEmpty(sCourseId))
                //{
                //    var LiNotEntryList = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                //    {
                //        CLASS_ID = sClassId,
                //        FEEDBACK_ID = sFeedbackId,
                //        COURSE_ID = sCourseId
                //    }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackNotEntryStudents1), sAcademicYear).DataSource.Data;
                //    if (LiNotEntryList != null && LiNotEntryList.Count > 0)
                //    {
                //        objViewModel.liFeedbackEvaluationForReport = LiNotEntryList;
                //    }
                //}
                //else {
                //    var LiNotEntryList = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                //    {
                //        CLASS_ID = sClassId,
                //        FEEDBACK_ID = sFeedbackId
                //    }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackNotEntryStudents), sAcademicYear).DataSource.Data;
                //    if (LiNotEntryList != null && LiNotEntryList.Count > 0)
                //    {
                //        objViewModel.liFeedbackEvaluationForReport = LiNotEntryList;
                //    }
                //}


            }
            else
            {
                objViewModel.sErrorMessage = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return View(objViewModel);
        }

        public ActionResult CourseBasedFeedbackReportForStaff()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            try
            {

                string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                string sUserType = Session[Common.SESSION_VARIABLES.USER_TYPE] != null ? Session[Common.SESSION_VARIABLES.USER_TYPE].ToString() : string.Empty;
                // feed back
                var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                if (fbSetting != null && fbSetting.Count > 0)
                {
                    objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                }

                var liClasses = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null,
                        SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchClassesbyUserId2), sAcademicYear).DataSource.Data;

                if (liClasses != null && liClasses.Count > 0)
                {
                    objViewModel.CLASS_ID = new SelectList(liClasses, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    objViewModel.ADMITTED_CLASS = new SelectList(liClasses, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FeedbackReportForStaff", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FeedbackReportForStaff", ex.Message);
                }
                // return RedirectToAction("FeedbackReport", "Account");
            }
            return View(objViewModel);
        }

        public ActionResult BindCourseBasedFeedbackReportForStaff(string sStudentId, string sFeedbackId, string sClassId)
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            string sUserRole = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString();
            string StudentId = string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new SaveNMERegisteredStudent();
                var sStudentIds = sStudentId.Split(',');
                foreach (string item in sStudentIds)
                {
                    string sSQL = SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchStudentListFeedBack).Replace(Common.Delimiter.QUS + Common.FBSTUDENT_ENTRY.STUDENT_ID, item);
                    var liRegularCourses = (List<FEEDBACK_REPORT_FIELDS>)CMS.DAO.CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FEEDBACK_REPORT_FIELDS() { AC_YEAR = sAcademicYear }, sSQL, sAcademicYear).DataSource.Data;
                    // Check Registration Exist ...

                    //var liRegularCourses1 = (List<FEEDBACK_REPORT_FIELDS>)CMS.DAO.CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new STU_CLASS() { STUDENT_ID = item },
                    //SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchStudentListFeedBack), sAcademicYear).DataSource.Data;

                    //var liOptionalCourses = (List<FEEDBACK_REPORT_FIELDS>)DAO.CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new STU_CLASS() { STUDENT_ID = item },
                    //SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;
                    ////Loop Optional Papers  To Remove Unnecessary Courses...
                    //if (liOptionalCourses != null && liOptionalCourses.Count > 0)
                    //{
                    //    foreach (var item1 in liOptionalCourses)
                    //    {
                    //        var IsExist = liRegularCourses.Where(o => o.COURSE_ROOT_ID == item1.COURSE_ROOT_ID).ToList();
                    //        if (IsExist != null && IsExist.Count > 0)
                    //        {
                    //            liRegularCourses.RemoveAll(o => o.COURSE_STRUCTURE_ID == IsExist.FirstOrDefault().COURSE_STRUCTURE_ID);
                    //            item1.COURSE_STRUCTURE_ID = IsExist.FirstOrDefault().COURSE_STRUCTURE_ID;
                    //            liRegularCourses.Add(item1);
                    //        }
                    //    }
                    //}



                    objViewModel.liFeedbackEvaluationForReport = liRegularCourses;
                    //foreach (var item2 in liRegularCourses)
                    //{
                    //    string course_id = item2.COURSE_ROOT_ID;
                    //    var LiNotEntryList = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                    //    {
                    //        CLASS_ID = sClassId,
                    //        FEEDBACK_ID = sFeedbackId,
                    //        COURSE_ID = course_id
                    //    }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackEntryStudents), sAcademicYear).DataSource.Data;
                    //    if (LiNotEntryList != null && LiNotEntryList.Count > 0)
                    //    {
                    //        objViewModel.liFeedbackEvaluationForReport = LiNotEntryList;
                    //    }
                    //}

                }

            }
            else
            {
                objViewModel.sErrorMessage = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return View(objViewModel);
        }


        public JsonResult GetStudentAndClassList(string sClassId, string sAcYear, string sSemesterId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                // Fetch Student By Class Id ...
                var liStudentList = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new STU_CLASS() { CLASS_ID = sClassId },
                      // SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.FetchNMENotRegisteredStudentForDDL), sAcademicYear).DataSource.Data;
                      SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackClassWiseStudentsList), sAcademicYear).DataSource.Data;
                if (liStudentList != null && liStudentList.Count > 0)
                {
                    foreach (var item in liStudentList)
                    {
                        ObjJsonData.sResult += "<option value=" + item.STUDENT_ID + "> " + item.FIRST_NAME + " </option>";
                    }
                }

                //// Fetch NME Course By Class Id ...
                //string sSQL = SQL.NME.NMESQL.GetNMESQL(NMESQLCommands.LoadNMECourseList).Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sSemesterId);
                //var liCourseList = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(new STU_CLASS() { CLASS_ID = sClassId }, sSQL, sAcYear).DataSource.Data;
                ////var liCourseList = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(new STU_CLASS() {CLASS_ID=sClassId },
                ////      SQL.Student.StudentSQL.GetStudentSQL(StudentSQLCommads.FetchOptionalCoursebyShiftIdAndClassId),sAcademicYear).DataSource.Data;
                //if (liCourseList != null && liCourseList.Count > 0)
                //{
                //    ObjJsonData.ErrorCode = "<option value=0> -- Select -- </option>";
                //    foreach (var item in liCourseList)
                //    {
                //        ObjJsonData.ErrorCode += "<option value=" + item.COURSE_ROOT_ID + " cms-Semster=" + item.SEMESTER_ID + " cms-sClassId=" + item.NME_CLASS_ID + " cms-SettingsId=" + item.SETTINGS_ID + " " + item.QUOTA + ">" + item.COURSE_TITLE + "</option>";
                //    }
                //}
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FeedbackReportForPrincipal()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            try
            {

                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                // feed back
                var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                if (fbSetting != null && fbSetting.Count > 0)
                {
                    objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FeedbackReportForPrincipal", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FeedbackReportForPrincipal", ex.Message);
                }
                // return RedirectToAction("FeedbackReport", "Account");
            }
            return View(objViewModel);
        }

        public ActionResult BindFeedbackReportForPrincipal(string sFeedbackId)
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            //  string StaffId = string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sFeedbackId))
            {
                //fetch feedback evaluation 
                objViewModel.liFeedbackEvaluationForReport = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                {
                    ASSESSEE = sUserId,
                    FEEDBACK_ID = sFeedbackId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackEvaluationForRepoprtByFeedbackandPrincipal), sAcademicYear).DataSource.Data;


                //fetch feedback faculty performance index
                objViewModel.liFeedbackPerformanceIndexForReport = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                {
                    ASSESSEE = sUserId,
                    FEEDBACK_ID = sFeedbackId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackPerformanceIndexEvaluationForRepoprtByFeedbackandStaff), sAcademicYear).DataSource.Data;


                //fetch feedback faculty self appraisal
                objViewModel.liFeedbackSelfAppraisalForReport = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                {
                    ASSESSEE = sUserId,
                    FEEDBACK_ID = sFeedbackId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackSelfAppraisalEvaluationForRepoprtByFeedbackandStaff), sAcademicYear).DataSource.Data;

                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
            }
            else
            {
                objViewModel.sErrorMessage = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return View(objViewModel);
        }

        public ActionResult FeedbackReportForDean()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            try
            {

                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                // feed back
                var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                if (fbSetting != null && fbSetting.Count > 0)
                {
                    objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FeedbackReportForDean", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FeedbackReportForDean", ex.Message);
                }
                // return RedirectToAction("FeedbackReport", "Account");
            }
            return View(objViewModel);
        }
        public ActionResult BindFeedbackReportForDean(string sFeedbackId)
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            //  string StaffId = string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sFeedbackId))
            {
                //fetch feedback evaluation 
                objViewModel.liFeedbackEvaluationForReport = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                {
                    ASSESSEE = sUserId,
                    FEEDBACK_ID = sFeedbackId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackEvaluationForRepoprtByFeedbackandDean), sAcademicYear).DataSource.Data;


                //fetch feedback faculty performance index
                objViewModel.liFeedbackPerformanceIndexForReport = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                {
                    ASSESSEE = sUserId,
                    FEEDBACK_ID = sFeedbackId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackPerformanceIndexEvaluationForRepoprtByFeedbackandStaff), sAcademicYear).DataSource.Data;


                //fetch feedback faculty self appraisal
                objViewModel.liFeedbackSelfAppraisalForReport = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                {
                    ASSESSEE = sUserId,
                    FEEDBACK_ID = sFeedbackId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackSelfAppraisalEvaluationForRepoprtByFeedbackandStaff), sAcademicYear).DataSource.Data;

                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
            }
            else
            {
                objViewModel.sErrorMessage = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return View(objViewModel);
        }

        public ActionResult FeedbackReportForHod()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            try
            {

                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                // feed back
                var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                if (fbSetting != null && fbSetting.Count > 0)
                {
                    objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FeedbackReportForHod", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FeedbackReportForHod", ex.Message);
                }
                // return RedirectToAction("FeedbackReport", "Account");
            }
            return View(objViewModel);
        }
        public ActionResult BindFeedbackReportForHod(string sFeedbackId)
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            //  string StaffId = string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sFeedbackId))
            {
                //fetch feedback evaluation 
                objViewModel.liFeedbackEvaluationForReport = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                {
                    ASSESSEE = sUserId,
                    FEEDBACK_ID = sFeedbackId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackEvaluationForRepoprtByFeedbackandHod), sAcademicYear).DataSource.Data;


                //fetch feedback faculty performance index
                objViewModel.liFeedbackPerformanceIndexForReport = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                {
                    ASSESSEE = sUserId,
                    FEEDBACK_ID = sFeedbackId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackPerformanceIndexEvaluationForRepoprtByFeedbackandStaff), sAcademicYear).DataSource.Data;


                //fetch feedback faculty self appraisal
                objViewModel.liFeedbackSelfAppraisalForReport = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(new FBEVALUATION()
                {
                    ASSESSEE = sUserId,
                    FEEDBACK_ID = sFeedbackId
                }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackSelfAppraisalEvaluationForRepoprtByFeedbackandStaff), sAcademicYear).DataSource.Data;

                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
            }
            else
            {
                objViewModel.sErrorMessage = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return View(objViewModel);
        }

        public ActionResult FeedbackReportClassCourseWiseforStaff()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

                    // feed back
                    var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                    if (fbSetting != null && fbSetting.Count > 0)
                    {
                        objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                    }

                    var liClassList = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(new CourseWiseStaffList() { STAFF_ID = sUserId },
                         ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchClassList), sAcademicYear).DataSource.Data;
                    if (liClassList != null && liClassList.Count > 0)
                    {
                        objViewModel.ClassList = new SelectList(liClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FeedbackReportClassCourseWiseforStaff", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FeedbackReportClassCourseWiseforStaff", ex.Message);
                }
                return RedirectToAction("Index", "Account");
            }
            return View(objViewModel);
        }
        public ActionResult BindClassCourseFeedbackReport(string sFbSettingId, string sClassId, string sCourseId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                try
                {
                    var ObjModel = new FBEVALUATION();
                    ObjModel.FEEDBACK_ID = sFbSettingId;
                    ObjModel.ASSESSEE = sUserId;
                    ObjModel.COURSE_ID = sCourseId;
                    ObjModel.CLASS_ID = sClassId;

                    //Fetch Feedback Role setting
                    var Fetchrolesetting = (List<FBFEEDBACK_ROLE_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_ROLE_SETTINGS>(new FBFEEDBACK_ROLE_SETTINGS() { SUP_FEEDBACK_ROLE_ID = Common.FBFEEDBACK_ROLE.STAFF_BY_STUDENT, FEEDBACK_ID = sFbSettingId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchfeedbackroleId)).DataSource.Data;
                    if (Fetchrolesetting != null && Fetchrolesetting.Count > 0)
                    {
                        ObjModel.FEEDBACK_ROLE_ID = Fetchrolesetting.FirstOrDefault().FEEDBACK_ROLE_ID;
                        // Fetch All Questions By FeedBack Id ,...
                        ObjViewModel.QuestionList = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionsByFeedBackIdandRoleId), sAcademicYear).DataSource.Data;
                    }
                    // Fetch Students Count Answered For Each Questions By Class & Course ,...
                    ObjViewModel.StudentCount = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCountByFBQuestion), sAcademicYear).DataSource.Data;

                    // Fetch Total Student Strength By Class & Course ,...
                    ObjViewModel.liStudentStrength = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCountByClassAndCourseId), sAcademicYear).DataSource.Data;


                    // Fetch College Details ,...
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "BindFeedBackReport", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(ObjViewModel);
        }

        public string FetchClassByDepartmentId(string sDepartmentId)
        {
            string sOption = string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(sDepartmentId))
            {
                try
                {
                    string sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByDepartmentIds).Replace(Common.Delimiter.QUS + Common.CP_DEPARTMENT_2017.DEPARTMENT, sDepartmentId);
                    var ClassList = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, sSQL).DataSource.Data;

                    if (ClassList != null && ClassList.Count > 0)
                    {
                        foreach (var item in ClassList)
                        {
                            sOption += "<option value=" + item.CLASS_ID + ">" + item.CLASS_NAME + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FeedbackController", "FetchClassByDepartmentId", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FeedbackController", "FetchClassByDepartmentId", ex.Message);
                    }
                }
            }
            return sOption;
        }

        public string FetchFeedbackByAcademicYear(string sAcademicYear)
        {
            string sOption = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liFeedback = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(new ACADEMIC_YEAR() { ACADEMIC_YEAR_ID = sAcademicYear }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedBackSettingsByAcademicYear), sAcademicYear).DataSource.Data;

                    if (liFeedback != null && liFeedback.Count > 0)
                    {
                        foreach (var item in liFeedback)
                        {
                            sOption += "<option value=" + item.FEEDBACK_ID + ">" + item.FEEDBACK_NAME + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FeedbackController", "FetchFeedbackByAcademicYear", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FeedbackController", "FetchFeedbackByAcademicYear", ex.Message);
                    }
                }
            }
            return sOption;
        }
        public string FetchStudentFeedbackByAcademicYear(string sAcademicYear)
        {
            string sOption = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liFeedback = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(new ACADEMIC_YEAR() { ACADEMIC_YEAR_ID = sAcademicYear }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchStudentFeedBackSettingsByAcademicYear), sAcademicYear).DataSource.Data;

                    if (liFeedback != null && liFeedback.Count > 0)
                    {
                        foreach (var item in liFeedback)
                        {
                            sOption += "<option value=" + item.FEEDBACK_ID + ">" + item.FEEDBACK_NAME + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FeedbackController", "FetchFeedbackByAcademicYear", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FeedbackController", "FetchFeedbackByAcademicYear", ex.Message);
                    }
                }
            }
            return sOption;
        }

        public ActionResult BindFeedBackReportForStudent(string sFbSettingId, string sClassId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sFbSettingId) && !string.IsNullOrEmpty(sClassId))
            {
                try
                {
                    var ObjModel = new FBEVALUATION();
                    ObjModel.FEEDBACK_ID = sFbSettingId;
                    ObjModel.CLASS_ID = sClassId;
                    var liacyear = (List<FBFEEDBACK_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(new FBFEEDBACK_SETTINGS() { FEEDBACK_ID = sFbSettingId },
                        SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackYearBySettingId)).DataSource.Data;
                    // Fetch Evaluation By FeedBack Id and class id ,...
                    string sSql = SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbEvaluationInfoForReportByClasswiseandAcYear);

                    ObjViewModel.LiEvaluation = (List<FBEVALUATION>)CMSHandler.FetchData<FBEVALUATION>(ObjModel, sSql.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassId), liacyear.FirstOrDefault().ACADEMIC_YEAR).DataSource.Data;

                    if (ObjViewModel.LiEvaluation != null && ObjViewModel.LiEvaluation.Count > 0)
                    {
                        //move only questions in this list
                        var liQuestion = ObjViewModel.LiEvaluation.GroupBy(x => x.QUESTION_ID).Select(s => s.First()).ToList();
                        if (liQuestion != null && liQuestion.Count > 0)
                        {
                            ObjViewModel.QuestionList = (from s in liQuestion select new FBFEEDBACK_QUESTIONS() { QUESTION_ID = s.QUESTION_ID, QUESTION = s.QUESTION }).ToList();
                        }

                    }


                    // Fetch Feedback question mapping by feedback id
                    ObjViewModel.LiFeedbackQuestionMapping = (List<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING>)CMSHandler.FetchData<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbFeedbackQuestionsMappingByFeedbackId), sAcademicYear).DataSource.Data;

                    // Fetch College Details ,...
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "BindFeedBackReportForStudent", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(ObjViewModel);
        }

        public ActionResult BindFeedBackReportForStaff(string sFbSettingId, string sStaffId, string sAcYear)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            var ObjJsonData = new JsonResultData();
            //  var sAcademicYear = sAcYear;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sFbSettingId) && !string.IsNullOrEmpty(sStaffId))
            {
                try
                {
                    var ObjModel = new FBEVALUATION();
                    ObjModel.FEEDBACK_ID = sFbSettingId;
                    ObjModel.ASSESSEE = sStaffId;


                    var liacyear = (List<FBFEEDBACK_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(new FBFEEDBACK_SETTINGS() { FEEDBACK_ID = sFbSettingId },
                        SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackYearBySettingId)).DataSource.Data;

                    // Fetch Total Student Strength By Class & Course ,...
                    //ObjViewModel.liStudentStrength = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCountByClassAndCourseId), sAcademicYear).DataSource.Data;

                    // Fetch Evaluation By FeedBack Id and class id ,...
                    string sSql = SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbEvaluationInfoForReportByStaffwiseandAcYear);
                    sSql = sSql.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, liacyear.FirstOrDefault().ACADEMIC_YEAR);
                    ObjViewModel.LiEvaluation = (List<FBEVALUATION>)CMSHandler.FetchData<FBEVALUATION>(ObjModel, sSql, liacyear.FirstOrDefault().ACADEMIC_YEAR).DataSource.Data;

                    if (ObjViewModel.LiEvaluation != null && ObjViewModel.LiEvaluation.Count > 0)
                    {
                        //move only questions in this list
                        var liQuestion = ObjViewModel.LiEvaluation.GroupBy(x => x.QUESTION_ID).Select(s => s.First()).ToList();
                        if (liQuestion != null && liQuestion.Count > 0)
                        {
                            ObjViewModel.QuestionList = (from s in liQuestion select new FBFEEDBACK_QUESTIONS() { QUESTION_ID = s.QUESTION_ID, QUESTION = s.QUESTION }).ToList();
                        }

                    }
                    ObjViewModel.liStudentStrength = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchClassWiseStudentStrengthForFeedbackReport).Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, liacyear.FirstOrDefault().ACADEMIC_YEAR), sAcademicYear).DataSource.Data;


                    // Fetch Feedback question mapping by feedback id
                    ObjViewModel.LiFeedbackQuestionMapping = (List<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING>)CMSHandler.FetchData<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbFeedbackQuestionsMappingByFeedbackId), sAcademicYear).DataSource.Data;

                    // Fetch College Details ,...
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;

                    // Fetch Students Count Answered For Each Questions By Class & Course ,...
                    //ObjViewModel.StudentCount = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCountByFBQuestion1), sAcademicYear).DataSource.Data;
                    ObjViewModel.StudentCount = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCountByFBQuestion1RatingBasedOverAll).Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, liacyear.FirstOrDefault().ACADEMIC_YEAR), sAcademicYear).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "BindFeedBackReportForStudent", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(ObjViewModel);
        }


        public ActionResult BindStudentFeedBackReportByDept(string sFbSettingId, string sAcademicYear, string sApplicationType, string sClassYear)
        {
            // string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sFbSettingId) && !string.IsNullOrEmpty(sApplicationType) && !string.IsNullOrEmpty(sClassYear))
            {
                try
                {
                    var ObjModel = new FEEDBACK_REPORT_FIELDS();
                    ObjModel.FEEDBACK_ID = sFbSettingId;
                    ObjModel.CLASS_YEAR = sClassYear;
                    ObjModel.APPTYPE_ID = sApplicationType;

                    ObjViewModel.liFeedbackEvaluationForReport = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentFeedbackReportByDept), sAcademicYear).DataSource.Data;

                    // Fetch College Details ,...
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "BindStudentFeedBackReportByDept", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(ObjViewModel);
        }
        public ActionResult BinFeedBackReportPieChart(string sFbSettingId, string sAcademicYear, string sApplicationType, string sClassID)
        {
            // string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sFbSettingId) && !string.IsNullOrEmpty(sApplicationType) && !string.IsNullOrEmpty(sClassID))
            {
                try
                {
                    var ObjModel = new FEEDBACK_REPORT_FIELDS();
                    ObjModel.FEEDBACK_ID = sFbSettingId;


                    var liacyear = (List<FBFEEDBACK_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(new FBFEEDBACK_SETTINGS() { FEEDBACK_ID = sFbSettingId },
                        SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackYearBySettingId)).DataSource.Data;

                    // Fetch Total Student Strength By Class & Course ,...
                    //ObjViewModel.liStudentStrength = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCountByClassAndCourseId), sAcademicYear).DataSource.Data;

                    // Fetch Evaluation By FeedBack Id and class id ,...
                    string sSql = SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbEvaluationInfoForReportByClassandAcYear);
                    sSql = sSql.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, liacyear.FirstOrDefault().ACADEMIC_YEAR).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassID);
                    ObjViewModel.LiEvaluation = (List<FBEVALUATION>)CMSHandler.FetchData<FBEVALUATION>(ObjModel, sSql, liacyear.FirstOrDefault().ACADEMIC_YEAR).DataSource.Data;

                    if (ObjViewModel.LiEvaluation != null && ObjViewModel.LiEvaluation.Count > 0)
                    {
                        //move only questions in this list
                        var liQuestion = ObjViewModel.LiEvaluation.GroupBy(x => x.QUESTION_ID).Select(s => s.First()).ToList();
                        if (liQuestion != null && liQuestion.Count > 0)
                        {
                            ObjViewModel.QuestionList = (from s in liQuestion select new FBFEEDBACK_QUESTIONS() { QUESTION_ID = s.QUESTION_ID, QUESTION = s.QUESTION }).ToList();
                        }
                    }
                    ObjViewModel.liStudentStrength = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchClassWiseStudentStrengthForFeedbackReport).Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, liacyear.FirstOrDefault().ACADEMIC_YEAR), sAcademicYear).DataSource.Data;

                    // Fetch Feedback question mapping by feedback id
                    ObjViewModel.LiFeedbackQuestionMapping = (List<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING>)CMSHandler.FetchData<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbFeedbackQuestionsMappingByFeedbackId), sAcademicYear).DataSource.Data;

                    // Fetch College Details ,...
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;

                    // Fetch Students Count Answered For Each Questions By Class & Course ,...
                    //ObjViewModel.StudentCount = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCountByFBQuestion1), sAcademicYear).DataSource.Data;
                    ObjViewModel.StudentCount = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCountByFBQuestionRatingbyClassWise).Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, liacyear.FirstOrDefault().ACADEMIC_YEAR).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassID), sAcademicYear).DataSource.Data;


                    ObjModel.FEEDBACK_ID = sFbSettingId;
                    ObjModel.CLASS_ID = sClassID;
                    ObjModel.APPTYPE_ID = sApplicationType;

                    ObjViewModel.liFeedbackEvaluationForReport = (List<FEEDBACK_REPORT_FIELDS>)CMSHandler.FetchData<FEEDBACK_REPORT_FIELDS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentFeedbackReportByClass_id).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassID), sAcademicYear).DataSource.Data;

                    // Fetch College Details ,...
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "BinFeedBackReportPieChart", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.InvalidInput;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(ObjViewModel);
        }
        /// <summary>
        ///  Setting Wise Feedback report genertaion with pie chart 
        /// </summary>
        /// <author>Yesudass</author>
        /// <date>07 June 2025</date>
        /// <returns></returns>

        public ActionResult BindFeedBackReportSettingWise(string sFbSettingId, string sAcademicYear)
        {
            string AcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            var ObjJsonData = new JsonResultData();
            //  var sAcademicYear = sAcYear;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sFbSettingId))
            {
                try
                {
                    var ObjModel = new FBEVALUATION();
                    ObjModel.FEEDBACK_ID = sFbSettingId;

                    var liacyear = (List<FBFEEDBACK_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(new FBFEEDBACK_SETTINGS() { FEEDBACK_ID = sFbSettingId },
                        SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackSettingBySettingId)).DataSource.Data;
                    if (liacyear != null && liacyear.Count > 0)
                    {
                        ObjViewModel.liFbSettings = liacyear;
                        // Fetch Total Student Strength By Class & Course ,...
                        //ObjViewModel.liStudentStrength = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCountByClassAndCourseId), sAcademicYear).DataSource.Data;

                        // Fetch Evaluation By FeedBack Id and class id ,...
                        string sSql = SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedEvalutaionByFeedbackID);
                        sSql = sSql.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                        ObjViewModel.LiEvaluation = (List<FBEVALUATION>)CMSHandler.FetchData<FBEVALUATION>(ObjModel, sSql, liacyear.FirstOrDefault().ACADEMIC_YEAR).DataSource.Data;

                        if (ObjViewModel.LiEvaluation != null && ObjViewModel.LiEvaluation.Count > 0)
                        {
                            //move only questions in this list
                            var liQuestion = ObjViewModel.LiEvaluation.GroupBy(x => x.QUESTION_ID).Select(s => s.First()).ToList();
                            if (liQuestion != null && liQuestion.Count > 0)
                            {
                                ObjViewModel.QuestionList = (from s in liQuestion select new FBFEEDBACK_QUESTIONS() { QUESTION_ID = s.QUESTION_ID, QUESTION = s.QUESTION, GROUP_TITLE = s.GROUP_TITLE, QUESTION_GROUP = s.QUESTION_GROUP }).ToList();
                            }
                        }
                        if (liacyear.FirstOrDefault().IS_HOSTEL == "1")
                        {
                            ObjViewModel.liStudentStrength = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchHostelStudentStrengthForFeedbackReport).Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, liacyear.FirstOrDefault().ACADEMIC_YEAR), sAcademicYear).DataSource.Data;
                        }
                        else
                        {
                            ObjViewModel.liStudentStrength = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentStrengthFromFbClassSettingForFeedbackReport).Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, liacyear.FirstOrDefault().ACADEMIC_YEAR), sAcademicYear).DataSource.Data;
                        }

                        // Fetch Feedback question mapping by feedback id
                        ObjViewModel.LiFeedbackQuestionMapping = (List<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING>)CMSHandler.FetchData<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackQuestionsMappingByFeedbackId), sAcademicYear).DataSource.Data;

                        // Fetch College Details ,...
                        ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;

                        // Fetch Students Count Answered For Each Questions By Class & Course ,...
                        //ObjViewModel.StudentCount = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCountByFBQuestion1), sAcademicYear).DataSource.Data;
                        ObjViewModel.StudentCount = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchAssorCountByFBQuestion1RatingBasedOverAll).Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, liacyear.FirstOrDefault().ACADEMIC_YEAR), sAcademicYear).DataSource.Data;

                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "BindFeedBackReportForStudent", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(ObjViewModel);
        }

        #endregion

        #region FeedBack Repoert For SJC
        public ActionResult FeedbackReportForSJC()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.AcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var lifbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                    if (lifbSetting != null && lifbSetting.Count > 0)
                    {
                        ObjViewModel.FeedBackSettingList = new SelectList(lifbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                    }

                    var liDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null,
                            SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    if (liDepartment != null && liDepartment.Count > 0)
                    {
                        ObjViewModel.DepartmentList = new SelectList(liDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "FeedbackReportForSJC", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindFeedBackReportForSJC(string sFbSettingId, string sSatffId, string sClassId, string sCourseId, string sAcYear)
        {
            //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            var sAcademicYear = sAcYear;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new FBEVALUATION();
                    ObjModel.FEEDBACK_ID = sFbSettingId;
                    ObjModel.ASSESSEE = sSatffId;
                    ObjModel.COURSE_ID = sCourseId;
                    ObjModel.CLASS_ID = sClassId;
                    // Fetch All Questions By FeedBack Id ,...
                    ObjViewModel.QuestionList = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionsByFeedBackId), sAcademicYear).DataSource.Data;

                    // Fetch Students Count Answered For Each Questions By Class & Course ,...

                    //ObjViewModel.StudentCount = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCountByFBQuestion), sAcademicYear).DataSource.Data;

                    ObjViewModel.StudentCount = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCountByFBQuestion1RatingBased), sAcademicYear).DataSource.Data;
                    // Fetch Total Student Strength By Class & Course ,...

                    ObjViewModel.liStudentStrength = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCountByClassAndCourseId), sAcademicYear).DataSource.Data;
                    // Fetch College Details ,...
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "BindFeedBackReport", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(ObjViewModel);
        }
        #endregion


        #region FeedBack Repoert For staff
        public ActionResult CourseWiseFeedbackReportForStaff()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.AcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var lifbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                    if (lifbSetting != null && lifbSetting.Count > 0)
                    {
                        ObjViewModel.FeedBackSettingList = new SelectList(lifbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                    }

                    var liDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null,
                            SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    if (liDepartment != null && liDepartment.Count > 0)
                    {
                        ObjViewModel.DepartmentList = new SelectList(liDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "FeedbackReportForSJC", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }


        public ActionResult BindCourseWiseFeedbackReportForStaff(string sFbSettingId, string sSatffId, string sClassId, string sCourseId, string sAcYear)
        {
            //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            var sAcademicYear = sAcYear;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new FBEVALUATION();
                    ObjModel.FEEDBACK_ID = sFbSettingId;
                    ObjModel.ASSESSEE = sSatffId;
                    ObjModel.COURSE_ID = sCourseId;
                    ObjModel.CLASS_ID = sClassId;

                    // Fetch All Questions By FeedBack Id ,...
                    ObjViewModel.QuestionList = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionsByFeedBackId), sAcademicYear).DataSource.Data;

                    // Fetch Students Count Answered For Each Questions By Class & Course ,...
                    ObjViewModel.StudentCount = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCountByFBQuestion), sAcademicYear).DataSource.Data;

                    // Fetch Total Student Strength By Class & Course ,...
                    ObjViewModel.liStudentStrength = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCountByClassAndCourseId), sAcademicYear).DataSource.Data;


                    // Fetch College Details ,...
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "BindFeedBackReport", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(ObjViewModel);
        }
        #endregion

        #region External Feed Back Entry

        // External User Info ...
        public ActionResult ExternalUserList()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRoleByLikeKeyword).Replace(Common.Delimiter.QUS + Common.SUP_ROLE.ROLE_NAME, "External");
                    var liRole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, sSQL).DataSource.Data;
                    if (liRole != null && liRole.Count > 0)
                    {
                        ObjViewModel.Role = new SelectList(liRole, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);
                    }

                    sSQL = string.Empty;
                    sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupUserTypeByLikeKeyword).Replace(Common.Delimiter.QUS + Common.SUP_USER_TYPE.USER_TYPE_NAME, "External");
                    var liUserType = (List<SUP_USER_TYPE>)CMSHandler.FetchData<SUP_USER_TYPE>(null, sSQL).DataSource.Data;
                    if (liUserType != null && liUserType.Count > 0)
                    {
                        ObjViewModel.UserType = new SelectList(liUserType, Common.SUP_USER_TYPE.USER_TYPE_ID, Common.SUP_USER_TYPE.USER_TYPE_NAME);
                    }

                    var liDesignation = (List<Sup_Designation>)CMSHandler.FetchData<Sup_Designation>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDesignation)).DataSource.Data;
                    if (liDesignation != null && liDesignation.Count > 0)
                    {
                        ObjViewModel.Designation = new SelectList(liDesignation, Common.SUP_DESIGNATION.DESIGNATIONID, Common.SUP_DESIGNATION.DESIGNATION);
                    }

                    var liDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(new STF_PERSONAL_INFO() { STAFF_ID = sUserId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartmentByUserId)).DataSource.Data;
                    if (liDepartment != null && liDepartment.Count > 0)
                    {
                        ObjViewModel.DepartmentList = new SelectList(liDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    }

                    ObjViewModel.liExternalUserInfo = (List<EXTERNAL_USERS>)CMSHandler.FetchData<EXTERNAL_USERS>(null, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchExternalUserInfo), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "ExternalUserList", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("ExternalUserList", "FeedBack");
            }
            return View(ObjViewModel);
        }

        public JsonResult FetchExternalInfoById(string sExternalUserId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liExternalUser = (List<EXTERNAL_USERS>)CMSHandler.FetchData<EXTERNAL_USERS>(new EXTERNAL_USERS() { EXTERNAL_USER_ID = sExternalUserId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchExternalUserById), sAcademicYear).DataSource.Data;
                    if (liExternalUser != null && liExternalUser.Count > 0)
                    {
                        return Json(liExternalUser, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "FetchExternalInfoById", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveExternalUserInfo(string sJson)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<EXTERNAL_USERS>(sJson);
                    //Result.DOB = CommonMethods.ConvertdatetoyearFromat(Result.DOB);

                    var liAutoNumber = (List<AUTO_GENERATE_NUMBERS>)CMSHandler.FetchData<AUTO_GENERATE_NUMBERS>(null, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchAutoGenerateNumber)).DataSource.Data;

                    // Check Is External user Exist ...,
                    var IsExternalUserExist = (List<EXTERNAL_USERS>)CMSHandler.FetchData<EXTERNAL_USERS>(Result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.CheckIsExternalUserExist), sAcademicYear).DataSource.Data;
                    if (IsExternalUserExist != null && IsExternalUserExist.Count > 0)
                    {
                        Result.EXTERNAL_USER_ID = IsExternalUserExist.FirstOrDefault().EXTERNAL_USER_ID;
                        Result.EXTERNAL_USER_CODE = IsExternalUserExist.FirstOrDefault().EXTERNAL_USER_CODE;
                        // Update .....
                        var Update = CMSHandler.SaveOrUpdate(Result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.UpdateExternalUserInfo), sAcademicYear);
                        if (Update.Success && Update.RowsAffected != 0)
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
                        Result.EXTERNAL_USER_CODE = "EX0" + liAutoNumber.FirstOrDefault().advance_receipt_no;
                        // Insert .....
                        var Insert = CMSHandler.SaveOrUpdate(Result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.InsertExternalUserInfo), sAcademicYear, true);
                        if (Insert.Success && Insert.RowsAffected != 0)
                        {
                            var sResultArg = CMSHandler.SaveOrUpdate(new AUTO_GENERATE_NUMBERS()
                            {
                                exam_receipt_no = liAutoNumber.FirstOrDefault().exam_receipt_no,
                                advance_receipt_no = liAutoNumber.FirstOrDefault().advance_receipt_no
                            }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.UpdateAutoGenerateNumber));
                            Result.EXTERNAL_USER_ID = Insert.RowUniqueId.ToString();
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        else
                        {
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }

                    var ObjModel = new USER_ACCOUNT_INFO();
                    ObjModel.USERNAME = Result.EXTERNAL_USER_CODE;
                    ObjModel.PASSWORD = CommonMethods.GetSha256FromString(Result.MOBILE_NO);
                    ObjModel.ROLE = Result.ROLE;
                    ObjModel.ROLE_ID = Result.ROLE;
                    ObjModel.NAME = Result.FIRST_NAME;
                    ObjModel.MOBILE = Result.MOBILE_NO;
                    ObjModel.USER_ID = Result.EXTERNAL_USER_ID;
                    ObjModel.USER_TYPE = Result.USER_TYPE;
                    ObjModel.EMAIL = Result.MAIL_ID;
                    // Check User Account Is Exist ...,
                    var IsUserAccountExist = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(ObjModel, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.CheckIsUserAccountExist), sAcademicYear).DataSource.Data;
                    if (IsUserAccountExist != null && IsUserAccountExist.Count > 0)
                    {
                        ObjModel.USER_ACCOUNT_ID = IsUserAccountExist.FirstOrDefault().USER_ACCOUNT_ID;
                        var Update = CMSHandler.SaveOrUpdate(ObjModel, SQL.Account.AccountSQL.GetAccountSQL(AccountSQLCommands.UpdateUserAccount), sAcademicYear);
                    }
                    else
                    {
                        var Insert = CMSHandler.SaveOrUpdate(ObjModel, SQL.Account.AccountSQL.GetAccountSQL(AccountSQLCommands.SaveUserAccount), sAcademicYear);
                        if (Insert.Success && Insert.RowsAffected != 0)
                        {
                            var InsertUserRights = CMSHandler.SaveOrUpdate(ObjModel, SQL.Account.AccountSQL.GetAccountSQL(AccountSQLCommands.SaveUserRolesByAcYear), sAcademicYear);
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "SaveExternalUserInfo", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteExternalUserInfo(string sExternalUserId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Delete = CMSHandler.SaveOrUpdate(new EXTERNAL_USERS() { EXTERNAL_USER_ID = sExternalUserId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.DeleteExternalUserInfo), sAcademicYear);
                    if (Delete.Success && Delete.RowsAffected != 0)
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
                        ObjLog.WriteError("FeedBackController", "DeleteExternalUserInfo", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        // External User Course Mapping ....
        public ActionResult ExternalUserCourseMappingList()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liFeedBackSetting = (List<FBFEEDBACK_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                    if (liFeedBackSetting != null && liFeedBackSetting.Count > 0)
                    {
                        ObjViewModel.FeedBackSettingList = new SelectList(liFeedBackSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                    }

                    var liStaff = DapperHandler.FetchAll<STF_PERSONAL_INFO>(SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchExternalUserList), new { ENTRY_ID = sUserId }, sAcademicYear).ToList();
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        ObjViewModel.StaffListByDepartmet = new SelectList(liStaff, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    }
                    var liCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new STF_PERSONAL_INFO() { STAFF_ID = sUserId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartmentCourseListbyStaffId), sAcademicYear).DataSource.Data;
                    if (liCourse != null && liCourse.Count > 0)
                    {
                        ObjViewModel.CourseList = new SelectList(liCourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_TITLE);
                    }

                    ObjViewModel.liExternalUserCourseMapping = (List<FB_EXTERNAL_USER_COURSE_MAPPING>)CMSHandler.FetchData<FB_EXTERNAL_USER_COURSE_MAPPING>(
                        new FB_EXTERNAL_USER_COURSE_MAPPING() { ENTRY_ID = sUserId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchCourseMappingInfo), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "ExternalUserCourseMappingList", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("ExternalUserCourseMappingList", "FeedBack");
            }

            return View(ObjViewModel);
        }

        public JsonResult FetchExternalUserCourseById(string sCourseMappingId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liExternalUserCourseMapping = (List<FB_EXTERNAL_USER_COURSE_MAPPING>)CMSHandler.FetchData<FB_EXTERNAL_USER_COURSE_MAPPING>(
                        new FB_EXTERNAL_USER_COURSE_MAPPING() { COURSE_MAPPING_ID = sCourseMappingId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchCourseMappingInfoById), sAcademicYear).DataSource.Data;
                    if (liExternalUserCourseMapping != null && liExternalUserCourseMapping.Count > 0)
                    {
                        return Json(liExternalUserCourseMapping, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "FetchExternalUserCourseById", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveExternalUserCourseMapping(string sJson)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<FB_EXTERNAL_USER_COURSE_MAPPING>(sJson);
                    Result.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(Result.DATE_FROM);
                    Result.DATE_TO = CommonMethods.ConvertdatetoyearFromat(Result.DATE_TO);
                    Result.ENTRY_ID = sUserId;
                    var CourseId = Result.COURSE_ID.Split(',');
                    if (CourseId.Length > 0)
                    {
                        foreach (var item in CourseId)
                        {
                            Result.COURSE_ID = item;
                            // Is Exist ...,
                            var IsExist = (List<FB_EXTERNAL_USER_COURSE_MAPPING>)CMSHandler.FetchData<FB_EXTERNAL_USER_COURSE_MAPPING>(Result,
                                        SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.IsExistCourseMappingInfo), sAcademicYear).DataSource.Data;
                            if (IsExist != null && IsExist.Count > 0)
                            {
                                Result.COURSE_MAPPING_ID = IsExist.FirstOrDefault().COURSE_MAPPING_ID;
                                // Update ...,
                                var Update = CMSHandler.SaveOrUpdate(Result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.UpdateCourseMapping), sAcademicYear);
                                if (Update.Success && Update.RowsAffected != 0)
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
                                // Insert ...,
                                var Insert = CMSHandler.SaveOrUpdate(Result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.SaveCourseMappingInfo), sAcademicYear);
                                if (Insert.Success && Insert.RowsAffected != 0)
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
                        ObjLog.WriteError("FeedBackController", "SaveExternalUserCourseMapping", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteExternalUserCourseMapping(string sCourseMappingId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Delete = CMSHandler.SaveOrUpdate(new FB_EXTERNAL_USER_COURSE_MAPPING() { COURSE_MAPPING_ID = sCourseMappingId },
                            SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.DeleteCourseMapping), sAcademicYear);
                    if (Delete.Success && Delete.RowsAffected != 0)
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        ObjJsonData.Message = Common.Messages.RecordDeletedSuccessfully;
                    }
                    else
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        ObjJsonData.Message = Common.Messages.FailedToDeletedTryAgain;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "DeleteExternalUserCourseMapping", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        // SMS 
        public JsonResult SendSMS()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            bool bFlag = false;
            List<MessageResult.listsms> listbulkSMS = new List<MessageResult.listsms>();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liEmailSetting = DapperHandler.FetchAll<EMAIL_SETTING>(SQL.Communication.CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchEmailSetting), null).ToList();

                    var liSendSMS = (List<MessageResult.sms>)CMSHandler.FetchData<MessageResult.sms>(new FB_EXTERNAL_USER_COURSE_MAPPING() { ENTRY_ID = sUserId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchMessageContentForExternalUserCourseMapping), sAcademicYear).DataSource.Data;
                    if (liSendSMS != null && liSendSMS.Count > 0)
                    {
                        var liSentSMS = (List<FB_SENT_SMS>)CMSHandler.FetchData<FB_SENT_SMS>(null, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFBSentSMSList), sAcademicYear).DataSource.Data;
                        if (liSentSMS != null && liSentSMS.Count > 0)
                        {
                            foreach (var item in liSentSMS)
                            {
                                liSendSMS.RemoveAll(x => x.custom == item.STAFF_ID);
                            }
                        }

                        foreach (var item in liSendSMS)
                        {
                            listbulkSMS.Add(new MessageResult.listsms() { mobiles = item.to, sms = item.message });
                            bFlag = CommonMethods.SendMailFromGmailSMTP(liEmailSetting.FirstOrDefault().FROM_EMAIL_ID, liEmailSetting.FirstOrDefault().FROM_EMAIL_PASSWORD, item.custom2, "User Credential From St.Joseph's College - Cuddalore", item.message);
                        }


                        if (liSendSMS != null && liSendSMS.Count > 0)
                        {
                            //liSendSMS.Add(new MessageResult.sms() { to = "9443246856", message = "Today Late Message Sent Successfully !", custom = "0", custom1 = "0", custom2 = "0", CreditCount = "1" });
                            var ObjControllerModel = new CommunicationController();
                            ObjControllerModel.liSMS = liSendSMS;
                            ObjControllerModel.listbulkSMS = listbulkSMS;
                            ObjControllerModel.SendMessage();
                            if (ObjControllerModel.sResultMessage == Common.ErrorCode.Ok)
                            {
                                string sSQL = @"INSERT INTO FB_SENT_SMS(STAFF_ID,IS_SMS_SENT,FEEDBACK_SETTING_ID,ACADEMIC_YEAR)VALUES";
                                foreach (var item in liSendSMS)
                                {
                                    sSQL += "('" + item.custom + "','1','" + item.custom1 + "','" + sAcademicYear + "'),";
                                }
                                sSQL = sSQL.TrimEnd(',');
                                using (MySQLHandler ObjHandler = new MySQLHandler())
                                {
                                    var sResultArg = ObjHandler.ExecuteAsScripts(sSQL);
                                }
                                ObjJsonData.Message = "SMS Sent Successfully !";
                                ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            }
                            else
                            {
                                ObjJsonData.Message = "Sorry ! Failed To Sent Messages .!";
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            }
                        }
                        else
                        {
                            ObjJsonData.Message = "Sorry ! No Content Available !.";
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = "Sorry ! No Content Available !.";
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "SendSMS", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }


        // External User Feed Back Entry ....
        public ActionResult ExternalUserFeedBackEntry(string sCourseId, string sCourseCode, string sFeedbackId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.sErrorMessage = sCourseId;
                    ObjViewModel.sErrorCode = sUserId;
                    ObjViewModel.Question = sCourseCode;
                    ObjViewModel.liExternalUserEntry = (List<FB_EXTERNAL_USER_ENTRY>)CMSHandler.FetchData<FB_EXTERNAL_USER_ENTRY>(new FB_EXTERNAL_USER_ENTRY() { COURSE_ID = sCourseId, STAFF_ID = sUserId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchExternalFeedbackEntryByCourseAndStaffId), sAcademicYear).DataSource.Data;
                    ObjViewModel.QuestionList = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_QUESTIONS() { FEEDBACK_ID = sFeedbackId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FectchFBQuestionsByFeedbackId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "ExternalUserCourseMappingList", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("ExternalUserFeedBackEntry", "FeedBack");
            }

            return View(ObjViewModel);
        }

        public JsonResult SaveExternalUserFeedBackEntry(string sJson, string sFinished)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<SAVE_JSON>(sJson);
                    if (Result.liFbExternalUserEntry != null && Result.liFbExternalUserEntry.Count > 0)
                    {
                        foreach (var item in Result.liFbExternalUserEntry)
                        {
                            if (!string.IsNullOrEmpty(item.FB_ENTRY_ID))
                            {
                                var UpdateFeedBackEntry = CMSHandler.SaveOrUpdate(item, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.UpdateExternalFeedbackEntry), sAcademicYear);
                                if (UpdateFeedBackEntry.Success && UpdateFeedBackEntry.RowsAffected != 0)
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
                                var SaveFeedBackEntry = CMSHandler.SaveOrUpdate(item, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.SaveExternalFeedbackEntry), sAcademicYear);
                                if (SaveFeedBackEntry.Success && SaveFeedBackEntry.RowsAffected != 0)
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
                        if (sFinished == Common.IsActiveFalg.IsActive)
                        {
                            var liCourseInfo = (List<FB_EXTERNAL_USER_COURSE_MAPPING>)CMSHandler.FetchData<FB_EXTERNAL_USER_COURSE_MAPPING>(new FB_EXTERNAL_USER_COURSE_MAPPING()
                            { COURSE_ID = Result.liFbExternalUserEntry.FirstOrDefault().COURSE_ID, STAFF_ID = sUserId },
                                SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchExternalCourseMappingInfoByCourseAndStaffId), sAcademicYear).DataSource.Data;

                            var UpdateFinishedStatus = CMSHandler.SaveOrUpdate(new FB_EXTERNAL_USER_COURSE_MAPPING() { COURSE_MAPPING_ID = liCourseInfo.FirstOrDefault().COURSE_MAPPING_ID },
                                SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.UpdateFBFinishedStatus), sAcademicYear);
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "SaveExternalUserFeedBackEntry", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Generate E - Certificate
        public ActionResult ECertificateForFeedback()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(new STF_PERSONAL_INFO() { STAFF_ID = sUserId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchAllotedDepartmentForExternalUser), sAcademicYear).DataSource.Data;
                    if (liDepartment != null && liDepartment.Count > 0)
                    {
                        ObjViewModel.DepartmentList = new SelectList(liDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    }

                    // Fetch College Details ,...
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "ECertificateForFeedback", ex.Message);
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

        public JsonResult FetchCourseByDepartmentId(string sDepartmentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new STF_PERSONAL_INFO() { DEPARTMENT = sDepartmentId, STAFF_ID = sUserId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchAllotedCoursesByDepartmnetIdAndExternalUserId), sAcademicYear).DataSource.Data;
                    if (liCourse != null && liCourse.Count > 0)
                    {
                        ObjJsonData.sResult += "<option> -- Select -- </option>";
                        foreach (var item in liCourse)
                        {
                            ObjJsonData.sResult += "<option value='" + item.COURSE_ROOT_ID + "'>" + item.COURSE_TITLE + "</option>";
                        }
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "FetchCourseByDepartmentId", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateECertificateForExternalFeedback(string sCourseId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liCertificateInfo = (List<CERTIFICATE_INFO>)CMSHandler.FetchData<CERTIFICATE_INFO>(new CERTIFICATE_INFO() { STAFF_ID = sUserId, COURSE_ID = sCourseId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchExternalUserCertificateInfo), sAcademicYear).DataSource.Data;
                    // Fetch College Details ,...
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "GenerateECertificateForExternalFeedback", ex.Message);
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
        #endregion

        #region Report For External Feedback Entry
        public ActionResult FbExternalUserEntryReportForAdmin()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liAcademicYear = DapperHandler.FetchAll<ACADEMIC_YEAR>(SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchAcademicYearList)).ToList();
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.AcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    var liDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null,
                            SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    if (liDepartment != null && liDepartment.Count > 0)
                    {
                        ObjViewModel.DepartmentList = new SelectList(liDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "FbExternalUserEntry", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult FbExternalUserEntryReportForHOD()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liAcademicYear = DapperHandler.FetchAll<ACADEMIC_YEAR>(SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchAcademicYearList)).ToList();
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.AcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    var liDepartment = DapperHandler.FetchAll<STF_PERSONAL_INFO>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartmentByStaffId), new { STAFF_ID = sUserId }).ToList();
                    if (liDepartment != null && liDepartment.Count > 0)
                    {
                        ObjViewModel.sErrorMessage = liDepartment.FirstOrDefault().DEPARTMENT;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "FbExternalUserEntryReportForHOD", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult FetchCourseByDepartmentForExternalFeedback(string sAcYear, string sFeedBackSettingId, string sDepartmentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liCourse = DapperHandler.FetchAll<CP_COURSE_ROOT>(SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchCourseByDepartmentForExternalFeedback), new { FEEDBACK_SETTING_ID = sFeedBackSettingId, DEPARTMENT_ID = sDepartmentId }, sAcYear).ToList();
                    if (liCourse != null && liCourse.Count > 0)
                    {
                        ObjJsonData.sResult += "<option> -- Select -- </option>";
                        foreach (var item in liCourse)
                        {
                            if (!string.IsNullOrEmpty(item.PAPER)) { item.PAPER = "class='fa fa-check text-success'"; } else { item.PAPER = "class='fa fa-times text-danger'"; }
                            ObjJsonData.sResult += "<option value='" + item.COURSE_ROOT_ID + "' " + item.PAPER + ">" + item.COURSE_TITLE + "</option>";
                        }
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "FetchCourseByDepartmentForExternalFeedback", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchFeedbackSettingByAcYear(string sAcademicYear)
        {
            var JsonData = new JsonResultData();
            var liFeedBackSetting = DapperHandler.FetchAll<FBFEEDBACK_SETTINGS>(SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), null, sAcademicYear).ToList();
            if (liFeedBackSetting != null && liFeedBackSetting.Count > 0)
            {
                JsonData.sResult = "<option value='0'> -- Select -- </option>";
                foreach (var item in liFeedBackSetting)
                {
                    JsonData.sResult += "<option value='" + item.FEEDBACK_ID + "'> " + item.FEEDBACK_NAME + " </option>";
                }
                JsonData.ErrorCode = Common.ErrorCode.Ok;
            }
            else
            {
                JsonData.Message = Common.Messages.NoRecordsfound;
            }
            return Json(JsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchExternalFeedbackSettingByAcYear(string sAcademicYear)
        {
            var JsonData = new JsonResultData();
            var liFeedBackSetting = DapperHandler.FetchAll<FBFEEDBACK_SETTINGS>(SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchExternalActiveFeedBackSettings), null, sAcademicYear).ToList();
            if (liFeedBackSetting != null && liFeedBackSetting.Count > 0)
            {
                JsonData.sResult = "<option value='0'> -- Select -- </option>";
                foreach (var item in liFeedBackSetting)
                {
                    JsonData.sResult += "<option value='" + item.FEEDBACK_ID + "'> " + item.FEEDBACK_NAME + " </option>";
                }
                JsonData.ErrorCode = Common.ErrorCode.Ok;
            }
            else
            {
                JsonData.Message = Common.Messages.NoRecordsfound;
            }
            return Json(JsonData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult BindFbExternalUserEntryByCourseId(string sFeedbackId, string sCourseId, string sAcYear)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Fetch College Details ,...
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    ObjViewModel.liExternalUserCourseMapping = (List<FB_EXTERNAL_USER_COURSE_MAPPING>)CMSHandler.FetchData<FB_EXTERNAL_USER_COURSE_MAPPING>(new FB_EXTERNAL_USER_COURSE_MAPPING() { FEEDBACK_SETTING_ID = sFeedbackId, COURSE_ID = sCourseId },
                                 SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchExternalFeedBackReportByCourseAndSettingId), sAcYear).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "BindFbExternalUserEntryByCourseId", ex.Message);
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

        public ActionResult FbExternalUserEntryStatusForAdmin()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liAcademicYear = DapperHandler.FetchAll<ACADEMIC_YEAR>(SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchAcademicYearList)).ToList();
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.AcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    var liDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null,
                            SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    if (liDepartment != null && liDepartment.Count > 0)
                    {
                        ObjViewModel.DepartmentList = new SelectList(liDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "FbExternalUserEntryStatusForAdmin", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindFbExternalUserEntryStatus(string sAcYear, string sDepartmentId, string sFeedBackSettingId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liCourse = DapperHandler.FetchAll<CP_COURSE_ROOT>(SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchCourseByDepartmentForExternalFeedbackStatus), new { FEEDBACK_SETTING_ID = sFeedBackSettingId, DEPARTMENT_ID = sDepartmentId }, sAcYear).ToList();

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "BindFbExternalUserEntryStatus", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return View();
        }
        // External User Course Mapping List For Admin ...,
        public ActionResult FbExternalUserCourseMappingListForAdmin()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liExternalUserCourseMapping = (List<FB_EXTERNAL_USER_COURSE_MAPPING>)CMSHandler.FetchData<FB_EXTERNAL_USER_COURSE_MAPPING>(
                        null, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchExternalUserCourseMappingListForAdmin), sAcademicYear).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "FbExternalUserCourseMappingListForAdmin", ex.Message);
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

        #region FeedBack Entry For Syllabus By Student
        public ActionResult FbSyllabusEntryForStudent()
        {
            var ObjViewModel = new FeedbackViewModel();
            var sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new GET_PROGRAMME_AND_BATCH();
                    ObjModel.STUDENT_ID = sUserId;
                    var liBatchAndProgramme = (List<GET_PROGRAMME_AND_BATCH>)CMSHandler.FetchData<GET_PROGRAMME_AND_BATCH>(ObjModel,
                        ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchProgramAndBatchByStudentID), sAcademicYear).DataSource.Data;
                    ObjModel.BATCH = liBatchAndProgramme.FirstOrDefault().BATCH;
                    ObjModel.PROGRAMME = liBatchAndProgramme.FirstOrDefault().PROGRAMME;
                    var liRegularCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(ObjModel,
                        ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamRegistration), sAcademicYear).DataSource.Data;
                    var liOptionalCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(ObjModel,
                          ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;

                    // var liTemp = liRegularCourses.Where(o => o.COURSE_STRUCTURE_ID == "579").ToList();
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

                    ObjViewModel.liExamRegistrationCourse = liRegularCourses;


                    var lifbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                    if (lifbSetting != null && lifbSetting.Count > 0)
                    {
                        ObjViewModel.FeedBackSettingList = new SelectList(lifbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "FbSyllabusEntryForStudent", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindFbSyllabusEntryByCourseId(string sCourseId, string sFeedbackSettingId, string sCourseCode)
        {
            var ObjViewModel = new FeedbackViewModel();
            var ObjJsonData = new JsonResultData();
            var sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.Question = sCourseCode;
                    var liIsExist = (List<FBEVALUATIONSYLLABUS>)CMSHandler.FetchData<FBEVALUATIONSYLLABUS>(new FBEVALUATIONSYLLABUS() { FEEDBACK_ID = sFeedbackSettingId, COURSE_ID = sCourseId, STUDENT_ID = sUserId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.SyllabusFeedbackIsExistBySettingAndCourseId), sAcademicYear).DataSource.Data;
                    if (liIsExist != null && liIsExist.Count > 0)
                    {
                        ObjJsonData.Message = "Sorry ! Feedback already done for this course ";
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                    ObjViewModel.QuestionList = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBEVALUATION() { FEEDBACK_ID = sFeedbackSettingId, COURSE_ID = sCourseId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionsByFeedBackId), sAcademicYear).DataSource.Data;

                    ObjViewModel.liOptions = (List<FBOPTIONS>)CMSHandler.FetchData<FBOPTIONS>(new FBEVALUATION() { FEEDBACK_ID = sFeedbackSettingId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackOptionsByFeedbackId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "BindFbSyllabusEntryByCourseId", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult SaveFbSyllabusEntry(string sJson, string sCourseId, string sFeedbackId)
        {
            var ObjJsonData = new JsonResultData();
            var sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sSQL = @"INSERT INTO FBEVALUATIONSYLLABUS(FEEDBACK_ID,STUDENT_ID,COURSE_ID,QUESTION_ID,ANSWER_ID,COMMENTS,ACADEMIC_YEAR)VALUES";
                    var sResult = JsonConvert.DeserializeObject<SAVE_JSON>(sJson);
                    if (sResult.liFbEvaluationSyllabus != null && sResult.liFbEvaluationSyllabus.Count > 0)
                    {
                        foreach (var item in sResult.liFbEvaluationSyllabus)
                        {
                            if (string.IsNullOrEmpty(item.COMMENTS) || item.COMMENTS == "undefined") { item.COMMENTS = string.Empty; }
                            sSQL += "('" + sFeedbackId + "','" + sUserId + "','" + sCourseId + "','" + item.QUESTION_ID + "','" + item.ANSWER_ID + "','" + item.COMMENTS + "','" + sAcademicYear + "'),";
                        }

                        sSQL = sSQL.TrimEnd(',');
                        sSQL = sSQL + ";";
                        using (MySQLHandler ObjHandler = new MySQLHandler())
                        {
                            var sResultArgs = ObjHandler.ExecuteAsScripts(sSQL);
                            if (sResultArgs.Success && sResultArgs.RowsAffected != 0)
                            {
                                ObjJsonData.Message = "( " + sResultArgs.RowsAffected.ToString() + " ) Records Saved Successfully !";
                                ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            }
                            else
                            {
                                ObjJsonData.Message = "( " + sResultArgs.RowsAffected.ToString() + " ) failed to save records !";
                                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "SaveFbSyllabusEntry", ex.Message);
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

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ListFbSyllabusEntry()
        {
            var ObjViewModel = new FeedbackViewModel();
            var sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Fetch College Details ,...
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;

                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.AcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    var lifbSetting = (List<FBFEEDBACK_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                    if (lifbSetting != null && lifbSetting.Count > 0)
                    {
                        ObjViewModel.FeedBackSettingList = new SelectList(lifbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                    }

                    var liDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    if (liDepartment != null && liDepartment.Count > 0)
                    {
                        ObjViewModel.DepartmentList = new SelectList(liDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "ListFbSyllabusEntry", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchFeedBacksettingByYear(string sAcyear)
        {
            FeedbackViewModel objFeedbackViewModel = new FeedbackViewModel();
            string option = string.Empty;
            try
            {

                var lifbSetting = (List<FBFEEDBACK_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeedBackSettingsByYear), sAcyear).DataSource.Data;

                if (lifbSetting != null && lifbSetting.Count > 0)
                {
                    foreach (var item in lifbSetting)
                    {
                        option += "<option value='" + item.FEEDBACK_ID + "' >" + item.FEEDBACK_NAME + "</option>";
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FetchFeedBacksettingByYear", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FetchFeedBacksettingByYear", ex.Message);
                }
            }

            return Json(option, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchStaffFeedBacksettingByYear(string sAcyear)
        {
            FeedbackViewModel objFeedbackViewModel = new FeedbackViewModel();
            string option = string.Empty;
            try
            {

                var lifbSetting = (List<FBFEEDBACK_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffFeedBackSettingsByYear), sAcyear).DataSource.Data;

                if (lifbSetting != null && lifbSetting.Count > 0)
                {
                    foreach (var item in lifbSetting)
                    {
                        option += "<option value='" + item.FEEDBACK_ID + "' >" + item.FEEDBACK_NAME + "</option>";
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FetchFeedBacksettingByYear", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FetchFeedBacksettingByYear", ex.Message);
                }
            }

            return Json(option, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FetchSyllabusFeedBacksettingByYear(string sAcyear)
        {
            FeedbackViewModel objFeedbackViewModel = new FeedbackViewModel();
            string option = string.Empty;
            try
            {

                var lifbSetting = (List<FBFEEDBACK_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSyllabusFeedBackSettingsByYear), sAcyear).DataSource.Data;

                if (lifbSetting != null && lifbSetting.Count > 0)
                {
                    foreach (var item in lifbSetting)
                    {
                        option += "<option value='" + item.FEEDBACK_ID + "' >" + item.FEEDBACK_NAME + "</option>";
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeedbackController", "FetchFeedBacksettingByYear", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeedbackController", "FetchFeedBacksettingByYear", ex.Message);
                }
            }

            return Json(option, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BindListFbSyllabusEntryByCourseId(string sCourseId, string sCourseCode, string sFeedbackId, string sAcYear)
        {
            var ObjViewModel = new FeedbackViewModel();
            var sAcademicYear = sAcYear;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Student Count List ...
                    ObjViewModel.liSyllabusEvaluationStudent = (List<FBEVALUATIONSYLLABUS>)CMSHandler.FetchData<FBEVALUATIONSYLLABUS>(new FBEVALUATION() { FEEDBACK_ID = sFeedbackId, COURSE_ID = sCourseId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbSyllabusStudentCount), sAcademicYear).DataSource.Data;

                    // Option Count List ...
                    ObjViewModel.liSyllabusEvaluationOption = (List<FBEVALUATIONSYLLABUS>)CMSHandler.FetchData<FBEVALUATIONSYLLABUS>(new FBEVALUATION() { FEEDBACK_ID = sFeedbackId, COURSE_ID = sCourseId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbOptionEvaluationCount), sAcademicYear).DataSource.Data;

                    // Question List ...
                    ObjViewModel.QuestionList = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBEVALUATION() { FEEDBACK_ID = sFeedbackId, COURSE_ID = sCourseId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionsByFeedBackId), sAcademicYear).DataSource.Data;

                    // Option List ...
                    ObjViewModel.liOptions = (List<FBOPTIONS>)CMSHandler.FetchData<FBOPTIONS>(new FBEVALUATION() { FEEDBACK_ID = sFeedbackId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackOptionsByFeedbackId), sAcademicYear).DataSource.Data;
                    // Fetch College Details ,...
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "BindListFbSyllabusEntryByCourseId", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult ListFbSyllabusEntryByDepartment()
        {
            var ObjViewModel = new FeedbackViewModel();
            var sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Fetch College Details ,...
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;

                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.AcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    var lifbSetting = (List<FBFEEDBACK_SETTINGS>)CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                    if (lifbSetting != null && lifbSetting.Count > 0)
                    {
                        ObjViewModel.FeedBackSettingList = new SelectList(lifbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                    }

                    var liDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    if (liDepartment != null && liDepartment.Count > 0)
                    {
                        ObjViewModel.DepartmentList = new SelectList(liDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "ListFbSyllabusEntry", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindListFbSyllabusEntryByDepartmentId(string department, string sFeedbackId, string sAcYear)
        {
            var ObjViewModel = new FeedbackViewModel();
            var sAcademicYear = sAcYear;

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liCourseList = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(
                        new CP_COURSE_ROOT() { DEPARTMENT = department, FEEDBACK_ID = sFeedbackId },
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseListByFeedbackId),
                        sAcademicYear
                    ).DataSource.Data;

                    // Initialize lists to avoid null reference issues
                    ObjViewModel.liSyllabusEvaluationStudent = new List<FBEVALUATIONSYLLABUS>();
                    ObjViewModel.liSyllabusEvaluationOption = new List<FBEVALUATIONSYLLABUS>();
                    ObjViewModel.QuestionList = new List<FBFEEDBACK_QUESTIONS>();
                    ObjViewModel.liOptions = new List<FBOPTIONS>();

                    foreach (var sCourseId in ObjViewModel.liCourseList)
                    {
                        // Student Count List ...
                        var studentList = (List<FBEVALUATIONSYLLABUS>)CMSHandler.FetchData<FBEVALUATIONSYLLABUS>(
                            new FBEVALUATION() { FEEDBACK_ID = sFeedbackId, COURSE_ID = sCourseId.COURSE_ROOT_ID },
                            SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbSyllabusStudentCount),
                            sAcademicYear
                        ).DataSource.Data;

                        if (studentList != null)
                        {
                            ObjViewModel.liSyllabusEvaluationStudent.AddRange(studentList);
                        }

                        // Option Count List ...
                        var optionList = (List<FBEVALUATIONSYLLABUS>)CMSHandler.FetchData<FBEVALUATIONSYLLABUS>(
                            new FBEVALUATION() { FEEDBACK_ID = sFeedbackId, COURSE_ID = sCourseId.COURSE_ROOT_ID },
                            SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFbOptionEvaluationCount),
                            sAcademicYear
                        ).DataSource.Data;

                        if (optionList != null)
                        {
                            ObjViewModel.liSyllabusEvaluationOption.AddRange(optionList);
                        }


                    }
                    // Question List ...
                    var questionList = (List<FBFEEDBACK_QUESTIONS>)CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(
                        new FBEVALUATION() { FEEDBACK_ID = sFeedbackId },
                        SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionsByFeedBackId),
                        sAcademicYear
                    ).DataSource.Data;

                    if (questionList != null)
                    {
                        ObjViewModel.QuestionList.AddRange(questionList);
                    }

                    // Option List ...
                    var options = (List<FBOPTIONS>)CMSHandler.FetchData<FBOPTIONS>(
                        new FBEVALUATION() { FEEDBACK_ID = sFeedbackId },
                        SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackOptionsByFeedbackId),
                        sAcademicYear
                    ).DataSource.Data;

                    if (options != null)
                    {
                        ObjViewModel.liOptions.AddRange(options);
                    }
                    // Fetch College Details
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(
                        null,
                        SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)
                    ).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (var ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedbackController", "BindListFbSyllabusEntryByDepartmentId", ex.Message);
                    }
                    // Consider redirecting to an error view or returning a model with error information
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }

            return View(ObjViewModel);
        }

        #endregion

        #region Feedbck Class Setting 
        public ActionResult ListFeedbackClassSetting()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    FeedbackViewModel.sAcademicAttempt = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? 0 : FeedbackViewModel.sAcademicAttempt + 1;
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                        objViewModel.AcademicYearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);

                    var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchFeedBackSettings), sAcademicYear).DataSource.Data;
                    if (fbSetting != null && fbSetting.Count > 0)
                    {
                        objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                    }
                    var liClassList = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchClassListForFeedbck), sAcademicYear).DataSource.Data;
                    if (liClassList != null && liClassList.Count > 0)
                    {
                        objViewModel.ClassList = new SelectList(liClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                    var classYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data;
                    if (classYear != null && classYear.Count > 0)
                    {
                        objViewModel.ClassYearList = new SelectList(classYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                    }
                    var liShift = (List<Sup_Shift>)CMS.DAO.CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (liShift != null && liShift.Count > 0)
                    {
                        objViewModel.ShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    var programmeLevel = (List<SUP_PROGRAMME_LEVEL>)CMSHandler.FetchData<SUP_PROGRAMME_LEVEL>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel)).DataSource.Data;
                    if (programmeLevel != null && programmeLevel.Count > 0)
                    {
                        objViewModel.ProgrammeLevelList = new SelectList(programmeLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
                    }
                    objViewModel.liFbClassSettings = (List<FBFEEDBACK_CLASS_SETTING>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_CLASS_SETTING>(null, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedBackClassSettings), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "ListFeedbackSetting", ex.Message);
                    }
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindFeedbackClassSetting(string sacyear, string sfeedbacksetting)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //var isExist = (List<FBFEEDBACK_CLASS_SETTING>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_CLASS_SETTING>(new FBFEEDBACK_CLASS_SETTING { FEEDBACK_ID = sfeedbacksetting, ACADEMIC_YEAR = sacyear }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.isalreadyUpdatedfeedbckclass), sAcademicYear).DataSource.Data;
                    //if (isExist == null)
                    //{
                    //    resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new FBFEEDBACK_CLASS_SETTING { FEEDBACK_ID = sfeedbacksetting, ACADEMIC_YEAR = sacyear }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.UpdateFeedBackClassSettingToAllCLass), sAcademicYear);
                    //}

                    objViewModel.liFbClassSettings = (List<FBFEEDBACK_CLASS_SETTING>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_CLASS_SETTING>(new FBFEEDBACK_CLASS_SETTING() { FEEDBACK_ID = sfeedbacksetting }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedBackClassSettings), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "ListFeedbackSetting", ex.Message);
                    }
                }

            }
            return View(objViewModel);
        }
        public JsonResult FetchLiFeedBackClassSettingById(string sFeedBackId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sFeedBackId))
                {
                    // fetch setting 
                    objResultData.oResult = (List<FBFEEDBACK_CLASS_SETTING>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_CLASS_SETTING>(new FBFEEDBACK_CLASS_SETTING() { CLASS_SETTING_ID = sFeedBackId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedBackClassSettingsById), sAcademicYear).DataSource.Data;
                }
            }
            return Json(objResultData);
        }
        public JsonResult FetchgfeedbckByAcyear(string sacyear)
        {
            string sOption = string.Empty;
            FBFEEDBACK_CLASS_SETTING ObjModel = new FBFEEDBACK_CLASS_SETTING();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    ObjModel.ACADEMIC_YEAR = sacyear;
                    var FetchFeedback = (List<FBFEEDBACK_CLASS_SETTING>)CMSHandler.FetchData<FBFEEDBACK_CLASS_SETTING>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeedbackByacyear)).DataSource.Data;
                    if (FetchFeedback != null && FetchFeedback.Count > 0)
                    {
                        foreach (var item in FetchFeedback)
                        {
                            sOption += "<option value='" + item.FEEDBACK_ID + "' >" + item.FEEDBACK_NAME + "</option>";
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
                    objHandler.WriteError("FetchProgrammeByAppType", "StudentController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchProgrammeByAppType", "StudentController", ex.Message);
                }
            }
            return Json(sOption);
        }
        public JsonResult FetchgfeedbckClassByAcyear(string sacyear, string sFeedBackId)
        {
            string sOption = string.Empty;
            FBFEEDBACK_CLASS_SETTING ObjModel = new FBFEEDBACK_CLASS_SETTING();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    ObjModel.ACADEMIC_YEAR = sacyear;
                    ObjModel.FEEDBACK_ID = sFeedBackId;
                    var FetchFeedback = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeedbackClassByFeedbck)).DataSource.Data;
                    if (FetchFeedback != null && FetchFeedback.Count > 0)
                    {
                        foreach (var item in FetchFeedback)
                        {
                            sOption += "<option value='" + item.CLASS_ID + "' >" + item.CLASS_NAME + "</option>";
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
                    objHandler.WriteError("FetchProgrammeByAppType", "StudentController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchProgrammeByAppType", "StudentController", ex.Message);
                }
            }
            return Json(sOption);
        }
        public JsonResult SaveorUpdateFeedBackClassSetting(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<FBFEEDBACK_CLASS_SETTING>(sJsonData);
                    if (result != null)
                    {
                        if (result.CLASS_SETTING_ID != null && result.CLASS_SETTING_ID != "")
                        {
                            // UPDATE setting 
                            resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.UpdateFeedBackClassSetting), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            // ADD setting 
                            resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(result, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.SaveFeedBackSClassetting).Replace("?CLASS_ID", result.CLASS_ID), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "SaveorUpdateFeedBackSetting", ex.Message);
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
        public JsonResult ActivateorDeactivateFeedBackClassSettingBySettingId(string sFeedBackId, string sMode)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                if (!string.IsNullOrEmpty(sFeedBackId) && !string.IsNullOrEmpty(sMode))
                {
                    // fetch setting 
                    resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new FBFEEDBACK_CLASS_SETTING() { CLASS_SETTING_ID = sFeedBackId, IS_ACTIVE = sMode }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.ActivateorDeactivateFeedBackClassSettingById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                }
            }
            return Json(objResultData);
        }
        public JsonResult DeleteLiFeedBackClassSettingById(string sFeedBackId)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sFeedBackId))
                    {
                        // fetch setting 
                        resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new FBFEEDBACK_CLASS_SETTING() { CLASS_SETTING_ID = sFeedBackId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.DeleteFeedBackClassSettingById), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "DeleteLiFeedBackSettingById", ex.Message);
                    }
                }

            }
            return Json(objResultData);
        }
        #endregion


        #region Feedbck Question Objective Mapping 


        public ActionResult ListFeedbackQuestionObjectiveMapping()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var fbSetting = (List<FBFEEDBACK_SETTINGS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_SETTINGS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchActiveFeedBackSettings), sAcademicYear).DataSource.Data;
                    if (fbSetting != null && fbSetting.Count > 0)
                    {
                        objViewModel.FeedBackSettingList = new SelectList(fbSetting, Common.FBFEEDBACK_SETTINGS.FEEDBACK_ID, Common.FBFEEDBACK_SETTINGS.FEEDBACK_NAME);
                    }

                    //var fbQuestions = (List<FBFEEDBACK_QUESTIONS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchFeedbackQuestionList), sAcademicYear).DataSource.Data;
                    //if (fbQuestions != null && fbQuestions.Count > 0)
                    //{
                    //    objViewModel.sQuestionList = new SelectList(fbQuestions, Common.FBFEEDBACK_QUESTIONS.QUESTION_ID, Common.FBFEEDBACK_QUESTIONS.QUESTION);
                    //}


                    var fbObjective = (List<FBOBJECTIVES>)CMS.DAO.CMSHandler.FetchData<FBOBJECTIVES>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchObjectiveList), sAcademicYear).DataSource.Data;
                    if (fbObjective != null && fbObjective.Count > 0)
                    {
                        objViewModel.sObjectiveList = new SelectList(fbObjective, Common.FBOBJECTIVES.OBJECTIVE_ID, Common.FBOBJECTIVES.OBJECTIVES);
                    }

                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "ListFeedbackQuestionObjectiveMapping", ex.Message);
                    }
                }


            }

            return View(objViewModel);
        }

        public ActionResult BindFeedbackQuestionObjectives(string sacyear, string sfeedbacksetting)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objViewModel.LiFeedbackQuestionMapping = (List<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING>(new FBFEEDBACK_CLASS_SETTING { FEEDBACK_ID = sfeedbacksetting }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchFeedbackQuestionMapping), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "ListFeedbackSetting", ex.Message);
                    }
                }

            }
            return View(objViewModel);
        }

        public JsonResult FetchQuestionByFeedbackId(string sFeedbackId)
        {
            string sOption = string.Empty;
            FBFEEDBACK_CLASS_SETTING ObjModel = new FBFEEDBACK_CLASS_SETTING();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var fbQuestions = (List<FBFEEDBACK_QUESTIONS>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_QUESTIONS>(new FBFEEDBACK_QUESTIONS() { FEEDBACK_ID = sFeedbackId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchFeedbackQuestionList), null).DataSource.Data;

                    //var FetchFeedback = (List<FBFEEDBACK_CLASS_SETTING>)CMSHandler.FetchData<FBFEEDBACK_CLASS_SETTING>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFeedbackByacyear)).DataSource.Data;
                    if (fbQuestions != null && fbQuestions.Count > 0)
                    {
                        foreach (var item in fbQuestions)
                        {
                            sOption += "<option value='" + item.QUESTION_ID + "' >" + item.QUESTION + "</option>";
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
                    objHandler.WriteError("FetchProgrammeByAppType", "StudentController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchProgrammeByAppType", "StudentController", ex.Message);
                }
            }
            return Json(sOption);
        }
        public JsonResult FetchFeedbackQuestionObjectiveMappingById(string QuestionObjectId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(QuestionObjectId))
                {
                    // fetch setting 
                    objResultData.oResult = (List<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING>)CMS.DAO.CMSHandler.FetchData<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING>(new FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING() { QUESTION_OBJECT_ID = QuestionObjectId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.FetchQuestionObjectiveMappingById), sAcademicYear).DataSource.Data;
                }
            }
            return Json(objResultData);
        }


        public JsonResult SaveOrUpdateFeedbackQuestionObjectiveMapping(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING>(sJsonData);
                    if (result != null)
                    {

                        // Splitting QUESTION_ID and OBJECTIVE_ID into lists
                        var questionIds = result.QUESTION_ID.Split(',').ToList();
                        var objectiveIds = result.OBJECTIVE_ID.Split(',').ToList();

                        bool isSuccess = true;


                        foreach (var questionId in questionIds)
                        {
                            foreach (var objectiveId in objectiveIds)
                            {
                                FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING mapping = new FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING
                                {
                                    QUESTION_OBJECT_ID = result.QUESTION_OBJECT_ID,
                                    FEEDBACK_ID = result.FEEDBACK_ID,
                                    QUESTION_ID = questionId.Trim(),
                                    OBJECTIVE_ID = objectiveId.Trim(),
                                    ACADEMIC_YEAR = sAcademicYear,
                                    IS_ACTIVE = result.IS_ACTIVE
                                };
                                if (mapping.QUESTION_OBJECT_ID != null && mapping.QUESTION_OBJECT_ID != "")
                                {
                                    // UPDATE setting 
                                    resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(mapping, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.UpdateFeedbackQuestionObjectiveMapping), sAcademicYear);
                                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                                    objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                }
                                else
                                {
                                    // ADD setting 
                                    resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(mapping, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.SaveFeedbackQuestionObjectiveMapping), sAcademicYear);
                                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                    objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                                }
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "SaveOrUpdateFeedbackQuestionObjectiveMapping", ex.Message);
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



        public JsonResult DeleteFeedbackQuestionObjectiveMappingById(string QuestionObjectId)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(QuestionObjectId))
                    {
                        // fetch setting 
                        resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING() { QUESTION_OBJECT_ID = QuestionObjectId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.DeleteFeedbackQuestionObjectiveMappingById), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("FeedBackController", "DeleteLiFeedBackSettingById", ex.Message);
                    }
                }

            }
            return Json(objResultData);
        }


        public JsonResult ActivateorDeactivateFeedbackQuestionObjectiveMappingId(string QuestionObjectId, string sMode)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                if (!string.IsNullOrEmpty(QuestionObjectId) && !string.IsNullOrEmpty(sMode))
                {
                    // fetch setting 
                    //resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new FBFEEDBACK_SETTINGS() { QUESTION_OBJECT_ID = QuestionObjectId, IS_ACTIVE = sMode }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.ActivateorDeactivateFeedBackSettingById), sAcademicYear);
                    resultArgs = CMS.DAO.CMSHandler.SaveOrUpdate(new FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING() { QUESTION_OBJECT_ID = QuestionObjectId, IS_ACTIVE = sMode }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(DAO.FeedbackSQLCommands.ActivateorDeactivateFeedbackQuestionObjectiveMappingById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                }
            }
            return Json(objResultData);
        }



        #endregion

    }
}