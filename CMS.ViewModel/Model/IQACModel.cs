using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{
    public class IQACModel
    {
        public string STAFF_ID { get; set; }
        public string FIRST_NAME { get; set; }
    }

    public class IQAC_TEACHING_SUBJECTS_WORKLOAD
    {
        public string IQAC_ID { get; set; }
        public string WORKLOAD_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string LECTURE_ALLOTTED { get; set; }
        public string LECTURE_TAKEN { get; set; }
        public string PRACTICAL_ALLOTTED { get; set; }
        public string PRACTICAL_TAKEN { get; set; }
        public string CASUAL_LEAVE { get; set; }
        public string ON_DUTY { get; set; }
        public string MEDICAL_LEAVE { get; set; }
        public string ENTRY_DTATE { get; set; }
        public string MONTH_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FIRST_NAME { get; set; }
        public string CLASS_CODE { get; set; }
        public string SEMESTER_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string DEPARTMENT { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public string STAFF_CODE { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string DAY_ORDER_MONTH { get; set; }


    }

    public class IQAC_PREPARATION_AND_UPDATION_OF_NOTES
    {
        public string IQAC_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string TOPIC { get; set; }
        public string TITLE { get; set; }
        public string AUTHOR { get; set; }
        public string PUBLICATION { get; set; }
        public string YEAR_OF_PUBLICATION { get; set; }
        public string PLACE_OF_LIBRARY { get; set; }
        public string TOPIC_REFERRED { get; set; }
        public string WEBSITE_REFFERED { get; set; }
        public string E_RESOURSE_DEVELOPED { get; set; }
        public string FILE_PATH { get; set; }
        public string IMG_PATH { get; set; }
        public string ENTRY_DTATE { get; set; }
        public string MONTH_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

        public string FIRST_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public string CLASS_CODE { get; set; }
        public string CLASS_NAME { get; set; }
        public string SEMESTER_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string DAY_ORDER_MONTH { get; set; }
        public string STAFF_CODE { get; set; }
        public string QUESTION { get; set; }


    }

    public class IQAC_CO_CURRICULAR_ASPECTS
    {
        public string IQAC_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string DEPT_CODE { get; set; }
        public string NAME_OF_COMMITEE { get; set; }
        public string ACTIVITY_DATE { get; set; }
        public string FILE_PATH { get; set; }
        public string IMG_PATH { get; set; }
        public string ACTIVITY_COMPLETED { get; set; }
        public string ENTRY_DTATE { get; set; }
        public string MONTH_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FIRST_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public string CLASS_CODE { get; set; }
        public string CLASS_NAME { get; set; }
        public string SEMESTER_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string DAY_ORDER_MONTH { get; set; }
        public string STAFF_CODE { get; set; }

    }

    public class IQAC_EXTENSION_WORKS
    {
        public string IQAC_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string DEPT_CODE { get; set; }
        public string ACTIVITY_DATE { get; set; }
        public string NAME_OF_THE_PROGRAMME { get; set; }
        public string ACTIVITY_COMPLETED { get; set; }
        public string NO_OF_PARTICIPANTS { get; set; }
        public string PLACE { get; set; }
        public string ENTRY_DTATE { get; set; }
        public string MONTH_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FILE_PATH { get; set; }
        public string IMG_PATH { get; set; }
        public string FIRST_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string CLASS_NAME { get; set; }
        public string SEMESTER_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string DAY_ORDER_MONTH { get; set; }
        public string STAFF_CODE { get; set; }
    }

    public class IQAC_PERSONAL_ACADEMIC_ACTIVITY
    {
        public string IQAC_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string CLASS_CODE { get; set; }
        public string SEMESTER_ID { get; set; }
        public string DEPT_CODE { get; set; }
        public string ACTIVITY_DATE { get; set; }
        public string ACTIVITY { get; set; }
        public string TOPIC { get; set; }
        public string IF_OTHERS_SPECIFY { get; set; }
        public string NO_OF_PARTICIPANTS { get; set; }
        public string ENTRY_DTATE { get; set; }
        public string MONTH_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FIRST_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string CLASS_NAME { get; set; }

        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string DAY_ORDER_MONTH { get; set; }
        public string STAFF_CODE { get; set; }
    }

    public class IQAC_WORK_DONE_DATA
    {
        public string IQAC_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string UNIT { get; set; }
        public string ALLOTTED { get; set; }
        public string COVERED { get; set; }
        public string ENTRY_DTATE { get; set; }
        public string MONTH_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FIRST_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string CLASS_NAME { get; set; }

        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string DAY_ORDER_MONTH { get; set; }
        public string STAFF_CODE { get; set; }
    }

    public class IQAC_EXAMINATION_DUTIES_INTERNAL
    {
        public string IQAC_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string EXAMINATION_DTATE { get; set; }
        public string PAPER_SETTING { get; set; }
        public string INVIGILATORS { get; set; }
        public string PAPER { get; set; }
        public string PAPERS { get; set; }
        public string ASSIGNMENTS { get; set; }
        public string ENTRY_DTATE { get; set; }
        public string MONTH_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FIRST_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string CLASS_NAME { get; set; }

        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string DAY_ORDER_MONTH { get; set; }
        public string STAFF_CODE { get; set; }
    }

    public class IQAC_PROFESSIONAL_ACADEMIC_ACTIVITIES
    {
        public string IQAC_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string DEPT_CODE { get; set; }
        public string ACTIVITY { get; set; }
        public string ACTIVITY_DATE { get; set; }
        public string IF_OTHER_SPECIFY { get; set; }
        public string INSTITUTION_PLACE { get; set; }
        public string ENTRY_DTATE { get; set; }
        public string MONTH_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class IQAC_RESEARCH_PROJECTS
    {
        public string IQAC_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string DEPT_CODE { get; set; }
        public string PROJECT_TYPE { get; set; }
        public string PROJECT_TITLE { get; set; }
        public string DURATION { get; set; }
        public string FUNDING_AGENCY { get; set; }
        public string ENTRY_DTATE { get; set; }
        public string MONTH_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_A
    {
        public string IQAC_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string DEPT_CODE { get; set; }
        public string CO_AUTHORS { get; set; }
        public string PAPER_TITLE { get; set; }
        public string SEMINAR { get; set; }
        public string RESEARCH_DATE { get; set; }
        public string PLACE { get; set; }
        public string ENTRY_DATE { get; set; }
        public string MONTH_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_B
    {
        public string IQAC_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string DEPT_CODE { get; set; }
        public string CO_AUTHORS { get; set; }
        public string PAPER_TITLE { get; set; }
        public string JOURNAL_TITLE { get; set; }
        public string VOLUME_NUMBER_PAGES { get; set; }
        public string UGC_LISTED { get; set; }
        public string ENTRY_DATE { get; set; }
        public string MONTH_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_C
    {
        public string IQAC_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string DEPT_CODE { get; set; }
        public string SEMINAR_TITLE { get; set; }
        public string SEMINAR_DATE { get; set; }
        public string ORGANIZER_AND_VENUE { get; set; }
        public string ENTRY_DATE { get; set; }
        public string MONTH_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_D
    {
        public string IQAC_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string DEPT_CODE { get; set; }
        public string WORKSHOP_TITLE { get; set; }
        public string WORKSHOP_DATE { get; set; }
        public string ORGANIZER_AND_VENUE { get; set; }
        public string ENTRY_DATE { get; set; }
        public string MONTH_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_E
    {
        public string IQAC_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string DEPT_CODE { get; set; }
        public string BOOK_TITLE { get; set; }
        public string PUBLISHER_PLACE { get; set; }
        public string VOLUME_PAGES { get; set; }
        public string ISBN_NO { get; set; }
        public string ENTRY_DATE { get; set; }
        public string MONTH_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class IQAC_QUESTION
    {
        public string IQAC_QUESTION_ID { get; set; }
        public string IQAC_QUESTIONS { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class IQAC_CURRICULAR_COORDINATOR
    {
        public string IQAC_ID { get; set; }
        public string COMMITTEE_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string STAFF_NAME { get; set; }
        public string STAFF_ID { get; set; }
        public string ACDEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class STF_LESSON_PLAN
    {
        public string IQAC_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string CYCLE { get; set; }
        public string UNIT { get; set; }
        public string TOPIC { get; set; }
        public string STATUS { get; set; }
        public string ENTRY_DTATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

        public string STAFF_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string SEMESTER_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string DEPARTMENT { get; set; }
        public string CLASS_NAME { get; set; }
        public string CLASS_ID { get; set; }
        public string TEACHING_METHOD { get; set; }

    }

    public class IQAC_SUBJECT_ASSOCIATION
    {
        public string SUBJECT_ASSOCIATION_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string STAFF_CODE { get; set; }
        public string ASSOCIATION_NAME { get; set; }
        public string POSISION_HELD { get; set; }
        public string PERIOD { get; set; }
        public string PERIOD_TO { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string MONTH_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FILE_PATH { get; set; }
    }

    public class IQAC_HONOURS
    {
        public string HONOR_ID { get; set; }
        public string HONOR_TYPE_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string TITLE_OF_TALK { get; set; }
        public string PROGRAMME_TITLE { get; set; }
        public string INSTITION_NAME { get; set; }
        public string PLACE { get; set; }
        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }

        public string ENTRY_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string MONTH_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FILE_PATH { get; set; }
        public string DEPARTMENT { get; set; }

    }

    public class IQAC_AWARDS
    {
        public string AWARD_ID { get; set; }
        public string AWARD_TITLE { get; set; }
        public string STAFF_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string YEAR { get; set; }
        public string AWARDED_BY { get; set; }
        public string LEVEL_ID { get; set; }
        public string LEVEL { get; set; }
        public string FILE_PATH { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string MONTH_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string DEPARTMENT { get; set; }

    }

    public class IQAC_MEMBERSHIP
    {
        public string MEMBERSHIP_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string MEMBERSHIP_TYPE_ID { get; set; }
        public string MEMBERSHIP_TYPE { get; set; }
        public string BOARD_NAME { get; set; }
        public string COLLEGE_UNIVERSITY { get; set; }
        public string PERIOD { get; set; }
        public string PERIOD_TO { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string MONTH_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FILE_PATH { get; set; }
        public string DEPARTMENT { get; set; }

    }

    public class IQAC_STAFF_ACTIVITY
    {
        public string IQAC_STAFF_ACTIVITY_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string ACTIVITY_ID { get; set; }
        public string PROGRAMME_TYPE_ID { get; set; }
        public string ACTIVITY_DATE { get; set; }
        public string ACTIVITY_DATE_TO { get; set; }
        public string ACTIVITY_TITLE { get; set; }
        public string ORGANIZER { get; set; }
        public string PLACE { get; set; }
        public string LEVEL_ID { get; set; }
        public string PAPER_TITLE { get; set; }
        public string ISBN_NUMBER { get; set; }
        public string PUBLISHED_IN_PROCEEDING { get; set; }
        public string PRESENTING_AUTHOR { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string MONTH_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FILE_PATH { get; set; }
        public string DEPARTMENT { get; set; }

    }

    public class IQAC_PAPER_PUBLICATION
    {
        public string IQAC_PAPER_PUBLICATION_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string PAPER_TITLE { get; set; }
        public string JOURNEL_NAME { get; set; }
        public string PUBLICATION_YEAR { get; set; }
        public string PUBLICATION_VOLUME { get; set; }
        public string ISSUE_NUMBER { get; set; }
        public string UGC_NUMBER { get; set; }
        public string OTHERS { get; set; }
        public string PAGE_NUMBER { get; set; }
        public string ISSN_NUMBER { get; set; }
        public string ISBN_NUMBER { get; set; }
        public string SCOPUS_ID { get; set; }
        public string WEB_OF_SCIENCE { get; set; }
        public string UGC_LISTED { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string IMPACT_FACT { get; set; }
        public string CITATIONS { get; set; }
        public string CITATION_INDEX { get; set; }
        public string CORRESPONDING_AUTHOR { get; set; }
        public string FIRST_AUTHOR { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string MONTH_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FILE_PATH { get; set; }
        public string DEPARTMENT { get; set; }
    }

    public class IQAC_BOOK_PUBLICATION
    {
        public string IQAC_BOOK_PUBLICATION_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string CHAPTER { get; set; }
        public string BOOK_TITLE { get; set; }
        public string CHAPTER_TITLE { get; set; }
        public string PUBLISHER { get; set; }
        public string PUBLICATION_YEAR { get; set; }
        public string ISBN_NUMBER { get; set; }
        public string SINGLE_AUTHOR { get; set; }
        public string EDITOR { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string MONTH_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FILE_PATH { get; set; }
        public string DEPARTMENT { get; set; }

    }

    public class Json_Save
    {
        public List<IQAC_TEACHING_SUBJECTS_WORKLOAD> liWorkload { get; set; }
        public List<IQAC_PREPARATION_AND_UPDATION_OF_NOTES> liPreparation { get; set; }
        public List<IQAC_PREPARATION_AND_UPDATION_OF_NOTES> liUPreparation { get; set; }
        public List<IQAC_CO_CURRICULAR_ASPECTS> liCurricularAspects { get; set; }
        public List<IQAC_CO_CURRICULAR_ASPECTS> liUCurricularAspects { get; set; }
        public List<IQAC_EXTENSION_WORKS> liExtensionWorks { get; set; }
        public List<IQAC_PERSONAL_ACADEMIC_ACTIVITY> liPersonalAcademicActivity { get; set; }
        public List<IQAC_WORK_DONE_DATA> liWorkDoneData { get; set; }
        public List<IQAC_EXAMINATION_DUTIES_INTERNAL> liExaminationDutiesInternal { get; set; }
        public List<IQAC_PROFESSIONAL_ACADEMIC_ACTIVITIES> liProfessionalAcademicActivity { get; set; }
        public List<IQAC_RESEARCH_PROJECTS> liResearchProject { get; set; }
        public List<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_A> liResearchAcademicContributionA { get; set; }
        public List<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_B> liResearchAcademicContributionB { get; set; }
        public List<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_C> liResearchAcademicContributionC { get; set; }
        public List<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_D> liResearchAcademicContributionD { get; set; }
        public List<IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_E> liResearchAcademicContributionE { get; set; }

        public List<STF_LESSON_PLAN> liLessonPlan { get; set; }
        public List<SUP_IQAC_RESOURCE_PERSON> liResourcePerson { get; set; }
        public List<SUP_IQAC_ORGANIZER> liOrganizer { get; set; }

    }

    public class IQAC_PROJECT
    {
        public string IQAC_PROJECT_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string PROJECT_TYPE { get; set; }
        public string PROJECT_TITLE { get; set; }
        public string DURATION { get; set; }
        public string PERIOD { get; set; }
        public string PERIOD_TO { get; set; }
        public string TYPE_OF_FUNDING { get; set; }
        public string FUNDING_AGENCY { get; set; }
        public string AMOUNT { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string MONTH_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string DAY_ORDER_MONTH { get; set; }
        public string FILE_PATH { get; set; }
        public string DEPARTMENT { get; set; }

    }


    public class SUP_IQAC_PROJECT_TYPE
    {
        public string PROJECT_TYPE_ID { get; set; }
        public string PROJECT_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SUP_IQAC_TYPE_OF_FUNDING
    {
        public string TYPE_FUNDING_ID { get; set; }
        public string FUNDING_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }


    public class IQAC_RESEARCH_GUIDENCE
    {
        public string IQAC_RESEARCH_GUDIENCE_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string CATEGORY_ID { get; set; }
        public string CATEGORY { get; set; }

        public string STUDENT_NAME { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string PROJECT_TITLE { get; set; }
        public string REGISTRATION_YEAR { get; set; }
        public string COMPLITION_YEAR { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string MONTH_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

        public string STAFF_CODE { get; set; }
        public string DAY_ORDER_MONTH { get; set; }
        public string FILE_PATH { get; set; }
        public string DEPARTMENT { get; set; }


    }

    public class SUP_IQAC_PHD_RESEARCH_CATEGORY
    {
        public string IQAC_CATEGORY_ID { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }



    public class IQAC_PHD_GUIDENCE
    {
        public string IQAC_PHD_GUDIENCE_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string CATEGORY_ID { get; set; }
        public string STUDENT_NAME { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string PROJECT_TITLE { get; set; }
        public string REGISTRATION_YEAR { get; set; }
        public string STATUS { get; set; }
        public string GUDIENCE_DATE { get; set; }
        public string MINI_GUDIENCE_DATE { get; set; }
        public string MAXI_GUDIENCE_DATE { get; set; }

        public string ENTRY_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string MONTH_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string DAY_ORDER_MONTH { get; set; }
        public string FILE_PATH { get; set; }
        public string DEPARTMENT { get; set; }

    }

    public class SUP_IQAC_PHD_GUIDENCE_CATEGORY
    {
        public string IQAC_CATEGORY_ID { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_IQAC_STATUS
    {
        public string IQAC_STATUS_ID { get; set; }
        public string STATUS_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class DEPT_DAILY_ACTIVITY
    {
        public string ACTIVITY_ID { get; set; }
        public string ACTIVITY { get; set; }
        public string ACTIVITY_DATE { get; set; }
        public string STAFF_ID { get; set; }
        public string FILE_PATH { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string DEPARTMENT { get; set; }

    }

    public class IQAC_ASSOCIATION_ACTIVITIES
    {
        public string IQAC_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string SHIFT_ID { get; set; }
        public string ACTIVITY_ID { get; set; }
        public string IF_OTHERS_SPECIFY { get; set; }
        public string ACTIVITY_DATE { get; set; }
        public string DETAILS { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FILE_PATH { get; set; }
        public string ENTRY_DATE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string SHIFT_NAME { get; set; }
        public string ACTIVITY_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string TO_DATE { get; set; }

        public string RESOURCE_PERSON { get; set; }


    }

    public class IQAC_SEMINOR_WORKSOP_ORGANIZED
    {
        public string IQAC_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string PROGRAMME_TYPE_ID { get; set; }
        public string IF_OTHER_SPECIFY { get; set; }
        public string ORGANIZED_DATE { get; set; }
        public string TO_DATE { get; set; }

        public string TITLE { get; set; }
        public string LEVEL_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FILE_PATH { get; set; }
        public string ENTRY_DATE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string PROGRAMME_TYPE { get; set; }
        public string LEVEL { get; set; }
        public string DEPARTMENT { get; set; }

        public string RESOURCE_PERSON { get; set; }

    }

    public class SUP_IQAC_RESOURCE_PERSON
    {
        public string RESOURCE_PERSON_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string GUEST_ID { get; set; }
        public string PRIMARY_KEY_ID { get; set; }
        public string PERSON_NAME { get; set; }
        public string DESIGNATION { get; set; }
        public string INSTITUTION { get; set; }
        public string PLACE { get; set; }
        public string TOPIC { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string DEPARTMENT { get; set; }

    }

    public class SUP_IQAC_ACTIVITY
    {
        public string ACTIVITY_ID { get; set; }
        public string ACTIVITY_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_IQAC_GUEST
    {
        public string GUEST_ID { get; set; }
        public string GUEST_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_IQAC_ORGANIZER
    {
        public string SUP_ORGANIZER_ID { get; set; }
        public string ORGANIZER_ID { get; set; }
        public string PRIMARY_ID { get; set; }
        public string ORGANIZER_NAME { get; set; }
        public string ORGANIZER { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class IQACReport
    {
        public string STAFF_ID { get; set; }
        public string STAFF_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string QUESTION_ID { get; set; }
        public string STATUS { get; set; }
        public string MONTH_ID { get; set; }
        public string SEMESTER_TYPE_ID { get; set; }
    }
}

