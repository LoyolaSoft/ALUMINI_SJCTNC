using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.ViewModel.Model;
using System.Web.Mvc;
using System.ComponentModel;

namespace CMS.ViewModel.ViewModel
{
    public class OnlineClassViewModel
    {
        public List<OnlineClassModel> liOnlineClass { get; set; }
        public List<SUP_HOURS> liHours { get; set; }
        public List<CollegeDetails> liCollegeDetails { get; set; }

        // Select list 
        [DisplayName("Class")]
        public SelectList ClassList { get; set; }
    }
}
