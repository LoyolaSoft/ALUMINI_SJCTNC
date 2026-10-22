using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.ViewModel.Model;
using System.Web.Mvc;

namespace CMS.ViewModel.ViewModel
{
    public class AssociationViewModel
    {
        #region SelectList
        public SelectList SemesterType { get; set; }
        public SelectList AssociationGroup { get; set; }
        public SelectList Class { get; set; }
        public SelectList StudentList { get; set; }       
        public SelectList AcademicYearList { get; set; }       
        public string ACADEMIC_YEAR { get; set; }       
        public string CLASS_ID { get; set; }       
        
         
        #endregion

        #region Model
        public ASSOCIATION_GROUP_SETTING objAssociationSetting = new ASSOCIATION_GROUP_SETTING();
        #endregion

        #region List
        public List<ASSOCIATION_GROUP_SETTING> liAssociationSetting = new List<ASSOCIATION_GROUP_SETTING>();
        public List<ASSOCIATION_STUDENT_REGISTRATION> liAssociationRegistration = new List<ASSOCIATION_STUDENT_REGISTRATION>();
        #endregion
    }
}
