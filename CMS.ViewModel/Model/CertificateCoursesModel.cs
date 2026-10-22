using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{
    public class CertificateCoursesModel
    {
    }


    public class CERTIFICATE_COURSE_ROOT
    {
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string SEMESTER_ID { get; set; }
        public string COURSE_ORDER { get; set; }
        public string QUOTA { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string SETTINGS_ID { get; set; }
        public string SELECTED { get; set; }
        public string CLASS_NAME { get; set; }
        public string STUDENT_COUNT { get; set; }
        public string SETTING_ID { get; set; }
        public string BALANCE_QUOTA { get; set; }
        public string USED_QUOTA { get; set; }
    }

    public class CERTIFICATE_CLASS_COURSE
    {
        public string CLASS_COURSE_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string SETTING_ID { get; set; }
        public string QUOTA { get; set; }
        
    }

    public class CERTIFICATE_STUDENT_REGISTRATION
    {
        public string REGISTRATION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string TIME_REGISTERED { get; set; }
        public string SEMESTER { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string COURSE_TITLE { get; set; }
        public string SETTING_ID { get; set; }
    }
    public class CERTIFICATE_COURSE_SETTINGS
    {
        public string SETTINGS_ID { get; set; }
        public string SETTING_ID { get; set; }
        public string SETTINGS_NAME { get; set; }
        public string SETTING_NAME { get; set; }
        public string CLASS_CODE { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string IS_ALLOWED { get; set; }
        public string CLASS_ID { get; set; }
        public string TEMP_ID { get; set; }
        public string SEMESTER { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string P_LEVEL { get; set; }
        public string TIME_FROM { get; set; }
        public string TIME_TO { get; set; }
        public string SHIFT { get; set; }
    }

    public class CERTIFICATE_COURSE_SETTING
    {
        public string SETTING_ID { get; set; }
        public string SETTING_NAME { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string SEMESTER_TYPE_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string SEMESTER_TYPE { get; set; }
    }
    public class ASSOCIATION_GROUP_CLASS_QUOTA
    {
        public string ASSOCIATION_GROUP_CLASS_QUOTA_ID { get; set; }
        public string ASSOCIATION_GROUP_SETTING_ID { get; set; }
        public string ASSOCICATION_GROUP_ID { get; set; }
        public string ASSOCIATION_GROUP { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string SETTING_ID { get; set; }
        public string QUOTA { get; set; }

    }
}
