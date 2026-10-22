using System;

namespace CMS.Models
{
    public class AlumniElectionRegistration
    {
        public int ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string NAME { get; set; }
        public string MOBILE_NO { get; set; }
        public int? PROGRAMME { get; set; } // Stores PROGRAMME_ID
        public string PROGRAMME_NAME { get; set; }
        public string YEAR_OF_PASSING { get; set; }
        public string ELECTION_STATUS { get; set; } // "APPROVED" or "REJECTED"
        public string IP_ADDRESS { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public int IS_DELETED { get; set; }
        public string PROGRAMME_CODE { get; set; }
        public string REGISTRATION_NO { get; set; }
        public string SERIAL_NO { get; set; }
    }

    public class ProgrammeDropdownItem
    {
        public int PROGRAMME_ID { get; set; }
        public string PROGRAMME_NAME { get; set; }
    }

    public class AlumniElectionSetting
    {
        public int ID { get; set; }
        public string SETTING_NAME { get; set; }
        public DateTime? ELECTION_DATE { get; set; }
        public DateTime? LAST_DATE_SUBMISSION { get; set; }
        public string NOTICE_PDF_PATH { get; set; }
        public string NOTES { get; set; }
        public string ELECTION_PLACE { get; set; }
        public string ELECTION_TIME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public int IS_ACTIVE { get; set; }
        public int IS_DELETED { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
    }

    public class ProgrammeWiseElectionReportItem
    {
        public string PROGRAMME_NAME { get; set; }
        public int TOTAL_COUNT { get; set; }
        public int APPROVED_COUNT { get; set; }
        public int REJECTED_COUNT { get; set; }
        public double ATTENDANCE_PERCENTAGE
        {
            get
            {
                return TOTAL_COUNT > 0 ? Math.Round(((double)APPROVED_COUNT / TOTAL_COUNT) * 100.0, 1) : 0.0;
            }
        }
    }

    public class AlumniElectionCardViewModel
    {
        public AlumniElectionRegistration Registration { get; set; }
        public AlumniElectionSetting Setting { get; set; }
        public string SerialNo { get; set; }
        public string PassNo { get; set; }
    }
}

