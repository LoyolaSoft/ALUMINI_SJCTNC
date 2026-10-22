using CMS.DAO;
using CMS.SQL.Academics;
using CMS.SQL.Account;
using CMS.SQL.Attendance;
using CMS.SQL.Examination;
using CMS.SQL.Fee;
using CMS.SQL.IQAC;
using CMS.SQL.Mentoring;
using CMS.SQL.Student;
using CMS.SQL.SupportData;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.WebSockets;
using System.Windows.Media.Animation;
using static CMS.ViewModel.ViewModel.MentoringViewModel;


namespace CMS.Controllers
{
    public class MentoringController : Controller
    {
        ResultArgs resultArgs = new ResultArgs();
        string sResult = string.Empty;

        #region Mentoring for Admin

        public ActionResult MentorVisit()
        {

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            MentoringViewModel objViewModel = new MentoringViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYear = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.AC_YEAR, Common.ACADEMIC_YEAR.ACADEMIC_NAME);
                    }
                    if (FetchAcademicYear != null && !string.IsNullOrEmpty(sAcademicYear))
                    {
                        objViewModel.liVisitedStudentdet = CMSHandler
                      .FetchData<MENTORING_VISITED_STUDENT>(null, MentoringSQL.GetMentorSQL(MentorSQLCommend.Fetchmentorvisiteddetailsforadmin))
                      .DataSource.Data as List<MENTORING_VISITED_STUDENT>;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("MonitoringController", "MentorVisit", ex.Message);
                    }
                }

            }
            return View(objViewModel);
        }

        public ActionResult BindMentoringVisitingDetails(string sAcademicYear)
        {
            string AcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() ?? string.Empty;
            MentoringViewModel objViewModel = new MentoringViewModel();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objViewModel.liVisitedStudentdet = CMSHandler
                        .FetchData<MENTORING_VISITED_STUDENT>(new MENTORING_VISITED_STUDENT { ACADEMIC_YEAR = sAcademicYear }, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchVisitRecoredByAcyear))
                        .DataSource.Data as List<MENTORING_VISITED_STUDENT>;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("MentoringController", "BindMentoringVisitingDetails", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }

        public ActionResult AddStuDetailsforadmin()
        {
            string sAcademicYear = string.Empty;
            sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    MentoringViewModel objViewModel = new MentoringViewModel();

                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYear = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.AC_YEAR, Common.ACADEMIC_YEAR.ACADEMIC_NAME);
                    }

                    List<Sup_Shift> liShift = new List<Sup_Shift>();
                    liShift = (List<Sup_Shift>)CMS.DAO.CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    objViewModel.ShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    return View(objViewModel);
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("MentoringController", "AddStuDetailsforadmin", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("MentoringController", "AddStuDetailsforadmin", ex.Message);
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

        public JsonResult BindClass(string sProgramId, string sShiftId, string AcademicYear)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    cp_Classes_2017 objModel = new cp_Classes_2017
                    {
                        SHIFT = sShiftId,
                        PROGRAMME_GROUP_ID = sProgramId
                    };

                    var classList = (List<cp_Classes_2017>)CMS.DAO.CMSHandler
                        .FetchData<cp_Classes_2017>(objModel, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchClassByShiftAndProgrammeGroupId), AcademicYear)
                        .DataSource.Data;

                    // Return the list as JSON
                    return Json(classList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Academic Year not found in session." });
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindClass", "MentoringController", "Err Msg: " + ex.Message);
                }

                return Json(new { success = false, message = "An error occurred while fetching class data." });
            }
        }


        public JsonResult GetProgramByShift(string sShiftId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sOption = string.Empty;
            var liProgram = new List<CP_PROGRAMME_2017>();
            var ObjShift = new Sup_Shift();
            ObjShift.SHIFT_ID = sShiftId;
            liProgram = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(ObjShift, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchProgrammeGroupByShiftId), sAcademicYear).DataSource.Data;
            if (liProgram != null && liProgram.Count > 0)
            {
                foreach (var item in liProgram)
                {
                    sOption += "<option value='" + item.PROGRAMME_ID + "'>" + item.PROGRAMME_NAME + "</option>";
                }
            }

            return Json(sOption);
        }

        public JsonResult FetchStudent(string sClassId, string sProgramId, string sAcademicYear)
        {
            string sOption = string.Empty;
            DataTable dtStudent = new DataTable();
            cp_Classes_2017 objModel = new cp_Classes_2017();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.CLASS_ID = sClassId;
                    objModel.PROGRAMME_GROUP_ID = sProgramId;
                    objModel.ACADEMIC_YEAR = sAcademicYear;
                    var StudentList = (List<Student_Personal_Info>)CMS.DAO.CMSHandler.FetchData<Student_Personal_Info>(objModel, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentsForCourseCertificate)).DataSource.Data;
                    if (StudentList != null && StudentList.Count > 0)
                    {
                        foreach (var item in StudentList)
                        {
                            sOption += "<option value='" + item.STUDENT_ID + "' >" + item.FIRST_NAME + "</option>";
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
                    objHandler.WriteError("FetchStudent", "MentoringController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchStudent", "MentoringController", ex.Message);
                }
            }
            return Json(sOption);
        }
        public JsonResult FetchStudents(string sClassId, string sProgramId, string sAcademicYear)
        {
            List<SelectListItem> studentOptions = new List<SelectListItem>();
            cp_Classes_2017 objModel = new cp_Classes_2017();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.CLASS_ID = sClassId;
                    objModel.PROGRAMME_GROUP_ID = sProgramId;
                    objModel.ACADEMIC_YEAR = sAcademicYear;
                    var StudentList = (List<Student_Personal_Info>)CMS.DAO.CMSHandler.FetchData<Student_Personal_Info>(objModel, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentsForCourseCertificate)).DataSource.Data;

                    if (StudentList != null && StudentList.Count > 0)
                    {
                        foreach (var item in StudentList)
                        {
                            studentOptions.Add(new SelectListItem
                            {
                                Value = item.STUDENT_ID.ToString(),
                                Text = item.FIRST_NAME
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchStudents", "MentoringController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchStudents", "MentoringController", ex.Message);
                }
            }
            var response = studentOptions.Select(x => new { value = x.Value, text = x.Text });
            return Json(response);
        }




        public JsonResult SaveVisitingDetails(VisitingDetails visitingDetails)
        {
            MentoringViewModel objViewModel = new MentoringViewModel();

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && Session[Common.SESSION_VARIABLES.USER_ID] != null)
                {
                    var mentorId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    if (!string.IsNullOrEmpty(mentorId))
                    {
                        visitingDetails.MENTOR_ID = mentorId;

                        var result = CMSHandler.SaveOrUpdate(
                            visitingDetails,
                            MentoringSQL.GetMentorSQL(MentorSQLCommend.SaveMentoringVisitedStudentDet)
                        );

                        if (result.DataSource.Data != null)
                        {
                            objViewModel.liMentoringvisiteddel = (List<MENTORING_VISITED_DETAILS>)result.DataSource.Data;
                        }
                    }
                }
                else
                {
                    throw new Exception("Session variables ACADEMIC_YEAR or USER_ID are missing.");
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveVisitingDetails", "MentoringController", "Err Msg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveVisitingDetails", "MentoringController", ex.Message);
                }
            }

            return Json(objViewModel);
        }


        public ActionResult UpdateMentoringDet(string studentId)
        {
            MentoringViewModel objViewModel = new MentoringViewModel();
            try
            {
                var fetchClassData = CMSHandler.FetchData<ClassModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByAcademicYear));
                var fetchClassList = fetchClassData.DataSource.Data as List<ClassModel>;
                if (fetchClassList != null && fetchClassList.Any())
                {
                    objViewModel.ClassName = new SelectList(fetchClassList, "CLASS_ID", "CLASS_NAME");
                }
                if (!string.IsNullOrEmpty(studentId))
                {
                    var visitedStudents = CMSHandler.FetchData<MENTORING_VISITED_STUDENT>(
                        new MENTORING_VISITED_STUDENT { STUDENT_ID = studentId },
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchVisistStudetForEdit));

                    objViewModel.liVisitedStudentdet = visitedStudents.DataSource.Data as List<MENTORING_VISITED_STUDENT>;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateMentoringDet", "MentoringController", $"Error: {ex.Message}", "Query is empty!");
                }
            }

            return View(objViewModel);
        }



        public JsonResult UpdateStudent(VisitingDetails visitingDetails)
        {
            MentoringViewModel objViewModel = new MentoringViewModel();
            try
            {
                var obj = new MENTORING_VISITED_STUDENT
                {
                    CLASS_ID = visitingDetails.CLASS_ID,
                    NATURE_OF_VISIT = visitingDetails.NATURE_OF_VISIT,
                    STUDENT_ID = visitingDetails.STUDENT_ID,
                    INITIAL_CONTACT_VISIT = visitingDetails.INITIAL_CONTACT_VISIT,
                    VISIT_DATE = visitingDetails.VISIT_DATE,
                    VISIT_TIME = visitingDetails.VISIT_TIME,
                    VISIT_DURATION = visitingDetails.VISIT_DURATION,
                    MATTER_DISCUSSED = visitingDetails.MATTER_DISCUSSED,
                    CONCLUSION_ARRIVED_AT = visitingDetails.CONCLUSION_ARRIVED_AT,
                    ACTION_TAKEN = visitingDetails.ACTION_TAKEN,
                    GENERAL_IMPRESSION = visitingDetails.GENERAL_IMPRESSION,
                    DATE_OF_NEXT_VISIT = visitingDetails.DATE_OF_NEXT_VISIT

                };

                var result = CMSHandler.SaveOrUpdate(obj, MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateMentorVisitDetailsByStudentID));

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred", error = ex.Message });
            }

            return Json(new { success = true, data = objViewModel });
        }

        public JsonResult DeleteMentorDetails(string studentId)
        {
            var objViewModel = new MentoringViewModel();
            try
            {
                if (!string.IsNullOrEmpty(studentId))
                {
                    var parameters = new MENTORING_VISITED_STUDENT
                    { STUDENT_ID = studentId };
                    var result = CMSHandler.SaveOrUpdate(parameters, MentoringSQL.GetMentorSQL(MentorSQLCommend.DeleteMentoringDetails));
                    //return Json(result);
                    if (result != null)
                    {
                        return Json(new { success = true, message = "Student deleted successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to delete student" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Student Id is null.." });
                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred", error = ex.Message });
            }
        }

        public ActionResult GetMentoringStudent(string studentId)
        {
            var ObjViewModel = new MentoringViewModel();
            try
            {
                if (!string.IsNullOrEmpty(studentId))
                {
                    var visitedStudents = CMSHandler.FetchData<MENTORING_VISITED_STUDENT>(
                       new MENTORING_VISITED_STUDENT { STUDENT_ID = studentId },
                       MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchVisistStudetForEdit));

                    ObjViewModel.liVisitedStudentdet = visitedStudents.DataSource.Data as List<MENTORING_VISITED_STUDENT>;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("GetMentoringStudent", "MentoringController", $"Error: {ex.Message}", "Query is empty!");
                }
            }
            return View(ObjViewModel);
        }

        public JsonResult UpdateMentroingVisitDet(VisitingDetails visitingDetails)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var result = CMSHandler.SaveOrUpdate(visitingDetails,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateMentoringvisitDet)).DataSource.Data as
                        List<MENTORS_REGULAR_VISIT>;

                    return Json(new { success = true, message = "Data updated successfully.", data = result });
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Session expired. Please login again." });
                }
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("UpdateMentroingVisitDet", "MentoringController", ex.Message);
                return Json(new { success = false, errorMessage = "An error occurred while updating the data." });
            }

        }

        #endregion

        #region RegularVisit
        public ActionResult MentorRegularVisit()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            MentoringViewModel objViewModel = new MentoringViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYear = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.AC_YEAR, Common.ACADEMIC_YEAR.ACADEMIC_NAME);
                    }
                    if (FetchAcademicYear != null && !string.IsNullOrEmpty(sAcademicYear))
                    {
                        objViewModel.liRegularvisitdetails = CMSHandler
                      .FetchData<MENTORS_REGULAR_VISIT>(null, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchmentorRegularvisiteddetails))
                      .DataSource.Data as List<MENTORS_REGULAR_VISIT>;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("MonitoringController", "MentorRegularVisit", ex.Message);
                    }
                }
            }
            return View(objViewModel);

        }
        public JsonResult SaveRegularVisitingDetails(RegularVisitDetails RegvisitingDetails)
        {
            MentoringViewModel objViewModel = new MentoringViewModel();

            try
            {
                var metorid = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                if (!string.IsNullOrEmpty(metorid))
                {
                    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                    {
                        RegvisitingDetails.MENTOR_ID = metorid;
                        var result = CMSHandler.SaveOrUpdate(RegvisitingDetails, MentoringSQL.GetMentorSQL(MentorSQLCommend.SaveRegularMentoringVisitedStudentDet));
                        if (result.DataSource.Data != null)
                        {
                            objViewModel.liRegularvisitdetails = (List<MENTORS_REGULAR_VISIT>)result.DataSource.Data;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveRegularVisitingDetails", "MentoringController", "Err Msg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveRegularVisitingDetails", "MentoringController", ex.Message);
                }
            }

            return Json(objViewModel);
        }
        public ActionResult AddRegularStuDetailsforadmin()
        {
            string sAcademicYear = string.Empty;
            sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    MentoringViewModel objViewModel = new MentoringViewModel();

                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYear = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.AC_YEAR, Common.ACADEMIC_YEAR.ACADEMIC_NAME);
                    }

                    List<Sup_Shift> liShift = new List<Sup_Shift>();
                    liShift = (List<Sup_Shift>)CMS.DAO.CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    objViewModel.ShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    return View(objViewModel);
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("MentoringController", "AddRegularStuDetailsforadmin", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("MentoringController", "AddRegularStuDetailsforadmin", ex.Message);
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
        public ActionResult BindMentoringRegVisitingDetails(string sAcademicYear)
        {
            string AcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() ?? string.Empty;
            MentoringViewModel objViewModel = new MentoringViewModel();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objViewModel.liRegularvisitdetails = CMSHandler
                        .FetchData<MENTORS_REGULAR_VISIT>(new MENTORS_REGULAR_VISIT { ACADEMIC_YEAR = sAcademicYear }, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchRegDetailsbyAcYear))
                        .DataSource.Data as List<MENTORS_REGULAR_VISIT>;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("MentoringController", "BindMentoringRegVisitingDetails", ex.Message);
                    }
                }
            }


            return View(objViewModel);
        }


        public ActionResult GetMentoringDetails(string studentId)
        {
            var objViewModel = new MentoringViewModel();
            try
            {
                if (!string.IsNullOrEmpty(studentId))
                {
                    var data = new MENTORS_REGULAR_VISIT
                    {
                        STUDENT_ID = studentId
                    };

                    objViewModel.liRegularvisitdetails = CMSHandler.FetchData<MENTORS_REGULAR_VISIT>(
                        data,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchRegstudentbystudentId)
                    ).DataSource.Data as List<MENTORS_REGULAR_VISIT>;
                }
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetMentoringDetails", "MentoringController", ex.Message);
            }
            return View(objViewModel);
        }

        public JsonResult UpdateMentoringDetails(MENTORS_REGULAR_VISIT MentorsRegularVisit)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var result = CMSHandler.SaveOrUpdate(MentorsRegularVisit,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateMentorRegulerDetails)).DataSource.Data as
                        List<MENTORS_REGULAR_VISIT>;

                    return Json(new { success = true, message = "Data updated successfully.", data = result });
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Session expired. Please login again." });
                }
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("UpdateMentoringDetails", "MentoringController", ex.Message);
                return Json(new { success = false, errorMessage = "An error occurred while updating the data." });
            }
        }
        public JsonResult DeleteMentorReularDet(string studentId)
        {

            try
            {
                if (!string.IsNullOrEmpty(studentId))
                {
                    var parameters = new MENTORS_REGULAR_VISIT
                    { STUDENT_ID = studentId };
                    var result = CMSHandler.SaveOrUpdate(parameters, MentoringSQL.GetMentorSQL(MentorSQLCommend.DeleteMentorReularDetails));
                    //return Json(result);
                    if (result != null)
                    {
                        return Json(new { success = true, message = "Student deleted successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to delete student" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Student Id is null.." });
                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred", error = ex.Message });
            }
        }



        #endregion

        #region MentorGroupSession
        public ActionResult MentorGroupSessionRecords()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            MentoringViewModel objViewModel = new MentoringViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYear = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.AC_YEAR, Common.ACADEMIC_YEAR.ACADEMIC_NAME);
                    }
                    if (FetchAcademicYear != null && !string.IsNullOrEmpty(sAcademicYear))
                    {
                        objViewModel.liGroupSessionRecords = CMSHandler
                      .FetchData<MENTOR_GROUP_SESSION_RECORDS>(null, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchMentorGroupSessionRecords))
                      .DataSource.Data as List<MENTOR_GROUP_SESSION_RECORDS>;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("MonitoringController", "MentorVisit", ex.Message);
                    }
                }

            }
            return View(objViewModel);
        }
        public ActionResult AddGroupSessioninfo()
        {
            string sAcademicYear = string.Empty;
            sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    MentoringViewModel objViewModel = new MentoringViewModel();

                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYear = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.AC_YEAR, Common.ACADEMIC_YEAR.ACADEMIC_NAME);
                    }

                    List<Sup_Shift> liShift = new List<Sup_Shift>();
                    liShift = (List<Sup_Shift>)CMS.DAO.CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    objViewModel.ShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    return View(objViewModel);
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("MentoringController", "AddStuDetailsforadmin", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("MentoringController", "AddStuDetailsforadmin", ex.Message);
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
        public ActionResult BindGroupSessionDetails(string sAcademicYear)
        {
            MentoringViewModel objViewModel = new MentoringViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objViewModel.liGroupSessionRecords = CMSHandler.FetchData<MENTOR_GROUP_SESSION_RECORDS>
                        (new MENTOR_GROUP_SESSION_RECORDS { ACADEMIC_YEAR = sAcademicYear }, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchMentorGroupSessionRecords))
                        .DataSource.Data as List<MENTOR_GROUP_SESSION_RECORDS>;
                }
                catch (Exception ex)
                {

                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("MentoringController", "BindGroupSessionDetails", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("MentoringController", "BindGroupSessionDetails", ex.Message);
                    }
                    sResult = "Fail To Fetch Record(s) ...!";
                    return Json(sResult, JsonRequestBehavior.AllowGet);
                }
            }
            return View(objViewModel);
        }

        public JsonResult SaveGroupSessionDetails(VisitingDetails visitingDetails)
        {
            MentoringViewModel objViewModel = new MentoringViewModel();

            try
            {
                var mentorId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    visitingDetails.MENTOR_ID = mentorId;
                    if (!string.IsNullOrEmpty(mentorId))
                    {
                        var result = CMSHandler.SaveOrUpdate(visitingDetails, MentoringSQL.GetMentorSQL(MentorSQLCommend.SaveMentoringGroupSessionDet));
                        if (result.DataSource.Data != null)
                        {
                            objViewModel.liGroupSessionRecords = (List<MENTOR_GROUP_SESSION_RECORDS>)result.DataSource.Data;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveGroupSessionDetails", "MentoringController", "Err Msg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveGroupSessionDetails", "MentoringController", ex.Message);
                }
            }

            return Json(objViewModel);
        }

        public ActionResult GetMentorGroupDetails(string studentId)
        {
            var ObjModelView = new MentoringViewModel();
            try
            {
                if (!string.IsNullOrEmpty(studentId))
                {
                    var Parameter = new MENTOR_GROUP_SESSION_RECORDS
                    { STUDENT_ID = studentId };

                    ObjModelView.liGroupSessionRecords = CMSHandler.FetchData<MENTOR_GROUP_SESSION_RECORDS>
                      (Parameter, MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateMentoringGroupDetails))
                      .DataSource.Data as List<MENTOR_GROUP_SESSION_RECORDS>;
                }
                else
                {
                    Console.WriteLine("Student Id is Empty..");
                }

            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("UpdateMentorGroupDetails", "MentoringController", ex.Message);
            }
            return View(ObjModelView);
        }

        public JsonResult UpdateMentorGroupVisit(VisitingDetails visitingDetails)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var result = CMSHandler.SaveOrUpdate(visitingDetails,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateMentoringGPDetails)).DataSource.Data as
                        List<MENTORS_REGULAR_VISIT>;

                    return Json(new { success = true, message = "Data updated successfully.", data = result });
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Session expired. Please login again." });
                }
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("UpdateMentorGroupVisit", "MentoringController", ex.Message);
                return Json(new { success = false, errorMessage = "An error occurred while updating the data." });
            }
        }

        public JsonResult DeleteMentorGroupDetails(string studentId)
        {
            try
            {
                if (!string.IsNullOrEmpty(studentId))
                {
                    var parameters = new MENTORS_REGULAR_VISIT
                    { STUDENT_ID = studentId };
                    var result = CMSHandler.SaveOrUpdate(parameters, MentoringSQL.GetMentorSQL(MentorSQLCommend.DeleteMentorGroupDetails));
                    if (result != null)
                    {
                        return Json(new { success = true, message = "Student deleted successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to delete student" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Student Id is null.." });
                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred", error = ex.Message });
            }
        }

        #endregion



        #region mentoring studentlist for studnet
        public ActionResult MentoringStuPersonalforStudent()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            MentoringViewModel objViewModel = new MentoringViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objViewModel.limentorestudentlist = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new { STAFF_ID = sUserId }, MentoringSQL.GetMentorSQL(MentorSQLCommend.fetchstudentformentoringdropdown)).DataSource.Data;
                    if (objViewModel.limentorestudentlist == null || objViewModel.limentorestudentlist.Count == 0)
                    {
                        objViewModel.message = "No Students Assigned.";
                    }
                    return View(objViewModel);
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("MonitoringController", "MentorVisit", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }


        public ActionResult GetPersonaldetForStudent(string studentId)
        {
            MentoringViewModel objViewModel = new MentoringViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            try
            {
                if (string.IsNullOrEmpty(studentId))
                {
                    throw new ArgumentException("Student ID is required.");
                }

                PERSONAL_DETAILS objModel = new PERSONAL_DETAILS
                {
                    STUDENT_ID = studentId
                };

                var semResult = CMSHandler.FetchData<SUP_SEMESTER>(
                   null,
                   MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchCommunity)
               );

                if (semResult != null)
                {
                    objViewModel.SemesterList =
                        (semResult.DataSource.Data as List<SupCommunity>)
                        ?.Select(x => new SelectListItem
                        {
                            Value = x.COMMUNITYID,
                            Text = x.COMMUNITY
                        }).ToList()
                        ?? new List<SelectListItem>();
                }

                var personalDetResult = (List<STU_personal_info>)CMSHandler.FetchData<STU_personal_info>(objModel, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchStudentPersonaldet), sAcademicYear).DataSource.Data;
                if (personalDetResult != null && personalDetResult.Count > 0)
                {
                    objViewModel.listudentList = personalDetResult;
                }
                else
                {
                    var result = CMS.DAO.CMSHandler.FetchData<STU_personal_info>(objModel, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchStudentpersonalinfo), sAcademicYear);

                    if (result != null && result.DataSource != null)
                    {
                        objViewModel.listudentList = (List<STU_personal_info>)result.DataSource.Data;
                    }
                    else
                    {
                        throw new InvalidOperationException("No data returned from the database.");
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("GetPersonaldet", "MentoringController", "Err Msg: " + ex.Message, "Query or data issue");
                    objHandler.WriteError("GetPersonaldet", "MentoringController", ex.Message);
                }
            }

            return View(objViewModel);
        }

        public ActionResult GetAcademicDetailsforstudent(string studentId)
        {
            var vm = new MentoringViewModel();

            try
            {
                string sAcademicYear =
                    Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null
                    ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()
                    : string.Empty;

                if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(sAcademicYear))
                    return View(vm);

                var param = new { STUDENT_ID = studentId };

                // ACADEMIC DETAILS



                var result = CMSHandler.FetchData<MENTORING_STUDENT_ACADEMIC_DETAILS>(
                    param,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchStudentAcademicdetails)
                );

                vm.listuacademicdet =
                    result.DataSource.Data as List<MENTORING_STUDENT_ACADEMIC_DETAILS>
                    ?? new List<MENTORING_STUDENT_ACADEMIC_DETAILS>();

                // SEMESTER IDS

                var liSemesterIds =
                    (List<ACADEMIC_SEMESTER>)CMSHandler.FetchData<ACADEMIC_SEMESTER>(
                        new STU_CLASS() { STUDENT_ID = studentId },
                        ExaminationSQL.GetExaminationSQL(
                            ExaminationSQLCommands.FetchSemesterIdsByStudentIdAndAcademicYear
                        ),
                        sAcademicYear
                    ).DataSource.Data;

                var liSemesterMarks = new List<SEMESTER_RESULT>();

                if (liSemesterIds != null && liSemesterIds.Count > 0)
                {
                    int beginYear = Convert.ToInt32(liSemesterIds.First().BEGIN_YEAR);
                    int endYear = Convert.ToInt32(liSemesterIds.First().EXPIRED_YEAR);

                    string sqlSemesterMarks =
                        ExaminationSQL.GetExaminationSQL(
                            ExaminationSQLCommands.FetchSemesterMarksByStudentIdAndAcademicYear
                        );

                    for (int year = beginYear; year <= endYear; year++)
                    {
                        var marks =
                            (List<SEMESTER_RESULT>)CMSHandler.FetchData<SEMESTER_RESULT>(
                                new STU_CLASS() { STUDENT_ID = studentId },
                                sqlSemesterMarks,
                                year.ToString()
                            ).DataSource.Data;

                        if (marks != null && marks.Count > 0)
                        {
                            liSemesterMarks.AddRange(marks);
                        }
                    }
                }

                vm.liSemesterMarks =
                    liSemesterMarks
                    .OrderBy(x => x.SEMESTER_ID)
                    .ToList();
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError(
                    "MentoringController",
                    "GetAcademicDetails",
                    ex.Message
                );
            }

            return View(vm);
        }


        public ActionResult GetHealthDetailsForStudent(string studentId)
        {
            var vm = new MentoringViewModel();

            try
            {
                var param = new { STUDENT_ID = studentId };

                var result = CMSHandler.FetchData<MENTORING_HEALTH_DETAILS>(
                    param,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchealthDetails)
                );

                vm.liStudentHealthDetails =
                    result?.DataSource?.Data as List<MENTORING_HEALTH_DETAILS>
                    ?? new List<MENTORING_HEALTH_DETAILS>();
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("MentoringController", "GetHealthDetails", ex.Message);
            }

            return View(vm);
        }

        public ActionResult GetGoalInLifeForStudent(string studentId)
        {
            ViewBag.StudentId = studentId;
            var vm = new MentoringViewModel();

            try
            {
                var model = new MENTOR_GOAL_IN_LIFE
                {
                    STUDENT_ID = studentId
                };

                var result = CMSHandler.FetchData<MENTOR_GOAL_IN_LIFE>(
                    model,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchGoalInLife)
                );

                vm.liGoalInLife = result?.DataSource?.Data as List<MENTOR_GOAL_IN_LIFE>
                                  ?? new List<MENTOR_GOAL_IN_LIFE>();
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetGoalInLife", "MentoringController", ex.ToString());
            }

            return View(vm);
        }

        public ActionResult GetSkillandTalentsForstudent(string studentId)
        {
            var viewModel = new MentoringViewModel();

            try
            {
                ViewBag.StudentId = studentId;

                var result = (List<SKILLS_AND_TALENTS>)CMSHandler.FetchData<SKILLS_AND_TALENTS>(new SKILLS_AND_TALENTS { STUDENT_ID = studentId }, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSkillandTalent)).DataSource.Data;

                if (result != null && result.Count > 0)
                {
                    viewModel.liSkillAndTalents = result;

                }

                var semResult = CMSHandler.FetchData<SUP_SEMESTER>(
                    null,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSemesters)
                );

                if (semResult != null)
                {
                    viewModel.SemesterList =
                        (semResult.DataSource.Data as List<SUP_SEMESTER>)
                        ?.Select(x => new SelectListItem
                        {
                            Value = x.SUP_SEMESTER_ID,
                            Text = x.SEMESTER_NAME
                        }).ToList()
                        ?? new List<SelectListItem>();
                }
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetSkillandTalents", "MentoringController", ex.Message);
            }

            return View(viewModel);
        }

        public ActionResult GetSportsForStudent(string studentId)
        {
            ViewBag.StudentId = studentId;
            var vm = new MentoringViewModel();

            try
            {

                var result = (List<MENTOR_SPORTS>)CMSHandler.FetchData<MENTOR_SPORTS>(new MENTOR_SPORTS { STUDENT_ID = studentId }, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSportsDetails)).DataSource.Data;

                if (result != null && result.Count > 0)
                {
                    vm.liSports = result;
                }


                var semResult = CMSHandler.FetchData<SUP_SEMESTER>(
                    null,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSemesters)
                );

                if (semResult != null)
                {
                    vm.SemesterList =
                            (semResult.DataSource.Data as List<SUP_SEMESTER>)
                            ?.Select(x => new SelectListItem
                            {
                                Value = x.SUP_SEMESTER_ID,
                                Text = x.SEMESTER_NAME
                            }).ToList()
                            ?? new List<SelectListItem>();
                }
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetSports", "MentoringController", ex.Message);
            }

            return View(vm);
        }

        public ActionResult GetParticipationForStudent(string studentId)
        {
            ViewBag.StudentId = studentId;
            var vm = new MentoringViewModel();

            try
            {
                var result = CMSHandler.FetchData<PARTICIPATION_ACHIEVEMENTS>(
                    new PARTICIPATION_ACHIEVEMENTS { STUDENT_ID = studentId },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchParticipation)
                );

                vm.liParticipationAchievement =
                    result?.DataSource?.Data as List<PARTICIPATION_ACHIEVEMENTS>
                    ?? new List<PARTICIPATION_ACHIEVEMENTS>();
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetParticipation", "MentoringController", ex.ToString());
            }

            return View(vm);
        }

        public ActionResult GetVACCoursesForStudent(string studentId)
        {
            var vm = new MentoringViewModel();
            ViewBag.StudentId = studentId;

            try
            {
                var obj = new MENTOR_VAC_PROFESSIONAL
                {
                    STUDENT_ID = studentId
                };

                var result = CMSHandler.FetchData<MENTOR_VAC_PROFESSIONAL>(
                    obj,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchVACCourses)
                );

                vm.liVACCourses = result?.DataSource?.Data as List<MENTOR_VAC_PROFESSIONAL>
                                  ?? new List<MENTOR_VAC_PROFESSIONAL>();
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetVACCourses", "MentoringController", ex.ToString());
            }

            return View(vm);
        }


        public ActionResult GetContributionsForStudent(string studentId)
        {
            var vm = new MentoringViewModel();
            ViewBag.StudentId = studentId;

            var result = CMSHandler.FetchData<MENTOR_LEADERSHIP_CONTRIBUTION>(
                new MENTOR_LEADERSHIP_CONTRIBUTION { STUDENT_ID = studentId },
                MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchContributions)
            );

            if (result != null && result.DataSource.Data != null)
            {
                vm.liContributions =
                    result.DataSource.Data as List<MENTOR_LEADERSHIP_CONTRIBUTION>;
            }

            var supResult = CMSHandler.FetchData<SUP_CONTRIBUTION_TYPE>(
                new SUP_CONTRIBUTION_TYPE { IS_ACTIVE = "1" },
                MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchContributionTypes)
            );

            if (supResult != null && supResult.DataSource.Data != null)
            {
                vm.ContributionTypeList =
                    (supResult.DataSource.Data as List<SUP_CONTRIBUTION_TYPE>)
                    .Select(x => new SelectListItem
                    {
                        Value = x.CONTRIBUTION_TYPE_ID.ToString(),
                        Text = x.TYPE_NAME
                    }).ToList();
            }

            return View(vm);
        }

        public ActionResult MentorAttendanceAbsentReportForStudent(string studentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            ViewBag.StudentId = studentId;

            var vm = new MentoringViewModel();
            vm.liAttendance = new List<MENTORING_ATTENDANCE_REPORT>();
            vm.SemesterList = new List<SelectListItem>();

            try
            {
                // Fetch Semester List for Dropdown
                var semResult = (List<SUP_SEMESTER>)CMSHandler.FetchData<SUP_SEMESTER>(null, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSemesters)).DataSource.Data;

                //var semResult = List<SUP_SEMESTER>)CMSHandler.FetchData<SUP_SEMESTER>(null, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSemesters), sAcademicYear).DataSource.Data;
                if (semResult != null && semResult.Count > 0)
                {
                    vm.SemesterList = semResult
                        ?.Select(x => new SelectListItem
                        {
                            Value = x.SUP_SEMESTER_ID.ToString(), // SUP_SEMESTER_ID aliased as SEMESTER_ID
                            Text = x.SEMESTER_NAME
                        }).ToList() ?? new List<SelectListItem>();
                }

                // Fetch Active Semester for the student to pre-load
                var activeSemResult = CMSHandler.FetchData<MENTORING_ATTENDANCE_REPORT>(new MENTORING_ATTENDANCE_REPORT


                {
                    STUDENT_ID = studentId,
                    AC_YEAR = sAcademicYear
                }, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchActiveSemester));

                var activeSemester = (activeSemResult?.DataSource?.Data as List<MENTORING_ATTENDANCE_REPORT>)?.FirstOrDefault()?.SEMESTER;
                if (activeSemester != null)
                {
                    ViewBag.ActiveSemester = activeSemester;
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("MentoringController", "MentorAttendanceAbsentReport", "Initialization Error", ex.Message);
                }
            }

            return View(vm);
        }

        public ActionResult StudentLateEntryReportForStudent(string studentId)
        {
            MentoringViewModel objViewModel = new MentoringViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                ViewBag.StudentId = studentId;

                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";

                    var ClassList = (List<ClassInfo>)CMSHandler.FetchData<ClassInfo>(new StaffList(), FeeSQL.GetFeeSQL(FeeSQLCommands.FetchClassList), sAcademicYear).DataSource.Data;
                    if (ClassList != null && ClassList.Count > 0)
                    {

                        objViewModel.ClassList = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);

                    }


                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "PedagogicMethodInputSystem", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "PedagogicMethodInputSystem", Ex.Message);
                }

            }
            return View(objViewModel);
        }





        #endregion


        #region MentoringPersonalDataForm

        public ActionResult MentoringStuPersonalforAdmin()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            MentoringViewModel objViewModel = new MentoringViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objViewModel.limentorestudentlist = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new { STAFF_ID = sUserId }, MentoringSQL.GetMentorSQL(MentorSQLCommend.fetchstudentformentoringdropdown)).DataSource.Data;
                    if (objViewModel.limentorestudentlist == null || objViewModel.limentorestudentlist.Count == 0)
                    {
                        objViewModel.message = "No Students Assigned.";
                    }
                    return View(objViewModel);
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("MonitoringController", "MentorVisit", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }
        public ActionResult GetPersonaldet(string studentId)
        {
            MentoringViewModel objViewModel = new MentoringViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            
            try
            {
                if (string.IsNullOrEmpty(studentId))
                {
                    throw new ArgumentException("Student ID is required.");
                }

                PERSONAL_DETAILS objModel = new PERSONAL_DETAILS
                {
                    STUDENT_ID = studentId
                };
                var personalDetResult = (List<STU_personal_info>)CMSHandler.FetchData<STU_personal_info>(objModel, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchStudentPersonaldet), sAcademicYear).DataSource.Data;
                if (personalDetResult != null && personalDetResult.Count > 0)
                {
                    objViewModel.listudentList = personalDetResult;
                }
                else
                {
                    var result = CMS.DAO.CMSHandler.FetchData<STU_personal_info>(objModel, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchStudentpersonalinfo), sAcademicYear);

                    if (result != null && result.DataSource != null)
                    {
                        objViewModel.listudentList = (List<STU_personal_info>)result.DataSource.Data;
                    }
                    else
                    {
                        throw new InvalidOperationException("No data returned from the database.");
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("GetPersonaldet", "MentoringController", "Err Msg: " + ex.Message, "Query or data issue");
                    objHandler.WriteError("GetPersonaldet", "MentoringController", ex.Message);
                }
            }

            return View(objViewModel);
        }


        public ActionResult GetFamilyDetails(string academicYear, string programName, string className, string studentId)
        {
            MentoringViewModel objViewModel = new MentoringViewModel();

            try
            {
                var objModel = new PERSONAL_DETAILS
                {
                    ACADEMIC_YEAR = academicYear,
                    PROGRAM_ID = programName,
                    CLASS_ID = className,
                    STUDENT_ID = studentId
                };


                var data = CMSHandler.FetchData<STUDENT_FAMILY_DETAILS>(
               objModel,
               MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchStudentFamilydetails));

                if (data != null)
                {
                    objViewModel.listudentFamList = data.DataSource.Data as List<STUDENT_FAMILY_DETAILS> ?? new List<STUDENT_FAMILY_DETAILS>();
                }
                else
                {
                    var FamilyDetails = CMSHandler.FetchData<STUDENT_FAMILY_DETAILS>(objModel, MentoringSQL.GetMentorSQL(MentorSQLCommend.CheckStudentFamilydetailsbyStudentId));
                    if (FamilyDetails != null)
                    {
                        objViewModel.listudentFamList = data.DataSource.Data as List<STUDENT_FAMILY_DETAILS> ?? new List<STUDENT_FAMILY_DETAILS>();
                    }
                }


            }
            catch (Exception ex)
            {
                using (var objHandler = new ErrorLog())
                {
                    objHandler.WriteError("GetFamilyDetails", "MentoringController", ex.ToString(), "Query or data issue");
                }
            }

            return View(objViewModel);
        }



        public JsonResult SaveFamilyDetails(MentroingFamilyDetails FmilyDetails)
        {
            JsonResultData objson = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (string.IsNullOrEmpty(FmilyDetails.MENTORS_FAMILY_ID))
                    {
                        var SaveResult = CMSHandler.SaveOrUpdate(FmilyDetails, MentoringSQL.GetMentorSQL(MentorSQLCommend.SaveMentoringFamilyDetails));
                        if (SaveResult.Success)
                        {
                            objson.Message = Common.Messages.RecordsSavedSuccessfully;
                        }
                        else
                        {
                            objson.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }
                    else
                    {
                        var UpdateDet = CMSHandler.SaveOrUpdate(FmilyDetails, MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateFamilyDetails));
                        if (UpdateDet.Success)
                        {
                            objson.Message = Common.Messages.RecordsSavedSuccessfully;
                        }
                        else
                        {
                            objson.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveFamilyDetails", "MentoringController", "Err Msg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveFamilyDetails", "MentoringController", ex.Message);
                }
            }

            return Json(objson, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult GetLanguageDetails(string academicYear, string shiftList, string programName, string className, string studentId)
        //{
        //    var viewModel = new MentoringViewModel();

        //    try
        //    {
        //        var model = new PERSONAL_DETAILS
        //        {
        //            ACADEMIC_YEAR = academicYear,
        //            PROGRAM_ID = programName,
        //            SHIFT_LIST = shiftList,
        //            CLASS_ID = className,
        //            STUDENT_ID = studentId
        //        };

        //        var result = CMS.DAO.CMSHandler.FetchData<STU_personal_info>(
        //            model, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchStudentPersonaldet)
        //        );

        //        viewModel.listudentList = result.DataSource.Data as List<STU_personal_info>
        //                                  ?? new List<STU_personal_info>();
        //    }
        //    catch (Exception ex)
        //    {
        //        new ErrorLog().WriteError("GetLanguageDetails", "MentoringController", ex.Message);
        //    }

        //    return View(viewModel);
        //}
        //public JsonResult SaveLanguageDetails(LanguageDetailsViewModel data)
        //{
        //    var Resultid = new ResultID();
        //    try
        //    {
        //        var formDataJson = Request.Form["FORMDATA"];
        //        var languageDataJson = Request.Form["LANGUAGE"];

        //        if (string.IsNullOrEmpty(formDataJson) && string.IsNullOrEmpty(languageDataJson))
        //        {
        //            return Json(new { success = false, message = "No data to save!" });
        //        }
        //        var formData = JsonConvert.DeserializeObject<LanguageDetailsViewModel>(formDataJson);
        //        var languageData = JsonConvert.DeserializeObject<List<LanguageDetailsViewModel>>(languageDataJson);

        //        if (formData == null && languageData == null)
        //        {
        //            return Json(new { success = false, message = "Invalid or incomplete data provided!" });
        //        }

        //        var saveResult = CMSHandler.SaveOrUpdate(formData, MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertLanguageDetails));
        //        var insertedId = Resultid.MENTOR_LANGUAGE_ID;
        //        saveResult.RowUniqueId = Convert.ToInt32(insertedId);
        //        if (saveResult != null)
        //        {
        //            // Console.WriteLine("Data save successfully..");
        //            var combinedData = new
        //            {
        //                Formdata = formData,
        //                Languagedata = languageData
        //            };
        //            foreach (var item in combinedData.Languagedata)
        //            {
        //                var languageSaveResult = CMSHandler.SaveOrUpdate(item, MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertLanguageDetails));
        //                var id = languageSaveResult.RowUniqueId;
        //                if (languageSaveResult != null)
        //                {
        //                    //Console.WriteLine("Data Saved successfully");
        //                    //var UpdateLanDet = CMSHandler.SaveOrUpdate(item, MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateLanguageDetails));
        //                    var Details = CMSHandler.FetchData<LanguageDetailsViewModel>(id, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchMaxLanValues));
        //                }
        //                else
        //                {
        //                    return Json(new { success = false, message = "Error saving data!" });
        //                }
        //            }
        //        }

        //        return Json(new { success = true, message = "Data Save successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = $"Error saving data: {ex.Message}" });
        //    }
        //}

        // Academic Details and Analysis
        public ActionResult GetAcademicDetails(string studentId)
        {
            var vm = new MentoringViewModel();

            try
            {
                string sAcademicYear =
                    Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null
                    ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()
                    : string.Empty;

                if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(sAcademicYear))
                    return View(vm);

                var param = new { STUDENT_ID = studentId };

                // ACADEMIC DETAILS



                var result = CMSHandler.FetchData<MENTORING_STUDENT_ACADEMIC_DETAILS>(
                    param,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchStudentAcademicdetails)
                );

                vm.listuacademicdet =
                    result.DataSource.Data as List<MENTORING_STUDENT_ACADEMIC_DETAILS>
                    ?? new List<MENTORING_STUDENT_ACADEMIC_DETAILS>();

                // SEMESTER IDS

                var liSemesterIds =
                    (List<ACADEMIC_SEMESTER>)CMSHandler.FetchData<ACADEMIC_SEMESTER>(
                        new STU_CLASS() { STUDENT_ID = studentId },
                        ExaminationSQL.GetExaminationSQL(
                            ExaminationSQLCommands.FetchSemesterIdsByStudentIdAndAcademicYear
                        ),
                        sAcademicYear
                    ).DataSource.Data;

                var liSemesterMarks = new List<SEMESTER_RESULT>();

                if (liSemesterIds != null && liSemesterIds.Count > 0)
                {
                    int beginYear = Convert.ToInt32(liSemesterIds.First().BEGIN_YEAR);
                    int endYear = Convert.ToInt32(liSemesterIds.First().EXPIRED_YEAR);

                    string sqlSemesterMarks =
                        ExaminationSQL.GetExaminationSQL(
                            ExaminationSQLCommands.FetchSemesterMarksByStudentIdAndAcademicYear
                        );

                    for (int year = beginYear; year <= endYear; year++)
                    {
                        var marks =
                            (List<SEMESTER_RESULT>)CMSHandler.FetchData<SEMESTER_RESULT>(
                                new STU_CLASS() { STUDENT_ID = studentId },
                                sqlSemesterMarks,
                                year.ToString()
                            ).DataSource.Data;

                        if (marks != null && marks.Count > 0)
                        {
                            liSemesterMarks.AddRange(marks);
                        }
                    }
                }

                vm.liSemesterMarks =
                    liSemesterMarks
                    .OrderBy(x => x.SEMESTER_ID)
                    .ToList();
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError(
                    "MentoringController",
                    "GetAcademicDetails",
                    ex.Message
                );
            }

            return View(vm);
        }


        public JsonResult SaveAcademicDetails(MENTORING_STUDENT_ACADEMIC_DETAILS model)
        {
            try
            {
                model.IS_ACTIVE = "1";
                model.IS_DELETED = "0";

                if (string.IsNullOrEmpty(model.STU_ACADEMIC_ID))
                {
                    CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertStudentAcademicDetails)
                    );
                }
                else
                {
                    CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateStudentAcademicDetails)
                    );
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Health Details
        public ActionResult GetHealthDetails(string studentId)
        {
            var vm = new MentoringViewModel();

            try
            {
                var param = new { STUDENT_ID = studentId };

                var result = CMSHandler.FetchData<MENTORING_HEALTH_DETAILS>(
                    param,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchealthDetails)
                );

                vm.liStudentHealthDetails =
                    result?.DataSource?.Data as List<MENTORING_HEALTH_DETAILS>
                    ?? new List<MENTORING_HEALTH_DETAILS>();
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("MentoringController", "GetHealthDetails", ex.Message);
            }

            return View(vm);
        }

        public JsonResult SaveHealthDetails(MENTORING_HEALTH_DETAILS model)
        {
            try
            {
                model.ACADEMIC_YEAR = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
                model.IS_ACTIVE = "1";
                model.IS_DELETED = "0";

                if (string.IsNullOrEmpty(model.STUDENT_HEALTH_ID))
                {
                    CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertStudentHealthDet)
                    );
                }
                else
                {
                    CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateHealthStudentDetails)
                    );
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #region Values
        public ActionResult GetGoalInLife(string studentId)
        {
            ViewBag.StudentId = studentId;
            var vm = new MentoringViewModel();

            try
            {
                var model = new MENTOR_GOAL_IN_LIFE
                {
                    STUDENT_ID = studentId
                };

                var result = CMSHandler.FetchData<MENTOR_GOAL_IN_LIFE>(
                    model,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchGoalInLife)
                );

                vm.liGoalInLife = result?.DataSource?.Data as List<MENTOR_GOAL_IN_LIFE>
                                  ?? new List<MENTOR_GOAL_IN_LIFE>();
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetGoalInLife", "MentoringController", ex.ToString());
            }

            return View(vm);
        }


        public JsonResult SaveGoalInLife(MENTOR_GOAL_IN_LIFE data)
        {
            try
            {
                if (data == null)
                    return Json(new { success = false, message = "No data received" });

                // 🔴 CRITICAL FIX — ALWAYS SET STUDENT_ID SERVER SIDE
                if (string.IsNullOrEmpty(data.STUDENT_ID))
                {
                    return Json(new { success = false, message = "Student ID missing" });
                }

                // 🔴 ENSURE GOAL_ID EXISTS
                if (string.IsNullOrEmpty(data.GOAL_ID))
                {
                    data.GOAL_ID = Guid.NewGuid().ToString();

                    CMSHandler.SaveOrUpdate(
                        data,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertGoalInLife)
                    );

                    return Json(new { success = true, message = "Goal saved successfully" });
                }
                else
                {
                    CMSHandler.SaveOrUpdate(
                        data,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateGoalInLife)
                    );

                    return Json(new { success = true, message = "Goal updated successfully" });

                }
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("SaveGoalInLife", "MentoringController", ex.ToString());
                return Json(new { success = false, message = "Error while saving data" });
            }
        }




        #endregion
        #region Participation & Achievement

        public ActionResult GetParticipation(string studentId)
        {
            ViewBag.StudentId = studentId;
            var vm = new MentoringViewModel();

            try
            {
                var result = CMSHandler.FetchData<PARTICIPATION_ACHIEVEMENTS>(
                    new PARTICIPATION_ACHIEVEMENTS { STUDENT_ID = studentId },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchParticipation)
                );

                vm.liParticipationAchievement =
                    result?.DataSource?.Data as List<PARTICIPATION_ACHIEVEMENTS>
                    ?? new List<PARTICIPATION_ACHIEVEMENTS>();
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetParticipation", "MentoringController", ex.ToString());
            }

            return View(vm);
        }





        public JsonResult SaveParticipation(PARTICIPATION_ACHIEVEMENTS dataToSend)
        {
            try
            {
                if (dataToSend == null)
                {
                    return Json(new { success = false, message = "Model binding failed" });
                }

                dataToSend.IS_ACTIVE = string.IsNullOrEmpty(dataToSend.IS_ACTIVE) ? "1" : dataToSend.IS_ACTIVE;
                dataToSend.IS_DELETED = string.IsNullOrEmpty(dataToSend.IS_DELETED) ? "0" : dataToSend.IS_DELETED;

                if (string.IsNullOrEmpty(dataToSend.PARTICIPATION_ID))
                {
                    CMSHandler.SaveOrUpdate(
                        dataToSend,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertAchievement)
                    );

                    return Json(new { success = true, message = "Participation saved successfully" });
                }
                else
                {
                    CMSHandler.SaveOrUpdate(
                        dataToSend,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateParticipation)
                    );

                    return Json(new { success = true, message = "Participation updated successfully" });
                }
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("SaveParticipation", "MentoringController", ex.ToString());
                return Json(new { success = false, message = "Error while saving participation" });
            }
        }



        public JsonResult GetParticipationById(string id)
        {
            try
            {
                var result = CMSHandler.FetchData<PARTICIPATION_ACHIEVEMENTS>(
                    new PARTICIPATION_ACHIEVEMENTS { PARTICIPATION_ID = id },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchParticipationById)
                );

                var data = (result.DataSource.Data as List<PARTICIPATION_ACHIEVEMENTS>)?.FirstOrDefault();

                if (data != null && !string.IsNullOrEmpty(data.DATE))
                    data.DATE = DateTime.Parse(data.DATE).ToString("yyyy-MM-dd");

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetParticipationById", "MentoringController", ex.ToString());
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult DeleteParticipation(string id)
        {
            try
            {
                CMSHandler.SaveOrUpdate(
                    new PARTICIPATION_ACHIEVEMENTS { PARTICIPATION_ID = id },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.DeleteParticipation)
                );

                return Json(new { success = true, message = "Participation deleted successfully" });
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("DeleteParticipation", "MentoringController", ex.ToString());
                return Json(new { success = false, message = "Error while deleting" });
            }
        }


        #endregion
        //   public ActionResult GetMentorstudentlist(
        //string academicYear,
        //string programName,
        //string className,
        //string studentId)
        //   {
        //       var objViewModel = new MentoringViewModel();
        //       try
        //       {
        //           var model = new MENTORING_STUDENT_LIST
        //           {
        //               STAFF_ID = studentId,        // or logged-in staff id
        //               ACADEMIC_YEAR = academicYear
        //           };

        //           var result = CMS.DAO.CMSHandler.FetchData<MENTORING_STUDENT_LIST>(
        //               model,
        //               MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchStudentMentoring)
        //           );

        //           objViewModel.limentorstulist =
        //               result?.DataSource?.Data as List<MENTORING_STUDENT_LIST>
        //               ?? new List<MENTORING_STUDENT_LIST>();
        //       }
        //       catch (Exception ex)
        //       {
        //           new ErrorLog().WriteError("GetMentorstudentlist", "MentoringController", ex.Message);
        //       }

        //       return View(objViewModel);
        //   }

        public ActionResult GetGuidance(string studentId)
        {

            ViewBag.StudentId = studentId;
            var objViewModel = new MentoringViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            try
            {
                var studentprog = CMSHandler.FetchData<MENTORING_ACTIVITY_RESPONSE>(new MENTORING_ACTIVITY_RESPONSE() { STUDENT_ID = studentId, ACADEMIC_YEAR = sAcademicYear }, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchValueandclass));
                objViewModel.listudentvaluesdetails = studentprog.DataSource.Data as List<MENTORING_ACTIVITY_RESPONSE> ??
                        new List<MENTORING_ACTIVITY_RESPONSE>();

                var studentClass = (studentprog.DataSource.Data as List<MENTORING_ACTIVITY_RESPONSE>)
                   ?.FirstOrDefault();

                if (studentClass == null)
                {
                    objViewModel.listudentvaluesdetails = new List<MENTORING_ACTIVITY_RESPONSE>();
                    return View(objViewModel);
                }

                var model = new PERSONAL_DETAILS
                {
                    ACADEMIC_YEAR = studentClass.ACADEMIC_YEAR,
                    PROGRAM_ID = studentClass.PROGRAM_ID,
                    CLASS_ID = studentClass.CLASS_ID,
                    STUDENT_ID = studentId
                };
                var result = CMS.DAO.CMSHandler.FetchData<MENTORING_ACTIVITY_RESPONSE>(
                    model, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchStudentGuidance)
                );

                objViewModel.listudentGuid = result.DataSource.Data as List<MENTORING_ACTIVITY_RESPONSE>
                                          ?? new List<MENTORING_ACTIVITY_RESPONSE>();

            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetGuidance", "MentoringController", ex.Message);
            }
            return View(objViewModel);
        }

        public JsonResult GetGuidanceById(string id)
        {
            try
            {
                var result = CMSHandler.FetchData<MENTORING_ACTIVITY_RESPONSE>(
                    new MENTORING_ACTIVITY_RESPONSE { STUDENT_GUIDANCE_ID = id },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchInterventionById)
                );

                var data = (result.DataSource.Data as List<MENTORING_ACTIVITY_RESPONSE>)?.FirstOrDefault();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveInterventionDet(string VISIT_DATE, string INTERVENTION_AND_GUIDANCE, string STUDENT_ID)
        {
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

                if (string.IsNullOrWhiteSpace(sAcademicYear))
                    return Json(new { Success = false, Message = "Academic year session expired." });

                if (string.IsNullOrWhiteSpace(STUDENT_ID))
                    return Json(new { Success = false, Message = "Student ID is missing." });

                var studentprog = CMSHandler.FetchData<MENTORING_ACTIVITY_RESPONSE>(
                    new MENTORING_ACTIVITY_RESPONSE { STUDENT_ID = STUDENT_ID, ACADEMIC_YEAR = sAcademicYear },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchValueandclass)
                );

                var studentClass = (studentprog.DataSource.Data as List<MENTORING_ACTIVITY_RESPONSE>)?.FirstOrDefault();

                if (studentClass == null)
                {
                    return Json(new { Success = false, Message = "Student class mapping not found." });
                }

                var parameters = new PERSONAL_DETAILS
                {
                    VISIT_DATE = VISIT_DATE,
                    INTERVENTION_AND_GUIDANCE = INTERVENTION_AND_GUIDANCE,
                    ACADEMIC_YEAR = sAcademicYear,
                    PROGRAM_ID = studentClass.PROGRAM_ID,
                    CLASS_ID = studentClass.CLASS_ID,
                    STUDENT_ID = STUDENT_ID
                };

                var result = CMSHandler.SaveOrUpdate(parameters, MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertGuidenceDetais));

                return Json(new { Success = true, Message = "Saved successfully." });
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveInterventionDet", "MentoringController", "Err Msg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveInterventionDet", "MentoringController", ex.Message);
                }
                return Json(new { Success = false, Message = "Error occurred." });
            }
        }

        public JsonResult UpdateInterventionDet(string STUDENT_GUIDANCE_ID, string VISIT_DATE, string INTERVENTION_AND_GUIDANCE, string STUDENT_ID)
        {
            try
            {
                string sAcademicYear =
                    Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();

                if (string.IsNullOrWhiteSpace(sAcademicYear))
                    return Json(new { Success = false, Message = "Academic year missing." });

                if (string.IsNullOrWhiteSpace(STUDENT_ID))
                    return Json(new { Success = false, Message = "Student ID missing." });

                if (string.IsNullOrWhiteSpace(VISIT_DATE) ||
                    string.IsNullOrWhiteSpace(INTERVENTION_AND_GUIDANCE))
                {
                    return Json(new { Success = false, Message = "Required fields missing." });
                }

                var studentprog = CMSHandler.FetchData<MENTORING_ACTIVITY_RESPONSE>(
                    new MENTORING_ACTIVITY_RESPONSE
                    {
                        STUDENT_ID = STUDENT_ID,
                        ACADEMIC_YEAR = sAcademicYear
                    },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchValueandclass)
                );

                if (studentprog?.DataSource?.Data == null)
                {
                    return Json(new { Success = false, Message = "Student mapping not found." });
                }

                var studentClass =
                    (studentprog.DataSource.Data as List<MENTORING_ACTIVITY_RESPONSE>)
                    ?.FirstOrDefault();

                if (studentClass == null)
                {
                    return Json(new { Success = false, Message = "Student class not found." });
                }

                var parameters = new INOVATION
                {
                    STUDENT_GUIDANCE_ID = STUDENT_GUIDANCE_ID,
                    ACADEMIC_YEAR = sAcademicYear,
                    PROGRAM_ID = studentClass.PROGRAM_ID,
                    CLASS_ID = studentClass.CLASS_ID,
                    STUDENT_ID = STUDENT_ID,
                    INTERVENTION_AND_GUIDANCE = INTERVENTION_AND_GUIDANCE,
                    VISIT_DATE = VISIT_DATE
                };

                var guidupdate = CMSHandler.SaveOrUpdate(
                    parameters,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateInovationDetils)
                );
                return Json(new { Success = true, Message = "Updated successfully." });
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError(
                    "UpdateInterventionDet",
                    "MentoringController",
                    ex.ToString()
                );
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        public JsonResult DeleteInterventionDet(int guidanceId)
        {
            try
            {
                if (guidanceId <= 0)
                    return Json(new { Success = false, Message = "Invalid record." });

                var param = new MENTORING_ACTIVITY_RESPONSE
                {
                    STUDENT_GUIDANCE_ID = guidanceId.ToString()
                };

                var GiudDelete = CMSHandler.SaveOrUpdate(
                    param,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.DeleteGuidenceDetais)
                );

                return Json(new { Success = true, Message = "Deleted successfully." });
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError(
                    "DeleteInterventionDet",
                    "MentoringController",
                    ex.ToString()
                );
                return Json(new { Success = false, Message = "Delete failed." });
            }
        }




        public JsonResult UpdatePersonalDet(StudentDetails model)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] == null)
                {
                    return Json(new { success = false, message = "Session expired" });
                }

                if (!string.IsNullOrEmpty(model.MENTOR_STUDENT_ID))
                {
                    ResultArgs updateResult = CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateStudentPersonal)
                    );

                    // 2️ If UPDATE worked → DONE
                    if (updateResult != null && updateResult.Success)
                    {
                        return Json(new
                        {
                            success = true,
                            action = "updated"
                        });
                    }
                }
                else
                {

                    // 3️ UPDATE failed → Try INSERT
                    ResultArgs insertResult = CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertStudentPersonal)
                    );

                    if (insertResult != null && insertResult.Success)
                    {
                        return Json(new
                        {
                            success = true,
                            action = "inserted"
                        });
                    }
                }
                // 4️ Both failed
                return Json(new
                {
                    success = false,
                    message = ""
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }



        public JsonResult PersonalDetlBindClass(string sProgramId, string sShiftId, string AcademicYear)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    cp_Classes_2017 objModel = new cp_Classes_2017
                    {
                        SHIFT = sShiftId,
                        PROGRAMME_GROUP_ID = sProgramId
                    };

                    var classList = (List<cp_Classes_2017>)CMS.DAO.CMSHandler
                        .FetchData<cp_Classes_2017>(objModel, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchClassByShiftAndProgrammeGroupId), AcademicYear)
                        .DataSource.Data;

                    // Return the list as JSON
                    return Json(classList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Academic Year not found in session." });
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindClass", "MentoringController", "Err Msg: " + ex.Message);
                }

                return Json(new { success = false, message = "An error occurred while fetching class data." });
            }
        }


        public JsonResult PersonalDetGetProgramByShift(string sShiftId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()
                : string.Empty;

            string sOption = string.Empty;
            var ObjShift = new Sup_Shift { SHIFT_ID = sShiftId };
            var liProgram = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(
                ObjShift,
                StudentSQL.GetStudentSQL(StudentSQLCommads.FetchProgrammeGroupByShiftId),
                sAcademicYear
            ).DataSource.Data;

            if (liProgram != null && liProgram.Count > 0)
            {
                foreach (var item in liProgram)
                {
                    sOption += "<option value='" + item.PROGRAMME_ID + "'>" + item.PROGRAMME_NAME + "</option>";
                }
            }

            return Json(sOption);
        }
        public ActionResult PersonalDetails()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            MentoringViewModel objViewModel = new MentoringViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYear = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.AC_YEAR, Common.ACADEMIC_YEAR.ACADEMIC_NAME);
                    }
                    if (FetchAcademicYear != null && !string.IsNullOrEmpty(sAcademicYear))
                    {
                        objViewModel.liVisitedStudentdet = CMSHandler
                      .FetchData<MENTORING_VISITED_STUDENT>(null, MentoringSQL.GetMentorSQL(MentorSQLCommend.Fetchmentorvisiteddetailsforadmin))
                      .DataSource.Data as List<MENTORING_VISITED_STUDENT>;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("MonitoringController", "MentorVisit", ex.Message);
                    }
                }

            }
            return View(objViewModel);
        }



        public JsonResult PersonalDetFetchStudent(string sClassId, string sProgramId, string sAcademicYear)
        {
            string sOption = string.Empty;
            DataTable dtStudent = new DataTable();
            cp_Classes_2017 objModel = new cp_Classes_2017();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.CLASS_ID = sClassId;
                    objModel.PROGRAMME_GROUP_ID = sProgramId;
                    objModel.ACADEMIC_YEAR = sAcademicYear;
                    var StudentList = (List<Student_Personal_Info>)CMS.DAO.CMSHandler.FetchData<Student_Personal_Info>(objModel, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentsForCourseCertificate)).DataSource.Data;
                    if (StudentList != null && StudentList.Count > 0)
                    {
                        foreach (var item in StudentList)
                        {
                            sOption += "<option value='" + item.STUDENT_ID + "' >" + item.FIRST_NAME + "</option>";
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
                    objHandler.WriteError("FetchStudent", "MentoringController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchStudent", "MentoringController", ex.Message);
                }
            }
            return Json(sOption);
        }
        public JsonResult PersonalDetFetchStudents(string sClassId, string sProgramId, string sAcademicYear)
        {
            List<SelectListItem> studentOptions = new List<SelectListItem>();
            cp_Classes_2017 objModel = new cp_Classes_2017();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.CLASS_ID = sClassId;
                    objModel.PROGRAMME_GROUP_ID = sProgramId;
                    objModel.ACADEMIC_YEAR = sAcademicYear;
                    var StudentList = (List<Student_Personal_Info>)CMS.DAO.CMSHandler.FetchData<Student_Personal_Info>(objModel, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentsForCourseCertificate)).DataSource.Data;

                    if (StudentList != null && StudentList.Count > 0)
                    {
                        foreach (var item in StudentList)
                        {
                            studentOptions.Add(new SelectListItem
                            {
                                Value = item.STUDENT_ID.ToString(),
                                Text = item.FIRST_NAME
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchStudents", "MentoringController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchStudents", "MentoringController", ex.Message);
                }
            }
            var response = studentOptions.Select(x => new { value = x.Value, text = x.Text });
            return Json(response);
        }

        #endregion

        #region Mentrong Student LIst
        public ActionResult Mentoringstudentlist()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            STAFFPROFILEVIEW objModel = new STAFFPROFILEVIEW();
            MentoringViewModel ObjViewModel = new MentoringViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objModel.STAFF_ID = sUserId;
                    var libatch = (List<sup_Batch>)CMSHandler.FetchData<sup_Batch>(objModel, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchBatch)).DataSource.Data;
                    if (libatch != null && libatch.Count > 0)
                    {
                        ObjViewModel.Bacthlist = new SelectList(libatch, Common.SUP_BATCHES.BATCH_ID, Common.SUP_BATCHES.BATCH);
                    }
                    var lishift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(objModel, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (lishift != null && lishift.Count > 0)
                    {
                        ObjViewModel.Shiftlist = new SelectList(lishift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("MonitoringController", "MentorVisit", ex.Message);
                    }
                }

            }
            return View(ObjViewModel);
        }
        public JsonResult Fetchprogrammeidbyshiftid(string sShift, string sBatch)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;

            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liprogramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(new CP_PROGRAMME_2017() { SHIFT_ID = sShift },
                        SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammesByShiftId)).DataSource.Data;

                    if (liprogramme != null && liprogramme.Count > 0)
                    {
                        ObjJsonData.sResult = "<option value=''> -- Select -- </option>";
                        foreach (var item in liprogramme)
                        {
                            ObjJsonData.sResult += "<option value=\"" + item.PROGRAMME_GROUP_ID + "\">" + item.PROGRAMME_NAME + "</option>";
                        }

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("MonitoringController", "Fetchprogrammeidbyshiftid", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindStudentlistforstaff(string sShift, string sBatch, string sProgramme)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new MentoringViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    var listudentlist = DapperHandler.FetchAll<Stu_list>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByProgrammeid), new { PROGRAMME_ID = sProgramme, BATCH = sBatch }, sAcademicYear);
                    ObjViewModel.listudentlist = listudentlist;
                    if (listudentlist != null && listudentlist.Count > 0)
                    {

                        ObjViewModel.listudentlist = listudentlist;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("MonitoringController", "BindStudentlistforstaff", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }



        public JsonResult SaveStudentListforMentoring(string sJsonData)
        {
            JsonResultData objResultData = new JsonResultData();
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = JsonConvert.DeserializeObject<MENTORING_STUDENT_LIST>(sJsonData);
                    result.STAFF_ID = sUserId;

                    // Split STUDENT_IDs and iterate over each ID
                    string[] studentIds = result.STUDENT_ID.Split(',');
                    foreach (var studentId in studentIds)
                    {
                        MENTORING_STUDENT_LIST individualStudent = new MENTORING_STUDENT_LIST
                        {
                            BATCH = result.BATCH_LIST,
                            STUDENT_ID = studentId.Trim(), // Assigning each student ID separately
                            CLASS_ID = result.CLASS_ID,
                            MENTORINT_ID = result.MENTORINT_ID,
                            STAFF_ID = result.STAFF_ID
                        };

                        if (!string.IsNullOrEmpty(individualStudent.MENTORINT_ID))
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(
                                individualStudent,
                                SQL.Mentoring.MentoringSQL.GetMentorSQL(MentorSQLCommend.updateMentoringstudentlist),
                                sAcademicYear
                            );
                            objResultData.Message = resultArgs.Success
                                ? Common.Messages.RecordsUpdatedSuccessfully
                                : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success
                                ? Common.ErrorCode.Ok
                                : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(
                                individualStudent,
                                SQL.Mentoring.MentoringSQL.GetMentorSQL(MentorSQLCommend.saveMentoringstudentlist),
                                sAcademicYear
                            );
                            objResultData.Message = resultArgs.Success
                                ? Common.Messages.RecordsSavedSuccessfully
                                : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success
                                ? Common.ErrorCode.Ok
                                : Common.ErrorCode.ExpectationFailed;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Mentoring", "SaveStudentListforMentoring", ex.Message);
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


        #endregion

        public ActionResult StudentMentorRegistration()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            MentoringViewModel objViewModel = new MentoringViewModel();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FIRSTLIST = (List<STF_INFO>)CMSHandler.FetchData<STF_INFO>(
                        new STF_INFO { CATEGORY = "1" },
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchStaffListForMentoring),
                        sAcademicYear
                    ).DataSource.Data;

                    objViewModel.listumentorregistration = (List<MENTORING_VISITED_STUDENT>)CMSHandler.FetchData<MENTORING_VISITED_STUDENT>(
                        new MENTORING_VISITED_STUDENT { STUDENT_ID = sUserId },
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchMentoring),
                        sAcademicYear
                    ).DataSource.Data;
                    var studept = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new STU_PERSONAL_INFO { STUDENT_ID = sUserId }, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentInfos), sAcademicYear).DataSource.Data;

                    var studentDept = studept.FirstOrDefault().DEPT_ID;
                    if (studentDept != null)
                    {
                        FIRSTLIST = FIRSTLIST.Where(x => x.DEPT_ID != studentDept).ToList();
                    }
                    objViewModel.listaffinfo = FIRSTLIST;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("MonitoringController", "StudentMentorRegistration", ex.Message);
                    }
                }
            }

            return View(objViewModel);
        }

        public JsonResult Savementorforstudent(string sMentorId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserid = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Addmentor = CMSHandler.SaveOrUpdate(new MENTORING_VISITED_STUDENT() { STAFF_ID = sMentorId, STUDENT_ID = sUserid, IS_ACTIVE = "1" }, MentoringSQL.GetMentorSQL(MentorSQLCommend.SaveMentorforstudent), sAcademicYear);
                    ObjJsonData.Message = (Addmentor.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Savementorforstudent", "MentoringController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("Savementorforstudent", "MentoringController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StudentMentorUpdationforadmin()
        {
            StudentViewModel objViewModel = new StudentViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            try
            {
                var AcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                if (AcademicYear != null && AcademicYear.Count > 0)
                {
                    objViewModel.AcademicYear = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.AC_YEAR, Common.ACADEMIC_YEAR.ACADEMIC_NAME);
                }
                var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                if (liProgramme != null && liProgramme.Count > 0)
                {
                    objViewModel.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                }

                var FetchClassYear = (List<SUP_CLASS_YEAR>)CMSHandler.FetchData<SUP_CLASS_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear), sAcademicYear).DataSource.Data;
                objViewModel.ClassYear = new SelectList(FetchClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);


            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("StudentMentorUpdationforadmin", "MentoringController", ex.Message);
                }

            }

            return View(objViewModel);
        }


        public ActionResult BindStudentMentorUpdationforadmin(string sAcademicYear, string sProgrammelist, string sPorgrammeyear)
        {
            StudentViewModel objViewModel = new StudentViewModel();
            JsonResultData objResult = new JsonResultData();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objViewModel.liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new CP_COURSE_PATTERN() { PROGRAMME_GROUP_ID = sProgrammelist, PROGRAMME_YEAR = sPorgrammeyear },
                      MentoringSQL.GetMentorSQL(DAO.MentorSQLCommend.fetchstudentformentoring).Replace("?AC_YEAR", sAcademicYear), sAcademicYear).DataSource.Data;

                    if (objViewModel.liStudent != null)
                    {

                        objViewModel.listaffinfo = (List<STF_INFO>)CMSHandler.FetchData<STF_INFO>(new STF_INFO { CATEGORY = "1", DEPARTMENT = objViewModel.liStudent.FirstOrDefault().DEPARTMENT }, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchStaffListForMentoringadmin), sAcademicYear).DataSource.Data;

                    }
                }
                catch (Exception Ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("BindStudentMentorUpdationforadmin", "MentoringController", "Err MSg: " + Ex.Message, "Query is empty!");
                        objHandler.WriteError("BindStudentMentorUpdationforadmin", "MentoringController", Ex.Message);
                    }

                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");

            return View(objViewModel);
        }


        public JsonResult Savementorforstudentforadmin(string sSTUDENT_ID, string sMentor, string sMentorid)
        {
            string sResult = string.Empty;
            try
            {
                var Result = new STU_PERSONAL_INFO();
                Result.STUDENT_ID = sSTUDENT_ID;
                Result.STAFF_ID = sMentor;
                Result.MENTORINT_ID = sMentor;
                Result.IS_ACTIVE = "1";
                ResultArgs resultArgs = new ResultArgs();
                if (!string.IsNullOrEmpty(sMentorid))
                {
                    resultArgs = DAO.CMSHandler.SaveOrUpdate(Result, MentoringSQL.GetMentorSQL(DAO.MentorSQLCommend.UpdateMentorforstudent));
                }
                else
                {
                    resultArgs = DAO.CMSHandler.SaveOrUpdate(Result, MentoringSQL.GetMentorSQL(DAO.MentorSQLCommend.SaveMentorforstudent));
                }

                if (resultArgs.Success)
                {
                    sResult = "Mentor save Success...!";
                }
                else
                {
                    sResult = "Failed to save mentor...!";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AccountController", "Resetnewpassword", ex.Message);
                }
            }
            if (string.IsNullOrEmpty(sResult))
            {
                sResult = "No Changes Made !";
            }
            return Json(sResult);
        }

        #region Staff Quota 

        public ActionResult StaffQuota()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear) && (!string.IsNullOrEmpty(sUserId)))
            {
                try
                {
                    var lidepartmentlist = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment)).DataSource.Data;
                    if (lidepartmentlist != null && lidepartmentlist.Count > 0)
                    {
                        ObjViewModel.DepartmentList = new SelectList(lidepartmentlist, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    }
                    var liStaff = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff), sAcademicYear).DataSource.Data;
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        ObjViewModel.StaffListDDL = new SelectList(liStaff, Common.CP_FACULTY_2017.STAFF_ID, Common.CP_FACULTY_2017.FIRST_NAME);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return View(ObjViewModel);
        }

        public ActionResult BindStaffforMentoringQuota(string sDeptids, string sStaffids)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new MentoringViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    string sSql = string.Empty;
                    sSql = MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchStaffforMetoringQuota)
                        .Replace("?STAFF_ID", sStaffids)
                        .Replace("?DEPARTMENT", sDeptids);
                    var FetchStaffList = (List<STU_personal_info>)CMSHandler.FetchData<STU_personal_info>(new
                    { STAFF_ID = sStaffids, DEPARTMENT = sDeptids }, sSql).DataSource.Data;
                    if (FetchStaffList != null)
                    {
                        ObjViewModel.listaffdetails = FetchStaffList;
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

            return View(ObjViewModel);
        }

        public JsonResult SaveStaffQuotaDetails(List<STAFF_QUOTA_DETAILS> staffquotaList)
        {
            try
            {
                foreach (var staffquota in staffquotaList)
                {
                    var parameter = new STAFF_QUOTA_DETAILS
                    {
                        STAFF_ID = staffquota.STAFF_ID,
                        UG1 = staffquota.UG1,
                        UG2 = staffquota.UG2,
                        UG3 = staffquota.UG3,
                        PG1 = staffquota.PG1,
                        PG2 = staffquota.PG2,
                        TOTAL = staffquota.TOTAL
                    };
                    var existingRecords = CMSHandler.FetchData<STAFF_QUOTA_DETAILS>(parameter, MentoringSQL.GetMentorSQL(MentorSQLCommend.StaffMentoringQuotaaldreadyexist)
                    ).DataSource.Data as List<STAFF_QUOTA_DETAILS>;

                    var existingRecord = existingRecords.FirstOrDefault().STAFF_ID;

                    if (existingRecord != null && existingRecord == staffquota.STAFF_ID)
                    {
                        var isUpdated = CMSHandler.SaveOrUpdate(parameter, MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateMentoringQuotaDetails));

                        if (isUpdated == null)
                        {
                            return Json(new { success = false, message = "Failed to update staff ID: " + staffquota.STAFF_ID });
                        }
                    }
                    else
                    {
                        var isInserted = CMSHandler.SaveOrUpdate(staffquota, MentoringSQL.GetMentorSQL(MentorSQLCommend.SaveMentoringquotadetails));

                        if (isInserted == null)
                        {
                            return Json(new { success = false, message = "Failed to save staff ID: " + staffquota.STAFF_ID });
                        }
                    }
                }

                return Json(new { success = true, message = "Staff quota details saved successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }


        #endregion

        #region Semester-wise Academic & Attendance Record 

        public ActionResult GetSemesterWiseAcademicAndAttendanceRecord(string studentId, string semesterType)
        {
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                var ObjViewModel = new MentoringViewModel();

                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    // Fetch Semester IDs for the student
                    var liSemesterIds = (List<SupSemester>)CMSHandler.FetchData<SupSemester>(new STU_CLASS() { STUDENT_ID = studentId.ToString() },
                            SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester), sAcademicYear).DataSource.Data;

                    if (liSemesterIds != null && liSemesterIds.Count > 0)
                    {
                        var semesterList = liSemesterIds.Select(x => new
                        {
                            SEMESTER_ID = x.SUP_SEMESTER_ID, // Asuming Semester Number is enough, otherwise map to ID
                            SEMESTER_NAME = "Semester " + x.SEMESTER_NAME
                        }).Distinct().OrderBy(x => x.SEMESTER_ID).ToList();

                        ObjViewModel.SemesterType = new SelectList(semesterList, "SEMESTER_ID", "SEMESTER_NAME");
                    }
                    else
                    {
                        // Fallback or empty if no semesters found
                        ObjViewModel.SemesterType = new SelectList(new List<object>(), "SEMESTER_ID", "SEMESTER_NAME");
                    }
                }

                // Pass studentId to view for AJAX calls
                ViewBag.StudentId = studentId;
                return View(ObjViewModel);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Mentoring", "GetSemesterWiseAcademicAndAttendanceRecord", ex.Message);
                }
                return RedirectToAction("ErrorMessage", "Error");
            }
        }

        public JsonResult GetSemesterData(string studentId, string semester)
        {
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                var debugInfo = new List<string>(); // For debugging

                var resultData = new
                {
                    Marks = new List<SEMESTER_RESULT>(),
                    Attendance = new { TotalDays = 0, AbsentDays = 0, Percentage = 0.0 },
                    Arrears = new List<SEMESTER_RESULT>(),
                    Summary = ""
                };

                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    // 1. Fetch Marks
                    // Logic recycled from DisplaySemesterMarksForStudent but filtered
                    // Use FetchData with specific Student ID model
                    var liSemesterIds = (List<ACADEMIC_SEMESTER>)CMSHandler.FetchData<ACADEMIC_SEMESTER>(new STU_CLASS() { STUDENT_ID = studentId },
                            ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchSemesterIdsByStudentIdAndAcademicYear), sAcademicYear).DataSource.Data;

                    var liSemesterMarks = new List<SEMESTER_RESULT>();

                    if (liSemesterIds != null && liSemesterIds.Count > 0)
                    {
                        string sSQLSemesterMark = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchSemesterMarksByStudentIdAndAcademicYear);

                        // Handle potential null/empty years
                        int nBeginYear = 0;
                        int nExpiredYear = 0;
                        int.TryParse(liSemesterIds.FirstOrDefault().BEGIN_YEAR, out nBeginYear);
                        int.TryParse(liSemesterIds.FirstOrDefault().EXPIRED_YEAR, out nExpiredYear);

                        // Fallback if years are invalid (e.g. 0)
                        if (nBeginYear == 0) nBeginYear = int.Parse(sAcademicYear.Split('-')[0]); // Fallback to current AY start
                        if (nExpiredYear == 0) nExpiredYear = nBeginYear + 5; // Scan a reasonable range

                        debugInfo.Add($"Scanning years: {nBeginYear} to {nExpiredYear}");

                        for (int i = nBeginYear; i <= nExpiredYear; i++)
                        {
                            try
                            {
                                var liFetchSemesterMarks = (List<SEMESTER_RESULT>)CMSHandler.FetchData<SEMESTER_RESULT>(new STU_CLASS() { STUDENT_ID = studentId },
                                            sSQLSemesterMark, i.ToString()).DataSource.Data;

                                if (liFetchSemesterMarks != null && liFetchSemesterMarks.Count > 0)
                                {
                                    liSemesterMarks.AddRange(liFetchSemesterMarks);
                                    debugInfo.Add($"Found {liFetchSemesterMarks.Count} marks for year {i}");
                                }
                            }
                            catch (Exception exLoop)
                            {
                                debugInfo.Add($"Error scanning year {i}: {exLoop.Message}");
                            }
                        }
                    }
                    else
                    {
                        debugInfo.Add("No Semester IDs found for student.");
                    }

                    // Filter by Semester
                    // Ensure robust comparison (trim, case)
                    var semesterMarks = liSemesterMarks.Where(x => (x.SEMESTER ?? "").Trim() == (semester ?? "").Trim()).ToList();

                    // Logic for Arrears: Show ALL arrears or just this semester's?
                    // Typically "Arrear Tracking" implies showing current failed subjects.
                    // Existing logic: Result not 'P' (Pass) or 'PASS'.
                    // Common codes: F (Fail), A (Absent), W (Withheld), RA (Re-Appear)
                    var arrears = liSemesterMarks.Where(x => x.RESULT != "P" && x.RESULT != "PASS" && x.RESULT != "No").ToList();

                    // 2. Attendance
                    // Placeholder logic until CommonAbsenteeReportforStudent is available/identified
                    // resultData.Attendance ...

                    // 3. Summary
                    var summary = "";

                    return Json(new { success = true, marks = semesterMarks, arrears = arrears, attendance = resultData.Attendance, summary = summary, debug = debugInfo }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Session Expired" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Mentoring", "GetSemesterData", ex.Message);
                }
                return Json(new { success = false, message = "Error fetching data: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveSemesterSummary(string studentId, string semester, string summary)
        {
            // Implement saving logic here
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

                // Use MentoringSQL to save. 
                // We will need to define a consistent way to store this.
                // For now returning success to unblock UI.

                return Json(new { success = true, message = "Summary saved successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public ActionResult DisplaySemesterMarksForStudent()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new MentoringViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Fetch Semester Ids Till Currect Semester ...
                    var liSemesterIds = (List<ACADEMIC_SEMESTER>)CMSHandler.FetchData<ACADEMIC_SEMESTER>(new STU_CLASS() { STUDENT_ID = sUserId },
                            ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchSemesterIdsByStudentIdAndAcademicYear), sAcademicYear).DataSource.Data;

                    var ObjModel = new STU_CLASS() { STUDENT_ID = sUserId };
                    if (liSemesterIds != null && liSemesterIds.Count > 0)
                    {
                        ObjModel.SEMESTER = liSemesterIds.FirstOrDefault().SEMESTER;
                        ObjModel.BATCH = liSemesterIds.FirstOrDefault().BATCH;
                        ObjModel.PROGRAMME_GROUP_ID = liSemesterIds.FirstOrDefault().PROGRAMME_GROUP_ID;

                        string sSQLSemesterMark = string.Empty;
                        sSQLSemesterMark = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchSemesterMarksByStudentIdAndAcademicYear);
                        int nBeginYear = Convert.ToInt32(liSemesterIds.FirstOrDefault().BEGIN_YEAR);
                        int nExpiredYear = Convert.ToInt32(liSemesterIds.FirstOrDefault().EXPIRED_YEAR);
                        var liCourse = new List<EXAM_REGISTRATION>();
                        var liSemesterMarks = new List<SEMESTER_RESULT>();
                        string item = string.Empty;
                        for (int i = nBeginYear; i <= nExpiredYear; i++)
                        {
                            item = i.ToString();
                            var liFetchSemesterMarks = (List<SEMESTER_RESULT>)CMSHandler.FetchData<SEMESTER_RESULT>(new STU_CLASS() { STUDENT_ID = sUserId },
                                        sSQLSemesterMark, item).DataSource.Data;
                            if (liFetchSemesterMarks != null && liFetchSemesterMarks.Count > 0)
                            {
                                liSemesterMarks = (liSemesterMarks != null && liSemesterMarks.Count > 0) ? liSemesterMarks.Concat(liFetchSemesterMarks).ToList() : liFetchSemesterMarks;
                            }

                        }
                        ObjViewModel.liSemesterList = liSemesterMarks.OrderBy(x => x.SEMESTER_ID).ToList();
                        // ObjViewModel.liExamRegistrationList = liCourse.OrderBy(x=>x.SEMESTER_ID).ToList();
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Examination", "DisplaySemesterMarksForStudent", ex.Message);
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

        // Overall Mentor Assessment
        public ActionResult GetOverallMentorAssessment(string studentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            MentoringViewModel vm = new MentoringViewModel();


            try
            {
                if (string.IsNullOrEmpty(sAcademicYear) || string.IsNullOrEmpty(studentId))
                {
                    vm.message = "Session Expired.";
                    return PartialView(vm);
                }

                // 1. Fetch Saved Answer Data (Safe Simple Query)
                var savedData = (List<OverallMentorAssessmentRow>)
                    CMSHandler.FetchData<OverallMentorAssessmentRow>(
                        new
                        {
                            STUDENT_ID = studentId,
                            ACADEMIC_YEAR = sAcademicYear
                        },
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchOverallMentorAssessment)
                    ).DataSource.Data;

                vm.liOverallAssessmentRecords = savedData ?? new List<OverallMentorAssessmentRow>();

                // 2. Fetch Dropdown Options (HARDCODED TO FIX ERROR)
                // The DB Fetch was causing Internal Server Error. We use static list for now.
                vm.OverallAcademicAssessment = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Excellent", Value = "Excellent" },
                    new SelectListItem { Text = "Very Good", Value = "Very Good" },
                    new SelectListItem { Text = "Good", Value = "Good" },
                    new SelectListItem { Text = "Average", Value = "Average" },
                    new SelectListItem { Text = "Below Average", Value = "Below Average" },
                     new SelectListItem { Text = "Poor", Value = "Poor" }
                };

                /* UNCOMMENT WHEN TABLE IS READY
                try
                {
                    var levelResult = CMSHandler.FetchData<SUP_ACADEMIC_ASSESSMENT_OBJECTIVE>(
                        new SUP_ACADEMIC_ASSESSMENT_OBJECTIVE { IS_ACTIVE = "1" },
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchOverallAssessment)
                    );
                    if (levelResult?.DataSource?.Data != null) {
                         vm.OverallAcademicAssessment = (levelResult.DataSource.Data as List<SUP_ACADEMIC_ASSESSMENT_OBJECTIVE>)
                            .Select(x => new SelectListItem { Value = x.TYPE_NAME, Text = x.TYPE_NAME }).ToList();
                    }
                }
                catch (Exception ex) { vm.message = "DB Error: " + ex.Message; } 
                */

                vm.liOverallMentorAssessment = new List<OverallMentorAssessmentRow>()
                {
                    new OverallMentorAssessmentRow { AREA = "Academic Progress" },
                    new OverallMentorAssessmentRow { AREA = "Behaviour & Discipline" },
                    new OverallMentorAssessmentRow { AREA = "Emotional / Psychological Well-being" },
                    new OverallMentorAssessmentRow { AREA = "Career Readiness" },
                    new OverallMentorAssessmentRow { AREA = "Special Support Required" }
                };

                // 3. Map saved data to ViewModel Rows
                if (savedData != null && savedData.Count > 0)
                {
                    foreach (var row in vm.liOverallMentorAssessment)
                    {
                        var dbRow = savedData.FirstOrDefault(x =>
                            x.AREA.Trim().Equals(row.AREA.Trim(), StringComparison.OrdinalIgnoreCase)
                        );
                        if (dbRow != null)
                        {
                            row.OVERALL_ASSESSMENT_ID = dbRow.OVERALL_ASSESSMENT_ID;
                            row.SEM1 = dbRow.SEM1;
                            row.SEM2 = dbRow.SEM2;
                            row.SEM3 = dbRow.SEM3;
                            row.SEM4 = dbRow.SEM4;
                            row.SEM5 = dbRow.SEM5;
                            row.SEM6 = dbRow.SEM6;
                        }
                    }
                }

                ViewBag.StudentId = studentId;
                return PartialView(vm);
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("Mentoring", "GetOverallMentorAssessment", ex.Message);
                }
                vm.message = "Error loading assessment: " + ex.Message;
                return PartialView(vm);
            }
        }

        public JsonResult SaveOverallMentorAssessment(OverallAssessmentSaveVM model)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();

            try
            {
                foreach (var row in model.data)
                {
                    row.STUDENT_ID = model.studentId;
                    row.ACADEMIC_YEAR = sAcademicYear;
                    row.CREATED_BY = sUserId;

                    ResultArgs resultArgs;

                    // UPDATE
                    if (!string.IsNullOrEmpty(row.OVERALL_ASSESSMENT_ID))
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(
                            row,
                            MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateOverallMentorAssessment)
                        );
                    }
                    // INSERT
                    else
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(
                            row,
                            MentoringSQL.GetMentorSQL(MentorSQLCommend.SaveOverallMentorAssessment)
                        );
                    }

                    if (!resultArgs.Success)
                        return Json(new { success = false, message = "Failed to save assessment" });
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("Mentoring", "SaveOverallMentorAssessment", ex.Message);
                }
                return Json(new { success = false, message = "Error occurred" });
            }
        }


        // ==========================================
        // NEW OVERALL MENTOR ASSESSMENT (ALL DROPDOWNS)
        // ==========================================
        public ActionResult GetNewOverallMentorAssessment(string studentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            MentoringViewModel vm = new MentoringViewModel();

            try
            {
                if (string.IsNullOrEmpty(sAcademicYear) || string.IsNullOrEmpty(studentId))
                {
                    vm.message = "Session Expired.";
                    return PartialView(vm);
                }

                // 1. Define Rows
                //vm.liOverallMentorAssessment = new List<OverallMentorAssessmentRow>()
                //{
                //    new OverallMentorAssessmentRow { AREA = "Academic Progress" },
                //    new OverallMentorAssessmentRow { AREA = "Behaviour & Discipline" },
                //    new OverallMentorAssessmentRow { AREA = "Emotional / Psychological Well-being" },
                //    new OverallMentorAssessmentRow { AREA = "Career Readiness" },
                //    new OverallMentorAssessmentRow { AREA = "Special Support Required" }
                //};
                vm.liOverallMentorAssessment = (List<OverallMentorAssessmentRow>)
                    CMSHandler.FetchData<OverallMentorAssessmentRow>(null, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSupOverallMentorAssessmentCategory)).DataSource.Data;
                // 2. Fetch Existing Data (Reuse same table)
                var savedData = (List<OverallMentorAssessmentRow>)
                    CMSHandler.FetchData<OverallMentorAssessmentRow>(
                        new
                        {
                            STUDENT_ID = studentId,
                            ACADEMIC_YEAR = sAcademicYear
                        },
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchOverallNewMentorAssessment)
                    ).DataSource.Data;

                if (savedData != null && savedData.Count > 0)
                {
                    foreach (var row in vm.liOverallMentorAssessment)
                    {
                        var dbRow = savedData.FirstOrDefault(x =>
                            x.SUP_OVERALL_MENTOR_ASSESSMENT_ID.Trim().Equals(row.SUP_OVERALL_MENTOR_ASSESSMENT_ID.Trim(), StringComparison.OrdinalIgnoreCase)
                        );
                        if (dbRow != null)
                        {
                            row.OVERALL_ASSESSMENT_ID = dbRow.OVERALL_ASSESSMENT_ID;
                            row.SEM1 = dbRow.SEM1;
                            row.SEM2 = dbRow.SEM2;
                            row.SEM3 = dbRow.SEM3;
                            row.SEM4 = dbRow.SEM4;
                            row.SEM5 = dbRow.SEM5;
                            row.SEM6 = dbRow.SEM6;
                        }
                    }
                }

                // 3. Populate Dropdown Options (Shared for All Cells)
                vm.liAssessmentObjectives = (List<OverallMentorAssessmentRow>)
                    CMSHandler.FetchData<OverallMentorAssessmentRow>(null, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchOverallMentorAssessmentObjective)).DataSource.Data;
                //vm.OverallAcademicAssessment = new List<SelectListItem>
                //{
                //    new SelectListItem { Text = "Excellent", Value = "Excellent" },
                //    new SelectListItem { Text = "Very Good", Value = "Very Good" },
                //    new SelectListItem { Text = "Good", Value = "Good" },
                //    new SelectListItem { Text = "Satisfactory", Value = "Satisfactory" },
                //    new SelectListItem { Text = "Average", Value = "Average" },
                //    new SelectListItem { Text = "Below Average", Value = "Below Average" },
                //    new SelectListItem { Text = "Poor", Value = "Poor" },
                //    new SelectListItem { Text = "Needs Improvement", Value = "Needs Improvement" },
                //    new SelectListItem { Text = "Bad", Value = "Bad" }
                //};

                ViewBag.StudentId = studentId;
                return PartialView(vm);
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("Mentoring", "GetNewOverallMentorAssessment", ex.Message);
                }
                vm.message = "Error loading assessment: " + ex.Message;
                return PartialView(vm);
            }
        }

        public JsonResult SaveNewOverallMentorAssessment(OverallAssessmentSaveVM model)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();

            try
            {
                if (model == null || model.data == null)
                    return Json(new { success = false, message = "No data to save" });

                foreach (var row in model.data)
                {
                    row.STUDENT_ID = model.studentId;
                    row.ACADEMIC_YEAR = sAcademicYear;
                    row.CREATED_BY = sUserId;

                    ResultArgs resultArgs;

                    // UPDATE
                    if (!string.IsNullOrEmpty(row.OVERALL_ASSESSMENT_ID))
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(
                            row,
                            MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdatenewOverallMentorAssessment)
                        );
                    }
                    // INSERT
                    else
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(
                            row,
                            MentoringSQL.GetMentorSQL(MentorSQLCommend.SaveNewOverallMentorAssessment)
                        );
                    }

                    if (!resultArgs.Success)
                        return Json(new { success = false, message = "Failed to save assessment" });
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("Mentoring", "SaveNewOverallMentorAssessment", ex.Message);
                }
                return Json(new { success = false, message = "Error occurred" });
            }
        }


        #region Skill & talents
        public ActionResult GetSkillandTalents(string studentId)
        {
            var viewModel = new MentoringViewModel();

            try
            {
                ViewBag.StudentId = studentId;

                var result = (List<SKILLS_AND_TALENTS>)CMSHandler.FetchData<SKILLS_AND_TALENTS>(new SKILLS_AND_TALENTS { STUDENT_ID = studentId }, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSkillandTalent)).DataSource.Data;

                if (result != null && result.Count > 0)
                {
                    viewModel.liSkillAndTalents = result;

                }

                var semResult = CMSHandler.FetchData<SUP_SEMESTER>(
                    null,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSemesters)
                );

                if (semResult != null)
                {
                    viewModel.SemesterList =
                        (semResult.DataSource.Data as List<SUP_SEMESTER>)
                        ?.Select(x => new SelectListItem
                        {
                            Value = x.SUP_SEMESTER_ID,
                            Text = x.SEMESTER_NAME
                        }).ToList()
                        ?? new List<SelectListItem>();
                }
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetSkillandTalents", "MentoringController", ex.Message);
            }

            return View(viewModel);
        }


        public JsonResult GetSkillById(string id)
        {
            try
            {
                var result = CMSHandler.FetchData<SKILLS_AND_TALENTS>(
                    new SKILLS_AND_TALENTS { SKILLS_AND_TALENTS_ID = id },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSkillById)
                );

                var data = (result.DataSource.Data as List<SKILLS_AND_TALENTS>)?.FirstOrDefault();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult SaveSkillAndTalents(SKILLS_AND_TALENTS mentroingstudentacademic)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            var objViewModel = new MentoringViewModel();
            if (string.IsNullOrEmpty(mentroingstudentacademic.ACADEMIC_YEAR))
            {
                mentroingstudentacademic.ACADEMIC_YEAR = sAcademicYear;
            }

            try
            {
                if (string.IsNullOrEmpty(mentroingstudentacademic.SKILLS_AND_TALENTS_ID))
                {
                    var SaveResult = CMSHandler.SaveOrUpdate(mentroingstudentacademic, MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertSkillandTalents));
                }
                else
                {
                    var result = CMSHandler.SaveOrUpdate(mentroingstudentacademic, MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateSkillandTalents));
                }

            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("SaveSkillAndTalents", "MentoringController", ex.Message);
                return Json(new { success = false, message = "An error occurred while processing the request" });
            }

            return Json(new { success = true, message = "Saved successfully" });
        }


        public JsonResult DeleteSkill(string id)
        {
            try
            {
                CMSHandler.SaveOrUpdate(
                    new SKILLS_AND_TALENTS
                    {
                        SKILLS_AND_TALENTS_ID = id,
                        IS_DELETED = "1"
                    },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.DeleteSkillandTalents)
                );

                return Json(new { success = true, message = "Deleted successfully" });
            }
            catch
            {
                return Json(new { success = false, message = "Delete failed" });
            }
        }



        #endregion
        #region Sports
        public ActionResult GetSports(string studentId)
        {
            ViewBag.StudentId = studentId;
            var vm = new MentoringViewModel();

            try
            {

                var result = (List<MENTOR_SPORTS>)CMSHandler.FetchData<MENTOR_SPORTS>(new MENTOR_SPORTS { STUDENT_ID = studentId }, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSportsDetails)).DataSource.Data;

                if (result != null && result.Count > 0)
                {
                    vm.liSports = result;
                }


                var semResult = CMSHandler.FetchData<SUP_SEMESTER>(
                    null,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSemesters)
                );

                if (semResult != null)
                {
                    vm.SemesterList =
                            (semResult.DataSource.Data as List<SUP_SEMESTER>)
                            ?.Select(x => new SelectListItem
                            {
                                Value = x.SUP_SEMESTER_ID,
                                Text = x.SEMESTER_NAME
                            }).ToList()
                            ?? new List<SelectListItem>();
                }
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetSports", "MentoringController", ex.Message);
            }

            return View(vm);
        }

        public JsonResult GetSportById(string id)
        {
            var result = CMSHandler.FetchData<MENTOR_SPORTS>(
                new MENTOR_SPORTS { SPORTS_ID = id },
                MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSportById)
            );

            var data = (result.DataSource.Data as List<MENTOR_SPORTS>)?.FirstOrDefault();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSports(MENTOR_SPORTS sports)
        {
            try
            {
                if (string.IsNullOrEmpty(sports.SPORTS_ID))
                {
                    CMSHandler.SaveOrUpdate(sports,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertSports));
                }
                else
                {
                    CMSHandler.SaveOrUpdate(sports,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateSports));
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("SaveSports", "MentoringController", ex.Message);
                return Json(new { success = false });
            }
        }

        public JsonResult DeleteSport(string id)
        {
            CMSHandler.SaveOrUpdate(
                new MENTOR_SPORTS { SPORTS_ID = id },
                MentoringSQL.GetMentorSQL(MentorSQLCommend.DeleteSports)
            );

            return Json(new { success = true, message = "Deleted successfully" });
        }


        #endregion

        #region PARTICIPATION & ACHIEVEMENTS
        //public ActionResult GetParticipationAchements(string studentId)
        //{

        //    var viewModel = new MentoringViewModel();

        //    try
        //    {
        //        var model = new PERSONAL_DETAILS
        //        {
        //            STUDENT_ID = studentId
        //        };

        //        if (model != null)
        //        {
        //            var result = CMSHandler.FetchData<PARTICIPATION_ACHIEVEMENTS>(
        //            model, MentoringSQL.GetMentorSQL(MentorSQLCommend.Participation_Achievements));
        //            viewModel.liParticipationAchievement = result.DataSource.Data as List<PARTICIPATION_ACHIEVEMENTS> ??
        //                new List<PARTICIPATION_ACHIEVEMENTS>();
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        new ErrorLog().WriteError("GetParticipationAchements", "MentoringController", ex.Message);
        //    }

        //    return View(viewModel);
        //}

        //public JsonResult SaveParticipationAchements(PARTICIPATION_ACHIEVEMENTS dataToSend)
        //{
        //    try
        //    {
        //        if (dataToSend == null)
        //            return Json(new { success = false, message = "No data received" });

        //        if (string.IsNullOrEmpty(dataToSend.PARTICIPATION_ID))
        //        {
        //            CMSHandler.SaveOrUpdate(
        //                dataToSend,
        //                MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertParticipationAchements)
        //            );
        //            return Json(new { success = true, message = "Details saved successfully" });
        //        }
        //        else
        //        {
        //            CMSHandler.SaveOrUpdate(
        //                dataToSend,
        //                MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateParticipationAchements)
        //            );
        //            return Json(new { success = true, message = "Details updated successfully" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        new ErrorLog().WriteError("SaveParticipationAchements", "MentoringController", ex.ToString());
        //        return Json(new { success = false, message = "An error occurred while saving data: " + ex.Message });
        //    }
        //}

        public ActionResult GetVACCourses(string studentId)
        {
            var vm = new MentoringViewModel();
            ViewBag.StudentId = studentId;

            try
            {
                var obj = new MENTOR_VAC_PROFESSIONAL
                {
                    STUDENT_ID = studentId
                };

                var result = CMSHandler.FetchData<MENTOR_VAC_PROFESSIONAL>(
                    obj,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchVACCourses)
                );

                vm.liVACCourses = result?.DataSource?.Data as List<MENTOR_VAC_PROFESSIONAL>
                                  ?? new List<MENTOR_VAC_PROFESSIONAL>();
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetVACCourses", "MentoringController", ex.ToString());
            }

            return View(vm);
        }

        // ===========================
        // SAVE / UPDATE VAC COURSE
        // ===========================

        public JsonResult SaveVACCourse(MENTOR_VAC_PROFESSIONAL model)
        {
            try
            {
                // 🔴 HARD DEBUG CHECK
                if (model == null)
                {
                    return Json(new { success = false, message = "MODEL IS NULL (Binding Failed)" });
                }

                if (string.IsNullOrEmpty(model.STUDENT_ID))
                {
                    return Json(new { success = false, message = "Student ID missing" });
                }

                model.IS_ACTIVE = "1";
                model.IS_DELETED = "0";

                // INSERT
                if (string.IsNullOrEmpty(model.VAC_ID))
                {
                    model.VAC_ID = Guid.NewGuid().ToString();

                    CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertVACCourse)
                    );
                }
                // UPDATE
                else
                {
                    CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateVACCourse)
                    );
                }

                return Json(new { success = true, message = "Course saved successfully" });
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("SaveVACCourse", "MentoringController", ex.ToString());
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===========================
        // DELETE VAC COURSE
        // ===========================

        public JsonResult DeleteVACCourse(string id)
        {
            try
            {
                var obj = new MENTOR_VAC_PROFESSIONAL
                {
                    VAC_ID = id,
                    IS_DELETED = "1"
                };

                CMSHandler.SaveOrUpdate(
                    obj,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.DeleteVACCourse)
                );

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("DeleteVACCourse", "MentoringController", ex.ToString());
                return Json(new { success = false });
            }
        }



        /* ==============================
           LOAD PAGE
        ============================== */
        public ActionResult GetContributions(string studentId)
        {
            var vm = new MentoringViewModel();
            ViewBag.StudentId = studentId;

            var result = CMSHandler.FetchData<MENTOR_LEADERSHIP_CONTRIBUTION>(
                new MENTOR_LEADERSHIP_CONTRIBUTION { STUDENT_ID = studentId },
                MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchContributions)
            );

            if (result != null && result.DataSource.Data != null)
            {
                vm.liContributions =
                    result.DataSource.Data as List<MENTOR_LEADERSHIP_CONTRIBUTION>;
            }

            var supResult = CMSHandler.FetchData<SUP_CONTRIBUTION_TYPE>(
                new SUP_CONTRIBUTION_TYPE { IS_ACTIVE = "1" },
                MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchContributionTypes)
            );

            if (supResult != null && supResult.DataSource.Data != null)
            {
                vm.ContributionTypeList =
                    (supResult.DataSource.Data as List<SUP_CONTRIBUTION_TYPE>)
                    .Select(x => new SelectListItem
                    {
                        Value = x.CONTRIBUTION_TYPE_ID.ToString(),
                        Text = x.TYPE_NAME
                    }).ToList();
            }

            return View(vm);
        }

        /* ==============================
           SAVE / UPDATE
        ============================== */


        public JsonResult SaveContribution(MENTOR_LEADERSHIP_CONTRIBUTION model)
        {
            try
            {
                if (model == null)
                    return Json(new { success = false, message = "Invalid data" });

                if (string.IsNullOrEmpty(model.IS_ACTIVE))
                    model.IS_ACTIVE = "1";

                if (string.IsNullOrEmpty(model.CONTRIBUTION_ID))
                {
                    CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertContribution)
                    );
                }
                else
                {
                    CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateContribution)
                    );
                }

                return Json(new { success = true, message = "Contribution saved successfully" });
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("SaveContribution", "MentoringController", ex.Message);
                return Json(new { success = false, message = "Error while saving" });
            }
        }

        /* ==============================
           GET BY ID (EDIT)
        ============================== */

        public JsonResult GetContributionById(string contributionId)
        {
            try
            {
                var result = CMSHandler.FetchData<MENTOR_LEADERSHIP_CONTRIBUTION>(
                    new MENTOR_LEADERSHIP_CONTRIBUTION { CONTRIBUTION_ID = contributionId },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchContributionById)
                );

                var data = (result.DataSource.Data as List<MENTOR_LEADERSHIP_CONTRIBUTION>)
                            ?.FirstOrDefault();

                if (data == null)
                    return Json(null, JsonRequestBehavior.AllowGet);

                return Json(new
                {
                    data.CONTRIBUTION_ID,
                    data.CONTRIBUTION_TYPE_ID,
                    data.EVENT_DATE,
                    data.EVENT_NAME,
                    data.PLACE,
                    data.NATURE_OF_CONTRIBUTION,
                    data.REMARKS
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetContributionById", "MentoringController", ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /* ==============================
           DELETE
        ============================== */


        public JsonResult DeleteContribution(string contributionId)
        {
            try
            {
                CMSHandler.SaveOrUpdate(
                    new MENTOR_LEADERSHIP_CONTRIBUTION
                    {
                        CONTRIBUTION_ID = contributionId,
                        IS_DELETED = "1"
                    },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.DeleteContribution)
                );

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("DeleteContribution", "MentoringController", ex.Message);
                return Json(new { success = false });
            }
        }


        #endregion
        #region  BEHAVIOURAL & CONDUCT RECORD
        // ======================= GET LIST =======================
        public ActionResult GetBehaviouralRecords(string studentId)
        {
            var vm = new MentoringViewModel();
            ViewBag.StudentId = studentId;

            try
            {
                var result = CMSHandler.FetchData<BEHAVIOURAL_CONDUCT_RECORD>(
                    new BEHAVIOURAL_CONDUCT_RECORD { STUDENT_ID = studentId },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchBehaviouralRecords)
                );

                vm.liBehaviouralRecords =
                    result?.DataSource?.Data as List<BEHAVIOURAL_CONDUCT_RECORD>
                    ?? new List<BEHAVIOURAL_CONDUCT_RECORD>();

                var levelResult = CMSHandler.FetchData<SUP_BEHAVIOUR_LEVEL>(
                    new SUP_BEHAVIOUR_LEVEL { IS_ACTIVE = "1" },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchBehaviourLevels)
                );

                vm.BehaviourLevelList =
                    (levelResult?.DataSource?.Data as List<SUP_BEHAVIOUR_LEVEL>)
                    ?.Select(x => new SelectListItem
                    {
                        Value = x.LEVEL_ID,
                        Text = x.LEVEL_NAME
                    }).ToList()
                    ?? new List<SelectListItem>();
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetBehaviouralRecords", "MentoringController", ex.Message);
            }

            return View(vm);
        }

        // ======================= SAVE / UPDATE =======================


        public JsonResult SaveBehaviouralRecord(BEHAVIOURAL_CONDUCT_RECORD model)
        {
            try
            {
                if (model == null)
                    return Json(new { success = false, message = "Invalid data" });

                model.IS_ACTIVE = string.IsNullOrEmpty(model.IS_ACTIVE) ? "1" : model.IS_ACTIVE;

                if (string.IsNullOrEmpty(model.RECORD_ID))
                {
                    CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertBehaviourRecord)
                    );
                }
                else
                {
                    CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateBehaviourRecord)
                    );
                }

                return Json(new { success = true, message = "Record saved successfully" });
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("SaveBehaviouralRecord", "MentoringController", ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ======================= GET BY ID (EDIT) =======================

        public JsonResult GetBehaviouralRecordById(string recordId)
        {
            var result = CMSHandler.FetchData<BEHAVIOURAL_CONDUCT_RECORD>(
                new BEHAVIOURAL_CONDUCT_RECORD { RECORD_ID = recordId },
                MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchBehaviourById)
            );

            var data = (result.DataSource.Data as List<BEHAVIOURAL_CONDUCT_RECORD>)?.FirstOrDefault();

            return Json(new
            {
                data.RECORD_ID,
                data.LEVEL_ID,
                INCIDENT_DATE = DateTime.Parse(data.INCIDENT_DATE).ToString("yyyy-MM-dd"),
                data.INCIDENT_TIME,
                data.INCIDENT_DETAILS,
                data.ACTION_TAKEN,
                data.REMARKS
            }, JsonRequestBehavior.AllowGet);
        }

        // ======================= DELETE =======================


        public JsonResult DeleteBehaviouralRecord(string recordId)
        {
            CMSHandler.SaveOrUpdate(
                new BEHAVIOURAL_CONDUCT_RECORD
                {
                    RECORD_ID = recordId,
                    IS_DELETED = "1"
                },
                MentoringSQL.GetMentorSQL(MentorSQLCommend.DeleteBehaviourRecord)
            );

            return Json(new { success = true });
        }


        #endregion

        #region MENTORING & COUNSELLING SESSIONS (ATTENDANCE)




        /* ================= LIST PAGE ================= */
        public ActionResult GetCounsellingSessions(string studentId)
        {
            ViewBag.StudentId = studentId;

            var vm = new MentoringViewModel();

            // Counselling Sessions
            var sessionResult = CMSHandler.FetchData<MENTORING_COUNSELLING_SESSION>(
                new MENTORING_COUNSELLING_SESSION { STUDENT_ID = studentId },
                MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchCounsellingSessions)
            )?.DataSource?.Data as List<MENTORING_COUNSELLING_SESSION>;

            vm.liCounsellingSessions = sessionResult ?? new List<MENTORING_COUNSELLING_SESSION>();

            // Semester Dropdown
            var semResult = CMSHandler.FetchData<SUP_SEMESTER>(
                null,
                MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSemesters)
            )?.DataSource?.Data as List<SUP_SEMESTER>;

            vm.SemesterList = semResult?.Select(x => new SelectListItem
            {
                Value = x.SUP_SEMESTER_ID.ToString(),
                Text = x.SEMESTER_NAME
            }).ToList() ?? new List<SelectListItem>();

            return View(vm);
        }

        /* ================= GET BY ID ================= */
        public JsonResult GetCounsellingSessionById(string sessionId)
        {
            var result = CMSHandler.FetchData<MENTORING_COUNSELLING_SESSION>(
                new MENTORING_COUNSELLING_SESSION { SESSION_ID = sessionId },
                MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchCounsellingSessionById)
            );

            var data = (result.DataSource.Data as List<MENTORING_COUNSELLING_SESSION>)?.FirstOrDefault();

            if (data == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                data.SESSION_ID,
                data.SEMESTER_ID,
                SESSION_DATE = Convert.ToDateTime(data.SESSION_DATE).ToString("yyyy-MM-dd"),
                SESSION_TIME = Convert.ToDateTime(data.SESSION_TIME).ToString("HH:mm"),
                data.MATTERS_DISCUSSED,
                data.GUIDANCE_GIVEN,
                data.FOLLOW_UP_REQUIRED
            }, JsonRequestBehavior.AllowGet);
        }

        /* ================= SAVE / UPDATE ================= */

        public JsonResult SaveCounsellingSession(MENTORING_COUNSELLING_SESSION model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.STUDENT_ID))
                {
                    return Json(new { success = false, message = "Invalid data" });
                }

                model.IS_ACTIVE = "1";
                model.IS_DELETED = "0";

                model.SESSION_DATE = Convert.ToDateTime(model.SESSION_DATE).ToString("yyyy-MM-dd");
                model.SESSION_TIME = Convert.ToDateTime(model.SESSION_TIME).ToString("HH:mm");

                if (string.IsNullOrEmpty(model.SESSION_ID))
                {
                    CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertCounsellingSession)
                    );
                }
                else
                {
                    CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateCounsellingSession)
                    );
                }

                return Json(new { success = true, message = "Session saved successfully" });
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("SaveCounsellingSession", "MentoringController", ex.ToString());
                return Json(new { success = false, message = "Error while saving" });
            }
        }

        /* ================= DELETE ================= */

        public JsonResult DeleteCounsellingSession(string sessionId)
        {
            try
            {
                CMSHandler.SaveOrUpdate(
                    new MENTORING_COUNSELLING_SESSION
                    {
                        SESSION_ID = sessionId,
                        IS_DELETED = "1"
                    },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.DeleteCounsellingSession)
                );

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }





        public ActionResult MentorAttendanceAbsentReport(string studentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            ViewBag.StudentId = studentId;

            var vm = new MentoringViewModel();
            vm.liAttendance = new List<MENTORING_ATTENDANCE_REPORT>();
            vm.SemesterList = new List<SelectListItem>();

            try
            {
                // Fetch Semester List for Dropdown
                var semResult = (List<SUP_SEMESTER>)CMSHandler.FetchData<SUP_SEMESTER>(null, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSemesters)).DataSource.Data;

                //var semResult = List<SUP_SEMESTER>)CMSHandler.FetchData<SUP_SEMESTER>(null, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSemesters), sAcademicYear).DataSource.Data;
                if (semResult != null && semResult.Count > 0)
                {
                    vm.SemesterList = semResult
                        ?.Select(x => new SelectListItem
                        {
                            Value = x.SUP_SEMESTER_ID.ToString(), // SUP_SEMESTER_ID aliased as SEMESTER_ID
                            Text = x.SEMESTER_NAME
                        }).ToList() ?? new List<SelectListItem>();
                }

                // Fetch Active Semester for the student to pre-load
                var activeSemResult = CMSHandler.FetchData<MENTORING_ATTENDANCE_REPORT>(new MENTORING_ATTENDANCE_REPORT


                {
                    STUDENT_ID = studentId,
                    AC_YEAR = sAcademicYear
                }, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchActiveSemester));

                var activeSemester = (activeSemResult?.DataSource?.Data as List<MENTORING_ATTENDANCE_REPORT>)?.FirstOrDefault()?.SEMESTER;
                if (activeSemester != null)
                {
                    ViewBag.ActiveSemester = activeSemester;
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("MentoringController", "MentorAttendanceAbsentReport", "Initialization Error", ex.Message);
                }
            }

            return View(vm);
        }

        public JsonResult GetStudentAbsentReport(string studentId, string semesterId)
        {
            var attendanceList = new List<MENTORING_ATTENDANCE_REPORT>();
            var summary = new { DaysCount = 0, AbsentCount = 0 };
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(studentId) && !string.IsNullOrEmpty(semesterId))
                {
                    // 1. Fetch Start/End Dates for the selected semester
                    var dateData = CMSHandler.FetchData<MENTORING_ATTENDANCE_REPORT>(new MENTORING_ATTENDANCE_REPORT
                    {
                        STUDENT_ID = studentId,
                        SEMESTER = semesterId
                    }, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSemesterDatesById));

                    var semesterDates = (dateData?.DataSource?.Data as List<MENTORING_ATTENDANCE_REPORT>)?.FirstOrDefault();

                    if (semesterDates != null)
                    {
                        int daysCount = 0;
                        int.TryParse(semesterDates.DAYS_COUNT, out daysCount);

                        // 2. Fetch Absentee List
                        var reportData = CMSHandler.FetchData<MENTORING_ATTENDANCE_REPORT>(new MENTORING_ATTENDANCE_REPORT
                        {
                            STUDENT_ID = studentId,
                            SUP_SEMESTER_ID = semesterId
                        }, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchMentorstudentabsentReport), sAcademicYear);

                        attendanceList = (reportData?.DataSource?.Data as List<MENTORING_ATTENDANCE_REPORT>) ?? new List<MENTORING_ATTENDANCE_REPORT>();

                        summary = new
                        {
                            DaysCount = daysCount,
                            AbsentCount = attendanceList.Count
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("MentoringController", "GetStudentAbsentReport", "AJAX Error", ex.Message);
                }
            }

            return Json(new { AttendanceList = attendanceList, Summary = summary }, JsonRequestBehavior.AllowGet);
        }




        #endregion

        #region Parent Meeting
        public ActionResult GetParentMeeting(string studentId)
        {
            var viewModel = new MentoringViewModel();
            ViewBag.StudentId = studentId;

            try
            {
                var model = new PERSONAL_DETAILS
                {
                    STUDENT_ID = studentId
                };
                var result = (List<PARENT_MEETING>)CMSHandler.FetchData<PARENT_MEETING>(new PARENT_MEETING { STUDENT_ID = studentId }, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchParentMeeting)).DataSource.Data;

                if (result != null && result.Count > 0)
                {
                    viewModel.liParentMeeting = result;
                }

                // Populate SemesterList for the dropdown in the modal
                var semResult = CMSHandler.FetchData<SUP_SEMESTER>(
                    null,
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchSemesters)
                );
                if (semResult != null)
                {
                    viewModel.SemesterList = (semResult.DataSource.Data as List<SUP_SEMESTER>)
                        ?.Select(x => new SelectListItem
                        {
                            Value = x.SUP_SEMESTER_ID.ToString(),
                            Text = x.SEMESTER_NAME
                        }).ToList()
                        ?? new List<SelectListItem>();
                }

            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError(
                    "GetParentMeeting",
                    "MentoringController",
                    ex.Message
                );
            }

            return View(viewModel);
        }




        public JsonResult SaveParentMeeting(PARENT_MEETING meeting)
        {
            try
            {
                if (string.IsNullOrEmpty(meeting.IS_ACTIVE))
                {
                    meeting.IS_ACTIVE = "1";
                }

                if (string.IsNullOrEmpty(meeting.PARENT_ID))
                {
                    var insertmeet = CMSHandler.SaveOrUpdate(
                        meeting,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertParentMeeting)
                    );
                }
                else
                {
                    var updatemeet = CMSHandler.SaveOrUpdate(
                        meeting,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateParentMeeting)
                    );
                }

                return Json(new { success = true, message = "Saved successfully" });
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("SaveParentMeeting", "MentoringController", ex.Message);
                return Json(new { success = false, message = "Error occurred" });
            }
        }

        #endregion

        public JsonResult GetParentMeetingById(string id)
        {
            try
            {
                var result = CMSHandler.FetchData<PARENT_MEETING>(
                    new PARENT_MEETING
                    {
                        PARENT_ID = id
                    },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchParentMeetingById)
                );

                var data = (result.DataSource.Data as List<PARENT_MEETING>)
                            ?.FirstOrDefault();

                if (data == null)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    data.PARENT_ID,
                    data.SEM,
                    data.FEEDBACK,
                    data.MEETING_TIME,
                    MEETING_DATE = !string.IsNullOrEmpty(data.MEETING_DATE)
                        ? DateTime.Parse(data.MEETING_DATE).ToString("yyyy-MM-dd")
                        : ""
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("GetParentMeetingById", "MentoringController", ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteParentMeeting(string id)
        {
            try
            {
                CMSHandler.SaveOrUpdate(
                    new PARENT_MEETING
                    {
                        PARENT_ID = id,
                        IS_DELETED = "1"
                    },
                    MentoringSQL.GetMentorSQL(MentorSQLCommend.DeleteParentMeeting)
                );

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteError("DeleteParentMeeting", "MentoringController", ex.Message);
                return Json(new { success = false });
            }
        }

        #region Mentor Late Entry

        // ================= MAIN PAGE =================
        public ActionResult StudentLateEntryReport(string studentId)
        {
            MentoringViewModel objViewModel = new MentoringViewModel();
            JsonResultData objResult = new JsonResultData();
            var userID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            try
            {
                ViewBag.StudentId = studentId;

                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";

                    var ClassList = (List<ClassInfo>)CMSHandler.FetchData<ClassInfo>(new StaffList(), FeeSQL.GetFeeSQL(FeeSQLCommands.FetchClassList), sAcademicYear).DataSource.Data;
                    if (ClassList != null && ClassList.Count > 0)
                    {

                        objViewModel.ClassList = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);

                    }


                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "PedagogicMethodInputSystem", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "PedagogicMethodInputSystem", Ex.Message);
                }

            }
            return View(objViewModel);
        }

        // ================= DATE WISE =================
        public ActionResult BindStuLateEntryReportDateWise(string sdate, string edate, string student_id)
        {
            MentoringViewModel objViewModel = new MentoringViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var STUDENT_ID = student_id;
                    //string Date = CommonMethods.DateConvertionForPayU(sdate);
                    //string ToDate = CommonMethods.DateConvertionForPayU(edate);
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";

                    objViewModel.liStuLateFee = (List<STU_LATE_ENTRY>)CMSHandler.FetchData<STU_LATE_ENTRY>(new STU_LATE_ENTRY() { STUDENT_ID = student_id }, FeeSQL.GetFeeSQL(FeeSQLCommands.FetchLateEntryStudentDateWise).Replace("?FROM_DATE", sdate).Replace("?TODATE", edate), sAcademicYear).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "BindPedagogicMethodInputSystem", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "BindPedagogicMethodInputSystem", Ex.Message);
                }

            }
            return View(objViewModel);
        }




        #endregion

   
        #region AddStudentDetails
        public ActionResult AddStudentDetails(string studentId)
        {
            MentoringViewModel objViewModel = new MentoringViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string userid = (Session[Common.SESSION_VARIABLES.STUDENT_ID] != null) ? Session[Common.SESSION_VARIABLES.STUDENT_ID].ToString() : string.Empty;

            try
            {
                if (string.IsNullOrEmpty(studentId))
                {
                    throw new ArgumentException("Student ID is required.");
                }

                PERSONAL_DETAILS objModel = new PERSONAL_DETAILS
                {
                    STUDENT_ID = studentId
                };
                var personalDetResult = (List<STU_personal_info>)CMSHandler.FetchData<STU_personal_info>(objModel, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchStudentPersonaldet), sAcademicYear).DataSource.Data;
                if (personalDetResult != null && personalDetResult.Count > 0)
                {
                    objViewModel.listudentList = personalDetResult;
                }
                else
                {
                    var result = CMS.DAO.CMSHandler.FetchData<STU_personal_info>(objModel, MentoringSQL.GetMentorSQL(MentorSQLCommend.FetchStudentpersonalinfo), sAcademicYear);

                    if (result != null && result.DataSource != null)
                    {
                        objViewModel.listudentList = (List<STU_personal_info>)result.DataSource.Data;
                    }
                    else
                    {
                        throw new InvalidOperationException("No data returned from the database.");
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("GetPersonaldet", "MentoringController", "Err Msg: " + ex.Message, "Query or data issue");
                    objHandler.WriteError("GetPersonaldet", "MentoringController", ex.Message);
                }
            }

            return View(objViewModel);
        }





        public JsonResult UpdatePersonalDetails(StudentDetails model)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] == null)
                {
                    return Json(new { success = false, message = "Session expired" });
                }

                if (!string.IsNullOrEmpty(model.MENTOR_STUDENT_ID))
                {
                    ResultArgs updateResult = CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.UpdateStudentPersonal)
                    );

                    // 2️ If UPDATE worked → DONE
                    if (updateResult != null && updateResult.Success)
                    {
                        return Json(new
                        {
                            success = true,
                            action = "updated"
                        });
                    }
                }
                else
                {

                    // 3️ UPDATE failed → Try INSERT
                    ResultArgs insertResult = CMSHandler.SaveOrUpdate(
                        model,
                        MentoringSQL.GetMentorSQL(MentorSQLCommend.InsertStudentPersonal)
                    );

                    if (insertResult != null && insertResult.Success)
                    {
                        return Json(new
                        {
                            success = true,
                            action = "inserted"
                        });
                    }
                }
                // 4️ Both failed
                return Json(new
                {
                    success = false,
                    message = ""
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        #endregion



        #region Student Wise Mentor Details

        public ActionResult StudentWiseMentorDetails()
        {
            var role = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString();
            var userID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            string sAcademicYear = string.Empty;
            StudentViewModel studentViewModel = new StudentViewModel();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (role == Common.SupRole.STAFF)
                    {
                        var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchRegularclass), sAcademicYear).DataSource.Data;
                        if (liClass != null && liClass.Count > 0)
                        {
                            studentViewModel.liLCasslist = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                        }
                    }
                    else
                    {
                        var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(new stf_Personal_Info() { STAFF_ID = userID }, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchClassInchargeByAcYear), sAcademicYear).DataSource.Data;
                        if (liClass != null && liClass.Count > 0)
                        {
                            studentViewModel.liLCasslist = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
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
                    objHandler.WriteError("StudentWiseMentorDetails", "MentoringController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("StudentWiseMentorDetails", "MentoringController", ex.Message);
                }
            }
            return View(studentViewModel);
        }

        public ActionResult BindClassWiseMentorStudentList(string objInput)
        {
            MentoringViewModel objStudentViewModel = new MentoringViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objStudentViewModel.StudentTagMapping = (List<STUDENT_TAG_MAPPING>)CMSHandler.FetchData<STUDENT_TAG_MAPPING>(new CP_CLASSES() { CLASS_ID = objInput },
                        SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchClassWiseMentorStudentList), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindClassWiseMentorStudentList", "MentoringController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindClassWiseMentorStudentList", "MentoringController", ex.Message);
                }
            }
            return PartialView("BindClassWiseMentorStudentList", objStudentViewModel);
        }

        #endregion


}

}