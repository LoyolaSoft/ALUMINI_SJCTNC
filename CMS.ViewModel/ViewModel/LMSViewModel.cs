using CMS.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModel.ViewModel
{
    public class LMSViewModel
    {
        public List<LMS_COURSEWISE_ANNOUNCEMENT> liAnnouncement { get; set; }
        public List<LMS_COURSEWISE_ATTACHMENTS> lilmscoursewiseatt = new List<LMS_COURSEWISE_ATTACHMENTS>();
        public List<LMS_COURSEWISE_ATTACHMENTS> liCoursesinfo = new List<LMS_COURSEWISE_ATTACHMENTS>();
        public List<LMS_COURSEWISE_FILES> lilmscoursewisefiles = new List<LMS_COURSEWISE_FILES>();
        public List<LMS_STUDENT_ASSIGNMENT_SUBMISSION> listuassignment = new List<LMS_STUDENT_ASSIGNMENT_SUBMISSION>();
        public List<LMS_ASSIGNMENT_SETTING> liassignmentsetting = new List<LMS_ASSIGNMENT_SETTING>();
        public List<LMS_COURSEWISE_ANNOUNCEMENT> listudent = new List<LMS_COURSEWISE_ANNOUNCEMENT>();
        public List<LMS_ASSIGNMENT_SETTING> listudentgroupinfo = new List<LMS_ASSIGNMENT_SETTING>();
        public List<LMS_COURSEWISE_ATTACHMENTS> listattachmnets = new List<LMS_COURSEWISE_ATTACHMENTS>();
        public List<LMS_STUDENT_ASSIGNMENT_SUBMISSION> limarksGrade = new List<LMS_STUDENT_ASSIGNMENT_SUBMISSION>();
        public List<LMS_ZOOM_MEETINGS> lizoomMeting = new List<LMS_ZOOM_MEETINGS>();
        public List<STU_PERSONAL_INFO> listudentList = new List<STU_PERSONAL_INFO>();
      
        public List<QUIZ_CLASS_COURSE_STAFF> liQuizSetting { get; set; }
        public List<LMS_COURSEWISE_ANNOUNCEMENT> listAnnouncement { get; set; }
        //public List<LMS_COURSEWISE_ANNOUNCEMENT> listAnnouncement { get; set; }
        public List<CP_CLASS_COURSE_STAFF> liClasscoursestaff { get; set; }
        public List<EXAM_REGISTRATION> liExamRegistrationList { get; set; }
        public SelectList UNITLIST { get; set; }
        public SelectList COURSELIST { get; set; }
        public SelectList liStudentLists { get; set; }
        public SelectList SETTING { get; set; }
        public SelectList CLASS_GROUP { get; set; }
        public SelectList CLASSLIST { get; set; }
        public SelectList LMSCLASSNOTES { get; set; }
        public SelectList LMSQUESTIONPAPER { get; set; }
        public SelectList LMSQUESTIONPAPERTYPE { get; set; }
        public SelectList ACDEMICYEAR { get; set; }
        public SelectList staffList { get; set; }
        public SelectList sClassList { get; set; }
        public SelectList sCourseList { get; set; }
        public SelectList sStudentList { get; set; }
        public SelectList ClassList { get; set; }
        public SelectList CourseList { get; set; }
        
    }

    public class Jsonlmsstudentclassgroup
    {
        public List<LMS_ASSIGNMENT_SETTING> lmsstudentclass { get; set; }
    }
    
}
