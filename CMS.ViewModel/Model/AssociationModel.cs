using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{
    class AssociationModel
    {
    }

    public class ASSOCIATION_GROUP_SETTING
    {
        public string ASSOCIATION_GROUP_SETTING_ID { get; set; }
        public string SETTING_NAME { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string SEMESTER_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string SEMESTER_TYPE { get; set; }
    }
    public class ASSOCIATION_GROUPS
    {
        public string ASSOCICATION_GROUP_ID { get; set; }
        public string ASSOCIATION_GROUP { get; set; }
        public string ASSOCIATION_GROUP_CODE { get; set; }
        public string ASSOCIATION_GROUP_ORDER { get; set; }
        public string QUOTA { get; set; }
        public string SEMESTER_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string BALANCE_QUOTA { get; set; }
    }

    public class ASSOCIATION_STUDENT_REGISTRATION
    {
        public string REGISTRATION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string SETTING_ID { get; set; }
        public string TIME_REGISTERED { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ALLOTED_SEATS { get; set; }
        public string SUMOFQUTOA { get; set; }
        public string ASSOCIATION_GROUP { get; set; }
        public string SEMESTER_ID { get; set; }
        public string COURSEAVAILABILITY { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string CLASS_NAME { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string ASSOCICATION_GROUP_ID { get; set; }
        public string BALANCE_QUOTA { get; set; }

    }
}
