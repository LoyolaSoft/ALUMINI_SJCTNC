using CMS.DAO;
using CMS.Models;
using CMS.SQL.Academics;
using CMS.SQL.Fee;
using CMS.SQL.Student;
using CMS.SQL.SupportData;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CMS.Controllers
{
    public class AcademicsController : Controller
    {
        ResultArgs resultArgs = new ResultArgs();
        ResultArgs resultArg = new ResultArgs();
        DataValue dv = new DataValue();
        string sResult = string.Empty;
        // GET: Academics
        #region Academic Year
        public ActionResult AcademicYearInfo()
        {
            return View();
        }
        [UserRights("ADMIN")]
        public ActionResult ListAcademicDetails()
        {
            AcademicsViewModel objAcademicyear = new AcademicsViewModel();
            DataTable dtAcademicYearList = new DataTable();
            //IList<AcademicYearModel> liList = new List<AcademicYearModel>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    dtAcademicYearList = objAcademicyear.FetchAcademicYearDetails().DataSource.Table;
                    objAcademicyear.liAcademicYear = (from DataRow dr in dtAcademicYearList.Rows
                                                      select new AcademicYearModel
                                                      {
                                                          ACADEMIC_YEAR_ID = dr[Common.ACC_YEAR.ACADEMIC_YEAR_ID].ToString(),
                                                          AC_YEAR = dr[Common.ACC_YEAR.AC_YEAR].ToString(),
                                                          DATE_FROM = dr[Common.ACC_YEAR.DATE_FROM].ToString(),
                                                          DATE_TO = dr[Common.ACC_YEAR.DATE_TO].ToString(),
                                                          ACTIVE_YEAR = dr[Common.ACC_YEAR.ACTIVE_YEAR].ToString(),
                                                          ACADEMIC_NAME = dr[Common.ACC_YEAR.ACADEMIC_NAME].ToString(),
                                                      }).ToList();
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListAcademicDetails", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListAcademicDetails", "AcademicsController", ex.Message);
                }
            }
            return View(objAcademicyear);
        }
        public string UpdateActiveAcademic(string sActiveYearId)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (sActiveYearId != null)
                    {
                        DataTable dt = new DataTable();
                        dv.Clear();
                        dv.Add(Common.ACC_YEAR.ACADEMIC_YEAR_ID, sActiveYearId, EnumCommand.DataType.String);
                        using (AcademicsViewModel objAcademicYearView = new AcademicsViewModel())
                        {
                            DataTable dtActiveYear = new DataTable();
                            resultArg = objAcademicYearView.UpdateActiveAcademic(dv);
                            resultArg = objAcademicYearView.UpdateActiveYear(dv);
                            dt = objAcademicYearView.FetchAcademicYearDetails().DataSource.Table;
                            dtActiveYear = objAcademicYearView.FetchActiveYear().DataSource.Table;
                            sResult = resultArg.Success.ToString();
                            Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] = dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.AC_YEAR].ToString();
                            Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_NAME] = dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.ACADEMIC_NAME].ToString();
                        }
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            AcademicYearModel objAcademicYear = new AcademicYearModel()
                            {
                                AC_YEAR = dt.Rows[0][Common.ACC_YEAR.AC_YEAR].ToString(),
                                DATE_FROM = dt.Rows[0][Common.ACC_YEAR.DATE_FROM].ToString(),
                                DATE_TO = dt.Rows[0][Common.ACC_YEAR.DATE_TO].ToString(),
                                ACTIVE_YEAR = dt.Rows[0][Common.ACC_YEAR.ACTIVE_YEAR].ToString(),
                                ACADEMIC_NAME = dt.Rows[0][Common.ACC_YEAR.ACADEMIC_NAME].ToString(),
                            };
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
                    objHandler.WriteError("ListAcademicDetails", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListAcademicDetails", "AcademicsController", ex.Message);
                }
            }
            return sResult;
        }
        public string InsertAcademicYearInfo(string str)
        {
            try
            {
                AcademicYearModel objModel = new AcademicYearModel();
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (str != null)
                    {
                        string createQuery;
                        string filename;
                        var Result = JsonConvert.DeserializeObject<AcademicYearModel>(str);
                        objModel.AC_YEAR = Result.AC_YEAR;
                        var CheckExisting = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(objModel, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchExistingAcademicYear)).DataSource.Data;
                        if (CheckExisting == null)
                        {
                            dv.Clear();
                            dv.Add(Common.ACC_YEAR.AC_YEAR, Result.AC_YEAR, EnumCommand.DataType.String);
                            dv.Add(Common.ACC_YEAR.DATE_FROM, CommonMethods.ConvertdatetoyearFromat(Result.DATE_FROM), EnumCommand.DataType.String);
                            dv.Add(Common.ACC_YEAR.DATE_TO, CommonMethods.ConvertdatetoyearFromat(Result.DATE_TO), EnumCommand.DataType.String);
                            dv.Add(Common.ACC_YEAR.ACADEMIC_NAME, Result.ACADEMIC_NAME, EnumCommand.DataType.String);
                            using (AcademicsViewModel objAcademicView = new AcademicsViewModel())
                            {
                                resultArg = objAcademicView.InsertAcademicYearInfo(dv);
                                if (resultArg.Success)
                                {
                                    filename = "F:\\DB INSERT AND UPDATE\\DB SCRIPTS\\CREATE QUERY FOR ACADEMIC YEAR BASED TABLE.txt";
                                    if (System.IO.File.Exists(filename) == true)
                                    {
                                        createQuery = System.IO.File.ReadAllText(filename);
                                        createQuery = createQuery.Replace(Common.ACADEMIC_YEAR.AC_YEAR, Result.AC_YEAR);
                                        var ExecuteCreateQuery = new MySQLHandler().ExecuteAsScripts(createQuery);
                                        //System.IO.StreamReader objreader;
                                        //new System.IO.StreamReader(filename);
                                        //createQuery = objreader.ReadToEnd();
                                        //objreader.Close();
                                    }
                                }
                                sResult = resultArg.Success.ToString();
                            }
                        }
                        else
                            sResult = resultArg.Success.ToString();
                    }
                }
                else
                    sResult = Common.ErrorMessage.RequestTimeout;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("InsertAcademicYearInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("InsertAcademicYearInfo", "AcademicsController", ex.Message);
                }
            }
            return sResult;
        }
        public ActionResult EditAcademicYearInfo(int id)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYeadId = Convert.ToString(id);
                    Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] = sAcademicYeadId;
                    dv.Clear();
                    dv.Add(Common.ACC_YEAR.ACADEMIC_YEAR_ID, sAcademicYeadId, EnumCommand.DataType.String);
                    DataTable dtFetchRecordById = new DataTable();
                    using (AcademicsViewModel objAcademicyear = new AcademicsViewModel())
                    {
                        dtFetchRecordById = objAcademicyear.FetchAcademicYearById(dv).DataSource.Table;
                    }
                    //List<AcademicYearModel> liList = new List<AcademicYearModel>();
                    //liList = (from DataRow dr in dtFetchRecordById.Rows
                    AcademicYearModel objAcademicYearView = new AcademicYearModel()
                    {
                        AC_YEAR = dtFetchRecordById.Rows[0][Common.ACC_YEAR.AC_YEAR].ToString(),
                        DATE_FROM = dtFetchRecordById.Rows[0][Common.ACC_YEAR.DATE_FROM].ToString(),
                        DATE_TO = dtFetchRecordById.Rows[0][Common.ACC_YEAR.DATE_TO].ToString(),
                        ACTIVE_YEAR = dtFetchRecordById.Rows[0][Common.ACC_YEAR.ACTIVE_YEAR].ToString(),
                        ACADEMIC_NAME = dtFetchRecordById.Rows[0][Common.ACC_YEAR.ACADEMIC_NAME].ToString(),
                    };
                    return View(objAcademicYearView);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EditAcademicYearInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EditAcademicYearInfo", "AcademicsController", ex.Message);
                }
            }
            return View();
        }
        public string UpdateAcademicYearInfo(string str)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    AcademicYearModel objModel = new AcademicYearModel();
                    var sAcademicYearId = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString();
                    if (str != null)
                    {
                        var Result = JsonConvert.DeserializeObject<AcademicYearModel>(str);
                        objModel.AC_YEAR = Result.AC_YEAR;
                        var CheckExisting = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(objModel, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchExistingAcademicYear)).DataSource.Data;
                        if (CheckExisting == null)
                        {
                            dv.Clear();
                            dv.Add(Common.ACC_YEAR.AC_YEAR, Result.AC_YEAR, EnumCommand.DataType.String);
                            dv.Add(Common.ACC_YEAR.DATE_FROM, CommonMethods.ConvertdatetoyearFromat(Result.DATE_FROM), EnumCommand.DataType.String);
                            dv.Add(Common.ACC_YEAR.DATE_TO, CommonMethods.ConvertdatetoyearFromat(Result.DATE_TO), EnumCommand.DataType.String);
                            dv.Add(Common.ACC_YEAR.ACADEMIC_NAME, Result.ACADEMIC_NAME, EnumCommand.DataType.String);
                            dv.Add(Common.ACC_YEAR.ACADEMIC_YEAR_ID, sAcademicYearId, EnumCommand.DataType.String);
                            using (AcademicsViewModel objAcademicYearView = new AcademicsViewModel())
                            {
                                resultArg = objAcademicYearView.UpdateAcademicYearInfo(dv);
                                sResult = resultArg.Success.ToString();
                            }
                        }
                        else
                        {
                            if (CheckExisting.Count > 1) { sResult = resultArg.Success.ToString(); }
                            else
                            { }
                        }
                    }
                }
                else
                    sResult = Common.ErrorMessage.RequestTimeout;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateAcademicYearInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateAcademicYearInfo", "AcademicsController", ex.Message);
                }
            }
            return sResult;
        }
        public JsonResult DeleteAcademicYearInfo(string id)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (id != null)
                    {
                        using (AcademicsViewModel objAcademicYearView = new AcademicsViewModel())
                        {
                            dv.Clear();
                            dv.Add(Common.ACC_YEAR.ACADEMIC_YEAR_ID, id, EnumCommand.DataType.String);
                            resultArg = objAcademicYearView.DeleteAcademicYearInfo(dv);
                        }
                    }
                    ObjJsonData.sResult = resultArg.Success.ToString();
                    ObjJsonData.Message = (resultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
                else
                {
                    ObjJsonData.sResult = "False";
                    ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DeleteAcademicYearInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DeleteAcademicYearInfo", "AcademicsController", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.sResult = "False";
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            // return RedirectToAction("ListAcademicDetails", "Academics");
        }
        #endregion 

        #region Course root
        public ActionResult CourseRootInfo()
        {
            CourseRootModel objCourseRoot = new CourseRootModel();
            DataTable dtDepartment = new DataTable();
            DataTable dtPart = new DataTable();
            DataTable dtCourseType = new DataTable();
            DataTable dtPaperCode = new DataTable();
            DataTable dtSemester = new DataTable();
            DataTable dtUGCCourseType = new DataTable();
            DataTable dtPaperType = new DataTable();
            DataTable dtSubjectType = new DataTable();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    dtDepartment = SupportDataMethod.FetchDepartment((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    dtPart = SupportDataMethod.FetchPart().DataSource.Table;
                    dtCourseType = SupportDataMethod.FetchCourseItems((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    dtPaperCode = SupportDataMethod.FetchPaperCode().DataSource.Table;
                    dtSemester = SupportDataMethod.FetchSemester().DataSource.Table;
                    dtUGCCourseType = SupportDataMethod.FetchUGCCoursetype().DataSource.Table;
                    dtPaperType = SupportDataMethod.FetchPaperType().DataSource.Table;
                    dtSubjectType = SupportDataMethod.FetchSubjectType().DataSource.Table;

                    List<SupDepartment> liDepartment = new List<SupDepartment>();
                    List<SUP_PART> liPart = new List<SUP_PART>();
                    List<SupCourseType> liCourseType = new List<SupCourseType>();
                    List<SupPaperCode> liPaperCode = new List<SupPaperCode>();
                    List<SupSemester> liSemester = new List<SupSemester>();
                    List<SupIsAlliedSubject> liIsAllied = new List<SupIsAlliedSubject>();
                    List<SupIsCompulsory> liIsCompulsory = new List<SupIsCompulsory>();
                    List<SupUGCCourseType> liUGCCourseType = new List<SupUGCCourseType>();
                    List<SupPaperType> liPaperType = new List<SupPaperType>();
                    List<SUP_SUBJECT_TYPE> liSupbjectType = new List<SUP_SUBJECT_TYPE>();

                    if (dtDepartment != null && dtDepartment.Rows.Count > 0)
                    {
                        liDepartment = (from DataRow dr in dtDepartment.Rows
                                        select new SupDepartment()
                                        {
                                            DEPARTMENT_ID = dr[Common.SUP_DEPARTMENT.DEPARTMENT_ID].ToString(),
                                            DEPARTMENT = dr[Common.SUP_DEPARTMENT.DEPARTMENT].ToString()
                                        }).ToList();
                        objCourseRoot.DEPARTMENT = new SelectList(liDepartment, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                    }

                    if (dtPart != null && dtPart.Rows.Count > 0)
                    {
                        liPart = (from DataRow dr in dtPart.Rows
                                  select new SUP_PART()
                                  {
                                      PART_ID = dr[Common.SUP_PART.PART_ID].ToString(),
                                      PART = dr[Common.SUP_PART.PART].ToString()
                                  }).ToList();
                        objCourseRoot.PART = new SelectList(liPart, Common.SUP_PART.PART_ID, Common.SUP_PART.PART);
                    }
                    if (dtCourseType != null && dtCourseType.Rows.Count > 0)
                    {
                        liCourseType = (from DataRow dr in dtCourseType.Rows
                                        select new SupCourseType()
                                        {
                                            COURSE_TYPE_ID = dr[Common.CP_COURSE_TYPE_2017.COURSE_TYPE_ID].ToString(),
                                            COURSE_TYPE = dr[Common.CP_COURSE_TYPE_2017.COURSE_TYPE].ToString()
                                        }).ToList();
                        objCourseRoot.COURSE_TYPE = new SelectList(liCourseType, Common.CP_COURSE_TYPE_2017.COURSE_TYPE_ID, Common.CP_COURSE_TYPE_2017.COURSE_TYPE);
                    }

                    if (dtPaperCode != null && dtPaperCode.Rows.Count > 0)
                    {
                        liPaperCode = (from DataRow dr in dtPaperCode.Rows
                                       select new SupPaperCode()
                                       {
                                           PAPER_CODE_ID = dr[Common.SUP_PAPER_CODE.PAPER_CODE_ID].ToString(),
                                           PAPER_CODE = dr[Common.SUP_PAPER_CODE.PAPER_CODE].ToString()
                                       }).ToList();
                        objCourseRoot.PAPER_CODE = new SelectList(liPaperCode, Common.SUP_PAPER_CODE.PAPER_CODE_ID, Common.SUP_PAPER_CODE.PAPER_CODE);
                    }
                    if (dtSemester != null && dtSemester.Rows.Count > 0)
                    {
                        liSemester = (from DataRow dr in dtSemester.Rows
                                      select new SupSemester()
                                      {
                                          SUP_SEMESTER_ID = dr[Common.SUP_SEMESTER.SUP_SEMESTER_ID].ToString(),
                                          SEMESTER_NAME = dr[Common.SUP_SEMESTER.SEMESTER_NAME].ToString()
                                      }).ToList();
                        objCourseRoot.SEMESTER = new SelectList(liSemester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    }

                    if (dtUGCCourseType != null && dtUGCCourseType.Rows.Count > 0)
                    {
                        liUGCCourseType = (from DataRow dr in dtUGCCourseType.Rows
                                           select new SupUGCCourseType()
                                           {
                                               UGC_COURSE_TYPE_ID = dr[Common.CP_UGC_COURSE_TYPE.UGC_COURSE_TYPE_ID].ToString(),
                                               UGC_COURSE_TYPE = dr[Common.CP_UGC_COURSE_TYPE.UGC_COURSE_TYPE].ToString()
                                           }).ToList();
                        objCourseRoot.UGC_COURSE_TYPE = new SelectList(liUGCCourseType, Common.CP_UGC_COURSE_TYPE.UGC_COURSE_TYPE_ID, Common.CP_UGC_COURSE_TYPE.UGC_COURSE_TYPE);
                    }
                    if (dtPaperType != null && dtPaperType.Rows.Count > 0)
                    {
                        liPaperType = (from DataRow dr in dtPaperType.Rows
                                       select new SupPaperType()
                                       {
                                           PAPER_TYPE_ID = dr[Common.SUP_PAPER_TYPE.PAPER_TYPE_ID].ToString(),
                                           PAPER_TYPE = dr[Common.SUP_PAPER_TYPE.PAPER_TYPE].ToString()
                                       }).ToList();
                        objCourseRoot.PAPER_TYPE = new SelectList(liPaperType, Common.SUP_PAPER_TYPE.PAPER_TYPE_ID, Common.SUP_PAPER_TYPE.PAPER_TYPE);
                    }
                    if (dtSubjectType != null && dtSubjectType.Rows.Count > 0)
                    {
                        liSupbjectType = (from DataRow dr in dtSubjectType.Rows
                                          select new SUP_SUBJECT_TYPE()
                                          {
                                              SUBJECT_TYPE_ID = dr[Common.SUP_SUBJECT_TYPE.SUBJECT_TYPE_ID].ToString(),
                                              SUBJECT_TYPE = dr[Common.SUP_SUBJECT_TYPE.SUBJECT_TYPE].ToString()
                                          }).ToList();
                        objCourseRoot.SUBJECT_TYPE = new SelectList(liSupbjectType, Common.SUP_SUBJECT_TYPE.SUBJECT_TYPE_ID, Common.SUP_SUBJECT_TYPE.SUBJECT_TYPE);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CourseRootInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CourseRootInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objCourseRoot);
        }
        [UserRights("ADMIN")]
        public ActionResult ListCourseInfo()
        {
            AcademicsViewModel objCourseRootView = new AcademicsViewModel();
            // IList<CourseRootInfo> liCourseRoot = new List<CourseRootInfo>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    DataTable dtCourseRootInfo = new DataTable();
                    dtCourseRootInfo = objCourseRootView.FetchCourseRootInfo((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    if (dtCourseRootInfo != null && dtCourseRootInfo.Rows.Count > 0)
                    {
                        objCourseRootView.liCourseRootList = (from DataRow dr in dtCourseRootInfo.Rows
                                                              select new CourseRootInfo
                                                              {
                                                                  COURSE_ROOT_ID = dr[Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID].ToString(),
                                                                  DEPARTMENT = dr[Common.CP_DEPARTMENT_2017.DEPARTMENT].ToString(),
                                                                  YEAR = dr[Common.CP_COURSE_ROOT_2017.YEAR].ToString(),
                                                                  PART = dr[Common.SUP_PART.PART].ToString(),
                                                                  COURSE_TYPE = dr[Common.CP_COURSE_TYPE_2017.COURSE_TYPE].ToString(),
                                                                  COURSE_TITLE = dr[Common.CP_COURSE_ROOT_2017.COURSE_TITLE].ToString(),
                                                                  COURSE_CODE = dr[Common.CP_COURSE_ROOT_2017.COURSE_CODE].ToString(),
                                                                  SEMESTER = dr[Common.SUP_SEMESTER.SEMESTER_NAME].ToString(),
                                                                  IS_NME_SUBJECT = dr[Common.CP_COURSE_ROOT_2017.IS_NME_SUBJECT].ToString(),
                                                                  IS_ALLIED_SUBJECT = dr[Common.CP_COURSE_ROOT_2017.IS_ALLIED_SUBJECT].ToString()
                                                              }).ToList();
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListCourseInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListCourseInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objCourseRootView);
        }
        public JsonResult InsertCourseRootInfo(string str)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (str != null)
                    {
                        DataTable dtCourseRoot = new DataTable();
                        var Result = JsonConvert.DeserializeObject<CourseRootInfo>(str);
                        string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                        dv.Clear();
                        dv.Add(Common.CP_COURSE_ROOT_2017.COURSE_CODE, Result.COURSE_CODE, EnumCommand.DataType.String);
                        using (AcademicsViewModel objCourseRoot = new AcademicsViewModel())
                        {
                            dtCourseRoot = objCourseRoot.CheckCourseCode(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                            if (dtCourseRoot != null && dtCourseRoot.Rows.Count == 0)
                            {
                                dv.Clear();
                                dv.Add(Common.CP_COURSE_ROOT_2017.DEPARTMENT, Result.DEPARTMENT, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.YEAR_FROM, Result.YEAR_FROM, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.YEAR_TO, Result.YEAR_TO, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.YEAR, Result.YEAR, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.PART, Result.PART, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.COURSE_TYPE, Result.COURSE_TYPE, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.HRS_PER_WEEK, Result.HRS_PER_WEEK, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.CREDITS, Result.CREDITS, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.PAPER_CODE, Result.PAPER_CODE, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.COURSE_TITLE, Result.COURSE_TITLE, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.CONS_COURSE_TITLE, Result.CONS_COURSE_TITLE, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.COURSE_CODE, Result.COURSE_CODE, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.SEMESTER_ID, Result.SEMESTER, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.IS_NME_SUBJECT, Result.IS_NME_SUBJECT, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.IS_ALLIED_SUBJECT, Result.IS_ALLIED_SUBJECT, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.COURSE_ORDER, Result.COURSE_ORDER, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.SUBJECTS, Result.SUBJECTS, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.IS_COMPULSORY, Result.IS_COMPULSORY_SUBJECT, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.UGC_COURSE_TYPE, Result.UGC_COURSE_TYPE, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.PAPER_TYPE, Result.PAPER_TYPE, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.SUBJECT_TYPE, Result.SUBJECT_TYPE, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.INTERNAL, Result.INTERNAL, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.EXTERNAL, Result.EXTERNAL, EnumCommand.DataType.String);
                                dv.Add(Common.CP_COURSE_ROOT_2017.TOTAL, Result.TOTAL, EnumCommand.DataType.String);
                                resultArg = objCourseRoot.InsertCourseRootInfo(dv, sAcademicYear);
                                sResult = resultArg.Success.ToString();
                                if (sResult == "True")
                                    sResult = Common.Messages.RecordsSavedSuccessfully;
                                else
                                    sResult = "Enter Correct Values";
                            }
                            else
                                sResult = "Course Code is already exist..!";
                        }
                    }
                }
                else
                    return Json(sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("InsertCourseRootInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("InsertCourseRootInfo", "AcademicsController", ex.Message);
                }
            }
            return Json(sResult);
        }
        public JsonResult DeleteCourseRoot(string id)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    using (AcademicsViewModel objCourseRoot = new AcademicsViewModel())
                    {
                        dv.Clear();
                        dv.Add(Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, id, EnumCommand.DataType.String);
                        resultArg = objCourseRoot.DeleteCourseRoot(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                        sResult = resultArg.Success.ToString();
                    }
                    ObjJsonData.Message = (resultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.sResult = sResult;
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    ObjJsonData.sResult = "False";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DeleteCourseRoot", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DeleteCourseRoot", "AcademicsController", ex.Message);
                }
                ObjJsonData.sResult = "False";
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditCourseRootInfo(string id)
        {
            CourseRootModel objCourseRoot = new CourseRootModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    dv.Clear();
                    dv.Add(Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, id, EnumCommand.DataType.String);
                    DataTable dtFetchCourseRootById = new DataTable();
                    Session[Common.SESSION_VARIABLES.COURSE_ROOT_ID] = id;
                    DataTable dtDepartment = new DataTable();
                    DataTable dtPart = new DataTable();
                    DataTable dtCourseType = new DataTable();
                    DataTable dtPaperCode = new DataTable();
                    DataTable dtSemester = new DataTable();
                    DataTable dtUGCCourseType = new DataTable();
                    DataTable dtPaperType = new DataTable();
                    DataTable dtSubjectType = new DataTable();

                    dtDepartment = SupportDataMethod.FetchDepartment((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    dtPart = SupportDataMethod.FetchPart().DataSource.Table;
                    dtCourseType = SupportDataMethod.FetchCourseItems((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    dtPaperCode = SupportDataMethod.FetchPaperCode().DataSource.Table;
                    dtSemester = SupportDataMethod.FetchSemester().DataSource.Table;
                    dtUGCCourseType = SupportDataMethod.FetchUGCCoursetype().DataSource.Table;
                    dtPaperType = SupportDataMethod.FetchPaperType().DataSource.Table;
                    dtSubjectType = SupportDataMethod.FetchSubjectType().DataSource.Table;

                    List<SupDepartment> liDepartment = new List<SupDepartment>();
                    List<SUP_PART> liPart = new List<SUP_PART>();
                    List<SupCourseType> liCourseType = new List<SupCourseType>();
                    List<SupPaperCode> liPaperCode = new List<SupPaperCode>();
                    List<SupSemester> liSemester = new List<SupSemester>();
                    List<SupUGCCourseType> liUGCCourseType = new List<SupUGCCourseType>();
                    List<SupPaperType> liPaperType = new List<SupPaperType>();
                    List<SUP_SUBJECT_TYPE> liSupbjectType = new List<SUP_SUBJECT_TYPE>();

                    if (dtDepartment != null && dtDepartment.Rows.Count > 0)
                    {
                        liDepartment = (from DataRow dr in dtDepartment.Rows
                                        select new SupDepartment()
                                        {
                                            DEPARTMENT_ID = dr[Common.SUP_DEPARTMENT.DEPARTMENT_ID].ToString(),
                                            DEPARTMENT = dr[Common.SUP_DEPARTMENT.DEPARTMENT].ToString()
                                        }).ToList();
                        objCourseRoot.DEPARTMENT = new SelectList(liDepartment, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                    }

                    if (dtPart != null && dtPart.Rows.Count > 0)
                    {
                        liPart = (from DataRow dr in dtPart.Rows
                                  select new SUP_PART()
                                  {
                                      PART_ID = dr[Common.SUP_PART.PART_ID].ToString(),
                                      PART = dr[Common.SUP_PART.PART].ToString()
                                  }).ToList();
                        objCourseRoot.PART = new SelectList(liPart, Common.SUP_PART.PART_ID, Common.SUP_PART.PART);
                    }
                    if (dtCourseType != null && dtCourseType.Rows.Count > 0)
                    {
                        liCourseType = (from DataRow dr in dtCourseType.Rows
                                        select new SupCourseType()
                                        {
                                            COURSE_TYPE_ID = dr[Common.CP_COURSE_TYPE_2017.COURSE_TYPE_ID].ToString(),
                                            COURSE_TYPE = dr[Common.CP_COURSE_TYPE_2017.COURSE_TYPE].ToString()
                                        }).ToList();
                        objCourseRoot.COURSE_TYPE = new SelectList(liCourseType, Common.CP_COURSE_TYPE_2017.COURSE_TYPE_ID, Common.CP_COURSE_TYPE_2017.COURSE_TYPE);
                    }

                    if (dtPaperCode != null && dtPaperCode.Rows.Count > 0)
                    {
                        liPaperCode = (from DataRow dr in dtPaperCode.Rows
                                       select new SupPaperCode()
                                       {
                                           PAPER_CODE_ID = dr[Common.SUP_PAPER_CODE.PAPER_CODE_ID].ToString(),
                                           PAPER_CODE = dr[Common.SUP_PAPER_CODE.PAPER_CODE].ToString()
                                       }).ToList();
                        objCourseRoot.PAPER_CODE = new SelectList(liPaperCode, Common.SUP_PAPER_CODE.PAPER_CODE_ID, Common.SUP_PAPER_CODE.PAPER_CODE);
                    }
                    if (dtSemester != null && dtSemester.Rows.Count > 0)
                    {
                        liSemester = (from DataRow dr in dtSemester.Rows
                                      select new SupSemester()
                                      {
                                          SUP_SEMESTER_ID = dr[Common.SUP_SEMESTER.SUP_SEMESTER_ID].ToString(),
                                          SEMESTER_NAME = dr[Common.SUP_SEMESTER.SEMESTER_NAME].ToString()
                                      }).ToList();
                        objCourseRoot.SEMESTER = new SelectList(liSemester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    }

                    if (dtUGCCourseType != null && dtUGCCourseType.Rows.Count > 0)
                    {
                        liUGCCourseType = (from DataRow dr in dtUGCCourseType.Rows
                                           select new SupUGCCourseType()
                                           {
                                               UGC_COURSE_TYPE_ID = dr[Common.CP_UGC_COURSE_TYPE.UGC_COURSE_TYPE_ID].ToString(),
                                               UGC_COURSE_TYPE = dr[Common.CP_UGC_COURSE_TYPE.UGC_COURSE_TYPE].ToString()
                                           }).ToList();
                        objCourseRoot.UGC_COURSE_TYPE = new SelectList(liUGCCourseType, Common.CP_UGC_COURSE_TYPE.UGC_COURSE_TYPE_ID, Common.CP_UGC_COURSE_TYPE.UGC_COURSE_TYPE);
                    }
                    if (dtPaperType != null && dtPaperType.Rows.Count > 0)
                    {
                        liPaperType = (from DataRow dr in dtPaperType.Rows
                                       select new SupPaperType()
                                       {
                                           PAPER_TYPE_ID = dr[Common.SUP_PAPER_TYPE.PAPER_TYPE_ID].ToString(),
                                           PAPER_TYPE = dr[Common.SUP_PAPER_TYPE.PAPER_TYPE].ToString()
                                       }).ToList();
                        objCourseRoot.PAPER_TYPE = new SelectList(liPaperType, Common.SUP_PAPER_TYPE.PAPER_TYPE_ID, Common.SUP_PAPER_TYPE.PAPER_TYPE);
                    }
                    if (dtSubjectType != null && dtSubjectType.Rows.Count > 0)
                    {
                        liSupbjectType = (from DataRow dr in dtSubjectType.Rows
                                          select new SUP_SUBJECT_TYPE()
                                          {
                                              SUBJECT_TYPE_ID = dr[Common.SUP_SUBJECT_TYPE.SUBJECT_TYPE_ID].ToString(),
                                              SUBJECT_TYPE = dr[Common.SUP_SUBJECT_TYPE.SUBJECT_TYPE].ToString()
                                          }).ToList();
                        objCourseRoot.SUBJECT_TYPE = new SelectList(liSupbjectType, Common.SUP_SUBJECT_TYPE.SUBJECT_TYPE_ID, Common.SUP_SUBJECT_TYPE.SUBJECT_TYPE);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EditCourseRootInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EditCourseRootInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objCourseRoot);
        }
        public JsonResult FetchCourseRootInfo(string id)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    dv.Clear();
                    dv.Add(Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, id, EnumCommand.DataType.String);
                    DataTable dtFetchCourseRootById = new DataTable();
                    AcademicsViewModel objCourseRootView = new AcademicsViewModel();
                    dtFetchCourseRootById = objCourseRootView.FetchCourseRootInfoById(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    if (dtFetchCourseRootById != null && dtFetchCourseRootById.Rows.Count > 0)
                    {
                        CourseRootInfo objCourseRoot = new CourseRootInfo()
                        {
                            DEPARTMENT = dtFetchCourseRootById.Rows[0][Common.CP_COURSE_ROOT_2017.DEPARTMENT].ToString(),
                            YEAR_FROM = dtFetchCourseRootById.Rows[0][Common.CP_COURSE_ROOT_2017.YEAR_FROM].ToString(),
                            YEAR_TO = dtFetchCourseRootById.Rows[0][Common.CP_COURSE_ROOT_2017.YEAR_TO].ToString(),
                            SEMESTER_FROM = dtFetchCourseRootById.Rows[0]["SEMESTER_FROM"].ToString(),
                            SEMESTER_TO = dtFetchCourseRootById.Rows[0]["SEMESTER_TO"].ToString(),
                            YEAR = dtFetchCourseRootById.Rows[0][Common.CP_COURSE_ROOT_2017.YEAR].ToString(),
                            PART = dtFetchCourseRootById.Rows[0]["PART"].ToString(),
                            COURSE_TYPE = dtFetchCourseRootById.Rows[0]["COURSE_TYPE"].ToString(),
                            HRS_PER_WEEK = dtFetchCourseRootById.Rows[0][Common.CP_COURSE_ROOT_2017.HRS_PER_WEEK].ToString(),
                            CREDITS = dtFetchCourseRootById.Rows[0][Common.CP_COURSE_ROOT_2017.CREDITS].ToString(),
                            OPTION_NAME = dtFetchCourseRootById.Rows[0]["OPTION_NAME"].ToString(),
                            PAPER_CODE = dtFetchCourseRootById.Rows[0][Common.CP_COURSE_ROOT_2017.PAPER_CODE].ToString(),
                            COURSE_TITLE = dtFetchCourseRootById.Rows[0][Common.CP_COURSE_ROOT_2017.COURSE_TITLE].ToString(),
                            CONS_COURSE_TITLE = dtFetchCourseRootById.Rows[0][Common.CP_COURSE_ROOT_2017.CONS_COURSE_TITLE].ToString(),
                            COURSE_CODE = dtFetchCourseRootById.Rows[0][Common.CP_COURSE_ROOT_2017.COURSE_CODE].ToString(),
                            SEMESTER = dtFetchCourseRootById.Rows[0]["SEMESTER_ID"].ToString(),
                            IS_NME_SUBJECT = dtFetchCourseRootById.Rows[0]["IS_NME_SUBJECT"].ToString(),
                            IS_ALLIED_SUBJECT = dtFetchCourseRootById.Rows[0]["IS_ALLIED_SUBJECT"].ToString(),
                            COURSE_ORDER = dtFetchCourseRootById.Rows[0][Common.CP_COURSE_ROOT_2017.COURSE_ORDER].ToString(),
                            SUBJECTS = dtFetchCourseRootById.Rows[0][Common.CP_COURSE_ROOT_2017.SUBJECTS].ToString(),
                            IS_COMPULSORY_SUBJECT = dtFetchCourseRootById.Rows[0]["IS_COMPULSORY"].ToString(),
                            UGC_COURSE_TYPE = dtFetchCourseRootById.Rows[0]["UGC_COURSE_TYPE"].ToString(),
                            PAPER_TYPE = dtFetchCourseRootById.Rows[0][Common.SUP_PAPER_TYPE.PAPER_TYPE].ToString(),
                            SUBJECT_TYPE = dtFetchCourseRootById.Rows[0][Common.SUP_SUBJECT_TYPE.SUBJECT_TYPE].ToString(),
                            INTERNAL = dtFetchCourseRootById.Rows[0][Common.CP_COURSE_ROOT_2017.INTERNAL].ToString(),
                            EXTERNAL = dtFetchCourseRootById.Rows[0][Common.CP_COURSE_ROOT_2017.EXTERNAL].ToString(),
                            TOTAL = dtFetchCourseRootById.Rows[0][Common.CP_COURSE_ROOT_2017.TOTAL].ToString(),
                        };
                        return Json(objCourseRoot);
                    }
                    else
                    {
                        return Json(sResult);
                    }
                }
                else
                    return Json(sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchCourseRootInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchCourseRootInfo", "AcademicsController", ex.Message);
                    return Json(sResult = ex.Message);
                }
            }
        }
        public ActionResult UpdateCourseRootInfo(string id)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var sCourseRootId = Session[Common.SESSION_VARIABLES.COURSE_ROOT_ID].ToString();
                    if (id != null)
                    {
                        var Result = JsonConvert.DeserializeObject<CourseRootInfo>(id);
                        dv.Clear();
                        dv.Add(Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, sCourseRootId, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.DEPARTMENT, Result.DEPARTMENT, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.YEAR_FROM, Result.YEAR_FROM, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.YEAR_TO, Result.YEAR_TO, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.YEAR, Result.YEAR, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.PART, Result.PART, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.COURSE_TYPE, Result.COURSE_TYPE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.HRS_PER_WEEK, Result.HRS_PER_WEEK, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.CREDITS, Result.CREDITS, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.PAPER_CODE, Result.PAPER_CODE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.COURSE_TITLE, Result.COURSE_TITLE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.CONS_COURSE_TITLE, Result.CONS_COURSE_TITLE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.COURSE_CODE, Result.COURSE_CODE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.SEMESTER_ID, Result.SEMESTER, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.IS_NME_SUBJECT, Result.IS_NME_SUBJECT, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.IS_ALLIED_SUBJECT, Result.IS_ALLIED_SUBJECT, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.COURSE_ORDER, Result.COURSE_ORDER, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.SUBJECTS, Result.SUBJECTS, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.IS_COMPULSORY, Result.IS_COMPULSORY_SUBJECT, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.UGC_COURSE_TYPE, Result.UGC_COURSE_TYPE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.PAPER_TYPE, Result.PAPER_TYPE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.SUBJECT_TYPE, Result.SUBJECT_TYPE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.INTERNAL, Result.INTERNAL, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.EXTERNAL, Result.EXTERNAL, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_ROOT_2017.TOTAL, Result.TOTAL, EnumCommand.DataType.String);
                        using (AcademicsViewModel objCourseRoot = new AcademicsViewModel())
                        {
                            resultArg = objCourseRoot.UpdateCourseRootInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                            sResult = resultArg.Success.ToString();
                            if (resultArg.Success)
                                objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                            else
                                objResult.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }
                    else
                        objResult.Message = Common.ErrorMessage.NoContent;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateCourseRootInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateCourseRootInfo", "AcademicsController", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Course Wise Staff
        public ActionResult CourseWiseStaffInfo()
        {

            CourseWiseStaffModel objCourseWiseStaff = new CourseWiseStaffModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    DataTable dtDepartment = new DataTable();

                    DataTable dtCourse = new DataTable();
                    DataTable dtStaff = new DataTable();
                    DataTable dtShift = new DataTable();
                    DataTable dtSemester = new DataTable();
                    DataTable dtClasstype = new DataTable();

                    dtDepartment = SupportDataMethod.FetchDepartment((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;

                    dtCourse = SupportDataMethod.FetchCourses((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    dtStaff = SupportDataMethod.FetchStaff().DataSource.Table;
                    dtShift = SupportDataMethod.FetchShift().DataSource.Table;
                    dtSemester = SupportDataMethod.FetchSemester().DataSource.Table;
                    dtClasstype = SupportDataMethod.FetchClassType().DataSource.Table;

                    List<SupDepartment> liDepartment = new List<SupDepartment>();

                    List<cp_Course_Root_2017> liCourse = new List<cp_Course_Root_2017>();
                    List<stf_Personal_Info> liStaff = new List<stf_Personal_Info>();
                    List<Sup_Shift> liShit = new List<Sup_Shift>();
                    List<SupSemester> liSemester = new List<SupSemester>();
                    List<SupClassType> liClassType = new List<SupClassType>();

                    if (dtDepartment != null && dtDepartment.Rows.Count > 0)
                    {
                        liDepartment = (from DataRow dr in dtDepartment.Rows
                                        select new SupDepartment()
                                        {
                                            DEPARTMENT_ID = dr[Common.SUP_DEPARTMENT.DEPARTMENT_ID].ToString(),
                                            DEPARTMENT = dr[Common.SUP_DEPARTMENT.DEPARTMENT].ToString()
                                        }).ToList();
                        objCourseWiseStaff.DEPARTMENT = new SelectList(liDepartment, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                    }


                    if (dtClasstype != null && dtClasstype.Rows.Count > 0)
                    {
                        liClassType = (from DataRow dr in dtClasstype.Rows
                                       select new SupClassType()
                                       {
                                           CLASS_TYPE_ID = dr[Common.SUP_CLASS_TYPE.CLASS_TYPE_ID].ToString(),
                                           CLASS_TYPE = dr[Common.SUP_CLASS_TYPE.CLASS_TYPE].ToString()
                                       }).ToList();
                        objCourseWiseStaff.CLASS_TYPE = new SelectList(liClassType, Common.SUP_CLASS_TYPE.CLASS_TYPE_ID, Common.SUP_CLASS_TYPE.CLASS_TYPE);
                    }
                    if (dtCourse != null && dtCourse.Rows.Count > 0)
                    {
                        liCourse = (from DataRow dr in dtCourse.Rows
                                    select new cp_Course_Root_2017()
                                    {
                                        COURSE_ROOT_ID = dr[Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID].ToString(),
                                        COURSE_CODE = dr[Common.CP_COURSE_ROOT_2017.COURSE_CODE].ToString()
                                    }).ToList();
                        objCourseWiseStaff.COURSE_ID = new SelectList(liCourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
                    }
                    if (dtStaff != null && dtStaff.Rows.Count > 0)
                    {
                        liStaff = (from DataRow dr in dtStaff.Rows
                                   select new stf_Personal_Info()
                                   {
                                       STAFF_ID = dr[Common.STF_PERSONAL_INFO.STAFF_ID].ToString(),
                                       STAFF_CODE = dr["STAFF_NAME"].ToString()
                                   }).ToList();
                        objCourseWiseStaff.STAFF_ID = new SelectList(liStaff, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.STAFF_CODE);
                    }
                    if (dtShift != null && dtShift.Rows.Count > 0)
                    {
                        liShit = (from DataRow dr in dtShift.Rows
                                  select new Sup_Shift()
                                  {
                                      SHIFT_ID = dr[Common.SUP_SHIFT.SHIFT_ID].ToString(),
                                      SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString()
                                  }).ToList();
                        objCourseWiseStaff.SHIFT = new SelectList(liShit, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    if (dtSemester != null && dtSemester.Rows.Count > 0)
                    {
                        liSemester = (from DataRow dr in dtSemester.Rows
                                      select new SupSemester()
                                      {
                                          SUP_SEMESTER_ID = dr[Common.SUP_SEMESTER.SUP_SEMESTER_ID].ToString(),
                                          SEMESTER_NAME = dr[Common.SUP_SEMESTER.SEMESTER_NAME].ToString()
                                      }).ToList();
                        objCourseWiseStaff.SEMESTER_ID = new SelectList(liSemester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    }

                    var liSemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null,
                            SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType), sAcademicYear).DataSource.Data;
                    if (liSemesterType != null && liSemesterType.Count > 0)
                    {
                        objCourseWiseStaff.SemesterType = new SelectList(liSemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CourseWiseStaffInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CourseWiseStaffInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objCourseWiseStaff);
        }
        [UserRights("ADMIN")]
        public ActionResult ListCourseWiseStaffInfo()
        {
            AcademicsViewModel objCourseWiseStaff = new AcademicsViewModel();
            // IList<CourseWiseStaffInfo> liCourseWiseStaff = new List<CourseWiseStaffInfo>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
                    DataTable dtCourseWiseStaff = new DataTable();
                    dtCourseWiseStaff = objCourseWiseStaff.FetchCourseWiseStaff(sAcademicYear, sActiveSemester).DataSource.Table;
                    if (dtCourseWiseStaff != null && dtCourseWiseStaff.Rows.Count > 0)
                    {
                        objCourseWiseStaff.liCourseWiseStaff = (from DataRow dr in dtCourseWiseStaff.Rows
                                                                select new CourseWiseStaffInfo()
                                                                {
                                                                    CLSCRSSTF_ID = dr[Common.CP_CLASS_COURSE_STAFF_2017.CLSCRSSTF_ID].ToString(),
                                                                    CLASS_ID = dr[Common.CP_CLASSES_2017.CLASS_ID].ToString(),
                                                                    CLASS_NAME = dr[Common.CP_CLASSES_2017.CLASS_NAME].ToString(),
                                                                    COURSE_ID = dr[Common.CP_CLASS_COURSE_STAFF_2017.COURSE_ID].ToString(),
                                                                    COURSE_CODE = dr[Common.CP_COURSE_ROOT_2017.COURSE_CODE].ToString(),
                                                                    COURSE_TITLE = dr[Common.CP_COURSE_ROOT_2017.COURSE_TITLE].ToString(),
                                                                    STAFF_ID = dr[Common.CP_CLASS_COURSE_STAFF_2017.STAFF_NAME].ToString(),
                                                                    STAFF_COUNT = dr[Common.CP_CLASS_COURSE_STAFF_2017.STAFF_COUNT].ToString(),
                                                                    SEMESTER_ID = dr[Common.CP_CLASS_COURSE_STAFF_2017.SEMESTER_ID].ToString(),
                                                                    SHIFT = dr[Common.CP_CLASS_COURSE_STAFF_2017.SHIFT].ToString(),
                                                                }).ToList();
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListCourseWiseStaffInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListCourseWiseStaffInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objCourseWiseStaff);
        }
        public JsonResult InsertCourseWiseStaff(string sJsonCourseWiseStaff)
        {
            string Result = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    JsonCourseWiseStaff objJson = JsonConvert.DeserializeObject<JsonCourseWiseStaff>(sJsonCourseWiseStaff);
                    AcademicsViewModel objCourseWiseStaff = new AcademicsViewModel();
                    sResult = objCourseWiseStaff.BulkSaveCourseWiseStaff(objJson, Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString());


                }
                else
                    sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("InsertCourseWiseStaff", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("InsertCourseWiseStaff", "AcademicsController", ex.Message);
                }
            }
            return Json(sResult);
        }
        public JsonResult DeleteCourseWiseStaffInfo(string id)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    using (AcademicsViewModel objCourseWiseStaff = new AcademicsViewModel())
                    {
                        dv.Clear();
                        dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.CLSCRSSTF_ID, id, EnumCommand.DataType.String);
                        resultArg = objCourseWiseStaff.DeleteCourseWiseStaffINfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                        sResult = resultArg.Success.ToString();
                    }
                    ObjJsonData.Message = (resultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.sResult = sResult;
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    ObjJsonData.sResult = "False";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DeleteCourseWiseStaffInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DeleteCourseWiseStaffInfo", "AcademicsController", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.sResult = "False";
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public string GetClassListByClassTypeId(string sDepartment, string sShift, string sClassType)
        {
            string sOption = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    DataTable dtClassList = new DataTable();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    using (AcademicsViewModel objAcademicsview = new AcademicsViewModel())
                    {
                        dv.Clear();
                        dv.Add(Common.CP_CLASSES_2017.DEPARTMENT, sDepartment, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.SHIFT, sShift, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.CLASS_TYPE, sClassType, EnumCommand.DataType.String);
                        dtClassList = objAcademicsview.GetClassListByClassTypeId(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                        foreach (DataRow item in dtClassList.Rows)
                        {
                            sOption += "<option value='" + item[Common.CP_CLASSES_2017.CLASS_ID].ToString() + "' >" + item[Common.CP_CLASSES_2017.CLASS_CODE].ToString() + "</option>";
                        }
                    }
                }
                else
                    sOption = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("GetClassListByClassTypeId", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("GetClassListByClassTypeId", "AcademicsController", ex.Message);
                }
            }
            return sOption;
        }
        public string GetClassTypeByShiftId(string sShift)
        {
            string sOption = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    DataTable dtClassType = new DataTable();
                    using (AcademicsViewModel objAcademicsview = new AcademicsViewModel())
                    {
                        dv.Clear();
                        dv.Add(Common.CP_CLASSES_2017.DEPARTMENT, sShift, EnumCommand.DataType.String);
                        dtClassType = objAcademicsview.GetClassTypeByShiftId(dv).DataSource.Table;
                        foreach (DataRow item in dtClassType.Rows)
                        {
                            sOption += "<option value='" + item[Common.SUP_CLASS_TYPE.CLASS_TYPE_ID].ToString() + "' >" + item[Common.SUP_CLASS_TYPE.CLASS_TYPE].ToString() + "</option>";
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
                    objHandler.WriteError("GetClassTypeByShiftId", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("GetClassTypeByShiftId", "AcademicsController", ex.Message);
                }
            }
            return sOption;
        }
        public ActionResult EditCourseWiseStaff(string id)
        {
            CourseWiseStaffModel objCourseWiseStaff = new CourseWiseStaffModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    dv.Clear();
                    dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.CLSCRSSTF_ID, id, EnumCommand.DataType.String);
                    DataTable dtFetchCourseWiseStaffInfoById = new DataTable();
                    Session[Common.SESSION_VARIABLES.CLSCRSSTF_ID] = id;

                    DataTable dtClass = new DataTable();
                    DataTable dtCourse = new DataTable();
                    DataTable dtStaff = new DataTable();
                    DataTable dtShift = new DataTable();
                    DataTable dtSemester = new DataTable();

                    dtClass = SupportDataMethod.FetchClass(sAcademicYear).DataSource.Table;
                    dtCourse = SupportDataMethod.FetchCourses((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    dtStaff = SupportDataMethod.FetchStaff().DataSource.Table;
                    dtShift = SupportDataMethod.FetchShift().DataSource.Table;
                    dtSemester = SupportDataMethod.FetchSemester().DataSource.Table;

                    List<cp_Classes_2017> liClass = new List<cp_Classes_2017>();
                    List<cp_Course_Root_2017> liCourse = new List<cp_Course_Root_2017>();
                    List<stf_Personal_Info> liStaff = new List<stf_Personal_Info>();
                    List<Sup_Shift> liShit = new List<Sup_Shift>();
                    List<SupSemester> liSemester = new List<SupSemester>();

                    if (dtClass != null && dtClass.Rows.Count > 0)
                    {
                        liClass = (from DataRow dr in dtClass.Rows
                                   select new cp_Classes_2017()
                                   {
                                       CLASS_ID = dr[Common.CP_CLASSES_2017.CLASS_ID].ToString(),
                                       CLASS_NAME = dr[Common.CP_CLASSES_2017.CLASS_NAME].ToString()
                                   }).ToList();
                        objCourseWiseStaff.CLASS_ID = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                    if (dtCourse != null && dtCourse.Rows.Count > 0)
                    {
                        liCourse = (from DataRow dr in dtCourse.Rows
                                    select new cp_Course_Root_2017()
                                    {
                                        COURSE_ROOT_ID = dr[Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID].ToString(),
                                        COURSE_CODE = dr[Common.CP_COURSE_ROOT_2017.COURSE_CODE].ToString()
                                    }).ToList();
                        objCourseWiseStaff.COURSE_ID = new SelectList(liCourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
                    }
                    if (dtStaff != null && dtStaff.Rows.Count > 0)
                    {
                        liStaff = (from DataRow dr in dtStaff.Rows
                                   select new stf_Personal_Info()
                                   {
                                       STAFF_ID = dr[Common.STF_PERSONAL_INFO.STAFF_ID].ToString(),
                                       STAFF_CODE = dr["STAFF_NAME"].ToString()
                                   }).ToList();
                        objCourseWiseStaff.STAFF_ID = new SelectList(liStaff, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.STAFF_CODE);
                    }
                    if (dtShift != null && dtShift.Rows.Count > 0)
                    {
                        liShit = (from DataRow dr in dtShift.Rows
                                  select new Sup_Shift()
                                  {
                                      SHIFT_ID = dr[Common.SUP_SHIFT.SHIFT_ID].ToString(),
                                      SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString()
                                  }).ToList();
                        objCourseWiseStaff.SHIFT = new SelectList(liShit, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    if (dtSemester != null && dtSemester.Rows.Count > 0)
                    {
                        liSemester = (from DataRow dr in dtSemester.Rows
                                      select new SupSemester()
                                      {
                                          SUP_SEMESTER_ID = dr[Common.SUP_SEMESTER.SUP_SEMESTER_ID].ToString(),
                                          SEMESTER_NAME = dr[Common.SUP_SEMESTER.SEMESTER_NAME].ToString()
                                      }).ToList();
                        objCourseWiseStaff.SEMESTER_ID = new SelectList(liSemester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EditCourseWiseStaff", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EditCourseWiseStaff", "AcademicsController", ex.Message);
                }
            }

            return View(objCourseWiseStaff);
        }
        public JsonResult FetchCourseWiseStaffById(string CourseWiseStaffId)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    dv.Clear();
                    dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.CLSCRSSTF_ID, CourseWiseStaffId, EnumCommand.DataType.String);
                    Session[Common.SESSION_VARIABLES.CLSCRSSTF_ID] = CourseWiseStaffId;
                    DataTable dtFetchClassWiseStaffById = new DataTable();
                    AcademicsViewModel objCourseWiseStaff = new AcademicsViewModel();
                    dtFetchClassWiseStaffById = objCourseWiseStaff.FetchCourseWiseStaffById(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    if (dtFetchClassWiseStaffById != null && dtFetchClassWiseStaffById.Rows.Count > 0)
                    {
                        CourseWiseStaffInfo objCourseWiseStaffInfo = new CourseWiseStaffInfo()
                        {
                            CLSCRSSTF_ID = dtFetchClassWiseStaffById.Rows[0][Common.CP_CLASS_COURSE_STAFF_2017.CLSCRSSTF_ID].ToString(),
                            CLASS_ID = dtFetchClassWiseStaffById.Rows[0][Common.CP_CLASS_COURSE_STAFF_2017.CLASS_ID].ToString(),
                            COURSE_ID = dtFetchClassWiseStaffById.Rows[0][Common.CP_CLASS_COURSE_STAFF_2017.COURSE_ID].ToString(),
                            STAFF_ID = dtFetchClassWiseStaffById.Rows[0][Common.CP_CLASS_COURSE_STAFF_2017.STAFF_ID].ToString(),
                            HRS_WEEK = dtFetchClassWiseStaffById.Rows[0][Common.CP_CLASS_COURSE_STAFF_2017.HRS_WEEK].ToString(),
                            HRS_TERM = dtFetchClassWiseStaffById.Rows[0][Common.CP_CLASS_COURSE_STAFF_2017.HRS_TERM].ToString(),
                            SHIFT = dtFetchClassWiseStaffById.Rows[0][Common.CP_CLASS_COURSE_STAFF_2017.SHIFT].ToString(),
                            SEMESTER_ID = dtFetchClassWiseStaffById.Rows[0][Common.CP_CLASS_COURSE_STAFF_2017.SEMESTER_ID].ToString(),
                            IS_PRIMARY_STAFF = dtFetchClassWiseStaffById.Rows[0][Common.CP_CLASS_COURSE_STAFF_2017.IS_PRIMARY_STAFF].ToString()
                        };
                        return Json(objCourseWiseStaffInfo);
                    }
                    else
                        return Json(sResult);
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
                    objHandler.WriteError("FetchCourseWiseStaffById", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchCourseWiseStaffById", "AcademicsController", ex.Message);
                    return Json(sResult);
                }
            }
        }
        public ActionResult FetchSelectedCourseWiseStaff(string sCourseId, string sClassId)
        {
            var ObjViewModel = new AcademicsViewModel();
            var liSelectedCourseWiseStaff = new List<CourseWiseStaffInfo>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    dv.Clear();
                    dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.COURSE_ID, sCourseId, EnumCommand.DataType.String);
                    dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.CLASS_ID, sClassId, EnumCommand.DataType.String);
                    DataTable dtFetchSelectedCourseStaff = new DataTable();
                    AcademicsViewModel objSelectedCourseWiseStaff = new AcademicsViewModel();
                    dtFetchSelectedCourseStaff = objSelectedCourseWiseStaff.FetchSelectedCourseWiseStaff(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    ObjViewModel.liCourseWiseStaff = (from DataRow dr in dtFetchSelectedCourseStaff.Rows
                                                      select new CourseWiseStaffInfo()
                                                      {
                                                          CLSCRSSTF_ID = dr[Common.CP_CLASS_COURSE_STAFF_2017.CLSCRSSTF_ID].ToString(),
                                                          CLASS_ID = dr[Common.CP_CLASS_COURSE_STAFF_2017.CLASS_ID].ToString(),
                                                          CLASS_NAME = dr[Common.CP_CLASSES_2017.CLASS_NAME].ToString(),
                                                          COURSE_ID = dr[Common.CP_CLASS_COURSE_STAFF_2017.COURSE_ID].ToString(),
                                                          COURSE_CODE = dr[Common.CP_COURSE_ROOT_2017.COURSE_CODE].ToString(),
                                                          COURSE_TITLE = dr[Common.CP_COURSE_ROOT_2017.COURSE_TITLE].ToString(),
                                                          STAFF_ID = dr[Common.CP_CLASS_COURSE_STAFF_2017.STAFF_NAME].ToString(),
                                                          //STAFF_COUNT = dr[Common.CP_CLASS_COURSE_STAFF_2017.STAFF_COUNT].ToString(),
                                                          SEMESTER_ID = dr[Common.CP_CLASS_COURSE_STAFF_2017.SEMESTER_ID].ToString(),
                                                          SHIFT = dr[Common.CP_CLASS_COURSE_STAFF_2017.SHIFT].ToString(),
                                                      }).ToList();

                }
                else
                    sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchSelectedCourseWiseStaff", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchSelectedCourseWiseStaff", "AcademicsController", ex.Message);
                }
            }
            return View(ObjViewModel);

        }
        public string EditSelectedCourseWiseStaff(string CourseWiseStaffId)
        {
            string sClass = string.Empty;
            string sCourse = string.Empty;
            string sStaff = string.Empty;
            string sShift = string.Empty;
            string sSemester = string.Empty;
            string JsonData = string.Empty;

            CourseWiseStaffModel objSelectedCourseWiseStaff = new CourseWiseStaffModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            DataTable dtClass = new DataTable();
            DataTable dtCourse = new DataTable();
            DataTable dtStaff = new DataTable();
            DataTable dtShift = new DataTable();
            DataTable dtSemester = new DataTable();

            dtClass = SupportDataMethod.FetchClass(sAcademicYear).DataSource.Table;
            dtCourse = SupportDataMethod.FetchCourses(sAcademicYear).DataSource.Table;
            dtStaff = SupportDataMethod.FetchStaff().DataSource.Table;
            dtShift = SupportDataMethod.FetchShift().DataSource.Table;
            dtSemester = SupportDataMethod.FetchSemester().DataSource.Table;

            List<cp_Classes_2017> liClass = new List<cp_Classes_2017>();
            List<cp_Course_Root_2017> liCourse = new List<cp_Course_Root_2017>();
            List<stf_Personal_Info> liStaff = new List<stf_Personal_Info>();
            List<Sup_Shift> liShit = new List<Sup_Shift>();
            List<SupSemester> liSemester = new List<SupSemester>();

            if (dtClass != null && dtClass.Rows.Count > 0)
            {
                foreach (DataRow item in dtClass.Rows)
                {
                    sClass += "<option value='" + item[Common.CP_CLASSES_2017.CLASS_ID].ToString() + "'>" + item[Common.CP_CLASSES_2017.CLASS_NAME].ToString() + "</option>";
                }
            }
            if (dtCourse != null && dtCourse.Rows.Count > 0)
            {
                foreach (DataRow item in dtCourse.Rows)
                {
                    sCourse += "<option value='" + item[Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID].ToString() + "'>" + item[Common.CP_COURSE_ROOT_2017.COURSE_CODE].ToString() + "</option>";
                }
            }
            if (dtStaff != null && dtStaff.Rows.Count > 0)
            {
                foreach (DataRow item in dtStaff.Rows)
                {
                    sStaff += "<option value='" + item[Common.STF_PERSONAL_INFO.STAFF_ID].ToString() + "'>" + item[Common.STF_PERSONAL_INFO.STAFF_NAME].ToString() + "</option>";
                }
            }
            if (dtShift != null && dtShift.Rows.Count > 0)
            {
                foreach (DataRow item in dtShift.Rows)
                {
                    sShift += "<option value='" + item[Common.SUP_SHIFT.SHIFT_ID].ToString() + "'>" + item[Common.SUP_SHIFT.SHIFT_NAME].ToString() + "</option>";
                }
            }
            if (dtSemester != null && dtSemester.Rows.Count > 0)
            {
                foreach (DataRow item in dtSemester.Rows)
                {
                    sSemester += "<option value='" + item[Common.SUP_SEMESTER.SUP_SEMESTER_ID].ToString() + "'>" + item[Common.SUP_SEMESTER.SEMESTER_NAME].ToString() + "</option>";
                }
            }
            var objJsonData = new CourseWiseStaffInfo() { CLASS_ID = sClass, COURSE_ID = sCourse, STAFF_ID = sStaff, SHIFT = sShift, SEMESTER_ID = sSemester };
            JsonData = JsonConvert.SerializeObject(objJsonData);
            return JsonData;
        }
        public ActionResult UpdateSelectedCourseWiseStaffInfo(string jsonSelectedCourseWiseStaff)
        {
            string sSelectedCourseWiseStaffId = Session[Common.SESSION_VARIABLES.CLSCRSSTF_ID].ToString();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (jsonSelectedCourseWiseStaff != null)
            {
                var Result = JsonConvert.DeserializeObject<CourseWiseStaffInfo>(jsonSelectedCourseWiseStaff);
                dv.Clear();
                dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.CLSCRSSTF_ID, sSelectedCourseWiseStaffId, EnumCommand.DataType.String);
                dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.CLASS_ID, Result.CLASS_ID, EnumCommand.DataType.String);
                dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.COURSE_ID, Result.COURSE_ID, EnumCommand.DataType.String);
                dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.STAFF_ID, Result.STAFF_ID, EnumCommand.DataType.String);
                dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.HRS_WEEK, Result.HRS_WEEK, EnumCommand.DataType.String);
                dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.HRS_TERM, Result.HRS_TERM, EnumCommand.DataType.String);
                dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.SHIFT, Result.SHIFT, EnumCommand.DataType.String);
                dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.SEMESTER_ID, Result.SEMESTER_ID, EnumCommand.DataType.String);
                dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.IS_PRIMARY_STAFF, Result.IS_PRIMARY_STAFF, EnumCommand.DataType.String);
                using (AcademicsViewModel objDepartment = new AcademicsViewModel())
                {
                    resultArg = objDepartment.UpdateSelectedCourseWiseStaffInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                    sResult = resultArg.Success.ToString();
                }
            }
            return RedirectToAction("FetchSelectedCourseWiseStaff", "Academics");
        }
        public ActionResult BindStaffsbyClassId(string sClassId, string sDepartment, string sShift, string sClassType)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            DataTable dtCourseStaff = new DataTable();
            DataTable dtEthicsPapers = new DataTable();
            DataTable dtSemester = new DataTable();
            DataTable dtStaffList = new DataTable();
            //dtStaffList = SupportDataMethod.FetchStaff().DataSource.Table;
            dtStaffList = SupportDataMethod.FetchTeachingStaffOnly().DataSource.Table;
            dtSemester = SupportDataMethod.FetchSemester().DataSource.Table;
            List<stf_Personal_Info> liStaff = new List<stf_Personal_Info>();
            List<SupSemester> liSemester = new List<SupSemester>();
            List<CourseWiseStaffInfo> liethics = new List<CourseWiseStaffInfo>();
            CourseWiseStaffInfo objModel = new CourseWiseStaffInfo();
            using (AcademicsViewModel objCourseWiseStaff = new AcademicsViewModel())
            {
                try
                {
                    dv.Clear();
                    dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.CLASS_ID, sClassId, EnumCommand.DataType.String);
                    dv.Add(Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, sDepartment, EnumCommand.DataType.String);
                    dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.SHIFT, sShift, EnumCommand.DataType.String);
                    dv.Add(Common.CP_CLASSES_2017.CLASS_TYPE, sClassType, EnumCommand.DataType.String);
                    objModel.CLASS_ID = sClassId;
                    objModel.DEPARTMENT_ID = sDepartment;
                    objModel.SHIFT = sShift;
                    objModel.CLASS_TYPE = sClassType;
                    dtCourseStaff = objCourseWiseStaff.FetchCourseWiseStaffInfoByClassId(dv, sAcademicYear, sActiveSemester).DataSource.Table;
                    liethics = (List<CourseWiseStaffInfo>)CMSHandler.FetchData<CourseWiseStaffInfo>(objModel, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchEthicsPaperByClassId), sAcademicYear).DataSource.Data;
                    if (dtSemester != null && dtSemester.Rows.Count > 0)
                    {
                        liSemester = (from DataRow dr in dtSemester.Rows
                                      select new SupSemester()
                                      {
                                          SUP_SEMESTER_ID = dr[Common.SUP_SEMESTER.SUP_SEMESTER_ID].ToString(),
                                          SEMESTER_NAME = dr[Common.SUP_SEMESTER.SEMESTER_NAME].ToString()
                                      }).ToList();
                        objCourseWiseStaff.Semester_List = liSemester;
                    }
                    if (dtStaffList != null && dtStaffList.Rows.Count > 0)
                    {
                        liStaff = (from DataRow dr in dtStaffList.Rows
                                   select new stf_Personal_Info()
                                   {
                                       STAFF_ID = dr[Common.STF_PERSONAL_INFO.STAFF_ID].ToString(),
                                       STAFF_CODE = dr[Common.STF_PERSONAL_INFO.STAFF_NAME].ToString(),
                                   }).ToList();
                        objCourseWiseStaff.Staff_List = liStaff;
                    }
                    //IList<CourseWiseStaffInfo> liList = new List<CourseWiseStaffInfo>();
                    if (dtCourseStaff != null && dtCourseStaff.Rows.Count > 0)
                    {
                        var CourseWiseStaff = (from DataRow dr in dtCourseStaff.Rows
                                               select new CourseWiseStaffInfo()
                                               {
                                                   //CLSCRSSTF_ID = dr[Common.CP_CLASS_COURSE_STAFF_2017.CLSCRSSTF_ID].ToString(),
                                                   CLASS_ID = dr[Common.CP_CLASSES_2017.CLASS_ID].ToString(),
                                                   //SHIFT = dr["SHIFT_NAME"].ToString(),
                                                   STAFF_ID = dr[Common.CP_CLASS_COURSE_STAFF_2017.STAFF_NAME].ToString(),
                                                   COURSE_ID = dr[Common.CP_CLASS_COURSE_STAFF_2017.COURSE_ID].ToString(),
                                                   COURSE_CODE = dr[Common.CP_COURSE_ROOT_2017.COURSE_CODE].ToString(),
                                                   COURSE_TITLE = dr[Common.CP_COURSE_ROOT_2017.COURSE_TITLE].ToString(),
                                                   SEMESTER = dr[Common.SUP_SEMESTER.SEMESTER_NAME].ToString(),
                                                   SEMESTER_ID = dr[Common.SUP_SEMESTER.SUP_SEMESTER_ID].ToString(),
                                                   HRS_WEEK = dr[Common.CP_CLASS_COURSE_STAFF_2017.HRS_WEEK].ToString(),
                                                   HRS_TERM = dr[Common.CP_CLASS_COURSE_STAFF_2017.HRS_TERM].ToString()
                                               }).ToList();
                        objCourseWiseStaff.objCourseWiseStaff = CourseWiseStaff;
                    }
                    // ****** commented by paul if required this method plz ask me ********
                    //if (liethics != null && liethics.Count > 0)
                    //{
                    //    if (liethics != null && liethics.Count > 0)
                    //    {
                    //        foreach (var item in liethics)
                    //        {
                    //            if (item.CLASS_ID == sClassId)
                    //            {
                    //                objCourseWiseStaff.objCourseWiseStaff.Add(item);
                    //            }
                    //        }
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Academics", "BindStaffsbyClassId", ex.Message);
                    }
                }

                return View(objCourseWiseStaff);
            }
        }

        public ActionResult ListClassCourseStaffForSJC()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AcademicsViewModel();
            var ObjModel = new STF_PERSONAL_INFO();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    ObjModel.STAFF_ID = sUserId;
                    var liStaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAcademicYear).DataSource.Data;
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        ObjModel.STAFF_ID = liStaff.FirstOrDefault().STAFF_ID;
                        ObjModel.DEPARTMENT = liStaff.FirstOrDefault().DEPARTMENT;

                        string sSQL = AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseWiseStaffInfoByDepartmentId).Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.SHIFT, liStaff.FirstOrDefault().SHIFT);
                        ObjViewModel.liCourseWiseStaff = (List<CourseWiseStaffInfo>)CMSHandler.FetchData<CourseWiseStaffInfo>(ObjModel, sSQL, sAcademicYear).DataSource.Data;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AcademicsController", "ListClassCourseStaffForSJC", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult ClassCourseStaffInfoForSJC()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AcademicsViewModel();
            var ObjModel = new STF_PERSONAL_INFO();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    List<SupSemester> liSemester = new List<SupSemester>();
                    List<SupClassType> liClassType = new List<SupClassType>();
                    List<Sup_Shift> liShiftList = new List<Sup_Shift>();
                    var dtSemester = SupportDataMethod.FetchSemester().DataSource.Table;
                    var dtClasstype = SupportDataMethod.FetchClassType().DataSource.Table;
                    ObjModel.STAFF_ID = sUserId;
                    var liStaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAcademicYear).DataSource.Data;
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        ObjModel.STAFF_ID = liStaff.FirstOrDefault().STAFF_ID;
                        ObjModel.DEPARTMENT = liStaff.FirstOrDefault().DEPARTMENT;
                        var sShift = liStaff.FirstOrDefault().SHIFT.Split(',');

                        var liShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift), sAcademicYear).DataSource.Data;
                        foreach (var item in sShift)
                        {
                            var Shift = liShift.Where(x => x.SHIFT_ID == item).ToList();
                            liShiftList = (liShiftList != null && liShiftList.Count > 0) ? liShiftList.Concat(Shift).ToList() : Shift;
                        }

                        if (liShiftList != null && liShiftList.Count > 0)
                        {
                            ObjViewModel.Shift = new SelectList(liShiftList, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                        }

                        var liDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartmentById), sAcademicYear).DataSource.Data;
                        if (liDepartment != null && liDepartment.Count > 0)
                        {
                            ObjViewModel.Department = new SelectList(liDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                        }

                        string sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByDepartmenIdShifId).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.SHIFT, liStaff.FirstOrDefault().SHIFT);
                        var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(ObjModel, sSQL, sAcademicYear).DataSource.Data;
                        if (FetchClass != null && FetchClass.Count > 0)
                        {
                            ObjViewModel.liClass = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                        }

                        if (dtClasstype != null && dtClasstype.Rows.Count > 0)
                        {
                            liClassType = (from DataRow dr in dtClasstype.Rows
                                           select new SupClassType()
                                           {
                                               CLASS_TYPE_ID = dr[Common.SUP_CLASS_TYPE.CLASS_TYPE_ID].ToString(),
                                               CLASS_TYPE = dr[Common.SUP_CLASS_TYPE.CLASS_TYPE].ToString()
                                           }).ToList();
                            ObjViewModel.liClassType = new SelectList(liClassType, Common.SUP_CLASS_TYPE.CLASS_TYPE_ID, Common.SUP_CLASS_TYPE.CLASS_TYPE);
                        }

                        if (dtSemester != null && dtSemester.Rows.Count > 0)
                        {
                            liSemester = (from DataRow dr in dtSemester.Rows
                                          select new SupSemester()
                                          {
                                              SUP_SEMESTER_ID = dr[Common.SUP_SEMESTER.SUP_SEMESTER_ID].ToString(),
                                              SEMESTER_NAME = dr[Common.SUP_SEMESTER.SEMESTER_NAME].ToString()
                                          }).ToList();
                            ObjViewModel.Semester = new SelectList(liSemester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                        }

                        var liSemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null,
                                SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType), sAcademicYear).DataSource.Data;
                        if (liSemesterType != null && liSemesterType.Count > 0)
                        {
                            ObjViewModel.liSemesterType = new SelectList(liSemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AcademicsController", "ClassCourseStaffInfoForSJC", ex.Message);
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

        // This Module Not In Use .
        #region Academic Curriculum
        [UserRights("ADMIN")]
        public ActionResult ListAcademicCurriculumInfo()
        {

            AcademicCurriculumViewModel objAcademicViewModel = new AcademicCurriculumViewModel();
            string sSelectedValue = string.Empty;
            DataTable dtAcademicYear = new DataTable();

            dtAcademicYear = SupportDataMethod.FetchAcademicYearList().DataSource.Table;
            var liSupAcademicYear = new List<SUP_ACADEMIC_YEAR_LIST>();
            liSupAcademicYear = (from DataRow dr in dtAcademicYear.Rows
                                 select new SUP_ACADEMIC_YEAR_LIST()
                                 {
                                     ACADEMIC_YEAR_ID = dr[Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID].ToString(),
                                     ACTIVE_YEAR = dr[Common.ACADEMIC_YEAR.ACTIVE_YEAR].ToString(),
                                     AC_YEAR = dr[Common.ACADEMIC_YEAR.AC_YEAR].ToString()
                                 }).ToList();

            DataRow[] drArray = dtAcademicYear.Select(Common.ACADEMIC_YEAR.ACTIVE_YEAR + "=1");
            sSelectedValue = drArray[0][Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID].ToString();
            objAcademicViewModel.ddlAcademicYearList = new SelectList(liSupAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR, sSelectedValue);

            return View(objAcademicViewModel);
        }

        public ActionResult listAcademicCurriculum(string sAademicYear)
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                AcademicsViewModel objAcademicCurriculumview = new AcademicsViewModel();
                DataTable dtAcademicCurriculum = new DataTable();
                dv.Clear();
                dv.Add(Common.ACADEMIC_YEAR.AC_YEAR, (string.IsNullOrEmpty(sAademicYear) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : sAademicYear), EnumCommand.DataType.String);
                objAcademicCurriculumview.dv = dv;
                dtAcademicCurriculum = objAcademicCurriculumview.FetchAcademicCurriculumInfo((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                IList<AcademicCurriculumInfo> liAcademicCurriculum = new List<AcademicCurriculumInfo>();
                if (dtAcademicCurriculum != null && dtAcademicCurriculum.Rows.Count > 0)
                {
                    liAcademicCurriculum = (from DataRow dr in dtAcademicCurriculum.Rows
                                            select new AcademicCurriculumInfo()
                                            {
                                                CURRICULUM_ID = dr[Common.CP_ACADEMIC_CURRICULUM_2017.CURRICULUM_ID].ToString(),
                                                PROGRAMME = dr["PROGRAMME_CODE"].ToString(),
                                                BATCH = dr[Common.CP_ACADEMIC_CURRICULUM_2017.BATCH].ToString(),
                                                COURSE_ID = dr[Common.CP_COURSE_ROOT_2017.COURSE_CODE].ToString(),
                                                //S_NO=dr[Common.CP_ACADEMIC_CURRICULUM_2017.S_NO].ToString(),
                                                SEMESTER = dr["SEMESTER_NAME"].ToString()
                                            }).ToList();

                }
                return PartialView(liAcademicCurriculum);
            }
            return RedirectToAction("Index", "Account");
        }
        /// <summary>
        /// this action for add page
        /// </summary>
        /// <returns></returns>
        public ActionResult AcademicCurriculum()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            AcademicCurriculumModel objAcademicCurriculum = new AcademicCurriculumModel();
            AcademicsViewModel objAcademicViewModel = new AcademicsViewModel();
            DataTable dtBatch = new DataTable();
            DataTable dtProgramme = new DataTable();
            DataTable dtCourseList = new DataTable();
            DataTable dtSemester = new DataTable();

            dtBatch = SupportDataMethod.FetchActiveBatch().DataSource.Table;
            dtProgramme = SupportDataMethod.FetchProgram((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
            // dtCourseList = objAcademicViewModel.FetchCourseBySemester()
            dtSemester = SupportDataMethod.FetchSemester().DataSource.Table;

            List<sup_Batch> liBatch = new List<sup_Batch>();
            List<cp_Program_2017> liProgramme = new List<cp_Program_2017>();
            List<cp_Course_Root_2017> liCourse = new List<cp_Course_Root_2017>();
            List<SupSemester> liSemester = new List<SupSemester>();

            if (dtBatch != null && dtBatch.Rows.Count > 0)
            {
                liBatch = (from DataRow dr in dtBatch.Rows
                           select new sup_Batch()
                           {
                               BATCH_ID = dr[Common.SUP_BATCHES.BATCH_ID].ToString(),
                               BATCH = dr[Common.SUP_BATCHES.BATCH].ToString()
                           }).ToList();
                objAcademicCurriculum.BATCH = new SelectList(liBatch, Common.SUP_BATCHES.BATCH_ID, Common.SUP_BATCHES.BATCH);
            }
            if (dtProgramme != null && dtProgramme.Rows.Count > 0)
            {
                liProgramme = (from DataRow dr in dtProgramme.Rows
                               select new cp_Program_2017()
                               {
                                   PROGRAMME_ID = dr[Common.CP_PROGRAMME_2017.PROGRAMME_ID].ToString(),
                                   PROGRAMME_NAME = dr[Common.CP_PROGRAMME_2017.PROGRAMME_NAME].ToString()
                               }).ToList();
                objAcademicCurriculum.PROGRAMME = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
            }

            liCourse.Add(new cp_Course_Root_2017() { COURSE_ROOT_ID = "0", COURSE_CODE = "---Select---" });
            objAcademicCurriculum.COURSE_LIST = new SelectList(liCourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
            if (dtSemester != null && dtSemester.Rows.Count > 0)
            {
                liSemester = (from DataRow dr in dtSemester.Rows
                              select new SupSemester()
                              {
                                  SUP_SEMESTER_ID = dr[Common.SUP_SEMESTER.SUP_SEMESTER_ID].ToString(),
                                  SEMESTER_NAME = dr[Common.SUP_SEMESTER.SEMESTER_NAME].ToString()
                              }).ToList();
                objAcademicCurriculum.SEMESTER = new SelectList(liSemester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
            }
            return View(objAcademicCurriculum);
        }

        public string SaveAcademicCurriculum(string sCurriculumCourse)
        {
            string sResult = string.Empty;
            JSonCurriculumCourse objJSonCurriculumCourse = JsonConvert.DeserializeObject<JSonCurriculumCourse>(sCurriculumCourse);
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                AcademicsViewModel objAcademicsViewModel = new AcademicsViewModel();
                resultArg = objAcademicsViewModel.BulkSaveCurriculum(objJSonCurriculumCourse, Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString());
                if (resultArg.Success)
                    sResult = Common.Messages.RecordsSavedSuccessfully;
                else
                    sResult = Common.Messages.FailedToSaveRecords;
            }
            else
            {
                sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return sResult;
        }
        public ActionResult BindCoursesById(string sBatchId, string sProgrammeIds)
        {
            DataTable dtCurriculumCourse = new DataTable();
            using (AcademicsViewModel objCurriculum = new AcademicsViewModel())
            {
                dv.Clear();
                dv.Add(Common.CP_ACADEMIC_CURRICULUM_2017.BATCH, sBatchId, EnumCommand.DataType.String);
                dv.Add(Common.CP_ACADEMIC_CURRICULUM_2017.PROGRAMME, sProgrammeIds, EnumCommand.DataType.String);
                dtCurriculumCourse = objCurriculum.FetchCurriculumInfoById(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                if (dtCurriculumCourse != null && dtCurriculumCourse.Rows.Count > 0)
                {
                    objCurriculum.objCourseCurriculum = (from DataRow dr in dtCurriculumCourse.Rows
                                                         select new AcademicCurriculumInfo()
                                                         {
                                                             CURRICULUM_ID = dr[Common.CP_ACADEMIC_CURRICULUM_2017.CURRICULUM_ID].ToString(),
                                                             BATCH = dr[Common.CP_ACADEMIC_CURRICULUM_2017.BATCH].ToString(),
                                                             PROGRAMME = dr["PROGRAMME_CODE"].ToString(),
                                                             COURSE_ID = dr["COURSE_CODE"].ToString(),
                                                             SEMESTER = dr["SEMESTER_NAME"].ToString()
                                                         }).ToList();
                }
                return View(objCurriculum);
            }

        }
        public ActionResult DeleteCurriculumInfo(string id)
        {
            string sResult = string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                using (AcademicsViewModel objCurriculum = new AcademicsViewModel())
                {
                    dv.Clear();
                    dv.Add(Common.CP_ACADEMIC_CURRICULUM_2017.CURRICULUM_ID, id, EnumCommand.DataType.String);
                    resultArg = objCurriculum.DeleteCurriculumInfo(dv, Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString());
                    if (resultArg.Success)
                    {
                        TempData["DeleteSuccess"] = Common.Messages.RecordsSavedSuccessfully;
                    }
                    else
                    {
                        TempData["DeleteSuccess"] = Common.Messages.RecordsSavedSuccessfully;

                    }
                }
            }
            else
            {
                TempData["DeleteSuccess"] = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return RedirectToAction("listacademiccurriculuminfo", "academics");
        }

        public string FetchCourseListBySemester(string sBatchId, string sProgrammeId)
        {
            string sResult = string.Empty;
            DataTable dtCourseList = new DataTable();
            dv.Clear();
            dv.Add(Common.CP_ACADEMIC_CURRICULUM_2017.BATCH, sBatchId, EnumCommand.DataType.String);
            dv.Add(Common.CP_ACADEMIC_CURRICULUM_2017.PROGRAMME, sProgrammeId, EnumCommand.DataType.String);

            using (AcademicsViewModel objViewModel = new AcademicsViewModel())
            {
                dtCourseList = objViewModel.FetchCourseBySemester(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                if (dtCourseList != null && dtCourseList.Rows.Count > 0)
                {
                    foreach (DataRow item in dtCourseList.Rows)
                    {
                        sResult += "<option value='" + item[Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID] + "' " + item["SELECTED"] + " >" + item[Common.CP_COURSE_ROOT_2017.COURSE_TITLE] + "</option>";
                    }

                }
            }

            return sResult;

        }
        public ActionResult SaveOrUpdateCurriculum(string str)
        {
            if (str != null)
            {
                string sResult = string.Empty;
                string sInsertQuery = string.Empty;
                string sUpdateQuery = string.Empty;
                string sInsertQuerycom = string.Empty;
                var Result = JsonConvert.DeserializeObject<AcademicCurriculumInfo>(str);
                sInsertQuery = @"INSERT INTO ACADEMIC_CURRICULUM_2017(PROGRAMME,BATCH,COURSE_ID,IS_ACTIVE,SEMESTER)VALUES";
                sInsertQuerycom = sInsertQuery;

            }
            return RedirectToAction("ListAcademicCurriculumInfo", "Academics");
        }
        #endregion

        #region Semester Root
        [UserRights("ADMIN")]
        public ActionResult ListSemesterRoot()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                AcademicsViewModel objAcademicyear = new AcademicsViewModel();
                DataTable dtSemesterRoot = new DataTable();
                dtSemesterRoot = objAcademicyear.FetchSemesterRootInfo(sAcademicYear).DataSource.Table;
                //  IList<SemesterRootInfo> liList = new List<SemesterRootInfo>();
                if (dtSemesterRoot != null && dtSemesterRoot.Rows.Count > 0)
                {
                    objAcademicyear.liSemesterRoot = (from DataRow dr in dtSemesterRoot.Rows
                                                      select new SemesterRootInfo
                                                      {
                                                          SEMESTER_ID = dr[Common.CP_SEMESTER_ROOT_2017.ACC_SEMESTER_ID].ToString(),
                                                          BATCH = dr[Common.CP_SEMESTER_ROOT_2017.BATCH].ToString(),
                                                          PROGRAMME = dr[Common.CP_PROGRAMME_2017.PROGRAMME_NAME].ToString(),
                                                          DATE_FROM = dr[Common.CP_SEMESTER_ROOT_2017.DATE_FROM].ToString(),
                                                          DATE_TO = dr[Common.CP_SEMESTER_ROOT_2017.DATE_TO].ToString(),
                                                          IS_ACTIVE = dr[Common.CP_SEMESTER_ROOT_2017.IS_ACTIVE].ToString(),
                                                          SEMESTER = dr[Common.SUP_SEMESTER.SEMESTER_NAME].ToString(),
                                                      }).ToList();
                }

                // Programme Level ....
                var liProgrammeLevel = (List<SUP_PROGRAMME_LEVEL>)CMSHandler.FetchData<SUP_PROGRAMME_LEVEL>(null,
                    SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel)).DataSource.Data;
                if (liProgrammeLevel != null && liProgrammeLevel.Count > 0)
                {
                    objAcademicyear.ProgrammeLevel = new SelectList(liProgrammeLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
                }

                // Academic Year ....
                var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
                    SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                if (liAcYear != null && liAcYear.Count > 0)
                {
                    objAcademicyear.AcademicYear = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }

                // Semeser ...
                var liSemester = (List<SUP_SEMESTER>)CMSHandler.FetchData<SUP_SEMESTER>(null,
                    SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data;
                if (liSemester != null && liSemester.Count > 0)
                {
                    objAcademicyear.Semester = new SelectList(liSemester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                }

                // Batch ...
                var liBatch = (List<sup_Batch>)CMSHandler.FetchData<sup_Batch>(null,
                    SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchBatch)).DataSource.Data;
                if (liBatch != null && liBatch.Count > 0)
                {
                    objAcademicyear.Batch = new SelectList(liBatch, Common.SUP_BATCHES.BATCH_ID, Common.SUP_BATCHES.BATCH);
                }

                return View(objAcademicyear);
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
        }
        public JsonResult FetchProgrammeByProgrammeLevel(string sProgrammeLevelId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(new CP_PROGRAMME_2017 { PROGRAMME_LEVEL = sProgrammeLevelId },
                    AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchProgrammeByProgrammeLevel), sAcademicYear).DataSource.Data;
                if (liProgramme != null && liProgramme.Count > 0)
                {
                    foreach (var item in liProgramme)
                    {
                        ObjJsonData.sResult += "<option value=" + item.PROGRAMME_GROUP_ID + " SELECTED>" + item.PROGRAMME_NAME + "</option>";
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        //public string UpdateActiveSemesterRoot(string sActiveSemesterId)
        //{
        //    if (sActiveSemesterId != null)
        //    {
        //        DataTable dt = new DataTable();
        //        dv.Clear();
        //        dv.Add(Common.CP_SEMESTER_ROOT_2017.SEMESTER_ID, sActiveSemesterId, EnumCommand.DataType.String);
        //        using (AcademicsViewModel objAcademicYearView = new AcademicsViewModel())
        //        {
        //            resultArg = objAcademicYearView.UpdateActiveSemester(dv);
        //            dt = objAcademicYearView.FetchSemesterRootInfo().DataSource.Table;
        //            sResult = resultArg.Success.ToString();
        //        }
        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            SemesterRootInfo objAcademicYear = new SemesterRootInfo()
        //            {
        //                BATCH = dt.Rows[0]["BATCH"].ToString(),
        //                PROGRAMME = dt.Rows[0]["PROGRAMME_NAME"].ToString(),
        //                DATE_FROM = dt.Rows[0][Common.CP_SEMESTER_ROOT_2017.DATE_FROM].ToString(),
        //                DATE_TO = dt.Rows[0][Common.CP_SEMESTER_ROOT_2017.DATE_TO].ToString(),
        //                IS_ACTIVE = dt.Rows[0][Common.CP_SEMESTER_ROOT_2017.IS_ACTIVE].ToString(),
        //                SEMESTER = dt.Rows[0]["SEMESTER_NAME"].ToString()
        //            };
        //        }
        //    }

        //    return sResult;
        //}
        public ActionResult SemesterRootInfo()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            SemesterRootModel objSemesterRoot = new SemesterRootModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    DataTable dtBatch = new DataTable();
                    DataTable dtProgramme = new DataTable();
                    DataTable dtSemester = new DataTable();

                    dtBatch = SupportDataMethod.FetchBatch().DataSource.Table;
                    //  dtProgramme = SupportDataMethod.FetchProgram(sAcademicYear).DataSource.Table;
                    dtSemester = SupportDataMethod.FetchSemester().DataSource.Table;

                    IList<sup_Batch> liBatch = new List<sup_Batch>();
                    //  IList<cp_Program_2017> liProgramme = new List<cp_Program_2017>();
                    IList<SupSemester> liSemester = new List<SupSemester>();

                    if (dtBatch != null && dtBatch.Rows.Count > 0)
                    {
                        liBatch = (from DataRow dr in dtBatch.Rows
                                   select new sup_Batch()
                                   {
                                       BATCH_ID = dr[Common.SUP_BATCHES.BATCH_ID].ToString(),
                                       BATCH = dr[Common.SUP_BATCHES.BATCH].ToString()
                                   }).ToList();
                        objSemesterRoot.BATCH = new SelectList(liBatch, Common.SUP_BATCHES.BATCH_ID, Common.SUP_BATCHES.BATCH);
                    }

                    // Fetch Programme Group ...
                    var liProgrammeList = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                    if (liProgrammeList != null && liProgrammeList.Count > 0)
                    {
                        objSemesterRoot.PROGRAMME = new SelectList(liProgrammeList, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    }

                    if (dtSemester != null && dtSemester.Rows.Count > 0)
                    {
                        liSemester = (from DataRow dr in dtSemester.Rows
                                      select new SupSemester()
                                      {
                                          SUP_SEMESTER_ID = dr[Common.SUP_SEMESTER.SUP_SEMESTER_ID].ToString(),
                                          SEMESTER_NAME = dr[Common.SUP_SEMESTER.SEMESTER_NAME].ToString()
                                      }).ToList();
                        objSemesterRoot.SEMESTER = new SelectList(liSemester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("AcademicController", "SemesterRootInfo", ex.Message);
                    }
                }
            }
            else
            {

            }

            return View(objSemesterRoot);
        }
        public string GetSemesterId(string sProgramme, string sBatch)
        {
            string sOption = string.Empty;
            DataTable dtSemesterList = new DataTable();
            DataTable dtTempSemester = new DataTable();
            DataTable dtSemester = new DataTable();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            using (AcademicsViewModel objAcademicsview = new AcademicsViewModel())
            {
                dv.Clear();
                dv.Add(Common.CP_SEMESTER_ROOT_2017.PROGRAMME, sProgramme, EnumCommand.DataType.String);
                dv.Add(Common.CP_SEMESTER_ROOT_2017.BATCH, sBatch, EnumCommand.DataType.String);
                dtSemesterList = objAcademicsview.GetSemesterId(dv, sAcademicYear).DataSource.Table;
                dtSemester = SupportDataMethod.FetchSemester().DataSource.Table;
                dtTempSemester = SupportDataMethod.FetchSemester().DataSource.Table;
                if (dtSemesterList != null && dtSemesterList.Rows.Count > 0)
                {
                    foreach (DataRow item in dtSemesterList.Rows)
                    {
                        sOption += "<option value='" + item[Common.SUP_SEMESTER.SUP_SEMESTER_ID] + "' " + item["SELECTED"] + " >" + item[Common.SUP_SEMESTER.SEMESTER_NAME] + "</option>";
                        foreach (DataRow row in dtSemester.Rows)
                            if (item[Common.SUP_SEMESTER.SUP_SEMESTER_ID].ToString() == row[Common.SUP_SEMESTER.SUP_SEMESTER_ID].ToString())
                            {
                                row.Delete();
                            }
                        dtSemester.AcceptChanges();
                    }
                }
                if (dtSemester != null && dtSemester.Rows.Count > 0)
                {
                    foreach (DataRow item in dtSemester.Rows)
                    {
                        sOption += "<option value='" + item[Common.SUP_SEMESTER.SUP_SEMESTER_ID] + "' >" + item[Common.SUP_SEMESTER.SEMESTER_NAME] + "</option>";
                    }
                }

            }
            return sOption;
        }
        public string InsertSemesterRootInfo(string sSemester)
        {
            string Result = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //var sResult = JsonConvert.DeserializeObject<SemesterRootInfo>(sSemester);
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            JsonSemesterRoot objJson = JsonConvert.DeserializeObject<JsonSemesterRoot>(sSemester);
            AcademicsViewModel objSemesterRoot = new AcademicsViewModel();
            if (objJson.SemesterRoot != null && objJson.SemesterRoot.Count > 0)
            {
                resultArg = objSemesterRoot.BulkSaveSemesterRoot(objJson, sAcademicYear);

                sResult = (resultArg.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
            }

            return sResult;
        }
        public JsonResult DeleteSemesterRootInfo(string id)
        {
            string sResult = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                using (AcademicsViewModel objSemesterRoot = new AcademicsViewModel())
                {
                    dv.Clear();
                    dv.Add(Common.CP_SEMESTER_ROOT_2017.ACC_SEMESTER_ID, id, EnumCommand.DataType.String);
                    resultArg = objSemesterRoot.DeleteSemesterRootInfo(dv, sAcademicYear);
                    ObjJsonData.Message = (resultArg.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.sResult = resultArg.Success.ToString();
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.sResult = "False";
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateActiveSemester(string sJsonData)
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                var Result = JsonConvert.DeserializeObject<ACADEMIC_SEMESTER>(sJsonData);
                if (!string.IsNullOrEmpty(Result.PROGRAMME_GROUP_ID))
                {
                    string sSQL = AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.UpdateAcademicSemesterInMultiple).
                        Replace(Common.Delimiter.QUS + Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Result.PROGRAMME_GROUP_ID);
                    var UpdateActiveSemester = CMSHandler.SaveOrUpdate(Result, sSQL, Result.ACADEMIC_YEAR);
                    ObjJsonData.Message = (UpdateActiveSemester.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                }
                else
                {
                    ObjJsonData.Message = "Sorry Your Record Con't Be Update !";
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Department
        public ActionResult DepartmentInfo()
        {
            DepartmentModel objDepartmentModel = new DepartmentModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    DataTable dtFaculty = new DataTable();
                    dtFaculty = SupportDataMethod.FetchFaculty((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    var liFaculty = new List<SupFaculty>();
                    if (dtFaculty != null && dtFaculty.Rows.Count > 0)
                    {
                        liFaculty = (from DataRow dr in dtFaculty.Rows
                                     select new SupFaculty()
                                     {
                                         FACULTY_ID = dr[Common.CP_FACULTY_2017.FACULTY_ID].ToString(),
                                         FACULTY = dr[Common.CP_FACULTY_2017.FACULTY].ToString()
                                     }).ToList();
                        objDepartmentModel.FACULTY = new SelectList(liFaculty, Common.CP_FACULTY_2017.FACULTY_ID, Common.CP_FACULTY_2017.FACULTY);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DepartmentInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DepartmentInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objDepartmentModel);
        }
        [UserRights("ADMIN")]
        public ActionResult ListDepartmentInfo()
        {
            AcademicsViewModel objDepartment = new AcademicsViewModel();
            var liDepartment = new List<DepartmentInfo>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    DataTable dtDepartmentList = new DataTable();
                    dtDepartmentList = objDepartment.FetchDepartmentInfo((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    if (dtDepartmentList != null && dtDepartmentList.Rows.Count > 0)
                    {
                        objDepartment.liDepartmentList = (from DataRow dr in dtDepartmentList.Rows
                                                          select new DepartmentInfo()
                                                          {
                                                              DEPARTMENT_ID = dr[Common.CP_DEPARTMENT_2017.DEPARTMENT_ID].ToString(),
                                                              DEPARTMENT_CODE = dr[Common.CP_DEPARTMENT_2017.DEPARTMENT_CODE].ToString(),
                                                              DEPARTMENT = dr[Common.CP_DEPARTMENT_2017.DEPARTMENT].ToString(),
                                                              DEPARTMENT_ORDER = dr[Common.CP_DEPARTMENT_2017.DEPARTMENT_ORDER].ToString(),
                                                              FACULTY = dr[Common.CP_FACULTY_2017.FACULTY].ToString(),
                                                              YEAR_OF_PUBLISHMENT = dr[Common.CP_DEPARTMENT_2017.YEAR_OF_PUBLISHMENT].ToString(),
                                                              IS_ACTIVE = dr[Common.CP_DEPARTMENT_2017.IS_ACTIVE].ToString(),
                                                          }).ToList();
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListDepartmentInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListDepartmentInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objDepartment);
        }
        public JsonResult InsertDepartmentInfo(string sJsonDepartment)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (sJsonDepartment != null)
                    {
                        DataTable dtDepartment = new DataTable();
                        var Result = JsonConvert.DeserializeObject<DepartmentInfo>(sJsonDepartment);

                        dv.Clear();
                        dv.Add(Common.CP_DEPARTMENT_2017.DEPARTMENT_CODE, Result.DEPARTMENT_CODE, EnumCommand.DataType.String);
                        using (AcademicsViewModel objDepartment = new AcademicsViewModel())
                        {
                            dtDepartment = objDepartment.CheckDepartment(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                            if (dtDepartment != null && dtDepartment.Rows.Count == 0)
                            {
                                dv.Clear();
                                dv.Add(Common.CP_DEPARTMENT_2017.DEPARTMENT, Result.DEPARTMENT, EnumCommand.DataType.String);
                                dv.Add(Common.CP_DEPARTMENT_2017.DEPARTMENT_CODE, Result.DEPARTMENT_CODE, EnumCommand.DataType.String);
                                dv.Add(Common.CP_DEPARTMENT_2017.DEPARTMENT_ORDER, Result.DEPARTMENT_ORDER, EnumCommand.DataType.String);
                                dv.Add(Common.CP_DEPARTMENT_2017.FACULTY, Result.FACULTY, EnumCommand.DataType.String);
                                dv.Add(Common.CP_DEPARTMENT_2017.YEAR_OF_PUBLISHMENT, Result.YEAR_OF_PUBLISHMENT, EnumCommand.DataType.String);
                                dv.Add(Common.CP_DEPARTMENT_2017.IS_ACTIVE, Result.IS_ACTIVE, EnumCommand.DataType.String);
                                resultArg = objDepartment.InsertDepartmentInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                                sResult = resultArg.Success.ToString();
                                if (sResult == "True")
                                    sResult = "Records Saved Successfully";
                                else
                                    sResult = "Enter Correct Values";
                            }
                            else
                                sResult = "Department Already Exists...!";
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
                    objHandler.WriteError("InsertDepartmentInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("InsertDepartmentInfo", "AcademicsController", ex.Message);
                }
            }
            return Json(sResult);
        }
        public ActionResult EditDepartmentInfo(int id)
        {
            DepartmentModel objDepartmentModel = new DepartmentModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sDepartmentId = Convert.ToString(id);
                    Session[Common.SESSION_VARIABLES.DEPARTMENT_ID] = sDepartmentId;
                    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                    {
                        string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                        DataTable dtFaculty = new DataTable();
                        dtFaculty = SupportDataMethod.FetchFaculty((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                        var liFaculty = new List<SupFaculty>();
                        if (dtFaculty != null && dtFaculty.Rows.Count > 0)
                        {
                            liFaculty = (from DataRow dr in dtFaculty.Rows
                                         select new SupFaculty()
                                         {
                                             FACULTY_ID = dr[Common.CP_FACULTY_2017.FACULTY_ID].ToString(),
                                             FACULTY = dr[Common.CP_FACULTY_2017.FACULTY].ToString()
                                         }).ToList();
                            objDepartmentModel.FACULTY = new SelectList(liFaculty, Common.CP_FACULTY_2017.FACULTY_ID, Common.CP_FACULTY_2017.FACULTY);
                        }
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EditDepartmentInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EditDepartmentInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objDepartmentModel);
        }
        public JsonResult UpdateDepartmentInfo(string JsonDepartment)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var sDepartmentId = Session[Common.SESSION_VARIABLES.DEPARTMENT_ID].ToString();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (JsonDepartment != null)
                    {
                        var Result = JsonConvert.DeserializeObject<DepartmentInfo>(JsonDepartment);
                        dv.Clear();
                        dv.Add(Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, sDepartmentId, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEPARTMENT_2017.DEPARTMENT, Result.DEPARTMENT, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEPARTMENT_2017.DEPARTMENT_CODE, Result.DEPARTMENT_CODE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEPARTMENT_2017.DEPARTMENT_ORDER, Result.DEPARTMENT_ORDER, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEPARTMENT_2017.FACULTY, Result.FACULTY, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEPARTMENT_2017.YEAR_OF_PUBLISHMENT, Result.YEAR_OF_PUBLISHMENT, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEPARTMENT_2017.IS_ACTIVE, Result.IS_ACTIVE, EnumCommand.DataType.String);
                        using (AcademicsViewModel objDepartment = new AcademicsViewModel())
                        {
                            resultArg = objDepartment.UpdateDepartmentInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                            sResult = resultArg.Success.ToString();
                        }
                        ObjJsonData.Message = (resultArg.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        ObjJsonData.sResult = sResult;
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    ObjJsonData.sResult = "False";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateDepartmentInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateDepartmentInfo", "AcademicsController", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.sResult = "False";
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchDepartmentInfoById(string id)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sDepartmentId = Convert.ToString(id);
                    Session[Common.SESSION_VARIABLES.DEPARTMENT_ID] = sDepartmentId;
                    dv.Clear();
                    dv.Add(Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, sDepartmentId, EnumCommand.DataType.String);
                    //AcademicsViewModel objDepartment = new AcademicsViewModel();
                    DataTable dtFetchDepartmentInfoById = new DataTable();
                    using (AcademicsViewModel objDepartment = new AcademicsViewModel())
                    {
                        dtFetchDepartmentInfoById = objDepartment.FetchDepartmentInfoById(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    }
                    if (dtFetchDepartmentInfoById != null && dtFetchDepartmentInfoById.Rows.Count > 0)
                    {
                        DepartmentInfo objDepartment = new DepartmentInfo()
                        {
                            DEPARTMENT = dtFetchDepartmentInfoById.Rows[0][Common.CP_DEPARTMENT_2017.DEPARTMENT].ToString(),
                            DEPARTMENT_CODE = dtFetchDepartmentInfoById.Rows[0][Common.CP_DEPARTMENT_2017.DEPARTMENT_CODE].ToString(),
                            DEPARTMENT_ORDER = dtFetchDepartmentInfoById.Rows[0][Common.CP_DEPARTMENT_2017.DEPARTMENT_ORDER].ToString(),
                            FACULTY = dtFetchDepartmentInfoById.Rows[0][Common.CP_DEPARTMENT_2017.FACULTY].ToString(),
                            YEAR_OF_PUBLISHMENT = dtFetchDepartmentInfoById.Rows[0][Common.CP_DEPARTMENT_2017.YEAR_OF_PUBLISHMENT].ToString(),
                            IS_ACTIVE = dtFetchDepartmentInfoById.Rows[0][Common.CP_DEPARTMENT_2017.IS_ACTIVE].ToString(),

                        };
                        return Json(objDepartment);
                    }
                    else
                        return Json(sResult);
                }
                else
                    return Json(sResult);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchDepartmentInfoById", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchDepartmentInfoById", "AcademicsController", ex.Message);
                    return Json(sResult = ex.Message);
                }
            }
        }
        public JsonResult DeleteDepartmentInfo(string id)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    using (AcademicsViewModel objDepartment = new AcademicsViewModel())
                    {
                        dv.Clear();
                        dv.Add(Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, id, EnumCommand.DataType.String);
                        resultArg = objDepartment.DeleteDepartmentInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                        sResult = resultArg.Success.ToString();
                    }
                    ObjJsonData.Message = (resultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.sResult = sResult;
                }
                else
                {
                    ObjJsonData.sResult = "False";
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DeleteDepartmentInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DeleteDepartmentInfo", "AcademicsController", ex.Message);
                }
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.sResult = "False";
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Faculty
        public ActionResult FacultyInfo()
        {
            return View();
        }
        [UserRights("ADMIN")]
        public ActionResult ListFacultyInfo()
        {
            AcademicsViewModel objFaculty = new AcademicsViewModel();
            var liFaculty = new List<FacultyInfo>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    DataTable dtFaculty = new DataTable();
                    dtFaculty = objFaculty.FetchFAcutlyInfo((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    if (dtFaculty != null && dtFaculty.Rows.Count > 0)
                    {
                        objFaculty.liFaculty = (from DataRow dr in dtFaculty.Rows
                                                select new FacultyInfo()
                                                {
                                                    FACULTY_ID = dr[Common.CP_FACULTY_2017.FACULTY_ID].ToString(),
                                                    FACULTY = dr[Common.CP_FACULTY_2017.FACULTY].ToString(),
                                                    FACULTY_CODE = dr[Common.CP_FACULTY_2017.FACULTY_CODE].ToString(),
                                                    FACULTY_ORDER = dr[Common.CP_FACULTY_2017.FACULTY_ORDER].ToString(),
                                                    FAC_SERIES = dr[Common.CP_FACULTY_2017.FAC_SERIES].ToString(),
                                                    IS_ACTIVE = dr[Common.CP_FACULTY_2017.IS_ACTIVE].ToString(),
                                                }).ToList();
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListFacultyInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListFacultyInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objFaculty);
        }
        public JsonResult InsertFacultyInfo(string sJsonFaculty)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (sJsonFaculty != null)
                    {
                        DataTable dtFactulty = new DataTable();
                        var Result = JsonConvert.DeserializeObject<FacultyInfo>(sJsonFaculty);
                        dv.Clear();
                        dv.Add(Common.CP_FACULTY_2017.FACULTY_CODE, Result.FACULTY_CODE, EnumCommand.DataType.String);
                        using (AcademicsViewModel objFaculty = new AcademicsViewModel())
                        {
                            dtFactulty = objFaculty.CheckFaculty(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                            if (dtFactulty != null && dtFactulty.Rows.Count == 0)
                            {
                                dv.Clear();
                                dv.Add(Common.CP_FACULTY_2017.FACULTY, Result.FACULTY, EnumCommand.DataType.String);
                                dv.Add(Common.CP_FACULTY_2017.FACULTY_CODE, Result.FACULTY_CODE, EnumCommand.DataType.String);
                                dv.Add(Common.CP_FACULTY_2017.FACULTY_ORDER, Result.FACULTY_ORDER, EnumCommand.DataType.String);
                                dv.Add(Common.CP_FACULTY_2017.FAC_SERIES, Result.FAC_SERIES, EnumCommand.DataType.String);
                                resultArg = objFaculty.InsertFacultyInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                                sResult = resultArg.Success.ToString();
                                sResult = (resultArg.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            }
                            else
                            {
                                sResult = "Faculty Already Exists...!";
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
                    objHandler.WriteError("InsertFacultyInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("InsertFacultyInfo", "AcademicsController", ex.Message);
                }
            }
            return Json(sResult);
        }
        public ActionResult EditFacultyInfo(int id)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sFacultyId = Convert.ToString(id);
                    Session[Common.SESSION_VARIABLES.FACULTY_ID] = sFacultyId;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EditFacultyInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EditFacultyInfo", "AcademicsController", ex.Message);
                }
            }
            return View();
        }
        public JsonResult FetchFacultyInfoById(string sJsonFacultyId)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sFacultyId = Convert.ToString(sJsonFacultyId);
                    Session[Common.SESSION_VARIABLES.FACULTY_ID] = sFacultyId;
                    dv.Clear();
                    dv.Add(Common.CP_FACULTY_2017.FACULTY_ID, sFacultyId, EnumCommand.DataType.String);
                    //AcademicsViewModel objFacutly = new AcademicsViewModel();
                    DataTable dtFaculty = new DataTable();
                    using (AcademicsViewModel objFaculty = new AcademicsViewModel())
                    {
                        dtFaculty = objFaculty.FetchFacultyInfoById(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    }
                    if (dtFaculty != null && dtFaculty.Rows.Count > 0)
                    {
                        FacultyInfo objFaculty = new FacultyInfo()
                        {
                            FACULTY_ID = dtFaculty.Rows[0][Common.CP_FACULTY_2017.FACULTY_ID].ToString(),
                            FACULTY = dtFaculty.Rows[0][Common.CP_FACULTY_2017.FACULTY].ToString(),
                            FACULTY_CODE = dtFaculty.Rows[0][Common.CP_FACULTY_2017.FACULTY_CODE].ToString(),
                            FACULTY_ORDER = dtFaculty.Rows[0][Common.CP_FACULTY_2017.FACULTY_ORDER].ToString(),
                            FAC_SERIES = dtFaculty.Rows[0][Common.CP_FACULTY_2017.FAC_SERIES].ToString(),
                        };
                        return Json(objFaculty);
                    }
                    else
                    {
                        return Json(sResult);
                    }
                }
                else
                    return Json(sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchFacultyInfoById", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchFacultyInfoById", "AcademicsController", ex.Message);
                    return Json(sResult = ex.Message);
                }
            }
        }
        public JsonResult UpdateFacultyInfo(string sJsonFaculty)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var sFacultyId = Session[Common.SESSION_VARIABLES.FACULTY_ID].ToString();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (sJsonFaculty != null)
                    {
                        var Result = JsonConvert.DeserializeObject<FacultyInfo>(sJsonFaculty);
                        dv.Clear();
                        dv.Add(Common.CP_FACULTY_2017.FACULTY_ID, sFacultyId, EnumCommand.DataType.String);
                        dv.Add(Common.CP_FACULTY_2017.FACULTY, Result.FACULTY, EnumCommand.DataType.String);
                        dv.Add(Common.CP_FACULTY_2017.FACULTY_CODE, Result.FACULTY_CODE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_FACULTY_2017.FACULTY_ORDER, Result.FACULTY_ORDER, EnumCommand.DataType.String);
                        dv.Add(Common.CP_FACULTY_2017.FAC_SERIES, Result.FAC_SERIES, EnumCommand.DataType.String);
                        using (AcademicsViewModel objFacutly = new AcademicsViewModel())
                        {
                            resultArg = objFacutly.UpdateFacultyInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                            sResult = (resultArg.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        }
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
                    objHandler.WriteError("UpdateFacultyInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateFacultyInfo", "AcademicsController", ex.Message);
                }
                sResult = Common.ErrorMessage.InternalServerError;
            }
            return Json(sResult);
        }
        public JsonResult DeleteFacultyInfo(string id)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    using (AcademicsViewModel objFaculty = new AcademicsViewModel())
                    {
                        dv.Clear();
                        dv.Add(Common.CP_FACULTY_2017.FACULTY_ID, id, EnumCommand.DataType.String);
                        resultArg = objFaculty.DeleteFacultyInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                        sResult = resultArg.Success.ToString();
                    }
                    ObjJsonData.Message = (resultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.sResult = sResult;
                    //if (sResult == "True") 
                    //    TempData["DeleteSuccess"] = "Record deleted successfully ...!";
                    //else
                    //    TempData["DeleteError"] = "Record not deleted Try again ...!";
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    ObjJsonData.sResult = "False";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DeleteFacultyInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DeleteFacultyInfo", "AcademicsController", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.sResult = "False";
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Class
        public ActionResult ClassInfo()
        {
            ClassModel objClass = new ClassModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    DataTable dtClassType = new DataTable();
                    DataTable dtClassLevel = new DataTable();
                    DataTable dtClassYear = new DataTable();
                    DataTable dtProgramme = new DataTable();
                    DataTable dtDepartment = new DataTable();
                    DataTable dtLanguage = new DataTable();
                    DataTable dtShift = new DataTable();
                    DataTable dtCourseId = new DataTable();

                    dtClassType = SupportDataMethod.FetchClassType().DataSource.Table;
                    dtClassLevel = SupportDataMethod.FetchClassLevel().DataSource.Table;
                    dtClassYear = SupportDataMethod.FetchClassYear().DataSource.Table;
                    dtProgramme = SupportDataMethod.FetchProgrammeGroupProgramme().DataSource.Table;
                    dtDepartment = SupportDataMethod.FetchDepartment((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    dtLanguage = SupportDataMethod.FetchLanguage().DataSource.Table;
                    dtShift = SupportDataMethod.FetchShift().DataSource.Table;
                    dtCourseId = SupportDataMethod.FetchNMECourses((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;

                    List<SupClassType> liClassType = new List<SupClassType>();
                    List<SupClassLevel> liClassLevel = new List<SupClassLevel>();
                    List<SubClassYear> liClassYear = new List<SubClassYear>();
                    List<cp_Program_2017> liProgramme = new List<cp_Program_2017>();
                    List<SupDepartment> liDepartment = new List<SupDepartment>();
                    List<Sup_Language> liLanguage = new List<Sup_Language>();
                    List<Sup_Shift> liShift = new List<Sup_Shift>();
                    List<cp_Course_Root_2017> liCourse = new List<cp_Course_Root_2017>();

                    if (dtDepartment != null && dtDepartment.Rows.Count > 0)
                    {
                        liDepartment = (from DataRow dr in dtDepartment.Rows
                                        select new SupDepartment()
                                        {
                                            DEPARTMENT_ID = dr[Common.SUP_DEPARTMENT.DEPARTMENT_ID].ToString(),
                                            DEPARTMENT = dr[Common.SUP_DEPARTMENT.DEPARTMENT].ToString()
                                        }).ToList();
                        objClass.DEPARTMENT = new SelectList(liDepartment, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                    }

                    if (dtClassType != null && dtClassType.Rows.Count > 0)
                    {
                        liClassType = (from DataRow dr in dtClassType.Rows
                                       select new SupClassType()
                                       {
                                           CLASS_TYPE_ID = dr[Common.SUP_CLASS_TYPE.CLASS_TYPE_ID].ToString(),
                                           CLASS_TYPE = dr[Common.SUP_CLASS_TYPE.CLASS_TYPE].ToString()
                                       }).ToList();
                        objClass.CLASS_TYPE = new SelectList(liClassType, Common.SUP_CLASS_TYPE.CLASS_TYPE_ID, Common.SUP_CLASS_TYPE.CLASS_TYPE);
                    }
                    if (dtClassLevel != null && dtClassLevel.Rows.Count > 0)
                    {
                        liClassLevel = (from DataRow dr in dtClassLevel.Rows
                                        select new SupClassLevel()
                                        {
                                            CLASSLEVELID = dr[Common.SUP_CLASS_LEVEL.CLASSLEVELID].ToString(),
                                            CLASSLEVEL = dr[Common.SUP_CLASS_LEVEL.CLASSLEVEL].ToString()
                                        }).ToList();
                        objClass.CLASS_LEVEL = new SelectList(liClassLevel, Common.SUP_CLASS_LEVEL.CLASSLEVELID, Common.SUP_CLASS_LEVEL.CLASSLEVEL);
                    }

                    if (dtClassYear != null && dtClassYear.Rows.Count > 0)
                    {
                        liClassYear = (from DataRow dr in dtClassYear.Rows
                                       select new SubClassYear()
                                       {
                                           CLASS_YEAR_ID = dr[Common.SUP_CLASS_YEAR.CLASS_YEAR_ID].ToString(),
                                           CLASS_YEAR = dr[Common.SUP_CLASS_YEAR.CLASS_YEAR_ID].ToString()
                                       }).ToList();
                        objClass.CLASS_YEAR = new SelectList(liClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                    }
                    if (dtProgramme != null && dtProgramme.Rows.Count > 0)
                    {
                        liProgramme = (from DataRow dr in dtProgramme.Rows
                                       select new cp_Program_2017()
                                       {
                                           PROGRAMME_GROUP_ID = dr[Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID].ToString(),
                                           PROGRAMME_NAME = dr[Common.CP_PROGRAMME_2017.PROGRAMME_NAME].ToString()
                                       }).ToList();
                        objClass.PROGRAMME = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    }

                    if (dtLanguage != null && dtLanguage.Rows.Count > 0)
                    {
                        liLanguage = (from DataRow dr in dtLanguage.Rows
                                      select new Sup_Language()
                                      {
                                          LANGUAGE_ID = dr[Common.SUP_LANGUAGE.LANGUAGE_ID].ToString(),
                                          LANGUAGE_NAME = dr[Common.SUP_LANGUAGE.LANGUAGE_NAME].ToString()
                                      }).ToList();
                        objClass.LANGUAGE = new SelectList(liLanguage, Common.SUP_LANGUAGE.LANGUAGE_ID, Common.SUP_LANGUAGE.LANGUAGE_NAME);
                    }
                    if (dtShift != null && dtShift.Rows.Count > 0)
                    {
                        liShift = (from DataRow dr in dtShift.Rows
                                   select new Sup_Shift()
                                   {
                                       SHIFT_ID = dr[Common.SUP_SHIFT.SHIFT_ID].ToString(),
                                       SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString()
                                   }).ToList();
                        objClass.SHIFT = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    if (dtCourseId != null && dtCourseId.Rows.Count > 0)
                    {
                        liCourse = (from DataRow dr in dtCourseId.Rows
                                    select new cp_Course_Root_2017()
                                    {
                                        COURSE_ROOT_ID = dr[Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID].ToString(),
                                        COURSE_TITLE = dr[Common.CP_COURSE_ROOT_2017.COURSE_TITLE].ToString()
                                    }).ToList();
                        objClass.COURSE_ID = new SelectList(liCourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_TITLE);
                    }

                    var liProgrammemode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null,
                         SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;
                    if (liProgrammemode != null && liProgrammemode.Count > 0)
                    {
                        objClass.PROGRAMME_MODE = new SelectList(liProgrammemode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ClassInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ClassInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objClass);
        }
        [UserRights("ADMIN")]
        public ActionResult ListClassInfo()
        {
            AcademicsViewModel objClassList = new AcademicsViewModel();
            var liClass = new List<ClassInfo>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    DataTable dtClassList = new DataTable();
                    dtClassList = objClassList.FetchClassInfo((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    if (dtClassList != null && dtClassList.Rows.Count > 0)
                    {
                        objClassList.liClassList = (from DataRow dr in dtClassList.Rows
                                                    select new ClassInfo()
                                                    {
                                                        CLASS_ID = dr[Common.CP_CLASSES_2017.CLASS_ID].ToString(),
                                                        CLASS_CODE = dr[Common.CP_CLASSES_2017.CLASS_CODE].ToString(),
                                                        CLASS_NAME = dr[Common.CP_CLASSES_2017.CLASS_NAME].ToString(),
                                                        //CLASS_DESCRIPTION = dr[Common.CP_CLASSES_2017.CLASS_DESCRIPTION].ToString(),
                                                        //CLASS_LEVEL = dr[Common.SUP_CLASS_LEVEL.CLASSLEVEL].ToString(),
                                                        CLASS_TYPE = dr[Common.SUP_CLASS_TYPE.CLASS_TYPE].ToString(),
                                                        //CLASS_YEAR = dr[Common.SUP_CLASS_YEAR.CLASS_YEAR].ToString(),
                                                        PROGRAMME = dr[Common.CP_PROGRAMME_2017.PROGRAMME_NAME].ToString(),
                                                        DEPARTMENT = dr[Common.CP_DEPARTMENT_2017.DEPARTMENT].ToString(),
                                                        //LANGUAGE = dr[Common.SUP_LANGUAGE.LANGUAGE_NAME].ToString(),
                                                        //IS_CHOICE = dr[Common.SUP_IS_CHOICE.IS_CHOICE_NAME].ToString(),
                                                        //CLASS_ORDER = dr[Common.CP_CLASSES_2017.CLASS_ORDER].ToString(),
                                                        SHIFT = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString(),
                                                        //COURSE_ID = dr[Common.CP_COURSE_ROOT_2017.COURSE_TITLE].ToString(),
                                                    }).ToList();
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListClassInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListClassInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objClassList);
        }
        public JsonResult InsertClassInfo(string sJsonClass)
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                if (sJsonClass != null)
                {
                    DataTable dtClass = new DataTable();
                    var Result = JsonConvert.DeserializeObject<ClassInfo>(sJsonClass);

                    dv.Clear();
                    dv.Add(Common.CP_CLASSES_2017.CLASS_CODE, Result.CLASS_CODE, EnumCommand.DataType.String);
                    using (AcademicsViewModel objClass = new AcademicsViewModel())
                    {
                        dtClass = objClass.CheckClass(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                        if (dtClass != null && dtClass.Rows.Count == 0)
                        {
                            var FetchProgrammeGroupInfo = (List<SUP_APPTYPE_PROG_GROUPS>)CMSHandler.FetchData<SUP_APPTYPE_PROG_GROUPS>(Result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupById)).DataSource.Data;
                            if (FetchProgrammeGroupInfo != null && FetchProgrammeGroupInfo.Count > 0)
                            {
                                Result.SHIFT = FetchProgrammeGroupInfo.FirstOrDefault().SHIFT;
                                Result.PROGRAMME_MODE = FetchProgrammeGroupInfo.FirstOrDefault().PROGRAMME_MODE;
                            }
                            dv.Clear();
                            if (String.IsNullOrEmpty(Result.COURSE_ID))
                            {
                                Result.COURSE_ID = "0";
                            }
                            dv.Add(Common.CP_CLASSES_2017.CLASS_CODE, Result.CLASS_CODE, EnumCommand.DataType.String);
                            dv.Add(Common.CP_CLASSES_2017.CLASS_NAME, Result.CLASS_NAME, EnumCommand.DataType.String);
                            dv.Add(Common.CP_CLASSES_2017.CLASS_DESCRIPTION, Result.CLASS_DESCRIPTION, EnumCommand.DataType.String);
                            dv.Add(Common.CP_CLASSES_2017.CLASS_TYPE, Result.CLASS_TYPE, EnumCommand.DataType.String);
                            dv.Add(Common.CP_CLASSES_2017.CLASS_LEVEL, Result.CLASS_LEVEL, EnumCommand.DataType.String);
                            dv.Add(Common.CP_CLASSES_2017.CLASS_YEAR, Result.CLASS_YEAR, EnumCommand.DataType.String);
                            dv.Add(Common.CP_CLASSES_2017.PROGRAMME_GROUP_ID, Result.PROGRAMME_GROUP_ID, EnumCommand.DataType.String);
                            dv.Add(Common.CP_CLASSES_2017.DEPARTMENT, Result.DEPARTMENT, EnumCommand.DataType.String);
                            dv.Add(Common.CP_CLASSES_2017.LANGUAGE, Result.LANGUAGE, EnumCommand.DataType.String);
                            dv.Add(Common.CP_CLASSES_2017.IS_CHOICE, Result.IS_CHOICE, EnumCommand.DataType.String);
                            dv.Add(Common.CP_CLASSES_2017.CLASS_ORDER, Result.CLASS_ORDER, EnumCommand.DataType.String);
                            dv.Add(Common.CP_CLASSES_2017.SHIFT, Result.SHIFT, EnumCommand.DataType.String);
                            dv.Add(Common.CP_CLASSES_2017.PROGRAMME_MODE, Result.PROGRAMME_MODE, EnumCommand.DataType.String);
                            dv.Add(Common.CP_CLASSES_2017.COURSE_ID, Result.COURSE_ID, EnumCommand.DataType.String);
                            resultArg = objClass.InsertClassInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                            sResult = resultArg.Success.ToString();
                            if (sResult == "True")
                            {
                                sResult = "Records Saved Successfully";
                            }
                            else
                            {
                                sResult = "Enter Correct Values";
                            }
                        }
                        else
                        {
                            sResult = "Class Already Exists";
                        }
                    }
                }
            }
            return Json(sResult);
        }
        public ActionResult EditClassInfo(int id)
        {
            string sClassId = Convert.ToString(id);
            Session[Common.SESSION_VARIABLES.CLASS_ID] = sClassId;

            ClassModel objClass = new ClassModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                DataTable dtClassType = new DataTable();
                DataTable dtClassLevel = new DataTable();
                DataTable dtClassYear = new DataTable();
                DataTable dtProgramme = new DataTable();
                DataTable dtDepartment = new DataTable();
                DataTable dtLanguage = new DataTable();
                DataTable dtShift = new DataTable();
                DataTable dtCourseId = new DataTable();

                dtClassType = SupportDataMethod.FetchClassType().DataSource.Table;
                dtClassLevel = SupportDataMethod.FetchClassLevel().DataSource.Table;
                dtClassYear = SupportDataMethod.FetchClassYear().DataSource.Table;
                dtProgramme = SupportDataMethod.FetchProgrammeGroupProgramme().DataSource.Table;
                dtDepartment = SupportDataMethod.FetchDepartment((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                dtLanguage = SupportDataMethod.FetchLanguage().DataSource.Table;
                dtShift = SupportDataMethod.FetchShift().DataSource.Table;
                dtCourseId = SupportDataMethod.FetchNMECourses((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;

                List<SupClassType> liClassType = new List<SupClassType>();
                List<SupClassLevel> liClassLevel = new List<SupClassLevel>();
                List<SubClassYear> liClassYear = new List<SubClassYear>();
                List<cp_Program_2017> liProgramme = new List<cp_Program_2017>();
                List<SupDepartment> liDepartment = new List<SupDepartment>();
                List<Sup_Language> liLanguage = new List<Sup_Language>();
                List<Sup_Shift> liShift = new List<Sup_Shift>();
                List<cp_Course_Root_2017> liCourse = new List<cp_Course_Root_2017>();

                if (dtDepartment != null && dtDepartment.Rows.Count > 0)
                {
                    liDepartment = (from DataRow dr in dtDepartment.Rows
                                    select new SupDepartment()
                                    {
                                        DEPARTMENT_ID = dr[Common.SUP_DEPARTMENT.DEPARTMENT_ID].ToString(),
                                        DEPARTMENT = dr[Common.SUP_DEPARTMENT.DEPARTMENT].ToString()
                                    }).ToList();
                    objClass.DEPARTMENT = new SelectList(liDepartment, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                }

                if (dtClassType != null && dtClassType.Rows.Count > 0)
                {
                    liClassType = (from DataRow dr in dtClassType.Rows
                                   select new SupClassType()
                                   {
                                       CLASS_TYPE_ID = dr[Common.SUP_CLASS_TYPE.CLASS_TYPE_ID].ToString(),
                                       CLASS_TYPE = dr[Common.SUP_CLASS_TYPE.CLASS_TYPE].ToString()
                                   }).ToList();
                    objClass.CLASS_TYPE = new SelectList(liClassType, Common.SUP_CLASS_TYPE.CLASS_TYPE_ID, Common.SUP_CLASS_TYPE.CLASS_TYPE);
                }
                if (dtClassLevel != null && dtClassLevel.Rows.Count > 0)
                {
                    liClassLevel = (from DataRow dr in dtClassLevel.Rows
                                    select new SupClassLevel()
                                    {
                                        CLASSLEVELID = dr[Common.SUP_CLASS_LEVEL.CLASSLEVELID].ToString(),
                                        CLASSLEVEL = dr[Common.SUP_CLASS_LEVEL.CLASSLEVEL].ToString()
                                    }).ToList();
                    objClass.CLASS_LEVEL = new SelectList(liClassLevel, Common.SUP_CLASS_LEVEL.CLASSLEVELID, Common.SUP_CLASS_LEVEL.CLASSLEVEL);
                }

                if (dtClassYear != null && dtClassYear.Rows.Count > 0)
                {
                    liClassYear = (from DataRow dr in dtClassYear.Rows
                                   select new SubClassYear()
                                   {
                                       CLASS_YEAR_ID = dr[Common.SUP_CLASS_YEAR.CLASS_YEAR_ID].ToString(),
                                       CLASS_YEAR = dr[Common.SUP_CLASS_YEAR.CLASS_YEAR_ID].ToString()
                                   }).ToList();
                    objClass.CLASS_YEAR = new SelectList(liClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                }
                if (dtProgramme != null && dtProgramme.Rows.Count > 0)
                {
                    liProgramme = (from DataRow dr in dtProgramme.Rows
                                   select new cp_Program_2017()
                                   {
                                       PROGRAMME_GROUP_ID = dr[Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID].ToString(),
                                       PROGRAMME_NAME = dr[Common.CP_PROGRAMME_2017.PROGRAMME_NAME].ToString()
                                   }).ToList();
                    objClass.PROGRAMME = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                }

                if (dtLanguage != null && dtLanguage.Rows.Count > 0)
                {
                    liLanguage = (from DataRow dr in dtLanguage.Rows
                                  select new Sup_Language()
                                  {
                                      LANGUAGE_ID = dr[Common.SUP_LANGUAGE.LANGUAGE_ID].ToString(),
                                      LANGUAGE_NAME = dr[Common.SUP_LANGUAGE.LANGUAGE_NAME].ToString()
                                  }).ToList();
                    objClass.LANGUAGE = new SelectList(liLanguage, Common.SUP_LANGUAGE.LANGUAGE_ID, Common.SUP_LANGUAGE.LANGUAGE_NAME);
                }
                if (dtShift != null && dtShift.Rows.Count > 0)
                {
                    liShift = (from DataRow dr in dtShift.Rows
                               select new Sup_Shift()
                               {
                                   SHIFT_ID = dr[Common.SUP_SHIFT.SHIFT_ID].ToString(),
                                   SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString()
                               }).ToList();
                    objClass.SHIFT = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }
                if (dtCourseId != null && dtCourseId.Rows.Count > 0)
                {
                    liCourse = (from DataRow dr in dtCourseId.Rows
                                select new cp_Course_Root_2017()
                                {
                                    COURSE_ROOT_ID = dr[Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID].ToString(),
                                    COURSE_TITLE = dr[Common.CP_COURSE_ROOT_2017.COURSE_TITLE].ToString()
                                }).ToList();
                    objClass.COURSE_ID = new SelectList(liCourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_TITLE);
                }
            }
            return View(objClass);
        }
        public JsonResult FetchClassInfoById(string id)
        {
            string sClassId = Convert.ToString(id);
            Session[Common.SESSION_VARIABLES.CLASS_ID] = sClassId;
            dv.Clear();
            dv.Add(Common.CP_CLASSES_2017.CLASS_ID, sClassId, EnumCommand.DataType.String);
            //AcademicsViewModel objDepartment = new AcademicsViewModel();
            DataTable dtFetchclassById = new DataTable();
            using (AcademicsViewModel objClass = new AcademicsViewModel())
            {
                dtFetchclassById = objClass.FetchClassInfoById(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
            }
            if (dtFetchclassById != null && dtFetchclassById.Rows.Count > 0)
            {
                ClassInfo objClass = new ClassInfo()
                {
                    CLASS_CODE = dtFetchclassById.Rows[0][Common.CP_CLASSES_2017.CLASS_CODE].ToString(),
                    CLASS_NAME = dtFetchclassById.Rows[0][Common.CP_CLASSES_2017.CLASS_NAME].ToString(),
                    CLASS_DESCRIPTION = dtFetchclassById.Rows[0][Common.CP_CLASSES_2017.CLASS_DESCRIPTION].ToString(),
                    CLASS_TYPE = dtFetchclassById.Rows[0][Common.CP_CLASSES_2017.CLASS_TYPE].ToString(),
                    CLASS_LEVEL = dtFetchclassById.Rows[0][Common.CP_CLASSES_2017.CLASS_LEVEL].ToString(),
                    CLASS_YEAR = dtFetchclassById.Rows[0][Common.CP_CLASSES_2017.CLASS_YEAR].ToString(),
                    PROGRAMME_GROUP_ID = dtFetchclassById.Rows[0][Common.CP_CLASSES_2017.PROGRAMME_GROUP_ID].ToString(),
                    DEPARTMENT = dtFetchclassById.Rows[0][Common.CP_CLASSES_2017.DEPARTMENT].ToString(),
                    LANGUAGE = dtFetchclassById.Rows[0][Common.CP_CLASSES_2017.LANGUAGE].ToString(),
                    IS_CHOICE = dtFetchclassById.Rows[0][Common.CP_CLASSES_2017.IS_CHOICE].ToString(),
                    CLASS_ORDER = dtFetchclassById.Rows[0][Common.CP_CLASSES_2017.CLASS_ORDER].ToString(),
                    SHIFT = dtFetchclassById.Rows[0][Common.CP_CLASSES_2017.SHIFT].ToString(),
                    COURSE_ID = dtFetchclassById.Rows[0][Common.CP_CLASSES_2017.COURSE_ID].ToString()
                };
                return Json(objClass);
            }
            else
            {
                return Json(sResult);
            }

        }
        public JsonResult UpdateClassInfo(string sJsonClass)
        {
            var sClassId = (Session[Common.SESSION_VARIABLES.CLASS_ID] != null) ? Session[Common.SESSION_VARIABLES.CLASS_ID].ToString() : string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (sJsonClass != null)
                    {
                        var Result = JsonConvert.DeserializeObject<ClassInfo>(sJsonClass);
                        var FetchProgrammeGroupInfo = (List<SUP_APPTYPE_PROG_GROUPS>)CMSHandler.FetchData<SUP_APPTYPE_PROG_GROUPS>(Result, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupById)).DataSource.Data;
                        if (FetchProgrammeGroupInfo != null && FetchProgrammeGroupInfo.Count > 0)
                        {
                            Result.SHIFT = FetchProgrammeGroupInfo.FirstOrDefault().SHIFT;
                            Result.PROGRAMME_MODE = FetchProgrammeGroupInfo.FirstOrDefault().PROGRAMME_MODE;
                        }
                        if (String.IsNullOrEmpty(Result.COURSE_ID))
                        {
                            Result.COURSE_ID = "0";
                        }
                        dv.Clear();
                        dv.Add(Common.CP_CLASSES_2017.CLASS_ID, sClassId, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.CLASS_CODE, Result.CLASS_CODE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.CLASS_NAME, Result.CLASS_NAME, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.CLASS_DESCRIPTION, Result.CLASS_DESCRIPTION, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.CLASS_TYPE, Result.CLASS_TYPE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.CLASS_LEVEL, Result.CLASS_LEVEL, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.CLASS_YEAR, Result.CLASS_YEAR, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.PROGRAMME_GROUP_ID, Result.PROGRAMME_GROUP_ID, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.DEPARTMENT, Result.DEPARTMENT, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.LANGUAGE, Result.LANGUAGE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.IS_CHOICE, Result.IS_CHOICE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.CLASS_ORDER, Result.CLASS_ORDER, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.SHIFT, Result.SHIFT, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.PROGRAMME_MODE, Result.PROGRAMME_MODE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_CLASSES_2017.COURSE_ID, Result.COURSE_ID, EnumCommand.DataType.String);
                        using (AcademicsViewModel objClass = new AcademicsViewModel())
                        {
                            resultArg = objClass.UpdateClassInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                            sResult = (resultArg.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("UpdateClassInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("UpdateClassInfo", "AcademicsController", ex.Message);
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
        public JsonResult DeleteClassInfo(string id)
        {
            var ObjJsonData = new JsonResultData();
            var sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    using (AcademicsViewModel objClass = new AcademicsViewModel())
                    {
                        dv.Clear();
                        dv.Add(Common.CP_CLASSES_2017.CLASS_ID, id, EnumCommand.DataType.String);
                        resultArg = objClass.DeleteClassInfo(dv, sAcademicYear);
                        sResult = resultArg.Success.ToString();
                    }
                    ObjJsonData.Message = (resultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.sResult = sResult;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DeleteClassInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DeleteClassInfo", "AcademicsController", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.sResult = "False";
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.sResult = "False";
            }

            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Batch
        public ActionResult BatchInfo()
        {
            BatchModel objBatch = new BatchModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    DataTable dtBatch = new DataTable();
                    DataTable dtAcademicYear = new DataTable();
                    dtBatch = SupportDataMethod.FetchBatch().DataSource.Table;
                    dtAcademicYear = SupportDataMethod.FetchAcademicYearList().DataSource.Table;
                    var liBatch = new List<sup_Batch>();
                    var liAcademicYear = new List<SUP_ACADEMIC_YEAR_LIST>();
                    //objBatch.BatchList = (List<BatchInfo>)CMSHandler.FetchData<BatchInfo>(null,
                    //    SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchBatch)).DataSource.Data;
                    if (dtBatch != null && dtBatch.Rows.Count > 0)
                    {
                        liBatch = (from DataRow dr in dtBatch.Rows
                                   select new sup_Batch()
                                   {
                                       BATCH_ID = dr[Common.SUP_BATCHES.BATCH_ID].ToString(),
                                       BATCH = dr[Common.SUP_BATCHES.BATCH].ToString()
                                   }).ToList();
                        objBatch.BATCH_ID = new SelectList(liBatch, Common.SUP_BATCHES.BATCH_ID, Common.SUP_BATCHES.BATCH);
                    }
                    if (dtAcademicYear != null && dtAcademicYear.Rows.Count > 0)
                    {
                        liAcademicYear = (from DataRow dr in dtAcademicYear.Rows
                                          select new SUP_ACADEMIC_YEAR_LIST()
                                          {
                                              ACADEMIC_YEAR_ID = dr[Common.ACC_YEAR.ACADEMIC_YEAR_ID].ToString(),
                                              AC_YEAR = dr[Common.ACC_YEAR.AC_YEAR].ToString()
                                          }).ToList();
                        objBatch.ACADEMIC_YEAR_ID = new SelectList(liAcademicYear, Common.ACC_YEAR.ACADEMIC_YEAR_ID, Common.ACC_YEAR.AC_YEAR);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BatchInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BatchInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objBatch);
        }
        [UserRights("ADMIN")]
        public ActionResult ListBatchInfo()
        {
            var liBatch = new List<BatchInfo>();
            AcademicsViewModel objBatch = new AcademicsViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {

                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    DataTable dtBatch = new DataTable();
                    dtBatch = objBatch.FetchBatchInfo((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    if (dtBatch != null && dtBatch.Rows.Count > 0)
                    {
                        objBatch.liBatchList = (from DataRow dr in dtBatch.Rows
                                                select new BatchInfo()
                                                {
                                                    AC_BATCH_ID = dr[Common.ACADEMIC_BATCHES.AC_BATCH_ID].ToString(),
                                                    ACADEMIC_YEAR_ID = dr[Common.ACADEMIC_YEAR.AC_YEAR].ToString(),
                                                    BATCH_ID = dr[Common.SUP_BATCHES.BATCH].ToString(),
                                                }).ToList();
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListBatchInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListBatchInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objBatch);
        }
        public JsonResult InsertBatchInfo(string sJsonBatch)
        {
            string Result = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    JsonBatchInfo objJson = JsonConvert.DeserializeObject<JsonBatchInfo>(sJsonBatch);
                    AcademicsViewModel objBatch = new AcademicsViewModel();
                    resultArg = objBatch.BulkSaveBatchInfo(objJson, Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString());
                    sResult = (resultArg.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                }
                else
                    sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("InsertBatchInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("InsertBatchInfo", "AcademicsController", ex.Message);
                }
                sResult = Common.ErrorMessage.InternalServerError;
            }
            return Json(sResult);
        }
        public JsonResult DeleteBatchInfo(string id)
        {
            var ObjJsonData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    using (AcademicsViewModel ObjViewModel = new AcademicsViewModel())
                    {
                        dv.Clear();
                        dv.Add(Common.ACADEMIC_BATCHES.AC_BATCH_ID, id, EnumCommand.DataType.String);
                        resultArg = CMSHandler.SaveOrUpdate(new sup_Batch() { AC_BATCH_ID = id }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.DeleteAcademicBatches));
                        //sResult = resultArg.Success.ToString();
                        ObjJsonData.Message = (resultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                        ObjJsonData.sResult = resultArg.Success.ToString();
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DeleteBatchInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DeleteBatchInfo", "AcademicsController", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.sResult = "False";
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.sResult = "False";
            }

            //if (sResult == "True")
            //{
            //    TempData["DeleteSuccess"] = "Record deleted successfully ...!";
            //}
            //else
            //{
            //    TempData["DeleteError"] = "Record not deleted Try again ...!";
            //}
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Degree
        public ActionResult DegreeInfo()
        {
            DegreeModel objDegree = new DegreeModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    DataTable dtProgrammetype = new DataTable();
                    DataTable dtProgrammeLevel = new DataTable();
                    DataTable dtFaculty = new DataTable();

                    dtProgrammeLevel = SupportDataMethod.FetchProgrammeLevel().DataSource.Table;
                    dtProgrammetype = SupportDataMethod.FetchProgrammeType().DataSource.Table;
                    dtFaculty = SupportDataMethod.FetchFaculty((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;

                    var liProgrammeLevel = new List<SupProgrammeLevel>();
                    var liProgrammeType = new List<SupProgrammeType>();
                    var liFaculty = new List<SupFaculty>();

                    if (dtFaculty != null && dtFaculty.Rows.Count > 0)
                    {
                        liFaculty = (from DataRow dr in dtFaculty.Rows
                                     select new SupFaculty()
                                     {
                                         FACULTY_ID = dr[Common.CP_FACULTY_2017.FACULTY_ID].ToString(),
                                         FACULTY = dr[Common.CP_FACULTY_2017.FACULTY].ToString()
                                     }).ToList();
                        objDegree.FACULTY = new SelectList(liFaculty, Common.CP_FACULTY_2017.FACULTY_ID, Common.CP_FACULTY_2017.FACULTY);
                    }
                    if (dtProgrammetype != null && dtProgrammetype.Rows.Count > 0)
                    {
                        liProgrammeType = (from DataRow dr in dtProgrammetype.Rows
                                           select new SupProgrammeType()
                                           {
                                               PROGRAMME_TYPE_ID = dr[Common.SUP_PROGRAMME_TYPE.PROGRAMME_TYPE_ID].ToString(),
                                               PROGRAMME_TYPE = dr[Common.SUP_PROGRAMME_TYPE.PROGRAMME_TYPE].ToString()
                                           }).ToList();
                        objDegree.PROGRAMME_TYPE = new SelectList(liProgrammeType, Common.SUP_PROGRAMME_TYPE.PROGRAMME_TYPE_ID, Common.SUP_PROGRAMME_TYPE.PROGRAMME_TYPE);
                    }
                    if (dtProgrammeLevel != null && dtProgrammeLevel.Rows.Count > 0)
                    {
                        liProgrammeLevel = (from DataRow dr in dtProgrammeLevel.Rows
                                            select new SupProgrammeLevel()
                                            {
                                                PROGRAMME_LEVEL_ID = dr[Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID].ToString(),
                                                PROGRAMME_LEVEL = dr[Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL].ToString()
                                            }).ToList();
                        objDegree.PROGRAMME_LEVEL = new SelectList(liProgrammeLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DegreeInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DegreeInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objDegree);
        }
        [UserRights("ADMIN")]
        public ActionResult ListDegreeInfo()
        {
            AcademicsViewModel objDegree = new AcademicsViewModel();
            var liDegree = new List<DegreeInfo>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    DataTable dtDegree = new DataTable();
                    dtDegree = objDegree.FetchDegreeInfo((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    if (dtDegree != null && dtDegree.Rows.Count > 0)
                    {
                        objDegree.liDegreeList = (from DataRow dr in dtDegree.Rows
                                                  select new DegreeInfo()
                                                  {
                                                      DEGREE_ID = dr[Common.CP_DEGREE_2017.DEGREE_ID].ToString(),
                                                      DEGREE = dr[Common.CP_DEGREE_2017.DEGREE].ToString(),
                                                      PROGRAMME_TYPE = dr[Common.CP_DEGREE_2017.PROGRAMME_TYPE].ToString(),
                                                      PROGRAMME_LEVEL = dr[Common.CP_DEGREE_2017.PROGRAMME_LEVEL].ToString(),
                                                      DEGREE_ORDER = dr[Common.CP_DEGREE_2017.DEGREE_ORDER].ToString(),
                                                      FACULTY = dr[Common.CP_DEGREE_2017.FACULTY].ToString(),
                                                      BOS_NAME = dr[Common.CP_DEGREE_2017.BOS_NAME].ToString(),
                                                      PREFIX = dr[Common.CP_DEGREE_2017.PREFIX].ToString(),
                                                      BOS_SERIES_ROLLNO = dr[Common.CP_DEGREE_2017.BOS_SERIES_ROLLNO].ToString(),
                                                      BOS_SERIES_REGNO = dr[Common.CP_DEGREE_2017.BOS_SERIES_REGNO].ToString(),
                                                      BOS_SERIES_ADMNO = dr[Common.CP_DEGREE_2017.BOS_SERIES_ADMNO].ToString(),
                                                  }).ToList();
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListDegreeInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListDegreeInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objDegree);
        }
        public JsonResult InsertDegreeInfo(string JsonDegree)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (JsonDegree != null)
                    {
                        DataTable dtDegree = new DataTable();
                        var Result = JsonConvert.DeserializeObject<DegreeInfo>(JsonDegree);
                        dv.Clear();
                        dv.Add(Common.CP_DEGREE_2017.PREFIX, Result.PREFIX, EnumCommand.DataType.String);
                        using (AcademicsViewModel objDegree = new AcademicsViewModel())
                        {
                            dtDegree = objDegree.CheckDegree(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                            if (dtDegree != null && dtDegree.Rows.Count == 0)
                            {
                                dv.Clear();
                                dv.Add(Common.CP_DEGREE_2017.DEGREE, Result.DEGREE, EnumCommand.DataType.String);
                                dv.Add(Common.CP_DEGREE_2017.PROGRAMME_TYPE, Result.PROGRAMME_TYPE, EnumCommand.DataType.String);
                                dv.Add(Common.CP_DEGREE_2017.PROGRAMME_LEVEL, Result.PROGRAMME_LEVEL, EnumCommand.DataType.String);
                                dv.Add(Common.CP_DEGREE_2017.DEGREE_ORDER, Result.DEGREE_ORDER, EnumCommand.DataType.String);
                                dv.Add(Common.CP_DEGREE_2017.FACULTY, Result.FACULTY, EnumCommand.DataType.String);
                                dv.Add(Common.CP_DEGREE_2017.BOS_NAME, Result.BOS_NAME, EnumCommand.DataType.String);
                                dv.Add(Common.CP_DEGREE_2017.PREFIX, Result.PREFIX, EnumCommand.DataType.String);
                                dv.Add(Common.CP_DEGREE_2017.BOS_SERIES_ROLLNO, Result.BOS_SERIES_ROLLNO, EnumCommand.DataType.String);
                                dv.Add(Common.CP_DEGREE_2017.BOS_SERIES_REGNO, Result.BOS_SERIES_REGNO, EnumCommand.DataType.String);
                                dv.Add(Common.CP_DEGREE_2017.BOS_SERIES_ADMNO, Result.BOS_SERIES_ADMNO, EnumCommand.DataType.String);
                                resultArg = objDegree.InsertDegreeInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                                sResult = resultArg.Success.ToString();
                                if (sResult == "True")
                                    sResult = "Records Saved Successfully";
                                else
                                    sResult = "Enter Correct Values";
                            }
                            else
                                sResult = "Degree Already Exists";
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
                    objHandler.WriteError("InsertDegreeInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("InsertDegreeInfo", "AcademicsController", ex.Message);
                }
            }
            return Json(sResult);
        }
        public ActionResult EditDegreeInfo(int id)
        {
            DegreeModel objDegree = new DegreeModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sDegreeId = Convert.ToString(id);
                    Session[Common.SESSION_VARIABLES.DEGREE_ID] = sDegreeId;
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    DataTable dtProgrammetype = new DataTable();
                    DataTable dtProgrammeLevel = new DataTable();
                    DataTable dtFaculty = new DataTable();

                    dtProgrammeLevel = SupportDataMethod.FetchProgrammeLevel().DataSource.Table;
                    dtProgrammetype = SupportDataMethod.FetchProgrammeType().DataSource.Table;
                    dtFaculty = SupportDataMethod.FetchFaculty((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;

                    var liProgrammeLevel = new List<SupProgrammeLevel>();
                    var liProgrammeType = new List<SupProgrammeType>();
                    var liFaculty = new List<SupFaculty>();

                    if (dtFaculty != null && dtFaculty.Rows.Count > 0)
                    {
                        liFaculty = (from DataRow dr in dtFaculty.Rows
                                     select new SupFaculty()
                                     {
                                         FACULTY_ID = dr[Common.CP_FACULTY_2017.FACULTY_ID].ToString(),
                                         FACULTY = dr[Common.CP_FACULTY_2017.FACULTY].ToString()
                                     }).ToList();
                        objDegree.FACULTY = new SelectList(liFaculty, Common.CP_FACULTY_2017.FACULTY_ID, Common.CP_FACULTY_2017.FACULTY);
                    }
                    if (dtProgrammetype != null && dtProgrammetype.Rows.Count > 0)
                    {
                        liProgrammeType = (from DataRow dr in dtProgrammetype.Rows
                                           select new SupProgrammeType()
                                           {
                                               PROGRAMME_TYPE_ID = dr[Common.SUP_PROGRAMME_TYPE.PROGRAMME_TYPE_ID].ToString(),
                                               PROGRAMME_TYPE = dr[Common.SUP_PROGRAMME_TYPE.PROGRAMME_TYPE].ToString()
                                           }).ToList();
                        objDegree.PROGRAMME_TYPE = new SelectList(liProgrammeType, Common.SUP_PROGRAMME_TYPE.PROGRAMME_TYPE_ID, Common.SUP_PROGRAMME_TYPE.PROGRAMME_TYPE);
                    }
                    if (dtProgrammeLevel != null && dtProgrammeLevel.Rows.Count > 0)
                    {
                        liProgrammeLevel = (from DataRow dr in dtProgrammeLevel.Rows
                                            select new SupProgrammeLevel()
                                            {
                                                PROGRAMME_LEVEL_ID = dr[Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID].ToString(),
                                                PROGRAMME_LEVEL = dr[Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL].ToString()
                                            }).ToList();
                        objDegree.PROGRAMME_LEVEL = new SelectList(liProgrammeLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListDegreeInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListDegreeInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objDegree);
        }
        public JsonResult FetchDegreeInfoById(string id)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sDegreeId = Convert.ToString(id);
                    Session[Common.SESSION_VARIABLES.DEGREE_ID] = sDegreeId;
                    dv.Clear();
                    dv.Add(Common.CP_DEGREE_2017.DEGREE_ID, sDegreeId, EnumCommand.DataType.String);
                    //AcademicsViewModel objDepartment = new AcademicsViewModel();
                    DataTable dtFetchDegreeInfoById = new DataTable();
                    using (AcademicsViewModel objDegree = new AcademicsViewModel())
                    {
                        dtFetchDegreeInfoById = objDegree.FetchDegreeInfoById(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    }
                    if (dtFetchDegreeInfoById != null && dtFetchDegreeInfoById.Rows.Count > 0)
                    {
                        DegreeInfo objDepartment = new DegreeInfo()
                        {
                            DEGREE = dtFetchDegreeInfoById.Rows[0][Common.CP_DEGREE_2017.DEGREE].ToString(),
                            PROGRAMME_TYPE = dtFetchDegreeInfoById.Rows[0][Common.CP_DEGREE_2017.PROGRAMME_TYPE].ToString(),
                            PROGRAMME_LEVEL = dtFetchDegreeInfoById.Rows[0][Common.CP_DEGREE_2017.PROGRAMME_LEVEL].ToString(),
                            DEGREE_ORDER = dtFetchDegreeInfoById.Rows[0][Common.CP_DEGREE_2017.DEGREE_ORDER].ToString(),
                            FACULTY = dtFetchDegreeInfoById.Rows[0][Common.CP_DEGREE_2017.FACULTY].ToString(),
                            BOS_NAME = dtFetchDegreeInfoById.Rows[0][Common.CP_DEGREE_2017.BOS_NAME].ToString(),
                            PREFIX = dtFetchDegreeInfoById.Rows[0][Common.CP_DEGREE_2017.PREFIX].ToString(),
                            BOS_SERIES_ROLLNO = dtFetchDegreeInfoById.Rows[0][Common.CP_DEGREE_2017.BOS_SERIES_ROLLNO].ToString(),
                            BOS_SERIES_REGNO = dtFetchDegreeInfoById.Rows[0][Common.CP_DEGREE_2017.BOS_SERIES_REGNO].ToString(),
                            BOS_SERIES_ADMNO = dtFetchDegreeInfoById.Rows[0][Common.CP_DEGREE_2017.BOS_SERIES_ADMNO].ToString(),

                        };
                        return Json(objDepartment);
                    }
                    else
                    {
                        return Json(sResult);
                    }
                }
                else
                    return Json(sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchDegreeInfoById", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchDegreeInfoById", "AcademicsController", ex.Message);
                    return Json(sResult = ex.Message);
                }
            }
        }
        public JsonResult UpdateDegreeInfo(string JsonDegree)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var sDegreeId = Session[Common.SESSION_VARIABLES.DEGREE_ID].ToString();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (JsonDegree != null)
                    {
                        var Result = JsonConvert.DeserializeObject<DegreeInfo>(JsonDegree);
                        dv.Clear();
                        dv.Add(Common.CP_DEGREE_2017.DEGREE_ID, sDegreeId, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEGREE_2017.DEGREE, Result.DEGREE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEGREE_2017.PROGRAMME_TYPE, Result.PROGRAMME_TYPE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEGREE_2017.PROGRAMME_LEVEL, Result.PROGRAMME_LEVEL, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEGREE_2017.FACULTY, Result.FACULTY, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEGREE_2017.DEGREE_ORDER, Result.DEGREE_ORDER, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEGREE_2017.BOS_NAME, Result.BOS_NAME, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEGREE_2017.PREFIX, Result.PREFIX, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEGREE_2017.BOS_SERIES_ROLLNO, Result.BOS_SERIES_ROLLNO, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEGREE_2017.BOS_SERIES_REGNO, Result.BOS_SERIES_REGNO, EnumCommand.DataType.String);
                        dv.Add(Common.CP_DEGREE_2017.BOS_SERIES_ADMNO, Result.BOS_SERIES_ADMNO, EnumCommand.DataType.String);
                        using (AcademicsViewModel objDepartment = new AcademicsViewModel())
                        {
                            resultArg = objDepartment.UpdateDegreeInfo(dv, sAcademicYear);
                            sResult = resultArg.Success.ToString();
                        }
                        ObjJsonData.Message = (resultArg.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        ObjJsonData.sResult = sResult;
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    ObjJsonData.sResult = "False";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateDegreeInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateDegreeInfo", "AcademicsController", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.sResult = "False";
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteDegreeInfo(string id)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    using (AcademicsViewModel objDegree = new AcademicsViewModel())
                    {
                        dv.Clear();
                        dv.Add(Common.CP_DEGREE_2017.DEGREE_ID, id, EnumCommand.DataType.String);
                        resultArg = objDegree.DeleteDegreeInfo(dv, sAcademicYear);
                        sResult = resultArg.Success.ToString();
                    }
                    ObjJsonData.Message = (resultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.sResult = sResult;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DeleteDegreeInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DeleteDegreeInfo", "AcademicsController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.sResult = "False";
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Programme
        public ActionResult ProgrammeInfo()
        {
            ProgrammeModel objProgramme = new ProgrammeModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    DataTable dtDepartment = new DataTable();
                    DataTable dtDegree = new DataTable();
                    DataTable dtDurationUnit = new DataTable();
                    DataTable dtChannel = new DataTable();
                    DataTable dtSubjects = new DataTable();
                    DataTable dtMediumOfInstruction = new DataTable();

                    dtDepartment = SupportDataMethod.FetchDepartment((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    dtDegree = SupportDataMethod.FetchDegree().DataSource.Table;
                    dtDurationUnit = SupportDataMethod.FetchDurationUnit().DataSource.Table;
                    dtChannel = SupportDataMethod.FetchChannel().DataSource.Table;
                    dtSubjects = SupportDataMethod.FetchSubjects().DataSource.Table;
                    dtMediumOfInstruction = SupportDataMethod.FetchMediumOfInstruction().DataSource.Table;

                    var liDepartment = new List<SupDepartment>();
                    var liDegree = new List<SUP_DEGREE>();
                    var liDurationUnit = new List<Sup_Duration_Unit>();
                    var lichannel = new List<Sup_Channel>();
                    var liSubjects = new List<Sup_Subject>();
                    var liMediumOfInstruction = new List<SUP_MEDIUM_OF_INSTRUCTION>();


                    if (dtDepartment != null && dtDepartment.Rows.Count > 0)
                    {
                        liDepartment = (from DataRow dr in dtDepartment.Rows
                                        select new SupDepartment()
                                        {
                                            DEPARTMENT_ID = dr[Common.SUP_DEPARTMENT.DEPARTMENT_ID].ToString(),
                                            DEPARTMENT = dr[Common.SUP_DEPARTMENT.DEPARTMENT].ToString()
                                        }).ToList();
                        objProgramme.DEPARTMENT = new SelectList(liDepartment, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                    }
                    if (dtDegree != null && dtDegree.Rows.Count > 0)
                    {
                        liDegree = (from DataRow dr in dtDegree.Rows
                                    select new SUP_DEGREE()
                                    {
                                        DEGREE_ID = dr[Common.SUP_DEGREE.DEGREE_ID].ToString(),
                                        DEGREE = dr[Common.SUP_DEGREE.DEGREE].ToString()
                                    }).ToList();
                        objProgramme.DEGREE = new SelectList(liDegree, Common.SUP_DEGREE.DEGREE_ID, Common.SUP_DEGREE.DEGREE);
                    }
                    if (dtDurationUnit != null && dtDurationUnit.Rows.Count > 0)
                    {
                        liDurationUnit = (from DataRow dr in dtDurationUnit.Rows
                                          select new Sup_Duration_Unit()
                                          {
                                              DURATION_UNIT_ID = dr[Common.SUP_DURATION_UNIT.DURATION_UNIT_ID].ToString(),
                                              DURATION_UNIT = dr[Common.SUP_DURATION_UNIT.DURATION_UNIT].ToString()
                                          }).ToList();
                        objProgramme.DURATION_UNIT = new SelectList(liDurationUnit, Common.SUP_DURATION_UNIT.DURATION_UNIT_ID, Common.SUP_DURATION_UNIT.DURATION_UNIT);
                    }
                    if (dtChannel != null && dtChannel.Rows.Count > 0)
                    {
                        lichannel = (from DataRow dr in dtChannel.Rows
                                     select new Sup_Channel()
                                     {
                                         CHANNEL_ID = dr[Common.SUP_CHANNEL.CHANNEL_ID].ToString(),
                                         CHANNEL = dr[Common.SUP_CHANNEL.CHANNEL].ToString()
                                     }).ToList();
                        objProgramme.CHANNEL = new SelectList(lichannel, Common.SUP_CHANNEL.CHANNEL_ID, Common.SUP_CHANNEL.CHANNEL);
                    }
                    if (dtSubjects != null && dtSubjects.Rows.Count > 0)
                    {
                        liSubjects = (from DataRow dr in dtSubjects.Rows
                                      select new Sup_Subject()
                                      {
                                          SUBJECT_ID = dr[Common.SUP_SUBJECT.SUBJECT_ID].ToString(),
                                          SUBJECT = dr[Common.SUP_SUBJECT.SUBJECT].ToString()
                                      }).ToList();
                        objProgramme.SUBJECTS = new SelectList(liSubjects, Common.SUP_SUBJECT.SUBJECT_ID, Common.SUP_SUBJECT.SUBJECT);
                    }
                    if (dtMediumOfInstruction != null && dtMediumOfInstruction.Rows.Count > 0)
                    {
                        liMediumOfInstruction = (from DataRow dr in dtMediumOfInstruction.Rows
                                                 select new SUP_MEDIUM_OF_INSTRUCTION()
                                                 {
                                                     MEDIUM_ID = dr[Common.SUP_MEDIUM_OF_INSTRUCTION.MEDIUM_ID].ToString(),
                                                     MEDIUM_OF_INSTRUCTION = dr[Common.SUP_MEDIUM_OF_INSTRUCTION.MEDIUM_OF_INSTRUCTION].ToString()
                                                 }).ToList();
                        objProgramme.MEDIUM_OF_INSTRACTION = new SelectList(liMediumOfInstruction, Common.SUP_MEDIUM_OF_INSTRUCTION.MEDIUM_ID, Common.SUP_MEDIUM_OF_INSTRUCTION.MEDIUM_OF_INSTRUCTION);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ProgrammeInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ProgrammeInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objProgramme);
        }
        [UserRights("ADMIN")]
        public ActionResult listProgrammeInfo()
        {
            AcademicsViewModel objProgramme = new AcademicsViewModel();
            var liProgramme = new List<ProgrammeInfo>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    DataTable dtProgrammeList = new DataTable();
                    dtProgrammeList = objProgramme.FetchProgrammeInfo((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    if (dtProgrammeList != null && dtProgrammeList.Rows.Count > 0)
                    {
                        objProgramme.liProgrammeList = (from DataRow dr in dtProgrammeList.Rows
                                                        select new ProgrammeInfo()
                                                        {
                                                            PROGRAMME_ID = dr[Common.CP_PROGRAMME_2017.PROGRAMME_ID].ToString(),
                                                            PROGRAMME_CODE = dr[Common.CP_PROGRAMME_2017.PROGRAMME_CODE].ToString(),
                                                            PROGRAMME_NAME = dr[Common.CP_PROGRAMME_2017.PROGRAMME_NAME].ToString(),
                                                            PROGRAMME_DESCRIPTION = dr[Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION].ToString(),
                                                            DEPARTMENT = dr[Common.CP_DEPARTMENT_2017.DEPARTMENT].ToString(),
                                                            DEGREE = dr[Common.CP_DEGREE_2017.DEGREE].ToString(),
                                                            PROGRAMME_ORDER = dr[Common.CP_PROGRAMME_2017.PROGRAMME_ORDER].ToString(),
                                                            DURATION_UNIT = dr[Common.SUP_DURATION_UNIT.DURATION_UNIT].ToString(),
                                                            NO_OF_SEMESTER = dr[Common.CP_PROGRAMME_2017.NO_OF_SEMESTER].ToString(),
                                                            CHANNEL = dr[Common.SUP_CHANNEL.CHANNEL].ToString(),
                                                            IS_AIDED = dr[Common.SUP_IS_AIDED.IS_AIDED_NAME].ToString(),
                                                            PRO_SERIES_ROLLNO = dr[Common.CP_PROGRAMME_2017.PRO_SERIES_ROLLNO].ToString(),
                                                            NON_AIDED = dr[Common.SUP_NON_AIDED.NON_AIDED_NAME].ToString(),
                                                            IS_REGULAR = dr[Common.SUP_IS_REGULAR.IS_REGULAR_NAME].ToString(),
                                                            SUBJECTS = dr[Common.SUP_SUBJECT.SUBJECT].ToString(),
                                                            PRO_SERIES_REGNO = dr[Common.CP_PROGRAMME_2017.PRO_SERIES_REGNO].ToString(),
                                                            PRO_SERIES_ADMNO = dr[Common.CP_PROGRAMME_2017.PRO_SERIES_ADMNO].ToString(),
                                                            MEDIUM_OF_INSTRACTION = dr[Common.SUP_MEDIUM_OF_INSTRUCTION.MEDIUM_OF_INSTRUCTION].ToString(),
                                                        }).ToList();
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("listProgrammeInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("listProgrammeInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objProgramme);
        }
        public JsonResult InsertProgrammeInfo(string sJsonProgramme)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (sJsonProgramme != null)
                    {
                        DataTable dtProgramme = new DataTable();
                        var Result = JsonConvert.DeserializeObject<ProgrammeInfo>(sJsonProgramme);
                        dv.Clear();
                        dv.Add(Common.CP_PROGRAMME_2017.PROGRAMME_CODE, Result.PROGRAMME_CODE, EnumCommand.DataType.String);
                        using (AcademicsViewModel objProgramme = new AcademicsViewModel())
                        {
                            dtProgramme = objProgramme.CheckProgramme(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                            if (dtProgramme != null && dtProgramme.Rows.Count == 0)
                            {
                                dv.Clear();
                                dv.Add(Common.CP_PROGRAMME_2017.PROGRAMME_CODE, Result.PROGRAMME_CODE, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.PROGRAMME_NAME, Result.PROGRAMME_NAME, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION, Result.PROGRAMME_DESCRIPTION, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.DEPARTMENT, Result.DEPARTMENT, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.DEGREE, Result.DEGREE, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.PROGRAMME_ORDER, Result.PROGRAMME_ORDER, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.DURATION_UNIT, Result.DURATION_UNIT, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.NO_OF_SEMESTER, Result.NO_OF_SEMESTER, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.CHANNEL, Result.CHANNEL, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.IS_AIDED, Result.IS_AIDED, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.PRO_SERIES_ROLLNO, Result.PRO_SERIES_ROLLNO, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.NON_AIDED, Result.NON_AIDED, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.IS_REGULAR, Result.IS_REGULAR, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.SUBJECTS, Result.SUBJECTS, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.PRO_SERIES_REGNO, Result.PRO_SERIES_REGNO, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.PRO_SERIES_ADMNO, Result.PRO_SERIES_ADMNO, EnumCommand.DataType.String);
                                dv.Add(Common.CP_PROGRAMME_2017.MEDIUM_OF_INSTRACTION, Result.MEDIUM_OF_INSTRACTION, EnumCommand.DataType.String);
                                resultArg = objProgramme.InsertProgrammeInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                                sResult = resultArg.Success.ToString();
                                if (sResult == "True")
                                    sResult = "Records Saved Successfully";
                                else
                                    sResult = "Enter Correct Values";
                            }
                            else
                                sResult = "Programme Already Exists...!";
                        }
                    }
                }
                else
                    return Json(sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("InsertProgrammeInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("InsertProgrammeInfo", "AcademicsController", ex.Message);
                }
            }
            return Json(sResult);
        }
        public ActionResult EditProgrammeInfo(int id)
        {
            ProgrammeModel objProgramme = new ProgrammeModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sProgrammeId = Convert.ToString(id);
                    Session[Common.SESSION_VARIABLES.PROGRAMME_ID] = sProgrammeId;
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    DataTable dtDepartment = new DataTable();
                    DataTable dtDegree = new DataTable();
                    DataTable dtDurationUnit = new DataTable();
                    DataTable dtChannel = new DataTable();
                    DataTable dtSubjects = new DataTable();
                    DataTable dtMediumOfInstruction = new DataTable();

                    dtDepartment = SupportDataMethod.FetchDepartment((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    dtDegree = SupportDataMethod.FetchDegree().DataSource.Table;
                    dtDurationUnit = SupportDataMethod.FetchDurationUnit().DataSource.Table;
                    dtChannel = SupportDataMethod.FetchChannel().DataSource.Table;
                    dtSubjects = SupportDataMethod.FetchSubjects().DataSource.Table;
                    dtMediumOfInstruction = SupportDataMethod.FetchMediumOfInstruction().DataSource.Table;

                    IList<SupDepartment> liDepartment = new List<SupDepartment>();
                    IList<SUP_DEGREE> liDegree = new List<SUP_DEGREE>();
                    IList<Sup_Duration_Unit> liDurationUnit = new List<Sup_Duration_Unit>();
                    IList<Sup_Channel> lichannel = new List<Sup_Channel>();
                    IList<Sup_Subject> liSubjects = new List<Sup_Subject>();
                    IList<SUP_MEDIUM_OF_INSTRUCTION> liMediumOfInstruction = new List<SUP_MEDIUM_OF_INSTRUCTION>();


                    if (dtDepartment != null && dtDepartment.Rows.Count > 0)
                    {
                        liDepartment = (from DataRow dr in dtDepartment.Rows
                                        select new SupDepartment()
                                        {
                                            DEPARTMENT_ID = dr[Common.SUP_DEPARTMENT.DEPARTMENT_ID].ToString(),
                                            DEPARTMENT = dr[Common.SUP_DEPARTMENT.DEPARTMENT].ToString()
                                        }).ToList();
                        objProgramme.DEPARTMENT = new SelectList(liDepartment, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                    }
                    if (dtDegree != null && dtDegree.Rows.Count > 0)
                    {
                        liDegree = (from DataRow dr in dtDegree.Rows
                                    select new SUP_DEGREE()
                                    {
                                        DEGREE_ID = dr[Common.SUP_DEGREE.DEGREE_ID].ToString(),
                                        DEGREE = dr[Common.SUP_DEGREE.DEGREE].ToString()
                                    }).ToList();
                        objProgramme.DEGREE = new SelectList(liDegree, Common.SUP_DEGREE.DEGREE_ID, Common.SUP_DEGREE.DEGREE);
                    }
                    if (dtDurationUnit != null && dtDurationUnit.Rows.Count > 0)
                    {
                        liDurationUnit = (from DataRow dr in dtDurationUnit.Rows
                                          select new Sup_Duration_Unit()
                                          {
                                              DURATION_UNIT_ID = dr[Common.SUP_DURATION_UNIT.DURATION_UNIT_ID].ToString(),
                                              DURATION_UNIT = dr[Common.SUP_DURATION_UNIT.DURATION_UNIT].ToString()
                                          }).ToList();
                        objProgramme.DURATION_UNIT = new SelectList(liDurationUnit, Common.SUP_DURATION_UNIT.DURATION_UNIT_ID, Common.SUP_DURATION_UNIT.DURATION_UNIT);
                    }
                    if (dtChannel != null && dtChannel.Rows.Count > 0)
                    {
                        lichannel = (from DataRow dr in dtChannel.Rows
                                     select new Sup_Channel()
                                     {
                                         CHANNEL_ID = dr[Common.SUP_CHANNEL.CHANNEL_ID].ToString(),
                                         CHANNEL = dr[Common.SUP_CHANNEL.CHANNEL].ToString()
                                     }).ToList();
                        objProgramme.CHANNEL = new SelectList(lichannel, Common.SUP_CHANNEL.CHANNEL_ID, Common.SUP_CHANNEL.CHANNEL);
                    }
                    if (dtSubjects != null && dtSubjects.Rows.Count > 0)
                    {
                        liSubjects = (from DataRow dr in dtSubjects.Rows
                                      select new Sup_Subject()
                                      {
                                          SUBJECT_ID = dr[Common.SUP_SUBJECT.SUBJECT_ID].ToString(),
                                          SUBJECT = dr[Common.SUP_SUBJECT.SUBJECT].ToString()
                                      }).ToList();
                        objProgramme.SUBJECTS = new SelectList(liSubjects, Common.SUP_SUBJECT.SUBJECT_ID, Common.SUP_SUBJECT.SUBJECT);
                    }
                    if (dtMediumOfInstruction != null && dtMediumOfInstruction.Rows.Count > 0)
                    {
                        liMediumOfInstruction = (from DataRow dr in dtMediumOfInstruction.Rows
                                                 select new SUP_MEDIUM_OF_INSTRUCTION()
                                                 {
                                                     MEDIUM_ID = dr[Common.SUP_MEDIUM_OF_INSTRUCTION.MEDIUM_ID].ToString(),
                                                     MEDIUM_OF_INSTRUCTION = dr[Common.SUP_MEDIUM_OF_INSTRUCTION.MEDIUM_OF_INSTRUCTION].ToString()
                                                 }).ToList();
                        objProgramme.MEDIUM_OF_INSTRACTION = new SelectList(liMediumOfInstruction, Common.SUP_MEDIUM_OF_INSTRUCTION.MEDIUM_ID, Common.SUP_MEDIUM_OF_INSTRUCTION.MEDIUM_OF_INSTRUCTION);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EditProgrammeInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EditProgrammeInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objProgramme);
        }
        public JsonResult UpdateProgrammeInfo(string JsonProgramme)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var sProgrammeId = Session[Common.SESSION_VARIABLES.PROGRAMME_ID].ToString();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (JsonProgramme != null)
                    {
                        var Result = JsonConvert.DeserializeObject<ProgrammeInfo>(JsonProgramme);
                        dv.Clear();
                        dv.Add(Common.CP_PROGRAMME_2017.PROGRAMME_ID, sProgrammeId, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.PROGRAMME_CODE, Result.PROGRAMME_CODE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.PROGRAMME_NAME, Result.PROGRAMME_NAME, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION, Result.PROGRAMME_DESCRIPTION, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.DEPARTMENT, Result.DEPARTMENT, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.DEGREE, Result.DEGREE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.PROGRAMME_ORDER, Result.PROGRAMME_ORDER, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.DURATION_UNIT, Result.DURATION_UNIT, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.NO_OF_SEMESTER, Result.NO_OF_SEMESTER, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.CHANNEL, Result.CHANNEL, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.IS_AIDED, Result.IS_AIDED, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.PRO_SERIES_ROLLNO, Result.PRO_SERIES_ROLLNO, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.NON_AIDED, Result.NON_AIDED, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.IS_REGULAR, Result.IS_REGULAR, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.SUBJECTS, Result.SUBJECTS, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.PRO_SERIES_REGNO, Result.PRO_SERIES_REGNO, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.PRO_SERIES_ADMNO, Result.PRO_SERIES_ADMNO, EnumCommand.DataType.String);
                        dv.Add(Common.CP_PROGRAMME_2017.MEDIUM_OF_INSTRACTION, Result.MEDIUM_OF_INSTRACTION, EnumCommand.DataType.String);
                        using (AcademicsViewModel objDepartment = new AcademicsViewModel())
                        {
                            resultArg = objDepartment.UpdateProgrammeInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                            sResult = resultArg.Success.ToString();
                        }
                        ObjJsonData.Message = (resultArg.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        ObjJsonData.sResult = sResult;
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    ObjJsonData.sResult = "False";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateProgrammeInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateProgrammeInfo", "AcademicsController", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchProgrammeInfoById(string id)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sProgrammeId = Convert.ToString(id);
                    Session[Common.SESSION_VARIABLES.PROGRAMME_ID] = sProgrammeId;
                    dv.Clear();
                    dv.Add(Common.CP_PROGRAMME_2017.PROGRAMME_ID, sProgrammeId, EnumCommand.DataType.String);
                    //AcademicsViewModel objDepartment = new AcademicsViewModel();
                    DataTable dtFetchProgrammeInfoById = new DataTable();
                    using (AcademicsViewModel objDepartment = new AcademicsViewModel())
                    {
                        dtFetchProgrammeInfoById = objDepartment.FetchProgrammeInfoById(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    }
                    if (dtFetchProgrammeInfoById != null && dtFetchProgrammeInfoById.Rows.Count > 0)
                    {
                        ProgrammeInfo objDepartment = new ProgrammeInfo()
                        {
                            PROGRAMME_CODE = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.PROGRAMME_CODE].ToString(),
                            PROGRAMME_NAME = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.PROGRAMME_NAME].ToString(),
                            PROGRAMME_DESCRIPTION = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.PROGRAMME_DESCRIPTION].ToString(),
                            DEPARTMENT = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.DEPARTMENT].ToString(),
                            DEGREE = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.DEGREE].ToString(),
                            PROGRAMME_ORDER = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.PROGRAMME_ORDER].ToString(),
                            NON_AIDED = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.NON_AIDED].ToString(),
                            DURATION_UNIT = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.DURATION_UNIT].ToString(),
                            NO_OF_SEMESTER = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.NO_OF_SEMESTER].ToString(),
                            CHANNEL = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.CHANNEL].ToString(),
                            IS_AIDED = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.IS_AIDED].ToString(),
                            PRO_SERIES_ROLLNO = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.PRO_SERIES_ROLLNO].ToString(),
                            IS_REGULAR = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.IS_REGULAR].ToString(),
                            SUBJECTS = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.SUBJECTS].ToString(),
                            PRO_SERIES_REGNO = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.PRO_SERIES_REGNO].ToString(),
                            PRO_SERIES_ADMNO = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.PRO_SERIES_ADMNO].ToString(),
                            MEDIUM_OF_INSTRACTION = dtFetchProgrammeInfoById.Rows[0][Common.CP_PROGRAMME_2017.MEDIUM_OF_INSTRACTION].ToString(),
                        };
                        return Json(objDepartment);
                    }
                    else
                    {
                        return Json(sResult);
                    }
                }
                else
                    return Json(sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchProgrammeInfoById", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchProgrammeInfoById", "AcademicsController", ex.Message);
                    return Json(sResult = ex.Message);
                }
            }
        }
        public JsonResult DeleteProgrammeInfo(string id)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    using (AcademicsViewModel objDepartment = new AcademicsViewModel())
                    {
                        dv.Clear();
                        dv.Add(Common.CP_PROGRAMME_2017.PROGRAMME_ID, id, EnumCommand.DataType.String);
                        resultArg = objDepartment.DeleteProgrammeInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                        sResult = resultArg.Success.ToString();
                    }
                    ObjJsonData.Message = (resultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.sResult = sResult;
                    //if (sResult == "True")
                    //    TempData["DeleteSuccess"] = "Record deleted successfully ...!";
                    //else
                    //    TempData["DeleteError"] = "Record not deleted Try again ...!";
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    ObjJsonData.sResult = "False";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DeleteProgrammeInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DeleteProgrammeInfo", "AcademicsController", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Class Course
        public JsonResult SaveClassCourses(string strJsonClassCourse)
        {
            string sResult = string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            AcademicsViewModel objAcademicViewModel = new AcademicsViewModel();
            JsonClassCourseRoot objJsonClassCourseRoot = JsonConvert.DeserializeObject<JsonClassCourseRoot>(strJsonClassCourse);
            resultArg = objAcademicViewModel.SaveClassCourse(objJsonClassCourseRoot, sAcademicYear);
            sResult = (resultArg.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
            return Json(sResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteClassCourse(string sAcademicYear, string sClassCourse)
        {
            string sResult = string.Empty;
            using (AcademicsViewModel objAcademicViewModel = new AcademicsViewModel())
            {
                resultArg = objAcademicViewModel.DeleteClassCourse(sAcademicYear, sClassCourse);
                sResult = (resultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
            }
            return Json(sResult, JsonRequestBehavior.AllowGet);
        }
        [UserRights("ADMIN")]
        public ActionResult ClassCourseList()
        {
            AcademicsViewModel objAcademicViewModel = new AcademicsViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                DataTable dtAcademicYearList = new DataTable();
                DataTable dtClasses = new DataTable();
                dtAcademicYearList = SupportDataMethod.FetchAcademicYearList().DataSource.Table;
                dtClasses = SupportDataMethod.FetchRegularClass(sAcademicYear).DataSource.Table;

                if (dtAcademicYearList != null && dtAcademicYearList.Rows.Count > 0)
                {
                    objAcademicViewModel.AcademicYearList = (from DataRow dr in dtAcademicYearList.Rows select new SUP_ACADEMIC_YEAR_LIST() { ACADEMIC_YEAR_ID = dr[Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID].ToString(), ACTIVE_YEAR = dr[Common.ACADEMIC_YEAR.ACTIVE_YEAR].ToString(), AC_YEAR = dr[Common.ACADEMIC_YEAR.AC_YEAR].ToString() }).ToList();
                    objAcademicViewModel.AcademicYearList.Add(new SUP_ACADEMIC_YEAR_LIST() { ACADEMIC_YEAR_ID = "0", ACTIVE_YEAR = "0", AC_YEAR = "--Select Academic Year--" });
                }
                else
                {
                    objAcademicViewModel.AcademicYearList.Add(new SUP_ACADEMIC_YEAR_LIST() { ACADEMIC_YEAR_ID = "0", ACTIVE_YEAR = "1", AC_YEAR = "--Select Academic Year--" });
                }
                if (dtClasses != null && dtClasses.Rows.Count > 0)
                {
                    objAcademicViewModel.ClassList = new List<SUP_CLASS>();
                    objAcademicViewModel.ClassList.Add(new SUP_CLASS() { CLASS_ID = "0", CLASS_NAME = "--Select Class--" });
                    objAcademicViewModel.ClassList = (from DataRow dr in dtClasses.Rows select new SUP_CLASS() { CLASS_ID = dr[Common.CP_CLASSES_2017.CLASS_ID].ToString(), CLASS_NAME = dr[Common.CP_CLASSES_2017.CLASS_NAME].ToString() }).ToList();

                }
                else
                {

                    objAcademicViewModel.ClassList = new List<SUP_CLASS>();

                    objAcademicViewModel.ClassList.Add(new SUP_CLASS() { CLASS_ID = "0", CLASS_NAME = "---Select---" });
                }
            }

            return View(objAcademicViewModel);
        }
        public ActionResult BindClassCourse(string sClassId, string sACYear)
        {
            AcademicsViewModel objAcademicViewModel = new AcademicsViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = string.Empty;
                sAcademicYear = (string.IsNullOrEmpty(sACYear) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : sACYear);
                DataTable dtClassCourseList = new DataTable();
                dtClassCourseList = objAcademicViewModel.FetchClassCourseList(sAcademicYear, sClassId).DataSource.Table;
                if (dtClassCourseList != null && dtClassCourseList.Rows.Count > 0)
                {
                    objAcademicViewModel.ClassCourseList = (from DataRow dr in dtClassCourseList.Rows
                                                            select new ClassCourse()
                                                            {
                                                                CLASS_COURSE_ID = dr[Common.CP_CLASS_COURSE_2017.CLASS_COURSE_ID].ToString(),
                                                                CLASS_ID = dr[Common.CP_CLASS_COURSE_2017.CLASS_ID].ToString(),
                                                                CLASS_NAME = dr[Common.CP_CLASSES_2017.CLASS_NAME].ToString(),
                                                                COURSE_CODE = dr[Common.CP_COURSE_ROOT_2017.COURSE_CODE].ToString(),
                                                                COURSE_ID = dr[Common.CP_CLASS_COURSE_2017.COURSE_ID].ToString(),
                                                                COURSE_TITLE = dr[Common.CP_COURSE_ROOT_2017.COURSE_TITLE].ToString(),
                                                                COURSE_TYPE = dr[Common.CP_COURSE_TYPE_2017.COURSE_TYPE].ToString(),
                                                                IS_ALLIED_SUBJECT = dr[Common.CP_COURSE_ROOT_2017.IS_ALLIED_SUBJECT].ToString(),
                                                                IS_NME_SUBJECT = dr[Common.CP_COURSE_ROOT_2017.IS_NME_SUBJECT].ToString(),
                                                                SEMESTER_NAME = dr[Common.SUP_SEMESTER.SEMESTER_NAME].ToString(),
                                                                SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString()
                                                            }).ToList();

                }
            }
            return View(objAcademicViewModel);
        }
        public JsonResult FetchClassCourseByClassCourseid(string sAcademicYear, string sClassCourse)
        {
            var objJsonClassCourse = new ClassCourseDDL();
            try
            {
                string sCourseId = string.Empty;
                string sClassId = string.Empty;
                AcademicsViewModel objAcademicViewModel = new AcademicsViewModel();
                DataTable dtClassType = new DataTable();
                DataTable dtCourseList = new DataTable();
                DataTable dtClassList = new DataTable();
                DataTable dtClassCourse = new DataTable();

                string sClassType = string.Empty;
                string sClassList = string.Empty;
                string sCourseList = string.Empty;

                dtClassType = SupportDataMethod.FetchClassType().DataSource.Table;
                dtClassList = SupportDataMethod.FetchClass(sAcademicYear).DataSource.Table;

                dtClassCourse = objAcademicViewModel.FetchClassCourseByClassCourseId(sAcademicYear, sClassCourse).DataSource.Table;
                if (dtClassCourse != null && dtClassCourse.Rows.Count > 0)
                {
                    sCourseId = dtClassCourse.Rows[0][Common.CP_CLASS_COURSE_2017.COURSE_ID].ToString();
                    sClassId = dtClassCourse.Rows[0][Common.CP_CLASS_COURSE_2017.CLASS_ID].ToString();
                }
                if (dtClassType != null)
                {
                    foreach (DataRow dr in dtClassType.Rows)
                    {
                        sClassType += "<option value='" + dr[Common.SUP_CLASS_TYPE.CLASS_TYPE_ID] + "' >" + dr[Common.SUP_CLASS_TYPE.CLASS_TYPE] + "</option>";
                    }
                }
                if (dtClassList != null)
                {
                    foreach (DataRow dr in dtClassList.Rows)
                    {
                        sClassList += "<option value='" + dr[Common.CP_CLASSES_2017.CLASS_ID] + "' " + (!string.Equals(sClassId, dr[Common.CP_CLASSES_2017.CLASS_ID].ToString()) ? string.Empty : "selected ") + " >" + dr[Common.CP_CLASSES_2017.CLASS_NAME] + "</option>";
                    }
                }

                objJsonClassCourse.sClassesList = sClassList;

                objJsonClassCourse.sClassType = sClassType;

                var obj = JsonConvert.SerializeObject(objJsonClassCourse);
                return Json(objJsonClassCourse, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AcademicController", "FetchClassCourseByClassCourseid", ex.Message);
                }
            }
            return Json(objJsonClassCourse);
        }
        public JsonResult FetchCourseListByClassId(string sAcademicYear, string sClassId)
        {
            AcademicsViewModel objAcademicViewModel = new AcademicsViewModel();
            var objJson = new ClassCourseDDL();
            string sCourseList = string.Empty;
            DataTable dtCourseList = new DataTable();
            dtCourseList = objAcademicViewModel.FetchCourseListByClassId(sAcademicYear, sClassId).DataSource.Table;

            if (dtCourseList != null && dtCourseList.Rows.Count > 0)

            {
                foreach (DataRow dr in dtCourseList.Rows)
                {
                    sCourseList += "<option value='" + dr[Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID] + "' >" + dr[Common.CP_COURSE_ROOT_2017.COURSE_TITLE] + "</option>";
                }
            }
            objJson.sCourseList = sCourseList;
            return Json(objJson, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Course Structure
        public ActionResult ListCourseStructure()
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    // Fetch Faculty ....
                    var Faculty = (List<SupFaculty>)CMSHandler.FetchData<SupFaculty>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFaculty)).DataSource.Data;
                    if (Faculty != null && Faculty.Count > 0)
                        objViewModel.FacultyList = new SelectList(Faculty, Common.CP_FACULTY_2017.FACULTY_ID, Common.CP_FACULTY_2017.FACULTY);

                    // FetchYear ...
                    var AcdemicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcdemicYear != null && AcdemicYear.Count > 0)
                        objViewModel.AcademicYear = new SelectList(AcdemicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);


                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListCourseStructure", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListCourseStructure", "AcademicsController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindCourseStructureByAcademicYear(string AcYear, string sFacultyId)
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objViewModel.liCourseStructure = (List<CP_COURSE_STRUCTURE>)CMSHandler.FetchData<CP_COURSE_STRUCTURE>(new CP_COURSE_STRUCTURE() { ACADEMIC_YEAR = AcYear, FACULTY_ID = sFacultyId }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseStructureByAcademicYearAndFaculty), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AcademicsController", "BindCourseStructureByAcademicYear", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AcademicsController", "BindCourseStructureByAcademicYear", ex.Message);
                }
            }
            return View(objViewModel);
        }
        // Course Structure For Add / Edit ....
        public ActionResult CourseStructure(string id)
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            try
            {
                string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(sAcademicyear))
                {
                    // Fetch Faculty DDL .....
                    var Faculty = (List<SupFaculty>)CMSHandler.FetchData<SupFaculty>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFaculty)).DataSource.Data;
                    if (Faculty != null && Faculty.Count > 0)
                        objViewModel.FacultyList = new SelectList(Faculty, Common.CP_FACULTY_2017.FACULTY_ID, Common.CP_FACULTY_2017.FACULTY);

                    // Academic Year ....
                    var AcdemicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcdemicYear != null && AcdemicYear.Count > 0)
                    {
                        objViewModel.AcademicYear = new SelectList(AcdemicYear, Common.ACADEMIC_YEAR.AC_YEAR, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    // ProgrammeYear....
                    var ProgrammeYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data;
                    if (ProgrammeYear != null && ProgrammeYear.Count > 0)
                    {
                        objViewModel.ProgrammeYear = new SelectList(ProgrammeYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                    }
                    // Part ....
                    var Part = (List<SUP_PART>)CMSHandler.FetchData<SUP_PART>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchPart)).DataSource.Data;
                    if (Part != null && Part.Count > 0)
                        objViewModel.Part = new SelectList(Part, Common.SUP_PART.PART_ID, Common.SUP_PART.PART);

                    // Semester  ....
                    var Semester = (List<SUP_SEMESTER>)CMSHandler.FetchData<SUP_SEMESTER>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data;
                    if (Semester != null && Semester.Count > 0)
                        objViewModel.Semester = new SelectList(Semester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);

                    // UGC Type ....
                    var liUgcType = (List<CP_UGC_COURSE_TYPE>)CMSHandler.FetchData<CP_UGC_COURSE_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchUGCCoursetype)).DataSource.Data;
                    if (liUgcType != null && liUgcType.Count > 0)
                        objViewModel.UGC_Type = new SelectList(liUgcType, Common.CP_UGC_COURSE_TYPE.UGC_COURSE_TYPE_ID, Common.CP_UGC_COURSE_TYPE.UGC_COURSE_TYPE);

                    // Programme Level ....
                    var Programmelevel = (List<SUP_PROGRAMME_LEVEL>)CMSHandler.FetchData<SUP_PROGRAMME_LEVEL>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel)).DataSource.Data;
                    if (Programmelevel != null && Programmelevel.Count > 0)
                        objViewModel.ProgrammeLevel = new SelectList(Programmelevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);

                    if (!string.IsNullOrEmpty(id))
                    {
                        var liCourseStructure = (List<CP_COURSE_STRUCTURE>)CMSHandler.FetchData<CP_COURSE_STRUCTURE>(new CP_COURSE_STRUCTURE() { COURSE_STRUCTURE_ID = id }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCoursestructureByCoursestructureId)).DataSource.Data;
                        if (liCourseStructure != null && liCourseStructure.Count > 0)
                        {
                            objViewModel.ObjCourseStructure.COURSE_STRUCTURE_ID = liCourseStructure.FirstOrDefault().COURSE_STRUCTURE_ID;
                            objViewModel.ObjCourseStructure.CREDIT = liCourseStructure.FirstOrDefault().CREDIT;
                            objViewModel.ObjCourseStructure.PART_ID = liCourseStructure.FirstOrDefault().PART_ID;
                            objViewModel.ObjCourseStructure.UGC_TYPE_ID = liCourseStructure.FirstOrDefault().UGC_TYPE_ID;
                            objViewModel.ObjCourseStructure.HRS_PER_WEEK = liCourseStructure.FirstOrDefault().HRS_PER_WEEK;
                            objViewModel.ObjCourseStructure.MARKS = liCourseStructure.FirstOrDefault().MARKS;
                            objViewModel.ObjCourseStructure.SEMESTER_ID = liCourseStructure.FirstOrDefault().SEMESTER_ID;
                            objViewModel.ObjCourseStructure.FACULTY_ID = liCourseStructure.FirstOrDefault().FACULTY_ID;
                            objViewModel.ObjCourseStructure.PROGRAMME_LEVEL = liCourseStructure.FirstOrDefault().PROGRAMME_LEVEL;
                            objViewModel.ObjCourseStructure.PROGRAMME_YEAR = liCourseStructure.FirstOrDefault().PROGRAMME_YEAR;
                            objViewModel.ObjCourseStructure.ACADEMIC_YEAR = liCourseStructure.FirstOrDefault().ACADEMIC_YEAR;
                            objViewModel.ObjCourseStructure.IS_OPTIONAL = liCourseStructure.FirstOrDefault().IS_OPTIONAL;
                            objViewModel.ObjCourseStructure.IS_NME = liCourseStructure.FirstOrDefault().IS_NME;
                            objViewModel.ObjCourseStructure.IS_ALLIED = liCourseStructure.FirstOrDefault().IS_ALLIED;
                        }
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CourseStructure", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CourseStructure", "AcademicsController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        // Save Course Structure ...
        public JsonResult SaveCourseStructure(string CourseStructureJsonData)
        {
            JsonResultData ObjJasonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<CP_COURSE_STRUCTURE>(CourseStructureJsonData);
                    //var liExist =(List<CP_COURSE_STRUCTURE>) CMSHandler.FetchData<CP_COURSE_STRUCTURE>(Result,AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.CoursestructureIsExist)).DataSource.Data;
                    if (!string.IsNullOrEmpty(Result.COURSE_STRUCTURE_ID))
                    {
                        var UpdateCourseStructure = CMSHandler.SaveOrUpdate(Result, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.UpdateCourseStructure), sAcademicYear);
                        ObjJasonData.Message = (UpdateCourseStructure.Success) ? Common.ErrorCode.Ok : Common.Messages.FailedToUpdatedRecords;
                    }
                    else
                    {
                        var InsertCourseStructure = CMSHandler.SaveOrUpdate(Result, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.InsertCoursestructure), sAcademicYear);
                        ObjJasonData.Message = (InsertCourseStructure.Success) ? Common.ErrorCode.Ok : Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SaveCourseStructure", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("SaveCourseStructure", "AcademicsController", ex.Message);
                    }
                    ObjJasonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJasonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJasonData, JsonRequestBehavior.AllowGet);
        }
        // Delete Course structure ...
        public JsonResult DeleteCourseStructure(string CourseStructureId)
        {
            JsonResultData ObjJasonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(CourseStructureId))
                {
                    try
                    {
                        var DeleteCourseStructure = CMSHandler.SaveOrUpdate(new CP_COURSE_STRUCTURE() { COURSE_STRUCTURE_ID = CourseStructureId }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.DeleteCoursestructure), sAcademicYear);
                        ObjJasonData.Message = (DeleteCourseStructure.Success) ? Common.ErrorCode.Ok : Common.Messages.FailedToUpdatedRecords;
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("SaveCourseStructure", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("SaveCourseStructure", "AcademicsController", ex.Message);
                        }
                        ObjJasonData.Message = Common.ErrorMessage.InternalServerError;
                    }
                }
                else
                {
                    ObjJasonData.Message = Common.ErrorMessage.BadRequest;
                }
            }
            else
            {
                ObjJasonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJasonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Cousre Pattern
        public ActionResult ListCoursePattern()
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var AcdemicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcdemicYear != null && AcdemicYear.Count > 0)
                        objViewModel.AcademicYear = new SelectList(AcdemicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    var programme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeByApprogramme)).DataSource.Data;
                    if (programme != null && programme.Count > 0)
                        objViewModel.Programme = new SelectList(programme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListCoursePattern", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListCoursePattern", "AcademicsController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindCoursePatternByProgramme(string sAcYear, string sProgramme)
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sActiveSemester = Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString();
                    string sSQL = AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCoursePatternByAcademicYearandProgramme).Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                    objViewModel.liCoursePattern = (List<CP_COURSE_PATTERN>)CMSHandler.FetchData<CP_COURSE_PATTERN>(new CP_COURSE_PATTERN()
                    { ACADEMIC_YEAR = sAcYear, PROGRAMME_GROUP_ID = sProgramme }, sSQL, sAcYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AcademicsController", "BindCoursePatternByProgramme", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AcademicsController", "BindCoursePatternByProgramme", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult AddCoursePattern()
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {

                    var AcdemicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    var programme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeByApprogramme)).DataSource.Data;
                    var Faculty = (List<SupFaculty>)CMSHandler.FetchData<SupFaculty>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFaculty)).DataSource.Data;
                    var Semester = (List<SupSemester>)CMSHandler.FetchData<SupSemester>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data;
                    var ProgrammeLevel = (List<SupProgrammeLevel>)CMSHandler.FetchData<SupProgrammeLevel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel)).DataSource.Data;
                    if (AcdemicYear != null && AcdemicYear.Count > 0)
                        objViewModel.AcademicYear = new SelectList(AcdemicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    if (programme != null && programme.Count > 0)
                        objViewModel.Programme = new SelectList(programme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    if (Semester != null && Semester.Count > 0)
                        objViewModel.Semester = new SelectList(Semester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    if (ProgrammeLevel != null && ProgrammeLevel.Count > 0)
                        objViewModel.ProgrammeLevel = new SelectList(ProgrammeLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
                    if (Faculty != null && Faculty.Count > 0)
                        objViewModel.FacultyList = new SelectList(Faculty, Common.CP_FACULTY_2017.FACULTY_ID, Common.CP_FACULTY_2017.FACULTY);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AddCoursePattern", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AddCoursePattern", "AcademicsController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult FetchCoursePatternByForAddEdit(string sAcYear, string sFacultyId, string sProgramme, string sSemester, string sProgrammeLevel)
        {
            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                AcademicsViewModel objViewModel = new AcademicsViewModel();
                string sSqlCP = string.Empty;
                string sSqlCS = string.Empty;
                try
                {
                    List<CP_COURSE_PATTERN> lstTempCousrePattern = new List<CP_COURSE_PATTERN>();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    sSqlCP = AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCoursePatternForAddEdit).Replace(Common.Delimiter.QUS + Common.CP_COURSE_STRUCTURE.SEMESTER_ID, sSemester);
                    sSqlCS = AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseStructureforCoursePattern).Replace(Common.Delimiter.QUS + Common.CP_COURSE_STRUCTURE.SEMESTER_ID, sSemester);
                    objViewModel.liCoursePattern = (List<CP_COURSE_PATTERN>)CMSHandler.FetchData<CP_COURSE_PATTERN>(new CP_COURSE_PATTERN() { ACADEMIC_YEAR = sAcYear, FACULTY_ID = sFacultyId, PROGRAMME_GROUP_ID = sProgramme, SEMESTER_ID = sSemester, PROGRAMME_LEVEL = sProgrammeLevel }, sSqlCP, sAcademicYear).DataSource.Data;
                    objViewModel.liCourseStructure = (List<CP_COURSE_STRUCTURE>)CMSHandler.FetchData<CP_COURSE_STRUCTURE>(new CP_COURSE_STRUCTURE() { ACADEMIC_YEAR = sAcYear, FACULTY_ID = sFacultyId, SEMESTER_ID = sSemester, PROGRAMME_LEVEL = sProgrammeLevel }, sSqlCS, sAcademicYear).DataSource.Data;

                    string sSQL = AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchAllCourseListForCoursePattern).Replace(Common.Delimiter.QUS + Common.CP_COURSE_STRUCTURE.SEMESTER_ID, sSemester);
                    objViewModel.liCourseRoot = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(null, sSQL).DataSource.Data;
                }
                catch (Exception ex)
                {

                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AcademicsController", "BindCoursePatternByProgramme", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("AcademicsController", "BindCoursePatternByProgramme", ex.Message);
                    }
                }
                return View(objViewModel);
            }
            else
                return RedirectToAction("ErrorMessage", "error");
        }
        public JsonResult SaveCoursePattern(string sJsonSaveCoursePattern)
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    JSONCOURSEPATTERN objJson = serializer.Deserialize<JSONCOURSEPATTERN>(sJsonSaveCoursePattern);
                    string sSQL = string.Empty;
                    if (objJson.JSONlstCP_COURSE_PATTERN != null && objJson.JSONlstCP_COURSE_PATTERN.Count > 0)
                    {
                        foreach (var item in objJson.JSONlstCP_COURSE_PATTERN)
                        {

                            sSQL = SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchCoursePattenAvailableCourses);
                            var liCoursePattern = (List<CP_COURSE_PATTERN>)CMSHandler.FetchData<CP_COURSE_PATTERN>(item, sSQL, sAcademicYear).DataSource.Data;
                            var sArray = item.COURSE_ID.Split(',').ToArray();
                            if (liCoursePattern != null && liCoursePattern.Count > 0)
                            {
                                foreach (var array in sArray)
                                {

                                    var liCourseRoot = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(item,
                                           SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOptionalPaperByCourseId), sAcademicYear).DataSource.Data;
                                    if (liCourseRoot == null)
                                    {
                                        item.IS_OPTIONAL = "1";
                                    }

                                    var IsExist = liCoursePattern.Where(o => o.COURSE_ID == array).ToList();
                                    if (IsExist.Count == 0)
                                    {
                                        // Insert ...
                                        item.COURSE_ID = array;
                                        if (item.COURSE_ID == "0" || item.COURSE_ID == "null")
                                        {
                                            continue;
                                        }
                                        resultArg = CMSHandler.SaveOrUpdate(item, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.InsertCoursePattern), sAcademicYear);
                                        objResult.Message = (resultArg.Success) ? Common.ErrorCode.Ok : Common.Messages.FailedToUpdatedRecords;
                                    }
                                }

                                foreach (var lst in liCoursePattern)
                                {
                                    bool hasItem = sArray.Cast<string>().Any(i => i == lst.COURSE_ID);
                                    if (hasItem == false)
                                    {
                                        // Delete ...
                                        item.COURSE_ID = lst.COURSE_ID;
                                        item.COURSE_PATTERN_ID = lst.COURSE_PATTERN_ID;
                                        resultArg = CMSHandler.SaveOrUpdate(item, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.DeleteCoursePattern), sAcademicYear);
                                        objResult.Message = (resultArg.Success) ? Common.ErrorCode.Ok : Common.Messages.FailedToUpdatedRecords;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var array in sArray)
                                {

                                    // Insert ...
                                    item.COURSE_ID = array;
                                    if (item.COURSE_ID == "0" || item.COURSE_ID == "null")
                                    {
                                        continue;
                                    }
                                    var liCourseRoot = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(item,
                                             SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOptionalPaperByCourseId), sAcademicYear).DataSource.Data;
                                    if (liCourseRoot == null)
                                    {
                                        item.IS_OPTIONAL = "1";
                                    }
                                    resultArg = CMSHandler.SaveOrUpdate(item, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.InsertCoursePattern), sAcademicYear);
                                    objResult.Message = (resultArg.Success) ? Common.ErrorCode.Ok : Common.Messages.FailedToUpdatedRecords;
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(objResult.Message))
                        {
                            objResult.Message = "No Changes Made !";
                        }

                    }
                    else
                    {
                        objResult.Message = Common.ErrorMessage.BadRequest;
                    }
                }
                else
                {
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AcademicsController", "SaveCoursePattern", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AcademicsController", "SaveCoursePattern", ex.Message);
                }
            }

            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        // Department Wise Course Pattern ...,
        public ActionResult CoursePatternEntryForIncharge()
        {
            var ObjViewModel = new AcademicsViewModel();
            try
            {
                var AcdemicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                if (AcdemicYear != null && AcdemicYear.Count > 0)
                    ObjViewModel.AcademicYear = new SelectList(AcdemicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);

                var Semester = (List<SupSemester>)CMSHandler.FetchData<SupSemester>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data;
                if (Semester != null && Semester.Count > 0)
                    ObjViewModel.Semester = new SelectList(Semester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);

                var ProgrammeLevel = (List<SupProgrammeLevel>)CMSHandler.FetchData<SupProgrammeLevel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel)).DataSource.Data;
                if (ProgrammeLevel != null && ProgrammeLevel.Count > 0)
                    ObjViewModel.ProgrammeLevel = new SelectList(ProgrammeLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("CoursePatternEntryForIncharge", "AcademicController", ex.Message);
                }
            }

            return View(ObjViewModel);
        }

        public JsonResult FetchProgrammeByDepartmentAndProgrammelevel(string sProgrammeLevel)
        {
            var ObjJsonData = new JsonResultData();
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sUserId))
            {
                try
                {
                    var liStaff = DapperHandler.FetchAll<STF_PERSONAL_INFO>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), new { STAFF_ID = sUserId });
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        var liProgrammeGroup = DapperHandler.FetchAll<CP_PROGRAMME_GROUP>(AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchProgrammeGroupByApplicationTypeAndDepartmentId), new { APPTYPE_ID = sProgrammeLevel, DEPARTMENT_ID = liStaff.FirstOrDefault().DEPARTMENT });
                        if (liProgrammeGroup != null && liProgrammeGroup.Count > 0)
                        {
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            ObjJsonData.sResult = "<option value='0'> -- Select -- </option>";
                            ObjJsonData.Message = liProgrammeGroup.FirstOrDefault().FACULTY;
                            foreach (var item in liProgrammeGroup)
                            {
                                ObjJsonData.sResult += "<option value='" + item.PROGRAMME_GROUP_ID + "'>" + item.PROGRAMME_NAME + "</option>";
                            }
                        }
                        else
                        {
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        }
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
                        ObjLog.WriteError("FetchProgrammeByDepartmentAndFaculty", "AcademicController", ex.Message);
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
        #endregion

        #region UgcCourseType
        public ActionResult ListUgcCourseType()
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objViewModel.liUgcCourseType = (List<CP_UGC_COURSE_TYPE>)CMSHandler.FetchData<CP_UGC_COURSE_TYPE>(null, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.UgcCourseTypeList), sAcademicYear).DataSource.Data;
                    var liPart = (List<SUP_PART>)CMSHandler.FetchData<SUP_PART>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchPart)).DataSource.Data;
                    var liProgrammeLevel = (List<SUP_PROGRAMME_LEVEL>)CMSHandler.FetchData<SUP_PROGRAMME_LEVEL>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel)).DataSource.Data;
                    if (liPart != null && liPart.Count > 0)
                    {
                        objViewModel.Part = new SelectList(liPart, Common.SUP_PART.PART_ID, Common.SUP_PART.PART);
                    }
                    if (liProgrammeLevel != null && liProgrammeLevel.Count > 0)
                    {
                        objViewModel.ProgrammeLevel = new SelectList(liProgrammeLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AcademicsController", "ListUgcCourseType", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("AcademicsController", "ListUgcCourseType", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(objViewModel);
        }

        public JsonResult FetchUgcCourseTypeById(string sUgcCourseTypeId)
        {
            var liUgcCourseType = new List<CP_UGC_COURSE_TYPE>();
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    liUgcCourseType = (List<CP_UGC_COURSE_TYPE>)CMSHandler.FetchData<CP_UGC_COURSE_TYPE>(new CP_UGC_COURSE_TYPE() { UGC_COURSE_TYPE_ID = sUgcCourseTypeId }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchUgcCourseTypeById), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchUgcCourseTypeById", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchUgcCourseTypeById", "AcademicsController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return Json(liUgcCourseType, JsonRequestBehavior.AllowGet);
        }

        // Save Certificate Courses ...
        public JsonResult SaveUgcCourseType(string JsonSaveUgcCousrseType)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<CP_UGC_COURSE_TYPE>(JsonSaveUgcCousrseType);
                    //var liExist = (List<CP_UGC_COURSE_TYPE>)CMSHandler.FetchData<CP_UGC_COURSE_TYPE>(Result, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.CheckExistUgcCourseType), sAcademicYear).DataSource.Data;
                    if (!string.IsNullOrEmpty(Result.UGC_COURSE_TYPE_ID))
                    {
                        var UpdateUgcCourseType = CMSHandler.SaveOrUpdate(Result, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.UpdateUgcCourseType), sAcademicYear);
                        ObjJsonData.Message = (UpdateUgcCourseType.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    }
                    else
                    {
                        var InsertUgcCourseType = CMSHandler.SaveOrUpdate(Result, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.InsertUgcCourseType), sAcademicYear);
                        ObjJsonData.Message = (InsertUgcCourseType.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SaveUgcCourseType", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("SaveUgcCourseType", "AcademicsController", ex.Message);
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
        public JsonResult DeleteUgcCourseType(string sUgcCourseTypeId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DeleteUgcCourseType = CMSHandler.SaveOrUpdate(new CP_UGC_COURSE_TYPE() { UGC_COURSE_TYPE_ID = sUgcCourseTypeId }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.DeleteUgcCourseType), sAcademicYear);
                    ObjJsonData.Message = (DeleteUgcCourseType.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DeleteUgcCourseType", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DeleteUgcCourseType", "AcademicsController", ex.Message);
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

        // The Next 3 Region Is For CIA Component Group Marks

        #region Course Type
        public ActionResult CourseTypeInfo()
        {
            CourseTypeModel objCourseType = new CourseTypeModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                DataTable dtPart = new DataTable();
                DataTable dtProgrammeLevel = new DataTable();

                dtPart = SupportDataMethod.FetchPart().DataSource.Table;
                dtProgrammeLevel = SupportDataMethod.FetchProgrammeLevel().DataSource.Table;

                var liPart = new List<SUP_PART>();
                var liProgrammeLevel = new List<SupProgrammeLevel>();
                if (dtPart != null && dtPart.Rows.Count > 0)
                {
                    liPart = (from DataRow dr in dtPart.Rows
                              select new SUP_PART()
                              {
                                  PART_ID = dr[Common.SUP_PART.PART_ID].ToString(),
                                  PART = dr[Common.SUP_PART.PART].ToString()
                              }).ToList();
                    objCourseType.PART_ID = new SelectList(liPart, Common.SUP_PART.PART_ID, Common.SUP_PART.PART);
                }
                if (dtProgrammeLevel != null && dtProgrammeLevel.Rows.Count > 0)
                {
                    liProgrammeLevel = (from DataRow dr in dtProgrammeLevel.Rows
                                        select new SupProgrammeLevel()
                                        {
                                            PROGRAMME_LEVEL_ID = dr[Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID].ToString(),
                                            PROGRAMME_LEVEL = dr[Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL].ToString()
                                        }).ToList();
                    objCourseType.PROGRAMME_LEVEL = new SelectList(liProgrammeLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
                }
            }
            return View(objCourseType);
        }
        [UserRights("ADMIN")]
        public ActionResult ListCoursetype()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                AcademicsViewModel objCourseType = new AcademicsViewModel();
                DataTable dtCourseType = new DataTable();
                dtCourseType = objCourseType.FetchCourseTypeInfo(sAcademicYear).DataSource.Table;
                var liCourseType = new List<CourseTypeInfo>();
                if (dtCourseType != null && dtCourseType.Rows.Count > 0)
                {
                    objCourseType.liCourseType = (from DataRow dr in dtCourseType.Rows
                                                  select new CourseTypeInfo()
                                                  {
                                                      COURSE_TYPE_ID = dr[Common.CP_COURSE_TYPE_2017.COURSE_TYPE_ID].ToString(),
                                                      COURSE_TYPE = dr[Common.CP_COURSE_TYPE_2017.COURSE_TYPE].ToString(),
                                                      NO_OF_COMPONENTS = dr[Common.CP_COURSE_TYPE_2017.NO_OF_COMPONENTS].ToString(),
                                                      COURSE_TYPE_ORDER = dr[Common.CP_COURSE_TYPE_2017.COURSE_TYPE_ORDER].ToString(),
                                                      IS_STU_BASED_SELECTION = dr[Common.CP_COURSE_TYPE_2017.IS_STU_BASED_SELECTION].ToString(),
                                                      INTERNAL = dr[Common.CP_COURSE_TYPE_2017.INTERNAL].ToString(),
                                                      EXTERNAL = dr[Common.CP_COURSE_TYPE_2017.EXTERNAL].ToString(),
                                                      TOTAL = dr[Common.CP_COURSE_TYPE_2017.TOTAL].ToString(),
                                                      HOURS = dr[Common.CP_COURSE_TYPE_2017.HOURS].ToString(),
                                                      PART_ID = dr[Common.SUP_PART.PART].ToString(),
                                                      PROGRAMME_LEVEL = dr[Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL].ToString(),
                                                      CREDITS = dr[Common.CP_COURSE_TYPE_2017.CREDITS].ToString(),
                                                      IS_STUDENT_VIEW_CIA_MARK = dr[Common.CP_COURSE_TYPE_2017.IS_STUDENT_VIEW_CIA_MARK].ToString(),
                                                  }).ToList();
                }
                return View(objCourseType);
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
        }
        public JsonResult InsertCourseTypeInfo(string sJsonCourseType)
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                if (sJsonCourseType != null)
                {
                    DataTable dtCourseType = new DataTable();
                    var Result = JsonConvert.DeserializeObject<CourseTypeInfo>(sJsonCourseType);

                    dv.Clear();
                    dv.Add(Common.CP_COURSE_TYPE_2017.COURSE_TYPE, Result.COURSE_TYPE, EnumCommand.DataType.String);
                    using (AcademicsViewModel objCourseType = new AcademicsViewModel())
                    {
                        dtCourseType = objCourseType.CheckCourseType(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                        if (dtCourseType != null && dtCourseType.Rows.Count == 0)
                        {
                            dv.Clear();
                            dv.Add(Common.CP_COURSE_TYPE_2017.COURSE_TYPE, Result.COURSE_TYPE, EnumCommand.DataType.String);
                            dv.Add(Common.CP_COURSE_TYPE_2017.NO_OF_COMPONENTS, Result.NO_OF_COMPONENTS, EnumCommand.DataType.String);
                            dv.Add(Common.CP_COURSE_TYPE_2017.COURSE_TYPE_ORDER, Result.COURSE_TYPE_ORDER, EnumCommand.DataType.String);
                            dv.Add(Common.CP_COURSE_TYPE_2017.IS_STU_BASED_SELECTION, Result.IS_STU_BASED_SELECTION, EnumCommand.DataType.String);
                            dv.Add(Common.CP_COURSE_TYPE_2017.INTERNAL, Result.INTERNAL, EnumCommand.DataType.String);
                            dv.Add(Common.CP_COURSE_TYPE_2017.EXTERNAL, Result.EXTERNAL, EnumCommand.DataType.String);
                            dv.Add(Common.CP_COURSE_TYPE_2017.TOTAL, Result.TOTAL, EnumCommand.DataType.String);
                            dv.Add(Common.CP_COURSE_TYPE_2017.HOURS, Result.HOURS, EnumCommand.DataType.String);
                            dv.Add(Common.CP_COURSE_TYPE_2017.PART_ID, Result.PART_ID, EnumCommand.DataType.String);
                            dv.Add(Common.CP_COURSE_TYPE_2017.PROGRAMME_LEVEL, Result.PROGRAMME_LEVEL, EnumCommand.DataType.String);
                            dv.Add(Common.CP_COURSE_TYPE_2017.CREDITS, Result.CREDITS, EnumCommand.DataType.String);
                            dv.Add(Common.CP_COURSE_TYPE_2017.IS_STUDENT_VIEW_CIA_MARK, Result.IS_STUDENT_VIEW_CIA_MARK, EnumCommand.DataType.String);
                            resultArg = objCourseType.InsertCourseTypeInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                            sResult = resultArg.Success.ToString();
                            if (sResult == "True")
                            {
                                sResult = "Records Saved Successfully";
                            }
                            else
                            {
                                sResult = "Enter Correct Values";
                            }
                        }
                        else
                        {
                            sResult = "Course Type Already Exists...!";
                        }
                    }
                }
                else
                {
                    sResult = Common.ErrorMessage.NoContent;
                }
            }
            else
            {
                sResult = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(sResult);
        }
        public ActionResult EditCourseTypeInfo(int id)
        {
            string sCourseTypeId = Convert.ToString(id);
            Session[Common.SESSION_VARIABLES.COURSE_TYPE_ID] = sCourseTypeId;
            CourseTypeModel objCourseType = new CourseTypeModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                DataTable dtPart = new DataTable();
                DataTable dtProgrammeLevel = new DataTable();

                dtPart = SupportDataMethod.FetchPart().DataSource.Table;
                dtProgrammeLevel = SupportDataMethod.FetchProgrammeLevel().DataSource.Table;

                var liPart = new List<SUP_PART>();
                var liProgrammeLevel = new List<SupProgrammeLevel>();
                if (dtPart != null && dtPart.Rows.Count > 0)
                {
                    liPart = (from DataRow dr in dtPart.Rows
                              select new SUP_PART()
                              {
                                  PART_ID = dr[Common.SUP_PART.PART_ID].ToString(),
                                  PART = dr[Common.SUP_PART.PART].ToString()
                              }).ToList();
                    objCourseType.PART_ID = new SelectList(liPart, Common.SUP_PART.PART_ID, Common.SUP_PART.PART);
                }
                if (dtProgrammeLevel != null && dtProgrammeLevel.Rows.Count > 0)
                {
                    liProgrammeLevel = (from DataRow dr in dtProgrammeLevel.Rows
                                        select new SupProgrammeLevel()
                                        {
                                            PROGRAMME_LEVEL_ID = dr[Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID].ToString(),
                                            PROGRAMME_LEVEL = dr[Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL].ToString()
                                        }).ToList();
                    objCourseType.PROGRAMME_LEVEL = new SelectList(liProgrammeLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
                }
            }
            return View(objCourseType);
        }
        public JsonResult FetchCourseTypeInfoById(string id)
        {
            string sCourseTypeId = Convert.ToString(id);
            Session[Common.SESSION_VARIABLES.COURSE_TYPE_ID] = sCourseTypeId;
            dv.Clear();
            dv.Add(Common.CP_COURSE_TYPE_2017.COURSE_TYPE_ID, sCourseTypeId, EnumCommand.DataType.String);
            //AcademicsViewModel objDepartment = new AcademicsViewModel();
            DataTable dtFetchCourseTypeInfoById = new DataTable();
            using (AcademicsViewModel objCourseType = new AcademicsViewModel())
            {
                dtFetchCourseTypeInfoById = objCourseType.FetchCourseTypeInfoById(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
            }
            if (dtFetchCourseTypeInfoById != null && dtFetchCourseTypeInfoById.Rows.Count > 0)
            {
                CourseTypeInfo objDepartment = new CourseTypeInfo()
                {
                    COURSE_TYPE_ID = dtFetchCourseTypeInfoById.Rows[0][Common.CP_COURSE_TYPE_2017.COURSE_TYPE].ToString(),
                    COURSE_TYPE = dtFetchCourseTypeInfoById.Rows[0][Common.CP_COURSE_TYPE_2017.COURSE_TYPE].ToString(),
                    NO_OF_COMPONENTS = dtFetchCourseTypeInfoById.Rows[0][Common.CP_COURSE_TYPE_2017.NO_OF_COMPONENTS].ToString(),
                    COURSE_TYPE_ORDER = dtFetchCourseTypeInfoById.Rows[0][Common.CP_COURSE_TYPE_2017.COURSE_TYPE_ORDER].ToString(),
                    IS_STU_BASED_SELECTION = dtFetchCourseTypeInfoById.Rows[0][Common.CP_COURSE_TYPE_2017.IS_STU_BASED_SELECTION].ToString(),
                    INTERNAL = dtFetchCourseTypeInfoById.Rows[0][Common.CP_COURSE_TYPE_2017.INTERNAL].ToString(),
                    EXTERNAL = dtFetchCourseTypeInfoById.Rows[0][Common.CP_COURSE_TYPE_2017.EXTERNAL].ToString(),
                    TOTAL = dtFetchCourseTypeInfoById.Rows[0][Common.CP_COURSE_TYPE_2017.TOTAL].ToString(),
                    HOURS = dtFetchCourseTypeInfoById.Rows[0][Common.CP_COURSE_TYPE_2017.HOURS].ToString(),
                    PART_ID = dtFetchCourseTypeInfoById.Rows[0][Common.CP_COURSE_TYPE_2017.PART_ID].ToString(),
                    PROGRAMME_LEVEL = dtFetchCourseTypeInfoById.Rows[0][Common.CP_COURSE_TYPE_2017.PROGRAMME_LEVEL].ToString(),
                    CREDITS = dtFetchCourseTypeInfoById.Rows[0][Common.CP_COURSE_TYPE_2017.CREDITS].ToString(),
                    IS_STUDENT_VIEW_CIA_MARK = dtFetchCourseTypeInfoById.Rows[0][Common.CP_COURSE_TYPE_2017.IS_STUDENT_VIEW_CIA_MARK].ToString(),
                };
                return Json(objDepartment);
            }
            else
            {
                return Json(sResult);
            }

        }
        public JsonResult UpdateCourseTypeInfo(string JsonCourseType)
        {
            var ObjJsonData = new JsonResultData();
            var sCourseType = (Session[Common.SESSION_VARIABLES.COURSE_TYPE_ID] != null) ? Session[Common.SESSION_VARIABLES.COURSE_TYPE_ID].ToString() : string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (JsonCourseType != null)
                    {
                        var Result = JsonConvert.DeserializeObject<CourseTypeInfo>(JsonCourseType);
                        dv.Clear();
                        dv.Add(Common.CP_COURSE_TYPE_2017.COURSE_TYPE_ID, sCourseType, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_TYPE_2017.COURSE_TYPE, Result.COURSE_TYPE, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_TYPE_2017.NO_OF_COMPONENTS, Result.NO_OF_COMPONENTS, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_TYPE_2017.COURSE_TYPE_ORDER, Result.COURSE_TYPE_ORDER, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_TYPE_2017.IS_STU_BASED_SELECTION, Result.IS_STU_BASED_SELECTION, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_TYPE_2017.INTERNAL, Result.INTERNAL, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_TYPE_2017.EXTERNAL, Result.EXTERNAL, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_TYPE_2017.TOTAL, Result.TOTAL, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_TYPE_2017.HOURS, Result.HOURS, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_TYPE_2017.PART_ID, Result.PART_ID, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_TYPE_2017.PROGRAMME_LEVEL, Result.PROGRAMME_LEVEL, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_TYPE_2017.CREDITS, Result.CREDITS, EnumCommand.DataType.String);
                        dv.Add(Common.CP_COURSE_TYPE_2017.IS_STUDENT_VIEW_CIA_MARK, Result.IS_STUDENT_VIEW_CIA_MARK, EnumCommand.DataType.String);
                        using (AcademicsViewModel objDepartment = new AcademicsViewModel())
                        {
                            resultArg = objDepartment.UpdateCourseTypeInfo(dv, sAcademicYear);
                            //sResult = resultArg.Success.ToString();
                            ObjJsonData.Message = (resultArg.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            ObjJsonData.sResult = resultArg.Success.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AcademicsController", "UpdateCourseTypeInfo", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("AcademicsController", "UpdateCourseTypeInfo", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.sResult = "False";
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.sResult = "False";
            }

            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCourseTypeInfo(string id)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    using (AcademicsViewModel objCourseType = new AcademicsViewModel())
                    {
                        dv.Clear();
                        dv.Add(Common.CP_COURSE_TYPE_2017.COURSE_TYPE_ID, id, EnumCommand.DataType.String);
                        resultArg = objCourseType.DeleteCourseTypeInfo(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                        ObjJsonData.Message = (resultArg.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                        ObjJsonData.sResult = resultArg.Success.ToString();
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AcademicsController", "DeleteCourseTypeInfo", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("AcademicsController", "DeleteCourseTypeInfo", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.sResult = "False";
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.sResult = "False";
            }

            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            //if (sResult == "True")
            //{
            //    TempData["DeleteSuccess"] = "Record deleted successfully ...!";
            //}
            //else
            //{
            //    TempData["DeleteError"] = "Record not deleted Try again ...!";
            //}
            //return RedirectToAction("ListCoursetype", "Academics");
        }
        #endregion

        #region CourseTypeGroup
        public ActionResult ListCourseTypeGroup()
        {
            var ObjViewModel = new AcademicsViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.LiCourseTypeGroup = (List<CP_COURSE_TYPE_GROUP>)CMSHandler.FetchData<CP_COURSE_TYPE_GROUP>(null,
                        AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseTypeGroupList), sAcademicYear).DataSource.Data;

                    ObjViewModel.liCourseType = (List<CourseTypeInfo>)CMSHandler.FetchData<CourseTypeInfo>(null,
                            SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseType), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AcademicsController", "ListCourseTypeGroup", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult CourseTypeGroup()
        {
            var ObjViewModel = new AcademicsViewModel();
            CP_COURSE_TYPE_GROUP ObjModel = new CP_COURSE_TYPE_GROUP();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ddlCourseType = (List<CourseTypeModel>)CMSHandler.FetchData<CourseTypeModel>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseType)).DataSource.Data;
                    if (ddlCourseType != null && ddlCourseType.Count > 0)
                    {
                        ObjViewModel.CourseType = new SelectList(ddlCourseType, Common.CP_COURSE_TYPE_2017.COURSE_TYPE_ID, Common.CP_COURSE_TYPE_2017.COURSE_TYPE);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AcademicsController", "CourseTypeGroup", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchCourseTypeGroupById(string sCourseTypeGroupId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liFetchCourseTypeGroup = (List<CP_COURSE_TYPE_GROUP>)CMSHandler.FetchData<CP_COURSE_TYPE_GROUP>(new CP_COURSE_TYPE_GROUP() { GROUP_ID = sCourseTypeGroupId },
                        AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseTypeGroupById), sAcademicYear).DataSource.Data;
                if (liFetchCourseTypeGroup != null && liFetchCourseTypeGroup.Count > 0)
                {
                    return Json(liFetchCourseTypeGroup, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.NoRecordsfound;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchCourseTypeGroupDDL(string sCourseTypeId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            int sCount = 0;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sCourseTypeId))
                {
                    var liCourseTypeGroup = (List<CP_COURSE_TYPE_GROUP>)CMSHandler.FetchData<CP_COURSE_TYPE_GROUP>(new CourseTypeInfo() { COURSE_TYPE_ID = sCourseTypeId },
                          AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseTypeGrupByCourseTypeId), sAcademicYear).DataSource.Data;
                    if (liCourseTypeGroup != null && liCourseTypeGroup.Count > 0 && !string.IsNullOrEmpty(liCourseTypeGroup.FirstOrDefault().NO_OF_COMPONENTS))
                    {
                        ObjJsonData.sResult = "<tbody>";

                        for (var i = 0; i < Convert.ToInt16(liCourseTypeGroup.FirstOrDefault().NO_OF_COMPONENTS); i++)
                        {


                            if (liCourseTypeGroup.Count - 1 >= i)
                            {
                                if (liCourseTypeGroup[i] != null)
                                {
                                    ObjJsonData.sResult += "<tr id='row_" + i + "'>" +
                                      "<td><input class='form-control'  id='" + liCourseTypeGroup[i].GROUP_ID + "'" +
                                 " name='group' placeholder='Enter Group Component' value='" + liCourseTypeGroup[i].CA_GROUP + "' type='text' cms-row-id='row_" + i + "' data-parsley-required='true'></input>" +
                                      "</td>" +
                                "<td><input class='form-control'  id='" + liCourseTypeGroup[i].GROUP_ID + "'  name='mark' " +
                                 "placeholder='Enter Group mark' type=number value='" + liCourseTypeGroup[i].GROUP_MARK + "' type='number' cms-row-id='row_" + i + "' data-parsley-required='true'></input></td>" +
                                "</tr>";
                                }

                            }

                            else
                            {
                                ObjJsonData.sResult += "<tr id='row_" + i + "'>" +
                               "<td><input class='form-control'  id=''" +
                          " name='group' placeholder='Enter Group Component' value='' type='text' cms-row-id='row_" + i + "' data-parsley-required='true'></input>" +
                               "</td>" +
                         "<td><input class='form-control'  id=''  name='mark' " +
                          "placeholder='Enter Group mark' type=number value='' type='number' cms-row-id='row_" + i + "' data-parsley-required='true'></input></td>" +
                         "</tr>";
                            }
                            sCount++;
                        }
                        ObjJsonData.sResult += "</tbody>";
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.BadRequest;

                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;

            }

            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveCourseTypeGroup(string sjsonData)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<List<CP_COURSE_TYPE_GROUP>>(sjsonData);

                    if (Result != null && Result.Count > 0)
                    {
                        foreach (var result in Result)
                        {
                            if (!string.IsNullOrEmpty(result.COURSE_TYPE_ID) && !string.IsNullOrEmpty(result.CA_GROUP))
                            {
                                if (!string.IsNullOrEmpty(result.GROUP_ID))
                                {
                                    var sUpdateCourseTypeGroup = CMSHandler.SaveOrUpdate(result, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.UpdateCourseTypeGroup), sAcademicYear);
                                    ObjJsonData.Message = (sUpdateCourseTypeGroup.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                                }
                                else
                                {
                                    var sSaveCourseTypeGroup = CMSHandler.SaveOrUpdate(result, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.SaveCourseTypeGroup), sAcademicYear);
                                    ObjJsonData.Message = (sSaveCourseTypeGroup.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("AcademicsController", "SaveCourseTypeGroup", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCourseTypeGroup(string sCourseTypeGroupId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region CIAGroupComponent
        public ActionResult ListCIAGroupComponent()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new AcademicsViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var Component = (List<SUP_CIA_COMPONENTS>)CMSHandler.FetchData<SUP_CIA_COMPONENTS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCiaComponent)).DataSource.Data;
                if (Component != null && Component.Count > 0)
                {
                    ObjViewModel.CIAComponent = new SelectList(Component, Common.SUP_CIA_COMPONENTS.COMPONENT_ID, Common.SUP_CIA_COMPONENTS.COMPONENT);
                }
                ObjViewModel.LiCourseTypeGroup = (List<CP_COURSE_TYPE_GROUP>)CMSHandler.FetchData<CP_COURSE_TYPE_GROUP>(null, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.ListCIAGroupComponent)).DataSource.Data;
                var CourseType = (List<CourseTypeInfo>)CMSHandler.FetchData<CourseTypeInfo>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseType)).DataSource.Data;
                if (CourseType != null && CourseType.Count > 0)
                    ObjViewModel.liCourseType = CourseType;
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult FetchCourseTypeGroupDDLandCiaComponent(string sCourseTypeId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sCourseTypeId))
                {
                    var liCourseTypeGroup = (List<CIA_GROUP_COMPONENT>)CMSHandler.FetchData<CIA_GROUP_COMPONENT>(new CourseTypeInfo() { COURSE_TYPE_ID = sCourseTypeId },
                           AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseTypeandCourseGroupandComponentByCourseTypeId), sAcademicYear).DataSource.Data;
                    ObjJsonData.oResult = liCourseTypeGroup;
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.BadRequest;

                }
                var CiaComponent = (List<SUP_CIA_COMPONENTS>)CMSHandler.FetchData<SUP_CIA_COMPONENTS>(null,
                SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCiaComponent)).DataSource.Data;

                if (CiaComponent != null && CiaComponent.Count > 0)
                {
                    ObjJsonData.sResult = "<select class='selectpicker show-menu-arrow show-tick drop-down form-control input-sm'   data-live-search='true' data-parsley-required='true'  name='ciacomponent'><option value='' selected>---Select---</option>";
                    foreach (var item in CiaComponent)
                    {

                        ObjJsonData.sResult += "<option value='" + item.COMPONENT_ID + "'>" + item.COMPONENT + "</option>";
                    }
                    ObjJsonData.sResult += "</select>";
                }

            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;

            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SaveCIAGroupComponent(string sjsonData)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<List<CIA_GROUP_COMPONENT>>(sjsonData);

                    if (Result != null && Result.Count > 0)
                    {
                        foreach (var result in Result)
                        {

                            if (!string.IsNullOrEmpty(result.CIA_GROUP_COMPONENT_ID))
                            {
                                var sUpdateCourseTypeGroup = CMSHandler.SaveOrUpdate(result, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.UpdateCiaGroupComponent), sAcademicYear);
                                ObjJsonData.Message = (sUpdateCourseTypeGroup.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            }
                            else
                            {
                                var sSaveCourseTypeGroup = CMSHandler.SaveOrUpdate(result, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.SaveCiaGroupComponent), sAcademicYear);
                                ObjJsonData.Message = (sSaveCourseTypeGroup.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            }


                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("AcademicsController", "SaveCIAGroupComponent", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult DeletedCIAGroupComponentById(string sGroupComponentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                var sUpdateCourseTypeGroup = CMSHandler.SaveOrUpdate(new CIA_GROUP_COMPONENT() { CIA_GROUP_COMPONENT_ID = sGroupComponentId }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.DeletedCiaGroupComponentById), sAcademicYear);
                ObjJsonData.Message = (sUpdateCourseTypeGroup.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;

            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region ProgrammeGroup
        //List ProgrammeGroup
        public ActionResult ListProgrammeGroup()
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objViewModel.liProgrammeGroup = (List<CP_PROGRAMME_GROUP>)CMSHandler.FetchData<CP_PROGRAMME_GROUP>(null, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchProgrammeGroupList), sAcademicYear).DataSource.Data;
                    var liApplicationType = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType)).DataSource.Data;
                    var liShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    var liProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;
                    var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgram)).DataSource.Data;
                    if (liApplicationType != null && liApplicationType.Count > 0)
                    {
                        objViewModel.ApplicationType = new SelectList(liApplicationType, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                    }
                    if (liShift != null && liShift.Count > 0)
                    {
                        objViewModel.Shift = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    if (liProgrammeMode != null && liProgrammeMode.Count > 0)
                    {
                        objViewModel.ProgrammeMode = new SelectList(liProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                    }
                    if (liProgramme != null && liProgramme.Count > 0)
                    {
                        objViewModel.Programme = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ListProgrammeGroup", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ListProgrammeGroup", "AcademicsController", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(objViewModel);
        }
        public JsonResult LoadProgrammes(string sApplicationTypeId, string sShiftId, string sProgrammeModeId)
        {
            JsonResultData objResult = new JsonResultData();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                CP_PROGRAMME_GROUP objProgrammeGroup = new CP_PROGRAMME_GROUP();
                try
                {
                    objProgrammeGroup.APPTYPE_ID = sApplicationTypeId;
                    objProgrammeGroup.SHIFT = sShiftId;
                    objProgrammeGroup.PROGRAMME_MODE = sProgrammeModeId;
                    var Programme = (List<CP_PROGRAMME_GROUP>)CMSHandler.FetchData<CP_PROGRAMME_GROUP>(objProgrammeGroup, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchSelectedProgrammeForProgrammeGroup), sAcademicYear).DataSource.Data;
                    if (Programme != null)
                    {
                        foreach (var item in Programme)
                        {
                            objResult.sResult += "<option value='" + item.PROGRAMME_ID + "' " + item.STATUS + ">" + item.PROGRAMME_NAME + "</option>";
                        }
                    }
                }
                catch (Exception Ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Academics", "LoadProgrammes", "Err MSg: " + Ex.Message, "Query is empty!");
                        objHandler.WriteError("Academics", "LoadProgrammes", Ex.Message);
                    }
                }
            }
            else
            {
                objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveProgrammeGroup(string sJsonProgrammeGroup)
        {
            JsonResultData objResult = new JsonResultData();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            try
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    CP_PROGRAMME_GROUP objModel = new CP_PROGRAMME_GROUP();
                    var FetchProgrammeGroupId = new List<CP_PROGRAMME_GROUP>();
                    var TempList = new List<CP_PROGRAMME_GROUP>();
                    var obj = JsonConvert.DeserializeObject<List<CP_PROGRAMME_GROUP>>(sJsonProgrammeGroup);
                    foreach (var item in obj)
                    {
                        objModel.APPTYPE_ID = item.APPTYPE_ID;
                        objModel.PROGRAMME_ID = item.PROGRAMME_ID;
                        objModel.ACADEMIC_YEAR = sAcademicYear;
                        objModel.SHIFT = item.SHIFT;
                        objModel.PROGRAMME_MODE = item.PROGRAMME_MODE;
                    }
                    FetchProgrammeGroupId = (List<CP_PROGRAMME_GROUP>)CMSHandler.FetchData<CP_PROGRAMME_GROUP>(objModel, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchProgrammeForRemoveDublicates), sAcademicYear).DataSource.Data;
                    //this loop removes existing main heads 
                    if (FetchProgrammeGroupId != null && FetchProgrammeGroupId.Count() > 0)
                    {
                        if (obj != null && obj.Count > 0)
                        {
                            foreach (var list in FetchProgrammeGroupId)
                            {
                                TempList.Add(obj.SingleOrDefault(S => S.PROGRAMME_ID == list.PROGRAMME_ID && S.APPTYPE_ID == list.APPTYPE_ID && S.PROGRAMME_MODE == list.PROGRAMME_MODE && S.SHIFT == list.SHIFT));
                                obj.Remove(obj.SingleOrDefault(S => S.PROGRAMME_ID == list.PROGRAMME_ID && S.APPTYPE_ID == list.APPTYPE_ID && S.PROGRAMME_MODE == list.PROGRAMME_MODE && S.SHIFT == list.SHIFT));
                            }
                            foreach (var item in TempList)
                            {
                                if (item != null)
                                    FetchProgrammeGroupId.Remove(FetchProgrammeGroupId.SingleOrDefault(S => S.PROGRAMME_ID == item.PROGRAMME_ID && S.APPTYPE_ID == item.APPTYPE_ID && S.PROGRAMME_MODE == item.PROGRAMME_MODE && S.SHIFT == item.SHIFT));
                            }
                            //this loop deletes records
                            foreach (var item in FetchProgrammeGroupId)
                            {
                                item.ACADEMIC_YEAR = sAcademicYear;
                                var o = CMSHandler.SaveOrUpdate(item, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.DeleteProgrammeGroup));
                                if (o.Success)
                                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                            }
                        }
                    }
                    //this loop saves records
                    foreach (var item in obj)
                    {
                        item.ACADEMIC_YEAR = sAcademicYear;
                        var SaveProgrammeGroup = CMSHandler.SaveOrUpdate(item, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.InsertProgrammeGroup));
                        if (SaveProgrammeGroup.Success)
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Academics", "SaveProgrammeGroup", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Academics", "SaveProgrammeGroup", Ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteProgrammeGroup(string sProgrammeGroupId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DeleteUgcCourseType = CMSHandler.SaveOrUpdate(new CP_PROGRAMME_GROUP() { PROGRAMME_GROUP_ID = sProgrammeGroupId }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.DeleteProgrammeGroup), sAcademicYear);
                    ObjJsonData.Message = (DeleteUgcCourseType.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DeleteProgrammeGroup", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DeleteProgrammeGroup", "AcademicsController", ex.Message);
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

        #region Class Incharges  
        /// <summary>
        /// This actin is used to Get Classess and StaffList By DepartMent and Shift
        /// </summary>
        /// <returns>AcademicsViewModel</returns>
        public ActionResult AssignClassIncharges()
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var liDepartment = (List<Department>)CMSHandler.FetchData<Department>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment)).DataSource.Data;

                    var liShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    objViewModel.Shift = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    objViewModel.Department = new SelectList(liDepartment, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AcademicsController", "AssignClassIncharges", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AcademicsController", "AssignClassIncharges", ex.Message);
                }
            }
            return View(objViewModel);
        }
        /// <summary>
        /// This action is used to Bind Classess and StaffList 
        /// </summary>
        /// <param name="department"></param>
        /// <param name="shift"></param>
        /// <returns>AcademicsViewModel</returns>
        public ActionResult BindClassessForAssign(string department, string shift)
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            CP_CLASSES objModel = new CP_CLASSES();
            try
            {
                if (department != null && shift != null)
                {
                    objModel.DEPARTMENT = department;
                    objModel.SHIFT = shift;
                    objViewModel.Staff_List = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new stf_Personal_Info() { DEPARTMENT = objModel.DEPARTMENT }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffByDepartmentId)).DataSource.Data;
                    objViewModel.ClassList = (List<SUP_CLASS>)CMSHandler.FetchData<SUP_CLASS>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByDepartmenIdShifId)).DataSource.Data;
                    objViewModel.liClassInchargeInfo = (List<ClassInchargeInfo>)CMSHandler.FetchData<ClassInchargeInfo>(objModel, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchClassessByDepartmentshift)).DataSource.Data;
                }
            }

            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AcademicsController", "BindClassForAssign", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AcademicsController", "BindClassForAssign", ex.Message);
                }

            }
            return View(objViewModel);
        }
        public JsonResult SaveClassIncharges(string Classincharge)
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            JsonResultData objResultData = new JsonResultData();
            ResultArgs ResultArg = new ResultArgs();
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var classinchargedetails = serializer.Deserialize<JsonList>(Classincharge);
                    CLASS_INCHARGE objModel = new CLASS_INCHARGE();
                    if (classinchargedetails != null)
                    {
                        foreach (var item in classinchargedetails.liClassincharge)
                        {
                            objModel.CLASS_ID = item.CLASS_ID;
                            ResultArg = CMSHandler.SaveOrUpdate(objModel, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.UpdateIsDeletedinClassIncharge));
                            objModel.ACADEMIC_YEAR = sAcademicYear;
                            var staffids = item.STAFF_ID.Split(',');
                            foreach (var staffid in staffids)
                            {
                                objModel.STAFF_ID = staffid;
                                var ClassInchargelist = (List<CLASS_INCHARGE>)CMSHandler.FetchData<CLASS_INCHARGE>(objModel, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchClassInchargeByClassandStaff)).DataSource.Data;
                                if (ClassInchargelist != null && ClassInchargelist.Count > 0)
                                {
                                    objModel.STAFF_ID = ClassInchargelist.FirstOrDefault().STAFF_ID;
                                    ResultArg = CMSHandler.SaveOrUpdate(objModel, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.UpdateClassIncharge));
                                }
                                else
                                {

                                    var liIncharge = (List<CLASS_INCHARGE>)CMSHandler.FetchData<CLASS_INCHARGE>(objModel, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchClassInchargeByClassandStaff)).DataSource.Data;

                                    ResultArg = CMSHandler.SaveOrUpdate(objModel, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.SaveClassIncharges));
                                    objResultData.Message = Common.Messages.RecordsSavedSuccessfully;

                                }


                            }
                        }
                        objResultData.Message = (ResultArg.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;

                    }
                    else
                        objResultData.Message = Common.Messages.FailedToSaveRecords;
                }
                else
                {
                    objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AcademicsController", "SaveClassIncharge", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AcademicsController", "SaveClassIncharge", ex.Message);
                }
                objResultData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region CoursePatternReportByAcademicYearWise

        public ActionResult AcademicWiseCoursePatternReport()
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    //Fetch Academic Year List 
                    var AcdemicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    //Fetch Programme
                    var programme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeByApprogramme)).DataSource.Data;
                    //Fetch Faculty
                    var Faculty = (List<SupFaculty>)CMSHandler.FetchData<SupFaculty>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFaculty)).DataSource.Data;
                    //Fetch Semester
                    var Semester = (List<SupSemester>)CMSHandler.FetchData<SupSemester>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data;
                    //Fetch ProgrammeLevel
                    var ProgrammeLevel = (List<SupProgrammeLevel>)CMSHandler.FetchData<SupProgrammeLevel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel)).DataSource.Data;
                    //Fetch Shift
                    var Shift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    //Fetch ProgrammeMode
                    var ProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;


                    if (AcdemicYear != null && AcdemicYear.Count > 0)
                        objViewModel.AcademicYear = new SelectList(AcdemicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    if (programme != null && programme.Count > 0)
                        objViewModel.Programme = new SelectList(programme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    if (Semester != null && Semester.Count > 0)
                        objViewModel.Semester = new SelectList(Semester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    if (ProgrammeLevel != null && ProgrammeLevel.Count > 0)
                        objViewModel.ProgrammeLevel = new SelectList(ProgrammeLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
                    if (Faculty != null && Faculty.Count > 0)
                        objViewModel.FacultyList = new SelectList(Faculty, Common.CP_FACULTY_2017.FACULTY_ID, Common.CP_FACULTY_2017.FACULTY);
                    if (Shift != null && Shift.Count > 0)
                        objViewModel.Shift = new SelectList(Shift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    if (ProgrammeMode != null && ProgrammeMode.Count > 0)
                        objViewModel.ProgrammeMode = new SelectList(ProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Academics", "CoursePatternReport", ex.Message);
                    }
                }

            }
            return View(objViewModel);
        }
        /// <summary>
        /// Fetch Programme by ProgrammeMode and shift
        /// </summary>
        /// <param name="sShift"></param>
        /// <param name="sProgrammeMode"></param>
        /// <returns></returns>
        public JsonResult FetchProgrammeByProgrammeModeandShift(string sShift, string sProgrammeMode)
        {
            var reportFieldsForCaste = new List<REPORT_FIELDS_FORCASTEWISE>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    REPORT_FIELDS_FORCASTEWISE reportFields = new REPORT_FIELDS_FORCASTEWISE();
                    reportFields.SHIFT = sShift;
                    reportFields.PROGRAMME_MODE = sProgrammeMode;
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    //Fetch Programme
                    reportFieldsForCaste = (List<REPORT_FIELDS_FORCASTEWISE>)CMSHandler.FetchData<REPORT_FIELDS_FORCASTEWISE>(reportFields, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupByShiftAndProgrammeMode), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Academics", "FetchProgrammeByProgrammeModeandShift", ex.Message);
                    }

                }
            }
            return Json(reportFieldsForCaste, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Bind AcademicWise Course Pattern Report
        /// </summary>
        /// <param name="sAcademic"></param>
        /// <param name="sFaculty"></param>
        /// <param name="sProgrammeLevel"></param>
        /// <param name="sProgramme"></param>
        /// <param name="sSemester"></param>
        /// <returns></returns>
        public ActionResult BindAcademicWiseCoursePatternReport(string sAcademic, string sFaculty, string sProgrammeLevel, string sProgramme, string sSemester)
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sSql = SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCoursePatternReportByAcademicWise);
                    objViewModel.liCoursePatternDetails = (List<COURSEPATTERN_REPORT_DETAILS>)CMSHandler.FetchData<COURSEPATTERN_REPORT_DETAILS>(null, sSql.Replace(Common.Delimiter.QUS + Common.COURSEPATTERN_REPORT_FIELDS.ACADEMIC_YEAR, sAcademic)
                     .Replace(Common.Delimiter.QUS + Common.COURSEPATTERN_REPORT_FIELDS.FACULTY_ID, sFaculty).Replace(Common.Delimiter.QUS + Common.COURSEPATTERN_REPORT_FIELDS.PROGRAMME_LEVEL, sProgrammeLevel).Replace(Common.Delimiter.QUS + Common.COURSEPATTERN_REPORT_FIELDS.PROGRAMME_GROUP_ID, sProgramme)
                     .Replace(Common.Delimiter.QUS + Common.COURSEPATTERN_REPORT_FIELDS.SEMESTER_ID, sSemester), sAcademicYear).DataSource.Data;
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Academics", "BindAcademicWiseCoursePatternReport", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }




        #endregion

        #region BulkImportForCourseStructureAndCoursePattern
        public ActionResult BulkImportForCoursePattern()
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    var AcdemicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcdemicYear != null && AcdemicYear.Count > 0)
                        objViewModel.AcademicYear = new SelectList(AcdemicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Academics", "BindAcademicWiseCoursePatternReport", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }
        public JsonResult BindBulkImportForCoursePattern(string sFromAcademicYear, string sToAcademicYear)
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            JsonResultData objResult = new JsonResultData();
            CP_COURSE_PATTERN ObjModel = new CP_COURSE_PATTERN();
            string Ssql;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    var CourseStructure = (List<CP_COURSE_STRUCTURE>)CMSHandler.FetchData<CP_COURSE_STRUCTURE>(null, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseStructureByAcademicYear), sFromAcademicYear).DataSource.Data;
                    var CheckIsExistCoursepattern = (List<CP_COURSE_PATTERN>)CMSHandler.FetchData<CP_COURSE_PATTERN>(null, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.CheckIsExistCoursepatternByAcademicyear), sToAcademicYear).DataSource.Data;
                    if (CourseStructure != null && CourseStructure.Count > 0 && CheckIsExistCoursepattern == null)
                    {
                        foreach (var item in CourseStructure)
                        {
                            Ssql = string.Empty;
                            item.ACADEMIC_YEAR = sToAcademicYear;
                            var InsertCourseStructure = CMSHandler.SaveOrUpdate(item, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.InsertCoursestructure), sFromAcademicYear, true);
                            if (InsertCourseStructure.Success)
                            {
                                Ssql = AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.SelectInsertCoursepatternByCoursestructureId).Replace(Common.Delimiter.AT + Common.CP_COURSE_PATTERN.COURSE_STRUCTURE_ID, InsertCourseStructure.RowUniqueId.ToString());
                                ObjModel.COURSE_STRUCTURE_ID = item.COURSE_STRUCTURE_ID;
                                ObjModel.ACADEMIC_YEAR = sFromAcademicYear;
                                var InsertCoursepatternByCourseStructureId = CMSHandler.SaveOrUpdate(ObjModel, Ssql, sToAcademicYear);
                                if (InsertCoursepatternByCourseStructureId.Success)
                                {
                                    objResult.ErrorCode = Common.ErrorCode.Accepted;
                                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    objResult.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                        objResult.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Academics", "BindAcademicWiseCoursePatternReport", ex.Message);
                    }
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #endregion\

        #region For Rescapes

        public ActionResult ListRescapesClassCourseStaff()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new AcademicsViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.AcademicYear = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    var liProgramList = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeByApprogramme)).DataSource.Data;
                    if (liProgramList != null && liProgramList.Count > 0)
                    {
                        ObjViewModel.Programme = new SelectList(liProgramList, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    }

                    var liCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new CP_COURSE_ROOT() { UGC_COURSE_TYPE = "104" }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByUgcTypeId)).DataSource.Data;
                    if (liCourse != null && liCourse.Count > 0)
                    {
                        ObjViewModel.CourseList = new SelectList(liCourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_TITLE);
                    }

                    var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses)).DataSource.Data;
                    if (liClass != null && liClass.Count > 0)
                    {
                        ObjViewModel.liClass = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }

                    var liStaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff)).DataSource.Data;
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        ObjViewModel.Staff = new SelectList(liStaff, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AcademicController", "ListRescapesClassCourseStaff", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult ClassCourseStaffForRescapes(string sAcYear)
        {
            AcademicsViewModel ObjViewModel = new AcademicsViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liClassCourseStaffForRescapes = (List<CLASSCOURSESTAFFFORRESCAPES>)CMSHandler.FetchData<CLASSCOURSESTAFFFORRESCAPES>(null, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.ListClassCourseStaffForRescapes), sAcYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ClassCourseStaffForRescapes", "AcademicsController", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult FetchRescapesClassCourseStaffById(string sClassWiseStaffId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liFetchRescapesClassCourseStaff = (List<CLASSCOURSESTAFFFORRESCAPES>)CMSHandler.FetchData<CLASSCOURSESTAFFFORRESCAPES>(new CLASSCOURSESTAFFFORRESCAPES() { CLS_CRSE_STF_ID = sClassWiseStaffId }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchClassCourseStaffForRescapesById)).DataSource.Data;
                    if (liFetchRescapesClassCourseStaff != null && liFetchRescapesClassCourseStaff.Count > 0)
                    {
                        return Json(liFetchRescapesClassCourseStaff, JsonRequestBehavior.AllowGet);
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
                        ObjLog.WriteError("AcademicsController", "FetchRescapesClassCourseStaffById", ex.Message);
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

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveRescapesClassCourseStaff(string sClassCourseStaffId, string sAcademicYearId, string sClassId, string sCourseId, string sStaffId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new CLASSCOURSESTAFFFORRESCAPES();
                    ObjModel.CLS_CRSE_STF_ID = sClassCourseStaffId;
                    ObjModel.CLASS_ID = sClassId;
                    ObjModel.COURSE_ID = sCourseId;
                    ObjModel.STAFF_ID = sStaffId;
                    ObjModel.ACADEMIC_YEAR = sAcademicYearId;

                    if (!string.IsNullOrEmpty(sClassCourseStaffId))
                    {
                        // Update ....
                        var Update = CMSHandler.SaveOrUpdate(ObjModel, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.UpdateClassCourseStaffForRescapes));
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
                        var IsExist = (List<CLASSCOURSESTAFFFORRESCAPES>)CMSHandler.FetchData<CLASSCOURSESTAFFFORRESCAPES>(ObjModel, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.IsExistClassCourseStaffForRescapes)).DataSource.Data;
                        if (IsExist != null && IsExist.Count > 0)
                        {
                            ObjJsonData.Message = "This Record Already Exist !";
                            ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        }
                        else
                        {
                            // Save Data ...,
                            var Save = CMSHandler.SaveOrUpdate(ObjModel, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.SaveClassCourseStaffForRescapes));
                            if (Save.Success && Save.RowsAffected != 0)
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
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AcademicsController", "SaveRescapesClassCourseStaff", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteRescapesClassCourseStaff(string sClassWiseStaffId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Delete = CMSHandler.SaveOrUpdate(new CLASSCOURSESTAFFFORRESCAPES() { CLS_CRSE_STF_ID = sClassWiseStaffId }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.DeleteClassCourseStaffForRescapes));
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
                        ObjLog.WriteError("AcademicsController", "DeleteRescapesClassCourseStaff", ex.Message);
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

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region All Course Syllabus For Student
        public ActionResult SyllabusForStudents()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AcademicsViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liStuClass = (List<STU_CLASS>)CMSHandler.FetchData<STU_CLASS>(new STU_CLASS() { STUDENT_ID = sUserId },
                        AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchAllStudiedClassAndYearByUserId), sAcademicYear).DataSource.Data;
                    var liCourse = new List<COURSEPATTERN_REPORT_DETAILS>();
                    if (liStuClass != null && liStuClass.Count > 0)
                    {
                        foreach (var item in liStuClass)
                        {
                            var liRegularCourses = (List<COURSEPATTERN_REPORT_DETAILS>)CMSHandler.FetchData<COURSEPATTERN_REPORT_DETAILS>(item,
                                    AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchRegularSyllabusForStudentByUserId), item.ACADEMIC_YEAR).DataSource.Data;
                            var liOptionalCourses = (List<COURSEPATTERN_REPORT_DETAILS>)CMSHandler.FetchData<COURSEPATTERN_REPORT_DETAILS>(item,
                                    AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchOptionalSyllabusForStudentByUserId), item.ACADEMIC_YEAR).DataSource.Data;

                            if (liOptionalCourses != null && liOptionalCourses.Count > 0)
                            {
                                foreach (var Courseitem in liOptionalCourses)
                                {
                                    var IsExist = liRegularCourses.Where(o => o.COURSE_ROOT_ID == Courseitem.COURSE_ROOT_ID).ToList();
                                    if (IsExist != null && IsExist.Count > 0)
                                    {
                                        liRegularCourses.RemoveAll(o => o.COURSE_STRUCTURE_ID == IsExist.FirstOrDefault().COURSE_STRUCTURE_ID);
                                        Courseitem.COURSE_STRUCTURE_ID = IsExist.FirstOrDefault().COURSE_STRUCTURE_ID;
                                        liRegularCourses.Add(Courseitem);
                                    }
                                }
                            }
                            liCourse = (liCourse != null && liCourse.Count > 0) ? liCourse.Concat(liRegularCourses).ToList() : liRegularCourses;
                        }
                        ObjViewModel.liCourse = liCourse.OrderBy(x => x.SEMESTER_ID).ThenBy(x => x.PART).ToList();
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AcademicsController", "SyllabusForStudents", ex.Message);
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

        #region Course Syllabus For Teaching Staff
        public ActionResult SyllabusForCourseStaff()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AcademicsViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liCourse = (List<COURSEPATTERN_REPORT_DETAILS>)CMSHandler.FetchData<COURSEPATTERN_REPORT_DETAILS>(new STF_PERSONAL_INFO() { STAFF_ID = sUserId },
                        AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchSyllabusForCourseStaffByUserId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AcademicsController", "SyllabusForCourseStaff", ex.Message);
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

        #region Course Syllabus For HOD
        public ActionResult SyllabusForHOD()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AcademicsViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liCourse = (List<COURSEPATTERN_REPORT_DETAILS>)CMSHandler.FetchData<COURSEPATTERN_REPORT_DETAILS>(new STF_PERSONAL_INFO() { STAFF_ID = sUserId },
                        AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchSyllabusForHODByUserId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AcademicsController", "SyllabusForHOD", ex.Message);
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

        #region AcademicsCrossVerification

        public ActionResult AcademicsCrossverification()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new AcademicsViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                    if (liProgramme != null && liProgramme.Count > 0)
                    {
                        ObjViewModel.Programme = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    }

                    var SemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
                    if (SemesterType != null && SemesterType.Count > 0)
                    {
                        ObjViewModel.liSemesterType = new SelectList(SemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AcademicsController", "AcademicsCrossverification", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }

            return View(ObjViewModel);
        }

        public ActionResult BindAcademicsCrossverification(string sProgrammeGroupId, string sSemesterType)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new AcademicsViewModel();
            var LiClassCourse = new List<ClassCourse>();
            var LiClassCourseStaff = new List<CP_CLASS_COURSE_STAFF>();
            var LiStudents = new List<ListStudents>();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var LiRegularclasses = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(new CP_CLASSES { PROGRAMME_GROUP_ID = sProgrammeGroupId, CLASS_TYPE = Common.SupClassType.Regular }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchRegularClassByProgrammeGroupId)).DataSource.Data;

                    // Class Course checking
                    // Find Course doesn't assign in Class course
                    if (LiRegularclasses != null && LiRegularclasses.Count > 0)
                    {
                        foreach (var item in LiRegularclasses)
                        {
                            var liCoursePatternRegular = (List<CP_COURSE_PATTERN>)CMSHandler.FetchData<CP_COURSE_PATTERN>(new CP_COURSE_PATTERN { PROGRAMME_GROUP_ID = sProgrammeGroupId, CLASS_YEAR = item.CLASS_YEAR }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchRegularCourseByProgrammeGroupIdandClassYearId), sAcademicYear).DataSource.Data;
                            var liclassCourse = (List<ClassCourse>)CMSHandler.FetchData<ClassCourse>(new ClassCourse { CLASS_ID = item.CLASS_ID }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchClassCourseList), sAcademicYear).DataSource.Data;
                            if (liCoursePatternRegular != null && liCoursePatternRegular.Count > 0)
                            {
                                foreach (var CPitem in liCoursePatternRegular)
                                {
                                    var course = liclassCourse.Where(O => O.COURSE_CODE == CPitem.COURSE_CODE).ToList();
                                    if (course != null && course.Count > 0)
                                    {
                                        liclassCourse.RemoveAll(o => o.COURSE_CODE == CPitem.COURSE_CODE);
                                    }
                                    else
                                    {
                                        LiClassCourse.Add(new ClassCourse { CLASS_ID = item.CLASS_ID, CLASS_NAME = item.CLASS_NAME, COURSE_CODE = CPitem.COURSE_CODE, COURSE_TITLE = CPitem.COURSE_TITLE, STATUS = "1" });
                                    }
                                }
                                if (liclassCourse != null && liclassCourse.Count > 0)
                                {
                                    foreach (var citem in liclassCourse)
                                    {
                                        LiClassCourse.Add(new ClassCourse { CLASS_ID = item.CLASS_ID, CLASS_NAME = item.CLASS_NAME, COURSE_CODE = citem.COURSE_CODE, COURSE_TITLE = citem.COURSE_TITLE, STATUS = "2" });
                                    }
                                }
                            }
                        }
                    }
                    if (LiRegularclasses != null && LiRegularclasses.Count > 0)
                    {
                        foreach (var item in LiRegularclasses)
                        {
                            var liCoursePatternForClass = (List<CP_COURSE_PATTERN>)CMSHandler.FetchData<CP_COURSE_PATTERN>(new CP_COURSE_PATTERN { PROGRAMME_GROUP_ID = sProgrammeGroupId, CLASS_YEAR = item.CLASS_YEAR }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseByProgrammeGroupIdandClassYearId), sAcademicYear).DataSource.Data;
                            var LiClassCourseStaffbyclass = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(new CP_CLASS_COURSE_STAFF { CLASS_ID = item.CLASS_ID, SEMESTER_TYPE_ID = sSemesterType }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchClassCourseStaffByGroupCourseIdClassId), sAcademicYear).DataSource.Data;
                            if (liCoursePatternForClass != null && liCoursePatternForClass.Count > 0)
                            {
                                if (LiClassCourseStaffbyclass != null && LiClassCourseStaffbyclass.Count > 0)
                                {
                                    foreach (var cpitem in liCoursePatternForClass)
                                    {
                                        var ClassCourseStaff = LiClassCourseStaffbyclass.Where(o => o.COURSE_ID == cpitem.COURSE_ID).ToList();
                                        if (ClassCourseStaff != null && ClassCourseStaff.Count > 0)
                                        {
                                            LiClassCourseStaffbyclass.RemoveAll(o => o.COURSE_CODE == cpitem.COURSE_CODE);
                                        }
                                        else if (cpitem.IS_COMPULSORY != "1")
                                        {
                                            var CheckForOptionalCourseStaff = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(new CP_CLASS_COURSE_STAFF { COURSE_ROOT_ID = cpitem.COURSE_ID, SHIFT = item.SHIFT }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchClassCourseStaffByOptionClass), sAcademicYear).DataSource.Data;
                                            if (CheckForOptionalCourseStaff != null && CheckForOptionalCourseStaff.Count > 0)
                                            {
                                                if (!string.IsNullOrEmpty(CheckForOptionalCourseStaff.FirstOrDefault().STAFF_ID))
                                                {
                                                    // have to remove
                                                }
                                                else
                                                {
                                                    LiClassCourseStaff.Add(new CP_CLASS_COURSE_STAFF { CLASS_ID = item.CLASS_ID, CLASS_NAME = item.CLASS_NAME, COURSE_CODE = cpitem.COURSE_CODE, COURSE_TITLE = cpitem.COURSE_TITLE, STATUS = "1" });
                                                }
                                            }
                                            else
                                            {
                                                LiClassCourseStaff.Add(new CP_CLASS_COURSE_STAFF { CLASS_ID = item.CLASS_ID, CLASS_NAME = item.CLASS_NAME, COURSE_CODE = cpitem.COURSE_CODE, COURSE_TITLE = cpitem.COURSE_TITLE, STATUS = "1" });
                                            }
                                        }
                                        else
                                        {
                                            LiClassCourseStaff.Add(new CP_CLASS_COURSE_STAFF { CLASS_ID = item.CLASS_ID, CLASS_NAME = item.CLASS_NAME, COURSE_CODE = cpitem.COURSE_CODE, COURSE_TITLE = cpitem.COURSE_TITLE, STATUS = "1" });
                                        }
                                    }
                                    if (LiClassCourseStaffbyclass != null && LiClassCourseStaffbyclass.Count > 0)
                                    {
                                        foreach (var citem in LiClassCourseStaffbyclass)
                                        {
                                            LiClassCourseStaff.Add(new CP_CLASS_COURSE_STAFF { CLASS_ID = item.CLASS_ID, CLASS_NAME = item.CLASS_NAME, COURSE_CODE = citem.COURSE_CODE, COURSE_TITLE = citem.COURSE_TITLE, STATUS = "2" });
                                        }
                                    }
                                }

                            }
                        }
                    }
                    // Class Course staff checking
                    // OptionalCourseStuentChecking
                    //Doesnot in StuCourseInfo
                    if (LiRegularclasses != null && LiRegularclasses.Count > 0)
                    {
                        foreach (var Rcitem in LiRegularclasses)
                        {
                            var LiOptionalCourseStructure = (List<CP_COURSE_PATTERN>)CMSHandler.FetchData<CP_COURSE_PATTERN>(new CP_COURSE_PATTERN { PROGRAMME_GROUP_ID = sProgrammeGroupId, CLASS_YEAR = Rcitem.CLASS_YEAR }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchOptionalCourseStructureByProgrammeGroupIdandClassYearId), sAcademicYear).DataSource.Data;
                            if (LiOptionalCourseStructure != null && LiOptionalCourseStructure.Count > 0)
                            {
                                foreach (var csItem in LiOptionalCourseStructure)
                                {
                                    var LiDosenotInStuCourseInfo = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(new CP_COURSE_PATTERN { COURSE_STRUCTURE_ID = csItem.COURSE_STRUCTURE_ID, CLASS_ID = Rcitem.CLASS_ID }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchStudentListDoesntAssingnOptionalCourseByCourseStructureByClass), sAcademicYear).DataSource.Data;
                                    if (LiDosenotInStuCourseInfo != null && LiDosenotInStuCourseInfo.Count > 0)
                                    {
                                        foreach (var stitem in LiDosenotInStuCourseInfo)
                                        {
                                            LiStudents.Add(new ListStudents { STUDENT_ID = stitem.STUDENT_ID, ROLL_NO = stitem.ROLL_NO, REGISTER_NO = stitem.REGISTER_NO, FIRST_NAME = stitem.FIRST_NAME, CLASS_NAME = stitem.CLASS_NAME, COURSE_TITLE = csItem.UGC_COURSE_TYPE, STATUS = "1" });
                                        }
                                    }
                                    var LiDublicateInStuCourseInfo = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(new CP_COURSE_PATTERN { COURSE_STRUCTURE_ID = csItem.COURSE_STRUCTURE_ID, CLASS_ID = Rcitem.CLASS_ID }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchDublicateStudentOptionalCourseByCourseStructureByClass), sAcademicYear).DataSource.Data;
                                    if (LiDublicateInStuCourseInfo != null && LiDublicateInStuCourseInfo.Count > 0)
                                    {
                                        foreach (var stitem in LiDublicateInStuCourseInfo)
                                        {
                                            if (stitem.STUDENT_COUNT != "1")
                                            {
                                                LiStudents.Add(new ListStudents { STUDENT_ID = stitem.STUDENT_ID, ROLL_NO = stitem.ROLL_NO, REGISTER_NO = stitem.REGISTER_NO, FIRST_NAME = stitem.FIRST_NAME, CLASS_NAME = stitem.CLASS_NAME, COURSE_CODE = stitem.COURSE_CODE, STATUS = "2" });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    ObjViewModel.ClassCourseList = LiClassCourse;
                    ObjViewModel.LiCpClassCourseStaff = LiClassCourseStaff;
                    ObjViewModel.LiStudentList = LiStudents;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AcademicsController", "AcademicsCrossverification", ex.Message);
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

        #region CourseRoot Update For COE
        public ActionResult ListCourseRootForCoe(string sId)
        {

            string sAcademicYear = string.Empty;
            sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    AcademicsViewModel objViewModel = new AcademicsViewModel();

                    var AcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcademicYear != null && AcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYear = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    var lidepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    if (lidepartment != null && lidepartment.Count > 0)
                    {
                        objViewModel.Department = new SelectList(lidepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);

                    }


                    // Semeser ...
                    var liSemester = (List<SUP_SEMESTER>)CMSHandler.FetchData<SUP_SEMESTER>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data;
                    if (liSemester != null && liSemester.Count > 0)
                    {
                        objViewModel.Semester = new SelectList(liSemester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    }

                    if (!string.IsNullOrEmpty(sId))
                    {
                        var liCourseRoot = (List<CourseRootInfo>)CMSHandler.FetchData<CourseRootInfo>(new CourseRootInfo() { COURSE_ROOT_ID = sId },
                       SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseInfoByIdForCoe), sAcademicYear).DataSource.Data;


                        if (liCourseRoot != null && liCourseRoot.Count > 0)
                        {
                            objViewModel.acYear = liCourseRoot.FirstOrDefault().ACADEMIC_YEAR_ID;
                            objViewModel.sDept = liCourseRoot.FirstOrDefault().DEPARTMENT;
                            objViewModel.sSemester = liCourseRoot.FirstOrDefault().SEMESTER;
                        }
                    }

                    return View(objViewModel);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StudentController", "ListCourseRootForCoe", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StudentController", "ListCourseRootForCoe", ex.Message);
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


        public ActionResult ListCourseInfoByAcademicAndDepartmentAndSemesterType(string sAcademicYear, string sDept, string sSemester)
        {
            AcademicsViewModel objCourseRootView = new AcademicsViewModel();
            // IList<CourseRootInfo> liCourseRoot = new List<CourseRootInfo>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var liCourseRoot = (List<CourseRootInfo>)CMSHandler.FetchData<CourseRootInfo>(new CourseRootInfo() { DEPARTMENT = sDept, SEMESTER = sSemester },
                        SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseRootInfoByAcYearAndDepartmentAndSemesterType), sAcademicYear).DataSource.Data;

                    if (liCourseRoot != null && liCourseRoot.Count > 0)
                    {
                        objCourseRootView.liCourseRootList = liCourseRoot;
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListCourseInfoByAcademicAndDepartmentAndSemesterType", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListCourseInfoByAcademicAndDepartmentAndSemesterType", "AcademicsController", ex.Message);
                }
            }
            return View(objCourseRootView);
        }

        public ActionResult EditCourseRootInfoForCoe(string id)
        {
            CourseRootModel objCourseRoot = new CourseRootModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    dv.Clear();
                    dv.Add(Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, id, EnumCommand.DataType.String);
                    DataTable dtFetchCourseRootById = new DataTable();
                    Session[Common.SESSION_VARIABLES.COURSE_ROOT_ID] = id;
                    DataTable dtDepartment = new DataTable();
                    DataTable dtPart = new DataTable();
                    DataTable dtCourseType = new DataTable();
                    DataTable dtPaperCode = new DataTable();
                    DataTable dtSemester = new DataTable();
                    DataTable dtUGCCourseType = new DataTable();
                    DataTable dtPaperType = new DataTable();
                    DataTable dtSubjectType = new DataTable();

                    dtDepartment = SupportDataMethod.FetchDepartment((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    dtPart = SupportDataMethod.FetchPart().DataSource.Table;
                    dtCourseType = SupportDataMethod.FetchCourseItems((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                    dtPaperCode = SupportDataMethod.FetchPaperCode().DataSource.Table;
                    dtSemester = SupportDataMethod.FetchSemester().DataSource.Table;
                    dtUGCCourseType = SupportDataMethod.FetchUGCCoursetype().DataSource.Table;
                    dtPaperType = SupportDataMethod.FetchPaperType().DataSource.Table;
                    dtSubjectType = SupportDataMethod.FetchSubjectType().DataSource.Table;

                    List<SupDepartment> liDepartment = new List<SupDepartment>();
                    List<SUP_PART> liPart = new List<SUP_PART>();
                    List<SupCourseType> liCourseType = new List<SupCourseType>();
                    List<SupPaperCode> liPaperCode = new List<SupPaperCode>();
                    List<SupSemester> liSemester = new List<SupSemester>();
                    List<SupUGCCourseType> liUGCCourseType = new List<SupUGCCourseType>();
                    List<SupPaperType> liPaperType = new List<SupPaperType>();
                    List<SUP_SUBJECT_TYPE> liSupbjectType = new List<SUP_SUBJECT_TYPE>();

                    if (dtDepartment != null && dtDepartment.Rows.Count > 0)
                    {
                        liDepartment = (from DataRow dr in dtDepartment.Rows
                                        select new SupDepartment()
                                        {
                                            DEPARTMENT_ID = dr[Common.SUP_DEPARTMENT.DEPARTMENT_ID].ToString(),
                                            DEPARTMENT = dr[Common.SUP_DEPARTMENT.DEPARTMENT].ToString()
                                        }).ToList();
                        objCourseRoot.DEPARTMENT = new SelectList(liDepartment, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                    }

                    if (dtPart != null && dtPart.Rows.Count > 0)
                    {
                        liPart = (from DataRow dr in dtPart.Rows
                                  select new SUP_PART()
                                  {
                                      PART_ID = dr[Common.SUP_PART.PART_ID].ToString(),
                                      PART = dr[Common.SUP_PART.PART].ToString()
                                  }).ToList();
                        objCourseRoot.PART = new SelectList(liPart, Common.SUP_PART.PART_ID, Common.SUP_PART.PART);
                    }
                    if (dtCourseType != null && dtCourseType.Rows.Count > 0)
                    {
                        liCourseType = (from DataRow dr in dtCourseType.Rows
                                        select new SupCourseType()
                                        {
                                            COURSE_TYPE_ID = dr[Common.CP_COURSE_TYPE_2017.COURSE_TYPE_ID].ToString(),
                                            COURSE_TYPE = dr[Common.CP_COURSE_TYPE_2017.COURSE_TYPE].ToString()
                                        }).ToList();
                        objCourseRoot.COURSE_TYPE = new SelectList(liCourseType, Common.CP_COURSE_TYPE_2017.COURSE_TYPE_ID, Common.CP_COURSE_TYPE_2017.COURSE_TYPE);
                    }

                    if (dtPaperCode != null && dtPaperCode.Rows.Count > 0)
                    {
                        liPaperCode = (from DataRow dr in dtPaperCode.Rows
                                       select new SupPaperCode()
                                       {
                                           PAPER_CODE_ID = dr[Common.SUP_PAPER_CODE.PAPER_CODE_ID].ToString(),
                                           PAPER_CODE = dr[Common.SUP_PAPER_CODE.PAPER_CODE].ToString()
                                       }).ToList();
                        objCourseRoot.PAPER_CODE = new SelectList(liPaperCode, Common.SUP_PAPER_CODE.PAPER_CODE_ID, Common.SUP_PAPER_CODE.PAPER_CODE);
                    }
                    if (dtSemester != null && dtSemester.Rows.Count > 0)
                    {
                        liSemester = (from DataRow dr in dtSemester.Rows
                                      select new SupSemester()
                                      {
                                          SUP_SEMESTER_ID = dr[Common.SUP_SEMESTER.SUP_SEMESTER_ID].ToString(),
                                          SEMESTER_NAME = dr[Common.SUP_SEMESTER.SEMESTER_NAME].ToString()
                                      }).ToList();
                        objCourseRoot.SEMESTER = new SelectList(liSemester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    }

                    if (dtUGCCourseType != null && dtUGCCourseType.Rows.Count > 0)
                    {
                        liUGCCourseType = (from DataRow dr in dtUGCCourseType.Rows
                                           select new SupUGCCourseType()
                                           {
                                               UGC_COURSE_TYPE_ID = dr[Common.CP_UGC_COURSE_TYPE.UGC_COURSE_TYPE_ID].ToString(),
                                               UGC_COURSE_TYPE = dr[Common.CP_UGC_COURSE_TYPE.UGC_COURSE_TYPE].ToString()
                                           }).ToList();
                        objCourseRoot.UGC_COURSE_TYPE = new SelectList(liUGCCourseType, Common.CP_UGC_COURSE_TYPE.UGC_COURSE_TYPE_ID, Common.CP_UGC_COURSE_TYPE.UGC_COURSE_TYPE);
                    }
                    if (dtPaperType != null && dtPaperType.Rows.Count > 0)
                    {
                        liPaperType = (from DataRow dr in dtPaperType.Rows
                                       select new SupPaperType()
                                       {
                                           PAPER_TYPE_ID = dr[Common.SUP_PAPER_TYPE.PAPER_TYPE_ID].ToString(),
                                           PAPER_TYPE = dr[Common.SUP_PAPER_TYPE.PAPER_TYPE].ToString()
                                       }).ToList();
                        objCourseRoot.PAPER_TYPE = new SelectList(liPaperType, Common.SUP_PAPER_TYPE.PAPER_TYPE_ID, Common.SUP_PAPER_TYPE.PAPER_TYPE);
                    }
                    if (dtSubjectType != null && dtSubjectType.Rows.Count > 0)
                    {
                        liSupbjectType = (from DataRow dr in dtSubjectType.Rows
                                          select new SUP_SUBJECT_TYPE()
                                          {
                                              SUBJECT_TYPE_ID = dr[Common.SUP_SUBJECT_TYPE.SUBJECT_TYPE_ID].ToString(),
                                              SUBJECT_TYPE = dr[Common.SUP_SUBJECT_TYPE.SUBJECT_TYPE].ToString()
                                          }).ToList();
                        objCourseRoot.SUBJECT_TYPE = new SelectList(liSupbjectType, Common.SUP_SUBJECT_TYPE.SUBJECT_TYPE_ID, Common.SUP_SUBJECT_TYPE.SUBJECT_TYPE);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EditCourseRootInfo", "AcademicsController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EditCourseRootInfo", "AcademicsController", ex.Message);
                }
            }
            return View(objCourseRoot);
        }

        #endregion

        #region Calendar Working Day Report
        public ActionResult DateWiseCalenderReport()
        {
            AcademicsViewModel ObjViewModel = new AcademicsViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (liAcYear != null && liAcYear.Count > 0)
                    {
                        ObjViewModel.AcademicYear = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var semesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
                    if (semesterType != null && semesterType.Count > 0)
                    {
                        ObjViewModel.SemesterType = new SelectList(semesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Fee", "FetchDaywiseReportByTransactionId", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Fee", "FetchDaywiseReportByTransactionId", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindDateWiseCalenderReport(string SemesterType, string sAcademicYear)
        {
            AcademicsViewModel ObjViewModel = new AcademicsViewModel();
            JsonResultData ObjJsonData = new JsonResultData();
            var calendar = new CALENDER_2017();
            //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                calendar.ACADEMIC_YEAR = sAcademicYear;
                calendar.SEMESTER_TYPE = SemesterType;
                ObjViewModel.liCalendarList = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(calendar, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCalendarDateWiseReport), sAcademicYear).DataSource.Data;
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        #endregion


        #region Semester Duration
        public ActionResult SemesterDuration()
        {
            AcademicsViewModel viewmodal = new AcademicsViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                viewmodal.liSemesterDuration = (List<SEMESTER_DURATION>)CMSHandler.FetchData<SEMESTER_DURATION>(null,
                AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchSemesterDuration), sAcademicYear).DataSource.Data;

                var liAcYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null,
                SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                if (liAcYear != null && liAcYear.Count > 0)
                {
                    viewmodal.AcademicYear = new SelectList(liAcYear, Common.ACADEMIC_YEAR.AC_YEAR, Common.ACADEMIC_YEAR.AC_YEAR);
                }
                var liSemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null,
                SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
                if (liSemesterType != null && liSemesterType.Count > 0)
                {
                    viewmodal.SemesterType = new SelectList(liSemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                }
            }

            return View(viewmodal);
        }

        public JsonResult SaveSemesterDuration(string duration)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)

            {
                try
                {

                    var result = JsonConvert.DeserializeObject<SEMESTER_DURATION>(duration);
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    result.FROM_DATE = CommonMethods.ConvertdatetoyearFromat(result.FROM_DATE);
                    result.TO_DATE = CommonMethods.ConvertdatetoyearFromat(result.TO_DATE);

                    if (result.DURATION_ID != "" && result.DURATION_ID != null)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.UpdateSemesterDuration), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {

                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.AddSemesterDuration), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Academics", "SaveSemesterDuration", ex.ToString());
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

        public JsonResult FetchSemesterDurationById(string DurationID)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && DurationID != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = (List<SEMESTER_DURATION>)CMSHandler.FetchData<SEMESTER_DURATION>(new SEMESTER_DURATION()
                    { DURATION_ID = DurationID }, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchSemesterDurationbyId), sAcademicYear).DataSource.Data;
                    if (result != null && result.Count > 0)
                        objResultData.oResult = result;
                    else
                        objResultData.Message = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Academics", "FetchSemesterDurationById", ex.Message);
                    }
                }

            }
            else
            {
                objResultData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSemesterDurationById(string DurationID)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && DurationID != "")
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    resultArgs = CMSHandler.SaveOrUpdate(new SEMESTER_DURATION { DURATION_ID = DurationID, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.DeleteSemesterDurationById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Academics", "DeleteSemesterDurationById", ex.Message);
                    }
                }
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Student Tag Mapping

        public ActionResult StudentTagMapping()
        {
            AcademicsViewModel viewmodal = new AcademicsViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                viewmodal.liSupStudentTag = (List<STUDENT_TAG_LIST>)CMSHandler.FetchData<STUDENT_TAG_LIST>(null,
                AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchStudentTagList), sAcademicYear).DataSource.Data;


            }

            return View(viewmodal);
        }


        public JsonResult SaveStudentTagList(string TagList)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)

            {
                try
                {

                    var result = JsonConvert.DeserializeObject<STUDENT_TAG_LIST>(TagList);
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();


                    if (result.TAG_ID != "" && result.TAG_ID != null)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.UpdateStudentTagList), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {

                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.AddStudentTagList), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Academics", "SaveStudentTagList", ex.ToString());
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

        public JsonResult FetchStudentTagById(string TagID)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && TagID != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = (List<STUDENT_TAG_LIST>)CMSHandler.FetchData<STUDENT_TAG_LIST>(new STUDENT_TAG_LIST()
                    { TAG_ID = TagID }, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchStudentTagbyId), sAcademicYear).DataSource.Data;
                    if (result != null && result.Count > 0)
                        objResultData.oResult = result;
                    else
                        objResultData.Message = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Academics", "FetchStudentTagById", ex.Message);
                    }
                }

            }
            else
            {
                objResultData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteStudentTagById(string TagID)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && TagID != "")
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    resultArgs = CMSHandler.SaveOrUpdate(new STUDENT_TAG_LIST { TAG_ID = TagID, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.DeleteStudentTagById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Academics", "DeleteSemesterDurationById", ex.Message);
                    }
                }
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Syllabus Upload
        
        

        [HttpGet]
        public JsonResult GetCourseList(string sTerm, string sAcYear)
        {
            List<CourseRootInfo> liCourse = new List<CourseRootInfo>();
            try
            {
                var result = CMSHandler.FetchData<CourseRootInfo>(new { COURSE_CODE = "%" + sTerm + "%", AC_YEAR = sAcYear }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseByCourseCode), sAcYear);
                liCourse = (List<CourseRootInfo>)result.DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AcademicsController", "GetCourseList", ex.Message);
                }
            }
            
            var list = (from item in liCourse
                        select new
                        {
                            label = item.COURSE_CODE + " - " + item.COURSE_TITLE,
                            value = item.COURSE_CODE,
                            id = item.COURSE_ROOT_ID,
                            title = item.COURSE_TITLE
                        }).Cast<object>().ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SyllabusUpload()
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    DataTable dtAcademicYear = objViewModel.FetchAcademicYearDetails().DataSource.Table;
                    if (dtAcademicYear != null && dtAcademicYear.Rows.Count > 0)
                    {
                        objViewModel.AcademicYearList = (from DataRow dr in dtAcademicYear.Rows
                                                         select new SUP_ACADEMIC_YEAR_LIST
                                                         {
                                                             ACADEMIC_YEAR_ID = dr[Common.ACC_YEAR.AC_YEAR].ToString(),
                                                             AC_YEAR = dr[Common.ACC_YEAR.AC_YEAR].ToString()
                                                         }).ToList();
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AcademicsController", "SyllabusUpload", ex.Message);
                }
            }
            return View(objViewModel);
        }

        [HttpGet]
        public ActionResult GetSyllabusList(string sAcYear)
        {
            AcademicsViewModel objViewModel = new AcademicsViewModel();
            try
            {
                var result = CMSHandler.FetchData<AcademicSyllabusModel>(null, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchAcademicSyllabusByAcYear), sAcYear);
                objViewModel.liAcademicSyllabus = (List<AcademicSyllabusModel>)result.DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AcademicsController", "GetSyllabusList", ex.Message);
                }
            }
            return PartialView("_SyllabusList", objViewModel);
        }

        [HttpGet]
        public JsonResult CheckCourseCodeDetails(string sCourseCode, string sAcYear)
        {
            List<CourseRootInfo> liCourse = new List<CourseRootInfo>();
            try
            {
                var result = CMSHandler.FetchData<CourseRootInfo>(new { COURSE_CODE = sCourseCode, AC_YEAR = sAcYear }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseByCourseCode), sAcYear);
                liCourse = (List<CourseRootInfo>)result.DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AcademicsController", "CheckCourseCodeDetails", ex.Message);
                }
            }
            return Json(liCourse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UploadSyllabusFile(HttpPostedFileBase file, string sAcYear, string sCourseCode, int iCourseId, int iSyllabusId = 0)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (file != null && file.ContentLength > 0 && !string.IsNullOrEmpty(sAcYear) && !string.IsNullOrEmpty(sCourseCode) && iCourseId > 0)
                {
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (fileExtension == ".pdf")
                    {
                        string folderPath = Server.MapPath("~/Syllabus/" + sAcYear);
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        string fileName = sCourseCode + ".pdf";
                        string filePath = Path.Combine(folderPath, fileName);

                        AcademicSyllabusModel objModel = new AcademicSyllabusModel
                        {
                            SYLLABUS_ID =Convert.ToString(iSyllabusId),
                            COURSE_ID = Convert.ToString(iCourseId),
                            FILE_PATH = "/Syllabus/" + sAcYear + "/" + fileName,
                            CREATED_BY = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString(),
                            MODIFIED_BY = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString(),
                            ACADEMIC_YEAR = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString()
                        };

                        ResultArgs result;
                        if (iSyllabusId > 0)
                        {
                            result = CMSHandler.SaveOrUpdate(objModel, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.UpdateAcademicSyllabus), sAcYear);
                        }
                        else
                        {
                            // Check if already exists in DB for this course
                            var existResult = CMSHandler.FetchData<AcademicSyllabusModel>(new { COURSE_ID = iCourseId }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.CheckSyllabusExist), sAcYear);
                            var existList = (List<AcademicSyllabusModel>)existResult.DataSource.Data;

                            if (existList != null && existList.Count > 0)
                            {
                                objModel.SYLLABUS_ID = existList[0].SYLLABUS_ID;
                                result = CMSHandler.SaveOrUpdate(objModel, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.UpdateAcademicSyllabus), sAcYear);
                            }
                            else
                            {
                                result = CMSHandler.SaveOrUpdate(objModel, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.InsertAcademicSyllabus), sAcYear);
                            }
                        }

                        if (result.Success)
                        {
                            file.SaveAs(filePath);
                            objResult.sResult = "True";
                            objResult.Message = "Syllabus Saved Successfully!";
                        }
                        else
                        {
                            objResult.sResult = "False";
                            objResult.Message = "Failed to save syllabus in database!";
                        }
                    }
                    else
                    {
                        objResult.sResult = "False";
                        objResult.Message = "Only PDF files are allowed!";
                    }
                }
                else
                {
                    objResult.sResult = "False";
                    objResult.Message = "Invalid File or Data!";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AcademicsController", "UploadSyllabusFile", ex.Message);
                }
                objResult.sResult = "False";
                objResult.Message = "Error: " + ex.Message;
            }
            return Json(objResult);
        }

        [HttpPost]
        public JsonResult DeleteSyllabusEntry(string iSyllabusId, string sAcYear)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                var result = CMSHandler.SaveOrUpdate(new AcademicSyllabusModel { SYLLABUS_ID = iSyllabusId }, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.DeleteAcademicSyllabus), sAcYear);
                if (result.Success)
                {
                    objResult.sResult = "True";
                    objResult.Message = "Syllabus deleted successfully";
                }
                else
                {
                    objResult.sResult = "False";
                    objResult.Message = "Failed to delete syllabus";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AcademicsController", "DeleteSyllabusEntry", ex.Message);
                }
                objResult.sResult = "False";
                objResult.Message = "Error: " + ex.Message;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
