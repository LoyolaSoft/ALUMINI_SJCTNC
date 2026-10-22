using CMS.Utility;
using CMS.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModel.ViewModel
{
    public class MentoringViewModel
    {
        public string sSQL = string.Empty;
        ResultArgs resultArg = new ResultArgs();
        DataValue dv = new DataValue();
        public SelectList AcademicYear { get; set; }
        public SelectList Acyear { get; set; }
        public SelectList Bacthlist { get; set; }
        public SelectList liprogramme { get; set; }
        public SelectList Shiftlist { get; set; }
        public SelectList StaffListDDL { get; set; }
        public SelectList DepartmentList { get; set; }
        public SelectList StudentList { get; set; }
        public SelectList SemesterType { get; set; }
        public SelectList liLCasslist { get; set; }
        public SelectList ClassList { get; set; }
       

        public string message { get; set; }


        public List<MENTOR_GOAL_IN_LIFE> liGoalInLife { get; set; }

        public List<MentoringVisit> listformonitorvist = new List<MentoringVisit>();
        public List<MENTORING_VISITED_STUDENT> liVisitedStudentdet { get; set; }
        public List<MENTORING_STUDENT_LIST_FOR_STAFF> liMentorForStaff { get; set; }
        public List<STUDENT_TAG_MAPPING> StudentTagMapping { get; set; }
        public List<MENTORING_VISITED_DETAILS> liMentoringvisiteddel { get; set; }
        public List<SEMESTER_RESULT> liSemesterList { get; set; }
        public List<SKILLS_AND_TALENTS> SkillList { get; set; }
        public List<Student_Health_Details> liHealthstudentList { get; set; }
        public List<MENTORS_REGULAR_VISIT> liRegularvisitdetails { get; set; }
        public List<SUP_STUDENT_TAG_LIST> TagNameLI { get; set; }
        public List<MENTOR_GROUP_SESSION_RECORDS> liGroupSessionRecords { get; set; }
        public List<MENTORING_STUDENT_LIST> limentorestudentlist { get; set; }
        public List<STU_personal_info> listudentList { get; set; }
        public List<STUDENT_FAMILY_DETAILS> listudentFamList { get; set; }
        public List<MENTORING_ACTIVITY_RESPONSE> listudentGuid { get; set; }
        public List<MENTORING_STUDENT_LIST> limentorstulist { get; set; }
        public List<MENTORING_ACTIVITY_RESPONSE> listudentValues { get; set; }
        public List<MENTORING_ACTIVITY_RESPONSE> listudentvaluesdetails { get; set; }
        public List<MENTORING_HEALTH_DETAILS> lihealthstudentdet { get; set; }
        public List<MENTORING_STUDENT_ACADEMIC_DETAILS> listuacademicdet { get; set; }
        public List<MENTORING_STUDENT_ACADEMIC_ANALYSIS> liAcademicAnalysis { get; set; }
        public List<MENTORING_HEALTH_DETAILS> liStudentHealthDetails { get; set; }
        public List<SEMESTER_RESULT> liSemesterMarks { get; set; }
        public List<SKILLS_AND_TALENTS> liSkillAndTalents { get; set; }
        public List<MENTOR_SPORTS> liSports { get; set; }
        public List<PARENT_MEETING> liParentMeeting { get; set; }
        public List<MENTOR_LATE_ENTRY> liLateEntry { get; set; }
 
        public List<MENTOR_VAC_PROFESSIONAL> liVACCourses { get; set; }
        public List<MENTOR_LEADERSHIP_CONTRIBUTION> liContributions { get; set; }
       
        public List<SelectListItem> ContributionTypeList { get; set; }
        public List<PARTICIPATION_ACHIEVEMENTS> liParticipationAchievement { get; set; }
        public List<LanguageDetailsViewModel> listudentLanguageList { get; set; }
        public List<Stu_list> listudentlist { get; set; }
        public List<STU_personal_info> listaffdetails { get; set; }
        public List<STF_INFO> listaffinfo { get; set; }
        public List<MENTORING_VISITED_STUDENT> listumentorregistration { get; set; }
        public List<OverallMentorAssessmentRow> liOverallMentorAssessment { get; set; }

        public class OverallAssessmentSaveVM
        {
            public string studentId { get; set; }
            public List<OverallMentorAssessmentRow> data { get; set; }
        }
        public List<BEHAVIOURAL_CONDUCT_RECORD> liBehaviouralRecords { get; set; }
        public List<OverallMentorAssessmentRow> liOverallAssessmentRecords { get; set; }
        public List<OverallMentorAssessmentRow> liAssessmentObjectives { get; set; }
        public List<SelectListItem> BehaviourLevelList { get; set; }
        public List<SelectListItem> OverallAcademicAssessment { get; set; }
        public List<MENTORING_COUNSELLING_SESSION> liCounsellingSessions { get; set; }
        public List<SelectListItem> SemesterList { get; set; }
        public List<MENTORING_ATTENDANCE_REPORT> liAttendance { get; set; }
       
        public List<STU_LATE_ENTRY> liStuLateFee { get; set; }


        public SelectList ProgramName { get; set; }
        public SelectList ClassName { get; set; }
        public SelectList StuName { get; set; }
        public SelectList ShiftList { get; set; }
        public string NatureofVisit { get; set; }
        public SelectList FamilyType { get; set; }
        public SelectList Staff { get; set; }
        public string InitialContactVisit { get; set; }
        public string VisitDate { get; set; }
        public string VisitTime { get; set; }
        public string VisitDuration { get; set; }
        public string MatterDiscussed { get; set; }
        public string ConclusionArrived { get; set; }
        public string ActionTaken { get; set; }
        public string Notes { get; set; }
        public string ImpressionaboutStudent { get; set; }
        public string NextVisitDate { get; set; }
        public string RegularVisit { get; set; }
        public string ShortReview { get; set; }
        public string CLASS_ID { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SHIFT_LIST { get; set; }
        public string StudentId { get; set; }
        public string RollNo { get; set; }
        public string FirstName { get; set; }
        public string ClassCode { get; set; }
        public string NatureOfVisit { get; set; }
        public string ConclusionArrivedAt { get; set; }
        public string GeneralImpression { get; set; }
        public string CLASSID { get; set; }
        public string PROGRAMID { get; set; }
        public string STUDENT_GUIDANCE_ID { get; set; }






    }
}
