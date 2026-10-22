using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CMS.Utility;
using CMS.ViewModel.ViewModel;
using Microsoft.Reporting.WebForms;

namespace CMS.RDLCPages
{
    public partial class TransferCertificateForAlumini : System.Web.UI.Page
    {
        string sAdmissionNo = string.Empty;
        string AcademicYear = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                sAdmissionNo = Request.QueryString["sAdmissionNo"].ToString();
                RenderReport();
            }
        }

        public void RenderReport()
        {
            DataTable dtCollegeInfo = new DataTable();
            DataTable dtTransferCertificate = new DataTable();
            DataValue dv = new DataValue();
            if (Session[Common.SESSION_VARIABLES.COLLEGE_INFO] != null)
            {
                dtCollegeInfo = (DataTable)Session[Common.SESSION_VARIABLES.COLLEGE_INFO];
                //  sCourseId = (Session[Common.SESSION_VARIABLES.COURSE_ID] != null) ? Session[Common.SESSION_VARIABLES.COURSE_ID].ToString() : "0";
            }
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]!=null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            StudentViewModel objViewModel = new StudentViewModel();
            dv.Clear();
            dv.Add(Common.STU_PERSONAL_INFO.ADMISSION_NO, sAdmissionNo, EnumCommand.DataType.String);
            dtTransferCertificate = objViewModel.BindStudentsForOldTC(dv,sAcademicYear).DataSource.Table;
            rvTransferCertificateForalumini.Reset();
            rvTransferCertificateForalumini.LocalReport.EnableExternalImages = true;
            rvTransferCertificateForalumini.LocalReport.ReportPath = Server.MapPath("~/CMSReports/TransferCertificates.rdlc");
            rvTransferCertificateForalumini.LocalReport.DataSources.Add(new ReportDataSource("CollegeInfo", dtCollegeInfo));
            rvTransferCertificateForalumini.LocalReport.DataSources.Add(new ReportDataSource("TransferCertifcate", dtTransferCertificate));
        }
    }
}