using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CMS.DAO;
using CMS.Models;
using CMS.Utility;
using System.Runtime.CompilerServices;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CMS.Controllers
{
    public class AlumniElectionController : Controller
    {
        // Check if admin user is logged in
        private bool IsAdminLoggedIn()
        {
            return Session != null && Session[Common.SESSION_VARIABLES.USER_ID] != null;
        }

        // Helper to fetch active academic year from academic_year table
        private string GetActiveAcademicYear()
        {
            try
            {
                var sql = "SELECT AC_YEAR FROM academic_year WHERE ACTIVE_YEAR = 1 AND IS_DELETED = 0 LIMIT 1;";
                var list = DapperHandler.FetchAll<string>(sql);
                if (list != null && list.Count > 0 && !string.IsNullOrWhiteSpace(list[0]))
                {
                    return list[0].Trim();
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objLog = new ErrorLog())
                {
                    objLog.WriteError("AlumniElectionController", "GetActiveAcademicYear", ex.Message);
                }
            }
            return DateTime.Now.Year.ToString();
        }

        // Helper to fetch the currently active election setting
        public AlumniElectionSetting GetActiveElectionSetting()
        {
            try
            {
                var sql = @"SELECT ID, SETTING_NAME, ELECTION_DATE, LAST_DATE_SUBMISSION, NOTICE_PDF_PATH, 
                                   NOTES, ELECTION_PLACE, ELECTION_TIME, ACADEMIC_YEAR, IS_ACTIVE, IS_DELETED, CREATED_DATE 
                            FROM alumni_election_setting 
                            WHERE IS_ACTIVE = 1 AND IS_DELETED = 0 
                            ORDER BY ID DESC LIMIT 1;";
                var list = DapperHandler.FetchAll<AlumniElectionSetting>(sql);
                if (list != null && list.Count > 0)
                {
                    return list[0];
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objLog = new ErrorLog())
                {
                    objLog.WriteError("AlumniElectionController", "GetActiveElectionSetting", ex.Message);
                }
            }
            return null;
        }

        // =========================================================================
        // PUBLIC FACING ACTIONS (No Login Required)
        // =========================================================================

        // GET: /AlumniElection or /AlumniElection/Index
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                // Fetch list of programmes with PROGRAMME_ID and PROGRAMME_NAME
                var sqlProgrammes = @"SELECT PROGRAMME_ID, PROGRAMME_NAME 
                                      FROM cp_programme 
                                      WHERE IS_DELETED = 0 AND PROGRAMME_NAME IS NOT NULL AND TRIM(PROGRAMME_NAME) != '' 
                                      ORDER BY PROGRAMME_NAME ASC;";
                var programmes = DapperHandler.FetchAll<ProgrammeDropdownItem>(sqlProgrammes);
                ViewBag.ProgrammeList = programmes ?? new List<ProgrammeDropdownItem>();

                // Fetch active academic year
                string activeYear = GetActiveAcademicYear();
                ViewBag.ActiveAcademicYear = activeYear;

                // Fetch active election setting
                var activeSetting = GetActiveElectionSetting();
                ViewBag.ActiveSetting = activeSetting;

                // Check if registration has closed
                bool isRegistrationClosed = false;
                string closedReason = "";
                if (activeSetting == null)
                {
                    isRegistrationClosed = true;
                    closedReason = "Alumni election attendance registration is currently not active.";
                }
                else if (activeSetting.LAST_DATE_SUBMISSION.HasValue && DateTime.Now > activeSetting.LAST_DATE_SUBMISSION.Value)
                {
                    isRegistrationClosed = true;
                    closedReason = "The submission deadline was " + activeSetting.LAST_DATE_SUBMISSION.Value.ToString("dd-MMM-yyyy hh:mm tt") + ". New attendance submissions are now closed.";
                }
                ViewBag.IsRegistrationClosed = isRegistrationClosed;
                ViewBag.ClosedReason = closedReason;

                // Generate list of passing years (from current year down to 1960)
                int currentYear = DateTime.Now.Year;
                var years = new List<string>();
                for (int y = currentYear; y >= 1960; y--)
                {
                    years.Add(y.ToString());
                }
                ViewBag.YearList = years;
            }
            catch (Exception ex)
            {
                using (ErrorLog objLog = new ErrorLog())
                {
                    objLog.WriteError("AlumniElectionController", "Index", ex.Message);
                }
                ViewBag.ProgrammeList = new List<ProgrammeDropdownItem>();
                ViewBag.YearList = new List<string>();
                ViewBag.ActiveAcademicYear = DateTime.Now.Year.ToString();
                ViewBag.ActiveSetting = null;
                ViewBag.IsRegistrationClosed = false;
            }

            return View();
        }

        // GET: /AlumniElection/Notice
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Notice()
        {
            try
            {
                var activeSetting = GetActiveElectionSetting();
                ViewBag.ActiveSetting = activeSetting;
            }
            catch { }
            return View();
        }

        // Aliases for the Notice page
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Announcement()
        {
            return RedirectToAction("Notice");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Portal()
        {
            return RedirectToAction("Notice");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Info()
        {
            return RedirectToAction("Notice");
        }

        // GET: /AlumniElection/CheckMobile?mobile=9876543210
        [AllowAnonymous]
        [HttpGet]
        public ActionResult CheckMobile(string mobile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mobile))
                {
                    return Json(new { success = false, found = false, message = "Please enter a mobile number." }, JsonRequestBehavior.AllowGet);
                }

                string cleanMobile = Regex.Replace(mobile.Trim(), @"[^\d]", "");
                if (cleanMobile.Length < 10)
                {
                    return Json(new { success = false, found = false, message = "Please enter a valid 10-digit mobile number." }, JsonRequestBehavior.AllowGet);
                }
                if (cleanMobile.Length > 10)
                {
                    cleanMobile = cleanMobile.Substring(cleanMobile.Length - 10);
                }

                // 1. Check alumni_election_registration first
                var electionSql = @"SELECT ID, ACADEMIC_YEAR, NAME, MOBILE_NO, PROGRAMME, PROGRAMME_NAME, YEAR_OF_PASSING, ELECTION_STATUS 
                                    FROM alumni_election_registration 
                                    WHERE (MOBILE_NO = @MOBILE_NO OR RIGHT(MOBILE_NO, 10) = @MOBILE_NO) AND IS_DELETED != 1 
                                    ORDER BY ID DESC LIMIT 1;";
                var electionRecords = DapperHandler.FetchAll<AlumniElectionRegistration>(electionSql, new { MOBILE_NO = cleanMobile });

                if (electionRecords != null && electionRecords.Count > 0)
                {
                    var existing = electionRecords[0];
                    return Json(new
                    {
                        success = true,
                        found = true,
                        isEligible = true,
                        source = "election_registration",
                        alreadyRegistered = true,
                        message = "This mobile number is already registered! Your previous details have been retrieved below. You can update your response.",
                        registerUrl = "https://alumini.sjctnc.edu.in/",
                        data = new
                        {
                            id = existing.ID,
                            name = existing.NAME,
                            mobile = cleanMobile,
                            programme = existing.PROGRAMME,
                            programmeName = existing.PROGRAMME_NAME,
                            yearOfPassing = existing.YEAR_OF_PASSING,
                            electionStatus = existing.ELECTION_STATUS
                        }
                    }, JsonRequestBehavior.AllowGet);
                }

                // 2. If not found in election registration, check alumni_data_collection
                var alumniSql = @"SELECT 
                                        ID, NAME, MOBILE_NO, cpg.PROGRAMME_ID AS PROGRAMME, BATCH
                                    FROM
                                        alumni_data_collection AS adc
                                            INNER JOIN
                                        cp_programme_group AS cpg ON cpg.PROGRAMME_GROUP_ID = adc.PROGRAMME
                                    WHERE
                                        (MOBILE_NO = @MOBILE_NO
                                            OR RIGHT(REPLACE(REPLACE(REPLACE(REPLACE(MOBILE_NO, ' ', ''),
                                                        '-',
                                                        ''),
                                                    '+',
                                                    ''),
                                                '.',
                                                ''),
                                            10) = @MOBILE_NO
                                            OR MOBILE_NO LIKE @MOBILE_LIKE)
                                    ORDER BY ID DESC
                                    LIMIT 1;";
                var alumniRecords = DapperHandler.FetchAll<AlumniDataCollectionItem>(alumniSql, new { MOBILE_NO = cleanMobile, MOBILE_LIKE = "%" + cleanMobile });

                if (alumniRecords != null && alumniRecords.Count > 0)
                {
                    var alumni = alumniRecords[0];

                    // Extract year of passing from BATCH (e.g. "UG:2004-2007" -> 2007, "2003-2006" -> 2006)
                    string passingYear = "";
                    if (!string.IsNullOrWhiteSpace(alumni.BATCH))
                    {
                        var matches = Regex.Matches(alumni.BATCH, @"\b(19\d\d|20\d\d)\b");
                        if (matches.Count > 0)
                        {
                            passingYear = matches[matches.Count - 1].Value;
                        }
                    }

                    int? progId = null;
                    int parsedId;
                    if (!string.IsNullOrWhiteSpace(alumni.PROGRAMME))
                    {
                        if (int.TryParse(alumni.PROGRAMME.Trim(), out parsedId))
                        {
                            progId = parsedId;
                        }
                        else
                        {
                            try
                            {
                                var matchProg = DapperHandler.FetchAll<int>("SELECT PROGRAMME_ID FROM cp_programme WHERE LOWER(PROGRAMME_NAME) = LOWER(@PNAME) OR LOWER(PROGRAMME_CODE) = LOWER(@PNAME) LIMIT 1;", new { PNAME = alumni.PROGRAMME.Trim() });
                                if (matchProg != null && matchProg.Count > 0)
                                {
                                    progId = matchProg[0];
                                }
                            }
                            catch { }
                        }
                    }

                    return Json(new
                    {
                        success = true,
                        found = true,
                        isEligible = true,
                        source = "alumni_data_collection",
                        alreadyRegistered = false,
                        message = "Alumni record verified from college database! Your details have been pre-filled. Please confirm your attendance.",
                        registerUrl = "https://alumini.sjctnc.edu.in/",
                        data = new
                        {
                            id = 0,
                            name = alumni.NAME != null ? alumni.NAME.Trim() : "",
                            mobile = cleanMobile,
                            programme = progId,
                            programmeName = "",
                            yearOfPassing = passingYear,
                            electionStatus = (string)null
                        }
                    }, JsonRequestBehavior.AllowGet);
                }

                // 3. Not found in either table -> Candidate is NOT eligible
                return Json(new
                {
                    success = true,
                    found = false,
                    isEligible = false,
                    source = "none",
                    alreadyRegistered = false,
                    message = "Mobile number not found in Alumni Database. Only registered alumni are eligible for the election. Please register first at https://alumini.sjctnc.edu.in/",
                    registerUrl = "https://alumini.sjctnc.edu.in/",
                    data = (object)null
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog objLog = new ErrorLog())
                {
                    objLog.WriteError("AlumniElectionController", "CheckMobile", ex.Message);
                }
                return Json(new { success = false, found = false, isEligible = false, message = "Could not verify mobile number." }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: /AlumniElection/SubmitResponse
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SubmitResponse(AlumniElectionRegistration model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new { success = false, message = "Invalid form submission." }, JsonRequestBehavior.AllowGet);
                }

                // Check election setting deadline
                var activeSetting = GetActiveElectionSetting();
                if (activeSetting == null)
                {
                    return Json(new { success = false, message = "Election registration is not active at this time." }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrWhiteSpace(model.MOBILE_NO))
                {
                    return Json(new { success = false, message = "Please enter your mobile number." }, JsonRequestBehavior.AllowGet);
                }

                string cleanMobile = Regex.Replace(model.MOBILE_NO.Trim(), @"[^\d]", "");
                if (cleanMobile.Length < 10)
                {
                    return Json(new { success = false, message = "Please enter a valid 10-digit mobile number." }, JsonRequestBehavior.AllowGet);
                }
                if (cleanMobile.Length > 10)
                {
                    cleanMobile = cleanMobile.Substring(cleanMobile.Length - 10);
                }
                model.MOBILE_NO = cleanMobile;

                // Check if existing record exists in election registration
                var checkSql = "SELECT ID FROM alumni_election_registration WHERE (MOBILE_NO = @MOBILE_NO OR RIGHT(MOBILE_NO, 10) = @MOBILE_NO) AND IS_DELETED = 0 LIMIT 1;";
                var existingRecords = DapperHandler.FetchAll<int>(checkSql, new { MOBILE_NO = model.MOBILE_NO });
                bool isExisting = (existingRecords != null && existingRecords.Count > 0);

                // If not already in election registration, strictly verify existence in alumni_data_collection
                if (!isExisting)
                {
                    var alumniCheckSql = @"SELECT ID FROM alumni_data_collection 
                                           WHERE (MOBILE_NO = @MOBILE_NO 
                                              OR RIGHT(REPLACE(REPLACE(REPLACE(REPLACE(MOBILE_NO, ' ', ''), '-', ''), '+', ''), '.', ''), 10) = @MOBILE_NO 
                                              OR MOBILE_NO LIKE @MOBILE_LIKE) 
                                           LIMIT 1;";
                    var alumniMatch = DapperHandler.FetchAll<int>(alumniCheckSql, new { MOBILE_NO = cleanMobile, MOBILE_LIKE = "%" + cleanMobile });
                    if (alumniMatch == null || alumniMatch.Count == 0)
                    {
                        return Json(new
                        {
                            success = false,
                            isEligible = false,
                            message = "Submission blocked: Only registered alumni in the college database are eligible for the election. Please register first at https://alumini.sjctnc.edu.in/ before submitting.",
                            registerUrl = "https://alumini.sjctnc.edu.in/"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                // Enforce submission deadline for new submissions
                if (!isExisting && activeSetting.LAST_DATE_SUBMISSION.HasValue && DateTime.Now > activeSetting.LAST_DATE_SUBMISSION.Value)
                {
                    return Json(new { success = false, message = "The deadline for submitting election attendance was " + activeSetting.LAST_DATE_SUBMISSION.Value.ToString("dd-MMM-yyyy hh:mm tt") + ". New registrations are closed." }, JsonRequestBehavior.AllowGet);
                }

                // Fetch active academic year
                model.ACADEMIC_YEAR = activeSetting.ACADEMIC_YEAR ?? GetActiveAcademicYear();

                if (string.IsNullOrWhiteSpace(model.NAME))
                {
                    return Json(new { success = false, message = "Please enter your name." }, JsonRequestBehavior.AllowGet);
                }

                if (!model.PROGRAMME.HasValue || model.PROGRAMME.Value <= 0)
                {
                    return Json(new { success = false, message = "Please select your programme." }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrWhiteSpace(model.YEAR_OF_PASSING))
                {
                    return Json(new { success = false, message = "Please select your year of passing." }, JsonRequestBehavior.AllowGet);
                }

                string status = (model.ELECTION_STATUS ?? "").Trim().ToUpper();
                if (status != "APPROVED" && status != "REJECTED")
                {
                    return Json(new { success = false, message = "Please indicate whether you approve or reject coming for the election." }, JsonRequestBehavior.AllowGet);
                }
                model.ELECTION_STATUS = status;

                // Lookup PROGRAMME_NAME from cp_programme
                try
                {
                    var progSql = "SELECT PROGRAMME_NAME FROM cp_programme WHERE PROGRAMME_ID = @ID LIMIT 1;";
                    var pNames = DapperHandler.FetchAll<string>(progSql, new { ID = model.PROGRAMME.Value });
                    if (pNames != null && pNames.Count > 0)
                    {
                        model.PROGRAMME_NAME = pNames[0];
                    }
                }
                catch
                {
                    model.PROGRAMME_NAME = "";
                }

                // Capture IP address
                string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = Request.UserHostAddress;
                }
                model.IP_ADDRESS = ipAddress ?? string.Empty;

                model.NAME = model.NAME.Trim();
                model.YEAR_OF_PASSING = model.YEAR_OF_PASSING.Trim();

                bool isUpdate = false;
                int savedId = 0;
                if (isExisting)
                {
                    isUpdate = true;
                    int existingId = existingRecords[0];
                    model.ID = existingId;
                    savedId = existingId;

                    var updateSql = @"UPDATE alumni_election_registration 
                                      SET ACADEMIC_YEAR = @ACADEMIC_YEAR,
                                          NAME = @NAME, 
                                          PROGRAMME = @PROGRAMME, 
                                          PROGRAMME_NAME = @PROGRAMME_NAME,
                                          YEAR_OF_PASSING = @YEAR_OF_PASSING, 
                                          ELECTION_STATUS = @ELECTION_STATUS, 
                                          IP_ADDRESS = @IP_ADDRESS, 
                                          UPDATED_DATE = NOW() 
                                      WHERE ID = @ID;";
                    DapperHandler.SaveOrUpdate(updateSql, model);
                }
                else
                {
                    var insertSql = @"INSERT INTO alumni_election_registration 
                                          (ACADEMIC_YEAR, NAME, MOBILE_NO, PROGRAMME, PROGRAMME_NAME, YEAR_OF_PASSING, ELECTION_STATUS, IP_ADDRESS, CREATED_DATE, IS_DELETED) 
                                      VALUES 
                                          (@ACADEMIC_YEAR, @NAME, @MOBILE_NO, @PROGRAMME, @PROGRAMME_NAME, @YEAR_OF_PASSING, @ELECTION_STATUS, @IP_ADDRESS, NOW(), 0);";
                    DapperHandler.SaveOrUpdate(insertSql, model);

                    // Retrieve inserted ID
                    var idList = DapperHandler.FetchAll<int>("SELECT ID FROM alumni_election_registration WHERE MOBILE_NO = @MOBILE_NO ORDER BY ID DESC LIMIT 1;", new { MOBILE_NO = model.MOBILE_NO });
                    if (idList != null && idList.Count > 0)
                    {
                        savedId = idList[0];
                    }
                }

                string statusText = (model.ELECTION_STATUS == "APPROVED") ? "Approved (Attending)" : "Rejected (Not Attending)";
                string actionText = isUpdate ? "Your response has been updated" : "Your response has been recorded";

                return Json(new
                {
                    success = true,
                    isUpdate = isUpdate,
                    id = savedId,
                    message = actionText + " as " + statusText + ". Thank you!",
                    status = model.ELECTION_STATUS,
                    name = model.NAME,
                    mobile = model.MOBILE_NO,
                    programmeName = model.PROGRAMME_NAME,
                    academicYear = model.ACADEMIC_YEAR,
                    canTakeCard = (model.ELECTION_STATUS == "APPROVED")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog objLog = new ErrorLog())
                {
                    objLog.WriteError("AlumniElectionController", "SubmitResponse", ex.Message);
                }
                return Json(new { success = false, message = "An error occurred while saving your response. Please try again." }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /AlumniElection/AttendanceCard?mobile=9876543210
        [AllowAnonymous]
        [HttpGet]
        public ActionResult AttendanceCard(string mobile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mobile))
                {
                    ViewBag.ErrorMessage = "Mobile number is required to generate the Attendance Card.";
                    return View();
                }

                string cleanMobile = Regex.Replace(mobile.Trim(), @"[^\d]", "");
                if (cleanMobile.Length > 10) cleanMobile = cleanMobile.Substring(cleanMobile.Length - 10);

                var regSql = @"SELECT r.ID, r.ACADEMIC_YEAR, r.NAME, r.MOBILE_NO, r.PROGRAMME, 
                                      COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, '') AS PROGRAMME_NAME, 
                                      COALESCE(p.PROGRAMME_CODE, '') AS PROGRAMME_CODE,
                                      r.YEAR_OF_PASSING, r.ELECTION_STATUS, r.CREATED_DATE, r.UPDATED_DATE 
                               FROM alumni_election_registration r 
                               LEFT JOIN cp_programme p ON (r.PROGRAMME = p.PROGRAMME_ID OR (p.PROGRAMME_NAME = r.PROGRAMME_NAME AND p.IS_DELETED = 0))
                               WHERE (r.MOBILE_NO = @MOBILE OR RIGHT(r.MOBILE_NO, 10) = @MOBILE) AND r.IS_DELETED = 0 
                               ORDER BY r.ID DESC LIMIT 1;";
                var regList = DapperHandler.FetchAll<AlumniElectionRegistration>(regSql, new { MOBILE = cleanMobile });

                if (regList == null || regList.Count == 0)
                {
                    ViewBag.ErrorMessage = "No registration record found for mobile number: " + cleanMobile;
                    return View();
                }

                var registration = regList[0];
                if (string.IsNullOrWhiteSpace(registration.PROGRAMME_CODE) && (!string.IsNullOrWhiteSpace(registration.PROGRAMME_NAME) || registration.PROGRAMME.HasValue))
                {
                    try
                    {
                        var pCode = DapperHandler.FetchAll<string>("SELECT PROGRAMME_CODE FROM cp_programme WHERE (PROGRAMME_ID = @PROG OR PROGRAMME_NAME = @NAME) AND IS_DELETED = 0 LIMIT 1;", new { PROG = registration.PROGRAMME ?? 0, NAME = registration.PROGRAMME_NAME ?? "" });
                        if (pCode != null && pCode.Count > 0 && !string.IsNullOrWhiteSpace(pCode[0]))
                        {
                            registration.PROGRAMME_CODE = pCode[0].Trim();
                        }
                    }
                    catch { }
                }
                if (registration.ELECTION_STATUS != "APPROVED")
                {
                    ViewBag.ErrorMessage = "Attendance Card is only issued to alumni who have confirmed their attendance (Approved). Your current response is recorded as: " + registration.ELECTION_STATUS;
                    return View();
                }

                var activeSetting = GetActiveElectionSetting();
                string serialNo = GetAlumniSerialNo(cleanMobile, registration.ID);
                string year = !string.IsNullOrWhiteSpace(registration.ACADEMIC_YEAR) 
                    ? registration.ACADEMIC_YEAR.Trim() 
                    : (activeSetting != null && !string.IsNullOrWhiteSpace(activeSetting.ACADEMIC_YEAR) ? activeSetting.ACADEMIC_YEAR.Trim() : "2026");
                string passNo = "SJC-ELEC-" + year + "-" + registration.ID.ToString("D4");
                string regNo = "SJC-" + (!string.IsNullOrWhiteSpace(registration.PROGRAMME_CODE) ? registration.PROGRAMME_CODE.Trim() : "ELEC") + "-" + year + "-" + serialNo;

                registration.SERIAL_NO = passNo;
                registration.REGISTRATION_NO = regNo;

                var vm = new AlumniElectionCardViewModel
                {
                    Registration = registration,
                    Setting = activeSetting,
                    SerialNo = serialNo,
                    PassNo = passNo
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                using (ErrorLog objLog = new ErrorLog())
                {
                    objLog.WriteError("AlumniElectionController", "AttendanceCard", ex.Message);
                }
                ViewBag.ErrorMessage = "An error occurred while generating your Attendance Card.";
                return View();
            }
        }

        private string GetAlumniSerialNo(string mobile, int fallbackId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mobile))
                {
                    return fallbackId.ToString("D4");
                }

                string cleanMobile = Regex.Replace(mobile.Trim(), @"[^\d]", "");
                if (cleanMobile.Length > 10) cleanMobile = cleanMobile.Substring(cleanMobile.Length - 10);

                var findSql = @"SELECT ID, PROGRAMME FROM alumni_data_collection 
                                WHERE (MOBILE_NO = @MOBILE 
                                    OR RIGHT(REPLACE(REPLACE(REPLACE(REPLACE(MOBILE_NO, ' ', ''), '-', ''), '+', ''), '.', ''), 10) = @MOBILE 
                                    OR MOBILE_NO LIKE @MOBILE_LIKE) 
                                ORDER BY ID DESC LIMIT 1;";
                var match = DapperHandler.FetchAll<AlumniDataCollectionItem>(findSql, new { MOBILE = cleanMobile, MOBILE_LIKE = "%" + cleanMobile });

                if (match != null && match.Count > 0 && !string.IsNullOrWhiteSpace(match[0].PROGRAMME))
                {
                    var target = match[0];
                    var countSql = @"SELECT COUNT(*) FROM alumni_data_collection 
                                     WHERE TRIM(PROGRAMME) = TRIM(@PROG) AND ID <= @ID;";
                    var counts = DapperHandler.FetchAll<int>(countSql, new { PROG = target.PROGRAMME, ID = target.ID });
                    if (counts != null && counts.Count > 0 && counts[0] > 0)
                    {
                        return counts[0].ToString("D4");
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objLog = new ErrorLog())
                {
                    objLog.WriteError("AlumniElectionController", "GetAlumniSerialNo", ex.Message);
                }
            }

            return fallbackId.ToString("D4");
        }

        // =========================================================================
        // ADMIN MANAGEMENT ACTIONS (Login Required)
        // =========================================================================

        // GET: /AlumniElection/AlumniElectionSetting
        [HttpGet]
        public ActionResult AlumniElectionSetting()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            try
            {
                var sql = @"SELECT ID, SETTING_NAME, ELECTION_DATE, LAST_DATE_SUBMISSION, NOTICE_PDF_PATH, 
                                   NOTES, ELECTION_PLACE, ELECTION_TIME, ACADEMIC_YEAR, IS_ACTIVE, IS_DELETED, CREATED_DATE, UPDATED_DATE, CREATED_BY 
                            FROM alumni_election_setting 
                            WHERE IS_DELETED = 0 
                            ORDER BY IS_ACTIVE DESC, ID DESC;";
                var list = DapperHandler.FetchAll<AlumniElectionSetting>(sql) ?? new List<AlumniElectionSetting>();
                ViewBag.ActiveAcademicYear = GetActiveAcademicYear();
                return View(list);
            }
            catch (Exception ex)
            {
                using (ErrorLog objLog = new ErrorLog())
                {
                    objLog.WriteError("AlumniElectionController", "AlumniElectionSetting", ex.Message);
                }
                return View(new List<AlumniElectionSetting>());
            }
        }

        // Alias GET: /AlumniElection/Settings
        [HttpGet]
        public ActionResult Settings()
        {
            return RedirectToAction("AlumniElectionSetting");
        }

        // GET: /AlumniElection/GetSetting?id=1
        [HttpGet]
        public ActionResult GetSetting(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return Json(new { success = false, message = "Unauthorized. Please log in as admin." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var sql = @"SELECT ID, SETTING_NAME, ELECTION_DATE, LAST_DATE_SUBMISSION, NOTICE_PDF_PATH, 
                                   NOTES, ELECTION_PLACE, ELECTION_TIME, ACADEMIC_YEAR, IS_ACTIVE 
                            FROM alumni_election_setting 
                            WHERE ID = @ID AND IS_DELETED = 0 LIMIT 1;";
                var list = DapperHandler.FetchAll<AlumniElectionSetting>(sql, new { ID = id });
                if (list != null && list.Count > 0)
                {
                    var item = list[0];
                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            id = item.ID,
                            settingName = item.SETTING_NAME,
                            electionDate = item.ELECTION_DATE.HasValue ? item.ELECTION_DATE.Value.ToString("yyyy-MM-dd") : "",
                            lastDateSubmission = item.LAST_DATE_SUBMISSION.HasValue ? item.LAST_DATE_SUBMISSION.Value.ToString("yyyy-MM-ddTHH:mm") : "",
                            noticePdfPath = item.NOTICE_PDF_PATH,
                            notes = item.NOTES,
                            electionPlace = item.ELECTION_PLACE,
                            electionTime = item.ELECTION_TIME,
                            academicYear = item.ACADEMIC_YEAR,
                            isActive = item.IS_ACTIVE == 1
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Setting not found." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: /AlumniElection/SaveSetting
        [HttpPost]
        public ActionResult SaveSetting(AlumniElectionSetting model, HttpPostedFileBase noticePdfFile)
        {
            if (!IsAdminLoggedIn())
            {
                return Json(new { success = false, message = "Unauthorized. Please log in as admin." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.SETTING_NAME))
                {
                    return Json(new { success = false, message = "Setting Name is required." }, JsonRequestBehavior.AllowGet);
                }

                // Handle PDF upload if provided
                if (noticePdfFile != null && noticePdfFile.ContentLength > 0)
                {
                    string ext = Path.GetExtension(noticePdfFile.FileName).ToLower();
                    if (ext != ".pdf")
                    {
                        return Json(new { success = false, message = "Only PDF files are accepted for Official Election Notice." }, JsonRequestBehavior.AllowGet);
                    }

                    string uploadsDir = Server.MapPath("~/Content/election_docs/uploads/");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    string safeName = "Notice_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + Path.GetFileName(noticePdfFile.FileName);
                    string targetFilePath = Path.Combine(uploadsDir, safeName);
                    noticePdfFile.SaveAs(targetFilePath);
                    model.NOTICE_PDF_PATH = "/Content/election_docs/uploads/" + safeName;
                }

                string currentUser = (Session != null && Session[Common.SESSION_VARIABLES.USERNAME] != null) ? Session[Common.SESSION_VARIABLES.USERNAME].ToString() : "Admin";

                if (string.IsNullOrWhiteSpace(model.ACADEMIC_YEAR))
                {
                    model.ACADEMIC_YEAR = GetActiveAcademicYear();
                }

                // If setting is marked active, deactivate others
                if (model.IS_ACTIVE == 1)
                {
                    var deactSql = "UPDATE alumni_election_setting SET IS_ACTIVE = 0 WHERE IS_DELETED = 0;";
                    DapperHandler.SaveOrUpdate(deactSql);
                }

                if (model.ID > 0)
                {
                    var updateSql = @"UPDATE alumni_election_setting 
                                      SET SETTING_NAME = @SETTING_NAME, 
                                          ELECTION_DATE = @ELECTION_DATE, 
                                          LAST_DATE_SUBMISSION = @LAST_DATE_SUBMISSION, 
                                          NOTICE_PDF_PATH = COALESCE(@NOTICE_PDF_PATH, NOTICE_PDF_PATH), 
                                          NOTES = @NOTES, 
                                          ELECTION_PLACE = @ELECTION_PLACE, 
                                          ELECTION_TIME = @ELECTION_TIME, 
                                          ACADEMIC_YEAR = @ACADEMIC_YEAR, 
                                          IS_ACTIVE = @IS_ACTIVE, 
                                          UPDATED_DATE = NOW() 
                                      WHERE ID = @ID;";
                    DapperHandler.SaveOrUpdate(updateSql, model);
                    return Json(new { success = true, message = "Election setting updated successfully." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.CREATED_BY = currentUser;
                    if (string.IsNullOrEmpty(model.NOTICE_PDF_PATH))
                    {
                        model.NOTICE_PDF_PATH = "/Content/election_docs/Alumni_Election_Notice_2026.pdf";
                    }
                    var insertSql = @"INSERT INTO alumni_election_setting 
                                      (SETTING_NAME, ELECTION_DATE, LAST_DATE_SUBMISSION, NOTICE_PDF_PATH, NOTES, ELECTION_PLACE, ELECTION_TIME, ACADEMIC_YEAR, IS_ACTIVE, IS_DELETED, CREATED_DATE, CREATED_BY) 
                                      VALUES 
                                      (@SETTING_NAME, @ELECTION_DATE, @LAST_DATE_SUBMISSION, @NOTICE_PDF_PATH, @NOTES, @ELECTION_PLACE, @ELECTION_TIME, @ACADEMIC_YEAR, @IS_ACTIVE, 0, NOW(), @CREATED_BY);";
                    DapperHandler.SaveOrUpdate(insertSql, model);
                    return Json(new { success = true, message = "Election setting created successfully." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objLog = new ErrorLog())
                {
                    objLog.WriteError("AlumniElectionController", "SaveSetting", ex.Message);
                }
                return Json(new { success = false, message = "Error saving setting: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: /AlumniElection/SetActiveSetting?id=1
        [HttpPost]
        public ActionResult SetActiveSetting(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return Json(new { success = false, message = "Unauthorized. Please log in as admin." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                DapperHandler.SaveOrUpdate("UPDATE alumni_election_setting SET IS_ACTIVE = 0 WHERE IS_DELETED = 0;");
                DapperHandler.SaveOrUpdate("UPDATE alumni_election_setting SET IS_ACTIVE = 1, UPDATED_DATE = NOW() WHERE ID = @ID;", new { ID = id });
                return Json(new { success = true, message = "Election setting marked as active." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: /AlumniElection/DeleteSetting?id=1
        [HttpPost]
        public ActionResult DeleteSetting(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return Json(new { success = false, message = "Unauthorized. Please log in as admin." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                DapperHandler.SaveOrUpdate("UPDATE alumni_election_setting SET IS_DELETED = 1, IS_ACTIVE = 0, UPDATED_DATE = NOW() WHERE ID = @ID;", new { ID = id });
                return Json(new { success = true, message = "Election setting deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // =========================================================================
        // ADMIN REPORTS ACTIONS (Login Required)
        // =========================================================================

        // GET: /AlumniElection/Reports
        [HttpGet]
        public ActionResult Reports(string academicYear, int? programme, string status)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            try
            {
                // Fetch list of programmes
                var progSql = @"SELECT PROGRAMME_ID, PROGRAMME_NAME 
                                FROM cp_programme 
                                WHERE IS_DELETED = 0 AND PROGRAMME_NAME IS NOT NULL AND TRIM(PROGRAMME_NAME) != '' 
                                ORDER BY PROGRAMME_NAME ASC;";
                ViewBag.ProgrammeList = DapperHandler.FetchAll<ProgrammeDropdownItem>(progSql) ?? new List<ProgrammeDropdownItem>();

                // Fetch academic years
                var acYearSql = "SELECT DISTINCT ACADEMIC_YEAR FROM alumni_election_registration WHERE ACADEMIC_YEAR IS NOT NULL AND IS_DELETED = 0 ORDER BY ACADEMIC_YEAR DESC;";
                var acYears = DapperHandler.FetchAll<string>(acYearSql) ?? new List<string>();
                string activeYear = GetActiveAcademicYear();
                if (!acYears.Contains(activeYear))
                {
                    acYears.Insert(0, activeYear);
                }
                ViewBag.AcademicYearList = acYears;
                ViewBag.SelectedAcademicYear = !string.IsNullOrEmpty(academicYear) ? academicYear : activeYear;
                ViewBag.SelectedProgramme = programme;
                ViewBag.SelectedStatus = status;

                return View();
            }
            catch (Exception ex)
            {
                using (ErrorLog objLog = new ErrorLog())
                {
                    objLog.WriteError("AlumniElectionController", "Reports", ex.Message);
                }
                return View();
            }
        }

        private List<int> ParseProgrammeIds(string programme)
        {
            var progIds = new List<int>();
            if (!string.IsNullOrWhiteSpace(programme) && programme != "ALL")
            {
                var parts = programme.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    int id;
                    if (int.TryParse(part.Trim(), out id) && id > 0)
                    {
                        progIds.Add(id);
                    }
                }
            }
            return progIds;
        }

        private List<string> ParseStatuses(string status)
        {
            var statuses = new List<string>();
            if (!string.IsNullOrWhiteSpace(status) && status != "ALL")
            {
                var parts = status.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    var s = part.Trim().ToUpper();
                    if (s == "APPROVED" || s == "REJECTED")
                    {
                        statuses.Add(s);
                    }
                }
            }
            return statuses;
        }

        // GET: /AlumniElection/GetReportData
        [HttpGet]
        public ActionResult GetReportData(string academicYear, string programme, string status)
        {
            if (!IsAdminLoggedIn())
            {
                return Json(new { success = false, message = "Unauthorized. Please log in as admin." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var progIds = ParseProgrammeIds(programme);
                var statuses = ParseStatuses(status);

                // 1. Fetch detailed registrations sorted alphabetically by Name
                var whereClause = new StringBuilder("WHERE r.IS_DELETED = 0 ");
                var p = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(academicYear) && academicYear != "ALL")
                {
                    whereClause.Append("AND r.ACADEMIC_YEAR = @AC_YEAR ");
                    p.Add("AC_YEAR", academicYear.Trim());
                }

                if (progIds.Count > 0)
                {
                    whereClause.Append("AND r.PROGRAMME IN (" + string.Join(",", progIds) + ") ");
                }

                if (statuses.Count == 1)
                {
                    whereClause.Append("AND r.ELECTION_STATUS = @STATUS ");
                    p.Add("STATUS", statuses[0]);
                }
                else if (statuses.Count > 1)
                {
                    whereClause.Append("AND r.ELECTION_STATUS IN ('" + string.Join("','", statuses) + "') ");
                }

                var listSql = @"SELECT r.ID, r.ACADEMIC_YEAR, r.NAME, r.MOBILE_NO, r.PROGRAMME, 
                                       COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified') AS PROGRAMME_NAME, 
                                       COALESCE(p.PROGRAMME_CODE, '') AS PROGRAMME_CODE,
                                       r.YEAR_OF_PASSING, r.ELECTION_STATUS, r.CREATED_DATE, r.UPDATED_DATE 
                                FROM alumni_election_registration r 
                                LEFT JOIN cp_programme p ON r.PROGRAMME = p.PROGRAMME_ID 
                                " + whereClause.ToString() + @"
                                ORDER BY r.NAME ASC;";

                var registrations = DapperHandler.FetchAll<AlumniElectionRegistration>(listSql, p) ?? new List<AlumniElectionRegistration>();

                // 2. Fetch Programme-Wise Summary
                var summaryWhere = new StringBuilder("WHERE r.IS_DELETED = 0 ");
                var sp = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(academicYear) && academicYear != "ALL")
                {
                    summaryWhere.Append("AND r.ACADEMIC_YEAR = @AC_YEAR ");
                    sp.Add("AC_YEAR", academicYear.Trim());
                }
                if (progIds.Count > 0)
                {
                    summaryWhere.Append("AND r.PROGRAMME IN (" + string.Join(",", progIds) + ") ");
                }
                if (statuses.Count == 1)
                {
                    summaryWhere.Append("AND r.ELECTION_STATUS = @STATUS ");
                    sp.Add("STATUS", statuses[0]);
                }
                else if (statuses.Count > 1)
                {
                    summaryWhere.Append("AND r.ELECTION_STATUS IN ('" + string.Join("','", statuses) + "') ");
                }

                var summarySql = @"SELECT 
                                       COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified') AS PROGRAMME_NAME,
                                       COUNT(*) AS TOTAL_COUNT,
                                       SUM(CASE WHEN r.ELECTION_STATUS = 'APPROVED' THEN 1 ELSE 0 END) AS APPROVED_COUNT,
                                       SUM(CASE WHEN r.ELECTION_STATUS = 'REJECTED' THEN 1 ELSE 0 END) AS REJECTED_COUNT
                                   FROM alumni_election_registration r
                                   LEFT JOIN cp_programme p ON r.PROGRAMME = p.PROGRAMME_ID
                                   " + summaryWhere.ToString() + @"
                                   GROUP BY COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified')
                                   ORDER BY PROGRAMME_NAME ASC;";

                var programmeSummary = DapperHandler.FetchAll<ProgrammeWiseElectionReportItem>(summarySql, sp) ?? new List<ProgrammeWiseElectionReportItem>();

                // Calculate summary KPIs
                int totalRegistered = registrations.Count;
                int approvedCount = registrations.Count(x => x.ELECTION_STATUS == "APPROVED");
                int rejectedCount = registrations.Count(x => x.ELECTION_STATUS == "REJECTED");
                int totalProgrammes = programmeSummary.Count;

                var detailedList = registrations.Select((r, index) => new
                {
                    sno = index + 1,
                    id = r.ID,
                    regNo = "SJC-ELEC-" + (!string.IsNullOrWhiteSpace(r.ACADEMIC_YEAR) ? r.ACADEMIC_YEAR.Trim() : "2026") + "-" + r.ID.ToString("D4"),
                    name = r.NAME,
                    mobile = r.MOBILE_NO,
                    programme = r.PROGRAMME_NAME,
                    yearOfPassing = r.YEAR_OF_PASSING,
                    status = r.ELECTION_STATUS,
                    academicYear = r.ACADEMIC_YEAR,
                    createdDate = r.CREATED_DATE.HasValue ? r.CREATED_DATE.Value.ToString("dd-MMM-yyyy hh:mm tt") : "-"
                }).ToList();

                var programmeWiseList = registrations
                    .OrderBy(r => !string.IsNullOrWhiteSpace(r.PROGRAMME_NAME) ? r.PROGRAMME_NAME.Trim() : "Unspecified", StringComparer.CurrentCultureIgnoreCase)
                    .ThenBy(r => !string.IsNullOrWhiteSpace(r.NAME) ? r.NAME.Trim() : "", StringComparer.CurrentCultureIgnoreCase)
                    .Select((r, index) => new
                    {
                        sno = index + 1,
                        id = r.ID,
                        regNo = "SJC-ELEC-" + (!string.IsNullOrWhiteSpace(r.ACADEMIC_YEAR) ? r.ACADEMIC_YEAR.Trim() : "2026") + "-" + r.ID.ToString("D4"),
                        name = r.NAME,
                        mobile = r.MOBILE_NO,
                        programme = !string.IsNullOrWhiteSpace(r.PROGRAMME_NAME) ? r.PROGRAMME_NAME.Trim() : "Unspecified",
                        yearOfPassing = r.YEAR_OF_PASSING,
                        status = r.ELECTION_STATUS,
                        academicYear = r.ACADEMIC_YEAR,
                        createdDate = r.CREATED_DATE.HasValue ? r.CREATED_DATE.Value.ToString("dd-MMM-yyyy hh:mm tt") : "-"
                    }).ToList();

                return Json(new
                {
                    success = true,
                    kpis = new
                    {
                        totalRegistered = totalRegistered,
                        approvedCount = approvedCount,
                        rejectedCount = rejectedCount,
                        totalProgrammes = totalProgrammes
                    },
                    registrations = detailedList,
                    programmeSummary = programmeSummary,
                    programmeWiseRegistrations = programmeWiseList
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog objLog = new ErrorLog())
                {
                    objLog.WriteError("AlumniElectionController", "GetReportData", ex.Message);
                }
                return Json(new { success = false, message = "Failed to load report data: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /AlumniElection/ExportReportsToExcel
        [HttpGet]
        public ActionResult ExportReportsToExcel(string academicYear, string programme, string status, string reportType, bool includeSignature = false)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            try
            {
                StringBuilder sb = new StringBuilder();
                var progIds = ParseProgrammeIds(programme);
                var statuses = ParseStatuses(status);

                if (reportType == "programme")
                {
                    // Programme-wise Summary Excel
                    var where = new StringBuilder("WHERE r.IS_DELETED = 0 ");
                    var p = new Dictionary<string, object>();
                    if (!string.IsNullOrWhiteSpace(academicYear) && academicYear != "ALL")
                    {
                        where.Append("AND r.ACADEMIC_YEAR = @AC_YEAR ");
                        p.Add("AC_YEAR", academicYear.Trim());
                    }
                    if (progIds.Count > 0)
                    {
                        where.Append("AND r.PROGRAMME IN (" + string.Join(",", progIds) + ") ");
                    }
                    if (statuses.Count == 1)
                    {
                        where.Append("AND r.ELECTION_STATUS = @STATUS ");
                        p.Add("STATUS", statuses[0]);
                    }
                    else if (statuses.Count > 1)
                    {
                        where.Append("AND r.ELECTION_STATUS IN ('" + string.Join("','", statuses) + "') ");
                    }

                    var summarySql = @"SELECT 
                                           COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified') AS PROGRAMME_NAME,
                                           COUNT(*) AS TOTAL_COUNT,
                                           SUM(CASE WHEN r.ELECTION_STATUS = 'APPROVED' THEN 1 ELSE 0 END) AS APPROVED_COUNT,
                                           SUM(CASE WHEN r.ELECTION_STATUS = 'REJECTED' THEN 1 ELSE 0 END) AS REJECTED_COUNT
                                       FROM alumni_election_registration r
                                       LEFT JOIN cp_programme p ON r.PROGRAMME = p.PROGRAMME_ID
                                       " + where.ToString() + @"
                                       GROUP BY COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified')
                                       ORDER BY PROGRAMME_NAME ASC;";

                    var summaryList = DapperHandler.FetchAll<ProgrammeWiseElectionReportItem>(summarySql, p) ?? new List<ProgrammeWiseElectionReportItem>();

                    sb.Append("<table border='1'>");
                    sb.Append("<tr><th colspan='6' style='background-color: #1e3a8a; color: #ffffff; font-size: 15pt; font-weight: bold; text-align: center; padding: 10px;'>ST. JOSEPH'S COLLEGE - ALUMNI ELECTION PROGRAMME SUMMARY REPORT</th></tr>");
                    sb.Append("<tr><th colspan='6' style='background-color: #f1f5f9; font-size: 10pt; text-align: center;'>Academic Year: " + (academicYear ?? "All") + " | Generated On: " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") + "</th></tr>");
                    sb.Append("<tr style='background-color: #e2e8f0; font-weight: bold;'>");
                    sb.Append("<th style='width: 50px; text-align: center;'>#</th>");
                    sb.Append("<th style='text-align: left;'>Programme Name</th>");
                    sb.Append("<th style='width: 140px; text-align: right;'>Total Registered</th>");
                    sb.Append("<th style='width: 160px; text-align: right;'>Attending (Approved)</th>");
                    sb.Append("<th style='width: 180px; text-align: right;'>Cannot Attend (Rejected)</th>");
                    sb.Append("<th style='width: 130px; text-align: right;'>Attendance %</th>");
                    sb.Append("</tr>");

                    int sno = 1;
                    int totRegistered = 0, totApproved = 0, totRejected = 0;
                    foreach (var item in summaryList)
                    {
                        totRegistered += item.TOTAL_COUNT;
                        totApproved += item.APPROVED_COUNT;
                        totRejected += item.REJECTED_COUNT;

                        sb.Append("<tr>");
                        sb.Append("<td style='text-align: center;'>" + (sno++) + "</td>");
                        sb.Append("<td style='font-weight: bold;'>" + item.PROGRAMME_NAME + "</td>");
                        sb.Append("<td style='text-align: right;'>" + item.TOTAL_COUNT + "</td>");
                        sb.Append("<td style='text-align: right; color: #059669; font-weight: bold;'>" + item.APPROVED_COUNT + "</td>");
                        sb.Append("<td style='text-align: right; color: #dc2626;'>" + item.REJECTED_COUNT + "</td>");
                        sb.Append("<td style='text-align: right; font-weight: bold;'>" + item.ATTENDANCE_PERCENTAGE + "%</td>");
                        sb.Append("</tr>");
                    }

                    double overallPct = (totRegistered > 0) ? Math.Round(((double)totApproved / totRegistered) * 100.0, 1) : 0.0;
                    sb.Append("<tr style='background-color: #f8fafc; font-weight: bold;'>");
                    sb.Append("<td colspan='2' style='text-align: right;'>TOTAL:</td>");
                    sb.Append("<td style='text-align: right; color: #1e3a8a;'>" + totRegistered + "</td>");
                    sb.Append("<td style='text-align: right; color: #059669;'>" + totApproved + "</td>");
                    sb.Append("<td style='text-align: right; color: #dc2626;'>" + totRejected + "</td>");
                    sb.Append("<td style='text-align: right; color: #1e3a8a;'>" + overallPct + "%</td>");
                    sb.Append("</tr>");
                    sb.Append("</table>");

                    string fileName = "AlumniElection_ProgrammeSummary_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xls";
                    return File(Encoding.UTF8.GetBytes(sb.ToString()), "application/vnd.ms-excel", fileName);
                }
                else if (reportType == "programme_sheets")
                {
                    // Option 1: Programme-wise Excel with a SEPARATE SHEET PER PROGRAMME (.xlsx)
                    var where = new StringBuilder("WHERE r.IS_DELETED = 0 ");
                    var p = new Dictionary<string, object>();

                    if (!string.IsNullOrWhiteSpace(academicYear) && academicYear != "ALL")
                    {
                        where.Append("AND r.ACADEMIC_YEAR = @AC_YEAR ");
                        p.Add("AC_YEAR", academicYear.Trim());
                    }
                    if (progIds.Count > 0)
                    {
                        where.Append("AND r.PROGRAMME IN (" + string.Join(",", progIds) + ") ");
                    }
                    if (statuses.Count == 1)
                    {
                        where.Append("AND r.ELECTION_STATUS = @STATUS ");
                        p.Add("STATUS", statuses[0]);
                    }
                    else if (statuses.Count > 1)
                    {
                        where.Append("AND r.ELECTION_STATUS IN ('" + string.Join("','", statuses) + "') ");
                    }

                    var listSql = @"SELECT r.ID, r.ACADEMIC_YEAR, r.NAME, r.MOBILE_NO, r.PROGRAMME, 
                                           COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified') AS PROGRAMME_NAME, 
                                           COALESCE(p.PROGRAMME_CODE, '') AS PROGRAMME_CODE,
                                           r.YEAR_OF_PASSING, r.ELECTION_STATUS, r.CREATED_DATE 
                                    FROM alumni_election_registration r 
                                    LEFT JOIN cp_programme p ON r.PROGRAMME = p.PROGRAMME_ID 
                                    " + where.ToString() + @"
                                    ORDER BY COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified') ASC, r.NAME ASC;";

                    var dataList = DapperHandler.FetchAll<AlumniElectionRegistration>(listSql, p) ?? new List<AlumniElectionRegistration>();

                    var grouped = dataList
                        .GroupBy(x => !string.IsNullOrWhiteSpace(x.PROGRAMME_NAME) ? x.PROGRAMME_NAME.Trim() : "Unspecified")
                        .OrderBy(g => g.Key, StringComparer.CurrentCultureIgnoreCase)
                        .ToList();

                    try
                    {
                        return ExportProgrammeSheetsWithEPPlus(grouped, academicYear, includeSignature);
                    }
                    catch (Exception epEx)
                    {
                        using (ErrorLog objLog = new ErrorLog())
                        {
                            objLog.WriteError("AlumniElectionController", "ExportProgrammeSheetsWithEPPlus_Fallback", epEx.ToString());
                        }
                        byte[] xmlBytes = GenerateSpreadsheetMLProgrammeSheets(grouped, academicYear, includeSignature);
                        string fileName = "AlumniElection_ProgrammeWise_SeparateSheets_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xls";
                        return File(xmlBytes, "application/vnd.ms-excel", fileName);
                    }
                }
                else if (reportType == "programme_files" || reportType == "programme_zip")
                {
                    // Option 2: Programme-wise SEPARATE EXCEL FILE PER PROGRAMME (ZIP Bundle / Single XLSX)
                    var where = new StringBuilder("WHERE r.IS_DELETED = 0 ");
                    var p = new Dictionary<string, object>();

                    if (!string.IsNullOrWhiteSpace(academicYear) && academicYear != "ALL")
                    {
                        where.Append("AND r.ACADEMIC_YEAR = @AC_YEAR ");
                        p.Add("AC_YEAR", academicYear.Trim());
                    }
                    if (progIds.Count > 0)
                    {
                        where.Append("AND r.PROGRAMME IN (" + string.Join(",", progIds) + ") ");
                    }
                    if (statuses.Count == 1)
                    {
                        where.Append("AND r.ELECTION_STATUS = @STATUS ");
                        p.Add("STATUS", statuses[0]);
                    }
                    else if (statuses.Count > 1)
                    {
                        where.Append("AND r.ELECTION_STATUS IN ('" + string.Join("','", statuses) + "') ");
                    }

                    var listSql = @"SELECT r.ID, r.ACADEMIC_YEAR, r.NAME, r.MOBILE_NO, r.PROGRAMME, 
                                           COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified') AS PROGRAMME_NAME, 
                                           COALESCE(p.PROGRAMME_CODE, '') AS PROGRAMME_CODE,
                                           r.YEAR_OF_PASSING, r.ELECTION_STATUS, r.CREATED_DATE 
                                    FROM alumni_election_registration r 
                                    LEFT JOIN cp_programme p ON r.PROGRAMME = p.PROGRAMME_ID 
                                    " + where.ToString() + @"
                                    ORDER BY COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified') ASC, r.NAME ASC;";

                    var dataList = DapperHandler.FetchAll<AlumniElectionRegistration>(listSql, p) ?? new List<AlumniElectionRegistration>();

                    var grouped = dataList
                        .GroupBy(x => !string.IsNullOrWhiteSpace(x.PROGRAMME_NAME) ? x.PROGRAMME_NAME.Trim() : "Unspecified")
                        .OrderBy(g => g.Key, StringComparer.CurrentCultureIgnoreCase)
                        .ToList();

                    try
                    {
                        return ExportProgrammeFilesWithEPPlus(grouped, academicYear, includeSignature);
                    }
                    catch (Exception epEx)
                    {
                        using (ErrorLog objLog = new ErrorLog())
                        {
                            objLog.WriteError("AlumniElectionController", "ExportProgrammeFilesWithEPPlus_Fallback", epEx.ToString());
                        }
                        return ExportProgrammeFilesFallback(grouped, academicYear, includeSignature);
                    }
                }
                else if (reportType == "programme_list")
                {
                    // Programme-wise Registered Alumni List Excel
                    var where = new StringBuilder("WHERE r.IS_DELETED = 0 ");
                    var p = new Dictionary<string, object>();

                    if (!string.IsNullOrWhiteSpace(academicYear) && academicYear != "ALL")
                    {
                        where.Append("AND r.ACADEMIC_YEAR = @AC_YEAR ");
                        p.Add("AC_YEAR", academicYear.Trim());
                    }
                    if (progIds.Count > 0)
                    {
                        where.Append("AND r.PROGRAMME IN (" + string.Join(",", progIds) + ") ");
                    }
                    if (statuses.Count == 1)
                    {
                        where.Append("AND r.ELECTION_STATUS = @STATUS ");
                        p.Add("STATUS", statuses[0]);
                    }
                    else if (statuses.Count > 1)
                    {
                        where.Append("AND r.ELECTION_STATUS IN ('" + string.Join("','", statuses) + "') ");
                    }

                    var listSql = @"SELECT r.ID, r.ACADEMIC_YEAR, r.NAME, r.MOBILE_NO, r.PROGRAMME, 
                                           COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified') AS PROGRAMME_NAME, 
                                           COALESCE(p.PROGRAMME_CODE, '') AS PROGRAMME_CODE,
                                           r.YEAR_OF_PASSING, r.ELECTION_STATUS, r.CREATED_DATE 
                                    FROM alumni_election_registration r 
                                    LEFT JOIN cp_programme p ON r.PROGRAMME = p.PROGRAMME_ID 
                                    " + where.ToString() + @"
                                    ORDER BY COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified') ASC, r.NAME ASC;";

                    var dataList = DapperHandler.FetchAll<AlumniElectionRegistration>(listSql, p) ?? new List<AlumniElectionRegistration>();

                    string colSpanTitle = includeSignature ? "9" : "8";
                    sb.Append("<table border='1'>");
                    sb.Append("<tr><th colspan='" + colSpanTitle + "' style='background-color: #1e3a8a; color: #ffffff; font-size: 15pt; font-weight: bold; text-align: center; padding: 10px;'>ST. JOSEPH'S COLLEGE - ALUMNI ELECTION PROGRAMME-WISE REGISTERED ALUMNI REPORT</th></tr>");
                    sb.Append("<tr><th colspan='" + colSpanTitle + "' style='background-color: #f1f5f9; font-size: 10pt; text-align: center;'>Academic Year: " + (academicYear ?? "All") + " | Generated On: " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") + "</th></tr>");
                    sb.Append("<tr style='background-color: #e2e8f0; font-weight: bold;'>");
                    sb.Append("<th style='width: 50px; text-align: center;'>#</th>");
                    sb.Append("<th style='text-align: center;'>Registration No</th>");
                    sb.Append("<th style='text-align: left;'>Programme / Degree</th>");
                    sb.Append("<th style='text-align: left;'>Alumnus Name</th>");
                    sb.Append("<th style='text-align: center;'>Mobile Number</th>");
                    sb.Append("<th style='text-align: center;'>Year of Passing</th>");
                    sb.Append("<th style='text-align: center;'>Attendance Status</th>");
                    sb.Append("<th style='text-align: center;'>Registration Date</th>");
                    if (includeSignature)
                    {
                        sb.Append("<th style='width: 140px; text-align: center;'>Signature</th>");
                    }
                    sb.Append("</tr>");

                    int sno = 1;
                    string currentProg = null;
                    foreach (var item in dataList)
                    {
                        string progName = !string.IsNullOrWhiteSpace(item.PROGRAMME_NAME) ? item.PROGRAMME_NAME : "Unspecified";
                        if (currentProg != progName)
                        {
                            currentProg = progName;
                            int progCount = dataList.Count(x => (!string.IsNullOrWhiteSpace(x.PROGRAMME_NAME) ? x.PROGRAMME_NAME : "Unspecified") == currentProg);
                            int progApproved = dataList.Count(x => (!string.IsNullOrWhiteSpace(x.PROGRAMME_NAME) ? x.PROGRAMME_NAME : "Unspecified") == currentProg && x.ELECTION_STATUS == "APPROVED");
                            int progRejected = dataList.Count(x => (!string.IsNullOrWhiteSpace(x.PROGRAMME_NAME) ? x.PROGRAMME_NAME : "Unspecified") == currentProg && x.ELECTION_STATUS == "REJECTED");
                            sb.Append("<tr style='background-color: #dbeafe; font-weight: bold;'>");
                            sb.Append("<td colspan='" + colSpanTitle + "' style='padding: 8px 12px; color: #1e3a8a;'>Programme: " + currentProg + " (Total: " + progCount + " | Attending: " + progApproved + " | Cannot Attend: " + progRejected + ")</td>");
                            sb.Append("</tr>");
                        }

                        string regNo = "SJC-ELEC-" + (!string.IsNullOrWhiteSpace(item.ACADEMIC_YEAR) ? item.ACADEMIC_YEAR.Trim() : "2026") + "-" + item.ID.ToString("D4");
                        string statusColor = (item.ELECTION_STATUS == "APPROVED") ? "#059669" : "#dc2626";
                        string statusLabel = (item.ELECTION_STATUS == "APPROVED") ? "ATTENDING (APPROVED)" : "CANNOT ATTEND (REJECTED)";

                        sb.Append("<tr>");
                        sb.Append("<td style='text-align: center;'>" + (sno++) + "</td>");
                        sb.Append("<td style='text-align: center; font-weight: bold; color: #1e3a8a;'>" + regNo + "</td>");
                        sb.Append("<td>" + item.PROGRAMME_NAME + "</td>");
                        sb.Append("<td style='font-weight: bold;'>" + item.NAME + "</td>");
                        sb.Append("<td style='text-align: center;'>" + item.MOBILE_NO + "</td>");
                        sb.Append("<td style='text-align: center;'>" + item.YEAR_OF_PASSING + "</td>");
                        sb.Append("<td style='text-align: center; color: " + statusColor + "; font-weight: bold;'>" + statusLabel + "</td>");
                        sb.Append("<td style='text-align: center;'>" + (item.CREATED_DATE.HasValue ? item.CREATED_DATE.Value.ToString("dd-MMM-yyyy hh:mm tt") : "-") + "</td>");
                        if (includeSignature)
                        {
                            sb.Append("<td style='height: 35px; width: 140px;'></td>");
                        }
                        sb.Append("</tr>");
                    }

                    sb.Append("<tr style='background-color: #f8fafc; font-weight: bold;'>");
                    sb.Append("<td colspan='6' style='text-align: right;'>TOTAL REGISTRATIONS:</td>");
                    sb.Append("<td colspan='" + (includeSignature ? "3" : "2") + "' style='text-align: left; color: #1e3a8a;'>" + dataList.Count + "</td>");
                    sb.Append("</tr>");
                    sb.Append("</table>");

                    string fileName = "AlumniElection_ProgrammeWiseAlumniList_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xls";
                    return File(Encoding.UTF8.GetBytes(sb.ToString()), "application/vnd.ms-excel", fileName);
                }
                else
                {
                    // Detailed Registrations Excel in Alphabetical Order by Name
                    var where = new StringBuilder("WHERE r.IS_DELETED = 0 ");
                    var p = new Dictionary<string, object>();

                    if (!string.IsNullOrWhiteSpace(academicYear) && academicYear != "ALL")
                    {
                        where.Append("AND r.ACADEMIC_YEAR = @AC_YEAR ");
                        p.Add("AC_YEAR", academicYear.Trim());
                    }
                    if (progIds.Count > 0)
                    {
                        where.Append("AND r.PROGRAMME IN (" + string.Join(",", progIds) + ") ");
                    }
                    if (statuses.Count == 1)
                    {
                        where.Append("AND r.ELECTION_STATUS = @STATUS ");
                        p.Add("STATUS", statuses[0]);
                    }
                    else if (statuses.Count > 1)
                    {
                        where.Append("AND r.ELECTION_STATUS IN ('" + string.Join("','", statuses) + "') ");
                    }

                    var listSql = @"SELECT r.ID, r.ACADEMIC_YEAR, r.NAME, r.MOBILE_NO, r.PROGRAMME, 
                                           COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified') AS PROGRAMME_NAME, 
                                           COALESCE(p.PROGRAMME_CODE, '') AS PROGRAMME_CODE,
                                           r.YEAR_OF_PASSING, r.ELECTION_STATUS, r.CREATED_DATE 
                                    FROM alumni_election_registration r 
                                    LEFT JOIN cp_programme p ON r.PROGRAMME = p.PROGRAMME_ID 
                                    " + where.ToString() + @"
                                    ORDER BY r.NAME ASC;";

                    var dataList = DapperHandler.FetchAll<AlumniElectionRegistration>(listSql, p) ?? new List<AlumniElectionRegistration>();

                    string colSpanTitle = includeSignature ? "9" : "8";
                    sb.Append("<table border='1'>");
                    sb.Append("<tr><th colspan='" + colSpanTitle + "' style='background-color: #1e3a8a; color: #ffffff; font-size: 15pt; font-weight: bold; text-align: center; padding: 10px;'>ST. JOSEPH'S COLLEGE - ALUMNI ELECTION ATTENDANCE REPORT (ALPHABETICAL)</th></tr>");
                    sb.Append("<tr><th colspan='" + colSpanTitle + "' style='background-color: #f1f5f9; font-size: 10pt; text-align: center;'>Academic Year: " + (academicYear ?? "All") + " | Generated On: " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") + "</th></tr>");
                    sb.Append("<tr style='background-color: #e2e8f0; font-weight: bold;'>");
                    sb.Append("<th style='width: 50px; text-align: center;'>#</th>");
                    sb.Append("<th style='text-align: center;'>Registration No</th>");
                    sb.Append("<th style='text-align: left;'>Alumnus Name</th>");
                    sb.Append("<th style='text-align: center;'>Mobile Number</th>");
                    sb.Append("<th style='text-align: left;'>Programme</th>");
                    sb.Append("<th style='text-align: center;'>Year of Passing</th>");
                    sb.Append("<th style='text-align: center;'>Attendance Status</th>");
                    sb.Append("<th style='text-align: center;'>Registration Date</th>");
                    if (includeSignature)
                    {
                        sb.Append("<th style='width: 140px; text-align: center;'>Signature</th>");
                    }
                    sb.Append("</tr>");

                    int sno = 1;
                    foreach (var item in dataList)
                    {
                        string regNo = "SJC-ELEC-" + (!string.IsNullOrWhiteSpace(item.ACADEMIC_YEAR) ? item.ACADEMIC_YEAR.Trim() : "2026") + "-" + item.ID.ToString("D4");
                        string statusColor = (item.ELECTION_STATUS == "APPROVED") ? "#059669" : "#dc2626";
                        string statusLabel = (item.ELECTION_STATUS == "APPROVED") ? "ATTENDING (APPROVED)" : "CANNOT ATTEND (REJECTED)";

                        sb.Append("<tr>");
                        sb.Append("<td style='text-align: center;'>" + (sno++) + "</td>");
                        sb.Append("<td style='text-align: center; font-weight: bold; color: #1e3a8a;'>" + regNo + "</td>");
                        sb.Append("<td style='font-weight: bold;'>" + item.NAME + "</td>");
                        sb.Append("<td style='text-align: center;'>" + item.MOBILE_NO + "</td>");
                        sb.Append("<td>" + item.PROGRAMME_NAME + "</td>");
                        sb.Append("<td style='text-align: center;'>" + item.YEAR_OF_PASSING + "</td>");
                        sb.Append("<td style='text-align: center; color: " + statusColor + "; font-weight: bold;'>" + statusLabel + "</td>");
                        sb.Append("<td style='text-align: center;'>" + (item.CREATED_DATE.HasValue ? item.CREATED_DATE.Value.ToString("dd-MMM-yyyy hh:mm tt") : "-") + "</td>");
                        if (includeSignature)
                        {
                            sb.Append("<td style='height: 35px; width: 140px;'></td>");
                        }
                        sb.Append("</tr>");
                    }

                    sb.Append("<tr style='background-color: #f8fafc; font-weight: bold;'>");
                    sb.Append("<td colspan='6' style='text-align: right;'>TOTAL REGISTRATIONS:</td>");
                    sb.Append("<td colspan='" + (includeSignature ? "3" : "2") + "' style='text-align: left; color: #1e3a8a;'>" + dataList.Count + "</td>");
                    sb.Append("</tr>");
                    sb.Append("</table>");

                    string fileName = "AlumniElection_AlphabeticalReport_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xls";
                    return File(Encoding.UTF8.GetBytes(sb.ToString()), "application/vnd.ms-excel", fileName);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objLog = new ErrorLog())
                {
                    objLog.WriteError("AlumniElectionController", "ExportReportsToExcel", ex.Message);
                }
                return Content("Error generating Excel report: " + ex.Message);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private ActionResult ExportProgrammeSheetsWithEPPlus(List<IGrouping<string, AlumniElectionRegistration>> grouped, string academicYear, bool includeSignature)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var sumWs = package.Workbook.Worksheets.Add("Programme Summary");
                PopulateSummaryWorksheet(sumWs, grouped, academicYear);

                foreach (var group in grouped)
                {
                    string sheetName = GetUniqueWorksheetName(package, group.Key);
                    var progWs = package.Workbook.Worksheets.Add(sheetName);
                    PopulateProgrammeWorksheet(progWs, group.Key, group.ToList(), academicYear, includeSignature);
                }

                using (var stream = new MemoryStream())
                {
                    package.SaveAs(stream);
                    string fileName = "AlumniElection_ProgrammeWise_SeparateSheets_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private ActionResult ExportProgrammeFilesWithEPPlus(List<IGrouping<string, AlumniElectionRegistration>> grouped, string academicYear, bool includeSignature)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            if (grouped.Count == 1)
            {
                var singleGroup = grouped[0];
                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add(GetUniqueWorksheetName(package, singleGroup.Key));
                    PopulateProgrammeWorksheet(ws, singleGroup.Key, singleGroup.ToList(), academicYear, includeSignature);
                    using (var stream = new MemoryStream())
                    {
                        package.SaveAs(stream);
                        string safeName = GetSafeFileName(singleGroup.Key);
                        string fileName = "AlumniElection_" + safeName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            else
            {
                using (var zipStream = new MemoryStream())
                {
                    using (var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create, true))
                    {
                        using (var sumPackage = new OfficeOpenXml.ExcelPackage())
                        {
                            var sumWs = sumPackage.Workbook.Worksheets.Add("Programme Summary");
                            PopulateSummaryWorksheet(sumWs, grouped, academicYear);
                            using (var sumStream = new MemoryStream())
                            {
                                sumPackage.SaveAs(sumStream);
                                var sumEntry = archive.CreateEntry("00_All_Programmes_Summary.xlsx");
                                using (var es = sumEntry.Open())
                                {
                                    var b = sumStream.ToArray();
                                    es.Write(b, 0, b.Length);
                                }
                            }
                        }

                        foreach (var group in grouped)
                        {
                            using (var progPackage = new OfficeOpenXml.ExcelPackage())
                            {
                                var ws = progPackage.Workbook.Worksheets.Add(GetUniqueWorksheetName(progPackage, group.Key));
                                PopulateProgrammeWorksheet(ws, group.Key, group.ToList(), academicYear, includeSignature);

                                using (var progStream = new MemoryStream())
                                {
                                    progPackage.SaveAs(progStream);
                                    string safeProgName = GetSafeFileName(group.Key);
                                    string entryName = safeProgName + ".xlsx";
                                    int dupCount = 1;
                                    while (archive.GetEntry(entryName) != null)
                                    {
                                        entryName = safeProgName + "_" + dupCount + ".xlsx";
                                        dupCount++;
                                    }

                                    var entry = archive.CreateEntry(entryName);
                                    using (var es = entry.Open())
                                    {
                                        var b = progStream.ToArray();
                                        es.Write(b, 0, b.Length);
                                    }
                                }
                            }
                        }
                    }

                    string zipName = "AlumniElection_ProgrammeWise_ExcelFiles_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".zip";
                    return File(zipStream.ToArray(), "application/zip", zipName);
                }
            }
        }

        private byte[] GenerateSpreadsheetMLProgrammeSheets(List<IGrouping<string, AlumniElectionRegistration>> grouped, string academicYear, bool includeSignature)
        {
            var sb = new StringBuilder(64000);
            AppendSpreadsheetMLHeader(sb);

            int maxCols = includeSignature ? 7 : 6;
            string genDate = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt");
            string acDisplay = !string.IsNullOrWhiteSpace(academicYear) && academicYear != "ALL" ? academicYear.Trim() : "All";

            // Sheet 1: Programme Summary
            AppendSpreadsheetMLSummaryWorksheet(sb, grouped, acDisplay, genDate);

            // Subsequent Sheets: One per Programme
            var usedSheetNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Programme Summary" };
            foreach (var group in grouped)
            {
                string safeSheetName = GetSafeSpreadsheetMLSheetName(group.Key, usedSheetNames);
                usedSheetNames.Add(safeSheetName);
                AppendSpreadsheetMLProgrammeWorksheet(sb, group.Key, safeSheetName, group.ToList(), acDisplay, genDate, includeSignature, maxCols);
            }

            sb.AppendLine("</Workbook>");
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private ActionResult ExportProgrammeFilesFallback(List<IGrouping<string, AlumniElectionRegistration>> grouped, string academicYear, bool includeSignature)
        {
            string genDate = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt");
            string acDisplay = !string.IsNullOrWhiteSpace(academicYear) && academicYear != "ALL" ? academicYear.Trim() : "All";
            int maxCols = includeSignature ? 7 : 6;

            if (grouped.Count == 1)
            {
                var singleGroup = grouped[0];
                var sb = new StringBuilder(16000);
                AppendSpreadsheetMLHeader(sb);
                AppendSpreadsheetMLProgrammeWorksheet(sb, singleGroup.Key, "Alumni List", singleGroup.ToList(), acDisplay, genDate, includeSignature, maxCols);
                sb.AppendLine("</Workbook>");

                string safeName = GetSafeFileName(singleGroup.Key);
                string fileName = "AlumniElection_" + safeName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xls";
                return File(Encoding.UTF8.GetBytes(sb.ToString()), "application/vnd.ms-excel", fileName);
            }
            else
            {
                using (var zipStream = new MemoryStream())
                {
                    using (var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create, true))
                    {
                        var sumSb = new StringBuilder(16000);
                        AppendSpreadsheetMLHeader(sumSb);
                        AppendSpreadsheetMLSummaryWorksheet(sumSb, grouped, acDisplay, genDate);
                        sumSb.AppendLine("</Workbook>");
                        byte[] sumBytes = Encoding.UTF8.GetBytes(sumSb.ToString());

                        var sumEntry = archive.CreateEntry("00_All_Programmes_Summary.xls");
                        using (var es = sumEntry.Open())
                        {
                            es.Write(sumBytes, 0, sumBytes.Length);
                        }

                        foreach (var group in grouped)
                        {
                            var progSb = new StringBuilder(16000);
                            AppendSpreadsheetMLHeader(progSb);
                            AppendSpreadsheetMLProgrammeWorksheet(progSb, group.Key, "Alumni List", group.ToList(), acDisplay, genDate, includeSignature, maxCols);
                            progSb.AppendLine("</Workbook>");
                            byte[] progBytes = Encoding.UTF8.GetBytes(progSb.ToString());

                            string safeProgName = GetSafeFileName(group.Key);
                            string entryName = safeProgName + ".xls";
                            int dupCount = 1;
                            while (archive.GetEntry(entryName) != null)
                            {
                                entryName = safeProgName + "_" + dupCount + ".xls";
                                dupCount++;
                            }
                            var entry = archive.CreateEntry(entryName);
                            using (var es = entry.Open())
                            {
                                es.Write(progBytes, 0, progBytes.Length);
                            }
                        }
                    }

                    string zipName = "AlumniElection_ProgrammeWise_ExcelFiles_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".zip";
                    return File(zipStream.ToArray(), "application/zip", zipName);
                }
            }
        }

        private static void AppendSpreadsheetMLHeader(StringBuilder sb)
        {
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<?mso-application progid=\"Excel.Sheet\"?>");
            sb.AppendLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"");
            sb.AppendLine(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
            sb.AppendLine(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
            sb.AppendLine(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"");
            sb.AppendLine(" xmlns:html=\"http://www.w3.org/TR/REC-html40\">");
            sb.AppendLine(" <Styles>");
            sb.AppendLine("  <Style ss:ID=\"Default\" ss:Name=\"Normal\"><Alignment ss:Vertical=\"Center\"/><Font ss:FontName=\"Calibri\" ss:Size=\"10\"/></Style>");
            sb.AppendLine("  <Style ss:ID=\"MainTitle\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Calibri\" ss:Size=\"13\" ss:Bold=\"1\" ss:Color=\"#FFFFFF\"/><Interior ss:Color=\"#1E3A8A\" ss:Pattern=\"Solid\"/></Style>");
            sb.AppendLine("  <Style ss:ID=\"SubTitle\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Calibri\" ss:Size=\"11\" ss:Bold=\"1\" ss:Color=\"#1E3A8A\"/></Style>");
            sb.AppendLine("  <Style ss:ID=\"MetaInfo\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Calibri\" ss:Size=\"9.5\" ss:Italic=\"1\" ss:Color=\"#475569\"/></Style>");
            sb.AppendLine("  <Style ss:ID=\"ColHeader\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Calibri\" ss:Size=\"10\" ss:Bold=\"1\" ss:Color=\"#FFFFFF\"/><Interior ss:Color=\"#2563EB\" ss:Pattern=\"Solid\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#CBD5E1\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#CBD5E1\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#CBD5E1\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#CBD5E1\"/></Borders></Style>");
            sb.AppendLine("  <Style ss:ID=\"CellCenter\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Calibri\" ss:Size=\"10\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/></Borders></Style>");
            sb.AppendLine("  <Style ss:ID=\"CellSerial\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Calibri\" ss:Size=\"10\" ss:Bold=\"1\" ss:Color=\"#1E3A8A\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/></Borders></Style>");
            sb.AppendLine("  <Style ss:ID=\"CellReg\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Calibri\" ss:Size=\"10\" ss:Bold=\"1\" ss:Color=\"#0284C7\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/></Borders></Style>");
            sb.AppendLine("  <Style ss:ID=\"CellLeft\"><Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Calibri\" ss:Size=\"10\" ss:Bold=\"1\" ss:Color=\"#0F172A\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/></Borders></Style>");
            sb.AppendLine("  <Style ss:ID=\"CellRight\"><Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Calibri\" ss:Size=\"10\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/></Borders></Style>");
            sb.AppendLine("  <Style ss:ID=\"CellApproved\"><Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Calibri\" ss:Size=\"10\" ss:Bold=\"1\" ss:Color=\"#15803D\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/></Borders></Style>");
            sb.AppendLine("  <Style ss:ID=\"CellRejected\"><Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Calibri\" ss:Size=\"10\" ss:Bold=\"1\" ss:Color=\"#B91C1C\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/></Borders></Style>");
            sb.AppendLine("  <Style ss:ID=\"CellSig\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Bottom\"/><Font ss:FontName=\"Calibri\" ss:Size=\"10\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Dot\" ss:Weight=\"1\" ss:Color=\"#94A3B8\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/></Borders></Style>");
            sb.AppendLine("  <Style ss:ID=\"SummaryLabel\"><Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Calibri\" ss:Size=\"10\" ss:Bold=\"1\"/><Interior ss:Color=\"#F1F5F9\" ss:Pattern=\"Solid\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\" ss:Color=\"#1E3A8A\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\" ss:Color=\"#1E3A8A\"/></Borders></Style>");
            sb.AppendLine("  <Style ss:ID=\"SummaryVal\"><Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Calibri\" ss:Size=\"10\" ss:Bold=\"1\" ss:Color=\"#1E3A8A\"/><Interior ss:Color=\"#F1F5F9\" ss:Pattern=\"Solid\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\" ss:Color=\"#1E3A8A\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\" ss:Color=\"#1E3A8A\"/></Borders></Style>");
            sb.AppendLine(" </Styles>");
        }

        private static void AppendSpreadsheetMLSummaryWorksheet(StringBuilder sb, List<IGrouping<string, AlumniElectionRegistration>> grouped, string acDisplay, string genDate)
        {
            sb.AppendLine(" <Worksheet ss:Name=\"Programme Summary\">");
            sb.AppendLine("  <Table>");
            sb.AppendLine("   <Column ss:Width=\"35\"/>");
            sb.AppendLine("   <Column ss:Width=\"240\"/>");
            sb.AppendLine("   <Column ss:Width=\"90\"/>");
            sb.AppendLine("   <Column ss:Width=\"110\"/>");
            sb.AppendLine("   <Column ss:Width=\"120\"/>");
            sb.AppendLine("   <Column ss:Width=\"90\"/>");

            sb.AppendLine("   <Row ss:Height=\"26\"><Cell ss:StyleID=\"MainTitle\" ss:MergeAcross=\"5\"><Data ss:Type=\"String\">ST. JOSEPH'S COLLEGE OF ARTS AND SCIENCE (AUTONOMOUS), CUDDALORE</Data></Cell></Row>");
            sb.AppendLine("   <Row ss:Height=\"22\"><Cell ss:StyleID=\"SubTitle\" ss:MergeAcross=\"5\"><Data ss:Type=\"String\">ALUMNI ELECTION ATTENDANCE - PROGRAMME-WISE SUMMARY</Data></Cell></Row>");
            sb.AppendLine("   <Row ss:Height=\"20\"><Cell ss:StyleID=\"MetaInfo\" ss:MergeAcross=\"5\"><Data ss:Type=\"String\">Academic Year: " + EscapeXml(acDisplay) + " | Generated: " + EscapeXml(genDate) + "</Data></Cell></Row>");
            sb.AppendLine("   <Row ss:Height=\"10\"/>");

            sb.AppendLine("   <Row ss:Height=\"24\">");
            sb.AppendLine("    <Cell ss:StyleID=\"ColHeader\"><Data ss:Type=\"String\">#</Data></Cell>");
            sb.AppendLine("    <Cell ss:StyleID=\"ColHeader\"><Data ss:Type=\"String\">Programme Name</Data></Cell>");
            sb.AppendLine("    <Cell ss:StyleID=\"ColHeader\"><Data ss:Type=\"String\">Total Registered</Data></Cell>");
            sb.AppendLine("    <Cell ss:StyleID=\"ColHeader\"><Data ss:Type=\"String\">Attending (Approved)</Data></Cell>");
            sb.AppendLine("    <Cell ss:StyleID=\"ColHeader\"><Data ss:Type=\"String\">Cannot Attend (Rejected)</Data></Cell>");
            sb.AppendLine("    <Cell ss:StyleID=\"ColHeader\"><Data ss:Type=\"String\">Attendance %</Data></Cell>");
            sb.AppendLine("   </Row>");

            int sumSno = 1;
            int grandTotal = 0, grandApproved = 0, grandRejected = 0;
            foreach (var g in grouped)
            {
                int tot = g.Count();
                int app = g.Count(x => x.ELECTION_STATUS == "APPROVED");
                int rej = g.Count(x => x.ELECTION_STATUS == "REJECTED");
                double pct = (tot > 0) ? Math.Round(((double)app / tot) * 100.0, 1) : 0.0;
                grandTotal += tot;
                grandApproved += app;
                grandRejected += rej;

                sb.AppendLine("   <Row ss:Height=\"20\">");
                sb.AppendLine("    <Cell ss:StyleID=\"CellCenter\"><Data ss:Type=\"Number\">" + (sumSno++) + "</Data></Cell>");
                sb.AppendLine("    <Cell ss:StyleID=\"CellLeft\"><Data ss:Type=\"String\">" + EscapeXml(g.Key) + "</Data></Cell>");
                sb.AppendLine("    <Cell ss:StyleID=\"CellRight\"><Data ss:Type=\"Number\">" + tot + "</Data></Cell>");
                sb.AppendLine("    <Cell ss:StyleID=\"CellApproved\"><Data ss:Type=\"Number\">" + app + "</Data></Cell>");
                sb.AppendLine("    <Cell ss:StyleID=\"CellRejected\"><Data ss:Type=\"Number\">" + rej + "</Data></Cell>");
                sb.AppendLine("    <Cell ss:StyleID=\"CellRight\"><Data ss:Type=\"String\">" + pct + "%</Data></Cell>");
                sb.AppendLine("   </Row>");
            }

            double grandPct = (grandTotal > 0) ? Math.Round(((double)grandApproved / grandTotal) * 100.0, 1) : 0.0;
            sb.AppendLine("   <Row ss:Height=\"22\">");
            sb.AppendLine("    <Cell ss:StyleID=\"SummaryLabel\" ss:MergeAcross=\"1\"><Data ss:Type=\"String\">TOTAL:</Data></Cell>");
            sb.AppendLine("    <Cell ss:StyleID=\"SummaryVal\"><Data ss:Type=\"Number\">" + grandTotal + "</Data></Cell>");
            sb.AppendLine("    <Cell ss:StyleID=\"SummaryVal\"><Data ss:Type=\"Number\">" + grandApproved + "</Data></Cell>");
            sb.AppendLine("    <Cell ss:StyleID=\"SummaryVal\"><Data ss:Type=\"Number\">" + grandRejected + "</Data></Cell>");
            sb.AppendLine("    <Cell ss:StyleID=\"SummaryVal\"><Data ss:Type=\"String\">" + grandPct + "%</Data></Cell>");
            sb.AppendLine("   </Row>");
            sb.AppendLine("  </Table>");
            sb.AppendLine(" </Worksheet>");
        }

        private void AppendSpreadsheetMLProgrammeWorksheet(StringBuilder sb, string progName, string sheetName, List<AlumniElectionRegistration> alumniList, string acDisplay, string genDate, bool includeSignature, int maxCols)
        {
            int totCount = alumniList.Count;
            int approvedCount = alumniList.Count(x => x.ELECTION_STATUS == "APPROVED");
            int rejectedCount = alumniList.Count(x => x.ELECTION_STATUS == "REJECTED");

            sb.AppendLine(" <Worksheet ss:Name=\"" + EscapeXml(sheetName) + "\">");
            sb.AppendLine("  <Table>");
            sb.AppendLine("   <Column ss:Width=\"35\"/>");
            sb.AppendLine("   <Column ss:Width=\"140\"/>");
            sb.AppendLine("   <Column ss:Width=\"150\"/>");
            sb.AppendLine("   <Column ss:Width=\"200\"/>");
            sb.AppendLine("   <Column ss:Width=\"95\"/>");
            sb.AppendLine("   <Column ss:Width=\"75\"/>");
            if (includeSignature)
            {
                sb.AppendLine("   <Column ss:Width=\"130\"/>");
            }

            sb.AppendLine("   <Row ss:Height=\"26\"><Cell ss:StyleID=\"MainTitle\" ss:MergeAcross=\"" + (maxCols - 1) + "\"><Data ss:Type=\"String\">ST. JOSEPH'S COLLEGE OF ARTS AND SCIENCE (AUTONOMOUS), CUDDALORE</Data></Cell></Row>");
            sb.AppendLine("   <Row ss:Height=\"22\"><Cell ss:StyleID=\"SubTitle\" ss:MergeAcross=\"" + (maxCols - 1) + "\"><Data ss:Type=\"String\">ALUMNI ELECTION ATTENDANCE - " + EscapeXml(progName.ToUpper()) + "</Data></Cell></Row>");
            sb.AppendLine("   <Row ss:Height=\"20\"><Cell ss:StyleID=\"MetaInfo\" ss:MergeAcross=\"" + (maxCols - 1) + "\"><Data ss:Type=\"String\">Academic Year: " + EscapeXml(acDisplay) + " | Total: " + totCount + " | Attending: " + approvedCount + " | Cannot Attend: " + rejectedCount + " | Generated: " + EscapeXml(genDate) + "</Data></Cell></Row>");
            sb.AppendLine("   <Row ss:Height=\"10\"/>");

            sb.AppendLine("   <Row ss:Height=\"24\">");
            sb.AppendLine("    <Cell ss:StyleID=\"ColHeader\"><Data ss:Type=\"String\">#</Data></Cell>");
            sb.AppendLine("    <Cell ss:StyleID=\"ColHeader\"><Data ss:Type=\"String\">Serial No.</Data></Cell>");
            sb.AppendLine("    <Cell ss:StyleID=\"ColHeader\"><Data ss:Type=\"String\">Registered No.</Data></Cell>");
            sb.AppendLine("    <Cell ss:StyleID=\"ColHeader\"><Data ss:Type=\"String\">Alumnus Name (A to Z)</Data></Cell>");
            sb.AppendLine("    <Cell ss:StyleID=\"ColHeader\"><Data ss:Type=\"String\">Mobile Number</Data></Cell>");
            sb.AppendLine("    <Cell ss:StyleID=\"ColHeader\"><Data ss:Type=\"String\">Passing Year</Data></Cell>");
            if (includeSignature)
            {
                sb.AppendLine("    <Cell ss:StyleID=\"ColHeader\"><Data ss:Type=\"String\">Signature</Data></Cell>");
            }
            sb.AppendLine("   </Row>");

            var sortedAlumni = alumniList.OrderBy(x => (x.NAME ?? "").Trim(), StringComparer.CurrentCultureIgnoreCase).ToList();
            int rowSno = 1;
            foreach (var alumni in sortedAlumni)
            {
                string year = !string.IsNullOrWhiteSpace(alumni.ACADEMIC_YEAR) ? alumni.ACADEMIC_YEAR.Trim() : (acDisplay != "All" ? acDisplay : "2026");
                string serialNo = "SJC-ELEC-" + year + "-" + alumni.ID.ToString("D4");
                string registeredNo = "SJC-" + (!string.IsNullOrWhiteSpace(alumni.PROGRAMME_CODE) ? alumni.PROGRAMME_CODE.Trim() : "ELEC") + "-" + year + "-" + GetAlumniSerialNo(alumni.MOBILE_NO, alumni.ID);

                sb.AppendLine("   <Row ss:Height=\"" + (includeSignature ? "28" : "20") + "\">");
                sb.AppendLine("    <Cell ss:StyleID=\"CellCenter\"><Data ss:Type=\"Number\">" + (rowSno++) + "</Data></Cell>");
                sb.AppendLine("    <Cell ss:StyleID=\"CellSerial\"><Data ss:Type=\"String\">" + EscapeXml(serialNo) + "</Data></Cell>");
                sb.AppendLine("    <Cell ss:StyleID=\"CellReg\"><Data ss:Type=\"String\">" + EscapeXml(registeredNo) + "</Data></Cell>");
                sb.AppendLine("    <Cell ss:StyleID=\"CellLeft\"><Data ss:Type=\"String\">" + EscapeXml(alumni.NAME) + "</Data></Cell>");
                sb.AppendLine("    <Cell ss:StyleID=\"CellCenter\"><Data ss:Type=\"String\">" + EscapeXml(alumni.MOBILE_NO) + "</Data></Cell>");
                sb.AppendLine("    <Cell ss:StyleID=\"CellCenter\"><Data ss:Type=\"String\">" + EscapeXml(alumni.YEAR_OF_PASSING) + "</Data></Cell>");
                if (includeSignature)
                {
                    sb.AppendLine("    <Cell ss:StyleID=\"CellSig\"><Data ss:Type=\"String\"></Data></Cell>");
                }
                sb.AppendLine("   </Row>");
            }

            sb.AppendLine("   <Row ss:Height=\"22\">");
            sb.AppendLine("    <Cell ss:StyleID=\"SummaryLabel\" ss:MergeAcross=\"2\"><Data ss:Type=\"String\">Total for " + EscapeXml(progName) + ":</Data></Cell>");
            sb.AppendLine("    <Cell ss:StyleID=\"SummaryVal\" ss:MergeAcross=\"" + (maxCols - 4) + "\"><Data ss:Type=\"String\">" + totCount + " Registered (" + approvedCount + " Attending, " + rejectedCount + " Cannot Attend)</Data></Cell>");
            sb.AppendLine("   </Row>");

            sb.AppendLine("  </Table>");
            sb.AppendLine(" </Worksheet>");
        }

        private static string EscapeXml(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }

        private static string GetSafeSpreadsheetMLSheetName(string progName, HashSet<string> usedNames)
        {
            string baseName = string.IsNullOrWhiteSpace(progName) ? "Unspecified" : progName.Trim();
            baseName = Regex.Replace(baseName, @"[\\/*?:\[\]]", "");
            baseName = baseName.Trim();
            if (baseName.Length == 0) baseName = "Programme";
            if (baseName.Length > 28) baseName = baseName.Substring(0, 28).Trim();

            string uniqueName = baseName;
            int counter = 1;
            while (usedNames.Contains(uniqueName))
            {
                string suffix = "_" + counter;
                int maxBaseLen = 31 - suffix.Length;
                string trimmedBase = (baseName.Length > maxBaseLen) ? baseName.Substring(0, maxBaseLen) : baseName;
                uniqueName = trimmedBase + suffix;
                counter++;
            }
            return uniqueName;
        }

        private string GetUniqueWorksheetName(OfficeOpenXml.ExcelPackage package, string progName)
        {
            string baseName = string.IsNullOrWhiteSpace(progName) ? "Unspecified" : progName.Trim();
            baseName = Regex.Replace(baseName, @"[\\/*?:\[\]]", "");
            baseName = baseName.Trim();
            if (baseName.Length == 0) baseName = "Programme";
            if (baseName.Length > 28) baseName = baseName.Substring(0, 28).Trim();

            string uniqueName = baseName;
            int counter = 1;
            while (package.Workbook.Worksheets[uniqueName] != null)
            {
                string suffix = "_" + counter;
                int maxBaseLen = 31 - suffix.Length;
                string trimmedBase = (baseName.Length > maxBaseLen) ? baseName.Substring(0, maxBaseLen) : baseName;
                uniqueName = trimmedBase + suffix;
                counter++;
            }
            return uniqueName;
        }

        private string GetSafeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Unspecified";
            string safe = Regex.Replace(name.Trim(), @"[\\/:*?""<>|]", "_");
            safe = safe.Trim();
            if (safe.Length > 80) safe = safe.Substring(0, 80).Trim();
            return string.IsNullOrWhiteSpace(safe) ? "Programme" : safe;
        }

        private void PopulateProgrammeWorksheet(OfficeOpenXml.ExcelWorksheet ws, string progName, List<AlumniElectionRegistration> alumniList, string acYear, bool includeSignature)
        {
            int maxCols = includeSignature ? 7 : 6;
            int totCount = alumniList.Count;
            int approvedCount = alumniList.Count(x => x.ELECTION_STATUS == "APPROVED");
            int rejectedCount = alumniList.Count(x => x.ELECTION_STATUS == "REJECTED");

            // College Header Row 1
            ws.Cells[1, 1].Value = "ST. JOSEPH'S COLLEGE OF ARTS AND SCIENCE (AUTONOMOUS), CUDDALORE";
            ws.Cells[1, 1, 1, maxCols].Merge = true;
            ws.Cells[1, 1].Style.Font.Size = 13;
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.Font.Color.SetColor(Color.White);
            ws.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(30, 58, 138));
            ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(1).Height = 26;

            // Subtitle Row 2
            ws.Cells[2, 1].Value = "ALUMNI ELECTION ATTENDANCE - " + progName.ToUpper();
            ws.Cells[2, 1, 2, maxCols].Merge = true;
            ws.Cells[2, 1].Style.Font.Size = 11;
            ws.Cells[2, 1].Style.Font.Bold = true;
            ws.Cells[2, 1].Style.Font.Color.SetColor(Color.FromArgb(30, 58, 138));
            ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(2).Height = 22;

            // Info Row 3
            ws.Cells[3, 1].Value = "Academic Year: " + (acYear ?? "All") + " | Total: " + totCount + " | Attending: " + approvedCount + " | Cannot Attend: " + rejectedCount + " | Generated: " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt");
            ws.Cells[3, 1, 3, maxCols].Merge = true;
            ws.Cells[3, 1].Style.Font.Size = 9.5f;
            ws.Cells[3, 1].Style.Font.Italic = true;
            ws.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[3, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(3).Height = 20;

            // Table Header Row 5
            string[] headers = includeSignature 
                ? new string[] { "#", "Serial No.", "Registered No.", "Alumnus Name (A to Z)", "Mobile Number", "Passing Year", "Signature" }
                : new string[] { "#", "Serial No.", "Registered No.", "Alumnus Name (A to Z)", "Mobile Number", "Passing Year" };

            for (int col = 0; col < headers.Length; col++)
            {
                var cell = ws.Cells[5, col + 1];
                cell.Value = headers[col];
                cell.Style.Font.Bold = true;
                cell.Style.Font.Color.SetColor(Color.White);
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 99, 235));
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(203, 213, 225));
                cell.Style.HorizontalAlignment = (col == 0 || col == 1 || col == 2 || col == 4 || col == 5 || (includeSignature && col == 6))
                    ? ExcelHorizontalAlignment.Center
                    : ExcelHorizontalAlignment.Left;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            ws.Row(5).Height = 24;

            // Sort strictly alphabetical by name
            var sortedAlumni = alumniList.OrderBy(x => (x.NAME ?? "").Trim(), StringComparer.CurrentCultureIgnoreCase).ToList();
            int rIdx = 6;
            int sno = 1;
            foreach (var alumni in sortedAlumni)
            {
                string year = !string.IsNullOrWhiteSpace(alumni.ACADEMIC_YEAR) ? alumni.ACADEMIC_YEAR.Trim() : (!string.IsNullOrWhiteSpace(acYear) && acYear != "ALL" ? acYear.Trim() : "2026");
                string serialNo = "SJC-ELEC-" + year + "-" + alumni.ID.ToString("D4");
                string registeredNo = "SJC-" + (!string.IsNullOrWhiteSpace(alumni.PROGRAMME_CODE) ? alumni.PROGRAMME_CODE.Trim() : "ELEC") + "-" + year + "-" + GetAlumniSerialNo(alumni.MOBILE_NO, alumni.ID);

                ws.Cells[rIdx, 1].Value = sno++;
                ws.Cells[rIdx, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells[rIdx, 2].Value = serialNo;
                ws.Cells[rIdx, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rIdx, 2].Style.Font.Bold = true;

                ws.Cells[rIdx, 3].Value = registeredNo;
                ws.Cells[rIdx, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rIdx, 3].Style.Font.Bold = true;

                ws.Cells[rIdx, 4].Value = alumni.NAME;
                ws.Cells[rIdx, 4].Style.Font.Bold = true;

                ws.Cells[rIdx, 5].Value = alumni.MOBILE_NO;
                ws.Cells[rIdx, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells[rIdx, 6].Value = alumni.YEAR_OF_PASSING;
                ws.Cells[rIdx, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                if (includeSignature)
                {
                    ws.Cells[rIdx, 7].Value = "";
                    ws.Cells[rIdx, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Dotted;
                    ws.Row(rIdx).Height = 28;
                }
                else
                {
                    ws.Row(rIdx).Height = 20;
                }

                for (int col = 1; col <= maxCols; col++)
                {
                    ws.Cells[rIdx, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(226, 232, 240));
                    ws.Cells[rIdx, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    if (rIdx % 2 == 1)
                    {
                        ws.Cells[rIdx, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[rIdx, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(248, 250, 252));
                    }
                }

                rIdx++;
            }

            // Summary Row
            ws.Cells[rIdx, 1].Value = "Total for " + progName + ":";
            ws.Cells[rIdx, 1, rIdx, 3].Merge = true;
            ws.Cells[rIdx, 1].Style.Font.Bold = true;
            ws.Cells[rIdx, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[rIdx, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            ws.Cells[rIdx, 4].Value = totCount + " Registered (" + approvedCount + " Attending, " + rejectedCount + " Cannot Attend)";
            ws.Cells[rIdx, 4, rIdx, maxCols].Merge = true;
            ws.Cells[rIdx, 4].Style.Font.Bold = true;
            ws.Cells[rIdx, 4].Style.Font.Color.SetColor(Color.FromArgb(30, 58, 138));
            ws.Cells[rIdx, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            for (int col = 1; col <= maxCols; col++)
            {
                ws.Cells[rIdx, col].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rIdx, col].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                ws.Cells[rIdx, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[rIdx, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(241, 245, 249));
            }
            ws.Row(rIdx).Height = 22;

            ws.Cells[5, 1, rIdx, maxCols].AutoFitColumns();
            if (includeSignature)
            {
                ws.Column(7).Width = 24;
            }
        }

        private void PopulateSummaryWorksheet(OfficeOpenXml.ExcelWorksheet ws, List<IGrouping<string, AlumniElectionRegistration>> grouped, string acYear)
        {
            ws.Cells[1, 1].Value = "ST. JOSEPH'S COLLEGE OF ARTS AND SCIENCE (AUTONOMOUS), CUDDALORE";
            ws.Cells[1, 1, 1, 6].Merge = true;
            ws.Cells[1, 1].Style.Font.Size = 13;
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.Font.Color.SetColor(Color.White);
            ws.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(30, 58, 138));
            ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(1).Height = 26;

            ws.Cells[2, 1].Value = "ALUMNI ELECTION ATTENDANCE - PROGRAMME-WISE SUMMARY";
            ws.Cells[2, 1, 2, 6].Merge = true;
            ws.Cells[2, 1].Style.Font.Size = 11;
            ws.Cells[2, 1].Style.Font.Bold = true;
            ws.Cells[2, 1].Style.Font.Color.SetColor(Color.FromArgb(30, 58, 138));
            ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(2).Height = 22;

            int totalAll = grouped.Sum(g => g.Count());
            int approvedAll = grouped.Sum(g => g.Count(x => x.ELECTION_STATUS == "APPROVED"));
            int rejectedAll = grouped.Sum(g => g.Count(x => x.ELECTION_STATUS == "REJECTED"));

            ws.Cells[3, 1].Value = "Academic Year: " + (acYear ?? "All") + " | Total Programmes: " + grouped.Count + " | Total Registered: " + totalAll + " | Attending: " + approvedAll + " | Generated: " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt");
            ws.Cells[3, 1, 3, 6].Merge = true;
            ws.Cells[3, 1].Style.Font.Size = 9.5f;
            ws.Cells[3, 1].Style.Font.Italic = true;
            ws.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[3, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(3).Height = 20;

            string[] headers = new string[] { "#", "Programme Name", "Total Registered", "Attending (Approved)", "Cannot Attend (Rejected)", "Attendance %" };
            for (int col = 0; col < headers.Length; col++)
            {
                var cell = ws.Cells[5, col + 1];
                cell.Value = headers[col];
                cell.Style.Font.Bold = true;
                cell.Style.Font.Color.SetColor(Color.White);
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 99, 235));
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(203, 213, 225));
                cell.Style.HorizontalAlignment = (col == 0 || col >= 2) ? ExcelHorizontalAlignment.Center : ExcelHorizontalAlignment.Left;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            ws.Row(5).Height = 24;

            int rIdx = 6;
            int sno = 1;
            foreach (var group in grouped)
            {
                int pTot = group.Count();
                int pApp = group.Count(x => x.ELECTION_STATUS == "APPROVED");
                int pRej = group.Count(x => x.ELECTION_STATUS == "REJECTED");
                double pPct = (pTot > 0) ? Math.Round(((double)pApp / pTot) * 100.0, 1) : 0.0;

                ws.Cells[rIdx, 1].Value = sno++;
                ws.Cells[rIdx, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells[rIdx, 2].Value = group.Key;
                ws.Cells[rIdx, 2].Style.Font.Bold = true;

                ws.Cells[rIdx, 3].Value = pTot;
                ws.Cells[rIdx, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells[rIdx, 3].Style.Font.Bold = true;

                ws.Cells[rIdx, 4].Value = pApp;
                ws.Cells[rIdx, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells[rIdx, 4].Style.Font.Bold = true;
                ws.Cells[rIdx, 4].Style.Font.Color.SetColor(Color.FromArgb(21, 128, 61));

                ws.Cells[rIdx, 5].Value = pRej;
                ws.Cells[rIdx, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells[rIdx, 5].Style.Font.Color.SetColor(Color.FromArgb(185, 28, 28));

                ws.Cells[rIdx, 6].Value = pPct + "%";
                ws.Cells[rIdx, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells[rIdx, 6].Style.Font.Bold = true;

                for (int col = 1; col <= 6; col++)
                {
                    ws.Cells[rIdx, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(226, 232, 240));
                    ws.Cells[rIdx, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    if (rIdx % 2 == 1)
                    {
                        ws.Cells[rIdx, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[rIdx, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(248, 250, 252));
                    }
                }
                rIdx++;
            }

            // Grand Total Row
            double overallPct = (totalAll > 0) ? Math.Round(((double)approvedAll / totalAll) * 100.0, 1) : 0.0;
            ws.Cells[rIdx, 1].Value = "Grand Total:";
            ws.Cells[rIdx, 1, rIdx, 2].Merge = true;
            ws.Cells[rIdx, 1].Style.Font.Bold = true;
            ws.Cells[rIdx, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            ws.Cells[rIdx, 3].Value = totalAll;
            ws.Cells[rIdx, 3].Style.Font.Bold = true;
            ws.Cells[rIdx, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[rIdx, 3].Style.Font.Color.SetColor(Color.FromArgb(30, 58, 138));

            ws.Cells[rIdx, 4].Value = approvedAll;
            ws.Cells[rIdx, 4].Style.Font.Bold = true;
            ws.Cells[rIdx, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[rIdx, 4].Style.Font.Color.SetColor(Color.FromArgb(21, 128, 61));

            ws.Cells[rIdx, 5].Value = rejectedAll;
            ws.Cells[rIdx, 5].Style.Font.Bold = true;
            ws.Cells[rIdx, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[rIdx, 5].Style.Font.Color.SetColor(Color.FromArgb(185, 28, 28));

            ws.Cells[rIdx, 6].Value = overallPct + "%";
            ws.Cells[rIdx, 6].Style.Font.Bold = true;
            ws.Cells[rIdx, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[rIdx, 6].Style.Font.Color.SetColor(Color.FromArgb(30, 58, 138));

            for (int col = 1; col <= 6; col++)
            {
                ws.Cells[rIdx, col].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rIdx, col].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                ws.Cells[rIdx, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[rIdx, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(241, 245, 249));
            }
            ws.Row(rIdx).Height = 22;

            ws.Cells[5, 1, rIdx, 6].AutoFitColumns();
        }

        // GET: /AlumniElection/PrintReport
        [HttpGet]
        public ActionResult PrintReport(string academicYear, string programme, string status, string reportType, bool includeSignature = false)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            try
            {
                string acYear = string.IsNullOrWhiteSpace(academicYear) ? "ALL" : academicYear.Trim();
                string repType = string.IsNullOrWhiteSpace(reportType) ? "alphabetical" : reportType.Trim().ToLower();
                var progIds = ParseProgrammeIds(programme);
                var statuses = ParseStatuses(status);

                ViewBag.AcademicYear = acYear;
                ViewBag.SelectedProgramme = programme;
                ViewBag.ReportType = repType;
                ViewBag.IsPaged = (repType == "programme_paged");
                ViewBag.IncludeSignature = includeSignature;
                ViewBag.GeneratedDate = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt");
                ViewBag.ActiveSetting = GetActiveElectionSetting();
                ViewBag.ActiveAcademicYear = GetActiveAcademicYear();

                if (statuses.Count == 1)
                {
                    ViewBag.Status = (statuses[0] == "APPROVED") ? "Approved (Attending)" : "Cannot Attend (Rejected)";
                }
                else if (statuses.Count > 1)
                {
                    ViewBag.Status = "Approved & Rejected";
                }
                else
                {
                    ViewBag.Status = "All Statuses";
                }

                if (progIds.Count == 1)
                {
                    try
                    {
                        var progSql = "SELECT PROGRAMME_NAME FROM cp_programme WHERE PROGRAMME_ID = @ID LIMIT 1;";
                        var pNames = DapperHandler.FetchAll<string>(progSql, new { ID = progIds[0] });
                        ViewBag.ProgrammeName = (pNames != null && pNames.Count > 0) ? pNames[0] : "Programme ID: " + progIds[0];
                    }
                    catch
                    {
                        ViewBag.ProgrammeName = "Programme ID: " + progIds[0];
                    }
                }
                else if (progIds.Count > 1)
                {
                    try
                    {
                        var progSql = "SELECT PROGRAMME_NAME FROM cp_programme WHERE PROGRAMME_ID IN (" + string.Join(",", progIds) + ");";
                        var pNames = DapperHandler.FetchAll<string>(progSql, null);
                        ViewBag.ProgrammeName = (pNames != null && pNames.Count > 0)
                            ? (pNames.Count <= 3 ? string.Join(", ", pNames) : string.Join(", ", pNames.Take(2)) + " and " + (pNames.Count - 2) + " more (" + pNames.Count + " Selected)")
                            : progIds.Count + " Programmes Selected";
                    }
                    catch
                    {
                        ViewBag.ProgrammeName = progIds.Count + " Programmes Selected";
                    }
                }
                else
                {
                    ViewBag.ProgrammeName = "All Programmes";
                }

                if (repType == "programme")
                {
                    var where = new StringBuilder("WHERE r.IS_DELETED = 0 ");
                    var p = new Dictionary<string, object>();
                    if (acYear != "ALL")
                    {
                        where.Append("AND r.ACADEMIC_YEAR = @AC_YEAR ");
                        p.Add("AC_YEAR", acYear);
                    }
                    if (progIds.Count > 0)
                    {
                        where.Append("AND r.PROGRAMME IN (" + string.Join(",", progIds) + ") ");
                    }
                    if (statuses.Count == 1)
                    {
                        where.Append("AND r.ELECTION_STATUS = @STATUS ");
                        p.Add("STATUS", statuses[0]);
                    }
                    else if (statuses.Count > 1)
                    {
                        where.Append("AND r.ELECTION_STATUS IN ('" + string.Join("','", statuses) + "') ");
                    }

                    var summarySql = @"SELECT 
                                           COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified') AS PROGRAMME_NAME,
                                           COUNT(*) AS TOTAL_COUNT,
                                           SUM(CASE WHEN r.ELECTION_STATUS = 'APPROVED' THEN 1 ELSE 0 END) AS APPROVED_COUNT,
                                           SUM(CASE WHEN r.ELECTION_STATUS = 'REJECTED' THEN 1 ELSE 0 END) AS REJECTED_COUNT
                                       FROM alumni_election_registration r
                                       LEFT JOIN cp_programme p ON r.PROGRAMME = p.PROGRAMME_ID
                                       " + where.ToString() + @"
                                       GROUP BY COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified')
                                       ORDER BY PROGRAMME_NAME ASC;";

                    var dataList = DapperHandler.FetchAll<ProgrammeWiseElectionReportItem>(summarySql, p) ?? new List<ProgrammeWiseElectionReportItem>();
                    return View("PrintReport", dataList);
                }
                else if (repType == "programme_list" || repType == "programme_paged")
                {
                    var where = new StringBuilder("WHERE r.IS_DELETED = 0 ");
                    var p = new Dictionary<string, object>();

                    if (acYear != "ALL")
                    {
                        where.Append("AND r.ACADEMIC_YEAR = @AC_YEAR ");
                        p.Add("AC_YEAR", acYear);
                    }

                    if (progIds.Count > 0)
                    {
                        where.Append("AND r.PROGRAMME IN (" + string.Join(",", progIds) + ") ");
                    }

                    if (statuses.Count == 1)
                    {
                        where.Append("AND r.ELECTION_STATUS = @STATUS ");
                        p.Add("STATUS", statuses[0]);
                    }
                    else if (statuses.Count > 1)
                    {
                        where.Append("AND r.ELECTION_STATUS IN ('" + string.Join("','", statuses) + "') ");
                    }

                    var listSql = @"SELECT r.ID, r.ACADEMIC_YEAR, r.NAME, r.MOBILE_NO, r.PROGRAMME, 
                                           COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified') AS PROGRAMME_NAME, 
                                           COALESCE(p.PROGRAMME_CODE, '') AS PROGRAMME_CODE,
                                           r.YEAR_OF_PASSING, r.ELECTION_STATUS, r.CREATED_DATE, r.UPDATED_DATE 
                                    FROM alumni_election_registration r 
                                    LEFT JOIN cp_programme p ON r.PROGRAMME = p.PROGRAMME_ID 
                                    " + where.ToString() + @"
                                    ORDER BY COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified') ASC, r.NAME ASC;";

                    var dataList = DapperHandler.FetchAll<AlumniElectionRegistration>(listSql, p) ?? new List<AlumniElectionRegistration>();
                    foreach (var item in dataList)
                    {
                        string year = !string.IsNullOrWhiteSpace(item.ACADEMIC_YEAR) ? item.ACADEMIC_YEAR.Trim() : (acYear != "ALL" ? acYear : "2026");
                        item.SERIAL_NO = "SJC-ELEC-" + year + "-" + item.ID.ToString("D4");
                        item.REGISTRATION_NO = "SJC-" + (!string.IsNullOrWhiteSpace(item.PROGRAMME_CODE) ? item.PROGRAMME_CODE.Trim() : "ELEC") + "-" + year + "-" + GetAlumniSerialNo(item.MOBILE_NO, item.ID);
                    }
                    return View("PrintReport", dataList);
                }
                else
                {
                    var where = new StringBuilder("WHERE r.IS_DELETED = 0 ");
                    var p = new Dictionary<string, object>();

                    if (acYear != "ALL")
                    {
                        where.Append("AND r.ACADEMIC_YEAR = @AC_YEAR ");
                        p.Add("AC_YEAR", acYear);
                    }

                    if (progIds.Count > 0)
                    {
                        where.Append("AND r.PROGRAMME IN (" + string.Join(",", progIds) + ") ");
                    }

                    if (statuses.Count == 1)
                    {
                        where.Append("AND r.ELECTION_STATUS = @STATUS ");
                        p.Add("STATUS", statuses[0]);
                    }
                    else if (statuses.Count > 1)
                    {
                        where.Append("AND r.ELECTION_STATUS IN ('" + string.Join("','", statuses) + "') ");
                    }

                    var listSql = @"SELECT r.ID, r.ACADEMIC_YEAR, r.NAME, r.MOBILE_NO, r.PROGRAMME, 
                                           COALESCE(p.PROGRAMME_NAME, r.PROGRAMME_NAME, 'Unspecified') AS PROGRAMME_NAME, 
                                           COALESCE(p.PROGRAMME_CODE, '') AS PROGRAMME_CODE,
                                           r.YEAR_OF_PASSING, r.ELECTION_STATUS, r.CREATED_DATE, r.UPDATED_DATE 
                                    FROM alumni_election_registration r 
                                    LEFT JOIN cp_programme p ON r.PROGRAMME = p.PROGRAMME_ID 
                                    " + where.ToString() + @"
                                    ORDER BY r.NAME ASC;";

                    var dataList = DapperHandler.FetchAll<AlumniElectionRegistration>(listSql, p) ?? new List<AlumniElectionRegistration>();
                    foreach (var item in dataList)
                    {
                        string year = !string.IsNullOrWhiteSpace(item.ACADEMIC_YEAR) ? item.ACADEMIC_YEAR.Trim() : (acYear != "ALL" ? acYear : "2026");
                        item.SERIAL_NO = "SJC-ELEC-" + year + "-" + item.ID.ToString("D4");
                        item.REGISTRATION_NO = "SJC-" + (!string.IsNullOrWhiteSpace(item.PROGRAMME_CODE) ? item.PROGRAMME_CODE.Trim() : "ELEC") + "-" + year + "-" + GetAlumniSerialNo(item.MOBILE_NO, item.ID);
                    }
                    return View("PrintReport", dataList);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objLog = new ErrorLog())
                {
                    objLog.WriteError("AlumniElectionController", "PrintReport", ex.Message);
                }
                return Content("<script>alert('Error generating print view: " + ex.Message + "'); window.close();</script>");
            }
        }
    }

    public class AlumniDataCollectionItem
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string MOBILE_NO { get; set; }
        public string PROGRAMME { get; set; }
        public string BATCH { get; set; }
    }
}

