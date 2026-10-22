using CMS.DAO;
using CMS.Models;
using CMS.SQL.Attendance;
using CMS.SQL.Dashboard;
using CMS.SQL.Examination;
using CMS.SQL.Fee;
using CMS.SQL.Library;
using CMS.SQL.Mentor;
using CMS.SQL.Placement;
using CMS.SQL.StaffSQL;
using CMS.SQL.Student;
using CMS.SQL.SupportData;
using CMS.SQL.TimeTable;
using CMS.Utility;
using CMS.ViewModel;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using OfficeOpenXml;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Windows.Resources;

namespace CMS.Controllers
{
    public class MentorController : Controller
    {
        string sResult = string.Empty;

        DataValue dv = new DataValue();
        ResultArgs resultArgs = new ResultArgs();
        JsonResultData objJsonData = new JsonResultData();
        #region Student Mentor
        public ActionResult mentoring_student_list()
        {
            SupportDataViewModel viewmodal = new SupportDataViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string userID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                viewmodal.liMentorStudent = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new MENTORING_STUDENT_LIST { STUDENT_ID = userID }, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchMentorStudentReportList), sAcademicYear).DataSource.Data;

            }
            return View(viewmodal);
        }

        #endregion

        #region Mentoring Student List for staff
        public ActionResult MentorStudentListForStaff()
        {
            SupportDataViewModel viewmodal = new SupportDataViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string userID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                viewmodal.liMentorForStaff = (List<MENTORING_STUDENT_LIST_FOR_STAFF>)CMSHandler.FetchData<MENTORING_STUDENT_LIST_FOR_STAFF>(new MENTORING_STUDENT_LIST_FOR_STAFF { STAFF_ID = userID }, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchMentorStudentForStaff), sAcademicYear).DataSource.Data;
            }
            return View(viewmodal);
        }
        public ActionResult ViewStudentListByStudentId(string sStudentId)
        {
            StudentViewModel ObjViewModel = new StudentViewModel();
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sSQL = string.Empty;
                    sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentDetailsbyStudentId);
                    ObjViewModel.liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new STU_PERSONAL_INFO() { STUDENT_ID = sStudentId }, sSQL, sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("MentorController", "ViewStudentListByStudentId", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("MentorController", "ViewStudentListByStudentId", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return View(ObjViewModel);
        }

        public ActionResult ExportallData()
        {
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

                if (string.IsNullOrEmpty(sAcademicYear))
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }

                var liMentorForStaff = (List<MENTORING_STUDENT_LIST_FOR_STAFF>)CMSHandler.FetchData<MENTORING_STUDENT_LIST_FOR_STAFF>(new MENTORING_STUDENT_LIST_FOR_STAFF { STAFF_ID = sStaffId }, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchMentorStudentForStaff), sAcademicYear).DataSource.Data;

                if (liMentorForStaff == null || liMentorForStaff.Count == 0)
                {
                    TempData["Message"] = "No students allocated.";
                    return RedirectToAction("MentorStudentListForStaff");
                }

                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets.Add("Student Information");

                    int col = 1;
                    ws.Cells[1, col++].Value = "S.No";
                    ws.Cells[1, col++].Value = "First Name";
                    ws.Cells[1, col++].Value = "Roll No";
                    ws.Cells[1, col++].Value = "Register No";
                    ws.Cells[1, col++].Value = "Admission No";
                    ws.Cells[1, col++].Value = "Admission Date";
                    ws.Cells[1, col++].Value = "Programme";
                    ws.Cells[1, col++].Value = "Class";
                    ws.Cells[1, col++].Value = "Batch";
                    ws.Cells[1, col++].Value = "Last School";
                    ws.Cells[1, col++].Value = "Last Studied Place";
                    ws.Cells[1, col++].Value = "Tamil Name";
                    ws.Cells[1, col++].Value = "Gender";
                    ws.Cells[1, col++].Value = "Date of Birth";
                    ws.Cells[1, col++].Value = "Religion";
                    ws.Cells[1, col++].Value = "Community";
                    ws.Cells[1, col++].Value = "Caste";
                    ws.Cells[1, col++].Value = "Mother Tongue";
                    ws.Cells[1, col++].Value = "Blood Group";
                    ws.Cells[1, col++].Value = "Nationality";
                    ws.Cells[1, col++].Value = "Residency Type";
                    ws.Cells[1, col++].Value = "Specially Abled";
                    ws.Cells[1, col++].Value = "Father Name";
                    ws.Cells[1, col++].Value = "Father Occupation";
                    ws.Cells[1, col++].Value = "Father Income";
                    ws.Cells[1, col++].Value = "Father Mobile";
                    ws.Cells[1, col++].Value = "Mother Name";
                    ws.Cells[1, col++].Value = "Mother Occupation";
                    ws.Cells[1, col++].Value = "Mother Income";
                    ws.Cells[1, col++].Value = "Email";
                    ws.Cells[1, col++].Value = "Mobile No";
                    ws.Cells[1, col++].Value = "Address Line 1";
                    ws.Cells[1, col++].Value = "City";
                    ws.Cells[1, col++].Value = "District";
                    ws.Cells[1, col++].Value = "State";
                    ws.Cells[1, col++].Value = "Pincode";
                    ws.Cells[1, col++].Value = "Aadhar Number";
                    ws.Cells[1, col++].Value = "HSS No";
                    ws.Cells[1, col++].Value = "Group Code";
                    ws.Cells[1, col++].Value = "EMIS No";
                    ws.Cells[1, col++].Value = "Study Board";
                    ws.Cells[1, col++].Value = "HSS Total";
                    ws.Cells[1, col++].Value = "Medium of Study";
                    ws.Cells[1, col++].Value = "Extra Curricular";
                    ws.Cells[1, col++].Value = "Sports";
                    ws.Cells[1, col++].Value = "First Graduate";
                    ws.Cells[1, col++].Value = "Bank Name";
                    ws.Cells[1, col++].Value = "Branch Name";
                    ws.Cells[1, col++].Value = "Account No";
                    ws.Cells[1, col++].Value = "IFSC Code";
                    ws.Cells[1, col++].Value = "UMIS No";
                    ws.Cells[1, col++].Value = "Enrollment No";

                    using (var range = ws.Cells[1, 1, 1, col - 1])
                    {
                        range.Style.Font.Bold = true;
                    }

                    int row = 2;
                    string sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentDetailsbyStudentId);
                    int sno = 1;

                    foreach (var studentItem in liMentorForStaff)
                    {
                        var liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new STU_PERSONAL_INFO() { STUDENT_ID = studentItem.STUDENT_ID }, sSQL, sAcademicYear).DataSource.Data;
                        if (liStudent != null && liStudent.Count > 0)
                        {
                            var s = liStudent.FirstOrDefault();
                            col = 1;
                            ws.Cells[row, col++].Value = sno++;
                            ws.Cells[row, col++].Value = s.FIRST_NAME;
                            ws.Cells[row, col++].Value = s.ROLL_NO;
                            ws.Cells[row, col++].Value = s.REGISTER_NO;
                            ws.Cells[row, col++].Value = s.ADMISSION_NO;
                            ws.Cells[row, col++].Value = s.ADMISSION_DATE;
                            ws.Cells[row, col++].Value = s.PROGRAMME_NAME;
                            ws.Cells[row, col++].Value = s.CLASS_NAME;
                            ws.Cells[row, col++].Value = s.BATCH;
                            ws.Cells[row, col++].Value = s.LAST_SCHOOL_OR_COLLEGE;
                            ws.Cells[row, col++].Value = s.LAST_STUDIED_PLACE;
                            ws.Cells[row, col++].Value = s.TAMIL_NAME;
                            ws.Cells[row, col++].Value = s.GENDER;
                            ws.Cells[row, col++].Value = s.DATE_OF_BIRTH;
                            ws.Cells[row, col++].Value = s.RELIGION;
                            ws.Cells[row, col++].Value = s.COMMUNITY;
                            ws.Cells[row, col++].Value = s.CASTE;
                            ws.Cells[row, col++].Value = s.MOTHER_TONGUE;
                            ws.Cells[row, col++].Value = s.BLOOD_GROUP;
                            ws.Cells[row, col++].Value = s.NATIONALITY;
                            ws.Cells[row, col++].Value = s.RESIDENCY_TYPE;
                            ws.Cells[row, col++].Value = s.SPECIALLY_ABLED;
                            ws.Cells[row, col++].Value = s.FATHER_NAME;
                            ws.Cells[row, col++].Value = s.FR_OCCUPATION;
                            ws.Cells[row, col++].Value = s.FR_INCOME;
                            ws.Cells[row, col++].Value = s.FR_MOBILE;
                            ws.Cells[row, col++].Value = s.MOTHER_NAME;
                            ws.Cells[row, col++].Value = s.MO_OCCUPATION;
                            ws.Cells[row, col++].Value = s.MO_INCOME;
                            ws.Cells[row, col++].Value = s.STU_EMAILID;
                            ws.Cells[row, col++].Value = s.STU_MOBILENO;
                            ws.Cells[row, col++].Value = s.ADDRESS_LINE1;
                            ws.Cells[row, col++].Value = s.CITY;
                            ws.Cells[row, col++].Value = s.DISTRICT;
                            ws.Cells[row, col++].Value = s.STATE_NAME;
                            ws.Cells[row, col++].Value = s.PINCODE;
                            ws.Cells[row, col++].Value = s.AADHAR_NUMBER;
                            ws.Cells[row, col++].Value = s.HSS_NO;
                            ws.Cells[row, col++].Value = s.HSS_GROUP_CODE;
                            ws.Cells[row, col++].Value = s.EMIS_NO;
                            ws.Cells[row, col++].Value = s.STUDY_BOARD;
                            ws.Cells[row, col++].Value = s.HSS_TOTAL;
                            ws.Cells[row, col++].Value = s.MEDIUM_STUDY;
                            ws.Cells[row, col++].Value = s.EXTRA_CURRRICULAR;
                            ws.Cells[row, col++].Value = s.SPORTS;
                            ws.Cells[row, col++].Value = s.FIRSTGRADUATE;
                            ws.Cells[row, col++].Value = s.BANK_NAME;
                            ws.Cells[row, col++].Value = s.BRANCH_NAME;
                            ws.Cells[row, col++].Value = s.ACCOUNT_NO;
                            ws.Cells[row, col++].Value = s.IFSC_CODE;
                            ws.Cells[row, col++].Value = s.UMIS_NO;
                            ws.Cells[row, col++].Value = s.ENROLLMENT_NO;

                            row++;
                        }
                    }
                    // ws.Cells[ws.Dimension.Address].AutoFitColumns();
                    using (var stream = new MemoryStream())
                    {
                        package.SaveAs(stream);
                        string fileName = "MentorStudentAllData_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".xlsx";
                        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(stream.ToArray(), contentType, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("MentorController", "ExportallData", ex.Message);
                }
                return Content("Error: " + ex.ToString());
            }
        }

        #endregion

        #region Mentoring Staff Allocation
        public ActionResult MentoringAllocation()
        {
            string sAcademicYear = string.Empty;
            TransferCertificate objTransferCertificate = new TransferCertificate();
            sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    List<Sup_Shift> liShift = new List<Sup_Shift>();
                    liShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    objTransferCertificate.ShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    // Program ...
                    DataTable dtProgram = new DataTable();
                    dtProgram = SupportDataMethod.FetchProgram(sAcademicYear).DataSource.Table;
                    List<cp_Program_2017> liProgram = new List<cp_Program_2017>();
                    if (dtProgram != null && dtProgram.Rows.Count > 0)
                    {
                        liProgram = (from DataRow dr in dtProgram.Rows
                                     select new cp_Program_2017()
                                     {
                                         PROGRAMME_ID = dr[Common.CP_PROGRAMME_2017.PROGRAMME_ID].ToString(),
                                         PROGRAMME_NAME = dr[Common.CP_PROGRAMME_2017.PROGRAMME_NAME].ToString()
                                     }).ToList();
                        //objTransferCertificate.ProgramName = new SelectList(liProgram, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    }
                    // Fetch class ....
                    DataTable dtClass = new DataTable();

                    dtClass = SupportDataMethod.FetchClass(sAcademicYear).DataSource.Table;
                    List<cp_Classes_2017> liClass = new List<cp_Classes_2017>();
                    if (dtClass != null && dtClass.Rows.Count > 0)
                    {
                        liClass = (from DataRow dr in dtClass.Rows
                                   select new cp_Classes_2017()
                                   {
                                       CLASS_ID = dr[Common.CP_CLASSES_2017.CLASS_ID].ToString(),
                                       CLASS_NAME = dr[Common.CP_CLASSES_2017.CLASS_NAME].ToString()
                                   }).ToList();
                        //objTransferCertificate.ClassName = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                    var AcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcademicYear != null && AcademicYear.Count > 0)
                    {
                        objTransferCertificate.AcademicYear = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var classYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data;
                    if (classYear != null && classYear.Count > 0)
                    {
                        objTransferCertificate.ClassYear = new SelectList(classYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                    }
                    var programmeLevel = (List<SUP_PROGRAMME_LEVEL>)CMSHandler.FetchData<SUP_PROGRAMME_LEVEL>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel)).DataSource.Data;
                    if (programmeLevel != null && programmeLevel.Count > 0)
                    {
                        objTransferCertificate.programmeLevel = new SelectList(programmeLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StudentController", "TCPrintView", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StudentController", "TCPrintView", ex.Message);
                    }
                    sResult = "Fail To Fetch Record(s) ...!";
                    return Json(sResult, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(objTransferCertificate);
        }

        public JsonResult SaveStaffMetoringAllotment(string sAcademicYear, string sProgramName, string sShift, string sProgramLevel, string classyear)
        {
            var AcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sResult = string.Empty;
            var ObjJsonData = new JsonResultData();

            try
            {
                if (!string.IsNullOrEmpty(AcademicYear))
                {
                    if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sProgramName) && !string.IsNullOrEmpty(sShift) && !string.IsNullOrEmpty(sProgramLevel))
                    {
                        var sSQL1 = MentoringSQL.GetMentoring(MentoringSQLCommads.FetchAllotedMetorstudentandStaffList).Replace("?SHIFT", sShift).Replace("?PROGRAMME_GROUP_ID", sProgramName).Replace("?APPTYPE_ID", sProgramLevel).Replace("?CLASS_YEAR", classyear);
                        var fetchStudentListExist = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new MENTORING_STUDENT_LIST()
                        {
                            ACADEMIC_YEAR = sAcademicYear,
                        }, sSQL1, sAcademicYear).DataSource.Data;
                        if (fetchStudentListExist == null)
                            if (fetchStudentListExist == null || fetchStudentListExist.Count <= 0)
                            {
                                var sSQL = MentoringSQL.GetMentoring(MentoringSQLCommads.FetchStudentlistForMentoringAllotment).Replace("?SHIFT", sShift).Replace("?PROGRAMME_GROUP_ID", sProgramName).Replace("?APPTYPE_ID", sProgramLevel).Replace("?CLASS_YEAR", classyear);
                                var fetchStudentList = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new MENTORING_STUDENT_LIST()
                                {
                                    ACADEMIC_YEAR = sAcademicYear,
                                }, sSQL, sAcademicYear).DataSource.Data;


                                var FetchMentrorStaffList = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(null, MentoringSQL.GetMentoring(MentoringSQLCommads.FetchMentorStaffList), sAcademicYear).DataSource.Data;

                                if (fetchStudentList != null && fetchStudentList.Count > 0 && FetchMentrorStaffList != null && FetchMentrorStaffList.Count > 0)
                                {
                                    try
                                    {
                                        var studentsByGender = fetchStudentList.GroupBy(s => s.GENDER).ToDictionary(g => g.Key, g => g.ToList());

                                        var staffByGender = FetchMentrorStaffList.GroupBy(s => s.GENDER).ToDictionary(g => g.Key, g => g.ToList());

                                        // Fetch staff max limit settings
                                        var settingStaff = CMSHandler.FetchData<MENTORING_STUDENT_LIST>(
                                            new MENTORING_STUDENT_LIST() { BATCH = sAcademicYear },
                                            MentoringSQL.GetMentoring(MentoringSQLCommads.FetchMentoringSetting),
                                            sAcademicYear
                                        )?.DataSource.Data as List<MENTORING_STUDENT_LIST> ?? new List<MENTORING_STUDENT_LIST>();

                                        var staffLimit = settingStaff.ToDictionary(s => s.STAFF_ID, s => Convert.ToInt32(s.MAX_STUDENT_COUNT));

                                        // Fetch ALL existing allocations (global allocation across years)
                                        var sSQL2 = MentoringSQL.GetMentoring(MentoringSQLCommads.FetchGlobalAllotedMentoringDetails);

                                        var globalAllocatedData = CMSHandler.FetchData<MENTORING_STUDENT_LIST>(
                                            new MENTORING_STUDENT_LIST() { ACADEMIC_YEAR = sAcademicYear },
                                            sSQL2,
                                            sAcademicYear
                                        )?.DataSource.Data as List<MENTORING_STUDENT_LIST>;

                                        var globalAllocated = globalAllocatedData ?? new List<MENTORING_STUDENT_LIST>();
                                        try
                                        {
                                            // FIRST: Check if there are any unallocated students for THIS CLASS YEAR
                                            var sSQLAllocated = MentoringSQL.GetMentoring(MentoringSQLCommads.FetchAllotedMetorstudentandStaffList)
                                                            .Replace("?SHIFT", sShift)
                                                            .Replace("?PROGRAMME_GROUP_ID", sProgramName)
                                                            .Replace("?APPTYPE_ID", sProgramLevel)
                                                            .Replace("?CLASS_YEAR", classyear);

                                            var existingAllocations = CMSHandler.FetchData<MENTORING_STUDENT_LIST>(
                                                new MENTORING_STUDENT_LIST() { BATCH = sAcademicYear },
                                                sSQLAllocated,
                                                sAcademicYear
                                            ).DataSource.Data as List<MENTORING_STUDENT_LIST> ?? new List<MENTORING_STUDENT_LIST>();

                                            // Create a set of already allocated student IDs for THIS CLASS YEAR
                                            var allocatedStudentIds = new HashSet<string>(existingAllocations.Select(e => e.STUDENT_ID));

                                            // Check if ANY student is unallocated for THIS CLASS YEAR
                                            bool hasUnallocatedStudents = false;
                                            foreach (var gender in studentsByGender.Keys)
                                            {
                                                var students = studentsByGender[gender];
                                                if (students.Any(stu => !allocatedStudentIds.Contains(stu.STUDENT_ID)))
                                                {
                                                    hasUnallocatedStudents = true;
                                                    break;
                                                }
                                            }

                                            // If no unallocated students, return appropriate message
                                            if (!hasUnallocatedStudents)
                                            {
                                                return Json(new
                                                {
                                                    Status = false,
                                                    Message = $"All students for Class Year {classyear} are already allocated to mentors in batch {sAcademicYear}."
                                                }, JsonRequestBehavior.AllowGet);
                                            }
                                            try
                                            {
                                                // Calculate remaining capacity for ALL staff
                                                var staffRemainingCapacity = new Dictionary<string, int>();

                                                // For staff WITH settings: use their max limit
                                                foreach (var staff in settingStaff)
                                                {
                                                    int maxLimit = Convert.ToInt32(staff.MAX_STUDENT_COUNT);

                                                    // Get current allocation for THIS STAFF (from both existing allocations and global)
                                                    int existingAllocationForStaff = existingAllocations.Count(e => e.STAFF_ID == staff.STAFF_ID);
                                                    int globalAllocationForStaff = globalAllocated.Count(g => g.STAFF_ID == staff.STAFF_ID);
                                                    int currentAllocation = existingAllocationForStaff + globalAllocationForStaff;

                                                    int remaining = maxLimit - currentAllocation;
                                                    staffRemainingCapacity[staff.STAFF_ID] = remaining > 0 ? remaining : 0;
                                                }

                                                StringBuilder sbAllocated = new StringBuilder();
                                                sbAllocated.Append("INSERT INTO MENTORING_STUDENT_LIST (STUDENT_ID, CLASS_ID, STAFF_ID, BATCH, CREATED_BY) VALUES ");

                                                bool firstAllocated = true;
                                                string createdBy = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                                                string batch = sAcademicYear;
                                                bool anyAllocationMade = false;
                                                int totalAllocatedCount = 0;

                                                foreach (var gender in studentsByGender.Keys)
                                                {
                                                    if (!staffByGender.ContainsKey(gender))
                                                        continue;

                                                    var students = studentsByGender[gender];
                                                    var staff = staffByGender[gender];

                                                    // Remove already allocated students for this gender and THIS CLASS YEAR
                                                    var unallocatedStudents = students.Where(stu => !allocatedStudentIds.Contains(stu.STUDENT_ID)).ToList();

                                                    if (!unallocatedStudents.Any())
                                                        continue; // Skip this gender if all students are already allocated for this class year


                                                    // Use ALL staff, not just those with settings
                                                    var mergedStaff = staff.Union(settingStaff.Where(s => s.GENDER == gender)).GroupBy(s => s.STAFF_ID).Select(g => g.First()).ToList();

                                                    // For staff WITHOUT settings, calculate their remaining capacity
                                                    foreach (var staffMember in mergedStaff.Where(s => !staffRemainingCapacity.ContainsKey(s.STAFF_ID)))
                                                    {
                                                        // Get already allocated count for this staff (existing + global)
                                                        int existingAllocForStaff = existingAllocations.Count(e => e.STAFF_ID == staffMember.STAFF_ID);
                                                        int globalAllocForStaff = globalAllocated.Count(x => x.STAFF_ID == staffMember.STAFF_ID);
                                                        int alreadyAllocated = existingAllocForStaff + globalAllocForStaff;

                                                        int totalStudents = unallocatedStudents.Count;
                                                        int totalStaff = mergedStaff.Count;

                                                        // Dynamically calculated ideal share per staff
                                                        int idealPerStaff = (int)Math.Ceiling((double)totalStudents / totalStaff);

                                                        // For staff without settings, use unlimited capacity
                                                        // But still calculate a reasonable limit
                                                        int calculatedCapacity = Math.Max(idealPerStaff * 2, 20); // Give generous capacity

                                                        int remaining = calculatedCapacity - alreadyAllocated;
                                                        staffRemainingCapacity[staffMember.STAFF_ID] = remaining > 0 ? remaining : 0;
                                                    }

                                                    // Filter out staff with no remaining capacity
                                                    var eligibleStaff = mergedStaff.Where(s => staffRemainingCapacity[s.STAFF_ID] > 0).ToList();

                                                    // If no staff eligible, log and continue
                                                    if (!eligibleStaff.Any())
                                                    {
                                                        Console.WriteLine($"No eligible staff for gender: {gender}");
                                                        continue;
                                                    }

                                                    // Current batch load - count only allocations for THIS BATCH
                                                    var staffCurrentLoad = eligibleStaff.ToDictionary(s => s.STAFF_ID, s => existingAllocations.Count(e => e.STAFF_ID == s.STAFF_ID));

                                                    // Also add any new allocations from globalAllocated for this batch
                                                    foreach (var staffId in staffCurrentLoad.Keys.ToList())
                                                    {
                                                        staffCurrentLoad[staffId] += globalAllocated.Count(g => g.STAFF_ID == staffId && g.BATCH == batch);
                                                    }

                                                    foreach (var student in unallocatedStudents)
                                                    {
                                                        // Find staff with minimum current load who has remaining capacity
                                                        var currentStaff = eligibleStaff.Where(s => staffRemainingCapacity[s.STAFF_ID] > 0).OrderBy(s => staffCurrentLoad[s.STAFF_ID]).ThenBy(s => s.STAFF_ID).FirstOrDefault();

                                                        if (currentStaff == null)
                                                        {
                                                            Console.WriteLine($"No eligible staff found for student: {student.STUDENT_ID}");
                                                            continue;
                                                        }

                                                        if (!firstAllocated) sbAllocated.Append(",");

                                                        sbAllocated.AppendFormat("('{0}', '{1}', '{2}', '{3}', '{4}')",
                                                            student.STUDENT_ID,
                                                            student.CLASS_ID,
                                                            currentStaff.STAFF_ID,
                                                            batch,
                                                            createdBy);

                                                        // Update both current load and remaining capacity
                                                        staffCurrentLoad[currentStaff.STAFF_ID]++;
                                                        staffRemainingCapacity[currentStaff.STAFF_ID]--;

                                                        // Add to global allocated for tracking within this session
                                                        globalAllocated.Add(new MENTORING_STUDENT_LIST
                                                        {
                                                            STUDENT_ID = student.STUDENT_ID,
                                                            CLASS_ID = student.CLASS_ID,
                                                            STAFF_ID = currentStaff.STAFF_ID,
                                                            BATCH = batch
                                                        });

                                                        anyAllocationMade = true;
                                                        totalAllocatedCount++;
                                                        firstAllocated = false;
                                                    }
                                                }

                                                if (anyAllocationMade)
                                                {
                                                    string finalQuery = sbAllocated.ToString();
                                                    var SaveAllotment = CMSHandler.SaveOrUpdate(null, finalQuery);

                                                    return Json(new
                                                    {
                                                        Status = true,
                                                        Message = $"Mentoring Allocation Saved Successfully! Allocated {totalAllocatedCount} students for Class Year {classyear}."
                                                    }, JsonRequestBehavior.AllowGet);
                                                }
                                                else
                                                {
                                                    // Provide more detailed error message
                                                    string detailedMessage = "No new allocations were made. Possible reasons:\n" +
                                                                           "1. All students for this class year are already allocated\n" +
                                                                           "2. No eligible staff available for unallocated students\n" +
                                                                           $"3. Check staff capacity settings for Class Year: {classyear}";

                                                    Console.WriteLine(detailedMessage);

                                                    return Json(new
                                                    {
                                                        Status = false,
                                                        Message = detailedMessage
                                                    }, JsonRequestBehavior.AllowGet);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                using (ErrorLog ObjLog = new ErrorLog())
                                                {
                                                    ObjLog.WriteError("MentorController", "SaveStaffMetoringAllotment", ex.Message);
                                                }

                                                return Json(new
                                                {
                                                    Status = false,
                                                    Message = $"Internal Server Error! Please try again. Error: {ex.Message}"
                                                }, JsonRequestBehavior.AllowGet);
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            using (ErrorLog ObjLog = new ErrorLog())
                                            {
                                                ObjLog.WriteError("MentorController", "SaveStaffMetoringAllotment", ex.Message);
                                            }

                                            return Json(new
                                            {
                                                Status = false,
                                                Message = $"Internal Server Error! Please try again. Error: {ex.Message}"
                                            }, JsonRequestBehavior.AllowGet);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        using (ErrorLog ObjLog = new ErrorLog())
                                        {
                                            ObjLog.WriteError("MentorController", "SaveStaffMetoringAllotment", ex.Message);
                                        }

                                        return Json(new { Status = false, Message = "Internal Server Error! Please try again." }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    var sSQL2 = MentoringSQL.GetMentoring(MentoringSQLCommads.FetchAllotedMetorstudentandStaffList).Replace("?SHIFT", sShift).Replace("?PROGRAMME_GROUP_ID", sProgramName).Replace("?APPTYPE_ID", sProgramLevel).Replace("?CLASS_YEAR", classyear);
                                    var FetchStudentForCheck = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new MENTORING_STUDENT_LIST() { ACADEMIC_YEAR = sAcademicYear }, sSQL2, sAcademicYear).DataSource.Data;

                                    var sSQL = MentoringSQL.GetMentoring(MentoringSQLCommads.FetchStudentlistForMentoringAllotment).Replace("?SHIFT", sShift).Replace("?PROGRAMME_GROUP_ID", sProgramName).Replace("?APPTYPE_ID", sProgramLevel).Replace("?CLASS_YEAR", classyear);
                                    var fetchStudentList = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new MENTORING_STUDENT_LIST() { ACADEMIC_YEAR = sAcademicYear, }, sSQL, sAcademicYear).DataSource.Data;
                                    var remainingStudents = fetchStudentList.Where(stu => !FetchStudentForCheck.Any(e => e.STUDENT_ID == stu.STUDENT_ID)).ToList();
                                    if (remainingStudents != null && remainingStudents.Count > 0)
                                    {
                                        // Get staff list and settings again
                                        var FetchMentrorStaffList = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(null, MentoringSQL.GetMentoring(MentoringSQLCommads.FetchMentorStaffList), sAcademicYear).DataSource.Data;

                                        var staffByGender = FetchMentrorStaffList.GroupBy(s => s.GENDER).ToDictionary(g => g.Key, g => g.ToList());

                                        var settingStaff = CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new MENTORING_STUDENT_LIST() { BATCH = sAcademicYear }, MentoringSQL.GetMentoring(MentoringSQLCommads.FetchMentoringSetting), sAcademicYear)?.DataSource.Data as List<MENTORING_STUDENT_LIST> ?? new List<MENTORING_STUDENT_LIST>();

                                        var staffLimit = settingStaff.ToDictionary(s => s.STAFF_ID, s => Convert.ToInt32(s.MAX_STUDENT_COUNT));

                                        StringBuilder sbAllocated = new StringBuilder();
                                        sbAllocated.Append("INSERT INTO MENTORING_STUDENT_LIST (STUDENT_ID, CLASS_ID, STAFF_ID, BATCH, CREATED_BY) VALUES ");

                                        bool firstAllocated = true;
                                        string createdBy = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                                        string batch = sAcademicYear;

                                        var remainingByGender = remainingStudents.GroupBy(s => s.GENDER).ToDictionary(g => g.Key, g => g.ToList());

                                        foreach (var gender in remainingByGender.Keys)
                                        {
                                            if (!staffByGender.ContainsKey(gender))
                                                continue;

                                            var students = remainingByGender[gender];
                                            var staff = staffByGender[gender];

                                            int defaultLimit = (int)Math.Ceiling((double)students.Count / staff.Count);

                                            var mergedStaff = staff.Union(settingStaff.Where(s => s.GENDER == gender)).GroupBy(s => s.STAFF_ID).Select(g => g.First()).ToList();

                                            // Existing load count
                                            var staffCurrentLoad = mergedStaff.ToDictionary(s => s.STAFF_ID, s => FetchStudentForCheck.Count(e => e.STAFF_ID == s.STAFF_ID));

                                            foreach (var student in students)
                                            {
                                                var currentStaff = mergedStaff.OrderBy(s => staffCurrentLoad[s.STAFF_ID])
                                                    .FirstOrDefault(s =>
                                                    {
                                                        int maxLimit = staffLimit.ContainsKey(s.STAFF_ID) ? staffLimit[s.STAFF_ID] : defaultLimit;
                                                        return staffCurrentLoad[s.STAFF_ID] < maxLimit;
                                                    });

                                                if (currentStaff == null)
                                                    continue;

                                                if (!firstAllocated) sbAllocated.Append(",");

                                                sbAllocated.AppendFormat("('{0}', '{1}', '{2}', '{3}', '{4}')",
                                                   student.STUDENT_ID,
                                                   student.CLASS_ID,
                                                   currentStaff.STAFF_ID,
                                                   batch,
                                                   createdBy);

                                                staffCurrentLoad[currentStaff.STAFF_ID]++;
                                                firstAllocated = false;
                                            }
                                        }

                                        // Final DB insert
                                        if (!firstAllocated)
                                        {
                                            string finalQuery = sbAllocated.ToString();
                                            var SaveAllotment = CMSHandler.SaveOrUpdate(null, finalQuery);
                                            return Json(new { Status = true, Message = "Mentoring Allocation Saved Successfully!" }, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            return Json(new
                                            {
                                                Status = false,
                                                Message = "No students available for Mentoring Allocation!"
                                            }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else
                                    {
                                        return Json(new
                                        {
                                            Status = false,
                                            Message = "All students are already allocated for this criteria."
                                        }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                catch (Exception ex)
                                {

                                    using (ErrorLog ObjLog = new ErrorLog())
                                    {
                                        ObjLog.WriteError("MentorController", "SaveStaffMetoringAllotment", ex.Message);
                                    }
                                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    return Json("Internal Server Error! Please try again.", JsonRequestBehavior.AllowGet);
                                }

                            }

                    }
                }
                return Json("Internal Server Error! Please try again.", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("MentorController", "SaveStaffMetoringAllotment", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                return Json("Internal Server Error! Please try again.", JsonRequestBehavior.AllowGet);

            }
        }


        public ActionResult BindMentoringStaffList(string sAcademicYear, string sProgramName, string sShift, string sProgramLevel, string classyear)
        {
            SupportDataViewModel viewmodal = new SupportDataViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                var sSQL = MentoringSQL.GetMentoring(MentoringSQLCommads.FetchAllotedMetorstudentandStaffList).Replace("?SHIFT", sShift).Replace("?PROGRAMME_GROUP_ID", sProgramName).Replace("?APPTYPE_ID", sProgramLevel).Replace("?CLASS_YEAR", classyear);
                viewmodal.liAllotedMentorList = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new MENTORING_STUDENT_LIST()
                {
                    ACADEMIC_YEAR = sAcademicYear,
                }, sSQL, sAcademicYear).DataSource.Data;
            }


            return View(viewmodal);
        }
        #endregion

        #region Class Wise Student List

        public ActionResult ClassWiseMentorList()
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
                    if (role == Common.SupRole.ADMIN)
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
                    objHandler.WriteError("ClassWiseMentorList", "MentorController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ClassWiseMentorList", "MentorController", ex.Message);
                }
            }
            return View(studentViewModel);
        }


        public ActionResult BindClassWiseMentorStudentList(string objInput)
        {

            MentoringViewModel objStudentViewModel = new MentoringViewModel();
            var userID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    //objStudentViewModel.sDayOrder = (Session[Common.SESSION_VARIABLES.DAY_ORDER] != null) ? Session[Common.SESSION_VARIABLES.DAY_ORDER].ToString() : string.Empty;
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    string sClassId = string.Empty;
                    // Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] = "1,3,5";

                    //timeTable.CLASS_ID = obj.ClassId;
                    //timeTable.ATTENDANCE_DATE = DateTime.Today.ToString("yyyy-MM-dd");

                    objStudentViewModel.StudentTagMapping = (List<STUDENT_TAG_MAPPING>)CMSHandler.FetchData<STUDENT_TAG_MAPPING>(new CP_CLASSES() { CLASS_ID = objInput },
                        SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchClassWiseMentorStudentList), sAcademicYear).DataSource.Data;

                    //objStudentViewModel.TagNameLI = (List<SUP_STUDENT_TAG_LIST>)CMSHandler.FetchData<SUP_STUDENT_TAG_LIST>(new stf_Personal_Info() { STAFF_ID = userID }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchReasonForTagStudentMapping), sAcademicYear).DataSource.Data;



                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Mentor", "BindClassWiseMentorStudentList", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Mentor", "BindClassWiseMentorStudentList", Ex.Message);
                }

            }
            return View(objStudentViewModel);
        }
        #endregion





        #region New Student Mentor Allocated
        public ActionResult MissingMentorStudent()
        {
            string sAcademicYear = string.Empty;
            TransferCertificate objTransferCertificate = new TransferCertificate();
            sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    List<Sup_Shift> liShift = new List<Sup_Shift>();
                    liShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    objTransferCertificate.ShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    // Program ...
                    DataTable dtProgram = new DataTable();
                    dtProgram = SupportDataMethod.FetchProgram(sAcademicYear).DataSource.Table;
                    List<cp_Program_2017> liProgram = new List<cp_Program_2017>();
                    if (dtProgram != null && dtProgram.Rows.Count > 0)
                    {
                        liProgram = (from DataRow dr in dtProgram.Rows
                                     select new cp_Program_2017()
                                     {
                                         PROGRAMME_ID = dr[Common.CP_PROGRAMME_2017.PROGRAMME_ID].ToString(),
                                         PROGRAMME_NAME = dr[Common.CP_PROGRAMME_2017.PROGRAMME_NAME].ToString()
                                     }).ToList();
                        //objTransferCertificate.ProgramName = new SelectList(liProgram, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    }
                    // Fetch class ....
                    DataTable dtClass = new DataTable();

                    dtClass = SupportDataMethod.FetchClass(sAcademicYear).DataSource.Table;
                    List<cp_Classes_2017> liClass = new List<cp_Classes_2017>();
                    if (dtClass != null && dtClass.Rows.Count > 0)
                    {
                        liClass = (from DataRow dr in dtClass.Rows
                                   select new cp_Classes_2017()
                                   {
                                       CLASS_ID = dr[Common.CP_CLASSES_2017.CLASS_ID].ToString(),
                                       CLASS_NAME = dr[Common.CP_CLASSES_2017.CLASS_NAME].ToString()
                                   }).ToList();
                        //objTransferCertificate.ClassName = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                    var AcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcademicYear != null && AcademicYear.Count > 0)
                    {
                        objTransferCertificate.AcademicYear = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var classYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data;
                    if (classYear != null && classYear.Count > 0)
                    {
                        objTransferCertificate.ClassYear = new SelectList(classYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                    }
                    var programmeLevel = (List<SUP_PROGRAMME_LEVEL>)CMSHandler.FetchData<SUP_PROGRAMME_LEVEL>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel)).DataSource.Data;
                    if (programmeLevel != null && programmeLevel.Count > 0)
                    {
                        objTransferCertificate.programmeLevel = new SelectList(programmeLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StudentController", "TCPrintView", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StudentController", "TCPrintView", ex.Message);
                    }
                    sResult = "Fail To Fetch Record(s) ...!";
                    return Json(sResult, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(objTransferCertificate);
        }

        public JsonResult SaveNewStudentStaffMetoringAllotment(string sAcademicYear, string sProgramName, string sShift, string sProgramLevel, string classyear)
        {
            var AcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sResult = string.Empty;
            var ObjJsonData = new JsonResultData();

            try
            {
                if (!string.IsNullOrEmpty(AcademicYear))
                {
                    if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sProgramName) && !string.IsNullOrEmpty(sShift) && !string.IsNullOrEmpty(sProgramLevel))
                    {
                        var sSQL1 = MentoringSQL.GetMentoring(MentoringSQLCommads.FetchAllotedMetorstudentandStaffList).Replace("?SHIFT", sShift).Replace("?PROGRAMME_GROUP_ID", sProgramName).Replace("?APPTYPE_ID", sProgramLevel).Replace("?CLASS_YEAR", classyear);
                        var fetchStudentListExist = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new MENTORING_STUDENT_LIST()
                        {
                            ACADEMIC_YEAR = sAcademicYear,
                        }, sSQL1, sAcademicYear).DataSource.Data;
                        if (fetchStudentListExist == null)
                            if (fetchStudentListExist == null || fetchStudentListExist.Count <= 0)
                            {
                                var sSQL = MentoringSQL.GetMentoring(MentoringSQLCommads.FetchStudentlistForMentoringAllotment).Replace("?SHIFT", sShift).Replace("?PROGRAMME_GROUP_ID", sProgramName).Replace("?APPTYPE_ID", sProgramLevel).Replace("?CLASS_YEAR", classyear);
                                var fetchStudentList = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new MENTORING_STUDENT_LIST()
                                {
                                    ACADEMIC_YEAR = sAcademicYear,
                                }, sSQL, sAcademicYear).DataSource.Data;


                                var FetchMentrorStaffList = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(null, MentoringSQL.GetMentoring(MentoringSQLCommads.FetchMentorStaffList), sAcademicYear).DataSource.Data;

                                if (fetchStudentList != null && fetchStudentList.Count > 0 && FetchMentrorStaffList != null && FetchMentrorStaffList.Count > 0)
                                {
                                    try
                                    {
                                        var studentsByGender = fetchStudentList.GroupBy(s => s.GENDER).ToDictionary(g => g.Key, g => g.ToList());

                                        var staffByGender = FetchMentrorStaffList.GroupBy(s => s.GENDER).ToDictionary(g => g.Key, g => g.ToList());

                                        // Fetch staff max limit settings
                                        var settingStaff = CMSHandler.FetchData<MENTORING_STUDENT_LIST>(
                                            new MENTORING_STUDENT_LIST() { BATCH = sAcademicYear },
                                            MentoringSQL.GetMentoring(MentoringSQLCommads.FetchMentoringSetting),
                                            sAcademicYear
                                        )?.DataSource.Data as List<MENTORING_STUDENT_LIST> ?? new List<MENTORING_STUDENT_LIST>();

                                        var staffLimit = settingStaff.ToDictionary(s => s.STAFF_ID, s => Convert.ToInt32(s.MAX_STUDENT_COUNT));

                                        // Fetch ALL existing allocations (global allocation across years)
                                        var sSQL2 = MentoringSQL.GetMentoring(MentoringSQLCommads.FetchGlobalAllotedMentoringDetails);

                                        var globalAllocatedData = CMSHandler.FetchData<MENTORING_STUDENT_LIST>(
                                            new MENTORING_STUDENT_LIST() { ACADEMIC_YEAR = sAcademicYear },
                                            sSQL2,
                                            sAcademicYear
                                        )?.DataSource.Data as List<MENTORING_STUDENT_LIST>;

                                        var globalAllocated = globalAllocatedData ?? new List<MENTORING_STUDENT_LIST>();
                                        try
                                        {
                                            // FIRST: Check if there are any unallocated students for THIS CLASS YEAR
                                            var sSQLAllocated = MentoringSQL.GetMentoring(MentoringSQLCommads.FetchAllotedMetorstudentandStaffList)
                                                            .Replace("?SHIFT", sShift)
                                                            .Replace("?PROGRAMME_GROUP_ID", sProgramName)
                                                            .Replace("?APPTYPE_ID", sProgramLevel)
                                                            .Replace("?CLASS_YEAR", classyear);

                                            var existingAllocations = CMSHandler.FetchData<MENTORING_STUDENT_LIST>(
                                                new MENTORING_STUDENT_LIST() { BATCH = sAcademicYear },
                                                sSQLAllocated,
                                                sAcademicYear
                                            ).DataSource.Data as List<MENTORING_STUDENT_LIST> ?? new List<MENTORING_STUDENT_LIST>();

                                            // Create a set of already allocated student IDs for THIS CLASS YEAR
                                            var allocatedStudentIds = new HashSet<string>(existingAllocations.Select(e => e.STUDENT_ID));

                                            // Check if ANY student is unallocated for THIS CLASS YEAR
                                            bool hasUnallocatedStudents = false;
                                            foreach (var gender in studentsByGender.Keys)
                                            {
                                                var students = studentsByGender[gender];
                                                if (students.Any(stu => !allocatedStudentIds.Contains(stu.STUDENT_ID)))
                                                {
                                                    hasUnallocatedStudents = true;
                                                    break;
                                                }
                                            }

                                            // If no unallocated students, return appropriate message
                                            if (!hasUnallocatedStudents)
                                            {
                                                return Json(new
                                                {
                                                    Status = false,
                                                    Message = $"All students for Class Year {classyear} are already allocated to mentors in batch {sAcademicYear}."
                                                }, JsonRequestBehavior.AllowGet);
                                            }
                                            try
                                            {
                                                // Calculate remaining capacity for ALL staff
                                                var staffRemainingCapacity = new Dictionary<string, int>();

                                                // For staff WITH settings: use their max limit
                                                foreach (var staff in settingStaff)
                                                {
                                                    int maxLimit = Convert.ToInt32(staff.MAX_STUDENT_COUNT);

                                                    // Get current allocation for THIS STAFF (from both existing allocations and global)
                                                    int existingAllocationForStaff = existingAllocations.Count(e => e.STAFF_ID == staff.STAFF_ID);
                                                    int globalAllocationForStaff = globalAllocated.Count(g => g.STAFF_ID == staff.STAFF_ID);
                                                    int currentAllocation = existingAllocationForStaff + globalAllocationForStaff;

                                                    int remaining = maxLimit - currentAllocation;
                                                    staffRemainingCapacity[staff.STAFF_ID] = remaining > 0 ? remaining : 0;
                                                }

                                                StringBuilder sbAllocated = new StringBuilder();
                                                sbAllocated.Append("INSERT INTO MENTORING_STUDENT_LIST (STUDENT_ID, CLASS_ID, STAFF_ID, BATCH, CREATED_BY) VALUES ");

                                                bool firstAllocated = true;
                                                string createdBy = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                                                string batch = sAcademicYear;
                                                bool anyAllocationMade = false;
                                                int totalAllocatedCount = 0;

                                                foreach (var gender in studentsByGender.Keys)
                                                {
                                                    if (!staffByGender.ContainsKey(gender))
                                                        continue;

                                                    var students = studentsByGender[gender];
                                                    var staff = staffByGender[gender];

                                                    // Remove already allocated students for this gender and THIS CLASS YEAR
                                                    var unallocatedStudents = students.Where(stu => !allocatedStudentIds.Contains(stu.STUDENT_ID)).ToList();

                                                    if (!unallocatedStudents.Any())
                                                        continue; // Skip this gender if all students are already allocated for this class year


                                                    // Use ALL staff, not just those with settings
                                                    var mergedStaff = staff.Union(settingStaff.Where(s => s.GENDER == gender)).GroupBy(s => s.STAFF_ID).Select(g => g.First()).ToList();

                                                    // For staff WITHOUT settings, calculate their remaining capacity
                                                    foreach (var staffMember in mergedStaff.Where(s => !staffRemainingCapacity.ContainsKey(s.STAFF_ID)))
                                                    {
                                                        // Get already allocated count for this staff (existing + global)
                                                        int existingAllocForStaff = existingAllocations.Count(e => e.STAFF_ID == staffMember.STAFF_ID);
                                                        int globalAllocForStaff = globalAllocated.Count(x => x.STAFF_ID == staffMember.STAFF_ID);
                                                        int alreadyAllocated = existingAllocForStaff + globalAllocForStaff;

                                                        int totalStudents = unallocatedStudents.Count;
                                                        int totalStaff = mergedStaff.Count;

                                                        // Dynamically calculated ideal share per staff
                                                        int idealPerStaff = (int)Math.Ceiling((double)totalStudents / totalStaff);

                                                        // For staff without settings, use unlimited capacity
                                                        // But still calculate a reasonable limit
                                                        int calculatedCapacity = Math.Max(idealPerStaff * 2, 20); // Give generous capacity

                                                        int remaining = calculatedCapacity - alreadyAllocated;
                                                        staffRemainingCapacity[staffMember.STAFF_ID] = remaining > 0 ? remaining : 0;
                                                    }

                                                    // Filter out staff with no remaining capacity
                                                    var eligibleStaff = mergedStaff.Where(s => staffRemainingCapacity[s.STAFF_ID] > 0).ToList();

                                                    // If no staff eligible, log and continue
                                                    if (!eligibleStaff.Any())
                                                    {
                                                        Console.WriteLine($"No eligible staff for gender: {gender}");
                                                        continue;
                                                    }

                                                    // Current batch load - count only allocations for THIS BATCH
                                                    var staffCurrentLoad = eligibleStaff.ToDictionary(s => s.STAFF_ID, s => existingAllocations.Count(e => e.STAFF_ID == s.STAFF_ID));

                                                    // Also add any new allocations from globalAllocated for this batch
                                                    foreach (var staffId in staffCurrentLoad.Keys.ToList())
                                                    {
                                                        staffCurrentLoad[staffId] += globalAllocated.Count(g => g.STAFF_ID == staffId && g.BATCH == batch);
                                                    }

                                                    foreach (var student in unallocatedStudents)
                                                    {
                                                        // Find staff with minimum current load who has remaining capacity
                                                        var currentStaff = eligibleStaff.Where(s => staffRemainingCapacity[s.STAFF_ID] > 0).OrderBy(s => staffCurrentLoad[s.STAFF_ID]).ThenBy(s => s.STAFF_ID).FirstOrDefault();

                                                        if (currentStaff == null)
                                                        {
                                                            Console.WriteLine($"No eligible staff found for student: {student.STUDENT_ID}");
                                                            continue;
                                                        }

                                                        if (!firstAllocated) sbAllocated.Append(",");

                                                        sbAllocated.AppendFormat("('{0}', '{1}', '{2}', '{3}', '{4}')",
                                                            student.STUDENT_ID,
                                                            student.CLASS_ID,
                                                            currentStaff.STAFF_ID,
                                                            batch,
                                                            createdBy);

                                                        // Update both current load and remaining capacity
                                                        staffCurrentLoad[currentStaff.STAFF_ID]++;
                                                        staffRemainingCapacity[currentStaff.STAFF_ID]--;

                                                        // Add to global allocated for tracking within this session
                                                        globalAllocated.Add(new MENTORING_STUDENT_LIST
                                                        {
                                                            STUDENT_ID = student.STUDENT_ID,
                                                            CLASS_ID = student.CLASS_ID,
                                                            STAFF_ID = currentStaff.STAFF_ID,
                                                            BATCH = batch
                                                        });

                                                        anyAllocationMade = true;
                                                        totalAllocatedCount++;
                                                        firstAllocated = false;
                                                    }
                                                }

                                                if (anyAllocationMade)
                                                {
                                                    string finalQuery = sbAllocated.ToString();
                                                    var SaveAllotment = CMSHandler.SaveOrUpdate(null, finalQuery);

                                                    return Json(new
                                                    {
                                                        Status = true,
                                                        Message = $"Mentoring Allocation Saved Successfully! Allocated {totalAllocatedCount} students for Class Year {classyear}."
                                                    }, JsonRequestBehavior.AllowGet);
                                                }
                                                else
                                                {
                                                    // Provide more detailed error message
                                                    string detailedMessage = "No new allocations were made. Possible reasons:\n" +
                                                                           "1. All students for this class year are already allocated\n" +
                                                                           "2. No eligible staff available for unallocated students\n" +
                                                                           $"3. Check staff capacity settings for Class Year: {classyear}";

                                                    Console.WriteLine(detailedMessage);

                                                    return Json(new
                                                    {
                                                        Status = false,
                                                        Message = detailedMessage
                                                    }, JsonRequestBehavior.AllowGet);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                using (ErrorLog ObjLog = new ErrorLog())
                                                {
                                                    ObjLog.WriteError("MentorController", "SaveStaffMetoringAllotment", ex.Message);
                                                }

                                                return Json(new
                                                {
                                                    Status = false,
                                                    Message = $"Internal Server Error! Please try again. Error: {ex.Message}"
                                                }, JsonRequestBehavior.AllowGet);
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            using (ErrorLog ObjLog = new ErrorLog())
                                            {
                                                ObjLog.WriteError("MentorController", "SaveStaffMetoringAllotment", ex.Message);
                                            }

                                            return Json(new
                                            {
                                                Status = false,
                                                Message = $"Internal Server Error! Please try again. Error: {ex.Message}"
                                            }, JsonRequestBehavior.AllowGet);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        using (ErrorLog ObjLog = new ErrorLog())
                                        {
                                            ObjLog.WriteError("MentorController", "SaveStaffMetoringAllotment", ex.Message);
                                        }

                                        return Json(new { Status = false, Message = "Internal Server Error! Please try again." }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    var sSQL2 = MentoringSQL.GetMentoring(MentoringSQLCommads.FetchAllotedMetorstudentandStaffList).Replace("?SHIFT", sShift).Replace("?PROGRAMME_GROUP_ID", sProgramName).Replace("?APPTYPE_ID", sProgramLevel).Replace("?CLASS_YEAR", classyear);
                                    var FetchStudentForCheck = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new MENTORING_STUDENT_LIST() { ACADEMIC_YEAR = sAcademicYear }, sSQL2, sAcademicYear).DataSource.Data;

                                    var sSQL = MentoringSQL.GetMentoring(MentoringSQLCommads.FetchStudentlistForMentoringAllotment).Replace("?SHIFT", sShift).Replace("?PROGRAMME_GROUP_ID", sProgramName).Replace("?APPTYPE_ID", sProgramLevel).Replace("?CLASS_YEAR", classyear);
                                    var fetchStudentList = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new MENTORING_STUDENT_LIST() { ACADEMIC_YEAR = sAcademicYear, }, sSQL, sAcademicYear).DataSource.Data;
                                    var remainingStudents = fetchStudentList.Where(stu => !FetchStudentForCheck.Any(e => e.STUDENT_ID == stu.STUDENT_ID)).ToList();
                                    if (remainingStudents != null && remainingStudents.Count > 0)
                                    {
                                        // Get staff list and settings again
                                        var FetchMentrorStaffList = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(null, MentoringSQL.GetMentoring(MentoringSQLCommads.FetchMentorStaffList), sAcademicYear).DataSource.Data;

                                        var staffByGender = FetchMentrorStaffList.GroupBy(s => s.GENDER).ToDictionary(g => g.Key, g => g.ToList());

                                        var settingStaff = CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new MENTORING_STUDENT_LIST() { BATCH = sAcademicYear }, MentoringSQL.GetMentoring(MentoringSQLCommads.FetchMentoringSetting), sAcademicYear)?.DataSource.Data as List<MENTORING_STUDENT_LIST> ?? new List<MENTORING_STUDENT_LIST>();

                                        var staffLimit = settingStaff.ToDictionary(s => s.STAFF_ID, s => Convert.ToInt32(s.MAX_STUDENT_COUNT));

                                        StringBuilder sbAllocated = new StringBuilder();
                                        sbAllocated.Append("INSERT INTO MENTORING_STUDENT_LIST (STUDENT_ID, CLASS_ID, STAFF_ID, BATCH, CREATED_BY) VALUES ");

                                        bool firstAllocated = true;
                                        string createdBy = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                                        string batch = sAcademicYear;

                                        var remainingByGender = remainingStudents.GroupBy(s => s.GENDER).ToDictionary(g => g.Key, g => g.ToList());

                                        foreach (var gender in remainingByGender.Keys)
                                        {
                                            if (!staffByGender.ContainsKey(gender))
                                                continue;

                                            var students = remainingByGender[gender];
                                            var staff = staffByGender[gender];

                                            int defaultLimit = (int)Math.Ceiling((double)students.Count / staff.Count);

                                            var mergedStaff = staff.Union(settingStaff.Where(s => s.GENDER == gender)).GroupBy(s => s.STAFF_ID).Select(g => g.First()).ToList();

                                            // Existing load count
                                            var staffCurrentLoad = mergedStaff.ToDictionary(s => s.STAFF_ID, s => FetchStudentForCheck.Count(e => e.STAFF_ID == s.STAFF_ID));

                                            foreach (var student in students)
                                            {
                                                var currentStaff = mergedStaff.OrderBy(s => staffCurrentLoad[s.STAFF_ID])
                                                    .FirstOrDefault(s =>
                                                    {
                                                        int maxLimit = staffLimit.ContainsKey(s.STAFF_ID) ? staffLimit[s.STAFF_ID] : defaultLimit;
                                                        return staffCurrentLoad[s.STAFF_ID] < maxLimit;
                                                    });

                                                if (currentStaff == null)
                                                    continue;

                                                if (!firstAllocated) sbAllocated.Append(",");

                                                sbAllocated.AppendFormat("('{0}', '{1}', '{2}', '{3}', '{4}')",
                                                   student.STUDENT_ID,
                                                   student.CLASS_ID,
                                                   currentStaff.STAFF_ID,
                                                   batch,
                                                   createdBy);

                                                staffCurrentLoad[currentStaff.STAFF_ID]++;
                                                firstAllocated = false;
                                            }
                                        }

                                        // Final DB insert
                                        if (!firstAllocated)
                                        {
                                            string finalQuery = sbAllocated.ToString();
                                            var SaveAllotment = CMSHandler.SaveOrUpdate(null, finalQuery);
                                            return Json(new { Status = true, Message = "Mentoring Allocation Saved Successfully!" }, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            return Json(new
                                            {
                                                Status = false,
                                                Message = "No students available for Mentoring Allocation!"
                                            }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else
                                    {
                                        return Json(new
                                        {
                                            Status = false,
                                            Message = "All students are already allocated for this criteria."
                                        }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                catch (Exception ex)
                                {

                                    using (ErrorLog ObjLog = new ErrorLog())
                                    {
                                        ObjLog.WriteError("MentorController", "SaveNewStudentStaffMetoringAllotment", ex.Message);
                                    }
                                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    return Json("Internal Server Error! Please try again.", JsonRequestBehavior.AllowGet);
                                }

                            }

                    }
                }
                return Json("Internal Server Error! Please try again.", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("MentorController", "SaveNewStudentStaffMetoringAllotment", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                return Json("Internal Server Error! Please try again.", JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public ActionResult ManualMentoringAllocation(List<MENTORING_STUDENT_LIST> allocationList)
        {
            try
            {
                if (allocationList != null && allocationList.Count > 0)
                {
                    StringBuilder sbQueries = new StringBuilder();
                    var userCode = Session["StaffCode"]?.ToString() ?? "ADMIN"; // Default to ADMIN if session null

                    foreach (var item in allocationList)
                    {
                        string query = MentoringSQL.GetMentoring(MentoringSQLCommads.SaveMentoringAllocation)
                            .Replace("STUDENT_ID", "'" + item.STUDENT_ID + "'")
                            .Replace("CLASS_ID", "'" + item.CLASS_ID + "'")
                            .Replace("STAFF_ID", "'" + item.STAFF_ID + "'")
                            .Replace("CREATED_BY", "'" + userCode + "'")
                            .Replace("CREATED_DATE", "NOW()");
                         
                        sbQueries.Append(query);
                    }

                    if (sbQueries.Length > 0)
                    {
                        var result = CMSHandler.SaveOrUpdate(null, sbQueries.ToString());
                        if (result.Success)
                        {
                            return Json(new { Status = true, Message = "Mentoring Allocation Saved Successfully!" });
                        }
                    }
                }
                
                return Json(new { Status = false, Message = "Failed to save allocation." });
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("MentorController", "ManualMentoringAllocation", ex.Message);
                }
                return Json(new { Status = false, Message = "Internal Server Error!" });
            }
        }


        public ActionResult BindNewStudentMentoringStaffList(string sAcademicYear, string sProgramName, string sShift, string sProgramLevel, string classyear)
        {
            
            sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            SupportDataViewModel viewmodal = new SupportDataViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                var sSQL = MentoringSQL.GetMentoring(MentoringSQLCommads.FetchAllotedMetorstudentandStaffList).Replace("?SHIFT", sShift).Replace("?PROGRAMME_GROUP_ID", sProgramName).Replace("?APPTYPE_ID", sProgramLevel).Replace("?CLASS_YEAR", classyear);
                viewmodal.liAllotedMentorList = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new MENTORING_STUDENT_LIST()
                {
                    ACADEMIC_YEAR = sAcademicYear,
                }, sSQL, sAcademicYear).DataSource.Data;

                // Fetch Staff With Count For Dropdown
                viewmodal.liMentorForStaff = (List<MENTORING_STUDENT_LIST_FOR_STAFF>)CMSHandler.FetchData<MENTORING_STUDENT_LIST_FOR_STAFF>(new MENTORING_STUDENT_LIST_FOR_STAFF()
                {
                    AC_YEAR = sAcademicYear // Mapping ?AC_YEAR
                }, MentoringSQL.GetMentoring(MentoringSQLCommads.FetchStaffWithCountForAllocation), sAcademicYear).DataSource.Data;

                // Fetch All Eligible Students
                var sSQLAll = MentoringSQL.GetMentoring(MentoringSQLCommads.FetchStudentlistForMentoringAllotment).Replace("?SHIFT", sShift).Replace("?PROGRAMME_GROUP_ID", sProgramName).Replace("?APPTYPE_ID", sProgramLevel).Replace("?CLASS_YEAR", classyear);
                var allStudents = (List<MENTORING_STUDENT_LIST>)CMSHandler.FetchData<MENTORING_STUDENT_LIST>(new MENTORING_STUDENT_LIST()
                {
                    ACADEMIC_YEAR = sAcademicYear,
                }, sSQLAll, sAcademicYear).DataSource.Data;

                if (allStudents != null && allStudents.Count > 0)
                {
                    if (viewmodal.liAllotedMentorList != null && viewmodal.liAllotedMentorList.Count > 0)
                    {
                        var allocatedStudentIds = new HashSet<string>(viewmodal.liAllotedMentorList.Select(x => x.STUDENT_ID));
                        viewmodal.liUnallocatedMentorList = allStudents.Where(x => !allocatedStudentIds.Contains(x.STUDENT_ID))
                                                                       .OrderBy(x => x.CLASS_NAME)
                                                                       .ThenBy(x => x.ROLL_NO)
                                                                       .ToList();
                    }
                    else
                    {
                        viewmodal.liUnallocatedMentorList = allStudents.OrderBy(x => x.CLASS_NAME)
                                                                       .ThenBy(x => x.ROLL_NO)
                                                                       .ToList();
                    }
                }
            }


            return View(viewmodal);
        }
        #endregion





    }
}