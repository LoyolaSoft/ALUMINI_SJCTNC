using CMS.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModel.ViewModel
{
    public class CertificateCoursesViewModel
    {
        #region SelectList
        public SelectList SemesterType { get; set; }
        public SelectList SettingList { get; set; }
        
        public SelectList ShiftList { get; set; }
        public SelectList Class { get; set; }
        public SelectList Course { get; set; }
        public SelectList Student { get; set; }
        #endregion

        #region Properties
        public string sErrorMessage { get; set; }
        public string sSettingId { get; set; }
        public string sMessage { get; set; }
        #endregion

        #region Model
        public CERTIFICATE_COURSE_ROOT ObjCourseRoot = new CERTIFICATE_COURSE_ROOT();
        public CERTIFICATE_CLASS_COURSE ObjClassCourse = new CERTIFICATE_CLASS_COURSE();
        public CERTIFICATE_STUDENT_REGISTRATION ObjStudentRegistration = new CERTIFICATE_STUDENT_REGISTRATION();
        public CERTIFICATE_COURSE_SETTINGS ObjCertificateCourseSetting = new CERTIFICATE_COURSE_SETTINGS();
        #endregion

        #region List
        public List<CERTIFICATE_COURSE_ROOT> liCourseRoot = new List<CERTIFICATE_COURSE_ROOT>();
        public List<CERTIFICATE_CLASS_COURSE> liClassCourse = new List<CERTIFICATE_CLASS_COURSE>();
        public List<CERTIFICATE_STUDENT_REGISTRATION> liStudentRegistration = new List<CERTIFICATE_STUDENT_REGISTRATION>();
        public List<STU_PERSONAL_INFO> liStudent = new List<STU_PERSONAL_INFO>();
        public List<CERTIFICATE_COURSE_SETTINGS> liCertificateCourseSettings = new List<CERTIFICATE_COURSE_SETTINGS>();
        public List<CERTIFICATE_COURSE_SETTING> liCertificateCourseSetiing = new List<CERTIFICATE_COURSE_SETTING>();
        public List<ASSOCIATION_GROUP_CLASS_QUOTA> liAssociationClassCourse = new List<ASSOCIATION_GROUP_CLASS_QUOTA>();
        public List<ASSOCIATION_GROUPS> liAssociation;
        #endregion

    }
}
