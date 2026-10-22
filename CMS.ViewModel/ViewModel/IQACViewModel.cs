using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.ComponentModel;
using CMS.ViewModel.Model;
namespace CMS.ViewModel.ViewModel
{
    public class IQACViewModel
    {
        #region Properties
        public bool sFlag = false;

        public string sMessage { get; set; }
        #endregion

        #region LinQ List
        public List<CALENDER_2017> liMonthList { get; set; }
        public List<SUP_MONTHS> liSupMonthList { get; set; }
        public List<IQAC_TEACHING_SUBJECTS_WORKLOAD> liTeachingSubjectWorkload { get; set; }
        public List<IQAC_TEACHING_SUBJECTS_WORKLOAD> liClassCourseStaff { get; set; }

        public List<IQAC_PREPARATION_AND_UPDATION_OF_NOTES> liPreparation { get; set; }
        public List<IQAC_CO_CURRICULAR_ASPECTS> liCurricularAspects { get; set; }
        public List<IQAC_EXTENSION_WORKS> liExtensionWorks { get; set; }
        public List<IQAC_PERSONAL_ACADEMIC_ACTIVITY> liPersonalAcademicActivity { get; set; }
        public List<IQAC_WORK_DONE_DATA> liWorkDoneData { get; set; }
        public List<IQAC_EXAMINATION_DUTIES_INTERNAL> liExaminationDutiesInternal { get; set; }
        public List<IQAC_PROFESSIONAL_ACADEMIC_ACTIVITIES> liProfessionalAcademicActivity { get; set; }
        public List<IQAC_RESEARCH_PROJECTS> liResearchProjects { get; set; }
        public List<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_A> liResearchAcademicContributionA { get; set; }
        public List<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_B> liResearchAcademicContributionB { get; set; }
        public List<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_C> liResearchAcademicContributionC { get; set; }
        public List<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_D> liResearchAcademicContributionD { get; set; }
        public List<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_E> liResearchAcademicContributionE { get; set; }
        public List<IQAC_AWARDS> liAwards { get; set; }
        public List<IQAC_SUBJECT_ASSOCIATION> liSubjectAssociation { get; set; }
        public List<IQAC_HONOURS> liHonor { get; set; }
        public List<IQAC_MEMBERSHIP> liMembership { get; set; }
        public List<IQAC_PROJECT> liIQACProject { get; set; }
        public List<IQAC_RESEARCH_GUIDENCE> liIQACResearchGuidence{get; set;}
        public List<IQAC_PHD_GUIDENCE> liIQACPhdGuidence { get; set; }
        public List<IQAC_BOOK_PUBLICATION> liBookPublication { get; set; }
        public List<IQAC_PAPER_PUBLICATION> liPaperPublication { get; set; }
        public List<IQAC_STAFF_ACTIVITY> liStaffActivity { get; set; }
        public List<IQAC_CURRICULAR_COORDINATOR> liCommittee { get; set; }
        public List<STF_LESSON_PLAN> liLessonPlan { get; set; }
        public List<CollegeDetails> liCollegeDetails { get; set; }
        public List<DEPT_DAILY_ACTIVITY> liDailyActivity { get; set; }
        public List<IQAC_ASSOCIATION_ACTIVITIES> liAssociationActivity { get; set; }
        public List<IQAC_SEMINOR_WORKSOP_ORGANIZED> liSeminorWorkshopOrganized { get; set; }
        public List<SUP_IQAC_RESOURCE_PERSON> liResourcePerson { get; set; }
        public List<SUP_IQAC_ORGANIZER> liOrganizer { get; set; }

        public List<IQACReport> liReport { get; set; }
        #endregion

        #region Select List
        public SelectList sCourseList { get; set; }
        public SelectList sClassList { get; set; }
        public SelectList sMonthList { get; set; }
        public SelectList sAcademicYearList { get; set; }
        public SelectList sIQACQuestionsList { get; set; }
        public SelectList sProgrammeTypeList { get; set; }
        public SelectList sLevelList { get; set; }
        public SelectList sActivityType { get; set; }
        public SelectList sIQACProjecttype { get; set; }
        public SelectList sIQACTypeofFunding { get; set; }
        public SelectList sIQACCategory { get; set; }
        public SelectList sCommitteeList { get; set; }
        public SelectList sStatusList { get; set; }
        public SelectList sShiftList { get; set; }
        public SelectList sGuestList { get; set; }
        public SelectList sStaffList { get; set; }
        public SelectList sOrganizerList { get; set; }
        public SelectList sDepartmentList { get; set; }
        public SelectList sSemesterType { get; set; }
        #endregion

        #region Objects

        #endregion
    }
}
