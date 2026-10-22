using CMS.DAO;
using CMS.SQL.Academics;
using CMS.SQL.Student;
using CMS.SQL.SupportData;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using Remotion.Reflection.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Windows.Controls.Primitives;

namespace CMS.Controllers
{
    public class EventController : Controller
    {
        ResultArgs resultArgs = new ResultArgs();
        // GET: Event
        public ActionResult EventRegisterationForm()
        {
            AcademicsViewModel viewmodal = new AcademicsViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                viewmodal.liEventRegisterForm = (List<EVENT_REGISTER_FORM>)CMSHandler.FetchData<EVENT_REGISTER_FORM>(null,
                AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchEventReisterForm), sAcademicYear).DataSource.Data;

                var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchClassInchargeByAcYear), sAcademicYear).DataSource.Data;
                if (liClass != null && liClass.Count > 0)
                {
                    viewmodal.liLCasslist = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }

                var liClassYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear), sAcademicYear).DataSource.Data;
                if (liClassYear != null && liClassYear.Count > 0)
                {
                    viewmodal.liYearlist = new SelectList(liClassYear, Common.SubClassYear.CLASS_YEAR_ID, Common.SubClassYear.CLASS_YEAR);
                }



                var liprogramLevel = (List<SUP_PROGRAMME_LEVEL>)CMSHandler.FetchData<SUP_PROGRAMME_LEVEL>(null,
               SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel)).DataSource.Data;
                if (liprogramLevel != null && liprogramLevel.Count > 0)
                {
                    viewmodal.liprogramLevel = new SelectList(liprogramLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
                }

                var liSupEventMode = (List<SUP_EVENT_MODE>)CMSHandler.FetchData<SUP_EVENT_MODE>(null,
                SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupMode)).DataSource.Data;
                if (liSupEventMode != null && liSupEventMode.Count > 0)
                {
                    viewmodal.liSupEventMode = new SelectList(liSupEventMode, Common.SUP_EVENT_MODE.MODE_ID, Common.SUP_EVENT_MODE.MODE_NAME);
                }

                var liSupShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null,
                SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                if (liSupShift != null && liSupShift.Count > 0)
                {

                    viewmodal.liSupShift = new SelectList(liSupShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }

                var liSupRole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null,
             SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRole)).DataSource.Data;
                if (liSupRole != null && liSupRole.Count > 0)
                {
                    viewmodal.liSupRole = new SelectList(liSupRole, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);
                }

            }

            return View(viewmodal);
        }



        public JsonResult SaveEventRegisteration(string Register)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)

            {
                try
                {

                    var result = JsonConvert.DeserializeObject<EVENT_REGISTER_FORM>(Register);
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sSemesterType = Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString();


                    result.DATE = CommonMethods.DateConvertionForPayU(result.DATE);
                    //result.TO_DATE = CommonMethods.ConvertdatetoyearFromat(result.TO_DATE);
                    result.ACADEMIC_YEAR = sAcademicYear;
                    result.SEMESTER_ID = sSemesterType;

                    if (result.EVENT_ID != "" && result.EVENT_ID != null)
                    {
                        result.CLASS_ID = result.CLASSES_ID;


                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.UpdateEventRegisteration), sAcademicYear);
                        objResultData.Message = resultArgs.Success ?Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = (resultArgs.Success ) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(result.CLASSES_ID) && result.CLASSES_ID != "null")
                        {
                            var classIds = result.CLASSES_ID.Split(',');

                            foreach (var classId in classIds)
                            {
                                // Trim in case there are spaces
                                result.CLASS_ID = classId.Trim();

                                resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.AddEventRegister), sAcademicYear);
                                objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                            }
                        }
                        else
                        {
                            result.CLASS_ID = null;

                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.AddEventRegister), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                        }
                    }
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Event", "SaveEventRegisteration", ex.ToString());
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

        public ActionResult EventRegisterStudent()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString() ?? string.Empty;
            string sStudentId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString() ?? string.Empty;
            string sRole = Session[Common.SESSION_VARIABLES.USER_ROLE_ID]?.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Student basic details
                    var liStudentDetails = (List<STU_CLASS>)CMSHandler.FetchData<STU_CLASS>(
                        new STU_PERSONAL_INFO()
                        {
                            STUDENT_ID = sStudentId,
                            ACADEMIC_YEAR = sAcademicYear
                        },
                        AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.GetStudentDetailsForEventResgister),
                        sAcademicYear
                    ).DataSource.Data;

                    var stu = liStudentDetails.FirstOrDefault();
                    if (stu == null)
                    {
                        return View(new EventRegisterStudentViewModel());
                    }

                    // Fetch event list
                    var rawEvents = (List<EVENT_REGISTER_FORM>)CMSHandler.FetchData<EVENT_REGISTER_FORM>(
                        new EVENT_REGISTER_FORM()
                        {
                            CLASS_ID = stu.CLASS_ID,
                            ACADEMIC_YEAR = stu.ACADEMIC_YEAR,
                            SEMESTER_ID = stu.SEMESTER_ID,
                            SROLES = sRole,
                            STUDENT_ID = sStudentId
                        },
                        AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.GetEventsForStudent),
                        sAcademicYear
                    ).DataSource.Data;

                    // Convert to ViewModel list
                    List<EventRegisterFormModel> liEvents = new List<EventRegisterFormModel>();

                    foreach (var row in rawEvents)
                    {
                        // Values come directly as string? keep everything as string for UI
                        string totalSeats = row.TOTAL_SEATS.ToString() ?? "0";
                        string registeredCount = row.REGISTERED_COUNT.ToString() ?? "0";
                        string availableSeats = row.AVAILABLE_SEATS.ToString() ?? "0";
                        string isRegistered = row.IS_REGISTERED.ToString() ?? "0";

                        liEvents.Add(new EventRegisterFormModel
                        {
                            EVENT_ID = row.EVENT_ID,
                            CLASS_ID = row.CLASS_ID,
                            EVENT_NAME = row.EVENT_NAME,
                            EVENT_TIME = row.EVENT_TIME,
                            EVENT_VENUE = row.EVENT_VENUE,
                            TITLE = row.TITLE,
                            DATE = row.DATE,
                            MODE = row.MODE,
                            MODE_NAME = row.MODE_NAME,
                            ROLE = row.ROLE,

                            NO_OF_REGISTER = totalSeats,
                            REGISTERED_COUNT = registeredCount,
                            AVAILABLE_SEATS = availableSeats,
                            IS_REGISTERED = isRegistered,

                            IS_ACTIVE = row.IS_ACTIVE,
                            IS_DELETED = row.IS_DELETED,
                            ACADEMIC_YEAR = row.ACADEMIC_YEAR,
                            SEMESTER_ID = row.SEMESTER_ID
                        });
                    }



                    // Prepare ViewModel
                    EventRegisterStudentViewModel vm = new EventRegisterStudentViewModel()
                    {
                        STUDENT_ID = sStudentId,
                        CLASS_ID = stu.CLASS_ID,
                        SEMESTER_ID = stu.SEMESTER_ID,
                        ACADEMIC_YEAR = stu.ACADEMIC_YEAR,
                        Events = liEvents
                    };

                    return View(vm);
                }
                catch (Exception ex)
                {
                    EventRegisterStudentViewModel errorVm = new EventRegisterStudentViewModel();
                    ModelState.AddModelError("", "Error: " + ex.Message);
                    return View(errorVm);
                }
            }

            return View();
        }


        public JsonResult RegisterToEvent(string eventId)
        {
            JsonResultData objResultData = new JsonResultData();

            try
            {
                string studentId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();

                STUDENT_EVENT_REGISTER result = new STUDENT_EVENT_REGISTER()
                {
                    STUDENT_ID = studentId,
                    EVENT_ID = eventId
                };

                var resultArgs = CMSHandler.SaveOrUpdate(
                    result,
                    SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.AddStudentEventRegister),
                    sAcademicYear
                );

                objResultData.Message = resultArgs.Success
                    ? Common.Messages.RecordsSavedSuccessfully
                    : Common.Messages.FailedToSaveRecords;

                return Json(new
                {
                    success = resultArgs.Success,
                    message = objResultData.Message
                });
            }
            catch
            {
                return Json(new { success = false, message = "Registration failed!" });
            }
        }


        public ActionResult EventFeedbackQuestions()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString() ?? string.Empty;

            EventFeedbackViewModel model = new EventFeedbackViewModel();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var eventList = (List<EventMasterModel>)CMSHandler.FetchData<EventMasterModel>(
                        new EventMasterModel()
                        {
                            ACADEMIC_YEAR = sAcademicYear
                        },
                        AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.GetAllEventsBySemester),
                        sAcademicYear
                    ).DataSource.Data;

                    model.Events = eventList;
                    
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                }
            }

            return View(model);
        }

        public JsonResult GetRolesByEvent(string eventId)
        {
            var roles =(List<SUP_ROLE>) CMSHandler.FetchData<SUP_ROLE>(
                new EventMasterModel() { EVENT_ID = eventId },
                AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.GetRoleByEvent),
                null
            ).DataSource.Data;

            var roleList = roles.Select(r => new SelectListItem
            {
                Value = r.ROLE_ID.ToString(),
                Text = r.ROLE_NAME
            });

            return Json(roleList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQuestions(int eventId, int roleId)
        {
            var questions = CMSHandler.FetchData<EventFBQuestion>(
                new EventFBQuestion()
                {
                    EVENT_ID = eventId.ToString(),
                    ROLE_ID = roleId.ToString()
                },
                AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.GetQuestionByEventRoleID),
                null
            ).DataSource.Data as List<EventFBQuestion>;

            return PartialView("_QuestionsPartial", questions);
        }


        public ActionResult AddQuestion(int eventId, int roleId)
        {
            var questionTypes = CMSHandler.FetchData<SUP_QUIZ_QUESTION_TYPE>(
                null,
                AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.GetQuestionType),
                null
            ).DataSource.Data as List<SUP_QUIZ_QUESTION_TYPE>;

            var model = new EventFBQuestion
            {
                QUESTION_ID = "0",
                EVENT_ID = eventId.ToString(),
                ROLE_ID = roleId.ToString(),
                ACADEMIC_YEAR = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString(),
                QUESTION_TYPE = ""
            };

            ViewBag.QuestionTypes = new SelectList(
                questionTypes,
                "QUESTION_TYPE_ID",
                "QUESTION_TYPE",
                model.QUESTION_TYPE
             );

            return PartialView("_QuestionFormPartial", model);
        }



        public ActionResult EditQuestion(int questionId)
        {
            var questionTypes = CMSHandler.FetchData<SUP_QUIZ_QUESTION_TYPE>(
                null,
                AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.GetQuestionType),
                null
            ).DataSource.Data as List<SUP_QUIZ_QUESTION_TYPE>;

            var model = (CMSHandler.FetchData<EventFBQuestion>(
                new EventFBQuestion() { QUESTION_ID = questionId.ToString() },
                AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.GETQUESTIONBYID),
                null
            ).DataSource.Data as List<EventFBQuestion>)?.FirstOrDefault();

            if (model == null)
                return HttpNotFound("Question not found.");

            // ⭐ FIXED DROPDOWN VALUE BINDING ⭐
            ViewBag.QuestionTypes = questionTypes
                .Select(t => new SelectListItem
                {
                    Value = t.QUESTION_TYPE_ID.ToString(),
                    Text = t.QUESTION_TYPE,
                    Selected = t.QUESTION_TYPE_ID.ToString() == model.QUESTION_TYPE
                })
                .ToList();

            return PartialView("_QuestionFormPartial", model);
        }






        public JsonResult SaveQuestion(EventFBQuestion model)
        {
            JsonResultData objResultData = new JsonResultData();

            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
                bool isNew = Convert.ToInt32(model.QUESTION_ID) == 0;

                if (isNew)
                {
                    
                    var resultArgs = CMSHandler.SaveOrUpdate(
                        model,
                        SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.EventInsertQuestionSQL),
                        sAcademicYear
                    );

                    objResultData.Message = resultArgs.Success
                        ? Common.Messages.RecordsSavedSuccessfully
                        : Common.Messages.FailedToSaveRecords;

                    return Json(new { success = resultArgs.Success, message = objResultData.Message });
                }
                else
                {
                  
                    var resultArgs = CMSHandler.SaveOrUpdate(
                        model,
                        SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.EventUpdateQuestionSQL),
                        sAcademicYear
                    );

                    objResultData.Message = resultArgs.Success
                        ? Common.Messages.RecordsUpdatedSuccessfully
                        : Common.Messages.FailedToUpdatedRecords;

                    return Json(new { success = resultArgs.Success, message = objResultData.Message });
                }
            }
            catch (Exception ex)
            {
                objResultData.Message = "Error saving question: " + ex.Message;
                return Json(new { success = false, message = objResultData.Message });
            }
        }



        public JsonResult DeleteQuestion(int questionId)
        {
            try
            {
                string academicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();

                
                EventFBQuestion deleteModel = new EventFBQuestion()
                {
                    QUESTION_ID = questionId.ToString()
                };

               
                var resultArgs = CMSHandler.SaveOrUpdate(
                    deleteModel,
                    AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.DeleteEventQuetions),
                    academicYear
                );

                return Json(new
                {
                    success = resultArgs.Success,
                    message = resultArgs.Success ? "Deleted successfully" : "Failed to delete"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }


        public ActionResult EventFeedBackQuesObjective()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString() ?? string.Empty;

            EventFeedbackViewModel model = new EventFeedbackViewModel();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var eventList = (List<EventMasterModel>)CMSHandler.FetchData<EventMasterModel>(
                        new EventMasterModel()
                        {
                            ACADEMIC_YEAR = sAcademicYear
                        },
                        AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.GetAllEventsBySemester),
                        sAcademicYear
                    ).DataSource.Data;

                    model.Events = eventList;

                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                }
            }

            return View(model);
        }



        public ActionResult GetQuestionsobjective(int eventId, int roleId)
        {
            var questions = CMSHandler.FetchData<EventFBQuestion>(
                new EventFBQuestion()
                {
                    EVENT_ID = eventId.ToString(),
                    ROLE_ID = roleId.ToString()
                },
                AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FecthEventQuestionandObjective),
                null
            ).DataSource.Data as List<EventFBQuestion>;

            return PartialView("_QuestionsObjectivePartial", questions);
        }

        public ActionResult AddObjective(int eventId, int roleId)
        {
            var questions = CMSHandler.FetchData<EventFBQuestion>(
                new EventFBQuestion { EVENT_ID = eventId.ToString(), ROLE_ID = roleId.ToString() },
                AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FecthEventQuestion),
                null
            ).DataSource.Data as List<EventFBQuestion>;

            
            var objectives = CMSHandler.FetchData<EventFBQuestion>(
                null,
                AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.GetAllObjectives),
                null
            ).DataSource.Data as List<EventFBQuestion>;

            var model = new EventFBObjectiveMappingVM()
            {
                EVENT_ID = eventId.ToString(),
                ROLE_ID = roleId.ToString(),
                QuestionList = questions?.Select(x => new SelectListItem
                {
                    Value = x.QUESTION_ID,
                    Text = x.QUESTION
                }).ToList(),

                ObjectiveList = objectives?.Select(x => new SelectListItem
                {
                    Value = x.OBJECTIVE_ID,
                    Text = x.OBJECTIVES
                }).ToList(),
            };

            return View("BindAddObjectiveMapping", model);
        }


        public JsonResult SaveObjectiveMapping(EventFBObjectiveMappingVM model)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString() ?? "";

            try
            {
                if (model.SelectedQuestions == null || !model.SelectedQuestions.Any())
                    return Json(new { success = false, message = "Please select at least one question." });

                if (model.SelectedObjectives == null || !model.SelectedObjectives.Any())
                    return Json(new { success = false, message = "Please select at least one objective." });

                var questions = model.SelectedQuestions.Distinct().ToList();
                var objectives = model.SelectedObjectives.Distinct().ToList();
                int qObjId = 0;
                int.TryParse(model.QUESTION_OBJECT_ID, out qObjId);

                if (qObjId > 0)
                {
                    string qid = model.SelectedQuestions.First();
                    string oid = model.SelectedObjectives.First();

                    CMSHandler.SaveOrUpdate(
                        new EventFBQuestion()
                        {
                            QUESTION_OBJECT_ID = qObjId.ToString(),
                            QUESTION_ID = qid,
                            OBJECTIVE_ID = oid
                        },
                        AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.UpdateEventQuestionObjective)
                    );

                    return Json(new { success = true, message = "Mapping updated successfully!" });
                }

                foreach (var qid in questions)
                {
                    foreach (var oid in objectives)
                    {
                        CMSHandler.SaveOrUpdate(
                            new EventFBQuestion()
                            {
                                QUESTION_ID = qid,
                                OBJECTIVE_ID = oid,
                                EVENT_ID = model.EVENT_ID,
                                ROLE_ID = model.ROLE_ID,
                                ACADEMIC_YEAR = sAcademicYear
                            },
                            AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.InsertEventFBQuestionObjectiveMapping)
                        );
                    }
                }

                return Json(new { success = true, message = "Mapping inserted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult EditObjective(int eventId, int questionObjectId)
        {
            var rowsResult = CMSHandler.FetchData<EventFBQuestion>(
                new EventFBQuestion
                {
                    EVENT_ID = eventId.ToString(),
                    QUESTION_OBJECT_ID = questionObjectId.ToString()
                },
                AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FecthEditobjectiveQuestion)
            );

            var rows = rowsResult.DataSource.Data as List<EventFBQuestion>;
            if (rows == null || rows.Count == 0)
                return Json(new { status = false, message = "Mapping not found." }, JsonRequestBehavior.AllowGet);

            var selectedQuestionIds = rows
                .Select(x => x.QUESTION_ID.ToString())
                .Distinct()
                .ToList();

            var selectedObjectiveIds = rows
                .Select(x => x.OBJECTIVE_ID.ToString())
                .Distinct()
                .ToList();

            var objectivesResult = CMSHandler.FetchData<EventFBQuestion>(
                null,
                AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.GetAllObjectives),
                null
            );

            var allObjectives = objectivesResult.DataSource.Data as List<EventFBQuestion> ?? new List<EventFBQuestion>();

            var model = new EventFBObjectiveMappingVM
            {
                QUESTION_OBJECT_ID = rows.First().QUESTION_OBJECT_ID,
                EVENT_ID = eventId.ToString(),
                ROLE_ID = rows.First().ROLE_ID,

                QuestionList = rows
                    .Select(q => new SelectListItem
                    {
                        Value = q.QUESTION_ID.ToString(),
                        Text = q.QUESTION,
                        Selected = selectedQuestionIds.Contains(q.QUESTION_ID.ToString())
                    })
                    .Distinct()
                    .ToList(),

                ObjectiveList = allObjectives
                    .Select(o => new SelectListItem
                    {
                        Value = o.OBJECTIVE_ID.ToString(),
                        Text = o.OBJECTIVES,
                        Selected = selectedObjectiveIds.Contains(o.OBJECTIVE_ID.ToString())
                    })
                    .ToList()
            };

            return View("BindAddObjectiveMapping", model);
        }


        public JsonResult DeleteObjective(int questionObjectId)
        {
            if (questionObjectId <= 0)
                return Json(new { success = false, message = "Invalid mapping ID." });

            try
            {
                // Assign variable
                var objToDelete = new EventFBQuestion
                {
                    QUESTION_OBJECT_ID = questionObjectId.ToString()
                };

                
                var rowsAffected = CMSHandler.SaveOrUpdate(
                    objToDelete,
                    AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.DeleteEventFBObjective)
                );

                if (rowsAffected.Success)
                {
                    return Json(new { success = true, message = "Mapping deleted successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "No mapping was deleted. Check if the ID exists." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }







        public JsonResult FetchClassListByClassYarandShift(string sProgramId, string sShiftId, string classyear)
        {
            string sClass = string.Empty;
            string sAcademicYear = string.Empty;
            cp_Classes_2017 objModel = new cp_Classes_2017();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    var Class = (List<cp_Classes_2017>)CMS.DAO.CMSHandler.FetchData<cp_Classes_2017>(objModel, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchClassListByClassYarandShift).Replace("?CLASS_LEVEL", sProgramId).Replace("?CLASS_YEAR", classyear).Replace("?SHIFT", sShiftId), sAcademicYear).DataSource.Data;
                    foreach (var item in Class)
                    {
                        sClass += "<option value='" + item.CLASS_ID + "'>" + item.CLASS_NAME + "</option>";
                    }
                }
                else
                    return Json(sClass);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EventRegisterForm", "Event", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EventRegisterForm", "Event", ex.Message);
                }
            }
            return Json(sClass);
        }




        public JsonResult FetchEventRegisterById(string EventID)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && EventID != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = (List<EVENT_REGISTER_FORM>)CMSHandler.FetchData<EVENT_REGISTER_FORM>(new EVENT_REGISTER_FORM()
                    { EVENT_ID = EventID }, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchEventRegisterById), sAcademicYear).DataSource.Data;
                    if (result != null && result.Count > 0)
                        objResultData.oResult = result;
                    else
                        objResultData.Message = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Event", "FetchEventRegisterById", ex.Message);
                    }
                }

            }
            else
            {
                objResultData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult DeleteEventRegisterById(string EventID)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && EventID != "")
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    resultArgs = CMSHandler.SaveOrUpdate(new EVENT_REGISTER_FORM { EVENT_ID = EventID, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.DeleteEventRegisterById), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Event", "DeleteEventRegisterById", ex.Message);
                    }
                }
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }



        #region Event Feedback Entry

        public async Task<ActionResult> EventFeedbackEntry()
        {
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            FeedbackViewModel.sAcademicAttempt = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? 0 : FeedbackViewModel.sAcademicAttempt + 1;
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserRollId = Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            string sRole = Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            try
            {
                //fetch user role 

                if (!string.IsNullOrEmpty(sRole))
                {

                    var fbSetting = (List<EVENT_FBQUESTIONS>)CMS.DAO.CMSHandler.FetchData<EVENT_FBQUESTIONS>(new STU_CLASS { STUDENT_ID = sUserId, ACADEMIC_YEAR = sAcademicYear }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchEventFeedbackEntry), sAcademicYear).DataSource.Data;
                    if (fbSetting != null && fbSetting.Count > 0)
                    {
                        objViewModel.EventFeedBackStudent = new SelectList(fbSetting, Common.EVENT_FBQUESTIONS.EVENT_ID, Common.EVENT_FBQUESTIONS.EVENT_NAME);
                    }
                    else
                    {
                        objViewModel.sErrorMessage = "There is no feedback For your Class";
                    }
                }
                return View(objViewModel);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EventController", "EventFeedbackEntry", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EventController", "EventFeedbackEntry", ex.Message);
                }
                return RedirectToAction("EventFeedbackEntry", "Account");
            }

        }

        public async Task<ActionResult> FeedBackQuestionViewNew(string sFeedBackId)
        {
            FeedbackViewModel objFeedback = new FeedbackViewModel();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                string sUserRollId = Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
                string sRole = Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
                
               

                List<EVENT_FEEDBACK_OBJECTIVE> ObjectiveList = new List<EVENT_FEEDBACK_OBJECTIVE>();
                List<EVENT_FEEDBACK_QUESTIONS> QuestoinList = new List<EVENT_FEEDBACK_QUESTIONS>();
                List<EVENT_FEEDBACK_OBJECTIVE> MappingList = new List<EVENT_FEEDBACK_OBJECTIVE>();


                // validate if is attended already
                //student to staff by class and course
                if (!string.IsNullOrEmpty(sFeedBackId))
                {

                    var IsExist = (List<EVENT_FEEDBACK_EVALUATION>)CMSHandler.FetchData<EVENT_FEEDBACK_EVALUATION>(new EVENT_FEEDBACK_EVALUATION() { STUDENT_ID = sUserId, ACADEMIC_YEAR = sAcademicYear }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.ValidateEventByStudentId), sAcademicYear).DataSource.Data;
                    if (IsExist != null && IsExist.Count > 0)
                    {
                        objFeedback.sErrorMessage = "Already Attended!";
                        return View(objFeedback);
                    }
                    else
                    {

                        ObjectiveList = (List<EVENT_FEEDBACK_OBJECTIVE>)CMSHandler.FetchData<EVENT_FEEDBACK_OBJECTIVE>(new EVENT_FEEDBACK_QUESTIONS() { STUDENT_ID = sUserId, ACADEMIC_YEAR = sAcademicYear, EVENT_ID = sFeedBackId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchEventObjectiveList), sAcademicYear).DataSource.Data;
                        objFeedback.EventObjectiveList = ObjectiveList;

                        MappingList  = (List<EVENT_FEEDBACK_OBJECTIVE>)CMSHandler.FetchData<EVENT_FEEDBACK_OBJECTIVE>(new EVENT_FEEDBACK_QUESTIONS() { STUDENT_ID = sUserId, ACADEMIC_YEAR = sAcademicYear, EVENT_ID = sFeedBackId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchEventMappingList), sAcademicYear).DataSource.Data;
                        objFeedback.EventMappingList = MappingList;


                        var QuestionList = (List<EVENT_FEEDBACK_QUESTIONS>)CMSHandler.FetchData<EVENT_FEEDBACK_QUESTIONS>(new EVENT_FEEDBACK_EVALUATION() { STUDENT_ID = sUserId, ACADEMIC_YEAR = sAcademicYear ,EVENT_ID= sFeedBackId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchEventQuestionList), sAcademicYear).DataSource.Data;
                        if (QuestionList != null && QuestionList.Count > 0)
                        {
                            objFeedback.EventQuestionList = QuestionList;
                        }
                        else
                        {
                            objFeedback.sErrorMessage = "No questions were added!";
                        }
                        //fetch assessee name
                       
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EventController", "FeedBackQuestionViewNew", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EventController", "FeedBackQuestionViewNew", ex.Message);
                }
                return RedirectToAction("Index1", "Account");
            }

            return View(objFeedback);
        }

        public JsonResult SaveFeedBackAnswer(string sJsonData, string sFeedBackAnswer)
        {
            JsonResultData objJsonData = new JsonResultData();
            FeedbackViewModel objViewModel = new FeedbackViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sClassId = Session[Common.SESSION_VARIABLES.CLASS_ID] != null ? Session[Common.SESSION_VARIABLES.CLASS_ID].ToString() : "0";
            string sRoleId = Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : "0";

            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sUserId))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<JSON_SAVE_QUESTION>(sFeedBackAnswer);
                    var Result1 = JsonConvert.DeserializeObject<EVENT_FEEDBACK_EVALUATION>(sJsonData);
                    if (!string.IsNullOrEmpty(Result1.EVENT_ID))
                    {
                        //validate if it is already existed or not
                        
                            var IsExist = (List<EVENT_FEEDBACK_EVALUATION>)CMSHandler.FetchData<EVENT_FEEDBACK_EVALUATION>(new EVENT_FEEDBACK_EVALUATION() { STUDENT_ID = sUserId, EVENT_ID = Result1.EVENT_ID }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.ValidateEventByStudentId), sAcademicYear).DataSource.Data;
                            if (IsExist != null && IsExist.Count > 0)
                            {
                                objJsonData.Message = Common.Messages.RecordsExistAlready;
                                return Json(objJsonData, JsonRequestBehavior.AllowGet);
                            }
                        
                        String sSQL = @"INSERT INTO EVENT_FBEVALUATION(STUDENT_ID,CLASS_ID,EVENT_ID,QUESTION,ANSWER,ANSWER_IN_TEXT,ROLE_ID,ISDELETED)VALUES";
                        foreach (var item in Result.liFB_Answer)
                        {
                            if (!string.IsNullOrEmpty(item.QUESTION))
                            {
                                string answer = !string.IsNullOrEmpty(item.ANSWER) ? item.ANSWER : "0";
                                string answerText = !string.IsNullOrEmpty(item.ANSWER_IN_TEXT) ? item.ANSWER_IN_TEXT : "null";

                                sSQL += "('" + sUserId + "','" + sClassId + "','" + Result1.EVENT_ID + "','" + item.QUESTION + "','" + answer + "','" + answerText + "','" + sRoleId + "','0'),";
                            }

                        }
                        sSQL = sSQL.Trim(',') + ";";
                        
                        resultArgs = objViewModel.SaveFeedback(sSQL);
                        
                        objJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;

                    }
                    else
                    {
                        objJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("EventController", "SaveFeedBackAnswer", ex.Message);
                    }
                    objJsonData.Message = "Error occurred"; 
                }
            }
            else
            {
                objJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(objJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion



    }

}