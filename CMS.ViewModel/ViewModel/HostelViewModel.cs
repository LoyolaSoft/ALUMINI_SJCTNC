using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAO;
using CMS.Utility;
using System.Web.Mvc;
using System.ComponentModel;
using CMS.ViewModel.Model;
using System.Data;

namespace CMS.ViewModel.ViewModel
{
    public class HostelViewModel:IDisposable
    {
        #region String Properties

        #endregion

        #region SelectList Properties
        public SelectList ClassList { get; set; }
        public SelectList YearList { get; set; }
        public SelectList StudentList { get; set; }
        public SelectList HostelList { get; set; }
        public SelectList BlockList { get; set; }
        public SelectList AcademicYear { get; set; }
        #endregion

        #region Object && List Classes
        public StudentPersonal ObjStuPersonalInfo = new StudentPersonal();
        public List<SUP_RELATION> liRelation { get; set; }
        public List<Sup_Occupation> liOccupation { get; set; }
        public List<STU_RELATIONS> liStuRelation { get; set; }
        public List<STUDENT_PERSONAL_INFO> liStudentInfo { get; set; }
        public List<HOSTEL_STUDENT_INFO> liHostelStudentList { get; set; }
        public List<CollegeDetails> liCollegeDetails { get; set; }
        #endregion


        #region Disposable Method
        public void Dispose()
        {
            GC.Collect();
        }
        #endregion
    }
}
