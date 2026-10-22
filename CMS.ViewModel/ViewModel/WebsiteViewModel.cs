using CMS.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModel.ViewModel
{
    public class WebsiteViewModel
    {
        #region Properties
        public bool sFlag = false;

        public string sMessage { get; set; }
        public string sClickDate { get; set; }
        public string sFromDate { get; set; }
        public string sToDate { get; set; }
        #endregion

        #region LinQ List
        public List<WEB_HOME_EVENTS> liWebsiteEvent { get; set; }
        public List<STU_PLACEMENT_NEWS> liPlacementnews { get; set; }
        public List<PLACED_STUDENT_LIST> liInterviewPlacedList { get; set; }
        public List<WEB_MENU_CLICKS_REPORT> liMenuClicksReport { get; set; }
        public List<WEB_MENU_CLICKS_REPORT> liMenuWiseTotalClicksReport { get; set; }
        public List<ROLE_RESPONSIBILITY> liRoleResponsibility { get; set; }
        #endregion

        #region Select List
        public SelectList sCourseList { get; set; }
        public SelectList sClassList { get; set; }
        public SelectList sMonthList { get; set; }
        public SelectList sAcademicYearList { get; set; }
        public SelectList sShiftList { get; set; }
        public SelectList sGuestList { get; set; }
        public SelectList sStaffList { get; set; }
   
        public SelectList sDepartmentList { get; set; }
        public SelectList sAchievement { get; set; }
        public SelectList sSemesterType { get; set; }
        public SelectList sSdgList { get; set; }
        #endregion

        #region Objects

        #endregion
    }
}
