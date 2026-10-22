using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CMS.ViewModel.Model;

namespace CMS.ViewModel.ViewModel
{
    public class CommunicationViewModel
    {
        public SelectList ClassList { get; set; }
        public SelectList DepartmentList { get; set; }
        public SelectList Programme { get; set; }
        public SelectList StaffType { get; set; }
        public SelectList StaffCategoryList { get; set; }
        public SelectList RoleList { get; set; }
        public SelectList messageType { get; set; }
        public List<Student_Personal_Info> LIStudentDetails { get; set; }
        public List<Student_Personal_Info> LIStaffDetails { get; set; }
        public List<SENT_SMS_2017> lstSentMessage { get; set; }
        public List<SENT_SMS_2017> LISentMessage = new List<SENT_SMS_2017>();
        public List<SENT_SMS_LIST_2017> lstSentMessageList { get; set; }
        public List<SENT_SMS_LIST_2017> LISentMessageList = new List<SENT_SMS_LIST_2017>();
        public List<SMSBalence> LiSmsBalance = new List<SMSBalence>();
        public string HiddenFiled { get; set; }

        public bool rBtnStudent { get; set; }
        public bool rBtnStaff { get; set; }
        public bool rBtnBoth { get; set; }
        public bool rBtnAll { get; set; }
        public bool rBtnIndividual { get; set; }
        public bool txtCredits { get; set; }
        public bool chkCC { get; set; }
        public bool chkAddStaff { get; set; }
        public bool btnForward { get; set; }
        public bool btnBackward { get; set; }
    }
}
