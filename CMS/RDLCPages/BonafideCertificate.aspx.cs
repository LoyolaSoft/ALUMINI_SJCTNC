using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.Utility;
using System.Data;

namespace CMS.RDLCPages
{
    public partial class BonafideCertificate : System.Web.UI.Page
    {
        string sStudentIds = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                sStudentIds = Request.QueryString["sStudentIds"].ToString();
                RenderReport();
            }
        }
        public void RenderReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            DataTable dtBonafideCertificate = new DataTable();
            using (ViewModel.ViewModel.StudentViewModel ObjViewModel = new ViewModel.ViewModel.StudentViewModel())
            {
                dtBonafideCertificate = ObjViewModel.FetchBonafideCertificate(sStudentIds, sAcademicYear).DataSource.Table;
            }
            rvBonafideCertificate.Reset();
            rvBonafideCertificate.LocalReport.EnableExternalImages = true;
            rvBonafideCertificate.LocalReport.ReportPath = Server.MapPath("~/CMSReports/StudentBonafideCertificate.rdlc");
            rvBonafideCertificate.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("BonafideCertificate", dtBonafideCertificate));
        }
    }
}