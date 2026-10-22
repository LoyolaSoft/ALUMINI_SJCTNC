using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModel.Model
{
    public class MeetingCreation
    {
        public string MEETING_ID { get; set; }
        public string MEETING_NAME { get; set; }
        public string MEETING_HOST { get; set; }
        public string RESPONSIBLE_PERSON { get; set; }
        public string REPORT_FILE { get; set; }
        public string DATE { get; set; }
        public string START_TIME { get; set; }
        public string END_TIME { get; set; }
        public string ARRIVAL_BEFORE { get; set; }
        public string ATTENDEES { get; set; }
        [AllowHtml]
        public string AGENDA { get; set; }
        public string STAFF_CATEGORY { get; set; }
        public string FILE_UPLOAD { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string STAFF_ID { get; set; }
        public string STAFF_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string UserId { get; set; }
    }
}
