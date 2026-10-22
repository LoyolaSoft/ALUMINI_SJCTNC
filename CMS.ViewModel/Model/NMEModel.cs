using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{

    public class NMERegistraion
    {

    }

    public class NMEClassWiseStudent
    {
        public string ADMISSION_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string COURSE_TITLE { get; set; }
        public string CLASS_NAME { get; set; }

    }
    public class NMEClassList
    {
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
    }
    #region NME Course Registration
    public class NMECourseRegistration
    {
        public string REGISTRATION_ID { get; set; }
        [DisplayName("Course Type")]
        public string COURSE_TYPE { get; set; }
        [DisplayName("Course Code")]
        public string COURSE_CODE { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_TYPE_ID { get; set; }
        [DisplayName("Course Title")]
        public string COURSE_TITLE { get; set; }
        [DisplayName("Semester")]
        public string SEMESTER_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
    }
    public class FetchNMECourseRegistration
    {
        public string COURSE_TYPE { get; set; }
        public string COURSE_TYPE_ID { get; set; }
    }
    public class SaveNMECourseRegistration
    {
        public string REGISTRATION_ID { get; set; }
        [DisplayName("Course Type")]
        public string COURSE_TYPE { get; set; }
        public string COURSE_TYPE_ID { get; set; }
        [DisplayName("Course")]
        public string COURSE_ID { get; set; }
        public string COURSE_ROOT_ID { get; set; }
    }
    public class NMERegisteredStudent
    {
        [DisplayName("Course")]
        public string COURSE_CODE { get; set; }
        public string COURSE_ID { get; set; }
        public string REGISTRATION_ID { get; set; }
        [DisplayName("Student")]
        public string STUDENT_ID { get; set; }
        public string SEMESTER { get; set; }
        [DisplayName("Student")]
        public string FIRST_NAME { get; set; }
        [DisplayName("Student")]
        public string ROLL_NO { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_CODE { get; set; }
        public string S_CLASS_ID { get; set; }
        public string SUMOFQUTOA { get; set; }
        public string ALLOTED_SEATS { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SETTINGS_NAME { get; set; }
        public string SELECTED_CLASS { get; set; }
        public string COURSE_TITLE { get; set; }
        public string STATUS { get; set; }
        public string SHIFT_ID { get; set; }
        
    }
    public class SaveNMERegisteredStudent
    {
        public string REGISTRATION_ID { get; set; }
        [DisplayName("Student")]
        public string STUDENT_ID { get; set; }
        [DisplayName("Student")]
        public string ROLL_NO { get; set; }
        public string CLASS_ID { get; set; }
        [DisplayName("Course")]
        public string COURSE_ID { get; set; }
        public string SETTINGS_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SEMESTER { get; set; }
        public string S_CLASS_ID { get; set; }
    }
    public class DeleteNMECourseRegistration
    {
        public string REGISTRATION_ID { get; set; }
        public string IS_DELETED { get; set; }
    }
    #endregion
    #region NME Settings
    public class NMESettings
    {
        public string SETTINGS_ID { get; set; }
        [DisplayName("Setting Name")]
        public string SETTINGS_NAME { get; set; }
        [DisplayName("Class")]
        public string CLASS_CODE { get; set; }
        [DisplayName("Date From")]
        public string DATE_FROM { get; set; }
        [DisplayName("Date To")]
        public string DATE_TO { get; set; }
        [DisplayName("Academic Year")]
        public string ACADEMIC_YEAR { get; set; }
        [DisplayName("Semester")]
        public string SEMESTER_NAME { get; set; }
        [DisplayName("Registration Open")]
        public string IS_ALLOWED { get; set; }
    }
    public class SaveNMESettings
    {
        public string SETTINGS_ID { get; set; }
        [DisplayName("Setting Name")]
        public string SETTINGS_NAME { get; set; }
        [DisplayName("Class")]
        public string CLASS_ID { get; set; }
        [DisplayName("Date From")]
        public string DATE_FROM { get; set; }
        [DisplayName("Date To")]
        public string DATE_TO { get; set; }
        [DisplayName("Academic Year")]
        public string ACADEMIC_YEAR { get; set; }
        [DisplayName("Semester")]
        public string SEMESTER { get; set; }
        public string IS_ALLOWED { get; set; }
        public string SHIFT { get; set; }
    }
    public class DeleteNMESettings
    {
        public string SETTINGS_ID { get; set; }
        public string IS_DELETED { get; set; }
    }
    #endregion
    #region NME Class Course
    public class NMEClassCourse
    {
        public string NME_CLASS_COURSE_ID { get; set; }
        [DisplayName("Course Code")]
        public string COURSE_CODE { get; set; }
        [DisplayName("Quota")]
        public string QUOTA { get; set; }
        [DisplayName("Course Title")]
        public string COURSE_TITLE { get; set; }
        [DisplayName("Setting Name")]
        public string SETTINGS_NAME { get; set; }
        public string SETTINGS_ID { get; set; }
        [DisplayName("Class")]
        public string CLASS_ID { get; set; }
        [DisplayName("Class")]
        public string CLASS_CODE { get; set; }
        public string COURSE_ID { get; set; }
        public string SELECTED { get; set; }
    }
    public class BindNMEClassCourseQuota
    {
        public string COURSE_TITLE { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string CLASS_ID { get; set; }
        public string SETTINGS_ID { get; set; }
        public string TOTALQUOTA { get; set; }
        public string SUMOFQUOTA { get; set; }
        public string CLASSQUOTA { get; set; }
        public string REMAININGQUOTA { get; set; }
        public string STUDENT_COUNT { get; set; }
        public string QUOTA { get; set; }
        public string ALLOTED_SEATS { get; set; }
        public string SHIFT { get; set; }

    }
    public class JsonNMEClassCourse
    {
        public List<SaveNMEClassCourse> NMEClassCourse { get; set; }
    }
    public class JsonNMECourseRegistration
    {
        public List<SaveNMEClassCourse> NMEClassCourse { get; set; }
    }
    public class SaveNMEClassCourse
    {
        public string NME_CLASS_COURSE_ID { get; set; }
        public string NME_CLASS_COURSE_QUOTA_ID { get; set; }
        [DisplayName("Course Title")]
        public string COURSE_ID { get; set; }
        [DisplayName("Setting Name")]
        public string SETTINGS_ID { get; set; }
        [DisplayName("Class")]
        public string CLASS_ID { get; set; }
        [DisplayName("Class")]
        public string CLASS_CODE { get; set; }
        public string QUOTA { get; set; }
    }
    public class FetchNMEModel
    {
        public string CLASS { get; set; }
        public string COURSE { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
    }
    public class DeleteNMEClassCourse
    {
        public string NME_CLASS_COURSE_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
    }
    #endregion
    #region NME Course Allotment
    public class NMECourseAllotment
    {
        public string ALLOTMENT_ID { get; set; }
        [DisplayName("Shift")]
        public string SHIFT_NAME { get; set; }
        [DisplayName("Course Code")]
        public string COURSE_CODE { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        [DisplayName("Course Title")]
        public string COURSE_TITLE { get; set; }
        [DisplayName("Alloted Seats")]
        public string ALLOTED_SEATS { get; set; }
        [DisplayName("Academic Year")]
        public string ACADEMIC_YEAR { get; set; }
        public string QUOTA { get; set; }
        public string STUDENT_COUNT { get; set; }
    }
    public class SaveNMECourseAllotment
    {
        public string ALLOTMENT_ID { get; set; }
        [DisplayName("Shift")]
        public string SHIFT_ID { get; set; }
        [DisplayName("Course Code")]
        public string COURSE_ID { get; set; }
        [DisplayName("Alloted Seats")]
        public string ALLOTED_SEATS { get; set; }
        [DisplayName("Academic Year")]
        public string ACADEMIC_YEAR { get; set; }
    }
    public class DeleteNMECourseAllotment
    {
        public string ALLOTMENT_ID { get; set; }
        public string IS_DELETED { get; set; }
    }
    #endregion
    #region NME Class Course Quota
    public class NMEClassCourseQuota
    {
        public string NME_CLASS_COURSE_QUOTA_ID { get; set; }
        [DisplayName("Class")]
        public string CLASS_ID { get; set; }
        [DisplayName("Course Code")]
        public string COURSE_CODE { get; set; }
        [DisplayName("Course Title")]
        public string COURSE_TITLE { get; set; }
        [DisplayName("Alloted Seats")]
        public string QUOTA { get; set; }
        [DisplayName("Setting Name")]
        public string SETTINGS_NAME { get; set; }
    }
    public class SaveNMEClassCourseQuota
    {
        public string NME_CLASS_COURSE_QUOTA_ID { get; set; }
        [DisplayName("Class")]
        public string CLASS_ID { get; set; }
        [DisplayName("Course Code")]
        public string COURSE_ID { get; set; }
        [DisplayName("Alloted Seats")]
        public string QUOTA { get; set; }
        [DisplayName("Setting Name")]
        public string SETTINGS_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class DeleteNMEClassCourseQuota
    {
        public string NME_CLASS_COURSE_QUOTA_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
    }
    #endregion
}
